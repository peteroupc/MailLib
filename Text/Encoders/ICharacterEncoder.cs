using System;
using System.IO;
using PeterO;
using PeterO.Mail;
using PeterO.Text;
namespace PeterO.Text.Encoders {
/// <summary>
/// </summary>
public interface ICharacterEncoder {
    /// <summary>Converts a Unicode code point to bytes and writes them to an output
    /// stream.</summary>
    /// <returns>The number of bytes written to the stream; -1 if no further code
    /// points remain (for example, if _c_ is -1 indicating the end of the stream),
    /// or -2 if an encoding error occurs.</returns>
  int Encode(int c, Stream output);
}
}
