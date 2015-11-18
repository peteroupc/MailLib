package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;

    /**
     * Defines a method that can be implemented by classes that convert a stream of
     * bytes to Unicode code points.
     */
public interface ICharacterDecoder {
    /**
     * Reads bytes from an input transform until a Unicode character is decoded or
     * until the end of the stream is reached. <p>If this method returns -2,
     * indicating an error, the caller of this method can take one of a
     * variety of actions to handle the error. For example, it can output a
     * replacement code point instead, or it can throw an exception. In some
     * cases, where the error won't cause data loss or a security problem,
     * the caller can also ignore the decoder error.</p>
     * @param input Source of bytes to decode into characters. The decoder can
     * maintain internal state, including data on bytes already read, so
     * this parameter should not change when using the same character
     * decoder object.
     * @return The Unicode code point decoded, from 0-0xd7ff or from 0xe000 to
     * 0x10ffff. Returns -1 if the end of the source is reached or -2 if a
     * decoder error occurs.
     */
  int ReadChar(IByteReader input);
}
