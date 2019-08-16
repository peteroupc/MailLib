/*
  Written by Peter O. in 2014.
  Any copyright is dedicated to the Public Domain.
  http://creativecommons.org/publicdomain/zero/1.0/
  If you like this, you should donate to Peter O.
  at: http://peteroupc.github.io/
   */
using System;
using System.Collections.Generic;

namespace PeterO.Text {
    /// <summary>
    /// <para>A character input class that implements the Unicode
    /// normalization algorithm and contains methods and functionality to
    /// test and convert text strings for normalization. This is similar to
    /// the deprecated Normalizer class, except it implements the
    /// ICharacterInput interface.</para>
    /// <list>
    /// <item><b>NFD</b> (Normalization Form D) decomposes combined forms
    /// to their constituent characters (E plus acute, for example), then
    /// reorders combining marks to a standardized order. This is called
    /// canonical decomposition.</item>
    /// <item><b>NFC</b> does canonical decomposition, then combines
    /// certain constituent characters to their composites (E-acute, for
    /// example). This is called canonical composition.</item>
    /// <item>Two normalization forms, <b>NFKC</b> and <b>NFKD</b>, are
    /// similar to NFC and NFD, except they also "decompose" certain
    /// characters, such as ligatures, font or positional variants, and
    /// subscripts, whose visual distinction can matter in some contexts.
    /// This is called compatibility decomposition.</item></list>
    /// <para>For more information, see Standard Annex 15 at
    /// <c>http://www.unicode.org/reports/tr15/</c>.</para>
    /// <para><b>Thread safety:</b> This class is mutable; its properties
    /// can be changed. None of its instance methods are designed to be
    /// thread safe. Therefore, access to objects from this class must be
    /// synchronized if multiple threads can access them at the same
    /// time.</para>
    /// <para>NOTICE: While this class's source code is in the public
    /// domain, the class uses an internal class, called NormalizationData,
    /// that includes data derived from the Unicode Character Database. In
    /// case doing so is required, the permission notice for the Unicode
    /// Character Database is given here:</para>
    /// <para>COPYRIGHT AND PERMISSION NOTICE.</para>
    /// <para>Copyright (c) 1991-2014 Unicode, Inc. All rights reserved.
    /// Distributed under the Terms of Use in
    /// http://www.unicode.org/copyright.html.</para>
    /// <para>Permission is hereby granted, free of charge, to any person
    /// obtaining a copy of the Unicode data files and any associated
    /// documentation (the "Data Files") or Unicode software and any
    /// associated documentation (the "Software") to deal in the Data Files
    /// or Software without restriction, including without limitation the
    /// rights to use, copy, modify, merge, publish, distribute, and/or
    /// sell copies of the Data Files or Software, and to permit persons to
    /// whom the Data Files or Software are furnished to do so, provided
    /// that (a) this copyright and permission notice appear with all
    /// copies of the Data Files or Software, (b) this copyright and
    /// permission notice appear in associated documentation, and (c) there
    /// is clear notice in each modified Data File or in the Software as
    /// well as in the documentation associated with the Data File(s) or
    /// Software that the data or software has been modified.</para>
    /// <para>THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT
    /// WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    /// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
    /// PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS. IN NO EVENT
    /// SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE BE
    /// LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL
    /// DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA
    /// OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
    /// TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
    /// PERFORMANCE OF THE DATA FILES OR SOFTWARE.</para>
    /// <para>Except as contained in this notice, the name of a copyright
    /// holder shall not be used in advertising or otherwise to promote the
    /// sale, use or other dealings in these Data Files or Software without
    /// prior written authorization of the copyright
    /// holder.</para></summary>
  [Obsolete("Renamed to NormalizerInput.")]
  public sealed class NormalizingCharacterInput : ICharacterInput {
    private readonly ICharacterInput nci;

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizingCharacterInput'/>
    /// class.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    public NormalizingCharacterInput(
  string str) : this(
  str,
  Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizingCharacterInput'/>
    /// class.</summary>
    /// <param name='input'>The parameter <paramref name='input'/> is an
    /// ICharacterInput object.</param>
    public NormalizingCharacterInput(
  ICharacterInput input) : this(
  input,
  Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizingCharacterInput'/>
    /// class.</summary>
    /// <param name='characterList'>The parameter <paramref
    /// name='characterList'/> is an IList object.</param>
    public NormalizingCharacterInput(IList<int> characterList)
        : this(characterList, Normalization.NFC) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizingCharacterInput'/>
    /// class.</summary>
    /// <param name='characterList'>The parameter <paramref
    /// name='characterList'/> is an IList object.</param>
    /// <param name='form'>The parameter <paramref name='form'/> is a
    /// Normalization object.</param>
    public NormalizingCharacterInput(
    IList<int> characterList,
    Normalization form)
    : this(new PartialListCharacterInput(characterList), form) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizingCharacterInput'/>
    /// class.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='index'>The parameter <paramref name='index'/> is a
    /// 32-bit signed integer.</param>
    /// <param name='length'>The parameter <paramref name='length'/> is a
    /// 32-bit signed integer.</param>
    /// <param name='form'>The parameter <paramref name='form'/> is a
    /// Normalization object.</param>
    public NormalizingCharacterInput(
      string str,
      int index,
      int length,
      Normalization form) {
      this.nci = new NormalizerInput(str, index, length, form);
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizingCharacterInput'/>
    /// class.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='form'>The parameter <paramref name='form'/> is a
    /// Normalization object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public NormalizingCharacterInput(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      this.nci = new NormalizerInput(str, 0, str.Length, form);
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Text.NormalizingCharacterInput'/>
    /// class.</summary>
    /// <param name='stream'>The parameter <paramref name='stream'/> is an
    /// ICharacterInput object.</param>
    /// <param name='form'>The parameter <paramref name='form'/> is a
    /// Normalization object.</param>
    public NormalizingCharacterInput(
      ICharacterInput stream,
      Normalization form) {
      this.nci = new NormalizerInput(stream, form);
    }

    /// <summary>Determines whether the text provided by a character input
    /// is normalized.</summary>
    /// <param name='chars'>A object that implements a streamable character
    /// input.</param>
    /// <param name='form'>Specifies the normalization form to
    /// check.</param>
    /// <returns><c>true</c> if the text is normalized; otherwise,
    /// <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    public static bool IsNormalized(
      ICharacterInput chars,
      Normalization form) {
      return NormalizerInput.IsNormalized(chars, form);
    }

    /// <summary>Converts a string to the given Unicode normalization
    /// form.</summary>
    /// <param name='str'>An arbitrary string.</param>
    /// <param name='form'>The Unicode normalization form to convert
    /// to.</param>
    /// <returns>The parameter <paramref name='str'/> converted to the
    /// given normalization form.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='str'/> contains an unpaired surrogate code point.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static string Normalize(string str, Normalization form) {
      return NormalizerInput.Normalize(str, form);
    }

    /// <summary>Determines whether the given string is in the given
    /// Unicode normalization form.</summary>
    /// <param name='str'>An arbitrary string.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    /// <returns><c>true</c> if the given string is in the given Unicode
    /// normalization form; otherwise, <c>false</c>. Returns <c>false</c>
    /// if the string contains an unpaired surrogate code point.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static bool IsNormalized(string str, Normalization form) {
      return NormalizerInput.IsNormalized(str, form);
    }

