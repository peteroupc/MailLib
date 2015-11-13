using System;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Mail;
using PeterO.Text;
namespace PeterO.Text.Encoders {
  internal class EncoderAlgorithms {
    private class DecodeWithFallbackDecoder : ICharacterDecoder {
      bool bomChecked;
      DecoderState state;
      ICharacterDecoder decoder;
      public DecodeWithFallbackDecoder(ICharacterEncoding encoding) {
        this.decoder = encoding.GetDecoder();
        state = new DecoderState(3);
      }

      public int ReadChar(ITransform input) {
        if (!bomChecked) {
          int c = 0;
          var buffer = new int[3];
          int bufferCount = 0;
          bomChecked = true;
          while (c >= 0 && bufferCount < 3) {
            c = input.ReadByte();
            if (c >= 0) {
              buffer[bufferCount++] = c;
            }
          }
          if (bufferCount >= 3 && buffer[0] == 0xef &&
             buffer[1] == 0xbb && buffer[2] == 0xbf) {
               decoder = new EncodingUtf8().GetDecoder();
          } else if (bufferCount >= 2 && buffer[0] == 0xfe &&
            buffer[1] == 0xff) {
              if (bufferCount == 3) {
                state.PrependOne(buffer[2]);
              }
              decoder = new EncodingUtf16BE().GetDecoder();
          } else if (bufferCount >= 2 && buffer[0] == 0xff &&
            buffer[1] == 0xfe) {
            if (bufferCount == 3) {
              state.PrependOne(buffer[2]);
            }
            decoder = new EncodingUtf16().GetDecoder();
          } else {
            // No BOM found
            if (bufferCount == 1) {
              state.PrependOne(buffer[0]);
            } else if (bufferCount == 2) {
              state.PrependTwo(buffer[0], buffer[1]);
            } else if (bufferCount == 3) {
              state.PrependThree(buffer[0], buffer[1], buffer[2]);
            }
          }
        }
        return this.decoder.ReadChar(
          state.ToTransformIfBuffered(input));
      }
    }

    private class BomBufferedTransform : ITransform {
      int[] buffer;
      int bufferOffset;
      int bufferCount;
      ITransform transform;
      bool bomChecked;
      public BomBufferedTransform(ITransform transform) {
        buffer = new int[3];
        this.transform = transform;
      }

      private void CheckForUtf8BOM() {
        int c = 0;
        while (c >= 0 && bufferCount < 3) {
          c = transform.ReadByte();
          if (c >= 0) {
            buffer[bufferCount++] = c;
          }
        }
        if (bufferCount >= 3 && buffer[0] == 0xef &&
           buffer[1] == 0xbb && buffer[2] == 0xbf) {
          // UTF-8 BOM found
          bufferOffset = bufferCount = 0;
        } else {
          // BOM not found
          bufferOffset = 0;
        }
      }

      public int ReadByte() {
        if (!bomChecked) {
          bomChecked = true;
          CheckForUtf8BOM();
        }
        if (bufferOffset < bufferCount) {
          int c = buffer[bufferOffset++];
          if (bufferOffset >= bufferCount) {
            bufferOffset = bufferCount = 0;
          }
          return c;
        }
        return this.transform.ReadByte();
      }
    }

    public static ICharacterInput Utf8DecodeAlgorithmInput(
       ITransform transform) {
      // Implements the "utf-8 decode" algorithm in the Encoding
      // Standard
      if ((transform) == null) {
  throw new ArgumentNullException("transform");
}
      var bomTransform = new BomBufferedTransform(transform);
      return EncoderHelper.DecoderToInput(
        new EncodingUtf8().GetDecoder(),
        bomTransform);
    }

    public static int Utf8EncodeAlgorithm(
       ICharacterInput stream,
       Stream output) {
      // Implements the "utf-8 encode" algorithm
      // in the Encoding Standard
      return EncodeAlgorithm(stream, new EncodingUtf8(), output);
    }

    public static int EncodeAlgorithm(
      ICharacterInput stream,
      ICharacterEncoding encoding,
      Stream output) {
      int total = 0;
      ICharacterEncoder encoder = encoding.GetEncoder();
      // Implements the "encode" algorithm
      // in the Encoding Standard
      if ((stream) == null) {
  throw new ArgumentNullException("stream");
}
      if ((output) == null) {
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
       ITransform transform) {
      // Implements the "utf-8 decode without BOM" algorithm
      // in the Encoding Standard
      if ((transform) == null) {
  throw new ArgumentNullException("transform");
}
      return EncoderHelper.DecoderToInput(
        new EncodingUtf8().GetDecoder(),
        transform);
    }

    public static ICharacterInput DecodeAlgorithmInput(
       ITransform transform,
       ICharacterEncoding fallbackEncoding) {
      // Implements the "decode" algorithm in the Encoding
      // Standard
      if ((transform) == null) {
  throw new ArgumentNullException("transform");
}
      if ((fallbackEncoding) == null) {
  throw new ArgumentNullException("fallbackEncoding");
}
      ICharacterDecoder decoder = new DecodeWithFallbackDecoder(
        fallbackEncoding);
      return EncoderHelper.DecoderToInput(decoder, transform);
    }
  }
}
