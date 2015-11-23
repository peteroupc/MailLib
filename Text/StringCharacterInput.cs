/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO;

namespace PeterO.Text {
  internal class StringCharacterInput : ICharacterInput
  {
    private readonly string str;
    private int index;
    private readonly int endIndex;

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
      throw new ArgumentException("index (" + index + ") is less than " +
          "0");
      }
      if (index > chars.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          chars.Length);
      }
      if (length < 0) {
    throw new ArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > chars.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          chars.Length);
      }
      if (chars.Length - index < length) {
        throw new ArgumentException("chars's length minus " + index + " (" +
          (chars.Length - index) + ") is less than " + length);
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
