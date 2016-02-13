## PeterO.Mail.DispositionBuilder

    public class DispositionBuilder

Description of DispositionBuilder.

### DispositionBuilder Constructor

    public DispositionBuilder(
        PeterO.Mail.ContentDisposition mt);

Initializes a new instance of the  class.

<b>Parameters:</b>

 * <i>mt</i>: A ContentDisposition object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>mt</i>
 is null.

### DispositionBuilder Constructor

    public DispositionBuilder(
        string type);

Initializes a new instance of the  class.

<b>Parameters:</b>

 * <i>type</i>: A text string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>type</i>
 is null.

### DispositionBuilder Constructor

    public DispositionBuilder();

Initializes a new instance of the  class.

### DispositionType

    public string DispositionType { get; set;}

Gets or sets this value's disposition type, such value, such as "inline" or "attachment".

<b>Returns:</b>

This value's disposition type, such value, such as "inline" or "attachment".

### IsMultipart

    public bool IsMultipart { get; }

<b>Deprecated.</b> Irrelevant for content dispositions; will be removed in the future.

Gets a value indicating whether this is a multipart media type.

<b>Returns:</b>

 `true`  if this is a multipart media type; otherwise,  `false` .

### IsText

    public bool IsText { get; }

<b>Deprecated.</b> Irrelevant for content dispositions; will be removed in the future.

Gets a value indicating whether this is a text media type.

<b>Returns:</b>

 `true`  if this is a text media type; otherwise,  `false` .

### RemoveParameter

    public PeterO.Mail.DispositionBuilder RemoveParameter(
        string name);

Removes a parameter from this content disposition. Does nothing if the parameter's name doesn't exist.

<b>Parameters:</b>

 * <i>name</i>: The parameter to remove. The name is compared case insensitively.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
 is null.

### SetDispositionType

    public PeterO.Mail.DispositionBuilder SetDispositionType(
        string str);

Sets the disposition type, such as "inline".

<b>Parameters:</b>

 * <i>str</i>: A text string.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.

### SetParameter

    public PeterO.Mail.DispositionBuilder SetParameter(
        string name,
        string value);

Sets a parameter of this content disposition.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to set. If this name already exists (compared case-insensitively), it will be overwritten.

 * <i>value</i>: Value of the parameter to set.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
Either  <i>value</i>
 or  <i>name</i>
 is null.

 * System.ArgumentException:
The parameter <i>name</i>
 is empty, or it isn't a well-formed parameter name.

### ToDisposition

    public PeterO.Mail.ContentDisposition ToDisposition();

Converts this object to an immutable ContentDisposition object.

<b>Return Value:</b>

A MediaType object.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Return Value:</b>

A string representation of this object.
