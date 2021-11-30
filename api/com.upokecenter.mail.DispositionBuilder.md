# com.upokecenter.mail.DispositionBuilder

## Constructors

* `DispositionBuilder() DispositionBuilder`<br>
 Deprecated. Initializes a new instance of the DispositionBuilder class using the disposition
  type "attachment" .
* `DispositionBuilder​(ContentDisposition mt) DispositionBuilder`<br>
 Deprecated. Initializes a new instance of the DispositionBuilder class using the data from
 the given content disposition.
* `DispositionBuilder​(java.lang.String type) DispositionBuilder`<br>
 Deprecated. Initializes a new instance of the DispositionBuilder class using the given
 disposition type.

## Methods

* `java.lang.String getDispositionType()`<br>
 Deprecated. Gets this value's disposition type, such as "inline" or "attachment".
* `boolean isMultipart()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.
 Irrelevant for content dispositions; will be removed in the future.
* `boolean isText()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.
 Irrelevant for content dispositions; will be removed in the future.
* `DispositionBuilder RemoveParameter​(java.lang.String name)`<br>
 Deprecated. Removes a parameter from this content disposition.
* `void setDispositionType​(java.lang.String value)`<br>
 Deprecated.
* `DispositionBuilder SetDispositionType​(java.lang.String str)`<br>
 Deprecated. Sets the disposition type, such as "inline".
* `DispositionBuilder SetParameter​(java.lang.String name,
java.lang.String value)`<br>
 Deprecated. Sets a parameter of this content disposition.
* `ContentDisposition ToDisposition()`<br>
 Deprecated. Converts this object to an immutable ContentDisposition object.
* `java.lang.String toString()`<br>
 Deprecated. Converts this object to a text string.

## Method Details

### <a id='getDispositionType()'>getDispositionType</a>

Gets this value's disposition type, such as "inline" or "attachment".

**Returns:**

* This value's disposition type, such as "inline" or "attachment" .

**Throws:**

* <code>java.lang.NullPointerException</code> - The property is being set and the value is
 null.

* <code>java.lang.IllegalArgumentException</code> - The property is being set and the value is an
 empty string.

### <a id='setDispositionType(java.lang.String)'>setDispositionType</a>

### <a id='isText()'>isText</a>

Gets a value indicating whether this is a text media type.

**Returns:**

* <code>true</code> If this is a text media type; otherwise, <code>false</code>.

### <a id='isMultipart()'>isMultipart</a>

Gets a value indicating whether this is a multipart media type.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, <code>
 false</code>.

### <a id='ToDisposition()'>ToDisposition</a>

Converts this object to an immutable ContentDisposition object.

**Returns:**

* A MediaType object.

### <a id='SetDispositionType(java.lang.String)'>SetDispositionType</a>

Sets the disposition type, such as "inline". This method enables the pattern
 of method chaining (e.g., <code>new ...().getSet()...().getSet()...()</code>)
 unlike with the DispositionType property in .NET or the
 setDispositionType method (with small s) in Java.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

* <code>java.lang.IllegalArgumentException</code> - Str is empty.

### <a id='RemoveParameter(java.lang.String)'>RemoveParameter</a>

Removes a parameter from this content disposition. Does nothing if the
 parameter's name doesn't exist.

**Parameters:**

* <code>name</code> - The parameter to remove. The name is compared using a basic
 case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U+0041 to U+005A) in both strings to basic
 lower-case letters.).

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>name</code> is null.

### <a id='SetParameter(java.lang.String,java.lang.String)'>SetParameter</a>

Sets a parameter of this content disposition.

**Parameters:**

* <code>name</code> - Name of the parameter to set. If this name already exists
 (compared using a basic case-insensitive comparison), it will be
 overwritten. (Two strings are equal in a basic case-insensitive
 comparison, if they match after converting the basic upper-case
 letters A to Z (U+0041 to U+005A) in both strings to basic
 lower-case letters.).

* <code>value</code> - Value of the parameter to set.

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - Either <code>value</code> or <code>name</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>name</code> is empty, or it isn't a
 well-formed parameter name.

### <a id='toString()'>toString</a>

Converts this object to a text string.

**Overrides:**

* <code>toString</code> in class <code>java.lang.Object</code>

**Returns:**

* A string representation of this object.
