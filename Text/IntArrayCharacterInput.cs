using System;

namespace PeterO.Text {
    /// <summary>A character input with an integer array as the backing
    /// store.</summary>
  internal sealed class IntArrayCharacterInput : ICharacterInput {
    private int pos;
    private readonly int[] ilist;

    public IntArrayCharacterInput(int[] ilist) {
      this.ilist = ilist;
    }

    public int ReadChar() {
      int[] arr = this.ilist;
      return (this.pos < this.ilist.Length) ? arr[this.pos++] : (-1);
    }

    public int Read(int[] buf, int offset, int unitCount) {
      if (buf == null) {
        throw new ArgumentNullException("buf");
      }
      if (offset < 0) {
    throw new ArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > buf.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          buf.Length);
      }
      if (unitCount < 0) {
        throw new ArgumentException("unitCount (" + unitCount +
          ") is less than " + "0");
      }
      if (unitCount > buf.Length) {
        throw new ArgumentException("unitCount (" + unitCount +
          ") is more than " + buf.Length);
      }
      if (buf.Length - offset < unitCount) {
        throw new ArgumentException("buf's length minus " + offset + " (" +
          (buf.Length - offset) + ") is less than " + unitCount);
      }
      if (unitCount == 0) {
        return 0;
      }
      int[] arr = this.ilist;
      int size = this.ilist.Length;
      var count = 0;
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
}
