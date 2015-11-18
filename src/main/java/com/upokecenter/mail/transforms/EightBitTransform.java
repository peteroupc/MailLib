package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class EightBitTransform implements IByteReader {
    private IByteReader input;

    public EightBitTransform (IByteReader stream) {
      this.input = stream;
    }

    public int read() {
      int ret = this.input.read();
      if (ret == 0) {
        throw new MessageDataException("Invalid character in message body");
      }
      return ret;
    }
  }
