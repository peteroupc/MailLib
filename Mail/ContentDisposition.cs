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
    /// <summary>Description of ContentDisposition.</summary>
  public class ContentDisposition
  {
    private string dispositionType;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string DispositionType {
      get {
        return this.dispositionType;
      }
    }

    #region Equals and GetHashCode implementation
    public override bool Equals(object obj) {
      ContentDisposition other = obj as ContentDisposition;
      if (other == null) {
        return false;
      }
      return this.dispositionType == other.dispositionType &&
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
    /// <value>Whether the disposition type is inline.</value>
    public bool IsInline {
      get {
        return this.dispositionType.Equals("inline");
      }
    }

    /// <summary>Gets a value indicating whether the disposition type is
    /// attachment.</summary>
    /// <value>Whether the disposition type is attachment.</value>
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

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IDictionary<string, string> Parameters {
      get {
        return new ReadOnlyMap<string, string>(this.parameters);
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sb);
      return sb.ToString();
    }

    private static string RemoveEncodedWordEnds(string str) {
      StringBuilder sb = new StringBuilder();
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
            sb.Append(0xfffd);
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
    /// occur so frequently in Content-Disposition filenames); characters
    /// unsuitable for use in a filename (including the directory separators
    /// slash and backslash) are replaced with underscores; and the filename
    /// is truncated if it would otherwise be too long.</returns>
    public static string MakeFilename(string str) {
      if (str == null) {
        return String.Empty;
      }
      str = ParserUtility.TrimSpaceAndTab(str);
      if (str.IndexOf("=?") >= 0) {
        // May contain encoded words, which are very frequent
        // in Content-Disposition filenames
        str = Rfc2047.DecodeEncodedWords(str, 0, str.Length, EncodedWordContext.Unstructured);
        if (str.IndexOf("=?") >= 0) {
          // Remove ends of encoded words that remain
          str = RemoveEncodedWordEnds(str);
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
      StringBuilder builder = new StringBuilder();
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
                   c == ':' || c == '<' || c == '>' || c == '"' || c == 0x7f) {
          // Unsuitable character for a filename
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
          strLower.IndexOf("nul.") ==0 ||
          strLower.Equals("prn") ||
          strLower.IndexOf("prn.") ==0 ||
          strLower.Equals("aux") ||
          strLower.IndexOf("aux.") ==0 ||
          strLower.Equals("con") ||
          strLower.IndexOf("con.") ==0 ||
          (strLower.Length >= 4 && strLower.IndexOf("lpt") ==0 && strLower[3] >= '1' && strLower[3] <= '9') ||
          (strLower.Length >= 4 && strLower.IndexOf("com") ==0 && strLower[3] >= '1' && strLower[3] <= '9')) {
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

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object. (2).</param>
    /// <returns>A string object.</returns>
    public string GetParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (this.parameters.ContainsKey(name)) {
        return this.parameters[name];
      }
      return null;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A Boolean object.</returns>
    private bool ParseDisposition(string str) {
      bool httpRules = false;
      int index = 0;
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      int endIndex = str.Length;
      if (httpRules) {
        index = MediaType.skipLws(str, index, endIndex);
      } else {
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      }
      int i = MediaType.SkipMimeToken(str, index, endIndex, null, httpRules);
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
          // not followed by ";", so not a media type
          return false;
        }
      }
      index = i;
      return MediaType.ParseParameters(str, index, endIndex, httpRules, this.parameters);
    }

    private static ContentDisposition Build(string name) {
      ContentDisposition dispo = new ContentDisposition();
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
    public static readonly ContentDisposition Attachment = Build("attachment");

    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
    #endif
    public static readonly ContentDisposition Inline = Build("inline");

    private ContentDisposition() {
    }

    /// <summary>Parses a media type string and returns a media type object.</summary>
    /// <returns>A media type object, or "Attachment" if <paramref name='dispoValue'/>
    /// is empty or syntactically invalid.</returns>
    /// <param name='dispoValue'>A string object.</param>
    public static ContentDisposition Parse(string dispoValue) {
      return Parse(dispoValue, Attachment);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='defaultValue'>Can be null.</param>
    /// <returns>A ContentDisposition object.</returns>
    public static ContentDisposition Parse(string str, ContentDisposition defaultValue) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      ContentDisposition dispo = new ContentDisposition();
      dispo.parameters = new SortedMap<string, string>();
      if (!dispo.ParseDisposition(str)) {
        #if DEBUG
        // Console.WriteLine("Unparsable: " + str);
        #endif
        return defaultValue;
      }
      return dispo;
    }
  }
}
