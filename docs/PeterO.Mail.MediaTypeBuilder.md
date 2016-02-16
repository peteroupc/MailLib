## PeterO.Mail.MediaTypeBuilder

    public sealed class MediaTypeBuilder

A mutable media type object.

### MediaTypeBuilder Constructor

    public MediaTypeBuilder(
        PeterO.Mail.MediaType mt);

Initializes a new instance of the [PeterO.Mail.MediaTypeBuilder](PeterO.Mail.MediaTypeBuilder.md) class using the data from another media type.

<b>Parameters:</b>

 * <i>mt</i>: A MediaType object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>mt</i>
 is null.

### MediaTypeBuilder Constructor

    public MediaTypeBuilder(
        string type,
        string subtype);

Initializes a new instance of the [PeterO.Mail.MediaTypeBuilder](PeterO.Mail.MediaTypeBuilder.md) class.

<b>Parameters:</b>

 * <i>type</i>: The media type's top-level type.

 * <i>subtype</i>: The media type's subtype.

### MediaTypeBuilder Constructor

    public MediaTypeBuilder();

Initializes a new instance of the [PeterO.Mail.MediaTypeBuilder](PeterO.Mail.MediaTypeBuilder.md) class, using the type "application/octet-stream".

### IsMultipart

    public bool IsMultipart { get; }

Gets a value indicating whether this is a multipart media type.

<b>Returns:</b>

 `true`  If this is a multipart media type; otherwise, . `false` .

### IsText

    public bool IsText { get; }

Gets a value indicating whether this is a text media type.

<b>Returns:</b>

 `true`  If this is a text media type; otherwise, . `false` .

### SubType

    public string SubType { get; set;}

Gets or sets this value's subtype.

<b>Returns:</b>

This value's subtype.

### TopLevelType

    public string TopLevelType { get; set;}

Gets or sets this value's top-level type.

<b>Returns:</b>

This value's top-level type.

### RemoveParameter

    public PeterO.Mail.MediaTypeBuilder RemoveParameter(
        string name);

Removes a parameter from this builder object. Does nothing if the parameter's name doesn't exist.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to remove. The name is compared case-insensitively.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
 is null.

### SetParameter

    public PeterO.Mail.MediaTypeBuilder SetParameter(
        string name,
        string value);

Sets a parameter's name and value for this media type.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to set, such as "charset". The name is compared case-insensitively.

 * <i>value</i>: A text string giving the parameter's value.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>value</i>
 or  <i>name</i>
 is null.

 * System.ArgumentException:
The parameter <i>name</i>
 is empty or syntactically invalid.

### SetSubType

    public PeterO.Mail.MediaTypeBuilder SetSubType(
        string str);

Sets this media type's subtype, such as "plain" or "xml" .

<b>Parameters:</b>

 * <i>str</i>: A text string naming a media subtype.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.

 * System.ArgumentException:
The parameter <i>str</i>
 is empty or syntactically invalid.

### SetTopLevelType

    public PeterO.Mail.MediaTypeBuilder SetTopLevelType(
        string str);

Sets this media type's top-level type.

<b>Parameters:</b>

 * <i>str</i>: A text string naming a top-level type, such as "text" or "audio".

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.

 * System.ArgumentException:
The parameter <i>str</i>
 is syntactically invalid for a top-level type.

### ToMediaType

    public PeterO.Mail.MediaType ToMediaType();

Converts this builder to an immutable media type object.

<b>Return Value:</b>

A MediaType object.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Return Value:</b>

A string representation of this object.
