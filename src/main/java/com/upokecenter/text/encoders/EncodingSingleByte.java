package com.upokecenter.text.encoders;

import java.io.*;
import com.upokecenter.util.*;

import com.upokecenter.text.*;

 public class EncodingSingleByte implements ICharacterEncoding {
   private static class Decoder implements ICharacterDecoder {
      private int[] mapping;

      public Decoder (int[] mapping) {
        this.mapping = mapping;
      }

     public int ReadChar(ITransform transform) {
       int b = transform.read();
       return (b < 0) ? (-1) : ((b < 0x80) ? b : this.mapping.get(b - 0x80));
    }
  }

   private static class Encoder implements ICharacterEncoder {
      private int[] mapping;

      public Encoder (int[] mapping) {
        this.mapping = mapping;
      }

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
         for (int i = 0; i < this.mapping.length; ++i) {
           if (this.mapping.get(i) == c) {
             output.write((byte)(i + 0x80));
             return 1;
           }
         }
         return -2;
    }
  }

   private Encoder encoder;
   private Decoder decoder;

  public EncodingSingleByte (int[] mapping) {
        if (mapping == null) {
  throw new NullPointerException("mapping");
}
        if (mapping.length != 128) {
  throw new IllegalArgumentException("mapping.length (" + mapping.length +
    ") is not equal to " + 128);
}
this.encoder = new Encoder(mapping);
this.decoder = new Decoder(mapping);
      }

  public ICharacterDecoder GetDecoder() {
    return this.decoder;
  }

  public ICharacterEncoder GetEncoder() {
    return this.encoder;
  }
 }
