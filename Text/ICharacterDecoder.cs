using System;
using System.IO;
using PeterO;

namespace PeterO.Text {
    /// <summary>Defines a method that can be implemented by classes that
    /// convert a stream of bytes to Unicode code points.</summary>
public interface ICharacterDecoder {
    /// <summary>Reads bytes from an input transform until a Unicode
    /// character is decoded or until the end of the stream is
    /// reached.</summary>
    /// <param name='input'>Source of bytes to decode into characters. The
    /// decoder can maintain internal state, including data on bytes
    /// already read, so this parameter should not change when using the
    /// same character decoder object.</param>
    /// <returns>The Unicode code point decoded, from 0-0xd7ff or from
    /// 0xe000 to 0x10ffff. Returns -1 if the end of the source is reached
    /// or -2 if a decoder error occurs.</returns>
  int ReadChar(ITransform input);
}
}
