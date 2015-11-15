package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.util.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class BoundaryCheckerTransform implements ITransform {
    private ITransform input;
    private boolean ungetting;
    private int lastByte;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;
    private boolean started;
    private boolean readingHeaders;
    private boolean hasNewBodyPart;
    private boolean endOfStream;
    private ArrayList<String> boundaries;

    private void ResizeBuffer(int size) {
      this.buffer = (this.buffer == null) ? ((new byte[size + 10])) : this.buffer;
      if (size > this.buffer.length) {
        byte[] newbuffer = new byte[size + 10];
        System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
        this.buffer = newbuffer;
      }
      this.bufferCount = size;
      this.bufferIndex = 0;
    }

    public BoundaryCheckerTransform (ITransform stream) {
      this.input = stream;
      this.boundaries = new ArrayList<String>();
      this.started = true;
    }

    public void PushBoundary(String boundary) {
      this.boundaries.add(boundary);
    }

    public int read() {
      if (this.bufferIndex < this.bufferCount) {
        int ret = this.buffer[this.bufferIndex];
        ++this.bufferIndex;
        if (this.bufferIndex == this.bufferCount) {
          this.bufferCount = 0;
          this.bufferIndex = 0;
        }
        ret &= 0xff;
        return ret;
      }
      if (this.hasNewBodyPart || this.endOfStream) {
        return -1;
      }
int c = this.lastByte = this.ungetting ? this.lastByte :
        this.input.read();
      this.ungetting = false;
      if (this.readingHeaders) {
        return c;
      }
      if (c < 0) {
        this.started = false;
        return c;
      }
      if (c == '-' && this.started) {
        // Check for a boundary
        this.started = false;
    c = this.lastByte = this.ungetting ? this.lastByte :
          this.input.read();
        this.ungetting = false;
        if (c == '-') {
          // Possible boundary candidate
          return this.CheckBoundaries(false);
        }
        this.ungetting = true;
        return '-';
      }
      this.started = false;
      if (c == 0x0d) {
    c = this.lastByte = this.ungetting ? this.lastByte :
          this.input.read();
        this.ungetting = false;
        if (c == 0x0a) {
          // Line break was read
    c = this.lastByte = this.ungetting ? this.lastByte :
            this.input.read();
          this.ungetting = false;
          if (c == -1) {
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            return 0x0d;
          }
          if (c == 0x0d) {
            // Unget the CR, in case the next line is a boundary line
            this.ungetting = true;
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            return 0x0d;
          }
          if (c != '-') {
            this.ResizeBuffer(2);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)c;
            return 0x0d;
          }
    c = this.lastByte = this.ungetting ? this.lastByte :
            this.input.read();
          this.ungetting = false;
          if (c == -1) {
            this.ResizeBuffer(2);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)'-';
            return 0x0d;
          }
          if (c == 0x0d) {
            // Unget the CR, in case the next line is a boundary line
            this.ungetting = true;
            this.ResizeBuffer(2);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)'-';
            return 0x0d;
          }
          if (c != '-') {
            this.ResizeBuffer(3);
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)'-';
            this.buffer[2] = (byte)c;
            return 0x0d;
          }
          // Possible boundary candidate
          return this.CheckBoundaries(true);
        }
        this.ungetting = true;
        return 0x0d;
      }
      return c;
    }

    private int CheckBoundaries(boolean includeCrLf) {
      // Reached here when the "--" of a possible
      // boundary delimiter is read. We need to
      // check boundaries here in order to find out
      // whether to emit the CRLF before the "--".

      boolean done = false;
      while (!done) {
        done = true;
        int bufferStart = 0;
        if (includeCrLf) {
          this.ResizeBuffer(3);
          bufferStart = 3;
          // store LF, '-', and '-' in the buffer in case
          // the boundary check fails, in which case
          // this method will return CR
          this.buffer[0] = 0x0a;
          this.buffer[1] = (byte)'-';
          this.buffer[2] = (byte)'-';
        } else {
          bufferStart = 1;
          this.ResizeBuffer(1);
          this.buffer[0] = (byte)'-';
        }
        // Check up to 72 bytes (the maximum size
        // of a boundary plus 2 bytes for the closing
        // hyphens)
        int c;
        int bytesRead = 0;
        for (int i = 0; i < 72; ++i) {
          c = this.lastByte = this.ungetting ? this.lastByte :
            this.input.read(); this.ungetting = false;
          if (c < 0 || c >= 0x80 || c == 0x0d) {
            this.ungetting = true;
            break;
          }
          // Console.Write("" + ((char)c));
          ++bytesRead;
          this.ResizeBuffer(bytesRead + bufferStart);
          this.buffer[bytesRead + bufferStart - 1] = (byte)c;
        }
        // System.out.println("::" + bytesRead);
        // NOTE: All boundary strings are assumed to
        // have only ASCII characters (with values
        // less than 128). Check boundaries from
        // top to bottom in the stack.
        String matchingBoundary = null;
        int matchingIndex = -1;
        for (int i = this.boundaries.size() - 1; i >= 0; --i) {
          String boundary = this.boundaries.get(i);
          if (!((boundary) == null || (boundary).length() == 0) && boundary.length() <= bytesRead) {
            boolean match = true;
            for (int j = 0; j < boundary.length(); ++j) {
   match &= (boundary.charAt(j) & 0xff) == (int)(this.buffer[j + bufferStart] &
                0xff);
            }
            if (match) {
              matchingBoundary = boundary;
              matchingIndex = i;
              break;
            }
          }
        }
        if (matchingBoundary != null) {
          boolean closingDelim = false;
          // Pop the stack until the matching body part
          // is on top
          while (this.boundaries.size() > matchingIndex + 1) {
            this.boundaries.remove(matchingIndex + 1);
          }
          // Boundary line found
          if (matchingBoundary.length() + 1 < bytesRead) {
            closingDelim |= this.buffer[matchingBoundary.length() +
              bufferStart] == '-' && this.buffer[matchingBoundary.length() + 1 +
              bufferStart] == '-';
          }
          // Clear the buffer, the boundary line
          // isn't part of any body data
          this.bufferCount = 0;
          this.bufferIndex = 0;
          if (closingDelim) {
            // Pop this entry, it's the top of the stack
            this.boundaries.remove(this.boundaries.size() - 1);
            if (this.boundaries.size() == 0) {
              // There's nothing else significant
              // after this boundary,
              // so return now
              this.hasNewBodyPart = false;
              this.endOfStream = true;
              this.bufferCount = 0;
              return -1;
            }
            // Read to end of line. Since this is the last body
            // part, the rest of the data before the next boundary
            // is insignificant
            while (true) {
    c = this.lastByte = this.ungetting ? this.lastByte :
                this.input.read();
              this.ungetting = false;
              if (c == -1) {
                // The body higher up didn't end yet
                throw new MessageDataException("Premature end of message");
              }
              if (c == 0x0d) {
    c = this.lastByte = this.ungetting ? this.lastByte :
                  this.input.read();
                this.ungetting = false;
                if (c == -1) {
                  // The body higher up didn't end yet
                  throw new MessageDataException("Premature end of message");
                }
                if (c == 0x0a) {
                  // Start of new body part
    c = this.lastByte = this.ungetting ? this.lastByte :
                    this.input.read();
                  this.ungetting = false;
                  if (c == -1) {
                    throw new MessageDataException("Premature end of message");
                  }
                  if (c == 0x0d) {
                    // Unget the CR, in case the next line is a boundary line
                    this.ungetting = true;
                    continue;
                  }
                  if (c != '-') {
                    // Not a boundary delimiter
                    continue;
                  }
    c = this.lastByte = this.ungetting ? this.lastByte :
                    this.input.read();
                  this.ungetting = false;
                  if (c == -1) {
                    throw new MessageDataException("Premature end of message");
                  }
                  if (c == 0x0d) {
                    // Unget the CR, in case the next line is a boundary line
                    this.ungetting = true;
                    continue;
                  }
                  if (c != '-') {
                    // Not a boundary delimiter
                    continue;
                  }
                  // Found the next boundary delimiter
                  done = false;
                  break;
                }
                this.ungetting = true;
              }
            }
            if (!done) {
              // Recheck the next line for a boundary delimiter
              continue;
            }
          } else {
            // Read to end of line (including CRLF; the
            // next line will start the headers of the
            // next body part).
            while (true) {
    c = this.lastByte = this.ungetting ? this.lastByte :
                this.input.read();
              this.ungetting = false;
              if (c == -1) {
                throw new MessageDataException("Premature end of message");
              }
              if (c == 0x0d) {
    c = this.lastByte = this.ungetting ? this.lastByte :
                  this.input.read();
                this.ungetting = false;
                if (c == -1) {
                  throw new MessageDataException("Premature end of message");
                }
                if (c == 0x0a) {
                  // Start of new body part
                  this.hasNewBodyPart = true;
                  this.bufferCount = 0;
                  return -1;
                }
                this.ungetting = true;
              }
            }
          }
        }
        // Not a boundary, return CR (the
        // ReadByte method will then return LF,
        // the hyphens, and the other bytes
        // already read)
        return includeCrLf ? 0x0d : '-';
      }
      // Not a boundary, return CR (the
      // ReadByte method will then return LF,
      // the hyphens, and the other bytes
      // already read)
      return includeCrLf ? 0x0d : '-';
    }

    public int BoundaryCount() {
      return this.boundaries.size();
    }

    public void StartBodyPartHeaders() {
      this.readingHeaders = true;
      this.hasNewBodyPart = false;
    }

    public void EndBodyPartHeaders() {
      this.readingHeaders = false;
      this.hasNewBodyPart = false;
      this.started = true;  // in case a boundary delimiter immediately starts
    }

    /**
     * Gets a value indicating whether a new body part was detected.
     * @return True if a new body part was detected; otherwise, false.
     */
    public final boolean getHasNewBodyPart() {
        return this.hasNewBodyPart;
      }
  }
