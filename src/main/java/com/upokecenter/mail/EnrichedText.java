package com.upokecenter.mail;

import java.util.*;

import com.upokecenter.util.*;

    /**
     * This is an internal API.
     */
  final class EnrichedText {
private EnrichedText() {
}
    private static boolean IsTokenAsciiIgnoreCase(
      String str,
      int index,
      int endIndex,
      String strToMatch) {
      for (int i = 0; i < strToMatch.length(); ++i) {
        int idx = index + i;
        if (idx >= endIndex) {
          return false;
        }
        char c1 = (str.charAt(idx) >= 0x41 && str.charAt(idx) <= 0x5a) ?
            (char)(str.charAt(idx) + 0x20) : str.charAt(idx);
        char c2 = (strToMatch.charAt(i) >= 0x41 && strToMatch.charAt(i) <= 0x5a) ?
            (char)(strToMatch.charAt(i) + 0x20) : strToMatch.charAt(i);
        if (c1 != c2) {
          return false;
        }
      }
      return true;
    }
    private static boolean IsHexChar(char c) {
      return (c >= 'a' && c <= 'f') ||
        (c >= 'A' && c <= 'F') || (c >= '0' && c <= '9');
    }
    private static String ParseColor(String str, int index, int endIndex) {
      String[] colorNames = {
        "yellow",
        "red",
        "green",
        "blue",
        "black",
        "white",
        "cyan",
        "magenta",
      };
      for (String name : colorNames) {
        if (IsTokenAsciiIgnoreCase(str, index, endIndex, name)) {
          return name;
        }
      }
      if (index + 13 >= endIndex) {
        return null;
      }
      for (int i = 0; i < 14; ++i) {
        if (i == 4 || i == 8) {
          {
            if (str.charAt(index + i) != ',') {
              {
                return null;
              }
            }
          }
        } else {
          if (!IsHexChar(str.charAt(index + i))) {
            return null;
          }
        }
      }
      String ret = "#";
      return
ret + str.substring(index, (index)+(2)) + str.substring(index + 5, (index + 5)+(2)) +
  str.substring(
    index, (
    index)+(10));
    }
    private static int SkipFont(String str, int index, int endIndex) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <=
              57) ||
           (str.charAt(index) >= 44 && str.charAt(index) <= 45) || (str.charAt(index) == 32) ||
              (str.charAt(index) >= 12 && str.charAt(index) <= 13) || (str.charAt(index)
              >= 9 &&
           str.charAt(index) <= 10) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) ||
              (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
              ++indexTemp2;
              break;
            }
            int indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index)
          <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <=
              65535))) {
                ++indexTemp3;
                break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                indexTemp3 += 2;
                break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3;
              break;
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
    private static int SkipLang(String str, int index, int endIndex) {
      while (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) ||
            (str.charAt(index) == 45) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) ||
          (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
        ++index;
      }
      return index;
    }

    private static String TrimSpaces(String s) {
      if (((s) == null || (s).length() == 0)) {
        return s;
      }
      int index = 0;
      int valueSLength = s.length();
      while (index < valueSLength) {
        char c = s.charAt(index);
        if (c != 0x09 && c != 0x0a && c != 0x0c && c != 0x0d && c !=
            0x20) {
          break;
        }
        ++index;
      }
      if (index == valueSLength) {
        return "";
      }
      int startIndex = index;
      index = valueSLength - 1;
      while (index >= 0) {
        char c = s.charAt(index);
        if (c != 0x09 && c != 0x0a && c != 0x0c && c != 0x0d && c !=
            0x20) {
          return s.substring(startIndex, (startIndex)+((index + 1) - startIndex));
        }
        --index;
      }
      return "";
    }
    public static String EnrichedToPlain(
      String str,
      int index,
      int endIndex) {
      StringBuilder originalBuilder = new StringBuilder();
      StringBuilder paramBuilder = new StringBuilder();
      StringBuilder currentBuilder = originalBuilder;
      boolean withinParam = false;
      int nofillDepth = 0;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index + 1 < endIndex && str.charAt(index) == 60 && str.charAt(index + 1) ==
                  60) {
              currentBuilder.append("<");
              indexTemp2 += 2;
              break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) == 60)) {
                ++index;
              } else {
                break;
              }
              boolean isEndTag = false;
              if (index < endIndex && (str.charAt(index) == 47)) {
                ++index;
                isEndTag = true;
              }
              int commandStart = index;
              for (int i3 = 0; i3 < 60; ++i3) {
                if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index)
