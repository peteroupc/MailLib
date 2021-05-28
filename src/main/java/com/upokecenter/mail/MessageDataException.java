package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/

 */

  /**
   * Exception thrown when a message has invalid syntax. <p>This library may
   * throw exceptions of this type in certain cases, notably when errors
   * occur, and may supply messages to those exceptions (the message can be
   * accessed through the <code>Message</code> property in.NET or the
   * <code>getMessage()</code> method in Java). These messages are intended to be
   * read by humans to help diagnose the error (or other cause of the
   * exception); they are not intended to be parsed by computer programs,
   * and the exact text of the messages may change at any time between
   * versions of this library.</p>
   */

public final class MessageDataException extends RuntimeException {
private static final long serialVersionUID = 1L;
    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MessageDataException} class.
     */
    public MessageDataException() {
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MessageDataException} class.
     * @param message A string to use as the exception message.
     */
    public MessageDataException(String message) {
 super(message);
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MessageDataException} class.
     * @param message A string to use as the exception message.
     * @param innerException The parameter {@code innerException} is an Exception
     * object.
     */
    public MessageDataException(String message, Throwable innerException) {
 super(message);
initCause(innerException);;
    }
  }
