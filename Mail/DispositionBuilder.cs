/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;

using PeterO;

namespace PeterO.Mail {
    /// <summary>Description of DispositionBuilder.</summary>
  public class DispositionBuilder {
    private string type;
    private readonly IDictionary<string, string> parameters;

    /// <summary>Gets or sets this value's disposition type, such value,
    /// such as "inline" or "attachment".</summary>
    /// <value>This value&#x27;s disposition type, such value, such as
    /// &#x22;inline&#x22; or &#x22;attachment&#x22;.</value>
    public string DispositionType {
      get {
        return this.type;
      }

      set {
        this.SetDispositionType(value);
      }
    }

    /// <summary>Initializes a new instance of the DispositionBuilder
    /// class.</summary>
    public DispositionBuilder() {
      this.parameters = new Dictionary<string, string>();
      this.type = "attachment";
    }

    /// <summary>Initializes a new instance of the DispositionBuilder
    /// class.</summary>
    /// <param name='mt'>A ContentDisposition object.</param>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='mt'/> is null.</exception>
    public DispositionBuilder(ContentDisposition mt) {
      if (mt == null) {
        throw new ArgumentNullException("mt");
      }
      this.parameters = new Dictionary<string, string>(mt.Parameters);
      this.type = mt.DispositionType;
    }

    /// <summary>Initializes a new instance of the DispositionBuilder
    /// class.</summary>
    /// <param name='type'>A string object.</param>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='type'/> is null.</exception>
    public DispositionBuilder(string type) {
      if (type == null) {
  throw new ArgumentNullException("type");
}
if (type.Length == 0) {
  throw new ArgumentException("type" + " is empty.");
}
      this.parameters = new Dictionary<string, string>();
      this.SetDispositionType(type);
    }

    /// <summary>Gets a value indicating whether this is a text media
    /// type.</summary>
    /// <value>True if this is a text media type; otherwise, false.</value>
[Obsolete("Irrelevant for content dispositions; will be removed in the future."
)]
    public bool IsText {
      get {
        return this.DispositionType.Equals("text");
      }
    }

    /// <summary>Gets a value indicating whether this is a multipart media
    /// type.</summary>
    /// <value>True if this is a multipart media type; otherwise,
    /// false.</value>
[Obsolete("Irrelevant for content dispositions; will be removed in the future."
)]
    public bool IsMultipart {
      get {
        return this.DispositionType.Equals("multipart");
      }
    }

    /// <summary>Converts this object to an immutable ContentDisposition
    /// object.</summary>
    /// <returns>A MediaType object.</returns>
    public ContentDisposition ToDisposition() {
      return new ContentDisposition(this.type, this.parameters);
    }

    /// <summary>Sets the disposition type, such as "inline".</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='str'/> is null.</exception>
    public DispositionBuilder SetDispositionType(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (str.Length == 0) {
        throw new ArgumentException("str is empty.");
      }
   if (MediaType.skipMimeTypeSubtype(str, 0, str.Length, null) !=
        str.Length) {
        throw new ArgumentException("Not a well-formed type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /// <summary>Removes a parameter from this content disposition. Does
    /// nothing if the parameter's name doesn't exist.</summary>
    /// <param name='name'>The parameter to remove. The name is compared
    /// case insensitively.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">The parameter <paramref
    /// name='name'/> is null.</exception>
    public DispositionBuilder RemoveParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      this.parameters.Remove(DataUtilities.ToLowerCaseAscii(name));
      return this;
    }

    /// <summary>Sets a parameter of this content disposition.</summary>
    /// <param name='name'>Name of the parameter to set. If this name
    /// already exists (compared case-insensitively), it will be
    /// overwritten.</param>
    /// <param name='value'>Value of the parameter to set.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref
    /// name='value'/> or <paramref name='name'/> is null.</exception>
    /// <exception cref="ArgumentException">The parameter <paramref
    /// name='name'/> is empty, or it isn't a well-formed parameter
    /// name.</exception>
    public DispositionBuilder SetParameter(string name, string value) {
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
if (MediaType.skipMimeTypeSubtype(name, 0, name.Length, null) !=
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