<= 57) || (str.charAt(index) == 45) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) ||
(str.charAt(index) >= 65 && str.charAt(index) <=

                90))) {
                  ++index;
                } else if (i3 < 1) {
                  index = indexStart3;
                  break;
                } else {
                  break;
                }
              }
              if (index == indexStart3) {
                break;
              }
              int commandEnd = index;
              if (index < endIndex && (str.charAt(index) == 62)) {
                ++index;
                String command = DataUtilities.ToLowerCaseAscii(str.substring(
                  commandStart, (
                  commandStart)+(commandEnd - commandStart)));
                if (command.equals("nofill")) {
                  if (isEndTag && nofillDepth > 0) {
                    --nofillDepth;
                  } else if (!isEndTag) {
                    ++nofillDepth;
                  }
                }
                if (command.equals("param")) {
                  if (isEndTag) {
                    withinParam = false;
                    currentBuilder = originalBuilder;
                  } else {
                    withinParam = true;
                    paramBuilder.delete(0, paramBuilder.length());
                    currentBuilder = paramBuilder;
                  }
                }
              } else {
                index = indexStart3;
                break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3;
              break;
            }
            int indexStart2 = index;
            int lineBreakCount = 0;
            int i2 = 0;
            for (; true; ++i2) {
              indexTemp3 = index;
              do {
                if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1)
                  == 10) {
                  indexTemp3 += 2;
                  break;
                }
                if (index < endIndex && ((str.charAt(index) == 13) || (str.charAt(index)
                == 10))) {
                  ++indexTemp3;
                  break;
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
                  currentBuilder.append("\r\n");
                }
              } else {
                if (lineBreakCount == 1) {
                  currentBuilder.append(' ');
                } else if (!withinParam) {
                  if (lineBreakCount == 2) {
                    currentBuilder.append("\r\n");
                  } else {
                    int j = 0;
                    for (j = 1; j < lineBreakCount; ++j) {
                      currentBuilder.append("\r\n");
                    }
                  }
                } else {
                  currentBuilder.append(' ');
                }
              }
              indexTemp2 = indexTemp3;
              index = indexStart2;
              break;
            }
            if (index < endIndex && ((str.charAt(index) >= 0 && str.charAt(index) <=
            9) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) || (str.charAt(index) >= 14 &&
            str.charAt(index) <= 127))) {
              // Ordinary character
              if (str.charAt(index) == 0) {
                // Null
                currentBuilder.append((char)0xfffd);
              } else {
                currentBuilder.append(str.charAt(index));
              }
              ++indexTemp2;
              break;
            }
            indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index)
          <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <=
              65535))) {
                // BMP character
                currentBuilder.append(str.charAt(index));
                ++indexTemp3;
                break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                // Supplementary character
                currentBuilder.append(str.charAt(index));
                currentBuilder.append(str.charAt(index + 1));
                indexTemp3 += 2;
                break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3;
              break;
            }
            if (index < endIndex && (str.charAt(index) >= 55296 && str.charAt(index)
            <= 57343)) {
              // Unpaired surrogate
              currentBuilder.append((char)0xfffd);
              ++indexTemp2;
              break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
      } while (false);
      return originalBuilder.toString();
    }
    public static String EnrichedToHtml(
      String str,
      int index,
      int endIndex) {
      StringBuilder originalBuilder = new StringBuilder();
      StringBuilder paramBuilder = new StringBuilder();
      StringBuilder currentBuilder = originalBuilder;
      boolean withinParam = false;
      int nofillDepth = 0;
      originalBuilder.append("<!DOCTYPE html><html><title>Untitled</title>");
      originalBuilder.append("<style>p { margin-bottom: 0em; ")
           .append("margin-top: 0em; }");
      originalBuilder.append("</style><body>");
      String lastCommand = "";
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index + 1 < endIndex && str.charAt(index) == 60 && str.charAt(index + 1) ==
                  60) {
              currentBuilder.append("&lt;");
              indexTemp2 += 2;
              break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) == 60)) {
                ++index;
              } else {
                break;
              }
              boolean isEndTag = false;
              if (index < endIndex && (str.charAt(index) == 47)) {
                ++index;
                isEndTag = true;
              }
              int commandStart = index;
              for (int i3 = 0; i3 < 60; ++i3) {
                if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index)
