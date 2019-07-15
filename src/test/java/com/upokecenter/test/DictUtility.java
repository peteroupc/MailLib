package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

  public final class DictUtility {
private DictUtility() {
}
    private static final String HexAlphabet = "0123456789ABCDEF";

    public static List<Map<String, String>>
      DictList(
      Map<String, String>... dicts) {
      if (dicts == null) {
        throw new NullPointerException("dicts");
      }
      List<Map<String, String>> list =
        new ArrayList<Map<String, String>>();
      for (Map<String, String> dict : dicts) {
        list.add(dict);
      }
      return list;
    }

    public static Map<String, String> MakeDict(
      String... keyvalues) {
      if (keyvalues == null) {
        throw new NullPointerException("keyvalues");
      }
      if (keyvalues.length % 2 != 0) {
        throw new IllegalArgumentException("keyvalues");
      }
      HashMap<String, String> dict = new HashMap<String, String>();
      for (int i = 0; i < keyvalues.length; i += 2) {
        dict.put((String)keyvalues[i], keyvalues[i + 1]);
      }
      return dict;
    }

    public static String ToJSON(String[] arr) {
      StringBuilder sb = new StringBuilder().append("[");
      for (int i = 0; i < arr.length; ++i) {
        if (i > 0) {
          sb.append(",");
        }
        sb.append("\"");
        String str = arr[i];
        for (int j = 0; j < str.length(); ++j) {
          if ((str.charAt(j) & 0xfc00) == 0xdc00 ||
             ((str.charAt(j) & 0xfc00) == 0xd800 && (j == str.length() - 1 ||
             (str.charAt(j + 1) & 0xfc00) != 0xdc00))) {
            throw new IllegalArgumentException("arr is invalid");
          }
          if (str.charAt(j) == '\"') {
            sb.append("\\\"");
          } else if (str.charAt(j) == '\\') {
   sb.append("\\\\");
 } else if (str.charAt(j) == '\r') {
   sb.append("\\r");
 } else if (str.charAt(j) == '\n') {
   sb.append("\\n");
          } else if (str.charAt(j) < 0x20 || str.charAt(j) >= 0x7f) {
            int ch = (int)str.charAt(j);
            sb.append("\\u")
               .append(HexAlphabet.charAt((ch >> 12) & 15))
               .append(HexAlphabet.charAt((ch >> 8) & 15))
               .append(HexAlphabet.charAt((ch >> 4) & 15)).append(HexAlphabet.charAt(ch &
15));
          }
        }
        sb.append("\"");
      }
      return sb.append("]").toString();
    }

    public static String[] ParseJSONStringArray(String str) {
      int i = 0;
      ArrayList<String> list = new ArrayList<String>();
      StringBuilder sb = new StringBuilder();
      while (i < str.length() && (
         str.charAt(i) == 0x20 || str.charAt(i) == 0x0d || str.charAt(i) == 0x0a ||
         str.charAt(i) == 0x09)) {
        ++i;
      }
      if (i >= str.length() || str.charAt(i) != '[') {
        return null;
      }
      ++i;
      boolean endValue = false;
      while (true) {
        while (i < str.length() && (
           str.charAt(i) == 0x20 || str.charAt(i) == 0x0d || str.charAt(i) == 0x0a ||
           str.charAt(i) == 0x09)) {
          ++i;
        }
        if (i >= str.length() || (
          str.charAt(i) != ']' && str.charAt(i) != '"' && str.charAt(i) != 0x2c)) {
          return null;
        }
        switch (str.charAt(i)) {
          case ']':
            ++i;
            while (i < str.length() && (
              str.charAt(i) == 0x20 || str.charAt(i) == 0x0d || str.charAt(i) == 0x0a || str.charAt(i)
              == 0x09)) {
              ++i;
            }
            return i == str.length() ? list.ToArray() : null;
          case (char)0x2c:
            if (!endValue) {
              return null;
            }
            endValue = false;
            break;
          case '"':
            i = ParseJSONString(str, i + 1, sb);
            if (i < 0) {
              return null;
            }
            endValue = true;
            list.add(sb.toString());
            break;
        }
      }
    }

    private static String ParseJSONString(String str) {
      if (str == null || str.length() < 2 || str.charAt(0) != '"') {
        return null;
      }
      StringBuilder sb = new StringBuilder();
      return ParseJSONString(str, 1, sb) ==
              str.length() ? sb.toString() : null;
    }

    private static int ParseJSONString(
      String str,
      int index,
      StringBuilder sb) {
      int c;
      sb.delete(0, (0)+(sb.length()));
      while (index < str.length()) {
        c = index >= str.length() ? -1 : str.charAt(index++);
        if (c == -1 || c < 0x20) {
          return -1;
        }
        if ((c & 0xfc00) == 0xd800 && index < str.length() &&
          (str.charAt(index) & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (str.charAt(index) & 0x3ff);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          return -1;
        }
        switch (c) {
          case '\\':
            c = index >= str.length() ? -1 : str.charAt(index++);
            switch (c) {
              case '\\':
                sb.append('\\');
                break;
              case '/':
                // Now allowed to be escaped under RFC 8259
                sb.append('/');
                break;
              case '\"':
                sb.append('\"');
                break;
              case 'b':
                sb.append('\b');
                break;
              case 'f':
                sb.append('\f');
                break;
              case 'n':
                sb.append('\n');
                break;
              case 'r':
                sb.append('\r');
                break;
              case 't':
                sb.append('\t');
                break;
              case 'u': { // Unicode escape
                  c = 0;
                  // Consists of 4 hex digits
                  for (int i = 0; i < 4; ++i) {
                    int ch = index >= str.length() ? -1 : str.charAt(index++);
                    if (ch >= '0' && ch <= '9') {
                      c <<= 4;
                      c |= ch - '0';
                    } else if (ch >= 'A' && ch <= 'F') {
                      c <<= 4;
                      c |= ch + 10 - 'A';
                    } else if (ch >= 'a' && ch <= 'f') {
                      c <<= 4;
                      c |= ch + 10 - 'a';
                    } else {
                      return -1;
                    }
                  }
                  if ((c & 0xf800) != 0xd800) {
                    // Non-surrogate
                    sb.append((char)c);
                  } else if ((c & 0xfc00) == 0xd800) {
                    int ch = index >= str.length() ? -1 : str.charAt(index++);
                    if (ch != '\\' ||
                       (index >= str.length() ? -1 : str.charAt(index++)) != 'u') {
                      return -1;
                    }
                    int c2 = 0;
                    for (int i = 0; i < 4; ++i) {
                      ch = index >= str.length() ? -1 : str.charAt(index++);
                      if (ch >= '0' && ch <= '9') {
                        c2 <<= 4;
                        c2 |= ch - '0';
                      } else if (ch >= 'A' && ch <= 'F') {
                        c2 <<= 4;
                        c2 |= ch + 10 - 'A';
                      } else if (ch >= 'a' && ch <= 'f') {
                        c2 <<= 4;
                        c2 |= ch + 10 - 'a';
                      } else {
                        return -1;
                      }
                    }
                    if ((c2 & 0xfc00) != 0xdc00) {
                      return -1;
                    } else {
                      sb.append((char)c);
                      sb.append((char)c2);
                    }
                  } else {
                    return -1;
                  }
                  break;
                }
              default: {
                  // NOTE: Includes surrogate code
                  // units
                  return -1;
                }
            }
            break;
          case 0x22: // double quote
            return index;
          default: {
              // NOTE: Assumes the character reader
              // throws an error on finding illegal surrogate
              // pairs in the String or invalid encoding
              // in the stream
              if ((c >> 16) == 0) {
                sb.append((char)c);
              } else {
                sb.append((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
                sb.append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
              }
              break;
            }
        }
      }
      return -1;
    }
  }
