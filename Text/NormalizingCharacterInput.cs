/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;

using PeterO.Text.Encoders;

namespace PeterO.Text {
    /// <summary><para>A character input class that implements the Unicode
    /// normalization algorithm and contains methods and functionality to test
    /// and
    /// convert Unicode strings for Unicode normalization. This is similar to
    /// the
    /// Normalizer class, except it implements the ICharacterInput
    /// interface.</para>
    /// <para>NOTICE: While this class's source code is in the public domain,
    /// the
    /// class uses an internal class, called NormalizationData, that includes
    /// data
    /// derived from the Unicode Character Database. In case doing so is
    /// required,
    /// the permission notice for the Unicode Character Database is given
    /// here:</para>
    /// <para>COPYRIGHT AND PERMISSION NOTICE</para>
    /// <para>Copyright
    /// (c) 1991-2014 Unicode, Inc. All rights reserved. Distributed under the
    /// Terms
    /// of Use in http://www.unicode.org/copyright.html.</para>
    /// <para>Permission is
    /// hereby granted, free of charge, to any person obtaining a copy of the
    /// Unicode data files and any associated documentation (the "Data Files")
    /// or
    /// Unicode software and any associated documentation (the "Software") to
    /// deal
    /// in the Data Files or Software without restriction, including without
    /// limitation the rights to use, copy, modify, merge, publish, distribute,
    /// and/or sell copies of the Data Files or Software, and to permit persons
    /// to
    /// whom the Data Files or Software are furnished to do so, provided that
    /// (a)
    /// this copyright and permission notice appear with all copies of the Data
    /// Files or Software, (b) this copyright and permission notice appear in
    /// associated documentation, and (c) there is clear notice in each modified
    /// Data File or in the Software as well as in the documentation associated
    /// with
    /// the Data File(s) or Software that the data or software has been
    /// modified.</para>
    /// <para>THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS",
    /// WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
    /// LIMITED
    /// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE
    /// AND
    /// NONINFRINGEMENT OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT
    /// HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR
    /// ANY
    /// SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER
    /// RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF
    /// CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
    /// CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA FILES OR
    /// SOFTWARE.</para>
    /// <para>Except as contained in this notice, the name of a copyright holder
    /// shall not be used in advertising or otherwise to promote the sale, use
    /// or
    /// other dealings in these Data Files or Software without prior written
    /// authorization of the copyright holder.</para>
    /// </summary>
  public sealed class NormalizingCharacterInput : ICharacterInput
  {
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
        int valueSIndex = ch - 0xac00;
        int trail = 0x11a7 + (valueSIndex % 28);
        buffer[index++] = 0x1100 + (valueSIndex / 588);
        buffer[index++] = 0x1161 + ((valueSIndex % 588) / 28);
        if (trail != 0x11a7) {
          buffer[index++] = trail;
        }
        return index;
      }
      return DecompToBufferInternal(ch, compat, buffer, index);
    }

    internal static bool IsStableCodePoint(int cp, Normalization form) {
      // Exclude YOD and HIRIQ because of Corrigendum 2
      return UnicodeDatabase.IsStableCodePoint(cp, form) && cp != 0x5b4 &&
        cp != 0x5d9;
    }

    internal static void ReorderBuffer(int[] buffer, int index, int length) {
      int i;
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
      if (length < 0) {
        throw new ArgumentException("length (" + length + ") is less than " +
              "0");
      }
      if (length > buffer.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          buffer.Length);
      }
      if (buffer.Length - index < length) {
        throw new ArgumentException("buffer's length minus " + index + " (" +
          (buffer.Length - index) + ") is less than " + length);
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
            // Console.WriteLine("lead= {0:X4} ccc=" + lead);
            // Console.WriteLine("trail={0:X4} ccc=" + trail);
            // Console.WriteLine("now "+ToString(buffer,index,length));
            changed = true;
            // Lead is now at trail's position
          } else {
            lead = trail;
          }
        }
      } while (changed);
    }

    internal static int ComposeBuffer(int[] array, int length) {
#if DEBUG
      if (array == null) {
        throw new ArgumentNullException("array");
      }
      if (length < 0) {
        throw new ArgumentException("length (" + length + ") is less than " +
              "0");
      }
      if (length > array.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          array.Length);
      }
      if (array.Length < length) {
        throw new ArgumentException("array's length (" + array.Length +
          ") is less than " + length);
      }
