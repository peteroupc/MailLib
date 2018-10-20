package com.upokecenter.mail;

import com.upokecenter.util.*;

    /**
     * Contains methods for parsing and generating date-time strings following the
     * Internet Message Format (RFC 5322).
     */
  public final class MailDateTime {
private MailDateTime() {
}
    private static String[] valueDaysOfWeek = {
      "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"
    };

    private static String[] valueMonths = {
      "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul",
      "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    /**
     * Generates a date-time string following the Internet Message Format (RFC
     * 5322) from an 8-element array.
     * @param dateTime The date and time in the form of an 8-element array. See
     * <see
  * cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
     * for information on the format of this parameter.
     * @return A date-time string.
     * @throws IllegalArgumentException The parameter {@code dateTime} is null or
     * invalid.
     */
    public static String GenerateDateString(int[] dateTime) {
      return GenerateDateString(dateTime, false);
    }

    private static int[] totdays = {
      0, 31, 59, 90, 120, 151, 181, 212, 243,
      273, 304, 334 };

    private static int[] numdays = {
      31, 28, 31, 30, 31, 30, 31, 31, 30, 31,
      30, 31 };

    static boolean IsValidDateTime(int[] dateTime) {
      if (dateTime == null || dateTime.length < 8) {
        return false;
      }
      if (dateTime[1] < 1 || dateTime[1] > 12 || dateTime[2] < 1) {
        return false;
      }
      boolean leap = IsLeapYear(dateTime[0]);
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
dateTime[6] < 0 ||
dateTime[6] >= 1000 || dateTime[7] <= -1440 ||
        dateTime[7] >= 1440);
    }

    private static boolean IsLeapYear(int yr) {
      yr %= 400;
      if (yr < 0) {
        yr += 400;
      }
      return (((yr % 4) == 0) && ((yr % 100) != 0)) || ((yr % 400) == 0);
    }

    static void AddMinutes(int[] dateTime, int minutes) {
      if (minutes < -1439) {
        throw new IllegalArgumentException("minutes (" + minutes +
          ") is not greater or equal to " + (-1439));
      }
      if (minutes > 1439) {
        throw new IllegalArgumentException("minutes (" + minutes +
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

    static void DecrementDay(int[] dateTime) {
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

    static void IncrementDay(int[] dateTime) {
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

    static int GetDayOfWeek(int[] dateTime) {
      // Based on public-domain code which was
      // written by Paul Edwards in 1993
      int yr = dateTime[0];
      int mo = dateTime[1];
      int da = dateTime[2];
      int addon = 0;  // number of days that have advanced
      boolean leap;  // is this year a leap year?
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
        return -1;  // validate the month
      }
      if (da < 1) {
        return -1;  // and day of month
      }
      if (leap && (mo == 2)) {
        if (da > (numdays[mo - 1] + 1)) {
          return -1;
        }
      } else if (da > numdays[mo - 1]) {
        return -1;
      }
      addon += yr;  // The day advances by one day every year
      addon += yr / 4;  // An additional day if it is divisible bay 4
      addon -= yr / 100;  // Unless it is divisible by 100
      /* However, we should not count that
         extra day if the current year is a leap
         year and we haven't gone past 29th February */
      if (leap && (mo <= 2)) {
        --addon;
      }
      addon += totdays[mo - 1]; /* The day of the week increases by
                the number of days in all the months up till now */
      addon += da;  // the day of week advances for each day
                    /* Now as we all know, 2000-01-01 is a Saturday. Using this
                    as our reference point, and the knowledge that we want to
                    return 0..6 for Sunday..Saturday,
                    we find out that we need to compensate by adding 6. */
      addon += 6;
      return addon % 7; /* the remainder after dividing by 7
                    gives the day of week */
    }

    /**
     * Generates a date-time string following the Internet Message Format (RFC
     * 5322) from an 8-element array, optionally using a "GMT" time zone
     * indicator.
     * @param dateTime The date and time in the form of an 8-element array. See
     * <see
  * cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
     * for information on the format of this parameter.
     * @param gmt If true, uses the string "GMT" as the time zone offset.
     * @return A date-time string.
     * @throws IllegalArgumentException The parameter {@code dateTime} is null or
     * invalid.
     * @throws UnsupportedOperationException The time zone offset is other than 0
     * and {@code gmt} is true.
     */
    public static String GenerateDateString(int[] dateTime, boolean gmt) {
      if (!IsValidDateTime(dateTime) || dateTime[0] < 0) {
        throw new IllegalArgumentException("Invalid date and time");
      }
      if (gmt && dateTime[7] != 0) {
        // Use time offset to convert local time to UTC/GMT
        int[] newDateTime = new int[8];
        System.arraycopy(dateTime, 0, newDateTime, 0, 7);
        AddMinutes(newDateTime, -dateTime[7]);
        dateTime = newDateTime;
      }
      int dow = GetDayOfWeek(dateTime);
      if (dow < 0) {
        throw new IllegalArgumentException("Invalid date and time");
      }
      String dayString = valueDaysOfWeek[dow];
      String monthString = valueMonths[dateTime[1]];
      StringBuilder sb = new StringBuilder();
      sb.append(dayString);
      sb.append(", ");
      sb.append((char)('0' + ((dateTime[2] / 10) % 10)));
      sb.append((char)('0' + (dateTime[2] % 10)));
      sb.append(' ');
      sb.append(monthString);
      sb.append(' ');
      String yearString = ParserUtility.IntToString(dateTime[0]);
      if (yearString.length() < 4) {
        for (int i = 0; i < 4 - yearString.length(); ++i) {
          sb.append('0');
        }
      }
      sb.append(yearString);
      sb.append(' ');
      sb.append((char)('0' + ((dateTime[3] / 10) % 10)));
      sb.append((char)('0' + (dateTime[3] % 10)));
      sb.append(':');
      sb.append((char)('0' + ((dateTime[4] / 10) % 10)));
      sb.append((char)('0' + (dateTime[4] % 10)));
      sb.append(':');
      sb.append((char)('0' + ((dateTime[5] / 10) % 10)));
      sb.append((char)('0' + (dateTime[5] % 10)));
      sb.append(' ');
      if (gmt) {
        sb.append("GMT");
      } else {
        int offset = dateTime[7];
        sb.append((offset < 0) ? '-' : '+');
        offset = Math.abs(offset);
        int hours = (offset / 60) % 24;
        int minutes = offset % 60;
        sb.append((char)('0' + ((hours / 10) % 10)));
        sb.append((char)('0' + (hours % 10)));
        sb.append((char)('0' + ((minutes / 10) % 10)));
        sb.append((char)('0' + (minutes % 10)));
      }
      return sb.toString();
    }

    /**
     * Gets the date and time extracted from a date-time string following the
     * Internet Message Format (RFC 5322), with an option to allow obsolete
     * time zone strings to appear in the date-time string. If an array is
     * returned, the elements of that array (starting from 0) are as
     * follows: <ul> <li>0 - The year. For example, the value 2000 means
     * 2000 C.E.</li> <li>1 - Month of the year, from 1 (January) through 12
     * (December).</li> <li>2 - Day of the month, from 1 through 31.</li>
     * <li>3 - Hour of the day, from 0 through 23.</li> <li>4 - Minute of
     * the hour, from 0 through 59.</li> <li>5 - Second of the minute, from
     * 0 through 60 (this value can go up to 60 to accommodate leap
     * seconds). (Leap seconds are additional seconds added to adjust
     * international atomic time, or TAI, to an approximation of
     * astronomical time known as coordinated universal time, or UTC.)</li>
     * <li>6 - Milliseconds of the second, from 0 through 999. Will always
     * be 0.</li> <li>7 - Number of minutes to subtract from this date and
     * time to get global time. This number can be positive or
     * negative.</li></ul>
     * @param str A date-time string.
     * @param parseObsoleteZones If set to {@code true}, this method allows
     * obsolete time zones (single-letter time zones, "GMT", "UT", and
     * certain three-letter combinations) to appear in the date-time string.
     * @return An 8-element array containing the date and time, or {@code null} if
     * {@code str} is null, empty, or syntactically invalid, or if the
     * string's year would overflow the range of a 32-bit signed integer.
     */
    public static int[] ParseDateString(String str, boolean parseObsoleteZones) {
      if (((str) == null || (str).length() == 0)) {
 return null;
}
      int[] ret = new int[8];
      if (
  ParseHeaderExpandedDate(
  str,
  0,
  str.length(),
  ret,
  parseObsoleteZones) == str.length()) {
        return ret;
      } else {
        return null;
      }
    }

    /**
     * Gets the date and time extracted from a date-time string following the
     * Internet Message Format (RFC 5322). However, this method does not
     * allow obsolete time zone strings to appear in the date-time string.
     * See <see
  * cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
     * for information on the format of this method's return value.
     * @param str String.
     * @return An 8-element array containing the date and time, or {@code null} if
     * {@code str} is null, empty, or syntactically invalid, or if the
     * string's year would overflow the range of a 32-bit signed integer.
     */
    public static int[] ParseDateString(String str) {
      return ParseDateString(str, false);
    }

    static int ParseHeaderExpandedDate(
  String str,
  int index,
  int endIndex,
  int[] ret,
  boolean parseObsoleteZones) {
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
          if (index + 2 < endIndex && (str.charAt(index) & ~32) == 74 &&
            (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 78) {
            month = 1;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 70 &&
                (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) ==
                    66) {
            month = 2;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
                (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) ==
                    82) {
            month = 3;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 65 &&
                (str.charAt(index + 1) & ~32) == 80 && (str.charAt(index + 2) & ~32) ==
                    82) {
            month = 4;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
                (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) ==
                    89) {
            month = 5;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 74 &&
                (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) ==
                    78) {
            month = 6;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 74 &&
                (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) ==
                    76) {
            month = 7;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 65 &&
                (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) ==
                    71) {
            month = 8;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 83 &&
                (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) ==
                    80) {
            month = 9;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 79 &&
                (str.charAt(index + 1) & ~32) == 67 && (str.charAt(index + 2) & ~32) ==
                    84) {
            month = 10;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 78 &&
                (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) ==
                    86) {
            month = 11;
            indexTemp2 += 3;
          } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 68 &&
                (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) ==
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
                if (parseObsoleteZones) {
                  if (index + 1 < endIndex && (str.charAt(index) & ~32) == 85 &&
                  (str.charAt(index + 1) & ~32) == 84) {
                    offset = 0;
                    indexTemp4 += 2;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 71 &&
                    (str.charAt(index + 1) & ~32) == 77 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = 0;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -5 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -4 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -6 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -5 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -7 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -6 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 &&
                    (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -8 * 60;
                    indexTemp4 += 3;
                  } else if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 &&
                    (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32)
                    == 84) {
                    offset = -7 * 60;
                    indexTemp4 += 3;
                  } else if (index < endIndex && ((str.charAt(index) >= 65 &&
                 str.charAt(index) <= 73) || (str.charAt(index) >= 75 && str.charAt(index) <= 90) ||
                (str.charAt(index) >= 97 && str.charAt(index) <= 105) || (str.charAt(index)
                    >= 107 && str.charAt(index) <= 122))) {
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
          int dow = GetDayOfWeek(ret);
          if (dow != dayOfWeek) {
            return indexStart;
          }
        }
      }
      return indexTemp;
    }
  }
