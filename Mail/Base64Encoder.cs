/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

namespace PeterO.Mail {
    /// <summary>Encodes binary data in Base64.</summary>
  internal sealed class Base64Encoder : IStringEncoder
  {
    private const string Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    private int lineCount;
    private int quantumCount;
    private int byte1;
    private int byte2;
    private bool padding;
    private bool lenientLineBreaks;
    private bool haveCR;
    private bool unlimitedLineLength;
    private string alphabet;
    private char[] charBuffer;

    public Base64Encoder(bool padding, bool lenientLineBreaks, bool unlimitedLineLength) :
      this(padding, lenientLineBreaks, unlimitedLineLength, Base64Classic) {
    }

    public Base64Encoder(bool padding, bool lenientLineBreaks, bool unlimitedLineLength, string alphabet) {
      if (alphabet == null) {
        throw new ArgumentNullException("alphabet");
      }
      if (alphabet.Length != 64) {
        throw new ArgumentException("alphabet.Length (" + alphabet.Length + ") is not equal to 64");
      }
      this.charBuffer = new char[8];
      this.padding = padding;
      this.unlimitedLineLength = unlimitedLineLength;
      this.lenientLineBreaks = lenientLineBreaks;
      this.byte1 = -1;
      this.byte2 = -1;
      this.alphabet = alphabet;
    }

    private void LineAwareAppend(StringBuilder sb, char c) {
      if (!this.unlimitedLineLength) {
        if (this.lineCount >= 76) {
          sb.Append("\r\n");
          this.lineCount = 0;
        }
        ++this.lineCount;
      }
      sb.Append(c);
    }

    private void LineAwareAppendFour(StringBuilder sb, char c1, char c2, char c3, char c4) {
      int charCount = 0;
      if (!this.unlimitedLineLength) {
        if (this.lineCount >= 76) {
          // Output CRLF
          this.charBuffer[charCount++] = '\r';
          this.charBuffer[charCount++] = '\n';
          this.lineCount = 0;
        } else if (this.lineCount + 3 >= 76) {
          this.LineAwareAppend(sb, c1);
          this.LineAwareAppend(sb, c2);
          this.LineAwareAppend(sb, c3);
          this.LineAwareAppend(sb, c4);
          return;
        }
        this.lineCount += 4;
      }
      this.charBuffer[charCount++] = c1;
      this.charBuffer[charCount++] = c2;
      this.charBuffer[charCount++] = c3;
      this.charBuffer[charCount++] = c4;
      sb.Append(this.charBuffer, 0, charCount);
    }

    private void AddByteInternal(StringBuilder str, byte b) {
      int ib = ((int)b) & 0xff;
      if (this.quantumCount == 2) {
        this.LineAwareAppendFour(
          str,
          this.alphabet[(this.byte1 >> 2) & 63],
          this.alphabet[((this.byte1 & 3) << 4) + ((this.byte2 >> 4) & 15)],
          this.alphabet[((this.byte2 & 15) << 2) + ((ib >> 6) & 3)],
          this.alphabet[ib & 63]);
        this.byte1 = -1;
        this.byte2 = -1;
        this.quantumCount = 0;
      } else if (this.quantumCount == 1) {
        this.byte2 = ib;
        this.quantumCount = 2;
      } else {
        this.byte1 = ib;
        this.quantumCount = 1;
      }
    }

    private void AddByte(StringBuilder str, byte b) {
      if (this.lenientLineBreaks) {
        if (b == 0x0d) {
          // CR
          this.haveCR = true;
          this.AddByteInternal(str, (byte)0x0d);
          this.AddByteInternal(str, (byte)0x0a);
          return;
        }
        if (b == 0x0a && !this.haveCR) {
          // bare LF
          if (this.haveCR) {
            // Do nothing, this is an LF that follows CR
            this.haveCR = false;
          } else {
            this.AddByteInternal(str, (byte)0x0d);
            this.AddByteInternal(str, (byte)0x0a);
            this.haveCR = false;
          }
          return;
        }
      }
      this.AddByteInternal(str, b);
      this.haveCR = false;
    }

    public void WriteToString(StringBuilder str, byte b) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.AddByte(str, b);
    }

    public void FinalizeEncoding(StringBuilder str) {
      if (this.quantumCount == 2) {
        char c1 = this.alphabet[(this.byte1 >> 2) & 63];
        char c2 = this.alphabet[((this.byte1 & 3) << 4) + ((this.byte2 >> 4) & 15)];
        char c3 = this.alphabet[((this.byte2 & 15) << 2)];
        if (this.padding) {
          this.LineAwareAppendFour(str, c1, c2, c3, '=');
        } else {
          this.LineAwareAppend(str, c1);
          this.LineAwareAppend(str, c2);
          this.LineAwareAppend(str, c3);
        }
        this.byte1 = -1;
        this.byte2 = -1;
        this.quantumCount = 0;
      } else if (this.quantumCount == 1) {
        char c1 = this.alphabet[(this.byte1 >> 2) & 63];
        char c2 = this.alphabet[((this.byte1 & 3) << 4)];
        if (this.padding) {
          this.LineAwareAppendFour(str, c1, c2, '=', '=');
        } else {
          this.LineAwareAppend(str, c1);
          this.LineAwareAppend(str, c2);
        }
        this.byte1 = -1;
        this.byte2 = -1;
        this.quantumCount = 0;
      }
      this.haveCR = false;
    }

public void WriteToStringAndFinalize(StringBuilder str, byte[] data, int offset, int count) {
      this.WriteToString(str, data, offset, count);
      this.FinalizeEncoding(str);
    }

    public void WriteToString(StringBuilder str, byte[] data, int offset, int count) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((int)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((int)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count < 0) {
        throw new ArgumentException("count (" + Convert.ToString((int)count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + Convert.ToString((int)count, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" + Convert.ToString((int)(data.Length - offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((int)count, System.Globalization.CultureInfo.InvariantCulture));
      }
      for (int i = 0; i < count; ++i) {
        this.AddByte(str, data[offset + i]);
      }
    }
  }
}
