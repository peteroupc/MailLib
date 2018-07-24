/*
Written by Peter O. in 2014-2018.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Text;
using PeterO;

namespace PeterO.Mail {
  internal sealed class HeaderEncoder {
    private readonly StringBuilder builder;
    private int maxLineLength;
    private int column;
    private int startColumn;

    public HeaderEncoder() : this(Message.MaxRecHeaderLineLength, 0) { }

    public HeaderEncoder(int maxLineLength, int startColumn) {
      // Minimum supported max line length is 15 to accommodate
      // certain multi-character units that must appear together
      // on the same line
      if (maxLineLength >= 0 && maxLineLength < 15) {
        throw new NotSupportedException();
      }
      builder = new StringBuilder();
      this.maxLineLength = maxLineLength;
      this.column = startColumn;
      this.startColumn = startColumn;
    }

    public void Reset() {
      Reset(this.startColumn, 0);
    }
    public void Reset(int column, int length) {
      this.column = column;
      if (length == 0) {
        this.builder.Length = length;
      } else {
        string oldstring = this.builder.ToString().Substring(0, length);
        this.builder.Length = 0;
        this.builder.Append(oldstring);
      }
    }
    public int GetColumn() {
      return this.column;
    }
    public int GetLength() {
      return this.builder.Length;
    }
    public int GetMaxLineLength() {
      return this.maxLineLength;
    }
    public bool CanFitSymbol(string symbol) {
      return this.maxLineLength < 0 || 1 + symbol.Length <= this.maxLineLength;
    }
    public bool TryAppendSymbol(string symbol) {
      if (CanFitSymbol(symbol)) {
        AppendSymbol(symbol);
        return true;
      }
      return false;
    }
    public HeaderEncoder AppendBreak() {
      this.builder.Append("\r\n ");
      this.column = 1;
      return this;
    }

    private bool AppendSpaceAndSymbol(string symbol, int startIndex, int
      endIndex, bool writeSpace) {
      if (startIndex == endIndex) {
        return writeSpace;
      }
      int spaceLength;
      spaceLength = writeSpace ? 1 : 0;
      if (maxLineLength < 0 || this.column + (endIndex - startIndex) +
        spaceLength <= this.maxLineLength) {
        if (writeSpace) {
          this.builder.Append(" ");
        }
        this.builder.Append(symbol.Substring(startIndex, endIndex -
            startIndex));
        this.column += (endIndex - startIndex) + spaceLength;
      } else {
        this.builder.Append("\r\n ");
        this.builder.Append(symbol.Substring(startIndex, endIndex -
            startIndex));
        this.column = 1 + (endIndex - startIndex);
      }
      return false;  // No need to write space anymore
    }

    public HeaderEncoder AppendString(string symbol) {
      return AppendString(symbol, 0, symbol.Length);
    }

    public HeaderEncoder AppendString(string symbol, int startIndex, int
  endIndex) {
      return AppendString(symbol, startIndex, endIndex, true);
    }

      public HeaderEncoder AppendString(string symbol, int startIndex, int
                    endIndex, bool structured) {
      if (symbol.Length > 0) {
        int i = startIndex;
        int symbolBegin = startIndex;
        var writeSpace = false;
        while (i < endIndex) {
          if (symbol[i] == '\r' && i + 1 < endIndex &&
               symbol[i + 1] == '\n') {
            writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                 writeSpace);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (structured && (symbol[i] == '<' || symbol[i] == '>' ||
            symbol[i] == ',' ||
                    symbol[i] == ';')) {
            // Additional characters between which linear white space can
            // freely appear
            // in structured header fields. They are the union of RFC 822's
            // specials
            // and RFC 2045's tspecials, with the exception of parentheses
            // (comment
            // delimiters), square brackets (domain literal delimiters),
            // double quote (quoted string delimiter), at-sign (better not
            // to separate
            // email addresses), colon (separates components of times and IPv6
            // addresses), and the backslash (since it serves as an escape
            // in some header fields).
            writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                 writeSpace);
            symbolBegin = i;
            ++i;
            continue;
          } else if (structured && (symbol[i] == '"' || symbol[i] == '[')) {
            // May begin quoted-string or domain literal
            // (use ParseQuotedStringCore instead of
            // ParseQuotedString because it excludes optional CFWS at ends)
            int si = symbol[i] == '"' ?
                    HeaderParser.ParseQuotedStringCore(symbol, i, endIndex,
                null) : HeaderParser.ParseDomainLiteralCore(symbol, i, endIndex,
                    null);
            if (si != i) {
              writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                    writeSpace);
              AppendQuotedStringOrDomain(symbol.Substring(i, si - i),
                   writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
          } else if (structured && symbol[i] == '(') {
            // May begin comment
            int si = HeaderParserUtility.ParseCommentLax(symbol, i, endIndex,
                    null);
            if (si != i) {
              writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                    writeSpace);
              AppendComment(symbol.Substring(i, si - i), writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
          } else if (symbol[i] == ' ' && i+1 < endIndex && symbol[i+1]!='\t' &&
                    symbol[i+1]!='\r' && symbol[i+1]!=' ') {
            AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
            writeSpace = true;
            i = HeaderParser.ParseFWS(symbol, i, endIndex, null);
            symbolBegin = i;
          } else if (symbol[i] == ' ' || symbol[i] == '\t') {
            //DebugUtility.Log("Special whitespace|" + symbol.Substring(i,
            // endIndex - i));
            AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
            writeSpace = true;
            i = HeaderParser.ParseFWS(symbol, i, endIndex, null);
            symbolBegin = i;
          } else {
            ++i;
          }
        }
        writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin,
                    endIndex, writeSpace);
        if (writeSpace) {
          AppendSpace();
        }
      }
      return this;
    }

    private bool SimpleAppendString(string symbol, int startIndex, int
  endIndex) {
      if (symbol.Length > 0) {
        int i = startIndex;
        int symbolBegin = startIndex;
        while (i < endIndex) {
          if (symbol[i] == '\r' && i + 1 < endIndex &&
               symbol[i + 1] == '\n') {
            AppendSpaceAndSymbol(symbol, symbolBegin, i, false);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (symbol[i] == '"') {
            return false;
          } else if (symbol[i] == '[') {
            int j = i + 1;
            while (j < endIndex) {
              if (symbol[j] == ' ' || symbol[j] == '\t' || symbol[j] == '\r') {
 return false;
  } else if (symbol[j] == ']') {
 break;
} else {
 ++j;
}
            }
            return false;
    } else if (symbol[i] == ' ' && i + 1 < endIndex && symbol[i + 1] != '\t'
            &&
                    symbol[i + 1] != '\r' && symbol[i + 1] != ' ') {
            AppendSpaceAndSymbol(symbol, symbolBegin, i, false);
            AppendSpace();
            symbolBegin = i + 1;
            ++i;
          } else if (symbol[i] == ' ' || symbol[i] == '\t') {
 /*DebugUtility.Log("Special whitespace|" + symbol.Substring(i, endIndex -
              i));
            */ AppendSpaceAndSymbol(symbol, symbolBegin, i, false);
            AppendBreak();
            symbolBegin = i;
            ++i;
            while (i < endIndex) {
              if (symbol[i] == ' ' || symbol[i] == '\t') {
 ++i;
} else {
 break;
}
            }
          } else {
            ++i;
          }
        }
        AppendSpaceAndSymbol(symbol, symbolBegin, endIndex, false);
      }
      return true;
    }

    private void AppendComment(string symbol, bool writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'comment'
      // and begins and ends with parentheses
      if (symbol.Length == 0 || symbol[0] != '(') throw new ArgumentException();
      var i = 0;
      var symbolBegin = 0;
      while (i < symbol.Length) {
        if (symbol[i] == '\r' && i + 1 < symbol.Length && symbol[i + 1] == '\n'
) {
          writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          symbolBegin = i + 2;
          i += 2;
        } else if (symbol[i] == '(' || symbol[i] == ')') {
          writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          writeSpace = AppendSpaceAndSymbol(symbol, i, i + 1, writeSpace);
          symbolBegin = i + 1;
          ++i;
        } else if (symbol[i] == ' ' || symbol[i] == '\t') {
          AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          writeSpace = true;
          i = HeaderParser.ParseFWS(symbol, i, symbol.Length, null);
          symbolBegin = i;
        } else if (symbol[i] == '\\' && i + 1 < symbol.Length) {
          int cp = DataUtilities.CodePointAt(symbol, i + 1);
          int sym = (cp >= 0x10000) ? 3 : 2;
          i += sym;
        } else {
          ++i;
        }
      }
      AppendSpaceAndSymbol(symbol, symbolBegin, symbol.Length, writeSpace);
    }

    private bool CanCharUnitFit(int currentWordLength, int unitLength, bool
      writeSpace) {
      // 75 is max. allowed length of an encoded word
      var effectiveMaxLength = 75;
      if (this.GetMaxLineLength() >= 0) {
        effectiveMaxLength = Math.Min(
         effectiveMaxLength,
         this.GetMaxLineLength());
      }
      if (currentWordLength == 0) {
        // 12 characters for prologue and epilogue
        var extraSpace = 0;
        if (writeSpace) {
          extraSpace = 1;
        }
        return this.column + 12 + unitLength + extraSpace <= effectiveMaxLength;
      } else {
        return this.column + 2 + unitLength <= effectiveMaxLength;
      }
    }

    private static string TrimAllSpaceLine(string str) {
      // Keep a generated header field from having an all-whitespace
      // line at the end
      if (str.Length > 0) {
        int c = str[str.Length - 1];
        if (c != ' ' && c != '\t' && c != '\n') {
          return str;
        }
        string ret = str;
        for (int i = str.Length - 1; i >= 0; --i) {
          switch ((int)str[i]) {
            case 0x20:  // space
            case 0x09:  // tab
              break;
            case 0x0a:  // LF
              if (i > 0 && str[i - 1] == '\r') {
                --i;
                ret = str.Substring(0, i);
              } else {
                return ret;
              }
              break;
            default: return ret;
          }
        }
        return ret;
      }
      return str;
    }

    private void AppendOne(char c) {
      this.builder.Append(c);
      ++this.column;
    }
    private const string HexChars = "0123456789ABCDEF";
    private void AppendQEncoding(int ch) {
      this.builder.Append('=');
      this.builder.Append(HexChars[(ch >> 4) & 15]);
      this.builder.Append(HexChars[ch & 15]);
      this.column += 3;
    }

    private static readonly int[] smallchars = {
      0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0,
  0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
  0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 };

    public HeaderEncoder AppendAsEncodedWords(string symbol) {
      var i = 0;
      var currentWordLength = 0;
      while (i < symbol.Length) {
        int ch = DataUtilities.CodePointAt(symbol, i);
        if (ch >= 0x10000) {
          ++i;
        }
        bool smallChar = ch <= 0x7e && ch > 0x20 && smallchars[ch - 0x20] == 1;
        var unitLength = 1;
        if (ch == 0x20 || smallChar) {
          unitLength = 1;
        } else {
     unitLength = (ch <= 0x7f) ? (3) : ((ch <= 0x7ff) ? (6) : ((ch <=
            0xffff) ? (9) : (12)));
        }
        if (!CanCharUnitFit(currentWordLength, unitLength, false)) {
          if (currentWordLength > 0) {
            AppendSymbol("?=");
            if (CanCharUnitFit(0, unitLength, true)) {
              AppendSpace();
            } else {
              AppendBreak();
            }
          } else {
            AppendBreak();
          }
          AppendSymbol("=?utf-8?Q?");
          currentWordLength = 12;
        } else if (currentWordLength == 0) {
          AppendSymbol("=?utf-8?Q?");
          currentWordLength = 12;
        }
        if (ch == 0x20) {
          this.AppendOne('_');
        } else if (smallChar) {
          this.AppendOne((char)ch);
        } else if (ch <= 0x7f) {
          this.AppendQEncoding(ch);
        } else if (ch <= 0x7ff) {
          this.AppendQEncoding(0xc0 | ((ch >> 6) & 0x1f));
          this.AppendQEncoding(0x80 | (ch & 0x3f));
        } else if (ch <= 0xffff) {
          this.AppendQEncoding(0xe0 | ((ch >> 12) & 0x0f));
          this.AppendQEncoding(0x80 | ((ch >> 6) & 0x3f));
          this.AppendQEncoding(0x80 | (ch & 0x3f));
        } else {
          this.AppendQEncoding(0xf0 | ((ch >> 18) & 0x07));
          this.AppendQEncoding(0x80 | ((ch >> 12) & 0x3f));
          this.AppendQEncoding(0x80 | ((ch >> 6) & 0x3f));
          this.AppendQEncoding(0x80 | (ch & 0x3f));
        }
        currentWordLength += unitLength;
        ++i;
      }
      if (currentWordLength > 0) {
        AppendSymbol("?=");
      }
      return this;
    }

    private static readonly byte[] Base64Classic = {
  0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d,
  0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a,
  0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d,
  0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a,
        0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x2b, 0x2f
      };

    private void AppendFinalBase64(int[] b64) {
      int b1 = b64[0];
      int b2 = b64[1];
      int quantumCount = b64[2];
      if (quantumCount == 2) {
        this.builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
        this.builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4) &
          15)]);
        this.builder.Append((char)Base64Classic[((b2 & 15) << 2)]);
        this.builder.Append('=');
        this.column += 4;
      } else if (quantumCount == 1) {
        this.builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
        this.builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4) &
          15)]);
        this.builder.Append("==");
        this.column += 4;
      }
      b64[2] = 0;
    }

    private void AppendBase64(int[] b64, int value) {
      int quantumCount = b64[2];
      if (quantumCount == 2) {
        int b1 = b64[0];
        int b2 = b64[1];
        this.builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
        this.builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4) &
          15)]);
        this.builder.Append((char)Base64Classic[((b2 & 15) << 2) + ((value>>
          6) & 3)]);
        this.builder.Append((char)Base64Classic[value & 63]);
        this.column += 4;
        b64[2] = 0;
      } else if (quantumCount == 1) {
        b64[1] = value;
        b64[2] = 2;
      } else if (quantumCount == 0) {
        b64[0] = value;
        b64[2] = 1;
      }
    }

    public HeaderEncoder AppendAsEncodedWordsB(string symbol) {
      var i = 0;
      var currentWordLength = 0;
      var base64state = new int[] { 0, 0, 0 };
      while (i < symbol.Length) {
        int ch = DataUtilities.CodePointAt(symbol, i);
        if (ch >= 0x10000) {
          ++i;
        }
 var unitLength = (ch <= 0x7f) ? (1) : ((ch <= 0x7ff) ? (2) : ((ch <=
          0xffff) ? (3) : (4)));
        var bytesNeeded = 4 + (base64state[2] + unitLength > 3 ? 4 : 0);
        if (!CanCharUnitFit(currentWordLength, bytesNeeded, false)) {
          if (currentWordLength > 0) {
            AppendFinalBase64(base64state);
            AppendSymbol("?=");
            if (CanCharUnitFit(0, unitLength, true)) {
              AppendSpace();
            } else {
              AppendBreak();
            }
          } else {
            AppendBreak();
          }
          AppendSymbol("=?utf-8?B?");
          currentWordLength = 12;
        } else if (currentWordLength == 0) {
          AppendSymbol("=?utf-8?B?");
          currentWordLength = 12;
        }
        if (ch <= 0x7f) {
          AppendBase64(base64state, ch);
        } else if (ch <= 0x7ff) {
          AppendBase64(base64state, 0xc0 | ((ch >> 6) & 0x1f));
          AppendBase64(base64state, 0x80 | (ch & 0x3f));
        } else if (ch <= 0xffff) {
          AppendBase64(base64state, 0xe0 | ((ch >> 12) & 0x0f));
          AppendBase64(base64state, 0x80 | ((ch >> 6) & 0x3f));
          AppendBase64(base64state, 0x80 | (ch & 0x3f));
        } else {
          AppendBase64(base64state, 0xf0 | ((ch >> 18) & 0x07));
          AppendBase64(base64state, 0x80 | ((ch >> 12) & 0x3f));
          AppendBase64(base64state, 0x80 | ((ch >> 6) & 0x3f));
          AppendBase64(base64state, 0x80 | (ch & 0x3f));
        }
        currentWordLength += bytesNeeded - 4;
        ++i;
      }
      if (currentWordLength > 0) {
        AppendFinalBase64(base64state);
        AppendSymbol("?=");
      }
      return this;
    }

    private void AppendQuotedStringOrDomain(string symbol, bool writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'quoted-string'
      // and begins and ends with a double quote, or is a syntactically
      // valid domain literal
      // and begins and ends with opening/closing brackets
      if (symbol.Length == 0) {
        throw new ArgumentException();
      }
      var i = 0;
      var symbolBegin = 0;
      while (i < symbol.Length) {
        if (symbol[i] == '\r' && i + 1 < symbol.Length &&
           symbol[i + 1] == '\n') {
          writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          symbolBegin = i + 2;
          i += 2;
        } else if (symbol[i] == ' ' || symbol[i] == '\t') {
          AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          AppendSpaceOrTab(symbol[i]);
          writeSpace = false;
          symbolBegin = i + 1;
          ++i;
        } else if (symbol[i] == '\\' && i + 1 < symbol.Length) {
          int cp = DataUtilities.CodePointAt(symbol, i + 1);
          int sym = (cp >= 0x10000) ? 3 : 2;
          i += sym;
        } else {
          ++i;
        }
      }
      AppendSpaceAndSymbol(symbol, symbolBegin, symbol.Length, writeSpace);
    }

    private HeaderEncoder AppendSpaceOrTab(char ch) {
      if (maxLineLength < 0 || this.column + 1 <= this.maxLineLength) {
        this.builder.Append(ch);
        ++this.column;
      } else {
        this.builder.Append("\r\n");
        this.builder.Append(ch);
        this.column = 1;
      }
      return this;
    }

    public HeaderEncoder AppendSpaceOrTabIfNeeded(char ch) {
      if (this.builder.Length == 0) {
        return AppendSpaceOrTab(ch);
      }
      bool endsWithSpace = (
        this.builder[this.builder.Length - 1] == 0x20 || this.builder[this.builder.Length - 1] == 0x09);
      if (!endsWithSpace) {
        AppendSpaceOrTab(ch);
      }
      return this;
    }

    public HeaderEncoder AppendSpaceIfNeeded() {
      return AppendSpaceOrTabIfNeeded(' ');
    }

    public HeaderEncoder AppendSpace() {
      return AppendSpaceOrTab(' ');
    }
    // NOTE: Assumes that all symbols being appended
    // contain no unpaired surrogates and no line breaks
    public HeaderEncoder AppendSymbol(string symbol) {
      if (symbol.Length > 0) {
        if (maxLineLength < 0 || this.column + symbol.Length <=
                this.maxLineLength) {
          this.builder.Append(symbol);
          this.column += symbol.Length;
        } else {
          if (this.column>1) {
 this.builder.Append("\r\n ");
}
          this.builder.Append(symbol);
          this.column = 1 + symbol.Length;
        }
      }
      return this;
    }

    // Returns true only if:
    // * Header field has a name followed by colon followed by SP
    // * Header field's value matches the production "unstructured"
    // in RFC 5322 without any obsolete syntax
    // * Each line is no more than MaxRecHeaderLineLength characters in length,
    // except that a line with no whitespace other than leading and trailing may
    // go up to MaxHardHeaderLineLength characters in length
    // * Value has no all-whitespace or blank lines
    // * Text has only printable ASCII characters, CR,
    // LF, and/or TAB
    public static bool CanOutputRaw(string s) {
      var foundColon = false;
      int len = s.Length;
      var chunkLength = 0;
      for (var i = 0; i < len; ++i) {
        if (s[i] == ':') {
          foundColon = true;
          if (i + 1 >= len || s[i + 1] != 0x20) {
            // Colon not followed by SPACE (0x20)
            return false;
          }
          chunkLength = i + 2;
          if (chunkLength > Message.MaxRecHeaderLineLength) {
            return false;
          }
          break;
        }
        if (s[i] == 0x0d || s[i] == 0x09 || s[i] == 0x20) {
          return false;
        }
      }
      if (!foundColon) {
        return false;
      }
      var whitespaceState = 3;
      for (int i = chunkLength; i < len;) {
        if (s[i] == 0x0d) {
          if (i + 2 >= len || s[i + 1] != 0x0a || (s[i + 2] != 0x09 && s[i +
            2] != 0x20)) {
            // bare CR, or CRLF not followed by SP/TAB
            return false;
          }
          i += 3;
          chunkLength = 1;
          whitespaceState = 1;
          var found = false;
          for (int j = i; j < len; ++j) {
            if (s[j] != 0x09 && s[j] != 0x20 && s[j] != 0x0d) {
              found = true; break;
            } else if (s[j]==0x0d) {
              // Possible CRLF after all-whitespace line
              return false;
            }
          }
          if (!found) {
            // CRLF followed by an all-whitespace line
            return false;
          }
        } else {
          char c = s[i];
          if (c >= 0x7f || (c < 0x20 && c != 0x09 && c != 0x0d)) {
            // CTLs (except TAB, SPACE, and CR) and non-ASCII
            // characters
            return false;
          }
          ++i;
          ++chunkLength;
          if (c == 0x09 || c == 0x20) {
            // 1 = Whitespace at start of line
            // 2 = Nonwhitespace after initial whitespace
            // 3 = After nonwhitespace
            if (whitespaceState == 2) {
 whitespaceState = 3;
}
          } else {
            if (whitespaceState == 1) {
 whitespaceState = 2;
}
          }
          if (whitespaceState < 3) {
            if (chunkLength > Message.MaxHardHeaderLineLength) {
              return false;
            }
          } else {
            if (chunkLength > Message.MaxRecHeaderLineLength) {
              return false;
            }
          }
        }
      }
      return true;
    }

    private static string CapitalizeHeaderField(string s) {
      var builder = new StringBuilder();
      var afterHyphen = true;
      for (int i = 0; i < s.Length; ++i) {
        if (afterHyphen && s[i] >= 'a' && s[i] <= 'z') {
          builder.Append((char)(s[i] - 0x20));
        } else {
          builder.Append(s[i]);
        }
        afterHyphen = s[i] == '-';
      }
      string ret = builder.ToString();
      return ret.Equals("Mime-Version") ? "MIME-Version" :
        (ret.Equals("Message-Id") ? "Message-ID" : ret);
    }

    public HeaderEncoder AppendFieldName(string fieldName) {
      this.AppendSymbol(CapitalizeHeaderField(fieldName) + ":");
      this.AppendSpace();
      return this;
    }

    public static string TrimLeadingFWS(string fieldValue) {
      int fws = HeaderParser.ParseFWS(fieldValue, 0, fieldValue.Length, null);
      if (fws > 0) {
        fieldValue = fieldValue.Substring(fws);
      }
      return fieldValue;
    }

    public static string EncodeFieldAsEncodedWords(string fieldName, string
        fieldValue) {
      var sa = new HeaderEncoder().AppendFieldName(fieldName);
      sa.AppendAsEncodedWords(TrimLeadingFWS(fieldValue));
      return sa.ToString();
    }
    public static string EncodeField(string fieldName, string
        fieldValue) {
      bool structured = HeaderFieldParsers.GetParser(fieldName).IsStructured();
      var trialField = CapitalizeHeaderField(fieldName) + ": " + fieldValue;
      if (CanOutputRaw(trialField)) {
 return trialField;
}
      var sa = new HeaderEncoder().AppendFieldName(fieldName);
      if (sa.SimpleAppendString(fieldValue, 0, fieldValue.Length)) {
        trialField = sa.ToString();
        if (CanOutputRaw(trialField)) {
          return trialField;
        }
      }
      //DebugUtility.Log("Must wrap '" + fieldName + "'");
      //DebugUtility.Log(fieldValue);
      sa = new HeaderEncoder().AppendFieldName(fieldName);
      fieldValue = TrimLeadingFWS(fieldValue);
      sa.AppendString(fieldValue, 0, fieldValue.Length, structured);
      return sa.ToString();
    }
    public override string ToString() {
      return TrimAllSpaceLine(this.builder.ToString());
    }
  }
}
