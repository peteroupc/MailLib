/*
Written in 2015 by Peter Occil.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
*/
package com.upokecenter.util;

import java.text.DateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.Locale;
import java.util.TimeZone;

final class DateTimeUtilities {
  private DateTimeUtilities(){}

  private static int[] totdays =
      {0,31,59,90,120,151,181,212,243,273,304,334};
  private static int[] numdays =
      {31,28,31,30,31,30,31,31,30,31,30,31};

  public static int GetDayOfWeek(int[] dateTime){
// Based on public-domain code which was
      // written by Paul Edwards in 1993
      int yr = dateTime[0];
      int mo = dateTime[1];
      int da = dateTime[2];
      int addon = 0;  /* number of days that have advanced */
      boolean leap;     /* is this year a leap year? */
      yr %= 400;
      if (yr < 0) yr += 400;
      /* is the current year a leap year? */
      if (((((yr % 4) == 0) && ((yr % 100) != 0)) || ((yr % 400) == 0))) {
        leap = true;
      } else leap = false;
      if ((mo < 1) || (mo > 12)) return 0;  /* validate the month */
      if (da < 1) return 0;                 /* and day of month */
      if (leap && (mo == 2)) {
        if (da > (numdays[mo - 1] + 1)) return 0;
      } else if (da > numdays[mo - 1]) return 0;
      addon += yr;            /* The day advances by one day every year */
      addon += yr / 4;          /* An additional day if it is divisible bay 4 */
      addon -= yr / 100;        /* Unless it is divisible by 100 */
      /* However, we should not count that
         extra day if the current year is a leap
         year and we haven't gone past 29th February */
      if (leap && (mo <= 2)) addon--;
      addon += totdays[mo - 1]; /* The day of the week increases by
                             the number of days in all the months
                             up till now */
      addon += da;      /* the day of week advances for each day */
                        /* Now as we all know, 2000-01-01 is a Saturday.  Using this
                        as our reference point, and the knowledge that we want to
                        return [1..7] for Sunday..Saturday [changed from 0..6 --PO],
                        we find out that we need to compensate by adding 6. */
      addon += 6;
      return (addon % 7) + 1;  /* the remainder after dividing by 7
                        gives the day of week */
  }
  public static int[] GetCurrentUniversalTime(){
    Calendar c=Calendar.getInstance(TimeZone.getTimeZone("GMT"),
        Locale.US);
    c.setTimeInMillis(new Date().getTime());
    return new int[]{
        c.get(Calendar.YEAR),
        c.get(Calendar.MONTH)+1,
        c.get(Calendar.DAY_OF_MONTH),
        c.get(Calendar.HOUR_OF_DAY),
        c.get(Calendar.MINUTE),
        // In some Java implementations, maybe, the second
        // might go beyond 59 due to leap seconds.
        Math.min(59,c.get(Calendar.SECOND)),
        c.get(Calendar.MILLISECOND),
        0 // Time zone offset always 0 for GMT/UTC
    };
  }
  public static int[] GetCurrentLocalTime(){
    Calendar c=Calendar.getInstance();
    c.setTimeInMillis(date);
    return new int[]{
        c.get(Calendar.YEAR),
        c.get(Calendar.MONTH)+1,
        c.get(Calendar.DAY_OF_MONTH),
        c.get(Calendar.HOUR_OF_DAY),
        c.get(Calendar.MINUTE),
        // In some Java implementations, maybe, the second
        // might go beyond 59 due to leap seconds.
        Math.min(59,c.get(Calendar.SECOND)),
        c.get(Calendar.MILLISECOND),
        // Don't use ZONE_OFFSET, since it apparently
        // doesn't change after setTimeInMillis changes
        c.getTimeZone().getOffset(date)/60000
    };
  }
}
