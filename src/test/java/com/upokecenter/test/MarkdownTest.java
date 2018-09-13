package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public class MarkdownTest {
    public Message MarkdownMessage(String str) {
      Message msg = new Message();
      msg.SetTextBody(str);
      msg.setContentType(MediaType.Parse("text/markdown\u003bcharset=utf-8"));
      return msg;
    }

    public void TestMarkdownOne(
  String expectedOutput,
  String input) {
      Message msg = this.MarkdownMessage(input);
      Assert.assertEquals(expectedOutput, msg.GetFormattedBodyString());
    }

    @Test
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
      "<ul><li>A</li></ul><p> B</p><ul><li>B</li></ul>",
      "* A\r\n\r\n B\r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p><p>B</p></li><li>B</li></ul>",
      "* A\r\n\r\n B\r\n* B");
      this.TestMarkdownOne(
      "<ul><li><p>A</p><p>B\r\nC</p></li><li>B</li></ul>",
      "* A\r\n\r\n B\r\nC\r\n* B");
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
    }
    @Test
    public void TestMarkdownAmpersand() {
      this.TestMarkdownOne(
      "<p>A</p><pre><code>C&\r\n\tD</code></pre>",
      "A\r\n\r\n\tC&amp;\r\n\t\tD");
    }
    @Test
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
    }
    @Test
    public void TestMarkdown3() {
      this.TestMarkdownOne(
  "<blockquote><p>A</p><blockquote><p>B</p></blockquote><p>C</p></blockquote>",
  "> A\r\n> > B\r\n> C");
    }
    @Test(timeout = 5000)
    public void TestMarkdown4() {
      this.TestMarkdownOne(
  "<blockquote><pre><code>Code</code></pre></blockquote>",
  "> \tCode");
    }
    @Test(timeout = 5000)
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
    @Test(timeout = 5000)
    public void TestMarkdown6() {
      this.TestMarkdownOne(
  "<p>A<br/>\r\nB</p>",
  "A \r\nB");
      this.TestMarkdownOne(
  "<p>A \r\nB</p>",
  "A \r\nB");
      this.TestMarkdownOne(
  "<p>A<br/>\r\nB</p>",
  "A \r\nB");
      this.TestMarkdownOne(
      "A\r\n===\r\n\r\nA",
      "<h1>A</h1><p>A</p>");
      this.TestMarkdownOne(
      "A\r\n---\r\n\r\nA",
      "<h1>A</h1><p>A</p>");
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
