/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

namespace PeterO.Mail
{
    /// <summary>Utility methods for domain names.</summary>
  internal static class DomainUtility
  {
    public static int EncodedDomainNameLength(string str, int index, int endIndex) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (endIndex < 0) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (endIndex < index) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture));
      }
      int length = endIndex - index;
      // Split into labels
      int lastLabel = 0;
      int retval = 0;
      for (int i = index; i <= endIndex; ++i) {
        if (i == endIndex || str[i] == '.') {
          int labelLength = i - lastLabel;
          if (labelLength != 0) {
            labelLength = PunycodeLength(str, lastLabel, i);
            if (labelLength < 0) {
              // Overflow error occurred
              labelLength = i - lastLabel;
            }
          }
          retval += labelLength;
          if (i == endIndex) {
            // No more labels
            break;
          } else {
            // Add the dot
            ++retval;
          }
          lastLabel = i + 1;
        }
      }
      return retval;
    }

    /// <summary>Gets the Punycode length of a string (Punycode is defined
    /// in RFC 3492).</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='endIndex'>A 32-bit signed integer. (2).</param>
    /// <returns>The Punycode length of the encoded string. If the string
    /// contains non-ASCII characters, returns the Punycode length plus
    /// 4 (the length of the prefix "xn--"). Returns -1 if an overflow error
    /// occurs.</returns>
    public static int PunycodeLength(string str, int index, int endIndex) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (endIndex < 0) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (endIndex < index) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture));
      }
      int n = 128;
      int delta = 0;
      int bias = 72;
      int h = 0;
      int tmpIndex;
      int firstIndex = -1;
      int codePointLength = 0;
      int basicsBeforeFirstNonbasic = 0;
      bool allBasics = true;
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
      int length = 4;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        int c = (int)str[tmpIndex];
        if (c >= 0xd800 && c <= 0xdbff && tmpIndex + 1 < endIndex &&
            str[tmpIndex + 1] >= 0xdc00 && str[tmpIndex + 1] <= 0xdfff) {
          c = 0x10000 + ((c - 0xd800) * 0x400) + (str[tmpIndex + 1] - 0xdc00);
          ++tmpIndex;
        } else if (c >= 0xd800 && c <= 0xdfff) {
          // unpaired surrogate
          c = 0xfffd;
        }
        ++codePointLength;
        if (c < 0x80) {
          // This is a basic (ASCII) code point
          ++length;
          ++h;
        } else if (firstIndex < 0) {
          firstIndex = tmpIndex;
        }
        ++tmpIndex;
      }
      if (h != 0) {
        ++length;
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
          int c = (int)str[tmpIndex];
          if (c >= 0xd800 && c <= 0xdbff && tmpIndex + 1 < endIndex &&
              str[index + 1] >= 0xdc00 && str[tmpIndex + 1] <= 0xdfff) {
            c = 0x10000 + ((c - 0xd800) * 0x400) + (str[tmpIndex + 1] - 0xdc00);
            ++tmpIndex;
          } else if (c >= 0xd800 && c <= 0xdfff) {
            // unpaired surrogate
            c = 0xfffd;
          }
          if (c >= n && c < min) {
            min = c;
          }
          ++tmpIndex;
        }
        int d = min - n;
        if (d > Int32.MaxValue / (h + 1)) {
          return -1;
        }
        d *= h + 1;
        n = min;
        if (d > Int32.MaxValue - delta) {
          return -1;
        }
        delta += d;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Int32.MaxValue - delta) {
          return -1;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = (int)str[tmpIndex];
          if (c >= 0xd800 && c <= 0xdbff && tmpIndex + 1 < endIndex &&
              str[tmpIndex + 1] >= 0xdc00 && str[tmpIndex + 1] <= 0xdfff) {
            c = 0x10000 + ((c - 0xd800) * 0x400) + (str[index + 1] - 0xdc00);
            ++tmpIndex;
          } else if (c >= 0xd800 && c <= 0xdfff) {
            // unpaired surrogate
            c = 0xfffd;
          }
          ++tmpIndex;
          if (c < n) {
            if (delta == Int32.MaxValue) {
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
              int digit = t + ((q - t) %(36 - t));
              ++length;
              q -= t;
              q /= 36 - t;
              k += 36;
            }
            ++length;
            delta = (h == b) ? delta / 700 : delta >> 1;
            delta += delta / (h + 1);
            k = 0;
            while (delta > 455) {
              delta /= 35;
              k += 36;
            }
            bias = k + ((36 * delta) /(delta + 38));
            delta = 0;
            ++h;
          }
        }
        ++n;
        ++delta;
      }
      return length;
    }

    private static string valuePunycodeAlphabet = "abcdefghijklmnopqrstuvwxyz0123456789";

    //
    // Gets the Punycode encoding of a string (Punycode
    // is defined in RFC 3492).
    //
    public static string PunycodeEncode(string str, int index, int endIndex) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (endIndex < 0) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (endIndex < index) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture));
      }
      int n = 128;
      int delta = 0;
      int bias = 72;
      int h = 0;
      int tmpIndex;
      int length = endIndex - index;
      int firstIndex = -1;
      int codePointLength = 0;
      int basicsBeforeFirstNonbasic = 0;
      bool allBasics = true;
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        if (str[tmpIndex] >= 0x80) {
          allBasics = false;
          break;
        }
        ++tmpIndex;
      }
      if (allBasics) {
        return str.Substring(index, endIndex - index);
      }
      StringBuilder builder = new StringBuilder();
      builder.Append("xn--");
      tmpIndex = index;
      while (tmpIndex < endIndex) {
        int c = (int)str[tmpIndex];
        if (c >= 0xd800 && c <= 0xdbff && tmpIndex + 1 < endIndex &&
            str[tmpIndex + 1] >= 0xdc00 && str[tmpIndex + 1] <= 0xdfff) {
          c = 0x10000 + ((c - 0xd800) * 0x400) + (str[tmpIndex + 1] - 0xdc00);
          ++tmpIndex;
        } else if (c >= 0xd800 && c <= 0xdfff) {
          // unpaired surrogate
          c = 0xfffd;
        }
        ++codePointLength;
        if (c < 0x80) {
          // This is a basic (ASCII) code point
          builder.Append((char)c);
          ++h;
        } else if (firstIndex < 0) {
          firstIndex = tmpIndex;
        }
        ++tmpIndex;
      }
      if (h != 0) {
        builder.Append('-');
      }
      int b = h;
      if (firstIndex >= 0) {
        basicsBeforeFirstNonbasic = firstIndex - index;
      } else {
        // No non-basic code points
        return str.Substring(index, endIndex - index);
      }
      while (h < codePointLength) {
        int min = 0x110000;
        tmpIndex = firstIndex;
        while (tmpIndex < endIndex) {
          int c = (int)str[tmpIndex];
          if (c >= 0xd800 && c <= 0xdbff && tmpIndex + 1 < endIndex &&
              str[index + 1] >= 0xdc00 && str[tmpIndex + 1] <= 0xdfff) {
            c = 0x10000 + ((c - 0xd800) * 0x400) + (str[tmpIndex + 1] - 0xdc00);
            ++tmpIndex;
          } else if (c >= 0xd800 && c <= 0xdfff) {
            // unpaired surrogate
            c = 0xfffd;
          }
          if (c >= n && c < min) {
            min = c;
          }
          ++tmpIndex;
        }
        int d = min - n;
        if (d > Int32.MaxValue / (h + 1)) {
          return null;
        }
        d *= h + 1;
        n = min;
        if (d > Int32.MaxValue - delta) {
          return null;
        }
        delta += d;
        tmpIndex = firstIndex;
        if (basicsBeforeFirstNonbasic > Int32.MaxValue - delta) {
          return null;
        }
        delta += basicsBeforeFirstNonbasic;
        while (tmpIndex < endIndex) {
          int c = (int)str[tmpIndex];
          if (c >= 0xd800 && c <= 0xdbff && tmpIndex + 1 < endIndex &&
              str[tmpIndex + 1] >= 0xdc00 && str[tmpIndex + 1] <= 0xdfff) {
            c = 0x10000 + ((c - 0xd800) * 0x400) + (str[index + 1] - 0xdc00);
            ++tmpIndex;
          } else if (c >= 0xd800 && c <= 0xdfff) {
            // unpaired surrogate
            c = 0xfffd;
          }
          ++tmpIndex;
          if (c < n) {
            if (delta == Int32.MaxValue) {
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
              int digit = t + ((q - t) %(36 - t));
              builder.Append(valuePunycodeAlphabet[digit]);
              q -= t;
              q /= 36 - t;
              k += 36;
            }
            builder.Append(valuePunycodeAlphabet[q]);
            delta = (h == b) ? delta / 700 : delta >> 1;
            delta += delta / (h + 1);
            k = 0;
            while (delta > 455) {
              delta /= 35;
              k += 36;
            }
            bias = k + ((36 * delta) /(delta + 38));
            delta = 0;
            ++h;
          }
        }
        ++n;
        ++delta;
      }
      return builder.ToString();
    }
  }
}
