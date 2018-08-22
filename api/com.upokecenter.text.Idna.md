# com.upokecenter.text.Idna

    public final class Idna extends Object

<p>Contains methods that implement Internationalized Domain Names in
 Applications (IDNA). IDNA enables using a wider range of letters,
 numbers, and certain other characters in domain names. This class
 implements the 2008 revision of IDNA, also known as IDNA2008. </p>
 <p>NOTICE: While this class's source code is in the public domain,
 the class uses two internal classes, called <code>NormalizationData</code>
 and <code>IdnaData</code> , that include data derived from the Unicode
 Character Database. See the documentation for the NormalizerInput
 class for the permission notice for the Unicode Character Database.
 </p>

## Methods

* `static String EncodeDomainName​(String value)`<br>
 Tries to encode each label of a domain name into Punycode.
* `static boolean IsValidDomainName​(String str,
                 boolean lookupRules)`<br>
 Determines whether the given string is a syntactically valid domain name.

## Method Details

### EncodeDomainName
    public static String EncodeDomainName​(String value)
Tries to encode each label of a domain name into Punycode.

**Parameters:**

* <code>value</code> - A domain name.

**Returns:**

* The domain name where each label with code points outside the Basic
 Latin range (U + 0000 to U + 007F) is encoded into Punycode. Labels where
 this is not possible remain unchanged.

**Throws:**

* <code>NullPointerException</code> - Value is null.

### IsValidDomainName
    public static boolean IsValidDomainName​(String str, boolean lookupRules)
Determines whether the given string is a syntactically valid domain name.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

* <code>lookupRules</code> - If true, uses rules to apply when looking up the string
 as a domain name. If false, uses rules to apply when registering the
 string as a domain name.

**Returns:**

* <code>true</code> if the given string is a syntactically valid domain
 name; otherwise; false.
