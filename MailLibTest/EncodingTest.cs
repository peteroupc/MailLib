/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;
using PeterO;
using PeterO.Mail;
using PeterO.Text;

namespace MailLibTest {
  [TestFixture]
  public class EncodingTest
  {
    /// <summary>Note: If lenientLineBreaks is true, treats CR, LF, and
    /// CRLF as line breaks and writes CRLF when encountering these breaks.
    /// If unlimitedLineLength is true, doesn't check that no more than 76
    /// characters are in each line. If an encoded line ends with spaces and/or
    /// tabs, those characters are deleted (RFC 2045, sec. 6.7, rule 3).</summary>
    /// <param name='outputStream'>A readable data stream.</param>
    /// <param name='data'>A byte array.</param>
    /// <param name='offset'>A 32-bit signed integer.</param>
    /// <param name='count'>A 32-bit signed integer. (2).</param>
    /// <param name='lenientLineBreaks'>A Boolean object.</param>
    /// <param name='unlimitedLineLength'>A Boolean object. (2).</param>
    private static void ReadQuotedPrintable(
      Stream outputStream,
      byte[] data,
      int offset,
      int count,
      bool lenientLineBreaks,
      bool unlimitedLineLength) {
      if (outputStream == null) {
        throw new ArgumentNullException("outputStream");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count < 0) {
        throw new ArgumentException("count (" + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" + Convert.ToString((long)(data.Length - offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture));
      }
      using (MemoryStream ms = new MemoryStream(data, offset, count)) {
        QuotedPrintableTransform t = new QuotedPrintableTransform(
          new WrappedStream(ms),
          lenientLineBreaks,
          unlimitedLineLength ? -1 : 76,
          false);
        while (true) {
          int c = t.ReadByte();
          if (c < 0) {
            return;
          }
          outputStream.WriteByte((byte)c);
        }
      }
    }

    public void TestDecodeQuotedPrintable(string input, string expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using (MemoryStream ms = new MemoryStream()) {
        ReadQuotedPrintable(ms, bytes, 0, bytes.Length, true, true);
        Assert.AreEqual(expectedOutput, DataUtilities.GetUtf8String(ms.ToArray(), true));
      }
    }

    public void TestFailQuotedPrintable(string input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using (MemoryStream ms = new MemoryStream()) {
        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.Length, true, true);
          Assert.Fail("Should have failed");
        } catch (MessageDataException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    public void TestFailQuotedPrintableNonLenient(string input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using (MemoryStream ms = new MemoryStream()) {
        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.Length, false, false);
          Assert.Fail("Should have failed");
        } catch (MessageDataException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    public void TestQuotedPrintable(string input, int mode, string expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      StringBuilder sb = new StringBuilder();
      var enc = new QuotedPrintableEncoder(mode, false);
      enc.WriteToString(sb, bytes, 0, bytes.Length);
      Assert.AreEqual(expectedOutput, sb.ToString());
    }

    public void TestQuotedPrintable(string input, string a, string b, string c) {
      this.TestQuotedPrintable(input, 0, a);
      this.TestQuotedPrintable(input, 1, b);
      this.TestQuotedPrintable(input, 2, c);
    }

    public void TestQuotedPrintable(string input, string a) {
      this.TestQuotedPrintable(input, 0, a);
      this.TestQuotedPrintable(input, 1, a);
      this.TestQuotedPrintable(input, 2, a);
    }

    public string Repeat(string s, int count) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < count; ++i) {
        sb.Append(s);
      }
      return sb.ToString();
    }

    private void TestParseDomain(string str, string expected) {
      Assert.AreEqual(str.Length, HeaderParser.ParseDomain(str, 0, str.Length, null));
      Assert.AreEqual(expected, HeaderParserUtility.ParseDomain(str, 0, str.Length));
    }

    private void TestParseLocalPart(string str, string expected) {
      Assert.AreEqual(str.Length, HeaderParser.ParseLocalPart(str, 0, str.Length, null));
      Assert.AreEqual(expected, HeaderParserUtility.ParseLocalPart(str, 0, str.Length));
    }

    [Test]
    public void TestParseDomainAndLocalPart() {
      this.TestParseDomain("x", "x");
      this.TestParseLocalPart("x", "x");
      this.TestParseLocalPart("\"" + "\"", String.Empty);
      this.TestParseDomain("x.example", "x.example");
      this.TestParseLocalPart("x.example", "x.example");
      this.TestParseLocalPart("x.example\ud800\udc00.example.com", "x.example\ud800\udc00.example.com");
      this.TestParseDomain("x.example\ud800\udc00.example.com", "x.example\ud800\udc00.example.com");
      this.TestParseDomain("x.example.com", "x.example.com");
      this.TestParseLocalPart("x.example.com", "x.example.com");
      this.TestParseLocalPart("\"(not a comment)\"", "(not a comment)");
      this.TestParseLocalPart("(comment1) x (comment2)", "x");
      this.TestParseLocalPart("(comment1) example (comment2) . (comment3) com", "example.com");
      this.TestParseDomain("(comment1) x (comment2)", "x");
      this.TestParseDomain("(comment1) example (comment2) . (comment3) com", "example.com");
      this.TestParseDomain("(comment1) [x] (comment2)", "[x]");
      this.TestParseDomain("(comment1) [a.b.c.d] (comment2)", "[a.b.c.d]");
      this.TestParseDomain("[]", "[]");
      this.TestParseDomain("[a .\r\n b. c.d ]", "[a.b.c.d]");
    }

    public void TestWordWrapOne(string firstWord, string nextWords, string expected) {
      var ww = new WordWrapEncoder(firstWord);
      ww.AddString(nextWords);
      //Console.WriteLine(ww.ToString());
      Assert.AreEqual(expected, ww.ToString());
    }

    [Test]
    public void TestWordWrap() {
      this.TestWordWrapOne("Subject:", this.Repeat("xxxx ", 10) + "y", "Subject: " + this.Repeat("xxxx ", 10) + "y");
      this.TestWordWrapOne("Subject:", this.Repeat("xxxx ", 10), "Subject: " + this.Repeat("xxxx ", 9) + "xxxx");
    }

    [Test]
    public void TestHeaderFields() {
      string testString = "Joe P Customer <customer@example.com>, Jane W Customer <jane@example.com>";
      Assert.AreEqual(testString.Length,
                      HeaderParser.ParseMailboxList(testString, 0, testString.Length, null));
      try {
        new Message().SetHeader("from","\"a\r\nb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","\"a\rb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","\"a\r b\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","\"a\r\n b\" <x@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","\"a\r\n b\" <x@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","=?utf-8?q?=01?= <x@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","=?utf-8?q?=01?= <x@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","\"a\nb\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Message().SetHeader("from","\"a\0b\" <x@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    public static String ToString(byte[] array) {
      StringBuilder builder = new StringBuilder();
      bool first = true;
      builder.Append("[");
      foreach(byte v in array) {
        if (!first) {
          builder.Append(", ");
        }
        builder.Append(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture));
        first = false;
      }
      builder.Append("]");
      return builder.ToString();
    }

    public static void AssertEqual(byte[] expected, byte[] actual) {
      AssertEqual(expected, actual, String.Empty);
    }
    public static void AssertEqual(byte[] expected, byte[] actual, String msg) {
      if (expected.Length != actual.Length) {
        Assert.Fail(
          "\nexpected: "+ToString(expected)+"\n"+
          "\nwas:      "+ToString(actual)+"\n"+msg);
      }
      for (int i = 0;i<expected.Length; ++i) {
        if (expected[i]!=actual[i]) {
          Assert.Fail(
            "\nexpected: "+ToString(expected)+"\n"+
            "\nwas:      "+ToString(actual)+"\n"+msg);
        }
      }
    }

    internal static ITransform Transform(string str) {
      return new WrappedStream(new MemoryStream(DataUtilities.GetUtf8Bytes(str, true)));
    }
    internal static ITransform Transform(byte[] bytes) {
      return new WrappedStream(new MemoryStream(bytes));
    }

    internal static byte[] GetBytes(ITransform trans) {
      using(var ms = new MemoryStream()) {
        int c;
        while ((c = trans.ReadByte()) >= 0) {
          ms.WriteByte((byte)c);
        }
        return ms.ToArray();
      }
    }

    [Test]
    public void TestBase64() {
      AssertEqual(
        new byte[] { 0, 16, 1 },
        GetBytes(new Base64Transform(Transform("ABAB"),true)));
      AssertEqual(
        new byte[] { 0, 16, 1, 93 },
        GetBytes(new Base64Transform(Transform("ABABXX=="),true)));
      AssertEqual(
        new byte[] { 0, 16, 1, 93 },
        GetBytes(new Base64Transform(Transform("ABABXX==="),true)));
      AssertEqual(
        new byte[] { 0, 16, 1, 93 },
        GetBytes(new Base64Transform(Transform("ABABXX"),true)));
      AssertEqual(
        new byte[] { 169, 172, 241, 179, 7, 157, 114, 247, 235 },
        GetBytes(new Base64Transform(Transform("qazxswedcvfr"),true)));
      AssertEqual(
        new byte[] { 255, 239, 254, 103 },
        GetBytes(new Base64Transform(Transform("/+/+Zz=="),true)));
    }

    [Test]
    public void TestLz4() {
      Lz4.Decompress(NormalizationData.CombiningClasses);
    }

    [Test]
    public void TestGenerate() {
      List<string> msgids = new List<string>();
      // Tests whether unique Message IDs are generated for each message.
      for (var i = 0; i < 1000; ++i) {
        string msgtext=new Message().SetHeader("from","me@example.com").SetTextBody("Hello world.").Generate();
        if (!IsGoodAsciiMessageFormat(msgtext, false)) {
          Assert.Fail("Bad message format generated");
        }
        string msgid=new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(msgtext,true))).GetHeader("message-id");
        if (msgids.Contains(msgid)) {
          Assert.Fail(msgid);
        }
        msgids.Add(msgid);
      }
    }

