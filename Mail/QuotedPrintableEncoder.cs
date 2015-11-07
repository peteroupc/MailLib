/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;

namespace PeterO.Mail {
    /// <summary>Encodes binary data into Quoted Printable.</summary>
  internal sealed class QuotedPrintableEncoder : IStringEncoder
  {
    private const string HexAlphabet = "0123456789ABCDEF";
    private int lineCount;
    private int lineBreakMode;
    private bool unlimitedLineLength;

    // lineBreakMode:
    // 0 - no line breaks
    // 1 - treat CRLF as a line break
    // 2 - treat CR, LF, and CRLF as a line break
    public QuotedPrintableEncoder(int lineBreakMode, bool unlimitedLineLength) {
      this.lineBreakMode = lineBreakMode;
      this.unlimitedLineLength = unlimitedLineLength;
    }

    private void IncrementLineCount(StringBuilder str, int length) {
      if (!this.unlimitedLineLength) {
        if (this.lineCount + length > 75) {
          // 76 including the final '='
          str.Append("=\r\n");
          this.lineCount = length;
        } else {
          this.lineCount += length;
        }
      }
    }

    private void IncrementAndAppend(StringBuilder str, string appendStr) {
      if (!this.unlimitedLineLength) {
        if (this.lineCount + appendStr.Length > 75) {
          // 76 including the final '='
          str.Append("=\r\n");
          this.lineCount = appendStr.Length;
        } else {
          this.lineCount += appendStr.Length;
        }
      }
      str.Append(appendStr);
    }

    private void IncrementAndAppendChar(StringBuilder str, char ch) {
      if (!this.unlimitedLineLength) {
        if (this.lineCount + 1 > 75) {
          // 76 including the final '='
          str.Append("=\r\n");
          this.lineCount = 1;
        } else {
          ++this.lineCount;
        }
      }
      str.Append(ch);
    }

    public void FinalizeEncoding(StringBuilder str) {
      // No need to finalize encoding for quoted printable
    }

    public void WriteToString(StringBuilder str, byte b) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.WriteToString(str, new[] { b }, 0, 1);
    }

    public void WriteToString(
StringBuilder str,
byte[] data,
int offset,
int count) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
    throw new ArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          data.Length);
      }
      if (count < 0) {
      throw new ArgumentException("count (" + count + ") is less than " +
          "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + count + ") is more than " +
          data.Length);
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" +
          (data.Length - offset) + ") is less than " + count);
      }
      int length = offset + count;
      int i = offset;
      var buf = new char[4];
      for (i = offset; i < length; ++i) {
        if (data[i] == 0xd) {
          if (this.lineBreakMode == 0) {
            this.IncrementAndAppend(str, "=0D");
          } else if (i + 1 >= length || data[i + 1] != 0xa) {
            if (this.lineBreakMode == 2) {
              str.Append("\r\n");
              this.lineCount = 0;
            } else {
              this.IncrementAndAppend(str, "=0D");
            }
          } else {
            ++i;
            str.Append("\r\n");
            this.lineCount = 0;
          }
        } else if (data[i] == 0xa) {
          if (this.lineBreakMode == 2) {
            str.Append("\r\n");
            this.lineCount = 0;
          } else {
            this.IncrementAndAppend(str, "=0A");
          }
        } else if (data[i] == 9) {
          this.IncrementAndAppend(str, "=09");
        } else if (this.lineCount == 0 && data[i] == (byte)'.' && i + 1 <
          length && (data[i] == '\r' || data[i] == '\n')) {
          this.IncrementAndAppend(str, "=2E");
        } else if (this.lineCount == 0 && i + 4 < length && data[i] ==
          (byte)'F' && data[i + 1] == (byte)'r' && data[i + 2] == (byte)'o' &&
          data[i + 3] == (byte)'m' && data[i + 4] == (byte)' ') {
          // See page 7-8 of RFC 2049
          this.IncrementAndAppend(str, "=46rom ");
          i += 4;
        } else if (data[i] == 32) {
          if (i + 1 == length) {
            this.IncrementAndAppend(str, "=20");
          } else if (i + 2 < length && this.lineBreakMode > 0) {
            if (data[i + 1] == 0xd && data[i + 2] == 0xa) {
              this.IncrementAndAppend(str, "=20\r\n");
              this.lineCount = 0;
              i += 2;
            } else {
              this.IncrementAndAppendChar(str, (char)data[i]);
            }
          } else if (i + 1 < length && this.lineBreakMode == 2) {
            if (data[i + 1] == 0xd || data[i + 1] == 0xa) {
              this.IncrementLineCount(str, 3);
              str.Append("=20\r\n");
              this.lineCount = 0;
              ++i;
            } else {
              this.IncrementAndAppendChar(str, (char)data[i]);
            }
          } else {
            this.IncrementAndAppendChar(str, (char)data[i]);
          }
        } else if (data[i] == (byte)'=') {
          this.IncrementAndAppend(str, "=3D");
        } else if ((data[i] >= 'A' && data[i] <= 'Z') ||
    (data[i] >= '0' && data[i] <= '9') || (data[i] >= 'a' && data[i] <= 'z'
) ||
                   "()'+-.,/?:".IndexOf((char)data[i]) >= 0) {
          this.IncrementAndAppendChar(str, (char)data[i]);
        } else {
          this.IncrementLineCount(str, 3);
          buf[0] = '=';
          buf[1] = HexAlphabet[(data[i] >> 4) & 15];
          buf[2] = HexAlphabet[data[i] & 15];
          str.Append(buf, 0, 3);
        }
      }
    }
  }
}
