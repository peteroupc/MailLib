package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;

import com.upokecenter.text.*;

  class EncodingAscii implements ICharacterEncoding {
    private static class Decoder implements ICharacterDecoder {
      public int ReadChar(ITransform transform) {
        int b = transform.read();
        return (b < 0) ? (-1) : ((b < 0x80) ? b : -2);
      }
    }

    private static class Encoder implements ICharacterEncoder {
      public int Encode(
      int c,
      InputStream output) {
        if (c < 0) {
          return -1;
        }
        if (c < 0x80) {
          output.write((byte)c);
          return 1;
        }
        return -2;
      }
    }

    private Encoder encoder;
    private Decoder decoder;

    public EncodingAscii () {
      this.encoder = new Encoder();
      this.decoder = new Decoder();
    }

    public ICharacterDecoder GetDecoder() {
      return this.decoder;
    }

    public ICharacterEncoder GetEncoder() {
      return this.encoder;
    }
  }
