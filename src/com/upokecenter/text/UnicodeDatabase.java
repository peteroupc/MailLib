package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  final class UnicodeDatabase {
private UnicodeDatabase() {
}
    private static ByteData classes = null;
    private static Object classesSyncRoot = new Object();

    public static int GetCombiningClass(int cp) {
      synchronized(classesSyncRoot) {
        if (classes == null) {
          classes = ByteData.Decompress(PeterO.Text.NormalizationData.CombiningClasses);
        }
      }
      return ((int)classes.GetByte(cp)) & 0xff;
    }

    private static ByteData idnaCat = null;
    private static Object idnaCatSyncRoot = new Object();

    public static int GetIdnaCategory(int cp) {
      synchronized(idnaCatSyncRoot) {
        if (idnaCat == null) {
          idnaCat = ByteData.Decompress(IdnaData.IdnaCategories);
        }
      }
      return ((int)idnaCat.GetByte(cp)) & 0xff;
    }

    private static ByteData combmark = null;
    private static Object valueCmSyncRoot = new Object();

    public static boolean IsCombiningMark(int cp) {
      synchronized(valueCmSyncRoot) {
        if (combmark == null) {
          combmark = ByteData.Decompress(
            IdnaData.CombiningMarks);
        }
        return combmark.GetBoolean(cp);
      }
    }

    private static ByteData stablenfc = null;
    private static ByteData stablenfd = null;
    private static ByteData stablenfkc = null;
    private static ByteData stablenfkd = null;
    private static Object stableSyncRoot = new Object();

    public static boolean IsStableCodePoint(int cp, PeterO.Text.Normalization form) {
      synchronized(stableSyncRoot) {
        if (form == PeterO.Text.Normalization.NFC) {
          if (stablenfc == null) {
            stablenfc = ByteData.Decompress(PeterO.Text.NormalizationData.StableNFC);
          }
          return stablenfc.GetBoolean(cp);
        }
        if (form == PeterO.Text.Normalization.NFD) {
          if (stablenfd == null) {
            stablenfd = ByteData.Decompress(PeterO.Text.NormalizationData.StableNFD);
          }
          return stablenfd.GetBoolean(cp);
        }
        if (form == PeterO.Text.Normalization.NFKC) {
          if (stablenfkc == null) {
            stablenfkc = ByteData.Decompress(PeterO.Text.NormalizationData.StableNFKC);
          }
          return stablenfkc.GetBoolean(cp);
        }
        if (form == PeterO.Text.Normalization.NFKD) {
          if (stablenfkd == null) {
            stablenfkd = ByteData.Decompress(PeterO.Text.NormalizationData.StableNFKD);
          }
          return stablenfkd.GetBoolean(cp);
        }
        return false;
      }
    }

    private static int[] decomps = null;

    public static int GetDecomposition(int cp, boolean compat, int[] buffer, int offset) {
      if (cp < 0x80) {
        // ASCII characters have no decomposition
        buffer[offset++] = cp;
        return offset;
      }
      decomps = PeterO.Text.NormalizationData.DecompMappings;
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
              System.arraycopy(PeterO.Text.NormalizationData.CompatDecompMappings, realIndex, buffer, offset, size);
            } else {
              realIndex = 1 + (decomps[0] << 1) + (data & 0x1fffff);
              System.arraycopy(decomps, realIndex, buffer, offset, size);
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
      synchronized(pairsSyncRoot) {
        if (pairs == null) {
          pairs = PeterO.Text.NormalizationData.ComposedPairs;
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
