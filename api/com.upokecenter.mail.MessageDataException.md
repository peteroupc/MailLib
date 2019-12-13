# com.upokecenter.mail.MessageDataException

    public final class MessageDataException extends java.lang.RuntimeException

Exception thrown when a message has invalid syntax. <p>This library may
 throw exceptions of this type in certain cases, notably when errors
 occur, and may supply messages to those exceptions (the message can be
 accessed through the <code>Message</code> property in.NET or the
 <code>getMessage()</code> method in Java). These messages are intended to be
 read by humans to help diagnose the error (or other cause of the
 exception); they are not intended to be parsed by computer programs,
 and the exact text of the messages may change at any time between
 versions of this library.</p>

## Methods

* `MessageDataException() MessageDataException`<br>
 Initializes a new instance of the MessageDataException class.
* `MessageDataException​(java.lang.String message) MessageDataException`<br>
 Initializes a new instance of the MessageDataException class.
* `MessageDataException​(java.lang.String message,
                    java.lang.Throwable innerException) MessageDataException`<br>
 Initializes a new instance of the MessageDataException class.

## Constructors

* `MessageDataException() MessageDataException`<br>
 Initializes a new instance of the MessageDataException class.
* `MessageDataException​(java.lang.String message) MessageDataException`<br>
 Initializes a new instance of the MessageDataException class.
* `MessageDataException​(java.lang.String message,
                    java.lang.Throwable innerException) MessageDataException`<br>
 Initializes a new instance of the MessageDataException class.
