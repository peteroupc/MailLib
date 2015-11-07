/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO;
using PeterO.Mail;
using PeterO.Text;

namespace MailLibTest {
  [TestClass]
  public class EncodingTest {
    public static string MailNamespace() {
      return typeof(Message).Namespace;
    }

    public static string EscapeString(string str) {
      string hex = "0123456789abcdef";
      var sb = new StringBuilder();
      for (int i = 0; i < str.Length; ++i) {
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
        } else if (c < 0x20 || c >= 0x7f) {
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

    internal static bool IsGoodAsciiMessageFormat(string str, bool
      hasMessageType) {
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
      while (index < endIndex) {
        char c = str[index];
        if (index == 0 && (c == 0x20 || c == 0x09)) {
          Console.WriteLine("Starts with whitespace");
          return false;
        }
        if (c >= 0x80) {
          Console.WriteLine("Non-ASCII character (0x" + ToBase16(new[] {
            (byte)c }) + ")");
          return false;
        }
        if (c == '\r' && index + 1 < endIndex && str[index + 1] == '\n') {
          index += 2;
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
          startsWithSpace |= index < endIndex && (str[index] == ' ' ||
            str[index] == '\t');
          continue;
        }
        if (c == '\r' || c == '\n') {
          Console.WriteLine("Bare CR or bare LF");
          return false;
        }
        if (headers && c == ':' && !colon && !startsWithSpace) {
          if (index + 1 >= endIndex) {
            Console.WriteLine("Colon at end");
            return false;
          }
          if (index == 0 || str[index - 1] == 0x20 || str[index - 1] == 0x09||
            str[index - 1] == 0x0d) {
Console.WriteLine("End of line, whitespace, or start of message before colon");
            return false;
          }
          if (str[index + 1] != 0x20) {
            string test = str.Substring(Math.Max(index + 2 - 30, 0),
              Math.Min(index + 2, 30));
  Console.WriteLine("No space after header name and colon: (0x {0:X2}) [" +
              test + "] " + index);
            return false;
          }
          colon = true;
        }
        if (c == '\t' || c == 0x20) {
          ++lineLength;
          wordLength = 0;
        } else {
          ++lineLength;
          ++wordLength;
          hasNonWhiteSpace = true;
          hasLongWord |= (wordLength > 77) || (lineLength == wordLength &&
            wordLength > 78);
        }
        if (c == 0) {
   Console.WriteLine("CTL in message (0x" + ToBase16(new[] { (byte)c }) +
            ")");
          return false;
        }
        if (headers && (c == 0x7f || (c < 0x20 && c != 0x09))) {
    Console.WriteLine("CTL in header (0x" + ToBase16(new[] { (byte)c }) +
            ")");
          return false;
        }
        int maxLineLength = 998;
        if (!headers && (!hasLongWord && !hasMessageType)) {
          // Set max length for the body to 78 unless a line
          // contains a word so long that exceeding 78 characters
          // is unavoidable
          maxLineLength = 78;
        }
        if (lineLength > maxLineLength) {
          Console.WriteLine("Line length exceeded (" + maxLineLength + " " +
            (str.Substring(index - 78, 78)) + ")");
          return false;
        }
        ++index;
      }
      return true;
    }

    internal static string ToBase16(byte[] bytes) {
      var sb = new StringBuilder();
      string hex = "0123456789ABCDEF";
      for (int i = 0; i < bytes.Length; ++i) {
        sb.Append(hex[(bytes[i] >> 4) & 15]);
        sb.Append(hex[(bytes[i]) & 15]);
      }
      return sb.ToString();
    }

    public static String ToString(byte[] array) {
      var builder = new StringBuilder();
      bool first = true;
      builder.Append("[");
      foreach (byte v in array) {
        int vi = ((int)v) & 0xff;
        if (!first) {
          builder.Append(", ");
        }
        builder.Append(Convert.ToString((int)vi,
          System.Globalization.CultureInfo.InvariantCulture));
        first = false;
      }
      builder.Append("]");
      return builder.ToString();
    }

    public static void AssertEqual(byte[] expectedBytes, byte[] actualBytes) {
      AssertEqual(expectedBytes, actualBytes, String.Empty);
    }
    public static void AssertEqual(byte[] expectedBytes, byte[] actualBytes,
      String msg) {
      if (expectedBytes.Length != actualBytes.Length) {
        Assert.Fail("\nexpected: " + ToString(expectedBytes) + "\n" +
          "\nwas:      " + ToString(actualBytes) + "\n" + msg);
      }
      for (int i = 0; i < expectedBytes.Length; ++i) {
        if (expectedBytes[i] != actualBytes[i]) {
          Assert.Fail("\nexpected: " + ToString(expectedBytes) + "\n" +
            "\nwas:      " + ToString(actualBytes) + "\n" + msg);
        }
      }
    }

    public static string Repeat(string s, int count) {
      var sb = new StringBuilder();
      for (int i = 0; i < count; ++i) {
        sb.Append(s);
      }
      return sb.ToString();
    }

    // -----------------------------------------------------------

    private static void ReadQuotedPrintable(Stream outputStream,
      byte[] data, int offset,
      int count, bool lenientLineBreaks,
      bool unlimitedLineLength) {
      if (outputStream == null) {
        throw new ArgumentNullException("outputStream");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
    throw new ArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          data.Length);
      }
      if (count < 0) {
      throw new ArgumentException("count (" + count + ") is less than " +
          "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + count + ") is more than " +
          data.Length);
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" +
          (data.Length - offset) + ") is less than " + count);
      }
      var ms = new MemoryStream(data, offset, count);
      try {
    object t = Reflect.Construct(MailNamespace() +
          ".QuotedPrintableTransform" , Reflect.Construct(MailNamespace() +
            ".WrappedStream" , ms), lenientLineBreaks,
          unlimitedLineLength ? -1 : 76, false);
        object readByteMethod = Reflect.GetMethod(t, "ReadByte");
        readByteMethod = readByteMethod ?? (Reflect.GetMethod(t, "read"));
        while (true) {
          var c = (int)Reflect.InvokeMethod(t, readByteMethod);
          if (c < 0) {
            return;
          }
          outputStream.WriteByte((byte)c);
        }
      } catch (IOException ex) {
        Assert.Fail(ex.Message);
      }
    }

