package com.upokecenter.text;

import java.util.*;

  final class PartialListCharacterInput implements ICharacterInput {
    private int pos;
    private int endPos;
    private List<Integer> ilist;

    public PartialListCharacterInput (List<Integer> ilist, int start, int length) {
      if (ilist == null) {
        throw new NullPointerException("ilist");
      }
      if (start < 0) {
        throw new IllegalArgumentException("start (" + Integer.toString((int)start) + ") is less than " + "0");
      }
      if (start > ilist.size()) {
        throw new IllegalArgumentException("start (" + Integer.toString((int)start) + ") is more than " + Integer.toString((int)ilist.size()));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is less than " + "0");
      }
      if (length > ilist.size()) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is more than " + Integer.toString((int)ilist.size()));
      }
      if (ilist.size() - start < length) {
        throw new IllegalArgumentException("ilist's length minus " + start + " (" + Integer.toString((int)(ilist.size() - start)) + ") is less than " + Integer.toString((int)length));
      }
      this.ilist = ilist;
      this.pos = start;
      this.endPos = start + length;
    }

    public int ReadChar() {
      return (this.pos < this.endPos) ? this.ilist.get(this.pos++) : (-1);
    }

    public int Read(int[] buf, int offset, int unitCount) {
      if (buf == null) {
        throw new NullPointerException("buf");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Integer.toString((int)offset) + ") is less than " + "0");
      }
      if (offset > buf.length) {
        throw new IllegalArgumentException("offset (" + Integer.toString((int)offset) + ") is more than " + Integer.toString((int)buf.length));
      }
      if (unitCount < 0) {
        throw new IllegalArgumentException("unitCount (" + Integer.toString((int)unitCount) + ") is less than " + "0");
      }
      if (unitCount > buf.length) {
        throw new IllegalArgumentException("unitCount (" + Integer.toString((int)unitCount) + ") is more than " + Integer.toString((int)buf.length));
      }
      if (buf.length - offset < unitCount) {
        throw new IllegalArgumentException("buf's length minus " + offset + " (" + Integer.toString((int)(buf.length - offset)) + ") is less than " + Integer.toString((int)unitCount));
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
