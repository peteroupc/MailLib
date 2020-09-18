using System;
using System.Collections.Generic;
using System.Text;

namespace MailLibTest {
  public static class DictUtility {
    private const string HexAlphabet = "0123456789ABCDEF";

    public static IList<IDictionary<String, String>> DictList(
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

    public static IDictionary<string, string> MakeDict(
      params string[] keyvalues) {
      if (keyvalues == null) {
        throw new ArgumentNullException(nameof(keyvalues));
      }
      if (keyvalues.Length % 2 != 0) {
        throw new ArgumentException("keyvalues has odd length");
      }
      var dict = new Dictionary<string, string>();
      for (var i = 0; i < keyvalues.Length; i += 2) {
        dict.Add((string)keyvalues[i], keyvalues[i + 1]);
      }
      return dict;
    }

    public static string ToJSON(
      IList<IDictionary<string, string>> dictlist) {
      var sb = new StringBuilder().Append("[");
      if (dictlist == null) {
        throw new ArgumentNullException(nameof(dictlist));
      }
      for (var i = 0; i < dictlist.Count; ++i) {
        if (i > 0) {
          sb.Append(",");
        }
        IDictionary<string, string> dict = dictlist[i];
        var larray = new string[dict.Count * 2];
        var k = 0;
        foreach (string key in dict.Keys) {
          larray[k] = key;
          larray[k + 1] = dict[key];
          k += 2;
        }
        sb.Append(ToJSON(larray));
      }
      return sb.Append("]").ToString();
    }

    private static void JSONEscape(string str, StringBuilder sb) {
      for (var j = 0; j < str.Length; ++j) {
        if ((str[j] & 0xfc00) == 0xdc00 ||
          ((str[j] & 0xfc00) == 0xd800 && (j == str.Length - 1 ||
              (str[j + 1] & 0xfc00) != 0xdc00))) {
          throw new ArgumentException("arr is invalid");
        }
        if (str[j] == '\"') {
          sb.Append("\\\"");
        } else if (str[j] == '\\') {
          sb.Append("\\\\");
        } else if (str[j] == '\r') {
          sb.Append("\\r");
        } else if (str[j] == '\n') {
          sb.Append("\\n");
        } else if (str[j] == '\t') {
          sb.Append("\\t");
        } else if (str[j] == 0x20 && j + 1 < str.Length && str[j + 1] ==
          0x20) {
          sb.Append("\\u0020");
        } else if (str[j] < 0x20 || str[j] >= 0x7f) {
          var ch = (int)str[j];
          sb.Append("\\u")
          .Append(HexAlphabet[(ch >> 12) & 15])
          .Append(HexAlphabet[(ch >> 8) & 15])
          .Append(HexAlphabet[(ch >> 4) & 15]).Append(HexAlphabet[ch & 15]);
        } else {
          sb.Append(str[j]);
        }
      }
    }

    public static string[] SetResource(
      string[] resources,
      string name,
      string value) {
      if (resources == null) {
        throw new ArgumentNullException(nameof(resources));
      }
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (value == null) {
        throw new ArgumentNullException(nameof(value));
      }
      if (name.IndexOf('=', StringComparison.Ordinal) != -1) {
        throw new ArgumentException("name has an equal sign");
      }
      var sb = new StringBuilder();
      JSONEscape(value, sb);
      var list = new List<string>();
      string resourceLine = name + "=" + sb.ToString();
      var added = false;
      foreach (string resource in resources) {
        if (resource.IndexOf(
          name + "=",
          StringComparison.Ordinal) == 0) {
          list.Add(resourceLine);
          added = true;
        } else {
          list.Add(resource);
        }
      }
      if (!added) {
        list.Add(resourceLine);
      }
      return (string[])list.ToArray();
    }

    public static string ToJSON(string[] arr) {
      var sb = new StringBuilder().Append("[");
      if (arr == null) {
        throw new ArgumentNullException(nameof(arr));
      }
      for (var i = 0; i < arr.Length; ++i) {
        if (i > 0) {
          sb.Append(",");
        }
        sb.Append("\"");
        JSONEscape(arr[i], sb);
        sb.Append("\"");
      }
      return sb.Append("]").ToString();
    }

    public static IList<IDictionary<string, string>> ParseJSONDictList(
      string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      var i = 0;
      var list = new List<IDictionary<string, string>>();
      while (i < str.Length && (
          str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a ||
          str[i] == 0x09)) {
        ++i;
      }
      if (i >= str.Length || str[i] != '[') {
        throw new InvalidOperationException("invalid start of list");
      }
      ++i;
      var endPos = new int[] { 0 };
      var endValue = false;
      string[] stringArray = null;
      while (true) {
        while (i < str.Length && (
            str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a ||
            str[i] == 0x09)) {
          ++i;
        }
        if (i >= str.Length || (
            str[i] != ']' && str[i] != '[' && str[i] != 0x2c)) {
          throw new InvalidOperationException("Invalid JSON");
        }
        switch (str[i]) {
          case ']':
            ++i;
            while (i < str.Length && (
                str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a || str[i]
                == 0x09)) {
              ++i;
            }
            return i == str.Length ? list : null;
          case (char)0x2c:
            if (!endValue) {
              throw new InvalidOperationException("unexpected comma");
            }
            ++i;
            endValue = false;
            break;
          case '[':
            endPos[0] = i;
            stringArray = ParseJSONStringArray(str, endPos);
            if (stringArray == null) {
              throw new InvalidOperationException("invalid string array");
            }
            i = endPos[0];
            endValue = true;
            list.Add(MakeDict(stringArray));
            break;
        }
      }
    }

    public static string[] ParseJSONStringArray(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      var endPos = new int[] { 0 };
      string[] ret = ParseJSONStringArray(str, endPos);
      if (endPos[0] != str.Length) {
        throw new InvalidOperationException("Invalid JSON");
      }
      return ret;
    }
    public static string[] ParseJSONStringArray(string str, int[] endPos) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (endPos == null) {
        throw new ArgumentNullException(nameof(endPos));
      }
      int i = endPos[0];
      var j = 0;
      var list = new List<string>();
      var sb = new StringBuilder();
      while (i < str.Length && (
          str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a ||
          str[i] == 0x09)) {
        ++i;
      }
      if (i >= str.Length || str[i] != '[') {
        throw new InvalidOperationException("Invalid JSON: " +
          str.Substring(i));
      }
      ++i;
      var endValue = false;
      while (true) {
        while (i < str.Length && (
            str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a ||
            str[i] == 0x09)) {
          ++i;
        }
        if (i >= str.Length || (
            str[i] != ']' && str[i] != '"' && str[i] != 0x2c)) {
          throw new InvalidOperationException("Invalid JSON:" +
            "\u0020" + str.Substring(i));
        }
        var si = (int)str[i];
        switch (si) {
          case 0x5d:
            // right square bracket
            ++i;
            while (i < str.Length && (
                str[i] == 0x20 || str[i] == 0x0d || str[i] == 0x0a || str[i]
                == 0x09)) {
              ++i;
            }
            endPos[0] = i;
            return (string[])list.ToArray();
          case 0x2c:
            // comma
            if (!endValue) {
              throw new InvalidOperationException("Invalid JSON:" +
                "\u0020" + str.Substring(i));
            }
            ++i;
            endValue = false;
            break;
          case 0x22:
            // double quote
            j = i;
            i = ParseJSONString(str, i + 1, sb);
            if (i < 0) {
              throw new InvalidOperationException("Invalid JSON: bad string:" +
                "\u0020" + str.Substring(j));
            }
            endValue = true;
            list.Add(sb.ToString());
            break;
        }
      }
    }

    private static string ParseJSONString(string str) {
      if (str == null || str.Length < 2 || str[0] != '"') {
        throw new InvalidOperationException("Invalid JSON");
      }
      var sb = new StringBuilder();
      int result = ParseJSONString(str, 1, sb);
      if (result != str.Length) {
        throw new InvalidOperationException("Invalid JSON");
      }
      return sb.ToString();
    }

    private static int ParseJSONString(
      string str,
      int index,
      StringBuilder sb) {
      #if DEBUG
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (sb == null) {
        throw new ArgumentNullException(nameof(sb));
      }
      #endif

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
