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
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.ContentDisposition"]/*'/>
  public class ContentDisposition {
    private readonly string dispositionType;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.DispositionType"]/*'/>
    public string DispositionType {
      get {
        return this.dispositionType;
      }
    }

    #region Equals and GetHashCode implementation
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var other = obj as ContentDisposition;
      if (other == null) {
        return false;
      }
      return this.dispositionType.Equals(other.dispositionType) &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetHashCode"]/*'/>
    public override int GetHashCode() {
      var hashCode = 632580499;
        if (this.dispositionType != null) {
          hashCode = unchecked(hashCode + (632580503 *
            this.dispositionType.GetHashCode()));
        }
        if (this.parameters != null) {
          hashCode = unchecked(hashCode + (632580587 * this.parameters.Count));
        }
      return hashCode;
    }
    #endregion

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.IsInline"]/*'/>
    public bool IsInline {
      get {
        return this.dispositionType.Equals("inline");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.IsAttachment"]/*'/>
    public bool IsAttachment {
      get {
        return this.dispositionType.Equals("attachment");
      }
    }

    internal ContentDisposition(
 string type,
 IDictionary<string, string> parameters) {
      this.dispositionType = type;
      this.parameters = new Dictionary<string, string>(parameters);
    }

    private readonly IDictionary<string, string> parameters;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.Parameters"]/*'/>
    public IDictionary<string, string> Parameters {
      get {
        return new ReadOnlyMap<string, string>(this.parameters);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.ToString"]/*'/>
    public override string ToString() {
      var sb = new StringBuilder();
      sb.Append(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sb);
      return sb.ToString();
    }

    private static string RemoveEncodedWordEnds(string str) {
      var sb = new StringBuilder();
      var index = 0;
      var inEncodedWord = false;
      while (index < str.Length) {
        if (!inEncodedWord && index + 1 < str.Length && str[index] == '=' &&
          str[index + 1] == '?') {
          // Remove start of encoded word
          inEncodedWord = true;
          index += 2;
          int start = index;
          var qmarks = 0;
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
          if (qmarks == 2) {
            inEncodedWord = true;
          } else {
            inEncodedWord = false;
            sb.Append('=');
            sb.Append('?');
            index = start;
          }
        } else if (inEncodedWord && index + 1 < str.Length && str[index] ==
          '?' && str[index + 1] == '=') {
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.MakeFilename(System.String)"]/*'/>
    public static string MakeFilename(string str) {
      if (str == null) {
        return String.Empty;
      }
      str = ParserUtility.TrimAndCollapseSpaceAndTab(str);
      if (str.IndexOf("=?", StringComparison.Ordinal) >= 0) {
        // May contain encoded words, which are very frequent
        // in Content-Disposition filenames (they would appear quoted
        // in the Content-Disposition "filename" parameter); these changes
        // appear justified in sec. 2.3 of RFC 2183, which says that
        // the parameter's value "should be used as a
        // basis for the actual filename, where possible."
        str = Rfc2047.DecodeEncodedWordsLenient(
  str,
  0,
  str.Length);
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
        string charset = Encodings.ResolveAliasForEmail(
  str.Substring(
  0,
  str.IndexOf('\'')));
        if (!String.IsNullOrEmpty(charset)) {
          string newstr = MediaType.DecodeRfc2231Extension(str);
          if (!String.IsNullOrEmpty(newstr)) {
            // Value was decoded under RFC 2231
            str = newstr;
          }
        }
      }
      str = ParserUtility.TrimAndCollapseSpaceAndTab(str);
      if (str.Length == 0) {
        return "_";
      }
      var builder = new StringBuilder();
      // Replace unsuitable characters for filenames
      // and make sure the filename's
      // length doesn't exceed 243. (A few additional characters
      // may be added later on.)
      // NOTE: Even if there are directory separators (backslash
      // and forward slash), the filename is not treated as a
      // file system path (in accordance with sec. 2.3 of RFC
      // 2183); as a result, the directory separators
      // will be treated as unsuitable characters for filenames
      // and are handled below.
      var i = 0;
      while (i < str.Length && builder.Length < 243) {
        int c = DataUtilities.CodePointAt(str, i);
        if (c >= 0x10000) {
          ++i;
        }
        if (c < 0) {
          c = 0xfffd;
        }
        if (c == (int)'\t' || c == 0xa0 || c == 0x3000 ||
   c == 0x180e || c == 0x1680 ||
   (c >= 0x2000 && c <= 0x200b) || c == 0x205f || c == 0x202f || c == 0xfeff) {
          // Replace space-like characters (including tab) with space
          builder.Append(' ');
        } else if (c < 0x20 || c == '\\' || c == '/' || c == '*' ||
          c == '?' || c == '|' ||
    c == ':' || c == '<' || c == '>' || c == '"' ||
          (c >= 0x7f && c <= 0x9f)) {
          // Unsuitable character for a filename (one of the characters
          // reserved by Windows,
          // backslash, forward slash, ASCII controls, and C1 controls).
          builder.Append('_');
  } else if (c=='!' && i+1<str.Length && str[i]=='[') {
     // '![ ... ]' may be interpreted in BASH as an evaluator;
     // replace '!' with underscore
    builder.Append('_');
  } else if (c=='`') {
     // '`' starts a command in BASH and possibly other shells
    builder.Append('_');
  } else if (c=='$') {
     // '$' starts a variable in BASH and possibly other shells
    builder.Append('_');
        } else if (c == 0x2028 || c == 0x2029) {
          // line break characters (0x85 is already included above)
          builder.Append('_');
        } else if ((c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <= 0xfdef)) {
          // noncharacters
          builder.Append('_');
        } else if (c == '%') {
          // Treat percent character as unsuitable, even though it can occur
          // in a Windows filename, since it's used in MS-DOS and Windows
          // in environment variable placeholders
          builder.Append('_');
        } else {
          if (builder.Length < 242 || c < 0x10000) {
            if (c <= 0xffff) {
              builder.Append((char)c);
            } else if (c <= 0x10ffff) {
              builder.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              builder.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        }
  ++i;
      }
      str = builder.ToString();
      str = ParserUtility.TrimAndCollapseSpaceAndTab(str);
      if (str.Length == 0) {
        return "_";
      }
      string strLower = DataUtilities.ToLowerCaseAscii(str);
      // Reserved filenames on Windows
      bool reservedFilename =
  strLower.Equals(
  "nul") || strLower.Equals("clock$") ||
strLower.IndexOf(
  "nul.",
  StringComparison.Ordinal) == 0 || strLower.Equals(
  "prn") ||
strLower.IndexOf(
  "prn.",
  StringComparison.Ordinal) == 0 || strLower.Equals(
  "aux") ||
strLower.IndexOf(
  "aux.",
  StringComparison.Ordinal) == 0 || strLower.Equals(
  "con") ||
strLower.IndexOf(
  "con.",
  StringComparison.Ordinal) == 0 || (
  strLower.Length >= 4 && strLower.IndexOf(
  "lpt",
  StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
       strLower[3] <= '9') || (strLower.Length >= 4 &&
              strLower.IndexOf(
  "com",
  StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
            strLower[3] <= '9');
      bool bracketDigit = str[0] == '{' && str.Length > 1 &&
            str[1] >= '0' && str[1] <= '9';
      // Home folder convention (tilde).
        // Filenames starting with hyphens can also be
        // problematic especially in Unix-based systems,
        // and filenames starting with dollar sign can
        // be misinterpreted if they're treated as expansion
        // symbols
     bool homeFolder = str[0] == '~' || str[0] == '-' || str[0] == '$';
     // Starts with period; may be hidden in some configurations
     bool period = str[0] == '.';
      if (reservedFilename || bracketDigit || homeFolder ||
           period) {
        str = "_" + str;
      }
      // Avoid space before and after last dot
      for (int i = str.Length - 1; i >= 0; --i) {
        if (str[i] == '.') {
          bool spaceAfter = i + 1 < str.Length && str[i + 1] == 0x20;
          bool spaceBefore = i > 0 && str[i - 1] == 0x20;
          if (spaceAfter && spaceBefore) {
            str = str.Substring(0, i - 1) + "_._" + str.Substring(i + 2);
          } else if (spaceAfter) {
            str = str.Substring(0, i) + "._" + str.Substring(i + 2);
          } else if (spaceBefore) {
            str = str.Substring(0, i - 1) + "_." + str.Substring(i + 1);
          }
          break;
        }
      }
      str = NormalizerInput.Normalize(str, Normalization.NFC);
      // Ensure length is 254 or less
      if (str.Length > 254) {
        char c = str[254];
        var newLength = 254;
        if ((c & 0xfc00) == 0xdc00) {
          --newLength;
        }
        str = str.Substring(0, newLength);
      }
      if (str[str.Length - 1] == '.' || str[str.Length - 1] == '~') {
        // Ends in a dot or tilde (a file whose name ends with
        // the latter may be treated as
        // a backup file especially in Unix-based systems).
        // NOTE: Although concatenation of two NFC strings
        // doesn't necessarily lead to an NFC string, this
        // particular concatenation doesn't disturb the NFC
        // status of the string.
        str += "_";
      }
      return str;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetParameter(System.String)"]/*'/>
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

    private static ContentDisposition ParseDisposition(string str) {
      const bool HttpRules = false;
      var index = 0;
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      int endIndex = str.Length;
      var parameters = new Dictionary<string, string>();
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      int i = MediaType.SkipMimeToken(str, index, endIndex, null, HttpRules);
      if (i == index) {
        return null;
      }
      string dispositionType =
        DataUtilities.ToLowerCaseAscii(str.Substring(index, i - index));
      if (i < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return new ContentDisposition(
            dispositionType,
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
        parameters) ? new ContentDisposition(
            dispositionType,
            parameters) : null;
    }

    private static ContentDisposition Build(string name) {
      return new ContentDisposition(
        name,
        new Dictionary<string, string>());
    }

    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
    #endif
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Mail.ContentDisposition.Attachment"]/*'/>
    public static readonly ContentDisposition Attachment =
      Build("attachment");

    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
    #endif
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Mail.ContentDisposition.Inline"]/*'/>
    public static readonly ContentDisposition Inline =
      Build("inline");

    private ContentDisposition() {
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.Parse(System.String)"]/*'/>
    public static ContentDisposition Parse(string dispoValue) {
      if (dispoValue == null) {
  throw new ArgumentNullException("dispoValue");
}
      return Parse(dispoValue, Attachment);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.Parse(System.String,PeterO.Mail.ContentDisposition)"]/*'/>
    public static ContentDisposition Parse(
  string dispositionValue,
  ContentDisposition defaultValue) {
      if (dispositionValue == null) {
        throw new ArgumentNullException("dispositionValue");
      }
      ContentDisposition dispo = ParseDisposition(dispositionValue);
      return dispo ?? defaultValue;
    }
  }
}
