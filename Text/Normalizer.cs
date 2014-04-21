using System;
using System.Collections.Generic;
using System.IO;

namespace PeterO.Text {
  /// <summary>Not documented yet.</summary>
  public sealed class Normalizer : ICharacterInput
  {
    public static IList<int> GetChars(string str, Normalization form) {
      if ((str) == null) {
        throw new ArgumentNullException("str");
      }
      return GetChars(new StringCharacterInput(str), form);
    }
    public static IList<int> GetChars(ICharacterInput str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      Normalizer norm = new Normalizer(str, form);
      int[] buffer = new int[64];
      IList<int> ret = new List<int>();
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

    public static bool IsNormalized(IList<int> chars, Normalization form) {
      int lastNonStable = -1;
      bool fastCheck = true;
      int mask = (form == Normalization.NFC) ? unchecked((int)0xffffff00) :
        unchecked((int)0xffffff80);
      if (chars == null) {
        throw new ArgumentException("chars");
      }
      for (int i = 0; i < chars.Count; ++i) {
        int c = chars[i];
        if (c < 0 || c > 0x10ffff || (c >= 0xd800 && c <= 0xdfff)) {
          throw new ArgumentException("'chars' contains an invalid Unicode code point.");
        }
        if (fastCheck && (c & mask) == 0) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC).
          // No normalization is ever necessary
          // in this situation.
          continue;
        } else {
          bool isStable = IsStableCodePoint(c, form);
          fastCheck = false;
          if (lastNonStable < 0 && !isStable) {
            // First non-stable code point in a row
            lastNonStable = i;
          } else if (lastNonStable >= 0 && isStable) {
            // We have at least one non-stable code point,
            // normalize these code points.
            int j = lastNonStable;
            foreach (int ch in Normalizer.GetChars(
              new PartialListCharacterInput(chars, lastNonStable, i - lastNonStable),
              form)) {
              if (ch != chars[j++]) {
                return false;
              }
            }
            // If result's length doesn't match
            if (j != i) {
              return false;
            }
            lastNonStable = -1;
          }
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
    private int[] charbuffer = new int[4];
    private bool charbufmark = true;
    private int charbufpos = 0;
    private int charbufend = 0;

    private void Unmark() {
      this.charbufmark = false;
    }

    private void Mark() {
      // Already have unread data; shift buffer to the beginning
      if (this.charbufpos < this.charbufend) {
        Array.Copy(this.charbuffer, this.charbufpos, this.charbuffer, 0, this.charbufend - this.charbufpos);
      }
      this.charbufend -= this.charbufpos;
      this.charbufpos = 0;
      this.charbufmark = true;
    }

    private void Unget(int length) {
      this.charbufpos -= length;
      this.charbufpos = Math.Max(this.charbufpos, 0);
    }

    private int GetNextChar() {
      if (this.charbufpos < this.charbufend) {
        return this.charbuffer[this.charbufpos++];
      }
      int ch;
      if (this.iterator == null) {
        ch = (this.characterListPos >= this.characterList.Count) ? -1 : this.characterList[this.characterListPos++];
      } else {
        ch = this.iterator.Read();
      }
      if (ch < 0) {
        this.endOfString = true;
      } else if (ch > 0x10ffff || (ch >= 0xd800 && ch <= 0xdfff)) {
        throw new IOException();
      }
      if (this.charbufmark) {
        if (this.charbufpos >= this.charbufend) {
          if (this.charbufend >= this.charbuffer.Length) {
            Array.Copy(this.charbuffer, this.charbufpos, this.charbuffer, 0, this.charbufend - this.charbufpos);
            --this.charbufpos;
            --this.charbufend;
          }
        }
        this.charbuffer[this.charbufpos++] = ch;
        ++this.charbufend;
      }
      return ch;
    }

    // static bool stable = false;
    private bool basic = true;

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
      // Fill buffer with processed code points
      do {
        //Console.WriteLine("indexes=" + this.processedIndex + " " + this.flushIndex + ", length=" + length + " total=" + (total));
        count = Math.Max(0, Math.Min(this.processedIndex - this.flushIndex, length - total));
        if (count != 0) {
          #if DEBUG
          if (this.buffer == null) {
            throw new ArgumentException("buffer is null");
          }
          #endif
          Array.Copy(this.buffer, this.flushIndex, chars, index, count);
        }
        index += count;
        total += count;
        this.flushIndex += count;
        // Check for all-ASCII text (all-Latin-1 text for NFC)
        while (this.basic && total < length) {
          this.Mark();
          int c = this.GetNextChar();
          // DebugUtility.log("%c [%04X] ascii %d %d",(char)c,c,
          // charbufpos, charbufend);
          if (c < 0) {
            this.endOfString = true;
            break;
          } else if (c < this.basicEnd) {
            while (total < length) {
              int c2 = this.GetNextChar();
              if (c2 < 0) {
                this.endOfString = true;
                chars[index++] = c;
                ++total;
                break;
              } else if (c2 < this.basicEnd) {
                chars[index++] = c;
                ++total;
                this.Unget(1);
                c = c2;
              } else {
                this.Unget(2);
                break;
              }
            }
            break;
          } else {
            this.basic = false;
            this.Unget(1);
            // DebugUtility.log("%c ascii false %d %d %d %d",(char)c,
            // charbufpos, charbufend, total, length);
            break;
          }
        }
        // DebugUtility.log("unmarking total=%d length=%d",
        // charbufpos, charbufend);
        this.Unmark();
        // Try to fill buffer with stable code points,
        // as an optimization
        while (total < length) {
          // DebugUtility.log("before mark total=%d length=%d",
          // charbufpos, charbufend);
          this.Mark();
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
            this.Unget(1);
            break;
          }
        }
        this.Unmark();
        // Ensure that more data is available
        if (total < length && this.flushIndex == this.processedIndex) {
          if (this.lastStableIndex > 0) {
            // Move unprocessed data to the beginning of
            // the buffer
            #if DEBUG
            if (this.buffer == null) {
              throw new ArgumentNullException("this.buffer");
            }
            if (this.endIndex<this.lastStableIndex) {
              throw new ArgumentException("endIndex less than lastStableIndex");
            }
            #endif
            Array.Copy(this.buffer, this.lastStableIndex, this.buffer, 0, this.buffer.Length - this.lastStableIndex);
            //       Console.WriteLine("endIndex=" + (this.endIndex));
            this.endIndex-=this.lastStableIndex;
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
            //Console.WriteLine("stable({0:X4})=" + (IsStableCodePoint(this.buffer[i], this.form)));
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
        //Console.WriteLine(ToString(buffer, index, length));
        int lead = UnicodeDatabase.GetCombiningClass(buffer[index]);
        int trail;
        for (i = 1; i < length; ++i) {
          int offset = index + i;
          trail = UnicodeDatabase.GetCombiningClass(buffer[offset]);
          if (trail != 0 && lead > trail) {
            int c = buffer[offset - 1];
            buffer[offset - 1] = buffer[offset];
            buffer[offset] = c;
            //Console.WriteLine("lead= {0:X4} ccc=" + (lead));
            //Console.WriteLine("trail={0:X4} ccc=" + (trail));
            //Console.WriteLine("now "+ToString(buffer,index,length));
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
              starterCh = 0xac00 + ((lead * 21 + vowel) * 28);
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
