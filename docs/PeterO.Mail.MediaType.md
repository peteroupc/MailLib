## PeterO.Mail.MediaType

    public sealed class MediaType

Specifies what kind of data a message body is.

A media type consists of a top-level type (the general category of the data), a subtype (the specific type), and an optional list of parameters. For example, the media type `text/plain; charset = utf-8` is a text media type ("text"), namely, a plain text type ("plain"), and he parameters say that the data uses UTF-8, a Unicode character ncoding ("charset = utf-8"). Other top-level types include "audio", video", and "application".

A media type is sometimes known as a "MIME type", for Multipurpose Internet Mail Extensions, the standard that introduced media types.

This type is immutable, meaning its values can't be changed once it' s created. To create a changeable media type object, use the MediaTypeBuilder class.

<b>Note:</b>According to RFC 2049, unrecognized subtypes of the top-level type `multipart` must be treated as `multipart/mixed` and unrecognized media types as the media type `application/octet-stream` .

### ApplicationOctetStream

    public static readonly PeterO.Mail.MediaType ApplicationOctetStream;

Specifies the media type "application/octet-stream", used for arbitrary binary data.

### MessageRfc822

    public static readonly PeterO.Mail.MediaType MessageRfc822;

Specifies the media type "message/rfc822", used for Internet mail messages.

### TextPlainAscii

    public static readonly PeterO.Mail.MediaType TextPlainAscii;

Specifies the media type "text/plain" and the "charset" parameter "US-ASCII", used for plain text data.

### TextPlainUtf8

    public static readonly PeterO.Mail.MediaType TextPlainUtf8;

Specifies the media type "text/plain" and the "charset" parameter "utf-8", used for plain text data that may contain characters outside the basic Latin range (U+0000 to U+007F).

### IsMultipart

    public bool IsMultipart { get; }

Gets a value indicating whether this is a multipart media type.

<b>Returns:</b>

 `true`  If this is a multipart media type; otherwise, . `false` .

### IsText

    public bool IsText { get; }

Gets a value indicating whether this is a text media type ("text/*").

<b>Returns:</b>

 `true` If this is a text media type; otherwise, . `false` .

### Parameters

    public System.Collections.Generic.IDictionary Parameters { get; }

Gets a list of the parameters contained in this media type object.

<b>Returns:</b>

A list of the parameters contained in this media type object; the names of each parameter appear in an undefined order. NOTE: Previous versions erroneously stated that the list will be sorted by name. In fact, the names will not be guaranteed to appear in any particular order; this is at least the case in version 0.10.0.

### SubType

    public string SubType { get; }

Gets this media type's subtype (for example, "plain" in "text/plain"). The resulting string will be in lowercase letters.

<b>Returns:</b>

This media type's subtype.

### TopLevelType

    public string TopLevelType { get; }

Gets the name of this media type's top-level type (such as "text" in "text/plain", or "audio" in "audio/basic"). The resulting string will be in lowercase letters.

<b>Returns:</b>

The name of this media type's top-level type (such as "text" or "audio".

### TypeAndSubType

    public string TypeAndSubType { get; }

Gets the top level type and subtype of this media type, separated by a slash; for example, "text/plain". The resulting string will be in lowercase letters.

<b>Returns:</b>

The top level type and subtype of this media type, separated by a slash; for example, "text/plain".

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: The parameter <i>obj</i>
is an arbitrary object.

<b>Return Value:</b>

 `true` if this object and another object are equal; otherwise,  `false` .

### GetCharset

    public string GetCharset();

Gets this media type's "charset" parameter, naming a character encoding used to represent text in the data that uses this media type.

<b>Return Value:</b>

If the "charset" parameter is present and non-empty, returns the result of the Encodings.ResolveAliasForEmail method for that parameter, except that result's basic upper-case letters A to Z (U+0041 to U+005A) are converted to lower case. If the "charset" parameter is absent or empty, returns the default value, if any, for that parameter given the media type (e.g., "us-ascii" if the media type is "text/plain"; see RFC2046), or the empty string if there is none.

### GetHashCode

    public override int GetHashCode();

Calculates the hash code of this object. No application or process IDs are used in the hash code calculation.

<b>Return Value:</b>

A 32-bit signed integer.

### GetParameter

    public string GetParameter(
        string name);

Gets the value of a parameter in this media type, such as "charset" or "format".

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to get. The name is compared using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to lower case.).

<b>Return Value:</b>

The value of the parameter as a string, or null if the parameter doesn't exist.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
 is null.

 * System.ArgumentException:
Name is empty.

### Parse

    public static PeterO.Mail.MediaType Parse(
        string mediaTypeValue);

Parses a media type string and returns a media type object. For further information, see the overload taking a MediaType parameter.

<b>Parameters:</b>

 * <i>mediaTypeValue</i>: A text string representing a media type. This media type can include parameters.

<b>Return Value:</b>

A media type object, or MediaType.TextPlainAscii if <i>mediaTypeValue</i>
is empty or syntactically invalid.

### Parse

    public static PeterO.Mail.MediaType Parse(
        string str,
        PeterO.Mail.MediaType defaultValue);

Parses a media type string and returns a media type object, or the default value if the string is invalid. This method checks the syntactic validity of the string, but not whether it has all parameters it's required to have or whether the parameters themselves are set to valid values for the parameter.This method assumes the given media type string was directly extracted from the Content-Type header field (as defined for email messages) and follows the syntax given in RFC 2045. Accordingly, among other things, the media type string can contain comments (delimited by parentheses).

RFC 2231 extensions allow each media type parameter to be associated with a character encoding and/or language, and support parameter values that span two or more key-value pairs. Parameters making use of RFC 2231 extensions have names with an asterisk ("*"). Such a parameter will be ignored if it is ill-formed because of RFC 2231's rules (except for illegal percent-decoding or undecodable sequences for the given character enoding). Examples of RFC 2231 extensions follow (both examples encode the same "filename" parameter):

<b>text/example; filename*=utf-8'en'filename.txt</b>

<b>text/example; filename*0*=utf-8'en'file; filename*1*=name%2Etxt</b>

This implementation ignores keys (in parameter key-value pairs) that appear more than once in the media type. Nothing in RFCs 2045, 2183, 2231, 6266, or 7231 explicitly disallows such keys, or otherwise specifies error-handling behavior for such keys.

<b>Parameters:</b>

 * <i>str</i>: A text string representing a media type. This media type can include parameters.

 * <i>defaultValue</i>: The media type to return if the string is syntactically invalid. Can be null.

<b>Return Value:</b>

A MediaType object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
is null.

### ToSingleLineString

    public string ToSingleLineString();

Converts this media type to a text string form suitable for inserting in HTTP headers. Notably, the string contains the value of a Content-Type header field (without the text necessarily starting with "Content-Type" followed by a space), and consists of a single line.

<b>Return Value:</b>

A text string form of this media type.

### ToString

    public override string ToString();

Converts this media type to a text string form suitable for inserting in email headers. Notably, the string contains the value of a Content-Type header field (without the text necessarily starting with "Content-Type" followed by a space), and consists of one or more lines.

<b>Return Value:</b>

A text string form of this media type.

### ToUriSafeString

    public string ToUriSafeString();

Converts this media type to a text string form suitable for data URIs. Notably, the string contains the value of a Content-Type header field (without the text necessarily starting with "Content-Type" followed by a space), consists of a single line, and uses percent-encoding as necessary or convenient so that the resulting string can validly appear in a URI path.

<b>Return Value:</b>

A text string form of this media type.
