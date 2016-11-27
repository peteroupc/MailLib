package com.upokecenter.text;
/*
Written by Peter O. in 2014-2016.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

  final class UnicodeDatabase {
private UnicodeDatabase() {
}
    private static final Object ValueClassesSyncRoot = new Object();
    private static final Object ValueIdnaCatSyncRoot = new Object();
    private static final Object ValuePairsSyncRoot = new Object();
    private static final Object ValueQcsSyncRoot = new Object();
    private static final Object ValueCmSyncRoot = new Object();
    private static ByteData classes;

    private static ByteData combmark;

    private static int[] decomps;

    private static ByteData idnaCat;
    private static int[] pairs;

    private static int pairsLength;

    private static ByteData qcsnfc;
    private static ByteData qcsnfd;
    private static ByteData qcsnfkc;
    private static ByteData qcsnfkd;

    public static int GetCombiningClass(int cp) {
      if (cp<0x300 || cp >= 0xe0000) {
 return 0;
}
      synchronized (ValueClassesSyncRoot) {
  classes = (classes == null) ? (ByteData.Decompress(NormalizationData.CombiningClasses)) : classes;
      }
      return ((int)classes.GetByte(cp)) & 0xff;
    }

    public static int GetComposedPair(int first, int second) {
      if (((first | second) >> 17) != 0) {
        return -1;
      }
      if (first < 0x80 && second < 0x80) {
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
      int right = (decomps.length >> 1) - 1;
      while (left <= right) {
        int index = (left + right) >> 1;
        int realIndex = (index << 1);
        int dri = decomps [realIndex];
        int dricp = dri & 0x1fffff;
        if (dricp == cp) {
          int data = dri;
          int data1 = decomps[realIndex + 1];
          if ((data & (1 << 23)) > 0 && !compat) {
            buffer[offset++] = cp;
            return offset;
          }
          if ((data & (1 << 22)) > 0) {
            // Singleton
            buffer[offset++] = data1;
            return offset;
          }
          if ((data & (1 << 24)) > 0) {
            // Pair of two BMP code points
           buffer[offset++] = data1 & 0xffff;
           buffer[offset++] = (data1 >> 16) & 0xffff;
           return offset;
          }
          // Complex case
          int size = data1 >> 24;
          if (size <= 0) {
 throw new IllegalStateException();
}
          realIndex = data1 & 0x1fffff;
          System.arraycopy(
                NormalizationData.ComplexDecompMappings,
                realIndex,
                buffer,
                offset,
                size);
          return offset + size;
        }
        if (dricp < cp) {
          left = index + 1;
        } else {
          right = index - 1;
        }
      }
      buffer[offset++] = cp;
      return offset;
    }

    public static int GetIdnaCategory(int cp) {
      synchronized (ValueIdnaCatSyncRoot) {
        idnaCat = (idnaCat == null) ? (ByteData.Decompress(IdnaData.IdnaCategories)) : idnaCat;
      }
      return ((int)idnaCat.GetByte(cp)) & 0xff;
    }

    public static boolean IsCombiningMark(int cp) {
      synchronized (ValueCmSyncRoot) {
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
      synchronized (ValueQcsSyncRoot) {
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
      synchronized (ValuePairsSyncRoot) {
        if (pairs == null) {
          pairs = NormalizationData.ComposedPairs;
          pairsLength = pairs.length / 3;
        }
      }
    }
  }
