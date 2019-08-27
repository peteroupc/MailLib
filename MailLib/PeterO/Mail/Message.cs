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
  /// <summary>Represents an email message, and contains methods and
  /// properties for accessing and modifying email message data. This
  /// class implements the Internet Message Format (RFC 5322) and
  /// Multipurpose Internet Mail Extensions (MIME; RFC 2045-2047, RFC
  /// 2049).</summary>
  /// <remarks>
  /// <para><b>Thread safety:</b> This class is mutable; its properties
  /// can be changed. None of its instance methods are designed to be
  /// thread safe. Therefore, access to objects from this class must be
  /// synchronized if multiple threads can access them at the same
  /// time.</para>
  /// <para>The following lists known deviations from the mail
  /// specifications (Internet Message Format and MIME):</para>
  /// <list type=''>
  /// <item>If a message has two or more Content-Type header fields, it
  /// is treated as having a content type of "application/octet-stream",
  /// unless one or more of the header fields is syntactically
  /// invalid.</item>
  /// <item>Illegal UTF-8 byte sequences appearing in header field values
  /// are replaced with replacement characters. Moreover, UTF-8 is parsed
  /// everywhere in header field values, even in those parts of some
  /// structured header fields where this appears not to be allowed.
  /// (UTF-8 is a character encoding for the Unicode character
  /// set.)</item>
  /// <item>This implementation can parse a message even if that message
  /// is without a From header field, without a Date header field, or
  /// without both.</item>
  /// <item>The To and Cc header fields are allowed to contain only
  /// comments and whitespace, but these "empty" header fields will be
  /// omitted when generating.</item>
  /// <item>There is no line length limit imposed when parsing header
  /// fields, except header field names.</item>
  /// <item>There is no line length limit imposed when parsing
  /// quoted-printable or base64 encoded bodies.</item>
  /// <item>If the transfer encoding is absent and the content type is
  /// "message/rfc822", bytes with values greater than 127 are still
  /// allowed, despite the default value of "7bit" for
  /// "Content-Transfer-Encoding".</item>
  /// <item>In the following cases, if the transfer encoding is absent,
  /// declared as 7bit, or treated as 7bit, bytes greater than 127 are
  /// still allowed:</item>
  /// <item>(a) The preamble and epilogue of multipart messages, which
  /// will be ignored.</item>
  /// <item>(b) If the charset is declared to be <c>utf-8</c>.</item>
  /// <item>(c) If the content type is "text/html" and the charset is
  /// declared to be <c>us-ascii</c>, "windows-1252", "windows-1251", or
  /// "iso-8859-*" (all single byte encodings).</item>
  /// <item>(d) In non-MIME message bodies and in text/plain message
  /// bodies. Any bytes greater than 127 are replaced with the substitute
  /// character byte (0x1a).</item>
  /// <item>If the message starts with the word "From" (and no other case
  /// variations of that word) followed by one or more space (U+0020) not
  /// followed by colon, that text and the rest of the text is skipped up
  /// to and including a line feed (U+000A). (See also RFC 4155, which
  /// describes the so-called "mbox" convention with "From" lines of this
  /// kind.)</item>
  /// <item>The name <c>ascii</c> is treated as a synonym for
  /// <c>us-ascii</c>, despite being a reserved name under RFC 2046. The
  /// names <c>cp1252</c> and <c>utf8</c> are treated as synonyms for
  /// <c>windows-1252</c> and <c>utf-8</c>, respectively, even though
  /// they are not IANA registered aliases.</item>
  /// <item>The following deviations involve encoded words under RFC
  /// 2047:</item>
  /// <item>(a) If a sequence of encoded words decodes to a string with a
  /// CTL character (U+007F, or a character less than U+0020 and not TAB)
  /// after being converted to Unicode, the encoded words are left
  /// un-decoded.</item>
  /// <item>(b) This implementation can decode encoded words regardless
  /// of the character length of the line in which they appear. This
  /// implementation can generate a header field line with one or more
  /// encoded words even if that line is more than 76 characters long.
  /// (This implementation follows the recommendation in RFC 5322 to
  /// limit header field lines to no more than 78 characters, where
  /// possible; see also RFC 6532.)</item></list>
  /// <para>It would be appreciated if users of this library contact the
  /// author if they find other ways in which this implementation
  /// deviates from the mail specifications or other applicable
  /// specifications.</para>
  /// <para>This class currently doesn't support the "padding" parameter
  /// for message bodies with the media type "application/octet-stream"
  /// or treated as that media type (see RFC 2046 sec. 4.5.1).</para>
  /// <para>In this implementation, if the content-transfer-encoding
  /// "quoted-printable" or "base64" occurs in a message or body part
  /// with content type "multipart/*" or "message/*" (other than
  /// "message/global", "message/global-headers",
  /// "message/global-disposition-notification", or
  /// "message/global-delivery-status"), that encoding is treated as
  /// unrecognized for the purpose of treating that message or body part
  /// as having a content type of "application/octet-stream" rather than
  /// the declared content type. This is a clarification to RFCs 2045 and
  /// 2049. (This may result in "misdecoded" messages because in
  /// practice, most if not all messages of this kind don't use
  /// quoted-printable or base64 encodings for the whole body, but may do
  /// so in the body parts they contain.)</para>
  /// <para>This implementation can decode an RFC 2047 encoded word that
  /// uses ISO-2022-JP or ISO-2022-JP-2 (encodings that use code
  /// switching) even if the encoded word's payload ends in a different
  /// mode from "ASCII mode". (Each encoded word still starts in "ASCII
  /// mode", though.) This, however, is not a deviation to RFC 2047
  /// because the relevant rule only concerns bringing the output device
  /// back to "ASCII mode" after the decoded text is displayed (see last
  /// paragraph of sec. 6.2) -- since the decoded text is converted to
  /// Unicode rather than kept as ISO-2022-JP or ISO-2022-JP-2, this is
  /// not applicable since there is no such thing as "ASCII mode" in the
  /// Unicode Standard.</para>
  /// <para>Note that this library (the MailLib library) has no
  /// facilities for sending and receiving email messages, since that's
  /// outside this library's scope.</para></remarks>
  public sealed class Message {
    // Recomm. max. number of CHARACTERS per line (excluding CRLF)
    // (see RFC 5322, 6532)
    internal const int MaxRecHeaderLineLength = 78;
    internal const int MaxShortHeaderLineLength = 76;
    // Max. number of OCTETS per line (excluding CRLF)
    // (see RFC 5322, 6532)
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

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.Message'/> class. Reads from the given
    /// Stream object to initialize the email message.</summary>
    /// <param name='stream'>A readable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='stream'/> is null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>The message is
    /// malformed. See the remarks.</exception>
    /// <remarks><b>Remarks:</b>
    /// <para>This constructor parses an email message, and extracts its
    /// header fields and body, and throws a MessageDataException if the
    /// message is malformed. However, even if a MessageDataException is
    /// thrown, it can still be possible to display the message, especially
    /// because most email malformations seen in practice are benign in
    /// nature (such as the use of very long lines in the message). One way
    /// an application can handle the exception is to read all the bytes
    /// from the stream, to display the message, or part of it, as raw text
    /// (using <c>DataUtilities.GetUtf8String(bytes, true)</c> ), and to
    /// optionally extract important header fields, such as From, To, Date,
    /// and Subject, from the message's text using the <c>ExtractHeader</c>
    /// method. Even so, though, any message for which this constructor
    /// throws a MessageDataException ought to be treated with
    /// suspicion.</para></remarks>
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

    private static int SkipCaseString(byte[] bytes, int index, string value) {
      // NOTE: assumes value is all-ASCII
      int start = index;
      for (var i = 0; i < value.Length; ++i) {
        int b = ((int)bytes[index + i]) & 0xff;
        var c = (int)value[i];
        if (b >= 0x41 && b <= 0x5a) {
          b += 0x20;
        }
        if (c >= 0x41 && c <= 0x5a) {
          c += 0x20;
        }
        if (index + 1 >= bytes.Length || b != c) {
          return start;
        }
      }
      return index + value.Length;
    }

    private static int EndOfLine(byte[] bytes, int index) {
      return (index >= 2 && bytes[index - 1] == 0x0a && bytes[index - 2] ==
0x0d) ?
(index - 2) : index;
    }

    private static int SkipLine(byte[] bytes, int index) {
      while (index < bytes.Length) {
        if (bytes[index] == 0x0d && index + 1 < bytes.Length && bytes[index +
1] == 0x0a) {
          return index + 2;
        }
        ++index;
      }
      return index;
    }

    private static int SkipWsp(byte[] bytes, int index) {
      while (index < bytes.Length) {
        if (bytes[index] != 0x09 && bytes[index] != 0x20) {
          return index;
        }
        ++index;
      }
      return index;
    }

    /// <summary>Extracts the value of a header field from a byte array
    /// representing an email message. The return value is intended for
    /// display purposes, not for further processing, and this method is
    /// intended to be used as an error handling tool for email messages
    /// that are slightly malformed. (Note that malformed email messages
    /// ought to be treated with greater suspicion than well-formed email
    /// messages.).</summary>
    /// <param name='bytes'>A byte array representing an email
    /// message.</param>
    /// <param name='headerFieldName'>The name of the header field to
    /// extract. This name will be compared with the names of header fields
    /// in the given message using a basic case-insensitive comparison.
    /// (Two strings are equal in such a comparison, if they match after
    /// converting the basic upper-case letters A to Z (U+0041 to U+005A)
    /// in both strings to basic lower-case letters.).</param>
    /// <returns>The value of the first instance of the header field with
    /// the given name. Leading space and/or tab bytes (0x20 and/or 0x09)
    /// and CR/LF (0x0d/0x0a) pairs will be removed from the header field
    /// value, and the value is treated as encoded in UTF-8 (an 8-bit
    /// encoding form of the Unicode Standard) where illegally encoded
    /// UTF-8 is replaced as appropriate with replacement characters
    /// (U+FFFD). Returns null if <paramref name='bytes'/> is null, if
    /// <paramref name='headerFieldName'/> is null, is more than 997
    /// characters long, or has a character less than U+0021 or greater
    /// than U+007E in the Unicode Standard, if a header field with that
    /// name does not exist, or if a body (even an empty one) does not
    /// follow the header fields.</returns>
    public static string ExtractHeader(byte[] bytes, string headerFieldName) {
      if (bytes == null) {
        return null;
      }
      if (String.IsNullOrEmpty(headerFieldName) ||
         headerFieldName.Length > 997) {
        return null;
      }
      for (var i = 0; i < headerFieldName.Length; ++i) {
        if (headerFieldName[i] >= 0x7f || headerFieldName[i] <= 0x20 ||
                   headerFieldName[i] == ':') {
          break;
        }
      }
      var index = 0;
      string ret = null;
      while (index < bytes.Length) {
        if (index + 1 < bytes.Length && bytes[index] == 0x0d &&
           bytes[index + 1] == 0x0a) {
          // End of headers reached, so output the header field
          // found if any
          return ret;
        }
        if (ret != null) {
          // Already have a header field, so skip the line
          index = SkipLine(bytes, index);
          continue;
        }
        int n = SkipCaseString(bytes, index, headerFieldName);
        if (n == index) {
          // Not the desired header field
          index = SkipLine(bytes, index);
          continue;
        }
        n = SkipWsp(bytes, n);
        if (n >= bytes.Length || bytes[n] != ':') {
          // Not the desired header field
          index = SkipLine(bytes, index);
          continue;
        }
        n = SkipWsp(bytes, n + 1);
        using (var ms = new MemoryStream()) {
          int endLine = SkipLine(bytes, n);
          ms.Write(bytes, n, EndOfLine(bytes, endLine) - n);
          index = endLine;
          while (endLine < bytes.Length &&
               (bytes[endLine] == 0x09 || bytes[endLine] == 0x20)) {
            int s = endLine;
            endLine = SkipLine(bytes, endLine);
            index = endLine;
            ms.Write(bytes, s, EndOfLine(bytes, endLine) - s);
          }
          ret = DataUtilities.GetUtf8String(ms.ToArray(), true);
        }
      }
      return null;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.Message'/> class. Reads from the given byte
    /// array to initialize the email message.</summary>
    /// <param name='bytes'>A readable data stream.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>The message is
    /// malformed. See the remarks.</exception>
    /// <remarks><b>Remarks:</b>
    /// <para>This constructor parses an email message, and extracts its
    /// header fields and body, and throws a MessageDataException if the
    /// message is malformed. However, even if a MessageDataException is
    /// thrown, it can still be possible to display the message, especially
    /// because most email malformations seen in practice are benign in
    /// nature (such as the use of very long lines in the message). One way
    /// an application can handle the exception is to display the message,
    /// or part of it, as raw text (using
    /// <c>DataUtilities.GetUtf8String(bytes, true)</c> ), and to
    /// optionally extract important header fields, such as From, To, Date,
    /// and Subject, from the message's text using the <c>ExtractHeader</c>
    /// method. Even so, though, any message for which this constructor
    /// throws a MessageDataException ought to be treated with
    /// suspicion.</para></remarks>
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

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.Message'/> class. The message will be plain
    /// text and have an artificial From address.</summary>
    public Message() {
      this.headers = new List<string>();
      this.parts = new List<Message>();
      this.body = new byte[0];
      this.contentType = MediaType.TextPlainAscii;
      this.headers.Add("message-id");
      this.headers.Add(this.GenerateMessageID());
      this.headers.Add("from");
      this.headers.Add("me@from-address.invalid");
      this.headers.Add("mime-version");
      this.headers.Add("1.0");
    }

    /// <summary>Creates a message object with no header fields.</summary>
    /// <returns>A message object with no header fields.</returns>
    public static Message NewBodyPart() {
      var msg = new Message();
      msg.contentType = MediaType.TextPlainAscii;
      // No headers by default (see RFC 2046 sec. 5.1)
      msg.headers.Clear();
      return msg;
    }

    /// <summary>Sets this message's Date header field to the current time
    /// as its value, with an unspecified time zone offset.
    /// <para>This method can be used when the message is considered
    /// complete and ready to be generated, for example, using the
    /// "Generate()" method.</para></summary>
    /// <returns>This object.</returns>
    public Message SetCurrentDate() {
      // NOTE: Use global rather than local time; there are overriding
      // reasons not to use local time, despite the SHOULD in RFC 5322
      return this.SetDate(DateTimeUtilities.GetCurrentGlobalTime());
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

    /// <summary>Gets a list of addresses found in the BCC header field or
    /// fields.</summary>
    /// <value>A list of addresses found in the BCC header field or
    /// fields.</value>
    [Obsolete("Use GetAddresses(\"Bcc\") instead.")]
    public IList<NamedAddress> BccAddresses {
      get {
        return this.GetAddresses("bcc");
      }
    }

    /// <summary>Gets the body of this message as a text string. See the
    /// <c>GetBodyString()</c> method.</summary>
    /// <value>The body of this message as a text string.</value>
    /// <exception cref='NotSupportedException'>See the
    /// <c>GetBodyString()</c> method.</exception>
    [Obsolete("Use GetBodyString() instead.")]
    public string BodyString {
      get {
        return this.GetBodyString();
      }
    }

    private static ICharacterEncoding GetEncoding(string charset) {
        ICharacterEncoding enc = Encodings.GetEncoding(
          charset,
          true);
        if (enc == null) {
          if (charset.Equals("gb2312",
              StringComparison.Ordinal)) {
            // HACK
            enc = Encodings.GetEncoding("gb2312", false);
          } else {
            return null;
          }
        }
        return enc;
    }

private static bool DefinesCharsetParameter(MediaType mt) {
// All media types that specify a charset parameter, either as a
// required or an optional parameter.
// NOTE: Up-to-date as of August 26, 2019
if (mt.HasStructuredSuffix("xml") ||
 mt.TopLevelType.Equals("text", StringComparison.Ordinal) ||
 mt.TypeAndSubType.Equals("image/vnd.wap.wbmp", StringComparison.Ordinal)) {
  return true;
}
if (mt.TopLevelType.Equals("application", StringComparison.Ordinal)) {
return mt.SubType.Equals("vnd.uplanet.alert-wbxml", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.wap.wmlscriptc", StringComparison.Ordinal) ||
mt.SubType.Equals("xml-dtd", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.picsel", StringComparison.Ordinal) ||
mt.SubType.Equals("news-groupinfo", StringComparison.Ordinal) ||
mt.SubType.Equals("ecmascript", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.cacheop-wbxml", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.bearer-choice", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.wap.slc", StringComparison.Ordinal) ||
mt.SubType.Equals("nss", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.3gpp.mcdata-payload", StringComparison.Ordinal) ||
mt.SubType.Equals("activity+json", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.list-wbxml", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.3gpp.mcdata-signalling", StringComparison.Ordinal) ||
mt.SubType.Equals("sgml-open-catalog", StringComparison.Ordinal) ||
mt.SubType.Equals("smil", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.channel", StringComparison.Ordinal) ||
mt.SubType.Equals("javascript", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.syncml.dm+wbxml", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.ah-barcode", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.alert", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.wap.wbxml", StringComparison.Ordinal) ||
mt.SubType.Equals("xml-external-parsed-entity", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.listcmd-wbxml", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.list", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.listcmd", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.msign", StringComparison.Ordinal) ||
mt.SubType.Equals("news-checkgroups", StringComparison.Ordinal) ||
mt.SubType.Equals("fhir+json", StringComparison.Ordinal) ||
mt.SubType.Equals("set-registration", StringComparison.Ordinal) ||
mt.SubType.Equals("sql", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.wap.sic", StringComparison.Ordinal) ||
mt.SubType.Equals("prs.alvestrand.titrax-sheet", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.bearer-choice-wbxml",
  StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.wap.wmlc", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.channel-wbxml", StringComparison.Ordinal) ||
mt.SubType.Equals("iotp", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.uplanet.cacheop", StringComparison.Ordinal) ||
mt.SubType.Equals("xml", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.adobe.xfdf", StringComparison.Ordinal) ||
mt.SubType.Equals("vnd.dpgraph", StringComparison.Ordinal);
}
return false;
}

    private void GetBodyStrings(
       IList<string> bodyStrings,
       IList<MediaType> mediaTypes) {
        if (this.ContentDisposition != null &&
           !this.ContentDisposition.IsInline) {
         // Content-Disposition is present and other than inline; ignore.
         // See also RFC 2183 sec. 2.8 and 2.9.
         return;
        }
        MediaType mt = this.ContentType;
        if (mt.IsMultipart) {
          IList<Message> parts = this.Parts;
          if (mt.SubType.Equals("alternative",
            StringComparison.Ordinal)) {
            // Navigate the parts in reverse order
            for (var i = parts.Count - 1; i >= 0; --i) {
              int oldCount = bodyStrings.Count;
              parts[i].GetBodyStrings(bodyStrings, mediaTypes);
              if (oldCount != bodyStrings.Count) {
                break;
              }
            }
          } else {
            // Any other multipart
            for (var i = 0; i < parts.Count; ++i) {
              parts[i].GetBodyStrings(bodyStrings, mediaTypes);
            }
          }
        }
        if (!DefinesCharsetParameter(this.ContentType)) {
          // Nontext and no charset parameter defined
          return;
        }
        string charsetName = this.ContentType.GetCharset();
        ICharacterEncoding charset = GetEncoding(charsetName);
        if (charset == null &&
             this.ContentType.TypeAndSubType.Equals("text/html",
  StringComparison.Ordinal)) {
           charsetName = GuessHtmlEncoding(this.body);
           charset = Encodings.GetEncoding(charsetName);
        }
        if (charset != null) {
          bodyStrings.Add(Encodings.DecodeToString(
            charset,
            DataIO.ToReader(this.body)));
          mediaTypes.Add(this.ContentType);
        }
    }

    private string GetBodyStringNoThrow() {
        IList<string> bodyStrings = new List<string>();
        IList<MediaType> mediaTypes = new List<MediaType>();
        this.GetBodyStrings(bodyStrings, mediaTypes);
        if (bodyStrings.Count > 0) {
          return bodyStrings[0];
        } else {
           return null;
        }
    }

    private void AccumulateAttachments(
        IList<Message> attachments,
        bool root) {
       if (this.ContentDisposition != null &&
          !this.ContentDisposition.IsInline && !root) {
          attachments.Add(this);
          return;
       }
       MediaType mt = this.ContentType;
       if (mt.SubType.Equals("alternative", StringComparison.Ordinal)) {
          // Navigate the parts in reverse order
          for (var i = this.parts.Count - 1; i >= 0; --i) {
            if (this.GetBodyStringNoThrow() != null) {
              this.parts[i].AccumulateAttachments(attachments, false);
              break;
            }
          }
       } else {
          // Any other multipart
          for (var i = 0; i < this.parts.Count; ++i) {
            this.parts[i].AccumulateAttachments(attachments, false);
          }
       }
    }

    /// <summary>Gets a list of descendant body parts of this message that
    /// are considered attachments. An
    /// <i>attachment</i> is a body part or descendant body part that has a
    /// content disposition with a type other than inline. This message
    /// itself is not included in the list even if it's an attachment as
    /// just defined.</summary>
    /// <returns/>
    public IList<Message> GetAttachments() {
       var list = new List<Message>();
       this.AccumulateAttachments(list, true);
       return list;
    }

    /// <summary>Gets the body of this message as a text string. If this
    /// message's media type is "multipart/alternative", returns the result
    /// of this method for the last supported body part. For any other
    /// "multipart" media type, returns the result of this method for the
    /// first body part for which this method returns a text
    /// string.</summary>
    /// <returns>The body of this message as a text string.</returns>
    /// <exception cref='NotSupportedException'>This message is a multipart
    /// message without a supported body part; or this message has a
    /// content disposition with a type other than "inline"; or this
    /// message's media type is a non-multipart type and does not specify
    /// the use of a "charset" parameter, has no character encoding
    /// declared or assumed for it (which is usually the case for non-text
    /// messages), or has an unsupported character encoding.</exception>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1024",
    Justification="This method may throw NotSupportedException among other things - making it too heavyweight to be a property.")]
#endif
    public string GetBodyString() {
        // TODO: Consider returning null rather than throwing an exception
        // in public API
        string str = this.GetBodyStringNoThrow();
        if (str == null) {
          throw new NotSupportedException("No supported text to show");
        }
        return str;
    }

    /// <summary>Gets a list of addresses found in the CC header field or
    /// fields.</summary>
    /// <value>A list of addresses found in the CC header field or
    /// fields.</value>
    [Obsolete("Use GetAddresses(\"Cc\") instead.")]
    public IList<NamedAddress> CCAddresses {
      get {
        return this.GetAddresses("cc");
      }
    }

    internal static string ExtractCharsetFromMeta(string value) {
      if (value == null) {
        return value;
      }
      // We assume value is lower-case here
      var index = 0;
      int length = value.Length;
      var c = (char)0;
      while (true) {
        index = value.IndexOf("charset", 0, StringComparison.Ordinal);
        if (index < 0) {
          return null;
        }
        index += 7;
        // skip whitespace
        while (index < length) {
          c = value[index];
          if (c != 0x09 && c != 0x0c && c != 0x0d && c != 0x0a && c != 0x20) {
            break;
          }
          ++index;
        }
        if (index >= length) {
          return null;
        }
        if (value[index] == '=') {
          ++index;
          break;
        }
      }
      // skip whitespace
      while (index < length) {
        c = value[index];
        if (c != 0x09 && c != 0x0c && c != 0x0d && c != 0x0a && c != 0x20) {
          break;
        }
        ++index;
      }
      if (index >= length) {
        return null;
      }
      c = value[index];
      if (c == '"' || c == '\'') {
        ++index;
        int nextIndex = index;
        while (nextIndex < length) {
          char c2 = value[nextIndex];
          if (c == c2) {
            return Encodings.ResolveAlias(
  value.Substring(
    index,
    nextIndex - index));
          }
          ++nextIndex;
        }
        return null;
      } else {
        int nextIndex = index;
        while (nextIndex < length) {
          char c2 = value[nextIndex];
          if (
            c2 == 0x09 || c2 == 0x0c || c2 == 0x0d || c2 == 0x0a || c2 ==
0x20 ||
            c2 == 0x3b) {
            break;
          }
          ++nextIndex;
        }
        return
    Encodings.ResolveAlias(value.Substring(index, nextIndex - index));
      }
    }

    private static int ReadAttribute(
      byte[] data,
      int length,
      int position,
      StringBuilder attrName,
      StringBuilder attrValue) {
      if (attrName != null) {
        attrName.Remove(0, attrName.Length);
      }
      if (attrValue != null) {
        attrValue.Remove(0, attrValue.Length);
      }
      while (position < length && (data[position] == 0x09 ||
          data[position] == 0x0a || data[position] == 0x0c ||
          data[position] == 0x0d || data[position] == 0x20 ||
          data[position] == 0x2f)) {
        ++position;
      }
      if (position >= length || data[position] == 0x3f) {
        return position;
      }
      var empty = true;
      var tovalue = false;
      var b = 0;
      // Skip attribute name
      while (true) {
        if (position >= length) {
          // end of stream reached, so clear
          // the attribute name to indicate failure
          if (attrName != null) {
            attrName.Remove(0, attrName.Length);
          }
          return position;
        }
        b = data[position] & 0xff;
        if (b == 0x3d && !empty) {
          ++position;
          tovalue = true;
          break;
        } else if (b == 0x09 || b == 0x0a || b == 0x0c || b == 0x0d || b ==
            0x20) {
          break;
        } else if (b == 0x2f || b == 0x3e) {
          return position;
        } else {
          if (attrName != null) {
            if (b >= 0x41 && b <= 0x5a) {
              attrName.Append((char)(b + 0x20));
            } else {
              attrName.Append((char)b);
            }
          }
          empty = false;
          ++position;
        }
      }
      if (!tovalue) {
        while (position < length) {
          b = data[position] & 0xff;
          if (b != 0x09 && b != 0x0a && b != 0x0c && b != 0x0d && b != 0x20) {
            break;
          }
          ++position;
        }
        if (position >= length) {
          // end of stream reached, so clear
          // the attribute name to indicate failure
          if (attrName != null) {
            attrName.Remove(0, attrName.Length);
          }
          return position;
        }
        if ((data[position] & 0xff) != 0x3d) {
          return position;
        }
        ++position;
      }
      while (position < length) {
        b = data[position] & 0xff;
        if (b != 0x09 && b != 0x0a && b != 0x0c && b != 0x0d && b != 0x20) {
          break;
        }
        ++position;
      }
      // Skip value
      if (position >= length) {
        // end of stream reached, so clear
        // the attribute name to indicate failure
        if (attrName != null) {
          attrName.Remove(0, attrName.Length);
        }
        return position;
      }
      b = data[position] & 0xff;
      if (b == 0x22 || b == 0x27) { // have quoted _string
        ++position;
        while (true) {
          if (position >= length) {
            // end of stream reached, so clear
            // the attribute name and value to indicate failure
            if (attrName != null) {
              attrName.Remove(0, attrName.Length);
            }
            if (attrValue != null) {
              attrValue.Remove(0, attrValue.Length);
            }
            return position;
          }
          int b2 = data[position] & 0xff;
          if (b == b2) { // quote mark reached
            ++position;
            break;
          }
          if (attrValue != null) {
            if (b2 >= 0x41 && b2 <= 0x5a) {
              attrValue.Append((char)(b2 + 0x20));
            } else {
              attrValue.Append((char)b2);
            }
          }
          ++position;
        }
        return position;
      } else if (b == 0x3e) {
        return position;
      } else {
        if (attrValue != null) {
          if (b >= 0x41 && b <= 0x5a) {
            attrValue.Append((char)(b + 0x20));
          } else {
            attrValue.Append((char)b);
          }
        }
        ++position;
      }
      while (true) {
        if (position >= length) {
          // end of stream reached, so clear
          // the attribute name and value to indicate failure
          if (attrName != null) {
            attrName.Remove(0, attrName.Length);
          }
          if (attrValue != null) {
            attrValue.Remove(0, attrValue.Length);
          }
          return position;
        }
        b = data[position] & 0xff;
        if (b == 0x09 || b == 0x0a || b == 0x0c || b == 0x0d || b == 0x20 || b
          == 0x3e) {
          return position;
        }
        if (attrValue != null) {
          if (b >= 0x41 && b <= 0x5a) {
            attrValue.Append((char)(b + 0x20));
          } else {
            attrValue.Append((char)b);
          }
        }
        ++position;
      }
    }
    // NOTE: To be used when the encoding is not otherwise provided
    // by the transport layer (e.g., Content-Type) and the transport
    // layer's encoding is not overridden by the end user
    private static string GuessHtmlEncoding(byte[] data) {
      var b = 0;
      var count = Math.Min(data.Length, 1024);
      var position = 0;
      while (position < count) {
        if (position + 4 <= count && data[position + 0] == 0x3c &&
            (data[position + 1] & 0xff) == 0x21 &&
        (data[position + 2] & 0xff) == 0x2d &&
            (data[position + 3] & 0xff) == 0x2d) {
          // Skip comment
          var hyphenCount = 2;
          position += 4;
          while (position < count) {
            int c = data[position] & 0xff;
            if (c == '-') {
              hyphenCount = Math.Min(2, hyphenCount + 1);
            } else if (c == '>' && hyphenCount >= 2) {
              break;
            } else {
              hyphenCount = 0;
            }
            ++position;
          }
        } else if (position + 6 <= count && data[position] == 0x3c &&
   ((data[position + 1] & 0xff) == 0x4d || (data[position + 1] & 0xff) ==
            0x6d) &&
   ((data[position + 2] & 0xff) == 0x45 || (data[position + 2] & 0xff) ==
            0x65) &&
          ((data[position + 3] & 0xff) == 0x54 || (data[position + 3] & 0xff) ==
            0x74) && (data[position + 4] == 0x41 ||
                   data[position + 4] == 0x61) &&
   (data[position + 5] == 0x09 || data[position + 5] == 0x0a ||
         data[position + 5] == 0x0d ||
          data[position + 5] == 0x0c || data[position + 5] == 0x20 ||
                data[position + 5] == 0x2f)) {
          // META tag
          var haveHttpEquiv = false;
          var haveContent = false;
          var haveCharset = false;
          var gotPragma = false;
          var needPragma = 0; // need pragma null
          string charset = null;
          var attrName = new StringBuilder();
          var attrValue = new StringBuilder();
          position += 5;
          while (true) {
            int
    newpos = ReadAttribute(
      data,
      count,
      position,
      attrName,
      attrValue);
            if (newpos == position) {
              break;
            }
            string attrNameString = attrName.ToString();
            if (!haveHttpEquiv && attrNameString.Equals("http-equiv",
  StringComparison.Ordinal)) {
              haveHttpEquiv = true;
              if (attrValue.ToString().Equals("content-type",
  StringComparison.Ordinal)) {
                gotPragma = true;
              }
            } else if (!haveContent && attrNameString.Equals("content",
  StringComparison.Ordinal)) {
              haveContent = true;
              if (charset == null) {
                string newCharset =
  ExtractCharsetFromMeta(attrValue.ToString());
                if (newCharset != null) {
                  charset = newCharset;
                  needPragma = 2; // need pragma true
                }
              }
            } else if (!haveCharset && attrNameString.Equals("charset",
  StringComparison.Ordinal)) {
              haveCharset = true;
              charset = Encodings.ResolveAlias(attrValue.ToString());
              needPragma = 1; // need pragma false
            }
            position = newpos;
          }
          if (needPragma == 0 || (needPragma == 2 && !gotPragma) || charset ==
                 null) {
            ++position;
          } else {
            if ("utf-16le".Equals(charset, StringComparison.Ordinal) ||
"utf-16be".Equals(charset, StringComparison.Ordinal)) {
              charset = "utf-8";
            }
            return charset;
          }
        } else if ((position + 3 <= count &&
                data[position] == 0x3c && (data[position + 1] & 0xff) == 0x2f &&
  (((data[position + 2] & 0xff) >= 0x41 && (data[position + 2] & 0xff) <=
           0x5a) ||
         ((data[position + 2] & 0xff) >= 0x61 && (data[position + 2] & 0xff) <=
           0x7a))) ||
                    // </X
                    (position + 2 <= count && data[position] == 0x3c &&
        (((data[position + 1] & 0xff) >= 0x41 && (data[position + 1] & 0xff)
              <= 0x5a) ||
         ((data[position + 1] & 0xff) >= 0x61 && (data[position + 1] & 0xff)
               <= 0x7a)))) { // <X
          // </X
          while (position < count) {
            if (data[position] == 0x09 ||
                data[position] == 0x0a || data[position] == 0x0c ||
                data[position] == 0x0d || data[position] == 0x20 ||
                data[position] == 0x3e) {
              break;
            }
            ++position;
          }
          while (true) {
            int newpos = ReadAttribute(data, count, position, null, null);
            if (newpos == position) {
              break;
            }
            position = newpos;
          }
          ++position;
        } else if (position + 2 <= count && data[position] == 0x3c &&
    ((data[position + 1] & 0xff) == 0x21 || (data[position + 1] & 0xff) ==
           0x3f || (data[position + 1] & 0xff) == 0x2f)) {
          // <! or </ or <?
          while (position < count) {
            if (data[position] != 0x3e) {
              break;
            }
            ++position;
          }
          ++position;
        } else {
          ++position;
        }
      }
      var byteIndex = 0;
      int b1 = byteIndex >= data.Length ? -1 : ((int)data[byteIndex++]) &
0xff;
int b2 = byteIndex >= data.Length ? -1 : ((int)data[byteIndex++]) &
0xff;
if (b1 == 0xfe && b2 == 0xff) {
          return "utf-16be";
        }
        if (b1 == 0xff && b2 == 0xfe) {
          return "utf-16le";
        }
        int b3 = byteIndex >= data.Length ? -1 : ((int)data[byteIndex++]) &
0xff;
if (b1 == 0xef && b2 == 0xbb && b3 == 0xbf) {
          return "utf-8";
        }
      byteIndex = 0;
      var maybeUtf8 = 0;
      // Check for UTF-8
      position = 0;
      while (position < count) {
        b = data[position] & 0xff;
        if (b < 0x80) {
          ++position;
          continue;
        }
        if (position + 2 <= count && (b >= 0xc2 && b <= 0xdf) &&
         ((data[position + 1] & 0xff) >= 0x80 && (data[position + 1] & 0xff) <=
              0xbf)) {
          // DebugUtility.Log("%02X %02X",data[position],data[position+1]);
          position += 2;
          maybeUtf8 = 1;
        } else if (position + 3 <= count && (b >= 0xe0 && b <= 0xef) &&
      ((data[position + 2] & 0xff) >= 0x80 && (data[position + 2] & 0xff) <=
              0xbf)) {
          int startbyte = (b == 0xe0) ? 0xa0 : 0x80;
          int endbyte = (b == 0xed) ? 0x9f : 0xbf;
          // DebugUtility.Log("%02X %02X %02X"
          // , data[position], data[position + 1], data[position + 2]);
          if ((data[position + 1] & 0xff) < startbyte ||
              (data[position + 1] & 0xff) > endbyte) {
            maybeUtf8 = -1;
            break;
          }
          position += 3;
          maybeUtf8 = 1;
        } else if (position + 4 <= count && (b >= 0xf0 && b <= 0xf4) &&
   ((data[position + 2] & 0xff) >= 0x80 && (data[position + 2] & 0xff) <=
        0xbf) &&
      ((data[position + 3] & 0xff) >= 0x80 && (data[position + 3] & 0xff) <=
              0xbf)) {
          int startbyte = (b == 0xf0) ? 0x90 : 0x80;
          int endbyte = (b == 0xf4) ? 0x8f : 0xbf;
          // DebugUtility.Log("%02X %02X %02X %02X"
          // , data[position], data[position + 1], data[position + 2],
          // data[position + 3]);
          if ((data[position + 1] & 0xff) < startbyte ||
              (data[position + 1] & 0xff) > endbyte) {
            maybeUtf8 = -1;
            break;
          }
          position += 4;
          maybeUtf8 = 1;
        } else {
          if (position + 4 < count) {
            // we check for position here because the data may
            // end within a UTF-8 byte sequence
            maybeUtf8 = -1;
          }
          break;
        }
      }
      return maybeUtf8 == 1 ? "utf-8" : "windows-1252";
    }

    /// <summary>
    /// <para>Gets a Hypertext Markup Language (HTML) rendering of this
    /// message's text body. This method currently supports any message for
    /// which <c>GetBodyString()</c> outputs a text string and treats the
    /// following media types specially: text/plain with
    /// <c>format=flowed</c>, text/enriched, text/markdown (original
    /// Markdown).</para></summary>
    /// <returns>An HTML rendering of this message's text.</returns>
    /// <exception cref='NotSupportedException'>No supported body part was
    /// found; see <c>GetBodyString()</c> for more information.</exception>
    /// <remarks>
    /// <para>REMARK: The Markdown implementation currently supports all
    /// features of original Markdown, except that the
    /// implementation:</para>
    /// <list>
    /// <item>does not strictly check the placement of "block-level HTML
    /// elements",</item>
    /// <item>does not prevent Markdown content from being interpreted as
    /// such merely because it's contained in a "block-level HTML element",
    /// and</item>
    /// <item>does not deliberately use HTML escapes to obfuscate email
    /// addresses wrapped in angle-brackets.</item></list></remarks>
    public string GetFormattedBodyString() {
      // TODO: Consider returning null rather than throwing an exception
      // in public API
      string text = this.GetFormattedBodyStringNoThrow();
      if (text == null) {
        throw new NotSupportedException();
      }
      return text;
    }

    private string GetFormattedBodyStringNoThrow() {
      var bodyStrings = new List<string>();
      var mediaTypes = new List<MediaType>();
      this.GetBodyStrings(bodyStrings, mediaTypes);
      if (bodyStrings.Count == 0) {
        return null;
      }
      string text = bodyStrings[0];
      MediaType mt = mediaTypes[0];
      string fmt = mt.GetParameter("format");
      string dsp = mt.GetParameter("delsp");
      bool formatFlowed = DataUtilities.ToLowerCaseAscii(
      fmt == null ? "fixed" : fmt)
    .Equals("flowed", StringComparison.Ordinal);
      bool delSp = DataUtilities.ToLowerCaseAscii(
        dsp == null ? "no" : dsp).Equals("yes", StringComparison.Ordinal);
      if (mt.TypeAndSubType.Equals("text/plain", StringComparison.Ordinal)) {
        if (formatFlowed) {
          return FormatFlowed.FormatFlowedText(text, delSp);
        } else {
          return FormatFlowed.NonFormatFlowedText(text);
        }
      } else if (mt.TypeAndSubType.Equals("text/html",
          StringComparison.Ordinal)) {
        return text;
      } else if (mt.TypeAndSubType.Equals("text/markdown",
          StringComparison.Ordinal)) {
        MediaType previewType = MediaType.Parse("text/html");
        if (this.ContentDisposition != null) {
          string pt = this.ContentDisposition.GetParameter("preview-type");
          previewType = MediaType.Parse(
            pt == null ? String.Empty : pt,
            previewType);
        }
        if (previewType.TypeAndSubType.Equals("text/html",
          StringComparison.Ordinal)) {
          return FormatFlowed.MarkdownText(text, 0);
        } else {
          return FormatFlowed.NonFormatFlowedText(text);
        }
      } else if (mt.TypeAndSubType.Equals("text/enriched",
         StringComparison.Ordinal)) {
        return EnrichedText.EnrichedToHtml(text, 0, text.Length);
      } else {
        return FormatFlowed.NonFormatFlowedText(text);
      }
    }

    /// <summary>Gets or sets this message's content disposition. The
    /// content disposition specifies how a user agent should display or
    /// otherwise handle this message. Can be set to null. If set to a
    /// disposition or to null, updates the Content-Disposition header
    /// field as appropriate. (There is no default content disposition if
    /// this value is null, and disposition types unrecognized by the
    /// application should be treated as "attachment"; see RFC 2183 sec.
    /// 2.8.).</summary>
    /// <value>This message's content disposition, or null if none is
    /// specified.</value>
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

    /// <summary>Gets or sets this message's media type. When getting, the
    /// media type may differ in certain cases from the value of the
    /// Content-Type header field, if any, and may have a value even if the
    /// Content-Type header field is absent from this message. If set to a
    /// media type, updates the Content-Type header field as appropriate.
    /// Cannot be set to null.</summary>
    /// <value>This message's media type.</value>
    /// <exception cref='ArgumentNullException'>This value is being set and
    /// "value" is null.</exception>
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

    /// <summary>
    /// <para>Gets a file name suggested by this message for saving the
    /// message's body to a file. For more information on the algorithm,
    /// see ContentDisposition.MakeFilename.</para>
    /// <para>This method generates a file name based on the
    /// <c>filename</c> parameter of the Content-Disposition header field,
    /// if it exists, or on the <c>name</c> parameter of the Content-Type
    /// header field, otherwise.</para></summary>
    /// <value>A suggested name for the file. Returns the empty string if
    /// there is no filename suggested by the content type or content
    /// disposition, or if that filename is an empty string.</value>
    public string FileName {
      get {
        ContentDisposition disp = this.contentDisposition;
        return (disp != null) ?
          ContentDisposition.MakeFilename(disp.GetParameter("filename")) :
        ContentDisposition.MakeFilename(this.contentType.GetParameter(
  "name"));
      }
    }

    /// <summary>Gets a list of addresses contained in the header fields
    /// with the given name in this message.</summary>
    /// <param name='headerName'>The name of the header fields to
    /// retrieve.</param>
    /// <returns>A list of addresses, in the order in which they appear in
    /// this message's header fields of the given name.</returns>
    /// <exception cref='NotSupportedException'>The parameter <paramref
    /// name='headerName'/> is not supported for this method. Currently,
    /// the only header fields supported are To, Cc, Bcc, Reply-To, Sender,
    /// and From.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='headerName'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='headerName'/> is empty.</exception>
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
        return ParseAddresses(this.GetMultipleHeaders(headerName));
      } else {
        throw new NotSupportedException("Not supported for: " + headerName);
      }
    }

    /// <summary>Gets a list of addresses found in the From header field or
    /// fields.</summary>
    /// <value>A list of addresses found in the From header field or
    /// fields.</value>
    [Obsolete("Use GetAddresses(\"From\") instead.")]
    public IList<NamedAddress> FromAddresses {
      get {
        return this.GetAddresses("from");
      }
    }

    /// <summary>Gets a snapshot of the header fields of this message, in
    /// the order in which they appear in the message. For each item in the
    /// list, the key is the header field's name (where any basic
    /// upper-case letters, U+0041 to U+005A, are converted to basic
    /// lower-case letters) and the value is the header field's
    /// value.</summary>
    /// <value>A snapshot of the header fields of this message.</value>
    public IList<KeyValuePair<string, string>> HeaderFields {
      get {
        var list = new List<KeyValuePair<string, string>>();
        for (int i = 0; i < this.headers.Count; i += 2) {
          list.Add(
      new KeyValuePair<string, string>(this.headers[i], this.headers[i + 1]));
        }
        return list;
      }
    }

    /// <summary>Gets a list of all the parts of this message. This list is
    /// editable. This will only be used if the message is a multipart
    /// message.</summary>
    /// <value>A list of all the parts of this message. This list is
    /// editable. This will only be used if the message is a multipart
    /// message.</value>
    public IList<Message> Parts {
      get {
        return this.parts;
      }
    }

    /// <summary>Gets or sets this message's subject. The subject's value
    /// is found as though GetHeader("subject") were called.</summary>
    /// <value>This message's subject, or null if there is none.</value>
    public string Subject {
      get {
        return this.GetHeader("subject");
      }

      set {
        this.SetHeader("subject", value);
      }
    }

    /// <summary>Gets a list of addresses found in the To header field or
    /// fields.</summary>
    /// <value>A list of addresses found in the To header field or
    /// fields.</value>
    [Obsolete("Use GetAddresses(\"To\") instead.")]
    public IList<NamedAddress> ToAddresses {
      get {
        return this.GetAddresses("to");
      }
    }

    /// <summary>Adds a header field to the end of the message's header.
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='header'>A key/value pair. The key is the name of the
    /// header field, such as "From" or "Content-ID". The value is the
    /// header field's value.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The key or value of
    /// <paramref name='header'/> is null.</exception>
    /// <exception cref='ArgumentException'>The header field name is too
    /// long or contains an invalid character, or the header field's value
    /// is syntactically invalid.</exception>
    public Message AddHeader(KeyValuePair<string, string> header) {
      return this.AddHeader(header.Key, header.Value);
    }

    /// <summary>Adds a header field to the end of the message's header.
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='name'>Name of a header field, such as "From" or
    /// "Content-ID" .</param>
    /// <param name='value'>Value of the header field.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> or <paramref name='value'/> is null.</exception>
    /// <exception cref='ArgumentException'>The header field name is too
    /// long or contains an invalid character, or the header field's value
    /// is syntactically invalid.</exception>
    public Message AddHeader(string name, string value) {
      name = ValidateHeaderField(name, value);
      int index = this.headers.Count / 2;
      this.headers.Add(String.Empty);
      this.headers.Add(String.Empty);
      return this.SetHeader(index, name, value);
    }

    /// <summary>Generates this message's data in text form.
    /// <para>The generated message will have only Basic Latin code points
    /// (U+0000 to U+007F), and the transfer encoding will always be 7bit,
    /// quoted-printable, or base64 (the declared transfer encoding for
    /// this message will be ignored).</para>
    /// <para>The following applies to the following header fields: From,
    /// To, Cc, Bcc, Reply-To, Sender, Resent-To, Resent-From, Resent-Cc,
    /// Resent-Bcc, and Resent-Sender. If the header field exists, but has
    /// an invalid syntax, has no addresses, or appears more than once,
    /// this method will combine the addresses into one header field if
    /// possible (in the case of all fields given other than From and
    /// Sender), and otherwise generate a synthetic header field with the
    /// display-name set to the contents of all of the header fields with
    /// the same name, and the address set to
    /// <c>me@[header-name]-address.invalid</c> as the address (a
    /// <c>.invalid</c> address is a reserved address that can never belong
    /// to anyone). (An exception is that the Resent-* header fields may
    /// appear more than once.) The generated message should always have a
    /// From header field.</para>
    /// <para>If a Date and/or Message-ID header field doesn't exist, a
    /// field with that name will be generated (using the current local
    /// time for the Date field).</para>
    /// <para>When encoding the message's body, if the message has a text
    /// content type ("text/*"), the line breaks are a CR byte (carriage
    /// return, 0x0d) followed by an LF byte (line feed, 0x0a), CR alone,
    /// or LF alone. If the message has any other content type, only CR
    /// followed by LF is considered a line break.</para></summary>
    /// <returns>The generated message.</returns>
    /// <exception cref='PeterO.Mail.MessageDataException'>The message
    /// can't be generated.</exception>
    public string Generate() {
      var aw = new ArrayWriter();
      this.Generate(aw, 0);
      return DataUtilities.GetUtf8String(aw.ToArray(), false);
    }

    /// <summary>Generates this message's data as a byte array, using the
    /// same algorithm as the Generate method.</summary>
    /// <returns>The generated message as a byte array.</returns>
    public byte[] GenerateBytes() {
      var aw = new ArrayWriter();
      this.Generate(aw, 0);
      return aw.ToArray();
    }

    /// <summary>Gets the byte array for this message's body. This method
    /// doesn' t make a copy of that byte array.</summary>
    /// <returns>A byte array.</returns>
    public byte[] GetBody() {
      return this.body;
    }

    /// <summary>Gets the date and time extracted from this message's Date
    /// header field (the value of which is found as though
    /// GetHeader("date") were called). See
    /// <b>MailDateTime.ParseDateString(bool)</b> for more information on
    /// the format of the date-time array returned by this
    /// method.</summary>
    /// <returns>An array of 32-bit unsigned integers.</returns>
    public int[] GetDate() {
      string field = this.GetHeader("date");
      return (field == null) ? null : MailDateTime.ParseDateString(field, true);
    }

    /// <summary>Sets this message's Date header field to the given date
    /// and time.</summary>
    /// <param name='dateTime'>An array containing at least eight elements
    /// expressing a date and time. See
    /// <b>MailDateTime.ParseDateString(bool)</b> for more information on
    /// this parameter.</param>
    /// <returns>This object.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='dateTime'/> contains fewer than eight elements or contains
    /// invalid values (see <b>MailDateTime.ParseString(bool)</b>
    /// ).</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='dateTime'/> is null.</exception>
    public Message SetDate(int[] dateTime) {
      if (dateTime == null) {
        throw new ArgumentNullException(nameof(dateTime));
      }
      if (!MailDateTime.IsValidDateTime(dateTime)) {
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

    /// <summary>Returns the mail message contained in this message's
    /// body.</summary>
    /// <returns>A message object if this object's content type is
    /// "message/rfc822", "message/news", or "message/global", or null
    /// otherwise.</returns>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1024",
    Justification="This method may throw MessageDataException among other things - making it too heavyweight to be a property.")]
#endif
    public Message GetBodyMessage() {
      return (this.ContentType.TopLevelType.Equals("message",
  StringComparison.Ordinal) && (this.ContentType.SubType.Equals("rfc822",
  StringComparison.Ordinal) ||
           this.ContentType.SubType.Equals("news", StringComparison.Ordinal) ||
           this.ContentType.SubType.Equals("global",
  StringComparison.Ordinal))) ? new Message(this.body) : null;
    }

    /// <summary>Gets the name and value of a header field by
    /// index.</summary>
    /// <param name='index'>Zero-based index of the header field to
    /// get.</param>
    /// <returns>A key/value pair. The key is the name of the header field,
    /// such as "From" or "Content-ID". The value is the header field's
    /// value.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is 0 or at least as high as the number of header
    /// fields.</exception>
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

    /// <summary>Gets the first instance of the header field with the
    /// specified name, using a basic case-insensitive comparison. (Two
    /// strings are equal in such a comparison, if they match after
    /// converting the basic upper-case letters A to Z (U+0041 to U+005A)
    /// in both strings to basic lower-case letters.).</summary>
    /// <param name='name'>The name of a header field.</param>
    /// <returns>The value of the first header field with that name, or
    /// null if there is none.</returns>
    /// <exception cref='ArgumentNullException'>Name is null.</exception>
    public string GetHeader(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name, StringComparison.Ordinal)) {
          // Get the first instance of the header field
          return this.headers[i + 1];
        }
      }
      return null;
    }

    /// <summary>Gets an array with the values of all header fields with
    /// the specified name, using a basic case-insensitive comparison. (Two
    /// strings are equal in such a comparison, if they match after
    /// converting the basic upper-case letters A to Z (U+0041 to U+005A)
    /// in both strings to basic lower-case letters.).</summary>
    /// <param name='name'>The name of a header field.</param>
    /// <returns>An array containing the values of all header fields with
    /// the given name, in the order they appear in the message. The array
    /// will be empty if no header field has that name.</returns>
    /// <exception cref='ArgumentNullException'>Name is null.</exception>
    public string[] GetHeaderArray(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      var list = new List<string>();
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name, StringComparison.Ordinal)) {
          list.Add(this.headers[i + 1]);
        }
      }
      return (string[])list.ToArray();
    }

    /// <summary>Deletes all header fields in this message. Also clears
    /// this message's content disposition and resets its content type to
    /// MediaType.TextPlainAscii.</summary>
    /// <returns>This object.</returns>
    public Message ClearHeaders() {
      this.headers.Clear();
      this.contentType = MediaType.TextPlainAscii;
      this.contentDisposition = null;
      return this;
    }

    /// <summary>Removes a header field by index.
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='index'>Zero-based index of the header field to
    /// set.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is 0 or at least as high as the number of header
    /// fields.</exception>
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
      if (name.Equals("content-type", StringComparison.Ordinal)) {
        this.contentType = MediaType.TextPlainAscii;
      } else if (name.Equals("content-disposition", StringComparison.Ordinal)) {
        this.contentDisposition = null;
      }
      return this;
    }

    /// <summary>Removes all instances of the given header field from this
    /// message. If this is a multipart message, the header field is not
    /// removed from its body part headers. A basic case-insensitive
    /// comparison is used. (Two strings are equal in such a comparison, if
    /// they match after converting the basic upper-case letters A to Z
    /// (U+0041 to U+005A) in both strings to basic lower-case letters.).
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='name'>The name of the header field to remove.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    public Message RemoveHeader(string name) {
      if (name == null) {
        throw new ArgumentNullException(nameof(name));
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      // Remove the header field
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name, StringComparison.Ordinal)) {
          this.headers.RemoveAt(i);
          this.headers.RemoveAt(i);
          i -= 2;
        }
      }
      if (name.Equals("content-type", StringComparison.Ordinal)) {
        this.contentType = MediaType.TextPlainAscii;
      } else if (name.Equals("content-disposition", StringComparison.Ordinal)) {
        this.contentDisposition = null;
      }
      return this;
    }

    /// <summary>Sets the body of this message to the given byte array.
    /// This method doesn't make a copy of that byte array.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <returns>This object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    public Message SetBody(byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      this.body = bytes;
      return this;
    }

    /// <summary>Sets the name and value of a header field by index.
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='index'>Zero-based index of the header field to
    /// set.</param>
    /// <param name='header'>A key/value pair. The key is the name of the
    /// header field, such as "From" or "Content-ID". The value is the
    /// header field's value.</param>
    /// <returns>A Message object.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is 0 or at least as high as the number of header
    /// fields; or, the header field name is too long or contains an
    /// invalid character, or the header field's value is syntactically
    /// invalid.</exception>
    /// <exception cref='ArgumentNullException'>The key or value of
    /// <paramref name='header'/> is null.</exception>
    public Message SetHeader(int index, KeyValuePair<string, string> header) {
      return this.SetHeader(index, header.Key, header.Value);
    }

    /// <summary>Sets the name and value of a header field by index.
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='index'>Zero-based index of the header field to
    /// set.</param>
    /// <param name='name'>Name of a header field, such as "From" or
    /// "Content-ID" .</param>
    /// <param name='value'>Value of the header field.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is 0 or at least as high as the number of header
    /// fields; or, the header field name is too long or contains an
    /// invalid character, or the header field's value is syntactically
    /// invalid.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> or <paramref name='value'/> is null.</exception>
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
      if (name.Equals("content-type", StringComparison.Ordinal)) {
        this.contentType = MediaType.Parse(value);
      } else if (name.Equals("content-disposition", StringComparison.Ordinal)) {
        this.contentDisposition = ContentDisposition.Parse(value);
      }
      return this;
    }

    /// <summary>Sets the value of a header field by index without changing
    /// its name.
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='index'>Zero-based index of the header field to
    /// set.</param>
    /// <param name='value'>Value of the header field.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='index'/> is 0 or at least as high as the number of header
    /// fields; or, the header field name is too long or contains an
    /// invalid character, or the header field's value is syntactically
    /// invalid.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='value'/> is null.</exception>
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

    /// <summary>Decodes RFC 2047 encoded words from the given header field
    /// value and returns a string with those words decoded. For an example
    /// of encoded words, see the constructor for
    /// PeterO.Mail.NamedAddress.</summary>
    /// <param name='name'>Name of the header field. This determines the
    /// syntax of the "value" parameter and is necessary to help this
    /// method interpret encoded words properly.</param>
    /// <param name='value'>A header field value that could contain encoded
    /// words. For example, if the name parameter is "From", this parameter
    /// could be "=?utf-8?q?me?= &lt;me@example.com&gt;".</param>
    /// <returns>The header field value with valid encoded words
    /// decoded.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    public static string DecodeHeaderValue(string name, string value) {
      return HeaderFieldParsers.GetParser(name).DecodeEncodedWords(value);
    }

    /// <summary>Sets the value of this message's header field. If a header
    /// field with the same name exists, its value is replaced. If the
    /// header field's name occurs more than once, only the first instance
    /// of the header field is replaced.
    /// <para>This method updates the ContentType and ContentDisposition
    /// properties if those header fields have been modified by this
    /// method.</para></summary>
    /// <param name='name'>The name of a header field, such as "from" or
    /// "subject" .</param>
    /// <param name='value'>The header field's value.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentException'>The header field name is too
    /// long or contains an invalid character, or the header field's value
    /// is syntactically invalid.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> or <paramref name='value'/> is null.</exception>
    public Message SetHeader(string name, string value) {
      name = ValidateHeaderField(name, value);
      // Add the header field
      var index = 0;
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name, StringComparison.Ordinal)) {
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

    /// <summary>Sets the body of this message to the specified string in
    /// Hypertext Markup Language (HTML) format. The character sequences CR
    /// (carriage return, "\r", U+000D), LF (line feed, "\n", U+000A), and
    /// CR/LF will be converted to CR/LF line breaks. Unpaired surrogate
    /// code points will be replaced with replacement characters.</summary>
    /// <param name='str'>A string consisting of the message in HTML
    /// format.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    public Message SetHtmlBody(string str) {
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      this.body = DataUtilities.GetUtf8Bytes(str, true, true);
      this.ContentType = IsShortAndAllAscii(str) ? TextHtmlAscii :
        TextHtmlUtf8;
      return this;
    }

    /// <summary>Sets the body of this message to a multipart body with
    /// plain text and Hypertext Markup Language (HTML) versions of the
    /// same message. The character sequences CR (carriage return, "\r",
    /// U+000D), LF (line feed, "\n", U+000A), and CR/LF will be converted
    /// to CR/LF line breaks. Unpaired surrogate code points will be
    /// replaced with replacement characters.</summary>
    /// <param name='text'>A string consisting of the plain text version of
    /// the message.</param>
    /// <param name='html'>A string consisting of the HTML version of the
    /// message.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='text'/> or <paramref name='html'/> is null.</exception>
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
      string mtypestr = "multipart/alternative; " +
        "boundary=\"=_Boundary00000000\"";
      this.ContentType = MediaType.Parse(mtypestr);
      IList<Message> messageParts = this.Parts;
      messageParts.Clear();
      messageParts.Add(textMessage);
      messageParts.Add(htmlMessage);
      return this;
    }

    /// <summary>Sets the body of this message to a multipart body with
    /// plain text, Markdown, and Hypertext Markup Language (HTML) versions
    /// of the same message. The character sequences CR (carriage return,
    /// "\r", U+000D), LF (line feed, "\n", U+000A), and CR/LF will be
    /// converted to CR/LF line breaks. Unpaired surrogate code points will
    /// be replaced with replacement characters.</summary>
    /// <param name='text'>A string consisting of the plain text version of
    /// the message. Can be null, in which case the value of the "markdown"
    /// parameter is used as the plain text version.</param>
    /// <param name='markdown'>A string consisting of the Markdown version
    /// of the message. For interoperability, this Markdown version will be
    /// converted to HTML, where the Markdown text is assumed to be in the
    /// original Markdown flavor.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='markdown'/> is null.</exception>
    /// <remarks>
    /// <para>REMARK: The Markdown-to-HTML implementation currently
    /// supports all features of original Markdown, except that the
    /// implementation:</para>
    /// <list>
    /// <item>does not strictly check the placement of "block-level HTML
    /// elements",</item>
    /// <item>does not prevent Markdown content from being interpreted as
    /// such merely because it's contained in a "block-level HTML element",
    /// and</item>
    /// <item>does not deliberately use HTML escapes to obfuscate email
    /// addresses wrapped in angle-brackets.</item></list></remarks>
    public Message SetTextAndMarkdown(string text, string markdown) {
      if (markdown == null) {
        throw new ArgumentNullException(nameof(markdown));
      }
      text = text ?? markdown;
      Message textMessage = NewBodyPart().SetTextBody(text);
      Message markdownMessage = NewBodyPart().SetTextBody(markdown);
      string mtypestr = "text/markdown; charset=utf-8";
      markdownMessage.ContentType = MediaType.Parse(mtypestr);
      // Take advantage of SetTextBody's line break conversion
      string markdownText = markdownMessage.GetBodyString();
      Message htmlMessage = NewBodyPart().SetHtmlBody(
         FormatFlowed.MarkdownText(markdownText, 0));
      mtypestr = "multipart/alternative; boundary=\"=_Boundary00000000\"";
      this.ContentType = MediaType.Parse(mtypestr);
      IList<Message> messageParts = this.Parts;
      messageParts.Clear();
      messageParts.Add(textMessage);
      messageParts.Add(markdownMessage);
      messageParts.Add(htmlMessage);
      return this;
    }

    /// <summary>Sets the body of this message to the specified plain text
    /// string. The character sequences CR (carriage return, "\r", U+000D),
    /// LF (line feed, "\n", U+000A), and CR/LF will be converted to CR/LF
    /// line breaks. Unpaired surrogate code points will be replaced with
    /// replacement characters. This method changes this message's media
    /// type to plain text.</summary>
    /// <param name='str'>A string consisting of the message in plain text
    /// format.</param>
    /// <returns>This instance.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
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
      return this.AddBodyPart(
        inputStream,
        mediaType,
        filename,
        disposition,
        false);
    }

    /// <summary>Adds an inline body part with an empty body and with the
    /// given media type to this message. Before the new body part is
    /// added, if this message isn't already a multipart message, it
    /// becomes a "multipart/mixed" message with the current body converted
    /// to an inline body part.</summary>
    /// <param name='mediaType'>A media type to assign to the body
    /// part.</param>
    /// <returns>A Message object for the generated body part.</returns>
    public Message AddInline(MediaType mediaType) {
      return this.AddBodyPart(null, mediaType, null, "inline", true);
    }

    /// <summary>Adds an attachment with an empty body and with the given
    /// media type to this message. Before the new attachment is added, if
    /// this message isn't already a multipart message, it becomes a
    /// "multipart/mixed" message with the current body converted to an
    /// inline body part.</summary>
    /// <param name='mediaType'>A media type to assign to the
    /// attachment.</param>
    /// <returns>A Message object for the generated attachment.</returns>
    public Message AddAttachment(MediaType mediaType) {
      return this.AddBodyPart(null, mediaType, null, "attachment", true);
    }

    private Message AddBodyPart(
      Stream inputStream,
      MediaType mediaType,
      string filename,
      string disposition,
      bool allowNullStream) {
      if (!allowNullStream && inputStream == null) {
        throw new ArgumentNullException(nameof(inputStream));
      }
      if (mediaType == null) {
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
            if (mediaType.IsMultipart) {
              try {
                var transform = DataIO.ToReader(inputStream);
                bodyPart.ReadMultipartBody(transform);
              } catch (InvalidOperationException ex) {
                throw new MessageDataException(ex.Message, ex);
              }
            } else {
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
          }
        } catch (IOException ex) {
          throw new MessageDataException("An I/O error occurred.", ex);
        }
      }
      var dispBuilder = new DispositionBuilder(disposition);
      if (!String.IsNullOrEmpty(filename)) {
        string basename = BaseName(filename);
        if (!String.IsNullOrEmpty(basename)) {
          dispBuilder.SetParameter(
            "filename",
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
      var i = 0;
      for (i = filename.Length - 1; i >= 0; --i) {
        if (filename[i] == '\\' || filename[i] == '/') {
          return filename.Substring(i + 1);
        }
      }
      return filename;
    }

    private static string ExtensionName(string filename) {
      var i = 0;
      for (i = filename.Length - 1; i >= 0; --i) {
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
        if (ext.Equals(".doc", StringComparison.Ordinal) ||
ext.Equals(".dot", StringComparison.Ordinal)) {
          return MediaType.Parse("application/msword");
        }
        if (ext.Equals(".pdf", StringComparison.Ordinal)) {
          return MediaType.Parse("application/pdf");
        }
        if (ext.Equals(".key", StringComparison.Ordinal)) {
          return MediaType.Parse("application/pgp-keys");
        }
        if (ext.Equals(".sig", StringComparison.Ordinal)) {
          return MediaType.Parse("application/pgp-signature");
        }
        if (ext.Equals(".rtf", StringComparison.Ordinal)) {
          return MediaType.Parse("application/rtf");
        }
        if (ext.Equals(".docx", StringComparison.Ordinal)) {
          return

  MediaType.Parse("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        if (ext.Equals(".zip", StringComparison.Ordinal)) {
          return MediaType.Parse("application/zip");
        }
        if (ext.Equals(".m4a", StringComparison.Ordinal) ||
ext.Equals(".mp2", StringComparison.Ordinal) ||
ext.Equals(".mp3", StringComparison.Ordinal) ||
ext.Equals(".mpega", StringComparison.Ordinal) ||
ext.Equals(".mpga", StringComparison.Ordinal)) {
          return MediaType.Parse("audio/mpeg");
        }
        if (ext.Equals(".gif", StringComparison.Ordinal)) {
          return MediaType.Parse("image/gif");
        }
        if (ext.Equals(".jpe", StringComparison.Ordinal) ||
ext.Equals(".jpeg", StringComparison.Ordinal) ||
ext.Equals(".jpg", StringComparison.Ordinal)) {
          return MediaType.Parse("image/jpeg");
        }
        if (ext.Equals(".png", StringComparison.Ordinal)) {
          return MediaType.Parse("image/png");
        }
        if (ext.Equals(".tif", StringComparison.Ordinal) ||
ext.Equals(".tiff", StringComparison.Ordinal)) {
          return MediaType.Parse("image/tiff");
        }
        if (ext.Equals(".eml", StringComparison.Ordinal)) {
          return MediaType.Parse("message/rfc822");
        }
        if (ext.Equals(".rst", StringComparison.Ordinal)) {
          return MediaType.Parse("text/prs.fallenstein.rst\u003bcharset=utf-8");
        }
        if (ext.Equals(".htm", StringComparison.Ordinal) ||
ext.Equals(".html", StringComparison.Ordinal) ||
ext.Equals(".shtml", StringComparison.Ordinal)) {
          return MediaType.Parse("text/html\u003bcharset=utf-8");
        }
        if (ext.Equals(".md", StringComparison.Ordinal) ||
ext.Equals(".markdown", StringComparison.Ordinal)) {
          return MediaType.Parse("text/markdown\u003bcharset=utf-8");
        }
        if (ext.Equals(".asc", StringComparison.Ordinal) ||
ext.Equals(".brf", StringComparison.Ordinal) ||
ext.Equals(".pot", StringComparison.Ordinal) ||
ext.Equals(".srt", StringComparison.Ordinal) ||
ext.Equals(".text", StringComparison.Ordinal) ||
ext.Equals(".txt", StringComparison.Ordinal)) {
          return MediaType.Parse("text/plain\u003bcharset=utf-8");
        }
      }
      return MediaType.ApplicationOctetStream;
    }

    /// <summary>Adds an attachment to this message in the form of data
    /// from the given readable stream, and with the given media type.
    /// Before the new attachment is added, if this message isn't already a
    /// multipart message, it becomes a "multipart/mixed" message with the
    /// current body converted to an inline body part.</summary>
    /// <param name='inputStream'>A readable data stream.</param>
    /// <param name='mediaType'>A media type to assign to the
    /// attachment.</param>
    /// <returns>A Message object for the generated attachment.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='inputStream'/> or <paramref name='mediaType'/> is
    /// null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>An I/O error
    /// occurred.</exception>
    /// <example>
    ///  The following example (written in C# for the.NET
    /// version) is an extension method that adds an attachment
    /// from a byte array to a message.
    /// <code>public static Message AddAttachmentFromBytes(this Message msg, byte[]
    /// bytes, MediaType mediaType) { using (var fs = new MemoryStream(bytes)) {
    /// return msg.AddAttachment(fs, mediaType); } }</code>
    ///  .
    /// </example>
    public Message AddAttachment(Stream inputStream, MediaType mediaType) {
      return this.AddBodyPart(inputStream, mediaType, null, "attachment");
    }

    /// <summary>Adds an attachment to this message in the form of data
    /// from the given readable stream, and with the given file name.
    /// Before the new attachment is added, if this message isn't already a
    /// multipart message, it becomes a "multipart/mixed" message with the
    /// current body converted to an inline body part.</summary>
    /// <param name='inputStream'>A readable data stream.</param>
    /// <param name='filename'>A file name to assign to the attachment. Can
    /// be null or empty, in which case no file name is assigned. Only the
    /// file name portion of this parameter is used, which in this case
    /// means the portion of the string after the last "/" or "\", if
    /// either character exists, or the entire string otherwise An
    /// appropriate media type (or "application/octet-stream") will be
    /// assigned to the attachment based on this file name's extension. If
    /// the file name has an extension .txt, .text, .htm, .html, .shtml,
    /// .asc, .brf, .pot, .rst, .md, .markdown, or .srt, the media type
    /// will have a "charset" of "utf-8".</param>
    /// <returns>A Message object for the generated attachment.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='inputStream'/> is null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>An I/O error
    /// occurred.</exception>
    public Message AddAttachment(Stream inputStream, string filename) {
      return
  this.AddBodyPart(
  inputStream,
  SuggestMediaType(filename),
  filename,
  "attachment");
    }

    /// <summary>Adds an attachment to this message in the form of data
    /// from the given readable stream, and with the given media type and
    /// file name. Before the new attachment is added, if this message
    /// isn't already a multipart message, it becomes a "multipart/mixed"
    /// message with the current body converted to an inline body
    /// part.</summary>
    /// <param name='inputStream'>A readable data stream.</param>
    /// <param name='mediaType'>A media type to assign to the
    /// attachment.</param>
    /// <param name='filename'>A file name to assign to the attachment. Can
    /// be null or empty, in which case no file name is assigned. Only the
    /// file name portion of this parameter is used, which in this case
    /// means the portion of the string after the last "/" or "\", if
    /// either character exists, or the entire string otherwise.</param>
    /// <returns>A Message object for the generated attachment.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='inputStream'/> or <paramref name='mediaType'/> is
    /// null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>An I/O error
    /// occurred.</exception>
    public Message AddAttachment(
      Stream inputStream,
      MediaType mediaType,
      string filename) {
      return this.AddBodyPart(
        inputStream,
        mediaType,
        filename,
        "attachment");
    }

    /// <summary>Adds an inline body part to this message in the form of
    /// data from the given readable stream, and with the given media type.
    /// Before the new body part is added, if this message isn't already a
    /// multipart message, it becomes a "multipart/mixed" message with the
    /// current body converted to an inline body part.</summary>
    /// <param name='inputStream'>A readable data stream.</param>
    /// <param name='mediaType'>A media type to assign to the body
    /// part.</param>
    /// <returns>A Message object for the generated body part.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='inputStream'/> or <paramref name='mediaType'/> is
    /// null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>An I/O error
    /// occurred.</exception>
    /// <example>
    ///  The following example (written in C# for the.NET
    /// version) is an extension method that adds an inline
    /// body part from a byte array to a message.
    /// <code>public static Message AddInlineFromBytes(this Message msg, byte[] bytes,
    /// MediaType mediaType) { using (MemoryStream fs = new MemoryStream(bytes))
    /// { return msg.AddInline(fs, mediaType); } }</code>
    ///  .
    /// </example>
    public Message AddInline(Stream inputStream, MediaType mediaType) {
      return this.AddBodyPart(inputStream, mediaType, null, "inline");
    }

    /// <summary>Adds an inline body part to this message in the form of
    /// data from the given readable stream, and with the given file name.
    /// Before the new body part is added, if this message isn't already a
    /// multipart message, it becomes a "multipart/mixed" message with the
    /// current body converted to an inline body part.</summary>
    /// <param name='inputStream'>A readable data stream.</param>
    /// <param name='filename'>A file name to assign to the inline body
    /// part. Can be null or empty, in which case no file name is assigned.
    /// Only the file name portion of this parameter is used, which in this
    /// case means the portion of the string after the last "/" or "\", if
    /// either character exists, or the entire string otherwise An
    /// appropriate media type (or "application/octet-stream") will be
    /// assigned to the body part based on this file name's extension. If
    /// the file name has an extension .txt, .text, .htm, .html, .shtml,
    /// .asc, .brf, .pot, .rst, .md, .markdown, or .srt, the media type
    /// will have a "charset" of "utf-8".</param>
    /// <returns>A Message object for the generated body part.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='inputStream'/> or "mediaType" is null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>An I/O error
    /// occurred.</exception>
    public Message AddInline(Stream inputStream, string filename) {
      return this.AddBodyPart(
  inputStream,
  SuggestMediaType(filename),
  filename,
  "inline");
    }

    /// <summary>Adds an inline body part to this message in the form of
    /// data from the given readable stream, and with the given media type
    /// and file name. Before the new body part is added, if this message
    /// isn't already a multipart message, it becomes a "multipart/mixed"
    /// message with the current body converted to an inline body
    /// part.</summary>
    /// <param name='inputStream'>A readable data stream.</param>
    /// <param name='mediaType'>A media type to assign to the body
    /// part.</param>
    /// <param name='filename'>A file name to assign to the body
    /// part.</param>
    /// <returns>A Message object for the generated body part.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='inputStream'/> or <paramref name='mediaType'/> is
    /// null.</exception>
    /// <exception cref='PeterO.Mail.MessageDataException'>An I/O error
    /// occurred.</exception>
    public Message AddInline(
      Stream inputStream,
      MediaType mediaType,
      string filename) {
      return this.AddBodyPart(inputStream, mediaType, filename, "inline");
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

    /// <summary>Selects a body part for a multiple-language message (
    /// <c>multipart/multilingual</c> ) according to the given language
    /// priority list.</summary>
    /// <param name='languages'>A list of basic language ranges, sorted in
    /// descending order of priority (see the
    /// LanguageTags.LanguageTagFilter method).</param>
    /// <returns>The best matching body part for the given languages. If
    /// the body part has no subject, then the top-level subject is used.
    /// If this message is not a multipart/multilingual message or has
    /// fewer than two body parts, returns this object. If no body part
    /// matches the given languages, returns the last body part if its
    /// language is "zxx", or the second body part otherwise.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> is null.</exception>
    public Message SelectLanguageMessage(
       IList<string> languages) {
      return this.SelectLanguageMessage(languages, false);
    }

    /// <summary>Selects a body part for a multiple-language message (
    /// <c>multipart/multilingual</c> ) according to the given language
    /// priority list and original-language preference.</summary>
    /// <param name='languages'>A list of basic language ranges, sorted in
    /// descending order of priority (see the
    /// LanguageTags.LanguageTagFilter method).</param>
    /// <param name='preferOriginals'>If true, a body part marked as the
    /// original language version is chosen if it matches one of the given
    /// language ranges, even if the original language has a lower priority
    /// than another language with a matching body part.</param>
    /// <returns>The best matching body part for the given languages. If
    /// the body part has no subject, then the top-level subject is used.
    /// If this message is not a multipart/multilingual message or has
    /// fewer than two body parts, returns this object. If no body part
    /// matches the given languages, returns the last body part if its
    /// language is "zxx", or the second body part otherwise.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> is null.</exception>
    public Message SelectLanguageMessage(
       IList<string> languages,
       bool preferOriginals) {
      if (this.ContentType.TypeAndSubType.Equals("multipart/multilingual",
  StringComparison.Ordinal) && this.Parts.Count >= 2) {
        string subject = this.GetHeader("subject");
        int passes = preferOriginals ? 2 : 1;
        IList<string> clang;
        IList<string> filt;
        for (var i = 0; i < passes; ++i) {
          foreach (Message part in this.Parts) {
            clang = LanguageTags.GetLanguageList(part.GetHeader(
        "content-language"));
            if (clang == null) {
              continue;
            }
            if (preferOriginals && i == 0) { // Allow originals only, on first
              string ctt =
  GetContentTranslationType(part.GetHeader("content-translation-type"));
              if (!ctt.Equals("original", StringComparison.Ordinal)) {
                continue;
              }
            }
            filt = LanguageTags.LanguageTagFilter(languages, clang);
            if (filt.Count > 0) {
              Message ret = part.GetBodyMessage();
              if (ret != null) {
                if (subject != null && ret.GetHeader("subject") == null) {
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
          if (subject != null && firstmsg.GetHeader("subject") == null) {
            firstmsg.SetHeader("subject", subject);
          }
          return firstmsg;
        }
      }
      return this;
    }

    /// <summary>Generates a multilingual message (see RFC 8255) from a
    /// list of messages and a list of language strings.</summary>
    /// <param name='messages'>A list of messages forming the parts of the
    /// multilingual message object. Each message should have the same
    /// content, but be in a different language. Each message must have a
    /// From header field and use the same email address in that field as
    /// the other messages. The messages should be ordered in descending
    /// preference of language.</param>
    /// <param name='languages'>A list of language strings corresponding to
    /// the messages given in the "messages" parameter. A language string
    /// at a given index corresponds to the message at the same index. Each
    /// language string must follow the syntax of the Content-Language
    /// header field (see LanguageTags.GetLanguageList).</param>
    /// <returns>A Message object with the content type
    /// "multipart/multilingual" . It will begin with an explanatory body
    /// part and be followed by the messages given in the <paramref
    /// name='messages'/> parameter in the order given.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='messages'/> or <paramref name='languages'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='messages'/> or <paramref name='languages'/> is empty, their
    /// lengths don't match, at least one message is "null", each message
    /// doesn't contain the same email addresses in their From header
    /// fields, <paramref name='languages'/> contains a syntactically
    /// invalid language tag list, <paramref name='languages'/> contains
    /// the language tag "zzx" not appearing alone or at the end of the
    /// language tag list, or the first message contains no From header
    /// field.</exception>
    public static Message MakeMultilingualMessage(
  IList<Message> messages,
  IList<string> languages) {
      if (messages == null) {
        throw new ArgumentNullException(nameof(messages));
      }
      if (languages == null) {
        throw new ArgumentNullException(nameof(languages));
      }
      if (messages.Count < 0) {
        throw new ArgumentException("messages.Count (" + messages.Count +
          ") is less than 0");
      }
      if (messages.Count != languages.Count) {
        throw new ArgumentException("messages.Count (" + messages.Count +
          ") is not equal to " + languages.Count);
      }
      StringBuilder prefaceBody;
      for (var i = 0; i < messages.Count; ++i) {
        if (messages[i] == null) {
          throw new ArgumentException("A message in 'messages' is null");
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
      prefaceBody = new StringBuilder().Append("This is a multilingual " +
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
        bool langInd = i == languages.Count - 1 && langs.Count == 1 &&
          langs[0].Equals("zxx", StringComparison.Ordinal);
        if (!langInd && LanguageTags.LanguageTagFilter(
          zxx,
          langs).Count > 0) {
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
      msg.ContentDisposition = ContentDisposition.Parse("inline");
      string toHeader = messages[0].GetHeader("to");
      Message preface;
      if (toHeader != null) {
        msg.SetHeader("to", toHeader);
      }
      msg.SetHeader("subject", prefaceSubject.ToString());
      preface = msg.AddInline(MediaType.Parse("text/plain;charset=utf-8"));
      preface.SetTextBody(prefaceBody.ToString());
      for (var i = 0; i < messages.Count; ++i) {
        MediaType mt = MediaType.Parse("message/rfc822");
        string msgstring = messages[i].Generate();
        if (msgstring.IndexOf("\r\n--", StringComparison.Ordinal) >= 0 || (
          msgstring.Length >= 2 && msgstring[0] == '-' &&
             msgstring[1] == '-')) {
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
        if (c == 0 || c == 0x7f) {
          // Console.WriteLine("NULL or DEL character");
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
        bool origRecipient = fieldName.Equals("original-recipient",
  StringComparison.Ordinal);
        bool finalRecipient = fieldName.Equals("final-recipient",
  StringComparison.Ordinal);
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
              false); // throws on invalid UTF-8
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
              string field = origRecipient ? "Downgraded-Original-Recipient" :
                "Downgraded-Final-Recipient";
              headerValue = DataUtilities.GetUtf8String(
                bytes,
                headerValueStart,
                headerValueEnd - headerValueStart,
                true); // replaces invalid UTF-8
              string newField = HeaderEncoder.EncodeFieldAsEncodedWords(
                field,
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

    private static HeaderEncoder EncodeCommentsInText(
      HeaderEncoder enc,
      string str) {
      var i = 0;
      var begin = 0;
      if (str.IndexOf('(') < 0) {
        return enc.AppendString(str);
      }
      var sb = new StringBuilder();
      while (i < str.Length) {
        if (str[i] == '(') {
          int si = HeaderParserUtility.ParseCommentLax(
            str,
            i,
            str.Length,
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
        // if (atomText != typeEnd) {
        // isUtf8 = false;
        // }
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
            EncodeCommentsInText(
  encoder,
  HeaderEncoder.TrimLeadingFWS(typePart + builder));
          } else {
            EncodeCommentsInText(
  encoder,
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
        status[0] = 0; // Unchanged
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

    internal static bool HasTextToEscapeOrEncodedWordStarts(
      string s,
      int index,
      int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, true);
    }

    internal static bool HasTextToEscape(string s, int index, int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, false);
    }

    internal static bool HasTextToEscape(string s) {
      return HasTextToEscapeOrEncodedWordStarts(s, 0, s.Length, false);
    }

    internal static bool HasTextToEscapeOrEncodedWordStarts(
      string s,
      int index,
      int endIndex,
      bool checkEWStarts) {
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

    internal static IList<NamedAddress> ParseAddresses(string[] values) {
      var list = new List<NamedAddress>();
      foreach (string addressValue in values) {
        list.AddRange(NamedAddress.ParseAddresses(addressValue));
      }
      return list;
    }

    internal static int ParseUnstructuredText(
      string s,
      int startIndex,
      int endIndex) {
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
              found = true;
              break;
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
          // DEVIATION: Replace bytes greater than 127 and null with the
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
      // Line length in OCTETS, not characters
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
            throw new MessageDataException("Premature end of message " +
               "before all headers were read, while reading header field name");
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
              // MaxHardHeaderLineLength octets includes the colon
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
              " while reading header field value";
            throw new MessageDataException(exstring);
          }
          if (c == '\r') {
            // We're only looking for the single-byte LF, so
            // there's no need to use ReadUtf8Char
            c = stream.ReadByte();
            if (c < 0) {
              string exstring = "Premature end before all headers were read," +
                " while looking for LF";
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
            ++lineCount; // Increment for the CR
          }
          // NOTE: Header field line limit not enforced here, only
          // in the header field name; it's impossible to generate
          // a conforming message if the name is too long
          // NOTE: Some emails still have bytes greater than 127 in an unencoded
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
            // DebugUtility.Log("c=" + c + "," + lineCount + "," +
            // state[0]+ ","+state[1]+","+state[2]);
            lineCount = state[0];
            ungetLast = state[2] == 1;
            lastByte = state[1];
            if (c <= 0xffff) {
              sb.Append((char)c);
            } else if (c <= 0x10ffff) {
              sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
              sb.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
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
      if (ungetState[1] < 0x80) {
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
            ungetState[2] = 1; // unget last
            ungetState[1] = b; // last byte
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
          ungetState[2] = 1; // unget last
          ungetState[1] = b; // last byte
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
      return allTextBytes ? EncodingSevenBit :
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
          throw new ArgumentException("Header field value contains invalid" +
"\u0020text");
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
      bool isMultipart = builder.TopLevelType.Equals(
        "multipart",
        StringComparison.Ordinal);
      string boundary = String.Empty;
      if (isMultipart) {
        boundary = GenerateBoundary(depth);
        builder.SetParameter("boundary", boundary);
      } else {
        if (builder.TopLevelType.Equals("message", StringComparison.Ordinal)) {
          if (builder.SubType.Equals("delivery-status",
                     StringComparison.Ordinal) ||
                     builder.SubType.Equals("global-delivery-status",
                     StringComparison.Ordinal) ||
                     builder.SubType.Equals("disposition-notification",
                     StringComparison.Ordinal) ||
                     builder.SubType.Equals("global-disposition-notification",
                     StringComparison.Ordinal)) {
            bodyToWrite = DowngradeDeliveryStatus(bodyToWrite);
          }
          bool msgCanBeUnencoded = CanBeUnencoded(bodyToWrite, depth > 0);
          if ((builder.SubType.Equals("rfc822", StringComparison.Ordinal) ||
builder.SubType.Equals(
            "news",
            StringComparison.Ordinal)) && !msgCanBeUnencoded) {
            builder.SetSubType("global");
          } else if (builder.SubType.Equals("disposition-notification",
  StringComparison.Ordinal) && !msgCanBeUnencoded) {
            builder.SetSubType("global-disposition-notification");
          } else if (builder.SubType.Equals("delivery-status",
  StringComparison.Ordinal) && !msgCanBeUnencoded) {
            builder.SetSubType("global-delivery-status");
          } else if (!msgCanBeUnencoded && !builder.SubType.Equals("global",
  StringComparison.Ordinal) &&
            !builder.SubType.Equals("global-disposition-notification",
  StringComparison.Ordinal) && !builder.SubType.Equals("global-delivery-status",
  StringComparison.Ordinal) && !builder.SubType.Equals("global-headers",
  StringComparison.Ordinal)) {
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
      transferEnc = topLevel.Equals("message", StringComparison.Ordinal) ||
        topLevel.Equals("multipart", StringComparison.Ordinal) ?
(topLevel.Equals("multipart", StringComparison.Ordinal) || (
          !builder.SubType.Equals("global", StringComparison.Ordinal) &&
          !builder.SubType.Equals("global-headers", StringComparison.Ordinal) &&
          !builder.SubType.Equals("global-disposition-notification",
  StringComparison.Ordinal) && !builder.SubType.Equals("global-delivery-status",
  StringComparison.Ordinal))) ? EncodingSevenBit : TransferEncodingToUse(
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
        if (name.Equals("content-type", StringComparison.Ordinal)) {
          if (haveContentType) {
            // Already outputted, continue
            continue;
          }
          haveContentType = true;
          value = builder.ToString();
        }
        if (name.Equals("content-disposition", StringComparison.Ordinal)) {
          if (haveContentDisp || contentDisp == null) {
            // Already outputted, continue
            continue;
          }
          haveContentDisp = true;
          value = contentDisp;
        } else if (name.Equals("content-transfer-encoding",
  StringComparison.Ordinal)) {
          if (haveContentEncoding) {
            // Already outputted, continue
            continue;
          }
          haveContentEncoding = true;
          value = encodingString;
        } else if (name.Equals("date", StringComparison.Ordinal)) {
          if (haveDate) {
            continue;
          }
          haveDate = true;
        } else if (name.Equals("from", StringComparison.Ordinal)) {
          if (haveFrom) {
            // Already outputted, continue
            continue;
          }
          haveFrom = true;
        }
        if (
          depth > 0 &&
name.Length >= 2 &&
          name[0] == '-' && name[1] == '-') {
          // don't generate header fields starting with "--"
          // in body parts
          continue;
        }
        if (name.Equals("mime-version", StringComparison.Ordinal)) {
          if (depth > 0) {
            // Don't output if this is a body part
            continue;
          }
          haveMimeVersion = true;
        } else if (name.Equals("message-id", StringComparison.Ordinal)) {
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
              /*
              #if DEBUG
                              DebugUtility.Log (name+" "+isValidAddressing);
                              {
                                var ssb = new StringBuilder();
                                foreach (var mhs in this.GetMultipleHeaders
(name)) {
                                  ssb.Append (mhs + " ");
                               if (isValidAddressing && name=="sender") {
                                  DebugUtility.Log(""+new NamedAddress(mhs));
                                  DebugUtility.Log("" + new
NamedAddress(mhs).DisplayName);
                                  DebugUtility.Log("" + new
NamedAddress(mhs).Address);
                               }
                                }
                                DebugUtility.Log (ssb.ToString());
              }
#endif
              */ if (!isValidAddressing) {
                value = String.Empty;
                if (!name.Equals("from", StringComparison.Ordinal) &&
!name.Equals("sender", StringComparison.Ordinal)) {
                  value = GenerateAddressList(
                    NamedAddress.ParseAddresses(value));
                }
                if (value.Length == 0) {
                  // Synthesize a field
                  rawField = this.SynthesizeField(name);
                }
              }
            } else if (headerIndex <= 10) {
              // Resent-* fields can appear more than once
              value = GenerateAddressList(
                NamedAddress.ParseAddresses(value));
              if (value.Length == 0) {
                // No addresses, synthesize a field
                rawField = this.SynthesizeField(name);
              }
            }
          }
        }
        rawField = rawField ?? HeaderEncoder.EncodeField(name, value);
        if (HeaderEncoder.CanOutputRaw(rawField)) {
          AppendAscii(output, rawField);
        } else {
          // DebugUtility.Log("Can't output '"+name+"' raw");
          string downgraded = HeaderFieldParsers.GetParser(name)
                    .DowngradeHeaderField(name, value);
          if (
            HasTextToEscape(
              downgraded,
              0,
              downgraded.Length)) {
            if (name.Equals("message-id", StringComparison.Ordinal) ||
              name.Equals("resent-message-id", StringComparison.Ordinal) ||
              name.Equals("in-reply-to", StringComparison.Ordinal) ||
              name.Equals("references", StringComparison.Ordinal) ||
              name.Equals(
                "original-recipient",
                StringComparison.Ordinal) ||
              name.Equals("final-recipient", StringComparison.Ordinal)) {
              // Header field still contains invalid characters (such
              // as non-ASCII characters in 7-bit messages), convert
              // to a downgraded field
              downgraded = HeaderEncoder.EncodeFieldAsEncodedWords(
                  "downgraded-" + name,
                  ParserUtility.TrimSpaceAndTab(value));
            } else {
#if DEBUG
              string exText = "Header field still has non-Ascii or controls: " +
                    name + "\n" + downgraded;
              throw new MessageDataException(exText);
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
        // NOTE: Use global rather than local time; there are overriding
        // reasons not to use local time, despite the SHOULD in RFC 5322
        string dateString = MailDateTime.GenerateDateString(
          DateTimeUtilities.GetCurrentGlobalTime());
        AppendAscii(
          output,
          dateString);
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
      bool isText = builder.TopLevelType.Equals("text",
  StringComparison.Ordinal);
      switch (transferEnc) {
        case EncodingBase64:
          bodyEncoder = new Base64Encoder(
            true,
            isText,
            false);
          break;
        case EncodingQuotedPrintable:
          bodyEncoder = new QuotedPrintableEncoder(
            isText ? 2 : 0,
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
        bool writeNewLine = depth > 0;
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
        if (this.headers[i].Equals(name, StringComparison.Ordinal)) {
          headerList.Add(this.headers[i + 1]);
        }
      }
      return (string[])headerList.ToArray();
    }

    private bool IsValidAddressingField(string name) {
      name = DataUtilities.ToLowerCaseAscii(name);
      var have = false;
      for (int i = 0; i < this.headers.Count; i += 2) {
        if (this.headers[i].Equals(name, StringComparison.Ordinal)) {
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

    /// <summary>Creates a message object from a MailTo URI (uniform
    /// resource identifier). For more information, see
    /// <b>FromMailtoUri(string)</b>.</summary>
    /// <param name='url'>A MailTo URI.</param>
    /// <returns>A Message object created from the given MailTo URI. Returs
    /// null if <paramref name='url'/> is null, is syntactically invalid,
    /// or is not a MailTo URI.</returns>
    [Obsolete("Renamed to FromMailtoUri.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1055",
      Justification = "This is an obsolete API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1054",
      Justification = "This is an obsolete API.")]
    public static Message FromMailtoUrl(string url) {
      return MailtoUris.MailtoUriMessage(url);
    }

    /// <summary>Generates a MailTo URI (uniform resource identifier)
    /// corresponding to this message. The following header fields, and
    /// only these, are used to generate the URI: To, Cc, Bcc, In-Reply-To,
    /// Subject, Keywords, Comments. The message body is included in the
    /// URI only if <c>GetBodyString()</c> would return a non-empty string.
    /// The To header field is included in the URI only if it has display
    /// names or group syntax.</summary>
    /// <returns>A MailTo URI corresponding to this message.</returns>
    [Obsolete("Renamed to ToMailtoUri.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1055",
      Justification = "This is an obsolete API.")]
    public string ToMailtoUrl() {
      return MailtoUris.MessageToMailtoUri(this);
    }

    /// <summary>Creates a message object from a MailTo URI (uniform
    /// resource identifier). The MailTo URI can contain key-value pairs
    /// that follow a question-mark, as in the following example:
    /// "mailto:me@example.com?subject=A%20Subject". In this example,
    /// "subject" is the subject of the email address. Only certain keys
    /// are supported, namely, "to", "cc", "bcc", "subject", "in-reply-to",
    /// "comments", "keywords", and "body". The first seven are header
    /// field names that will be used to set the returned message's
    /// corresponding header fields. The last, "body", sets the body of the
    /// message to the given text. Keys other than these eight will be
    /// ignored. (Keys are compared using a basic case-sensitive
    /// comparison, in which two strings are equal if they match after
    /// converting the basic upper-case letters A to Z (U+0041 to U+005A)
    /// in both strings to basic lower-case letters.) The same key (matched
    /// using a basic case-insensitive comparison) can appear more than
    /// once; for "subject", "cc", "bcc", and "in-reply-to", the last value
    /// with the given key is used; for "to", all header field values as
    /// well as the path are combined to a single To header field; for
    /// "keywords" and "comments", each value adds another header field of
    /// the given key; and for "body", the last value with that key is used
    /// as the body.</summary>
    /// <param name='uri'>The parameter <paramref name='uri'/> is a text
    /// string.</param>
    /// <returns>A Message object created from the given MailTo URI. Returs
    /// null if <paramref name='uri'/> is null, is syntactically invalid,
    /// or is not a MailTo URI.</returns>
    public static Message FromMailtoUri(string uri) {
      return MailtoUris.MailtoUriMessage(uri);
    }

    /// <summary>Creates a message object from a MailTo URI (uniform
    /// resource identifier) in the form of a URI object. For more
    /// information, see <b>FromMailtoUri(string)</b>.</summary>
    /// <param name='uri'>The MailTo URI in the form of a URI
    /// object.</param>
    /// <returns>A Message object created from the given MailTo URI. Returs
    /// null if <paramref name='uri'/> is null, is syntactically invalid,
    /// or is not a MailTo URI.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='uri'/> is null.</exception>
    public static Message FromMailtoUri(Uri uri) {
      if (uri == null) {
        throw new ArgumentNullException(nameof(uri));
      }
      return MailtoUris.MailtoUriMessage(uri.ToString());
    }

    /// <summary>Generates a MailTo URI (uniform resource identifier)
    /// corresponding to this message. The following header fields, and
    /// only these, are used to generate the URI: To, Cc, Bcc, In-Reply-To,
    /// Subject, Keywords, Comments. The message body is included in the
    /// URI only if <c>GetBodyString()</c> would return a non-empty
    /// string.. The To header field is included in the URI only if it has
    /// display names or group syntax.</summary>
    /// <returns>A MailTo URI corresponding to this message.</returns>
    public string ToMailtoUri() {
      return MailtoUris.MessageToMailtoUri(this);
    }

    private void ProcessHeaders(bool assumeMime, bool digest) {
      var haveContentType = false;
      bool mime = assumeMime;
      var haveContentDisp = false;
      string transferEncodingValue = String.Empty;
      for (int i = 0; i < this.headers.Count; i += 2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("content-transfer-encoding",
            StringComparison.Ordinal)) {
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
        mime |= name.Equals("mime-version", StringComparison.Ordinal);
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
        if (mime && name.Equals("content-transfer-encoding",
  StringComparison.Ordinal)) {
          value = DataUtilities.ToLowerCaseAscii(transferEncodingValue);
          this.headers[i + 1] = value;
          if (value.Equals("7bit", StringComparison.Ordinal)) {
            this.transferEncoding = EncodingSevenBit;
          } else if (value.Equals("8bit", StringComparison.Ordinal)) {
            this.transferEncoding = EncodingEightBit;
          } else if (value.Equals("binary", StringComparison.Ordinal)) {
            this.transferEncoding = EncodingBinary;
          } else if (value.Equals("quoted-printable",
  StringComparison.Ordinal)) {
            this.transferEncoding = EncodingQuotedPrintable;
          } else if (value.Equals("base64", StringComparison.Ordinal)) {
            this.transferEncoding = EncodingBase64;
          } else {
            // Unrecognized transfer encoding
            DebugUtility.Log("unrecognized: " + value);
            this.transferEncoding = EncodingUnknown;
          }
          haveContentEncoding = true;
        } else if (mime && name.Equals("content-type",
  StringComparison.Ordinal)) {
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
                // If we get here, then either:
                // - The charset is present but unrecognized or empty, or
                // - The charset is absent and the media type has
                // no default charset assumed for it.
                if (ctype.Parameters.ContainsKey("charset")) {
                  // The charset is present but unrecognized or
                  // empty; treat the content as application/octet-stream
                  // for conformance with RFC 2049.
                  ctype = MediaType.ApplicationOctetStream;
                } else if (!ctype.StoresCharsetInPayload()) {
                  // The charset is absent, and the media type:
                  // - has no default assumed for it, and
                  // - does not specify how the charset is determined
                  // from the payload.
                  // In this case, treat the body as being in an
                  // "unrecognized" charset, so as application/octet-stream
                  // for conformance with RFC 2049.
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
        } else if (mime && name.Equals("content-disposition",
  StringComparison.Ordinal)) {
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
      if (!haveContentEncoding && ctype.TypeAndSubType.Equals(
        "message/rfc822",
        StringComparison.Ordinal)) {
        // DEVIATION: Be a little more liberal with rfc822
        // messages with bytes greater than 127
        this.transferEncoding = EncodingEightBit;
      }
      if (this.transferEncoding == EncodingQuotedPrintable ||
          this.transferEncoding == EncodingBase64) {
        if (ctype.IsMultipart || (ctype.TopLevelType.Equals("message",
  StringComparison.Ordinal) && !ctype.SubType.Equals("global",
  StringComparison.Ordinal) && !ctype.SubType.Equals("global-headers",
  StringComparison.Ordinal) && !ctype.SubType.Equals(
    "global-disposition-notification",
    StringComparison.Ordinal) && !ctype.SubType.Equals("global-delivery-status",
  StringComparison.Ordinal))) {
            // CLARIFICATION: Treat quoted-printable and base64
            // as "unrecognized" encodings in multipart and most
            // message media types, for the purpose of treating the
            // content type as "application/octet-stream".
            ctype = MediaType.ApplicationOctetStream;
        }
      }
      // Update content type as appropriate
      // NOTE: Setting the field, not the setter,
      // because it's undesirable here to add a Content-Type
      // header field, as the setter does
      this.contentType = ctype;
      if (this.transferEncoding == EncodingSevenBit) {
        string charset = this.contentType.GetCharset();
        if (charset.Equals("utf-8", StringComparison.Ordinal)) {
          // DEVIATION: Be a little more liberal with UTF-8
          this.transferEncoding = EncodingEightBit;
        } else if (ctype.TypeAndSubType.Equals("text/html",
            StringComparison.Ordinal)) {
          if (charset.Equals("us-ascii", StringComparison.Ordinal) ||
              charset.Equals("windows-1252", StringComparison.Ordinal) ||
              charset.Equals("windows-1251", StringComparison.Ordinal) ||
              (charset.Length > 9 && charset.Substring(0, 9).Equals(
                "iso-8859-",
                StringComparison.Ordinal))) {
            // DEVIATION: Be a little more liberal with text/html and
            // single-byte charsets or UTF-8
            this.transferEncoding = EncodingEightBit;
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
              Message parentMessage = multipartStack[
                    multipartStack.Count - 1].Message;
              boundaryChecker.StartBodyPartHeaders();
              MediaType ctype = parentMessage.ContentType;
              bool parentIsDigest = ctype.SubType.Equals("digest",
  StringComparison.Ordinal) && ctype.IsMultipart;
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
              bool isTextPlain = ctype.TypeAndSubType.Equals(
                "text/plain",
                StringComparison.Ordinal);
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                isTextPlain);
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
      bool isTextPlain =
this.ContentType.TypeAndSubType.Equals("text/plain",
  StringComparison.Ordinal);
      IByteReader transform = MakeTransferEncoding(
        stream,
        this.transferEncoding,
        isTextPlain);
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
      var encoder = new HeaderEncoder(76, 0).AppendFieldName(name);
      string fullField = ParserUtility.Implode(
        this.GetMultipleHeaders(name),
        "\u002c ");
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
      /// <summary>Gets a value which is used in an internal API.</summary>
      /// <value>This is an internal API.</value>
      public Message Message { get; private set; }

      /// <summary>Gets a value which is used in an internal API.</summary>
      /// <value>This is an internal API.</value>
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
