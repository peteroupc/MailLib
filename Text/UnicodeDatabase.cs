/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Text {
  internal static class UnicodeDatabase {
    private static ByteData classes;
    private static readonly Object classesSyncRoot = new Object();

    public static int GetCombiningClass(int cp) {
      lock (classesSyncRoot) {
  classes = classes ?? ByteData.Decompress(NormalizationData.CombiningClasses);
      }
      return ((int)classes.GetByte(cp)) & 0xff;
    }

    private static ByteData idnaCat;
    private static readonly Object idnaCatSyncRoot = new Object();

    public static int GetIdnaCategory(int cp) {
      lock (idnaCatSyncRoot) {
        idnaCat = idnaCat ?? ByteData.Decompress(IdnaData.IdnaCategories);
      }
      return ((int)idnaCat.GetByte(cp)) & 0xff;
    }

    private static ByteData combmark;
    private static readonly Object valueCmSyncRoot = new Object();

    public static bool IsCombiningMark(int cp) {
      lock (valueCmSyncRoot) {
        combmark = combmark ?? ByteData.Decompress(IdnaData.CombiningMarks);
        return combmark.GetBoolean(cp);
      }
    }

    private static ByteData stablenfc;
    private static ByteData stablenfd;
    private static ByteData stablenfkc;
    private static ByteData stablenfkd;
    private static readonly Object stableSyncRoot = new Object();

    public static bool IsStableCodePoint(int cp, Normalization form) {
      lock (stableSyncRoot) {
        if (form == Normalization.NFC) {
     stablenfc = stablenfc ?? ByteData.Decompress(NormalizationData.StableNFC);
          return stablenfc.GetBoolean(cp);
        }
        if (form == Normalization.NFD) {
     stablenfd = stablenfd ?? ByteData.Decompress(NormalizationData.StableNFD);
          return stablenfd.GetBoolean(cp);
        }
        if (form == Normalization.NFKC) {
  stablenfkc = stablenfkc ?? ByteData.Decompress(NormalizationData.StableNFKC);
          return stablenfkc.GetBoolean(cp);
        }
        if (form == Normalization.NFKD) {
  stablenfkd = stablenfkd ?? ByteData.Decompress(NormalizationData.StableNFKD);
          return stablenfkd.GetBoolean(cp);
        }
        return false;
      }
    }

    private static int[] decomps;

    public static int GetDecomposition(
int cp,
bool compat,
int[] buffer,
int offset) {
      if (cp < 0x80) {
        // ASCII characters have no decomposition
        buffer[offset++] = cp;
        return offset;
      }
      decomps = NormalizationData.DecompMappings;
      var left = 0;
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
              Array.Copy(
                NormalizationData.CompatDecompMappings,
                realIndex,
                buffer,
                offset,
                size);
            } else {
              realIndex = 1 + (decomps[0] << 1) + (data & 0x1fffff);
              Array.Copy(decomps, realIndex, buffer, offset, size);
            }
            buffer[offset] &= 0x1fffff;
          }
          return offset + size;
        }
        if (decomps[realIndex] < cp) {
          left = index + 1;
        } else {
          right = index - 1;
        }
      }
      buffer[offset++] = cp;
      return offset;
    }

    private static int pairsLength;
    private static int[] pairs;
    private static readonly Object pairsSyncRoot = new Object();

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
      var left = 0;
      int right = pairsLength - 1;
      while (left <= right) {
        int index = (left + right) >> 1;
        int realIndex = index * 3;
        if (pairs[realIndex] == first) {
          if (pairs[realIndex + 1] == second) {
            return pairs[realIndex + 2];
          }
          if (pairs[realIndex + 1] < second) {
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
