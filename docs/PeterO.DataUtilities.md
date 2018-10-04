## PeterO.DataUtilities

    public static class DataUtilities

Contains methods useful for reading and writing strings. It is designed to have no dependencies other than the basic runtime class library.Many of these methods work with text encoded in UTF-8, an encoding form of the Unicode Standard which uses one byte to encode the most basic characters and two to four bytes to encode other characters. For example, the `GetUtf8` method converts a text string to an array of bytes in UTF-8.

In C# and Java, text strings are represented as sequences of 16-bit values called `char` s. These sequences are well-formed under UTF-16, a 16-bit encoding form f Unicode, except if they contain unpaired surrogate code points. (A urrogate code point is used to encode supplementary characters, those ith code points U+10000 or higher, in UTF-16. A surrogate pair is a igh surrogate [U+D800 to U+DBFF] followed by a low surrogate [U+DC00 to +DFFF]. An unpaired surrogate code point is a surrogate not appearing n a surrogate pair.) Many of the methods in this class allow setting he behavior to follow when unpaired surrogate code points are found in ext strings, such as throwing an error or treating the unpaired urrogate as a replacement character (U+FFFD).


### Member Summary
* <code>[CodePointAt(string, int)](#CodePointAt_string_int)</code> - Gets the Unicode code point at the given index of the string.
* <code>[CodePointAt(string, int, int)](#CodePointAt_string_int_int)</code> - Gets the Unicode code point at the given index of the string.
* <code>[CodePointBefore(string, int)](#CodePointBefore_string_int)</code> - Gets the Unicode code point just before the given index of the string.
* <code>[CodePointBefore(string, int, int)](#CodePointBefore_string_int_int)</code> - Gets the Unicode code point just before the given index of the string.
* <code>[CodePointCompare(string, string)](#CodePointCompare_string_string)</code> - Compares two strings in Unicode code point order.
* <code>[CodePointLength(string)](#CodePointLength_string)</code> - Finds the number of Unicode code points in the given text string.
* <code>[GetUtf8Bytes(string, bool)](#GetUtf8Bytes_string_bool)</code> - Encodes a string in UTF-8 as a byte array.
* <code>[GetUtf8Bytes(string, bool, bool)](#GetUtf8Bytes_string_bool_bool)</code> - Encodes a string in UTF-8 as a byte array.
* <code>[GetUtf8Length(string, bool)](#GetUtf8Length_string_bool)</code> - Calculates the number of bytes needed to encode a string in UTF-8.
* <code>[GetUtf8String(byte[], bool)](#GetUtf8String_byte_bool)</code> - Generates a text string from a UTF-8 byte array.
* <code>[GetUtf8String(byte[], int, int, bool)](#GetUtf8String_byte_int_int_bool)</code> - Generates a text string from a portion of a UTF-8 byte array.
* <code>[ReadUtf8FromBytes(byte[], int, int, System.Text.StringBuilder, bool)](#ReadUtf8FromBytes_byte_int_int_System_Text_StringBuilder_bool)</code> - Reads a string in UTF-8 encoding from a byte array.
* <code>[ReadUtf8ToString(System.IO.Stream)](#ReadUtf8ToString_System_IO_Stream)</code> - Reads a string in UTF-8 encoding from a data stream in full and returns that string.
* <code>[ReadUtf8ToString(System.IO.Stream, int, bool)](#ReadUtf8ToString_System_IO_Stream_int_bool)</code> - Reads a string in UTF-8 encoding from a data stream and returns that string.
* <code>[ReadUtf8(System.IO.Stream, int, System.Text.StringBuilder, bool)](#ReadUtf8_System_IO_Stream_int_System_Text_StringBuilder_bool)</code> - Reads a string in UTF-8 encoding from a data stream.
* <code>[ToLowerCaseAscii(string)](#ToLowerCaseAscii_string)</code> - Returns a string with the basic upper-case letters A to Z (U+0041 to U+005A) converted to lower-case.
* <code>[ToUpperCaseAscii(string)](#ToUpperCaseAscii_string)</code> - Returns a string with the basic lower-case letters A to Z (U+0061 to U+007A) converted to upper-case.
* <code>[WriteUtf8(string, System.IO.Stream, bool)](#WriteUtf8_string_System_IO_Stream_bool)</code> - Writes a string in UTF-8 encoding to a data stream.
* <code>[WriteUtf8(string, int, int, System.IO.Stream, bool)](#WriteUtf8_string_int_int_System_IO_Stream_bool)</code> - Writes a portion of a string in UTF-8 encoding to a data stream.
* <code>[WriteUtf8(string, int, int, System.IO.Stream, bool, bool)](#WriteUtf8_string_int_int_System_IO_Stream_bool_bool)</code> - Writes a portion of a string in UTF-8 encoding to a data stream.

<a id="CodePointAt_string_int"></a>

### CodePointLength

    public static int CodePointLength(
        string str);

Finds the number of Unicode code points in the given text string. Unpaired surrogate code points increase this number by 1. This is not necessarily the length of the string in "char" s.

<b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

<b>Return Value:</b>

The number of Unicode code points in the given string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.


<a id="GetUtf8Bytes_string_bool"></a>

### ToUpperCaseAscii

    public static string ToUpperCaseAscii(
        string str);

Returns a string with the basic lower-case letters A to Z (U+0061 to U+007A) converted to upper-case. Other characters remain unchanged.

<b>Parameters:</b>

 * <i>str</i>: The parameter <i>str</i>
is a text string.



<b>Return Value:</b>

The converted string, or null if <i>str</i>
is null.

<a id="WriteUtf8_string_int_int_System_IO_Stream_bool"></a>
### WriteUtf8

    public static int WriteUtf8(
        string str,
        int offset,
        int length,
        System.IO.Stream stream,
        bool replace);

Writes a portion of a string in UTF-8 encoding to a data stream.

<b>Parameters:</b>

 * <i>str</i>: A string to write.

 * <i>offset</i>: The zero-based index where the string portion to write begins.

 * <i>length</i>: The length of the string portion to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U+FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Return Value:</b>

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and <i>replace</i>
is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null or <i>stream</i>
is null.

 * System.ArgumentException:
The parameter <i>offset</i>
is less than 0, <i>length</i>
is less than 0, or <i>offset</i>
plus <i>length</i>
is greater than the string's length.

 * System.IO.IOException:
An I/O error occurred.

<a id="WriteUtf8_string_int_int_System_IO_Stream_bool_bool"></a>
### WriteUtf8

    public static int WriteUtf8(
        string str,
        int offset,
        int length,
        System.IO.Stream stream,
        bool replace,
        bool lenientLineBreaks);

Writes a portion of a string in UTF-8 encoding to a data stream.

<b>Parameters:</b>

 * <i>str</i>: A string to write.

 * <i>offset</i>: The zero-based index where the string portion to write begins.

 * <i>length</i>: The length of the string portion to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U+FFFD). If false, stops processing when an unpaired surrogate code point is seen.

 * <i>lenientLineBreaks</i>: If true, replaces carriage return (CR) not followed by line feed (LF) and LF not preceded by CR with CR-LF pairs.

<b>Return Value:</b>

0 if the entire string portion was written; or -1 if the string portion contains an unpaired surrogate code point and <i>replace</i>
is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null or <i>stream</i>
is null.

 * System.ArgumentException:
The parameter <i>offset</i>
is less than 0, <i>length</i>
is less than 0, or <i>offset</i>
plus <i>length</i>
is greater than the string's length.

 * System.IO.IOException:
An I/O error occurred.

<a id="WriteUtf8_string_System_IO_Stream_bool"></a>
### WriteUtf8

    public static int WriteUtf8(
        string str,
        System.IO.Stream stream,
        bool replace);

Writes a string in UTF-8 encoding to a data stream.

<b>Parameters:</b>

 * <i>str</i>: A string to write.

 * <i>stream</i>: A writable data stream.

 * <i>replace</i>: If true, replaces unpaired surrogate code points with the replacement character (U+FFFD). If false, stops processing when an unpaired surrogate code point is seen.

<b>Return Value:</b>

0 if the entire string was written; or -1 if the string contains an unpaired surrogate code point and <i>replace</i>
is false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null or <i>stream</i>
is null.

 * System.IO.IOException:
An I/O error occurred.
