## PeterO.Mail.MailDateTime

    public static class MailDateTime

Contains methods for parsing and generating date-time strings following the Internet Message Format (RFC 5322).

### Member Summary
* <code>[GenerateDateString(int[])](#GenerateDateString_int)</code> - Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array.
* <code>[GenerateDateString(int[], bool)](#GenerateDateString_int_bool)</code> - Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array.
* <code>[ParseDateString(string)](#ParseDateString_string)</code> - Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322).
* <code>[ParseDateString(string, bool)](#ParseDateString_string_bool)</code> - Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322), with an option to allow obsolete time zone strings to appear in the date-time string.
* <code>[ParseDateStringHttp(string)](#ParseDateStringHttp_string)</code> - Parses a date string in one of the three formats allowed by HTTP/1.

<a id="GenerateDateString_int_bool"></a>
### GenerateDateString

    public static string GenerateDateString(
        int[] dateTime,
        bool gmt);

Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array.

<b>Parameters:</b>

 * <i>dateTime</i>: The date and time in the form of an 8-element array. See  `ParseDateString(string, bool)`  for information on the format of this parameter.

 * <i>gmt</i>: If true, uses the string "GMT" as the time zone offset.

<b>Return Value:</b>

A date-time string.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>dateTime</i>
 is null or invalid (see  `ParseDateString(string, bool)`  ).

 * System.ArgumentNullException:
The parameter  <i>dateTime</i>
 is null.

<a id="GenerateDateString_int"></a>
### GenerateDateString

    public static string GenerateDateString(
        int[] dateTime);

Generates a date-time string following the Internet Message Format (RFC 5322) from an 8-element array.

<b>Parameters:</b>

 * <i>dateTime</i>: The date and time in the form of an 8-element array. See  `ParseDateString(string, bool)`  for information on the format of this parameter.

<b>Return Value:</b>

A date-time string.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>dateTime</i>
 is null or invalid (see  `ParseDateString(string, bool)`  ).

<a id="ParseDateString_string_bool"></a>
### ParseDateString

    public static int[] ParseDateString(
        string str,
        bool parseObsoleteZones);

Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322), with an option to allow obsolete time zone strings to appear in the date-time string. If an array is returned, the elements of that array (starting from 0) are as follows:

 * 0 - The year. For example, the value 2000 means 2000 C.E. This value cannot be less than 1900 (a restriction specified by RFC 5322).

 * 1 - Month of the year, from 1 (January) through 12 (December).

 * 2 - Day of the month, from 1 through 31.

 * 3 - Hour of the day, from 0 through 23.

 * 4 - Minute of the hour, from 0 through 59.

 * 5 - Second of the minute, from 0 through 59, except this value can be 60 if the hour of the day is 23 and the minute of the hour is 59, to accommodate leap seconds. (Leap seconds are additional seconds added to adjust international atomic time, or TAI, to an approximation of astronomical time known as coordinated universal time, or UTC.)

 * 6 - Fractional seconds. The return value will always have this value set to 0, since fractional seconds cannot be expressed in the date-time format specified by RFC 5322. This value cannot be less than 0.

 * 7 - Number of minutes to subtract from this date and time to get global time. This number can be positive or negative, but cannot be less than -1439 or greater than 1439.

If a method or property uses an array of this format and refers to this method's documentation, that array may have any number of elements 8 or greater.

<b>Parameters:</b>

 * <i>str</i>: A date-time string.

 * <i>parseObsoleteZones</i>: If set to  `true` , this method allows obsolete time zones (single-letter time zones, "GMT", "UT", and certain three-letter combinations) to appear in the date-time string.

<b>Return Value:</b>

An 8-element array containing the date and time, or  `null`  if  <i>str</i>
 is null, empty, or syntactically invalid, or if the string's year would overflow the range of a 32-bit signed integer.

<a id="ParseDateString_string"></a>
### ParseDateString

    public static int[] ParseDateString(
        string str);

Gets the date and time extracted from a date-time string following the Internet Message Format (RFC 5322). Obsolete time zone strings are not allowed to appear in the date-time string. See  `ParseDateString(string, bool)`  for information on this method's return value.

<b>Parameters:</b>

 * <i>str</i>: A date-time string.

<b>Return Value:</b>

An 8-element array containing the date and time, or  `null`  if  <i>str</i>
 is null, empty, or syntactically invalid, or if the string's year would overflow the range of a 32-bit signed integer.

<a id="ParseDateStringHttp_string"></a>
### ParseDateStringHttp

    public static int[] ParseDateStringHttp(
        string v);

Parses a date string in one of the three formats allowed by HTTP/1.1 (RFC 7231).

<b>Parameters:</b>

 * <i>v</i>: A date-time string.

<b>Return Value:</b>

An array of 8 elements as specified in the  `ParseDateString(string, bool)`  method.
