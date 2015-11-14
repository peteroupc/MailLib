package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;

import com.upokecenter.text.*;

  class EncodingReplacement implements ICharacterEncoding {
    private static class Decoder implements ICharacterDecoder {
      private int replacement = 0;

      public int ReadChar(ITransform transform) {
        if (this.replacement == 0) {
          this.replacement = 1;
          return -2;
        }
        return -1;
      }
    }

    public EncodingReplacement () {
    }

    public ICharacterDecoder GetDecoder() {
      return new Decoder();
    }

    public ICharacterEncoder GetEncoder() {
      return new EncodingUtf8().GetEncoder();
    }
  }
