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
     * Not documented yet.
     * @param str Not documented yet.
     * @param form A Normalization object.
     * @return A Boolean object.
     */
    public static boolean IsNormalized(String str, Normalization form) {
      return NormalizingCharacterInput.IsNormalized(str, form);
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int ReadChar() {
      return this.nci.ReadChar();
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
      return this.nci.Read(chars, index, length);
    }
  }
