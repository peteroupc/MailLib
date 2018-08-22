# com.upokecenter.mail.MediaType

    public final class MediaType extends Object

<p>Specifies what kind of data a message body is. </p> <p>A media type
 consists of a top-level type (the general category of the data), a
 subtype (the specific type), and an optional list of parameters. For
 example, the media type <code>text/plain; charset = utf-8</code> is a text
 media type ("text"), namely, a plain text type ("plain"), and the
 parameters say that the data uses UTF-8, a Unicode character encoding
 ("charset = utf-8"). Other top-level types include "audio", "video",
 and "application". </p> <p>A media type is sometimes known as a "MIME
 type", for Multipurpose Internet Mail Extensions, the standard that
 introduced media types. </p> <p>This type is immutable, meaning its
 values can't be changed once it' s created. To create a changeable
 media type object, use the MediaTypeBuilder class. </p>
 <p><b>Note:</b> According to RFC 2049, unrecognized subtypes of the
 top-level type <code>multipart</code> must be treated as
 <code>multipart/mixed</code> and unrecognized media types as the media type
 <code>application/octet-stream</code> . </p>

## Fields

* `static MediaType ApplicationOctetStream`<br>
 Specifies the media type "application/octet-stream", used for arbitrary
 binary data.
* `static MediaType MessageRfc822`<br>
 Specifies the media type "message/rfc822", used for Internet mail messages.
* `static MediaType TextPlainAscii`<br>
 Specifies the media type "text/plain" and the "charset" parameter
 "US-ASCII", used for plain text data.
* `static MediaType TextPlainUtf8`<br>
 Specifies the media type "text/plain" and the "charset" parameter "utf-8",
 used for plain text data that may contain characters outside the
 basic Latin range (U + 0000 to U + 007F).

## Methods

* `boolean equals​(Object obj)`<br>
 Determines whether this object and another object are equal.
* `String GetCharset()`<br>
 Gets this media type's "charset" parameter, naming a character encoding used
 to represent text in the data that uses this media type.
* `String GetParameter​(String name)`<br>
 Gets the value of a parameter in this media type, such as "charset" or
 "format".
* `Map<String,String> getParameters()`<br>
 Gets a list of the parameters contained in this media type object.
* `String getSubType()`<br>
 Gets this media type's subtype.
* `String getTopLevelType()`<br>
 Gets the name of this media type's top-level type (such as "text" or
 "audio").
* `String getTypeAndSubType()`<br>
 Gets the top level type and subtype of this media type, separated by a
 slash; for example, "text/plain".
* `int hashCode()`<br>
 Calculates the hash code of this object.
* `boolean isMultipart()`<br>
 Gets a value indicating whether this is a multipart media type.
* `boolean isText()`<br>
 Gets a value indicating whether this is a text media type ("text/*").
* `static MediaType Parse​(String mediaTypeValue)`<br>
 Parses a media type string and returns a media type object.
* `static MediaType Parse​(String str,
     MediaType defaultValue)`<br>
 Parses a media type string and returns a media type object, or the default
 value if the string is invalid.
* `String ToSingleLineString()`<br>
 Converts this media type to a text string form suitable for inserting in
 HTTP headers.
* `String toString()`<br>
 Converts this media type to a text string form suitable for inserting in
 email headers.
* `String ToUriSafeString()`<br>
 Converts this media type to a text string form suitable for data URIs.

## Field Details

### TextPlainAscii
    public static final MediaType TextPlainAscii
Specifies the media type "text/plain" and the "charset" parameter
 "US-ASCII", used for plain text data.
### TextPlainUtf8
    public static final MediaType TextPlainUtf8
Specifies the media type "text/plain" and the "charset" parameter "utf-8",
 used for plain text data that may contain characters outside the
 basic Latin range (U + 0000 to U + 007F).
### MessageRfc822
    public static final MediaType MessageRfc822
Specifies the media type "message/rfc822", used for Internet mail messages.
### ApplicationOctetStream
    public static final MediaType ApplicationOctetStream
Specifies the media type "application/octet-stream", used for arbitrary
 binary data.
## Method Details

### getTopLevelType
    public final String getTopLevelType()
Gets the name of this media type's top-level type (such as "text" or
 "audio").

**Returns:**

* The name of this media type's top-level type (such as "text" or
 "audio".

### equals
    public boolean equals​(Object obj)
Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code>&nbsp;in class&nbsp;<code>Object</code>

**Parameters:**

* <code>obj</code> - The parameter <code>obj</code> is an arbitrary object.

**Returns:**

* <code>true</code> if this object and another object are equal; otherwise,
 <code>false</code> .

### hashCode
    public int hashCode()
Calculates the hash code of this object. No application or process IDs are
 used in the hash code calculation.

**Overrides:**

* <code>hashCode</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A 32-bit signed integer.

### getSubType
    public final String getSubType()
Gets this media type's subtype.

**Returns:**

* This media type's subtype.

### isText
    public final boolean isText()
Gets a value indicating whether this is a text media type ("text/*").

