using System;
using System.Text;

namespace PeterO.Text {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Text.ProtocolStrings"]/*'/>
  internal static class ProtocolStrings {
    /// <summary>Determines whether the given string belongs in RFC 8264's
    /// IdentifierClass. In general, the IdentifierClass contains all code
    /// points in the Freeform class, except certain uncommon letters and
    /// digits as well as spaces, non-ASCII punctuation, and non-ASCII
    /// symbols.</summary>
    /// <param name='str'>A string to check.</param>
    /// <returns><c>true</c> if the given string is empty or contains only
    /// characters allowed in RFC 8264's FreeformClass (in the contexts
    /// required); otherwise, <c>false</c>. Returns <c>false</c> if
    /// <paramref name='str'/> is null.</returns>
    public static bool IsInIdentifierClass(string str) {
      return Idna.IsInIdentifierClass(str);
    }

    /// <summary>Determines whether the given string belongs in RFC 8264's
    /// FreeformClass. In general, the FreeformClass contains most letters,
    /// digits, spaces, punctuation, and symbols in the Unicode standard,
    /// as well as all ASCII printable characters, but excludes control
    /// characters and separators.</summary>
    /// <param name='str'>A string to check.</param>
    /// <returns><c>true</c> if the given string is empty or contains only
    /// characters allowed in RFC 8264's FreeformClass (in the contexts
    /// required); otherwise, <c>false</c>. Returns <c>false</c> if
    /// <paramref name='str'/> is null.</returns>
    public static bool IsInFreeformClass(string str) {
      return Idna.IsInFreeformClass(str);
    }

    /// <summary>Checks the validity of a string without spaces that can
    /// serve to identify a user or account (a "userpart"), where the case
    /// of letters in the string is mapped to lowercase. This checking is
    /// done using the UsernameCaseMapped profile in RFC 8265.</summary>
    /// <param name='str'>A string to prepare that represents a user or
    /// account identifier.</param>
    /// <returns>A userpart prepared under the UsernameCaseMapped profile
    /// in RFC 8265 (among other things, the string will be converted to
    /// lowercase). Returns null if <paramref name='str'/> is invalid under
    /// that profile (including if <paramref name='str'/> is null or
    /// empty). For comparison purposes, return values of this method
    /// should be compared code point by code point (see RFC 8265, sec.
    /// 3.3.4).</returns>
    public static string UserpartEnforce(string str) {
      return UserpartEnforce(str, false);
    }

    /// <summary>Checks the validity of a string that can serve to identify
    /// a user or account (a "username"), where the string is made of one
    /// or more parts called "userparts" separated by spaces (U+0020) and
    /// where the case of letters in the string is mapped to lowercase.
    /// This checking is done using the UsernameCaseMapped profile in RFC
    /// 8265.</summary>
    /// <param name='str'>A string to prepare that represents a user or
    /// account identifier.</param>
    /// <returns>A user name where each of its parts is prepared under the
    /// UsernameCaseMapped profile in RFC 8265 (among other things, the
    /// string will be converted to lowercase). Returns null if any of
    /// those parts is invalid under that profile (including if <paramref
    /// name='str'/> is null or empty). Note that there will be as many
    /// spaces of separation between parts of the return value as between
    /// parts of the input; this method will not collapse multiple spaces
    /// (U+0020) into a single space. If such space collapsing on a string
    /// (or rejection of strings with multiple consecutive spaces) is
    /// desired, it should be done before that string is passed to this
    /// method. For comparison purposes, return values of this method
    /// should be compared code point by code point (see RFC 8265, sec.
    /// 3.3.4).</returns>
    public static string UsernameEnforce(string str) {
      return UsernameEnforce(str, false);
    }

    /// <summary>Checks the validity of a string without spaces that can
    /// serve to identify a user or account (a "userpart"), where the case
    /// of letters in the string is either mapped to lowercase or
    /// preserved. This checking is done using the UsernameCaseMapped or
    /// UsernameCasePreserved profile in RFC 8265.</summary>
    /// <param name='str'>A string to prepare that represents a user or
    /// account identifier.</param>
    /// <param name='preserveCase'>If true, use the UsernameCasePreserved
    /// profile to prepare the string. If false, use the UsernameCaseMapped
    /// profile.</param>
    /// <returns>A userpart prepared under the UsernameCaseMapped or
    /// UsernameCasePreserved profile in RFC 8265. Returns null if
    /// <paramref name='str'/> is invalid under that profile (including if
    /// <paramref name='str'/> is null or empty). For comparison purposes,
    /// return values of this method (with the same value for <paramref
    /// name='preserveCase'/> ) should be compared code point by code point
    /// (see RFC 8265, secs. 3.3.4 and 3.4.4).</returns>
    public static string UserpartEnforce(string str, bool preserveCase) {
      if (preserveCase) {
 return Idna.UsernameCasePreservedEnforce(str);
} else {
 return Idna.UsernameCaseMappedEnforce(str);
}
    }

