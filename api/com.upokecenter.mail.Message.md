# com.upokecenter.mail.Message

    public final class Message extends Object

<p>Represents an email message, and contains methods and properties for
 accessing and modifying email message data. This class implements the
 Internet Message Format (RFC 5322) and Multipurpose Internet Mail Extensions
 (MIME; RFC 2045-2047, RFC 2049).</p><p><b>Thread safety:</b> This class is
 mutable; its properties can be changed. None of its instance methods are
 designed to be thread safe. Therefore, access to objects from this class
 must be synchronized if multiple threads can access them at the same
 time.</p> <p>The following lists known deviations from the mail
 specifications (Internet Message Format and MIME):</p> <ul> <li>If a message
 has two or more Content-Type header fields, it is treated as having a
 content type of "application/octet-stream", unless one or more of the header
 fields is syntactically invalid.</li><li>Illegal UTF-8 byte sequences
 appearing in header field values are replaced with replacement characters.
 Moreover, UTF-8 is parsed everywhere in header field values, even in those
 parts of some structured header fields where this appears not to be allowed.
 (UTF-8 is a character encoding for the Unicode character set.)</li><li>This
 implementation can parse a message even if that message is without a From
 header field, without a Date header field, or without both.</li><li>The To
 and Cc header fields are allowed to contain only comments and whitespace,
 but these "empty" header fields will be omitted when
 generating.</li><li>There is no line length limit imposed when parsing
 header fields, except header field names.</li><li>There is no line length
 limit imposed when parsing quoted-printable or base64 encoded
 bodies.</li><li>If the transfer encoding is absent and the content type is
 "message/rfc822", bytes with values greater than 127 are still allowed,
 despite the default value of "7bit" for
 "Content-Transfer-Encoding".</li><li>In the following cases, if the transfer
 encoding is absent, declared as 7bit, or treated as 7bit, bytes greater than
 127 are still allowed:</li><li>(a) The preamble and epilogue of multipart
 messages, which will be ignored.</li><li>(b) If the charset is declared to
 be <code>utf-8</code>.</li><li>(c) If the content type is "text/html" and the
 charset is declared to be <code>us-ascii</code>, "windows-1252", "windows-1251",
 or "iso-8859-*" (all single byte encodings).</li><li>(d) In text/plain
 message bodies. Any bytes greater than 127 are replaced with the substitute
 character byte (0x1a).</li><li>(e) In MIME message bodies (this is not a
 deviation from MIME, though). Any bytes greater than 127 are replaced with
 the substitute character byte (0x1a).</li><li>If the message starts with the
 word "From" (and no other case variations of that word) followed by one or
 more space (U+0020) not followed by colon, that text and the rest of the
 text is skipped up to and including a line feed (U+000A). (See also RFC
 4155, which describes the so-called "mbox" convention with "From" lines of
 this kind.)</li><li>The name <code>ascii</code> is treated as a synonym for
 <code>us-ascii</code>, despite being a reserved name under RFC 2046. The names
 <code>cp1252</code> and <code>utf8</code> are treated as synonyms for <code>
 windows-1252</code> and <code>utf-8</code>, respectively, even though they are not IANA
 registered aliases.</li><li>The following deviations involve encoded words
 under RFC 2047:</li><li>(a) If a sequence of encoded words decodes to a
 string with a CTL character (U+007F, or a character less than U+0020 and not
 TAB) after being converted to Unicode, the encoded words are left
 un-decoded.</li><li>(b) This implementation can decode encoded words
 regardless of the character length of the line in which they appear. This
 implementation can generate a header field line with one or more encoded
 words even if that line is more than 76 characters long. (This
 implementation follows the recommendation in RFC 5322 to limit header field
 lines to no more than 78 characters, where possible; see also RFC
 6532.)</li></ul> <p>It would be appreciated if users of this library contact
 the author if they find other ways in which this implementation deviates
 from the mail specifications or other applicable specifications.</p> <p>If a
 MIME message or body part has an unrecognized transfer encoding, its content
 type is treated as "application/octet-stream" rather than the declared
 content type (per MIME) and its transfer encoding is treated as 7bit (a
 clarification to MIME).</p> <p>This class currently doesn't support the
 "padding" parameter for message bodies with the media type
 "application/octet-stream" or treated as that media type (see RFC 2046 sec.
 4.5.1).</p> <p>In this implementation, if the content-transfer-encoding
 "quoted-printable" or "base64" occurs in a message or body part with content
 type "multipart/*" or "message/*" (other than "message/global",
 "message/global-headers", "message/global-disposition-notification", or
 "message/global-delivery-status"), that encoding is treated as unrecognized
 for the purpose of treating that message or body part as having a content
 type of "application/octet-stream" rather than the declared content type.
 This is a clarification to RFCs 2045 and 2049. (This may result in
 "misdecoded" messages because in practice, most if not all messages of this
 kind don't use quoted-printable or base64 encodings for the whole body, but
 may do so in the body parts they contain.)</p> <p>This implementation can
 decode an RFC 2047 encoded word that uses ISO-2022-JP or ISO-2022-JP-2
 (encodings that use code switching) even if the encoded word's payload ends
 in a different mode from "ASCII mode". (Each encoded word still starts in
 "ASCII mode", though.) This, however, is not a deviation to RFC 2047 because
 the relevant rule only concerns bringing the output device back to "ASCII
 mode" after the decoded text is displayed (see last paragraph of sec. 6.2)
 -- since the decoded text is converted to Unicode rather than kept as
 ISO-2022-JP or ISO-2022-JP-2, this is not applicable since there is no such
 thing as "ASCII mode" in the Unicode Standard.</p> <p>Note that this library
 (the MailLib library) has no facilities for sending and receiving email
 messages, since that's outside this library's scope.</p>

