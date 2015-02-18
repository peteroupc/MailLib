package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

  // A seven-bit transform used for text/plain data
  final class LiberalSevenBitTransform implements ITransform {
    private ITransform input;

    public LiberalSevenBitTransform (ITransform stream) {
      this.input = stream;
    }

    public int read() {
      int ret = this.input.read();
      if (ret > 0x80 || ret == 0) {
        // Null or outside the ASCII range; replace with
        // 0x1a, the ASCII SUB (substitute) character
        return 0x1a;
      }
      return ret;
    }
  }
