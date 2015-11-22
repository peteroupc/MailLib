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
          var builder = new StringBuilder();
          string hex = "0123456789ABCDEF";
          builder.Append("Non-ASCII character (0x");
          builder.Append(hex[((int)c >> 4) & 15]);
          builder.Append(hex[((int)c) & 15]);
          builder.Append(")");
          Console.WriteLine(builder.ToString());
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
          if (index == 0 || str[index - 1] == 0x20 || str[index - 1] == 0x09 ||
              str[index - 1] == 0x0d) {
         string
  wl="End of line, whitespace, or start of message before colon" ;
  Console.WriteLine(wl);
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
          var builder = new StringBuilder();
          string hex = "0123456789ABCDEF";
          builder.Append("CTL in message (0x");
          builder.Append(hex[((int)c >> 4) & 15]);
          builder.Append(hex[((int)c) & 15]);
          builder.Append(")");
          Console.WriteLine(builder.ToString());
          return false;
        }
        if (headers && (c == 0x7f || (c < 0x20 && c != 0x09))) {
          var builder = new StringBuilder();
          string hex = "0123456789ABCDEF";
          builder.Append("CTL in header (0x");
          builder.Append(hex[((int)c >> 4) & 15]);
          builder.Append(hex[((int)c) & 15]);
          builder.Append(")");
          Console.WriteLine(builder.ToString());
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

    private void TestParseDomain(string str, string expected) {
      Assert.IsTrue(str.Length == (int)Reflect.InvokeStatic(MailNamespace() +
                    ".HeaderParser", "ParseDomain", str, 0, str.Length, null));
      Assert.AreEqual(expected, (string)Reflect.InvokeStatic(MailNamespace() +
                    ".HeaderParserUtility", "ParseDomain", str, 0, str.Length));
    }

    private void TestParseLocalPart(string str, string expected) {
      Assert.IsTrue(str.Length == (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseLocalPart" , str, 0, str.Length,
                    null));
      Assert.AreEqual(expected, (string)Reflect.InvokeStatic(MailNamespace() +
                ".HeaderParserUtility" , "ParseLocalPart" , str, 0,
                    str.Length));
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
      TestWordWrapOne("Subject:", Repeat("xxxx ", 10) + "y", "Subject: " +
                    Repeat("xxxx ", 10) + "y");
      TestWordWrapOne("Subject:", Repeat("xxxx ", 10), "Subject: " +
                    Repeat("xxxx ", 9) + "xxxx");
    }

    [TestMethod]
    public void TestHeaderFields() {
      string testString =

  "Joe P Customer <customer@example.com>, Jane W Customer <jane@example.com>"
        ;
      if (testString.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                    ".HeaderParser", "ParseMailboxList", testString, 0,
                    testString.Length, null)) {
        Assert.Fail(testString);
      }
    }

    [TestMethod]
    public void TestPunycodeDecode() {
      {
        var stringTemp = (string)Reflect.InvokeStatic(typeof(Idna).Namespace +
                    ".DomainUtility",
                    "PunycodeDecode", "xn--e-ufa", 4, 9);
        Assert.AreEqual(
          "e\u00e1",
          stringTemp);
      }
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
                    EncodingTest.Repeat("local", 200), "example.com");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    internal static object Transform(string str) {
      return DataIO.ToTransform(DataUtilities.GetUtf8Bytes(str, true));
    }

    private void TestBase64Decode(byte[] expected, string input) {
      string msgString = "From: <test@example.com>\r\n" +
        "MIME-Version: 1.0\r\n" + "Content-Type: application/octet-stream\r\n" +
        "Content-Transfer-Encoding: base64\r\n\r\n" + input;
      Message msg = MessageTest.MessageFromString(msgString);
      AssertEqual(expected, msg.GetBody());
    }

    private void TestDecodeQuotedPrintable(string input, string expected) {
      string msgString = "From: <test@example.com>\r\n" +
        "MIME-Version: 1.0\r\n" + "Content-Type: application/octet-stream\r\n" +
        "Content-Transfer-Encoding: quoted-printable\r\n\r\n" + input;
      Message msg = MessageTest.MessageFromString(msgString);
      AssertEqual(DataUtilities.GetUtf8Bytes(expected, true), msg.GetBody());
    }

    public static void TestFailQuotedPrintableNonLenient(string input) {
      string msgString = "From: <test@example.com>\r\n" +
        "MIME-Version: 1.0\r\n" + "Content-Type: application/octet-stream\r\n" +
        "Content-Transfer-Encoding: quoted-printable\r\n\r\n" + input;
      try {
        MessageTest.MessageFromString(msgString);
      } catch (MessageDataException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [TestMethod]
    public void TestBase64() {
      TestBase64Decode(
        new byte[] { 0, 16, 1 }, "ABAB");
      TestBase64Decode(new byte[] { 0, 16, 1, 93 }, "ABABXX==");
      TestBase64Decode(
     new byte[] { 169, 172, 241, 179, 7, 157, 114, 247, 235 },
          "qazxswedcvfr");
      TestBase64Decode(
        new byte[] { 255, 239, 254, 103 }, "/+/+Zz==");
      TestBase64Decode(
        new byte[] { 0, 16, 1, 93 }, "ABABXX===");
      TestBase64Decode(
        new byte[] { 0, 16, 1, 93 }, "ABABXX");
      TestBase64Decode(new byte[] { 0 }, "AA==");
      TestBase64Decode(new byte[] { 1 }, "AQ==");
      TestBase64Decode(new byte[] { 2 }, "Ag==");
      TestBase64Decode(new byte[] { 3 }, "Aw==");
      TestBase64Decode(new byte[] { 4 }, "BA==");
      TestBase64Decode(new byte[] { 5 }, "BQ==");
      TestBase64Decode(new byte[] { 6 }, "Bg==");
      TestBase64Decode(new byte[] { 7 }, "Bw==");
      TestBase64Decode(new byte[] { 8 }, "CA==");
      TestBase64Decode(new byte[] { 9 }, "CQ==");
      TestBase64Decode(new byte[] { 10 }, "Cg==");
      TestBase64Decode(new byte[] { 11 }, "Cw==");
      TestBase64Decode(new byte[] { 12 }, "DA==");
      TestBase64Decode(new byte[] { 13 }, "DQ==");
      TestBase64Decode(new byte[] { 14 }, "Dg==");
      TestBase64Decode(new byte[] { 15 }, "Dw==");
      TestBase64Decode(new byte[] { 16 }, "EA==");
      TestBase64Decode(new byte[] { 17 }, "EQ==");
      TestBase64Decode(new byte[] { 18 }, "Eg==");
      TestBase64Decode(new byte[] { 19 }, "Ew==");
      TestBase64Decode(new byte[] { 20 }, "FA==");
      TestBase64Decode(new byte[] { 21 }, "FQ==");
      TestBase64Decode(new byte[] { 22 }, "Fg==");
      TestBase64Decode(new byte[] { 23 }, "Fw==");
      TestBase64Decode(new byte[] { 24 }, "GA==");
      TestBase64Decode(new byte[] { 25 }, "GQ==");
      TestBase64Decode(new byte[] { 26 }, "Gg==");
      TestBase64Decode(new byte[] { 27 }, "Gw==");
      TestBase64Decode(new byte[] { 28 }, "HA==");
      TestBase64Decode(new byte[] { 29 }, "HQ==");
      TestBase64Decode(new byte[] { 30 }, "Hg==");
      TestBase64Decode(new byte[] { 31 }, "Hw==");
      TestBase64Decode(new byte[] { 32 }, "IA==");
      TestBase64Decode(new byte[] { 33 }, "IQ==");
      TestBase64Decode(new byte[] { 34 }, "Ig==");
      TestBase64Decode(new byte[] { 35 }, "Iw==");
      TestBase64Decode(new byte[] { 36 }, "JA==");
      TestBase64Decode(new byte[] { 37 }, "JQ==");
      TestBase64Decode(new byte[] { 38 }, "Jg==");
      TestBase64Decode(new byte[] { 39 }, "Jw==");
      TestBase64Decode(new byte[] { 40 }, "KA==");
      TestBase64Decode(new byte[] { 41 }, "KQ==");
      TestBase64Decode(new byte[] { 42 }, "Kg==");
      TestBase64Decode(new byte[] { 43 }, "Kw==");
      TestBase64Decode(new byte[] { 44 }, "LA==");
      TestBase64Decode(new byte[] { 45 }, "LQ==");
      TestBase64Decode(new byte[] { 46 }, "Lg==");
      TestBase64Decode(new byte[] { 47 }, "Lw==");
      TestBase64Decode(new byte[] { 48 }, "MA==");
      TestBase64Decode(new byte[] { 49 }, "MQ==");
      TestBase64Decode(new byte[] { 50 }, "Mg==");
      TestBase64Decode(new byte[] { 51 }, "Mw==");
      TestBase64Decode(new byte[] { 52 }, "NA==");
      TestBase64Decode(new byte[] { 53 }, "NQ==");
      TestBase64Decode(new byte[] { 54 }, "Ng==");
      TestBase64Decode(new byte[] { 55 }, "Nw==");
      TestBase64Decode(new byte[] { 56 }, "OA==");
      TestBase64Decode(new byte[] { 57 }, "OQ==");
      TestBase64Decode(new byte[] { 58 }, "Og==");
      TestBase64Decode(new byte[] { 59 }, "Ow==");
      TestBase64Decode(new byte[] { 60 }, "PA==");
      TestBase64Decode(new byte[] { 61 }, "PQ==");
      TestBase64Decode(new byte[] { 62 }, "Pg==");
      TestBase64Decode(new byte[] { 63 }, "Pw==");
      TestBase64Decode(new byte[] { 64 }, "QA==");
      TestBase64Decode(new byte[] { 65 }, "QQ==");
      TestBase64Decode(new byte[] { 66 }, "Qg==");
      TestBase64Decode(new byte[] { 67 }, "Qw==");
      TestBase64Decode(new byte[] { 68 }, "RA==");
      TestBase64Decode(new byte[] { 69 }, "RQ==");
      TestBase64Decode(new byte[] { 70 }, "Rg==");
      TestBase64Decode(new byte[] { 71 }, "Rw==");
      TestBase64Decode(new byte[] { 72 }, "SA==");
      TestBase64Decode(new byte[] { 73 }, "SQ==");
      TestBase64Decode(new byte[] { 74 }, "Sg==");
      TestBase64Decode(new byte[] { 75 }, "Sw==");
      TestBase64Decode(new byte[] { 76 }, "TA==");
      TestBase64Decode(new byte[] { 77 }, "TQ==");
      TestBase64Decode(new byte[] { 78 }, "Tg==");
      TestBase64Decode(new byte[] { 79 }, "Tw==");
      TestBase64Decode(new byte[] { 80 }, "UA==");
      TestBase64Decode(new byte[] { 81 }, "UQ==");
      TestBase64Decode(new byte[] { 82 }, "Ug==");
      TestBase64Decode(new byte[] { 83 }, "Uw==");
      TestBase64Decode(new byte[] { 84 }, "VA==");
      TestBase64Decode(new byte[] { 85 }, "VQ==");
      TestBase64Decode(new byte[] { 86 }, "Vg==");
      TestBase64Decode(new byte[] { 87 }, "Vw==");
      TestBase64Decode(new byte[] { 88 }, "WA==");
      TestBase64Decode(new byte[] { 89 }, "WQ==");
      TestBase64Decode(new byte[] { 90 }, "Wg==");
      TestBase64Decode(new byte[] { 91 }, "Ww==");
      TestBase64Decode(new byte[] { 92 }, "XA==");
      TestBase64Decode(new byte[] { 93 }, "XQ==");
      TestBase64Decode(new byte[] { 94 }, "Xg==");
      TestBase64Decode(new byte[] { 95 }, "Xw==");
      TestBase64Decode(new byte[] { 96 }, "YA==");
      TestBase64Decode(new byte[] { 97 }, "YQ==");
      TestBase64Decode(new byte[] { 98 }, "Yg==");
      TestBase64Decode(new byte[] { 99 }, "Yw==");
      TestBase64Decode(new byte[] { 100 }, "ZA==");
      TestBase64Decode(new byte[] { 101 }, "ZQ==");
      TestBase64Decode(new byte[] { 102 }, "Zg==");
      TestBase64Decode(new byte[] { 103 }, "Zw==");
      TestBase64Decode(new byte[] { 104 }, "aA==");
      TestBase64Decode(new byte[] { 105 }, "aQ==");
      TestBase64Decode(new byte[] { 106 }, "ag==");
      TestBase64Decode(new byte[] { 107 }, "aw==");
      TestBase64Decode(new byte[] { 108 }, "bA==");
      TestBase64Decode(new byte[] { 109 }, "bQ==");
      TestBase64Decode(new byte[] { 110 }, "bg==");
      TestBase64Decode(new byte[] { 111 }, "bw==");
      TestBase64Decode(new byte[] { 112 }, "cA==");
      TestBase64Decode(new byte[] { 113 }, "cQ==");
      TestBase64Decode(new byte[] { 114 }, "cg==");
      TestBase64Decode(new byte[] { 115 }, "cw==");
      TestBase64Decode(new byte[] { 116 }, "dA==");
      TestBase64Decode(new byte[] { 117 }, "dQ==");
      TestBase64Decode(new byte[] { 118 }, "dg==");
      TestBase64Decode(new byte[] { 119 }, "dw==");
      TestBase64Decode(new byte[] { 120 }, "eA==");
      TestBase64Decode(new byte[] { 121 }, "eQ==");
      TestBase64Decode(new byte[] { 122 }, "eg==");
      TestBase64Decode(new byte[] { 123 }, "ew==");
      TestBase64Decode(new byte[] { 124 }, "fA==");
      TestBase64Decode(new byte[] { 125 }, "fQ==");
      TestBase64Decode(new byte[] { 126 }, "fg==");
      TestBase64Decode(new byte[] { 127 }, "fw==");
      TestBase64Decode(new byte[] { 128 }, "gA==");
      TestBase64Decode(new byte[] { 129 }, "gQ==");
      TestBase64Decode(new byte[] { 130 }, "gg==");
      TestBase64Decode(new byte[] { 131 }, "gw==");
      TestBase64Decode(new byte[] { 132 }, "hA==");
      TestBase64Decode(new byte[] { 133 }, "hQ==");
      TestBase64Decode(new byte[] { 134 }, "hg==");
      TestBase64Decode(new byte[] { 135 }, "hw==");
      TestBase64Decode(new byte[] { 136 }, "iA==");
      TestBase64Decode(new byte[] { 137 }, "iQ==");
      TestBase64Decode(new byte[] { 138 }, "ig==");
      TestBase64Decode(new byte[] { 139 }, "iw==");
      TestBase64Decode(new byte[] { 140 }, "jA==");
      TestBase64Decode(new byte[] { 141 }, "jQ==");
      TestBase64Decode(new byte[] { 142 }, "jg==");
      TestBase64Decode(new byte[] { 143 }, "jw==");
      TestBase64Decode(new byte[] { 144 }, "kA==");
      TestBase64Decode(new byte[] { 145 }, "kQ==");
      TestBase64Decode(new byte[] { 146 }, "kg==");
      TestBase64Decode(new byte[] { 147 }, "kw==");
      TestBase64Decode(new byte[] { 148 }, "lA==");
      TestBase64Decode(new byte[] { 149 }, "lQ==");
      TestBase64Decode(new byte[] { 150 }, "lg==");
      TestBase64Decode(new byte[] { 151 }, "lw==");
      TestBase64Decode(new byte[] { 152 }, "mA==");
      TestBase64Decode(new byte[] { 153 }, "mQ==");
      TestBase64Decode(new byte[] { 154 }, "mg==");
      TestBase64Decode(new byte[] { 155 }, "mw==");
      TestBase64Decode(new byte[] { 156 }, "nA==");
      TestBase64Decode(new byte[] { 157 }, "nQ==");
      TestBase64Decode(new byte[] { 158 }, "ng==");
      TestBase64Decode(new byte[] { 159 }, "nw==");
      TestBase64Decode(new byte[] { 160 }, "oA==");
      TestBase64Decode(new byte[] { 161 }, "oQ==");
      TestBase64Decode(new byte[] { 162 }, "og==");
      TestBase64Decode(new byte[] { 163 }, "ow==");
      TestBase64Decode(new byte[] { 164 }, "pA==");
      TestBase64Decode(new byte[] { 165 }, "pQ==");
      TestBase64Decode(new byte[] { 166 }, "pg==");
      TestBase64Decode(new byte[] { 167 }, "pw==");
      TestBase64Decode(new byte[] { 168 }, "qA==");
      TestBase64Decode(new byte[] { 169 }, "qQ==");
      TestBase64Decode(new byte[] { 170 }, "qg==");
      TestBase64Decode(new byte[] { 171 }, "qw==");
      TestBase64Decode(new byte[] { 172 }, "rA==");
      TestBase64Decode(new byte[] { 173 }, "rQ==");
      TestBase64Decode(new byte[] { 174 }, "rg==");
      TestBase64Decode(new byte[] { 175 }, "rw==");
      TestBase64Decode(new byte[] { 176 }, "sA==");
      TestBase64Decode(new byte[] { 177 }, "sQ==");
      TestBase64Decode(new byte[] { 178 }, "sg==");
      TestBase64Decode(new byte[] { 179 }, "sw==");
      TestBase64Decode(new byte[] { 180 }, "tA==");
      TestBase64Decode(new byte[] { 181 }, "tQ==");
      TestBase64Decode(new byte[] { 182 }, "tg==");
      TestBase64Decode(new byte[] { 183 }, "tw==");
      TestBase64Decode(new byte[] { 184 }, "uA==");
      TestBase64Decode(new byte[] { 185 }, "uQ==");
      TestBase64Decode(new byte[] { 186 }, "ug==");
      TestBase64Decode(new byte[] { 187 }, "uw==");
      TestBase64Decode(new byte[] { 188 }, "vA==");
      TestBase64Decode(new byte[] { 189 }, "vQ==");
      TestBase64Decode(new byte[] { 190 }, "vg==");
      TestBase64Decode(new byte[] { 191 }, "vw==");
      TestBase64Decode(new byte[] { 192 }, "wA==");
      TestBase64Decode(new byte[] { 193 }, "wQ==");
      TestBase64Decode(new byte[] { 194 }, "wg==");
      TestBase64Decode(new byte[] { 195 }, "ww==");
      TestBase64Decode(new byte[] { 196 }, "xA==");
      TestBase64Decode(new byte[] { 197 }, "xQ==");
      TestBase64Decode(new byte[] { 198 }, "xg==");
      TestBase64Decode(new byte[] { 199 }, "xw==");
      TestBase64Decode(new byte[] { 200 }, "yA==");
      TestBase64Decode(new byte[] { 201 }, "yQ==");
      TestBase64Decode(new byte[] { 202 }, "yg==");
      TestBase64Decode(new byte[] { 203 }, "yw==");
      TestBase64Decode(new byte[] { 204 }, "zA==");
      TestBase64Decode(new byte[] { 205 }, "zQ==");
      TestBase64Decode(new byte[] { 206 }, "zg==");
      TestBase64Decode(new byte[] { 207 }, "zw==");
      TestBase64Decode(new byte[] { 208 }, "0A==");
      TestBase64Decode(new byte[] { 209 }, "0Q==");
      TestBase64Decode(new byte[] { 210 }, "0g==");
      TestBase64Decode(new byte[] { 211 }, "0w==");
      TestBase64Decode(new byte[] { 212 }, "1A==");
      TestBase64Decode(new byte[] { 213 }, "1Q==");
      TestBase64Decode(new byte[] { 214 }, "1g==");
      TestBase64Decode(new byte[] { 215 }, "1w==");
      TestBase64Decode(new byte[] { 216 }, "2A==");
      TestBase64Decode(new byte[] { 217 }, "2Q==");
      TestBase64Decode(new byte[] { 218 }, "2g==");
      TestBase64Decode(new byte[] { 219 }, "2w==");
      TestBase64Decode(new byte[] { 220 }, "3A==");
      TestBase64Decode(new byte[] { 221 }, "3Q==");
      TestBase64Decode(new byte[] { 222 }, "3g==");
      TestBase64Decode(new byte[] { 223 }, "3w==");
      TestBase64Decode(new byte[] { 224 }, "4A==");
      TestBase64Decode(new byte[] { 225 }, "4Q==");
      TestBase64Decode(new byte[] { 226 }, "4g==");
      TestBase64Decode(new byte[] { 227 }, "4w==");
      TestBase64Decode(new byte[] { 228 }, "5A==");
      TestBase64Decode(new byte[] { 229 }, "5Q==");
      TestBase64Decode(new byte[] { 230 }, "5g==");
      TestBase64Decode(new byte[] { 231 }, "5w==");
      TestBase64Decode(new byte[] { 232 }, "6A==");
      TestBase64Decode(new byte[] { 233 }, "6Q==");
      TestBase64Decode(new byte[] { 234 }, "6g==");
      TestBase64Decode(new byte[] { 235 }, "6w==");
      TestBase64Decode(new byte[] { 236 }, "7A==");
      TestBase64Decode(new byte[] { 237 }, "7Q==");
      TestBase64Decode(new byte[] { 238 }, "7g==");
      TestBase64Decode(new byte[] { 239 }, "7w==");
      TestBase64Decode(new byte[] { 240 }, "8A==");
      TestBase64Decode(new byte[] { 241 }, "8Q==");
      TestBase64Decode(new byte[] { 242 }, "8g==");
      TestBase64Decode(new byte[] { 243 }, "8w==");
      TestBase64Decode(new byte[] { 244 }, "9A==");
      TestBase64Decode(new byte[] { 245 }, "9Q==");
      TestBase64Decode(new byte[] { 246 }, "9g==");
      TestBase64Decode(new byte[] { 247 }, "9w==");
      TestBase64Decode(new byte[] { 248 }, "+A==");
      TestBase64Decode(new byte[] { 249 }, "+Q==");
      TestBase64Decode(new byte[] { 250 }, "+g==");
      TestBase64Decode(new byte[] { 251 }, "+w==");
      TestBase64Decode(new byte[] { 252 }, "/A==");
      TestBase64Decode(new byte[] { 253 }, "/Q==");
      TestBase64Decode(new byte[] { 254 }, "/g==");
      TestBase64Decode(new byte[] { 255 }, "/w==");
      TestBase64Decode(new byte[] { 0, 2 }, "AAI=");
      TestBase64Decode(new byte[] { 1, 3 }, "AQM=");
      TestBase64Decode(new byte[] { 2, 4 }, "AgQ=");
      TestBase64Decode(new byte[] { 3, 5 }, "AwU=");
      TestBase64Decode(new byte[] { 4, 6 }, "BAY=");
      TestBase64Decode(new byte[] { 5, 7 }, "BQc=");
      TestBase64Decode(new byte[] { 6, 8 }, "Bgg=");
      TestBase64Decode(new byte[] { 7, 9 }, "Bwk=");
      TestBase64Decode(new byte[] { 8, 10 }, "CAo=");
      TestBase64Decode(new byte[] { 9, 11 }, "CQs=");
      TestBase64Decode(new byte[] { 10, 12 }, "Cgw=");
      TestBase64Decode(new byte[] { 11, 13 }, "Cw0=");
      TestBase64Decode(new byte[] { 12, 14 }, "DA4=");
      TestBase64Decode(new byte[] { 13, 15 }, "DQ8=");
      TestBase64Decode(new byte[] { 14, 16 }, "DhA=");
      TestBase64Decode(new byte[] { 15, 17 }, "DxE=");
      TestBase64Decode(new byte[] { 16, 18 }, "EBI=");
      TestBase64Decode(new byte[] { 17, 19 }, "ERM=");
      TestBase64Decode(new byte[] { 18, 20 }, "EhQ=");
      TestBase64Decode(new byte[] { 19, 21 }, "ExU=");
      TestBase64Decode(new byte[] { 20, 22 }, "FBY=");
      TestBase64Decode(new byte[] { 21, 23 }, "FRc=");
      TestBase64Decode(new byte[] { 22, 24 }, "Fhg=");
      TestBase64Decode(new byte[] { 23, 25 }, "Fxk=");
      TestBase64Decode(new byte[] { 24, 26 }, "GBo=");
      TestBase64Decode(new byte[] { 25, 27 }, "GRs=");
      TestBase64Decode(new byte[] { 26, 28 }, "Ghw=");
      TestBase64Decode(new byte[] { 27, 29 }, "Gx0=");
      TestBase64Decode(new byte[] { 28, 30 }, "HB4=");
      TestBase64Decode(new byte[] { 29, 31 }, "HR8=");
      TestBase64Decode(new byte[] { 30, 32 }, "HiA=");
      TestBase64Decode(new byte[] { 31, 33 }, "HyE=");
      TestBase64Decode(new byte[] { 32, 34 }, "ICI=");
      TestBase64Decode(new byte[] { 33, 35 }, "ISM=");
      TestBase64Decode(new byte[] { 34, 36 }, "IiQ=");
      TestBase64Decode(new byte[] { 35, 37 }, "IyU=");
      TestBase64Decode(new byte[] { 36, 38 }, "JCY=");
      TestBase64Decode(new byte[] { 37, 39 }, "JSc=");
      TestBase64Decode(new byte[] { 38, 40 }, "Jig=");
      TestBase64Decode(new byte[] { 39, 41 }, "Jyk=");
      TestBase64Decode(new byte[] { 40, 42 }, "KCo=");
      TestBase64Decode(new byte[] { 41, 43 }, "KSs=");
      TestBase64Decode(new byte[] { 42, 44 }, "Kiw=");
      TestBase64Decode(new byte[] { 43, 45 }, "Ky0=");
      TestBase64Decode(new byte[] { 44, 46 }, "LC4=");
      TestBase64Decode(new byte[] { 45, 47 }, "LS8=");
      TestBase64Decode(new byte[] { 46, 48 }, "LjA=");
      TestBase64Decode(new byte[] { 47, 49 }, "LzE=");
      TestBase64Decode(new byte[] { 48, 50 }, "MDI=");
      TestBase64Decode(new byte[] { 49, 51 }, "MTM=");
      TestBase64Decode(new byte[] { 50, 52 }, "MjQ=");
      TestBase64Decode(new byte[] { 51, 53 }, "MzU=");
      TestBase64Decode(new byte[] { 52, 54 }, "NDY=");
      TestBase64Decode(new byte[] { 53, 55 }, "NTc=");
      TestBase64Decode(new byte[] { 54, 56 }, "Njg=");
      TestBase64Decode(new byte[] { 55, 57 }, "Nzk=");
      TestBase64Decode(new byte[] { 56, 58 }, "ODo=");
      TestBase64Decode(new byte[] { 57, 59 }, "OTs=");
      TestBase64Decode(new byte[] { 58, 60 }, "Ojw=");
      TestBase64Decode(new byte[] { 59, 61 }, "Oz0=");
      TestBase64Decode(new byte[] { 60, 62 }, "PD4=");
      TestBase64Decode(new byte[] { 61, 63 }, "PT8=");
      TestBase64Decode(new byte[] { 62, 64 }, "PkA=");
      TestBase64Decode(new byte[] { 63, 65 }, "P0E=");
      TestBase64Decode(new byte[] { 64, 66 }, "QEI=");
      TestBase64Decode(new byte[] { 65, 67 }, "QUM=");
      TestBase64Decode(new byte[] { 66, 68 }, "QkQ=");
      TestBase64Decode(new byte[] { 67, 69 }, "Q0U=");
      TestBase64Decode(new byte[] { 68, 70 }, "REY=");
      TestBase64Decode(new byte[] { 69, 71 }, "RUc=");
      TestBase64Decode(new byte[] { 70, 72 }, "Rkg=");
      TestBase64Decode(new byte[] { 71, 73 }, "R0k=");
      TestBase64Decode(new byte[] { 72, 74 }, "SEo=");
      TestBase64Decode(new byte[] { 73, 75 }, "SUs=");
      TestBase64Decode(new byte[] { 74, 76 }, "Skw=");
      TestBase64Decode(new byte[] { 75, 77 }, "S00=");
      TestBase64Decode(new byte[] { 76, 78 }, "TE4=");
      TestBase64Decode(new byte[] { 77, 79 }, "TU8=");
      TestBase64Decode(new byte[] { 78, 80 }, "TlA=");
      TestBase64Decode(new byte[] { 79, 81 }, "T1E=");
      TestBase64Decode(new byte[] { 80, 82 }, "UFI=");
      TestBase64Decode(new byte[] { 81, 83 }, "UVM=");
      TestBase64Decode(new byte[] { 82, 84 }, "UlQ=");
      TestBase64Decode(new byte[] { 83, 85 }, "U1U=");
      TestBase64Decode(new byte[] { 84, 86 }, "VFY=");
      TestBase64Decode(new byte[] { 85, 87 }, "VVc=");
      TestBase64Decode(new byte[] { 86, 88 }, "Vlg=");
      TestBase64Decode(new byte[] { 87, 89 }, "V1k=");
      TestBase64Decode(new byte[] { 88, 90 }, "WFo=");
      TestBase64Decode(new byte[] { 89, 91 }, "WVs=");
      TestBase64Decode(new byte[] { 90, 92 }, "Wlw=");
      TestBase64Decode(new byte[] { 91, 93 }, "W10=");
      TestBase64Decode(new byte[] { 92, 94 }, "XF4=");
      TestBase64Decode(new byte[] { 93, 95 }, "XV8=");
      TestBase64Decode(new byte[] { 94, 96 }, "XmA=");
      TestBase64Decode(new byte[] { 95, 97 }, "X2E=");
      TestBase64Decode(new byte[] { 96, 98 }, "YGI=");
      TestBase64Decode(new byte[] { 97, 99 }, "YWM=");
      TestBase64Decode(new byte[] { 98, 100 }, "YmQ=");
      TestBase64Decode(new byte[] { 99, 101 }, "Y2U=");
      TestBase64Decode(new byte[] { 100, 102 }, "ZGY=");
      TestBase64Decode(new byte[] { 101, 103 }, "ZWc=");
      TestBase64Decode(new byte[] { 102, 104 }, "Zmg=");
      TestBase64Decode(new byte[] { 103, 105 }, "Z2k=");
      TestBase64Decode(new byte[] { 104, 106 }, "aGo=");
      TestBase64Decode(new byte[] { 105, 107 }, "aWs=");
      TestBase64Decode(new byte[] { 106, 108 }, "amw=");
      TestBase64Decode(new byte[] { 107, 109 }, "a20=");
      TestBase64Decode(new byte[] { 108, 110 }, "bG4=");
      TestBase64Decode(new byte[] { 109, 111 }, "bW8=");
      TestBase64Decode(new byte[] { 110, 112 }, "bnA=");
      TestBase64Decode(new byte[] { 111, 113 }, "b3E=");
      TestBase64Decode(new byte[] { 112, 114 }, "cHI=");
      TestBase64Decode(new byte[] { 113, 115 }, "cXM=");
      TestBase64Decode(new byte[] { 114, 116 }, "cnQ=");
      TestBase64Decode(new byte[] { 115, 117 }, "c3U=");
      TestBase64Decode(new byte[] { 116, 118 }, "dHY=");
      TestBase64Decode(new byte[] { 117, 119 }, "dXc=");
      TestBase64Decode(new byte[] { 118, 120 }, "dng=");
      TestBase64Decode(new byte[] { 119, 121 }, "d3k=");
      TestBase64Decode(new byte[] { 120, 122 }, "eHo=");
      TestBase64Decode(new byte[] { 121, 123 }, "eXs=");
      TestBase64Decode(new byte[] { 122, 124 }, "enw=");
      TestBase64Decode(new byte[] { 123, 125 }, "e30=");
      TestBase64Decode(new byte[] { 124, 126 }, "fH4=");
      TestBase64Decode(new byte[] { 125, 127 }, "fX8=");
      TestBase64Decode(new byte[] { 126, 128 }, "foA=");
      TestBase64Decode(new byte[] { 127, 129 }, "f4E=");
      TestBase64Decode(new byte[] { 128, 130 }, "gII=");
      TestBase64Decode(new byte[] { 129, 131 }, "gYM=");
      TestBase64Decode(new byte[] { 130, 132 }, "goQ=");
      TestBase64Decode(new byte[] { 131, 133 }, "g4U=");
      TestBase64Decode(new byte[] { 132, 134 }, "hIY=");
      TestBase64Decode(new byte[] { 133, 135 }, "hYc=");
      TestBase64Decode(new byte[] { 134, 136 }, "hog=");
      TestBase64Decode(new byte[] { 135, 137 }, "h4k=");
      TestBase64Decode(new byte[] { 136, 138 }, "iIo=");
      TestBase64Decode(new byte[] { 137, 139 }, "iYs=");
      TestBase64Decode(new byte[] { 138, 140 }, "iow=");
      TestBase64Decode(new byte[] { 139, 141 }, "i40=");
      TestBase64Decode(new byte[] { 140, 142 }, "jI4=");
      TestBase64Decode(new byte[] { 141, 143 }, "jY8=");
      TestBase64Decode(new byte[] { 142, 144 }, "jpA=");
      TestBase64Decode(new byte[] { 143, 145 }, "j5E=");
      TestBase64Decode(new byte[] { 144, 146 }, "kJI=");
      TestBase64Decode(new byte[] { 145, 147 }, "kZM=");
      TestBase64Decode(new byte[] { 146, 148 }, "kpQ=");
      TestBase64Decode(new byte[] { 147, 149 }, "k5U=");
      TestBase64Decode(new byte[] { 148, 150 }, "lJY=");
      TestBase64Decode(new byte[] { 149, 151 }, "lZc=");
      TestBase64Decode(new byte[] { 150, 152 }, "lpg=");
      TestBase64Decode(new byte[] { 151, 153 }, "l5k=");
      TestBase64Decode(new byte[] { 152, 154 }, "mJo=");
      TestBase64Decode(new byte[] { 153, 155 }, "mZs=");
      TestBase64Decode(new byte[] { 154, 156 }, "mpw=");
      TestBase64Decode(new byte[] { 155, 157 }, "m50=");
      TestBase64Decode(new byte[] { 156, 158 }, "nJ4=");
      TestBase64Decode(new byte[] { 157, 159 }, "nZ8=");
      TestBase64Decode(new byte[] { 158, 160 }, "nqA=");
      TestBase64Decode(new byte[] { 159, 161 }, "n6E=");
      TestBase64Decode(new byte[] { 160, 162 }, "oKI=");
      TestBase64Decode(new byte[] { 161, 163 }, "oaM=");
      TestBase64Decode(new byte[] { 162, 164 }, "oqQ=");
      TestBase64Decode(new byte[] { 163, 165 }, "o6U=");
      TestBase64Decode(new byte[] { 164, 166 }, "pKY=");
      TestBase64Decode(new byte[] { 165, 167 }, "pac=");
      TestBase64Decode(new byte[] { 166, 168 }, "pqg=");
      TestBase64Decode(new byte[] { 167, 169 }, "p6k=");
      TestBase64Decode(new byte[] { 168, 170 }, "qKo=");
      TestBase64Decode(new byte[] { 169, 171 }, "qas=");
      TestBase64Decode(new byte[] { 170, 172 }, "qqw=");
      TestBase64Decode(new byte[] { 171, 173 }, "q60=");
      TestBase64Decode(new byte[] { 172, 174 }, "rK4=");
      TestBase64Decode(new byte[] { 173, 175 }, "ra8=");
      TestBase64Decode(new byte[] { 174, 176 }, "rrA=");
      TestBase64Decode(new byte[] { 175, 177 }, "r7E=");
      TestBase64Decode(new byte[] { 176, 178 }, "sLI=");
      TestBase64Decode(new byte[] { 177, 179 }, "sbM=");
      TestBase64Decode(new byte[] { 178, 180 }, "srQ=");
      TestBase64Decode(new byte[] { 179, 181 }, "s7U=");
      TestBase64Decode(new byte[] { 180, 182 }, "tLY=");
      TestBase64Decode(new byte[] { 181, 183 }, "tbc=");
      TestBase64Decode(new byte[] { 182, 184 }, "trg=");
      TestBase64Decode(new byte[] { 183, 185 }, "t7k=");
      TestBase64Decode(new byte[] { 184, 186 }, "uLo=");
      TestBase64Decode(new byte[] { 185, 187 }, "ubs=");
      TestBase64Decode(new byte[] { 186, 188 }, "urw=");
      TestBase64Decode(new byte[] { 187, 189 }, "u70=");
      TestBase64Decode(new byte[] { 188, 190 }, "vL4=");
      TestBase64Decode(new byte[] { 189, 191 }, "vb8=");
      TestBase64Decode(new byte[] { 190, 192 }, "vsA=");
      TestBase64Decode(new byte[] { 191, 193 }, "v8E=");
      TestBase64Decode(new byte[] { 192, 194 }, "wMI=");
      TestBase64Decode(new byte[] { 193, 195 }, "wcM=");
      TestBase64Decode(new byte[] { 194, 196 }, "wsQ=");
      TestBase64Decode(new byte[] { 195, 197 }, "w8U=");
      TestBase64Decode(new byte[] { 196, 198 }, "xMY=");
      TestBase64Decode(new byte[] { 197, 199 }, "xcc=");
      TestBase64Decode(new byte[] { 198, 200 }, "xsg=");
      TestBase64Decode(new byte[] { 199, 201 }, "x8k=");
      TestBase64Decode(new byte[] { 200, 202 }, "yMo=");
      TestBase64Decode(new byte[] { 201, 203 }, "ycs=");
      TestBase64Decode(new byte[] { 202, 204 }, "ysw=");
      TestBase64Decode(new byte[] { 203, 205 }, "y80=");
      TestBase64Decode(new byte[] { 204, 206 }, "zM4=");
      TestBase64Decode(new byte[] { 205, 207 }, "zc8=");
      TestBase64Decode(new byte[] { 206, 208 }, "ztA=");
      TestBase64Decode(new byte[] { 207, 209 }, "z9E=");
      TestBase64Decode(new byte[] { 208, 210 }, "0NI=");
      TestBase64Decode(new byte[] { 209, 211 }, "0dM=");
      TestBase64Decode(new byte[] { 210, 212 }, "0tQ=");
      TestBase64Decode(new byte[] { 211, 213 }, "09U=");
      TestBase64Decode(new byte[] { 212, 214 }, "1NY=");
      TestBase64Decode(new byte[] { 213, 215 }, "1dc=");
      TestBase64Decode(new byte[] { 214, 216 }, "1tg=");
      TestBase64Decode(new byte[] { 215, 217 }, "19k=");
      TestBase64Decode(new byte[] { 216, 218 }, "2No=");
      TestBase64Decode(new byte[] { 217, 219 }, "2ds=");
      TestBase64Decode(new byte[] { 218, 220 }, "2tw=");
      TestBase64Decode(new byte[] { 219, 221 }, "290=");
      TestBase64Decode(new byte[] { 220, 222 }, "3N4=");
      TestBase64Decode(new byte[] { 221, 223 }, "3d8=");
      TestBase64Decode(new byte[] { 222, 224 }, "3uA=");
      TestBase64Decode(new byte[] { 223, 225 }, "3+E=");
      TestBase64Decode(new byte[] { 224, 226 }, "4OI=");
      TestBase64Decode(new byte[] { 225, 227 }, "4eM=");
      TestBase64Decode(new byte[] { 226, 228 }, "4uQ=");
      TestBase64Decode(new byte[] { 227, 229 }, "4+U=");
      TestBase64Decode(new byte[] { 228, 230 }, "5OY=");
      TestBase64Decode(new byte[] { 229, 231 }, "5ec=");
      TestBase64Decode(new byte[] { 230, 232 }, "5ug=");
      TestBase64Decode(new byte[] { 231, 233 }, "5+k=");
      TestBase64Decode(new byte[] { 232, 234 }, "6Oo=");
      TestBase64Decode(new byte[] { 233, 235 }, "6es=");
      TestBase64Decode(new byte[] { 234, 236 }, "6uw=");
      TestBase64Decode(new byte[] { 235, 237 }, "6+0=");
      TestBase64Decode(new byte[] { 236, 238 }, "7O4=");
      TestBase64Decode(new byte[] { 237, 239 }, "7e8=");
      TestBase64Decode(new byte[] { 238, 240 }, "7vA=");
      TestBase64Decode(new byte[] { 239, 241 }, "7/E=");
      TestBase64Decode(new byte[] { 240, 242 }, "8PI=");
      TestBase64Decode(new byte[] { 241, 243 }, "8fM=");
      TestBase64Decode(new byte[] { 242, 244 }, "8vQ=");
      TestBase64Decode(new byte[] { 243, 245 }, "8/U=");
      TestBase64Decode(new byte[] { 244, 246 }, "9PY=");
      TestBase64Decode(new byte[] { 245, 247 }, "9fc=");
      TestBase64Decode(new byte[] { 246, 248 }, "9vg=");
      TestBase64Decode(new byte[] { 247, 249 }, "9/k=");
      TestBase64Decode(new byte[] { 248, 250 }, "+Po=");
      TestBase64Decode(new byte[] { 249, 251 }, "+fs=");
      TestBase64Decode(new byte[] { 250, 252 }, "+vw=");
      TestBase64Decode(new byte[] { 251, 253 }, "+/0=");
      TestBase64Decode(new byte[] { 252, 254 }, "/P4=");
      TestBase64Decode(new byte[] { 253, 255 }, "/f8=");
      TestBase64Decode(new byte[] { 254, 0 }, "/gA=");
      TestBase64Decode(new byte[] { 255, 1 }, "/wE=");
      TestBase64Decode(new byte[] { 0, 2, 4 }, "AAIE");
      TestBase64Decode(new byte[] { 1, 3, 5 }, "AQMF");
      TestBase64Decode(new byte[] { 2, 4, 6 }, "AgQG");
      TestBase64Decode(new byte[] { 3, 5, 7 }, "AwUH");
      TestBase64Decode(new byte[] { 4, 6, 8 }, "BAYI");
      TestBase64Decode(new byte[] { 5, 7, 9 }, "BQcJ");
      TestBase64Decode(new byte[] { 6, 8, 10 }, "BggK");
      TestBase64Decode(new byte[] { 7, 9, 11 }, "BwkL");
      TestBase64Decode(new byte[] { 8, 10, 12 }, "CAoM");
      TestBase64Decode(new byte[] { 9, 11, 13 }, "CQsN");
      TestBase64Decode(new byte[] { 10, 12, 14 }, "CgwO");
      TestBase64Decode(new byte[] { 11, 13, 15 }, "Cw0P");
      TestBase64Decode(new byte[] { 12, 14, 16 }, "DA4Q");
      TestBase64Decode(new byte[] { 13, 15, 17 }, "DQ8R");
      TestBase64Decode(new byte[] { 14, 16, 18 }, "DhAS");
      TestBase64Decode(new byte[] { 15, 17, 19 }, "DxET");
      TestBase64Decode(new byte[] { 16, 18, 20 }, "EBIU");
      TestBase64Decode(new byte[] { 17, 19, 21 }, "ERMV");
      TestBase64Decode(new byte[] { 18, 20, 22 }, "EhQW");
      TestBase64Decode(new byte[] { 19, 21, 23 }, "ExUX");
      TestBase64Decode(new byte[] { 20, 22, 24 }, "FBYY");
      TestBase64Decode(new byte[] { 21, 23, 25 }, "FRcZ");
      TestBase64Decode(new byte[] { 22, 24, 26 }, "Fhga");
      TestBase64Decode(new byte[] { 23, 25, 27 }, "Fxkb");
      TestBase64Decode(new byte[] { 24, 26, 28 }, "GBoc");
      TestBase64Decode(new byte[] { 25, 27, 29 }, "GRsd");
      TestBase64Decode(new byte[] { 26, 28, 30 }, "Ghwe");
      TestBase64Decode(new byte[] { 27, 29, 31 }, "Gx0f");
      TestBase64Decode(new byte[] { 28, 30, 32 }, "HB4g");
      TestBase64Decode(new byte[] { 29, 31, 33 }, "HR8h");
      TestBase64Decode(new byte[] { 30, 32, 34 }, "HiAi");
      TestBase64Decode(new byte[] { 31, 33, 35 }, "HyEj");
      TestBase64Decode(new byte[] { 32, 34, 36 }, "ICIk");
      TestBase64Decode(new byte[] { 33, 35, 37 }, "ISMl");
      TestBase64Decode(new byte[] { 34, 36, 38 }, "IiQm");
      TestBase64Decode(new byte[] { 35, 37, 39 }, "IyUn");
      TestBase64Decode(new byte[] { 36, 38, 40 }, "JCYo");
      TestBase64Decode(new byte[] { 37, 39, 41 }, "JScp");
      TestBase64Decode(new byte[] { 38, 40, 42 }, "Jigq");
      TestBase64Decode(new byte[] { 39, 41, 43 }, "Jykr");
      TestBase64Decode(new byte[] { 40, 42, 44 }, "KCos");
      TestBase64Decode(new byte[] { 41, 43, 45 }, "KSst");
      TestBase64Decode(new byte[] { 42, 44, 46 }, "Kiwu");
      TestBase64Decode(new byte[] { 43, 45, 47 }, "Ky0v");
      TestBase64Decode(new byte[] { 44, 46, 48 }, "LC4w");
      TestBase64Decode(new byte[] { 45, 47, 49 }, "LS8x");
      TestBase64Decode(new byte[] { 46, 48, 50 }, "LjAy");
      TestBase64Decode(new byte[] { 47, 49, 51 }, "LzEz");
      TestBase64Decode(new byte[] { 48, 50, 52 }, "MDI0");
      TestBase64Decode(new byte[] { 49, 51, 53 }, "MTM1");
      TestBase64Decode(new byte[] { 50, 52, 54 }, "MjQ2");
      TestBase64Decode(new byte[] { 51, 53, 55 }, "MzU3");
      TestBase64Decode(new byte[] { 52, 54, 56 }, "NDY4");
      TestBase64Decode(new byte[] { 53, 55, 57 }, "NTc5");
      TestBase64Decode(new byte[] { 54, 56, 58 }, "Njg6");
      TestBase64Decode(new byte[] { 55, 57, 59 }, "Nzk7");
      TestBase64Decode(new byte[] { 56, 58, 60 }, "ODo8");
      TestBase64Decode(new byte[] { 57, 59, 61 }, "OTs9");
      TestBase64Decode(new byte[] { 58, 60, 62 }, "Ojw+");
      TestBase64Decode(new byte[] { 59, 61, 63 }, "Oz0/");
      TestBase64Decode(new byte[] { 60, 62, 64 }, "PD5A");
      TestBase64Decode(new byte[] { 61, 63, 65 }, "PT9B");
      TestBase64Decode(new byte[] { 62, 64, 66 }, "PkBC");
      TestBase64Decode(new byte[] { 63, 65, 67 }, "P0FD");
      TestBase64Decode(new byte[] { 64, 66, 68 }, "QEJE");
      TestBase64Decode(new byte[] { 65, 67, 69 }, "QUNF");
      TestBase64Decode(new byte[] { 66, 68, 70 }, "QkRG");
      TestBase64Decode(new byte[] { 67, 69, 71 }, "Q0VH");
      TestBase64Decode(new byte[] { 68, 70, 72 }, "REZI");
      TestBase64Decode(new byte[] { 69, 71, 73 }, "RUdJ");
      TestBase64Decode(new byte[] { 70, 72, 74 }, "RkhK");
      TestBase64Decode(new byte[] { 71, 73, 75 }, "R0lL");
      TestBase64Decode(new byte[] { 72, 74, 76 }, "SEpM");
      TestBase64Decode(new byte[] { 73, 75, 77 }, "SUtN");
      TestBase64Decode(new byte[] { 74, 76, 78 }, "SkxO");
      TestBase64Decode(new byte[] { 75, 77, 79 }, "S01P");
      TestBase64Decode(new byte[] { 76, 78, 80 }, "TE5Q");
      TestBase64Decode(new byte[] { 77, 79, 81 }, "TU9R");
      TestBase64Decode(new byte[] { 78, 80, 82 }, "TlBS");
      TestBase64Decode(new byte[] { 79, 81, 83 }, "T1FT");
      TestBase64Decode(new byte[] { 80, 82, 84 }, "UFJU");
      TestBase64Decode(new byte[] { 81, 83, 85 }, "UVNV");
      TestBase64Decode(new byte[] { 82, 84, 86 }, "UlRW");
      TestBase64Decode(new byte[] { 83, 85, 87 }, "U1VX");
      TestBase64Decode(new byte[] { 84, 86, 88 }, "VFZY");
      TestBase64Decode(new byte[] { 85, 87, 89 }, "VVdZ");
      TestBase64Decode(new byte[] { 86, 88, 90 }, "Vlha");
      TestBase64Decode(new byte[] { 87, 89, 91 }, "V1lb");
      TestBase64Decode(new byte[] { 88, 90, 92 }, "WFpc");
      TestBase64Decode(new byte[] { 89, 91, 93 }, "WVtd");
      TestBase64Decode(new byte[] { 90, 92, 94 }, "Wlxe");
      TestBase64Decode(new byte[] { 91, 93, 95 }, "W11f");
      TestBase64Decode(new byte[] { 92, 94, 96 }, "XF5g");
      TestBase64Decode(new byte[] { 93, 95, 97 }, "XV9h");
      TestBase64Decode(new byte[] { 94, 96, 98 }, "XmBi");
      TestBase64Decode(new byte[] { 95, 97, 99 }, "X2Fj");
      TestBase64Decode(new byte[] { 96, 98, 100 }, "YGJk");
      TestBase64Decode(new byte[] { 97, 99, 101 }, "YWNl");
      TestBase64Decode(new byte[] { 98, 100, 102 }, "YmRm");
      TestBase64Decode(new byte[] { 99, 101, 103 }, "Y2Vn");
      TestBase64Decode(new byte[] { 100, 102, 104 }, "ZGZo");
      TestBase64Decode(new byte[] { 101, 103, 105 }, "ZWdp");
      TestBase64Decode(new byte[] { 102, 104, 106 }, "Zmhq");
      TestBase64Decode(new byte[] { 103, 105, 107 }, "Z2lr");
      TestBase64Decode(new byte[] { 104, 106, 108 }, "aGps");
      TestBase64Decode(new byte[] { 105, 107, 109 }, "aWtt");
      TestBase64Decode(new byte[] { 106, 108, 110 }, "amxu");
      TestBase64Decode(new byte[] { 107, 109, 111 }, "a21v");
      TestBase64Decode(new byte[] { 108, 110, 112 }, "bG5w");
      TestBase64Decode(new byte[] { 109, 111, 113 }, "bW9x");
      TestBase64Decode(new byte[] { 110, 112, 114 }, "bnBy");
      TestBase64Decode(new byte[] { 111, 113, 115 }, "b3Fz");
      TestBase64Decode(new byte[] { 112, 114, 116 }, "cHJ0");
      TestBase64Decode(new byte[] { 113, 115, 117 }, "cXN1");
      TestBase64Decode(new byte[] { 114, 116, 118 }, "cnR2");
      TestBase64Decode(new byte[] { 115, 117, 119 }, "c3V3");
      TestBase64Decode(new byte[] { 116, 118, 120 }, "dHZ4");
      TestBase64Decode(new byte[] { 117, 119, 121 }, "dXd5");
      TestBase64Decode(new byte[] { 118, 120, 122 }, "dnh6");
      TestBase64Decode(new byte[] { 119, 121, 123 }, "d3l7");
      TestBase64Decode(new byte[] { 120, 122, 124 }, "eHp8");
      TestBase64Decode(new byte[] { 121, 123, 125 }, "eXt9");
      TestBase64Decode(new byte[] { 122, 124, 126 }, "enx+");
      TestBase64Decode(new byte[] { 123, 125, 127 }, "e31/");
      TestBase64Decode(new byte[] { 124, 126, 128 }, "fH6A");
      TestBase64Decode(new byte[] { 125, 127, 129 }, "fX+B");
      TestBase64Decode(new byte[] { 126, 128, 130 }, "foCC");
      TestBase64Decode(new byte[] { 127, 129, 131 }, "f4GD");
      TestBase64Decode(new byte[] { 128, 130, 132 }, "gIKE");
      TestBase64Decode(new byte[] { 129, 131, 133 }, "gYOF");
      TestBase64Decode(new byte[] { 130, 132, 134 }, "goSG");
      TestBase64Decode(new byte[] { 131, 133, 135 }, "g4WH");
      TestBase64Decode(new byte[] { 132, 134, 136 }, "hIaI");
      TestBase64Decode(new byte[] { 133, 135, 137 }, "hYeJ");
      TestBase64Decode(new byte[] { 134, 136, 138 }, "hoiK");
      TestBase64Decode(new byte[] { 135, 137, 139 }, "h4mL");
      TestBase64Decode(new byte[] { 136, 138, 140 }, "iIqM");
      TestBase64Decode(new byte[] { 137, 139, 141 }, "iYuN");
      TestBase64Decode(new byte[] { 138, 140, 142 }, "ioyO");
      TestBase64Decode(new byte[] { 139, 141, 143 }, "i42P");
      TestBase64Decode(new byte[] { 140, 142, 144 }, "jI6Q");
      TestBase64Decode(new byte[] { 141, 143, 145 }, "jY+R");
      TestBase64Decode(new byte[] { 142, 144, 146 }, "jpCS");
      TestBase64Decode(new byte[] { 143, 145, 147 }, "j5GT");
      TestBase64Decode(new byte[] { 144, 146, 148 }, "kJKU");
      TestBase64Decode(new byte[] { 145, 147, 149 }, "kZOV");
      TestBase64Decode(new byte[] { 146, 148, 150 }, "kpSW");
      TestBase64Decode(new byte[] { 147, 149, 151 }, "k5WX");
      TestBase64Decode(new byte[] { 148, 150, 152 }, "lJaY");
      TestBase64Decode(new byte[] { 149, 151, 153 }, "lZeZ");
      TestBase64Decode(new byte[] { 150, 152, 154 }, "lpia");
      TestBase64Decode(new byte[] { 151, 153, 155 }, "l5mb");
      TestBase64Decode(new byte[] { 152, 154, 156 }, "mJqc");
      TestBase64Decode(new byte[] { 153, 155, 157 }, "mZud");
      TestBase64Decode(new byte[] { 154, 156, 158 }, "mpye");
      TestBase64Decode(new byte[] { 155, 157, 159 }, "m52f");
      TestBase64Decode(new byte[] { 156, 158, 160 }, "nJ6g");
      TestBase64Decode(new byte[] { 157, 159, 161 }, "nZ+h");
      TestBase64Decode(new byte[] { 158, 160, 162 }, "nqCi");
      TestBase64Decode(new byte[] { 159, 161, 163 }, "n6Gj");
      TestBase64Decode(new byte[] { 160, 162, 164 }, "oKKk");
      TestBase64Decode(new byte[] { 161, 163, 165 }, "oaOl");
      TestBase64Decode(new byte[] { 162, 164, 166 }, "oqSm");
      TestBase64Decode(new byte[] { 163, 165, 167 }, "o6Wn");
      TestBase64Decode(new byte[] { 164, 166, 168 }, "pKao");
      TestBase64Decode(new byte[] { 165, 167, 169 }, "paep");
      TestBase64Decode(new byte[] { 166, 168, 170 }, "pqiq");
      TestBase64Decode(new byte[] { 167, 169, 171 }, "p6mr");
      TestBase64Decode(new byte[] { 168, 170, 172 }, "qKqs");
      TestBase64Decode(new byte[] { 169, 171, 173 }, "qaut");
      TestBase64Decode(new byte[] { 170, 172, 174 }, "qqyu");
      TestBase64Decode(new byte[] { 171, 173, 175 }, "q62v");
      TestBase64Decode(new byte[] { 172, 174, 176 }, "rK6w");
      TestBase64Decode(new byte[] { 173, 175, 177 }, "ra+x");
      TestBase64Decode(new byte[] { 174, 176, 178 }, "rrCy");
      TestBase64Decode(new byte[] { 175, 177, 179 }, "r7Gz");
      TestBase64Decode(new byte[] { 176, 178, 180 }, "sLK0");
      TestBase64Decode(new byte[] { 177, 179, 181 }, "sbO1");
      TestBase64Decode(new byte[] { 178, 180, 182 }, "srS2");
      TestBase64Decode(new byte[] { 179, 181, 183 }, "s7W3");
      TestBase64Decode(new byte[] { 180, 182, 184 }, "tLa4");
      TestBase64Decode(new byte[] { 181, 183, 185 }, "tbe5");
      TestBase64Decode(new byte[] { 182, 184, 186 }, "tri6");
      TestBase64Decode(new byte[] { 183, 185, 187 }, "t7m7");
      TestBase64Decode(new byte[] { 184, 186, 188 }, "uLq8");
      TestBase64Decode(new byte[] { 185, 187, 189 }, "ubu9");
      TestBase64Decode(new byte[] { 186, 188, 190 }, "ury+");
      TestBase64Decode(new byte[] { 187, 189, 191 }, "u72/");
      TestBase64Decode(new byte[] { 188, 190, 192 }, "vL7A");
      TestBase64Decode(new byte[] { 189, 191, 193 }, "vb/B");
      TestBase64Decode(new byte[] { 190, 192, 194 }, "vsDC");
      TestBase64Decode(new byte[] { 191, 193, 195 }, "v8HD");
      TestBase64Decode(new byte[] { 192, 194, 196 }, "wMLE");
      TestBase64Decode(new byte[] { 193, 195, 197 }, "wcPF");
      TestBase64Decode(new byte[] { 194, 196, 198 }, "wsTG");
      TestBase64Decode(new byte[] { 195, 197, 199 }, "w8XH");
      TestBase64Decode(new byte[] { 196, 198, 200 }, "xMbI");
      TestBase64Decode(new byte[] { 197, 199, 201 }, "xcfJ");
      TestBase64Decode(new byte[] { 198, 200, 202 }, "xsjK");
      TestBase64Decode(new byte[] { 199, 201, 203 }, "x8nL");
      TestBase64Decode(new byte[] { 200, 202, 204 }, "yMrM");
      TestBase64Decode(new byte[] { 201, 203, 205 }, "ycvN");
      TestBase64Decode(new byte[] { 202, 204, 206 }, "yszO");
      TestBase64Decode(new byte[] { 203, 205, 207 }, "y83P");
      TestBase64Decode(new byte[] { 204, 206, 208 }, "zM7Q");
      TestBase64Decode(new byte[] { 205, 207, 209 }, "zc/R");
      TestBase64Decode(new byte[] { 206, 208, 210 }, "ztDS");
      TestBase64Decode(new byte[] { 207, 209, 211 }, "z9HT");
      TestBase64Decode(new byte[] { 208, 210, 212 }, "0NLU");
      TestBase64Decode(new byte[] { 209, 211, 213 }, "0dPV");
      TestBase64Decode(new byte[] { 210, 212, 214 }, "0tTW");
      TestBase64Decode(new byte[] { 211, 213, 215 }, "09XX");
      TestBase64Decode(new byte[] { 212, 214, 216 }, "1NbY");
      TestBase64Decode(new byte[] { 213, 215, 217 }, "1dfZ");
      TestBase64Decode(new byte[] { 214, 216, 218 }, "1tja");
      TestBase64Decode(new byte[] { 215, 217, 219 }, "19nb");
      TestBase64Decode(new byte[] { 216, 218, 220 }, "2Nrc");
      TestBase64Decode(new byte[] { 217, 219, 221 }, "2dvd");
      TestBase64Decode(new byte[] { 218, 220, 222 }, "2tze");
      TestBase64Decode(new byte[] { 219, 221, 223 }, "293f");
      TestBase64Decode(new byte[] { 220, 222, 224 }, "3N7g");
      TestBase64Decode(new byte[] { 221, 223, 225 }, "3d/h");
      TestBase64Decode(new byte[] { 222, 224, 226 }, "3uDi");
      TestBase64Decode(new byte[] { 223, 225, 227 }, "3+Hj");
      TestBase64Decode(new byte[] { 224, 226, 228 }, "4OLk");
      TestBase64Decode(new byte[] { 225, 227, 229 }, "4ePl");
      TestBase64Decode(new byte[] { 226, 228, 230 }, "4uTm");
      TestBase64Decode(new byte[] { 227, 229, 231 }, "4+Xn");
      TestBase64Decode(new byte[] { 228, 230, 232 }, "5Obo");
      TestBase64Decode(new byte[] { 229, 231, 233 }, "5efp");
      TestBase64Decode(new byte[] { 230, 232, 234 }, "5ujq");
      TestBase64Decode(new byte[] { 231, 233, 235 }, "5+nr");
      TestBase64Decode(new byte[] { 232, 234, 236 }, "6Ors");
      TestBase64Decode(new byte[] { 233, 235, 237 }, "6evt");
      TestBase64Decode(new byte[] { 234, 236, 238 }, "6uzu");
      TestBase64Decode(new byte[] { 235, 237, 239 }, "6+3v");
      TestBase64Decode(new byte[] { 236, 238, 240 }, "7O7w");
      TestBase64Decode(new byte[] { 237, 239, 241 }, "7e/x");
      TestBase64Decode(new byte[] { 238, 240, 242 }, "7vDy");
      TestBase64Decode(new byte[] { 239, 241, 243 }, "7/Hz");
      TestBase64Decode(new byte[] { 240, 242, 244 }, "8PL0");
      TestBase64Decode(new byte[] { 241, 243, 245 }, "8fP1");
      TestBase64Decode(new byte[] { 242, 244, 246 }, "8vT2");
      TestBase64Decode(new byte[] { 243, 245, 247 }, "8/X3");
      TestBase64Decode(new byte[] { 244, 246, 248 }, "9Pb4");
      TestBase64Decode(new byte[] { 245, 247, 249 }, "9ff5");
      TestBase64Decode(new byte[] { 246, 248, 250 }, "9vj6");
      TestBase64Decode(new byte[] { 247, 249, 251 }, "9/n7");
      TestBase64Decode(new byte[] { 248, 250, 252 }, "+Pr8");
      TestBase64Decode(new byte[] { 249, 251, 253 }, "+fv9");
      TestBase64Decode(new byte[] { 250, 252, 254 }, "+vz+");
      TestBase64Decode(new byte[] { 251, 253, 255 }, "+/3/");
      TestBase64Decode(new byte[] { 252, 254, 0 }, "/P4A");
      TestBase64Decode(new byte[] { 253, 255, 1 }, "/f8B");
      TestBase64Decode(new byte[] { 254, 0, 2 }, "/gAC");
      TestBase64Decode(new byte[] { 255, 1, 3 }, "/wED");
    }

    private void TestPercentEncodingOne(string expected, string input) {
      ContentDisposition cd = ContentDisposition.Parse(
        "inline; filename*=utf-8''" + input);
      Assert.AreEqual(expected, cd.GetParameter("filename"));
    }

    [TestMethod]
    public void TestPercentEncoding() {
      ICharacterEncoding utf8 = Encodings.GetEncoding("utf-8");
      TestPercentEncodingOne("test\u00be", "test%c2%be");
      TestPercentEncodingOne("tesA", "tes%41");
      TestPercentEncodingOne("tesa", "tes%61");
      TestPercentEncodingOne("tes\r\na", "tes%0d%0aa");
      TestPercentEncodingOne(
        "tes%xx",
        "tes%xx");
      TestPercentEncodingOne("tes%dxx", "tes%dxx");
    }

    private static void AssertUtf8Equal(byte[] expected, byte[] actual) {
      Assert.AreEqual(DataUtilities.GetUtf8String(expected, true),
                    DataUtilities.GetUtf8String(actual, true));
    }

    private static string WrapHeader(string s) {
      return Reflect.Invoke(Reflect.Construct(MailNamespace() +
                    ".WordWrapEncoder", String.Empty), "AddString",
                    s).ToString();
    }

    private void TestDowngradeDSNOne(string expected, string actual) {
      Assert.AreEqual(expected, (string)Reflect.InvokeStatic(MailNamespace() +
                    ".Message", "DowngradeRecipientHeaderValue", actual));
      string dsn;
      string expectedDSN;
      byte[] bytes;
      byte[] expectedBytes;
      bool encap = (expected.StartsWith("=?", StringComparison.Ordinal));
      dsn = "X-Ignore: X\r\n\r\nOriginal-Recipient: " + actual +
        "\r\nFinal-Recipient: " + actual + "\r\nX-Ignore: Y\r\n\r\n";
      if (encap) expectedDSN = "X-Ignore: X\r\n\r\n" +
        WrapHeader("Downgraded-Original-Recipient: " + expected) +
        "\r\n" + WrapHeader("Downgraded-Final-Recipient: " + expected) +
        "\r\nX-Ignore: Y\r\n\r\n";
      else {
        expectedDSN = "X-Ignore: X\r\n\r\n" +
          WrapHeader("Original-Recipient: " + expected) + "\r\n" +
          WrapHeader("Final-Recipient: " + expected) +
          "\r\nX-Ignore: Y\r\n\r\n";
      }
      bytes = (byte[])Reflect.InvokeStatic(MailNamespace() + ".Message",
             "DowngradeDeliveryStatus" , DataUtilities.GetUtf8Bytes(dsn,
                    true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
      dsn = "X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\nOriginal-Recipient: " +
        actual + "\r\nFinal-Recipient: " + actual +
        "\r\nX-Ignore: Y\r\n\r\n";
      if (encap)
        expectedDSN = "X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\n" +
          WrapHeader("Downgraded-Original-Recipient: " + expected) +
          "\r\n" + WrapHeader("Downgraded-Final-Recipient: " + expected) +
          "\r\nX-Ignore: Y\r\n\r\n";
      else {
        expectedDSN = "X-Ignore: X\r\n\r\nX-Ignore: X\r\n Y\r\n" +
          WrapHeader("Original-Recipient: " + expected) + "\r\n" +
          WrapHeader("Final-Recipient: " + expected) +
          "\r\nX-Ignore: Y\r\n\r\n";
      }
      bytes = (byte[])Reflect.InvokeStatic(MailNamespace() + ".Message",
             "DowngradeDeliveryStatus" , DataUtilities.GetUtf8Bytes(dsn,
                    true));
      expectedBytes = DataUtilities.GetUtf8Bytes(expectedDSN, true);
      AssertUtf8Equal(expectedBytes, bytes);
      dsn = "X-Ignore: X\r\n\r\nOriginal-recipient : " + actual +
        "\r\nFinal-Recipient: " + actual + "\r\nX-Ignore: Y\r\n\r\n";
      if (encap) expectedDSN = "X-Ignore: X\r\n\r\n" +
        WrapHeader("Downgraded-Original-Recipient: " + expected) +
        "\r\n" + WrapHeader("Downgraded-Final-Recipient: " + expected) +
        "\r\nX-Ignore: Y\r\n\r\n";
      else {
        expectedDSN = "X-Ignore: X\r\n\r\n" +
          WrapHeader("Original-recipient : " + expected) + "\r\n" +
          WrapHeader("Final-Recipient: " + expected) +
          "\r\nX-Ignore: Y\r\n\r\n";
      }
      bytes = (byte[])Reflect.InvokeStatic(MailNamespace() + ".Message",
             "DowngradeDeliveryStatus" , DataUtilities.GetUtf8Bytes(dsn,
                    true));
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
                    "FF20}.example", ("utf-8; x@x\u00be\uff20.example"));
      TestDowngradeDSNOne("(=?utf-8?Q?=C2=BE?=) utf-8; x@x.example",
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

    [TestMethod]
    public void TestLanguageTags() {
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-a-bb-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "0-xx-xx"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "9-xx-xx"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "a-xx-xx"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "x-xx-xx"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                  ".ParserUtility" , "IsValidLanguageTag",
                    "en-US-u-islamcal"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
              ".ParserUtility" , "IsValidLanguageTag",
                    "zh-CN-a-myext-x-private"
));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
              ".ParserUtility" , "IsValidLanguageTag",
                    "en-a-myext-b-another"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "de-419-DE"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "a-DE"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
              ".ParserUtility" , "IsValidLanguageTag",
                    "ar-a-aaa-b-bbb-a-ccc"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "qbb-us"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "zh-yue"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-us"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "e0-us"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-gb-1999"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-gb-1999-1998"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-gb-1999-1999"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-gb-oed"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "sr-Latn-RS"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "x-aaaaaaaaa-y-z"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "x-aaaaaaaa-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "a-b-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "a-bb-xx-yy-zz"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "a-bb-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "a-x-y-z"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "x-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "i-lojban"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "i-klingon"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "art-lojban"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "sgn-be-fr"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "no-bok"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "z-xx-xx"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag",
                    "en-aaa-bbbb-x-xxx-yyy-zzz"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
               ".ParserUtility" , "IsValidLanguageTag",
                    "en-aaa-bbbb-x-x-y-z"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-aaa-bbb"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-aaa-bbb-ccc"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-aaa-bbbb"));
      Assert.IsTrue((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-aaa-bbbb-cc"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-aaa-bbb-"));
      Assert.IsFalse((bool)Reflect.InvokeStatic(MailNamespace() +
                    ".ParserUtility", "IsValidLanguageTag", "en-aaa-bbb-ccc-"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void TestDecodeQuotedPrintable() {
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
      TestDecodeQuotedPrintable("te=\r\nst", "test");
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
      TestDecodeQuotedPrintable("te w\r\nst", "te w\r\nst");
      TestDecodeQuotedPrintable("te =\r\nst", "te st");
    }
    [TestMethod]
    public void TestSpaceBeforeBreakQuotedPrintable() {
      TestDecodeQuotedPrintable("te \r\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te\t \r\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te \t\r\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te \r\n", "te\r\n");
      TestDecodeQuotedPrintable("te\t \r\n", "te\r\n");
      TestDecodeQuotedPrintable("te \t\r\n", "te\r\n");
    }
    //[TestMethod]
    public void TestLenientQuotedPrintable() {
      // Ignore for now, Message constructor currently uses
      // non-lenient quoted-printable parsing
      TestDecodeQuotedPrintable("te\rst", "te\r\nst");
      TestDecodeQuotedPrintable("te\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te \t\nst", "te\r\nst");
      TestDecodeQuotedPrintable("te=\rst", "test");
      TestDecodeQuotedPrintable("te=\nst", "test");
      TestDecodeQuotedPrintable("te\t \nst", "te\r\nst");
      TestDecodeQuotedPrintable("te\t \rst", "te\r\nst");
      TestDecodeQuotedPrintable("te \rst", "te\r\nst");
      TestDecodeQuotedPrintable("te \nst", "te\r\nst");
      TestDecodeQuotedPrintable("te=\r", "te");
      TestDecodeQuotedPrintable("te=\n", "te");
    }
    [TestMethod]
    public void TestNonLenientQuotedPrintable() {
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
      string str = "From: " + input + " <test@example.com>\r\n\r\nTest";
      Message msg = MessageTest.MessageFromString(str);
      Assert.AreEqual(
        expected + " <test@example.com>",
        msg.GetHeader("from"));
    }

    public static void TestEncodedWordsOne(string expected, string input) {
      string par = "(";
      Assert.AreEqual(expected, Reflect.InvokeStatic(MailNamespace() +
                    ".Rfc2047", "DecodeEncodedWords", input, 0, input.Length,
                Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
                    "Unstructured")));
      Assert.AreEqual(
        "(" + expected + ") en",
        DecodeHeaderField("content-language", "(" + input + ") en"));
      Assert.AreEqual(" (" + expected + ") en",
                 DecodeHeaderField("content-language" , " (" + input +
                    ") en"));
      Assert.AreEqual(" " + par + "comment " + par + "cmt " + expected +
                    ")comment) en",
              DecodeHeaderField("content-language" , " (comment (cmt " +
                input + ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + "=?bad?= " + expected + ")comment) en",
        DecodeHeaderField("content-language", " (comment (=?bad?= " + input +
                    ")comment) en"));
      Assert.AreEqual(
        " " + par + "comment " + par + String.Empty + expected + ")comment) en",
        DecodeHeaderField("content-language", " (comment (" + input +
                    ")comment) en"));
      Assert.AreEqual(
        " (" + expected + "()) en", DecodeHeaderField("content-language",
                    " (" + input + "()) en"));
      Assert.AreEqual(
        " en (" + expected + ")",
        DecodeHeaderField("content-language", " en (" + input + ")"));
      Assert.AreEqual(expected,
                    DecodeHeaderField("subject", input));
    }

    [TestMethod]
    public void TestEncodedPhrase2() {
      {
        string stringTemp = DowngradeHeaderField("subject",
                    "(tes\u00bet) x@x.example");
        Assert.AreEqual(
          "=?utf-8?Q?=28tes=C2=BEt=29_x=40x=2Eexample?=",
          stringTemp);
      }
    }

    [TestMethod]
    public void TestToFieldDowngrading() {
      string sep = ", ";
      Assert.AreEqual("x <x@example.com>" + sep + "\"X\" <y@example.com>",
       DowngradeHeaderField("to",
                    "x <x@example.com>, \"X\" <y@example.com>"));
      Assert.AreEqual("x <x@example.com>" + sep +
               "=?utf-8?Q?=C2=BE?= <y@example.com>",
                    DowngradeHeaderField("to",
                    "x <x@example.com>, \u00be <y@example.com>"));
      Assert.AreEqual("x <x@example.com>" + sep +
                    "=?utf-8?Q?=C2=BE?= <y@example.com>",
  DowngradeHeaderField("to",
                    "x <x@example.com>, \"\u00be\" <y@example.com>"));
      Assert.AreEqual(
   "x <x@example.com>" + sep + "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <y@example.com>",
        DowngradeHeaderField("to",
                    "x <x@example.com>, x\u00e1 x x\u00e1 <y@example.com>"));
      {
        string stringTemp = DowngradeHeaderField("to",
                    "g: x@example.com, x\u00e1y@example.com;");
        Assert.AreEqual(
          "g =?utf-8?Q?x=40example=2Ecom=2C_x=C3=A1y=40example=2Ecom?= :;",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("to",
                    "g: x@example.com, x@\u0300.example;");
        Assert.AreEqual(
          "g =?utf-8?Q?x=40example=2Ecom=2C_x=40=CC=80=2Eexample?= :;",
          stringTemp);
      }
      Assert.AreEqual("g: x@example.com" + sep + "x@xn--e-ufa.example;",
           DowngradeHeaderField("to",
                    "g: x@example.com, x@e\u00e1.example;"));
      {
  string stringTemp = DowngradeHeaderField("sender",
          "x <x@e\u00e1.example>");
        Assert.AreEqual(
          "x <x@xn--e-ufa.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("sender",
                    "x\u00e1 x x\u00e1 <x@example.com>");
        Assert.AreEqual(
          "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@example.com>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("sender",
                    "x\u00e1 x x\u00e1 <x@e\u00e1.example>");
        Assert.AreEqual(
          "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@xn--e-ufa.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("sender",
          "x <x\u00e1y@example.com>");
        Assert.AreEqual(
          "x =?utf-8?Q?x=C3=A1y=40example=2Ecom?= :;",
          stringTemp);
      }
    }

    private static string EncodeComment(string str) {
      return (string)Reflect.InvokeStatic(MailNamespace() + ".Rfc2047",
                    "EncodeComment", str, 0, str.Length);
    }

    private static string DowngradeHeaderField(string name, string value) {
      return (string)Reflect.Invoke(Reflect.InvokeStatic(MailNamespace() +
                    ".HeaderFieldParsers", "GetParser", name),
                    "DowngradeFieldValue", value);
    }

    private static string DecodeHeaderField(string name, string value) {
      return (string)Reflect.Invoke(Reflect.InvokeStatic(MailNamespace() +
                    ".HeaderFieldParsers", "GetParser", name),
                    "DecodeEncodedWords", value);
    }

    [TestMethod]
    public void TestCommentsToWords() {
      {
        string stringTemp = EncodeComment("(x)");
        Assert.AreEqual(
          "(=?utf-8?Q?x?=)",
          stringTemp);
      }
      {
        string stringTemp = EncodeComment("(x\\y)");
        Assert.AreEqual(
          "(=?utf-8?Q?xy?=)",
          stringTemp);
      }
      {
        string stringTemp = EncodeComment("(x\r\n y)");
        Assert.AreEqual(
          "(=?utf-8?Q?x_y?=)",
          stringTemp);
      }
      {
        string stringTemp = EncodeComment("(x\u00a0)");
        Assert.AreEqual(
          "(=?utf-8?Q?x=C2=A0?=)",
          stringTemp);
      }
      {
        string stringTemp = EncodeComment("(x\\\u00a0)");
        Assert.AreEqual(
          "(=?utf-8?Q?x=C2=A0?=)",
          stringTemp);
      }
      Assert.AreEqual("(=?utf-8?Q?x?=())", EncodeComment("(x())"));
      Assert.AreEqual("(=?utf-8?Q?x?=()=?utf-8?Q?y?=)",
                    EncodeComment("(x()y)"));
      Assert.AreEqual("(=?utf-8?Q?x?=(=?utf-8?Q?ab?=)=?utf-8?Q?y?=)",
                    EncodeComment("(x(a\\b)y)"));
      {
        string stringTemp = EncodeComment("()");
        Assert.AreEqual(
          "()",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(test) x@x.example");
        Assert.AreEqual(
          "(test) x@x.example",
          stringTemp);
      }
      {
 string stringTemp = DowngradeHeaderField("from",
          "(tes\u00bet) x@x.example");
        Assert.AreEqual(
          "(=?utf-8?Q?tes=C2=BEt?=) x@x.example",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("content-language",
                    "(tes\u00bet) en");
        Assert.AreEqual(
          "(=?utf-8?Q?tes=C2=BEt?=) en",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) Test <x@x.example>");
        Assert.AreEqual(
          "(comment) Test <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) Tes\u00bet <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) Tes\u00bet Subject <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) Test Sub\u00beject <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) \"Tes\u00bet\" <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) \"Tes\u00bet Subject\" <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) \"Test Sub\u00beject\" <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) \"Tes\u00bet   Subject\" <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "(comment) \"Tes\u00bet Subject\" (comment) <x@x.example>");
        Assert.AreEqual(
          "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "\"Tes\u00bet Subject\" (comment) <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from", "Test <x@x.example>");
        Assert.AreEqual(
          "Test <x@x.example>",
          stringTemp);
      }
      {
 string stringTemp = DowngradeHeaderField("from",
          "Tes\u00bet <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "Tes\u00bet Subject <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "Test Sub\u00beject <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "\"Tes\u00bet\" <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "\"Tes\u00bet Subject\" <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "\"Test Sub\u00beject\" <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "\"Tes\u00bet   Subject\" <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
          stringTemp);
      }
      {
        string stringTemp = DowngradeHeaderField("from",
                    "\"Tes\u00bet Subject\" (comment) <x@x.example>");
        Assert.AreEqual(
          "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
          stringTemp);
      }
    }

    private void TestParseCommentStrictCore(string input) {
      Assert.AreEqual(input.Length, Reflect.InvokeStatic(MailNamespace() +
                    ".HeaderParserUtility", "ParseCommentStrict", input, 0,
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
    [Timeout(5000)]
    public void TestEncodedWords() {
      string par = "(";
      TestEncodedWordsPhrase("(sss) y", "(sss) =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("tes=dxx", "=?us-ascii?q?tes=dxx?=");
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
TestEncodedWordsPhrase("xy (sss)",
        "=?us-ascii?q?x?= =?us-ascii?q?y?= (sss)");
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
      TestEncodedWordsOne("=?utf-8?Q?" + Repeat("x", 200) + "?=",
                    "=?utf-8?Q?" + Repeat("x", 200) + "?=");
      TestEncodedWordsPhrase("=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=",
                    "=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=");
    }

    [TestMethod]
    public void TestHeaderParsingRfc2047() {
      string tmp = "=?utf-8?q??=\r\n \r\nABC";
      Assert.AreEqual(tmp, (string)Reflect.InvokeStatic(MailNamespace() +
                    ".Rfc2047", "DecodeEncodedWords", tmp, 0, tmp.Length,
                Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
                    "Unstructured")));
      tmp = "=?utf-8?q??=\r\n \r\n ABC";
      Assert.AreEqual(tmp, (string)Reflect.InvokeStatic(MailNamespace() +
                    ".Rfc2047", "DecodeEncodedWords", tmp, 0, tmp.Length,
                Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
                    "Unstructured")));
    }

    [TestMethod]
    [Timeout(5000)]
    public void TestHeaderParsing() {
      string tmp;
      tmp = " A Xxxxx: Yyy Zzz <x@x.example>;";
      if (tmp.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length,
                    null)) {
        Assert.Fail(tmp);
      }
      // just a local part in address
      if (0 != (int)Reflect.InvokeStatic(MailNamespace() + ".HeaderParser",
                    "ParseHeaderFrom", "\"Me\" <1234>", 0, 11, null)) {
        Assert.Fail(tmp);
      }
      tmp = "<x@x.invalid>";
      if (tmp.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length,
                    null)) {
        Assert.Fail(tmp);
      }
      tmp = "<x y@x.invalid>";  // local part is not a dot-atom
      if (0 != (int)Reflect.InvokeStatic(MailNamespace() + ".HeaderParser",
                    "ParseHeaderTo", tmp, 0, tmp.Length, null)) {
        Assert.Fail(tmp);
      }
      tmp = " <x@x.invalid>";
      if (tmp.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length,
                    null)) {
        Assert.Fail(tmp);
      }
      // Group syntax
      tmp = "G:;";
      if (tmp.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length,
                    null)) {
        Assert.Fail(tmp);
      }
      tmp = "G:a <x@x.example>;";
      if (tmp.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length,
                    null)) {
        Assert.Fail(tmp);
      }
      tmp = " A Xxxxx: ;";
      if (tmp.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length,
                    null)) {
        Assert.Fail(tmp);
      }
      tmp = " A Xxxxx: Yyy Zzz <x@x.example>, y@y.example, Ww <z@z.invalid>;";
      if (tmp.Length != (int)Reflect.InvokeStatic(MailNamespace() +
                 ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.Length,
                    null)) {
        Assert.Fail(tmp);
      }
    }

    public static void TestQuotedPrintableRoundTrip(byte[] bytes) {
      TestQuotedPrintableRoundTrip(bytes, 0);
      TestQuotedPrintableRoundTrip(bytes, 1);
      TestQuotedPrintableRoundTrip(bytes, 2);
    }

    public static string EncodeQP(byte[] bytes, int mode) {
      object enc = Reflect.Construct(
        MailNamespace() + ".QuotedPrintableEncoder",
        mode,
        false);
      var enc2 = (ICharacterEncoder)enc;
      int offset = 0;
      var aw = new ArrayWriter();
      while (true) {
        int c = -1;
        if (offset < bytes.Length) {
          c = ((int)bytes[offset++]) & 0xff;
        }
        if (enc2.Encode(c, aw) < 0) {
          break;
        }
      }
      return DataUtilities.GetUtf8String(aw.ToArray(), true);
    }

    public static string EncodeB64(byte[] bytes, int mode) {
      bool a=(mode%2) == 1;
      bool b=(mode%4) == 1;
      object enc = Reflect.Construct(
        MailNamespace() + ".Base64Encoder",
        a,
        false,
        b);
      var enc2 = (ICharacterEncoder)enc;
      int offset = 0;
      var aw = new ArrayWriter();
      while (true) {
        int c = -1;
        if (offset < bytes.Length) {
          c = ((int)bytes[offset++]) & 0xff;
        }
        if (enc2.Encode(c, aw) < 0) {
          break;
        }
      }
      return DataUtilities.GetUtf8String(aw.ToArray(), true);
    }

    public static void TestBase64RoundTrip(byte[] bytes, int mode) {
      string input = EncodeB64(bytes, mode);
      string msgString = "From: <test@example.com>\r\n" +
        "MIME-Version: 1.0\r\n" + "Content-Type: application/octet-stream\r\n" +
        "Content-Transfer-Encoding: base64\r\n\r\n" + input;
      Message msg = MessageTest.MessageFromString(msgString);
      AssertEqual(bytes, msg.GetBody(), input);
      msg = MessageTest.MessageFromString(msg.Generate());
      AssertEqual(bytes, msg.GetBody(), input);
    }

    public static void TestQuotedPrintableRoundTrip(byte[] bytes, int mode) {
      string input = EncodeQP(bytes, mode);
      string msgString = "From: <test@example.com>\r\n" +
        "MIME-Version: 1.0\r\n" + "Content-Type: application/octet-stream\r\n" +
        "Content-Transfer-Encoding: quoted-printable\r\n\r\n" + input;
      Message msg = MessageTest.MessageFromString(msgString);
      AssertEqual(bytes, msg.GetBody(), input);
      msg = MessageTest.MessageFromString(msg.Generate());
      AssertEqual(bytes, msg.GetBody(), input);
    }

    public static void TestQuotedPrintableRoundTrip(string str, int mode) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(str, true);
      string input = EncodeQP(bytes, mode);
      string msgString = "From: <test@example.com>\r\n" +
        "MIME-Version: 1.0\r\n" + "Content-Type: application/octet-stream\r\n" +
        "Content-Transfer-Encoding: quoted-printable\r\n\r\n" + input;
      Message msg = MessageTest.MessageFromString(msgString);
      AssertEqual(bytes, msg.GetBody(), input);
      msg = MessageTest.MessageFromString(msg.Generate());
      AssertEqual(bytes, msg.GetBody(), input);
    }

    private byte[] RandomBytes(Random rnd) {
      int count = 10 + rnd.Next(350);
      var arr = new byte[count];
      for (var i = 0; i < count; ++i) {
        arr[i] = (byte)rnd.Next(0x100);
      }
      return arr;
    }

    [TestMethod]
    public void TestRandomBase64() {
      var rnd = new Random();
      for (var i = 0; i < 5000; ++i) {
        TestBase64RoundTrip(RandomBytes(rnd), 0);
        TestBase64RoundTrip(RandomBytes(rnd), 1);
        TestBase64RoundTrip(RandomBytes(rnd), 2);
        TestBase64RoundTrip(RandomBytes(rnd), 3);
      }
    }

    [TestMethod]
    public void TestRandomQuotedPrintable() {
      var rnd = new Random();
      for (var i = 0; i < 5000; ++i) {
        TestQuotedPrintableRoundTrip(RandomBytes(rnd), 0);
        TestQuotedPrintableRoundTrip(RandomBytes(rnd), 1);
      }
    }

    [TestMethod]
    public void TestQuotedPrintableSpecific() {
      TestQuotedPrintableRoundTrip("T\u000best\r\nFrom Me", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\nGood ", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\nFrom ", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\nFromMe", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\nFroMe", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\nFrMe", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\nFMe", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\n.\r\nGood ", 2);
      TestQuotedPrintableRoundTrip("T\u000best\r\n.\r\nFrom Me", 2);
    }

    [TestMethod]
    public void TestReceivedHeader() {
      object parser = Reflect.InvokeStatic(MailNamespace() +
                    ".HeaderFieldParsers", "GetParser", "received");
      string test =
        "from x.y.example by a.b.example; Thu, 31 Dec 2012 00:00:00 -0100";
      if (test.Length != (int)Reflect.Invoke(parser, "Parse", test, 0,
                    test.Length, null)) {
        Assert.Fail(test);
      }
    }
  }
}
