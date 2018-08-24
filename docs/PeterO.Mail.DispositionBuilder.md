## PeterO.Mail.DispositionBuilder

    public class DispositionBuilder

A mutable data type that allows a content disposition to be built.

### DispositionBuilder Constructor

    public DispositionBuilder(
        PeterO.Mail.ContentDisposition mt);

Initializes a new instance of the [PeterO.Mail.DispositionBuilder](PeterO.Mail.DispositionBuilder.md) class using the data from the given content disposition.

<b>Parameters:</b>

 * <i>mt</i>: The parameter  <i>mt</i>
 is a ContentDisposition object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>mt</i>
 is null.

### DispositionBuilder Constructor

    public DispositionBuilder(
        string type);

Initializes a new instance of the [PeterO.Mail.DispositionBuilder](PeterO.Mail.DispositionBuilder.md) class using the given disposition type.

<b>Parameters:</b>

 * <i>type</i>: The parameter  <i>type</i>
 is a text string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>type</i>
 is null.

### DispositionBuilder Constructor

    public DispositionBuilder();

Initializes a new instance of the [PeterO.Mail.DispositionBuilder](PeterO.Mail.DispositionBuilder.md) class using the disposition type "attachment".

### DispositionType

    public string DispositionType { get; set;}

Gets or sets this value's disposition type, such as "inline" or "attachment".

<b>Returns:</b>

This value's disposition type, such as "inline" or "attachment".

### IsMultipart

    public bool IsMultipart { get; }

<b>Deprecated.</b> Irrelevant for content dispositions; will be removed in the future.

Gets a value indicating whether this is a multipart media type.

<b>Returns:</b>

 `true`  If this is a multipart media type; otherwise, . `false` .

### IsText

    public bool IsText { get; }

<b>Deprecated.</b> Irrelevant for content dispositions; will be removed in the future.

Gets a value indicating whether this is a text media type.

<b>Returns:</b>

 `true`  If this is a text media type; otherwise, . `false` .

### RemoveParameter

    public PeterO.Mail.DispositionBuilder RemoveParameter(
        string name);

Removes a parameter from this content disposition. Does nothing if the parameter's name doesn't exist.

<b>Parameters:</b>

 * <i>name</i>: The parameter to remove. The name is compared using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to lower case.).

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

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.

 * System.ArgumentException:
Str is empty.

### SetParameter

    public PeterO.Mail.DispositionBuilder SetParameter(
        string name,
        string value);

Sets a parameter of this content disposition.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to set. If this name already exists (compared using a basic case-insensitive comparison), it will be overwritten. (Two strings are equal in a basic case-insensitive comparison, if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to lower case.).

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