## Constructors

## Methods

* `Message AddAttachment(MediaType mediaType)`<br>
 Adds an attachment with an empty body and with the specified media type to
 this message.

* `Message AddAttachment(InputStream inputStream,
 MediaType mediaType)`<br>
 Adds an attachment to this message in the form of data from the specified
 readable stream, and with the specified media type.

* `Message AddAttachment(InputStream inputStream,
 MediaType mediaType,
 String filename)`<br>
 Adds an attachment to this message in the form of data from the specified
 readable stream, and with the specified media type and file name.

* `Message AddAttachment(InputStream inputStream,
 String filename)`<br>
 Adds an attachment to this message in the form of data from the specified
 readable stream, and with the specified file name.

* `Message AddHeader(String name,
 String value)`<br>
 Adds a header field to the end of the message's header.

* `Message AddHeader(Map.Entry<String,String> header)`<br>
 Adds a header field to the end of the message's header.

* `Message AddInline(MediaType mediaType)`<br>
 Adds an inline body part with an empty body and with the specified media
 type to this message.

* `Message AddInline(InputStream inputStream,
 MediaType mediaType)`<br>
 Adds an inline body part to this message in the form of data from the
 specified readable stream, and with the specified media type.

* `Message AddInline(InputStream inputStream,
 MediaType mediaType,
 String filename)`<br>
 Adds an inline body part to this message in the form of data from the
 specified readable stream, and with the specified media type and file name.

* `Message AddInline(InputStream inputStream,
 String filename)`<br>
 Adds an inline body part to this message in the form of data from the
 specified readable stream, and with the specified file name.

* `Message ClearHeaders()`<br>
 Deletes all header fields in this message.

* `static String DecodeHeaderValue(String name,
 String value)`<br>
 Decodes RFC 2047 encoded words from the specified header field value and
 returns a string with those words decoded.

* `static String ExtractHeader(byte[] bytes,
 String headerFieldName)`<br>
 Extracts the value of a header field from a byte array representing an email
 message.

* `static Message FromMailtoUri(String uri)`<br>
 Creates a message object from a MailTo URI (uniform resource identifier).

* `static Message FromMailtoUri(URI uri)`<br>
 Creates a message object from a MailTo URI (uniform resource identifier)
 in the form of a URI object.

* `static Message FromMailtoUrl(String url)`<br>
 Deprecated.
Renamed to FromMailtoUri.

* `String Generate()`<br>
 Generates this message's data in text form.

* `byte[] GenerateBytes()`<br>
 Generates this message's data as a byte array, using the same algorithm as
 the Generate method.

* `List<NamedAddress> GetAddresses(String headerName)`<br>
 Gets a list of addresses contained in the header fields with the specified
 name in this message.

* `List<Message> GetAttachments()`<br>
 Gets a list of descendant body parts of this message that are considered
 attachments.

* `final List<NamedAddress> getBccAddresses()`<br>
 Deprecated.
Use GetAddresses(\Bcc\) instead.

* `byte[] GetBody()`<br>
 Gets the byte array for this message's body.

* `Message GetBodyMessage()`<br>
 Returns the mail message contained in this message's body.

* `final String getBodyString()`<br>
 Deprecated.
Use GetBodyString() instead.

* `String GetBodyString()`<br>
 Gets the body of this message as a text string.

* `final List<NamedAddress> getCCAddresses()`<br>
 Deprecated.
Use GetAddresses(\Cc\) instead.

* `final ContentDisposition getContentDisposition()`<br>
 Gets this message's content disposition.

* `final MediaType getContentType()`<br>
 Gets this message's media type.

* `int[] GetDate()`<br>
 Gets the date and time extracted from this message's Date header field
 (the value of which is found as though GetHeader("date") were called).

* `final String getFileName()`<br>
 Gets a file name suggested by this message for saving the message's body
 to a file.

* `String GetFormattedBodyString()`<br>
 Gets a Hypertext Markup Language (HTML) rendering of this message's text
 body.

* `final List<NamedAddress> getFromAddresses()`<br>
 Deprecated.
Use GetAddresses(\From\) instead.

* `Map.Entry<String,String> GetHeader(int index)`<br>
 Gets the name and value of a header field by index.

* `String GetHeader(String name)`<br>
 Gets the first instance of the header field with the specified name, using a
 basic case-insensitive comparison.

* `String[] GetHeaderArray(String name)`<br>
 Gets an array with the values of all header fields with the specified name,
 using a basic case-insensitive comparison.

* `final List<Map.Entry<String,String>> getHeaderFields()`<br>
 Gets a snapshot of the header fields of this message, in the order in which
 they appear in the message.

* `final List<Message> getParts()`<br>
 Gets a list of all the parts of this message.

* `final String getSubject()`<br>
 Gets this message's subject.

* `final List<NamedAddress> getToAddresses()`<br>
 Deprecated.
Use GetAddresses(\To\) instead.

* `static Message MakeMultilingualMessage(List<Message> messages,
 List<String> languages)`<br>
 Generates a multilingual message (see RFC 8255) from a list of messages and
 a list of language strings.

* `static Message NewBodyPart()`<br>
 Creates a message object with no header fields.

* `Message RemoveHeader(int index)`<br>
 Removes a header field by index.

* `Message RemoveHeader(String name)`<br>
 Removes all instances of the specified header field from this message.

* `Message SelectLanguageMessage(List<String> languages)`<br>
 Selects a body part for a multiple-language message(
 multipart/multilingual) according to the specified language priority list.

