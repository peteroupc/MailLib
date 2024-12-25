package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

import java.util.*;
import com.upokecenter.util.*;

  /**
   * A mutable media type object.
   * @deprecated Use MediaType.Builder instead.
 */
@Deprecated
  public final class MediaTypeBuilder {
    private final Map<String, String> parameters;
    private String type;
    private String subtype;

    /**
     * Gets this value's top-level type.
     * @return A text string naming this object's top-level type, such as "text" or
     * "audio" .
     * @throws NullPointerException The property is being set and the value is
     * null.
     * @throws IllegalArgumentException The property is being set and the value is
     * syntactically invalid for a top-level type.
     */
    public final String getTopLevelType() {
        return this.type;
      }
public final void setTopLevelType(String value) {
        this.SetTopLevelType(value);
      }

    /**
     * Gets this value's subtype.
     * @return A text string naming this object's subtype, such as "plain" or
     * "xml".
     * @throws NullPointerException The property is being set and the value is
     * null.
     * @throws IllegalArgumentException The property is being set and the value is
     * syntactically invalid for a subtype.
     */
    public final String getSubType() {
        return this.subtype;
      }
public final void setSubType(String value) {
        this.SetSubType(value);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MediaTypeBuilder} class, using the type
     * "application/octet-stream" .
     */
    public MediaTypeBuilder() {
      this.parameters = new HashMap<String, String>();
      this.type = "application";
      this.subtype = "octet-stream";
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.MediaTypeBuilder} class using the data from another
     * media type.
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
     * @param type The media type's top-level type.
     * @param subtype The media type's subtype.
     */
    public MediaTypeBuilder(String type, String subtype) {
      this.parameters = new HashMap<String, String>();
      this.SetTopLevelType(type);
      this.SetSubType(subtype);
    }

    /**
     * Gets a value indicating whether this is a text media type.
     * @return {@code true} If this is a text media type; otherwise, {@code false}.
     * @deprecated Instead of using this property, use the TopLevelType property and compare
 * the result with the exact String 'text'.
 */
@Deprecated
    public final boolean isText() {
        return this.getTopLevelType().equals("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return {@code true} If this is a multipart media type; otherwise, {@code
     * false}.
     * @deprecated Instead of using this property, use the TopLevelType property and compare
 * the result with the exact String 'multipart'.
 */
@Deprecated
    public final boolean isMultipart() {
        return this.getTopLevelType().equals("multipart");
      }

    /**
     * Converts this builder to an immutable media type object.
     * @return A MediaType object.
     */
    public MediaType ToMediaType() {
      return new MediaType(this.type, this.subtype, this.parameters);
    }

    /**
     * Sets this media type's top-level type. This method enables the pattern of
     * method chaining (for example, {@code new...().getSet()...().getSet()...()}) unlike
     * with the TopLevelType property in.NET or the setTopLevelType method (with
     * small s) in Java.
     * @param str A text string naming a top-level type, such as "text" or "audio"
     * .
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
        throw new IllegalArgumentException("Not a well-formed top level type: " +
str);
      }
      this.type = com.upokecenter.util.DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Removes a parameter from this builder object. Does nothing if the
     * parameter's name doesn't exist.
     * @param name Name of the parameter to remove. The name is compared using a
     * basic case-insensitive comparison. (Two strings are equal in such a
     * comparison, if they match after converting the basic uppercase letters A to
     * Z (U+0041 to U+005A) in both strings to basic lowercase letters.).
     * @return This instance.
     * @throws NullPointerException The parameter {@code name} is null.
     */
    public MediaTypeBuilder RemoveParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      this.parameters.remove(com.upokecenter.util.DataUtilities.ToLowerCaseAscii(name));
      return this;
    }

    /**
     * Sets a parameter's name and value for this media type.
     * @param name Name of the parameter to set, such as "charset" . The name is
     * compared using a basic case-insensitive comparison. (Two strings are equal
     * in such a comparison, if they match after converting the basic uppercase
     * letters A to Z (U+0041 to U+005A) in both strings to basic lowercase
     * letters.).
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
      this.parameters.put(com.upokecenter.util.DataUtilities.ToLowerCaseAscii(name), value);
      return this;
    }

    /**
     * Sets this media type's subtype, such as "plain" or "xml" . This method
     * enables the pattern of method chaining (for example, {@code
     * new...().getSet()...().getSet()...()}) unlike with the SubType property in.NET or the
     * setSubType method (with small s) in Java.
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
      this.subtype = com.upokecenter.util.DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Converts this object to a text string of the media type it represents, in
     * the same form as {@code MediaType.toString}.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.ToMediaType().toString();
    }
  }
