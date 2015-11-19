package com.upokecenter.text;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import com.upokecenter.text.encoders.*;

    /**
     * <p>Implements the Unicode normalization algorithm and contains methods and
     * functionality to test and convert Unicode strings for Unicode
     * normalization.</p> <p>NOTICE: While this class's source code is in
     * the public domain, the class uses an class, called
     * NormalizationData, that includes data derived from the Unicode
     * Character Database. See the documentation for the
     * NormalizingCharacterInput class for the permission notice for the
     * Unicode Character Database.</p>
     * @deprecated Use NormalizingCharacterInput instead; that class is much more flexible than
* Normalizer.
 */
@Deprecated
  public final class Normalizer {
    /**
     * Converts a string to the given Unicode normalization form.
     * @param str An arbitrary string.
     * @param form The Unicode normalization form to convert to.
     * @return The parameter {@code str} converted to the given normalization form.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static String Normalize(String str, Normalization form) {
      return NormalizingCharacterInput.Normalize(str, form);
    }

    private NormalizingCharacterInput nci;

    /**
     * Initializes a new instance of the Normalizer class.
     * @param str A string object.
     * @param form A Normalization object.
     */
    public Normalizer (String str, Normalization form) {
      this.nci = new NormalizingCharacterInput(str, form);
    }

    /**
     * Returns whether this string is normalized.
     * @param str The string to check.
     * @param form A Normalization object.
     * @return True if this string is normalized; otherwise, false.
     */
    public static boolean IsNormalized(String str, Normalization form) {
      return NormalizingCharacterInput.IsNormalized(str, form);
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
     * @param index Index in the output buffer to start writing to.
     * @param length Maximum number of code points to write.
     * @return The number of Unicode code points read, or 0 if the end of the
     * source is reached.
     * @throws java.lang.IllegalArgumentException Either "index" or "length" is less than 0
     * or greater than "chars"'s length, or "chars"'s length minus "index"
     * is less than "length".
     * @throws java.lang.NullPointerException The parameter "chars" is null.
     */
    public int Read(int[] chars, int index, int length) {
      return this.nci.Read(chars, index, length);
    }
  }
