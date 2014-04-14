/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail
{
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

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A Message object.</returns>
    public Message SetTextBody(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.body = DataUtilities.GetUtf8Bytes(str, true);
      this.contentType = MediaType.Parse("text/plain; charset=utf-8");
      this.SetHeader("content-transfer-encoding", "quoted-printable");
      return this;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A Message object.</returns>
    public Message SetHtmlBody(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      this.body = DataUtilities.GetUtf8Bytes(str, true);
      this.contentType = MediaType.Parse("text/html; charset=utf-8");
      this.SetHeader("content-transfer-encoding", "quoted-printable");
      return this;
    }

    /// <summary>Not documented yet.</summary>
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
      Message textMessage = new Message().SetTextBody(text);
      Message htmlMessage = new Message().SetTextBody(html);
      this.contentType = MediaType.Parse("multipart/alternative; boundary=\"=_boundary\"");
      this.SetHeader("content-transfer-encoding", "7bit");
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

    private static IList<NamedAddress> ParseAddresses(string value) {
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
    }

    /// <summary>Returns the mail message contained in this message's body.</summary>
    /// <returns>A message object if this object's content type is "message/rfc822",
    /// or null otherwise.</returns>
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
          if (!HeaderFields.GetParser(name).IsStructured()) {
            // Decode encoded words where they appear in unstructured
            // header fields
            // TODO: Also decode encoded words in structured header
            // fields (at least in phrases and maybe also comments)
            value = Rfc2047.DecodeEncodedWords(value, 0, value.Length, EncodedWordContext.Unstructured);
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
          value = ParserUtility.ToLowerCaseAscii(transferEncodingValue);
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
        if (this.contentType.TopLevelType.Equals("multipart") ||
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
      name = ParserUtility.ToLowerCaseAscii(name);
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
      int indexStart = index;
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
      name = ParserUtility.ToLowerCaseAscii(name);
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

    private static int CharLength(string str, int index) {
      if (str == null || index < 0 || index >= str.Length) {
        return 1;
      }
      int c = str[index];
      if (c >= 0xd800 && c <= 0xdbff && index + 1 < str.Length &&
          str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
        return 2;
      }
      return 1;
    }

    public static string ConvertCommentsToEncodedWords(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return ConvertCommentsToEncodedWords(str, 0, str.Length);
    }

    public static string ConvertCommentsToEncodedWords(string str, int index, int length) {
      // NOTE: Assumes that the comment is syntactically valid
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (str.Length - index < length) {
        throw new ArgumentException("str's length minus " + index + " (" + Convert.ToString((long)(str.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      int endIndex = index + length;
      if (length < 2 || str[index] != '(' || str[endIndex - 1] != ')') {
        return str.Substring(index, length);
      }
      EncodedWordEncoder encoder;
      int nextComment = str.IndexOf('(', index + 1);
      int nextBackslash = str.IndexOf('\\', index + 1);
      // don't count comments or backslashes beyond
      // the desired portion
      if (nextComment >= endIndex) {
        nextComment = -1;
      }
      if (nextBackslash >= endIndex) {
        nextBackslash = -1;
      }
      bool haveEscape = nextBackslash >= 0;
      if (!haveEscape) {
        // Check for possible folding whitespace
        nextBackslash = str.IndexOf('\n', index + 1);
        if (nextBackslash >= endIndex) {
          nextBackslash = -1;
        }
        haveEscape = nextBackslash >= 0;
      }
      if (nextComment < 0 && nextBackslash < 0) {
        // No escapes or nested comments, so it's relatively easy
        if (length == 2) {
          return "()";
        }
        encoder = new EncodedWordEncoder();
        encoder.AddPrefix("(");
        encoder.AddString(str, index + 1, length - 2);
        encoder.FinalizeEncoding(")");
        return encoder.ToString();
      }
      if (nextBackslash < 0) {
        // No escapes; just look for '(' and ')'
        encoder = new EncodedWordEncoder();
        while (true) {
          int parenStart = index;
          // Get the next run of parentheses
          while (index < endIndex) {
            if (str[index] == '(' || str[index] == ')') {
              ++index;
            } else {
              break;
            }
          }
          // Get the next run of non-parentheses
          int parenEnd = index;
          while (index < endIndex) {
            if (str[index] == '(' || str[index] == ')') {
              break;
            } else {
              ++index;
            }
          }
          if (parenEnd == index) {
            encoder.FinalizeEncoding(str.Substring(parenStart, parenEnd - parenStart));
            break;
          } else {
            encoder.AddPrefix(str.Substring(parenStart, parenEnd - parenStart));
            encoder.AddString(str, parenEnd, index - parenEnd);
          }
        }
        return encoder.ToString();
      }
      StringBuilder builder = new StringBuilder();
      // escapes, but no nested comments
      if (nextComment < 0) {
        ++index;  // skip the first parenthesis
        while (index < endIndex) {
          if (str[index] == ')') {
            // End of the comment
            break;
          } else if (str[index] == '\r' && index + 2 < endIndex &&
                     str[index + 1] == '\n' && (str[index + 2] == 0x20 || str[index + 2] == 0x09)) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1 < endIndex) {
            // Quoted pair
            int charLen = CharLength(str, index + 1);
            builder.Append(str.Substring(index + 1, charLen));
            index += 1 + charLen;
          } else {
            // Other comment text
            builder.Append(str[index]);
            ++index;
          }
        }
        if (builder.Length == 0) {
          return "()";
        }
        encoder = new EncodedWordEncoder();
        encoder.AddPrefix("(");
        encoder.AddString(builder.ToString());
        encoder.FinalizeEncoding(")");
        return encoder.ToString();
      }
      // escapes and nested comments
      encoder = new EncodedWordEncoder();
      while (true) {
        int parenStart = index;
        // Get the next run of parentheses
        while (index < endIndex) {
          if (str[index] == '(' || str[index] == ')') {
            ++index;
          } else {
            break;
          }
        }
        // Get the next run of non-parentheses
        int parenEnd = index;
        builder.Remove(0, builder.Length);
        while (index < endIndex) {
          if (str[index] == '(' || str[index] == ')') {
            break;
          } else if (str[index] == '\r' && index + 2 < endIndex &&
                     str[index + 1] == '\n' && (str[index + 2] == 0x20 || str[index + 2] == 0x09)) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1 < endIndex) {
            // Quoted pair
            int charLen = CharLength(str, index + 1);
            builder.Append(str.Substring(index + 1, charLen));
            index += 1 + charLen;
          } else {
            // Other comment text
            builder.Append(str[index]);
            ++index;
          }
        }
        if (builder.Length == 0) {
          encoder.FinalizeEncoding(str.Substring(parenStart, parenEnd - parenStart));
          break;
        } else {
          encoder.AddPrefix(str.Substring(parenStart, parenEnd - parenStart));
          encoder.AddString(builder.ToString());
        }
      }
      return encoder.ToString();
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
          String.Format("Differs [length {0} vs. {1}]\r\nA={2}\r\nB={3}", a.Length, b.Length, a.Substring(sa, salen), b.Substring(sa, sblen)));
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
      if (topLevel.Equals("text")) {
        int lengthCheck = Math.Min(this.body.Length, 4096);
        int highBytes = 0;
        int lineLength = 0;
        bool allTextBytes = isBodyPart ? false : true;
        for (int i = 0; i < lengthCheck; ++i) {
          if ((this.body[i] & 0x80) != 0) {
            ++highBytes;
            allTextBytes = false;
          } else if (this.body[i] == 0x00 || this.body[i] == 0x7f) {
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
              this.body[i] == '.' && this.body[i+1]=='\r' && this.body[i+2]=='\n') {
            // See RFC2049 sec. 3
            allTextBytes = false;
          }
          if (lineLength == 0 && i + 4 < this.body.Length &&
              this.body[i] == 'F' && this.body[i+1]=='r' && this.body[i+2]=='o' &&
              this.body[i + 3]=='m' && this.body[i+4]==' ') {
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
        } else {
          return EncodingQuotedPrintable;
        }
      }
      return EncodingBase64;
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
        throw new MessageDataException(ex.Message + " " + this.GetHeader("from"),ex);
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
      string newHeaders = this.GenerateHeaders();
      Message newMessage = new Message(new MemoryStream(DataUtilities.GetUtf8Bytes(newHeaders, true)));
      CheckDiff(listFrom, GenerateAddressList(newMessage.FromAddresses));
      CheckDiff(listTo, GenerateAddressList(newMessage.ToAddresses));
      CheckDiff(listCc, GenerateAddressList(newMessage.CCAddresses));
      return String.Empty;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    public string GenerateHeaders() {
      return this.GenerateHeaders(false);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    /// <param name='bodyPart'>A Boolean object.</param>
    public string GenerateHeaders(bool bodyPart) {
      StringBuilder sb = new StringBuilder();
      bool haveMimeVersion = false;
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
        } else if (name.Equals("content-type") ||
                   name.Equals("content-transfer-encoding")) {
          // These header fields will be written later
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
      if (!haveMimeVersion && bodyPart) {
        sb.Append("MIME-Version: 1.0\r\n");
      }
      MediaTypeBuilder builder = new MediaTypeBuilder(this.ContentType);
      int transferEncoding = this.TransferEncodingToUse(bodyPart);
      IStringEncoder bodyEncoder = null;
      switch (transferEncoding) {
        case EncodingBase64:
          sb.Append("Content-Transfer-Encoding: base64\r\n");
          bodyEncoder=new Base64Encoder(true, builder.TopLevelType.Equals("text") ? true : false);
          break;
        case EncodingQuotedPrintable:
          sb.Append("Content-Transfer-Encoding: quoted-printable\r\n");
          bodyEncoder = new QuotedPrintableEncoder(builder.TopLevelType.Equals("text") ? 2 : 0);
          break;
        default:
          sb.Append("Content-Transfer-Encoding: 7bit\r\n");
          bodyEncoder = new IdentityEncoder();
          break;
      }
      int index = 0;
      bool isMultipart = false;
      if (builder.TopLevelType.Equals("multipart")) {
        string boundary = "=_" + Convert.ToString((int)index, System.Globalization.CultureInfo.CurrentCulture);
        builder.SetParameter("boundary", boundary);
        isMultipart = true;
      }
      sb.Append("Content-Type: " + builder.ToMediaType().ToString() + "\r\n");
      sb.Append("\r\n");
      if (!isMultipart) {
        bodyEncoder.WriteToString(sb, this.body, 0, this.body.Length);
        bodyEncoder.FinalizeEncoding(sb);
      } else {
        // TODO: Implement multipart body encoding
      }
      return sb.ToString();
    }

    private static void ReadHeaders(
      ITransform stream,
      IList<string> headerList) {
      int lineCount = 0;
      StringBuilder sb = new StringBuilder();
      StreamWithUnget ungetStream = new StreamWithUnget(stream);
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
        if (mediaType.TopLevelType.Equals("multipart")) {
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
                  bufferCount = 0;
                }
                leaf.body = ms.ToArray();
              }
              while (multipartStack.Count > stackCount) {
                multipartStack.RemoveAt(stackCount);
              }
              Message parentMessage = multipartStack[multipartStack.Count - 1].Message;
              boundaryChecker.StartBodyPartHeaders();
              ReadHeaders(stream, msg.headers);
              bool parentIsDigest = parentMessage.ContentType.SubType.Equals("digest") &&
                parentMessage.ContentType.TopLevelType.Equals("multipart");
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.Parts.Add(msg);
              multipartStack.Add(entry);
              ms.SetLength(0);
              if (msg.ContentType.TopLevelType.Equals("multipart")) {
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
      if (this.contentType.TopLevelType.Equals("multipart")) {
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
