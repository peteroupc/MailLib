## PeterO.Mail.ContentDisposition

    public class ContentDisposition

 Specifies how a message body should be displayed or handled by a mail user agent. This type is immutable; its contents can't be changed after it's created. To create a changeable disposition object, use the DispositionBuilder class.

 <b>About the "filename" parameter</b>

 The "filename" parameter of a content disposition suggests a name to use when saving data to a file. For the "filename" parameter, the GetParameter method and Parameters property (  `getParameters`  ) method in Java) do not adapt that parameter's value using the ContentDisposition.MakeFilename method. Thus, for example, the "filename" parameter, if any, returned by this method could have an arbitrary length, be encoded using RFC 2047 encoded words (which some email and HTTP implementations still like to write out in headers, even though that RFC says encoded words "MUST NOT appear within a 'quoted-string'"; see ContentDisposition.MakeFilename), or not be usable as is as a file name.

 <b>Example:</b> An example of RFC 2047 encoded words is:

 <b>=?UTF-8?Q?test?=</b>

 Content-Disposition header fields like the following have appeared in practice:

 <b>Content-Disposition: attachment; filename==?UTF-8?Q?example?=</b>

 <b>Content-Disposition: attachment; filename==?UTF-8?Q?test.png?=</b>

 <b>Content-Disposition: attachment; filename="=?UTF-8?Q?test.png?="</b>

 In this implementation, the first and second of these are syntactically invalid, so they trigger parse errors, while the third of these is syntactically valid, but the "filename" parameter is treated as "=?UTF-8?Q?test.png?=", not "test.png" or something else -- RFC 2047 encoded words are not decoded at the moment a content disposition is parsed (using the Parse method).

