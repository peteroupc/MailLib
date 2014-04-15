/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail
{
  internal enum PhraseTextMode {
    /// <summary>Return the decoded text of the phrase.</summary>
    DecodedText,

    /// <summary>Return the un-decoded text of the phrase.</summary>
    UndecodedText,

    /// <summary>Return the decoded text of the phrase and its comments.</summary>
    DecodedTextAndComments
  }
}
