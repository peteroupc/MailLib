/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;

using PeterO.Text.Encoders;

namespace PeterO.Text {
    /// <summary><para>Implements the Unicode normalization algorithm and
    /// contains methods and functionality to test and convert Unicode
    /// strings for Unicode normalization.</para>
    /// <para>NOTICE: While this
    /// class's source code is in the public domain, the class uses an
    /// internal class, called NormalizationData, that includes data
    /// derived from the Unicode Character Database. See the documentation
    /// for the NormalizingCharacterInput class for the permission notice
    /// for the Unicode Character Database.</para>
    /// </summary>
  [Obsolete(
  "Use NormalizingCharacterInput instead; that class is much more flexible than Normalizer." )]
  public sealed class Normalizer {
    /// <summary>Converts a string to the given Unicode normalization
    /// form.</summary>
    /// <param name='str'>An arbitrary string.</param>
    /// <param name='form'>The Unicode normalization form to convert
    /// to.</param>
    /// <returns>The parameter <paramref name='str'/> converted to the
    /// given normalization form.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static string Normalize(string str, Normalization form) {
      return NormalizingCharacterInput.Normalize(str, form);
    }

    private NormalizingCharacterInput nci;

    /// <summary>Initializes a new instance of the Normalizer
    /// class.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='form'>A Normalization object.</param>
    public Normalizer(string str, Normalization form) {
      this.nci = new NormalizingCharacterInput(str, form);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>Not documented yet.</param>
    /// <param name='form'>A Normalization object.</param>
    /// <returns>A Boolean object.</returns>
    public static bool IsNormalized(string str, Normalization form) {
      return NormalizingCharacterInput.IsNormalized(str, form);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadChar() {
      return this.nci.ReadChar();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='chars'>An array of 32-bit unsigned integers.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='length'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> or "this.buffer" is null.</exception>
    public int Read(int[] chars, int index, int length) {
      return this.nci.Read(chars, index, length);
    }
  }
}
