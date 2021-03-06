# com.upokecenter.text.NormalizerInput

    public final class NormalizerInput extends java.lang.Object implements com.upokecenter.text.ICharacterInput

<p>A character input class that implements the Unicode normalization
 algorithm and contains methods and functionality to test and convert
 text strings for normalization. This is similar to the deprecated
 Normalizer class, except it implements the ICharacterInput
 interface.</p> <p>The Unicode Standard includes characters, such as an
 acute accent, that can be combined with other characters to make new
 characters. For example, the letter E combines with an acute accent to
 make E-acute (É). In some cases, the combined form (E-acute) should be
 treated as equivalent to the uncombined form (E plus acute). For this
 reason, the standard defines four <i>normalization forms</i> that
 convert strings to a single equivalent form:</p> <ul> <li><b>NFD</b>
 (Normalization Form D) decomposes combined forms to their constituent
 characters (E plus acute, for example), then reorders combining marks
 to a standardized order. This is called canonical decomposition.</li>
 <li><b>NFC</b> does canonical decomposition, then combines certain
 constituent characters to their composites (E-acute, for example).
 This is called canonical composition.</li> <li>Two normalization
 forms, <b>NFKC</b> and <b>NFKD</b>, are similar to NFC and NFD, except
  they also "decompose" certain characters, such as ligatures, font or
 positional variants, and subscripts, whose visual distinction can
 matter in some contexts. This is called compatibility
 decomposition.</li></ul> <p>For more information, see Standard Annex
 15 at <code>http://www.unicode.org/reports/tr15/</code>.</p> <p><b>Thread
 safety:</b> This class is mutable; its properties can be changed. None
 of its instance methods are designed to be thread safe. Therefore,
 access to objects from this class must be synchronized if multiple
 threads can access them at the same time.</p> <p>NOTICE: While this
 class's source code is in the public domain, the class uses an
 class, called NormalizationData, that includes data derived
 from the Unicode Character Database. In case doing so is required, the
 permission notice for the Unicode Character Database is given
 here:</p> <p>COPYRIGHT AND PERMISSION NOTICE.</p> <p>Copyright (c)
 1991-2014 Unicode, Inc. All rights reserved. Distributed under the
 Terms of Use in http://www.unicode.org/copyright.html.</p>
 <p>Permission is hereby granted, free of charge, to any person
 obtaining a copy of the Unicode data files and any associated
  documentation (the "Data Files") or Unicode software and any
  associated documentation (the "Software") to deal in the Data Files or
 Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, and/or sell copies
 of the Data Files or Software, and to permit persons to whom the Data
 Files or Software are furnished to do so, provided that (a) this
 copyright and permission notice appear with all copies of the Data
 Files or Software, (b) this copyright and permission notice appear in
 associated documentation, and (c) there is clear notice in each
 modified Data File or in the Software as well as in the documentation
 associated with the Data File(s) or Software that the data or software
  has been modified.</p> <p>THE DATA FILES AND SOFTWARE ARE PROVIDED "AS
  IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
 NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS. IN NO
 EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE
 LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL
 DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR
 PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
 TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
 PERFORMANCE OF THE DATA FILES OR SOFTWARE.</p> <p>Except as contained
 in this notice, the name of a copyright holder shall not be used in
 advertising or otherwise to promote the sale, use or other dealings in
 these Data Files or Software without prior written authorization of
 the copyright holder.</p>

## Methods

* `NormalizerInput​(com.upokecenter.text.ICharacterInput input) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class using Normalization Form
 C.
* `NormalizerInput​(com.upokecenter.text.ICharacterInput stream,
               Normalization form) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class.
* `NormalizerInput​(java.lang.String str) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class using Normalization Form
 C.
* `NormalizerInput​(java.lang.String str,
               int index,
               int length,
               Normalization form) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class.
