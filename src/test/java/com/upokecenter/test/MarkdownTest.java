package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public class MarkdownTest {
    public static Message MarkdownMessage(String str) {
      Message msg = new Message();
      msg.SetTextBody(str);
      msg.setContentType(MediaType.Parse("text/markdown\u003bcharset=utf-8"));
      return msg;
    }

    @Test
    public void TestMarkdown() {
      AppResources resources = new AppResources("Resources");
      String[] strings = DictUtility.ParseJSONStringArray(
         resources.GetString("markdown"));
      for (int i = 0; i < strings.length; i += 2) {
        Message msg = MarkdownMessage(strings[i + 1]);
        Assert.assertEquals(strings[i], msg.GetFormattedBodyString());
      }
    }
  }
