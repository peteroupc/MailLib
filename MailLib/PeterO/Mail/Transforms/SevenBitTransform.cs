/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO;
using PeterO.Mail;

namespace PeterO.Mail.Transforms {
  internal sealed class SevenBitTransform : IByteReader {
    private readonly IByteReader transform;

    public SevenBitTransform(IByteReader stream) {
      this.transform = stream;
    }

    public int ReadByte() {
      int ret = this.transform.ReadByte();
      if (ret > 0x80 || ret == 0) {
        // NOTE: See definition of 7bit Data in RFC2045 sec. 2.7 (line length
        // limits are not enforced here, though)
        throw new MessageDataException("Invalid character in message body");
      }
      return ret;
    }
  }
}
