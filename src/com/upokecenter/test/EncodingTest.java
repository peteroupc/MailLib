package com.upokecenter.test; import com.upokecenter.util.*;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;
import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

  public class EncodingTest {

    public static String EscapeString(String str) {
      String hex = "0123456789abcdef";
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (c == 0x09) {
          sb.append("\\t");
        } else if (c == 0x0d) {
          sb.append("\\r");
        } else if (c == 0x0a) {
          sb.append("\\n");
        } else if (c == 0x22) {
          sb.append("\\\"");
        } else if (c == 0x5c) {
          sb.append("\\\\");
        } else if (c < 0x20 || c >= 0x7f) {
          sb.append("\\u");
          sb.append(hex.charAt((c >> 12) & 15));
          sb.append(hex.charAt((c >> 8) & 15));
          sb.append(hex.charAt((c >> 4) & 15));
          sb.append(hex.charAt((c) & 15));
        } else {
          sb.append(c);
        }
      }
      return sb.toString();
    }

    static boolean IsGoodAsciiMessageFormat(String str, boolean hasMessageType) {
      int lineLength = 0;
      int wordLength = 0;
      int index = 0;
      int endIndex = str.length();
      boolean headers = true;
      boolean colon = false;
      boolean hasNonWhiteSpace = false;
      boolean startsWithSpace = false;
      boolean hasLongWord = false;
      if (index == endIndex) {
        System.out.println("Message is empty");
        return false;
      }
      while (index < endIndex) {
        char c = str.charAt(index);
        if (index == 0 && (c == 0x20 || c == 0x09)) {
          System.out.println("Starts with whitespace");
          return false;
        }
        if (c >= 0x80) {
          System.out.println("Non-ASCII character (0x {0:X2})", (int)c);
          return false;
        }
        if (c == '\r' && index + 1 < endIndex && str.charAt(index + 1) == '\n') {
          index += 2;
          if (headers && lineLength == 0) {
            // Start of the body
            headers = false;
          } else if (headers && !hasNonWhiteSpace) {
            System.out.println("Line has only whitespace");
            return false;
          }
          lineLength = 0;
          wordLength = 0;
          colon = false;
          hasNonWhiteSpace = false;
          hasLongWord = false;
          startsWithSpace = false;
          if (index < endIndex && (str.charAt(index) == ' ' || str.charAt(index) == '\t')) {
            startsWithSpace = true;
          }
          continue;
        } else if (c == '\r' || c == '\n') {
          System.out.println("Bare CR or bare LF");
          return false;
        }
        if (headers && c == ':' && !colon && !startsWithSpace) {
          if (index + 1 >= endIndex) {
            System.out.println("Colon at end");
            return false;
          }
          if (index == 0 || str.charAt(index - 1) == 0x20 || str.charAt(index - 1) == 0x09 || str.charAt(index - 1) == 0x0d) {
            System.out.println("End of line, whitespace, or start of message before colon");
            return false;
          }
          if (str.charAt(index + 1) != 0x20) {
            String test = str.substring(Math.max(index + 2 - 30, 0),(Math.max(index + 2 - 30, 0))+(Math.min(index + 2, 30)));
            System.out.println("No space after header name and colon: (0x {0:X2}) [" + test + "] " + (index));
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
          hasLongWord |= (wordLength > 77) || (lineLength == wordLength && wordLength > 78);
        }
        if (c == 0) {
          System.out.println("CTL in message (0x {0:X2})", (int)c);
          return false;
        }
        if (headers && (c == 0x7f || (c < 0x20 && c != 0x09))) {
          System.out.println("CTL in header (0x {0:X2})", (int)c);
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
          System.out.println("Line length exceeded (" + maxLineLength + " " + (str.substring(index - 78,(index - 78)+(78))) + ")");
          return false;
        }
        ++index;
      }
      return true;
    }

    public static void Timeout(int duration, Action action) {
      String stackTrace = null;
      Object stackTraceLock = new Object();
      System.Threading.Thread thread = new Thread(new Runnable(){ public void run() {  {
          try {
            action();
          } catch (Exception ex) {
            synchronized(stackTraceLock) {
              stackTrace = ex.getClass().getFullName() + "\n" + ex.getMessage() + "\n" + ex.getStackTrace();
              System.Threading.Monitor.PulseAll(stackTraceLock);
            }
          }
        } }});
      thread.start();
      if (!thread.join(duration)) {
        thread.stop();
        String trace = null;
        synchronized(stackTraceLock) {
          while (stackTrace == null) {
            System.Threading.Monitor.Wait(stackTraceLock);
          }
          trace = stackTrace;
        }
        if (trace != null) {
          Assert.fail(trace);
        }
      }
    }

    static boolean IsRareMixerHeader(String hdrname) {
      return hdrname.equals("content-identifier") || hdrname.equals("x400-content-identifier") || hdrname.equals("x400-content-return") || hdrname.equals("x400-content-type") || hdrname.equals("x400-mts-identifier") || hdrname.equals("x400-originator") || hdrname.equals("x400-received") || hdrname.equals("x400-recipients") || hdrname.equals("x400-trace") || hdrname.equals("original-encoded-information-types") || hdrname.equals("conversion") || hdrname.equals("conversion-with-loss") || hdrname.equals("dl-expansion-history") || hdrname.equals("originator-return-address") ||
        hdrname.equals("discarded-x400-mts-extensions") || hdrname.equals("supersedes") || hdrname.equals("expires") ||
        hdrname.equals("content-return") ||
        hdrname.equals("autoforwarded") || hdrname.equals("generate-delivery-report") || hdrname.equals("incomplete-copy") || hdrname.equals("message-type") || hdrname.equals("discarded-x400-ipms-extensions") || hdrname.equals("autosubmitted") || hdrname.equals("prevent-nondelivery-report") || hdrname.equals("alternate-recipient") || hdrname.equals("disclose-recipients");
    }

    static String ToBase16(byte[] bytes) {
      StringBuilder sb = new StringBuilder();
      String hex = "0123456789ABCDEF";
      for (int i = 0; i < bytes.length; ++i) {
        sb.append(hex.charAt((bytes[i] >> 4) & 15));
        sb.append(hex.charAt((bytes[i]) & 15));
      }
      return sb.toString();
    }

    static String Hash(String str) {
      using (System.Security.Cryptography.SHA1Managed sha1=new System.Security.Cryptography.SHA1Managed()) {
        return ToBase16(sha1.ComputeHash(DataUtilities.GetUtf8Bytes(str, true)));
      }
    }

    public static String toString(byte[] array) {
      StringBuilder builder = new StringBuilder();
      boolean first = true;
      builder.append("[");
      for(byte v : array) {
        if (!first) {
          builder.append(", ");
        }
        builder.append((v).toString());
        first = false;
      }
      builder.append("]");
      return builder.toString();
    }

    public static void AssertEqual(byte[] expected, byte[] actual) {
      AssertEqual(expected, actual, "");
    }
    public static void AssertEqual(byte[] expected, byte[] actual, String msg) {
      if (expected.length() != actual.length()) {
        Assert.fail(
          "\nexpected: " + toString(expected) + "\n" +
          "\nwas:      " + toString(actual) + "\n" + msg);
      }
      for (int i = 0; i < expected.length(); ++i) {
        if (expected.charAt(i) != actual.charAt(i)) {
          Assert.fail(
            "\nexpected: " + toString(expected) + "\n" +
            "\nwas:      " + toString(actual) + "\n" + msg);
        }
      }
    }

    public static String Repeat(String s, int count) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < count; ++i) {
        sb.append(s);
      }
      return sb.toString();
    }

    /*
    private static void ReadQuotedPrintable(
      OutputStream outputStream,
      byte[] data,
      int offset,
      int count,
      boolean lenientLineBreaks,
      boolean unlimitedLineLength) {
      if (outputStream == null) {
        throw new NullPointerException("outputStream");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)data.length));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is more than " + Long.toString((long)data.length));
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length - offset)) + ") is less than " + Long.toString((long)count));
      }
      using (java.io.ByteArrayInputStream ms = new java.io.ByteArrayInputStream(data, offset, count)) {
        QuotedPrintableTransform t = new QuotedPrintableTransform(
          new WrappedStream(ms),
          lenientLineBreaks,
          unlimitedLineLength ? -1 : 76,
          false);
        while (true) {
          int c = t.read();
          if (c < 0) {
            return;
          }
          outputStream.write((byte)c);
        }
      }
    }

    public void TestDecodeQuotedPrintable(String input, String expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

        ReadQuotedPrintable(ms, bytes, 0, bytes.length, true, true);
        Assert.assertEquals(expectedOutput, DataUtilities.GetUtf8String(ms.toByteArray(), true));
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    public void TestFailQuotedPrintable(String input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.length, true, true);
          Assert.fail("Should have failed");
        } catch (MessageDataException ex) {
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    public void TestFailQuotedPrintableNonLenient(String input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.length, false, false);
          Assert.fail("Should have failed");
        } catch (MessageDataException ex) {
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    public void TestQuotedPrintable(String input, int mode, String expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      StringBuilder sb = new StringBuilder();
      QuotedPrintableEncoder enc=new QuotedPrintableEncoder(mode, false);
      enc.WriteToString(sb, bytes, 0, bytes.length);
      Assert.assertEquals(expectedOutput, sb.toString());
    }

    public void TestQuotedPrintable(String input, String a, String b, String c) {
      this.TestQuotedPrintable(input, 0, a);
      this.TestQuotedPrintable(input, 1, b);
      this.TestQuotedPrintable(input, 2, c);
    }

    public void TestQuotedPrintable(String input, String a) {
      this.TestQuotedPrintable(input, 0, a);
      this.TestQuotedPrintable(input, 1, a);
      this.TestQuotedPrintable(input, 2, a);
    }

    private void TestParseDomain(String str, String expected) {
      Assert.assertEquals(str.length(), HeaderParser.ParseDomain(str, 0, str.length(), null));
      Assert.assertEquals(expected, HeaderParserUtility.ParseDomain(str, 0, str.length()));
    }

    private void TestParseLocalPart(String str, String expected) {
      Assert.assertEquals(str.length(), HeaderParser.ParseLocalPart(str, 0, str.length(), null));
      Assert.assertEquals(expected, HeaderParserUtility.ParseLocalPart(str, 0, str.length()));
    }

    @Test
    public void TestParseDomainAndLocalPart() {
      this.TestParseDomain("x", "x");
      this.TestParseLocalPart("x", "x");
      this.TestParseLocalPart("\"" + "\"", "");
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

    public void TestWordWrapOne(String firstWord, String nextWords, String expected) {
      WordWrapEncoder ww=new WordWrapEncoder(firstWord);
      ww.AddString(nextWords);
      //System.out.println(ww.toString());
      Assert.assertEquals(expected, ww.toString());
    }

    @Test
    public void TestWordWrap() {
      this.TestWordWrapOne("Subject:", Repeat("xxxx ", 10) + "y", "Subject: " + Repeat("xxxx ", 10) + "y");
      this.TestWordWrapOne("Subject:", Repeat("xxxx ", 10), "Subject: " + Repeat("xxxx ", 9) + "xxxx");
    }

    @Test
    public void TestHeaderFields() {
      String testString = "Joe P Customer <customer@example.com>, Jane W Customer <jane@example.com>";
      Assert.assertEquals(testString.length(),
                      HeaderParser.ParseMailboxList(testString, 0, testString.length(), null));
    }

    @Test
    public void TestPunycodeDecode() {
      Assert.assertEquals(
  "e\u00e1",
  DomainUtility.PunycodeDecode("xn--e-ufa", 4, 9));
    }

    @Test public void TestAddressInternal() {
     try {
        new Address(null, "example.com");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address("local", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Address(EncodingTest.Repeat("local", 200), "example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      }

    static ITransform Transform(String str) {
      return new WrappedStream(new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(str, true)));
    }
    static ITransform Transform(byte[] bytes) {
      return new WrappedStream(new java.io.ByteArrayInputStream(bytes));
    }

    static byte[] GetBytes(ITransform trans) {
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

        int c = 0;
        while ((c = trans.read()) >= 0) {
          ms.write((byte)c);
        }
        return ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    @Test
    public void TestBase64() {
      AssertEqual(
        new byte[] {  0, 16, 1  },
        GetBytes(new Base64Transform(Transform("ABAB"),true)));
      AssertEqual(
        new byte[] {  0, 16, 1, 93  },
        GetBytes(new Base64Transform(Transform("ABABXX=="),true)));
      AssertEqual(
        new byte[] {  0, 16, 1, 93  },
        GetBytes(new Base64Transform(Transform("ABABXX==="),true)));
      AssertEqual(
        new byte[] {  0, 16, 1, 93  },
        GetBytes(new Base64Transform(Transform("ABABXX"),true)));
      AssertEqual(
        new byte[] {  169, 172, 241, 179, 7, 157, 114, 247, 235  },
        GetBytes(new Base64Transform(Transform("qazxswedcvfr"),true)));
      AssertEqual(
        new byte[] {  255, 239, 254, 103  },
        GetBytes(new Base64Transform(Transform("/+/+Zz=="),true)));
    }

    @Test
    public void TestLz4() {
      Lz4.Decompress(NormalizationData.CombiningClasses);
    }

    @Test
    public void TestArgumentValidation() {
      try {
        new Base64Encoder(false, false, false, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Base64Encoder(false,false,false,"xyz");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new WordWrapEncoder(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      try {
        Charsets.GetCharset(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), null, 0, 1);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] {  0  }, -1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 2, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 0, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 0, 2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new Base64Encoder(false, false, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), null, 0, 1);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] {  0  }, -1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 2, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 0, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 0, 2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new QuotedPrintableEncoder(0, false).WriteToString(new StringBuilder(), new byte[] {  0  }, 1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new BEncodingStringTransform(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    @Test
    public void TestShiftJIS() {
      // Adapted from the public domain Gonk test cases
      byte[] bytes;
      ICharset charset=Charsets.GetCharset("shift_jis");
      bytes = new byte[] {  (byte)0x82, 0x58, 0x33, 0x41, 0x61, 0x33, (byte)0x82, 0x60,
        (byte)0x82, (byte)0x81, 0x33, (byte)0xb1, (byte)0xaf, 0x33, (byte)0x83, 0x41,
        (byte)0x83, (byte)0x96, 0x33, (byte)0x82, (byte)0xa0, 0x33, (byte)0x93, (byte)0xfa,
        0x33, 0x3a, 0x3c, 0x33, (byte)0x81, (byte)0x80, (byte)0x81, (byte)0x8e,
        0x33, 0x31, (byte)0x82, 0x51, 0x41, 0x61, (byte)0x82, 0x51,
        (byte)0x82, 0x60, (byte)0x82, (byte)0x81, (byte)0x82, 0x51, (byte)0xb1, (byte)0xaf,
        (byte)0x82, 0x51, (byte)0x83, 0x41, (byte)0x83, (byte)0x96, (byte)0x82, 0x51,
        (byte)0x82, (byte)0xa0, (byte)0x82, 0x51, (byte)0x93, (byte)0xfa, (byte)0x82, 0x51,
        0x3a, 0x3c, (byte)0x82, 0x51, (byte)0x81, (byte)0x80, (byte)0x81, (byte)0x8e,
        (byte)0x82, 0x51  };
      "\uFF19\u0033\u0041\u0061\u0033\uFF21\uFF41\u0033\uFF71\uFF6F\u0033\u30A2\u30F6\u0033\u3042\u0033\u65E5\u0033\u003A\u003C\u0033\u00F7\u2103\u0033\u0031\uFF12\u0041\u0061\uFF12\uFF21\uFF41\uFF12\uFF71\uFF6F\uFF12\u30A2\u30F6\uFF12\u3042\uFF12\u65E5\uFF12\u003A\u003C\uFF12\u00F7\u2103\uFF12" expected=("\uFF19\u0033\u0041\u0061\u0033\uFF21\uFF41\u0033\uFF71\uFF6F\u0033\u30A2\u30F6\u0033\u3042\u0033\u65E5\u0033\u003A\u003C\u0033\u00F7\u2103\u0033\u0031\uFF12\u0041\u0061\uFF12\uFF21\uFF41\uFF12\uFF71\uFF6F\uFF12\u30A2\u30F6\uFF12\u3042\uFF12\u65E5\uFF12\u003A\u003C\uFF12\u00F7\u2103\uFF12");
      Assert.assertEquals(expected, charset.GetString(Transform(bytes)));
    }

    @Test
    public void TestIso2022JP() {
      byte[] bytes;
      ICharset charset=Charsets.GetCharset("iso-2022-jp");
      bytes = new byte[] {  0x20, 0x41, 0x61, 0x5c  };
      Assert.assertEquals(" Aa\\",charset.GetString(Transform(bytes)));
      // Illegal byte in escape middle state
      bytes = new byte[] {  0x1b, 0x28, 0x47, 0x21, 0x41, 0x31, 0x5c  };
      Assert.assertEquals("\ufffd\u0028\u0047!A1\\",charset.GetString(Transform(bytes)));
      // Katakana
      bytes = new byte[] {  0x1b, 0x28, 0x49, 0x21, 0x41, 0x31, 0x5c  };
      Assert.assertEquals("\uff61\uff81\uff71\uff9c",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x28, 0x49, 0x20, 0x41, 0x61, 0x5c  };
      Assert.assertEquals("\ufffd\uff81\ufffd\uff9c",charset.GetString(Transform(bytes)));
      // ASCII state via escape
      bytes = new byte[] {  0x1b, 0x28, 0x42, 0x20, 0x41, 0x61, 0x5c  };
      Assert.assertEquals(" Aa\\",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x28, 0x4a, 0x20, 0x41, 0x61, 0x5c  };
      Assert.assertEquals(" Aa\\",charset.GetString(Transform(bytes)));
      // JIS0208 state
      bytes = new byte[] {  0x1b, 0x24, 0x40, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23  };
      Assert.assertEquals("\u3000\u3001\u3002",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23  };
      Assert.assertEquals("\u3000\u3001\u3002",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x22, 0x0a, 0x21, 0x23  };
      Assert.assertEquals("\u3000\u3001\n!#",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x22, 0x1b, 0x28, 0x42, 0x21, 0x23  };
      Assert.assertEquals("\u3000\u3001!#",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x24, 0x42, 0x21, 0x21, 0x21, 0x0a, 0x21, 0x23, 0x22  };
      Assert.assertEquals("\u3000\ufffd\u3002\ufffd",charset.GetString(Transform(bytes)));
      // Illegal state
      bytes = new byte[] {  0x1b, 0x24, 0x4f, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23  };
      Assert.assertEquals("\ufffd\u0024\u004f!!!\u0022!#",charset.GetString(Transform(bytes)));
      // JIS0212 state
      bytes = new byte[] {  0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x30, 0x22, 0x31  };
      Assert.assertEquals("\u02d8\u02c7\u00b8",
                      charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x30, 0x0a, 0x22, 0x31  };
      Assert.assertEquals("\u02d8\u02c7\n\u00221",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x30, 0x1b, 0x28, 0x42, 0x22, 0x31  };
      Assert.assertEquals("\u02d8\u02c7\u00221",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x1b, 0x24, 0x28, 0x44, 0x22, 0x2f, 0x22, 0x0a, 0x22, 0x31, 0x23  };
      Assert.assertEquals("\u02d8\ufffd\u00b8\ufffd",charset.GetString(Transform(bytes)));
      // Illegal state
      bytes = new byte[] {  0x1b, 0x24, 0x28, 0x4f, 0x21, 0x21, 0x21, 0x22, 0x21, 0x23  };
      Assert.assertEquals("\ufffd\u0024\u0028\u004f!!!\u0022!#",charset.GetString(Transform(bytes)));
      // Illegal state at end
      bytes = new byte[] {  0x41, 0x1b  };
      Assert.assertEquals("A\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x41, 0x1b, 0x27  };
      Assert.assertEquals("A\ufffd'",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x41, 0x1b, 0x24  };
      Assert.assertEquals("A\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  0x41, 0x1b, 0x24, 0x28  };
      Assert.assertEquals("A\ufffd\u0028",charset.GetString(Transform(bytes)));
    }

    private static void AssertUtf8Equal(byte[] expected, byte[] actual) {
      Assert.assertEquals(DataUtilities.GetUtf8String(expected, true),
                      DataUtilities.GetUtf8String(actual, true));
    }

    private static String WrapHeader(String s) {
      return new WordWrapEncoder("").AddString(s).toString();
    }

    private void TestDowngradeDSNOne(String expected, String actual) {
      Assert.assertEquals(expected, Message.DowngradeRecipientHeaderValue(actual));
      String dsn;
      String expectedDSN;
      byte[] bytes;
      byte[] expectedBytes;
      boolean encap=(expected.StartsWith("=?"));
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

    @Test
    public void TestDowngradeDSN() {
      String hexstart = "\\x" + "{";
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

    @Test
    public void TestLanguageTags() {
      if(!(ParserUtility.IsValidLanguageTag("en-a-bb-x-y-z")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("0-xx-xx"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("9-xx-xx"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("a-xx-xx"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("x-xx-xx")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-US-u-islamcal")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("zh-CN-a-myext-x-private")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-a-myext-b-another")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("de-419-DE"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("a-DE"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("ar-a-aaa-b-bbb-a-ccc"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("qbb-us")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("zh-yue")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-us")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("e0-us"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-gb-1999")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-gb-1999-1998")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("en-gb-1999-1999"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-gb-oed")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("sr-Latn-RS")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("x-aaaaaaaaa-y-z"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("x-aaaaaaaa-y-z")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("a-b-x-y-z"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("a-bb-xx-yy-zz"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("a-bb-x-y-z"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("a-x-y-z"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("x-x-y-z")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("i-lojban"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("i-klingon")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("art-lojban")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("sgn-be-fr")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("no-bok")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("z-xx-xx"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-aaa-bbbb-x-xxx-yyy-zzz")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-aaa-bbbb-x-x-y-z")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("en-aaa-bbb"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("en-aaa-bbb-ccc"))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-aaa-bbbb")))Assert.fail();
      if(!(ParserUtility.IsValidLanguageTag("en-aaa-bbbb-cc")))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("en-aaa-bbb-"))Assert.fail();
      if(ParserUtility.IsValidLanguageTag("en-aaa-bbb-ccc-"))Assert.fail();
    }

    @Test
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
      this.TestDecodeQuotedPrintable(Repeat("a", 100), Repeat("a", 100));
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
      // ((output instanceof is) ? (is)output : null); the second '=' starts a soft line break,
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
      this.TestFailQuotedPrintableNonLenient(Repeat("a", 77));
      this.TestFailQuotedPrintableNonLenient(Repeat("=7F", 26));
      this.TestFailQuotedPrintableNonLenient("aa\r\n" + Repeat("a", 77));
      this.TestFailQuotedPrintableNonLenient("aa\r\n" + Repeat("=7F", 26));
    }

    public void TestEncodedWordsPhrase(String expected, String input) {
      Assert.assertEquals(
        expected + " <test@example.com>",
        HeaderFieldParsers.GetParser("from").DecodeEncodedWords(input + " <test@example.com>"));
    }

    public void TestEncodedWordsOne(String expected, String input) {
      String par = "(";
      Assert.assertEquals(expected, Rfc2047.DecodeEncodedWords(input, 0, input.length(), EncodedWordContext.Unstructured));
      Assert.assertEquals(
        "(" + expected + ") en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords("(" + input + ") en"));
      Assert.assertEquals(
        " (" + expected + ") en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (" + input + ") en"));
      Assert.assertEquals(
        " " + par + "comment " + par + "cmt " + expected + ")comment) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (comment (cmt " + input + ")comment) en"));
      Assert.assertEquals(
        " " + par + "comment " + par + "=?bad?= " + expected + ")comment) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (comment (=?bad?= " + input + ")comment) en"));
      Assert.assertEquals(
        " " + par + "comment " + par + "" + expected + ")comment) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (comment (" + input + ")comment) en"));
      Assert.assertEquals(
        " (" + expected + "()) en",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" (" + input + "()) en"));
      Assert.assertEquals(
        " en (" + expected + ")",
        HeaderFieldParsers.GetParser("content-language").DecodeEncodedWords(" en (" + input + ")"));
      Assert.assertEquals(
        expected,
        HeaderFieldParsers.GetParser("subject").DecodeEncodedWords(input));
    }

    @Test
    public void TestEncodedPhrase2() {
      Assert.assertEquals(
        "=?utf-8?Q?=28tes=C2=BEt=29_x=40x=2Eexample?=",
        HeaderFieldParsers.GetParser("subject").DowngradeFieldValue("(tes\u00bet) x@x.example"));
    }

    @Test
    public void TestToFieldDowngrading() {
      String sep=", ";
      Assert.assertEquals(
        "x <x@example.com>" + sep + "\"X\" <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, \"X\" <y@example.com>"));
      Assert.assertEquals(
        "x <x@example.com>" + sep + "=?utf-8?Q?=C2=BE?= <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, \u00be <y@example.com>"));
      Assert.assertEquals(
        "x <x@example.com>" + sep + "=?utf-8?Q?=C2=BE?= <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, \"\u00be\" <y@example.com>"));
      Assert.assertEquals(
        "x <x@example.com>" + sep + "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <y@example.com>",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("x <x@example.com>, x\u00e1 x x\u00e1 <y@example.com>"));
      Assert.assertEquals(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=C3=A1y=40example=2Ecom?= :;",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("g: x@example.com, x\u00e1y@example.com;"));
      Assert.assertEquals(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=40=CC=80=2Eexample?= :;",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("g: x@example.com, x@\u0300.example;"));
      Assert.assertEquals(
        "g: x@example.com" + sep + "x@xn--e-ufa.example;",
        HeaderFieldParsers.GetParser("to").DowngradeFieldValue("g: x@example.com, x@e\u00e1.example;"));
      Assert.assertEquals(
        "x <x@xn--e-ufa.example>",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x <x@e\u00e1.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@example.com>",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x\u00e1 x x\u00e1 <x@example.com>"));
      Assert.assertEquals(
        "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@xn--e-ufa.example>",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x\u00e1 x x\u00e1 <x@e\u00e1.example>"));
      Assert.assertEquals(
        "x =?utf-8?Q?x=C3=A1y=40example=2Ecom?= :;",
        HeaderFieldParsers.GetParser("sender").DowngradeFieldValue("x <x\u00e1y@example.com>"));
    }

    private static String EncodeComment(String str) {
      return Rfc2047.EncodeComment(str, 0, str.length());
    }

    @Test
    public void TestCommentsToWords() {
      Assert.assertEquals("(=?utf-8?Q?x?=)", EncodeComment("(x)"));
      Assert.assertEquals("(=?utf-8?Q?xy?=)", EncodeComment("(x\\y)"));
      Assert.assertEquals("(=?utf-8?Q?x_y?=)", EncodeComment("(x\r\n y)"));
      Assert.assertEquals("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\u00a0)"));
      Assert.assertEquals("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\\\u00a0)"));
      Assert.assertEquals("(=?utf-8?Q?x?=())", EncodeComment("(x())"));
      Assert.assertEquals("(=?utf-8?Q?x?=()=?utf-8?Q?y?=)", EncodeComment("(x()y)"));
      Assert.assertEquals("(=?utf-8?Q?x?=(=?utf-8?Q?ab?=)=?utf-8?Q?y?=)", EncodeComment("(x(a\\b)y)"));
      Assert.assertEquals("()", EncodeComment("()"));
      Assert.assertEquals("(test) x@x.example", HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(test) x@x.example"));
      Assert.assertEquals(
        "(=?utf-8?Q?tes=C2=BEt?=) x@x.example",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(tes\u00bet) x@x.example"));
      Assert.assertEquals(
        "(=?utf-8?Q?tes=C2=BEt?=) en",
        HeaderFieldParsers.GetParser("content-language").DowngradeFieldValue("(tes\u00bet) en"));
      Assert.assertEquals(
        "(comment) Test <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Test <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Tes\u00bet <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Tes\u00bet Subject <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) Test Sub\u00beject <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet\" <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet Subject\" <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Test Sub\u00beject\" <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("(comment) \"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.assertEquals(
        "Test <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Test <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Tes\u00bet <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Tes\u00bet Subject <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("Test Sub\u00beject <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet\" <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Test Sub\u00beject\" <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.assertEquals(
        "=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        HeaderFieldParsers.GetParser("from").DowngradeFieldValue("\"Tes\u00bet Subject\" (comment) <x@x.example>"));
    }

    private void TestParseCommentStrictCore(String input) {
      Assert.assertEquals(input,input.length(),HeaderParserUtility.ParseCommentStrict(input, 0, input.length()));
    }

    @Test
    public void TestPercentEncoding() {
      Assert.assertEquals(
        "test\u00be",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("test%c2%be")));
      Assert.assertEquals(
        "tesA",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%41")));
      Assert.assertEquals(
        "tesa",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%61")));
      Assert.assertEquals(
        "tes\r\na",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%0d%0aa")));
      Assert.assertEquals(
        "tes\r\na",
        Charsets.Utf8.GetString(new QEncodingStringTransform("tes=0d=0aa")));
      Assert.assertEquals(
        "tes%xx",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%xx")));
      Assert.assertEquals(
        "tes%dxx",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes%dxx")));
      Assert.assertEquals(
        "tes=dxx",
        Charsets.Utf8.GetString(new QEncodingStringTransform("tes=dxx")));
      Assert.assertEquals(
        "tes??x",
        Charsets.Utf8.GetString(new PercentEncodingStringTransform("tes\r\nx")));
    }

    @Test
    public void TestParseCommentStrict() {
      TestParseCommentStrictCore("(y)");
      TestParseCommentStrictCore("(e\\y)");
      TestParseCommentStrictCore("(a(b)c)");
      TestParseCommentStrictCore("()");
      TestParseCommentStrictCore("(x)");
    }
    @Test
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

    @Test
    public void TestEncodedWords() {
      String par = "(";
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
      this.TestEncodedWordsOne("=?utf-8?Q?" + Repeat("x", 200) + "?=", "=?utf-8?Q?" + Repeat("x", 200) + "?=");
      this.TestEncodedWordsPhrase("=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=", "=?x-undefined?q?abcde?= =?x-undefined?q?abcde?=");
    }

    @Test
    public void TestEucJP() {
      byte[] bytes;
      ICharset charset=Charsets.GetCharset("euc-jp");
      bytes = new byte[] {  (byte)0x8e  };
      Assert.assertEquals("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x8e, 0x21  };
      Assert.assertEquals("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x8e, (byte)0x8e, (byte)0xa1  };
      Assert.assertEquals("\ufffd\uff61",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x8f  };
      Assert.assertEquals("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x8f, 0x21  };
      Assert.assertEquals("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x8f, (byte)0xa1  };
      Assert.assertEquals("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x8f, (byte)0xa1, 0x21  };
      Assert.assertEquals("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x90  };
      Assert.assertEquals("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x90, 0x21  };
      Assert.assertEquals("\ufffd!",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0xa1  };
      Assert.assertEquals("\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0xa1, (byte)0xa1  };
      Assert.assertEquals("\u3000",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x90, (byte)0xa1, (byte)0xa1  };
      Assert.assertEquals("\ufffd\u3000",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0x90, (byte)0xa1, (byte)0xa1, (byte)0xa1  };
      Assert.assertEquals("\ufffd\u3000\ufffd",charset.GetString(Transform(bytes)));
      bytes = new byte[] {  (byte)0xa1, 0x21  };
      Assert.assertEquals("\ufffd!",charset.GetString(Transform(bytes)));
      String result;
      bytes = new byte[] {  0x15, (byte)0xf2, (byte)0xbf, (byte)0xdd, (byte)0xd7, 0x13, (byte)0xeb, (byte)0xcf, (byte)0x8e, (byte)0xd6, (byte)0x8f, (byte)0xec, (byte)0xe9, (byte)0x8f, (byte)0xd6, (byte)0xe6, (byte)0x8f, (byte)0xd3, (byte)0xa3, (byte)0x8e, (byte)0xd4, 0x66, (byte)0x8f, (byte)0xb9, (byte)0xfc, (byte)0x8e, (byte)0xb0, (byte)0x8f, (byte)0xea, (byte)0xd8, 0x29, (byte)0x8e, (byte)0xca, (byte)0x8e, (byte)0xd4, (byte)0xc9, (byte)0xb5, 0x1e, 0x09, (byte)0x8e, (byte)0xab, (byte)0xc2, (byte)0xc5, (byte)0x8e, (byte)0xa7, (byte)0x8e, (byte)0xb6, 0x3d, (byte)0xe1, (byte)0xd9, (byte)0xb7, (byte)0xd5, 0x7b, 0x05, (byte)0xe6, (byte)0xce, 0x1d, (byte)0x8f, (byte)0xbd, (byte)0xbe, (byte)0xd8, (byte)0xae, (byte)0x8e, (byte)0xc3, (byte)0x8f, (byte)0xc1, (byte)0xda, (byte)0xd5, (byte)0xbb, (byte)0xb2, (byte)0xa2, (byte)0xcc, (byte)0xd4, 0x42, (byte)0x8e, (byte)0xa2, (byte)0xed, (byte)0xd4, (byte)0xc6, (byte)0xe0, (byte)0x8f, (byte)0xe0, (byte)0xd5, (byte)0x8e, (byte)0xd8, (byte)0xb0, (byte)0xc8, (byte)0x8f, (byte)0xa2, (byte)0xb8, (byte)0xb9, (byte)0xf1, (byte)0x8e, (byte)0xb0, (byte)0xd9, (byte)0xc0, 0x13  };
      result = "\u0015\u9ba8\u6bbc\u0013\u8a85\uff96\u9ea8\u81f2\u7c67\uff94f\u5aba\uff70\u9b8a)\uff8a\uff94\u8b2c\u001e\u0009\uff6b\u59a5\uff67\uff76=\u75ca\u834a"+
        "{\u0005\u8004\u001d\u5fd1\u60bd\uff83\u6595\u5a9a\u65fa\u731bB\uff62\u8f33\u5948\u8ec1\uff98\u978d\u0384\u56fd\uff70\u62c8\u0013";
      Assert.assertEquals(result, (charset.GetString(Transform(bytes))));
    }

    @Test(timeout=5000)
    public void TestHeaderParsing() {
      String tmp;
      tmp=" A Xxxxx: Yyy Zzz <x@x.example>;";
      Assert.assertEquals(tmp.length(), HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
      // just a local part in address
      Assert.assertEquals(0,HeaderParser.ParseHeaderFrom("\"Me\" <1234>",0,11,null));
      tmp="<x@x.invalid>";
      Assert.assertEquals(tmp.length(), HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
      tmp="<x y@x.invalid>";  // local part is not a dot-atom
      Assert.assertEquals(0, HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
      tmp=" <x@x.invalid>";
      Assert.assertEquals(tmp.length(), HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
      tmp="=?utf-8?q??=\r\n \r\nABC";
      Assert.assertEquals(tmp, Rfc2047.DecodeEncodedWords(tmp, 0, tmp.length(), EncodedWordContext.Unstructured));
      tmp="=?utf-8?q??=\r\n \r\n ABC";
      Assert.assertEquals(tmp, Rfc2047.DecodeEncodedWords(tmp, 0, tmp.length(), EncodedWordContext.Unstructured));
      // Group syntax
      tmp="G:;";
      Assert.assertEquals(tmp.length(), HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
      tmp="G:a <x@x.example>;";
      Assert.assertEquals(tmp.length(), HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
      tmp=" A Xxxxx: ;";
      Assert.assertEquals(tmp.length(), HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
      tmp=" A Xxxxx: Yyy Zzz <x@x.example>, y@y.example, Ww <z@z.invalid>;";
      Assert.assertEquals(tmp.length(), HeaderParser.ParseHeaderTo(tmp, 0, tmp.length(), null));
    }

    @Test
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
      this.TestQuotedPrintable(Repeat("a", 75), Repeat("a", 75));
      this.TestQuotedPrintable(Repeat("a", 76), Repeat("a", 75) + "=\r\na");
      this.TestQuotedPrintable(Repeat("\u000c", 30), Repeat("=0C", 25) + "=\r\n" + Repeat("=0C", 5));
    }
    public void TestUtf7One(String input, String expected) {
      Assert.assertEquals(expected, Charsets.GetCharset("utf-7").GetString(EncodingTest.Transform(input)));
    }

    @Test
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

    @Test
    public void TestReceivedHeader() {
      IHeaderFieldParser parser=HeaderFieldParsers.GetParser("received");
      String test="from x.y.example by a.b.example; Thu, 31 Dec 2012 00:00:00 -0100";
      Assert.assertEquals(test.length(), parser.Parse(test, 0, test.length(), null));
    }
     */
  }
