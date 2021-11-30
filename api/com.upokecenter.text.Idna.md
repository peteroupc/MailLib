# com.upokecenter.text.Idna

## Methods

* `static java.lang.String DecodeDomainName​(java.lang.String value)`<br>
 Tries to encode each XN-label of the given domain name into Unicode.
* `static java.lang.String EncodeDomainName​(java.lang.String value)`<br>
 Tries to encode each label of a domain name with code points outside the
 Basic Latin range (U+0000 to U+007F) into an XN-label.
* `static boolean IsValidDomainName​(java.lang.String str,
boolean lookupRules)`<br>
 Determines whether the given string is a domain name containing only
 U-labels, A-labels, NR-LDH labels, or any combination of these,
  separated by dots (".").

## Method Details

### <a id='DecodeDomainName(java.lang.String)'>DecodeDomainName</a>

Tries to encode each XN-label of the given domain name into Unicode. This
 method does not check the syntactic validity of the domain name
 before proceeding.

**Parameters:**

* <code>value</code> - A domain name.

**Returns:**

* The domain name where each XN-label is encoded into Unicode. Labels
 where this is not possible remain unchanged.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>value</code> is null.

### <a id='EncodeDomainName(java.lang.String)'>EncodeDomainName</a>

Tries to encode each label of a domain name with code points outside the
 Basic Latin range (U+0000 to U+007F) into an XN-label. This method
 does not check the syntactic validity of the domain name before
 proceeding.

**Parameters:**

* <code>value</code> - A domain name.

**Returns:**

* The domain name where each label with code points outside the Basic
 Latin range (U+0000 to U+007F) is encoded into an XN-label. Labels
 where this is not possible remain unchanged.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>value</code> is null.

### <a id='IsValidDomainName(java.lang.String,boolean)'>IsValidDomainName</a>

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

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.
