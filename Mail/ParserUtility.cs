/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Text;

using PeterO;

namespace PeterO.Mail {
  internal static class ParserUtility {
    public static bool EndsWith(string str, string suffix, int strStartPos) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (suffix == null) {
        throw new ArgumentNullException("suffix");
      }
      if (strStartPos < 0) {
        throw new ArgumentException("strStartPos (" + Convert.ToString((int)strStartPos, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (strStartPos > str.Length) {
        throw new ArgumentException("strStartPos (" + Convert.ToString((int)strStartPos, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      int endpos = suffix.Length + strStartPos;
      return (endpos <= str.Length) && str.Substring(strStartPos, endpos - strStartPos).Equals(suffix);
    }

    public static bool StartsWith(string str, string prefix) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (prefix == null) {
        throw new ArgumentNullException("prefix");
      }
      return (prefix.Length >= str.Length) && str.Substring(0, prefix.Length).Equals(prefix);
    }

    public static string TrimSpaceAndTab(string s) {
      return string.IsNullOrEmpty(s) ? s : TrimSpaceAndTabLeft(TrimSpaceAndTabRight(s));
    }

    public static string TrimSpaceAndTabLeft(string s) {
      if (string.IsNullOrEmpty(s)) {
        return s;
      }
      int index = 0;
      int valueSLength = s.Length;
      while (index < valueSLength) {
        char c = s[index];
        if (c != 0x09 && c != 0x20) {
          break;
        }
        ++index;
      }
      return (index == valueSLength) ? String.Empty : ((index == 0) ? s : s.Substring(index));
    }

    public static string TrimSpaceAndTabRight(string s) {
      if (string.IsNullOrEmpty(s)) {
        return s;
      }
      int index = s.Length - 1;
      while (index >= 0) {
        char c = s[index];
        if (c != 0x09 && c != 0x20) {
          return s.Substring(0, index + 1);
        }
        --index;
      }
      return String.Empty;
    }

    public static bool IsNullEmptyOrSpaceTabOnly(string str) {
      return String.IsNullOrEmpty(str) || SkipSpaceAndTab(str, 0, str.Length) == str.Length;
    }

    public static int ParseFWSLax(string str, int index, int endIndex, StringBuilder sb) {
      while (index < endIndex) {
        int tmp = index;
        // Skip CRLF
        if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
          index += 2;
        }
        // Add WSP
        if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
          if (sb != null) {
            sb.Append(str[index]);
          }
          ++index;
        } else {
          return tmp;
        }
      }
      return index;
    }

    // Wsp, a.k.a. 1*LWSP-char under RFC 822
    public static int SkipSpaceAndTab(string str, int index, int endIndex) {
      while (index < endIndex) {
        if (str[index] == 0x09 || str[index] == 0x20) {
          ++index;
        } else {
          break;
        }
      }
      return index;
    }

    /// <summary>Splits a string by a delimiter. If the string ends with the
    /// delimiter, the result will end with an empty string. If the string
    /// begins with the delimiter, the result will start with an empty string.</summary>
    /// <param name='s'>A string to split.</param>
    /// <param name='delimiter'>A string to signal where each substring
    /// begins and ends.</param>
    /// <returns>An array containing strings that are split by the delimiter.
    /// If s is null or empty, returns an array whose sole element is the empty
    /// string.</returns>
    /// <exception cref='System.ArgumentException'>Delimiter is null
    /// or empty.</exception>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='delimiter'/> is null.</exception>
    public static string[] SplitAt(string s, string delimiter) {
      if (delimiter == null) {
        throw new ArgumentNullException("delimiter");
      }
      if (delimiter.Length == 0) {
        throw new ArgumentException("delimiter is empty.");
      }
      if (string.IsNullOrEmpty(s)) {
        return new[] { String.Empty };
      }
      int index = 0;
      bool first = true;
      List<string> strings = null;
      int delimLength = delimiter.Length;
      while (true) {
        int index2 = s.IndexOf(delimiter, index, StringComparison.Ordinal);
        if (index2 < 0) {
          if (first) {
            return new[] { s };
          }
          strings = strings ?? (new List<string>());
          strings.Add(s.Substring(index));
          break;
        } else {
          first = false;
          string newstr = s.Substring(index, (index2)-index);
          strings = strings ?? (new List<string>());
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return (string[])strings.ToArray();
    }

    public static bool IsValidLanguageTag(string str) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      int index = 0;
      int endIndex = str.Length;
      int startIndex = index;
      if (index + 1 < endIndex) {
        char c1 = str[index];
        char c2 = str[index + 1];
        if (((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z')) && ((c2 >= 'A' && c2 <= 'Z') || (c2 >= 'a' && c2 <= 'z'))) {
          index += 2;
          if (index == endIndex) {
            return true;  // case AA
          }
          index += 2;
          // convert the language tag to lower case
          // to simplify handling
          str = DataUtilities.ToLowerCaseAscii(str);
          c1 = str[index];
          // Straightforward cases
          if (c1 >= 'a' && c1 <= 'z') {
            ++index;
            // case AAA
            if (index == endIndex) {
              return true;
            }
            c1 = str[index];  // get the next character
          }
          if (c1 == '-') { // case AA- or AAA-
            ++index;
            if (index + 2 == endIndex) {  // case AA-?? or AAA-??
              c1 = str[index];
              c2 = str[index];
              if ((c1 >= 'a' && c1 <= 'z') && (c2 >= 'a' && c2 <= 'z')) {
                return true;  // case AA-BB or AAA-BB
              }
            }
          }
          // match grandfathered language tags
          if (str.Equals("sgn-be-fr") || str.Equals("sgn-be-nl") || str.Equals("sgn-ch-de") ||
                   str.Equals("en-gb-oed")) {
            return true;
          }
          // More complex cases
          string[] splitString = SplitAt(str.Substring(startIndex, endIndex - startIndex), "-");
          if (splitString.Length == 0) {
            return false;
          }
          int splitIndex = 0;
          int splitLength = splitString.Length;
          int len = lengthIfAllAlpha(splitString[splitIndex]);
          if (len < 2 || len > 8) {
            return false;
          }
          if (len == 2 || len == 3) {
            ++splitIndex;
            // skip optional extended language subtags
            for (int i = 0; i < 3; ++i) {
              if (splitIndex < splitLength && lengthIfAllAlpha(splitString[splitIndex]) == 3) {
                if (i >= 1) {
                  // point 4 in section 2.2.2 renders two or
                  // more extended language subtags invalid
                  return false;
                }
                ++splitIndex;
              } else {
                break;
              }
            }
          }
          // optional script
          if (splitIndex < splitLength && lengthIfAllAlpha(splitString[splitIndex]) == 4) {
            ++splitIndex;
          }
          // optional region
          if (splitIndex < splitLength && lengthIfAllAlpha(splitString[splitIndex]) == 2) {
            ++splitIndex;
          } else if (splitIndex < splitLength && lengthIfAllDigit(splitString[splitIndex]) == 3) {
            ++splitIndex;
          }
          // variant, any number
          IList<string> variants = null;
          while (splitIndex < splitLength) {
            string curString = splitString[splitIndex];
            len = lengthIfAllAlphaNum(curString);
            if (len >= 5 && len <= 8) {
              variants = variants ?? (new List<string>());
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                return false;  // variant already exists; see point 5 in section 2.2.5
              }
              ++splitIndex;
            } else if (len == 4 && (curString[0] >= '0' && curString[0] <= '9')) {
              variants = variants ?? (new List<string>());
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                return false;  // variant already exists; see point 5 in section 2.2.5
              }
              ++splitIndex;
            } else {
              break;
            }
          }
          // extension, any number
          if (variants != null) {
            variants.Clear();
          }
          while (splitIndex < splitLength) {
            string curString = splitString[splitIndex];
            int curIndex = splitIndex;
            if (lengthIfAllAlphaNum(curString) == 1 &&
                      !curString.Equals("x")) {
              variants = variants ?? (new List<string>());
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                return false;  // extension already exists
              }
              ++splitIndex;
              bool havetoken = false;
              while (splitIndex < splitLength) {
                curString = splitString[splitIndex];
                len = lengthIfAllAlphaNum(curString);
                if (len >= 2 && len <= 8) {
                  havetoken = true;
                  ++splitIndex;
                } else {
                  break;
                }
              }
              if (!havetoken) {
                splitIndex = curIndex;
                break;
              }
            } else {
              break;
            }
          }
          // optional private use
          if (splitIndex < splitLength) {
            int curIndex = splitIndex;
            if (splitString[splitIndex].Equals("x")) {
              ++splitIndex;
              bool havetoken = false;
              while (splitIndex < splitLength) {
                len = lengthIfAllAlphaNum(splitString[splitIndex]);
                if (len >= 1 && len <= 8) {
                  havetoken = true;
                  ++splitIndex;
                } else {
                  break;
                }
              }
              if (!havetoken) {
                splitIndex = curIndex;
              }
            }
          }
          // check if all the tokens were used
          return splitIndex == splitLength;
        }
        if (c2 == '-' && (c1 == 'x' || c1 == 'X')) {
          // private use
          ++index;
          while (index < endIndex) {
            int count = 0;
            if (str[index] != '-') {
              return false;
            }
            ++index;
            while (index < endIndex) {
              c1 = str[index];
              if ((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') || (c1 >= '0' && c1 <= '9')) {
                ++count;
                if (count > 8) {
                  return false;
                }
              } else if (c1 == '-') {
                break;
              } else {
                return false;
              }
              ++index;
            }
            if (count < 1) {
              return false;
            }
          }
          return true;
        }
        if (c2 == '-' && (c1 == 'i' || c1 == 'I')) {
          // grandfathered language tags
          str = DataUtilities.ToLowerCaseAscii(str);
          return str.Equals("i-ami") || str.Equals("i-bnn") ||
          str.Equals("i-default") || str.Equals("i-enochian") ||
          str.Equals("i-hak") || str.Equals("i-klingon") ||
          str.Equals("i-lux") || str.Equals("i-navajo") ||
          str.Equals("i-mingo") || str.Equals("i-pwn") ||
          str.Equals("i-tao") || str.Equals("i-tay") ||
          str.Equals("i-tsu");
        }
        return false;
      }
      return false;
    }

    private static int lengthIfAllAlpha(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z'))) {
          return 0;
        }
      }
      return len;
    }

    private static int lengthIfAllAlphaNum(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') || (c1 >= '0' && c1 <= '9'))) {
          return 0;
        }
      }
      return len;
    }

    private static int lengthIfAllDigit(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!(c1 >= '0' && c1 <= '9')) {
          return 0;
        }
      }
      return len;
    }
  }
}
