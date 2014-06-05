## PeterO.Text.Idna

    public static class Idna

Contains methods that implement Internationalized Domain Names in Applications (IDNA).

NOTICE: While this class's source code is in the public domain, the class uses two internal classes, called  `NormalizationData` and  `IdnaData` , that include data derived from the Unicode Character Database. See the documentation for the Normalizer class for the permission notice for the Unicode Character Database.

### EncodeDomainName

    public static string EncodeDomainName(
        string value);

Tries to encode each label of a domain name into PunyCode.

<b>Parameters:</b>

 * <i>value</i>: A domain name.

<b>Returns:</b>

The domain name where each label with non-ASCII characters is encoded into PunyCode. Labels where this is not possible remain unchanged.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
Value is null.

### IsValidDomainName

    public static bool IsValidDomainName(
        string str,
        bool lookupRules);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>lookupRules</i>: A Boolean object. (2).

<b>Returns:</b>

A Boolean object.


