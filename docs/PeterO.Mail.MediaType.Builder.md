## PeterO.Mail.MediaType.Builder

    public sealed class Builder

A mutable data type that allows a media type object to be built.

### Member Summary
* <code>[RemoveParameter(string)](#RemoveParameter_string)</code> - Removes a parameter from this builder object.
* <code>[SetParameter(string, string)](#SetParameter_string_string)</code> - Sets a parameter's name and value for this media type.
* <code>[SetSubType(string)](#SetSubType_string)</code> - Sets this media type's subtype, such as "plain" or "xml" .
* <code>[SetTopLevelType(string)](#SetTopLevelType_string)</code> - Sets this media type's top-level type.
* <code>[SubType](#SubType)</code> - Gets or sets this value's subtype.
* <code>[ToMediaType()](#ToMediaType)</code> - Converts this builder to an immutable media type object.
* <code>[TopLevelType](#TopLevelType)</code> - Gets or sets this value's top-level type.
* <code>[ToString()](#ToString)</code> - Converts this object to a text string of the media type it represents, in the same form as MediaType.

<a id="Void_ctor_PeterO_Mail_MediaType"></a>
### Builder Constructor

    public Builder(
        PeterO.Mail.MediaType mt);

Initializes a new instance of the [PeterO.Mail.MediaType.Builder](PeterO.Mail.MediaType.Builder.md) class using the data from another media type.

<b>Parameters:</b>

 * <i>mt</i>: The parameter  <i>mt</i>
 is a MediaType object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>mt</i>
 is null.

<a id="Void_ctor_System_String_System_String"></a>
### Builder Constructor

    public Builder(
        string type,
        string subtype);

Initializes a new instance of the [PeterO.Mail.MediaType.Builder](PeterO.Mail.MediaType.Builder.md) class.

<b>Parameters:</b>

 * <i>type</i>: The media type's top-level type.

 * <i>subtype</i>: The media type's subtype.

<a id="Void_ctor"></a>
### Builder Constructor

    public Builder();

Initializes a new instance of the [PeterO.Mail.MediaType.Builder](PeterO.Mail.MediaType.Builder.md) class, using the type "application/octet-stream" .

<a id="SubType"></a>
### SubType

    public string SubType { get; set; }

Gets or sets this value's subtype.

<b>Returns:</b>

A text string naming this object's subtype, such as "plain" or "xml".

<b>Exceptions:</b>

 * System.ArgumentNullException:
The property is being set and the value is null.

 * System.ArgumentException:
The property is being set and the value is syntactically invalid for a subtype.

<a id="TopLevelType"></a>
### TopLevelType

    public string TopLevelType { get; set; }

Gets or sets this value's top-level type.

<b>Returns:</b>

A text string naming this object's top-level type, such as "text" or "audio" .

<b>Exceptions:</b>

 * System.ArgumentNullException:
The property is being set and the value is null.

 * System.ArgumentException:
The property is being set and the value is syntactically invalid for a top-level type.

<a id="RemoveParameter_string"></a>
### RemoveParameter

    public PeterO.Mail.MediaType.Builder RemoveParameter(
        string name);

Removes a parameter from this builder object. Does nothing if the parameter's name doesn't exist.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to remove. The name is compared using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic uppercase letters A to Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

<a id="SetParameter_string_string"></a>
### SetParameter

    public PeterO.Mail.MediaType.Builder SetParameter(
        string name,
        string value);

Sets a parameter's name and value for this media type.

<b>Parameters:</b>

 * <i>name</i>: Name of the parameter to set, such as "charset" . The name is compared using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic uppercase letters A to Z (U+0041 to U+005A) in both strings to basic lowercase letters.).

 * <i>value</i>: A text string giving the parameter's value.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>value</i>
 or  <i>name</i>
 is null.

 * System.ArgumentException:
The parameter  <i>name</i>
 is empty or syntactically invalid.

<a id="SetSubType_string"></a>
### SetSubType

    public PeterO.Mail.MediaType.Builder SetSubType(
        string str);

Sets this media type's subtype, such as "plain" or "xml" . This method enables the pattern of method chaining (for example,  `new...().Set...().Set...()`  ) unlike with the SubType property in.NET or the setSubType method (with small s) in Java.

<b>Parameters:</b>

 * <i>str</i>: A text string naming a media subtype.

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The parameter  <i>str</i>
 is empty or syntactically invalid.

<a id="SetTopLevelType_string"></a>
### SetTopLevelType

    public PeterO.Mail.MediaType.Builder SetTopLevelType(
        string str);

Sets this media type's top-level type. This method enables the pattern of method chaining (for example,  `new...().Set...().Set...()`  ) unlike with the TopLevelType property in.NET or the setTopLevelType method (with small s) in Java.

<b>Parameters:</b>

 * <i>str</i>: A text string naming a top-level type, such as "text" or "audio" .

<b>Return Value:</b>

This instance.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
The parameter  <i>str</i>
 is syntactically invalid for a top-level type.

<a id="ToMediaType"></a>
### ToMediaType

    public PeterO.Mail.MediaType ToMediaType();

Converts this builder to an immutable media type object.

<b>Return Value:</b>

A MediaType object.

<a id="ToString"></a>
### ToString

    public override string ToString();

Converts this object to a text string of the media type it represents, in the same form as  `MediaType.ToString` .

<b>Return Value:</b>

A string representation of this object.
