using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
/// <summary>
/// </summary>
public interface ICharacterEncoding {
    /// <summary>Creates an encoder for this character encoding with initial
    /// state.
    /// If the encoder is stateless, multiple calls of this method can return
    /// the
    /// same encoder.</summary>
    /// <returns>Not documented yet.</returns>
  ICharacterEncoder GetEncoder();

    /// <summary>Creates a decoder for this character encoding with initial
    /// state.
    /// If the decoder is stateless, multiple calls of this method can return
    /// the
    /// same decoder.</summary>
    /// <returns>Not documented yet.</returns>
  ICharacterDecoder GetDecoder();
}
}
