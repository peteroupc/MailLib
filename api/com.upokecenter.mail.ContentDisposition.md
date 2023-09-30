# com.upokecenter.mail.ContentDisposition

    public class ContentDisposition extends Object

<p>Specifies how a message body should be displayed or handled by a mail
 user agent. This type is immutable; its contents can't be changed after it's
 created. To create a changeable disposition object, use the
 ContentDisposition.Builder class.</p> <p><b>About the "filename"
 parameter</b></p> <p>The "filename" parameter of a content disposition
 suggests a name to use when saving data to a file. For the "filename"
 parameter, the GetParameter method and Parameters property(<code>
 getParameters</code>) method in Java) do not adapt that parameter's value using
 the ContentDisposition.MakeFilename method. Thus, for example, the
 "filename" parameter, if any, returned by this method could have an
 arbitrary length, be encoded using RFC 2047 encoded words (which some email
 and HTTP implementations still like to write out in headers, even though
 that RFC says encoded words "MUST NOT appear within a 'quoted-string'"; see
 ContentDisposition.MakeFilename), or not be usable as is as a file name.</p>
 <p><b>Example:</b> An example of RFC 2047 encoded words is:</p>
 <p><b>=?UTF-8?Q?test?=</b></p> <p>Content-Disposition header fields like the
 following have appeared in practice:</p> <p><b>Content-Disposition:
 attachment; filename==?UTF-8?Q?example?=</b></p> <p><b>Content-Disposition:
 attachment; filename==?UTF-8?Q?test.png?=</b></p> <p><b>Content-Disposition:
 attachment; filename="=?UTF-8?Q?test.png?="</b></p> <p>In this
 implementation, the first and second of these are syntactically invalid, so
 they trigger parse errors, while the third of these is syntactically valid,
 but the "filename" parameter is treated as "=?UTF-8?Q?test.png?=", not
 "test.png" or something else -- RFC 2047 encoded words are not decoded at
 the moment a content disposition is parsed (using the Parse method).</p>

## Nested Classes

* `static final class  ContentDisposition.Builder`<br>
 A mutable data type that allows a content disposition to be built.

## Fields

* `static final ContentDisposition Attachment`<br>
 The content disposition value "attachment" .

* `static final ContentDisposition Inline`<br>
 The content disposition value "inline" .

## Methods

* `boolean equals(Object obj)`<br>
 Determines whether this object and another object are equal.

