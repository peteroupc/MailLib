/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Mail {
    /// <summary>Exception thrown when a message has invalid
    /// syntax.</summary>
  public class MessageDataException : Exception {
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.MessageDataException'/> class.</summary>
    public MessageDataException() {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.MessageDataException'/> class.</summary>
    /// <param name='message'>A string to use as the exception
    /// message.</param>
    public MessageDataException(string message) : base(message) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.MessageDataException'/> class.</summary>
    /// <param name='message'>A string to use as the exception
    /// message.</param>
    /// <param name='innerException'>The parameter <paramref
    /// name='innerException'/> is an Exception object.</param>
    public MessageDataException(string message, Exception innerException)
      : base(message, innerException) {
    }
  }
}
