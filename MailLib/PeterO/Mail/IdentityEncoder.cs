/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
    /// <summary/>
  internal sealed class IdentityEncoder : ICharacterEncoder
  {
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
