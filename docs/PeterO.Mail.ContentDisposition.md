## PeterO.Mail.ContentDisposition

    public class ContentDisposition

Specifies how a message body should be displayed or handled by a mail ser agent. This type is immutable; its contents can't be changed after t's created. To create a changeable disposition object, use the ispositionBuilder class.<b>About the "filename" parameter</b>

The "filename" parameter of a content disposition suggests a name to se when saving data to a file. For the "filename" parameter, the etParameter method and Parameters property ( `
          getParameters
        ` ) method in Java) do not adapt that parameter's value using the ontentDisposition.MakeFilename method. Thus, for example, the filename" parameter, if any, returned by this method could have an rbitrary length, be encoded using RFC 2047 encoded words (which some mail and HTTP implementations still like to write out in headers, even hough that RFC says encoded words "MUST NOT appear within a quoted-string'"; see ContentDisposition.MakeFilename), or not be usable s is as a file name.

<b>Example:</b>An example of RFC 2047 encoded words is:

<b>=?UTF-8?Q?test?=</b>

Content-Disposition header fields like the following have appeared in ractice:

<b>Content-Disposition: attachment; filename==?UTF-8?Q?example?=</b>

<b>Content-Disposition: attachment; filename==?UTF-8?Q?test.png?=</b>

<b>Content-Disposition: attachment; filename="=?UTF-8?Q?test.png?="</b>

In this implementation, the first and second of these are syntactically nvalid, so they trigger parse errors, while the third of these is yntactically valid, but the "filename" parameter is treated as =?UTF-8?Q?test.png?=", not "test.png" or something else -- RFC 2047 ncoded words are not decoded at the moment a content disposition is arsed (using the Parse method).

### Attachment

    public static readonly PeterO.Mail.ContentDisposition Attachment;

The content disposition value "attachment".

### Inline

    public static readonly PeterO.Mail.ContentDisposition Inline;

The content disposition value "inline".

### DispositionType

    public string DispositionType { get; }

Gets a string containing this object's disposition type, such as "inline" r "attachment". Note that under RFC 6266 sec. 4.2 and RFC 2183 sec. 2.8, nrecognized disposition types should be treated as "attachment".

<b>Returns:</b>

A string containing this object's disposition type, such as "inline" or attachment".

### IsAttachment

    public bool IsAttachment { get; }

Gets a value indicating whether the disposition type is attachment.

<b>Returns:</b>

 `
        true
      ` If the disposition type is attachment; otherwise, `
        false
      ` .

### IsInline

    public bool IsInline { get; }

Gets a value indicating whether the disposition type is inline.

<b>Returns:</b>

 `
        true
      ` If the disposition type is inline; otherwise, `
        false
      ` .

### Parameters

    public System.Collections.Generic.IDictionary Parameters { get; }

Gets a list of parameter names associated with this object and their alues.For the "filename" parameter, the value of that parameter is not dapted with the ContentDisposition.MakeFilename method; see the ocumentation for the ContentDisposition class.

<b>Returns:</b>

A read-only list of parameter names associated with this object and their alues.NOTE: Previous versions erroneously stated that the list will be sorted y name. In fact, the names will not be guaranteed to appear in any articular order; this is at least the case in version 0.10.0.

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: The parameter <i>obj</i>
is an arbitrary object.

<b>Return Value:</b>

 `
        true
      ` if the objects are equal; otherwise, `
        false
      ` .

### GetCreationDate

    public int[] GetCreationDate();

Gets the date and time extracted from this content disposition's creation-date" parameter, which specifies the date of creation of a file RFC 2183 sec. 2.4). See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this method's return value.

<b>Return Value:</b>

The extracted date and time as an 8-element array, or `
        null
      ` if the "creation-date" parameter doesn't exist, is an empty string, or is yntactically invalid, or if the parameter's year would overflow a 32-bit igned integer.

### GetFilename

    public string GetFilename();

