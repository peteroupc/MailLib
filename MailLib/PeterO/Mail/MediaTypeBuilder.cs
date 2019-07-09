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
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.MediaTypeBuilder"]/*'/>
  public sealed class MediaTypeBuilder {
    private readonly IDictionary<string, string> parameters;
    private string type;
    private string subtype;

    /// <xmlbegin id='4'/>
    /// <value/>
    public string TopLevelType {
      get {
        return this.type;
      }

      set {
        this.SetTopLevelType(value);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.MediaTypeBuilder.SubType"]/*'/>
    public string SubType {
      get {
        return this.subtype;
      }

      set {
        this.SetSubType(value);
      }
    }

    /// <xmlbegin id='5'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='MediaTypeBuilder'/> class.</summary>
    public MediaTypeBuilder() {
      this.parameters = new Dictionary<string, string>();
      this.type = "application";
      this.subtype = "octet-stream";
    }

    /// <xmlbegin id='6'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='MediaTypeBuilder'/> class.</summary>
    /// <param name='mt'>The parameter <paramref name='mt'/> is a MediaType
    /// object.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='mt'/> is null.</exception>
    public MediaTypeBuilder(MediaType mt) {
      if (mt == null) {
        throw new ArgumentNullException(nameof(mt));
      }
      this.parameters = new Dictionary<string, string>(mt.Parameters);
      this.type = mt.TopLevelType;
      this.subtype = mt.SubType;
    }

    /// <xmlbegin id='7'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='MediaTypeBuilder'/> class.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is a text
    /// string.</param>
    /// <param name='subtype'>The parameter <paramref name='subtype'/> is a
    /// text string.</param>
    public MediaTypeBuilder(string type, string subtype) {
      this.parameters = new Dictionary<string, string>();
      this.SetTopLevelType(type);
      this.SetSubType(subtype);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.MediaTypeBuilder.IsText"]/*'/>
    public bool IsText {
      get {
        return this.TopLevelType.Equals("text");
      }
    }

    /// <xmlbegin id='8'/>
    /// <value/>
    public bool IsMultipart {
      get {
        return this.TopLevelType.Equals("multipart");
      }
    }

    /// <xmlbegin id='9'/>
    /// <returns>A MediaType object.</returns>
    public MediaType ToMediaType() {
      return new MediaType(this.type, this.subtype, this.parameters);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.MediaTypeBuilder.SetTopLevelType(System.String)"]/*'/>
    public MediaTypeBuilder SetTopLevelType(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length == 0) {
        throw new ArgumentException("str is empty.");
      }
      if (MediaType.SkipMimeTypeSubtype(str, 0, str.Length, null) !=
        str.Length) {
        throw new ArgumentException("Not a well-formed top level type: " + str);
      }
      this.type = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.MediaTypeBuilder.RemoveParameter(System.String)"]/*'/>
    public MediaTypeBuilder RemoveParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      this.parameters.Remove(DataUtilities.ToLowerCaseAscii(name));
      return this;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.MediaTypeBuilder.SetParameter(System.String,System.String)"]/*'/>
    public MediaTypeBuilder SetParameter(string name, string value) {
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.MediaTypeBuilder.SetSubType(System.String)"]/*'/>
    public MediaTypeBuilder SetSubType(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (str.Length == 0) {
        throw new ArgumentException("str is empty.");
      }
      if (MediaType.SkipMimeTypeSubtype(str, 0, str.Length, null) !=
        str.Length) {
        throw new ArgumentException("Not a well-formed subtype: " + str);
      }
      this.subtype = DataUtilities.ToLowerCaseAscii(str);
      return this;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.MediaTypeBuilder.ToString"]/*'/>
    public override string ToString() {
      return this.ToMediaType().ToString();
    }
  }
}
