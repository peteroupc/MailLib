using System;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;

namespace MailLibTest {
  [TestFixture]
  public class MarkdownTest {
    public Message MarkdownMessage(string str) {
      var msg = new Message();
      msg.SetTextBody(str);
      msg.ContentType = MediaType.Parse("text/markdown\u003bcharset=utf-8");
      return msg;
    }

    public void TestMarkdownOne(
  string expectedOutput,
  string input) {
      Message msg = this.MarkdownMessage(input);
      Assert.AreEqual(expectedOutput, msg.GetFormattedBodyString());
    }

    [Test]
    public void TestMarkdown() {
      this.TestMarkdownOne("<p><em>Text</em></p>", "_Text_");
      this.TestMarkdownOne("<p><em>Text</em></p>", "*Text*");
      this.TestMarkdownOne("<p><strong>Text</strong></p>", "__Text__");
      this.TestMarkdownOne("<p><strong>Text</strong></p>", "**Text**");
      this.TestMarkdownOne("<p><s>Text</s></p>", "<s>Text</s>");
      this.TestMarkdownOne("<p><a href=\"x\">y</a></p>", "[y](x)");
    this.TestMarkdownOne(
  "<p><img src=\"x\" alt=\"y\" /></p>",
  "![y](x)");
    }
  }
}