* `Message SelectLanguageMessage(List<String> languages,
 boolean preferOriginals)`<br>
 Selects a body part for a multiple-language message(
 multipart/multilingual) according to the specified language priority list
 and original-language preference.

* `Message SetBody(byte[] bytes)`<br>
 Sets the body of this message to the specified byte array.

* `final void setContentDisposition(ContentDisposition value)`<br>
  
* `final void setContentType(MediaType value)`<br>
  
* `Message SetCurrentDate()`<br>
 Sets this message's Date header field to the current time as its value,
 with an unspecified time zone offset.

* `Message SetDate(int[] dateTime)`<br>
 Sets this message's Date header field to the specified date and time.

* `Message SetHeader(int index,
 String value)`<br>
 Sets the value of a header field by index without changing its name.

* `Message SetHeader(int index,
 String name,
 String value)`<br>
 Sets the name and value of a header field by index.

* `Message SetHeader(int index,
 Map.Entry<String,String> header)`<br>
 Sets the name and value of a header field by index.

* `Message SetHeader(String name,
 String value)`<br>
 Sets the value of this message's header field.

* `Message SetHtmlBody(String str)`<br>
 Sets the body of this message to the specified string in Hypertext Markup
 Language (HTML) format.

* `final void setSubject(String value)`<br>
  
* `Message SetTextAndHtml(String text,
 String html)`<br>
 Sets the body of this message to a multipart body with plain text and
 Hypertext Markup Language (HTML) versions of the same message.

* `Message SetTextAndMarkdown(String text,
 String markdown)`<br>
 Sets the body of this message to a multipart body with plain text,
 Markdown, and Hypertext Markup Language (HTML) versions of the same message.

* `Message SetTextBody(String str)`<br>
 Sets the body of this message to the specified plain text string.

* `String ToMailtoUri()`<br>
 Generates a MailTo URI (uniform resource identifier) corresponding to this
 message.

* `String ToMailtoUrl()`<br>
 Deprecated.
Renamed to ToMailtoUri.

## Method Details

### ExtractHeader

    public static String ExtractHeader(byte[] bytes, String headerFieldName)

Extracts the value of a header field from a byte array representing an email
 message. The return value is intended for display purposes, not for further
 processing, and this method is intended to be used as an error handling tool
 for email messages that are slightly malformed. (Note that malformed email
 messages ought to be treated with greater suspicion than well-formed email
 messages.).

**Parameters:**

* <code>bytes</code> - A byte array representing an email message.

* <code>headerFieldName</code> - The name of the header field to extract. This name
 will be compared with the names of header fields in the specified message
 using a basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic uppercase letters A to
 Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

**Returns:**

* The value of the first instance of the header field with the
 specified name. Leading space and/or tab bytes (0x20 and/or 0x09) and CR/LF
 (0x0d/0x0a) pairs will be removed from the header field value, and the value
 is treated as encoded in UTF-8 (an 8-bit encoding form of the Unicode
 Standard) where illegally encoded UTF-8 is replaced as appropriate with
 replacement characters (U+FFFD). Returns null if <code>bytes</code> is null, if
 <code>headerFieldName</code> is null, is more than 997 characters long, or has a
 character less than U+0021 or greater than U+007E in the Unicode Standard,
 if a header field with that name does not exist, or if a body (even an empty
 one) does not follow the header fields.

### NewBodyPart

    public static Message NewBodyPart()

Creates a message object with no header fields.

**Returns:**

* A message object with no header fields.

### SetCurrentDate

    public Message SetCurrentDate()

<p>Sets this message's Date header field to the current time as its value,
 with an unspecified time zone offset. </p> <p>This method can be used when
 the message is considered complete and ready to be generated, for example,
 using the "Generate()" method.</p>

**Returns:**

* This object.

### getBccAddresses

    @Deprecated public final List<NamedAddress> getBccAddresses()

Gets a list of addresses found in the BCC header field or fields.

**Returns:**

* A list of addresses found in the BCC header field or fields.

### getBodyString

    @Deprecated public final String getBodyString()

Gets the body of this message as a text string. See the <code>
 GetBodyString()</code> method.

**Returns:**

* The body of this message as a text string.

**Throws:**

* <code>UnsupportedOperationException</code> - See the <code>GetBodyString()</code> method.

### GetAttachments

    public List<Message> GetAttachments()

<p>Gets a list of descendant body parts of this message that are considered
 attachments. An <i>attachment</i> is a body part or descendant body part
 that has a content disposition with a type other than inline. This message
 itself is not included in the list even if it's an attachment as just
 defined.</p>

**Returns:**

* A list of descendant body parts of this message that are considered
 attachments.

### GetBodyString

    public String GetBodyString()

Gets the body of this message as a text string. If this message's media type
 is "multipart/alternative", returns the result of this method for the last
 supported body part. For any other "multipart" media type, returns the
 result of this method for the first body part for which this method returns
 a text string.

**Returns:**

* The body of this message as a text string.

**Throws:**

* <code>UnsupportedOperationException</code> - This message is a multipart message without a
 supported body part; or this message has a content disposition with a type
 other than "inline"; or this message's media type is a nonmultipart type and
 does not specify the use of a "charset" parameter, has no character encoding
 declared or assumed for it (which is usually the case for nontext messages),
 or has an unsupported character encoding.

### getCCAddresses

    @Deprecated public final List<NamedAddress> getCCAddresses()

Gets a list of addresses found in the CC header field or fields.

**Returns:**

* A list of addresses found in the CC header field or fields.

### GetFormattedBodyString

    public String GetFormattedBodyString()

