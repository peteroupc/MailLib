package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class BEncodingStringTransform implements IByteReader {
    private String input;
    private int inputIndex;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    public BEncodingStringTransform (String input) {
      if (input == null) {
        throw new NullPointerException("input");
      }
      this.input = input;
      this.buffer = new byte[4];
    }

    private void ResizeBuffer(int size) {
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
      int value = 0;
      int count = 0;
      int[] alphabet = Base64Transform.Alphabet;
      while (count < 4) {
        int c = (this.inputIndex < this.input.length()) ?
          this.input.charAt(this.inputIndex++) : -1;
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
          c = alphabet[c];
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
