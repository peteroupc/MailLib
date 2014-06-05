## PeterO.Mail.MediaTypeBuilder

    public sealed class MediaTypeBuilder

A mutable media type object.

### MediaTypeBuilder Constructor

    public MediaTypeBuilder();

Initializes a new instance of the MediaTypeBuilder class.

### MediaTypeBuilder Constructor

    public MediaTypeBuilder(
        PeterO.Mail.MediaType mt);

Initializes a new instance of the MediaTypeBuilder class.

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

Initializes a new instance of the MediaTypeBuilder class.

<b>Parameters:</b>

 * <i>type</i>: A string object.

 * <i>subtype</i>: A string object. (2).

### ToMediaType

    public PeterO.Mail.MediaType ToMediaType();

Converts this builder to an immutable media type object.

<b>Returns:</b>

A MediaType object.

### SetTopLevelType

    public PeterO.Mail.MediaTypeBuilder SetTopLevelType(
        string str);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A string object.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.

### RemoveParameter

    public PeterO.Mail.MediaTypeBuilder RemoveParameter(
        string name);

Removes a parameter from this builder object.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to remove. The name is compared case-insensitively.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>name</i>
 is null.

### SetParameter

    public PeterO.Mail.MediaTypeBuilder SetParameter(
        string name,
        string value);

Not documented yet.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to set. The name is compared case-insensitively.

 * <i>value</i>: A string object. (2).

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>value</i>
 or  <i>name</i>
 is null.

### SetSubType

    public PeterO.Mail.MediaTypeBuilder SetSubType(
        string str);

Not documented yet.

<b>Parameters:</b>

 * <i>str</i>: A string object.

<b>Returns:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>str</i>
 is null.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string representation of this object.

### TopLevelType

    public string TopLevelType { get; set;}

Gets or sets this value's top-level type.

<b>Returns:</b>

This value's top-level type.

### SubType

    public string SubType { get; set;}

Gets or sets this value's subtype.

<b>Returns:</b>

This value's subtype.

### IsText

    public bool IsText { get; }

Gets a value indicating whether this is a text media type.

<b>Returns:</b>

True if this is a text media type; otherwise, false..

### IsMultipart

    public bool IsMultipart { get; }

Gets a value indicating whether this is a multipart media type.

<b>Returns:</b>

True if this is a multipart media type; otherwise, false..


