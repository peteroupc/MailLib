package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

  class EncodingXUserDefined implements ICharacterReader, ICharacterEncoder {
    ITransform transform;
    public EncodingXUserDefined (ITransform transform) {
      this.transform = transform;
    }

    public int ReadChar() {
      int b = transform.read();
      if (b < 0) {
 return -1;
}
      return (b < 0x80) ? (b) : (0xf700 + b);
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
      if (c >= 0xf780 && c <= 0xf7ff) {
        output.write((byte)(c - 0xf700));
        return 1;
      }
      return EncoderHelper.Error(c, output, replace);
    }
  }
