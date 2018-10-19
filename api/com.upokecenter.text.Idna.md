# com.upokecenter.text.Idna

    public final class Idna extends Object

<p>Contains methods that implement Internationalized Domain Names in
 Applications (IDNA). IDNA enables using a wider range of letters,
 numbers, and certain other characters in domain names. This class
 implements the 2008 revision of IDNA, also known as IDNA2008.</p>
 <p>The following summarizes the rules for domain names in IDNA2008;
 see RFC5890 for more information and additional terminology.</p> <p>A
 domain name is divided into one or more strings separated by dots
 ("."), called <b>labels</b>. For IDNA2008's purposes, a valid label
 can be an <b>NR-LDH label</b>, an <b>A-label</b>, or a
 <b>U-label</b>.</p> <p>An LDH label contains only basic letters,
 basic digits, and/or "-", and neither begins nor ends with "-". For
 example, "exa-mple" and "1example" are LDH labels, but not
 "-example".</p> <p>An NR-LDH label is an LDH label whose third and
 fourth characters are not both "-". For example, "ex--ample" is not
 an NR-LDH label.</p> <p>A U-label contains one or more characters
 outside the Basic Latin range (U + 0000 to U + 007F) and meets IDNA2008
 requirements for labels with such characters. An example is
 "e&#xe1;".</p> <p>An A-label is an LDH label beginning with "xn--" in
 any combination of case, and is convertible to a U-label. An example
 is "xn--e-ufa".</p> <p>An XN-label is an LDH label beginning with
 "xn--" in any combination of case.</p> <p>NOTICE: While this class's
 source code is in the public domain, the class uses two internal
 classes, called <code>NormalizationData</code> and <code>IdnaData</code>, that
 include data derived from the Unicode Character Database. See the
 documentation for the NormalizerInput class for the permission notice
 for the Unicode Character Database.</p>

## Methods

* `static String DecodeDomainName​(String value)`<br>
 Tries to encode each XN-label of the given domain name into Unicode.
* `static String EncodeDomainName​(String value)`<br>
 Tries to encode each label of a domain name with code points outside the
 Basic Latin range (U + 0000 to U + 007F) into an XN-label.
* `static boolean IsValidDomainName​(String str,
                 boolean lookupRules)`<br>
 Determines whether the given string is a domain name containing only
 U-labels, A-labels, NR-LDH labels, or any combination of these,
 separated by dots (".").

## Method Details

### DecodeDomainName
    public static String DecodeDomainName​(String value)
Tries to encode each XN-label of the given domain name into Unicode. This
 method does not check the syntactic validity of the domain name
 before proceeding.

**Parameters:**

* <code>value</code> - A domain name.

**Returns:**

* The domain name where each XN-label is encoded into Unicode. Labels
 where this is not possible remain unchanged.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>value</code> is null.

### EncodeDomainName
    public static String EncodeDomainName​(String value)
Tries to encode each label of a domain name with code points outside the
 Basic Latin range (U + 0000 to U + 007F) into an XN-label. This method
 does not check the syntactic validity of the domain name before
 proceeding.

**Parameters:**

* <code>value</code> - A domain name.

**Returns:**

* The domain name where each label with code points outside the Basic
 Latin range (U + 0000 to U + 007F) is encoded into an XN-label. Labels
 where this is not possible remain unchanged.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>value</code> is null.

### IsValidDomainName
    public static boolean IsValidDomainName​(String str, boolean lookupRules)
Determines whether the given string is a domain name containing only
 U-labels, A-labels, NR-LDH labels, or any combination of these,
 separated by dots (".").

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

* <code>lookupRules</code> - If true, uses rules to apply when looking up the string
 as a domain name. If false, uses rules to apply when registering the
 string as a domain name.

**Returns:**

* <code>true</code> if the given string is a syntactically valid domain
 name; otherwise; false.
