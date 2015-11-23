## PeterO.Text.NormalizingCharacterInput

    public sealed class NormalizingCharacterInput :
        PeterO.Text.ICharacterInput

A character input class that implements the Unicode normalization algorithm and contains methods and functionality to test and convert text strings for normalization. This is similar to the Normalizer class, except it implements the ICharacterInput interface.

NOTICE: While this class's source code is in the public domain, the class uses an internal class, called NormalizationData, that includes data derived from the Unicode Character Database. In case doing so is required, the permission notice for the Unicode Character Database is given here:

COPYRIGHT AND PERMISSION NOTICE

Copyright (c) 1991-2014 Unicode, Inc. All rights reserved. Distributed under the Terms of Use in [http://www.unicode.org/copyright.html.](http://www.unicode.org/copyright.html.)

Permission is hereby granted, free of charge, to any person obtaining a copy of the Unicode data files and any associated documentation (the "Data Files") or Unicode software and any associated documentation (the "Software") to deal in the Data Files or Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, and/or sell copies of the Data Files or Software, and to permit persons to whom the Data Files or Software are furnished to do so, provided that (a) this copyright and permission notice appear with all copies of the Data Files or Software, (b) this copyright and permission notice appear in associated documentation, and (c) there is clear notice in each modified Data File or in the Software as well as in the documentation associated with the Data File(s) or Software that the data or software has been modified.

THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA FILES OR SOFTWARE.

Except as contained in this notice, the name of a copyright holder shall not be used in advertising or otherwise to promote the sale, use or other dealings in these Data Files or Software without prior written authorization of the copyright holder.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        PeterO.Text.ICharacterInput input);

Initializes a new instance of the NormalizingCharacterInput class using Normalization Form C.

<b>Parameters:</b>

 * <i>input</i>: An ICharacterInput object.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        PeterO.Text.ICharacterInput stream,
        PeterO.Text.Normalization form);

Initializes a new instance of the NormalizingCharacterInput class.

<b>Parameters:</b>

 * <i>stream</i>: An ICharacterInput object.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str);

Initializes a new instance of the NormalizingCharacterInput class using Normalization Form C.

<b>Parameters:</b>

 * <i>str</i>: A string specifying the text to normalize.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str,
        int index,
        int length,
        PeterO.Text.Normalization form);

Initializes a new instance of the NormalizingCharacterInput class. Uses a portion of a string as the input.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>index</i>: A 32-bit signed integer.

 * <i>length</i>: A 32-bit signed integer. (2).

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str,
        PeterO.Text.Normalization form);

Initializes a new instance of the NormalizingCharacterInput class.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        System.Collections.Generic.IList characterList);

Initializes a new instance of the NormalizingCharacterInput class using Normalization Form C.

<b>Parameters:</b>

 * <i>characterList</i>: A list of Unicode code points specifying the text to normalize.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        System.Collections.Generic.IList characterList,
        PeterO.Text.Normalization form);

Initializes a new instance of the NormalizingCharacterInput class using the given normalization form.

<b>Parameters:</b>

 * <i>characterList</i>: An IList object.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>characterList</i>
 is null.

### GetChars

    public static System.Collections.Generic.IList GetChars(
        PeterO.Text.ICharacterInput str,
        PeterO.Text.Normalization form);

Gets a list of normalized code points after reading from a character stream.

<b>Parameters:</b>

 * <i>str</i>: An object that implements a stream of Unicode characters.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Returns:</b>

A list of the normalized Unicode characters.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### GetChars

    public static System.Collections.Generic.IList GetChars(
        string str,
        PeterO.Text.Normalization form);

Gets a list of normalized code points after reading from a string.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Returns:</b>

A list of the normalized Unicode characters.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### IsNormalized

    public static bool IsNormalized(
        PeterO.Text.ICharacterInput chars,
        PeterO.Text.Normalization form);

Determines whether the text provided by a character input is normalized.

<b>Parameters:</b>

 * <i>chars</i>: A object that implements a streamable character input.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Returns:</b>

True if the text is normalized; otherwise, false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

### IsNormalized

    public static bool IsNormalized(
        string str,
        PeterO.Text.Normalization form);

Determines whether the given string is in the given Unicode normalization form.

<b>Parameters:</b>

 * <i>str</i>: An arbitrary string.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Returns:</b>

True if the given string is in the given Unicode normalization form; otherwise, false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### IsNormalized

    public static bool IsNormalized(
        System.Collections.Generic.IList charList,
        PeterO.Text.Normalization form);

Determines whether the given list of characters is in the given Unicode normalization form.

<b>Parameters:</b>

 * <i>charList</i>: A list of Unicode code points.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Returns:</b>

True if the given list of characters is in the given Unicode normalization form; otherwise, false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>charList</i>
 is null.

### Normalize

    public static string Normalize(
        string str,
        PeterO.Text.Normalization form);

Converts a string to the given Unicode normalization form.

<b>Parameters:</b>

 * <i>str</i>: An arbitrary string.

 * <i>form</i>: The Unicode normalization form to convert to.

<b>Returns:</b>

The parameter  <i>str</i>
 converted to the given normalization form.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### Read

    public sealed int Read(
        int[] chars,
        int index,
        int length);

Reads a sequence of Unicode code points from a data source.

<b>Parameters:</b>

 * <i>chars</i>: Output buffer.

 * <i>index</i>: A zero-based index showing where the desired portion of  <i>chars</i>
 begins.

 * <i>length</i>: The number of elements in the desired portion of  <i>chars</i>
 (but not more than  <i>chars</i>
 's length).

<b>Returns:</b>

The number of Unicode code points read, or 0 if the end of the source is reached.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

 * System.ArgumentException:
Either  <i>index</i>
or  <i>length</i>
 is less than 0 or greater than <i>chars</i>
 's length, or  <i>chars</i>
 's length minus  <i>index</i>
 is less than  <i>length</i>
.

### ReadChar

    public sealed int ReadChar();

Reads a Unicode character from a data source.

<b>Returns:</b>

Either a Unicode code point (from 0-0xd7ff or from 0xe000 to 0x10ffff), or the value -1 indicating the end of the source.
