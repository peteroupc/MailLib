using System;
using System.IO;
using PeterO;

namespace PeterO.Text {
/// <summary>
/// Defines methods that can be implemented by classes
/// that convert to and from bytes and character code points.
/// </summary>
public interface ICharacterEncoding {
    /// <summary>Creates an encoder for this character encoding with
    /// initial state. If the encoder is stateless, multiple calls of this
    /// method can return the same encoder.</summary>
    /// <returns>Not documented yet.</returns>
  ICharacterEncoder GetEncoder();

    /// <summary>Creates a decoder for this character encoding with initial
    /// state. If the decoder is stateless, multiple calls of this method
    /// can return the same decoder.</summary>
    /// <returns>Not documented yet.</returns>
  ICharacterDecoder GetDecoder();
}
}
