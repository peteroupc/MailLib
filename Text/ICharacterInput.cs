using System;

namespace PeterO.Text {
    /// <summary>An interface for reading Unicode characters from a data
    /// source.</summary>
  public interface ICharacterInput {
    /// <summary>Reads a Unicode character from a data source.</summary>
    /// <returns>Either a Unicode code point (from 0-0xd7ff or from 0xe000
    /// to 0x10ffff), or the value -1 indicating the end of the
    /// source.</returns>
    int ReadChar();

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
    /// <exception cref='ArgumentNullException'>Should be thrown if "chars"
    /// is null.</exception>
    int Read(int[] chars, int index, int length);
  }
}
