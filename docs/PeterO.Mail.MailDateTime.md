## PeterO.Mail.MailDateTime

    public static class MailDateTime

Contains methods for parsing and generating date-time strings following the Internet Message Format (RFC 5322).

### GenerateDateString

    public static string GenerateDateString(
        int[] dateTime);

Generates a date-time string following the nternet Message Format (RFC 5322) from an 8-element array.

<b>Parameters:</b>

 * <i>dateTime</i>: The date and time in the form of an 8-element array. See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this parameter.

<b>Return Value:</b>

A date-time string.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter "dateTime" is null or invalid.

### GenerateDateString

    public static string GenerateDateString(
        int[] dateTime,
        bool gmt);

Not documented yet.

<b>Parameters:</b>

 * <i>dateTime</i>: Not documented yet.

 * <i>gmt</i>: Not documented yet.

<b>Return Value:</b>

A string object.

### ParseDateString

    public static int[] ParseDateString(
        string str);

Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322). However, this method does not allow obsolete time zone strings to appear in the date-time string. See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this method's return value.

<b>Parameters:</b>

 * <i>str</i>: String.

<b>Return Value:</b>

An 8-element array containing the date and time, or  `null`  if <i>str</i>
 is null, empty, or syntactically invalid, or if the string's year would overflow the range of a 32-bit signed integer.

### ParseDateString

    public static int[] ParseDateString(
        string str,
        bool parseObsoleteZones);

Gets the date and time extracted from a date-time string following the nternet Message Format (RFC 5322), with an option to allow obsolete time one strings to appear in the date-time string.

<b>Parameters:</b>

 * <i>str</i>: String.

 * <i>parseObsoleteZones</i>: If set to  `true` , this method allows obsolete time zones (single-letter time zones, "GMT", "UT", and certain three-letter combinations) to appear in the date-time string.

<b>Return Value:</b>

An 8-element array containing the date and time, or  `null`  if <i>str</i>
 is null, empty, or syntactically invalid, or if the string's year would overflow the range of a 32-bit signed integer. If an array is returned, element of that array (starting from 0) is as follows:

 * 0 - The year. For example, the value 2000 means 2000 C.E.

 * 1 - Month of the year, from 1 (January) through 12 (December).

 * 2 - Day of the month, from 1 through 31.

 * 3 - Hour of the day, from 0 through 23.

 * 4 - Minute of the hour, from 0 through 59.

 * 5 - Second of the minute, from 0 through 60 (this value can go up to 60 to accommodate leap seconds). (Leap seconds are additional seconds added to adjust international atomic time, or TAI, to an approximation of astronomical time known as coordinated universal time, or UTC.)

 * 6 - Milliseconds of the second, from 0 through 999. Will always be 0.

 * 7 - Number of minutes to subtract from this date and time to get global time. This number can be positive or negative.
