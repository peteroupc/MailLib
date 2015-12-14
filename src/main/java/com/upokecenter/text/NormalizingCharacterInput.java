package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.util.*;

    /**
     * <p>A character input class that implements the Unicode normalization
     * algorithm and contains methods and functionality to test and convert
     * text strings for normalization. This is similar to the Normalizer
     * class, except it implements the ICharacterInput interface.</p> <p>The
     * Unicode Standard includes characters, such as an acute accent, that
     * can be combined with other characters to make new characters. For
     * example, the letter E combines with an acute accent to make E-acute
     * (&#xc9;). In some cases, the combined form (E-acute) should be
     * treated as equivalent to the uncombined form (E plus acute). For this
     * reason, the standard defines four <i>normalization forms</i> that
     * convert strings to a single equivalent form:</p> <ul> <li><b>NFD</b>
     * (Normalization Form D) decomposes combined forms to their constituent
     * characters (E plus acute, for example). This is called canonical
     * decomposition.</li> <li><b>NFC</b> does canonical decomposition, then
     * combines certain constituent characters to their composites (E-acute,
     * for example). This is called canonical composition.</li> <li>Two
     * normalization forms, <b>NFKC</b> and <b>NFKD</b>, are similar to NFC
     * and NFD, except they also "decompose" certain characters, such as
     * ligatures, font or positional variants, and subscripts, whose visual
     * distinction can matter in some contexts. This is called compatibility
     * decomposition.</li> <li>The four normalization forms also enforce a
     * standardized order for combining marks, since they can otherwise
     * appear in an arbitrary order.</li></ul> <p>For more information, see
     * Standard Annex 15 at http://www.unicode.org/reports/tr15/ .</p>
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
  public final class NormalizingCharacterInput implements ICharacterInput {
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
            // Lead is now at trail's position
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

    private static List<Integer> GetChars(ICharacterInput input) {
      int[] buffer = new int[64];
      List<Integer> ret = new ArrayList<Integer>(24);
      int count = 0;
      while ((count = input.Read(buffer, 0, buffer.length)) > 0) {
        for (int i = 0; i < count; ++i) {
          ret.add(buffer[i]);
        }
      }
      return ret;
    }

    /**
     * Gets a list of normalized code points after reading from a string.
     * @param str A string object.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return A list of the normalized Unicode characters.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static List<Integer> GetChars(String str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return GetChars(new NormalizingCharacterInput(str, form));
    }

    /**
     * Gets a list of normalized code points after reading from a character stream.
     * @param str An object that implements a stream of Unicode characters.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return A list of the normalized Unicode characters.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static List<Integer> GetChars(ICharacterInput str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return GetChars(new NormalizingCharacterInput(str, form));
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
     * Initializes a new instance of the NormalizingCharacterInput class using
     * Normalization Form C.
     * @param characterList A list of Unicode code points specifying the text to
     * normalize.
     */
    public NormalizingCharacterInput (List<Integer> characterList) {
 this(characterList, Normalization.NFC);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class using
     * Normalization Form C.
     * @param str A string specifying the text to normalize.
     */
    public NormalizingCharacterInput (
  String str) {
 this(
  str, Normalization.NFC);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class using
     * Normalization Form C.
     * @param input An ICharacterInput object.
     */
    public NormalizingCharacterInput (
ICharacterInput input) {
 this(
input, Normalization.NFC);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class using the
     * given normalization form.
     * @param characterList An List object.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @throws NullPointerException The parameter {@code characterList} is null.
     */
    public NormalizingCharacterInput (
List<Integer> characterList,
Normalization form) {
 this(new PartialListCharacterInput(characterList), form);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class. Uses a
     * portion of a string as the input.
     * @param str A string object.
     * @param index A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     */
    public NormalizingCharacterInput (
String str,
int index,
int length,
Normalization form) {
 this(
Encodings.StringToInput(str, index, length), form);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class.
     * @param str A string object.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     */
    public NormalizingCharacterInput (String str, Normalization form) {
 this(Encodings.StringToInput(str), form);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class.
     * @param stream An ICharacterInput object.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @throws NullPointerException The parameter {@code stream} is null.
     */
    public NormalizingCharacterInput (
   ICharacterInput stream,
   Normalization form) {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      this.lastQcsIndex = -1;
      this.iterator = stream;
      this.form = form;
      this.lastCharBuffer = new int[2];
      this.compatMode = form == Normalization.NFKC || form ==
          Normalization.NFKD;
    }

    /**
     * Determines whether the text provided by a character input is normalized.
     * @param chars A object that implements a streamable character input.
     * @param form Specifies the normalization form to check.
     * @return True if the text is normalized; otherwise, false.
     * @throws NullPointerException The parameter {@code chars} is null.
     */
    public static boolean IsNormalized(ICharacterInput chars, Normalization form) {
      if (chars == null) {
        throw new NullPointerException("chars");
      }
      List<Integer> list = new ArrayList<Integer>();
      int ch = 0;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      boolean norm = true;
      while ((ch = chars.ReadChar()) >= 0) {
        if ((ch & 0x1ff800) == 0xd800) {
          return false;
        }
        if (norm && (ch & mask) != ch) {
          norm = false;
        }
        list.add(ch);
      }
      return norm || IsNormalized(list, form);
    }

    private static boolean NormalizeAndCheck(
List<Integer> charList,
int start,
int length,
Normalization form) {
      int i = 0;
      for (int ch : NormalizingCharacterInput.GetChars(
        new PartialListCharacterInput(charList, start, length),
        form)) {
        if (i >= length) {
          return false;
        }
        if (ch != charList.get(start + i)) {
          return false;
        }
        ++i;
      }
      return true;
    }

    /**
     * Converts a string to the given Unicode normalization form.
     * @param str An arbitrary string.
     * @param form The Unicode normalization form to convert to.
     * @return The parameter {@code str} converted to the given normalization form.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static String Normalize(String str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() <= 1024 && IsNormalized(str, form)) {
        return str;
      }
      return Encodings.InputToString(
        new NormalizingCharacterInput(str, form));
    }

    /**
     * Determines whether the given string is in the given Unicode normalization
     * form.
     * @param str An arbitrary string.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return True if the given string is in the given Unicode normalization form;
     * otherwise, false.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static boolean IsNormalized(String str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      int nonQcsStart = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      int lastQcs = 0;
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
        isQcs = (c >= 0xf0000) ? true : (
UnicodeDatabase.IsQuickCheckStarter(
c,
form));
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
str.length() - nonQcsStart,
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
      NormalizingCharacterInput norm = new NormalizingCharacterInput(
     charString,
     start,
     length,
     form);
      int ch = 0;
      while ((ch = norm.ReadChar()) >= 0) {
        int c = charString.charAt(i);
        if ((c & 0x1ffc00) == 0xd800 && i + 1 < charString.length() &&
            charString.charAt(i + 1) >= 0xdc00 && charString.charAt(i + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (charString.charAt(i + 1) - 0xdc00);
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

    /**
     * Determines whether the given array of characters is in the given Unicode
     * normalization form.
     * @param charArray An array of Unicode code points.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return True if the given list of characters is in the given Unicode
     * normalization form; otherwise, false.
     * @throws NullPointerException The parameter "charList" is null.
     */
    public static boolean IsNormalized(int[] charArray, Normalization form) {
      if (charArray == null) {
  throw new NullPointerException("charArray");
}
      return IsNormalized(new PartialArrayCharacterInput(charArray), form);
    }

    /**
     * Determines whether the given list of characters is in the given Unicode
     * normalization form.
     * @param charList A list of Unicode code points.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return True if the given list of characters is in the given Unicode
     * normalization form; otherwise, false.
     * @throws NullPointerException The parameter {@code charList} is null.
     */
    public static boolean IsNormalized(List<Integer> charList, Normalization form) {
      int nonQcsStart = -1;
      int lastQcs = 0;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      if (charList == null) {
        throw new NullPointerException("charList");
      }
      for (int i = 0; i < charList.size(); ++i) {
        int c = charList.get(i);
        if (c < 0 || c > 0x10ffff || ((c & 0x1ff800) == 0xd800)) {
          return false;
        }
        boolean isQcs = false;
        isQcs = (c & mask) == c && (i + 1 == charList.size() || (charList.get(i +
               1) & mask) == charList.get(i + 1)) ? true :
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
charList.size() - nonQcsStart,
form)) {
          return false;
        }
      }
      return true;
    }

    private final int[] readbuffer = new int[1];

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

    /**
     * Reads a sequence of Unicode code points from a data source.
     * @param chars Output buffer.
     * @param index A zero-based index showing where the desired portion of {@code
     * chars} begins.
     * @param length The number of elements in the desired portion of {@code chars}
     * (but not more than {@code chars} 's length).
     * @return The number of Unicode code points read, or 0 if the end of the
     * source is reached.
     * @throws NullPointerException The parameter {@code chars} is null.
     * @throws IllegalArgumentException Either {@code index} or {@code length} is less
     * than 0 or greater than {@code chars} 's length, or {@code chars} 's
     * length minus {@code index} is less than {@code length}.
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
        count = Math.min(this.processedIndex - this.flushIndex, length - total);
        if (count < 0) {
          count = 0;
        }
        if (count != 0) {
          // Fill buffer with processed code points
          System.arraycopy(this.buffer, this.flushIndex, chars, index, count);
        }
        index += count;
        total += count;
        this.flushIndex += count;
        boolean decompForm = (this.form == Normalization.NFD ||
            this.form == Normalization.NFKD);
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
          boolean decompForm = (this.form == Normalization.NFD ||
            this.form == Normalization.NFKD);
          boolean nextIsQCS = false;
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

    private static final class PartialArrayCharacterInput implements ICharacterInput {
      private final int endPos;
      private final int[] array;
      private int pos;

      public PartialArrayCharacterInput (int[] array, int start, int length) {
        this.array = array;
        this.pos = start;
        this.endPos = start + length;
      }

      public PartialArrayCharacterInput (int[] array) {
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
  }