    /// <summary>Gets a list of normalized code points after reading from a
    /// string.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    /// <returns>A list of the normalized Unicode characters.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    [Obsolete("Instead of this method, create a NormalizerInput on " +
     "the string and call ReadChar to get the normalized string's code " +
     "points.")]
    public static IList<int> GetChars(string str, Normalization form) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      IList<int> ret = new List<int>();
      int ch;
      var input = new NormalizingCharacterInput(str, form);
      while ((ch = input.ReadChar()) >= 0) {
        ret.Add(ch);
      }
      return ret;
    }

    /// <summary>Gets a list of normalized code points after reading from a
    /// character stream.</summary>
    /// <param name='chars'>An object that implements a stream of Unicode
    /// characters.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    /// <returns>A list of the normalized Unicode characters.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    [Obsolete("Instead of this method, create a NormalizerInput on the " +
     "input and call ReadChar to get the normalized string's code points.")]
    public static IList<int> GetChars(
      ICharacterInput chars,
      Normalization form) {
      if (chars == null) {
        throw new ArgumentNullException(nameof(chars));
      }
      IList<int> ret = new List<int>();
      int ch;
      chars = new NormalizingCharacterInput(chars, form);
      while ((ch = chars.ReadChar()) >= 0) {
        ret.Add(ch);
      }
      return ret;
    }

    /// <summary>Determines whether the given array of characters is in the
    /// given Unicode normalization form.</summary>
    /// <param name='charArray'>An array of Unicode code points.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    /// <returns><c>true</c> if the given list of characters is in the
    /// given Unicode normalization form; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter "charList" is
    /// null.</exception>
    [Obsolete("Either convert the array to a string or wrap it in " +
"an ICharacterInput and call the corresponding overload instead.")]
    public static bool IsNormalized(int[] charArray, Normalization form) {
      if (charArray == null) {
        throw new ArgumentNullException(nameof(charArray));
      }
      return IsNormalized(new PartialArrayCharacterInput(charArray), form);
    }

    /// <summary>Determines whether the given list of characters is in the
    /// given Unicode normalization form.</summary>
    /// <param name='charList'>A list of Unicode code points.</param>
    /// <param name='form'>Specifies the normalization form to use when
    /// normalizing the text.</param>
    /// <returns><c>true</c> if the given list of characters is in the
    /// given Unicode normalization form; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='charList'/> is null.</exception>
    [Obsolete("Either convert the list to a string or wrap it in " +
"an ICharacterInput and call the corresponding overload instead.")]
    public static bool IsNormalized(IList<int> charList, Normalization form) {
      return IsNormalized(
  new PartialListCharacterInput(charList),
  form);
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
    /// <param name='index'>A zero-based index showing where the desired
    /// portion of <paramref name='chars'/> begins.</param>
    /// <param name='length'>The number of elements in the desired portion
    /// of <paramref name='chars'/> (but not more than <paramref
    /// name='chars'/> 's length).</param>
    /// <returns>The number of Unicode code points read, or 0 if the end of
    /// the source is reached.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref name='index'/>
    /// or <paramref name='length'/> is less than 0 or greater than
    /// <paramref name='chars'/> 's length, or <paramref name='chars'/> ' s
    /// length minus <paramref name='index'/> is less than <paramref
    /// name='length'/>.</exception>
    public int Read(int[] chars, int index, int length) {
      return this.nci.Read(chars, index, length);
    }

    private sealed class PartialArrayCharacterInput : ICharacterInput {
      private readonly int endPos;
      private readonly int[] array;
      private int pos;

      public PartialArrayCharacterInput(int[] array, int start, int length) {
        this.array = array;
        this.pos = start;
        this.endPos = start + length;
      }

      public PartialArrayCharacterInput(int[] array) {
        this.array = array;
        this.pos = 0;
        this.endPos = array.Length;
      }

      public int ReadChar() {
        return (this.pos < this.endPos) ? this.array[this.pos++] : (-1);
      }

      public int Read(int[] buf, int offset, int unitCount) {
        if (unitCount == 0) {
          return 0;
        }
        int maxsize = Math.Min(unitCount, this.endPos - this.pos);
        Array.Copy(this.array, this.pos, buf, offset, maxsize);
        this.pos += maxsize;
        return maxsize == 0 ? -1 : maxsize;
      }
    }
  }
}