    [Test]
    public void TestNewMessage() {
      Assert.IsTrue(new Message().ContentType != null);
    }

    [Test]
    public void TestMakeFilename() {
      Assert.AreEqual(
        "hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?hello.txt?="));
      Assert.AreEqual(
        "hello.txt",
        ContentDisposition.MakeFilename("=?utf-8?q?___hello.txt___?="));
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

    [Test]
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
    }

    public void TestRfc2231Extension(string mtype, string param, string expected) {
      var mt = MediaType.Parse(mtype);
      Assert.AreEqual(expected, mt.GetParameter(param));
    }

    public void SingleTestMediaTypeEncoding(string value, string expected) {
      MediaType mt = new MediaTypeBuilder("x", "y").SetParameter("z", value).ToMediaType();
      string topLevel = mt.TopLevelType;
      string sub = mt.SubType;
      var mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt.ToString() +
        "\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      using (MemoryStream ms = new MemoryStream(DataUtilities.GetUtf8Bytes(mtstring, true))) {
        var msg = new Message(ms);
        Assert.AreEqual(topLevel, msg.ContentType.TopLevelType);
        Assert.AreEqual(sub, msg.ContentType.SubType);
        Assert.AreEqual(value, msg.ContentType.GetParameter("z"), mt.ToString());
      }
    }

    [Test]
    public void TestArgumentValidation() {
      try {
        new Base64Encoder(false, false, false, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Base64Encoder(false,false,false,"xyz");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new WordWrapEncoder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      #if DEBUG
      try {
        new Tokener().RestoreState(-1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Tokener().RestoreState(1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      #endif
      try {
        MediaType.TextPlainAscii.GetParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MediaType.TextPlainAscii.GetParameter("");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        MediaType.Parse(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.AreEqual("application",new MediaTypeBuilder().TopLevelType);
      Assert.AreEqual("text",new MediaTypeBuilder(MediaType.TextPlainAscii).TopLevelType);
      Assert.AreEqual("plain",new MediaTypeBuilder(MediaType.TextPlainAscii).SubType);
      Assert.IsTrue(MediaType.TextPlainAscii.Equals(MediaType.Parse("text/plain; charset=us-ascii")));
      Assert.IsTrue(MediaType.TextPlainAscii.GetHashCode()==MediaType.Parse("text/plain; charset=us-ascii").GetHashCode());
    }

    [Test]
    public void TestMediaTypeBuilder() {
      MediaTypeBuilder builder;
      try {
        new MediaTypeBuilder(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      builder=new MediaTypeBuilder("text","plain");
      try {
        builder.SetTopLevelType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter(null,"v");
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
        builder.SetParameter("","v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetParameter("v",null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        builder.SetTopLevelType("");
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
        new MediaTypeBuilder().SetParameter("x",String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x;y","v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("x/y","v");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Charsets.GetCharset(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] { 0 }, -1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 2, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 0, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 0, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), null, 0, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] { 0 }, -1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 2, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 0, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 0, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] { 0 }, 1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new BEncodingStringTransform(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsTrue(MediaType.Parse("text/plain").IsText);
      Assert.IsTrue(MediaType.Parse("multipart/alternative").IsMultipart);
      Assert.AreEqual("example/x",MediaType.Parse("example/x ").TypeAndSubType);
      Assert.AreEqual("text/plain",MediaType.Parse("example/x, a=b").TypeAndSubType);
      Assert.AreEqual("example/x",MediaType.Parse("example/x ; a=b").TypeAndSubType);
      Assert.AreEqual("example/x",MediaType.Parse("example/x; a=b").TypeAndSubType);
      Assert.AreEqual("example/x",MediaType.Parse("example/x; a=b ").TypeAndSubType);
    }

    [Test]
    public void TestShiftJIS() {
      // Adapted from the public domain Gonk test cases
      byte[] bytes;
      ICharset charset=Charsets.GetCharset("shift_jis");
      bytes = new byte[] { 0x82, 0x58, 0x33, 0x41, 0x61, 0x33, 0x82, 0x60,
        0x82, 0x81, 0x33, 0xb1, 0xaf, 0x33, 0x83, 0x41,
        0x83, 0x96, 0x33, 0x82, 0xa0, 0x33, 0x93, 0xfa,
        0x33, 0x3a, 0x3c, 0x33, 0x81, 0x80, 0x81, 0x8e,
        0x33, 0x31, 0x82, 0x51, 0x41, 0x61, 0x82, 0x51,
        0x82, 0x60, 0x82, 0x81, 0x82, 0x51, 0xb1, 0xaf,
        0x82, 0x51, 0x83, 0x41, 0x83, 0x96, 0x82, 0x51,
        0x82, 0xa0, 0x82, 0x51, 0x93, 0xfa, 0x82, 0x51,
        0x3a, 0x3c, 0x82, 0x51, 0x81, 0x80, 0x81, 0x8e,
        0x82, 0x51 };
      var expected=("\uFF19\u0033\u0041\u0061\u0033\uFF21\uFF41\u0033\uFF71\uFF6F\u0033\u30A2\u30F6\u0033\u3042\u0033\u65E5\u0033\u003A\u003C\u0033\u00F7\u2103\u0033\u0031\uFF12\u0041\u0061\uFF12\uFF21\uFF41\uFF12\uFF71\uFF6F\uFF12\u30A2\u30F6\uFF12\u3042\uFF12\u65E5\uFF12\u003A\u003C\uFF12\u00F7\u2103\uFF12");
      Assert.AreEqual(expected, charset.GetString(Transform(bytes)));
    }

    [Test]
    public void TestIso2022JP() {
      byte[] bytes;
      ICharset charset=Charsets.GetCharset("iso-2022-jp");
      bytes = new byte[] { 0x20, 0x41, 0x61, 0x5c };
      Assert.AreEqual(" Aa\\",charset.GetString(Transform(bytes)));
      // Illegal byte in escape middle state
      bytes = new byte[] { 0x1b, 0x28, 0x47, 0x21, 0x41, 0x31, 0x5c };
      Assert.AreEqual("\ufffd\u0028\u0047!A1\\",charset.GetString(Transform(bytes)));
      // Katakana
      bytes = new byte[] { 0x1b, 0x28, 0x49, 0x21, 0x41, 0x31, 0x5c };
      Assert.AreEqual("\uff61\uff81\uff71\uff9c",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x28, 0x49, 0x20, 0x41, 0x61, 0x5c };
      Assert.AreEqual("\ufffd\uff81\ufffd\uff9c",charset.GetString(Transform(bytes)));
      // ASCII state via escape
      bytes = new byte[] { 0x1b, 0x28, 0x42, 0x20, 0x41, 0x61, 0x5c };
      Assert.AreEqual(" Aa\\",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x28, 0x4a, 0x20, 0x41, 0x61, 0x5c };
      Assert.AreEqual(" Aa\\",charset.GetString(Transform(bytes)));
      // JIS0208 state
      bytes = new byte[] { 0x1b, 0x24, 0x40, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23 };
      Assert.AreEqual("\u3000\u3001\u3002",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23 };
      Assert.AreEqual("\u3000\u3001\u3002",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x22, 0x0a, 0x21, 0x23 };
      Assert.AreEqual("\u3000\u3001\n!#",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x22, 0x1b, 0x28, 0x42, 0x21, 0x23 };
      Assert.AreEqual("\u3000\u3001!#",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x0a, 0x21, 0x23, 0x22 };
      Assert.AreEqual("\u3000\ufffd\u3002\ufffd",charset.GetString(Transform(bytes)));
      // Illegal state
      bytes = new byte[] { 0x1b, 0x24, 0x4f, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23 };
      Assert.AreEqual("\ufffd\u0024\u004f!!!\u0022!#",charset.GetString(Transform(bytes)));
      // JIS0212 state
      bytes = new byte[] { 0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x30, 0x22, 0x31 };
      Assert.AreEqual("\u02d8\u02c7\u00b8",
                      charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x30, 0x0a, 0x22, 0x31 };
      Assert.AreEqual("\u02d8\u02c7\n\u00221",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x30, 0x1b, 0x28, 0x42, 0x22, 0x31 };
      Assert.AreEqual("\u02d8\u02c7\u00221",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x0a, 0x22, 0x31, 0x23 };
      Assert.AreEqual("\u02d8\ufffd\u00b8\ufffd",charset.GetString(Transform(bytes)));
      // Illegal state
      bytes = new byte[] { 0x1b, 0x24, 0x28, 0x4f, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23 };
      Assert.AreEqual("\ufffd\u0024\u0028\u004f!!!\u0022!#",charset.GetString(Transform(bytes)));
      // Illegal state at end
      bytes = new byte[] { 0x41, 0x1b };
      Assert.AreEqual("A\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x41, 0x1b, 0x27 };
      Assert.AreEqual("A\ufffd'",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x41, 0x1b, 0x24 };
      Assert.AreEqual("A\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x41, 0x1b, 0x24, 0x28 };
      Assert.AreEqual("A\ufffd\u0028",charset.GetString(Transform(bytes)));
    }

    private static void AssertUtf8Equal(byte[] expected, byte[] actual) {
      Assert.AreEqual(DataUtilities.GetUtf8String(expected, true),
                      DataUtilities.GetUtf8String(actual, true));
    }

    private static string WrapHeader(string s) {
      return new WordWrapEncoder("").AddString(s).ToString();
    }

    private void TestDowngradeDSNOne(string expected, string actual) {
      Assert.AreEqual(expected, Message.DowngradeRecipientHeaderValue(actual));
      string dsn;
      string expectedDSN;
      byte[] bytes;
      byte[] expectedBytes;
      bool encap=(expected.StartsWith("=?"));
      dsn="X-Ignore: X\r\n\r\nOriginal-Recipient: "+actual+"\r\nFinal-Recipient: "+actual+"\r\nX-Ignore: Y\r\n\r\n";
      if (encap)
        expectedDSN="X-Ignore: X\r\n\r\n"+WrapHeader("Downgraded-Original-Recipient: "+expected)+
          "\r\n"+WrapHeader("Downgraded-Final-Recipient: "+expected)+"\r\nX-Ignore: Y\r\n\r\n";
      else {
        expectedDSN="X-Ignore: X\r\n\r\n"+WrapHeader("Original-Recipient: "+expected)+"\r\n"+WrapHeader("Final-Recipient: "+expected)+"\r\nX-Ignore: Y\r\n\r\n";
      }
      bytes = Message.DowngradeDeliveryStatus(DataUtilities.GetUtf8Bytes(dsn, true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
      dsn="X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\nOriginal-Recipient: "+actual+"\r\nFinal-Recipient: "+actual+"\r\nX-Ignore: Y\r\n\r\n";
      if (encap)
        expectedDSN="X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\n"+WrapHeader("Downgraded-Original-Recipient: "+expected)+
          "\r\n"+WrapHeader("Downgraded-Final-Recipient: "+expected)+"\r\nX-Ignore: Y\r\n\r\n";
      else {
        expectedDSN="X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\n"+WrapHeader("Original-Recipient: "+expected)+"\r\n"+WrapHeader("Final-Recipient: "+expected)+"\r\nX-Ignore: Y\r\n\r\n";
      }
      bytes = Message.DowngradeDeliveryStatus(DataUtilities.GetUtf8Bytes(dsn, true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
      dsn="X-Ignore: X\r\n\r\nOriginal-recipient : "+actual+"\r\nFinal-Recipient: "+actual+"\r\nX-Ignore: Y\r\n\r\n";
      if (encap)
        expectedDSN="X-Ignore: X\r\n\r\n"+WrapHeader("Downgraded-Original-Recipient: "+expected)+
          "\r\n"+WrapHeader("Downgraded-Final-Recipient: "+expected)+"\r\nX-Ignore: Y\r\n\r\n";
      else {
        expectedDSN="X-Ignore: X\r\n\r\n"+WrapHeader("Original-recipient : "+expected)+"\r\n"+WrapHeader("Final-Recipient: "+expected)+"\r\nX-Ignore: Y\r\n\r\n";
      }
      bytes = Message.DowngradeDeliveryStatus(DataUtilities.GetUtf8Bytes(dsn, true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
    }

    [Test]
    public void TestDowngradeDSN() {
      string hexstart = "\\x" + "{";
      TestDowngradeDSNOne(
        "utf-8; x@x.example",
        ("utf-8; x@x.example"));
      TestDowngradeDSNOne(
        "utf-8; x@x" + hexstart + "BE}.example",
        ("utf-8; x@x\u00be.example"));
      TestDowngradeDSNOne(
        "utf-8; x@x" + hexstart + "BE}" + hexstart + "FF20}.example",
        ("utf-8; x@x\u00be\uff20.example"));
      TestDowngradeDSNOne(
        "(=?utf-8?Q?=C2=BE?=) utf-8; x@x.example",
        ("(\u00be) utf-8; x@x.example"));
      TestDowngradeDSNOne(
        "(=?utf-8?Q?=C2=BE?=) rfc822; x@x.example",
        ("(\u00be) rfc822; x@x.example"));
      TestDowngradeDSNOne(
        "(=?utf-8?Q?=C2=BE?=) rfc822(=?utf-8?Q?=C2=BE?=); x@x.example",
        ("(\u00be) rfc822(\u00be); x@x.example"));
      TestDowngradeDSNOne(
        "(=?utf-8?Q?=C2=BE?=) utf-8(=?utf-8?Q?=C2=BE?=); x@x" + hexstart + "BE}" + hexstart + "FF20}.example",
        ("(\u00be) utf-8(\u00be); x@x\u00be\uff20.example"));
      TestDowngradeDSNOne(
        "=?utf-8?Q?=28=C2=BE=29_rfc822=3B_x=40x=C2=BE=EF=BC=A0=2Eexample?=",
        ("(\u00be) rfc822; x@x\u00be\uff20.example"));
    }

    [Test]
    public void TestLanguageTags() {
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-a-bb-x-y-z"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("0-xx-xx"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("9-xx-xx"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("a-xx-xx"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("x-xx-xx"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-US-u-islamcal"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("zh-CN-a-myext-x-private"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-a-myext-b-another"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("de-419-DE"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("a-DE"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("ar-a-aaa-b-bbb-a-ccc"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("qbb-us"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("zh-yue"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-us"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("e0-us"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-gb-1999"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-gb-1999-1998"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("en-gb-1999-1999"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-gb-oed"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("sr-Latn-RS"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("x-aaaaaaaaa-y-z"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("x-aaaaaaaa-y-z"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("a-b-x-y-z"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("a-bb-xx-yy-zz"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("a-bb-x-y-z"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("a-x-y-z"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("x-x-y-z"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("i-lojban"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("i-klingon"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("art-lojban"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("sgn-be-fr"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("no-bok"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("z-xx-xx"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-aaa-bbbb-x-xxx-yyy-zzz"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-aaa-bbbb-x-x-y-z"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("en-aaa-bbb"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("en-aaa-bbb-ccc"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-aaa-bbbb"));
      Assert.IsTrue(ParserUtility.IsValidLanguageTag("en-aaa-bbbb-cc"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("en-aaa-bbb-"));
      Assert.IsFalse(ParserUtility.IsValidLanguageTag("en-aaa-bbb-ccc-"));
    }

    [Test]
    public void TestMediaTypeEncoding() {
      TestMediaTypeRoundTrip("xy"+this.Repeat("\"",20)+"z");
      this.SingleTestMediaTypeEncoding("xyz", "x/y;z=xyz");
      this.SingleTestMediaTypeEncoding("xy z", "x/y;z=\"xy z\"");
      this.SingleTestMediaTypeEncoding("xy\u00a0z", "x/y;z*=utf-8''xy%C2%A0z");
      this.SingleTestMediaTypeEncoding("xy\ufffdz", "x/y;z*=utf-8''xy%C2z");
      this.SingleTestMediaTypeEncoding("xy" + this.Repeat("\ufffc", 50) + "z", "x/y;z*=utf-8''xy" + this.Repeat("%EF%BF%BD", 50) + "z");
      this.SingleTestMediaTypeEncoding("xy" + this.Repeat("\u00a0", 50) + "z", "x/y;z*=utf-8''xy" + this.Repeat("%C2%A0", 50) + "z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(" ",20)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(" ",50)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(" ",80)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(" ",150)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("\'",150)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("\"",150)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(":",20)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("%",20)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("%",150)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(":",150)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("@",20)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("@",150)+"z");
      Assert.AreEqual("2",MediaType.Parse("x/y;z=1;z*=utf-8''2").GetParameter("z"));
    }

    private static void TestMediaTypeRoundTrip(string str) {
      Assert.AreEqual(str,MediaType.Parse(new MediaTypeBuilder("x","y").SetParameter("z",str).ToString()).GetParameter("z"));
    }

    [Test]
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
      Assert.IsNull(MediaType.Parse("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz",null));
      Assert.IsNull(MediaType.Parse("text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz",null));
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

    [Test]
    public void TestDecode() {
      this.TestDecodeQuotedPrintable("test", "test");
      this.TestDecodeQuotedPrintable("te \tst", "te \tst");
      this.TestDecodeQuotedPrintable("te=20", "te ");
      this.TestDecodeQuotedPrintable("te=09", "te\t");
      this.TestDecodeQuotedPrintable("te ", "te");
      this.TestDecodeQuotedPrintable("te\t", "te");
      this.TestDecodeQuotedPrintable("te=61st", "teast");
      this.TestDecodeQuotedPrintable("te=3dst", "te=st");
      this.TestDecodeQuotedPrintable("te=c2=a0st", "te\u00a0st");
      this.TestDecodeQuotedPrintable("te=3Dst", "te=st");
      this.TestDecodeQuotedPrintable("te=0D=0Ast", "te\r\nst");
      this.TestDecodeQuotedPrintable("te=0Dst", "te\rst");
      this.TestDecodeQuotedPrintable("te=0Ast", "te\nst");
      this.TestDecodeQuotedPrintable("te=C2=A0st", "te\u00a0st");
      this.TestDecodeQuotedPrintable("te=3st", "te=3st");
      this.TestDecodeQuotedPrintable("te==C2=A0st", "te=\u00a0st");
      this.TestDecodeQuotedPrintable(this.Repeat("a", 100), this.Repeat("a", 100));
      this.TestDecodeQuotedPrintable("te\r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\rst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te=\r\nst", "test");
      this.TestDecodeQuotedPrintable("te=\rst", "test");
      this.TestDecodeQuotedPrintable("te=\nst", "test");
      this.TestDecodeQuotedPrintable("te=\r", "te");
      this.TestDecodeQuotedPrintable("te=\n", "te");
      this.TestDecodeQuotedPrintable("te=xy", "te=xy");
      this.TestDecodeQuotedPrintable("te\u000cst", "test");
      this.TestDecodeQuotedPrintable("te\u007fst", "test");
      this.TestDecodeQuotedPrintable("te\u00a0st", "test");
      this.TestDecodeQuotedPrintable("te==20", "te= ");
      this.TestDecodeQuotedPrintable("te===20", "te== ");
      this.TestDecodeQuotedPrintable("te==xy", "te==xy");
      // here, the first '=' starts a malformed sequence, so is
      // output as is; the second '=' starts a soft line break,
      // so is ignored
      this.TestDecodeQuotedPrintable("te==", "te=");
      this.TestDecodeQuotedPrintable("te==\r\nst", "te=st");
      this.TestDecodeQuotedPrintable("te=3", "te=3");
      this.TestDecodeQuotedPrintable("te \r\n", "te\r\n");
      this.TestDecodeQuotedPrintable("te \r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te w\r\nst", "te w\r\nst");
      this.TestDecodeQuotedPrintable("te =\r\nst", "te st");
      this.TestDecodeQuotedPrintable("te \t\r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\t \r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te \nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te \t\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\t \nst", "te\r\nst");
      this.TestFailQuotedPrintableNonLenient("te\rst");
      this.TestFailQuotedPrintableNonLenient("te\nst");
      this.TestFailQuotedPrintableNonLenient("te=\rst");
      this.TestFailQuotedPrintableNonLenient("te=\nst");
      this.TestFailQuotedPrintableNonLenient("te=\r");
      this.TestFailQuotedPrintableNonLenient("te=\n");
      this.TestFailQuotedPrintableNonLenient("te \rst");
      this.TestFailQuotedPrintableNonLenient("te \nst");
      this.TestFailQuotedPrintableNonLenient(this.Repeat("a", 77));
      this.TestFailQuotedPrintableNonLenient(this.Repeat("=7F", 26));
      this.TestFailQuotedPrintableNonLenient("aa\r\n" + this.Repeat("a", 77));
      this.TestFailQuotedPrintableNonLenient("aa\r\n" + this.Repeat("=7F", 26));
    }

    public void TestEncodedWordsPhrase(string expected, string input) {
      Assert.AreEqual(
        expected + " <test@example.com>",
        HeaderFieldParsers.GetParser("from").DecodeEncodedWords(input + " <test@example.com>"));
    }

    public void TestEncodedWordsOne(string expected, string input) {
      string par = "(";
      Assert.AreEqual(expected, Rfc2047.DecodeEncodedWords(input, 0, input.Length, EncodedWordContext.Unstructured));
      Assert.AreEqual(
        "(" + expected + ") en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords("(" + input + ") en"));
      Assert.AreEqual(
        " (" + expected + ") en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (" + input + ") en"));
      Assert.AreEqual(
        " " + par + "comment " + par + "cmt " + expected + ")comment) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (comment (cmt " + input + ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + "=?bad?= " + expected + ")comment) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (comment (=?bad?= " + input + ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + String.Empty + expected + ")comment) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (comment (" + input + ")comment) en"));
      Assert.AreEqual(
        " (" + expected + "()) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (" + input + "()) en"));
      Assert.AreEqual(
        " en (" + expected + ")",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" en (" + input + ")"));
      Assert.AreEqual(
        expected,
        HeaderFieldParsers.GetParser("subject").DecodeEncodedWords(input));
    }

    [Test]
    public void TestEncodedPhrase2() {
      Assert.AreEqual(
        "=?utf-8?Q?=28tes=C2=BEt=29_x=40x=2Eexample?=",
        HeaderFieldParsers.GetParser("subject").DowngradeFieldValue("(tes\u00bet) x@x.example"));
    }

    [Test]
    public void TestToFieldDowngrading() {
      string sep=", ";
      Assert.AreEqual(
        "x <x@example.com>" + sep + "\"X\" <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, \"X\" <y@example.com>"));
      Assert.AreEqual(
        "x <x@example.com>" + sep + "=?utf-8?Q?=C2=BE?= <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, \u00be <y@example.com>"));
      Assert.AreEqual(
        "x <x@example.com>" + sep + "=?utf-8?Q?=C2=BE?= <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, \"\u00be\" <y@example.com>"));
      Assert.AreEqual(
        "x <x@example.com>" + sep + "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, x\u00e1 x x\u00e1 <y@example.com>"));
      Assert.AreEqual(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=C3=A1y=40example=2Ecom?= :;",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("g: x@example.com, x\u00e1y@example.com;"));
      Assert.AreEqual(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=40=CC=80=2Eexample?= :;",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("g: x@example.com, x@\u0300.example;"));
      Assert.AreEqual(
        "g: x@example.com" + sep + "x@xn--e-ufa.example;",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("g: x@example.com, x@e\u00e1.example;"));
      Assert.AreEqual(
        "x <x@xn--e-ufa.example>",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x <x@e\u00e1.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@example.com>",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x\u00e1 x x\u00e1 <x@example.com>"));
      Assert.AreEqual(
        "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@xn--e-ufa.example>",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x\u00e1 x x\u00e1 <x@e\u00e1.example>"));
      Assert.AreEqual(
        "x =?utf-8?Q?x=C3=A1y=40example=2Ecom?= :;",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x <x\u00e1y@example.com>"));
    }

    private static string EncodeComment(string str) {
      return Rfc2047.EncodeComment(str, 0, str.Length);
    }

    [Test]
    public void TestCommentsToWords() {
      Assert.AreEqual("(=?utf-8?Q?x?=)", EncodeComment("(x)"));
      Assert.AreEqual("(=?utf-8?Q?xy?=)", EncodeComment("(x\\y)"));
      Assert.AreEqual("(=?utf-8?Q?x_y?=)", EncodeComment("(x\r\n y)"));
      Assert.AreEqual("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\u00a0)"));
      Assert.AreEqual("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\\\u00a0)"));
      Assert.AreEqual("(=?utf-8?Q?x?=())", EncodeComment("(x())"));
      Assert.AreEqual("(=?utf-8?Q?x?=()=?utf-8?Q?y?=)", EncodeComment("(x()y)"));
      Assert.AreEqual("(=?utf-8?Q?x?=(=?utf-8?Q?ab?=)=?utf-8?Q?y?=)", EncodeComment("(x(a\\b)y)"));
      Assert.AreEqual("()", EncodeComment("()"));
      Assert.AreEqual("(test) x@x.example", HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(test) x@x.example"));
      Assert.AreEqual(
        "(=?utf-8?Q?tes=C2=BEt?=) x@x.example",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(tes\u00bet) x@x.example"));
      Assert.AreEqual(
        "(=?utf-8?Q?tes=C2=BEt?=) en",
        HeaderFieldParsers.GetParser("content-language").DowngradeFieldValue("(tes\u00bet) en"));
      Assert.AreEqual(
        "(comment) Test <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Test <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Tes\u00bet <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Tes\u00bet Subject <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Test Sub\u00beject <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet Subject\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Test Sub\u00beject\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.AreEqual(
        "Test <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Test <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Tes\u00bet <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Tes\u00bet Subject <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Test Sub\u00beject <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Test Sub\u00beject\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" (comment) <x@x.example>"));
    }

    internal static bool IsGoodAsciiMessageFormat(string str, bool hasMessageType) {
      int lineLength = 0;
      int wordLength = 0;
      int index = 0;
      int endIndex = str.Length;
      bool headers = true;
      bool colon = false;
      bool hasNonWhiteSpace = false;
      bool startsWithSpace = false;
      bool hasLongWord = false;
      if (index == endIndex) {
        Console.WriteLine("Message is empty");
        return false;
      }
      while (index<endIndex) {
        char c = str[index];
        if (index == 0 && (c == 0x20 || c == 0x09)) {
          Console.WriteLine("Starts with whitespace");
          return false;
        }
        if (c >= 0x80) {
          Console.WriteLine("Non-ASCII character (0x {0:X2})",(int)c);
          return false;
        }
        if (c=='\r' && index+1<endIndex && str[index+1]=='\n') {
          index+=2;
          if (headers && lineLength == 0) {
            // Start of the body
            headers = false;
          } else if (headers && !hasNonWhiteSpace) {
            Console.WriteLine("Line has only whitespace");
            return false;
          }
          lineLength = 0;
          wordLength = 0;
          colon = false;
          hasNonWhiteSpace = false;
          hasLongWord = false;
          startsWithSpace = false;
          if (index<endIndex && (str[index]==' ' || str[index]=='\t')) {
            startsWithSpace = true;
          }
          continue;
        } else if (c=='\r' || c=='\n') {
          Console.WriteLine("Bare CR or bare LF");
          return false;
        }
        if (headers && c==':' && !colon && !startsWithSpace) {
          if (index + 1 >= endIndex) {
            Console.WriteLine("Colon at end");
            return false;
          }
          if (index == 0 || str[index-1]==0x20 || str[index-1]==0x09 || str[index-1]==0x0d) {
            Console.WriteLine("End of line, whitespace, or start of message before colon");
            return false;
          }
          if (str[index + 1]!=0x20) {
            string test = str.Substring(Math.Max(index + 2-30, 0), Math.Min(index + 2, 30));
            Console.WriteLine("No space after header name and colon: (0x {0:X2}) [" + test + "] " + (index));
            return false;
          }
          colon = true;
        }
        if (c=='\t'  || c==0x20) {
          ++lineLength;
          wordLength = 0;
        } else {
          ++lineLength;
          ++wordLength;
          hasNonWhiteSpace = true;
          hasLongWord|=(wordLength>77) || (lineLength == wordLength && wordLength>78);
        }
        if (c == 0) {
          Console.WriteLine("CTL in message (0x {0:X2})",(int)c);
          return false;
        }
        if (headers && (c == 0x7f || (c<0x20 && c != 0x09))) {
          Console.WriteLine("CTL in header (0x {0:X2})",(int)c);
          return false;
        }
        int maxLineLength = 998;
        if (!headers && (!hasLongWord && !hasMessageType)) {
          // Set max length for the body to 78 unless a line
          // contains a word so long that exceeding 78 characters
          // is unavoidable
          maxLineLength = 78;
        }
        if (lineLength>maxLineLength) {
          Console.WriteLine("Line length exceeded (" + maxLineLength + " " + (str.Substring(index-78, 78)) + ")");
          return false;
        }
        ++index;
      }
      return true;
    }

    private void TestParseCommentStrictCore(string input) {
      Assert.AreEqual(input.Length, HeaderParserUtility.ParseCommentStrict(input, 0, input.Length), input);
    }

    [Test]
    public void TestPercentEncoding() {
      Assert.AreEqual(
        "test\u00be",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("test%c2%be")));
      Assert.AreEqual(
        "tesA",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%41")));
      Assert.AreEqual(
        "tesa",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%61")));
      Assert.AreEqual(
        "tes\r\na",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%0d%0aa")));
      Assert.AreEqual(
        "tes\r\na",
        Charsets.Utf8.GetString(new QEncodingStringTransform("tes=0d=0aa")));
      Assert.AreEqual(
        "tes%xx",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%xx")));
      Assert.AreEqual(
        "tes%dxx",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%dxx")));
      Assert.AreEqual(
        "tes=dxx",
        Charsets.Utf8.GetString(new QEncodingStringTransform("tes=dxx")));
      Assert.AreEqual(
        "tes??x",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes\r\nx")));
    }

    [Test]
    public void TestParseCommentStrict() {
      TestParseCommentStrictCore("(y)");
      TestParseCommentStrictCore("(e\\y)");
      TestParseCommentStrictCore("(a(b)c)");
      TestParseCommentStrictCore("()");
      TestParseCommentStrictCore("(x)");
    }
    [Test]
    public void TestEncodedWordsReservedChars() {
      // Check decoding of encoded words containing reserved characters
      // such as specials and CTLs:
      // U + 007F, should not be directly representable
      this.TestEncodedWordsPhrase("=?utf-8?q?x_=7F?=","=?utf-8?q?x_=7F?=");
      // U + 0001, should not be directly representable
      this.TestEncodedWordsPhrase("=?utf-8?q?x_=01?=","=?utf-8?q?x_=01?=");
      // CR and LF, should not be directly representable
      this.TestEncodedWordsPhrase("=?utf-8?q?x_=0D=0A?=","=?utf-8?q?x_=0D=0A?=");
      // Parentheses
      this.TestEncodedWordsPhrase("\"x (y)\"","=?utf-8?q?x_=28y=29?=");
      // Colons and angle brackets
      this.TestEncodedWordsPhrase("\"x <y:z>\"","=?utf-8?q?x_=3Cy=3Az=3E?=");
      // Encoded word lookalikes
      this.TestEncodedWordsPhrase(
        "\"=?utf-8?q?xyz?=\"",
        "=?utf-8?q?=3D=3Futf-8=3Fq=3Fxyz=3F=3D?=");
      this.TestEncodedWordsPhrase(
        "\"=?utf-8?q?xyz?=\"",
        "=?utf-8?q?=3D=3Futf-8=3F?= =?utf-8?q?q=3Fxyz=3F=3D?=");
      // Already quoted material
      this.TestEncodedWordsPhrase(
        "me (x) \"x:y\"",
        "=?utf-8?q?me?= (x) \"x:y\"");
      // Already quoted material with a special
      this.TestEncodedWordsPhrase(
        "me \"x:y\"",
        "=?utf-8?q?me?= \"x:y\"");
    }

    [Test]
    public void TestEncodedWords() {
      string par = "(";
      this.TestEncodedWordsPhrase("(sss) y", "(sss) =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("xy", "=?us-ascii?q?x?= =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("=?bad1?= =?bad2?= =?bad3?=", "=?bad1?= =?bad2?= =?bad3?=");
      // quoted because one word was decoded
      this.TestEncodedWordsPhrase("\"y =?bad2?= =?bad3?=\"", "=?us-ascii?q?y?= =?bad2?= =?bad3?=");
      // quoted because one word was decoded
      this.TestEncodedWordsPhrase("\"=?bad1?= y =?bad3?=\"", "=?bad1?= =?us-ascii?q?y?= =?bad3?=");
      this.TestEncodedWordsPhrase("xy", "=?us-ascii?q?x?= =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(" xy", " =?us-ascii?q?x?= =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("xy (sss)", "=?us-ascii?q?x?= =?us-ascii?q?y?= (sss)");
      this.TestEncodedWordsPhrase("x (sss) y", "=?us-ascii?q?x?= (sss) =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("x (z) y", "=?us-ascii?q?x?= (=?utf-8?Q?z?=) =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(
        "=?us-ascii?q?x?=" + par + "sss)=?us-ascii?q?y?=",
        "=?us-ascii?q?x?=(sss)=?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(
        "=?us-ascii?q?x?=" + par + "z)=?us-ascii?q?y?=",
        "=?us-ascii?q?x?=(=?utf-8?Q?z?=)=?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(
        "=?us-ascii?q?x?=" + par + "z) y",
        "=?us-ascii?q?x?=(=?utf-8?Q?z?=) =?us-ascii?q?y?=");
      this.TestEncodedWordsOne("x y", "=?utf-8?Q?x_?= =?utf-8?Q?y?=");
      this.TestEncodedWordsOne("abcde abcde", "abcde abcde");
      this.TestEncodedWordsOne("abcde", "abcde");
      this.TestEncodedWordsOne("abcde", "=?utf-8?Q?abcde?=");
      this.TestEncodedWordsOne("=?utf-8?Q?abcde?=extra", "=?utf-8?Q?abcde?=extra");
      this.TestEncodedWordsOne("abcde ", "=?utf-8?Q?abcde?= ");
      this.TestEncodedWordsOne(" abcde", " =?utf-8?Q?abcde?=");
      this.TestEncodedWordsOne(" abcde", " =?utf-8?Q?abcde?=");
      this.TestEncodedWordsOne("ab\u00a0de", "=?utf-8?Q?ab=C2=A0de?=");
      this.TestEncodedWordsOne("xy", "=?utf-8?Q?x?= =?utf-8?Q?y?=");
      this.TestEncodedWordsOne("x y", "x =?utf-8?Q?y?=");
      this.TestEncodedWordsOne("x y", "x =?utf-8?Q?y?=");
      this.TestEncodedWordsOne("x y", "=?utf-8?Q?x?= y");
      this.TestEncodedWordsOne("x y", "=?utf-8?Q?x?= y");
      this.TestEncodedWordsOne("xy", "=?utf-8?Q?x?= =?utf-8?Q?y?=");
      this.TestEncodedWordsOne("abc de", "=?utf-8?Q?abc=20de?=");
      this.TestEncodedWordsOne("abc de", "=?utf-8?Q?abc_de?=");
      this.TestEncodedWordsOne("abc\ufffdde", "=?us-ascii?q?abc=90de?=");
      this.TestEncodedWordsOne("=?x-undefined?q?abcde?=", "=?x-undefined?q?abcde?=");
      this.TestEncodedWordsOne("=?utf-8?Q?" + this.Repeat("x", 200) + "?=", "=?utf-8?Q?" + this.Repeat("x", 200) + "?=");
      this.TestEncodedWordsPhrase("=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=", "=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=");
    }

    [Test]
    public void TestSetHeader() {
      Assert.AreEqual("my subject",new Message().SetHeader("comments","subject").SetHeader("subject","my subject").GetHeader("subject"));
    }

    public static string EscapeString(string str) {
      string hex="0123456789abcdef";
      StringBuilder sb = new StringBuilder();
      for (int i = 0;i<str.Length; ++i) {
        char c = str[i];
        if (c == 0x09) {
          sb.Append("\\t");
        } else if (c == 0x0d) {
          sb.Append("\\r");
        } else if (c == 0x0a) {
          sb.Append("\\n");
        } else if (c == 0x22) {
          sb.Append("\\\"");
        } else if (c == 0x5c) {
          sb.Append("\\\\");
        } else if (c<0x20 || c >= 0x7f) {
          sb.Append("\\u");
          sb.Append(hex[(c >> 12) & 15]);
          sb.Append(hex[(c >> 8) & 15]);
          sb.Append(hex[(c >> 4) & 15]);
          sb.Append(hex[(c) & 15]);
        } else {
          sb.Append(c);
        }
      }
      return sb.ToString();
    }

    [Test]
    public void TestEucJP() {
      byte[] bytes;
      ICharset charset=Charsets.GetCharset("euc-jp");
      bytes = new byte[] { 0x8e };
      Assert.AreEqual("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x8e, 0x21 };
      Assert.AreEqual("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x8e, 0x8e, 0xa1 };
      Assert.AreEqual("\ufffd\uff61",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x8f };
      Assert.AreEqual("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x8f, 0x21 };
      Assert.AreEqual("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x8f, 0xa1 };
      Assert.AreEqual("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x8f, 0xa1, 0x21 };
      Assert.AreEqual("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x90 };
      Assert.AreEqual("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x90, 0x21 };
      Assert.AreEqual("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0xa1 };
      Assert.AreEqual("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0xa1, 0xa1 };
      Assert.AreEqual("\u3000",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x90, 0xa1, 0xa1 };
      Assert.AreEqual("\ufffd\u3000",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0x90, 0xa1, 0xa1, 0xa1 };
      Assert.AreEqual("\ufffd\u3000\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] { 0xa1, 0x21 };
      Assert.AreEqual("\ufffd!",charset.GetString(Transform(bytes)));
      string result;
      bytes = new byte[] { 0x15, 0xf2, 0xbf, 0xdd, 0xd7, 0x13, 0xeb, 0xcf, 0x8e, 0xd6, 0x8f, 0xec, 0xe9, 0x8f, 0xd6, 0xe6, 0x8f, 0xd3, 0xa3, 0x8e, 0xd4, 0x66, 0x8f, 0xb9, 0xfc, 0x8e, 0xb0, 0x8f, 0xea, 0xd8, 0x29, 0x8e, 0xca, 0x8e, 0xd4, 0xc9, 0xb5, 0x1e, 0x09, 0x8e, 0xab, 0xc2, 0xc5, 0x8e, 0xa7, 0x8e, 0xb6, 0x3d, 0xe1, 0xd9, 0xb7, 0xd5, 0x7b, 0x05, 0xe6, 0xce, 0x1d, 0x8f, 0xbd, 0xbe, 0xd8, 0xae, 0x8e, 0xc3, 0x8f, 0xc1, 0xda, 0xd5, 0xbb, 0xb2, 0xa2, 0xcc, 0xd4, 0x42, 0x8e, 0xa2, 0xed, 0xd4, 0xc6, 0xe0, 0x8f, 0xe0, 0xd5, 0x8e, 0xd8, 0xb0, 0xc8, 0x8f, 0xa2, 0xb8, 0xb9, 0xf1, 0x8e, 0xb0, 0xd9, 0xc0, 0x13 };
      result = "\u0015\u9ba8\u6bbc\u0013\u8a85\uff96\u9ea8\u81f2\u7c67\uff94f\u5aba\uff70\u9b8a)\uff8a\uff94\u8b2c\u001e\u0009\uff6b\u59a5\uff67\uff76=\u75ca\u834a"+
      "{\u0005\u8004\u001d\u5fd1\u60bd\uff83\u6595\u5a9a\u65fa\u731bB\uff62\u8f33\u5948\u8ec1\uff98\u978d\u0384\u56fd\uff70\u62c8\u0013";
      Assert.AreEqual(result, (charset.GetString(Transform(bytes))));
    }

    [Test]
    public void TestNamedAddress() {
      Assert.AreEqual("\"Me \" <me@example.com>",new NamedAddress("Me ","me@example.com").ToString());
      Assert.AreEqual("\" Me\" <me@example.com>",new NamedAddress(" Me","me@example.com").ToString());
      try {
        new NamedAddress("",(string)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("",(Address)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("x at example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("x");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("x@");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address((string)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("a b@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("a b@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("ab.example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("ab@exa mple.example");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("ab@example.com addr");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("Me <me@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("Me\u00e0 <me@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("\"Me\" <me@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("\"Me\u00e0\" <me@example.com>");
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("Me <me@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("Me\u00e0 <me@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("\"Me\" <me@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("\"Me\u00e0\" <me@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new NamedAddress("Me <me@example.com>, Fred <fred@example.com>");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      Assert.IsFalse(new NamedAddress("x@example.com").IsGroup);
      Assert.AreEqual("x@example.com",new NamedAddress("x@example.com").Name);
      Assert.AreEqual("x@example.com",new NamedAddress("x@example.com").Address.ToString());
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
        new Address(null,"example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("local",null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address(Repeat("local",200),"example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("local=domain.example");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("local@");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address(Repeat("local",200)+"@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new Address("lo,cal@example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestMailbox() {
      var mbox="Me <@example.org,@example.net,@example.com:me@x.example>";
      var result = new NamedAddress(mbox);
      Assert.AreEqual("Me <me@x.example>",result.ToString());
    }

    [Test]
    [Timeout(5000)]
    public void TestHeaderParsing() {
      string tmp;
      tmp=" A Xxxxx: Yyy Zzz <x@x.example>;";
      Assert.AreEqual(tmp.Length, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
      // just a local part in address
      Assert.AreEqual(0,HeaderParser.ParseHeaderFrom("\"Me\" <1234>",0,11,null));
      tmp="<x@x.invalid>";
      Assert.AreEqual(tmp.Length, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
      tmp="<x y@x.invalid>";  // local part is not a dot-atom
      Assert.AreEqual(0, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
      tmp=" <x@x.invalid>";
      Assert.AreEqual(tmp.Length, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
      tmp="=?utf-8?q??=\r\n \r\nABC";
      Assert.AreEqual(tmp, Rfc2047.DecodeEncodedWords(tmp, 0, tmp.Length, EncodedWordContext.Unstructured));
      tmp="=?utf-8?q??=\r\n \r\n ABC";
      Assert.AreEqual(tmp, Rfc2047.DecodeEncodedWords(tmp, 0, tmp.Length, EncodedWordContext.Unstructured));
      // Group syntax
      tmp="G:;";
      Assert.AreEqual(tmp.Length, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
      tmp="G:a <x@x.example>;";
      Assert.AreEqual(tmp.Length, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
      tmp=" A Xxxxx: ;";
      Assert.AreEqual(tmp.Length, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
      tmp=" A Xxxxx: Yyy Zzz <x@x.example>, y@y.example, Ww <z@z.invalid>;";
      Assert.AreEqual(tmp.Length, HeaderParser.ParseHeaderTo(tmp, 0, tmp.Length, null));
    }

    [Test]
    public void TestEncode() {
      this.TestQuotedPrintable("test", "test");
      this.TestQuotedPrintable("te\u000cst", "te=0Cst");
      this.TestQuotedPrintable("te\u007Fst", "te=7Fst");
      this.TestQuotedPrintable("te ", "te=20");
      this.TestQuotedPrintable("te\t", "te=09");
      this.TestQuotedPrintable("te st", "te st");
      this.TestQuotedPrintable("te=st", "te=3Dst");
      this.TestQuotedPrintable("te\r\nst", "te=0D=0Ast", "te\r\nst", "te\r\nst");
      this.TestQuotedPrintable("te\rst", "te=0Dst", "te=0Dst", "te\r\nst");
      this.TestQuotedPrintable("te\nst", "te=0Ast", "te=0Ast", "te\r\nst");
      this.TestQuotedPrintable("te " + " " + "\r\nst", "te " + " " + "=0D=0Ast", "te =20\r\nst", "te =20\r\nst");
      this.TestQuotedPrintable("te \r\nst", "te =0D=0Ast", "te=20\r\nst", "te=20\r\nst");
      this.TestQuotedPrintable("te \t\r\nst", "te =09=0D=0Ast", "te =09\r\nst", "te =09\r\nst");
      this.TestQuotedPrintable("te\t\r\nst", "te=09=0D=0Ast", "te=09\r\nst", "te=09\r\nst");
      this.TestQuotedPrintable(this.Repeat("a", 75), this.Repeat("a", 75));
      this.TestQuotedPrintable(this.Repeat("a", 76), this.Repeat("a", 75) + "=\r\na");
      this.TestQuotedPrintable(this.Repeat("\u000c", 30), this.Repeat("=0C", 25) + "=\r\n" + this.Repeat("=0C", 5));
    }

    public static void Timeout(int duration, Action action) {
      string stackTrace = null;
      object stackTraceLock = new Object();
      System.Threading.Thread thread = new System.Threading.Thread(
        () => {
          try {
            action();
          } catch (Exception ex) {
            lock (stackTraceLock) {
              stackTrace = ex.GetType().FullName + "\n" + ex.Message + "\n" + ex.StackTrace;
              System.Threading.Monitor.PulseAll(stackTraceLock);
            }
          }
        });
      thread.Start();
      if (!thread.Join(duration)) {
        thread.Abort();
        string trace = null;
        lock (stackTraceLock) {
          while (stackTrace == null) {
            System.Threading.Monitor.Wait(stackTraceLock);
          }
          trace = stackTrace;
        }
        if (trace != null) {
          Assert.Fail(trace);
        }
      }
    }

    internal static bool IsRareMixerHeader(string hdrname) {
      return hdrname.Equals("content-identifier") || hdrname.Equals("x400-content-identifier") || hdrname.Equals("x400-content-return") || hdrname.Equals("x400-content-type") || hdrname.Equals("x400-mts-identifier") || hdrname.Equals("x400-originator") || hdrname.Equals("x400-received") || hdrname.Equals("x400-recipients") || hdrname.Equals("x400-trace") || hdrname.Equals("original-encoded-information-types") || hdrname.Equals("conversion") || hdrname.Equals("conversion-with-loss") || hdrname.Equals("dl-expansion-history") || hdrname.Equals("originator-return-address") ||
        hdrname.Equals("discarded-x400-mts-extensions") || hdrname.Equals("supersedes") || hdrname.Equals("expires") ||
        hdrname.Equals("content-return") ||
        hdrname.Equals("autoforwarded") || hdrname.Equals("generate-delivery-report") || hdrname.Equals("incomplete-copy") || hdrname.Equals("message-type") || hdrname.Equals("discarded-x400-ipms-extensions") || hdrname.Equals("autosubmitted") || hdrname.Equals("prevent-nondelivery-report") || hdrname.Equals("alternate-recipient") || hdrname.Equals("disclose-recipients");
    }

    public void TestUtf7One(string input, string expected) {
      Assert.AreEqual(expected, Charsets.GetCharset("utf-7").GetString(EncodingTest.Transform(input)));
    }

    [Test]
    public void TestUtf7() {
      TestUtf7One("\\","\ufffd");
      TestUtf7One("~","\ufffd");
      TestUtf7One("\u0001","\ufffd");
      TestUtf7One("\u007f","\ufffd");
      TestUtf7One("\r\n\t '!\"#'(),$-%@[]^&=<>;*_`{}./:|?","\r\n\t '!\"#'(),$-%@[]^&=<>;*_`{}./:|?");
      TestUtf7One("x+--","x+-");
      TestUtf7One("x+-y","x+y");
      // Illegal byte after plus
      TestUtf7One("+!","\ufffd!");
      TestUtf7One("+\n","\ufffd\n");
      TestUtf7One("+\u007f","\ufffd\ufffd");
      TestUtf7One("+","\ufffd");
      // Incomplete byte
      TestUtf7One("+D?","\ufffd?");
      TestUtf7One("+D\u007f","\ufffd\ufffd");
      TestUtf7One("+D","\ufffd");
      // Only one UTF-16 byte
      TestUtf7One("+DE?","\ufffd?");
      TestUtf7One("+DE","\ufffd");
      TestUtf7One("+DE\u007f","\ufffd\ufffd");
      // UTF-16 code unit
      TestUtf7One("+DEE?","\u0c41?");
      TestUtf7One("+DEE","\u0c41");
      TestUtf7One("+DEE\u007f","\u0c41\ufffd");
      // UTF-16 code unit (redundant pad bit)
      TestUtf7One("+DEF?","\u0c41\ufffd?");
      TestUtf7One("+DEF","\u0c41\ufffd");
      TestUtf7One("+DEF\u007f","\u0c41\ufffd\ufffd");
      // High surrogate code unit
      TestUtf7One("+2AA?","\ufffd?");
      TestUtf7One("+2AA","\ufffd");
      TestUtf7One("+2AA\u007f","\ufffd\ufffd");
      // Low surrogate code unit
      TestUtf7One("+3AA?","\ufffd?");
      TestUtf7One("+3AA","\ufffd");
      TestUtf7One("+3AA\u007f","\ufffd\ufffd");
      // Surrogate pair
      TestUtf7One("+2ADcAA?","\ud800\udc00?");
      TestUtf7One("+2ADcAA","\ud800\udc00");
      TestUtf7One("+2ADcAA\u007f","\ud800\udc00\ufffd");
      // High surrogate followed by surrogate pair
      TestUtf7One("+2ADYANwA?","\ufffd\ud800\udc00?");
      TestUtf7One("+2ADYANwA","\ufffd\ud800\udc00");
      TestUtf7One("+2ADYANwA\u007f","\ufffd\ud800\udc00\ufffd");
      // Two UTF-16 code units
      TestUtf7One("+AMAA4A?","\u00c0\u00e0?");
      TestUtf7One("+AMAA4A","\u00c0\u00e0");
      TestUtf7One("+AMAA4A-Next","\u00c0\u00e0Next");
      TestUtf7One("+AMAA4A!Next","\u00c0\u00e0!Next");
      TestUtf7One("+AMAA4A\u007f","\u00c0\u00e0\ufffd");
      // Two UTF-16 code units (redundant pad bit)
      TestUtf7One("+AMAA4B?","\u00c0\u00e0\ufffd?");
      TestUtf7One("+AMAA4B","\u00c0\u00e0\ufffd");
      TestUtf7One("+AMAA4B-Next","\u00c0\u00e0\ufffdNext");
      TestUtf7One("+AMAA4B!Next","\u00c0\u00e0\ufffd!Next");
      TestUtf7One("+AMAA4B\u007f","\u00c0\u00e0\ufffd\ufffd");
    }

    [Test]
    public void TestReceivedHeader() {
      IHeaderFieldParser parser=HeaderFieldParsers.GetParser("received");
      string test="from x.y.example by a.b.example; Thu, 31 Dec 2012 00:00:00 -0100";
      Assert.AreEqual(test.Length, parser.Parse(test, 0, test.Length, null));
    }

    internal static string ToBase16(byte[] bytes) {
      StringBuilder sb = new StringBuilder();
      string hex="0123456789ABCDEF";
      for (int i = 0;i<bytes.Length; ++i) {
        sb.Append(hex[(bytes[i]>>4) & 15]);
        sb.Append(hex[(bytes[i]) & 15]);
      }
      return sb.ToString();
    }

    internal static string Hash(string str) {
      using(var sha1 = new System.Security.Cryptography.SHA1Managed()) {
        return ToBase16(sha1.ComputeHash(DataUtilities.GetUtf8Bytes(str, true)));
      }
    }

    internal static bool HasNestedMessageType(Message message) {
      if (message.ContentType.TopLevelType.Equals("message")) {
        if (message.ContentType.SubType.Equals("global")) {
          return false;
        }
        if (message.ContentType.SubType.Equals("global-headers")) {
          return false;
        }
        if (message.ContentType.SubType.Equals("global-delivery-status")) {
          return false;
        }
        if (message.ContentType.SubType.Equals("global-disposition-notification")) {
          return false;
        }
        return true;
      }
      foreach(var part in message.Parts) {
        if (HasNestedMessageType(part)) {
          return true;
        }
      }
      return false;
    }

    [Test]
    public void TestBoundaryReading() {
      byte[] body;
      string messageStart="MIME-Version: 1.0\r\n";
      messageStart+="Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart+="Preamble\r\n";
      messageStart+="--b1\r\n";
      string message = messageStart;
      message+="Content-Type: text/plain\r\n\r\n";
      message+="Test\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      Message msg;
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart",msg.ContentType.TopLevelType);
      Assert.AreEqual("b1",msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("text",msg.Parts[0].ContentType.TopLevelType);
      Assert.AreEqual("Test",msg.Parts[0].BodyString);
      // Nested Multipart body part
      message = messageStart;
      message+="Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message+="\r\n--b2\r\n";
      message+="Content-Type: text/plain\r\n\r\n";
      message+="Test\r\n";
      message+="--b2--\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual(1, msg.Parts[0].Parts.Count);
      Assert.AreEqual("Test",msg.Parts[0].Parts[0].BodyString);
      // No headers in body part
      message = messageStart;
      message+="\r\n";
      message+="Test\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("Test",msg.Parts[0].BodyString);
      // No CRLF before first boundary
      message="MIME-Version: 1.0\r\n";
      message+="Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      message+="--b1\r\n";
      message+="Content-Type: text/plain\r\n\r\n";
      message+="Test\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("Test",msg.Parts[0].BodyString);
      // Base64 body part
      message = messageStart;
      message+="Content-Type: application/octet-stream\r\n";
      message+="Content-Transfer-Encoding: base64\r\n\r\n";
      message+="ABABXX==\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart",msg.ContentType.TopLevelType);
      Assert.AreEqual("b1",msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("application",msg.Parts[0].ContentType.TopLevelType);
      body = msg.Parts[0].GetBody();
      Assert.AreEqual(0, body[0]);
      Assert.AreEqual(16, body[1]);
      Assert.AreEqual(1, body[2]);
      Assert.AreEqual(93, body[3]);
      Assert.AreEqual(4, body.Length);
      // Base64 body part II
      message = messageStart;
      message+="Content-Type: application/octet-stream\r\n";
      message+="Content-Transfer-Encoding: base64\r\n\r\n";
      message+="ABABXX==\r\n\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart",msg.ContentType.TopLevelType);
      Assert.AreEqual("b1",msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual("application",msg.Parts[0].ContentType.TopLevelType);
      body = msg.Parts[0].GetBody();
      Assert.AreEqual(0, body[0]);
      Assert.AreEqual(16, body[1]);
      Assert.AreEqual(1, body[2]);
      Assert.AreEqual(93, body[3]);
      Assert.AreEqual(4, body.Length);
      // Base64 in nested body part
      message = messageStart;
      message+="Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message+="--b2\r\n";
      message+="Content-Type: application/octet-stream\r\n";
      message+="Content-Transfer-Encoding: base64\r\n\r\n";
      message+="ABABXX==\r\n";
      message+="--b2--\r\n\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual("multipart",msg.ContentType.TopLevelType);
      Assert.AreEqual("b1",msg.ContentType.GetParameter("boundary"));
      Assert.AreEqual(1, msg.Parts.Count);
      Message part = msg.Parts[0];
      Assert.AreEqual("application",part.Parts[0].ContentType.TopLevelType);
      body = part.Parts[0].GetBody();
      Assert.AreEqual(0, body[0]);
      Assert.AreEqual(16, body[1]);
      Assert.AreEqual(1, body[2]);
      Assert.AreEqual(93, body[3]);
      Assert.AreEqual(4, body.Length);
      // Nested Multipart body part II
      message = messageStart;
      message+="Content-Type: multipart/mixed; boundary=b2\r\n\r\n";
      message+="--b2\r\n";
      message+="Content-Type: text/plain\r\n\r\n";
      message+="Test\r\n";
      message+="--b2--\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
      Assert.AreEqual(1, msg.Parts.Count);
      Assert.AreEqual(1, msg.Parts[0].Parts.Count);
      Assert.AreEqual("Test",msg.Parts[0].Parts[0].BodyString);
    }
  }
}
