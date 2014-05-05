package com.upokecenter.text;

import java.util.*;
import java.io.*;

    /**
     * A character input with an integer array as the backing store.
     */
  final class IntArrayCharacterInput implements ICharacterInput {
    private int pos;
    private int[] ilist;

    public IntArrayCharacterInput (int[] ilist) {
      this.ilist = ilist;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int ReadChar() {
      int[] arr = this.ilist;
      if (this.pos < this.ilist.length) {
        return arr[this.pos++];
      }
      return -1;
    }

    /**
     * Not documented yet.
     * @param buf An array of 32-bit unsigned integers.
     * @param offset A 32-bit signed integer. (2).
     * @param unitCount A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int Read(int[] buf, int offset, int unitCount) {
      if (buf == null) {
        throw new NullPointerException("buf");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > buf.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)buf.length));
      }
      if (unitCount < 0) {
        throw new IllegalArgumentException("unitCount (" + Long.toString((long)unitCount) + ") is less than " + "0");
      }
      if (unitCount > buf.length) {
        throw new IllegalArgumentException("unitCount (" + Long.toString((long)unitCount) + ") is more than " + Long.toString((long)buf.length));
      }
      if (buf.length - offset < unitCount) {
        throw new IllegalArgumentException("buf's length minus " + offset + " (" + Long.toString((long)(buf.length - offset)) + ") is less than " + Long.toString((long)unitCount));
      }
      if (unitCount == 0) {
        return 0;
      }
      int[] arr = this.ilist;
      int size = this.ilist.length;
      int count = 0;
      while (this.pos < size && unitCount > 0) {
        buf[offset] = arr[this.pos];
        ++offset;
        ++count;
        --unitCount;
        ++this.pos;
      }
      return count == 0 ? -1 : count;
    }
  }
