/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
