# com.upokecenter.text.NormalizingCharacterInput

    @Deprecated public final class NormalizingCharacterInput extends Object implements com.upokecenter.text.ICharacterInput

Deprecated.&nbsp;
<div class='block'>Renamed to NormalizerInput.</div>

## Methods

* `NormalizingCharacterInput(com.upokecenter.text.ICharacterInput input)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(com.upokecenter.text.ICharacterInput stream,
                         Normalization form)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(List<Integer> characterList)`<br>
 Deprecated.
* `NormalizingCharacterInput(List<Integer> characterList,
                         Normalization form)`<br>
 Deprecated.
* `NormalizingCharacterInput(String str)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(String str,
                         int index,
                         int length,
                         Normalization form)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(String str,
                         Normalization form)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `static List<Integer> GetChars(com.upokecenter.text.ICharacterInput input,
        Normalization form)`<br>
 Deprecated.
Instead of this method, create a NormalizerInput on the input and call
ReadChar to get the normalized String's code points.
 Instead of this method, create a NormalizerInput on the input and call
ReadChar to get the normalized String's code points.
* `static List<Integer> GetChars(String str,
        Normalization form)`<br>
 Deprecated.
Instead of this method, create a NormalizerInput on the String and call
ReadChar to get the normalized String's code points.
 Instead of this method, create a NormalizerInput on the String and call
ReadChar to get the normalized String's code points.
* `static boolean IsNormalized(com.upokecenter.text.ICharacterInput chars,
            Normalization form)`<br>
 Deprecated.  Determines whether the text provided by a character input is normalized.
* `static boolean IsNormalized(int[] charArray,
            Normalization form)`<br>
 Deprecated.
Either convert the array to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
 Either convert the array to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
* `static boolean IsNormalized(List<Integer> charList,
            Normalization form)`<br>
 Deprecated.
Either convert the list to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
 Either convert the list to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
* `static boolean IsNormalized(String str,
            Normalization form)`<br>
 Deprecated.  Determines whether the given string is in the given Unicode normalization
 form.
* `static String Normalize(String str,
         Normalization form)`<br>
 Deprecated.  Converts a string to the given Unicode normalization form.
* `int Read(int[] chars,
    int index,
    int length)`<br>
 Deprecated.  Reads a sequence of Unicode code points from a data source.
* `int ReadChar()`<br>
 Deprecated.  Reads a Unicode character from a data source.

## Constructors

* `NormalizingCharacterInput(com.upokecenter.text.ICharacterInput input)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(com.upokecenter.text.ICharacterInput stream,
                         Normalization form)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(List<Integer> characterList)`<br>
 Deprecated.
* `NormalizingCharacterInput(List<Integer> characterList,
                         Normalization form)`<br>
 Deprecated.
* `NormalizingCharacterInput(String str)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(String str,
                         int index,
                         int length,
                         Normalization form)`<br>
 Deprecated.  Initializes a new instance of the
 class.
* `NormalizingCharacterInput(String str,
                         Normalization form)`<br>
 Deprecated.  Initializes a new instance of the
 class.

## Method Details

### NormalizingCharacterInput
    public NormalizingCharacterInput(String str)
Deprecated.&nbsp;

**Parameters:**

* <code>str</code> - A public object.

### NormalizingCharacterInput
    public NormalizingCharacterInput(com.upokecenter.text.ICharacterInput input)
Deprecated.&nbsp;

**Parameters:**

* <code>input</code> - A public object.

### NormalizingCharacterInput
    public NormalizingCharacterInput(List<Integer> characterList)
Deprecated.&nbsp;
### NormalizingCharacterInput
    public NormalizingCharacterInput(List<Integer> characterList, Normalization form)
Deprecated.&nbsp;
### NormalizingCharacterInput
    public NormalizingCharacterInput(String str, int index, int length, Normalization form)
Deprecated.&nbsp;

**Parameters:**

* <code>str</code> - A public object.

* <code>index</code> - A 32-bit signed integer.

* <code>length</code> - Another 32-bit signed integer.

* <code>form</code> - A Normalization object.

### NormalizingCharacterInput
    public NormalizingCharacterInput(String str, Normalization form)
Deprecated.&nbsp;

**Parameters:**

* <code>str</code> - A text string.

* <code>form</code> - A Normalization object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> or "input" or
 "charArray" is null.

### NormalizingCharacterInput
    public NormalizingCharacterInput(com.upokecenter.text.ICharacterInput stream, Normalization form)
Deprecated.&nbsp;

**Parameters:**

* <code>stream</code> - A public object.

* <code>form</code> - A Normalization object.

### IsNormalized
    public static boolean IsNormalized(com.upokecenter.text.ICharacterInput chars, Normalization form)
Deprecated.&nbsp;

**Parameters:**

* <code>chars</code> - A object that implements a streamable character input.

* <code>form</code> - Specifies the normalization form to check.

**Returns:**

* <code>true</code> if the text is normalized; otherwise, <code>false</code>.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>chars</code> is null.

### Normalize
    public static String Normalize(String str, Normalization form)
Deprecated.&nbsp;

**Parameters:**

* <code>str</code> - An arbitrary string.

* <code>form</code> - The Unicode normalization form to convert to.

**Returns:**

* The parameter <code>str</code> converted to the given normalization form.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>str</code> contains an
 unpaired surrogate code point.

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### IsNormalized
    public static boolean IsNormalized(String str, Normalization form)
Deprecated.&nbsp;

**Parameters:**

* <code>str</code> - An arbitrary string.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* <code>true</code> if the given string is in the given Unicode
 normalization form; otherwise, <code>false</code>. Returns <code>false</code>
 if the string contains an unpaired surrogate code point.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### GetChars
    @Deprecated public static List<Integer> GetChars(String str, Normalization form)
Deprecated.&nbsp;Instead of this method, create a NormalizerInput on the String and call
ReadChar to get the normalized String's code points.

**Parameters:**

* <code>str</code> - A text string.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* A list of the normalized Unicode characters.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### GetChars
    @Deprecated public static List<Integer> GetChars(com.upokecenter.text.ICharacterInput input, Normalization form)
Deprecated.&nbsp;Instead of this method, create a NormalizerInput on the input and call
ReadChar to get the normalized String's code points.

**Parameters:**

* <code>str</code> - An object that implements a stream of Unicode characters.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* A list of the normalized Unicode characters.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### IsNormalized
    @Deprecated public static boolean IsNormalized(int[] charArray, Normalization form)
Deprecated.&nbsp;Either convert the array to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.

**Parameters:**

* <code>charArray</code> - An array of Unicode code points.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* <code>true</code> if the given list of characters is in the given Unicode
 normalization form; otherwise, <code>false</code>.

**Throws:**

* <code>NullPointerException</code> - The parameter "charList" is null.

### IsNormalized
    @Deprecated public static boolean IsNormalized(List<Integer> charList, Normalization form)
Deprecated.&nbsp;Either convert the list to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
### ReadChar
    public int ReadChar()
Deprecated.&nbsp;

**Specified by:**

* <code>ReadChar</code>&nbsp;in interface&nbsp;<code>com.upokecenter.text.ICharacterInput</code>

**Returns:**

* Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
 0x10ffff), or the value -1 indicating the end of the source.

### Read
    public int Read(int[] chars, int index, int length)
Deprecated.&nbsp;

**Specified by:**

* <code>Read</code>&nbsp;in interface&nbsp;<code>com.upokecenter.text.ICharacterInput</code>

**Parameters:**

* <code>chars</code> - Output buffer.

* <code>index</code> - A zero-based index showing where the desired portion of <code>chars</code> begins.

* <code>length</code> - The number of elements in the desired portion of <code>chars</code>
 (but not more than <code>chars</code> 's length).

**Returns:**

* The number of Unicode code points read, or 0 if the end of the
 source is reached.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>chars</code> is null.

* <code>IllegalArgumentException</code> - Either <code>index</code> or <code>length</code> is
 less than 0 or greater than <code>chars</code> 's length, or <code>chars</code>
 ' s length minus <code>index</code> is less than <code>length</code>.
