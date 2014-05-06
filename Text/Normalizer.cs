/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

namespace PeterO.Text {
  /// <summary>Implements the Unicode normalization algorithm and contains
  /// methods and functionality to test and convert Unicode strings for
  /// Unicode normalization.</summary>
  public sealed class Normalizer
  {
    public static string Normalize(string str, PeterO.Text.Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      Normalizer norm = new Normalizer(str, form);
      StringBuilder builder = new StringBuilder();
      int c = 0;
      while ((c = norm.ReadChar()) >= 0) {
        if (c <= 0xffff) {
          builder.Append((char)c);
        } else if (c <= 0x10ffff) {
          builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          builder.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
      return builder.ToString();
    }

    internal static int DecompToBufferInternal(int ch, bool compat, int[] buffer, int index) {
      #if DEBUG
      if (buffer == null) {
        throw new ArgumentNullException("buffer");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > buffer.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)buffer.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      #endif

      int offset = PeterO.Text.UnicodeDatabase.GetDecomposition(ch, compat, buffer, index);
      if (buffer[index] != ch) {
        int[] copy = new int[offset - index];
        Array.Copy(buffer, index, copy, 0, copy.Length);
        offset = index;
        foreach (int element in copy) {
          offset = DecompToBufferInternal(element, compat, buffer, offset);
        }
      }
      return offset;
    }

    private int DecompToBuffer(int ch, bool compat, int[] buffer, int index) {
      #if DEBUG
      if (buffer == null) {
        throw new ArgumentNullException("buffer");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > buffer.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)buffer.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      #endif

      if (ch >= 0xac00 && ch < 0xac00 + 11172) {
        // Hangul syllable
        int valueSIndex = ch - 0xac00;
        int trail = 0x11a7 + (valueSIndex % 28);
        buffer[index++] = 0x1100 + (valueSIndex / 588);
        buffer[index++] = 0x1161 + ((valueSIndex % 588) / 28);
        if (trail != 0x11a7) {
          buffer[index++] = trail;
        }
        return index;
      } else {
        return DecompToBufferInternal(ch, compat, buffer, index);
      }
    }

    private int lastStableIndex;
    private int endIndex;
    private int iterEndIndex;
    private int[] buffer;
    private bool compatMode;
    private PeterO.Text.Normalization form;
    private int processedIndex;
    private int flushIndex;
    private string iterator;
    private int characterListPos;

    public Normalizer(string str, PeterO.Text.Normalization form) {
      if (str == null) {
        throw new ArgumentException("stream");
      }
      this.iterEndIndex = str.Length;
      this.lastStableIndex = -1;
      this.iterator = str;
      this.form = form;
      this.compatMode = form == PeterO.Text.Normalization.NFKC || form == PeterO.Text.Normalization.NFKD;
    }

    private Normalizer Init(string str, int index, int length, PeterO.Text.Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(str.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(str.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (str.Length - index < length) {
        throw new ArgumentException("str's length minus " + index + " (" + Convert.ToString((long)(str.Length-index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      this.lastStableIndex = -1;
      this.characterListPos = index;
      this.iterator = str;
      this.iterEndIndex = index + length;
      this.form = form;
      this.compatMode = form == PeterO.Text.Normalization.NFKC || form == PeterO.Text.Normalization.NFKD;
      return this;
    }

    private static bool NormalizeAndCheckString(
      string charList,
      int start,
      int length,
      PeterO.Text.Normalization form) {
      int i = start;
      Normalizer norm = new Normalizer(charList, form).Init(charList, start, length, form);
      int ch = 0;
      while ((ch = norm.ReadChar()) >= 0) {
        int c = charList[i];
        if ((c & 0xfc00) == 0xd800 && i + 1 < charList.Length &&
            charList[i + 1] >= 0xdc00 && charList[i + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (charList[i + 1] - 0xdc00);
          ++i;
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate
          c = 0xfffd;
        }
        ++i;
        if (c != ch) {
          return false;
        }
      }
      return i == start + length;
    }

    public static bool IsNormalized(string str, PeterO.Text.Normalization form) {
      if (str == null) {
        return false;
      }
      int lastNonStable = -1;
      int mask = (form == PeterO.Text.Normalization.NFC) ? 0xff : 0x7f;
      for (int i = 0; i < str.Length; ++i) {
        int c = str[i];
        if ((c & 0xfc00) == 0xd800 && i + 1 < str.Length &&
            str[i + 1] >= 0xdc00 && str[i + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (str[i + 1] - 0xdc00);
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate
          return false;
        }
        bool isStable = false;
        if ((c & mask) == c && (i + 1 == str.Length || (str[i + 1] & mask) == str[i + 1])) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC) or the end of string.
          // Treat the first character as stable
          // in this situation.
          isStable = true;
        } else {
          isStable = IsStableCodePoint(c, form);
        }
        if (lastNonStable < 0 && !isStable) {
          // First non-stable code point in a row
          lastNonStable = i;
        } else if (lastNonStable >= 0 && isStable) {
          // We have at least one non-stable code point,
          // normalize these code points.
          if (!NormalizeAndCheckString(str, lastNonStable, i - lastNonStable, form)) {
            return false;
          }
          lastNonStable = -1;
        }
        if (c >= 0x10000) {
          ++i;
        }
      }
      if (lastNonStable >= 0) {
        if (!NormalizeAndCheckString(str, lastNonStable, str.Length - lastNonStable, form)) {
          return false;
        }
      }
      return true;
    }

    private int[] readbuffer = new int[1];

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
      } else if(this.characterListPos>=this.iterEndIndex){
        ch = -1;
      } else {
        ch = this.iterator[this.characterListPos];
        if ((ch & 0xfc00) == 0xd800 && this.characterListPos + 1 < this.iterEndIndex &&
            this.iterator[this.characterListPos + 1] >= 0xdc00 && this.iterator[this.characterListPos + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          ch = 0x10000 + ((ch - 0xd800) << 10) + (this.iterator[this.characterListPos + 1] - 0xdc00);
          ++this.characterListPos;
        } else if ((ch & 0xf800) == 0xd800) {
          // unpaired surrogate
          ch = 0xfffd;
        }
        ++this.characterListPos;
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
    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new ArgumentNullException("chars");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > chars.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)chars.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > chars.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)chars.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (chars.Length - index < length) {
        throw new ArgumentException("chars's length minus " + index + " (" + Convert.ToString((long)(chars.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
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
          } else if (PeterO.Text.UnicodeDatabase.IsStableCodePoint(c, this.form)) {
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
          // DebugUtility.log("before mark total=%d length=%d",
          // charbufpos, charbufend);
          int c = this.GetNextChar();
          if (c < 0) {
            this.endOfString = true;
            break;
          }
          // DebugUtility.log("%04X %s",c,IsStableCodePoint(c,this.form));
          if (IsStableCodePoint(c, this.form)) {
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

    private static bool IsStableCodePoint(int cp, PeterO.Text.Normalization form) {
      // Exclude YOD and HIRIQ because of Corrigendum 2
      return PeterO.Text.UnicodeDatabase.IsStableCodePoint(cp, form) && cp != 0x5b4 && cp != 0x5d9;
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
          this.endIndex = this.DecompToBuffer(c, this.compatMode, this.buffer, this.endIndex);
        }
        // Check for the last stable code point if the
        // end of the string is not reached yet
        if (!this.endOfString) {
          bool haveNewStable = false;
          // NOTE: lastStableIndex begins at -1
          for (int i = this.endIndex - 1; i > this.lastStableIndex; --i) {
            // Console.WriteLine("stable({0:X4})=" + (IsStableCodePoint(this.buffer[i], this.form)));
            if (IsStableCodePoint(this.buffer[i], this.form)) {
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
      this.ReorderBuffer(this.buffer, 0, this.lastStableIndex);
      if (this.form == PeterO.Text.Normalization.NFC || this.form == PeterO.Text.Normalization.NFKC) {
        // Composition
        this.processedIndex = this.ComposeBuffer(this.buffer, this.lastStableIndex);
      } else {
        this.processedIndex = this.lastStableIndex;
      }
      return true;
    }

    private void ReorderBuffer(int[] buffer, int index, int length) {
      int i;
      #if DEBUG
      if (buffer == null) {
        throw new ArgumentNullException("buffer");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > buffer.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)buffer.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > buffer.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)buffer.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (buffer.Length - index < length) {
        throw new ArgumentException("buffer's length minus " + index + " (" + Convert.ToString((long)(buffer.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      #endif

      if (length < 2) {
        return;
      }
      bool changed;
      do {
        changed = false;
        // Console.WriteLine(ToString(buffer, index, length));
        int lead = PeterO.Text.UnicodeDatabase.GetCombiningClass(buffer[index]);
        int trail;
        for (i = 1; i < length; ++i) {
          int offset = index + i;
          trail = PeterO.Text.UnicodeDatabase.GetCombiningClass(buffer[offset]);
          if (trail != 0 && lead > trail) {
            int c = buffer[offset - 1];
            buffer[offset - 1] = buffer[offset];
            buffer[offset] = c;
            // Console.WriteLine("lead= {0:X4} ccc=" + (lead));
            // Console.WriteLine("trail={0:X4} ccc=" + (trail));
            // Console.WriteLine("now "+ToString(buffer,index,length));
            changed = true;
            // Lead is now at trail's position
          } else {
            lead = trail;
          }
        }
      } while (changed);
    }

    private int ComposeBuffer(int[] array, int length) {
      #if DEBUG
      if (array == null) {
        throw new ArgumentNullException("array");
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > array.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)array.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (array.Length < length) {
        throw new ArgumentException("array's length (" + Convert.ToString((long)array.Length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      #endif

      if (length < 2) {
        return length;
      }
      int starterPos = 0;
      int retval = length;
      int starter = array[0];
      int last = PeterO.Text.UnicodeDatabase.GetCombiningClass(starter);
      if (last != 0) {
        last = 256;
      }
      int compPos = 0;
      int endPos = 0 + length;
      bool composed = false;
      for (int decompPos = compPos; decompPos < endPos; ++decompPos) {
        int ch = array[decompPos];
        int valuecc = PeterO.Text.UnicodeDatabase.GetCombiningClass(ch);
        if (decompPos > compPos) {
          int lead = starter - 0x1100;
          if (0 <= lead && lead < 19) {
            // Found Hangul L jamo
            int vowel = ch - 0x1161;
            if (0 <= vowel && vowel < 21 && (last < valuecc || last == 0)) {
              starter = 0xac00 + (((lead * 21) + vowel) * 28);
              array[starterPos] = starter;
              array[decompPos] = 0x110000;
              composed = true;
              --retval;
              continue;
            }
          }
          int syllable = starter - 0xac00;
          if (0 <= syllable && syllable < 11172 && (syllable % 28) == 0) {
            // Found Hangul LV jamo
            int trail = ch - 0x11a7;
            if (0 < trail && trail < 28 && (last < valuecc || last == 0)) {
              starter += trail;
              array[starterPos] = starter;
              array[decompPos] = 0x110000;
              composed = true;
              --retval;
              continue;
            }
          }
        }
        int composite = PeterO.Text.UnicodeDatabase.GetComposedPair(starter, ch);
        bool diffClass = last < valuecc;
        if (composite >= 0 && (diffClass || last == 0)) {
          array[starterPos] = composite;
          starter = composite;
          array[decompPos] = 0x110000;
          composed = true;
          --retval;
          continue;
        }
        if (valuecc == 0) {
          starterPos = decompPos;
          starter = ch;
        }
        last = valuecc;
      }
      if (composed) {
        int j = compPos;
        for (int i = compPos; i < endPos; ++i) {
          if (array[i] != 0x110000) {
            array[j++] = array[i];
          }
        }
      }
      return retval;
    }
  }
}
