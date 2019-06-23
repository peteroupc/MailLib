package com.upokecenter.text;

  final class ByteData {
    private final byte[] array;

    /**
     * Decompresses a byte array compressed using the LZ4 format (see "LZ4 Format
     * Description" by Y Collet for more information).
     * @param input Input byte array.
     * @return Decompressed output byte array.
     * @throws java.lang.NullPointerException The parameter "output" is null.
     * @throws IllegalArgumentException Invalid LZ4.
     */
    public static byte[] DecompressLz4(byte[] input) {
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
        if (pos > outputPos) {
          throw new IllegalArgumentException("pos (" + pos + ") is more than " +
            output.length);
        }
        if (matchLength < 0) {
          throw new IllegalArgumentException("matchLength (" + matchLength +
            ") is less than 0");
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

    public static ByteData Decompress(byte[] data) {
      return new ByteData(DecompressLz4(data));
    }

    public ByteData(byte[] array) {
      this.array = array;
    }

    public boolean GetBoolean(int cp) {
      if (cp < 0) {
        throw new IllegalArgumentException("cp (" + cp + ") is less than " + "0");
      }
      if (cp > 0x10ffff) {
     throw new IllegalArgumentException("cp (" + cp + ") is more than " + 0x10ffff);
      }
      int b = this.array[cp >> 13] & 0xff;
      switch (b) {
        case 0xfe: return false;
        case 0xff: return true;
        default: {
            int index = 136 + (b << 10) + ((cp & 8191) >> 3);
            return (this.array[index] & (1 << (cp & 7))) > 0;
          }
      }
    }

    public byte GetByte(int cp) {
      if (cp < 0) {
        throw new IllegalArgumentException("cp (" + cp + ") is less than " + "0");
      }
      if (cp > 0x10ffff) {
     throw new IllegalArgumentException("cp (" + cp + ") is more than " + 0x10ffff);
      }
      int index = (cp >> 9) << 1;
      int x = this.array[index + 1];
      if ((x & 0x80) != 0) {  // Indicates a default value.
        return this.array[index];
      }
      // Indicates an array block.
      x = (x << 8) | (((int)this.array[index]) & 0xff);
      index = 0x1100 + (x << 9) + (cp & 511);
      return this.array[index];
    }
  }
