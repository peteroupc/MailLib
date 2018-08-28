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
    private static final Object ValueSyncRoot = new Object();

    private static volatile ByteData classes;

    private static volatile ByteData combmark;
    private static volatile ByteData fhwidth;
    private static volatile ByteData casedprop;

    private static volatile ByteData idnaCat;
    private static volatile ByteData precisCat;
    private static volatile ByteData qcsnfc;
    private static volatile ByteData qcsnfd;
    private static volatile ByteData qcsnfkc;
    private static volatile ByteData qcsnfkd;

    public static int GetCombiningClass(int cp) {
      if (cp < 0x300 || cp >= 0xe0000) {
 return 0;
}
  if (classes == null) {
synchronized (ValueSyncRoot) {
classes = (classes == null) ? (ByteData.Decompress(NormalizationData.CombiningClasses)) : classes;
}
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
      int[] pairs = NormalizationData.ComposedPairs;
      int left = 0;
      int right = (pairs.length / 3) - 1;
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
      int[] decomps = NormalizationData.DecompMappings;
      int left = 0;
      int right = (decomps.length >> 1) - 1;
      while (left <= right) {
        int index = (left + right) >> 1;
        int realIndex = index << 1;
        int dri = decomps[realIndex];
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

    public static int GetLowerCaseMapping(
  int cp,
  int[] buffer,
  int offset) {
      if (cp < 0x80) {
        buffer[offset++] = (cp >= 0x41 && cp <= 0x5a) ? cp + 32 : cp;
        return offset;
      }
      int[] decomps = NormalizationData.LowerCaseMappings2;
      for (int i = 0; i < decomps.length; i += 3) {
        if (decomps[i] == cp) {
          buffer[offset++] = decomps[i + 1];
          buffer[offset++] = decomps[i + 2];
          return offset;
        }
      }
      if (cp >= 0x10000) {
      decomps = NormalizationData.LowerCaseMappings32;
      int left = 0;
      int right = (decomps.length >> 1) - 1;
      while (left <= right) {
        int index = (left + right) >> 1;
        int realIndex = index << 1;
        int dri = decomps[realIndex];
        int dricp = dri;
        if (dricp == cp) {
          buffer[offset++] = decomps[realIndex + 1];
          return offset;
        }
        if (dricp < cp) {
          left = index + 1;
        } else {
          right = index - 1;
        }
      }
      buffer[offset++] = cp;
      return offset;
      } else {
      decomps = NormalizationData.LowerCaseMappings;
      int left = 0;
      int right = decomps.length - 1;
      while (left <= right) {
        int index = (left + right) >> 1;
        int realIndex = index;
        int dri = decomps[realIndex];
        int dricp = (dri >> 16) & 0xffff;
        if (dricp == cp) {
          buffer[offset++] = dri & 0xffff;
          return offset;
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
    }

    public static int GetIdnaCategory(int cp) {
       if (cp < 0) {
 return 0;
}
        if (idnaCat == null) {
synchronized (ValueSyncRoot) {
idnaCat = (idnaCat == null) ? (ByteData.Decompress(IdnaData.IdnaCategories)) : idnaCat;
}
}
      return ((int)idnaCat.GetByte(cp)) & 0xff;
    }

    public static int GetCasedProperty(int cp) {
       if (cp < 0) {
 return 0;
}
        if (casedprop == null) {
synchronized (ValueSyncRoot) {
  casedprop = (casedprop == null) ? (ByteData.Decompress(NormalizationData.CaseProperty)) : casedprop;
}
}
      return ((int)casedprop.GetByte(cp)) & 0xff;
    }

    public static int GetPrecisCategory(int cp) {
       if (cp < 0) {
 return 0;
}
      if (precisCat == null) {
synchronized (ValueSyncRoot) {
precisCat = (precisCat == null) ? (ByteData.Decompress(IdnaData.PrecisCategories)) : precisCat;
}
}
      return ((int)precisCat.GetByte(cp)) & 0xff;
    }

    public static boolean IsCombiningMark(int cp) {
        if (combmark == null) {
synchronized (ValueSyncRoot) {
combmark = (combmark == null) ? (ByteData.Decompress(IdnaData.CombiningMarks)) : combmark;
}
}
        return combmark.GetBoolean(cp);
    }

    public static boolean IsFullOrHalfWidth(int cp) {
        if (fhwidth == null) {
synchronized (ValueSyncRoot) {
fhwidth = (fhwidth == null) ? (ByteData.Decompress(IdnaData.FullHalfWidth)) : fhwidth;
}
}
        return fhwidth.GetBoolean(cp);
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
        if (form == Normalization.NFC) {
          if (qcsnfc == null) {
synchronized (ValueSyncRoot) {
qcsnfc = (qcsnfc == null) ? (ByteData.Decompress(NormalizationData.QCSNFC)) : qcsnfc;
}
}
bd = qcsnfc;
        }
        if (form == Normalization.NFD) {
          if (qcsnfd == null) {
synchronized (ValueSyncRoot) {
qcsnfd = (qcsnfd == null) ? (ByteData.Decompress(NormalizationData.QCSNFD)) : qcsnfd;
}
}
bd = qcsnfd;
        }
        if (form == Normalization.NFKC) {
      if (qcsnfkc == null) {
synchronized (ValueSyncRoot) {
qcsnfkc = (qcsnfkc == null) ? (ByteData.Decompress(NormalizationData.QCSNFKC)) : qcsnfkc;
}
}
bd = qcsnfkc;
        }
        if (form == Normalization.NFKD) {
      if (qcsnfkd == null) {
synchronized (ValueSyncRoot) {
qcsnfkd = (qcsnfkd == null) ? (ByteData.Decompress(NormalizationData.QCSNFKD)) : qcsnfkd;
}
}
bd = qcsnfkd;
        }
      return bd != null && bd.GetBoolean(cp);
    }
  }
