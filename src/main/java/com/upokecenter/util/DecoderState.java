package com.upokecenter.text;

import com.upokecenter.mail.*;

class DecoderState {
  int[] bytes;
  int[] chars;
  int prependedBytes;
  int charCount;
  int charOffset;
  ITransform stream;
  public DecoderState (ITransform stream, int initialSize) {
    this.bytes = new int[initialSize];
  }
  public void AppendChar(int ch) {
    if (chars == null) {
      chars = new int[4];
    } else if (charCount >= chars.length) {
      int[] newchars = new int[chars.length + 8];
      System.arraycopy(this.chars, 0, newchars, 0, this.chars.length);
      this.chars = newchars;
    }
    this.chars[charCount]=ch;
    ++charCount;
  }
  public int GetChar() {
    if (charCount == 0) {
 return -1;
}
    int c = chars[charOffset++];
    if (charOffset >= charCount) {
      charCount = 0;
      charOffset = 0;
    }
    return c;
  }
  public int read() throws java.io.IOException {
   if (prependedBytes>0) {
    --prependedBytes;
    int b = this.bytes[prependedBytes];
    return b;
   } else {
    return this.stream.read();
   }
  }
  public void PrependOne(int b1) {
    if (prependedBytes + 1>this.bytes.length) {
      int[] newbytes = new int[prependedBytes + 8];
      System.arraycopy(this.bytes, 0, newbytes, 0, this.bytes.length);
      this.bytes = newbytes;
    }
    bytes[prependedBytes++]=b1;
  }
  public void PrependTwo(int b1, int b2) {
    if (prependedBytes + 2>this.bytes.length) {
      int[] newbytes = new int[prependedBytes + 8];
      System.arraycopy(this.bytes, 0, newbytes, 0, this.bytes.length);
      this.bytes = newbytes;
    }
    bytes[prependedBytes++]=b2;
    bytes[prependedBytes++]=b1;
  }
  public void PrependThree(int b1, int b2, int b3) {
    if (prependedBytes + 3>this.bytes.length) {
      int[] newbytes = new int[prependedBytes + 8];
      System.arraycopy(this.bytes, 0, newbytes, 0, this.bytes.length);
      this.bytes = newbytes;
    }
    bytes[prependedBytes++]=b3;
    bytes[prependedBytes++]=b2;
    bytes[prependedBytes++]=b1;
  }
}
