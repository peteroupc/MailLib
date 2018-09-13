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
      TestMarkdownOne(
"<ul><li>ABC</li></ul>",
"* ABC");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "* A\r\n* B");
      TestMarkdownOne(
      "<ol><li>A</li><li>B</li></ol>",
      "0. A\r\n1. B");
      TestMarkdownOne(
      "<ol><li>A</li><li>B</li></ol>",
      "1. A\r\n2. B");
      TestMarkdownOne(
      "<ol><li>A</li><li>B</li></ol>",
      "1. A\r\n999999999999. B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "+ A\r\n* B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "- A\r\n* B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "* A\r\n+ B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "+ A\r\n+ B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "- A\r\n+ B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "* A\r\n- B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "+ A\r\n- B");
      TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "- A\r\n- B");
TestMarkdownOne(
"<ul><li><p>A</p></li><li><p>B</p></li></ul>",
"* A\r\n\r\n* B");
      TestMarkdownOne(
      "<ul><li><p>A</p></li><li><p>B</p></li></ul>",
      "* A\r\n   \r\n* B");
TestMarkdownOne(
"<ul><li><p>A</p></li><li><p>B</p></li></ul>",
"* A\r\n\r\n* B");
      TestMarkdownOne(
      "<ul><li>A\r\nB\r\nC</li><li>B</li></ul>",
      "* A\r\nB\r\nC\r\n* B");
      TestMarkdownOne(
      "<ul><li>A\r\nB\r\nC</li><li>B</li></ul>",
      "* A\r\n B\r\n C\r\n* B");
      TestMarkdownOne(
      "<ul><li><p>A</p><p>B</p></li><li>B</li></ul>",
      "* A\r\n\r\n\tB\r\n* B");
      TestMarkdownOne(
      "<ul><li>A\r\nB</li></ul><p>B</p><ul><li>B</li></ul>",
      "* A\r\n\r\n   B\r\n* B");
      TestMarkdownOne(
      "<ul><li><p>A</p><p>B</p></li><li>B</li></ul>",
      "* A\r\n\r\n    B\r\n* B");
      TestMarkdownOne(
      "<ul><li><p>A</p><p>B\r\nC</p></li><li>B</li></ul>",
      "* A\r\n\r\n    B\r\nC\r\n* B");
TestMarkdownOne(
"<ul><li><p>A</p><blockquote>C\r\nD</blockquote></li><li>B</li></ul>",
"* A\r\n\r\n\t> C\r\n\t> D\r\n* B");
      TestMarkdownOne(
      "<ul><li><p>A</p><pre><code>C\r\nD</code></pre></li><li>B</li></ul>",
      "* A\r\n\r\n\t\tC\r\n\t\tD\r\n* B");
      TestMarkdownOne(
      "<p>A</p><pre><code>C\r\nD</code></pre>",
      "A\r\n\r\n\tC\r\n\tD");
      TestMarkdownOne(
      "<p>A</p><pre><code>C\r\n\tD</code></pre>",
      "A\r\n\r\n\tC\r\n\t\tD");
      TestMarkdownOne(
      "<p>A</p><pre><code>C&\r\n\tD</code></pre>",
      "A\r\n\r\n\tC&amp;\r\n\t\tD");
      TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n***");
      TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n---");
      TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n___");
      TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n* * *");
      TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n- - -");
      TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n_ _ _");
TestMarkdownOne(
"<blockquote><p>A\r\nB</p><p>C</p></blockquote>",
"> A\r\n> B\r\n> \r\n> C");
      TestMarkdownOne(
      "<blockquote><p>A\r\nB\r\nC</p></blockquote>",
      "> A\r\nB\r\nC");
      TestMarkdownOne(
"<blockquote><p>A</p><blockquote>B</blockquote><p>C</p></blockquote>",
"> A\r\n> > B\r\n> C");
      TestMarkdownOne(
"<blockquote><pre><code>Code</code></pre></blockquote>",
"> \tCode");
      TestMarkdownOne(
"<h1>Test</h1>",
"# Test #");
      TestMarkdownOne(
      "<h1>Test</h1>",
      "# Test ##");
      TestMarkdownOne(
      "<h1>Test</h1>",
      "# Test #####");
      TestMarkdownOne(
      "<h1>Test</h1>",
      "# Test ##########");
      TestMarkdownOne(
"<p>A<br/>B</p>",
"A  \r\nB");
      TestMarkdownOne(
"<p>A<br/>B</p>",
"A  \r\nB");
      TestMarkdownOne(
      "A\r\n===\r\n\r\nA",
      "<h1>A</h1><p>A</p>");
      TestMarkdownOne(
      "A\r\n---\r\n\r\nA",
      "<h1>A</h1><p>A</p>");
      TestMarkdownOne(
"<p>C <code>abc</code> D</p>",
"C `abc` D");
      TestMarkdownOne(
      "<p>C <code>abc</code> D</p>",
      "C ``abc`` D");
      TestMarkdownOne(
      "<p>C <code> &#x60; </code> D</p>",
      "C `` ` `` D");
      TestMarkdownOne(
      "<p>C &#x60;<code>abc</code> D</p>",
      "C \\``abc` D");




    }
  }
}
