# com.upokecenter.mail.MediaType.Builder

    public static final class MediaType.Builder extends Object

A mutable data type that allows a media type object to be built.

## Constructors

## Methods

* `final String getSubType()`<br>
 Gets this value's subtype.

* `final String getTopLevelType()`<br>
 Gets this value's top-level type.

* `MediaType.Builder RemoveParameter(String name)`<br>
 Removes a parameter from this builder object.

* `MediaType.Builder SetParameter(String name,
 String value)`<br>
 Sets a parameter's name and value for this media type.

* `final void setSubType(String value)`<br>
  
* `MediaType.Builder SetSubType(String str)`<br>
 Sets this media type's subtype, such as "plain" or "xml" .

* `final void setTopLevelType(String value)`<br>
  
* `MediaType.Builder SetTopLevelType(String str)`<br>
 Sets this media type's top-level type.

* `MediaType ToMediaType()`<br>
 Converts this builder to an immutable media type object.

* `String toString()`<br>
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

### ToMediaType

    public MediaType ToMediaType()

Converts this builder to an immutable media type object.

**Returns:**

* A MediaType object.

### SetTopLevelType

    public MediaType.Builder SetTopLevelType(String str)

Sets this media type's top-level type. This method enables the pattern of
 method chaining (for example, <code>new...().getSet()...().getSet()...()</code>) unlike
 with the TopLevelType property in.NET or the setTopLevelType method (with
 small s) in Java.

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

    public MediaType.Builder RemoveParameter(String name)

Removes a parameter from this builder object. Does nothing if the
 parameter's name doesn't exist.

**Parameters:**

* <code>name</code> - Name of the parameter to remove. The name is compared using a
 basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic uppercase letters A to
 Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetParameter

    public MediaType.Builder SetParameter(String name, String value)

Sets a parameter's name and value for this media type.

**Parameters:**

* <code>name</code> - Name of the parameter to set, such as "charset" . The name is
 compared using a basic case-insensitive comparison. (Two strings are equal
 in such a comparison, if they match after converting the basic uppercase
 letters A to Z (U+0041 to U+005A) in both strings to basic lowercase
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

    public MediaType.Builder SetSubType(String str)

Sets this media type's subtype, such as "plain" or "xml" . This method
 enables the pattern of method chaining (for example, <code>
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
