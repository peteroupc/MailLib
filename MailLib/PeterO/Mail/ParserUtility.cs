/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace PeterO.Mail {
  internal static class ParserUtility {
    private static string valueDigits = "0123456789";

    public static string IntToString(int value) {
      if (value == Int32.MinValue) {
        return "-2147483648";
      }
      if (value == 0) {
        return "0";
      }
      bool neg = value < 0;
      var chars = new char[12];
      var count = 11;
      if (neg) {
        value = -value;
      }
      while (value > 43698) {
        int intdivvalue = value / 10;
        char digit = valueDigits[(int)(value - (intdivvalue * 10))];
        chars[count--] = digit;
        value = intdivvalue;
      }
      while (value > 9) {
        int intdivvalue = (value * 26215) >> 18;
        char digit = valueDigits[(int)(value - (intdivvalue * 10))];
        chars[count--] = digit;
        value = intdivvalue;
      }
      if (value != 0) {
        chars[count--] = valueDigits[(int)value];
      }
      if (neg) {
        chars[count] = '-';
      } else {
        ++count;
      }
      return new String(chars, count, 12 - count);
    }

    public static string TrimSpaceAndTab(string str) {
      if (String.IsNullOrEmpty(str)) {
        return str;
      }
      var index = 0;
      int valueSLength = str.Length;
      while (index < valueSLength) {
        char c = str[index];
        if (c != 0x09 && c != 0x20) {
          break;
        }
        ++index;
      }
      if (index == valueSLength) {
        return String.Empty;
      }
      int indexStart = index;
      index = str.Length - 1;
      while (index >= 0) {
        char c = str[index];
        if (c != 0x09 && c != 0x20) {
          int indexEnd = index + 1;
          if (indexEnd == indexStart) {
            return String.Empty;
          }
          return (indexEnd == str.Length && indexStart == 0) ? str :
            str.Substring(indexStart, indexEnd - indexStart);
        }
        --index;
      }
      return String.Empty;
    }

    public static string[] SplitAt(string str, string delimiter) {
      if (delimiter == null) {
        throw new ArgumentNullException(nameof(delimiter));
      }
      if (delimiter.Length == 0) {
        throw new ArgumentException("delimiter is empty.");
      }
      if (String.IsNullOrEmpty(str)) {
        return new[] { String.Empty };
      }
      var index = 0;
      var first = true;
      List<string> strings = null;
      int delimLength = delimiter.Length;
      while (true) {
        int index2 = str.IndexOf(delimiter, index, StringComparison.Ordinal);
        if (index2 < 0) {
          if (first) {
            var strret = new string[1];
            strret[0] = str;
            return strret;
          }
          strings = strings ?? new List<string>();
          strings.Add(str.Substring(index));
          break;
        } else {
          first = false;
          string newstr = str.Substring(index, index2 - index);
          strings = strings ?? new List<string>();
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return (string[])strings.ToArray();
    }

    public static string Implode(string[] strings, string delim) {
      if (strings.Length == 0) {
        return String.Empty;
      }
      if (strings.Length == 1) {
        return strings[0];
      }
      var sb = new StringBuilder();
      var first = true;
      foreach (string s in strings) {
        if (!first) {
          sb.Append(delim);
        }
        sb.Append(s);
        first = false;
      }
      return sb.ToString();
    }
  }
}
