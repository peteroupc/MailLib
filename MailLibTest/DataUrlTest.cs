using System;
using NUnit.Framework;
using PeterO.Mail;

namespace MailLibTest {
  [TestFixture]
  public class DataUrlTest {
    private Message TestMailToOne(string s) {
      Console.WriteLine(s);
      Message msg = DataUrl.MailToUrlMessage(s);
      Console.WriteLine(msg);
      if (msg == null) {
 Assert.Fail();
 }
      return msg;
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
      Console.WriteLine(msg.ToAddresses[0]);
      msg = this.TestMailToOne("mailto:me@b%c3%a8.example");
      {
string stringTemp = msg.GetHeader("to");
Assert.AreEqual(
  "me@b\u00e8.example",
  stringTemp);
}
      Console.WriteLine(msg.ToAddresses[0]);
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
