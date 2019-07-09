using System;
using System.Collections.Generic;
using System.Text;

namespace MailLibTest {
  public static class DictUtility {
    public static IList<IDictionary<String, String>>
      DictList(
      params IDictionary<String, String>[] dicts) {
      if (dicts == null) {
        throw new ArgumentNullException(nameof(dicts));
      }
      IList<IDictionary<String, String>> list =
        new List<IDictionary<String, String>>();
      foreach (IDictionary<String, String> dict in dicts) {
        list.Add(dict);
      }
      return list;
    }

    public static IDictionary<string, string> MakeDict(params string[]
  keyvalues) {
      if (keyvalues == null) {
        throw new ArgumentNullException(nameof(keyvalues));
      }
      if (keyvalues.Length % 2 != 0) {
        throw new ArgumentException("keyvalues");
      }
      var dict = new Dictionary<string, string>();
      for (var i = 0; i < keyvalues.Length; i += 2) {
        dict.Add((string)keyvalues[i], keyvalues[i + 1]);
      }
      return dict;
    }




    public static string[] ParseJSONStringArray(string str) {
      int i = 0;
      var list = new List<string>();
      var sb = new StringBuilder();
      while (i < str.Length && (
         str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a || str[i] == 0x09)) {
        i++;
      }
      if (i >= str.Length || str[i] != '[') return null;
      i++;
      bool endValue = false;
      while (true) {
        while (i < str.Length && (
           str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a || str[i] == 0x09)) {
          i++;
        }
        if (i >= str.Length || (
          str[i] != ']' && str[i] != '"' &&
                                str[i] != ',')) return null;
        switch (str[i]) {
          case ']':
            i++;
            while (i < str.Length && (
              str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a || str[i] == 0x09)) {
              i++;
            }
            return i == str.Length ? list.ToArray() : null;
          case ',':
            if (!endValue) return null;
            endValue = false;
            break;
          case '"':
            i = ParseJSONString(str, i + 1, sb);
            if (i < 0) return null;
            endValue = true;
            list.Add(sb.ToString());
            break;
        }
      }
    }

    private static string ParseJSONString(string str) {
      if (str == null || str.Length < 2 || str[0] != '"')
        return null;
      var sb = new StringBuilder();
      if (ParseJSONString(str, 1, sb) == str.Length) {
        return sb.ToString();
      }
      return null;
    }

    private static int ParseJSONString(string str, int index, StringBuilder sb) {
      int c;
      sb.Remove(0, sb.Length);
      while (index < str.Length) {
        c = index >= str.Length ? -1 : str[index++];
        if (c == -1 || c < 0x20) {
          return -1;
        }
        if ((c & 0xfc00) == 0xd800 && index < str.Length &&
          (str[index] & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (str[index] & 0x3ff);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          return -1;
        }
        switch (c) {
          case '\\':
            c = index >= str.Length ? -1 : str[index++];
            switch (c) {
              case '\\':
                sb.Append('\\');
                break;
              case '/':
                // Now allowed to be escaped under RFC 8259
                sb.Append('/');
                break;
              case '\"':
                sb.Append('\"');
                break;
              case 'b':
                sb.Append('\b');
                break;
              case 'f':
                sb.Append('\f');
                break;
              case 'n':
                sb.Append('\n');
                break;
              case 'r':
                sb.Append('\r');
                break;
              case 't':
                sb.Append('\t');
                break;
              case 'u': { // Unicode escape
                  c = 0;
                  // Consists of 4 hex digits
                  for (var i = 0; i < 4; ++i) {
                    int ch = index >= str.Length ? -1 : str[index++];
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
                    sb.Append((char)c);
                  } else if ((c & 0xfc00) == 0xd800) {
                    int ch = index >= str.Length ? -1 : str[index++];
                    if (ch != '\\' ||
                       (index >= str.Length ? -1 : str[index++]) != 'u') {
                      return -1;
                    }
                    var c2 = 0;
                    for (var i = 0; i < 4; ++i) {
                      ch = index >= str.Length ? -1 : str[index++];
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
                      sb.Append((char)c);
                      sb.Append((char)c2);
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
              // pairs in the string or invalid encoding
              // in the stream
              if ((c >> 16) == 0) {
                sb.Append((char)c);
              } else {
                sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
                sb.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
              }
              break;
            }
        }
      }
      return -1;
    }
  }
}
