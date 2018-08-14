# com.upokecenter.mail.Message

    public final class Message extends Object

<p>Represents an email message, and contains methods and properties for
 accessing and modifying email message data. This class implements the
 Internet Message Format (RFC 5322) and Multipurpose Internet Mail
 Extensions (MIME; RFC 2045-2047, RFC 2049).</p> <p><b>Thread
 safety:</b> This class is mutable; its properties can be changed.
 None of its instance methods are designed to be thread safe.
 Therefore, access to objects from this class must be synchronized if
 multiple threads can access them at the same time.</p> <p>The
 following lists known deviations from the mail specifications
 (Internet Message Format and MIME):</p> <ul> <li>The
 content-transfer-encodings "quoted-printable" and "base64" are
 treated as 7bit instead if they occur in a message or body part with
 content type "multipart/*" or "message/*" (other than
 "message/global", "message/global-headers",
 "message/global-disposition-notification", or
 "message/global-delivery-status").</li> <li>If a message has two or
 more Content-Type header fields, it is treated as having a content
 type of "application/octet-stream", unless one or more of the header
 fields is syntactically invalid.</li> <li>Illegal UTF-8 byte
 sequences appearing in header field values are replaced with
 replacement characters. Moreover, UTF-8 is parsed everywhere in
 header field values, even in those parts of some structured header
 fields where this appears not to be allowed. (UTF-8 is a character
 encoding for the Unicode character set.)</li> <li>This implementation
 can parse a message even if that message is without a From header
 field, without a Date header field, or without both.</li> <li>The To
 and Cc header fields are allowed to contain only comments and
 whitespace, but these "empty" header fields will be omitted when
 generating.</li> <li>There is no line length limit imposed when
 parsing header fields, except header field names.</li> <li>There is
 no line length limit imposed when parsing quoted-printable or base64
 encoded bodies.</li> <li>If the transfer encoding is absent and the
 content type is "message/rfc822", bytes with values greater than 127
 (called "8-bit bytes" in the rest of this summary) are still allowed,
 despite the default value of "7bit" for
 "Content-Transfer-Encoding".</li> <li>In the following cases, if the
 transfer encoding is absent, declared as 7bit, or treated as 7bit,
 8-bit bytes are still allowed:</li> <li>(a) The preamble and epilogue
 of multipart messages, which will be ignored.</li> <li>(b) If the
 charset is declared to be <code>utf-8</code>.</li> <li>(c) If the content
 type is "text/html" and the charset is declared to be
 <code>us-ascii</code>, "windows-1252", "windows-1251", or "iso-8859-*" (all
 single byte encodings).</li> <li>(d) In non-MIME message bodies and
 in text/plain message bodies. Any 8-bit bytes are replaced with the
 substitute character byte (0x1a).</li> <li>If the message starts with
 the word "From" (and no other case variations of that word) followed
 by one or more space (U + 0020) not followed by colon, that text and
 the rest of the text is skipped up to and including a line feed
 (U+000A). (See also RFC 4155, which describes the so-called "mbox"
 convention with "From" lines of this kind.)</li> <li>The name
 <code>ascii</code> is treated as a synonym for <code>us-ascii</code>, despite
 being a reserved name under RFC 2046. The name <code>cp1252</code> is
 treated as a synonym for <code>windows-1252</code> , even though it's not
 an IANA registered alias.</li> <li>The following deviations involve
 encoded words under RFC 2047:</li> <li>(a) If a sequence of encoded
 words decodes to a string with a CTL character (U + 007F, or a
 character less than U + 0020 and not TAB) after being converted to
 Unicode, the encoded words are left un-decoded.</li> <li>(b) This
 implementation can decode encoded words regardless of the character
 length of the line in which they appear. This implementation can
 generate a header field line with one or more encoded words even if
 that line is more than 76 characters long. (This implementation
 follows the recommendation in RFC 5322 to limit header field lines to
 no more than 78 characters, where possible.)</li></ul> <p>It would be
 appreciated if users of this library contact the author if they find
 other ways in which this implementation deviates from the mail
 specifications or other applicable specifications.</p> <p>Note that
 this class currently doesn't support the "padding" parameter for
 message bodies with the media type "application/octet-stream" or
 treated as that media type (see RFC 2046 sec. 4.5.1).</p> <p>Note
 that this implementation can decode an RFC 2047 encoded word that
 uses ISO-2022-JP (the only supported encoding that uses code
 switching) even if the encoded word's payload ends in a different
 mode from "ASCII mode". (Each encoded word still starts in "ASCII
 mode", though.) This, however, is not a deviation to RFC 2047 because
 the relevant rule only concerns bringing the output device back to
 "ASCII mode" after the decoded text is displayed (see last paragraph
 of sec. 6.2) -- since the decoded text is converted to Unicode rather
 than kept as ISO-2022-JP, this is not applicable since there is no
 such thing as "ASCII mode" in the Unicode Standard.</p> <p>Note that
 this library (the MailLib library) has no facilities for sending and
 receiving email messages, since that's outside this library's
 scope.</p>

## Methods

* `Message() Message`<br>
 Initializes a new instance of the Message
 class.
* `Message​(byte[] bytes) Message`<br>
 Initializes a new instance of the Message
 class.
* `Message​(InputStream stream) Message`<br>
 Initializes a new instance of the Message
 class.
* `Message AddAttachment​(MediaType mediaType)`<br>
 Not documented yet.
* `Message AddAttachment​(InputStream inputStream,
             MediaType mediaType)`<br>
 Adds an attachment to this message in the form of data from the given
 readable stream, and with the given media type.
* `Message AddAttachment​(InputStream inputStream,
             MediaType mediaType,
             String filename)`<br>
 Adds an attachment to this message in the form of data from the given
 readable stream, and with the given media type and file name.
* `Message AddAttachment​(InputStream inputStream,
             String filename)`<br>
 Adds an attachment to this message in the form of data from the given
 readable stream, and with the given file name.
* `Message AddHeader​(String name,
         String value)`<br>
 Adds a header field to the end of the message's header.
* `Message AddHeader​(Map.Entry<String,String> header)`<br>
 Adds a header field to the end of the message's header.
* `Message AddInline​(MediaType mediaType)`<br>
 Not documented yet.
* `Message AddInline​(InputStream inputStream,
         MediaType mediaType)`<br>
 Adds an inline body part to this message in the form of data from the given
 readable stream, and with the given media type.
* `Message AddInline​(InputStream inputStream,
         MediaType mediaType,
         String filename)`<br>
 Adds an inline body part to this message in the form of data from the given
 readable stream, and with the given media type and file name.
* `Message AddInline​(InputStream inputStream,
         String filename)`<br>
 Adds an inline body part to this message in the form of data from the given
 readable stream, and with the given file name.
* `Message ClearHeaders()`<br>
 Not documented yet.
* `String Generate()`<br>
 Generates this message's data in text form.
* `byte[] GenerateBytes()`<br>
 Generates this message's data as a byte array, using the same algorithm as
 the Generate method.
* `List<NamedAddress> GetAddresses​(String headerName)`<br>
 Not documented yet.
* `List<NamedAddress> getBccAddresses()`<br>
 Deprecated.
Use GetAddresses(\Bcc\) instead.
 Use GetAddresses(\Bcc\) instead.
* `byte[] GetBody()`<br>
 Gets the byte array for this message's body.
* `Message GetBodyMessage()`<br>
 Returns the mail message contained in this message's body.
* `String getBodyString()`<br>
 Gets the body of this message as a text string.
* `List<NamedAddress> getCCAddresses()`<br>
 Deprecated.
Use GetAddresses(\Cc\) instead.
 Use GetAddresses(\Cc\) instead.
* `ContentDisposition getContentDisposition()`<br>
 Gets this message's content disposition.
* `MediaType getContentType()`<br>
 Gets this message's media type.
* `int[] GetDate()`<br>
 Gets the date and time extracted from this message's Date header field (the
 value of which is found as though GetHeader("date") were called).
