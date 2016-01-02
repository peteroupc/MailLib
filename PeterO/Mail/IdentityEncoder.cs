/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;
using System.IO;

using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.IdentityEncoder"]/*'/>
  internal sealed class IdentityEncoder : ICharacterEncoder
  {
    public int Encode(int c, IWriter s) {
      if (s == null) {
  throw new ArgumentNullException("s");
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