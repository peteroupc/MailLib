/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;

using PeterO;

namespace PeterO.Mail {
  internal sealed class EncodedWordEncoder {
    private const string HexChars = "0123456789ABCDEF";

    private StringBuilder currentWord;
    private StringBuilder fullString;
    private int spaceCount;

    // Doesn't add a space to the beginning of
    // the output
    public EncodedWordEncoder() {
      this.currentWord = new StringBuilder();
      this.fullString = new StringBuilder();
    }

    private void AppendChar(char ch) {
      this.PrepareToAppend(1);
      this.currentWord.Append(ch);
    }

    private void PrepareToAppend(int numChars) {
      // 2 for the ending "?="
      if (this.currentWord.Length + numChars + 2 > 75) {
        this.spaceCount = 1;
      }
      if (this.currentWord.Length + numChars + 2 > 75) {
        // Encoded word would be too big,
        // so output that word
        if (this.spaceCount > 0) {
          this.fullString.Append(' ');
        }
        this.fullString.Append(this.currentWord);
        this.fullString.Append("?=");
        this.currentWord.Remove(0, this.currentWord.Length);
        this.currentWord.Append("=?utf-8?Q?");
        this.spaceCount = 1;
      }
    }

    public EncodedWordEncoder FinalizeEncoding(string suffix) {
      if (this.currentWord.Length > 0) {
        if (this.currentWord.Length + 2 + suffix.Length > 75) {
          // Too big to fit the current line,
          // create a new line
          this.spaceCount = 1;
        }
        if (this.spaceCount > 0) {
          this.fullString.Append(' ');
        }
        this.fullString.Append(this.currentWord);
        this.fullString.Append("?=");
        if (suffix.Length > 0) {
          this.fullString.Append(suffix);
        }
        this.spaceCount = 1;
        this.currentWord.Remove(0, this.currentWord.Length);
      }
      return this;
    }

    public EncodedWordEncoder FinalizeEncoding() {
      return this.FinalizeEncoding(String.Empty);
    }

    public EncodedWordEncoder AddPrefix(string str) {
      if (!String.IsNullOrEmpty(str)) {
        this.FinalizeEncoding();
        this.currentWord.Append(str);
        this.currentWord.Append("=?utf-8?Q?");
        this.spaceCount = 0;
      }
      return this;
    }

    public EncodedWordEncoder AddString(string str, int index, int length) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
      throw new ArgumentException("index (" + index + ") is less than " +
          "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          str.Length);
      }
      if (length < 0) {
    throw new ArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          str.Length);
      }
      if (str.Length - index < length) {
        throw new ArgumentException("str's length minus " + index + " (" +
          (str.Length - index) + ") is less than " + length);
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

    public EncodedWordEncoder AddString(string str) {
      return this.AddString(str, 0, str.Length);
    }

    public void AddChar(int ch) {
      if (this.currentWord.Length == 0) {
        this.currentWord.Append("=?utf-8?Q?");
      }
      if (ch == 0x20) {
        this.AppendChar('_');
      } else if (ch < 0x80 && ch > 0x20 && ch != (char)'"' && ch != (char)',' &&
                 "?()<>[]:;@\\.=_".IndexOf((char)ch) < 0) {
        // A visible ASCII character other than specials, quote, comma, ?, or ()
        this.AppendChar((char)ch);
      } else if (ch < 0x80) {
        this.PrepareToAppend(3);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(ch >> 4) & 15]);
        this.currentWord.Append(HexChars[ch & 15]);
      } else if (ch < 0x800) {
        int w = (byte)(0xc0 | ((ch >> 6) & 0x1f));
        int x = (byte)(0x80 | (ch & 0x3f));
        this.PrepareToAppend(6);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(w >> 4) & 15]);
        this.currentWord.Append(HexChars[w & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(x >> 4) & 15]);
        this.currentWord.Append(HexChars[x & 15]);
      } else if (ch < 0x10000) {
        this.PrepareToAppend(9);
        int w = (byte)(0xe0 | ((ch >> 12) & 0x0f));
        int x = (byte)(0x80 | ((ch >> 6) & 0x3f));
        int y = (byte)(0x80 | (ch & 0x3f));
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(w >> 4) & 15]);
        this.currentWord.Append(HexChars[w & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(x >> 4) & 15]);
        this.currentWord.Append(HexChars[x & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(y >> 4) & 15]);
        this.currentWord.Append(HexChars[y & 15]);
      } else {
        this.PrepareToAppend(12);
        int w = (byte)(0xf0 | ((ch >> 18) & 0x07));
        int x = (byte)(0x80 | ((ch >> 12) & 0x3f));
        int y = (byte)(0x80 | ((ch >> 6) & 0x3f));
        int z = (byte)(0x80 | (ch & 0x3f));
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(w >> 4) & 15]);
        this.currentWord.Append(HexChars[w & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(x >> 4) & 15]);
        this.currentWord.Append(HexChars[x & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(y >> 4) & 15]);
        this.currentWord.Append(HexChars[y & 15]);
        this.currentWord.Append('=');
        this.currentWord.Append(HexChars[(z >> 4) & 15]);
        this.currentWord.Append(HexChars[z & 15]);
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.fullString.ToString();
    }
  }
}
