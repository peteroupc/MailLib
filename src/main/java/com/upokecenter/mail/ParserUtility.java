package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.util.*;

import com.upokecenter.util.*;

  final class ParserUtility {
private ParserUtility() {
}
    public static boolean EndsWith(String str, String suffix, int strStartPos) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (suffix == null) {
        throw new NullPointerException("suffix");
      }
      if (strStartPos < 0) {
        throw new IllegalArgumentException("strStartPos (" + strStartPos +
          ") is less than " + "0");
      }
      if (strStartPos > str.length()) {
        throw new IllegalArgumentException("strStartPos (" + strStartPos +
          ") is more than " + str.length());
      }
      int endpos = suffix.length() + strStartPos;
      return (
endpos <= str.length()) && str.substring(
strStartPos, (
strStartPos)+(endpos - strStartPos)).equals(suffix);
    }

    public static boolean StartsWith(String str, String prefix) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (prefix == null) {
        throw new NullPointerException("prefix");
      }
      return (
prefix.length() >= str.length()) && str.substring(
0, (
0)+(prefix.length())).equals(prefix);
    }

    public static String TrimSpaceAndTab(String str) {
      return ((str) == null || (str).length() == 0) ? str :
        TrimSpaceAndTabLeft(TrimSpaceAndTabRight(str));
    }

    public static String TrimSpaceAndTabLeft(String str) {
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
      return (index == valueSLength) ? "" : ((index == 0) ? str :
        str.substring(index));
    }

    public static String TrimAndCollapseSpaceAndTab(String str) {
      if (((str) == null || (str).length() == 0)) {
        return str;
      }
      StringBuilder builder = null;
      int index = 0;
      // Skip leading whitespace, if any
      while (index < str.length()) {
        char c = str.charAt(index);
        if (c == 0x09 || c == 0x20) {
          builder = (builder == null) ? ((new StringBuilder())) : builder;
          ++index;
        } else {
          break;
        }
      }
      var leadIndex = index;
      while (index < str.length()) {
        int si = index;
        char c = str.charAt(index++);
        boolean isspace = c == 0x20;
        int count = 0;
        while (c == 0x09 || c == 0x20) {
          ++count;
          if (index < str.length()) {
            c = str.charAt(index++);
          } else {
            break;
          }
        }
        if (count > 0) {
          if (builder == null) {
            builder = new StringBuilder();
            builder.append(str.substring(leadIndex, (leadIndex)+(si)));
          }
          if (c != 0x09 && c != 0x20) {
            builder.append(' ');
            builder.append(c);
          }
        } else {
          if (builder != null) {
            builder.append(c);
          }
        }
      }
      return (builder == null) ? str : builder.toString();
    }

    public static String TrimSpaceAndTabRight(String str) {
      if (((str) == null || (str).length() == 0)) {
        return str;
      }
      int index = str.length() - 1;
      while (index >= 0) {
        char c = str.charAt(index);
        if (c != 0x09 && c != 0x20) {
          return str.substring(0, index + 1);
        }
        --index;
      }
      return "";
    }

    public static boolean IsNullEmptyOrSpaceTabOnly(String str) {
      return ((
str) == null || (
str).length() == 0) || SkipSpaceAndTab(
str,
0,
str.length()) == str.length();
    }

    public static int ParseFWSLax(
String str,
int index,
int endIndex,
StringBuilder sb) {
      while (index < endIndex) {
        int tmp = index;
        // Skip CRLF
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) == 10) {
          index += 2;
        }
        // Add WSP
        if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
          if (sb != null) {
            sb.append(str.charAt(index));
          }
          ++index;
        } else {
          return tmp;
        }
      }
      return index;
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

    /**
     * Splits a string by a delimiter. If the string ends with the delimiter, the
     * result will end with an empty string. If the string begins with the
     * delimiter, the result will start with an empty string.
     * @param str A string to split.
     * @param delimiter A string to signal where each substring begins and ends.
     * @return An array containing strings that are split by the delimiter. If the
     * string to split is null or empty, returns an array whose sole element
     * is the empty string.
     * @throws IllegalArgumentException Delimiter is null or empty.
     * @throws NullPointerException The parameter {@code delimiter} is null.
     */
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
          strings = (strings == null) ? ((new ArrayList<String>())) : strings;
          strings.add(str.substring(index));
          break;
        } else {
          first = false;
          String newstr = str.substring(index, (index)+((index2)-index));
          strings = (strings == null) ? ((new ArrayList<String>())) : strings;
          strings.add(newstr);
          index = index2 + delimLength;
        }
      }
      return strings.toArray(new String[] { });
    }

    public static boolean IsValidLanguageTag(String str) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      int index = 0;
      int endIndex = str.length();
      int startIndex = index;
      if (index + 1 < endIndex) {
        char c1 = str.charAt(index);
        char c2 = str.charAt(index + 1);
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
          c1 = str.charAt(index);
          // Straightforward cases
          if (c1 >= 'a' && c1 <= 'z') {
            ++index;
            // case AAA: a 3-character language
            if (index == endIndex) {
              return true;
            }
            c1 = str.charAt(index);  // get the next character
          }
          if (c1 == '-') { // case AA- or AAA-
            ++index;
            if (index + 2 == endIndex) {  // case AA-?? or AAA-??
              c1 = str.charAt(index);
              c2 = str.charAt(index);
              if ((c1 >= 'a' && c1 <= 'z') && (c2 >= 'a' && c2 <= 'z')) {
                // case AA-BB or AAA-BB: BB is a 2-letter region
                return true;
              }
            }
          }
          // match grandfathered language tags
          if (str.equals("sgn-be-fr") || str.equals("sgn-be-nl") ||
            str.equals("sgn-ch-de") || str.equals("en-gb-oed")) {
            return true;
          }
          // More complex cases
          String[] splitString = SplitAt(
str.substring(startIndex, (startIndex)+(endIndex - startIndex)),
"-");
          if (splitString.length == 0) {
            return false;
          }
          int splitIndex = 0;
          int splitLength = splitString.length;
          int len = lengthIfAllAlpha(splitString[splitIndex]);
          if (len < 2 || len > 8) {
            return false;
          }
          if (len == 2 || len == 3) {
            ++splitIndex;
            // skip optional extended language subtags
            for (int i = 0; i < 3; ++i) {
              if (splitIndex < splitLength &&
                lengthIfAllAlpha(splitString[splitIndex]) == 3) {
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
            lengthIfAllAlpha(splitString[splitIndex]) == 4) {
            ++splitIndex;
          }
          // optional region
          if (splitIndex < splitLength &&
            lengthIfAllAlpha(splitString[splitIndex]) == 2) {
            ++splitIndex;
          } else if (splitIndex < splitLength &&
            lengthIfAllDigit(splitString[splitIndex]) == 3) {
            ++splitIndex;
          }
          // variant, any number
          List<String> variants = null;
          while (splitIndex < splitLength) {
            String curString = splitString[splitIndex];
            len = lengthIfAllAlphaNum(curString);
            if (len >= 5 && len <= 8) {
              variants = (variants == null) ? ((new ArrayList<String>())) : variants;
              if (!variants.contains(curString)) {
                variants.add(curString);
              } else {
                 // variant already exists; see point 5 in section
                // 2.2.5
         return false;
              }
              ++splitIndex;
         } else if (len == 4 && (curString.charAt(0) >= '0' && curString.charAt(0) <= '9'
)) {
              variants = (variants == null) ? ((new ArrayList<String>())) : variants;
              if (!variants.contains(curString)) {
                variants.add(curString);
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
            variants.clear();
          }
          while (splitIndex < splitLength) {
            String curString = splitString[splitIndex];
            int curIndex = splitIndex;
            if (lengthIfAllAlphaNum(curString) == 1 &&
                    !curString.equals("x")) {
              variants = (variants == null) ? ((new ArrayList<String>())) : variants;
              if (!variants.contains(curString)) {
                variants.add(curString);
              } else {
                return false;  // extension already exists
              }
              ++splitIndex;
              boolean havetoken = false;
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
            if (splitString[splitIndex].equals("x")) {
              ++splitIndex;
              boolean havetoken = false;
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
            if (str.charAt(index) != '-') {
              return false;
            }
            ++index;
            while (index < endIndex) {
              c1 = str.charAt(index);
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
          return str.equals("i-ami") || str.equals("i-bnn") ||
          str.equals("i-default") || str.equals("i-enochian") ||
          str.equals("i-hak") || str.equals("i-klingon") ||
          str.equals("i-lux") || str.equals("i-navajo") ||
          str.equals("i-mingo") || str.equals("i-pwn") ||
          str.equals("i-tao") || str.equals("i-tay") ||
          str.equals("i-tsu");
        }
        return false;
      }
      return false;
    }

    private static int lengthIfAllAlpha(String str) {
      int len = (str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z'))) {
          return 0;
        }
      }
      return len;
    }

    private static int lengthIfAllAlphaNum(String str) {
      int len = (str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') || (c1
          >= '0' && c1 <= '9'))) {
          return 0;
        }
      }
      return len;
    }

    private static int lengthIfAllDigit(String str) {
      int len = (str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!(c1 >= '0' && c1 <= '9')) {
          return 0;
        }
      }
      return len;
    }
  }