<= 57) || (str.charAt(index) == 45) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) ||
(str.charAt(index) >= 65 && str.charAt(index) <=

                90))) {
                  ++index;
                } else if (i3 < 1) {
                  index = indexStart3;
                  break;
                } else {
                  break;
                }
              }
              if (index == indexStart3) {
                break;
              }
              int commandEnd = index;
              if (index < endIndex && (str.charAt(index) == 62)) {
                ++index;
                String command = DataUtilities.ToLowerCaseAscii(str.substring(
                  commandStart, (
                  commandStart)+(commandEnd - commandStart)));
                if (!withinParam) {
                  if (command.equals("bold")) {
                    currentBuilder.append('<');
                    if (isEndTag) {
                      currentBuilder.append('/');
                    }
                    currentBuilder.append("b>");
                  } else if (command.equals("italic")) {
                    currentBuilder.append('<');
                    if (isEndTag) {
                      currentBuilder.append('/');
                    }
                    currentBuilder.append("i>");
                  } else if (command.equals("fixed")) {
                    if (isEndTag) {
                      currentBuilder.append("</span>");
                    } else {
                      currentBuilder.append("<span style='font-family:");
                      currentBuilder.append("monospaced'>");
                    }
                  } else if (command.equals("center")) {
                    if (isEndTag) {
                      currentBuilder.append("</div>");
                    } else {
                      currentBuilder.append("<div style='text-align:center'>");
                    }
                  } else if (command.equals("flushleft")) {
                    if (isEndTag) {
                      currentBuilder.append("</div>");
                    } else {
                      currentBuilder.append("<div style='text-align:left'>");
                    }
                  } else if (command.equals("flushright")) {
                    if (isEndTag) {
                      currentBuilder.append("</div>");
                    } else {
                      currentBuilder.append("<div style='text-align:right'>");
                    }
                  } else if (command.equals("flushboth")) {
                    if (isEndTag) {
                      currentBuilder.append("</div>");
                    } else {
                      currentBuilder.append("<div style='text-align:justify'>");
                    }
                  } else if (command.equals("fontfamily")) {
                    if (isEndTag) {
                      currentBuilder.append("</span>");
                    }
                  } else if (command.equals("color")) {
                    if (isEndTag) {
                      currentBuilder.append("</span>");
                    }
                  } else if (command.equals("paraindent")) {
                    if (isEndTag) {
                      currentBuilder.append("</div>");
                    }
                  } else if (command.equals("smaller")) {
                    if (isEndTag) {
                      currentBuilder.append("</span>");
                    } else {
                      currentBuilder.append("<span style='font-size:85%'>");
                    }
                  } else if (command.equals("nofill")) {
                    if (isEndTag && nofillDepth > 0) {
                      --nofillDepth;
                    } else if (!isEndTag) {
                      ++nofillDepth;
                    }
                  } else if (command.equals("bigger")) {
                    if (isEndTag) {
                      currentBuilder.append("</span>");
                    } else {
                      currentBuilder.append("<span style='font-size:110%'>");
                    }
                  } else if (command.equals("lang")) {
                    if (isEndTag) {
                      currentBuilder.append("</span>");
                    }
                  } else if (command.equals("excerpt")) {
                    if (isEndTag) {
                      currentBuilder.append("</blockquote>");
                    } else {
                      currentBuilder.append("<blockquote>");
                    }
                  }
                  if (!command.equals("param")) {
                    lastCommand = command;
                  }
                }
                boolean wasWithinParam = withinParam;
                if (command.equals("param")) {
                  if (isEndTag) {
                    withinParam = false;
                    currentBuilder = originalBuilder;
                    String p = TrimSpaces(paramBuilder.toString());
                    if (lastCommand.equals("fontfamily")) {
                      if (SkipFont(p, 0, p.length()) == p.length()) {
                        currentBuilder.append("<span style='font-family: " + p +
                        "'> ");
                      } else {
                        currentBuilder.append("<span>");
                      }
                    } else if (lastCommand.equals("color")) {
                      p = ParseColor(
            DataUtilities.ToLowerCaseAscii(p),
            0,
            p.length());
                      if (p != null) {
                      currentBuilder.append("<span style='color: " + p +
                          "'>");
                      } else {
                        currentBuilder.append("<span>");
                      }
                    } else if (lastCommand.equals("lang")) {
                      if (SkipLang(p, 0, p.length()) == p.length()) {
    currentBuilder.append("<span lang=' " + DataUtilities.ToLowerCaseAscii(p) +
                                        "'> ");
                      } else {
                        currentBuilder.append("<span>");
                      }
                    } else if (lastCommand.equals("paraindent")) {
                      p = DataUtilities.ToLowerCaseAscii(p);
                      String[] valuePList = ParserUtility.SplitAt(p, ",");
                      boolean leftFlag = false;
                      boolean rightFlag = false;
                      boolean inFlag = false;
                      boolean outFlag = false;
                      StringBuilder styleBuilder = new StringBuilder();
                      for (String valuePItem : valuePList) {
                        String valuePItem2 = TrimSpaces(valuePItem);
                        if (!leftFlag && valuePItem2.equals("left")) {
                          styleBuilder.append("padding-left: 2em;");
                        } else if (!rightFlag && valuePItem2.equals("right")) {
                          styleBuilder.append("padding-right: 2em;");
                        } else if (!inFlag && valuePItem2.equals("in")) {
                          styleBuilder.append("text-indent: 2em;");
                        } else if (!outFlag && valuePItem2.equals("out")) {
                          styleBuilder.append("text-indent: -2em;");
                          styleBuilder.append("margin-left: 2em;");
                        }
                      }
                      if (!inFlag && !outFlag) {
                        styleBuilder.append("padding-top:0;");
                        styleBuilder.append("padding-bottom: 0;");
                      }
                      currentBuilder.append("<div style=' " +
                      styleBuilder.toString() + "'> ");
                    }
                    lastCommand = "";
                  } else {
                    withinParam = true;
                    paramBuilder.delete(0, paramBuilder.length());
                    currentBuilder = paramBuilder;
                  }
                }
                if (withinParam && wasWithinParam) {
                  currentBuilder.append("<");
                  if (isEndTag) {
                    currentBuilder.append('/');
                  }
                  currentBuilder.append(command);
                  currentBuilder.append(">");
                }
              } else {
                index = indexStart3;
                break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3;
              break;
            }
            int indexStart2 = index;
            int lineBreakCount = 0;
            for (int i2 = 0; true; ++i2) {
              indexTemp3 = index;
              do {
                if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1)
                  == 10) {
                  indexTemp3 += 2;
                  break;
                }
                if (index < endIndex && ((str.charAt(index) == 13) || (str.charAt(index)
                == 10))) {
                  ++indexTemp3;
                  break;
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
                  currentBuilder.append("<br>");
                }
              } else {
                if (lineBreakCount == 1) {
                  currentBuilder.append(' ');
                } else if (!withinParam) {
                  if (lineBreakCount == 2) {
                    currentBuilder.append("<br>");
                  } else {
                    int j = 0;
                    currentBuilder.append("<p>");
                    for (j = 2; j < lineBreakCount; ++j) {
                      currentBuilder.append("<br>");
                    }
                  }
                } else {
                  currentBuilder.append(' ');
                }
              }
              indexTemp2 = indexTemp3;
              index = indexStart2;
              break;
            }
            if (index < endIndex && ((str.charAt(index) >= 0 && str.charAt(index) <=
            9) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) || (str.charAt(index) >= 14 &&
            str.charAt(index) <= 127))) {
              // Ordinary character
              if (str.charAt(index) == 0) {
                // Null
                currentBuilder.append((char)0xfffd);
              } else if (str.charAt(index) == '&') {
                currentBuilder.append(withinParam ? "&" : "&amp;");
              } else {
                currentBuilder.append(str.charAt(index));
              }
              ++indexTemp2;
              break;
            }
            indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index)
          <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <=
              65535))) {
                // BMP character
                currentBuilder.append(str.charAt(index));
                ++indexTemp3;
                break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                // Supplementary character
                currentBuilder.append(str.charAt(index));
                currentBuilder.append(str.charAt(index + 1));
                indexTemp3 += 2;
                break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3;
              break;
            }
            if (index < endIndex && (str.charAt(index) >= 55296 && str.charAt(index)
            <= 57343)) {
              // Unpaired surrogate
              currentBuilder.append((char)0xfffd);
              ++indexTemp2;
              break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
      } while (false);
      originalBuilder.append("</body></html>");
      return originalBuilder.toString();
    }
  }
