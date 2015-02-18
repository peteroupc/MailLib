package com.upokecenter.test; import com.upokecenter.util.*;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

  public class EncodingTest {
    public static String MailNamespace() {
      return Message.class.getPackage().getName();
    }

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

    static boolean IsGoodAsciiMessageFormat(String str, boolean
      hasMessageType) {
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
          System.out.println("Non-ASCII character (0x" + ToBase16(new byte[] { (byte)c  }) + ")");
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
          startsWithSpace |= index < endIndex && (str.charAt(index) == ' ' ||
            str.charAt(index) == '\t');
          continue;
        }
        if (c == '\r' || c == '\n') {
          System.out.println("Bare CR or bare LF");
          return false;
        }
        if (headers && c == ':' && !colon && !startsWithSpace) {
          if (index + 1 >= endIndex) {
            System.out.println("Colon at end");
            return false;
          }
          if (index == 0 || str.charAt(index - 1) == 0x20 || str.charAt(index - 1) == 0x09||
            str.charAt(index - 1) == 0x0d) {
System.out.println("End of line, whitespace, or start of message before colon");
            return false;
          }
          if (str.charAt(index + 1) != 0x20) {
            String test = str.substring(Math.max(index + 2 - 30, 0), (Math.max(index + 2 - 30, 0))+(Math.min(index + 2, 30)));
  System.out.println("No space after header name and colon: (0x {0:X2}) [" +
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
   System.out.println("CTL in message (0x" + ToBase16(new byte[] { (byte)c  }) +
            ")");
          return false;
        }
        if (headers && (c == 0x7f || (c < 0x20 && c != 0x09))) {
    System.out.println("CTL in header (0x" + ToBase16(new byte[] { (byte)c  }) +
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
          System.out.println("Line length exceeded (" + maxLineLength + " " +
            (str.substring(index - 78,(index - 78)+(78))) + ")");
          return false;
        }
        ++index;
      }
      return true;
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

    public static String toString(byte[] array) {
      StringBuilder builder = new StringBuilder();
      boolean first = true;
      builder.append("[");
      for (byte v : array) {
        int vi = ((int)v) & 0xff;
        if (!first) {
          builder.append(", ");
        }
        builder.append(Integer.toString((int)vi));
        first = false;
      }
      builder.append("]");
      return builder.toString();
    }

    public static void AssertEqual(byte[] expectedBytes, byte[] actualBytes) {
      AssertEqual(expectedBytes, actualBytes, "");
    }
    public static void AssertEqual(byte[] expectedBytes, byte[] actualBytes,
      String msg) {
      if (expectedBytes.length != actualBytes.length) {
        Assert.fail("\nexpected: " + toString(expectedBytes) + "\n" +
          "\nwas:      " + toString(actualBytes) + "\n" + msg);
      }
      for (int i = 0; i < expectedBytes.length; ++i) {
        if (expectedBytes[i] != actualBytes[i]) {
          Assert.fail("\nexpected: " + toString(expectedBytes) + "\n" +
            "\nwas:      " + toString(actualBytes) + "\n" + msg);
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

    // -----------------------------------------------------------

    private static void ReadQuotedPrintable(OutputStream outputStream,
      byte[] data, int offset,
      int count, boolean lenientLineBreaks,
      boolean unlimitedLineLength) {
      if (outputStream == null) {
        throw new NullPointerException("outputStream");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
    throw new IllegalArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          data.length);
      }
      if (count < 0) {
      throw new IllegalArgumentException("count (" + count + ") is less than " +
          "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + count + ") is more than " +
          data.length);
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" +
          (data.length - offset) + ") is less than " + count);
      }
      java.io.ByteArrayInputStream ms = new java.io.ByteArrayInputStream(data, offset, count);
      try {
    Object t = Reflect.Construct(MailNamespace() +
          ".QuotedPrintableTransform" , Reflect.Construct(MailNamespace() +
            ".WrappedStream" , ms), lenientLineBreaks,
          unlimitedLineLength ? -1 : 76, false);
        Object readByteMethod = Reflect.GetMethod(t, "ReadByte");
        readByteMethod = (readByteMethod == null) ? ((Reflect.GetMethod(t, "read"))) : readByteMethod;
        while (true) {
          int c = (Integer)Reflect.InvokeMethod(t, readByteMethod);
          if (c < 0) {
            return;
          }
          outputStream.write((byte)c);
        }
      } catch (IOException ex) {
        Assert.fail(ex.getMessage());
      }
    }

    public static void TestDecodeQuotedPrintable(String input, String
      expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms = null;
try {
ms = new java.io.ByteArrayOutputStream();

        ReadQuotedPrintable(ms, bytes, 0, bytes.length, true, true);
        Assert.assertEquals(expectedOutput,
          DataUtilities.GetUtf8String(ms.toByteArray(), true));
}
finally {
try { if (ms != null)ms.close(); } catch (java.io.IOException ex) {}
}
    }

    public static void TestFailQuotedPrintable(String input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms = null;
try {
ms = new java.io.ByteArrayOutputStream();

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
try { if (ms != null)ms.close(); } catch (java.io.IOException ex) {}
}
    }

    public static void TestFailQuotedPrintableNonLenient(String input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms = null;
try {
ms = new java.io.ByteArrayOutputStream();

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
try { if (ms != null)ms.close(); } catch (java.io.IOException ex) {}
}
    }

    public static void TestQuotedPrintable(String input, int mode, String
      expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      StringBuilder sb = new StringBuilder();
      Object enc = Reflect.Construct(MailNamespace() +
        ".QuotedPrintableEncoder" , mode, false);
      Reflect.Invoke(enc, "WriteToString", sb, bytes, 0, bytes.length);
      Assert.assertEquals(expectedOutput, sb.toString());
    }

  public void TestQuotedPrintable(String input, String a, String b, String
      c) {
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, b);
      TestQuotedPrintable(input, 2, c);
    }

    public void TestQuotedPrintable(String input, String a) {
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, a);
      TestQuotedPrintable(input, 2, a);
    }

    private void TestParseDomain(String str, String expected) {
      if (!(str.length() == (Integer)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParser" , "ParseDomain" , str, 0, str.length(), null)))Assert.fail();
      Assert.assertEquals(expected, (String)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParserUtility" , "ParseDomain" , str, 0, str.length()));
    }

    private void TestParseLocalPart(String str, String expected) {
      if (!(str.length() == (Integer)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParser" , "ParseLocalPart" , str, 0, str.length(), null)))Assert.fail();
      Assert.assertEquals(expected, (String)Reflect.InvokeStatic(MailNamespace()+
        ".HeaderParserUtility" , "ParseLocalPart" , str, 0, str.length()));
    }

    @Test
    public void TestParseDomainAndLocalPart() {
      TestParseDomain("x", "x");
      TestParseLocalPart("x", "x");
      TestParseLocalPart("\"" + "\"", "");
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

    public static void TestWordWrapOne(String firstWord, String nextWords,
      String expected) {
Object ww = Reflect.Construct(MailNamespace() + ".WordWrapEncoder",
        firstWord);
      Reflect.Invoke(ww, "AddString", nextWords);
      Assert.assertEquals(expected, ww.toString());
    }

    @Test
    public void TestWordWrap() {
      TestWordWrapOne("Subject:" , Repeat("xxxx " , 10) + "y" , "Subject: "+
        Repeat("xxxx " , 10) + "y");
      TestWordWrapOne("Subject:" , Repeat("xxxx " , 10), "Subject: " +
        Repeat("xxxx " , 9) + "xxxx");
    }

    @Test
    public void TestHeaderFields() {
      String testString =
  "Joe P Customer <customer@example.com>, Jane W Customer <jane@example.com>"
        ;
      if (testString.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseMailboxList" , testString, 0,
        testString.length(), null)) {
 Assert.fail(testString);
}
    }

    @Test
    public void TestPunycodeDecode() {
      Assert.assertEquals(
  "e\u00e1",
  Reflect.InvokeStatic(Idna.class.getPackage().getName() + ".DomainUtility",
    "PunycodeDecode" , "xn--e-ufa" , 4, 9));
    }

    @Test
    public void TestAddressInternal() {
      try {
        Reflect.Construct(MailNamespace() + ".Address", null, "example.com");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".Address", "local", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".Address",
          EncodingTest.Repeat("local" , 200), "example.com");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    static Object Transform(String str) {
      return Reflect.Construct(MailNamespace() + ".WrappedStream" , new java.io.ByteArrayInputStream(DataUtilities.GetUtf8Bytes(str, true)));
    }
    static Object Transform(byte[] bytes) {
      return Reflect.Construct(MailNamespace() + ".WrappedStream" , new java.io.ByteArrayInputStream(bytes));
    }

    static byte[] GetBytes(Object trans) {
      Object readByteMethod = Reflect.GetMethod(trans, "ReadByte");
      readByteMethod = (readByteMethod == null) ? ((Reflect.GetMethod(trans, "read"))) : readByteMethod;
      java.io.ByteArrayOutputStream ms = null;
try {
ms = new java.io.ByteArrayOutputStream();

        int c = 0;
        while ((c = (Integer)Reflect.InvokeMethod(trans, readByteMethod)) >= 0) {
          ms.write((byte)c);
        }
        return ms.toByteArray();
}
finally {
try { if (ms != null)ms.close(); } catch (java.io.IOException ex) {}
}
    }

    @Test
    public void TestBase64() {
      AssertEqual(
        new byte[] { 0, 16, 1  }, GetBytes(Reflect.Construct(MailNamespace()+
          ".Base64Transform" , Transform("ABAB"), true)));
      AssertEqual(new byte[] { 0, 16, 1, 93  },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("ABABXX=="), true)));
      AssertEqual(
        new byte[] { 0, 16, 1, 93  },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("ABABXX==="), true)));
      AssertEqual(
        new byte[] { 0, 16, 1, 93  },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("ABABXX"), true)));
      AssertEqual(
        new byte[] { (byte)169, (byte)172, (byte)241, (byte)179, 7, (byte)157, 114, (byte)247, (byte)235  },
        GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("qazxswedcvfr"), true)));
      AssertEqual(
        new byte[] { (byte)255, (byte)239, (byte)254, 103  },
          GetBytes(Reflect.Construct(MailNamespace() + ".Base64Transform",
          Transform("/+/+Zz=="), true)));
    }
    @Test
    public void TestPercentEncoding() {
   Object utf8 = Reflect.GetFieldStatic(MailNamespace() + ".Charsets",
        "Utf8");
      Assert.assertEquals(
        "test\u00be", (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "test%c2%be")));
      Assert.assertEquals("tesA",
        (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%41")));
      Assert.assertEquals("tesa",
        (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%61")));
      Assert.assertEquals("tes\r\na",
        (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%0d%0aa")));
      Assert.assertEquals("tes\r\na",
        (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() + ".QEncodingStringTransform",
          "tes=0d=0aa")));
      Assert.assertEquals(
        "tes%xx", (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%xx")));
      Assert.assertEquals("tes%dxx",
        (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes%dxx")));
      Assert.assertEquals("tes=dxx",
        (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() + ".QEncodingStringTransform",
          "tes=dxx")));
      Assert.assertEquals(
        "tes??x", (String)Reflect.Invoke(utf8, "GetString",
          Reflect.Construct(MailNamespace() +
            ".PercentEncodingStringTransform" , "tes\r\nx")));
    }

    @Test
    public void TestArgumentValidation() {
      try {
        Reflect.Construct(MailNamespace() + ".Base64Encoder" , false, false,
          false, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".Base64Encoder" , false, false,
          false, "xyz");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Construct(MailNamespace() + ".WordWrapEncoder", (Object)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        CharsetsTest.GetCharset(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      Object encoder = Reflect.Construct(MailNamespace() + ".Base64Encoder"
        , false, false, false);
      try {
     Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), null, 0,
          1);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, -1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 2, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 0, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 0, 2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      encoder = Reflect.Construct(MailNamespace() +
        ".QuotedPrintableEncoder" , 0, false);
      try {
     Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), null, 0,
          1);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, -1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 2, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 0, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 0, 2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Reflect.Invoke(encoder, "WriteToString" , new StringBuilder(), new byte[] { 0  }, 1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
Reflect.Construct(MailNamespace() + ".BEncodingStringTransform",
          (Object)null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }

    private static void AssertUtf8Equal(byte[] expected, byte[] actual) {
      Assert.assertEquals(DataUtilities.GetUtf8String(expected, true),
                      DataUtilities.GetUtf8String(actual, true));
    }

    private static String WrapHeader(String s) {
      return Reflect.Invoke(Reflect.Construct(MailNamespace() +
        ".WordWrapEncoder" , ""), "AddString",
        s).toString();
    }

    private void TestDowngradeDSNOne(String expected, String actual) {
      Assert.assertEquals(expected, (String)Reflect.InvokeStatic(MailNamespace()+
        ".Message" , "DowngradeRecipientHeaderValue" , actual));
      String dsn;
      String expectedDSN;
      byte[] bytes;
      byte[] expectedBytes;
      boolean encap = (expected.startsWith("=?"));
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

    @Test
    public void TestDowngradeDSN() {
      String hexstart = "\\x" + "{";
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

    @Test
    public void TestLanguageTags() {
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-a-bb-x-y-z")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "0-xx-xx"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "9-xx-xx"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-xx-xx"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-xx-xx")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-US-u-islamcal")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "zh-CN-a-myext-x-private"
)))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-a-myext-b-another")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "de-419-DE"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-DE"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "ar-a-aaa-b-bbb-a-ccc"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "qbb-us")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "zh-yue")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-us")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "e0-us"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-1999")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-1999-1998")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-1999-1999"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-gb-oed")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "sr-Latn-RS")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-aaaaaaaaa-y-z"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-aaaaaaaa-y-z")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-b-x-y-z"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-bb-xx-yy-zz"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-bb-x-y-z"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "a-x-y-z"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "x-x-y-z")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "i-lojban"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "i-klingon")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "art-lojban")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "sgn-be-fr")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "no-bok")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "z-xx-xx"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag",
        "en-aaa-bbbb-x-xxx-yyy-zzz")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbbb-x-x-y-z")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb-ccc"))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbbb")))Assert.fail();
      if (!((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbbb-cc")))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb-"))Assert.fail();
      if ((Boolean)Reflect.InvokeStatic(MailNamespace() +
        ".ParserUtility" , "IsValidLanguageTag" , "en-aaa-bbb-ccc-"))Assert.fail();
    }

    @Test
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
      // ((output instanceof is) ? (is)output : null); the second '=' starts a soft line break,
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

    public static void TestEncodedWordsPhrase(String expected, String input) {
      Assert.assertEquals(expected + " <test@example.com>",
        DecodeHeaderField("from", input + " <test@example.com>"));
    }

    public static void TestEncodedWordsOne(String expected, String input) {
      String par = "(";
      Assert.assertEquals(expected, Reflect.InvokeStatic(MailNamespace() +
        ".Rfc2047" , "DecodeEncodedWords" , input, 0, input.length(),
        Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
        "Unstructured")));
      Assert.assertEquals(
        "(" + expected + ") en",
        DecodeHeaderField("content-language" , "("+ input + ") en"));
      Assert.assertEquals(" (" + expected + ") en",
        DecodeHeaderField("content-language", " (" + input + ") en"));
      Assert.assertEquals(" " + par + "comment " + par + "cmt " + expected +
        ")comment) en",
        DecodeHeaderField("content-language" , " (comment (cmt " + input +
          ")comment) en"));
      Assert.assertEquals(
        " " + par + "comment " + par + "=?bad?= " + expected + ")comment) en",
        DecodeHeaderField("content-language" , " (comment (=?bad?= " + input+
          ")comment) en"));
      Assert.assertEquals(
        " " + par + "comment " + par + "" + expected + ")comment) en",
DecodeHeaderField("content-language" , " (comment (" + input +
          ")comment) en"));
      Assert.assertEquals(
        " (" + expected + "()) en" , DecodeHeaderField("content-language",
          " (" + input + "()) en"));
      Assert.assertEquals(
        " en (" + expected + ")",
        DecodeHeaderField("content-language", " en (" + input + ")"));
      Assert.assertEquals(expected,
        DecodeHeaderField("subject", input));
    }

    @Test
    public void TestEncodedPhrase2() {
      Assert.assertEquals(
        "=?utf-8?Q?=28tes=C2=BEt=29_x=40x=2Eexample?=",
        DowngradeHeaderField("subject", "(tes\u00bet) x@x.example"));
    }

    @Test
    public void TestToFieldDowngrading() {
      String sep = ", ";
      Assert.assertEquals("x <x@example.com>" + sep + "\"X\" <y@example.com>",
        DowngradeHeaderField("to", "x <x@example.com>, \"X\" <y@example.com>"));
      Assert.assertEquals("x <x@example.com>" + sep +
        "=?utf-8?Q?=C2=BE?= <y@example.com>" , DowngradeHeaderField("to",
          "x <x@example.com>, \u00be <y@example.com>"));
      Assert.assertEquals("x <x@example.com>" + sep +
        "=?utf-8?Q?=C2=BE?= <y@example.com>",
  DowngradeHeaderField("to" , "x <x@example.com>, \"\u00be\" <y@example.com>"));
      Assert.assertEquals(
   "x <x@example.com>" + sep + "=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <y@example.com>",
        DowngradeHeaderField("to",
          "x <x@example.com>, x\u00e1 x x\u00e1 <y@example.com>"));
      Assert.assertEquals(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=C3=A1y=40example=2Ecom?= :;",
        DowngradeHeaderField("to", "g: x@example.com, x\u00e1y@example.com;"));
 Assert.assertEquals(
        "g =?utf-8?Q?x=40example=2Ecom=2C_x=40=CC=80=2Eexample?= :;",
        DowngradeHeaderField("to", "g: x@example.com, x@\u0300.example;"));
      Assert.assertEquals("g: x@example.com" + sep + "x@xn--e-ufa.example;",
        DowngradeHeaderField("to", "g: x@example.com, x@e\u00e1.example;"));
      Assert.assertEquals("x <x@xn--e-ufa.example>",
        DowngradeHeaderField("sender", "x <x@e\u00e1.example>"));
      Assert.assertEquals("=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@example.com>",
        DowngradeHeaderField("sender", "x\u00e1 x x\u00e1 <x@example.com>"));
      Assert.assertEquals("=?utf-8?Q?x=C3=A1_x_x=C3=A1?= <x@xn--e-ufa.example>",
      DowngradeHeaderField("sender" , "x\u00e1 x x\u00e1 <x@e\u00e1.example>"));
      Assert.assertEquals("x =?utf-8?Q?x=C3=A1y=40example=2Ecom?= :;",
        DowngradeHeaderField("sender", "x <x\u00e1y@example.com>"));
    }

    private static String EncodeComment(String str) {
      return (String)Reflect.InvokeStatic(MailNamespace() + ".Rfc2047",
        "EncodeComment" , str, 0, str.length());
    }

    private static String DowngradeHeaderField(String name, String value) {
      return (String)Reflect.Invoke(Reflect.InvokeStatic(MailNamespace() +
        ".HeaderFieldParsers" , "GetParser" , name),
        "DowngradeFieldValue", value);
    }

    private static String DecodeHeaderField(String name, String value) {
      return (String)Reflect.Invoke(Reflect.InvokeStatic(MailNamespace() +
        ".HeaderFieldParsers" , "GetParser" , name),
        "DecodeEncodedWords", value);
    }

    @Test
    public void TestCommentsToWords() {
      Assert.assertEquals("(=?utf-8?Q?x?=)", EncodeComment("(x)"));
      Assert.assertEquals("(=?utf-8?Q?xy?=)", EncodeComment("(x\\y)"));
      Assert.assertEquals("(=?utf-8?Q?x_y?=)", EncodeComment("(x\r\n y)"));
      Assert.assertEquals("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\u00a0)"));
      Assert.assertEquals("(=?utf-8?Q?x=C2=A0?=)", EncodeComment("(x\\\u00a0)"));
      Assert.assertEquals("(=?utf-8?Q?x?=())", EncodeComment("(x())"));
    Assert.assertEquals("(=?utf-8?Q?x?=()=?utf-8?Q?y?=)",
        EncodeComment("(x()y)"));
      Assert.assertEquals("(=?utf-8?Q?x?=(=?utf-8?Q?ab?=)=?utf-8?Q?y?=)",
        EncodeComment("(x(a\\b)y)"));
      Assert.assertEquals("()", EncodeComment("()"));
      Assert.assertEquals("(test) x@x.example" , DowngradeHeaderField("from",
        "(test) x@x.example"));
      Assert.assertEquals(
        "(=?utf-8?Q?tes=C2=BEt?=) x@x.example",
        DowngradeHeaderField("from" , "(tes\u00bet) x@x.example"));
      Assert.assertEquals("(=?utf-8?Q?tes=C2=BEt?=) en",
        DowngradeHeaderField("content-language", "(tes\u00bet) en"));
      Assert.assertEquals("(comment) Test <x@x.example>",
        DowngradeHeaderField("from", "(comment) Test <x@x.example>"));
      Assert.assertEquals("(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "(comment) Tes\u00bet <x@x.example>"));
      Assert.assertEquals("(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
   DowngradeHeaderField("from" , "(comment) Tes\u00bet Subject <x@x.example>"));
      Assert.assertEquals("(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
   DowngradeHeaderField("from" , "(comment) Test Sub\u00beject <x@x.example>"));
      Assert.assertEquals("(comment) =?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "(comment) \"Tes\u00bet\" <x@x.example>"));
      Assert.assertEquals("(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Tes\u00bet Subject\" <x@x.example>"));
      Assert.assertEquals("(comment) =?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Test Sub\u00beject\" <x@x.example>"));
   Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.assertEquals(
        "(comment) =?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        DowngradeHeaderField("from",
          "(comment) \"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        DowngradeHeaderField("from",
          "\"Tes\u00bet Subject\" (comment) <x@x.example>"));
      Assert.assertEquals("Test <x@x.example>",
        DowngradeHeaderField("from", "Test <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "Tes\u00bet <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        DowngradeHeaderField("from", "Tes\u00bet Subject <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        DowngradeHeaderField("from", "Test Sub\u00beject <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Tes=C2=BEt?= <x@x.example>",
        DowngradeHeaderField("from", "\"Tes\u00bet\" <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Tes=C2=BEt_Subject?= <x@x.example>",
        DowngradeHeaderField("from", "\"Tes\u00bet Subject\" <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Test_Sub=C2=BEject?= <x@x.example>",
        DowngradeHeaderField("from", "\"Test Sub\u00beject\" <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Tes=C2=BEt___Subject?= <x@x.example>",
        DowngradeHeaderField("from", "\"Tes\u00bet   Subject\" <x@x.example>"));
      Assert.assertEquals("=?utf-8?Q?Tes=C2=BEt_Subject?= (comment) <x@x.example>",
        DowngradeHeaderField("from",
          "\"Tes\u00bet Subject\" (comment) <x@x.example>"));
    }

    private void TestParseCommentStrictCore(String input) {
      Assert.assertEquals(input, input.length(), Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParserUtility" , "ParseCommentStrict" , input, 0,
        input.length()));
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

    @Test
    public void TestEncodedWords() {
      String par = "(";
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

    @Test
    public void TestHeaderParsingRfc2047() {
      String tmp = "=?utf-8?q??=\r\n \r\nABC";
      Assert.assertEquals(tmp, (String)Reflect.InvokeStatic(MailNamespace() +
        ".Rfc2047" , "DecodeEncodedWords" , tmp, 0, tmp.length(),
        Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
        "Unstructured")));
      tmp = "=?utf-8?q??=\r\n \r\n ABC";
      Assert.assertEquals(tmp, (String)Reflect.InvokeStatic(MailNamespace() +
        ".Rfc2047" , "DecodeEncodedWords" , tmp, 0, tmp.length(),
        Reflect.GetFieldStatic(MailNamespace() + ".EncodedWordContext",
        "Unstructured")));
    }

    @Test(timeout = 5000)
    public void TestHeaderParsing() {
      String tmp;
      tmp = " A Xxxxx: Yyy Zzz <x@x.example>;";
      if (tmp.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
      // just a local part in address
      if (0!=(Integer)Reflect.InvokeStatic(MailNamespace() + ".HeaderParser",
        "ParseHeaderFrom" , "\"Me\" <1234>" , 0, 11, null)) {
 Assert.fail(tmp);
}
      tmp = "<x@x.invalid>";
      if (tmp.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
      tmp = "<x y@x.invalid>";  // local part is not a dot-atom
      if (0!=(Integer)Reflect.InvokeStatic(MailNamespace() + ".HeaderParser",
        "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
      tmp = " <x@x.invalid>";
      if (tmp.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
      // Group syntax
      tmp = "G:;";
      if (tmp.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
      tmp = "G:a <x@x.example>;";
      if (tmp.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
      tmp = " A Xxxxx: ;";
      if (tmp.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
      tmp = " A Xxxxx: Yyy Zzz <x@x.example>, y@y.example, Ww <z@z.invalid>;";
      if (tmp.length() != (Integer)Reflect.InvokeStatic(MailNamespace() +
        ".HeaderParser" , "ParseHeaderTo" , tmp, 0, tmp.length(), null)) {
 Assert.fail(tmp);
}
    }

    @Test
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

    @Test
    public void TestReceivedHeader() {
      Object parser = Reflect.InvokeStatic(MailNamespace() +
        ".HeaderFieldParsers" , "GetParser" , "received");
      String test =
        "from x.y.example by a.b.example; Thu, 31 Dec 2012 00:00:00 -0100" ;
      if (test.length()!=(Integer)Reflect.Invoke(parser, "Parse" , test, 0,
        test.length(), null)) {
        Assert.fail(test);
      }
    }
  }
