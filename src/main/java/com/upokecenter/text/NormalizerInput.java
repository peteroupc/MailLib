package com.upokecenter.text;
/*
  Written by Peter O. in 2014.
  Any copyright is dedicated to the Public Domain.
  http://creativecommons.org/publicdomain/zero/1.0/
  If you like this, you should donate to Peter O.
  at: http://peteroupc.github.io/
   */

    /**
     * <p>A character input class that implements the Unicode normalization
     * algorithm and contains methods and functionality to test and convert
     * text strings for normalization. This is similar to the deprecated
     * Normalizer class, except it implements the ICharacterInput
     * interface.</p> <p>The Unicode Standard includes characters, such as
     * an acute accent, that can be combined with other characters to make
     * new characters. For example, the letter E combines with an acute
     * accent to make E-acute (&#xc9;). In some cases, the combined form
     * (E-acute) should be treated as equivalent to the uncombined form (E
     * plus acute). For this reason, the standard defines four
     * <i>normalization forms</i> that convert strings to a single
     * equivalent form:</p> <ul> <li><b>NFD</b> (Normalization Form D)
     * decomposes combined forms to their constituent characters (E plus
     * acute, for example). This is called canonical decomposition.</li>
     * <li><b>NFC</b> does canonical decomposition, then combines certain
     * constituent characters to their composites (E-acute, for example).
     * This is called canonical composition.</li> <li>Two normalization
     * forms, <b>NFKC</b> and <b>NFKD</b>, are similar to NFC and NFD,
     * except they also "decompose" certain characters, such as ligatures,
     * font or positional variants, and subscripts, whose visual distinction
     * can matter in some contexts. This is called compatibility
     * decomposition.</li> <li>The four normalization forms also enforce a
     * standardized order for combining marks, since they can otherwise
     * appear in an arbitrary order.</li></ul> <p>For more information, see
     * Standard Annex 15 at <code>http://www.unicode.org/reports/tr15/</code>.</p>
     * <p><b>Thread safety:</b> This class is mutable; its properties can be
     * changed. None of its instance methods are designed to be thread safe.
     * Therefore, access to objects from this class must be synchronized if
     * multiple threads can access them at the same time.</p> <p>NOTICE:
     * While this class's source code is in the public domain, the class
     * uses an class, called NormalizationData, that includes data
     * derived from the Unicode Character Database. In case doing so is
     * required, the permission notice for the Unicode Character Database is
     * given here:</p> <p>COPYRIGHT AND PERMISSION NOTICE</p> <p>Copyright
     * (c) 1991-2014 Unicode, Inc. All rights reserved. Distributed under
     * the Terms of Use in http://www.unicode.org/copyright.html.</p>
     * <p>Permission is hereby granted, free of charge, to any person
     * obtaining a copy of the Unicode data files and any associated
     * documentation (the "Data Files") or Unicode software and any
     * associated documentation (the "Software") to deal in the Data Files
     * or Software without restriction, including without limitation the
     * rights to use, copy, modify, merge, publish, distribute, and/or sell
     * copies of the Data Files or Software, and to permit persons to whom
     * the Data Files or Software are furnished to do so, provided that (a)
     * this copyright and permission notice appear with all copies of the
     * Data Files or Software, (b) this copyright and permission notice
     * appear in associated documentation, and (c) there is clear notice in
     * each modified Data File or in the Software as well as in the
     * documentation associated with the Data File(s) or Software that the
     * data or software has been modified.</p> <p>THE DATA FILES AND
     * SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
     * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
     * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
     * OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
     * HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY
     * SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER
     * RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF
     * CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
     * CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA FILES OR
     * SOFTWARE.</p> <p>Except as contained in this notice, the name of a
     * copyright holder shall not be used in advertising or otherwise to
     * promote the sale, use or other dealings in these Data Files or
     * Software without prior written authorization of the copyright
     * holder.</p>
     */
  public final class NormalizerInput implements ICharacterInput {
    static int DecompToBufferInternal(
  int ch,
  boolean compat,
  int[] buffer,
  int index) {
      int offset = UnicodeDatabase.GetDecomposition(
      ch,
      compat,
      buffer,
      index);
      if (buffer[index] != ch) {
        int[] copy = new int[offset - index];
        System.arraycopy(buffer, index, copy, 0, copy.length);
        offset = index;
        for (int i = 0; i < copy.length; ++i) {
          offset = DecompToBufferInternal(copy[i], compat, buffer, offset);
        }
      }
      return offset;
    }

    static int DecompToBuffer(
  int ch,
  boolean compat,
  int[] buffer,
  int index) {
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

    static void ReorderBuffer(int[] buffer, int index, int length) {
      int i;
      if (length < 2) {
        return;
      }
      boolean changed;
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

    static int ComposeBuffer(int[] array, int length) {
      if (length < 2) {
        return length;
      }
      int starterPos = 0;
      int retval = length;
      // DebugUtility.Log ("buf=" + (EC(array,0,length)));
      int starter = array[0];
      int last = UnicodeDatabase.GetCombiningClass(starter);
      if (last != 0) {
        last = 256;
      }
      int endPos = 0 + length;
      boolean composed = false;
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
        boolean diffClass = last < valuecc;
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
        int j = 0;
        for (int i = 0; i < endPos; ++i) {
          if (array[i] != 0x110000) {
            array[j++] = array[i];
          }
        }
      }
      return retval;
    }

    private int lastQcsIndex;
    private int endIndex;
    private int[] buffer;
    private final boolean compatMode;
    private final Normalization form;
    private int processedIndex;
    private int flushIndex;
    private final ICharacterInput iterator;

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizerInput} class using Normalization Form
     * C.
     * @param str A string specifying the text to normalize.
     */
    public NormalizerInput(
  String str) {
 this(
  str, Normalization.NFC);
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizerInput} class using Normalization Form
     * C.
     * @param input The parameter {@code input} is an ICharacterInput object.
     */
    public NormalizerInput(
  ICharacterInput input) {
 this(
  input, Normalization.NFC);
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizerInput} class. Uses a portion of a
     * string as the input.
     * @param str The parameter {@code str} is a text string.
     * @param index A zero-based index showing where the desired portion of {@code
     * str} begins.
     * @param length The number of elements in the desired portion of {@code str}
     * (but not more than {@code str} 's length).
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     */
    public NormalizerInput(
  String str,
  int index,
  int length,
  Normalization form) {
 this(
  new StringCharacterInput2(str, index, length), form);
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizerInput} class.
     * @param str The parameter {@code str} is a text string.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     */
    public NormalizerInput(String str, Normalization form) {
 this(new StringCharacterInput2(str), form);
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizerInput} class.
     * @param stream The parameter {@code stream} is an ICharacterInput object.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @throws java.lang.NullPointerException The parameter {@code stream} is null.
     */
    public NormalizerInput(
   ICharacterInput stream,
   Normalization form) {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      this.lastQcsIndex = -1;
      this.iterator = stream;
      this.form = form;
      this.readbuffer = new int[1];
      this.lastCharBuffer = new int[2];
      this.compatMode = form == Normalization.NFKC || form ==
          Normalization.NFKD;
    }

    /**
     * Determines whether the text provided by a character input is normalized.
     * @param chars A object that implements a streamable character input.
     * @param form Specifies the normalization form to check.
     * @return {@code true} if the text is normalized; otherwise, {@code false}.
     * @throws java.lang.NullPointerException The parameter {@code chars} is null.
     */
    public static boolean IsNormalized(
  ICharacterInput chars,
  Normalization form) {
      if (chars == null) {
        throw new NullPointerException("chars");
      }
      int listIndex = 0;
      int[] array = new int[16];
      boolean haveNonQcs = false;
      while (true) {
        int c = chars.ReadChar();
        if (c < 0) {
          break;
        }
        if ((c & 0x1ff800) == 0xd800) {
          return false;
        }
        boolean isQcs = (c >= 0xf0000) ? true :
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
        if (listIndex >= array.length) {
          int[] newArray = new int[array.length * 2];
          System.arraycopy(array, 0, newArray, 0, listIndex);
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

    private static boolean NormalizeAndCheck(
  int[] charArray,
  int start,
  int length,
  Normalization form) {
      int i = 0;
      int ch;
      NormalizerInput input = new NormalizerInput(
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

    /**
     * Converts a string to the given Unicode normalization form.
     * @param str An arbitrary string.
     * @param form The Unicode normalization form to convert to.
     * @return The parameter {@code str} converted to the given normalization form.
     * @throws IllegalArgumentException The parameter {@code str} contains an
     * unpaired surrogate code point.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     */
    public static String Normalize(String str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() <= 1024 && IsNormalized(str, form)) {
        return str;
      }
      return Encodings.InputToString(
        new NormalizerInput(str, form));
    }

    /**
     * Determines whether the given string is in the given Unicode normalization
     * form.
     * @param str An arbitrary string.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return {@code true} if the given string is in the given Unicode
     * normalization form; otherwise, {@code false}. Returns {@code false}
     * if the string contains an unpaired surrogate code point.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     */
    public static boolean IsNormalized(String str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      // DebugUtility.Log (str);
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      int lastQcsIndex = 0;
      boolean haveNonQcs = false;
      for (int i = 0; i < str.length(); ++i) {
        int c = str.charAt(i);
        if ((c & 0xfc00) == 0xd800 && i + 1 < str.length() &&
            str.charAt(i + 1) >= 0xdc00 && str.charAt(i + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (str.charAt(i + 1) - 0xdc00);
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate
          return false;
        }
        boolean isQcs = false;
        if ((c & mask) == c && (i + 1 == str.length() || (str.charAt(i + 1) & mask)
          == str.charAt(i + 1))) {
          // Quick check for an ASCII character (or Latin-1 in NFC) followed
          // by another
          // ASCII character (or Latin-1 in NFC) or the end of String.
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
    str.length() - lastQcsIndex,
    form)) {
          return false;
        }
      }
      return true;
    }

    private static boolean NormalizeAndCheckString(
      String charString,
      int start,
      int length,
      Normalization form) {
      int i = start;
      NormalizerInput norm = new NormalizerInput(
     charString,
     start,
     length,
     form);
      int ch = 0;
      int endIndex = start + length;
      while ((ch = norm.ReadChar()) >= 0) {
        int c = charString.charAt(i);
        if ((c & 0x1ffc00) == 0xd800 && i + 1 < endIndex &&
            charString.charAt(i + 1) >= 0xdc00 && charString.charAt(i + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (charString.charAt(i + 1) - 0xdc00);
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

    private final int[] readbuffer;

    /**
     * Reads a Unicode character from a data source.
     * @return Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
     * 0x10ffff), or the value -1 indicating the end of the source.
     */
    public int ReadChar() {
      int r = this.Read(this.readbuffer, 0, 1);
      return r == 1 ? this.readbuffer[0] : -1;
    }

    private boolean endOfString;
    private int[] lastCharBuffer;
    private int lastCharPos;

    private void PrependOne(int c) {
      if (this.lastCharPos + 1 > this.lastCharBuffer.length) {
        int[] newbuffer = new int[this.lastCharPos + 8];
        System.arraycopy(this.lastCharBuffer, 0, newbuffer, 0, this.lastCharPos);
        this.lastCharBuffer = newbuffer;
      }
      this.lastCharBuffer[this.lastCharPos++] = c;
    }

    private void PrependTwo(int c1, int c2) {
      if (this.lastCharPos + 2 > this.lastCharBuffer.length) {
        int[] newbuffer = new int[this.lastCharPos + 8];
        System.arraycopy(this.lastCharBuffer, 0, newbuffer, 0, this.lastCharPos);
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
        throw new IllegalArgumentException("Invalid character: " + ch);
      }
      return ch;
    }

    /* private static String EC(int c) {
              if (c < 0) {
                  return "<" + c + ">";
              }
      String uesc = "\\u";
              if (c >= 0x10000) {
                  return String.Format (uesc + "{0:X8}", c);
              }
        return (c >= 0x7f || c<0x20) ? (String.Format (uesc + "{0:X4}", c)):
          ((c == 0x20) ? ("<space>") : ("<" + ((char)c) + ">"));
          }

      private static String EC(int[] b, int o, int sz) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < sz; ++i) {
          sb.append (EC (b [i + o]));
        }
        return sb.toString();
      }

    */

    /**
     * Reads a sequence of Unicode code points from a data source.
     * @param chars Output buffer.
     * @param index A zero-based index showing where the desired portion of {@code
     * chars} begins.
     * @param length The number of elements in the desired portion of {@code chars}
     * (but not more than {@code chars} 's length).
     * @return The number of Unicode code points read, or 0 if the end of the
     * source is reached.
     * @throws java.lang.NullPointerException The parameter {@code chars} is null.
     * @throws IllegalArgumentException Either {@code index} or {@code length} is
     * less than 0 or greater than {@code chars} 's length, or {@code chars}
     * ' s length minus {@code index} is less than {@code length}.
     */
    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new NullPointerException("chars");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
            "0");
      }
      if (index > chars.length) {
        throw new IllegalArgumentException("index (" + index + ") is more than " +
          chars.length);
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + length + ") is less than " +
              "0");
      }
      if (length > chars.length) {
        throw new IllegalArgumentException("length (" + length + ") is more than " +
          chars.length);
      }
      if (chars.length - index < length) {
        throw new IllegalArgumentException("chars's length minus " + index + " (" +
          (chars.length - index) + ") is less than " + length);
      }
      if (length == 0) {
        return 0;
      }
      int total = 0;
      int count = 0;
      if (this.processedIndex == this.flushIndex && this.flushIndex == 0) {
        while (total < length) {
          int c = this.GetNextChar();
          // DebugUtility.Log ("read: " + (EC(c)));
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
        count = Math.min(
   this.processedIndex - this.flushIndex,
   length - total);
        if (count < 0) {
          count = 0;
        }
        // DebugUtility.Log ("B count=" + count);
        if (count != 0) {
          // Fill buffer with processed code points
          System.arraycopy(this.buffer, this.flushIndex, chars, index, count);
        }
        index += count;
        total += count;
        this.flushIndex += count;
        boolean decompForm = this.form == Normalization.NFD ||
            this.form == Normalization.NFKD;
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

            System.arraycopy(
  this.buffer,
  this.lastQcsIndex,
  this.buffer,
  0,
  this.buffer.length - this.lastQcsIndex);
            // System.out.println("endIndex=" + (this.endIndex));
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
      count = Math.max(
  0,
  Math.min(this.processedIndex - this.flushIndex, length - total));
      System.arraycopy(this.buffer, this.flushIndex, chars, index, count);
      index += count;
      total += count;
      this.flushIndex += count;
      return (total == 0) ? -1 : total;
    }

    private boolean LoadMoreData() {
      boolean done = false;
      while (!done) {
        this.buffer = (this.buffer == null) ? ((new int[32])) : this.buffer;
        // Fill buffer with decompositions until the buffer is full
        // or the end of the String is reached.
        while (this.endIndex + 18 <= this.buffer.length) {
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
        // end of the String is not reached yet
        if (!this.endOfString) {
          boolean haveNewQcs = false;
          // NOTE: lastQcsIndex begins at -1
          boolean decompForm = this.form == Normalization.NFD ||
            this.form == Normalization.NFKD;
          boolean nextIsQCS = false;
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
            int[] newBuffer = new int[(this.buffer.length + 4) * 2];
            System.arraycopy(this.buffer, 0, newBuffer, 0, this.buffer.length);
            this.buffer = newBuffer;
            continue;
          }
        } else {
          // End of String
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
      // DebugUtility.Log ("reordering " + (// EC (buffer, 0, lastQcsIndex)) +
      // " [" + this.form + "]");
      // Canonical reordering
      ReorderBuffer(this.buffer, 0, this.lastQcsIndex);
      if (this.form == Normalization.NFC || this.form == Normalization.NFKC) {
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

    private static final class PartialArrayCharacterInput implements ICharacterInput {
      private final int endPos;
      private final int[] array;
      private int pos;

      public PartialArrayCharacterInput(int[] array, int start, int length) {
        this.array = array;
        this.pos = start;
        this.endPos = start + length;
      }

      public PartialArrayCharacterInput(int[] array) {
        this.array = array;
        this.pos = 0;
        this.endPos = array.length;
      }

      public int ReadChar() {
        return (this.pos < this.endPos) ? this.array[this.pos++] : (-1);
      }

      public int Read(int[] buf, int offset, int unitCount) {
        if (unitCount == 0) {
          return 0;
        }
        int maxsize = Math.min(unitCount, this.endPos - this.pos);
        System.arraycopy(this.array, this.pos, buf, offset, maxsize);
        this.pos += maxsize;
        return maxsize == 0 ? -1 : maxsize;
      }
    }

    private static final class StringCharacterInput2 implements ICharacterInput {
      private final String str;
      private final int endIndex;
      private int index;

      public StringCharacterInput2(String str) {
        if (str == null) {
          throw new NullPointerException("str");
        }
        this.str = str;
        this.endIndex = str.length();
      }

      public StringCharacterInput2(String str, int index, int length) {
        if (str == null) {
          throw new NullPointerException("str");
        }
        if (index < 0) {
          throw new IllegalArgumentException("index (" + index + ") is less than " +
              "0");
        }
        if (index > str.length()) {
          throw new IllegalArgumentException("index (" + index + ") is more than " +
            str.length());
        }
        if (length < 0) {
          throw new IllegalArgumentException("length (" + length +
              ") is less than " + "0");
        }
        if (length > str.length()) {
          throw new IllegalArgumentException("length (" + length +
              ") is more than " + str.length());
        }
        if (str.length() - index < length) {
          throw new IllegalArgumentException("str's length minus " + index + " (" +
            (str.length() - index) + ") is less than " + length);
        }
        this.str = str;
        this.index = index;
        this.endIndex = index + length;
      }

      public int ReadChar() {
        if (this.index >= this.endIndex) {
          return -1;
        }
        int c = this.str.charAt(this.index);
        if ((c & 0xfc00) == 0xd800 && this.index + 1 < this.endIndex &&
      this.str.charAt(this.index + 1) >= 0xdc00 && this.str.charAt(this.index + 1) <=
              0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (this.str.charAt(this.index + 1) -
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
          throw new NullPointerException("chars");
        }
        if (index < 0) {
          throw new IllegalArgumentException("index (" + index + ") is less than " +
              "0");
        }
        if (index > chars.length) {
          throw new IllegalArgumentException("index (" + index + ") is more than " +
            chars.length);
        }
        if (length < 0) {
          throw new IllegalArgumentException("length (" + length +
              ") is less than " + "0");
        }
        if (length > chars.length) {
          throw new IllegalArgumentException("length (" + length +
              ") is more than " + chars.length);
        }
        if (chars.length - index < length) {
          throw new IllegalArgumentException("chars's length minus " + index + " (" +
            (chars.length - index) + ") is less than " + length);
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
