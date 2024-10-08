package com.upokecenter.mail.transforms;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

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
