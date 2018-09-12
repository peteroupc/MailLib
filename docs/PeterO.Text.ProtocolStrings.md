## PeterO.Text.ProtocolStrings

    public static class ProtocolStrings

Contains methods for preparing user-facing protocol strings (such as user identifiers) for comparison and validity checking. Such strings can be _internationalized_, that is, contain characters beyond the Basic Latin block (U+0000 to U+007F) of the Unicode Standard. See RFC 8264. (Currently there are three profiles for internationalized strings: one each for strings serving as user identifiers, arbitrary single-line strings [such as passwords], or display names. Other user-facing internationalized strings not expressly handled by this class include file and directory names, domain names, profile data voluntarily entered by users, and the text of article, post, and message bodies.).

### IsInFreeformClass

    public static bool IsInFreeformClass(
        string str);

Determines whether the given string belongs in RFC 8264's FreeformClass. In general, the FreeformClass contains most letters, digits, spaces, punctuation, and symbols in the Unicode standard, as well as all basic printable characters (U+0021 to U+007E), but excludes control characters and separators.

<b>Parameters:</b>

 * <i>str</i>: A string to check.

<b>Return Value:</b>

 `true`  if the given string is empty or contains only characters allowed in RFC 8264's FreeformClass (in the contexts required); otherwise,  `false` . Returns  `false`  if <i>str</i>
 is null.

### IsInIdentifierClass

    public static bool IsInIdentifierClass(
        string str);

Determines whether the given string belongs in RFC 8264's IdentifierClass. In general, the IdentifierClass contains all code points in the Freeform class, except certain uncommon letters and digits, spaces, as well as punctuation and symbols outside the Basic Latin range (U+0000 to U+007F).

<b>Parameters:</b>

 * <i>str</i>: A string to check.

<b>Return Value:</b>

 `true`  if the given string is empty or contains only characters allowed in RFC 8264's FreeformClass (in the contexts required); otherwise,  `false` . Returns  `false`  if <i>str</i>
 is null.

### NicknameEnforce

    public static string NicknameEnforce(
        string str);

Checks the validity of a string serving as a "memorable, human-friendly name" for something (see RFC 8266), as opposed to that thing's identity for authentication or authorization purposes (see sec. 6.1 of that RFC). This checking is done using the Nickname profile in RFC 8266.

<b>Parameters:</b>

 * <i>str</i>: A string serving as a nickname for something.

<b>Return Value:</b>

A nickname prepared for enforcement under the Nickname profile in RFC 8266. Returns null if that string is invalid under that profile (including if  <i>str</i>
 is null or empty). Return values of this method should not be used for comparison purposes (see RFC 8266, sec. 2.3); for such purposes, use the NicknameForComparison method instead.

### NicknameForComparison

    public static string NicknameForComparison(
        string str);

Prepares for comparison a string serving as a "memorable, human-friendly name" for something (see RFC 8266), as opposed to that thing's identity for authentication or authorization purposes (see sec. 6.1 of that RFC). This operation is done using the Nickname profile in RFC 8266.

<b>Parameters:</b>

 * <i>str</i>: A string serving as a nickname for something.

<b>Return Value:</b>

A nickname prepared for comparison under the Nickname profile in RFC 8266. Returns null if that string is invalid under that profile (including if  <i>str</i>
 is null or empty). For comparison purposes, return values of this method should be compared code point by code point (see RFC 8266, sec. 2.4).

### OpaqueStringEnforce

    public static string OpaqueStringEnforce(
        string str);

Checks the validity of a string serving as an arbitrary single-line sequence of characters, such as a passphrase. This checking is done using the OpaqueString profile in RFC 8265. (REMARK: Specifying a string as this method does is not ideal if the string represents a password or other sensitive data, since strings are immutable in .NET and Java, so that its contents cannot be cleared when done. An application concerned about security may want to reimplement this method by passing a clearable array of characters rather than a text string.).

<b>Parameters:</b>

 * <i>str</i>: A string to prepare that represents an arbitrary single-line sequence of characters entered by a user.

<b>Return Value:</b>

