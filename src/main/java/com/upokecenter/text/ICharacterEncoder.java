package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

    /**
     * Defines a method that can be implemented by classes that convert Unicode
     * code points to bytes.
     */
  public interface ICharacterEncoder {
    /**
     * Converts a Unicode code point to bytes and writes the bytes to an output
     * stream.
     * @param c Either a Unicode code point (from 0-0xd7ff or from 0xe000 to
     * 0x10ffff), or the value -1 indicating the end of the stream.
     * @param output Output stream where the converted bytes will be written. The
     * decoder can maintain internal state, including data on bytes already
     * ((passed instanceof input) ? (input)passed : null), so this parameter
     * should not change when using the same character encoder object.
     * @return The number of bytes written to the stream; -1 if no further code
     * points remain (for example, if _c_ is -1 indicating the end of the
     * stream), or -2 if an encoding error occurs. (Note that it's possible
     * for this method to return 0 if, for example, it can't generate new
     * bytes yet based on the current input.).
     */
    int Encode(int c, IWriter output);
  }
