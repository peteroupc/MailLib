using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncodingUtf16BE : ICharacterEncoding {
   public ICharacterDecoder GetDecoder() {
      return new EncodingUtf16.Decoder(true);
    }

    public ICharacterEncoder GetEncoder() {
      return new EncodingUtf16.Encoder(true);
    }
  }
}
