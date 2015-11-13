using System;
using System.IO;
using PeterO;
using PeterO.Mail;
using PeterO.Text;
namespace PeterO.Text.Encoders {
/// <summary>
/// </summary>
public interface ICharacterDecoder {
    /// <summary>Reads bytes from an input transform until a Unicode character is
    /// decoded or until the end of the stream is reached.</summary>
    /// <param name='input'>Source of bytes to decode into characters. The decoder
    /// can maintain internal state, including data on bytes already read, so this
    /// parameter should not change when using the same character decoder
    /// object.</param>
    /// <returns>The Unicode character decoded, from U + 0000 to U + 10FFFF. Returns
    /// -1 if the end of the source is reached or -2 if a decoder error
    /// occurs.</returns>
  int ReadChar(ITransform input);
}
}
