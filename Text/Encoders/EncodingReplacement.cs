using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncodingReplacement : ICharacterEncoding {
    private class Decoder : ICharacterDecoder {
      private int replacement;

      public int ReadChar(IByteReader transform) {
        if (this.replacement == 0) {
          this.replacement = 1;
          return -2;
        }
        return -1;
      }
    }

    public EncodingReplacement() {
    }

    public ICharacterDecoder GetDecoder() {
      return new Decoder();
    }

    public ICharacterEncoder GetEncoder() {
      return new EncodingUtf8().GetEncoder();
    }
  }
}
