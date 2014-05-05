package com.upokecenter.text;

import java.util.*;

    /**
     * Not documented yet.
     */
  final class PartialListCharacterInput implements ICharacterInput {
    private int pos;
    private int endPos;
    private List<Integer> ilist;

    public PartialListCharacterInput (List<Integer> ilist, int start, int length) {
      if (ilist == null) {
        throw new NullPointerException("ilist");
      }
      if (start < 0) {
        throw new IllegalArgumentException("start (" + Long.toString((long)start) + ") is less than " + "0");
      }
      if (start > ilist.size()) {
        throw new IllegalArgumentException("start (" + Long.toString((long)start) + ") is more than " + Long.toString((long)ilist.size()));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > ilist.size()) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)ilist.size()));
      }
      if (ilist.size() - start < length) {
        throw new IllegalArgumentException("ilist's length minus " + start + " (" + Long.toString((long)(ilist.size() - start)) + ") is less than " + Long.toString((long)length));
      }
      this.ilist = ilist;
      this.pos = start;
      this.endPos = start + length;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int ReadChar() {
      if (this.pos < this.endPos) {
        return this.ilist.get(this.pos++);
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
      int size = this.endPos;
      int count = 0;
      while (this.pos < size && unitCount > 0) {
        buf[offset] = this.ilist.get(this.pos);
        ++offset;
        ++count;
        --unitCount;
        ++this.pos;
      }
      return count == 0 ? -1 : count;
    }
  }
