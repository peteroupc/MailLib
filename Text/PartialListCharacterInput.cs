using System;
using System.Collections.Generic;

namespace PeterO.Text {
    /// <summary>Not documented yet.</summary>
  internal sealed class PartialListCharacterInput : ICharacterInput {
    private int pos;
    private int endPos;
    private IList<int> ilist;

    public PartialListCharacterInput(IList<int> ilist, int start, int length) {
      if (ilist == null) {
        throw new ArgumentNullException("ilist");
      }
      if (start < 0) {
        throw new ArgumentException("start (" + Convert.ToString((long)start, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (start > ilist.Count) {
        throw new ArgumentException("start (" + Convert.ToString((long)start, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)ilist.Count, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > ilist.Count) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)ilist.Count, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (ilist.Count - start < length) {
        throw new ArgumentException("ilist's length minus " + start + " (" + Convert.ToString((long)(ilist.Count - start), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      this.ilist = ilist;
      this.pos = start;
      this.endPos = start + length;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadChar() {
      if (this.pos < this.endPos) {
        return this.ilist[this.pos++];
      }
      return -1;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='buf'>An array of 32-bit unsigned integers.</param>
    /// <param name='offset'>A 32-bit signed integer. (2).</param>
    /// <param name='unitCount'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Read(int[] buf, int offset, int unitCount) {
      if (buf == null) {
        throw new ArgumentNullException("buf");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > buf.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)buf.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (unitCount < 0) {
        throw new ArgumentException("unitCount (" + Convert.ToString((long)unitCount, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (unitCount > buf.Length) {
        throw new ArgumentException("unitCount (" + Convert.ToString((long)unitCount, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)buf.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (buf.Length - offset < unitCount) {
        throw new ArgumentException("buf's length minus " + offset + " (" + Convert.ToString((long)(buf.Length - offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)unitCount, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (unitCount == 0) {
        return 0;
      }
      int size = this.endPos;
      int count = 0;
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
