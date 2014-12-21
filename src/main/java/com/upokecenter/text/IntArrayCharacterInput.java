package com.upokecenter.text;

    /**
     * A character input with an integer array as the backing store.
     */
  final class IntArrayCharacterInput implements ICharacterInput {
    private int pos;
    private int[] ilist;

    public IntArrayCharacterInput (int[] ilist) {
      this.ilist = ilist;
    }

    public int ReadChar() {
      int[] arr = this.ilist;
      return (this.pos < this.ilist.length) ? arr[this.pos++] : (-1);
    }

    public int Read(int[] buf, int offset, int unitCount) {
      if (buf == null) {
        throw new NullPointerException("buf");
      }
      if (offset < 0) {
    throw new IllegalArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > buf.length) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          buf.length);
      }
      if (unitCount < 0) {
        throw new IllegalArgumentException("unitCount (" + unitCount +
          ") is less than " + "0");
      }
      if (unitCount > buf.length) {
        throw new IllegalArgumentException("unitCount (" + unitCount +
          ") is more than " + buf.length);
      }
      if (buf.length - offset < unitCount) {
        throw new IllegalArgumentException("buf's length minus " + offset + " (" +
          (buf.length - offset) + ") is less than " + unitCount);
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
