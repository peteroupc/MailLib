package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

    /**
     * <p>Specifies how a message body should be displayed or handled by a mail
     * user agent. This type is immutable; its contents can't be changed
     * after it's created. To create a changeable disposition object, use
     *  the DispositionBuilder class.</p> <p><b>About the "filename"
     *  parameter</b></p> <p>The "filename" parameter of a content
     * disposition suggests a name to use when saving data to a file. For
     *  the "filename" parameter, the GetParameter method and Parameters
     * property (<code>getParameters</code>) method in Java) do not adapt that
     * parameter's value using the ContentDisposition.MakeFilename method.
     *  Thus, for example, the "filename" parameter, if any, returned by
     * this method could have an arbitrary length, be encoded using RFC
     * 2047 encoded words (which some email and HTTP implementations still
     * like to write out in headers, even though that RFC says encoded
     *  words "MUST NOT appear within a 'quoted-string'"; see
     * ContentDisposition.MakeFilename), or not be usable as is as a file
     * name.</p> <p><b>Example:</b> An example of RFC 2047 encoded words
     * is:</p> <p><b>=?UTF-8?Q?test?=</b></p> <p>Content-Disposition header
     * fields like the following have appeared in practice:</p>
     * <p><b>Content-Disposition: attachment;
     * filename==?UTF-8?Q?example?=</b></p> <p><b>Content-Disposition:
     * attachment; filename==?UTF-8?Q?test.png?=</b></p>
     * <p><b>Content-Disposition: attachment;
     *  filename="=?UTF-8?Q?test.png?="</b></p> <p>In this implementation,
     * the first and second of these are syntactically invalid, so they
     * trigger parse errors, while the third of these is syntactically
     *  valid, but the "filename" parameter is treated as
     *  "=?UTF-8?Q?test.png?=", not "test.png" or something else -- RFC 2047
     * encoded words are not decoded at the moment a content disposition is
     * parsed (using the Parse method).</p>
     */
  public class ContentDisposition {
    private final String dispositionType;

    /**
     * Gets a string containing this object's disposition type, such as "inline" or
     *  "attachment". Note that under RFC 6266 sec. 4.2 and RFC 2183 sec.
     * 2.8, unrecognized disposition types should be treated as
     *  "attachment".
     * @return A string containing this object's disposition type, such as "inline"
     *  or "attachment".
     */
    public final String getDispositionType() {
        return this.dispositionType;
      }

    /**
     * Determines whether this object and another object are equal.
     * @param obj The parameter {@code obj} is an arbitrary object.
     * @return {@code true} if the objects are equal; otherwise, {@code false}.
     */
    @Override public boolean equals(Object obj) {
      ContentDisposition other = ((obj instanceof ContentDisposition) ? (ContentDisposition)obj : null);
      if (other == null) {
        return false;
      }
      return this.dispositionType.startsWith(other.dispositionType) &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /**
     * Calculates the hash code of this object. No application or process IDs are
     * used in the hash code calculation.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 632580499;
      if (this.dispositionType != null) {
        for (int i = 0; i < this.dispositionType.length(); ++i) {
 valueHashCode = (valueHashCode + (632580503 *
          this.dispositionType.charAt(i)));
 }
      }
      if (this.parameters != null) {
        valueHashCode = (valueHashCode + (632580587 *
                this.parameters.size()));
      }
      return valueHashCode;
    }

    /**
     * Gets a value indicating whether the disposition type is inline.
     * @return {@code true} If the disposition type is inline; otherwise, {@code
     * false}.
     */
    public final boolean isInline() {
        return this.dispositionType.startsWith("inline");
      }

    /**
     * Gets a value indicating whether the disposition type is attachment.
     * @return {@code true} If the disposition type is attachment; otherwise,
     * {@code false}.
     */
    public final boolean isAttachment() {
        return this.dispositionType.startsWith("attachment");
      }

    ContentDisposition(
 String type,
 Map<String, String> parameters) {
      if (type == null) {
        throw new NullPointerException("type");
      }
      this.dispositionType = type;
      this.parameters = new HashMap<String, String>(parameters);
    }

    private ContentDisposition() {
      this.dispositionType = "";
      this.parameters = new HashMap<String, String>();
    }

    private final Map<String, String> parameters;

    /**
     * Gets a list of parameter names associated with this object and their values.
     *  <p>For the "filename" parameter, the value of that parameter is not
     * adapted with the ContentDisposition.MakeFilename method; see the
     * documentation for the ContentDisposition class.</p>
     * @return A read-only list of parameter names associated with this object and
     * their values. NOTE: Previous versions erroneously stated that the
     * list will be sorted by name. In fact, the names will not be
     * guaranteed to appear in any particular order; this is at least the
     * case in version 0.10.0.
     */
    public final Map<String, String> getParameters() {
        return java.util.Collections.unmodifiableMap(this.parameters);
      }

    /**
     * Converts this content disposition to a text string form suitable for
     * inserting in email headers. Notably, the string contains the value
     * of a Content-Disposition header field (without the text necessarily
     *  starting with "Content-Disposition" followed by a space), and
     * consists of one or more lines.
     * @return A text string form of this content disposition.
     */
    @Override public String toString() {
      // NOTE: 21 is the length of "Content-Disposition: " (with
      // trailing space).
      HeaderEncoder sa = new HeaderEncoder(Message.MaxRecHeaderLineLength, 21);
      sa.AppendSymbol(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sa);
      return sa.toString();
    }

    /**
     * Converts this content disposition to a text string form suitable for
     * inserting in HTTP headers. Notably, the string contains the value of
     * a Content-Disposition header field (without the text necessarily
     *  starting with "Content-Disposition" followed by a space), and
     * consists of a single line.
     * @return A text string form of this content disposition.
     */
    public String ToSingleLineString() {
      // NOTE: 21 is the length of "Content-Disposition: " (with trailing
      // space).
      HeaderEncoder sa = new HeaderEncoder(-1, 21);
      sa.AppendSymbol(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sa);
      return sa.toString();
    }

    /**
     * Converts a file name from the Content-Disposition header to a suitable name
     * for saving data to a file. This method is idempotent; that is,
     * calling the method again on the result doesn't change that result.
     *  <p>Examples:</p> <p><code>"=?utf-8?q?hello=2Etxt?="
     *  -&gt;"hello.txt"</code> (RFC 2047 encoding).</p>
     *  <p><code>"=?utf-8?q?long_filename?=" -&gt;"long filename"</code> (RFC 2047
     *  encoding).</p> <p><code>"utf-8'en'hello%2Etxt" -&gt;"hello.txt"</code>
     *  (RFC 2231 encoding).</p> <p><code>"nul.txt" -&gt;"_nul.txt"</code>
     *  (Reserved name).</p> <p><code>"dir1/dir2/file"
     *  -&gt;"dir1_dir2_file"</code> (Directory separators).</p><p>
     * <p><b>Remarks:</b></p> <ul> <li>The exact file name conversion used
     * by this method is not guaranteed to remain the same between versions
     * of this library, with the exception that the return value will be in
     * normalization form C, will not contain base + slash code points,
     * will not be null, and will be an empty string only if <paramref
     * name='str'/> is null or empty.</li> <li>The string returned by this
     * method is normalized using Unicode normalization form C (NFC) (see
     * the {@link com.upokecenter.text.NormalizerInput} class for details).
     * Although most file systems preserve the normalization of file names,
     * there is one notable exception: The HFS Plus file system (on macOS
     * before High Sierra) stores file names using a modified version of
     * normalization form D (NFD) in which certain code points are not
     * decomposed, including all base + slash code points, which are the
     * only composed code points in Unicode that are decomposed in NFD but
     * not in HFS Plus's version of NFD. If the filename will be used to
     * save a file to an HFS Plus storage device, it is enough to normalize
     * the return value with NFD for this purpose (because all base + slash
     * code points were converted beforehand by MakeFilename to an
     *  alternate form). See also Apple's Technical Q&amp;A "Text Encodings
     *  in VFS" and Technical Note TN1150, "HFS Plus Volume Format".</li>
     * <li> <p>Email and HTTP headers may specify suggested filenames using
     * the Content-Disposition header field's <code>filename</code> parameter or,
     * in practice, the Content-Type header field's <code>name</code>
     * parameter.</p> <p>Although RFC 2047 encoded words appearing in both
     * parameters are written out by some implementations, this practice is
     * often discouraged (especially since the RFC itself says that encoded
     *  words "MUST NOT appear within a 'quoted-string'"). Nevertheless, the
     * MakeFilename method has a basis in the RFCs to decode RFC 2047
     * encoded words (and RFC 2231 encoding) in file names passed to this
     * method.</p> <p>RFC 2046 sec. 4.5.1 (<code>application/octet-stream</code>
     * subtype in Content-Type header field) cites an earlier RFC 1341,
     *  which "defined the use of a 'NAME' parameter which gave a
     * <i>suggested</i> file name to be used if the data were written to a
     *  file". Also, RFC 2183 sec. 2.3 (<code>filename</code> parameter in
     *  Content-Disposition) confirms that the " <i>suggested</i> filename"
     *  in the <code>filename</code> parameter "should be <i>used as a basis</i>
     *  for the actual filename, where possible", and that that file name
     *  should "not.get(be) blindly use.get(d)". See also RFC 6266, section
     * 4.3, which discusses the use of that parameter in Hypertext Transfer
     *  Protocol (HTTP).</p> <p>To the extent that the "name" parameter is
     * not allowed in message bodies other than those with the media type
     *  "application/octet-stream" or treated as that media-type, this is a
     * deviation of RFC 2045 and 2046 (see also RFC 2045 sec. 5, which says
     *  that "[t]here are NO globally meaningful parameters that apply to
     *  all media types"). (Some email implementations may still write out
     *  the "name" parameter, even for media types other than
     * <code>application/octet-stream</code> and even though RFC 2046 has
     * deprecated that parameter.)</p></li></ul>.</p>
     * @param str A string representing a file name. Can be null.
     * @return A string with the converted version of the file name. Among other
     * things, encoded words under RFC 2047 are decoded (since they occur
     * so frequently in Content-Disposition filenames); the value is
     * decoded under RFC 2231 if possible; characters unsuitable for use in
     * a filename (including the directory separators slash and backslash)
     * are replaced with underscores; spaces and tabs are collapsed to a
     * single space; leading and trailing spaces and tabs are removed; and
     * the filename is truncated if it would otherwise be too long. Also,
     * for reasons stated in the remarks, a character that is the combined
     *  form of a base character and a combining slash is replaced with "!"
     * followed by the base character. The returned string will be in
     * normalization form C. Returns the empty string if {@code str} is
     * null or empty.
     */
    public static String MakeFilename(String str) {
      return MakeFilenameMethod.MakeFilename(str);
    }

    /**
     * Gets an adapted version of the "filename" parameter in this content
     *  disposition object by using the "MakeFilename" method.
     * @return The adapted file name in the form of a string. Returns the empty
     *  string if there is no "filename" parameter or that parameter is
     * empty.
     */
    public String GetFilename() {
      return MakeFilename(this.GetParameter("filename"));
    }

    /**
     * Gets the date and time extracted from this content disposition's
     *  "creation-date" parameter, which specifies the date of creation of a
     * file (RFC 2183 sec. 2.4). See <see
     * cref='PeterO.Mail.MailDateTime.ParseDateString(
     * System.String,System.Boolean)'/> for information on the format of
     * this method's return value.
     * @return The extracted date and time as an 8-element array, or {@code null}
     *  if the "creation-date" parameter doesn't exist, is an empty string,
     * or is syntactically invalid, or if the parameter's year would
     * overflow a 32-bit signed integer.
     */
    public int[] GetCreationDate() {
      return MailDateTime.ParseDateString(
        this.GetParameter("creation-date"));
    }

    /**
     * Gets the date and time extracted from this content disposition's
     *  "modification-date" parameter, which specifies the date of last
     * modification of a file (RFC 2183 sec. 2.5). See <see
     * cref='PeterO.Mail.MailDateTime.ParseDateString(
     * System.String,System.Boolean)'/> for information on the format of
     * this method's return value.
     * @return The extracted date and time as an 8-element array, or {@code null}
     *  if the "modification-date" parameter doesn't exist, is an empty
     * string, or is syntactically invalid, or if the parameter's year
     * would overflow a 32-bit signed integer.
     */
    public int[] GetModificationDate() {
      return MailDateTime.ParseDateString(
        this.GetParameter("modification-date"));
    }

    /**
     * Gets the date and time extracted from this content disposition's "read-date"
     * parameter, which specifies the date at which a file was last read
     * (RFC 2183 sec. 2.6). See <see
     * cref='PeterO.Mail.MailDateTime.ParseDateString(
     * System.String,System.Boolean)'/> for information on the format of
     * this method's return value.
     * @return The extracted date and time as an 8-element array, or {@code null}
     *  if the "read-date" parameter doesn't exist, is an empty string, or
     * is syntactically invalid, or if the parameter's year would overflow
     * a 32-bit signed integer.
     */
    public int[] GetReadDate() {
      return MailDateTime.ParseDateString(
        this.GetParameter("read-date"));
    }

    /**
     * Gets a parameter from this disposition object. For the "filename" parameter,
     * the value of that parameter is not adapted with the
     * ContentDisposition.MakeFilename method; see the documentation for
     * the ContentDisposition class.
     * @param name The name of the parameter to get. The name will be matched using
     * a basic case-insensitive comparison. (Two strings are equal in such
     * a comparison, if they match after converting the basic upper-case
     * letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).
     * Can't be null.
     * @return The value of the parameter, or null if the parameter does not exist.
     * @throws NullPointerException The parameter {@code name} is null.
     * @throws IllegalArgumentException The parameter {@code name} is empty.
     */
    public String GetParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (name.length() == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return this.parameters.containsKey(name) ? this.parameters.get(name) :
               null;
    }

    private static ContentDisposition ParseDisposition(String str) {
      boolean HttpRules = false;
      int index = 0;
      if (str == null) {
        throw new NullPointerException("str");
      }
      int endIndex = str.length();
      HashMap<String, String> parameters = new HashMap<String, String>();
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
      String dispoType =
        DataUtilities.ToLowerCaseAscii(str.substring(index, (index)+(i - index)));
      if (i < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return new ContentDisposition(
            dispoType,
            parameters);
        }
        if (i3 < endIndex && str.charAt(i3) != ';') {
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
        parameters) ? new ContentDisposition(
            dispoType,
            parameters) : null;
    }

    private static ContentDisposition Build(String name) {
      return new ContentDisposition(
        name,
        new HashMap<String, String>());
    }

    /**
     * The content disposition value "attachment" .
     */
    public static final ContentDisposition Attachment =
      Build("attachment");

    /**
     * The content disposition value "inline" .
     */
    public static final ContentDisposition Inline =
      Build("inline");

    /**
     * Creates a new content disposition object from the value of a
     * Content-Disposition header field.
     * @param dispoValue The parameter {@code dispoValue} is a text string.
     * @return A content disposition object, or ContentDisposition.Attachment" if
     * {@code dispoValue} is empty or syntactically invalid.
     * @throws NullPointerException The parameter {@code dispoValue} is null.
     */
    public static ContentDisposition Parse(String dispoValue) {
      if (dispoValue == null) {
        throw new NullPointerException("dispoValue");
      }
      return Parse(dispoValue, Attachment);
    }

    /**
     * Parses a content disposition string and returns a content disposition
     * object, or the default value if the string is invalid. This method
     * checks the syntactic validity of the string, but not whether it has
     * all parameters it's required to have or whether the parameters
     * themselves are set to valid values for the parameter. <p>This method
     * assumes the given content disposition string was directly extracted
     * from the Content-Disposition header field (as defined for email
     * messages) and follows the syntax given in RFC 2183. Accordingly,
     * among other things, the content disposition string can contain
     * comments (delimited by parentheses).</p> <p>RFC 2231 extensions
     * allow each content disposition parameter to be associated with a
     * character encoding and/or language, and support parameter values
     * that span two or more key-value pairs. Parameters making use of RFC
     *  2231 extensions have names with an asterisk ("*"). Such a parameter
     * will be ignored if it is ill-formed because of RFC 2231's rules
     * (except for illegal percent-decoding or undecodable sequences for
     * the given character encoding). Examples of RFC 2231 extensions
     *  follow (both examples encode the same "filename" parameter):</p>
     * <p><b>inline; filename*=utf-8'en'filename.txt</b></p> <p><b>inline;
     * filename*0*=utf-8'en'file; filename*1*=name%2Etxt</b></p> <p>This
     * implementation ignores keys (in parameter key-value pairs) that
     * appear more than once in the content disposition. Nothing in RFCs
     * 2045, 2183, 2231, 6266, or 7231 explicitly disallows such keys, or
     * otherwise specifies error-handling behavior for such keys.</p>
     * @param dispositionValue A text string that should be the value of a
     * Content-Disposition header field.
     * @param defaultValue The value to return in case the disposition value is
     * syntactically invalid. Can be null.
     * @return A ContentDisposition object.
     * @throws NullPointerException The parameter {@code dispositionValue} is
     * null.
     */
    public static ContentDisposition Parse(
      String dispositionValue,
      ContentDisposition defaultValue) {
      if (dispositionValue == null) {
        throw new NullPointerException("dispositionValue");
      }
      ContentDisposition dispo = ParseDisposition(dispositionValue);
      return (dispo == null) ? (defaultValue) : dispo;
    }
  }
