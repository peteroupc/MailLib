## PeterO.Text.NormalizingCharacterInput

    public sealed class NormalizingCharacterInput :
        PeterO.Text.ICharacterInput

Implements the Unicode normalization algorithm and contains methods and functionality to test and convert Unicode strings for Unicode normalization. This is similar to the Normalizer class, except it implements the ICharacterInput interface.

NOTICE: While this class's source code is in the public domain, the class uses an internal class, called NormalizationData, that includes data derived from the Unicode Character Database. See the documentation for the Normalizer class for the permission notice for the Unicode Character Database.

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

 * <i>form</i>: A Normalization object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str);

Initializes a new instance of the NormalizingCharacterInput class using Normalization Form C.

<b>Parameters:</b>

 * <i>str</i>: A string object.

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

 * <i>form</i>: A Normalization object.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        string str,
        PeterO.Text.Normalization form);

Initializes a new instance of the NormalizingCharacterInput class.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>form</i>: A Normalization object.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        System.Collections.Generic.IList characterList);

Initializes a new instance of the NormalizingCharacterInput class using Normalization Form C.

<b>Parameters:</b>

 * <i>characterList</i>: An IList object.

### NormalizingCharacterInput Constructor

    public NormalizingCharacterInput(
        System.Collections.Generic.IList characterList,
        PeterO.Text.Normalization form);

Initializes a new instance of the NormalizingCharacterInput class using the given normalization form.

<b>Parameters:</b>

 * <i>characterList</i>: An IList object.

 * <i>form</i>: A Normalization object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>characterList</i>
 is null.

### GetChars

    public static System.Collections.Generic.IList GetChars(
        PeterO.Text.ICharacterInput str,
        PeterO.Text.Normalization form);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: An ICharacterInput object.

 * <i>form</i>: A Normalization object.

<b>Returns:</b>

A list of Unicode characters.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
is null.

### GetChars

    public static System.Collections.Generic.IList GetChars(
        string str,
        PeterO.Text.Normalization form);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>form</i>: A Normalization object.

<b>Returns:</b>

A list of Unicode characters.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
is null.

### IsNormalized

    public static bool IsNormalized(
        PeterO.Text.ICharacterInput chars,
        PeterO.Text.Normalization form);

Not documented yet.

<b>Parameters:</b>

 * <i>chars</i>: An ICharacterInput object.

 * <i>form</i>: A Normalization object.

<b>Returns:</b>

A Boolean object.

### IsNormalized

    public static bool IsNormalized(
        string str,
        PeterO.Text.Normalization form);

Determines whether the given string is in the given Unicode normalization form.

<b>Parameters:</b>

 * <i>str</i>: An arbitrary string.

 * <i>form</i>: A Normalization object.

<b>Returns:</b>

True if the given string is in the given Unicode normalization form; otherwise, false.

### IsNormalized

    public static bool IsNormalized(
        System.Collections.Generic.IList charList,
        PeterO.Text.Normalization form);

Determines whether the given list of characters is in the given Unicode normalization form.

<b>Parameters:</b>

 * <i>charList</i>: An IList object.

 * <i>form</i>: A Normalization object.

<b>Returns:</b>

True if the given list of characters is in the given Unicode normalization form; otherwise, false.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "chars" is null.

### Read

    public sealed int Read(
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
The parameter  <i>chars</i>
 or "this.buffer" is null.

### ReadChar

    public sealed int ReadChar();

Not documented yet.

<b>Returns:</b>

A 32-bit signed integer.
