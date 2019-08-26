## PeterO.Text.NormalizingCharacterInput

    public sealed class NormalizingCharacterInput :
        PeterO.Text.ICharacterInput

<b>Deprecated.</b> Renamed to NormalizerInput.

A character input class that implements the Unicode normalization algorithm and contains methods and functionality to test and convert text strings for normalization. This is similar to the deprecated Normalizer class, except it implements the ICharacterInput interface.

 * <b>NFD</b> (Normalization Form D) decomposes combined forms to their constituent characters (E plus acute, for example), then reorders combining marks to a standardized order. This is called canonical decomposition.

 * <b>NFC</b> does canonical decomposition, then combines certain constituent characters to their composites (E-acute, for example). This is called canonical composition.

 * Two normalization forms, <b>NFKC</b> and <b>NFKD</b>, are similar to NFC and NFD, except they also "decompose" certain characters, such as ligatures, font or positional variants, and subscripts, whose visual distinction can matter in some contexts. This is called compatibility decomposition.

For more information, see Standard Annex 15 at  `http://www.unicode.org/reports/tr15/` .

<b>Thread safety:</b> This class is mutable; its properties can be changed. None of its instance methods are designed to be thread safe. Therefore, access to objects from this class must be synchronized if multiple threads can access them at the same time.

NOTICE: While this class's source code is in the public domain, the class uses an internal class, called NormalizationData, that includes data derived from the Unicode Character Database. In case doing so is required, the permission notice for the Unicode Character Database is given here:

COPYRIGHT AND PERMISSION NOTICE.

Copyright (c) 1991-2014 Unicode, Inc. All rights reserved. Distributed under the Terms of Use in http://www.unicode.org/copyright.html.

Permission is hereby granted, free of charge, to any person obtaining a copy of the Unicode data files and any associated documentation (the "Data Files") or Unicode software and any associated documentation (the "Software") to deal in the Data Files or Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, and/or sell copies of the Data Files or Software, and to permit persons to whom the Data Files or Software are furnished to do so, provided that (a) this copyright and permission notice appear with all copies of the Data Files or Software, (b) this copyright and permission notice appear in associated documentation, and (c) there is clear notice in each modified Data File or in the Software as well as in the documentation associated with the Data File(s) or Software that the data or software has been modified.

THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA FILES OR SOFTWARE.

Except as contained in this notice, the name of a copyright holder shall not be used in advertising or otherwise to promote the sale, use or other dealings in these Data Files or Software without prior written authorization of the copyright holder.

