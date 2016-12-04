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
 content-transfer-encoding "quoted-printable" is treated as 7bit
 instead if it occurs in a message or body part with content type
 "multipart/*" or "message/*" (other than "message/global",
 "message/global-headers", "message/global-disposition-notification",
 or "message/global-delivery-status").</li> <li>If a message has two
 or more Content-Type header fields, it is treated as having a content
 type of "application/octet-stream", unless one or more of the header
 fields is syntactically invalid.</li> <li>Illegal UTF-8 byte
 sequences appearing in header field values are replaced with
 replacement characters. Moreover, UTF-8 is parsed everywhere in
 header field values, even in those parts of some structured header
 fields where this appears not to be allowed. (UTF-8 is a character
 encoding for the Unicode character set.)</li> <li>The To and Cc
 header fields are allowed to contain only comments and whitespace,
 but these "empty" header fields will be omitted when generating.</li>
 <li>There is no line length limit imposed when parsing header fields,
 except header field names.</li> <li>There is no line length limit
 imposed when parsing quoted-printable or base64 encoded bodies.</li>
 <li>If the transfer encoding is absent and the content type is
 "message/rfc822", bytes with values greater than 127 (called "8-bit
 bytes" in the rest of this summary) are still allowed, despite the
 default value of "7bit" for "Content-Transfer-Encoding".</li> <li>In
 the following cases, if the transfer encoding is absent or declared
 as 7bit, 8-bit bytes are still allowed:</li> <li>(a) The preamble and
 epilogue of multipart messages, which will be ignored.</li> <li>(b)
 If the charset is declared to be <code>utf-8</code>.</li> <li>(c) If the
 content type is "text/html" and the charset is declared to be
 <code>ascii</code>, <code>us-ascii</code>, "windows-1252", "windows-1251", or
 "iso-8859-*" (all single byte encodings).</li> <li>(d) In non-MIME
 message bodies and in text/plain message bodies. Any 8-bit bytes are
 replaced with the substitute character byte (0x1a).</li> <li>If the
 first line of the message starts with the word "From" followed by a
 space, it is skipped.</li> <li>The name <code>ascii</code> is treated as a
 synonym for <code>us-ascii</code>, despite being a reserved name under RFC
 2046. The name <code>cp1252</code> is treated as a synonym for
 <code>windows-1252</code> , even though it's not an IANA registered
 alias.</li> <li>The following deviations involve encoded words under
 RFC 2047:</li> <li>(a) If a sequence of encoded words decodes to a
 string with a CTL character (U + 007F, or a character less than U + 0020
 and not TAB) after being converted to Unicode, the encoded words are
 left un-decoded.</li> <li>(b) This implementation can decode an
 encoded word that uses ISO-2022-JP (the only supported encoding that
 uses code switching) even if the encoded word's payload ends in a
 different mode from "ASCII mode". (Each encoded word still starts in
 that mode, though.)</li></ul> ---.

## Methods

* `Message() Message`<br>
 Initializes a new instance of the Message
 class.
* `Message(byte[] bytes) Message`<br>
 Initializes a new instance of the Message
 class.
* `Message(InputStream stream) Message`<br>
 Initializes a new instance of the Message
 class.
* `Message AddHeader(Map.Entry<String,String> header)`<br>
 Adds a header field to the end of the message's header.
* `Message AddHeader(String name,
         String value)`<br>
 Adds a header field to the end of the message's header.
* `String Generate()`<br>
 Generates this message's data in text form.
* `List<NamedAddress> getBccAddresses()`<br>
 Gets a list of addresses found in the BCC header field or fields.
* `byte[] GetBody()`<br>
 Gets the byte array for this message's body.
* `Message GetBodyMessage()`<br>
 Returns the mail message contained in this message's body.
* `String getBodyString()`<br>
 Gets the body of this message as a text string.
* `List<NamedAddress> getCCAddresses()`<br>
 Gets a list of addresses found in the CC header field or fields.
* `ContentDisposition getContentDisposition()`<br>
 Gets this message's content disposition.
* `MediaType getContentType()`<br>
 Gets this message's media type.
* `int[] GetDate()`<br>
 Gets the date and time extracted from this message's Date header field (as
 though GetHeader("date") were called).
