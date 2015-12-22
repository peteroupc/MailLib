## PeterO.Mail.Message

    public sealed class Message

Represents an email message, and contains methods and properties for accessing and modifying email message data. This class implements the Internet Message Format (RFC 5322) and Multipurpose Internet Mail Extensions (MIME; RFC 2045-2047, RFC 2049).

Thread safety: This class is mutable; its properties can be changed. None of its instance methods are designed to be thread safe. Therefore, access to objects from this class must be synchronized if multiple threads can access them at the same time.

The following lists known deviations from the mail specifications (Internet Message Format and MIME):

 * The content-transfer-encoding "quoted-printable" is treated as 7bit instead if it occurs in a message or body part with content type "multipart/*" or "message/*" (other than "message/global", "message/global-headers", "message/global-disposition-notification", or "message/global-delivery-status").

 * If a message has two or more Content-Type header fields, it is treated as having a content type of "application/octet-stream", unless one or more of the header fields is syntactically invalid.

 * Illegal UTF-8 byte sequences appearing in header field values are replaced with replacement characters. Moreover, UTF-8 is parsed everywhere in header field values, even in those parts of some structured header fields where this appears not to be allowed. (UTF-8 is a character encoding for the Unicode character set.)

 * The To and Cc header fields are allowed to contain only comments and whitespace, but these "empty" header fields will be omitted when generating.

 * There is no line length limit imposed when parsing header fields, except header field names.

 * There is no line length limit imposed when parsing quoted-printable or base64 encoded bodies.

 * If the transfer encoding is absent and the content type is "message/rfc822" , bytes with values greater than 127 (called "8-bit bytes" in the rest of this summary) are still allowed, despite the default value of "7bit" for "Content-Transfer-Encoding".

 * In the following cases, if the transfer encoding is absent or declared as 7bit, 8-bit bytes are still allowed:

 * (a) The preamble and epilogue of multipart messages, which will be ignored.

 * (b) If the charset is declared to be  `utf-8` .

 * (c) If the content type is "text/html" and the charset is declared to be  `ascii` ,  `us-ascii` , "windows-1252", "windows-1251", or "iso-8859-*" (all single byte encodings).

 * (d) In non-MIME message bodies and in text/plain message bodies. Any 8-bit bytes are replaced with the substitute character byte (0x1a).

 * If the first line of the message starts with the word "From" followed by a space, it is skipped.

 * The name  `ascii`  is treated as a synonym for `us-ascii` , despite being a reserved name under RFC 2046. The name  `cp1252`  is treated as a synonym for  `windows-1252` , even though it's not an IANA registered alias.

 * The following deviations involve encoded words under RFC 2047:

 * (a) If a sequence of encoded words decodes to a string with a CTL character (U + 007F, or a character less than U + 0020 and not TAB) after being converted to Unicode, the encoded words are left un-decoded.

 * (b) This implementation can decode an encoded word that uses ISO-2022-JP (the only supported encoding that uses code switching) even if the encoded word's payload ends in a different mode from "ASCII mode". (Each encoded word still starts in that mode, though.)

 ---.

### Message Constructor

    public Message(
        byte[] bytes);

Initializes a new instance of the Message class. Reads from the given byte array to initialize the message.

<b>Parameters:</b>

 * <i>bytes</i>: A readable data stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

### Message Constructor

    public Message(
        System.IO.Stream stream);

Initializes a new instance of the Message class. Reads from the given Stream object to initialize the message.

<b>Parameters:</b>

 * <i>stream</i>: A readable data stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>stream</i>
 is null.

### BccAddresses

    public System.Collections.Generic.IList BccAddresses { get; }

Gets a list of addresses found in the BCC header field or fields.

<b>Returns:</b>

A list of addresses found in the BCC header field or fields.

### BodyString

    public string BodyString { get; }

Gets the body of this message as a text string.

<b>Returns:</b>

The body of this message as a text string.

<b>Exceptions:</b>

 * System.NotSupportedException:
This message has no character encoding declared on it (which is usually the case for non-text messages), or the character encoding is not supported.

### CCAddresses

    public System.Collections.Generic.IList CCAddresses { get; }

Gets a list of addresses found in the CC header field or fields.

<b>Returns:</b>

A list of addresses found in the CC header field or fields.

### ContentDisposition

    public PeterO.Mail.ContentDisposition ContentDisposition { get; set;}

