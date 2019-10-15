/*
Written by Peter O. in 2014-2016.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Text {
  internal static class UnicodeDatabase {
    private static readonly object ValueSyncRoot = new Object();

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
        lock (ValueSyncRoot) {
          classes = classes ?? ByteData.Decompress (
              NormalizationData.CombiningClasses);
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
      var left = 0;
      int right = (pairs.Length / 3) - 1;
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
      bool compat,
      int[] buffer,
      int offset) {
      if (cp < 0x80) {
        // ASCII characters have no decomposition
        buffer[offset++] = cp;
        return offset;
      }
      int[] decomps = NormalizationData.DecompMappings;
      var left = 0;
      int right = (decomps.Length >> 1) - 1;
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
            throw new InvalidOperationException();
          }
          realIndex = data1 & 0x1fffff;
          Array.Copy(
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
      for (var i = 0; i < decomps.Length; i += 3) {
        if (decomps[i] == cp) {
          buffer[offset++] = decomps[i + 1];
          buffer[offset++] = decomps[i + 2];
          return offset;
        }
      }
      if (cp >= 0x10000) {
        decomps = NormalizationData.LowerCaseMappings32;
        var left = 0;
        int right = (decomps.Length >> 1) - 1;
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
        var left = 0;
        int right = decomps.Length - 1;
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
        lock (ValueSyncRoot) {
          idnaCat = idnaCat ?? ByteData.Decompress(IdnaData.IdnaCategories);
        }
      }
      return ((int)idnaCat.GetByte(cp)) & 0xff;
    }

    public static int GetCasedProperty(int cp) {
      if (cp < 0) {
        return 0;
      }
      if (casedprop == null) {
        lock (ValueSyncRoot) {
          casedprop = casedprop ?? ByteData.Decompress (
              NormalizationData.CaseProperty);
        }
      }
      return ((int)casedprop.GetByte(cp)) & 0xff;
    }

    public static int GetPrecisCategory(int cp) {
      if (cp < 0) {
        return 0;
      }
      if (precisCat == null) {
        lock (ValueSyncRoot) {
          precisCat = precisCat ??
            ByteData.Decompress(IdnaData.PrecisCategories);
        }
      }
      return ((int)precisCat.GetByte(cp)) & 0xff;
    }

    public static bool IsCombiningMark(int cp) {
      if (combmark == null) {
        lock (ValueSyncRoot) {
          combmark = combmark ?? ByteData.Decompress(IdnaData.CombiningMarks);
        }
      }
      return combmark.GetBoolean(cp);
    }

    public static bool IsFullOrHalfWidth(int cp) {
      if (fhwidth == null) {
        lock (ValueSyncRoot) {
          fhwidth = fhwidth ?? ByteData.Decompress(IdnaData.FullHalfWidth);
        }
      }
      return fhwidth.GetBoolean(cp);
    }

    public static bool IsQuickCheckStarter(int cp, Normalization form) {
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
          lock (ValueSyncRoot) {
            qcsnfc = qcsnfc ?? ByteData.Decompress(NormalizationData.QCSNFC);
          }
        }
        bd = qcsnfc;
      }
      if (form == Normalization.NFD) {
        if (qcsnfd == null) {
          lock (ValueSyncRoot) {
            qcsnfd = qcsnfd ?? ByteData.Decompress(NormalizationData.QCSNFD);
          }
        }
        bd = qcsnfd;
      }
      if (form == Normalization.NFKC) {
        if (qcsnfkc == null) {
          lock (ValueSyncRoot) {
            qcsnfkc = qcsnfkc ?? ByteData.Decompress(
  NormalizationData.QCSNFKC);
          }
        }
        bd = qcsnfkc;
      }
      if (form == Normalization.NFKD) {
        if (qcsnfkd == null) {
          lock (ValueSyncRoot) {
            qcsnfkd = qcsnfkd ?? ByteData.Decompress(
  NormalizationData.QCSNFKD);
          }
        }
        bd = qcsnfkd;
      }
      return bd != null && bd.GetBoolean(cp);
    }
  }
}
