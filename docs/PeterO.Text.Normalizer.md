## PeterO.Text.Normalizer

    public sealed class Normalizer

<b>Obsolete.</b> Use NormalizerInput instead; that class is much more flexible than Normalizer.

Implements the Unicode normalization algorithm and contains methods and functionality to test and convert Unicode strings for Unicode normalization.

NOTICE: While this class's source code is in the public domain, the class uses an internal class, called NormalizationData, that includes data derived from the Unicode Character Database. See the documentation for the NormalizerInput class for the permission notice for the Unicode Character Database.

### Member Summary
* <code>[IsNormalized(string, PeterO.Text.Normalization)](#IsNormalized_string_PeterO_Text_Normalization)</code> - Returns whether this string is normalized.
* <code>[Normalize(string, PeterO.Text.Normalization)](#Normalize_string_PeterO_Text_Normalization)</code> - Converts a string to the specified Unicode normalization form.
* <code>[Read(int[], int, int)](#Read_int_int_int)</code> - Reads a sequence of Unicode code points from a data source.
* <code>[ReadChar()](#ReadChar)</code> - Reads a Unicode character from a data source.

<a id="Void_ctor_System_String_PeterO_Text_Normalization"></a>
### Normalizer Constructor

    public Normalizer(
        string str,
        PeterO.Text.Normalization form);

Initializes a new instance of the [PeterO.Text.Normalizer](PeterO.Text.Normalizer.md) class.

<b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>form</i>: The parameter  <i>form</i>
 is a Normalization object.

<a id="IsNormalized_string_PeterO_Text_Normalization"></a>
### IsNormalized

    public static bool IsNormalized(
        string str,
        PeterO.Text.Normalization form);

Returns whether this string is normalized.

<b>Parameters:</b>

 * <i>str</i>: The string to check.

 * <i>form</i>: The parameter  <i>form</i>
 is a Normalization object.

<b>Return Value:</b>

 `true`  if this string is normalized; otherwise,  `false` . Returns  `false`  if the string contains an unpaired surrogate code point.

<a id="Normalize_string_PeterO_Text_Normalization"></a>
### Normalize

    public static string Normalize(
        string str,
        PeterO.Text.Normalization form);

Converts a string to the specified Unicode normalization form.

<b>Parameters:</b>

 * <i>str</i>: An arbitrary string.

 * <i>form</i>: The Unicode normalization form to convert to.

<b>Return Value:</b>

The parameter  <i>str</i>
 converted to the specified normalization form.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="Read_int_int_int"></a>
### Read

    public int Read(
        int[] chars,
        int index,
        int length);

Reads a sequence of Unicode code points from a data source.

<b>Parameters:</b>

 * <i>chars</i>: Output buffer.

 * <i>index</i>: Index in the output buffer to start writing to.

 * <i>length</i>: Maximum number of code points to write.

<b>Return Value:</b>

The number of Unicode code points read, or 0 if the end of the source is reached.

<b>Exceptions:</b>

 * System.ArgumentException:
Either  <i>index</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>chars</i>
 's length, or  <i>chars</i>
 's length minus  <i>index</i>
 is less than  <i>length</i>
.

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

<a id="ReadChar"></a>
### ReadChar

    public int ReadChar();

Reads a Unicode character from a data source.

<b>Return Value:</b>

Either a Unicode code point (from 0-0xd7ff or from 0xe000 to 0x10ffff), or the value -1 indicating the end of the source.
