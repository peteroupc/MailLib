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
    }
  }
