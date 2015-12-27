/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO;
using PeterO.Mail;

namespace PeterO.Mail.Transforms {
  internal sealed class BEncodingStringTransform : IByteReader {
    private static readonly int[] Alphabet = { -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
      52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
      41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

    private readonly string input;
    private int inputIndex;
    private readonly byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    public BEncodingStringTransform(String input) {
      if (input == null) {
        throw new ArgumentNullException("input");
      }
      this.input = input;
      this.buffer = new byte[4];
    }

    private void ResizeBuffer(int size) {
      this.bufferCount = size;
      this.bufferIndex = 0;
    }

    public int ReadByte() {
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
      var value = 0;
      var count = 0;
      int c;
      while (count < 4) {
        c = (this.inputIndex < this.input.Length) ?
          this.input[this.inputIndex++] : -1;
        if (c < 0) {
          // End of stream
          if (count == 1) {
            // Not supposed to happen;
            // invalid number of base64 characters, so
            // return the ASCII substitute character
            return 0x1a;
          }
          if (count == 2) {
            --this.inputIndex;
            value <<= 12;
            return (byte)((value >> 16) & 0xff);
          }
          if (count == 3) {
            --this.inputIndex;
            value <<= 6;
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)((value >> 8) & 0xff);
            return (byte)((value >> 16) & 0xff);
          }
          return -1;
        }
        if (c >= 0x80) {
          // ignore this character
        } else {
          c = Alphabet[c];
          // non-base64 characters are ignored
          if (c >= 0) {
            value <<= 6;
            value |= c;
            ++count;
          }
        }
      }
      this.ResizeBuffer(2);
      this.buffer[0] = (byte)((value >> 8) & 0xff);
      this.buffer[1] = (byte)(value & 0xff);
      return (byte)((value >> 16) & 0xff);
    }
  }
}
