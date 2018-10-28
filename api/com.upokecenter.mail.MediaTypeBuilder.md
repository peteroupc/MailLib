# com.upokecenter.mail.MediaTypeBuilder

    public final class MediaTypeBuilder extends Object

A mutable media type object.

## Methods

* `MediaTypeBuilder() MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class, using the type
 "application/octet-stream" .
* `MediaTypeBuilder​(MediaType mt) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class using the data from
 another media type.
* `MediaTypeBuilder​(String type,
                String subtype) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.
* `String getSubType()`<br>
 Gets this value's subtype.
* `String getTopLevelType()`<br>
 Gets this value's top-level type.
* `boolean isMultipart()`<br>
 Gets a value indicating whether this is a multipart media type.
* `boolean isText()`<br>
 Gets a value indicating whether this is a text media type.
* `MediaTypeBuilder RemoveParameter​(String name)`<br>
 Removes a parameter from this builder object.
* `MediaTypeBuilder SetParameter​(String name,
            String value)`<br>
 Sets a parameter's name and value for this media type.
* `void setSubType​(String value)`<br>
* `MediaTypeBuilder SetSubType​(String str)`<br>
 Sets this media type's subtype, such as "plain" or "xml" .
* `void setTopLevelType​(String value)`<br>
* `MediaTypeBuilder SetTopLevelType​(String str)`<br>
 Sets this media type's top-level type.
* `MediaType ToMediaType()`<br>
 Converts this builder to an immutable media type object.
* `String toString() MediaType.toString`<br>
 Converts this object to a text string of the media type it represents, in
 the same form as MediaType.toString

## Constructors

* `MediaTypeBuilder() MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class, using the type
 "application/octet-stream" .
* `MediaTypeBuilder​(MediaType mt) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class using the data from
 another media type.
* `MediaTypeBuilder​(String type,
                String subtype) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.

## Method Details

### MediaTypeBuilder
    public MediaTypeBuilder()
Initializes a new instance of the <code>MediaTypeBuilder</code> class, using the type
 "application/octet-stream" .
### MediaTypeBuilder
    public MediaTypeBuilder​(MediaType mt)
Initializes a new instance of the <code>MediaTypeBuilder</code> class using the data from
 another media type.

**Parameters:**

* <code>mt</code> - The parameter <code>mt</code> is a MediaType object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>mt</code> is null.

### MediaTypeBuilder
    public MediaTypeBuilder​(String type, String subtype)
Initializes a new instance of the <code>MediaTypeBuilder</code> class.

**Parameters:**

* <code>type</code> - The media type's top-level type.

* <code>subtype</code> - The media type's subtype.

### getTopLevelType
    public final String getTopLevelType()
Gets this value's top-level type.

**Returns:**

* This value's top-level type.

### setTopLevelType
    public final void setTopLevelType​(String value)
### getSubType
    public final String getSubType()
Gets this value's subtype.

**Returns:**

* This value's subtype.

### setSubType
    public final void setSubType​(String value)
### isText
    public final boolean isText()
Gets a value indicating whether this is a text media type.

**Returns:**

* <code>true</code> If this is a text media type; otherwise, . <code>
 false</code>.

### isMultipart
    public final boolean isMultipart()
Gets a value indicating whether this is a multipart media type.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, . <code>
 false</code>.

### ToMediaType
    public MediaType ToMediaType()
Converts this builder to an immutable media type object.

**Returns:**

* A MediaType object.

### SetTopLevelType
    public MediaTypeBuilder SetTopLevelType​(String str)
Sets this media type's top-level type.

**Parameters:**

* <code>str</code> - A text string naming a top-level type, such as "text" or "audio"
 .

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>str</code> is syntactically
 invalid for a top-level type.

### RemoveParameter
    public MediaTypeBuilder RemoveParameter​(String name)
Removes a parameter from this builder object. Does nothing if the
 parameter's name doesn't exist.

**Parameters:**

* <code>name</code> - Name of the parameter to remove. The name is compared using a
 basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetParameter
    public MediaTypeBuilder SetParameter​(String name, String value)
Sets a parameter's name and value for this media type.

**Parameters:**

* <code>name</code> - Name of the parameter to set, such as "charset" . The name is
 compared using a basic case-insensitive comparison. (Two strings are
 equal in such a comparison, if they match after converting the basic
 upper-case letters A to Z (U + 0041 to U + 005A) in both strings to lower
 case.).

* <code>value</code> - A text string giving the parameter's value.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>value</code> or <code>
 name</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>name</code> is empty or
 syntactically invalid.

### SetSubType
    public MediaTypeBuilder SetSubType​(String str)
Sets this media type's subtype, such as "plain" or "xml" .

**Parameters:**

* <code>str</code> - A text string naming a media subtype.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>str</code> is empty or
 syntactically invalid.

### toString
    public String toString()
Converts this object to a text string of the media type it represents, in
 the same form as <code>MediaType.toString</code>

**Overrides:**

* <code>toString</code> in class <code>Object</code>

**Returns:**

* A string representation of this object.
