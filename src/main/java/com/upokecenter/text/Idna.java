package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

    /**
     * <p>Contains methods that implement Internationalized Domain Names in
     * Applications (IDNA). IDNA enables using a wider range of letters,
     * numbers, and certain other characters in domain names.</p> <p>NOTICE:
     * While this class's source code is in the public domain, the class
     * uses two internal classes, called <code>NormalizationData</code> and
     * <code>IdnaData</code> , that include data derived from the Unicode
     * Character Database. See the documentation for the Normalizer class
     * for the permission notice for the Unicode Character Database.</p>
     */
  public final class Idna {
private Idna() {
}
    private static final int Unassigned = 0;
    // PValid = 1;
    private static final int Disallowed = 2;
    private static final int ContextJ = 3;
    private static final int ContextO = 4;
    private static final int BidiClassL = 0;
    private static final int BidiClassR = 1;
    private static final int BidiClassAL = 2;
    private static final int BidiClassEN = 3;
    private static final int BidiClassES = 4;
    private static final int BidiClassET = 5;
    private static final int BidiClassAN = 6;
    private static final int BidiClassCS = 7;
    private static final int BidiClassNSM = 8;
    private static final int BidiClassBN = 9;
    private static final int BidiClassON = 10;

    private static ByteData bidiClasses;
    private static ByteData joiningTypes;
    private static ByteData scripts;
    private static Object bidiClassesSync = new Object();
    private static Object joiningTypesSync = new Object();
    private static Object scriptsSync = new Object();

    static int CodePointBefore(String str, int index) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index <= 0) {
        return -1;
      }
      if (index > str.length()) {
        return -1;
      }
      int c = str.charAt(index - 1);
      if ((c & 0xfc00) == 0xdc00 && index - 2 >= 0 &&
             str.charAt(index - 2) >= 0xd800 && str.charAt(index - 2) <= 0xdbff) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((str.charAt(index - 2) - 0xd800) << 10) + (c - 0xdc00);
      }
      return ((c & 0xf800) == 0xd800) ? 0xfffd : c;
    }

    static int CodePointAt(String str, int index) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index >= str.length()) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str.charAt(index);
      if ((c & 0xfc00) == 0xd800 && index + 1 < str.length() &&
          str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        return 0x10000 + ((c - 0xd800) << 10) + (str.charAt(index + 1) - 0xdc00);
      }
      return ((c & 0xf800) == 0xd800) ? 0xfffd : c;
    }

    static int GetBidiClass(int ch) {
      ByteData table = null;
      synchronized (bidiClassesSync) {
        bidiClasses = (bidiClasses == null) ? (ByteData.Decompress(IdnaData.BidiClasses)) : bidiClasses;
        table = bidiClasses;
      }
      return table.GetByte(ch);
    }

    private static int GetJoiningType(int ch) {
      ByteData table = null;
      synchronized (joiningTypesSync) {
     joiningTypes = (joiningTypes == null) ? (ByteData.Decompress(IdnaData.JoiningTypes)) : joiningTypes;
        table = joiningTypes;
      }
      return table.GetByte(ch);
    }

    private static int GetScript(int ch) {
      ByteData table = null;
      synchronized (scriptsSync) {
        scripts = (scripts == null) ? (ByteData.Decompress(IdnaData.IdnaRelevantScripts)) : scripts;
        table = scripts;
      }
      return table.GetByte(ch);
    }

    private static boolean JoiningTypeTransparent(int ch) {
      return GetJoiningType(ch) == 1;
    }

    private static boolean JoiningTypeLeftOrDual(int ch) {
      int jtype = GetJoiningType(ch);
      return jtype == 3 || jtype == 4;
    }

    private static boolean JoiningTypeRightOrDual(int ch) {
      int jtype = GetJoiningType(ch);
      return jtype == 2 || jtype == 4;
    }

    private static boolean IsGreek(int ch) {
      return GetScript(ch) == 1;
    }

    private static boolean IsHebrew(int ch) {
      return GetScript(ch) == 2;
    }

    private static boolean IsKanaOrHan(int ch) {
      return GetScript(ch) == 3;
    }

    private static boolean IsValidConjunct(String str, int index) {
      // Assumes that the character at the given index
      // is Zero-Width Non-Joiner
      // Check the left
      boolean found = false;
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
      while (index < str.length()) {
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

    private static boolean HasRtlCharacters(String str) {
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) >= 0x80) {
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

    /**
     * Tries to encode each label of a domain name into Punycode.
     * @param value A domain name.
     * @return The domain name where each label with non-ASCII characters is
     * encoded into Punycode. Labels where this is not possible remain
     * unchanged.
     * @throws NullPointerException Value is null.
     */
    public static String EncodeDomainName(String value) {
      if (value == null) {
        throw new NullPointerException("value");
      }
      if (value.length() == 0) {
        return "";
      }
      StringBuilder builder = new StringBuilder();
      String retval = null;
      int lastIndex = 0;
      for (int i = 0; i < value.length(); ++i) {
        char c = value.charAt(i);
        if (c == '.') {
          if (i != lastIndex) {
            retval = DomainUtility.PunycodeEncodePortion(value, lastIndex, i);
            if (retval == null) {
              // Append the unmodified domain plus the dot
              builder.append(value.substring(lastIndex, (lastIndex)+((i + 1) - lastIndex)));
            } else {
              builder.append(retval);
              builder.append('.');
            }
          }
          lastIndex = i + 1;
        }
      }
  retval = DomainUtility.PunycodeEncodePortion(value, lastIndex,
        value.length());
      if (retval == null) {
        builder.append(value.substring(lastIndex, (lastIndex)+(value.length() - lastIndex)));
      } else {
        builder.append(retval);
      }
      return builder.toString();
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param lookupRules Another Boolean object.
     * @return A Boolean object.
     */
    public static boolean IsValidDomainName(String str, boolean lookupRules) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      boolean bidiRule = HasRtlCharacters(str);
      int lastIndex = 0;
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (c == '.') {
          if (i == lastIndex) {
            // Empty label
            return false;
          }
          if (!IsValidLabel(str.substring(lastIndex, (lastIndex)+(i - lastIndex)),
            lookupRules, bidiRule)) {
            return false;
          }
          lastIndex = i + 1;
        }
      }
      return (str.length() != lastIndex) &&
        IsValidLabel(str.substring(lastIndex, (lastIndex)+(str.length() - lastIndex)),
        lookupRules, bidiRule);
    }

    private static String ToLowerCaseAscii(String str) {
      if (str == null) {
        return null;
      }
      int len = str.length();
      char c = (char)0;
      boolean hasUpperCase = false;
      for (int i = 0; i < len; ++i) {
        c = str.charAt(i);
        if (c >= 'A' && c <= 'Z') {
          hasUpperCase = true;
          break;
        }
      }
      if (!hasUpperCase) {
        return str;
      }
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < len; ++i) {
        c = str.charAt(i);
        if (c >= 'A' && c <= 'Z') {
          builder.append((char)(c + 0x20));
        } else {
          builder.append(c);
        }
      }
      return builder.toString();
    }

