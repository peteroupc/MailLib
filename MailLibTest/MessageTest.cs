using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;
using PeterO.Mail;
namespace MailLibTest {
  [TestClass]
  public class MessageTest {
    [TestMethod]
    public void TestMediaTypeEncodingSingle() {
      SingleTestMediaTypeEncoding("xyz");
      SingleTestMediaTypeEncoding("xy z");
      SingleTestMediaTypeEncoding("xy\u00a0z");
      SingleTestMediaTypeEncoding("xy\ufffdz");
      SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\ufffc", 50) + "z");
      SingleTestMediaTypeEncoding("xy" + EncodingTest.Repeat("\u00a0", 50) + "z");
    }

    [TestMethod]
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
      Assert.AreEqual("2", MediaType.Parse("x/y;z=1;z*=utf-8''2").GetParameter("z"));
    }

    private static Message MessageFromString(string str) {
      return new Message(new MemoryStream(
        DataUtilities.GetUtf8Bytes(str, true)));
    }

    private static void TestMediaTypeRoundTrip(string str) {
      string mtstring = new MediaTypeBuilder("x", "y").SetParameter("z", str).ToString();
      Assert.IsFalse(mtstring.Contains("\r\n\r\n"));
      Assert.IsFalse(mtstring.Contains("\r\n \r\n"));
      Assert.AreEqual(str, MediaType.Parse(mtstring).GetParameter("z"));
      var mtmessage = new Message(new MemoryStream(
        DataUtilities.GetUtf8Bytes("MIME-Version: 1.0\r\nContent-Type: " + mtstring + "\r\n\r\n", true)));
      Assert.IsTrue(EncodingTest.IsGoodAsciiMessageFormat(mtmessage.Generate(), false));
    }

