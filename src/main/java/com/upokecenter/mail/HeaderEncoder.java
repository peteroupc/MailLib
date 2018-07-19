package com.upokecenter.mail;
/*
Written by Peter O. in 2014-2018.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import com.upokecenter.util.*;

  final class HeaderEncoder {
    private final StringBuilder builder;
    private int maxLineLength;
    private int column;
    private int startColumn;

    public HeaderEncoder() {
 this(76, 0); }

    public HeaderEncoder(int maxLineLength, int startColumn) {
      // Minimum supported max line length is 15 to accommodate
      // certain multi-character units that must appear together
      // on the same line
      if (maxLineLength >= 0 && maxLineLength < 15) {
        throw new UnsupportedOperationException();
      }
      builder = new StringBuilder();
      this.maxLineLength = maxLineLength;
      this.column = startColumn;
      this.startColumn = startColumn;
    }
    public void Reset(int column, int length) {
      this.column = column;
      if (length == 0) {
        this.builder.setLength(length);
      } else {
        String oldstring = this.builder.toString().substring(0, length);
        this.builder.setLength(0);
        this.builder.append(oldstring);
      }
    }
    public int GetColumn() {
      return this.column;
    }
    public int GetLength() {
      return this.builder.length();
    }
    public int GetMaxLineLength() {
      return this.maxLineLength;
    }
    public boolean CanFitSymbol(String symbol) {
      return this.maxLineLength < 0 || 1 + symbol.length() <= this.maxLineLength;
    }
    public boolean TryAppendSymbol(String symbol) {
      if (CanFitSymbol(symbol)) {
        AppendSymbol(symbol);
        return true;
      }
      return false;
    }
    public HeaderEncoder AppendBreak() {
      this.builder.append("\r\n ");
      this.column = 1;
      return this;
    }

    private boolean AppendSpaceAndSymbol(String symbol, int startIndex, int
      endIndex, boolean writeSpace) {
      if (startIndex == endIndex) {
        return writeSpace;
      }
      int spaceLength;
      spaceLength = writeSpace ? 1 : 0;
      if (maxLineLength < 0 || this.column + (endIndex - startIndex) +
        spaceLength <= this.maxLineLength) {
        if (writeSpace) {
          this.builder.append(" ");
        }
      this.builder.append(symbol.substring(startIndex, (startIndex)+(endIndex -
          startIndex)));
        this.column += (endIndex - startIndex) + spaceLength;
      } else {
        this.builder.append("\r\n ");
      this.builder.append(symbol.substring(startIndex, (startIndex)+(endIndex -
          startIndex)));
        this.column = 1 + (endIndex - startIndex);
      }
      return false;  // No need to write space anymore
    }

    public HeaderEncoder AppendString(String symbol) {
      return AppendString(symbol, 0, symbol.length());
    }

    public HeaderEncoder AppendString(String symbol, int startIndex, int
      endIndex) {
      if (symbol.length() > 0) {
        var i = startIndex;
        var symbolBegin = startIndex;
        boolean writeSpace = false;
        while (i < endIndex) {
          if (symbol.charAt(i) == '\r' && i + 1 < endIndex &&
               symbol.charAt(i + 1) == '\n') {
            writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                 writeSpace);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (symbol.charAt(i)=='<' || symbol.charAt(i)=='>' || symbol.charAt(i)==',' ||
                    symbol.charAt(i)==';' || symbol.charAt(i)==':') {
            // Additional characters between which linear white space can
            // freely appear
            // in structured header fields. They are the union of RFC 822's
            // specials
            // and RFC 2045's tspecials, with the exception of parentheses
            // (comment
            // delimiters), square brackets (domain literal delimiters),
            // double quote (quoted String delimiter), at-sign (better not
            // to separate
            // email addresses), and the backslash (since it serves as an escape
            // in some header fields).
            writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                 writeSpace);
            symbolBegin = i;
            ++i;
            continue;
          } else if (symbol.charAt(i) == '"' || symbol.charAt(i) == '[') {
            // May begin quoted-String or domain literal
            // (use ParseQuotedStringCore instead of
            // ParseQuotedString because it excludes optional CFWS at ends)
            int si = symbol.charAt(i) == '"' ?
                    HeaderParser.ParseQuotedStringCore(symbol, i, endIndex,
                null) : HeaderParser.ParseDomainLiteralCore(symbol, i, endIndex,
                    null);
            if (si != i) {
              writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                    writeSpace);
           AppendQuotedStringOrDomain(symbol.substring(i, (i)+(si - i)),
                writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
          } else if (symbol.charAt(i) == '(') {
            // May begin comment
            int si = HeaderParserUtility.ParseCommentLax(symbol, i, endIndex,
                    null);
            if (si != i) {
              writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                    writeSpace);
              AppendComment(symbol.substring(i, (i)+(si - i)), writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
          } else if (symbol.charAt(i) == ' ' || symbol.charAt(i) == '\t') {
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

    private void AppendComment(String symbol, boolean writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'comment'
      // and begins and ends with parentheses
      if (symbol.length() == 0 || symbol.charAt(0) != '(') throw new IllegalArgumentException();
      int i = 0;
      int symbolBegin = 0;
      while (i < symbol.length()) {
        if (symbol.charAt(i) == '\r' && i + 1 < symbol.length() && symbol.charAt(i + 1) == '\n'
) {
          writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          symbolBegin = i + 2;
          i += 2;
        } else if (symbol.charAt(i) == '(' || symbol.charAt(i) == ')') {
          writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          writeSpace = AppendSpaceAndSymbol(symbol, i, i + 1, writeSpace);
          symbolBegin = i + 1;
          ++i;
        } else if (symbol.charAt(i) == ' ' || symbol.charAt(i) == '\t') {
          AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          writeSpace = true;
          i = HeaderParser.ParseFWS(symbol, i, symbol.length(), null);
          symbolBegin = i;
        } else if (symbol.charAt(i) == '\\' && i + 1 < symbol.length()) {
          int cp = DataUtilities.CodePointAt(symbol, i + 1);
          int sym = (cp >= 0x10000) ? 3 : 2;
          i += sym;
        } else {
          ++i;
        }
      }
      AppendSpaceAndSymbol(symbol, symbolBegin, symbol.length(), writeSpace);
    }

    private boolean CanCharUnitFit(int currentWordLength, int unitLength, boolean
      writeSpace) {
  int effectiveMaxLength = 75;  // 75 is max. allowed length of an encoded
        word
      if (this.GetMaxLineLength() >= 0) {
        effectiveMaxLength = Math.min(
         effectiveMaxLength,
         this.GetMaxLineLength());
      }
      if (currentWordLength == 0) {
        // 12 characters for prologue and epilogue
        int extraSpace = 0;
        if (writeSpace) {
          extraSpace = 1;
        }
        return this.column + 12 + unitLength + extraSpace <= effectiveMaxLength;
      } else {
        return this.column + 2 + unitLength <= effectiveMaxLength;
      }
    }

    private static String TrimAllSpaceLine(String str) {
      // Keep a generated header field from having an all-whitespace
      // line at the end
      if (str.length() > 0) {
        int c = str.charAt(str.length() - 1);
        if (c != ' ' && c != '\t' && c != '\n') {
          return str;
        }
        String ret = str;
        for (var i = str.length() - 1; i >= 0; --i) {
          switch (str.charAt(i)) {
            case ' ':
            case '\t':
              break;
            case '\n':
              if (i > 0 && str.charAt(i - 1) == '\r') {
                --i;
                ret = str.substring(0, i);
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
      this.builder.append(c);
      ++this.column;
    }
    private static final String HexChars = "0123456789ABCDEF";
    private void AppendQEncoding(int ch) {
      this.builder.append('=');
      this.builder.append(HexChars.charAt((ch >> 4) & 15));
      this.builder.append(HexChars.charAt(ch & 15));
      this.column += 3;
    }

    public HeaderEncoder AppendAsEncodedWords(String symbol) {
      int i = 0;
      int currentWordLength = 0;
      while (i < symbol.length()) {
        int ch = DataUtilities.CodePointAt(symbol, i);
        if (ch >= 0x10000) {
          ++i;
        }
  boolean smallChar = ch < 0x80 && ch > 0x20 && ch != (char)'"' && ch !=
          (char)','&&
                "?()<>[]:;@\\.=_".indexOf((char)ch) < 0;
        int unitLength = 1;
        if (ch == 0x20 || smallChar) {
          unitLength = 1;
        } else if (ch <= 0x7f) {
          unitLength = 3;
        } else if (ch <= 0x7ff) {
          unitLength = 6;
        } else {
 unitLength = (ch <= 0xffff) ? (9) : (12);
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

    private void AppendQuotedStringOrDomain(String symbol, boolean writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'quoted-String'
      // and begins and ends with a double quote, or is a syntactically
      // valid domain literal
      // and begins and ends with opening/closing brackets
      if (symbol.length() == 0) {
        throw new IllegalArgumentException();
      }
      int i = 0;
      int symbolBegin = 0;
      while (i < symbol.length()) {
        if (symbol.charAt(i) == '\r' && i + 1 < symbol.length() && symbol.charAt(i + 1) == '\n'
) {
          writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          symbolBegin = i + 2;
          i += 2;
        } else if (symbol.charAt(i) == ' ') {
          AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          writeSpace = true;
          symbolBegin = i + 1;
          ++i;
        } else if (symbol.charAt(i) == '\\' && i + 1 < symbol.length()) {
          int cp = DataUtilities.CodePointAt(symbol, i + 1);
          int sym = (cp >= 0x10000) ? 3 : 2;
          i += sym;
        } else {
          ++i;
        }
      }
      AppendSpaceAndSymbol(symbol, symbolBegin, symbol.length(), writeSpace);
    }

    public HeaderEncoder AppendSpace() {
      if (maxLineLength < 0 || this.column + 1 <= this.maxLineLength) {
        this.builder.append(" ");
        ++this.column;
      } else {
        this.builder.append("\r\n ");
        this.column = 1;
      }
      return this;
    }
    // NOTE: Assumes that all symbols being appended
    // contain no unpaired surrogates and no line breaks
    public HeaderEncoder AppendSymbol(String symbol) {
      if (symbol.length() > 0) {
        if (maxLineLength < 0 || this.column + symbol.length() <=
                this.maxLineLength) {
          this.builder.append(symbol);
          this.column += symbol.length();
        } else {
          this.builder.append("\r\n ");
          this.builder.append(symbol);
          this.column = 1 + symbol.length();
        }
      }
      return this;
    }

    public static String CapitalizeHeaderField(String s) {
      StringBuilder builder = new StringBuilder();
      boolean afterHyphen = true;
      for (int i = 0; i < s.length(); ++i) {
        if (afterHyphen && s.charAt(i) >= 'a' && s.charAt(i) <= 'z') {
          builder.append((char)(s.charAt(i) - 0x20));
        } else {
          builder.append(s.charAt(i));
        }
        afterHyphen = s.charAt(i) == '-';
      }
      String ret = builder.toString();
      return ret.equals("Mime-Version") ? "MIME-Version" :
        (ret.equals("Message-Id") ? "Message-ID" : ret);
    }

    public HeaderEncoder AppendFieldName(String fieldName) {
      this.AppendSymbol(CapitalizeHeaderField(fieldName) + ":");
      this.AppendSpace();
      return this;
    }

    public static String TrimLeadingFWS(String fieldValue) {
      var fws = HeaderParser.ParseFWS(fieldValue, 0, fieldValue.length(), null);
      if (fws > 0) {
        fieldValue = fieldValue.substring(fws);
      }
      return fieldValue;
    }

    public static String EncodeFieldAsEncodedWords(String fieldName, String
        fieldValue) {
      HeaderEncoder sa = new HeaderEncoder().AppendFieldName(fieldName);
      sa.AppendAsEncodedWords(TrimLeadingFWS(fieldValue));
      return sa.toString();
    }
    public static String EncodeField(String fieldName, String
        fieldValue) {
      HeaderEncoder sa = new HeaderEncoder().AppendFieldName(fieldName);
      sa.AppendString(TrimLeadingFWS(fieldValue));
      return sa.toString();
    }
    @Override public String toString() {
      return TrimAllSpaceLine(this.builder.toString());
    }
  }
