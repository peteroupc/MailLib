## PeterO.Mail.MessageDataException

    public sealed class MessageDataException :
        System.Exception,
        System.Runtime.InteropServices._Exception,
        System.Runtime.Serialization.ISerializable

Exception thrown when a message has invalid syntax. This library may throw exceptions of this type in certain cases, notably when errors occur, and may supply messages to those exceptions (the message can be accessed through the  `Message`  property in.NET or the  `getMessage()`  method in Java). These messages are intended to be read by humans to help diagnose the error (or other cause of the exception); they are not intended to be parsed by computer programs, and the exact text of the messages may change at any time between versions of this library.

### Member Summary

<a id="Void_ctor_System_String"></a>
### MessageDataException Constructor

    public MessageDataException(
        string message);

Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.

<b>Parameters:</b>

 * <i>message</i>: A string to use as the exception message.

<a id="Void_ctor_System_String_System_Exception"></a>
### MessageDataException Constructor

    public MessageDataException(
        string message,
        System.Exception innerException);

Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.

<b>Parameters:</b>

 * <i>message</i>: A string to use as the exception message.

 * <i>innerException</i>: The parameter  <i>innerException</i>
 is an Exception object.

<a id="Void_ctor"></a>
### MessageDataException Constructor

    public MessageDataException();

Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.
