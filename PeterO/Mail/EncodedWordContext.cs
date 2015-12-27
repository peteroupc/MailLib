/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Mail {
    /// <include file='docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.EncodedWordContext"]'/>
  internal enum EncodedWordContext {
    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Mail.EncodedWordContext.Unstructured"]'/>
    Unstructured,

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Mail.EncodedWordContext.Phrase"]'/>
    Phrase,

    /// <include file='docs.xml'
    /// path='docs/doc[@name="F:PeterO.Mail.EncodedWordContext.Comment"]'/>
    Comment
  }
}
