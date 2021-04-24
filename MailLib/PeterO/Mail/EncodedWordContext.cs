/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Mail {
  /// <summary>Specifies the context where an encoded word under RFC 2047
  /// can appear.</summary>
  internal enum EncodedWordContext {
    /// <summary>In an unstructured header field's value.</summary>
    Unstructured,

    /// <summary>In a "word" element within a "phrase" of a structured
    /// header field.</summary>
    Phrase,

    /// <summary>Contains methods for parsing and matching language
    /// tags.</summary>
    Comment,
  }
}
