using System;
using System.Collections.Generic;

namespace PeterO.Text {
  internal sealed class PartialListCharacterInput : ICharacterInput {
    private readonly int endPos;
    private readonly IList<int> ilist;
    private int pos;

    public PartialListCharacterInput(IList<int> ilist, int start, int length) {
      if (ilist == null) {
        throw new ArgumentNullException(nameof(ilist));
      }
      if (start < 0) {
      throw new ArgumentException("start (" + start + ") is less than " +
          "0");
      }
      if (start > ilist.Count) {
        throw new ArgumentException("start (" + start + ") is more than " +
          ilist.Count);
      }
      if (length < 0) {
    throw new ArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > ilist.Count) {
        throw new ArgumentException("length (" + length + ") is more than " +
          ilist.Count);
      }
      if (ilist.Count - start < length) {
        throw new ArgumentException("ilist's length minus " + start + " (" +
          (ilist.Count - start) + ") is less than " + length);
      }
      this.ilist = ilist;
      this.pos = start;
      this.endPos = start + length;
    }

    public PartialListCharacterInput(IList<int> ilist) {
      if (ilist == null) {
  throw new ArgumentNullException(nameof(ilist));
}
      this.ilist = ilist;
      this.pos = 0;
      this.endPos = ilist.Count;
    }

    public int ReadChar() {
      return (this.pos < this.endPos) ? this.ilist[this.pos++] : (-1);
    }

    public int Read(int[] buf, int offset, int unitCount) {
      if (buf == null) {
        throw new ArgumentNullException(nameof(buf));
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
      int size = this.endPos;
      var count = 0;
      while (this.pos < size && unitCount > 0) {
        buf[offset] = this.ilist[this.pos];
        ++offset;
        ++count;
        --unitCount;
        ++this.pos;
      }
      return count == 0 ? -1 : count;
    }
  }
}
