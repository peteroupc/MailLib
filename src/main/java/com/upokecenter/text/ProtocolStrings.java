package com.upokecenter.text;

  /**
   * <p>Contains methods for preparing user-facing protocol strings (such as user
   * identifiers) for equality comparison and validity checking. Such strings can
   * be _internationalized_, that is, contain characters beyond the Basic Latin
   * block (U+0000 to U+007F) of the Unicode Standard. See RFC 8264. Currently
   * there are four profiles for internationalized strings: two for strings
   * serving as user identifiers, one for arbitrary single-line strings (such as
   * passwords), and one for display names.</p><ul> <li>Other user-facing
   * internationalized strings not expressly handled by this class include the
   * following. Their preparation and comparison are outside the scope of this
   * class. <br/> -- File and directory names. <br/> -- Domain names. <br/> --
   * Text strings with multiple language versions (such as a checkbox's label or
   * a dialog box's title). <br/> -- Profile data voluntarily entered by users.
   * <br/> -- The text of article, post, and message bodies.</li><li>The methods
   * in this class are not well suited for <i>collation</i>, or lexicographic
   * ordering, which is a comparison of text strings that is usually
   * language-dependent and goes beyond equality comparison. Further discussion
   * on collation can be found in Unicode Technical Standard 10 (UTS 10),
   * "Unicode Collation Algorithm".</li><li>As explained in UTS 10 sec. 1.6,
   * collation serves the purposes of searching and selection (for example,
   * searching by name or by title). However, this class is directed more to
   * equality comparisons for authentication or authorization purposes, or to
   * avoid creating multiple items that use the same string, rather than, say, to
   * comparisons of names or parts of names for the purpose of showing matching
   * records.</li></ul> <p><b>Security Considerations</b></p> <p>Many of the
   * methods in this class take text strings and output text strings. However,
   * specifying text strings as these methods do is not ideal if the string
   * represents a password or other sensitive data, since strings are immutable
   * in.NET and Java, so that they can't be modified, and the memory they occupy
   * is not guaranteed to be cleared in a timely fashion due to garbage
   * collection.</p> <p>The methods in this class are not guaranteed to be
   * "constant-time" (nondata-dependent) for all relevant inputs. Neither are
   * string comparison methods (such as string.equals) necessarily guaranteed to
   * be "constant-time". Certain attacks that involve encrypted communications
   * have exploited the timing and other aspects of such communications to derive
   * keying material or cleartext indirectly, or for example, to leak information
   * about whether a user name and password were accepted by the server, or
   * whether a user name or display name was already taken. This is an important
   * consideration especially for strings representing account identifiers or
   * passwords, even when comparing two such strings of the same length.</p>
   */
  public final class ProtocolStrings {
private ProtocolStrings() {
}
    /**
     * Determines whether the specified string belongs in RFC 8264's
     * IdentifierClass. In general, the IdentifierClass contains all code points in
     * the FreeformClass, except certain uncommon letters and digits, spaces, as
     * well as punctuation and symbols outside the Basic Latin range (U+0000 to
     * U+007F).
     * @param str A string to check.
     * @return {@code true} if the specified string is empty or contains only
     * characters allowed in RFC 8264's IdentifierClass (in the contexts required);
     * otherwise, {@code false}. Returns {@code false} if {@code str} is null.
     */
    public static boolean IsInIdentifierClass(String str) {
      return Idna.IsInIdentifierClass(str);
    }

    /**
     * Determines whether the specified string belongs in RFC 8264's FreeformClass.
     * In general, the FreeformClass contains most letters, digits, spaces,
     * punctuation, and symbols in the Unicode standard, as well as all basic
     * printable characters (U+0021 to U+007E), but excludes control characters and
     * separators. Horizontal tab, U+0009, and other code points in the range
     * U+0000 to U+001F, are among the excluded characters.
     * @param str A string to check.
     * @return {@code true} if the specified string is empty or contains only
     * characters allowed in RFC 8264's FreeformClass (in the contexts required);
     * otherwise, {@code false}. Returns {@code false} if {@code str} is null.
     */
    public static boolean IsInFreeformClass(String str) {
      return Idna.IsInFreeformClass(str);
    }

    /**
     * Checks the validity of a string without spaces that can serve to identify a
     * user or account (a "userpart"), where the case of letters in the string is
     * mapped to lowercase. This checking is done using the UsernameCaseMapped
     * profile in RFC 8265.
     * @param str A string to prepare that represents a user or account identifier.
     * @return A userpart prepared under the UsernameCaseMapped profile in RFC 8265
     * (among other things, the string will be converted to lowercase). Returns
     * null if {@code str} is invalid under that profile (including if {@code str}
     * is null or empty). For comparison purposes, return values of this method
     * should be compared code point by code point (see RFC 8265, sec. 3.3.4).
     */
    public static String UserpartEnforce(String str) {
      return UserpartEnforce(str, false);
    }

    /**
     * Checks the validity of a string that can serve to identify a user or account
     * (a "username"), where the string is made of one or more parts called
     * "userparts" separated by spaces (U+0020) and where the case of letters in
     * the string is mapped to lowercase. This checking is done using the
     * UsernameCaseMapped profile in RFC 8265.
     * @param str A string to prepare that represents a user or account identifier.
     * @return A username where each of its parts is prepared under the
     * UsernameCaseMapped profile in RFC 8265 (among other things, the string will
     * be converted to lowercase). Returns null if any of those parts is invalid
     * under that profile (including if {@code str} is null or empty). Note that
     * there will be as many spaces of separation between parts of the return value
     * as between parts of the input; this method will not collapse multiple spaces
     * (U+0020) into a single space. If such space collapsing on a string (or
     * rejection of strings with multiple consecutive spaces) is desired, it should
     * be done before that string is passed to this method. For comparison
     * purposes, return values of this method should be compared code point by code
     * point (see RFC 8265, sec. 3.3.4).
     */
    public static String UsernameEnforce(String str) {
      return UsernameEnforce(str, false);
    }

    /**
     * Checks the validity of a string without spaces that can serve to identify a
     * user or account (a "userpart"), where the case of letters in the string is
     * either mapped to lowercase or preserved. This checking is done using the
     * UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265.
     * @param str A string to prepare that represents a user or account identifier.
     * @param preserveCase If true, use the UsernameCasePreserved profile to
     * prepare the string. If false, use the UsernameCaseMapped profile.
     * @return A userpart prepared under the UsernameCaseMapped or
     * UsernameCasePreserved profile in RFC 8265. Returns null if {@code str} is
     * invalid under that profile (including if {@code str} is null or empty). For
     * comparison purposes, return values of this method (with the same value for
     * {@code preserveCase}) should be compared code point by code point (see RFC
     * 8265, secs. 3.3.4 and 3.4.4).
     */
    public static String UserpartEnforce(String str, boolean preserveCase) {
      if (preserveCase) {
        return (str == null) ? null :
          Idna.UsernameCasePreservedEnforce(str);
        } else {
        return (str == null) ? null : Idna.UsernameCaseMappedEnforce(str);
      }
    }

    /**
     * Checks the validity of a string that can serve to identify a user or account
     * (a "username"), where the string is made of one or more parts called
     * "userparts" separated by spaces (U+0020) and where the case of letters in
     * the string is either mapped to lowercase or preserved. This checking is done
     * using the UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265.
     * @param str A string to prepare that represents a user or account identifier.
     * @param preserveCase If true, use the UsernameCasePreserved profile to
     * prepare each part of the string. If false, use the UsernameCaseMapped
     * profile.
     * @return A username where each of its parts is prepared under the
     * UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265. Returns
     * null if any of those parts is invalid under that profile (including if
     * {@code str} is null or empty). Note that there will be as many spaces of
     * separation between parts of the return value as between parts of the input;
     * this method will not collapse multiple spaces (U+0020) into a single space.
     * If such space collapsing on a string (or rejection of strings with multiple
     * consecutive spaces) is desired, it should be done before that string is
     * passed to this method. For comparison purposes, return values of this method
     * (with the same value for {@code preserveCase}) should be compared code
     * point by code point (see RFC 8265, secs. 3.3.4 and 3.4.4).
     */
    public static String UsernameEnforce(String str, boolean preserveCase) {
      if (((str) == null || (str).length() == 0)) {
        return null;
      }
      if (str.charAt(0) == ' ' || str.charAt(str.length() - 1) == ' ') {
        return null;
      }
      int lastPos = 0;
      int i = 0;
      StringBuilder sb = null;
      while (i < str.length()) {
        if (str.charAt(i) == ' ') {
          String part = UserpartEnforce(
              str.substring(lastPos, (lastPos)+(i - lastPos)),
              preserveCase);
          if (part == null) {
            return null;
          }
          sb = (sb == null) ? (new StringBuilder()) : sb;
          sb.append(part);
          while (i < str.length()) {
            if (str.charAt(i) != ' ') {
              break;
            }
            sb.append(' ');
            ++i;
          }
          lastPos = i;
        } else {
          ++i;
        }
      }
      if (lastPos == 0) {
        return UserpartEnforce(str, preserveCase);
      }
      if (lastPos != str.length()) {
        String part = UserpartEnforce(
            str.substring(lastPos, (lastPos)+(str.length() - lastPos)),
            preserveCase);
        if (part == null) {
          return null;
        }
        sb = (sb == null) ? (new StringBuilder()) : sb;
        sb.append(part);
      }
      return sb.toString();
    }

    /**
     * Checks the validity of a string serving as an arbitrary single-line sequence
     * of characters, such as a passphrase. This checking is done using the
     * OpaqueString profile in RFC 8265.
     * @param str A string to prepare that represents an arbitrary single-line
     * sequence of characters entered by a user.
     * @return A string prepared under the OpaqueString profile in RFC 8265.
     * Returns null if that string is invalid under that profile (including if
     * {@code str} is null or empty). For comparison purposes, return values of
     * this method should be compared code point by code point (see RFC 8265, sec.
     * 4.2.3).
     */
    public static String OpaqueStringEnforce(String str) {
      return (str == null) ? null : Idna.OpaqueStringEnforce(str);
    }

    /**
     * Checks the validity of a string serving as a "memorable, human-friendly
     * name" for something (see RFC 8266), as opposed to that thing's identity for
     * authentication or authorization purposes (see sec. 6.1 of that RFC). This
     * checking is done using the Nickname profile in RFC 8266. Such names are not
     * intended to serve as URIs or file paths (see sec. 6.1 of that RFC).
     * @param str A string serving as a nickname for something.
     * @return A nickname prepared for enforcement under the Nickname profile in
     * RFC 8266. Returns null if that string is invalid under that profile
     * (including if {@code str} is null or empty). Return values of this method
     * should not be used for comparison purposes (see RFC 8266, sec. 2.3); for
     * such purposes, use the NicknameForComparison method instead.
     */
    public static String NicknameEnforce(String str) {
      return (str == null) ? null : Idna.NicknameEnforce(str);
    }

    /**
     * Prepares for comparison a string serving as a "memorable, human-friendly
     * name" for something (see RFC 8266), as opposed to that thing's identity for
     * authentication or authorization purposes (see sec. 6.1 of that RFC). This
     * operation is done using the Nickname profile in RFC 8266. Such names are not
     * intended to serve as URIs or file paths (see sec. 6.1 of that RFC).
     * @param str A string serving as a nickname for something.
     * @return A nickname prepared for comparison under the Nickname profile in RFC
     * 8266. Returns null if that string is invalid under that profile (including
     * if {@code str} is null or empty). For comparison purposes, return values of
     * this method should be compared code point by code point (see RFC 8266, sec.
     * 2.4).
     */
    public static String NicknameForComparison(String str) {
      return (str == null) ? null : Idna.NicknameForComparison(str);
    }
  }
