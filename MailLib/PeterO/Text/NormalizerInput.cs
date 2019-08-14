/*
  Written by Peter O. in 2014.
  Any copyright is dedicated to the Public Domain.
  http://creativecommons.org/publicdomain/zero/1.0/
  If you like this, you should donate to Peter O.
  at: http://peteroupc.github.io/
   */
using System;

namespace PeterO.Text {
    /// <summary>
    /// <para>A character input class that implements the Unicode
    /// normalization algorithm and contains methods and functionality to
    /// test and convert text strings for normalization. This is similar to
    /// the deprecated Normalizer class, except it implements the
    /// ICharacterInput interface.</para>
    /// <para>The Unicode Standard includes characters, such as an acute
    /// accent, that can be combined with other characters to make new
    /// characters. For example, the letter E combines with an acute accent
    /// to make E-acute (É). In some cases, the combined form (E-acute)
    /// should be treated as equivalent to the uncombined form (E plus
    /// acute). For this reason, the standard defines four
    /// <i>normalization forms</i> that convert strings to a single
    /// equivalent form:</para>
    /// <list>
    /// <item><b>NFD</b> (Normalization Form D) decomposes combined forms
    /// to their constituent characters (E plus acute, for example), then
    /// reorders combining marks to a standardized order. This is called
    /// canonical decomposition.</item>
    /// <item><b>NFC</b> does canonical decomposition, then combines
    /// certain constituent characters to their composites (E-acute, for
    /// example). This is called canonical composition.</item>
    /// <item>Two normalization forms, <b>NFKC</b> and <b>NFKD</b>, are
    /// similar to NFC and NFD, except they also "decompose" certain
    /// characters, such as ligatures, font or positional variants, and
    /// subscripts, whose visual distinction can matter in some contexts.
    /// This is called compatibility decomposition.</item></list>
    /// <para>For more information, see Standard Annex 15 at
    /// <c>http://www.unicode.org/reports/tr15/</c>.</para>
    /// <para><b>Thread safety:</b> This class is mutable; its properties
    /// can be changed. None of its instance methods are designed to be
    /// thread safe. Therefore, access to objects from this class must be
    /// synchronized if multiple threads can access them at the same
    /// time.</para>
    /// <para>NOTICE: While this class's source code is in the public
    /// domain, the class uses an internal class, called NormalizationData,
    /// that includes data derived from the Unicode Character Database. In
    /// case doing so is required, the permission notice for the Unicode
    /// Character Database is given here:</para>
    /// <para>COPYRIGHT AND PERMISSION NOTICE</para>
    /// <para>Copyright (c) 1991-2014 Unicode, Inc. All rights reserved.
    /// Distributed under the Terms of Use in
    /// http://www.unicode.org/copyright.html.</para>
    /// <para>Permission is hereby granted, free of charge, to any person
    /// obtaining a copy of the Unicode data files and any associated
    /// documentation (the "Data Files") or Unicode software and any
    /// associated documentation (the "Software") to deal in the Data Files
    /// or Software without restriction, including without limitation the
    /// rights to use, copy, modify, merge, publish, distribute, and/or
    /// sell copies of the Data Files or Software, and to permit persons to
    /// whom the Data Files or Software are furnished to do so, provided
    /// that (a) this copyright and permission notice appear with all
    /// copies of the Data Files or Software, (b) this copyright and
    /// permission notice appear in associated documentation, and (c) there
    /// is clear notice in each modified Data File or in the Software as
    /// well as in the documentation associated with the Data File(s) or
    /// Software that the data or software has been modified.</para>
    /// <para>THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT
    /// WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    /// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
    /// PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS. IN NO EVENT
    /// SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE
    /// LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL
    /// DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA
    /// OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
    /// TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
    /// PERFORMANCE OF THE DATA FILES OR SOFTWARE.</para>
    /// <para>Except as contained in this notice, the name of a copyright
    /// holder shall not be used in advertising or otherwise to promote the
    /// sale, use or other dealings in these Data Files or Software without
    /// prior written authorization of the copyright
    /// holder.</para></summary>
  public sealed class NormalizerInput : ICharacterInput {
    internal static int DecompToBufferInternal(
      int ch,
      bool compat,
      int[] buffer,
      int index) {
#if DEBUG
      if (buffer == null) {
        throw new ArgumentNullException(nameof(buffer));
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
    private static bool IsDecompositionForm(Normalization form) {
      return form == Normalization.NFD || form == Normalization.NFKD;
   }

    internal static int DecompToBuffer(
      int ch,
      bool compat,
      int[] buffer,
      int index) {
#if DEBUG
      if (buffer == null) {
        throw new ArgumentNullException(nameof(buffer));
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
      // DebugUtility.Log ("buf=" + (EC(array,0,length)));
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
          if (lead >= 0 && lead < 19) {
            // Found Hangul L jamo
            int vowel = ch - 0x1161;
            if (vowel >= 0 && vowel < 21 && (last < valuecc || last == 0)) {
              starter = 0xac00 + (((lead * 21) + vowel) * 28);
              array[starterPos] = starter;
              array[decompPos] = 0x110000;
              composed = true;
              --retval;
              continue;
            }
          }
          int syllable = starter - 0xac00;
          if (syllable >= 0 && syllable < 11172 && (syllable % 28) == 0) {
            // Found Hangul LV jamo
            int trail = ch - 0x11a7;
            if (trail > 0 && trail < 28 && (last < valuecc || last == 0)) {
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
      // DebugUtility.Log ("bufend=" + (EC (array, 0, length)));
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

    private readonly bool compatMode;
    private readonly Normalization form;
    private readonly ICharacterInput iterator;
    private int lastQcsIndex;
    private int endIndex;
    private int[] buffer;
    private int processedIndex;
    private int flushIndex;

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizerInput'/> class using Normalization
    /// Form C.</summary>
    /// <param name='str'>A string specifying the text to
    /// normalize.</param>
    public NormalizerInput(
  string str) : this(
  str,
  Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizerInput'/> class using Normalization
    /// Form C.</summary>
    /// <param name='input'>The parameter <paramref name='input'/> is an
    /// ICharacterInput object.</param>
    public NormalizerInput(
  ICharacterInput input) : this(
  input,
  Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizerInput'/> class. Uses a portion of
    /// a string as the input.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='index'>A zero-based index showing where the desired
    /// portion of <paramref name='str'/> begins.</param>
    /// <param name='length'>The number of elements in the desired portion
    /// of <paramref name='str'/> (but not more than <paramref name='str'/>
    /// 's length).</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    public NormalizerInput(
      string str,
      int index,
      int length,
      Normalization form) : this(
  new StringCharacterInput2(str, index, length),
  form) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizerInput'/> class.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    public NormalizerInput(string str, Normalization form)
      : this(new StringCharacterInput2(str), form) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizerInput'/> class.</summary>
    /// <param name='stream'>The parameter <paramref name='stream'/> is an
    /// ICharacterInput object.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    public NormalizerInput(
      ICharacterInput stream,
      Normalization form) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      this.lastQcsIndex = -1;
      this.iterator = stream;
      this.form = form;
      this.readbuffer = new int[1];
      this.lastCharBuffer = new int[2];
      this.compatMode = form == Normalization.NFKC || form ==
          Normalization.NFKD;
    }

    /// <summary>Determines whether the text provided by a character input
    /// is normalized.</summary>
    /// <param name='chars'>A object that implements a streamable character
    /// input.</param>
    /// <param name='form'>Specifies the normalization form to
    /// check.</param>
    /// <returns><c>true</c> if the text is normalized; otherwise,
    /// <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    public static bool IsNormalized(
      ICharacterInput chars,
      Normalization form) {
      if (chars == null) {
        throw new ArgumentNullException(nameof(chars));
      }
      var listIndex = 0;
      var array = new int[16];
      var haveNonQcs = false;
      while (true) {
        int c = chars.ReadChar();
        if (c < 0) {
          break;
        }
        if ((c & 0x1ff800) == 0xd800) {
          return false;
        }
        bool isQcs = (c >= 0xf0000) ? true :
UnicodeDatabase.IsQuickCheckStarter(
  c,
  form);

        if (isQcs) {
          if (haveNonQcs) {
            if (!NormalizeAndCheck(
              array,
              0,
              listIndex,
              form)) {
              return false;
            }
          }
          listIndex = 0;
          haveNonQcs = false;
        } else {
          haveNonQcs = true;
        }
        if (listIndex >= array.Length) {
          var newArray = new int[array.Length * 2];
          Array.Copy(array, 0, newArray, 0, listIndex);
          array = newArray;
        }
        array[listIndex++] = c;
      }
      if (haveNonQcs) {
        if (!NormalizeAndCheck(
          array,
          0,
          listIndex,
          form)) {
          return false;
        }
      }
      return true;
    }

    private static bool NormalizeAndCheck(
      int[] charArray,
      int start,
      int length,
      Normalization form) {
      var i = 0;
      int ch;
      var input = new NormalizerInput(
  new PartialArrayCharacterInput(charArray, start, length),
  form);
      while ((ch = input.ReadChar()) >= 0) {
        if (i >= length) {
          return false;
        }
        if (ch != charArray[start + i]) {
          return false;
        }
        ++i;
      }
      return i == length;
    }

    /// <summary>Converts a string to the given Unicode normalization
    /// form.</summary>
    /// <param name='str'>An arbitrary string.</param>
    /// <param name='form'>The Unicode normalization form to convert
    /// to.</param>
    /// <returns>The parameter <paramref name='str'/> converted to the
    /// given normalization form.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='str'/> contains an unpaired surrogate code point.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static string Normalize(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length <= 1024 && IsNormalized(str, form)) {
        return str;
      }
      return Encodings.InputToString(
        new NormalizerInput(str, form));
    }

    /// <summary>Determines whether the given string is in the given
    /// Unicode normalization form.</summary>
    /// <param name='str'>An arbitrary string.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    /// <returns><c>true</c> if the given string is in the given Unicode
    /// normalization form; otherwise, <c>false</c>. Returns <c>false</c>
    /// if the string contains an unpaired surrogate code point.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static bool IsNormalized(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      // DebugUtility.Log (str);
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      var lastQcsIndex = 0;
      var haveNonQcs = false;
      for (int i = 0; i < str.Length; ++i) {
        int c = str[i];
        if ((c & 0xfc00) == 0xd800 && i + 1 < str.Length &&
            (str[i + 1] & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (str[i + 1] & 0x3ff);
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
          if (haveNonQcs) {
            if (!NormalizeAndCheckString(
              str,
              lastQcsIndex,
              i - lastQcsIndex,
              form)) {
              return false;
            }
          }
          lastQcsIndex = i;
          haveNonQcs = false;
        } else {
          haveNonQcs = true;
        }
        // DebugUtility.Log ("ch=" + (// EC (c)) + " qcs=" + isQcs + " lastqcs="
        // + lastQcs + " nqs=" + nonQcsStart);
        if (c >= 0x10000) {
          ++i;
        }
      }
      if (haveNonQcs) {
        if (!NormalizeAndCheckString(
          str,
          lastQcsIndex,
          str.Length - lastQcsIndex,
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
      var norm = new NormalizerInput(
        charString,
        start,
        length,
        form);
      var ch = 0;
      int endIndex = start + length;
      while ((ch = norm.ReadChar()) >= 0) {
        int c = charString[i];
        if ((c & 0x1ffc00) == 0xd800 && i + 1 < endIndex &&
            (charString[i + 1] & 0xfc00) == 0xdc00) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (charString[i + 1] & 0x3ff);
          ++i;
        } else if ((c & 0x1ff800) == 0xd800) {
          // unpaired surrogate
          return false;
        }
        ++i;
        if (c != ch) {
          return false;
        }
      }
      return i == endIndex;
    }

    private readonly int[] readbuffer;

    /// <summary>Reads a Unicode character from a data source.</summary>
    /// <returns>Either a Unicode code point (from 0-0xd7ff or from 0xe000
    /// to 0x10ffff), or the value -1 indicating the end of the
    /// source.</returns>
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

    /* private static string EC (int c) {
              if (c < 0) {
                  return "<" + c + ">";
              }
      string uesc = "\\u";
              if (c >= 0x10000) {
                  return String.Format (uesc + "{0:X8}", c);
              }
        return (c >= 0x7f || c<0x20) ? (String.Format (uesc + "{0:X4}", c)):
          ((c == 0x20) ? ("<space>") : ("<" + ((char)c) + ">"));
          }

      private static string EC (int[] b, int o, int sz) {
        var sb = new System.Text.StringBuilder();
        for (var i = 0; i < sz; ++i) {
          sb.Append (EC (b [i + o]));
        }
        return sb.ToString();
      }

    */

    /// <summary>Reads a sequence of Unicode code points from a data
    /// source.</summary>
    /// <param name='chars'>Output buffer.</param>
    /// <param name='index'>A zero-based index showing where the desired
    /// portion of <paramref name='chars'/> begins.</param>
    /// <param name='length'>The number of elements in the desired portion
    /// of <paramref name='chars'/> (but not more than <paramref
    /// name='chars'/> 's length).</param>
    /// <returns>The number of Unicode code points read, or 0 if the end of
    /// the source is reached.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref name='index'/>
    /// or <paramref name='length'/> is less than 0 or greater than
    /// <paramref name='chars'/> 's length, or <paramref name='chars'/> ' s
    /// length minus <paramref name='index'/> is less than <paramref
    /// name='length'/>.</exception>
    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new ArgumentNullException(nameof(chars));
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
          // DebugUtility.Log ("read: " + (EC(c)));
          if (c < 0) {
            return (total == 0) ? -1 : total;
          }
          if (c < 0x80 || UnicodeDatabase.IsQuickCheckStarter(c, this.form)) {
            if (IsDecompositionForm(this.form)) {
              chars[index] = c;
              ++total;
              ++index;
            } else {
              while (total < length) {
                int c2 = this.GetNextChar();
                // DebugUtility.Log ("read in qcs: " + (EC (c)));
                if (c2 < 0) {
                  chars[index] = c;
                  ++total;
                  return total;
                } else {
                  this.PrependTwo(c, c2);
                  // DebugUtility.Log ("prepending: " + (EC (c)) + ", " + (EC
                  // (c2)));
                  break;
                }
              }
              break;
            }
          } else {
            this.PrependOne(c);
            // DebugUtility.Log ("prepending: " + (EC (c)));
            break;
          }
        }
        if (total == length) {
          return total;
        }
      }
      do {
        count = Math.Min(
          this.processedIndex - this.flushIndex,
          length - total);
        if (count < 0) {
          count = 0;
        }
        // DebugUtility.Log ("B count=" + count);
        if (count != 0) {
          // Fill buffer with processed code points
          Array.Copy(this.buffer, this.flushIndex, chars, index, count);
        }
        index += count;
        total += count;
        this.flushIndex += count;
        bool decompForm = IsDecompositionForm(this.form);
        // Try to fill buffer with quick-check starters,
        // as an optimization.
        // There is a check for processedIndex == endIndex
        // because currently, this loop may read ahead
        // and add characters to the output
        // before all the characters in the intermediate
        // buffer have been fully processed
        while (total < length && this.processedIndex == this.endIndex) {
          int c = this.GetNextChar();
          // DebugUtility.Log ("B read: " + (EC(c)));
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
                // DebugUtility.Log ("B read in qcs: " + (EC(c)));
                if (c2 < 0) {
                  chars[index] = c;
                  ++total;
                  return total;
                } else {
                  this.PrependTwo(c, c2);
                  // DebugUtility.Log ("B prepending: " + (EC(c)) + ", " +
                  // (// EC(c2)));
                  break;
                }
              }
              break;
            }
          } else {
            this.PrependOne(c);
            // DebugUtility.Log ("B prepending: " + (EC(c)));
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
          // DebugUtility.Log ("C read: " + (EC(c)));
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
          bool decompForm = IsDecompositionForm(this.form);
          var nextIsQCS = false;
          for (int i = this.endIndex - 1; i > this.lastQcsIndex; --i) {
            if (
  UnicodeDatabase.IsQuickCheckStarter(
    this.buffer[i],
    this.form)) {
              // DebugUtility.Log ("" + (EC (buffer [i])) + " is qcs");
              if (decompForm) {
                this.lastQcsIndex = i;
                haveNewQcs = true;
                break;
              } else if (i + 1 < this.endIndex && (nextIsQCS ||
         UnicodeDatabase.IsQuickCheckStarter(
           this.buffer[i + 1],
           this.form))) {
                // DebugUtility.Log ("" + (EC (buffer [i +
                // 1])) + " (next) is qcs");
                this.lastQcsIndex = i;
                haveNewQcs = true;
                break;
              } else {
                nextIsQCS = true;
              }
            } else {
              // DebugUtility.Log ("" + (EC (buffer [i])) + " is not qcs");
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
        // DebugUtility.Log ("no data");
        return false;
      }
      this.flushIndex = 0;
       // DebugUtility.Log ("reordering " + // (EC (buffer, 0, lastQcsIndex)) +
      // " [" + this.form + "]");
      // Canonical reordering
      ReorderBuffer(this.buffer, 0, this.lastQcsIndex);
      if (!IsDecompositionForm(this.form)) {
        // Composition
        // DebugUtility.Log ("composing " + (// EC (buffer, 0, lastQcsIndex)) +
        // " [" + this.form + "]");
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
          throw new ArgumentNullException(nameof(str));
        }
        this.str = str;
        this.endIndex = str.Length;
      }

      public StringCharacterInput2(string str, int index, int length) {
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
        if (length < 0) {
          throw new ArgumentException("length (" + length +
              ") is less than " + "0");
        }
        if (length > str.Length) {
          throw new ArgumentException("length (" + length +
              ") is more than " + str.Length);
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
        if ((c & 0xfc00) == 0xd800 && this.index + 1 < this.endIndex &&
      this.str[this.index + 1] >= 0xdc00 && this.str[this.index + 1] <=
              0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c & 0x3ff) << 10) + (this.str[this.index + 1] &
0x3ff);
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
          throw new ArgumentNullException(nameof(chars));
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
          throw new ArgumentException("length (" + length +
              ") is less than " + "0");
        }
        if (length > chars.Length) {
          throw new ArgumentException("length (" + length +
              ") is more than " + chars.Length);
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
