/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Text {
    /// <summary>Implements the LZ4 algorithm (see "LZ4 Format Description"
    /// by Y Collet for more information).</summary>
  internal static class Lz4 {
    /// <summary>Decompresses a byte array compressed using the LZ4
    /// format.</summary>
    /// <param name='input'>Input byte array.</param>
    /// <returns>Decompressed output byte array.</returns>
    public static byte[] Decompress(byte[] input) {
      int index = 0;
      var copy = new byte[16];
      var writer = new ArrayWriter(8 + (input.Length * 3 / 2));
      while (index < input.Length) {
        int b = input[index];
        int literalLength = (b >> 4) & 15;
        int matchLength = b & 15;
        ++index;
        // Literals
        if (literalLength == 15) {
          while (index < input.Length) {
            b = ((int)input[index]) & 0xff;
            literalLength += b;
            ++index;
            if (b != 255) {
              break;
            }
            if (index >= input.Length) {
              throw new ArgumentException("Invalid LZ4");
            }
          }
        }
        if (index + literalLength - 1 >= input.Length) {
          throw new ArgumentException("Invalid LZ4");
        }
        if (literalLength > 0) {
          writer.Write(input, index, literalLength);
          index += literalLength;
        }
        if (index == input.Length) {
          break;
        }
        if (index + 1 >= input.Length) {
          throw new ArgumentException("Invalid LZ4");
        }
        // Match copy
        int offset = ((int)input[index]) & 0xff;
        offset |= (((int)input[index + 1]) & 0xff) << 8;
        index += 2;
        if (offset == 0) {
          throw new ArgumentException("Invalid LZ4");
        }
        if (matchLength == 15) {
          while (index < input.Length) {
            b = ((int)input[index]) & 0xff;
            matchLength += b;
            ++index;
            if (b != 255) {
              break;
            }
            if (index >= input.Length) {
              throw new ArgumentException("Invalid LZ4");
            }
          }
        }
        matchLength += 4;
       // Console.WriteLine("match=" + matchLength + " offset=" + offset +
       // " index=" + index);
        int pos = writer.Position - offset;
        int oldPos = writer.Position;
        if (pos < 0) {
          throw new ArgumentException("Invalid LZ4");
        }
        if (matchLength > offset) {
          throw new ArgumentException("Invalid LZ4");
        }
        if (matchLength > copy.Length) {
          copy = new byte[matchLength];
        }
        writer.Position = pos;
        writer.ReadBytes(copy, 0, matchLength);
        // Console.WriteLine("match "+ToString(copy,0,matchLength));
        writer.Position = oldPos;
        writer.Write(copy, 0, matchLength);
      }
      return writer.ToArray();
    }
  }
}
