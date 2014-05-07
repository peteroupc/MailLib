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

using PeterO;

namespace PeterO.Mail {
    /// <summary>Represents an email message. <para><b>Thread safety:</b>
    /// This class is mutable; its properties can be changed. None of its methods
    /// are designed to be thread safe. Therefore, access to objects from
    /// this class must be synchronized if multiple threads can access them
    /// at the same time.</para>
    /// <para>The following lists known deviations from the mail specifications
    /// (Internet Message Format and MIME):</para>
    /// <list type=''> <item>The content-transfer-encoding "quoted-printable"
    /// is treated as 7bit instead if it occurs in a message or body part with
    /// content type "multipart/*" or "message/*" (other than "message/global",
    /// "message/global-headers", "message/global-disposition-notification",
    /// or "message/global-delivery-status").</item>
    /// <item>Non-UTF-8 bytes appearing in header field values are replaced
    /// with replacement characters. Moreover, UTF-8 is parsed everywhere
    /// in header field values, even in those parts of some structured header
    /// fields where this appears not to be allowed.</item>
    /// <item>The To and Cc header fields are allowed to contain only comments
    /// and whitespace, but these "empty" header fields will be omitted when
    /// generating.</item>
    /// <item>There is no line length limit imposed when parsing quoted-printable
    /// or base64 encoded bodies.</item>
    /// <item>In non-MIME message bodies, in text/plain message bodies,
    /// and in the prologue and epilogue of multipart messages (which will
    /// be ignored), if the transfer encoding is absent or declared as 7bit,
    /// any 8-bit bytes are replaced with question marks.</item>
    /// <item>The name <code>ascii</code>
    /// is treated as a synonym for <code>us-ascii</code>
    /// , despite being a reserved name under RFC 2046.</item>
    /// <item>If a sequence of encoded words (RFC 2047) decodes to a string
    /// with a CTL character (U + 007F, or a character less than U + 0020 and not
    /// TAB) after being converted to Unicode, the encoded words are left
    /// un-decoded.</item>
    /// </list>
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

