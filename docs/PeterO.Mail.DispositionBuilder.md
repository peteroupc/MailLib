## PeterO.Mail.DispositionBuilder

    public class DispositionBuilder

A mutable data type that allows a content disposition to be built.


### Member Summary
* <code>[DispositionType](#DispositionType)</code> - Gets or sets this value's disposition type, such as "inline" or "attachment".
* <code>[IsMultipart](#IsMultipart)</code> - Gets a value indicating whether this is a multipart media type.
* <code>[IsText](#IsText)</code> - Gets a value indicating whether this is a text media type.
* <code>[RemoveParameter(string)](#RemoveParameter_string)</code> - Removes a parameter from this content disposition.
* <code>[SetDispositionType(string)](#SetDispositionType_string)</code> - Sets the disposition type, such as "inline".
* <code>[SetParameter(string, string)](#SetParameter_string_string)</code> - Sets a parameter of this content disposition.
* <code>[ToDisposition()](#ToDisposition)</code> - Converts this object to an immutable ContentDisposition object.
* <code>[ToString()](#ToString)</code> - Converts this object to a text string.

<a id="Void_ctor_ContentDisposition"></a>

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
