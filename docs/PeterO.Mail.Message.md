## PeterO.Mail.Message

    public sealed class Message

Represents an email message, and contains methods and properties for ccessing and modifying email message data. This class implements the nternet Message Format (RFC 5322) and Multipurpose Internet Mail xtensions (MIME; RFC 2045-2047, RFC 2049).

<b>Thread safety:</b>This class is mutable; its properties can be changed. None of its nstance methods are designed to be thread safe. Therefore, access to bjects from this class must be synchronized if multiple threads can ccess them at the same time.

The following lists known deviations from the mail specifications Internet Message Format and MIME):

 * The content-transfer-encodings "quoted-printable" and "base64" are reated as 7bit instead if they occur in a message or body part with ontent type "multipart/*" or "message/*" (other than message/global", "message/global-headers", message/global-disposition-notification", or message/global-delivery-status").

 * If a message has two or more Content-Type header fields, it is reated as having a content type of "application/octet-stream", unless ne or more of the header fields is syntactically invalid.

 * Illegal UTF-8 byte sequences appearing in header field values are eplaced with replacement characters. Moreover, UTF-8 is parsed verywhere in header field values, even in those parts of some tructured header fields where this appears not to be allowed. (UTF-8 s a character encoding for the Unicode character set.)

 * This implementation can parse a message even if that message is ithout a From header field, without a Date header field, or without oth.

 * The To and Cc header fields are allowed to contain only comments and hitespace, but these "empty" header fields will be omitted when enerating.

 * There is no line length limit imposed when parsing header fields, xcept header field names.

 * There is no line length limit imposed when parsing quoted-printable r base64 encoded bodies.

 * If the transfer encoding is absent and the content type is message/rfc822", bytes with values greater than 127 (called "8-bit ytes" in the rest of this summary) are still allowed, despite the efault value of "7bit" for "Content-Transfer-Encoding".

 * In the following cases, if the transfer encoding is absent, declared s 7bit, or treated as 7bit, 8-bit bytes are still allowed:

 * (a) The preamble and epilogue of multipart messages, which will be gnored.

 * (b) If the charset is declared to be `
            utf-8
          ` .

 * (c) If the content type is "text/html" and the charset is declared to e `
            us-ascii
          ` , "windows-1252", "windows-1251", or "iso-8859-*" (all single byte ncodings).

 * (d) In non-MIME message bodies and in text/plain message bodies. Any -bit bytes are replaced with the substitute character byte (0x1a).

 * If the message starts with the word "From" (and no other case ariations of that word) followed by one or more space (U+0020) not ollowed by colon, that text and the rest of the text is skipped up to nd including a line feed (U+000A). (See also RFC 4155, which escribes the so-called "mbox" convention with "From" lines of this ind.)

 * The name `
            ascii
          ` is treated as a synonym for `
            us-ascii
          ` , despite being a reserved name under RFC 2046. The name `
            cp1252
          ` is treated as a synonym for `
            windows-1252
          ` , even though it's not an IANA registered alias.

 * The following deviations involve encoded words under RFC 2047:

 * (a) If a sequence of encoded words decodes to a string with a CTL haracter (U+007F, or a character less than U+0020 and not TAB) after eing converted to Unicode, the encoded words are left un-decoded.

 * (b) This implementation can decode encoded words regardless of the haracter length of the line in which they appear. This implementation an generate a header field line with one or more encoded words even f that line is more than 76 characters long. (This implementation ollows the recommendation in RFC 5322 to limit header field lines to o more than 78 characters, where possible.)

It would be appreciated if users of this library contact the author if hey find other ways in which this implementation deviates from the mail pecifications or other applicable specifications.

Note that this class currently doesn't support the "padding" parameter or message bodies with the media type "application/octet-stream" or reated as that media type (see RFC 2046 sec. 4.5.1).

Note that this implementation can decode an RFC 2047 encoded word that ses ISO-2022-JP (the only supported encoding that uses code switching) ven if the encoded word's payload ends in a different mode from "ASCII ode". (Each encoded word still starts in "ASCII mode", though.) This, owever, is not a deviation to RFC 2047 because the relevant rule only oncerns bringing the output device back to "ASCII mode" after the ecoded text is displayed (see last paragraph of sec. 6.2) -- since the ecoded text is converted to Unicode rather than kept as ISO-2022-JP, his is not applicable since there is no such thing as "ASCII mode" in he Unicode Standard.

Note that this library (the MailLib library) has no facilities for ending and receiving email messages, since that's outside this ibrary's scope.

### Message Constructor

    public Message(
        byte[] bytes);

Initializes a new instance of the[PeterO.Mail.Message](PeterO.Mail.Message.md)class. Reads from the given byte array to initialize the message.

<b>Parameters:</b>

 * <i>bytes</i>: A readable data stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>bytes</i>
is null.

### Message Constructor

    public Message(
        System.IO.Stream stream);

Initializes a new instance of the[PeterO.Mail.Message](PeterO.Mail.Message.md)class. Reads from the given Stream object to initialize the message.

<b>Parameters:</b>

 * <i>stream</i>: A readable data stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>stream</i>
is null.

### BccAddresses

    public System.Collections.Generic.IList BccAddresses { get; }

<b>Deprecated.</b> Use GetAddresses("Bcc") instead.

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
Either this message is a multipart message, so it doesn't have its own ody text, or this message has no character encoding declared or assumed or it (which is usually the case for non-text messages), or the character ncoding is not supported.

### CCAddresses

    public System.Collections.Generic.IList CCAddresses { get; }

<b>Deprecated.</b> Use GetAddresses("Cc") instead.

Gets a list of addresses found in the CC header field or fields.

<b>Returns:</b>

A list of addresses found in the CC header field or fields.

### ContentDisposition

    public PeterO.Mail.ContentDisposition ContentDisposition { get; set;}

Gets or sets this message's content disposition. The content disposition pecifies how a user agent should display or otherwise handle this essage. Can be set to null. If set to a disposition or to null, updates he Content-Disposition header field as appropriate.

<b>Returns:</b>

This message's content disposition, or null if none is specified.

### ContentType

    public PeterO.Mail.MediaType ContentType { get; set;}

Gets or sets this message's media type. When getting, the media type may iffer in certain cases from the value of the Content-Type header field, f any, and may have a value even if the Content-Type header field is bsent from this message. If set to a media type, updates the Content-Type eader field as appropriate. Cannot be set to null.

<b>Returns:</b>

This message's media type.

<b>Exceptions:</b>

 * System.ArgumentNullException:
This value is being set and "value" is null.

### FileName

    public string FileName { get; }

Gets a file name suggested by this message for saving the message's ody to a file. For more information on the algorithm, see ontentDisposition.MakeFilename.

This method generates a file name based on the `
          filename
        ` parameter of the Content-Disposition header field, if it exists, or on he `
          name
        ` parameter of the Content-Type header field, otherwise.

<b>Returns:</b>

A suggested name for the file. Returns the empty string if there is no ilename suggested by the content type or content disposition, or if that ilename is an empty string.

### FromAddresses

    public System.Collections.Generic.IList FromAddresses { get; }

<b>Deprecated.</b> Use GetAddresses("From") instead.

Gets a list of addresses found in the From header field or fields.

<b>Returns:</b>

A list of addresses found in the From header field or fields.

### HeaderFields

    public System.Collections.Generic.IList HeaderFields { get; }

Gets a snapshot of the header fields of this message, in the order in hich they appear in the message. For each item in the list, the key is he header field's name (where any basic upper-case letters [U+0041 to +005A] are converted to lower case) and the value is the header field's alue.

<b>Returns:</b>

A snapshot of the header fields of this message.

### Parts

    public System.Collections.Generic.IList Parts { get; }

Gets a list of all the parts of this message. This list is editable. This ill only be used if the message is a multipart message.

<b>Returns:</b>

A list of all the parts of this message. This list is editable. This will nly be used if the message is a multipart message.

### Subject

    public string Subject { get; set;}

Gets or sets this message's subject.

<b>Returns:</b>

This message's subject.

### ToAddresses

    public System.Collections.Generic.IList ToAddresses { get; }

<b>Deprecated.</b> Use GetAddresses("To") instead.

Gets a list of addresses found in the To header field or fields.

<b>Returns:</b>

A list of addresses found in the To header field or fields.

### AddAttachment

    public PeterO.Mail.Message AddAttachment(
        PeterO.Mail.MediaType mediaType);

Adds an attachment with an empty body and with the given media type to his message. Before the new attachment is added, if this message isn't lready a multipart message, it becomes a "multipart/mixed" message with he current body converted to an inline body part.

<b>Parameters:</b>

 * <i>mediaType</i>: A media type to assign to the attachment.

<b>Return Value:</b>

A Message object for the generated attachment.

### AddAttachment

    public PeterO.Mail.Message AddAttachment(
        System.IO.Stream inputStream,
        PeterO.Mail.MediaType mediaType);

Adds an attachment to this message in the form of data from the given eadable stream, and with the given media type. Before the new attachment s added, if this message isn't already a multipart message, it becomes a multipart/mixed" message with the current body converted to an inline ody part.

The following example (written in C# for the .NET version) is an xtension method that adds an attachment from a byte array to a message.

    public static Message AddAttachmentFromBytes(this Message msg, byte[]
    ytes, MediaType mediaType){ using(var fs=new MemoryStream(bytes)){
    eturn msg.AddAttachment(fs,mediaType); } }

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>mediaType</i>: A media type to assign to the attachment.

<b>Return Value:</b>

A Message object for the generated attachment.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "inputStream" or "mediaType" is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.

### AddAttachment

    public PeterO.Mail.Message AddAttachment(
        System.IO.Stream inputStream,
        PeterO.Mail.MediaType mediaType,
        string filename);

Adds an attachment to this message in the form of data from the given eadable stream, and with the given media type and file name. Before the ew attachment is added, if this message isn't already a multipart essage, it becomes a "multipart/mixed" message with the current body onverted to an inline body part.

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>mediaType</i>: A media type to assign to the attachment.

 * <i>filename</i>: A file name to assign to the attachment. Can be null or empty, in which ase no file name is assigned. Only the file name portion of this arameter is used, which in this case means the portion of the string fter the last "/" or "\", if either character exists, or the entire tring otherwise.

<b>Return Value:</b>

A Message object for the generated attachment.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "inputStream" or "mediaType" is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.

### AddAttachment

    public PeterO.Mail.Message AddAttachment(
        System.IO.Stream inputStream,
        string filename);

Adds an attachment to this message in the form of data from the given eadable stream, and with the given file name. Before the new attachment s added, if this message isn't already a multipart message, it becomes a multipart/mixed" message with the current body converted to an inline ody part.

The following example (written in C# for the .NET version) is an xtension method that adds an attachment from a data file to a message.

    public static Message AddAttachmentFromFile(this Message msg, string
    ilename){ using(var fs=new FileStream(filename,FileMode.Open)){ return
    sg.AddAttachment(fs,filename); } }

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>filename</i>: A file name to assign to the attachment. If the file name has one of ertain extensions (such as ".html"), an appropriate media type will be ssigned to the attachment based on that extension; otherwise, the media ype "application/octet-stream" is assigned. Can be null or empty, in hich case no file name is assigned. Only the file name portion of this arameter is used, which in this case means the portion of the string fter the last "/" or "\", if either character exists, or the entire tring otherwise.

<b>Return Value:</b>

A Message object for the generated attachment.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "inputStream" is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.

### AddHeader

    public PeterO.Mail.Message AddHeader(
        string name,
        string value);

Adds a header field to the end of the message's header.Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: Name of a header field, such as "From" or "Content-ID".

 * <i>value</i>: Value of the header field.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
or <i>value</i>
is null.

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or he header field's value is syntactically invalid.

### AddHeader

    public PeterO.Mail.Message AddHeader(
        System.Collections.Generic.KeyValuePair header);

Adds a header field to the end of the message's header.Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>header</i>: A key/value pair. The key is the name of the header field, such as "From" r "Content-ID". The value is the header field's value.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The key or value of <i>header</i>
is null.

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or he header field's value is syntactically invalid.

### AddInline

    public PeterO.Mail.Message AddInline(
        PeterO.Mail.MediaType mediaType);

Adds an inline body part with an empty body and with the given media type o this message. Before the new body part is added, if this message isn't lready a multipart message, it becomes a "multipart/mixed" message with he current body converted to an inline body part.

<b>Parameters:</b>

 * <i>mediaType</i>: A media type to assign to the body part.

<b>Return Value:</b>

A Message object for the generated body part.

### AddInline

    public PeterO.Mail.Message AddInline(
        System.IO.Stream inputStream,
        PeterO.Mail.MediaType mediaType);

Adds an inline body part to this message in the form of data from the iven readable stream, and with the given media type. Before the new body art is added, if this message isn't already a multipart message, it ecomes a "multipart/mixed" message with the current body converted to an nline body part.

The following example (written in C# for the .NET version) is an xtension method that adds an inline body part from a byte array to a essage.

    public static Message AddInlineFromBytes(this Message msg, byte[]
    ytes, MediaType mediaType){ using(var fs=new MemoryStream(bytes)){
    eturn msg.AddInline(fs,mediaType); } }

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>mediaType</i>: A media type to assign to the body part.

<b>Return Value:</b>

A Message object for the generated body part.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "inputStream" or "mediaType" is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.

### AddInline

    public PeterO.Mail.Message AddInline(
        System.IO.Stream inputStream,
        PeterO.Mail.MediaType mediaType,
        string filename);

Adds an inline body part to this message in the form of data from the iven readable stream, and with the given media type and file name. Before he new body part is added, if this message isn't already a multipart essage, it becomes a "multipart/mixed" message with the current body onverted to an inline body part.

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>mediaType</i>: A media type to assign to the body part.

 * <i>filename</i>: A file name to assign to the body part.

<b>Return Value:</b>

A Message object for the generated body part.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "inputStream" or "mediaType" is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.

### AddInline

    public PeterO.Mail.Message AddInline(
        System.IO.Stream inputStream,
        string filename);

Adds an inline body part to this message in the form of data from the iven readable stream, and with the given file name. Before the new body art is added, if this message isn't already a multipart message, it ecomes a "multipart/mixed" message with the current body converted to an nline body part.

The following example (written in C# for the .NET version) is an xtension method that adds an inline body part from a data file to a essage.

    public static Message AddAttachmentFromInline(this Message msg, string
    ilename){ using(var fs=new FileStream(filename,FileMode.Open)){ return
    sg.AddInline(fs,filename); } }

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>filename</i>: A file name to assign to the body part. If the file name has one of ertain extensions (such as ".html"), an appropriate media type will be ssigned to the body part based on that extension; otherwise, the media ype "application/octet-stream" is assigned. Can be null or empty, in hich case no file name is assigned. Only the file name portion of this arameter is used, which in this case means the portion of the string fter the last "/" or "\", if either character exists, or the entire tring otherwise.

<b>Return Value:</b>

A Message object for the generated body part.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter "inputStream" is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.

### ClearHeaders

    public PeterO.Mail.Message ClearHeaders();

Deletes all header fields in this message. Also clears this message's ontent disposition and resets its content type to ediaType.TextPlainAscii.

<b>Return Value:</b>

This object.

### DecodeHeaderValue

    public static string DecodeHeaderValue(
        string name,
        string value);

Not documented yet.

<b>Parameters:</b>

 * <i>name</i>: Not documented yet.

 * <i>value</i>: Not documented yet.

<b>Return Value:</b>

A text string.

### FromMailtoUrl

    public static PeterO.Mail.Message FromMailtoUrl(
        string url);

Not documented yet.

### Generate

    public string Generate();

Generates this message's data in text form.The generated message will have only Basic Latin code points (U+0000 to +007F), and the transfer encoding will always be 7bit, uoted-printable, or base64 (the declared transfer encoding for this essage will be ignored).

The following applies to the following header fields: From, To, Cc, cc, Reply-To, Sender, Resent-To, Resent-From, Resent-Cc, Resent-Bcc, nd Resent-Sender. If the header field exists, but has an invalid yntax, has no addresses, or appears more than once, this method will enerate a synthetic header field with the display-name set to the ontents of all of the header fields with the same name, and the address et to `
          me@[header-name]-address.invalid
        ` as the address (a `
          .invalid
        ` address is a reserved address that can never belong to anyone). (An xception is that the Resent-* header fields may appear more than once.) he generated message should always have a From header field.

If a Date and/or Message-ID header field doesn't exist, a field with hat name will be generated (using the current local time for the Date ield).

When encoding the message's body, if the message has a text content ype ("text/*"), the line breaks are a CR byte (carriage return, 0x0d) ollowed by an LF byte (line feed, 0x0a), CR alone, or LF alone. If the essage has any other content type, only CR followed by LF is considered line break.

<b>Return Value:</b>

The generated message.

<b>Exceptions:</b>

 * PeterO.Mail.MessageDataException:
The message can't be generated.

### GenerateBytes

    public byte[] GenerateBytes();

Generates this message's data as a byte array, using the same algorithm s the Generate method.

<b>Return Value:</b>

The generated message as a byte array.

### GetAddresses

    public System.Collections.Generic.IList GetAddresses(
        string headerName);

Gets a list of addresses contained in the header fields with the given ame in this message.

<b>Parameters:</b>

 * <i>headerName</i>: The name of the header fields to retrieve.

<b>Return Value:</b>

A list of addresses, in the order in which they appear in this message's eader fields of the given name.

<b>Exceptions:</b>

 * System.NotSupportedException:
"headerName" is not supported for this method. Currently, the only header ields supported are To, Cc, Bcc, Reply-To, Sender, and From.

 * System.ArgumentNullException:
"headerName" is null.

 * System.ArgumentException:
"headerName" is empty.

### GetBody

    public byte[] GetBody();

Gets the byte array for this message's body. This method doesn't make a opy of that byte array.

<b>Return Value:</b>

A byte array.

### GetBodyMessage

    public PeterO.Mail.Message GetBodyMessage();

Returns the mail message contained in this message's body.

<b>Return Value:</b>

A message object if this object's content type is "message/rfc822", message/news", or "message/global", or null otherwise.

### GetDate

    public int[] GetDate();

Gets the date and time extracted from this message's Date header field the value of which is found as though GetHeader("date") were called). See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for more information on the format of the date-time array returned by his method.

<b>Return Value:</b>

An array containing eight elements. Returns null if the Date header oesn't exist, if the Date field is syntactically or semantically invalid, r if the field's year would overflow a 32-bit signed integer.

### GetHeader

    public string GetHeader(
        string name);

Gets the first instance of the header field with the specified name, sing a basic case-insensitive comparison. (Two strings are equal in such comparison, if they match after converting the basic upper-case letters to Z (U+0041 to U+005A) in both strings to lower case.).

<b>Parameters:</b>

 * <i>name</i>: The name of a header field.

<b>Return Value:</b>

The value of the first header field with that name, or null if there is one.

<b>Exceptions:</b>

 * System.ArgumentNullException:
Name is null.

### GetHeader

    public System.Collections.Generic.KeyValuePair GetHeader(
        int index);

Gets the name and value of a header field by index.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to get.

<b>Return Value:</b>

A key/value pair. The key is the name of the header field, such as "From" r "Content-ID". The value is the header field's value.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>index</i>
is 0 or at least as high as the number of header fields.

### GetHeaderArray

    public string[] GetHeaderArray(
        string name);

Gets an array with the values of all header fields with the specified ame, using a basic case-insensitive comparison. (Two strings are equal in uch a comparison, if they match after converting the basic upper-case etters A to Z (U+0041 to U+005A) in both strings to lower case.).

<b>Parameters:</b>

 * <i>name</i>: The name of a header field.

<b>Return Value:</b>

An array containing the values of all header fields with the given name, n the order they appear in the message. The array will be empty if no eader field has that name.

<b>Exceptions:</b>

 * System.ArgumentNullException:
Name is null.

### MakeMultilingualMessage

    public static PeterO.Mail.Message MakeMultilingualMessage(
        System.Collections.Generic.IList messages,
        System.Collections.Generic.IList languages);

Generates a multilingual message (see RFC 8255) from a list of messages nd a list of language strings.

<b>Parameters:</b>

 * <i>messages</i>: A list of messages forming the parts of the multilingual message object. ach message should have the same content, but be in a different language. ach message must have a From header field and use the same email address n that field as the other messages. The messages should be ordered in escending preference of language.

 * <i>languages</i>: A list of language strings corresponding to the messages given in the messages" parameter. A language string at a given index corresponds to he message at the same index. Each language string must follow the syntax f the Content-Language header field (see LanguageTags.GetLanguageList).

<b>Return Value:</b>

A Message object with the content type "multipart/multilingual". It will egin with an explanatory body part and be followed by the messages given n the "messages" parameter in the order given.

<b>Exceptions:</b>

 * System.ArgumentNullException:
"messages" or "languages" is null.

 * System.ArgumentException:
"messages" or "languages" is empty, their lengths don't match, at least ne message is "null", each message doesn't contain the same email ddresses in their From header fields, "languages" contains a yntactically invalid language tag list, "languages" contains the language ag "zzx" not appearing alone or at the end of the language tag list, or he first message contains no From header field.

### NewBodyPart

    public static PeterO.Mail.Message NewBodyPart();

Creates a message object with no header fields.

<b>Return Value:</b>

A message object with no header fields.

### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        int index);

Removes a header field by index.Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>index</i>
is 0 or at least as high as the number of header fields.

### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        string name);

Removes all instances of the given header field from this message. If his is a multipart message, the header field is not removed from its body art headers. A basic case-insensitive comparison is used. (Two strings re equal in such a comparison, if they match after converting the basic pper-case letters A to Z (U+0041 to U+005A) in both strings to lower ase.).Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: The name of the header field to remove.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
is null.

### SelectLanguageMessage

    public PeterO.Mail.Message SelectLanguageMessage(
        System.Collections.Generic.IList languages);

Not documented yet.

<b>Parameters:</b>

 * <i>languages</i>: Not documented yet.

<b>Return Value:</b>

A Message object.

### SelectLanguageMessage

    public PeterO.Mail.Message SelectLanguageMessage(
        System.Collections.Generic.IList languages,
        bool preferOriginals);

Not documented yet.

<b>Parameters:</b>

 * <i>languages</i>: Not documented yet.

 * <i>preferOriginals</i>: Not documented yet.

<b>Return Value:</b>

A Message object.

### SetBody

    public PeterO.Mail.Message SetBody(
        byte[] bytes);

Sets the body of this message to the given byte array. This method oesn't make a copy of that byte array.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array.

<b>Return Value:</b>

This object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>bytes</i>
is null.

### SetCurrentDate

    public PeterO.Mail.Message SetCurrentDate();

Sets this message's Date header field to the current time as its value.This method can be used when the message is considered complete and eady to be generated, for example, using the "Generate()" method.

<b>Return Value:</b>

This object.

### SetDate

    public PeterO.Mail.Message SetDate(
        int[] dateTime);

Sets this message's Date header field to the given date and time.

<b>Parameters:</b>

 * <i>dateTime</i>: An array containing eight elements. Each element of the array (starting rom 0) is as follows:

 * 0 - The year. For example, the value 2000 means 2000 C.E.

 * 1 - Month of the year, from 1 (January) through 12 (December).

 * 2 - Day of the month, from 1 through 31.

 * 3 - Hour of the day, from 0 through 23.

 * 4 - Minute of the hour, from 0 through 59.

 * 5 - Second of the minute, from 0 through 60 (this value can go up to 0 to accommodate leap seconds). (Leap seconds are additional seconds dded to adjust international atomic time, or TAI, to an approximation f astronomical time known as coordinated universal time, or UTC.)

 * 6 - Milliseconds of the second, from 0 through 999. This value is not sed to generate the date string, but must still be valid.

 * 7 - Number of minutes to subtract from this date and time to get lobal time. This number can be positive or negative.

.

<b>Return Value:</b>

This object.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>dateTime</i>
contains fewer than eight elements, contains invalid values, or contains year less than 0.

 * System.ArgumentNullException:
The parameter <i>dateTime</i>
is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        string name,
        string value);

Sets the name and value of a header field by index.Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

 * <i>name</i>: Name of a header field, such as "From" or "Content-ID".

 * <i>value</i>: Value of the header field.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>index</i>
is 0 or at least as high as the number of header fields; or, the header ield name is too long or contains an invalid character, or the header ield's value is syntactically invalid.

 * System.ArgumentNullException:
The parameter <i>name</i>
or <i>value</i>
is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        string value);

Sets the value of a header field by index without changing its name.Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

 * <i>value</i>: Value of the header field.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>index</i>
is 0 or at least as high as the number of header fields; or, the header ield name is too long or contains an invalid character, or the header ield's value is syntactically invalid.

 * System.ArgumentNullException:
The parameter <i>value</i>
is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        System.Collections.Generic.KeyValuePair header);

Sets the name and value of a header field by index.Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

 * <i>header</i>: A key/value pair. The key is the name of the header field, such as "From" r "Content-ID". The value is the header field's value.

<b>Return Value:</b>

A Message object.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>index</i>
is 0 or at least as high as the number of header fields; or, the header ield name is too long or contains an invalid character, or the header ield's value is syntactically invalid.

 * System.ArgumentNullException:
The key or value of <i>header</i>
is null.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        string name,
        string value);

Sets the value of this message's header field. If a header field with the ame name exists, its value is replaced. If the header field's name occurs ore than once, only the first instance of the header field is replaced.Updates the ContentType and ContentDisposition properties if those eader fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: The name of a header field, such as "from" or "subject".

 * <i>value</i>: The header field's value.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or he header field's value is syntactically invalid.

 * System.ArgumentNullException:
The parameter <i>name</i>
or <i>value</i>
is null.

### SetHtmlBody

    public PeterO.Mail.Message SetHtmlBody(
        string str);

Sets the body of this message to the specified string in HTML format. The haracter sequences CR (carriage return, "\r", U+000D), LF (line feed, \n", U+000A), and CR/LF will be converted to CR/LF line breaks. Unpaired urrogate code points will be replaced with replacement characters.

<b>Parameters:</b>

 * <i>str</i>: A string consisting of the message in HTML format.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null.

### SetTextAndHtml

    public PeterO.Mail.Message SetTextAndHtml(
        string text,
        string html);

Sets the body of this message to a multipart body with plain text and TML versions of the same message. The character sequences CR (carriage eturn, "\r" , U+000D), LF (line feed, "\n", U+000A), and CR/LF will be onverted to CR/LF line breaks. Unpaired surrogate code points will be eplaced with replacement characters.

<b>Parameters:</b>

 * <i>text</i>: A string consisting of the plain text version of the message.

 * <i>html</i>: A string consisting of the HTML version of the message.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>text</i>
or <i>html</i>
is null.

### SetTextBody

    public PeterO.Mail.Message SetTextBody(
        string str);

Sets the body of this message to the specified plain text string. The haracter sequences CR (carriage return, "\r", U+000D), LF (line feed, \n", U+000A), and CR/LF will be converted to CR/LF line breaks. Unpaired urrogate code points will be replaced with replacement characters. This ethod changes this message's media type to plain text.

<b>Parameters:</b>

 * <i>str</i>: A string consisting of the message in plain text format.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null.

### ToMailtoUrl

    public string ToMailtoUrl();

Not documented yet.
