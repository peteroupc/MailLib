/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

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
