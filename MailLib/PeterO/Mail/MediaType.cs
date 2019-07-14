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
using PeterO.Mail.Transforms;
using PeterO.Text;

namespace PeterO.Mail {
    /// <summary>
    /// <para>Specifies what kind of data a message body is.</para>
    /// <para>A media type consists of a top-level type (the general
    /// category of the data), a subtype (the specific type), and an
    /// optional list of parameters. For example, the media type
    /// <c>text/plain; charset = utf-8</c> is a text media type ("text"),
    /// namely, a plain text type ("plain"), and the parameters say that
    /// the data uses UTF-8, a Unicode character encoding ("charset =
    /// utf-8"). Other top-level types include "audio", "video", and
    /// "application".</para>
    /// <para>A media type is sometimes known as a "MIME type", for
    /// Multipurpose Internet Mail Extensions, the standard that introduced
    /// media types.</para>
    /// <para>This type is immutable, meaning its values can't be changed
    /// once it' s created. To create a changeable media type object, use
    /// the MediaTypeBuilder class.</para>
    /// <para><b>Note:</b> According to RFC 2049, unrecognized subtypes of
    /// the top-level type <c>multipart</c> must be treated as
    /// <c>multipart/mixed</c> and unrecognized media types as the media
    /// type <c>application/octet-stream</c>.</para></summary>
  public sealed class MediaType {
    // Printable ASCII characters that cannot appear in a
    // parameter value under RFC 2231 (including single quote
    // and percent)
    private const string AttrValueSpecials = "()<>@,;:\\\"/[]?='%*";
    private const string ValueHex = "0123456789ABCDEF";

    private readonly string topLevelType;

    private static readonly ICharacterEncoding USAsciiEncoding =
      Encodings.GetEncoding("us-ascii", true);

    /// <summary>Gets the name of this media type's top-level type (such as
    /// "text" in "text/plain", or "audio" in "audio/basic"). The resulting
    /// string will be in lowercase letters.</summary>
    /// <value>The name of this media type's top-level type (such as "text"
    /// or "audio" .</value>
    public string TopLevelType {
      get {
        return this.topLevelType;
      }
    }

    #region Equals and GetHashCode implementation

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is not
    /// documented yet.</param>
    /// <returns>Either <c>true</c> or <c>false</c>.</returns>
    public override bool Equals(object obj) {
      var other = obj as MediaType;
      if (other == null) {
        return false;
      }
      return this.topLevelType.Equals(other.topLevelType,
  StringComparison.Ordinal) &&
        this.subType.Equals(other.subType, StringComparison.Ordinal) &&
          CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /// <summary>Calculates the hash code of this object. No application or
    /// process IDs are used in the hash code calculation.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public override int GetHashCode() {
      var hashCode = 632580499;
      if (this.topLevelType != null) {
        for (var i = 0; i < this.topLevelType.Length; ++i) {
          hashCode = unchecked(hashCode + (632580563 *
                    this.topLevelType[i]));
        }
      }
      if (this.subType != null) {
        for (var i = 0; i < this.subType.Length; ++i) {
          hashCode = unchecked(hashCode + (632580563 *
                    this.subType[i]));
        }
      }
      if (this.parameters != null) {
        hashCode = unchecked(hashCode + (632580587 * this.parameters.Count));
      }
      return hashCode;
    }
    #endregion

    private readonly string subType;

    /// <summary>Gets this media type's subtype (for example, "plain" in
    /// "text/plain"). The resulting string will be in lowercase
    /// letters.</summary>
    /// <value>This media type's subtype.</value>
    public string SubType {
      get {
        return this.subType;
      }
    }

    /// <summary>Gets a value indicating whether this is a text media type
    /// ("text/&#x2a;").</summary>
    /// <value><c>true</c> If this is a text media type; otherwise,.
    /// <c>false</c>.</value>
    public bool IsText {
      get {
        return this.TopLevelType.Equals("text", StringComparison.Ordinal);
      }
    }

    /// <summary>Gets a value indicating whether this is a multipart media
    /// type.</summary>
    /// <value><c>true</c> If this is a multipart media type; otherwise,.
    /// <c>false</c>.</value>
    public bool IsMultipart {
      get {
        return this.TopLevelType.Equals("multipart", StringComparison.Ordinal);
      }
    }

    internal MediaType(
  string type,
  string subtype,
  IDictionary<string, string> parameters) {
      this.topLevelType = type;
      this.subType = subtype;
      this.parameters = new Dictionary<string, string>(parameters);
    }

    private readonly Dictionary<string, string> parameters;

    /// <summary>Gets a list of the parameters contained in this media type
    /// object.</summary>
    /// <value>A list of the parameters contained in this media type
    /// object; the names of each parameter appear in an undefined order.
    /// NOTE: Previous versions erroneously stated that the list will be
    /// sorted by name. In fact, the names will not be guaranteed to appear
    /// in any particular order; this is at least the case in version
    /// 0.10.0.</value>
    public IDictionary<string, string> Parameters {
      get {
        return new ReadOnlyMap<string, string>(this.parameters);
      }
    }

    internal enum QuotedStringRule {
    /// <summary>Use HTTP rules for quoted strings.</summary>
      Http,

    /// <summary>Use Internet Message Format rules for quoted
    /// strings.</summary>
      Rfc5322,
    }

