package com.upokecenter.mail;

import com.upokecenter.util.*;
import com.upokecenter.mail.transforms.*;
import com.upokecenter.text.*;

  final class MakeFilenameMethod {
private MakeFilenameMethod() {
}
    private static String TrimAndCollapseSpaceAndTab(String str) {
      if (((str) == null || (str).length() == 0)) {
        return str;
      }
      StringBuilder builder = null;
      int index = 0;
      int leadIndex;
      // Skip leading whitespace, if any
      while (index < str.length()) {
        char c = str.charAt(index);
        if (c == 0x09 || c == 0x20) {
          builder = (builder == null) ? ((new StringBuilder())) : builder;
          ++index;
        } else {
          break;
        }
      }
      leadIndex = index;
      while (index < str.length()) {
        int si = index;
        char c = str.charAt(index++);
        int count = 0;
        while (c == 0x09 || c == 0x20) {
          ++count;
          if (index < str.length()) {
            c = str.charAt(index++);
          } else {
            break;
          }
        }
        if (count > 0) {
          if (builder == null) {
            builder = new StringBuilder();
            builder.append(str.substring(leadIndex, (leadIndex)+(si)));
          }
          if (c != 0x09 && c != 0x20) {
            builder.append(' ');
            builder.append(c);
          }
        } else {
          if (builder != null) {
            builder.append(c);
          }
        }
      }
      return (builder == null) ? str : builder.toString();
    }

    private static String DecodeEncodedWordsLenient(
  String str,
  int index,
  int endIndex) {
      int state = 0;
      int markStart = 0;
      int wordStart = 0;
      int charsetStart = -1;
      int charsetEnd = -1;
      int dataStart = -1;
      int encoding = 0;
      boolean haveSpace = false;
      if (str.indexOf('=') < 0) {
        // Contains no equal sign, and therefore no
        // encoded words
        return str.substring(index, (index)+(endIndex - index));
      }
      StringBuilder builder = new StringBuilder();
      while (index <= endIndex) {
        switch (state) {
          case 0:
            // normal
            if (index >= endIndex) {
              ++index;
              break;
            }
            if (str.charAt(index) == '=' && index + 1 < endIndex &&
            str.charAt(index + 1) == '?') {
              wordStart = index;
              state = 1;
              index += 2;
              charsetStart = index;
            } else {
              ++index;
            }
            break;
          case 1:
            // charset
            if (index >= endIndex) {
              state = 0;
              haveSpace = false;
              index = charsetStart;
              break;
            }
            if (str.charAt(index) == '?') {
              charsetEnd = index;
              state = 2;
              ++index;
            } else {
              ++index;
            }
            break;
          case 2:
            // encoding
            if (index >= endIndex) {
              state = 0;
              haveSpace = false;
              index = charsetStart;
              break;
            }
            if ((str.charAt(index) == 'b' || str.charAt(index) == 'B') && index +
          1 < endIndex && str.charAt(index + 1) == '?') {
              encoding = 1;
              state = 3;
              index += 2;
              dataStart = index;
            } else if ((str.charAt(index) == 'q' || str.charAt(index) == 'Q') &&
             index + 1 < endIndex && str.charAt(index + 1) == '?') {
              encoding = 2;
              state = 3;
              index += 2;
              dataStart = index;
            } else {
              state = 0;
              haveSpace = false;
              index = charsetStart;
            }
            break;
          case 3:
            // data
            if (index >= endIndex) {
              state = 0;
              haveSpace = false;
              index = charsetStart;
              break;
            }
            if (str.charAt(index) == '?' && index + 1 < endIndex &&
            str.charAt(index + 1) == '=') {
              String charset = str.substring(
           charsetStart, (
           charsetStart)+(charsetEnd - charsetStart));
              String data = str.substring(dataStart, (dataStart)+(index - dataStart));
              index += 2;
              int endData = index;
              boolean acceptedEncodedWord = true;
              int asterisk = charset.indexOf('*');
              String decodedWord = null;
              if (asterisk >= 1) {
                boolean asteriskAtEnd = asterisk + 1 >= charset.length();
                charset = charset.substring(0, asterisk);
                // Ignore language parameter after the asterisk
                acceptedEncodedWord &= !asteriskAtEnd;
              } else {
                acceptedEncodedWord &= asterisk != 0;
              }
              if (acceptedEncodedWord) {
                IByteReader transform = (encoding == 1) ?
                (IByteReader)new BEncodingStringTransform(data) :
                (IByteReader)new QEncodingStringTransform(data);
                ICharacterEncoding charEncoding = Encodings.GetEncoding(
                charset,
                true);
                if (charEncoding != null) {
                  decodedWord = Encodings.DecodeToString(
                  charEncoding,
                  transform);
                }
              }
              if (decodedWord == null) {
                index = charsetStart;
                haveSpace = false;
                state = 0;
              } else {
                if (!haveSpace) {
                  builder.append(
                 str.substring(
                 markStart, (
                 markStart)+(wordStart - markStart)));
                }
                builder.append(decodedWord);
                haveSpace = false;
                markStart = endData;
                state = 4;
              }
            } else {
              ++index;
            }
            break;
          case 4:
            // space after data
            if (index >= endIndex) {
              ++index;
              break;
            }
            if (str.charAt(index) == '=' && index + 1 < endIndex &&
            str.charAt(index + 1) == '?') {
              wordStart = index;
              state = 1;
              index += 2;
              charsetStart = index;
            } else if (str.charAt(index) == 0x20 || str.charAt(index) == 0x09) {
              ++index;
              haveSpace = true;
            } else {
              ++index;
              state = 0;
              haveSpace = false;
            }
            break;
          default: throw new IllegalStateException();
        }
      }
      builder.append(str.substring(markStart, (markStart)+(str.length() - markStart)));
      return builder.toString();
    }

    private static String DecodeRfc2231ExtensionLenient(String value) {
      // NOTE: Differs from MediaType.DecodeRfc2231Extension in that
      // it doesn't check the syntax of the language tag.
      // This method is only used to adapt suggested filenames
      // (and the language tag is not used for that purpose here
      // anyway), so it is not necessary to follow RFC 2231 or BCP
      // 47 strictly here.
      int firstQuote = value.indexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      int secondQuote = value.indexOf('\'',firstQuote + 1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      String charset = value.substring(0, firstQuote);
      // NOTE: Language tag is ignored here
      String paramValue = value.substring(secondQuote + 1);
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = (cs == null) ? (Encodings.GetEncoding("us-ascii", true)) : cs;
      int quote = paramValue.indexOf('\'');
      return (quote >= 0) ? null : Encodings.DecodeToString(
        cs,
        new PercentEncodingStringTransform(paramValue));
    }

    private static String RemoveEncodedWordEnds(String str) {
      StringBuilder sb = new StringBuilder();
      int index = 0;
      boolean inEncodedWord = false;
      while (index < str.length()) {
        if (!inEncodedWord && index + 1 < str.length() && str.charAt(index) == '=' &&
              str.charAt(index + 1) == '?') {
          // Remove start of encoded word
          inEncodedWord = true;
          index += 2;
          int start = index;
          int qmarks = 0;
          // skip charset and encoding
          while (index < str.length()) {
            if (str.charAt(index) == '?') {
              ++qmarks;
              ++index;
              if (qmarks == 2) {
                break;
              }
            } else {
              ++index;
            }
          }
          if (qmarks == 2) {
            inEncodedWord = true;
          } else {
            inEncodedWord = false;
            sb.append('=');
            sb.append('?');
            index = start;
          }
        } else if (inEncodedWord && index + 1 < str.length() && str.charAt(index)
             == '?' && str.charAt(index + 1) == '=') {
          // End of encoded word
          index += 2;
          inEncodedWord = false;
        } else {
          int c = DataUtilities.CodePointAt(str, index);
          if (c == 0xfffd) {
            sb.append((char)0xfffd);
            ++index;
          } else {
            sb.append(str.charAt(index++));
            if (c >= 0x10000) {
              sb.append(str.charAt(index++));
            }
          }
        }
      }
      return sb.toString();
    }

    private static String SurrogateCleanup(String str) {
      int i = 0;
      while (i < str.length()) {
        int c = DataUtilities.CodePointAt(str, i, 2);
        // NOTE: Unpaired surrogates are replaced with -1
        if (c >= 0x10000) {
          ++i;
        }
        if (c < 0) {
          break;
        }
        ++i;
      }
      if (i >= str.length()) {
        return str;
      }
      StringBuilder builder = new StringBuilder();
      builder.append(str.substring(0, i));
      while (i < str.length()) {
        int c = DataUtilities.CodePointAt(str, i, 0);
        // NOTE: Unpaired surrogates are replaced with U + FFFD
        if (c >= 0x10000) {
          builder.append(str.charAt(i));
          builder.append(str.charAt(i + 1));
          i += 2;
        } else {
          char ch = (char)c;
          builder.append(ch);
          ++i;
        }
      }
      return builder.toString();
    }

    private static final int[] ValueCharCheck = {
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0
    };

    private static final int[] ValueCharCheckInitial = {
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1,
      2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
      0, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1,
      2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0
    };

    private static boolean SimplifiedFileCheck(String name) {
      if (((name) == null || (name).length() == 0) || name.length() > 128) {
        return false;
      }
      boolean dotSeen = false;
      for (int i = 0; i < name.length(); ++i) {
        int c = (int)name.charAt(i);
        if (c >= 0x7f) {
          return false;
        }
        if (c == 0x2e) {
          if (i == 0 || i == name.length() - 1 || dotSeen) {
            return false;
          }
          dotSeen = true;
          continue;
        }
        int cc = (i == 0) ? ValueCharCheckInitial[c] : ValueCharCheck[c];
        dotSeen = false;
        if (cc == 0) {
          return false;
        }
        if (cc == 2) {
          if (name.length() == 3 || name.length() == 4) {
            return false;
          }
          if (name.length() > 4 && (name.charAt(3) == '.' || name.charAt(4) == '.')) {
            return false;
          }
        }
      }
      return true;
    }

    private static String Rfc2231Adjust(String str) {
      int index = str.indexOf('\'');
      if (index > 0) {
        // Check for RFC 2231 encoding, as long as the value before the
        // apostrophe is a recognized charset. It appears to be common,
        // too, to use quotes around a filename parameter AND use
        // RFC 2231 encoding, even though all the examples in that RFC
        // show unquoted use of this encoding.
        String charset = Encodings.ResolveAliasForEmail(
  str.substring(
  0, (
  0)+(index)));
        if (!((charset) == null || (charset).length() == 0)) {
          String newstr = DecodeRfc2231ExtensionLenient(str);
          if (!((newstr) == null || (newstr).length() == 0)) {
            // Value was decoded under RFC 2231
            str = newstr;
            index = str.indexOf('\'');
            if (index > 0) {
              String tmpstr = str.substring(0, index);
              charset = Encodings.ResolveAliasForEmail(tmpstr);
              if (!((charset) == null || (charset).length() == 0)) {
                // First part of String is again a
                // charset, so ensure idempotency by ensuring
                // that part is no longer a charset
                str = tmpstr + "_" + str.substring(index + 1);
              }
            }
          }
        }
      }
      return str;
    }

    public static String MakeFilename(String str) {
      if (((str) == null || (str).length() == 0)) {
        return "";
      }
      if (SimplifiedFileCheck(str)) {
        return str;
      }
      int i;
      str = TrimAndCollapseSpaceAndTab(str);
      str = SurrogateCleanup(str);
      if (str.indexOf("=?") >= 0) {
        // May contain encoded words, which are very frequent
        // in Content-Disposition filenames (they would appear quoted
        // in the Content-Disposition "filename" parameter); these
        // changes
        // appear justified in sec. 2.3 of RFC 2183, which says that
        // the parameter's value "should be used as a
        // basis for the actual filename, where possible."
        str = MakeFilenameMethod.DecodeEncodedWordsLenient(
  str,
  0,
  str.length());
        if (str.indexOf("=?") >= 0) {
          // Remove ends of encoded words that remain
          str = RemoveEncodedWordEnds(str);
        }
      }
      str = Rfc2231Adjust(str);
      String oldstr = null;
      do {
        oldstr = str;
        str = TrimAndCollapseSpaceAndTab(str);
        if (str.length() == 0) {
          return "_";
        }
        StringBuilder builder = new StringBuilder();
        // Replace unsuitable characters for filenames
        // and make sure the filename's
        // length doesn't exceed 254. (A few additional characters
        // may be added later on.)
        // NOTE: Even if there are directory separators (backslash
        // and forward slash), the filename is not treated as a
        // file system path (in accordance with sec. 2.3 of RFC
        // 2183); as a result, the directory separators
        // will be treated as unsuitable characters for filenames
        // and are handled below.
        i = 0;
        while (i < str.length() && builder.length() < 254) {
          int c = DataUtilities.CodePointAt(str, i, 0);
          // NOTE: Unpaired surrogates are replaced with U + FFFD
          if (c >= 0x10000) {
            ++i;
          }
          if (c == (int)'\t' || c == 0xa0 || c == 0x3000 ||
     c == 0x180e || c == 0x1680 ||
  (c >= 0x2000 && c <= 0x200b) || c == 0x205f || c == 0x202f || c ==
       0xfeff) {
            // Replace space-like characters (including tab) with space
            builder.append(' ');
          } else if (c < 0x20 || c == '\\' || c == '/' || c == '*' ||
            c == '?' || c == '|' ||
      c == ':' || c == '<' || c == '>' || c == '"' ||
            (c >= 0x7f && c <= 0x9f)) {
            // Unsuitable character for a filename (one of the
            // characters
            // reserved by Windows,
            // backslash, forward slash, ASCII controls, and C1
            // controls).
            builder.append('_');
          } else if (c == '!' && i + 1 < str.length() && str.charAt(i + 1) == '[') {
            // '![ ... ]' may be interpreted in BASH as an evaluator;
            // replace '!' with underscore
            builder.append('_');
          } else if (c == '`') {
            // '`' starts a command in BASH and possibly other shells
            builder.append('_');
          } else if (c == '\'') {
            // "'" starts a filename String in BASH and possibly other shells
            builder.append('_');
          } else if (c == '#') {
            // Fragment identifier for URIs
            builder.append('_');
          } else if (c == '$') {
            // '$' starts a variable in BASH and possibly other shells
            builder.append('_');
          } else if (c == ';') {
            // ';' separates command lines in BASH and possibly
            // other shells
            builder.append('_');
          } else if (c == 0x2028 || c == 0x2029) {
            // line break characters (0x85 is already included above)
            builder.append('_');
          } else if ((c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <=
                 0xfdef)) {
            // noncharacters
            builder.append('_');
          } else if (c == '%') {
            // Treat percent ((character instanceof unsuitable) ? (unsuitable)character : null), even though it
            // can occur
            // in a Windows filename, since it's used in MS-DOS and
            // Windows
            // in environment variable placeholders
            builder.append('_');
          } else {
            if (builder.length() < 254 || c < 0x10000) {
              if (c <= 0xffff) {
                builder.append((char)c);
              } else if (c <= 0x10ffff) {
                builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) +
                    0xd800));
                builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
              }
            } else if (builder.length() >= 253) {
              break;
            }
          }
          ++i;
        }
        str = builder.toString();
        if (str.length() == 0) {
 return "_";
}
        String strLower = DataUtilities.ToLowerCaseAscii(str);
        // Reserved filenames: NUL, CLOCK$, PRN, AUX, CON, as
        // well as "!["
        boolean reservedFilename = strLower.equals(
          "nul") || strLower.equals("clock$") || strLower.indexOf(
          "nul.") == 0 || strLower.equals(
          "prn") || strLower.indexOf(
          "prn.") == 0 || strLower.indexOf(
          "![") >= 0 || strLower.equals(
          "aux") || strLower.indexOf(
          "aux.") == 0 || strLower.equals(
          "con") || strLower.indexOf(
          "con.") == 0;
        // LPTn, COMn
     if (strLower.length() == 4 || (strLower.length() > 4 && (strLower.charAt(4) == '.' ||
          strLower.charAt(4) == ' '))) {
          reservedFilename = reservedFilename || (strLower.indexOf(
            "lpt") == 0 && strLower.charAt(3) >= '0' &&
                 strLower.charAt(3) <= '9');
          reservedFilename = reservedFilename || (strLower.indexOf(
          "com") == 0 && strLower.charAt(3) >= '0' &&
                strLower.charAt(3) <= '9');
        }
        boolean bracketDigit = str.charAt(0) == '{' && str.length() > 1 &&
              str.charAt(1) >= '0' && str.charAt(1) <= '9';
        // Home folder convention (tilde).
        // Filenames starting with hyphens can also be
        // problematic especially in Unix-based systems,
        // and filenames starting with dollar sign can
        // be misinterpreted if they're treated as expansion
        // symbols
        boolean homeFolder = str.charAt(0) == '~' || str.charAt(0) == '-' || str.charAt(0) ==
            '$';
        // Starts with period; may be hidden in some configurations
        boolean period = str.charAt(0) == '.';
        if (reservedFilename || bracketDigit || homeFolder ||
             period) {
          str = "_" + str;
        }
        str = TrimAndCollapseSpaceAndTab(str);
        str = NormalizerInput.Normalize(str, Normalization.NFC);
        // Avoid space before and after last dot
        for (i = str.length() - 1; i >= 0; --i) {
          if (str.charAt(i) == '.') {
            boolean spaceAfter = i + 1 < str.length() && str.charAt(i + 1) == 0x20;
            boolean spaceBefore = i > 0 && str.charAt(i - 1) == 0x20;
            if (spaceAfter && spaceBefore) {
              str = str.substring(0,i - 1) + "_._" + str.substring(i +
                  2);
            } else if (spaceAfter) {
              str = str.substring(0,i) + "._" + str.substring(i + 2);
            } else if (spaceBefore) {
              str = str.substring(0,i - 1) + "_." + str.substring(i +
                 1);
            }
            break;
          }
        }
        if (str.charAt(str.length() - 1) == '.' || str.charAt(str.length() - 1) == '~') {
          // Ends in a dot or tilde (a file whose name ends with
          // the latter may be treated as
          // a backup file especially in Unix-based systems).
          // NOTE: Although concatenation of two NFC strings
          // doesn't necessarily lead to an NFC String, this
          // particular concatenation doesn't disturb the NFC
          // status of the String.
          str += "_";
        }
      } while (!oldstr.equals(str));
      return str;
    }
  }