Gets an adapted version of the "filename" parameter in this content isposition object by using the "MakeFilename" method.

<b>Return Value:</b>

The adapted file name in the form of a string. Returns the empty string f there is no "filename" parameter or that parameter is empty.

### GetHashCode

    public override int GetHashCode();

Calculates the hash code of this object. No application or process IDs re used in the hash code calculation.

<b>Return Value:</b>

A 32-bit hash code.

### GetModificationDate

    public int[] GetModificationDate();

Gets the date and time extracted from this content disposition's modification-date" parameter, which specifies the date of last odification of a file (RFC 2183 sec. 2.5). See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this method's return value.

<b>Return Value:</b>

The extracted date and time as an 8-element array, or `
        null
      ` if the "modification-date" parameter doesn't exist, is an empty string, r is syntactically invalid, or if the parameter's year would overflow a 2-bit signed integer.

### GetParameter

    public string GetParameter(
        string name);

Gets a parameter from this disposition object. For the "filename" arameter, the value of that parameter is not adapted with the ontentDisposition.MakeFilename method; see the documentation for the ontentDisposition class.

<b>Parameters:</b>

 * <i>name</i>: The name of the parameter to get. The name will be matched using a basic ase-insensitive comparison. (Two strings are equal in such a comparison, f they match after converting the basic upper-case letters A to Z (U+0041 o U+005A) in both strings to lower case.). Can't be null.

<b>Return Value:</b>

The value of the parameter, or null if the parameter does not exist.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>name</i>
is null.

 * System.ArgumentException:
The parameter <i>name</i>
is empty.

### GetReadDate

    public int[] GetReadDate();

Gets the date and time extracted from this content disposition's read-date" parameter, which specifies the date at which a file was last ead (RFC 2183 sec. 2.6). See**PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)**for information on the format of this method's return value.

<b>Return Value:</b>

The extracted date and time as an 8-element array, or `
        null
      ` if the "read-date" parameter doesn't exist, is an empty string, or is yntactically invalid, or if the parameter's year would overflow a 32-bit igned integer.

### MakeFilename

    public static string MakeFilename(
        string str);

Converts a file name from the Content-Disposition header to a suitable ame for saving data to a file. This method is idempotent; that is, alling the method again on the result doesn't change that result.Examples:

 `
          "=?utf-8?q?hello=2Etxt?=" -> "hello.txt"
        ` (RFC 2047 encoding)

 `
          "=?utf-8?q?long_filename?=" -> "long filename"
        ` (RFC 2047 encoding)

 `
          "utf-8'en'hello%2Etxt" -> "hello.txt"
        ` (RFC 2231 encoding)

 `
          "nul.txt" -> "_nul.txt"
        ` (Reserved name)

 `
          "dir1/dir2/file" -> "dir1_dir2_file"
        ` (Directory separators)