    /// <summary>Checks the validity of a string that can serve to identify
    /// a user or account (a "username"), where the string is made of one
    /// or more parts called "userparts" separated by spaces (U+0020) and
    /// where the case of letters in the string is either mapped to
    /// lowercase or preserved. This checking is done using the
    /// UsernameCaseMapped or UsernameCasePreserved profile in RFC
    /// 8265.</summary>
    /// <param name='str'>A string to prepare that represents a user or
    /// account identifier.</param>
    /// <param name='preserveCase'>If true, use the UsernameCasePreserved
    /// profile to prepare each part of the string. If false, use the
    /// UsernameCaseMapped profile.</param>
    /// <returns>A user name where each of its parts is prepared under the
    /// UsernameCaseMapped or UsernameCasePreserved profile in RFC 8265.
    /// Returns null if any of those parts is invalid under that profile
    /// (including if <paramref name='str'/> is null or empty). Note that
    /// there will be as many spaces of separation between parts of the
    /// return value as between parts of the input; this method will not
    /// collapse multiple spaces (U+0020) into a single space. If such
    /// space collapsing on a string (or rejection of strings with multiple
    /// consecutive spaces) is desired, it should be done before that
    /// string is passed to this method. For comparison purposes, return
    /// values of this method (with the same value for <paramref
    /// name='preserveCase'/> ) should be compared code point by code point
    /// (see RFC 8265, secs. 3.3.4 and 3.4.4).</returns>
    public static string UsernameEnforce(string str, bool preserveCase) {
      if (String.IsNullOrEmpty(str)) {
 return null;
}
      if (str[0] == ' ' || str[str.Length - 1] == ' ') {
 return null;
}
      var lastPos = 0;
      var i = 0;
      StringBuilder sb = null;
      while (i <= str.Length) {
        if (str[i] == ' ') {
          string part = UserpartEnforce(
            str.Substring(lastPos, i - lastPos),
            preserveCase);
          if (part == null) {
 return null;
}
          sb = sb ?? (new StringBuilder());
          sb.Append(part);
          sb.Append(' ');
          while (i < str.Length) {
            if (str[i] != ' ') {
 break;
}
            sb.Append(' ');
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
      if (lastPos != str.Length) {
        string part = UserpartEnforce(
          str.Substring(lastPos, str.Length - lastPos),
          preserveCase);
        if (part == null) {
 return null;
}
        sb = sb ?? (new StringBuilder());
        sb.Append(part);
      }
      return sb.ToString();
    }

    /// <summary>Checks the validity of a string serving as an arbitrary
    /// sequence of characters, such as a passphrase. This checking is done
    /// using the OpaqueString profile in RFC 8265. (REMARK: Specifying a
    /// string as this method does is not ideal if the string represents a
    /// password or other sensitive data, since strings are immutable in
    /// .NET and Java, so that its contents cannot be cleared when done. An
    /// application concerned about security may want to reimplement this
    /// method by passing a clearable array of characters rather than a
    /// text string.).</summary>
    /// <param name='str'>A string to prepare that represents an arbitrary
    /// sequence of characters entered by a user.</param>
    /// <returns>An opaque string prepared under the OpaqueString profile
    /// in RFC 8265. Returns null if that string is invalid under that
    /// profile (including if <paramref name='str'/> is null or empty). For
    /// comparison purposes, return values of this method should be
    /// compared code point by code point (see RFC 8265, sec.
    /// 4.2.3).</returns>
    public static string OpaqueStringEnforce(string str) {
      return Idna.OpaqueStringEnforce(str);
    }

    /// <summary>Checks the validity of a string serving as a "memorable,
    /// human-friendly name" for something (see RFC 8266), as opposed to
    /// that thing's identity for authentication or authorization purposes
    /// (see sec. 6.1 of that RFC). This checking is done using the
    /// Nickname profile in RFC 8266.</summary>
    /// <param name='str'>A string serving as a nickname for
    /// something.</param>
    /// <returns>A nickname prepared for enforcement under the Nickname
    /// profile in RFC 8266. Returns null if that string is invalid under
    /// that profile (including if <paramref name='str'/> is null or
    /// empty). Return values of this method should be used for comparison
    /// purposes (see RFC 8266, sec. 2.3).</returns>
    public static string NicknameEnforce(string str) {
      return Idna.NicknameEnforce(str);
    }

    /// <summary>Prepares for comparison a string serving as a "memorable,
    /// human-friendly name" for something (see RFC 8266), as opposed to
    /// that thing's identity for authentication or authorization purposes
    /// (see sec. 6.1 of that RFC). This operation is done using the
    /// Nickname profile in RFC 8266.</summary>
    /// <param name='str'>A string serving as a nickname for
    /// something.</param>
    /// <returns>A nickname prepared for comparison under the Nickname
    /// profile in RFC 8266. Returns null if that string is invalid under
    /// that profile (including if <paramref name='str'/> is null or
    /// empty). For comparison purposes, return values of this method
    /// should be compared code point by code point (see RFC 8266, sec.
    /// 2.4).</returns>
    public static string NicknameForComparison(string str) {
      return Idna.NicknameForComparison(str);
    }
  }
}
