## PeterO.Mail.MessageDataException

    public class MessageDataException :
        System.Exception,
        System.Runtime.Serialization.ISerializable,
        System.Runtime.InteropServices._Exception

Exception thrown when a message has invalid syntax.

### MessageDataException Constructor

    public MessageDataException();

Initializes a new instance of the MessageDataException class.

### MessageDataException Constructor

    public MessageDataException(
        string message);

Initializes a new instance of the MessageDataException class.

<b>Parameters:</b>

 * <i>message</i>: A string object.

### MessageDataException Constructor

    public MessageDataException(
        string message,
        System.Exception innerException);

Initializes a new instance of the MessageDataException class.

<b>Parameters:</b>

 * <i>message</i>: A string object.

 * <i>innerException</i>: An Exception object.