#endif

      if (length < 2) {
        return length;
      }
      int starterPos = 0;
      int retval = length;
      int starter = array[0];
      int last = UnicodeDatabase.GetCombiningClass(starter);
      if (last != 0) {
        last = 256;
      }
      int endPos = 0 + length;
      bool composed = false;
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
        int j = 0;
        for (int i = 0; i < endPos; ++i) {
          if (array[i] != 0x110000) {
            array[j++] = array[i];
          }
        }
      }
      return retval;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <returns>A list of Unicode characters.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str' />
    /// is null.</exception>
    public static IList<int> GetChars(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return GetChars(new StringCharacterInput(str), form);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>An ICharacterInput object.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <returns>A list of Unicode characters.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str' />
    /// is null.</exception>
    public static IList<int> GetChars(ICharacterInput str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      var norm = new NormalizingCharacterInput(str, form);
      var buffer = new int[64];
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
    /// class
    /// using Normalization Form C.</summary>
    /// <param name='characterList'>An IList object.</param>
    public NormalizingCharacterInput(IList<int> characterList) :
      this(characterList, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class
    /// using Normalization Form C.</summary>
    /// <param name='str'>A string object.</param>
  public NormalizingCharacterInput(
string str) : this(
str,
Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class
    /// using Normalization Form C.</summary>
    /// <param name='input'>An ICharacterInput object.</param>
    public NormalizingCharacterInput(
ICharacterInput input) : this(
input,
Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class
    /// using the given normalization form.</summary>
    /// <param name='characterList'>An IList object.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='characterList'/> is null.</exception>
    public NormalizingCharacterInput(
IList<int> characterList,
Normalization form) {
      if (characterList == null) {
        throw new ArgumentNullException("characterList");
      }
      this.lastStableIndex = -1;
      this.characterList = characterList;
      this.form = form;
    this.compatMode = form == Normalization.NFKC || form ==
        Normalization.NFKD;
    }

    /// <summary>Initializes a new instance of the NormalizingCharacterInput
    /// class.
    /// Uses a portion of a string as the input.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='length'>A 32-bit signed integer. (2).</param>
    /// <param name='form'>A Normalization object.</param>
    public NormalizingCharacterInput(
string str,
int index,
int length,
Normalization form) : this(
new StringCharacterInput(str, index, length),
form) {
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
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
 public NormalizingCharacterInput(
ICharacterInput stream,
Normalization form) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.lastStableIndex = -1;
      this.iterator = stream;
      this.form = form;
    this.compatMode = form == Normalization.NFKC || form ==
        Normalization.NFKD;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='chars'>An ICharacterInput object.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <returns>A Boolean object.</returns>
    public static bool IsNormalized(ICharacterInput chars, Normalization form) {
      if (chars == null) {
        return false;
      }

      IList<int> list = new List<int>();
      int ch = 0;
      while ((ch = chars.ReadChar()) >= 0) {
        if ((ch & 0x1ff800) == 0xd800) {
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

    /// <summary>Converts a string to the given Unicode normalization
    /// form.</summary>
    /// <param name='str'>An arbitrary string.</param>
    /// <param name='form'>The Unicode normalization form to convert to.</param>
    /// <returns>The parameter <paramref name='str'/> converted to the given
    /// normalization form.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str' />
    /// is null.</exception>
    public static string Normalize(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (str.Length <= 1024 && IsNormalized(str, form)) {
        return str;
      }
      return EncoderHelper.InputToString(
        new NormalizingCharacterInput(str, form));
    }

    /// <summary>Determines whether the given string is in the given Unicode
    /// normalization form.</summary>
    /// <param name='str'>An arbitrary string.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <returns>True if the given string is in the given Unicode normalization
    /// form; otherwise, false.</returns>
    public static bool IsNormalized(string str, Normalization form) {
      if (str == null) {
        return false;
      }
      int nonStableStart = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
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
        if ((c & mask) == c && (i + 1 == str.Length || (str[i + 1] & mask)
          == str[i + 1])) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC) or the end of string.
          // Treat the first character as stable
          // in this situation.
          isStable = true;
        } else {
          isStable = NormalizingCharacterInput.IsStableCodePoint(c, form);
        }
        if (nonStableStart < 0 && !isStable) {
          // First non-stable code point in a row
          nonStableStart = i;
        } else if (nonStableStart >= 0 && isStable) {
          // We have at least one non-stable code point,
          // normalize these code points.
          if (!NormalizeAndCheckString(
         str,
         nonStableStart,
         i - nonStableStart,
         form)) {
            return false;
          }
          nonStableStart = -1;
        }
        if (c >= 0x10000) {
          ++i;
        }
      }
      if (nonStableStart >= 0) {
        if (!NormalizeAndCheckString(
str,
nonStableStart,
str.Length - nonStableStart,
form)) {
          return false;
        }
      }
      return true;
    }

    private static bool NormalizeAndCheckString(
      string charList,
      int start,
      int length,
      Normalization form) {
      int i = start;
      var norm = new NormalizingCharacterInput(
     charList,
     start,
     length,
     form);
      int ch = 0;
      while ((ch = norm.ReadChar()) >= 0) {
        int c = charList[i];
        if ((c & 0x1ffc00) == 0xd800 && i + 1 < charList.Length &&
            charList[i + 1] >= 0xdc00 && charList[i + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (charList[i + 1] - 0xdc00);
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

    /// <summary>Determines whether the given list of characters is in the given
    /// Unicode normalization form.</summary>
    /// <param name='charList'>An IList object.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <returns>True if the given list of characters is in the given Unicode
    /// normalization form; otherwise, false.</returns>
    /// <exception cref='ArgumentNullException'>The parameter "chars" is
    /// null.</exception>
    public static bool IsNormalized(IList<int> charList, Normalization form) {
      int nonStableStart = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      if (charList == null) {
        throw new ArgumentNullException("charList");
      }
      for (int i = 0; i < charList.Count; ++i) {
        int c = charList[i];
        if (c < 0 || c > 0x10ffff || ((c & 0x1ff800) == 0xd800)) {
          return false;
        }
        bool isStable = false;
        if ((c & mask) == c && (i + 1 == charList.Count || (charList[i + 1 ]&
          mask) == charList[i + 1])) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC) or the end of string.
          // Treat the first character as stable
          // in this situation.
          isStable = true;
        } else {
          isStable = IsStableCodePoint(c, form);
        }
        if (nonStableStart < 0 && !isStable) {
          // First non-stable code point in a row
          nonStableStart = i;
        } else if (nonStableStart >= 0 && isStable) {
          // We have at least one non-stable code point,
          // normalize these code points.
  if (!NormalizeAndCheck(
charList,
nonStableStart,
i - nonStableStart,
form)) {
            return false;
          }
          nonStableStart = -1;
        }
      }
      if (nonStableStart >= 0) {
        if (!NormalizeAndCheck(
charList,
nonStableStart,
charList.Count - nonStableStart,
form)) {
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

    private bool endOfString;
    private int lastChar = -1;
    private bool ungetting;

    private void Unget() {
      this.ungetting = true;
    }

    private int GetNextChar() {
      int ch;
      if (this.ungetting) {
        ch = this.lastChar;
        this.ungetting = false;
        return ch;
      }
      ch = (this.iterator == null) ? ((this.characterListPos >=
this.characterList.Count) ? -1 :
          this.characterList[this.characterListPos++]) :
        this.iterator.ReadChar();
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
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> or "this.buffer" is null.</exception>
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
      int total = 0;
      int count = 0;
      if (this.processedIndex == this.flushIndex && this.flushIndex == 0) {
        while (total < length) {
          int c = this.GetNextChar();
          if (c < 0) {
            return (total == 0) ? -1 : total;
          }
          if (IsStableCodePoint(c, this.form)) {
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
        // Console.WriteLine("indexes=" + this.processedIndex + " " +
        // this.flushIndex + ", length=" + length + " total=" + total);
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
            Array.Copy(
this.buffer,
this.lastStableIndex,
this.buffer,
0,
this.buffer.Length - this.lastStableIndex);
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
      bool done = false;
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
        // Check for the last stable code point if the
        // end of the string is not reached yet
        if (!this.endOfString) {
          bool haveNewStable = false;
          // NOTE: lastStableIndex begins at -1
          for (int i = this.endIndex - 1; i > this.lastStableIndex; --i) {
            // Console.WriteLine("stable({0:X4})=" +
            // (IsStableCodePoint(this.buffer[i], this.form)));
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
            var newBuffer = new int[(this.buffer.Length + 4) * 2];
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
      ReorderBuffer(this.buffer, 0, this.lastStableIndex);
      if (this.form == Normalization.NFC || this.form == Normalization.NFKC) {
        // Composition
        this.processedIndex = ComposeBuffer(
this.buffer,
this.lastStableIndex);
      } else {
        this.processedIndex = this.lastStableIndex;
      }
      return true;
    }
  }
}
