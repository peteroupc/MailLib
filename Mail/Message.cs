/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail {
    /// <summary>Represents an email message. <para><b>Thread safety:</b>
    /// This class is mutable; its properties can be changed. None of its methods
    /// are designed to be thread safe. Therefore, access to objects from
    /// this class must be synchronized if multiple threads can access them
    /// at the same time.</para>
    /// </summary>
  public sealed class Message {
    private const int EncodingSevenBit = 0;
    private const int EncodingUnknown = -1;
    private const int EncodingEightBit = 3;
    private const int EncodingBinary = 4;
    private const int EncodingQuotedPrintable = 1;
    private const int EncodingBase64 = 2;

    private IList<string> headers;

    private IList<Message> parts;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<Message> Parts {
      get {
        return this.parts;
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<string> Headers {
      get {
        return this.headers;
      }
    }

    private byte[] body;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    /// <returns>A byte array.</returns>
    public byte[] GetBody() {
      return this.body;
    }

    private static byte[] GetUtf8Bytes(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      try {
        using (MemoryStream ms = new MemoryStream()) {
          if (DataUtilities.WriteUtf8(str, 0, str.Length, ms, true, true) != 0) {
            throw new ArgumentException("Unpaired surrogate code point");
          }
          return ms.ToArray();
        }
      } catch (IOException ex) {
        throw new ArgumentException("I/O error occurred", ex);
      }
    }

    /// <summary>Sets the body of this message to the specified plain text
    /// string. The character sequences CR, LF, and CR/LF will be converted
    /// to CR/LF line breaks.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>This instance.</returns>
    public Message SetTextBody(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.body = GetUtf8Bytes(str);
      this.contentType = MediaType.Parse("text/plain; charset=utf-8");
      return this;
    }

    /// <summary>Sets the body of this message to the specified string in
    /// HTML format. The character sequences CR, LF, and CR/LF will be converted
    /// to CR/LF line breaks.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>This instance.</returns>
    public Message SetHtmlBody(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.body = GetUtf8Bytes(str);
      this.contentType = MediaType.Parse("text/html; charset=utf-8");
      return this;
    }

    /// <summary>Sets the body of this message to a multipart body with plain
    /// text and HTML versions of the same message. The character sequences
    /// CR, LF, and CR/LF will be converted to CR/LF line breaks.</summary>
    /// <param name='text'>A string object.</param>
    /// <param name='html'>A string object. (2).</param>
    /// <returns>A Message object.</returns>
    public Message SetTextAndHtml(string text, string html) {
      if (text == null) {
        throw new ArgumentNullException("text");
      }
      if (html == null) {
        throw new ArgumentNullException("html");
      }
      // The spec for multipart/alternative (RFC 2046) says that
      // the fanciest version of the message should go last (in
      // this case, the HTML version)
      Message textMessage = new Message().SetTextBody(text);
      Message htmlMessage = new Message().SetHtmlBody(html);
      this.contentType = MediaType.Parse("multipart/alternative; boundary=\"=_boundary\"");
      this.Parts.Clear();
      this.Parts.Add(textMessage);
      this.Parts.Add(htmlMessage);
      return this;
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> FromAddresses {
      get {
        return ParseAddresses(this.GetHeader("from"));
      }
    }

    internal static IList<NamedAddress> ParseAddresses(string value) {
      Tokener tokener = new Tokener();
      if (value == null) {
        return new List<NamedAddress>();
      }
      if (HeaderParser.ParseHeaderTo(value, 0, value.Length, tokener) != value.Length) {
        // Invalid syntax
        return new List<NamedAddress>();
      }
      return HeaderParserUtility.ParseAddressList(value, 0, value.Length, tokener.GetTokens());
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> ToAddresses {
      get {
        return ParseAddresses(this.GetHeader("to"));
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> CCAddresses {
      get {
        return ParseAddresses(this.GetHeader("cc"));
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> BccAddresses {
      get {
        return ParseAddresses(this.GetHeader("bcc"));
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string Subject {
      get {
        return this.GetHeader("subject");
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string BodyString {
      get {
        using (MemoryStream ms = new MemoryStream(this.body)) {
          Charsets.ICharset charset = Charsets.GetCharset(this.ContentType.GetCharset());
          if (charset == null) {
            throw new NotSupportedException("Not in a supported character set.");
          }
          ITransform transform = new WrappedStream(ms);
          return charset.GetString(transform);
        }
      }
    }

    public Message(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.headers = new List<string>();
      this.parts = new List<Message>();
      this.ReadMessage(new WrappedStream(stream));
    }

    public Message() {
      this.headers = new List<string>();
      this.parts = new List<Message>();
      this.body = new byte[0];
    }

    public Message(string fromAddress) {
      this.headers = new List<string>();
      this.body = new byte[0];
      this.parts = new List<Message>();
      this.SetHeader("from", fromAddress);
    }

    /// <summary>Returns the mail message contained in this message's body.</summary>
    /// <returns>A message object if this object's content type is "message/rfc822",
    /// or null otherwise.</returns>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1024",
      Justification="This method may throw MessageDataException among other things - making it too heavyweight to be a property.")]
    #endif
    public Message GetBodyMessage() {
      if (this.ContentType.TopLevelType.Equals("message") &&
          this.ContentType.SubType.Equals("rfc822")) {
        using (MemoryStream ms = new MemoryStream(this.body)) {
          return new Message(ms);
        }
      }
      return null;
    }

    private MediaType contentType;
    private int transferEncoding;

    /// <summary>Gets or sets this message's media type.</summary>
    /// <value>This message&apos;s media type.</value>
    public MediaType ContentType {
      get {
        return this.contentType;
      }

      set {
        if (value == null) {
          throw new ArgumentNullException("value");
        }
        if (!this.ContentType.Equals(value)) {
          this.contentType = value;
          if (!value.IsMultipart) {
            this.Parts.Clear();
          }
          this.SetHeader("content-type", this.contentType.ToString());
        }
      }
    }

    private void ProcessHeaders(bool assumeMime, bool digest) {
      bool haveContentType = false;
      bool mime = assumeMime;
      string transferEncodingValue = String.Empty;
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("from")) {
          if (HeaderParser.ParseHeaderFrom(value, 0, value.Length, null) != value.Length) {
            // Console.WriteLine(this.GetHeader("date"));
            // throw new MessageDataException("Invalid From header: "+value);
          }
        }
        if (name.Equals("to") && !ParserUtility.IsNullEmptyOrSpaceTabOnly(value)) {
          if (HeaderParser.ParseHeaderTo(value, 0, value.Length, null) != value.Length) {
            throw new MessageDataException("Invalid To header: " + value);
          }
        }
        if (name.Equals("cc") && !ParserUtility.IsNullEmptyOrSpaceTabOnly(value)) {
          if (HeaderParser.ParseHeaderCc(value, 0, value.Length, null) != value.Length) {
            throw new MessageDataException("Invalid Cc header: " + value);
          }
        }
        if (name.Equals("bcc") && !ParserUtility.IsNullEmptyOrSpaceTabOnly(value)) {
          if (HeaderParser.ParseHeaderBcc(value, 0, value.Length, null) != value.Length) {
            throw new MessageDataException("Invalid Bcc header: " + value);
          }
        }
        if (name.Equals("content-transfer-encoding")) {
          int startIndex = HeaderParser.ParseCFWS(value, 0, value.Length, null);
          int endIndex = HeaderParser.ParseToken(value, startIndex, value.Length, null);
          if (HeaderParser.ParseCFWS(value, endIndex, value.Length, null) == value.Length) {
            transferEncodingValue = value.Substring(startIndex, endIndex - startIndex);
          } else {
            transferEncodingValue = String.Empty;
          }
        }
        if (name.Equals("mime-version")) {
          mime = true;
        }
        if (value.IndexOf("=?") >= 0) {
          IHeaderFieldParser parser = HeaderFields.GetParser(name);
          if (!parser.IsStructured()) {
            // Decode encoded words where they appear in unstructured
            // header fields
            // TODO: Also decode encoded words in structured header
            // fields (at least in phrases and maybe also comments)
            value = parser.DecodeEncodedWords(value);
            this.headers[i + 1] = value;
          }
        }
      }
      bool haveFrom = false;
      bool haveSubject = false;
      bool haveTo = false;
      this.contentType = digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii;
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
          this.headers.RemoveAt(i);
          this.headers.RemoveAt(i);
          i -= 2;
        } else if (mime && name.Equals("content-type")) {
          if (haveContentType) {
            throw new MessageDataException("Already have this header: " + name);
          }
          this.contentType = MediaType.Parse(
            value,
            digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii);
          haveContentType = true;
        } else if (name.Equals("from")) {
          if (haveFrom) {
            throw new MessageDataException("Already have this header: " + name);
          }
          haveFrom = true;
        } else if (name.Equals("to")) {
          if (haveTo) {
            throw new MessageDataException("Already have this header: " + name);
          }
          haveTo = true;
        } else if (name.Equals("subject")) {
          if (haveSubject && value != this.GetHeader("subject")) {
            // DEVIATION: Don't throw an error unless
            // the new subject is different from the existing one
            throw new MessageDataException("Already have this header: " + name);
          }
          haveSubject = true;
        }
      }
      if (this.transferEncoding == EncodingUnknown) {
        this.contentType = MediaType.Parse("application/octet-stream");
      }
      if (this.transferEncoding == EncodingQuotedPrintable ||
          this.transferEncoding == EncodingBase64 ||
          this.transferEncoding == EncodingUnknown) {
        if (this.contentType.IsMultipart ||
            this.contentType.TopLevelType.Equals("message")) {
          throw new MessageDataException("Invalid content encoding for multipart or message");
        }
      }
    }

    private static bool IsWellFormedBoundary(string str) {
      if (str == null || str.Length < 1 || str.Length > 70) {
        return false;
      }
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (i == str.Length - 1 && c == 0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
          (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
          c == 0x20 || c == 0x2c || "'()-./+_:=?".IndexOf(c) >= 0)) {
          return false;
        }
      }
      return true;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object. (2).</param>
    /// <returns>A string object.</returns>
    public string GetHeader(string name) {
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          return this.headers[i + 1];
        }
      }
      return null;
    }

    // Returns true only if:
    // * Text matches the production "unstructured"
    // in RFC 5322 without any obsolete syntax
    // * Each line is no more than 75 characters in length
    // * Text has only printable ASCII characters, CR,
    // LF, and/or TAB
    internal static bool CanOutputRaw(string s) {
      int len = s.Length;
      int chunkLength = 0;
      bool maybe = false;
      for (int i = 0; i < len; ++i) {
        char c = s[i];
        if (c == 0x0d) {
          if (i + 1 >= len || s[i + 1] != 0x0a) {
            // bare CR
            return false;
          } else if (i + 2 >= len || (s[i + 2] != 0x09 && s[i + 2] != 0x20)) {
            // CRLF not followed by whitespace
            return false;
          }
          chunkLength = 0;
          maybe = true;
          i += 2;
          continue;
        }
        if (c >= 0x7f || (c < 0x20 && c != 0x09 && c != 0x0d)) {
          // CTLs (except TAB, SPACE, and CR) and non-ASCII
          // characters
          return false;
        }
        ++chunkLength;
        if (chunkLength > 75) {
          return false;
        }
      }
      if (maybe) {
        return ParseUnstructuredText(s, 0, s.Length) == s.Length;
      }
      return true;
    }

    private static string Capitalize(string s) {
      StringBuilder builder = new StringBuilder();
      bool afterHyphen = true;
      for (int i = 0; i < s.Length; ++i) {
        if (afterHyphen && s[i] >= 'a' && s[i] <= 'z') {
          builder.Append((char)(s[i] - 0x20));
        } else {
          builder.Append(s[i]);
        }
        if (s[i] == '-') {
          afterHyphen = true;
        } else {
          afterHyphen = false;
        }
      }
      return builder.ToString();
    }

    /// <summary>Returns true if the string has: * non-ASCII characters
    /// * "=?" * CTLs other than tab, or * a word longer than 75 characters. Can
    /// return false even if the string has: * CRLF followed by a line with just
    /// whitespace.</summary>
    /// <param name='s'>A string object.</param>
    /// <returns>A Boolean object.</returns>
    internal static bool HasTextToEscape(string s) {
      return HasTextToEscape(s, 0, s.Length);
    }

    internal static bool HasTextToEscape(string s, int index, int endIndex) {
      int len = endIndex;
      int chunkLength = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = s[i];
        if (c == '=' && i + 1 < len && c == '?') {
          // "=?" (start of an encoded word)
          return true;
        }
        if (c == 0x0d) {
          if (i + 1 >= len || s[i + 1] != 0x0a) {
            // bare CR
            // Console.WriteLine("bare CR");
            return true;
          } else if (i + 2 >= len || (s[i + 2] != 0x09 && s[i + 2] != 0x20)) {
            // CRLF not followed by whitespace
            return true;
          }
          chunkLength = 0;
          ++i;
          continue;
        } else if (c == 0x0a) {
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
          if (chunkLength > 75) {
            return true;
          }
        }
      }
      return false;
    }

    private static int ParseUnstructuredText(string str, int index, int endIndex) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                do {
                  int indexTemp4;
                  indexTemp4 = index;
                  do {
                    int indexStart4 = index;
                    while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                      ++index;
                    }
                    if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
                      index += 2;
                    } else {
                      index = indexStart4; break;
                    }
                    indexTemp4 = index;
                    index = indexStart4;
                  } while (false);
                  if (indexTemp4 != index) {
                    index = indexTemp4;
                  } else { break;
                  }
                } while (false);
                if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                  ++index;
                  while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                    ++index;
                  }
                } else {
                  index = indexStart3; break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
                  indexTemp3 += 2; break;
                }
                if (index < endIndex && (str[index] >= 33 && str[index] <= 126)) {
                  ++indexTemp3; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    /// <summary>Sets the value of this message's header field. If a header
    /// field with the same name exists, its value is replaced.</summary>
    /// <param name='name'>The name of a header field, such as &quot;from&quot;
    /// or &quot;subject&quot;.</param>
    /// <param name='value'>A string object.</param>
    /// <returns>A Message object.</returns>
    public Message SetHeader(string name, string value) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      if (value == null) {
        throw new ArgumentNullException("value");
      }
      if (name.Length > 997) {
        throw new ArgumentException("Header field name too long");
      }
      for (int i = 0; i < name.Length; ++i) {
        if (name[i] <= 0x20 || name[i] == ':' || name[i] >= 0x7f) {
          throw new ArgumentException("Header field name contains an invalid character");
        }
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      // Check characters in structured header fields
      if (HeaderFields.GetParser(name).IsStructured()) {
        if (ParseUnstructuredText(value, 0, value.Length) != value.Length) {
          throw new ArgumentException("Header field value contains invalid text");
        }
      }
      // Add the header field
      for (int i = 0; i < this.headers.Count; ++i) {
        if (this.headers[i].Equals(name)) {
          this.headers[i + 1] = value;
          return this;
        }
      }
      this.headers.Add(name);
      this.headers.Add(value);
      return this;
    }

    private static bool StartsWithWhitespace(string str) {
      return str.Length > 0 && (str[0] == ' ' || str[0] == 0x09 || str[0] == '\r');
    }

    private static void CheckDiff(string a, string b) {
      if (!a.Equals(b)) {
        int pt = Math.Min(a.Length, b.Length);
        for (int i = 0; i < Math.Min(a.Length, b.Length); ++i) {
          if (a[i] != b[i]) {
            pt = i;
            break;
          }
        }
        int sa = Math.Max(pt - 50, 0);
        int salen = Math.Min(100, a.Length - sa);
        int sblen = Math.Min(100, b.Length - sa);
        throw new MessageDataException(
          "Differs [length " + a.Length + " vs. " + b.Length + "]\r\nA=" + a.Substring(sa, salen) + "\r\nB=" + b.Substring(sa, sblen));
      }
    }

    private int TransferEncodingToUse(bool isBodyPart) {
      string topLevel = this.contentType.TopLevelType;
      if (topLevel.Equals("message") || topLevel.Equals("multipart")) {
        return EncodingSevenBit;
      }
      if (this.body == null || this.body.Length == 0) {
        return EncodingSevenBit;
      }
      int lengthCheck = Math.Min(this.body.Length, 4096);
      int highBytes = 0;
      int zeroBytes = 0;
      int lineLength = 0;
      bool allTextBytes = isBodyPart ? false : true;
      for (int i = 0; i < lengthCheck; ++i) {
        if ((this.body[i] & 0x80) != 0) {
          ++highBytes;
          allTextBytes = false;
        } else if (this.body[i] == 0x00) {
          allTextBytes = false;
          ++zeroBytes;
        } else if (this.body[i] == 0x7f) {
          allTextBytes = false;
        } else if (this.body[i] == (byte)'\r') {
          if (i + 1 >= this.body.Length || this.body[i + 1] != (byte)'\n') {
            // bare CR
            allTextBytes = false;
          } else if (i > 0 && (this.body[i - 1] == (byte)' ' || this.body[i - 1] == (byte)'\t')) {
            // Space followed immediately by CRLF
            allTextBytes = false;
          } else {
            ++i;
            lineLength = 0;
            continue;
          }
        } else if (this.body[i] == (byte)'\n') {
          // bare LF
          allTextBytes = false;
        }
        if (lineLength == 0 && i + 2 < this.body.Length &&
            this.body[i] == '.' && this.body[i + 1] == '\r' && this.body[i + 2] == '\n') {
          // See RFC2049 sec. 3
          allTextBytes = false;
        }
        if (lineLength == 0 && i + 4 < this.body.Length &&
            this.body[i] == 'F' && this.body[i + 1] == 'r' && this.body[i + 2] == 'o' &&
            this.body[i + 3] == 'm' && this.body[i + 4] == ' ') {
          // See RFC2049 sec. 3
          allTextBytes = false;
        }
        ++lineLength;
        if (lineLength > 78) {
          allTextBytes = false;
        }
      }
      if (lengthCheck == this.body.Length && allTextBytes) {
        return EncodingSevenBit;
      } if (highBytes > (lengthCheck / 3)) {
        return EncodingBase64;
      } if (zeroBytes > (lengthCheck / 10)) {
        return EncodingBase64;
      } else {
        return EncodingQuotedPrintable;
      }
    }

    internal static string GenerateAddressList(IList<NamedAddress> list) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < list.Count; ++i) {
        if (i > 0) {
          sb.Append(", ");
        }
        sb.Append(list[i].ToString());
      }
      return sb.ToString();
    }

    internal string GenerateAbbreviatedHeaders() {
      string listFrom = null;
      try {
        listFrom = GenerateAddressList(this.FromAddresses);
      } catch (ArgumentException ex) {
        throw new MessageDataException(ex.Message + " " + this.GetHeader("from"), ex);
      }
      var listTo = GenerateAddressList(this.ToAddresses);
      var listCc = GenerateAddressList(this.CCAddresses);
      string oldFrom = this.GetHeader("from");
      string oldTo = this.GetHeader("to");
      string oldCC = this.GetHeader("cc");
      this.headers.Clear();
      if (oldFrom != null) {
        this.SetHeader("x-old-from", oldFrom);
      }
      if (oldTo != null) {
        this.SetHeader("x-old-to", oldTo);
      }
      if (oldCC != null) {
        this.SetHeader("x-old-cc", oldCC);
      }
      try {
        if (!String.IsNullOrEmpty(listFrom)) {
          this.SetHeader("from", listFrom);
        }
      } catch (ArgumentException ex) {
        throw new MessageDataException(ex.Message + "\r\n" + listFrom, ex);
      }
      try {
        if (!String.IsNullOrEmpty(listTo)) {
          this.SetHeader("to", listTo);
        }
      } catch (ArgumentException ex) {
        throw new MessageDataException(ex.Message + "\r\n" + listTo, ex);
      }
      try {
        if (!String.IsNullOrEmpty(listCc)) {
          this.SetHeader("cc", listCc);
        }
      } catch (ArgumentException ex) {
        throw new MessageDataException(ex.Message + "\r\n" + listCc, ex);
      }
      this.ContentType = MediaType.Parse("text/plain");
      string newHeaders = this.Generate();
      Message newMessage = new Message(new MemoryStream(GetUtf8Bytes(newHeaders)));
      CheckDiff(listFrom, GenerateAddressList(newMessage.FromAddresses));
      CheckDiff(listTo, GenerateAddressList(newMessage.ToAddresses));
      CheckDiff(listCc, GenerateAddressList(newMessage.CCAddresses));
      return String.Empty;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    public string Generate() {
      return this.Generate(0);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    /// <param name='depth'>A 32-bit signed integer.</param>
    private string Generate(int depth) {
      StringBuilder sb = new StringBuilder();
      bool haveMimeVersion = false;
      bool haveContentEncoding = false;
      bool haveContentType = false;
      MediaTypeBuilder builder = new MediaTypeBuilder(this.ContentType);
      int transferEncoding = this.TransferEncodingToUse(depth > 0);
      string encodingString = "7bit";
      if (transferEncoding == EncodingBase64) {
        encodingString = "base64";
      } else if (transferEncoding == EncodingQuotedPrintable) {
        encodingString = "quoted-printable";
      }
      bool isMultipart = false;
      string boundary = String.Empty;
      if (builder.IsMultipart) {
        boundary = "=_Boundary" +
          Convert.ToString((int)depth, System.Globalization.CultureInfo.CurrentCulture);
        builder.SetParameter("boundary", boundary);
        isMultipart = true;
      }
      // Write the header fields
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("mime-version")) {
          haveMimeVersion = true;
        }
        IHeaderFieldParser parser = HeaderFields.GetParser(name);
        if (!parser.IsStructured()) {
          // Outputting an unstructured header field
          string rawField = Capitalize(name) + ":" +
            (StartsWithWhitespace(value) ? String.Empty : " ") + value;
          if (CanOutputRaw(rawField)) {
            // TODO: Try to preserve header field name (before the colon)
            sb.Append(rawField);
            sb.Append("\r\n");
          } else {
            var encoder = new WordWrapEncoder(Capitalize(name) + ":");
            // If this header field contains text that must be
            // encoded (such as non-ASCII characters)
            if (HasTextToEscape(value)) {
              // Convert the entire header field value to encoded
              // words
              encoder.AddString(
                new EncodedWordEncoder()
                .AddString(value)
                .FinalizeEncoding()
                .ToString());
            } else {
              encoder.AddString(value);
            }
            sb.Append(encoder.ToString());
            sb.Append("\r\n");
          }
        } else if (name.Equals("content-transfer-encoding")) {
          sb.Append(Capitalize(name) + ":" + encodingString);
          haveContentEncoding = true;
        } else if (name.Equals("content-type")) {
          sb.Append(Capitalize(name) + ":" + builder.ToString());
          haveContentType = true;
        } else {
          // Outputting a structured header field
          string rawField = Capitalize(name) + ":" +
            (StartsWithWhitespace(value) ? String.Empty : " ") + value;
          if (CanOutputRaw(rawField)) {
            // TODO: Try to preserve header field name (before the colon)
            sb.Append(rawField);
          } else if (HasTextToEscape(value)) {
            string downgraded = HeaderFields.GetParser(name).DowngradeFieldValue(value);
            // TODO: If the header field is still not downgraded,
            // write a "Downgraded-" header field instead (applies to
            // Message-ID, Resent-Message-ID, In-Reply-To, References,
            // Original-Recipient, and Final-Recipient)
            var encoder = new WordWrapEncoder(Capitalize(name) + ":");
            encoder.AddString(downgraded);
            sb.Append(encoder.ToString());
          } else {
            var encoder = new WordWrapEncoder(Capitalize(name) + ":");
            encoder.AddString(value);
            sb.Append(encoder.ToString());
          }
          sb.Append("\r\n");
        }
      }
      if (!haveMimeVersion && depth == 0) {
        sb.Append("MIME-Version: 1.0\r\n");
      }
      if (!haveContentType) {
        sb.Append("Content-Type: " + builder.ToString() + "\r\n");
      }
      if (!haveContentEncoding) {
        sb.Append("Content-Transfer-Encoding: " + encodingString + "\r\n");
      }
      IStringEncoder bodyEncoder = null;
      switch (transferEncoding) {
        case EncodingBase64:
          bodyEncoder = new Base64Encoder(true, builder.IsText ? true : false, false);
          break;
        case EncodingQuotedPrintable:
          bodyEncoder = new QuotedPrintableEncoder(builder.IsText ? 2 : 0, false);
          break;
        default:
          bodyEncoder = new IdentityEncoder();
          break;
      }
      // Write the body
      sb.Append("\r\n");
      if (!isMultipart) {
        bodyEncoder.WriteToString(sb, this.body, 0, this.body.Length);
        bodyEncoder.FinalizeEncoding(sb);
      } else {
         foreach (var part in this.Parts) {
          sb.Append("\r\n--" + boundary + "\r\n");
          sb.Append(part.Generate(depth + 1));
        }
        sb.Append("\r\n--" + boundary + "--");
      }
      return sb.ToString();
    }

    private static void ReadHeaders(
      ITransform stream,
      IList<string> headerList) {
      int lineCount = 0;
      StringBuilder sb = new StringBuilder();
      TransformWithUnget ungetStream = new TransformWithUnget(stream);
      while (true) {
        sb.Remove(0, sb.Length);
        bool first = true;
        bool endOfHeaders = false;
        bool wsp = false;
        lineCount = 0;
        while (true) {
          int c = ungetStream.ReadByte();
          if (c == -1) {
            throw new MessageDataException("Premature end before all headers were read");
          }
          ++lineCount;
          if (first && c == '\r') {
            if (ungetStream.ReadByte() == '\n') {
              endOfHeaders = true;
              break;
            } else {
              throw new MessageDataException("CR not followed by LF");
            }
          }
          if (lineCount > 998) {
            throw new MessageDataException("Header field line too long");
          }
          if ((c >= 0x21 && c <= 57) || (c >= 59 && c <= 0x7e)) {
            if (wsp) {
              throw new MessageDataException("Whitespace within header field");
            }
            first = false;
            if (c >= 'A' && c <= 'Z') {
              c += 0x20;
            }
            sb.Append((char)c);
          } else if (!first && c == ':') {
            break;
          } else if (c == 0x20 || c == 0x09) {
            wsp = true;
            first = false;
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
        string fieldName = sb.ToString();
        sb.Remove(0, sb.Length);
        // Read the header field value
        while (true) {
          int c = ungetStream.ReadByte();
          if (c == -1) {
            throw new MessageDataException("Premature end before all headers were read");
          }
          if (c == '\r') {
            c = ungetStream.ReadByte();
            if (c == '\n') {
              lineCount = 0;
              // Parse obsolete folding whitespace (obs-fws) under RFC5322
              // (parsed according to errata), same as LWSP in RFC5234
              bool fwsFirst = true;
              bool haveFWS = false;
              while (true) {
                // Skip the CRLF pair, if any (except if iterating for
                // the first time, since CRLF was already parsed)
                if (!fwsFirst) {
                  c = ungetStream.ReadByte();
                  if (c == '\r') {
                    c = ungetStream.ReadByte();
                    if (c == '\n') {
                      // CRLF was read
                      sb.Append("\r\n");
                      lineCount = 0;
                    } else {
                      // It's the first part of the line, where the header name
                      // should be, so the CR here is illegal
                      throw new MessageDataException("CR not followed by LF");
                    }
                  } else {
                    // anything else, unget
                    ungetStream.Unget();
                  }
                }
                fwsFirst = false;
                int c2 = ungetStream.ReadByte();
                if (c2 == 0x20 || c2 == 0x09) {
                  ++lineCount;
                  sb.Append((char)c2);
                  haveFWS = true;
                  if (lineCount > 998) {
                    throw new MessageDataException("Header field line too long");
                  }
                } else {
                  ungetStream.Unget();
                  break;
                }
              }
              if (haveFWS) {
                // We have folding whitespace, line
                // count found as above
                continue;
              }
              break;
            } else {
              sb.Append('\r');
              ungetStream.Unget();
              ++lineCount;
            }
          }
          if (lineCount > 998) {
            throw new MessageDataException("Header field line too long");
          }
          if (c < 0x80) {
            sb.Append((char)c);
          } else {
            if (!HeaderFields.GetParser(fieldName).IsStructured()) {
              // DEVIATION: Some emails still have an unencoded subject line
              // or other unstructured header field
              sb.Append('\ufffd');
            } else {
              throw new MessageDataException("Malformed header field value " + sb.ToString().Substring(
                Math.Max(sb.Length - 100, 0)));
            }
          }
        }
        string fieldValue = sb.ToString();
        headerList.Add(fieldName);
        headerList.Add(fieldValue);
      }
    }

    private class MessageStackEntry {
      private Message message;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
      public Message Message {
        get {
          return this.message;
        }
      }

      private string boundary;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
      public string Boundary {
        get {
          return this.boundary;
        }
      }

      public MessageStackEntry(Message msg) {
        #if DEBUG
        if (msg == null) {
          throw new ArgumentNullException("msg");
        }
        #endif

        this.message = msg;
        MediaType mediaType = msg.ContentType;
        if (mediaType.IsMultipart) {
          this.boundary = mediaType.GetParameter("boundary");
          if (this.boundary == null) {
            throw new MessageDataException("Multipart message has no boundary defined");
          }
          if (!IsWellFormedBoundary(this.boundary)) {
            throw new MessageDataException("Multipart message has an invalid boundary defined");
          }
        }
      }
    }

    private void ReadMultipartBody(ITransform stream) {
      int baseTransferEncoding = this.transferEncoding;
      BoundaryCheckerTransform boundaryChecker = new BoundaryCheckerTransform(stream);
      ITransform currentTransform = MakeTransferEncoding(
        boundaryChecker,
        baseTransferEncoding,
        this.ContentType.TypeAndSubType.Equals("text/plain"));
      IList<MessageStackEntry> multipartStack = new List<MessageStackEntry>();
      MessageStackEntry entry = new Message.MessageStackEntry(this);
      multipartStack.Add(entry);
      boundaryChecker.PushBoundary(entry.Boundary);
      Message leaf = null;
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.Length;
      this.body = new byte[0];
      using (MemoryStream ms = new MemoryStream()) {
        while (true) {
          int ch = 0;
          try {
            ch = currentTransform.ReadByte();
          } catch (MessageDataException) {
            /*
            ms.Write(buffer, 0, bufferCount);
            buffer = ms.ToArray();
            string ss = DataUtilities.GetUtf8String(
              buffer,
              Math.Max(buffer.Length - 80, 0),
              Math.Min(buffer.Length, 80),
              true);
            Console.WriteLine(ss);
             */
            throw;
          }
          if (ch < 0) {
            if (boundaryChecker.HasNewBodyPart) {
              Message msg = new Message();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack
              #if DEBUG
              if (multipartStack.Count < stackCount) {
                throw new ArgumentException("multipartStack.Count (" + Convert.ToString((long)multipartStack.Count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)stackCount, System.Globalization.CultureInfo.InvariantCulture));
              }
              #endif
              if (leaf != null) {
                if (bufferCount > 0) {
                  ms.Write(buffer, 0, bufferCount);
                }
                leaf.body = ms.ToArray();
                // Clear for the next body
                ms.SetLength(0);
                bufferCount = 0;
              } else {
                // Clear for the next body
                bufferCount = 0;
                ms.SetLength(0);
              }
              while (multipartStack.Count > stackCount) {
                multipartStack.RemoveAt(stackCount);
              }
              Message parentMessage = multipartStack[multipartStack.Count - 1].Message;
              boundaryChecker.StartBodyPartHeaders();
              ReadHeaders(stream, msg.headers);
              bool parentIsDigest = parentMessage.ContentType.SubType.Equals("digest") &&
                parentMessage.ContentType.IsMultipart;
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.Parts.Add(msg);
              multipartStack.Add(entry);
              ms.SetLength(0);
              if (msg.ContentType.IsMultipart) {
                leaf = null;
              } else {
                leaf = msg;
              }
              boundaryChecker.PushBoundary(entry.Boundary);
              boundaryChecker.EndBodyPartHeaders();
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                msg.ContentType.TypeAndSubType.Equals("text/plain"));
            } else {
              // All body parts were read
              if (leaf != null) {
                if (bufferCount > 0) {
                  ms.Write(buffer, 0, bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.ToArray();
              }
              return;
            }
          } else {
            buffer[bufferCount++] = (byte)ch;
            if (bufferCount >= bufferLength) {
              ms.Write(buffer, 0, bufferCount);
              bufferCount = 0;
            }
          }
        }
      }
    }

    private static ITransform MakeTransferEncoding(
      ITransform stream,
      int encoding,
      bool plain) {
      ITransform transform = new EightBitTransform(stream);
      if (encoding == EncodingQuotedPrintable) {
        transform = new QuotedPrintableTransform(stream, false);
      } else if (encoding == EncodingBase64) {
        transform = new Base64Transform(stream, false);
      } else if (encoding == EncodingEightBit) {
        transform = new EightBitTransform(stream);
      } else if (encoding == EncodingBinary) {
        transform = new BinaryTransform(stream);
      } else if (encoding == EncodingSevenBit) {
        if (plain) {
          // DEVIATION: Replace 8-bit bytes and null with the
          // question mark character for text/plain and non-MIME
          // messages
          transform = new LiberalSevenBitTransform(stream);
        } else {
          transform = new SevenBitTransform(stream);
        }
      }
      return transform;
    }

    private void ReadSimpleBody(ITransform stream) {
      ITransform transform = MakeTransferEncoding(
        stream,
        this.transferEncoding,
        this.ContentType.TypeAndSubType.Equals("text/plain"));
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.Length;
      using (MemoryStream ms = new MemoryStream()) {
        while (true) {
          int ch = 0;
          try {
            ch = transform.ReadByte();
          } catch (MessageDataException) {
            /*
            ms.Write(buffer, 0, bufferCount);
            buffer = ms.ToArray();
            string ss = DataUtilities.GetUtf8String(
              buffer,
              Math.Max(buffer.Length - 80, 0),
              Math.Min(buffer.Length, 80),
              true);
            Console.WriteLine(ss);
             */
            throw;
          }
          if (ch < 0) {
            break;
          }
          buffer[bufferCount++] = (byte)ch;
          if (bufferCount >= bufferLength) {
            ms.Write(buffer, 0, bufferCount);
            bufferCount = 0;
          }
        }
        if (bufferCount > 0) {
          ms.Write(buffer, 0, bufferCount);
        }
        this.body = ms.ToArray();
      }
    }

    private void ReadMessage(ITransform stream) {
      ReadHeaders(stream, this.headers);
      this.ProcessHeaders(false, false);
      if (this.contentType.IsMultipart) {
        this.ReadMultipartBody(stream);
      } else {
        if (this.contentType.TopLevelType.Equals("message")) {
          // Console.WriteLine(this.contentType);
        }
        this.ReadSimpleBody(stream);
      }
    }
  }
}
