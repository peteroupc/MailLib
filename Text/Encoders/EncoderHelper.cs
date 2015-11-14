using System;
using System.IO;
using System.Text;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncoderHelper {
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
