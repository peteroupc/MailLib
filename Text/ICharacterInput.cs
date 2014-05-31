using System;

namespace PeterO.Text {
    /// <summary>An interface for reading Unicode characters.</summary>
  public interface ICharacterInput {
    int ReadChar();

    int Read(int[] chars, int index, int length);
  }
}