* `String getFileName()`<br>
 Gets a file name suggested by this message for saving the message's body
 to a file.
* `List<NamedAddress> getFromAddresses()`<br>
 Deprecated.
Use GetAddresses(\From\) instead.
 Use GetAddresses(\From\) instead.
* `Map.Entry<String,String> GetHeader​(int index)`<br>
 Gets the name and value of a header field by index.
* `String GetHeader​(String name)`<br>
 Gets the first instance of the header field with the specified name, using a
 basic case-insensitive comparison.
* `String[] GetHeaderArray​(String name)`<br>
 Gets an array with the values of all header fields with the specified name,
 using a basic case-insensitive comparison.
* `List<Map.Entry<String,String>> getHeaderFields()`<br>
 Gets a snapshot of the header fields of this message, in the order in which
 they appear in the message.
* `List<Message> getParts()`<br>
 Gets a list of all the parts of this message.
* `String getSubject()`<br>
 Gets this message's subject.
* `List<NamedAddress> getToAddresses()`<br>
 Deprecated.
Use GetAddresses(\To\) instead.
 Use GetAddresses(\To\) instead.
* `static Message NewBodyPart()`<br>
 Not documented yet.
* `Message RemoveHeader​(int index)`<br>
 Removes a header field by index.
* `Message RemoveHeader​(String name)`<br>
 Removes all instances of the given header field from this message.
