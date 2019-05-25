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
    /// <summary>Initializes a new instance of the MessageDataException
    /// class.</summary>
    public MessageDataException() {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='MessageDataException'/> class.</summary>
    /// <param name='message'>A string object.</param>
    public MessageDataException(string message) : base(message) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='MessageDataException'/> class.</summary>
    /// <param name='message'>A string object.</param>
    /// <param name='innerException'>An Exception object.</param>
    public MessageDataException(string message, Exception innerException) :
      base(message, innerException) {
    }
  }
}