    private static int SkipQtextOrQuotedPair(
      string s,
      int index,
      int endIndex,
      QuotedStringRule rule) {
      if (index >= endIndex) {
        return index;
      }
      int i2;
      if (rule == QuotedStringRule.Http) {
        char c = s[index];
        // NOTE: Space and tab were handled earlier;
        // bytes higher than 0x7f are part of obs-text
        if (c < 0x100 && c >= 0x21 && c != 0x7F && c != '\\' && c != '"') {
          return index + 1;
        }
        i2 = SkipQuotedPair(s, index, endIndex);
        return i2;
      }
      if (rule == QuotedStringRule.Rfc5322) {
        i2 = index;
        // qtext (RFC5322 sec. 3.2.1)
        if (i2 < endIndex) {
          char c = s[i2];
          // Non-ASCII (allowed in internationalized email headers under
          // RFC6532)
          if ((c & 0xfc00) == 0xd800 && i2 + 1 < endIndex && (s[i2 + 1] &
            0xfc00) == 0xdc00) {
            i2 += 2;
          } else if ((c & 0xf800) == 0xd800) {
            // unchanged; it's a bare surrogate
          } else if (c >= 0x80) {
            ++i2;
          }
          if (c >= 33 && c <= 126 && c != '\\' && c != '"') {
            ++i2;
          }
          // obs-qtext (same as obs-ctext)
          if ((c < 0x20 && c != 0x00 && c != 0x09 && c != 0x0a && c != 0x0d) ||
            c == 0x7f) {
            ++i2;
          }
        }
        if (index != i2) {
          return i2;
        }
        index = i2;
        i2 = HeaderParser.ParseQuotedPair(s, index, endIndex, null);
        return i2;
      }
      throw new ArgumentException(rule.ToString());
    }

    // quoted-pair (RFC5322 sec. 3.2.1)
    internal static int SkipQuotedPair(string s, int index, int endIndex) {
      if (index + 1 < endIndex && s[index] == '\\') {
        char c = s[index + 1];
        // Non-ASCII (allowed in internationalized email headers under RFC6532)
        if ((c & 0xfc00) == 0xd800 && index + 2 < endIndex && (s[index + 2] &
          0xfc00) == 0xdc00) {
          return index + 3;
        }
        if ((c & 0xf800) == 0xd800) {
          return index;
        }
        if (c >= 0x80) {
          return index + 2;
        }
        if (c == 0x20 || c == 0x09 || (c >= 0x21 && c <= 0x7e)) {
          return index + 2;
        }
        // obs-qp
        if ((c < 0x20 && c != 0x09) || c == 0x7f) {
          return index + 2;
        }
      }
      return index;
    }

    // quoted-string (RFC5322 sec. 3.2.4)
    internal static int SkipQuotedString(
      string s,
      int index,
      int endIndex,
      StringBuilder builder) {
      return SkipQuotedString(
        s,
        index,
        endIndex,
        builder,
        QuotedStringRule.Rfc5322);
    }

    private static int ParseFWSLax(
      string str,
      int index,
      int endIndex,
      StringBuilder sb) {
      while (index < endIndex) {
        int tmp = index;
        // Skip CRLF
        if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
          index += 2;
        }
        // Add WSP
        if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
          if (sb != null) {
            sb.Append(str[index]);
          }
          ++index;
        } else {
          return tmp;
        }
      }
      return index;
    }

    private static int SkipQuotedString(
      string str,
      int index,
      int endIndex,
      StringBuilder builder, // receives the unescaped version of the string
      QuotedStringRule rule) {
      int startIndex = index;
      int valueBLength = (builder == null) ? 0 : builder.Length;
      index = (rule != QuotedStringRule.Rfc5322) ? index :
        HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (!(index < endIndex && str[index] == '"')) {
        if (builder != null) {
          builder.Length = valueBLength;
        }
        return startIndex; // not a valid quoted-string
      }
      ++index;
      while (index < endIndex) {
        int i2 = index;
        if (rule == QuotedStringRule.Http) {
          if (str[index] == ' ' || str[index] == '\t') {
            if (builder != null) {
              builder.Append(str[index]);
            }
            ++index;
            continue;
          }
        } else if (rule == QuotedStringRule.Rfc5322) {
          // Skip tabs, spaces, and folding whitespace
          i2 = ParseFWSLax(str, index, endIndex, builder);
        }
        index = i2;
        char c = str[index];
        if (c == '"') { // end of quoted-string
          ++index;
          // NOTE: Don't skip CFWS even if the rule is Rfc5322
          return index;
        }
        int oldIndex = index;
        index = SkipQtextOrQuotedPair(str, index, endIndex, rule);
        if (index == oldIndex) {
          if (builder != null) {
            builder.Remove(valueBLength, builder.Length - valueBLength);
          }
          return startIndex;
        }
        if (builder != null) {
          // this is a qtext or quoted-pair, so
          // append the last character read
          builder.Append(str[index - 1]);
        }
      }
      if (builder != null) {
        builder.Remove(valueBLength, builder.Length - valueBLength);
      }
      return startIndex; // not a valid quoted-string
    }

    private static bool IsAttributeChar(int c) {
      return c >= 33 && c <= 126 && AttrValueSpecials.IndexOf((char)c) < 0;
    }
    // Intersection of ASCII characters that can appear in a URI path and in
    // an RFC 2231 parameter value (excludes percent
    // and single quote, which serve special purposes
    // in such values)
    private static bool IsIsecnOfUrlPathAndAttrValueChar(int c) {
      return c >= 33 && c <= 126 && ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                    "!$&*+-._~".IndexOf((char)c) >= 0);
    }

    private static void PctAppend(StringBuilder sb, int w) {
      // NOTE: Use uppercase hex characters
      // to encode according to RFC 2231, but the augmented
      // BNF for ext-octet in that RFC allows both upper-case
      // and lower-case, even though only upper-case
      // appears in that production. This
      // is due to the nature of augmented BNF (see RFC
      // 5234 sec 2.3).
      sb.Append('%');
      sb.Append(ValueHex[(w >> 4) & 15]);
      sb.Append(ValueHex[w & 15]);
    }

    private static int EncodeContinuation(
      string str,
      int startPos,
      HeaderEncoder sa,
      bool uriSafe) {
      int column = sa.GetColumn();
      int maxLineLength = sa.GetMaxLineLength();
      int index = startPos;
      var sb = new StringBuilder();
      while (index < str.Length && (maxLineLength < 0 || column <=
        maxLineLength)) {
        int c = str[index];
        bool first = index == 0;
        int contin = (index == 0) ? 7 : 0;
        if ((c & 0xfc00) == 0xd800 && index + 1 < str.Length &&
            (str[index + 1] & 0xfc00) == 0xdc00) {
          c = 0x10000 + ((c & 0x3ff) << 10) + (str[index + 1] & 0x3ff);
        } else if ((c & 0xf800) == 0xd800) {
          c = 0xfffd;
        }
        if (uriSafe ? IsIsecnOfUrlPathAndAttrValueChar(c) :
          IsAttributeChar(c)) {
          ++contin;
        } else if (c <= 0x7f) {
          contin += 3;
        } else if (c <= 0x7ff) {
          contin += 6;
        } else if (c <= 0xffff) {
          contin += 9;
        } else {
          contin += 12;
        }
        if (maxLineLength >= 0 && column + contin > maxLineLength) {
          break;
        }
        if (first) {
          sb.Append("utf-8''");
        }
        if (IsAttributeChar(c)) {
          sb.Append((char)c);
        } else if (c <= 0x7f) {
          PctAppend(sb, c);
        } else if (c <= 0x7ff) {
          PctAppend(sb, 0xc0 | ((c >> 6) & 0x1f));
          PctAppend(sb, 0x80 | (c & 0x3f));
        } else if (c <= 0xffff) {
          PctAppend(sb, 0xe0 | ((c >> 12) & 0x0f));
          PctAppend(sb, 0x80 | ((c >> 6) & 0x3f));
          PctAppend(sb, 0x80 | (c & 0x3f));
        } else {
          PctAppend(sb, 0xf0 | ((c >> 18) & 0x07));
          PctAppend(sb, 0x80 | ((c >> 12) & 0x3f));
          PctAppend(sb, 0x80 | ((c >> 6) & 0x3f));
          PctAppend(sb, 0x80 | (c & 0x3f));
          ++index; // Because it uses 2 surrogates
        }
        ++index;
        column += contin;
      }
      if (maxLineLength >= 0 && index == startPos) {
        // No room to put any continuation here;
        // add a line break and try again
        sa.AppendBreak();
        return EncodeContinuation(str, startPos, sa, uriSafe);
      }
      sa.AppendSymbol(sb.ToString());
      return index;
    }

    private static void AppendComplexParamValue(
      string name,
      string str,
      HeaderEncoder sa,
      bool uriSafe) {
#if DEBUG
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length == 0) {
        throw new ArgumentException("str" + " is empty.");
      }
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (name.Length == 0) {
        throw new ArgumentException("name" + " is empty.");
      }
