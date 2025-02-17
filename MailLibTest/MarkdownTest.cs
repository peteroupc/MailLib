using System;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;

namespace MailLibTest {
  [TestFixture]
  public class MarkdownTest {
    public static Message MarkdownMessage(string str) {
      var msg = new Message();
      msg.SetTextBody(str);
      msg.ContentType = MediaType.Parse("text/markdown\u003bcharset=utf-8");
      return msg;
    }

    public static void TestMarkdownOne(
      string expectedOutput,
      string input) {
      Message msg = MarkdownMessage(input);
      Assert.AreEqual(
        expectedOutput,
        msg.GetFormattedBodyString());
    }

    [Test]
    public void TestMarkdown() {
      TestMarkdownOne("<p><em>Text</em></p>", "_Text_");
      TestMarkdownOne(
        "<p>1.5 xyz</p>",
        "1.5 xyz");
      TestMarkdownOne("<p><em>Text</em></p>", "*Text*");
      TestMarkdownOne(
        "<p><strong>Text</strong></p>",
        "__Text__");
      TestMarkdownOne("<p><strong>Text</strong></p>", "**Text**");
      TestMarkdownOne(
        "<p><s>Text</s></p>",
        "<s>Text</s>");
      TestMarkdownOne("<p><a href=\"x\">y</a></p>", "[y](x)");
      TestMarkdownOne(
        "<p><a href=\"x\" title=\"z\">y</a></p>",
        "[y](x \"z\")");
      TestMarkdownOne(
        "<p><img src=\"x\" alt=\"y\" /></p>",
        "![y](x)");
      TestMarkdownOne(
        "<p><a href=\"x\">y</a></p>",
        "[y](x)");
      TestMarkdownOne(
        "<p><img src=\"x\" alt=\"y\" title=\"z\" /></p>",
        "![y](x \"z\")");
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
        "* A\r\n \r\n* B");
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
        "<ul><li>A</li></ul><p>\u0020\u0020\u0020B</p><ul><li>B</li></ul>",
        "* A\r\n\r\n\u0020\u0020\u0020B\r\n* B");
      TestMarkdownOne(
        "<ul><li><p>A</p><p>B</p></li><li>B</li></ul>",
        "* A\r\n\r\n\u0020 \u0020 B\r\n* B");
      TestMarkdownOne(
        "<ul><li><p>A</p><p>B\r\nC</p></li><li>B</li></ul>",
        "* A\r\n\r\n\u0020 \u0020 B\r\nC\r\n* B");
      {
        string objectTemp =

  "<ul><li><p>A</p><blockquote><p>C\r\nD</p></blockquote></li><li>B</li></ul>";
        string objectTemp2 = "* A\r\n\r\n\t> C\r\n\t> D\r\n* B";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      TestMarkdownOne(
        "<ul><li><p>A</p><pre><code>C\r\nD</code></pre></li><li>B</li></ul>",
        "* A\r\n\r\n\t\tC\r\n\t\tD\r\n* B");
      TestMarkdownOne(
        "<p>A</p><pre><code>C\r\nD</code></pre>",
        "A\r\n\r\n\tC\r\n\tD");
      TestMarkdownOne(
        "<p>A</p><pre><code>C\r\n\tD</code></pre>",
        "A\r\n\r\n\tC\r\n\t\tD");
      {
        string objectTemp = "<p><a" +
  "\u0020href=\"http://www.example.com/\">http://www.example.com/</a></p>";
        string objectTemp2 = "<http://www.example.com/>";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      // NOTE: Obfuscation of email addresses with
      // automatic link syntax is deliberately not supported.
      TestMarkdownOne(
        "<p><a href=\"mailto:me@example.com\">me@example.com</a></p>",
        "<me@example.com>");
    }
    [Test]
    public void TestMarkdownAmpersand() {
      TestMarkdownOne(
        "<p>e&#x2014;</p>",
        "e&#x2014;");
      TestMarkdownOne(
        "<p>e&amp;</p>",
        "e&amp;");
      TestMarkdownOne(
        "<p>A</p><pre><code>C&a" + "mp;\r\n\tD</code></pre>",
        "A\r\n\r\n\tC&\r\n\t\tD");
      TestMarkdownOne(
        "<p>e&amp;x</p>",
        "e&x");
    }
    [Test]
    public void TestMarkdownRefLinks() {
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\u005btest]: http://www.example.com \"Title\"");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: <http://www.example.com>");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\u005btest]: <http://www.example.com> \"Title\"");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com 'Title'");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n \u005btest]: http://www.example.com (Title)");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n \u005btest]: http://www.example.com (Title)");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u0020 \u005btest]: http://www.example.com (Title)");
      TestMarkdownOne(
        "<p>test</p><p>(Not a title)</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n(Not a title)");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n (Title)");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n \"Title\"");
      TestMarkdownOne(
        "<p>test</p>",
        "test\r\n\r\n\u005btest]: http://www.example.com\r\n 'Title'");
    }
    [Test]
    public void TestMarkdown2() {
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
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext][TeSt]\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext][TeSt]\r\n\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
        "[Linktext][TeSt]\r\n\u005btest]: http://www.example.com\u0020\"Title\"");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext][TeSt]\r\n\r\n\u005btest]: <http://www.example.com>");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">Linktext</a></p>",
        "[Linktext][TeSt]\r\n\u005btest]: <http://www.example.com>\u0020\"Title\"");
    }
    [Test]
    public void TestMarkdown2a() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com 'Title'";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2b() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com (Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2c() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n \u005btest]:" +
        "\u0020http://www.example.com (Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2d() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n\u0020 \u005btest]:" +
        "\u0020http://www.example.com (Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2e() {
      string objectTemp = "<p><a" +
        "\u0020href=\"http://www.example.com\">Linktext</a></p><p>(Not a" +
        "\u0020title\u0029</p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com\r\n(Not a title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2f() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com\r\n (Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2g() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com\r\n \"Title\"";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2h() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext][TeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com\r\n 'Title'";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2i() {
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext] \u005bTeSt]\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: http://www.example.com");
    }
    [Test]
    public void TestMarkdown2j() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\u005btest]:" +
        "\u0020http://www.example.com \"Title\"";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2k() {
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">Linktext</a></p>",
        "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: <http://www.example.com>");
    }
    [Test]
    public void TestMarkdown2l() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\u005btest]:" +
        "\u0020<http://www.example.com> \"Title\"";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }

    [Test]
    public void TestMarkdown2m() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com 'Title'";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2n() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com (Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2o() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n \u005btest]:" +
        "\u0020http://www.example.com (Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2p() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n\u0020" +
        "\u0020\u005btest]: " + "http://www.example.com (Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }

    [Test]
    public void TestMarkdown2q() {
      string objectTemp = "<p><a" +
        "\u0020href=\"http://www.example.com\">Linktext</a></p><p>(Not a" +
        "\u0020title)</p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com\r\n(No" +
        "t a title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2r() {
      string objectTemp = "<p><a href=\"http://www.example.com\" " +
        "title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]: " +
        "http://www.example.com\r\n" +
        "\u0020(Title)";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2s() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        "\u0020title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]:" +
        "\u0020http://www.example.com\r\n" +
        " \"Title\"";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }
    [Test]
    public void TestMarkdown2t() {
      string objectTemp = "<p><a href=\"http://www.example.com\"" +
        " title=\"Title\">Linktext</a></p>";
      string objectTemp2 = "[Linktext] \u005bTeSt]\r\n\r\n\u005btest]:" +
        " http://www.example.com\r\n" +
        " 'Title'";
      TestMarkdownOne(
        objectTemp,
        objectTemp2);
    }

    [Test]
    public void TestMarkdown2u() {
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">tEsT</a></p>",
        "[tEsT][]\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\u005btest]: http://www.example.com \"Title\"");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: <http://www.example.com>");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\u005btest]: <http://www.example.com> \"Title\"");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com 'Title'");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n \u005btest]: http://www.example.com (Title)");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n \u005btest]: http://www.example.com (Title)");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u0020 \u005btest]: http://www.example.com (Title)");
    }

    [Test]
    public void TestMarkdown2v() {
      string stringTemp = "<p><a" +
        "\u0020href=\"http://www.example.com\">tEsT</a></p>" +
        "<p>(Not a title)</p>";
      string stringTemp2 =
        "[tEsT][]\r\n\r\n\u005btest]:\u0020http://www.example.com\r\n(Not" +
        "\u0020a title)";
      TestMarkdownOne(stringTemp, stringTemp2);
    }
    [Test]
    public void TestMarkdown2w() {
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n (Title)");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n \"Title\"");
      TestMarkdownOne(
        "<p><a href=\"http://www.example.com\" title=\"Title\">tEsT</a></p>",
        "[tEsT][]\r\n\r\n\u005btest]: http://www.example.com\r\n 'Title'");
      TestMarkdownOne(
        "<p><img src=\"http://www.example.com\" alt=\"tEsT\" /></p>",
        "![tEsT][]\r\n\u005btest]: http://www.example.com");
      TestMarkdownOne(
        "<p><img src=\"http://www.example.com\" alt=\"tEsT\" /></p>",
        "![tEsT][]\r\n\r\n\u005btest]: http://www.example.com");
      {
        string stringTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string stringTemp2 =
          "![tEsT][]\r\n\u005btest]:\u0020http://www.example.com \"Title\"";
        TestMarkdownOne(stringTemp, stringTemp2);
      }
      TestMarkdownOne(
        "<p><img src=\"http://www.example.com\" alt=\"tEsT\" /></p>",
        "![tEsT][]\r\n\r\n\u005btest]: <http://www.example.com>");
      {
        string stringTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string stringTemp2 =
          "![tEsT][]\r\n\u005btest]:\u0020<http://www.example.com> \"Title\"";
        TestMarkdownOne(stringTemp, stringTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n\u005btest]:" +
          "\u0020http://www.example.com 'Title'";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n\u005btest]:" +
          "\u0020http://www.example.com (Title)";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n \u005btest]:" +
          "\u0020http://www.example.com (Title)";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n\u0020 \u005btest]:" +
          "\u0020http://www.example.com (Title)";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" /></p><p>(Not a title)</p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n\u005btest]:" +
          "\u0020http://www.example.com\r\n(Not a title)";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n\u005btest]:" +
          "\u0020http://www.example.com\r\n (Title)";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n\u005btest]:" +
          "\u0020http://www.example.com\r\n \"Title\"";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
      {
        string objectTemp = "<p><img src=\"http://www.example.com\"" +
          "\u0020alt=\"tEsT\" title=\"Title\" /></p>";
        string objectTemp2 = "![tEsT][]\r\n\r\n\u005btest]:" +
          "\u0020http://www.example.com\r\n 'Title'";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
    }
    [Test]
    public void TestMarkdown3() {
      TestMarkdownOne(
  "<blockquote><p>A</p><blockquote><p>B</p></blockquote><p>C</p></blockquote>",
  "> A\r\n> > B\r\n> \r\n> C");
      {
        string objectTemp =
  "<blockquote><p>A</p><blockquote><p>B\r\nC</p></blockquote></blockquote>";
        string objectTemp2 = "> A\r\n> > B\r\n> C";
        TestMarkdownOne(
          objectTemp,
          objectTemp2);
      }
    }
    [Test]
    [Timeout(5000)]
    public void TestMarkdown4() {
      TestMarkdownOne(
        "<blockquote><pre><code>Code</code></pre></blockquote>",
        "> \tCode");
    }
    [Test]
    [Timeout(5000)]
    public void TestMarkdown5() {
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
    }
    [Test]
    [Timeout(5000)]
    public void TestMarkdown6() {
      TestMarkdownOne(
        "<p>A<br/>\r\nB</p>",
        "A\u0020 \r\nB");
      TestMarkdownOne(
        "<p>A \r\nB</p>",
        "A \r\nB");
      TestMarkdownOne(
        "<p>A<br/>\r\nB</p>",
        "A\u0020 \r\nB");
      TestMarkdownOne(
        "<h1>A</h1><p>A</p>",
        "A\r\n===\r\n\r\nA");
      TestMarkdownOne(
        "<h2>A</h2><p>A</p>",
        "A\r\n---\r\n\r\nA");
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