<p>Gets a Hypertext Markup Language (HTML) rendering of this message's text
 body. This method currently supports any message for which <code>
 GetBodyString()</code> gives out a text string and treats the following media
 types specially: text/plain with <code>format = flowed</code>, text/enriched,
 text/markdown (original Markdown).</p><p>REMARK: The Markdown implementation
 currently supports all features of original Markdown, except that the
 implementation:</p> <ul> <li>does not strictly check the placement of
 "block-level HTML elements",</li><li>does not prevent Markdown content from
 being interpreted as such merely because it's contained in a "block-level
 HTML element", and</li><li>does not deliberately use HTML escapes to
 obfuscate email addresses wrapped in angle-brackets.</li></ul>

**Returns:**

* An HTML rendering of this message's text.

**Throws:**

* <code>UnsupportedOperationException</code> - No supported body part was found; see <code>
 GetBodyString()</code> for more information.

### getContentDisposition

    public final ContentDisposition getContentDisposition()

Gets this message's content disposition. The content disposition specifies
 how a user agent should display or otherwise handle this message. Can be set
 to null. If set to a disposition or to null, updates the Content-Disposition
 header field as appropriate. (There is no default content disposition if
 this value is null, and disposition types unrecognized by the application
 should be treated as "attachment"; see RFC 2183 sec. 2.8.).

**Returns:**

* This message's content disposition, or null if none is specified.

### setContentDisposition

    public final void setContentDisposition(ContentDisposition value)

### getContentType

    public final MediaType getContentType()

Gets this message's media type. When getting, the media type may differ in
 certain cases from the value of the Content-Type header field, if any, and
 may have a value even if the Content-Type header field is absent from this
 message. If set to a media type, updates the Content-Type header field as
 appropriate. Cannot be set to null.

**Returns:**

* This message's media type.

**Throws:**

* <code>NullPointerException</code> - This value is being set and "value" is null.

### setContentType

    public final void setContentType(MediaType value)

### getFileName

    public final String getFileName()

<p>Gets a file name suggested by this message for saving the message's body
 to a file. For more information on the algorithm, see
 ContentDisposition.MakeFilename.</p> <p>This method generates a file name
 based on the <code>filename</code> parameter of the Content-Disposition header
 field, if it exists, or on the <code>name</code> parameter of the Content-Type
 header field, otherwise.</p>

**Returns:**

* A suggested name for the file. Returns the empty string if there is
 no filename suggested by the content type or content disposition, or if that
 filename is an empty string.

### GetAddresses

    public List<NamedAddress> GetAddresses(String headerName)

Gets a list of addresses contained in the header fields with the specified
 name in this message.

**Parameters:**

* <code>headerName</code> - The name of the header fields to retrieve.

**Returns:**

* A list of addresses, in the order in which they appear in this
 message's header fields of the specified name.

**Throws:**

* <code>UnsupportedOperationException</code> - The parameter <code>headerName</code> is not
 supported for this method. Currently, the only header fields supported are
 To, Cc, Bcc, Reply-To, Sender, and From.

* <code>NullPointerException</code> - The parameter <code>headerName</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>headerName</code> is empty.

### getFromAddresses

    @Deprecated public final List<NamedAddress> getFromAddresses()

Gets a list of addresses found in the From header field or fields.

**Returns:**

* A list of addresses found in the From header field or fields.

### getHeaderFields

    public final List<Map.Entry<String,String>> getHeaderFields()

Gets a snapshot of the header fields of this message, in the order in which
 they appear in the message. For each item in the list, the key is the header
 field's name (where any basic uppercase letters, U+0041 to U+005A, are
 converted to basic lowercase letters) and the value is the header field's
 value.

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

Gets this message's subject. The subject's value is found as though
 GetHeader("subject") were called.

**Returns:**

* This message's subject, or null if there is none.

### setSubject

    public final void setSubject(String value)

### getToAddresses

    @Deprecated public final List<NamedAddress> getToAddresses()

Gets a list of addresses found in the To header field or fields.

**Returns:**

* A list of addresses found in the To header field or fields.

### AddHeader

    public Message AddHeader(Map.Entry<String,String> header)

<p>Adds a header field to the end of the message's header. </p> <p>This
 method updates the ContentType and ContentDisposition properties if those
 header fields have been modified by this method.</p>

**Parameters:**

* <code>header</code> - A key/value pair. The key is the name of the header field,
 such as "From" or "Content-ID". The value is the header field's value.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The key or value of <code>header</code> is null.

* <code>IllegalArgumentException</code> - The header field name is too long or contains an
 invalid character, or the header field's value is syntactically invalid.

### AddHeader

    public Message AddHeader(String name, String value)

<p>Adds a header field to the end of the message's header. </p> <p>This
 method updates the ContentType and ContentDisposition properties if those
 header fields have been modified by this method.</p>

**Parameters:**

* <code>name</code> - Name of a header field, such as "From" or "Content-ID" .

* <code>value</code> - Value of the header field.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>value</code> is
 null.

* <code>IllegalArgumentException</code> - The header field name is too long or contains an
 invalid character, or the header field's value is syntactically invalid.

### Generate

    public String Generate()

