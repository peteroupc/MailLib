package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

  class EncodingSingleByte implements ICharacterReader, ICharacterEncoder {
    int[] mapping;
    ITransform transform;
    public EncodingSingleByte (ITransform transform, int[] mapping) {
      if ((mapping) == null) {
  throw new NullPointerException("mapping");
}
      if (!(mapping.length).equals(128)) {
  throw new IllegalArgumentException("mapping.length (" + mapping.length +
    ") is not equal to " + 128);
}
      this.transform = transform;
      this.mapping = mapping;
    }

    public int ReadChar() {
      int b = transform.read();
      if (b < 0) {
 return -1;
}
      return (b < 0x80) ? (b) : (this.mapping.get(b - 0x80));
    }
    public int Encode(
     ICharacterReader reader,
     InputStream output,
     boolean replace) {
      int c = reader.ReadChar();
      if (c < 0) {
 return -1;
}
      if (c < 0x80) {
        output.write((byte)c);
        return 1;
      }
      for (int i = 0; i < mapping.length; ++i) {
        if (mapping.get(i) == c) {
          output.write((byte)(i + 0x80));
          return 1;
        }
      }
      return EncoderHelper.Error(c, output, replace);
    }
  }
