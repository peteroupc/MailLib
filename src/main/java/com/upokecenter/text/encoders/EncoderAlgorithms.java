package com.upokecenter.text.encoders;

import java.io.*;

import com.upokecenter.util.*;

import com.upokecenter.text.*;

  public class EncoderAlgorithms {
    private static class DecodeWithFallbackDecoder implements ICharacterDecoder,
      ICharacterEncoding {
      private boolean bomChecked;
      private DecoderState state;
      private ICharacterDecoder decoder;

      public DecodeWithFallbackDecoder (ICharacterEncoding encoding) {
        this.decoder = encoding.GetDecoder();
        this.state = new DecoderState(3);
      }

      public int ReadChar(ITransform input) {
        if (!this.bomChecked) {
          int c = 0;
          int[] buffer = new int[3];
          int bufferCount = 0;
          this.bomChecked = true;
          while (c >= 0 && bufferCount < 3) {
            c = input.read();
            if (c >= 0) {
              buffer[bufferCount++] = c;
            }
          }
          if (bufferCount >= 3 && buffer[0] == 0xef &&
             buffer[1] == 0xbb && buffer[2] == 0xbf) {
            this.decoder = Encodings.UTF8.GetDecoder();
          } else if (bufferCount >= 2 && buffer[0] == 0xfe &&
            buffer[1] == 0xff) {
            if (bufferCount == 3) {
              this.state.PrependOne(buffer[2]);
            }
            this.decoder = new EncodingUtf16BE().GetDecoder();
          } else if (bufferCount >= 2 && buffer[0] == 0xff &&
            buffer[1] == 0xfe) {
            if (bufferCount == 3) {
              this.state.PrependOne(buffer[2]);
            }
            this.decoder = new EncodingUtf16().GetDecoder();
          } else {
            // No BOM found
            if (bufferCount == 1) {
              this.state.PrependOne(buffer[0]);
            } else if (bufferCount == 2) {
              this.state.PrependTwo(buffer[0], buffer[1]);
            } else if (bufferCount == 3) {
              this.state.PrependThree(buffer[0], buffer[1], buffer[2]);
            }
          }
        }
        return this.decoder.ReadChar(
          this.state.ToTransformIfBuffered(input));
      }

      public ICharacterEncoder GetEncoder() {
        throw new UnsupportedOperationException();
      }

      public ICharacterDecoder GetDecoder() {
        return this;
      }
    }

    private static class BomBufferedTransform implements ITransform {
      private int[] buffer;
      private int bufferOffset;
      private int bufferCount;
      private ITransform transform;
      private boolean bomChecked;

      public BomBufferedTransform (ITransform transform) {
        this.buffer = new int[3];
        this.transform = transform;
      }

      private void CheckForUtf8BOM() {
        int c = 0;
        while (c >= 0 && this.bufferCount < 3) {
          c = this.transform.read();
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

      public int read() {
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
        return this.transform.read();
      }
    }

    public static ICharacterInput Utf8DecodeAlgorithmInput(
       ITransform transform) {
      // Implements the "utf-8 decode" algorithm in the Encoding
      // Standard
      if (transform == null) {
        throw new NullPointerException("transform");
      }
      BomBufferedTransform bomTransform = new BomBufferedTransform(transform);
      return Encodings.GetDecoderInput(
Encodings.UTF8,
bomTransform);
    }

    public static int Utf8EncodeAlgorithm(
       ICharacterInput stream,
       InputStream output) {
      // Implements the "utf-8 encode" algorithm
      // in the Encoding Standard
      return EncodeAlgorithm(stream, Encodings.UTF8, output);
    }

    /**
     *
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param stream Not documented yet.
     * @param encoding Not documented yet.
     * @param output Not documented yet. (3).
     */

    /**
     *
     * @throws NullPointerException The parameter {@code stream} or {@code output}
     * is null.
     */

  /**
   * Not documented yet.
   */
    public static int EncodeAlgorithm(
      ICharacterInput stream,
      ICharacterEncoding encoding,
      InputStream output) {
      int total = 0;
      ICharacterEncoder encoder = encoding.GetEncoder();
      // Implements the "encode" algorithm
      // in the Encoding Standard
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      if (output == null) {
        throw new NullPointerException("output");
      }
      DecoderState state = new DecoderState(1);
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
            throw new IllegalArgumentException("code point out of range");
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
      if (transform == null) {
        throw new NullPointerException("transform");
      }
      return Encodings.GetDecoderInput(
Encodings.UTF8,
transform);
    }

    public static ICharacterInput DecodeAlgorithmInput(
       ITransform transform,
       ICharacterEncoding fallbackEncoding) {
      // Implements the "decode" algorithm in the Encoding
      // Standard
      if (transform == null) {
        throw new NullPointerException("transform");
      }
      if (fallbackEncoding == null) {
        throw new NullPointerException("fallbackEncoding");
      }
      DecodeWithFallbackDecoder decoder = new DecodeWithFallbackDecoder(
        fallbackEncoding);
      return Encodings.GetDecoderInput(decoder, transform);
    }
  }
