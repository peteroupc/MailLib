using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class DataUrlTest {
    private void TestMatchBasicNone(string[] langranges, string str) {
      IList<string> r = LanguageTags.LanguageTagFilter(
          new List<string>(langranges),
          new List<string>(new string[] { str }),
          false,
          false);
      Assert.AreEqual(0, r.Count);
    }

    private void TestMatchBasicContained(string[] langranges, string str) {
      IList<string> r = LanguageTags.LanguageTagFilter(
          new List<string>(langranges),
          new List<string>(new string[] { str }),
          false,
          false);
      Assert.IsTrue(r.Count > 0);
    }
    [Test]
    public void TestMatchLangTagsBasic() {
      string[] langranges = { "fr-ca", "es" };
      this.TestMatchBasicContained(langranges, "fr-ca");
      this.TestMatchBasicContained(langranges, "es");
      this.TestMatchBasicNone(langranges, "fr-cam");
      this.TestMatchBasicContained(langranges, "fr-ca-9999");
      this.TestMatchBasicNone(langranges, "fr-zxxx-ca");
      this.TestMatchBasicContained(langranges, "es-cam");
      this.TestMatchBasicContained(langranges, "es-ca-9999");
      this.TestMatchBasicContained(langranges, "es-zxxx-ca");
      langranges = new string[] {
        "fr-ca", "*", "es",
      };
      this.TestMatchBasicContained(langranges, "es");
      this.TestMatchBasicContained(langranges, "nl");
      langranges = new string[] { null, "*" };
    }

    private Message TestMailToOne(string s) {
      Console.WriteLine(s);
      Message msg = Message.FromMailtoUri(s);
      Console.WriteLine(msg);
      if (msg == null) {
        Assert.Fail();
      }
      return msg;
    }

    internal static void TestDataUrlRoundTrip(string data) {
      MediaType mt = DataUris.DataUriMediaType(data);
      byte[] bytes = DataUris.DataUriBytes(data);
      Assert.NotNull(mt, data);
      Assert.NotNull(bytes, data);
      string data2 = DataUris.MakeDataUri(bytes, mt);
      MediaType mt2 = DataUris.DataUriMediaType(data2);
      byte[] bytes2 = DataUris.DataUriBytes(data2);
      TestCommon.AssertByteArraysEqual(bytes, bytes2);
      Assert.AreEqual(mt, mt2, data);
    }

    [Test]
    [Timeout(5000)]
    public void TestMailTo() {
      Message msg;
      this.TestMailToOne("mailto:me@example.com");
      this.TestMailToOne("mailto:you@example.com?subject=current-issue");
      msg = this.TestMailToOne("mailto:you@example.com?body=x%20abcdefg-hijk");
      {
        string stringTemp = msg.GetBodyString();
        Assert.AreEqual(
          "x abcdefg-hijk",
          stringTemp);
}
      msg = this.TestMailToOne(
  "mailto:you@example.com?body=x%20abcdefg-h%0d%0aijk");
      {
        string stringTemp = msg.GetBodyString();
        Assert.AreEqual(
          "x abcdefg-h\r\nijk",
          stringTemp);
}
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
      {
        string stringTemp = msg.GetBodyString();
        Assert.AreEqual(
          "b\u00e7",
          stringTemp);
}
      msg =

        this.TestMailToOne(
  "mailto:me@example.com?subject=%3D%3futf-8%3fQ%3fb%3dC3%3dA7%3f%3d");
      Console.Write(msg.GetHeader("subject"));
    }
  }
}