* `Message SetBody​(byte[] bytes)`<br>
 Sets the body of this message to the given byte array.
* `void setContentDisposition​(ContentDisposition value)`<br>
* `void setContentType​(MediaType value)`<br>
* `Message SetCurrentDate()`<br>
 Sets this message's Date header field to the current time as its value.
* `Message SetDate​(int[] dateTime)`<br>
 Sets this message's Date header field to the given date and time.
* `Message SetHeader​(int index,
         String value)`<br>
 Sets the value of a header field by index without changing its name.
* `Message SetHeader​(int index,
         String name,
         String value)`<br>
 Sets the name and value of a header field by index.
* `Message SetHeader​(int index,
         Map.Entry<String,String> header)`<br>
 Sets the name and value of a header field by index.
* `Message SetHeader​(String name,
         String value)`<br>
 Sets the value of this message's header field.
* `Message SetHtmlBody​(String str)`<br>
 Sets the body of this message to the specified string in HTML format.
* `void setSubject​(String value)`<br>
* `Message SetTextAndHtml​(String text,
              String html)`<br>
 Sets the body of this message to a multipart body with plain text and HTML
 versions of the same message.
* `Message SetTextBody​(String str)`<br>
 Sets the body of this message to the specified plain text string.

## Constructors

* `Message() Message`<br>
 Initializes a new instance of the Message
 class.
* `Message​(byte[] bytes) Message`<br>
 Initializes a new instance of the Message
 class.
* `Message​(InputStream stream) Message`<br>
 Initializes a new instance of the Message
 class.

## Method Details

### Message
    public Message​(InputStream stream)
Initializes a new instance of the <code>Message</code>
 class. Reads from the given InputStream object to initialize the message.

**Parameters:**

* <code>stream</code> - A readable data stream.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>stream</code> is null.

### Message
    public Message​(byte[] bytes)
Initializes a new instance of the <code>Message</code>
 class. Reads from the given byte array to initialize the message.

**Parameters:**

* <code>bytes</code> - A readable data stream.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> is null.

### Message
    public Message()
Initializes a new instance of the <code>Message</code>
 class. The message will be plain text and have an artificial From
 address.
### NewBodyPart
    public static Message NewBodyPart()
Not documented yet.

**Returns:**

* This object.

