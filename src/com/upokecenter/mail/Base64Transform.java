package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  final class Base64Transform implements ITransform {
    static final int[] Alphabet = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
      52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
      41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

    private ITransform input;
    private int lineCharCount;
    private boolean lenientLineBreaks;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;
    private int maxLineLength;

    public Base64Transform (ITransform input, boolean lenientLineBreaks) {
 this(input,lenientLineBreaks,76);
    }

    public Base64Transform (ITransform input, boolean lenientLineBreaks, int maxLineLength) {
      this.input = input;
      this.maxLineLength = maxLineLength;
      this.lenientLineBreaks = lenientLineBreaks;
      this.buffer = new byte[4];
    }

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
    private void ResizeBuffer(int size) {
      this.bufferCount = size;
      this.bufferIndex = 0;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
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
      int value = 0;
      int count = 0;
      int lastByte = 0;
      boolean ungetting = false;
      while (count < 4) {
        int c = lastByte = ungetting ? lastByte : this.input.read();
        ungetting = false;
        if (c < 0) {
          // End of stream
          if (count == 1) {
            // Not supposed to happen
            throw new MessageDataException("Invalid number of base64 characters");
          } else if (count == 2) {
            value <<= 12;
            return (byte)((value >> 16) & 0xff);
          } else if (count == 3) {
            value <<= 6;
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)((value >> 8) & 0xff);
            return (byte)((value >> 16) & 0xff);
          }
          return -1;
        } else if (c == 0x0d) {
          c = lastByte = ungetting ? lastByte : this.input.read();
          if (c == 0x0a) {
            this.lineCharCount = 0;
            continue;
          } else {
            ungetting = true;
            if (this.lenientLineBreaks) {
              this.lineCharCount = 0;
              continue;
            }
          }
        } else if (c == 0x0a) {
          if (this.lenientLineBreaks) {
            this.lineCharCount = 0;
            continue;
          }
        } else if (c >= 0x80) {
          // Ignore
        } else {
          c = Alphabet[c];
          if (c >= 0) {
            value <<= 6;
            value |= c;
            ++count;
          }
        }
        if (this.maxLineLength > 0) {
          ++this.lineCharCount;
          if (this.lineCharCount > this.maxLineLength) {
            throw new MessageDataException("Encoded base64 line too long");
          }
        }
      }
      this.ResizeBuffer(2);
      this.buffer[0] = (byte)((value >> 8) & 0xff);
      this.buffer[1] = (byte)(value & 0xff);
      return (byte)((value >> 16) & 0xff);
    }
  }
