/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;
using System.Text;

using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.QuotedPrintableEncoder"]/*'/>
  internal sealed class QuotedPrintableEncoder : ICharacterEncoder {
    private const string HexAlphabet = "0123456789ABCDEF";
    private int lineCount;
    private readonly int lineBreakMode;
    private readonly bool unlimitedLineLength;

    // lineBreakMode:
    // 0 - no line breaks
    // 1 - treat CRLF as a line break
    // 2 - treat CR, LF, and CRLF as a line break
    public QuotedPrintableEncoder(int lineBreakMode, bool unlimitedLineLength) {
      this.lineBreakMode = lineBreakMode;
      this.unlimitedLineLength = unlimitedLineLength;
    }

    private int IncrementAndAppend(IWriter output, string appendStr) {
      var count = 0;
      if (!this.unlimitedLineLength) {
        if (this.lineCount + appendStr.Length > 75) {
          // 76 including the final '='
          output.WriteByte(0x3d);
          output.WriteByte(0x0d);
          output.WriteByte(0x0a);
          this.lineCount = 0;
          count += 3;
        }
      }
      for (int i = 0; i < appendStr.Length; ++i) {
        if (i == 0 && this.lineCount == 0 && appendStr[i] == '.') {
          output.WriteByte((byte)'=');
          output.WriteByte((byte)'2');
          output.WriteByte((byte)'E');
          this.lineCount += 2;
          count += 2;
        } else {
          output.WriteByte((byte)appendStr[i]);
        }
        ++count;
      }
      this.lineCount += appendStr.Length;
      return count;
    }

    private int IncrementAndAppendChars(
IWriter output,
char b1,
char b2,
char b3) {
      var count = 0;
      if (!this.unlimitedLineLength) {
        if (this.lineCount + 3 > 75) {
          // 76 including the final '='
          output.WriteByte(0x3d);
          output.WriteByte(0x0d);
          output.WriteByte(0x0a);
          this.lineCount = 0;
          count += 3;
        }
      }
      if (this.lineCount == 0 && b1 == '.') {
        output.WriteByte((byte)'=');
        output.WriteByte((byte)'2');
        output.WriteByte((byte)'E');
        this.lineCount += 2;
        count += 2;
      } else {
        output.WriteByte((byte)b1);
      }
      output.WriteByte((byte)b2);
      output.WriteByte((byte)b3);
      this.lineCount += 3;
      count += 3;
      return count;
    }

    private int IncrementAndAppendChar(IWriter output, char ch) {
      var count = 1;
      if (!this.unlimitedLineLength) {
        if (this.lineCount + 1 > 75) {
          // 76 including the final '='
          byte[] buf;
          if (ch == '.') {
            buf = new byte[] { 0x3d, 0x0d, 0x0a, (byte)'=',
            (byte)'2', (byte)'E' };
          } else {
            buf = new byte[] { 0x3d, 0x0d, 0x0a, (byte)ch };
          }
          output.Write(buf, 0, buf.Length);
          this.lineCount = buf.Length - 3;
          return buf.Length;
        }
      }
      if (this.lineCount == 0 && ch == '.') {
        output.WriteByte((byte)'=');
        output.WriteByte((byte)'2');
        output.WriteByte((byte)'E');
        this.lineCount += 2;
        count += 2;
      } else {
        output.WriteByte((byte)ch);
      }
      ++this.lineCount;
      return count;
    }

    private int machineState;

    public int Encode(
      int c,
      IWriter output) {
      if (output == null) {
        throw new ArgumentNullException("output");
      }
      var count = 0;
      if (c >= 0) {
        c &= 0xff;
      }
      while (true) {
        switch (this.machineState) {
          case 0: {
              // Normal
              if (c < 0) {
                return -1;
              }
              if (c == 0xd) {
                if (this.lineBreakMode == 0) {
                  return count + this.IncrementAndAppend(output, "=0D");
                } else {
                  this.machineState = 1;
                  return count;
                }
              } else if (c == 0xa) {
                if (this.lineBreakMode == 2) {
                  output.WriteByte((byte)0x0d);
                  output.WriteByte((byte)0x0a);
                  this.lineCount = 0;
                  return 2 + count;
                } else {
                  return count + this.IncrementAndAppend(output, "=0A");
                }
              } else if (c == 0x09) {
                return count + this.IncrementAndAppend(output, "=09");
              } else if (c == 0x3d) {
                return count + this.IncrementAndAppend(output, "=3D");
              } else if (c == 0x2e && this.lineCount == 0) {
                return count + this.IncrementAndAppend(output, "=2E");
              } else if (c == 0x46 && this.lineCount == 0) {
                this.machineState = 3;
                return count;
              } else if (c == 0x20) {
                this.machineState = 7;
                return count;
              } else if ((c >= 'A' && c <= 'Z') ||
                (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') ||
                "()'+-.,/?:".IndexOf((char)c) >= 0) {
                return count + this.IncrementAndAppendChar(output, (char)c);
              } else {
                count += this.IncrementAndAppendChars(
output,
(char)0x3d,
HexAlphabet[(c >> 4) & 15],
HexAlphabet[c & 15]);
                return count;
              }
            }
          case 1: {
              // Carriage return
              if (c == 0x0a) {
                output.WriteByte((byte)0x0d);
                output.WriteByte((byte)0x0a);
                this.lineCount = 0;
                count += 2;
                this.machineState = 0;
                return count;
              } else {
                if (this.lineBreakMode == 2) {
                  output.WriteByte((byte)0x0d);
                  output.WriteByte((byte)0x0a);
                  this.lineCount = 0;
                  this.machineState = 0;
                  count += 2;
                  continue;
                } else {
                  count += this.IncrementAndAppend(output, "=0D");
                  this.machineState = 0;
                  continue;
                }
              }
            }
          case 3: {
              // Capital F at beginning of line
              // See page 7-8 of RFC 2049
              if (c == (byte)'r') {
                this.machineState = 4;
                return count;
              } else {
                count += this.IncrementAndAppendChar(output, (char)'F');
                this.machineState = 0;
                continue;
              }
            }
          case 4: {
              // 'Fr' at beginning of line
              if (c == (byte)'o') {
                this.machineState = 5;
                return count;
              } else {
                count += this.IncrementAndAppendChar(output, (char)'F');
                count += this.IncrementAndAppendChar(output, (char)'r');
                this.machineState = 0;
                continue;
              }
            }
          case 5: {
              // 'Fro' at beginning of line
              if (c == (byte)'m') {
                this.machineState = 6;
                return count;
              } else {
                count += this.IncrementAndAppendChar(output, (char)'F');
                count += this.IncrementAndAppendChar(output, (char)'r');
                count += this.IncrementAndAppendChar(output, (char)'o');
                this.machineState = 0;
                continue;
              }
            }
          case 6: {
              // 'From' at beginning of line
              if (c == 0x20) {
                count += this.IncrementAndAppend(output, "=46");
                count += this.IncrementAndAppendChar(output, (char)'r');
                count += this.IncrementAndAppendChar(output, (char)'o');
                count += this.IncrementAndAppendChar(output, (char)'m');
                this.machineState = 0;
                continue;
              } else {
                count += this.IncrementAndAppendChar(output, (char)'F');
                count += this.IncrementAndAppendChar(output, (char)'r');
                count += this.IncrementAndAppendChar(output, (char)'o');
                count += this.IncrementAndAppendChar(output, (char)'m');
                this.machineState = 0;
                continue;
              }
            }
          case 7: {
              // Space
              if (c < 0) {
                count += this.IncrementAndAppend(output, "=20");
                this.machineState = 0;
                return count;
              } else if (this.lineBreakMode == 2 && c == 0x0a) {
                count += this.IncrementAndAppend(output, "=20\r\n");
                this.lineCount = 0;
                this.machineState = 0;
                return count;
              } else if (this.lineBreakMode == 2 && c == 0x0d) {
                this.machineState = 8;
                return count;
              } else if (this.lineBreakMode == 2) {
                count += this.IncrementAndAppendChar(output, ' ');
                this.machineState = 0;
                continue;
              } else if (this.lineBreakMode == 1 && c == 0x0d) {
                this.machineState = 8;
                return count;
              } else if (this.lineBreakMode == 1 && c == 0x0a) {
                count += this.IncrementAndAppend(output, "=20");
                this.machineState = 0;
                continue;
              } else {
                count += this.IncrementAndAppendChar(output, ' ');
                this.machineState = 0;
                continue;
              }
            }
          case 8: {
              // Space, CR
              if (c < 0) {
                if (this.lineBreakMode == 2) {
                  // Space, linebreak, EOF
                  count += this.IncrementAndAppend(output, "=20\r\n");
                  this.lineCount = 0;
                  this.machineState = 0;
                  return count;
                } else if (this.lineBreakMode == 1) {
                  // Space, CR, EOF
                  count += this.IncrementAndAppend(output, "=20");
                  count += this.IncrementAndAppend(output, "=0D");
                  this.lineCount = 0;
                  this.machineState = 0;
                  return count;
                } else {
                  // Space, CR, EOF
                  count += this.IncrementAndAppend(output, "=20");
                  count += this.IncrementAndAppend(output, "=0D");
                  this.machineState = 0;
                  return count;
                }
              } else if (c == 0x0a) {
                if (this.lineBreakMode == 2 || this.lineBreakMode == 1) {
                  // Space, linebreak
                  count += this.IncrementAndAppend(output, "=20\r\n");
                  this.lineCount = 0;
                  this.machineState = 0;
                  return count;
                } else {
                  // Space, CR, LF
                  count += this.IncrementAndAppend(output, "=20");
                  count += this.IncrementAndAppend(output, "=0D");
                  count += this.IncrementAndAppend(output, "=0A");
                  this.machineState = 0;
                  return count;
                }
              } else if (c == 0x0d) {
                count += this.IncrementAndAppendChar(output, ' ');
                count += this.IncrementAndAppend(output, "=0D");
                this.machineState = 0;
                continue;
              } else {
                count += this.IncrementAndAppendChar(output, ' ');
                count += this.IncrementAndAppend(output, "=0D");
                this.machineState = 0;
                continue;
              }
            }
        }
      }
    }
  }
}
