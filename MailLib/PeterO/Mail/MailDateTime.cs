using System;
using System.Text;
using PeterO;

namespace PeterO.Mail {
    /// <summary>Contains methods for parsing and generating date-time
    /// strings following the Internet Message Format (RFC 5322).</summary>
  public static class MailDateTime {
    private static string[] valueDaysOfWeek = {
      "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
    };

    private static string[] valueMonths = {
      String.Empty, "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul",
      "Aug", "Sep", "Oct", "Nov", "Dec",
    };

    /// <summary>Generates a date-time string following the Internet
    /// Message Format (RFC 5322) from an 8-element array.</summary>
    /// <param name='dateTime'>The date and time in the form of an
    /// 8-element array. See <c>ParseDateString(bool)</c> for information
    /// on the format of this parameter.</param>
    /// <returns>A date-time string.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='dateTime'/> is null or invalid (see
    /// <c>ParseDateString(bool)</c> ).</exception>
    public static string GenerateDateString(int[] dateTime) {
      return GenerateDateString(dateTime, false);
    }

    private static int[] totdays = {
      0, 31, 59, 90, 120, 151, 181, 212, 243,
      273, 304, 334,
    };

    private static int[] numdays = {
      31, 28, 31, 30, 31, 30, 31, 31, 30, 31,
      30, 31,
    };

    internal static bool IsValidDateTime(int[] dateTime) {
      if (dateTime == null || dateTime.Length < 8) {
        return false;
      }
      if (dateTime[1] < 1 || dateTime[1] > 12 || dateTime[2] < 1) {
        return false;
      }
      bool leap = IsLeapYear(dateTime[0]);
      if (dateTime[0] < 1900) {
         // NOTE: RFC 5322 allows for only years 1900 or greater.
         return false;
      }
      if (dateTime[1] == 4 || dateTime[1] == 6 || dateTime[1] == 9 ||
        dateTime[1] == 11) {
        if (dateTime[2] > 30) {
          return false;
        }
      } else if (dateTime[1] == 2) {
        if (dateTime[2] > (leap ? 29 : 28)) {
          return false;
        }
      } else {
        if (dateTime[2] > 31) {
          return false;
        }
      }
      return !(dateTime[3] < 0 || dateTime[4] < 0 || dateTime[5] < 0 ||
dateTime[3] >= 24 || dateTime[4] >= 60 || dateTime[5] >= 61 ||
dateTime[6] < 0 || dateTime[7] <= -1440 ||
        dateTime[7] >= 1440);
    }

    private static bool IsLeapYear(int yr) {
      yr %= 400;
      if (yr < 0) {
        yr += 400;
      }
      return (((yr % 4) == 0) && ((yr % 100) != 0)) || ((yr % 400) == 0);
    }

    internal static void AddMinutes(int[] dateTime, int minutes) {
      if (minutes < -1439) {
        throw new ArgumentException("minutes (" + minutes +
          ") is not greater or equal to " + (-1439));
      }
      if (minutes > 1439) {
        throw new ArgumentException("minutes (" + minutes +
          ") is not less or equal to 1439");
      }
      int homi = (dateTime[3] * 60) + dateTime[4];
      homi += minutes;
      if (homi >= 0 && homi < 1440) {
        dateTime[3] = (homi / 60) % 24;
        dateTime[4] = homi % 60;
      } else if (homi < 0) {
        homi += 1440;
        dateTime[3] = (homi / 60) % 24;
        dateTime[4] = homi % 60;
        DecrementDay(dateTime);
      } else {
        homi -= 1440;
        dateTime[3] = (homi / 60) % 24;
        dateTime[4] = homi % 60;
        IncrementDay(dateTime);
      }
    }

    internal static void DecrementDay(int[] dateTime) {
      int da = dateTime[2] - 1;
      if (da >= 1) {
        dateTime[2] = da;
        return;
      }
      int days = numdays[dateTime[1] - 1];
      if (dateTime[1] == 2) {
        days = IsLeapYear(dateTime[0]) ? 29 : 28;
      }
      dateTime[2] = days;
      dateTime[1] = dateTime[1] - 1;
      if (dateTime[1] <= 0) {
        dateTime[0] = dateTime[0] - 1;
        dateTime[1] = 12;
      }
    }