### SetCurrentDate
    public Message SetCurrentDate()
Sets this message's Date header field to the current time as its value.
 <p>This method can be used when the message is considered complete
 and ready to be generated, for example, using the "Generate()"
 method.</p>

**Returns:**

* This object.

### getBccAddresses
    @Deprecated public final List<NamedAddress> getBccAddresses()
Deprecated.
<div class='deprecationComment'>Use GetAddresses(\Bcc\) instead.</div>

**Returns:**

* A list of addresses found in the BCC header field or fields.

### getBodyString
    public final String getBodyString()
Gets the body of this message as a text string.

**Returns:**

* The body of this message as a text string.

**Throws:**

* <code>UnsupportedOperationException</code> - Either this message is a multipart
 message, so it doesn't have its own body text, or this message has no
 character encoding declared or assumed for it (which is usually the
 case for non-text messages), or the character encoding is not
 supported.

### getCCAddresses
    @Deprecated public final List<NamedAddress> getCCAddresses()
Deprecated.
<div class='deprecationComment'>Use GetAddresses(\Cc\) instead.</div>

**Returns:**

* A list of addresses found in the CC header field or fields.

### getContentDisposition
    public final ContentDisposition getContentDisposition()
Gets this message's content disposition. The content disposition specifies
 how a user agent should display or otherwise handle this message. Can
 be set to null. If set to a disposition or to null, updates the
 Content-Disposition header field as appropriate.

**Returns:**

* This message's content disposition, or null if none is specified.

### setContentDisposition
    public final void setContentDisposition​(ContentDisposition value)
### getContentType
    public final MediaType getContentType()
Gets this message's media type. When getting, the media type may differ in
 certain cases from the value of the Content-Type header field, if
 any, and may have a value even if the Content-Type header field is
 absent from this message. If set to a media type, updates the
 Content-Type header field as appropriate. Cannot be set to null.

**Returns:**

* This message's media type.

**Throws:**

* <code>NullPointerException</code> - This value is being set and "value" is
 null.

### setContentType
    public final void setContentType​(MediaType value)
### getFileName
    public final String getFileName()
<p>Gets a file name suggested by this message for saving the message's body
 to a file. For more information on the algorithm, see
 ContentDisposition.MakeFilename.</p> <p>This method generates a file
 name based on the <code>filename</code> parameter of the
 Content-Disposition header field, if it exists, or on the <code>name</code>
 parameter of the Content-Type header field, otherwise.</p>

**Returns:**

* A suggested name for the file. Returns the empty string if there is
 no filename suggested by the content type or content disposition, or
 if that filename is an empty string.

### GetAddresses
    public List<NamedAddress> GetAddresses​(String headerName)
Not documented yet.

**Parameters:**

* <code>headerName</code> - The parameter <code>headerName</code> is not documented yet.

**Returns:**

* A list of addresses, in the order in which they appear in this
 message's header fields of the given name.

### getFromAddresses
    @Deprecated public final List<NamedAddress> getFromAddresses()
Deprecated.
<div class='deprecationComment'>Use GetAddresses(\From\) instead.</div>

**Returns:**

* A list of addresses found in the From header field or fields.

### getHeaderFields
    public final List<Map.Entry<String,String>> getHeaderFields()
Gets a snapshot of the header fields of this message, in the order in which
 they appear in the message. For each item in the list, the key is the
 header field's name (where any basic upper-case letters [U+0041 to
 U + 005A] are converted to lower case) and the value is the header
 field's value.

**Returns:**

* A snapshot of the header fields of this message.

### getParts
    public final List<Message> getParts()
Gets a list of all the parts of this message. This list is editable. This
 will only be used if the message is a multipart message.

**Returns:**

* A list of all the parts of this message. This list is editable. This
 will only be used if the message is a multipart message.

### getSubject
    public final String getSubject()
Gets this message's subject.

**Returns:**

* This message's subject.

### setSubject
    public final void setSubject​(String value)
