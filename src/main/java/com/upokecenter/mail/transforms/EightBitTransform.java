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

  public final class EightBitTransform implements IByteReader {
    private final IByteReader input;

    public EightBitTransform(IByteReader stream) {
      this.input = stream;
    }

    public int read() {
      int ret = this.input.read();
      if (ret == 0) {
        // NOTE: See definition of 8bit Data in RFC2045 sec. 2.8 (line length
        // limits are not enforced here, though)
        throw new MessageDataException("Invalid character in message body");
      }
      return ret;
    }
  }
