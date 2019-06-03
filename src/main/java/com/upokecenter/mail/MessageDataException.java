package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

    /**
     * Exception thrown when a message has invalid syntax.
     */
  public class MessageDataException extends RuntimeException {
private static final long serialVersionUID = 1L;
    /**
     * Initializes a new instance of the {@link MessageDataException} class.
     */
    public MessageDataException() {
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MessageDataException} class.
     * @param message The parameter {@code message} is a text string.
     */
    public MessageDataException(String message) {
 super(message);
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MessageDataException} class.
     * @param message The parameter {@code message} is a text string.
     * @param innerException The parameter {@code innerException} is an Exception
     * object.
     */
    public MessageDataException(String message, Throwable innerException) {
 super(message, innerException);
    }
  }
