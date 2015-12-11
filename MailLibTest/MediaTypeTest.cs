using NUnit.Framework;
using PeterO.Mail;
using System;
namespace MailLibTest {
  [TestFixture]
  public partial class MediaTypeTest {
    [Test]
    public void TestEquals() {
      // not implemented yet
    }
    [Test]
    public void TestGetCharset() {
      MediaType mt;
      mt = MediaType.Parse("text/plain");
      {
        {
          string stringTemp = mt.GetCharset();
          Assert.AreEqual(
          "us-ascii",
          stringTemp);
        }
      }
      mt = MediaType.Parse("text/vcard");
      {
        {
          string stringTemp = mt.GetCharset();
          Assert.AreEqual(
          "utf-8",
          stringTemp);
        }
      }
      mt = MediaType.Parse("text/x-unknown");
      Assert.AreEqual(String.Empty, mt.GetCharset());

      {
      string stringTemp = MediaType.Parse("text/plain").GetCharset();
      Assert.AreEqual(
        "us-ascii",
        stringTemp); }
      {
        string stringTemp = MediaType.Parse("TEXT/PLAIN").GetCharset();
    Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("TeXt/PlAiN").GetCharset();
  Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/troff").GetCharset();
Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      Assert.AreEqual("utf-8" , MediaType.Parse("text/plain; CHARSET=UTF-8"
).GetCharset());
      Assert.AreEqual("utf-8" , MediaType.Parse("text/plain; ChArSeT=UTF-8"
).GetCharset());
      Assert.AreEqual("utf-8" , MediaType.Parse("text/plain; charset=UTF-8"
).GetCharset());
      // Note that MIME implicitly allows whitespace around the equal sign
      {
        string stringTemp = MediaType.Parse("text/plain; charset = UTF-8"
).GetCharset();
Assert.AreEqual(
          "utf-8",
          stringTemp);
      }
      {
  string stringTemp = MediaType.Parse("text/plain; charset (cmt) = (cmt) UTF-8"
).GetCharset();
Assert.AreEqual(
          "utf-8",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/plain; charset='UTF-8'"
).GetCharset();
Assert.AreEqual(
          "'utf-8'",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/plain; charset=\"UTF-8\""
).GetCharset();
Assert.AreEqual(
          "utf-8",
          stringTemp);
      }
      {
        string stringTemp =
          MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\""
).GetCharset();
Assert.AreEqual(
          "utf-8",
          stringTemp);
      }
      {
        string stringTemp =
          MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\""
).GetCharset();
Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      {
     string stringTemp = MediaType.Parse("text/plain; foo='; charset=\"UTF-8\""
).GetCharset();
Assert.AreEqual(
          "utf-8",
          stringTemp);
      }
      {
   string stringTemp = MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\""
).GetCharset();
Assert.AreEqual(
          "utf-8",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/plain; charset=\"UTF-\\8\""
).GetCharset();
Assert.AreEqual(
          "utf-8",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("nana").GetCharset();
Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      Assert.AreEqual("", MediaType.Parse("text/xyz").GetCharset());
      Assert.AreEqual("utf-8" , MediaType.Parse("text/xyz;charset=UTF-8"
).GetCharset());
      Assert.AreEqual("utf-8" , MediaType.Parse("text/xyz;charset=utf-8"
).GetCharset());
      Assert.AreEqual("" , MediaType.Parse("text/xyz;chabset=utf-8"
).GetCharset());
      Assert.AreEqual("utf-8" , MediaType.Parse("text/xml;charset=utf-8"
).GetCharset());
      Assert.AreEqual("utf-8" , MediaType.Parse("text/plain;charset=utf-8"
).GetCharset());
      {
  string stringTemp = MediaType.Parse("text/plain;chabset=utf-8"
).GetCharset();
Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      Assert.AreEqual("utf-8" , MediaType.Parse("image/xml;charset=utf-8"
).GetCharset());
      Assert.AreEqual("" , MediaType.Parse("image/xml;chabset=utf-8"
).GetCharset());
      Assert.AreEqual("utf-8" , MediaType.Parse("image/plain;charset=utf-8"
).GetCharset());
      Assert.AreEqual("" , MediaType.Parse("image/plain;chabset=utf-8"
).GetCharset());
    }
    [Test]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [Test]
    public void TestGetParameter() {
      // not implemented yet
    }
    [Test]
    public void TestIsMultipart() {
      // not implemented yet
    }
    [Test]
    public void TestIsText() {
      // not implemented yet
    }
    [Test]
    public void TestParameters() {
      // not implemented yet
    }
    [Test]
    public void TestParse() {
      // not implemented yet
    }
    [Test]
    public void TestSubType() {
      // not implemented yet
    }
    [Test]
    public void TestTopLevelType() {
      // not implemented yet
    }
    [Test]
    public void TestToString() {
      // not implemented yet
    }
    [Test]
    public void TestTypeAndSubType() {
      // not implemented yet
    }
  }
}