    [TestMethod]
    public void TestGenerate() {
      var msgids = new List<string>();
      // Tests whether unique Message IDs are generated for each message.
      for (int i = 0; i < 1000; ++i) {
        string msgtext = new Message().SetHeader("from", "me@example.com").SetTextBody("Hello world.").Generate();
        if (!EncodingTest.IsGoodAsciiMessageFormat(msgtext, false)) {
          Assert.Fail("Bad message format generated");
        }
        string msgid = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(msgtext, true))).GetHeader("message-id");
        if (msgids.Contains(msgid)) {
          Assert.Fail(msgid);
        }
        msgids.Add(msgid);
      }
    }

    [TestMethod]
    public void TestContentTypeDefaults() {
      string start = "From: me@example.com\r\nMIME-Version: 1.0\r\n";
      string msg;
      msg = start + "\r\n\r\n";
      Assert.AreEqual(MediaType.TextPlainAscii, MessageFromString(msg).ContentType);
      msg = start + "Content-Type: text/html\r\n\r\n";
      Assert.AreEqual(MediaType.Parse("text/html"), MessageFromString(msg).ContentType);
      msg = start + "Content-Type: text/\r\n\r\n";
      Assert.AreEqual(MediaType.TextPlainAscii, MessageFromString(msg).ContentType);
      msg = start + "Content-Type: /html\r\n\r\n";
      Assert.AreEqual(MediaType.TextPlainAscii, MessageFromString(msg).ContentType);
      // All header fields are syntactically valid
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";
      Assert.AreEqual(MediaType.ApplicationOctetStream, MessageFromString(msg).ContentType);
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: text/html\r\n\r\n";
      Assert.AreEqual(MediaType.ApplicationOctetStream, MessageFromString(msg).ContentType);
      // First header field is syntactically invalid
      msg = start + "Content-Type: /plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";
      Assert.AreEqual(MediaType.TextPlainAscii, MessageFromString(msg).ContentType);
      // Second header field is syntactically invalid
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\n\r\n";
      Assert.AreEqual(MediaType.TextPlainAscii, MessageFromString(msg).ContentType);
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\nContent-Type: text/html\r\n\r\n";
      Assert.AreEqual(MediaType.TextPlainAscii, MessageFromString(msg).ContentType);
      // Third header field is syntactically invalid
      msg = start + "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: audio\r\n\r\n";
      Assert.AreEqual(MediaType.TextPlainAscii, MessageFromString(msg).ContentType);
    }

    [TestMethod]
    public void TestNewMessage() {
      Assert.IsTrue(new Message().ContentType != null);
    }

    [TestMethod]
    public void TestPrematureEnd() {
      try {
        Assert.AreEqual(null, new Message(new MemoryStream(DataUtilities.GetUtf8Bytes("From: me@example.com\r\nDate", true))));
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Message(new MemoryStream(DataUtilities.GetUtf8Bytes("From: me@example.com\r\nDate\r", true))));
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Message(new MemoryStream(DataUtilities.GetUtf8Bytes("Received: from x", true))));
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Message(new MemoryStream(DataUtilities.GetUtf8Bytes("Received: from x\r", true))));
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestMakeFilename() {
      Assert.AreEqual(
        "hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?hello.txt?="));
      Assert.AreEqual(
        "hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?___hello.txt___?="));
      Assert.AreEqual(
        "hello.txt",
        ContentDisposition.MakeFilename("  =?utf-8?q?hello.txt?=  "));
      Assert.AreEqual(
        "hello.txt",
        ContentDisposition.MakeFilename("  =?utf-8?q?___hello.txt___?=  "));
      Assert.AreEqual(
        "a\u00e7\u00e3o.txt",
        ContentDisposition.MakeFilename("=?iso-8859-1?q?a=E7=E3o.txt?="));
      Assert.AreEqual(
        "a\u00e7\u00e3o.txt",
        ContentDisposition.MakeFilename("a\u00e7\u00e3o.txt"));
      Assert.AreEqual(
        "hello.txt",
        ContentDisposition.MakeFilename("=?x-unknown?q?hello.txt?="));
      Assert.AreEqual(
        "_",
        ContentDisposition.MakeFilename("=?x-unknown"));
      Assert.AreEqual(
        "my_file_name_.txt",
        ContentDisposition.MakeFilename("my?file<name>.txt"));
      Assert.AreEqual(
        "my file name_.txt",
        ContentDisposition.MakeFilename("my file\tname\".txt"));
      Assert.AreEqual(
        "my\ufffdfile\ufffdname\ud800\udc00.txt",
        ContentDisposition.MakeFilename("my\ud800file\udc00name\ud800\udc00.txt"));
      Assert.AreEqual(
        "file\ufffdname",
        ContentDisposition.MakeFilename("=?x-unknown?Q?file\ud800name?="));
      Assert.AreEqual(
        "file\u00bename.txt",
        ContentDisposition.MakeFilename("utf-8''file%c2%bename.txt"));
      Assert.AreEqual(
        "file\u00bename.txt",
        ContentDisposition.MakeFilename("utf-8'en'file%c2%bename.txt"));
      Assert.AreEqual(
        "x-unknown'en'file%c2%bename.txt",
        ContentDisposition.MakeFilename("x-unknown'en'file%c2%bename.txt"));
      Assert.AreEqual(
        "file\u00bename.txt",
        ContentDisposition.MakeFilename("utf-8'en-us'file%c2%bename.txt"));
      Assert.AreEqual(
        "_..._",
        ContentDisposition.MakeFilename("..."));
      Assert.AreEqual(
        "_~home",
        ContentDisposition.MakeFilename("~home"));
      Assert.AreEqual(
        "_~nul",
        ContentDisposition.MakeFilename("~nul"));
      Assert.AreEqual(
        "myfilename.txt._",
        ContentDisposition.MakeFilename("myfilename.txt."));
      Assert.AreEqual(
        "_nul",
        ContentDisposition.MakeFilename("nul"));
      Assert.AreEqual(
        "_nul",
        ContentDisposition.MakeFilename("   nul   "));
      Assert.AreEqual(
        "ordinary",
        ContentDisposition.MakeFilename("   ordinary   "));
      Assert.AreEqual(
        "_nul.txt",
        ContentDisposition.MakeFilename("nul.txt"));
      Assert.AreEqual(
        "_con",
        ContentDisposition.MakeFilename("con"));
      Assert.AreEqual(
        "_aux",
        ContentDisposition.MakeFilename("aux"));
      Assert.AreEqual(
        "_lpt1device",
        ContentDisposition.MakeFilename("lpt1device"));
      Assert.AreEqual(
        "my_file_name_.txt",
        ContentDisposition.MakeFilename("my\u0001file\u007fname*.txt"));
      Assert.AreEqual(
        "folder_hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?folder\\hello.txt?="));
      Assert.AreEqual(
        "folder_",
        ContentDisposition.MakeFilename("folder/"));
      Assert.AreEqual(
        "folder______",
        ContentDisposition.MakeFilename("folder//////"));
      Assert.AreEqual(
        "fol_der_",
        ContentDisposition.MakeFilename("fol/der/"));
      Assert.AreEqual(
        "fol_der______",
        ContentDisposition.MakeFilename("fol/der//////"));
      Assert.AreEqual(
        "folder_hello.txt",
        ContentDisposition.MakeFilename("folder/hello.txt"));
      Assert.AreEqual(
        "fol_der_hello.txt",
        ContentDisposition.MakeFilename("fol/der/hello.txt"));
      Assert.AreEqual(
        "folder_hello.txt",
        ContentDisposition.MakeFilename("=?x-unknown?q?folder\\hello.txt?="));
    }

    [TestMethod]
    public void TestCharset() {
      Assert.AreEqual("us-ascii", MediaType.Parse("text/plain").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("TEXT/PLAIN").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("TeXt/PlAiN").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("text/troff").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; CHARSET=UTF-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; ChArSeT=UTF-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; charset=UTF-8").GetCharset());
      // Note that MIME implicitly allows whitespace around the equal sign
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; charset = UTF-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; charset (cmt) = (cmt) UTF-8").GetCharset());
      Assert.AreEqual("'utf-8'", MediaType.Parse("text/plain; charset='UTF-8'").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; foo='; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain; charset=\"UTF-\\8\"").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("nana").GetCharset());
      Assert.AreEqual("", MediaType.Parse("text/xxx").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/xxx;charset=UTF-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/xxx;charset=utf-8").GetCharset());
      Assert.AreEqual("", MediaType.Parse("text/xxx;chabset=utf-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/xml;charset=utf-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("text/plain;charset=utf-8").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("text/plain;chabset=utf-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("image/xml;charset=utf-8").GetCharset());
      Assert.AreEqual("", MediaType.Parse("image/xml;chabset=utf-8").GetCharset());
      Assert.AreEqual("utf-8", MediaType.Parse("image/plain;charset=utf-8").GetCharset());
      Assert.AreEqual("", MediaType.Parse("image/plain;chabset=utf-8").GetCharset());
    }

    [TestMethod]
    public void TestCodePointCompare() {
      Assert.IsTrue(DataUtilities.CodePointCompare("abc", "def") < 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800\udc00", "a\ud900\udc00") < 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800\udc00", "a\ud800\udc00") == 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800", "a\ud800") == 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\udc00", "a\udc00") == 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800\udc00", "a\ud800\udd00") < 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800\ufffd", "a\ud800\udc00") < 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud800\ud7ff", "a\ud800\udc00") < 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ufffd\udc00", "a\ud800\udc00") < 0);
      Assert.IsTrue(DataUtilities.CodePointCompare("a\ud7ff\udc00", "a\ud800\udc00") < 0);
    }

    public static void TestRfc2231Extension(string mtype, string param, string expected) {
      MediaType mt = MediaType.Parse(mtype);
      Assert.AreEqual(expected, mt.GetParameter(param));
    }

    [TestMethod]
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
      Assert.IsNull(MediaType.Parse("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz", null));
      Assert.IsNull(MediaType.Parse("text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz", null));
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

    public static void SingleTestMediaTypeEncoding(string value) {
      MediaType mt = new MediaTypeBuilder("x", "y").SetParameter("z", value).ToMediaType();
      string topLevel = mt.TopLevelType;
      string sub = mt.SubType;
      string mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      using (var ms = new MemoryStream(DataUtilities.GetUtf8Bytes(mtstring, true))) {
        var msg = new Message(ms);
        Assert.AreEqual(topLevel, msg.ContentType.TopLevelType);
        Assert.AreEqual(sub, msg.ContentType.SubType);
        Assert.AreEqual(value, msg.ContentType.GetParameter("z"), mt.ToString());
      }
    }

    [TestMethod]
    public void TestSetHeader() {
      try {
        new Message().SetHeader("from", "\"a\r\nb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\rb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r b\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r\n b\" <x@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r\n b\" <x@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "=?utf-8?q?=01?= <x@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "=?utf-8?q?=01?= <x@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\nb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\0b\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestNamedAddress() {
      Assert.AreEqual("\"Me \" <me@example.com>", new NamedAddress("Me ", "me@example.com").ToString());
      Assert.AreEqual("\" Me\" <me@example.com>", new NamedAddress(" Me", "me@example.com").ToString());
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty, (string)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty, (Address)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x at example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x@"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address((string)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("a b@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("a b@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("ab.example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("ab@exa mple.example"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("ab@example.com addr"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual("Me <me@example.com>",
                        new NamedAddress("Me <me@example.com>").ToString());
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        if ((new NamedAddress("Me\u00e0 <me@example.com>"))==null)Assert.Fail();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        if ((new NamedAddress("\"Me\" <me@example.com>"))==null)Assert.Fail();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        if ((new NamedAddress("\"Me\u00e0\" <me@example.com>"))==null)Assert.Fail();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("Me <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("Me\u00e0 <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("\"Me\" <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("\"Me\u00e0\" <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("Me <me@example.com>, Fred <fred@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsFalse(new NamedAddress("x@example.com").IsGroup);
      Assert.AreEqual("x@example.com", new NamedAddress("x@example.com").Name);
      Assert.AreEqual("x@example.com", new NamedAddress("x@example.com").Address.ToString());
      Assert.AreEqual(
        "\"(lo cal)\"@example.com",
        new Address("\"(lo cal)\"@example.com").ToString());
      Assert.AreEqual(
        "local",
        new Address("local@example.com").LocalPart);
      Assert.AreEqual(
        "example.com",
        new Address("local@example.com").Domain);
      try {
        Assert.AreEqual(null, new Address("local=domain.example"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("local@"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address(EncodingTest.Repeat("local", 200) + "@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("lo,cal@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestHeaderManip() {
      Assert.AreEqual("comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").AddHeader("x-comment", "comment").GetHeader("x-comment"));
      Assert.AreEqual("comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").AddHeader(new KeyValuePair<string, string>("x-comment", "comment")).GetHeader("x-comment"));
      Assert.AreEqual("from", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "you@example.com").GetHeader(0).Key);
      Assert.AreEqual("you@example.com", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "you@example.com").GetHeader(0).Value);
      Assert.AreEqual("x-comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "x-comment", "comment").GetHeader(0).Key);
      Assert.AreEqual("comment", MessageFromString("From: Me <me@example.com>\r\n\r\n").SetHeader(0, "x-comment", "comment").GetHeader(0).Value);
      Message msg = MessageFromString("From: Me <me@example.com>\r\n\r\n");
      try {
        msg.SetHeader(0, (string)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(0, null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.AddHeader(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(-1, "me@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(-1, "To", "me@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.GetHeader(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.RemoveHeader(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestMessageTests() {
      var multipart = "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";
      string msg;
      msg = multipart + "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\nContent-Description: description\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = multipart + "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = multipart + "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: 7bit\r\n\r\nA";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: 7bit\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static void MessageDoubtfulCases() {
      // The following tests currently fail, since I
      // don't know the best way to handle these cases
      // without being too lenient
      var multipart = "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";
      string msg;
      // Multipart message with base64
      msg = multipart + "Content-Transfer-Encoding: base64\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Message top-level-type with base64
      msg = "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: base64\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Truncated top-level multipart message
      msg = multipart + "\r\n--b\r\nContent-Type: text/plain\r\n\r\nHello World";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Truncated top-level multipart message
      msg = multipart + "\r\n--b\r\nContent-Type: text/html\r\n\r\n<b>Hello World</b>";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Truncated top-level multipart message
      msg = multipart + "\r\n--b\r\nContent-Type: text/html\r\n";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Message/rfc822 without a content-transfer-encoding;
      // so a 7-bit encoding is assumed;
      // From header field contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\nX\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Message/rfc822 with content-transfer-encoding base64;
      // which is not allowed for this media type
      msg = multipart + "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: base64\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nXX==\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Text/Rfc822-headers without a content-transfer-encoding;
      // so a 7-bit encoding is assumed;
      // From header field contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = multipart + "\r\n--b\r\nContent-Type: text/rfc822-headers\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Text/html without a content-transfer-encoding and an empty
      // charset, so a 7-bit encoding is assumed;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = "Mime-Version: 1.0\r\nContent-Type: text/html; charset=\"\"\r\n\r\n\ufffd";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Text/html with an explicit Content-Transfer-Encoding of 7bit;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = "Mime-Version: 1.0\r\nContent-Type: text/html\r\nContent-Transfer-Encoding: 7bit\r\n\r\n\ufffd";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Text/html without a content-transfer-encoding,
      // so a 7-bit encoding is assumed;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg = "Mime-Version: 1.0\r\nContent-Type: text/html\r\n\r\n\ufffd";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Base64 body with only one encoded octet;
      // should the incomplete decoded byte be ignored
      // or included?
      msg = "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: base64\r\n\r\nA";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestMailbox() {
      string mbox = "Me <@example.org,@example.net,@example.com:me@x.example>";
      var result = new NamedAddress(mbox);
      Assert.AreEqual("Me <me@x.example>", result.ToString());
    }

    internal static bool HasNestedMessageType(Message message) {
      if (message.ContentType.TopLevelType.Equals("message")) {
        return (!message.ContentType.SubType.Equals("global")) && ((!message.ContentType.SubType.Equals("global-headers")) && ((message.ContentType.SubType.Equals("global-delivery-status")) ||                                                                                                                               (message.ContentType.SubType.Equals("global-disposition-notification"))));
      }
      foreach (Message part in message.Parts) {
        if (HasNestedMessageType(part)) {
          return true;
        }
      }
      return false;
    }

    [TestMethod]
    public void TestBoundaryReading() {
      byte[] body;
      string messageStart = "MIME-Version: 1.0\r\n";
      messageStart += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart += "Preamble\r\n";
      messageStart += "--b1\r\n";
      string message = messageStart;
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      Message msg;
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      Assert.AreEqual("b1", msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("text", msg.Parts[0].ContentType.TopLevelType);
      Assert.AreEqual("Test", msg.Parts[0].BodyString);
      // Nested Multipart body part
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "\r\n--b2\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual(1, msg.Parts[0].Parts.Count);
      Assert.AreEqual("Test", msg.Parts[0].Parts[0].BodyString);
      // No headers in body part
      message = messageStart;
      message += "\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("Test", msg.Parts[0].BodyString);
      // No CRLF before first boundary
      message = "MIME-Version: 1.0\r\n";
      message += "Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      message += "--b1\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("Test", msg.Parts[0].BodyString);
      // Base64 body part
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      Assert.AreEqual("b1", msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("application", msg.Parts[0].ContentType.TopLevelType);
      body = msg.Parts[0].GetBody();
      Assert.AreEqual(0, body[0]);
      Assert.AreEqual(16, body[1]);
      Assert.AreEqual(1, body[2]);
      Assert.AreEqual(93, body[3]);
      Assert.AreEqual(4, body.Length);
      // Base64 body part II
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      Assert.AreEqual("b1", msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("application", msg.Parts[0].ContentType.TopLevelType);
      body = msg.Parts[0].GetBody();
      Assert.AreEqual(0, body[0]);
      Assert.AreEqual(16, body[1]);
      Assert.AreEqual(1, body[2]);
      Assert.AreEqual(93, body[3]);
      Assert.AreEqual(4, body.Length);
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
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      Assert.AreEqual("b1", msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Message part = msg.Parts[0];
      Assert.AreEqual("application", part.Parts[0].ContentType.TopLevelType);
      body = part.Parts[0].GetBody();
      Assert.AreEqual(0, body[0]);
      Assert.AreEqual(16, body[1]);
      Assert.AreEqual(1, body[2]);
      Assert.AreEqual(93, body[3]);
      Assert.AreEqual(4, body.Length);
      // Nested Multipart body part II
      message = messageStart;
      message += "Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message += "--b2\r\n";
      message += "Content-Type: text/plain\r\n\r\n";
      message += "Test\r\n";
      message += "--b2--\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual(1, msg.Parts[0].Parts.Count);
      Assert.AreEqual("Test", msg.Parts[0].Parts[0].BodyString);
    }
    [TestMethod]
    public void TestArgumentValidationMediaType() {
      try {
        MediaType.TextPlainAscii.GetParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MediaType.TextPlainAscii.GetParameter(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MediaType.Parse(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual("application", new MediaTypeBuilder().TopLevelType);
      Assert.AreEqual("text", new MediaTypeBuilder(MediaType.TextPlainAscii).TopLevelType);
      Assert.AreEqual("plain", new MediaTypeBuilder(MediaType.TextPlainAscii).SubType);
      Assert.AreEqual(MediaType.TextPlainAscii, MediaType.Parse("text/plain; charset=us-ascii"));
      Assert.IsTrue(MediaType.TextPlainAscii.GetHashCode() == MediaType.Parse("text/plain; charset=us-ascii").GetHashCode());
    }
    [TestMethod]
    public void TestMediaTypeBuilder() {
      MediaTypeBuilder builder;
      try {
        Assert.AreEqual(null, new MediaTypeBuilder(null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      builder = new MediaTypeBuilder("text", "plain");
      try {
        builder.SetTopLevelType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter(null, "v");
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter(String.Empty, "v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter("v", null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetTopLevelType(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetTopLevelType("e=");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetTopLevelType("e/e");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetSubType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter("v");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetSubType(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x;y");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x/y");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x", String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x;y", "v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x/y", "v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    //[TestMethod]
    public void TestMessageMergeFields() {
      string msg;
      msg = "From: x1@example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "To: x1@example.com\r\nTo: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("to");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Cc: x1@example.com\r\nCc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("cc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Bcc: x1@example.com\r\nBcc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("bcc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Reply-To: x1@example.com\r\nReply-To: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("reply-to");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Resent-To: x1@example.com\r\nResent-To: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("resent-to");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Resent-Cc: x1@example.com\r\nResent-Cc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("resent-cc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Resent-Bcc: x1@example.com\r\nResent-Bcc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("resent-bcc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      // Invalid header fields
      msg = "From: x1@example.com\r\nFrom: x2.example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.AreEqual("x1@example.com", msg);
      msg = "From: x1.example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.AreEqual("x2@example.com", msg);
    }

    [TestMethod]
    public void TestMessageArgumentValidation() {
      try {
        new Message().GetHeader(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetBody(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestSetHeader2() {
      Assert.AreEqual("my subject", new Message().SetHeader("comments", "subject").SetHeader("subject", "my subject").GetHeader("subject"));
    }

    [TestMethod]
    public void TestMediaTypeArgumentValidationExtra() {
      Assert.IsTrue(MediaType.Parse("text/plain").IsText);
      Assert.IsTrue(MediaType.Parse("multipart/alternative").IsMultipart);
      Assert.AreEqual("example/x", MediaType.Parse("example/x ").TypeAndSubType);
      Assert.AreEqual("text/plain", MediaType.Parse("example/x, a=b").TypeAndSubType);
      Assert.AreEqual("example/x", MediaType.Parse("example/x ; a=b").TypeAndSubType);
      Assert.AreEqual("example/x", MediaType.Parse("example/x; a=b").TypeAndSubType);
      Assert.AreEqual("example/x", MediaType.Parse("example/x; a=b ").TypeAndSubType);
    }
    [TestMethod]
    public void TestContentHeadersOnlyInBodyParts() {
      var msg = new Message().SetTextAndHtml("Hello", "Hello");
      msg.SetHeader("x-test", "test");
      msg.Parts[0].SetHeader("x-test", "test");
      Assert.AreEqual("test", msg.GetHeader("x-test"));
      Assert.AreEqual("test", msg.Parts[0].GetHeader("x-test"));
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(msg.Generate(), true)));
      Assert.AreEqual("test", msg.GetHeader("x-test"));
      Assert.AreEqual(null, msg.Parts[0].GetHeader("x-test"));
    }
  }
}
