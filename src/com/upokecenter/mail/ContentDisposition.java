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

    /**
     * Description of ContentDisposition.
     */
  public class ContentDisposition
  {
    private String dispositionType;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public String getDispositionType() {
        return this.dispositionType;
      }

    @Override public boolean equals(Object obj) {
      ContentDisposition other = ((obj instanceof ContentDisposition) ? (ContentDisposition)obj : null);
      if (other == null) {
        return false;
      }
      return this.dispositionType == other.dispositionType &&
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
     * @return Whether the disposition type is inline.
     */
    public boolean isInline() {
        return this.dispositionType.equals("inline");
      }

    /**
     * Gets a value indicating whether the disposition type is attachment.
     * @return Whether the disposition type is attachment.
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

    /**
     * Not documented yet.
     * @param name A string object. (2).
     * @return A string object.
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

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A Boolean object.
     */
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

    public static final ContentDisposition Attachment = Build("attachment");

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
     * Not documented yet.
     * @param str A string object.
     * @param defaultValue Can be null.
     * @return A ContentDisposition object.
     */
    public static ContentDisposition Parse(String str, ContentDisposition defaultValue) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      ContentDisposition dispo = new ContentDisposition();
      dispo.parameters = new TreeMap<String, String>();
      if (!dispo.ParseDisposition(str)) {

        return defaultValue;
      }
      return dispo;
    }
  }
