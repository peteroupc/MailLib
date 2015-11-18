using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncodingAscii : ICharacterEncoding {
    private class Decoder : ICharacterDecoder {
      public int ReadChar(IByteReader transform) {
        int b = transform.ReadByte();
        return (b < 0) ? (-1) : ((b < 0x80) ? b : -2);
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
        return -2;
      }
    }

    private Encoder encoder;
    private Decoder decoder;

    public EncodingAscii() {
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