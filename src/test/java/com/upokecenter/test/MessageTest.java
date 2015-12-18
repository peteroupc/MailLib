package com.upokecenter.test; import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

import java.util.*;
import java.io.*;

  public class MessageTest {
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
      if ((msg) == null) {
        Assert.fail();
      }
      String ret = msg.Generate();
Assert.assertEquals(ret,2,EncodingTest.IsGoodAsciiMessageFormat(ret, false, ""));
      return ret;
    }

    @Test
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
        String stringTemp = MediaType.Parse("x/y;z=1;z*=utf-8''2"
).GetParameter("z");
        Assert.assertEquals(
        "2",
        stringTemp);
      }
    }

    static Message MessageFromString(String str) {
      Message msgobj = new Message(DataUtilities.GetUtf8Bytes(str,
                    true));
      MessageGenerate(msgobj);
      return msgobj;
    }

    static void MessageConstructOnly(String str) {
      new Message(DataUtilities.GetUtf8Bytes(str,
                    true));
    }

    private static void TestMediaTypeRoundTrip(String str) {
      String mtstring = new MediaTypeBuilder("x", "y").SetParameter("z",
                    str).toString();
      if (mtstring.contains("\r\n\r\n"))Assert.fail();
      if (mtstring.contains("\r\n \r\n"))Assert.fail();
      Assert.assertEquals(str, MediaType.Parse(mtstring).GetParameter("z"));
      var mtmessage = MessageFromString("MIME-Version: 1.0\r\nContent-Type: " +
                    mtstring + "\r\n\r\n");
      MessageGenerate(mtmessage);
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
        if (EncodingTest.IsGoodAsciiMessageFormat(msgtext, false, "TestGenerate"
) != 2) {
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
    public void TestContentTypeDefaults() {
      String start = "From: me@example.com\r\nMIME-Version: 1.0\r\n";
      String msg;
      msg = start + "\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MessageFromString(msg).getContentType());
      msg = start + "Content-Type: text/html\r\n\r\n";
      Assert.assertEquals(MediaType.Parse("text/html"),
                    MessageFromString(msg).getContentType());
      msg = start + "Content-Type: text/\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MessageFromString(msg).getContentType());
      msg = start + "Content-Type: /html\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MessageFromString(msg).getContentType());
      // All header fields are syntactically valid
      msg = start +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n"
        ;
      Assert.assertEquals(MediaType.ApplicationOctetStream,
                    MessageFromString(msg).getContentType());
      msg = start +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: text/html\r\n\r\n"
        ;
      Assert.assertEquals(MediaType.ApplicationOctetStream,
                    MessageFromString(msg).getContentType());
      // First header field is syntactically invalid
      msg = start +
  "Content-Type: /plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MessageFromString(msg).getContentType());
      // Second header field is syntactically invalid
      msg = start +
  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MessageFromString(msg).getContentType());
      msg = start +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\nContent-Type: text/html\r\n\r\n"
        ;
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MessageFromString(msg).getContentType());
      // Third header field is syntactically invalid
      msg = start +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: audio\r\n\r\n"
        ;
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MessageFromString(msg).getContentType());
    }

    @Test
    public void TestNewMessage() {
      if (!(new Message().getContentType() != null))Assert.fail();
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
          System.out.print("");
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
      }
    }

    public static void TestRfc2231Extension(String mtype, String param,
                    String expected) {
      MediaType mt = MediaType.Parse(mtype);
      Assert.assertEquals(expected, mt.GetParameter(param));
      String str = "From: me@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: " + mtype + "\r\n\r\nTest";
      Message msg = MessageFromString(str);
      mt = msg.getContentType();
      Assert.assertEquals(expected, mt.GetParameter(param));
    }

    @Test
    public void TestRfc2231Extensions() {
      TestRfc2231Extension("text/plain; charset=\"utf-8\"", "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*=us-ascii'en'utf-8",
                    "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*=us-ascii''utf-8",
                    "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*='en'utf-8", "charset",
                    "utf-8");
      TestRfc2231Extension("text/plain; charset*='i-unknown'utf-8", "charset",
                    "us-ascii");
      TestRfc2231Extension("text/plain; charset*=us-ascii'i-unknown'utf-8",
                    "charset",
                    "us-ascii");
      TestRfc2231Extension("text/plain; charset*=''utf-8", "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*0=a;charset*1=b", "charset",
                    "ab");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%20b", "charset",
                    "a b");
      TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b",
                    "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b",
                    "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b",
                    "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b",
                    "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*0=\"a\";charset*1=b",
                    "charset", "ab");

  TestRfc2231Extension("text/plain; charset*0*=utf-8''a%20b;charset*1*=c%20d"
                ,
                    "charset", "a bc d");
      TestRfc2231Extension(
        "text/plain; charset*0=ab;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "abiso-8859-1-en-xyz");
      TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "a biso-8859-1-en-xyz");

      if
     ((MediaType.Parse("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz"
                    ,
                    null)) != null) {
        Assert.fail();
      }

      if

  ((MediaType.Parse("text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz"
                    ,
                    null)) != null) {
        Assert.fail();
      }
      TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1=a%20b",
        "charset",
        "a ba%20b");
      TestRfc2231Extension(
         "text/plain; Charset*0*=utf-8''a%20b;cHarset*1=a%20b",
         "charset",
         "a ba%20b");
      TestRfc2231Extension(
        "text/plain\r\n (; charset=x;y=\");ChaRseT*=''a%41b-c(\")",
        "charset",
        "aAb-c");
      TestRfc2231Extension(
        "text/plain;\r\n chARSet (xx=y) = (\"z;) abc (d;e\") ; format = flowed",
        "charset",
        "abc");
      TestRfc2231Extension(
        "text/plain;\r\n charsET (xx=y) = (\"z;) abc (d;e\") ; format = flowed",
        "format",
        "flowed");
    }

    public static void SingleTestMediaTypeEncoding(String value) {
      MediaType mt = new MediaTypeBuilder("x", "y").SetParameter("z",
                    value).ToMediaType();
      String topLevel = mt.getTopLevelType();
      String sub = mt.getSubType();
      String mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      Message msg = MessageFromString(mtstring);
      Assert.assertEquals(topLevel, msg.getContentType().getTopLevelType());
      Assert.assertEquals(sub, msg.getContentType().getSubType());
      Assert.assertEquals(mt.toString(),value,msg.getContentType().GetParameter("z"));
    }

    @Test
    public void TestNamedAddress() {
      Assert.assertEquals("\"Me \" <me@example.com>", new NamedAddress("Me ",
                    "me@example.com").toString());
      Assert.assertEquals("\" Me\" <me@example.com>", new NamedAddress(" Me",
                    "me@example.com").toString());

      try {
        Assert.assertEquals(null, new Address(""));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("a b@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("a b@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("ab.example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("ab@exa mple.example"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("ab@example.com addr"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("Me\u00e0 <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("\"Me\" <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("\"Me\u00e0\" <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        String st = "Me <me@example.com>, Fred <fred@example.com>";
        Assert.assertEquals(null, new NamedAddress(st));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      if (new NamedAddress("x@example.com").isGroup())Assert.fail();
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
      Assert.assertEquals("\"(lo cal)\"@example.com",
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
          "From: Me <me@example.com>\r\n\r\n").AddHeader("x-comment", "comment"
).GetHeader("x-comment");
        Assert.assertEquals(
          "comment",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
        "From: Me <me@example.com>\r\n\r\n").AddHeader(new AbstractMap.SimpleImmutableEntry<String,
                String>("x-comment", "comment")).GetHeader("x-comment");
        Assert.assertEquals(
          "comment",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(0,
                    "you@example.com").GetHeader(0).getKey();
        Assert.assertEquals(
          "from",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(0,
                    "you@example.com").GetHeader(0).getValue();
        Assert.assertEquals(
          "you@example.com",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(0,
                    "x-comment", "comment").GetHeader(0).getKey();
        Assert.assertEquals(
          "x-comment",
          stringTemp);
      }
      {
        String stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(0,
                    "x-comment", "comment").GetHeader(0).getValue();
        Assert.assertEquals(
          "comment",
          stringTemp);
      }
      Message msg = MessageFromString("From: Me <me@example.com>\r\n\r\n");
      try {
        msg.SetHeader(0, (String)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(0, null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.AddHeader(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(-1, "me@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(-1, "To", "me@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.GetHeader(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.RemoveHeader(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestMessageTests() {
      String multipart =
        "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n"
        ;
      String msg;
      msg = multipart +

  "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\nContent-Description: description\r\n\r\n\r\n--b--"
        ;
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart +
        "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart +
        "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg =

  "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--"
        ;
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg =

  "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: 7bit\r\n\r\nA"
        ;
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--"
        ;
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: 7bit\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--"
        ;
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
      String multipart =
        "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n"
        ;
      String msg;
      // Multipart message with base64
      msg = multipart +
        "Content-Transfer-Encoding: base64\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message top-level-type with base64
      msg =

  "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: base64\r\n--b\r\n\r\n\r\n--b--"
        ;
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level multipart message
      msg = multipart +
          "\r\n--b\r\nContent-Type: text/plain\r\n\r\nHello World";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level multipart message
      msg = multipart +
        "\r\n--b\r\nContent-Type: text/html\r\n\r\n<b>Hello World</b>";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level multipart message
      msg = multipart + "\r\n--b\r\nContent-Type: text/html\r\n";
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
      msg = multipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\nX\r\n--b--"
        ;
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message/rfc822 with content-transfer-encoding base64;
      // which is not allowed for this media type
      msg = multipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: base64\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nXX==\r\n--b--"
        ;
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
      msg = multipart +

  "\r\n--b\r\nContent-Type: text/rfc822-headers\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\n--b--"
        ;
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
  "Mime-Version: 1.0\r\nContent-Type: text/html; charset=\"\"\r\n\r\n\ufffd"
        ;
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

  "Mime-Version: 1.0\r\nContent-Type: text/html\r\nContent-Transfer-Encoding: 7bit\r\n\r\n\ufffd"
        ;
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

  "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: base64\r\n\r\nA"
        ;
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
           ((message.getContentType().getSubType().equals("global-delivery-status")) ||
            (message.getContentType().getSubType().equals(
              "global-disposition-notification"))));
      }
      for (Message part : message.getParts()) {
        if (HasNestedMessageType(part)) {
          return true;
        }
      }
      return false;
    }

    @Test
    public void TestBoundaryReading() {
      byte[] body;
      String messageStart = "MIME-Version: 1.0\r\n";
      messageStart += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart += "Preamble\r\n";
      messageStart += "--b1\r\n";
      String message = messageStart;
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
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
      Assert.assertEquals(0, body[0]);
      Assert.assertEquals(16, body[1]);
      Assert.assertEquals(1, body[2]);
      Assert.assertEquals(93, body[3]);
      Assert.assertEquals(4, body.length);
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
      Assert.assertEquals(0, body[0]);
      Assert.assertEquals(16, body[1]);
      Assert.assertEquals(1, body[2]);
      Assert.assertEquals(93, body[3]);
      Assert.assertEquals(4, body.length);
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
      Assert.assertEquals(0, body[0]);
      Assert.assertEquals(16, body[1]);
      Assert.assertEquals(1, body[2]);
      Assert.assertEquals(93, body[3]);
      Assert.assertEquals(4, body.length);
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
    public void TestArgumentValidationMediaType() {
      try {
        MediaType.TextPlainAscii.GetParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MediaType.TextPlainAscii.GetParameter("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MediaType.Parse(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
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
      Assert.assertEquals(MediaType.TextPlainAscii,
                    MediaType.Parse("text/plain; charset=us-ascii"));
      if (!(MediaType.TextPlainAscii.hashCode() ==
                MediaType.Parse("text/plain; charset=us-ascii"
).hashCode()))Assert.fail();
    }
    @Test
    public void TestMediaTypeBuilder() {
      MediaTypeBuilder builder;
      try {
        Assert.assertEquals(null, new MediaTypeBuilder(null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      builder = new MediaTypeBuilder("text", "plain");
      try {
        builder.SetTopLevelType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter(null, "v");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter("", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter("v", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("e=");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("e/e");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x;y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x/y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x/y", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
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
      String str = "From: me@example.com\r\nSubject: Test\r\n " +
        "\r\nX-Header: Header\r\n\r\nBody";
      msg = MessageFromString(str);
      {
        String stringTemp = msg.GetHeader("subject");
        Assert.assertEquals(
        "Test ",
        stringTemp);
      }
    }

    @Test
    public void TestEmptyGroup() {
      Message msg;
      String str = "From: me@example.com\r\nTo: empty-group:;" +
        "\r\nCc: empty-group:;" + "\r\nBcc: empty-group:;" +
        "\r\n\r\nBody";
      msg = MessageFromString(str);
    }

    @Test
    public void TestMediaTypeArgumentValidationExtra() {
      if (!(MediaType.Parse("text/plain").isText()))Assert.fail();
      if (!(MediaType.Parse("multipart/alternative").isMultipart()))Assert.fail();
      {
        String stringTemp = MediaType.Parse(
                "example/x ").getTypeAndSubType();
        Assert.assertEquals(
        "example/x",
        stringTemp);
      }
      String strtest = "example/x" + "," + " a=b";
      {
        String stringTemp = MediaType.Parse(strtest).getTypeAndSubType();
        Assert.assertEquals(
          "text/plain",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("example/x ; a=b"
).getTypeAndSubType();
        Assert.assertEquals(
        "example/x",
        stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("example/x; a=b"
).getTypeAndSubType();
        Assert.assertEquals(
        "example/x",
        stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("example/x; a=b "
).getTypeAndSubType();
        Assert.assertEquals(
        "example/x",
        stringTemp);
      }
    }
    @Test
    public void TestContentHeadersOnlyInBodyParts() {
      Message msg = new Message().SetTextAndHtml("Hello", "Hello");
      msg.SetHeader("x-test", "test");
      msg.getParts().get(0).SetHeader("x-test", "test");
      {
        String stringTemp = msg.GetHeader("x-test");
        Assert.assertEquals(
          "test",
          stringTemp);
      }
      {
        String stringTemp = msg.getParts().get(0).GetHeader("x-test");
        Assert.assertEquals(
          "test",
          stringTemp);
      }
      msg = MessageFromString(msg.Generate());
      {
        String stringTemp = msg.GetHeader("x-test");
        Assert.assertEquals(
          "test",
          stringTemp);
      }
      Assert.assertEquals(null, msg.getParts().get(0).GetHeader("x-test"));
    }

    @Test
    public void TestConstructor() {
      try {
        Assert.assertEquals(null, new Message((InputStream)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Message((byte[])null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
  MessageConstructOnly("From: x@example.com\r\nSub ject: Test\r\n\r\nBody"
);
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
    MessageConstructOnly("From: x@example.com\r\nX-" +
          EncodingTest.Repeat("a",
2000) + ": Test\r\n\r\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
    MessageConstructOnly("From: x@example.com\r\nX-" +
          EncodingTest.Repeat("a",
996) + ":\r\n Test\r\n\r\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\n: Test\r\n\r\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nSubject: Test\r\n\rBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nSubject: Test\r\n\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\nSubject: Test\n\nBody");
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MessageFromString("From: x@example.com\r\nX-" + EncodingTest.Repeat("a",
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
      Message msg = new Message()
       .SetTextBody("Test");
      msg.setContentType(MediaType.Parse("text/plain);charset=x-unknown");
      try {
        msg.getBodyString().toString();
        Assert.fail("Should have failed");
      } catch (UnsupportedOperationException ex) {
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    private static void TestFileNameOne(String input, String expected) {
      Message msg;
      String str = "From: x@example.com\r\nMIME-Version: 1.0\r\n" +
 "Content-Type: text/plain\r\nContent-Disposition: inline; filename=" +
          input + "\r\n\r\nEmpty.";
      msg = MessageFromString(str);
      Assert.assertEquals(expected, msg.getFileName());
      str = "From: x@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: text/plain; name=" + input +
        "\r\n\r\nEmpty.";
      msg = MessageFromString(str);
      Assert.assertEquals(expected, msg.getFileName());
    }
    @Test
    public void TestFileName() {
      TestFileNameOne("com.txt", "com.txt");
      TestFileNameOne("com0.txt", "_com0.txt");
      TestFileNameOne("-hello.txt", "_-hello.txt");
      TestFileNameOne("lpt0.txt", "_lpt0.txt");
      TestFileNameOne("\"hello.txt\"", "hello.txt");
      TestFileNameOne("\"=?utf-8?q?hello=2Etxt?=\"", "hello.txt");
      TestFileNameOne("\"utf-8''hello%2Etxt\"", "hello.txt");
    }
    @Test
    public void TestFromAddresses() {
      String str = "From: me@example.com\r\nSubject: Subject\r\n\r\nBody";
      Message msg = MessageFromString(str);
      Message genmsg = MessageFromString(MessageGenerate(msg));
      Assert.assertEquals(1, msg.getFromAddresses().size());
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
      String str = "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
      Message msg = MessageFromString(str);
      try {
        msg.GetHeader(2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().GetHeader(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestHeaderFields() {
      // not implemented yet
    }
    @Test
    public void TestParts() {
      // not implemented yet
    }
    @Test
    public void TestRemoveHeader() {
      String str = "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
      Message msg = MessageFromString(str);
      try {
        msg.RemoveHeader(2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
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
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\rb\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r b\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r\n b\" <x@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
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
        System.out.print("");
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
        new Message().SetHeader("from", "\"a\nb\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\0b\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      String str = "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
      Message msg = MessageFromString(str);
      try {
        msg.SetHeader(2, "X-Header2", "2");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(2, "2");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(1, (String)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e:d", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e d", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e\u007f", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e\u00a0", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("e\u0008", "x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        System.out.print("");
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
    }
    @Test
    public void TestSetHtmlBody() {
      Message msg = new Message();
      try {
        msg.SetHtmlBody(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetTextAndHtml("test", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        System.out.print("");
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
        System.out.print("");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
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
