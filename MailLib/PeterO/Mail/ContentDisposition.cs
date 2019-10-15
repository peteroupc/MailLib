/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
  /// <summary>
  /// <para>Specifies how a message body should be displayed or handled
  /// by a mail user agent. This type is immutable; its contents can't be
  /// changed after it's created. To create a changeable disposition
  /// object, use the DispositionBuilder class.</para>
  /// <para><b>About the "filename" parameter</b></para>
  /// <para>The "filename" parameter of a content disposition suggests a
  /// name to use when saving data to a file. For the "filename"
  /// parameter, the GetParameter method and Parameters property (
  /// <c>getParameters</c> ) method in Java) do not adapt that
  /// parameter's value using the ContentDisposition.MakeFilename method.
  /// Thus, for example, the "filename" parameter, if any, returned by
  /// this method could have an arbitrary length, be encoded using RFC
  /// 2047 encoded words (which some email and HTTP implementations still
  /// like to write out in headers, even though that RFC says encoded
  /// words "MUST NOT appear within a 'quoted-string'"; see
  /// ContentDisposition.MakeFilename), or not be usable as is as a file
  /// name.</para>
  /// <para><b>Example:</b> An example of RFC 2047 encoded words
  /// is:</para>
  /// <para><b>=?UTF-8?Q?test?=</b></para>
  /// <para>Content-Disposition header fields like the following have
  /// appeared in practice:</para>
  /// <para><b>Content-Disposition: attachment;
  /// filename==?UTF-8?Q?example?=</b></para>
  /// <para><b>Content-Disposition: attachment;
  /// filename==?UTF-8?Q?test.png?=</b></para>
  /// <para><b>Content-Disposition: attachment;
  /// filename="=?UTF-8?Q?test.png?="</b></para>
  /// <para>In this implementation, the first and second of these are
  /// syntactically invalid, so they trigger parse errors, while the
  /// third of these is syntactically valid, but the "filename" parameter
  /// is treated as "=?UTF-8?Q?test.png?=", not "test.png" or something
  /// else -- RFC 2047 encoded words are not decoded at the moment a
  /// content disposition is parsed (using the Parse
  /// method).</para></summary>
  public class ContentDisposition {
    private readonly string dispositionType;

    /// <summary>Gets a string containing this object's disposition type,
    /// such as "inline" or "attachment". Note that under RFC 6266 sec. 4.2
    /// and RFC 2183 sec. 2.8, unrecognized disposition types should be
    /// treated as "attachment". (There is no default content disposition
    /// in a message has no Content-Disposition header field.). The
    /// resulting string will be in lower case; that is, with its basic
    /// upper-case letters ("A" to "Z") converted to basic lower-case
    /// letters ("a" to "z").</summary>
    /// <value>A string containing this object's disposition type, such as
    /// "inline" or "attachment".</value>
    public string DispositionType {
      get {
        return this.dispositionType;
      }
    }

    #region Equals and GetHashCode implementation

    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise,
    /// <c>false</c>.</returns>
    public override bool Equals(object obj) {
      var other = obj as ContentDisposition;
      if (other == null) {
        return false;
      }
      return this.dispositionType.Equals(other.dispositionType,
          StringComparison.Ordinal) &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /// <summary>Calculates the hash code of this object. The exact
    /// algorithm used by this method may change between versions of this
    /// library, and no application or process IDs are used in the hash
    /// code calculation.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      var hashCode = 632580499;
      if (this.dispositionType != null) {
        for (var i = 0; i < this.dispositionType.Length; ++i) {
          hashCode = unchecked(hashCode + (632580503 *
                this.dispositionType[i]));
        }
      }
      if (this.parameters != null) {
        hashCode = unchecked(hashCode + (632580587 *
              this.parameters.Count));
      }
      return hashCode;
    }
    #endregion

    /// <summary>Gets a value indicating whether the disposition type is
    /// inline.</summary>
    /// <value><c>true</c> If the disposition type is inline; otherwise,
    /// <c>false</c>.</value>
    public bool IsInline {
      get {
        return this.dispositionType.Equals("inline", StringComparison.Ordinal);
      }
    }

    /// <summary>Gets a value indicating whether the disposition type is
    /// attachment.</summary>
    /// <value><c>true</c> If the disposition type is attachment;
    /// otherwise, <c>false</c>.</value>
    public bool IsAttachment {
      get {
        return this.dispositionType.Equals("attachment",
            StringComparison.Ordinal);
      }
    }

    internal ContentDisposition (
      string type,
      IDictionary<string, string> parameters) {
      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }
      this.dispositionType = type;
      this.parameters = new Dictionary<string, string>(parameters);
    }

    private ContentDisposition() {
      this.dispositionType = String.Empty;
      this.parameters = new Dictionary<string, string>();
    }

    private readonly IDictionary<string, string> parameters;

    /// <summary>Gets a list of parameter names associated with this object
    /// and their values. Each parameter name will be in lower case; that
    /// is, with its basic upper-case letters ("A" to "Z") converted to
    /// basic lower-case letters ("a" to "z").
    /// <para>For the "filename" parameter, the value of that parameter is
    /// not adapted with the ContentDisposition.MakeFilename method; see
    /// the documentation for the ContentDisposition
    /// class.</para></summary>
    /// <value>A read-only list of parameter names associated with this
    /// object and their values. NOTE: Previous versions erroneously stated
    /// that the list will be sorted by name. In fact, the names will not
    /// be guaranteed to appear in any particular order; this is at least
    /// the case in version 0.10.0.</value>
    public IDictionary<string, string> Parameters {
      get {
        return new ReadOnlyMap<string, string>(this.parameters);
      }
    }

    /// <summary>Converts this content disposition to a text string form
    /// suitable for inserting in email headers. Notably, the string
    /// contains the value of a Content-Disposition header field (without
    /// the text necessarily starting with "Content-Disposition" followed
    /// by a space), and consists of one or more lines.</summary>
    /// <returns>A text string form of this content disposition.</returns>
    public override string ToString() {
      // NOTE: 21 is the length of "Content-Disposition: " (with
      // trailing space).
      var sa = new HeaderEncoder(Message.MaxRecHeaderLineLength, 21);
      sa.AppendSymbol(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sa);
      return sa.ToString();
    }

    /// <summary>Converts this content disposition to a text string form
    /// suitable for inserting in HTTP headers. Notably, the string
    /// contains the value of a Content-Disposition header field (without
    /// the text necessarily starting with "Content-Disposition" followed
    /// by a space), and consists of a single line.</summary>
    /// <returns>A text string form of this content disposition.</returns>
    public string ToSingleLineString() {
      // NOTE: 21 is the length of "Content-Disposition: " (with trailing
      // space).
      var sa = new HeaderEncoder(-1, 21);
      sa.AppendSymbol(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sa);
      return sa.ToString();
    }

    /// <summary>Converts a file name from the Content-disposition header
    /// field (or another string representing a title and an optional file
    /// extension) to a suitable name for saving data to a file. This
    /// method is idempotent; that is, calling the method again on the
    /// result doesn't change that result. The method avoids characters and
    /// combinations of characters that are problematic to use in certain
    /// file systems, and leaves the vast majority of file names seen in
    /// practice untouched.
    /// <para>Examples of how this method works follows:</para>
    /// <para><c>"=?utf-8?q?hello=2Etxt?=" -&gt;"hello.txt"</c> (RFC 2047
    /// encoding).</para>
    /// <para><c>"=?utf-8?q?long_filename?=" -&gt;"long filename"</c> (RFC
    /// 2047 encoding).</para>
    /// <para><c>"utf-8'en'hello%2Etxt" -&gt;"hello.txt"</c> (RFC 2231
    /// encoding).</para>
    /// <para><c>"nul.txt" -&gt;"_nul.txt"</c> (Reserved name).</para>
    /// <para><c>"dir1/dir2/file" -&gt;"dir1_dir2_file"</c> (Directory
    /// separators).</para></summary>
    /// <param name='str'>A string representing a file name. Can be
    /// null.</param>
    /// <returns>A string with the converted version of the file name.
    /// Among other things, encoded words under RFC 2047 are decoded (since
    /// they occur so frequently in Content-Disposition filenames); the
    /// value is decoded under RFC 2231 if possible; characters unsuitable
    /// for use in a filename (including the directory separators slash and
    /// backslash) are replaced with underscores; spaces and tabs are
    /// collapsed to a single space; leading and trailing spaces and tabs
    /// are removed; and the filename is truncated if it would otherwise be
    /// too long. Also, for reasons stated in the remarks, a character that
    /// is the combined form of a base character and a combining slash is
    /// replaced with "!" followed by the base character. The returned
    /// string will be in normalization form C. Returns the empty string if
    /// <paramref name='str'/> is null or empty.</returns>
    /// <remarks>
    /// <para><b>Remarks:</b></para>
    /// <list>
    /// <item>This method should be used only to prepare a file name for
    /// the purpose of suggesting a name by which to save data. It should
    /// not be used to prepare file names of existing files for the purpose
    /// of reading them, since this method may replace certain characters
    /// with other characters in some cases, such that two different inputs
    /// may map to the same output.</item>
    /// <item><b>File Name Support.</b> For recommendations on file name
    /// support, see "
    /// <a href='https://peteroupc.github.io/filenames.html'>File Name
    /// Support in Applications</a> ".</item>
    /// <item><b>Guarantees.</b> The exact file name conversion used by
    /// this method is not guaranteed to remain the same between versions
    /// of this library, with the exception that the return value will be
    /// in normalization form C, will not contain base + slash code points,
    /// will not be null, and will be an empty string only if <paramref
    /// name='str'/> is null or empty.</item>
    /// <item>
    /// <para><b>'Name' and 'Filename' Parameters.</b> Email and HTTP
    /// headers may specify suggested filenames using the
    /// Content-Disposition header field's <c>filename</c> parameter or, in
    /// practice, the Content-Type header field's <c>name</c>
    /// parameter.</para>
    /// <para>Although RFC 2047 encoded words appearing in both parameters
    /// are written out by some implementations, this practice is often
    /// discouraged (especially since the RFC itself says that encoded
    /// words "MUST NOT appear within a 'quoted-string'"). Nevertheless,
    /// the MakeFilename method has a basis in the RFCs to decode RFC 2047
    /// encoded words (and RFC 2231 encoding) in file names passed to this
    /// method.</para>
    /// <para>RFC 2046 sec. 4.5.1 ( <c>application/octet-stream</c> subtype
    /// in Content-Type header field) cites an earlier RFC 1341, which
    /// "defined the use of a 'NAME' parameter which gave a
    /// <i>suggested</i> file name to be used if the data were written to a
    /// file". Also, RFC 2183 sec. 2.3 ( <c>filename</c> parameter in
    /// Content-Disposition) confirms that the "
    /// <i>suggested</i> filename" in the <c>filename</c> parameter "should
    /// be
    /// <i>used as a basis</i> for the actual filename, where possible",
    /// and that that file name should "not [be] blindly use[d]". See also
    /// RFC 6266, section 4.3, which discusses the use of that parameter in
    /// Hypertext Transfer Protocol (HTTP).</para>
    /// <para>To the extent that the "name" parameter is not allowed in
    /// message bodies other than those with the media type
    /// "application/octet-stream" or treated as that media-type, this is a
    /// deviation of RFC 2045 and 2046 (see also RFC 2045 sec. 5, which
    /// says that "[t]here are NO globally meaningful parameters that apply
    /// to all media types"). (Some email implementations may still write
    /// out the "name" parameter, even for media types other than
    /// <c>application/octet-stream</c> and even though RFC 2046 has
    /// deprecated that parameter.)</para></item></list>.</remarks>
    public static string MakeFilename(string str) {
      return MakeFilenameMethod.MakeFilename(str);
    }

    /// <summary>Gets an adapted version of the "filename" parameter in
    /// this content disposition object by using the "MakeFilename"
    /// method.</summary>
    /// <returns>The adapted file name in the form of a string. Returns the
    /// empty string if there is no "filename" parameter or that parameter
    /// is empty.</returns>
    public string GetFilename() {
      return MakeFilename(this.GetParameter("filename"));
    }

    /// <summary>Gets the date and time extracted from this content
    /// disposition's "creation-date" parameter, which specifies the date
    /// of creation of a file (RFC 2183 sec. 2.4). The parameter is parsed
    /// as though by <c>MailDateTime.ParseDateString</c> with obsolete time
    /// zones (including "GMT") allowed. See that method's documentation
    /// for information on the format of this method's return
    /// value.</summary>
    /// <returns>The extracted date and time as an 8-element array, or
    /// <c>null</c> if the "creation-date" parameter doesn't exist, is an
    /// empty string, or is syntactically invalid, or if the parameter's
    /// year would overflow a 32-bit signed integer.</returns>
    public int[] GetCreationDate() {
      return MailDateTime.ParseDateString (
          this.GetParameter("creation-date"),
          true);
    }

    /// <summary>Gets the date and time extracted from this content
    /// disposition's "modification-date" parameter, which specifies the
    /// date of last modification of a file (RFC 2183 sec. 2.5). The
    /// parameter is parsed as though by
    /// <c>MailDateTime.ParseDateString</c> with obsolete time zones
    /// (including "GMT") allowed. See that method's documentation for
    /// information on the format of this method's return value.</summary>
    /// <returns>The extracted date and time as an 8-element array, or
    /// <c>null</c> if the "modification-date" parameter doesn't exist, is
    /// an empty string, or is syntactically invalid, or if the parameter's
    /// year would overflow a 32-bit signed integer.</returns>
    public int[] GetModificationDate() {
      return MailDateTime.ParseDateString (
          this.GetParameter("modification-date"),
          true);
    }

    /// <summary>Gets the date and time extracted from this content
    /// disposition's "read-date" parameter, which specifies the date at
    /// which a file was last read (RFC 2183 sec. 2.6). The parameter is
    /// parsed as though by <c>MailDateTime.ParseDateString</c> with
    /// obsolete time zones (including "GMT") allowed. See that method's
    /// documentation for information on the format of this method's return
    /// value.</summary>
    /// <returns>The extracted date and time as an 8-element array, or
    /// <c>null</c> if the "read-date" parameter doesn't exist, is an empty
    /// string, or is syntactically invalid, or if the parameter's year
    /// would overflow a 32-bit signed integer.</returns>
    public int[] GetReadDate() {
      return MailDateTime.ParseDateString (
          this.GetParameter("read-date"),
          true);
    }

    /// <summary>Gets a parameter from this disposition object. For the
    /// "filename" parameter, the value of that parameter is not adapted
    /// with the ContentDisposition.MakeFilename method; see the
    /// documentation for the ContentDisposition class.</summary>
    /// <param name='name'>The name of the parameter to get. The name will
    /// be matched using a basic case-insensitive comparison. (Two strings
    /// are equal in such a comparison, if they match after converting the
    /// basic upper-case letters A to Z (U+0041 to U+005A) in both strings
    /// to basic lower-case letters.). Can't be null.</param>
    /// <returns>The value of the parameter, or null if the parameter does
    /// not exist.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='name'/> is empty.</exception>
    public string GetParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return this.parameters.ContainsKey(name) ? this.parameters[name] :
        null;
    }

    private static ContentDisposition ParseDisposition(string str) {
      const bool HttpRules = false;
      var index = 0;
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      int endIndex = str.Length;
      var parameters = new Dictionary<string, string>();
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      int i = MediaType.SkipMimeToken(
        str,
        index,
        endIndex,
        null,
        HttpRules);
      if (i == index) {
        return null;
      }
      string dispoType =
        DataUtilities.ToLowerCaseAscii(str.Substring(index, i - index));
      if (i < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return new ContentDisposition(
            dispoType,
            parameters);
        }
        if (i3 < endIndex && str[i3] != ';') {
          // not followed by ";", so not a content disposition
          return null;
        }
      }
      index = i;
      return MediaType.ParseParameters(
        str,
        index,
        endIndex,
        HttpRules,
        parameters) ? new ContentDisposition (
          dispoType,
          parameters) : null;
    }

    private static ContentDisposition Build(string name) {
      return new ContentDisposition (
          name,
          new Dictionary<string, string>());
    }

    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This instance is immutable")]
    #endif

    /// <summary>The content disposition value "attachment" .</summary>
    public static readonly ContentDisposition Attachment =
      Build("attachment");

    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "This instance is immutable")]
    #endif

    /// <summary>The content disposition value "inline" .</summary>
    public static readonly ContentDisposition Inline =
      Build("inline");

    /// <summary>Creates a new content disposition object from the value of
    /// a Content-Disposition header field.</summary>
    /// <param name='dispoValue'>The parameter <paramref
    /// name='dispoValue'/> is a text string.</param>
    /// <returns>A content disposition object, or
    /// ContentDisposition.Attachment" if <paramref name='dispoValue'/> is
    /// empty or syntactically invalid.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dispoValue'/> is null.</exception>
    public static ContentDisposition Parse(string dispoValue) {
      if (dispoValue == null) {
        throw new ArgumentNullException(nameof(dispoValue));
      }
      return Parse(dispoValue, Attachment);
    }

    /// <summary>Parses a content disposition string and returns a content
    /// disposition object, or the default value if the string is invalid.
    /// This method checks the syntactic validity of the string, but not
    /// whether it has all parameters it's required to have or whether the
    /// parameters themselves are set to valid values for the parameter.
    /// <para>This method assumes the given content disposition string was
    /// directly extracted from the Content-Disposition header field (as
    /// defined for email messages) and follows the syntax given in RFC
    /// 2183. Accordingly, among other things, the content disposition
    /// string can contain comments (delimited by parentheses).</para>
    /// <para>RFC 2231 extensions allow each content disposition parameter
    /// to be associated with a character encoding and/or language, and
    /// support parameter values that span two or more key-value pairs.
    /// Parameters making use of RFC 2231 extensions have names with an
    /// asterisk ("*"). Such a parameter will be ignored if it is
    /// ill-formed because of RFC 2231's rules (except for illegal
    /// percent-decoding or undecodable sequences for the given character
    /// encoding). Examples of RFC 2231 extensions follow (both examples
    /// encode the same "filename" parameter):</para>
    /// <para><b>inline; filename*=utf-8'en'filename.txt</b></para>
    /// <para><b>inline; filename*0*=utf-8'en'file;
    /// filename*1*=name%2Etxt</b></para>
    /// <para>This implementation ignores keys (in parameter key-value
    /// pairs) that appear more than once in the content disposition.
    /// Nothing in RFCs 2045, 2183, 2231, 6266, or 7231 explicitly
    /// disallows such keys, or otherwise specifies error-handling behavior
    /// for such keys.</para></summary>
    /// <param name='dispositionValue'>A text string that should be the
    /// value of a Content-Disposition header field.</param>
    /// <param name='defaultValue'>The value to return in case the
    /// disposition value is syntactically invalid. Can be null.</param>
    /// <returns>A ContentDisposition object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dispositionValue'/> is null.</exception>
    public static ContentDisposition Parse(
      string dispositionValue,
      ContentDisposition defaultValue) {
      if (dispositionValue == null) {
        throw new ArgumentNullException(nameof(dispositionValue));
      }
      ContentDisposition dispo = ParseDisposition(dispositionValue);
      return dispo ?? defaultValue;
    }
  }
}
