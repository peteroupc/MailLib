## PeterO.Mail.ContentDisposition.Builder

    public sealed class Builder

A mutable data type that allows a content disposition to be built.

### Member Summary
* <code>[DispositionType](#DispositionType)</code> - Gets or sets this value's disposition type, such as "inline" or "attachment".
* <code>[RemoveParameter(string)](#RemoveParameter_string)</code> - Removes a parameter from this content disposition.
* <code>[SetDispositionType(string)](#SetDispositionType_string)</code> - Sets the disposition type, such as "inline".
* <code>[SetParameter(string, string)](#SetParameter_string_string)</code> - Sets a parameter of this content disposition.
* <code>[ToDisposition()](#ToDisposition)</code> - Converts this object to an immutable ContentDisposition object.
* <code>[ToString()](#ToString)</code> - Converts this object to a text string.

<a id="Void_ctor_PeterO_Mail_ContentDisposition"></a>
### Builder Constructor

    public Builder(
        PeterO.Mail.ContentDisposition mt);

Initializes a new instance of the [PeterO.Mail.ContentDisposition.Builder](PeterO.Mail.ContentDisposition.Builder.md) class using the data from the specified content disposition.

<b>Parameters:</b>

 * <i>mt</i>: The parameter  <i>mt</i>
 is a ContentDisposition object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>mt</i>
 is null.

<a id="Void_ctor_System_String"></a>
### Builder Constructor

    public Builder(
        string type);

Initializes a new instance of the [PeterO.Mail.ContentDisposition.Builder](PeterO.Mail.ContentDisposition.Builder.md) class using the specified disposition type.

<b>Parameters:</b>

 * <i>type</i>: The parameter  <i>type</i>
 is a text string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>type</i>
 is null.

 * System.ArgumentException:
Type is empty.

<a id="Void_ctor"></a>
### Builder Constructor

    public Builder();

Initializes a new instance of the [PeterO.Mail.ContentDisposition.Builder](PeterO.Mail.ContentDisposition.Builder.md) class using the disposition type "attachment" .

<a id="DispositionType"></a>
### DispositionType

    public string DispositionType { get; set; }

Gets or sets this value's disposition type, such as "inline" or "attachment".

<b>Returns:</b>

This value's disposition type, such as "inline" or "attachment" .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The property is being set and the value is null.

 * System.ArgumentException:
The property is being set and the value is an empty string.

<a id="RemoveParameter_string"></a>
### RemoveParameter

    public PeterO.Mail.ContentDisposition.Builder RemoveParameter(
        string name);

Removes a parameter from this content disposition. Does nothing if the parameter's name doesn't exist.

<b>Parameters:</b>

 * <i>name</i>: The parameter to remove. The name is compared using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic uppercase letters A to Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

<a id="SetDispositionType_string"></a>
### SetDispositionType

    public PeterO.Mail.ContentDisposition.Builder SetDispositionType(
        string str);

Sets the disposition type, such as "inline". This method enables the pattern of method chaining (for example,  `new
            ...().Set...().Set...()`  ) unlike with the DispositionType property in .NET or the setDispositionType method (with small s) in Java.

<b>Parameters:</b>

 * <i>str</i>: The parameter  <i>str</i>
 is a text string.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
Str is empty.

<a id="SetParameter_string_string"></a>
### SetParameter

    public PeterO.Mail.ContentDisposition.Builder SetParameter(
        string name,
        string value);

Sets a parameter of this content disposition.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to set. If this name already exists (compared using a basic case-insensitive comparison), it will be overwritten. (Two strings are equal in a basic case-insensitive comparison, if they match after converting the basic uppercase letters A to Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

 * <i>value</i>: Value of the parameter to set.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
Either  <i>value</i>
 or  <i>name</i>
 is null.

 * System.ArgumentException:
The parameter  <i>name</i>
 is empty, or it isn't a well-formed parameter name.

<a id="ToDisposition"></a>
### ToDisposition

    public PeterO.Mail.ContentDisposition ToDisposition();

Converts this object to an immutable ContentDisposition object.

<b>Return Value:</b>

A MediaType object.

<a id="ToString"></a>
### ToString

    public override string ToString();

Converts this object to a text string.

<b>Return Value:</b>

A string representation of this object.
