# com.upokecenter.mail.DispositionBuilder

    public class DispositionBuilder extends java.lang.Object

A mutable data type that allows a content disposition to be built.

## Methods

* `DispositionBuilder() DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder​(ContentDisposition mt) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder​(java.lang.String type) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `java.lang.String getDispositionType()`<br>
 Gets this value's disposition type, such as "inline" or "attachment".
* `boolean isMultipart()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.
 Irrelevant for content dispositions; will be removed in the future.
* `boolean isText()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.
 Irrelevant for content dispositions; will be removed in the future.
* `DispositionBuilder RemoveParameter​(java.lang.String name)`<br>
 Removes a parameter from this content disposition.
* `void setDispositionType​(java.lang.String value)`<br>
* `DispositionBuilder SetDispositionType​(java.lang.String str)`<br>
 Sets the disposition type, such as "inline".
* `DispositionBuilder SetParameter​(java.lang.String name,
            java.lang.String value)`<br>
 Sets a parameter of this content disposition.
* `ContentDisposition ToDisposition()`<br>
 Converts this object to an immutable ContentDisposition object.
* `java.lang.String toString()`<br>
 Converts this object to a text string.

## Constructors

* `DispositionBuilder() DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder​(ContentDisposition mt) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder​(java.lang.String type) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.

## Method Details

### DispositionBuilder
    public DispositionBuilder()
Initializes a new instance of the <code>DispositionBuilder</code> class.
### DispositionBuilder
    public DispositionBuilder​(ContentDisposition mt)
Initializes a new instance of the <code>DispositionBuilder</code> class.

**Parameters:**

* <code>mt</code> - The parameter <code>mt</code> is a ContentDisposition object.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>mt</code> is null.

### DispositionBuilder
    public DispositionBuilder​(java.lang.String type)
Initializes a new instance of the <code>DispositionBuilder</code> class.

**Parameters:**

* <code>type</code> - The parameter <code>type</code> is a text string.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>type</code> is null.

### getDispositionType
    public final java.lang.String getDispositionType()
Gets this value's disposition type, such as "inline" or "attachment".

**Returns:**

* This value's disposition type, such as "inline" or "attachment" .

### setDispositionType
    public final void setDispositionType​(java.lang.String value)
### isText
    @Deprecated public final boolean isText()
Deprecated.
Irrelevant for content dispositions; will be removed in the future.

**Returns:**

* <code>true</code> If this is a text media type; otherwise, . <code>
 false</code> .

### isMultipart
    @Deprecated public final boolean isMultipart()
Deprecated.
Irrelevant for content dispositions; will be removed in the future.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, . <code>
 false</code> .

### ToDisposition
    public ContentDisposition ToDisposition()
Converts this object to an immutable ContentDisposition object.

**Returns:**

* A MediaType object.

### SetDispositionType
    public DispositionBuilder SetDispositionType​(java.lang.String str)
Sets the disposition type, such as "inline".

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is a text string.

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>str</code> is null.

* <code>java.lang.IllegalArgumentException</code> - Str is empty.

### RemoveParameter
    public DispositionBuilder RemoveParameter​(java.lang.String name)
Removes a parameter from this content disposition. Does nothing if the
 parameter's name doesn't exist.

**Parameters:**

* <code>name</code> - The parameter to remove. The name is compared using a basic
 case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case
 letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>name</code> is null.

### SetParameter
    public DispositionBuilder SetParameter​(java.lang.String name, java.lang.String value)
Sets a parameter of this content disposition.

**Parameters:**

* <code>name</code> - Name of the parameter to set. If this name already exists
 (compared using a basic case-insensitive comparison), it will be
 overwritten. (Two strings are equal in a basic case-insensitive
 comparison, if they match after converting the basic upper-case
 letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).

* <code>value</code> - Value of the parameter to set.

**Returns:**

* This instance.

**Throws:**

* <code>java.lang.NullPointerException</code> - Either <code>value</code> or <code>name</code> is
 null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>name</code> is empty, or it
 isn't a well-formed parameter name.

### toString
    public java.lang.String toString()
Converts this object to a text string.

**Overrides:**

* <code>toString</code> in class <code>java.lang.Object</code>

**Returns:**

* A string representation of this object.
