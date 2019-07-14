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
    /// <para>Implements the Unicode normalization algorithm and contains
    /// methods and functionality to test and convert Unicode strings for
    /// Unicode normalization.</para>
    /// <para>NOTICE: While this class's source code is in the public
    /// domain, the class uses an internal class, called NormalizationData,
    /// that includes data derived from the Unicode Character Database. See
    /// the documentation for the NormalizerInput class for the permission
    /// notice for the Unicode Character Database.</para></summary>
  [Obsolete(
    "Use NormalizerInput instead; that class is much more flexible than " +
    "Normalizer.")]
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
      return NormalizerInput.Normalize(str, form);
    }

    private readonly NormalizerInput nci;

    /// <summary>Initializes a new instance of the
    /// <see cref='Normalizer'/> class.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='form'>The parameter <paramref name='form'/> is a
    /// Normalization object.</param>
    public Normalizer(string str, Normalization form) {
      this.nci = new NormalizerInput(str, form);
    }

    /// <summary>Returns whether this string is normalized.</summary>
    /// <param name='str'>The string to check.</param>
    /// <param name='form'>The parameter <paramref name='form'/> is a
    /// Normalization object.</param>
    /// <returns><c>true</c> if this string is normalized; otherwise,
    /// <c>false</c>. Returns <c>false</c> if the string contains an
    /// unpaired surrogate code point.</returns>
    public static bool IsNormalized(string str, Normalization form) {
      return NormalizerInput.IsNormalized(str, form);
    }

    /// <summary>Reads a Unicode character from a data source.</summary>
    /// <returns>Either a Unicode code point (from 0-0xd7ff or from 0xe000
    /// to 0x10ffff), or the value -1 indicating the end of the
    /// source.</returns>
    public int ReadChar() {
      return this.nci.ReadChar();
    }

    /// <summary>Reads a sequence of Unicode code points from a data
    /// source.</summary>
    /// <param name='chars'>Output buffer.</param>
    /// <param name='index'>Index in the output buffer to start writing
    /// to.</param>
    /// <param name='length'>Maximum number of code points to
    /// write.</param>
    /// <returns>The number of Unicode code points read, or 0 if the end of
    /// the source is reached.</returns>
    /// <exception cref='ArgumentException'>Either <paramref name='index'/>
    /// or <paramref name='length'/> is less than 0 or greater than
    /// <paramref name='chars'/> 's length, or <paramref name='chars'/> ' s
    /// length minus <paramref name='index'/> is less than <paramref
    /// name='length'/>.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    public int Read(int[] chars, int index, int length) {
      return this.nci.Read(chars, index, length);
    }
  }
}
