package com.upokecenter.util;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Implements the LZ4 algorithm (see "LZ4 Format Description" by Y Collet
     * for more information).
     */
  final class Lz4 {
private Lz4() {
}
    /**
     * Decompresses a byte array compressed using the LZ4 format.
     * @param input Input byte array.
     * @return Decompressed output byte array.
     */
    public static byte[] Decompress(byte[] input) {
      int index = 0;
      byte[] copy = new byte[16];
      ArrayWriter ms=new ArrayWriter(8 + (input.length * 3 / 2));
      while (index < input.length) {
        int b = input[index];
        int literalLength = (b >> 4) & 15;
        int matchLength = b & 15;
        ++index;
// System.out.println("New token, index=" + (index));
        // Literals
        if (literalLength == 15) {
          while (index < input.length) {
            b = ((int)input[index]) & 0xff;
            literalLength += b;
            ++index;
            if (b != 255) {
              break;
            }
            if (index >= input.length) {
              throw new IllegalArgumentException("Invalid LZ4");
            }
          }
        }
        // System.out.println("literal=" + literalLength + ", index=" + (index));
        if (index + literalLength - 1 >= input.length) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        if (literalLength > 0) {
          ms.WriteBytes(input, index, literalLength);
// System.out.println("literal [idx="+index+", len="+literalLength+"] ");
          index += literalLength;
        }
        if (index == input.length) {
          break;
        }
        if (index + 1 >= input.length) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        // Matcher copy
        int offset = ((int)input[index]) & 0xff;
        offset |= (((int)input[index + 1]) & 0xff) << 8;
        index += 2;
        if (offset == 0) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        if (matchLength == 15) {
          while (index < input.length) {
            b = ((int)input[index]) & 0xff;
            matchLength += b;
            ++index;
            if (b != 255) {
              break;
            }
            if (index >= input.length) {
              throw new IllegalArgumentException("Invalid LZ4");
            }
          }
        }
        matchLength += 4;
       // System.out.println("match=" + matchLength + " offset=" + offset + " index=" + (index));
        int pos = ms.getPosition() - offset;
        int oldPos = ms.getPosition();
        if (pos < 0) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        if (matchLength > offset) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        if (matchLength > copy.length) {
          copy = new byte[matchLength];
        }
        ms.setPosition(pos);
        ms.ReadBytes(copy, 0, matchLength);
        // System.out.println("match "+toString(copy,0,matchLength));
        ms.setPosition(oldPos);
        ms.WriteBytes(copy, 0, matchLength);
      }
      return ms.ToArray();
    }
  }
