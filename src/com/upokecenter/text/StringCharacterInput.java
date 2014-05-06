package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Description of StringCharacterInput.
     */
  class StringCharacterInput implements ICharacterInput
  {
    private String str;
    private int index;
    private int endIndex;

    public StringCharacterInput (String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      this.str = str;
      this.endIndex = str.length();
    }

    public StringCharacterInput (String str, int index, int length) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is more than " + Long.toString((long)str.length()));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > str.length()) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)str.length()));
      }
      if (str.length() - index < length) {
        throw new IllegalArgumentException("str's length minus " + index + " (" + Long.toString((long)(str.length() - index)) + ") is less than " + Long.toString((long)length));
      }
      this.str = str;
      this.index = index;
      this.endIndex = index + length;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int ReadChar() {
      if (this.index >= this.endIndex) {
        return -1;
      }
      int c = this.str.charAt(this.index);
      if ((c & 0xfc00) == 0xd800 && this.index + 1 < this.endIndex &&
          this.str.charAt(this.index + 1) >= 0xdc00 && this.str.charAt(this.index + 1) <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        c = 0x10000 + ((c - 0xd800) << 10) + (this.str.charAt(this.index + 1) - 0xdc00);
        ++this.index;
      } else if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        c = 0xfffd;
      }
      ++this.index;
      return c;
    }

    /**
     * Not documented yet.
     * @param chars An array of 32-bit unsigned integers.
     * @param index A 32-bit signed integer. (2).
     * @param length A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new NullPointerException("chars");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is less than " + "0");
      }
      if (index > chars.length) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is more than " + Long.toString((long)chars.length));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > chars.length) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)chars.length));
      }
      if (chars.length - index < length) {
        throw new IllegalArgumentException("chars's length minus " + index + " (" + Long.toString((long)(chars.length - index)) + ") is less than " + Long.toString((long)length));
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