<b>Remark:</b>Email and HTTP headers may specify suggested filenames using the ontent-Disposition header field's `
          filename
        ` parameter or, in practice, the Content-Type header field's `
          name
        ` parameter.

Although RFC 2047 encoded words appearing in both parameters are ritten out by some implementations, this practice is discouraged by ome (especially since the RFC itself says that encoded words "MUST NOT ppear within a 'quoted-string'"). Nevertheless, the MakeFilename method as a basis in the RFCs to decode RFC 2047 encoded words (and RFC 2231 ncoding) in file names passed to this method.

RFC 2046 sec. 4.5.1 ( `
          application/octet-stream
        ` subtype in Content-Type header field) cites an earlier RFC 1341, which defined the use of a 'NAME' parameter which gave a<i>suggested</i>file name to be used if the data were written to a file". Also, RFC 183 sec. 2.3 ( `
          filename
        ` parameter in Content-Disposition) confirms that the "<i>suggested</i>filename" in the `
          filename
        ` parameter "should be<i>used as a basis</i>for the actual filename, where possible", and that that file name hould "not [be] blindly use[d]". See also RFC 6266, section 4.3, which iscusses the use of that parameter in Hypertext Transfer Protocol HTTP).

To the extent that the "name" parameter is not allowed in message odies other than those with the media type "application/octet-stream" r treated as that media-type, this is a deviation of RFC 2045 and 2046 see also RFC 2045 sec. 5, which says that "[t]here are NO globally eaningful parameters that apply to all media types"). (Some email mplementations may still write out the "name" parameter, even in media ypes other than `
          application/octet-stream
        ` and even though RFC 2046 has deprecated that parameter.)

<b>Parameters:</b>

 * <i>str</i>: A string representing a file name. Can be null.

<b>Return Value:</b>

A string with the converted version of the file name. Among other things, ncoded words under RFC 2047 are decoded (since they occur so frequently n Content-Disposition filenames); the value is decoded under RFC 2231 if ossible; characters unsuitable for use in a filename (including the irectory separators slash and backslash) are replaced with underscores; paces and tabs are collapsed to a single space; leading and trailing paces and tabs are removed; and the filename is truncated if it would therwise be too long. The returned string will be in normalization form . Returns the empty string if "str" is null or empty.

### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispositionValue,
        PeterO.Mail.ContentDisposition defaultValue);

Parses a content disposition string and returns a content disposition bject, or the default value if the string is invalid. This method checks he syntactic validity of the string, but not whether it has all arameters it's required to have or whether the parameters themselves are et to valid values for the parameter.This method assumes the given content disposition string was directly xtracted from the Content-Disposition header field (as defined for mail messages) and follows the syntax given in RFC 2183. Accordingly, mong other things, the content disposition string can contain comments delimited by parentheses).

RFC 2231 extensions allow each content disposition parameter to be ssociated with a character encoding and/or language, and support arameter values that span two or more key-value pairs. Parameters aking use of RFC 2231 extensions have names with an asterisk ("*"). uch a parameter will be ignored if it is ill-formed because of RFC 231's rules (except for illegal percent-decoding or undecodable equences for the given character enoding). Examples of RFC 2231 xtensions follow (both examples encode the same "filename" parameter):

<b>inline; filename*=utf-8'en'filename.txt</b>

<b>inline; filename*0*=utf-8'en'file; filename*1*=name%2Etxt</b>

This implementation ignores keys (in parameter key-value pairs) that ppear more than once in the content disposition. Nothing in RFCs 2045, 183, 2231, 6266, or 7231 explicitly disallows such keys, or otherwise pecifies error-handling behavior for such keys.

<b>Parameters:</b>

 * <i>dispositionValue</i>: A text string that should be the value of a Content-Disposition header ield.

 * <i>defaultValue</i>: The value to return in case the disposition value is syntactically nvalid. Can be null.

<b>Return Value:</b>

A ContentDisposition object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>dispositionValue</i>
is null.

### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispoValue);

Creates a new content disposition object from the value of a ontent-Disposition header field.

<b>Parameters:</b>

 * <i>dispoValue</i>: The parameter <i>dispoValue</i>
is a text string.

<b>Return Value:</b>

A content disposition object, or ContentDisposition.Attachment" if <i>dispoValue</i>
is empty or syntactically invalid.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>dispoValue</i>
is null.

### ToSingleLineString

    public string ToSingleLineString();

Converts this content disposition to a text string form suitable for nserting in HTTP headers. Notably, the string contains the value of a ontent-Disposition header field (without the text necessarily starting ith "Content-Disposition" followed by a space), and consists of a single ine.

<b>Return Value:</b>

A text string form of this content disposition.

### ToString

    public override string ToString();

Converts this content disposition to a text string form suitable for nserting in email headers. Notably, the string contains the value of a ontent-Disposition header field (without the text necessarily starting ith "Content-Disposition" followed by a space), and consists of one or ore lines.

<b>Return Value:</b>

A text string form of this content disposition.
