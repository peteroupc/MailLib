/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
  /// <summary>This is an internal API.</summary>
  internal sealed class IdentityEncoder : ICharacterEncoder {
    public int Encode(int c, IWriter s) {
      if (s == null) {
        throw new ArgumentNullException(nameof(s));
      }
      if (c < 0) {
        return -1;
      }
      c &= 0xff;
      s.WriteByte((byte)c);
      return 1;
    }
  }
}
