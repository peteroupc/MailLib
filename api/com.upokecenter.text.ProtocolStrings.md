# com.upokecenter.text.ProtocolStrings

## Methods

* `static boolean IsInFreeformClass​(java.lang.String str)`<br>
 Determines whether the given string belongs in RFC 8264's FreeformClass.
* `static boolean IsInIdentifierClass​(java.lang.String str)`<br>
 Determines whether the given string belongs in RFC 8264's IdentifierClass.
* `static java.lang.String NicknameEnforce​(java.lang.String str)`<br>
 Checks the validity of a string serving as a "memorable, human-friendly
  name" for something (see RFC 8266), as opposed to that thing's
 identity for authentication or authorization purposes (see sec.
* `static java.lang.String NicknameForComparison​(java.lang.String str)`<br>
 Prepares for comparison a string serving as a "memorable, human-friendly
  name" for something (see RFC 8266), as opposed to that thing's
 identity for authentication or authorization purposes (see sec.
* `static java.lang.String OpaqueStringEnforce​(java.lang.String str)`<br>
 Checks the validity of a string serving as an arbitrary single-line sequence
 of characters, such as a passphrase.
* `static java.lang.String UsernameEnforce​(java.lang.String str)`<br>
 Checks the validity of a string that can serve to identify a user or account
  (a "username"), where the string is made of one or more parts called
  "userparts" separated by spaces (U+0020) and where the case of
 letters in the string is mapped to lowercase.
* `static java.lang.String UsernameEnforce​(java.lang.String str,
boolean preserveCase)`<br>
 Checks the validity of a string that can serve to identify a user or account
  (a "username"), where the string is made of one or more parts called
  "userparts" separated by spaces (U+0020) and where the case of
 letters in the string is either mapped to lowercase or preserved.
* `static java.lang.String UserpartEnforce​(java.lang.String str)`<br>
 Checks the validity of a string without spaces that can serve to identify a
  user or account (a "userpart"), where the case of letters in the
 string is mapped to lowercase.
* `static java.lang.String UserpartEnforce​(java.lang.String str,
boolean preserveCase)`<br>
 Checks the validity of a string without spaces that can serve to identify a
  user or account (a "userpart"), where the case of letters in the
 string is either mapped to lowercase or preserved.

## Method Details

### <a id='IsInIdentifierClass(java.lang.String)'>IsInIdentifierClass</a>

Determines whether the given string belongs in RFC 8264's IdentifierClass.
 In general, the IdentifierClass contains all code points in the
 FreeformClass, except certain uncommon letters and digits, spaces,
 as well as punctuation and symbols outside the Basic Latin range
 (U+0000 to U+007F).

**Parameters:**

* <code>str</code> - A string to check.

**Returns:**

* <code>true</code> if the given string is empty or contains only
 characters allowed in RFC 8264's IdentifierClass (in the contexts
 required); otherwise, <code>false</code>. Returns <code>false</code> if <code>
 str</code> is null.

### <a id='IsInFreeformClass(java.lang.String)'>IsInFreeformClass</a>

Determines whether the given string belongs in RFC 8264's FreeformClass. In
 general, the FreeformClass contains most letters, digits, spaces,
 punctuation, and symbols in the Unicode standard, as well as all
 basic printable characters (U+0021 to U+007E), but excludes control
 characters and separators. Horizontal tab, U+0009, and other code
 points in the range U+0000 to U+001F, are among the excluded
 characters.

**Parameters:**

* <code>str</code> - A string to check.

**Returns:**

* <code>true</code> if the given string is empty or contains only
 characters allowed in RFC 8264's FreeformClass (in the contexts
 required); otherwise, <code>false</code>. Returns <code>false</code> if <code>
 str</code> is null.

### <a id='UserpartEnforce(java.lang.String)'>UserpartEnforce</a>

Checks the validity of a string without spaces that can serve to identify a
  user or account (a "userpart"), where the case of letters in the
 string is mapped to lowercase. This checking is done using the
 UsernameCaseMapped profile in RFC 8265.

**Parameters:**

* <code>str</code> - A string to prepare that represents a user or account identifier.

**Returns:**

* A userpart prepared under the UsernameCaseMapped profile in RFC 8265
 (among other things, the string will be converted to lowercase).
 Returns null if <code>str</code> is invalid under that profile (including
 if <code>str</code> is null or empty). For comparison purposes, return
 values of this method should be compared code point by code point
 (see RFC 8265, sec. 3.3.4).

### <a id='UsernameEnforce(java.lang.String)'>UsernameEnforce</a>

Checks the validity of a string that can serve to identify a user or account
  (a "username"), where the string is made of one or more parts called
  "userparts" separated by spaces (U+0020) and where the case of
 letters in the string is mapped to lowercase. This checking is done
 using the UsernameCaseMapped profile in RFC 8265.

**Parameters:**

* <code>str</code> - A string to prepare that represents a user or account identifier.

**Returns:**

