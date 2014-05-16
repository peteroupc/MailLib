package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  final class EncodedWordEncoder {
    private StringBuilder currentWord;
    private StringBuilder fullString;
    private int spaceCount;

    private static String hex = "0123456789ABCDEF";

    // Doesn't add a space to the beginning of
    // the output
    public EncodedWordEncoder () {
      this.currentWord = new StringBuilder();
      this.fullString = new StringBuilder();
    }

    private void AppendChar(char ch) {
      this.PrepareToAppend(1);
      this.currentWord.append(ch);
    }

    private void PrepareToAppend(int numChars) {
      // 2 for the ending "?="
      if (this.currentWord.length() + numChars + 2 > 75) {
        this.spaceCount = 1;
      }
      if (this.currentWord.length() + numChars + 2 > 75) {
        // Encoded word would be too big,
        // so output that word
        if (this.spaceCount > 0) {
          this.fullString.append(' ');
        }
        this.fullString.append(this.currentWord);
        this.fullString.append("?=");
        this.currentWord.delete(0,(0)+(this.currentWord.length()));
        this.currentWord.append("=?utf-8?Q?");
        this.spaceCount = 1;
      }
    }

    /**
     * Not documented yet.
     * @param suffix A string object.
     * @return An EncodedWordEncoder object.
     */
    public EncodedWordEncoder FinalizeEncoding(String suffix) {
      if (this.currentWord.length() > 0) {
        if (this.currentWord.length() + 2 + suffix.length() > 75) {
          // Too big to fit the current line,
          // create a new line
          this.spaceCount = 1;
        }
        if (this.spaceCount > 0) {
          this.fullString.append(' ');
        }
        this.fullString.append(this.currentWord);
        this.fullString.append("?=");
        if (suffix.length() > 0) {
          this.fullString.append(suffix);
        }
        this.spaceCount = 1;
        this.currentWord.delete(0,(0)+(this.currentWord.length()));
      }
      return this;
    }

    /**
     * Not documented yet.
     * @return An EncodedWordEncoder object.
     */
    public EncodedWordEncoder FinalizeEncoding() {
      return this.FinalizeEncoding("");
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return An EncodedWordEncoder object.
     */
    public EncodedWordEncoder AddPrefix(String str) {
      if (!((str)==null || (str).length()==0)) {
        this.FinalizeEncoding();
        this.currentWord.append(str);
        this.currentWord.append("=?utf-8?Q?");
        this.spaceCount = 0;
      }
      return this;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @return An EncodedWordEncoder object.
     */
    public EncodedWordEncoder AddString(String str, int index, int length) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is less than " + "0");
      }
      if (length > str.length()) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (str.length() - index < length) {
        throw new IllegalArgumentException("str's length minus " + index + " (" + Integer.toString((int)(str.length() - index)) + ") is less than " + Integer.toString((int)length));
      }
      for (int j = index; j < index + length; ++j) {
        int c = DataUtilities.CodePointAt(str, j);
        if (c >= 0x10000) {
          ++j;
        }
        this.AddChar(c);
      }
      return this;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return An EncodedWordEncoder object.
     */
    public EncodedWordEncoder AddString(String str) {
      return this.AddString(str, 0, str.length());
    }

    /**
     * Not documented yet.
     * @param ch A 32-bit signed integer.
     */
    public void AddChar(int ch) {
      if (this.currentWord.length() == 0) {
        this.currentWord.append("=?utf-8?Q?");
      }
      if (ch == 0x20) {
        this.AppendChar('_');
      } else if (ch < 0x80 && ch > 0x20 && ch != (char)'"' && ch != (char)',' &&
                 "?()<>[]:;@\\.=_".indexOf((char)ch) < 0) {
        // A visible ASCII character other than specials, quote, comma, ?, or ()
        this.AppendChar((char)ch);
      } else if (ch < 0x80) {
        this.PrepareToAppend(3);
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(ch >> 4));
        this.currentWord.append(hex.charAt(ch & 15));
      } else if (ch < 0x800) {
        int w = (byte)(0xc0 | ((ch >> 6) & 0x1f));
        int x = (byte)(0x80 | (ch & 0x3f));
        this.PrepareToAppend(6);
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(w >> 4));
        this.currentWord.append(hex.charAt(w & 15));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(x >> 4));
        this.currentWord.append(hex.charAt(x & 15));
      } else if (ch < 0x10000) {
        this.PrepareToAppend(9);
        int w = (byte)(0xe0 | ((ch >> 12) & 0x0f));
        int x = (byte)(0x80 | ((ch >> 6) & 0x3f));
        int y = (byte)(0x80 | (ch & 0x3f));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(w >> 4));
        this.currentWord.append(hex.charAt(w & 15));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(x >> 4));
        this.currentWord.append(hex.charAt(x & 15));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(y >> 4));
        this.currentWord.append(hex.charAt(y & 15));
      } else {
        this.PrepareToAppend(12);
        int w = (byte)(0xf0 | ((ch >> 18) & 0x07));
        int x = (byte)(0x80 | ((ch >> 12) & 0x3f));
        int y = (byte)(0x80 | ((ch >> 6) & 0x3f));
        int z = (byte)(0x80 | (ch & 0x3f));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(w >> 4));
        this.currentWord.append(hex.charAt(w & 15));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(x >> 4));
        this.currentWord.append(hex.charAt(x & 15));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(y >> 4));
        this.currentWord.append(hex.charAt(y & 15));
        this.currentWord.append('=');
        this.currentWord.append(hex.charAt(z >> 4));
        this.currentWord.append(hex.charAt(z & 15));
      }
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.fullString.toString();
    }
  }
