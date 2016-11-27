package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  // A seven-bit transform used for text/plain data
  public final class LiberalSevenBitTransform implements IByteReader {
    private final IByteReader input;

    public LiberalSevenBitTransform(IByteReader stream) {
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
