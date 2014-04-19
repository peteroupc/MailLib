/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

namespace PeterO.Mail {
  internal interface IStringEncoder
  {
    void WriteToString(StringBuilder str, byte b);

    void WriteToString(StringBuilder str, byte[] data, int offset, int count);

    void FinalizeEncoding(StringBuilder str);
  }
}