    internal static void IncrementDay(int[] dateTime) {
      int da = dateTime[2] + 1;
      if (da <= 28) {
        dateTime[2] = da;
        return;
      }
      int days = numdays[dateTime[1] - 1];
      if (dateTime[1] == 2) {
        days = IsLeapYear(dateTime[0]) ? 29 : 28;
      }
      if (da <= days) {
        dateTime[2] = da;
        return;
      }
      dateTime[2] = 1;
      dateTime[1] = dateTime[1] + 1;
      if (dateTime[1] >= 13) {
        dateTime[0] = dateTime[0] + 1;
        dateTime[1] = 1;
      }
    }

    internal static int GetDayOfWeek(int[] dateTime) {
      // Based on public-domain code which was
      // written by Paul Edwards in 1993
      int yr = dateTime[0];
      int mo = dateTime[1];
      int da = dateTime[2];
      var addon = 0; // number of days that have advanced
      bool leap; // is this year a leap year?
      yr %= 400;
      if (yr < 0) {
        yr += 400;
      }
      // is the current year a leap year?
      if ((((yr % 4) == 0) && ((yr % 100) != 0)) || ((yr % 400) == 0)) {
        leap = true;
      } else {
        leap = false;
      }
      if ((mo < 1) || (mo > 12)) {
        return -1; // validate the month
      }
      if (da < 1) {
        return -1; // and day of month
      }
      if (leap && (mo == 2)) {
        if (da > (numdays[mo - 1] + 1)) {
          return -1;
        }
      } else if (da > numdays[mo - 1]) {
        return -1;
      }
      addon += yr; // The day advances by one day every year
      addon += yr / 4; // An additional day if it is divisible bay 4
      addon -= yr / 100; // Unless it is divisible by 100
      /* However, we should not count that
         extra day if the current year is a leap
         year and we haven't gone past 29th February */
      if (leap && (mo <= 2)) {
        --addon;
      }
      addon += totdays[mo - 1]; /* The day of the week increases by
                the number of days in all the months up till now */
      addon += da; // the day of week advances for each day
                   /* Now as we all know, 2000-01-01 is a Saturday. Using this
                   as our reference point, and the knowledge that we want to
                   return 0..6 for Sunday..Saturday,
                   we find out that we need to compensate by adding 6. */
      addon += 6;
      return addon % 7; /* the remainder after dividing by 7
                    gives the day of week */
    }

