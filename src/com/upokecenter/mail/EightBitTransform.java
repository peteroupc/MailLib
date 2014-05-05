package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  final class EightBitTransform implements ITransform {
    private ITransform stream;

    public EightBitTransform (ITransform stream) {
      this.stream = stream;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int read() {
      int ret = this.stream.read();
      if (ret == 0) {
        throw new MessageDataException("Invalid character in message body");
      }
      return ret;
    }
  }
