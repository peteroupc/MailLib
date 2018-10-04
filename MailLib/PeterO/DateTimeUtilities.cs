using System;

namespace PeterO {
  internal static class DateTimeUtilities {
    private static int[] totdays = {
      0, 31, 59, 90, 120, 151, 181, 212, 243,
      273, 304, 334 };

    private static int[] numdays = {
      31, 28, 31, 30, 31, 30, 31, 31, 30, 31,
      30, 31 };

    public static bool IsValidDateTime(int[] dateTime) {
      if (dateTime == null || dateTime.Length < 8) {
        return false;
      }
      if (dateTime[1] < 1 || dateTime[1] > 12 || dateTime[2] < 1) {
        return false;
      }
      bool leap = IsLeapYear(dateTime[0]);
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

    private static bool IsLeapYear(int yr) {
      yr %= 400;
      if (yr < 0) {
        yr += 400;
      }
      return (((yr % 4) == 0) && ((yr % 100) != 0)) || ((yr % 400) == 0);
    }

    public static void AddMinutes(int[] dateTime, int minutes) {
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

    public static void DecrementDay(int[] dateTime) {
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

    public static void IncrementDay(int[] dateTime) {
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

    public static int GetDayOfWeek(int[] dateTime) {
      // Based on public-domain code which was
      // written by Paul Edwards in 1993
      int yr = dateTime[0];
      int mo = dateTime[1];
      int da = dateTime[2];
      var addon = 0;  // number of days that have advanced
      bool leap;  // is this year a leap year?
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

    public static int[] GetCurrentLocalTime() {
      var ret = new int[8];
      DateTime dt = DateTime.Now;
      ret[0] = dt.Year;
      ret[1] = dt.Month;
      ret[2] = dt.Day;
      ret[3] = dt.Hour;
      ret[4] = dt.Minute;
      ret[5] = dt.Second;
      ret[6] = dt.Millisecond;
#if NET20
      DateTime dtu = dt.ToUniversalTime();
      TimeSpan ts = dt - dtu;
      var minutes = (int)Math.Round(ts.TotalMinutes);
      ret[7] = minutes;
#else
    ret[7] = (int)Math.Round(TimeZoneInfo.Local.GetUtcOffset(dt).TotalMinutes);
#endif
      return ret;
    }

    public static int[] GetCurrentGlobalTime() {
      var ret = new int[8];
      DateTime dt = DateTime.UtcNow;
      ret[0] = dt.Year;
      ret[1] = dt.Month;
      ret[2] = dt.Day;
      ret[3] = dt.Hour;
      ret[4] = dt.Minute;
      ret[5] = dt.Second;
      ret[6] = dt.Millisecond;
      ret[7] = 0;  // time zone offset
      return ret;
    }
  }
}
