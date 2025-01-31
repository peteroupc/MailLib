/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using PeterO;

namespace PeterO.Mail {
  /// <summary>A mutable data type that allows a content disposition to
  /// be built.</summary>
  [Obsolete("Use ContentDisposition.Builder instead.")]
  public class DispositionBuilder {
    private readonly ContentDisposition.Builder builder;

    /// <summary>Gets or sets this value's disposition type, such as
    /// "inline" or "attachment".</summary>
    /// <value>This value's disposition type, such as "inline" or
    /// "attachment" .</value>
    /// <exception cref='ArgumentNullException'>The property is being set
    /// and the value is null.</exception>
    /// <exception cref='ArgumentException'>The property is being set and
    /// the value is an empty string.</exception>
    public string DispositionType {
      get {
        return this.builder.DispositionType;
      }

      set {
        this.builder.SetDispositionType(value);
      }
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.DispositionBuilder'/> class using the
    /// disposition type "attachment" .</summary>
    public DispositionBuilder() {
      this.builder = new ContentDisposition.Builder();
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.DispositionBuilder'/> class using the data
    /// from the specified content disposition.</summary>
    /// <param name='mt'>The parameter <paramref name='mt'/> is a
    /// ContentDisposition object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='mt'/> is null.</exception>
    public DispositionBuilder(ContentDisposition mt) {
      this.builder = new ContentDisposition.Builder(mt);
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.DispositionBuilder'/> class using the
    /// specified disposition type.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is a text
    /// string.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='type'/> is null.</exception>
    /// <exception cref='ArgumentException'>Type is empty.</exception>
    public DispositionBuilder(string type) {
      this.builder = new ContentDisposition.Builder(type);
    }

    /// <summary>Gets a value indicating whether this is a text media
    /// type.</summary>
    /// <value><c>true</c> If this is a text media type; otherwise,
    /// <c>false</c>.</value>
    [Obsolete(
        "Irrelevant for content dispositions; will be removed in the future.")]
    public bool IsText {
      get {
        return this.builder.IsText;
      }
    }

    /// <summary>Gets a value indicating whether this is a multipart media
    /// type.</summary>
    /// <value><c>true</c> If this is a multipart media type; otherwise,
    /// <c>false</c>.</value>
    [Obsolete("Irrelevant for content dispositions; will be removed in the" +
        "\u0020future.")]
    public bool IsMultipart {
      get {
        return this.builder.IsMultipart;
      }
    }

    /// <summary>Converts this object to an immutable ContentDisposition
    /// object.</summary>
    /// <returns>A MediaType object.</returns>
    public ContentDisposition ToDisposition() {
      return this.builder.ToDisposition();
    }

    /// <summary>Sets the disposition type, such as "inline". This method
    /// enables the pattern of method chaining (for example, <c>new
    /// ...().Set...().Set...()</c> ) unlike with the DispositionType
    /// property in .NET or the setDispositionType method (with small s) in
    /// Java.</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>Str is empty.</exception>
    public DispositionBuilder SetDispositionType(string str) {
      this.builder.SetDispositionType(str);
      return this;
    }

    /// <summary>Removes a parameter from this content disposition. Does
    /// nothing if the parameter's name doesn't exist.</summary>
    /// <param name='name'>The parameter to remove. The name is compared
    /// using a basic case-insensitive comparison. (Two strings are equal
    /// in such a comparison, if they match after converting the basic
    /// uppercase letters A to Z (U+0041 to U+005A) in both strings to
    /// basic lowercase letters.).</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    public DispositionBuilder RemoveParameter(string name) {
      this.builder.RemoveParameter(name);
      return this;
    }

    /// <summary>Sets a parameter of this content disposition.</summary>
    /// <param name='name'>Name of the parameter to set. If this name
    /// already exists (compared using a basic case-insensitive
    /// comparison), it will be overwritten. (Two strings are equal in a
    /// basic case-insensitive comparison, if they match after converting
    /// the basic uppercase letters A to Z (U+0041 to U+005A) in both
    /// strings to basic lowercase letters.).</param>
    /// <param name='value'>Value of the parameter to set.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>Either <paramref
    /// name='value'/> or <paramref name='name'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='name'/> is empty, or it isn't a well-formed parameter
    /// name.</exception>
    public DispositionBuilder SetParameter(string name, string value) {
      this.builder.SetParameter(name, value);
      return this;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.builder.ToDisposition().ToString();
    }
  }
}
