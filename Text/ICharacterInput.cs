using System;

namespace PeterO.Text {
    /// <summary>An interface for reading Unicode characters from a data source.</summary>
  public interface ICharacterInput {
    /// <summary>Reads a Unicode character from a data source.</summary>
    /// <returns>The Unicode character read, from U + 0000 to U + 10FFFF. Returns
    /// -1 if the end of the source is reached.</returns>
    int ReadChar();

    /// <summary>Reads a sequence of Unicode code points from a data source.</summary>
    /// <returns>The number of Unicode code points read, or 0 if the end of the
    /// source is reached.</returns>
    int Read(int[] chars, int index, int length);
  }
}
