package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

    /**
     * Specifies how a message body should be displayed or handled by a mail
     * user agent. <p>This type is immutable; its contents can't be changed
     * after it's created.</p>
     */
  public class ContentDisposition
  {
    private String dispositionType;

    /**
     * Gets a string containing this object's disposition type, such as
     * "inline" or "attachment".
     * @return A string containing this object's disposition type, such
     * as "inline" or "attachment".
     */
    public String getDispositionType() {
        return this.dispositionType;
      }

    /**
     * Determines whether this object and another object are equal.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; otherwise, false.
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
     * Returns the hash code for this instance.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 632580499;
      {
        if (this.dispositionType != null) {
          valueHashCode += 632580503 * this.dispositionType.hashCode();
        }
        if (this.parameters != null) {
          valueHashCode += 632580587 * this.parameters.size();
        }
      }
      return valueHashCode;
    }

    /**
     * Gets a value indicating whether the disposition type is inline.
     * @return True if the disposition type is inline; otherwise, false..
     */
    public boolean isInline() {
        return this.dispositionType.equals("inline");
      }

    /**
     * Gets a value indicating whether the disposition type is attachment.
     * @return True if the disposition type is attachment; otherwise, false..
     */
    public boolean isAttachment() {
        return this.dispositionType.equals("attachment");
      }

    ContentDisposition(String type, Map<String, String> parameters) {
      this.dispositionType = type;
      this.parameters = new TreeMap<String, String>(parameters);
    }

    private TreeMap<String, String> parameters;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public Map<String, String> getParameters() {
        return java.util.Collections.unmodifiableMap(this.parameters);
      }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      StringBuilder sb = new StringBuilder();
      sb.append(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sb);
      return sb.toString();
    }

    private static String RemoveEncodedWordEnds(String str) {
      StringBuilder sb = new StringBuilder();
      int index = 0;
      boolean inEncodedWord = false;
      while (index < str.length()) {
        if (!inEncodedWord && index + 1 < str.length() && str.charAt(index) == '=' && str.charAt(index + 1) == '?') {
          // Remove start of encoded word
          inEncodedWord = true;
          index += 2;
          int qmarks = 0;
          // skip charset and encoding
          while (index < str.length()) {
            if (str.charAt(index) == '?') {
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
        } else if (inEncodedWord && index + 1 < str.length() && str.charAt(index) == '?' && str.charAt(index + 1) == '=') {
          // End of encoded word
          index += 2;
          inEncodedWord = false;
        } else {
          int c = DataUtilities.CodePointAt(str, index);
          if (c == 0xfffd) {
            sb.append((char)0xfffd);
            ++index;
          } else {
            sb.append(str.charAt(index++));
            if (c >= 0x10000) {
              sb.append(str.charAt(index++));
            }
          }
        }
      }
      return sb.toString();
    }

    /**
     * Converts a filename from the Content-Disposition header to a suitable
     * name for saving data to a file.
     * @param str A string representing a file name.
     * @return A string with the converted version of the file name. Among
     * other things, encoded words under RFC 2047 are decoded (since they
     * occur so frequently in Content-Disposition filenames); the value
     * is decoded under RFC 2231 if possible; characters unsuitable for
     * use in a filename (including the directory separators slash and backslash)
     * are replaced with underscores; and the filename is truncated if it
     * would otherwise be too long.
     */
    public static String MakeFilename(String str) {
      if (str == null) {
        return "";
      }
      str = ParserUtility.TrimSpaceAndTab(str);
      if (str.indexOf("=?") >= 0) {
        // May contain encoded words, which are very frequent
        // in Content-Disposition filenames (they would appear quoted
        // in the Content-Disposition "filename" parameter); these changes
        // appear justified in sec. 2.3 of RFC 2183, which says that
        // the parameter's value "should be used as a
        // basis for the actual filename, where possible."
        str = Rfc2047.DecodeEncodedWords(str, 0, str.length(), EncodedWordContext.Unstructured);
        if (str.indexOf("=?") >= 0) {
          // Remove ends of encoded words that remain
          str = RemoveEncodedWordEnds(str);
        }
      } else if (str.indexOf('\'') > 0) {
        // Check for RFC 2231 encoding, as long as the value before the
        // apostrophe is a recognized charset. It appears to be common,
        // too, to use quotes around a filename parameter AND use
        // RFC 2231 encoding, even though all the examples in that RFC
        // show unquoted use of this encoding.
        String charset = Charsets.ResolveAliasForEmail(str.substring(0,str.indexOf('\'')));
        if (!((charset)==null || (charset).length()==0)) {
          String newstr = MediaType.DecodeRfc2231Extension(str);
          if (!((newstr)==null || (newstr).length()==0)) {
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
      if (str.length() == 0) {
        return "_";
      }
      StringBuilder builder = new StringBuilder();
      // Replace unsuitable characters for filenames
      // and make sure the filename's
      // length doesn't exceed 250
      for (int i = 0; i < str.length() && builder.length() < 250; ++i) {
        int c = DataUtilities.CodePointAt(str, i);
        if (c >= 0x10000) {
          ++i;
        }
        if (c == (int)'\t') {
          // Replace tab with space
          builder.append(' ');
        } else if (c < 0x20 || c == '\\' || c == '/' || c == '*' || c == '?' || c == '|' ||
                   c == ':' || c == '<' || c == '>' || c == '"' || (c >= 0x7f && c <= 0x9F)) {
          // Unsuitable character for a filename (one of the characters reserved by Windows,
          // backslash, forward slash, ASCII controls, and C1 controls).
          builder.append('_');
        } else {
          if (builder.length() < 249 || c < 0x10000) {
            if (c <= 0xffff) {
              builder.append((char)c);
            } else if (c <= 0x10ffff) {
              builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        }
      }
      str = builder.toString();
      str = ParserUtility.TrimSpaceAndTab(str);
      if (str.length() == 0) {
        return "_";
      }
      if (str.charAt(str.length() - 1) == '.') {
        // Ends in a dot
        str += "_";
      }
      String strLower = DataUtilities.ToLowerCaseAscii(str);
      if (strLower.equals("nul") ||
          strLower.indexOf("nul.") == 0 ||
          strLower.equals("prn") ||
          strLower.indexOf("prn.") == 0 ||
          strLower.equals("aux") ||
          strLower.indexOf("aux.") == 0 ||
          strLower.equals("con") ||
          strLower.indexOf("con.") == 0 ||
          (strLower.length() >= 4 && strLower.indexOf("lpt") == 0 && strLower.charAt(3) >= '1' && strLower.charAt(3) <= '9') ||
          (strLower.length() >= 4 && strLower.indexOf("com") == 0 && strLower.charAt(3) >= '1' && strLower.charAt(3) <= '9')) {
        // Reserved filenames on Windows
        str = "_" + str;
      }
      if (str.charAt(0) == '~') {
        // Home folder convention
        str = "_" + str;
      }
      if (str.charAt(0) == '.') {
        // Starts with period; may be hidden in some configurations
        str = "_" + str;
      }
      return Normalizer.Normalize(str, Normalization.NFC);
    }

    /**
     * Gets a parameter from this disposition object.
     * @param name The name of the parameter to get. The name will be matched
     * case-insensitively. Can&apos;t be null.
     * @return The value of the parameter, or null if the parameter does not
     * exist.
     * @throws java.lang.NullPointerException The parameter {@code name}
     * is null.
     * @throws java.lang.IllegalArgumentException The parameter {@code name} is
     * empty.
     */
    public String GetParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (name.length() == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (this.parameters.containsKey(name)) {
        return this.parameters.get(name);
      }
      return null;
    }

    private boolean ParseDisposition(String str) {
      boolean httpRules = false;
      int index = 0;
      if (str == null) {
        throw new NullPointerException("str");
      }
      int endIndex = str.length();
      if (httpRules) {
        index = MediaType.skipLws(str, index, endIndex);
      } else {
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      }
      int i = MediaType.SkipMimeToken(str, index, endIndex, null, httpRules);
      if (i == index) {
        return false;
      }
      this.dispositionType = DataUtilities.ToLowerCaseAscii(str.substring(index,(index)+(i - index)));
      if (i < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return true;
        }
        if (i3 < endIndex && str.charAt(i3) != ';') {
          // not followed by ";", so not a media type
          return false;
        }
      }
      index = i;
      return MediaType.ParseParameters(str, index, endIndex, httpRules, this.parameters);
    }

    private static ContentDisposition Build(String name) {
      ContentDisposition dispo = new ContentDisposition();
      dispo.parameters = new TreeMap<String, String>();
      dispo.dispositionType = name;
      return dispo;
    }

    /**
     * Not documented yet.
     */
    public static final ContentDisposition Attachment = Build("attachment");

    /**
     * Not documented yet.
     */
    public static final ContentDisposition Inline = Build("inline");

    private ContentDisposition() {
    }

    /**
     * Parses a media type string and returns a media type object.
     * @param dispoValue A string object.
     * @return A media type object, or "Attachment" if {@code dispoValue}
     * is empty or syntactically invalid.
     */
    public static ContentDisposition Parse(String dispoValue) {
      return Parse(dispoValue, Attachment);
    }

    /**
     * Creates a new content disposition object from the value of a Content-Disposition
     * header field.
     * @param dispositionValue A string object that should be the value
     * of a Content-Disposition header field.
     * @param defaultValue The value to return in case the disposition value
     * is syntactically invalid. Can be null.
     * @return A ContentDisposition object.
     */
    public static ContentDisposition Parse(String dispositionValue, ContentDisposition defaultValue) {
      if (dispositionValue == null) {
        throw new NullPointerException("str");
      }
      ContentDisposition dispo = new ContentDisposition();
      dispo.parameters = new TreeMap<String, String>();
      if (!dispo.ParseDisposition(dispositionValue)) {

        return defaultValue;
      }
      return dispo;
    }
  }
