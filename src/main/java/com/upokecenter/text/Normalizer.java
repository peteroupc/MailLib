package com.upokecenter.text;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

  /**
   * <p>Implements the Unicode normalization algorithm and contains methods and
   * functionality to test and convert Unicode strings for Unicode
   * normalization.</p> <p>NOTICE: While this class's source code is in the
   * public domain, the class uses an class, called NormalizationData,
   * that includes data derived from the Unicode Character Database. See the
   * documentation for the NormalizerInput class for the permission notice for
   * the Unicode Character Database.</p>
   * @deprecated Use NormalizerInput instead; that class is much more flexible than
 * Normalizer.
 */
@Deprecated
  public final class Normalizer {
    /**
     * Converts a string to the specified Unicode normalization form.
     * @param str An arbitrary string.
     * @param form The Unicode normalization form to convert to.
     * @return The parameter {@code str} converted to the specified normalization
     * form.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static String Normalize(String str, Normalization form) {
      return NormalizerInput.Normalize(str, form);
    }

    private final NormalizerInput nci;

    /**
     * Initializes a new instance of the {@link com.upokecenter.text.Normalizer}
     * class.
     * @param str The parameter {@code str} is a text string.
     * @param form The parameter {@code form} is a Normalization object.
     */
    public Normalizer(String str, Normalization form) {
      this.nci = new NormalizerInput(str, form);
    }

    /**
     * Returns whether this string is normalized.
     * @param str The string to check.
     * @param form The parameter {@code form} is a Normalization object.
     * @return {@code true} if this string is normalized; otherwise, {@code false}.
     * Returns {@code false} if the string contains an unpaired surrogate code
     * point.
     */
    public static boolean IsNormalized(String str, Normalization form) {
      return NormalizerInput.IsNormalized(str, form);
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
     * @throws IllegalArgumentException Either {@code index} or {@code length} is less
     * than 0 or greater than {@code chars} 's length, or {@code chars} 's length
     * minus {@code index} is less than {@code length}.
     * @throws NullPointerException The parameter {@code chars} is null.
     */
    public int Read(int[] chars, int index, int length) {
      return this.nci.Read(chars, index, length);
    }
  }
