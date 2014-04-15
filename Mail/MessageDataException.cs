/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Mail
{
    /// <summary>Exception thrown when a message has invalid syntax.</summary>
  public class MessageDataException : Exception
  {
    public MessageDataException() {
    }

    public MessageDataException(string message) : base(message) {
    }

    public MessageDataException(string message, Exception innerException) : base(message, innerException) {
    }
  }
}