* `int[] GetCreationDate()`<br>
 Gets the date and time extracted from this content disposition's
 "creation-date" parameter, which specifies the date of creation of a file
 (RFC 2183 sec.

* `final String getDispositionType()`<br>
 Gets a string containing this object's disposition type, such as "inline" or
 "attachment".

* `String GetFilename()`<br>
 Gets an adapted version of the "filename" parameter in this content
 disposition object by using the "MakeFilename" method.

* `int[] GetModificationDate()`<br>
 Gets the date and time extracted from this content disposition's
 "modification-date" parameter, which specifies the date of last modification
 of a file (RFC 2183 sec.

* `String GetParameter(String name)`<br>
 Gets a parameter from this disposition object.

* `final Map<String,String> getParameters()`<br>
 Gets a list of parameter names associated with this object and their
 values.

* `int[] GetReadDate()`<br>
 Gets the date and time extracted from this content disposition's "read-date"
 parameter, which specifies the date at which a file was last read (RFC 2183
 sec.

* `int hashCode()`<br>
 Calculates the hash code of this object.

* `final boolean isAttachment()`<br>
 Gets a value indicating whether the disposition type is attachment.

* `final boolean isInline()`<br>
 Gets a value indicating whether the disposition type is inline.

* `static String MakeFilename(String str)`<br>
 Converts a file name from the Content-disposition header field (or
 another string representing a title and an optional file extension) to a
 suitable name for saving data to a file.

* `static ContentDisposition Parse(String dispoValue)`<br>
 Creates a new content disposition object from the value of a
 Content-Disposition header field.

* `static ContentDisposition Parse(String dispositionValue,
 ContentDisposition defaultValue)`<br>
 Parses a content disposition string and returns a content disposition
 object, or the default value if the string is invalid.

* `String ToSingleLineString()`<br>
 Converts this content disposition to a text string form suitable for
 inserting in HTTP headers.

* `String toString()`<br>
 Converts this content disposition to a text string form suitable for
 inserting in email headers.

## Field Details

### Attachment
    public static final ContentDisposition Attachment
The content disposition value "attachment" .
### Inline
    public static final ContentDisposition Inline
The content disposition value "inline" .
## Method Details

### getDispositionType
    public final String getDispositionType()
Gets a string containing this object's disposition type, such as "inline" or
 "attachment". Note that under RFC 6266 sec. 4.2 and RFC 2183 sec. 2.8,
 unrecognized disposition types should be treated as "attachment". (There is
 no default content disposition in a message has no Content-Disposition
 header field.). The resulting string will be in lower case; that is, with
 its basic upper-case letters ("A" to "Z") converted to basic lower-case
 letters ("a" to "z").

**Returns:**

* A string containing this object's disposition type, such as "inline"
 or "attachment".

### equals
    public boolean equals(Object obj)
Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code> in class <code>Object</code>

**Parameters:**

* <code>obj</code> - The parameter <code>obj</code> is an arbitrary object.

**Returns:**

* <code>true</code> if the objects are equal; otherwise, <code>false</code>. In
 this method, two objects are not equal if they don't have the same type or
 if one is null and the other isn't.

### hashCode
    public int hashCode()
Calculates the hash code of this object. The exact algorithm used by this
 method may change between versions of this library, and no application or
 process IDs are used in the hash code calculation.

**Overrides:**

* <code>hashCode</code> in class <code>Object</code>

**Returns:**

* A 32-bit hash code.

### isInline
    public final boolean isInline()
Gets a value indicating whether the disposition type is inline.

**Returns:**

* <code>true</code> If the disposition type is inline; otherwise, <code>
 false</code>.

### isAttachment
    public final boolean isAttachment()
Gets a value indicating whether the disposition type is attachment.

**Returns:**

* <code>true</code> If the disposition type is attachment; otherwise,
 <code>false</code>.

### getParameters
    public final Map<String,String> getParameters()
<p>Gets a list of parameter names associated with this object and their
 values. Each parameter name will be in lower case; that is, with its basic
 upper-case letters ("A" to "Z") converted to basic lower-case letters ("a"
 to "z"). </p> <p>For the "filename" parameter, the value of that parameter
 is not adapted with the ContentDisposition.MakeFilename method; see the
 documentation for the ContentDisposition class.</p>

**Returns:**

* A read-only list of parameter names associated with this object and
 their values. NOTE: Previous versions erroneously stated that the list will
 be sorted by name. In fact, the names will not be guaranteed to appear in
 any particular order; this is at least the case in version 0.10.0.

### toString
    public String toString()
Converts this content disposition to a text string form suitable for
 inserting in email headers. Notably, the string contains the value of a
 Content-Disposition header field (without the text necessarily starting with
 "Content-Disposition" followed by a space), and consists of one or more
 lines.

**Overrides:**

* <code>toString</code> in class <code>Object</code>

**Returns:**

* A text string form of this content disposition.

### ToSingleLineString
    public String ToSingleLineString()
Converts this content disposition to a text string form suitable for
 inserting in HTTP headers. Notably, the string contains the value of a
 Content-Disposition header field (without the text necessarily starting with
 "Content-Disposition" followed by a space), and consists of a single line.

**Returns:**

* A text string form of this content disposition.

### MakeFilename
    public static String MakeFilename(String str)
<p>Converts a file name from the Content-disposition header field (or
 another string representing a title and an optional file extension) to a
 suitable name for saving data to a file. This method is idempotent; that is,
 calling the method again on the result doesn't change that result. The
 method avoids characters and combinations of characters that are problematic
 to use in certain file systems, and leaves the vast majority of file names
 seen in practice untouched. </p><p>Examples of how this method works
 follows:</p> <p><code>"=?utf-8?q?hello=2Etxt?=" -&amp;gt;"hello.txt"</code> (RFC 2047
 encoding).</p> <p><code>"=?utf-8?q?long_filename?=" -&amp;gt;"long filename"</code>
 (RFC 2047 encoding).</p> <p><code>"utf-8'en'hello%2Etxt" -&amp;gt;"hello.txt"</code>
 (RFC 2231 encoding).</p> <p><code>"nul.txt" -&amp;gt;"_nul.txt"</code> (Reserved
 name).</p> <p><code>"dir1/dir2/file" -&amp;gt;"dir1_dir2_file"</code> (Directory
 separators).</p><p><b>Remarks:</b></p> <ul> <li>This method should be used
 only to prepare a file name for the purpose of suggesting a name by which to
 save data. It should not be used to prepare file names of existing files for
 the purpose of reading them, since this method may replace certain
 characters with other characters in some cases, such that two different
 inputs may map to the same output.</li><li><b>File Name Support.</b> For
 recommendations on file name support, see " File Name Support in
 Applications ".</li><li><b>Guarantees.</b> The exact file name
 conversion used by this method is not guaranteed to remain the same between
 versions of this library, with the exception that the return value will be
 in normalization form C, will not contain base + slash code points, will not
 be null, and will be an empty string only if the parameter str is null or
 empty.</li><li> <p><b>'Name' and 'Filename' Parameters.</b> Email and HTTP
 headers may specify suggested filenames using the Content-Disposition header
 field's <code>filename</code> parameter or, in practice, the Content-Type header
 field's <code>name</code> parameter.</p> <p>Although RFC 2047 encoded words
 appearing in both parameters are written out by some implementations, this
 practice is often discouraged (especially since the RFC itself says that
 encoded words "MUST NOT appear within a 'quoted-string'"). Nevertheless, the
 MakeFilename method has a basis in the RFCs to decode RFC 2047 encoded words
 (and RFC 2231 encoding) in file names passed to this method.</p> <p>RFC 2046
 sec. 4.5.1 (<code>application/octet-stream</code> subtype in Content-Type header
 field) cites an earlier RFC 1341, which "defined the use of a 'NAME'
 parameter which gave a <i>suggested</i> file name to be used if the data
 were written to a file". Also, RFC 2183 sec. 2.3 (<code>filename</code>
 parameter in Content-Disposition) confirms that the " <i>suggested</i>
 filename" in the <code>filename</code> parameter "should be <i>used as a
 basis</i> for the actual filename, where possible", and that that file name
 should "not.get(be) blindly use.get(d)". See also RFC 6266, section 4.3,
 which discusses the use of that parameter in Hypertext Transfer Protocol
 (HTTP).</p> <p>To the extent that the "name" parameter is not allowed in
 message bodies other than those with the media type
 "application/octet-stream" or treated as that media type, this is a
 deviation of RFC 2045 and 2046 (see also RFC 2045 sec. 5, which says that
 "[t]here are NO globally meaningful parameters that apply to all media
 types"). (Some email implementations may still write out the "name"
 parameter, even for media types other than <code>application/octet-stream</code>
 and even though RFC 2046 has deprecated that parameter.)</p></li></ul>.

**Parameters:**

* <code>str</code> - A string representing a file name. Can be null.

**Returns:**

* A string with the converted version of the file name. Among other
 things, encoded words under RFC 2047 are decoded (since they occur so
 frequently in Content-Disposition filenames); the value is decoded under RFC
 2231 if possible; characters unsuitable for use in a filename (including the
 directory separators slash and backslash) are replaced with underscores;
 spaces and tabs are collapsed to a single space; leading and trailing spaces
 and tabs are removed; and the filename is truncated if it would otherwise be
 too long. Also, for reasons stated in the remarks, a character that is the
 combined form of a base character and a combining slash is replaced with "!"
 followed by the base character. The returned string will be in normalization
 form C. Returns the empty string if <code>str</code> is null or empty.

### GetFilename
    public String GetFilename()
Gets an adapted version of the "filename" parameter in this content
 disposition object by using the "MakeFilename" method.

**Returns:**

* The adapted file name in the form of a string. Returns the empty
 string if there is no "filename" parameter or that parameter is empty.

### GetCreationDate
    public int[] GetCreationDate()
Gets the date and time extracted from this content disposition's
 "creation-date" parameter, which specifies the date of creation of a file
 (RFC 2183 sec. 2.4). The parameter is parsed as though by <code>
 MailDateTime.ParseDateString</code> with obsolete time zones (including "GMT")
 allowed. See that method's documentation for information on the format of
 this method's return value.

**Returns:**

* The extracted date and time as an 8-element array, or <code>null</code>
 if the "creation-date" parameter doesn't exist, is an empty string, or is
 syntactically invalid, or if the parameter's year would overflow a 32-bit
 signed integer.

### GetModificationDate
    public int[] GetModificationDate()
Gets the date and time extracted from this content disposition's
 "modification-date" parameter, which specifies the date of last modification
 of a file (RFC 2183 sec. 2.5). The parameter is parsed as though by <code>
 MailDateTime.ParseDateString</code> with obsolete time zones (including "GMT")
 allowed. See that method's documentation for information on the format of
 this method's return value.

**Returns:**

* The extracted date and time as an 8-element array, or <code>null</code>
 if the "modification-date" parameter doesn't exist, is an empty string, or
 is syntactically invalid, or if the parameter's year would overflow a 32-bit
 signed integer.

### GetReadDate
    public int[] GetReadDate()
Gets the date and time extracted from this content disposition's "read-date"
 parameter, which specifies the date at which a file was last read (RFC 2183
 sec. 2.6). The parameter is parsed as though by <code>
 MailDateTime.ParseDateString</code> with obsolete time zones (including "GMT")
 allowed. See that method's documentation for information on the format of
 this method's return value.

**Returns:**

* The extracted date and time as an 8-element array, or <code>null</code>
 if the "read-date" parameter doesn't exist, is an empty string, or is
 syntactically invalid, or if the parameter's year would overflow a 32-bit
 signed integer.

### GetParameter
    public String GetParameter(String name)
Gets a parameter from this disposition object. For the "filename" parameter,
 the value of that parameter is not adapted with the
 ContentDisposition.MakeFilename method; see the documentation for the
 ContentDisposition class.

**Parameters:**

* <code>name</code> - The name of the parameter to get. The name will be matched using
 a basic case-insensitive comparison. (Two strings are equal in such a
 comparison, if they match after converting the basic upper-case letters A to
 Z (U+0041 to U+005A) in both strings to basic lower-case letters.). Can't be
 null.

**Returns:**

* The value of the parameter, or null if the parameter does not exist.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>name</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>name</code> is empty.

### Parse
    public static ContentDisposition Parse(String dispoValue)
Creates a new content disposition object from the value of a
 Content-Disposition header field.

**Parameters:**

* <code>dispoValue</code> - The parameter <code>dispoValue</code> is a text string.

**Returns:**

* A content disposition object, or ContentDisposition.Attachment" if
 <code>dispoValue</code> is empty or syntactically invalid.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>dispoValue</code> is null.

### Parse
    public static ContentDisposition Parse(String dispositionValue, ContentDisposition defaultValue)
<p>Parses a content disposition string and returns a content disposition
 object, or the default value if the string is invalid. This method checks
 the syntactic validity of the string, but not whether it has all parameters
 it's required to have or whether the parameters themselves are set to valid
 values for the parameter. </p><p>This method assumes the given content
 disposition string was directly extracted from the Content-Disposition
 header field (as defined for email messages) and follows the syntax given in
 RFC 2183. Accordingly, among other things, the content disposition string
 can contain comments (delimited by parentheses).</p> <p>RFC 2231 extensions
 allow each content disposition parameter to be associated with a character
 encoding and/or language, and support parameter values that span two or more
 key-value pairs. Parameters making use of RFC 2231 extensions have names
 with an asterisk ("*"). Such a parameter will be ignored if it is ill-formed
 because of RFC 2231's rules (except for illegal percent-decoding or
 undecodable sequences for the given character encoding). Examples of RFC
 2231 extensions follow (both examples encode the same "filename"
 parameter):</p> <p><b>inline; filename*=utf-8'en'filename.txt</b></p>
 <p><b>inline; filename*0*=utf-8'en'file; filename*1*=name%2Etxt</b></p>
 <p>This implementation ignores keys (in parameter key-value pairs) that
 appear more than once in the content disposition. Nothing in RFCs 2045,
 2183, 2231, 6266, or 7231 explicitly disallows such keys, or otherwise
 specifies error-handling behavior for such keys.</p>

**Parameters:**

* <code>dispositionValue</code> - A text string that should be the value of a
 Content-Disposition header field.

* <code>defaultValue</code> - The value to return in case the disposition value is
 syntactically invalid. Can be null.

**Returns:**

* A ContentDisposition object.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>dispositionValue</code> is
 null.
