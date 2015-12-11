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
      mt=MediaType.Parse("text/plain");
      {
{
string stringTemp = mt.GetCharset();
Assert.AreEqual(
"us-ascii",
stringTemp);
}
}
      mt=MediaType.Parse("text/vcard");
      {
{
string stringTemp = mt.GetCharset();
Assert.AreEqual(
"utf-8",
stringTemp);
}
}
      mt=MediaType.Parse("text/x-unknown");
      Assert.AreEqual(String.Empty, mt.GetCharset());
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
