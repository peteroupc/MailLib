## PeterO.Text.Idna

    public static class Idna

Contains methods that implement Internationalized Domain Names in Applications (IDNA). IDNA enables using a wider range of letters, numbers, and certain other characters in domain names. This class implements the 2008 revision of IDNA, also known as IDNA2008.

NOTICE: While this class's source code is in the public domain, the class uses two internal classes, called `NormalizationData` and `IdnaData` , that include data derived from the Unicode Character Database. See he documentation for the NormalizerInput class for the permission otice for the Unicode Character Database.

### DecodeDomainName

    public static string DecodeDomainName(
        string value);

Tries to encode each XN-label (Basic Latin label starting with "xn--") of the given domain name into Unicode. This method does not check the syntactic validity of the domain name before proceeding.

<b>Parameters:</b>

 * <i>value</i>: A domain name.

<b>Return Value:</b>

The domain name where each XN-label is encoded into Unicode. Labels where this is not possible remain unchanged.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>value</i>
 is null.

### EncodeDomainName

    public static string EncodeDomainName(
        string value);

Tries to encode each label of a domain name with code points outside the Basic Latin range (U+0000 to U+007F) into an XN-label (a label starting with "xn--" and having only basic letters, basic digits, and/or "-"). This method does not check the syntactic validity of the domain name before proceeding.

<b>Parameters:</b>

 * <i>value</i>: A domain name.

<b>Return Value:</b>

The domain name where each label with code points outside the Basic Latin range (U+0000 to U+007F) is encoded into an XN-label. Labels where this is not possible remain unchanged.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>value</i>
 is null.

### IsValidDomainName

    public static bool IsValidDomainName(
        string str,
        bool lookupRules);

Determines whether the given string is a domain name containing only U-labels (labels meeting IDNA2008 requirements for labels with characters outside the Basic Latin range, U+0000 to U+007F), A-labels (labels starting with "xn--" and convertible to U-labels), NR-LDH labels (as defined in RFC 5890), or any combination of these, separated by dots ("."). See RFC 5890 and 5891 (IDNA).

<b>Parameters:</b>

 * <i>str</i>: The parameter <i>str</i>
is a text string.

 * <i>lookupRules</i>: If true, uses rules to apply when looking up the string as a domain name. If false, uses rules to apply when registering the string as a domain name.

<b>Return Value:</b>

 `true` if the given string is a syntactically valid domain name; otherwise; alse.