A string prepared under the OpaqueString profile in RFC 8265. Returns null if that string is invalid under that profile (including if  <i>str</i>
 is null or empty). For comparison purposes, return values of this method should be compared code point by code point (see RFC 8265, sec. 4.2.3).

### UsernameEnforce

    public static string UsernameEnforce(
        string str);

Checks the validity of a string that can serve to identify a user or account (a "username"), where the string is made of one or more parts called "userparts" separated by spaces (U+0020) and where the case of letters in the string is mapped to lowercase. This checking is done using the UsernameCaseMapped profile in RFC 8265.

<b>Parameters:</b>

 * <i>str</i>: A string to prepare that represents a user or account identifier.

<b>Return Value:</b>

A username where each of its parts is prepared under the UsernameCaseMapped profile in RFC 8265 (among other things, the string will be converted to lowercase). Returns null if any of those parts is invalid under that profile (including if  <i>str</i>
 is null or empty). Note that there will be as many spaces of separation between parts of the return value as between parts of the input; this method will not collapse multiple spaces (U+0020) into a single space. If such space collapsing on a string (or rejection of strings with multiple consecutive spaces) is desired, it should be done before that string is passed to this method. For comparison purposes, return values of this method should be compared code point by code point (see RFC 8265, sec. 3.3.4).

### UsernameEnforce

    public static string UsernameEnforce(
        string str,
        bool preserveCase);

Checks the validity of a string that can serve to identify a user or account (a "username"), where the string is made of one or more parts called "userparts" separated by spaces (U+0020) and where the case of letters in the string is either mapped to lowercase or preserved. This checking is done using the UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265.

<b>Parameters:</b>

 * <i>str</i>: A string to prepare that represents a user or account identifier.

 * <i>preserveCase</i>: If true, use the UsernameCasePreserved profile to prepare each part of the string. If false, use the UsernameCaseMapped profile.

<b>Return Value:</b>

A username where each of its parts is prepared under the UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265. Returns null if any of those parts is invalid under that profile (including if  <i>str</i>
 is null or empty). Note that there will be as many spaces of separation between parts of the return value as between parts of the input; this method will not collapse multiple spaces (U+0020) into a single space. If such space collapsing on a string (or rejection of strings with multiple consecutive spaces) is desired, it should be done before that string is passed to this method. For comparison purposes, return values of this method (with the same value for  <i>preserveCase</i>
 ) should be compared code point by code point (see RFC 8265, secs. 3.3.4 and 3.4.4).

### UserpartEnforce

    public static string UserpartEnforce(
        string str);

Checks the validity of a string without spaces that can serve to identify a user or account (a "userpart"), where the case of letters in the string is mapped to lowercase. This checking is done using the UsernameCaseMapped profile in RFC 8265.

<b>Parameters:</b>

 * <i>str</i>: A string to prepare that represents a user or account identifier.

<b>Return Value:</b>

A userpart prepared under the UsernameCaseMapped profile in RFC 8265 (among other things, the string will be converted to lowercase). Returns null if  <i>str</i>
 is invalid under that profile (including if  <i>str</i>
 is null or empty). For comparison purposes, return values of this method should be compared code point by code point (see RFC 8265, sec. 3.3.4).

### UserpartEnforce

    public static string UserpartEnforce(
        string str,
        bool preserveCase);

Checks the validity of a string without spaces that can serve to identify a user or account (a "userpart"), where the case of letters in the string is either mapped to lowercase or preserved. This checking is done using the UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265.

<b>Parameters:</b>

 * <i>str</i>: A string to prepare that represents a user or account identifier.

 * <i>preserveCase</i>: If true, use the UsernameCasePreserved profile to prepare the string. If false, use the UsernameCaseMapped profile.

<b>Return Value:</b>

A userpart prepared under the UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265. Returns null if <i>str</i>
 is invalid under that profile (including if <i>str</i>
 is null or empty). For comparison purposes, return values of this method (with the same value for  <i>preserveCase</i>
 ) should be compared code point by code point (see RFC 8265, secs. 3.3.4 and 3.4.4).
