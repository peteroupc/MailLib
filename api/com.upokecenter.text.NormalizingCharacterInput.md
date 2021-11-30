# com.upokecenter.text.NormalizingCharacterInput

## Constructors

* `NormalizingCharacterInput​(com.upokecenter.text.ICharacterInput input) NormalizingCharacterInput`<br>
 Deprecated. Initializes a new instance of the NormalizingCharacterInput class.
* `NormalizingCharacterInput​(com.upokecenter.text.ICharacterInput stream,
Normalization form) NormalizingCharacterInput`<br>
 Deprecated. Initializes a new instance of the NormalizingCharacterInput class.
* `NormalizingCharacterInput​(java.lang.String str) NormalizingCharacterInput`<br>
 Deprecated. Initializes a new instance of the NormalizingCharacterInput class.
* `NormalizingCharacterInput​(java.lang.String str,
int index,
int length,
Normalization form) NormalizingCharacterInput`<br>
 Deprecated. Initializes a new instance of the NormalizingCharacterInput class.
* `NormalizingCharacterInput​(java.lang.String str,
Normalization form) NormalizingCharacterInput`<br>
 Deprecated. Initializes a new instance of the NormalizingCharacterInput class.
* `NormalizingCharacterInput​(java.util.List<java.lang.Integer> characterList) NormalizingCharacterInput`<br>
 Deprecated. Initializes a new instance of the NormalizingCharacterInput class.
* `NormalizingCharacterInput​(java.util.List<java.lang.Integer> characterList,
Normalization form) NormalizingCharacterInput`<br>
 Deprecated. Initializes a new instance of the NormalizingCharacterInput class.

## Methods

* `static java.util.List<java.lang.Integer> GetChars​(com.upokecenter.text.ICharacterInput chars,
Normalization form)`<br>
 Deprecated.
Instead of this method, create a NormalizerInput on the input and call
 ReadChar to get the normalized String's code points.
 Instead of this method, create a NormalizerInput on the input and call
 ReadChar to get the normalized String's code points.
* `static java.util.List<java.lang.Integer> GetChars​(java.lang.String str,
Normalization form)`<br>
 Deprecated.
Instead of this method, create a NormalizerInput on the String and call
 ReadChar to get the normalized String's code points.
 Instead of this method, create a NormalizerInput on the String and call
 ReadChar to get the normalized String's code points.
* `static boolean IsNormalized​(int[] charArray,
Normalization form)`<br>
 Deprecated.
Either convert the array to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
 Either convert the array to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
* `static boolean IsNormalized​(com.upokecenter.text.ICharacterInput chars,
Normalization form)`<br>
 Deprecated. Determines whether the text provided by a character input is normalized.
* `static boolean IsNormalized​(java.lang.String str,
Normalization form)`<br>
 Deprecated. Determines whether the given string is in the given Unicode normalization
 form.
* `static boolean IsNormalized​(java.util.List<java.lang.Integer> charList,
Normalization form)`<br>
 Deprecated.
Either convert the list to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
 Either convert the list to a String or wrap it in an ICharacterInput and
 call the corresponding overload instead.
* `static java.lang.String Normalize​(java.lang.String str,
Normalization form)`<br>
 Deprecated. Converts a string to the given Unicode normalization form.
* `int Read​(int[] chars,
int index,
int length)`<br>
 Deprecated. Reads a sequence of Unicode code points from a data source.
* `int ReadChar()`<br>
 Deprecated. Reads a Unicode character from a data source.

## Method Details

### <a id='IsNormalized(com.upokecenter.text.ICharacterInput,com.upokecenter.text.Normalization)'>IsNormalized</a>

Determines whether the text provided by a character input is normalized.

**Parameters:**

* <code>chars</code> - A object that implements a streamable character input.

* <code>form</code> - Specifies the normalization form to check.

**Returns:**

* <code>true</code> if the text is normalized; otherwise, <code>false</code>.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>chars</code> is null.

### <a id='Normalize(java.lang.String,com.upokecenter.text.Normalization)'>Normalize</a>

Converts a string to the given Unicode normalization form.

**Parameters:**

* <code>str</code> - An arbitrary string.

* <code>form</code> - The Unicode normalization form to convert to.

**Returns:**

* The parameter <code>str</code> converted to the given normalization form.

**Throws:**

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>str</code> contains an unpaired
 surrogate code point.

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

### <a id='IsNormalized(java.lang.String,com.upokecenter.text.Normalization)'>IsNormalized</a>

Determines whether the given string is in the given Unicode normalization
 form.

**Parameters:**

* <code>str</code> - An arbitrary string.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* <code>true</code> if the given string is in the given Unicode
 normalization form; otherwise, <code>false</code>. Returns <code>false</code>
 if the string contains an unpaired surrogate code point.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

### <a id='GetChars(java.lang.String,com.upokecenter.text.Normalization)'>GetChars</a>

Gets a list of normalized code points after reading from a string.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* A list of the normalized Unicode characters.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

### <a id='GetChars(com.upokecenter.text.ICharacterInput,com.upokecenter.text.Normalization)'>GetChars</a>

Gets a list of normalized code points after reading from a character stream.

**Parameters:**

* <code>chars</code> - An object that implements a stream of Unicode characters.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* A list of the normalized Unicode characters.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>chars</code> is null.

### <a id='IsNormalized(int[],com.upokecenter.text.Normalization)'>IsNormalized</a>

Determines whether the given array of characters is in the given Unicode
 normalization form.

**Parameters:**

* <code>charArray</code> - An array of Unicode code points.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* <code>true</code> if the given list of characters is in the given Unicode
 normalization form; otherwise, <code>false</code>.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter "charList" is null.

### <a id='IsNormalized(java.util.List,com.upokecenter.text.Normalization)'>IsNormalized</a>

Determines whether the given list of characters is in the given Unicode
 normalization form.

**Parameters:**

* <code>charList</code> - A list of Unicode code points.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Returns:**

* <code>true</code> if the given list of characters is in the given Unicode
 normalization form; otherwise, <code>false</code>.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>charList</code> is null.

### <a id='ReadChar()'>ReadChar</a>

Reads a Unicode character from a data source.

**Specified by:**

* <code>ReadChar</code> in interface <code>com.upokecenter.text.ICharacterInput</code>

**Returns:**

* Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
 0x10ffff), or the value -1 indicating the end of the source.

### <a id='Read(int[],int,int)'>Read</a>

Reads a sequence of Unicode code points from a data source.

**Specified by:**

* <code>Read</code> in interface <code>com.upokecenter.text.ICharacterInput</code>

**Parameters:**

* <code>chars</code> - Output buffer.

* <code>index</code> - An index starting at 0 showing where the desired portion of
 <code>chars</code> begins.

* <code>length</code> - The number of elements in the desired portion of <code>chars</code>
 (but not more than <code>chars</code> 's length).

**Returns:**

* The number of Unicode code points read, or 0 if the end of the
 source is reached.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>chars</code> is null.

* <code>java.lang.IllegalArgumentException</code> - Either <code>index</code> or <code>length</code> is less
 than 0 or greater than <code>chars</code> 's length, or <code>chars</code> 's
 length minus <code>index</code> is less than <code>length</code>.
