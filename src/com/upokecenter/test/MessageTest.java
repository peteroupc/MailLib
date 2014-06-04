package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

  class MessageTest {
    @Test
    public void TestMediaTypeEncodingSingle() {
      this.SingleTestMediaTypeEncoding("xyz", "x/y;z=xyz");
      this.SingleTestMediaTypeEncoding("xy z", "x/y;z=\"xy z\"");
      this.SingleTestMediaTypeEncoding("xy\u00a0z", "x/y;z*=utf-8''xy%C2%A0z");
      this.SingleTestMediaTypeEncoding("xy\ufffdz", "x/y;z*=utf-8''xy%C2z");
      this.SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\ufffc", 50) + "z", "x/y;z*=utf-8''xy" + EncodingTest.Repeat("%EF%BF%BD", 50) + "z");
      this.SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\u00a0", 50) + "z", "x/y;z*=utf-8''xy" + EncodingTest.Repeat("%C2%A0", 50) + "z");
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

    private static void TestMediaTypeRoundTrip(String str) {
      MediaTypeBuilder mtstring=new MediaTypeBuilder("x", "y").SetParameter("z", str).toString();
      if(mtstring.Contains("\r\n\r\n"))Assert.fail();
      if(mtstring.Contains("\r\n \r\n"))Assert.fail();
      Assert.assertEquals(str, MediaType.Parse(mtstring).GetParameter("z"));
      Message mtmessage=new Message(new java.io.ByteArrayInputStream(
        DataUtilities.GetUtf8Bytes("MIME-Version: 1.0\r\nContent-Type: " + mtstring + "\r\n\r\n", true)));
      if(!(EncodingTest.IsGoodAsciiMessageFormat(mtmessage.Generate(), false)))Assert.fail();
    }

    @Test
    public void TestGenerate() {
      ArrayList<String> msgids = new ArrayList<String>();
      // Tests whether unique Message IDs are generated for each message.
      for (var i = 0; i < 1000; ++i) {
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
    public void TestNewMessage() {
      if(!(new Message().getContentType() != null))Assert.fail();
    }

    @Test
    public void TestPrematureEnd() {
      try {
        new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("From: me@example.com\r\nDate", true)));
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("From: me@example.com\r\nDate\r", true)));
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("Received: from x", true)));
        Assert.fail("Should have failed");
      } catch (MessageDataException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Message(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes("Received: from x\r", true)));
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
    }

    public void TestRfc2231Extension(String mtype, String param, String expected) {
      var mt = MediaType.Parse(mtype);
      Assert.assertEquals(expected, mt.GetParameter(param));
    }

    @Test
    public void TestRfc2231Extensions() {
      this.TestRfc2231Extension("text/plain; charset=\"utf-8\"", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*=us-ascii'en'utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*=us-ascii''utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*='en'utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*=''utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*0=a;charset*1=b", "charset", "ab");
      this.TestRfc2231Extension("text/plain; charset*=utf-8''a%20b", "charset", "a b");
      this.TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*0=\"a\";charset*1=b", "charset", "ab");
      this.TestRfc2231Extension("text/plain; charset*0*=utf-8''a%20b;charset*1*=c%20d", "charset", "a bc d");
      this.TestRfc2231Extension(
        "text/plain; charset*0=ab;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "abiso-8859-1-en-xyz");
      this.TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "a biso-8859-1-en-xyz");
      if((MediaType.Parse("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz", null))!=null)Assert.fail();
      if((MediaType.Parse("text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz", null))!=null)Assert.fail();
      this.TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1=a%20b",
        "charset",
        "a ba%20b");
      this.TestRfc2231Extension(
        "text/plain\r\n (; charset=x;y=\");ChaRseT*=''a%41b-c(\")",
        "charset",
        "aAb-c");
      this.TestRfc2231Extension(
        "text/plain;\r\n chARSet (xx=y) = (\"z;) abc (d;e\") ; format = flowed",
        "charset",
        "abc");
      this.TestRfc2231Extension(
        "text/plain;\r\n charsET (xx=y) = (\"z;) abc (d;e\") ; format = flowed",
        "format",
        "flowed");
    }

    public void SingleTestMediaTypeEncoding(String value, String expected) {
      MediaType mt = new MediaTypeBuilder("x", "y").SetParameter("z", value).ToMediaType();
      String topLevel = mt.getTopLevelType();
      String sub = mt.getSubType();
      var mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt.toString() +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      java.io.ByteArrayInputStream ms=null;
try {
ms=new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(mtstring, true));

        Message msg=new Message(ms);
        Assert.assertEquals(topLevel, msg.getContentType().TopLevelType);
        Assert.assertEquals(sub, msg.getContentType().SubType);
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
        new NamedAddress("", (String)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("", (Address)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("x at example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("x");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("x@");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address((String)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("a b@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("a b@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("ab.example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("ab@exa mple.example");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("ab@example.com addr");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("Me <me@example.com>");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("Me\u00e0 <me@example.com>");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("\"Me\" <me@example.com>");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("\"Me\u00e0\" <me@example.com>");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("Me <me@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("Me\u00e0 <me@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("\"Me\" <me@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("\"Me\u00e0\" <me@example.com>");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new NamedAddress("Me <me@example.com>, Fred <fred@example.com>");
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
        new Address("local=domain.example");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("local@");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address(EncodingTest.Repeat("local", 200) + "@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("lo,cal@example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestMailbox() {
      var mbox = "Me <@example.org,@example.net,@example.com:me@x.example>";
      NamedAddress result=new NamedAddress(mbox);
      Assert.assertEquals("Me <me@x.example>", result.toString());
    }

    static boolean HasNestedMessageType(Message message) {
      if (message.getContentType().TopLevelType.equals("message")) {
        if (message.getContentType().SubType.equals("global")) {
          return false;
        }
        if (message.getContentType().SubType.equals("global-headers")) {
          return false;
        }
        if (message.getContentType().SubType.equals("global-delivery-status")) {
          return false;
        }
        if (message.getContentType().SubType.equals("global-disposition-notification")) {
          return false;
        }
        return true;
      }
      for(Object part : message.getParts()) {
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
      Assert.assertEquals("multipart", msg.getContentType().TopLevelType);
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("text", msg.getParts().get(0).getContentType().TopLevelType);
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
      Assert.assertEquals("multipart", msg.getContentType().TopLevelType);
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("application", msg.getParts().get(0).getContentType().TopLevelType);
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
      Assert.assertEquals("multipart", msg.getContentType().TopLevelType);
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
      Assert.assertEquals(1, msg.getParts().size());
      Assert.assertEquals("application", msg.getParts().get(0).getContentType().TopLevelType);
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
      Assert.assertEquals("multipart", msg.getContentType().TopLevelType);
      Assert.assertEquals("b1", msg.getContentType().GetParameter("boundary"));
      Assert.assertEquals(1, msg.getParts().size());
      Message part = msg.getParts().get(0);
      Assert.assertEquals("application", part.getParts().get(0).getContentType().TopLevelType);
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
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      Assert.assertEquals("application", new MediaTypeBuilder().getTopLevelType());
      Assert.assertEquals("text", new MediaTypeBuilder(MediaType.TextPlainAscii).getTopLevelType());
      Assert.assertEquals("plain", new MediaTypeBuilder(MediaType.TextPlainAscii).getSubType());
      if(!(MediaType.TextPlainAscii.equals(MediaType.Parse("text/plain; charset=us-ascii"))))Assert.fail();
      if(!(MediaType.TextPlainAscii.hashCode() == MediaType.Parse("text/plain; charset=us-ascii").hashCode()))Assert.fail();
    }
    @Test
    public void TestMediaTypeBuilder() {
      MediaTypeBuilder builder;
      try {
        new MediaTypeBuilder(null);
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