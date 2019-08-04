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

    /**
     * A mutable media type object.
     */
  public final class MediaTypeBuilder {
    private final Map<String, String> parameters;
    private String type;
    private String subtype;

    /**
     * Gets this value's top-level type.
     * @return This value's top-level type.
     */
    public final String getTopLevelType() {
        return this.type;
      }
public final void setTopLevelType(String value) {
        this.SetTopLevelType(value);
      }

    /**
     * Gets this value's subtype.
     * @return This value's subtype.
     */
    public final String getSubType() {
        return this.subtype;
      }
public final void setSubType(String value) {
        this.SetSubType(value);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MediaTypeBuilder} class.
     */
    public MediaTypeBuilder() {
      this.parameters = new HashMap<String, String>();
      this.type = "application";
      this.subtype = "octet-stream";
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MediaTypeBuilder} class.
     * @param mt The parameter {@code mt} is a MediaType object.
     * @throws NullPointerException The parameter {@code mt} is null.
     */
    public MediaTypeBuilder(MediaType mt) {
      if (mt == null) {
        throw new NullPointerException("mt");
      }
      this.parameters = new HashMap<String, String>(mt.getParameters());
      this.type = mt.getTopLevelType();
      this.subtype = mt.getSubType();
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MediaTypeBuilder} class.
     * @param type The parameter {@code type} is a text string.
     * @param subtype The parameter {@code subtype} is a text string.
     */
    public MediaTypeBuilder(String type, String subtype) {
      this.parameters = new HashMap<String, String>();
      this.SetTopLevelType(type);
      this.SetSubType(subtype);
    }

    /**
     * Gets a value indicating whether this is a text media type.
     * @return {@code true} If this is a text media type; otherwise, {@code false}.
     */
    public final boolean isText() {
        return this.getTopLevelType().startsWith("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return {@code true} If this is a multipart media type; otherwise, {@code
     * false}.
     */
    public final boolean isMultipart() {
        return this.getTopLevelType().startsWith("multipart");
      }

    /**
     * Converts this builder to an immutable media type object.
     * @return A MediaType object.
     */
    public MediaType ToMediaType() {
      return new MediaType(this.type, this.subtype, this.parameters);
    }

    /**
     * Sets this media type's top-level type.
     * @param str A text string naming a top-level type, such as "text" or "audio"
     *.
     * @return This instance.
     * @throws NullPointerException The parameter {@code str} is null.
     * @throws IllegalArgumentException The parameter {@code str} is syntactically invalid
     * for a top-level type.
     */
    public MediaTypeBuilder SetTopLevelType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.SkipMimeTypeSubtype(str, 0, str.length(), null) !=
        str.length()) {
        throw new IllegalArgumentException("Not a well-formed top level type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Removes a parameter from this builder object. Does nothing if the
     * parameter's name doesn't exist.
     * @param name Name of the parameter to remove. The name is compared using a
     * basic case-insensitive comparison. (Two strings are equal in such a
     * comparison, if they match after converting the basic upper-case
     * letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).
     * @return This instance.
     * @throws NullPointerException The parameter {@code name} is null.
     */
    public MediaTypeBuilder RemoveParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      this.parameters.remove(DataUtilities.ToLowerCaseAscii(name));
      return this;
    }

    /**
     * Sets a parameter's name and value for this media type.
     * @param name Name of the parameter to set, such as "charset" . The name is
     * compared using a basic case-insensitive comparison. (Two strings are
     * equal in such a comparison, if they match after converting the basic
     * upper-case letters A to Z (U + 0041 to U + 005A) in both strings to
     * lower case.).
     * @param value A text string giving the parameter's value.
     * @return This instance.
     * @throws NullPointerException The parameter {@code value} or {@code name} is
     * null.
     * @throws IllegalArgumentException The parameter {@code name} is empty or
     * syntactically invalid.
     */
    public MediaTypeBuilder SetParameter(String name, String value) {
      if (value == null) {
        throw new NullPointerException("value");
      }
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (name.length() == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      if (MediaType.SkipMimeTypeSubtype(name, 0, name.length(), null) !=
        name.length()) {
      throw new IllegalArgumentException("Not a well-formed parameter name: " +
          name);
      }
      this.parameters.put(DataUtilities.ToLowerCaseAscii(name), value);
      return this;
    }

    /**
     * Sets this media type's subtype, such as "plain" or "xml" .
     * @param str A text string naming a media subtype.
     * @return This instance.
     * @throws NullPointerException The parameter {@code str} is null.
     * @throws IllegalArgumentException The parameter {@code str} is empty or
     * syntactically invalid.
     */
    public MediaTypeBuilder SetSubType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.SkipMimeTypeSubtype(str, 0, str.length(), null) !=
        str.length()) {
        throw new IllegalArgumentException("Not a well-formed subtype: " + str);
      }
      this.subtype = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Converts this object to a text string of the media type it represents, in
     * the same form as <code>MediaType.toString</code>.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.ToMediaType().toString();
    }
  }
