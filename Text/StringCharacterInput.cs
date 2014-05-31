/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using PeterO;

namespace PeterO.Text {
  internal class StringCharacterInput : ICharacterInput
  {
    private string str;
    private int index;
    private int endIndex;

    public StringCharacterInput(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.str = str;
      this.endIndex = str.Length;
    }

    public StringCharacterInput(string str, int index, int length) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((int)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((int)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length (" + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (str.Length - index < length) {
        throw new ArgumentException("str's length minus " + index + " (" + Convert.ToString((int)(str.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      this.str = str;
      this.index = index;
      this.endIndex = index + length;
    }

    public int ReadChar() {
      if (this.index >= this.endIndex) {
        return -1;
      }
      int c = DataUtilities.CodePointAt(this.str, this.index);
      if (c >= 0x10000) {
 ++this.index;
}
      ++this.index;
      return c;
    }

    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new ArgumentNullException("chars");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((int)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > chars.Length) {
        throw new ArgumentException("index (" + Convert.ToString((int)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)chars.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > chars.Length) {
        throw new ArgumentException("length (" + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)chars.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (chars.Length - index < length) {
        throw new ArgumentException("chars's length minus " + index + " (" + Convert.ToString((int)(chars.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (this.endIndex == this.index) {
        return -1;
      }
      if (length == 0) {
        return 0;
      }
      for (int i = 0; i < length; ++i) {
        int c = this.ReadChar();
        if (c == -1) {
          return (i == 0) ? -1 : i;
        }
        chars[index + i] = c;
      }
      return length;
    }
  }
}
