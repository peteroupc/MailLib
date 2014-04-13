/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail
{
    /// <summary>Specifies the context where an encoded word under RFC 2047
    /// can appear.</summary>
  internal enum EncodedWordContext {
    /// <summary>In an unstructured header field's value.</summary>
    Unstructured,

    /// <summary>In a "word" element within a "phrase" of a structured header
    /// field.</summary>
    Phrase,

    /// <summary>In a comment within a structured header field.</summary>
    Comment
  }
}
