/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text;

using NUnit.Framework;
using PeterO;
using PeterO.Mail;

namespace MailLibTest
{
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
          ms,
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
      var enc = new QuotedPrintableEncoder(mode);
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
    }

    [Test]
    public void TestCharset() {
      Assert.AreEqual("us-ascii", MediaType.Parse("text/plain").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("TEXT/PLAIN").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("TeXt/PlAiN").GetCharset());
      Assert.AreEqual("us-ascii", MediaType.Parse("text/xml").GetCharset());
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
    public void TestMediaTypeEncoding() {
      this.SingleTestMediaTypeEncoding("xyz", "x/y;z=xyz");
      this.SingleTestMediaTypeEncoding("xy z", "x/y;z=\"xy z\"");
      this.SingleTestMediaTypeEncoding("xy\u00a0z", "x/y;z*=utf-8''xy%C2%A0z");
      this.SingleTestMediaTypeEncoding("xy\ufffdz", "x/y;z*=utf-8''xy%C2z");
      this.SingleTestMediaTypeEncoding("xy" + this.Repeat("\ufffc", 50) + "z", "x/y;z*=utf-8''xy" + this.Repeat("%EF%BF%BD", 50) + "z");
      this.SingleTestMediaTypeEncoding("xy" + this.Repeat("\u00a0", 50) + "z", "x/y;z*=utf-8''xy" + this.Repeat("%C2%A0", 50) + "z");
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
      this.TestFailQuotedPrintable("te=3st");
      this.TestDecodeQuotedPrintable(this.Repeat("a", 100), this.Repeat("a", 100));
      this.TestDecodeQuotedPrintable("te\r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\rst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te=\r\nst", "test");
      this.TestDecodeQuotedPrintable("te=\rst", "test");
      this.TestDecodeQuotedPrintable("te=\nst", "test");
      this.TestDecodeQuotedPrintable("te=\r", "te");
      this.TestDecodeQuotedPrintable("te=\n", "te");
      this.TestFailQuotedPrintable("te=xy");
      this.TestFailQuotedPrintable("te\u000cst");
      this.TestFailQuotedPrintable("te\u007fst");
      this.TestFailQuotedPrintable("te\u00a0st");
      this.TestFailQuotedPrintable("te=3");
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

    private static string EncodeComment(string str){
      return Rfc2047.EncodeComment(str,0,str.Length);
    }
    
    [Test]
    public void TestCommentsToWords() {
      string par = "(";
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
        par + "tes\u00bet) x@x.example",
        HeaderFields.GetParser("subject").DowngradeFieldValue("(tes\u00bet) x@x.example"));
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

    private void TestParseCommentStrictCore(string input) {
      Assert.AreEqual(input.Length, HeaderParserUtility.ParseCommentStrict(input, 0, input.Length), input);
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
    public void TestEncodedWords() {
      string par = "(";
      this.TestEncodedWordsPhrase("(sss) y", "(sss) =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("xy", "=?us-ascii?q?x?= =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("=?bad1?= =?bad2?= =?bad3?=", "=?bad1?= =?bad2?= =?bad3?=");
      this.TestEncodedWordsPhrase("y =?bad2?= =?bad3?=", "=?us-ascii?q?y?= =?bad2?= =?bad3?=");
      this.TestEncodedWordsPhrase("=?bad1?= y =?bad3?=", "=?bad1?= =?us-ascii?q?y?= =?bad3?=");
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
    }

    [Test]
    public void TestNamedAddress() {
      Assert.AreEqual("\"Me \" <me@example.com>",new NamedAddress("Me ","me@example.com").ToString());
      Assert.AreEqual("\" Me\" <me@example.com>",new NamedAddress(" Me","me@example.com").ToString());
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
  }
}
