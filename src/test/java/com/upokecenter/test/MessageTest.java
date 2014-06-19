package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public class MessageTest {
    @Test
    public void TestMediaTypeEncodingSingle() {
      SingleTestMediaTypeEncoding("xyz", "x/y;z=xyz");
      SingleTestMediaTypeEncoding("xy z", "x/y;z=\"xy z\"");
      SingleTestMediaTypeEncoding("xy\u00a0z", "x/y;z*=utf-8''xy%C2%A0z");
      SingleTestMediaTypeEncoding("xy\ufffdz", "x/y;z*=utf-8''xy%C2z");
      SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\ufffc", 50) + "z", "x/y;z*=utf-8''xy" + EncodingTest.Repeat("%EF%BF%BD", 50) + "z");
      SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\u00a0", 50) + "z", "x/y;z*=utf-8''xy" + EncodingTest.Repeat("%C2%A0", 50) + "z");
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
      Assert.assertEquals("2", MediaType.Parse("x/y;z=1;z*=utf-8''2").GetParameter("z"));
    }

    private static Message MessageFromString(String str) {
      return new Message(new java.io.ByteArrayInputStream(
        DataUtilities.GetUtf8Bytes(str, true)));
    }

    private static void TestMediaTypeRoundTrip(String str) {
      String mtstring = new MediaTypeBuilder("x", "y").SetParameter("z", str).toString();
      if(mtstring.contains("\r\n\r\n"))Assert.fail();
      if(mtstring.contains("\r\n \r\n"))Assert.fail();
      Assert.assertEquals(str, MediaType.Parse(mtstring).GetParameter("z"));
      Message mtmessage = new Message(new java.io.ByteArrayInputStream(
        DataUtilities.GetUtf8Bytes("MIME-Version: 1.0\r\nContent-Type: " + mtstring + "\r\n\r\n", true)));
      if(!(EncodingTest.IsGoodAsciiMessageFormat(mtmessage.Generate(), false)))Assert.fail();
    }

    @Test
    public void TestGenerate() {
      ArrayList<String> msgids = new ArrayList<String>();
      // Tests whether unique Message IDs are generated for each message.
      for (int i = 0; i < 1000; ++i) {
        String msgtext = new Message().SetHeader("from", "me@example.com").SetTextBody("Hello world.").Generate();
        if (!EncodingTest.IsGoodAsciiMessageFormat(msgtext, false)) {
          Assert.fail("Bad message format generated");
        }
        String msgid = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(msgtext, true))).GetHeader("message-id");
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
      Assert.assertEquals(MediaType.TextPlainAscii, MessageFromString(msg).getContentType());
      msg = start + "Content-Type: text/html\r\n\r\n";
      Assert.assertEquals(MediaType.Parse("text/html"), MessageFromString(msg).getContentType());
      msg = start + "Content-Type: text/\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii, MessageFromString(msg).getContentType());
      msg = start + "Content-Type: /html\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii, MessageFromString(msg).getContentType());
      // All header fields are syntactically valid
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";
      Assert.assertEquals(MediaType.ApplicationOctetStream, MessageFromString(msg).getContentType());
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: text/html\r\n\r\n";
      Assert.assertEquals(MediaType.ApplicationOctetStream, MessageFromString(msg).getContentType());
      // First header field is syntactically invalid
      msg = start + "Content-Type: /plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii, MessageFromString(msg).getContentType());
      // Second header field is syntactically invalid
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii, MessageFromString(msg).getContentType());
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\nContent-Type: text/html\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii, MessageFromString(msg).getContentType());
      // Third header field is syntactically invalid
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: audio\r\n\r\n";
      Assert.assertEquals(MediaType.TextPlainAscii, MessageFromString(msg).getContentType());
    }

    @Test
    public void TestNewMessage() {
      if(!(new Message().getContentType() != null))Assert.fail();
    }

    @Test
    public void TestPrematureEnd() {
      try {
        Assert.assertEquals(null, new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("From: me@example.com\r\nDate", true))));
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("From: me@example.com\r\nDate\r", true))));
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("Received: from x", true))));
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("Received: from x\r", true))));
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestMakeFilename() {
      Assert.assertEquals(
        "hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?hello.txt?="));
      Assert.assertEquals(
        "hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?___hello.txt___?="));
      Assert.assertEquals(
        "hello.txt",
        ContentDisposition.MakeFilename("  =?utf-8?q?hello.txt?=  "));
      Assert.assertEquals(
        "hello.txt",
        ContentDisposition.MakeFilename("  =?utf-8?q?___hello.txt___?=  "));
      Assert.assertEquals(
        "a\u00e7\u00e3o.txt",
        ContentDisposition.MakeFilename("=?iso-8859-1?q?a=E7=E3o.txt?="));
      Assert.assertEquals(
        "a\u00e7\u00e3o.txt",
        ContentDisposition.MakeFilename("a\u00e7\u00e3o.txt"));
      Assert.assertEquals(
        "hello.txt",
        ContentDisposition.MakeFilename("=?x-unknown?q?hello.txt?="));
      Assert.assertEquals(
        "_",
        ContentDisposition.MakeFilename("=?x-unknown"));
      Assert.assertEquals(
        "my_file_name_.txt",
        ContentDisposition.MakeFilename("my?file<name>.txt"));
      Assert.assertEquals(
        "my file name_.txt",
        ContentDisposition.MakeFilename("my file\tname\".txt"));
      Assert.assertEquals(
        "my\ufffdfile\ufffdname\ud800\udc00.txt",
        ContentDisposition.MakeFilename("my\ud800file\udc00name\ud800\udc00.txt"));
      Assert.assertEquals(
        "file\ufffdname",
        ContentDisposition.MakeFilename("=?x-unknown?Q?file\ud800name?="));
      Assert.assertEquals(
        "file\u00bename.txt",
        ContentDisposition.MakeFilename("utf-8''file%c2%bename.txt"));
      Assert.assertEquals(
        "file\u00bename.txt",
        ContentDisposition.MakeFilename("utf-8'en'file%c2%bename.txt"));
      Assert.assertEquals(
        "x-unknown'en'file%c2%bename.txt",
        ContentDisposition.MakeFilename("x-unknown'en'file%c2%bename.txt"));
      Assert.assertEquals(
        "file\u00bename.txt",
        ContentDisposition.MakeFilename("utf-8'en-us'file%c2%bename.txt"));
      Assert.assertEquals(
        "_..._",
        ContentDisposition.MakeFilename("..."));
      Assert.assertEquals(
        "_~home",
        ContentDisposition.MakeFilename("~home"));
      Assert.assertEquals(
        "_~nul",
        ContentDisposition.MakeFilename("~nul"));
      Assert.assertEquals(
        "myfilename.txt._",
        ContentDisposition.MakeFilename("myfilename.txt."));
      Assert.assertEquals(
        "_nul",
        ContentDisposition.MakeFilename("nul"));
      Assert.assertEquals(
        "_nul",
        ContentDisposition.MakeFilename("   nul   "));
      Assert.assertEquals(
        "ordinary",
        ContentDisposition.MakeFilename("   ordinary   "));
      Assert.assertEquals(
        "_nul.txt",
        ContentDisposition.MakeFilename("nul.txt"));
      Assert.assertEquals(
        "_con",
        ContentDisposition.MakeFilename("con"));
      Assert.assertEquals(
        "_aux",
        ContentDisposition.MakeFilename("aux"));
      Assert.assertEquals(
        "_lpt1device",
        ContentDisposition.MakeFilename("lpt1device"));
      Assert.assertEquals(
        "my_file_name_.txt",
        ContentDisposition.MakeFilename("my\u0001file\u007fname*.txt"));
      Assert.assertEquals(
        "folder_hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?folder\\hello.txt?="));
      Assert.assertEquals(
        "folder_",
        ContentDisposition.MakeFilename("folder/"));
      Assert.assertEquals(
        "folder______",
        ContentDisposition.MakeFilename("folder//////"));
      Assert.assertEquals(
        "fol_der_",
        ContentDisposition.MakeFilename("fol/der/"));
      Assert.assertEquals(
        "fol_der______",
        ContentDisposition.MakeFilename("fol/der//////"));
      Assert.assertEquals(
        "folder_hello.txt",
        ContentDisposition.MakeFilename("folder/hello.txt"));
      Assert.assertEquals(
        "fol_der_hello.txt",
        ContentDisposition.MakeFilename("fol/der/hello.txt"));
      Assert.assertEquals(
        "folder_hello.txt",
        ContentDisposition.MakeFilename("=?x-unknown?q?folder\\hello.txt?="));
    }

    @Test
    public void TestCharset() {
      Assert.assertEquals("us-ascii", MediaType.Parse("text/plain").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("TEXT/PLAIN").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("TeXt/PlAiN").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("text/troff").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; CHARSET=UTF-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; ChArSeT=UTF-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset=UTF-8").GetCharset());
      // Note that MIME implicitly allows whitespace around the equal sign
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset = UTF-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset (cmt) = (cmt) UTF-8").GetCharset());
      Assert.assertEquals("'utf-8'", MediaType.Parse("text/plain; charset='UTF-8'").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; foo='; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset=\"UTF-\\8\"").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("nana").GetCharset());
      Assert.assertEquals("", MediaType.Parse("text/xxx").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/xxx;charset=UTF-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/xxx;charset=utf-8").GetCharset());
      Assert.assertEquals("", MediaType.Parse("text/xxx;chabset=utf-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/xml;charset=utf-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain;charset=utf-8").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("text/plain;chabset=utf-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("image/xml;charset=utf-8").GetCharset());
      Assert.assertEquals("", MediaType.Parse("image/xml;chabset=utf-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("image/plain;charset=utf-8").GetCharset());
      Assert.assertEquals("", MediaType.Parse("image/plain;chabset=utf-8").GetCharset());
    }

    @Test
    public void TestCodePointCompare() {
      if(!(DataUtilities.CodePointCompare("abc", "def") < 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ud800\udc00", "a\ud900\udc00") < 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ud800\udc00", "a\ud800\udc00") == 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ud800", "a\ud800") == 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\udc00", "a\udc00") == 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ud800\udc00", "a\ud800\udd00") < 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ud800\ufffd", "a\ud800\udc00") < 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ud800\ud7ff", "a\ud800\udc00") < 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ufffd\udc00", "a\ud800\udc00") < 0))Assert.fail();
      if(!(DataUtilities.CodePointCompare("a\ud7ff\udc00", "a\ud800\udc00") < 0))Assert.fail();
    }

    public static void TestRfc2231Extension(String mtype, String param, String expected) {
      MediaType mt = MediaType.Parse(mtype);
      Assert.assertEquals(expected, mt.GetParameter(param));
    }

    @Test
    public void TestRfc2231Extensions() {
      TestRfc2231Extension("text/plain; charset=\"utf-8\"", "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*=us-ascii'en'utf-8", "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*=us-ascii''utf-8", "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*='en'utf-8", "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*=''utf-8", "charset", "utf-8");
      TestRfc2231Extension("text/plain; charset*0=a;charset*1=b", "charset", "ab");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%20b", "charset", "a b");
      TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b", "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b", "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b", "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b", "charset", "a\u00a0b");
      TestRfc2231Extension("text/plain; charset*0=\"a\";charset*1=b", "charset", "ab");
      TestRfc2231Extension("text/plain; charset*0*=utf-8''a%20b;charset*1*=c%20d", "charset", "a bc d");
      TestRfc2231Extension(
        "text/plain; charset*0=ab;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "abiso-8859-1-en-xyz");
      TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "a biso-8859-1-en-xyz");
      if((MediaType.Parse("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz", null))!=null)Assert.fail();
      if((MediaType.Parse("text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz", null))!=null)Assert.fail();
      TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1=a%20b",
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

    public static void SingleTestMediaTypeEncoding(String value, String expected) {
      MediaType mt = new MediaTypeBuilder("x", "y").SetParameter("z", value).ToMediaType();
      String topLevel = mt.getTopLevelType();
      String sub = mt.getSubType();
      String mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      java.io.ByteArrayInputStream ms=null;
try {
ms=new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(mtstring, true));

        Message msg = new Message(ms);
        Assert.assertEquals(topLevel, msg.getContentType().getTopLevelType());
        Assert.assertEquals(sub, msg.getContentType().getSubType());
        Assert.assertEquals(mt.toString(),expected,mt.toString());
        Assert.assertEquals(mt.toString(),value,msg.getContentType().GetParameter("z"));
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    @Test
    public void TestSetHeader() {
      try {
        new Message().SetHeader("from", "\"a\r\nb\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\rb\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r b\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r\n b\" <x@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
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
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetHeader("from", "\"a\0b\" <x@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestNamedAddress() {
      Assert.assertEquals("\"Me \" <me@example.com>", new NamedAddress("Me ", "me@example.com").toString());
      Assert.assertEquals("\" Me\" <me@example.com>", new NamedAddress(" Me", "me@example.com").toString());
      try {
        Assert.assertEquals(null, new NamedAddress("", (String)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("", (Address)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("x at example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("x"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("x@"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address((String)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress(""));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address(""));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("a b@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("a b@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("ab.example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("ab@exa mple.example"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("ab@example.com addr"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("Me <me@example.com>"));
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("Me\u00e0 <me@example.com>"));
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("\"Me\" <me@example.com>"));
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("\"Me\u00e0\" <me@example.com>"));
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("Me <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("Me\u00e0 <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("\"Me\" <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("\"Me\u00e0\" <me@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("Me <me@example.com>, Fred <fred@example.com>"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      if(new NamedAddress("x@example.com").isGroup())Assert.fail();
      Assert.assertEquals("x@example.com", new NamedAddress("x@example.com").getName());
      Assert.assertEquals("x@example.com", new NamedAddress("x@example.com").getAddress().toString());
      Assert.assertEquals(
        "\"(lo cal)\"@example.com",
        new Address("\"(lo cal)\"@example.com").toString());
      Assert.assertEquals(
        "local",
        new Address("local@example.com").getLocalPart());
      Assert.assertEquals(
        "example.com",
        new Address("local@example.com").getDomain());
      try {
        Assert.assertEquals(null, new Address("local=domain.example"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("local@"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address(EncodingTest.Repeat("local", 200) + "@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("lo,cal@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestHeaderManip() {
      Assert.assertEquals("comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").AddHeader("x-comment", "comment").GetHeader("x-comment"));
      Assert.assertEquals("comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").AddHeader(new AbstractMap.SimpleImmutableEntry<String, String>("x-comment", "comment")).GetHeader("x-comment"));
      Assert.assertEquals("from", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "you@example.com").GetHeader(0).getKey());
      Assert.assertEquals("you@example.com", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "you@example.com").GetHeader(0).getValue());
      Assert.assertEquals("x-comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "x-comment", "comment").GetHeader(0).getKey());
      Assert.assertEquals("comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "x-comment", "comment").GetHeader(0).getValue());
      Message msg = MessageFromString("From: Me <me@example.com>\r\n\r\n");
      try {
        msg.SetHeader(0, (String)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(0, null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.AddHeader(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(-1, "me@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.SetHeader(-1, "To", "me@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.GetHeader(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        msg.RemoveHeader(-1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestMessageTests() {
      String multipart = "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";
      String msg;
      msg = multipart + "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\nContent-Description: description\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart + "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart + "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: 7bit\r\n\r\nA";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: 7bit\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";
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
      String multipart = "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";
      String msg;
      // Multipart message with base64
      msg = multipart + "Content-Transfer-Encoding: base64\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message top-level-type with base64
      msg = "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: base64\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level multipart message
      msg = multipart + "\r\n--b\r\nContent-Type: text/plain\r\n\r\nHello World";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Truncated top-level multipart message
      msg = multipart + "\r\n--b\r\nContent-Type: text/html\r\n\r\n<b>Hello World</b>";
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
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\nX\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Message/rfc822 with content-transfer-encoding base64;
      // which is not allowed for this media type
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: base64\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nXX==\r\n--b--";
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
      msg = multipart + "\r\n--b\r\nContent-Type: text/rfc822-headers\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\n--b--";
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
      msg = "Mime-Version: 1.0\r\nContent-Type: text/html; charset=\"\"\r\n\r\n\ufffd";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      // Text/html with an explicit Content-Transfer-Encoding of 7bit;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = "Mime-Version: 1.0\r\nContent-Type: text/html\r\nContent-Transfer-Encoding: 7bit\r\n\r\n\ufffd";
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
      msg = "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: base64\r\n\r\nA";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestMailbox() {
      String mbox = "Me <@example.org,@example.net,@example.com:me@x.example>";
      NamedAddress result = new NamedAddress(mbox);
      Assert.assertEquals("Me <me@x.example>", result.toString());
    }

    static boolean HasNestedMessageType(Message message) {
      if (message.getContentType().getTopLevelType().equals("message")) {
        return (!message.getContentType().getSubType().equals("global")) && ((!message.getContentType().getSubType().equals("global-headers")) && ((message.getContentType().getSubType().equals("global-delivery-status")) ||                                                                                                                               (message.getContentType().getSubType().equals("global-disposition-notification"))));
      }
      for(Message part : message.getParts()) {
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
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
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
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals(1, msg.getParts().get(0).getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getParts().get(0).getBodyString());
      // No headers in body part
      message = messageStart;
      message += "\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
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
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("Test", msg.getParts().get(0).getBodyString());
      // Base64 body part
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
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
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
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
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.assertEquals("multipart", msg.getContentType().getTopLevelType());
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
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
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(message, true)));
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
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MediaType.TextPlainAscii.GetParameter("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        MediaType.Parse(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      Assert.assertEquals("application", new MediaTypeBuilder().getTopLevelType());
      Assert.assertEquals("text", new MediaTypeBuilder(MediaType.TextPlainAscii).getTopLevelType());
      Assert.assertEquals("plain", new MediaTypeBuilder(MediaType.TextPlainAscii).getSubType());
      Assert.assertEquals(MediaType.TextPlainAscii, MediaType.Parse("text/plain; charset=us-ascii"));
      if(!(MediaType.TextPlainAscii.hashCode() == MediaType.Parse("text/plain; charset=us-ascii").hashCode()))Assert.fail();
    }
    @Test
    public void TestMediaTypeBuilder() {
      MediaTypeBuilder builder;
      try {
        Assert.assertEquals(null, new MediaTypeBuilder(null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      builder = new MediaTypeBuilder("text", "plain");
      try {
        builder.SetTopLevelType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter(null, "v");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter("", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetParameter("v", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("e=");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        builder.SetTopLevelType("e/e");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
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
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x;y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x/y");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
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
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x/y", "v");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestMessageMergeFields() {
      String msg;
      msg = "From: x1@example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "To: x1@example.com\r\nTo: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Cc: x1@example.com\r\nCc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("cc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Bcc: x1@example.com\r\nBcc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("bcc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Reply-To: x1@example.com\r\nReply-To: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("reply-to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-To: x1@example.com\r\nResent-To: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("resent-to");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-Cc: x1@example.com\r\nResent-Cc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("resent-cc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      msg = "Resent-Bcc: x1@example.com\r\nResent-Bcc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("resent-bcc");
      Assert.assertEquals(msg,"x1@example.com,x2@example.com");
      // Invalid header fields
      msg = "From: x1@example.com\r\nFrom: x2.example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.assertEquals("x1@example.com", msg);
      msg = "From: x1.example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.assertEquals("x2@example.com", msg);
    }

    @Test
    public void TestMessageArgumentValidation() {
      try {
        new Message().GetHeader(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message().SetBody(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetHeader2() {
      Assert.assertEquals("my subject", new Message().SetHeader("comments", "subject").SetHeader("subject", "my subject").GetHeader("subject"));
    }

    @Test
    public void TestMediaTypeArgumentValidationExtra() {
      if(!(MediaType.Parse("text/plain").isText()))Assert.fail();
      if(!(MediaType.Parse("multipart/alternative").isMultipart()))Assert.fail();
      Assert.assertEquals("example/x", MediaType.Parse("example/x ").getTypeAndSubType());
      Assert.assertEquals("text/plain", MediaType.Parse("example/x, a=b").getTypeAndSubType());
      Assert.assertEquals("example/x", MediaType.Parse("example/x ; a=b").getTypeAndSubType());
      Assert.assertEquals("example/x", MediaType.Parse("example/x; a=b").getTypeAndSubType());
      Assert.assertEquals("example/x", MediaType.Parse("example/x; a=b ").getTypeAndSubType());
    }
    @Test
    public void TestContentHeadersOnlyInBodyParts() {
      Message msg = new Message().SetTextAndHtml("Hello", "Hello");
      msg.SetHeader("x-test", "test");
      msg.getParts().get(0).SetHeader("x-test", "test");
      Assert.assertEquals("test", msg.GetHeader("x-test"));
      Assert.assertEquals("test", msg.getParts().get(0).GetHeader("x-test"));
      msg = new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(msg.Generate(), true)));
      Assert.assertEquals("test", msg.GetHeader("x-test"));
      Assert.assertEquals(null, msg.getParts().get(0).GetHeader("x-test"));
    }
  }
