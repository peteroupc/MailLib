package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

    /**
     * Implements the LZ4 algorithm (see "LZ4 Format Description" by Y Collet for
     * more information).
     */
  final class Lz4 {
private Lz4() {
}
    /**
     * Decompresses a byte array compressed using the LZ4 format.
     * @param input Input byte array.
     * @return Decompressed output byte array.
     * @throws NullPointerException The parameter "output" is null.
     */
    public static byte[] Decompress(byte[] input) {
      int index = 0;
      byte[] copy = new byte[16];
      byte[] output = new byte[8 + (input.length * 3 / 2)];
      int outputPos = 0;
      while (index < input.length) {
        int b = input[index];
        int literalLength = (b >> 4) & 15;
        int matchLength = b & 15;
        ++index;
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
        if (index + literalLength - 1 >= input.length) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        if (literalLength > 0) {
          if (output.length - outputPos < literalLength) {
            int newSize = (outputPos + literalLength + 1000);
            byte[] newoutput = new byte[newSize];
            System.arraycopy(output, 0, newoutput, 0, outputPos);
            output = newoutput;
          }
          System.arraycopy(input, index, output, outputPos, literalLength);
          outputPos += literalLength;
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
        // System.out.println("match=" + matchLength + " offset=" + offset +
        // " index=" + index);
        int pos = outputPos - offset;
        if (pos < 0) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        if (matchLength > offset) {
          throw new IllegalArgumentException("Invalid LZ4");
        }
        if (matchLength > copy.length) {
          copy = new byte[matchLength];
        }
        if (output == null) {
          throw new NullPointerException("output");
        }
        if (pos < 0) {
          throw new IllegalArgumentException("pos (" + pos +
            ") is less than " + 0);
        }
        if (pos > outputPos) {
          throw new IllegalArgumentException("pos (" + pos + ") is more than " +
            output.length);
        }
        if (matchLength < 0) {
          throw new IllegalArgumentException("matchLength (" + matchLength +
            ") is less than " + 0);
        }
        if (matchLength > outputPos) {
          throw new IllegalArgumentException("matchLength (" + matchLength +
            ") is more than " + outputPos);
        }
        if (outputPos - pos < matchLength) {
          throw new IllegalArgumentException("outputPos minus " + pos + " (" +
            (outputPos - pos) + ") is less than " + matchLength);
        }
        System.arraycopy(output, pos, copy, 0, matchLength);
        if (output.length - outputPos < matchLength) {
          int newSize = (outputPos + matchLength + 1000);
          byte[] newoutput = new byte[newSize];
          System.arraycopy(output, 0, newoutput, 0, outputPos);
          output = newoutput;
        }
        System.arraycopy(copy, 0, output, outputPos, matchLength);
        outputPos += matchLength;
      }
      byte[] ret = new byte[outputPos];
      System.arraycopy(output, 0, ret, 0, outputPos);
      return ret;
    }
  }
