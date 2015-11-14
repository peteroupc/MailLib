package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;

/// <summary>
/// </summary>
public interface ICharacterDecoder {
    /**
     * Reads bytes from an input transform until a Unicode character is decoded or
     * until the end of the stream is reached.
     * @param input Source of bytes to decode into characters. The decoder can
     * maintain internal state, including data on bytes already read, so
     * this parameter should not change when using the same character
     * decoder object.
     * @return The Unicode code point decoded, from 0-0xd7ff or from 0xe000 to
     * 0x10ffff. Returns -1 if the end of the source is reached or -2 if a
     * decoder error occurs.
     */
  int ReadChar(ITransform input);
}
