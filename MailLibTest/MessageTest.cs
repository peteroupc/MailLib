using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;

namespace MailLibTest {
  [TestFixture]
  public class MessageTest {
    [Test]
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

    public static string MessageGenerate(Message msg) {
      if (msg == null) {
        Assert.Fail();
      }
      string ret = msg.Generate();
{
object objectTemp = 2;
object objectTemp2 = EncodingTest.IsGoodAsciiMessageFormat(
  ret,
  false,
  String.Empty);
string messageTemp = ret;
Assert.AreEqual(objectTemp, objectTemp2, messageTemp);
}
      return ret;
    }

    [Test]
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
        string stringTemp = MediaType.Parse("x/y;z=1;z*=utf-8''2")
.GetParameter("z");
        Assert.AreEqual(
        "2",
        stringTemp);
      }
    }

    internal static Message MessageFromString(string valueMessageString) {
      var msgobj = new Message(
  DataUtilities.GetUtf8Bytes(
  valueMessageString,
  true));
      MessageGenerate(msgobj);
      return msgobj;
    }

    internal static void MessageConstructOnly(string valueMessageString) {
      new Message(
  DataUtilities.GetUtf8Bytes(
  valueMessageString,
  true));
    }

    private static void TestMediaTypeRoundTrip(string valueMessageString) {
      string mtstring = new MediaTypeBuilder(
  "x",
  "y").SetParameter(
  "z",
  valueMessageString).ToString();
      Assert.IsFalse(mtstring.Contains("\r\n\r\n"));
      Assert.IsFalse(mtstring.Contains("\r\n \r\n"));
   {
object objectTemp = valueMessageString;
object objectTemp2 = MediaType.Parse(mtstring).GetParameter(
  "z");
Assert.AreEqual(objectTemp, objectTemp2);
}
  Message mtmessage = MessageFromString("MIME-Version: 1.0\r\nContent-Type: " +
                    mtstring + "\r\n\r\n");
      MessageGenerate(mtmessage);
    }

    [Test]
    public void TestGenerate() {
      var msgids = new List<string>();
      // Tests whether unique Message IDs are generated for each message.
      for (int i = 0; i < 1000; ++i) {
        string msgtext = MessageGenerate(new Message().SetHeader(
          "from",
          "me@example.com")
          .SetTextBody("Hello world."));
        if (EncodingTest.IsGoodAsciiMessageFormat(
          msgtext,
          false,
          "TestGenerate") != 2) {
          Assert.Fail("Bad message format generated");
        }
        string msgid = MessageFromString(msgtext).GetHeader("message-id");
        if (msgids.Contains(msgid)) {
          Assert.Fail(msgid);
        }
        msgids.Add(msgid);
      }
    }

    [Test] public void TestMultipleReplyTo () {
            const string ValueMultipleReplyTo = "Reply-to: x@example.com\r\n" +
             "Reply-to: y@example.com\r\n" + "Reply-to: z@example.com\r\n" +
             "Reply-to: w@example.com\r\n" + "From: me@example.com\r\n\r\n";
            try {
 MessageFromString (ValueMultipleReplyTo).Generate();
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
        }

    [Test]
    public void TestContentTypeDefaults() {
  const string ValueStartCTD = "From: me@example.com\r\nMIME-Version: 1.0\r\n";
      string msg;
      msg = ValueStartCTD + "\r\n\r\n";
      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MessageFromString(msg).ContentType);
      msg = ValueStartCTD + "Content-Type: text/html\r\n\r\n";
      Assert.AreEqual(
  MediaType.Parse("text/html"),
  MessageFromString(msg).ContentType);
      msg = ValueStartCTD + "Content-Type: text/\r\n\r\n";
      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MessageFromString(msg).ContentType);
      msg = ValueStartCTD + "Content-Type: /html\r\n\r\n";
      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MessageFromString(msg).ContentType);
      // All header fields are syntactically valid
      msg = ValueStartCTD +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";

      Assert.AreEqual(
  MediaType.ApplicationOctetStream,
  MessageFromString(msg).ContentType);
      msg = ValueStartCTD +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: text/html\r\n\r\n";

      Assert.AreEqual(
  MediaType.ApplicationOctetStream,
  MessageFromString(msg).ContentType);
      // First header field is syntactically invalid
      msg = ValueStartCTD +
  "Content-Type: /plain;charset=utf-8\r\nContent-Type: image/jpeg\r\n\r\n";
      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MessageFromString(msg).ContentType);
      // Second header field is syntactically invalid
      msg = ValueStartCTD +
  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\n\r\n";
      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MessageFromString(msg).ContentType);
      msg = ValueStartCTD +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image\r\nContent-Type: text/html\r\n\r\n";

      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MessageFromString(msg).ContentType);
      // Third header field is syntactically invalid
      msg = ValueStartCTD +

  "Content-Type: text/plain;charset=utf-8\r\nContent-Type: image/jpeg\r\nContent-Type: audio\r\n\r\n";

      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MessageFromString(msg).ContentType);
    }

    [Test]
    public void TestNewMessage() {
      Assert.IsTrue(new Message().ContentType != null);
    }

    [Test]
    public void TestPrematureEnd() {
      string[] messages = {
        "From: me@example.com\r\nDate",
        "From: me@example.com\r\nDate\r",
        "Received: from x",
        "Received: from x\r"
      };
      foreach (string msgstr in messages) {
        try {
          byte[] data = DataUtilities.GetUtf8Bytes(msgstr, true);
          Assert.AreEqual(null, new Message(data));
          Assert.Fail("Should have failed");
        } catch (MessageDataException) {
          new Object();
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    public static void TestRfc2231Extension(
  string mtype,
  string param,
  string expected) {
      MediaType mt = MediaType.Parse(mtype);
      Assert.AreEqual(expected, mt.GetParameter(param));
      var valueMessageString = "From: me@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: " + mtype + "\r\n\r\nTest";
      Message msg = MessageFromString(valueMessageString);
      mt = msg.ContentType;
      Assert.AreEqual(expected, mt.GetParameter(param));
    }

    [Test]
    public void TestRfc2231Extensions() {
      TestRfc2231Extension("text/plain; charset=\"utf-8\"", "charset", "utf-8");
      TestRfc2231Extension(
  "text/plain; charset*=us-ascii'en'utf-8",
  "charset",
  "utf-8");
      TestRfc2231Extension(
  "text/plain; charset*=us-ascii''utf-8",
  "charset",
  "utf-8");
      TestRfc2231Extension(
  "text/plain; charset*='en'utf-8",
  "charset",
  "utf-8");
      TestRfc2231Extension(
  "text/plain; charset*='i-unknown'utf-8",
  "charset",
  "us-ascii");
      TestRfc2231Extension(
  "text/plain; charset*=us-ascii'i-unknown'utf-8",
  "charset",
  "us-ascii");
      TestRfc2231Extension("text/plain; charset*=''utf-8", "charset", "utf-8");
      TestRfc2231Extension(
  "text/plain; charset*0=a;charset*1=b",
  "charset",
  "ab");
      TestRfc2231Extension(
  "text/plain; charset*=utf-8''a%20b",
  "charset",
  "a b");
      TestRfc2231Extension(
  "text/plain; charset*=iso-8859-1''a%a0b",
  "charset",
  "a\u00a0b");
      TestRfc2231Extension(
  "text/plain; charset*=utf-8''a%c2%a0b",
  "charset",
  "a\u00a0b");
      TestRfc2231Extension(
  "text/plain; charset*=iso-8859-1''a%a0b",
  "charset",
  "a\u00a0b");
      TestRfc2231Extension(
  "text/plain; charset*=utf-8''a%c2%a0b",
  "charset",
  "a\u00a0b");
      TestRfc2231Extension(
  "text/plain; charset*0=\"a\";charset*1=b",
  "charset",
  "ab");

  TestRfc2231Extension(
  "text/plain; charset*0*=utf-8''a%20b;charset*1*=c%20d",
  "charset",
  "a bc d");
      TestRfc2231Extension(
        "text/plain; charset*0=ab;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "abiso-8859-1-en-xyz");
      TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1-en-xyz",
        "charset",
        "a biso-8859-1-en-xyz");

 if (
  MediaType.Parse(
  "text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz",
  null) != null) {
        Assert.Fail();
      }

 if (
  MediaType.Parse(
  "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz",
  null) != null) {
        Assert.Fail();
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

    public static void SingleTestMediaTypeEncoding(string value) {
      MediaType mt = new MediaTypeBuilder(
  "x",
  "y").SetParameter(
  "z",
  value).ToMediaType();
      string topLevel = mt.TopLevelType;
      string sub = mt.SubType;
      string mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      Message msg = MessageFromString(mtstring);
      Assert.AreEqual(topLevel, msg.ContentType.TopLevelType);
      Assert.AreEqual(sub, msg.ContentType.SubType);
      Assert.AreEqual(
  value,
  msg.ContentType.GetParameter("z"),
  mt.ToString());
    }

    [Test]
    public void TestNamedAddress() {
      {
object objectTemp = "\"Me \" <me@example.com>";
object objectTemp2 = new NamedAddress(
  "Me ",
  "me@example.com").ToString();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
object objectTemp = "\" Me\" <me@example.com>";
object objectTemp2 = new NamedAddress(
  " Me",
  "me@example.com").ToString();
Assert.AreEqual(objectTemp, objectTemp2);
}

      try {
        Assert.AreEqual(null, new Address(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("a b@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("a b@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("ab.example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("ab@exa mple.example"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("ab@example.com addr"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        {
          string stringTemp = new
                    NamedAddress("Me <me@example.com>").ToString();
          Assert.AreEqual(
          "Me <me@example.com>",
          stringTemp);
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        if ((new NamedAddress("Me\u00e0 <me@example.com>")) == null) {
          Assert.Fail();
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        if ((new NamedAddress("\"Me\" <me@example.com>")) == null) {
          Assert.Fail();
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        if ((new NamedAddress("\"Me\u00e0\" <me@example.com>")) == null) {
          Assert.Fail();
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("Me <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("Me\u00e0 <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("\"Me\" <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("\"Me\u00e0\" <me@example.com>"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        const string ValueSt = "Me <me@example.com>, Fred <fred@example.com>";
        Assert.AreEqual(null, new NamedAddress(ValueSt));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsFalse(new NamedAddress("x@example.com").IsGroup);
      {
        string stringTemp = new NamedAddress("x@example.com").Name;
        Assert.AreEqual(
          "x@example.com",
          stringTemp);
      }
      {
        string stringTemp = new NamedAddress(
                "x@example.com").Address.ToString();
        Assert.AreEqual(
        "x@example.com",
        stringTemp);
      }
      Assert.AreEqual(
  "\"(lo cal)\"@example.com",
  new Address("\"(lo cal)\"@example.com").ToString());
      {
        string stringTemp = new Address("local@example.com").LocalPart;
        Assert.AreEqual(
          "local",
          stringTemp);
      }
      {
        string stringTemp = new Address("local@example.com").Domain;
        Assert.AreEqual(
          "example.com",
          stringTemp);
      }
    }

    [Test]
    public void TestHeaderManip() {
      {
        string stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").AddHeader("x-comment", "comment")
.GetHeader("x-comment");
        Assert.AreEqual(
          "comment",
          stringTemp);
      }
      {
        var kvp = new KeyValuePair<string, string>("x-comment", "comment");
        string stringTemp = MessageFromString(
    "From: Me <me@example.com>\r\n\r\n")
.AddHeader(kvp).GetHeader("x-comment");
        Assert.AreEqual(
          "comment",
          stringTemp);
      }
      {
        string stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(
  0,
  "you@example.com").GetHeader(0).Key;
        Assert.AreEqual(
          "from",
          stringTemp);
      }
      {
        string stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(
  0,
  "you@example.com").GetHeader(0).Value;
        Assert.AreEqual(
          "you@example.com",
          stringTemp);
      }
      {
        string stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(
  0,
  "x-comment",
  "comment").GetHeader(0).Key;
        Assert.AreEqual(
          "x-comment",
          stringTemp);
      }
      {
        string stringTemp = MessageFromString(
          "From: Me <me@example.com>\r\n\r\n").SetHeader(
  0,
  "x-comment",
  "comment").GetHeader(0).Value;
        Assert.AreEqual(
          "comment",
          stringTemp);
      }
      Message msg = MessageFromString("From: Me <me@example.com>\r\n\r\n");
      try {
        msg.SetHeader(0, (string)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(0, null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.AddHeader(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(-1, "me@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(-1, "To", "me@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.GetHeader(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.RemoveHeader(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestMessageTests() {
      const string ValueMultipart =
  "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";

      string msg;
      msg = ValueMultipart +

  "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\nContent-Description: description\r\n\r\n\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = ValueMultipart +
        "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = ValueMultipart +
        "Content-Transfer-Encoding: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg =

  "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: 8bit\r\n\r\n--b\r\n\r\n\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg =

  "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: 7bit\r\n\r\nA";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: 7bit\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nX\r\n--b--";

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
      const string ValueMultipart =
  "MIME-Version: 1.0\r\nContent-Type: multipart/mixed; boundary=b\r\n";

      string msg;
      // Multipart message with base64
      msg = ValueMultipart +
        "Content-Transfer-Encoding: base64\r\n\r\n--b\r\n\r\n\r\n--b--";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Message top-level-type with base64
      msg =

  "MIME-Version: 1.0\r\nContent-Type: message/rfc822\r\nContent-Type: base64\r\n--b\r\n\r\n\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Truncated top-level multipart message
      msg = ValueMultipart +
          "\r\n--b\r\nContent-Type: text/plain\r\n\r\nHello World";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Truncated top-level multipart message
      msg = ValueMultipart +
        "\r\n--b\r\nContent-Type: text/html\r\n\r\n<b>Hello World</b>";
      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Truncated top-level ValueMultipart message
      msg = ValueMultipart + "\r\n--b\r\nContent-Type: text/html\r\n";
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
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\nX\r\n--b--";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Message/rfc822 with content-transfer-encoding base64;
      // which is not allowed for this media type
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: message/rfc822\r\nContent-Transfer-Encoding: base64\r\n\r\nFrom: \"Me\" <me@example.com>\r\n\r\nXX==\r\n--b--";

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
      msg = ValueMultipart +

  "\r\n--b\r\nContent-Type: text/rfc822-headers\r\n\r\nFrom: \"\ufffd\ufffd\" <me@example.com>\r\n\r\n--b--";

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
      msg =
  "Mime-Version: 1.0\r\nContent-Type: text/html; charset=\"\"\r\n\r\n\ufffd";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      // Text/html with an explicit Content-Transfer-Encoding of 7bit;
      // Body contains non-ASCII characters, so
      // would be illegal in a 7-bit encoding
      msg =

  "Mime-Version: 1.0\r\nContent-Type: text/html\r\nContent-Transfer-Encoding: 7bit\r\n\r\n\ufffd";

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
      msg =

  "Mime-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nContent-Transfer-Encoding: base64\r\n\r\nA";

      try {
        MessageFromString(msg);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    internal static bool HasNestedMessageType(Message message) {
      if (message.ContentType.TopLevelType.Equals("message")) {
        return (!message.ContentType.SubType.Equals("global")) &&
          ((!message.ContentType.SubType.Equals("global-headers")) &&
           (message.ContentType.SubType.Equals("global-delivery-status") ||
    message.ContentType.SubType.Equals("global-disposition-notification")));
      }
      foreach (Message part in message.Parts) {
        if (HasNestedMessageType(part)) {
          return true;
        }
      }
      return false;
    }

    [Test]
    public void TestGetDate() {
      var msg = new Message();
      int[] date;
      msg.SetHeader("date", "Sat, 1 Jan 2000 12:34:56 +1034");
      date = msg.GetDate();
      Assert.AreEqual(2000, date[0]);
      Assert.AreEqual(1, date[1]);
      Assert.AreEqual(1, date[2]);
      Assert.AreEqual(12, date[3]);
      Assert.AreEqual(34, date[4]);
      Assert.AreEqual(56, date[5]);
      Assert.AreEqual(0, date[6]);
      Assert.AreEqual((10 * 60) + 34, date[7]);
      msg.SetHeader("date", "Mon, 1 Jan 1900 23:59:60 -1034");
      date = msg.GetDate();
      Assert.AreEqual(1900, date[0]);
      Assert.AreEqual(1, date[1]);
      Assert.AreEqual(1, date[2]);
      Assert.AreEqual(23, date[3]);
      Assert.AreEqual(59, date[4]);
      Assert.AreEqual(60, date[5]);
      Assert.AreEqual(0, date[6]);
      Assert.AreEqual(-((10 * 60) + 34), date[7]);
      msg.SetHeader("date", "Sun, 1 Jan 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
      msg.SetHeader("date", "1 Jan 2000 12:34:56 +1034");
      date = msg.GetDate();
      Assert.AreEqual(2000, date[0]);
      Assert.AreEqual(1, date[1]);
      Assert.AreEqual(1, date[2]);
      msg.SetHeader("date", "32 Jan 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
      msg.SetHeader("date", "30 Feb 2000 12:34:56 +1034");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
      msg.SetHeader("date", "1 Feb 999999999999999999999 12:34:56 +1034");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
      msg.SetHeader("date", "1 Jan 2000 24:34:56 +1034");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
      msg.SetHeader("date", "1 Jan 2000 01:60:56 +1034");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
      msg.SetHeader("date", "1 Jan 2000 01:01:61 +1034");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
      msg.SetHeader("date", "1 Jan 2000 01:01:01 +1099");
      if (msg.GetDate() != null) {
 Assert.Fail();
 }
    }

    [Test]
    public void TestSetDate() {
      var msg = new Message();
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 0, 0 });
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { -1, 1, 1, 0, 0, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 0, 1, 0, 0, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 13, 1, 0, 0, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 0, 0, 0, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 32, 0, 0, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, -5, 0, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 24, 0, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, -1, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 60, 0, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 0, -1, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 61, 0, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, -1, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 1000, 0 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 0, -1440 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 msg.SetDate(new int[] { 2000, 1, 1, 0, 0, 0, 0, 1440 });
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }

    [Test]
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
      msg = MessageFromString(message);
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      {
        string stringTemp = msg.ContentType.GetParameter("boundary");
        Assert.AreEqual(
          "b1",
          stringTemp);
      }
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
      msg = MessageFromString(message);
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual(1, msg.Parts[0].Parts.Count);
      Assert.AreEqual("Test", msg.Parts[0].Parts[0].BodyString);
      // No headers in body part
      message = messageStart;
      message += "\r\n";
      message += "Test\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
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
      msg = MessageFromString(message);
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("Test", msg.Parts[0].BodyString);
      // Base64 body part
      message = messageStart;
      message += "Content-Type: application/octet-stream\r\n";
      message += "Content-Transfer-Encoding: base64\r\n\r\n";
      message += "ABABXX==\r\n";
      message += "--b1--\r\n";
      message += "Epilogue";
      msg = MessageFromString(message);
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      {
        string stringTemp = msg.ContentType.GetParameter("boundary");
        Assert.AreEqual(
          "b1",
          stringTemp);
      }
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
      msg = MessageFromString(message);
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      {
        string stringTemp = msg.ContentType.GetParameter("boundary");
        Assert.AreEqual(
          "b1",
          stringTemp);
      }
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
      msg = MessageFromString(message);
      Assert.AreEqual("multipart", msg.ContentType.TopLevelType);
      {
        string stringTemp = msg.ContentType.GetParameter("boundary");
        Assert.AreEqual(
          "b1",
          stringTemp);
      }
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
      msg = MessageFromString(message);
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual(1, msg.Parts[0].Parts.Count);
      Assert.AreEqual("Test", msg.Parts[0].Parts[0].BodyString);
    }
    [Test]
    public void TestArgumentValidationMediaType() {
      try {
        MediaType.TextPlainAscii.GetParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MediaType.TextPlainAscii.GetParameter(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MediaType.Parse(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      {
        string stringTemp = new MediaTypeBuilder().TopLevelType;
        Assert.AreEqual(
          "application",
          stringTemp);
      }
      {
        string stringTemp = new
                    MediaTypeBuilder(MediaType.TextPlainAscii).TopLevelType;
        Assert.AreEqual(
        "text",
        stringTemp);
      }
      {
        string stringTemp = new
                    MediaTypeBuilder(MediaType.TextPlainAscii).SubType;
        Assert.AreEqual(
        "plain",
        stringTemp);
      }
      Assert.AreEqual(
  MediaType.TextPlainAscii,
  MediaType.Parse("text/plain; charset=us-ascii"));
      Assert.IsTrue(MediaType.TextPlainAscii.GetHashCode() ==
                MediaType.Parse("text/plain; charset=us-ascii")
.GetHashCode());
    }
    [Test]
    public void TestMediaTypeBuilder() {
      MediaTypeBuilder builder;
      try {
        Assert.AreEqual(null, new MediaTypeBuilder(null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      builder = new MediaTypeBuilder("text", "plain");
      try {
        builder.SetTopLevelType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter(null, "v");
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter(String.Empty, "v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter("v", null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetTopLevelType(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetTopLevelType("e=");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetTopLevelType("e/e");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetSubType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().RemoveParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
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
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x;y");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetSubType("x/y");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
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
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x/y", "v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    // [Test]
    public static void TestMessageMergeFields() {
      string msg;
      msg = "From: x1@example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg =
        MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "To: x1@example.com\r\nTo: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "to");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Cc: x1@example.com\r\nCc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "cc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Bcc: x1@example.com\r\nBcc: x2@example.com\r\n\r\n";
      msg = MessageFromString(MessageFromString(msg).Generate()).GetHeader(
   "bcc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Reply-To: x1@example.com\r\nReply-To: x2@example.com\r\n\r\n";
      msg =
MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "reply-to");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Resent-To: x1@example.com\r\nResent-To: x2@example.com\r\n\r\n";
      msg =
MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "resent-to");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Resent-Cc: x1@example.com\r\nResent-Cc: x2@example.com\r\n\r\n";
      msg =
MessageFromString(MessageFromString(msg).Generate()).GetHeader(
  "resent-cc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      msg = "Resent-Bcc: x1@example.com\r\nResent-Bcc: x2@example.com\r\n\r\n";
      msg =
MessageFromString(MessageFromString(msg).Generate())
          .GetHeader("resent-bcc");
      Assert.AreEqual("x1@example.com, x2@example.com", msg);
      // Invalid header fields
      msg = "From: x1@example.com\r\nFrom: x2.example.com\r\n\r\n";
      msg =
        MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.AreEqual("x1@example.com", msg);
      msg = "From: x1.example.com\r\nFrom: x2@example.com\r\n\r\n";
      msg =
        MessageFromString(MessageFromString(msg).Generate()).GetHeader("from");
      Assert.AreEqual("x2@example.com", msg);
    }

    [Test]
    public void TestFWSAtSubjectEnd() {
      Message msg;
   const string ValueStringVar = "From: me@example.com\r\nSubject: Test\r\n " +
        "\r\nX-Header: Header\r\n\r\nBody";
      msg = MessageFromString(ValueStringVar);
      {
        string stringTemp = msg.GetHeader("subject");
        Assert.AreEqual(
        "Test ",
        stringTemp);
      }
    }

    [Test]
    public void TestEmptyGroup() {
    const string ValueStringVar = "From: me@example.com\r\nTo: empty-group:;" +
        "\r\nCc: empty-group:;" + "\r\nBcc: empty-group:;" +
        "\r\n\r\nBody";
      MessageFromString(ValueStringVar);
    }

    [Test]
    public void TestMediaTypeArgumentValidationExtra() {
      Assert.IsTrue(MediaType.Parse("text/plain").IsText);
      Assert.IsTrue(MediaType.Parse("multipart/alternative").IsMultipart);
      {
        string stringTemp = MediaType.Parse(
                "example/x ").TypeAndSubType;
        Assert.AreEqual(
        "example/x",
        stringTemp);
      }
      const string ValueStrtest = "example/x" + "," + " a=b";
      {
        string stringTemp = MediaType.Parse(ValueStrtest).TypeAndSubType;
        Assert.AreEqual(
          "text/plain",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("example/x ; a=b")
.TypeAndSubType;
        Assert.AreEqual(
        "example/x",
        stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("example/x; a=b")
.TypeAndSubType;
        Assert.AreEqual(
        "example/x",
        stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("example/x; a=b ")
.TypeAndSubType;
        Assert.AreEqual(
        "example/x",
        stringTemp);
      }
    }
    [Test]
    public void TestContentHeadersOnlyInBodyParts() {
      var msg = new Message().SetTextAndHtml("Hello", "Hello");
      msg.SetHeader("x-test", "test");
      msg.Parts[0].SetHeader("x-test", "test");
      {
        string stringTemp = msg.GetHeader("x-test");
        Assert.AreEqual(
          "test",
          stringTemp);
      }
      {
        string stringTemp = msg.Parts[0].GetHeader("x-test");
        Assert.AreEqual(
          "test",
          stringTemp);
      }
      msg = MessageFromString(msg.Generate());
      {
        string stringTemp = msg.GetHeader("x-test");
        Assert.AreEqual(
          "test",
          stringTemp);
      }
      Assert.AreEqual(null, msg.Parts[0].GetHeader("x-test"));
    }

    [Test]
    public void TestConstructor() {
      try {
        Assert.AreEqual(null, new Message((Stream)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Message((byte[])null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
  MessageConstructOnly(
  "From: x@example.com\r\nSub ject: Test\r\n\r\nBody");
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
    MessageConstructOnly("From: x@example.com\r\nX-" +
          EncodingTest.Repeat(
  "a",
  2000) + ": Test\r\n\r\nBody");
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
    MessageConstructOnly("From: x@example.com\r\nX-" +
          EncodingTest.Repeat(
  "a",
  996) + ":\r\n Test\r\n\r\nBody");
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\n: Test\r\n\r\nBody");
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nSubject: Test\r\n\rBody");
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\r\nSubject: Test\r\n\nBody");
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MessageConstructOnly("From: x@example.com\nSubject: Test\n\nBody");
        Assert.Fail("Should have failed");
      } catch (MessageDataException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MessageFromString(
  "From: x@example.com\r\nX-" + EncodingTest.Repeat(
  "a",
  995) + ":\r\n Test\r\n\r\nBody");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MessageFromString("From: x@example.com\r\nX-Test:\r\n " +
          EncodingTest.Repeat("a", 997) + "\r\n\r\nBody");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestAddHeader() {
      // not implemented yet
    }
    [Test]
    public void TestBccAddresses() {
      // not implemented yet
    }
    [Test]
    public void TestBodyString() {
      var msg = new Message().SetTextBody("Test");
      string mtype = "text/plain;charset=x-unknown";
      msg.ContentType = MediaType.Parse(mtype);
      try {
        msg.BodyString.ToString();
        Assert.Fail("Should have failed");
      } catch (NotSupportedException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestCCAddresses() {
      // not implemented yet
    }
    [Test]
    public void TestContentDisposition() {
      // not implemented yet
    }
    [Test]
    public void TestContentType() {
      var msg = new Message().SetTextBody("text");
      msg.ContentType = MediaType.Parse("text/html");
      try {
        msg.ContentType = null;
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static void TestFileNameOne(string input, string expected) {
      Message msg;
   String valueMessageString = "From: x@example.com\r\nMIME-Version: 1.0\r\n" +
 "Content-Type: text/plain\r\nContent-Disposition: inline; filename=" +
          input + "\r\n\r\nEmpty.";
      msg = MessageFromString(valueMessageString);
      Assert.AreEqual(expected, msg.FileName);
      valueMessageString = "From: x@example.com\r\nMIME-Version: 1.0\r\n" +
        "Content-Type: text/plain; name=" + input +
        "\r\n\r\nEmpty.";
      msg = MessageFromString(valueMessageString);
      Assert.AreEqual(expected, msg.FileName);
    }
    [Test]
    public void TestFileName() {
      TestFileNameOne("com.txt", "com.txt");
      TestFileNameOne("com0.txt", "_com0.txt");
      TestFileNameOne("-hello.txt", "_-hello.txt");
      TestFileNameOne("lpt0.txt", "_lpt0.txt");
      TestFileNameOne("\"hello.txt\"", "hello.txt");
      TestFileNameOne("\"=?utf-8?q?hello=2Etxt?=\"", "hello.txt");
      TestFileNameOne("\"utf-8''hello%2Etxt\"", "hello.txt");
    }
    [Test]
    public void TestFromAddresses() {
 string valueMessageString =
        "From: me@example.com\r\nSubject: Subject\r\n\r\nBody";
      Message msg = MessageFromString(valueMessageString);
      MessageFromString(MessageGenerate(msg));
      Assert.AreEqual(1, msg.FromAddresses.Count);
    }
    [Test]
    public void TestGetBody() {
      // not implemented yet
    }
    [Test]
    public void TestGetBodyMessage() {
      // not implemented yet
    }
    [Test]
    public void TestGetHeader() {
    const string ValueVsv = "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
      Message msg = MessageFromString(ValueVsv);
      try {
        msg.GetHeader(2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().GetHeader(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestHeaderFields() {
      // not implemented yet
    }
    [Test]
    public void TestParts() {
      // not implemented yet
    }

const string ValueVrhs =
        "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
    [Test]
    public void TestRemoveHeader() {
      Message msg = MessageFromString(ValueVrhs);
      try {
        msg.RemoveHeader(2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetBody() {
      try {
        new Message().SetBody(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetHeader() {
      try {
        new Message().SetHeader("from", "\"a\r\nb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\rb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r b\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\r\n b\" <x@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
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
        new Object();
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
 new Message().SetHeader("from", "\"Me\" <x@example.com>");
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
        new Message().SetHeader("from", "\"a\nb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from", "\"a\0b\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
const string ValueVms =
        "From: me@example.com\r\nX-Header: 1\r\n\r\nTest";
      Message msg = MessageFromString(ValueVms);
      try {
        msg.SetHeader(2, "X-Header2", "2");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(2, "2");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        msg.SetHeader(1, (string)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      {
        string stringTemp = new Message()
                    .SetHeader("comments", "subject")
                    .SetHeader("subject", "my subject")
                    .GetHeader("subject");
        Assert.AreEqual(
        "my subject",
        stringTemp);
      }
      try {
        new Message().SetHeader(EncodingTest.Repeat("a", 998), "x");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("e:d", "x");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("e d", "x");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("e\u007f", "x");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("e\u00a0", "x");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("e\u0008", "x");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      string longHeader = EncodingTest.Repeat("a", 997);
      try {
        MessageGenerate(new Message().SetHeader(longHeader, "x"));
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
            var headerNames = new string[] { "from", "to", "cc",
              "bcc", "reply-to" };
            foreach (string headerName in headerNames) {
                try {
 new Message().SetHeader(headerName, "\"Me\" <x@example.com>");
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
                try {
 new Message().SetHeader(headerName, "\"Me Me\" <x@example.com>");
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
                try {
 new Message().SetHeader(headerName, "\"Me(x)\" <x@example.com>");
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
                try {
 new Message().SetHeader(headerName, "\"Me\u002c Me\" <x@example.com>");
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
                try {
 new Message().SetHeader(headerName, "\"Me\u002c Me(x)\" <x@example.com>");
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
            }
        }
    [Test]
    public void TestSetHtmlBody() {
      var msg = new Message();
      try {
        msg.SetHtmlBody(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetTextAndHtml() {
      try {
        new Message().SetTextAndHtml(null, "test");
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetTextAndHtml("test", null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetTextBody() {
      var msg = new Message();
      try {
        msg.SetTextBody(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        new Object();
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSubject() {
      var msg = new Message();
      msg.Subject = "Test";
      Assert.AreEqual("Test", msg.Subject);
      msg.Subject = "Test2";
      Assert.AreEqual("Test2", msg.Subject);
    }
    [Test]
    public void TestToAddresses() {
      // not implemented yet
    }
  }
}
