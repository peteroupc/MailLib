package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;

  final class HeaderParserUtility {
private HeaderParserUtility() {
}
    static final int TokenComment = 2;
    static final int TokenPhraseAtom = 3;
    static final int TokenPhraseAtomOrDot = 4;
    static final int TokenPhrase = 1;
    static final int TokenGroup = 5;
    static final int TokenMailbox = 6;
    static final int TokenQuotedString = 7;
    static final int TokenLocalPart = 8;
    static final int TokenDomain = 9;

    public static int ParseHeaderExpandedDate(
  String str,
  int index,
  int endIndex,
  int[] ret) {
      int i, i3, indexStart, indexStart2, indexStart3, indexTemp,
        indexTemp2, indexTemp3, indexTemp4;
      int dayOfWeek = -1, day = -1, month = -1, year = -1, hour = -1, minute
        = -1, second = -1, offset = -1, yearDigits = 0;
      indexStart = index;
      indexTemp = index;
      do {
        do {
          indexTemp2 = index;
          do {
            indexStart2 = index;
            index = HeaderParser.ParseCFWS(str, index, endIndex, null);
            do {
              indexTemp3 = index;
              do {
                if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
                  (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) ==
                  78) {
                  dayOfWeek = 1;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str.charAt(index) & ~32) == 84 &&
                  (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) ==
                  69) {
                  dayOfWeek = 2;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str.charAt(index) & ~32) == 87 &&
                  (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) ==
                  68) {
                  dayOfWeek = 3;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str.charAt(index) & ~32) == 84 &&
                  (str.charAt(index + 1) & ~32) == 72 && (str.charAt(index + 2) & ~32) ==
                  85) {
                  dayOfWeek = 4;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str.charAt(index) & ~32) == 70 &&
                  (str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) ==
                  73) {
                  dayOfWeek = 5;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str.charAt(index) & ~32) == 83 &&
                  (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) ==
                  84) {
                  dayOfWeek = 6;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str.charAt(index) & ~32) == 83 &&
                  (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) ==
                  78) {
                  dayOfWeek = 0;
                  indexTemp3 += 3; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            index = HeaderParser.ParseCFWS(str, index, endIndex, null);
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        } while (false);
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        day = 0;
        for (i = 0; i < 2; ++i) {
          if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            day *= 10;
            day += ((int)str.charAt(index)) - 48;
            if (day == 0) {
              return indexStart;
            }
            ++index;
          } else if (i < 1) {
            index = indexStart; break;
          } else {
 break;
}
        }
        if (index == indexStart) {
 break;
}
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        do {
          indexTemp2 = index;
          do {
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 74 &&
              (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 78) {
              month = 1;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 70 &&
              (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 66) {
              month = 2;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
              (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 82) {
              month = 3;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 65 &&
              (str.charAt(index + 1) & ~32) == 80 && (str.charAt(index + 2) & ~32) == 82) {
              month = 4;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
              (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 89) {
              month = 5;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 74 &&
              (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 78) {
              month = 6;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 74 &&
              (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 76) {
              month = 7;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 65 &&
              (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 71) {
              month = 8;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 83 &&
              (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 80) {
              month = 9;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 79 &&
              (str.charAt(index + 1) & ~32) == 67 && (str.charAt(index + 2) & ~32) == 84) {
              month = 10;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 78 &&
              (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 86) {
              month = 11;
              indexTemp2 += 3; break;
            }
            if (index + 2 < endIndex && (str.charAt(index) & ~32) == 68 &&
              (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 67) {
              month = 12;
              indexTemp2 += 3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        yearDigits = 0;
        year = 0;
        if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) ||
          (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
          year *= 10;
          year += ((int)str.charAt(index)) - 48;
          year *= 10;
          year += ((int)str.charAt(index + 1)) - 48;
          index += 2;
          yearDigits += 2;
        } else {
          index = indexStart; break;
        }
        while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          yearDigits = Math.min(yearDigits + 1, 4);
          if (year > Integer.MAX_VALUE / 10) {
            // year would overflow
            return indexStart;
          }
          year *= 10;
          if (year > Integer.MAX_VALUE - 10) {
            // year would overflow
            return indexStart;
          }
          year += ((int)str.charAt(index)) - 48;
          ++index;
        }
        if (yearDigits == 3 || (yearDigits == 2 && year >= 50)) {
          year += 1900;
        } else if (yearDigits == 2) {
          year += 2000;
        }
        boolean leap = year % 4 == 0 && (year % 100 != 0 || year %
          400 == 0);
        if (month == 4 || month == 6 || month == 9 || month == 11) {
          if (day > 30) {
 return indexStart;
}
        } else if (month == 2) {
          if (day > (leap ? 29 : 28)) {
 return indexStart;
}
        } else {
          if (day > 31) {
 return indexStart;
}
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        hour = minute = second = 0;
        if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) ||
          (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
          hour *= 10;
          hour += ((int)str.charAt(index)) - 48;
          hour *= 10;
          hour += ((int)str.charAt(index + 1)) - 48;
          if (hour >= 24) {
            return indexStart;
          }
          index += 2;
        } else {
          index = indexStart; break;
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && (str.charAt(index) == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) ||
          (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
          minute *= 10;
          minute += ((int)str.charAt(index)) - 48;
          minute *= 10;
          minute += ((int)str.charAt(index + 1)) - 48;
          if (minute >= 60) {
            return indexStart;
          }
          minute = Math.min(59, minute);
          index += 2;
        } else {
          index = indexStart; break;
        }
        second = 0;
        do {
          indexTemp2 = index;
          do {
            indexStart2 = index;
            index = HeaderParser.ParseCFWS(str, index, endIndex, null);
            if (index < endIndex && (str.charAt(index) == 58)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = HeaderParser.ParseCFWS(str, index, endIndex, null);
            if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <=
              57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
              second *= 10;
              second += ((int)str.charAt(index)) - 48;
              second *= 10;
              second += ((int)str.charAt(index + 1)) - 48;
              if (second >= 61) {
                return indexStart;
              }
              index += 2;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        } while (false);
        do {
          indexTemp2 = index;
          do {
            indexTemp3 = index;
            do {
              indexStart3 = index;
              for (i3 = 0; true; ++i3) {
                indexTemp4 = HeaderParser.ParseFWS(str, index, endIndex, null);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else {
                  if (i3 < 1) {
                    index = indexStart3;
                  }
                  break;
                }
              }
              if (index == indexStart3) {
                break;
              }
              boolean minus = false;
          if (index < endIndex && ((str.charAt(index) == 43) || (str.charAt(index) ==
                45))) {
                minus = str.charAt(index) == 45;
                ++index;
              } else {
                index = indexStart3; break;
              }
              if (index + 3 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index)
                <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) ||
                (str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index +
                3) >= 48 && str.charAt(index + 3) <= 57))) {
       int offsethr = (((int)str.charAt(index) - 48) * 10) +
                ((int)str.charAt(index + 1) - 48);
  int offsetmin = (((int)str.charAt(index + 2) - 48) * 10) +
                ((int)str.charAt(index + 3) - 48);
                if (offsetmin >= 60) {
                  return indexStart;
                }
                offset = (offsethr * 60) + offsetmin;
                offset %= 1440;
                if (minus) {
 offset = -offset;
}
                index += 4;
              } else {
                index = indexStart3; break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = index;
            do {
              indexStart3 = index;
              index = HeaderParser.ParseCFWS(str, index, endIndex, null);
              do {
                indexTemp4 = index;
                do {
                  if (index + 1 < endIndex && (str.charAt(index) & ~32) == 85 &&
                    (str.charAt(index + 1) & ~32) == 84) {
                    offset = 0;
                    indexTemp4 += 2; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 71 &&
                    (str.charAt(index + 1) & ~32) == 77 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = 0;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -5 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -4 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -6 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -5 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -7 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -6 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -8 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -7 * 60;
                    indexTemp4 += 3; break;
                  }
                  if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index)
                    <= 73) || (str.charAt(index) >= 75 && str.charAt(index) <= 90) ||
                    (str.charAt(index) >= 97 && str.charAt(index) <= 105) || (str.charAt(index)
                    >= 107 && str.charAt(index) <= 122))) {
                    offset = 0;
                    ++indexTemp4; break;
                  }
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else {
                  index = indexStart3; break;
                }
              } while (false);
              if (index == indexStart3) {
                break;
              }
              index = HeaderParser.ParseCFWS(str, index, endIndex, null);
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (indexStart != indexTemp) {
        ret[0] = year;
        ret[1] = month;
        ret[2] = day;
        ret[3] = hour;
        ret[4] = minute;
        ret[5] = second;
        ret[6] = 0;
        ret[7] = offset;
        if (dayOfWeek >= 0) {
          int dow = DateTimeUtilities.GetDayOfWeek(ret);
          if (dow != dayOfWeek) {
            return indexStart;
          }
        }
      }
      return indexTemp;
    }

    private static boolean ShouldQuote(String str) {
      if (str.length() == 0) {
        // Empty String
        return true;
      }
      if (str.charAt(str.length() - 1) == ' ' || str.charAt(str.length() - 1) == '\t') {
        // Space or tab at end
        return true;
      }
      if (str.charAt(0) == ' ' || str.charAt(0) == '\t') {
        // Space or tab at beginning
        return true;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '\\' || str.charAt(i) == '"') {
          return true;
        }
        if ((str.charAt(i) == ' ' || str.charAt(i) == '\t') && i + 1 < str.length() &&
            (str.charAt(i + 1) == ' ' || str.charAt(i + 1) == '\t')) {
          // run of two or more space and/or tab
          return true;
        }
        if ((str.charAt(i) == '\r') && i + 1 < str.length() &&
            (str.charAt(i + 1) == '\n')) {
          // CRLF
          if (i == 0 && i + 2 < str.length() && (str.charAt(i + 1) == ' ' || str.charAt(i +
            1) == '\t')) {
            // CRLF followed by space or tab at beginning
            return true;
          }
          continue;
        }
        char c = str.charAt(i);
        // Has specials, or CTLs other than tab
        if ((c < 0x20 && c != '\t') || c == 0x7F || c == 0x28 || c == 0x29 ||
          c == 0x3c || c == 0x3e || c == 0x5b || c == 0x5d || c == 0x3a || c
            == 0x3b || c == 0x40 ||
              c == 0x5c || c == 0x2c || c == 0x2e || c == '"') {
          return true;
        }
      }
      return false;
    }

    public static String QuoteValueIfNeeded(String str) {
      return (!ShouldQuote(str)) ? str : QuoteValue(str);
    }

    public static String ParseGroupList(String str, int index, int endIndex) {
      // NOTE: Assumes the String matches the production "group"
      int tmp = HeaderParser.ParsePhrase(str, index, endIndex, null);
      if (tmp == index) {
        return "";
      }
      index = tmp;
      if (index < endIndex && str.charAt(index) == ':') {
        ++index;
      } else {
        return "";
      }
      tmp = HeaderParser.ParseGroupList(str, index, endIndex, null);
      return str.substring(index, (index)+(tmp - index));
    }

    public static String QuoteValue(String str) {
      // Quotes a String according to RFC 5322 rules.
      StringBuilder builder = new StringBuilder();
      builder.append('"');
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '\\' || str.charAt(i) == '"') {
          builder.append('\\');
          builder.append(str.charAt(i));
        } else {
          builder.append(str.charAt(i));
        }
      }
      builder.append('"');
      return builder.toString();
    }

    private static String ParseDotAtomAfterCFWS(
  String str,
  int index,
  int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a dot-atom
      StringBuilder builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          break;
        }
        builder.append(str.substring(start, (start)+(index - start)));
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && str.charAt(index) == '.') {
          builder.append('.');
          ++index;
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
      }
      return builder.toString();
    }

    private static String ParseDotWordAfterCFWS(
  String str,
  int index,
  int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a word separated by dots
      StringBuilder builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          if (index < endIndex && str.charAt(index) == '"') {
            // it's a quoted String instead
            index = MediaType.SkipQuotedString(str, index, endIndex, builder);
          } else {
            break;
          }
        } else {
          builder.append(str.substring(start, (start)+(index - start)));
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && str.charAt(index) == '.') {
          builder.append('.');
          ++index;
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
      }
      return builder.toString();
    }

    public static String ParseLocalPart(String str, int index, int endIndex) {
      // NOTE: Assumes the String matches the production "local-part"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      return ParseDotWordAfterCFWS(str, index, endIndex);
    }

    public static String ParseDomain(String str, int index, int endIndex) {
      // NOTE: Assumes the String matches the production "domain"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index < endIndex && str.charAt(index) == '[') {
        // It's a domain literal
        ++index;
        StringBuilder builder = new StringBuilder();
        builder.append('[');
        while (index < endIndex) {
          index = HeaderParser.ParseFWS(str, index, endIndex, null);
          if (index >= endIndex) {
            break;
          }
          if (str.charAt(index) == ']') {
            break;
          }
          if (str.charAt(index) == '\\') {
            int startQuote = index;
            index = HeaderParser.ParseQuotedPair(str, index, endIndex, null);
            if (index == startQuote) {
       builder.append(
  str.substring(
  startQuote + 1, (
  startQuote + 1)+(index - (startQuote + 1))));
            } else {
              ++index;
            }
          } else {
            builder.append(str.charAt(index));
            ++index;
          }
        }
        builder.append(']');
        return builder.toString();
      }
      // It's a dot-atom
      return ParseDotAtomAfterCFWS(str, index, endIndex);
    }

    public static List<NamedAddress> ParseAddressList(
  String str,
  int index,
  int endIndex,
  List<int[]> tokens) {
      int lastIndex = index;
      List<NamedAddress> addresses = new ArrayList<NamedAddress>();
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= lastIndex && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
          if (tokenKind == TokenGroup) {
            addresses.add(ParseGroup(str, tokenIndex, tokenEnd, tokens));
            lastIndex = tokenEnd;
          } else if (tokenKind == TokenMailbox) {
            try {
              addresses.add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
            } catch (IndexOutOfRangeException ex) {
    throw new IllegalStateException(
  "str=" + str + " index=" + index,
  ex);
                    }
            lastIndex = tokenEnd;
          }
        }
      }
      return addresses;
    }

    public static NamedAddress ParseAddress(
  String str,
  int index,
  int endIndex,
  List<int[]> tokens) {
      int lastIndex = index;
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= lastIndex && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
          if (tokenKind == TokenGroup) {
            return ParseGroup(str, tokenIndex, tokenEnd, tokens);
          }
          if (tokenKind == TokenMailbox) {
            return ParseMailbox(str, tokenIndex, tokenEnd, tokens);
          }
        }
      }
      return null;
    }

    public static NamedAddress ParseGroup(
  String str,
  int index,
  int endIndex,
  List<int[]> tokens) {
      String displayName = null;
      boolean haveDisplayName = false;
      List<NamedAddress> mailboxes = new ArrayList<NamedAddress>();
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
          if (tokenKind == TokenPhrase && !haveDisplayName) {
            // Phrase
            displayName = Rfc2047.DecodePhraseText(
              str,
              tokenIndex,
              tokenEnd,
              tokens,
              false);
            // Set haveDisplayName, which needs to be done because
            // the mailboxes that follow may themselves have display names
            haveDisplayName = true;
          } else if (tokenKind == TokenMailbox) {
            mailboxes.add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
          }
        }
      }
      return new NamedAddress(displayName, mailboxes);
    }

    public static NamedAddress ParseMailbox(
  String str,
  int index,
  int endIndex,
  List<int[]> tokens) {
      String displayName = null;
      String localPart = null;
      String domain = null;
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
          switch (tokenKind) {
            case TokenPhrase:
              // Phrase
              displayName = Rfc2047.DecodePhraseText(
                str,
                tokenIndex,
                tokenEnd,
                tokens,
                false);
              break;
            case TokenLocalPart:
              localPart = ParseLocalPart(str, tokenIndex, tokenEnd);
              break;
            case TokenDomain:
              // NOTE: Domain will end up as the last domain token,
              // even if the mailbox contains obsolete route syntax
              domain = ParseDomain(str, tokenIndex, tokenEnd);
              break;
          }
        }
      }

      return new NamedAddress(displayName, localPart, domain);
    }

    // Parses a comment using the obsolete syntax.
    static int ParseCommentLax(
  String str,
  int index,
  int endIndex,
  ITokener tokener) {
      int indexStart = index;
      int depth = 0;
      if (index < endIndex && (str.charAt(index) == 40)) {
        ++index;
      } else {
        return index;
      }
      int state = (tokener != null) ? tokener.GetState() : 0;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = HeaderParser.ParseFWS(str, index, endIndex, null);
            do {
              int indexTemp3 = index;
              do {
                if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <=
                  55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
                  str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 &&
                  str.charAt(index + 1) <= 57343))) {
                  indexTemp3 += 2; break;
                }
                int indexTemp4;
      indexTemp4 = HeaderParser.ParseQuotedPair(
  str,
  index,
  endIndex,
  null);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                if (index < endIndex && ((str.charAt(index) >= 1 && str.charAt(index) <=
                  8) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) ||
                  (str.charAt(index) >= 14 && str.charAt(index) <= 31) || (str.charAt(index) ==
                  127))) {
                  ++indexTemp3; break;
                }
                if (index < endIndex && ((str.charAt(index) >= 93 && str.charAt(index) <=
                  126) || (str.charAt(index) >= 42 && str.charAt(index) <= 91) ||
                  (str.charAt(index) >= 33 && str.charAt(index) <= 39))) {
                  ++indexTemp3; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = HeaderParser.ParseFWS(str, index, endIndex, null);
        if (index < endIndex && str.charAt(index) == 41) {
          // End of current comment
          ++index;
          if (depth == 0) {
            if (tokener != null) {
              tokener.Commit(TokenComment, indexStart, index);
            }
            return index;
          }
          --depth;
        } else if (index < endIndex && str.charAt(index) == 40) {
          // Start of nested comment
          ++index;
          ++depth;
        } else {
          if (tokener != null) {
            tokener.RestoreState(state);
          }
          return indexStart;
        }
      } while (true);
    }

    // Parses a comment without using the obsolete syntax.
  static int ParseCommentStrict(
  String str,
  int index,
  int endIndex) {
      if (index < endIndex && (str.charAt(index) == 40)) {
        ++index;
      } else {
        return index;
      }

      int indexStart = index;
      int depth = 0;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                do {
                  int indexTemp4;
                  indexTemp4 = index;
                  do {
                    int indexStart4 = index;
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                    9))) {
                    ++index;
                    }
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) ==
                    10) {
                    index += 2;
                    } else {
                    index = indexStart4; break;
                    }
                    indexTemp4 = index;
                    index = indexStart4;
                  } while (false);
                  if (indexTemp4 != index) {
                    index = indexTemp4;
                  } else { break;
                  }
                } while (false);
           if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                  9))) {
                  ++index;
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                    9))) {
                    ++index;
                  }
                } else {
                  index = indexStart3; break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            do {
              int indexTemp3 = index;
              do {
                if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <=
                  55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
                  str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 &&
                  str.charAt(index + 1) <= 57343))) {
                  indexTemp3 += 2; break;
                }
                int indexTemp4;
                indexTemp4 = index;
                do {
                  int indexStart4 = index;
                  if (index < endIndex && (str.charAt(index) == 92)) {
                    ++index;
                  } else {
                    break;
                  }
                  do {
                    int indexTemp5;
                    indexTemp5 = index;
                    do {
                    if (index < endIndex && ((str.charAt(index) == 32) ||
                    (str.charAt(index) == 9) || (str.charAt(index) >= 128 &&
                    str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 &&
                    str.charAt(index) <= 65535))) {
                    ++indexTemp5; break;
                    }
                    if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
                    str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 &&
                    str.charAt(index + 1) <= 57343))) {
                    indexTemp5 += 2; break;
                    }
             if (index < endIndex && (str.charAt(index) >= 33 && str.charAt(index) <=
                    126)) {
                    ++indexTemp5; break;
                    }
                    } while (false);
                    if (indexTemp5 != index) {
                    index = indexTemp5;
                    } else {
                    index = indexStart4; break;
                    }
                  } while (false);
                  if (index == indexStart4) {
                    break;
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                if (index < endIndex && ((str.charAt(index) >= 93 && str.charAt(index) <=
                  126) || (str.charAt(index) >= 42 && str.charAt(index) <= 91) ||
                  (str.charAt(index) >= 33 && str.charAt(index) <= 39))) {
                  ++indexTemp3; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                  9))) {
                  ++index;
                }
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) ==
                  10) {
                  index += 2;
                } else {
                  index = indexStart3; break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
              ++index;
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                9))) {
                ++index;
              }
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        if (index < endIndex && str.charAt(index) == 41) {
          // End of current comment
          ++index;
          if (depth == 0) {
            return index;
          }
          --depth;
        } else if (index < endIndex && str.charAt(index) == 40) {
          // Start of nested comment
          ++index;
          ++depth;
        } else {
          return indexStart;
        }
      } while (true);
    }
  }
