package com.upokecenter.test; import com.upokecenter.util.*;

import com.upokecenter.util.*;

  class ByteArrayTransform implements ITransform {
    byte[] bytes;
    int offset;
    int endOffset;

    public ByteArrayTransform (byte[] bytes) {
      if ((bytes) == null) {
  throw new NullPointerException("bytes");
}
       this.bytes = bytes;
      this.offset = 0;
      this.endOffset = bytes.length;
    }

    public ByteArrayTransform (byte[] bytes, int offset, int length) {
      if ((bytes) == null) {
  throw new NullPointerException("bytes");
}
if (offset < 0) {
  throw new IllegalArgumentException("offset (" + offset +
    ") is less than " + 0);
}
if (offset > bytes.length) {
  throw new IllegalArgumentException("offset (" + offset + ") is more than "+
    bytes.length);
}
if (length < 0) {
  throw new IllegalArgumentException("length (" + length +
    ") is less than " + 0);
}
if (length > bytes.length) {
  throw new IllegalArgumentException("length (" + length + ") is more than "+
    bytes.length);
}
if (bytes.length-offset < length) {
  throw new IllegalArgumentException("bytes's length minus " + offset + " (" +
    (bytes.length-offset) + ") is less than " + length);
}
      this.bytes = bytes;
      this.offset = offset;
      this.endOffset = offset + length;
    }

    public int read() {
      if (offset >= endOffset) {
 return -1;
}
      int b = bytes[offset++];
      return ((int)b) & 0xff;
    }
  }