private static boolean IsValidLabel(String str, boolean lookupRules, boolean
      bidiRule) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      boolean maybeALabel = str.length() >= 4 && (str.charAt(0) == 'x' || str.charAt(0) == 'X') &&
        (str.charAt(1) == 'n' || str.charAt(1) == 'N') && str.charAt(2) == '-' && str.charAt(3) == '-';
      boolean allLDH = true;
      for (int i = 0; i < str.length(); ++i) {
    if ((str.charAt(i) >= 'a' && str.charAt(i) <= 'z') || (str.charAt(i) >= 'A' && str.charAt(i) <= 'Z'
) ||
                (str.charAt(i) >= '0' && str.charAt(i) <= '9') || str.charAt(i) == '-') {
          // LDH character
          continue;
        }
        if (str.charAt(i) >= 0x80) {
          // Non-ASCII character
          allLDH = false;
          continue;
        }
        return false;
      }
      if (maybeALabel) {
        str = ToLowerCaseAscii(str);
        String ustr = DomainUtility.PunycodeDecode(str, 4, str.length());
        if (ustr == null) {
          // NOTE: Returns null if "str" contains non-ASCII characters
          return false;
        }
        if (!IsValidULabel(ustr, lookupRules, bidiRule)) {
          return false;
        }
        String astr = DomainUtility.PunycodeEncodePortion(ustr, 0, ustr.length());
        // NOTE: "astr" and "str" will contain only ASCII characters
        // at this point, so a simple null check and
        // binary comparison are enough
        return (astr != null) && astr.equals(str);
      }
      if (allLDH) {
        if (str.length() >= 4 && str.charAt(2) == '-' && str.charAt(3) == '-') {
          // Contains a hyphen at the third and fourth (one-based) character
          //positions
          return false;
        }
        if (str.charAt(0) != '-' && str.charAt(str.length() - 1) != '-' && !(str.charAt(0) >= '0'&&
          str.charAt(0) <= '9')) {
          // Only LDH characters, doesn't start with hyphen or digit,
          // and doesn't end with hyphen
          return true;
        }
      }
      return IsValidULabel(str, lookupRules, bidiRule);
    }

    private static boolean IsValidULabel(String str, boolean lookupRules, boolean
      bidiRule) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      if (str.length() > 63 && !lookupRules) {
        // Too long
        return false;
      }
      if (str.length() >= 4 && str.charAt(2) == '-' && str.charAt(3) == '-') {
        // Contains a hyphen at the third and fourth (one-based) character
        //positions
        return false;
      }
      if (!lookupRules) {
        // Checking for a hyphen at the start and
        // the end is part of the registration validity
        // rules (sec. 4.2 of RFC 5891), but not the lookup
        // rules (sec. 5.4).
        if (str.charAt(0) == '-' || str.charAt(str.length() - 1) == '-') {
          return false;
        }
      }
      if (!Normalizer.IsNormalized(str, Normalization.NFC)) {
        return false;
      }
      int ch;
      boolean first = true;
      boolean haveContextual = false;
      boolean rtl = false;
      int bidiClass;
      for (int i = 0; i < str.length(); ++i) {
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
        boolean regArabDigits = false;
        boolean extArabDigits = false;
        boolean haveKatakanaMiddleDot = false;
        boolean haveKanaOrHan = false;
        int lastChar = 0;
        for (int i = 0; i < str.length(); ++i) {
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
            if (!(i - 1 >= 0 && i + 1 < str.length() &&
                lastChar == 0x6c && str.charAt(i + 1) == 0x6c)) {
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
            if (i + 1 >= str.length() || !IsGreek(CodePointAt(str, i + 1))) {
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
        boolean found = false;
        for (int i = str.length(); i > 0; --i) {
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
        boolean haveEN = false;
        boolean haveAN = false;
        for (int i = 0; i < str.length(); ++i) {
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
      int aceLength = DomainUtility.PunycodeLength(str, 0, str.length());
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
