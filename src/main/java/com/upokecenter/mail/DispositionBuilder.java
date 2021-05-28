package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */

import java.util.*;
import com.upokecenter.util.*;

  /**
   * A mutable data type that allows a content disposition to be built.
   * @deprecated Use ContentDisposition.Builder instead.
 */
@Deprecated
  public class DispositionBuilder {
    private final ContentDisposition.Builder builder;

    /**
     * Gets this value's disposition type, such as "inline" or "attachment".
     * @return This value's disposition type, such as "inline" or "attachment" .
     * @throws NullPointerException The property is being set and the value is
     * null.
     * @throws IllegalArgumentException The property is being set and the value is an
     * empty string.
     */
    public final String getDispositionType() {
        return this.builder.getDispositionType();
      }
public final void setDispositionType(String value) {
        this.builder.SetDispositionType(value);
      }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.DispositionBuilder} class using the disposition
     *  type "attachment" .
     */
    public DispositionBuilder() {
      this.builder = new ContentDisposition.Builder();
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.DispositionBuilder} class using the data from
     * the given content disposition.
     * @param mt The parameter {@code mt} is a ContentDisposition object.
     * @throws NullPointerException The parameter {@code mt} is null.
     */
    public DispositionBuilder(ContentDisposition mt) {
      this.builder = new ContentDisposition.Builder(mt);
    }

    /**
     * Initializes a new instance of the {@link
     * com.upokecenter.mail.DispositionBuilder} class using the given
     * disposition type.
     * @param type The parameter {@code type} is a text string.
     * @throws NullPointerException The parameter {@code type} is null.
     * @throws IllegalArgumentException Type is empty.
     */
    public DispositionBuilder(String type) {
      this.builder = new ContentDisposition.Builder(type);
    }

    /**
     * Gets a value indicating whether this is a text media type.
     * @return {@code true} If this is a text media type; otherwise, {@code false}.
     * @deprecated Irrelevant for content dispositions; will be removed in the future.
 */
@Deprecated
    public final boolean isText() {
        return this.builder.isText();
      }

    /**
     * Gets a value indicating whether this is a multipart media type.
     * @return {@code true} If this is a multipart media type; otherwise, {@code
     * false}.
     * @deprecated Irrelevant for content dispositions; will be removed in the\u0020future.
 */
@Deprecated
    public final boolean isMultipart() {
        return this.builder.isMultipart();
      }

    /**
     * Converts this object to an immutable ContentDisposition object.
     * @return A MediaType object.
     */
    public ContentDisposition ToDisposition() {
        return this.builder.ToDisposition();
    }

    /**
     * Sets the disposition type, such as "inline". This method enables the pattern
     * of method chaining (e.g., <code>new ...().getSet()...().getSet()...()</code>)
     * unlike with the DispositionType property in .NET or the
     * setDispositionType method (with small s) in Java.
     * @param str The parameter {@code str} is a text string.
     * @return This instance.
     * @throws NullPointerException The parameter {@code str} is null.
     * @throws IllegalArgumentException Str is empty.
     */
    public DispositionBuilder SetDispositionType(String str) {
        this.builder.SetDispositionType(str);
        return this;
    }

    /**
     * Removes a parameter from this content disposition. Does nothing if the
     * parameter's name doesn't exist.
     * @param name The parameter to remove. The name is compared using a basic
     * case-insensitive comparison. (Two strings are equal in such a
     * comparison, if they match after converting the basic upper-case
     * letters A to Z (U+0041 to U+005A) in both strings to basic
     * lower-case letters.).
     * @return This instance.
     * @throws NullPointerException The parameter {@code name} is null.
     */
    public DispositionBuilder RemoveParameter(String name) {
        this.builder.RemoveParameter(name);
        return this;
    }

    /**
     * Sets a parameter of this content disposition.
     * @param name Name of the parameter to set. If this name already exists
     * (compared using a basic case-insensitive comparison), it will be
     * overwritten. (Two strings are equal in a basic case-insensitive
     * comparison, if they match after converting the basic upper-case
     * letters A to Z (U+0041 to U+005A) in both strings to basic
     * lower-case letters.).
     * @param value Value of the parameter to set.
     * @return This instance.
     * @throws NullPointerException Either {@code value} or {@code name} is null.
     * @throws IllegalArgumentException The parameter {@code name} is empty, or it isn't a
     * well-formed parameter name.
     */
    public DispositionBuilder SetParameter(String name, String value) {
        this.builder.SetParameter(name, value);
        return this;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.builder.ToDisposition().toString();
    }
  }
