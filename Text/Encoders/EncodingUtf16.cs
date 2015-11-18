using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncodingUtf16 : ICharacterEncoding {
    private class Decoder : ICharacterDecoder {
      private DecoderState state;
      private int lead;
      private int surrogate;
      private bool bigEndian;

      public Decoder(bool bigEndian) {
        this.bigEndian = bigEndian;
        this.state = new DecoderState(1);
        this.lead = -1;
        this.surrogate = -1;
      }

      public int ReadChar(IByteReader stream) {
        while (true) {
          int b = this.state.ReadInputByte(stream);
          if (b < 0) {
            if (this.lead >= 0 || this.surrogate >= 0) {
              this.lead = this.surrogate = -1;
              return -2;
            }
            return -1;
          }
          if (this.lead < 0) {
            this.lead = b;
            continue;
          }
       int code = this.bigEndian ? b + (this.lead << 8) : this.lead + (b << 8);
          this.lead = -1;
          if (this.surrogate >= 0) {
            if ((code & 0xfc00) == 0xdc00) {
              code = 0x10000 + (code - 0xdc00) +((this.lead - 0xd800) << 10);
              this.lead = -1;
              return code;
            }
            this.lead = -1;
            int b1 = (code >> 8) & 0xff;
            int b2 = code & 0xff;
            if (this.bigEndian) {
              this.state.PrependTwo(b1, b2);
            } else {
              this.state.PrependTwo(b2, b1);
            }
            return -2;
          }
          if ((code & 0xfc00) == 0xd800) {
            this.surrogate = code;
          } else if ((code & 0xfc00) == 0xdc00) {
            return -2;
          } else {
            return code;
          }
        }
      }
    }

    private class Encoder : ICharacterEncoder {
      private bool bigEndian;

      public Encoder(bool bigEndian) {
        this.bigEndian = bigEndian;
      }

      public int Encode(
       int c,
       IWriter output) {
        if (c < 0) {
          return -1;
        }
        if (c > 0x10ffff || ((c & 0xf800) == 0xd800)) {
          return -2;
        }
        int b1, b2;
        if (c <= 0xffff) {
          b1 = (c >> 8) & 0xff;
           b2 = c & 0xff;
          if (this.bigEndian) {
             output.WriteByte((byte)b1);
             output.WriteByte((byte)b2);
          } else {
             output.WriteByte((byte)b2);
             output.WriteByte((byte)b1);
          }
          return 2;
        }
        int c1 = ((c - 0x10000) >> 10) +0xd800;
        int c2 = ((c - 0x10000) & 0x3ff) +0xdc00;
           b1 = (c1 >> 8) & 0xff;
          b2 = c1 & 0xff;
          if (this.bigEndian) {
             output.WriteByte((byte)b1);
             output.WriteByte((byte)b2);
          } else {
             output.WriteByte((byte)b2);
             output.WriteByte((byte)b1);
          }
           b1 = (c2 >> 8) & 0xff;
          b2 = c2 & 0xff;
          if (this.bigEndian) {
             output.WriteByte((byte)b1);
             output.WriteByte((byte)b2);
          } else {
             output.WriteByte((byte)b2);
             output.WriteByte((byte)b1);
          }
        return 4;
      }
    }

    internal static ICharacterDecoder GetDecoder2(bool bigEndian) {
      return new Decoder(bigEndian);
    }

    internal static ICharacterEncoder GetEncoder2(bool bigEndian) {
      return new Encoder(bigEndian);
    }

    public ICharacterDecoder GetDecoder() {
      return GetDecoder2(false);
    }

    public ICharacterEncoder GetEncoder() {
      return GetEncoder2(false);
    }
  }
}