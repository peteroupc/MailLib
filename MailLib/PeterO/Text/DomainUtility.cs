/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Text;
using PeterO;

// NOTE: Implements Punycode defined in RFC 3492
namespace PeterO.Text {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Text.DomainUtility"]/*'/>
  internal static class DomainUtility {
    private static int CodePointAt(string str, int index, int endIndex) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index >= endIndex) {
        return -1;
      }
      if (index < 0) {
        return -1;
      }
      int c = str[index];
      if ((c & 0xfc00) == 0xd800 && index + 1 < endIndex &&
          str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
        // Get the Unicode code point for the surrogate pair
        c = 0x10000 + ((c - 0xd800) << 10) + (str[index + 1] - 0xdc00);
      } else if ((c & 0xf800) == 0xd800) {
        // unpaired surrogate
        return 0xfffd;
      }
      return c;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.DomainUtility.PunycodeLength(System.String,System.Int32,System.Int32)"]/*'/>
    public static int PunycodeLength(string str, int index, int endIndex) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index < 0) {
      throw new ArgumentException("index (" + index + ") is less than " +
          "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          str.Length);
      }
      if (endIndex < 0) {
throw new ArgumentException("endIndex (" + endIndex + ") is less than " +
          "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + endIndex +
          ") is more than " + str.Length);
      }
      if (endIndex < index) {
        throw new ArgumentException("endIndex (" + endIndex +
          ") is less than " + index);
      }
      var vnum = 128;
      var delta = 0;
      var bias = 72;
      var v4 = 0;
      int tmpIndex;
      var firstIndex = -1;
      var codePointLength = 0;
      var basicsBeforeFirstNonbasic = 0;
      var allBasics = true;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        if (str[tmpIndex] >= 0x80) {
          allBasics = false;
          break;
        }
        ++tmpIndex;
      }
      if (allBasics) {
        return endIndex - index;
      }
      var outputLength = 4;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        int c = CodePointAt(str, tmpIndex, endIndex);
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
        var min = 0x110000;
        tmpIndex = firstIndex;
        while (tmpIndex < endIndex) {
          int c = CodePointAt(str, tmpIndex, endIndex);
          tmpIndex += (c >= 0x10000) ? 2 : 1;
          if (c >= vnum && c < min) {
            min = c;
          }
        }
        int v3 = min - vnum;
        if (v3 > Int32.MaxValue / (v4 + 1)) {
          return -1;
        }
        v3 *= v4 + 1;
        vnum = min;
        if (v3 > Int32.MaxValue - delta) {
          return -1;
        }
        delta += v3;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Int32.MaxValue - delta) {
          return -1;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = CodePointAt(str, tmpIndex, endIndex);
          tmpIndex += (c >= 0x10000) ? 2 : 1;
          if (c < vnum) {
            if (delta == Int32.MaxValue) {
              return -1;
            }
            ++delta;
          } else if (c == vnum) {
            int v1 = delta;
            var v2 = 36;
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

 private static readonly int[] ValueDigitValues = { -1, -1, -1, -1, -1, -1,
      -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1 };

    internal static string PunycodeDecode(string str, int index, int endIndex) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index < 0) {
      throw new ArgumentException("index (" + index + ") is less than " +
          "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          str.Length);
      }
      if (endIndex < 0) {
throw new ArgumentException("endIndex (" + endIndex + ") is less than " +
          "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + endIndex +
          ") is more than " + str.Length);
      }
      if (endIndex < index) {
        throw new ArgumentException("endIndex (" + endIndex +
          ") is less than " + index);
      }
      if (index == endIndex) {
        return String.Empty;
      }
      int lastHyphen = endIndex - 1;
      while (lastHyphen >= index) {
        if (str[lastHyphen] == '-') {
          break;
        }
        --lastHyphen;
      }
      var i = 0;
      if (lastHyphen >= index) {
        for (i = index; i < lastHyphen; ++i) {
          if (str[i] >= 0x80) {
            return null;  // Non-basic character found
          }
        }
      }
      var builder = new StringBuilder();
      // Append all characters up to the last hyphen
      // (they will be ASCII at this point)
      for (int k = index; k < lastHyphen; ++k) {
        int c = str[k];
        if (c >= 0x41 && c <= 0x5a) {
           // convert to lowercase
          c += 0x20;
        }
        builder.Append((char)c);
      }
      if (lastHyphen >= index) {
        index = lastHyphen + 1;
      }
      i = 0;
      var vnum = 128;
      var bias = 72;
      int stringLength = builder.Length;
      var chararr = new char[2];
      while (index < endIndex) {
        int old = index;
        var w = 1;
        var k = 36;
        while (true) {
          if (index >= endIndex) {
            return null;
          }
          char c = str[index];
          ++index;
          if (c >= 0x80) {
            return null;
          }
          int digit = ValueDigitValues[(int)c];
          if (digit < 0) {
            return null;
          }
          if (digit > Int32.MaxValue / w) {
            return null;
          }
          int temp = digit * w;
          if (i > Int32.MaxValue - temp) {
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
          if (w > Int32.MaxValue / temp) {
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
        if (vnum > Int32.MaxValue - idiv) {
          return null;
        }
        vnum += idiv;
        i %= futureLength;
        if (vnum <= 0xffff) {
          chararr[0] = (char)vnum;
          builder.Insert(i, chararr, 0, 1);
        } else if (vnum <= 0x10ffff) {
          chararr[0] = (char)((((vnum - 0x10000) >> 10) & 0x3ff) + 0xd800);
          chararr[1] = (char)(((vnum - 0x10000) & 0x3ff) + 0xdc00);
          builder.Insert(i, chararr, 0, 2);
        } else {
          return null;
        }
        ++stringLength;
        ++i;
      }
      return builder.ToString();
    }

private const string PunycodeAlphabet =
      "abcdefghijklmnopqrstuvwxyz0123456789";

    internal static string PunycodeEncode(string str) {
      return PunycodeEncodePortion(str, 0, str.Length);
    }

    internal static string PunycodeEncodePortion(
  string str,
  int index,
  int endIndex) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index < 0) {
      throw new ArgumentException("index (" + index + ") is less than " +
          "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          str.Length);
      }
      if (endIndex < 0) {
throw new ArgumentException("endIndex (" + endIndex + ") is less than " +
          "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + endIndex +
          ") is more than " + str.Length);
      }
      if (endIndex < index) {
        throw new ArgumentException("endIndex (" + endIndex +
          ") is less than " + index);
      }
      var vnum = 128;
      var delta = 0;
      var bias = 72;
      var v4 = 0;
      int tmpIndex;
      var firstIndex = -1;
      var codePointLength = 0;
      var basicsBeforeFirstNonbasic = 0;
      var allBasics = true;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        if (str[tmpIndex] >= 0x80) {
          allBasics = false;
          break;
        }
        if (str[tmpIndex] >= 0x41 && str[tmpIndex] <= 0x5a) {
          // Treat as having a non-basic in case of an
          // upper-case ASCII character, since special
          // handling is required here
          allBasics = false;
          break;
        }
        ++tmpIndex;
      }
      if (allBasics) {
        return str.Substring(index, endIndex - index);
      }
      var builder = new StringBuilder();
      builder.Append("xn--");
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        int c = Idna.CodePointAt(str, tmpIndex);
        ++codePointLength;
        if (c >= 0x41 && c <= 0x5a) {
          // This is an uppercase ASCII character,
          // convert to lowercase
          builder.Append((char)(c + 0x20));
          ++v4;
        } else if (c < 0x80) {
          // This is a basic (ASCII) code point
          builder.Append((char)c);
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
        return builder.ToString();
      }
      if (v4 != 0) {
        builder.Append('-');
      }
      while (v4 < codePointLength) {
        var min = 0x110000;
        tmpIndex = firstIndex;
        while (tmpIndex < endIndex) {
          int c = Idna.CodePointAt(str, tmpIndex);
          if (c >= vnum && c < min) {
            min = c;
          }
          if (c >= 0x10000) {
            ++tmpIndex;
          }
          ++tmpIndex;
        }
        int v3 = min - vnum;
        if (v3 > Int32.MaxValue / (v4 + 1)) {
          return null;
        }
        v3 *= v4 + 1;
        vnum = min;
        if (v3 > Int32.MaxValue - delta) {
          return null;
        }
        delta += v3;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Int32.MaxValue - delta) {
          return null;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = Idna.CodePointAt(str, tmpIndex);
          if (c >= 0x10000) {
            ++tmpIndex;
          }
          ++tmpIndex;
          if (c < vnum) {
            if (delta == Int32.MaxValue) {
              return null;
            }
            ++delta;
          } else if (c == vnum) {
            int q = delta;
            var k = 36;
            while (true) {
              int v5;
              v5 = (k <= bias) ? 1 : ((k >= bias + 26) ? 26 : (k - bias));
              if (q < v5) {
                break;
              }
              int digit = v5 + ((q - v5) % (36 - v5));
              builder.Append(PunycodeAlphabet[digit]);
              q -= v5;
              q /= 36 - v5;
              k += 36;
            }
            builder.Append(PunycodeAlphabet[q]);
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
      return builder.ToString();
    }
  }
}
