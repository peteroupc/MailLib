using System;
using System.IO;

namespace PeterO.Text {
  internal static class UnicodeDatabase {
    /// <summary>Not documented yet.</summary>
    public sealed class ByteData {
      private byte[] array;

      public static ByteData Decompress(byte[] data) {
        return new ByteData(Lz4Decompress(data));
      }

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
          classes = ByteData.Decompress(NormalizationData.CombiningClasses);
        }
      }
      return ((int)classes.ReadByte(cp)) & 0xff;
    }

    private static ByteData idnaCat = null;
    private static Object idnaCatSyncRoot = new Object();

    public static int GetIdnaCategory(int cp) {
      lock (idnaCatSyncRoot) {
        if (idnaCat == null) {
          idnaCat = ByteData.Decompress(IdnaData.IdnaCategories);
        }
      }
      return ((int)idnaCat.ReadByte(cp)) & 0xff;
    }

    private static ByteData combmark = null;
    private static Object valueCmSyncRoot = new Object();

    public static bool IsCombiningMark(int cp) {
      lock (valueCmSyncRoot) {
        if (combmark == null) {
          combmark = ByteData.Decompress(
            IdnaData.CombiningMarks);
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
            stablenfc = ByteData.Decompress(NormalizationData.StableNFC);
          }
          return stablenfc.ReadBoolean(cp);
        }
        if (form == Normalization.NFD) {
          if (stablenfd == null) {
            stablenfd = ByteData.Decompress(NormalizationData.StableNFD);
          }
          return stablenfd.ReadBoolean(cp);
        }
        if (form == Normalization.NFKC) {
          if (stablenfkc == null) {
            stablenfkc = ByteData.Decompress(NormalizationData.StableNFKC);
          }
          return stablenfkc.ReadBoolean(cp);
        }
        if (form == Normalization.NFKD) {
          if (stablenfkd == null) {
            stablenfkd = ByteData.Decompress(NormalizationData.StableNFKD);
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
