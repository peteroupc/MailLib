using System;
using System.IO;

namespace PeterO.Text {
  internal static class UnicodeDatabase {
    private static String ToString(byte[] array, int index, int length) {
      System.Text.StringBuilder builder = new System.Text.StringBuilder();
      bool first = true;
      builder.Append("[");
      for (int i = 0; i < length; ++i) {
        if (!first) {
          builder.Append(", ");
        }
        builder.Append(String.Empty + array[i + index]);
        first = false;
      }
      builder.Append("]");
      return builder.ToString();
    }

    /// <summary>Decompresses a byte array compressed using the LZ4 format.</summary>
    /// <param name='input'>Input byte array.</param>
    /// <returns>Decompressed output byte array.</returns>
    private static byte[] Lz4Decompress(byte[] input) {
      int index = 0;
      byte[] copy = new byte[16];
      using (MemoryStream ms = new MemoryStream()) {
        while (index < input.Length) {
          int b = input[index];
          int literalLength = (b >> 4) & 15;
          int matchLength = b & 15;
          ++index;
          // Console.WriteLine("New token, index=" + (index));
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
          // Console.WriteLine("literal=" + literalLength + ", index=" + (index));
          if (index + literalLength - 1 >= input.Length) {
            throw new ArgumentException("Invalid LZ4");
          }
          if (literalLength > 0) {
            ms.Write(input, index, literalLength);
            // Console.WriteLine("literal [idx="+index+"] "+ToString(input,index,literalLength));
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
          // Console.WriteLine("offset=" + offset + ", index=" + (index));
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
          long pos = ms.Position - offset;
          long oldPos = ms.Position;

          if (pos < 0) {
            throw new ArgumentException("Invalid LZ4");
          }
          if (matchLength > offset) {
            throw new ArgumentException("Invalid LZ4");
          }
          if (matchLength > copy.Length) {
            copy = new byte[matchLength];
          }
          ms.Position = pos;
          ms.Read(copy, 0, matchLength);
          // Console.WriteLine("match "+ToString(copy,0,matchLength));
          ms.Position = oldPos;
          ms.Write(copy, 0, matchLength);
        }
        return ms.ToArray();
      }
    }

    /// <summary>Not documented yet.</summary>
    public sealed class ByteData {
      private byte[] array;

      public ByteData(String path) {
        throw new NotSupportedException();
      }

      public ByteData(byte[] array) {
        this.array = array;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='cp'>A 32-bit signed integer.</param>
    /// <returns>A Boolean object.</returns>
      public bool ReadBoolean(int cp) {
        if (cp < 0) {
          throw new ArgumentException("cp (" + Convert.ToString((long)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        if (cp > 0x10ffff) {
          throw new ArgumentException("cp (" + Convert.ToString((long)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)0x10ffff, System.Globalization.CultureInfo.InvariantCulture));
        }
        int b = this.array[cp >> 13] & 0xff;
        switch (b) {
          case 0xfe:
            return false;
          case 0xff:
            return true;
          default:
            {
              int t = cp & 8191;
              int index = 136 + (b << 10) + (t >> 3);
              return (this.array[index] & (1 << (t & 7))) > 0;
            }
        }
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='cp'>A 32-bit signed integer.</param>
    /// <returns>A Byte object.</returns>
      public byte ReadByte(int cp) {
        if (cp < 0) {
          throw new ArgumentException("cp (" + Convert.ToString((long)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        if (cp > 0x10ffff) {
          throw new ArgumentException("cp (" + Convert.ToString((long)cp, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)0x10ffff, System.Globalization.CultureInfo.InvariantCulture));
        }
        int index = (cp >> 9) << 1;
        int x = this.array[index + 1];
        if ((x & 0x80) != 0) {  // Indicates a default value.
          return this.array[index];
        } else {
          x = (x << 8) | (((int)this.array[index]) & 0xff);  // Indicates an array block.
          index = 0x1100 + (x << 9) + (cp & 511);
          return this.array[index];
        }
      }
    }

    private static ByteData classes = null;
    private static Object classesSyncRoot = new Object();

    public static int GetCombiningClass(int cp) {
      lock (classesSyncRoot) {
        if (classes == null) {
          classes = new ByteData(Lz4Decompress(NormalizationData.CombiningClasses));
        }
      }
      return ((int)classes.ReadByte(cp)) & 0xff;
    }

    private static ByteData idnaCat = null;
    private static Object idnaCatSyncRoot = new Object();

    public static int GetIdnaCategory(int cp) {
      lock (idnaCatSyncRoot) {
        if (idnaCat == null) {
          idnaCat = new ByteData(Lz4Decompress(IdnaData.IdnaCategories));
        }
      }
      return ((int)idnaCat.ReadByte(cp)) & 0xff;
    }

    private static ByteData combmark = null;
    private static Object valueCmSyncRoot = new Object();

    public static bool IsCombiningMark(int cp) {
      lock (valueCmSyncRoot) {
        if (combmark == null) {
          combmark = new ByteData(Lz4Decompress(
            IdnaData.CombiningMarks));
        }
        return combmark.ReadBoolean(cp);
      }
    }

    private static ByteData stablenfc = null;
    private static ByteData stablenfd = null;
    private static ByteData stablenfkc = null;
    private static ByteData stablenfkd = null;
    private static Object stableSyncRoot = new Object();

    public static bool IsStableCodePoint(int cp, Normalization form) {
      lock (stableSyncRoot) {
        if (form == Normalization.NFC) {
          if (stablenfc == null) {
            stablenfc = new ByteData(Lz4Decompress(NormalizationData.StableNFC));
          }
          return stablenfc.ReadBoolean(cp);
        }
        if (form == Normalization.NFD) {
          if (stablenfd == null) {
            stablenfd = new ByteData(Lz4Decompress(NormalizationData.StableNFD));
          }
          return stablenfd.ReadBoolean(cp);
        }
        if (form == Normalization.NFKC) {
          if (stablenfkc == null) {
            stablenfkc = new ByteData(Lz4Decompress(NormalizationData.StableNFKC));
          }
          return stablenfkc.ReadBoolean(cp);
        }
        if (form == Normalization.NFKD) {
          if (stablenfkd == null) {
            stablenfkd = new ByteData(Lz4Decompress(NormalizationData.StableNFKD));
          }
          return stablenfkd.ReadBoolean(cp);
        }
        return false;
      }
    }

    private static int[] decomps = null;

    public static int GetDecomposition(int cp, bool compat, int[] buffer, int offset) {
      if (cp < 0x80) {
        // ASCII characters have no decomposition
        buffer[offset++] = cp;
        return offset;
      }
      decomps = NormalizationData.DecompMappings;
      int left = 0;
      int right = decomps[0] - 1;
      while (left <= right) {
        int index = (left + right) >> 1;
        int realIndex = 1 + (index << 1);
        if (decomps[realIndex] == cp) {
          int data = decomps[realIndex + 1];
          if ((data & (1 << 23)) > 0 && !compat) {
            buffer[offset++] = cp;
            return offset;
          }
          if ((data & (1 << 22)) > 0) {
            // Singleton
            buffer[offset++] = data & 0x1fffff;
            return offset;
          }
          int size = data >> 24;
          if (size > 0) {
            if ((data & (1 << 23)) > 0) {
              realIndex = data & 0x1fffff;
              Array.Copy(NormalizationData.CompatDecompMappings, realIndex, buffer, offset, size);
            } else {
              realIndex = 1 + (decomps[0] << 1) + (data & 0x1fffff);
              Array.Copy(decomps, realIndex, buffer, offset, size);
            }
            buffer[offset] &= 0x1fffff;
          }
          return offset + size;
        } else if (decomps[realIndex] < cp) {
          left = index + 1;
        } else {
          right = index - 1;
        }
      }
      buffer[offset++] = cp;
      return offset;
    }

    private static int pairsLength;
    private static int[] pairs = null;
    private static Object pairsSyncRoot = new Object();

    private static void EnsurePairs() {
      lock (pairsSyncRoot) {
        if (pairs == null) {
          pairs = NormalizationData.ComposedPairs;
          pairsLength = pairs.Length / 3;
        }
      }
    }

    public static int GetComposedPair(int first, int second) {
      if (((first | second) >> 17) != 0) {
        return -1;
      }
      EnsurePairs();
      int left = 0;
      int right = pairsLength - 1;
      while (left <= right) {
        int index = (left + right) >> 1;
        int realIndex = index * 3;
        if (pairs[realIndex] == first) {
          if (pairs[realIndex + 1] == second) {
            return pairs[realIndex + 2];
          } else if (pairs[realIndex + 1] < second) {
            left = index + 1;
          } else {
            right = index - 1;
          }
        } else if (pairs[realIndex] < first) {
          left = index + 1;
        } else {
          right = index - 1;
        }
      }
      return -1;
    }
  }
}
