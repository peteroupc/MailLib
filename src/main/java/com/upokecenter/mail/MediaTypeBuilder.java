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
    public String getTopLevelType() {
        return this.type;
      }
public void setTopLevelType(String value) {
        this.SetTopLevelType(value);
      }

    /**
     * Gets this value's subtype.
     * @return This value's subtype.
     */
    public String getSubType() {
        return this.subtype;
      }
public void setSubType(String value) {
        this.SetSubType(value);
      }

    public MediaTypeBuilder () {
      this.parameters = new HashMap<String, String>();
      this.type = "application";
      this.subtype = "octet-stream";
    }

    public MediaTypeBuilder (MediaType mt) {
      if (mt == null) {
        throw new NullPointerException("mt");
      }
      this.parameters = new HashMap<String, String>(mt.getParameters());
      this.type = mt.getTopLevelType();
      this.subtype = mt.getSubType();
    }

    public MediaTypeBuilder (String type, String subtype) {
      this.parameters = new HashMap<String, String>();
      this.SetTopLevelType(type);
      this.SetSubType(subtype);
    }

    /**
     * Gets a value indicating whether this is a text media type.
     * @return True if this is a text media type; otherwise, false..
     */
    public boolean isText() {
        return this.getTopLevelType().equals("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return True if this is a multipart media type; otherwise, false..
     */
    public boolean isMultipart() {
        return this.getTopLevelType().equals("multipart");
      }

    /**
     * Not documented yet.
     * @return A MediaType object.
     */
    public MediaType ToMediaType() {
      return new MediaType(this.type, this.subtype, this.parameters);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return This instance.
     */
    public MediaTypeBuilder SetTopLevelType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) != str.length()) {
        throw new IllegalArgumentException("Not a well-formed top level type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Not documented yet.
     * @param name A string object.
     * @return This instance.
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
     * @param name A string object.
     * @param value A string object. (2).
     * @return This instance.
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
      if (MediaType.skipMimeTypeSubtype(name, 0, name.length(), null) != name.length()) {
        throw new IllegalArgumentException("Not a well-formed parameter name: " + name);
      }
      this.parameters.put(DataUtilities.ToLowerCaseAscii(name),value);
      return this;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return This instance.
     */
    public MediaTypeBuilder SetSubType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) != str.length()) {
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
