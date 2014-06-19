/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
    /// <summary>Specifies how a message body should be displayed or handled
    /// by a mail user agent. <para>This type is immutable; its contents can't
    /// be changed after it's created.</para>
    /// </summary>
  public class ContentDisposition
  {
    private string dispositionType;

    /// <summary>Gets a string containing this object's disposition type,
    /// such as "inline" or "attachment".</summary>
    /// <value>A string containing this object&apos;s disposition type,
    /// such as &quot;inline&quot; or &quot;attachment&quot;.</value>
    public string DispositionType {
      get {
        return this.dispositionType;
      }
    }

    #region Equals and GetHashCode implementation
    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object obj) {
      var other = obj as ContentDisposition;
      if (other == null) {
        return false;
      }
      return this.dispositionType.Equals(other.dispositionType) &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 632580499;
      unchecked {
        if (this.dispositionType != null) {
          hashCode += 632580503 * this.dispositionType.GetHashCode();
        }
        if (this.parameters != null) {
          hashCode += 632580587 * this.parameters.Count;
        }
      }
      return hashCode;
    }
    #endregion

    /// <summary>Gets a value indicating whether the disposition type is
    /// inline.</summary>
    /// <value>True if the disposition type is inline; otherwise, false..</value>
    public bool IsInline {
      get {
        return this.dispositionType.Equals("inline");
      }
    }

    /// <summary>Gets a value indicating whether the disposition type is
    /// attachment.</summary>
    /// <value>True if the disposition type is attachment; otherwise, false..</value>
    public bool IsAttachment {
      get {
        return this.dispositionType.Equals("attachment");
      }
    }

    internal ContentDisposition(string type, IDictionary<string, string> parameters) {
      this.dispositionType = type;
      this.parameters = new SortedMap<string, string>(parameters);
    }

    private SortedMap<string, string> parameters;

    /// <summary>Gets a list of parameter names associated with this object
    /// and their values.</summary>
    /// <value>A list of parameter names associated with this object and
    /// their values. The names will be sorted.</value>
    public IDictionary<string, string> Parameters {
      get {
        return new ReadOnlyMap<string, string>(this.parameters);
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      var sb = new StringBuilder();
      sb.Append(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sb);
      return sb.ToString();
    }

    private static string RemoveEncodedWordEnds(string str) {
      var sb = new StringBuilder();
      int index = 0;
      bool inEncodedWord = false;
      while (index < str.Length) {
        if (!inEncodedWord && index + 1 < str.Length && str[index] == '=' && str[index + 1] == '?') {
          // Remove start of encoded word
          inEncodedWord = true;
          index += 2;
          int qmarks = 0;
          // skip charset and encoding
          while (index < str.Length) {
            if (str[index] == '?') {
              ++qmarks;
              ++index;
              if (qmarks == 2) {
                break;
              }
            } else {
              ++index;
            }
          }
          inEncodedWord = true;
        } else if (inEncodedWord && index + 1 < str.Length && str[index] == '?' && str[index + 1] == '=') {
          // End of encoded word
          index += 2;
          inEncodedWord = false;
        } else {
          int c = DataUtilities.CodePointAt(str, index);
          if (c == 0xfffd) {
            sb.Append((char)0xfffd);
            ++index;
          } else {
            sb.Append(str[index++]);
            if (c >= 0x10000) {
              sb.Append(str[index++]);
            }
          }
        }
      }
      return sb.ToString();
    }

    /// <summary>Converts a filename from the Content-Disposition header
    /// to a suitable name for saving data to a file.</summary>
    /// <param name='str'>A string representing a file name.</param>
    /// <returns>A string with the converted version of the file name. Among
    /// other things, encoded words under RFC 2047 are decoded (since they
    /// occur so frequently in Content-Disposition filenames); the value
    /// is decoded under RFC 2231 if possible; characters unsuitable for
    /// use in a filename (including the directory separators slash and backslash)
    /// are replaced with underscores; and the filename is truncated if it
    /// would otherwise be too long. Returns an empty string if <paramref
    /// name='str'/> is null.</returns>
    public static string MakeFilename(string str) {
      if (str == null) {
        return String.Empty;
      }
      str = ParserUtility.TrimSpaceAndTab(str);
      if (str.IndexOf("=?", StringComparison.Ordinal) >= 0) {
        // May contain encoded words, which are very frequent
        // in Content-Disposition filenames (they would appear quoted
        // in the Content-Disposition "filename" parameter); these changes
        // appear justified in sec. 2.3 of RFC 2183, which says that
        // the parameter's value "should be used as a
        // basis for the actual filename, where possible."
        str = Rfc2047.DecodeEncodedWords(str, 0, str.Length, EncodedWordContext.Unstructured);
        if (str.IndexOf("=?", StringComparison.Ordinal) >= 0) {
          // Remove ends of encoded words that remain
          str = RemoveEncodedWordEnds(str);
        }
      } else if (str.IndexOf('\'') > 0) {
        // Check for RFC 2231 encoding, as long as the value before the
        // apostrophe is a recognized charset. It appears to be common,
        // too, to use quotes around a filename parameter AND use
        // RFC 2231 encoding, even though all the examples in that RFC
        // show unquoted use of this encoding.
        string charset = Charsets.ResolveAliasForEmail(str.Substring(0, str.IndexOf('\'')));
        if (!String.IsNullOrEmpty(charset)) {
          string newstr = MediaType.DecodeRfc2231Extension(str);
          if (!String.IsNullOrEmpty(newstr)) {
            // Value was decoded under RFC 2231
            str = newstr;
          }
        }
      }
      str = ParserUtility.TrimSpaceAndTab(str);
      // NOTE: Even if there are directory separators (backslash
      // and forward slash), the filename is not treated as a
      // file system path (in accordance with sec. 2.3 of RFC
      // 2183); as a result, the directory separators
      // will be treated as unsuitable characters for filenames
      // and are handled below.
      if (str.Length == 0) {
        return "_";
      }
      var builder = new StringBuilder();
      // Replace unsuitable characters for filenames
      // and make sure the filename's
      // length doesn't exceed 250
      for (int i = 0; i < str.Length && builder.Length < 250; ++i) {
        int c = DataUtilities.CodePointAt(str, i);
        if (c >= 0x10000) {
          ++i;
        }
        if (c == (int)'\t') {
          // Replace tab with space
          builder.Append(' ');
        } else if (c < 0x20 || c == '\\' || c == '/' || c == '*' || c == '?' || c == '|' ||
                   c == ':' || c == '<' || c == '>' || c == '"' || (c >= 0x7f && c <= 0x9F)) {
          // Unsuitable character for a filename (one of the characters reserved by Windows,
          // backslash, forward slash, ASCII controls, and C1 controls).
          builder.Append('_');
        } else {
          if (builder.Length < 249 || c < 0x10000) {
            if (c <= 0xffff) {
              builder.Append((char)c);
            } else if (c <= 0x10ffff) {
              builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              builder.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        }
      }
      str = builder.ToString();
      str = ParserUtility.TrimSpaceAndTab(str);
      if (str.Length == 0) {
        return "_";
      }
      if (str[str.Length - 1] == '.') {
        // Ends in a dot
        str += "_";
      }
      string strLower = DataUtilities.ToLowerCaseAscii(str);
      if (strLower.Equals("nul") ||
          strLower.IndexOf("nul.", StringComparison.Ordinal) == 0 ||
          strLower.Equals("prn") ||
          strLower.IndexOf("prn.", StringComparison.Ordinal) == 0 ||
          strLower.Equals("aux") ||
          strLower.IndexOf("aux.", StringComparison.Ordinal) == 0 ||
          strLower.Equals("con") ||
          strLower.IndexOf("con.", StringComparison.Ordinal) == 0 ||
          (strLower.Length >= 4 && strLower.IndexOf("lpt", StringComparison.Ordinal) == 0 && strLower[3] >= '1' && strLower[3] <= '9') ||
          (strLower.Length >= 4 && strLower.IndexOf("com", StringComparison.Ordinal) == 0 && strLower[3] >= '1' && strLower[3] <= '9')) {
        // Reserved filenames on Windows
        str = "_" + str;
      }
      if (str[0] == '~') {
        // Home folder convention
        str = "_" + str;
      }
      if (str[0] == '.') {
        // Starts with period; may be hidden in some configurations
        str = "_" + str;
      }
      return Normalizer.Normalize(str, Normalization.NFC);
    }

    /// <summary>Gets a parameter from this disposition object.</summary>
    /// <param name='name'>The name of the parameter to get. The name will
    /// be matched case-insensitively. Can&apos;t be null.</param>
    /// <returns>The value of the parameter, or null if the parameter does
    /// not exist.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='name'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='name'/> is empty.</exception>
    public string GetParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return this.parameters.ContainsKey(name) ? this.parameters[name] : null;
    }

    private bool ParseDisposition(string str) {
      const bool HttpRules = false;
      int index = 0;
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      int endIndex = str.Length;
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      int i = MediaType.SkipMimeToken(str, index, endIndex, null, HttpRules);
      if (i == index) {
        return false;
      }
      this.dispositionType = DataUtilities.ToLowerCaseAscii(str.Substring(index, i - index));
      if (i < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return true;
        }
        if (i3 < endIndex && str[i3] != ';') {
          // not followed by ";", so not a content disposition
          return false;
        }
      }
      index = i;
      return MediaType.ParseParameters(str, index, endIndex, HttpRules, this.parameters);
    }

    private static ContentDisposition Build(string name) {
      var dispo = new ContentDisposition();
      dispo.parameters = new SortedMap<string, string>();
      dispo.dispositionType = name;
      return dispo;
    }

    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
    #endif
    /// <summary>The content disposition value "attachment".</summary>
    public static readonly ContentDisposition Attachment = Build("attachment");

    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
    #endif
    /// <summary>The content disposition value "inline".</summary>
    public static readonly ContentDisposition Inline = Build("inline");

    private ContentDisposition() {
    }

    /// <summary>Parses a content disposition string and returns a content
    /// disposition object.</summary>
    /// <param name='dispoValue'>A string object.</param>
    /// <returns>A content disposition object, or "Attachment" if <paramref
    /// name='dispoValue'/> is empty or syntactically invalid.</returns>
    public static ContentDisposition Parse(string dispoValue) {
      return Parse(dispoValue, Attachment);
    }

    /// <summary>Creates a new content disposition object from the value
    /// of a Content-Disposition header field.</summary>
    /// <param name='dispositionValue'>A string object that should be
    /// the value of a Content-Disposition header field.</param>
    /// <param name='defaultValue'>The value to return in case the disposition
    /// value is syntactically invalid. Can be null.</param>
    /// <returns>A ContentDisposition object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='dispositionValue'/> is null.</exception>
    public static ContentDisposition Parse(string dispositionValue, ContentDisposition defaultValue) {
      if (dispositionValue == null) {
        throw new ArgumentNullException("dispositionValue");
      }
      var dispo = new ContentDisposition();
      dispo.parameters = new SortedMap<string, string>();
      return (!dispo.ParseDisposition(dispositionValue)) ? defaultValue : dispo;
    }
  }
}
