# com.upokecenter.mail.MediaTypeBuilder

    public final class MediaTypeBuilder extends java.lang.Object

A mutable media type object.

## Methods

* `MediaTypeBuilder() MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.
* `MediaTypeBuilder​(MediaType mt) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.
* `MediaTypeBuilder​(java.lang.String type,
                java.lang.String subtype) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.
* `java.lang.String getSubType()`<br>
 Gets this value's subtype.
* `java.lang.String getTopLevelType()`<br>
 Gets a value not documented yet.
* `boolean isMultipart()`<br>
 Gets a value not documented yet.
* `boolean isText()`<br>
 Gets a value indicating whether this is a text media type.
* `MediaTypeBuilder RemoveParameter​(java.lang.String name)`<br>
 Removes a parameter from this builder object.
* `MediaTypeBuilder SetParameter​(java.lang.String name,
            java.lang.String value)`<br>
 Sets a parameter's name and value for this media type.
* `void setSubType​(java.lang.String value)`<br>
* `MediaTypeBuilder SetSubType​(java.lang.String str)`<br>
 Sets this media type's subtype, such as "plain" or "xml" .
* `void setTopLevelType​(java.lang.String value)`<br>
* `MediaTypeBuilder SetTopLevelType​(java.lang.String str)`<br>
 Sets this media type's top-level type.
* `MediaType ToMediaType()`<br>
* `java.lang.String toString() MediaType.toString`<br>
 Converts this object to a text string of the media type it represents, in
 the same form as MediaType.toString.

## Constructors

* `MediaTypeBuilder() MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.
* `MediaTypeBuilder​(MediaType mt) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.
* `MediaTypeBuilder​(java.lang.String type,
                java.lang.String subtype) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.

## Method Details

### MediaTypeBuilder
    public MediaTypeBuilder()
Initializes a new instance of the <code>MediaTypeBuilder</code> class.
### MediaTypeBuilder
    public MediaTypeBuilder​(MediaType mt)
Initializes a new instance of the <code>MediaTypeBuilder</code> class.

**Parameters:**

* <code>mt</code> - The parameter <code>mt</code> is a MediaType object.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>mt</code> is null.

### MediaTypeBuilder
    public MediaTypeBuilder​(java.lang.String type, java.lang.String subtype)
Initializes a new instance of the <code>MediaTypeBuilder</code> class.

**Parameters:**

* <code>type</code> - The parameter <code>type</code> is a text string.

* <code>subtype</code> - The parameter <code>subtype</code> is a text string.

### getTopLevelType
    public final java.lang.String getTopLevelType()
Gets a value not documented yet.

**Returns:**

* A value not documented yet.

### setTopLevelType
    public final void setTopLevelType​(java.lang.String value)
### getSubType
    public final java.lang.String getSubType()
Gets this value's subtype.

**Returns:**

* This value's subtype.

### setSubType
    public final void setSubType​(java.lang.String value)
### isText
    public final boolean isText()
Gets a value indicating whether this is a text media type.

**Returns:**

* <code>true</code> If this is a text media type; otherwise, . <code>
 false</code>.

### isMultipart
    public final boolean isMultipart()
Gets a value not documented yet.

**Returns:**

* A value not documented yet.

### ToMediaType
    public MediaType ToMediaType()

**Returns:**

* A MediaType object.

### SetTopLevelType
    public MediaTypeBuilder SetTopLevelType​(java.lang.String str)
Sets this media type's top-level type.

**Parameters:**

* <code>str</code> - A text string naming a top-level type, such as "text" or "audio"
.

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>str</code> is syntactically invalid
 for a top-level type.

### RemoveParameter
    public MediaTypeBuilder RemoveParameter​(java.lang.String name)
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

* <code>java.lang.NullPointerException</code> - The parameter <code>name</code> is null.

### SetParameter
    public MediaTypeBuilder SetParameter​(java.lang.String name, java.lang.String value)
Sets a parameter's name and value for this media type.

**Parameters:**

* <code>name</code> - Name of the parameter to set, such as "charset" . The name is
 compared using a basic case-insensitive comparison. (Two strings are
 equal in such a comparison, if they match after converting the basic
 upper-case letters A to Z (U + 0041 to U + 005A) in both strings to
 lower case.).

* <code>value</code> - A text string giving the parameter's value.

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>value</code> or <code>name</code> is
 null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>name</code> is empty or
 syntactically invalid.

### SetSubType
    public MediaTypeBuilder SetSubType​(java.lang.String str)
Sets this media type's subtype, such as "plain" or "xml" .

**Parameters:**

* <code>str</code> - A text string naming a media subtype.

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>str</code> is empty or
 syntactically invalid.

### toString
    public java.lang.String toString()
Converts this object to a text string of the media type it represents, in
 the same form as <code>MediaType.toString</code>.

**Overrides:**

* <code>toString</code> in class <code>java.lang.Object</code>

**Returns:**

* A string representation of this object.
