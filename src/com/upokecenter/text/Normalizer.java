package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Implements the Unicode normalization algorithm and contains methods
     * and functionality to test and convert Unicode strings for Unicode
     * normalization.
     */
  public final class Normalizer
  {
    public static String Normalize(String str, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() <= 1024 && IsNormalized(str, form)) {
        return str;
      }
      Normalizer norm = new Normalizer(str, form);
      StringBuilder builder = new StringBuilder();
      int c = 0;
      while ((c = norm.ReadChar()) >= 0) {
        if (c <= 0xffff) {
          builder.append((char)c);
        } else if (c <= 0x10ffff) {
          builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
      return builder.toString();
    }

    static int DecompToBufferInternal(int ch, boolean compat, int[] buffer, int index) {

      int offset = UnicodeDatabase.GetDecomposition(ch, compat, buffer, index);
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

    static int DecompToBuffer(int ch, boolean compat, int[] buffer, int index) {

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
    private boolean compatMode;
    private Normalization form;
    private int processedIndex;
    private int flushIndex;
    private String iterator;
    private int characterListPos;

    public Normalizer (String str, Normalization form) {
      if (str == null) {
        throw new IllegalArgumentException("stream");
      }
      this.readbuffer = new int[1];
      this.iterEndIndex = str.length();
      this.lastStableIndex = -1;
      this.iterator = str;
      this.form = form;
      this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
    }

    private Normalizer Init(String str, int index, int length, Normalization form) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + index + ") is more than " + Integer.toString((int)str.length()));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is less than " + "0");
      }
      if (length > str.length()) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is more than " + Integer.toString((int)str.length()));
      }
      if (str.length() - index < length) {
        throw new IllegalArgumentException("str's length minus " + index + " (" + Integer.toString((int)(str.length() - index)) + ") is less than " + Integer.toString((int)length));
      }
      this.readbuffer = new int[1];
      this.lastStableIndex = -1;
      this.characterListPos = index;
      this.iterator = str;
      this.iterEndIndex = index + length;
      this.form = form;
      this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
      return this;
    }

    private static boolean NormalizeAndCheckString(
      String charList,
      int start,
      int length,
      Normalization form) {
      int i = start;
      Normalizer norm = new Normalizer(charList, form).Init(charList, start, length, form);
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
     * Not documented yet.
     * @param str A string object.
     * @param form A Normalization object.
     * @return A Boolean object.
     */
    public static boolean IsNormalized(String str, Normalization form) {
      if (str == null) {
        return false;
      }
      int lastNonStable = -1;
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
        if ((c & mask) == c && (i + 1 == str.length() || (str.charAt(i + 1) & mask) == str.charAt(i + 1))) {
          // Quick check for an ASCII character followed by another
          // ASCII character (or Latin-1 in NFC) or the end of String.
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
        if (!NormalizeAndCheckString(str, lastNonStable, str.length() - lastNonStable, form)) {
          return false;
        }
      }
      return true;
    }

    private int[] readbuffer;

    public int ReadChar() {
      int r = this.Read(this.readbuffer, 0, 1);
      return r == 1 ? this.readbuffer[0] : -1;
    }

    private boolean endOfString = false;
    private int lastChar = -1;
    private boolean ungetting = false;

    private void Unget() {
      this.ungetting = true;
    }

    private int GetNextChar() {
      int ch;
      if (this.ungetting) {
        ch = this.lastChar;
        this.ungetting = false;
        return ch;
      } else if (this.characterListPos >= this.iterEndIndex) {
        ch = -1;
      } else {
        ch = this.iterator.charAt(this.characterListPos);
        if ((ch & 0xfc00) == 0xd800 && this.characterListPos + 1 < this.iterEndIndex &&
            this.iterator.charAt(this.characterListPos + 1) >= 0xdc00 && this.iterator.charAt(this.characterListPos + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          ch = 0x10000 + ((ch - 0xd800) << 10) + (this.iterator.charAt(this.characterListPos + 1) - 0xdc00);
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
     */
    public int Read(int[] chars, int index, int length) {
      if (chars == null) {
        throw new NullPointerException("chars");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " + "0");
      }
      if (index > chars.length) {
        throw new IllegalArgumentException("index (" + index + ") is more than " + chars.length);
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is less than " + "0");
      }
      if (length > chars.length) {
        throw new IllegalArgumentException("length (" + Integer.toString((int)length) + ") is more than " + chars.length);
      }
      if (chars.length - index < length) {
        throw new IllegalArgumentException("chars's length minus " + index + " (" + Integer.toString((int)(chars.length - index)) + ") is less than " + Integer.toString((int)length));
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
          } else if (IsStableCodePoint(c, this.form)) {
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
        // System.out.println("indexes=" + this.processedIndex + " " + this.flushIndex + ", length=" + length + " total=" + (total));
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

            System.arraycopy(this.buffer, this.lastStableIndex, this.buffer, 0, this.buffer.length - this.lastStableIndex);
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
      count = Math.max(0, Math.min(this.processedIndex - this.flushIndex, length - total));
      System.arraycopy(this.buffer, this.flushIndex, chars, index, count);
      index += count;
      total += count;
      this.flushIndex += count;
      return (total == 0) ? -1 : total;
    }

    static boolean IsStableCodePoint(int cp, Normalization form) {
      // Exclude YOD and HIRIQ because of Corrigendum 2
      return UnicodeDatabase.IsStableCodePoint(cp, form) && cp != 0x5b4 && cp != 0x5d9;
    }

    private boolean LoadMoreData() {
      boolean done = false;
      while (!done) {
        if (this.buffer == null) {
          this.buffer = new int[32];
        }
        // Fill buffer with decompositions until the buffer is full
        // or the end of the String is reached.
        while (this.endIndex + 18 <= this.buffer.length) {
          int c = this.GetNextChar();
          if (c < 0) {
            this.endOfString = true;
            break;
          }
          this.endIndex = DecompToBuffer(c, this.compatMode, this.buffer, this.endIndex);
        }
        // Check for the last stable code point if the
        // end of the String is not reached yet
        if (!this.endOfString) {
          boolean haveNewStable = false;
          // NOTE: lastStableIndex begins at -1
          for (int i = this.endIndex - 1; i > this.lastStableIndex; --i) {
            // System.out.println("stable({0:X4})=" + (IsStableCodePoint(this.buffer[i], this.form)));
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
        this.processedIndex = ComposeBuffer(this.buffer, this.lastStableIndex);
      } else {
        this.processedIndex = this.lastStableIndex;
      }
      return true;
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
            // System.out.println("lead= {0:X4} ccc=" + (lead));
            // System.out.println("trail={0:X4} ccc=" + (trail));
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
      int compPos = 0;
      int endPos = 0 + length;
      boolean composed = false;
      for (int decompPos = compPos; decompPos < endPos; ++decompPos) {
        int ch = array[decompPos];
        int valuecc = UnicodeDatabase.GetCombiningClass(ch);
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
