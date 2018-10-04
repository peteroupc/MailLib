## PeterO.Mail.MediaType

    public sealed class MediaType

Specifies what kind of data a message body is.

A media type consists of a top-level type (the general category of the data), a subtype (the specific type), and an optional list of parameters. For example, the media type `text/plain; charset = utf-8` is a text media type ("text"), namely, a plain text type ("plain"), and he parameters say that the data uses UTF-8, a Unicode character ncoding ("charset = utf-8"). Other top-level types include "audio", video", and "application".

A media type is sometimes known as a "MIME type", for Multipurpose Internet Mail Extensions, the standard that introduced media types.

This type is immutable, meaning its values can't be changed once it' s created. To create a changeable media type object, use the MediaTypeBuilder class.

<b>Note:</b>According to RFC 2049, unrecognized subtypes of the top-level type `multipart` must be treated as `multipart/mixed` and unrecognized media types as the media type `application/octet-stream` .


### Member Summary
* <code>[public static readonly PeterO.Mail.MediaType ApplicationOctetStream;](#ApplicationOctetStream)</code> - Specifies the media type "application/octet-stream", used for arbitrary binary data.
* <code>[Equals(object)](#Equals_object)</code> - Determines whether this object and another object are equal.
* <code>[GetCharset()](#GetCharset)</code> - Gets this media type's "charset" parameter, naming a character encoding used to represent text in the data that uses this media type.
* <code>[GetHashCode()](#GetHashCode)</code> - Calculates the hash code of this object.
* <code>[GetParameter(string)](#GetParameter_string)</code> - Gets the value of a parameter in this media type, such as "charset" or "format".
* <code>[IsMultipart](#IsMultipart)</code> - Gets a value indicating whether this is a multipart media type.
* <code>[IsText](#IsText)</code> - Gets a value indicating whether this is a text media type ("text/*").
* <code>[public static readonly PeterO.Mail.MediaType MessageRfc822;](#MessageRfc822)</code> - Specifies the media type "message/rfc822", used for Internet mail messages.
* <code>[Parameters](#Parameters)</code> - Gets a list of the parameters contained in this media type object.
* <code>[Parse(string)](#Parse_string)</code> - Parses a media type string and returns a media type object.
* <code>[Parse(string, PeterO.Mail.MediaType)](#Parse_string_PeterO_Mail_MediaType)</code> - Parses a media type string and returns a media type object, or the default value if the string is invalid.
* <code>[SubType](#SubType)</code> - Gets this media type's subtype (for example, "plain" in "text/plain").
* <code>[public static readonly PeterO.Mail.MediaType TextPlainAscii;](#TextPlainAscii)</code> - Specifies the media type "text/plain" and the "charset" parameter "US-ASCII", used for plain text data.
* <code>[public static readonly PeterO.Mail.MediaType TextPlainUtf8;](#TextPlainUtf8)</code> - Specifies the media type "text/plain" and the "charset" parameter "utf-8", used for plain text data that may contain characters outside the basic Latin range (U+0000 to U+007F).
* <code>[ToSingleLineString()](#ToSingleLineString)</code> - Converts this media type to a text string form suitable for inserting in HTTP headers.
* <code>[ToString()](#ToString)</code> - Converts this media type to a text string form suitable for inserting in email headers.
* <code>[ToUriSafeString()](#ToUriSafeString)</code> - Converts this media type to a text string form suitable for data URIs.
* <code>[TopLevelType](#TopLevelType)</code> - Gets the name of this media type's top-level type (such as "text" in "text/plain", or "audio" in "audio/basic").
* <code>[TypeAndSubType](#TypeAndSubType)</code> - Gets the top level type and subtype of this media type, separated by a slash; for example, "text/plain".

<a id="ApplicationOctetStream"></a>

### Parse

    public static PeterO.Mail.MediaType Parse(
        string mediaTypeValue);

Parses a media type string and returns a media type object. For further information, see the overload taking a MediaType parameter.

<b>Parameters:</b>

 * <i>mediaTypeValue</i>: A text string representing a media type. This media type can include parameters.

<b>Return Value:</b>

A media type object, or MediaType.TextPlainAscii if <i>mediaTypeValue</i>
is empty or syntactically invalid.

<a id="Parse_string_PeterO_Mail_MediaType"></a>
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


<a id="ToSingleLineString"></a>


<b>Return Value:</b>

A text string form of this media type.

<a id="ToString"></a>
### ToString

    public override string ToString();

Converts this media type to a text string form suitable for inserting in email headers. Notably, the string contains the value of a Content-Type header field (without the text necessarily starting with "Content-Type" followed by a space), and consists of one or more lines.

<b>Return Value:</b>

A text string form of this media type.


<a id="ToUriSafeString"></a>

### ToUriSafeString

    public string ToUriSafeString();

Converts this media type to a text string form suitable for data URIs. Notably, the string contains the value of a Content-Type header field (without the text necessarily starting with "Content-Type" followed by a space), consists of a single line, and uses percent-encoding as necessary or convenient so that the resulting string can validly appear in a URI path.

<b>Return Value:</b>

A text string form of this media type.
