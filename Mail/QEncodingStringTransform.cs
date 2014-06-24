/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Mail {
  internal sealed class QEncodingStringTransform : ITransform {
    private String input;
    private int inputIndex;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    public QEncodingStringTransform(
      String input) {
      this.input = input;
    }

    private void ResizeBuffer(int size) {
      this.buffer = this.buffer ?? (new byte[size + 10]);
      if (size > this.buffer.Length) {
        var newbuffer = new byte[size + 10];
        Array.Copy(this.buffer, newbuffer, this.buffer.Length);
        this.buffer = newbuffer;
      }
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
      int endIndex = this.input.Length;
      while (true) {
        int c = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
        if (c < 0) {
          // End of stream
          return -1;
        }
        if (c == 0x0d) {
          // Can't occur in the Q-encoding; replace
          // with the ASCII substitute character
          return 0x1a;
        }
        if (c == 0x0a) {
          // Can't occur in the Q-encoding; replace
          // with the ASCII substitute character
          return 0x1a;
        }
        if (c == '=') {
          int b1 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
          c = 0;
          if (b1 >= '0' && b1 <= '9') {
            c <<= 4;
            c |= b1 - '0';
          } else if (b1 >= 'A' && b1 <= 'F') {
            c <<= 4;
            c |= b1 + 10 - 'A';
          } else if (b1 >= 'a' && b1 <= 'f') {
            c <<= 4;
            c |= b1 + 10 - 'a';
          } else {
            --this.inputIndex;
            return '=';
          }
          int b2 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
          if (b2 >= '0' && b2 <= '9') {
            c <<= 4;
            c |= b2 - '0';
          } else if (b2 >= 'A' && b2 <= 'F') {
            c <<= 4;
            c |= b2 + 10 - 'A';
          } else if (b2 >= 'a' && b2 <= 'f') {
            c <<= 4;
            c |= b2 + 10 - 'a';
          } else {
            --this.inputIndex;
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)b1;
            return '=';
          }
          return c;
        }
        if (c <= 0x20 || c >= 0x7f) {
          // Can't occur in the Q-encoding; replace
          // with the ASCII substitute character
          return 0x1a;
        }
        if (c == '_') {
          // Underscore, use space
          return ' ';
        } else {
          // printable ASCII, return that byte
          return c;
        }
      }
    }
  }
}
