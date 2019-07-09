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
    /// path='docs/doc[@name="T:PeterO.Mail.DispositionBuilder"]/*'/>
  public class DispositionBuilder {
    private readonly IDictionary<string, string> parameters;
    private string type;

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Mail.DispositionBuilder.DispositionType"]/*'/>
    public string DispositionType {
      get {
        return this.type;
      }

      set {
        this.SetDispositionType(value);
      }
    }

    /// <xmlbegin id='31'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='DispositionBuilder'/> class.</summary>
    public DispositionBuilder() {
      this.parameters = new Dictionary<string, string>();
      this.type = "attachment";
    }

    /// <xmlbegin id='32'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='DispositionBuilder'/> class.</summary>
    /// <param name='mt'>The parameter <paramref name='mt'/> is a
    /// ContentDisposition object.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='mt'/> is null.</exception>
    public DispositionBuilder(ContentDisposition mt) {
      if (mt == null) {
        throw new ArgumentNullException(nameof(mt));
      }
      this.parameters = new Dictionary<string, string>(mt.Parameters);
      this.type = mt.DispositionType;
    }

    /// <xmlbegin id='33'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='DispositionBuilder'/> class.</summary>
    /// <param name='type'>The parameter <paramref name='type'/> is a text
    /// string.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='type'/> is null.</exception>
    /// <exception cref='T:System.ArgumentException'>Type is
    /// empty.</exception>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Mail.DispositionBuilder.IsText"]/*'/>
    [Obsolete(
      "Irrelevant for content dispositions; will be removed in the future.")]
    public bool IsText {
      get {
        return this.DispositionType.Equals("text");
      }
    }

    /// <xmlbegin id='34'/>
    /// <value/>
    [Obsolete(
      "Irrelevant for content dispositions; will be removed in the future.")]
    public bool IsMultipart {
      get {
        return this.DispositionType.Equals("multipart");
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.DispositionBuilder.ToDisposition"]/*'/>
    public ContentDisposition ToDisposition() {
      return new ContentDisposition(this.type, this.parameters);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.DispositionBuilder.SetDispositionType(System.String)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.DispositionBuilder.RemoveParameter(System.String)"]/*'/>
    public DispositionBuilder RemoveParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      this.parameters.Remove(DataUtilities.ToLowerCaseAscii(name));
      return this;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.DispositionBuilder.SetParameter(System.String,System.String)"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.DispositionBuilder.ToString"]/*'/>
    public override string ToString() {
      return this.ToDisposition().ToString();
    }
  }
}
