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
      this.TestMarkdownOne("<p>1.5 xyz</p>", "1.5 xyz");
      this.TestMarkdownOne("<p><em>Text</em></p>", "*Text*");
      this.TestMarkdownOne("<p><strong>Text</strong></p>", "__Text__");
      this.TestMarkdownOne("<p><strong>Text</strong></p>", "**Text**");
      this.TestMarkdownOne("<p><s>Text</s></p>", "<s>Text</s>");
      this.TestMarkdownOne("<p><a href=\"x\">y</a></p>", "[y](x)");
      this.TestMarkdownOne(
        "<p><a href=\"x\" title=\"z\">y</a></p>",
        "[y](x \"z\")");
      this.TestMarkdownOne(
    "<p><img src=\"x\" alt=\"y\" /></p>",
    "![y](x)");
      this.TestMarkdownOne("<p><a href=\"x\">y</a></p>", "[y](x)");
      this.TestMarkdownOne(
    "<p><img src=\"x\" alt=\"y\" title=\"z\" /></p>",
    "![y](x \"z\")");
      this.TestMarkdownOne(
  "<ul><li>ABC</li></ul>",
  "* ABC");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "* A\r\n* B");
      this.TestMarkdownOne(
      "<ol><li>A</li><li>B</li></ol>",
      "0. A\r\n1. B");
      this.TestMarkdownOne(
      "<ol><li>A</li><li>B</li></ol>",
      "1. A\r\n2. B");
      this.TestMarkdownOne(
      "<ol><li>A</li><li>B</li></ol>",
      "1. A\r\n999999999999. B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "+ A\r\n* B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "- A\r\n* B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "* A\r\n+ B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "+ A\r\n+ B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "- A\r\n+ B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "* A\r\n- B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "+ A\r\n- B");
      this.TestMarkdownOne(
      "<ul><li>A</li><li>B</li></ul>",
      "- A\r\n- B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p></li><li><p>B</p></li></ul>",
      "* A\r\n\r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p></li><li><p>B</p></li></ul>",
      "* A\r\n \r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p></li><li><p>B</p></li></ul>",
      "* A\r\n\r\n* B");
      this.TestMarkdownOne(
      "<ul><li>A\r\nB\r\nC</li><li>B</li></ul>",
      "* A\r\nB\r\nC\r\n* B");
      this.TestMarkdownOne(
      "<ul><li>A\r\nB\r\nC</li><li>B</li></ul>",
      "* A\r\n B\r\n C\r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p><p>B</p></li><li>B</li></ul>",
      "* A\r\n\r\n\tB\r\n* B");
      this.TestMarkdownOne(
      "<ul><li>A</li></ul><p>\u0020\u0020\u0020B</p><ul><li>B</li></ul>",
      "* A\r\n\r\n\u0020\u0020\u0020B\r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p><p>B</p></li><li>B</li></ul>",
      "* A\r\n\r\n\u0020 \u0020 B\r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p><p>B\r\nC</p></li><li>B</li></ul>",
      "* A\r\n\r\n\u0020 \u0020 B\r\nC\r\n* B");
      this.TestMarkdownOne(
  "<ul><li><p>A</p><blockquote><p>C\r\nD</p></blockquote></li><li>B</li></ul>",
  "* A\r\n\r\n\t> C\r\n\t> D\r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p><pre><code>C\r\nD</code></pre></li><li>B</li></ul>",
      "* A\r\n\r\n\t\tC\r\n\t\tD\r\n* B");
      this.TestMarkdownOne(
      "<p>A</p><pre><code>C\r\nD</code></pre>",
      "A\r\n\r\n\tC\r\n\tD");
      this.TestMarkdownOne(
      "<p>A</p><pre><code>C\r\n\tD</code></pre>",
      "A\r\n\r\n\tC\r\n\t\tD");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com/\">http://www.example.com/</a></p>",
  "<http://www.example.com/>");
      // NOTE: Obfuscation of email addresses with
      // automatic link syntax is deliberately not supported.
      this.TestMarkdownOne(
        "<p><a href=\"mailto:me@example.com\">me@example.com</a></p>",
        "<me@example.com>");
    }
    [Test]
    public void TestMarkdownAmpersand() {
      this.TestMarkdownOne(
        "<p>e&mdash;</p>",
        "e&mdash;");
      this.TestMarkdownOne(
        "<p>e&amp;</p>",
        "e&amp;");
      this.TestMarkdownOne(
        "<p>A</p><pre><code>C&amp;\r\n\tD</code></pre>",
        "A\r\n\r\n\tC&\r\n\t\tD");
      this.TestMarkdownOne(
        "<p>e&amp;x</p>",
        "e&x");
    }
    [Test]
    public void TestMarkdownRefLinks() {
      this.TestMarkdownOne(
  "<p>test</p>",
  "test\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\u005btest]: http://www.example.com \"Title\"");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: <http://www.example.com>");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\u005btest]: <http://www.example.com> \"Title\"");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com 'Title'");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n \u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u0020 \u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
        "<p>test</p><p>(Not a title)</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n(Not a title)");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n (Title)");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n \"Title\"");
      this.TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n 'Title'");
    }
    [Test]
    public void TestMarkdown2() {
      this.TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n***");
      this.TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n---");
      this.TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n___");
      this.TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n* * *");
      this.TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n- - -");
      this.TestMarkdownOne(
      "<p>A</p><hr/>",
      "A\r\n\r\n_ _ _");
      this.TestMarkdownOne(
        "<blockquote><p>A\r\nB</p><p>C</p></blockquote>",
        "> A\r\n> B\r\n> \r\n> C");
      this.TestMarkdownOne(
      "<blockquote><p>A\r\nB\r\nC</p></blockquote>",
      "> A\r\nB\r\nC");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\u005btest]: http://www.example.com \"Title\"");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext][TeSt]\r\n\r\n\u005btest]: <http://www.example.com>");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\u005btest]: <http://www.example.com> \"Title\"");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com 'Title'");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\r\n \u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\r\n\u0020 \u005btest]: http://www.example.com (Title)");
      string sx = "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n(Not a title)";
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\">Linktext</a></p><p>(Not a title)</p>",
  sx);
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n (Title)");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n \"Title\"");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n 'Title'");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\">Linktext</a></p>",
  "[Linktext] \u005bTeSt]\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext] \u005bTeSt]\r\n\u005btest]: http://www.example.com \"Title\"");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: <http://www.example.com>");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext] \u005bTeSt]\r\n\u005btest]: <http://www.example.com> \"Title\"");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com 'Title'");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
  "[Linktext] \u005bTeSt]\r\n\r\n \u005btest]: http://www.example.com (Title)");
      string s1, s2;
 s1 =
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>";

      s2 =

  "[Linktext] \u005bTeSt]\r\n\r\n\u0020 \u005btest]: http://www.example.com (Title)";
      this.TestMarkdownOne(s1, s2);

      s1 =
  "<p><a href=\"http://www.example.com\">Linktext</a></p><p>(Not a title)</p>";

      s2 =

  "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n(Not a title)";
      this.TestMarkdownOne(s1, s2);
 s1 =
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>";

      s2 =

  "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n (Title)";
      this.TestMarkdownOne(s1, s2);
 s1 =
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>";

      s2 =

  "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n \"Title\"";
      this.TestMarkdownOne(s1, s2);
 s1 =
  "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>";

      s2 =

  "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com\r\n 'Title'";
      this.TestMarkdownOne(s1, s2);
      s1 = "<p><a href=\"http://www.example.com\">tEsT</a></p>";
