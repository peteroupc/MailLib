/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
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
          unlimitedLineLength ? -1 : 76);
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
      try {
        new Message().SetHeader("from","=?utf-8?q?=01?= <x@example.com");
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
      TestMediaTypeRoundTrip("xy"+this.Repeat("\"",150)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(":",20)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat(":",150)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("@",20)+"z");
      TestMediaTypeRoundTrip("xy"+this.Repeat("@",150)+"z");
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
        "text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz",
        "charset",
        "abiso-8859-1'en'xyz");
      this.TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz",
        "charset",
        "a biso-8859-1'en'xyz");
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
        HeaderFields.GetParser("from").DecodeEncodedWords(input + " <test@example.com>"));
    }

    public void TestEncodedWordsOne(string expected, string input) {
      string par = "(";
      Assert.AreEqual(expected, Rfc2047.DecodeEncodedWords(input, 0, input.Length, EncodedWordContext.Unstructured));
      Assert.AreEqual(
        "(" + expected + ") en",
        HeaderFields.GetParser("content-language").DecodeEncodedWords("(" + input + ") en"));
      Assert.AreEqual(
        " (" + expected + ") en",
        HeaderFields.GetParser("content-language").DecodeEncodedWords(" (" + input + ") en"));
      Assert.AreEqual(
        " " + par + "comment " + par + "cmt " + expected + ")comment) en",
        HeaderFields.GetParser("content-language").DecodeEncodedWords(" (comment (cmt " + input + ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + "=?bad?= " + expected + ")comment) en",
        HeaderFields.GetParser("content-language").DecodeEncodedWords(" (comment (=?bad?= " + input + ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + String.Empty + expected + ")comment) en",
        HeaderFields.GetParser("content-language").DecodeEncodedWords(" (comment (" + input + ")comment) en"));
      Assert.AreEqual(
        " (" + expected + "()) en",
        HeaderFields.GetParser("content-language").DecodeEncodedWords(" (" + input + "()) en"));
      Assert.AreEqual(
        " en (" + expected + ")",
        HeaderFields.GetParser("content-language").DecodeEncodedWords(" en (" + input + ")"));
      Assert.AreEqual(
        expected,
        HeaderFields.GetParser("subject").DecodeEncodedWords(input));
    }

    [Test]
    public void TestEncodedPhrase2() {
      Assert.AreEqual(
        "=?utf-8?Q?=28tes=C2=BEt=29_x=40x=2Eexample?=",
        HeaderFields.GetParser("subject").DowngradeFieldValue("(tes\u00bet) x@x.example"));
    }

    [Test]
    public void TestToFieldDowngrading() {
      string sep=", ";
      Assert.AreEqual(
        "x <x@example.com>" + sep + "\"X\" <y@example.com>",
        HeaderFields.GetParser("to").DowngradeFieldValue("x <x@example.com>, \"X\" <y@example.com>"));
      Assert.AreEqual(
        "x <x@example.com>" + sep + "=?utf-8?Q?=C2=BE?= <y@example.com>",
        HeaderFields.GetParser("to").DowngradeFieldValue("x <x@example.com>, \u00be <y@example.com>"));
      Assert.AreEqual(
        "x <x@example.com>" + sep + "=?utf-8?Q?=C2=BE?= <y@example.com>",
        HeaderFields.GetParser("to").DowngradeFieldValue("x <x@example.com>, \"\u00be\" <y@example.com>"));
      Assert.AreEqual(
        "x <x@example.com>" + sep + "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <y@example.com>",
        HeaderFields.GetParser("to").DowngradeFieldValue("x <x@example.com>, x\u00e1 x x\u00e1 <y@example.com>"));
      Assert.AreEqual(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=C3=A1y=40example=2Ecom?= :;",
        HeaderFields.GetParser("to").DowngradeFieldValue("g: x@example.com, x\u00e1y@example.com;"));
      Assert.AreEqual(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=40=CC=80=2Eexample?= :;",
        HeaderFields.GetParser("to").DowngradeFieldValue("g: x@example.com, x@\u0300.example;"));
      Assert.AreEqual(
        "g: x@example.com" + sep + "x@xn--e-ufa.example;",
        HeaderFields.GetParser("to").DowngradeFieldValue("g: x@example.com, x@e\u00e1.example;"));
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
      Assert.AreEqual("(test) x@x.example", HeaderFields.GetParser("from").DowngradeFieldValue("(test) x@x.example"));
      Assert.AreEqual(
        "(=?utf-8?Q?tes=C2=BEt?=) x@x.example",
        HeaderFields.GetParser("from").DowngradeFieldValue("(tes\u00bet) x@x.example"));
      Assert.AreEqual(
        "(=?utf-8?Q?tes=C2=BEt?=) en",
        HeaderFields.GetParser("content-language").DowngradeFieldValue("(tes\u00bet) en"));
      Assert.AreEqual(
        "(comment) Test <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) Test <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) Tes\u00bet <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) Tes\u00bet Subject <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) Test Sub\u00beject <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet Subject\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) \"Test Sub\u00beject\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.AreEqual(
        "Test <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("Test <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("Tes\u00bet <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("Tes\u00bet Subject <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("Test Sub\u00beject <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("\"Tes\u00bet\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("\"Test Sub\u00beject\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("\"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.AreEqual(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFields.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" (comment) <x@x.example>"));
    }

    internal static bool IsGoodAsciiOnlyAndGoodLineLength(string str, bool hasMessageType) {
      int lineLength = 0;
      int wordLength = 0;
      int index = 0;
      int endIndex = str.Length;
      bool headers = true;
      bool hasLongWord = false;
      while (index<endIndex) {
        char c = str[index];
        if (c >= 0x80) {
          Console.WriteLine("Non-ASCII character (0x {0:X2})",(int)c);
          return false;
        }
        if (c=='\r' && index+1<endIndex && str[index+1]=='\n') {
          index+=2;
          if (headers && lineLength == 0) {
            // Start of the body
            headers = false;
          }
          lineLength = 0;
          wordLength = 0;
          hasLongWord = false;
          continue;
        } else if (c=='\r' || c=='\n') {
          Console.WriteLine("Bare CR or bare LF");
          return false;
        }
        if (c=='\t'  || c==0x20) {
          ++lineLength;
          wordLength = 0;
        } else {
          ++lineLength;
          ++wordLength;
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
        "tes?xx",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%xx")));
      Assert.AreEqual(
        "tes?xx",
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
      Assert.IsFalse(new NamedAddress("x@example.com").IsGroup);
      Assert.AreEqual("x@example.com",new NamedAddress("x@example.com").Name);
      Assert.AreEqual("x@example.com",new NamedAddress("x@example.com").Address.ToString());
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

    [Test]
    public void TestReceivedHeader() {
      IHeaderFieldParser parser=HeaderFields.GetParser("received");
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
      string messageStart="MIME-Version: 1.0\r\n";
      messageStart+="Content-Type: multipart/mixed; boundary=b1\r\n\r\n";
      messageStart+="Preamble\r\n";
      messageStart+="--b1\r\n";
      string message = messageStart;
      message+="Content-Type: text/plain\r\n\r\n";
      message+="Test\r\n";
      message+="--b1--\r\n";
      message+="Epilogue";
      Message msg = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(message, true)));
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
