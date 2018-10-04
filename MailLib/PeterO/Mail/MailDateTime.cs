using System;
using System.Text;
using PeterO;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.MailDateTime"]/*'/>
  public static class MailDateTime {
    private static string[] valueDaysOfWeek = {
      "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"
    };

    private static string[] valueMonths = {
      String.Empty, "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul",
      "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.MailDateTime.GenerateDateString(System.Int32[])"]/*'/>
    public static string GenerateDateString(int[] dateTime) {
      return GenerateDateString(dateTime, false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.MailDateTime.GenerateDateString(System.Int32[],System.Boolean)"]/*'/>
    public static string GenerateDateString(int[] dateTime, bool gmt) {
      if (!DateTimeUtilities.IsValidDateTime(dateTime) ||
        dateTime[0] < 0) {
        throw new ArgumentException("Invalid date and time");
      }
      if (gmt && dateTime[7] != 0) {
        // Use time offset to convert local time to UTC/GMT
        var newDateTime = new int[8];
        Array.Copy(dateTime, 0, newDateTime, 0, 7);
        DateTimeUtilities.AddMinutes(newDateTime, -dateTime[7]);
        dateTime = newDateTime;
      }
      int dow = DateTimeUtilities.GetDayOfWeek(dateTime);
      if (dow < 0) {
        throw new ArgumentException("Invalid date and time");
      }
      string dayString = valueDaysOfWeek[dow];
      string monthString = valueMonths[dateTime[1]];
      var sb = new StringBuilder();
      sb.Append(dayString);
      sb.Append(", ");
      sb.Append((char)('0' + ((dateTime[2] / 10) % 10)));
      sb.Append((char)('0' + (dateTime[2] % 10)));
      sb.Append(' ');
      sb.Append(monthString);
      sb.Append(' ');
      string yearString = ParserUtility.IntToString(dateTime[0]);
      if (yearString.Length < 4) {
        for (int i = 0; i < 4 - yearString.Length; ++i) {
          sb.Append('0');
        }
      }
      sb.Append(yearString);
      sb.Append(' ');
      sb.Append((char)('0' + ((dateTime[3] / 10) % 10)));
      sb.Append((char)('0' + (dateTime[3] % 10)));
      sb.Append(':');
      sb.Append((char)('0' + ((dateTime[4] / 10) % 10)));
      sb.Append((char)('0' + (dateTime[4] % 10)));
      sb.Append(':');
      sb.Append((char)('0' + ((dateTime[5] / 10) % 10)));
      sb.Append((char)('0' + (dateTime[5] % 10)));
      sb.Append(' ');
      if (gmt) {
        sb.Append("GMT");
      } else {
        int offset = dateTime[7];
        sb.Append((offset < 0) ? '-' : '+');
        offset = Math.Abs(offset);
        int hours = (offset / 60) % 24;
        int minutes = offset % 60;
        sb.Append((char)('0' + ((hours / 10) % 10)));
        sb.Append((char)('0' + (hours % 10)));
        sb.Append((char)('0' + ((minutes / 10) % 10)));
        sb.Append((char)('0' + (minutes % 10)));
      }
      return sb.ToString();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)"]/*'/>
    public static int[] ParseDateString(string str, bool parseObsoleteZones) {
      if (String.IsNullOrEmpty(str)) {
 return null;
}
      var ret = new int[8];
      if (
  ParseHeaderExpandedDate(
  str,
  0,
  str.Length,
  ret,
  parseObsoleteZones) == str.Length) {
        return ret;
      } else {
        return null;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.MailDateTime.ParseDateString(System.String)"]/*'/>
    public static int[] ParseDateString(string str) {
      return ParseDateString(str, false);
    }

    internal static int ParseHeaderExpandedDate(
  string str,
  int index,
  int endIndex,
  int[] ret,
  bool parseObsoleteZones) {
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
                if (index + 2 < endIndex && (str[index] & ~32) == 77 &&
                  (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) ==
                  78) {
                  dayOfWeek = 1;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 84 &&
                  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) ==
                  69) {
                  dayOfWeek = 2;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 87 &&
                  (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) ==
                  68) {
                  dayOfWeek = 3;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 84 &&
                  (str[index + 1] & ~32) == 72 && (str[index + 2] & ~32) ==
                  85) {
                  dayOfWeek = 4;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 70 &&
                  (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) ==
                  73) {
                  dayOfWeek = 5;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 83 &&
                  (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) ==
                  84) {
                  dayOfWeek = 6;
                  indexTemp3 += 3; break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 83 &&
                  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) ==
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
            if (index < endIndex && (str[index] == 44)) {
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
          if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
            day *= 10;
            day += ((int)str[index]) - 48;
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
          if (index + 2 < endIndex && (str[index] & ~32) == 74 &&
            (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 78) {
            month = 1;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 70 &&
                (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) ==
                    66) {
            month = 2;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 77 &&
                (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) ==
                    82) {
            month = 3;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 65 &&
                (str[index + 1] & ~32) == 80 && (str[index + 2] & ~32) ==
                    82) {
            month = 4;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 77 &&
                (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) ==
                    89) {
            month = 5;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 74 &&
                (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) ==
                    78) {
            month = 6;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 74 &&
                (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) ==
                    76) {
            month = 7;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 65 &&
                (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) ==
                    71) {
            month = 8;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 83 &&
                (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) ==
                    80) {
            month = 9;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 79 &&
                (str[index + 1] & ~32) == 67 && (str[index + 2] & ~32) ==
                    84) {
            month = 10;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 78 &&
                (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) ==
                    86) {
            month = 11;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str[index] & ~32) == 68 &&
                (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) ==
                    67) {
            month = 12;
            indexTemp2 += 3;
          }
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
        if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
          (str[index + 1] >= 48 && str[index + 1] <= 57))) {
          year *= 10;
          year += ((int)str[index]) - 48;
          year *= 10;
          year += ((int)str[index + 1]) - 48;
          index += 2;
          yearDigits += 2;
        } else {
          index = indexStart; break;
        }
        while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
          yearDigits = Math.Min(yearDigits + 1, 4);
          if (year > Int32.MaxValue / 10) {
            // year would overflow
            return indexStart;
          }
          year *= 10;
          if (year > Int32.MaxValue - 10) {
            // year would overflow
            return indexStart;
          }
          year += ((int)str[index]) - 48;
          ++index;
        }
        if (yearDigits == 3 || (yearDigits == 2 && year >= 50)) {
          year += 1900;
        } else if (yearDigits == 2) {
          year += 2000;
        }
        bool leap = year % 4 == 0 && (year % 100 != 0 || year %
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
        if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
          (str[index + 1] >= 48 && str[index + 1] <= 57))) {
          hour *= 10;
          hour += ((int)str[index]) - 48;
          hour *= 10;
          hour += ((int)str[index + 1]) - 48;
          if (hour >= 24) {
            return indexStart;
          }
          index += 2;
        } else {
          index = indexStart; break;
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && (str[index] == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
          (str[index + 1] >= 48 && str[index + 1] <= 57))) {
          minute *= 10;
          minute += ((int)str[index]) - 48;
          minute *= 10;
          minute += ((int)str[index + 1]) - 48;
          if (minute >= 60) {
            return indexStart;
          }
          minute = Math.Min(59, minute);
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
            if (index < endIndex && (str[index] == 58)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = HeaderParser.ParseCFWS(str, index, endIndex, null);
            if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <=
              57) || (str[index + 1] >= 48 && str[index + 1] <= 57))) {
              second *= 10;
              second += ((int)str[index]) - 48;
              second *= 10;
              second += ((int)str[index + 1]) - 48;
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
              for (i3 = 0; ; ++i3) {
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
              var minus = false;
              if (index < endIndex && ((str[index] == 43) || (str[index] ==
                    45))) {
                minus = str[index] == 45;
                ++index;
              } else {
                index = indexStart3; break;
              }
              if (index + 3 < endIndex && ((str[index] >= 48 && str[index]
                <= 57) || (str[index + 1] >= 48 && str[index + 1] <= 57) ||
                (str[index + 2] >= 48 && str[index + 2] <= 57) || (str[index +
                3] >= 48 && str[index + 3] <= 57))) {
                int offsethr = (((int)str[index] - 48) * 10) +
                    ((int)str[index + 1] - 48);
                int offsetmin = (((int)str[index + 2] - 48) * 10) +
                    ((int)str[index + 3] - 48);
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
                if (parseObsoleteZones) {
                  if (index + 1 < endIndex && (str[index] & ~32) == 85 &&
                  (str[index + 1] & ~32) == 84) {
                    offset = 0;
                    indexTemp4 += 2;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 71 &&
                    (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32)
                    == 84) {
                    offset = 0;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 69 &&
                    (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -5 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 69 &&
                    (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -4 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 67 &&
                    (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -6 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 67 &&
                    (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -5 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 77 &&
                    (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -7 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 77 &&
                    (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -6 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 80 &&
                    (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -8 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str[index] & ~32) == 80 &&
                    (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32)
                    == 84) {
                    offset = -7 * 60;
                    indexTemp4 += 3;
                  } else if (index < endIndex && ((str[index] >= 65 &&
                 str[index] <= 73) || (str[index] >= 75 && str[index] <= 90) ||
                (str[index] >= 97 && str[index] <= 105) || (str[index]
                    >= 107 && str[index] <= 122))) {
                    offset = 0;
                    ++indexTemp4;
                  }
                }
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
  }
}