#endif

      int column = sa.GetColumn();
      // Check if parameter is short enough for the column that
      // no continuations are needed
      int continColumn = column + name.Length + 9;
      int oldcolumn = sa.GetColumn();
      int oldlength = sa.GetLength();
      sa.AppendSymbol(name + "*").AppendSymbol("=");
      if (EncodeContinuation(str, 0, sa, uriSafe) != str.Length) {
        sa.Reset(oldcolumn, oldlength);
        var contin = 0;
        var index = 0;
        while (index < str.Length) {
          if (contin > 0) {
            sa.AppendSymbol(";");
          }
          sa.AppendSymbol(name + "*" + ParserUtility.IntToString(contin) + "*")
     .AppendSymbol("=");
          index = EncodeContinuation(str, index, sa, uriSafe);
          ++contin;
        }
      }
    }

    private static bool AppendSimpleParamValue(
      string name,
      string str,
      HeaderEncoder sa,
      bool uriSafe) {
      if (str.Length == 0) {
        if (uriSafe) {
          sa.AppendSymbol(name + "*");
          sa.AppendSymbol("=");
          sa.AppendSymbol("utf-8''");
        } else {
          sa.AppendSymbol(name);
          sa.AppendSymbol("=");
          sa.AppendSymbol("\"\"");
        }
        return true;
      }
      sa.AppendSymbol(name);
      sa.AppendSymbol("=");
      var simple = true;
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (uriSafe ? (!IsIsecnOfUrlPathAndAttrValueChar(c)) :
          (!IsAttributeChar(c))) {
          simple = false;
        }
      }
      if (simple) {
        return sa.TryAppendSymbol(str);
      }
      if (uriSafe) {
        return false;
      }
      var sb = new StringBuilder();
      sb.Append('"');
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c >= 32 && c <= 126 && c != '\\' && c != '"') {
          sb.Append(c);
        } else if (c == 0x09 || c == '\\' || c == '"') {
          sb.Append('\\');
          sb.Append(c);
        } else {
          // Requires complex encoding
          return false;
        }
        if (sa.GetMaxLineLength() >= 0 && sb.Length > sa.GetMaxLineLength()) {
          // Too long to fit (optimization for very
          // long parameter values)
          return false;
        }
      }
      sb.Append('"');
      return sa.TryAppendSymbol(sb.ToString());
    }

    internal static void AppendParameters(
      IDictionary<string, string> parameters,
      HeaderEncoder sa) {
      AppendParameters(parameters, sa, false);
    }
    internal static void AppendParameters(
      IDictionary<string, string> parameters,
      HeaderEncoder sa,
      bool uriSafe) {
      var keylist = new List<string>(parameters.Keys);
      keylist.Sort();
      foreach (string key in keylist) {
        string name = key;
        string value = parameters[key];
        sa.AppendSymbol(";");
        int oldcolumn = sa.GetColumn();
        int oldlength = sa.GetLength();
        if (!AppendSimpleParamValue(name, value, sa, uriSafe)) {
          sa.Reset(oldcolumn, oldlength);
          AppendComplexParamValue(name, value, sa, uriSafe);
        }
      }
    }

    /// <summary>Converts this media type to a text string form suitable
    /// for inserting in email headers. Notably, the string contains the
    /// value of a Content-Type header field (without the text necessarily
    /// starting with "Content-Type" followed by a space), and consists of
    /// one or more lines.</summary>
    /// <returns>A text string form of this media type.</returns>
    public override string ToString() {
      // NOTE: 14 is the length of "Content-Type: " (with trailing
      // space).
      var sa = new HeaderEncoder(Message.MaxRecHeaderLineLength, 14);
      sa.AppendSymbol(this.topLevelType + "/" + this.subType);
      AppendParameters(this.parameters, sa);
      return sa.ToString();
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A text string.</returns>
    public string ToSingleLineString() {
      // NOTE: 14 is the length of "Content-Type: " (with trailing space).
      var sa = new HeaderEncoder(-1, 14);
      sa.AppendSymbol(this.topLevelType + "/" + this.subType);
      AppendParameters(this.parameters, sa);
      return sa.ToString();
    }

    /// <summary>Converts this media type to a text string form suitable
    /// for data URIs. Notably, the string contains the value of a
    /// Content-Type header field (without the text necessarily starting
    /// with "Content-Type" followed by a space), consists of a single
    /// line, and uses percent-encoding as necessary or convenient so that
    /// the resulting string can validly appear in a URI path.</summary>
    /// <returns>A text string form of this media type.</returns>
    public string ToUriSafeString() {
      // NOTE: 14 is the length of "Content-Type: " (with trailing space).
      var sa = new HeaderEncoder(-1, 14);
      sa.AppendSymbol(this.topLevelType + "/" + this.subType);
      AppendParameters(this.parameters, sa, true);
      return sa.ToString();
    }

    internal static int SkipMimeToken(
      string str,
      int index,
      int endIndex,
      StringBuilder builder,
      bool httpRules) {
      int i = index;
      const string ValueSpecials = "()<>@,;:\\\"/[]?=";
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7f || (c == (c & 0x7f) &&
           ValueSpecials.IndexOf(c) >= 0)) {
          break;
        }
        if (httpRules && (c == '{' || c == '}')) {
          break;
        }
        if (builder != null) {
          builder.Append(c);
        }
        ++i;
      }
      return i;
    }

    internal static int SkipAttributeNameRfc2231(
      string str,
      int index,
      int endIndex,
      StringBuilder builder,
      bool httpRules) {
      if (httpRules) {
        return SkipMimeToken(str, index, endIndex, builder, httpRules);
      }
      int i = index;
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7f || ((c & 0x7f) == c &&
          AttrValueSpecials.IndexOf(c) >= 0)) {
          break;
        }
        if (builder != null) {
          builder.Append(c);
        }
        ++i;
      }
      if (i + 1 < endIndex && str[i] == '*' && str[i + 1] == '0') {
        // initial-section
        i += 2;
        if (builder != null) {
          builder.Append("*0");
        }
        if (i < endIndex && str[i] == '*') {
          ++i;
          if (builder != null) {
            builder.Append("*");
          }
        }
        return i;
      }
      if (i + 1 < endIndex && str[i] == '*' && str[i + 1] >= '1' && str[i +
        1] <= '9') {
        // other-sections
        if (builder != null) {
          builder.Append('*');
          builder.Append(str[i + 1]);
        }
        i += 2;
        while (i < endIndex && str[i] >= '0' && str[i] <= '9') {
          if (builder != null) {
            builder.Append(str[i]);
          }
          ++i;
        }
        if (i < endIndex && str[i] == '*') {
          if (builder != null) {
            builder.Append(str[i]);
          }
          ++i;
        }
        return i;
      }
      if (i < endIndex && str[i] == '*') {
        if (builder != null) {
          builder.Append(str[i]);
        }
        ++i;
      }
      return i;
    }

    internal static int SkipMimeTypeSubtype(
      string str,
      int index,
      int endIndex,
      StringBuilder builder) {
      int i = index;
      var count = 0;
      string specials = "!#$&-^_.+";
      while (i < endIndex) {
        char c = str[i];
        // See RFC6838
        if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' &&
          c <= '9')) {
          if (builder != null) {
            builder.Append(c);
          }
          ++i;
          ++count;
        } else if (count > 0 && (c == (c & 0x7f) && specials.IndexOf(c) >=
              0)) {
          if (builder != null) {
            builder.Append(c);
          }
          ++i;
          ++count;
        } else {
          break;
        }
        // type or subtype too long
        if (count > 127) {
          return index;
        }
      }
      return i;
    }

    internal bool StoresCharsetInPayload() {
      // Returns true if the media type is text and contains special
      // procedures for determining the charset from the payload
      // if no charset is given or supported in the charset
      // parameter.
      if (this.IsText) {
        string sub = this.SubType;
        if (sub.Equals("html", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("javascript", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("ecmascript", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("rtf", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("xml", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("xml-external-parsed-entity",
  StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.in3d.3dml", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.iptc.newsml", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.iptc.nitf", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.ms-mediapackage", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.net2phone.commcenter.command",
  StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.radisys.msml-basic-layout",
  StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.wap.si", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.wap.sl", StringComparison.Ordinal)) {
          return true;
        }
        if (sub.Equals("vnd.wap.wml", StringComparison.Ordinal)) {
          return true;
        }
      }
      return false;
    }

    /// <summary>Gets this media type's "charset" parameter, naming a
    /// character encoding used to represent text in the data that uses
    /// this media type.</summary>
    /// <returns>If the "charset" parameter is present and non-empty,
    /// returns the result of the Encodings.ResolveAliasForEmail method for
    /// that parameter, except that result's basic upper-case letters A to
    /// Z (U+0041 to U+005A) are converted to lower case. If the "charset"
    /// parameter is absent or empty, returns the default value, if any,
    /// for that parameter given the media type (e.g., "us-ascii" if the
    /// media type is "text/plain"; see RFC2046), or the empty string if
    /// there is none.</returns>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design", "CA1024",
  Justification="This method has different semantics from " +
        "GetParameter(\"charset\").")]
#endif
    public string GetCharset() {
      // NOTE: RFC6657 changed the rules for the default charset in text
      // media types, so that there is no default charset for as yet
      // undefined media types. However,
      // media types defined before this RFC (July 2012) are grandfathered
      // from the rule: those
      // media types "that fail to specify how the charset is determined" still
      // have US-ASCII as default. The text media types defined as of
      // Jun. 9, 2019, are listed below:
      //
      // -- No default charset assumed: --
      //
      // RTP payload types; these are usually unsuitable for MIME,
      // and don't permit a charset parameter, so a default charset is
      // irrelevant:
      // -- 1d-interleaved-parityfec, fwdred, red, parityfec, encaprtp,
      // raptorfec, rtp-enc-aescm128, t140, ulpfec, rtx, rtploopback, flexfec
      //
      // Charset determined out-of-band:
      // -- vnd.motorola.reflex*(5)*(10)
      //
      // Special procedure defined for charset detection:
      // -- ecmascript*(8), javascript*(8), html,
      // rtf*(5)
      //
      // XML formats (no default assumed if charset is absent, according
      // to RFC7303, the revision of the XML media type specification):
      // -- xml, xml-external-parsed-entity,
      // vnd.in3d.3dml*, vnd.iptc.newsml, vnd.iptc.nitf,
      // vnd.ms-mediapackage*(5),
      // vnd.net2phone.commcenter.command, vnd.radisys.msml-basic-layout,
      // vnd.wap.si, vnd.wap.sl, vnd.wap.wml
      //
      // Behavior deliberately undefined (so whether US-ASCII or another
      // charset is treated as default is irrelevant):
      // -- example
      //
      // These media types don't define a charset parameter (after
      // RFC6657):
      // -- grammar-ref-list*(9), vnd.hgl*(6)*(9), vnd.gml*(9),
      // vnd.senx.warpscript*(9), vnd.sosi*(9)
      //
      // Uses charset parameter, but no default charset specified (after
      // RFC6657):
      // -- markdown*
      //
      // -- US-ASCII assumed: --
      //
      // These media types don't define a charset parameter (before
      // RFC6657):
      // -- dns, mizar, vnd.latex-z,
      // prs.lines.tag, vnd.dmclientscript,
      // vnd.dvb.subtitle, rfc822-headers,
      // vnd.si.uricatalogue*(7), vnd.si.fly*(7)
      //
      // No charset parameter defined, but does specify ASCII only (after
      // RFC6657):
      // -- vnd.ascii-art****, prs.prop.logic*
      //
      // These media types don't define a default charset:
      // -- css, richtext, enriched, tab-separated-values*,
      // vnd.in3d.spot*, vnd.abc, vnd.wap.wmlscript, vnd.curl,
      // vnd.fmi.flexstor, uri-list, directory*
      //
      // US-ASCII default:
      // -- plain, sgml, troff
      //
      // -- UTF-8 assumed: --
      //
      // UTF-8 only:
      // -- vcard, jcr-cnd, cache-manifest
      //
      // Charset parameter defined but is "always ... UTF-8":
      // -- n3, turtle, vnd.debian.copyright, provenance-notation
      //
      // UTF-8 default:
      // -- csv, calendar**, vnd.a***, parameters, prs.fallenstein.rst,
      // vnd.esmertec.theme.descriptor, vnd.trolltech.linguist,
      // vnd.graphviz, vnd.sun.j2me.app-descriptor, strings*(5),
      // csv-schema*(5)
      //
      // * Required parameter.
      // ** No explicit default, but says that "[t]he charset supported
      // by this revision of iCalendar is UTF-8."
      // *(5) No charset parameter defined.
      // *(6) 8-bit encoding.
      // *(7) Says "US-ASCII" is always used, or otherwise says
      // the media type contains ASCII text
      // *(8) RFC4329: If charset unrecognized, check for UTF-8/16/32 BOM if it
      // exists; otherwise use UTF-8. If UTF-8, ignore UTF-8 BOM
      // *(9) After RFC 6657.
      // *(10) Charset determined "in an a priori manner", rather than
      // being stated in the payload.
      // *** Default is UTF-8 "if 8-bit bytes are encountered" (even if
      // none are found, though, a 7-bit ASCII text is still also UTF-8).
      // **** Content containing non-ASCII bytes "should be rejected".
      string param = this.GetParameter("charset");
      if (!String.IsNullOrEmpty(param)) {
        // Charset parameter is present and non-empty
        param = Encodings.ResolveAliasForEmail(param);
        return DataUtilities.ToLowerCaseAscii(param);
      } else {
        // Charset parameter is absent or empty
        if (this.IsText) {
          string sub = this.SubType;
          // Media types that assume a default of US-ASCII
          if (sub.Equals("plain", StringComparison.Ordinal) ||
  sub.Equals("sgml", StringComparison.Ordinal) ||
  sub.Equals("troff", StringComparison.Ordinal) ||
  sub.Equals("dns", StringComparison.Ordinal) ||
  sub.Equals("mizar", StringComparison.Ordinal) ||
  sub.Equals("prs.prop.logic", StringComparison.Ordinal) ||
  sub.Equals("vnd.ascii-art", StringComparison.Ordinal) ||
  sub.Equals("vnd.dmclientscript", StringComparison.Ordinal) ||
  sub.Equals("prs.lines.tag", StringComparison.Ordinal) ||
  sub.Equals("vnd.latex-z", StringComparison.Ordinal) ||
  sub.Equals("rfc822-headers", StringComparison.Ordinal) ||
  sub.Equals("vnd.dvb.subtitle", StringComparison.Ordinal) ||
  sub.Equals("vnd.fly", StringComparison.Ordinal) ||
  sub.Equals("directory", StringComparison.Ordinal) ||
  sub.Equals("css", StringComparison.Ordinal) ||
  sub.Equals("richtext", StringComparison.Ordinal) ||
  sub.Equals("enriched", StringComparison.Ordinal) ||
  sub.Equals("tab-separated-values", StringComparison.Ordinal) ||
  sub.Equals("vnd.in3d.spot", StringComparison.Ordinal) ||
  sub.Equals("vnd.abc", StringComparison.Ordinal) ||
  sub.Equals("vnd.wap.wmlscript", StringComparison.Ordinal) ||
  sub.Equals("vnd.curl", StringComparison.Ordinal) ||
  sub.Equals("vnd.fmi.flexstor", StringComparison.Ordinal) ||
  sub.Equals("uri-list", StringComparison.Ordinal) ||
  sub.Equals("vnd.si.uricatalogue", StringComparison.Ordinal)) {
            return "us-ascii";
          }
          // Media types that assume a default of UTF-8
          if (sub.Equals("vcard", StringComparison.Ordinal) ||
  sub.Equals("jcr-cnd", StringComparison.Ordinal) ||
  sub.Equals("n3", StringComparison.Ordinal) ||
  sub.Equals("turtle", StringComparison.Ordinal) ||
  sub.Equals("strings", StringComparison.Ordinal) ||
  sub.Equals("vnd.debian.copyright", StringComparison.Ordinal) ||
  sub.Equals("provenance-notation", StringComparison.Ordinal) ||
  sub.Equals("csv", StringComparison.Ordinal) ||
  sub.Equals("calendar", StringComparison.Ordinal) ||
  sub.Equals("vnd.a", StringComparison.Ordinal) ||
  sub.Equals("parameters", StringComparison.Ordinal) ||
  sub.Equals("prs.fallenstein.rst", StringComparison.Ordinal) ||
  sub.Equals("vnd.esmertec.theme.descriptor", StringComparison.Ordinal) ||
  sub.Equals("vnd.trolltech.linguist", StringComparison.Ordinal) ||
  sub.Equals("csv-schema", StringComparison.Ordinal) ||
  sub.Equals("vnd.graphviz", StringComparison.Ordinal) ||
  sub.Equals("cache-manifest", StringComparison.Ordinal) ||
  sub.Equals("vnd.sun.j2me.app-descriptor", StringComparison.Ordinal)) {
            return "utf-8";
          }
        }
        return String.Empty;
      }
    }

    /// <summary>Gets the value of a parameter in this media type, such as
    /// "charset" or "format".</summary>
    /// <param name='name'>Name of the parameter to get. The name is
    /// compared using a basic case-insensitive comparison. (Two strings
    /// are equal in such a comparison, if they match after converting the
    /// basic upper-case letters A to Z (U + 0041 to U + 005A) in both
    /// strings to lower case.).</param>
    /// <returns>The value of the parameter as a string, or null if the
    /// parameter doesn't exist.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    /// <exception cref='ArgumentException'>Name is empty.</exception>
    public string GetParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return this.parameters.ContainsKey(name) ? this.parameters[name] : null;
    }

    private static string DecodeRfc2231Extension(string value, bool httpRules) {
      int firstQuote = value.IndexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      int secondQuote = value.IndexOf('\'', firstQuote + 1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      string charset = value.Substring(0, firstQuote);
      if (httpRules && charset.Length == 0) {
        // charset is omitted, which is not allowed under RFC8187
        return null;
      }
      string language = value.Substring(
  firstQuote + 1,
  secondQuote - (firstQuote + 1));
      if (language.Length > 0 &&
        !LanguageTags.IsPotentiallyValidLanguageTag(language)) {
        // not a valid language tag
        return null;
      }
      string paramValue = value.Substring(secondQuote + 1);
      // NOTE: For HTTP (RFC 8187) no specific error-handling
      // behavior is mandated for "encoding errors", which can
      // be interpreted as including unsupported or unrecognized
      // character encodings (see sec. 3.2.1).
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = cs ?? USAsciiEncoding;
      return DecodeRfc2231Encoding(paramValue, cs);
    }

    private static ICharacterEncoding GetRfc2231Charset(string value) {
      // NOTE: Currently only used outside of httpRules
      if (value == null) {
        return USAsciiEncoding;
      }
      int firstQuote = value.IndexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return USAsciiEncoding;
      }
      int secondQuote = value.IndexOf('\'', firstQuote + 1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return USAsciiEncoding;
      }
      string charset = value.Substring(0, firstQuote);
      string language = value.Substring(
  firstQuote + 1,
  secondQuote - (firstQuote + 1));
      if (language.Length > 0 &&
        !LanguageTags.IsPotentiallyValidLanguageTag(language)) {
        // not a valid language tag
        return USAsciiEncoding;
      }
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = cs ?? USAsciiEncoding;
      return cs;
    }

    private static string DecodeRfc2231Encoding(
      string value,
      ICharacterEncoding charset) {
      // a value without a quote
      // mark is not a valid encoded parameter
      int quote = value.IndexOf('\'');
      return (quote >= 0) ? null : Encodings.DecodeToString(
        charset,
        new PercentEncodingStringTransform(value));
    }

    private static bool ExpandRfc2231Extensions(
  IDictionary<string, string> parameters,
  bool httpRules) {
      if (parameters.Count == 0) {
        return true;
      }
      // NOTE: RFC 2231 doesn't specify what happens if a "*" extension
      // and a "*0*" continuation both appear in the same media type string.
      // In this implementation, due to the sorting which follows,
      // the former will appear before the latter in the key list and
      // will generally be overridden by the latter in the
      // final parameter list.
      var keyList = new List<string>(parameters.Keys);
      keyList.Sort();
      foreach (string name in keyList) {
        if (!parameters.ContainsKey(name)) {
          continue;
        }
        string value = parameters[name];
        int asterisk = name.IndexOf('*');
        if (asterisk == name.Length - 1 && asterisk > 0) {
          // name*="value" (except when the parameter is just "*")
          // NOTE: As of RFC 5987 (now RFC 8187), this particular extension
          // is now allowed in HTTP
          string realName = name.Substring(0, name.Length - 1);
          string realValue = DecodeRfc2231Extension(value, httpRules);
          if (realValue == null) {
            continue;
          }
          parameters.Remove(name);
          // NOTE: Overrides the name without continuations
          // (also suggested by RFC8187 sec. 4.2)
          parameters[realName] = realValue;
          continue;
        }
        // name*0 or name*0*
        if (!httpRules && asterisk > 0 && ((asterisk == name.Length - 2 &&
          name[asterisk + 1] == '0') || (asterisk == name.Length - 3 &&
          name[asterisk + 1] == '0' && name[asterisk + 2] == '*'))) {
          string realName = name.Substring(0, asterisk);
          // NOTE: 'httpRules' for DecodeRfc2231Extension is false
          string realValue = (asterisk == name.Length - 3) ?
            DecodeRfc2231Extension(value, false) : value;
          ICharacterEncoding charsetUsed = GetRfc2231Charset(
            (asterisk == name.Length - 3) ? value : null);
          parameters.Remove(name);
          var pindex = 1;
          var builder = new StringBuilder();
          builder.Append(realValue);
          // search for name*1 or name*1*, then name*2 or name*2*,
          // and so on
          while (true) {
            string contin = realName + "*" +
              ParserUtility.IntToString(pindex);
            string continEncoded = contin + "*";
            if (parameters.ContainsKey(continEncoded)) {
              // Encoded continuation (checked first)
              string newEnc = DecodeRfc2231Encoding(
                parameters[continEncoded],
                charsetUsed);
              if (newEnc == null) {
                // Contains a quote character in the encoding, so illegal
                break;
              }
              builder.Append(newEnc);
              parameters.Remove(continEncoded);
            } else if (parameters.ContainsKey(contin)) {
              // Unencoded continuation (checked second)
              builder.Append(parameters[contin]);
              parameters.Remove(contin);
            } else {
              break;
            }
            ++pindex;
          }
          realValue = builder.ToString();
          // NOTE: Overrides the name without continuations
          parameters[realName] = realValue;
        }
      }
      keyList = new List<string>(parameters.Keys);
      foreach (string name in keyList) {
        // Check parameter names using stricter format
        // in RFC6838
        if (SkipMimeTypeSubtype(name, 0, name.Length, null) != name.Length) {
          // Illegal parameter name, so use default media type
          // return false;
          parameters.Remove(name);
        }
      }
      return true;
    }

    /// <summary>Gets the top level type and subtype of this media type,
    /// separated by a slash; for example, "text/plain". The resulting
    /// string will be in lowercase letters.</summary>
    /// <value>The top level type and subtype of this media type, separated
    /// by a slash; for example, "text/plain".</value>
    public string TypeAndSubType {
      get {
        return this.TopLevelType + "/" + this.SubType;
      }
    }

    internal static int SkipOws(string s, int index, int endIndex) {
      int i2 = index;
      while (i2 < endIndex) {
        if (s[i2] == 0x09 || s[i2] == 0x20) {
          ++index;
        }
        break;
      }
      return index;
    }

    internal static bool ParseParameters(
  string str,
  int index,
  int endIndex,
  bool httpRules,
  IDictionary<string, string> parameters) {
      var duplicateAttributes = new Dictionary<string, string>();
      var hasDuplicateAttributes = false;
      while (true) {
        // RFC5322 uses ParseCFWS when skipping whitespace;
        // HTTP currently uses skipOws
        index = httpRules ? SkipOws(str, index, endIndex) :
              HeaderParser.ParseCFWS(
                str,
                index,
                endIndex,
                null);
        if (index >= endIndex) {
          // No more parameters
          return ExpandRfc2231Extensions(parameters, httpRules);
        }
        if (str[index] != ';') {
          return false;
        }
        ++index;
        index = httpRules ? SkipOws(str, index, endIndex) :
              HeaderParser.ParseCFWS(
                str,
                index,
                endIndex,
                null);
        var builder = new StringBuilder();
        // NOTE:
        // 1. RFC6838 restricts the format of parameter names to the same
        // syntax as types and subtypes, but this syntax is incompatible with
        // the RFC2045 format for attributes (which is the same as for
        // MIME tokens).
        // 2. RFC2231 further restricts the syntax of MIME tokens to
        // accommodate certain extensions, but this is not checked here; rather,
        // RFC2231 extensions are handled after the attribute names and
        // values are parsed; in this process, certain attribute names
        // containing
        // an asterisk will be deleted and replaced with other parameters.
        // See also RFC 8187, sec. 3.2.1.
        int afteratt = SkipMimeToken(
          str,
          index,
          endIndex,
          builder,
          httpRules);
        if (afteratt == index) { // ill-formed attribute
          return false;
        }
        string attribute = builder.ToString();
        index = afteratt;
        if (!httpRules) {
          // NOTE: MIME implicitly doesn't restrict whether whitespace can
          // appear
          // around the equal sign separating an attribute and value, while
          // HTTP explicitly forbids such whitespace
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
        if (index >= endIndex) {
          return false;
        }
        if (str[index] != '=') {
          return false;
        }
        attribute = DataUtilities.ToLowerCaseAscii(attribute);
        if (parameters.ContainsKey(attribute)) {
          parameters.Remove(attribute);
          duplicateAttributes[attribute] = String.Empty;
          hasDuplicateAttributes = true;
        }
        ++index;
        if (!httpRules) {
          // See note above on whitespace around the equal sign
          index = HeaderParser.ParseCFWS(
            str,
            index,
            endIndex,
            null);
        }
        if (index >= endIndex) {
          // No more parameters
          return ExpandRfc2231Extensions(parameters, httpRules);
        }
        builder.Remove(0, builder.Length);
        int qs;
        // try getting the value quoted
        qs = SkipQuotedString(
          str,
          index,
          endIndex,
          builder,
          httpRules ? QuotedStringRule.Http : QuotedStringRule.Rfc5322);
        if (!httpRules && qs != index) {
          qs = HeaderParser.ParseCFWS(str, qs, endIndex, null);
        }
        if (qs != index) {
          // If the attribute name ends with '*' the value may not be a quoted
          // string because of RFC2231; if this happens, ignore the attribute
          if (attribute[attribute.Length - 1] != '*' &&
   (!hasDuplicateAttributes || !duplicateAttributes.ContainsKey(attribute))) {
            parameters[attribute] = builder.ToString();
          }
          index = qs;
          continue;
        }
        builder.Remove(0, builder.Length);
        // try getting the value unquoted
        // Note we don't use getAtom
        qs = SkipMimeToken(str, index, endIndex, builder, httpRules);
        if (qs != index) {
          if (!hasDuplicateAttributes ||
                  !duplicateAttributes.ContainsKey(attribute)) {
            parameters[attribute] = builder.ToString();
          }
          index = qs;
          continue;
        }
        // no valid value, return
        return false;
      }
    }

    private static MediaType ParseMediaType(string str) {
      const bool HttpRules = false;
      var index = 0;
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      int endIndex = str.Length;
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      int i = SkipMimeTypeSubtype(str, index, endIndex, null);
      if (i == index || i >= endIndex || str[i] != '/') {
        return null;
      }
      var parameters = new Dictionary<string, string>();
      string topLevelType =
        DataUtilities.ToLowerCaseAscii(str.Substring(index, i - index));
      ++i;
      int i2 = SkipMimeTypeSubtype(str, i, endIndex, null);
      if (i == i2) {
        return null;
      }
      string subType = DataUtilities.ToLowerCaseAscii(str.Substring(i, i2 - i));
      if (i2 < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i2, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return new MediaType(topLevelType, subType, parameters);
        }
        if (i3 < endIndex && str[i3] != ';') {
          // not followed by ";", so not a media type
          return null;
        }
      }
      index = i2;
      return ParseParameters(
        str,
        index,
        endIndex,
        HttpRules,
        parameters) ? new MediaType(topLevelType, subType, parameters) : null;
    }

#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
#endif

    /// <summary>Specifies the media type "text/plain" and the "charset"
    /// parameter "US-ASCII", used for plain text data.</summary>
    public static readonly MediaType TextPlainAscii =
      new MediaTypeBuilder(
        "text",
        "plain").SetParameter(
        "charset",
        "us-ascii").ToMediaType();

#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
#endif

    /// <summary>Specifies the media type "text/plain" and the "charset"
    /// parameter "utf-8", used for plain text data that may contain
    /// characters outside the basic Latin range (U + 0000 to U +
    /// 007F).</summary>
    public static readonly MediaType TextPlainUtf8 =
      new MediaTypeBuilder(
        "text",
        "plain").SetParameter(
        "charset",
        "utf-8").ToMediaType();

#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
#endif

    /// <summary>Specifies the media type "message/rfc822", used for
    /// Internet mail messages.</summary>
    public static readonly MediaType MessageRfc822 =
      new MediaTypeBuilder("message", "rfc822").ToMediaType();

#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
#endif

    /// <summary>Specifies the media type "application/octet-stream", used
    /// for arbitrary binary data.</summary>
    public static readonly MediaType ApplicationOctetStream =
      new MediaTypeBuilder("application", "octet-stream").ToMediaType();

    private MediaType() {
      this.topLevelType = String.Empty;
      this.subType = String.Empty;
      this.parameters = new Dictionary<string, string>();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='mediaTypeValue'>The parameter <paramref
    /// name='mediaTypeValue'/> is not documented yet.</param>
    /// <returns>A MediaType object.</returns>
    public static MediaType Parse(string mediaTypeValue) {
      return Parse(mediaTypeValue, TextPlainAscii);
    }

    /// <summary>Parses a media type string and returns a media type
    /// object, or the default value if the string is invalid. This method
    /// checks the syntactic validity of the string, but not whether it has
    /// all parameters it's required to have or whether the parameters
    /// themselves are set to valid values for the parameter.
    /// <para>This method assumes the given media type string was directly
    /// extracted from the Content-Type header field (as defined for email
    /// messages) and follows the syntax given in RFC 2045. Accordingly,
    /// among other things, the media type string can contain comments
    /// (delimited by parentheses).</para>
    /// <para>RFC 2231 extensions allow each media type parameter to be
    /// associated with a character encoding and/or language, and support
    /// parameter values that span two or more key-value pairs. Parameters
    /// making use of RFC 2231 extensions have names with an asterisk
    /// ("&#x2a;"). Such a parameter will be ignored if it is ill-formed
    /// because of RFC 2231's rules (except for illegal percent-decoding or
    /// undecodable sequences for the given character encoding). Examples
    /// of RFC 2231 extensions follow (both examples encode the same
    /// "filename" parameter):</para>
    /// <para><b>text/example;
    /// filename&#x2a;=utf-8'en'filename.txt</b></para>
    /// <para><b>text/example; filename&#x2a;0&#x2a;=utf-8'en'file;
    /// filename&#x2a;1&#x2a;=name%2Etxt</b></para>
    /// <para>This implementation ignores keys (in parameter key-value
    /// pairs) that appear more than once in the media type. Nothing in
    /// RFCs 2045, 2183, 2231, 6266, or 7231 explicitly disallows such
    /// keys, or otherwise specifies error-handling behavior for such
    /// keys.</para></summary>
    /// <param name='str'>A text string representing a media type. This
    /// media type can include parameters.</param>
    /// <param name='defaultValue'>The media type to return if the string
    /// is syntactically invalid. Can be null.</param>
    /// <returns>A MediaType object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public static MediaType Parse(string str, MediaType defaultValue) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      MediaType mt = ParseMediaType(str);
      return mt ?? defaultValue;
    }
  }
}
