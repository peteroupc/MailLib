using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.EnrichedText"]/*'/>
  internal static class EnrichedText {
private static bool IsTokenAsciiIgnoreCase(
  string str,
  int index,
  int endIndex,
  string strToMatch) {
      for (var i = 0; i < strToMatch.Length; ++i) {
        int idx = index + i;
        if (idx >= endIndex) {
 return false;
}
        char c1 = (str[idx] >= 0x41 && str[idx] <= 0x5a) ?
            (char)(str[idx] + 0x20) : str[idx];
        char c2 = (strToMatch[i] >= 0x41 && strToMatch[i] <= 0x5a) ?
            (char)(strToMatch[i] + 0x20) : strToMatch[i];
        if (c1 != c2) {
 return false;
}
      }
      return true;
    }

    private static bool IsHexChar(char c) {
      return (c >= 'a' && c <= 'f') ||
        (c >= 'A' && c <= 'F') || (c >= '0' && c <= '9');
    }

    private static string ParseColor(string str, int index, int endIndex) {
    string[] colorNames = {
"yellow",
  "red",
  "green",
  "blue",
  "black",
  "white",
  "cyan",
   "magenta"};
      foreach (string name in colorNames) {
        if (IsTokenAsciiIgnoreCase(str, index, endIndex, name)) {
 return name;
}
      }
      if (index + 13 >= endIndex) {
 return null;
}
      for (var i = 0; i < 14; ++i) {
        if (i == 4 || i == 8) {
{ if (str[index + i] != ',') {
{ return null;
} }
} } else { if (!IsHexChar(str[index + i])) {
 return null;
} }
      }
      string ret = "#";
      return
ret + str.Substring(index, 2) + str.Substring(index + 5, 2) +
  str.Substring(
  index,
  10);
    }

    private static int SkipFont(string str, int index, int endIndex) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str[index] >= 48 && str[index] <=
              57) ||
           (str[index] >= 44 && str[index] <= 45) || (str[index] == 32) ||
              (str[index] >= 12 && str[index] <= 13) || (str[index]
              >= 9 &&
           str[index] <= 10) || (str[index] >= 97 && str[index] <= 122) ||
              (str[index] >= 65 && str[index] <= 90))) {
              ++indexTemp2; break;
            }
            int indexTemp3 = index;
            do {
              if (index < endIndex && ((str[index] >= 128 && str[index]
          <= 55295) || (str[index] >= 57344 && str[index] <=
              65535))) {
                ++indexTemp3; break;
              }
              if (index + 1 < endIndex && ((str[index] >= 55296 &&
str[index] <= 56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
                indexTemp3 += 2; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    private static int SkipLang(string str, int index, int endIndex) {
      while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
            (str[index] == 45) || (str[index] >= 97 && str[index] <= 122) ||
          (str[index] >= 65 && str[index] <= 90))) {
        ++index;
      }
      return index;
    }

    private static string[] SplitAt(string str, string delimiter) {
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
          strings = strings ?? (new List<string>());
          strings.Add(str.Substring(index));
          break;
        } else {
          first = false;
          string newstr = str.Substring(index, index2 - index);
          strings = strings ?? (new List<string>());
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return (string[])strings.ToArray();
    }

    private static string TrimSpaces(string s) {
      if (String.IsNullOrEmpty(s)) {
        return s;
      }
      var index = 0;
      int valueSLength = s.Length;
      while (index < valueSLength) {
        char c = s[index];
        if (c != 0x09 && c != 0x0a && c != 0x0c && c != 0x0d && c !=
            0x20) {
          break;
        }
        ++index;
      }
      if (index == valueSLength) {
        return String.Empty;
      }
      int startIndex = index;
      index = valueSLength - 1;
      while (index >= 0) {
        char c = s[index];
        if (c != 0x09 && c != 0x0a && c != 0x0c && c != 0x0d && c !=
            0x20) {
          return s.Substring(startIndex, (index + 1) - startIndex);
        }
        --index;
      }
      return String.Empty;
    }

    public static string EnrichedToPlain(
  string str,
  int index,
  int endIndex) {
      var originalBuilder = new StringBuilder();
      var paramBuilder = new StringBuilder();
      StringBuilder currentBuilder = originalBuilder;
      var withinParam = false;
      var nofillDepth = 0;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index + 1 < endIndex && str[index] == 60 && str[index + 1] ==
                  60) {
              currentBuilder.Append("<");
              indexTemp2 += 2; break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str[index] == 60)) {
                ++index;
              } else {
                break;
              }
              var isEndTag = false;
              if (index < endIndex && (str[index] == 47)) {
                ++index;
                isEndTag = true;
              }
              int commandStart = index;
              for (int i3 = 0; i3 < 60; ++i3) {
                if (index < endIndex && ((str[index] >= 48 && str[index]
<= 57) || (str[index] == 45) || (str[index] >= 97 && str[index] <= 122) ||
(str[index] >= 65 && str[index] <=

                90))) {
                  ++index;
                } else if (i3 < 1) {
                  index = indexStart3; break;
                } else {
                  break;
                }
              }
              if (index == indexStart3) {
                break;
              }
              int commandEnd = index;
              if (index < endIndex && (str[index] == 62)) {
                ++index;
                string command = str.Substring(
  commandStart,
  commandEnd - commandStart).ToLowerInvariant();
                if (command.Equals("nofill")) {
                  if (isEndTag && nofillDepth > 0) {
                    --nofillDepth;
                  } else if (!isEndTag) {
                    ++nofillDepth;
                  }
                }
                if (command.Equals("param")) {
                  if (isEndTag) {
                    withinParam = false;
                    currentBuilder = originalBuilder;
                  } else {
                    withinParam = true;
                    paramBuilder.Remove(0, paramBuilder.Length);
                    currentBuilder = paramBuilder;
                  }
                }
              } else {
                index = indexStart3; break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            int indexStart2 = index;
            var lineBreakCount = 0;
            var i2 = 0;
            for (; ; ++i2) {
              indexTemp3 = index;
              do {
                if (index + 1 < endIndex && str[index] == 13 && str[index + 1]
                  == 10) {
                  indexTemp3 += 2; break;
                }
                if (index < endIndex && ((str[index] == 13) || (str[index]
                == 10))) {
                  ++indexTemp3; break;
                }
              } while (false);
              if (indexTemp3 == index) {
                if (i2 < 1) {
                  indexTemp2 = indexStart2;
                }
                lineBreakCount = i2;
                break;
              } else {
                index = indexTemp3;
              }
            }
            index = indexStart2;
            if (indexTemp3 != indexStart2) {
              // Line breaks
              if (nofillDepth > 0) {
                for (int j = 0; j < lineBreakCount; ++j) {
                  currentBuilder.Append("\r\n");
                }
              } else {
                if (lineBreakCount == 1) {
                  currentBuilder.Append(' ');
                } else if (!withinParam) {
                  if (lineBreakCount == 2) {
                    currentBuilder.Append("\r\n");
                  } else {
                    var j = 0;
                    for (j = 1; j < lineBreakCount; ++j) {
                    currentBuilder.Append("\r\n");
                    }
                  }
                } else {
                  currentBuilder.Append(' ');
                }
              }
              indexTemp2 = indexTemp3;
              index = indexStart2;
              break;
            }
            if (index < endIndex && ((str[index] >= 0 && str[index] <=
            9) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 &&
            str[index] <= 127))) {
              // Ordinary character
              if (str[index] == 0) {
                // Null
                currentBuilder.Append((char)0xfffd);
              } else {
                currentBuilder.Append(str[index]);
              }
              ++indexTemp2; break;
            }
            indexTemp3 = index;
            do {
              if (index < endIndex && ((str[index] >= 128 && str[index]
          <= 55295) || (str[index] >= 57344 && str[index] <=
              65535))) {
                // BMP character
                currentBuilder.Append(str[index]);
                ++indexTemp3; break;
              }
              if (index + 1 < endIndex && ((str[index] >= 55296 &&
str[index] <= 56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
                // Supplementary character
                currentBuilder.Append(str[index]);
                currentBuilder.Append(str[index + 1]);
                indexTemp3 += 2; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str[index] >= 55296 && str[index]
            <= 57343)) {
              // Unpaired surrogate
              currentBuilder.Append((char)0xfffd);
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
      } while (false);
      return originalBuilder.ToString();
    }

    public static string EnrichedToHtml(
  string str,
  int index,
  int endIndex) {
      var originalBuilder = new StringBuilder();
      var paramBuilder = new StringBuilder();
      StringBuilder currentBuilder = originalBuilder;
      var withinParam = false;
      var nofillDepth = 0;
      originalBuilder.Append("<!DOCTYPE html><html><title>Untitled</title>");
  originalBuilder.Append("<style>p { margin-bottom: 0em; margin-top: 0em; }");
      originalBuilder.Append("</style><body>");
      string lastCommand = String.Empty;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index + 1 < endIndex && str[index] == 60 && str[index + 1] ==
                  60) {
              currentBuilder.Append("&lt;");
              indexTemp2 += 2; break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str[index] == 60)) {
                ++index;
              } else {
                break;
              }
              var isEndTag = false;
              if (index < endIndex && (str[index] == 47)) {
                ++index;
                isEndTag = true;
              }
              int commandStart = index;
              for (int i3 = 0; i3 < 60; ++i3) {
                if (index < endIndex && ((str[index] >= 48 && str[index]
<= 57) || (str[index] == 45) || (str[index] >= 97 && str[index] <= 122) ||
(str[index] >= 65 && str[index] <=

                90))) {
                  ++index;
                } else if (i3 < 1) {
                  index = indexStart3; break;
                } else {
                  break;
                }
              }
              if (index == indexStart3) {
                break;
              }
              int commandEnd = index;
              if (index < endIndex && (str[index] == 62)) {
                ++index;
                string command = str.Substring(
  commandStart,
  commandEnd - commandStart).ToLowerInvariant();
                if (!withinParam) {
                  if (command.Equals("bold")) {
                    currentBuilder.Append('<');
                    if (isEndTag) {
                    currentBuilder.Append('/');
                    }
                    currentBuilder.Append("b>");
                  } else if (command.Equals("italic")) {
                    currentBuilder.Append('<');
                    if (isEndTag) {
                    currentBuilder.Append('/');
                    }
                    currentBuilder.Append("i>");
                  } else if (command.Equals("fixed")) {
                    if (isEndTag) {
                    currentBuilder.Append("</span>");
                    } else {
                    currentBuilder.Append("<span style='font-family:");
                    currentBuilder.Append("monospaced'>");
                    }
                  } else if (command.Equals("center")) {
                    if (isEndTag) {
                    currentBuilder.Append("</div>");
                    } else {
                    currentBuilder.Append("<div style='text-align:center'>");
                    }
                  } else if (command.Equals("flushleft")) {
                    if (isEndTag) {
                    currentBuilder.Append("</div>");
                    } else {
                    currentBuilder.Append("<div style='text-align:left'>");
                    }
                  } else if (command.Equals("flushright")) {
                    if (isEndTag) {
                    currentBuilder.Append("</div>");
                    } else {
                    currentBuilder.Append("<div style='text-align:right'>");
                    }
                  } else if (command.Equals("flushboth")) {
                    if (isEndTag) {
                    currentBuilder.Append("</div>");
                    } else {
                    currentBuilder.Append("<div style='text-align:justify'>");
                    }
                  } else if (command.Equals("fontfamily")) {
                    if (isEndTag) {
                    currentBuilder.Append("</span>");
                    }
                  } else if (command.Equals("color")) {
                    if (isEndTag) {
                    currentBuilder.Append("</span>");
                    }
                  } else if (command.Equals("paraindent")) {
                    if (isEndTag) {
                    currentBuilder.Append("</div>");
                    }
                  } else if (command.Equals("smaller")) {
                    if (isEndTag) {
                    currentBuilder.Append("</span>");
                    } else {
                    currentBuilder.Append("<span style='font-size:85%'>");
                    }
                  } else if (command.Equals("nofill")) {
                    if (isEndTag && nofillDepth > 0) {
                    --nofillDepth;
                    } else if (!isEndTag) {
                    ++nofillDepth;
                    }
                  } else if (command.Equals("bigger")) {
                    if (isEndTag) {
                    currentBuilder.Append("</span>");
                    } else {
                    currentBuilder.Append("<span style='font-size:110%'>");
                    }
                  } else if (command.Equals("lang")) {
                    if (isEndTag) {
                    currentBuilder.Append("</span>");
                    }
                  } else if (command.Equals("excerpt")) {
                    if (isEndTag) {
                    currentBuilder.Append("</blockquote>");
                    } else {
                    currentBuilder.Append("<blockquote>");
                    }
                  }
                  if (!command.Equals("param")) {
                    lastCommand = command;
                  }
                }
                bool wasWithinParam = withinParam;
                if (command.Equals("param")) {
                  if (isEndTag) {
                    withinParam = false;
                    currentBuilder = originalBuilder;
                    string p = TrimSpaces(paramBuilder.ToString());
                    if (lastCommand.Equals("fontfamily")) {
                    if (SkipFont(p, 0, p.Length) == p.Length) {
                    currentBuilder.Append("<span style='font-family: " + p +
                    "'> ");
                    } else {
                    currentBuilder.Append("<span>");
                    }
                    } else if (lastCommand.Equals("color")) {
                    p = ParseColor(
          DataUtilities.ToLowerCaseAscii(p),
          0,
          p.Length);
                    if (p != null) {
                    currentBuilder.Append("<span style='color: " + p + "'>");
                    } else {
                    currentBuilder.Append("<span>");
                    }
                    } else if (lastCommand.Equals("lang")) {
                    if (SkipLang(p, 0, p.Length) == p.Length) {
    currentBuilder.Append("<span lang=' " + DataUtilities.ToLowerCaseAscii(p) +
                    "'> ");
                    } else {
                    currentBuilder.Append("<span>");
                    }
                    } else if (lastCommand.Equals("paraindent")) {
                    p = DataUtilities.ToLowerCaseAscii(p);
                    string[] valuePList = SplitAt(p, ",");
                    var leftFlag = false;
                    var rightFlag = false;
                    var inFlag = false;
                    var outFlag = false;
                    var styleBuilder = new StringBuilder();
                    foreach (string valuePItem in valuePList) {
                    string valuePItem2 = TrimSpaces(valuePItem);
                    if (!leftFlag && valuePItem2.Equals("left")) {
                    styleBuilder.Append("padding-left: 2em;");
                    } else if (!rightFlag && valuePItem2.Equals("right")) {
                    styleBuilder.Append("padding-right: 2em;");
                    } else if (!inFlag && valuePItem2.Equals("in")) {
                    styleBuilder.Append("text-indent: 2em;");
                    } else if (!outFlag && valuePItem2.Equals("out")) {
                    styleBuilder.Append("text-indent: -2em;");
                    styleBuilder.Append("margin-left: 2em;");
                    }
                    }
                    if (!inFlag && !outFlag) {
                    styleBuilder.Append("padding-top:0;");
                    styleBuilder.Append("padding-bottom: 0;");
                    }
                    currentBuilder.Append("<div style=' " +
                    styleBuilder.ToString() + "'> ");
                    }
                    lastCommand = String.Empty;
                  } else {
                    withinParam = true;
                    paramBuilder.Remove(0, paramBuilder.Length);
                    currentBuilder = paramBuilder;
                  }
                }
                if (withinParam && wasWithinParam) {
                  currentBuilder.Append("<");
                  if (isEndTag) {
                    currentBuilder.Append('/');
                  }
                  currentBuilder.Append(command);
                  currentBuilder.Append(">");
                }
              } else {
                index = indexStart3; break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            int indexStart2 = index;
            var lineBreakCount = 0;
            for (int i2 = 0; ; ++i2) {
              indexTemp3 = index;
              do {
                if (index + 1 < endIndex && str[index] == 13 && str[index + 1]
                  == 10) {
                  indexTemp3 += 2; break;
                }
                if (index < endIndex && ((str[index] == 13) || (str[index]
                == 10))) {
                  ++indexTemp3; break;
                }
              } while (false);
              if (indexTemp3 == index) {
                if (i2 < 1) {
                  indexTemp2 = indexStart2;
                }
                lineBreakCount = i2;
                break;
              } else {
                index = indexTemp3;
              }
            }
            index = indexStart2;
            if (indexTemp3 != indexStart2) {
              // Line breaks
              if (nofillDepth > 0) {
                for (int j = 0; j < lineBreakCount; ++j) {
                  currentBuilder.Append("<br>");
                }
              } else {
                if (lineBreakCount == 1) {
                  currentBuilder.Append(' ');
                } else if (!withinParam) {
                  if (lineBreakCount == 2) {
                    currentBuilder.Append("<br>");
                  } else {
                    var j = 0;
                    currentBuilder.Append("<p>");
                    for (j = 2; j < lineBreakCount; ++j) {
                    currentBuilder.Append("<br>");
                    }
                  }
                } else {
                  currentBuilder.Append(' ');
                }
              }
              indexTemp2 = indexTemp3;
              index = indexStart2;
              break;
            }
            if (index < endIndex && ((str[index] >= 0 && str[index] <=
            9) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 &&
            str[index] <= 127))) {
              // Ordinary character
              if (str[index] == 0) {
                // Null
                currentBuilder.Append((char)0xfffd);
              } else if (str[index] == '&') {
                currentBuilder.Append(withinParam ? "&" : "&amp;");
              } else {
                currentBuilder.Append(str[index]);
              }
              ++indexTemp2; break;
            }
            indexTemp3 = index;
            do {
              if (index < endIndex && ((str[index] >= 128 && str[index]
          <= 55295) || (str[index] >= 57344 && str[index] <=
              65535))) {
                // BMP character
                currentBuilder.Append(str[index]);
                ++indexTemp3; break;
              }
              if (index + 1 < endIndex && ((str[index] >= 55296 &&
str[index] <= 56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
                // Supplementary character
                currentBuilder.Append(str[index]);
                currentBuilder.Append(str[index + 1]);
                indexTemp3 += 2; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str[index] >= 55296 && str[index]
            <= 57343)) {
              // Unpaired surrogate
              currentBuilder.Append((char)0xfffd);
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
      } while (false);
      originalBuilder.Append("</body></html>");
      return originalBuilder.ToString();
    }
  }
}
