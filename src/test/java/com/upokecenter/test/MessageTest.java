package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public class MessageTest {
    @Test
    public void TestPseudoboundary() {
      String msgstr =
        "From: me@example.com\r\nMIME-Version: 1.0\r\nContent-Type:" +
        "\u0020multipart/mixed;boundary = BOUNDARY\r\nContent-Encoding:" +
        "\u00207bit\r\n\r\n--BOUNDARY\r\nContent-Type: text/plain\r\n\r\n" +
        "-- NOT A BOUNDARY --\r\n--NOT A BOUNDARY EITHER\r\n--BOUNDARY--";
      Message msg = MessageFromString(msgstr);
      System.out.println(msg.getContentType());
      Assert.assertEquals(
        1,
        msg.getParts().size());
    }

    public static void TestExtractHeaderOne(
      String expected,
      String msg,
      String name) {
      if (msg == null) {
        Assert.assertEquals(expected, Message.ExtractHeader(null, name));
      } else {
        byte[] bytes = com.upokecenter.util.DataUtilities.GetUtf8Bytes(msg, true);
        Assert.assertEquals(
          expected,
          Message.ExtractHeader(bytes, name));
      }
    }

    @Test
    public void TestExtractHeader() {
      TestExtractHeaderOne(null, null, "From");
      TestExtractHeaderOne(null, "From: x\r\nDate: y\r\n\r\nBody", null);
      TestExtractHeaderOne("x", "From: x\r\nDate: y\r\n\r\nBody", "from");
      TestExtractHeaderOne(
        null,
        "From: x\r\nDate: y\r\n\r\nBody",
        "f\u007from");
      TestExtractHeaderOne(
        null,
        "From: x\r\nDate: y\r\n\r\nBody",
        "other");
      TestExtractHeaderOne("x", "From: x\r\nDate: y\r\n\r\nBody", "From");
      TestExtractHeaderOne("x", "From: x\r\nDate: y\r\n\r\nBody", "fRoM");
      TestExtractHeaderOne(null, "From: x\r\nDate: y", "from");
      TestExtractHeaderOne(null, "From: x\r\nDate: y\r\n", "from");
      TestExtractHeaderOne(
        "x",
        "X-Header: w\r\nFrom: x\r\nDate:\u0020y\r\n\r\nBody",
        "from");
      TestExtractHeaderOne("x",
        "X-Header: w\r\nFrom: x\r\nDate: y\r\n\r\nBody",
        "From");
      TestExtractHeaderOne(
        "x",
        "X-Header: w\r\nFrom: x\r\nDate: y\r\n\r\nBody",
        "fRoM");
      TestExtractHeaderOne(
        "x y z",
        "X-Header: w\r\nFrom: x\r\n y\r\n\u0020z\r\n\r\nBody",
        "from");
      TestExtractHeaderOne(
        "x \u0020y z",
        "X-Header: w\r\nFrom: x\r\n \u0020y\r\n\u0020z\r\n\r\nBody",
        "from");
      TestExtractHeaderOne(
        null,
        "X-Header: w\r\n\r\nFrom: x\r\n\r\nBody",
        "from");
    }

    @Test
    public void TestMultilingual() {
      List<String> languages = Arrays.asList("en", "fr");
      ArrayList<Message> messages = new ArrayList<Message>();
      messages.add(new Message()
        .SetHeader("from", "From-Lang1 <lang@example.com>")
        .SetHeader("subject", "Subject-Lang1").SetTextBody("Body-Lang1"));
      messages.add(new Message()
        .SetHeader("from", "From-Lang2 <lang@example.com>")
        .SetHeader("subject", "Subject-Lang2").SetTextBody("Body-Lang2"));
      Message msg = Message.MakeMultilingualMessage(messages, languages);
      if (msg == null) {
        Assert.fail();
      }
      languages = Arrays.asList("fr");
      Message msg2 = msg.SelectLanguageMessage(languages);
      {
        String stringTemp = msg2.GetHeader("subject");
        Assert.assertEquals(
          "Subject-Lang2",
          stringTemp);
      }
      languages = Arrays.asList("en");
      msg2 = msg.SelectLanguageMessage(languages);
      {
        String stringTemp = msg2.GetHeader("subject");
        Assert.assertEquals(
          "Subject-Lang1",
          stringTemp);
      }
    }

    @Test
    public void TestMediaTypeEncodingSingle() {
      SingleTestMediaTypeEncoding("xy z");
      SingleTestMediaTypeEncoding("xy z");
      SingleTestMediaTypeEncoding("xy\u00a0z");
      SingleTestMediaTypeEncoding("xy\ufffdz");
      SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\ufffc", 50) +
        "z");
      SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\u00a0", 50) +
        "z");
    }

    public static String MessageGenerate(Message msg) {
      if (msg == null) {
        Assert.fail();
      }
      if (msg == null) {
        throw new NullPointerException("msg");
      }
      String ret = msg.Generate();
      if (ret == null) {
        Assert.fail();
      }
      int fmtresult = EncodingTest.IsGoodAsciiMessageFormat(
          ret,
          false,
          "");
      if (fmtresult == 1) {
        System.out.println("fmtresult=1 for " +
          ret.substring(0, Math.min(ret.length(), 260)));
      }
      String messageTemp = ret;
      if (!(
        fmtresult != 0)) {
 Assert.fail(
        messageTemp.substring(0, Math.min(messageTemp.length(), 260)));
 }
      return ret;
    }

    @Test(timeout = 5000)
    public void TestMediaTypeEncodingRoundTrip() {
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat("\"", 20) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat(" ", 20) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat(" ", 50) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat(" ", 80) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat(" ", 150) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat("\'", 150) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat("\"", 150) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat(":", 20) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat("%", 20) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat("%", 150) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat(":", 150) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat("@", 20) + "z");
      TestMediaTypeRoundTrip("xy" + EncodingTest.Repeat("@", 150) + "z");
      {
        String stringTemp = MediaType.Parse("x/y;z=1;z*=utf-8''2")
          .GetParameter("z");
        Assert.assertEquals(
          "2",
          stringTemp);
      }
    }

    static Message MessageFromString(String valueMessageString) {
      Message msgobj = new Message(
        com.upokecenter.util.DataUtilities.GetUtf8Bytes(
          valueMessageString,
          true));
      MessageGenerate(msgobj);
      return msgobj;
    }

    static void MessageConstructOnly(String valueMessageString) {
      if (new Message(com.upokecenter.util.DataUtilities.GetUtf8Bytes(
        valueMessageString,
        true)) == null) {
        Assert.fail();
      }
    }

    private static void TestMediaTypeRoundTrip(String valueMessageString) {
      String mtstring = new MediaType.Builder(
        "x",
        "y").SetParameter(
        "z",
        valueMessageString).toString();
      if (mtstring.contains("\r\n \r\n")) {
 Assert.fail();
 }
      if (mtstring.contains("\r\n \r\n")) {
 Assert.fail();
 }
      {
        Object objectTemp = valueMessageString;
        Object objectTemp2 = MediaType.Parse(mtstring).GetParameter(
          "z");
        Assert.assertEquals(mtstring, objectTemp, objectTemp2);
      }
      String msgstring = "MIME-Version: 1.0\r\nContent-Type: " +
        mtstring + "\r\n\r\n";
      Message mtmessage = MessageFromString(msgstring);
      {
        boolean boolTemp = EncodingTest.IsGoodAsciiMessageFormat(
            msgstring,
            false,
            "TestGenerate") == 2;
        if (!(
          boolTemp)) {
 Assert.fail(
          msgstring);
 }
      }
      if (MessageGenerate(mtmessage) == null) {
        Assert.fail();
      }
    }

    @Test
    public void TestGenerate() {
      ArrayList<String> msgids = new ArrayList<String>();
      // Tests whether unique Message IDs are generated for each message.
      for (int i = 0; i < 1000; ++i) {
        String msgtext = MessageGenerate(new Message().SetHeader(
          "from",
          "me@example.com")
          .SetTextBody("Hello world."));
        if (EncodingTest.IsGoodAsciiMessageFormat(
          msgtext,
          false,
          "TestGenerate") != 2) {
          Assert.fail("Bad message format generated");
        }
        String msgid = MessageFromString(msgtext).GetHeader("message-id");
        if (msgids.contains(msgid)) {
          Assert.fail(msgid);
        }
        msgids.add(msgid);
      }
    }

    @Test
    public void TestGenerateLineWrap() {
      Message msg;
      String longvalue = "name1<name1@example.com>,name2<name2@example.com>," +
        "name3<name3@example.com>,name4<name4@example.com>";
      msg = new Message();
      msg.SetHeader(
        "to",
        longvalue);
      MessageGenerate(msg);
      msg = new Message();
      msg.SetHeader(
        "cc",
        longvalue);
      MessageGenerate(msg);
      msg = new Message();
      msg.SetHeader(
        "bcc",
        longvalue);
      MessageGenerate(msg);
    }

    @Test
    public void TestMultipleReplyTo() {
      String ValueMultipleReplyTo = "Reply-to: x@example.com\r\n" +
        "Reply-to: y@example.com\r\n" + "Reply-to: z@example.com\r\n" +
        "Reply-to: w@example.com\r\n" + "From: me@example.com\r\n\r\n";
      try {
        MessageFromString(ValueMultipleReplyTo).Generate();
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    private static byte[] BytesFromString(String str) {
      return com.upokecenter.util.DataUtilities.GetUtf8Bytes(
          str,
          true);
    }

    @Test
    public void TestAddAttachment() {
      Message msg;
      String stringBody = "This is a sample body.";
      byte[] bytesBody = BytesFromString(stringBody);
      String stringPart = "This is a sample body part.";
      byte[] bytesPart = BytesFromString(stringPart);
      try {
        {
          java.io.ByteArrayInputStream ms = null;
try {
ms = new java.io.ByteArrayInputStream(bytesPart);

          MediaType mt = MediaType.TextPlainAscii;
          try {
            new Message().AddAttachment(null, mt);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddAttachment(null, (MediaType)null);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddAttachment(null, (String)null);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddAttachment(ms, (MediaType)null);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddAttachment(ms, (String)null);
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddInline(null, mt);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddInline(null, (MediaType)null);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddInline(null, (String)null);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddInline(ms, (MediaType)null);
            Assert.fail("Should have failed");
          } catch (NullPointerException ex) {
            // NOTE: Intentionally empty
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
          try {
            new Message().AddInline(ms, (String)null);
          } catch (Exception ex) {
            Assert.fail(ex.toString());
            throw new IllegalStateException("", ex);
          }
}
finally {
try { if (ms != null) { ms.close(); } } catch (java.io.IOException ex) {}
}
}
        for (int phase = 0; phase < 12; ++phase) {
          {
            java.io.ByteArrayInputStream ms = null;
try {
ms = new java.io.ByteArrayInputStream(bytesPart);

            MediaType mt = (phase % 2 == 0) ? MediaType.TextPlainAscii :
              MediaType.Parse("text/troff;charset=us-ascii");
            String fn = null;
            msg = new Message().SetTextBody(stringBody);
            Assert.assertEquals(
              "text/plain",
              msg.getContentType().getTypeAndSubType());
            switch (phase) {
              case 0:
              case 1:
                msg.AddAttachment(ms, mt);
                break;
              case 2:
              case 3:
                mt = MediaType.TextPlainAscii;
                fn = "example.txt";
                msg.AddAttachment(
                  ms,
                  fn);
                break;
              case 4:
              case 5:
                fn = "example.txt";
                msg.AddAttachment(ms, mt, fn);
                break;
              case 6:
              case 7:
                msg.AddInline(ms, mt);
                break;
              case 8:
              case 9:
                mt = MediaType.TextPlainAscii;
                fn = "example.txt";
                msg.AddInline(
                  ms,
                  fn);
                break;
              case 10:
              case 11:
                fn = "example.txt";
                msg.AddInline(ms, mt, fn);
                break;
            }
            Assert.assertEquals(
              "multipart/mixed",
              msg.getContentType().getTypeAndSubType());
            Assert.assertEquals(2, msg.getParts().size());
            Assert.assertEquals(
              "text/plain",
              msg.getParts().get(0).getContentType().getTypeAndSubType());
            Assert.assertEquals(
              "inline",
              msg.getParts().get(0).getContentDisposition().getDispositionType());
            Assert.assertEquals(
              stringBody,
              msg.getParts().get(0).GetBodyString());
            Assert.assertEquals(
              mt.getTypeAndSubType(),
              msg.getParts().get(1).getContentType().getTypeAndSubType());
            Assert.assertEquals(
              phase < 6 ? "attachment" : "inline",
              msg.getParts().get(1).getContentDisposition().getDispositionType());
            // Change to inline because noninline dispositions
            // don't support GetBodyString
            msg.getParts().get(1).setContentDisposition(ContentDisposition.Inline);
            String bodyString = null;
            try {
              bodyString = msg.getParts().get(1).GetBodyString();
            } catch (UnsupportedOperationException nsex) {
              System.out.println("phase=" + phase + "\n" +
                stringPart + "\n" + msg.getParts().get(1).Generate());
              throw new IllegalStateException("", nsex);
            }
            if (!stringPart.equals(bodyString)) {
              Assert.fail("phase=" + phase + "\n" +
                stringPart + "\n" + bodyString);
            }
}
finally {
try { if (ms != null) { ms.close(); } } catch (java.io.IOException ex) {}
}
}
        }
      } catch (Exception ioe) {
        Assert.fail(ioe.toString());
        throw new IllegalStateException("", ioe);
      }
    }

    @Test
    public void TestContentTypeDefaults() {
      String ValueStartCTD = "From: me@example.com\r\nMIME-Version:" +
        "\u00201.0\r\n";
      String msg;
      msg = ValueStartCTD + "\r\n\r\n";
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      msg = ValueStartCTD + "Content-Type: text/html\r\n\r\n";
      Assert.assertEquals(
        MediaType.Parse("text/html"),
        MessageFromString(msg).getContentType());
      msg = ValueStartCTD + "Content-Type: text/\r\n\r\n";
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      msg = ValueStartCTD + "Content-Type: /html\r\n\r\n";
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      // All header fields are syntactically valid
      msg = ValueStartCTD +

        "Content-Type: text/plain;charset=utf-8\r\nContent-Type:" +
        "\u0020image/jpeg\r\n\r\n";

      Assert.assertEquals(
        MediaType.ApplicationOctetStream,
        MessageFromString(msg).getContentType());
      msg = ValueStartCTD +

        "Content-Type: text/plain;charset=utf-8\r\nContent-Type:" +
        "\u0020image/jpeg\r\nContent-Type: text/html\r\n\r\n";

      Assert.assertEquals(
        MediaType.ApplicationOctetStream,
        MessageFromString(msg).getContentType());
      // First header field is syntactically invalid
      msg = ValueStartCTD +
        "Content-Type: /plain;charset=utf-8\r\nContent-Type:" +
        "\u0020image/jpeg\r\n\r\n";
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      // Second header field is syntactically invalid
      msg = ValueStartCTD +
        "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\n\r\n";
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      msg = ValueStartCTD +

        "Content-Type: text/plain;charset=utf-8\r\nContent-Type:" +
        "\u0020image\r\nContent-Type: text/html\r\n\r\n";

      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      // Third header field is syntactically invalid
      msg = ValueStartCTD +

        "Content-Type: text/plain;charset=utf-8\r\nContent-Type:" +
        "\u0020image/jpeg\r\nContent-Type: audio\r\n\r\n";

      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      // Unknown encoding
      msg = ValueStartCTD +

        "Content-Type: text/plain;charset=utf-8\r\nContent-Transfer-Encoding:" +
        "\u0020unknown\r\n\r\n";
      Assert.assertEquals(
        MediaType.ApplicationOctetStream,
        MessageFromString(msg).getContentType());
      // Unsupported charset
      msg = ValueStartCTD + "Content-Type: text/plain;charset=unknown\r\n\r\n";
      Assert.assertEquals(
        MediaType.ApplicationOctetStream,
        MessageFromString(msg).getContentType());
      // Unregistered ISO-8859-*
      msg = ValueStartCTD +
        "Content-Type: text/plain;charset=iso-8859-999\r\n\r\n";
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
      // Registered ISO-8859-*
      msg = ValueStartCTD +
        "Content-Type: text/plain;charset=iso-8859-2-windows-latin-2\r\n\r\n";
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MessageFromString(msg).getContentType());
    }

    @Test
    public void TestNewMessage() {
      if (!(new Message().getContentType() != null)) {
 Assert.fail();
 }
    }

    @Test
    public void TestPrematureEnd() {
      String[] messages = {
        "From: me@example.com\r\nDate",
        "From: me@example.com\r\nDate\r",
        "Received: from x",
        "Received: from x\r",
      };
      for (String msgstr : messages) {
        try {
          byte[] data = com.upokecenter.util.DataUtilities.GetUtf8Bytes(msgstr, true);
          Assert.assertEquals(
            null,
            new Message(data));
          Assert.fail("Should have failed");
        } catch (MessageDataException ex) {
          // NOTE: Intentionally empty
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
      }
    }

    private static void TestRfc2231ExtensionMediaType(
      String mtype,
      String param,
      String expected) {
      MediaType mt = MediaType.Parse("text/plain" + mtype);
      Assert.assertEquals(
        expected,
        mt.GetParameter(param));
      String valueMessageString = "From: me@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: text/plain" + mtype + "\r\n\r\nTest";
      Message msg = MessageFromString(valueMessageString);
      mt = msg.getContentType();
      Assert.assertEquals(
        expected,
        mt.GetParameter(param));
    }

    private static void TestRfc2231ExtensionContentDisposition(
      String mtype,
      String param,
      String expected) {
      ContentDisposition mt = ContentDisposition.Parse("inline" + mtype);
      Assert.assertEquals(
        expected,
        mt.GetParameter(param));
      String valueMessageString = "From: me@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: text/plain\r\nContent-Disposition: inline" + mtype +
        "\r\n\r\nTest";
      Message msg = MessageFromString(valueMessageString);
      mt = msg.getContentDisposition();
      Assert.assertEquals(
        expected,
        mt.GetParameter(param));
    }

    /*
    private void AddExt(String str) {
      str = str.replace("\\", "\\\\");
      str = str.replace("\n", "\\n");
      str = str.replace("\r", "\\r");
    }
    */

    private void TestRfc2231Extension(
      String mtype,
      String param,
      String expected) {
      TestRfc2231ExtensionMediaType(mtype, param, expected);
      TestRfc2231ExtensionContentDisposition(mtype, param, expected);
    }

    @Test
    public void TestRfc2231Extensions() {
      // Includes tests to check percent encoding at end, ensuring
      // that an infinite-decoding-loop bug does not reappear.
      // NOTE: RFC8187 doesn't mandate any particular
      // error handling behavior for those tests
      String[] strings = ResourceUtil.GetStrings("rfc2231exts");
      for (int i = 0; i < strings.length; i += 3) {
        this.TestRfc2231Extension(strings[i], strings[i + 1], strings[i + 2]);
      }
    }

    private static void SingleTestMediaTypeEncodingMediaType(String value) {
      MediaType mt = new MediaType.Builder("x", "y")
      .SetParameter("z", value).ToMediaType();
      String topLevel = mt.getTopLevelType();
      String sub = mt.getSubType();
      String mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      Message msg = MessageFromString(mtstring);
      Assert.assertEquals(
        topLevel,
        msg.getContentType().getTopLevelType());
      Assert.assertEquals(sub, msg.getContentType().getSubType());
      Assert.assertEquals(mt.toString(),value,msg.getContentType().GetParameter("z"));
    }

    private static void SingleTestMediaTypeEncodingDisposition(String value) {
      ContentDisposition mt = new ContentDisposition.Builder("inline")
      .SetParameter("z", value).ToDisposition();
      String topLevel = mt.getDispositionType();
      String mtstring = "MIME-Version: 1.0\r\nContent-Type: text/plain" +
        "\r\nContent-Disposition: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      Message msg = MessageFromString(mtstring);
      Assert.assertEquals(
        topLevel,
        msg.getContentDisposition().getDispositionType());
      Assert.assertEquals(mt.toString(),value,msg.getContentDisposition().GetParameter("z"));
    }

    public static void SingleTestMediaTypeEncoding(String value) {
      SingleTestMediaTypeEncodingMediaType(value);
      SingleTestMediaTypeEncodingDisposition(value);
    }

    @Test
    public void TestNamedAddress() {
      {
        Object objectTemp = "\"Me \" <me@example.com>";
        Object objectTemp2 = new NamedAddress(
          "Me ",
          "me@example.com").toString();
        Assert.assertEquals(
          objectTemp,
          objectTemp2);
      }
      {
        Object objectTemp = "\" Me\" <me@example.com>";
        Object objectTemp2 = new NamedAddress(
          " Me",
          "me@example.com").toString();
        Assert.assertEquals(
          objectTemp,
          objectTemp2);
      }

      try {
        Assert.assertEquals(null, new Address(""));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("a b@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("a b@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("ab.example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("ab@exa mple.example"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("ab@example.com addr"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        {
          String stringTemp = new
          NamedAddress("Me <me@example.com>").toString();
          Assert.assertEquals(
            "Me <me@example.com>",
            stringTemp);
        }
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        if (new NamedAddress("Me\u00e0 <me@example.com>") == null) {
          Assert.fail();
        }
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        if (new NamedAddress("\"Me\" <me@example.com>") == null) {
          Assert.fail();
        }
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        if (new NamedAddress("\"Me\u00e0\" <me@example.com>") == null) {
          Assert.fail();
        }
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("Me <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("Me\u00e0 <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("\"Me\" <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("\"Me\u00e0\" <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        String ValueSt = "Me <me@example.com>, Fred <fred@example.com>";
        Assert.assertEquals(
          null,
          new NamedAddress(ValueSt));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      if (new NamedAddress("x@example.com").isGroup()) {
 Assert.fail();
 }
      {
        String stringTemp = new NamedAddress("x@example.com").getName();
        Assert.assertEquals(
          "x@example.com",
          stringTemp);
      }
      {
        String stringTemp = new NamedAddress(
          "x@example.com").getAddress().toString();
        Assert.assertEquals(
          "x@example.com",
          stringTemp);
      }
      Assert.assertEquals(
        "\"(lo cal)\"@example.com",
        new Address("\"(lo cal)\"@example.com").toString());
      {
        String stringTemp = new Address("local@example.com").getLocalPart();
        Assert.assertEquals(
          "local",
          stringTemp);
      }
      {
        String stringTemp = new Address("local@example.com").getDomain();
        Assert.assertEquals(
          "example.com",
          stringTemp);
      }
    }

    @Test
    public void TestHeaderManip() {
      {
        String stringTemp = MessageFromString(
            "From: Me <me@example.com>\r\n\r\n").AddHeader("x-comment",
            "comment").GetHeader("x-comment");
        Assert.assertEquals(
          "comment",
          stringTemp);
      }
      {
        Map.Entry<String, String> kvp = new AbstractMap.SimpleImmutableEntry<String, String>("x-comment", "comment");
        String stringTemp = MessageFromString(
            "From: Me <me@example.com>\r\n\r\n")
          .AddHeader(kvp).GetHeader("x-comment");
        Assert.assertEquals(
          "comment",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
            "From: Me <me@example.com>\r\n\r\n").SetHeader(
              0,
              "you@example.com").GetHeader(0).getKey();
        Assert.assertEquals(
          "from",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
            "From: Me <me@example.com>\r\n\r\n").SetHeader(
              0,
              "you@example.com").GetHeader(0).getValue();
        Assert.assertEquals(
          "you@example.com",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
            "From: Me <me@example.com>\r\n\r\n").SetHeader(
              0,
              "x-comment",
              "comment").GetHeader(0).getKey();
        Assert.assertEquals(
          "x-comment",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
            "From: Me <me@example.com>\r\n\r\n").SetHeader(
              0,
              "x-comment",
              "comment").GetHeader(0).getValue();
        Assert.assertEquals(
          "comment",
          stringTemp);
      }
      Message msg = MessageFromString("From: Me <me@example.com>\r\n\r\n");
      try {
        msg.SetHeader(0, (String)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(0, null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.AddHeader(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(-1, "me@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(-1, "To", "me@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.GetHeader(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.RemoveHeader(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestMessageTests() {
      String ValueMultipart =
        "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";

      String msg;
      msg = ValueMultipart +

        "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\nContent-Description:" +
        "\u0020description\r\n\r\n\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = ValueMultipart +
        "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = ValueMultipart +
        "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg =

        "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type:" +
        "\u00208bit\r\n\r\n--b\r\n\r\n\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg =

        "Mime-Version: 1.0\r\nContent-Type: text/plain;" +
        "\u0020charset=UTF-8\r\nContent-Transfer-Encoding: 7bit\r\n\r\nA";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = ValueMultipart +

        "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"Me\"" +
        "\u0020<me@example.com>\r\n\r\nX\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = ValueMultipart +

        "\r\n--b\r\nContent-Type:" +
        "\u0020message/rfc822\r\nContent-Transfer-Encoding:" +
        "\u00207bit\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    public static void MessageDoubtfulCases() {
      // The following tests currently fail, since I
      // don't know the best way to handle these cases
      // without being too lenient
      String ValueMultipart =
        "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";

      String msg;
      // Multipart message with base64
      msg = ValueMultipart +
        "Content-Transfer-Encoding: base64\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message top-level-type with base64
      msg =
        "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type:" +
        "\u0020base64\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level multipart message
      msg = ValueMultipart +
        "\r\n--b\r\nContent-Type: text/plain\r\n\r\nHello World";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level multipart message
      msg = ValueMultipart +
        "\r\n--b\r\nContent-Type: text/html\r\n\r\n<b>Hello World</b>";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level ValueMultipart message
      msg = ValueMultipart + "\r\n--b\r\nContent-Type: text/html\r\n";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message/rfc822 without a content-transfer-encoding;
      // so a 7-bit encoding is assumed;
      // From header field contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = ValueMultipart +

        "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom:" +
        "\u0020\"\ufffd\ufffd\"" + "\u0020<me@example.com>\r\n\r\nX\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message/rfc822 with content-transfer-encoding base64;
      // which is not allowed for this media type
      msg = ValueMultipart +

        "\r\n--b\r\nContent-Type:" +
        "\u0020message/rfc822\r\nContent-Transfer-Encoding:" +
        "\u0020base64\r\n\r\nFrom: \"Me\"" +
        "\u0020<me@example.com>\r\n\r\nXX==\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Text/Rfc822-headers without a content-transfer-encoding;
      // so a 7-bit encoding is assumed;
      // From header field contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = ValueMultipart +

        "\r\n--b\r\nContent-Type: text/rfc822-headers\r\n\r\nFrom:" +
        "\u0020\"\ufffd\ufffd\" <me@example.com>\r\n\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Text/html without a content-transfer-encoding and an empty
      // charset, so a 7-bit encoding is assumed;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = "Mime-Version: 1.0\r\nContent-Type: text/html;";
      msg += "charset=\"\"\r\n\r\n\ufffd";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Text/html with an explicit Content-Transfer-Encoding of 7bit;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg =

        "Mime-Version: 1.0\r\nContent-Type:" +
        "\u0020text/html\r\nContent-Transfer-Encoding: 7bit\r\n\r\n\ufffd";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Text/html without a content-transfer-encoding,
      // so a 7-bit encoding is assumed;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = "Mime-Version: 1.0\r\nContent-Type: text/html\r\n\r\n\ufffd";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Base64 body with only one encoded octet;
      // should the incomplete decoded byte be ignored
      // or included?
      msg =

        "Mime-Version: 1.0\r\nContent-Type: text/plain;" +
        "\u0020charset=UTF-8\r\nContent-Transfer-Encoding: base64\r\n\r\nA";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    private static boolean EqualsStr(String a, String b) {
      return a.equals(b);
    }

    static boolean HasNestedMessageType(Message message) {
      if (EqualsStr(message.getContentType().getTopLevelType(), "message")) {
        return !EqualsStr(message.getContentType().getSubType(), "global") &&
          !EqualsStr(message.getContentType().getSubType(), "global-headers") &&
          !EqualsStr(
            message.getContentType().getSubType(),
            "global-delivery-status") && !EqualsStr(
            message.getContentType().getSubType(),
            "global-disposition-notification");
      }
      for (Message part : message.getParts()) {
        if (HasNestedMessageType(part)) {
          return true;
        }
      }
      return false;
    }

    @Test
    public void TestMostlyAscii() {
      Message msg = new Message();
      Message msg2;
      String body;
      body = EncodingTest.Repeat(
          EncodingTest.Repeat("a", 76) + "\r\n",
          5) + "\u00e7\r\nthe end";
      msg.SetTextBody(body);
      MessageGenerate(msg);
      msg2 = new Message(msg.GenerateBytes());
      {
        Object objectTemp = body;
        Object objectTemp2 = com.upokecenter.util.DataUtilities.GetUtf8String(
            msg2.GetBody(),
            false);
        Assert.assertEquals(
          objectTemp,
          objectTemp2);
      }
      body = EncodingTest.Repeat(
          EncodingTest.Repeat("a", 76) + "\r\n",
          20) + "\u00e7\r\nthe end";
      msg.SetTextBody(body);
      msg2 = new Message(msg.GenerateBytes());
      Assert.assertEquals(
        body,
        com.upokecenter.util.DataUtilities.GetUtf8String(msg2.GetBody(), false));
      MessageGenerate(msg);
    }

    @Test
    public void TestGetDate() {
      Message msg = new Message();
      int[] date;
      msg.SetHeader("date", "Sat, 1 Jan 2000 12:34:56 +1034");
      date = msg.GetDate();
      Assert.assertEquals(
        2000,
        date[0]);
      Assert.assertEquals(1, date[1]);
      Assert.assertEquals(
        1,
        date[2]);
      Assert.assertEquals(12, date[3]);
      Assert.assertEquals(
        34,
        date[4]);
      Assert.assertEquals(56, date[5]);
      Assert.assertEquals(
        0,
        date[6]);
      Assert.assertEquals((10 * 60) + 34, date[7]);
      msg.SetHeader("date", "Mon, 1 Jan 1900 23:59:60 -1034");
      date = msg.GetDate();
      Assert.assertEquals(
        1900,
        date[0]);
      Assert.assertEquals(1, date[1]);
      Assert.assertEquals(
        1,
        date[2]);
      Assert.assertEquals(23, date[3]);
      Assert.assertEquals(
        59,
        date[4]);
      Assert.assertEquals(60, date[5]);
      Assert.assertEquals(
        0,
        date[6]);
      Assert.assertEquals(-((10 * 60) + 34), date[7]);
      msg.SetHeader("date", "Sun, 1 Jan 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "1 Jan 2000 12:34:56 +1034");
      date = msg.GetDate();
      Assert.assertEquals(
        2000,
        date[0]);
      Assert.assertEquals(1, date[1]);
      Assert.assertEquals(
        1,
        date[2]);
      msg.SetHeader("date", "32 Jan 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "30 Feb 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "1 Feb 999999999999999999999 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "1 Jan 2000 24:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "1 Jan 2000 01:60:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "1 Jan 2000 01:01:61 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "1 Jan 2000 01:01:01 +1099");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader(
        "date",
        "1 Jan 2000 01:01:01 +1060");
      if (msg.GetDate() != null) {
        Assert.fail();
      }

      msg.SetHeader(
        "date",
        "1 Jan 2000 01:01:01 +1061");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
    }

    @Test
    public void TestSetDate() {
      Message msg = new Message();
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 0, 0 });
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { -1, 1, 1, 0, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 0, 1, 0, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 13, 1, 0, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 0, 0, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 32, 0, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, -5, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 24, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, -1, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 60, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, -1, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 61, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, -1, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 1000, 0 });
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, -1, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 1899, 1, 1, 0, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 1, 1, 1, 0, 0, 0, 0, 0 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 0, -1440 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 0, 1440 });
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestBoundaryReadingWithTransportPadding() {
      String messageStart = "MIME-Version: 1.0\r\n";
      messageStart += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart += "Preamble\r\n";
      String[] paddings = {
        "",
        "\u0020",
        "\t", "\u0020\u0020",
        "\t\u0020", "\u0020\t", "\t\t",
      };
      for (String padding1 : paddings) {
        for (String padding2 : paddings) {
          String message = messageStart;
          message += "--b1" + padding1 + "\r\n";
          message += "Content-Type: text/plain\r\n\r\n";
          message += "Test\r\n";
          message += "--b1--" + padding2 + "\r\n";
          message += "Epilogue";
          Message msg;
          msg = MessageFromString(message);
          Assert.assertEquals(
            "multipart",
            msg.getContentType().getTopLevelType());
          {
            String stringTemp = msg.getContentType().GetParameter("boundary");
            Assert.assertEquals(
              "b1",
              stringTemp);
          }
          Assert.assertEquals(
            1,
            msg.getParts().size());
          Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
          {
            String stringTemp = msg.getParts().get(0).GetBodyString();
            Assert.assertEquals(
              "Test",
              stringTemp);
          }
        }
      }
    }
    @Test
    public void TestBoundaryMatching() {
      String messageStart = "MIME-Version: 1.0\r\n";
      messageStart += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart += "Preamble\r\n";
      String message;
      message = messageStart;
      message += "--b1\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1BOUNDARY\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test2\r\n";
      message += "--b1BOUNDARY--\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test3\r\n";
      message += "--b1--\r\n";
      message += "Epilogue\r\n";
      message += "--b1--\r\n";
      System.out.println(message);
      Message msg;
      msg = MessageFromString(message);
      Assert.assertEquals(
        3,
        msg.getParts().size());
      Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
      {
        String stringTemp = msg.getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      {
        String stringTemp = msg.getParts().get(1).GetBodyString();
        Assert.assertEquals(
          "Test2",
          stringTemp);
      }
      {
        String stringTemp = msg.getParts().get(2).GetBodyString();
        Assert.assertEquals(
          "Test3",
          stringTemp);
      }
      message = messageStart;
      message += "--b1BOUNDARY\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--BOUNDARY\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test3\r\n";
      message += "--b1--\r\n";
      message += "Epilogue\r\n";
      message += "--b1--\r\n";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
      {
        String stringTemp = msg.getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
    }

    @Test
    public void TestBoundaryReading() {
      byte[] bodyBytes;
      Message msg;
      String message;
      String messageStart = "MIME-Version: 1.0\r\n";
      messageStart += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart += "Preamble\r\n";
      messageStart += "--b1\r\n";
      message = messageStart;
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        "multipart",
        msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
      {
        String stringTemp = msg.getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "\r\n--b2\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      // No headers in body part
      message = messageStart;
      message += "\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      {
        String stringTemp = msg.getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      // No CRLF before first boundary
      message = "MIME-Version: 1.0\r\n";
      message += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      message += "--b1\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      {
        String stringTemp = msg.getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      // Base64 body part
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        "multipart",
        msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals("application", msg.getParts().get(0).getContentType().getTopLevelType());
      bodyBytes = msg.getParts().get(0).GetBody();
      Assert.assertEquals(
        0,
        bodyBytes[0]);
      Assert.assertEquals(16, bodyBytes[1]);
      Assert.assertEquals(
        1,
        bodyBytes[2]);
      Assert.assertEquals(93, bodyBytes[3]);
      Assert.assertEquals(
        4,
        bodyBytes.length);
      // Base64 body part II
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        "multipart",
        msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals("application", msg.getParts().get(0).getContentType().getTopLevelType());
      bodyBytes = msg.getParts().get(0).GetBody();
      Assert.assertEquals(
        0,
        bodyBytes[0]);
      Assert.assertEquals(16, bodyBytes[1]);
      Assert.assertEquals(
        1,
        bodyBytes[2]);
      Assert.assertEquals(93, bodyBytes[3]);
      Assert.assertEquals(
        4,
        bodyBytes.length);
      // Base64 in nested body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n";
      message += "--b2--\r\n\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        "multipart",
        msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Message part = msg.getParts().get(0);
      Assert.assertEquals(
        "application",
        part.getParts().get(0).getContentType().getTopLevelType());
      bodyBytes = part.getParts().get(0).GetBody();
      Assert.assertEquals(
        0,
        bodyBytes[0]);
      Assert.assertEquals(16, bodyBytes[1]);
      Assert.assertEquals(
        1,
        bodyBytes[2]);
      Assert.assertEquals(93, bodyBytes[3]);
      Assert.assertEquals(
        4,
        bodyBytes.length);
      // Nested Multipart body part II
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
    }

    @Test
    public void TestGetBodyString() {
      Message msg;
      String message;
      String textpart = "--b1\r\nContent-Type: text/plain\r\n\r\nText\r\n";
      String htmlpart = "--b1\r\nContent-Type: text/html;charset=utf-8" +
        "\r\n\r\nHTML\r\n";
      String textmultipart = "--b1\r\n" +
        "Content-Type: multipart/alternative;boundary=b2" +
        "\r\n\r\n--b2\r\nContent-Type: text/plain;charset=utf-8" +
        "\r\n\r\nText\r\n";
      String htmlmultipart = "--b1\r\n" +
        "Content-Type: multipart/alternative;boundary=b2" +
        "\r\n\r\n--b2\r\nContent-Type: text/html;charset=utf-8" +
        "\r\n\r\nHTML\r\n--b2--\r\n";
      String texample = "--b1\r\nContent-Type: text/example;charset=utf-8" +
        "\r\n\r\ntext/example\r\n";
      String aexample = "--b1\r\nContent-Type: application/example" +
        "\r\n\r\napp/example\r\n";
      String example1 = "--b1\r\nContent-Type: example/w1" +
        "\r\n\r\nexample/w1\r\n";
      String example2 = "--b1\r\nContent-Type: example/w2" +
        "\r\n\r\nexample/w2\r\n";
      String endmsg = "--b1--";
      String messageStart = "MIME-Version: 1.0\r\n";
      messageStart += "Content-Type: multipart/alternative; boundary=b1" +
        "\r\n\r\n";
      message = messageStart + textpart + htmlpart + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "HTML",
          stringTemp);
      }
      message = messageStart + htmlpart + textpart + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + textpart + htmlmultipart + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "HTML",
          stringTemp);
      }
      message = messageStart + htmlpart + textmultipart + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + htmlpart + texample + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "text/example",
          stringTemp);
      }
      message = messageStart + aexample + endmsg;
      msg = MessageFromString(message);
      try {
        msg.GetBodyString();
        Assert.fail("Should have failed");
      } catch (UnsupportedOperationException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      message = messageStart + aexample + example1 + endmsg;
      msg = MessageFromString(message);
      try {
        msg.GetBodyString();
        Assert.fail("Should have failed");
      } catch (UnsupportedOperationException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      message = messageStart + example1 + example2 + aexample + endmsg;
      msg = MessageFromString(message);
      try {
        msg.GetBodyString();
        Assert.fail("Should have failed");
      } catch (UnsupportedOperationException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      message = messageStart + aexample + textpart + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + textpart + aexample + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + example1 + endmsg;
      msg = MessageFromString(message);
      try {
        msg.GetBodyString();
        Assert.fail("Should have failed");
      } catch (UnsupportedOperationException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      message = messageStart + example1 + textpart + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + example1 + textmultipart + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + textpart + example1 + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + textmultipart + example1 + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "Text",
          stringTemp);
      }
      message = messageStart + htmlpart + example1 + endmsg;
      msg = MessageFromString(message);
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "HTML",
          stringTemp);
      }
    }

    @Test
    public void TestBoundaryReading2() {
      Message msg;
      String message;
      String messageStart = "MIME-Version: 1.0\r\n";
      messageStart += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart += "Preamble\r\n";
      messageStart += "--b1\r\n";
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test\r\n";
      message += "--Not-b2--\r\n";
      message += "--b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test\r\n--Not-b2--",
          stringTemp);
      }
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test\r\n";
      message += "--Not-b2--\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test2\r\n";
      message += "--b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(2, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test\r\n--Not-b2--",
          stringTemp);
      }
      {
        String stringTemp = msg.getParts().get(0).getParts().get(1).GetBodyString();
        Assert.assertEquals(
          "Test2",
          stringTemp);
      }
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test\r\n";
      message += "--b2--\r\n";
      message += "Epilogue-for-b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test\r\n";
      message += "--b2--\r\n";
      message += "--Epilogue-for-b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test\r\n";
      message += "--b2--\r\n";
      message += "--b2--epilogue--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(
        1,
        msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      {
        String stringTemp = msg.getParts().get(0).getParts().get(0).GetBodyString();
        Assert.assertEquals(
          "Test",
          stringTemp);
      }
    }

    @Test
    public void TestAuthResults() {
      Message msg = new Message();
      try {
        msg.SetHeader(
          "authentication-results",
          "example.com from=example.net; x=y (z); from=example.org; a=b (c)");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      String hdrvalue = "a.b.c; d=e f.a=@example.com f.b=x f.c=y; g=x" +
        "\u0020(y) h.a=me@example.com";
      try {
        msg.SetHeader(
          "authentication-results",
          hdrvalue);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(
          "authentication-results",
          "a.b.c;\r\n\td=e (f) g.h=ex@example.com;\r\n\ti=j k.m=@example.com");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestArgumentValidationMediaType() {
      try {
        MediaType.TextPlainAscii.GetParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MediaType.TextPlainAscii.GetParameter("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MediaType.Parse(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      {
        String stringTemp = new MediaType.Builder().getTopLevelType();
        Assert.assertEquals(
          "application",
          stringTemp);
      }
      {
        String stringTemp = new
        MediaType.Builder(MediaType.TextPlainAscii).getTopLevelType();
        Assert.assertEquals(
          "text",
          stringTemp);
      }
      {
        String stringTemp = new
        MediaType.Builder(MediaType.TextPlainAscii).getSubType();
        Assert.assertEquals(
          "plain",
          stringTemp);
      }
      Assert.assertEquals(
        MediaType.TextPlainAscii,
        MediaType.Parse("text/plain; charset=us-ascii"));
      if (!(MediaType.TextPlainAscii.hashCode() ==
        MediaType.Parse("text/plain; charset=us-ascii")
        .hashCode())) {
 Assert.fail();
 }
    }
    @Test
    public void TestMediaTypeBuilder() {
      MediaType.Builder builder;
      try {
        Assert.assertEquals(null, new MediaType.Builder(null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      builder = new MediaType.Builder("text", "plain");
      try {
        builder.SetTopLevelType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter(null, "v");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter("", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter("v", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("e=");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("e/e");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetSubType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().RemoveParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().RemoveParameter("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().RemoveParameter("v");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetSubType("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetSubType("x;y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetSubType("x/y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter("x", "");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter("x;y", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter("x/y", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    // @Test
    public static void TestMessageMergeFields() {
      String msg;
      msg = "From: x1@example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "from");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "To: x1@example.com\r\nTo: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Cc: x1@example.com\r\nCc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "cc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Bcc: x1@example.com\r\nBcc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "bcc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Reply-To: x1@example.com\r\nReply-To: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "reply-to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-To: x1@example.com\r\nResent-To: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "resent-to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-Cc: x1@example.com\r\nResent-Cc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "resent-cc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-Bcc: x1@example.com\r\nResent-Bcc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate())
        .GetHeader("resent-bcc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      // Invalid header fields
      msg = "From: x1@example.com\r\nFrom: x2.example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "from");
      Assert.assertEquals(
        "x1@example.com",
        msg);
      msg = "From: x1.example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
        "from");
      Assert.assertEquals(
        "x2@example.com",
        msg);
    }

    @Test
    public void TestFWSAtSubjectEnd() {
      Message msg;
      String ValueStringVar = "From: me@example.com\r\nSubject:" +
        "\u0020Test\r\n " + "\r\nX-Header: Header\r\n\r\nBody";
      msg = MessageFromString(ValueStringVar);
      {
        String stringTemp = msg.GetHeader("subject");
        Assert.assertEquals(
          "Test ",
          stringTemp);
      }
    }

    @Test
    public void TestEmptyGroup() {
      String ValueStringVar = "From: me@example.com\r\nTo:" +
        "\u0020empty-group:;" +
        "\r\nCc: empty-group:;" + "\r\nBcc: empty-group:;" +
        "\r\n\r\nBody";
      MessageFromString(ValueStringVar);
    }

    @Test
    public void TestMediaTypeArgumentValidationExtra() {
      if (!(MediaType.Parse("text/plain").isText())) {
 Assert.fail();
 }
      if (!(MediaType.Parse("multipart/alternative").isMultipart())) {
 Assert.fail();
 }
      {
        String stringTemp = MediaType.Parse(
            "example/x ").getTypeAndSubType();
        Assert.assertEquals(
          "example/x",
          stringTemp);
      }
      String ValueStrtest = "example/x" + "," + " a=b";
      {
        String stringTemp = MediaType.Parse(ValueStrtest).getTypeAndSubType();
        Assert.assertEquals(
          "text/plain",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse(
            "example/x ; a=b").getTypeAndSubType();
        Assert.assertEquals(
          "example/x",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse(
            "example/x; a=b").getTypeAndSubType();
        Assert.assertEquals(
          "example/x",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse(
            "example/x; a=b ").getTypeAndSubType();
        Assert.assertEquals(
          "example/x",
          stringTemp);
      }
    }
    @Test
    public void TestContentHeadersOnlyInBodyParts() {
      Message msg = new Message().SetTextAndHtml("Hello", "Hello");
      msg.SetHeader(
        "mime-version",
        "1.0");
      msg.getParts().get(0).SetHeader("mime-version", "1.0");
      {
        String stringTemp = msg.GetHeader("mime-version");
        Assert.assertEquals(
          "1.0",
          stringTemp);
      }
      {
        String stringTemp = msg.getParts().get(0).GetHeader("mime-version");
        Assert.assertEquals(
          "1.0",
          stringTemp);
      }
      msg = MessageFromString(msg.Generate());
      {
        String stringTemp = msg.GetHeader("mime-version");
        Assert.assertEquals(
          "1.0",
          stringTemp);
      }
      Assert.assertEquals(
        null,
        msg.getParts().get(0).GetHeader("mime-version"));
    }

    @Test
    public void TestConstructor() {
      try {
        Assert.assertEquals(null, new Message((InputStream)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Message((byte[])null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly(
          "From: x@example.com\r\nSub ject: Test\r\n\r\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nX-" +
          EncodingTest.Repeat(
            "a",
            2000) + ": Test\r\n\r\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nX-" +
          EncodingTest.Repeat(
            "a",
            996) + ":\r\n Test\r\n\r\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\n: Test\r\n\r\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nSubject: Test\r\n\rBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nSubject: Test\r\n\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\nSubject: Test\n\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageFromString(
          "From: x@example.com\r\nX-" + EncodingTest.Repeat(
            "a",
            995) + ":\r\n Test\r\n\r\nBody");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageFromString("From: x@example.com\r\nX-Test:\r\n " +
          EncodingTest.Repeat("a", 997) + "\r\n\r\nBody");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestAddHeader() {
      // not implemented yet
    }
    @Test
    public void TestBccAddresses() {
      // not implemented yet
    }
    @Test
    public void TestBodyString() {
      Message msg = new Message().SetTextBody("Test");
      String mtype = "text/plain;charset=x-unknown";
      msg.setContentType(MediaType.Parse(mtype));
      try {
        System.out.print(msg.GetBodyString());
        Assert.fail("Should have failed");
      } catch (UnsupportedOperationException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestCCAddresses() {
      // not implemented yet
    }
    @Test
    public void TestContentDisposition() {
      // not implemented yet
    }
    @Test(timeout = 10000)
    public void TestContentType() {
      Message msg = new Message().SetTextBody("text");
      msg.setContentType(MediaType.Parse("text/html"));
      try {
        msg.setContentType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Non-MIME message with no Content-Type
      String msgString;
      msgString = "From: me@example.com\r\n\r\nBody";
      msg = MessageFromString(msgString);
      {
        String stringTemp = msg.getContentType().toString();
        Assert.assertEquals(
          "text/plain;charset=us-ascii",
          stringTemp);
      }
    }

    private static void TestFileNameOne(String input, String expected) {
      Message msg;
      String valueMessageString = "From: x@example.com\r\nMIME-Version:" +
        "\u00201.0\r\n" + "Content-Type: text/plain\r\nContent-Disposition: " +
        new ContentDisposition.Builder("inline").SetParameter("filename", input)
        .toString() + "\r\n\r\nEmpty.";
      msg = MessageFromString(valueMessageString);
      Assert.assertEquals(valueMessageString, expected, msg.getFileName());
      valueMessageString = "From: x@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: " + new MediaType.Builder("text", "plain")
        .SetParameter("name", input).toString() +
        "\r\n\r\nEmpty.";
      msg = MessageFromString(valueMessageString);
      Assert.assertEquals(
        expected,
        msg.getFileName());
    }
    @Test
    public void TestFileName() {
      String[] fileNames = ResourceUtil.GetStrings("filenames");
      for (int i = 0; i < fileNames.length; i += 2) {
        TestFileNameOne(fileNames[i], fileNames[i + 1]);
      }
    }
    @Test
    public void TestFromAddresses() {
      String valueMessageString =
        "From: me@example.com\r\nSubject: Subject\r\n\r\nBody";
      Message msg = MessageFromString(valueMessageString);
      MessageFromString(MessageGenerate(msg));
      List<NamedAddress> fromaddrs = msg.GetAddresses("from");
      Assert.assertEquals(
        1,
        fromaddrs.size());
    }

    @Test
    public void TestMbox() {
      // Test handling of Mbox convention at start of message
      String msgString;
      Message msg;
      msgString = "From me@example.com\r\nFrom: me2@example.com\r\n\r\nBody";
      msg = MessageFromString(msgString);
      {
        String stringTemp = msg.GetHeader("from");
        Assert.assertEquals(
          "me2@example.com",
          stringTemp);
      }
      msgString = "From : me@example.com\r\nX-From:" +
        "\u0020me2@example.com\r\n\r\nBody";
      msg = MessageFromString(msgString);
      {
        String stringTemp = msg.GetHeader("from");
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
      }
      msgString = "From: me@example.com\r\nFrom me2@example.com\r\n\r\nBody";
      try {
        MessageConstructOnly(msgString);
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msgString = "From : me@example.com\r\nFrom me2@example.com\r\n\r\nBody";
      try {
        MessageConstructOnly(msgString);
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestGetBody() {
      // not implemented yet
    }
    @Test
    public void TestGetBodyMessage() {
      // not implemented yet
    }
    @Test
    public void TestGetHeader() {
      String ValueVsv = "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
      Message msg = MessageFromString(ValueVsv);
      try {
        msg.GetHeader(2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().GetHeader(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Non-MIME message with no Content-Type
      String msgString;
      msgString = "From: me@example.com\r\n\r\nBody";
      msg = MessageFromString(msgString);
      Assert.assertEquals(
        null,
        msg.GetHeader("content-type"));
    }
    @Test
    public void TestHeaderFields() {
      // not implemented yet
    }
    @Test
    public void TestParts() {
      // not implemented yet
    }

    static final String ValueVrhs =
      "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
    @Test
    public void TestRemoveHeader() {
      Message msg = MessageFromString(ValueVrhs);
      try {
        msg.RemoveHeader(2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetBody() {
      try {
        new Message().SetBody(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetHeader() {
      try {
        String setHeaderStr = "\"a\r\n" + "b\" <x@example.com>";
        new Message().SetHeader("from", setHeaderStr);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r b\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r b\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r\n b\" <x@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r\n b\" <x@example.com>");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "=?utf-8?q?=01?= <x@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "=?utf-8?q?=01?= <x@example.com>");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"Me\" <x@example.com>");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\nb\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\0b\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      String ValueVms =
        "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
      Message msg = MessageFromString(ValueVms);
      try {
        msg.SetHeader(2, "X-Header2", "2");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(2, "2");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(1, (String)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      {
        String stringTemp = new Message()
        .SetHeader("comments", "subject").SetHeader("subject", "my subject")
        .GetHeader("subject");
        Assert.assertEquals(
          "my subject",
          stringTemp);
      }
      try {
        new Message().SetHeader(EncodingTest.Repeat("a", 998), "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e:d", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e d", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e\u007f", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e\u00a0", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e\u0008", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      String longHeader = EncodingTest.Repeat("a", 997);
      try {
        MessageGenerate(new Message().SetHeader(longHeader, "x"));
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      String[] headerNames = new String[] {
        "from", "to", "cc",
        "bcc", "reply-to",
      };
      for (String headerName : headerNames) {
        try {
          new Message().SetHeader(headerName, "\"Me\" <x@example.com>");
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
        try {
          new Message().SetHeader(headerName, "\"Me Me\" <x@example.com>");
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
        try {
          new Message().SetHeader(headerName, "\"Me(x)\" <x@example.com>");
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
        try {
          new Message().SetHeader(
            headerName,
            "\"Me\u002c Me\" <x@example.com>");
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
        try {
          new Message().SetHeader(
            headerName,
            "\"Me\u002c Me(x)\" <x@example.com>");
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
      }
    }
    @Test
    public void TestSetHtmlBody() {
      Message msg = new Message();
      try {
        msg.SetHtmlBody(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetTextAndHtml() {
      try {
        new Message().SetTextAndHtml(null, "test");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetTextAndHtml("test", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetTextBody() {
      Message msg = new Message();
      try {
        msg.SetTextBody(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    private static void TestSetHeaderOne(
      Message msg,
      String header,
      String value) {
      msg.SetHeader(header, value);
      Assert.assertEquals(
        value,
        msg.GetHeader(header));
    }

    private static void TestSetHeaderInvalid(
      Message msg,
      String header,
      String value) {
      try {
        msg.SetHeader(header, value);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestSetHeaderAcceptLanguage() {
      Message msg = new Message();
      TestSetHeaderOne(msg, "accept-language", "en-us");
      TestSetHeaderOne(msg, "accept-language", "en-us \u002c de-de");
      TestSetHeaderInvalid(msg, "accept-language", "enenenene-us\u002c de-de");
      TestSetHeaderInvalid(msg, "accept-language", "en-ususususu\u002c de-de");
      TestSetHeaderInvalid(msg, "accept-language", "en-us\u002c\u002c de-de");
      TestSetHeaderOne(msg, "accept-language", "en-us \u002c de-de");
      TestSetHeaderOne(msg, "accept-language", "en-us \u002c de-de");
      TestSetHeaderOne(msg, "accept-language", "en-us \u002c de-de");
    }

    @Test
    public void TestSetHeaderTo() {
      Message msg = new Message();
      TestSetHeaderOne(msg, "to", "\"Example Example\" <example@example.com>");
      TestSetHeaderOne(
        msg,
        "to",
        "\"Example E. Example\" <example@example.com>");
      TestSetHeaderOne(
        msg,
        "to",
        "\"Example E. Example\" <example@EXAMPLE.COM>");
      String hdrvalue = "\"Example, Example\"" +
        "\u0020<example@example.com>";
      TestSetHeaderOne(msg, "to", hdrvalue);
      TestSetHeaderOne(
        msg,
        "to",
        "\"Example\u002c Example (ABC)\" <example@example.com>");
      TestSetHeaderOne(
        msg,
        "to",
        "\"Example\u002c Example \\(ABC\\)\" <example@example.com>");
      TestSetHeaderOne(
        msg,
        "to",
        "\u0020\"Example\u002c Example\" <example@example.com>");
    }

    @Test
    public void TestSubject() {
      Message msg = new Message();
      msg.setSubject("Test");
      Assert.assertEquals(
        "Test",
        msg.getSubject());
      msg.setSubject("Test2");
      Assert.assertEquals(
        "Test2",
        msg.getSubject());
    }
    @Test
    public void TestToAddresses() {
      // not implemented yet
    }

    @Test
    public void TestParseDateStringNull() {
      if (MailDateTime.ParseDateString(null) != null) {
        Assert.fail();
      }
      if (MailDateTime.ParseDateString(null, true) != null) {
        Assert.fail();
      }
      if (MailDateTime.ParseDateString(null, false) != null) {
        Assert.fail();
      }
      if (MailDateTime.ParseDateString("") != null) {
        Assert.fail();
      }
      if (MailDateTime.ParseDateString("", true) != null) {
        Assert.fail();
      }
      if (MailDateTime.ParseDateString("", false) != null) {
        Assert.fail();
      }
    }

    @Test
    public void TestParseDateStringTrue() {
      if (MailDateTime.ParseDateString(
        "Wed\u002c 07 Jan 2015 23:23:23 GMT",
        true) == null) {
        Assert.fail();
      }
    }

    @Test
    public void TestEmptyBody() {
      Message msg = new Message();
      msg = new Message(msg.GenerateBytes());
      byte[] bytes = msg.GetBody();
      Assert.assertEquals(
        0,
        bytes.length);
    }

    @Test
    public void TestTwoToFields() {
      Message msg = new Message();
      msg.AddHeader(
        "to",
        "a@example.com");
      msg.AddHeader("to", "b@example.com");
      String msggen = msg.Generate();
      int io = msggen.indexOf(
          "To: a@example.com\u002c b@example.com");
      Assert.assertEquals(
        -1,
        io);
    }

    @Test
    public void TestDateStringHttp() {
      int[] dtime;
      dtime = MailDateTime.ParseDateStringHttp(
          "Mon\u002c 06 May 2019 01:23:45 GMT");
      Assert.assertEquals(
        2019,
        dtime[0]);
      Assert.assertEquals(5, dtime[1]);
      Assert.assertEquals(
        6,
        dtime[2]);
      Assert.assertEquals(1, dtime[3]);
      Assert.assertEquals(
        23,
        dtime[4]);
      Assert.assertEquals(45, dtime[5]);
      Assert.assertEquals(
        0,
        dtime[6]);
      Assert.assertEquals(0, dtime[7]);
      dtime = MailDateTime.ParseDateStringHttp(
          "Tue\u002c 06 May 2019 01:23:45 GMT");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp(
          "Mon 06 May 2019 01:23:45 GMT");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp(
          "Fun\u002c 06 May 2019 01:23:45 GMT");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp(
          "Monday\u002c 06 May 2019 01:23:45 GMT");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp(
          "Monday\u002c 06-May-19 01:23:45 GMT");
      Assert.assertEquals(
        2019,
        dtime[0]);
      Assert.assertEquals(5, dtime[1]);
      Assert.assertEquals(
        6,
        dtime[2]);
      Assert.assertEquals(1, dtime[3]);
      Assert.assertEquals(
        23,
        dtime[4]);
      Assert.assertEquals(45, dtime[5]);
      Assert.assertEquals(
        0,
        dtime[6]);
      Assert.assertEquals(0, dtime[7]);
      dtime = MailDateTime.ParseDateStringHttp(
          "Tuesday\u002c 06-May-19 01:23:45 GMT");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp(
          "Funday\u002c 06-May-19 01:23:45 GMT");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp(
          "Mon\u002c 06-May-19 01:23:45 GMT");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp("Mon May\u0020 6 01:23:45" +
          "\u00202019");
      Assert.assertEquals(
        2019,
        dtime[0]);
      Assert.assertEquals(5, dtime[1]);
      Assert.assertEquals(
        6,
        dtime[2]);
      Assert.assertEquals(1, dtime[3]);
      Assert.assertEquals(
        23,
        dtime[4]);
      Assert.assertEquals(45, dtime[5]);
      Assert.assertEquals(
        0,
        dtime[6]);
      Assert.assertEquals(0, dtime[7]);
      dtime = MailDateTime.ParseDateStringHttp("Mon May 13 01:23:45 2019");
      Assert.assertEquals(
        2019,
        dtime[0]);
      Assert.assertEquals(5, dtime[1]);
      Assert.assertEquals(
        13,
        dtime[2]);
      Assert.assertEquals(1, dtime[3]);
      Assert.assertEquals(
        23,
        dtime[4]);
      Assert.assertEquals(45, dtime[5]);
      Assert.assertEquals(
        0,
        dtime[6]);
      Assert.assertEquals(0, dtime[7]);
      dtime = MailDateTime.ParseDateStringHttp("Tue May 13 01:23:45 2019");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp(
          "Mon\u002c May 13 01:23:45 2019");
      if (dtime != null) {
        Assert.fail();
      }
      dtime = MailDateTime.ParseDateStringHttp("Fun May 13 01:23:45 2019");
      if (dtime != null) {
        Assert.fail();
      }
    }
    @Test
    public void TestNamedAddressNoThrow() {
      Message msg = new Message();
      NamedAddress na = new NamedAddress("abc \"def\" ghi <me@example.com>");
      System.out.println(na);
      na = new NamedAddress("abc \"def\" ghi<me@example.com>");
      System.out.println(na);
      na = new NamedAddress("abc\"def\"ghi<m=e@example.com>");
      System.out.println(na);
      na = new NamedAddress("abc\"def\"ghi<m=e@example.com>");
      System.out.println(na);
      na = new NamedAddress("abc\"a=20b=20c\"ghi<m=e@example.com>");
      System.out.println(na);
      na = new NamedAddress("abc\"a=20b=20c\"?=<m=e@example.com>");
      System.out.println(na);
      na = new NamedAddress("?abc?\"a=20b=20c\"?=<m=e@example.com>");
      System.out.println(na);
      na = new NamedAddress("=?utf-8?q?\"a=20b=20c\"?=<m=e@example.com>");
      System.out.println(na);
      na = new NamedAddress("=?utf-8?q?\"a=20b=20c\"?=<m=e@example.com>");
      System.out.println(na);
    }
  }