**Returns:**

* <code>true</code> If this is a text media type; otherwise, . <code>
 false</code>.

### isMultipart
    public final boolean isMultipart()
Gets a value indicating whether this is a multipart media type.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, . <code>
 false</code>.

### getParameters
    public final Map<String,String> getParameters()
Gets a list of the parameters contained in this media type object.

**Returns:**

* A list of the parameters contained in this media type object; the
 names of each parameter appear in an undefined order. NOTE: Previous
 versions erroneously stated that the list will be sorted by name. In
 fact, the names will not be guaranteed to appear in any particular
 order; this is at least the case in version 0.10.0.

### toString
    public String toString()
Converts this media type to a text string form suitable for inserting in
 email headers. Notably, the string contains the value of a
 Content-Type header field (without the text necessarily starting with
 "Content-Type" followed by a space), and consists of one or more
 lines.

**Overrides:**

* <code>toString</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A text string form of this media type.

### ToSingleLineString
    public String ToSingleLineString()
Converts this media type to a text string form suitable for inserting in
 HTTP headers. Notably, the string contains the value of a
 Content-Type header field (without the text necessarily starting with
 "Content-Type" followed by a space), and consists of a single line.

**Returns:**

* A text string form of this media type.

### ToUriSafeString
    public String ToUriSafeString()
Converts this media type to a text string form suitable for data URIs.
 Notably, the string contains the value of a Content-Type header field
 (without the text necessarily starting with "Content-Type" followed
 by a space), consists of a single line, and uses percent-encoding as
 necessary or convenient so that the resulting string can validly
 appear in a URI path.

**Returns:**

* A text string form of this media type.

### GetCharset
    public String GetCharset()
Gets this media type's "charset" parameter, naming a character encoding used
 to represent text in the data that uses this media type.

**Returns:**

* If the "charset" parameter is present and non-empty, returns the
 result of the Encodings.ResolveAliasForEmail method for that
 parameter, except that result's basic upper-case letters A to Z
 (U+0041 to U+005A) are converted to lower case. If the "charset"
 parameter is absent or empty, returns the default value, if any, for
 that parameter given the media type (e.g., "us-ascii" if the media
 type is "text/plain"; see RFC2046), or the empty string if there is
 none.

### GetParameter
    public String GetParameter​(String name)
Gets the value of a parameter in this media type, such as "charset" or
 "format".

**Parameters:**

* <code>name</code> - Name of the parameter to get. The name is compared using a basic
 case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).

**Returns:**

* The value of the parameter as a string, or null if the parameter
 doesn't exist.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

* <code>IllegalArgumentException</code> - Name is empty.

### getTypeAndSubType
    public final String getTypeAndSubType()
Gets the top level type and subtype of this media type, separated by a
 slash; for example, "text/plain".

**Returns:**

* The top level type and subtype of this media type, separated by a
 slash; for example, "text/plain".

### Parse
    public static MediaType Parse​(String mediaTypeValue)
Parses a media type string and returns a media type object. For further
 information, see the overload taking a MediaType parameter.

**Parameters:**

* <code>mediaTypeValue</code> - A text string representing a media type. This media
 type can include parameters.

**Returns:**

* A media type object, or MediaType.TextPlainAscii if <code>
 mediaTypeValue</code> is empty or syntactically invalid.

### Parse
    public static MediaType Parse​(String str, MediaType defaultValue)
Parses a media type string and returns a media type object, or the default
 value if the string is invalid. This method checks the syntactic
 validity of the string, but not whether it has all parameters it's
 required to have or whether the parameters themselves are set to
 valid values for the parameter. <p>This method assumes the given
 media type string was directly extracted from the Content-Type header
 field (as defined for email messages) and follows the syntax given in
 RFC 2045. Accordingly, among other things, the media type string can
 contain comments (delimited by parentheses). </p> <p>RFC 2231
 extensions allow each media type parameter to be associated with a
 character encoding and/or language, and support parameter values that
 span two or more key-value pairs. Parameters making use of RFC 2231
 extensions have names with an asterisk ("*"). Such a parameter will
 be ignored if it is ill-formed because of RFC 2231's rules (except
 for illegal percent-decoding or undecodable sequences for the given
 character enoding). Examples of RFC 2231 extensions follow (both
 examples encode the same "filename" parameter): </p>
 <p><b>text/example; filename*=utf-8'en'filename.txt</b> </p>
 <p><b>text/example; filename*0*=utf-8'en'file;
 filename*1*=name%2Etxt</b> </p> <p>This implementation ignores keys
 (in parameter key-value pairs) that appear more than once in the
 media type. Nothing in RFCs 2045, 2183, 2231, 6266, or 7231
 explicitly disallows such keys, or otherwise specifies error-handling
 behavior for such keys. </p>

**Parameters:**

* <code>str</code> - A text string representing a media type. This media type can
 include parameters.

* <code>defaultValue</code> - The media type to return if the string is syntactically
 invalid. Can be null.

**Returns:**

* A MediaType object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.
