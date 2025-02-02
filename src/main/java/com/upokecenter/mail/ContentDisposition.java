package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

  /**
   * <p>Specifies how a message body should be displayed or handled by a mail
   * user agent. This type is immutable; its contents can't be changed after it's
   * created. To create a changeable disposition object, use the
   * ContentDisposition.Builder class.</p> <p><b>About the "filename"
   * parameter</b></p> <p>The "filename" parameter of a content disposition
   * suggests a name to use when saving data to a file. For the "filename"
   * parameter, the GetParameter method and Parameters property({@code
   * getParameters}) method in Java) do not adapt that parameter's value using
   * the ContentDisposition.MakeFilename method. Thus, for example, the
   * "filename" parameter, if any, returned by this method could have an
   * arbitrary length, be encoded using RFC 2047 encoded words (which some email
   * and HTTP implementations still like to write out in headers, even though
   * that RFC says encoded words "MUST NOT appear within a 'quoted-string'"; see
   * ContentDisposition.MakeFilename), or not be usable as is as a file name.</p>
   * <p><b>Example:</b> An example of RFC 2047 encoded words is:</p>
   * <p><b>=?UTF-8?Q?test?=</b></p> <p>Content-Disposition header fields like the
   * following have appeared in practice:</p> <p><b>Content-Disposition:
   * attachment; filename==?UTF-8?Q?example?=</b></p> <p><b>Content-Disposition:
   * attachment; filename==?UTF-8?Q?test.png?=</b></p> <p><b>Content-Disposition:
   * attachment; filename="=?UTF-8?Q?test.png?="</b></p> <p>In this
   * implementation, the first and second of these are syntactically invalid, so
   * they trigger parse errors, while the third of these is syntactically valid,
   * but the "filename" parameter is treated as "=?UTF-8?Q?test.png?=", not
   * "test.png" or something else -- RFC 2047 encoded words are not decoded at
   * the moment a content disposition is parsed (using the Parse method).</p>
   */
  public class ContentDisposition {
    private final String dispositionType;

    /**
     * Gets a string containing this object's disposition type, such as "inline" or
     * "attachment". Note that under RFC 6266 sec. 4.2 and RFC 2183 sec. 2.8,
     * unrecognized disposition types should be treated as "attachment". (There is
     * no default content disposition in a message has no Content-Disposition
     * header field.). The resulting string will be in lowercase; that is, with its
     * basic uppercase letters ("A" to "Z") converted to basic lowercase letters
     * ("a" to "z").
     * @return A string containing this object's disposition type, such as "inline"
     * or "attachment".
     */
    public final String getDispositionType() {
        return this.dispositionType;
      }

    /**
     * Determines whether this object and another object are equal.
     * @param obj The parameter {@code obj} is an arbitrary object.
     * @return {@code true} if the objects are equal; otherwise, {@code false}. In
     * this method, two objects are not equal if they don't have the same type or
     * if one is null and the other isn't.
     */
    @Override public boolean equals(Object obj) {
      ContentDisposition other = ((obj instanceof ContentDisposition) ? (ContentDisposition)obj : null);
      if (other == null) {
        return false;
      }
      return this.dispositionType.equals(other.dispositionType) &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /**
     * Calculates the hash code of this object. The exact algorithm used by this
     * method may change between versions of this library, and no application or
     * process IDs are used in the hash code calculation.
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
        return this.dispositionType.equals("inline");
      }

    /**
     * Gets a value indicating whether the disposition type is attachment.
     * @return {@code true} If the disposition type is attachment; otherwise,
     * {@code false}.
     */
    public final boolean isAttachment() {
        return this.dispositionType.equals("attachment");
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
     * <p>Gets a list of parameter names associated with this object and their
     * values. Each parameter name will be in lowercase; that is, with its basic
     * uppercase letters ("A" to "Z") converted to basic lowercase letters ("a" to
     * "z"). </p> <p>For the "filename" parameter, the value of that parameter is
     * not adapted with the ContentDisposition.MakeFilename method; see the
     * documentation for the ContentDisposition class.</p>
     * @return A read-only list of parameter names associated with this object and
     * their values. NOTE: Previous versions erroneously stated that the list will
     * be sorted by name. In fact, the names will not be guaranteed to appear in
     * any particular order; this is at least the case in version 0.10.0.
     */
    public final Map<String, String> getParameters() {
        return java.util.Collections.unmodifiableMap(this.parameters);
      }

    /**
     * Converts this content disposition to a text string form suitable for
     * inserting in email headers. Notably, the string contains the value of a
     * Content-Disposition header field (without the text necessarily starting with
     * "Content-Disposition" followed by a space), and consists of one or more
     * lines.
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
     * inserting in HTTP headers. Notably, the string contains the value of a
     * Content-Disposition header field (without the text necessarily starting with
     * "Content-Disposition" followed by a space), and consists of a single line.
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
     * <p>Converts a file name from the Content-disposition header field (or
     * another string representing a title and an optional file extension) to a
     * suitable name for saving data to a file. This method is idempotent; that is,
     * calling the method again on the result doesn't change that result. The
     * method avoids characters and combinations of characters that are problematic
     * to use in certain file systems, and leaves the vast majority of file names
     * seen in practice untouched. </p><p>Examples of how this method works
     * follows:</p> <p>{@code "=?utf-8?q?hello=2Etxt?=" -&gt;"hello.txt"} (RFC 2047
     * encoding).</p> <p>{@code "=?utf-8?q?long_filename?=" -&gt;"long filename"}
     * (RFC 2047 encoding).</p> <p>{@code "utf-8'en'hello%2Etxt" -&gt;"hello.txt"}
     * (RFC 2231 encoding).</p> <p>{@code "nul.txt" -&gt;"_nul.txt"} (Reserved
     * name).</p> <p>{@code "dir1/dir2/file" -&gt;"dir1_dir2_file"} (Directory
     * separators).</p><p><b>Remarks:</b></p> <ul> <li>This method should be used
     * only to prepare a file name for the purpose of suggesting a name by which to
     * save data. It should not be used to prepare file names of existing files for
     * the purpose of reading them, since this method may replace certain
     * characters with other characters in some cases, such that two different
     * inputs may map to the same output.</li><li><b>File Name Support.</b> For
     * recommendations on file name support, see " <a
     * href="https://peteroupc.github.io/filenames.html">File Name Support in
     * Applications</a> ".</li><li><b>Guarantees.</b> The exact file name
     * conversion used by this method is not guaranteed to remain the same between
     * versions of this library, with the exception that the return value will be
     * in normalization form C, will not contain base + slash code points, will not
     * be null, and will be an empty string only if the parameter str is null or
     * empty.</li><li> <p><b>'Name' and 'Filename' Parameters.</b> Email and HTTP
     * headers may specify suggested filenames using the Content-Disposition header
     * field's {@code filename} parameter or, in practice, the Content-Type header
     * field's {@code name} parameter.</p> <p>Although RFC 2047 encoded words
     * appearing in both parameters are written out by some implementations, this
     * practice is often discouraged (especially since the RFC itself says that
     * encoded words "MUST NOT appear within a 'quoted-string'"). Nevertheless, the
     * MakeFilename method has a basis in the RFCs to decode RFC 2047 encoded words
     * (and RFC 2231 encoding) in file names passed to this method.</p> <p>RFC 2046
     * sec. 4.5.1 ({@code application/octet-stream} subtype in Content-Type header
     * field) cites an earlier RFC 1341, which "defined the use of a 'NAME'
     * parameter which gave a <i>suggested</i> file name to be used if the data
     * were written to a file". Also, RFC 2183 sec. 2.3 ({@code filename}
     * parameter in Content-Disposition) confirms that the " <i>suggested</i>
     * filename" in the {@code filename} parameter "should be <i>used as a
     * basis</i> for the actual filename, where possible", and that that file name
     * should "not.get(be) blindly use.get(d)". See also RFC 6266, section 4.3,
     * which discusses the use of that parameter in Hypertext Transfer Protocol
     * (HTTP).</p> <p>To the extent that the "name" parameter is not allowed in
     * message bodies other than those with the media type
     * "application/octet-stream" or treated as that media type, this is a
     * deviation of RFC 2045 and 2046 (see also RFC 2045 sec. 5, which says that
     * "[t]here are NO globally meaningful parameters that apply to all media
     * types"). (Some email implementations may still write out the "name"
     * parameter, even for media types other than {@code application/octet-stream}
     * and even though RFC 2046 has deprecated that parameter.)</p></li></ul>.
     * @param str A string representing a file name. Can be null.
     * @return A string with the converted version of the file name. Among other
     * things, encoded words under RFC 2047 are decoded (since they occur so
     * frequently in Content-Disposition filenames); the value is decoded under RFC
     * 2231 if possible; characters unsuitable for use in a filename (including the
     * directory separators slash and backslash) are replaced with underscores;
     * spaces and tabs are collapsed to a single space; leading and trailing spaces
     * and tabs are removed; and the filename is truncated if it would otherwise be
     * too long. Also, for reasons stated in the remarks, a character that is the
     * combined form of a base character and a combining slash is replaced with "!"
     * followed by the base character. The returned string will be in normalization
     * form C. Returns the empty string if {@code str} is null or empty.
     */
    public static String MakeFilename(String str) {
      return MakeFilenameMethod.MakeFilename(str);
    }

    /**
     * Gets an adapted version of the "filename" parameter in this content
     * disposition object by using the "MakeFilename" method.
     * @return The adapted file name in the form of a string. Returns the empty
     * string if there is no "filename" parameter or that parameter is empty.
     */
    public String GetFilename() {
      return MakeFilename(this.GetParameter("filename"));
    }

    /**
     * Gets the date and time extracted from this content disposition's
     * "creation-date" parameter, which specifies the date of creation of a file
     * (RFC 2183 sec. 2.4). The parameter is parsed as though by {@code
     * MailDateTime.ParseDateString} with obsolete time zones (including "GMT")
     * allowed. See that method's documentation for information on the format of
     * this method's return value.
     * @return The extracted date and time as an 8-element array, or {@code null}
     * if the "creation-date" parameter doesn't exist, is an empty string, or is
     * syntactically invalid, or if the parameter's year would overflow a 32-bit
     * signed integer.
     */
    public int[] GetCreationDate() {
      return MailDateTime.ParseDateString(
          this.GetParameter("creation-date"),
          true);
    }

    /**
     * Gets the date and time extracted from this content disposition's
     * "modification-date" parameter, which specifies the date of last modification
     * of a file (RFC 2183 sec. 2.5). The parameter is parsed as though by {@code
     * MailDateTime.ParseDateString} with obsolete time zones (including "GMT")
     * allowed. See that method's documentation for information on the format of
     * this method's return value.
     * @return The extracted date and time as an 8-element array, or {@code null}
     * if the "modification-date" parameter doesn't exist, is an empty string, or
     * is syntactically invalid, or if the parameter's year would overflow a 32-bit
     * signed integer.
     */
    public int[] GetModificationDate() {
      return MailDateTime.ParseDateString(
          this.GetParameter("modification-date"),
          true);
    }

    /**
     * Gets the date and time extracted from this content disposition's "read-date"
     * parameter, which specifies the date at which a file was last read (RFC 2183
     * sec. 2.6). The parameter is parsed as though by {@code
     * MailDateTime.ParseDateString} with obsolete time zones (including "GMT")
     * allowed. See that method's documentation for information on the format of
     * this method's return value.
     * @return The extracted date and time as an 8-element array, or {@code null}
     * if the "read-date" parameter doesn't exist, is an empty string, or is
     * syntactically invalid, or if the parameter's year would overflow a 32-bit
     * signed integer.
     */
    public int[] GetReadDate() {
      return MailDateTime.ParseDateString(
          this.GetParameter("read-date"),
          true);
    }

    /**
     * Gets a parameter from this disposition object. For the "filename" parameter,
     * the value of that parameter is not adapted with the
     * ContentDisposition.MakeFilename method; see the documentation for the
     * ContentDisposition class.
     * @param name The name of the parameter to get. The name will be matched using
     * a basic case-insensitive comparison. (Two strings are equal in such a
     * comparison, if they match after converting the basic uppercase letters A to
     * Z (U+0041 to U+005A) in both strings to basic lowercase letters.). Can't be
     * null.
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
      name = com.upokecenter.util.DataUtilities.ToLowerCaseAscii(name);
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
      String dispoType = com.upokecenter.util.DataUtilities.ToLowerCaseAscii(str.substring(index, (index)+(i - index)));
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
     * <p>Parses a content disposition string and returns a content disposition
     * object, or the default value if the string is invalid. This method checks
     * the syntactic validity of the string, but not whether it has all parameters
     * it's required to have or whether the parameters themselves are set to valid
     * values for the parameter. </p><p>This method assumes the specified content
     * disposition string was directly extracted from the Content-Disposition
     * header field (as defined for email messages) and follows the syntax given in
     * RFC 2183. Accordingly, among other things, the content disposition string
     * can contain comments (delimited by parentheses).</p> <p>RFC 2231 extensions
     * allow each content disposition parameter to be associated with a character
     * encoding and/or language, and support parameter values that span two or more
     * key-value pairs. Parameters making use of RFC 2231 extensions have names
     * with an asterisk ("*"). Such a parameter will be ignored if it is ill-formed
     * because of RFC 2231's rules (except for illegal percent-decoding or
     * undecodable sequences for the specified character encoding). Examples of RFC
     * 2231 extensions follow (both examples encode the same "filename"
     * parameter):</p> <p><b>inline; filename*=utf-8'en'filename.txt</b></p>
     * <p><b>inline; filename*0*=utf-8'en'file; filename*1*=name%2Etxt</b></p>
     * <p>This implementation ignores keys (in parameter key-value pairs) that
     * appear more than once in the content disposition. Nothing in RFCs 2045,
     * 2183, 2231, 6266, or 7231 explicitly disallows such keys, or otherwise
     * specifies error-handling behavior for such keys.</p>
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

    /**
     * A mutable data type that allows a content disposition to be built.
     */
    public static final class Builder {
      private final Map<String, String> parameters;
      private String type;

      /**
       * Gets this value's disposition type, such as "inline" or "attachment".
       * @return This value's disposition type, such as "inline" or "attachment" .
       * @throws NullPointerException The property is being set and the value is
       * null.
       * @throws IllegalArgumentException The property is being set and the value is an
       * empty string.
       */
      public final String getDispositionType() {
          return this.type;
        }
public final void setDispositionType(String value) {
          this.SetDispositionType(value);
        }

      /**
       * Initializes a new instance of the {@link
       * com.upokecenter.mail.ContentDisposition.Builder} class using the disposition
       * type "attachment" .
       */
      public Builder() {
        this.parameters = new HashMap<String, String>();
        this.type = "attachment";
      }

      /**
       * Initializes a new instance of the {@link
       * com.upokecenter.mail.ContentDisposition.Builder} class using the data from
       * the specified content disposition.
       * @param mt The parameter {@code mt} is a ContentDisposition object.
       * @throws NullPointerException The parameter {@code mt} is null.
       */
      public Builder(ContentDisposition mt) {
        if (mt == null) {
          throw new NullPointerException("mt");
        }
        this.parameters = new HashMap<String, String>(mt.getParameters());
        this.type = mt.getDispositionType();
      }

      /**
       * Initializes a new instance of the {@link
       * com.upokecenter.mail.ContentDisposition.Builder} class using the specified
       * disposition type.
       * @param type The parameter {@code type} is a text string.
       * @throws NullPointerException The parameter {@code type} is null.
       * @throws IllegalArgumentException Type is empty.
       */
      public Builder(String type) {
        if (type == null) {
          throw new NullPointerException("type");
        }
        if (type.length() == 0) {
          throw new IllegalArgumentException("type is empty.");
        }
        this.parameters = new HashMap<String, String>();
        this.SetDispositionType(type);
      }

      /**
       * Converts this object to an immutable ContentDisposition object.
       * @return A MediaType object.
       */
      public ContentDisposition ToDisposition() {
        return new ContentDisposition(this.type, this.parameters);
      }

      /**
       * Sets the disposition type, such as "inline". This method enables the pattern
       * of method chaining (for example, {@code new ...().getSet()...().getSet()...()})
       * unlike with the DispositionType property in .NET or the setDispositionType
       * method (with small s) in Java.
       * @param str The parameter {@code str} is a text string.
       * @return This instance.
       * @throws NullPointerException The parameter {@code str} is null.
       * @throws IllegalArgumentException Str is empty.
       */
      public Builder SetDispositionType(String str) {
        if (str == null) {
          throw new NullPointerException("str");
        }
        if (str.length() == 0) {
          throw new IllegalArgumentException("str is empty.");
        }
        if (MediaType.SkipMimeTypeSubtype(str, 0, str.length(), null) !=
          str.length()) {
          throw new IllegalArgumentException("Not a well-formed type: " + str);
        }
        this.type = com.upokecenter.util.DataUtilities.ToLowerCaseAscii(str);
        return this;
      }

      /**
       * Removes a parameter from this content disposition. Does nothing if the
       * parameter's name doesn't exist.
       * @param name The parameter to remove. The name is compared using a basic
       * case-insensitive comparison. (Two strings are equal in such a comparison, if
       * they match after converting the basic uppercase letters A to Z (U+0041 to
       * U+005A) in both strings to basic lowercase letters.).
       * @return This instance.
       * @throws NullPointerException The parameter {@code name} is null.
       */
      public Builder RemoveParameter(String name) {
        if (name == null) {
          throw new NullPointerException("name");
        }
        this.parameters.remove(com.upokecenter.util.DataUtilities.ToLowerCaseAscii(name));
        return this;
      }

      /**
       * Sets a parameter of this content disposition.
       * @param name Name of the parameter to set. If this name already exists
       * (compared using a basic case-insensitive comparison), it will be
       * overwritten. (Two strings are equal in a basic case-insensitive comparison,
       * if they match after converting the basic uppercase letters A to Z (U+0041 to
       * U+005A) in both strings to basic lowercase letters.).
       * @param value Value of the parameter to set.
       * @return This instance.
       * @throws NullPointerException Either {@code value} or {@code name} is null.
       * @throws IllegalArgumentException The parameter {@code name} is empty, or it isn't a
       * well-formed parameter name.
       */
      public Builder SetParameter(String name, String value) {
        if (value == null) {
          throw new NullPointerException("value");
        }
        if (name == null) {
          throw new NullPointerException("name");
        }
        if (name.length() == 0) {
          throw new IllegalArgumentException("name is empty.");
        }
        if (MediaType.SkipMimeTypeSubtype(name, 0, name.length(), null) !=
          name.length()) {
          throw new IllegalArgumentException("Not a well-formed parameter name: " +
            name);
        }
        this.parameters.put(com.upokecenter.util.DataUtilities.ToLowerCaseAscii(name), value);
        return this;
      }

      /**
       * Converts this object to a text string.
       * @return A string representation of this object.
       */
      @Override public String toString() {
        return this.ToDisposition().toString();
      }
    }
  }
