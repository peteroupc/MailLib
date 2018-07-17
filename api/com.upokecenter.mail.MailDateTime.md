# com.upokecenter.mail.MailDateTime

    public final class MailDateTime extends Object

Not documented yet.

## Methods

* `static String GenerateDateString​(int[] dateTime)`<br>
 Generates a date-time string following the Internet Message Format (RFC
 5322) from an 8-element array.
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

* <code>IllegalArgumentException</code> - The parameter "dateTime" is null or
 invalid.

### ParseDateString
    public static int[] ParseDateString​(String str, boolean parseObsoleteZones)
Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322), with an option to allow obsolete
 time zone strings to appear in the date-time string.

**Parameters:**

* <code>str</code> - String.

* <code>parseObsoleteZones</code> - If set to <code>true</code>, this method allows
 obsolete time zones (single-letter time zones, "GMT", "UT", and
 certain three-letter combinations) to appear in the date-time string.

**Returns:**

* An 8-element array containing the date and time, or <code>null</code> if
 <code>str</code> is null, empty, or syntactically invalid, or if the
 string's year would overflow the range of a 32-bit signed integer. If
 an array is returned, element of that array (starting from 0) is as
 follows:

### ParseDateString
    public static int[] ParseDateString​(String str)
Gets the date and time extracted from a date-time string following the
 Internet Message Format (RFC 5322). However, this method does not
 allow obsolete time zone strings to appear in the date-time string.
 See <see cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
 for information on the format of this method's return value.

**Parameters:**

* <code>str</code> - String.

**Returns:**

* An 8-element array containing the date and time, or <code>null</code> if
 <code>str</code> is null, empty, or syntactically invalid, or if the
 string's year would overflow the range of a 32-bit signed integer.
