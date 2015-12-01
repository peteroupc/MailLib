/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test {
  internal static class TestCommon {
    public static string ToByteArrayString(byte[] bytes) {
      if (bytes == null) {
 return "null";
}
      var sb = new System.Text.StringBuilder();
      const string hex = "0123456789ABCDEF";
      sb.Append("new byte[] { ");
      for (var i = 0; i < bytes.Length; ++i) {
        if (i > 0) {
          sb.Append(","); }
        if ((bytes[i] & 0x80) != 0) {
          sb.Append("(byte)0x");
        } else {
          sb.Append("0x");
        }
        sb.Append(hex[(bytes[i] >> 4) & 0xf]);
        sb.Append(hex[bytes[i] & 0xf]);
      }
      sb.Append("}");
      return sb.ToString();
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
String.Format(
CultureInfo.InvariantCulture,
"{0} equals {1} but not vice versa",
o,
o2));
        }
        // Test for the guarantee that equal objects
        // must have equal hash codes
        if (o2.GetHashCode() != o.GetHashCode()) {
          // Don't use Assert.AreEqual directly because it has
          // quite a lot of overhead
          Assert.Fail(
String.Format(
CultureInfo.InvariantCulture,
"{0} and {1} don't have equal hash codes",
o,
o2));
        }
      } else {
        if (o2.Equals(o)) {
          Assert.Fail(String.Format(
CultureInfo.InvariantCulture,
"{0} does not equal {1} but not vice versa",
o,
o2));
        }
      }
    }
  }
}
