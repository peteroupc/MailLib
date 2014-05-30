package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  final class PercentEncodingStringTransform implements ITransform {
    private String input;
    private int inputIndex;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    public PercentEncodingStringTransform (
      String input) {
      this.input = input;
    }

    private void ResizeBuffer(int size) {
      if (this.buffer == null) {
        this.buffer = new byte[size + 10];
      } else if (size > this.buffer.length) {
        byte[] newbuffer = new byte[size + 10];
        System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
        this.buffer = newbuffer;
      }
      this.bufferCount = size;
      this.bufferIndex = 0;
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
      int endIndex = this.input.length();
      while (true) {
        int c = (this.inputIndex < endIndex) ? this.input.charAt(this.inputIndex++) : -1;
        if (c < 0) {
          // End of stream
          return -1;
        } else if (c == 0x0d) {
          // Can't occur in parameter value percent-encoding; replace
          return '?';
        } else if (c == 0x0a) {
          // Can't occur in parameter value percent-encoding; replace
          return '?';
        } else if (c == '%') {
          int b1 = (this.inputIndex < endIndex) ? this.input.charAt(this.inputIndex++) : -1;
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
            return '%';
          }
          int b2 = (this.inputIndex < endIndex) ? this.input.charAt(this.inputIndex++) : -1;
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
            return '%';
          }
          return c;
        } else if ((c < 0x20 && c != 0x09) || c >= 0x7f) {
          // Can't occur in parameter value percent-encoding; replace
          // with the ASCII substitute character
          return 0x1a;
        } else {
          // printable ASCII, space, or tab; return that byte
          // NOTE: Space and tab are included in case we are
          // decoding percent-encoded file names
          return c;
        }
      }
    }
  }