### getToAddresses
    @Deprecated public final List<NamedAddress> getToAddresses()
Deprecated.
<div class='deprecationComment'>Use GetAddresses(\To\) instead.</div>

**Returns:**

* A list of addresses found in the To header field or fields.

### AddHeader
    public Message AddHeader​(Map.Entry<String,String> header)
Adds a header field to the end of the message's header. <p>Updates the
 ContentType and ContentDisposition properties if those header fields
 have been modified by this method.</p>

**Parameters:**

* <code>header</code> - A key/value pair. The key is the name of the header field,
 such as "From" or "Content-ID". The value is the header field's
 value.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The key or value of <code>header</code> is
 null.

* <code>IllegalArgumentException</code> - The header field name is too long or
 contains an invalid character, or the header field's value is
 syntactically invalid.

### AddHeader
    public Message AddHeader​(String name, String value)
Adds a header field to the end of the message's header. <p>Updates the
 ContentType and ContentDisposition properties if those header fields
 have been modified by this method.</p>

**Parameters:**

* <code>name</code> - Name of a header field, such as "From" or "Content-ID".

* <code>value</code> - Value of the header field.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>
 value</code> is null.

* <code>IllegalArgumentException</code> - The header field name is too long or
 contains an invalid character, or the header field's value is
 syntactically invalid.

### Generate
    public String Generate()
Generates this message's data in text form. <p>The generated message will
 have only Basic Latin code points (U + 0000 to U + 007F), and the
 transfer encoding will always be 7bit, quoted-printable, or base64
 (the declared transfer encoding for this message will be
 ignored).</p> <p>The following applies to the following header
 fields: From, To, Cc, Bcc, Reply-To, Sender, Resent-To, Resent-From,
 Resent-Cc, Resent-Bcc, and Resent-Sender. If the header field exists,
 but has an invalid syntax, has no addresses, or appears more than
 once, this method will generate a synthetic header field with the
 display-name set to the contents of all of the header fields with the
 same name, and the address set to
 <code>me@[header-name]-address.invalid</code> as the address (a
 <code>.invalid</code> address is a reserved address that can never belong
 to anyone). (An exception is that the Resent-* header fields may
 appear more than once.) The generated message should always have a
 From header field.</p> <p>If a Date and/or Message-ID header field
 doesn't exist, a field with that name will be generated (using the
 current local time for the Date field).</p> <p>When encoding the
 message's body, if the message has a text content type ("text/*"),
 the line breaks are a CR byte (carriage return, 0x0d) followed by an
 LF byte (line feed, 0x0a), CR alone, or LF alone. If the message has
 any other content type, only CR followed by LF is considered a line
 break.</p>

**Returns:**

* The generated message.

**Throws:**

* <code>MessageDataException</code> - The message can't be
 generated.

### GenerateBytes
    public byte[] GenerateBytes()
Generates this message's data as a byte array, using the same algorithm as
 the Generate method.

**Returns:**

* The generated message as a byte array.

### GetBody
    public byte[] GetBody()
Gets the byte array for this message's body. This method doesn't make a copy
 of that byte array.

**Returns:**

* A byte array.

### GetDate
    public int[] GetDate()
Gets the date and time extracted from this message's Date header field (the
 value of which is found as though GetHeader("date") were called). See
 <see cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
 for more information on the format of the date-time array returned by
 this method.

**Returns:**

* An array containing eight elements. Returns null if the Date header
 doesn't exist, if the Date field is syntactically or semantically
 invalid, or if the field's year would overflow a 32-bit signed
 integer.

### SetDate
    public Message SetDate​(int[] dateTime)
Sets this message's Date header field to the given date and time.

**Parameters:**

