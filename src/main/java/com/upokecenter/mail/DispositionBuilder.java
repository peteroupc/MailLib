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
     * Description of DispositionBuilder.
     */
  public class DispositionBuilder {
    private final Map<String, String> parameters;
    private String type;

    /**
     * Gets this value's disposition type, such value, such as "inline" or
     * "attachment".
     * @return This value's disposition type, such value, such as "inline" or
     * "attachment".
     */
    public final String getDispositionType() {
        return this.type;
      }
public final void setDispositionType(String value) {
        this.SetDispositionType(value);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.DispositionBuilder} class.
     */
    public DispositionBuilder() {
      this.parameters = new HashMap<String, String>();
      this.type = "attachment";
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.DispositionBuilder} class.
     * @param mt A ContentDisposition object.
     * @throws java.lang.NullPointerException The parameter {@code mt} is null.
     */
    public DispositionBuilder(ContentDisposition mt) {
      if (mt == null) {
        throw new NullPointerException("mt");
      }
      this.parameters = new HashMap<String, String>(mt.getParameters());
      this.type = mt.getDispositionType();
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.DispositionBuilder} class.
     * @param type A text string.
     * @throws java.lang.NullPointerException The parameter {@code type} is null.
     */
    public DispositionBuilder(String type) {
      if (type == null) {
  throw new NullPointerException("type");
}
if (type.length() == 0) {
  throw new IllegalArgumentException("type" + " is empty.");
}
      this.parameters = new HashMap<String, String>();
      this.SetDispositionType(type);
    }

    /**
     * Gets a value indicating whether this is a text media type.
     * @return {@code true} If this is a text media type; otherwise, {@code false}.
     * @deprecated Irrelevant for content dispositions; will be removed in the future.
 */
@Deprecated
    public final boolean isText() {
        return this.getDispositionType().equals("text");
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return {@code true} If this is a multipart media type; otherwise, {@code
     * false}.
     * @deprecated Irrelevant for content dispositions; will be removed in the future.
 */
@Deprecated
    public final boolean isMultipart() {
        return this.getDispositionType().equals("multipart");
      }

    /**
     * Converts this object to an immutable ContentDisposition object.
     * @return A MediaType object.
     */
    public ContentDisposition ToDisposition() {
      return new ContentDisposition(this.type, this.parameters);
    }

    /**
     * Sets the disposition type, such as "inline".
     * @param str A text string.
     * @return This instance.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
     */
    public DispositionBuilder SetDispositionType(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        throw new IllegalArgumentException("str is empty.");
      }
   if (MediaType.SkipMimeTypeSubtype(str, 0, str.length(), null) !=
        str.length()) {
        throw new IllegalArgumentException("Not a well-formed type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /**
     * Removes a parameter from this content disposition. Does nothing if the
     * parameter's name doesn't exist.
     * @param name The parameter to remove. The name is compared case
     * insensitively.
     * @return This instance.
     * @throws java.lang.NullPointerException The parameter {@code name} is null.
     */
    public DispositionBuilder RemoveParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      this.parameters.remove(DataUtilities.ToLowerCaseAscii(name));
      return this;
    }

    /**
     * Sets a parameter of this content disposition.
     * @param name Name of the parameter to set. If this name already exists
     * (compared case-insensitively), it will be overwritten.
     * @param value Value of the parameter to set.
     * @return This instance.
     * @throws java.lang.NullPointerException Either {@code value} or {@code name} is
     * null.
     * @throws IllegalArgumentException The parameter {@code name} is empty, or it
     * isn't a well-formed parameter name.
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
if (MediaType.SkipMimeTypeSubtype(name, 0, name.length(), null) !=
        name.length()) {
      throw new IllegalArgumentException("Not a well-formed parameter name: " +
          name);
      }
      this.parameters.put(DataUtilities.ToLowerCaseAscii(name), value);
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
