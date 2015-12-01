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

    private static ByteData combmark;

    private static int[] decomps;

    private static ByteData idnaCat;
    private static final Object idnaCatSyncRoot = new Object();
    private static int[] pairs;

    private static int pairsLength;
    private static final Object pairsSyncRoot = new Object();

    private static ByteData qcsnfc;
    private static ByteData qcsnfd;
    private static ByteData qcsnfkc;
    private static ByteData qcsnfkd;
    private static final Object qcsSyncRoot = new Object();
    private static final Object valueCmSyncRoot = new Object();

    public static int GetCombiningClass(int cp) {
      synchronized (classesSyncRoot) {
  classes = (classes == null) ? (ByteData.Decompress(NormalizationData.CombiningClasses)) : classes;
      }
      return ((int)classes.GetByte(cp)) & 0xff;
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

    public static int GetIdnaCategory(int cp) {
      synchronized (idnaCatSyncRoot) {
        idnaCat = (idnaCat == null) ? (ByteData.Decompress(IdnaData.IdnaCategories)) : idnaCat;
      }
      return ((int)idnaCat.GetByte(cp)) & 0xff;
    }

    public static boolean IsCombiningMark(int cp) {
      synchronized (valueCmSyncRoot) {
        combmark = (combmark == null) ? (ByteData.Decompress(IdnaData.CombiningMarks)) : combmark;
        return combmark.GetBoolean(cp);
      }
    }

    public static boolean IsQuickCheckStarter(int cp, Normalization form) {
      // Code points for which QuickCheck = YES and with a combining
      // class of 0
      ByteData bd = null;
      if (form == Normalization.NFC &&
      (cp < NormalizationData.QCSNFCMin || cp >
          NormalizationData.QCSNFCMax)) {
        return true;
      }
      if (form == Normalization.NFD &&
      (cp < NormalizationData.QCSNFDMin || cp >
          NormalizationData.QCSNFDMax)) {
        return true;
      }
      if (form == Normalization.NFKC &&
    (cp < NormalizationData.QCSNFKCMin || cp >
          NormalizationData.QCSNFKCMax)) {
        return true;
      }
      if (form == Normalization.NFKD &&
    (cp < NormalizationData.QCSNFKDMin || cp >
          NormalizationData.QCSNFKDMax)) {
        return true;
      }
      synchronized (qcsSyncRoot) {
        if (form == Normalization.NFC) {
          bd = qcsnfc = (qcsnfc == null) ? (ByteData.Decompress(NormalizationData.QCSNFC)) : qcsnfc;
        }
        if (form == Normalization.NFD) {
          bd = qcsnfd = (qcsnfd == null) ? (ByteData.Decompress(NormalizationData.QCSNFD)) : qcsnfd;
        }
        if (form == Normalization.NFKC) {
      bd = qcsnfkc = (qcsnfkc == null) ? (ByteData.Decompress(NormalizationData.QCSNFKC)) : qcsnfkc;
        }
        if (form == Normalization.NFKD) {
      bd = qcsnfkd = (qcsnfkd == null) ? (ByteData.Decompress(NormalizationData.QCSNFKD)) : qcsnfkd;
        }
      }
      return bd != null && bd.GetBoolean(cp);
    }

    private static void EnsurePairs() {
      synchronized (pairsSyncRoot) {
        if (pairs == null) {
          pairs = NormalizationData.ComposedPairs;
          pairsLength = pairs.length / 3;
        }
      }
    }
  }
