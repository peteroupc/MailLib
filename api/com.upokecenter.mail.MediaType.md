# com.upokecenter.mail.MediaType

## Nested Classes

* `static class  MediaType.Builder`<br>
 A mutable data type that allows a media type object to be built.

## Fields

* `static MediaType ApplicationOctetStream`<br>
 Specifies the media type "application/octet-stream", used for arbitrary
 binary data.
* `static MediaType MessageRfc822`<br>
 Specifies the media type "message/rfc822", used for Internet mail messages.
* `static MediaType TextPlainAscii`<br>
 Specifies the media type "text/plain" and the "charset" parameter
  "US-ASCII", used for plain text data that contains only characters
 within the basic Latin range (U+0000 to U+007F).
* `static MediaType TextPlainUtf8`<br>
 Specifies the media type "text/plain" and the "charset" parameter "utf-8",
 used for plain text data that may contain characters outside the
 basic Latin range (U+0000 to U+007F).

## Methods

* `boolean equals​(java.lang.Object obj)`<br>
 Determines whether this object and another object are equal.
* `java.lang.String GetCharset()`<br>
 Gets this media type's "charset" parameter, naming a character encoding used
 to represent text in the data that uses this media type.
* `java.lang.String GetParameter​(java.lang.String name)`<br>
 Gets the value of a parameter in this media type, such as "charset" or
  "format".
* `java.util.Map<java.lang.String,​java.lang.String> getParameters()`<br>
 Gets a list of the parameter names contained in this media type object and
 their values.
* `java.lang.String getSubType()`<br>
 Gets this media type's subtype (for example, "plain" in "text/plain").
* `java.lang.String getTopLevelType()`<br>
 Gets the name of this media type's top-level type (such as "text" in
  "text/plain", or "audio" in "audio/basic").
* `java.lang.String getTypeAndSubType()`<br>
 Gets the top level type and subtype of this media type, separated by a
  slash; for example, "text/plain".
* `int hashCode()`<br>
 Calculates the hash code of this object.
* `boolean HasStructuredSuffix​(java.lang.String suffix)`<br>
 Returns whether this media type's subtype has the given structured syntax
 suffix.
* `boolean isMultipart()`<br>
 Gets a value indicating whether this is a multipart media type.
* `boolean isText()`<br>
 Gets a value indicating whether this is a text media type ("text/*").
* `static MediaType Parse​(java.lang.String mediaTypeValue)`<br>
 Parses a media type string and returns a media type object.
* `static MediaType Parse​(java.lang.String str,
MediaType defaultValue)`<br>
 Parses a media type string and returns a media type object, or the default
 value if the string is invalid.
* `java.lang.String ToSingleLineString()`<br>
 Converts this media type to a text string form suitable for inserting in
 HTTP headers.
* `java.lang.String toString()`<br>
 Converts this media type to a text string form suitable for inserting in
 email headers.
* `java.lang.String ToUriSafeString()`<br>
 Converts this media type to a text string form suitable for data URIs.

## Field Details

### <a id='TextPlainAscii'>TextPlainAscii</a>

Specifies the media type "text/plain" and the "charset" parameter
  "US-ASCII", used for plain text data that contains only characters
 within the basic Latin range (U+0000 to U+007F).
### <a id='TextPlainUtf8'>TextPlainUtf8</a>

Specifies the media type "text/plain" and the "charset" parameter "utf-8",
 used for plain text data that may contain characters outside the
 basic Latin range (U+0000 to U+007F).
### <a id='MessageRfc822'>MessageRfc822</a>

Specifies the media type "message/rfc822", used for Internet mail messages.
### <a id='ApplicationOctetStream'>ApplicationOctetStream</a>

Specifies the media type "application/octet-stream", used for arbitrary
 binary data.
## Method Details

### <a id='getTopLevelType()'>getTopLevelType</a>

