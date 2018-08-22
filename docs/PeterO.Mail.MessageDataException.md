## PeterO.Mail.MessageDataException

    public class MessageDataException :
        System.Exception,
        System.Runtime.InteropServices._Exception,
        System.Runtime.Serialization.ISerializable

Exception thrown when a message has invalid syntax.

### MessageDataException Constructor

    public MessageDataException(
        string message);

Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.

<b>Parameters:</b>

 * <i>message</i>: A string to use as the exception message.

### MessageDataException Constructor

    public MessageDataException(
        string message,
        System.Exception innerException);

Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.

<b>Parameters:</b>

 * <i>message</i>: A string to use as the exception message.

 * <i>innerException</i>: The parameter <i>innerException</i>
is an Exception object.

### MessageDataException Constructor

    public MessageDataException();

Initializes a new instance of the [PeterO.Mail.MessageDataException](PeterO.Mail.MessageDataException.md) class.
