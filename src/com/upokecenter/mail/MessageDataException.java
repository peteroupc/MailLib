package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Exception thrown when a message has invalid syntax.
     */
  public class MessageDataException extends RuntimeException {
private static final long serialVersionUID=1L;
    public MessageDataException () {
    }

    public MessageDataException (String message) {
 super(message);
    }

    public MessageDataException (String message, Throwable innerException) {
 super(message,innerException);
    }
  }
