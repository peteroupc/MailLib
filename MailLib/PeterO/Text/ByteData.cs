using System;

namespace PeterO.Text {
  internal sealed class ByteData {
    private readonly byte[] array;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.ByteData.DecompressLz4(System.Byte[])"]/*'/>
    public static byte[] DecompressLz4(byte[] input) {
      var index = 0;
      var copy = new byte[16];
      var output = new byte[8 + (input.Length * 3 / 2)];
      var outputPos = 0;
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
          if (output.Length - outputPos < literalLength) {
            int newSize = checked(outputPos + literalLength + 1000);
            var newoutput = new byte[newSize];
            Array.Copy(output, 0, newoutput, 0, outputPos);
            output = newoutput;
          }
          Array.Copy(input, index, output, outputPos, literalLength);
          outputPos += literalLength;
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
        int pos = outputPos - offset;
        if (pos < 0) {
          throw new ArgumentException("Invalid LZ4");
        }
        if (matchLength > offset) {
          throw new ArgumentException("Invalid LZ4");
        }
        if (matchLength > copy.Length) {
          copy = new byte[matchLength];
        }
        if (pos > outputPos) {
          throw new ArgumentException("pos (" + pos + ") is more than " +
            output.Length);
        }
        if (matchLength < 0) {
          throw new ArgumentException("matchLength (" + matchLength +
            ") is less than 0");
        }
        if (matchLength > outputPos) {
          throw new ArgumentException("matchLength (" + matchLength +
            ") is more than " + outputPos);
        }
        if (outputPos - pos < matchLength) {
          throw new ArgumentException("outputPos minus " + pos + " (" +
            (outputPos - pos) + ") is less than " + matchLength);
        }
        Array.Copy(output, pos, copy, 0, matchLength);
        if (output.Length - outputPos < matchLength) {
          int newSize = checked(outputPos + matchLength + 1000);
          var newoutput = new byte[newSize];
          Array.Copy(output, 0, newoutput, 0, outputPos);
          output = newoutput;
        }
        Array.Copy(copy, 0, output, outputPos, matchLength);
        outputPos += matchLength;
      }
      var ret = new byte[outputPos];
      Array.Copy(output, 0, ret, 0, outputPos);
      return ret;
    }

    public static ByteData Decompress(byte[] data) {
      return new ByteData(DecompressLz4(data));
    }

    public ByteData(byte[] array) {
      this.array = array;
    }

    public bool GetBoolean(int cp) {
      if (cp < 0) {
        throw new ArgumentException("cp (" + cp + ") is less than " + "0");
      }
      if (cp > 0x10ffff) {
     throw new ArgumentException("cp (" + cp + ") is more than " + 0x10ffff);
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
        throw new ArgumentException("cp (" + cp + ") is less than " + "0");
      }
      if (cp > 0x10ffff) {
     throw new ArgumentException("cp (" + cp + ") is more than " + 0x10ffff);
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
}