Gets or sets this message's content disposition. The content disposition specifies how a user agent should handle or otherwise display this message. Can be set to null.

<b>Returns:</b>

This message's content disposition, or null if none is specified.

### ContentType

    public PeterO.Mail.MediaType ContentType { get; set;}

Gets or sets this message's media type.

<b>Returns:</b>

This message's media type.

<b>Exceptions:</b>

 * System.ArgumentNullException:
This value is being set and "value" is null.

### FileName

    public string FileName { get; }

Gets a filename suggested by this message for saving the message's body to a file. For more information on the algorithm, see ContentDisposition.MakeFilename.

<b>Returns:</b>

A suggested name for the file, or the empty string if there is no filename suggested by the content type or content disposition.

### FromAddresses

    public System.Collections.Generic.IList FromAddresses { get; }

Gets a list of addresses found in the From header field or fields.

<b>Returns:</b>

A list of addresses found in the From header field or fields.

### HeaderFields

    public System.Collections.Generic.IList HeaderFields { get; }

Gets a snapshot of the header fields of this message, in the order in which they appear in the message. For each item in the list, the key is the header field's name (where any basic upper-case letters [U + 0041 to U + 005A] are converted to lower case) and the value is the header field's value.

<b>Returns:</b>

A snapshot of the header fields of this message.

### Parts

    public System.Collections.Generic.IList Parts { get; }

Gets a list of all the parts of this message. This list is editable. This will only be used if the message is a multipart message.

<b>Returns:</b>

A list of all the parts of this message. This list is editable. This will only be used if the message is a multipart message.

### Subject

    public string Subject { get; set;}

Gets or sets this message's subject.

<b>Returns:</b>

This message's subject.

### ToAddresses

    public System.Collections.Generic.IList ToAddresses { get; }

Gets a list of addresses found in the To header field or fields.

<b>Returns:</b>

A list of addresses found in the To header field or fields.

### AddHeader

    public PeterO.Mail.Message AddHeader(
        string name,
        string value);

Adds a header field to the end of the message's header.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: Name of a header field, such as "From" or "Content-ID".

 * <i>value</i>: Value of the header field.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 or  <i>value</i>
 is null.

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

### AddHeader

    public PeterO.Mail.Message AddHeader(
        System.Collections.Generic.KeyValuePair header);

Adds a header field to the end of the message's header.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>header</i>: A KeyValuePair object. The key is the name of the header field, such as "From" or "Content-ID". The value is the header field's value.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The key or value of <i>header</i>
 is null.

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

### Generate

    public string Generate();

Generates this message's data in text form.The generated message will have only Basic Latin code points (U + 0000 to U + 007F), and the transfer encoding will always be 7bit, quoted-printable, or base64 (the declared transfer encoding for this message will be ignored).

The following applies to the From, To, Cc, and Bcc header fields. If the header field exists, but has an invalid syntax or has no addresses, this method will generate a synthetic header field with the display-name set to the contents of all of the header fields with the same name, and the address set to `me@[header-name]-address.invalid`  as the address (a `.invalid`  address is a reserved address that can never belong to anyone). The generated message should always have a From header field.

If a Date and/or Message-ID header field doesn't exist, a field with that name will be generated (using the current local time for the Date field).

When encoding the message's body, if the message has a text content type ("text/*"), the line breaks are a CR byte (carriage return, 0x0d) followed by an LF byte (line feed, 0x0a), CR alone, or LF alone. If the message has any other content type, only CR followed by LF is considered a line break.

<b>Returns:</b>

The generated message.

<b>Exceptions:</b>

 * PeterO.Mail.MessageDataException:
The message can't be generated.

### GetBody

    public byte[] GetBody();

Gets the byte array for this message's body. This method doesn't make a copy of that byte array.

<b>Returns:</b>

A byte array.

### GetBodyMessage

    public PeterO.Mail.Message GetBodyMessage();

Returns the mail message contained in this message's body.

<b>Returns:</b>

A message object if this object's content type is "message/rfc822" , "message/news", or "message/global", or null otherwise.

### GetHeader

    public string GetHeader(
        string name);

Gets the first instance of the header field with the specified name, using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic upper-case letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).

<b>Parameters:</b>

 * <i>name</i>: The name of a header field.

<b>Returns:</b>

The value of the first header field with that name, or null if there is none.

<b>Exceptions:</b>

 * System.ArgumentNullException:
Name is null.

### GetHeader

    public System.Collections.Generic.KeyValuePair GetHeader(
        int index);

Gets the name and value of a header field by index.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to get.

<b>Returns:</b>

A KeyValuePair object. The key is the name of the header field, such as "From" or "Content-ID". The value is the header field's value.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>index</i>
 is 0 or at least as high as the number of header fields.

### GetHeaderArray

    public string[] GetHeaderArray(
        string name);

Gets an array with the values of all header fields with the specified name, using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic upper-case letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).

<b>Parameters:</b>

 * <i>name</i>: The name of a header field.

<b>Returns:</b>

An array containing the values of all header fields with the given name, in the order they appear in the message. The array will be empty if no header field has that name.

<b>Exceptions:</b>

 * System.ArgumentNullException:
Name is null.

### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        int index);

Removes a header field by index.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>index</i>
 is 0 or at least as high as the number of header fields.

### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        string name);

Removes all instances of the given header field from this message. If this is a multipart message, the header field is not removed from its body part headers. A basic case-insensitive comparison is used. (Two strings are equal in such a comparison, if they match after converting the basic upper-case letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: The name of the header field to remove.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

### SetBody

    public PeterO.Mail.Message SetBody(
        byte[] bytes);

Sets the body of this message to the given byte array. This method doesn't make a copy of that byte array.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array.

<b>Returns:</b>

This object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

### SetCurrentDate

    public PeterO.Mail.Message SetCurrentDate();

Sets this message's Date header field to the current time as its value.This method can be used when the message is considered complete and ready to be generated, for example, using the "Generate()" method.

<b>Returns:</b>

This object.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        string name,
        string value);

Sets the name and value of a header field by index.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

 * <i>name</i>: Name of a header field, such as "From" or "Content-ID".

 * <i>value</i>: Value of the header field.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>index</i>
 is 0 or at least as high as the number of header fields; or, the header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

 * System.ArgumentNullException:
The parameter  <i>name</i>
 or  <i>value</i>
 is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        string value);

Sets the value of a header field by index without changing its name.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

 * <i>value</i>: Value of the header field.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>index</i>
 is 0 or at least as high as the number of header fields; or, the header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

 * System.ArgumentNullException:
The parameter  <i>value</i>
 is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        System.Collections.Generic.KeyValuePair header);

Sets the name and value of a header field by index.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

 * <i>header</i>: A KeyValuePair object. The key is the name of the header field, such as "From" or "Content-ID". The value is the header field's value.

<b>Returns:</b>

A Message object.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter  <i>index</i>
 is 0 or at least as high as the number of header fields; or, the header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

 * System.ArgumentNullException:
The key or value of <i>header</i>
 is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        string name,
        string value);

Sets the value of this message's header field. If a header field with the same name exists, its value is replaced. If the header field's name occurs more than once, only the first instance of the header field is replaced.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: The name of a header field, such as "from" or "subject".

 * <i>value</i>: The header field's value.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

 * System.ArgumentNullException:
The parameter  <i>name</i>
 or  <i>value</i>
 is null.

### SetHtmlBody

    public PeterO.Mail.Message SetHtmlBody(
        string str);

Sets the body of this message to the specified string in HTML format. The character sequences CR (carriage return, "\r", U+000D), LF (line feed, "\n" , U+000A), and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code points will be replaced with replacement characters.

<b>Parameters:</b>

 * <i>str</i>: A string consisting of the message in HTML format.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

### SetTextAndHtml

    public PeterO.Mail.Message SetTextAndHtml(
        string text,
        string html);

Sets the body of this message to a multipart body with plain text and HTML versions of the same message. The character sequences CR (carriage return, "\r" , U+000D), LF (line feed, "\n", U + 000A), and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code points will be replaced with replacement characters.

<b>Parameters:</b>

 * <i>text</i>: A string consisting of the plain text version of the message.

 * <i>html</i>: A string consisting of the HTML version of the message.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>text</i>
 or  <i>html</i>
 is null.

### SetTextBody

    public PeterO.Mail.Message SetTextBody(
        string str);

Sets the body of this message to the specified plain text string. The character sequences CR (carriage return, "\r", U+000D), LF (line feed, "\n" , U+000A), and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code points will be replaced with replacement characters. This method changes this message's media type to plain text.

<b>Parameters:</b>

 * <i>str</i>: A string consisting of the message in plain text format.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.
