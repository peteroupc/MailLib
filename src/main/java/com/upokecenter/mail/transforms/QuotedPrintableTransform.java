package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class QuotedPrintableTransform implements IByteReader {
    private final boolean allowBareLfCr;
    private final boolean checkStrictEncoding;
    private final int maxLineSize;
    private final IByteReader input;

    public static final int MaxLineLength = 76;

    private final int[] printable = {
      0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };

    private int lineCharCount;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    private int lastByte;
    private boolean unget;

    public QuotedPrintableTransform(
      IByteReader input,
      boolean allowBareLfCr,
      int maxLineSize,
      boolean checkStrictEncoding) {
      this.maxLineSize = maxLineSize;
      this.allowBareLfCr = allowBareLfCr;
      this.checkStrictEncoding = checkStrictEncoding;
      this.input = input;
      this.lastByte = -1;
    }

    public QuotedPrintableTransform(
      IByteReader input,
      boolean allowBareLfCr) {
 this(
  input,
  allowBareLfCr,
  MaxLineLength,
  false);
    }

    public QuotedPrintableTransform(
      IByteReader input,
      boolean allowBareLfCr,
      int maxLineLength) {
 this(
  input,
  allowBareLfCr,
  maxLineLength,
  false);
    }

    private void ResizeBuffer(int size) {
      this.buffer = (this.buffer == null) ? ((new byte[size + 10])) : this.buffer;
      if (size > this.buffer.length) {
        byte[] newbuffer = new byte[size + 10];
        System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
        this.buffer = newbuffer;
      }
      this.bufferCount = size;
      this.bufferIndex = 0;
    }

    private int ReadInputByte() {
      if (this.unget) {
        this.unget = false;
      } else {
        this.lastByte = this.input.read();
      }
      return this.lastByte;
    }

    public int read() {
      if (this.bufferIndex < this.bufferCount) {
        int ret = this.buffer[this.bufferIndex];
        ++this.bufferIndex;
        if (this.bufferIndex == this.bufferCount) {
          this.bufferCount = 0;
          this.bufferIndex = 0;
        }
        ret &= 0xff;
        return ret;
      }
      while (true) {
        int c = this.ReadInputByte();
        if (c < 0) {
          // End of stream
          return -1;
        }
        if (!this.checkStrictEncoding && this.printable[c] == 1 &&
       this.maxLineSize < 0) {
          return c;
        }
        if (c == 0x0d) {
          // CR
          c = this.ReadInputByte();
          if (c == 0x0a) {
            // CRLF
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            this.lineCharCount = 0;
            return 0x0d;
          }
          this.unget = true;
          if (!this.allowBareLfCr) {
            throw new MessageDataException("Expected LF after CR");
          }
          // Ignore CR (part of suggested behavior by RFC 2045)
          continue;
        }
        if (c == 0x0a) { // LF
          if (!this.allowBareLfCr) {
            throw new MessageDataException("Expected LF after CR");
          }
          // Ignore LF (part of suggested behavior by RFC 2045)
          continue;
        }
        if (c == '=') { // Equals
          if (this.maxLineSize >= 0) {
            ++this.lineCharCount;
            if (this.lineCharCount > this.maxLineSize) {
              throw new
          MessageDataException("Encoded quoted-printable line too long");
            }
          }
          int b1 = this.ReadInputByte();
          c = 0;
          if (b1 >= '0' && b1 <= '9') {
            c <<= 4;
            c |= b1 - '0';
          } else if (b1 >= 'A' && b1 <= 'F') {
            c <<= 4;
            c |= b1 + 10 - 'A';
          } else if (b1 >= 'a' && b1 <= 'f') {
            c <<= 4;
            c |= b1 + 10 - 'a';
          } else if (b1 == '\r') {
            b1 = this.ReadInputByte();
            if (b1 == '\n') {
              // Soft line break
              this.lineCharCount = 0;
              continue;
            }
            if (!this.checkStrictEncoding && (
              this.maxLineSize > MaxLineLength || this.maxLineSize < 0)) {
              if (this.maxLineSize >= 0) {
                ++this.lineCharCount;
                if (this.lineCharCount > this.maxLineSize) {
                  throw new
              MessageDataException("Encoded quoted-printable line too long");
                }
              }
              this.unget = true;
              return '=';
            }
            if (this.allowBareLfCr) {
              this.unget = true;
              return '=';
            } else {
            throw new
              MessageDataException("CR not followed by LF in quoted-printable");
            }
          } else if (b1 == -1) {
            // Equals sign at end, ignore
            return -1;
          } else {
            if (!this.checkStrictEncoding && (
              this.maxLineSize > MaxLineLength ||
              this.maxLineSize < 0)) {
              // Unget the character, since it might
              // start a valid hex encoding or need
              // to be treated some other way
              this.unget = true;
              return '=';
            }
            if (b1 == '\n' && this.allowBareLfCr) {
              this.unget = true;
              return '=';
            } else {
               throw new
  MessageDataException("Invalid hex character in quoted-printable");
            }
          }
          int b2 = this.ReadInputByte();
          // At this point, only a hex character is expected
          if (b2 >= '0' && b2 <= '9') {
            c <<= 4;
            c |= b2 - '0';
          } else if (b2 >= 'A' && b2 <= 'F') {
            c <<= 4;
            c |= b2 + 10 - 'A';
          } else if (b2 >= 'a' && b2 <= 'f' && !this.checkStrictEncoding) {
            c <<= 4;
            c |= b2 + 10 - 'a';
          } else {
            if (!this.checkStrictEncoding && (
              this.maxLineSize > MaxLineLength || this.maxLineSize < 0)) {
              // Unget the character, since it might
              // start a valid hex encoding or need
              // to be treated some other way
              this.unget = true;
              if (this.maxLineSize >= 0) {
                ++this.lineCharCount;
                if (this.lineCharCount > this.maxLineSize) {
                  throw new
              MessageDataException("Encoded quoted-printable line too long");
                }
              }
              this.ResizeBuffer(1);
              this.buffer[0] = (byte)b1;
              return '=';
            }
            throw new
           MessageDataException("Invalid hex character in quoted-printable");
          }
          if (this.maxLineSize >= 0) {
            this.lineCharCount += 2;
            if (this.lineCharCount > this.maxLineSize) {
              throw new
          MessageDataException("Encoded quoted-printable line too long");
            }
          }
          return c;
        }
        if (c == ' ' || c == '\t') {
          // Space or tab. Since the quoted-printable spec
          // requires decoders to delete spaces and tabs before
          // CRLF, we need to create a lookahead buffer for
          // tabs and spaces read to see if they precede CRLF.
          int spaceCount = 1;
          if (this.maxLineSize >= 0) {
            ++this.lineCharCount;
            if (this.lineCharCount > this.maxLineSize) {
              throw new
          MessageDataException("Encoded quoted-printable line too long");
            }
          }
          // In most cases, though, there will only be
          // one space or tab
          int c2 = this.ReadInputByte();
          if (c2 != ' ' && c2 != '\t' && c2 != '\r' && c2 != '\n' && c2 >= 0) {
            // Simple: Space before a character other than
            // space, tab, CR, LF, or EOF
            if (c2 != '=' && c2 > 0x20 && c2 < 0x7f) {
              // Add the character to the buffer rather
              // than ungetting, for printable ASCII except
              // the Equals sign
              this.ResizeBuffer(1);
              this.buffer[0] = (byte)c2;
            } else {
              this.unget = true;
            }
            return c;
          }
          boolean endsWithEofOrCrLf = false;
          while (true) {
            if (c2 < 0) {
              // EOF
              this.unget = true;
              endsWithEofOrCrLf = true;
              break;
            }
            if (c2 == '\n' && this.allowBareLfCr) {
              // LF with bare LF/CR allowed.
              // Doesn't end with CRLF or EOF.
              break;
            }
            if (c2 == '\r') {
              // CR, may or may not be a line break
              c2 = this.ReadInputByte();
              if (c2 == '\n') {
                // LF, so it's a line break
                this.lineCharCount = 0;
                this.ResizeBuffer(1);
                this.buffer[0] = (byte)'\n';
                return 0x0d;
              } else if (this.allowBareLfCr) {
                // Doesn't end with CRLF or EOF.
                // Add just-read character to buffer
                // and return first space
                this.ResizeBuffer(spaceCount);
                this.buffer[spaceCount - 1] = (byte)c2;
                return c;
              } else {
                throw new MessageDataException("Expected LF after CR");
              }
            }
            if (c2 != ' ' && c2 != '\t') {
              // Not a space or tab
              this.unget = true;
              break;
            }
            // An additional space or tab
            this.ResizeBuffer(spaceCount);
            this.buffer[spaceCount - 1] = (byte)c2;
            ++spaceCount;
            if (this.maxLineSize >= 0) {
              ++this.lineCharCount;
              if (this.lineCharCount > this.maxLineSize) {
                throw new
            MessageDataException("Encoded quoted-printable line too long");
              }
            }
            c2 = this.ReadInputByte();
          }
          // Ignore space/tab runs if the line ends in that run
          if (!endsWithEofOrCrLf) {
            return c;
          }
          if (this.checkStrictEncoding) {
            throw new MessageDataException("Space or tab at end of line");
          }
          this.bufferCount = 0;
          continue;
        }
        if (c != '\t' && (c < 0x20 || c >= 0x7f)) {
          // Invalid character
          if (this.maxLineSize < 0) {
            // Ignore the character
          } else if (this.maxLineSize > MaxLineLength) {
            // Just increment the line count
            ++this.lineCharCount;
            if (this.lineCharCount > this.maxLineSize) {
              throw new
          MessageDataException("Encoded quoted-printable line too long");
            }
          } else {
       throw new MessageDataException("Invalid character in quoted-printable");
          }
        } else {
          // Any other character
          if (this.maxLineSize >= 0) {
            ++this.lineCharCount;
            if (this.lineCharCount > this.maxLineSize) {
              throw new
          MessageDataException("Encoded quoted-printable line too long");
            }
          } else if (this.checkStrictEncoding && (c >= 0x7f || c < 0x20)) {
            throw new MessageDataException("Invalid character");
          }
          return c;
        }
      }
    }
  }
