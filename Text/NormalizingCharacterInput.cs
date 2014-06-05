/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Text {
    /// <summary><para>Implements the Unicode normalization algorithm
    /// and contains methods and functionality to test and convert Unicode
    /// strings for Unicode normalization. This is similar to the Normalizer
    /// class, except it implements the ICharacterInput interface.</para>
    /// <para>NOTICE: While this class's source code is in the public domain,
    /// the class uses an internal class, called NormalizationData, that
    /// includes data derived from the Unicode Character Database. See the
    /// documentation for the Normalizer class for the permission notice
    /// for the Unicode Character Database.</para>
    /// </summary>
  public sealed class NormalizingCharacterInput : ICharacterInput
  {
    /// <summary>Not documented yet.</summary>
    /// <returns>An IList(int) object.</returns>
    /// <param name='str'>A string object.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null.</exception>
    public static IList<int> GetChars(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return GetChars(new StringCharacterInput(str), form);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IList(int) object.</returns>
    /// <param name='str'>An ICharacterInput object.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null.</exception>
    public static IList<int> GetChars(ICharacterInput str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      NormalizingCharacterInput norm = new NormalizingCharacterInput(str, form);
      int[] buffer = new int[64];
      IList<int> ret = new List<int>(24);
      int count = 0;
      while ((count = norm.Read(buffer, 0, buffer.Length)) > 0) {
        for (int i = 0; i < count; ++i) {
          ret.Add(buffer[i]);
        }
      }
      return ret;
    }

    private int lastStableIndex;
    private int endIndex;
    private int[] buffer;
    private bool compatMode;
    private Normalization form;
    private int processedIndex;
    private int flushIndex;
    private ICharacterInput iterator;
    private IList<int> characterList;
    private int characterListPos;

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class using Normalization Form C.</summary>
    /// <param name='characterList'>An IList object.</param>
    public NormalizingCharacterInput(IList<int> characterList) : this(characterList, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class using Normalization Form C.</summary>
    /// <param name='str'>A string object.</param>
    public NormalizingCharacterInput(string str) : this(str, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class using Normalization Form C.</summary>
    /// <param name='input'>An ICharacterInput object.</param>
    public NormalizingCharacterInput(ICharacterInput input) : this(input, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class using the given normalization form.</summary>
    /// <param name='characterList'>An IList object.</param>
    /// <param name='form'>A Normalization object.</param>
    public NormalizingCharacterInput(IList<int> characterList, Normalization form) {
      if (characterList == null) {
        throw new ArgumentNullException("characterList");
      }
      this.lastStableIndex = -1;
      this.characterList = characterList;
      this.form = form;
      this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='length'>A 32-bit signed integer. (2).</param>
    /// <param name='form'>A Normalization object.</param>
    public NormalizingCharacterInput(string str, int index, int length, Normalization form) :
      this(new StringCharacterInput(str, index, length), form) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='form'>A Normalization object.</param>
    public NormalizingCharacterInput(string str, Normalization form) :
      this(new StringCharacterInput(str), form) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class.</summary>
    /// <param name='stream'>An ICharacterInput object.</param>
    /// <param name='form'>A Normalization object.</param>
    public NormalizingCharacterInput(ICharacterInput stream, Normalization form) {
      if (stream == null) {
        throw new ArgumentException("stream");
      }
      this.lastStableIndex = -1;
      this.iterator = stream;
      this.form = form;
      this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    /// <param name='chars'>An ICharacterInput object.</param>
    /// <param name='form'>A Normalization object.</param>
    public static bool IsNormalized(ICharacterInput chars, Normalization form) {
      if (chars == null) {
        return false;
      }
      IList<int> list = new List<int>();
      int ch = 0;
      while ((ch = chars.ReadChar()) >= 0) {
        if ((ch & 0xf800) == 0xd800) {
          return false;
        }
        list.Add(ch);
      }
      return IsNormalized(list, form);
    }

    private static bool NormalizeAndCheck(
      IList<int> charList,
      int start,
      int length,
      Normalization form) {
      int i = 0;
      foreach (int ch in NormalizingCharacterInput.GetChars(
        new PartialListCharacterInput(charList, start, length),
        form)) {
        if (i >= length) {
          return false;
        }
        if (ch != charList[start + i]) {
          return false;
        }
        ++i;
      }
      return true;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    /// <param name='str'>A string object.</param>
    /// <param name='form'>A Normalization object.</param>
    public static bool IsNormalized(string str, Normalization form) {
      return Normalizer.IsNormalized(str, form);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    /// <param name='charList'>An IList object.</param>
    /// <param name='form'>A Normalization object.</param>
    public static bool IsNormalized(IList<int> charList, Normalization form) {
      int nonStableStart = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      if (charList == null) {
        throw new ArgumentException("chars");
      }
      for (int i = 0; i < charList.Count; ++i) {
        int c = charList[i];
        if (c < 0 || c > 0x10ffff || ((c & 0x1ff800) == 0xd800)) {
          return false;
        }
        bool isStable = false;
        if ((c & mask) == c && (i + 1 == charList.Count || (charList[i + 1] & mask) == charList[i + 1])) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC) or the end of string.
          // Treat the first character as stable
          // in this situation.
          isStable = true;
        } else {
          isStable = Normalizer.IsStableCodePoint(c, form);
        }
        if (nonStableStart < 0 && !isStable) {
          // First non-stable code point in a row
          nonStableStart = i;
        } else if (nonStableStart >= 0 && isStable) {
          // We have at least one non-stable code point,
          // normalize these code points.
          if (!NormalizeAndCheck(charList, nonStableStart, i - nonStableStart, form)) {
            return false;
          }
          nonStableStart = -1;
        }
      }
      if (nonStableStart >= 0) {
        if (!NormalizeAndCheck(charList, nonStableStart, charList.Count - nonStableStart, form)) {
          return false;
        }
      }
      return true;
    }

    private int[] readbuffer = new int[1];

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadChar() {
      int r = this.Read(this.readbuffer, 0, 1);
      return r == 1 ? this.readbuffer[0] : -1;
    }

    private bool endOfString = false;
    private int lastChar = -1;
    private bool ungetting = false;

    private void Unget() {
      this.ungetting = true;
    }

    private int GetNextChar() {
      int ch;
      if (this.ungetting) {
        ch = this.lastChar;
        this.ungetting = false;
        return ch;
      } else if (this.iterator == null) {
        ch = (this.characterListPos >= this.characterList.Count) ? -1 : this.characterList[this.characterListPos++];
      } else {
        ch = this.iterator.ReadChar();
      }
      if (ch < 0) {
        this.endOfString = true;
      } else if (ch > 0x10ffff || ((ch & 0x1ff800) == 0xd800)) {
        throw new ArgumentException("Invalid character: " + ch);
      }
      this.lastChar = ch;
      return ch;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='chars'>An array of 32-bit unsigned integers.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='length'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='chars'/> or "this.buffer" is null.</exception>
    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new ArgumentNullException("chars");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((int)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > chars.Length) {
        throw new ArgumentException("index (" + Convert.ToString((int)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)chars.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > chars.Length) {
        throw new ArgumentException("length (" + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)chars.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (chars.Length - index < length) {
        throw new ArgumentException("chars's length minus " + index + " (" + Convert.ToString((int)(chars.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((int)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length == 0) {
        return 0;
      }
      int total = 0;
      int count = 0;
      if (this.processedIndex == this.flushIndex && this.flushIndex == 0) {
        while (total < length) {
          int c = this.GetNextChar();
          if (c < 0) {
            return (total == 0) ? -1 : total;
          } else if (Normalizer.IsStableCodePoint(c, this.form)) {
            chars[index] = c;
            ++total;
            ++index;
          } else {
            this.Unget();
            break;
          }
        }
        if (total == length) {
          return total;
        }
      }
      do {
        // Console.WriteLine("indexes=" + this.processedIndex + " " + this.flushIndex + ", length=" + length + " total=" + (total));
        count = Math.Min(this.processedIndex - this.flushIndex, length - total);
        if (count < 0) {
          count = 0;
        }
        if (count != 0) {
          #if DEBUG
          if (this.buffer == null) {
            throw new ArgumentException("buffer is null");
          }
          #endif
          // Fill buffer with processed code points
          Array.Copy(this.buffer, this.flushIndex, chars, index, count);
        }
        index += count;
        total += count;
        this.flushIndex += count;
        // Try to fill buffer with stable code points,
        // as an optimization
        while (total < length) {
          // charbufpos, charbufend);
          int c = this.GetNextChar();
          if (c < 0) {
            this.endOfString = true;
            break;
          }
          if (Normalizer.IsStableCodePoint(c, this.form)) {
            chars[index++] = c;
            ++total;
          } else {
            this.Unget();
            break;
          }
        }
        // Ensure that more data is available
        if (total < length && this.flushIndex == this.processedIndex) {
          if (this.lastStableIndex > 0) {
            // Move unprocessed data to the beginning of
            // the buffer
            #if DEBUG
            if (this.buffer == null) {
              throw new ArgumentNullException("this.buffer");
            }
            if (this.endIndex < this.lastStableIndex) {
              throw new ArgumentException("endIndex less than lastStableIndex");
            }
            #endif
            Array.Copy(this.buffer, this.lastStableIndex, this.buffer, 0, this.buffer.Length - this.lastStableIndex);
            // Console.WriteLine("endIndex=" + (this.endIndex));
            this.endIndex -= this.lastStableIndex;
            this.lastStableIndex = 0;
          } else {
            this.endIndex = 0;
          }
          if (!this.LoadMoreData()) {
            break;
          }
        }
      } while (total < length);
      // Fill buffer with processed code points
      count = Math.Max(0, Math.Min(this.processedIndex - this.flushIndex, length - total));
      Array.Copy(this.buffer, this.flushIndex, chars, index, count);
      index += count;
      total += count;
      this.flushIndex += count;
      return (total == 0) ? -1 : total;
    }

    private bool LoadMoreData() {
      bool done = false;
      while (!done) {
        if (this.buffer == null) {
          this.buffer = new int[32];
        }
        // Fill buffer with decompositions until the buffer is full
        // or the end of the string is reached.
        while (this.endIndex + 18 <= this.buffer.Length) {
          int c = this.GetNextChar();
          if (c < 0) {
            this.endOfString = true;
            break;
          }
          this.endIndex = Normalizer.DecompToBuffer(c, this.compatMode, this.buffer, this.endIndex);
        }
        // Check for the last stable code point if the
        // end of the string is not reached yet
        if (!this.endOfString) {
          bool haveNewStable = false;
          // NOTE: lastStableIndex begins at -1
          for (int i = this.endIndex - 1; i > this.lastStableIndex; --i) {
            // Console.WriteLine("stable({0:X4})=" + (IsStableCodePoint(this.buffer[i], this.form)));
            if (Normalizer.IsStableCodePoint(this.buffer[i], this.form)) {
              this.lastStableIndex = i;
              haveNewStable = true;
              break;
            }
          }
          if (!haveNewStable || this.lastStableIndex <= 0) {
            // No stable code point was found (or last stable
            // code point is at beginning of buffer), increase
            // the buffer size
            int[] newBuffer = new int[(this.buffer.Length + 4) * 2];
            Array.Copy(this.buffer, 0, newBuffer, 0, this.buffer.Length);
            this.buffer = newBuffer;
            continue;
          }
        } else {
          // End of string
          this.lastStableIndex = this.endIndex;
        }
        done = true;
      }
      // No data in buffer
      if (this.endIndex == 0) {
        return false;
      }
      this.flushIndex = 0;
      // Canonical reordering
      Normalizer.ReorderBuffer(this.buffer, 0, this.lastStableIndex);
      if (this.form == Normalization.NFC || this.form == Normalization.NFKC) {
        // Composition
        this.processedIndex = Normalizer.ComposeBuffer(this.buffer, this.lastStableIndex);
      } else {
        this.processedIndex = this.lastStableIndex;
      }
      return true;
    }
  }
}
