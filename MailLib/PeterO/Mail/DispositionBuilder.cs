/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using PeterO;

namespace PeterO.Mail {
    /// <summary>A mutable data type that allows a content disposition to
    /// be built.</summary>
  public class DispositionBuilder {
    private readonly IDictionary<string, string> parameters;
    private string type;

    /// <summary>Gets or sets this value's disposition type, such as
    /// "inline" or "attachment".</summary>
    /// <value>This value's disposition type, such as "inline" or
    /// "attachment" .</value>
    public string DispositionType {
      get {
        return this.type;
      }

      set {
        this.SetDispositionType(value);
      }
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.DispositionBuilder'/> class using the
    /// disposition type "attachment" .</summary>
    public DispositionBuilder() {
      this.parameters = new Dictionary<string, string>();
      this.type = "attachment";
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.DispositionBuilder'/> class using the data
    /// from the given content disposition.</summary>
    /// <param name='mt'>The parameter <paramref name='mt'/> is a
    /// ContentDisposition object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='mt'/> is null.</exception>
    public DispositionBuilder(ContentDisposition mt) {
      if (mt == null) {
        throw new ArgumentNullException(nameof(mt));
      }
      this.parameters = new Dictionary<string, string>(mt.Parameters);
      this.type = mt.DispositionType;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.DispositionBuilder'/> class using the given
    /// disposition type.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is a text
    /// string.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='type'/> is null.</exception>
    /// <exception cref='ArgumentException'>Type is empty.</exception>
    public DispositionBuilder(string type) {
      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }
      if (type.Length == 0) {
        throw new ArgumentException("type is empty.");
      }
      this.parameters = new Dictionary<string, string>();
      this.SetDispositionType(type);
    }

    /// <summary>Gets a value indicating whether this is a text media
    /// type.</summary>
    /// <value><c>true</c> If this is a text media type; otherwise,
    /// <c>false</c>.</value>
    [Obsolete(
      "Irrelevant for content dispositions; will be removed in the future.")]
    public bool IsText {
      get {
        return this.DispositionType.Equals("text", StringComparison.Ordinal);
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
        return this.DispositionType.Equals("multipart",
  StringComparison.Ordinal);
      }
    }

    /// <summary>Converts this object to an immutable ContentDisposition
    /// object.</summary>
    /// <returns>A MediaType object.</returns>
    public ContentDisposition ToDisposition() {
      return new ContentDisposition(this.type, this.parameters);
    }

    /// <summary>Sets the disposition type, such as "inline".</summary>
    /// <param name='str'>The parameter <paramref name='str'/> is a text
    /// string.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>Str is empty.</exception>
    public DispositionBuilder SetDispositionType(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length == 0) {
        throw new ArgumentException("str is empty.");
      }
      if (MediaType.SkipMimeTypeSubtype(str, 0, str.Length, null) !=
        str.Length) {
        throw new ArgumentException("Not a well-formed type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /// <summary>Removes a parameter from this content disposition. Does
    /// nothing if the parameter's name doesn't exist.</summary>
    /// <param name='name'>The parameter to remove. The name is compared
    /// using a basic case-insensitive comparison. (Two strings are equal
    /// in such a comparison, if they match after converting the basic
    /// upper-case letters A to Z (U + 0041 to U + 005A) in both strings to
    /// basic lower-case letters.).</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    public DispositionBuilder RemoveParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      this.parameters.Remove(DataUtilities.ToLowerCaseAscii(name));
      return this;
    }

    /// <summary>Sets a parameter of this content disposition.</summary>
    /// <param name='name'>Name of the parameter to set. If this name
    /// already exists (compared using a basic case-insensitive
    /// comparison), it will be overwritten. (Two strings are equal in a
    /// basic case-insensitive comparison, if they match after converting
    /// the basic upper-case letters A to Z (U + 0041 to U + 005A) in both
    /// strings to basic lower-case letters.).</param>
    /// <param name='value'>Value of the parameter to set.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>Either <paramref
    /// name='value'/> or <paramref name='name'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='name'/> is empty, or it isn't a well-formed parameter
    /// name.</exception>
    public DispositionBuilder SetParameter(string name, string value) {
      if (value == null) {
        throw new ArgumentNullException(nameof(value));
      }
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      if (MediaType.SkipMimeTypeSubtype(name, 0, name.Length, null) !=
        name.Length) {
        throw new ArgumentException("Not a well-formed parameter name: " +
            name);
      }
      this.parameters[DataUtilities.ToLowerCaseAscii(name)] = value;
      return this;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.ToDisposition().ToString();
    }
  }
}
