package com.upokecenter.mail.transforms;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class SevenBitTransform implements IByteReader {
    private final IByteReader transform;

    public SevenBitTransform(IByteReader stream) {
      this.transform = stream;
    }

    public int read() {
      int ret = this.transform.read();
      if (ret > 0x80 || ret == 0) {
        // NOTE: See definition of 7bit Data in RFC2045 sec. 2.7 (line length
        // limits are not enforced here, though)
        throw new MessageDataException("Invalid character in message body");
      }
      return ret;
    }
  }