<p>Generates this message's data in text form. </p><p>The generated message
 will have only Basic Latin code points (U+0000 to U+007F), and the transfer
 encoding will always be 7bit, quoted-printable, or base64 (the declared
 transfer encoding for this message will be ignored).</p> <p>The following
 applies to the following header fields: From, To, Cc, Bcc, Reply-To, Sender,
 Resent-To, Resent-From, Resent-Cc, Resent-Bcc, and Resent-Sender. If the
 header field exists, but has an invalid syntax, has no addresses, or appears
 more than once, this method will combine the addresses into one header field
 if possible (in the case of all fields given other than From and Sender),
 and otherwise generate a synthetic header field with the display-name set to
 the contents of all of the header fields with the same name, and the address
 set to <code>me@[header-name]-address.invalid</code> as the address (a <code>
 .invalid</code> address is a reserved address that can never belong to anyone).
 (An exception is that the Resent-* header fields may appear more than once.)
 The generated message should always have a From header field.</p> <p>If a
 Date and/or Message-ID header field doesn't exist, a field with that name
 will be generated (using the current local time for the Date field).</p>
 <p>When encoding the message's body, if the message has a text content type
 ("text/*"), the line breaks are a CR byte (carriage return, 0x0d) followed
 by an LF byte (line feed, 0x0a), CR alone, or LF alone. If the message has
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

Gets the byte array for this message's body. This method doesn' t make a
 copy of that byte array.

**Returns:**

* A byte array.

### GetDate

    public int[] GetDate()

<p>Gets the date and time extracted from this message's Date header field
 (the value of which is found as though GetHeader("date") were called). See
 <b>MailDateTime.ParseDateString(string, boolean)</b> for more information on
 the format of the date-time array returned by this method.</p>

**Returns:**

* An array of 32-bit unsigned integers.

### SetDate

    public Message SetDate(int[] dateTime)

Sets this message's Date header field to the specified date and time.

**Parameters:**

* <code>dateTime</code> - <p>An array containing at least eight elements expressing a
 date and time. See <b>MailDateTime.ParseDateString(string, boolean)</b> for
 more information on this parameter.</p>

**Returns:**

* This object.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>dateTime</code> contains fewer than
 eight elements or contains invalid values (see
 MailDateTime.ParseDateString(string, boolean)).

* <code>NullPointerException</code> - The parameter <code>dateTime</code> is null.

### GetBodyMessage

    public Message GetBodyMessage()

Returns the mail message contained in this message's body.

**Returns:**

* A message object if this object's content type is "message/rfc822",
 "message/news", or "message/global", or null otherwise.

### GetHeader

    public Map.Entry<String,String> GetHeader(int index)

Gets the name and value of a header field by index.

**Parameters:**

* <code>index</code> - Zero-based index of the header field to get.

**Returns:**

* A key/value pair. The key is the name of the header field, such as
 "From" or "Content-ID". The value is the header field's value.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at least as
 high as the number of header fields.

### GetHeader

    public String GetHeader(String name)

Gets the first instance of the header field with the specified name, using a
 basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic uppercase letters A to
 Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

**Parameters:**

* <code>name</code> - The name of a header field.

**Returns:**

* The value of the first header field with that name, or null if there
 is none.

**Throws:**

* <code>NullPointerException</code> - Name is null.

### GetHeaderArray

    public String[] GetHeaderArray(String name)

Gets an array with the values of all header fields with the specified name,
 using a basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic uppercase letters A to
 Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

**Parameters:**

* <code>name</code> - The name of a header field.

**Returns:**

* An array containing the values of all header fields with the
 specified name, in the order they appear in the message. The array will be
 empty if no header field has that name.

**Throws:**

* <code>NullPointerException</code> - Name is null.

### ClearHeaders

    public Message ClearHeaders()

Deletes all header fields in this message. Also clears this message's
 content disposition and resets its content type to MediaType.TextPlainAscii.

**Returns:**

* This object.

### RemoveHeader

    public Message RemoveHeader(int index)

<p>Removes a header field by index. </p> <p>This method updates the
 ContentType and ContentDisposition properties if those header fields have
 been modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at least as
 high as the number of header fields.

### RemoveHeader

    public Message RemoveHeader(String name)

<p>Removes all instances of the specified header field from this message. If
 this is a multipart message, the header field is not removed from its body
 part headers. A basic case-insensitive comparison is used. (Two strings are
 equal in such a comparison, if they match after converting the basic
 uppercase letters A to Z (U+0041 to U+005A) in both strings to basic
 lowercase letters.). </p> <p>This method updates the ContentType and
 ContentDisposition properties if those header fields have been modified by
 this method.</p>

**Parameters:**

* <code>name</code> - The name of the header field to remove.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetBody

    public Message SetBody(byte[] bytes)

Sets the body of this message to the specified byte array. This method
 doesn't make a copy of that byte array.

**Parameters:**

* <code>bytes</code> - A byte array.

**Returns:**

* This object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> is null.

### SetHeader

    public Message SetHeader(int index, Map.Entry<String,String> header)

<p>Sets the name and value of a header field by index. </p> <p>This method
 updates the ContentType and ContentDisposition properties if those header
 fields have been modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

* <code>header</code> - A key/value pair. The key is the name of the header field,
 such as "From" or "Content-ID". The value is the header field's value.

**Returns:**

* A Message object.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at least as
 high as the number of header fields; or, the header field name is too long
 or contains an invalid character, or the header field's value is
 syntactically invalid.

* <code>NullPointerException</code> - The key or value of <code>header</code> is null.

### SetHeader

    public Message SetHeader(int index, String name, String value)

<p>Sets the name and value of a header field by index. </p> <p>This method
 updates the ContentType and ContentDisposition properties if those header
 fields have been modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

* <code>name</code> - Name of a header field, such as "From" or "Content-ID" .

* <code>value</code> - Value of the header field.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at least as
 high as the number of header fields; or, the header field name is too long
 or contains an invalid character, or the header field's value is
 syntactically invalid.

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>value</code> is
 null.

### SetHeader

    public Message SetHeader(int index, String value)