### Member Summary
* <code>[public static readonly PeterO.Mail.ContentDisposition Attachment;](#Attachment)</code> - The content disposition value "attachment" .
* <code>[DispositionType](#DispositionType)</code> - Gets a string containing this object's disposition type, such as "inline" or "attachment".
* <code>[Equals(object)](#Equals_object)</code> - Determines whether this object and another object are equal.
* <code>[GetCreationDate()](#GetCreationDate)</code> - Gets the date and time extracted from this content disposition's "creation-date" parameter, which specifies the date of creation of a file (RFC 2183 sec.
* <code>[GetFilename()](#GetFilename)</code> - Gets an adapted version of the "filename" parameter in this content disposition object by using the "MakeFilename" method.
* <code>[GetHashCode()](#GetHashCode)</code> - Calculates the hash code of this object.
* <code>[GetModificationDate()](#GetModificationDate)</code> - Gets the date and time extracted from this content disposition's "modification-date" parameter, which specifies the date of last modification of a file (RFC 2183 sec.
* <code>[GetParameter(string)](#GetParameter_string)</code> - Gets a parameter from this disposition object.
* <code>[GetReadDate()](#GetReadDate)</code> - Gets the date and time extracted from this content disposition's "read-date" parameter, which specifies the date at which a file was last read (RFC 2183 sec.
* <code>[public static readonly PeterO.Mail.ContentDisposition Inline;](#Inline)</code> - The content disposition value "inline" .
* <code>[IsAttachment](#IsAttachment)</code> - Gets a value indicating whether the disposition type is attachment.
* <code>[IsInline](#IsInline)</code> - Gets a value indicating whether the disposition type is inline.
* <code>[MakeFilename(string)](#MakeFilename_string)</code> - Converts a file name from the Content-Disposition header to a suitable name for saving data to a file.
* <code>[Parameters](#Parameters)</code> - Gets a list of parameter names associated with this object and their values.
* <code>[Parse(string)](#Parse_string)</code> - Creates a new content disposition object from the value of a Content-Disposition header field.
* <code>[Parse(string, PeterO.Mail.ContentDisposition)](#Parse_string_PeterO_Mail_ContentDisposition)</code> - Parses a content disposition string and returns a content disposition object, or the default value if the string is invalid.
* <code>[ToSingleLineString()](#ToSingleLineString)</code> - Converts this content disposition to a text string form suitable for inserting in HTTP headers.
* <code>[ToString()](#ToString)</code> - Converts this content disposition to a text string form suitable for inserting in email headers.

<a id="Attachment"></a>
### Attachment

    public static readonly PeterO.Mail.ContentDisposition Attachment;

 The content disposition value "attachment" .

  <a id="Inline"></a>
### Inline

    public static readonly PeterO.Mail.ContentDisposition Inline;

 The content disposition value "inline" .

  <a id="DispositionType"></a>
### DispositionType

    public string DispositionType { get; }

 Gets a string containing this object's disposition type, such as "inline" or "attachment". Note that under RFC 6266 sec. 4.2 and RFC 2183 sec. 2.8, unrecognized disposition types should be treated as "attachment".

 <b>Returns:</b>

A string containing this object's disposition type, such as "inline" or "attachment".

<a id="IsAttachment"></a>
### IsAttachment

    public bool IsAttachment { get; }

 Gets a value indicating whether the disposition type is attachment.

 <b>Returns:</b>

 `true`  If the disposition type is attachment; otherwise,  `false`  .

<a id="IsInline"></a>
### IsInline

    public bool IsInline { get; }

 Gets a value indicating whether the disposition type is inline.

 <b>Returns:</b>

 `true`  If the disposition type is inline; otherwise,  `false`  .

<a id="Parameters"></a>
### Parameters

    public System.Collections.Generic.IDictionary Parameters { get; }

 Gets a list of parameter names associated with this object and their values. For the "filename" parameter, the value of that parameter is not adapted with the ContentDisposition.MakeFilename method; see the documentation for the ContentDisposition class.

 <b>Returns:</b>

A read-only list of parameter names associated with this object and their values. NOTE: Previous versions erroneously stated that the list will be sorted by name. In fact, the names will not be guaranteed to appear in any particular order; this is at least the case in version 0.10.0.

<a id="Equals_object"></a>
### Equals

    public override bool Equals(
        object obj);

 Determines whether this object and another object are equal.

 <b>Parameters:</b>

 * <i>obj</i>: The parameter  <i>obj</i>
 is an arbitrary object.

<b>Return Value:</b>

 `true`  if the objects are equal; otherwise,  `false`  .

<a id="GetCreationDate"></a>
### GetCreationDate

    public int[] GetCreationDate();

 Gets the date and time extracted from this content disposition's "creation-date" parameter, which specifies the date of creation of a file (RFC 2183 sec. 2.4). See **M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)** for information on the format of this method's return value.

 <b>Return Value:</b>

The extracted date and time as an 8-element array, or  `null`  if the "creation-date" parameter doesn't exist, is an empty string, or is syntactically invalid, or if the parameter's year would overflow a 32-bit signed integer.

<a id="GetFilename"></a>
### GetFilename

    public string GetFilename();

 Gets an adapted version of the "filename" parameter in this content disposition object by using the "MakeFilename" method.

 <b>Return Value:</b>

The adapted file name in the form of a string. Returns the empty string if there is no "filename" parameter or that parameter is empty.

<a id="GetHashCode"></a>
### GetHashCode

    public override int GetHashCode();

 Calculates the hash code of this object. No application or process IDs are used in the hash code calculation.

 <b>Return Value:</b>

A 32-bit hash code.

<a id="GetModificationDate"></a>
### GetModificationDate

    public int[] GetModificationDate();

 Gets the date and time extracted from this content disposition's "modification-date" parameter, which specifies the date of last modification of a file (RFC 2183 sec. 2.5). See **M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)** for information on the format of this method's return value.

 <b>Return Value:</b>

The extracted date and time as an 8-element array, or  `null`  if the "modification-date" parameter doesn't exist, is an empty string, or is syntactically invalid, or if the parameter's year would overflow a 32-bit signed integer.

<a id="GetParameter_string"></a>
### GetParameter

    public string GetParameter(
        string name);

 Gets a parameter from this disposition object. For the "filename" parameter, the value of that parameter is not adapted with the ContentDisposition.MakeFilename method; see the documentation for the ContentDisposition class.

 <b>Parameters:</b>

 * <i>name</i>: The name of the parameter to get. The name will be matched using a basic case-insensitive comparison. (Two strings are equal in such a comparison, if they match after converting the basic upper-case letters A to Z (U+0041 to U+005A) in both strings to lower case.). Can't be null.

<b>Return Value:</b>

The value of the parameter, or null if the parameter does not exist.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

 * System.ArgumentException:
The parameter  <i>name</i>
 is empty.

<a id="GetReadDate"></a>
### GetReadDate

    public int[] GetReadDate();

 Gets the date and time extracted from this content disposition's "read-date" parameter, which specifies the date at which a file was last read (RFC 2183 sec. 2.6). See **M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)** for information on the format of this method's return value.

 <b>Return Value:</b>

The extracted date and time as an 8-element array, or  `null`  if the "read-date" parameter doesn't exist, is an empty string, or is syntactically invalid, or if the parameter's year would overflow a 32-bit signed integer.

<a id="MakeFilename_string"></a>
### MakeFilename

    public static string MakeFilename(
        string str);

 Converts a file name from the Content-Disposition header to a suitable name for saving data to a file. This method is idempotent; that is, calling the method again on the result doesn't change that result. Examples:

  `"=?utf-8?q?hello=2Etxt?=" ->"hello.txt"`  (RFC 2047 encoding).

  `"=?utf-8?q?long_filename?=" ->"long filename"`  (RFC 2047 encoding).

  `"utf-8'en'hello%2Etxt" ->"hello.txt"`  (RFC 2231 encoding).

  `"nul.txt" ->"_nul.txt"`  (Reserved name).

  `"dir1/dir2/file" ->"dir1_dir2_file"`  (Directory separators).

 <b>Remarks:</b>

  * The exact file name conversion used by this method is not guaranteed to remain the same between versions of this library, with the exception that the return value will be in normalization form C, will not contain base + slash code points, will not be null, and will be an empty string only if  <i>str</i>
 is null or empty.

  * The string returned by this method is normalized using Unicode normalization form C (NFC) (see the [PeterO.Text.NormalizerInput](PeterO.Text.NormalizerInput.md) class for details). Although most file systems preserve the normalization of file names, there is one notable exception: The HFS Plus file system (on macOS before High Sierra) stores file names using a modified version of normalization form D (NFD) in which certain code points are not decomposed, including all base + slash code points, which are the only composed code points in Unicode that are decomposed in NFD but not in HFS Plus's version of NFD. If the filename will be used to save a file to an HFS Plus storage device, it is enough to normalize the return value with NFD for this purpose (because all base + slash code points were converted beforehand by MakeFilename to an alternate form). See also Apple's Technical Q&A "Text Encodings in VFS" and Technical Note TN1150, "HFS Plus Volume Format".

  * Email and HTTP headers may specify suggested filenames using the Content-Disposition header field's  `filename`  parameter or, in practice, the Content-Type header field's  `name`  parameter.

 Although RFC 2047 encoded words appearing in both parameters are written out by some implementations, this practice is often discouraged (especially since the RFC itself says that encoded words "MUST NOT appear within a 'quoted-string'"). Nevertheless, the MakeFilename method has a basis in the RFCs to decode RFC 2047 encoded words (and RFC 2231 encoding) in file names passed to this method.

 RFC 2046 sec. 4.5.1 (  `application/octet-stream`  subtype in Content-Type header field) cites an earlier RFC 1341, which "defined the use of a 'NAME' parameter which gave a <i> suggested </i> file name to be used if the data were written to a file". Also, RFC 2183 sec. 2.3 (  `filename`  parameter in Content-Disposition) confirms that the " <i> suggested </i> filename" in the  `filename`  parameter "should be <i> used as a basis </i> for the actual filename, where possible", and that that file name should "not [be] blindly use[d]". See also RFC 6266, section 4.3, which discusses the use of that parameter in Hypertext Transfer Protocol (HTTP).

 To the extent that the "name" parameter is not allowed in message bodies other than those with the media type "application/octet-stream" or treated as that media-type, this is a deviation of RFC 2045 and 2046 (see also RFC 2045 sec. 5, which says that "[t]here are NO globally meaningful parameters that apply to all media types"). (Some email implementations may still write out the "name" parameter, even for media types other than  `application/octet-stream`  and even though RFC 2046 has deprecated that parameter.)

  .

 <b>Parameters:</b>

 * <i>str</i>: A string representing a file name. Can be null.

<b>Return Value:</b>

A string with the converted version of the file name. Among other things, encoded words under RFC 2047 are decoded (since they occur so frequently in Content-Disposition filenames); the value is decoded under RFC 2231 if possible; characters unsuitable for use in a filename (including the directory separators slash and backslash) are replaced with underscores; spaces and tabs are collapsed to a single space; leading and trailing spaces and tabs are removed; and the filename is truncated if it would otherwise be too long. Also, for reasons stated in the remarks, a character that is the combined form of a base character and a combining slash is replaced with "!" followed by the base character. The returned string will be in normalization form C. Returns the empty string if  <i>str</i>
 is null or empty.

<a id="Parse_string_PeterO_Mail_ContentDisposition"></a>
### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispositionValue,
        PeterO.Mail.ContentDisposition defaultValue);

 Parses a content disposition string and returns a content disposition object, or the default value if the string is invalid. This method checks the syntactic validity of the string, but not whether it has all parameters it's required to have or whether the parameters themselves are set to valid values for the parameter. This method assumes the given content disposition string was directly extracted from the Content-Disposition header field (as defined for email messages) and follows the syntax given in RFC 2183. Accordingly, among other things, the content disposition string can contain comments (delimited by parentheses).

 RFC 2231 extensions allow each content disposition parameter to be associated with a character encoding and/or language, and support parameter values that span two or more key-value pairs. Parameters making use of RFC 2231 extensions have names with an asterisk ("*"). Such a parameter will be ignored if it is ill-formed because of RFC 2231's rules (except for illegal percent-decoding or undecodable sequences for the given character encoding). Examples of RFC 2231 extensions follow (both examples encode the same "filename" parameter):

 <b>inline; filename*=utf-8'en'filename.txt</b>

 <b>inline; filename*0*=utf-8'en'file; filename*1*=name%2Etxt</b>

 This implementation ignores keys (in parameter key-value pairs) that appear more than once in the content disposition. Nothing in RFCs 2045, 2183, 2231, 6266, or 7231 explicitly disallows such keys, or otherwise specifies error-handling behavior for such keys.

 <b>Parameters:</b>

 * <i>dispositionValue</i>: A text string that should be the value of a Content-Disposition header field.

 * <i>defaultValue</i>: The value to return in case the disposition value is syntactically invalid. Can be null.

<b>Return Value:</b>

A ContentDisposition object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>dispositionValue</i>
 is null.

<a id="Parse_string"></a>
### Parse

    public static PeterO.Mail.ContentDisposition Parse(
        string dispoValue);

 Creates a new content disposition object from the value of a Content-Disposition header field.

 <b>Parameters:</b>

 * <i>dispoValue</i>: The parameter  <i>dispoValue</i>
 is a text string.

<b>Return Value:</b>

A content disposition object, or ContentDisposition.Attachment" if  <i>dispoValue</i>
 is empty or syntactically invalid.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>dispoValue</i>
 is null.

<a id="ToSingleLineString"></a>
### ToSingleLineString

    public string ToSingleLineString();

 Converts this content disposition to a text string form suitable for inserting in HTTP headers. Notably, the string contains the value of a Content-Disposition header field (without the text necessarily starting with "Content-Disposition" followed by a space), and consists of a single line.

 <b>Return Value:</b>

A text string form of this content disposition.

<a id="ToString"></a>
### ToString

    public override string ToString();

 Converts this content disposition to a text string form suitable for inserting in email headers. Notably, the string contains the value of a Content-Disposition header field (without the text necessarily starting with "Content-Disposition" followed by a space), and consists of one or more lines.

 <b>Return Value:</b>

A text string form of this content disposition.
