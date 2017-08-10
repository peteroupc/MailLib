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
     * Specifies how a message body should be displayed or handled by a mail user
     * agent. This type is immutable; its contents can't be changed after
     * it's created. To create a changeable disposition object, use the
     * DispositionBuilder class.
     */
  public class ContentDisposition {
    private final String dispositionType;

    /**
     * Gets a string containing this object's disposition type, such as "inline" or
     * "attachment".
     * @return A string containing this object's disposition type, such as "inline"
     * or "attachment".
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
      return this.dispositionType.equals(other.dispositionType) &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /**
     * Returns the hash code for this instance.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 632580499;
        if (this.dispositionType != null) {
          valueHashCode = (valueHashCode + (632580503 *
            this.dispositionType.hashCode()));
        }
        if (this.parameters != null) {
          valueHashCode = (valueHashCode + (632580587 * this.parameters.size()));
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
      if ((type) == null) {
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
     * @return A read-only list of parameter names associated with this object and
     * their values.getNOTE(): Previous versions erroneously stated that the list
     * will be sorted by name. In fact, the names will not be guaranteed to
     * appear in any particular order; this is at least the case in version
     * 0.10.0.
     */
    public final Map<String, String> getParameters() {
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
        if (!inEncodedWord && index + 1 < str.length() && str.charAt(index) == '=' &&
          str.charAt(index + 1) == '?') {
          // Remove start of encoded word
          inEncodedWord = true;
          index += 2;
          int start = index;
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
          if (qmarks == 2) {
            inEncodedWord = true;
          } else {
            inEncodedWord = false;
            sb.append('=');
            sb.append('?');
            index = start;
          }
        } else if (inEncodedWord && index + 1 < str.length() && str.charAt(index) ==
          '?' && str.charAt(index + 1) == '=') {
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
     * Converts a file name from the Content-Disposition header to a suitable name
     * for saving data to a file. <p>Examples:</p>
     * <p><code>"=?utf-8?q?hello=2Etxt?=" -&gt; "hello.txt"</code> (RFC 2047
     * encoding)</p> <p><code>"=?utf-8?q?long_filename?=" -&gt; "long
     * filename"</code> (RFC 2047 encoding)</p> <p><code>"utf-8'en'hello%2Etxt"
     * -&gt; "hello.txt"</code> (RFC 2231 encoding)</p> <p><code>"nul.txt" -&gt;
     * "_nul.txt"</code> (Reserved name)</p> <p><code>"dir1/dir2/file" -&gt;
     * "dir1_dir2_file"</code> (Directory separators)</p>
     * @param str A string representing a file name. Can be null.
     * @return A string with the converted version of the file name. Among other
     * things, encoded words under RFC 2047 are decoded (since they occur so
     * frequently in Content-Disposition filenames); the value is decoded
     * under RFC 2231 if possible; characters unsuitable for use in a
     * filename (including the directory separators slash and backslash) are
     * replaced with underscores; spaces and tabs are collapsed to a single
     * space; leading and trailing spaces and tabs are removed; and the
     * filename is truncated if it would otherwise be too long. The returned
     * string will be in normalization form C. Returns an empty string if
     * {@code str} is null.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     */
    public static String MakeFilename(String str) {
      if (str == null) {
        return "";
      }
      int i;
      str = ParserUtility.TrimAndCollapseSpaceAndTab(str);
      if (str.indexOf("=?") >= 0) {
        // May contain encoded words, which are very frequent
        // in Content-Disposition filenames (they would appear quoted
        // in the Content-Disposition "filename" parameter); these changes
        // appear justified in sec. 2.3 of RFC 2183, which says that
        // the parameter's value "should be used as a
        // basis for the actual filename, where possible."
        str = Rfc2047.DecodeEncodedWordsLenient(
  str,
  0,
  str.length());
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
        String charset = Encodings.ResolveAliasForEmail(
  str.substring(
  0, (
  0)+(str.indexOf('\''))));
        if (!((charset) == null || (charset).length() == 0)) {
          String newstr = MediaType.DecodeRfc2231Extension(str);
          if (!((newstr) == null || (newstr).length() == 0)) {
            // Value was decoded under RFC 2231
            str = newstr;
          }
        }
      }
      str = ParserUtility.TrimAndCollapseSpaceAndTab(str);
      if (str.length() == 0) {
        return "_";
      }
      StringBuilder builder = new StringBuilder();
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
      i = 0;
      while (i < str.length() && builder.length() < 243) {
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
          builder.append(' ');
        } else if (c < 0x20 || c == '\\' || c == '/' || c == '*' ||
          c == '?' || c == '|' ||
    c == ':' || c == '<' || c == '>' || c == '"' ||
          (c >= 0x7f && c <= 0x9f)) {
          // Unsuitable character for a filename (one of the characters
          // reserved by Windows,
          // backslash, forward slash, ASCII controls, and C1 controls).
          builder.append('_');
  } else if (c == '!' && i + 1 < str.length() && str.charAt(i) == '[') {
     // '![ ... ]' may be interpreted in BASH as an evaluator;
     // replace '!' with underscore
    builder.append('_');
  } else if (c == '`') {
     // '`' starts a command in BASH and possibly other shells
    builder.append('_');
  } else if (c == '$') {
     // '$' starts a variable in BASH and possibly other shells
    builder.append('_');
        } else if (c == 0x2028 || c == 0x2029) {
          // line break characters (0x85 is already included above)
          builder.append('_');
        } else if ((c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <= 0xfdef)) {
          // noncharacters
          builder.append('_');
        } else if (c == '%') {
          // Treat percent ((character instanceof unsuitable) ? (unsuitable)character : null), even though it can occur
          // in a Windows filename, since it's used in MS-DOS and Windows
          // in environment variable placeholders
          builder.append('_');
        } else {
          if (builder.length() < 242 || c < 0x10000) {
            if (c <= 0xffff) {
              builder.append((char)c);
            } else if (c <= 0x10ffff) {
              builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        }
  ++i;
      }
      str = builder.toString();
      str = ParserUtility.TrimAndCollapseSpaceAndTab(str);
      if (str.length() == 0) {
        return "_";
      }
      String strLower = DataUtilities.ToLowerCaseAscii(str);
      // Reserved filenames on Windows
      boolean reservedFilename =
  strLower.equals(
  "nul") || strLower.equals("clock$") ||
strLower.indexOf(
  "nul.") == 0 || strLower.equals(
  "prn") ||
strLower.indexOf(
  "prn.") == 0 || strLower.equals(
  "aux") ||
strLower.indexOf(
  "aux.") == 0 || strLower.equals(
  "con") ||
strLower.indexOf(
  "con.") == 0 || (
  strLower.length() >= 4 && strLower.indexOf(
  "lpt") == 0 && strLower.charAt(3) >= '0' &&
       strLower.charAt(3) <= '9') || (strLower.length() >= 4 &&
              strLower.indexOf(
  "com") == 0 && strLower.charAt(3) >= '0' &&
            strLower.charAt(3) <= '9');
      boolean bracketDigit = str.charAt(0) == '{' && str.length() > 1 &&
            str.charAt(1) >= '0' && str.charAt(1) <= '9';
      // Home folder convention (tilde).
        // Filenames starting with hyphens can also be
        // problematic especially in Unix-based systems,
        // and filenames starting with dollar sign can
        // be misinterpreted if they're treated as expansion
        // symbols
     boolean homeFolder = str.charAt(0) == '~' || str.charAt(0) == '-' || str.charAt(0) == '$';
     // Starts with period; may be hidden in some configurations
     boolean period = str.charAt(0) == '.';
      if (reservedFilename || bracketDigit || homeFolder ||
           period) {
        str = "_" + str;
      }
      // Avoid space before and after last dot
      for (i = str.length() - 1; i >= 0; --i) {
        if (str.charAt(i) == '.') {
          boolean spaceAfter = i + 1 < str.length() && str.charAt(i + 1) == 0x20;
          boolean spaceBefore = i > 0 && str.charAt(i - 1) == 0x20;
          if (spaceAfter && spaceBefore) {
            str = str.substring(0,i - 1) + "_._" + str.substring(i + 2);
          } else if (spaceAfter) {
            str = str.substring(0,i) + "._" + str.substring(i + 2);
          } else if (spaceBefore) {
            str = str.substring(0,i - 1) + "_." + str.substring(i + 1);
          }
          break;
        }
      }
      str = NormalizerInput.Normalize(str, Normalization.NFC);
      // Ensure length is 254 or less
      if (str.length() > 254) {
        char c = str.charAt(254);
        int newLength = 254;
        if ((c & 0xfc00) == 0xdc00) {
          --newLength;
        }
        str = str.substring(0, newLength);
      }
      if (str.charAt(str.length() - 1) == '.' || str.charAt(str.length() - 1) == '~') {
        // Ends in a dot or tilde (a file whose name ends with
        // the latter may be treated as
        // a backup file especially in Unix-based systems).
        // NOTE: Although concatenation of two NFC strings
        // doesn't necessarily lead to an NFC String, this
        // particular concatenation doesn't disturb the NFC
        // status of the String.
        str += "_";
      }
      return str;
    }

    /**
     * Gets a parameter from this disposition object.
     * @param name The name of the parameter to get. The name will be matched using
     * a basic case-insensitive comparison. (Two strings are equal in such a
     * comparison, if they match after converting the basic upper-case
     * letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).
     * Can't be null.
     * @return The value of the parameter, or null if the parameter does not exist.
     * @throws java.lang.NullPointerException The parameter {@code name} is null.
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
      return this.parameters.containsKey(name) ? this.parameters.get(name) : null;
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
      int i = MediaType.SkipMimeToken(str, index, endIndex, null, HttpRules);
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
     * The content disposition value "attachment".
     */
    public static final ContentDisposition Attachment =
      Build("attachment");

    /**
     * The content disposition value "inline".
     */
    public static final ContentDisposition Inline =
      Build("inline");

    /**
     * Parses a content disposition string and returns a content disposition
     * object.
     * @param dispoValue The parameter {@code dispoValue} is a text string.
     * @return A content disposition object, or "Attachment" if {@code dispoValue}
     * is empty or syntactically invalid.
     * @throws java.lang.NullPointerException The parameter {@code dispoValue} is
     * null.
     */
    public static ContentDisposition Parse(String dispoValue) {
      if (dispoValue == null) {
  throw new NullPointerException("dispoValue");
}
      return Parse(dispoValue, Attachment);
    }

    /**
     * Creates a new content disposition object from the value of a
     * Content-Disposition header field.
     * @param dispositionValue A text string that should be the value of a
     * Content-Disposition header field.
     * @param defaultValue The value to return in case the disposition value is
     * syntactically invalid. Can be null.
     * @return A ContentDisposition object.
     * @throws java.lang.NullPointerException The parameter {@code dispositionValue}
     * is null.
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
