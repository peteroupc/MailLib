# com.upokecenter.mail.DispositionBuilder

    public class DispositionBuilder extends Object

Description of DispositionBuilder.

## Methods

* `DispositionBuilder() DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder(ContentDisposition mt) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder(String type) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `String getDispositionType()`<br>
 Gets this value's disposition type, such value, such as "inline" or
 "attachment".
* `boolean isMultipart()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.
 Irrelevant for content dispositions; will be removed in the future.
* `boolean isText()`<br>
 Deprecated.
Irrelevant for content dispositions; will be removed in the future.
 Irrelevant for content dispositions; will be removed in the future.
* `DispositionBuilder RemoveParameter(String name)`<br>
 Removes a parameter from this content disposition.
* `void setDispositionType(String value)`<br>
* `DispositionBuilder SetDispositionType(String str)`<br>
 Sets the disposition type, such as "inline".
* `DispositionBuilder SetParameter(String name,
            String value)`<br>
 Sets a parameter of this content disposition.
* `ContentDisposition ToDisposition()`<br>
 Converts this object to an immutable ContentDisposition object.
* `String toString()`<br>
 Converts this object to a text string.

## Constructors

* `DispositionBuilder() DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder(ContentDisposition mt) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.
* `DispositionBuilder(String type) DispositionBuilder`<br>
 Initializes a new instance of the DispositionBuilder class.

## Method Details

### DispositionBuilder
    public DispositionBuilder()
Initializes a new instance of the <code>DispositionBuilder</code> class.
### DispositionBuilder
    public DispositionBuilder(ContentDisposition mt)
Initializes a new instance of the <code>DispositionBuilder</code> class.

**Parameters:**

* <code>mt</code> - A ContentDisposition object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>mt</code> is null.

### DispositionBuilder
    public DispositionBuilder(String type)
Initializes a new instance of the <code>DispositionBuilder</code> class.

**Parameters:**

* <code>type</code> - A text string.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>type</code> is null.

### getDispositionType
    public final String getDispositionType()
Gets this value's disposition type, such value, such as "inline" or
 "attachment".

**Returns:**

* This value's disposition type, such value, such as "inline" or
 "attachment".

### setDispositionType
    public final void setDispositionType(String value)
### isText
    @Deprecated public final boolean isText()
Deprecated.&nbsp;Irrelevant for content dispositions; will be removed in the future.

**Returns:**

* <code>true</code> If this is a text media type; otherwise, <code>false</code>.

### isMultipart
    @Deprecated public final boolean isMultipart()
Deprecated.&nbsp;Irrelevant for content dispositions; will be removed in the future.

**Returns:**

* <code>true</code> If this is a multipart media type; otherwise, <code>false</code>.

### ToDisposition
    public ContentDisposition ToDisposition()
Converts this object to an immutable ContentDisposition object.

**Returns:**

* A MediaType object.

### SetDispositionType
    public DispositionBuilder SetDispositionType(String str)
Sets the disposition type, such as "inline".

**Parameters:**

* <code>str</code> - A text string.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>str</code> is null.

### RemoveParameter
    public DispositionBuilder RemoveParameter(String name)
Removes a parameter from this content disposition. Does nothing if the
 parameter's name doesn't exist.

**Parameters:**

* <code>name</code> - The parameter to remove. The name is compared case
 insensitively.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

### SetParameter
    public DispositionBuilder SetParameter(String name, String value)
Sets a parameter of this content disposition.

**Parameters:**

* <code>name</code> - Name of the parameter to set. If this name already exists
 (compared case-insensitively), it will be overwritten.

* <code>value</code> - Value of the parameter to set.

**Returns:**

* This instance.

**Throws:**

* <code>NullPointerException</code> - Either <code>value</code> or <code>name</code> is
 null.

* <code>IllegalArgumentException</code> - The parameter <code>name</code> is empty, or it
 isn't a well-formed parameter name.

### toString
    public String toString()
Converts this object to a text string.

**Overrides:**

* <code>toString</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A string representation of this object.
