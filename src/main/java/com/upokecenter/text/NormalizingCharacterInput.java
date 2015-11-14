package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.util.*;

using PeterO.Text.Encoders;

    /**
     * <p>A character input class that implements the Unicode normalization
     * algorithm and contains methods and functionality to test and convert
     * Unicode strings for Unicode normalization. This is similar to the
     * Normalizer class, except it implements the ICharacterInput
     * interface.</p> <p>NOTICE: While this class's source code is in the
     * public domain, the class uses an class, called
     * NormalizationData, that includes data derived from the Unicode
     * Character Database. In case doing so is required, the permission
     * notice for the Unicode Character Database is given here:</p>
     * <p>COPYRIGHT AND PERMISSION NOTICE</p> <p>Copyright (c) 1991-2014
     * Unicode, Inc. All rights reserved. Distributed under the Terms of Use
     * in http://www.unicode.org/copyright.html.</p> <p>Permission is hereby
     * granted, free of charge, to any person obtaining a copy of the
     * Unicode data files and any associated documentation (the "Data
     * Files") or Unicode software and any associated documentation (the
     * "Software") to deal in the Data Files or Software without
     * restriction, including without limitation the rights to use, copy,
     * modify, merge, publish, distribute, and/or sell copies of the Data
     * Files or Software, and to permit persons to whom the Data Files or
     * Software are furnished to do so, provided that (a) this copyright and
     * permission notice appear with all copies of the Data Files or
     * Software, (b) this copyright and permission notice appear in
     * associated documentation, and (c) there is clear notice in each
     * modified Data File or in the Software as well as in the documentation
     * associated with the Data File(s) or Software that the data or
     * software has been modified.</p> <p>THE DATA FILES AND SOFTWARE ARE
     * PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
     * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY
     * RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN
     * THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR
     * CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
     * OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE
     * OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE
     * USE OR PERFORMANCE OF THE DATA FILES OR SOFTWARE.</p> <p>Except as
     * contained in this notice, the name of a copyright holder shall not be
     * used in advertising or otherwise to promote the sale, use or other
     * dealings in these Data Files or Software without prior written
     * authorization of the copyright holder.</p>
     */
  public final class NormalizingCharacterInput implements ICharacterInput
  {
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

    static boolean IsStableCodePoint(int cp, Normalization form) {
      // Exclude YOD and HIRIQ because of Corrigendum 2
      return UnicodeDatabase.IsStableCodePoint(cp, form) && cp != 0x5b4 &&
        cp != 0x5d9;
    }

    static void ReorderBuffer(int[] buffer, int index, int length) {
      int i;

      if (length < 2) {
        return;
      }
      boolean changed;
      do {
        changed = false;
        // System.out.println(toString(buffer, index, length));
        int lead = UnicodeDatabase.GetCombiningClass(buffer[index]);
        int trail;
        for (i = 1; i < length; ++i) {
          int offset = index + i;
          trail = UnicodeDatabase.GetCombiningClass(buffer[offset]);
          if (trail != 0 && lead > trail) {
            int c = buffer[offset - 1];
            buffer[offset - 1] = buffer[offset];
            buffer[offset] = c;
            // System.out.println("lead= {0:X4} ccc=" + lead);
            // System.out.println("trail={0:X4} ccc=" + trail);
            // System.out.println("now "+toString(buffer,index,length));
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

    /**
     * Not documented yet.
     * @param str A string object.
     * @param form A Normalization object.
     * @return A list of Unicode characters.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static List<Integer> GetChars(String str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return GetChars(new StringCharacterInput(str), form);
    }

    /**
     * Not documented yet.
     * @param str An ICharacterInput object.
     * @param form A Normalization object.
     * @return A list of Unicode characters.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static List<Integer> GetChars(ICharacterInput str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      NormalizingCharacterInput norm = new NormalizingCharacterInput(str, form);
      int[] buffer = new int[64];
      List<Integer> ret = new ArrayList<Integer>(24);
      int count = 0;
      while ((count = norm.Read(buffer, 0, buffer.length)) > 0) {
        for (int i = 0; i < count; ++i) {
          ret.add(buffer[i]);
        }
      }
      return ret;
    }

    private int lastStableIndex;
    private int endIndex;
    private int[] buffer;
    private boolean compatMode;
    private Normalization form;
    private int processedIndex;
    private int flushIndex;
    private ICharacterInput iterator;
    private List<Integer> characterList;
    private int characterListPos;

    /**
     * Initializes a new instance of the NormalizingCharacterInput class using
     * Normalization Form C.
     * @param characterList An List object.
     */
    public NormalizingCharacterInput (List<Integer> characterList) {
 this(characterList, Normalization.NFC);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class using
     * Normalization Form C.
     * @param str A string object.
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
     * @param form A Normalization object.
     * @throws NullPointerException The parameter {@code characterList} is null.
     */
    public NormalizingCharacterInput (
List<Integer> characterList,
Normalization form) {
      if (characterList == null) {
        throw new NullPointerException("characterList");
      }
      this.lastStableIndex = -1;
      this.characterList = characterList;
      this.form = form;
    this.compatMode = form == Normalization.NFKC || form ==
        Normalization.NFKD;
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class. Uses a
     * portion of a string as the input.
     * @param str A string object.
     * @param index A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @param form A Normalization object.
     */
    public NormalizingCharacterInput (
String str,
int index,
int length,
Normalization form) {
 this(
new StringCharacterInput(str, index, length), form);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class.
     * @param str A string object.
     * @param form A Normalization object.
     */
    public NormalizingCharacterInput (String str, Normalization form) {
 this(new StringCharacterInput(str), form);
    }

    /**
     * Initializes a new instance of the NormalizingCharacterInput class.
     * @param stream An ICharacterInput object.
     * @param form A Normalization object.
     * @throws NullPointerException The parameter {@code stream} is null.
     */
 public NormalizingCharacterInput (
ICharacterInput stream,
Normalization form) {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      this.lastStableIndex = -1;
      this.iterator = stream;
      this.form = form;
    this.compatMode = form == Normalization.NFKC || form ==
        Normalization.NFKD;
    }

    /**
     * Not documented yet.
     * @param chars An ICharacterInput object.
     * @param form A Normalization object.
     * @return A Boolean object.
     */
    public static boolean IsNormalized(ICharacterInput chars, Normalization form) {
      if (chars == null) {
        return false;
      }
      List<Integer> list = new ArrayList<Integer>();
      int ch = 0;
      while ((ch = chars.ReadChar()) >= 0) {
        if ((ch & 0xf800) == 0xd800) {
          return false;
        }
        list.add(ch);
      }
      return IsNormalized(list, form);
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
        if (ch != charList.charAt(start + i)) {
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
      return EncoderHelper.InputToString(
        new NormalizingCharacterInput(str));
    }

    /**
     * Determines whether the given string is in the given Unicode normalization
     * form.
     * @param str An arbitrary string.
     * @param form A Normalization object.
     * @return True if the given string is in the given Unicode normalization form;
     * otherwise, false.
     */
    public static boolean IsNormalized(String str, Normalization form) {
      if (str == null) {
        return false;
      }
      int nonStableStart = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
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
        boolean isStable = false;
        if ((c & mask) == c && (i + 1 == str.length() || (str.charAt(i + 1) & mask)
          == str.charAt(i + 1))) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC) or the end of String.
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
str.length() - nonStableStart,
form)) {
          return false;
        }
      }
      return true;
    }

    private static boolean NormalizeAndCheckString(
      String charList,
      int start,
      int length,
      Normalization form) {
      int i = start;
      NormalizingCharacterInput norm = new NormalizingCharacterInput(
     charList,
     start,
     length,
     form);
      int ch = 0;
      while ((ch = norm.ReadChar()) >= 0) {
        int c = charList.charAt(i);
        if ((c & 0xfc00) == 0xd800 && i + 1 < charList.length() &&
            charList.charAt(i + 1) >= 0xdc00 && charList.charAt(i + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (charList.charAt(i + 1) - 0xdc00);
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

    /**
     * Determines whether the given list of characters is in the given Unicode
     * normalization form.
     * @param charList An List object.
     * @param form A Normalization object.
     * @return True if the given list of characters is in the given Unicode
     * normalization form; otherwise, false.
     * @throws NullPointerException The parameter "chars" is null.
     */
    public static boolean IsNormalized(List<Integer> charList, Normalization form) {
      int nonStableStart = -1;
      int mask = (form == Normalization.NFC) ? 0xff : 0x7f;
      if (charList == null) {
        throw new NullPointerException("charList");
      }
      for (int i = 0; i < charList.size(); ++i) {
        int c = charList.charAt(i);
        if (c < 0 || c > 0x10ffff || ((c & 0x1ff800) == 0xd800)) {
          return false;
        }
        boolean isStable = false;
        if ((c & mask) == c && (i + 1 == charList.size() || (charList.charAt(i + 1)&
          mask) == charList.charAt(i + 1))) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC) or the end of String.
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
charList.size() - nonStableStart,
form)) {
          return false;
        }
      }
      return true;
    }

    private int[] readbuffer = new int[1];

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int ReadChar() {
      int r = this.Read(this.readbuffer, 0, 1);
      return r == 1 ? this.readbuffer[0] : -1;
    }

    private boolean endOfString;
    private int lastChar = -1;
    private boolean ungetting;

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
this.characterList.size()) ? -1 :
          this.characterList.get(this.characterListPos++)) :
        this.iterator.ReadChar();
      if (ch < 0) {
        this.endOfString = true;
      } else if (ch > 0x10ffff || ((ch & 0x1ff800) == 0xd800)) {
        throw new IllegalArgumentException("Invalid character: " + ch);
      }
      this.lastChar = ch;
      return ch;
    }

    /**
     * Not documented yet.
     * @param chars An array of 32-bit unsigned integers.
     * @param index A 32-bit signed integer. (2).
     * @param length A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     * @throws NullPointerException The parameter {@code chars} or "this.buffer"
     * is null.
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
        // System.out.println("indexes=" + this.processedIndex + " " +
        // this.flushIndex + ", length=" + length + " total=" + total);
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

            System.arraycopy(
this.buffer,
this.lastStableIndex,
this.buffer,
0,
this.buffer.length - this.lastStableIndex);
            // System.out.println("endIndex=" + (this.endIndex));
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
        // Check for the last stable code point if the
        // end of the String is not reached yet
        if (!this.endOfString) {
          boolean haveNewStable = false;
          // NOTE: lastStableIndex begins at -1
          for (int i = this.endIndex - 1; i > this.lastStableIndex; --i) {
            // System.out.println("stable({0:X4})=" +
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
            int[] newBuffer = new int[(this.buffer.length + 4) * 2];
            System.arraycopy(this.buffer, 0, newBuffer, 0, this.buffer.length);
            this.buffer = newBuffer;
            continue;
          }
        } else {
          // End of String
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
