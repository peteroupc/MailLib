/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Text;

namespace PeterO.Text {
  /// <summary>
  /// <para>Contains methods that implement Internationalized Domain
  /// Names in Applications (IDNA). IDNA enables using a wider range of
  /// letters, numbers, and certain other characters in domain names.
  /// This class implements the 2008 revision of IDNA, also known as
  /// IDNA2008.</para>
  /// <para>The following summarizes the rules for domain names in
  /// IDNA2008; see RFC5890 for more information and additional
  /// terminology.</para>
  /// <para>A domain name is divided into one or more strings separated
  /// by dots ("."), called <b>labels</b>. For IDNA2008's purposes, a
  /// valid label can be an <b>NR-LDH label</b>, an <b>A-label</b>, or
  /// a <b>U-label</b>.</para>
  /// <para>An LDH label contains only basic uppercase letters, basic
  /// lowercase letters, basic digits, and/or "-", and neither begins nor
  /// ends with "-". For example, "exa-mple", "EXAMPLE", and "1example"
  /// are LDH labels, but not "-example".</para>
  /// <para>An NR-LDH label is an LDH label whose third and fourth
  /// characters are not both "-". For example, "ex--ample" is not an
  /// NR-LDH label.</para>
  /// <para>A U-label contains one or more characters outside the Basic
  /// Latin range (U+0000 to U+007F) and meets IDNA2008 requirements for
  /// labels with such characters. An example is "eá".</para>
  /// <para>An A-label is an LDH label beginning with "xn--" where the
  /// letters can be any combination of basic uppercase and basic
  /// lowercase letters, and is convertible to a U-label. An example is
  /// "xn--e-ufa".</para>
  /// <para>An XN-label is an LDH label beginning with "xn--" where the
  /// letters can be any combination of basic uppercase and basic
  /// lowercase letters.</para>
  /// <para>NOTICE: While this class's source code is in the public
  /// domain, the class uses two internal classes, called
  /// <c>NormalizationData</c> and <c>IdnaData</c>, that include data
  /// derived from the Unicode Character Database. See the documentation
  /// for the NormalizerInput class for the permission notice for the
  /// Unicode Character Database.</para></summary>
  public static class Idna {
    private const int Unassigned = 0;
    // PValid = 1;
    private const int Disallowed = 2;
    private const int ContextJ = 3;
    private const int ContextO = 4;
    private const int IDDisallowed = 5;
    private const int BidiClassL = 0;
    private const int BidiClassR = 1;
    private const int BidiClassAL = 2;
    private const int BidiClassEN = 3;
    private const int BidiClassES = 4;
    private const int BidiClassET = 5;
    private const int BidiClassAN = 6;
    private const int BidiClassCS = 7;
    private const int BidiClassNSM = 8;
    private const int BidiClassBN = 9;
    private const int BidiClassON = 10;

    private static volatile object syncRoot = new Object();
    private static volatile ByteData bidiClasses;
    private static volatile ByteData joiningTypes;
    private static volatile ByteData scripts;
    private static volatile ByteData valueZsChars;

