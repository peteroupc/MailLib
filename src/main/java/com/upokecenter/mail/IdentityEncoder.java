package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.io.*;

import com.upokecenter.text.*;

    /**
     * An IdentityEncoder.
     */
  final class IdentityEncoder implements ICharacterEncoder
  {
    public int Encode(int c, OutputStream s) throws java.io.IOException {
      if (c < 0) {
 return -1;
}
      c &= 0xff;
      s.write((byte)c);
      return 1;
    }
  }
