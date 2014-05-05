using System;

namespace PeterO.Text {
    /// <summary>Not documented yet.</summary>
  public interface ICharacterInput {
    int ReadChar();

    int Read(int[] chars, int index, int length);
  }
}
