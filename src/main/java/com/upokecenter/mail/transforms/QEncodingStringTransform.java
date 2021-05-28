package com.upokecenter.mail.transforms;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/

 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class QEncodingStringTransform implements IByteReader {
    private final String input;
    private int inputIndex;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    public QEncodingStringTransform(String input) {
      this.input = input;
    }

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
        int c = (this.inputIndex < endIndex) ? this.input.charAt(this.inputIndex++) :
          -1;
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
          int b1 = (this.inputIndex < endIndex) ?
            this.input.charAt(this.inputIndex++) : -1;
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
          } else if (b1 == -1) {
            return '=';
          } else {
            --this.inputIndex;
            return '=';
          }
          int b2 = (this.inputIndex < endIndex) ?
            this.input.charAt(this.inputIndex++) : -1;
          if (b2 >= '0' && b2 <= '9') {
            c <<= 4;
            c |= b2 - '0';
          } else if (b2 >= 'A' && b2 <= 'F') {
            c <<= 4;
            c |= b2 + 10 - 'A';
          } else if (b2 >= 'a' && b2 <= 'f') {
            c <<= 4;
            c |= b2 + 10 - 'a';
          } else if (b2 == -1) {
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)b1;
            return '=';
          } else {
            --this.inputIndex;
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)b1;
            return '=';
          }
          return c;
        }
        if (c == 0x20) {
          // Space can't occur in the Q-encoding; output
          // the space character
          // NOTE: Since DecodeEncodedWords already checks
          // a word for spaces/tabs before using this class
          // on that String, it's not possible for this code
          // to reach this point in that case. The other place
          // where this class is used is to adapt file names
          // (notably in Content-Disposition headers where
          // RFC 2047-like encodings occur a lot in practice).
          // For that purpose, however, it is not necessary
          // to follow RFC 2047 (the encoded word specification)
          // strictly.
          return 0x20;
        } else if (c < 0x20 || c >= 0x7f) {
          // Can't occur in the Q-encoding; replace
          // with the ASCII substitute character
          return 0x1a;
        }
        return c == '_' ? 0x20 : c;
      }
    }
  }
