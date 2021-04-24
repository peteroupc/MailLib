package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.io.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

  /**
   * This is an internal API.
   */
  final class IdentityEncoder implements ICharacterEncoder {
    public int Encode(int c, IWriter s) {
      if (s == null) {
        throw new NullPointerException("s");
      }
      if (c < 0) {
        return -1;
      }
      c &= 0xff;
      s.write((byte)c);
      return 1;
    }
  }