    /// <summary>Gets a snapshot of the header fields of this message. The
    /// list contains an alternating set of header field names and values,
    /// in the order they were declared in the message.</summary>
    /// <value>A snapshot of the header fields of this message. The list contains
    /// an alternating set of header field names and values, in the order they
    /// were declared in the message.</value>
    public IList<string> Headers {
      get {
        return new List<string>(this.headers);
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

    /// <summary>Sets the body of this message to the specified plain text
    /// string. The character sequences CR, LF, and CR/LF will be converted
    /// to CR/LF line breaks. Unpaired surrogate code points will be replaced
    /// with replacement characters.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>This instance.</returns>
    public Message SetTextBody(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.body = GetUtf8Bytes(str);
      this.contentType = IsShortAndAllAscii(str) ? MediaType.TextPlainAscii : MediaType.TextPlainUtf8;
      return this;
    }

    /// <summary>Sets the body of this message to the specified string in
    /// HTML format. The character sequences CR, LF, and CR/LF will be converted
    /// to CR/LF line breaks. Unpaired surrogate code points will be replaced
    /// with replacement characters.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>This instance.</returns>
    public Message SetHtmlBody(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.body = GetUtf8Bytes(str);
      this.contentType = IsShortAndAllAscii(str) ? MediaType.TextPlainAscii : MediaType.TextPlainUtf8;
      return this;
    }

    /// <summary>Sets the body of this message to a multipart body with plain
    /// text and HTML versions of the same message. The character sequences
    /// CR, LF, and CR/LF will be converted to CR/LF line breaks. Unpaired
    /// surrogate code points will be replaced with replacement characters.</summary>
    /// <param name='text'>A string object.</param>
    /// <param name='html'>A string object. (2).</param>
    /// <returns>This instance.</returns>
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
      IList<Message> parts = this.Parts;
      parts.Clear();
      parts.Add(textMessage);
      parts.Add(htmlMessage);
      return this;
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> FromAddresses {
      get {
        return ParseAddresses(this.GetHeader("from"));
      }
    }

    private bool IsValidAddressingField(string name) {
      name = DataUtilities.ToLowerCaseAscii(name);
      bool have = false;
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          if (have) {
            return false;
          }
          string headerValue = this.headers[i + 1];
          if (HeaderFields.GetParser(name).Parse(headerValue, 0, headerValue.Length, null) !=
              headerValue.Length) {
            return false;
          }
          have = true;
        }
      }
      return true;
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

    internal static IList<NamedAddress> ParseAddresses(string[] values) {
      Tokener tokener = new Tokener();
      var list = new List<NamedAddress>();
      foreach (string addressValue in values) {
        if (addressValue == null) {
          continue;
        }
        if (HeaderParser.ParseHeaderTo(addressValue, 0, addressValue.Length, tokener) != addressValue.Length) {
          // Invalid syntax
          continue;
        }
        list.AddRange(HeaderParserUtility.ParseAddressList(addressValue, 0, addressValue.Length, tokener.GetTokens()));
      }
      return list;
    }

    /// <summary>Gets a list of addresses found in the To header field or fields.</summary>
    /// <value>A list of addresses found in the To header field or fields.</value>
    public IList<NamedAddress> ToAddresses {
      get {
        return ParseAddresses(this.GetMultipleHeaders("to"));
      }
    }

    /// <summary>Gets a list of addresses found in the CC header field or fields.</summary>
    /// <value>A list of addresses found in the CC header field or fields.</value>
    public IList<NamedAddress> CCAddresses {
      get {
        return ParseAddresses(this.GetMultipleHeaders("cc"));
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> BccAddresses {
      get {
        return ParseAddresses(this.GetMultipleHeaders("bcc"));
      }
    }

    /// <summary>Gets or sets this message's subject.</summary>
    /// <value>This message&apos;s subject.</value>
    public string Subject {
      get {
        return this.GetHeader("subject");
      }

      set {
        this.SetHeader("subject", value);
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

    /// <summary>Returns the mail message contained in this message's body.</summary>
    /// <returns>A message object if this object's content type is "message/rfc822"
    /// or "message/global", or null otherwise.</returns>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1024",
      Justification="This method may throw MessageDataException among other things - making it too heavyweight to be a property.")]
    #endif
    public Message GetBodyMessage() {
      if (this.ContentType.TopLevelType.Equals("message") &&
          (this.ContentType.SubType.Equals("rfc822") || this.ContentType.SubType.Equals("global"))) {
        using (MemoryStream ms = new MemoryStream(this.body)) {
          return new Message(ms);
        }
      }
      return null;
    }

    private MediaType contentType;
    private ContentDisposition contentDisposition;
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
            IList<Message> parts = this.Parts;
            parts.Clear();
          }
          this.SetHeader("content-type", this.contentType.ToString());
        }
      }
    }

    /// <summary>Gets or sets this message's content disposition.</summary>
    /// <value>This message&apos;s content disposition, or null if none
    /// is specified.</value>
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
          this.SetHeader("content-disposition", this.contentDisposition.ToString());
        }
      }
    }

    private void ProcessHeaders(bool assumeMime, bool digest) {
      bool haveContentType = false;
      bool mime = assumeMime;
      bool haveContentDisp = false;
      string transferEncodingValue = String.Empty;
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("to") && !ParserUtility.IsNullEmptyOrSpaceTabOnly(value)) {
          if (HeaderParser.ParseHeaderTo(value, 0, value.Length, null) != value.Length) {
            throw new MessageDataException("Invalid To header: " + value);
          }
        }
        if (name.Equals("cc") && !ParserUtility.IsNullEmptyOrSpaceTabOnly(value)) {
          if (HeaderParser.ParseHeaderTo(value, 0, value.Length, null) != value.Length) {
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
          // NOTE: Actually "token", but all known transfer encoding values
          // fit the same syntax as the stricter one for top-level types and subtypes
          int endIndex = MediaType.skipMimeTypeSubtype(value, startIndex, value.Length, null);
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
          // Decode encoded words in the header field where possible
          value = parser.DecodeEncodedWords(value);
          this.headers[i + 1] = value;
        }
      }
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
        } else if (mime && name.Equals("content-type")) {
          if (haveContentType) {
            string valueExMessage = "Already have this header: " + name;
            /*
            valueExMessage+="[old="+this.contentType+", new="+value+"]";
            valueExMessage = valueExMessage.Replace("\r\n"," ");
             */
            throw new MessageDataException(valueExMessage);
          }
          this.contentType = MediaType.Parse(
            value,
            digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii);
          haveContentType = true;
        } else if (mime && name.Equals("content-disposition")) {
          if (haveContentDisp) {
            string valueExMessage = "Already have this header: " + name;
            /*
            valueExMessage+="[old="+this.contentType+", new="+value+"]";
            valueExMessage = valueExMessage.Replace("\r\n"," ");
             */
            throw new MessageDataException(valueExMessage);
          }
          this.contentDisposition = ContentDisposition.Parse(value);
          haveContentDisp = true;
        }
      }
      if (this.transferEncoding == EncodingUnknown) {
        this.contentType = MediaType.Parse("application/octet-stream");
      }
      if (this.transferEncoding == EncodingQuotedPrintable ||
          this.transferEncoding == EncodingBase64 ||
          this.transferEncoding == EncodingUnknown) {
        if (this.contentType.IsMultipart ||
            (this.contentType.TopLevelType.Equals("message") &&
             !this.contentType.SubType.Equals("global") &&
             !this.contentType.SubType.Equals("global-headers") &&
             !this.contentType.SubType.Equals("global-disposition-notification") &&
             !this.contentType.SubType.Equals("global-delivery-status"))) {
          if (this.transferEncoding == EncodingQuotedPrintable) {
            // DEVIATION: Treat quoted-printable for multipart and message
            // as 7bit instead
            this.transferEncoding = EncodingSevenBit;
          } else {
            throw new MessageDataException("Invalid content encoding for multipart or message");
          }
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

    /// <summary>Gets the first instance of the header field with the specified
    /// name, comparing the field name in an ASCII case-insensitive manner.</summary>
    /// <param name='name'>A string object.</param>
    /// <returns>The value of the first header field with that name, or null
    /// if there is none.</returns>
    public string GetHeader(string name) {
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          // Get the first instance of the header field
          return this.headers[i + 1];
        }
      }
      return null;
    }

    private static string Implode(string[] strings, string delim) {
      if (strings.Length == 0) {
        return String.Empty;
      }
      if (strings.Length == 1) {
        return strings[0];
      }
      StringBuilder sb = new StringBuilder();
      bool first = true;
      foreach (string s in strings) {
        if (!first) {
          sb.Append(delim);
        }
        sb.Append(s);
      }
      return sb.ToString();
    }

    private string[] GetMultipleHeaders(string name) {
      var headers = new List<string>();
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          headers.Add(this.headers[i]);
        }
      }
      return (string[])headers.ToArray();
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
      string ret = builder.ToString();
      if (ret.Equals("Mime-Version")) {
        return "MIME-Version";
      }
      if (ret.Equals("Message-Id")) {
        return "Message-ID";
      }
      return ret;
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

    internal static bool HasTextToEscapeIgnoreEncodedWords(string s, int index, int endIndex) {
      int len = endIndex;
      int chunkLength = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = s[i];
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
          if (chunkLength > 997) {
            return true;
          }
        }
      }
      return false;
    }

    internal static int ParseUnstructuredText(string str, int index, int endIndex) {
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
    /// <param name='value'>The header field&apos;s value.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='System.ArgumentException'>The header field
    /// name is too long or contains an invalid character, or the header field's
    /// value is syntactically invalid.</exception>
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
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name)) {
          this.headers[i + 1] = value;
          return this;
        }
      }
      this.headers.Add(name);
      this.headers.Add(value);
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object.</param>
    /// <returns>A Message object.</returns>
    public Message RemoveHeader(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
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
      return this;
    }

    private static bool StartsWithWhitespace(string str) {
      return str.Length > 0 && (str[0] == ' ' || str[0] == 0x09 || str[0] == '\r');
    }

    private static int TransferEncodingToUse(byte[] body, bool isBodyPart) {
      if (body == null || body.Length == 0) {
        return EncodingSevenBit;
      }
      int lengthCheck = Math.Min(body.Length, 4096);
      int highBytes = 0;
      int ctlBytes = 0;
      int lineLength = 0;
      bool allTextBytes = isBodyPart ? false : true;
      for (int i = 0; i < lengthCheck; ++i) {
        if ((body[i] & 0x80) != 0) {
          ++highBytes;
          allTextBytes = false;
        } else if (body[i] == 0x00) {
          allTextBytes = false;
          ++ctlBytes;
        } else if (body[i] == 0x7f ||
                   (body[i] < 0x20 && body[i] != 0x0d &&
                    body[i] != 0x0a && body[i] != 0x09)) {
          allTextBytes = false;
          ++ctlBytes;
        } else if (body[i] == (byte)'\r') {
          if (i + 1 >= body.Length || body[i + 1] != (byte)'\n') {
            // bare CR
            allTextBytes = false;
          } else if (i > 0 && (body[i - 1] == (byte)' ' || body[i - 1] == (byte)'\t')) {
            // Space followed immediately by CRLF
            allTextBytes = false;
          } else {
            ++i;
            lineLength = 0;
            continue;
          }
        } else if (body[i] == (byte)'\n') {
          // bare LF
          allTextBytes = false;
        }
        if (lineLength == 0 && i + 2 < body.Length &&
            body[i] == '.' && body[i + 1] == '\r' && body[i + 2] == '\n') {
          // See RFC2049 sec. 3
          allTextBytes = false;
        }
        if (lineLength == 0 && i + 4 < body.Length &&
            body[i] == 'F' && body[i + 1] == 'r' && body[i + 2] == 'o' &&
            body[i + 3] == 'm' && body[i + 4] == ' ') {
          // See RFC2049 sec. 3
          allTextBytes = false;
        }
        ++lineLength;
        if (lineLength > 78) {
          allTextBytes = false;
        }
      }
      if (lengthCheck == body.Length && allTextBytes) {
        return EncodingSevenBit;
      } if (highBytes > (lengthCheck / 3)) {
        return EncodingBase64;
      } if (ctlBytes > 10) {
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

    internal static bool CanBeUnencoded(byte[] bytes, bool checkBoundaryDelimiter) {
      if (bytes == null || bytes.Length == 0) {
        return true;
      }
      int lineLength = 0;
      int index = 0;
      int endIndex = bytes.Length;
      bool headers = true;
      while (index < endIndex) {
        int c = ((int)bytes[index]) & 0xff;
        if (c >= 0x80) {
          // Console.WriteLine("Non-ASCII character (0x {0:X2})",(int)c);
          return false;
        }
        if (lineLength == 0 && checkBoundaryDelimiter && index + 4 < endIndex &&
            bytes[index] == '-' &&
            bytes[index + 1] == '-' &&
            bytes[index + 2] == '=' &&
            bytes[index + 3] == '_' &&
            bytes[index + 4] == 'B'){
          // Start of a reserved boundary delimiter
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
        } else if (c == '\r' || c == '\n') {
          // Console.WriteLine("Bare CR or bare LF");
          return false;
        }
        ++lineLength;
        if (lineLength > 78) {
          // Console.WriteLine("Line length exceeded (" + maxLineLength + " " + (str.Substring(index-78, 78)) + ")");
          return false;
        }
        ++index;
      }
      return true;
    }

    /// <summary>Generates this message's data in text form.<para>The
    /// generated message will always be 7-bit ASCII, and the transfer encoding
    /// will always be 7bit, quoted-printable, or base64 (the declared transfer
    /// encoding for this message will be ignored).</para>
    /// <para> If the From header field has an invalid syntax or has no addresses,
    /// or if the field is missing, this method will generate a synthetic From
    /// field with the contents of the previous From field or fields as the
    /// name, and the address <code>me@author-address.invalid</code>
    /// as the address (a <code>.invalid</code>
    /// address is a reserved address that can never belong to anyone). </para>
    /// </summary>
    /// <returns>The generated message.</returns>
    /// <exception cref="MessageDataException">The message can't be generated.</exception>
    public string Generate() {
      return this.Generate(0);
    }

    private string Generate(int depth) {
      StringBuilder sb = new StringBuilder();
      bool haveMimeVersion = false;
      bool haveContentEncoding = false;
      bool haveContentType = false;
      bool haveContentDisp = false;
      bool haveFrom = false;
      bool outputtedFrom = false;
      bool haveTo = false;
      byte[] bodyToWrite = this.body;
      bool haveCc = false;
      bool haveBcc = false;
      MediaTypeBuilder builder = new MediaTypeBuilder(this.ContentType);
      string contentDisp = (this.ContentDisposition == null) ? null :
        this.ContentDisposition.ToString();
      int transferEncoding = 0;
      bool isMultipart = false;
      string boundary = String.Empty;
      if (builder.IsMultipart) {
        boundary = "=_Boundary" +
          Convert.ToString((int)depth, System.Globalization.CultureInfo.CurrentCulture);
        builder.SetParameter("boundary", boundary);
        isMultipart = true;
      }
      if (!isMultipart) {
        if (builder.TopLevelType.Equals("message")) {
          if (builder.SubType.Equals("delivery-status") ||
              builder.SubType.Equals("global-delivery-status")) {
            bodyToWrite = DowngradeDeliveryStatus(bodyToWrite);
          }
          bool msgCanBeUnencoded=CanBeUnencoded(bodyToWrite, depth > 0);
          if (builder.SubType.Equals("rfc822") && !msgCanBeUnencoded) {
            builder.SetSubType("global");
          } else if (builder.SubType.Equals("disposition-notification") && !msgCanBeUnencoded) {
            builder.SetSubType("global-disposition-notification");
          } else if (builder.SubType.Equals("delivery-status") && !msgCanBeUnencoded) {
            builder.SetSubType("global-delivery-status");
          } else if (!msgCanBeUnencoded){
            throw new MessageDataException("Message body can't be encoded");
          }
        }
      }
      string topLevel = builder.TopLevelType;
      if (topLevel.Equals("message") || topLevel.Equals("multipart")) {
        if (topLevel.Equals("multipart") || (
          !builder.SubType.Equals("global") &&
          !builder.SubType.Equals("global-headers") &&
          !builder.SubType.Equals("global-disposition-notification") &&
          !builder.SubType.Equals("global-delivery-status"))) {
          transferEncoding = EncodingSevenBit;
        } else {
          transferEncoding = TransferEncodingToUse(bodyToWrite, depth > 0);
        }
      } else {
        transferEncoding = TransferEncodingToUse(bodyToWrite, depth > 0);
      }
      string encodingString = "7bit";
      if (transferEncoding == EncodingBase64) {
        encodingString = "base64";
      } else if (transferEncoding == EncodingQuotedPrintable) {
        encodingString = "quoted-printable";
      }
      // Write the header fields
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("mime-version")) {
          haveMimeVersion = true;
        }
        if (name.Equals("content-type")) {
          if (haveContentType) {
            // Already outputted, continue
            continue;
          }
          haveContentType = true;
          value = builder.ToString();
        } if (name.Equals("content-disposition")) {
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
        } else if (name.Equals("from")) {
          if (haveFrom) {
            // Already outputted, continue
            continue;
          }
          haveFrom = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.FromAddresses);
            if (value.Length == 0) {
              // No addresses, synthesize a From field
              string fullField = Implode(this.GetMultipleHeaders(name), ", ");
              value = new EncodedWordEncoder().AddString(fullField).FinalizeEncoding().ToString();
              if (value.Length > 0) {
                value += " <me@author-address.invalid>";
              } else {
                value = "me@author-address.invalid";
              }
            }
          }
          outputtedFrom = true;
        } else if (name.Equals("to")) {
          if (haveTo) {
            // Already outputted, continue
            continue;
          }
          haveTo = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.ToAddresses);
            if (value.Length == 0) {
              // No addresses
              continue;
            }
          }
        } else if (name.Equals("cc")) {
          if (haveCc) {
            // Already outputted, continue
            continue;
          }
          haveCc = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.CCAddresses);
            if (value.Length == 0) {
              // No addresses
              continue;
            }
          }
        } else if (name.Equals("bcc")) {
          if (haveBcc) {
            // Already outputted, continue
            continue;
          }
          haveBcc = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.BccAddresses);
            if (value.Length == 0) {
              // No addresses
              continue;
            }
          }
        }
        string rawField = Capitalize(name) + ":" +
          (StartsWithWhitespace(value) ? String.Empty : " ") + value;
        if (CanOutputRaw(rawField)) {
          sb.Append(rawField);
        } else if (HasTextToEscape(value)) {
          string downgraded = HeaderFields.GetParser(name).DowngradeFieldValue(value);
          if (HasTextToEscapeIgnoreEncodedWords(downgraded, 0, downgraded.Length)) {
            if (name.Equals("message-id") ||
                name.Equals("resent-message-id") ||
                name.Equals("in-reply-to") ||
                name.Equals("references") ||
                name.Equals("original-recipient") ||
                name.Equals("final-recipient")) {
              // Header field still contains non-ASCII characters, convert
              // to a downgraded field
              name = "downgraded-" + name;
              downgraded = Rfc2047.EncodeString(ParserUtility.TrimSpaceAndTab(value));
            } else {
              throw new MessageDataException("Header field still has non-Ascii: " + name + " " + value);
            }
          }
          // TODO: Don't collapse spaces if a DQUOTE appears
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
      if (!outputtedFrom && depth == 0) {
        // Output a synthetic From field if it doesn't
        // exist and this isn't a body part
        sb.Append("From: me@author-address.invalid\r\n");
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
        bodyEncoder.WriteToString(sb, bodyToWrite, 0, bodyToWrite.Length);
        bodyEncoder.FinalizeEncoding(sb);
      } else {
        foreach (Message part in this.Parts) {
          sb.Append("\r\n--" + boundary + "\r\n");
          sb.Append(part.Generate(depth + 1));
        }
        sb.Append("\r\n--" + boundary + "--");
      }
      return sb.ToString();
    }

    private static int ReadUtf8Char(TransformWithUnget stream, int[] bytesRead) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xbf;
      int read = 0;
      while (true) {
        int b = stream.ReadByte();
        ++read;
        if (b < 0) {
          if (bytesNeeded != 0) {
            stream.Unget();
            --read;
            bytesRead[0] = read;
            return 0xfffd;
          }
          return -1;
        } else if (bytesNeeded == 0) {
          if ((b & 0x7f) == b) {
            bytesRead[0] = read;
            return b;
          } else if (b >= 0xc2 && b <= 0xdf) {
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
            bytesRead[0] = read;
            return 0xfffd;
          }
          continue;
        } else if (b < lower || b > upper) {
          stream.Unget();
          return 0xfffd;
        } else {
          lower = 0x80;
          upper = 0xbf;
          ++bytesSeen;
          cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
          if (bytesSeen != bytesNeeded) {
            continue;
          }
          bytesRead[0] = read;
          return cp;
        }
      }
    }

    internal static string DowngradeRecipientHeaderValue(string headerValue) {
      return DowngradeRecipientHeaderValue(headerValue, null);
    }

    internal static string DowngradeRecipientHeaderValue(string headerValue, int[] status) {
      int index;
      if (HasTextToEscapeIgnoreEncodedWords(headerValue, 0, headerValue.Length)) {
        index = HeaderParser.ParseCFWS(headerValue, 0, headerValue.Length, null);
        int atomText = HeaderParser.ParsePhraseAtom(headerValue, index, headerValue.Length, null);
        int typeEnd = atomText;
        string origValue = headerValue;
        bool isUtf8 = typeEnd - index == 5 &&
          (headerValue[index] & ~0x20) == 'U' &&
          (headerValue[index + 1] & ~0x20) == 'T' &&
          (headerValue[index + 2] & ~0x20) == 'F' &&
          headerValue[index + 3] == '-' &&
          headerValue[index + 4] == '8';
        atomText = HeaderParser.ParseCFWS(headerValue, atomText, headerValue.Length, null);
        if (index < headerValue.Length && headerValue[atomText] == ';') {
          string typePart = headerValue.Substring(0, atomText + 1);
          // Downgrade the comments in the type part
          // NOTE: Final-recipient has the same syntax as original-recipient,
          // except for the header field name
          typePart = HeaderFields.GetParser("original-recipient").DowngradeFieldValue(typePart);
          if (isUtf8) {
            // Downgrade the non-ASCII characters in the address
            StringBuilder builder = new StringBuilder();
            string hex = "0123456789ABCDEF";
            for (int i = atomText + 1; i < headerValue.Length; ++i) {
              if (headerValue[i] < 0x80) {
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
                    builder.Append(hex[(cp >> j) & 15]);
                  }
                }
                builder.Append('}');
              }
            }
            headerValue = typePart + builder.ToString();
          } else {
            headerValue = typePart + headerValue.Substring(atomText + 1);
          }
        }
        if (HasTextToEscapeIgnoreEncodedWords(headerValue, 0, headerValue.Length)) {
          // Encapsulate the header field in encoded words
          if (status != null) {
            status[0] = 2;  // Encapsulated
          }
          return Rfc2047.EncodeString(ParserUtility.TrimSpaceAndTabLeft(origValue));
        }
        if (status != null) {
          status[0] = 1;  // Downgraded
        }
        return headerValue;
      } else {
        if (status != null) {
          status[0] = 0;  // Unchanged
        }
        return headerValue;
      }
    }

    // Parse the delivery status byte array to downgrade
    // the Original-Recipient and Final-Recipient header fields
    internal static byte[] DowngradeDeliveryStatus(byte[] bytes) {
      // int lineCount = 0;
      StringBuilder sb = new StringBuilder();
      int index = 0;
      int endIndex = bytes.Length;
      int lastIndex = -1;
      ArrayWriter writer = null;
      while (index < endIndex) {
        sb.Remove(0, sb.Length);
        bool first = true;
        int headerNameStart = index;
        int headerNameEnd = index;
        // lineCount = 0;
        bool endOfHeaders = false;
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
          } else if (c == 0x20 || c == 0x09) {
            first = false;
          }
          if (c != 0x20 && c != 0x09) {
            headerNameEnd = index;
          }
        }
        if (endOfHeaders) {
          break;
        }
        int headerValueStart = index;
        int headerValueEnd = index;
        string origFieldName =
          DataUtilities.GetUtf8String(bytes, headerNameStart, headerValueStart - headerNameStart, true);
        string fieldName = DataUtilities.ToLowerCaseAscii(
          DataUtilities.GetUtf8String(bytes, headerNameStart, headerNameEnd - headerNameStart, true));
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
              bool fwsFirst = true;
              bool haveFWS = false;
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
                      // lineCount = 0;
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
                  // ++lineCount;
                  haveFWS = true;
                } else {
                  --index;
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
            } else {
              --index;
              // ++lineCount;
            }
          }
          // ++lineCount;
        }
        if (origRecipient || finalRecipient) {
          string headerValue = DataUtilities.GetUtf8String(
            bytes,
            headerValueStart,
            headerValueEnd - headerValueStart,
            true);
          int[] status = new int[1];
          headerValue = DowngradeRecipientHeaderValue(headerValue, status);
          if (status[0] == 2 || status[0] == 1) {
            // Downgraded or encapsulated
            if (writer == null) {
              writer = new ArrayWriter();
              writer.WriteBytes(bytes, 0, headerNameStart);
            } else {
              writer.WriteBytes(bytes, lastIndex, headerNameStart - lastIndex);
            }
            WordWrapEncoder encoder = null;
            if (status[0] == 2) {
              encoder = new WordWrapEncoder((origRecipient ?
                                             "Downgraded-Original-Recipient" :
                                             "Downgraded-Final-Recipient") + ":");
            } else {
              encoder = new WordWrapEncoder(origFieldName);
            }
            encoder.AddString(headerValue);
            byte[] newBytes = DataUtilities.GetUtf8Bytes(encoder.ToString(), true);
            writer.WriteBytes(newBytes, 0, newBytes.Length);
            lastIndex = headerValueEnd;
          }
        }
      }
      if (writer != null) {
        writer.WriteBytes(bytes, lastIndex, bytes.Length - lastIndex);
        bytes = writer.ToArray();
      }
      return bytes;
    }

    private static void ReadHeaders(
      ITransform stream,
      IList<string> headerList) {
      int lineCount = 0;
      int[] bytesRead = new int[1];
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
          if ((c >= 0x21 && c <= 57) || (c >= 59 && c <= 0x7e)) {
            if (wsp) {
              throw new MessageDataException("Whitespace within header field name");
            }
            first = false;
            if (c >= 'A' && c <= 'Z') {
              c += 0x20;
            }
            sb.Append((char)c);
          } else if (!first && c == ':') {
            if (lineCount > 998) {
              // 998 characters includes the colon
              throw new MessageDataException("Header field name too long");
            }
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
        // Read the header field value using UTF-8 characters
        // rather than bytes (DEVIATION: RFC 6532 allows UTF-8
        // in header field values, but not everywhere in these values,
        // as is done here for convenience)
        while (true) {
          int c = ReadUtf8Char(ungetStream, bytesRead);
          if (c == -1) {
            throw new MessageDataException("Premature end before all headers were read");
          }
          if (c == '\r') {
            // We're only looking for the single-byte LF, so
            // there's no need to use ReadUtf8Char
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
                // Use ReadByte here since we're just looking for the single
                // byte characters CR and LF
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
                // Use ReadByte here since we're just looking for the single
                // byte characters space and tab
                int c2 = ungetStream.ReadByte();
                if (c2 == 0x20 || c2 == 0x09) {
                  ++lineCount;
                  sb.Append((char)c2);
                  haveFWS = true;
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
              // This ends the header field
              break;
            } else {
              sb.Append('\r');
              ungetStream.Unget();
              ++lineCount;
            }
          }
          lineCount += bytesRead[0];
          // NOTE: Header field line limit not enforced here, only
          // in the header field name; it's impossible to generate
          // a conforming message if the name is too long
          // NOTE: Some emails still have 8-bit bytes in an unencoded subject line
          // or other unstructured header field; however, since RFC6532,
          // we can just assume the UTF-8 encoding in these cases; in
          // case the bytes are not valid UTF-8, a replacement character
          // will be output
          if (c <= 0xffff) {
            sb.Append((char)c);
          } else if (c <= 0x10ffff) {
            sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
            sb.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
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
            throw new MessageDataException("Multipart message has an invalid boundary defined: " + this.boundary);
          }
        }
      }
    }

    private void ReadMultipartBody(ITransform stream) {
      int baseTransferEncoding = this.transferEncoding;
      BoundaryCheckerTransform boundaryChecker = new BoundaryCheckerTransform(stream);
      // Be liberal on the prologue and epilogue of multipart
      // messages, as they will be ignored.
      ITransform currentTransform = MakeTransferEncoding(
        boundaryChecker,
        baseTransferEncoding,
        true);
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
          } catch (MessageDataException ex) {
            string valueExMessage = ex.Message;
            /*
            ms.Write(buffer, 0, bufferCount);
            buffer = ms.ToArray();
            string ss = DataUtilities.GetUtf8String(
              buffer,
              Math.Max(buffer.Length - 35, 0),
              Math.Min(buffer.Length, 35),
              true);
            string transferEnc = (leaf ?? this).GetHeader("content-transfer-encoding");
            valueExMessage += " ["+ss+"] [type="+((leaf ?? this).ContentType ?? MediaType.TextPlainAscii)+
              "] [encoding=" + transferEnc+"]";
            valueExMessage = valueExMessage.Replace('\r',' ').Replace('\n',' ').Replace('\0',' ');
             */
            throw new MessageDataException(valueExMessage);
          }
          if (ch < 0) {
            if (boundaryChecker.HasNewBodyPart) {
              Message msg = new Message();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack
              #if DEBUG
              if (multipartStack.Count < stackCount) {
                throw new ArgumentException("multipartStack.Count (" + Convert.ToString((int)multipartStack.Count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((int)stackCount, System.Globalization.CultureInfo.InvariantCulture));
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
              MediaType ctype = parentMessage.ContentType;
              bool parentIsDigest = ctype.SubType.Equals("digest") &&
                ctype.IsMultipart;
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.Parts.Add(msg);
              multipartStack.Add(entry);
              ms.SetLength(0);
              ctype = msg.ContentType;
              if (ctype.IsMultipart) {
                leaf = null;
              } else {
                leaf = msg;
              }
              boundaryChecker.PushBoundary(entry.Boundary);
              boundaryChecker.EndBodyPartHeaders();
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                ctype.TypeAndSubType.Equals("text/plain"));
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
      bool useLiberalSevenBit) {
      ITransform transform = new EightBitTransform(stream);
      if (encoding == EncodingQuotedPrintable) {
        // NOTE: The max line size is actually 76, but some emails
        // have lines that exceed this size, so use an unlimited line length
        // when parsing
        transform = new QuotedPrintableTransform(stream, false, -1);
      } else if (encoding == EncodingBase64) {
        // NOTE: Same as quoted-printable regarding line length
        transform = new Base64Transform(stream, false, -1);
      } else if (encoding == EncodingEightBit) {
        transform = new EightBitTransform(stream);
      } else if (encoding == EncodingBinary) {
        transform = stream;
      } else if (encoding == EncodingSevenBit) {
        if (useLiberalSevenBit) {
          // DEVIATION: Replace 8-bit bytes and null with the
          // question mark character for text/plain messages,
          // non-MIME messages, and the prologue and epilogue of multipart
          // messages (which will be ignored).
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
          } catch (MessageDataException ex) {
            string valueExMessage = ex.Message;
            /*
            ms.Write(buffer, 0, bufferCount);
            buffer = ms.ToArray();
            string ss = DataUtilities.GetUtf8String(
              buffer,
              Math.Max(buffer.Length - 35, 0),
              Math.Min(buffer.Length, 35),
              true);
            string transferEnc=this.GetHeader("content-transfer-encoding");
            valueExMessage+=" ["+ss+"] [type="+(this.ContentType ?? MediaType.TextPlainAscii)+
              "] [encoding=" + transferEnc+"]";
            valueExMessage = valueExMessage.Replace('\r',' ').Replace('\n',' ').Replace('\0',' ');
             */
            throw new MessageDataException(valueExMessage, ex);
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
        this.ReadSimpleBody(stream);
      }
    }
  }
}
