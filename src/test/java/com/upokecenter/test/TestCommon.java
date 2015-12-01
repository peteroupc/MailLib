package com.upokecenter.test;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import org.junit.Assert;

  final class TestCommon {
private TestCommon() {
}
    public static String ToByteArrayString(byte[] bytes) {
      if (bytes == null) {
 return "null";
}
      StringBuilder sb = new StringBuilder();
      String hex = "0123456789ABCDEF";
      sb.append("new byte[] { ");
      for (int i = 0; i < bytes.length; ++i) {
        if (i > 0) {
          sb.append(",");  }
        if ((bytes[i] & 0x80) != 0) {
          sb.append("(byte)0x");
        } else {
          sb.append("0x");
        }
        sb.append(hex.charAt((bytes[i] >> 4) & 0xf));
        sb.append(hex.charAt(bytes[i] & 0xf));
      }
      sb.append("}");
      return sb.toString();
    }

    private static boolean ByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (arr1 == null) {
 return arr2 == null;
}
      if (arr2 == null) {
 return false;
}
      if (arr1.length != arr2.length) {
        return false;
      }
      for (int i = 0; i < arr1.length; ++i) {
        if (arr1[i] != arr2[i]) {
 return false;
}
      }
      return true;
    }

    public static void AssertByteArraysEqual(byte[] arr1, byte[] arr2) {
      if (!ByteArraysEqual(arr1, arr2)) {
     Assert.fail("Expected " + ToByteArrayString(arr1) + ", got " +
       ToByteArrayString(arr2));
      }
    }

    public static void AssertEqualsHashCode(Object o, Object o2) {
      if (o.equals(o2)) {
        if (!o2.equals(o)) {
          Assert.fail(
String.format(java.util.Locale.US,"%s equals %s but not vice versa",
o,
o2));
        }
        // Test for the guarantee that equal objects
        // must have equal hash codes
        if (o2.hashCode() != o.hashCode()) {
          // Don't use Assert.assertEquals directly because it has
          // quite a lot of overhead
          Assert.fail(
String.format(java.util.Locale.US,"%s and %s don't have equal hash codes",
o,
o2));
        }
      } else {
        if (o2.equals(o)) {
          Assert.fail(String.format(java.util.Locale.US,"%s does not equal %s but not vice versa",
o,
o2));
        }
      }
    }
  }
