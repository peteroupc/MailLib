package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

  final class UnicodeDatabase {
private UnicodeDatabase() {
}
    private static ByteData classes;
    private static final Object classesSyncRoot = new Object();

    public static int GetCombiningClass(int cp) {
      synchronized (classesSyncRoot) {
  classes = (classes == null) ? (ByteData.Decompress(NormalizationData.CombiningClasses)) : classes;
      }
      return ((int)classes.GetByte(cp)) & 0xff;
    }

    private static ByteData idnaCat;
    private static final Object idnaCatSyncRoot = new Object();

    public static int GetIdnaCategory(int cp) {
      synchronized (idnaCatSyncRoot) {
        idnaCat = (idnaCat == null) ? (ByteData.Decompress(IdnaData.IdnaCategories)) : idnaCat;
      }
      return ((int)idnaCat.GetByte(cp)) & 0xff;
    }

    private static ByteData combmark;
    private static final Object valueCmSyncRoot = new Object();

    public static boolean IsCombiningMark(int cp) {
      synchronized (valueCmSyncRoot) {
        combmark = (combmark == null) ? (ByteData.Decompress(IdnaData.CombiningMarks)) : combmark;
        return combmark.GetBoolean(cp);
      }
    }

    private static ByteData stablenfc;
    private static ByteData stablenfd;
    private static ByteData stablenfkc;
    private static ByteData stablenfkd;
    private static final Object stableSyncRoot = new Object();

    public static boolean IsStableCodePoint(int cp, Normalization form) {
      synchronized (stableSyncRoot) {
        if (form == Normalization.NFC) {
     stablenfc = (stablenfc == null) ? (ByteData.Decompress(NormalizationData.StableNFC)) : stablenfc;
          return stablenfc.GetBoolean(cp);
        }
        if (form == Normalization.NFD) {
     stablenfd = (stablenfd == null) ? (ByteData.Decompress(NormalizationData.StableNFD)) : stablenfd;
          return stablenfd.GetBoolean(cp);
        }
        if (form == Normalization.NFKC) {
  stablenfkc = (stablenfkc == null) ? (ByteData.Decompress(NormalizationData.StableNFKC)) : stablenfkc;
          return stablenfkc.GetBoolean(cp);
        }
        if (form == Normalization.NFKD) {
  stablenfkd = (stablenfkd == null) ? (ByteData.Decompress(NormalizationData.StableNFKD)) : stablenfkd;
          return stablenfkd.GetBoolean(cp);
        }
        return false;
      }
    }

    private static int[] decomps;

    public static int GetDecomposition(
int cp,
boolean compat,
int[] buffer,
int offset) {
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
              System.arraycopy(
                NormalizationData.CompatDecompMappings,
                realIndex,
                buffer,
                offset,
                size);
            } else {
              realIndex = 1 + (decomps[0] << 1) + (data & 0x1fffff);
              System.arraycopy(decomps, realIndex, buffer, offset, size);
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
    private static final Object pairsSyncRoot = new Object();

    private static void EnsurePairs() {
      synchronized (pairsSyncRoot) {
        if (pairs == null) {
          pairs = NormalizationData.ComposedPairs;
          pairsLength = pairs.length / 3;
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
