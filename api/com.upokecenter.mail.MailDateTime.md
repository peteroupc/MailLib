# com.upokecenter.mail.MailDateTime

    public final class MailDateTime extends java.lang.Object

Contains methods for parsing and generating date-time strings following the
 Internet Message Format (RFC 5322).

## Methods

* `static java.lang.String GenerateDateString​(int[] dateTime)`<br>
 Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array.
* `static java.lang.String GenerateDateString​(int[] dateTime,
                  boolean gmt)`<br>
 Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array.
* `static int[] ParseDateString​(java.lang.String str)`<br>
 Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322).
* `static int[] ParseDateString​(java.lang.String str,
               boolean parseObsoleteZones)`<br>
 Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322), with an option to allow obsolete
 time zone strings to appear in the date-time string.
* `static int[] ParseDateStringHttp​(java.lang.String v)`<br>
 Parses a date string in one of the three formats allowed by HTTP/1.1 (RFC
 7231).

## Method Details

### GenerateDateString
    public static java.lang.String GenerateDateString​(int[] dateTime)
Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array.

**Parameters:**

* <code>dateTime</code> - The date and time in the form of an 8-element array. See
 <code>ParseDateString(boolean)</code> for information on the format of
 this parameter.

**Returns:**

* A date-time string.

**Throws:**

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>dateTime</code> is null or invalid
 (see <code>ParseDateString(boolean)</code>).

### GenerateDateString
    public static java.lang.String GenerateDateString​(int[] dateTime, boolean gmt)
Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array.

**Parameters:**

* <code>dateTime</code> - The date and time in the form of an 8-element array. See
 <code>ParseDateString(boolean)</code> for information on the format of
 this parameter.

* <code>gmt</code> - If true, uses the string "GMT" as the time zone offset.

**Returns:**

* A date-time string.

**Throws:**

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>dateTime</code> is null or invalid
 (see <code>ParseDateString(boolean)</code>).

* <code>java.lang.NullPointerException</code> - The parameter <code>dateTime</code> is null.

### ParseDateString
    public static int[] ParseDateString​(java.lang.String str, boolean parseObsoleteZones)
Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322), with an option to allow obsolete
 time zone strings to appear in the date-time string. If an array is
 returned, the elements of that array (starting from 0) are as
 follows: <ul> <li>0 - The year. For example, the value 2000 means
 2000 C.E. This value cannot be less than 1900 (a restriction
 specified by RFC 5322).</li> <li>1 - Month of the year, from 1
 (January) through 12 (December).</li> <li>2 - Day of the month, from
 1 through 31.</li> <li>3 - Hour of the day, from 0 through 23.</li>
 <li>4 - Minute of the hour, from 0 through 59.</li> <li>5 - Second
 of the minute, from 0 through 59, except this value can be 60 if the
 hour of the day is 23 and the minute of the hour is 59, to
 accommodate leap seconds. (Leap seconds are additional seconds added
 to adjust international atomic time, or TAI, to an approximation of
 astronomical time known as coordinated universal time, or UTC.)</li>
 <li>6 - Fractional seconds. The return value will always have this
 value set to 0, since fractional seconds cannot be expressed in the
 date-time format specified by RFC 5322. This value cannot be less
 than 0.</li> <li>7 - Number of minutes to subtract from this date
 and time to get global time. This number can be positive or
 negative, but cannot be less than -1439 or greater than
 1439.</li></ul> <p>If a method or property uses an array of this
 format and refers to this method's documentation, that array may
 have any number of elements 8 or greater.</p>

**Parameters:**

* <code>str</code> - A date-time string.

* <code>parseObsoleteZones</code> - If set to <code>true</code>, this method allows
  obsolete time zones (single-letter time zones, "GMT", "UT", and
 certain three-letter combinations) to appear in the date-time
 string.

**Returns:**

* An 8-element array containing the date and time, or <code>null</code> if
 <code>str</code> is null, empty, or syntactically invalid, or if the
 string's year would overflow the range of a 32-bit signed integer.

### ParseDateString
    public static int[] ParseDateString​(java.lang.String str)
Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322). Obsolete time zone strings are
 not allowed to appear in the date-time string. See
 <code>ParseDateString(boolean)</code> for information on this method's
 return value.

**Parameters:**

* <code>str</code> - A date-time string.

**Returns:**

* An 8-element array containing the date and time, or <code>null</code> if
 <code>str</code> is null, empty, or syntactically invalid, or if the
 string's year would overflow the range of a 32-bit signed integer.

### ParseDateStringHttp
    public static int[] ParseDateStringHttp​(java.lang.String v)
Parses a date string in one of the three formats allowed by HTTP/1.1 (RFC
 7231).

**Parameters:**

* <code>v</code> - A date-time string.

**Returns:**

* An array of 8 elements as specified in the <code>
 ParseDateString(boolean)</code> method.
