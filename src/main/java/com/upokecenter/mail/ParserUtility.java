package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

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

    public static String[] SplitAt(String str, String delimiter) {
      if (delimiter == null) {
        throw new NullPointerException("delimiter");
      }
      if (delimiter.length() == 0) {
        throw new IllegalArgumentException("delimiter is empty.");
      }
      if (((str) == null || (str).length() == 0)) {
        return new String[] { "" };
      }
      int index = 0;
      boolean first = true;
      ArrayList<String> strings = null;
      int delimLength = delimiter.length();
      while (true) {
        int index2 = str.indexOf(delimiter, index);
        if (index2 < 0) {
          if (first) {
            String[] strret = new String[1];
            strret[0] = str;
            return strret;
          }
          strings = (strings == null) ? (new ArrayList<String>()) : strings;
          strings.add(str.substring(index));
          break;
        } else {
          first = false;
          String newstr = str.substring(index, (index)+(index2 - index));
          strings = (strings == null) ? (new ArrayList<String>()) : strings;
          strings.add(newstr);
          index = index2 + delimLength;
        }
      }
      return (String[])strings.toArray(new String[] { });
    }

    public static String Implode(String[] strings, String delim) {
      if (strings.length == 0) {
        return "";
      }
      if (strings.length == 1) {
        return strings[0];
      }
      StringBuilder sb = new StringBuilder();
      boolean first = true;
      for (String s : strings) {
        if (!first) {
          sb.append(delim);
        }
        sb.append(s);
        first = false;
      }
      return sb.toString();
    }
  }
