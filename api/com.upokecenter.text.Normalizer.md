# com.upokecenter.text.Normalizer

    @Deprecated public final class Normalizer extends Object

<p>Implements the Unicode normalization algorithm and contains methods and
 functionality to test and convert Unicode strings for Unicode
 normalization.</p> <p>NOTICE: While this class's source code is in the
 public domain, the class uses an class, called NormalizationData,
 that includes data derived from the Unicode Character Database. See the
 documentation for the NormalizerInput class for the permission notice for
 the Unicode Character Database.</p>

## Constructors

## Methods

* `static boolean IsNormalized(String str,
 Normalization form)`<br>
 Deprecated.
Returns whether this string is normalized.

* `static String Normalize(String str,
 Normalization form)`<br>
 Deprecated.
Converts a string to the given Unicode normalization form.

* `int Read(int[] chars,
 int index,
 int length)`<br>
 Deprecated.
Reads a sequence of Unicode code points from a data source.

* `int ReadChar()`<br>
 Deprecated.
Reads a Unicode character from a data source.

## Method Details

### Normalize
    public static String Normalize(String str, Normalization form)
Converts a string to the given Unicode normalization form.

**Parameters:**

* <code>str</code> - An arbitrary string.

* <code>form</code> - The Unicode normalization form to convert to.

**Returns:**

* The parameter <code>str</code> converted to the given normalization form.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### IsNormalized
    public static boolean IsNormalized(String str, Normalization form)
Returns whether this string is normalized.

**Parameters:**

* <code>str</code> - The string to check.

* <code>form</code> - The parameter <code>form</code> is a Normalization object.

**Returns:**

* <code>true</code> if this string is normalized; otherwise, <code>false</code>.
 Returns <code>false</code> if the string contains an unpaired surrogate code
 point.

### ReadChar
    public int ReadChar()
Reads a Unicode character from a data source.

**Returns:**

* Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
 0x10ffff), or the value -1 indicating the end of the source.

### Read
    public int Read(int[] chars, int index, int length)
Reads a sequence of Unicode code points from a data source.

**Parameters:**

* <code>chars</code> - Output buffer.

* <code>index</code> - Index in the output buffer to start writing to.

* <code>length</code> - Maximum number of code points to write.

**Returns:**

* The number of Unicode code points read, or 0 if the end of the
 source is reached.

**Throws:**

* <code>IllegalArgumentException</code> - Either <code>index</code> or <code>length</code> is less
 than 0 or greater than <code>chars</code> 's length, or <code>chars</code> 's length
 minus <code>index</code> is less than <code>length</code>.

* <code>NullPointerException</code> - The parameter <code>chars</code> is null.
