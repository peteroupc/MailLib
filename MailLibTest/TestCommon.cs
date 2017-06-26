/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Globalization;
using NUnit.Framework;

namespace Test {
  internal static class TestCommon {
    public static string ToByteArrayString(byte[] bytes) {
      if (bytes == null) {
 return "null";
}
      var sb = new System.Text.StringBuilder();
      const string ValueHex = "0123456789ABCDEF";
      sb.Append("new byte[] { ");
      for (var i = 0; i < bytes.Length; ++i) {
        if (i > 0) {
          sb.Append(", "); }
        if ((bytes[i] & 0x80) != 0) {
          sb.Append("(byte)0x");
        } else {
          sb.Append("0x");
        }
        sb.Append(ValueHex[(bytes[i] >> 4) & 0xf]);
        sb.Append(ValueHex[bytes[i] & 0xf]);
      }
      sb.Append("}");
      return sb.ToString();
    }

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (var i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }

    private static string valueDigits = "0123456789";

    public static string LongToString(long longValue) {
      if (longValue == Int64.MinValue) {
 return "-9223372036854775808";
}
      if (longValue == 0L) {
 return "0";
}
      bool neg = longValue < 0;
      var chars = new char[24];
      var count = 0;
      if (neg) {
        chars[0] = '-';
        ++count;
        longValue = -longValue;
      }
      while (longValue != 0) {
        char digit = valueDigits[(int)(longValue % 10)];
        chars[count++] = digit;
        longValue /= 10;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    public static string IntToString(int value) {
      if (value == Int32.MinValue) {
 return "-2147483648";
}
      if (value == 0) {
 return "0";
}
      bool neg = value < 0;
      var chars = new char[24];
      var count = 0;
      if (neg) {
        chars[0] = '-';
        ++count;
        value = -value;
      }
      while (value != 0) {
        char digit = valueDigits[(int)(value % 10)];
        chars[count++] = digit;
        value /= 10;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    private static bool ByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (arr1 == null) {
 return arr2 == null;
}
      if (arr2 == null) {
 return false;
}
      if (arr1.Length != arr2.Length) {
        return false;
      }
      for (var i = 0; i < arr1.Length; ++i) {
        if (arr1[i] != arr2[i]) {
 return false;
}
      }
      return true;
    }

    public static void AssertByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (!ByteArraysEqual(arr1, arr2)) {
     Assert.Fail("Expected " + ToByteArrayString(arr1) + ", got " +
       ToByteArrayString(arr2));
      }
    }

    public static void AssertEqualsHashCode(Object o, Object o2) {
      if (o.Equals(o2)) {
        if (!o2.Equals(o)) {
          Assert.Fail(
  String.Empty + o + " equals " + o2 + " but not vice versa");
        }
        // Test for the guarantee that equal objects
        // must have equal hash codes
        if (o2.GetHashCode() != o.GetHashCode()) {
          // Don't use Assert.AreEqual directly because it has
          // quite a lot of overhead
          Assert.Fail(
  String.Empty + o + " and " + o2 + " don't have equal hash codes");
        }
      } else {
        if (o2.Equals(o)) {
          Assert.Fail(String.Empty + o + " does not equal " + o2 +
            " but not vice versa");
        }
        // At least check that GetHashCode doesn't throw
        try {
 o.GetHashCode();
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
        try {
 o2.GetHashCode();
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      }
    }
  }
}
