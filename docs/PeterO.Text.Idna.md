## PeterO.Text.Idna

    public static class Idna

Contains methods that implement Internationalized Domain Names in Applications (IDNA). IDNA enables using a wider range of letters, numbers, and certain other characters in domain names. This class implements the 2008 revision of IDNA, also known as IDNA2008.

The following summarizes the rules for domain names in IDNA2008; see RFC5890 for more information and additional terminology.

A domain name is divided into one or more strings separated by dots ("."), called<b>labels</b>. For IDNA2008's purposes, a valid label can be an<b>NR-LDH label</b>, an<b>A-label</b>, or a<b>U-label</b>.

An LDH label contains only basic letters, basic digits, and/or "-", and neither begins nor ends with "-". For example, "exa-mple" and "1example" are LDH labels, but not "-example".

An NR-LDH label is an LDH label whose third and fourth characters are not both "-". For example, "ex--ample" is not an NR-LDH label.

A U-label contains one or more characters outside the Basic Latin range (U+0000 to U+007F) and meets IDNA2008 requirements for labels with such characters. An example is "eá".

An A-label is an LDH label beginning with "xn--" in any combination of case, and is convertible to a U-label. An example is "xn--e-ufa".

An XN-label is an LDH label beginning with "xn--" in any combination of case.

NOTICE: While this class's source code is in the public domain, the class uses two internal classes, called `NormalizationData` and `IdnaData` , that include data derived from the Unicode Character Database. See he documentation for the NormalizerInput class for the permission otice for the Unicode Character Database.

### Member Summary
* <code>[DecodeDomainName(string)](#DecodeDomainName_string)</code> - Tries to encode each XN-label of the given domain name into Unicode.
* <code>[EncodeDomainName(string)](#EncodeDomainName_string)</code> - Tries to encode each label of a domain name with code points outside the Basic Latin range (U+0000 to U+007F) into an XN-label.
* <code>[IsValidDomainName(string, bool)](#IsValidDomainName_string_bool)</code> - Determines whether the given string is a domain name containing only U-labels, A-labels, NR-LDH labels, or any combination of these, separated by dots (".

<a id="DecodeDomainName_string"></a>
### DecodeDomainName

    public static string DecodeDomainName(
        string value);

Tries to encode each XN-label of the given domain name into Unicode. This method does not check the syntactic validity of the domain name before proceeding.

<b>Parameters:</b>

 * <i>value</i>: A domain name.

<b>Return Value:</b>

The domain name where each XN-label is encoded into Unicode. Labels where this is not possible remain unchanged.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>value</i>
is null.

<a id="EncodeDomainName_string"></a>
### EncodeDomainName

    public static string EncodeDomainName(
        string value);

Tries to encode each label of a domain name with code points outside the Basic Latin range (U+0000 to U+007F) into an XN-label. This method does not check the syntactic validity of the domain name before proceeding.

<b>Parameters:</b>

 * <i>value</i>: A domain name.

<b>Return Value:</b>

The domain name where each label with code points outside the Basic Latin range (U+0000 to U+007F) is encoded into an XN-label. Labels where this is not possible remain unchanged.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>value</i>
is null.

<a id="IsValidDomainName_string_bool"></a>
### IsValidDomainName

    public static bool IsValidDomainName(
        string str,
        bool lookupRules);

Determines whether the given string is a domain name containing only U-labels, A-labels, NR-LDH labels, or any combination of these, separated by dots (".").

<b>Parameters:</b>

 * <i>str</i>: The parameter <i>str</i>
is a text string.

 * <i>lookupRules</i>: If true, uses rules to apply when looking up the string as a domain name. If false, uses rules to apply when registering the string as a domain name.

<b>Return Value:</b>

 `true` if the given string is a syntactically valid domain name; otherwise; alse.
