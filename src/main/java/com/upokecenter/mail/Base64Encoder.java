package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

    /**
     * Encodes binary data in Base64.
     */
  final class Base64Encoder implements IStringEncoder
  {
    private static final String Base64Classic =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" ;

    private int lineCount;
    private int quantumCount;
    private int byte1;
    private int byte2;
    private boolean padding;
    private boolean lenientLineBreaks;
    private boolean haveCR;
    private boolean unlimitedLineLength;
    private String alphabet;
    private char[] charBuffer;

    public Base64Encoder (boolean padding, boolean lenientLineBreaks, boolean
      unlimitedLineLength) {
 this(padding, lenientLineBreaks, unlimitedLineLength, Base64Classic);
    }

    public Base64Encoder (boolean padding, boolean lenientLineBreaks, boolean
      unlimitedLineLength, String alphabet) {
      if (alphabet == null) {
        throw new NullPointerException("alphabet");
      }
      if (alphabet.length() != 64) {
        throw new IllegalArgumentException("alphabet.length() (" + alphabet.length() +
          ") is not equal to 64");
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
          sb.append("\r\n");
          this.lineCount = 0;
        }
        ++this.lineCount;
      }
      sb.append(c);
    }

    private void LineAwareAppendFour(StringBuilder sb, char c1, char c2,
      char c3, char c4) {
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
      sb.append(this.charBuffer, 0, (0)+(charCount));
    }

    private void AddByteInternal(StringBuilder str, byte b) {
      int ib = ((int)b) & 0xff;
      if (this.quantumCount == 2) {
        this.LineAwareAppendFour(str,
          this.alphabet.charAt((this.byte1 >> 2) & 63), this.alphabet.charAt(((this.byte1&
            3) << 4) + ((this.byte2 >> 4) & 15)),
          this.alphabet.charAt(((this.byte2 & 15) << 2) + ((ib >> 6) & 3)),
          this.alphabet.charAt(ib & 63));
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
        throw new NullPointerException("str");
      }
      this.AddByte(str, b);
    }

    public void FinalizeEncoding(StringBuilder str) {
      if (this.quantumCount == 2) {
        char c1 = this.alphabet.charAt((this.byte1 >> 2) & 63);
   char c2 = this.alphabet.charAt(((this.byte1 & 3) << 4) + ((this.byte2 >> 4) &
          15));
        char c3 = this.alphabet.charAt(((this.byte2 & 15) << 2));
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
        char c1 = this.alphabet.charAt((this.byte1 >> 2) & 63);
        char c2 = this.alphabet.charAt(((this.byte1 & 3) << 4));
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

public void WriteToStringAndFinalize(StringBuilder str, byte[] data, int
  offset, int count) {
      this.WriteToString(str, data, offset, count);
      this.FinalizeEncoding(str);
    }

    public void WriteToString(StringBuilder str, byte[] data, int offset,
      int count) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
    throw new IllegalArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          data.length);
      }
      if (count < 0) {
      throw new IllegalArgumentException("count (" + count + ") is less than " +
          "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + count + ") is more than " +
          data.length);
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" +
          (data.length - offset) + ") is less than " + count);
      }
      for (int i = 0; i < count; ++i) {
        this.AddByte(str, data[offset + i]);
      }
    }
  }
