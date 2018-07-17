/*
Written by Peter O. in 2014-2018.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Text;

namespace PeterO.Mail {
  internal sealed class HeaderEncoder {
    private readonly StringBuilder builder;
    private int maxLineLength;
    private int column;
    private int startColumn;
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
      var spaceLength = (writeSpace) ? 1 : 0;
      if (maxLineLength < 0 || this.column + (endIndex-startIndex) +
        spaceLength <= this.maxLineLength) {
        if (writeSpace) {
 this.builder.Append(" ");
}
        this.builder.Append(symbol.Substring(startIndex, endIndex-startIndex));
        this.column += (endIndex-startIndex) + spaceLength;
      } else {
        this.builder.Append("\r\n ");
        this.builder.Append(symbol.Substring(startIndex, endIndex-startIndex));
        this.column = 1 + (endIndex-startIndex);
      }
      return false;  // No need to write space anymore
    }

    public HeaderEncoder AppendString(string symbol) {
      if (symbol.Length > 0) {
        var i = 0;
        var symbolBegin = 0;
        var writeSpace = false;
        while (i < symbol.Length) {
     if (symbol[i] == '\r' && i + 1 < symbol.Length && symbol[i + 1] == '\n'
) {
         writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
              writeSpace);
            symbolBegin = i + 2;
            i += 2;
            continue;
          } else if (symbol[i] == '"') {
            // May begin quoted-string (use ParseQuotedStringCore instead of
            // ParseQuotedString because it excludes optional CFWS at ends)
   int si = HeaderParser.ParseQuotedStringCore(symbol, i, symbol.Length,
              null);
            if (si != i) {
         writeSpace = AppendSpaceAndSymbol(symbol, symbolBegin, i,
                writeSpace);
              AppendQuotedString(symbol.Substring(i, si - i), writeSpace);
              writeSpace = false;
              i = si;
              symbolBegin = si;
            } else {
              ++i;
            }
          } else if (symbol[i] == '(') {
            // May begin comment
  int si = HeaderParserUtility.ParseCommentLax(symbol, i, symbol.Length,
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
            i = HeaderParser.ParseFWS(symbol, i, symbol.Length, null);
            symbolBegin = i;
          } else {
            ++i;
          }
        }
        AppendSpaceAndSymbol(symbol, symbolBegin, symbol.Length, writeSpace);
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

    private void AppendQuotedString(string symbol, bool writeSpace) {
      // NOTE: Assumes 'symbol' is a syntactically valid 'quoted-string'
      // and begins and ends with a double quote
      if (symbol.Length == 0 || symbol[0] != '"') {
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
        } else if (symbol[i] == ' ') {
          AppendSpaceAndSymbol(symbol, symbolBegin, i, writeSpace);
          writeSpace = true;
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
  public static string EncodeHeaderField(string fieldName, string
      fieldValue) {
      var sa = new HeaderEncoder(76, 0);
      sa.AppendSymbol(CapitalizeHeaderField(fieldName) + ":");
      if (fieldValue.Length == 0 || fieldValue[0] != ' ') {
        sa.AppendSpace();
      }
      sa.AppendString(fieldValue);
      return sa.ToString();
    }
    public override string ToString() {
      return this.builder.ToString();
    }
  }
}
