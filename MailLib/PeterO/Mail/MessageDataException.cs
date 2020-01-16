/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Mail {
  /// <summary>Exception thrown when a message has invalid syntax.
  /// <para>This library may throw exceptions of this type in certain
  /// cases, notably when errors occur, and may supply messages to those
  /// exceptions (the message can be accessed through the <c>Message</c>
  /// property in.NET or the <c>getMessage()</c> method in Java). These
  /// messages are intended to be read by humans to help diagnose the
  /// error (or other cause of the exception); they are not intended to
  /// be parsed by computer programs, and the exact text of the messages
  /// may change at any time between versions of this
  /// library.</para></summary>
#if NET20 || NET40
[Serializable]
#endif
public sealed class MessageDataException : Exception {
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

    #if NET20 || NET40
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.MessageDataException'/> class. Uses the
    /// given serialization and streaming contexts.</summary>
    /// <param name='info'>A System.Runtime.Serialization.SerializationInfo
    /// object.</param>
    /// <param name='context'>A
    /// System.Runtime.Serialization.StreamingContext object.</param>
    private MessageDataException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
      : base(info, context) {
    }
    #endif
  }
}