<p>Sets the value of a header field by index without changing its name. </p>
 <p>This method updates the ContentType and ContentDisposition properties if
 those header fields have been modified by this method.</p>

**Parameters:**

* <code>index</code> - Zero-based index of the header field to set.

* <code>value</code> - Value of the header field.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>index</code> is 0 or at least as
 high as the number of header fields; or, the header field name is too long
 or contains an invalid character, or the header field's value is
 syntactically invalid.

* <code>NullPointerException</code> - The parameter <code>value</code> is null.

### DecodeHeaderValue

    public static String DecodeHeaderValue(String name, String value)

Decodes RFC 2047 encoded words from the specified header field value and
 returns a string with those words decoded. For an example of encoded words,
 see the constructor for PeterO.Mail.NamedAddress.

**Parameters:**

* <code>name</code> - Name of the header field. This determines the syntax of the
 "value" parameter and is necessary to help this method interpret encoded
 words properly.

* <code>value</code> - A header field value that could contain encoded words. For
 example, if the name parameter is "From", this parameter could be
 "=?utf-8?q?me?= &lt;me@example.com&gt;".

**Returns:**

* The header field value with valid encoded words decoded.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetHeader

    public Message SetHeader(String name, String value)

<p>Sets the value of this message's header field. If a header field with the
 same name exists, its value is replaced. If the header field's name occurs
 more than once, only the first instance of the header field is replaced.
 </p> <p>This method updates the ContentType and ContentDisposition
 properties if those header fields have been modified by this method.</p>

**Parameters:**

* <code>name</code> - The name of a header field, such as "from" or "subject" .

* <code>value</code> - The header field's value.

**Returns:**

* This instance.

**Throws:**

* <code>IllegalArgumentException</code> - The header field name is too long or contains an
 invalid character, or the header field's value is syntactically invalid.

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>value</code> is
 null.

### SetHtmlBody

    public Message SetHtmlBody(String str)

Sets the body of this message to the specified string in Hypertext Markup
 Language (HTML) format. The character sequences CR (carriage return, "\r",
 U+000D), LF (line feed, "\n", U+000A), and CR/LF will be converted to CR/LF
 line breaks. Unpaired surrogate code points will be replaced with
 replacement characters.

**Parameters:**

* <code>str</code> - A string consisting of the message in HTML format.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### SetTextAndHtml

    public Message SetTextAndHtml(String text, String html)

Sets the body of this message to a multipart body with plain text and
 Hypertext Markup Language (HTML) versions of the same message. The character
 sequences CR (carriage return, "\r", U+000D), LF (line feed, "\n", U+000A),
 and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate code
 points will be replaced with replacement characters.

**Parameters:**

* <code>text</code> - A string consisting of the plain text version of the message.

* <code>html</code> - A string consisting of the HTML version of the message.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>text</code> or <code>html</code> is
 null.

### SetTextAndMarkdown

    public Message SetTextAndMarkdown(String text, String markdown)

<p>Sets the body of this message to a multipart body with plain text,
 Markdown, and Hypertext Markup Language (HTML) versions of the same message.
 The character sequences CR (carriage return, "\r", U+000D), LF (line feed,
 "\n", U+000A), and CR/LF will be converted to CR/LF line breaks. Unpaired
 surrogate code points will be replaced with replacement
 characters.</p><p>REMARK: The Markdown-to-HTML implementation currently
 supports all features of original Markdown, except that the
 implementation:</p> <ul> <li>does not strictly check the placement of
 "block-level HTML elements",</li><li>does not prevent Markdown content from
 being interpreted as such merely because it's contained in a "block-level
 HTML element", and</li><li>does not deliberately use HTML escapes to
 obfuscate email addresses wrapped in angle-brackets.</li></ul>

**Parameters:**

* <code>text</code> - A string consisting of the plain text version of the message.
 Can be null, in which case the value of the "markdown" parameter is used as
 the plain text version.

* <code>markdown</code> - A string consisting of the Markdown version of the message.
 For interoperability, this Markdown version will be converted to HTML, where
 the Markdown text is assumed to be in the original Markdown flavor.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>markdown</code> is null.

### SetTextBody

    public Message SetTextBody(String str)

Sets the body of this message to the specified plain text string. The
 character sequences CR (carriage return, "\r", U+000D), LF (line feed, "\n",
 U+000A), and CR/LF will be converted to CR/LF line breaks. Unpaired
 surrogate code points will be replaced with replacement characters. This
 method changes this message's media type to plain text.

**Parameters:**

* <code>str</code> - A string consisting of the message in plain text format.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### AddInline

    public Message AddInline(MediaType mediaType)

Adds an inline body part with an empty body and with the specified media
 type to this message. Before the new body part is added, if this message
 isn't already a multipart message, it becomes a "multipart/mixed" message
 with the current body converted to an inline body part.

**Parameters:**

* <code>mediaType</code> - A media type to assign to the body part.

**Returns:**

* A Message object for the generated body part.

### AddAttachment

    public Message AddAttachment(MediaType mediaType)

Adds an attachment with an empty body and with the specified media type to
 this message. Before the new attachment is added, if this message isn't
 already a multipart message, it becomes a "multipart/mixed" message with the
 current body converted to an inline body part.

**Parameters:**

* <code>mediaType</code> - A media type to assign to the attachment.

**Returns:**

* A Message object for the generated attachment.

### AddAttachment

    public Message AddAttachment(InputStream inputStream, MediaType mediaType)

