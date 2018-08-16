package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public class MessageTest {
    @Test
    public static void TestPseudoboundary() {
      String msgstr =
  "From: me@example.com\r\nMIME-Version: 1.0\r\nContent-Type: multipart/mixed;boundary=BOUNDARY\r\nContent-Encoding: 7bit\r\n\r\n--BOUNDARY\r\nContent-Type: text/plain\r\n\r\n"
        +
        "-- NOT A BOUNDARY --\r\n--NOT A BOUNDARY EITHER\r\n--BOUNDARY--";
      Message msg = MessageFromString(msgstr);
      System.out.println(msg.getContentType());
      Assert.assertEquals(1, msg.getParts().size());
    }

    @Test
    public void TestMultilingual() {
      List<String> languages =
        new ArrayList<String>(new String[] { "en", "fr" });
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
      languages = new ArrayList<String>(new String[] { "fr" });
      Message msg2 = msg.SelectLanguageMessage(languages);
      {
String stringTemp = msg2.GetHeader("subject");
Assert.assertEquals(
  "Subject-Lang2",
  stringTemp);
}
      languages = new ArrayList<String>(new String[] { "en" });
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
      SingleTestMediaTypeEncoding("xyz");
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
  DataUtilities.GetUtf8Bytes(
  valueMessageString,
  true));
      MessageGenerate(msgobj);
      return msgobj;
    }

    static void MessageConstructOnly(String valueMessageString) {
      if ((new Message(
  DataUtilities.GetUtf8Bytes(
  valueMessageString,
  true))) == null) {
        Assert.fail();
      }
    }

    private static void TestMediaTypeRoundTrip(String valueMessageString) {
      String mtstring = new MediaTypeBuilder(
  "x",
  "y").SetParameter(
  "z",
  valueMessageString).toString();
      if (mtstring.contains("\r\n\r\n")) {
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
        if (!(boolTemp)) {
 Assert.fail(msgstring);
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
      msg.SetHeader("to", longvalue);
      MessageGenerate(msg);
      msg = new Message();
      msg.SetHeader("cc", longvalue);
      MessageGenerate(msg);
      msg = new Message();
      msg.SetHeader("bcc", longvalue);
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
      return DataUtilities.GetUtf8Bytes(
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
try { if (ms != null) {
 ms.close();
 } } catch (java.io.IOException ex) {}
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
            Assert.assertEquals("text/plain", msg.getContentType().getTypeAndSubType());
            switch (phase) {
              case 0:
              case 1: msg.AddAttachment(ms, mt); break;
              case 2:
              case 3:
                mt = MediaType.TextPlainAscii;
                fn = "example.txt";
                msg.AddAttachment(ms, fn); break;
              case 4:
              case 5:
                fn = "example.txt";
                msg.AddAttachment(ms, mt, fn); break;
              case 6:
              case 7: msg.AddInline(ms, mt); break;
              case 8:
              case 9:
                mt = MediaType.TextPlainAscii;
                fn = "example.txt";
                msg.AddInline(ms, fn); break;
              case 10:
              case 11:
                fn = "example.txt";
                msg.AddInline(ms, mt, fn); break;
            }
            Assert.assertEquals("multipart/mixed", msg.getContentType().getTypeAndSubType());
            Assert.assertEquals(2, msg.getParts().size());
        Assert.assertEquals(
  "text/plain",
  msg.getParts().get(0).getContentType().getTypeAndSubType());
    Assert.assertEquals(
  "inline",
  msg.getParts().get(0).getContentDisposition().getDispositionType());
            Assert.assertEquals(stringBody, msg.getParts().get(0).getBodyString());
   Assert.assertEquals(
  mt.getTypeAndSubType(),
  msg.getParts().get(1).getContentType().getTypeAndSubType());
            Assert.assertEquals(
               phase < 6 ? "attachment" : "inline",
               msg.getParts().get(1).getContentDisposition().getDispositionType());
            Assert.assertEquals(stringPart, msg.getParts().get(1).getBodyString());
}
finally {
try { if (ms != null) {
 ms.close();
 } } catch (java.io.IOException ex) {}
}
}
        }
      } catch (IOException ioe) {
        Assert.fail(ioe.toString());
        throw new IllegalStateException("", ioe);
      }
    }

    @Test
    public void TestContentTypeDefaults() {
  String ValueStartCTD = "From: me@example.com\r\nMIME-Version: 1.0\r\n";
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

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";

      Assert.assertEquals(
  MediaType.ApplicationOctetStream,
  MessageFromString(msg).getContentType());
      msg = ValueStartCTD +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: text/html\r\n\r\n";

      Assert.assertEquals(
  MediaType.ApplicationOctetStream,
  MessageFromString(msg).getContentType());
      // First header field is syntactically invalid
      msg = ValueStartCTD +
  "Content-Type: /plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";
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

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\nContent-Type: text/html\r\n\r\n";

      Assert.assertEquals(
  MediaType.TextPlainAscii,
  MessageFromString(msg).getContentType());
      // Third header field is syntactically invalid
      msg = ValueStartCTD +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: audio\r\n\r\n";

      Assert.assertEquals(
  MediaType.TextPlainAscii,
  MessageFromString(msg).getContentType());
      // Unknown encoding
      msg = ValueStartCTD +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Transfer-Encoding: unknown\r\n\r\n";
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
        "Received: from x\r"
      };
      for (String msgstr : messages) {
        try {
          byte[] data = DataUtilities.GetUtf8Bytes(msgstr, true);
          Assert.assertEquals(null, new Message(data));
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
      Assert.assertEquals(expected, mt.GetParameter(param));
      String valueMessageString = "From: me@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: text/plain" + mtype + "\r\n\r\nTest";
      Message msg = MessageFromString(valueMessageString);
      mt = msg.getContentType();
      Assert.assertEquals(expected, mt.GetParameter(param));
    }

    private static void TestRfc2231ExtensionContentDisposition(
  String mtype,
  String param,
  String expected) {
      ContentDisposition mt = ContentDisposition.Parse("inline" + mtype);
      Assert.assertEquals(expected, mt.GetParameter(param));
      String valueMessageString = "From: me@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: text/plain\r\nContent-Disposition: inline" + mtype +
          "\r\n\r\nTest";
      Message msg = MessageFromString(valueMessageString);
      mt = msg.getContentDisposition();
      Assert.assertEquals(expected, mt.GetParameter(param));
    }

    public static void TestRfc2231Extension(
  String mtype,
  String param,
  String expected) {
      TestRfc2231ExtensionMediaType(mtype, param, expected);
      TestRfc2231ExtensionContentDisposition(mtype, param, expected);
    }

    @Test
    public void TestRfc2231ExtensionsEndPercent() {
      // Tests to check percent encoding at end, ensuring
      // that an infinite-decoding-loop bug does not reappear.
      // NOTE: RFC8187 doesn't mandate any particular
      // error handling behavior here
      TestRfc2231Extension(";param1*=utf-8''example%", "param1", "example%");
      TestRfc2231Extension(
  ";param1*=utf-8''example%;param2=x",
  "param1",
  "example%");
      TestRfc2231Extension(
  ";param2=x;param1*=utf-8''example%",
  "param1",
  "example%");
      TestRfc2231Extension(";param1*=utf-8''example%a", "param1", "example%a");
      TestRfc2231Extension(
  ";param1*=utf-8''example%a;param2=x",
  "param1",
  "example%a");
      TestRfc2231Extension(
  ";param2=x;param1*=utf-8''example%a",
  "param1",
  "example%a");
      TestRfc2231Extension(";param1*=utf-8''example%A", "param1", "example%A");
      TestRfc2231Extension(
  ";param1*=utf-8''example%A;param2=x",
  "param1",
  "example%A");
      TestRfc2231Extension(
  ";param2=x;param1*=utf-8''example%A",
  "param1",
  "example%A");
      TestRfc2231Extension(";param1*=utf-8''example%9", "param1", "example%9");
      TestRfc2231Extension(
  ";param1*=utf-8''example%9;param2=x",
  "param1",
  "example%9");
      TestRfc2231Extension(
  ";param2=x;param1*=utf-8''example%9",
  "param1",
  "example%9");
      TestRfc2231Extension(";param1*=utf-8''example%w", "param1", "example%w");
      TestRfc2231Extension(
  ";param1*=utf-8''example%w;param2=x",
  "param1",
  "example%w");
      TestRfc2231Extension(
  ";param2=x;param1*=utf-8''example%w",
  "param1",
  "example%w");
    }

    @Test
    public void TestRfc2231Extensions() {
      TestRfc2231Extension("; charset=\"utf-8\"", "charset", "utf-8");
      TestRfc2231Extension(
  "; charset*=us-ascii'en'utf-8",
  "charset",
  "utf-8");
      TestRfc2231Extension(
  "; charset*=us-ascii''utf-8",
  "charset",
  "utf-8");
      TestRfc2231Extension(
  "; charset*='en'utf-8",
  "charset",
  "utf-8");
      TestRfc2231Extension("; charset*=''utf-8", "charset", "utf-8");
      TestRfc2231Extension(
  "; charset*0=ut;charset*1=f-8",
  "charset",
  "utf-8");
      TestRfc2231Extension(
        "; charset*0=ut;charset*1=f;charset*2=-8",
        "charset",
        "utf-8");
      TestRfc2231Extension(
  "; mockcharset*=utf-8''a%20b",
  "mockcharset",
  "a b");
      TestRfc2231Extension(
  "; mockcharset*=iso-8859-1''a%a0b",
  "mockcharset",
  "a\u00a0b");
      TestRfc2231Extension(
  "; mockcharset*=utf-8''a%c2%a0b",
  "mockcharset",
  "a\u00a0b");
      TestRfc2231Extension(
  "; mockcharset*=iso-8859-1''a%a0b",
  "mockcharset",
  "a\u00a0b");
      TestRfc2231Extension(
  "; mockcharset*=utf-8''a%c2%a0b",
  "mockcharset",
  "a\u00a0b");
      TestRfc2231Extension(
  "; mockcharset*0=\"a\";mockcharset*1=b",
  "mockcharset",
  "ab");

      TestRfc2231Extension(
      "; mockcharset*0*=utf-8''a%20b;mockcharset*1*=c%20d",
      "mockcharset",
      "a bc d");
      TestRfc2231Extension(
        "; mockcharset*0=ab;mockcharset*1*=iso-8859-1-en-xyz",
        "mockcharset",
        "abiso-8859-1-en-xyz");
      TestRfc2231Extension(
        "; mockcharset*0*=utf-8''a%20b;mockcharset*1*=iso-8859-1-en-xyz",
        "mockcharset",
        "a biso-8859-1-en-xyz");
      TestRfc2231Extension(
        "; mockcharset*0*=utf-8''a%20b;mockcharset*1=a%20b",
        "mockcharset",
        "a ba%20b");
      TestRfc2231Extension(
         "; mockCharset*0*=utf-8''a%20b;mockcHarset*1=a%20b",
         "mockcharset",
         "a ba%20b");
      TestRfc2231Extension(
         "; mockCharset*0*=utf-8''a%20b;mockcHarset*1=\"a%20b\"",
         "mockcharset",
         "a ba%20b");
      TestRfc2231Extension(
        "\r\n (; mockcharset=x;y=\");mockChaRseT*=''a%41b-c(\")",
        "mockcharset",
        "aAb-c");
      TestRfc2231Extension(
        ";\r\n mockchARSet (xx=y) = (\"z;) abc (d;e\") ; format = flowed",
        "mockcharset",
        "abc");
      TestRfc2231Extension(
        ";\r\n mockcharsET (xx=y) = (\"z;) abc (d;e\") ; format = flowed",
        "format",
        "flowed");
    }

    private static void SingleTestMediaTypeEncodingMediaType(String value) {
      MediaType mt = new MediaTypeBuilder("x", "y")
         .SetParameter("z", value).ToMediaType();
      String topLevel = mt.getTopLevelType();
      String sub = mt.getSubType();
      String mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      Message msg = MessageFromString(mtstring);
      Assert.assertEquals(topLevel, msg.getContentType().getTopLevelType());
      Assert.assertEquals(sub, msg.getContentType().getSubType());
      Assert.assertEquals(mt.toString(),value,msg.getContentType().GetParameter("z"));
    }

    private static void SingleTestMediaTypeEncodingDisposition(String value) {
      ContentDisposition mt = new DispositionBuilder("inline")
         .SetParameter("z", value).ToDisposition();
      String topLevel = mt.getDispositionType();
      String mtstring = "MIME-Version: 1.0\r\nContent-Type: text/plain" +
        "\r\nContent-Disposition: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      Message msg = MessageFromString(mtstring);
      Assert.assertEquals(topLevel, msg.getContentDisposition().getDispositionType());
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
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "\" Me\" <me@example.com>";
        Object objectTemp2 = new NamedAddress(
          " Me",
          "me@example.com").toString();
        Assert.assertEquals(objectTemp, objectTemp2);
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
        if ((new NamedAddress("Me\u00e0 <me@example.com>")) == null) {
          Assert.fail();
        }
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        if ((new NamedAddress("\"Me\" <me@example.com>")) == null) {
          Assert.fail();
        }
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        if ((new NamedAddress("\"Me\u00e0\" <me@example.com>")) == null) {
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
        Assert.assertEquals(null, new NamedAddress(ValueSt));
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
          "From: Me <me@example.com>\r\n\r\n").AddHeader("x-comment", "comment")
.GetHeader("x-comment");
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

  "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\nContent-Description: description\r\n\r\n\r\n--b--";

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

  "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg =

  "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: 7bit\r\n\r\nA";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: 7bit\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";

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

  "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: base64\r\n--b\r\n\r\n\r\n--b--";

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

  "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\nX\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message/rfc822 with content-transfer-encoding base64;
      // which is not allowed for this media type
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: base64\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nXX==\r\n--b--";

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

  "\r\n--b\r\nContent-Type: text/rfc822-headers\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\n--b--";

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
      msg =
  "Mime-Version: 1.0\r\nContent-Type: text/html; charset=\"\"\r\n\r\n\ufffd";

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

  "Mime-Version: 1.0\r\nContent-Type: text/html\r\nContent-Transfer-Encoding: 7bit\r\n\r\n\ufffd";

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

  "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: base64\r\n\r\nA";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    static boolean HasNestedMessageType(Message message) {
      if (message.getContentType().getTopLevelType().equals("message")) {
        return (!message.getContentType().getSubType().equals("global")) &&
          ((!message.getContentType().getSubType().equals("global-headers")) &&
           (message.getContentType().getSubType().equals("global-delivery-status") ||
    message.getContentType().getSubType().equals("global-disposition-notification")));
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
       5) +
  "\u00e7\r\nthe end";
      msg.SetTextBody(body);
      MessageGenerate(msg);
      msg2 = new Message(msg.GenerateBytes());
      Assert.assertEquals(body, DataUtilities.GetUtf8String(msg2.GetBody(), false));
      body = EncodingTest.Repeat(
                    EncodingTest.Repeat("a", 76) + "\r\n",
                    20) +
   "\u00e7\r\nthe end";
      msg.SetTextBody(body);
      msg2 = new Message(msg.GenerateBytes());
      Assert.assertEquals(
  body,
  DataUtilities.GetUtf8String(msg2.GetBody(), false));
      MessageGenerate(msg);
    }

    @Test
    public void TestGetDate() {
      Message msg = new Message();
      int[] date;
      msg.SetHeader("date", "Sat, 1 Jan 2000 12:34:56 +1034");
      date = msg.GetDate();
      Assert.assertEquals(2000, date[0]);
      Assert.assertEquals(1, date[1]);
      Assert.assertEquals(1, date[2]);
      Assert.assertEquals(12, date[3]);
      Assert.assertEquals(34, date[4]);
      Assert.assertEquals(56, date[5]);
      Assert.assertEquals(0, date[6]);
      Assert.assertEquals((10 * 60) + 34, date[7]);
      msg.SetHeader("date", "Mon, 1 Jan 1900 23:59:60 -1034");
      date = msg.GetDate();
      Assert.assertEquals(1900, date[0]);
      Assert.assertEquals(1, date[1]);
      Assert.assertEquals(1, date[2]);
      Assert.assertEquals(23, date[3]);
      Assert.assertEquals(59, date[4]);
      Assert.assertEquals(60, date[5]);
      Assert.assertEquals(0, date[6]);
      Assert.assertEquals(-((10 * 60) + 34), date[7]);
      msg.SetHeader("date", "Sun, 1 Jan 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "1 Jan 2000 12:34:56 +1034");
      date = msg.GetDate();
      Assert.assertEquals(2000, date[0]);
      Assert.assertEquals(1, date[1]);
      Assert.assertEquals(1, date[2]);
      msg.SetHeader("date", "32 Jan 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "30 Feb 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "1 Feb 999999999999999999999 12:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "1 Jan 2000 24:34:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "1 Jan 2000 01:60:56 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "1 Jan 2000 01:01:61 +1034");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "1 Jan 2000 01:01:01 +1099");
      if (msg.GetDate() != null) {
        Assert.fail();
      }
      msg.SetHeader("date", "1 Jan 2000 01:01:01 +1060");
      if (msg.GetDate() != null) {
        Assert.fail();
      }

      msg.SetHeader("date", "1 Jan 2000 01:01:01 +1061");
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
          "\t\u0020", "\u0020\t", "\t\t"
      };
      for (Object padding1 : paddings) {
        for (Object padding2 : paddings) {
          String message = messageStart;
          message += "--b1" + padding1 + "\r\n";
          message += "Content-Type: text/plain\r\n\r\n";
          message += "Test\r\n";
          message += "--b1--" + padding2 + "\r\n";
          message += "Epilogue";
          Message msg;
          msg = MessageFromString(message);
          Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
          {
            String stringTemp = msg.getContentType().GetParameter("boundary");
            Assert.assertEquals(
              "b1",
              stringTemp);
          }
          Assert.assertEquals(1, msg.getParts().size());
          Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
          Assert.assertEquals("Test", msg.getParts().get(0).getBodyString());
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
      Assert.assertEquals(3, msg.getParts().size());
      Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
      Assert.assertEquals("Test", msg.getParts().get(0).getBodyString());
      Assert.assertEquals("Test2", msg.getParts().get(1).getBodyString());
      Assert.assertEquals("Test3", msg.getParts().get(2).getBodyString());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
      Assert.assertEquals("Test", msg.getParts().get(0).getBodyString());
    }

    @Test
    public void TestBoundaryReading() {
      byte[] body;
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
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("text", msg.getParts().get(0).getContentType().getTopLevelType());
      Assert.assertEquals("Test", msg.getParts().get(0).getBodyString());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getParts().get(0).getBodyString());
      // No headers in body part
      message = messageStart;
      message += "\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getBodyString());
      // No CRLF before first boundary
      message = "MIME-Version: 1.0\r\n";
      message += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      message += "--b1\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getBodyString());
      // Base64 body part
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("application", msg.getParts().get(0).getContentType().getTopLevelType());
      body = msg.getParts().get(0).GetBody();
      Assert.assertEquals(0, body.charAt(0));
      Assert.assertEquals(16, body.charAt(1));
      Assert.assertEquals(1, body.charAt(2));
      Assert.assertEquals(93, body.charAt(3));
      Assert.assertEquals(4, body.length());
      // Base64 body part II
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("application", msg.getParts().get(0).getContentType().getTopLevelType());
      body = msg.getParts().get(0).GetBody();
      Assert.assertEquals(0, body.charAt(0));
      Assert.assertEquals(16, body.charAt(1));
      Assert.assertEquals(1, body.charAt(2));
      Assert.assertEquals(93, body.charAt(3));
      Assert.assertEquals(4, body.length());
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
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      {
        String stringTemp = msg.getContentType().GetParameter("boundary");
        Assert.assertEquals(
          "b1",
          stringTemp);
      }
      Assert.assertEquals(1, msg.getParts().size());
      Message part = msg.getParts().get(0);
      Assert.assertEquals("application", part.getParts().get(0).getContentType().getTopLevelType());
      body = part.getParts().get(0).GetBody();
      Assert.assertEquals(0, body.charAt(0));
      Assert.assertEquals(16, body.charAt(1));
      Assert.assertEquals(1, body.charAt(2));
      Assert.assertEquals(93, body.charAt(3));
      Assert.assertEquals(4, body.length());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getParts().get(0).getBodyString());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test\r\n--Not-b2--", msg.getParts().get(0).getParts().get(0).getBodyString());
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain;charset=utf-8\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getParts().get(0).getBodyString());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(2, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test\r\n--Not-b2--", msg.getParts().get(0).getParts().get(0).getBodyString());
      Assert.assertEquals("Test2", msg.getParts().get(0).getParts().get(1).getBodyString());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getParts().get(0).getBodyString());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getParts().get(0).getBodyString());
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
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getParts().get(0).getBodyString());
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
      try {
        msg.SetHeader(
         "authentication-results",
         "a.b.c; d=e f.a=@example.com f.b=x f.c=y; g=x (y) h.a=me@example.com");
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
        String stringTemp = new MediaTypeBuilder().getTopLevelType();
        Assert.assertEquals(
          "application",
          stringTemp);
      }
      {
        String stringTemp = new
                    MediaTypeBuilder(MediaType.TextPlainAscii).getTopLevelType();
        Assert.assertEquals(
        "text",
        stringTemp);
      }
      {
        String stringTemp = new
                    MediaTypeBuilder(MediaType.TextPlainAscii).getSubType();
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
      MediaTypeBuilder builder;
      try {
        Assert.assertEquals(null, new MediaTypeBuilder(null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      builder = new MediaTypeBuilder("text", "plain");
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
        new MediaTypeBuilder().SetSubType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter("v");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x;y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x/y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x", "");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x;y", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x/y", "v");
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
      msg =
        MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
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
      msg =
MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "reply-to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-To: x1@example.com\r\nResent-To: x2@example.com\r\n\r\n";
      msg =
MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "resent-to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-Cc: x1@example.com\r\nResent-Cc: x2@example.com\r\n\r\n";
      msg =
MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "resent-cc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-Bcc: x1@example.com\r\nResent-Bcc: x2@example.com\r\n\r\n";
      msg =
MessageFromString(MessageFromString(msg).Generate())
          .GetHeader("resent-bcc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      // Invalid header fields
      msg = "From: x1@example.com\r\nFrom: x2.example.com\r\n\r\n";
      msg =
        MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.assertEquals("x1@example.com", msg);
      msg = "From: x1.example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg =
        MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.assertEquals("x2@example.com", msg);
    }

    @Test
    public void TestFWSAtSubjectEnd() {
      Message msg;
   String ValueStringVar = "From: me@example.com\r\nSubject: Test\r\n " +
              "\r\nX-Header: Header\r\n\r\nBody";
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
    String ValueStringVar = "From: me@example.com\r\nTo: empty-group:;" +
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
      msg.SetHeader("mime-version", "1.0");
      msg.getParts().get(0).SetHeader("mime-version", "1.0");
      {
        String stringTemp = msg.GetHeader("mime-version");
        Assert.assertEquals(
          "test",
          stringTemp);
      }
      {
        String stringTemp = msg.getParts().get(0).GetHeader("mime-version");
        Assert.assertEquals(
          "test",
          stringTemp);
      }
      msg = MessageFromString(msg.Generate());
      {
        String stringTemp = msg.GetHeader("mime-version");
        Assert.assertEquals(
          "1.0",
          stringTemp);
      }
      Assert.assertEquals(null, msg.getParts().get(0).GetHeader("mime-version"));
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
    /*
@Test
    public void TestAddAttachment() {
      Message msg = new Message();
      msg.SetHeader("from", "me@example.com");
      msg.SetHeader("subject", "Test message");
      msg.SetTextBody("This is a test body");
      using (System.IO.FileStream fs = new System.IO.FileStream(null,
                    System.IO.FileMode.Open)) {
        msg.AddAttachment(fs, "test.dat");
      }
      File.WriteAllBytes("message.eml",msg.GenerateBytes());
      System.out.println(msg.Generate());
    } */
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
        msg.getBodyString().toString();
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
    @Test
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
   String valueMessageString = "From: x@example.com\r\nMIME-Version: 1.0\r\n" +
            "Content-Type: text/plain\r\nContent-Disposition: " +
           new DispositionBuilder("inline").SetParameter("filename", input)
           .toString() + "\r\n\r\nEmpty.";
      msg = MessageFromString(valueMessageString);
      Assert.assertEquals(valueMessageString, expected, msg.getFileName());
      valueMessageString = "From: x@example.com\r\nMIME-Version: 1.0\r\n" +
      "Content-Type: " + new MediaTypeBuilder("text", "plain")
      .SetParameter("name", input).toString() +
        "\r\n\r\nEmpty.";
      msg = MessageFromString(valueMessageString);
      Assert.assertEquals(expected, msg.getFileName());
    }
    @Test
    public void TestFileName() {
      String[] fn = ContentDispositionTest.FileNames;
      for (int i = 0; i < fn.length(); i += 2) {
        TestFileNameOne(fn.charAt(i), fn.charAt(i + 1));
      }
    }
    @Test
    public void TestFromAddresses() {
      String valueMessageString =
             "From: me@example.com\r\nSubject: Subject\r\n\r\nBody";
      Message msg = MessageFromString(valueMessageString);
      MessageFromString(MessageGenerate(msg));
      List<NamedAddress> fromaddrs = msg.GetAddresses("from");
      Assert.assertEquals(1, fromaddrs.size());
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
    msgString = "From : me@example.com\r\nX-From: me2@example.com\r\n\r\nBody";
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
        new Message().SetHeader("from", "\"a\r\nb\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\rb\" <x@example.com>");
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
                    .SetHeader("comments", "subject")
                    .SetHeader("subject", "my subject")
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
      String[] headerNames = new String[] { "from", "to", "cc",
              "bcc", "reply-to" };
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
      Assert.assertEquals(value, msg.GetHeader(header));
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
      TestSetHeaderOne(msg, "accept-language", "en-us\u002c de-de");
      TestSetHeaderInvalid(msg, "accept-language", "enenenene-us\u002c de-de");
      TestSetHeaderInvalid(msg, "accept-language", "en-ususususu\u002c de-de");
      TestSetHeaderInvalid(msg, "accept-language", "en-us\u002c\u002c de-de");
      TestSetHeaderOne(msg, "accept-language", "en-us\u002cde-de");
      TestSetHeaderOne(msg, "accept-language", "en-us \u002cde-de");
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
      TestSetHeaderOne(msg, "to", "\"Example, Example\" <example@example.com>");
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
      Assert.assertEquals("Test", msg.getSubject());
      msg.setSubject("Test2");
      Assert.assertEquals("Test2", msg.getSubject());
    }
    @Test
    public void TestToAddresses() {
      // not implemented yet
    }
  }
