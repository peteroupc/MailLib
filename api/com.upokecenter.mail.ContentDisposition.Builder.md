# com.upokecenter.mail.ContentDisposition.Builder

    public static final class ContentDisposition.Builder extends Object

A mutable data type that allows a content disposition to be built.

## Constructors

## Methods

* `final String getDispositionType()`<br>
 Gets this value's disposition type, such as "inline" or "attachment".

* `ContentDisposition.Builder RemoveParameter(String name)`<br>
 Removes a parameter from this content disposition.

* `final void setDispositionType(String value)`<br>
  
* `ContentDisposition.Builder SetDispositionType(String str)`<br>
 Sets the disposition type, such as "inline".

* `ContentDisposition.Builder SetParameter(String name,
 String value)`<br>
 Sets a parameter of this content disposition.

* `ContentDisposition ToDisposition()`<br>
 Converts this object to an immutable ContentDisposition object.

* `String toString()`<br>
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

### ToDisposition

    public ContentDisposition ToDisposition()

Converts this object to an immutable ContentDisposition object.

**Returns:**

* A MediaType object.

### SetDispositionType

    public ContentDisposition.Builder SetDispositionType(String str)

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

    public ContentDisposition.Builder RemoveParameter(String name)

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

    public ContentDisposition.Builder SetParameter(String name, String value)

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
