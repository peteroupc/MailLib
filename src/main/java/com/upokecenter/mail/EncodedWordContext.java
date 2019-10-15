package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

  /**
   * Specifies the context where an encoded word under RFC 2047 can appear.
   */
  enum EncodedWordContext {
    /**
     * In an unstructured header field's value.
     */
    Unstructured,

    /**
     * In a "word" element within a "phrase" of a structured header field.
     */
    Phrase,

    /**
     * Contains methods for parsing and matching language tags.
     */
    Comment,
  }
