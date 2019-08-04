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
    private static final String HexChars = "0123456789ABCDEF";
    private static final String Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghi" +
            "jklmnopqrstuvwxyz0123456789+/";

    private final StringBuilder builder;
    private int maxLineLength;
    private int column;
    private int startColumn;

    public HeaderEncoder() {
 this(Message.MaxRecHeaderLineLength, 0);
    }

    public HeaderEncoder(int maxLineLength, int startColumn) {
      // Minimum supported max line length is 15 to accommodate
      // certain multi-character units that must appear together
      // on the same line
      if (maxLineLength >= 0 && maxLineLength < 15) {
        throw new UnsupportedOperationException();
      }
      this.builder = new StringBuilder();
      this.maxLineLength = maxLineLength;
      this.column = startColumn;
      this.startColumn = startColumn;
    }

    public void Reset() {
      this.Reset(this.startColumn, 0);
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
      if (this.CanFitSymbol(symbol)) {
        this.AppendSymbol(symbol);
        return true;
      }
      return false;
    }

    public HeaderEncoder AppendBreak() {
      this.builder.append("\r\n ");
      this.column = 1;
      return this;
    }

    private boolean AppendSpaceAndSymbol(
      String symbol,
      int startIndex,
      int endIndex,
      boolean writeSpace) {
      if (startIndex == endIndex) {
        return writeSpace;
      }
      int spaceLength;
      spaceLength = writeSpace ? 1 : 0;
      if (this.maxLineLength < 0 || this.column + (endIndex - startIndex) +
        spaceLength <= this.maxLineLength) {
        if (writeSpace) {
          this.builder.append(" ");
        }
        this.builder.append(
  symbol.substring(
    startIndex, (
    startIndex)+(endIndex - startIndex)));
        this.column += (endIndex - startIndex) + spaceLength;
      } else {
        this.builder.append("\r\n ");
        this.builder.append(
  symbol.substring(
    startIndex, (
    startIndex)+(endIndex - startIndex)));
        this.column = 1 + (endIndex - startIndex);
      }
      return false; // No need to write space anymore
    }

    public HeaderEncoder AppendString(String symbol) {
      return this.AppendString(symbol, 0, symbol.length());
    }

    public HeaderEncoder AppendString(
      String symbol,
      int startIndex,
      int endIndex) {
      return this.AppendString(symbol, startIndex, endIndex, true);
    }

    public HeaderEncoder AppendString(
      String symbol,
      int startIndex,
      int endIndex,
      boolean structured) {
      if (symbol.length() > 0) {
        int i = startIndex;
        int symbolBegin = startIndex;
        boolean writeSpace = false;
        while (i < endIndex) {
          if (symbol.charAt(i) == '\r' && i + 1 < endIndex &&
               symbol.charAt(i + 1) == '\n') {
            writeSpace = this.AppendSpaceAndSymbol(
              symbol,
              symbolBegin,
              i,
              writeSpace);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (structured && (symbol.charAt(i) == '<' || symbol.charAt(i) == '>' ||
            symbol.charAt(i) == ',' || symbol.charAt(i) == ';')) {
            // Additional characters between which linear white space can
            // freely appear
            // in structured header fields. They are the intersection of RFC
            // 822's
            // specials and RFC 2045's tspecials, with the exception of
            // parentheses
            // (comment
            // delimiters), square brackets (domain literal delimiters),
            // double quote (quoted String delimiter), at-sign (better not
            // to separate
            // email addresses), colon (separates components of times and IPv6
            // addresses), and the backslash (since it serves as an escape
            // in some header fields).
            writeSpace = this.AppendSpaceAndSymbol(
              symbol,
              symbolBegin,
              i,
              writeSpace);
            symbolBegin = i;
            ++i;
            continue;
          } else if (structured && (symbol.charAt(i) == '"' || symbol.charAt(i) == '[')) {
            // May begin quoted-String or domain literal
            // (use ParseQuotedStringCore instead of
            // ParseQuotedString because it excludes optional CFWS at ends)
         int si = symbol.charAt(i) == '"' ? HeaderParserUtility.ParseQuotedStringCore(
           symbol,
           i,
           endIndex) : HeaderParser.ParseDomainLiteralCore(
     symbol,
     i,
     endIndex,
     null);
     if (si != i) {
              writeSpace = this.AppendSpaceAndSymbol(
                symbol,
                symbolBegin,
                i,
                writeSpace);
              this.AppendQuotedStringOrDomain(
  symbol.substring(i, (i)+(si - i)),
  writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
          } else if (structured && symbol.charAt(i) == '(') {
            // May begin comment
            int si = HeaderParserUtility.ParseCommentLax(
              symbol,
              i,
              endIndex,
              null);
            if (si != i) {
              writeSpace = this.AppendSpaceAndSymbol(
                symbol,
                symbolBegin,
                i,
                writeSpace);
              this.AppendComment(symbol.substring(i, (i)+(si - i)), writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
    } else if (symbol.charAt(i) == ' ' && i + 1 < endIndex && symbol.charAt(i + 1) != '\t' &&
            symbol.charAt(i + 1) != '\r' && symbol.charAt(i + 1) != ' ') {
      this.AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
      writeSpace = true;
      i = HeaderParser.ParseFWS(symbol, i, endIndex, null);
      symbolBegin = i;
    } else if (symbol.charAt(i) == ' ' || symbol.charAt(i) == '\t') {
            // DebugUtility.Log("Special whitespace|" + symbol.substring(i,(i)+(// endIndex - i)));
            this.AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
            writeSpace = true;
            i = HeaderParser.ParseFWS(symbol, i, endIndex, null);
            symbolBegin = i;
          } else {
            ++i;
          }
        }
        writeSpace = this.AppendSpaceAndSymbol(
          symbol,
          symbolBegin,
          endIndex,
          writeSpace);
        if (writeSpace) {
          this.AppendSpace();
        }
      }
      return this;
    }

    private boolean SimpleAppendString(
      String symbol,
      int startIndex,
      int endIndex) {
      if (symbol.length() > 0) {
        int i = startIndex;
        int symbolBegin = startIndex;
        boolean writeSpace = false;
        while (i < endIndex) {
          if (symbol.charAt(i) == '\r' && i + 1 < endIndex &&
               symbol.charAt(i + 1) == '\n') {
            this.AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (symbol.charAt(i) == '"') {
            return false;
          } else if (symbol.charAt(i) == '[') {
            int j = i + 1;
            while (j < endIndex) {
              if (symbol.charAt(j) == ' ' || symbol.charAt(j) == '\t' || symbol.charAt(j) == '\r') {
                return false;
              } else if (symbol.charAt(j) == ']') {
                break;
              } else {
                ++j;
              }
            }
            return false;
          } else if (symbol.charAt(i) == ' ' && i + 1 < endIndex && symbol.charAt(i + 1) !=
'\t' &&
            symbol.charAt(i + 1) != '\r' && symbol.charAt(i + 1) != ' ') {
            // Single space followed by character other than CR/LF/Tab
            this.AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
            // AppendSpace();
            writeSpace = true;
            symbolBegin = i + 1;
            ++i;
          } else if (symbol.charAt(i) == ' ' || symbol.charAt(i) == '\t') {
            /*DebugUtility.Log("Special whitespace|" + symbol.substring(i,(i)+(endIndex - i)));
               */
            this.AppendSpaceAndSymbol(
              symbol,
              symbolBegin,
              i,
              writeSpace);
            this.AppendBreak();
            symbolBegin = i;
            ++i;
            while (i < endIndex) {
              if (symbol.charAt(i) == ' ' || symbol.charAt(i) == '\t') {
                ++i;
              } else {
                break;
              }
            }
          } else {
            ++i;
          }
        }
        this.AppendSpaceAndSymbol(
          symbol,
          symbolBegin,
          endIndex,
          writeSpace);
      }
      return true;
    }

    private void AppendComment(String symbol, boolean writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'comment'
      // and begins and ends with parentheses
      if (symbol.length() == 0 || symbol.charAt(0) != '(') {
        throw new IllegalArgumentException();
      }
      int i = 0;
      int symbolBegin = 0;
      while (i < symbol.length()) {
        if (symbol.charAt(i) == '\r' &&
            i + 1 < symbol.length() && symbol.charAt(i + 1) == '\n') {
          writeSpace = this.AppendSpaceAndSymbol(
            symbol,
            symbolBegin,
            i,
            writeSpace);
          symbolBegin = i + 2;
          i += 2;
        } else if (symbol.charAt(i) == '(' || symbol.charAt(i) == ')') {
          writeSpace = this.AppendSpaceAndSymbol(
            symbol,
            symbolBegin,
            i,
            writeSpace);
          writeSpace = this.AppendSpaceAndSymbol(
            symbol,
            i,
            i + 1,
            writeSpace);
          symbolBegin = i + 1;
          ++i;
        } else if (symbol.charAt(i) == ' ' || symbol.charAt(i) == '\t') {
          this.AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
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
      this.AppendSpaceAndSymbol(
        symbol,
        symbolBegin,
        symbol.length(),
        writeSpace);
    }

    private boolean CanCharUnitFit(
      int currentWordLength,
      int unitLength,
      boolean writeSpace) {
      // 75 is max. allowed length of an encoded word
      int effectiveMaxLength = 75;
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
        for (int i = str.length() - 1; i >= 0; --i) {
          switch ((int)str.charAt(i)) {
            case 0x20: // space
            case 0x09: // tab
              break;
            case 0x0a: // LF
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

    private void AppendQEncoding(int ch) {
      this.builder.append('=');
      this.builder.append(HexChars.charAt((ch >> 4) & 15));
      this.builder.append(HexChars.charAt(ch & 15));
      this.column += 3;
    }

    // See point 3 of RFC 2047 sec. 5 (but excludes '=')
    private static final int[] ValueSmallchars = {
      0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    };

    public HeaderEncoder AppendAsEncodedWords(String symbol) {
      int i = 0;
      int currentWordLength = 0;
      while (i < symbol.length()) {
        int ch = DataUtilities.CodePointAt(symbol, i);
        if (ch >= 0x10000) {
          ++i;
        }
        boolean smallChar = ch <= 0x7e && ch > 0x20 && ValueSmallchars[ch - 0x20]
          == 1;
        int unitLength = 1;
        unitLength = (ch == 0x20 || smallChar) ? 1 : ((ch <= 0x7f) ? 3 :
          ((ch <= 0x7ff) ? 6 : ((ch <= 0xffff) ? 9 : 12)));
        if (!this.CanCharUnitFit(
          currentWordLength,
          unitLength,
          false)) {
          if (currentWordLength > 0) {
            this.AppendSymbol("?=");
            if (this.CanCharUnitFit(0, unitLength, true)) {
              this.AppendSpace();
            } else {
              this.AppendBreak();
            }
          } else {
            this.AppendBreak();
          }
          this.AppendSymbol("=?utf-8?Q?");
          currentWordLength = 12;
        } else if (currentWordLength == 0) {
          this.AppendSymbol("=?utf-8?Q?");
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
        this.AppendSymbol("?=");
      }
      return this;
    }

    private void AppendFinalBase64(int[] b64) {
      int b1 = b64[0];
      int b2 = b64[1];
      int quantumCount = b64[2];
      if (quantumCount == 2) {
        this.builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
        this.builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4) &
          15)));
        this.builder.append((char)Base64Classic.charAt((b2 & 15) << 2));
        this.builder.append('=');
        this.column += 4;
      } else if (quantumCount == 1) {
        this.builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
        this.builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4) &
          15)));
        this.builder.append("==");
        this.column += 4;
      }
      b64[2] = 0;
    }

    private void AppendBase64(int[] b64, int value) {
      int quantumCount = b64[2];
      switch (quantumCount) {
        case 2:
          int b1 = b64[0];
          int b2 = b64[1];
          this.builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
          this.builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4) &
            15)));
          this.builder.append((char)Base64Classic.charAt(((b2 & 15) << 2) + ((value >>
            6) & 3)));
          this.builder.append((char)Base64Classic.charAt(value & 63));
          this.column += 4;
          b64[2] = 0;
          break;
        case 1:
          b64[1] = value;
          b64[2] = 2;
          break;
        case 0:
          b64[0] = value;
          b64[2] = 1;
          break;
      }
    }

    public HeaderEncoder AppendAsEncodedWordsB(String symbol) {
      int i = 0;
      int currentWordLength = 0;
      int[] base64state = new int[] { 0, 0, 0 };
      while (i < symbol.length()) {
        int ch = DataUtilities.CodePointAt(symbol, i);
        if (ch >= 0x10000) {
          ++i;
        }
        int unitLength = (ch <= 0x7f) ? 1 : ((ch <= 0x7ff) ? 2 : ((ch <=
          0xffff) ? 3 : 4));
        int bytesNeeded = 4 + (base64state[2] +
          unitLength > 3 ? 4 : 0);
        if (!this.CanCharUnitFit(
          currentWordLength,
          bytesNeeded,
          false)) {
          if (currentWordLength > 0) {
            this.AppendFinalBase64(base64state);
            this.AppendSymbol("?=");
            if (this.CanCharUnitFit(0, unitLength, true)) {
              this.AppendSpace();
            } else {
              this.AppendBreak();
            }
          } else {
            this.AppendBreak();
          }
          this.AppendSymbol("=?utf-8?B?");
          currentWordLength = 12;
        } else if (currentWordLength == 0) {
          this.AppendSymbol("=?utf-8?B?");
          currentWordLength = 12;
        }
        if (ch <= 0x7f) {
          this.AppendBase64(base64state, ch);
        } else if (ch <= 0x7ff) {
          this.AppendBase64(base64state, 0xc0 | ((ch >> 6) & 0x1f));
          this.AppendBase64(base64state, 0x80 | (ch & 0x3f));
        } else if (ch <= 0xffff) {
          this.AppendBase64(base64state, 0xe0 | ((ch >> 12) & 0x0f));
          this.AppendBase64(base64state, 0x80 | ((ch >> 6) & 0x3f));
          this.AppendBase64(base64state, 0x80 | (ch & 0x3f));
        } else {
          this.AppendBase64(base64state, 0xf0 | ((ch >> 18) & 0x07));
          this.AppendBase64(base64state, 0x80 | ((ch >> 12) & 0x3f));
          this.AppendBase64(base64state, 0x80 | ((ch >> 6) & 0x3f));
          this.AppendBase64(base64state, 0x80 | (ch & 0x3f));
        }
        currentWordLength += bytesNeeded - 4;
        ++i;
      }
      if (currentWordLength > 0) {
        this.AppendFinalBase64(base64state);
        this.AppendSymbol("?=");
      }
      return this;
    }

    private void AppendQuotedStringOrDomain(String symbol, boolean writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'quoted-String'
      // and begins and ends with a double quote, or is a syntactically
      // valid domain literal
      // and begins and ends with opening/closing brackets
      if (symbol.length() == 0) {
        throw new IllegalArgumentException("symbol is empty");
      }
      int i = 0;
      int symbolBegin = 0;
      while (i < symbol.length()) {
        if (symbol.charAt(i) == '\r' && i + 1 < symbol.length() &&
           symbol.charAt(i + 1) == '\n') {
          writeSpace = this.AppendSpaceAndSymbol(
            symbol,
            symbolBegin,
            i,
            writeSpace);
          symbolBegin = i + 2;
          i += 2;
        } else if (symbol.charAt(i) == ' ' || symbol.charAt(i) == '\t') {
          this.AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          this.AppendSpaceOrTab(symbol.charAt(i));
          writeSpace = false;
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
      this.AppendSpaceAndSymbol(
        symbol,
        symbolBegin,
        symbol.length(),
        writeSpace);
    }

    // ASCII characters allowed in atoms
    private static final int[] ValueAsciiAtext = {
      0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
    };

    private static boolean IsSimplePhrase(String str) {
      if (str.length() == 0) {
        return false;
      }
      int count = 0;
      if (str.charAt(0) == ' ' || str.charAt(str.length() - 1) == ' ') {
        return false;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) < 0x80 && str.charAt(i) > 0x20 && ValueAsciiAtext[(int)str.charAt(i) -
0x20]
          == 1) {
          // not simple if a word begins with "=?", an RFC
          // 2047 encoded word start
          if (count == 0 && str.charAt(i) == '=' &&
              i + 1 < str.length() && str.charAt(i + 1) == '?') {
            return false;
          }
          ++count;
          if (count > Message.MaxRecHeaderLineLength - 1) {
            return false;
          }
        } else if (str.charAt(i) == ' ' && i + 1 < str.length() && str.charAt(i + 1) != ' ') {
          count = 0;
        } else {
          return false;
        }
      }
      return true;
    }

    private static boolean IsQuotablePhrase(String str) {
      if (str.length() == 0) {
        return true;
      }
      int count = 1;
      for (int i = 0; i < str.length(); ++i) {
        /*
         // not quotable if a word begins with "=?", an RFC
        // 2047 encoded word start
        if (str.charAt(i) == '=' &&
          (i == 0 || str.charAt(i - 1) == 0x09 || str.charAt(i - 1) == 0x20) &&
          i + 1 < str.length() && str.charAt(i + 1) == '?') {
          return false;
        }*/
        if (!(str.charAt(i) == 0x09 || (str.charAt(i) >= 0x20 && str.charAt(i) <= 0x7e))) {
          return false;
        }
        // Add to character count
        if (str.charAt(i) == ' ' || str.charAt(i) == '\t') {
          // End of word, reset count to 0
          count = 0;
        } else if (str.charAt(i) == '\\' || str.charAt(i) == '"') {
          count += 2;
        } else {
          ++count;
        }
        // For ending DQUOTE
        if (i == str.length() - 1) {
          ++count;
        }
        if (count > Message.MaxRecHeaderLineLength - 1) {
          return false;
        }
      }
      return true;
    }

    public HeaderEncoder AppendPhrase(String phrase) {
      if (IsSimplePhrase(phrase)) {
        this.AppendString(phrase);
      } else if (IsQuotablePhrase(phrase)) {
        if (phrase.indexOf('"') < 0 && phrase.indexOf('\\') < 0) {
          this.AppendQuotedStringOrDomain("\"" + phrase + "\"", false);
        } else {
          StringBuilder builder = new StringBuilder();
          builder.append('"');
          for (int i = 0; i < phrase.length(); ++i) {
            if (phrase.charAt(i) == '\\' || phrase.charAt(i) == '"') {
              builder.append('\\');
              builder.append(phrase.charAt(i));
            } else {
              builder.append(phrase.charAt(i));
            }
          }
          builder.append('"');
          this.AppendQuotedStringOrDomain(
  builder.toString(),
  false);
        }
      } else {
        this.AppendAsEncodedWords(phrase);
      }
      return this;
    }

    private HeaderEncoder AppendSpaceOrTab(char ch) {
      if (this.maxLineLength < 0 || this.column + 1 <= this.maxLineLength) {
        this.builder.append(ch);
        ++this.column;
      } else {
        this.builder.append("\r\n");
        this.builder.append(ch);
        this.column = 1;
      }
      return this;
    }

    public HeaderEncoder AppendSpaceOrTabIfNeeded(char ch) {
      if (this.builder.length() == 0) {
        return this.AppendSpaceOrTab(ch);
      }
      boolean endsWithSpace = this.builder.charAt(this.builder.length() - 1) == 0x20 ||
        this.builder.charAt(this.builder.length() - 1) == 0x09;
      if (!endsWithSpace) {
        this.AppendSpaceOrTab(ch);
      }
      return this;
    }

    public HeaderEncoder AppendSpaceIfNeeded() {
      return this.AppendSpaceOrTabIfNeeded(' ');
    }

    public HeaderEncoder AppendSpace() {
      return this.AppendSpaceOrTab(' ');
    }

    public HeaderEncoder AppendSymbolWithLength(String symbol, int length) {
      if (length > 0) {
        if (this.maxLineLength < 0 || this.column + length <=
                this.maxLineLength) {
          this.builder.append(symbol);
          this.column += length;
        } else {
          if (this.column > 1) {
            this.builder.append("\r\n ");
          }
          this.builder.append(symbol);
          this.column = 1 + length;
        }
      }
      return this;
    }

    // NOTE: Assumes that all symbols being appended
    // contain no unpaired surrogates and no line breaks
    public HeaderEncoder AppendSymbol(String symbol) {
      return this.AppendSymbolWithLength(symbol, symbol.length());
    }

    // Returns true only if:
    // * Header field has a name followed by colon followed by SP,
    // unless the name is exactly 997 characters long (in which
    // case the colon must be followed by CRLF).
    // * Header field's value matches the production "unstructured"
    // in RFC 5322 without any obsolete syntax
    // * Each line is no more than MaxRecHeaderLineLength characters in length,
    // except that a line with no whitespace other than leading and trailing may
    // go up to MaxHardHeaderLineLength characters in length
    // * Value has no all-whitespace or blank lines
    // * Text has only printable ASCII characters, CR,
    // LF, and/or TAB
    public static boolean CanOutputRaw(String s) {
      boolean foundColon = false;
      int len = s.length();
      int chunkLength = 0;
      for (int i = 0; i < len; ++i) {
        if (s.charAt(i) == ':') {
          foundColon = true;
          if (i == 997) {
            if (i + 2 >= len || s.charAt(i + 1) != 0x0d || s.charAt(i + 2) != 0x0a) {
              // Colon not followed by CRLF in 997-character
              // header field name
              return false;
            }
            chunkLength = i + 1;
            break;
          }
          if (i + 1 >= len || s.charAt(i + 1) != 0x20) {
            // Colon not followed by SPACE (0x20)
            return false;
          }
          chunkLength = i + 2;
          if (chunkLength > Message.MaxRecHeaderLineLength) {
            return false;
          }
          break;
        }
        if (s.charAt(i) == 0x0d || s.charAt(i) == 0x09 || s.charAt(i) == 0x20) {
          return false;
        }
      }
      if (!foundColon) {
        return false;
      }
      int whitespaceState = 3;
      for (int i = chunkLength; i < len;) {
        if (s.charAt(i) == 0x0d) {
          if (i + 2 >= len || s.charAt(i + 1) != 0x0a || (s.charAt(i + 2) != 0x09 && s.charAt(i +
            2) != 0x20)) {
            // bare CR, or CRLF not followed by SP/TAB
            return false;
          }
          i += 3;
          chunkLength = 1;
          whitespaceState = 1;
          boolean found = false;
          for (int j = i; j < len; ++j) {
            if (s.charAt(j) != 0x09 && s.charAt(j) != 0x20 && s.charAt(j) != 0x0d) {
              found = true;
              break;
            } else if (s.charAt(j) == 0x0d) {
              // Possible CRLF after all-whitespace line
              return false;
            }
          }
          if (!found) {
            // CRLF followed by an all-whitespace line
            return false;
          }
        } else {
          char c = s.charAt(i);
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

    public static boolean CanOutputRawForNews(String s) {
      if (!CanOutputRaw(s)) {
        return false;
      }
      int colon = s.indexOf(':');
      if (colon < 0 || colon + 1 < s.length() || s.charAt(colon + 1) != ' ') {
        return false;
      }
      int i = colon + 1;
      for (; i < s.length(); ++i) {
        char c = s.charAt(i);
        if (c != 0x0d && c != 0x0a && c != 0x20 && c != 0x09) {
          return true;
        }
      }
      return false;
    }

    private static String CapitalizeHeaderField(String s) {
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
      if (ret.startsWith("Content-Id")) {
        return "Content-ID";
      }
      return ret.startsWith("Mime-Version") ?
"MIME-Version" :
        (ret.startsWith("Message-Id") ? "Message-ID" :
ret);
    }

    public HeaderEncoder AppendFieldName(String fieldName) {
      this.AppendSymbol(CapitalizeHeaderField(fieldName) + ":");
      this.AppendSpace();
      return this;
    }

    public static String TrimLeadingFWS(String fieldValue) {
      int fws = HeaderParser.ParseFWS(fieldValue, 0, fieldValue.length(), null);
      if (fws > 0) {
        fieldValue = fieldValue.substring(fws);
      }
      return fieldValue;
    }

    public static String EncodeFieldAsEncodedWords(
      String fieldName,
      String fieldValue) {
      HeaderEncoder sa = new HeaderEncoder();
      sa.AppendFieldName(fieldName);
      sa.AppendAsEncodedWords(TrimLeadingFWS(fieldValue));
      return sa.toString();
    }

    public static String EncodeField(
      String fieldName,
      String fieldValue) {
      boolean structured = HeaderFieldParsers.GetParser(fieldName).IsStructured();
      String trialField = CapitalizeHeaderField(fieldName) + ": " + fieldValue;
      if (CanOutputRaw(trialField)) {
        return trialField;
      }
      HeaderEncoder sa = new HeaderEncoder();
      sa.AppendFieldName(fieldName);
      if (sa.SimpleAppendString(fieldValue, 0, fieldValue.length())) {
        trialField = sa.toString();
        if (CanOutputRaw(trialField)) {
          return trialField;
        }
      }
      // DebugUtility.Log("Must wrap '" + fieldName + "'");
      // DebugUtility.Log(fieldValue);
      sa = new HeaderEncoder().AppendFieldName(fieldName);
      fieldValue = TrimLeadingFWS(fieldValue);
      sa.AppendString(fieldValue, 0, fieldValue.length(), structured);
      return sa.toString();
    }

    @Override public String toString() {
      return TrimAllSpaceLine(this.builder.toString());
    }
  }
