package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import com.upokecenter.util.*;

// NOTE: Implements Punycode defined in RFC 3492

  final class DomainUtility {
private DomainUtility() {
}
    private static final String PunycodeAlphabet =
      "abcdefghijklmnopqrstuvwxyz0123456789";
    static int ALabelLength(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
            "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + index + ") is more than " +
          str.length());
      }
      if (endIndex < 0) {
      throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is less than " + "0");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is more than " + str.length());
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is less than " + index);
      }
      int vnum = 128;
      int delta = 0;
      int bias = 72;
      int v4 = 0;
      int tmpIndex;
      int firstIndex = -1;
      int codePointLength = 0;
      int basicsBeforeFirstNonbasic = 0;
      boolean allBasics = true;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        if (str.charAt(tmpIndex) >= 0x80) {
          allBasics = false;
          break;
        }
        ++tmpIndex;
      }
      if (allBasics) {
        return endIndex - index;
      }
      int outputLength = 4;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        int c = str.charAt(tmpIndex);
        if ((c & 0xfc00) == 0xd800 && tmpIndex + 1 < endIndex &&
            (str.charAt(tmpIndex + 1) & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (str.charAt(tmpIndex + 1) & 0x3ff);
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate
          return -1;
        }
        ++codePointLength;
        if (c < 0x80) {
          // This is a basic (ASCII) code point
          ++outputLength;
          ++v4;
        } else if (firstIndex < 0) {
          firstIndex = tmpIndex;
        }
        // Increment index after setting firstIndex
        tmpIndex += (c >= 0x10000) ? 2 : 1;
      }
      if (v4 != 0) {
        ++outputLength;
      }
      int b = v4;
      if (firstIndex >= 0) {
        basicsBeforeFirstNonbasic = firstIndex - index;
      } else {
        // No non-basic code points
        return endIndex - index;
      }
      while (v4 < codePointLength) {
        int min = 0x110000;
        tmpIndex = firstIndex;
        while (tmpIndex < endIndex) {
          int c = str.charAt(tmpIndex);
          if ((c & 0xfc00) == 0xd800 && tmpIndex + 1 < endIndex &&
              (str.charAt(tmpIndex + 1) & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (str.charAt(tmpIndex + 1) & 0x3ff);
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate
            return -1;
          }
          tmpIndex += (c >= 0x10000) ? 2 : 1;
          if (c >= vnum && c < min) {
            min = c;
          }
        }
        int v3 = min - vnum;
        if (v3 > Integer.MAX_VALUE / (v4 + 1)) {
          return -1;
        }
        v3 *= v4 + 1;
        vnum = min;
        if (v3 > Integer.MAX_VALUE - delta) {
          return -1;
        }
        delta += v3;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Integer.MAX_VALUE - delta) {
          return -1;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = str.charAt(tmpIndex);
          if ((c & 0xfc00) == 0xd800 && tmpIndex + 1 < endIndex &&
              (str.charAt(tmpIndex + 1) & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (str.charAt(tmpIndex + 1) & 0x3ff);
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate
            return -1;
          }
          tmpIndex += (c >= 0x10000) ? 2 : 1;
          if (c < vnum) {
            if (delta == Integer.MAX_VALUE) {
              return -1;
            }
            ++delta;
          } else if (c == vnum) {
            int v1 = delta;
            int v2 = 36;
            while (true) {
              int v5;
              v5 = (v2 <= bias) ? 1 : ((v2 >= bias + 26) ? 26 : (v2 - bias));
              if (v1 < v5) {
                break;
              }
              ++outputLength;
              v1 -= v5;
              v1 /= 36 - v5;
              v2 += 36;
            }
            ++outputLength;
            delta = (v4 == b) ? delta / 700 : delta >> 1;
            delta += delta / (v4 + 1);
            v2 = 0;
            while (delta > 455) {
              delta /= 35;
              v2 += 36;
            }
            bias = v2 + ((36 * delta) / (delta + 38));
            delta = 0;
            ++v4;
          }
        }
        ++vnum;
        ++delta;
      }
      return outputLength;
    }
    private static final int[] ValueDigitValues = {
      -1, -1, -1, -1, -1, -1,
      -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
    };

    static String PunycodeDecode(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
            "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + index + ") is more than " +
          str.length());
      }
      if (endIndex < 0) {
      throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is less than " + "0");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is more than " + str.length());
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is less than " + index);
      }
      if (index == endIndex) {
        return "";
      }
      int lastHyphen = endIndex - 1;
      while (lastHyphen >= index) {
        if (str.charAt(lastHyphen) == '-') {
          break;
        }
        --lastHyphen;
      }
      int i = 0;
      if (lastHyphen >= index) {
        for (i = index; i < lastHyphen; ++i) {
          if (str.charAt(i) >= 0x80) {
            return null; // Non-basic character found
          }
        }
      }
      StringBuilder builder = new StringBuilder();
      // Append all characters up to the last hyphen
      // (they will be ASCII at this point)
      for (int k = index; k < lastHyphen; ++k) {
        int c = str.charAt(k);
        if (c >= 0x41 && c <= 0x5a) {
          // convert to lowercase
          c += 0x20;
        }
        builder.append((char)c);
      }
      if (lastHyphen >= index) {
        index = lastHyphen + 1;
      }
      i = 0;
      int vnum = 128;
      int bias = 72;
      int stringLength = builder.length();
      char[] chararr = new char[2];
      while (index < endIndex) {
        int old = index;
        int w = 1;
        int k = 36;
        while (true) {
          if (index >= endIndex) {
            return null;
          }
          char c = str.charAt(index);
          ++index;
          if (c >= 0x80) {
            return null;
          }
          int digit = ValueDigitValues[(int)c];
          if (digit < 0) {
            return null;
          }
          if (digit > Integer.MAX_VALUE / w) {
            return null;
          }
          int temp = digit * w;
          if (i > Integer.MAX_VALUE - temp) {
            return null;
          }
          i += temp;
          int v5 = k - bias;
          if (k <= bias) {
            v5 = 1;
          } else if (k >= bias + 26) {
            v5 = 26;
          }
          if (digit < v5) {
            break;
          }
          temp = 36 - v5;
          if (w > Integer.MAX_VALUE / temp) {
            return null;
          }
          w *= temp;
          k += 36;
        }
        int futureLength = stringLength + 1;
        int delta = (old == 0) ? (i - old) / 700 : (i - old) >> 1;
        delta += delta / futureLength;
        k = 0;
        while (delta > 455) {
          delta /= 35;
          k += 36;
        }
        bias = k + ((36 * delta) / (delta + 38));
        int idiv;
        idiv = i / futureLength;
        if (vnum > Integer.MAX_VALUE - idiv) {
          return null;
        }
        vnum += idiv;
        i %= futureLength;
        if (vnum <= 0xffff) {
          chararr[0] = (char)vnum;
          builder.insert(i, chararr, 0, 1);
        } else if (vnum <= 0x10ffff) {
          chararr[0] = (char)((((vnum - 0x10000) >> 10) & 0x3ff) | 0xd800);
          chararr[1] = (char)(((vnum - 0x10000) & 0x3ff) | 0xdc00);
          builder.insert(i, chararr, 0, 2);
        } else {
          return null;
        }
        ++stringLength;
        ++i;
      }
      return builder.toString();
    }

    static String ALabelEncode(String str) {
      return ALabelEncodePortion(str, 0, str.length());
    }

    static String ALabelEncodePortion(
      String str,
      int index,
      int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
            "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + index + ") is more than " +
          str.length());
      }
      if (endIndex < 0) {
      throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is less than " + "0");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is more than " + str.length());
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("endIndex (" + endIndex +
          ") is less than " + index);
      }
      int vnum = 128;
      int delta = 0;
      int bias = 72;
      int v4 = 0;
      int tmpIndex;
      int firstIndex = -1;
      int codePointLength = 0;
      int basicsBeforeFirstNonbasic = 0;
      boolean allBasics = true;
      boolean hasUpperCase = false;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        if (str.charAt(tmpIndex) >= 0x80) {
          allBasics = false;
          break;
        }
        if (str.charAt(tmpIndex) >= 0x41 && str.charAt(tmpIndex) <= 0x5a) {
          // Upper-case basic character
          hasUpperCase = true;
          break;
        }
        ++tmpIndex;
      }
      if (allBasics) {
        String rv = str.substring(index, (index)+(endIndex - index));
        // NOTE: Upper-case labels not converted to lower-case here
        return rv;
      }
      StringBuilder builder = new StringBuilder();
      builder.append("xn--");
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        int c = str.charAt(tmpIndex);
        if ((c & 0xfc00) == 0xd800 && tmpIndex + 1 < endIndex &&
            (str.charAt(tmpIndex + 1) & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (str.charAt(tmpIndex + 1) & 0x3ff);
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate
          return null;
        }
        ++codePointLength;
        if (c >= 0x41 && c <= 0x5a) {
          // This is an uppercase ASCII character,
          // convert to lowercase
          builder.append((char)(c + 0x20));
          ++v4;
        } else if (c < 0x80) {
          // This is a basic (ASCII) code point
          builder.append((char)c);
          ++v4;
        } else if (firstIndex < 0) {
          firstIndex = tmpIndex;
        }
        if (c >= 0x10000) {
          ++tmpIndex;
        }
        ++tmpIndex;
      }
      int b = v4;
      if (firstIndex >= 0) {
        basicsBeforeFirstNonbasic = firstIndex - index;
      } else {
        // No non-basic code points
        // (NOTE: Not encoded with "-" at end)
        return builder.toString();
      }
      if (v4 != 0) {
        builder.append('-');
      }
      while (v4 < codePointLength) {
        int min = 0x110000;
        tmpIndex = firstIndex;
        while (tmpIndex < endIndex) {
          int c = str.charAt(tmpIndex);
          if ((c & 0xfc00) == 0xd800 && tmpIndex + 1 < endIndex &&
              (str.charAt(tmpIndex + 1) & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (str.charAt(tmpIndex + 1) & 0x3ff);
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate
            return null;
          }
          if (c >= vnum && c < min) {
            min = c;
          }
          if (c >= 0x10000) {
            ++tmpIndex;
          }
          ++tmpIndex;
        }
        int v3 = min - vnum;
        if (v3 > Integer.MAX_VALUE / (v4 + 1)) {
          return null;
        }
        v3 *= v4 + 1;
        vnum = min;
        if (v3 > Integer.MAX_VALUE - delta) {
          return null;
        }
        delta += v3;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Integer.MAX_VALUE - delta) {
          return null;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = str.charAt(tmpIndex);
          if ((c & 0xfc00) == 0xd800 && tmpIndex + 1 < endIndex &&
              (str.charAt(tmpIndex + 1) & 0xfc00) == 0xdc00) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c & 0x3ff) << 10) + (str.charAt(tmpIndex + 1) & 0x3ff);
          } else if ((c & 0xf800) == 0xd800) {
            // unpaired surrogate
            return null;
          }
          if (c >= 0x10000) {
            ++tmpIndex;
          }
          ++tmpIndex;
          if (c < vnum) {
            if (delta == Integer.MAX_VALUE) {
              return null;
            }
            ++delta;
          } else if (c == vnum) {
            int q = delta;
            int k = 36;
            while (true) {
              int v5;
              v5 = (k <= bias) ? 1 : ((k >= bias + 26) ? 26 : (k - bias));
              if (q < v5) {
                break;
              }
              int digit = v5 + ((q - v5) % (36 - v5));
              builder.append(PunycodeAlphabet.charAt(digit));
              q -= v5;
              q /= 36 - v5;
              k += 36;
            }
            builder.append(PunycodeAlphabet.charAt(q));
            delta = (v4 == b) ? delta / 700 : delta >> 1;
            delta += delta / (v4 + 1);
            k = 0;
            while (delta > 455) {
              delta /= 35;
              k += 36;
            }
            bias = k + ((36 * delta) / (delta + 38));
            delta = 0;
            ++v4;
          }
        }
        ++vnum;
        ++delta;
      }
      return builder.toString();
    }
  }