    /// <summary>Generates a date-time string following the Internet
    /// Message Format (RFC 5322) from an 8-element array.</summary>
    /// <param name='dateTime'>The date and time in the form of an
    /// 8-element array. See <c>ParseDateString(bool)</c> for information
    /// on the format of this parameter.</param>
    /// <param name='gmt'>If true, uses the string "GMT" as the time zone
    /// offset.</param>
    /// <returns>A date-time string.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='dateTime'/> is null or invalid (see
    /// <c>ParseDateString(bool)</c> ).</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dateTime'/> is null.</exception>
    public static string GenerateDateString(int[] dateTime, bool gmt) {
      if (dateTime == null) {
        throw new ArgumentNullException(nameof(dateTime));
      }
      if (!IsValidDateTime(dateTime) || dateTime[0] < 0) {
        throw new ArgumentException("Invalid date and time");
      }
      if (gmt && dateTime[7] != 0) {
        // Use time offset to convert local time to UTC/GMT
        var newDateTime = new int[8];
        Array.Copy(dateTime, 0, newDateTime, 0, 7);
        AddMinutes(newDateTime, -dateTime[7]);
        dateTime = newDateTime;
      }
      int dow = GetDayOfWeek(dateTime);
      if (dow < 0) {
        throw new ArgumentException("Invalid date and time");
      }
      if (dateTime[0] < 0) {
        throw new ArgumentException("Invalid year");
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

    /// <summary>Gets the date and time extracted from a date-time string
    /// following the Internet Message Format (RFC 5322), with an option to
    /// allow obsolete time zone strings to appear in the date-time string.
    /// If an array is returned, the elements of that array (starting from
    /// 0) are as follows:
    /// <list>
    /// <item>0 - The year. For example, the value 2000 means 2000 C.E.
    /// This value cannot be less than 1900 (a restriction specified by RFC
    /// 5322).</item>
    /// <item>1 - Month of the year, from 1 (January) through 12
    /// (December).</item>
    /// <item>2 - Day of the month, from 1 through 31.</item>
    /// <item>3 - Hour of the day, from 0 through 23.</item>
    /// <item>4 - Minute of the hour, from 0 through 59.</item>
    /// <item>5 - Second of the minute, from 0 through 60 (this value can
    /// go up to 60 to accommodate leap seconds). (Leap seconds are
    /// additional seconds added to adjust international atomic time, or
    /// TAI, to an approximation of astronomical time known as coordinated
    /// universal time, or UTC.)</item>
    /// <item>6 - Fractional seconds. The return value will always have
    /// this value set to 0, since fractional seconds cannot be expressed
    /// in the date-time format specified by RFC 5322. This value cannot be
    /// less than 0.</item>
    /// <item>7 - Number of minutes to subtract from this date and time to
    /// get global time. This number can be positive or negative, but
    /// cannot be less than -1439 or greater than 1439.</item></list>
    /// <para>If a method or property uses an array of this format and
    /// refers to this method's documentation, that array may have any
    /// number of elements 8 or greater.</para></summary>
    /// <param name='str'>A date-time string.</param>
    /// <param name='parseObsoleteZones'>If set to <c>true</c>, this
    /// method allows obsolete time zones (single-letter time zones, "GMT",
    /// "UT", and certain three-letter combinations) to appear in the
    /// date-time string.</param>
    /// <returns>An 8-element array containing the date and time, or
    /// <c>null</c> if <paramref name='str'/> is null, empty, or
    /// syntactically invalid, or if the string's year would overflow the
    /// range of a 32-bit signed integer.</returns>
    public static int[] ParseDateString(string str, bool parseObsoleteZones) {
    if (String.IsNullOrEmpty(str)) {
      return null;
    }
    var ret = new int[8];
    if (ParseHeaderExpandedDate(
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

    /// <summary>Gets the date and time extracted from a date-time string
    /// following the Internet Message Format (RFC 5322). Obsolete time
    /// zone strings are not allowed to appear in the date-time string. See
    /// <c>ParseDateString(bool)</c> for information on this method's
    /// return value.</summary>
    /// <param name='str'>A date-time string.</param>
    /// <returns>An 8-element array containing the date and time, or
    /// <c>null</c> if <paramref name='str'/> is null, empty, or
    /// syntactically invalid, or if the string's year would overflow the
    /// range of a 32-bit signed integer.</returns>
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
// DebugUtility.Log("zone " + (str.Substring(index)));
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
                  indexTemp3 += 3;
                  break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 84 &&
                  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) ==
                  69) {
                  dayOfWeek = 2;
                  indexTemp3 += 3;
                  break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 87 &&
                  (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) ==
                  68) {
                  dayOfWeek = 3;
                  indexTemp3 += 3;
                  break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 84 &&
                  (str[index + 1] & ~32) == 72 && (str[index + 2] & ~32) ==
                  85) {
                  dayOfWeek = 4;
                  indexTemp3 += 3;
                  break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 70 &&
                  (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) ==
                  73) {
                  dayOfWeek = 5;
                  indexTemp3 += 3;
                  break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 83 &&
                  (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) ==
                  84) {
                  dayOfWeek = 6;
                  indexTemp3 += 3;
                  break;
                }
                if (index + 2 < endIndex && (str[index] & ~32) == 83 &&
                  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) ==
                  78) {
                  dayOfWeek = 0;
                  indexTemp3 += 3;
                  break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2;
                break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            index = HeaderParser.ParseCFWS(str, index, endIndex, null);
            if (index < endIndex && (str[index] == 44)) {
              ++index;
            } else {
              index = indexStart2;
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
        } while (false);
// DebugUtility.Log("zone " + (str.Substring(index)));
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        day = 0;
        // NOTE: Day can have a leading zero (e.g., 05).
        for (i = 0; i < 2; ++i) {
          if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
            day *= 10;
            day += ((int)str[index]) - 48;
            ++index;
          } else if (i < 1) {
            index = indexStart;
            break;
          } else {
            break;
          }
        }
        if (day == 0) {
          // Day parsed was 0
          return indexStart;
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
            index = indexStart;
            break;
          }
        } while (false);
// DebugUtility.Log("zone " + (str.Substring(index)));
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
          index = indexStart;
          break;
        }
// DebugUtility.Log("zone " + (str.Substring(index)));
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
// DebugUtility.Log("zone " + (str.Substring(index)));
        if (yearDigits == 3 || (yearDigits == 2 && year >= 50)) {
          year += 1900;
        } else if (yearDigits == 2) {
          year += 2000;
        }
        if (year < 1900) {
          // Year is less than 1900
          return indexStart;
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
// DebugUtility.Log("zone " + (str.Substring(index)));
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
          index = indexStart;
          break;
        }
// DebugUtility.Log("zone " + (str.Substring(index)));
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && (str[index] == 58)) {
          ++index;
        } else {
          index = indexStart;
          break;
        }
// DebugUtility.Log("zone " + (str.Substring(index)));
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
          index = indexStart;
          break;
        }
        second = 0;
