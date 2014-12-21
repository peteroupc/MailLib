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
     * A mutable media type object.
     */
  public final class MediaTypeBuilder
  {
    private String type;
    private String subtype;
    private Map<String, String> parameters;

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
     * Initializes a new instance of the MediaTypeBuilder class.
     */
    public MediaTypeBuilder () {
      this.parameters = new HashMap<String, String>();
      this.type = "application";
      this.subtype = "octet-stream";
    }

    /**
     * Initializes a new instance of the MediaTypeBuilder class.
     * @param mt A MediaType object.
     * @throws NullPointerException The parameter {@code mt} is null.
     */
    public MediaTypeBuilder (MediaType mt) {
      if (mt == null) {
        throw new NullPointerException("mt");
      }
      this.parameters = new HashMap<String, String>(mt.getParameters());
      this.type = mt.getTopLevelType();
      this.subtype = mt.getSubType();
    }

    /**
     * Initializes a new instance of the MediaTypeBuilder class.
     * @param type A string object.
     * @param subtype A string object. (2).
     */
    public MediaTypeBuilder (String type, String subtype) {
      this.parameters = new HashMap<String, String>();
      this.SetTopLevelType(type);
      this.SetSubType(subtype);
    }

    /**
     * Gets a value indicating whether this is a text media type.
     * @return True if this is a text media type; otherwise, false..
     */
    public final boolean isText() {
        return this.getTopLevelType().equals("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return True if this is a multipart media type; otherwise, false..
     */
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
     * Not documented yet.
     * @param str A string object.
     * @return This instance.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public MediaTypeBuilder SetTopLevelType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
   if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) !=
        str.length()) {
        throw new IllegalArgumentException("Not a well-formed top level type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Removes a parameter from this builder object.
     * @param name Name of the parameter to remove. The name is compared
     * case-insensitively.
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
     * Not documented yet.
     * @param name Name of the parameter to set. The name is compared
     * case-insensitively.
     * @param value A string object. (2).
     * @return This instance.
     * @throws NullPointerException The parameter {@code value} or {@code name} is
     * null.
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
if (MediaType.skipMimeTypeSubtype(name, 0, name.length(), null) !=
        name.length()) {
      throw new IllegalArgumentException("Not a well-formed parameter name: " +
          name);
      }
      this.parameters.put(DataUtilities.ToLowerCaseAscii(name), value);
      return this;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return This instance.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public MediaTypeBuilder SetSubType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
   if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) !=
        str.length()) {
        throw new IllegalArgumentException("Not a well-formed subtype: " + str);
      }
      this.subtype = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.ToMediaType().toString();
    }
  }
