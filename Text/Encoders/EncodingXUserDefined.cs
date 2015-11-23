using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncodingXUserDefined : ICharacterEncoding {
    private class Decoder : ICharacterDecoder {
      public int ReadChar(IByteReader transform) {
        int b = transform.ReadByte();
        return (b < 0) ? (-1) : ((b < 0x80) ? b : (0xf700 + b));
      }
    }

    private class Encoder : ICharacterEncoder {
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
        if (c >= 0xf780 && c <= 0xf7ff) {
          output.WriteByte((byte)(c - 0xf700));
          return 1;
        }
        return -2;
      }
    }

    private readonly Encoder encoder;
    private readonly Decoder decoder;

    public EncodingXUserDefined() {
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
}
