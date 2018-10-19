# com.upokecenter.mail.MailDateTime

    public final class MailDateTime extends Object

Contains methods for parsing and generating date-time strings following the
 Internet Message Format (RFC 5322).

## Methods

* `static String GenerateDateString​(int[] dateTime)`<br>
 Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array.
* `static String GenerateDateString​(int[] dateTime,
                  boolean gmt)`<br>
 Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array, optionally using a "GMT" time zone
 indicator.
* `static int[] ParseDateString​(String str)`<br>
 Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322).
* `static int[] ParseDateString​(String str,
               boolean parseObsoleteZones)`<br>
 Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322), with an option to allow obsolete
 time zone strings to appear in the date-time string.

## Method Details

### GenerateDateString
    public static String GenerateDateString​(int[] dateTime)
Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array.

**Parameters:**

* <code>dateTime</code> - The date and time in the form of an 8-element array. See
 <see cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
 for information on the format of this parameter.

**Returns:**

* A date-time string.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>dateTime</code> is null or
 invalid.

### GenerateDateString
    public static String GenerateDateString​(int[] dateTime, boolean gmt)
Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array, optionally using a "GMT" time zone
 indicator.

**Parameters:**

* <code>dateTime</code> - The date and time in the form of an 8-element array. See
 <see cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
 for information on the format of this parameter.

* <code>gmt</code> - If true, uses the string "GMT" as the time zone offset.

**Returns:**

* A date-time string.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>dateTime</code> is null or
 invalid.

* <code>UnsupportedOperationException</code> - The time zone offset is other than 0
 and <code>gmt</code> is true.

### ParseDateString
    public static int[] ParseDateString​(String str, boolean parseObsoleteZones)
Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322), with an option to allow obsolete
 time zone strings to appear in the date-time string. If an array is
 returned, the elements of that array (starting from 0) are as
 follows: <ul> <li>0 - The year. For example, the value 2000 means
 2000 C.E.</li> <li>1 - Month of the year, from 1 (January) through 12
 (December).</li> <li>2 - Day of the month, from 1 through 31.</li>
 <li>3 - Hour of the day, from 0 through 23.</li> <li>4 - Minute of
 the hour, from 0 through 59.</li> <li>5 - Second of the minute, from
 0 through 60 (this value can go up to 60 to accommodate leap
 seconds). (Leap seconds are additional seconds added to adjust
 international atomic time, or TAI, to an approximation of
 astronomical time known as coordinated universal time, or UTC.)</li>
 <li>6 - Milliseconds of the second, from 0 through 999. Will always
 be 0.</li> <li>7 - Number of minutes to subtract from this date and
 time to get global time. This number can be positive or
 negative.</li></ul>

**Parameters:**

* <code>str</code> - A date-time string.

* <code>parseObsoleteZones</code> - If set to <code>true</code>, this method allows
 obsolete time zones (single-letter time zones, "GMT", "UT", and
 certain three-letter combinations) to appear in the date-time string.

**Returns:**

* An 8-element array containing the date and time, or <code>null</code> if
 <code>str</code> is null, empty, or syntactically invalid, or if the
 string's year would overflow the range of a 32-bit signed integer.

### ParseDateString
    public static int[] ParseDateString​(String str)
Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322). However, this method does not
 allow obsolete time zone strings to appear in the date-time string.
 See <see cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
 for information on the format of this method's return value.

**Parameters:**

* <code>str</code> - string.

**Returns:**

* An 8-element array containing the date and time, or <code>null</code> if
 <code>str</code> is null, empty, or syntactically invalid, or if the
 string's year would overflow the range of a 32-bit signed integer.
