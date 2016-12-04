# com.upokecenter.mail.MediaType

    public final class MediaType extends Object

<p>Specifies what kind of data a message body is.</p> <p>A media type
 consists of a top-level type (the general category of the data), a
 subtype (the specific type), and an optional list of parameters. For
 example, the media type <code>text/plain; charset = utf-8</code> is a text
 media type ("text"), namely, a plain text type ("plain"), and the
 parameters say that the data uses UTF-8, a Unicode character encoding
 ("charset=utf-8"). Other top-level types include "audio", "video",
 and "application".</p> <p>A media type is sometimes known as a "MIME
 type", for Multipurpose Internet Mail Extensions, the standard that
 introduced media types.</p> <p>This type is immutable, meaning its
 values can't be changed once it' s created. To create a changeable
 media type object, use the MediaTypeBuilder class.</p>

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

* `boolean equals(Object obj)`<br>
 Determines whether this object and another object are equal.
* `String GetCharset()`<br>
 Gets this media type's "charset" parameter, naming a character encoding used
 to represent text in the data that uses this media type.
* `String GetParameter(String name)`<br>
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
 Returns the hash code for this instance.
* `boolean isMultipart()`<br>
 Gets a value indicating whether this is a multipart media type.
* `boolean isText()`<br>
 Gets a value indicating whether this is a text media type ("text/*").
* `static MediaType Parse(String mediaTypeValue)`<br>
 Parses a media type string and returns a media type object.
* `static MediaType Parse(String str,
     MediaType defaultValue)`<br>
 Parses a media type string and returns a media type object, or the default
 value if the string is invalid.
* `String toString()`<br>
 Converts this object to a text string.

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
    public boolean equals(Object obj)
Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code>&nbsp;in class&nbsp;<code>Object</code>

**Parameters:**

* <code>obj</code> - An arbitrary object.

**Returns:**

* <code>true</code> if this object and another object are equal; otherwise,
 <code>false</code>.

### hashCode
    public int hashCode()
Returns the hash code for this instance.

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

* <code>true</code> If this is a text media type; otherwise, <code>false</code>.

### isMultipart
    public final boolean isMultipart()
Gets a value indicating whether this is a multipart media type.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, <code>false</code>.

### getParameters
    public final Map<String,String> getParameters()
Gets a list of the parameters contained in this media type object.

**Returns:**

* A list of the parameters contained in this media type object. NOTE:
 Previous versions erroneously stated that the list will be sorted by
 name. In fact, the names will not be guaranteed to appear in any
 particular order; this is at least the case in version 0.10.0.

### toString
    public String toString()
Converts this object to a text string.

**Overrides:**

* <code>toString</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A string representation of this object.

### GetCharset
    public String GetCharset()
Gets this media type's "charset" parameter, naming a character encoding used
 to represent text in the data that uses this media type.

**Returns:**

* If the "charset" parameter exists, returns that parameter with the
 basic upper-case letters A to Z (U + 0041 to U + 005A) converted to lower
 case. Returns <code>"us-ascii"</code> instead if the media type is
 ill-formed (RFC2045 sec. 5.2), or if the media type is "text/plain"
 and doesn't have a "charset" parameter (see RFC2046), or the default
 value for that parameter, if any, for the media type if the "charset"
 parameter is absent. Returns an empty string in all other cases.

### GetParameter
    public String GetParameter(String name)
Gets the value of a parameter in this media type, such as "charset" or
 "format".

**Parameters:**

* <code>name</code> - Name of the parameter to get. The name is compared using a basic
 case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U + 0041 to U + 005A) in both strings to lower
 case.).

**Returns:**

* The value of the parameter as a string, or null if the parameter
 doesn't exist.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### getTypeAndSubType
    public final String getTypeAndSubType()
Gets the top level type and subtype of this media type, separated by a
 slash; for example, "text/plain".

**Returns:**

* The top level type and subtype of this media type, separated by a
 slash; for example, "text/plain".

### Parse
    public static MediaType Parse(String mediaTypeValue)
Parses a media type string and returns a media type object. This method
 checks the syntactic validity of the string, but not whether it has
 all parameters it's required to have or whether the parameters
 themselves are set to valid values for the parameter.

**Parameters:**

* <code>mediaTypeValue</code> - A text string representing a media type. This media
 type can include parameters.

**Returns:**

* A media type object, or text/plain if <code>mediaTypeValue</code> is
 empty or syntactically invalid.

### Parse
    public static MediaType Parse(String str, MediaType defaultValue)
Parses a media type string and returns a media type object, or the default
 value if the string is invalid. This method checks the syntactic
 validity of the string, but not whether it has all parameters it's
 required to have or whether the parameters themselves are set to
 valid values for the parameter.

**Parameters:**

* <code>str</code> - A text string representing a media type. This media type can
 include parameters.

* <code>defaultValue</code> - The media type to return if the string is syntactically
 invalid. Can be null.

**Returns:**

* A MediaType object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.