s2 = "[tEsT][]\r\n\u005btest]: http://www.example.com";
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\u005btest]: http://www.example.com \"Title\"");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: <http://www.example.com>");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\u005btest]: <http://www.example.com> \"Title\"");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com 'Title'");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n \u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u0020 \u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
  "<p><a href=\"http://www.example.com\">tEsT</a></p><p>(Not a title)</p>",
  "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n(Not a title)");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n (Title)");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n \"Title\"");
      this.TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n 'Title'");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" /></p>",
  "![tEsT][]\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
        "<p><img src=\"http://www.example.com\" alt=\"tEsT\" /></p>",
        "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\u005btest]: http://www.example.com \"Title\"");
      this.TestMarkdownOne(
        "<p><img src=\"http://www.example.com\" alt=\"tEsT\" /></p>",
        "![tEsT][]\r\n\r\n\u005btest]: <http://www.example.com>");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\u005btest]: <http://www.example.com> \"Title\"");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com 'Title'");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\r\n \u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\r\n\u0020 \u005btest]: http://www.example.com (Title)");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" /></p><p>(Not a title)</p>",
  "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n(Not a title)");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n (Title)");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n \"Title\"");
      this.TestMarkdownOne(
  "<p><img src=\"http://www.example.com\" alt=\"tEsT\" title=\"Title\" /></p>",
  "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n 'Title'");
    }
    [Test]
    public void TestMarkdown3() {
      this.TestMarkdownOne(
  "<blockquote><p>A</p><blockquote><p>B</p></blockquote><p>C</p></blockquote>",
  "> A\r\n> > B\r\n> \r\n> C");
      this.TestMarkdownOne(
  "<blockquote><p>A</p><blockquote><p>B\r\nC</p></blockquote></blockquote>",
  "> A\r\n> > B\r\n> C");
    }
    [Test]
    [Timeout(5000)]
    public void TestMarkdown4() {
      this.TestMarkdownOne(
  "<blockquote><pre><code>Code</code></pre></blockquote>",
  "> \tCode");
    }
    [Test]
    [Timeout(5000)]
    public void TestMarkdown5() {
      this.TestMarkdownOne(
  "<h1>Test</h1>",
  "# Test #");
      this.TestMarkdownOne(
      "<h1>Test</h1>",
      "# Test ##");
      this.TestMarkdownOne(
      "<h1>Test</h1>",
      "# Test #####");
      this.TestMarkdownOne(
      "<h1>Test</h1>",
      "# Test ##########");
    }
    [Test]
    [Timeout(5000)]
    public void TestMarkdown6() {
      this.TestMarkdownOne(
  "<p>A<br/>\r\nB</p>",
  "A\u0020 \r\nB");
      this.TestMarkdownOne(
  "<p>A \r\nB</p>",
  "A \r\nB");
      this.TestMarkdownOne(
  "<p>A<br/>\r\nB</p>",
  "A\u0020 \r\nB");
      this.TestMarkdownOne(
        "<h1>A</h1><p>A</p>",
        "A\r\n===\r\n\r\nA");
      this.TestMarkdownOne(
      "<h2>A</h2><p>A</p>",
      "A\r\n---\r\n\r\nA");
      this.TestMarkdownOne(
  "<p>C <code>abc</code> D</p>",
  "C `abc` D");
      this.TestMarkdownOne(
      "<p>C <code>abc</code> D</p>",
      "C ``abc`` D");
      this.TestMarkdownOne(
      "<p>C <code> &#x60; </code> D</p>",
      "C `` ` `` D");
      this.TestMarkdownOne(
      "<p>C &#x60;<code>abc</code> D</p>",
      "C \\``abc` D");
    }
  }
}
