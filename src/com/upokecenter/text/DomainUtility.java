package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Utility methods for domain names.
     */
  final class DomainUtility {
private DomainUtility() {
}
    private static int CodePointAt(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index >= endIndex) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str.charAt(index);
      if ((c & 0xfc00) == 0xd800 && index + 1 < endIndex &&
          str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        c = 0x10000 + ((c - 0xd800) << 10) + (str.charAt(index + 1) - 0xdc00);
        ++index;
      } else if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        return 0xfffd;
      }
      return c;
    }

    /**
     * Gets the PunyCode length of a string (PunyCode is defined in RFC 3492).
     * @param str A string object.
     * @param index A 32-bit signed integer.
     * @param endIndex A 32-bit signed integer. (2).
     * @return The PunyCode length of the encoded string. If the string contains
     * non-ASCII characters, returns the PunyCode length plus 4 (the length
     * of the ACE prefix). If there are only ASCII characters, returns the
     * length of the string. Returns -1 if an overflow error occurs.
     */
    public static int PunycodeLength(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (endIndex < 0) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is less than " + "0");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is less than " + Integer.toString((int)index));
      }
      int n = 128;
      int delta = 0;
      int bias = 72;
      int h = 0;
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
        int c = CodePointAt(str, tmpIndex, endIndex);
        ++codePointLength;
        if (c < 0x80) {
          // This is a basic (ASCII) code point
          ++outputLength;
          ++h;
        } else if (firstIndex < 0) {
          firstIndex = tmpIndex;
        }
        // Increment index after setting firstIndex
        tmpIndex += (c >= 0x10000) ? 2 : 1;
      }
      if (h != 0) {
        ++outputLength;
      }
      int b = h;
      if (firstIndex >= 0) {
        basicsBeforeFirstNonbasic = firstIndex - index;
      } else {
        // No non-basic code points
        return endIndex - index;
      }
      while (h < codePointLength) {
        int min = 0x110000;
        tmpIndex = firstIndex;
        while (tmpIndex < endIndex) {
          int c = CodePointAt(str, tmpIndex, endIndex);
          tmpIndex += (c >= 0x10000) ? 2 : 1;
          if (c >= n && c < min) {
            min = c;
          }
        }
        int d = min - n;
        if (d > Integer.MAX_VALUE / (h + 1)) {
          return -1;
        }
        d *= h + 1;
        n = min;
        if (d > Integer.MAX_VALUE - delta) {
          return -1;
        }
        delta += d;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Integer.MAX_VALUE - delta) {
          return -1;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = CodePointAt(str, tmpIndex, endIndex);
          tmpIndex += (c >= 0x10000) ? 2 : 1;
          if (c < n) {
            if (delta == Integer.MAX_VALUE) {
              return -1;
            }
            ++delta;
          } else if (c == n) {
            int q = delta;
            int k = 36;
            while (true) {
              int t;
              if (k <= bias) {
                t = 1;
              } else if (k >= bias + 26) {
                t = 26;
              } else {
                t = k - bias;
              }
              if (q < t) {
                break;
              }
              ++outputLength;
              q -= t;
              q /= 36 - t;
              k += 36;
            }
            ++outputLength;
            delta = (h == b) ? delta / 700 : delta >> 1;
            delta += delta / (h + 1);
            k = 0;
            while (delta > 455) {
              delta /= 35;
              k += 36;
            }
            bias = k + ((36 * delta) / (delta + 38));
            delta = 0;
            ++h;
          }
        }
        ++n;
        ++delta;
      }
      return outputLength;
    }

    private static int[] valueDigitValues = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1 };

    /**
     * Decodes a PunyCode-encoded string.
     * @param str A string to decode. Note that this doesn&apos;t include
     * a prefix such as.<code>xn--</code> .
     * @param index A 32-bit signed integer.
     * @param endIndex A 32-bit signed integer. (2).
     * @return A string object.
     */
    static String PunycodeDecode(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (endIndex < 0) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is less than " + "0");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is less than " + Integer.toString((int)index));
      }
      if (index == endIndex) {
        return "";
      }
      int lastHyphen = endIndex;
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
            return null;  // Non-basic character found
          }
        }
      }
      StringBuilder builder = new StringBuilder();
      // Append all characters up to the last hyphen
      // (they will be ASCII at this point)
      for (int k = index; k < endIndex; ++k) {
        int c = str.charAt(i);
        if (c >= 0x41 && c <= 0x5a) {
          c+=0x20;  // convert to lowercase
        }
        builder.append((char)c);
      }
      if (lastHyphen >= index) {
        index = lastHyphen + 1;
      }
      i = 0;
      int n = 128;
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
          k += 36;
          char c = str.charAt(index);
          if (c >= 0x80) {
            return null;
          }
          int digit = valueDigitValues[(int)c];
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
          i -= temp;
          int t = k - bias;
          if (k <= bias) {
            t = 1;
          } else if (k >= bias + 26) {
            t = 26;
          }
          if (digit < t) {
            break;
          }
          temp = 36 - t;
          if (w > Integer.MAX_VALUE / temp) {
            return null;
          }
          w *= temp;
        }
        int futureLength = stringLength + 1;
        int delta = (old == 0) ? (old - i) / 700 : (old - i) >> 1;
        delta += delta / futureLength;
        k = 0;
        while (delta > 455) {
          delta /= 35;
          k += 36;
        }
        bias = k + ((36 * delta) / (delta + 38));
        int idiv = i / futureLength;
        if (n > Integer.MAX_VALUE - idiv) {
          return null;
        }
        n += idiv;
        i %= futureLength;
        if (n <= 0xffff) {
          chararr[0] = (char)n;
          builder.insert(i, chararr, 0, 1);
        } else if (n <= 0x10ffff) {
          chararr[0] = (char)((((n - 0x10000) >> 10) & 0x3ff) + 0xd800);
          chararr[1] = (char)(((n - 0x10000) & 0x3ff) + 0xdc00);
          builder.insert(i, chararr, 0, 2);
        } else {
          return null;
        }
        ++futureLength;
        ++i;
      }
      return builder.toString();
    }

    private static String valuePunycodeAlphabet = "abcdefghijklmnopqrstuvwxyz0123456789";

    static String PunycodeEncode(String str) {
      return PunycodeEncodePortion(str, 0, str.length());
    }

    static String PunycodeEncodePortion(String str, int index, int endIndex) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + Integer.toString((int)index) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (endIndex < 0) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is less than " + "0");
      }
      if (endIndex > str.length()) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (endIndex < index) {
        throw new IllegalArgumentException("endIndex (" + Integer.toString((int)endIndex) + ") is less than " + Integer.toString((int)index));
      }
      int n = 128;
      int delta = 0;
      int bias = 72;
      int h = 0;
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
        } else if (str.charAt(tmpIndex) >= 0x41 && str.charAt(tmpIndex)<= 0x5a) {
          // Treat as having a non-basic in case of an
          // upper-case ASCII character, since special
          // handling is required here
          allBasics = false;
          break;
        }
        ++tmpIndex;
      }
      if (allBasics) {
        return str.substring(index,(index)+(endIndex - index));
      }
      StringBuilder builder = new StringBuilder();
      builder.append("xn--");
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        int c = Idna.CodePointAt(str, tmpIndex);
        ++codePointLength;
        if (c >= 0x41 && c <= 0x5a) {
          // This is an uppercase ASCII character,
          // convert to lowercase
          builder.append((char)(c + 0x20));
          ++h;
        } else if (c < 0x80) {
          // This is a basic (ASCII) code point
          builder.append((char)c);
          ++h;
        } else if (firstIndex < 0) {
          firstIndex = tmpIndex;
        }
        if (c >= 0x10000) {
          ++tmpIndex;
        }
        ++tmpIndex;
      }
      int b = h;
      if (firstIndex >= 0) {
        basicsBeforeFirstNonbasic = firstIndex - index;
      } else {
        // No non-basic code points
        // (NOTE: Not encoded with "-" at end)
        return builder.toString();
      }
      if (h != 0) {
        builder.append('-');
      }
      while (h < codePointLength) {
        int min = 0x110000;
        tmpIndex = firstIndex;
        while (tmpIndex < endIndex) {
          int c = Idna.CodePointAt(str, tmpIndex);
          if (c >= n && c < min) {
            min = c;
          }
          if (c >= 0x10000) {
            ++tmpIndex;
          }
          ++tmpIndex;
        }
        int d = min - n;
        if (d > Integer.MAX_VALUE / (h + 1)) {
          return null;
        }
        d *= h + 1;
        n = min;
        if (d > Integer.MAX_VALUE - delta) {
          return null;
        }
        delta += d;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Integer.MAX_VALUE - delta) {
          return null;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = Idna.CodePointAt(str, tmpIndex);
          if (c >= 0x10000) {
            ++tmpIndex;
          }
          ++tmpIndex;
          if (c < n) {
            if (delta == Integer.MAX_VALUE) {
              return null;
            }
            ++delta;
          } else if (c == n) {
            int q = delta;
            int k = 36;
            while (true) {
              int t;
              if (k <= bias) {
                t = 1;
              } else if (k >= bias + 26) {
                t = 26;
              } else {
                t = k - bias;
              }
              if (q < t) {
                break;
              }
              int digit = t + ((q - t) % (36 - t));
              builder.append(valuePunycodeAlphabet.charAt(digit));
              q -= t;
              q /= 36 - t;
              k += 36;
            }
            builder.append(valuePunycodeAlphabet.charAt(q));
            delta = (h == b) ? delta / 700 : delta >> 1;
            delta += delta / (h + 1);
            k = 0;
            while (delta > 455) {
              delta /= 35;
              k += 36;
            }
            bias = k + ((36 * delta) / (delta + 38));
            delta = 0;
            ++h;
          }
        }
        ++n;
        ++delta;
      }
      return builder.toString();
    }
  }