* `String getFileName()`<br>
 Gets a filename suggested by this message for saving the message's body to a
 file.
* `List<NamedAddress> getFromAddresses()`<br>
 Gets a list of addresses found in the From header field or fields.
* `Map.Entry<String,String> GetHeader(int index)`<br>
 Gets the name and value of a header field by index.
* `String GetHeader(String name)`<br>
 Gets the first instance of the header field with the specified name, using a
 basic case-insensitive comparison.
* `String[] GetHeaderArray(String name)`<br>
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
 Gets a list of addresses found in the To header field or fields.
* `Message RemoveHeader(int index)`<br>
 Removes a header field by index.
* `Message RemoveHeader(String name)`<br>
 Removes all instances of the given header field from this message.
* `Message SetBody(byte[] bytes)`<br>
 Sets the body of this message to the given byte array.
* `void setContentDisposition(ContentDisposition value)`<br>
* `void setContentType(MediaType value)`<br>
* `Message SetCurrentDate()`<br>
 Sets this message's Date header field to the current time as its value.
* `Message SetDate(int[] dateTime)`<br>
 Sets this message's Date header field to the given date and time.
* `Message SetHeader(int index,
         Map.Entry<String,String> header)`<br>
 Sets the name and value of a header field by index.
* `Message SetHeader(int index,
         String value)`<br>
 Sets the value of a header field by index without changing its name.
* `Message SetHeader(int index,
         String name,
         String value)`<br>
 Sets the name and value of a header field by index.
* `Message SetHeader(String name,
         String value)`<br>
 Sets the value of this message's header field.
* `Message SetHtmlBody(String str)`<br>
 Sets the body of this message to the specified string in HTML format.
* `void setSubject(String value)`<br>
* `Message SetTextAndHtml(String text,
              String html)`<br>
 Sets the body of this message to a multipart body with plain text and HTML
 versions of the same message.
* `Message SetTextBody(String str)`<br>
 Sets the body of this message to the specified plain text string.

## Constructors

* `Message() Message`<br>
 Initializes a new instance of the Message
 class.
* `Message(byte[] bytes) Message`<br>
 Initializes a new instance of the Message
 class.
* `Message(InputStream stream) Message`<br>
 Initializes a new instance of the Message
 class.

## Method Details

### Message
    public Message(InputStream stream)
Initializes a new instance of the <code>Message</code>
 class. Reads from the given InputStream object to initialize the message.

**Parameters:**

* <code>stream</code> - A readable data stream.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>stream</code> is null.

### Message
    public Message(byte[] bytes)
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
### SetCurrentDate
    public Message SetCurrentDate()
Sets this message's Date header field to the current time as its value.
 <p>This method can be used when the message is considered complete
 and ready to be generated, for example, using the "Generate()"
 method.</p>

**Returns:**

* This object.

### getBccAddresses
    public final List<NamedAddress> getBccAddresses()
Gets a list of addresses found in the BCC header field or fields.

**Returns:**

* A list of addresses found in the BCC header field or fields.

### getBodyString
    public final String getBodyString()
Gets the body of this message as a text string.

**Returns:**

* The body of this message as a text string.

**Throws:**

* <code>UnsupportedOperationException</code> - This message has no character encoding
 declared on it (which is usually the case for non-text messages), or
 the character encoding is not supported.

### getCCAddresses
    public final List<NamedAddress> getCCAddresses()
Gets a list of addresses found in the CC header field or fields.

**Returns:**

* A list of addresses found in the CC header field or fields.

### getContentDisposition
    public final ContentDisposition getContentDisposition()
Gets this message's content disposition. The content disposition specifies
 how a user agent should handle or otherwise display this message. Can
 be set to null.

**Returns:**

* This message's content disposition, or null if none is specified.

### setContentDisposition
    public final void setContentDisposition(ContentDisposition value)
### getContentType
    public final MediaType getContentType()
Gets this message's media type.

**Returns:**

* This message's media type.

**Throws:**

* <code>NullPointerException</code> - This value is being set and "value" is
 null.

### setContentType
    public final void setContentType(MediaType value)
### getFileName
    public final String getFileName()
Gets a filename suggested by this message for saving the message's body to a
 file. For more information on the algorithm, see
 ContentDisposition.MakeFilename.

