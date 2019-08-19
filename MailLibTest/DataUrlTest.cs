using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class DataUrlTest {
    public bool SplitIRIFails(string iri, bool expectedNonNull) {
      return expectedNonNull ? URIUtility.SplitIRI(iri) == null :
         URIUtility.SplitIRI(iri) != null;
    }

    [Test]
    public void TestIPv6() {
      var resources = new AppResources("Resources");
      string[] cases = DictUtility.ParseJSONStringArray(
         resources.GetString("ipv6parse"));
      for (var i = 0; i < cases.Length; i += 2) {
        if (this.SplitIRIFails(
          cases[i],
          cases[i + 1].Equals("1", StringComparison.Ordinal))) {
          Assert.Fail(cases[i] + " " + cases[i + 1]);
        }
      }
    }

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

    private static void TestEmptyPathOne(string uri) {
      int[] iriPositions = URIUtility.SplitIRI(uri);
      if (iriPositions == null) {
        Assert.Fail();
      }
      Assert.IsTrue(iriPositions[4] >= 0);
      Assert.AreEqual(iriPositions[4], iriPositions[5]);
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
    [Timeout(5000)]
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

    private static void AssertIdempotency(string s) {
      bool cond = URIUtility.IsValidIRI(s);

      Assert.IsTrue(cond, s);
      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          0);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 0),
          0);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          1);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 1),
          1);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          2);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 2),
          2);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          3);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 3),
          3);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
    }

    private static void AssertIdempotencyNeg(
  string s) {
      if ((bool)URIUtility.IsValidIRI(s)) {
        Assert.Fail(s);
      }

      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          0);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 0),
          0);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          1);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 1),
          1);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          2);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 2),
          2);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
      {
        var stringTemp = (string)URIUtility.EscapeURI(
          s,
          3);
        var stringTemp2 = (string)URIUtility.EscapeURI(
          (string)URIUtility.EscapeURI(s, 3),
          3);
        Assert.AreEqual(stringTemp, stringTemp2);
      }
    }

    private static void AssertResolve(String src, String baseuri, String dest) {
      AssertIdempotency(src);
      AssertIdempotency(baseuri);
      AssertIdempotency(dest);
      var res = (string)URIUtility.RelativeResolve(
        src,
        baseuri);
      AssertIdempotency(res);
      Assert.AreEqual(dest, res);
    }

    [Test]
    [Timeout(5000)]
    public void TestRelativeResolve() {
      AssertResolve(
        "index.html",
        "http://example.com",
        "http://example.com/index.html");
      AssertResolve(
        "./.x",
        "http://example.com/a/b/c/d/e.f",
        "http://example.com/a/b/c/d/.x");
      AssertResolve(
        ".x",
        "http://example.com/a/b/c/d/e.f",
        "http://example.com/a/b/c/d/.x");
      AssertResolve(
        "../.x",
        "http://example.com/a/b/c/d/e.f",
        "http://example.com/a/b/c/.x");
      AssertResolve(
        "../..../../../.../.x",
        "http://example.com/a/b/c/d/e.f",
        "http://example.com/a/b/.../.x");
    }

    [Test]
    [Timeout(5000)]
    public void TestUri() {
      AssertIdempotency(String.Empty);
      AssertIdempotency("e");
      AssertIdempotency("e:x");
      AssertIdempotency("e://x:@y");
      AssertIdempotency("e://x/y");
      AssertIdempotency("e://x//y");
      AssertIdempotency("a://x:@y/z");
      AssertIdempotency("a://x:/y");
      AssertIdempotency("aa:/w/x");
      AssertIdempotency("01/w/x");
      AssertIdempotency("e://x");
      AssertIdempotency("e://x/:/");
      AssertIdempotency("x/:/");
      AssertIdempotency("/:/");
      AssertIdempotency("///");
      AssertIdempotency("e://x:");
      AssertIdempotency("e://x:%30");
      AssertIdempotency("a://x:?x");
      AssertIdempotency("a://x#x");
      AssertIdempotency("a://x?x");
      AssertIdempotency("a://x:#x");
      AssertIdempotency("e://x@x");
      AssertIdempotency("e://x@:");
      AssertIdempotency("e://x@:?");
      AssertIdempotency("e://x@:#");
      AssertIdempotency("//x@x");
      AssertIdempotency("//x@:");
      AssertIdempotency("//x@:?");
      AssertIdempotency("//x@:#");
      AssertIdempotency("//x@:?x");
      AssertIdempotency("e://x@:#x");
      AssertIdempotency("a://x:?x");
      AssertIdempotency("a://x#x");
      AssertIdempotency("a://x?x");
      AssertIdempotency("a://x:#x");

      AssertIdempotencyNeg("e://^//y");
      AssertIdempotencyNeg("e^");
      AssertIdempotencyNeg("e://x:a");
      AssertIdempotencyNeg("a://x::/y");
      AssertIdempotency("x@yz");
      AssertIdempotencyNeg("x@y:z");
      AssertIdempotencyNeg("01:/w/x");
      AssertIdempotencyNeg("e://x:%30/");
      AssertIdempotencyNeg("a://xxx@[");
      AssertIdempotencyNeg("a://[");

      AssertIdempotency("a://[va.a]");
      AssertIdempotency("a://[v0.0]");
      AssertIdempotency("a://x:/");
      AssertIdempotency("a://[va.a]:/");

      AssertIdempotencyNeg("a://x%/");
      AssertIdempotencyNeg("a://x%xy/");
      AssertIdempotencyNeg("a://x%x%/");
      AssertIdempotencyNeg("a://x%%x/");
      AssertIdempotency("a://x%20/");
      AssertIdempotency("a://[v0.0]/");
      AssertIdempotencyNeg("a://[wa.a]");
      AssertIdempotencyNeg("a://[w0.0]");
      AssertIdempotencyNeg("a://[va.a/");
      AssertIdempotencyNeg("a://[v.a]");
      AssertIdempotencyNeg("a://[va.]");

      AssertIPv6("a:a:a:a:a:a:100.100.100.100");
      AssertIPv6("::a:a:a:a:a:100.100.100.100");
      AssertIPv6("::a:a:a:a:a:99.255.240.10");
      AssertIPv6("::a:a:a:a:99.255.240.10");
      AssertIPv6("::99.255.240.10");
      AssertIPv6Neg("99.255.240.10");
      AssertIPv6("a:a:a:a:a::99.255.240.10");
      AssertIPv6("a:a:a:a:a:a:a:a");
      AssertIPv6("aaa:a:a:a:a:a:a:a");
      AssertIPv6("aaaa:a:a:a:a:a:a:a");
      AssertIPv6("::a:a:a:a:a:a:a");
      AssertIPv6("a::a:a:a:a:a:a");
      AssertIPv6("a:a::a:a:a:a:a");
      AssertIPv6("a:a:a::a:a:a:a");
      AssertIPv6("a:a:a:a::a:a:a");
      AssertIPv6("a:a:a:a:a::a:a");
      AssertIPv6("a:a:a:a:a:a::a");
      AssertIPv6("a::a");
      AssertIPv6("::a");
      AssertIPv6("::a:a");
      AssertIPv6("::");
      AssertIPv6("a:a:a:a:a:a:a::");
      AssertIPv6("a:a:a:a:a:a::");
      AssertIPv6("a:a:a:a:a::");
      AssertIPv6("a:a:a:a::");
      AssertIPv6("a:a::");
      AssertIPv6Neg("a:a::a:a:a:a:a:a");
      AssertIPv6Neg("aaaaa:a:a:a:a:a:a:a");
      AssertIPv6Neg("a:a:a:a:a:a:a:100.100.100.100");
      AssertIPv6Neg("a:a:a:a:a:a::99.255.240.10");
      AssertIPv6Neg(":::a:a:a:a:a:a:a");
      AssertIPv6Neg("a:a:a:a:a:a:::a");
      AssertIPv6Neg("a:a:a:a:a:a:a:::");
      AssertIPv6Neg("::a:a:a:a:a:a:a:");
      AssertIPv6Neg("::a:a:a:a:a:a:a:a");
      AssertIPv6Neg("a:a");
      AssertIdempotency("e://[va.a]");
      AssertIdempotency("e://[v0.0]");

      AssertIdempotencyNeg("e://[wa.a]");
      AssertIdempotencyNeg("e://[va.^]");
      AssertIdempotencyNeg("e://[va.]");
      AssertIdempotencyNeg("e://[v.a]");
    }

    private static void AssertIPv6Neg(string str) {
      AssertIdempotencyNeg("e://[" + str + "]");
      AssertIdempotencyNeg("e://[" + str + "NANA]");
      AssertIdempotencyNeg("e://[" + str + "%25]");
      AssertIdempotencyNeg("e://[" + str + "%NANA]");
      AssertIdempotencyNeg("e://[" + str + "%25NANA]");
      AssertIdempotencyNeg("e://[" + str + "%52NANA]");
      AssertIdempotencyNeg("e://[" + str + "%25NA<>NA]");
      AssertIdempotencyNeg("e://[" + str + "%25NA%E2NA]");
      AssertIdempotencyNeg("e://[" + str + "%25NA%2ENA]");
    }

    private static void AssertIPv6(string str) {
      AssertIdempotency("e://[" + str + "]");

      AssertIdempotencyNeg("e://[" + str + "NANA]");
      AssertIdempotencyNeg("e://[" + str + "%25]");
      AssertIdempotencyNeg("e://[" + str + "%NANA]");
      AssertIdempotencyNeg("e://[" + str + "%52NANA]");
      AssertIdempotencyNeg("e://[" + str + "%25NA<>NA]");
      // NOTE: Commented out because current parser allows
      // IPv6 addresses with zone identifiers only if
      // the address is link-local
      // AssertIdempotency("e://[" + str + "%25NANA]");
      // AssertIdempotency("e://[" + str + "%25NA%E2NA]");
      // AssertIdempotency("e://[" + str + "%25NA%2ENA]");
    }
  }
}
