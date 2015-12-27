package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
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
     * ("charset=utf-8"). Other top-level types include "audio", "video",
     * and "application".</p> <p>A media type is sometimes known as a "MIME
     * type", for Multipurpose Internet Mail Extensions, the standard that
     * introduced media types.</p> <p>This type is immutable, meaning its
     * values can't be changed once it' s created. To create a changeable
     * media type object, use the MediaTypeBuilder class.</p>
     */
  public final class MediaType {
    private String topLevelType;

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
     * @param obj An arbitrary object.
     * @return True if this object and another object are equal; otherwise, false.
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
  valueHashCode = (valueHashCode + 632580503 *
            this.topLevelType.hashCode());
        }
        if (this.subType != null) {
       valueHashCode = (valueHashCode + 632580563 *
            this.subType.hashCode());
        }
        if (this.parameters != null) {
          valueHashCode = (valueHashCode + 632580587 * this.parameters.size());
        }
      return valueHashCode;
    }

    private String subType;

    /**
     * Gets this media type's subtype.
     * @return This media type's subtype.
     */
    public final String getSubType() {
        return this.subType;
      }

    /**
     * Gets a value indicating whether this is a text media type ("text/*").
     * @return True if this is a text media type; otherwise, false.
     */
    public final boolean isText() {
        return this.getTopLevelType().equals("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return True if this is a multipart media type; otherwise, false.
     */
    public final boolean isMultipart() {
        return this.getTopLevelType().equals("multipart");
      }

    MediaType(
String type,
 String subtype,
 Map<String,
      String> parameters) {
      this.topLevelType = type;
      this.subType = subtype;
      this.parameters = new TreeMap<String, String>(parameters);
    }

    private TreeMap<String, String> parameters;

    /**
     * Gets a sorted list of the parameters contained in this media type object.
     * @return A list of the parameters contained in this media type object, sorted
     * by name.
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

    private static int skipQtextOrQuotedPair(
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
        i2 = skipQuotedPair(s, index, endIndex);
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
    static int skipQuotedPair(String s, int index, int endIndex) {
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
    static int skipQuotedString(
String s,
int index,
int endIndex,
StringBuilder builder) {
return skipQuotedString(
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
    private static int skipQuotedString(
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
        if (c == '"') { // end of quoted-String
          ++index;
          // NOTE: Don't skip CFWS even if the rule is Rfc5322
          return index;
        }
        int oldIndex = index;
        index = skipQtextOrQuotedPair(str, index, endIndex, rule);
        if (index == oldIndex) {
          if (builder != null) {
            builder.delete(valueBLength, (valueBLength)+((builder.length())-valueBLength));
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
        builder.delete(valueBLength, (valueBLength)+((builder.length())-valueBLength));
      }
      return startIndex;  // not a valid quoted-String
    }

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (int i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }

    private static String valueDigits = "0123456789";

    private static String IntToString(int value) {
      if (value == Integer.MIN_VALUE) {
        return "-2147483648";
      }
      if (value == 0) {
        return "0";
      }
      boolean neg = value < 0;
      char[] chars = new char[24];
      int count = 0;
      if (neg) {
        chars[0] = '-';
        ++count;
        value = -value;
      }
      while (value != 0) {
        char digit = valueDigits.charAt((int)(value % 10));
        chars[count++] = digit;
        value /= 10;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    private static void AppendComplexParamValue(
String name,
String str,
StringBuilder sb) {
      int length = 1;
      int contin = 0;
      String ValueHex = "0123456789ABCDEF";
      length += name.length() + 12;
      int MaxLength = 76;
      if (sb.length() + name.length() + 9 + (str.length() * 3) <= MaxLength) {
        // Very short
        length = sb.length() + name.length() + 9;
        sb.append(name + "*=utf-8''");
      } else if (length + (str.length() * 3) <= MaxLength) {
        // Short enough that no continuations
        // are needed
        length -= 2;
        sb.append(name + "*=utf-8''");
      } else {
        sb.append(name + "*0*=utf-8''");
      }
      boolean first = true;
      int index = 0;
      while (index < str.length()) {
        int c = str.charAt(index);
        if ((c & 0xfc00) == 0xd800 && index + 1 < str.length() &&
            str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) << 10) + (str.charAt(index + 1) - 0xdc00);
          ++index;
        } else if ((c & 0xf800) == 0xd800) {
          // unpaired surrogate
          c = 0xfffd;
        }
        ++index;
      if (c >= 33 && c <= 126 && "()<>,;[]:@\"\\/?=*%'"
        .indexOf((char)c) < 0) {
          ++length;
          if (!first && length + 1 > MaxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString = name + "*" + IntToString(contin) + "*=";
            sb.append(continString);
            length = 1 + continString.length();
            ++length;
          }
          first = false;
          sb.append((char)c);
        } else if (c < 0x80) {
          length += 3;
          if (!first && length + 1 > MaxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString = name + "*" + IntToString(contin) +
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length += 3;
          }
          first = false;
          sb.append('%');
          sb.append(ValueHex.charAt((c >> 4) & 15));
          sb.append(ValueHex.charAt(c & 15));
        } else if (c < 0x800) {
          length += 6;
          if (!first && length + 1 > MaxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString = name + "*" + IntToString(contin) +
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length += 6;
          }
          first = false;
          int w = (byte)(0xc0 | ((c >> 6) & 0x1f));
          int x = (byte)(0x80 | (c & 0x3f));
          sb.append('%');
          sb.append(ValueHex.charAt((w >> 4) & 15));
          sb.append(ValueHex.charAt(w & 15));
          sb.append('%');
          sb.append(ValueHex.charAt((x >> 4) & 15));
          sb.append(ValueHex.charAt(x & 15));
        } else if (c < 0x10000) {
          length += 9;
          if (!first && length + 1 > MaxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString = name + "*" + IntToString(contin) +
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length += 9;
          }
          first = false;
          int w = (byte)(0xe0 | ((c >> 12) & 0x0f));
          int x = (byte)(0x80 | ((c >> 6) & 0x3f));
          int y = (byte)(0x80 | (c & 0x3f));
          sb.append('%');
          sb.append(ValueHex.charAt((w >> 4) & 15));
          sb.append(ValueHex.charAt(w & 15));
          sb.append('%');
          sb.append(ValueHex.charAt((x >> 4) & 15));
          sb.append(ValueHex.charAt(x & 15));
          sb.append('%');
          sb.append(ValueHex.charAt((y >> 4) & 15));
          sb.append(ValueHex.charAt(y & 15));
        } else {
          length += 12;
          if (!first && length + 1 > MaxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString = name + "*" + IntToString(contin) + "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length += 12;
          }
          first = false;
          int w = (byte)(0xf0 | ((c >> 18) & 0x07));
          int x = (byte)(0x80 | ((c >> 12) & 0x3f));
          int y = (byte)(0x80 | ((c >> 6) & 0x3f));
          int z = (byte)(0x80 | (c & 0x3f));
          sb.append('%');
          sb.append(ValueHex.charAt((w >> 4) & 15));
          sb.append(ValueHex.charAt(w & 15));
          sb.append('%');
          sb.append(ValueHex.charAt((x >> 4) & 15));
          sb.append(ValueHex.charAt(x & 15));
          sb.append('%');
          sb.append(ValueHex.charAt((y >> 4) & 15));
          sb.append(ValueHex.charAt(y & 15));
          sb.append('%');
          sb.append(ValueHex.charAt((z >> 4) & 15));
          sb.append(ValueHex.charAt(z & 15));
        }
      }
    }

    private static boolean AppendSimpleParamValue(
String name,
String str,
StringBuilder sb) {
      sb.append(name);
      sb.append('=');
      if (str.length() == 0) {
        sb.append("\"\"");
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
        sb.append(str);
        return true;
      }
      sb.append('"');
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (c >= 32 && c <= 126 && c != '\\' && c != '"') {
          sb.append(c);
        } else if (c == 0x20 || c == 0x09 || c == '\\' || c == '"') {
          sb.append('\\');
          sb.append(c);
        } else {
          // Requires complex encoding
          return false;
        }
      }
      sb.append('"');
      return true;
    }

    static int LastLineStart(StringBuilder sb) {
      String valueSbString = sb.toString();
      for (int i = sb.length() - 1; i >= 0; --i) {
        if (valueSbString.charAt(i) == '\n') {
          return i + 1;
        }
      }
      return 0;
    }

    static void AppendParameters(
Map<String, String>
      parameters,
 StringBuilder sb) {
      StringBuilder tmp = new StringBuilder();
      for (String key : parameters.keySet()) {
        int lineIndex = LastLineStart(sb);
        String name = key;
        String value = parameters.get(key);
        sb.append(';');
        tmp.setLength(0);
        if (!AppendSimpleParamValue(name, value, tmp)) {
          tmp.setLength(0);
          AppendComplexParamValue(name, value, tmp);
       if ((sb.length() - lineIndex) + tmp.length() > (lineIndex == 0 ? 76 :
            75)) {
            sb.append("\r\n ");
          }
          sb.append(tmp);
        } else {
       if ((sb.length() - lineIndex) + tmp.length() > (lineIndex == 0 ? 76 :
            75)) {
            sb.append("\r\n ");
          }
          sb.append(tmp);
        }
      }
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      StringBuilder sb = new StringBuilder();
      sb.append(this.topLevelType);
      sb.append('/');
      sb.append(this.subType);
      AppendParameters(this.parameters, sb);
      return sb.toString();
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
          "()<>@,;:\\\"/[]?='%*" .indexOf(c) >= 0)) {
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

    static int skipMimeTypeSubtype(
String str,
int index,
int endIndex,
StringBuilder builder) {
      int i = index;
      int count = 0;
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
    } else if (count > 0 && (c == (c & 0x7F) && "!#$&-^_.+" .indexOf(c) >=
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
     * case. Returns "us-ascii" instead if the media type is ill-formed
     * (RFC2045 sec. 5.2), or if the media type is "text/plain" and doesn't
     * have a "charset" parameter (see RFC2046), or the default value for
     * that parameter, if any, for the media type if the "charset" parameter
     * is absent. Returns an empty string in all other cases.
     */

    public String GetCharset() {
      // NOTE: RFC6657 changed the rules for the default charset in text
      // media types,
      // so that there is no default charset for as yet undefined media
      // types. However,
      // media types defined before this RFC are grandfathered from the
      // rule: those
      // media types "that fail to specify how the charset is determined" still
      // have US-ASCII as default. The text media types defined as of Nov. 20,
      // 2015, are listed below:
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
      // vnd.fly, rtf, rfc822-headers
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
      // No default specified (after RFC6657):
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
      // Charset parameter defined but is "always UTF-8":
      // -- n3, turtle, vnd.debian.copyright, provenance-notation
      //
      // UTF-8 default:
      // -- csv, calendar**, vnd.a***, parameters, prs.fallenstein.rst,
      // vnd.esmertec.theme.descriptor, vnd.trolltech.linguist,
      // vnd.graphviz, vnd.sun.j2me.app-descriptor
      //
      // * Required parameter.
      // ** No explicit default, but says that "[t]he charset supported
      // by this revision of iCalendar is UTF-8."
      // *** Default is UTF-8 "if 8-bit bytes are encountered" (even if
      // none are found, though, a 7-bit ASCII text is still also UTF-8)
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
            sub.equals("vnd.debian.copyright") ||
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
     * @param name Name of the parameter to get. The name is compared
     * case-insensitively.
     * @return The value of the parameter as a string, or null if the parameter
     * doesn't exist.
     * @throws NullPointerException The parameter {@code name} is null.
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

    static String DecodeRfc2231Extension(String value) {
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
      String language = value.substring(
firstQuote + 1, (
firstQuote + 1)+(secondQuote - (firstQuote + 1)));
      if (language.length() > 0 && !ParserUtility.IsValidLanguageTag(language)) {
        // not a valid language tag
        return null;
      }
      String paramValue = value.substring(secondQuote + 1);
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = (cs == null) ? (Encodings.GetEncoding("us-ascii", true)) : cs;
      return DecodeRfc2231Encoding(paramValue, cs);
    }

    private static ICharacterEncoding GetRfc2231Charset(String value) {
      if (value == null) {
        return Encodings.GetEncoding("us-ascii", true);
      }
      int firstQuote = value.indexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return Encodings.GetEncoding("us-ascii", true);
      }
      int secondQuote = value.indexOf('\'',firstQuote + 1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return Encodings.GetEncoding("us-ascii", true);
      }
      String charset = value.substring(0, firstQuote);
      String language = value.substring(
firstQuote + 1, (
firstQuote + 1)+(secondQuote - (firstQuote + 1)));
      if (language.length() > 0 && !ParserUtility.IsValidLanguageTag(language)) {
        // not a valid language tag
        return Encodings.GetEncoding("us-ascii", true);
      }
      ICharacterEncoding cs = Encodings.GetEncoding(charset, true);
      cs = (cs == null) ? (Encodings.GetEncoding("us-ascii", true)) : cs;
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
      parameters) {
      if (parameters.size() == 0) {
        return true;
      }
      List<String> keyList = new ArrayList<String>(parameters.keySet());
      for (String name : keyList) {
        if (!parameters.containsKey(name)) {
          continue;
        }
        String value = parameters.get(name);
        int asterisk = name.indexOf('*');
        if (asterisk == name.length() - 1 && asterisk > 0) {
          // name*="value" (except when the parameter is just "*")
          String realName = name.substring(0, name.length() - 1);
          String realValue = DecodeRfc2231Extension(value);
          if (realValue == null) {
            continue;
          }
          parameters.remove(name);
          // NOTE: Overrides the name without continuations
          parameters.put(realName, realValue);
          continue;
        }
        // name*0 or name*0*
        if (asterisk > 0 && ((asterisk == name.length() - 2 &&
          name.charAt(asterisk + 1) == '0') || (asterisk == name.length() - 3 &&
          name.charAt(asterisk + 1) == '0' && name.charAt(asterisk + 2) == '*'))) {
          String realName = name.substring(0, asterisk);
          String realValue = (asterisk == name.length() - 3) ?
            DecodeRfc2231Extension(value) : value;
          ICharacterEncoding charsetUsed = GetRfc2231Charset(
            (asterisk == name.length() - 3) ? value : null);
          parameters.remove(name);
          int pindex = 1;
          StringBuilder builder = new StringBuilder();
          builder.append(realValue);
          // search for name*1 or name*1*, then name*2 or name*2*,
          // and so on
          while (true) {
            String contin = realName + "*" + IntToString(pindex);
            String continEncoded = contin + "*";
            if (parameters.containsKey(contin)) {
              // Unencoded continuation
              builder.append(parameters.get(contin));
              parameters.remove(contin);
            } else if (parameters.containsKey(continEncoded)) {
              // Encoded continuation
              String newEnc = DecodeRfc2231Encoding(
             parameters.get(continEncoded),
             charsetUsed);
              if (newEnc == null) {
                // Contains a quote character in the encoding, so illegal
                return false;
              }
              builder.append(newEnc);
              parameters.remove(continEncoded);
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
      for (String name : parameters.keySet()) {
        // Check parameter names using stricter format
        // in RFC6838
        if (skipMimeTypeSubtype(name, 0, name.length(), null) != name.length()) {
          // Illegal parameter name, so use default media type
          return false;
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

    static int skipOws(String s, int index, int endIndex) {
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
      while (true) {
        // RFC5322 uses ParseCFWS when skipping whitespace;
        // HTTP currently uses skipOws
    index = httpRules ? skipOws(str, index, endIndex) :
          HeaderParser.ParseCFWS(
            str,
            index,
            endIndex,
            null);
        if (index >= endIndex) {
          // No more parameters
          return httpRules || ExpandRfc2231Extensions(parameters);
        }
        if (str.charAt(index) != ';') {
          return false;
        }
        ++index;
    index = httpRules ? skipOws(str, index, endIndex) :
          HeaderParser.ParseCFWS(
str,
index,
endIndex,
null);
        StringBuilder builder = new StringBuilder();
        // NOTE: RFC6838 restricts the format of parameter names to the same
        // syntax as types and subtypes, but this syntax is incompatible with
        // the RFC2231 format
        int afteratt = SkipAttributeNameRfc2231(
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
          index = HeaderParser .ParseCFWS(str, index, endIndex, null);
        }
        if (index >= endIndex) {
          return false;
        }
        if (str.charAt(index) != '=') {
          return false;
        }
        attribute = DataUtilities.ToLowerCaseAscii(attribute);
        if (parameters.containsKey(attribute)) {
          // System.out.println("Contains duplicate attribute " + attribute);
          return false;
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
          return httpRules || ExpandRfc2231Extensions(parameters);
        }
        builder.delete(0, (0)+(builder.length()));
        int qs;
        // If the attribute name ends with '*' the value may not be a quoted
        // String
        if (attribute.charAt(attribute.length() - 1) != '*') {
          // try getting the value quoted
          qs = skipQuotedString(
            str,
            index,
            endIndex,
            builder,
            httpRules ? QuotedStringRule.Http : QuotedStringRule.Rfc5322);
          if (!httpRules && qs != index) {
            qs = HeaderParser.ParseCFWS(str, qs, endIndex, null);
          }
          if (qs != index) {
            parameters.put(attribute, builder.toString());
            index = qs;
            continue;
          }
          builder.delete(0, (0)+(builder.length()));
        }
        // try getting the value unquoted
        // Note we don't use getAtom
        qs = SkipMimeToken(str, index, endIndex, builder, httpRules);
        if (qs != index) {
          parameters.put(attribute, builder.toString());
          index = qs;
          continue;
        }
        // no valid value, return
        return false;
      }
    }

    private boolean ParseMediaType(String str) {
      boolean HttpRules = false;
      int index = 0;
      if (str == null) {
        throw new NullPointerException("str");
      }
      int endIndex = str.length();
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      int i = skipMimeTypeSubtype(str, index, endIndex, null);
      if (i == index || i >= endIndex || str.charAt(i) != '/') {
        return false;
      }
      this.topLevelType =
        DataUtilities.ToLowerCaseAscii(str.substring(index, (index)+(i - index)));
      ++i;
      int i2 = skipMimeTypeSubtype(str, i, endIndex, null);
      if (i == i2) {
        return false;
      }
      this.subType = DataUtilities.ToLowerCaseAscii(str.substring(i, (i)+(i2 - i)));
      if (i2 < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i2, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return true;
        }
        if (i3 < endIndex && str.charAt(i3) != ';') {
          // not followed by ";", so not a media type
          return false;
        }
      }
      index = i2;
      return ParseParameters(str, index, endIndex, HttpRules, this.parameters);
    }

    /**
     * Specifies the media type "text/plain" and the "charset" parameter
     * "US-ASCII", used for plain text data.
     */
    public static final MediaType TextPlainAscii =
      new MediaTypeBuilder(
"text",
"plain").SetParameter("charset",
        "us-ascii").ToMediaType();

    /**
     * Specifies the media type "text/plain" and the "charset" parameter "utf-8",
     * used for Unicode plain text data.
     */
    public static final MediaType TextPlainUtf8 =
      new MediaTypeBuilder(
"text",
"plain").SetParameter("charset",
        "utf-8").ToMediaType();

    /**
     * Specifies the media type "message/rfc822" , used for Internet mail messages.
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
    }

    /**
     * Parses a media type string and returns a media type object. This method
     * checks the syntactic validity of the string, but not whether it has
     * all parameters it's required to have or whether the parameters
     * themselves are set to valid values for the parameter.
     * @param mediaTypeValue A string object representing a media type. This media
     * type can include parameters.
     * @return A media type object, or text/plain if {@code mediaTypeValue} is
     * empty or syntactically invalid.
     */
    public static MediaType Parse(String mediaTypeValue) {
      return Parse(mediaTypeValue, TextPlainAscii);
    }

    /**
     * Parses a media type string and returns a media type object, or the default
     * value if the string is invalid. This method checks the syntactic
     * validity of the string, but not whether it has all parameters it's
     * required to have or whether the parameters themselves are set to
     * valid values for the parameter.
     * @param str A string object representing a media type. This media type can
     * include parameters.
     * @param defaultValue The media type to return if the string is syntactically
     * invalid. Can be null.
     * @return A MediaType object.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static MediaType Parse(String str, MediaType defaultValue) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      MediaType mt = new MediaType();
      mt.parameters = new TreeMap<String, String>();
      if (!mt.ParseMediaType(str)) {
return defaultValue;
      }
      return mt;
    }
  }
