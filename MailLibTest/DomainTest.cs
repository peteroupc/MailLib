/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO.Text;

namespace MailLibTest {
  [TestClass]
  public class DomainTest
  {
    // Tests that all-ASCII strings remain unchanged by
    // PunycodeEncode.
    public void TestPunycode() {
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
      // Test other aspects of Punycode
      Assert.AreEqual(
        "xn--e-ufa",
        DomainUtility.PunycodeEncode("e\u00e1"));
      Assert.AreEqual(
        "e\u00e1",
        DomainUtility.PunycodeDecode("xn--e-ufa",4,9));
    }

    [TestMethod]
    public void IdnaTest() {
      Assert.IsTrue(Idna.IsValidDomainName("el\u00b7la",false));
      Assert.IsFalse(Idna.IsValidDomainName("-domain",false));
      Assert.IsFalse(Idna.IsValidDomainName("domain-",false));
      Assert.IsTrue(Idna.IsValidDomainName("xn--e-ufa",false));
      Assert.IsTrue(Idna.IsValidDomainName("xn--e-ufa.example",false));
      Assert.IsFalse(Idna.IsValidDomainName("ab--e-ufa",false));
      Assert.IsFalse(Idna.IsValidDomainName("ab--e-ufa.example",false));
      Assert.IsFalse(Idna.IsValidDomainName("xn--",false));
      Assert.IsFalse(Idna.IsValidDomainName("xn--.example",false));
      Assert.IsFalse(Idna.IsValidDomainName("example.xn--",false));
      // Label starting with digit is valid since there are no RTL labels
      Assert.IsTrue(Idna.IsValidDomainName("1domain.example",false));
      // Label starting with digit is not valid since there are RTL labels
      Assert.IsFalse(Idna.IsValidDomainName("1domain.example.\u05d0\u05d0",false));
      Assert.IsFalse(Idna.IsValidDomainName("\u05d0\u05d0.1domain.example",false));
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
      // ZWJ preceded by virama
      Assert.IsTrue(Idna.IsValidDomainName("xy\u094d\u200dz",false));
      Assert.IsFalse(Idna.IsValidDomainName("xy\u200dz",false));
      Assert.IsFalse(Idna.IsValidDomainName("\ua840\u0300\u0300\u200d\u0300\u0300\ua840",false));
      // ZWNJ preceded by virama
      Assert.IsTrue(Idna.IsValidDomainName("xy\u094d\u200cz",false));
      Assert.IsFalse(Idna.IsValidDomainName("xy\u200cz",false));
      // Dual-joining character (U + A840, Phags-pa KA) on both sides
      Assert.IsTrue(Idna.IsValidDomainName("\ua840\u200c\ua840",false));
      // Dual-joining character with intervening T-joining characters
      Assert.IsTrue(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\ua840",false));
      Assert.IsTrue(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\u0300\u0300\ua840",false));
      // Left-joining character (U + A872, the only such character
      // in Unicode 6.3, with Bidi type L) on left side
      Assert.IsTrue(Idna.IsValidDomainName("\ua872\u200c\ua840",false));
      Assert.IsTrue(Idna.IsValidDomainName("\ua872\u0300\u0300\u200c\u0300\u0300\ua840",false));
      // Left-joining character on right side
      Assert.IsFalse(Idna.IsValidDomainName("\ua840\u200c\ua872",false));
      Assert.IsFalse(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\u0300\u0300\ua872",false));
      // Nonjoining character on right side
      Assert.IsFalse(Idna.IsValidDomainName("\ua840\u200cx",false));
      Assert.IsFalse(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\u0300\u0300x",false));
      // Nonjoining character on left side
      Assert.IsFalse(Idna.IsValidDomainName("x\u200c\ua840",false));
      Assert.IsFalse(Idna.IsValidDomainName("x\u0300\u0300\u200c\u0300\u0300\ua840",false));
      // Keraia
      Assert.IsTrue(Idna.IsValidDomainName("x\u0375\u03b1",false));  // Greek
      Assert.IsFalse(Idna.IsValidDomainName("x\u0375a",false));  // Non-Greek
      // Geresh and gershayim
      Assert.IsTrue(Idna.IsValidDomainName("\u05d0\u05f3",false));  // Hebrew
      Assert.IsFalse(Idna.IsValidDomainName("\u0627\u05f3",false));  // Arabic (non-Hebrew)
      Assert.IsTrue(Idna.IsValidDomainName("\u05d0\u05f4",false));  // Hebrew
      Assert.IsFalse(Idna.IsValidDomainName("\u0627\u05f4",false));  // Arabic (non-Hebrew)
      // Bidi Rule: Hebrew and Latin in the same label
      Assert.IsFalse(Idna.IsValidDomainName("a\u05d0",false));  // Hebrew
      Assert.IsFalse(Idna.IsValidDomainName("\u05d0a",false));  // Hebrew
      // Arabic-indic digits and extended Arabic-indic digits
      Assert.IsFalse(Idna.IsValidDomainName("\u0627\u0660\u06f0\u0627",false));
      // Right-joining character (U + 062F; since the only right-joining characters in
      // Unicode have Bidi type R,
      // a different dual-joining character is used, U + 062D, which also has
      // the same Bidi type).
      Assert.IsTrue(Idna.IsValidDomainName("\u062d\u200c\u062f",false));
      Assert.IsTrue(Idna.IsValidDomainName("\u062d\u0300\u0300\u200c\u0300\u0300\u062f",false));
      // Right-joining character on left side
      Assert.IsFalse(Idna.IsValidDomainName("\u062f\u200c\u062d",false));
      Assert.IsFalse(Idna.IsValidDomainName("\u062f\u0300\u0300\u200c\u0300\u0300\u062d",false));
    }
  }
}
