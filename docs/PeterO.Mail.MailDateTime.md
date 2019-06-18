## PeterO.Mail.MailDateTime

    public static class MailDateTime

Contains methods for parsing and generating date-time strings following the Internet Message Format (RFC 5322).

### Member Summary
* <code>[GenerateDateString(int[])](#GenerateDateString_int)</code> - Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array.
* <code>[GenerateDateString(int[], bool)](#GenerateDateString_int_bool)</code> - Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array, optionally using a "GMT" time zone indicator.
* <code>[ParseDateString(string)](#ParseDateString_string)</code> - Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322).
* <code>[ParseDateString(string, bool)](#ParseDateString_string_bool)</code> - Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322), with an option to allow obsolete time zone strings to appear in the date-time string.
* <code>[ParseDateStringHttp(string)](#ParseDateStringHttp_string)</code> - Parses a date string in one of the three formats allowed by HTTP/1.

<a id="GenerateDateString_int"></a>
### GenerateDateString

    public static string GenerateDateString(
        int[] dateTime);

Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array.

<b>Parameters:</b>

 * <i>dateTime</i>: The date and time in the form of an 8-element array. See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this parameter.

<b>Return Value:</b>

A date-time string.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>dateTime</i>
is null or invalid, including if the year ( `dateTime[0]`  ) is less than 0.

<a id="GenerateDateString_int_bool"></a>
### GenerateDateString

    public static string GenerateDateString(
        int[] dateTime,
        bool gmt);

Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array, optionally using a "GMT" time zone indicator.

<b>Parameters:</b>

 * <i>dateTime</i>: The date and time in the form of an 8-element array. See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this parameter.

 * <i>gmt</i>: If true, uses the string "GMT" as the time zone offset.

<b>Return Value:</b>

A date-time string.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>dateTime</i>
is null or invalid, including if the year ( `dateTime[0]`  ) is less than 0.

 * System.NotSupportedException:
The time zone offset is other than 0 and <i>gmt</i>
is true.

<a id="ParseDateString_string"></a>
### ParseDateString

    public static int[] ParseDateString(
        string str);

Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322). However, this method does not allow obsolete time zone strings to appear in the date-time string. See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this method's return value.

<b>Parameters:</b>

 * <i>str</i>: The parameter <i>str</i>
is not documented yet.

<b>Return Value:</b>

An 8-element array containing the date and time, or `null`  if <i>str</i>
is null, empty, or syntactically invalid, or if the string's year would verflow the range of a 32-bit signed integer.

<a id="ParseDateString_string_bool"></a>
### ParseDateString

    public static int[] ParseDateString(
        string str,
        bool parseObsoleteZones);

Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322), with an option to allow obsolete time zone strings to appear in the date-time string. If an array is returned, the elements of that array (starting from 0) are as follows:

 * 0 - The year. For example, the value 2000 means 2000 C.E.

 * 1 - Month of the year, from 1 (January) through 12 (December).

 * 2 - Day of the month, from 1 through 31.

 * 3 - Hour of the day, from 0 through 23.

 * 4 - Minute of the hour, from 0 through 59.

 * 5 - Second of the minute, from 0 through 60 (this value can go up to 60 to accommodate leap seconds). (Leap seconds are additional seconds added to adjust international atomic time, or TAI, to an approximation of astronomical time known as coordinated universal time, or UTC.)

 * 6 - Milliseconds of the second, from 0 through 999. Will always be 0.

 * 7 - Number of minutes to subtract from this date and time to get global time. This number can be positive or negative.

<b>Parameters:</b>

 * <i>str</i>: A date-time string.

 * <i>parseObsoleteZones</i>: If set to `true`  , this method allows obsolete time zones (single-letter time zones, "GMT", "UT", and certain three-letter combinations) to appear in the date-time string.

<b>Return Value:</b>

An 8-element array containing the date and time, or `null`  if <i>str</i>
is null, empty, or syntactically invalid, or if the string's year would verflow the range of a 32-bit signed integer.

<a id="ParseDateStringHttp_string"></a>
### ParseDateStringHttp

    public static int[] ParseDateStringHttp(
        string v);

Parses a date string in one of the three formats allowed by HTTP/1.1.

<b>Parameters:</b>

 * <i>v</i>: The parameter <i>v</i>
is not documented yet.

<b>Return Value:</b>

A 64-bit signed integer.
