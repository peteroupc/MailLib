## PeterO.Mail.MediaType

    public sealed class MediaType

Specifies what kind of data a message body is.

A media type consists of a top-level type (the general category of the data), a subtype (the specific type), and an optional list of parameters. For example, the media type  `text/plain; charset = utf-8` is a text media type ("text"), namely, a plain text type ("plain"), and the parameters say that that the data uses the character set UTF-8, a form of Unicode ("charset=utf-8"). Other top-level types include "audio", "video", and "application".

This type is immutable, meaning its values can't be changed once it' s created. To create a changeable media type object, use the MediaTypeBuilder class.

### ApplicationOctetStream

    public static readonly PeterO.Mail.MediaType ApplicationOctetStream;

Specifies the media type "application/octet-stream", used for arbitrary binary data.

### MessageRfc822

    public static readonly PeterO.Mail.MediaType MessageRfc822;

Specifies the media type "message/rfc822" , used for Internet mail messages.

### TextPlainAscii

    public static readonly PeterO.Mail.MediaType TextPlainAscii;

Specifies the media type "text/plain" and the charset "US-ASCII", used for plain text data.

### TextPlainUtf8

    public static readonly PeterO.Mail.MediaType TextPlainUtf8;

Specifies the media type "text/plain" and the charset "utf-8", used for Unicode plain text data.

### IsMultipart

    public bool IsMultipart { get; }

Gets a value indicating whether this is a multipart media type.

<b>Returns:</b>

True if this is a multipart media type; otherwise, false..

### IsText

    public bool IsText { get; }

Gets a value indicating whether this is a text media type ("text/*").

<b>Returns:</b>

True if this is a text media type; otherwise, false..

### Parameters

    public System.Collections.Generic.IDictionary Parameters { get; }

Gets a sorted list of the parameters contained in this media type object.

<b>Returns:</b>

A list of the parameters contained in this media type object, sorted by name.

### SubType

    public string SubType { get; }

Gets this media type's subtype.

<b>Returns:</b>

This media type's subtype.

### TopLevelType

    public string TopLevelType { get; }

Gets a value not documented yet.

<b>Returns:</b>

A value not documented yet.

### TypeAndSubType

    public string TypeAndSubType { get; }

Gets the top level type and subtype of this media type, separated by a slash; for example, "text/plain".

<b>Returns:</b>

The top level type and subtype of this media type, separated by a slash; for example, "text/plain".

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Returns:</b>

True if the objects are equal; otherwise, false.

### GetCharset

    public string GetCharset();

Returns the charset parameter, converted to ASCII lower-case, if it exists, or  `"us-ascii"` if the media type is ill-formed (RFC2045 sec. 5.2), or if the media type is "text/plain" and doesn't have a charset parameter (see RFC2046), or the default charset, if any, for the media type if the charset parameter is absent. Returns an empty string in all other cases.

<b>Returns:</b>

A string object.

### GetHashCode

    public override int GetHashCode();

Returns the hash code for this instance.

<b>Returns:</b>

A 32-bit hash code.

### GetParameter

    public string GetParameter(
        string name);

Gets the value of a parameter in this media type, such as "charset".

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to get. The name is compared case-insensitively.

<b>Returns:</b>

The value of the parameter as a string, or null if the parameter doesn't exist.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

### Parse

    public static PeterO.Mail.MediaType Parse(
        string mediaTypeValue);

Parses a media type string and returns a media type object.

<b>Parameters:</b>

 * <i>mediaTypeValue</i>: A string object.

<b>Returns:</b>

A media type object, or text/plain if  <i>mediaTypeValue</i>
 is empty or syntactically invalid.

### Parse

    public static PeterO.Mail.MediaType Parse(
        string str,
        PeterO.Mail.MediaType defaultValue);

Parses a media type string and returns a media type object, or the default value if the string is invalid.

<b>Parameters:</b>

 * <i>str</i>: A string object representing a media type.

 * <i>defaultValue</i>: The media type to return if the string is syntactically invalid. Can be null.

<b>Returns:</b>

A MediaType object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
is null.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string representation of this object.
