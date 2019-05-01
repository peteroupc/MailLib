using System;
using System.Text;
using PeterO;
using PeterO.Mail.Transforms;
using PeterO.Text;

namespace PeterO.Mail {
  internal static class MakeFilenameMethod {
    private static string TrimAndCollapseSpaceAndTab(string str) {
      if (String.IsNullOrEmpty(str)) {
        return str;
      }
      StringBuilder builder = null;
      var index = 0;
      int leadIndex;
      // Skip leading whitespace, if any
      while (index < str.Length) {
        char c = str[index];
        if (c == 0x09 || c == 0x20) {
          builder = builder ?? (new StringBuilder());
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
        while (c == 0x09 || c == 0x20) {
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
          if (c != 0x09 && c != 0x20) {
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

    private static string DecodeEncodedWordsLenient(
  string str,
  int index,
  int endIndex) {
      var state = 0;
      var markStart = 0;
      var wordStart = 0;
      var charsetStart = -1;
      var charsetEnd = -1;
      var dataStart = -1;
      var encoding = 0;
      var haveSpace = false;
      if (str.IndexOf('=') < 0) {
        // Contains no equal sign, and therefore no
        // encoded words
        return str.Substring(index, endIndex - index);
      }
      var builder = new StringBuilder();
      while (index <= endIndex) {
        switch (state) {
          case 0:
            // normal
            if (index >= endIndex) {
              ++index;
              break;
            }
            if (str[index] == '=' && index + 1 < endIndex &&
            str[index + 1] == '?') {
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
            if (str[index] == '?') {
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
            if ((str[index] == 'b' || str[index] == 'B') && index +
          1 < endIndex && str[index + 1] == '?') {
              encoding = 1;
              state = 3;
              index += 2;
              dataStart = index;
            } else if ((str[index] == 'q' || str[index] == 'Q') &&
             index + 1 < endIndex && str[index + 1] == '?') {
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
            if (str[index] == '?' && index + 1 < endIndex &&
            str[index + 1] == '=') {
              string charset = str.Substring(
           charsetStart,
           charsetEnd - charsetStart);
              string data = str.Substring(dataStart, index - dataStart);
              index += 2;
              int endData = index;
              var acceptedEncodedWord = true;
              int asterisk = charset.IndexOf('*');
              string decodedWord = null;
              if (asterisk >= 1) {
                bool asteriskAtEnd = asterisk + 1 >= charset.Length;
                charset = charset.Substring(0, asterisk);
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
                  builder.Append(
                 str.Substring(
                 markStart,
                 wordStart - markStart));
                }
                builder.Append(decodedWord);
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
            if (str[index] == '=' && index + 1 < endIndex &&
            str[index + 1] == '?') {
              wordStart = index;
              state = 1;
              index += 2;
              charsetStart = index;
            } else if (str[index] == 0x20 || str[index] == 0x09) {
              ++index;
              haveSpace = true;
            } else {
              ++index;
              state = 0;
              haveSpace = false;
            }
            break;
          default: throw new InvalidOperationException();
        }
      }
      builder.Append(str.Substring(markStart, str.Length - markStart));
      return builder.ToString();
    }

    private static string DecodeRfc2231ExtensionLenient(string value) {
      // NOTE: Differs from MediaType.DecodeRfc2231Extension in that
      // it doesn't check the syntax of the language tag.
      // This method is only used to adapt suggested filenames
      // (and the language tag is not used for that purpose here
      // anyway), so it is not necessary to follow RFC 2231 or BCP
      // 47 strictly here.
      int firstQuote = value.IndexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      int secondQuote = value.IndexOf('\'', firstQuote + 1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      string charset = value.Substring(0, firstQuote);
      // NOTE: Language tag is ignored here
      string paramValue = value.Substring(secondQuote + 1);
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = cs ?? Encodings.GetEncoding("us-ascii", true);
      int quote = paramValue.IndexOf('\'');
      return (quote >= 0) ? null : Encodings.DecodeToString(
        cs,
        new PercentEncodingStringTransform(paramValue));
    }

    private static string RemoveEncodedWordEnds(string str) {
      var sb = new StringBuilder();
      var index = 0;
      var inEncodedWord = false;
      while (index < str.Length) {
        if (!inEncodedWord && index + 1 < str.Length && str[index] == '=' &&
              str[index + 1] == '?') {
          // Remove start of encoded word
          inEncodedWord = true;
          index += 2;
          int start = index;
          var qmarks = 0;
          // skip charset and encoding
          while (index < str.Length) {
            if (str[index] == '?') {
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
            sb.Append('=');
            sb.Append('?');
            index = start;
          }
        } else if (inEncodedWord && index + 1 < str.Length && str[index]
             == '?' && str[index + 1] == '=') {
          // End of encoded word
          index += 2;
          inEncodedWord = false;
        } else {
          int c = DataUtilities.CodePointAt(str, index);
          if (c == 0xfffd) {
            sb.Append((char)0xfffd);
            ++index;
          } else {
            sb.Append(str[index++]);
            if (c >= 0x10000) {
              sb.Append(str[index++]);
            }
          }
        }
      }
      return sb.ToString();
    }

    private static string SurrogateCleanup(string str) {
      var i = 0;
      while (i < str.Length) {
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
      if (i >= str.Length) {
        return str;
      }
      var builder = new StringBuilder();
      builder.Append(str.Substring(0, i));
      while (i < str.Length) {
        int c = DataUtilities.CodePointAt(str, i, 0);
        // NOTE: Unpaired surrogates are replaced with U + FFFD
        if (c >= 0x10000) {
          builder.Append(str[i]);
          builder.Append(str[i + 1]);
          i += 2;
        } else {
          var ch = (char)c;
          builder.Append(ch);
          ++i;
        }
      }
      return builder.ToString();
    }

    private static string BaseAndSlashCleanup(string str) {
      var i = 0;
      while (i < str.Length) {
        int c = str[i];
        if (c >= 0x219a && c <= 0x22ed) {
          break;
        } else {
          ++i;
        }
      }
      if (i >= str.Length) {
        return str;
      }
      var builder = new StringBuilder();
      builder.Append(str.Substring(0, i));
      while (i < str.Length) {
        int c = str[i];
        if (c >= 0x219a && c <= 0x22ed) {
          // If this is a character that is the combined form
          // of another character and a combining slash, use
          // an alternate form to ease compatibility when converting
          // the return value to normalization form D: this kind of
          // character can appear composed in NFC and is decomposed
          // in NFD, but will be left alone by HFS Plus's version of NFD.
          // This is the only kind of character in Unicode with this
          // normalization property.
          var tsb = new StringBuilder().Append((char)c);
          string tss = NormalizerInput.Normalize(
  tsb.ToString(),
  Normalization.NFD);
          if (tss.IndexOf((char)0x338) >= 0) {
              builder.Append('!');
              builder.Append(tss[0]);
          } else {
            builder.Append(c);
          }
          ++i;
        } else {
          builder.Append((char)c);
          ++i;
        }
      }
      return builder.ToString();
    }

    private static readonly int[] ValueCharCheck = {
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0
    };

    private static readonly int[] ValueCharCheckInitial = {
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1,
      2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
      0, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1,
      2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0
    };

    private static bool SimplifiedFileCheck(string name) {
      if (String.IsNullOrEmpty(name) || name.Length > 128) {
        return false;
      }
      var dotSeen = false;
      for (var i = 0; i < name.Length; ++i) {
        var c = (int)name[i];
        if (c >= 0x7f) {
          return false;
        }
        if (c == 0x2e) {
          if (i == 0 || i == name.Length - 1 || dotSeen) {
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
          if (name.Length == 3 || name.Length == 4) {
            return false;
          }
          if (name.Length > 4 && (name[3] == '.' || name[4] == '.')) {
            return false;
          }
        }
      }
      return true;
    }

    private static string Rfc2231Adjust(string str) {
      int index = str.IndexOf('\'');
      if (index > 0) {
        // Check for RFC 2231 encoding, as long as the value before the
        // apostrophe is a recognized charset. It appears to be common,
        // too, to use quotes around a filename parameter AND use
        // RFC 2231 encoding, even though all the examples in that RFC
        // show unquoted use of this encoding.
        string charset = Encodings.ResolveAliasForEmail(
  str.Substring(
  0,
  index));
        if (!String.IsNullOrEmpty(charset)) {
          string newstr = DecodeRfc2231ExtensionLenient(str);
          if (!String.IsNullOrEmpty(newstr)) {
            // Value was decoded under RFC 2231
            str = newstr;
            index = str.IndexOf('\'');
            if (index > 0) {
              string tmpstr = str.Substring(0, index);
              charset = Encodings.ResolveAliasForEmail(tmpstr);
              if (!String.IsNullOrEmpty(charset)) {
                // First part of string is again a
                // charset, so ensure idempotency by ensuring
                // that part is no longer a charset
                str = tmpstr + "_" + str.Substring(index + 1);
              }
            }
          }
        }
      }
      return str;
    }

    private const int MaxFileNameCodeUnitLength = 247;

    public static string MakeFilename(string str) {
      if (String.IsNullOrEmpty(str)) {
        return String.Empty;
      }
      if (SimplifiedFileCheck(str)) {
        return str;
      }
      int i;
      str = TrimAndCollapseSpaceAndTab(str);
      str = SurrogateCleanup(str);
      if (str.IndexOf("=?", StringComparison.Ordinal) >= 0) {
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
  str.Length);
        if (str.IndexOf("=?", StringComparison.Ordinal) >= 0) {
          // Remove ends of encoded words that remain
          str = RemoveEncodedWordEnds(str);
        }
      }
      str = Rfc2231Adjust(str);
      string oldstr = null;
      do {
        oldstr = str;
        str = TrimAndCollapseSpaceAndTab(str);
        if (str.Length == 0) {
          return "_";
        }
        int bracketedText=str.IndexOf('[');
        if (bracketedText >= 0) {
 bracketedText=str.IndexOf(']',bracketedText);
}
        var builder = new StringBuilder();
        // Replace unsuitable characters for filenames
        // and make sure the filename's
        // length doesn't exceed MaxFileNameCodeUnitLength.
        // (A few additional characters
        // may be added later on.)
        // NOTE: Even if there are directory separators (backslash
        // and forward slash), the filename is not treated as a
        // file system path (in accordance with sec. 2.3 of RFC
        // 2183); as a result, the directory separators
        // will be treated as unsuitable characters for filenames
        // and are handled below.
        i = 0;
        while (i < str.Length && builder.Length < MaxFileNameCodeUnitLength) {
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
            builder.Append(' ');
          } else if (c < 0x20 || c == '\\' || c == '/' || c == '*' ||
            c == '?' || c == '|' ||
      c == ':' || c == '<' || c == '>' || c == '"' ||
            (c >= 0x7f && c <= 0x9f)) {
            // Unsuitable character for a filename (one of the
            // characters
            // reserved by Windows,
            // backslash, forward slash, ASCII controls, and C1
            // controls).
            builder.Append('_');
          } else if (c == '!' && i + 1 < str.Length && str[i + 1] == '[') {
            // '![ ... ]' may be interpreted in BASH as an evaluator;
            // replace '!' with underscore
            builder.Append('_');
          } else if (bracketedText>= 0 && str[i]=='[') {
            // Avoid glob bracket pattern
            builder.Append('(');
          } else if (bracketedText>= 0 && str[i]==']') {
            // Avoid glob bracket pattern
            builder.Append(')');
          } else if (c == '`') {
            // '`' starts a command in BASH and possibly other shells
            builder.Append('_');
          } else if (c == '\'') {
            // "'" starts a filename string in BASH and possibly other shells
            builder.Append('_');
          } else if (c == '#') {
            // Fragment identifier for URIs
            builder.Append('_');
          } else if (c == '$') {
            // '$' starts a variable in BASH and possibly other shells
            builder.Append('_');
          } else if (c == ';') {
            // ';' separates command lines in BASH and possibly
            // other shells
            builder.Append('_');
          } else if (c == 0x2028 || c == 0x2029) {
            // line break characters (0x85 is already included above)
            builder.Append('_');
          } else if ((c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <=
                 0xfdef)) {
            // noncharacters
            builder.Append('_');
          } else if (c == '%') {
            // Treat percent character as unsuitable, even though it
            // can occur
            // in a Windows filename, since it's used in MS-DOS and
            // Windows
            // in environment variable placeholders
            builder.Append('_');
          } else {
            if (builder.Length < MaxFileNameCodeUnitLength || c < 0x10000) {
              if (c <= 0xffff) {
                builder.Append((char)c);
              } else if (c <= 0x10ffff) {
                builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) +
                    0xd800));
                builder.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
              }
            } else if (builder.Length >= MaxFileNameCodeUnitLength - 1) {
              break;
            }
          }
          ++i;
        }
        str = builder.ToString();
        if (str.Length == 0) {
 return "_";
}
        string strLower = DataUtilities.ToLowerCaseAscii(str);
        // Reserved filenames: NUL, CLOCK$, PRN, AUX, CON, as
        // well as "!["
        bool reservedFilename = strLower.Equals(
          "nul") || strLower.Equals("clock$") || strLower.IndexOf(
          "nul.",
          StringComparison.Ordinal) == 0 || strLower.Equals(
          "prn") || strLower.IndexOf(
          "prn.",
          StringComparison.Ordinal) == 0 || strLower.IndexOf(
          "![",
          StringComparison.Ordinal) >= 0 || strLower.Equals(
          "aux") || strLower.IndexOf(
          "aux.",
          StringComparison.Ordinal) == 0 || strLower.Equals(
          "con") || strLower.IndexOf(
          "con.",
          StringComparison.Ordinal) == 0;
        // LPTn, COMn
     if (strLower.Length == 4 || (strLower.Length > 4 && (strLower[4] == '.' ||
          strLower[4] == ' '))) {
          reservedFilename = reservedFilename || (strLower.IndexOf(
            "lpt",
            StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
                 strLower[3] <= '9');
          reservedFilename = reservedFilename || (strLower.IndexOf(
          "com",
          StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
                strLower[3] <= '9');
        }
        bool bracketDigit = str[0] == '{' && str.Length > 1 &&
              str[1] >= '0' && str[1] <= '9';
        // Home folder convention (tilde).
        // Filenames starting with hyphens can also be
        // problematic especially in Unix-based systems,
        // and filenames starting with dollar sign can
        // be misinterpreted if they're treated as expansion
        // symbols
        bool homeFolder = str[0] == '~' || str[0] == '-' || str[0] ==
            '$';
        // Starts with period; may be hidden in some configurations
        bool period = str[0] == '.';
        if (reservedFilename || bracketDigit || homeFolder ||
             period) {
          str = "_" + str;
        }
        str = TrimAndCollapseSpaceAndTab(str);
        str = NormalizerInput.Normalize(str, Normalization.NFC);
        str = BaseAndSlashCleanup(str);
        // Avoid space before and after last dot
        for (i = str.Length - 1; i >= 0; --i) {
          if (str[i] == '.') {
            bool spaceAfter = i + 1 < str.Length && str[i + 1] == 0x20;
            bool spaceBefore = i > 0 && str[i - 1] == 0x20;
            if (spaceAfter && spaceBefore) {
              str = str.Substring(0, i - 1) + "_._" + str.Substring(i +
                  2);
            } else if (spaceAfter) {
              str = str.Substring(0, i) + "._" + str.Substring(i + 2);
            } else if (spaceBefore) {
              str = str.Substring(0, i - 1) + "_." + str.Substring(i +
                 1);
            }
            break;
          }
        }
        if (str[str.Length - 1] == '.' || str[str.Length - 1] == '~') {
          // Ends in a dot or tilde (a file whose name ends with
          // the latter may be treated as
          // a backup file especially in Unix-based systems).
          // NOTE: Although concatenation of two NFC strings
          // doesn't necessarily lead to an NFC string, this
          // particular concatenation doesn't disturb the NFC
          // status of the string.
          str += "_";
        }
      } while (!oldstr.Equals(str));
      return str;
    }
  }
}
