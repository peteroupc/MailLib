using System;
using System.Collections.Generic;
using NUnit.Framework;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class DataUrlTest {
    [Test]
    public void TestMatchLangTagsBasic() {
      string[] langranges = { "fr-ca", "es" };
      Assert.AreEqual(0, DataUrl.MatchLangTagsBasic(langranges, "fr-ca"));
      Assert.AreEqual(1, DataUrl.MatchLangTagsBasic(langranges, "es"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsBasic(langranges, "fr-cam"));
      Assert.AreEqual(0, DataUrl.MatchLangTagsBasic(langranges, "fr-ca-9999"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsBasic(langranges, "fr-zxxx-ca"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsBasic(langranges, "es-cam"));
      Assert.AreEqual(0, DataUrl.MatchLangTagsBasic(langranges, "es-ca-9999"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsBasic(langranges, "es-zxxx-ca"));
      langranges =new string[]{ "fr-ca","*","es"};
      Assert.AreEqual(1, DataUrl.MatchLangTagsBasic(langranges, "es", false));
      Assert.AreEqual(2, DataUrl.MatchLangTagsBasic(langranges, "es", true));
      Assert.AreEqual(1, DataUrl.MatchLangTagsBasic(langranges, "nl", false));
      Assert.AreEqual(1, DataUrl.MatchLangTagsBasic(langranges, "nl", true));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsBasic(langranges, "", false));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsBasic(langranges, "", true));
      langranges = new string[] { null, "*" };
      Assert.Throws<ArgumentException>(() => DataUrl.MatchLangTagsBasic(langranges, "es", false));
      Assert.Throws<ArgumentException>(() => DataUrl.MatchLangTagsBasic(langranges, "es", true));
      langranges = new string[] { String.Empty, "*" };
      Assert.Throws<ArgumentException>(() => DataUrl.MatchLangTagsBasic(langranges, "es", false));
      Assert.Throws<ArgumentException>(() => DataUrl.MatchLangTagsBasic(langranges, "es", true));
    }

    private Message TestMailToOne(string s) {
      Console.WriteLine(s);
      Message msg = DataUrl.MailToUrlMessage(s);
      Console.WriteLine(msg);
      if (msg == null) {
 Assert.Fail();
 }
      return msg;
    }

    internal static void TestDataUrlRoundTrip(string data) {
      MediaType mt = DataUrl.DataUrlMediaType(data);
      byte[] bytes = DataUrl.DataUrlBytes(data);
      Assert.NotNull(mt, data);
      Assert.NotNull(bytes, data);
      string data2 = DataUrl.MakeDataUrl(bytes, mt);
      MediaType mt2 = DataUrl.DataUrlMediaType(data2);
      byte[] bytes2 = DataUrl.DataUrlBytes(data2);
      Test.TestCommon.AssertByteArraysEqual(bytes, bytes2);
      Assert.AreEqual(mt, mt2, data);
    }

    private static void TestEmptyPathOne(string uri) {
      int[] iri = Test.URIUtility.splitIRI(uri);
      if (iri == null) {
 Assert.Fail();
 }
      Assert.GreaterOrEqual(iri[4], 0);
      Assert.AreEqual(iri[4], iri[5]);
    }

    [Test]
    public void TestEmptyPathURIs() {
      TestEmptyPathOne("s://h");
      TestEmptyPathOne("s://h?x");
      TestEmptyPathOne("s://h#x");
      TestEmptyPathOne("//h");
      TestEmptyPathOne("//h?x");
      TestEmptyPathOne("//h#x");
      TestEmptyPathOne("s://");
      TestEmptyPathOne("s://?x");
      TestEmptyPathOne("s://#x");
      TestEmptyPathOne("s:");
      TestEmptyPathOne("s:?x");
      TestEmptyPathOne("s:#x");
    }

    [Test]
    public void TestExtended() {
      string[] ranges = { "fr-ca" };
      Assert.AreEqual(0, DataUrl.MatchLangTagsExtended(ranges, "fr-ca"));
      Assert.AreEqual(0, DataUrl.MatchLangTagsExtended(ranges, "FR-ca"));
      Assert.AreEqual(0, DataUrl.MatchLangTagsExtended(ranges, "fr-CA"));
      Assert.AreEqual(0, DataUrl.MatchLangTagsExtended(ranges, "fr-zxxx-ca"));
      Assert.AreEqual(0, DataUrl.MatchLangTagsExtended(ranges, "fr-zxxx-ca-xxxxxx"));
      Assert.AreEqual(0, DataUrl.MatchLangTagsExtended(ranges, "fr-zxxx-ca-u-xxxxxx"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsExtended(ranges, "fr"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsExtended(ranges, "fr-xxxx"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsExtended(ranges, "fr-FR"));
      Assert.AreEqual(-1, DataUrl.MatchLangTagsExtended(ranges, "fr-u-ca"));
    }
    [Test]
    public void TestMailTo() {
      Message msg;
      this.TestMailToOne("mailto:me@example.com");
      this.TestMailToOne("mailto:you@example.com?subject=current-issue");
 msg =
  this.TestMailToOne("mailto:you@example.com?body=x%20abcdefg-hijk");
      Assert.AreEqual("x abcdefg-hijk", msg.BodyString);
      msg =
  this.TestMailToOne("mailto:you@example.com?body=x%20abcdefg-h%0d%0aijk");
      Assert.AreEqual("x abcdefg-h\r\nijk", msg.BodyString);
      // ----
      msg = this.TestMailToOne("mailto:e%25f@m.example");
      {
string stringTemp = msg.GetHeader("to");
Assert.AreEqual(
  "e%f@m.example",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:e%26f@m.example");
      {
string stringTemp = msg.GetHeader("to");
Assert.AreEqual(
  "e&f@m.example",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:e%3ff@m.example");
      {
string stringTemp = msg.GetHeader("to");
Assert.AreEqual(
  "e?f@m.example",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:%22e%3ff%22@m.example");
      Assert.AreEqual("\"e?f\"@m.example", msg.GetHeader("to"));
      msg = this.TestMailToOne("mailto:%22e%40f%22@m.example");
      Assert.AreEqual("\"e@f\"@m.example", msg.GetHeader("to"));
      msg = this.TestMailToOne("mailto:%22e%5c%5c%40%5c%22f%22@m.example");
      Assert.AreEqual("\"e\\\\@\\\"f\"@m.example", msg.GetHeader("to"));
      msg = this.TestMailToOne("mailto:%22e'f%22@m.example");
      Assert.AreEqual("\"e'f\"@m.example", msg.GetHeader("to"));
      IList<NamedAddress> toaddrs = msg.GetAddresses("to");
      Console.WriteLine(toaddrs[0]);
      msg = this.TestMailToOne("mailto:me@b%c3%a8.example");
      {
string stringTemp = msg.GetHeader("to");
Assert.AreEqual(
  "me@b\u00e8.example",
  stringTemp);
}
      IList<NamedAddress> toaddr = msg.GetAddresses("to");
      Console.WriteLine(toaddr[0]);
      msg = this.TestMailToOne("mailto:me@example.com?subject=b%c3%a7");
      {
string stringTemp = msg.GetHeader("subject");
Assert.AreEqual(
  "b\u00e7",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:me@example.com?body=b%c3%a7");
      Assert.AreEqual("b\u00e7", msg.BodyString);
      msg =

  this.TestMailToOne("mailto:me@example.com?subject=%3D%3futf-8%3fQ%3fb%3dC3%3dA7%3f%3d");
      Console.Write(msg.GetHeader("subject"));
    }
  }
}
