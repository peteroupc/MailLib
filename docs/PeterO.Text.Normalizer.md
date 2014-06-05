## PeterO.Text.Normalizer

    public sealed class Normalizer

Implements the Unicode normalization algorithm and contains methods and functionality to test and convert Unicode strings for Unicode normalization.

NOTICE: While this class's source code is in the public domain, the class uses an internal class, called NormalizationData, that includes data derived from the Unicode Character Database. In case doing so is required, the permission notice for the Unicode Character Database is given here:

COPYRIGHT AND PERMISSION NOTICE

Copyright (c) 1991-2014 Unicode, Inc. All rights reserved. Distributed under the Terms of Use in http://www.unicode.org/copyright.html.

 Permission is hereby granted, free of charge, to any person obtaining a copy of the Unicode data files and any associated documentation (the "Data Files") or Unicode software and any associated documentation (the "Software") to deal in the Data Files or Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, and/or sell copies of the Data Files or Software, and to permit persons to whom the Data Files or Software are furnished to do so, provided that (a) this copyright and permission notice appear with all copies of the Data Files or Software, (b) this copyright and permission notice appear in associated documentation, and (c) there is clear notice in each modified Data File or in the Software as well as in the documentation associated with the Data File(s) or Software that the data or software has been modified.

THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA FILES OR SOFTWARE.

Except as contained in this notice, the name of a copyright holder shall not be used in advertising or otherwise to promote the sale, use or other dealings in these Data Files or Software without prior written authorization of the copyright holder.

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
The parameter <i>str</i>
 is null.

### Normalizer Constructor

    public Normalizer(
        string str,
        PeterO.Text.Normalization form);

Initializes a new instance of the Normalizer class.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>form</i>: A Normalization object.

### IsNormalized

    public static bool IsNormalized(
        string str,
        PeterO.Text.Normalization form);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>form</i>: A Normalization object.

<b>Returns:</b>

A Boolean object.

### ReadChar

    public int ReadChar();

Not documented yet.

<b>Returns:</b>

A 32-bit signed integer.

### Read

    public int Read(
        int[] chars,
        int index,
        int length);

Not documented yet.

<b>Parameters:</b>

 * <i>chars</i>: An array of 32-bit unsigned integers.

 * <i>index</i>: A 32-bit signed integer. (2).

 * <i>length</i>: A 32-bit signed integer. (3).

<b>Returns:</b>

A 32-bit signed integer.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>chars</i>
 or "this.buffer" is null.


