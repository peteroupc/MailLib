package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

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
