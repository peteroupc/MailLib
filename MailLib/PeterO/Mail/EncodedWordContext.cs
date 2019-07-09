/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.EncodedWordContext"]/*'/>
  internal enum EncodedWordContext {
    /// <xmlbegin id='1'/>
    /// <summary>In an unstructured header field's value.</summary>
    Unstructured,

    /// <xmlbegin id='2'/>
    /// <summary>In a "word" element within a "phrase" of a structured
    /// header field.</summary>
    Phrase,

    /// <xmlbegin id='3'/>
    /// <summary>Contains methods for parsing and matching language
    /// tags.</summary>
    Comment,
  }
}
