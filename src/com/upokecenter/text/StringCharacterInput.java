package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

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

    /**
     *
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
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is less than " + "0");
      }
      if (index > chars.length) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is more than " + Integer.toString((int)chars.length));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is less than " + "0");
      }
      if (length > chars.length) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is more than " + Integer.toString((int)chars.length));
      }
      if (chars.length - index < length) {
        throw new IllegalArgumentException("chars's length minus " + index + " (" + Integer.toString((int)(chars.length - index)) + ") is less than " + Integer.toString((int)length));
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
