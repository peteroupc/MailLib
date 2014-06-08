## PeterO.Mail.Message

    public sealed class Message

Represents an email message, and contains methods and properties for accessing and modifying email message data. This class implements the Internet Message Format (RFC 5322) and Multipurpose Internet Mail Extensions (MIME; RFC 2045-2047, RFC 2049). 

Thread safety:This class is mutable; its properties can be changed. None of its instance methods are designed to be thread safe. Therefore, access to objects from this class must be synchronized if multiple threads can access them at the same time.

The following lists known deviations from the mail specifications (Internet Message Format and MIME):

 * The content-transfer-encoding "quoted-printable" is treated as 7bit instead if it occurs in a message or body part with content type "multipart/*" or "message/*" (other than "message/global", "message/global-headers", "message/global-disposition-notification", or "message/global-delivery-status").

 * If a message has two or more Content-Type header fields, it is treated as having a content type of "application/octet-stream", unless one or more of the header fields is syntactically invalid.

 * Non-UTF-8 bytes appearing in header field values are replaced with replacement characters. Moreover, UTF-8 is parsed everywhere in header field values, even in those parts of some structured header fields where this appears not to be allowed.

 * The To and Cc header fields are allowed to contain only comments and whitespace, but these "empty" header fields will be omitted when generating.

 * There is no line length limit imposed when parsing quoted-printable or base64 encoded bodies.

 * In non-MIME message bodies, in text/plain message bodies, and in the prologue and epilogue of multipart messages (which will be ignored), if the transfer encoding is absent or declared as 7bit, any 8-bit bytes are replaced with the ASCII substitute character (0x1a).

 * If the transfer encoding is absent or declared as 7bit, and the charset is declared to be  `utf-8` , the transfer encoding is treated as 8bit instead.

 * In text/html message bodies, if the transfer encoding is absent or declared as 7bit, and the charset is declared to be  `ascii` ,  `us-ascii` , "windows-1252", "windows-1251", or "iso-8859-*" (all single byte encodings), the transfer encoding is treated as 8bit instead.

 * If the first line of the message starts with the word "From" followed by a space, it is skipped.

 * The name  `ascii` is treated as a synonym for  `us-ascii` , despite being a reserved name under RFC 2046. The name  `cp1252` is treated as a synonym for  `windows-1252` , even though it's not an IANA registered alias.

 * If a sequence of encoded words (RFC 2047) decodes to a string with a CTL character (U + 007F, or a character less than U + 0020 and not TAB) after being converted to Unicode, the encoded words are left un-decoded.

 * This implementation can decode an encoded word that uses ISO-2022-JP (the only supported encoding that uses code switching) even if the encoded word's payload ends in a different mode from ASCII mode. (Each encoded word still starts in ASCII mode, though.)

### GetBody

    public byte[] GetBody();

Gets the byte array for this message's body.

<b>Returns:</b>

A byte array.

### SetBody

    public void SetBody(
        byte[] bytes);

Sets the body of this message to the given byte array.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
Bytes is null.

### SetTextBody

    public PeterO.Mail.Message SetTextBody(
        string str);

Sets the body of this message to the specified plain text string. The character sequences CR, LF, and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code points will be replaced with replacement characters.

<b>Parameters:</b>

 * <i>str</i>: A string object.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.

### SetHtmlBody

    public PeterO.Mail.Message SetHtmlBody(
        string str);

Sets the body of this message to the specified string in HTML format. The character sequences CR, LF, and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code points will be replaced with replacement characters.

<b>Parameters:</b>

 * <i>str</i>: A string object.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.

### SetTextAndHtml

    public PeterO.Mail.Message SetTextAndHtml(
        string text,
        string html);

Sets the body of this message to a multipart body with plain text and HTML versions of the same message. The character sequences CR, LF, and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code points will be replaced with replacement characters.

<b>Parameters:</b>

 * <i>text</i>: A string object.

 * <i>html</i>: A string object. (2).

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>text</i>
 or  <i>html</i>
 is null.

### Message Constructor

    public Message(
        System.IO.Stream stream);

Initializes a new instance of the Message class. Reads from the given Stream object to initialize the message.

<b>Parameters:</b>

 * <i>stream</i>: A readable data stream.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>stream</i>
 is null.

### GetBodyMessage

    public PeterO.Mail.Message GetBodyMessage();

Returns the mail message contained in this message's body.

<b>Returns:</b>

A message object if this object's content type is "message/rfc822", "message/news", or "message/global", or null otherwise.

