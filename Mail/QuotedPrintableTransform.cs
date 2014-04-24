/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail {
  internal sealed class QuotedPrintableTransform : ITransform {
    private TransformWithUnget input;
    private int lineCharCount;
    private bool lenientLineBreaks;
    private byte[] buffer;
    private int bufferIndex;
    private int bufferCount;

    private int maxLineSize;

    public QuotedPrintableTransform(
      ITransform input,
      bool lenientLineBreaks,
      int maxLineSize) {
      this.maxLineSize = maxLineSize;
      this.input = new TransformWithUnget(input);
      this.lenientLineBreaks = lenientLineBreaks;
    }

    public QuotedPrintableTransform(
      ITransform input,
      bool lenientLineBreaks) : this(input, lenientLineBreaks, 76) {
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
    private void ResizeBuffer(int size) {
      if (this.buffer == null) {
        this.buffer = new byte[size + 10];
      } else if (size > this.buffer.Length) {
        byte[] newbuffer = new byte[size + 10];
        Array.Copy(this.buffer, newbuffer, this.buffer.Length);
        this.buffer = newbuffer;
      }
      this.bufferCount = size;
      this.bufferIndex = 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadByte() {
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
        int c = this.input.ReadByte();
        if (c < 0) {
          // End of stream
          return -1;
        } else if (c == 0x0d) {
          c = this.input.ReadByte();
          if (c == 0x0a) {
            // CRLF
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            this.lineCharCount = 0;
            return 0x0d;
          } else {
            this.input.Unget();
            if (!this.lenientLineBreaks) {
              throw new MessageDataException("Expected LF after CR");
            }
            // CR, so write CRLF
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            this.lineCharCount = 0;
            return 0x0d;
          }
        } else if (c == 0x0a) {
          if (!this.lenientLineBreaks) {
            throw new MessageDataException("Expected LF after CR");
          }
          // LF, so write CRLF
          this.ResizeBuffer(1);
          this.buffer[0] = 0x0a;
          this.lineCharCount = 0;
          return 0x0d;
        } else if (c == '=') {
          ++this.lineCharCount;
          if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
            throw new MessageDataException("Encoded quoted-printable line too long");
          }
          int b1 = this.input.ReadByte();
          int b2 = this.input.ReadByte();
          if (b2 >= 0 && b1 >= 0) {
            if (b1 == '\r' && b2 == '\n') {
              // Soft line break
              this.lineCharCount = 0;
              continue;
            } else if (b1 == '\r') {
              if (!this.lenientLineBreaks) {
                throw new MessageDataException("Expected LF after CR");
              }
              this.lineCharCount = 0;
              this.input.Unget();
              continue;
            } else if (b1 == '\n') {
              if (!this.lenientLineBreaks) {
                throw new MessageDataException("Bare LF not expected");
              }
              this.lineCharCount = 0;
              this.input.Unget();
              continue;
            }
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
            } else {
              throw new MessageDataException("Invalid hex character");
            }
            if (b2 >= '0' && b2 <= '9') {
              c <<= 4;
              c |= b2 - '0';
            } else if (b2 >= 'A' && b2 <= 'F') {
              c <<= 4;
              c |= b2 + 10 - 'A';
            } else if (b2 >= 'a' && b2 <= 'f') {
              c <<= 4;
              c |= b2 + 10 - 'a';
            } else {
              throw new MessageDataException("Invalid hex character");
            }
            this.lineCharCount += 2;
            if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
              throw new MessageDataException("Encoded quoted-printable line too long");
            }
            return c;
          } else if (b1 >= 0) {
            if (b1 == '\r') {
              // Soft line break
              if (!this.lenientLineBreaks) {
                throw new MessageDataException("Expected LF after CR");
              }
              this.lineCharCount = 0;
              this.input.Unget();
              continue;
            } else if (b1 == '\n') {
              // Soft line break
              if (!this.lenientLineBreaks) {
                throw new MessageDataException("Bare LF not expected");
              }
              this.lineCharCount = 0;
              this.input.Unget();
              continue;
            } else {
              throw new MessageDataException("Invalid data after equal sign");
            }
          } else {
            // Equal sign at end; ignore
            return -1;
          }
        } else if (c != '\t' && (c < 0x20 || c >= 0x7f)) {
          throw new MessageDataException("Invalid character in quoted-printable");
        } else if (c == ' ' || c == '\t') {
          // Space or tab. Since the quoted-printable spec
          // requires decoders to delete spaces and tabs before
          // CRLF, we need to create a lookahead buffer for
          // tabs and spaces read to see if they precede CRLF.
          int spaceCount = 1;
          ++this.lineCharCount;
          if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
            throw new MessageDataException("Encoded quoted-printable line too long");
          }
          // In most cases, though, there will only be
          // one space or tab
          int c2 = this.input.ReadByte();
          if (c2 != ' ' && c2 != '\t' && c2 != '\r' && c2 != '\n' && c2 >= 0) {
            // Simple: Space before a character other than
            // space, tab, CR, LF, or EOF
            this.input.Unget();
            return c;
          }
          bool endsWithLineBreak = false;
          while (true) {
            if ((c2 == '\n' && this.lenientLineBreaks) || c2 < 0) {
              // EOF, or LF with lenient line breaks
              this.input.Unget();
              endsWithLineBreak = true;
              break;
            } else if (c2 == '\r' && this.lenientLineBreaks) {
              // CR with lenient line breaks
              this.input.Unget();
              endsWithLineBreak = true;
              break;
            } else if (c2 == '\r') {
              // CR, may or may not be a line break
              c2 = this.input.ReadByte();
              // Add the CR to the
              // buffer, it won't be ignored
              this.ResizeBuffer(spaceCount);
              this.buffer[spaceCount - 1] = (byte)'\r';
              if (c2 == '\n') {
                // LF, so it's a line break
                this.lineCharCount = 0;
                this.ResizeBuffer(spaceCount + 1);
                this.buffer[spaceCount] = (byte)'\n';
                endsWithLineBreak = true;
                break;
              } else {
                if (!this.lenientLineBreaks) {
                  throw new MessageDataException("Expected LF after CR");
                }
                this.input.Unget();  // it's something else
                ++this.lineCharCount;
                if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
                  throw new MessageDataException("Encoded quoted-printable line too long");
                }
                break;
              }
            } else if (c2 != ' ' && c2 != '\t') {
              // Not a space or tab
              this.input.Unget();
              break;
            } else {
              // An additional space or tab
              this.ResizeBuffer(spaceCount);
              this.buffer[spaceCount - 1] = (byte)c2;
              ++spaceCount;
              ++this.lineCharCount;
              if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
                throw new MessageDataException("Encoded quoted-printable line too long");
              }
            }
            c2 = this.input.ReadByte();
          }
          // Ignore space/tab runs if the line ends in that run
          if (!endsWithLineBreak) {
            return c;
          } else {
            this.bufferCount = 0;
            continue;
          }
        } else {
          ++this.lineCharCount;
          if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
            throw new MessageDataException("Encoded quoted-printable line too long");
          }
          return c;
        }
      }
    }
  }
}
