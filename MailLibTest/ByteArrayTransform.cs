using System;

using PeterO;

namespace MailLibTest {
  class ByteArrayTransform : ITransform {
    byte[] bytes;
    int offset;
    int endOffset;

    public ByteArrayTransform(byte[] bytes) {
      if ((bytes) == null) {
  throw new ArgumentNullException("bytes");
}
       this.bytes = bytes;
      this.offset = 0;
      this.endOffset = bytes.Length;
    }

    public ByteArrayTransform(byte[] bytes, int offset, int length) {
      if ((bytes) == null) {
  throw new ArgumentNullException("bytes");
}
if (offset < 0) {
  throw new ArgumentException("offset (" + offset +
    ") is less than " + 0);
}
if (offset > bytes.Length) {
  throw new ArgumentException("offset (" + offset + ") is more than "+
    bytes.Length);
}
if (length < 0) {
  throw new ArgumentException("length (" + length +
    ") is less than " + 0);
}
if (length > bytes.Length) {
  throw new ArgumentException("length (" + length + ") is more than "+
    bytes.Length);
}
if (bytes.Length-offset < length) {
  throw new ArgumentException("bytes's length minus " + offset + " (" +
    (bytes.Length-offset) + ") is less than " + length);
}
      this.bytes = bytes;
      this.offset = offset;
      this.endOffset = offset + length;
    }

    public int ReadByte() {
      if (offset >= endOffset) {
 return -1;
}
      int b = bytes[offset++];
      return ((int)b) & 0xff;
    }
  }
}