* <code>dateTime</code> - An array containing eight elements. Each element of the
 array (starting from 0) is as follows: <ul> <li>0 - The year. For
 example, the value 2000 means 2000 C.E.</li> <li>1 - Month of the
 year, from 1 (January) through 12 (December).</li> <li>2 - Day of the
 month, from 1 through 31.</li> <li>3 - Hour of the day, from 0
 through 23.</li> <li>4 - Minute of the hour, from 0 through 59.</li>
 <li>5 - Second of the minute, from 0 through 60 (this value can go up
 to 60 to accommodate leap seconds). (Leap seconds are additional
 seconds added to adjust international atomic time, or TAI, to an
 approximation of astronomical time known as coordinated universal
 time, or UTC.)</li> <li>6 - Milliseconds of the second, from 0
 through 999. This value is not used to generate the date string, but
 must still be valid.</li> <li>7 - Number of minutes to subtract from
 this date and time to get global time. This number can be positive or
 negative.</li></ul>.

**Returns:**

* This object.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>dateTime</code> contains
 fewer than eight elements, contains invalid values, or contains a
 year less than 0.

* <code>NullPointerException</code> - The parameter <code>dateTime</code> is null.

### GetBodyMessage
    public Message GetBodyMessage()
Returns the mail message contained in this message's body.

**Returns:**

* A message object if this object's content type is "message/rfc822",
 "message/news", or "message/global", or null otherwise.

### GetHeader
    public Map.Entry<String,String> GetHeader​(int index)
Gets the name and value of a header field by index.

**Parameters:**

* <code>index</code> - Zero-based index of the header field to get.

**Returns:**

* A key/value pair. The key is the name of the header field, such as
 "From" or "Content-ID". The value is the header field's value.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at
 least as high as the number of header fields.

### GetHeader
    public String GetHeader​(String name)
Gets the first instance of the header field with the specified name, using a
 basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).

**Parameters:**

* <code>name</code> - The name of a header field.

**Returns:**

* The value of the first header field with that name, or null if there
 is none.

**Throws:**

* <code>NullPointerException</code> - Name is null.

### GetHeaderArray
    public String[] GetHeaderArray​(String name)
Gets an array with the values of all header fields with the specified name,
 using a basic case-insensitive comparison. (Two strings are equal in
 such a comparison, if they match after converting the basic
 upper-case letters A to Z (U + 0041 to U + 005A) in both strings to lower
 case.).

**Parameters:**

* <code>name</code> - The name of a header field.

**Returns:**

* An array containing the values of all header fields with the given
 name, in the order they appear in the message. The array will be
 empty if no header field has that name.

**Throws:**

* <code>NullPointerException</code> - Name is null.

### ClearHeaders
    public Message ClearHeaders()
Not documented yet.

**Returns:**

* A Message object.

### RemoveHeader
    public Message RemoveHeader​(int index)
Removes a header field by index. <p>Updates the ContentType and
 ContentDisposition properties if those header fields have been
 modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at
 least as high as the number of header fields.

### RemoveHeader
    public Message RemoveHeader​(String name)
Removes all instances of the given header field from this message. If this
 is a multipart message, the header field is not removed from its body
 part headers. A basic case-insensitive comparison is used. (Two
 strings are equal in such a comparison, if they match after
 converting the basic upper-case letters A to Z (U + 0041 to U + 005A) in
 both strings to lower case.). <p>Updates the ContentType and
 ContentDisposition properties if those header fields have been
 modified by this method.</p>

**Parameters:**

* <code>name</code> - The name of the header field to remove.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetBody
    public Message SetBody​(byte[] bytes)
Sets the body of this message to the given byte array. This method doesn't
 make a copy of that byte array.

**Parameters:**

* <code>bytes</code> - A byte array.

**Returns:**

* This object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> is null.

### SetHeader
    public Message SetHeader​(int index, Map.Entry<String,String> header)
Sets the name and value of a header field by index. <p>Updates the
 ContentType and ContentDisposition properties if those header fields
 have been modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

* <code>header</code> - A key/value pair. The key is the name of the header field,
 such as "From" or "Content-ID". The value is the header field's
 value.

