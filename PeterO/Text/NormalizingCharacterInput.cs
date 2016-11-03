/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;

namespace PeterO.Text {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Text.NormalizingCharacterInput"]/*'/>
  public sealed class NormalizingCharacterInput : ICharacterInput {
    internal static int DecompToBufferInternal(
  int ch,
  bool compat,
  int[] buffer,
  int index) {
#if DEBUG
      if (buffer == null) {
        throw new ArgumentNullException("buffer");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
            "0");
      }
      if (index > buffer.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          buffer.Length);
      }
#endif
      int offset = UnicodeDatabase.GetDecomposition(
      ch,
      compat,
      buffer,
      index);
      if (buffer[index] != ch) {
        var copy = new int[offset - index];
        Array.Copy(buffer, index, copy, 0, copy.Length);
        offset = index;
        for (int i = 0; i < copy.Length; ++i) {
          offset = DecompToBufferInternal(copy[i], compat, buffer, offset);
        }
      }
      return offset;
    }

    internal static int DecompToBuffer(
  int ch,
  bool compat,
  int[] buffer,
  int index) {
#if DEBUG
      if (buffer == null) {
        throw new ArgumentNullException("buffer");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
            "0");
      }
      if (index > buffer.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          buffer.Length);
      }
#endif

      if (ch >= 0xac00 && ch < 0xac00 + 11172) {
        // Hangul syllable
        int syllable = ch - 0xac00;
        int trail = syllable % 28;
        buffer[index++] = 0x1100 + (syllable / 588);
        buffer[index++] = 0x1161 + ((syllable % 588) / 28);
        if (trail != 0) {
          buffer[index++] = 0x11a7 + trail;
        }
        return index;
      }
      return DecompToBufferInternal(ch, compat, buffer, index);
    }

    internal static void ReorderBuffer(int[] buffer, int index, int length) {
      int i;
      if (length < 2) {
        return;
      }
      bool changed;
      do {
        changed = false;
        int lead = UnicodeDatabase.GetCombiningClass(buffer[index]);
        int trail;
        for (i = 1; i < length; ++i) {
          int offset = index + i;
          trail = UnicodeDatabase.GetCombiningClass(buffer[offset]);
          if (trail != 0 && lead > trail) {
            int c = buffer[offset - 1];
            buffer[offset - 1] = buffer[offset];
            buffer[offset] = c;
            changed = true;
          } else {
            lead = trail;
          }
        }
      } while (changed);
    }

    internal static int ComposeBuffer(int[] array, int length) {
      if (length < 2) {
        return length;
      }
      var starterPos = 0;
      int retval = length;
      int starter = array[0];
      int last = UnicodeDatabase.GetCombiningClass(starter);
      if (last != 0) {
        last = 256;
      }
      int endPos = 0 + length;
      var composed = false;
      for (int decompPos = 0; decompPos < endPos; ++decompPos) {
        int ch = array[decompPos];
        int valuecc = UnicodeDatabase.GetCombiningClass(ch);
        if (decompPos > 0) {
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
        int composite = UnicodeDatabase.GetComposedPair(starter, ch);
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
        var j = 0;
        for (int i = 0; i < endPos; ++i) {
          if (array[i] != 0x110000) {
            array[j++] = array[i];
          }
        }
      }
      return retval;
    }

    private static IList<int> GetChars(ICharacterInput input) {
      var buffer = new int[64];
      IList<int> ret = new List<int>(24);
      var count = 0;
      while ((count = input.Read(buffer, 0, buffer.Length)) > 0) {
        for (int i = 0; i < count; ++i) {
          ret.Add(buffer[i]);
        }
      }
      return ret;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.GetChars(System.String,PeterO.Text.Normalization)"]/*'/>
    public static IList<int> GetChars(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return GetChars(new NormalizingCharacterInput(str, form));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.GetChars(PeterO.Text.ICharacterInput,PeterO.Text.Normalization)"]/*'/>
    public static IList<int> GetChars(ICharacterInput str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return GetChars(new NormalizingCharacterInput(str, form));
    }

    private int lastQcsIndex;
    private int endIndex;
    private int[] buffer;
    private readonly bool compatMode;
    private readonly Normalization form;
    private int processedIndex;
    private int flushIndex;
    private readonly ICharacterInput iterator;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.Collections.Generic.IList{System.Int32})"]/*'/>
    public NormalizingCharacterInput(IList<int> characterList) :
      this(characterList, Normalization.NFC) {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.String)"]/*'/>
    public NormalizingCharacterInput(
  string str) : this(
  str,
  Normalization.NFC) {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(PeterO.Text.ICharacterInput)"]/*'/>
    public NormalizingCharacterInput(
  ICharacterInput input) : this(
  input,
  Normalization.NFC) {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.Collections.Generic.IList{System.Int32},PeterO.Text.Normalization)"]/*'/>
    public NormalizingCharacterInput(
  IList<int> characterList,
  Normalization form) :
  this(new PartialListCharacterInput(characterList), form) {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.String,System.Int32,System.Int32,PeterO.Text.Normalization)"]/*'/>
    public NormalizingCharacterInput(
  string str,
  int index,
  int length,
  Normalization form) : this(
  new StringCharacterInput2(str, index, length),
  form) {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.String,PeterO.Text.Normalization)"]/*'/>
    public NormalizingCharacterInput(string str, Normalization form) :
      this(new StringCharacterInput2(str), form) {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(PeterO.Text.ICharacterInput,PeterO.Text.Normalization)"]/*'/>
    public NormalizingCharacterInput(
   ICharacterInput stream,
   Normalization form) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.lastQcsIndex = -1;
      this.iterator = stream;
      this.form = form;
      this.lastCharBuffer = new int[2];
      this.compatMode = form == Normalization.NFKC || form ==
          Normalization.NFKD;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.IsNormalized(PeterO.Text.ICharacterInput,PeterO.Text.Normalization)"]/*'/>
    public static bool IsNormalized(ICharacterInput chars, Normalization form) {
      if (chars == null) {
        throw new ArgumentNullException("chars");
      }
      IList<int> list = new List<int>();
      var ch = 0;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      var norm = true;
      while ((ch = chars.ReadChar()) >= 0) {
        if ((ch & 0x1ff800) == 0xd800) {
          return false;
        }
        if (norm && (ch & mask) != ch) {
          norm = false;
        }
        list.Add(ch);
      }
      return norm || IsNormalized(list, form);
    }

    private static bool NormalizeAndCheck(
  IList<int> charList,
  int start,
  int length,
  Normalization form) {
      var i = 0;
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.Normalize(System.String,PeterO.Text.Normalization)"]/*'/>
    public static string Normalize(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (str.Length <= 1024 && IsNormalized(str, form)) {
        return str;
      }
      return Encodings.InputToString(
        new NormalizingCharacterInput(str, form));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.IsNormalized(System.String,PeterO.Text.Normalization)"]/*'/>
    public static bool IsNormalized(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      var nonQcsStart = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      var lastQcs = 0;
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
        var isQcs = false;
        if ((c & mask) == c && (i + 1 == str.Length || (str[i + 1] & mask)
          == str[i + 1])) {
          // Quick check for an ASCII character (or Latin-1 in NFC) followed
          // by another
          // ASCII character (or Latin-1 in NFC) or the end of string.
          // Treat the first character as QCS
          // in this situation.
          isQcs = true;
        } else {
        isQcs = (c >= 0xf0000) ? true :
UnicodeDatabase.IsQuickCheckStarter(
  c,
  form);
        }
        if (isQcs) {
          lastQcs = i;
        }
        if (nonQcsStart < 0 && !isQcs) {
          // First non-quick-check starter in a row
          nonQcsStart = lastQcs;
        } else if (nonQcsStart >= 0 && isQcs) {
          // We have at least one non-quick-check-starter,
          // normalize these code points.
          if (!NormalizeAndCheckString(
         str,
         nonQcsStart,
         i - nonQcsStart,
         form)) {
            return false;
          }
          nonQcsStart = -1;
        }
        if (c >= 0x10000) {
          ++i;
        }
      }
      if (nonQcsStart >= 0) {
        if (!NormalizeAndCheckString(
  str,
  nonQcsStart,
  str.Length - nonQcsStart,
  form)) {
          return false;
        }
      }
      return true;
    }

    private static bool NormalizeAndCheckString(
      string charString,
      int start,
      int length,
      Normalization form) {
      int i = start;
      var norm = new NormalizingCharacterInput(
     charString,
     start,
     length,
     form);
      var ch = 0;
      while ((ch = norm.ReadChar()) >= 0) {
        int c = charString[i];
        if ((c & 0x1ffc00) == 0xd800 && i + 1 < charString.Length &&
            charString[i + 1] >= 0xdc00 && charString[i + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (charString[i + 1] - 0xdc00);
          ++i;
        } else if ((c & 0x1ff800) == 0xd800) {
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.IsNormalized(System.Int32[],PeterO.Text.Normalization)"]/*'/>
    public static bool IsNormalized(int[] charArray, Normalization form) {
      if (charArray == null) {
  throw new ArgumentNullException("charArray");
}
      return IsNormalized(new PartialArrayCharacterInput(charArray), form);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.IsNormalized(System.Collections.Generic.IList{System.Int32},PeterO.Text.Normalization)"]/*'/>
    public static bool IsNormalized(IList<int> charList, Normalization form) {
      var nonQcsStart = -1;
      var lastQcs = 0;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      if (charList == null) {
        throw new ArgumentNullException("charList");
      }
      for (int i = 0; i < charList.Count; ++i) {
        int c = charList[i];
        if (c < 0 || c > 0x10ffff || ((c & 0x1ff800) == 0xd800)) {
          return false;
        }
        var isQcs = false;
        isQcs = (c & mask) == c && (i + 1 == charList.Count || (charList[i +
               1] & mask) == charList[i + 1]) ? true :
               UnicodeDatabase.IsQuickCheckStarter(c, form);
        if (isQcs) {
          lastQcs = i;
        }
        if (nonQcsStart < 0 && !isQcs) {
          // First non-quick-check starter in a row
          nonQcsStart = lastQcs;
        } else if (nonQcsStart >= 0 && isQcs) {
          // We have at least one non-quick-check starter,
          // normalize these code points.
          if (!NormalizeAndCheck(
        charList,
        nonQcsStart,
        i - nonQcsStart,
        form)) {
            return false;
          }
          nonQcsStart = -1;
        }
      }
      if (nonQcsStart >= 0) {
        if (!NormalizeAndCheck(
  charList,
  nonQcsStart,
  charList.Count - nonQcsStart,
  form)) {
          return false;
        }
      }
      return true;
    }

    private readonly int[] readbuffer = new int[1];

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.ReadChar"]/*'/>
    public int ReadChar() {
      int r = this.Read(this.readbuffer, 0, 1);
      return r == 1 ? this.readbuffer[0] : -1;
    }

    private bool endOfString;
    private int[] lastCharBuffer;
    private int lastCharPos;

    private void PrependOne(int c) {
      if (this.lastCharPos + 1 > this.lastCharBuffer.Length) {
        var newbuffer = new int[this.lastCharPos + 8];
        Array.Copy(this.lastCharBuffer, 0, newbuffer, 0, this.lastCharPos);
        this.lastCharBuffer = newbuffer;
      }
      this.lastCharBuffer[this.lastCharPos++] = c;
    }

    private void PrependTwo(int c1, int c2) {
      if (this.lastCharPos + 2 > this.lastCharBuffer.Length) {
        var newbuffer = new int[this.lastCharPos + 8];
        Array.Copy(this.lastCharBuffer, 0, newbuffer, 0, this.lastCharPos);
        this.lastCharBuffer = newbuffer;
      }
      this.lastCharBuffer[this.lastCharPos++] = c2;
      this.lastCharBuffer[this.lastCharPos++] = c1;
    }

    private int GetNextChar() {
      int ch;
      if (this.lastCharPos > 0) {
        --this.lastCharPos;
        ch = this.lastCharBuffer[this.lastCharPos];
        return ch;
      }
      ch = this.iterator.ReadChar();
      if (ch < 0) {
        this.endOfString = true;
      } else if (ch > 0x10ffff || ((ch & 0x1ff800) == 0xd800)) {
        throw new ArgumentException("Invalid character: " + ch);
      }
      return ch;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.Read(System.Int32[],System.Int32,System.Int32)"]/*'/>
    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new ArgumentNullException("chars");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
            "0");
      }
      if (index > chars.Length) {
        throw new ArgumentException("index (" + index + ") is more than " +
          chars.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length (" + length + ") is less than " +
              "0");
      }
      if (length > chars.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          chars.Length);
      }
      if (chars.Length - index < length) {
        throw new ArgumentException("chars's length minus " + index + " (" +
          (chars.Length - index) + ") is less than " + length);
      }
      if (length == 0) {
        return 0;
      }
      var total = 0;
      var count = 0;
      if (this.processedIndex == this.flushIndex && this.flushIndex == 0) {
        while (total < length) {
          int c = this.GetNextChar();
          if (c < 0) {
            return (total == 0) ? -1 : total;
          }
          if (c < 0x80 || UnicodeDatabase.IsQuickCheckStarter(c, this.form)) {
            if (this.form == Normalization.NFD ||
            this.form == Normalization.NFKD) {
              chars[index] = c;
              ++total;
              ++index;
            } else {
              while (total < length) {
                int c2 = this.GetNextChar();
                if (c2 < 0) {
                  chars[index] = c;
                  ++total;
                  return total;
                } else {
                  this.PrependTwo(c, c2);
                  break;
                }
              }
              break;
            }
          } else {
            this.PrependOne(c);
            break;
          }
        }
        if (total == length) {
          return total;
        }
      }
      do {
        count = Math.Min(this.processedIndex - this.flushIndex, length - total);
        if (count < 0) {
          count = 0;
        }
        if (count != 0) {
          // Fill buffer with processed code points
          Array.Copy(this.buffer, this.flushIndex, chars, index, count);
        }
        index += count;
        total += count;
        this.flushIndex += count;
        bool decompForm = this.form == Normalization.NFD ||
            this.form == Normalization.NFKD;
        // Try to fill buffer with quick-check starters,
        // as an optimization
        while (total < length) {
          int c = this.GetNextChar();
          if (c < 0) {
            this.endOfString = true;
            break;
          }
          if (UnicodeDatabase.IsQuickCheckStarter(c, this.form)) {
            if (decompForm) {
              chars[index] = c;
              ++total;
              ++index;
            } else {
              while (total < length) {
                int c2 = this.GetNextChar();
                if (c2 < 0) {
                  chars[index] = c;
                  ++total;
                  return total;
                } else {
                  this.PrependTwo(c, c2);
                  break;
                }
              }
              break;
            }
          } else {
            this.PrependOne(c);
            break;
          }
        }
        // Ensure that more data is available
        if (total < length && this.flushIndex == this.processedIndex) {
          if (this.lastQcsIndex > 0) {
            // Move unprocessed data to the beginning of
            // the buffer
#if DEBUG
            if (this.endIndex < this.lastQcsIndex) {
              throw new ArgumentException("endIndex less than lastQcsIndex");
            }
#endif
            Array.Copy(
  this.buffer,
  this.lastQcsIndex,
  this.buffer,
  0,
  this.buffer.Length - this.lastQcsIndex);
            // Console.WriteLine("endIndex=" + (this.endIndex));
            this.endIndex -= this.lastQcsIndex;
            this.lastQcsIndex = 0;
          } else {
            this.endIndex = 0;
          }
          if (!this.LoadMoreData()) {
            break;
          }
        }
      } while (total < length);
      // Fill buffer with processed code points
      count = Math.Max(
  0,
  Math.Min(this.processedIndex - this.flushIndex, length - total));
      Array.Copy(this.buffer, this.flushIndex, chars, index, count);
      index += count;
      total += count;
      this.flushIndex += count;
      return (total == 0) ? -1 : total;
    }

    private bool LoadMoreData() {
      var done = false;
      while (!done) {
        this.buffer = this.buffer ?? (new int[32]);
        // Fill buffer with decompositions until the buffer is full
        // or the end of the string is reached.
        while (this.endIndex + 18 <= this.buffer.Length) {
          int c = this.GetNextChar();
          if (c < 0) {
            this.endOfString = true;
            break;
          }
          this.endIndex = DecompToBuffer(
  c,
  this.compatMode,
  this.buffer,
  this.endIndex);
        }
        // Check for the last quick-check starter if the
        // end of the string is not reached yet
        if (!this.endOfString) {
          var haveNewQcs = false;
          // NOTE: lastQcsIndex begins at -1
          bool decompForm = this.form == Normalization.NFD ||
            this.form == Normalization.NFKD;
          var nextIsQCS = false;
          for (int i = this.endIndex - 1; i > this.lastQcsIndex; --i) {
            if (
  UnicodeDatabase.IsQuickCheckStarter(
  this.buffer[i],
  this.form)) {
              if (decompForm) {
                this.lastQcsIndex = i;
                haveNewQcs = true;
                break;
              } else if (i + 1 < this.endIndex && (nextIsQCS ||
         UnicodeDatabase.IsQuickCheckStarter(
  this.buffer[i + 1],
  this.form))) {
                this.lastQcsIndex = i;
                haveNewQcs = true;
                break;
              } else {
                nextIsQCS = true;
              }
            } else {
              nextIsQCS = false;
            }
          }
          if (!haveNewQcs || this.lastQcsIndex <= 0) {
            // No quick-check starter was found (or last quick-check
            // starter is at beginning of buffer), increase
            // the buffer size
            var newBuffer = new int[(this.buffer.Length + 4) * 2];
            Array.Copy(this.buffer, 0, newBuffer, 0, this.buffer.Length);
            this.buffer = newBuffer;
            continue;
          }
        } else {
          // End of string
          this.lastQcsIndex = this.endIndex;
        }
        done = true;
      }
      // No data in buffer
      if (this.endIndex == 0) {
        return false;
      }
      this.flushIndex = 0;
      // Canonical reordering
      ReorderBuffer(this.buffer, 0, this.lastQcsIndex);
      if (this.form == Normalization.NFC || this.form == Normalization.NFKC) {
        // Composition
        this.processedIndex = ComposeBuffer(
  this.buffer,
  this.lastQcsIndex);
      } else {
        this.processedIndex = this.lastQcsIndex;
      }
      return true;
    }

    private sealed class PartialArrayCharacterInput : ICharacterInput {
      private readonly int endPos;
      private readonly int[] array;
      private int pos;

      public PartialArrayCharacterInput(int[] array, int start, int length) {
        this.array = array;
        this.pos = start;
        this.endPos = start + length;
      }

      public PartialArrayCharacterInput(int[] array) {
        this.array = array;
        this.pos = 0;
        this.endPos = array.Length;
      }

      public int ReadChar() {
        return (this.pos < this.endPos) ? this.array[this.pos++] : (-1);
      }

      public int Read(int[] buf, int offset, int unitCount) {
        if (unitCount == 0) {
          return 0;
        }
        int maxsize = Math.Min(unitCount, this.endPos - this.pos);
        Array.Copy(this.array, this.pos, buf, offset, maxsize);
        this.pos += maxsize;
        return maxsize == 0 ? -1 : maxsize;
      }
    }

    private sealed class StringCharacterInput2 : ICharacterInput {
      private readonly string str;
      private readonly int endIndex;
      private int index;

      public StringCharacterInput2(string str) {
        if (str == null) {
          throw new ArgumentNullException("str");
        }
        this.str = str;
        this.endIndex = str.Length;
      }

      public StringCharacterInput2(string str, int index, int length) {
        if (str == null) {
          throw new ArgumentNullException("str");
        }
        if (index < 0) {
          throw new ArgumentException("index (" + index + ") is less than " +
              "0");
        }
        if (index > str.Length) {
          throw new ArgumentException("index (" + index + ") is more than " +
            str.Length);
        }
        if (length < 0) {
          throw new ArgumentException("length (" + length + ") is less than " +
                "0");
        }
        if (length > str.Length) {
          throw new ArgumentException("length (" + length + ") is more than " +
            str.Length);
        }
        if (str.Length - index < length) {
          throw new ArgumentException("str's length minus " + index + " (" +
            (str.Length - index) + ") is less than " + length);
        }
        this.str = str;
        this.index = index;
        this.endIndex = index + length;
      }

      public int ReadChar() {
        if (this.index >= this.endIndex) {
          return -1;
        }
        int c = this.str[this.index];
        if ((c & 0xfc00) == 0xd800 && this.index + 1 < this.str.Length &&
      this.str[this.index + 1] >= 0xdc00 && this.str[this.index + 1] <=
              0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (this.str[this.index + 1] -
              0xdc00);
          ++this.index;
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate, return
          // a number outside the Unicode range
          c = 0x110000;
        }
        ++this.index;
        return c;
      }

      public int Read(int[] chars, int index, int length) {
        if (chars == null) {
          throw new ArgumentNullException("chars");
        }
        if (index < 0) {
          throw new ArgumentException("index (" + index + ") is less than " +
              "0");
        }
        if (index > chars.Length) {
          throw new ArgumentException("index (" + index + ") is more than " +
            chars.Length);
        }
        if (length < 0) {
          throw new ArgumentException("length (" + length + ") is less than " +
                "0");
        }
        if (length > chars.Length) {
          throw new ArgumentException("length (" + length + ") is more than " +
            chars.Length);
        }
        if (chars.Length - index < length) {
          throw new ArgumentException("chars's length minus " + index + " (" +
            (chars.Length - index) + ") is less than " + length);
        }
        if (this.endIndex == this.index) {
          return -1;
        }
        if (length == 0) {
          return 0;
        }
        for (int i = 0; i < length; ++i) {
          int c = this.ReadChar();
          if (c == -1) {
            return (i == 0) ? -1 : i;
          }
          chars[index + i] = c;
        }
        return length;
      }
    }
  }
}
