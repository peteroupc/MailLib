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
import com.upokecenter.mail.transforms.*;
import com.upokecenter.text.*;

    /**
     * <p>Specifies what kind of data a message body is.</p> <p>A media type
     * consists of a top-level type (the general category of the data), a
     * subtype (the specific type), and an optional list of parameters. For
     * example, the media type <code>text/plain; charset = utf-8</code> is a text
     * media type ("text"), namely, a plain text type ("plain"), and the
     * parameters say that the data uses UTF-8, a Unicode character encoding
     * ("charset = utf-8"). Other top-level types include "audio", "video",
     * and "application".</p> <p>A media type is sometimes known as a "MIME
     * type", for Multipurpose Internet Mail Extensions, the standard that
     * introduced media types.</p> <p>This type is immutable, meaning its
     * values can't be changed once it' s created. To create a changeable
     * media type object, use the MediaTypeBuilder class.</p>
     * <p><b>Note:</b> According to RFC 2049, unrecognized subtypes of the
     * top-level type <code>multipart</code> must be treated as
     * <code>multipart/mixed</code> and unrecognized media types as the media type
     * <code>application/octet-stream</code>.</p>
     */
  public final class MediaType {
    // Printable ASCII characters that cannot appear in an
    // attribute value under RFC 2231
    private static final String AttrNameSpecials = "()<>@,;:\\\"/[]?='%*";
    private static final String ValueHex = "0123456789ABCDEF";

    private final String topLevelType;

    private static final ICharacterEncoding USAsciiEncoding =
      Encodings.GetEncoding("us-ascii", true);

    /**
     * Gets the name of this media type's top-level type (such as "text" or
     * "audio").
     * @return The name of this media type's top-level type (such as "text" or
     * "audio".
     */
    public final String getTopLevelType() {
        return this.topLevelType;
      }

    /**
     * Determines whether this object and another object are equal.
     * @param obj The parameter {@code obj} is an arbitrary object.
     * @return {@code true} if this object and another object are equal; otherwise,
     * {@code false}.
     */
    @Override public boolean equals(Object obj) {
      MediaType other = ((obj instanceof MediaType) ? (MediaType)obj : null);
      if (other == null) {
        return false;
      }
      return this.topLevelType.equals(other.topLevelType) &&
        this.subType.equals(other.subType) &&
          CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /**
     * Returns the hash code for this instance.
     * @return A 32-bit signed integer.
     */
    @Override public int hashCode() {
      int valueHashCode = 632580499;
      if (this.topLevelType != null) {
        valueHashCode = (valueHashCode + (632580503 *
                  this.topLevelType.hashCode()));
      }
      if (this.subType != null) {
        valueHashCode = (valueHashCode + (632580563 *
             this.subType.hashCode()));
      }
      if (this.parameters != null) {
        valueHashCode = (valueHashCode + (632580587 * this.parameters.size()));
      }
      return valueHashCode;
    }

    private final String subType;

    /**
     * Gets this media type's subtype.
     * @return This media type's subtype.
     */
    public final String getSubType() {
        return this.subType;
      }

    /**
     * Gets a value indicating whether this is a text media type ("text/*").
     * @return {@code true} If this is a text media type; otherwise, {@code false}.
     */
    public final boolean isText() {
        return this.getTopLevelType().equals("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return {@code true} If this is a multipart media type; otherwise, {@code
     * false}.
     */
    public final boolean isMultipart() {
        return this.getTopLevelType().equals("multipart");
      }

    MediaType(
  String type,
 String subtype,
 Map<String, String> parameters) {
      this.topLevelType = type;
      this.subType = subtype;
      this.parameters = new HashMap<String, String>(parameters);
    }

    private final HashMap<String, String> parameters;

    /**
     * Gets a list of the parameters contained in this media type object.
     * @return A list of the parameters contained in this media type object. NOTE:
     * Previous versions erroneously stated that the list will be sorted by
     * name. In fact, the names will not be guaranteed to appear in any
     * particular order; this is at least the case in version 0.10.0.
     */
    public final Map<String, String> getParameters() {
        return java.util.Collections.unmodifiableMap(this.parameters);
      }

    enum QuotedStringRule {
    /**
     * Use HTTP rules for quoted strings.
     */
      Http,

    /**
     * Use Internet Message Format rules for quoted strings.
     */
      Rfc5322
    }

    private static int SkipQtextOrQuotedPair(
      String s,
      int index,
      int endIndex,
      QuotedStringRule rule) {
      if (index >= endIndex) {
        return index;
      }
      int i2;
      if (rule == QuotedStringRule.Http) {
        char c = s.charAt(index);
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
          char c = s.charAt(i2);
          // Non-ASCII (allowed in internationalized email headers under
          // RFC6532)
          if ((c & 0xfc00) == 0xd800 && i2 + 1 < endIndex && s.charAt(i2 + 1) >=
            0xdc00 && s.charAt(i2 + 1) <= 0xdfff) {
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
      throw new IllegalArgumentException(rule.toString());
    }

    // quoted-pair (RFC5322 sec. 3.2.1)
    static int SkipQuotedPair(String s, int index, int endIndex) {
      if (index + 1 < endIndex && s.charAt(index) == '\\') {
        char c = s.charAt(index + 1);
        // Non-ASCII (allowed in internationalized email headers under RFC6532)
        if ((c & 0xfc00) == 0xd800 && index + 2 < endIndex && s.charAt(index + 2)
          >= 0xdc00 && s.charAt(index + 2) <= 0xdfff) {
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

    // quoted-String (RFC5322 sec. 3.2.4)
    static int SkipQuotedString(
  String s,
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
  String str,
  int index,
  int endIndex,
  StringBuilder sb) {
      while (index < endIndex) {
        int tmp = index;
        // Skip CRLF
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) == 10) {
          index += 2;
        }
        // Add WSP
        if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
          if (sb != null) {
            sb.append(str.charAt(index));
          }
          ++index;
        } else {
          return tmp;
        }
      }
      return index;
    }

    private static int SkipQuotedString(
  String str,
  int index,
  int endIndex,
  StringBuilder builder,  // receives the unescaped version of the String
  QuotedStringRule rule) {
      int startIndex = index;
      int valueBLength = (builder == null) ? 0 : builder.length();
      index = (rule != QuotedStringRule.Rfc5322) ? index :
        HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (!(index < endIndex && str.charAt(index) == '"')) {
        if (builder != null) {
          builder.setLength(valueBLength);
        }
        return startIndex;  // not a valid quoted-String
      }
      ++index;
      while (index < endIndex) {
        int i2 = index;
        if (rule == QuotedStringRule.Http) {
          if (str.charAt(index) == ' ' || str.charAt(index) == '\t') {
            if (builder != null) {
              builder.append(str.charAt(index));
            }
            ++index;
            continue;
          }
        } else if (rule == QuotedStringRule.Rfc5322) {
          // Skip tabs, spaces, and folding whitespace
          i2 = ParseFWSLax(str, index, endIndex, builder);
        }
        index = i2;
        char c = str.charAt(index);
        if (c == '"') {  // end of quoted-String
          ++index;
          // NOTE: Don't skip CFWS even if the rule is Rfc5322
          return index;
        }
        int oldIndex = index;
        index = SkipQtextOrQuotedPair(str, index, endIndex, rule);
        if (index == oldIndex) {
          if (builder != null) {
            builder.delete(valueBLength, (valueBLength)+((builder.length()) - valueBLength));
          }
          return startIndex;
        }
        if (builder != null) {
          // this is a qtext or quoted-pair, so
          // append the last character read
          builder.append(str.charAt(index - 1));
        }
      }
      if (builder != null) {
        builder.delete(valueBLength, (valueBLength)+((builder.length()) - valueBLength));
      }
      return startIndex;  // not a valid quoted-String
    }

    private static boolean IsAttributeChar(int c) {
      return c >= 33 && c <= 126 && AttrNameSpecials.indexOf((char)c) < 0;
    }
    private static void PctAppend(StringBuilder sb, int w) {
      // NOTE: Use uppercase hex characters
      // to encode according to RFC 2231, but the augmented
      // BNF for ext-octet in that RFC allows both upper-case
      // and lower-case, even though only upper-case
      // appears in that production. This
      // is due to the nature of augmented BNF (see RFC
      // 5234 sec 2.3).
      sb.append('%');
      sb.append(ValueHex.charAt((w >> 4) & 15));
      sb.append(ValueHex.charAt(w & 15));
    }

    private static int EncodeContinuation(String str, int startPos,
      HeaderEncoder sa) {
      int column = sa.GetColumn();
      int maxLineLength = sa.GetMaxLineLength();
      int index = startPos;
      StringBuilder sb = new StringBuilder();
 while (index < str.length() && (maxLineLength < 0 || column <=
        maxLineLength)) {
        int c = str.charAt(index);
        boolean first = (index == 0);
        int contin = (index == 0) ? 7 : 0;
        if ((c & 0xfc00) == 0xd800 && index + 1 < str.length() &&
            str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
          c = 0x10000 + ((c - 0xd800) << 10) + (str.charAt(index + 1) - 0xdc00);
        } else if ((c & 0xf800) == 0xd800) {
          c = 0xfffd;
        }
        if (IsAttributeChar(c)) {
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
          sb.append("utf-8''");
        }
        if (IsAttributeChar(c)) {
          sb.append((char)c);
        } else if (c <= 0x7f) {
          PctAppend(sb, c);
        } else if (c <= 0x7ff) {
          PctAppend(sb, (0xc0 | ((c >> 6) & 0x1f)));
          PctAppend(sb, (0x80 | (c & 0x3f)));
        } else if (c <= 0xffff) {
          PctAppend(sb, (0xe0 | ((c >> 12) & 0x0f)));
          PctAppend(sb, (0x80 | ((c >> 6) & 0x3f)));
          PctAppend(sb, (0x80 | (c & 0x3f)));
        } else {
          PctAppend(sb, (0xf0 | ((c >> 18) & 0x07)));
          PctAppend(sb, (0x80 | ((c >> 12) & 0x3f)));
          PctAppend(sb, (0x80 | ((c >> 6) & 0x3f)));
          PctAppend(sb, (0x80 | (c & 0x3f)));
          ++index;
        }
        ++index;
        column += contin;
      }
      if (maxLineLength >= 0 && index == startPos) {
        // No room to put any continuation here;
        // add a line break and try again
        sa.AppendBreak();
        return EncodeContinuation(str, startPos, sa);
      }
      sa.AppendSymbol(sb.toString());
      return index;
    }

    private static void AppendComplexParamValue(
  String name,
  String str,
  HeaderEncoder sa) {
      int column = sa.GetColumn();
      // Check if parameter is short enough for the column that
      // no continuations are needed
      int continColumn = column + name.length() + 9;
      int oldcolumn = sa.GetColumn();
      int oldlength = sa.GetLength();
      sa.AppendSymbol(name + "*").AppendSymbol("=");
      if (EncodeContinuation(str, 0, sa) != str.length()) {
        sa.Reset(oldcolumn, oldlength);
        int contin = 0;
        int index = 0;
        while (index < str.length()) {
          if (contin > 0) {
            sa.AppendSymbol(";");
          }
          sa.AppendSymbol(name + "*" + ParserUtility.IntToString(contin) + "*")
     .AppendSymbol("=");
          index = EncodeContinuation(str, index, sa);
          ++contin;
        }
      }
    }

    private static boolean AppendSimpleParamValue(
  String name,
  String str,
  HeaderEncoder sa) {
      sa.AppendSymbol(name);
      sa.AppendSymbol("=");
      if (str.length() == 0) {
        sa.AppendSymbol("\"\"");
        return true;
      }
      boolean simple = true;
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (!(c >= 33 && c <= 126 && "()<>,;[]:@\"\\/?=".indexOf(c) < 0)) {
          simple = false;
        }
      }
      if (simple) {
        return sa.TryAppendSymbol(str);
      }
      StringBuilder sb = new StringBuilder();
      sb.append('"');
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (c >= 32 && c <= 126 && c != '\\' && c != '"') {
          sb.append(c);
        } else if (c == 0x09 || c == '\\' || c == '"') {
          sb.append('\\');
          sb.append(c);
        } else {
          // Requires complex encoding
          return false;
        }
        if (sa.GetMaxLineLength() >= 0 && sb.length() > sa.GetMaxLineLength()) {
          // Too long to fit (optimization for very
          // long parameter values)
          return false;
        }
      }
      sb.append('"');
      return sa.TryAppendSymbol(sb.toString());
    }

    static void AppendParameters(
      Map<String, String> parameters,
      HeaderEncoder sa) {
      ArrayList<String> keylist = new ArrayList<String>(parameters.keySet());
      java.util.Collections.sort(keylist);
      for (String key : keylist) {
        String name = key;
        String value = parameters.get(key);
        sa.AppendSymbol(";");
        int oldcolumn = sa.GetColumn();
        int oldlength = sa.GetLength();
        if (!AppendSimpleParamValue(name, value, sa)) {
          sa.Reset(oldcolumn, oldlength);
          AppendComplexParamValue(name, value, sa);
        }
      }
    }

    /**
     * Converts this media type to a text string form suitable for inserting in
     * email headers. Notably, the string contains the value of a
     * Content-Type header field (without the text necessarily starting with
     * "Content-Type" followed by a space), and consists of one or more
     * lines.
     * @return A text string form of this media type.
     */
    @Override public String toString() {
      // NOTE: 76 is the maximum length of a line in an Internet
      // message header, and 14 is the length of "Content-Type: " (with trailing
      // space).
      HeaderEncoder sa = new HeaderEncoder(76, 14);
      sa.AppendSymbol(this.topLevelType + "/" + this.subType);
      AppendParameters(this.parameters, sa);
      return sa.toString();
    }

    /**
     * Converts this media type to a text string form suitable for inserting in
     * HTTP headers. Notably, the string contains the value of a
     * Content-Type header field (without the text necessarily starting with
     * "Content-Type" followed by a space), and consists of a single line.
     * @return A text string form of this media type.
     */
    public String ToSingleLineString() {
      // NOTE: 14 is the length of "Content-Type: " (with trailing space).
      HeaderEncoder sa = new HeaderEncoder(-1, 14);
      sa.AppendSymbol(this.topLevelType + "/" + this.subType);
      AppendParameters(this.parameters, sa);
      return sa.toString();
    }

    static int SkipMimeToken(
  String str,
  int index,
  int endIndex,
  StringBuilder builder,
  boolean httpRules) {
      int i = index;
      String ValueSpecials = "()<>@,;:\\\"/[]?=";
      while (i < endIndex) {
        char c = str.charAt(i);
        if (c <= 0x20 || c >= 0x7f || (c == (c & 0x7f) &&
           ValueSpecials.indexOf(c) >= 0)) {
          break;
        }
        if (httpRules && (c == '{' || c == '}')) {
          break;
        }
        if (builder != null) {
          builder.append(c);
        }
        ++i;
      }
      return i;
    }

    static int SkipAttributeNameRfc2231(
      String str,
      int index,
      int endIndex,
      StringBuilder builder,
      boolean httpRules) {
      if (httpRules) {
        return SkipMimeToken(str, index, endIndex, builder, httpRules);
      }
      int i = index;
      while (i < endIndex) {
        char c = str.charAt(i);
        if (c <= 0x20 || c >= 0x7f || ((c & 0x7f) == c &&
          AttrNameSpecials.indexOf(c) >= 0)) {
          break;
        }
        if (builder != null) {
          builder.append(c);
        }
        ++i;
      }
      if (i + 1 < endIndex && str.charAt(i) == '*' && str.charAt(i + 1) == '0') {
        // initial-section
        i += 2;
        if (builder != null) {
          builder.append("*0");
        }
        if (i < endIndex && str.charAt(i) == '*') {
          ++i;
          if (builder != null) {
            builder.append("*");
          }
        }
        return i;
      }
      if (i + 1 < endIndex && str.charAt(i) == '*' && str.charAt(i + 1) >= '1' && str.charAt(i +
        1) <= '9') {
        // other-sections
        if (builder != null) {
          builder.append('*');
          builder.append(str.charAt(i + 1));
        }
        i += 2;
        while (i < endIndex && str.charAt(i) >= '0' && str.charAt(i) <= '9') {
          if (builder != null) {
            builder.append(str.charAt(i));
          }
          ++i;
        }
        if (i < endIndex && str.charAt(i) == '*') {
          if (builder != null) {
            builder.append(str.charAt(i));
          }
          ++i;
        }
        return i;
      }
      if (i < endIndex && str.charAt(i) == '*') {
        if (builder != null) {
          builder.append(str.charAt(i));
        }
        ++i;
      }
      return i;
    }

    static int SkipMimeTypeSubtype(
  String str,
  int index,
  int endIndex,
  StringBuilder builder) {
      int i = index;
      int count = 0;
      String specials = "!#$&-^_.+";
      while (i < endIndex) {
        char c = str.charAt(i);
        // See RFC6838
        if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' &&
          c <= '9')) {
          if (builder != null) {
            builder.append(c);
          }
          ++i;
          ++count;
        } else if (count > 0 && (c == (c & 0x7f) && specials.indexOf(c) >=
              0)) {
          if (builder != null) {
            builder.append(c);
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

    /**
     * Gets this media type's "charset" parameter, naming a character encoding used
     * to represent text in the data that uses this media type.
     * @return If the "charset" parameter exists, returns that parameter with the
     * basic upper-case letters A to Z (U + 0041 to U + 005A) converted to lower
     * case. Returns {@code "us-ascii"} instead if the media type is
     * ill-formed (RFC2045 sec. 5.2), or if the media type is "text/plain"
     * and doesn't have a "charset" parameter (see RFC2046), or the default
     * value for that parameter, if any, for the media type if the "charset"
     * parameter is absent. Returns an empty string in all other cases.
     */

    public String GetCharset() {
      // NOTE: RFC6657 changed the rules for the default charset in text
      // media types,
      // so that there is no default charset for as yet undefined media
      // types. However,
      // media types defined before this RFC (July 2012) are grandfathered
      // from the rule: those
      // media types "that fail to specify how the charset is determined" still
      // have US-ASCII as default. The text media types defined as of
      // Jul. 11, 2018, are listed below:
      //
      // -- No default charset assumed: --
      //
      // RTP payload types; these are usually unsuitable for MIME,
      // and don't permit a charset parameter, so a default charset is
      // irrelevant:
      // -- 1d-interleaved-parityfec, fwdred, red, parityfec, encaprtp,
      // raptorfec, rtp-enc-aescm128, t140, ulpfec, rtx, rtploopback
      //
      // These media types don't define a charset parameter:
      // -- csv-schema, dns, grammar-ref-list, mizar, vnd.latex-z,
      // vnd.motorola.reflex,
      // vnd.si.uricatalogue, prs.lines.tag, vnd.dmclientscript,
      // vnd.dvb.subtitle,
      // vnd.fly, rtf, rfc822-headers, prs.prop.logic, vnd.ascii-art****,
      // vnd.hgl*(6), vnd.gml
      //
      // Special procedure defined for charset detection:
      // -- ecmascript, javascript, html
      //
      // XML formats (no default assumed if charset is absent, according
      // to RFC7303, the revision of the XML media type specification):
      // -- xml, xml-external-parsed-entity,
      // vnd.in3d.3dml*, vnd.iptc.newsml, vnd.iptc.nitf, vnd.ms-mediapackage,
      // vnd.net2phone.commcenter.command, vnd.radisys.msml-basic-layout,
      // vnd.wap.si, vnd.wap.sl, vnd.wap.wml
      //
      // Behavior deliberately undefined (so whether US-ASCII or another
      // charset is treated as default is irrelevant):
      // -- example
      //
      // Uses charset parameter, but no default charset specified (after
      // RFC6657):
      // -- markdown*
      //
      // -- US-ASCII assumed: --
      //
      // These media types don't define a default charset:
      // -- css, richtext, enriched, tab-separated-values,
      // vnd.in3d.spot*, vnd.abc, vnd.wap.wmlscript, vnd.curl,
      // vnd.fmi.flexstor, uri-list, directory
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
      // vnd.graphviz, vnd.sun.j2me.app-descriptor, strings*(5)
      //
      // * Required parameter.
      // ** No explicit default, but says that "[t]he charset supported
      // by this revision of iCalendar is UTF-8."
      // *(5) No charset parameter defined.
      // *(6) 8-bit encoding.
      // *** Default is UTF-8 "if 8-bit bytes are encountered" (even if
      // none are found, though, a 7-bit ASCII text is still also UTF-8).
      // **** Content containing non-ASCII bytes "should be rejected".
      String param = this.GetParameter("charset");
      if (param != null) {
        return DataUtilities.ToLowerCaseAscii(param);
      }
      if (this.isText()) {
        String sub = this.getSubType();
        // Media types that assume a default of US-ASCII
        if (sub.equals("plain") || sub.equals("sgml") ||
          sub.equals("troff") || sub.equals("directory") ||
          sub.equals("css") || sub.equals("richtext") ||
              sub.equals("enriched") || sub.equals("tab-separated-values") ||
              sub.equals("vnd.in3d.spot") || sub.equals("vnd.abc") ||
            sub.equals("vnd.wap.wmlscript") || sub.equals("vnd.curl") ||
              sub.equals("vnd.fmi.flexstor") || sub.equals("uri-list")) {
          return "us-ascii";
        }
        // Media types that assume a default of UTF-8
        if (sub.equals("vcard") || sub.equals("jcr-cnd") ||
          sub.equals("n3") || sub.equals("turtle") ||
  sub.equals("strings") || sub.equals("vnd.debian.copyright") ||
              sub.equals("provenance-notation") || sub.equals("csv") ||
   sub.equals("calendar") || sub.equals("vnd.a") ||
              sub.equals("parameters") || sub.equals("prs.fallenstein.rst") ||
              sub.equals("vnd.esmertec.theme.descriptor") ||
            sub.equals("vnd.trolltech.linguist") ||
              sub.equals("vnd.graphviz") || sub.equals("cache-manifest") ||
              sub.equals("vnd.sun.j2me.app-descriptor")) {
          return "utf-8";
        }
      }
      return "";
    }

    /**
     * Gets the value of a parameter in this media type, such as "charset" or
     * "format".
     * @param name Name of the parameter to get. The name is compared using a basic
     * case-insensitive comparison. (Two strings are equal in such a
     * comparison, if they match after converting the basic upper-case
     * letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).
     * @return The value of the parameter as a string, or null if the parameter
     * doesn't exist.
     * @throws java.lang.NullPointerException The parameter {@code name} is null.
     */
    public String GetParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (name.length() == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return this.parameters.containsKey(name) ? this.parameters.get(name) : null;
    }

    private static String DecodeRfc2231Extension(String value, boolean httpRules) {
      int firstQuote = value.indexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      int secondQuote = value.indexOf('\'',firstQuote + 1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      String charset = value.substring(0, firstQuote);
      if (httpRules && charset.length() == 0) {
        // charset is omitted, which is not allowed under RFC8187
        return null;
      }
      String language = value.substring(
  firstQuote + 1, (
  firstQuote + 1)+(secondQuote - (firstQuote + 1)));
      if (language.length() > 0 && !ParserUtility.IsValidLanguageTag(language)) {
        // not a valid language tag
        return null;
      }
      String paramValue = value.substring(secondQuote + 1);
      // NOTE: For HTTP (RFC 8187) no specific error-handling
      // behavior is mandated for "encoding errors", which can
      // be interpreted as including unsupported or unrecognized
      // character encodings (see sec. 3.2.1).
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = (cs == null) ? (USAsciiEncoding) : cs;
      return DecodeRfc2231Encoding(paramValue, cs);
    }

    private static ICharacterEncoding GetRfc2231Charset(String value) {
      // NOTE: Currently only used outside of httpRules
      if (value == null) {
        return USAsciiEncoding;
      }
      int firstQuote = value.indexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return USAsciiEncoding;
      }
      int secondQuote = value.indexOf('\'',firstQuote + 1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return USAsciiEncoding;
      }
      String charset = value.substring(0, firstQuote);
      String language = value.substring(
  firstQuote + 1, (
  firstQuote + 1)+(secondQuote - (firstQuote + 1)));
      if (language.length() > 0 && !ParserUtility.IsValidLanguageTag(language)) {
        // not a valid language tag
        return USAsciiEncoding;
      }
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = (cs == null) ? (USAsciiEncoding) : cs;
      return cs;
    }

    private static String DecodeRfc2231Encoding(
    String value,
    ICharacterEncoding charset) {
      // a value without a quote
      // mark is not a valid encoded parameter
      int quote = value.indexOf('\'');
      return (quote >= 0) ? null : Encodings.DecodeToString(
        charset,
        new PercentEncodingStringTransform(value));
    }

    private static boolean ExpandRfc2231Extensions(Map<String, String>
      parameters, boolean httpRules) {
      if (parameters.size() == 0) {
        return true;
      }
      // NOTE: RFC 2231 doesn't specify what happens if a "*" extension
      // and a "*0*" continuation both appear in the same media type String.
      // In this implementation, due to the sorting which follows,
      // the former will appear before the latter in the key list and
      // will generally be overridden by the latter in the
      // final parameter list.
      ArrayList<String> keyList = new ArrayList<String>(parameters.keySet());
      java.util.Collections.sort(keyList);
      for (String name : keyList) {
        if (!parameters.containsKey(name)) {
          continue;
        }
        String value = parameters.get(name);
        int asterisk = name.indexOf('*');
        if (asterisk == name.length() - 1 && asterisk > 0) {
          // name*="value" (except when the parameter is just "*")
          // NOTE: As of RFC 5987 (now RFC 8187), this particular extension
          // is now allowed in HTTP
          String realName = name.substring(0, name.length() - 1);
          String realValue = DecodeRfc2231Extension(value, httpRules);
          if (realValue == null) {
            continue;
          }
          parameters.remove(name);
          // NOTE: Overrides the name without continuations
          // (also suggested by RFC8187 sec. 4.2)
          parameters.put(realName, realValue);
          continue;
        }
        // name*0 or name*0*
        if (!httpRules && asterisk > 0 && ((asterisk == name.length() - 2 &&
          name.charAt(asterisk + 1) == '0') || (asterisk == name.length() - 3 &&
          name.charAt(asterisk + 1) == '0' && name.charAt(asterisk + 2) == '*'))) {
          String realName = name.substring(0, asterisk);
          // NOTE: 'httpRules' for DecodeRfc2231Extension is false
          String realValue = (asterisk == name.length() - 3) ?
            DecodeRfc2231Extension(value, false) : value;
          ICharacterEncoding charsetUsed = GetRfc2231Charset(
            (asterisk == name.length() - 3) ? value : null);
          parameters.remove(name);
          int pindex = 1;
          StringBuilder builder = new StringBuilder();
          builder.append(realValue);
          // search for name*1 or name*1*, then name*2 or name*2*,
          // and so on
          while (true) {
            String contin = realName + "*" +
              ParserUtility.IntToString(pindex);
            String continEncoded = contin + "*";
            if (parameters.containsKey(continEncoded)) {
              // Encoded continuation (checked first)
              String newEnc = DecodeRfc2231Encoding(
             parameters.get(continEncoded),
             charsetUsed);
              if (newEnc == null) {
                // Contains a quote character in the encoding, so illegal
                break;
              }
              builder.append(newEnc);
              parameters.remove(continEncoded);
            } else if (parameters.containsKey(contin)) {
              // Unencoded continuation (checked second)
              builder.append(parameters.get(contin));
              parameters.remove(contin);
            } else {
              break;
            }
            ++pindex;
          }
          realValue = builder.toString();
          // NOTE: Overrides the name without continuations
          parameters.put(realName, realValue);
        }
      }
      keyList = new ArrayList<String>(parameters.keySet());
      for (String name : keyList) {
        // Check parameter names using stricter format
        // in RFC6838
        if (SkipMimeTypeSubtype(name, 0, name.length(), null) != name.length()) {
          // Illegal parameter name, so use default media type
          //return false;
          parameters.remove(name);
        }
      }
      return true;
    }

    /**
     * Gets the top level type and subtype of this media type, separated by a
     * slash; for example, "text/plain".
     * @return The top level type and subtype of this media type, separated by a
     * slash; for example, "text/plain".
     */
    public final String getTypeAndSubType() {
        return this.getTopLevelType() + "/" + this.getSubType();
      }

    static int SkipOws(String s, int index, int endIndex) {
      int i2 = index;
      while (i2 < endIndex) {
        if (s.charAt(i2) == 0x09 || s.charAt(i2) == 0x20) {
          ++index;
        }
        break;
      }
      return index;
    }

    static boolean ParseParameters(
  String str,
      int index,
 int endIndex,
      boolean httpRules,
 Map<String, String> parameters) {
      HashMap<String, String> duplicateAttributes = new HashMap<String, String>();
      boolean hasDuplicateAttributes = false;
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
        if (str.charAt(index) != ';') {
          return false;
        }
        ++index;
        index = httpRules ? SkipOws(str, index, endIndex) :
              HeaderParser.ParseCFWS(
      str,
      index,
      endIndex,
      null);
        StringBuilder builder = new StringBuilder();
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
        if (afteratt == index) {  // ill-formed attribute
          return false;
        }
        String attribute = builder.toString();
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
        if (str.charAt(index) != '=') {
          return false;
        }
        attribute = DataUtilities.ToLowerCaseAscii(attribute);
        if (parameters.containsKey(attribute)) {
          parameters.remove(attribute);
          duplicateAttributes.put(attribute,"");
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
        builder.delete(0, (0)+(builder.length()));
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
            // String because of RFC2231; if this happens, ignore the attribute
            if (attribute.charAt(attribute.length() - 1) != '*' &&
     (!hasDuplicateAttributes || !duplicateAttributes.containsKey(attribute))) {
             parameters.put(attribute, builder.toString());
            }
            index = qs;
            continue;
          }
          builder.delete(0, (0)+(builder.length()));
        // try getting the value unquoted
        // Note we don't use getAtom
        qs = SkipMimeToken(str, index, endIndex, builder, httpRules);
        if (qs != index) {
    if (!hasDuplicateAttributes ||
            !duplicateAttributes.containsKey(attribute)) {
             parameters.put(attribute, builder.toString());
          }
          index = qs;
          continue;
        }
        // no valid value, return
        return false;
      }
    }

    private static MediaType ParseMediaType(String str) {
      boolean HttpRules = false;
      int index = 0;
      if (str == null) {
        throw new NullPointerException("str");
      }
      int endIndex = str.length();
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      int i = SkipMimeTypeSubtype(str, index, endIndex, null);
      if (i == index || i >= endIndex || str.charAt(i) != '/') {
        return null;
      }
      HashMap<String, String> parameters = new HashMap<String, String>();
      String topLevelType =
        DataUtilities.ToLowerCaseAscii(str.substring(index, (index)+(i - index)));
      ++i;
      int i2 = SkipMimeTypeSubtype(str, i, endIndex, null);
      if (i == i2) {
        return null;
      }
      String subType = DataUtilities.ToLowerCaseAscii(str.substring(i, (i)+(i2 - i)));
      if (i2 < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i2, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return new MediaType(topLevelType, subType, parameters);
        }
        if (i3 < endIndex && str.charAt(i3) != ';') {
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

    /**
     * Specifies the media type "text/plain" and the "charset" parameter
     * "US-ASCII", used for plain text data.
     */
    public static final MediaType TextPlainAscii =
      new MediaTypeBuilder(
  "text",
  "plain").SetParameter(
        "charset",
        "us-ascii").ToMediaType();

    /**
     * Specifies the media type "text/plain" and the "charset" parameter "utf-8",
     * used for plain text data that may contain characters outside the
     * basic Latin range (U + 0000 to U + 007F).
     */
    public static final MediaType TextPlainUtf8 =
      new MediaTypeBuilder(
  "text",
  "plain").SetParameter(
        "charset",
        "utf-8").ToMediaType();

    /**
     * Specifies the media type "message/rfc822", used for Internet mail messages.
     */
    public static final MediaType MessageRfc822 =
      new MediaTypeBuilder("message", "rfc822").ToMediaType();

    /**
     * Specifies the media type "application/octet-stream", used for arbitrary
     * binary data.
     */
    public static final MediaType ApplicationOctetStream =
      new MediaTypeBuilder("application", "octet-stream").ToMediaType();

    private MediaType() {
      this.topLevelType = "";
      this.subType = "";
      this.parameters = new HashMap<String, String>();
    }

    /**
     * Parses a media type string and returns a media type object. For further
     * information, see the overload taking a MediaType parameter.
     * @param mediaTypeValue A text string representing a media type. This media
     * type can include parameters.
     * @return A media type object, or MediaType.TextPlainAscii if {@code
     * mediaTypeValue} is empty or syntactically invalid.
     */
    public static MediaType Parse(String mediaTypeValue) {
      return Parse(mediaTypeValue, TextPlainAscii);
    }

    /**
     * Parses a media type string and returns a media type object, or the default
     * value if the string is invalid. This method checks the syntactic
     * validity of the string, but not whether it has all parameters it's
     * required to have or whether the parameters themselves are set to
     * valid values for the parameter. <p>RFC 2231 extensions allow each
     * media type parameter to be associated with a character encoding
     * and/or language, and support parameter values that span two or more
     * key-value pairs. Parameters making use of RFC 2231 extensions have
     * names with an asterisk ("*"). Such a parameter will be ignored if it
     * is ill-formed because of RFC 2231's rules (except for illegal
     * percent-decoding or undecodable sequences for the given character
     * enoding). Examples of RFC 2231 extensions follow (both examples
     * encode the same "filename" parameter):</p> <p><b>text/example;
     * filename*=utf-8'en'filename.txt</b></p> <p><b>text/example;
     * filename*0*=utf-8'en'file; filename*1*=name%2Etxt</b></p> <p>This
     * implementation ignores keys (in parameter key-value pairs) that
     * appear more than once in the media type. Nothing in RFCs 2045, 2183,
     * 6266, or 7231 explicitly disallows such keys, or otherwise specifies
     * error-handling behavior for such keys.</p>
     * @param str A text string representing a media type. This media type can
     * include parameters.
     * @param defaultValue The media type to return if the string is syntactically
     * invalid. Can be null.
     * @return A MediaType object.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     */
    public static MediaType Parse(String str, MediaType defaultValue) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      MediaType mt = ParseMediaType(str);
      return (mt == null) ? (defaultValue) : mt;
    }
  }