**Returns:**

* A Message object.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at
 least as high as the number of header fields; or, the header field
 name is too long or contains an invalid character, or the header
 field's value is syntactically invalid.

* <code>NullPointerException</code> - The key or value of <code>header</code> is
 null.

### SetHeader
    public Message SetHeader​(int index, String name, String value)
Sets the name and value of a header field by index. <p>Updates the
 ContentType and ContentDisposition properties if those header fields
 have been modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

* <code>name</code> - Name of a header field, such as "From" or "Content-ID".

* <code>value</code> - Value of the header field.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at
 least as high as the number of header fields; or, the header field
 name is too long or contains an invalid character, or the header
 field's value is syntactically invalid.

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>
 value</code> is null.

### SetHeader
    public Message SetHeader​(int index, String value)
Sets the value of a header field by index without changing its name.
 <p>Updates the ContentType and ContentDisposition properties if those
 header fields have been modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

* <code>value</code> - Value of the header field.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at
 least as high as the number of header fields; or, the header field
 name is too long or contains an invalid character, or the header
 field's value is syntactically invalid.

* <code>NullPointerException</code> - The parameter <code>value</code> is null.

### SetHeader
    public Message SetHeader​(String name, String value)
Sets the value of this message's header field. If a header field with the
 same name exists, its value is replaced. If the header field's name
 occurs more than once, only the first instance of the header field is
 replaced. <p>Updates the ContentType and ContentDisposition
 properties if those header fields have been modified by this
 method.</p>

**Parameters:**

* <code>name</code> - The name of a header field, such as "from" or "subject".

* <code>value</code> - The header field's value.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The header field name is too long or
 contains an invalid character, or the header field's value is
 syntactically invalid.

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>
 value</code> is null.

### SetHtmlBody
    public Message SetHtmlBody​(String str)
Sets the body of this message to the specified string in HTML format. The
 character sequences CR (carriage return, "&#x5c;r", U+000D), LF (line
 feed, "&#x5c;n", U+000A), and CR/LF will be converted to CR/LF line
 breaks. Unpaired surrogate code points will be replaced with
 replacement characters.

**Parameters:**

* <code>str</code> - A string consisting of the message in HTML format.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### SetTextAndHtml
    public Message SetTextAndHtml​(String text, String html)
Sets the body of this message to a multipart body with plain text and HTML
 versions of the same message. The character sequences CR (carriage
 return, "&#x5c;r" , U+000D), LF (line feed, "&#x5c;n", U+000A), and CR/LF will
 be converted to CR/LF line breaks. Unpaired surrogate code points
 will be replaced with replacement characters.

**Parameters:**

* <code>text</code> - A string consisting of the plain text version of the message.

* <code>html</code> - A string consisting of the HTML version of the message.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>text</code> or <code>
 html</code> is null.

### SetTextBody
    public Message SetTextBody​(String str)
Sets the body of this message to the specified plain text string. The
 character sequences CR (carriage return, "&#x5c;r", U+000D), LF (line
 feed, "&#x5c;n", U+000A), and CR/LF will be converted to CR/LF line
 breaks. Unpaired surrogate code points will be replaced with
 replacement characters. This method changes this message's media type
 to plain text.

**Parameters:**

* <code>str</code> - A string consisting of the message in plain text format.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### AddInline
    public Message AddInline​(MediaType mediaType)
Not documented yet.

**Parameters:**

* <code>mediaType</code> - The parameter <code>mediaType</code> is not documented yet.

**Returns:**

* A Message object.

### AddAttachment
    public Message AddAttachment​(MediaType mediaType)
Not documented yet.

**Parameters:**

* <code>mediaType</code> - The parameter <code>mediaType</code> is not documented yet.

**Returns:**

* A Message object.

### AddAttachment
    public Message AddAttachment​(InputStream inputStream, MediaType mediaType)
