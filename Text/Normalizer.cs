using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Text {
    /// <summary>Not documented yet.</summary>
  public sealed class Normalizer : ICharacterInput
  {
    public static IList<int> GetChars(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return GetChars(new StringCharacterInput(str), form);
    }

    /// <summary>Converts a string to Unicode normalization form C.</summary>
    /// <param name='str'>A string. Cannot be null.</param>
    /// <returns>The normalized string.</returns>
    public static string Normalize(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (str.Length < 1000) {
        bool allLatinOne = true;
        for (int i = 0; i < str.Length; ++i) {
          if ((str[i] >> 8) != 0) {
            allLatinOne = false;
            break;
          }
        }
        if (allLatinOne) {
          return str;
        }
      }
      return Normalize(str, Normalization.NFC);
    }

    public static string Normalize(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      Normalizer norm = new Normalizer(str, form);
      StringBuilder builder = new StringBuilder();
      int c = 0;
      while ((c = norm.Read()) >= 0) {
        if (c <= 0xffff) {
          builder.Append((char)c);
        } else if (c <= 0x10ffff) {
          builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          builder.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
      return builder.ToString();
    }

    public static IList<int> GetChars(ICharacterInput str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      Normalizer norm = new Normalizer(str, form);
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

      int offset = UnicodeDatabase.GetDecomposition(ch, compat, buffer, index);
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
    private int[] buffer;
    private bool compatMode;
    private Normalization form;
    private int processedIndex;
    private int flushIndex;
    private int basicEnd;
    private ICharacterInput iterator;
    private IList<int> characterList;
    private int characterListPos;

    /// <summary>Initializes a new instance of the Normalizer class using
    /// Normalization Form C.</summary>
    /// <param name='characterList'>An IList object.</param>
    public Normalizer(IList<int> characterList) : this(characterList, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the Normalizer class using
    /// Normalization Form C.</summary>
    /// <param name='str'>A string object.</param>
    public Normalizer(string str) : this(str, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the Normalizer class using
    /// Normalization Form C.</summary>
    /// <param name='input'>An ICharacterInput object.</param>
    public Normalizer(ICharacterInput input) : this(input, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the Normalizer class using
    /// the given normalization form.</summary>
    /// <param name='characterList'>An IList object.</param>
    /// <param name='form'>A Normalization object.</param>
    public Normalizer(IList<int> characterList, Normalization form) {
      if (characterList == null) {
        throw new ArgumentException("characterList");
      }
      this.lastStableIndex = -1;
      this.characterList = characterList;
      this.form = form;
      this.basicEnd = (form == Normalization.NFC) ? 0x100 : 0x80;
      this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
    }

    public Normalizer(string str, Normalization form) :
      this(new StringCharacterInput(str), form) {
    }

    public Normalizer(ICharacterInput stream, Normalization form) {
      if (stream == null) {
        throw new ArgumentException("stream");
      }
      this.lastStableIndex = -1;
      this.iterator = stream;
      this.form = form;
      this.basicEnd = (form == Normalization.NFC) ? 0x100 : 0x80;
      this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
    }

    public static bool IsNormalized(string str) {
      return IsNormalized(str, Normalization.NFC);
    }

    public static bool IsNormalized(string str, Normalization form) {
      int maxbasic = (form == Normalization.NFC) ? 0xff : 0x7f;
      bool basic = true;
      for (int i = 0; i < str.Length; ++i) {
        if (str[i] > maxbasic) {
          basic = false;
          break;
        }
        if ((str[i] & 0xf800) == 0xd800) {
          int cp = DataUtilities.CodePointAt(str, i);
          if (cp == 0xfffd) {
            return false;
          }
        }
      }
      if (basic) {
        return true;
      }
      return IsNormalized(new StringCharacterInput(str), form);
    }

    public static bool IsNormalized(ICharacterInput chars, Normalization form) {
      IList<int> list = new List<int>();
      int ch = 0;
      while ((ch = chars.Read()) >= 0) {
        if (ch >= 0xd800 && ch <= 0xdfff) {
          return false;
        }
        list.Add(ch);
      }
      return IsNormalized(list, form);
    }

    private static bool NormalizeAndCheck(
      IList<int> chars,
      int start,
      int length,
      Normalization form) {
      int i = 0;
      foreach (int ch in Normalizer.GetChars(
        new PartialListCharacterInput(chars, start, length),
        form)) {
        if (i >= length) {
          return false;
        }
        if (ch != chars[start + i]) {
          return false;
        }
        ++i;
      }
      return true;
    }

    public static bool IsNormalized(IList<int> chars, Normalization form) {
      int lastNonStable = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      if (chars == null) {
        throw new ArgumentException("chars");
      }
      for (int i = 0; i < chars.Count; ++i) {
        int c = chars[i];
        if (c < 0 || c > 0x10ffff || ((c & 0x1ff800) == 0xd800)) {
          return false;
        }
        bool isStable = false;
        if ((c & mask) == c && (i + 1 == chars.Count || (chars[i + 1] & mask) == chars[i + 1])) {
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
          if (!NormalizeAndCheck(chars, lastNonStable, i - lastNonStable, form)) {
            return false;
          }
          lastNonStable = -1;
        }
      }
      if (lastNonStable >= 0) {
        if (!NormalizeAndCheck(chars, lastNonStable, chars.Count - lastNonStable, form)) {
          return false;
        }
      }
      return true;
    }

    private int[] readbuffer = new int[1];

    public int Read() {
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
        ch = this.iterator.Read();
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
          } else if (UnicodeDatabase.IsStableCodePoint(c, this.form)) {
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

    private static bool IsStableCodePoint(int cp, Normalization form) {
      // Exclude YOD and HIRIQ because of Corrigendum 2
      return UnicodeDatabase.IsStableCodePoint(cp, form) && cp != 0x5b4 && cp != 0x5d9;
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
      if (this.form == Normalization.NFC || this.form == Normalization.NFKC) {
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
        int lead = UnicodeDatabase.GetCombiningClass(buffer[index]);
        int trail;
        for (i = 1; i < length; ++i) {
          int offset = index + i;
          trail = UnicodeDatabase.GetCombiningClass(buffer[offset]);
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

    private int ComposeBuffer(int[] list, int length) {
      #if DEBUG
      if (list == null) {
        throw new ArgumentNullException("list");
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > list.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)list.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (list.Length < length) {
        throw new ArgumentException("list's length (" + Convert.ToString((long)list.Length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      #endif

      if (length < 2) {
        return length;
      }
      int starterPos = 0;
      int retval = length;
      int starterCh = list[0];
      int lastClass = UnicodeDatabase.GetCombiningClass(starterCh);
      if (lastClass != 0) {
        lastClass = 256;
      }
      int compPos = 0;
      int endPos = 0 + length;
      bool composed = false;
      for (int decompPos = compPos; decompPos < endPos; ++decompPos) {
        int ch = list[decompPos];
        int valueChClass = UnicodeDatabase.GetCombiningClass(ch);
        if (decompPos > compPos) {
          int lead = starterCh - 0x1100;
          if (0 <= lead && lead < 19) {
            // Found Hangul L jamo
            int vowel = ch - 0x1161;
            if (0 <= vowel && vowel < 21 && (lastClass < valueChClass || lastClass == 0)) {
              starterCh = 0xac00 + (((lead * 21) + vowel) * 28);
              list[starterPos] = starterCh;
              list[decompPos] = 0x110000;
              composed = true;
              --retval;
              continue;
            }
          }
          int syllable = starterCh - 0xac00;
          if (0 <= syllable && syllable < 11172 && (syllable % 28) == 0) {
            // Found Hangul LV jamo
            int trail = ch - 0x11a7;
            if (0 < trail && trail < 28 && (lastClass < valueChClass || lastClass == 0)) {
              starterCh += trail;
              list[starterPos] = starterCh;
              list[decompPos] = 0x110000;
              composed = true;
              --retval;
              continue;
            }
          }
        }
        int composite = UnicodeDatabase.GetComposedPair(starterCh, ch);
        bool diffClass = lastClass < valueChClass;
        if (composite >= 0 && (diffClass || lastClass == 0)) {
          list[starterPos] = composite;
          starterCh = composite;
          list[decompPos] = 0x110000;
          composed = true;
          --retval;
          continue;
        }
        if (valueChClass == 0) {
          starterPos = decompPos;
          starterCh = ch;
        }
        lastClass = valueChClass;
      }
      if (composed) {
        int j = compPos;
        for (int i = compPos; i < endPos; ++i) {
          if (list[i] != 0x110000) {
            list[j++] = list[i];
          }
        }
      }
      return retval;
    }
  }
}
