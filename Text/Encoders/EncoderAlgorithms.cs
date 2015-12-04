using System;
using System.IO;
using System.Text;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncoderAlgorithms {
    private class DecodeWithFallbackDecoder : ICharacterDecoder,
      ICharacterEncoding {
      private bool bomChecked;
      private readonly DecoderState state;
      private ICharacterDecoder decoder;
      private bool useOriginal;

      public DecodeWithFallbackDecoder(ICharacterEncoding encoding) {
        this.decoder = encoding.GetDecoder();
        this.state = new DecoderState(3);
        this.useOriginal = false;
      }

      public int ReadChar(IByteReader input) {
        if (input == null) {
          throw new ArgumentNullException("input");
        }
        if (this.useOriginal) {
          return this.decoder.ReadChar(input);
        }
        if (!this.bomChecked) {
          var c = 0;
          var buffer = new int[3];
          var bufferCount = 0;
          this.bomChecked = true;
          while (c >= 0 && bufferCount < 3) {
            c = input.ReadByte();
            if (c >= 0) {
              buffer[bufferCount++] = c;
            }
          }
          if (bufferCount >= 3 && buffer[0] == 0xef &&
             buffer[1] == 0xbb && buffer[2] == 0xbf) {
            this.decoder = Encodings.UTF8.GetDecoder();
            this.useOriginal = true;
          } else if (bufferCount >= 2 && buffer[0] == 0xfe &&
            buffer[1] == 0xff) {
            if (bufferCount == 3) {
              this.state.PrependOne(buffer[2]);
              this.useOriginal = true;
            } else {
              this.useOriginal = false;
            }
            this.decoder = new EncodingUtf16BE().GetDecoder();
          } else if (bufferCount >= 2 && buffer[0] == 0xff &&
            buffer[1] == 0xfe) {
            if (bufferCount == 3) {
              this.state.PrependOne(buffer[2]);
              this.useOriginal = true;
            } else {
              this.useOriginal = false;
            }
            this.decoder = new EncodingUtf16().GetDecoder();
          } else {
            // No BOM found
            this.useOriginal = false;
            switch (bufferCount) {
              case 1:
                this.state.PrependOne(buffer[0]);
                break;
              case 2:
                this.state.PrependTwo(buffer[0], buffer[1]);
                break;
              case 3:
                this.state.PrependThree(buffer[0], buffer[1], buffer[2]);
                break;
            }
          }
        }
        IByteReader br = this.state.ToTransformIfBuffered(input);
        this.useOriginal = br == input;
        return this.decoder.ReadChar(br);
      }

      public ICharacterEncoder GetEncoder() {
        throw new NotSupportedException();
      }

      public ICharacterDecoder GetDecoder() {
        return this;
      }
    }

    private class BomBufferedTransform : IByteReader {
      private readonly int[] buffer;
      private int bufferOffset;
      private int bufferCount;
      private readonly IByteReader transform;
      private bool bomChecked;

      public BomBufferedTransform(IByteReader transform) {
        this.buffer = new int[3];
        this.transform = transform;
      }

      private void CheckForUtf8BOM() {
        var c = 0;
        while (c >= 0 && this.bufferCount < 3) {
          c = this.transform.ReadByte();
          if (c >= 0) {
            this.buffer[this.bufferCount++] = c;
          }
        }
        if (this.bufferCount >= 3 && this.buffer[0] == 0xef &&
           this.buffer[1] == 0xbb && this.buffer[2] == 0xbf) {
          // UTF-8 BOM found
          this.bufferOffset = this.bufferCount = 0;
        } else {
          // BOM not found
          this.bufferOffset = 0;
        }
      }

      public int ReadByte() {
        if (!this.bomChecked) {
          this.bomChecked = true;
          this.CheckForUtf8BOM();
        }
        if (this.bufferOffset < this.bufferCount) {
          int c = this.buffer[this.bufferOffset++];
          if (this.bufferOffset >= this.bufferCount) {
            this.bufferOffset = this.bufferCount = 0;
          }
          return c;
        }
        return this.transform.ReadByte();
      }
    }

    public static ICharacterInput Utf8DecodeAlgorithmInput(
       IByteReader transform) {
      // Implements the "utf-8 decode" algorithm in the Encoding
      // Standard
      if (transform == null) {
        throw new ArgumentNullException("transform");
      }
      var bomTransform = new BomBufferedTransform(transform);
      return Encodings.GetDecoderInput(
Encodings.UTF8,
bomTransform);
    }

    public static int Utf8EncodeAlgorithm(
       ICharacterInput stream,
       IWriter output) {
      // Implements the "utf-8 encode" algorithm
      // in the Encoding Standard
      return EncodeAlgorithm(stream, Encodings.UTF8, output);
    }

    public static int EncodeAlgorithm(
      ICharacterInput stream,
      ICharacterEncoding encoding,
      IWriter output) {
      var total = 0;
      ICharacterEncoder encoder = encoding.GetEncoder();
      // Implements the "encode" algorithm
      // in the Encoding Standard
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      if (output == null) {
        throw new ArgumentNullException("output");
      }
      var state = new DecoderState(1);
      while (true) {
        int c = state.GetChar();
        if (c < 0) {
          c = stream.ReadChar();
        }
        int r = encoder.Encode(c, output);
        if (r == -1) {
          break;
        }
        if (r == -2) {
          if (c < 0 || c >= 0x110000 || ((c & 0xf800) == 0xd800)) {
            throw new ArgumentException("code point out of range");
          }
          state.AppendChar(0x26);
          state.AppendChar(0x23);
          if (c == 0) {
            state.AppendChar(0x30);
          } else {
            while (c > 0) {
              state.AppendChar(0x30 + (c % 10));
              c /= 10;
            }
          }
          state.AppendChar(0x3b);
        } else {
          total += r;
        }
      }
      return total;
    }

    public static ICharacterInput Utf8DecodeWithoutBOMAlgorithmInput(
       IByteReader transform) {
      // Implements the "utf-8 decode without BOM" algorithm
      // in the Encoding Standard
      if (transform == null) {
        throw new ArgumentNullException("transform");
      }
      return Encodings.GetDecoderInput(
Encodings.UTF8,
transform);
    }

    public static ICharacterInput DecodeAlgorithmInput(
       IByteReader transform,
       ICharacterEncoding fallbackEncoding) {
      // Implements the "decode" algorithm in the Encoding
      // Standard
      if (transform == null) {
        throw new ArgumentNullException("transform");
      }
      if (fallbackEncoding == null) {
        throw new ArgumentNullException("fallbackEncoding");
      }
      var decoder = new DecodeWithFallbackDecoder(
        fallbackEncoding);
      return Encodings.GetDecoderInput(decoder, transform);
    }
  }
}
