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
      if (maxLineLength < 0 || this.column + (endIndex-startIndex) +
        spaceLength <= this.maxLineLength) {
        if (writeSpace) {
 this.builder.append(" ");
}
        this.builder.append(symbol.substring(startIndex, (startIndex)+(endIndex-startIndex)));
        this.column += (endIndex-startIndex) + spaceLength;
      } else {
        this.builder.append("\r\n ");
        this.builder.append(symbol.substring(startIndex, (startIndex)+(endIndex-startIndex)));
        this.column = 1 + (endIndex-startIndex);
      }
      return false;  // No need to write space anymore
    }

    public HeaderEncoder AppendString(String symbol) {
      if (symbol.length() > 0) {
        int i = 0;
        int symbolBegin = 0;
        boolean writeSpace = false;
        while (i < symbol.length()) {
     if (symbol.charAt(i) == '\r' && i + 1 < symbol.length() && symbol.charAt(i + 1) == '\n'
) {
         writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
              writeSpace);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (symbol.charAt(i) == '"') {
            // May begin quoted-String (use ParseQuotedStringCore instead of
            // ParseQuotedString because it excludes optional CFWS at ends)
   int si = HeaderParser.ParseQuotedStringCore(symbol, i, symbol.length(),
              null);
            if (si != i) {
         writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                writeSpace);
              AppendQuotedString(symbol.substring(i, (i)+(si - i)), writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
          } else if (symbol.charAt(i) == '(') {
            // May begin comment
  int si = HeaderParserUtility.ParseCommentLax(symbol, i, symbol.length(),
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
            i = HeaderParser.ParseFWS(symbol, i, symbol.length(), null);
            symbolBegin = i;
          } else {
            ++i;
          }
        }
        AppendSpaceAndSymbol(symbol, symbolBegin, symbol.length(), writeSpace);
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

    private void AppendQuotedString(String symbol, boolean writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'quoted-String'
      // and begins and ends with a double quote
      if (symbol.length() == 0 || symbol.charAt(0) != '"') {
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
      return ret.equals("Mime-Version") ? "MIME-Version" :
        (ret.equals("Message-Id") ? "Message-ID" : ret);
    }
  public static String EncodeHeaderField(String fieldName, String
      fieldValue) {
      HeaderEncoder sa = new HeaderEncoder(76, 0);
      sa.AppendSymbol(CapitalizeHeaderField(fieldName) + ":");
      if (fieldValue.length() == 0 || fieldValue.charAt(0) != ' ') {
        sa.AppendSpace();
      }
      sa.AppendString(fieldValue);
      return sa.toString();
    }
    @Override public String toString() {
      return this.builder.toString();
    }
  }
