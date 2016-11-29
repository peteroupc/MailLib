package com.upokecenter.text;
/*
  Written by Peter O. in 2014.
  Any copyright is dedicated to the Public Domain.
  http://creativecommons.org/publicdomain/zero/1.0/
  If you like this, you should donate to Peter O.
  at: http://peteroupc.github.io/
   */

  import java.util.*;

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
     * @deprecated Renamed to NormalizerInput.
 */
@Deprecated
    public final class NormalizingCharacterInput implements ICharacterInput {
      private final ICharacterInput nci;

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizingCharacterInput} class using
     * Normalization Form C.
     * @param str A string specifying the text to normalize.
     */
      public NormalizingCharacterInput(
    String str) {
 this(
    str, Normalization.NFC);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizingCharacterInput} class using
     * Normalization Form C.
     * @param input An ICharacterInput object.
     */
      public NormalizingCharacterInput(
    ICharacterInput input) {
 this(
    input, Normalization.NFC);
      }

    /**
     *
     * @deprecated Either convert the list to a String or wrap it in an ICharacterInput and
* call the corresponding overload instead.
 */
@Deprecated
      public NormalizingCharacterInput(List<Integer> characterList) {
 this(characterList, Normalization.NFC);
      }

    /**
     *
     * @deprecated Either convert the list to a String or wrap it in an ICharacterInput and
* call the corresponding overload instead.
 */
@Deprecated
      public NormalizingCharacterInput(
    List<Integer> characterList,
    Normalization form) {
 this(new PartialListCharacterInput(characterList), form);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizingCharacterInput} class. Uses a portion
     * of a string as the input.
     * @param str A text string.
     * @param index A zero-based index showing where the desired portion of {@code
     * str} begins.
     * @param length The number of elements in the desired portion of {@code str}
     * (but not more than {@code str} 's length).
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     */
      public NormalizingCharacterInput(
    String str,
    int index,
    int length,
    Normalization form) {
       this.nci = new NormalizerInput(str, index, length, form);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizingCharacterInput} class.
     * @param str A text string.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     */
      public NormalizingCharacterInput(String str, Normalization form) {
       this.nci = new NormalizerInput(str, form);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.text.NormalizingCharacterInput} class.
     * @param stream An ICharacterInput object.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @throws java.lang.NullPointerException The parameter {@code stream} is null.
     */
      public NormalizingCharacterInput(
     ICharacterInput stream,
     Normalization form) {
       this.nci = new NormalizerInput(stream, form);
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
      return NormalizerInput.IsNormalized(chars, form);
    }

    /**
     * Converts a string to the given Unicode normalization form.
     * @param str An arbitrary string.
     * @param form The Unicode normalization form to convert to.
     * @return The parameter {@code str} converted to the given normalization form.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     */
      public static String Normalize(String str, Normalization form) {
       return NormalizerInput.Normalize(str, form);
      }

    /**
     * Determines whether the given string is in the given Unicode normalization
     * form.
     * @param str An arbitrary string.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return {@code true} if the given string is in the given Unicode
     * normalization form; otherwise, {@code false}.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     */
      public static boolean IsNormalized(String str, Normalization form) {
        return NormalizerInput.IsNormalized(str, form);
      }

    /**
     * Gets a list of normalized code points after reading from a string.
     * @param str A text string.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return A list of the normalized Unicode characters.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     * @deprecated Instead of this method, create a NormalizingCharacterInput on the String and
 *call ReadChar to get the normalized String's code points.
 */
@Deprecated
      public static List<Integer> GetChars(String str, Normalization form) {
        if (str == null) {
          throw new NullPointerException("str");
        }
        List<Integer> ret = new ArrayList<Integer>();
        int ch;
        NormalizingCharacterInput input = new NormalizingCharacterInput(str, form);
        while ((ch = input.ReadChar()) >= 0) {
           ret.add(ch);
        }
        return ret;
      }

    /**
     * Gets a list of normalized code points after reading from a character stream.
     * @param str An object that implements a stream of Unicode characters.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return A list of the normalized Unicode characters.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     * @deprecated Instead of this method, create a NormalizingCharacterInput on the input and
 *call ReadChar to get the normalized String's code points.
 */
@Deprecated
   public static List<Integer> GetChars(
  ICharacterInput input,
  Normalization form) {
        if (input == null) {
          throw new NullPointerException("input");
        }
        List<Integer> ret = new ArrayList<Integer>();
        int ch;
        input = new NormalizingCharacterInput(input, form);
        while ((ch = input.ReadChar()) >= 0) {
           ret.add(ch);
        }
        return ret;
      }

    /**
     * Determines whether the given array of characters is in the given Unicode
     * normalization form.
     * @param charArray An array of Unicode code points.
     * @param form Specifies the normalization form to use when normalizing the
     * text.
     * @return {@code true} if the given list of characters is in the given Unicode
     * normalization form; otherwise, {@code false}.
     * @throws java.lang.NullPointerException The parameter "charList" is null.
     * @deprecated Either convert the array to a String or wrap it in an ICharacterInput and
* call the corresponding overload instead.
 */
@Deprecated
    public static boolean IsNormalized(int[] charArray, Normalization form) {
     if (charArray == null) {
      throw new NullPointerException("charArray");
     }
     return IsNormalized(new PartialArrayCharacterInput(charArray), form);
   }

    /**
     *
     * @deprecated Either convert the list to a String or wrap it in an ICharacterInput and
* call the corresponding overload instead.
 */
@Deprecated
      public static boolean IsNormalized(List<Integer> charList, Normalization form) {
          return IsNormalized(
  new PartialListCharacterInput(charList),
  form);
     }

    /**
     * Reads a Unicode character from a data source.
     * @return Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
     * 0x10ffff), or the value -1 indicating the end of the source.
     */
      public int ReadChar() {
        return this.nci.ReadChar();
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
     * @throws java.lang.NullPointerException The parameter {@code chars} is null.
     * @throws IllegalArgumentException Either {@code index} or {@code length} is
     * less than 0 or greater than {@code chars} 's length, or {@code chars}
     * ' s length minus {@code index} is less than {@code length}.
     */
      public int Read(int[] chars, int index, int length) {
        return this.nci.Read(chars, index, length);
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
    }
