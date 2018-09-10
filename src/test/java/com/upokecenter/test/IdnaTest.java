package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.text.*;

  public class IdnaTest {
    @Test
    public void TestEncodeDomainName() {
      String tmp;
      tmp = "ascii";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "-ascii-1-";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "-ascii-1";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "ascii-1-";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "1ascii-1";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "2ascii-1-";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "as.cii";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "as&cii";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "as`cii";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "\rascii";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "\nascii";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      tmp = "\u007fascii";
      Assert.assertEquals(tmp, Idna.EncodeDomainName(tmp));
      // Test other aspects of Punycode
      {
        String stringTemp = Idna.EncodeDomainName("e\u00e1");
        Assert.assertEquals(
        "xn--e-ufa",
        stringTemp);
      }
    }
    @Test
    public void TestDecodeDomainName() {
      {
String stringTemp = Idna.DecodeDomainName("xn--e-ufa");
Assert.assertEquals(
  "e\u00e1",
  stringTemp);
}
      {
String stringTemp = Idna.DecodeDomainName("xn--e-ufa.example");
Assert.assertEquals(
  "e\u00e1.example",
  stringTemp);
}
      {
String stringTemp = Idna.DecodeDomainName("site.xn--e-ufa.example");
Assert.assertEquals(
  "site.e\u00e1.example",
  stringTemp);
}
    }

    @Test
    public void TestProtocolStrings() {
      if (!(
       ProtocolStrings.IsInIdentifierClass("test\u007b}[]?^&"))) {
 Assert.fail();
 }
      if (!(
       ProtocolStrings.IsInFreeformClass("test\u007b}[]?^&"))) {
 Assert.fail();
 }
      if (
       ProtocolStrings.IsInIdentifierClass("test\u007b} []?^&")) {
 Assert.fail();
 }
      if (!(
       ProtocolStrings.IsInFreeformClass("test\u007b} []?^&"))) {
 Assert.fail();
 }
      {
String stringTemp = ProtocolStrings.UsernameEnforce("Σa");
Assert.assertEquals(
  "σa",
  stringTemp);
}
      if (
       ProtocolStrings.IsInIdentifierClass("tes\nt\u007b} []?^&")) {
 Assert.fail();
 }
      if (
       ProtocolStrings.IsInFreeformClass("tes\nt\u007b} []?^&")) {
 Assert.fail();
 }
      {
String stringTemp = ProtocolStrings.UserpartEnforce("TeSt");
Assert.assertEquals(
  "test",
  stringTemp);
}
      {
String stringTemp = ProtocolStrings.UserpartEnforce("TeSt", false);
Assert.assertEquals(
  "test",
  stringTemp);
}
      {
String stringTemp = ProtocolStrings.UserpartEnforce("TeSt", true);
Assert.assertEquals(
  "TeSt",
  stringTemp);
}
      Assert.assertEquals(
       null,
       ProtocolStrings.UserpartEnforce("Te St", false));
      {
String stringTemp = ProtocolStrings.UsernameEnforce("Te St", false);
Assert.assertEquals(
  "te st",
  stringTemp);
}
      {
String stringTemp = ProtocolStrings.UsernameEnforce("Te St", true);
Assert.assertEquals(
  "Te St",
  stringTemp);
}
      // Final capital sigma
      {
String stringTemp = ProtocolStrings.UserpartEnforce("x\u03a3");
Assert.assertEquals(
  "x\u03c2",
  stringTemp);
}
Assert.assertEquals(
  null,
  ProtocolStrings.UsernameEnforce(null));
Assert.assertEquals(
  null,
  ProtocolStrings.UsernameEnforce(""));
Assert.assertEquals(
  null,
  ProtocolStrings.UserpartEnforce(null));
Assert.assertEquals(
  null,
  ProtocolStrings.UserpartEnforce(""));
Assert.assertEquals(
  null,
  ProtocolStrings.OpaqueStringEnforce(null));
Assert.assertEquals(
  null,
  ProtocolStrings.OpaqueStringEnforce(""));
Assert.assertEquals(
  null,
  ProtocolStrings.NicknameEnforce(null));
Assert.assertEquals(
  null,
  ProtocolStrings.NicknameEnforce(""));
Assert.assertEquals(
  null,
  ProtocolStrings.NicknameForComparison(null));
Assert.assertEquals(
  null,
  ProtocolStrings.NicknameForComparison(""));

{
String stringTemp = ProtocolStrings.OpaqueStringEnforce("a b ccccc test");
Assert.assertEquals(
  "a b ccccc test",
  stringTemp);
}
{
String stringTemp = ProtocolStrings.NicknameEnforce("a b ccccc test");
Assert.assertEquals(
  "a b ccccc test",
  stringTemp);
}
{
String stringTemp = ProtocolStrings.NicknameEnforce("  a b ccccc test  ");
Assert.assertEquals(
  "a b ccccc test",
  stringTemp);
}
{
String stringTemp = ProtocolStrings.NicknameEnforce("  a b ccccc   test  ");
Assert.assertEquals(
  "a b ccccc test",
  stringTemp);
}
{
String stringTemp =
  ProtocolStrings.NicknameEnforce("  a b\u00a0ccccc   test  ");
Assert.assertEquals(
  "a b ccccc test",
  stringTemp);
}
Assert.assertEquals(
  null,
  ProtocolStrings.OpaqueStringEnforce("a\ntest"));
{
String stringTemp = ProtocolStrings.OpaqueStringEnforce("A b Ccccc tEst");
Assert.assertEquals(
  "A b Ccccc tEst",
  stringTemp);
}
{
String stringTemp = ProtocolStrings.OpaqueStringEnforce("a\u00e7c");
Assert.assertEquals(
  "a\u00e7c",
  stringTemp);
}
{
String stringTemp = ProtocolStrings.OpaqueStringEnforce("a\u00a0c");
Assert.assertEquals(
  "a c",
  stringTemp);
}
    }
    @Test
    public void TestIsValidDomainName() {
      if (!(Idna.IsValidDomainName("el\u00b7la", false))) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("-domain", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("domain-", false)) {
 Assert.fail();
 }
      if (!(Idna.IsValidDomainName("xn--e-ufa", false))) {
 Assert.fail();
 }
      if (!(Idna.IsValidDomainName("xn--e-ufa.example", false))) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("ab--e-ufa", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("ab--e-ufa.example", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("xn--", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("xn--.example", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("example.xn--", false)) {
 Assert.fail();
 }
      // Label starting with digit is valid since there are no RTL labels
      if (!(Idna.IsValidDomainName("1domain.example", false))) {
 Assert.fail();
 }
      // Label starting with digit is not valid since there are RTL labels
      if (
  Idna.IsValidDomainName(
  "1domain.example.\u05d0\u05d0",
  false)) {
 Assert.fail();
 }
      if (
  Idna.IsValidDomainName(
  "\u05d0\u05d0.1domain.example",
  false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("el\u00b7", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("el\u00b7ma", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("em\u00b7la", false)) {
 Assert.fail();
 }
      // 0x300 is the combining grave accent
      if (Idna.IsValidDomainName("\u0300xyz", false)) {
 Assert.fail();
 }
      if (!(Idna.IsValidDomainName("x\u0300yz", false))) {
 Assert.fail();
 }
      // Has white space
      if (Idna.IsValidDomainName("x\u0300y z", false)) {
 Assert.fail();
 }
      // 0x323 is dot below, with a lower combining
      // class than grave accent
      if (!(Idna.IsValidDomainName("x\u0323\u0300yz", false))) {
 Assert.fail();
 }
      // Not in NFC, due to the reordered combining marks
      if (Idna.IsValidDomainName("x\u0300\u0323yz", false)) {
 Assert.fail();
 }
      // 0xffbf is unassigned as of Unicode 6.3
      if (Idna.IsValidDomainName("x\uffbfyz", false)) {
 Assert.fail();
 }
      // 0xffff is a noncharacter
      if (Idna.IsValidDomainName("x\uffffyz", false)) {
 Assert.fail();
 }
      // 0x3042 is hiragana A, 0x30a2 is katakana A,
      // and 0x5000 is a Han character
      if (Idna.IsValidDomainName("xy\u30fb", false)) {
 Assert.fail();
 }
      if (!(Idna.IsValidDomainName("xy\u3042\u30fb", false))) {
 Assert.fail();
 }
      if (!(Idna.IsValidDomainName("xy\u30a2\u30fb", false))) {
 Assert.fail();
 }
      if (!(Idna.IsValidDomainName("xy\u5000\u30fb", false))) {
 Assert.fail();
 }
      // ZWJ preceded by virama
      if (!(Idna.IsValidDomainName("xy\u094d\u200dz", false))) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("xy\u200dz", false)) {
 Assert.fail();
 }

  if (
  Idna.IsValidDomainName(
  "\ua840\u0300\u0300\u200d\u0300\u0300\ua840",
  false)) {
 Assert.fail();
 }
      // ZWNJ preceded by virama
      if (!(Idna.IsValidDomainName("xy\u094d\u200cz", false))) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("xy\u200cz", false)) {
 Assert.fail();
 }
      // Dual-joining character (U + A840, Phags-pa KA) on both sides
      if (!(Idna.IsValidDomainName("\ua840\u200c\ua840", false))) {
 Assert.fail();
 }
      // Dual-joining character with intervening T-joining characters
      if (!(
  Idna.IsValidDomainName(
  "\ua840\u0300\u0300\u200c\ua840",
  false))) {
 Assert.fail();
 }

  if (!(
  Idna.IsValidDomainName(
  "\ua840\u0300\u0300\u200c\u0300\u0300\ua840",
  false))) {
 Assert.fail();
 }
      // Left-joining character (U + A872, the only such character
      // in Unicode 6.3, with Bidi type L) on left side
      if (!(Idna.IsValidDomainName("\ua872\u200c\ua840", false))) {
 Assert.fail();
 }

  if (!(
  Idna.IsValidDomainName(
  "\ua872\u0300\u0300\u200c\u0300\u0300\ua840",
  false))) {
 Assert.fail();
 }
      // Left-joining character on right side
      if (Idna.IsValidDomainName("\ua840\u200c\ua872", false)) {
 Assert.fail();
 }

  if (
  Idna.IsValidDomainName(
  "\ua840\u0300\u0300\u200c\u0300\u0300\ua872",
  false)) {
 Assert.fail();
 }
      // Nonjoining character on right side
      if (Idna.IsValidDomainName("\ua840\u200cx", false)) {
 Assert.fail();
 }

  if (
  Idna.IsValidDomainName(
  "\ua840\u0300\u0300\u200c\u0300\u0300x",
  false)) {
 Assert.fail();
 }
      // Nonjoining character on left side
      if (Idna.IsValidDomainName("x\u200c\ua840", false)) {
 Assert.fail();
 }

  if (
  Idna.IsValidDomainName(
  "x\u0300\u0300\u200c\u0300\u0300\ua840",
  false)) {
 Assert.fail();
 }
      // Consecutive ZWNJs
      if (Idna.IsValidDomainName("\ua840\u200c\u200c\ua840", false)) {
 Assert.fail();
 }

      // Keraia
      if (!(Idna.IsValidDomainName("x\u0375\u03b1", false))) {
 Assert.fail();
 }  // Greek
      if (Idna.IsValidDomainName("x\u0375a", false)) {
 Assert.fail();
 }  // Non-Greek
      // Geresh and gershayim
      if (!(Idna.IsValidDomainName("\u05d0\u05f3", false))) {
 Assert.fail();
 }  // Hebrew
      // Arabic (non-Hebrew)
      if (Idna.IsValidDomainName("\u0627\u05f3", false)) {
 Assert.fail();
 }
      if (!(Idna.IsValidDomainName("\u05d0\u05f4", false))) {
 Assert.fail();
 }  // Hebrew
      // Arabic (non-Hebrew)
      if (Idna.IsValidDomainName("\u0627\u05f4", false)) {
 Assert.fail();
 }
      // Bidi Rule: Hebrew and Latin in the same label
      if (Idna.IsValidDomainName("a\u05d0", false)) {
 Assert.fail();
 }  // Hebrew
      if (Idna.IsValidDomainName("\u05d0a", false)) {
 Assert.fail();
 }  // Hebrew
      // Arabic-indic digits and extended Arabic-indic digits
      if (Idna.IsValidDomainName("\u0627\u0660\u06f0\u0627", false)) {
 Assert.fail();
 }
      // Right-joining character (U + 062F; since the only right-joining
      // characters in Unicode 6.3 have Bidi type R,
      // a different dual-joining character is used, U + 062D, which also has
      // the same Bidi type).
      if (!(Idna.IsValidDomainName("\u062d\u200c\u062f", false))) {
 Assert.fail();
 }

  if (!(
  Idna.IsValidDomainName(
  "\u062d\u0300\u0300\u200c\u0300\u0300\u062f",
  false))) {
 Assert.fail();
 }
      // Right-joining character on left side
      if (Idna.IsValidDomainName("\u062f\u200c\u062d", false)) {
 Assert.fail();
 }

  if (
  Idna.IsValidDomainName(
  "\u062f\u0300\u0300\u200c\u0300\u0300\u062d",
  false)) {
 Assert.fail();
 }
      // Regression testa: U + 07FA mistakenly allowed (since
      // U + 07FA has Bidi type R, the other characters in these tests
      // also have Bidi type R).
      if (Idna.IsValidDomainName("\u07ca\u07fa\u07ca", false)) {
 Assert.fail();
 }
      if (Idna.IsValidDomainName("\u07fa", false)) {
 Assert.fail();
 }
    }
  }
