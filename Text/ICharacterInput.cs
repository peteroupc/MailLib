using System;

namespace PeterO.Text {
    /// <summary>An interface for reading Unicode characters.</summary>
  public interface ICharacterInput {
 /// <summary>Reads a Unicode character from a data source.</summary>
 /// <returns>The Unicode character read, from U+0000 to U+10FFFF.  Returns
 /// -1 if the end of the source is reached.</returns>
    int ReadChar();

 /// <summary>Reads a sequence of Unicode characters from a data source.</summary>
    int Read(int[] chars, int index, int length);
  }
}
