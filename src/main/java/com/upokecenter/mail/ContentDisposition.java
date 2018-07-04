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
            return this.dispositionType.equals (other.dispositionType) &&
              CollectionUtilities.MapEquals (this.parameters, other.parameters);
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
                return this.dispositionType.equals ("inline");
            }

    /**
     * Gets a value indicating whether the disposition type is attachment.
     * @return {@code true} If the disposition type is attachment; otherwise,
     * {@code false}.
     */
        public final boolean isAttachment() {
                return this.dispositionType.equals ("attachment");
            }

        ContentDisposition (
     String type,
     Map<String, String> parameters) {
            if ((type) == null) {
                throw new NullPointerException ("type");
            }
            this.dispositionType = type;
            this.parameters = new HashMap<String, String> (parameters);
        }

        private ContentDisposition () {
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
            sb.append (this.dispositionType);
            MediaType.AppendParameters (this.parameters, sb);
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
      return MakeFilenameMethod.MakeFilename (str);
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
                throw new NullPointerException ("name");
            }
            if (name.length() == 0) {
                throw new IllegalArgumentException ("name is empty.");
            }
            name = DataUtilities.ToLowerCaseAscii (name);
     return this.parameters.containsKey (name) ? this.parameters.get(name) :
              null;
        }

        private static ContentDisposition ParseDisposition(String str) {
            boolean HttpRules = false;
            int index = 0;
            if (str == null) {
                throw new NullPointerException ("str");
            }
            int endIndex = str.length();
            HashMap<String, String> parameters = new HashMap<String, String>();
            index = HeaderParser.ParseCFWS (str, index, endIndex, null);
       int i = MediaType.SkipMimeToken (str, index, endIndex, null,
              HttpRules);
            if (i == index) {
                return null;
            }
            String dispoType =
              DataUtilities.ToLowerCaseAscii (str.substring(index, (index)+(i - index)));
            if (i < endIndex) {
                // if not at end
                int i3 = HeaderParser.ParseCFWS (str, i, endIndex, null);
                if (i3 == endIndex) {
                    // at end
                    return new ContentDisposition (
                    dispoType,
                    parameters);
                }
                if (i3 < endIndex && str.charAt(i3) != ';') {
                    // not followed by ";", so not a content disposition
                    return null;
                }
            }
            index = i;
            return MediaType.ParseParameters (
              str,
              index,
              endIndex,
              HttpRules,
              parameters) ? new ContentDisposition (
                  dispoType,
                  parameters) : null;
        }

        private static ContentDisposition Build(String name) {
            return new ContentDisposition (
              name,
              new HashMap<String, String>());
        }

    /**
     * The content disposition value "attachment".
     */
        public static final ContentDisposition Attachment =
          Build ("attachment");

    /**
     * The content disposition value "inline".
     */
        public static final ContentDisposition Inline =
          Build ("inline");

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
                throw new NullPointerException ("dispoValue");
            }
            return Parse (dispoValue, Attachment);
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
                throw new NullPointerException ("dispositionValue");
            }
            ContentDisposition dispo = ParseDisposition (dispositionValue);
            return (dispo == null) ? (defaultValue) : dispo;
        }
    }
