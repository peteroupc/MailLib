# com.upokecenter.mail.DispositionBuilder

    @Deprecated public class DispositionBuilder extends Object

A mutable data type that allows a content disposition to be built.

## Constructors

## Methods

* `final String getDispositionType()`<br>
 Deprecated.
Gets this value's disposition type, such as "inline" or "attachment".

* `final boolean isMultipart()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.

* `final boolean isText()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.

* `DispositionBuilder RemoveParameter(String name)`<br>
 Deprecated.
Removes a parameter from this content disposition.

* `final void setDispositionType(String value)`<br>
 Deprecated.
 
* `DispositionBuilder SetDispositionType(String str)`<br>
 Deprecated.
Sets the disposition type, such as "inline".

* `DispositionBuilder SetParameter(String name,
 String value)`<br>
 Deprecated.
Sets a parameter of this content disposition.

* `ContentDisposition ToDisposition()`<br>
 Deprecated.
Converts this object to an immutable ContentDisposition object.

* `String toString()`<br>
 Deprecated.
Converts this object to a text string.

## Method Details

### getDispositionType

    public final String getDispositionType()

Gets this value's disposition type, such as "inline" or "attachment".

**Returns:**

* This value's disposition type, such as "inline" or "attachment" .

**Throws:**

* <code>NullPointerException</code> - The property is being set and the value is
 null.

* <code>IllegalArgumentException</code> - The property is being set and the value is an
 empty string.

### setDispositionType

    public final void setDispositionType(String value)

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

### ToDisposition

    public ContentDisposition ToDisposition()

Converts this object to an immutable ContentDisposition object.

**Returns:**

* A MediaType object.

### SetDispositionType

    public DispositionBuilder SetDispositionType(String str)

Sets the disposition type, such as "inline". This method enables the pattern
 of method chaining (for example, <code>new ...().getSet()...().getSet()...()</code>)
 unlike with the DispositionType property in .NET or the setDispositionType
 method (with small s) in Java.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

* <code>IllegalArgumentException</code> - Str is empty.

### RemoveParameter

    public DispositionBuilder RemoveParameter(String name)

Removes a parameter from this content disposition. Does nothing if the
 parameter's name doesn't exist.

**Parameters:**

* <code>name</code> - The parameter to remove. The name is compared using a basic
 case-insensitive comparison. (Two strings are equal in such a comparison, if
 they match after converting the basic uppercase letters A to Z (U+0041 to
 U+005A) in both strings to basic lowercase letters.).

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetParameter

    public DispositionBuilder SetParameter(String name, String value)

Sets a parameter of this content disposition.

**Parameters:**

* <code>name</code> - Name of the parameter to set. If this name already exists
 (compared using a basic case-insensitive comparison), it will be
 overwritten. (Two strings are equal in a basic case-insensitive comparison,
 if they match after converting the basic uppercase letters A to Z (U+0041 to
 U+005A) in both strings to basic lowercase letters.).

* <code>value</code> - Value of the parameter to set.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - Either <code>value</code> or <code>name</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>name</code> is empty, or it isn't a
 well-formed parameter name.

### toString

    public String toString()

Converts this object to a text string.

**Overrides:**

* <code>toString</code> in class <code>Object</code>

**Returns:**

* A string representation of this object.
