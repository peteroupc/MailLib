/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;
using System.Text;

using PeterO;

namespace PeterO.Mail {
  internal static class ParserUtility {
    public static string TrimAndCollapseSpaceAndTab(string str) {
      if (String.IsNullOrEmpty(str)) {
        return str;
      }
      StringBuilder builder = null;
      var index = 0;
      int leadIndex;
      // Skip leading whitespace, if any
      while (index < str.Length) {
        char c = str[index];
        if (c == 0x09 || c == 0x20) {
          builder = builder ?? (new StringBuilder());
          ++index;
        } else {
          break;
        }
      }
      leadIndex = index;
      while (index < str.Length) {
        int si = index;
        char c = str[index++];
        bool isspace = c == 0x20;
        var count = 0;
        while (c == 0x09 || c == 0x20) {
          ++count;
          if (index < str.Length) {
            c = str[index++];
          } else {
            break;
          }
        }
        if (count > 0) {
          if (builder == null) {
            builder = new StringBuilder();
            builder.Append(str.Substring(leadIndex, si));
          }
          if (c != 0x09 && c != 0x20) {
            builder.Append(' ');
            builder.Append(c);
          }
        } else {
          if (builder != null) {
            builder.Append(c);
          }
        }
      }
      return (builder == null) ? str : builder.ToString();
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ParserUtility.SplitAt(System.String,System.String)"]/*'/>
    public static string[] SplitAt(string str, string delimiter) {
      if (delimiter == null) {
        throw new ArgumentNullException("delimiter");
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
          strings = strings ?? (new List<string>());
          strings.Add(str.Substring(index));
          break;
        } else {
          first = false;
          string newstr = str.Substring(index, (index2)-index);
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
      var index = 0;
      int endIndex = str.Length;
      int startIndex = index;
      if (index + 1 < endIndex) {
        char c1 = str[index];
        char c2 = str[index + 1];
        if (((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z')) && ((c2
          >= 'A' && c2 <= 'Z') || (c2 >= 'a' && c2 <= 'z'))) {
          index += 2;
          if (index == endIndex) {
             // case AA: a 2-letter language
            return true;
          }
          index += 2;
          // convert the language tag to lower case
          // to simplify handling
          str = DataUtilities.ToLowerCaseAscii(str);
          c1 = str[index];
          // Straightforward cases
          if (c1 >= 'a' && c1 <= 'z') {
            ++index;
            // case AAA: a 3-character language
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
                // case AA-BB or AAA-BB: BB is a 2-letter region
                return true;
              }
            }
          }
          // match grandfathered language tags
          if (str.Equals("sgn-be-fr") || str.Equals("sgn-be-nl") ||
            str.Equals("sgn-ch-de") || str.Equals("en-gb-oed")) {
            return true;
          }
          // More complex cases
          string[] splitString = SplitAt(
str.Substring(startIndex, endIndex - startIndex),
"-");
          if (splitString.Length == 0) {
            return false;
          }
          var splitIndex = 0;
          int splitLength = splitString.Length;
          int len = LengthIfAllAlpha(splitString[splitIndex]);
          if (len < 2 || len > 8) {
            return false;
          }
          if (len == 2 || len == 3) {
            ++splitIndex;
            // skip optional extended language subtags
            for (int i = 0; i < 3; ++i) {
              if (splitIndex < splitLength &&
                LengthIfAllAlpha(splitString[splitIndex]) == 3) {
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
          if (splitIndex < splitLength &&
            LengthIfAllAlpha(splitString[splitIndex]) == 4) {
            ++splitIndex;
          }
          // optional region
          if (splitIndex < splitLength &&
            LengthIfAllAlpha(splitString[splitIndex]) == 2) {
            ++splitIndex;
          } else if (splitIndex < splitLength &&
            LengthIfAllDigit(splitString[splitIndex]) == 3) {
            ++splitIndex;
          }
          // variant, any number
          IList<string> variants = null;
          while (splitIndex < splitLength) {
            string curString = splitString[splitIndex];
            len = LengthIfAllAlphaNum(curString);
            if (len >= 5 && len <= 8) {
              variants = variants ?? (new List<string>());
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                 // variant already exists; see point 5 in section
                // 2.2.5
         return false;
              }
              ++splitIndex;
         } else if (len == 4 &&
              (curString[0] >= '0' && curString[0] <= '9')) {
              variants = variants ?? (new List<string>());
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                 // variant already exists; see point 5 in section
                // 2.2.5
         return false;
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
            if (LengthIfAllAlphaNum(curString) == 1 &&
                    !curString.Equals("x")) {
              variants = variants ?? (new List<string>());
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                return false;  // extension already exists
              }
              ++splitIndex;
              var havetoken = false;
              while (splitIndex < splitLength) {
                curString = splitString[splitIndex];
                len = LengthIfAllAlphaNum(curString);
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
              var havetoken = false;
              while (splitIndex < splitLength) {
                len = LengthIfAllAlphaNum(splitString[splitIndex]);
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
            var count = 0;
            if (str[index] != '-') {
              return false;
            }
            ++index;
            while (index < endIndex) {
              c1 = str[index];
              if ((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') ||
                (c1 >= '0' && c1 <= '9')) {
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

    private static int LengthIfAllAlpha(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z'))) {
          return 0;
        }
      }
      return len;
    }

    private static int LengthIfAllAlphaNum(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') || (c1
          >= '0' && c1 <= '9'))) {
          return 0;
        }
      }
      return len;
    }

    private static int LengthIfAllDigit(string str) {
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
