package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class BoundaryCheckerTransform implements IByteReader {
    private final IByteReader input;
    private final ArrayList<String> boundaries;
    private byte[] innerBuffer;
    private int innerBufferIndex;
    private int innerBufferCount;
    private boolean started;
    private boolean readingHeaders;
    private boolean hasNewBodyPart;
    private boolean endOfStream;

    private void StartInnerBuffer() {
      if (this.innerBufferCount > 0) {
        System.arraycopy(this.innerBuffer, this.innerBufferIndex,
          this.innerBuffer, 0, this.innerBufferCount);
      }
      this.innerBufferIndex = 0;
    }

    private void ResetInnerBuffer() {
      this.innerBufferIndex = 0;
    }

    private void ClearInnerBuffer() {
      this.innerBufferIndex = 0;
      this.innerBufferCount = 0;
    }

    private int InnerBufferRead() {
      int ret;
      if (this.innerBufferIndex < this.innerBufferCount) {
        ret = this.innerBuffer[this.innerBufferIndex];
        ++this.innerBufferIndex;
        if (this.innerBufferIndex == this.innerBufferCount) {
          this.innerBufferCount = 0;
          this.innerBufferIndex = 0;
        }
        ret &= 0xff;
        return ret;
      }
      ret = this.input.read();
      return ret;
    }

    private int InnerBufferReadAndStore() {
      int ret;
      if (this.innerBufferIndex < this.innerBufferCount) {
        ret = this.innerBuffer[this.innerBufferIndex];
        ++this.innerBufferIndex;
        ret &= 0xff;
        return ret;
      }
      ret = this.input.read();
      if (this.innerBuffer == null ||
         this.innerBufferIndex >= this.innerBuffer.length) {
        this.innerBuffer = (this.innerBuffer == null) ? ((new byte[32])) : this.innerBuffer;
        if (this.innerBufferIndex >= this.innerBuffer.length) {
          byte[] newbuffer = new byte[this.innerBuffer.length + 32];
          System.arraycopy(this.innerBuffer, 0, newbuffer, 0, this.innerBuffer.length);
          this.innerBuffer = newbuffer;
        }
      }
      if (ret >= 0) {
        this.innerBuffer[this.innerBufferIndex] = (byte)(ret & 0xff);
        ++this.innerBufferIndex;
        ++this.innerBufferCount;
      }
      return ret;
    }

    public BoundaryCheckerTransform(IByteReader stream, String
        initialBoundary) {
      this.input = stream;
      this.boundaries = new ArrayList<String>();
      this.started = true;
      this.boundaries.add(initialBoundary);
    }

    public int read() {
      if (this.hasNewBodyPart || this.endOfStream) {
        return -1;
      }
      int c = InnerBufferRead();
      if (this.readingHeaders) {
        return c;
      }
      // NOTE: Rest of method only used when
      // reading bodies of multipart body parts
      if (c < 0) {
        this.started = false;
        return c;
      }
      if (c == '-' && this.started) {
        // Check for a boundary at the start
        // of the body part
        StartInnerBuffer();
        c = InnerBufferReadAndStore();
        if (c == '-') {
          // Possible boundary candidate
          return this.CheckBoundaries(PartStart);
        }
        ResetInnerBuffer();
        return '-';
      }
      this.started = false;
      if (c != 0x0d) {
        // Anything other than CR; return that character
        return c;
      }
      // CR might signal next boundary or not
      StartInnerBuffer();
      if (InnerBufferReadAndStore() != 0x0a ||
         InnerBufferReadAndStore() != '-' || InnerBufferReadAndStore() != '-') {
        ResetInnerBuffer();
        return 0x0d;
      }
      // Possible boundary candidate
      return this.CheckBoundaries(PartBody);
    }

    private static final int PartStart = 0;
    private static final int PartBody = 1;
    private static final int PartEpilogue = 2;

    private int CheckBoundaries(int state) {
      // Reached here when the "--" of a possible
      // boundary delimiter is read. We need to
      // check boundaries here in order to find out
      // whether to emit the CRLF before the "--".
      byte[] boundaryBuffer = new byte[75];
      // Check up to 72 bytes (the maximum size
      // of a boundary plus 2 bytes for the closing
      // hyphens)
      int c;
      int lastC = 0;
      int bytesRead = 0;
      String matchingBoundary = null;
      int matchingIndex = -1;
      for (int i = 0; i < 72; ++i) {
        c = InnerBufferReadAndStore();
        if (c < 0 || c >= 0x80 || c == 0x0d) {
          lastC = c;
          break;
        }
        boundaryBuffer[bytesRead++] = (byte)c;
      }
      // System.out.println("::" + bytesRead);
      // NOTE: All boundary strings are assumed to
      // have only ASCII characters (with values
      // less than 128). Check boundaries from
      // top to bottom in the stack.
      for (int i = this.boundaries.size() - 1; i >= 0; --i) {
        String boundary = this.boundaries.get(i);
        if (!((boundary) == null || (boundary).length() == 0) && boundary.length() <= bytesRead) {
          boolean match = true;
          for (int j = 0; j < boundary.length(); ++j) {
            match &= (boundary.charAt(j) & 0xff) == (int)(boundaryBuffer[j] &
                    0xff);
          }
          if (match) {
            matchingBoundary = boundary;
            matchingIndex = i;
            break;
          }
        }
      }
      if (matchingBoundary == null) {
        // No matching boundary
        ResetInnerBuffer();
        return (state==PartBody ||state==PartEpilogue) ? 0x0d : '-';
      }
      boolean closingDelim = false;
      // Pop the stack until the matching body part
      // is on top
      while (this.boundaries.size() > matchingIndex + 1) {
        this.boundaries.remove(matchingIndex + 1);
      }
      // Boundary line found
      if (matchingBoundary.length() + 1 < bytesRead) {
        closingDelim |= (state == PartBody || state == PartEpilogue) &&
          boundaryBuffer[matchingBoundary.length()] == '-' &&
          boundaryBuffer[matchingBoundary.length() + 1] == '-';
      }
      ClearInnerBuffer();
      if (closingDelim) {
        // Pop this entry, it's the top of the stack
        this.boundaries.remove(this.boundaries.size() - 1);
        if (this.boundaries.size() == 0) {
          // There's nothing else significant
          // after this boundary,
          // so return now
          this.hasNewBodyPart = false;
          this.endOfStream = true;
          return -1;
        }
        // Since this is the last body
        // part, the rest of the data before the next boundary
        // is insignificant
        if (state == PartEpilogue) {
          // We're already in an epilogue, so
          // return now to avoid nesting in recursion
          return -1;
        }
        boolean unget = true;
        while (true) {
          c = unget ? lastC : InnerBufferRead();
          unget = false;
          if (c < 0) {
            // The body higher up didn't end yet
            throw new MessageDataException("Premature end of message");
          }
          if (c == 0x0d) {
            // CR might signal next boundary or not
            c = InnerBufferRead();
            if (c == 0x0d || c < 0) {
 unget = true;
}
            if (c == 0x0a) {
              // Start of new body part
              StartInnerBuffer();
              if (InnerBufferReadAndStore() != '-' ||
                InnerBufferReadAndStore() != '-') {
                // No boundary delimiter
                ResetInnerBuffer();
              } else {
                if (this.CheckBoundaries(PartEpilogue) == -1) {
                  return -1;
                }
              }
            }
          }
        }
      } else {
        // Read to end of line (including CRLF; the
        // next line will start the headers of the
        // next body part).
        boolean unget = true;
        while (true) {
          c = unget ? lastC : InnerBufferRead();
          unget = false;
          if (c < 0) {
            // The body higher up didn't end yet
            throw new MessageDataException("Premature end of message");
          }
          if (c == 0x0d) {
            c = InnerBufferRead();
            if (c == 0x0d || c < 0) {
 unget = true;
}
            if (c == 0x0a) {
              // Start of new body part
              this.hasNewBodyPart = true;
              return -1;
            }
          }
        }
      }
    }

    public int BoundaryCount() {
      return this.boundaries.size();
    }

    public void StartBodyPartHeaders() {
      this.readingHeaders = true;
      this.hasNewBodyPart = false;
    }

    public void EndBodyPartHeaders(String boundary) {
      this.readingHeaders = false;
      this.hasNewBodyPart = false;
      this.boundaries.add(boundary);
      this.started = true;  // in case a boundary delimiter immediately starts
    }

    /**
     * Gets a value indicating whether a new body part was detected.
     * @return {@code true} If a new body part was detected; otherwise, {@code
     * false}.
     */
    public final boolean getHasNewBodyPart() {
        return this.hasNewBodyPart;
      }
  }