Gets the name of this media type's top-level type (such as "text" in
  "text/plain", or "audio" in "audio/basic"). The resulting string
 will be in lower case; that is, with its basic upper-case letters
  ("A" to "Z") converted to basic lower-case letters ("a" to "z").

**Returns:**

* The name of this media type's top-level type (such as "text" or
  "audio" .

### <a id='HasStructuredSuffix(java.lang.String)'>HasStructuredSuffix</a>

Returns whether this media type's subtype has the given structured syntax
 suffix.

**Parameters:**

* <code>suffix</code> - A text string identifying a structured syntax suffix without
  the starting "+". Examples include "xml" and "json". The suffix is
 compared to the end of the media type's subtype using a basic
 case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U+0041 to U+005A) in both strings to basic
 lower-case letters.).

**Returns:**

* True if the media type's subtype ends with, but does not consist of,
  "+" followed by the <code>suffix</code> parameter (using a basic
 case-insensitive comparison); otherwise, <code>false</code>. For example,
  returns false if <code>suffix</code> is "xml" and the subtype is "+xml",
  but returns true if <code>suffix</code> is "xml" and the subtype is
  "example+xml". Returns false if <code>suffix</code> is null or an empty
 string.

### <a id='equals(java.lang.Object)'>equals</a>

Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code> in class <code>java.lang.Object</code>

**Parameters:**

* <code>obj</code> - The parameter <code>obj</code> is an arbitrary object.

**Returns:**

* <code>true</code> if this object and the other object are equal;
 otherwise, <code>false</code>.

### <a id='hashCode()'>hashCode</a>

Calculates the hash code of this object. The exact algorithm used by this
 method may change between versions of this library, and no
 application or process IDs are used in the hash code calculation.

**Overrides:**

* <code>hashCode</code> in class <code>java.lang.Object</code>

**Returns:**

* A 32-bit signed integer.

### <a id='getSubType()'>getSubType</a>

Gets this media type's subtype (for example, "plain" in "text/plain"). The
 resulting string will be in lower case; that is, with its basic
  upper-case letters ("A" to "Z") converted to basic lower-case
  letters ("a" to "z").

**Returns:**

* This media type's subtype.

### <a id='isText()'>isText</a>

Gets a value indicating whether this is a text media type ("text/*").

**Returns:**

* <code>true</code> If this is a text media type; otherwise, <code>false</code>.

### <a id='isMultipart()'>isMultipart</a>

Gets a value indicating whether this is a multipart media type.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, <code>
 false</code>.

### <a id='getParameters()'>getParameters</a>

Gets a list of the parameter names contained in this media type object and
 their values. Each parameter name will be in lower case; that is,
  with its basic upper-case letters ("A" to "Z") converted to basic
  lower-case letters ("a" to "z").

**Returns:**

* A list of the parameters contained in this media type object; the
 names of each parameter appear in an undefined order. NOTE: Previous
 versions erroneously stated that the list will be sorted by name. In
 fact, the names will not be guaranteed to appear in any particular
 order; this is at least the case in version 0.10.0.

### <a id='toString()'>toString</a>

Converts this media type to a text string form suitable for inserting in
 email headers. Notably, the string contains the value of a
 Content-Type header field (without the text necessarily starting
  with "Content-Type" followed by a space), and consists of one or
 more lines.

**Overrides:**

* <code>toString</code> in class <code>java.lang.Object</code>

**Returns:**

* A text string form of this media type.

### <a id='ToSingleLineString()'>ToSingleLineString</a>

Converts this media type to a text string form suitable for inserting in
 HTTP headers. Notably, the string contains the value of a
 Content-Type header field (without the text necessarily starting
  with "Content-Type" followed by a space), and consists of a single
 line.

**Returns:**

* A text string form of this media type.

### <a id='ToUriSafeString()'>ToUriSafeString</a>