**Returns:**

* A suggested name for the file, or the empty string if there is no
 filename suggested by the content type or content disposition.

### getFromAddresses
    public final List<NamedAddress> getFromAddresses()
Gets a list of addresses found in the From header field or fields.

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
    public final void setSubject(String value)
### getToAddresses
    public final List<NamedAddress> getToAddresses()
Gets a list of addresses found in the To header field or fields.

**Returns:**

* A list of addresses found in the To header field or fields.

### AddHeader
    public Message AddHeader(Map.Entry<String,String> header)
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
    public Message AddHeader(String name, String value)
Adds a header field to the end of the message's header. <p>Updates the
 ContentType and ContentDisposition properties if those header fields
 have been modified by this method.</p>

**Parameters:**

* <code>name</code> - Name of a header field, such as "From" or "Content-ID".

* <code>value</code> - Value of the header field.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>value</code> is null.

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
 to anyone). (An exception is that the Resent-From and Resent-Sender
 header fields may appear more than once.) The generated message
 should always have a From header field.</p> <p>If a Date and/or
 Message-ID header field doesn't exist, a field with that name will be
 generated (using the current local time for the Date field).</p>
 <p>When encoding the message's body, if the message has a text
 content type ("text/*"), the line breaks are a CR byte (carriage
 return, 0x0d) followed by an LF byte (line feed, 0x0a), CR alone, or
 LF alone. If the message has any other content type, only CR followed
 by LF is considered a line break.</p>

**Returns:**

* The generated message.

**Throws:**

* <code>PeterO.Mail.MessageDataException</code> - The message can't be generated.

### GetBody
    public byte[] GetBody()
Gets the byte array for this message's body. This method doesn't make a copy
 of that byte array.

**Returns:**

* A byte array.

### GetDate
    public int[] GetDate()
Gets the date and time extracted from this message's Date header field (as
 though GetHeader("date") were called). Each element of the array
 (starting from 0) is as follows: <ul> <li>0 - The year. For example,
 the value 2000 means 2000 C.E.</li> <li>1 - Month of the year, from 1
 (January) through 12 (December).</li> <li>2 - Day of the month, from
 1 through 31.</li> <li>3 - Hour of the day, from 0 through 23.</li>
 <li>4 - Minute of the hour, from 0 through 59.</li> <li>5 - Second of
 the minute, from 0 through 60 (this value can go up to 60 to
 accommodate leap seconds). (Leap seconds are additional seconds added
 to adjust international atomic time, or TAI, to an approximation of
 astronomical time known as coordinated universal time, or UTC.)</li>
 <li>6 - Milliseconds of the second, from 0 through 999. Will always
 be 0.</li> <li>7 - Number of minutes to subtract from this date and
 time to get global time. This number can be positive or
 negative.</li></ul>

**Returns:**

* An array containing eight elements. Returns null if the Date header
 doesn't exist, if the Date field is syntactically or semantically
 invalid, or if the field's year would overflow a 32-bit signed
 integer.

### SetDate
    public Message SetDate(int[] dateTime)
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
    public Map.Entry<String,String> GetHeader(int index)
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
    public String GetHeader(String name)
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
    public String[] GetHeaderArray(String name)
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

### RemoveHeader
    public Message RemoveHeader(int index)
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
    public Message RemoveHeader(String name)
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
    public Message SetBody(byte[] bytes)
Sets the body of this message to the given byte array. This method doesn't
 make a copy of that byte array.

**Parameters:**

* <code>bytes</code> - A byte array.

**Returns:**

* This object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> is null.

### SetHeader
    public Message SetHeader(int index, Map.Entry<String,String> header)
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
    public Message SetHeader(int index, String name, String value)
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

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>value</code> is null.

### SetHeader
    public Message SetHeader(int index, String value)
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
    public Message SetHeader(String name, String value)
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

* <code>NullPointerException</code> - The parameter <code>name</code> or <code>value</code> is null.

### SetHtmlBody
    public Message SetHtmlBody(String str)
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
    public Message SetTextAndHtml(String text, String html)
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

* <code>NullPointerException</code> - The parameter <code>text</code> or <code>html</code> is null.

### SetTextBody
    public Message SetTextBody(String str)
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