### GetHeader

    public string GetHeader(
        string name);

Gets the first instance of the header field with the specified name, comparing the field name in an ASCII case-insensitive manner.

<b>Parameters:</b>

 * <i>name</i>: The name of a header field.

<b>Returns:</b>

The value of the first header field with that name, or null if there is none.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
Name is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        string name,
        string value);

Sets the value of this message's header field. If a header field with the same name exists, its value is replaced.

<b>Parameters:</b>

 * <i>name</i>: The name of a header field, such as "from" or "subject".

 * <i>value</i>: The header field's value.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException: 
The header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

 * System.ArgumentNullException: 
The parameter <i>name</i>
 or  <i>value</i>
 is null.

### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        string name);

Removes all instances of the given header field from this message. If this is a multipart message, the header field is not removed from its body part headers.

<b>Parameters:</b>

 * <i>name</i>: The name of the header field to remove.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>name</i>
 is null.

### Generate

    public string Generate();

Generates this message's data in text form.The generated message will always be 7-bit ASCII, and the transfer encoding will always be 7bit, quoted-printable, or base64 (the declared transfer encoding for this message will be ignored).

The following applies to the From, To, Cc, and Bcc header fields. If the header field has an invalid syntax or has no addresses, this method will generate a synthetic header field with the display-name set to the contents of all of the header fields with the same name, and the address set to  `me@[header-name]-address.invalid` as the address (a  `.invalid` address is a reserved address that can never belong to anyone). 

<b>Returns:</b>

The generated message.

<b>Exceptions:</b>

 * PeterO.Mail.MessageDataException: 
The message can't be generated.

### Parts

    public System.Collections.Generic.IList Parts { get; }

Gets a list of all the parts of this message. This list is editable. This will only be used if the message is a multipart message.

<b>Returns:</b>

A list of all the parts of this message. This list is editable. This will only be used if the message is a multipart message.

### Headers

    public System.Collections.Generic.IList Headers { get; }

<b>Deprecated.</b> Use HeaderFields instead.

Gets a snapshot of the header fields of this message. The list contains an alternating set of header field names and values, in the order they were declared in the message.

<b>Returns:</b>

A snapshot of the header fields of this message. The list contains an alternating set of header field names and values, in the order they were declared in the message.

### HeaderFields

    public System.Collections.Generic.IList HeaderFields { get; }

Gets a snapshot of the header fields of this message. For each item in the list, the key is the header field's name and the value is its value.

<b>Returns:</b>

A snapshot of the header fields of this message.

### FromAddresses

    public System.Collections.Generic.IList FromAddresses { get; }

Gets a list of addresses found in the From header field or fields.

<b>Returns:</b>

A list of addresses found in the From header field or fields.

### ToAddresses

    public System.Collections.Generic.IList ToAddresses { get; }

Gets a list of addresses found in the To header field or fields.

<b>Returns:</b>

A list of addresses found in the To header field or fields.

### CCAddresses

    public System.Collections.Generic.IList CCAddresses { get; }

Gets a list of addresses found in the CC header field or fields.

<b>Returns:</b>

A list of addresses found in the CC header field or fields.

### BccAddresses

    public System.Collections.Generic.IList BccAddresses { get; }

Gets a list of addresses found in the BCC header field or fields.

<b>Returns:</b>

A list of addresses found in the BCC header field or fields.

### Subject

    public string Subject { get; set;}

Gets or sets this message's subject.

<b>Returns:</b>

This message's subject.

### BodyString

    public string BodyString { get; }

Gets the body of this message as a Unicode string.

<b>Returns:</b>

The body of this message as a Unicode string.

<b>Exceptions:</b>

 * System.NotSupportedException: 
This message has no character encoding declared on it, or the character encoding is not supported.

### ContentType

    public PeterO.Mail.MediaType ContentType { get; set;}

Gets or sets this message's media type.

<b>Returns:</b>

This message's media type.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
This value is being set and "value" is null.

### ContentDisposition

    public PeterO.Mail.ContentDisposition ContentDisposition { get; set;}

Gets or sets this message's content disposition. The content disposition specifies how a user agent should handle or otherwise display this message.

<b>Returns:</b>

This message's content disposition, or null if none is specified.

### FileName

    public string FileName { get; }

Gets a filename suggested by this message for saving the message's body to a file. For more information on the algorithm, see ContentDisposition.MakeFilename.

<b>Returns:</b>

A suggested name for the file, or the empty string if there is no filename suggested by the content type or content disposition.