* `NormalizerInput​(java.lang.String str,
               Normalization form) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class.
* `static boolean IsNormalized​(com.upokecenter.text.ICharacterInput chars,
            Normalization form)`<br>
 Determines whether the text provided by a character input is normalized.
* `static boolean IsNormalized​(java.lang.String str,
            Normalization form)`<br>
 Determines whether the given string is in the given Unicode normalization
 form.
* `static java.lang.String Normalize​(java.lang.String str,
         Normalization form)`<br>
 Converts a string to the given Unicode normalization form.
* `int Read​(int[] chars,
    int index,
    int length)`<br>
 Reads a sequence of Unicode code points from a data source.
* `int ReadChar()`<br>
 Reads a Unicode character from a data source.

## Constructors

* `NormalizerInput​(com.upokecenter.text.ICharacterInput input) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class using Normalization Form
 C.
* `NormalizerInput​(com.upokecenter.text.ICharacterInput stream,
               Normalization form) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class.
* `NormalizerInput​(java.lang.String str) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class using Normalization Form
 C.
* `NormalizerInput​(java.lang.String str,
               int index,
               int length,
               Normalization form) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class.
* `NormalizerInput​(java.lang.String str,
               Normalization form) NormalizerInput`<br>
 Initializes a new instance of the NormalizerInput class.

## Method Details

### NormalizerInput
    public NormalizerInput​(java.lang.String str)
Initializes a new instance of the <code>NormalizerInput</code> class using Normalization Form
 C.

**Parameters:**

* <code>str</code> - A string specifying the text to normalize.

### NormalizerInput
    public NormalizerInput​(com.upokecenter.text.ICharacterInput input)
Initializes a new instance of the <code>NormalizerInput</code> class using Normalization Form
 C.

**Parameters:**

* <code>input</code> - The parameter <code>input</code> is an ICharacterInput object.

### NormalizerInput
    public NormalizerInput​(java.lang.String str, int index, int length, Normalization form)
Initializes a new instance of the <code>NormalizerInput</code> class. Uses a portion of a
 string as the input.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

* <code>index</code> - An index starting at 0 showing where the desired portion of
 <code>str</code> begins.

* <code>length</code> - The number of elements in the desired portion of <code>str</code>
 (but not more than <code>str</code> 's length).

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Throws:**

* <code>java.lang.IllegalArgumentException</code> - Either <code>index</code> or <code>length</code> is less
 than 0 or greater than <code>str</code> 's length, or <code>str</code> 's
 length minus <code>index</code> is less than <code>length</code>.

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

### NormalizerInput
    public NormalizerInput​(java.lang.String str, Normalization form)
Initializes a new instance of the <code>NormalizerInput</code> class.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

### NormalizerInput
    public NormalizerInput​(com.upokecenter.text.ICharacterInput stream, Normalization form)
Initializes a new instance of the <code>NormalizerInput</code> class.

**Parameters:**

* <code>stream</code> - The parameter <code>stream</code> is an ICharacterInput object.

* <code>form</code> - Specifies the normalization form to use when normalizing the
 text.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>stream</code> is null.

### IsNormalized
    public static boolean IsNormalized​(com.upokecenter.text.ICharacterInput chars, Normalization form)
Determines whether the text provided by a character input is normalized.

**Parameters:**

* <code>chars</code> - A object that implements a streamable character input.

* <code>form</code> - Specifies the normalization form to check.

**Returns:**

* <code>true</code> if the text is normalized; otherwise, <code>false</code>.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>chars</code> is null.

### Normalize
    public static java.lang.String Normalize​(java.lang.String str, Normalization form)
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

### IsNormalized
    public static boolean IsNormalized​(java.lang.String str, Normalization form)
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

### ReadChar
    public int ReadChar()
Reads a Unicode character from a data source.

**Specified by:**

* <code>ReadChar</code> in interface <code>com.upokecenter.text.ICharacterInput</code>

**Returns:**

* Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
 0x10ffff), or the value -1 indicating the end of the source.

### Read
    public int Read​(int[] chars, int index, int length)
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