<p>Adds an attachment to this message in the form of data from the specified
 readable stream, and with the specified media type. Before the new
 attachment is added, if this message isn't already a multipart message, it
 becomes a "multipart/mixed" message with the current body converted to an
 inline body part.</p><p> The following example (written in C# for the.NET
 version) is an extension method that adds an attachment from a byte array to
 a message. </p><pre>public static Message AddAttachmentFromBytes(* Message msg, byte[] bytes, MediaType mediaType) { {
MemoryStream fs = null;
try {
fs = * new java.io.ByteArrayInputStream(bytes);
 return msg.AddAttachment(fs, mediaType);
}
finally {
try { if (fs != null) { fs.close(); } } catch (java.io.IOException ex) {}
}
}
 }</pre> .

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the attachment.

**Returns:**

* A Message object for the generated attachment.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>inputStream</code> or <code>
 mediaType</code> is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddAttachment

    public Message AddAttachment(InputStream inputStream, String filename)

Adds an attachment to this message in the form of data from the specified
 readable stream, and with the specified file name. Before the new attachment
 is added, if this message isn't already a multipart message, it becomes a
 "multipart/mixed" message with the current body converted to an inline body
 part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>filename</code> - A file name to assign to the attachment. Can be null or
 empty, in which case no file name is assigned. Only the file name portion of
 this parameter is used, which in this case means the portion of the string
 after the last "/" or "\", if either character exists, or the entire string
 otherwise An appropriate media type (or "application/octet-stream") will be
 assigned to the attachment based on this file name's extension. If the file
 name has an extension .txt, .text, .htm, .html, .shtml, .asc, .brf, .pot,
 .rst, .md, .markdown, or .srt, the media type will have a "charset" of
 "utf-8".

**Returns:**

* A Message object for the generated attachment.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>inputStream</code> is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddAttachment

    public Message AddAttachment(InputStream inputStream, MediaType mediaType, String filename)

Adds an attachment to this message in the form of data from the specified
 readable stream, and with the specified media type and file name. Before the
 new attachment is added, if this message isn't already a multipart message,
 it becomes a "multipart/mixed" message with the current body converted to an
 inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the attachment.

* <code>filename</code> - A file name to assign to the attachment. Can be null or
 empty, in which case no file name is assigned. Only the file name portion of
 this parameter is used, which in this case means the portion of the string
 after the last "/" or "\", if either character exists, or the entire string
 otherwise.

**Returns:**

* A Message object for the generated attachment.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>inputStream</code> or <code>
 mediaType</code> is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddInline

    public Message AddInline(InputStream inputStream, MediaType mediaType)

<p>Adds an inline body part to this message in the form of data from the
 specified readable stream, and with the specified media type. Before the new
 body part is added, if this message isn't already a multipart message, it
 becomes a "multipart/mixed" message with the current body converted to an
 inline body part.</p><p> The following example (written in C# for the.NET
 version) is an extension method that adds an inline body part from a byte
 array to a message. </p><pre>public static Message AddInlineFromBytes(* Message msg, byte[] bytes, MediaType mediaType) { {
MemoryStream fs = null;
try {
fs = * new java.io.ByteArrayInputStream(bytes);
 return msg.AddInline(fs, mediaType);
}
finally {
try { if (fs != null) { fs.close(); } } catch (java.io.IOException ex) {}
}
} }</pre> .

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the body part.

**Returns:**

* A Message object for the generated body part.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>inputStream</code> or <code>
 mediaType</code> is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddInline

    public Message AddInline(InputStream inputStream, String filename)

Adds an inline body part to this message in the form of data from the
 specified readable stream, and with the specified file name. Before the new
 body part is added, if this message isn't already a multipart message, it
 becomes a "multipart/mixed" message with the current body converted to an
 inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>filename</code> - A file name to assign to the inline body part. Can be null
 or empty, in which case no file name is assigned. Only the file name portion
 of this parameter is used, which in this case means the portion of the
 string after the last "/" or "\", if either character exists, or the entire
 string otherwise An appropriate media type (or "application/octet-stream")
 will be assigned to the body part based on this file name's extension. If
 the file name has an extension .txt, .text, .htm, .html, .shtml, .asc, .brf,
 .pot, .rst, .md, .markdown, or .srt, the media type will have a "charset" of
 "utf-8".

**Returns:**

* A Message object for the generated body part.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>inputStream</code> or
 "mediaType" is null.

* <code>MessageDataException</code> - An I/O error occurred.

### AddInline

    public Message AddInline(InputStream inputStream, MediaType mediaType, String filename)

Adds an inline body part to this message in the form of data from the
 specified readable stream, and with the specified media type and file name.
 Before the new body part is added, if this message isn't already a multipart
 message, it becomes a "multipart/mixed" message with the current body
 converted to an inline body part.

**Parameters:**

* <code>inputStream</code> - A readable data stream.

* <code>mediaType</code> - A media type to assign to the body part.

* <code>filename</code> - A file name to assign to the body part.

**Returns:**

* A Message object for the generated body part.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>inputStream</code> or <code>
 mediaType</code> is null.

* <code>MessageDataException</code> - An I/O error occurred.

### SelectLanguageMessage

    public Message SelectLanguageMessage(List<String> languages)

Selects a body part for a multiple-language message(<code>
 multipart/multilingual</code>) according to the specified language priority list.

**Parameters:**

* <code>languages</code> - A list of basic language ranges, sorted in descending order
 of priority (see the LanguageTags.LanguageTagFilter method).

**Returns:**

* The best matching body part for the specified languages. If the body
 part has no subject, then the top-level subject is used. If this message is
 not a multipart/multilingual message or has fewer than two body parts,
 returns this object. If no body part matches the specified languages,
 returns the last body part if its language is "zxx", or the second body part
 otherwise.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>languages</code> is null.