    internal static int CodePointBefore(string str, int index) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index <= 0) {
        return -1;
      }
      if (index > str.Length) {
        return -1;
      }
      int c = str[index - 1];
      if ((c & 0xfc00) == 0xdc00 && index - 2 >= 0 &&
        (str[index - 2] & 0xfc00) == 0xd800) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((str[index - 2] & 0x3ff) << 10) + (c & 0x3ff);
      }
      return ((c & 0xf800) == 0xd800) ? 0xfffd : c;
    }

    internal static int CodePointAt(string str, int index) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index >= str.Length) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str[index];
      if ((c & 0xfc00) == 0xd800 && index + 1 < str.Length &&
        (str[index + 1] & 0xfc00) == 0xdc00) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((c & 0x3ff) << 10) + (str[index + 1] & 0x3ff);
      }
      return ((c & 0xf800) == 0xd800) ? 0xfffd : c;
    }

    internal static int GetBidiClass(int ch) {
      ByteData table = null;
      if (bidiClasses == null) {
        lock (syncRoot) {
          bidiClasses = bidiClasses ??
            ByteData.Decompress(IdnaData.BidiClasses);
        }
      }
      table = bidiClasses;
      return table.GetByte(ch);
    }

    private static int GetJoiningType(int ch) {
      ByteData table = null;
      if (joiningTypes == null) {
        lock (syncRoot) {
          joiningTypes = joiningTypes ?? ByteData.Decompress(
              IdnaData.JoiningTypes);
        }
      }
      table = joiningTypes;
      return table.GetByte(ch);
    }

    private static bool IsZsCodePoint(int ch) {
      ByteData table = null;
      if (valueZsChars == null) {
        lock (syncRoot) {
          valueZsChars = valueZsChars ?? ByteData.Decompress(
              IdnaData.ZsCharacters);
        }
      }
      table = valueZsChars;
      return table.GetBoolean(ch);
    }

    private static int GetScript(int ch) {
      ByteData table = null;
      if (scripts == null) {
        lock (syncRoot) {
          scripts = scripts ?? ByteData.Decompress(
              IdnaData.IdnaRelevantScripts);
        }
      }
      table = scripts;
      return table.GetByte(ch);
    }

    private static bool JoiningTypeTransparent(int ch) {
      return GetJoiningType(ch) == 1;
    }

    private static bool JoiningTypeLeftOrDual(int ch) {
      int jtype = GetJoiningType(ch);
      return jtype == 3 || jtype == 4;
    }

    private static bool JoiningTypeRightOrDual(int ch) {
      int jtype = GetJoiningType(ch);
      return jtype == 2 || jtype == 4;
    }

    private static bool IsGreek(int ch) {
      return GetScript(ch) == 1;
    }

    private static bool IsHebrew(int ch) {
      return GetScript(ch) == 2;
    }

    private static bool IsKanaOrHan(int ch) {
      return GetScript(ch) == 3;
    }

    private static bool IsCaseIgnorable(int ch) {
      return UnicodeDatabase.GetCasedProperty(ch) == 2;
    }

    private static bool IsCased(int ch) {
      return UnicodeDatabase.GetCasedProperty(ch) == 1;
    }

    private static bool IsFinalSigmaContext(string str, int index) {
      // Assumes that the character at the specified index
      // is Capital Sigma
      // Check the left
      var found = false;
      int oldIndex = index;
      while (index > 0) {
        int ch = CodePointBefore(str, index);
        index -= (ch >= 0x10000) ? 2 : 1;
        if (IsCased(ch)) {
          found = true;
        } else if (!IsCaseIgnorable(ch)) {
          return false;
        }
      }
      if (!found) {
        return false;
      }
      // Check the right
      index = oldIndex + 1;
      while (index < str.Length) {
        int ch = CodePointAt(str, index);
        index += (ch >= 0x10000) ? 2 : 1;
        if (IsCased(ch)) {
          return false;
        }
        if (!IsCaseIgnorable(ch)) {
          return true;
        }
      }
      return true;
    }

    private static bool IsValidConjunct(string str, int index) {
      // Assumes that the character at the specified index
      // is Zero-Width Non-Joiner
      // Check the left
      var found = false;
      int oldIndex = index;
      while (index > 0) {
        int ch = CodePointBefore(str, index);
        index -= (ch >= 0x10000) ? 2 : 1;
        if (JoiningTypeLeftOrDual(ch)) {
          found = true;
        } else if (!JoiningTypeTransparent(ch)) {
          return false;
        }
      }
      if (!found) {
        return false;
      }
      // Check the right
      index = oldIndex + 1;
      while (index < str.Length) {
        int ch = CodePointAt(str, index);
        index += (ch >= 0x10000) ? 2 : 1;
        if (JoiningTypeRightOrDual(ch)) {
          return true;
        }
        if (!JoiningTypeTransparent(ch)) {
          return false;
        }
      }
      return false;
    }

    private static string ToLowerCase(string str) {
      var buffer = new int[2];
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; ++i) {
        int ch = CodePointAt(str, i);
        if (ch < 0) {
          return str;
        }
        if (ch == 931) {
          sb.Append(IsFinalSigmaContext(str, i) ? (char)962 : (char)963);
        } else {
          int size = UnicodeDatabase.GetLowerCaseMapping(ch, buffer, 0);
          for (var j = 0; j < size; ++j) {
            int c2 = buffer[j];
            if (c2 <= 0xffff) {
              sb.Append((char)c2);
            } else if (ch <= 0x10ffff) {
              sb.Append((char)((((c2 - 0x10000) >> 10) & 0x3ff) | 0xd800));
              sb.Append((char)(((c2 - 0x10000) & 0x3ff) | 0xdc00));
            }
          }
        }
        if (ch >= 0x10000) {
          ++i;
        }
      }
      return sb.ToString();
    }

    private static bool HasRtlCharacters(string str) {
      for (int i = 0; i < str.Length; ++i) {
        if (str[i] >= 0x80) {
          int c = CodePointAt(str, i);
          if (c >= 0x10000) {
            ++i;
          }
          int bidiClass = GetBidiClass(c);
          if (bidiClass == BidiClassAL || bidiClass == BidiClassAN ||
            bidiClass == BidiClassR) {
            return true;
          }
        }
      }
      return false;
    }

    private static string DecodeLabel(string str, int index, int endIndex) {
      if (endIndex - index > 4 && str[index] == 'x' &&
        str[index + 1] == 'n' && str[index + 2] == '-' &&
        str[index + 3] == '-') {
        return DomainUtility.PunycodeDecode(str, index + 4, endIndex);
      } else {
        return str.Substring(index, endIndex - index);
      }
    }

    /// <summary>Tries to encode each XN-label of the specified domain name
    /// into Unicode. This method does not check the syntactic validity of
    /// the domain name before proceeding.</summary>
    /// <param name='value'>A domain name.</param>
    /// <returns>The domain name where each XN-label is encoded into
    /// Unicode. Labels where this is not possible remain
    /// unchanged.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='value'/> is null.</exception>
    public static string DecodeDomainName(string value) {
      if (value == null) {
        throw new ArgumentNullException(nameof(value));
      }
      if (value.Length == 0) {
        return String.Empty;
      }
      var builder = new StringBuilder();
      string retval = null;
      var lastIndex = 0;
      for (int i = 0; i < value.Length; ++i) {
        char c = value[i];
        if (c == '.') {
          if (i != lastIndex) {
            retval = DecodeLabel(value, lastIndex, i);
            if (retval == null) {
              // Append the unmodified domain plus the dot
              builder.Append(value.Substring(lastIndex, (i + 1) - lastIndex));
            } else {
              builder.Append(retval);
              builder.Append('.');
            }
          }
          lastIndex = i + 1;
        }
      }
      retval = DecodeLabel(
          value,
          lastIndex,
          value.Length);
      if (retval == null) {
        builder.Append(value.Substring(lastIndex, value.Length - lastIndex));
      } else {
        builder.Append(retval);
      }
      return builder.ToString();
    }

    /// <summary>Tries to encode each label of a domain name with code
    /// points outside the Basic Latin range (U+0000 to U+007F) into an
    /// XN-label. This method does not check the syntactic validity of the
    /// domain name before proceeding.</summary>
    /// <param name='value'>A domain name.</param>
    /// <returns>The domain name where each label with code points outside
    /// the Basic Latin range (U+0000 to U+007F) is encoded into an
    /// XN-label. Labels where this is not possible remain
    /// unchanged.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='value'/> is null.</exception>
    public static string EncodeDomainName(string value) {
      if (value == null) {
        throw new ArgumentNullException(nameof(value));
      }
      if (value.Length == 0) {
        return String.Empty;
      }
      var builder = new StringBuilder();
      string retval = null;
      var lastIndex = 0;
      for (int i = 0; i < value.Length; ++i) {
        char c = value[i];
        if (c == '.') {
          if (i != lastIndex) {
            retval = DomainUtility.ALabelEncodePortion(value, lastIndex, i);
            if (retval == null) {
              // Append the unmodified domain plus the dot
              builder.Append(value.Substring(lastIndex, (i + 1) - lastIndex));
            } else {
              builder.Append(retval);
              builder.Append('.');
            }
          }
          lastIndex = i + 1;
        }
      }
      retval = DomainUtility.ALabelEncodePortion(
          value,
          lastIndex,
          value.Length);
      if (retval == null) {
        builder.Append(value.Substring(lastIndex, value.Length - lastIndex));
      } else {
        builder.Append(retval);
      }
      return builder.ToString();
    }

    /// <summary>Determines whether the specified string is a domain name
    /// containing only U-labels, A-labels, NR-LDH labels, or any
    /// combination of these, separated by dots (".").</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='lookupRules'>If true, uses rules to apply when looking
    /// up the string as a domain name. If false, uses rules to apply when
    /// registering the string as a domain name.</param>
    /// <returns><c>true</c> if the specified string is a syntactically
    /// valid domain name; otherwise; false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static bool IsValidDomainName(string str, bool lookupRules) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      bool bidiRule = HasRtlCharacters(str);
      var lastIndex = 0;
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c == '.') {
          if (i == lastIndex) {
            // Empty label
            return false;
          }
          if (!IsValidLabel(
            str.Substring(lastIndex, i - lastIndex),
            lookupRules,
            bidiRule)) {
            return false;
          }
          lastIndex = i + 1;
        }
      }
      return (str.Length != lastIndex) && IsValidLabel(
        str.Substring(lastIndex, str.Length - lastIndex),
        lookupRules,
        bidiRule);
    }

    private static string ToLowerCaseAscii(string str) {
      if (str == null) {
        return null;
      }
      int len = str.Length;
      var c = (char)0;
      var hasUpperCase = false;
      for (int i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'A' && c <= 'Z') {
          hasUpperCase = true;
          break;
        }
      }
      if (!hasUpperCase) {
        return str;
      }
      var builder = new StringBuilder();
      for (int i = 0; i < len; ++i) {
        c = str[i];
        if (c >= 'A' && c <= 'Z') {
          builder.Append((char)(c + 0x20));
        } else {
          builder.Append(c);
        }
      }
      return builder.ToString();
    }

    private static bool IsValidLabel(
      string str,
      bool lookupRules,
      bool bidiRule) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      bool maybeALabel = str.Length > 4 && (str[0] == 'x' || str[0] == 'X') &&
        (str[1] == 'n' || str[1] == 'N') && str[2] == '-' && str[3] == '-';
      var allLDH = true;
      for (int i = 0; i < str.Length; ++i) {
        if ((str[i] >= 'a' && str[i] <= 'z') ||
          (str[i] >= 'A' && str[i] <= 'Z') ||
          (str[i] >= '0' && str[i] <= '9') || str[i] == '-') {
          // LDH character
          continue;
        }
        if (str[i] >= 0x80) {
          // Non-ASCII character
          allLDH = false;
          continue;
        }
        return false;
      }
      if (maybeALabel) {
        str = ToLowerCaseAscii(str);
        string ustr = DomainUtility.PunycodeDecode(str, 4, str.Length);
        if (ustr == null) {
          // NOTE: Returns null if "str" contains non-ASCII characters
          return false;
        }
        if (!IsValidULabel(ustr, lookupRules, bidiRule)) {
          return false;
        }
        string astr = DomainUtility.ALabelEncodePortion(ustr, 0, ustr.Length);
        // NOTE: "astr" and "str" will contain only ASCII characters
        // at this point, so a simple null check and
        // binary comparison are enough
        return (astr != null) && astr.Equals(str, StringComparison.Ordinal);
      }
      if (allLDH) {
        if (bidiRule && str[0] >= '0' && str[0] <= '9') {
          // First character is a digit and the Bidi rule applies
          return false;
        }
        if (str.Length >= 4 && str[2] == '-' && str[3] == '-') {
          // Contains a hyphen at the third and fourth (one-based) character
          // positions
          return false;
        }
        if (str[0] != '-' && str[str.Length - 1] != '-') {
          // Only LDH characters, doesn't start with hyphen,
          // and doesn't end with hyphen
          return true;
        }
      }
      return IsValidULabel(str, lookupRules, bidiRule);
    }

    private static bool PassesContextChecks(string str) {
      var regArabDigits = false;
      var extArabDigits = false;
      var haveKatakanaMiddleDot = false;
      var haveKanaOrHan = false;
      var lastChar = 0;
      for (int i = 0; i < str.Length; ++i) {
        int thisChar = CodePointAt(str, i);
        if (thisChar >= 0x660 && thisChar <= 0x669) {
          // Arabic-Indic digits
          // NOTE: Test done here even under lookup rules,
          // even though they're CONTEXTO characters (performing
          // CONTEXTO checks is optional in lookup under RFC 5891, sec. 5.4).
          if (extArabDigits) {
            return false;
          }
          regArabDigits = true;
        } else if (thisChar >= 0x6f0 && thisChar <= 0x6f9) {
          // Extended Arabic-Indic digits
          // NOTE: Test done here even under lookup rules,
          // even though they're CONTEXTO characters (performing
          // CONTEXTO checks is optional in lookup under RFC 5891, sec. 5.4).
          if (regArabDigits) {
            return false;
          }
          extArabDigits = true;
        } else if (thisChar == 0xb7) {
          // Middle dot
          // NOTE: Test done here even under lookup rules,
          // even though it's a CONTEXTO character (performing
          // CONTEXTO checks is optional in lookup under RFC 5891, sec. 5.4).
          if (!(i - 1 >= 0 && i + 1 < str.Length &&
            lastChar == 0x6c && str[i + 1] == 0x6c)) {
            // Dot must come between two l's
            return false;
          }
        } else if (thisChar == 0x200d) {
          // Zero-width joiner
          if (UnicodeDatabase.GetCombiningClass(lastChar) != 9) {
            return false;
          }
        } else if (thisChar == 0x200c) {
          // Zero-width nonjoiner
          if (UnicodeDatabase.GetCombiningClass(lastChar) != 9 &&
            !IsValidConjunct(str, i)) {
            return false;
          }
        } else if (thisChar == 0x375) {
          // Keraia
          // NOTE: Test done here even under lookup rules,
          // even though it's a CONTEXTO character (performing
          // CONTEXTO checks is optional in lookup under RFC 5891, sec. 5.4).
          if (i + 1 >= str.Length || !IsGreek(CodePointAt(str, i + 1))) {
            return false;
          }
        } else if (thisChar == 0x5f3 || thisChar == 0x5f4) {
          // Geresh or gershayim
          // NOTE: Test done here even under lookup rules,
          // even though they're CONTEXTO characters (performing
          // CONTEXTO checks is optional in lookup under RFC 5891, sec. 5.4).
          if (i <= 0 || !IsHebrew(lastChar)) {
            return false;
          }
        } else if (thisChar == 0x30fb) {
          haveKatakanaMiddleDot = true;
        } else {
          int category = UnicodeDatabase.GetIdnaCategory(thisChar);
          if (category == ContextJ || category == ContextO) {
            // Context character without a rule
            return false;
          }
        }
        if (!haveKanaOrHan && IsKanaOrHan(thisChar)) {
          haveKanaOrHan = true;
        }
        if (thisChar >= 0x10000) {
          ++i;
        }
        lastChar = thisChar;
      }
      if (haveKatakanaMiddleDot && !haveKanaOrHan) {
        // NOTE: Test done here even under lookup rules,
        // even though it's a CONTEXTO character (CONTEXTO
        // checks are optional in lookup under RFC 5891, sec. 5.4).
        return false;
      }
      return true;
    }

    internal static bool IsInIdentifierClass(string str) {
      return IsInPrecisClass(str, false);
    }

    internal static bool IsInFreeformClass(string str) {
      return IsInPrecisClass(str, true);
    }

    private static string UsernameCasePreservedEnforceInternal(string str) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      str = WidthMapping(str);
      if (IsInPrecisClass(str, false)) {
        str = NormalizerInput.Normalize(str, Normalization.NFC);
        return (HasRtlCharacters(str) && !PassesBidiRule(str)) ? null :
          (str.Length == 0 ? null : str);
      }
      return null;
    }

    private static string NicknameInternal(string str, bool forComparison) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      if (IsInPrecisClass(str, true)) {
        str = TrimAndCollapseUnicodeSpaces(str);
        if (forComparison) {
          str = ToLowerCase(str);
        }
        str = NormalizerInput.Normalize(str, Normalization.NFKC);
        return str.Length == 0 ? null : str;
      }
      return null;
    }

    internal static string NicknameEnforce(string str) {
      string oldvalue = str;
      for (var i = 0; i < 4; ++i) {
        string newvalue = NicknameInternal(oldvalue, false);
        if (newvalue == null) {
          return null;
        }
        if (oldvalue.Equals(newvalue, StringComparison.Ordinal)) {
          return oldvalue;
        }
        oldvalue = newvalue;
      }
      return null;
    }

    internal static string NicknameForComparison(string str) {
      string oldvalue = str;
      for (var i = 0; i < 4; ++i) {
        string newvalue = NicknameInternal(oldvalue, true);
        if (newvalue == null) {
          return null;
        }
        if (oldvalue.Equals(newvalue, StringComparison.Ordinal)) {
          return oldvalue;
        }
        oldvalue = newvalue;
      }
      return null;
    }

    internal static string UsernameCasePreservedEnforce(string str) {
      string oldvalue = str;
      for (var i = 0; i < 4; ++i) {
        string newvalue = UsernameCasePreservedEnforceInternal(oldvalue);
        if (newvalue == null) {
          return null;
        }
        if (oldvalue.Equals(newvalue, StringComparison.Ordinal)) {
          return oldvalue;
        }
        oldvalue = newvalue;
      }
      return null;
    }

    private static string UsernameCaseMappedEnforceInternal(string str) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      str = WidthMapping(str);
      if (IsInPrecisClass(str, false)) {
        str = ToLowerCase(str);
        str = NormalizerInput.Normalize(str, Normalization.NFC);
        return (HasRtlCharacters(str) && !PassesBidiRule(str)) ? null :
          (str.Length == 0 ? null : str);
      }
      return null;
    }

    internal static string UsernameCaseMappedEnforce(string str) {
      string oldvalue = str;
      for (var i = 0; i < 4; ++i) {
        string newvalue = UsernameCaseMappedEnforceInternal(oldvalue);
        if (newvalue == null) {
          return null;
        }
        if (oldvalue.Equals(newvalue, StringComparison.Ordinal)) {
          return oldvalue;
        }
        oldvalue = newvalue;
      }
      return null;
    }

    private static string OpaqueStringEnforceInternal(string str) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      if (IsInPrecisClass(str, true)) {
        str = SpaceMapping(str);
        str = NormalizerInput.Normalize(str, Normalization.NFC);
        return str.Length == 0 ? null : str;
      }
      return null;
    }

    internal static string OpaqueStringEnforce(string str) {
      string oldvalue = str;
      for (var i = 0; i < 4; ++i) {
        string newvalue = OpaqueStringEnforceInternal(oldvalue);
        if (newvalue == null) {
          return null;
        }
        if (oldvalue.Equals(newvalue, StringComparison.Ordinal)) {
          return oldvalue;
        }
        oldvalue = newvalue;
      }
      return null;
    }

    private static bool IsInPrecisClass(string str, bool freeform) {
      if (String.IsNullOrEmpty(str)) {
        return str != null;
      }
      var haveContextual = false;
      for (int i = 0; i < str.Length; ++i) {
        int ch = CodePointAt(str, i);
        if (ch >= 0x10000) {
          ++i;
        }
        if (ch < 0) {
          return false;
        }
        int category = UnicodeDatabase.GetPrecisCategory(ch);
        if (category == Disallowed || category == Unassigned) {
          return false;
        }
        if (category == IDDisallowed && !freeform) {
          // Disallowed in identifiers only
          return false;
        }
        haveContextual |= category == ContextO || category == ContextJ;
      }
      if (haveContextual) {
        if (!PassesContextChecks(str)) {
          return false;
        }
      }
      return true;
    }

    private static bool PassesBidiRule(string str) {
      if (String.IsNullOrEmpty(str)) {
        return true;
      }
      int bidiClass;
      var rtl = false;
      int ch = CodePointAt(str, 0);
      if (ch < 0) {
        return false;
      }
      int bidi = GetBidiClass(ch);
      if (bidi == BidiClassR || bidi == BidiClassAL) {
        rtl = true;
      } else if (bidi != BidiClassL) {
        return false;
      }
      var found = false;
      for (int i = str.Length; i > 0; --i) {
        int c = CodePointBefore(str, i);
        if (c < 0) {
          return false;
        }
        if (c >= 0x10000) {
          --i;
        }
        bidiClass = GetBidiClass(c);
        if (rtl && (bidiClass == BidiClassR || bidiClass == BidiClassAL ||
          bidiClass == BidiClassAN)) {
          found = true;
          break;
        }
        if (!rtl && (bidiClass == BidiClassL)) {
          found = true;
          break;
        }
        if (bidiClass == BidiClassEN) {
          found = true;
          break;
        }
        if (bidiClass != BidiClassNSM) {
          return false;
        }
      }
      if (!found) {
        return false;
      }
      var haveEN = false;
      var haveAN = false;
      for (int i = 0; i < str.Length; ++i) {
        int c = CodePointAt(str, i);
        if (c >= 0x10000) {
          ++i;
        }
        bidiClass = GetBidiClass(c);
        if (rtl && (bidiClass == BidiClassR || bidiClass == BidiClassAL ||
          bidiClass == BidiClassAN)) {
          if (bidiClass == BidiClassAN) {
            if (haveEN) {
              return false;
            }
            haveAN = true;
          }
          continue;
        }
        if (!rtl && (bidiClass == BidiClassL)) {
          continue;
        }
        if (bidiClass == BidiClassEN) {
          if (rtl) {
            if (haveAN) {
              return false;
            }
            haveEN = false;
          }
          continue;
        }
        if (bidiClass == BidiClassES ||
          bidiClass == BidiClassCS || bidiClass == BidiClassET ||
          bidiClass == BidiClassON || bidiClass == BidiClassBN ||
          bidiClass == BidiClassNSM) {
          continue;
        }
        return false;
      }
      return true;
    }

    private static string WidthMapping(string str) {
      var index = 0;
      var i = 0;
      for (i = 0; i < str.Length; ++i) {
        int ch = CodePointAt(str, i);
        // Contains an unpaired surrogate, so bail out
        if (ch < 0) {
          return str;
        }
        if (ch >= 0x10000) {
          ++i;
        }
        // NOTE: Not coextensive with code points having
        // Decomposition_Type = Wide or Narrow, since U+3000,
        // ideographic space, is excluded. However, this
        // code point (as well as its decomposition mapping,
        // which is U+0020) will be excluded by the
        // IdentifierClass.
        if (UnicodeDatabase.IsFullOrHalfWidth(ch)) {
          break;
        }
        index = i;
      }
      if (index == str.Length) {
        return str;
      }
      var sb = new StringBuilder();
      sb.Append(str.Substring(0, index));
      for (i = index; i < str.Length; ++i) {
        int ch = CodePointAt(str, i);
        int istart = i;
        // Contains an unpaired surrogate, so bail out
        if (ch < 0) {
          return str;
        }
        if (ch >= 0x10000) {
          ++i;
        }
        if (UnicodeDatabase.IsFullOrHalfWidth(ch)) {
          string chs = str.Substring(istart, (i - istart) + 1);
          string nfkd = NormalizerInput.Normalize(
              chs,
              Normalization.NFKD);
          sb.Append(nfkd);
        } else {
          if (ch <= 0xffff) {
            {
              sb.Append((char)ch);
            }
          } else if (ch <= 0x10ffff) {
            sb.Append((char)((((ch - 0x10000) >> 10) & 0x3ff) | 0xd800));
            sb.Append((char)(((ch - 0x10000) & 0x3ff) | 0xdc00));
          }
        }
      }
      return sb.ToString();
    }

    private static string TrimAndCollapseUnicodeSpaces(string str) {
      if (String.IsNullOrEmpty(str)) {
        return str;
      }
      StringBuilder builder = null;
      var index = 0;
      int leadIndex;
      // Skip leading whitespace, if any
      while (index < str.Length) {
        char c = str[index];
        if (c == 0x20 || IsZsCodePoint(c)) {
          builder = builder ?? new StringBuilder();
          ++index;
        } else {
          break;
        }
      }
      leadIndex = index;
      while (index < str.Length) {
        int si = index;
        char c = str[index++];
        var count = 0;
        while (c == 0x20 || IsZsCodePoint(c)) {
          ++count;
          if (index < str.Length) {
            c = str[index++];
          } else {
            break;
          }
        }
        if (count > 0) {
          if (builder == null) {
            builder = new StringBuilder();
            builder.Append(str.Substring(leadIndex, si));
          }
          if (c != 0x20 && !IsZsCodePoint(c)) {
            builder.Append(' ');
            builder.Append(c);
          }
        } else {
          if (builder != null) {
            builder.Append(c);
          }
        }
      }
      return (builder == null) ? str : builder.ToString();
    }

    private static string SpaceMapping(string str) {
      var index = 0;
      var i = 0;
      for (i = 0; i < str.Length; ++i) {
        int ch = CodePointAt(str, i);
        // Contains an unpaired surrogate, so bail out
        if (ch < 0) {
          return str;
        }
        if (ch >= 0x10000) {
          ++i;
        }
        if (ch != 0x20 && IsZsCodePoint(ch)) {
          break;
        }
        index = i;
      }
      if (index == str.Length) {
        return str;
      }
      var sb = new StringBuilder();
      sb.Append(str.Substring(0, index));
      for (i = index; i < str.Length; ++i) {
        int ch = CodePointAt(str, i);
        int istart = i;
        // Contains an unpaired surrogate, so bail out
        if (ch < 0) {
          return str;
        }
        if (ch >= 0x10000) {
          ++i;
        }
        if (ch < 0x80 || !IsZsCodePoint(ch)) {
          if (ch <= 0xffff) {
            sb.Append((char)ch);
          } else if (ch <= 0x10ffff) {
            sb.Append((char)((((ch - 0x10000) >> 10) & 0x3ff) | 0xd800));
            sb.Append((char)(((ch - 0x10000) & 0x3ff) | 0xdc00));
          }
        } else {
          sb.Append(' ');
        }
      }
      return sb.ToString();
    }

    private static bool IsValidULabel(
      string str,
      bool lookupRules,
      bool bidiRule) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      if (str.Length > 63 && !lookupRules) {
        // Too long
        return false;
      }
      if (str.Length >= 4 && str[2] == '-' && str[3] == '-') {
        // Contains a hyphen at the third and fourth (one-based) character
        // positions
        return false;
      }
      if (!lookupRules) {
        // Checking for a hyphen at the start and
        // the end is part of the registration validity
        // rules (sec. 4.2 of RFC 5891), but not the lookup
        // rules (sec. 5.4).
        if (str[0] == '-' || str[str.Length - 1] == '-') {
          return false;
        }
      }
      if (!NormalizerInput.IsNormalized(str, Normalization.NFC)) {
        return false;
      }
      int ch;
      var first = true;
      var haveContextual = false;
      var nonascii = false;
      for (int i = 0; i < str.Length; ++i) {
        ch = CodePointAt(str, i);
        if (ch >= 0x10000) {
          ++i;
        }
        if (ch >= 0x80) {
          nonascii = true;
        }
        int category = UnicodeDatabase.GetIdnaCategory(ch);
        if (category == Disallowed || category == Unassigned) {
          return false;
        }
        if (first) {
          if (UnicodeDatabase.IsCombiningMark(ch)) {
            return false;
          }
        }
        haveContextual |= category == ContextO || category == ContextJ;
        first = false;
      }
      if (!nonascii) {
        return false;
      }
      if (haveContextual) {
        if (!PassesContextChecks(str)) {
          return false;
        }
      }
      if (bidiRule) {
        if (!PassesBidiRule(str)) {
          return false;
        }
      }
      int aceLength = DomainUtility.ALabelLength(str, 0, str.Length);
      if (aceLength < 0) {
        return false; // Overflow error
      }
      if (!lookupRules) {
        // Additional rules for nonlookup validation
        if (aceLength > 63) {
          return false;
        }
      }
      return true;
    }
  }
}
