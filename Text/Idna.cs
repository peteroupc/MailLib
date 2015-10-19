/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;

namespace PeterO.Text {
    /// <summary><para>Contains methods that implement Internationalized Domain
    /// Names in Applications (IDNA). IDNA enables using a wider range of letters,
    /// numbers, and certain other characters in domain names.</para>
    /// <para>NOTICE:
    /// While this class's source code is in the public domain, the class uses two
    /// internal classes, called <c>NormalizationData</c>
    /// and <c>IdnaData</c>
    /// , that
    /// include data derived from the Unicode Character Database. See the
    /// documentation for the Normalizer class for the permission notice for the
    /// Unicode Character Database.</para>
    /// </summary>
  public static class Idna {
    private const int Unassigned = 0;
    // PValid = 1;
    private const int Disallowed = 2;
    private const int ContextJ = 3;
    private const int ContextO = 4;
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

    private static ByteData bidiClasses;
    private static ByteData joiningTypes;
    private static ByteData scripts;
    private static object bidiClassesSync = new Object();
    private static object joiningTypesSync = new Object();
    private static object scriptsSync = new Object();

    internal static int CodePointBefore(string str, int index) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index <= 0) {
        return -1;
      }
      if (index > str.Length) {
        return -1;
      }
      int c = str[index - 1];
      if ((c & 0xfc00) == 0xdc00 && index - 2 >= 0 &&
             str[index - 2] >= 0xd800 && str[index - 2] <= 0xdbff) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((str[index - 2] - 0xd800) << 10) + (c - 0xdc00);
      }
      return ((c & 0xf800) == 0xd800) ? 0xfffd : c;
    }

    internal static int CodePointAt(string str, int index) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index >= str.Length) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str[index];
      if ((c & 0xfc00) == 0xd800 && index + 1 < str.Length &&
          str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((c - 0xd800) << 10) + (str[index + 1] - 0xdc00);
      }
      return ((c & 0xf800) == 0xd800) ? 0xfffd : c;
    }

    internal static int GetBidiClass(int ch) {
      ByteData table = null;
      lock (bidiClassesSync) {
        bidiClasses = bidiClasses ?? ByteData.Decompress(IdnaData.BidiClasses);
        table = bidiClasses;
      }
      return table.GetByte(ch);
    }

    private static int GetJoiningType(int ch) {
      ByteData table = null;
      lock (joiningTypesSync) {
     joiningTypes = joiningTypes ?? ByteData.Decompress(IdnaData.JoiningTypes);
        table = joiningTypes;
      }
      return table.GetByte(ch);
    }

    private static int GetScript(int ch) {
      ByteData table = null;
      lock (scriptsSync) {
        scripts = scripts ?? ByteData.Decompress(IdnaData.IdnaRelevantScripts);
        table = scripts;
      }
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

    private static bool IsValidConjunct(string str, int index) {
      // Assumes that the character at the given index
      // is Zero-Width Non-Joiner
      // Check the left
      bool found = false;
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

    /// <summary>Tries to encode each label of a domain name into
    /// Punycode.</summary>
    /// <param name='value'>A domain name.</param>
    /// <returns>The domain name where each label with non-ASCII characters is
    /// encoded into Punycode. Labels where this is not possible remain
    /// unchanged.</returns>
    /// <exception cref='ArgumentNullException'>Value is null.</exception>
    public static string EncodeDomainName(string value) {
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      if (value.Length == 0) {
        return String.Empty;
      }
      var builder = new StringBuilder();
      string retval = null;
      int lastIndex = 0;
      for (int i = 0; i < value.Length; ++i) {
        char c = value[i];
        if (c == '.') {
          if (i != lastIndex) {
            retval = DomainUtility.PunycodeEncodePortion(value, lastIndex, i);
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
  retval = DomainUtility.PunycodeEncodePortion(value, lastIndex,
        value.Length);
      if (retval == null) {
        builder.Append(value.Substring(lastIndex, value.Length - lastIndex));
      } else {
        builder.Append(retval);
      }
      return builder.ToString();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='lookupRules'>Another Boolean object.</param>
    /// <returns>A Boolean object.</returns>
    public static bool IsValidDomainName(string str, bool lookupRules) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      bool bidiRule = HasRtlCharacters(str);
      int lastIndex = 0;
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c == '.') {
          if (i == lastIndex) {
            // Empty label
            return false;
          }
          if (!IsValidLabel(str.Substring(lastIndex, i - lastIndex),
            lookupRules, bidiRule)) {
            return false;
          }
          lastIndex = i + 1;
        }
      }
      return (str.Length != lastIndex) &&
        IsValidLabel(str.Substring(lastIndex, str.Length - lastIndex),
        lookupRules, bidiRule);
    }

    private static string ToLowerCaseAscii(string str) {
      if (str == null) {
        return null;
      }
      int len = str.Length;
      var c = (char)0;
      bool hasUpperCase = false;
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

private static bool IsValidLabel(string str, bool lookupRules, bool
      bidiRule) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      bool maybeALabel = str.Length >= 4 && (str[0] == 'x' || str[0] == 'X') &&
        (str[1] == 'n' || str[1] == 'N') && str[2] == '-' && str[3] == '-';
      bool allLDH = true;
      for (int i = 0; i < str.Length; ++i) {
    if ((str[i] >= 'a' && str[i] <= 'z') || (str[i] >= 'A' && str[i] <= 'Z'
) ||
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
        string astr = DomainUtility.PunycodeEncodePortion(ustr, 0, ustr.Length);
        // NOTE: "astr" and "str" will contain only ASCII characters
        // at this point, so a simple null check and
        // binary comparison are enough
        return (astr != null) && astr.Equals(str);
      }
      if (allLDH) {
        if (str.Length >= 4 && str[2] == '-' && str[3] == '-') {
          // Contains a hyphen at the third and fourth (one-based) character
          //positions
          return false;
        }
        if (str[0] != '-' && str[str.Length - 1] != '-' && !(str[0] >= '0'&&
          str[0] <= '9')) {
          // Only LDH characters, doesn't start with hyphen or digit,
          // and doesn't end with hyphen
          return true;
        }
      }
      return IsValidULabel(str, lookupRules, bidiRule);
    }

    private static bool IsValidULabel(string str, bool lookupRules, bool
      bidiRule) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      if (str.Length > 63 && !lookupRules) {
        // Too long
        return false;
      }
      if (str.Length >= 4 && str[2] == '-' && str[3] == '-') {
        // Contains a hyphen at the third and fourth (one-based) character
        //positions
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
      if (!Normalizer.IsNormalized(str, Normalization.NFC)) {
        return false;
      }
      int ch;
      bool first = true;
      bool haveContextual = false;
      bool rtl = false;
      int bidiClass;
      for (int i = 0; i < str.Length; ++i) {
        ch = CodePointAt(str, i);
        if (ch >= 0x10000) {
          ++i;
        }
        int category = UnicodeDatabase.GetIdnaCategory(ch);
        if (category == Disallowed || category == Unassigned) {
          return false;
        }
        if (first) {
          if (UnicodeDatabase.IsCombiningMark(ch)) {
            return false;
          }
          if (bidiRule) {
            bidiClass = GetBidiClass(ch);
            if (bidiClass == BidiClassR || bidiClass == BidiClassAL) {
              rtl = true;
            } else if (bidiClass != BidiClassL) {
              // forbidden bidi type as the first character
              return false;
            }
          }
        }
        haveContextual |= category == ContextO || category == ContextJ;
        first = false;
      }
      if (haveContextual) {
        bool regArabDigits = false;
        bool extArabDigits = false;
        bool haveKatakanaMiddleDot = false;
        bool haveKanaOrHan = false;
        int lastChar = 0;
        for (int i = 0; i < str.Length; ++i) {
          int thisChar = CodePointAt(str, i);
          if (thisChar >= 0x660 && thisChar <= 0x669) {
            // Arabic-Indic digits
            // NOTE: Test done here even under lookup rules,
            // even though they're CONTEXTO characters
            if (extArabDigits) {
              return false;
            }
            regArabDigits = true;
          } else if (thisChar >= 0x6f0 && thisChar <= 0x6f9) {
            // Extended Arabic-Indic digits
            // NOTE: Test done here even under lookup rules,
            // even though they're CONTEXTO characters
            if (regArabDigits) {
              return false;
            }
            extArabDigits = true;
          } else if (thisChar == 0xb7) {
            // Middle dot
            // NOTE: Test done here even under lookup rules,
            // even though it's a CONTEXTO character
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
            // Zero-width non-joiner
            if (UnicodeDatabase.GetCombiningClass(lastChar) != 9 &&
                !IsValidConjunct(str, i)) {
              return false;
            }
          } else if (thisChar == 0x375) {
            // Keraia
            // NOTE: Test done here even under lookup rules,
            // even though it's a CONTEXTO character
            if (i + 1 >= str.Length || !IsGreek(CodePointAt(str, i + 1))) {
              return false;
            }
          } else if (thisChar == 0x5f3 || thisChar == 0x5f4) {
            // Geresh or gershayim
            // NOTE: Test done here even under lookup rules,
            // even though they're CONTEXTO characters
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
          // even though it's a CONTEXTO character
          return false;
        }
      }
      // Bidi Rule
      if (bidiRule) {
        bool found = false;
        for (int i = str.Length; i > 0; --i) {
          int c = CodePointBefore(str, i);
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
        bool haveEN = false;
        bool haveAN = false;
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
      }
      int aceLength = DomainUtility.PunycodeLength(str, 0, str.Length);
      if (aceLength < 0) {
        return false;  // Overflow error
      }
      if (!lookupRules) {
        // Additional rules for non-lookup validation
        if (aceLength > 63) {
          return false;
        }
      }
      return true;
    }
  }
}
