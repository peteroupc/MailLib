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
      if (symbol.Length > 0) {
        var i = startIndex;
        var symbolBegin = startIndex;
        var writeSpace = false;
        while (i < endIndex) {
          if (symbol[i] == '\r' && i + 1 < endIndex &&
               symbol[i + 1] == '\n') {
            writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                 writeSpace);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (symbol[i]=='<' || symbol[i]=='>' || symbol[i]==',' ||
                    symbol[i]==';' || symbol[i]==':') {
            // Additional characters between which linear white space can
            // freely appear
            // in structured header fields. They are the union of RFC 822's
            // specials
            // and RFC 2045's tspecials, with the exception of parentheses
            // (comment
            // delimiters), square brackets (domain literal delimiters),
            // double quote (quoted string delimiter), at-sign (better not
            // to separate
            // email addresses), and the backslash (since it serves as an escape
            // in some header fields).
            writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                 writeSpace);
            symbolBegin = i;
            ++i;
            continue;
          } else if (symbol[i] == '"' || symbol[i] == '[') {
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
          } else if (symbol[i] == '(') {
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
          } else if (symbol[i] == ' ' || symbol[i] == '\t') {
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
        for (var i = str.Length - 1; i >= 0; --i) {
          switch (str[i]) {
            case ' ':
            case '\t':
              break;
            case '\n':
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

    public HeaderEncoder AppendAsEncodedWords(string symbol) {
      var i = 0;
      var currentWordLength = 0;
      while (i < symbol.Length) {
        int ch = DataUtilities.CodePointAt(symbol, i);
        if (ch >= 0x10000) {
          ++i;
        }
  bool smallChar = ch < 0x80 && ch > 0x20 && ch != (char)'"' && ch !=
          (char)','&& "?()<>[]:;@\\.=_".IndexOf((char)ch) < 0;
        var unitLength = 1;
        if (ch == 0x20 || smallChar) {
          unitLength = 1;
        } else if (ch <= 0x7f) {
          unitLength = 3;
        } else {
 unitLength = (ch <= 0x7ff) ? (6) : ((ch <= 0xffff) ? (9) : (12));
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
        if (symbol[i] == '\r' && i + 1 < symbol.Length && symbol[i + 1] == '\n'
) {
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

    public HeaderEncoder AppendSpace() {
      if (maxLineLength < 0 || this.column + 1 <= this.maxLineLength) {
        this.builder.Append(" ");
        ++this.column;
      } else {
        this.builder.Append("\r\n ");
        this.column = 1;
      }
      return this;
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
          this.builder.Append("\r\n ");
          this.builder.Append(symbol);
          this.column = 1 + symbol.Length;
        }
      }
      return this;
    }

    public static string CapitalizeHeaderField(string s) {
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
      var fws = HeaderParser.ParseFWS(fieldValue, 0, fieldValue.Length, null);
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
      var sa = new HeaderEncoder().AppendFieldName(fieldName);
      sa.AppendString(TrimLeadingFWS(fieldValue));
      return sa.ToString();
    }
    public override string ToString() {
      return TrimAllSpaceLine(this.builder.ToString());
    }
  }
}
