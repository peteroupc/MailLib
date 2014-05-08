package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  // A seven-bit transform used for text/plain data
  final class LiberalSevenBitTransform implements ITransform {
    private ITransform input;

    public LiberalSevenBitTransform (ITransform stream) {
      this.input = stream;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int read() {
      int ret = this.input.read();
      if (ret > 0x80 || ret == 0) {
        return '?';
      }
      return ret;
    }
  }
