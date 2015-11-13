using System;
using System.IO;
using PeterO;
using PeterO.Mail;
using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncodingUtf16 : ICharacterEncoding {
    internal class Decoder : ICharacterDecoder {
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

      public int ReadChar(ITransform stream) {
        while (true) {
          int b = this.state.ReadByte(stream);
          if (b < 0) {
            if (this.lead >= 0 || this.surrogate >= 0) {
              this.lead = this.surrogate = -1;
              return -2;
            }
            return -1;
          }
          if (lead< 0) {
            lead = b;
            continue;
          }
          int code=(bigEndian) ? b+(lead << 8) : lead+(b << 8);
          lead=-1;
          if (surrogate >= 0) {
            if ((code & 0xfc00) == 0xdc00) {
              code = 0x10000+(code-0xdc00)+((lead-0xd800) << 10);
              lead=-1;
              return code;
            }
            lead=-1;
            int b1=(code >> 8) & 0xff;
            int b2=(code & 0xff);
            if (bigEndian) {
              state.PrependTwo(b1, b2);
            } else {
              state.PrependTwo(b2, b1);
            }
            return -2;
          }
          if ((code & 0xfc00) == 0xd800) {
            surrogate = code;
          } else if ((code & 0xfc00) == 0xdc00) {
            return -2;
          } else {
            return code;
          }
        }
      }
    }

    internal class Encoder : ICharacterEncoder {
      private bool bigEndian;
      public Encoder(bool bigEndian) {
        this.bigEndian = bigEndian;
      }
      public int Encode(
       int c,
       Stream output) {
        if (c < 0) {
          return -1;
        }
        if (c>0x10ffff || ((c & 0xf800) == 0xd800)) {
          return -2;
        }
        int b1, b2;
        if (c <= 0xffff) {
          b1=(c >> 8) & 0xff;
           b2=(c & 0xff);
          if (bigEndian) {
             output.WriteByte((byte)b1);
             output.WriteByte((byte)b2);
          } else {
             output.WriteByte((byte)b2);
             output.WriteByte((byte)b1);
          }
          return 2;
        }
        int c1=((c-0x10000) >> 10)+0xd800;
        int c2=((c-0x10000) & 0x3ff)+0xdc00;
           b1=(c1 >> 8) & 0xff;
          b2=(c1 & 0xff);
          if (bigEndian) {
             output.WriteByte((byte)b1);
             output.WriteByte((byte)b2);
          } else {
             output.WriteByte((byte)b2);
             output.WriteByte((byte)b1);
          }
           b1=(c2 >> 8) & 0xff;
          b2=(c2 & 0xff);
          if (bigEndian) {
             output.WriteByte((byte)b1);
             output.WriteByte((byte)b2);
          } else {
             output.WriteByte((byte)b2);
             output.WriteByte((byte)b1);
          }
        return 4;
      }
    }

    public ICharacterDecoder GetDecoder() {
      return new Decoder(false);
    }

    public ICharacterEncoder GetEncoder() {
      return new Encoder(false);
    }
  }
}
