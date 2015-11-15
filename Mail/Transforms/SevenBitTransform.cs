/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO;
using PeterO.Mail;

namespace PeterO.Mail.Transforms {
  internal sealed class SevenBitTransform : ITransform {
    private ITransform transform;

    public SevenBitTransform(ITransform stream) {
      this.transform = stream;
    }

    public int ReadByte() {
      int ret = this.transform.ReadByte();
      if (ret > 0x80 || ret == 0) {
          throw new MessageDataException("Invalid character in message body");
      }
      return ret;
    }
  }
}
