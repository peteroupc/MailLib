using System;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Mail;
using PeterO.Text;
namespace PeterO.Text.Encoders {
  internal class EncoderHelper {
    private class DecoderToInputClass : ICharacterInput {
      private ITransform stream;
      private ICharacterDecoder reader;

      public DecoderToInputClass(ICharacterDecoder reader, ITransform stream) {
        this.reader = reader;
        this.stream = stream;
      }

      public int ReadChar() {
        int c = this.reader.ReadChar(this.stream);
        return (c == -2) ? 0xfffd : c;
      }

      public int Read(int[] buffer, int offset, int length) {
        if (buffer == null) {
          throw new ArgumentNullException("buffer");
        }
        if (offset < 0) {
          throw new ArgumentException("offset (" + offset +
            ") is less than " + 0);
        }
        if (offset > buffer.Length) {
          throw new ArgumentException("offset (" + offset + ") is more than " +
            buffer.Length);
        }
        if (length < 0) {
          throw new ArgumentException("length (" + length +
            ") is less than " + 0);
        }
        if (length > buffer.Length) {
          throw new ArgumentException("length (" + length + ") is more than " +
            buffer.Length);
        }
        if (buffer.Length - offset < length) {
          throw new ArgumentException("buffer's length minus " + offset + " (" +
            (buffer.Length - offset) + ") is less than " + length);
        }
        int count = 0;
        for (var i = 0; i < length; ++i) {
          int c = this.ReadChar();
          if (c == -1) {
            break;
          }
          buffer[offset] = c;
          ++count;
          ++offset;
        }
        return count;
      }
    }

    public static ICharacterInput DecoderToInput(
   ICharacterDecoder decoder,
   ITransform transform) {
      return new DecoderToInputClass(decoder, transform);
    }

    public static string InputToString(ICharacterInput reader) {
      var builder = new StringBuilder();
      while (true) {
        int c = reader.ReadChar();
        if (c < 0) {
          break;
        }
        if (c <= 0xffff) {
          builder.Append((char)c);
        } else if (c <= 0x10ffff) {
          builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          builder.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
      return builder.ToString();
    }
  }
}