Converts this media type to a text string form suitable for data URIs.
 Notably, the string contains the value of a Content-Type header
  field (without the text necessarily starting with "Content-Type"
 followed by a space), consists of a single line, and uses
 percent-encoding as necessary or convenient so that the resulting
 string can validly appear in a URI path.

**Returns:**

* A text string form of this media type.

### <a id='GetCharset()'>GetCharset</a>

Gets this media type's "charset" parameter, naming a character encoding used
 to represent text in the data that uses this media type.

**Returns:**

* If the "charset" parameter is present and non-empty, returns the
 result of the Encodings.ResolveAliasForEmail method for that
 parameter, except that the result's basic upper-case letters A to Z
  (U+0041 to U+005A) are converted to lower case. If the "charset"
  parameter is empty, returns the empty string. If the "charset"
 parameter is absent, returns the default value, if any, for that
  parameter given the media type (e.g., "us-ascii" if the media type
  is "text/plain"; see RFC2046), or the empty string if there is none.

### <a id='GetParameter(java.lang.String)'>GetParameter</a>

Gets the value of a parameter in this media type, such as "charset" or
  "format".

**Parameters:**

* <code>name</code> - Name of the parameter to get. The name is compared using a basic
 case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U+0041 to U+005A) in both strings to basic
 lower-case letters.).

**Returns:**

* The value of the parameter as a string, or null if the parameter
 doesn't exist.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>name</code> is null.

* <code>java.lang.IllegalArgumentException</code> - Name is empty.

### <a id='getTypeAndSubType()'>getTypeAndSubType</a>

Gets the top level type and subtype of this media type, separated by a
  slash; for example, "text/plain". The resulting string will be in
 lowercase letters.

**Returns:**

* The top level type and subtype of this media type, separated by a
  slash; for example, "text/plain".

### <a id='Parse(java.lang.String)'>Parse</a>

Parses a media type string and returns a media type object. For further
 information, see the overload taking a MediaType parameter.

**Parameters:**

* <code>mediaTypeValue</code> - A text string representing a media type. This media
 type can include parameters.

**Returns:**

* A media type object, or MediaType.TextPlainAscii if <code>
 mediaTypeValue</code> is empty or syntactically invalid.

### <a id='Parse(java.lang.String,com.upokecenter.mail.MediaType)'>Parse</a>

Parses a media type string and returns a media type object, or the default
 value if the string is invalid. This method checks the syntactic
 validity of the string, but not whether it has all parameters it's
 required to have or whether the parameters themselves are set to
 valid values for the parameter. <p>This method assumes the given
 media type string was directly extracted from the Content-Type
 header field (as defined for email messages) and follows the syntax
 given in RFC 2045. Accordingly, among other things, the media type
 string can contain comments (delimited by parentheses).</p> <p>RFC
 2231 extensions allow each media type parameter to be associated
 with a character encoding and/or language, and support parameter
 values that span two or more key-value pairs. Parameters making use
  of RFC 2231 extensions have names with an asterisk ("*"). Such a
 parameter will be ignored if it is ill-formed because of RFC 2231's
 rules (except for illegal percent-decoding or undecodable sequences
 for the given character encoding). Examples of RFC 2231 extensions
  follow (both examples encode the same "filename" parameter):</p>
 <p><b>text/example; filename*=utf-8'en'filename.txt</b></p>
 <p><b>text/example; filename*0*=utf-8'en'file;
 filename*1*=name%2Etxt</b></p> <p>This implementation ignores keys
 (in parameter key-value pairs) that appear more than once in the
 media type. Nothing in RFCs 2045, 2183, 2231, 6266, or 7231
 explicitly disallows such keys, or otherwise specifies
 error-handling behavior for such keys.</p>

**Parameters:**

* <code>str</code> - A text string representing a media type. This media type can
 include parameters.

* <code>defaultValue</code> - The media type to return if the string is syntactically
 invalid. Can be null.

**Returns:**

* A MediaType object.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.