// DebugUtility.Log("zone " + (str.Substring(index)));
        do {
          indexTemp2 = index;
          do {
            indexStart2 = index;
            index = HeaderParser.ParseCFWS(str, index, endIndex, null);
            if (index < endIndex && (str[index] == 58)) {
              ++index;
            } else {
              index = indexStart2;
              break;
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
              index = indexStart2;
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
        } while (false);
// DebugUtility.Log("zone " + (str.Substring(index)));
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
              var minus = false;
              if (index < endIndex && ((str[index] == 43) || (str[index] ==
                    45))) {
                minus = str[index] == 45;
                ++index;
              } else {
                index = indexStart3;
                break;
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
                  index = indexStart3;
                  break;
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
              indexTemp2 = indexTemp3;
              break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart;
            break;
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
          int dow = GetDayOfWeek(ret);
          if (dow != dayOfWeek) {
            return indexStart;
          }
        }
      }
      return indexTemp;
    }

    /// <summary>Parses a date string in one of the three formats allowed
    /// by HTTP/1.1 (RFC 7231).</summary>
    /// <param name='v'>A date-time string.</param>
    /// <returns>An array of 8 elements as specified in the
    /// <c>ParseDateString(bool)</c> method.</returns>
    public static int[] ParseDateStringHttp(string v) {
      if (v == null) {
        return null;
      }
      var index = 0;
      int endIndex = v.Length;
      if (endIndex - index > 28 && ((v[index] >= 33 && v[index] <= 126) &&
        (v[index + 1] >= 33 && v[index + 1] <= 126) && (v[index + 2] >= 33 &&
        v[index + 2] <= 126)) && (endIndex - index > 4 && v[index + 3] ==
        44 && v[index + 4] == 32) && ((v[index + 5] >= 48 && v[index + 5] <=
        57) && (v[index + 6] >= 48 && v[index + 6] <= 57)) && (v[index + 7]
        == 32) && ((v[index + 8] >= 33 && v[index + 8] <= 126) && (v[index +
        9] >= 33 && v[index + 9] <= 126) && (v[index + 10] >= 33 && v[index +
        10] <= 126)) && (v[index + 11] == 32) && ((v[index + 12] >= 48 &&
        v[index + 12] <= 57) && (v[index + 13] >= 48 && v[index + 13] <= 57) &&
        (v[index + 14] >= 48 && v[index + 14] <= 57) && (v[index + 15] >=
        48 && v[index + 15] <= 57)) && (v[index + 16] == 32) && ((v[index +
        17] >= 48 && v[index + 17] <= 57) && (v[index + 18] >= 48 && v[index +
        18] <= 57)) && (v[index + 19] == 58) && ((v[index + 20] >= 48 &&
        v[index + 20] <= 57) && (v[index + 21] >= 48 && v[index + 21] <=
        57)) && (v[index + 22] == 58) && ((v[index + 23] >= 48 && v[index +
        23] <= 57) && (v[index + 24] >= 48 && v[index + 24] <= 57)) &&
        (v[index + 25] == 32) && (v[index + 26] == 71) && (v[index + 27] ==
        77) && (v[index + 28] == 84)) {
        if (index + 29 != endIndex) {
          return null;
        }
        int dow = ParseDOW(v, index, endIndex);
        int day = ((v[index + 5] - '0') * 10) + (v[index + 6] - '0');
        int month = ParseMonth(v, index + 8, endIndex);
        if (dow < 0 || month < 0) {
          return null;
        }
        int year = ((v[index + 12] - '0') * 1000) +
              ((v[index + 13] - '0') * 100) + ((v[index + 14] - '0') * 10) +
              (v[index + 15] - '0');
        if (year < 1900) {
          return null;
        }
        int hour = ((v[index + 17] - '0') * 10) + (v[index + 18] - '0');
        int minute = ((v[index + 20] - '0') * 10) + (v[index + 21] - '0');
        int second = ((v[index + 23] - '0') * 10) + (v[index + 24] - '0');
        int[] ret = { year, month, day, hour, minute, second, 0, 0 };
        return (dow == GetDayOfWeek(ret)) ? ret : null;
      }
      // ASCTIME
      if (endIndex - index > 23 && ((v[index] >= 33 && v[index] <= 126) &&
        (v[index + 1] >= 33 && v[index + 1] <= 126) && (v[index + 2] >= 33 &&
        v[index + 2] <= 126)) && (v[index + 3] == 32) && ((v[index + 4]
        >= 33 && v[index + 4] <= 126) && (v[index + 5] >= 33 && v[index + 5]
        <= 126) && (v[index + 6] >= 33 && v[index + 6] <= 126)) && (v[index +
        7] == 32) && ((v[index + 8] >= 48 && v[index + 8] <= 57) ||
        (v[index + 8] == 32)) && (v[index + 9] >= 48 && v[index + 9] <= 57) &&
        (v[index + 10] == 32) && ((v[index + 11] >= 48 && v[index + 11]
        <= 57) && (v[index + 12] >= 48 && v[index + 12] <= 57)) && (v[index +
        13] == 58) && ((v[index + 14] >= 48 && v[index + 14] <= 57) &&
        (v[index + 15] >= 48 && v[index + 15] <= 57)) && (v[index + 16] ==
        58) && ((v[index + 17] >= 48 && v[index + 17] <= 57) && (v[index +
        18] >= 48 && v[index + 18] <= 57)) && (v[index + 19] == 32) &&
        ((v[index + 20] >= 48 && v[index + 20] <= 57) && (v[index + 21] >=
        48 && v[index + 21] <= 57) && (v[index + 22] >= 48 && v[index + 22]
        <= 57) && (v[index + 23] >= 48 && v[index + 23] <= 57))) {
        if (index + 24 != endIndex) {
          return null;
        }
        int dow = ParseDOW(v, index, endIndex);
        int month = ParseMonth(v, index + 4, endIndex);
        if (dow < 0 || month < 0) {
          return null;
        }
        int day = (v[index + 8] == 32 ? 0 : (v[index + 8] - '0') * 10) +
          (v[index + 9] - '0');
        int year = ((v[index + 20] - '0') * 1000) +
    ((v[index + 21] - '0') * 100) + ((v[index + 22] - '0') * 10) +
    (v[index + 23] - '0');
        if (year < 1900) {
          return null;
        }
        int hour = ((v[index + 11] - '0') * 10) + (v[index + 12] - '0');
        int minute = ((v[index + 14] - '0') * 10) + (v[index + 15] - '0');
        int second = ((v[index + 17] - '0') * 10) + (v[index + 18] - '0');
        int[] ret = { year, month, day, hour, minute, second, 0, 0 };
        return (dow == GetDayOfWeek(ret)) ? ret : null;
      }
      // RFC 850
      int dowLong = ParseDOWLong(v, index, endIndex);
      if (dowLong < 0) {
        return null;
      }
      string dowNameLong = dowNamesLong[dowLong];
      index += dowNameLong.Length;
      if (endIndex - index > 23 && (endIndex - index > 1 && v[index] == 44 &&
        v[index + 1] == 32) && ((v[index + 2] >= 48 && v[index + 2] <=
        57) && (v[index + 3] >= 48 && v[index + 3] <= 57)) && (v[index + 4]
        == 45) && ((v[index + 5] >= 33 && v[index + 5] <= 126) && (v[index +
        6] >= 33 && v[index + 6] <= 126) && (v[index + 7] >= 33 && v[index +
        7] <= 126)) && (v[index + 8] == 45) && ((v[index + 9] >= 48 &&
        v[index + 9] <= 57) && (v[index + 10] >= 48 && v[index + 10] <= 57)) &&
        (v[index + 11] == 32) && ((v[index + 12] >= 48 && v[index + 12]
        <= 57) && (v[index + 13] >= 48 && v[index + 13] <= 57)) && (v[index +
        14] == 58) && ((v[index + 15] >= 48 && v[index + 15] <= 57) &&
        (v[index + 16] >= 48 && v[index + 16] <= 57)) && (v[index + 17] ==
        58) && ((v[index + 18] >= 48 && v[index + 18] <= 57) && (v[index +
        19] >= 48 && v[index + 19] <= 57)) && (v[index + 20] == 32) &&
        (v[index + 21] == 71) && (v[index + 22] == 77) && (v[index + 23] ==
        84)) {
        int idx = index + 2;
        index += 24;
        if (index != endIndex) {
          return null;
        }
        int month = ParseMonth(v, idx + 3, endIndex);
        if (dowLong < 0 || month < 0) {
          return null;
        }
        int day = ((v[idx] - '0') * 10) + (v[idx + 1] - '0');
        int year = ((v[idx + 7] - '0') * 10) + (v[idx + 8] - '0');
        int hour = ((v[idx + 10] - '0') * 10) + (v[idx + 11] - '0');
        int minute = ((v[idx + 13] - '0') * 10) + (v[idx + 14] - '0');
        int second = ((v[idx + 16] - '0') * 10) + (v[idx + 17] - '0');
        int thisyear = DateTime.UtcNow.Year;
        int this2digityear = thisyear % 100;
        int convertedYear = year + (thisyear - this2digityear);
        if (year - this2digityear > 50) {
          convertedYear -= 100;
        }
        if (convertedYear < 1900) {
          return null;
        }
        int[] ret = { convertedYear, month, day, hour, minute, second, 0, 0 };
        return (dowLong == GetDayOfWeek(ret)) ? ret : null;
      }
      return null;
    }

    private static string[] monthNames = {
      "Jan", "Feb", "Mar", "Apr", "May", "Jun",
      "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
    };

    private static string[] dowNames = {
      "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
    };

    private static string[] dowNamesLong = {
      "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday",
      "Saturday",
    };

    private static int ParseMonth(string v, int index, int endIndex) {
      if (endIndex - index <= 2) {
        return -1;
      }
      for (var i = 0; i < 12; ++i) {
        string monthName = monthNames[i];
        if (v[index] == monthName[0] &&
           v[index + 1] == monthName[1] && v[index + 2] == monthName[2]) {
          return i + 1;
        }
      }
      return -1;
    }

    private static int ParseDOW(string v, int index, int endIndex) {
      if (endIndex - index <= 2) {
        return -1;
      }
      for (var i = 0; i < 7; ++i) {
        string dowName = dowNames[i];
        if (v[index] == dowName[0] &&
            v[index + 1] == dowName[1] && v[index + 2] == dowName[2]) {
          return i;
        }
      }
      return -1;
    }

    private static int ParseDOWLong(string v, int index, int endIndex) {
      if (endIndex - index <= 2) {
        return -1;
      }
      for (var i = 0; i < 7; ++i) {
        string dowName = dowNamesLong[i];
        if (endIndex - index >= dowName.Length &&
           v.Substring(index, dowName.Length).Equals(dowName,
  StringComparison.Ordinal)) {
          return i;
        }
      }
      return -1;
    }
  }
}
