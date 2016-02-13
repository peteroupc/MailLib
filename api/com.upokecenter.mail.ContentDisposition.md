# com.upokecenter.mail.ContentDisposition

    public class ContentDisposition extends Object

Specifies how a message body should be displayed or handled by a mail user
 agent. This type is immutable; its contents can't be changed after
 it's created. To create a changeable disposition object, use the
 DispositionBuilder class.

## Fields

* `static ContentDisposition Attachment`<br>
 The content disposition value "attachment".
* `static ContentDisposition Inline`<br>
 The content disposition value "inline".

## Methods

* `boolean equals(Object obj)`<br>
 Determines whether this object and another object are equal.
* `String getDispositionType()`<br>
 Gets a string containing this object's disposition type, such as "inline" or
 "attachment".
* `String GetParameter(String name)`<br>
 Gets a parameter from this disposition object.
* `Map<String,String> getParameters()`<br>
 Gets a list of parameter names associated with this object and their values.
* `int hashCode()`<br>
 Returns the hash code for this instance.
* `boolean isAttachment()`<br>
 Gets a value indicating whether the disposition type is attachment.
* `boolean isInline()`<br>
 Gets a value indicating whether the disposition type is inline.
* `static String MakeFilename(String str)`<br>
 Converts a file name from the Content-Disposition header to a suitable name
 for saving data to a file.
* `static ContentDisposition Parse(String dispoValue)`<br>
 Parses a content disposition string and returns a content disposition
 object.
* `static ContentDisposition Parse(String dispositionValue,
     ContentDisposition defaultValue)`<br>
 Creates a new content disposition object from the value of a
 Content-Disposition header field.
* `String toString()`<br>
 Converts this object to a text string.

## Field Details

### Attachment
    public static final ContentDisposition Attachment
The content disposition value "attachment".
### Inline
    public static final ContentDisposition Inline
The content disposition value "inline".
## Method Details

### getDispositionType
    public final String getDispositionType()
Gets a string containing this object's disposition type, such as "inline" or
 "attachment".

**Returns:**

* A string containing this object's disposition type, such as "inline"
 or "attachment".

### equals
    public boolean equals(Object obj)
Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code>&nbsp;in class&nbsp;<code>Object</code>

**Parameters:**

* <code>obj</code> - An arbitrary object.

**Returns:**

* True if the objects are equal; otherwise, false.

### hashCode
    public int hashCode()
Returns the hash code for this instance.

**Overrides:**

* <code>hashCode</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A 32-bit hash code.

### isInline
    public final boolean isInline()
Gets a value indicating whether the disposition type is inline.

**Returns:**

* True if the disposition type is inline; otherwise, false.

### isAttachment
    public final boolean isAttachment()
Gets a value indicating whether the disposition type is attachment.

**Returns:**

* True if the disposition type is attachment; otherwise, false.

### getParameters
    public final Map<String,String> getParameters()
Gets a list of parameter names associated with this object and their values.

**Returns:**

* A read-only list of parameter names associated with this object and
 their values. The names will be sorted.

### toString
    public String toString()
Converts this object to a text string.

**Overrides:**

* <code>toString</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A string representation of this object.

### MakeFilename
    public static String MakeFilename(String str)
Converts a file name from the Content-Disposition header to a suitable name
 for saving data to a file. <p>Examples:</p>
 <p>"=?utf-8?q?hello=2Etxt?=" -&gt; "hello.txt" (RFC 2047
 encoding)</p> <p>"=?utf-8?q?long_filename?=" -&gt; "long filename"
 (RFC 2047 encoding)</p> <p>"utf-8'en'hello%2Etxt" -&gt; "hello.txt"
 (RFC 2231 encoding)</p> <p>"nul.txt" -&gt; "_nul.txt" (Reserved
 name)</p> <p>"dir1/dir2/file" -&gt; "dir1_dir2_file" (Directory
 separators)</p>

**Parameters:**

* <code>str</code> - A string representing a file name. Can be null.

**Returns:**

* A string with the converted version of the file name. Among other
 things, encoded words under RFC 2047 are decoded (since they occur so
 frequently in Content-Disposition filenames); the value is decoded
 under RFC 2231 if possible; characters unsuitable for use in a
 filename (including the directory separators slash and backslash) are
 replaced with underscores; spaces and tabs are collapsed to a single
 space; leading and trailing spaces and tabs are removed; and the
 filename is truncated if it would otherwise be too long. The returned
 string will be in normalization form C. Returns an empty string if
 <code>str</code> is null.

**Throws:**

* <code>NullPointerException</code> - The parameter "name" or <code>str</code> or
 "dispoValue" or "dispositionValue" is null.

### GetParameter
    public String GetParameter(String name)
Gets a parameter from this disposition object.

**Parameters:**

* <code>name</code> - The name of the parameter to get. The name will be matched
 case-insensitively. Can't be null.

**Returns:**

* The value of the parameter, or null if the parameter does not exist.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>name</code> is empty.

### Parse
    public static ContentDisposition Parse(String dispoValue)
Parses a content disposition string and returns a content disposition
 object.

**Parameters:**

* <code>dispoValue</code> - A string object.

**Returns:**

* A content disposition object, or "Attachment" if <code>dispoValue</code>
 is empty or syntactically invalid.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>dispoValue</code> is null.

### Parse
    public static ContentDisposition Parse(String dispositionValue, ContentDisposition defaultValue)
Creates a new content disposition object from the value of a
 Content-Disposition header field.

**Parameters:**

* <code>dispositionValue</code> - A string object that should be the value of a
 Content-Disposition header field.

* <code>defaultValue</code> - The value to return in case the disposition value is
 syntactically invalid. Can be null.

**Returns:**

* A ContentDisposition object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>dispositionValue</code> is
 null.
