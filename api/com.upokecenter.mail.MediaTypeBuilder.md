# com.upokecenter.mail.MediaTypeBuilder

    public final class MediaTypeBuilder extends java.lang.Object

A mutable media type object.

## Methods

* `MediaTypeBuilder() MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class, using the type
  "application/octet-stream" .
* `MediaTypeBuilder​(MediaType mt) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class using the data from
 another media type.
* `MediaTypeBuilder​(java.lang.String type,
                java.lang.String subtype) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class.
* `java.lang.String getSubType()`<br>
 Gets this value's subtype.
* `java.lang.String getTopLevelType()`<br>
 Gets this value's top-level type.
* `boolean isMultipart()`<br>
 Deprecated.
Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'multipart'.
 Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'multipart'.
* `boolean isText()`<br>
 Deprecated.
Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'text'.
 Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'text'.
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
 Converts this builder to an immutable media type object.
* `java.lang.String toString() MediaType.toString`<br>
 Converts this object to a text string of the media type it represents, in
 the same form as MediaType.toString.

## Constructors

* `MediaTypeBuilder() MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class, using the type
  "application/octet-stream" .
* `MediaTypeBuilder​(MediaType mt) MediaTypeBuilder`<br>
 Initializes a new instance of the MediaTypeBuilder class using the data from
 another media type.
* `MediaTypeBuilder​(java.lang.String type,
                java.lang.String subtype) MediaTypeBuilder`<br>
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

* <code>java.lang.NullPointerException</code> - The parameter <code>mt</code> is null.

### MediaTypeBuilder
    public MediaTypeBuilder​(java.lang.String type, java.lang.String subtype)
Initializes a new instance of the <code>MediaTypeBuilder</code> class.

**Parameters:**

* <code>type</code> - The media type's top-level type.

* <code>subtype</code> - The media type's subtype.

### getTopLevelType
    public final java.lang.String getTopLevelType()
Gets this value's top-level type.

**Returns:**

* A text string naming this object's top-level type, such as "text" or
  "audio" .

**Throws:**

* <code>java.lang.NullPointerException</code> - The property is being set and the value is
 null.

* <code>java.lang.IllegalArgumentException</code> - The property is being set and the value is
 syntactically invalid for a top-level type.

### setTopLevelType
    public final void setTopLevelType​(java.lang.String value)
### getSubType
    public final java.lang.String getSubType()
Gets this value's subtype.

**Returns:**

* A text string naming this object's subtype, such as "plain" or
  "xml".

**Throws:**

* <code>java.lang.NullPointerException</code> - The property is being set and the value is
 null.

* <code>java.lang.IllegalArgumentException</code> - The property is being set and the value is
 syntactically invalid for a subtype.

### setSubType
    public final void setSubType​(java.lang.String value)
### isText
    @Deprecated public final boolean isText()
Deprecated.
Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'text'.

**Returns:**

* <code>true</code> If this is a text media type; otherwise, <code>false</code>.

### isMultipart
    @Deprecated public final boolean isMultipart()
Deprecated.
Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'multipart'.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, <code>
 false</code>.

### ToMediaType
    public MediaType ToMediaType()
Converts this builder to an immutable media type object.

**Returns:**

* A MediaType object.

### SetTopLevelType
    public MediaTypeBuilder SetTopLevelType​(java.lang.String str)
Sets this media type's top-level type. This method enables the pattern of
 method chaining (e.g., <code>new...().getSet()...().getSet()...()</code>) unlike
 with the TopLevelType property in.NET or the setTopLevelType method
 (with small s) in Java.

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
 letters A to Z (U+0041 to U+005A) in both strings to basic
 lower-case letters.).

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
 upper-case letters A to Z (U+0041 to U+005A) in both strings to
 basic lower-case letters.).

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
Sets this media type's subtype, such as "plain" or "xml" . This method
 enables the pattern of method chaining (e.g.,
 <code>new...().getSet()...().getSet()...()</code>) unlike with the SubType property
 in.NET or the setSubType method (with small s) in Java.

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
