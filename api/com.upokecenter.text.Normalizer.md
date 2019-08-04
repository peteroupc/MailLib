# com.upokecenter.text.Normalizer

    @Deprecated public final class Normalizer extends java.lang.Object

Deprecated.
Use NormalizerInput instead; that class is much more flexible than
 Normalizer.

## Methods

* `Normalizer​(java.lang.String str,
          Normalization form) Normalizer`<br>
 Deprecated. Initializes a new instance of the Normalizer
 class.
* `static boolean IsNormalized​(java.lang.String str,
            Normalization form)`<br>
 Deprecated. Returns whether this string is normalized.
* `static java.lang.String Normalize​(java.lang.String str,
         Normalization form)`<br>
 Deprecated. Converts a string to the given Unicode normalization form.
* `int Read​(int[] chars,
    int index,
    int length)`<br>
 Deprecated. Reads a sequence of Unicode code points from a data source.
* `int ReadChar()`<br>
 Deprecated. Reads a Unicode character from a data source.

## Constructors

* `Normalizer​(java.lang.String str,
          Normalization form) Normalizer`<br>
 Deprecated. Initializes a new instance of the Normalizer
 class.

## Method Details

### Normalizer
    public Normalizer​(java.lang.String str, Normalization form)
Deprecated.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

* <code>form</code> - The parameter <code>form</code> is a Normalization object.

### Normalizer
    public Normalizer​(java.lang.String str, Normalization form)
Deprecated.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

* <code>form</code> - The parameter <code>form</code> is a Normalization object.

### Normalize
    public static java.lang.String Normalize​(java.lang.String str, Normalization form)
Deprecated.

**Parameters:**

* <code>str</code> - An arbitrary string.

* <code>form</code> - The Unicode normalization form to convert to.

**Returns:**

* The parameter <code>str</code> converted to the given normalization form.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

### IsNormalized
    public static boolean IsNormalized​(java.lang.String str, Normalization form)
Deprecated.

**Parameters:**

* <code>str</code> - The string to check.

* <code>form</code> - The parameter <code>form</code> is a Normalization object.

**Returns:**

* <code>true</code> if this string is normalized; otherwise, <code>false</code>.
 Returns <code>false</code> if the string contains an unpaired surrogate
 code point.

### ReadChar
    public int ReadChar()
Deprecated.

**Returns:**

* Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
 0x10ffff), or the value -1 indicating the end of the source.

### Read
    public int Read​(int[] chars, int index, int length)
Deprecated.

**Parameters:**

* <code>chars</code> - Output buffer.

* <code>index</code> - Index in the output buffer to start writing to.

* <code>length</code> - Maximum number of code points to write.

**Returns:**

* The number of Unicode code points read, or 0 if the end of the
 source is reached.

**Throws:**

* <code>java.lang.IllegalArgumentException</code> - Either <code>index</code> or <code>length</code> is less
 than 0 or greater than <code>chars</code> 's length, or <code>chars</code> ' s
 length minus <code>index</code> is less than <code>length</code>.

* <code>java.lang.NullPointerException</code> - The parameter <code>chars</code> is null.

* <code>java.lang.IllegalArgumentException</code> - Either <code>index</code> or <code>length</code> is less
 than 0 or greater than <code>chars</code> 's length, or <code>chars</code> 's
 length minus <code>index</code> is less than <code>length</code>.
