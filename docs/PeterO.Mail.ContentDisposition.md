## PeterO.Mail.ContentDisposition

    public class ContentDisposition

Specifies how a message body should be displayed or handled by a mail user agent. This type is immutable; its contents can't be changed after it's created.

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Returns:</b>

True if the objects are equal; otherwise, false.

### GetHashCode

    public override int GetHashCode();

Returns the hash code for this instance.

<b>Returns:</b>

A 32-bit hash code.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string representation of this object.

### MakeFilename

    public static string MakeFilename(
        string str);

Converts a filename from the Content-Disposition header to a suitable name for saving data to a file.

<b>Parameters:</b>

 * <i>str</i>: A string representing a file name.

<b>Returns:</b>

A string with the converted version of the file name. Among other things, encoded words under RFC 2047 are decoded (since they occur so frequently in Content-Disposition filenames); the value is decoded under RFC 2231 if possible; characters unsuitable for use in a filename (including the directory separators slash and backslash) are replaced with underscores; and the filename is truncated if it would otherwise be too long. Returns an empty string if  <i>str</i>
 is null.

### GetParameter

    public string GetParameter(
        string name);

Gets a parameter from this disposition object.

<b>Parameters:</b>

 * <i>name</i>: The name of the parameter to get. The name will be matched case-insensitively. Can't be null.

<b>Returns:</b>

The value of the parameter, or null if the parameter does not exist.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>name</i>
 is null.

 * System.ArgumentException: 
The parameter  <i>name</i>
 is empty.

### Attachment

    public static readonly PeterO.Mail.ContentDisposition Attachment;

The content disposition value "attachment".

### Inline

    public static readonly PeterO.Mail.ContentDisposition Inline;

The content disposition value "inline".

### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispoValue);

Parses a media type string and returns a media type object.

<b>Parameters:</b>

 * <i>dispoValue</i>: A string object.

<b>Returns:</b>

A media type object, or "Attachment" if  <i>dispoValue</i>
is empty or syntactically invalid.

### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispositionValue,
        PeterO.Mail.ContentDisposition defaultValue);

Creates a new content disposition object from the value of a Content-Disposition header field.

<b>Parameters:</b>

 * <i>dispositionValue</i>: A string object that should be the value of a Content-Disposition header field.

 * <i>defaultValue</i>: The value to return in case the disposition value is syntactically invalid. Can be null.

<b>Returns:</b>

A ContentDisposition object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>dispositionValue</i>
 is null.

### DispositionType

    public string DispositionType { get; }

Gets a string containing this object's disposition type, such as "inline" or "attachment".

<b>Returns:</b>

A string containing this object's disposition type, such as "inline" or "attachment".

### IsInline

    public bool IsInline { get; }

Gets a value indicating whether the disposition type is inline.

<b>Returns:</b>

True if the disposition type is inline; otherwise, false..

### IsAttachment

    public bool IsAttachment { get; }

Gets a value indicating whether the disposition type is attachment.

<b>Returns:</b>

True if the disposition type is attachment; otherwise, false..

### Parameters

    public System.Collections.Generic.IDictionary Parameters { get; }

Gets a list of parameter names associated with this object and their values.

<b>Returns:</b>

A list of parameter names associated with this object and their values. The names will be sorted.


