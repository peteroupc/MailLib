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

    public void TestMarkdownOne(string expectedOutput,
                    string input) {
      Message msg = MarkdownMessage(input);
      Assert.AreEqual(expectedOutput, msg.GetFormattedBodyString());
    }

    [Test]
    public void TestMarkdown() {
      TestMarkdownOne("<p><em>Text</em></p>", "_Text_");
      TestMarkdownOne("<p><em>Text</em></p>", "*Text*");
      TestMarkdownOne("<p><strong>Text</strong></p>", "__Text__");
      TestMarkdownOne("<p><strong>Text</strong></p>", "**Text**");
      TestMarkdownOne("<p><s>Text</s></p>", "<s>Text</s>");
    }
  }
}
