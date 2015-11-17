using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
 internal class EncodingSingleByte : ICharacterEncoding {
   private class Decoder : ICharacterDecoder {
      private int[] codepoints;

      public Decoder(int[] codepoints) {
        this.codepoints = codepoints;
      }

     public int ReadChar(ITransform transform) {
       int b = transform.ReadByte();
       return (b < 0) ? (-1) : ((b < 0x80) ? b : this.codepoints[b - 0x80]);
    }
  }

   private class Encoder : ICharacterEncoder {
      private int[] codepoints;

      public Encoder(int[] codepoints) {
        this.codepoints = codepoints;
      }

    public int Encode(
     int c,
     IWriter output) {
       if (c < 0) {
 return -1;
}
         if (c < 0x80) {
           output.WriteByte((byte)c);
           return 1;
         }
         for (var i = 0; i < this.codepoints.Length; ++i) {
           if (this.codepoints[i] == c) {
             output.WriteByte((byte)(i + 0x80));
             return 1;
           }
         }
         return -2;
    }
  }

   private Encoder encoder;
   private Decoder decoder;

  public EncodingSingleByte(int[] codepoints) {
        if (codepoints == null) {
  throw new ArgumentNullException("codepoints");
}
        if (codepoints.Length != 128) {
  throw new ArgumentException("codepoints.Length (" + codepoints.Length +
    ") is not equal to " + 128);
}
this.encoder = new Encoder(codepoints);
this.decoder = new Decoder(codepoints);
      }

  public ICharacterDecoder GetDecoder() {
    return this.decoder;
  }

  public ICharacterEncoder GetEncoder() {
    return this.encoder;
  }
 }
}
