using System;

namespace PeterO {
  internal static class DateTimeUtilities {
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
