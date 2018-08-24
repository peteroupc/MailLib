package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;

import com.upokecenter.util.*;

  final class ParserUtility {
private ParserUtility() {
}
    private static String valueDigits = "0123456789";

    public static String IntToString(int value) {
      if (value == Integer.MIN_VALUE) {
        return "-2147483648";
      }
      if (value == 0) {
        return "0";
      }
      boolean neg = value < 0;
      char[] chars = new char[12];
      int count = 11;
      if (neg) {
        value = -value;
      }
      while (value > 43698) {
        int intdivvalue = value / 10;
        char digit = valueDigits.charAt((int)(value - (intdivvalue * 10)));
        chars[count--] = digit;
        value = intdivvalue;
      }
      while (value > 9) {
        int intdivvalue = (value * 26215) >> 18;
        char digit = valueDigits.charAt((int)(value - (intdivvalue * 10)));
        chars[count--] = digit;
        value = intdivvalue;
      }
      if (value != 0) {
        chars[count--] = valueDigits.charAt((int)value);
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 12 - count);
    }

    public static String TrimSpaceAndTab(String str) {
      if (((str) == null || (str).length() == 0)) {
        return str;
      }
      int index = 0;
      int valueSLength = str.length();
      while (index < valueSLength) {
        char c = str.charAt(index);
        if (c != 0x09 && c != 0x20) {
          break;
        }
        ++index;
      }
      if (index == valueSLength) {
 return "";
}
      int indexStart = index;
      index = str.length() - 1;
      while (index >= 0) {
        char c = str.charAt(index);
        if (c != 0x09 && c != 0x20) {
          int indexEnd = index + 1;
          if (indexEnd == indexStart) {
 return "";
}
          return (indexEnd == str.length() && indexStart == 0) ? str :
            str.substring(indexStart, (indexStart)+(indexEnd - indexStart));
        }
        --index;
      }
      return "";
    }

    // Wsp, a.k.a. 1*LWSP-char under RFC 822
    public static int SkipSpaceAndTab(String str, int index, int endIndex) {
      while (index < endIndex) {
        if (str.charAt(index) == 0x09 || str.charAt(index) == 0x20) {
          ++index;
        } else {
          break;
        }
      }
      return index;
    }
    }
