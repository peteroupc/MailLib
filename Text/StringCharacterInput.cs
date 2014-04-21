/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Text {
  /// <summary>Description of StringCharacterInput.</summary>
  internal class StringCharacterInput : ICharacterInput
  {
    string str;
    int index;
    public StringCharacterInput(string str) {
      if ((str) == null) {
        throw new ArgumentNullException("str");
      }
      this.str = str;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int Read() {
      if (index >= str.Length) {
        return -1;
      }
      int c = str[index];
      if (c >= 0xd800 && c <= 0xdbff && index + 1 < str.Length &&
          str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        c = 0x10000 + ((c - 0xd800) * 0x400) + (str[index + 1] - 0xdc00);
        ++index;
      } else if (c >= 0xd800 && c <= 0xdfff) {
        // unpaired surrogate
        c = 0xfffd;
      }
      ++index;
      return c;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='chars'>An array of 32-bit unsigned integers.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='length'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Read(int[] chars, int index, int length) {
      if ((chars) == null) {
        throw new ArgumentNullException("chars");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)(index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > chars.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)(index), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(chars.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)(length), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > chars.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)(length), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(chars.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (chars.Length-index < length) {
        throw new ArgumentException("chars's length minus " + index + " (" + Convert.ToString((long)(chars.Length-index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)(length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (str.Length == this.index) {
        return -1;
      }
      if (length == 0) {
        return 0;
      }
      for (int i = 0; i < length; ++i) {
        int c = this.Read();
        if (c==-1) {
          return (i == 0) ? -1 : i;
        }
        chars[index + i]=c;
      }
      return length;
    }
  }
}
