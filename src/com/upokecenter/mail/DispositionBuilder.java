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
     * Description of DispositionBuilder.
     */
  public class DispositionBuilder
  {
    private String type;
    private Map<String, String> parameters;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public String getDispositionType() {
        return this.type;
      }
public void setDispositionType(String value) {
        this.SetDispositionType(value);
      }

    public DispositionBuilder () {
      this.parameters = new HashMap<String, String>();
      this.type = "attachment";
    }

    public DispositionBuilder (ContentDisposition mt) {
      if (mt == null) {
        throw new NullPointerException("mt");
      }
      this.parameters = new HashMap<String, String>(mt.getParameters());
      this.type = mt.getDispositionType();
    }

    public DispositionBuilder (String type) {
      this.parameters = new HashMap<String, String>();
      this.SetDispositionType(type);
    }

    /**
     * Gets a value indicating whether this is a text media type.
     * @return True if this is a text media type; otherwise, false..
     */
    public boolean isText() {
        return this.getDispositionType().equals("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return True if this is a multipart media type; otherwise, false..
     */
    public boolean isMultipart() {
        return this.getDispositionType().equals("multipart");
      }

    /**
     * Not documented yet.
     * @return A MediaType object.
     */
    public ContentDisposition ToDisposition() {
      return new ContentDisposition(this.type, this.parameters);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return This instance.
     */
    public DispositionBuilder SetDispositionType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
      if (MediaType.skipMimeTypeSubtype(str, 0, str.length(), null) != str.length()) {
        throw new IllegalArgumentException("Not a well-formed type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Not documented yet.
     * @param name A string object.
     * @return This instance.
     */
    public DispositionBuilder RemoveParameter(String name) {
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
    public DispositionBuilder SetParameter(String name, String value) {
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
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.ToDisposition().toString();
    }
  }