    public static void TestDecodeQuotedPrintable(string input, string
      expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using (var ms = new MemoryStream()) {
        ReadQuotedPrintable(ms, bytes, 0, bytes.Length, true, true);
        Assert.AreEqual(expectedOutput,
          DataUtilities.GetUtf8String(ms.ToArray(), true));
      }
    }

    public static void TestFailQuotedPrintable(string input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using (var ms = new MemoryStream()) {
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

    public static void TestFailQuotedPrintableNonLenient(string input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using (var ms = new MemoryStream()) {
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

    public static void TestQuotedPrintable(string input, int mode, string
      expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      var sb = new StringBuilder();
      object enc = Reflect.Construct(MailNamespace() +
        ".QuotedPrintableEncoder" , mode, false);
      Reflect.Invoke(enc, "WriteToString", sb, bytes, 0, bytes.Length);
      Assert.AreEqual(expectedOutput, sb.ToString());
    }

  public void TestQuotedPrintable(string input, string a, string b, string
      c) {
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, b);
      TestQuotedPrintable(input, 2, c);
    }

    public void TestQuotedPrintable(string input, string a) {
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, a);
      TestQuotedPrintable(input, 2, a);
    }

    private void TestParseDomain(string str, string expected) {
      Assert.IsTrue(str.Length == (int)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParser" , "ParseDomain" , str, 0, str.Length, null));
      Assert.AreEqual(expected, (string)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParserUtility" , "ParseDomain" , str, 0, str.Length));
    }

    private void TestParseLocalPart(string str, string expected) {
      Assert.IsTrue(str.Length == (int)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParser" , "ParseLocalPart" , str, 0, str.Length, null));
      Assert.AreEqual(expected, (string)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParserUtility" , "ParseLocalPart" , str, 0, str.Length));
    }

    [TestMethod]
    public void TestParseDomainAndLocalPart() {
      TestParseDomain("x", "x");
      TestParseLocalPart("x", "x");
      TestParseLocalPart("\"" + "\"", String.Empty);
      TestParseDomain("x.example", "x.example");
      TestParseLocalPart("x.example", "x.example");
      TestParseLocalPart("x.example\ud800\udc00.example.com",
        "x.example\ud800\udc00.example.com");
      TestParseDomain("x.example\ud800\udc00.example.com",
        "x.example\ud800\udc00.example.com");
      TestParseDomain("x.example.com", "x.example.com");
      TestParseLocalPart("x.example.com", "x.example.com");
      TestParseLocalPart("\"(not a comment)\"", "(not a comment)");
      TestParseLocalPart("(comment1) x (comment2)", "x");
      TestParseLocalPart("(comment1) example (comment2) . (comment3) com",
        "example.com");
      TestParseDomain("(comment1) x (comment2)", "x");
      TestParseDomain("(comment1) example (comment2) . (comment3) com",
        "example.com");
      TestParseDomain("(comment1) [x] (comment2)", "[x]");
      TestParseDomain("(comment1) [a.b.c.d] (comment2)", "[a.b.c.d]");
      TestParseDomain("[]", "[]");
      TestParseDomain("[a .\r\n b. c.d ]", "[a.b.c.d]");
    }

    public static void TestWordWrapOne(string firstWord, string nextWords,
      string expected) {
object ww = Reflect.Construct(MailNamespace() + ".WordWrapEncoder",
        firstWord);
      Reflect.Invoke(ww, "AddString", nextWords);
      Assert.AreEqual(expected, ww.ToString());
    }

    [TestMethod]
    public void TestWordWrap() {
      TestWordWrapOne("Subject:" , Repeat("xxxx " , 10) + "y" , "Subject: "+
        Repeat("xxxx " , 10) + "y");
      TestWordWrapOne("Subject:" , Repeat("xxxx " , 10), "Subject: " +
        Repeat("xxxx " , 9) + "xxxx");
    }

    [TestMethod]
    public void TestHeaderFields() {
      string testString =
  "Joe P Customer <customer@example.com>, Jane W Customer <jane@example.com>"
        ;
      if (testString.Length!=(int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseMailboxList" , testString, 0,
        testString.Length, null)) {
 Assert.Fail(testString);
}
    }

    [TestMethod]
    public void TestPunycodeDecode() {
      Assert.AreEqual(
  "e\u00e1",
  Reflect.InvokeStatic(typeof(Idna).Namespace + ".DomainUtility",
    "PunycodeDecode" , "xn--e-ufa" , 4, 9));
    }

    [TestMethod]
    public void TestAddressInternal() {
      try {
        Reflect.Construct(MailNamespace() + ".Address", null, "example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".Address", "local", null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".Address",
          EncodingTest.Repeat("local" , 200), "example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    internal static object Transform(string str) {
      return Reflect.Construct(MailNamespace() + ".WrappedStream" , new
        MemoryStream(DataUtilities.GetUtf8Bytes(str, true)));
    }
    internal static object Transform(byte[] bytes) {
      return Reflect.Construct(MailNamespace() + ".WrappedStream" , new
        MemoryStream(bytes));
    }

    internal static byte[] GetBytes(object trans) {
      object readByteMethod = Reflect.GetMethod(trans, "ReadByte");
      readByteMethod = readByteMethod ?? (Reflect.GetMethod(trans, "read"));
      using (var ms = new MemoryStream()) {
        int c = 0;
        while ((c = (int)Reflect.InvokeMethod(trans, readByteMethod)) >= 0) {
          ms.WriteByte((byte)c);
        }
        return ms.ToArray();
      }
    }

    [TestMethod]
    public void TestBase64() {
      AssertEqual(
        new byte[] { 0, 16, 1 }, GetBytes(Reflect.Construct(MailNamespace()+
          ".Base64Transform" , Transform("ABAB"), true)));
      AssertEqual(new byte[] { 0, 16, 1, 93 },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("ABABXX=="), true)));
      AssertEqual(
        new byte[] { 0, 16, 1, 93 },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("ABABXX==="), true)));
      AssertEqual(
        new byte[] { 0, 16, 1, 93 },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("ABABXX"), true)));
      AssertEqual(
        new byte[] { 169, 172, 241, 179, 7, 157, 114, 247, 235 },
        GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("qazxswedcvfr"), true)));
      AssertEqual(
        new byte[] { 255, 239, 254, 103 },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("/+/+Zz=="), true)));
    }
    [TestMethod]
    public void TestPercentEncoding() {
   object utf8 = Reflect.GetFieldStatic(MailNamespace() + ".Charsets",
        "Utf8");
      Assert.AreEqual(
        "test\u00be", (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "test%c2%be")));
      Assert.AreEqual("tesA",
        (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%41")));
      Assert.AreEqual("tesa",
        (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%61")));
      Assert.AreEqual("tes\r\na",
        (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%0d%0aa")));
      Assert.AreEqual("tes\r\na",
        (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() + ".QEncodingStringTransform",
          "tes=0d=0aa")));
      Assert.AreEqual(
        "tes%xx", (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%xx")));
      Assert.AreEqual("tes%dxx",
        (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%dxx")));
      Assert.AreEqual("tes=dxx",
        (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() + ".QEncodingStringTransform",
          "tes=dxx")));
      Assert.AreEqual(
        "tes??x", (string)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes\r\nx")));
    }

    [TestMethod]
    public void TestArgumentValidation() {
      try {
        Reflect.Construct(MailNamespace() + ".Base64Encoder" , false, false,
          false, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".Base64Encoder" , false, false,
          false, "xyz");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".WordWrapEncoder", (object)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        CharsetsTest.GetCharset(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      object encoder = Reflect.Construct(MailNamespace() + ".Base64Encoder"
        , false, false, false);
      try {
     Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), null, 0,
          1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, -1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 2, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 0, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 0, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      encoder = Reflect.Construct(MailNamespace() +
        ".QuotedPrintableEncoder" , 0, false);
      try {
     Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), null, 0,
          1);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, -1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 2, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 0, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 0, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new
          byte[] { 0 }, 1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
Reflect.Construct(MailNamespace() + ".BEncodingStringTransform",
          (object)null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    private static void AssertUtf8Equal(byte[] expected, byte[] actual) {
      Assert.AreEqual(DataUtilities.GetUtf8String(expected, true),
                    DataUtilities.GetUtf8String(actual, true));
    }

    private static string WrapHeader(string s) {
      return Reflect.Invoke(Reflect.Construct(MailNamespace() +
        ".WordWrapEncoder" , String.Empty), "AddString",
        s).ToString();
    }

    private void TestDowngradeDSNOne(string expected, string actual) {
      Assert.AreEqual(expected, (string)Reflect.InvokeStatic(MailNamespace()+
        ".Message" , "DowngradeRecipientHeaderValue" , actual));
      string dsn;
      string expectedDSN;
      byte[] bytes;
      byte[] expectedBytes;
      bool encap = (expected.StartsWith("=?", StringComparison.Ordinal));
      dsn = "X-Ignore: X\r\n\r\nOriginal-Recipient: " + actual +
        "\r\nFinal-Recipient: " + actual + "\r\nX-Ignore: Y\r\n\r\n" ;
      if (encap) expectedDSN = "X-Ignore: X\r\n\r\n" +
          WrapHeader("Downgraded-Original-Recipient: " + expected) +
          "\r\n" + WrapHeader("Downgraded-Final-Recipient: " + expected) +
            "\r\nX-Ignore: Y\r\n\r\n" ;
      else {
        expectedDSN = "X-Ignore: X\r\n\r\n" +
          WrapHeader("Original-Recipient: " + expected) + "\r\n" +
          WrapHeader("Final-Recipient: " + expected) +
          "\r\nX-Ignore: Y\r\n\r\n" ;
      }
      bytes = (byte[])Reflect.InvokeStatic(MailNamespace() + ".Message",
        "DowngradeDeliveryStatus" , DataUtilities.GetUtf8Bytes(dsn, true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
      dsn = "X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\nOriginal-Recipient: " +
        actual + "\r\nFinal-Recipient: " + actual +
        "\r\nX-Ignore: Y\r\n\r\n" ;
      if (encap)
        expectedDSN = "X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\n" +
          WrapHeader("Downgraded-Original-Recipient: " + expected) +
          "\r\n" + WrapHeader("Downgraded-Final-Recipient: " + expected) +
            "\r\nX-Ignore: Y\r\n\r\n" ;
      else {
        expectedDSN = "X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\n" +
          WrapHeader("Original-Recipient: " + expected) + "\r\n" +
          WrapHeader("Final-Recipient: " + expected) +
          "\r\nX-Ignore: Y\r\n\r\n" ;
      }
      bytes = (byte[])Reflect.InvokeStatic(MailNamespace() + ".Message",
        "DowngradeDeliveryStatus" , DataUtilities.GetUtf8Bytes(dsn, true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
      dsn = "X-Ignore: X\r\n\r\nOriginal-recipient : " + actual +
        "\r\nFinal-Recipient: " + actual + "\r\nX-Ignore: Y\r\n\r\n" ;
      if (encap) expectedDSN = "X-Ignore: X\r\n\r\n" +
          WrapHeader("Downgraded-Original-Recipient: " + expected) +
          "\r\n" + WrapHeader("Downgraded-Final-Recipient: " + expected) +
            "\r\nX-Ignore: Y\r\n\r\n" ;
      else {
        expectedDSN = "X-Ignore: X\r\n\r\n" +
          WrapHeader("Original-recipient : " + expected) + "\r\n" +
          WrapHeader("Final-Recipient: " + expected) +
          "\r\nX-Ignore: Y\r\n\r\n" ;
      }
      bytes = (byte[])Reflect.InvokeStatic(MailNamespace() + ".Message",
        "DowngradeDeliveryStatus" , DataUtilities.GetUtf8Bytes(dsn, true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
    }

    [TestMethod]
    public void TestDowngradeDSN() {
      string hexstart = "\\x" + "{";
      TestDowngradeDSNOne("utf-8; x@x.example",
        ("utf-8; x@x.example"));
      TestDowngradeDSNOne(
        "utf-8; x@x" + hexstart + "BE}.example",
        ("utf-8; x@x\u00be.example"));
      TestDowngradeDSNOne("utf-8; x@x" + hexstart + "BE}" + hexstart +
        "FF20}.example" , ("utf-8; x@x\u00be\uff20.example"));
      TestDowngradeDSNOne("(=?utf-8?Q?=C2=BE?=) utf-8; x@x.example",
        ("(\u00be) utf-8; x@x.example"));
      TestDowngradeDSNOne(
        "(=?utf-8?Q?=C2=BE?=) rfc822; x@x.example",
        ("(\u00be) rfc822; x@x.example"));
      TestDowngradeDSNOne(
        "(=?utf-8?Q?=C2=BE?=) rfc822(=?utf-8?Q?=C2=BE?=); x@x.example",
        ("(\u00be) rfc822(\u00be); x@x.example"));
      TestDowngradeDSNOne(
        "(=?utf-8?Q?=C2=BE?=) utf-8(=?utf-8?Q?=C2=BE?=); x@x" + hexstart + "BE}"+ hexstart + "FF20}.example",
        ("(\u00be) utf-8(\u00be); x@x\u00be\uff20.example"));
      TestDowngradeDSNOne(
        "=?utf-8?Q?=28=C2=BE=29_rfc822=3B_x=40x=C2=BE=EF=BC=A0=2Eexample?=",
        ("(\u00be) rfc822; x@x\u00be\uff20.example"));
    }

    [TestMethod]
    public void TestLanguageTags() {
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-a-bb-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "0-xx-xx"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "9-xx-xx"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-xx-xx"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-xx-xx"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-US-u-islamcal"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "zh-CN-a-myext-x-private"
));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-a-myext-b-another"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "de-419-DE"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-DE"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "ar-a-aaa-b-bbb-a-ccc"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "qbb-us"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "zh-yue"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-us"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "e0-us"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-1999"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-1999-1998"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-1999-1999"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-oed"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "sr-Latn-RS"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-aaaaaaaaa-y-z"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-aaaaaaaa-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-b-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-bb-xx-yy-zz"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-bb-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-x-y-z"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "i-lojban"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "i-klingon"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "art-lojban"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "sgn-be-fr"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "no-bok"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "z-xx-xx"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag",
        "en-aaa-bbbb-x-xxx-yyy-zzz"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbbb-x-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb-ccc"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbbb"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbbb-cc"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb-"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb-ccc-"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void TestDecode() {
      TestDecodeQuotedPrintable("test", "test");
      TestDecodeQuotedPrintable("te \tst", "te \tst");
      TestDecodeQuotedPrintable("te=20", "te ");
      TestDecodeQuotedPrintable("te=09", "te\t");
      TestDecodeQuotedPrintable("te ", "te");
      TestDecodeQuotedPrintable("te\t", "te");
      TestDecodeQuotedPrintable("te=61st", "teast");
      TestDecodeQuotedPrintable("te=3dst", "te=st");
      TestDecodeQuotedPrintable("te=c2=a0st", "te\u00a0st");
      TestDecodeQuotedPrintable("te=3Dst", "te=st");
      TestDecodeQuotedPrintable("te=0D=0Ast", "te\r\nst");
      TestDecodeQuotedPrintable("te=0Dst", "te\rst");
      TestDecodeQuotedPrintable("te=0Ast", "te\nst");
      TestDecodeQuotedPrintable("te=C2=A0st", "te\u00a0st");
      TestDecodeQuotedPrintable("te=3st", "te=3st");
      TestDecodeQuotedPrintable("te==C2=A0st", "te=\u00a0st");
      TestDecodeQuotedPrintable(Repeat("a", 100), Repeat("a", 100));
      TestDecodeQuotedPrintable("te\r\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te\rst", "te\r\nst");
      TestDecodeQuotedPrintable("te\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te=\r\nst", "test");
      TestDecodeQuotedPrintable("te=\rst", "test");
      TestDecodeQuotedPrintable("te=\nst", "test");
      TestDecodeQuotedPrintable("te=\r", "te");
      TestDecodeQuotedPrintable("te=\n", "te");
      TestDecodeQuotedPrintable("te=xy", "te=xy");
      TestDecodeQuotedPrintable("te\u000cst", "test");
      TestDecodeQuotedPrintable("te\u007fst", "test");
      TestDecodeQuotedPrintable("te\u00a0st", "test");
      TestDecodeQuotedPrintable("te==20", "te= ");
      TestDecodeQuotedPrintable("te===20", "te== ");
      TestDecodeQuotedPrintable("te==xy", "te==xy");
      // here, the first '=' starts a malformed sequence, so is
      // output as is; the second '=' starts a soft line break,
      // so is ignored
      TestDecodeQuotedPrintable("te==", "te=");
      TestDecodeQuotedPrintable("te==\r\nst", "te=st");
      TestDecodeQuotedPrintable("te=3", "te=3");
      TestDecodeQuotedPrintable("te \r\n", "te\r\n");
      TestDecodeQuotedPrintable("te \r\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te w\r\nst", "te w\r\nst");
      TestDecodeQuotedPrintable("te =\r\nst", "te st");
      TestDecodeQuotedPrintable("te \t\r\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te\t \r\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te \nst", "te\r\nst");
      TestDecodeQuotedPrintable("te \t\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te\t \nst", "te\r\nst");
      TestFailQuotedPrintableNonLenient("te\rst");
      TestFailQuotedPrintableNonLenient("te\nst");
      TestFailQuotedPrintableNonLenient("te=\rst");
      TestFailQuotedPrintableNonLenient("te=\nst");
      TestFailQuotedPrintableNonLenient("te=\r");
      TestFailQuotedPrintableNonLenient("te=\n");
      TestFailQuotedPrintableNonLenient("te \rst");
      TestFailQuotedPrintableNonLenient("te \nst");
      TestFailQuotedPrintableNonLenient(Repeat("a", 77));
      TestFailQuotedPrintableNonLenient(Repeat("=7F", 26));
      TestFailQuotedPrintableNonLenient("aa\r\n" + Repeat("a", 77));
      TestFailQuotedPrintableNonLenient("aa\r\n" + Repeat("=7F", 26));
    }

    public static void TestEncodedWordsPhrase(string expected, string input) {
      Assert.AreEqual(expected + " <test@example.com>",
        DecodeHeaderField("from", input + " <test@example.com>"));
    }

    public static void TestEncodedWordsOne(string expected, string input) {
      string par = "(";
      Assert.AreEqual(expected, Reflect.InvokeStatic(MailNamespace() +
        ".Rfc2047" , "DecodeEncodedWords" , input, 0, input.Length,
        Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
        "Unstructured")));
      Assert.AreEqual(
        "(" + expected + ") en",
        DecodeHeaderField("content-language" , "("+ input + ") en"));
      Assert.AreEqual(" (" + expected + ") en",
        DecodeHeaderField("content-language", " (" + input + ") en"));
      Assert.AreEqual(" " + par + "comment " + par + "cmt " + expected +
        ")comment) en",
        DecodeHeaderField("content-language" , " (comment (cmt " + input +
          ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + "=?bad?= " + expected + ")comment) en",
        DecodeHeaderField("content-language" , " (comment (=?bad?= " + input+
          ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + String.Empty + expected + ")comment) en",
DecodeHeaderField("content-language" , " (comment (" + input +
          ")comment) en"));
      Assert.AreEqual(
        " (" + expected + "()) en" , DecodeHeaderField("content-language",
          " (" + input + "()) en"));
      Assert.AreEqual(
        " en (" + expected + ")",
        DecodeHeaderField("content-language", " en (" + input + ")"));
      Assert.AreEqual(expected,
        DecodeHeaderField("subject", input));
    }

    [TestMethod]
    public void TestEncodedPhrase2() {
      Assert.AreEqual(
        "=?utf-8?Q?=28tes=C2=BEt=29_x=40x=2Eexample?=",
        DowngradeHeaderField("subject", "(tes\u00bet) x@x.example"));
    }

    [TestMethod]
    public void TestToFieldDowngrading() {
      string sep = ", ";
      Assert.AreEqual("x <x@example.com>" + sep + "\"X\" <y@example.com>",
        DowngradeHeaderField("to", "x <x@example.com>, \"X\" <y@example.com>"));
      Assert.AreEqual("x <x@example.com>" + sep +
        "=?utf-8?Q?=C2=BE?= <y@example.com>" , DowngradeHeaderField("to",
          "x <x@example.com>, \u00be <y@example.com>"));
      Assert.AreEqual("x <x@example.com>" + sep +
        "=?utf-8?Q?=C2=BE?= <y@example.com>",
  DowngradeHeaderField("to" , "x <x@example.com>, \"\u00be\" <y@example.com>"));
      Assert.AreEqual(
   "x <x@example.com>" + sep + "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <y@example.com>",
        DowngradeHeaderField("to",
          "x <x@example.com>, x\u00e1 x x\u00e1 <y@example.com>"));
      Assert.AreEqual(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=C3=A1y=40example=2Ecom?= :;",
        DowngradeHeaderField("to", "g: x@example.com, x\u00e1y@example.com;"));
 Assert.AreEqual(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=40=CC=80=2Eexample?= :;",
        DowngradeHeaderField("to", "g: x@example.com, x@\u0300.example;"));
      Assert.AreEqual("g: x@example.com" + sep + "x@xn--e-ufa.example;",
        DowngradeHeaderField("to", "g: x@example.com, x@e\u00e1.example;"));
      Assert.AreEqual("x <x@xn--e-ufa.example>",
        DowngradeHeaderField("sender", "x <x@e\u00e1.example>"));
      Assert.AreEqual("=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@example.com>",
        DowngradeHeaderField("sender", "x\u00e1 x x\u00e1 <x@example.com>"));
      Assert.AreEqual("=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@xn--e-ufa.example>",
      DowngradeHeaderField("sender" , "x\u00e1 x x\u00e1 <x@e\u00e1.example>"));
      Assert.AreEqual("x =?utf-8?Q?x=C3=A1y=40example=2Ecom?= :;",
        DowngradeHeaderField("sender", "x <x\u00e1y@example.com>"));
    }

    private static string EncodeComment(string str) {
      return (string)Reflect.InvokeStatic(MailNamespace() + ".Rfc2047",
        "EncodeComment" , str, 0, str.Length);
    }

    private static string DowngradeHeaderField(string name, string value) {
      return (string)Reflect.Invoke(Reflect.InvokeStatic(MailNamespace() +
        ".HeaderFieldParsers" , "GetParser" , name),
        "DowngradeFieldValue", value);
    }

    private static string DecodeHeaderField(string name, string value) {
      return (string)Reflect.Invoke(Reflect.InvokeStatic(MailNamespace() +
        ".HeaderFieldParsers" , "GetParser" , name),
        "DecodeEncodedWords", value);
    }

    [TestMethod]
    public void TestCommentsToWords() {
      Assert.AreEqual("(=?utf-8?Q?x?=)", EncodeComment("(x)"));
      Assert.AreEqual("(=?utf-8?Q?xy?=)", EncodeComment("(x\\y)"));
      Assert.AreEqual("(=?utf-8?Q?x_y?=)", EncodeComment("(x\r\n y)"));
      Assert.AreEqual("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\u00a0)"));
      Assert.AreEqual("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\\\u00a0)"));
      Assert.AreEqual("(=?utf-8?Q?x?=())", EncodeComment("(x())"));
    Assert.AreEqual("(=?utf-8?Q?x?=()=?utf-8?Q?y?=)",
        EncodeComment("(x()y)"));
      Assert.AreEqual("(=?utf-8?Q?x?=(=?utf-8?Q?ab?=)=?utf-8?Q?y?=)",
        EncodeComment("(x(a\\b)y)"));
      Assert.AreEqual("()", EncodeComment("()"));
      Assert.AreEqual("(test) x@x.example" , DowngradeHeaderField("from",
        "(test) x@x.example"));
      Assert.AreEqual(
        "(=?utf-8?Q?tes=C2=BEt?=) x@x.example",
        DowngradeHeaderField("from" , "(tes\u00bet) x@x.example"));
      Assert.AreEqual("(=?utf-8?Q?tes=C2=BEt?=) en",
        DowngradeHeaderField("content-language", "(tes\u00bet) en"));
      Assert.AreEqual("(comment) Test <x@x.example>",
        DowngradeHeaderField("from", "(comment) Test <x@x.example>"));
      Assert.AreEqual("(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "(comment) Tes\u00bet <x@x.example>"));
      Assert.AreEqual("(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
   DowngradeHeaderField("from" , "(comment) Tes\u00bet Subject <x@x.example>"));
      Assert.AreEqual("(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
   DowngradeHeaderField("from" , "(comment) Test Sub\u00beject <x@x.example>"));
      Assert.AreEqual("(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "(comment) \"Tes\u00bet\" <x@x.example>"));
      Assert.AreEqual("(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Tes\u00bet Subject\" <x@x.example>"));
      Assert.AreEqual("(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Test Sub\u00beject\" <x@x.example>"));
   Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.AreEqual(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        DowngradeHeaderField("from",
          "\"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.AreEqual("Test <x@x.example>",
        DowngradeHeaderField("from", "Test <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "Tes\u00bet <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        DowngradeHeaderField("from", "Tes\u00bet Subject <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        DowngradeHeaderField("from", "Test Sub\u00beject <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "\"Tes\u00bet\" <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        DowngradeHeaderField("from", "\"Tes\u00bet Subject\" <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        DowngradeHeaderField("from", "\"Test Sub\u00beject\" <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        DowngradeHeaderField("from", "\"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.AreEqual("=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        DowngradeHeaderField("from",
          "\"Tes\u00bet Subject\" (comment) <x@x.example>"));
    }

    private void TestParseCommentStrictCore(string input) {
      Assert.AreEqual(input.Length, Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParserUtility" , "ParseCommentStrict" , input, 0,
        input.Length), input);
    }

    [TestMethod]
    public void TestParseCommentStrict() {
      TestParseCommentStrictCore("(y)");
      TestParseCommentStrictCore("(e\\y)");
      TestParseCommentStrictCore("(a(b)c)");
      TestParseCommentStrictCore("()");
      TestParseCommentStrictCore("(x)");
    }
    [TestMethod]
    public void TestEncodedWordsReservedChars() {
      // Check decoding of encoded words containing reserved characters
      // such as specials and CTLs:
      // U + 007F, should not be directly representable
      TestEncodedWordsPhrase("=?utf-8?q?x_=7F?=", "=?utf-8?q?x_=7F?=");
      // U + 0001, should not be directly representable
      TestEncodedWordsPhrase("=?utf-8?q?x_=01?=", "=?utf-8?q?x_=01?=");
      // CR and LF, should not be directly representable
      TestEncodedWordsPhrase("=?utf-8?q?x_=0D=0A?=", "=?utf-8?q?x_=0D=0A?=");
      // Parentheses
      TestEncodedWordsPhrase("\"x (y)\"", "=?utf-8?q?x_=28y=29?=");
      // Colons and angle brackets
      TestEncodedWordsPhrase("\"x <y:z>\"", "=?utf-8?q?x_=3Cy=3Az=3E?=");
      // Encoded word lookalikes
      TestEncodedWordsPhrase(
        "\"=?utf-8?q?xyz?=\"",
        "=?utf-8?q?=3D=3Futf-8=3Fq=3Fxyz=3F=3D?=");
      TestEncodedWordsPhrase("\"=?utf-8?q?xyz?=\"",
        "=?utf-8?q?=3D=3Futf-8=3F?= =?utf-8?q?q=3Fxyz=3F=3D?=");
      // Already quoted material
      TestEncodedWordsPhrase(
        "me (x) \"x:y\"",
        "=?utf-8?q?me?= (x) \"x:y\"");
      // Already quoted material with a special
      TestEncodedWordsPhrase("me \"x:y\"",
        "=?utf-8?q?me?= \"x:y\"");
    }

    [TestMethod]
    public void TestEncodedWords() {
      string par = "(";
      TestEncodedWordsPhrase("(sss) y", "(sss) =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("xy", "=?us-ascii?q?x?= =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?bad1?= =?bad2?= =?bad3?=",
        "=?bad1?= =?bad2?= =?bad3?=");
      // quoted because one word was decoded
      TestEncodedWordsPhrase("\"y =?bad2?= =?bad3?=\"",
        "=?us-ascii?q?y?= =?bad2?= =?bad3?=");
      // quoted because one word was decoded
      TestEncodedWordsPhrase("\"=?bad1?= y =?bad3?=\"",
        "=?bad1?= =?us-ascii?q?y?= =?bad3?=");
      TestEncodedWordsPhrase("xy", "=?us-ascii?q?x?= =?us-ascii?q?y?=");
      TestEncodedWordsPhrase(" xy", " =?us-ascii?q?x?= =?us-ascii?q?y?=");
 TestEncodedWordsPhrase("xy (sss)" , "=?us-ascii?q?x?= =?us-ascii?q?y?= (sss)");
TestEncodedWordsPhrase("x (sss) y",
        "=?us-ascii?q?x?= (sss) =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("x (z) y",
        "=?us-ascii?q?x?= (=?utf-8?Q?z?=) =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?us-ascii?q?x?=" + par + "sss)=?us-ascii?q?y?=",
        "=?us-ascii?q?x?=(sss)=?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?us-ascii?q?x?=" + par + "z)=?us-ascii?q?y?=",
        "=?us-ascii?q?x?=(=?utf-8?Q?z?=)=?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?us-ascii?q?x?=" + par + "z) y",
        "=?us-ascii?q?x?=(=?utf-8?Q?z?=) =?us-ascii?q?y?=");
      TestEncodedWordsOne("x y", "=?utf-8?Q?x_?= =?utf-8?Q?y?=");
      TestEncodedWordsOne("abcde abcde", "abcde abcde");
      TestEncodedWordsOne("abcde", "abcde");
      TestEncodedWordsOne("abcde", "=?utf-8?Q?abcde?=");
      TestEncodedWordsOne("=?utf-8?Q?abcde?=extra", "=?utf-8?Q?abcde?=extra");
      TestEncodedWordsOne("abcde ", "=?utf-8?Q?abcde?= ");
      TestEncodedWordsOne(" abcde", " =?utf-8?Q?abcde?=");
      TestEncodedWordsOne(" abcde", " =?utf-8?Q?abcde?=");
      TestEncodedWordsOne("ab\u00a0de", "=?utf-8?Q?ab=C2=A0de?=");
      TestEncodedWordsOne("xy", "=?utf-8?Q?x?= =?utf-8?Q?y?=");
      TestEncodedWordsOne("x y", "x =?utf-8?Q?y?=");
      TestEncodedWordsOne("x y", "x =?utf-8?Q?y?=");
      TestEncodedWordsOne("x y", "=?utf-8?Q?x?= y");
      TestEncodedWordsOne("x y", "=?utf-8?Q?x?= y");
      TestEncodedWordsOne("xy", "=?utf-8?Q?x?= =?utf-8?Q?y?=");
      TestEncodedWordsOne("abc de", "=?utf-8?Q?abc=20de?=");
      TestEncodedWordsOne("abc de", "=?utf-8?Q?abc_de?=");
      TestEncodedWordsOne("abc\ufffdde", "=?us-ascii?q?abc=90de?=");
      TestEncodedWordsOne("=?x-undefined?q?abcde?=", "=?x-undefined?q?abcde?=");
      TestEncodedWordsOne("=?utf-8?Q?" + Repeat("x" , 200) + "?=",
        "=?utf-8?Q?" + Repeat("x" , 200) + "?=");
  TestEncodedWordsPhrase("=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=",
        "=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=");
    }

    [TestMethod]
    public void TestHeaderParsingRfc2047() {
      string tmp = "=?utf-8?q??=\r\n \r\nABC";
      Assert.AreEqual(tmp, (string)Reflect.InvokeStatic(MailNamespace() +
        ".Rfc2047" , "DecodeEncodedWords" , tmp, 0, tmp.Length,
        Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
        "Unstructured")));
      tmp = "=?utf-8?q??=\r\n \r\n ABC";
      Assert.AreEqual(tmp, (string)Reflect.InvokeStatic(MailNamespace() +
        ".Rfc2047" , "DecodeEncodedWords" , tmp, 0, tmp.Length,
        Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
        "Unstructured")));
    }

    [TestMethod]
    [Timeout(5000)]
    public void TestHeaderParsing() {
      string tmp;
      tmp = " A Xxxxx: Yyy Zzz <x@x.example>;";
      if (tmp.Length!=(int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
      // just a local part in address
      if (0!=(int)Reflect.InvokeStatic(MailNamespace() + ".HeaderParser",
        "ParseHeaderFrom" , "\"Me\" <1234>" , 0, 11, null)) {
 Assert.Fail(tmp);
}
      tmp = "<x@x.invalid>";
      if (tmp.Length!=(int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
      tmp = "<x y@x.invalid>";  // local part is not a dot-atom
      if (0!=(int)Reflect.InvokeStatic(MailNamespace() + ".HeaderParser",
        "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
      tmp = " <x@x.invalid>";
      if (tmp.Length!=(int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
      // Group syntax
      tmp = "G:;";
      if (tmp.Length!=(int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
      tmp = "G:a <x@x.example>;";
      if (tmp.Length!= (int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
      tmp = " A Xxxxx: ;";
      if (tmp.Length!= (int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
      tmp = " A Xxxxx: Yyy Zzz <x@x.example>, y@y.example, Ww <z@z.invalid>;";
      if (tmp.Length!=(int)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length, null)) {
 Assert.Fail(tmp);
}
    }

    [TestMethod]
    public void TestEncode() {
      TestQuotedPrintable("test", "test");
      TestQuotedPrintable("te\u000cst", "te=0Cst");
      TestQuotedPrintable("te\u007Fst", "te=7Fst");
      TestQuotedPrintable("te ", "te=20");
      TestQuotedPrintable("te\t", "te=09");
      TestQuotedPrintable("te st", "te st");
      TestQuotedPrintable("te=st", "te=3Dst");
      TestQuotedPrintable("te\r\nst", "te=0D=0Ast", "te\r\nst", "te\r\nst");
      TestQuotedPrintable("te\rst", "te=0Dst", "te=0Dst", "te\r\nst");
      TestQuotedPrintable("te\nst", "te=0Ast", "te=0Ast", "te\r\nst");
      TestQuotedPrintable("te " + " " + "\r\nst" , "te " + " " + "=0D=0Ast"
        , "te =20\r\nst" , "te =20\r\nst");
 TestQuotedPrintable("te \r\nst" , "te =0D=0Ast" , "te=20\r\nst",
        "te=20\r\nst");
      TestQuotedPrintable("te \t\r\nst" , "te =09=0D=0Ast" , "te =09\r\nst"
        , "te =09\r\nst");
      TestQuotedPrintable("te\t\r\nst" , "te=09=0D=0Ast" , "te=09\r\nst",
        "te=09\r\nst");
      TestQuotedPrintable(Repeat("a", 75), Repeat("a", 75));
      TestQuotedPrintable(Repeat("a", 76), Repeat("a", 75) + "=\r\na");
      TestQuotedPrintable(Repeat("\u000c" , 30), Repeat("=0C" , 25) +
        "=\r\n" + Repeat("=0C" , 5));
    }

    [TestMethod]
    public void TestReceivedHeader() {
      object parser = Reflect.InvokeStatic(MailNamespace() +
        ".HeaderFieldParsers" , "GetParser" , "received");
      string test =
        "from x.y.example by a.b.example; Thu, 31 Dec 2012 00:00:00 -0100" ;
      if (test.Length!=(int)Reflect.Invoke(parser, "Parse" , test, 0,
        test.Length, null)) {
        Assert.Fail(test);
      }
    }
  }
}
