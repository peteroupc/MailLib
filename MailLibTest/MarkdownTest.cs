using System;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class MarkdownTest {
    public static Message MarkdownMessage(string str) {
      var msg = new Message();
      msg.SetTextBody(str);
      msg.ContentType = MediaType.Parse("text/markdown\u003bcharset=utf-8");
      return msg;
    }

    [Test]
    public void TestMarkdown() {
      var resources = new AppResources("Resources");
      string[] strings = DictUtility.ParseJSONStringArray(
         resources.GetString("markdown"));
      for (var i = 0; i < strings.Length; i += 2) {
        Message msg = MarkdownMessage(strings[i + 1]);
        Assert.AreEqual(strings[i], msg.GetFormattedBodyString());
      }
    }
  }
}
