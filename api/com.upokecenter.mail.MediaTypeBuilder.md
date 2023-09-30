# com.upokecenter.mail.MediaTypeBuilder

    @Deprecated public final class MediaTypeBuilder extends Object

A mutable media type object.

## Constructors

## Methods

* `final String getSubType()`<br>
 Deprecated.
Gets this value's subtype.

* `final String getTopLevelType()`<br>
 Deprecated.
Gets this value's top-level type.

* `final boolean isMultipart()`<br>
 Deprecated.
Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'multipart'.

* `final boolean isText()`<br>
 Deprecated.
Instead of using this property, use the TopLevelType property and compare
 the result with the exact String 'text'.

* `MediaTypeBuilder RemoveParameter(String name)`<br>
 Deprecated.
Removes a parameter from this builder object.

* `MediaTypeBuilder SetParameter(String name,
 String value)`<br>
 Deprecated.
Sets a parameter's name and value for this media type.

* `final void setSubType(String value)`<br>
 Deprecated.
 
* `MediaTypeBuilder SetSubType(String str)`<br>
 Deprecated.
Sets this media type's subtype, such as "plain" or "xml" .

* `final void setTopLevelType(String value)`<br>
 Deprecated.
 
* `MediaTypeBuilder SetTopLevelType(String str)`<br>
 Deprecated.
Sets this media type's top-level type.

* `MediaType ToMediaType()`<br>
 Deprecated.
Converts this builder to an immutable media type object.

* `String toString()`<br>
 Deprecated.
Converts this object to a text string of the media type it represents, in
 the same form as MediaType.toString.

## Method Details

### getTopLevelType
    public final String getTopLevelType()
Gets this value's top-level type.

**Returns:**

* A text string naming this object's top-level type, such as "text" or
 "audio" .

**Throws:**

* <code>NullPointerException</code> - The property is being set and the value is
 null.

* <code>IllegalArgumentException</code> - The property is being set and the value is
 syntactically invalid for a top-level type.

### setTopLevelType
    public final void setTopLevelType(String value)
### getSubType
    public final String getSubType()
Gets this value's subtype.

**Returns:**

* A text string naming this object's subtype, such as "plain" or
 "xml".

**Throws:**

* <code>NullPointerException</code> - The property is being set and the value is
 null.

* <code>IllegalArgumentException</code> - The property is being set and the value is
 syntactically invalid for a subtype.

### setSubType
    public final void setSubType(String value)
### isText
    @Deprecated public final boolean isText()
Gets a value indicating whether this is a text media type.

**Returns:**

* <code>true</code> If this is a text media type; otherwise, <code>false</code>.

### isMultipart
    @Deprecated public final boolean isMultipart()
Gets a value indicating whether this is a multipart media type.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, <code>
 false</code>.

### ToMediaType
    public MediaType ToMediaType()
Converts this builder to an immutable media type object.

**Returns:**

* A MediaType object.

### SetTopLevelType
    public MediaTypeBuilder SetTopLevelType(String str)
Sets this media type's top-level type. This method enables the pattern of
 method chaining (e.g., <code>new...().getSet()...().getSet()...()</code>) unlike with the
 TopLevelType property in.NET or the setTopLevelType method (with small s) in
 Java.

**Parameters:**

* <code>str</code> - A text string naming a top-level type, such as "text" or "audio"
 .

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>str</code> is syntactically invalid
 for a top-level type.

### RemoveParameter
    public MediaTypeBuilder RemoveParameter(String name)
Removes a parameter from this builder object. Does nothing if the
 parameter's name doesn't exist.

**Parameters:**

* <code>name</code> - Name of the parameter to remove. The name is compared using a
 basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case letters A to
 Z (U+0041 to U+005A) in both strings to basic lower-case letters.).

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetParameter
    public MediaTypeBuilder SetParameter(String name, String value)
Sets a parameter's name and value for this media type.

**Parameters:**

* <code>name</code> - Name of the parameter to set, such as "charset" . The name is
 compared using a basic case-insensitive comparison. (Two strings are equal
 in such a comparison, if they match after converting the basic upper-case
 letters A to Z (U+0041 to U+005A) in both strings to basic lower-case
 letters.).

* <code>value</code> - A text string giving the parameter's value.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>value</code> or <code>name</code> is
 null.

* <code>IllegalArgumentException</code> - The parameter <code>name</code> is empty or
 syntactically invalid.

### SetSubType
    public MediaTypeBuilder SetSubType(String str)
Sets this media type's subtype, such as "plain" or "xml" . This method
 enables the pattern of method chaining (e.g., <code>
 new...().getSet()...().getSet()...()</code>) unlike with the SubType property in.NET or the
 setSubType method (with small s) in Java.

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
 the same form as <code>MediaType.toString</code>.

**Overrides:**

* <code>toString</code> in class <code>Object</code>

**Returns:**

* A string representation of this object.
