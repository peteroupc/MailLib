using System;

using PeterO;

namespace PeterO.Text {
  internal sealed class ByteData {
    private byte[] array;

    public static ByteData Decompress(byte[] data) {
      return new ByteData(Lz4.Decompress(data));
    }

    public ByteData(byte[] array) {
      this.array = array;
    }

    public bool GetBoolean(int cp) {
      if (cp < 0) {
        throw new ArgumentException("cp (" + Convert.ToString((int)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (cp > 0x10ffff) {
        throw new ArgumentException("cp (" + Convert.ToString((int)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)0x10ffff, System.Globalization.CultureInfo.InvariantCulture));
      }
      int b = this.array[cp >> 13] & 0xff;
      switch (b) {
        case 0xfe:
          return false;
        case 0xff:
          return true;
        default:
          {
            int t = cp & 8191;
            int index = 136 + (b << 10) + (t >> 3);
            return (this.array[index] & (1 << (t & 7))) > 0;
          }
      }
    }

    public byte GetByte(int cp) {
      if (cp < 0) {
        throw new ArgumentException("cp (" + Convert.ToString((int)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (cp > 0x10ffff) {
        throw new ArgumentException("cp (" + Convert.ToString((int)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)0x10ffff, System.Globalization.CultureInfo.InvariantCulture));
      }
      int index = (cp >> 9) << 1;
      int x = this.array[index + 1];
      if ((x & 0x80) != 0) {  // Indicates a default value.
        return this.array[index];
      } else {
        x = (x << 8) | (((int)this.array[index]) & 0xff);  // Indicates an array block.
        index = 0x1100 + (x << 9) + (cp & 511);
        return this.array[index];
      }
    }
  }
}
