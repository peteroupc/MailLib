using System;

namespace PeterO.Text {
    /// <summary>A character input with an integer array as the backing store.</summary>
  internal sealed class IntArrayCharacterInput : ICharacterInput {
    private int pos;
    private int[] ilist;

    public IntArrayCharacterInput(int[] ilist) {
      this.ilist = ilist;
    }

    public int ReadChar() {
      int[] arr = this.ilist;
      if (this.pos < this.ilist.Length) {
        return arr[this.pos++];
      }
      return -1;
    }

    public int Read(int[] buf, int offset, int unitCount) {
      if (buf == null) {
        throw new ArgumentNullException("buf");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((int)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > buf.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((int)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)buf.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (unitCount < 0) {
        throw new ArgumentException("unitCount (" + Convert.ToString((int)unitCount, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (unitCount > buf.Length) {
        throw new ArgumentException("unitCount (" + Convert.ToString((int)unitCount, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)buf.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (buf.Length - offset < unitCount) {
        throw new ArgumentException("buf's length minus " + offset + " (" + Convert.ToString((int)(buf.Length - offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((int)unitCount, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (unitCount == 0) {
        return 0;
      }
      int[] arr = this.ilist;
      int size = this.ilist.Length;
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
}
