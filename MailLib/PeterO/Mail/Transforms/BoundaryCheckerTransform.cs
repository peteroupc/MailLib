/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using PeterO;
using PeterO.Mail;

namespace PeterO.Mail.Transforms {
  internal sealed class BoundaryCheckerTransform : IByteReader {
    private const int PartStart = 0;
    private const int PartBody = 1;
    private const int PartEpilogue = 2;
    private readonly IByteReader input;
    private readonly List<string> boundaries;
    private byte[] innerBuffer;
    private int innerBufferIndex;
    private int innerBufferCount;
    private bool started;
    private bool readingHeaders;
    private bool hasNewBodyPart;
    private bool endOfStream;

    private void StartInnerBuffer() {
      if (this.innerBufferCount > 0) {
#if DEBUG
        if (this.innerBufferCount < this.innerBufferIndex) {
          throw new ArgumentException("this.innerBufferCount (" +
            this.innerBufferCount + ") is less than " +
            this.innerBufferIndex);
        }
#endif
        Array.Copy(
          this.innerBuffer,
          this.innerBufferIndex,
          this.innerBuffer,
          0,
          this.innerBufferCount - this.innerBufferIndex);
        this.innerBufferCount -= this.innerBufferIndex;
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
      ret = this.input.ReadByte();
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
      ret = this.input.ReadByte();
      if (this.innerBuffer == null ||
         this.innerBufferIndex >= this.innerBuffer.Length) {
        this.innerBuffer = this.innerBuffer ?? (new byte[32]);
        if (this.innerBufferIndex >= this.innerBuffer.Length) {
          var newbuffer = new byte[this.innerBuffer.Length + 32];
          Array.Copy(this.innerBuffer, newbuffer, this.innerBuffer.Length);
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

    public BoundaryCheckerTransform(
      IByteReader stream,
      string initialBoundary) {
      this.input = stream;
      this.boundaries = new List<string>();
      this.started = true;
      this.boundaries.Add(initialBoundary);
    }

    public int ReadByte() {
      if (this.hasNewBodyPart || this.endOfStream) {
        return -1;
      }
      int c = this.InnerBufferRead();
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
        this.StartInnerBuffer();
        c = this.InnerBufferReadAndStore();
        if (c == '-') {
          // Possible boundary candidate
          return this.CheckBoundaries(PartStart);
        }
        this.ResetInnerBuffer();
        return '-';
      }
      this.started = false;
      if (c != 0x0d) {
        // Anything other than CR; return that character
        return c;
      }
      // CR might signal next boundary or not
      this.StartInnerBuffer();
      if (this.InnerBufferReadAndStore() != 0x0a ||
         this.InnerBufferReadAndStore() != '-' ||
           this.InnerBufferReadAndStore() != '-') {
        this.ResetInnerBuffer();
        return 0x0d;
      }
      // Possible boundary candidate
      return this.CheckBoundaries(PartBody);
    }

    private int CheckBoundaries(int state) {
      // Reached here when the "--" of a possible
      // boundary delimiter is read. We need to
      // check boundaries here in order to find out
      // whether to emit the CRLF before the "--".
      var boundaryBuffer = new byte[75];
      // Check up to 72 bytes (the maximum size
      // of a boundary plus 2 bytes for the closing
      // hyphens)
      int c;
      var lastC = 0;
      var bytesRead = 0;
      string matchingBoundary = null;
      var matchingIndex = -1;
      for (int i = 0; i < 72; ++i) {
        c = this.InnerBufferReadAndStore();
        if (c < 0 || c >= 0x80 || c == 0x0d) {
          lastC = c;
          break;
        }
        boundaryBuffer[bytesRead++] = (byte)c;
      }
      // Console.WriteLine("::" + bytesRead);
      // NOTE: All boundary strings are assumed to
      // have only ASCII characters (with values
      // less than 128). Check boundaries from
      // top to bottom in the stack.
      for (int i = this.boundaries.Count - 1; i >= 0; --i) {
        string boundary = this.boundaries[i];
        if (!String.IsNullOrEmpty(boundary) && boundary.Length <= bytesRead) {
          var match = true;
          for (int j = 0; j < boundary.Length; ++j) {
            match &= (boundary[j] & 0xff) == (int)(boundaryBuffer[j] &
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
        this.ResetInnerBuffer();
        return (state == PartBody || state == PartEpilogue) ? 0x0d : '-';
      }
      var closingDelim = false;
      // Pop the stack until the matching body part
      // is on top
      while (this.boundaries.Count > matchingIndex + 1) {
        this.boundaries.RemoveAt(matchingIndex + 1);
      }
      // Boundary line found
      if (matchingBoundary.Length + 1 < bytesRead) {
        closingDelim |= (state == PartBody || state == PartEpilogue) &&
          boundaryBuffer[matchingBoundary.Length] == '-' &&
          boundaryBuffer[matchingBoundary.Length + 1] == '-';
      }
      this.ClearInnerBuffer();
      if (closingDelim) {
        // Pop this entry, it's the top of the stack
        this.boundaries.RemoveAt(this.boundaries.Count - 1);
        if (this.boundaries.Count == 0) {
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
        var unget = true;
        while (true) {
          c = unget ? lastC : this.InnerBufferRead();
          unget = false;
          if (c < 0) {
            // The body higher up didn't end yet
            throw new MessageDataException("Premature end of message");
          }
          if (c == 0x0d) {
            // CR might signal next boundary or not
            c = this.InnerBufferRead();
            if (c == 0x0d || c < 0) {
              unget = true;
            }
            if (c == 0x0a) {
              // Start of new body part
              this.StartInnerBuffer();
              if (this.InnerBufferReadAndStore() != '-' ||
                this.InnerBufferReadAndStore() != '-') {
                // No boundary delimiter
                this.ResetInnerBuffer();
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
        var unget = true;
        while (true) {
          c = unget ? lastC : this.InnerBufferRead();
          unget = false;
          if (c < 0) {
            // The body higher up didn't end yet
            throw new MessageDataException("Premature end of message");
          }
          if (c == 0x0d) {
            c = this.InnerBufferRead();
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
      return this.boundaries.Count;
    }

    public void StartBodyPartHeaders() {
#if DEBUG
      if (!this.hasNewBodyPart) {
        throw new ArgumentException("doesn't satisfy this.hasNewBodyPart");
      }
      if (this.readingHeaders) {
        throw new ArgumentException("doesn't satisfy !this.readingHeaders");
      }
#endif

      this.readingHeaders = true;
      this.hasNewBodyPart = false;
    }

    public void EndBodyPartHeaders(string boundary) {
#if DEBUG
      if (!this.readingHeaders) {
        throw new ArgumentException("doesn't satisfy this.readingHeaders");
      }
#endif
      this.readingHeaders = false;
      this.hasNewBodyPart = false;
      this.boundaries.Add(boundary);
      this.started = true; // in case a boundary delimiter immediately starts
    }

    /// <summary>Gets a value indicating whether a new body part was
    /// detected.</summary>
    /// <value><c>true</c> If a new body part was detected; otherwise,.
    /// <c>false</c>.</value>
    public bool HasNewBodyPart {
      get {
        return this.hasNewBodyPart;
      }
    }
  }
}
