## PeterO.Text.Normalizer

    public sealed class Normalizer

<b>Deprecated.</b> Use NormalizingCharacterInput instead; that class is much more flexible than Normalizer.

Implements the Unicode normalization algorithm and contains methods and functionality to test and convert Unicode strings for Unicode normalization.

NOTICE: While this class's source code is in the public domain, the class uses an internal class, called NormalizationData, that includes data derived from the Unicode Character Database. See the documentation for the NormalizingCharacterInput class for the permission notice for the Unicode Character Database.

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

 * <i>str</i>: Not documented yet.

 * <i>form</i>: A Normalization object.

<b>Returns:</b>

A Boolean object.

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
The parameter  <i>chars</i>
 is null.

### ReadChar

    public int ReadChar();

Not documented yet.

<b>Returns:</b>

A 32-bit signed integer.
