package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public class MarkdownTest {
    public Message MarkdownMessage(String str) {
      Message msg = new Message();
      msg.SetTextBody(str);
      msg.setContentType(MediaType.Parse("text/markdown);charset=utf-8");
      return msg;
    }

    public void TestMarkdownOne(String expectedOutput,
                    String input) {
      Message msg = MarkdownMessage(input);
      Assert.assertEquals(expectedOutput, msg.GetFormattedBodyString());
    }

    @Test
    public void TestMarkdown() {
      TestMarkdownOne("<p><em>Text</em></p>", "_Text_");
      TestMarkdownOne("<p><em>Text</em></p>", "*Text*");
      TestMarkdownOne("<p><strong>Text</strong></p>", "__Text__");
      TestMarkdownOne("<p><strong>Text</strong></p>", "**Text**");
      TestMarkdownOne("<p><s>Text</s></p>", "<s>Text</s>");
    }
  }
