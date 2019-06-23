/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.ContentDisposition"]/*'/>
  public class ContentDisposition {
    private readonly string dispositionType;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.DispositionType"]/*'/>
    public string DispositionType {
      get {
        return this.dispositionType;
      }
    }

    #region Equals and GetHashCode implementation

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var other = obj as ContentDisposition;
      if (other == null) {
        return false;
      }
      return this.dispositionType.Equals(other.dispositionType) &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetHashCode"]/*'/>
    public override int GetHashCode() {
      var hashCode = 632580499;
      if (this.dispositionType != null) {
        for (var i = 0; i < this.dispositionType.Length; ++i) {
 hashCode = unchecked(hashCode + (632580503 *
          this.dispositionType[i]));
 }
      }
      if (this.parameters != null) {
        hashCode = unchecked(hashCode + (632580587 *
                this.parameters.Count));
      }
      return hashCode;
    }
    #endregion

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.IsInline"]/*'/>
    public bool IsInline {
      get {
        return this.dispositionType.Equals("inline");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.IsAttachment"]/*'/>
    public bool IsAttachment {
      get {
        return this.dispositionType.Equals("attachment");
      }
    }

    internal ContentDisposition(
 string type,
 IDictionary<string, string> parameters) {
      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }
      this.dispositionType = type;
      this.parameters = new Dictionary<string, string>(parameters);
    }

    private ContentDisposition() {
      this.dispositionType = String.Empty;
      this.parameters = new Dictionary<string, string>();
    }

    private readonly IDictionary<string, string> parameters;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.ContentDisposition.Parameters"]/*'/>
    public IDictionary<string, string> Parameters {
      get {
        return new ReadOnlyMap<string, string>(this.parameters);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.ToString"]/*'/>
    public override string ToString() {
      // NOTE: 21 is the length of "Content-Disposition: " (with
      // trailing space).
      var sa = new HeaderEncoder(Message.MaxRecHeaderLineLength, 21);
      sa.AppendSymbol(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sa);
      return sa.ToString();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.ToSingleLineString"]/*'/>
    public string ToSingleLineString() {
      // NOTE: 21 is the length of "Content-Disposition: " (with trailing
      // space).
      var sa = new HeaderEncoder(-1, 21);
      sa.AppendSymbol(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sa);
      return sa.ToString();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.MakeFilename(System.String)"]/*'/>
    public static string MakeFilename(string str) {
      return MakeFilenameMethod.MakeFilename(str);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetFilename"]/*'/>
    public string GetFilename() {
      return MakeFilename(this.GetParameter("filename"));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetCreationDate"]/*'/>
    public int[] GetCreationDate() {
      return MailDateTime.ParseDateString(
        this.GetParameter("creation-date"));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetModificationDate"]/*'/>
    public int[] GetModificationDate() {
      return MailDateTime.ParseDateString(
        this.GetParameter("modification-date"));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetReadDate"]/*'/>
    public int[] GetReadDate() {
      return MailDateTime.ParseDateString(
        this.GetParameter("read-date"));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.GetParameter(System.String)"]/*'/>
    public string GetParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return this.parameters.ContainsKey(name) ? this.parameters[name] :
               null;
    }

    private static ContentDisposition ParseDisposition(string str) {
      const bool HttpRules = false;
      var index = 0;
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      int endIndex = str.Length;
      var parameters = new Dictionary<string, string>();
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      int i = MediaType.SkipMimeToken(
  str,
  index,
  endIndex,
  null,
  HttpRules);
      if (i == index) {
        return null;
      }
      string dispoType =
        DataUtilities.ToLowerCaseAscii(str.Substring(index, i - index));
      if (i < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return new ContentDisposition(
          dispoType,
          parameters);
        }
        if (i3 < endIndex && str[i3] != ';') {
          // not followed by ";", so not a content disposition
          return null;
        }
      }
      index = i;
      return MediaType.ParseParameters(
        str,
        index,
        endIndex,
        HttpRules,
        parameters) ? new ContentDisposition(
            dispoType,
            parameters) : null;
    }

    private static ContentDisposition Build(string name) {
      return new ContentDisposition(
        name,
        new Dictionary<string, string>());
    }

#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
#endif

    ///
    /// <summary>The content disposition value "attachment".
    /// </summary>
    ///
    public static readonly ContentDisposition Attachment =
      Build("attachment");

#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="This instance is immutable")]
#endif

    ///
    /// <summary>The content disposition value "inline".
    /// </summary>
    ///
    public static readonly ContentDisposition Inline =
      Build("inline");

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.Parse(System.String)"]/*'/>
    public static ContentDisposition Parse(string dispoValue) {
      if (dispoValue == null) {
        throw new ArgumentNullException(nameof(dispoValue));
      }
      return Parse(dispoValue, Attachment);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.ContentDisposition.Parse(System.String,PeterO.Mail.ContentDisposition)"]/*'/>
    public static ContentDisposition Parse(
  string dispositionValue,
  ContentDisposition defaultValue) {
      if (dispositionValue == null) {
        throw new ArgumentNullException(nameof(dispositionValue));
      }
      ContentDisposition dispo = ParseDisposition(dispositionValue);
      return dispo ?? defaultValue;
    }
  }
}
