package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.io.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class ByteArrayTransform implements ITransform {
    private byte[] bytes;
    private int offset;
    private int endOffset;

    public ByteArrayTransform (byte[] bytes) {
      if (bytes == null) {
  throw new NullPointerException("bytes");
}
      this.bytes = bytes;
      this.offset = 0;
      this.endOffset = bytes.length;
    }

    public ByteArrayTransform (byte[] bytes, int offset, int length) {
      if (bytes == null) {
  throw new NullPointerException("bytes");
}
if (offset < 0) {
  throw new IllegalArgumentException("offset (" + offset +
    ") is less than " + 0);
}
if (offset > bytes.length) {
  throw new IllegalArgumentException("offset (" + offset + ") is more than " +
    bytes.length);
}
if (length < 0) {
  throw new IllegalArgumentException("length (" + length +
    ") is less than " + 0);
}
if (length > bytes.length) {
  throw new IllegalArgumentException("length (" + length + ") is more than " +
    bytes.length);
}
if (bytes.length - offset < length) {
  throw new IllegalArgumentException("bytes's length minus " + offset + " (" +
    (bytes.length - offset) + ") is less than " + length);
}
      this.bytes = bytes;
      this.offset = offset;
      this.endOffset = offset + length;
    }

    public int read() {
      if (this.offset >= this.endOffset) {
 return -1;
}
      int b = this.bytes[this.offset++];
      return ((int)b) & 0xff;
    }
  }