### SelectLanguageMessage

    public Message SelectLanguageMessage(List<String> languages, boolean preferOriginals)

Selects a body part for a multiple-language message(<code>
 multipart/multilingual</code>) according to the specified language priority list
 and original-language preference.

**Parameters:**

* <code>languages</code> - A list of basic language ranges, sorted in descending order
 of priority (see the LanguageTags.LanguageTagFilter method).

* <code>preferOriginals</code> - If true, a body part marked as the original language
 version is chosen if it matches one of the specified language ranges, even
 if the original language has a lower priority than another language with a
 matching body part.

**Returns:**

* The best matching body part for the specified languages. If the body
 part has no subject, then the top-level subject is used. If this message is
 not a multipart/multilingual message or has fewer than two body parts,
 returns this object. If no body part matches the specified languages,
 returns the last body part if its language is "zxx", or the second body part
 otherwise.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>languages</code> is null.

### MakeMultilingualMessage

    public static Message MakeMultilingualMessage(List<Message> messages, List<String> languages)

Generates a multilingual message (see RFC 8255) from a list of messages and
 a list of language strings.

**Parameters:**

* <code>messages</code> - A list of messages forming the parts of the multilingual
 message object. Each message should have the same content, but be in a
 different language. Each message must have a From header field and use the
 same email address in that field as the other messages. The messages should
 be ordered in descending preference of language.

* <code>languages</code> - A list of language strings corresponding to the messages
 given in the "messages" parameter. A language string at a given index
 corresponds to the message at the same index. Each language string must
 follow the syntax of the Content-Language header field (see
 LanguageTags.GetLanguageList).

**Returns:**

* A Message object with the content type "multipart/multilingual" . It
 will begin with an explanatory body part and be followed by the messages
 given in the <code>messages</code> parameter in the order given.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>messages</code> or <code>
 languages</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>messages</code> or <code>
 languages</code> is empty, their lengths don't match, at least one message is
 "null", each message doesn't contain the same email addresses in their From
 header fields, <code>languages</code> contains a syntactically invalid language
 tag list, <code>languages</code> contains the language tag "zzx" not appearing
 alone or at the end of the language tag list, or the first message contains
 no From header field.

### FromMailtoUrl

    @Deprecated public static Message FromMailtoUrl(String url)

<p>Creates a message object from a MailTo URI (uniform resource identifier).
 For more information, see <b>FromMailtoUri(string)</b>.</p>

**Parameters:**

* <code>url</code> - A MailTo URI.

**Returns:**

* A Message object created from the specified MailTo URI. Returs null
 if <code>url</code> is null, is syntactically invalid, or is not a MailTo URI.

### ToMailtoUrl

    @Deprecated public String ToMailtoUrl()

Generates a MailTo URI (uniform resource identifier) corresponding to this
 message. The following header fields, and only these, are used to generate
 the URI: To, Cc, Bcc, In-Reply-To, Subject, Keywords, Comments. The message
 body is included in the URI only if <code>GetBodyString()</code> would return a
 nonempty string. The To header field is included in the URI only if it has
 display names or group syntax.

**Returns:**

* A MailTo URI corresponding to this message.

### FromMailtoUri

    public static Message FromMailtoUri(String uri)

Creates a message object from a MailTo URI (uniform resource identifier).
 The MailTo URI can contain key-value pairs that follow a question-mark, as
 in the following example: "mailto:me@example.com?subject=A%20Subject". In
 this example, "subject" is the subject of the email address. Only certain
 keys are supported, namely, "to", "cc", "bcc", "subject", "in-reply-to",
 "comments", "keywords", and "body". The first seven are header field names
 that will be used to set the returned message's corresponding header fields.
 The last, "body", sets the body of the message to the specified text. Keys
 other than these eight will be ignored. (Keys are compared using a basic
 case-sensitive comparison, in which two strings are equal if they match
 after converting the basic uppercase letters A to Z (U+0041 to U+005A) in
 both strings to basic lowercase letters.) The same key (matched using a
 basic case-insensitive comparison) can appear more than once; for "subject",
 "cc", "bcc", and "in-reply-to", the last value with the specified key is
 used; for "to", all header field values as well as the path are combined to
 a single To header field; for "keywords" and "comments", each value adds
 another header field of the specified key; and for "body", the last value
 with that key is used as the body.

**Parameters:**

* <code>uri</code> - The parameter <code>uri</code> is a text string.

**Returns:**

* A Message object created from the specified MailTo URI. Returs null
 if <code>uri</code> is null, is syntactically invalid, or is not a MailTo URI.

### FromMailtoUri

    public static Message FromMailtoUri(URI uri)

<p>Creates a message object from a MailTo URI (uniform resource identifier)
 in the form of a URI object. For more information, see
 <b>FromMailtoUri(string)</b>.</p>

**Parameters:**

* <code>uri</code> - The MailTo URI in the form of a URI object.

**Returns:**

* A Message object created from the specified MailTo URI. Returs null
 if <code>uri</code> is null, is syntactically invalid, or is not a MailTo URI.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>uri</code> is null.

### ToMailtoUri

    public String ToMailtoUri()

Generates a MailTo URI (uniform resource identifier) corresponding to this
 message. The following header fields, and only these, are used to generate
 the URI: To, Cc, Bcc, In-Reply-To, Subject, Keywords, Comments. The message
 body is included in the URI only if <code>GetBodyString()</code> would return a
 nonempty string.. The To header field is included in the URI only if it has
 display names or group syntax.

**Returns:**

* A MailTo URI corresponding to this message.
