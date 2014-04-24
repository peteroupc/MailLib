/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using NUnit.Framework;
using PeterO.Text;

namespace MailLibTest {
  [TestFixture]
  public class DomainTest
  {
    // Tests that all-ASCII strings remain unchanged by
    // PunycodeEncode.
    public void TestAllAsciiLabels() {
      string tmp;
      tmp="ascii";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="-ascii-1-";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="-ascii-1";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="ascii-1-";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="1ascii-1";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="2ascii-1-";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="as.cii";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="as&cii";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="as`cii";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="\rascii";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="\nascii";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="\u007fascii";
      Assert.AreEqual(tmp, DomainUtility.PunycodeEncode(tmp));
    }

    [Test]
    public void IdnaTest() {
      Assert.IsTrue(Idna.IsValidDomainName("el\u00b7la",false));
      Assert.IsFalse(Idna.IsValidDomainName("el\u00b7",false));
      Assert.IsFalse(Idna.IsValidDomainName("el\u00b7ma",false));
      Assert.IsFalse(Idna.IsValidDomainName("em\u00b7la",false));
      // 0x300 is the combining grave accent
      Assert.IsFalse(Idna.IsValidDomainName("\u0300xyz",false));
      Assert.IsTrue(Idna.IsValidDomainName("x\u0300yz",false));
      // Has white space
      Assert.IsFalse(Idna.IsValidDomainName("x\u0300y z",false));
      // 0x323 is dot below, with a lower combining
      // class than grave accent
      Assert.IsTrue(Idna.IsValidDomainName("x\u0323\u0300yz",false));
      // Not in NFC, due to the reordered combining marks
      Assert.IsFalse(Idna.IsValidDomainName("x\u0300\u0323yz",false));
      // 0xffbf is unassigned as of Unicode 6.3
      Assert.IsFalse(Idna.IsValidDomainName("x\uffbfyz",false));
      // 0xffff is a noncharacter
      Assert.IsFalse(Idna.IsValidDomainName("x\uffffyz",false));
      // 0x3042 is hiragana A, 0x30a2 is katakana A,
      // and 0x5000 is a Han character
      Assert.IsFalse(Idna.IsValidDomainName("xy\u30fb",false));
      Assert.IsTrue(Idna.IsValidDomainName("xy\u3042\u30fb",false));
      Assert.IsTrue(Idna.IsValidDomainName("xy\u30a2\u30fb",false));
      Assert.IsTrue(Idna.IsValidDomainName("xy\u5000\u30fb",false));
    }
  }
}
