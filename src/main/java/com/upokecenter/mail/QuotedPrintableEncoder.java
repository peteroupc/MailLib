package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Encodes binary data into Quoted Printable.
     */
  final class QuotedPrintableEncoder implements IStringEncoder
  {
    private static String valueHexAlphabet = "0123456789ABCDEF";
    private int lineCount;
    private int lineBreakMode;
    private boolean unlimitedLineLength;

    // lineBreakMode:
    // 0 - no line breaks
    // 1 - treat CRLF as a line break
    // 2 - treat CR, LF, and CRLF as a line break
    public QuotedPrintableEncoder (int lineBreakMode, boolean unlimitedLineLength) {
      this.lineBreakMode = lineBreakMode;
      this.unlimitedLineLength = unlimitedLineLength;
    }

    private void IncrementLineCount(StringBuilder str, int length) {
      if (!this.unlimitedLineLength) {
        if (this.lineCount + length > 75) {
          // 76 including the final '='
          str.append("=\r\n");
          this.lineCount = length;
        } else {
          this.lineCount += length;
        }
      }
    }

    private void IncrementAndAppend(StringBuilder str, String appendStr) {
      if (!this.unlimitedLineLength) {
        if (this.lineCount + appendStr.length() > 75) {
          // 76 including the final '='
          str.append("=\r\n");
          this.lineCount = appendStr.length();
        } else {
          this.lineCount += appendStr.length();
        }
      }
      str.append(appendStr);
    }

    private void IncrementAndAppendChar(StringBuilder str, char ch) {
      if (!this.unlimitedLineLength) {
        if (this.lineCount + 1 > 75) {
          // 76 including the final '='
          str.append("=\r\n");
          this.lineCount = 1;
        } else {
          ++this.lineCount;
        }
      }
      str.append(ch);
    }

    public void FinalizeEncoding(StringBuilder str) {
      // No need to finalize encoding for quoted printable
    }

    public void WriteToString(StringBuilder str, byte b) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      this.WriteToString(str, new byte[] {  b  }, 0, 1);
    }

    public void WriteToString(StringBuilder str, byte[] data, int offset, int count) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Integer.toString((int)offset) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Integer.toString((int)offset) + ") is more than " + Integer.toString((int)data.length));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Integer.toString((int)count) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Integer.toString((int)count) + ") is more than " + Integer.toString((int)data.length));
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Integer.toString((int)(data.length - offset)) + ") is less than " + Integer.toString((int)count));
      }
      int length = offset + count;
      int i = offset;
      char[] buf = new char[4];
      for (i = offset; i < length; ++i) {
        if (data[i] == 0xd) {
          if (this.lineBreakMode == 0) {
            this.IncrementAndAppend(str, "=0D");
          } else if (i + 1 >= length || data[i + 1] != 0xa) {
            if (this.lineBreakMode == 2) {
              str.append("\r\n");
              this.lineCount = 0;
            } else {
              this.IncrementAndAppend(str, "=0D");
            }
          } else {
            ++i;
            str.append("\r\n");
            this.lineCount = 0;
          }
        } else if (data[i] == 0xa) {
          if (this.lineBreakMode == 2) {
            str.append("\r\n");
            this.lineCount = 0;
          } else {
            this.IncrementAndAppend(str, "=0A");
          }
        } else if (data[i] == 9) {
          this.IncrementAndAppend(str, "=09");
        } else if (this.lineCount == 0 && data[i] == (byte)'.' && i + 1 < length && (data[i] == '\r' || data[i] == '\n')) {
          this.IncrementAndAppend(str, "=2E");
        } else if (this.lineCount == 0 && i + 4 < length && data[i] == (byte)'F' && data[i + 1] == (byte)'r' && data[i + 2] == (byte)'o' && data[i + 3] == (byte)'m' && data[i + 4] == (byte)' ') {
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
              str.append("=20\r\n");
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
                   (data[i] >= '0' && data[i] <= '9') ||
                   (data[i] >= 'a' && data[i] <= 'z') ||
                   "()'+-.,/?:".indexOf((char)data[i]) >= 0) {
          this.IncrementAndAppendChar(str, (char)data[i]);
        } else {
          this.IncrementLineCount(str, 3);
          buf[0] = '=';
          buf[1] = valueHexAlphabet.charAt((data[i] >> 4) & 15);
          buf[2] = valueHexAlphabet.charAt(data[i] & 15);
          str.append(buf,0,(0)+(3));
        }
      }
    }
  }
