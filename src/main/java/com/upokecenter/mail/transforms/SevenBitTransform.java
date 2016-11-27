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

  public final class SevenBitTransform implements IByteReader {
    private final IByteReader transform;

    public SevenBitTransform(IByteReader stream) {
      this.transform = stream;
    }

    public int read() {
      int ret = this.transform.read();
      if (ret > 0x80 || ret == 0) {
          throw new MessageDataException("Invalid character in message body");
      }
      return ret;
    }
  }
