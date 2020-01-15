package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.io.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

  /**
   * This is an class.
   */
  final class Base64Encoder implements ICharacterEncoder {
    static final int MaxLineLength = 76;

    private static final byte[] Base64Classic = {
      0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b,
      0x4c, 0x4d,
      0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58,
      0x59, 0x5a,
      0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b,
      0x6c, 0x6d,
      0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78,
      0x79, 0x7a,
      0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x2b, 0x2f,
     };

    private final byte[] alphabet;
    private final boolean lenientLineBreaks;

    private final boolean padding;
    private final boolean unlimitedLineLength;
    private int byte1;
    private int byte2;

    private boolean finalized;
    private boolean haveCR;
    private int lineCount;
    private int quantumCount;

    public Base64Encoder(
      boolean padding,
      boolean lenientLineBreaks,
      boolean unlimitedLineLength) {
      this.padding = padding;
      this.unlimitedLineLength = unlimitedLineLength;
      this.lenientLineBreaks = lenientLineBreaks;
      this.byte1 = -1;
      this.byte2 = -1;
      this.alphabet = Base64Classic;
    }

    public int Encode(int b, IWriter output) {
      if (b < 0) {
        return this.finalized ? (-1) : this.FinalizeEncoding(output);
      }
      b &= 0xff;
      int count = 0;
      if (this.lenientLineBreaks) {
        if (b == 0x0d) {
          // CR
          this.haveCR = true;
          count += this.AddByteInternal(output, 0x0d);
          count += this.AddByteInternal(output, 0x0a);
          return count;
        }
        if (b == 0x0a && !this.haveCR) {
          // bare LF
          if (this.haveCR) {
            // Do nothing, this is an LF that follows CR
            this.haveCR = false;
          } else {
            count += this.AddByteInternal(output, 0x0d);
            count += this.AddByteInternal(output, 0x0a);
            this.haveCR = false;
          }
          return count;
        }
      }
      count += this.AddByteInternal(output, b);
      this.haveCR = false;
      return count;
    }

    private static byte[] StringAlphabetToBytes(String alphabetString) {
      if (alphabetString == null) {
        throw new NullPointerException("alphabetString");
      }
      if (alphabetString.length() != 64) {
        throw new IllegalArgumentException("alphabet String length(" +
          alphabetString.length() + ") is not equal to 64");
      }
      byte[] alphabet = new byte[64];
      for (int i = 0; i < alphabetString.length(); ++i) {
        if (alphabetString.charAt(i) >= 0x100) {
          throw new
          IllegalArgumentException("alphabet String contains a non-Latin1 character");
        }
        alphabet[i] = (byte)alphabetString.charAt(i);
      }
      return alphabet;
    }

    private int AddByteInternal(IWriter output, int b) {
      int ib = b & 0xff;
      if (this.quantumCount == 2) {
        byte al = this.alphabet[((this.byte1 & 3) << 4) +
          ((this.byte2 >> 4) & 15)];
        int ret = this.LineAwareAppendFour(
            output,
            this.alphabet[(this.byte1 >> 2) & 63],
            al,
            this.alphabet[((this.byte2 & 15) << 2) + ((ib >> 6) & 3)],
            this.alphabet[ib & 63]);
        this.byte1 = -1;
        this.byte2 = -1;
        this.quantumCount = 0;
        return ret;
      }
      if (this.quantumCount == 1) {
        this.byte2 = ib;
        this.quantumCount = 2;
        return 0;
      } else {
        this.byte1 = ib;
        this.quantumCount = 1;
        return 0;
      }
    }

    private int FinalizeEncoding(IWriter output) {
      int count = 0;
      if (this.quantumCount == 2) {
        byte c1 = this.alphabet[(this.byte1 >> 2) & 63];
        byte c2 = this.alphabet[((this.byte1 & 3) << 4) + ((this.byte2 >> 4)
&
              15)];
        byte c3 = this.alphabet[(this.byte2 & 15) << 2];
        if (this.padding) {
          count += this.LineAwareAppendFour(output, c1, c2, c3, (byte)'=');
        } else {
          count += this.LineAwareAppend(output, c1);
          count += this.LineAwareAppend(output, c2);
          count += this.LineAwareAppend(output, c3);
        }
        this.byte1 = -1;
        this.byte2 = -1;
        this.quantumCount = 0;
      } else if (this.quantumCount == 1) {
        byte c1 = this.alphabet[(this.byte1 >> 2) & 63];
        byte c2 = this.alphabet[(this.byte1 & 3) << 4];
        if (this.padding) {
          count += this.LineAwareAppendFour(
              output,
              c1,
              c2,
              (byte)'=',
              (byte)'=');
        } else {
          count += this.LineAwareAppend(output, c1);
          count += this.LineAwareAppend(output, c2);
        }
        this.byte1 = -1;
        this.byte2 = -1;
        this.quantumCount = 0;
      }
      this.haveCR = false;
      this.finalized = true;
      return count;
    }

    private int LineAwareAppend(IWriter output, byte c) {
      int charCount = 0;
      if (!this.unlimitedLineLength) {
        if (this.lineCount >= MaxLineLength) {
          output.write((byte)0x0d);
          output.write((byte)0x0a);
          charCount += 2;
          this.lineCount = 0;
        }
        ++this.lineCount;
      }
      output.write((byte)c);
      return 1 + charCount;
    }

    private int LineAwareAppendFour(
      IWriter output,
      byte c1,
      byte c2,
      byte c3,
      byte c4) {
      int charCount = 0;
      byte[] bytes = new byte[6];
      if (!this.unlimitedLineLength) {
        if (this.lineCount >= MaxLineLength) {
          // Output CRLF
          bytes[charCount++] = (byte)0x0d;
          bytes[charCount++] = (byte)0x0a;
          this.lineCount = 0;
        } else if (this.lineCount + 3 >= MaxLineLength) {
          charCount += this.LineAwareAppend(output, c1);
          charCount += this.LineAwareAppend(output, c2);
          charCount += this.LineAwareAppend(output, c3);
          charCount += this.LineAwareAppend(output, c4);
          return charCount;
        }
        this.lineCount += 4;
      }
      bytes[charCount++] = (byte)c1;
      bytes[charCount++] = (byte)c2;
      bytes[charCount++] = (byte)c3;
      bytes[charCount++] = (byte)c4;
      output.write(bytes, 0, charCount);
      return charCount;
    }
  }
