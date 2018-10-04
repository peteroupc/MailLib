## PeterO.Mail.Message

    public sealed class Message

Represents an email message, and contains methods and properties for accessing and modifying email message data. This class implements the Internet Message Format (RFC 5322) and Multipurpose Internet Mail Extensions (MIME; RFC 2045-2047, RFC 2049).

<b>Thread safety:</b>This class is mutable; its properties can be changed. None of its nstance methods are designed to be thread safe. Therefore, access to bjects from this class must be synchronized if multiple threads can ccess them at the same time.

The following lists known deviations from the mail specifications (Internet Message Format and MIME):

 * The content-transfer-encodings "quoted-printable" and "base64" are treated as 7bit instead if they occur in a message or body part with content type "multipart/*" or "message/*" (other than "message/global", "message/global-headers", "message/global-disposition-notification", or "message/global-delivery-status").

 * If a message has two or more Content-Type header fields, it is treated as having a content type of "application/octet-stream", unless one or more of the header fields is syntactically invalid.

 * Illegal UTF-8 byte sequences appearing in header field values are replaced with replacement characters. Moreover, UTF-8 is parsed everywhere in header field values, even in those parts of some structured header fields where this appears not to be allowed. (UTF-8 is a character encoding for the Unicode character set.)

 * This implementation can parse a message even if that message is without a From header field, without a Date header field, or without both.

 * The To and Cc header fields are allowed to contain only comments and whitespace, but these "empty" header fields will be omitted when generating.

 * There is no line length limit imposed when parsing header fields, except header field names.

 * There is no line length limit imposed when parsing quoted-printable or base64 encoded bodies.

 * If the transfer encoding is absent and the content type is "message/rfc822", bytes with values greater than 127 (called "8-bit bytes" in the rest of this summary) are still allowed, despite the default value of "7bit" for "Content-Transfer-Encoding".

 * In the following cases, if the transfer encoding is absent, declared as 7bit, or treated as 7bit, 8-bit bytes are still allowed:

 * (a) The preamble and epilogue of multipart messages, which will be ignored.

 * (b) If the charset is declared to be `utf-8` .

 * (c) If the content type is "text/html" and the charset is declared to be `us-ascii` , "windows-1252", "windows-1251", or "iso-8859-*" (all single yte encodings).

 * (d) In non-MIME message bodies and in text/plain message bodies. Any 8-bit bytes are replaced with the substitute character byte (0x1a).

 * If the message starts with the word "From" (and no other case variations of that word) followed by one or more space (U+0020) not followed by colon, that text and the rest of the text is skipped up to and including a line feed (U+000A). (See also RFC 4155, which describes the so-called "mbox" convention with "From" lines of this kind.)

 * The name `ascii` is treated as a synonym for `us-ascii` , despite being a reserved name under RFC 2046. The name `cp1252` and `utf8` are treated as synonyms for `windows-1252` and `utf-8` , respectively, even though they are not IANA registered aliases.

 * The following deviations involve encoded words under RFC 2047:

 * (a) If a sequence of encoded words decodes to a string with a CTL character (U+007F, or a character less than U+0020 and not TAB) after being converted to Unicode, the encoded words are left un-decoded.

 * (b) This implementation can decode encoded words regardless of the character length of the line in which they appear. This implementation can generate a header field line with one or more encoded words even if that line is more than 76 characters long. (This implementation follows the recommendation in RFC 5322 to limit header field lines to no more than 78 characters, where possible.)

It would be appreciated if users of this library contact the author if they find other ways in which this implementation deviates from the mail specifications or other applicable specifications.

Note that this class currently doesn't support the "padding" parameter for message bodies with the media type "application/octet-stream" or treated as that media type (see RFC 2046 sec. 4.5.1).

Note that this implementation can decode an RFC 2047 encoded word that uses ISO-2022-JP (the only supported encoding that uses code switching) even if the encoded word's payload ends in a different mode from "ASCII mode". (Each encoded word still starts in "ASCII mode", though.) This, however, is not a deviation to RFC 2047 because the relevant rule only concerns bringing the output device back to "ASCII mode" after the decoded text is displayed (see last paragraph of sec. 6.2) -- since the decoded text is converted to Unicode rather than kept as ISO-2022-JP, this is not applicable since there is no such thing as "ASCII mode" in the Unicode Standard.

