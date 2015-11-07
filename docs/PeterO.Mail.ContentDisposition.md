## PeterO.Mail.ContentDisposition

    public class ContentDisposition

Specifies how a message body should be displayed or handled by a mail user agent. This type is immutable; its contents can't be changed after it's created.

### Attachment

    public static readonly PeterO.Mail.ContentDisposition Attachment;

The content disposition value "attachment".

### Inline

    public static readonly PeterO.Mail.ContentDisposition Inline;

The content disposition value "inline".

### DispositionType

    public string DispositionType { get; }

Gets a string containing this object's disposition type, such as "inline" or "attachment".

<b>Returns:</b>

A string containing this object's disposition type, such as "inline" or "attachment".

### IsAttachment

    public bool IsAttachment { get; }

Gets a value indicating whether the disposition type is attachment.

<b>Returns:</b>

True if the disposition type is attachment; otherwise, false..

### IsInline

    public bool IsInline { get; }

Gets a value indicating whether the disposition type is inline.

<b>Returns:</b>

True if the disposition type is inline; otherwise, false..

### Parameters

    public System.Collections.Generic.IDictionary Parameters { get; }

Gets a list of parameter names associated with this object and their values.

<b>Returns:</b>

A list of parameter names associated with this object and their values. The names will be sorted.

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object.

<b>Returns:</b>

True if this object and another object are equal; otherwise, false.

### GetHashCode

    public override int GetHashCode();

Returns the hash code for this instance.

<b>Returns:</b>

A 32-bit signed integer.

### GetParameter

    public string GetParameter(
        string name);

Gets a parameter from this disposition object.

<b>Parameters:</b>

 * <i>name</i>: Another string object.

<b>Returns:</b>

A string object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

 * System.ArgumentException:
The parameter  <i>name</i>
 is empty.

### MakeFilename

    public static string MakeFilename(
        string str);

Converts a filename from the Content-Disposition header to a suitable name for saving data to a file.

<b>Parameters:</b>

 * <i>str</i>: Another string object.

<b>Returns:</b>

A string object.

### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispositionValue,
        PeterO.Mail.ContentDisposition defaultValue);

Creates a new content disposition object from the value of a Content-Disposition header field.

<b>Parameters:</b>

 * <i>dispositionValue</i>: A string object.

 * <i>defaultValue</i>: Another ContentDisposition object.

<b>Returns:</b>

A ContentDisposition object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>dispositionValue</i>
 is null.

### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispoValue);

Parses a content disposition string and returns a content disposition object.

<b>Parameters:</b>

 * <i>dispoValue</i>: A string object.

<b>Returns:</b>

A ContentDisposition object.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string object.
