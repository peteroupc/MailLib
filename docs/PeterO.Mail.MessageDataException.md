## PeterO.Mail.MessageDataException

    public class MessageDataException :
        System.Exception,
        System.Runtime.InteropServices._Exception,
        System.Runtime.Serialization.ISerializable

 Exception thrown when a message has invalid syntax.

### Member Summary

<a id="Void_ctor_String"></a>
### MessageDataException Constructor

    public MessageDataException(
        string message);

 Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.

<b>Parameters:</b>

 * <i>message</i>: The parameter  <i>message</i>
 is a text string.

<a id="Void_ctor_String_Exception"></a>
### MessageDataException Constructor

    public MessageDataException(
        string message,
        System.Exception innerException);

 Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.

<b>Parameters:</b>

 * <i>message</i>: The parameter  <i>message</i>
 is a text string.

 * <i>innerException</i>: The parameter  <i>innerException</i>
 is an Exception object.

<a id="Void_ctor"></a>
### MessageDataException Constructor

    public MessageDataException();

 Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.