Note that this library (the MailLib library) has no facilities for sending and receiving email messages, since that's outside this library's scope.


### Member Summary
* <code>[AddAttachment(PeterO.Mail.MediaType)](#AddAttachment_PeterO_Mail_MediaType)</code> - Adds an attachment with an empty body and with the given media type to this message.
* <code>[AddAttachment(System.IO.Stream, PeterO.Mail.MediaType)](#AddAttachment_System_IO_Stream_PeterO_Mail_MediaType)</code> - Adds an attachment to this message in the form of data from the given readable stream, and with the given media type.
* <code>[AddAttachment(System.IO.Stream, PeterO.Mail.MediaType, string)](#AddAttachment_System_IO_Stream_PeterO_Mail_MediaType_string)</code> - Adds an attachment to this message in the form of data from the given readable stream, and with the given media type and file name.
* <code>[AddAttachment(System.IO.Stream, string)](#AddAttachment_System_IO_Stream_string)</code> - Adds an attachment to this message in the form of data from the given readable stream, and with the given file name.
* <code>[AddHeader(System.Collections.Generic.KeyValuePair)](#AddHeader_System_Collections_Generic_KeyValuePair)</code> - Adds a header field to the end of the message's header.
* <code>[AddHeader(string, string)](#AddHeader_string_string)</code> - Adds a header field to the end of the message's header.
* <code>[AddInline(PeterO.Mail.MediaType)](#AddInline_PeterO_Mail_MediaType)</code> - Adds an inline body part with an empty body and with the given media type to this message.
* <code>[AddInline(System.IO.Stream, PeterO.Mail.MediaType)](#AddInline_System_IO_Stream_PeterO_Mail_MediaType)</code> - Adds an inline body part to this message in the form of data from the given readable stream, and with the given media type.
* <code>[AddInline(System.IO.Stream, PeterO.Mail.MediaType, string)](#AddInline_System_IO_Stream_PeterO_Mail_MediaType_string)</code> - Adds an inline body part to this message in the form of data from the given readable stream, and with the given media type and file name.
* <code>[AddInline(System.IO.Stream, string)](#AddInline_System_IO_Stream_string)</code> - Adds an inline body part to this message in the form of data from the given readable stream, and with the given file name.
* <code>[BccAddresses](#BccAddresses)</code> - Gets a list of addresses found in the BCC header field or fields.
* <code>[BodyString](#BodyString)</code> - Gets the body of this message as a text string.
* <code>[CCAddresses](#CCAddresses)</code> - Gets a list of addresses found in the CC header field or fields.
* <code>[ClearHeaders()](#ClearHeaders)</code> - Deletes all header fields in this message.
* <code>[ContentDisposition](#ContentDisposition)</code> - Gets or sets this message's content disposition.
* <code>[ContentType](#ContentType)</code> - Gets or sets this message's media type.
* <code>[DecodeHeaderValue(string, string)](#DecodeHeaderValue_string_string)</code> - Decodes RFC 2047 encoded words from the given header field value and returns a string with those words decoded.
* <code>[FileName](#FileName)</code> -  Gets a file name suggested by this message for saving the message's body to a file.
* <code>[FromAddresses](#FromAddresses)</code> - Gets a list of addresses found in the From header field or fields.
* <code>[FromMailtoUri(string)](#FromMailtoUri_string)</code> - Creates a message object from a MailTo URI (uniform resource identifier).
* <code>[FromMailtoUrl(string)](#FromMailtoUrl_string)</code> - Creates a message object from a MailTo URI (uniform resource identifier).
* <code>[Generate()](#Generate)</code> - Generates this message's data in text form.
* <code>[GenerateBytes()](#GenerateBytes)</code> - Generates this message's data as a byte array, using the same algorithm as the Generate method.
* <code>[GetAddresses(string)](#GetAddresses_string)</code> - Gets a list of addresses contained in the header fields with the given name in this message.
* <code>[GetBody()](#GetBody)</code> - Gets the byte array for this message's body.
* <code>[GetBodyMessage()](#GetBodyMessage)</code> - Returns the mail message contained in this message's body.
* <code>[GetDate()](#GetDate)</code> - Gets the date and time extracted from this message's Date header field (the value of which is found as though GetHeader("date") were called).
* <code>[GetFormattedBodyString()](#GetFormattedBodyString)</code> -  Gets a Hypertext Markup Language (HTML) rendering of this message's text body.
* <code>[GetHeaderArray(string)](#GetHeaderArray_string)</code> - Gets an array with the values of all header fields with the specified name, using a basic case-insensitive comparison.
* <code>[GetHeader(int)](#GetHeader_int)</code> - Gets the name and value of a header field by index.
* <code>[GetHeader(string)](#GetHeader_string)</code> - Gets the first instance of the header field with the specified name, using a basic case-insensitive comparison.
* <code>[HeaderFields](#HeaderFields)</code> - Gets a snapshot of the header fields of this message, in the order in which they appear in the message.
* <code>[MakeMultilingualMessage(System.Collections.Generic.IList, System.Collections.Generic.IList)](#MakeMultilingualMessage_System_Collections_Generic_IList_System_Collections_Generic_IList)</code> - Generates a multilingual message (see RFC 8255) from a list of messages and a list of language strings.
* <code>[NewBodyPart()](#NewBodyPart)</code> - Creates a message object with no header fields.
* <code>[Parts](#Parts)</code> - Gets a list of all the parts of this message.
* <code>[RemoveHeader(int)](#RemoveHeader_int)</code> - Removes a header field by index.
* <code>[RemoveHeader(string)](#RemoveHeader_string)</code> - Removes all instances of the given header field from this message.
* <code>[SelectLanguageMessage(System.Collections.Generic.IList)](#SelectLanguageMessage_System_Collections_Generic_IList)</code> - Selects a body part for a multiple-language message (multipart/multilingual ) according to the given language priority list.
* <code>[SelectLanguageMessage(System.Collections.Generic.IList, bool)](#SelectLanguageMessage_System_Collections_Generic_IList_bool)</code> - Selects a body part for a multiple-language message (multipart/multilingual ) according to the given language priority list and original-language preference.
* <code>[SetBody(byte[])](#SetBody_byte)</code> - Sets the body of this message to the given byte array.
* <code>[SetCurrentDate()](#SetCurrentDate)</code> - Sets this message's Date header field to the current time as its value.
* <code>[SetDate(int[])](#SetDate_int)</code> - Sets this message's Date header field to the given date and time.
* <code>[SetHeader(int, System.Collections.Generic.KeyValuePair)](#SetHeader_int_System_Collections_Generic_KeyValuePair)</code> - Sets the name and value of a header field by index.
* <code>[SetHeader(int, string)](#SetHeader_int_string)</code> - Sets the value of a header field by index without changing its name.
* <code>[SetHeader(int, string, string)](#SetHeader_int_string_string)</code> - Sets the name and value of a header field by index.
* <code>[SetHeader(string, string)](#SetHeader_string_string)</code> - Sets the value of this message's header field.
* <code>[SetHtmlBody(string)](#SetHtmlBody_string)</code> - Sets the body of this message to the specified string in Hypertext Markup Language (HTML) format.
* <code>[SetTextAndHtml(string, string)](#SetTextAndHtml_string_string)</code> - Sets the body of this message to a multipart body with plain text and Hypertext Markup Language (HTML) versions of the same message.
* <code>[SetTextAndMarkdown(string, string)](#SetTextAndMarkdown_string_string)</code> - Sets the body of this message to a multipart body with plain text, Markdown, and Hypertext Markup Language (HTML) versions of the same message.
* <code>[SetTextBody(string)](#SetTextBody_string)</code> - Sets the body of this message to the specified plain text string.
* <code>[Subject](#Subject)</code> - Gets or sets this message's subject.
* <code>[ToAddresses](#ToAddresses)</code> - Gets a list of addresses found in the To header field or fields.
* <code>[ToMailtoUri()](#ToMailtoUri)</code> - Generates a MailTo URI (uniform resource identifier) corresponding to this message.
* <code>[ToMailtoUrl()](#ToMailtoUrl)</code> - Generates a MailTo URI (uniform resource identifier) corresponding to this message.

<a id="Void_ctor_Byte"></a>

### AddAttachment

    public PeterO.Mail.Message AddAttachment(
        PeterO.Mail.MediaType mediaType);

Adds an attachment with an empty body and with the given media type to this message. Before the new attachment is added, if this message isn't already a multipart message, it becomes a "multipart/mixed" message with the current body converted to an inline body part.

<b>Parameters:</b>

 * <i>mediaType</i>: A media type to assign to the attachment.

<b>Return Value:</b>

A Message object for the generated attachment.


<a id="AddAttachment_System_IO_Stream_PeterO_Mail_MediaType"></a>

### AddAttachment

    public PeterO.Mail.Message AddAttachment(
        System.IO.Stream inputStream,
        PeterO.Mail.MediaType mediaType,
        string filename);

Adds an attachment to this message in the form of data from the given readable stream, and with the given media type and file name. Before the new attachment is added, if this message isn't already a multipart message, it becomes a "multipart/mixed" message with the current body converted to an inline body part.

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>mediaType</i>: A media type to assign to the attachment.

 * <i>filename</i>: A file name to assign to the attachment. Can be null or empty, in which case no file name is assigned. Only the file name portion of this parameter is used, which in this case means the portion of the string after the last "/" or "\", if either character exists, or the entire string otherwise.

<b>Return Value:</b>

A Message object for the generated attachment.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>inputStream</i>
 or  <i>mediaType</i>
 is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.


<a id="AddAttachment_System_IO_Stream_string"></a>

### AddHeader

    public PeterO.Mail.Message AddHeader(
        string name,
        string value);

Adds a header field to the end of the message's header.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: Name of a header field, such as "From" or "Content-ID".

 * <i>value</i>: Value of the header field.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
 or  <i>value</i>
 is null.

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.

<a id="AddHeader_System_Collections_Generic_KeyValuePair"></a>
### AddHeader

    public PeterO.Mail.Message AddHeader(
        System.Collections.Generic.KeyValuePair header);

Adds a header field to the end of the message's header.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>header</i>: A key/value pair. The key is the name of the header field, such as "From" or "Content-ID". The value is the header field's value.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The key or value of  <i>header</i>
 is null.

 * System.ArgumentException:
The header field name is too long or contains an invalid character, or the header field's value is syntactically invalid.


<a id="AddInline_PeterO_Mail_MediaType"></a>

### AddInline

    public PeterO.Mail.Message AddInline(
        System.IO.Stream inputStream,
        PeterO.Mail.MediaType mediaType);

Adds an inline body part to this message in the form of data from the given readable stream, and with the given media type. Before the new body part is added, if this message isn't already a multipart message, it becomes a "multipart/mixed" message with the current body converted to an inline body part.

The following example (written in C# for the .NET version) is an extension method that adds an inline body part from a byte array to a message.

    public static Message AddInlineFromBytes(this Message msg, byte[]
                 bytes, MediaType mediaType) { using(MemoryStream fs = new MemoryStream(bytes)) {
                 return msg.AddInline(fs, mediaType); } }

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>mediaType</i>: A media type to assign to the body part.

<b>Return Value:</b>

A Message object for the generated body part.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>inputStream</i>
 or  <i>mediaType</i>
 is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.


<a id="AddInline_System_IO_Stream_PeterO_Mail_MediaType_string"></a>

### AddInline

    public PeterO.Mail.Message AddInline(
        System.IO.Stream inputStream,
        string filename);

Adds an inline body part to this message in the form of data from the given readable stream, and with the given file name. Before the new body part is added, if this message isn't already a multipart message, it becomes a "multipart/mixed" message with the current body converted to an inline body part.

<b>Parameters:</b>

 * <i>inputStream</i>: A readable data stream.

 * <i>filename</i>: A file name to assign to the inline body part. Can be null or empty, in which case no file name is assigned. Only the file name portion of this parameter is used, which in this case means the portion of the string after the last "/" or "\", if either character exists, or the entire string otherwise An appropriate media type (or "application/octet-stream") will be assigned to the body part based on this file name's extension. If the file name has an extension .txt, .text, .htm, .html, .shtml, .asc, .brf, .pot, .rst, .md, .markdown, or .srt, the media type will have a "charset" of "utf-8".

<b>Return Value:</b>

A Message object for the generated body part.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>inputStream</i>
 or "mediaType" is null.

 * PeterO.Mail.MessageDataException:
An I/O error occurred.


<a id="ClearHeaders"></a>

### DecodeHeaderValue

    public static string DecodeHeaderValue(
        string name,
        string value);

Decodes RFC 2047 encoded words from the given header field value and returns a string with those words decoded. For an example of encoded words, see the constructor for PeterO.Mail.NamedAddress.

<b>Parameters:</b>

 * <i>name</i>: Name of the header field. This determines the syntax of the "value" parameter and is necessary to help this method interpret encoded words properly.

 * <i>value</i>: A header field value that could contain encoded words. For example, if the name parameter is "From", this parameter could be "=?utf-8?q?me?= <me@example.com>".

<b>Return Value:</b>

The header field value with valid encoded words decoded.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
 is null.


<a id="FromMailtoUri_string"></a>

### FromMailtoUrl

    public static PeterO.Mail.Message FromMailtoUrl(
        string url);

<b>Deprecated.</b> Renamed to FromMailtoUri.

Creates a message object from a MailTo URI (uniform resource identifier). The MailTo URI can contain key-value pairs that follow a question-mark, as in the following example: "mailto:me@example.com?subject=A%20Subject". In this example, "subject" is the subject of the email address. Only certain keys are supported, namely, "to", "cc", "bcc", "subject", "in-reply-to", "comments", "keywords", and "body". The first seven are header field names that will be used to set the returned message's corresponding header fields. The last, "body", sets the body of the message to the given text. Keys other than these eight will be ignored.

<b>Parameters:</b>

 * <i>url</i>: The parameter  <i>url</i>
 is not documented yet.

<b>Return Value:</b>

A Message object created from the given MailTo URI. Returs null if  <i>uri</i>
 is null, is syntactically invalid, or is not a MailTo URI.


<a id="Generate"></a>

### GenerateBytes

    public byte[] GenerateBytes();

Generates this message's data as a byte array, using the same algorithm as the Generate method.

<b>Return Value:</b>

The generated message as a byte array.


<a id="GetAddresses_string"></a>

### GetBody

    public byte[] GetBody();

Gets the byte array for this message's body. This method doesn't make a copy of that byte array.

<b>Return Value:</b>

A byte array.

<a id="GetBodyMessage"></a>
### GetBodyMessage

    public PeterO.Mail.Message GetBodyMessage();

Returns the mail message contained in this message's body.

<b>Return Value:</b>

A message object if this object's content type is "message/rfc822", "message/news", or "message/global", or null otherwise.

<a id="GetDate"></a>
### GetDate

    public int[] GetDate();

Gets the date and time extracted from this message's Date header field (the value of which is found as though GetHeader("date") were called). See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for more information on the format of the date-time array returned by his method.

<b>Return Value:</b>

An array containing eight elements. Returns null if the Date header doesn't exist, if the Date field is syntactically or semantically invalid, or if the field's year would overflow a 32-bit signed integer.


<a id="GetFormattedBodyString"></a>

### MakeMultilingualMessage

    public static PeterO.Mail.Message MakeMultilingualMessage(
        System.Collections.Generic.IList messages,
        System.Collections.Generic.IList languages);

Generates a multilingual message (see RFC 8255) from a list of messages and a list of language strings.

<b>Parameters:</b>

 * <i>messages</i>: A list of messages forming the parts of the multilingual message object. Each message should have the same content, but be in a different language. Each message must have a From header field and use the same email address in that field as the other messages. The messages should be ordered in descending preference of language.

 * <i>languages</i>: A list of language strings corresponding to the messages given in the "messages" parameter. A language string at a given index corresponds to the message at the same index. Each language string must follow the syntax of the Content-Language header field (see LanguageTags.GetLanguageList).

<b>Return Value:</b>

A Message object with the content type "multipart/multilingual". It will begin with an explanatory body part and be followed by the messages given in the  <i>messages</i>
 parameter in the order given.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>messages</i>
 or  <i>languages</i>
 is null.

 * System.ArgumentException:
The parameter <i>messages</i>
 or  <i>languages</i>
 is empty, their lengths don't match, at least one message is "null", each message doesn't contain the same email addresses in their From header fields,  <i>languages</i>
 contains a syntactically invalid language tag list,  <i>languages</i>
 contains the language tag "zzx" not appearing alone or at the end of the language tag list, or the first message contains no From header field.


<a id="NewBodyPart"></a>

### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        int index);

Removes a header field by index.Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>index</i>: Zero-based index of the header field to set.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>index</i>
 is 0 or at least as high as the number of header fields.

<a id="RemoveHeader_string"></a>
### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        string name);

Removes all instances of the given header field from this message. If this is a multipart message, the header field is not removed from its body part headers. A basic case-insensitive comparison is used. (Two strings are equal in such a comparison, if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to lower case.).Updates the ContentType and ContentDisposition properties if those header fields have been modified by this method.

<b>Parameters:</b>

 * <i>name</i>: The name of the header field to remove.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
 is null.


<a id="SelectLanguageMessage_System_Collections_Generic_IList"></a>

### SelectLanguageMessage

    public PeterO.Mail.Message SelectLanguageMessage(
        System.Collections.Generic.IList languages,
        bool preferOriginals);

Selects a body part for a multiple-language message ( `multipart/multilingual`  ) according to the given language priority list and original-language preference.

<b>Parameters:</b>

 * <i>languages</i>: A list of basic language ranges, sorted in descending order of priority (see the LanguageTags.LanguageTagFilter method).

 * <i>preferOriginals</i>: If true, a body part marked as the original language version is chosen if it matches one of the given language ranges, even if the original language has a lower priority than another language with a matching body part.

<b>Return Value:</b>

The best matching body part for the given languages. If the body part has no subject, then the top-level subject is used. If this message is not a multipart/multilingual message or has fewer than two body parts, returns this object. If no body part matches the given languages, returns the last body part if its language is "zxx", or the second body part otherwise.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>languages</i>
 is null.


<a id="SetBody_byte"></a>

### SetTextAndMarkdown

    public PeterO.Mail.Message SetTextAndMarkdown(
        string text,
        string markdown);

Sets the body of this message to a multipart body with plain text, Markdown, and Hypertext Markup Language (HTML) versions of the same message. The character sequences CR (carriage return, "\r" , U+000D), LF (line feed, "\n", U+000A), and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code points will be replaced with replacement characters.

<b>Parameters:</b>

 * <i>text</i>: A string consisting of the plain text version of the message. Can be null, in which case the value of the "markdown" parameter is used as the plain text version.

 * <i>markdown</i>: A string consisting of the Markdown version of the message. For interoperability, this Markdown version will be converted to HTML.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>markdown</i>
 is null.


<a id="SetTextBody_string"></a>

### ToMailtoUri

    public string ToMailtoUri();

Generates a MailTo URI (uniform resource identifier) corresponding to this message. The following header fields, and only these, are used to generate the URI: To, Cc, Bcc, In-Reply-To, Subject, Keywords, Comments. The message body is included in the URI only if this message has a text media type and uses a supported character encoding ("charset" parameter). The To header field is included in the URI only if it has display names or group syntax.

<b>Return Value:</b>

A MailTo URI corresponding to this message.


<a id="ToMailtoUrl"></a>

### ToMailtoUrl

    public string ToMailtoUrl();

<b>Deprecated.</b> Renamed to ToMailtoUri.

Generates a MailTo URI (uniform resource identifier) corresponding to this message. The following header fields, and only these, are used to generate the URI: To, Cc, Bcc, In-Reply-To, Subject, Keywords, Comments. The message body is included in the URI only if this message has a text media type and uses a supported character encoding ("charset" parameter). The To header field is included in the URI only if it has display names or group syntax.

<b>Return Value:</b>

A MailTo URI corresponding to this message.