* A username where each of its parts is prepared under the
 UsernameCaseMapped profile in RFC 8265 (among other things, the
 string will be converted to lowercase). Returns null if any of those
 parts is invalid under that profile (including if <code>str</code> is
 null or empty). Note that there will be as many spaces of separation
 between parts of the return value as between parts of the input;
 this method will not collapse multiple spaces (U+0020) into a single
 space. If such space collapsing on a string (or rejection of strings
 with multiple consecutive spaces) is desired, it should be done
 before that string is passed to this method. For comparison
 purposes, return values of this method should be compared code point
 by code point (see RFC 8265, sec. 3.3.4).

### <a id='UserpartEnforce(java.lang.String,boolean)'>UserpartEnforce</a>

Checks the validity of a string without spaces that can serve to identify a
  user or account (a "userpart"), where the case of letters in the
 string is either mapped to lowercase or preserved. This checking is
 done using the UsernameCaseMapped or UsernameCasePreserved profile
 in RFC 8265.

**Parameters:**

* <code>str</code> - A string to prepare that represents a user or account identifier.

* <code>preserveCase</code> - If true, use the UsernameCasePreserved profile to
 prepare the string. If false, use the UsernameCaseMapped profile.

**Returns:**

* A userpart prepared under the UsernameCaseMapped or
 UsernameCasePreserved profile in RFC 8265. Returns null if <code>
 str</code> is invalid under that profile (including if <code>str</code> is null
 or empty). For comparison purposes, return values of this method
 (with the same value for <code>preserveCase</code>) should be compared
 code point by code point (see RFC 8265, secs. 3.3.4 and 3.4.4).

### <a id='UsernameEnforce(java.lang.String,boolean)'>UsernameEnforce</a>

Checks the validity of a string that can serve to identify a user or account
  (a "username"), where the string is made of one or more parts called
  "userparts" separated by spaces (U+0020) and where the case of
 letters in the string is either mapped to lowercase or preserved.
 This checking is done using the UsernameCaseMapped or
 UsernameCasePreserved profile in RFC 8265.

**Parameters:**

* <code>str</code> - A string to prepare that represents a user or account identifier.

* <code>preserveCase</code> - If true, use the UsernameCasePreserved profile to
 prepare each part of the string. If false, use the
 UsernameCaseMapped profile.

**Returns:**

* A username where each of its parts is prepared under the
 UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265.
 Returns null if any of those parts is invalid under that profile
 (including if <code>str</code> is null or empty). Note that there will be
 as many spaces of separation between parts of the return value as
 between parts of the input; this method will not collapse multiple
 spaces (U+0020) into a single space. If such space collapsing on a
 string (or rejection of strings with multiple consecutive spaces) is
 desired, it should be done before that string is passed to this
 method. For comparison purposes, return values of this method (with
 the same value for <code>preserveCase</code>) should be compared code
 point by code point (see RFC 8265, secs. 3.3.4 and 3.4.4).

### <a id='OpaqueStringEnforce(java.lang.String)'>OpaqueStringEnforce</a>

Checks the validity of a string serving as an arbitrary single-line sequence
 of characters, such as a passphrase. This checking is done using the
 OpaqueString profile in RFC 8265.

**Parameters:**

* <code>str</code> - A string to prepare that represents an arbitrary single-line
 sequence of characters entered by a user.

**Returns:**

* A string prepared under the OpaqueString profile in RFC 8265.
 Returns null if that string is invalid under that profile (including
 if <code>str</code> is null or empty). For comparison purposes, return
 values of this method should be compared code point by code point
 (see RFC 8265, sec. 4.2.3).

### <a id='NicknameEnforce(java.lang.String)'>NicknameEnforce</a>

Checks the validity of a string serving as a "memorable, human-friendly
  name" for something (see RFC 8266), as opposed to that thing's
 identity for authentication or authorization purposes (see sec. 6.1
 of that RFC). This checking is done using the Nickname profile in
 RFC 8266. Such names are not intended to serve as URIs or file paths
 (see sec. 6.1 of that RFC).

**Parameters:**

* <code>str</code> - A string serving as a nickname for something.

**Returns:**

* A nickname prepared for enforcement under the Nickname profile in
 RFC 8266. Returns null if that string is invalid under that profile
 (including if <code>str</code> is null or empty). Return values of this
 method should not be used for comparison purposes (see RFC 8266,
 sec. 2.3); for such purposes, use the NicknameForComparison method
 instead.

### <a id='NicknameForComparison(java.lang.String)'>NicknameForComparison</a>

Prepares for comparison a string serving as a "memorable, human-friendly
  name" for something (see RFC 8266), as opposed to that thing's
 identity for authentication or authorization purposes (see sec. 6.1
 of that RFC). This operation is done using the Nickname profile in
 RFC 8266. Such names are not intended to serve as URIs or file paths
 (see sec. 6.1 of that RFC).

**Parameters:**

* <code>str</code> - A string serving as a nickname for something.

**Returns:**

* A nickname prepared for comparison under the Nickname profile in RFC
 8266. Returns null if that string is invalid under that profile
 (including if <code>str</code> is null or empty). For comparison
 purposes, return values of this method should be compared code point
 by code point (see RFC 8266, sec. 2.4).