### Member Summary
* <code>[GetChars(PeterO.Text.ICharacterInput, PeterO.Text.Normalization)](#GetChars_PeterO_Text_ICharacterInput_PeterO_Text_Normalization)</code> - Gets a list of normalized code points after reading from a character stream.
* <code>[GetChars(string, PeterO.Text.Normalization)](#GetChars_string_PeterO_Text_Normalization)</code> - Gets a list of normalized code points after reading from a string.
* <code>[IsNormalized(int[], PeterO.Text.Normalization)](#IsNormalized_int_PeterO_Text_Normalization)</code> - Determines whether the given array of characters is in the given Unicode normalization form.
* <code>[IsNormalized(PeterO.Text.ICharacterInput, PeterO.Text.Normalization)](#IsNormalized_PeterO_Text_ICharacterInput_PeterO_Text_Normalization)</code> - Determines whether the text provided by a character input is normalized.
* <code>[IsNormalized(string, PeterO.Text.Normalization)](#IsNormalized_string_PeterO_Text_Normalization)</code> - Determines whether the given string is in the given Unicode normalization form.
* <code>[IsNormalized(System.Collections.Generic.IList, PeterO.Text.Normalization)](#IsNormalized_System_Collections_Generic_IList_PeterO_Text_Normalization)</code> - Determines whether the given list of characters is in the given Unicode normalization form.
* <code>[Normalize(string, PeterO.Text.Normalization)](#Normalize_string_PeterO_Text_Normalization)</code> - Converts a string to the given Unicode normalization form.
* <code>[Read(int[], int, int)](#Read_int_int_int)</code> - Reads a sequence of Unicode code points from a data source.
* <code>[ReadChar()](#ReadChar)</code> - Reads a Unicode character from a data source.

<a id="Void_ctor_PeterO_Text_ICharacterInput"></a>
### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        PeterO.Text.ICharacterInput input);

Initializes a new instance of the [PeterO.Text.NormalizingCharacterInput](PeterO.Text.NormalizingCharacterInput.md) class.

<b>Parameters:</b>

 * <i>input</i>: The parameter  <i>input</i>
 is an ICharacterInput object.

<a id="Void_ctor_PeterO_Text_ICharacterInput_PeterO_Text_Normalization"></a>
### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        PeterO.Text.ICharacterInput stream,
        PeterO.Text.Normalization form);

Initializes a new instance of the [PeterO.Text.NormalizingCharacterInput](PeterO.Text.NormalizingCharacterInput.md) class.

<b>Parameters:</b>

 * <i>stream</i>: The parameter  <i>stream</i>
 is an ICharacterInput object.

 * <i>form</i>: The parameter  <i>form</i>
 is a Normalization object.

<a id="Void_ctor_System_String"></a>
### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str);

Initializes a new instance of the [PeterO.Text.NormalizingCharacterInput](PeterO.Text.NormalizingCharacterInput.md) class.

<b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

<a id="Void_ctor_System_String_Int32_Int32_PeterO_Text_Normalization"></a>
### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str,
        int index,
        int length,
        PeterO.Text.Normalization form);

Initializes a new instance of the [PeterO.Text.NormalizingCharacterInput](PeterO.Text.NormalizingCharacterInput.md) class.

<b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>index</i>: The parameter  <i>index</i>
 is a 32-bit signed integer.

 * <i>length</i>: The parameter  <i>length</i>
 is a 32-bit signed integer.

 * <i>form</i>: The parameter  <i>form</i>
 is a Normalization object.

<a id="Void_ctor_System_String_PeterO_Text_Normalization"></a>
### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str,
        PeterO.Text.Normalization form);

Initializes a new instance of the [PeterO.Text.NormalizingCharacterInput](PeterO.Text.NormalizingCharacterInput.md) class.

<b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>form</i>: The parameter  <i>form</i>
 is a Normalization object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="Void_ctor_System_Collections_Generic_IList_1_System_Int32"></a>
### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        System.Collections.Generic.IList characterList);

Initializes a new instance of the [PeterO.Text.NormalizingCharacterInput](PeterO.Text.NormalizingCharacterInput.md) class.

<b>Parameters:</b>

 * <i>characterList</i>: The parameter  <i>characterList</i>
 is an IList object.

<a id="Void_ctor_System_Collections_Generic_IList_1_System_Int32_PeterO_Text_Normalization"></a>
### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        System.Collections.Generic.IList characterList,
        PeterO.Text.Normalization form);

Initializes a new instance of the [PeterO.Text.NormalizingCharacterInput](PeterO.Text.NormalizingCharacterInput.md) class.

<b>Parameters:</b>

 * <i>characterList</i>: The parameter  <i>characterList</i>
 is an IList object.

 * <i>form</i>: The parameter  <i>form</i>
 is a Normalization object.

<a id="GetChars_PeterO_Text_ICharacterInput_PeterO_Text_Normalization"></a>
### GetChars

    public static System.Collections.Generic.IList GetChars(
        PeterO.Text.ICharacterInput chars,
        PeterO.Text.Normalization form);

<b>Deprecated.</b> Instead of this method, create a NormalizerInput on the input and call ReadChar to get the normalized string's code points.

Gets a list of normalized code points after reading from a character stream.

<b>Parameters:</b>

 * <i>chars</i>: An object that implements a stream of Unicode characters.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Return Value:</b>

A list of the normalized Unicode characters.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

<a id="GetChars_string_PeterO_Text_Normalization"></a>
### GetChars

    public static System.Collections.Generic.IList GetChars(
        string str,
        PeterO.Text.Normalization form);

<b>Deprecated.</b> Instead of this method, create a NormalizerInput on the string and call ReadChar to get the normalized string's code points.

Gets a list of normalized code points after reading from a string.

<b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Return Value:</b>

A list of the normalized Unicode characters.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="IsNormalized_int_PeterO_Text_Normalization"></a>
### IsNormalized

    public static bool IsNormalized(
        int[] charArray,
        PeterO.Text.Normalization form);

<b>Deprecated.</b> Either convert the array to a string or wrap it in an ICharacterInput and call the corresponding overload instead.

Determines whether the given array of characters is in the given Unicode normalization form.

<b>Parameters:</b>

 * <i>charArray</i>: An array of Unicode code points.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Return Value:</b>

 `true`  if the given list of characters is in the given Unicode normalization form; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "charList" is null.

<a id="IsNormalized_PeterO_Text_ICharacterInput_PeterO_Text_Normalization"></a>
### IsNormalized

    public static bool IsNormalized(
        PeterO.Text.ICharacterInput chars,
        PeterO.Text.Normalization form);

Determines whether the text provided by a character input is normalized.

<b>Parameters:</b>

 * <i>chars</i>: A object that implements a streamable character input.

 * <i>form</i>: Specifies the normalization form to check.

<b>Return Value:</b>

 `true`  if the text is normalized; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

<a id="IsNormalized_string_PeterO_Text_Normalization"></a>
### IsNormalized

    public static bool IsNormalized(
        string str,
        PeterO.Text.Normalization form);

Determines whether the given string is in the given Unicode normalization form.

<b>Parameters:</b>

 * <i>str</i>: An arbitrary string.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Return Value:</b>

 `true`  if the given string is in the given Unicode normalization form; otherwise,  `false` . Returns  `false`  if the string contains an unpaired surrogate code point.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="IsNormalized_System_Collections_Generic_IList_PeterO_Text_Normalization"></a>
### IsNormalized

    public static bool IsNormalized(
        System.Collections.Generic.IList charList,
        PeterO.Text.Normalization form);

<b>Deprecated.</b> Either convert the list to a string or wrap it in an ICharacterInput and call the corresponding overload instead.

Determines whether the given list of characters is in the given Unicode normalization form.

<b>Parameters:</b>

 * <i>charList</i>: A list of Unicode code points.

 * <i>form</i>: Specifies the normalization form to use when normalizing the text.

<b>Return Value:</b>

 `true`  if the given list of characters is in the given Unicode normalization form; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>charList</i>
 is null.

<a id="Normalize_string_PeterO_Text_Normalization"></a>
### Normalize

    public static string Normalize(
        string str,
        PeterO.Text.Normalization form);

Converts a string to the given Unicode normalization form.

<b>Parameters:</b>

 * <i>str</i>: An arbitrary string.

 * <i>form</i>: The Unicode normalization form to convert to.

<b>Return Value:</b>

The parameter  <i>str</i>
 converted to the given normalization form.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>str</i>
 contains an unpaired surrogate code point.

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

<a id="Read_int_int_int"></a>
### Read

    public sealed int Read(
        int[] chars,
        int index,
        int length);

Reads a sequence of Unicode code points from a data source.

<b>Parameters:</b>

 * <i>chars</i>: Output buffer.

 * <i>index</i>: An index starting at 0 showing where the desired portion of  <i>chars</i>
 begins.

 * <i>length</i>: The number of elements in the desired portion of  <i>chars</i>
 (but not more than  <i>chars</i>
 's length).

<b>Return Value:</b>

The number of Unicode code points read, or 0 if the end of the source is reached.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>chars</i>
 is null.

 * System.ArgumentException:
Either  <i>index</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>chars</i>
 's length, or  <i>chars</i>
 ' s length minus  <i>index</i>
 is less than  <i>length</i>
.

<a id="ReadChar"></a>
### ReadChar

    public sealed int ReadChar();

Reads a Unicode character from a data source.

<b>Return Value:</b>

Either a Unicode code point (from 0-0xd7ff or from 0xe000 to 0x10ffff), or the value -1 indicating the end of the source.
