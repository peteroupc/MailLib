/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
 using System;
using System.IO;
using PeterO.Mail;
namespace MailLibTest {
  class Program {
    public static void Main() {
      new EncodingTest().TestRandomEncodedBytes();
    }
  }
}
