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
  /// path='docs/doc[@name="T:PeterO.Mail.MessageDataException"]/*'/>
  public class MessageDataException : Exception
  {
    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Mail.MessageDataException.#ctor"]/*'/>
    public MessageDataException() {
    }

    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Mail.MessageDataException.#ctor(System.String)"]/*'/>
    public MessageDataException(string message) : base(message) {
    }

    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Mail.MessageDataException.#ctor(System.String,System.Exception)"]/*'/>
    public MessageDataException(string message, Exception innerException) :
      base(message, innerException) {
    }
  }
}
