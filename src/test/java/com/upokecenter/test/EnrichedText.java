package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

    /**
     * Not documented yet.
     */
  public final class EnrichedText {
private EnrichedText() {
}
    /**
     * Not documented yet.
     * @param str The parameter {@code str} is not documented yet.
     * @param index The parameter {@code index} is not documented yet.
     * @param endIndex The parameter {@code endIndex} is not documented yet.
     * @return A string object.
     */
    private static String ParseColor(String str, int index, int endIndex) {
      if (index + 2 == endIndex - 1 && (str.charAt(index) & ~32) == 82 &&
        (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 68) {
        return str.substring(index, (index)+(endIndex - index));
      }
      if (index + 3 == endIndex - 1 && (str.charAt(index) & ~32) == 66 && (str.charAt(index +

   1) & ~32) == 76 && (str.charAt(index + 2) & ~32) == 85 && (str.charAt(index + 3) & ~32)
                   == 69) {
        return str.substring(index, (index)+(endIndex - index));
      }
      if (index + 4 == endIndex - 1 && (str.charAt(index) & ~32) == 71 &&
      (str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) == 69 &&
      (str.charAt(index + 3) & ~32) == 69 && (str.charAt(index + 4) & ~32) == 78) {
        return str.substring(index, (index)+(endIndex - index));
      }
      if (index + 5 == endIndex - 1 && (str.charAt(index) & ~32) == 89 &&
      (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 76 &&
      (str.charAt(index + 3) & ~32) == 76 && (str.charAt(index + 4) & ~32) == 79 &&
      (str.charAt(index + 5) & ~32) == 87) {
        return str.substring(index, (index)+(endIndex - index));
      }
      if (index + 3 == endIndex - 1 && (str.charAt(index) & ~32) == 67 &&
      (str.charAt(index + 1) & ~32) == 89 && (str.charAt(index + 2) & ~32) == 65 &&
      (str.charAt(index + 3) & ~32) == 78) {
        return str.substring(index, (index)+(endIndex - index));
      }
      if (index + 6 == endIndex - 1 && (str.charAt(index) & ~32) == 77 &&
      (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 71 &&
      (str.charAt(index + 3) & ~32) == 69 && (str.charAt(index + 4) & ~32) == 78 &&
      (str.charAt(index + 5) & ~32) == 84 && (str.charAt(index + 6) & ~32) == 65) {
        return str.substring(index, (index)+(endIndex - index));
      }
      if (index + 4 == endIndex - 1 && (str.charAt(index) & ~32) == 66 &&
      (str.charAt(index + 1) & ~32) == 76 && (str.charAt(index + 2) & ~32) == 65 &&
      (str.charAt(index + 3) & ~32) == 67 && (str.charAt(index + 4) & ~32) == 75) {
        return str.substring(index, (index)+(endIndex - index));
      }
      if (index + 4 == endIndex - 1 && (str.charAt(index) & ~32) == 87 &&
      (str.charAt(index + 1) & ~32) == 72 && (str.charAt(index + 2) & ~32) == 73 &&
      (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 69) {
        return str.substring(index, (index)+(endIndex - index));
      }
      {
        if (index + 3 < endIndex && (((str.charAt(index) >= 65 && str.charAt(index) <=
               70) || (str.charAt(index) >= 97 && str.charAt(index) <= 102) || (str.charAt(index) >=
               48 && str.charAt(index) <= 57)) && ((str.charAt(index + 1) >= 65 && str.charAt(index +
               1) <=
70) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102) || (str.charAt(index + 1) >=
 48 && str.charAt(index + 1) <= 57)) && ((str.charAt(index + 2) >=

               65 &&
           str.charAt(index + 2) <= 70) || (str.charAt(index + 2) >= 97 && str.charAt(index +
             2) <= 102) || (str.charAt(index + 2) >= 48 && str.charAt(index + 2) <=
             57)) && ((str.charAt(index + 3) >= 65 && str.charAt(index + 3) <= 70) ||
    (str.charAt(index + 3) >= 97 && str.charAt(index + 3) <= 102) || (str.charAt(index + 3) >= 48 &&

               str.charAt(index + 3) <= 57)))) {
          index += 4;
        } else {
          return null;
        }
   if (index + 4 < endIndex && (str.charAt(index) == 44) && (((str.charAt(index + 1) >= 65
          &&
    str.charAt(index + 1) <= 70) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102) ||
          (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <=

     57)) && ((str.charAt(index + 2) >= 65 && str.charAt(index + 2) <= 70) || (str.charAt(index +
          2) >= 97 && str.charAt(index + 2) <= 102) || (str.charAt(index + 2) >= 48 &&

   str.charAt(index + 2) <= 57)) && ((str.charAt(index + 3) >= 65 && str.charAt(index + 3) <= 70) ||
          (str.charAt(index + 3) >= 97 && str.charAt(index + 3) <= 102) ||

   (str.charAt(index + 3) >= 48 && str.charAt(index + 3) <= 57)) && ((str.charAt(index + 4) >= 65
          &&
    str.charAt(index + 4) <= 70) || (str.charAt(index + 4) >= 97 && str.charAt(index + 4) <= 102) ||
          (str.charAt(index + 4) >= 48 && str.charAt(index + 4) <=

                    57)))) {
          index += 5;
        } else {
          return null;
        }
    if (index + 4 == endIndex - 1 && (str.charAt(index) == 44) && (((str.charAt(index + 1)
          >= 65 && str.charAt(index + 1) <= 70) || (str.charAt(index + 1) >= 97
&&
                str.charAt(index + 1) <= 102) || (str.charAt(index + 1) >= 48 &&
                    str.charAt(index +
                    1) <= 57)) && ((str.charAt(index + 2) >= 65 && str.charAt(index + 2) <=
   70) || (str.charAt(index + 2) >= 97 && str.charAt(index + 2) <= 102) || (str.charAt(index + 2)
          >= 48 && str.charAt(index + 2) <= 57)) && ((str.charAt(index + 3) >= 65 &&

                  str.charAt(index + 3) <= 70) || (str.charAt(index + 3) >= 97 &&
                    str.charAt(index +
                    3) <= 102) || (str.charAt(index + 3) >= 48 && str.charAt(index + 3) <=
                    57)) && ((str.charAt(index + 4) >= 65 && str.charAt(index + 4) <= 70) ||
    (str.charAt(index + 4) >= 97 && str.charAt(index + 4) <= 102) || (str.charAt(index + 4) >= 48
               &&

                    str.charAt(index + 4) <= 57)))) {
          index += 5;
        } else {
          return null;
        }
        String ret = "#";
        return
  ret + str.substring(index, (index)+(2)) + str.substring(index + 5, (index + 5)+(2)) +
    str.substring(
  index, (
  index)+(10));
      }
    }

    /**
     * Not documented yet.
     * @param str The parameter {@code str} is not documented yet.
     * @param index The parameter {@code index} is not documented yet.
     * @param endIndex The parameter {@code endIndex} is not documented yet.
     * @return A 32-bit signed integer.
     */
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
              ++indexTemp2; break;
            }
            int indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index)
          <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <=
              65535))) {
                ++indexTemp3; break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
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

    /**
     * Not documented yet.
     * @param str The parameter {@code str} is not documented yet.
     * @param index The parameter {@code index} is not documented yet.
     * @param endIndex The parameter {@code endIndex} is not documented yet.
     * @return A 32-bit signed integer.
     */
    private static int SkipLang(String str, int index, int endIndex) {
      while (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) ||
            (str.charAt(index) == 45) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) ||
          (str.charAt(index) >= 65 && str.charAt(index) <= 90))) {
        ++index;
      }
      return index;
    }

    /**
     * Not documented yet.
     * @param str Not documented yet.
     * @param delimiter Not documented yet.
     * @return A string[] object.
     * @throws NullPointerException The parameter {@code delimiter} is null.
     * @throws IllegalArgumentException Delimiter is empty.
     */
    private static String[] SplitAt(String str, String delimiter) {
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
          String newstr = str.substring(index, (index)+((index2) - index));
          strings = (strings == null) ? ((new ArrayList<String>())) : strings;
          strings.add(newstr);
          index = index2 + delimLength;
        }
      }
      return strings.toArray(new String[] { });
    }

    /**
     * Not documented yet.
     * @param s The parameter {@code s} is not documented yet.
     * @return A string object.
     */
    private static String TrimSpaces(String s) {
      if (s == null || s.length() == 0) {
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

    /**
     * Not documented yet.
     * @param str The parameter {@code str} is not documented yet.
     * @param index The parameter {@code index} is not documented yet.
     * @param endIndex The parameter {@code endIndex} is not documented yet.
     * @return A string object.
     */
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
              indexTemp2 += 2; break;
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
                  index = indexStart3; break;
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
                String command = str.substring(
  commandStart, (
  commandStart)+(commandEnd - commandStart)).toLowerCase();
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
                    paramBuilder.delete(0, (0)+(paramBuilder.length()));
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
            int lineBreakCount = 0;
            for (int i2 = 0; ; ++i2) {
              indexTemp3 = index;
              do {
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) ==
                  10) {
                  indexTemp3 += 2; break;
                }
                if (index < endIndex && ((str.charAt(index) == 13) || (str.charAt(index)
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
              ++indexTemp2; break;
            }
            indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index)
          <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <=
              65535))) {
                // BMP character
                currentBuilder.append(str.charAt(index));
                ++indexTemp3; break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                // Supplementary character
                currentBuilder.append(str.charAt(index));
                currentBuilder.append(str.charAt(index + 1));
                indexTemp3 += 2; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str.charAt(index) >= 55296 && str.charAt(index)
            <= 57343)) {
              // Unpaired surrogate
              currentBuilder.append((char)0xfffd);
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
      return originalBuilder.toString();
    }

    /**
     * Not documented yet.
     * @param str The parameter {@code str} is not documented yet.
     * @param index The parameter {@code index} is not documented yet.
     * @param endIndex The parameter {@code endIndex} is not documented yet.
     * @return A string object.
     */
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
  originalBuilder.append("<style>p { margin-bottom: 0em; margin-top: 0em; }");
      originalBuilder.append("</style><body>");
      String lastCommand = "";
      do {
        while (true) {
          int indexTemp2 = index;
          do {
        if (index + 1 < endIndex && str.charAt(index) == 60 && str.charAt(index + 1) ==
              60) {
              currentBuilder.append("&lt;");
              indexTemp2 += 2; break;
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
                  index = indexStart3; break;
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
                String command = str.substring(
  commandStart, (
  commandStart)+(commandEnd - commandStart)).toLowerCase();
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
                    currentBuilder.append("<span style='color: " + p + "'>");
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
                    String[] valuePList = SplitAt(p, ",");
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
                    paramBuilder.delete(0, (0)+(paramBuilder.length()));
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
                index = indexStart3; break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            int indexStart2 = index;
            int lineBreakCount = 0;
            for (int i2 = 0; ; ++i2) {
              indexTemp3 = index;
              do {
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) ==
                  10) {
                  indexTemp3 += 2; break;
                }
                if (index < endIndex && ((str.charAt(index) == 13) || (str.charAt(index)
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
              ++indexTemp2; break;
            }
            indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index)
          <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <=
              65535))) {
                // BMP character
                currentBuilder.append(str.charAt(index));
                ++indexTemp3; break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                // Supplementary character
                currentBuilder.append(str.charAt(index));
                currentBuilder.append(str.charAt(index + 1));
                indexTemp3 += 2; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str.charAt(index) >= 55296 && str.charAt(index)
            <= 57343)) {
              // Unpaired surrogate
              currentBuilder.append((char)0xfffd);
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
      originalBuilder.append("</body></html>");
      return originalBuilder.toString();
    }
  }
