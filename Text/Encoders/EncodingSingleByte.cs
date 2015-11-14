using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
 internal class EncodingSingleByte : ICharacterEncoding {
   private class Decoder : ICharacterDecoder {
      private int[] mapping;

      public Decoder(int[] mapping) {
        this.mapping = mapping;
      }

     public int ReadChar(ITransform transform) {
       int b = transform.ReadByte();
       return (b < 0) ? (-1) : ((b < 0x80) ? b : this.mapping[b - 0x80]);
    }
  }

   private class Encoder : ICharacterEncoder {
      private int[] mapping;

      public Encoder(int[] mapping) {
        this.mapping = mapping;
      }

    public int Encode(
     int c,
     Stream output) {
       if (c < 0) {
 return -1;
}
         if (c < 0x80) {
           output.WriteByte((byte)c);
           return 1;
         }
         for (var i = 0; i < this.mapping.Length; ++i) {
           if (this.mapping[i ]==c) {
             output.WriteByte((byte)(i + 0x80));
             return 1;
           }
         }
         return -2;
    }
  }

   private Encoder encoder;
   private Decoder decoder;

  public EncodingSingleByte(int[] mapping) {
        if (mapping == null) {
  throw new ArgumentNullException("mapping");
}
        if (mapping.Length != 128) {
  throw new ArgumentException("mapping.Length (" + mapping.Length +
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
}