Adds an attachment to this message in the form of data from the given
 readable stream, and with the given media type. Before the new
 attachment is added, if this message isn't already a multipart
 message, it becomes a "multipart/mixed" message with the current body
 converted to an inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the attachment.

**Returns:**

* A Message object for the generated attachment.

**Throws:**

* <code>NullPointerException</code> - The parameter "inputStream" or
 "mediaType" is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddAttachment
    public Message AddAttachment​(InputStream inputStream, String filename)
Adds an attachment to this message in the form of data from the given
 readable stream, and with the given file name. Before the new
 attachment is added, if this message isn't already a multipart
 message, it becomes a "multipart/mixed" message with the current body
 converted to an inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>filename</code> - A file name to assign to the attachment. If the file name
 has one of certain extensions (such as ".html"), an appropriate media
 type will be assigned to the attachment based on that extension;
 otherwise, the media type "application/octet-stream" is assigned. Can
 be null or empty, in which case no file name is assigned. Only the
 file name portion of this parameter is used, which in this case means
 the portion of the string after the last "/" or "&#x5c;", if either
 character exists, or the entire string otherwise.

**Returns:**

* A Message object for the generated attachment.

**Throws:**

* <code>NullPointerException</code> - The parameter "inputStream" is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddAttachment
    public Message AddAttachment​(InputStream inputStream, MediaType mediaType, String filename)
Adds an attachment to this message in the form of data from the given
 readable stream, and with the given media type and file name. Before
 the new attachment is added, if this message isn't already a
 multipart message, it becomes a "multipart/mixed" message with the
 current body converted to an inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the attachment.

* <code>filename</code> - A file name to assign to the attachment. Can be null or
 empty, in which case no file name is assigned. Only the file name
 portion of this parameter is used, which in this case means the
 portion of the string after the last "/" or "&#x5c;", if either character
 exists, or the entire string otherwise.

**Returns:**

* A Message object for the generated attachment.

**Throws:**

* <code>NullPointerException</code> - The parameter "inputStream" or
 "mediaType" is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddInline
    public Message AddInline​(InputStream inputStream, MediaType mediaType)
Adds an inline body part to this message in the form of data from the given
 readable stream, and with the given media type. Before the new body
 part is added, if this message isn't already a multipart message, it
 becomes a "multipart/mixed" message with the current body converted
 to an inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the body part.

**Returns:**

* A Message object for the generated body part.

**Throws:**

* <code>NullPointerException</code> - The parameter "inputStream" or
 "mediaType" is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddInline
    public Message AddInline​(InputStream inputStream, String filename)
Adds an inline body part to this message in the form of data from the given
 readable stream, and with the given file name. Before the new body
 part is added, if this message isn't already a multipart message, it
 becomes a "multipart/mixed" message with the current body converted
 to an inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>filename</code> - A file name to assign to the body part. If the file name has
 one of certain extensions (such as ".html"), an appropriate media
 type will be assigned to the body part based on that extension;
 otherwise, the media type "application/octet-stream" is assigned. Can
 be null or empty, in which case no file name is assigned. Only the
 file name portion of this parameter is used, which in this case means
 the portion of the string after the last "/" or "&#x5c;", if either
 character exists, or the entire string otherwise.

**Returns:**

* A Message object for the generated body part.

**Throws:**

* <code>NullPointerException</code> - The parameter "inputStream" is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddInline
    public Message AddInline​(InputStream inputStream, MediaType mediaType, String filename)
Adds an inline body part to this message in the form of data from the given
 readable stream, and with the given media type and file name. Before
 the new body part is added, if this message isn't already a multipart
 message, it becomes a "multipart/mixed" message with the current body
 converted to an inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the body part.

* <code>filename</code> - A file name to assign to the body part.

**Returns:**

* A Message object for the generated body part.

**Throws:**

* <code>NullPointerException</code> - The parameter "inputStream" or
 "mediaType" is null.

* <code>MessageDataException</code> - An I/O error occurred.
