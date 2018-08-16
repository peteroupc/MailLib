/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Mail.Transforms;
using PeterO.Text;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.Message"]/*'/>
  public sealed class Message {
    internal const int MaxRecHeaderLineLength = 78;
    internal const int MaxShortHeaderLineLength = 76;
    internal const int MaxHardHeaderLineLength = 998;

    private const int EncodingBase64 = 2;
    private const int EncodingBinary = 4;
    private const int EncodingEightBit = 3;
    private const int EncodingQuotedPrintable = 1;
    private const int EncodingSevenBit = 0;
    private const int EncodingUnknown = -1;

    // NOTE: System.Random is not a cryptographic RNG.
    // If security is a concern, replace this call to System.Random
    // to the interface of a cryptographic RNG.
    private static readonly Random ValueMsgidRandom = new Random();
    private static readonly object ValueSequenceSync = new Object();

    private static readonly IDictionary<string, int> ValueHeaderIndices =
      MakeHeaderIndices();

    private readonly IList<string> headers;

    private readonly IList<Message> parts;

    private static int msgidSequence;
    private static bool seqFirstTime = true;
    private byte[] body;
    private ContentDisposition contentDisposition;

    private MediaType contentType;

    private int transferEncoding;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.#ctor(System.IO.Stream)"]/*'/>
    public Message(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      this.headers = new List<string>();
      this.parts = new List<Message>();
      this.body = new byte[0];
      IByteReader transform = DataIO.ToReader(stream);
      this.ReadMessage(transform);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.#ctor(System.Byte[])"]/*'/>
    public Message(byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      this.headers = new List<string>();
      this.parts = new List<Message>();
      this.body = new byte[0];
      IByteReader transform = DataIO.ToReader(bytes);
      this.ReadMessage(transform);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.#ctor"]/*'/>
    public Message() {
      this.headers = new List<string>();
      this.parts = new List<Message>();
      this.body = new byte[0];
      this.contentType = MediaType.TextPlainUtf8;
      this.headers.Add("message-id");
      this.headers.Add(this.GenerateMessageID());
      this.headers.Add("from");
      this.headers.Add("me@from-address.invalid");
      this.headers.Add("mime-version");
      this.headers.Add("1.0");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.NewBodyPart"]/*'/>
    public static Message NewBodyPart() {
      var msg = new Message();
      msg.contentType = MediaType.TextPlainAscii;
      // No headers by default (see RFC 2046 sec. 5.1)
      msg.headers.Clear();
      return msg;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetCurrentDate"]/*'/>
    public Message SetCurrentDate() {
      return this.SetDate(DateTimeUtilities.GetCurrentLocalTime());
    }

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (var i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.BccAddresses"]/*'/>
    [Obsolete("Use GetAddresses(\"Bcc\") instead.")]
    public IList<NamedAddress> BccAddresses {
      get {
return GetAddresses("bcc");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.BodyString"]/*'/>
    public string BodyString {
      get {
        if (this.ContentType.IsMultipart) {
          throw new

  NotSupportedException("This is a multipart message, so it doesn't have its own body text.");
        }
        ICharacterEncoding charset = Encodings.GetEncoding(
          this.ContentType.GetCharset(),
          true);
        if (charset == null) {
          throw new
            NotSupportedException("Not in a supported character encoding.");
        }
        return Encodings.DecodeToString(
          charset,
          DataIO.ToReader(this.body));
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.CCAddresses"]/*'/>
    [Obsolete("Use GetAddresses(\"Cc\") instead.")]
    public IList<NamedAddress> CCAddresses {
      get {
return GetAddresses("cc");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.ContentDisposition"]/*'/>
    public ContentDisposition ContentDisposition {
      get {
        return this.contentDisposition;
      }

      set {
        if (value == null) {
          this.contentDisposition = null;
          this.RemoveHeader("content-disposition");
        } else if (!value.Equals(this.contentDisposition)) {
          this.contentDisposition = value;
          this.SetHeader(
     "content-disposition",
     this.contentDisposition.ToString());
        }
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.ContentType"]/*'/>
    public MediaType ContentType {
      get {
        return this.contentType;
      }

      set {
        if (value == null) {
          throw new ArgumentNullException(nameof(value));
        }
        if (this.contentType == null ||
            !this.contentType.Equals(value)) {
          this.contentType = value;
          if (!value.IsMultipart) {
            IList<Message> thisParts = this.Parts;
            thisParts.Clear();
          }
          this.SetHeader("content-type", this.contentType.ToString());
        }
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.FileName"]/*'/>
    public string FileName {
      get {
        ContentDisposition disp = this.contentDisposition;
        return (disp != null) ?
          ContentDisposition.MakeFilename(disp.GetParameter("filename")) :
        ContentDisposition.MakeFilename(this.contentType.GetParameter(
  "name"));
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GetAddresses(System.String)"]/*'/>
    public IList<NamedAddress> GetAddresses(string headerName) {
      if (headerName == null) {
        throw new ArgumentNullException(nameof(headerName));
      }
      if (headerName.Length == 0) {
        throw new ArgumentException("headerName" + " is empty.");
      }
      headerName = DataUtilities.ToLowerCaseAscii(headerName);
      if (ValueHeaderIndices.ContainsKey(headerName) &&
         ValueHeaderIndices[headerName] <= 5) {
        // TODO: Maybe support Resent-*
        return ParseAddresses(this.GetMultipleHeaders(headerName));
      } else {
        throw new NotSupportedException("Not supported for: " + headerName);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.FromAddresses"]/*'/>
    [Obsolete("Use GetAddresses(\"From\") instead.")]
    public IList<NamedAddress> FromAddresses {
      get {
return GetAddresses("from");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.HeaderFields"]/*'/>
    public IList<KeyValuePair<string, string>> HeaderFields {
      get {
        var list = new List<KeyValuePair<string, string>>();
        for (int i = 0; i < this.headers.Count; i += 2) {
          list.Add(
            new KeyValuePair<string, string>(
              this.headers[i],
              this.headers[i + 1]));
        }
        return list;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.Parts"]/*'/>
    public IList<Message> Parts {
      get {
        return this.parts;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.Subject"]/*'/>
    public string Subject {
      get {
        return this.GetHeader("subject");
      }

      set {
        this.SetHeader("subject", value);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.ToAddresses"]/*'/>
    [Obsolete("Use GetAddresses(\"To\") instead.")]
    public IList<NamedAddress> ToAddresses {
      get {
return GetAddresses("to");
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddHeader(System.Collections.Generic.KeyValuePair{System.String,System.String})"]/*'/>
    public Message AddHeader(KeyValuePair<string, string> header) {
      return this.AddHeader(header.Key, header.Value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddHeader(System.String,System.String)"]/*'/>
    public Message AddHeader(string name, string value) {
      name = ValidateHeaderField(name, value);
      int index = this.headers.Count / 2;
      this.headers.Add(String.Empty);
      this.headers.Add(String.Empty);
      return this.SetHeader(index, name, value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.Generate"]/*'/>
    public string Generate() {
      var aw = new ArrayWriter();
      this.Generate(aw, 0);
      return DataUtilities.GetUtf8String(aw.ToArray(), false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GenerateBytes"]/*'/>
    public byte[] GenerateBytes() {
      var aw = new ArrayWriter();
      this.Generate(aw, 0);
      return aw.ToArray();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GetBody"]/*'/>
    public byte[] GetBody() {
      return this.body;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GetDate"]/*'/>
    public int[] GetDate() {
      string field = this.GetHeader("date");
      return (field == null) ? (null) : (MailDateTime.ParseDateString(field,
             true));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetDate(System.Int32[])"]/*'/>
    public Message SetDate(int[] dateTime) {
      if (dateTime == null) {
        throw new ArgumentNullException(nameof(dateTime));
      }
      if (!DateTimeUtilities.IsValidDateTime(dateTime)) {
        throw new ArgumentException("Invalid date and time");
      }
      if (dateTime[0] < 0) {
        throw new ArgumentException("Invalid year: " +
          ParserUtility.IntToString(dateTime[0]));
      }
      return this.SetHeader(
        "date",
        MailDateTime.GenerateDateString(dateTime));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GetBodyMessage"]/*'/>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1024",
      Justification="This method may throw MessageDataException among other things - making it too heavyweight to be a property.")]
#endif
    public Message GetBodyMessage() {
      return (this.ContentType.TopLevelType.Equals("message") &&
          (this.ContentType.SubType.Equals("rfc822") ||
           this.ContentType.SubType.Equals("news") ||
           this.ContentType.SubType.Equals("global"))) ? (new
             Message(this.body)) : null;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GetHeader(System.Int32)"]/*'/>
    public KeyValuePair<string, string> GetHeader(int index) {
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.Count / 2)) {
        throw new ArgumentException("index (" + index +
                    ") is not less than " + (this.headers.Count
                    / 2));
      }
      return new KeyValuePair<string, string>(
        this.headers[index],
        this.headers[index + 1]);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GetHeader(System.String)"]/*'/>
    public string GetHeader(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          // Get the first instance of the header field
          return this.headers[i + 1];
        }
      }
      return null;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.GetHeaderArray(System.String)"]/*'/>
    public string[] GetHeaderArray(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      var list = new List<string>();
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          list.Add(this.headers[i + 1]);
        }
      }
      return (string[])list.ToArray();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.ClearHeaders"]/*'/>
    public Message ClearHeaders() {
      this.headers.Clear();
      this.contentType = MediaType.TextPlainAscii;
      this.contentDisposition = null;
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.RemoveHeader(System.Int32)"]/*'/>
    public Message RemoveHeader(int index) {
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.Count / 2)) {
        throw new ArgumentException("index (" + index +
                    ") is not less than " + (this.headers.Count
                    / 2));
      }
      string name = this.headers[index * 2];
      this.headers.RemoveAt(index * 2);
      this.headers.RemoveAt(index * 2);
      if (name.Equals("content-type")) {
        this.contentType = MediaType.TextPlainAscii;
      } else if (name.Equals("content-disposition")) {
        this.contentDisposition = null;
      }
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.RemoveHeader(System.String)"]/*'/>
    public Message RemoveHeader(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      // Remove the header field
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          this.headers.RemoveAt(i);
          this.headers.RemoveAt(i);
          i -= 2;
        }
      }
      if (name.Equals("content-type")) {
        this.contentType = MediaType.TextPlainAscii;
      } else if (name.Equals("content-disposition")) {
        this.contentDisposition = null;
      }
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetBody(System.Byte[])"]/*'/>
    public Message SetBody(byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      this.body = bytes;
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetHeader(System.Int32,System.Collections.Generic.KeyValuePair{System.String,System.String})"]/*'/>
    public Message SetHeader(int index, KeyValuePair<string, string> header) {
      return this.SetHeader(index, header.Key, header.Value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetHeader(System.Int32,System.String,System.String)"]/*'/>
    public Message SetHeader(int index, string name, string value) {
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.Count / 2)) {
        throw new ArgumentException("index (" + index +
                    ") is not less than " + (this.headers.Count
                    / 2));
      }
      name = ValidateHeaderField(name, value);
      this.headers[index * 2] = name;
      this.headers[(index * 2) + 1] = value;
      if (name.Equals("content-type")) {
        this.contentType = MediaType.Parse(value);
      } else if (name.Equals("content-disposition")) {
        this.contentDisposition = ContentDisposition.Parse(value);
      }
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetHeader(System.Int32,System.String)"]/*'/>
    public Message SetHeader(int index, string value) {
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.Count / 2)) {
        throw new ArgumentException("index (" + index +
                    ") is not less than " + (this.headers.Count
                    / 2));
      }
      return this.SetHeader(index, this.headers[index * 2], value);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>Not documented yet.</param>
    /// <param name='value'>Not documented yet.</param>
    /// <returns>A string object.</returns>
    public static string DecodeHeaderValue(string name, string value) {
      return HeaderFieldParsers.GetParser(name).DecodeEncodedWords(value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetHeader(System.String,System.String)"]/*'/>
    public Message SetHeader(string name, string value) {
      name = ValidateHeaderField(name, value);
      // Add the header field
      var index = 0;
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          return this.SetHeader(index, name, value);
        }
        ++index;
      }
      this.headers.Add(String.Empty);
      this.headers.Add(String.Empty);
      return this.SetHeader(index, name, value);
    }

    private static readonly MediaType TextHtmlAscii =
      MediaType.Parse("text/html; charset=us-ascii");

    private static readonly MediaType TextHtmlUtf8 =
      MediaType.Parse("text/html; charset=utf-8");

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetHtmlBody(System.String)"]/*'/>
    public Message SetHtmlBody(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      this.body = DataUtilities.GetUtf8Bytes(str, true, true);
      this.ContentType = IsShortAndAllAscii(str) ? TextHtmlAscii :
        TextHtmlUtf8;
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetTextAndHtml(System.String,System.String)"]/*'/>
    public Message SetTextAndHtml(string text, string html) {
      if (text == null) {
        throw new ArgumentNullException(nameof(text));
      }
      if (html == null) {
        throw new ArgumentNullException(nameof(html));
      }
      // The spec for multipart/alternative (RFC 2046) says that
      // the fanciest version of the message should go last (in
      // this case, the HTML version)
      Message textMessage = NewBodyPart().SetTextBody(text);
      Message htmlMessage = NewBodyPart().SetHtmlBody(html);
    string mtypestr = "multipart/alternative; boundary=\"=_Boundary00000000\"" ;
      this.ContentType = MediaType.Parse(mtypestr);
      IList<Message> messageParts = this.Parts;
      messageParts.Clear();
      messageParts.Add(textMessage);
      messageParts.Add(htmlMessage);
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.SetTextBody(System.String)"]/*'/>
    public Message SetTextBody(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      this.body = DataUtilities.GetUtf8Bytes(str, true, true);
      this.ContentType = IsShortAndAllAscii(str) ? MediaType.TextPlainAscii :
        MediaType.TextPlainUtf8;
      return this;
    }

    private Message AddBodyPart(
         Stream inputStream,
         MediaType mediaType,
         string filename,
         string disposition) {
      return AddBodyPart(inputStream, mediaType, filename, disposition, false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddInline(PeterO.Mail.MediaType)"]/*'/>
    public Message AddInline(MediaType mediaType) {
      return AddBodyPart(null, mediaType, null, "inline", true);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddAttachment(PeterO.Mail.MediaType)"]/*'/>
    public Message AddAttachment(MediaType mediaType) {
      return AddBodyPart(null, mediaType, null, "attachment", true);
    }

    private Message AddBodyPart(
             Stream inputStream,
             MediaType mediaType,
             string filename,
             string disposition,
             bool allowNullStream) {
      if (!allowNullStream && (inputStream) == null) {
        throw new ArgumentNullException(nameof(inputStream));
      }
      if ((mediaType) == null) {
        throw new ArgumentNullException(nameof(mediaType));
      }
      Message bodyPart = NewBodyPart();
      bodyPart.SetHeader("content-id", this.GenerateMessageID());
      // NOTE: Using the setter because it also adds a Content-Type
      // header field
      bodyPart.ContentType = mediaType;
      if (inputStream != null) {
        try {
          using (var ms = new MemoryStream()) {
            var buffer = new byte[4096];
            while (true) {
              int cp = inputStream.Read(buffer, 0, buffer.Length);
              if (cp <= 0) {
                break;
              }
              ms.Write(buffer, 0, cp);
            }
            bodyPart.SetBody(ms.ToArray());
          }
        } catch (IOException ex) {
          throw new MessageDataException("An I/O error occurred.", ex);
        }
      }
      var dispBuilder = new DispositionBuilder(disposition);
      if (!String.IsNullOrEmpty(filename)) {
        string basename = BaseName(filename);
        if (!String.IsNullOrEmpty(basename)) {
          dispBuilder.SetParameter("filename",
            basename);
        }
      }
      bodyPart.ContentDisposition = dispBuilder.ToDisposition();
      if (this.ContentType.IsMultipart) {
        this.Parts.Add(bodyPart);
      } else {
        Message existingBody = NewBodyPart();
        existingBody.ContentDisposition = ContentDisposition.Parse("inline");
        existingBody.ContentType = this.ContentType;
        existingBody.SetBody(this.GetBody());
        string mtypestr = "multipart/mixed; boundary=\"=_Boundary00000000\"";
        this.ContentType = MediaType.Parse(mtypestr);
        this.Parts.Add(existingBody);
        this.Parts.Add(bodyPart);
      }
      return bodyPart;
    }
    private static string BaseName(string filename) {
      var i = filename.Length-1;
      for (; i >= 0; --i) {
        if (filename[i] == '\\' || filename[i] == '/') {
          return filename.Substring(i + 1);
        }
      }
      return filename;
    }

    private static string ExtensionName(string filename) {
      var i = filename.Length-1;
      for (; i >= 0; --i) {
        if (filename[i] == '\\' || filename[i] == '/') {
          return String.Empty;
        } else if (filename[i] == '.') {
          return filename.Substring(i);
        }
      }
      return String.Empty;
    }

    private static MediaType SuggestMediaType(string filename) {
      if (!String.IsNullOrEmpty(filename)) {
        string ext = DataUtilities.ToLowerCaseAscii(
           ExtensionName(filename));
        if (ext.Equals(".doc") || ext.Equals(".dot")) {
          return MediaType.Parse("application/msword");
        }
        if (ext.Equals(".bin") || ext.Equals(".deploy") || ext.Equals(".msp") ||
          ext.Equals(".msu")) {
          return MediaType.Parse("application/octet-stream");
        }
        if (ext.Equals(".pdf")) {
          return MediaType.Parse("application/pdf");
        }
        if (ext.Equals(".key")) {
          return MediaType.Parse("application/pgp-keys");
        }
        if (ext.Equals(".sig")) {
          return MediaType.Parse("application/pgp-signature");
        }
        if (ext.Equals(".rtf")) {
          return MediaType.Parse("application/rtf");
        }
        if (ext.Equals(".docx")) {
          return

  MediaType.Parse("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        if (ext.Equals(".zip")) {
          return MediaType.Parse("application/zip");
        }
        if (ext.Equals(".m4a") || ext.Equals(".mp2") || ext.Equals(".mp3") ||
          ext.Equals(".mpega") || ext.Equals(".mpga")) {
          return MediaType.Parse("audio/mpeg");
        }
        if (ext.Equals(".gif")) {
          return MediaType.Parse("image/gif");
        }
        if (ext.Equals(".jpe") || ext.Equals(".jpeg") || ext.Equals(".jpg")) {
          return MediaType.Parse("image/jpeg");
        }
        if (ext.Equals(".png")) {
          return MediaType.Parse("image/png");
        }
        if (ext.Equals(".tif") || ext.Equals(".tiff")) {
          return MediaType.Parse("image/tiff");
        }
        if (ext.Equals(".eml")) {
          return MediaType.Parse("message/rfc822");
        }
        if (ext.Equals(".htm") || ext.Equals(".html") || ext.Equals(".shtml")) {
          return MediaType.Parse("text/html\u003bcharset=utf-8");
        }
        if (ext.Equals(".asc") || ext.Equals(".brf") || ext.Equals(".pot") ||
          ext.Equals(".srt") || ext.Equals(".text") || ext.Equals(".txt")) {
          return MediaType.Parse("text/plain\u003bcharset=utf-8");
        }
      }
      return MediaType.ApplicationOctetStream;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddAttachment(System.IO.Stream,PeterO.Mail.MediaType)"]/*'/>
    public Message AddAttachment(Stream inputStream, MediaType mediaType) {
      return AddBodyPart(inputStream, mediaType, null, "attachment");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddAttachment(System.IO.Stream,System.String)"]/*'/>
    public Message AddAttachment(Stream inputStream, string filename) {
      return
  AddBodyPart(inputStream, SuggestMediaType(filename), filename, "attachment");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddAttachment(System.IO.Stream,PeterO.Mail.MediaType,System.String)"]/*'/>
    public Message AddAttachment(Stream inputStream, MediaType mediaType,
      string filename) {
      return AddBodyPart(inputStream, mediaType, filename, "attachment");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddInline(System.IO.Stream,PeterO.Mail.MediaType)"]/*'/>
    public Message AddInline(Stream inputStream, MediaType mediaType) {
      return AddBodyPart(inputStream, mediaType, null, "inline");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddInline(System.IO.Stream,System.String)"]/*'/>
    public Message AddInline(Stream inputStream, string filename) {
      return AddBodyPart(inputStream, SuggestMediaType(filename), filename,
        "inline");
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.AddInline(System.IO.Stream,PeterO.Mail.MediaType,System.String)"]/*'/>
    public Message AddInline(Stream inputStream, MediaType mediaType, string
      filename) {
      return AddBodyPart(inputStream, mediaType, filename, "inline");
    }

    private static bool HasSameAddresses(Message m1, Message m2) {
      IList<NamedAddress> n1 = m1.GetAddresses("from");
      IList<NamedAddress> n2 = m2.GetAddresses("from");
      if (n1.Count != n2.Count) {
        return false;
      }
      for (var i = 0; i < n1.Count; ++i) {
        if (!n1[i].AddressesEqual(n2[i])) {
          return false;
        }
      }
      return true;
    }
private static string GetContentTranslationType(string ctt) {
      if (String.IsNullOrEmpty(ctt)) {
 return String.Empty;
}
      int index = HeaderParser.ParseFWS(ctt, 0, ctt.Length, null);
      int cttEnd = HeaderParser.ParsePhraseAtom(ctt, index, ctt.Length, null);
      if (cttEnd != ctt.Length) {
 return String.Empty;
}
      return DataUtilities.ToLowerCaseAscii(
         ctt.Substring(index, cttEnd - index));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='languages'>Not documented yet.</param>
    /// <returns>A Message object.</returns>
    public Message SelectLanguageMessage(
       IList<string> languages) {
      return SelectLanguageMessage(languages, false);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='languages'>Not documented yet.</param>
    /// <param name='preferOriginals'>Not documented yet.</param>
    /// <returns>A Message object.</returns>
    public Message SelectLanguageMessage(
       IList<string> languages,
       bool preferOriginals) {
      if (this.ContentType.TypeAndSubType.Equals("multipart/multilingual") &&
         this.Parts.Count >= 2) {
        string subject = this.GetHeader("subject");
        int passes = (preferOriginals) ? 2 : 1;
        IList<string> clang;
        IList<string> filt;
        for (var i = 0; i < passes; ++i) {
 foreach (Message part in this.Parts) {
      clang = LanguageTags.GetLanguageList(part.GetHeader(
  "content-language"));
            if (clang == null) {
 continue;
}
         if (preferOriginals && i == 0) {  // Allow originals only, on first
              string ctt =
  GetContentTranslationType(part.GetHeader("content-translation-type"));
              if (!ctt.Equals("original")) {
                continue;
              }
            }
            filt = LanguageTags.LanguageTagFilter(languages, clang);
            if (filt.Count > 0) {
              Message ret = part.GetBodyMessage();
              if (ret != null) {
                if (subject!=null && ret.GetHeader("subject") == null) {
 ret.SetHeader("subject", subject);
}
                return ret;
              }
            }
          }
        }
        // Fall back
        Message firstmsg = this.Parts[1];
        Message lastPart = this.Parts[this.Parts.Count - 1];
        IList<string> zxx = new List<string>(new string[] { "zxx" });
        clang = LanguageTags.GetLanguageList(
          lastPart.GetHeader("content-language"));
        if (clang != null) {
          filt = LanguageTags.LanguageTagFilter(zxx, clang);
          if (filt.Count > 0) {
            firstmsg = lastPart;
          }
        }
        firstmsg = firstmsg.GetBodyMessage();
        if (firstmsg != null) {
          if (subject!=null && firstmsg.GetHeader("subject") == null) {
 firstmsg.SetHeader("subject", subject);
}
          return firstmsg;
        }
      }
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Message.MakeMultilingualMessage(System.Collections.Generic.IList{PeterO.Mail.Message},System.Collections.Generic.IList{System.String})"]/*'/>
    public static Message MakeMultilingualMessage(IList<Message> messages,
      IList<string> languages) {
      if ((messages) == null) {
        throw new ArgumentNullException(nameof(messages));
      }
      if ((languages) == null) {
        throw new ArgumentNullException(nameof(languages));
      }
      if (messages.Count < 0) {
        throw new ArgumentException("messages.Count (" + messages.Count +
          ") is less than 0");
      }
      if ((messages.Count)!=(languages.Count)) {
        throw new ArgumentException("messages.Count (" + messages.Count +
          ") is not equal to " + (languages.Count));
      }
      for (var i = 0; i < messages.Count; ++i) {
        if (messages[i] == null) {
 throw new ArgumentException("messages");
}
        if (i > 0 && !HasSameAddresses(messages[0], messages[i])) {
          throw new ArgumentException(
            "Each message doesn't contain the same email addresses");
        }
      }
        foreach (string lang in languages) {
        IList<string> langtags = LanguageTags.GetLanguageList(lang);
          if (langtags == null) {
            throw new ArgumentException(
            lang + " is an invalid list of language tags");
          }
        }
      var prefaceBody = new StringBuilder().Append("This is a multilingual " +
        "message, a message that\r\ncan be read in one or more different " +
        "languages. Each\r\npart of the message may appear inline, as an " +
        "attachment, or both.\r\n\r\n");
      prefaceBody.Append("Languages available:\r\n\r\n");
      foreach (string lang in languages) {
        prefaceBody.Append("- ").Append(lang).Append("\r\n");
      }
      var prefaceSubject = new StringBuilder();
      var zxx = new List<string>();
      zxx.Add("zxx");
      for (var i = 0; i < languages.Count; ++i) {
        IList<string> langs = LanguageTags.GetLanguageList(languages[i]);
        bool langInd=(i == languages.Count-1 && langs.Count == 1 &&
          langs[0].Equals("zxx"));
        if (!langInd && LanguageTags.LanguageTagFilter(
        zxx,
        langs).Count>0) {
          throw new ArgumentException("zxx tag can only appear at end");
        }
        string subject = messages[i].GetHeader("subject");
        if (!String.IsNullOrEmpty(subject)) {
          if (prefaceSubject.Length > 0) {
            prefaceSubject.Append(" / ");
          }
          prefaceSubject.Append(subject);
        }
      }
      if (prefaceSubject.Length == 0) {
        prefaceSubject.Append("Multilingual Message");
      prefaceSubject.Append(" (");
      for (var i = 0; i < languages.Count; ++i) {
        if (i > 0) {
          prefaceSubject.Append(", ");
        }
        prefaceSubject.Append(languages[i]);
      }
      prefaceSubject.Append(")");
      }
      string fromHeader = messages[0].GetHeader("from");
      if (fromHeader == null) {
        throw new ArgumentException("First message has no From header");
      }
      var msg = new Message();
      msg.ContentType = MediaType.Parse("multipart/multilingual");
      msg.SetHeader("from", fromHeader);
      msg.ContentDisposition = PeterO.Mail.ContentDisposition.Inline;
      string toHeader = messages[0].GetHeader("to");
      if (toHeader != null) {
        msg.SetHeader("to", toHeader);
      }
      msg.SetHeader("subject", prefaceSubject.ToString());
      var preface = msg.AddInline(MediaType.Parse("text/plain;charset=utf-8"));
      preface.SetTextBody(prefaceBody.ToString());
      for (var i = 0; i < messages.Count; ++i) {
        MediaType mt=MediaType.Parse("message/rfc822");
        string msgstring = messages[i].Generate();
        if (msgstring.IndexOf("\r\n--") >= 0 || msgstring.IndexOf("--")==0) {
          // Message/global allows quoted-printable and
          // base64, so we can avoid raw boundary delimiters
          mt = MediaType.Parse("message/global");
        }
        Message part = msg.AddInline(mt);
        part.SetHeader("content-language", languages[i]);
        part.SetBody(DataUtilities.GetUtf8Bytes(msgstring, true));
      }
      return msg;
    }

    internal static bool CanBeUnencoded(
      byte[] bytes,
      bool checkBoundaryDelimiter) {
      if (bytes == null || bytes.Length == 0) {
        return true;
      }
      var lineLength = 0;
      var index = 0;
      int endIndex = bytes.Length;
      var headers = true;
      while (index < endIndex) {
        int c = ((int)bytes[index]) & 0xff;
        if (c >= 0x80) {
          // Console.WriteLine("Non-ASCII character (0x {0:X2})",(int)c);
          return false;
        }
        if (lineLength == 0 && checkBoundaryDelimiter && index + 4 < endIndex &&
            bytes[index] == '-' && bytes[index + 1] == '-' &&
            bytes[index + 2] == '=' && bytes[index + 3] == '_' &&
            bytes[index + 4] == 'B') {
          // Start of a reserved boundary delimiter
          return false;
        }
        if (lineLength == 0 && index + 4 < endIndex &&
            bytes[index] == 'F' && bytes[index + 1] == 'r' &&
            bytes[index + 2] == 'o' && bytes[index + 3] == 'm' &&
            (bytes[index + 4] == ' ' || bytes[index + 4] == '\t')) {
          // Line starts with "From" followed by space
          return false;
        }
        if (lineLength == 0 && index < endIndex &&
            bytes[index] == '.') {
          // Dot at beginning of line
          return false;
        }
        if (c == '\r' && index + 1 < endIndex && bytes[index + 1] == '\n') {
          index += 2;
          if (headers && lineLength == 0) {
            // Start of the body
            headers = false;
          }
          lineLength = 0;
          continue;
        }
        if (c == '\r' || c == '\n') {
          // Console.WriteLine("Bare CR or bare LF");
          return false;
        }
        ++lineLength;
        if (lineLength > MaxRecHeaderLineLength) {
          // Console.WriteLine("Line length exceeded (" + lineLength +
          // " " + (str.Substring(index-MaxRecHeaderLineLength,
          // MaxRecHeaderLineLength)) + ")");
          return false;
        }
        ++index;
      }
      return true;
    }

    // Parse the delivery status byte array to downgrade
    // the Original-Recipient and Final-Recipient header fields
    // NOTE: RFC 3464 (delivery status notifications) refers
    // to RFC 822 for its message conventions (sec. 2.1.1), while RFC
    // 8098 (disposition notifications) refers to RFC 5322 for its
    // conventions (sec. 3.1.1). While RFC 822 allows white
    // space and comments to appear between lexical tokens
    // of structured header fields (see sec. 3.1.4 and
    // note in particular sec. A.3.3, which
    // shows white space between the header field name and the
    // colon), RFC 5322 doesn't (it refers to
    // RFC 5234, which states that linear white space is no longer
    // implicit in ABNF productions). However, RFC 5322 includes
    // an obsolete syntax allowing optional white space to appear
    // between the header field and the colon (sec. 4.5.8).
    internal static byte[] DowngradeDeliveryStatus(byte[] bytes) {
      var sb = new StringBuilder();
      var index = 0;
      int endIndex = bytes.Length;
      var lastIndex = -1;
      ArrayWriter writer = null;
      while (index < endIndex) {
        sb.Remove(0, sb.Length);
        var first = true;
        int headerNameStart = index;
        int headerNameEnd = index;
        // lineCount = 0;
        var endOfHeaders = false;
        while (true) {
          if (index >= endIndex) {
            // All headers read
            endOfHeaders = true;
            break;
          }
          int c = (index < endIndex) ? (((int)bytes[index]) & 0xff) : -1;
          // ++lineCount;
          ++index;
          if (c == '\r') {
            c = (index < endIndex) ? (((int)bytes[index]) & 0xff) : -1;
            ++index;
            if (c == '\n') {
              // lineCount = 0;
              headerNameStart = index;
            } else {
              --index;
              headerNameEnd = index;
            }
            continue;
          }
          if ((c >= 0x21 && c <= 57) || (c >= 59 && c <= 0x7e)) {
            first = false;
            if (c >= 'A' && c <= 'Z') {
              c += 0x20;
            }
            sb.Append((char)c);
          } else if (!first && c == ':') {
            break;
          } else {
            first &= c != 0x20 && c != 0x09;
          }
          // NOTE: Allows obsolete syntax between header field
          // name and colon. As a result, a header
          // field starting with "Example :" will be treated as a header
          // field named "Example" in this code.
          if (c != 0x20 && c != 0x09) {
            headerNameEnd = index;
          }
        }
        if (endOfHeaders) {
          break;
        }
        int headerValueStart = index;
        int headerValueEnd = index;
        string fieldName = DataUtilities.ToLowerCaseAscii(
          DataUtilities.GetUtf8String(
            bytes,
            headerNameStart,
            headerNameEnd - headerNameStart,
            true));
        bool origRecipient = fieldName.Equals("original-recipient");
        bool finalRecipient = fieldName.Equals("final-recipient");
        // Read the header field value using UTF-8 characters
        // rather than bytes
        while (true) {
          if (index >= endIndex) {
            // All headers read
            headerValueEnd = index;
            break;
          }
          int c = (index < endIndex) ? (((int)bytes[index]) & 0xff) : -1;
          ++index;
          if (c == '\r') {
            c = (index < endIndex) ? (((int)bytes[index]) & 0xff) : -1;
            ++index;
            if (c == '\n') {
              // lineCount = 0;
              // Parse obsolete folding whitespace (obs-fws) under RFC5322
              // (parsed according to errata), same as LWSP in RFC5234
              var fwsFirst = true;
              var haveFWS = false;
              var lineStart = true;
              while (true) {
                // Skip the CRLF pair, if any (except if iterating for
                // the first time, since CRLF was already parsed)
                if (!fwsFirst) {
                  c = (index < endIndex) ? (((int)bytes[index]) & 0xff) : -1;
                  ++index;
                  if (c == '\r') {
                    c = (index < endIndex) ? (((int)bytes[index]) & 0xff) : -1;
                    ++index;
                    if (c == '\n') {
                    // CRLF was read
                    lineStart = true;
                    } else {
                    // It's the first part of the line, where the header name
                    // should be, so the CR here is illegal
                    throw new MessageDataException("CR not followed by LF");
                    }
                  } else {
                    // anything else, unget
                    --index;
                  }
                }
                fwsFirst = false;
                // Use ReadByte here since we're just looking for the single
                // byte characters space and tab
                int c2 = (index < endIndex) ? (((int)bytes[index]) & 0xff) : -1;
                ++index;
                if (c2 == 0x20 || c2 == 0x09) {
                  lineStart = false;
                  haveFWS = true;
                } else {
                  --index;
                  // this isn't space or tab; if this is the start
                  // of the line, this is no longer FWS
                  haveFWS &= !lineStart;
                  break;
                }
              }
              if (haveFWS) {
                // We have folding whitespace, line
                // count found as above
                continue;
              }
              // This ends the header field
              // (the last two characters will be CRLF)
              headerValueEnd = index - 2;
              break;
            }
            --index;
            // ++lineCount;
          }
          // ++lineCount;
        }
        if (origRecipient || finalRecipient) {
          string headerValue = null;
          var status = new int[1];
          try {
            headerValue = DataUtilities.GetUtf8String(
            bytes,
            headerValueStart,
            headerValueEnd - headerValueStart,
            false);  // throws on invalid UTF-8
          } catch (ArgumentException) {
            // Invalid UTF-8, so encapsulate
            headerValue = null;
            status[0] = 2;
          }
          if (headerValue != null) {
            headerValue = DowngradeRecipientHeaderValue(
              fieldName,
              headerValue,
              status);
          }
          if (status[0] == 2 || status[0] == 1) {
            // Downgraded (1) or encapsulated (2)
            if (writer == null) {
              writer = new ArrayWriter();
              writer.Write(bytes, 0, headerNameStart);
            } else {
              writer.Write(bytes, lastIndex, headerNameStart - lastIndex);
            }
            if (status[0] == 1) {
              // Downgraded
              byte[] newBytes = DataUtilities.GetUtf8Bytes(
                 headerValue,
                 true);
              writer.Write(newBytes, 0, newBytes.Length);
            } else {
              // Encapsulated
              string field = (origRecipient ?
       "Downgraded-Original-Recipient" : "Downgraded-Final-Recipient");
              headerValue = DataUtilities.GetUtf8String(
                  bytes,
                  headerValueStart,
                  headerValueEnd - headerValueStart,
                  true);  // replaces invalid UTF-8
              string newField = HeaderEncoder.EncodeFieldAsEncodedWords(field,
                headerValue);
              byte[] newBytes = DataUtilities.GetUtf8Bytes(
                newField,
                true);
              writer.Write(newBytes, 0, newBytes.Length);
            }
          }
          lastIndex = headerValueEnd;
        }
      }
      if (writer != null) {
        writer.Write(bytes, lastIndex, bytes.Length - lastIndex);
        bytes = writer.ToArray();
      }
      return bytes;
    }

    private static HeaderEncoder EncodeCommentsInText(HeaderEncoder enc,
      string str) {
      var i = 0;
      var begin = 0;
      if (str.IndexOf('(') < 0) return enc.AppendString(str);
      var sb = new StringBuilder();
      while (i < str.Length) {
        if (str[i] == '(') {
          int si = HeaderParserUtility.ParseCommentLax(str, i, str.Length,
            null);
          if (si != i) {
            enc.AppendString(str, begin, i);
            Rfc2047.EncodeComment(enc, str, i, si);
            i = si;
            begin = si;
            continue;
          }
        }
        ++i;
      }
      return enc.AppendString(str, begin, str.Length);
    }

    internal static string DowngradeRecipientHeaderValue(
      string fieldName,
      string headerValue,
      int[] status) {
      int index;
      if (
        HasTextToEscape(
          headerValue,
          0,
          headerValue.Length)) {
        index = HeaderParser.ParseCFWS(
          headerValue,
          0,
          headerValue.Length,
          null);
        int atomText = HeaderParser.ParsePhraseAtom(
          headerValue,
          index,
          headerValue.Length,
          null);
        int typeEnd = atomText;
        string origValue = headerValue;
        bool isUtf8 = typeEnd - index == 5 &&
          (headerValue[index] & ~0x20) == 'U' && (headerValue[index +
               1] & ~0x20) == 'T' && (headerValue[index + 2] & ~0x20) == 'F'
                    &&
          headerValue[index + 3] == '-' && headerValue[index + 4] == '8';
        atomText = HeaderParser.ParseCFWS(
          headerValue,
          atomText,
          headerValue.Length,
          null);
        // NOTE: Commented out for now (see below)
        //if (atomText != typeEnd) {
        // isUtf8 = false;
        //}
        if (index < headerValue.Length && headerValue[atomText] == ';') {
          int addressPart = HeaderParser.ParseCFWS(
           headerValue,
           atomText + 1,
           headerValue.Length,
           null);
          var encoder = new HeaderEncoder().AppendFieldName(fieldName);
          if (isUtf8) {
            string typePart = headerValue.Substring(0, addressPart);
            // Downgrade the non-ASCII characters in the address
            // NOTE: The ABNF for utf-8-type-addr in RFC 6533
            // appears not to allow linear white space.
            // DEVIATION: Allow CFWS between "utf-8" and semicolon
            // for sake of robustness, even though it doesn't fit
            // utf-8-type-addr (see also RFC 8098 secs. 3.2.3
            // and 3.2.4)
            var builder = new StringBuilder();
            const string ValueHex = "0123456789ABCDEF";
            for (int i = addressPart; i < headerValue.Length; ++i) {
              if (headerValue[i] < 0x7f && headerValue[i] > 0x20 &&
                headerValue[i] != '\\' && headerValue[i] != '+' &&
                   headerValue[i] != '-') {
                builder.Append(headerValue[i]);
              } else {
                int cp = DataUtilities.CodePointAt(headerValue, i);
                if (cp >= 0x10000) {
                  ++i;
                }
                builder.Append("\\x");
                builder.Append('{');
                for (int j = 20; j >= 0; j -= 4) {
                  if ((cp >> j) != 0) {
                    builder.Append(ValueHex[(cp >> j) & 15]);
                  }
                }
                builder.Append('}');
              }
            }
            // Do comment downgrading last for sake of convenience
            // (especially since RFC 8098 (disposition notifications) allows
            // CFWS at the end of the address, though a conflict exists between
            // that RFC and utf-8-type-addr, which technically allows
            // parentheses to appear in the address).
            // NOTE: The syntax for Original-Recipient and Final-Recipient
            // header
            // field values (in RFC 3464, delivery status notifications) has a
            // structured part and an
            // unstructured part (generic-address defined as "*text") (see
            // RFC 3464 secs.
            // 2.3.1 and 2.3.2, which uses the conventions in RFC
            // 822, where linear white space can appear between lexical
            // tokens of a header field).
            EncodeCommentsInText(encoder,
                    HeaderEncoder.TrimLeadingFWS(typePart + builder));
          } else {
            EncodeCommentsInText(encoder,
                    HeaderEncoder.TrimLeadingFWS(headerValue));
          }
          headerValue = encoder.ToString();
        }
        if (
          HasTextToEscape(
            headerValue,
            0,
            headerValue.Length)) {
          if (status != null) {
            // Encapsulated
            status[0] = 2;
          }
          return null;
        }
        if (status != null) {
          // Downgraded
          status[0] = 1;
        }
        return headerValue;
      }
      if (status != null) {
        status[0] = 0;  // Unchanged
      }
      return null;
    }

    internal static string GenerateAddressList(IList<NamedAddress> list) {
      var sb = new StringBuilder();
      for (int i = 0; i < list.Count; ++i) {
        if (i > 0) {
          sb.Append(", ");
        }
        sb.Append(list[i].ToString());
      }
      return sb.ToString();
    }

    internal static bool HasTextToEscapeOrEncodedWordStarts(string s) {
      // <summary>Returns true if the string has:
      // * non-ASCII characters,
      // * "=?",
      // * CTLs other than tab, or
      // * a word longer than MaxRecHeaderLineLength minus 1 characters.
      // Can return false even if the string has:
      // * CRLF followed by a line with just whitespace.</summary>
      return HasTextToEscapeOrEncodedWordStarts(s, 0, s.Length, true);
    }

    internal static bool HasTextToEscapeOrEncodedWordStarts(string s, int
      index, int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, true);
    }

    internal static bool HasTextToEscape(string s, int index, int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, false);
    }

    internal static bool HasTextToEscape(string s) {
      return HasTextToEscapeOrEncodedWordStarts(s, 0, s.Length, false);
    }

    internal static bool HasTextToEscapeOrEncodedWordStarts(string s, int
      index, int endIndex, bool checkEWStarts) {
      int len = endIndex;
      var chunkLength = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = s[i];
        if (checkEWStarts && c == '=' && i + 1 < len && c == '?') {
          // "=?" (start of an encoded word)
          return true;
        }
        if (c == 0x0d) {
          if (i + 1 >= len || s[i + 1] != 0x0a) {
            // bare CR
            // Console.WriteLine("bare CR");
            return true;
          }
          if (i + 2 >= len || (s[i + 2] != 0x09 && s[i + 2] != 0x20)) {
            // CRLF not followed by whitespace
            return true;
          }
          chunkLength = 0;
          ++i;
          continue;
        }
        if (c == 0x0a) {
          // bare LF
          return true;
        }
        if (c >= 0x7f || (c < 0x20 && c != 0x09 && c != 0x0d)) {
          // CTLs (except TAB, SPACE, and CR) and non-ASCII
          // characters
          return true;
        }
        if (c == 0x20 || c == 0x09) {
          chunkLength = 0;
        } else {
          ++chunkLength;
          if (chunkLength > MaxRecHeaderLineLength - 1) {
            return true;
          }
        }
      }
      return false;
    }

    internal static IList<NamedAddress> ParseAddresses(string value) {
return ParseAddresses(new string[] { value});
    }

    internal static IList<NamedAddress> ParseAddresses(string[] values) {
      var list = new List<NamedAddress>();
      foreach (string addressValue in values) {
        if (addressValue == null) {
          continue;
        }
        var tokener = new Tokener();
        if (
          HeaderParser.ParseHeaderTo(
            addressValue,
            0,
            addressValue.Length,
            tokener) != addressValue.Length) {
          // Invalid syntax
          continue;
        }
        list.AddRange(
          HeaderParserUtility.ParseAddressList(
            addressValue,
            0,
            addressValue.Length,
            tokener.GetTokens()));
      }
      return list;
    }

    internal static int ParseUnstructuredText(string s, int startIndex, int
      endIndex) {
      // Parses "unstructured" in RFC 5322 without obsolete syntax
      // and with non-ASCII characters allowed
      for (int i = startIndex; i < endIndex;) {
        int c = DataUtilities.CodePointAt(s, i, 2);
        // NOTE: Unpaired surrogates are replaced with -1
        if (c == -1) {
          return i;
        } else if (c >= 0x10000) {
          {
            i += 2;
          }
        } else if (c == 0x0d) {
          if (i + 2 >= endIndex || s[i + 1] != 0x0a || (s[i + 2] != 0x09 &&
              s[i + 2] != 0x20)) {
            // bare CR, or CRLF not followed by SP/TAB
            return i;
          }
          i += 3;
          var found = false;
          for (int j = i; j < endIndex; ++j) {
            if (s[j] != 0x09 && s[j] != 0x20 && s[j] != 0x0d) {
              found = true; break;
            } else if (s[j] == 0x0d) {
              // Possible CRLF after all-whitespace line
              return i;
            }
          }
          if (!found) {
            // CRLF followed by an all-whitespace line
            return i;
          }
        } else {
          if (c == 0x7f || (c < 0x20 && c != 0x09 && c != 0x0d)) {
            // CTLs (except TAB, SPACE, and CR)
            return i;
          }
          ++i;
        }
      }
      return endIndex;
    }

    private static void AppendAscii(IWriter output, string str) {
      for (var i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c >= 0x80) {
          throw new MessageDataException("ASCII expected");
        }
        output.WriteByte((byte)c);
      }
    }

    private static string GenerateBoundary(int num) {
      var sb = new StringBuilder();
      const string ValueHex = "0123456789ABCDEF";
      sb.Append("=_Boundary");
      for (int i = 0; i < 4; ++i) {
        int b = (num >> 56) & 255;
        sb.Append(ValueHex[(b >> 4) & 15]);
        sb.Append(ValueHex[b & 15]);
        num <<= 8;
      }
      return sb.ToString();
    }

    private static string Implode(string[] strings, string delim) {
      if (strings.Length == 0) {
        return String.Empty;
      }
      if (strings.Length == 1) {
        return strings[0];
      }
      var sb = new StringBuilder();
      var first = true;
      foreach (string s in strings) {
        if (!first) {
          sb.Append(delim);
        }
        sb.Append(s);
        first = false;
      }
      return sb.ToString();
    }

    private static bool IsShortAndAllAscii(string str) {
      if (str.Length > 0x10000) {
        return false;
      }
      for (int i = 0; i < str.Length; ++i) {
        if ((((int)str[i]) >> 7) != 0) {
          return false;
        }
      }
      return true;
    }

    private static bool IsWellFormedBoundary(string str) {
      if (String.IsNullOrEmpty(str) || str.Length > 70) {
        return false;
      }
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (i == str.Length - 1 && c == 0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
          (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <=
            '9') || c == 0x20 || c == 0x2c || "'()-./+_:=?".IndexOf(c) >=
                    0)) {
          return false;
        }
      }
      return true;
    }

    private static IDictionary<string, int> MakeHeaderIndices() {
      var dict = new Dictionary<string, int>();
      dict["to"] = 0;
      dict["cc"] = 1;
      dict["bcc"] = 2;
      dict["from"] = 3;
      dict["sender"] = 4;
      dict["reply-to"] = 5;
      dict["resent-to"] = 6;
      dict["resent-cc"] = 7;
      dict["resent-bcc"] = 8;
      dict["resent-from"] = 9;
      dict["resent-sender"] = 10;
      return dict;
    }

    private static IByteReader MakeTransferEncoding(
      IByteReader stream,
      int encoding,
      bool useLiberalSevenBit) {
      IByteReader transform = new EightBitTransform(stream);
      switch (encoding) {
        case EncodingQuotedPrintable:
          // NOTE: The max line size is actually 76, but some emails
          // have lines that exceed this size, so use an unlimited line length
          // when parsing
          transform = new QuotedPrintableTransform(stream, true, -1);
          break;
        case EncodingBase64:
          // NOTE: Same as quoted-printable regarding line length
          transform = new Base64Transform(stream, false, -1, false);
          break;
        case EncodingEightBit:
          transform = new EightBitTransform(stream);
          break;
        case EncodingBinary:
          transform = stream;
          break;
        case EncodingSevenBit:
          // DEVIATION: Replace 8-bit bytes and null with the
          // ASCII substitute character (0x1a) for text/plain messages,
          // non-MIME messages, and the preamble and epilogue of multipart
          // messages (which will be ignored).
          transform = useLiberalSevenBit ?
            ((IByteReader)new LiberalSevenBitTransform(stream)) :
            ((IByteReader)new SevenBitTransform(stream));
          break;
      }

      return transform;
    }

    private static void ReadHeaders(
      IByteReader stream,
      ICollection<string> headerList,
      bool start) {
      var lineCount = 0;
      var bytesRead = new int[1];
      var sb = new StringBuilder();
      var ss = 0;
      var ungetLast = false;
      var lastByte = 0;
      while (true) {
        sb.Remove(0, sb.Length);
        var first = true;
        var endOfHeaders = false;
        var wsp = false;
        lineCount = 0;
        while (true) {
          int c = ungetLast ? lastByte : stream.ReadByte();
          ungetLast = false;
          if (start && ss >= 0) {
            if (ss == 0 && c == 'F') {
              ++ss;
            } else if (ss == 1 && c == 'r') {
              ++ss;
            } else if (ss == 2 && c == 'o') {
              ++ss;
            } else if (ss == 3 && c == 'm') {
              ++ss;
            } else if (ss == 4 && c == ' ') {
              ++ss;
            } else {
              ss = -1;
            }
          }
          if (c == -1) {
            throw new

  MessageDataException("Premature end of message before all headers were read, while reading header field name");
          }
          ++lineCount;
          if (first && c == '\r') {
            if (stream.ReadByte() == '\n') {
              endOfHeaders = true;
              break;
            }
            throw new MessageDataException("CR not followed by LF");
          }
          if ((c >= 0x21 && c <= 57) || (c >= 59 && c <= 0x7e)) {
            if (wsp) {
              throw new
                MessageDataException("Whitespace within header field name");
            }
            first = false;
            if (c >= 'A' && c <= 'Z') {
              c += 0x20;
            }
            sb.Append((char)c);
          } else if (!first && c == ':') {
            if (lineCount > Message.MaxHardHeaderLineLength) {
              // MaxHardHeaderLineLength characters includes the colon
              throw new MessageDataException("Header field name too long");
            }
            break;
          } else if (c == 0x20 || c == 0x09) {
            if (ss == 5) {
              ss = -1;
              // Possible Mbox convention
              var possibleMbox = true;
              var isFromField = false;
              sb.Remove(0, sb.Length);
              while (true) {
                c = stream.ReadByte();
                if (c == -1) {
                  throw new MessageDataException(
  "Premature end before all headers were read (Mbox convention)");
                } else if (c == ':' && possibleMbox) {
                  // Full fledged From header field
                  isFromField = true;
                  sb.Append("from");
                  start = false;
                  wsp = false;
                  first = true;
                  break;
                } else if (c == '\n') {
                  // End of line was reached
                  possibleMbox = false;
                  start = false;
                  wsp = false;
                  first = true;
                  break;
                } else if (c != 0x20) {
                  possibleMbox = false;
                }
              }
              if (isFromField) {
                break;
              }
            } else {
              wsp = true;
              first = false;
            }
          } else {
            throw new MessageDataException("Malformed header field name");
          }
        }
        if (endOfHeaders) {
          break;
        }
        if (sb.Length == 0) {
          throw new MessageDataException("Empty header field name");
        }
        // Set the header field name to the
        // string builder's current value
        string fieldName = sb.ToString();
        // Clear the string builder to read the
        // header field's value
        sb.Remove(0, sb.Length);
        // Skip initial spaces in the header field value,
        // to keep them from being added by the string builder
        while (true) {
          lastByte = stream.ReadByte();
          if (lastByte != 0x20) {
            ungetLast = true;
            break;
          }
          ++lineCount;
        }
        // Read the header field value using UTF-8 characters
        // rather than bytes (DEVIATION: RFC 6532 allows UTF-8
        // in header field values, but not everywhere in these values,
        // as is done here for convenience)
        while (true) {
          // We're only looking for the single-byte CR, so
          // there's no need to use ReadUtf8Char for now
          int c = ungetLast ? lastByte : stream.ReadByte();
          ungetLast = false;
          if (c == -1) {
            string exstring = "Premature end before all headers were read," +
              "while reading header field value";
            throw new MessageDataException(exstring);
          }
          if (c == '\r') {
            // We're only looking for the single-byte LF, so
            // there's no need to use ReadUtf8Char
            c = stream.ReadByte();
            if (c < 0) {
              string exstring = "Premature end before all headers were read," +
                "while looking for LF";
              throw new MessageDataException(exstring);
            }
            if (c == '\n') {
              lineCount = 0;
              // Parse obsolete folding whitespace (obs-fws) under RFC5322
              // (parsed according to errata), same as LWSP in RFC5234
              var fwsFirst = true;
              var haveFWS = false;
              while (true) {
                // Skip the CRLF pair, if any (except if iterating for
                // the first time, since CRLF was already parsed)
                // Use ReadByte here since we're just looking for the single
                // byte characters CR and LF
                if (!fwsFirst) {
                  c = ungetLast ? lastByte : stream.ReadByte();
                  ungetLast = false;
                  if (c == '\r') {
                    c = stream.ReadByte();
                    if (c == '\n') {
                    // CRLF was read
                    lineCount = 0;
                    } else {
                    // It's the first part of the line, where the header name
                    // should be, so the CR here is illegal
                    throw new MessageDataException("CR not followed by LF");
                    }
                  } else {
                    // anything else, unget
                    ungetLast = true;
                    lastByte = c;
                  }
                }
                fwsFirst = false;
                // Use ReadByte here since we're just looking for the single
                // byte characters space and tab
                int c2 = ungetLast ? lastByte : stream.ReadByte();
                ungetLast = false;
                if (c2 == 0x20 || c2 == 0x09) {
                  ++lineCount;
                  // Don't write SPACE as the first character of the value
                  if (c2 != 0x20 || sb.Length != 0) {
                    sb.Append((char)c2);
                  }
                  haveFWS = true;
                } else {
                  ungetLast = true;
                  lastByte = c2;
                  // this isn't space or tab; if this is the start
                  // of the line, this is no longer FWS
            haveFWS &= lineCount != 0;
                  break;
                }
              }
              if (haveFWS) {
                // We have folding whitespace, line
                // count found as above
                continue;
              }
              // This ends the header field
              break;
            }
            sb.Append('\r');
            ungetLast = true;
            lastByte = c;
            ++lineCount;  // Increment for the CR
          }
          // NOTE: Header field line limit not enforced here, only
          // in the header field name; it's impossible to generate
          // a conforming message if the name is too long
          // NOTE: Some emails still have 8-bit bytes in an unencoded
          // subject line
          // or other unstructured header field; however, since RFC6532,
          // we can just assume the UTF-8 encoding in these cases; in
          // case the bytes are not valid UTF-8, a replacement character
          // will be output
          if (c < 0x80) {
            sb.Append((char)c);
            ++lineCount;
          } else {
            int[] state = { lineCount, c, 1 };
            c = ReadUtf8Char(stream, state);
            //DebugUtility.Log("c=" + c + "," + lineCount + "," +
             // state[0]+ ","+state[1]+","+state[2]);
            lineCount = state[0];
            ungetLast = (state[2] == 1);
            lastByte = state[1];
            if (c <= 0xffff) {
              sb.Append((char)c);
            } else if (c <= 0x10ffff) {
              sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              sb.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        }
        string fieldValue = sb.ToString();
        headerList.Add(fieldName);
        headerList.Add(fieldValue);
      }
    }

    private static int ReadUtf8Char(
      IByteReader stream,
      int[] ungetState) {
      // ungetState is:
      // 0: line count in bytes
      // 1: last byte read (or -1 for EOF)
      // 2: whether to unget the last byte (1 if true)
      if (stream == null) {
        throw new ArgumentNullException(nameof(stream));
      }
      // NOTE: Currently assumes the last byte read
      // is 0x80 or greater (non-ASCII). This excludes
      // CR, which complicates a bit how line count in ungetState
      // is handled
      #if DEBUG
if ((ungetState[1]) < 0x80) {
  throw new ArgumentException("ungetState[1] (" + ungetState[1] +
    ") is less than " + 0x80);
}
#endif

      var cp = 0;
      var bytesSeen = 0;
      var bytesNeeded = 0;
      var lower = 0x80;
      var upper = 0xbf;
      int read = ungetState[0];
      while (true) {
        int b = ungetState[2] == 1 ?
          ungetState[1] : stream.ReadByte();
        ungetState[2] = 0;
        ++read;
        if (b < 0) {
          if (bytesNeeded != 0) {
            // Invalid multibyte character at end
            ungetState[2] = 1;  // unget last
            ungetState[1] = b;  // last byte
            --read;
            ungetState[0] = read;
            return 0xfffd;
          }
          return -1;
        }
        if (bytesNeeded == 0) {
          if ((b & 0x7f) == b) {
            // Valid single byte character
            ungetState[0] = read;
            return b;
          }
          if (b >= 0xc2 && b <= 0xdf) {
            bytesNeeded = 1;
            cp = (b - 0xc0) << 6;
          } else if (b >= 0xe0 && b <= 0xef) {
            lower = (b == 0xe0) ? 0xa0 : 0x80;
            upper = (b == 0xed) ? 0x9f : 0xbf;
            bytesNeeded = 2;
            cp = (b - 0xe0) << 12;
          } else if (b >= 0xf0 && b <= 0xf4) {
            lower = (b == 0xf0) ? 0x90 : 0x80;
            upper = (b == 0xf4) ? 0x8f : 0xbf;
            bytesNeeded = 3;
            cp = (b - 0xf0) << 18;
          } else {
            // Invalid starting byte
            ungetState[0] = read;
            return 0xfffd;
          }
          continue;
        }
        if (b < lower || b > upper) {
          // Invalid multibyte character
          ungetState[2] = 1;  // unget last
          ungetState[1] = b;  // last byte
          --read;
          ungetState[0] = read;
          return 0xfffd;
        }
        lower = 0x80;
        upper = 0xbf;
        ++bytesSeen;
        cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
        if (bytesSeen != bytesNeeded) {
          continue;
        }
        // Valid multibyte character
        ungetState[0] = read;
        return cp;
      }
    }

    private static bool StartsWithWhitespace(string str) {
      return str.Length > 0 && (str[0] == ' ' || str[0] == 0x09 || str[0] ==
                    '\r');
    }

    private static int TransferEncodingToUse(byte[] body, bool isBodyPart) {
      if (body == null || body.Length == 0) {
        return EncodingSevenBit;
      }
      int lengthCheck = Math.Min(body.Length, 1024);
      var highBytes = 0;
      var ctlBytes = 0;
      var lineLength = 0;
      // Assume 'allTextBytes' is false if this is a body part or not
      // all of the body is checked
      bool allTextBytes = (!isBodyPart) && lengthCheck == body.Length;
      for (int i = 0; i < lengthCheck; ++i) {
        if (i > 0 && (allTextBytes ? (i % 108 == 0) : (i % 36 == 0))) {
          if (highBytes + ctlBytes > i / 3) {
            return EncodingBase64;
          }
          if (!allTextBytes) {
            return EncodingQuotedPrintable;
          }
        }
        if ((body[i] & 0x80) != 0) {
          ++highBytes;
          allTextBytes = false;
        } else if (body[i] == 0x00) {
          allTextBytes = false;
          ++ctlBytes;
        } else if (body[i] == 0x09) {
          // TAB
          allTextBytes = false;
        } else if (body[i] == 0x7f ||
            (body[i] < 0x20 && body[i] != 0x0d && body[i] != 0x0a && body[i] !=
                0x09)) {
          allTextBytes = false;
          ++ctlBytes;
        } else if (body[i] == (byte)'\r') {
          if (i + 1 >= body.Length || body[i + 1] != (byte)'\n') {
            // bare CR
            allTextBytes = false;
          } else if (i > 0 && (body[i - 1] == (byte)' ' || body[i - 1] ==
                 (byte)'\t')) {
            // Space followed immediately by CRLF
            allTextBytes = false;
          } else if (i + 2 < body.Length && body[i + 1] == (byte)'\n' &&
                    body[i + 2] == (byte)'.') {
            // CR LF dot (see RFC 5321 secs. 4.1.1.4 and 4.5.2)
            allTextBytes = false;
          } else {
            ++i;
            lineLength = 0;
            continue;
          }
        } else {
          // not bare LF
          allTextBytes &= body[i] != (byte)'\n';
        }
        allTextBytes &= lineLength != 0 || i >= body.Length || body[i] !=
          '.';
        allTextBytes &= lineLength != 0 || i + 4 >= body.Length || body[i] !=
          'F' || body[i + 1] != 'r' || body[i + 2] != 'o' || body[i + 3] !=
                'm' || (body[i + 4] != ' ' && body[i + 4] != '\t');
        allTextBytes &= lineLength != 0 || i + 1 >= body.Length || body[i] !=
          '-' || body[i + 1] != '-';
        ++lineLength;
        allTextBytes &= lineLength <= MaxShortHeaderLineLength;
      }
      return (allTextBytes) ? EncodingSevenBit :
    ((highBytes > lengthCheck / 3) ? EncodingBase64 :
          EncodingQuotedPrintable);
    }

    private static string ValidateHeaderField(string name, string value) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      if (value == null) {
        throw new ArgumentNullException(nameof(value));
      }
      if (name.Length > MaxHardHeaderLineLength - 1) {
        throw new ArgumentException("Header field name too long");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < name.Length; ++i) {
        if (name[i] <= 0x20 || name[i] == ':' || name[i] >= 0x7f) {
          throw new
  ArgumentException("Header field name contains an invalid character");
        }
      }
      // Check characters in structured header fields
      IHeaderFieldParser parser = HeaderFieldParsers.GetParser(name);
      if (parser.IsStructured()) {
        if (ParseUnstructuredText(value, 0, value.Length) != value.Length) {
       throw new ArgumentException("Header field value contains invalid text");
        }
        if (parser.Parse(value, 0, value.Length, null) != value.Length) {
          throw new
  ArgumentException("Header field value is not in the correct format");
        }
      }
      return name;
    }

    private void Generate(IWriter output, int depth) {
      var haveMimeVersion = false;
      var haveContentEncoding = false;
      var haveContentType = false;
      var haveContentDisp = false;
      var haveMsgId = false;
      var haveFrom = false;
      var haveDate = false;
      var haveHeaders = new bool[11];
      byte[] bodyToWrite = this.body;
      var builder = new MediaTypeBuilder(this.ContentType);
      string contentDisp = (this.ContentDisposition == null) ? null :
        this.ContentDisposition.ToString();
      var transferEnc = 0;
      var isMultipart = false;
      string boundary = String.Empty;
      if (builder.IsMultipart) {
        boundary = GenerateBoundary(depth);
        builder.SetParameter("boundary", boundary);
        isMultipart = true;
      }
      if (!isMultipart) {
        if (builder.TopLevelType.Equals("message")) {
          if (builder.SubType.Equals("delivery-status") ||
              builder.SubType.Equals("global-delivery-status") ||
              builder.SubType.Equals("disposition-notification") ||
              builder.SubType.Equals("global-disposition-notification")) {
            bodyToWrite = DowngradeDeliveryStatus(bodyToWrite);
          }
          bool msgCanBeUnencoded = CanBeUnencoded(bodyToWrite, depth > 0);
          if ((builder.SubType.Equals("rfc822") || builder.SubType.Equals(
            "news")) && !msgCanBeUnencoded) {
            builder.SetSubType("global");
          } else if (builder.SubType.Equals("disposition-notification") &&
                    !msgCanBeUnencoded) {
            builder.SetSubType("global-disposition-notification");
          } else if (builder.SubType.Equals("delivery-status") &&
                    !msgCanBeUnencoded) {
            builder.SetSubType("global-delivery-status");
          } else if (!msgCanBeUnencoded && !builder.SubType.Equals("global") &&
            !builder.SubType.Equals("global-disposition-notification") &&
            !builder.SubType.Equals("global-delivery-status") &&
            !builder.SubType.Equals("global-headers")) {
#if DEBUG
            throw new MessageDataException("Message body can't be encoded: " +
              builder.ToString() + ", " + this.ContentType);
#else
{
 throw new MessageDataException("Message body can't be encoded");
}
#endif
          }
        }
      }
      string topLevel = builder.TopLevelType;
      // NOTE: RFC 6532 allows any transfer encoding for the
      // four global message types listed below.
      transferEnc = topLevel.Equals("message") ||
        topLevel.Equals("multipart") ? (topLevel.Equals("multipart") || (
          !builder.SubType.Equals("global") &&
          !builder.SubType.Equals("global-headers") &&
          !builder.SubType.Equals("global-disposition-notification") &&
          !builder.SubType.Equals("global-delivery-status"))) ?
          EncodingSevenBit : TransferEncodingToUse(
            bodyToWrite,
            depth > 0) : TransferEncodingToUse(bodyToWrite, depth > 0);
      string encodingString = "7bit";
      if (transferEnc == EncodingBase64) {
        encodingString = "base64";
      } else if (transferEnc == EncodingQuotedPrintable) {
        encodingString = "quoted-printable";
      }
      // Write the header fields
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        string rawField = null;
        if (name.Equals("content-type")) {
          if (haveContentType) {
            // Already outputted, continue
            continue;
          }
          haveContentType = true;
          value = builder.ToString();
        }
        if (name.Equals("content-disposition")) {
          if (haveContentDisp || contentDisp == null) {
            // Already outputted, continue
            continue;
          }
          haveContentDisp = true;
          value = contentDisp;
        } else if (name.Equals("content-transfer-encoding")) {
          if (haveContentEncoding) {
            // Already outputted, continue
            continue;
          }
          haveContentEncoding = true;
          value = encodingString;
        } else if (name.Equals("date")) {
          if (haveDate) {
            continue;
          }
          haveDate = true;
        } else if (name.Equals("from")) {
          if (haveFrom) {
            // Already outputted, continue
            continue;
          }
          haveFrom = true;
        }
        if (
          depth > 0 && name.IndexOf("--")==0) {
          // don't generate header fields starting with "--"
          // in body parts
          continue;
        }
        if (name.Equals("mime-version")) {
          if (depth > 0) {
            // Don't output if this is a body part
            continue;
          }
          haveMimeVersion = true;
        } else if (name.Equals("message-id")) {
          if (depth > 0) {
            // Don't output if this is a body part
            continue;
          }
          if (haveMsgId) {
            // Already outputted, continue
            continue;
          }
          haveMsgId = true;
        } else {
          if (ValueHeaderIndices.ContainsKey(name)) {
            if (depth > 0) {
              // Don't output if this is a body part
              continue;
            }
            int headerIndex = ValueHeaderIndices[name];
            if (headerIndex <= 5) {
              if (haveHeaders[headerIndex]) {
                // Already outputted, continue
                continue;
              }
              bool isValidAddressing = this.IsValidAddressingField(name);
              haveHeaders[headerIndex] = true;
              /*DebugUtility.Log (name+" "+isValidAddressing);
                {
                  var ssb = new StringBuilder();
                  foreach (var mhs in this.GetMultipleHeaders (name)) {
                    ssb.Append (mhs + " ");
                 if (isValidAddressing && name=="sender") {
                    DebugUtility.Log(""+new NamedAddress(mhs));
                    DebugUtility.Log("" + new NamedAddress(mhs).DisplayName);
                    DebugUtility.Log("" + new NamedAddress(mhs).Address);
                 }
                  }
                  DebugUtility.Log (ssb.ToString());
                }*/
              if (!isValidAddressing) {
                value = GenerateAddressList(
    ParseAddresses(this.GetMultipleHeaders(name)));
                if (value.Length == 0) {
                  // No addresses, synthesize a field
                  rawField = this.SynthesizeField(name);
                }
              }
            } else if (headerIndex <= 10) {
              // Resent-* fields can appear more than once
              value = GenerateAddressList(ParseAddresses(value));
              if (value.Length == 0) {
                // No addresses, synthesize a field
                rawField = this.SynthesizeField(name);
              }
            }
          }
        }
        rawField = rawField ?? (HeaderEncoder.EncodeField(name, value));
        if (HeaderEncoder.CanOutputRaw(rawField)) {
          AppendAscii(output, rawField);
        } else {
          //DebugUtility.Log("Can't output '"+name+"' raw");
          string downgraded = HeaderFieldParsers.GetParser(name)
                    .DowngradeHeaderField(name, value);
          if (
            HasTextToEscape(
              downgraded,
              0,
              downgraded.Length)) {
            if (name.Equals("message-id") ||
                name.Equals("resent-message-id") || name.Equals(
                "in-reply-to") || name.Equals("references") || name.Equals(
                "original-recipient") || name.Equals("final-recipient")) {
              // Header field still contains invalid characters (such
              // as non-ASCII characters in 7-bit messages), convert
              // to a downgraded field
              downgraded = HeaderEncoder.EncodeFieldAsEncodedWords(
                  "downgraded-" + name,
                  ParserUtility.TrimSpaceAndTab(value));
            } else {
#if DEBUG
              throw new
  MessageDataException("Header field still has non-Ascii or controls: " +
                    name + "\n" + downgraded);
#else
               throw new MessageDataException(
                 "Header field still has non-Ascii or controls");
#endif
            }
          } else {
            AppendAscii(
              output,
              downgraded);
          }
        }
        AppendAscii(output, "\r\n");
      }
      if (!haveFrom && depth == 0) {
        // Output a synthetic From field if it doesn't
        // exist and this isn't a body part
        AppendAscii(output, "From: me@author-address.invalid\r\n");
      }
      if (!haveDate && depth == 0) {
        AppendAscii(output, "Date: ");
        AppendAscii(
          output,
  MailDateTime.GenerateDateString(DateTimeUtilities.GetCurrentLocalTime()));
        AppendAscii(output, "\r\n");
      }
      if (!haveMsgId && depth == 0) {
        AppendAscii(
          output,
          HeaderEncoder.EncodeField("Message-ID", this.GenerateMessageID()));
        AppendAscii(output, "\r\n");
      }
      if (!haveMimeVersion && depth == 0) {
        AppendAscii(output, "MIME-Version: 1.0\r\n");
      }
      if (!haveContentType) {
        AppendAscii(output, "Content-Type: " + builder + "\r\n");
      }
      if (!haveContentEncoding) {
        AppendAscii(
      output,
      "Content-Transfer-Encoding: " + encodingString + "\r\n");
      }
      ICharacterEncoder bodyEncoder = null;
      switch (transferEnc) {
        case EncodingBase64:
          bodyEncoder = new Base64Encoder(true, builder.IsText, false);
          break;
        case EncodingQuotedPrintable:
          bodyEncoder = new QuotedPrintableEncoder(
            builder.IsText ? 2 : 0,
            false);
          break;
        default:
          bodyEncoder = new IdentityEncoder();
          break;
      }
      // Write the body
      AppendAscii(output, "\r\n");
      if (!isMultipart) {
        var index = 0;
        while (true) {
          int c = (index >= bodyToWrite.Length) ? -1 :
            ((int)bodyToWrite[index++]) & 0xff;
          int count = bodyEncoder.Encode(c, output);
          if (count == -2) {
            throw new MessageDataException("encoding error");
          }
          if (count == -1) {
            break;
          }
        }
      } else {
        bool writeNewLine = (depth > 0);
        foreach (Message part in this.Parts) {
          if (writeNewLine) {
 AppendAscii(output, "\r\n");
}
          writeNewLine = true;
          AppendAscii(output, "--" + boundary + "\r\n");
          part.Generate(output, depth + 1);
        }
        AppendAscii(output, "\r\n--" + boundary + "--");
      }
    }

    private string GenerateMessageID() {
      var builder = new StringBuilder();
      var seq = 0;
      builder.Append("<");
      lock (ValueSequenceSync) {
        if (seqFirstTime) {
          msgidSequence = ValueMsgidRandom.Next(65536);
          msgidSequence <<= 16;
          msgidSequence |= ValueMsgidRandom.Next(65536);
          seqFirstTime = false;
        }
        seq = unchecked(msgidSequence++);
      }
      const string ValueHex = "0123456789abcdef";
      byte[] ent;
      {
        ent = new byte[16];
        for (var i = 0; i < ent.Length; ++i) {
          ent[i] = unchecked((byte)ValueMsgidRandom.Next(256));
        }
      }
      long ticks = DateTime.UtcNow.Ticks;
      for (int i = 0; i < 10; ++i) {
        builder.Append(ValueHex[(int)(ticks & 15)]);
        ticks >>= 4;
      }
      for (int i = 0; i < ent.Length; ++i) {
        builder.Append(ValueHex[(int)(ent[i] & 15)]);
        builder.Append(ValueHex[(int)((ent[i] >> 4) & 15)]);
      }
      for (int i = 0; i < 8; ++i) {
        builder.Append(ValueHex[seq & 15]);
        seq >>= 4;
      }
      IList<NamedAddress> addresses = this.GetAddresses("from");
      if (addresses == null || addresses.Count == 0) {
        builder.Append("@local.invalid");
      } else {
        builder.Append("@");
        seq = addresses[0].IsGroup ? addresses[0].Name.GetHashCode() :
          addresses[0].Address.ToString().GetHashCode();
        for (int i = 0; i < 8; ++i) {
          builder.Append(ValueHex[seq & 15]);
          seq >>= 4;
        }
        builder.Append(".local.invalid");
      }
      builder.Append(">");
      // DebugUtility.Log(builder.ToString());
      return builder.ToString();
    }

    private string[] GetMultipleHeaders(string name) {
      var headerList = new List<string>();
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          headerList.Add(this.headers[i + 1]);
        }
      }
      return (string[])headerList.ToArray();
    }

    private bool IsValidAddressingField(string name) {
      name = DataUtilities.ToLowerCaseAscii(name);
      var have = false;
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          if (have) {
            return false;
          }
          string headerValue = this.headers[i + 1];
          if (
            HeaderFieldParsers.GetParser(
              name).Parse(
              headerValue,
              0,
              headerValue.Length,
              null) != headerValue.Length) {
            return false;
          }
          have = true;
        }
      }
      return true;
    }

    private void ProcessHeaders(bool assumeMime, bool digest) {
      var haveContentType = false;
      bool mime = assumeMime;
      var haveContentDisp = false;
      string transferEncodingValue = String.Empty;
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("content-transfer-encoding")) {
          int startIndex = HeaderParser.ParseCFWS(value, 0, value.Length, null);
          // NOTE: Actually "token", but all known transfer encoding values
          // fit the same syntax as the stricter one for top-level types and
          // subtypes
          int endIndex = MediaType.SkipMimeTypeSubtype(
            value,
            startIndex,
            value.Length,
            null);
          transferEncodingValue = (
            HeaderParser.ParseCFWS(
              value,
              endIndex,
              value.Length,
              null) == value.Length) ? value.Substring(
  startIndex,
  endIndex - startIndex) : String.Empty;
        }
        mime |= name.Equals("mime-version");
        if (value.IndexOf("=?", StringComparison.Ordinal) >= 0) {
          IHeaderFieldParser parser = HeaderFieldParsers.GetParser(name);
          // Decode encoded words in the header field where possible
          value = parser.DecodeEncodedWords(value);
          this.headers[i + 1] = value;
        }
      }
      MediaType ctype = digest ? MediaType.MessageRfc822 :
        MediaType.TextPlainAscii;
      var haveInvalid = false;
      var haveContentEncoding = false;
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (mime && name.Equals("content-transfer-encoding")) {
          value = DataUtilities.ToLowerCaseAscii(transferEncodingValue);
          this.headers[i + 1] = value;
          if (value.Equals("7bit")) {
            this.transferEncoding = EncodingSevenBit;
          } else if (value.Equals("8bit")) {
            this.transferEncoding = EncodingEightBit;
          } else if (value.Equals("binary")) {
            this.transferEncoding = EncodingBinary;
          } else if (value.Equals("quoted-printable")) {
            this.transferEncoding = EncodingQuotedPrintable;
          } else if (value.Equals("base64")) {
            this.transferEncoding = EncodingBase64;
          } else {
            // Unrecognized transfer encoding
            this.transferEncoding = EncodingUnknown;
          }
          haveContentEncoding = true;
        } else if (mime && name.Equals("content-type")) {
          if (haveContentType) {
            // DEVIATION: If there is already a content type,
            // treat content type as application/octet-stream
            if (haveInvalid || MediaType.Parse(value, null) == null) {
              ctype = MediaType.TextPlainAscii;
              haveInvalid = true;
            } else {
              ctype = MediaType.ApplicationOctetStream;
            }
          } else {
            ctype = MediaType.Parse(
              value,
              null);
            if (ctype == null) {
              ctype = digest ? MediaType.MessageRfc822 :
                MediaType.TextPlainAscii;
              haveInvalid = true;
            }
            // For conformance with RFC 2049
            if (ctype.IsText) {
              if (String.IsNullOrEmpty(ctype.GetCharset())) {
                if (!ctype.StoresCharsetInPayload()) {
                  // Used unless the media type defines how charset
                  // is determined from the payload
                  ctype = MediaType.ApplicationOctetStream;
                }
              } else {
                var builder = new MediaTypeBuilder(ctype)
                    .SetParameter("charset", ctype.GetCharset());
                ctype = builder.ToMediaType();
              }
            }
            haveContentType = true;
          }
        } else if (mime && name.Equals("content-disposition")) {
          if (haveContentDisp) {
            string valueExMessage = "Already have this header: " + name;
#if DEBUG
            valueExMessage += "[old=" + this.contentType + ", new=" + value +
              "]";
            valueExMessage = valueExMessage.Replace("\r\n", " ");
#endif
            throw new MessageDataException(valueExMessage);
          }
          this.contentDisposition = ContentDisposition.Parse(value);
          haveContentDisp = true;
        }
      }
      if (this.transferEncoding == EncodingUnknown) {
        ctype = MediaType.ApplicationOctetStream;
      }
      // Update content type as appropriate
      // NOTE: Setting the field, not the setter,
      // because it's undesirable here to add a Content-Type
      // header field, as the setter does
      this.contentType = ctype;
      if (!haveContentEncoding && this.contentType.TypeAndSubType.Equals(
        "message/rfc822")) {
        // DEVIATION: Be a little more liberal with rfc822
        // messages with 8-bit bytes
        this.transferEncoding = EncodingEightBit;
      }
      if (this.transferEncoding == EncodingSevenBit) {
        string charset = this.contentType.GetCharset();
        if (charset.Equals("utf-8")) {
          // DEVIATION: Be a little more liberal with UTF-8
          this.transferEncoding = EncodingEightBit;
        } else if (this.contentType.TypeAndSubType.Equals("text/html")) {
          if (charset.Equals("us-ascii") ||
              charset.Equals("windows-1252") || charset.Equals(
                "windows-1251") ||
              (charset.Length > 9 && charset.Substring(0, 9).Equals(
                "iso-8859-"))) {
            // DEVIATION: Be a little more liberal with text/html and
            // single-byte charsets or UTF-8
            this.transferEncoding = EncodingEightBit;
          }
        }
      }
      if (this.transferEncoding == EncodingQuotedPrintable ||
          this.transferEncoding == EncodingBase64 ||
          this.transferEncoding == EncodingUnknown) {
        if (this.contentType.IsMultipart ||
            (this.contentType.TopLevelType.Equals("message") &&
             !this.contentType.SubType.Equals("global") &&
             !this.contentType.SubType.Equals("global-headers") &&
             !this.contentType.SubType.Equals(
               "global-disposition-notification") &&
             !this.contentType.SubType.Equals("global-delivery-status"))) {
          if (this.transferEncoding == EncodingQuotedPrintable ||
              this.transferEncoding == EncodingBase64) {
            // DEVIATION: Treat quoted-printable for multipart and message
            // as 7bit instead
            this.transferEncoding = EncodingSevenBit;
          } else {
            string exceptText =
              "Invalid content encoding for multipart or message";
#if DEBUG
            exceptText += " [type=" + this.contentType + "]";
#endif
            throw new MessageDataException(exceptText);
          }
        }
      }
    }

    private void ReadMessage(IByteReader stream) {
      try {
        ReadHeaders(stream, this.headers, true);
        this.ProcessHeaders(false, false);
        if (this.contentType.IsMultipart) {
          this.ReadMultipartBody(stream);
        } else {
          this.ReadSimpleBody(stream);
        }
      } catch (InvalidOperationException ex) {
        throw new MessageDataException(ex.Message, ex);
      }
    }

    private void ReadMultipartBody(IByteReader stream) {
      int baseTransferEncoding = this.transferEncoding;
      IList<MessageStackEntry> multipartStack = new List<MessageStackEntry>();
      var entry = new MessageStackEntry(this);
      multipartStack.Add(entry);
      var boundaryChecker = new BoundaryCheckerTransform(
         stream,
         entry.Boundary);
      // Be liberal on the preamble and epilogue of multipart
      // messages, as they will be ignored.
      IByteReader currentTransform = MakeTransferEncoding(
        boundaryChecker,
        baseTransferEncoding,
        true);
      Message leaf = null;
      var buffer = new byte[8192];
      var bufferCount = 0;
      int bufferLength = buffer.Length;
      this.body = new byte[0];
      var aw = new ArrayWriter();
      {
        while (true) {
          var ch = 0;
          try {
            ch = currentTransform.ReadByte();
          } catch (MessageDataException ex) {
            string valueExMessage = ex.Message;
#if DEBUG
            aw.Write(buffer, 0, bufferCount);
            buffer = aw.ToArray();
            string ss = DataUtilities.GetUtf8String(
              buffer,
              Math.Max(buffer.Length - 35, 0),
              Math.Min(buffer.Length, 35),
              true);
            ss = String.Empty;
            string transferEnc = (leaf ?? this)
              .GetHeader("content-transfer-encoding");
            valueExMessage += " [" + ss + "] [type=" + ((leaf ??
                    this).ContentType ?? MediaType.TextPlainAscii) +
              "] [encoding=" + transferEnc + "]";
            valueExMessage = valueExMessage.Replace('\r', ' ')
              .Replace('\n', ' ').Replace('\0', ' ');
#endif
            throw new MessageDataException(valueExMessage);
          }
          if (ch < 0) {
            if (boundaryChecker.HasNewBodyPart) {
              Message msg = NewBodyPart();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack
#if DEBUG
              if (multipartStack.Count < stackCount) {
                throw new ArgumentException("multipartStack.Count (" +
                    multipartStack.Count + ") is less than " + stackCount);
              }
#endif
              if (leaf != null) {
                if (bufferCount > 0) {
                  aw.Write(buffer, 0, bufferCount);
                }
                leaf.body = aw.ToArray();
                // Clear for the next body
                aw.Clear();
                bufferCount = 0;
              } else {
                // Clear for the next body
                bufferCount = 0;
                aw.Clear();
              }
              while (multipartStack.Count > stackCount) {
                multipartStack.RemoveAt(stackCount);
              }
              Message parentMessage = multipartStack[multipartStack.Count -
                    1].Message;
              boundaryChecker.StartBodyPartHeaders();
              MediaType ctype = parentMessage.ContentType;
              bool parentIsDigest = ctype.SubType.Equals("digest") &&
                ctype.IsMultipart;
              ReadHeaders(stream, msg.headers, false);
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.Parts.Add(msg);
              multipartStack.Add(entry);
              aw.Clear();
              ctype = msg.ContentType;
              leaf = ctype.IsMultipart ? null : msg;
              boundaryChecker.EndBodyPartHeaders(entry.Boundary);
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                ctype.TypeAndSubType.Equals("text/plain"));
            } else {
              // All body parts were read
              if (leaf != null) {
                if (bufferCount > 0) {
                  aw.Write(buffer, 0, bufferCount);
                  bufferCount = 0;
                }
                leaf.body = aw.ToArray();
              }
              return;
            }
          } else {
            buffer[bufferCount++] = (byte)ch;
            if (bufferCount >= bufferLength) {
              aw.Write(buffer, 0, bufferCount);
              bufferCount = 0;
            }
          }
        }
      }
    }

    private void ReadSimpleBody(IByteReader stream) {
      IByteReader transform = MakeTransferEncoding(
        stream,
        this.transferEncoding,
        this.ContentType.TypeAndSubType.Equals("text/plain"));
      var buffer = new byte[8192];
      var bufferCount = 0;
      int bufferLength = buffer.Length;
      var aw = new ArrayWriter();
      {
        while (true) {
          var ch = 0;
          try {
            ch = transform.ReadByte();
          } catch (MessageDataException ex) {
            string valueExMessage = ex.Message;
#if DEBUG
            aw.Write(buffer, 0, bufferCount);
            buffer = aw.ToArray();
            string ss = DataUtilities.GetUtf8String(
              buffer,
              Math.Max(buffer.Length - 35, 0),
              Math.Min(buffer.Length, 35),
              true);
            ss = String.Empty;
            string transferEnc = this.GetHeader("content-transfer-encoding");
            valueExMessage += " [" + ss + "] [type=" + (this.ContentType ??
                MediaType.TextPlainAscii) + "] [encoding=" + transferEnc +
                    "]";
            valueExMessage = valueExMessage.Replace('\r', ' ')
              .Replace('\n', ' ').Replace('\0', ' ');
#endif
            throw new MessageDataException(valueExMessage, ex);
          }
          if (ch < 0) {
            break;
          }
          buffer[bufferCount++] = (byte)ch;
          if (bufferCount >= bufferLength) {
            aw.Write(buffer, 0, bufferCount);
            bufferCount = 0;
          }
        }
        if (bufferCount > 0) {
          aw.Write(buffer, 0, bufferCount);
        }
        this.body = aw.ToArray();
      }
    }

    private string SynthesizeField(string name) {
      var encoder = new HeaderEncoder(76, 0);
      encoder.AppendSymbol(name + ":");
      encoder.AppendSpace();
      string fullField = Implode(this.GetMultipleHeaders(name), ", ");
      string lcname = DataUtilities.ToLowerCaseAscii(name);
      if (fullField.Length == 0) {
        encoder.AppendSymbol("me@" + name + "-address.invalid");
      } else {
        encoder.AppendAsEncodedWords(fullField);
        encoder.AppendSpace();
        encoder.AppendSymbol("<me@" + name + "-address.invalid>");
      }
      return encoder.ToString();
    }

    private class MessageStackEntry {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.MessageStackEntry.Message"]/*'/>
      public Message Message { get; private set; }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Message.MessageStackEntry.Boundary"]/*'/>
      public string Boundary { get; private set; }

      public MessageStackEntry(Message msg) {
#if DEBUG
        if (msg == null) {
          throw new ArgumentNullException(nameof(msg));
        }
#endif
        string newBoundary = String.Empty;
        MediaType mediaType = msg.ContentType;
        if (mediaType.IsMultipart) {
          newBoundary = mediaType.GetParameter("boundary");
          if (newBoundary == null) {
            throw new
              MessageDataException("Multipart message has no boundary defined");
          }
          if (!IsWellFormedBoundary(newBoundary)) {
            throw new
  MessageDataException("Multipart message has an invalid boundary defined: " +
                newBoundary);
          }
        }
        this.Message = msg;
        this.Boundary = newBoundary;
      }
    }
  }
}
