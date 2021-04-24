package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;
import java.io.*;

import com.upokecenter.util.*;
import com.upokecenter.mail.transforms.*;
import com.upokecenter.text.*;

  /**
   * Represents an email message, and contains methods and properties for
   * accessing and modifying email message data. This class implements the
   * Internet Message Format (RFC 5322) and Multipurpose Internet Mail
   * Extensions (MIME; RFC 2045-2047, RFC 2049).<p> <p><b>Thread
   * safety:</b> This class is mutable; its properties can be changed. None
   * of its instance methods are designed to be thread safe. Therefore,
   * access to objects from this class must be synchronized if multiple
   * threads can access them at the same time.</p> <p>The following lists
   * known deviations from the mail specifications (Internet Message Format
   * and MIME):</p> <ul> <li>If a message has two or more Content-Type
   * header fields, it is treated as having a content type of
   *  "application/octet-stream", unless one or more of the header fields is
   * syntactically invalid.</li> <li>Illegal UTF-8 byte sequences appearing
   * in header field values are replaced with replacement characters.
   * Moreover, UTF-8 is parsed everywhere in header field values, even in
   * those parts of some structured header fields where this appears not to
   * be allowed. (UTF-8 is a character encoding for the Unicode character
   * set.)</li> <li>This implementation can parse a message even if that
   * message is without a From header field, without a Date header field,
   * or without both.</li> <li>The To and Cc header fields are allowed to
   *  contain only comments and whitespace, but these "empty" header fields
   * will be omitted when generating.</li> <li>There is no line length
   * limit imposed when parsing header fields, except header field
   * names.</li> <li>There is no line length limit imposed when parsing
   * quoted-printable or base64 encoded bodies.</li> <li>If the transfer
   *  encoding is absent and the content type is "message/rfc822", bytes
   * with values greater than 127 are still allowed, despite the default
   *  value of "7bit" for "Content-Transfer-Encoding".</li> <li>In the
   * following cases, if the transfer encoding is absent, declared as 7bit,
   * or treated as 7bit, bytes greater than 127 are still allowed:</li>
   * <li>(a) The preamble and epilogue of multipart messages, which will be
   * ignored.</li> <li>(b) If the charset is declared to be
   *  <code>utf-8</code>.</li> <li>(c) If the content type is "text/html" and the
   *  charset is declared to be <code>us-ascii</code>, "windows-1252",
   *  "windows-1251", or "iso-8859-*" (all single byte encodings).</li>
   * <li>(d) In text/plain message bodies. Any bytes greater than 127 are
   * replaced with the substitute character byte (0x1a).</li> <li>(e) In
   * MIME message bodies (this is not a deviation from MIME, though). Any
   * bytes greater than 127 are replaced with the substitute character byte
   *  (0x1a).</li> <li>If the message starts with the word "From" (and no
   * other case variations of that word) followed by one or more space
   * (U+0020) not followed by colon, that text and the rest of the text is
   * skipped up to and including a line feed (U+000A). (See also RFC 4155,
   *  which describes the so-called "mbox" convention with "From" lines of
   * this kind.)</li> <li>The name <code>ascii</code> is treated as a synonym for
   * <code>us-ascii</code>, despite being a reserved name under RFC 2046. The
   * names <code>cp1252</code> and <code>utf8</code> are treated as synonyms for
   * <code>windows-1252</code> and <code>utf-8</code>, respectively, even though they
   * are not IANA registered aliases.</li> <li>The following deviations
   * involve encoded words under RFC 2047:</li> <li>(a) If a sequence of
   * encoded words decodes to a string with a CTL character (U+007F, or a
   * character less than U+0020 and not TAB) after being converted to
   * Unicode, the encoded words are left un-decoded.</li> <li>(b) This
   * implementation can decode encoded words regardless of the character
   * length of the line in which they appear. This implementation can
   * generate a header field line with one or more encoded words even if
   * that line is more than 76 characters long. (This implementation
   * follows the recommendation in RFC 5322 to limit header field lines to
   * no more than 78 characters, where possible; see also RFC
   * 6532.)</li></ul> <p>It would be appreciated if users of this library
   * contact the author if they find other ways in which this
   * implementation deviates from the mail specifications or other
   * applicable specifications.</p> <p>This class currently doesn't support
   *  the "padding" parameter for message bodies with the media type
   *  "application/octet-stream" or treated as that media type (see RFC 2046
   * sec. 4.5.1).</p> <p>In this implementation, if the
   *  content-transfer-encoding "quoted-printable" or "base64" occurs in a
   *  message or body part with content type "multipart/*" or "message/*"
   *  (other than "message/global", "message/global-headers",
   *  "message/global-disposition-notification", or
   *  "message/global-delivery-status"), that encoding is treated as
   * unrecognized for the purpose of treating that message or body part as
   *  having a content type of "application/octet-stream" rather than the
   * declared content type. This is a clarification to RFCs 2045 and 2049.
   *  (This may result in "misdecoded" messages because in practice, most if
   * not all messages of this kind don't use quoted-printable or base64
   * encodings for the whole body, but may do so in the body parts they
   * contain.)</p> <p>This implementation can decode an RFC 2047 encoded
   * word that uses ISO-2022-JP or ISO-2022-JP-2 (encodings that use code
   * switching) even if the encoded word's payload ends in a different mode
   *  from "ASCII mode". (Each encoded word still starts in "ASCII mode",
   * though.) This, however, is not a deviation to RFC 2047 because the
   *  relevant rule only concerns bringing the output device back to "ASCII
   *  mode" after the decoded text is displayed (see last paragraph of sec.
   * 6.2) -- since the decoded text is converted to Unicode rather than
   * kept as ISO-2022-JP or ISO-2022-JP-2, this is not applicable since
   *  there is no such thing as "ASCII mode" in the Unicode Standard.</p>
   * <p>Note that this library (the MailLib library) has no facilities for
   * sending and receiving email messages, since that's outside this
   * library's scope.</p></p>
   */
  public final class Message {
    // Recomm. max. number of CHARACTERS per line (excluding CRLF)
    // (see RFC 5322, 6532)
    static final int MaxRecHeaderLineLength = 78;
    static final int MaxShortHeaderLineLength = 76;
    // Max. number of OCTETS per line (excluding CRLF)
    // (see RFC 5322, 6532)
    static final int MaxHardHeaderLineLength = 998;

    private static final int EncodingBase64 = 2;
    private static final int EncodingBinary = 4;
    private static final int EncodingEightBit = 3;
    private static final int EncodingQuotedPrintable = 1;
    private static final int EncodingSevenBit = 0;
    private static final int EncodingUnknown = -1;

    // NOTE: System.java.util.Random is not a cryptographic RNG.
    // If security is a concern, replace this call to System.java.util.getRandom()
    // to the interface of a cryptographic RNG.
    private static final java.util.Random ValueMsgidRandom = new java.util.Random();
    private static final Object ValueSequenceSync = new Object();

    private static final Map<String, Integer> ValueHeaderIndices =
      MakeHeaderIndices();

    private final List<String> headers;

    private final List<Message> parts;

    private static int msgidSequence;
    private static boolean seqFirstTime = true;
    private byte[] body;
    private ContentDisposition contentDisposition;

    private MediaType contentType;

    private int transferEncoding;

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.Message}
     * class. Reads from the given InputStream object to initialize the email
     * message.<p><b>Remarks:</b> <p>This constructor parses an email
     * message, and extracts its header fields and body, and throws a
     * MessageDataException if the message is malformed. However, even if a
     * MessageDataException is thrown, it can still be possible to display
     * the message, especially because most email malformations seen in
     * practice are benign in nature (such as the use of very long lines in
     * the message). One way an application can handle the exception is to
     * read all the bytes from the stream, to display the message, or part
     * of it, as raw text (using <code>DataUtilities.GetUtf8String(bytes,
     * true)</code>), and to optionally extract important header fields, such
     * as From, To, Date, and Subject, from the message's text using the
     * <code>ExtractHeader</code> method. Even so, though, any message for which
     * this constructor throws a MessageDataException ought to be treated
     * with suspicion.</p></p>
     * @param stream A readable data stream.
     * @throws NullPointerException The parameter {@code stream} is null.
     * @throws com.upokecenter.mail.MessageDataException The message is malformed.
     * See the remarks.
     */
    public Message(InputStream stream) {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      this.headers = new ArrayList<String>();
      this.parts = new ArrayList<Message>();
      this.body = new byte[0];
      IByteReader transform = DataIO.ToReader(stream);
      this.ReadMessage(transform);
    }

    private static int SkipCaseString(byte[] bytes, int index, String value) {
      // NOTE: assumes value is all-ASCII
      int start = index;
      for (int i = 0; i < value.length(); ++i) {
        int b = ((int)bytes[index + i]) & 0xff;
        int c = (int)value.charAt(i);
        if (b >= 0x41 && b <= 0x5a) {
          b += 0x20;
        }
        if (c >= 0x41 && c <= 0x5a) {
          c += 0x20;
        }
        if (index + 1 >= bytes.length || b != c) {
          return start;
        }
      }
      return index + value.length();
    }

    private static int EndOfLine(byte[] bytes, int index) {
      return (index >= 2 && bytes[index - 1] == 0x0a && bytes[index - 2] ==
          0x0d) ? (index - 2) : index;
    }

    private static int SkipLine(byte[] bytes, int index) {
      while (index < bytes.length) {
        if (bytes[index] == 0x0d && index + 1 < bytes.length && bytes[index +
            1] == 0x0a) {
          return index + 2;
        }
        ++index;
      }
      return index;
    }

    private static int SkipWsp(byte[] bytes, int index) {
      while (index < bytes.length) {
        if (bytes[index] != 0x09 && bytes[index] != 0x20) {
          return index;
        }
        ++index;
      }
      return index;
    }

    /**
     * Extracts the value of a header field from a byte array representing an email
     * message. The return value is intended for display purposes, not for
     * further processing, and this method is intended to be used as an
     * error handling tool for email messages that are slightly malformed.
     * (Note that malformed email messages ought to be treated with greater
     * suspicion than well-formed email messages.).
     * @param bytes A byte array representing an email message.
     * @param headerFieldName The name of the header field to extract. This name
     * will be compared with the names of header fields in the given
     * message using a basic case-insensitive comparison. (Two strings are
     * equal in such a comparison, if they match after converting the basic
     * upper-case letters A to Z (U+0041 to U+005A) in both strings to
     * basic lower-case letters.).
     * @return The value of the first instance of the header field with the given
     * name. Leading space and/or tab bytes (0x20 and/or 0x09) and CR/LF
     * (0x0d/0x0a) pairs will be removed from the header field value, and
     * the value is treated as encoded in UTF-8 (an 8-bit encoding form of
     * the Unicode Standard) where illegally encoded UTF-8 is replaced as
     * appropriate with replacement characters (U+FFFD). Returns null if
     * {@code bytes} is null, if {@code headerFieldName} is null, is more
     * than 997 characters long, or has a character less than U+0021 or
     * greater than U+007E in the Unicode Standard, if a header field with
     * that name does not exist, or if a body (even an empty one) does not
     * follow the header fields.
     */
    public static String ExtractHeader(byte[] bytes, String headerFieldName) {
      if (bytes == null) {
        return null;
      }
      if (((headerFieldName) == null || (headerFieldName).length() == 0) ||
        headerFieldName.length() > 997) {
        return null;
      }
      for (int i = 0; i < headerFieldName.length(); ++i) {
        if (headerFieldName.charAt(i) >= 0x7f || headerFieldName.charAt(i) <= 0x20 ||
          headerFieldName.charAt(i) == ':') {
          break;
        }
      }
      int index = 0;
      String ret = null;
      while (index < bytes.length) {
        if (index + 1 < bytes.length && bytes[index] == 0x0d &&
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
        if (n >= bytes.length || bytes[n] != ':') {
          // Not the desired header field
          index = SkipLine(bytes, index);
          continue;
        }
        n = SkipWsp(bytes, n + 1);
        {
          java.io.ByteArrayOutputStream ms = null;
try {
ms = new java.io.ByteArrayOutputStream();

          int endLine = SkipLine(bytes, n);
          ms.write(bytes, n, EndOfLine(bytes, endLine) - n);
          index = endLine;
          while (endLine < bytes.length &&
            (bytes[endLine] == 0x09 || bytes[endLine] == 0x20)) {
            int s = endLine;
            endLine = SkipLine(bytes, endLine);
            index = endLine;
            ms.write(bytes, s, EndOfLine(bytes, endLine) - s);
          }
          ret = DataUtilities.GetUtf8String(ms.toByteArray(), true);
}
finally {
try { if (ms != null) { ms.close(); } } catch (java.io.IOException ex) {}
}
}
      }
      return null;
    }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.Message}
     * class. Reads from the given byte array to initialize the email
     * message.<p><b>Remarks:</b> <p>This constructor parses an email
     * message, and extracts its header fields and body, and throws a
     * MessageDataException if the message is malformed. However, even if a
     * MessageDataException is thrown, it can still be possible to display
     * the message, especially because most email malformations seen in
     * practice are benign in nature (such as the use of very long lines in
     * the message). One way an application can handle the exception is to
     * display the message, or part of it, as raw text (using
     * <code>DataUtilities.GetUtf8String(bytes, true)</code>), and to optionally
     * extract important header fields, such as From, To, Date, and
     * Subject, from the message's text using the <code>ExtractHeader</code>
     * method. Even so, though, any message for which this constructor
     * throws a MessageDataException ought to be treated with
     * suspicion.</p></p>
     * @param bytes A readable data stream.
     * @throws NullPointerException The parameter {@code bytes} is null.
     * @throws com.upokecenter.mail.MessageDataException The message is malformed.
     * See the remarks.
     */
    public Message(byte[] bytes) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      this.headers = new ArrayList<String>();
      this.parts = new ArrayList<Message>();
      this.body = new byte[0];
      IByteReader transform = DataIO.ToReader(bytes);
      this.ReadMessage(transform);
    }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.Message}
     * class. The message will be plain text and have an artificial From
     * address.
     */
    public Message() {
      this.headers = new ArrayList<String>();
      this.parts = new ArrayList<Message>();
      this.body = new byte[0];
      this.contentType = MediaType.TextPlainAscii;
      this.headers.add("message-id");
      this.headers.add(this.GenerateMessageID());
      this.headers.add("from");
      this.headers.add("me@from-address.invalid");
      this.headers.add("mime-version");
      this.headers.add("1.0");
    }

    /**
     * Creates a message object with no header fields.
     * @return A message object with no header fields.
     */
    public static Message NewBodyPart() {
      Message msg = new Message();
      msg.contentType = MediaType.TextPlainAscii;
      // No headers by default (see RFC 2046 sec. 5.1)
      msg.headers.clear();
      return msg;
    }

    /**
     * Sets this message's Date header field to the current time as its value, with
     * an unspecified time zone offset. <p>This method can be used when the
     * message is considered complete and ready to be generated, for
     *  example, using the "Generate()" method.</p>
     * @return This object.
     */
    public Message SetCurrentDate() {
      // NOTE: Use global rather than local time; there are overriding
      // reasons not to use local time, despite the SHOULD in RFC 5322
      return this.SetDate(DateTimeUtilities.GetCurrentGlobalTime());
    }

    private static void ReverseChars(char[] chars, int offset, int length) {
      int half = length >> 1;
      int right = offset + length - 1;
      for (int i = 0; i < half; i++, right--) {
        char value = chars[offset + i];
        chars[offset + i] = chars[right];
        chars[right] = value;
      }
    }

    /**
     * Gets a list of addresses found in the BCC header field or fields.
     * @return A list of addresses found in the BCC header field or fields.
     * @deprecated Use GetAddresses(\Bcc\) instead.
 */
@Deprecated
    public final List<NamedAddress> getBccAddresses() {
        return this.GetAddresses("bcc");
      }

    /**
     * Gets the body of this message as a text string. See the
     * <code>GetBodyString()</code> method.
     * @return The body of this message as a text string.
     * @throws UnsupportedOperationException See the {@code GetBodyString()} method.
     * @deprecated Use GetBodyString() instead.
 */
@Deprecated

    public final String getBodyString() {
        return this.GetBodyString();
      }

    private static ICharacterEncoding GetEncoding(String charset) {
      ICharacterEncoding enc = Encodings.GetEncoding(
        charset,
        true);
      if (enc == null) {
        if (charset.equals("gb2312")) {
          // HACK
          enc = Encodings.GetEncoding("gb2312", false);
        } else {
          return null;
        }
      }
      return enc;
    }

    private static boolean DefinesCharsetParameter(MediaType mt) {
      // All media types that specify a charset parameter, either as a
      // required or an optional parameter.
      // NOTE: Up-to-date as of December 16, 2019
      if (mt.HasStructuredSuffix("xml") ||
        mt.getTopLevelType().equals("text") ||
        mt.getTypeAndSubType().equals("image/vnd.wap.wbmp")) {
        return true;
      }
      if (mt.getTopLevelType().equals("application")) {
        return mt.getSubType().equals("vnd.uplanet.alert-wbxml") ||
          mt.getSubType().equals("vnd.wap.wmlscriptc") ||
          mt.getSubType().equals("xml-dtd") ||
          mt.getSubType().equals("vnd.picsel") ||
          mt.getSubType().equals("news-groupinfo") ||
          mt.getSubType().equals("ecmascript") ||
          mt.getSubType().equals("vnd.uplanet.cacheop-wbxml") || mt.getSubType().equals(
              "vnd.uplanet.bearer-choice") ||
          mt.getSubType().equals("vnd.wap.slc") ||
          mt.getSubType().equals("nss") ||
          mt.getSubType().equals("vnd.3gpp.mcdata-payload") ||
          mt.getSubType().equals("activity+json") ||
          mt.getSubType().equals("vnd.uplanet.list-wbxml") || mt.getSubType().equals("vnd.3gpp.mcdata-signalling") ||
          mt.getSubType().equals("sgml-open-catalog") ||
          mt.getSubType().equals("smil") ||
          mt.getSubType().equals("vnd.uplanet.channel") ||
          mt.getSubType().equals("javascript") ||
          mt.getSubType().equals("vnd.syncml.dm+wbxml") ||
          mt.getSubType().equals("vnd.ah-barcode") ||
          mt.getSubType().equals("vnd.uplanet.alert") ||
          mt.getSubType().equals("vnd.wap.wbxml") ||
          mt.getSubType().equals("xml-external-parsed-entity") || mt.getSubType().equals(
              "vnd.uplanet.listcmd-wbxml") ||
          mt.getSubType().equals("vnd.uplanet.list") ||
          mt.getSubType().equals("vnd.uplanet.listcmd") ||
          mt.getSubType().equals("vnd.msign") ||
          mt.getSubType().equals("news-checkgroups") ||
          mt.getSubType().equals("fhir+json") ||
          mt.getSubType().equals("set-registration") ||
          mt.getSubType().equals("sql") ||
          mt.getSubType().equals("vnd.wap.sic") ||
          mt.getSubType().equals("prs.alvestrand.titrax-sheet") ||
          mt.getSubType().equals("vnd.uplanet.bearer-choice-wbxml") ||
          mt.getSubType().equals("vnd.wap.wmlc") ||
          mt.getSubType().equals("vnd.uplanet.channel-wbxml") ||
          mt.getSubType().equals("iotp") ||
          mt.getSubType().equals("vnd.uplanet.cacheop") ||
          mt.getSubType().equals("xml") ||
          mt.getSubType().equals("vnd.adobe.xfdf") ||
          mt.getSubType().equals("vnd.dpgraph");
      }
      return false;
    }

    private static String InputToStringWithHint(
          ICharacterInput reader,
          int capacityHint) {
      StringBuilder builder = new StringBuilder(capacityHint);
      while (true) {
        if (reader == null) {
          throw new NullPointerException("reader");
        }
        int c = reader.ReadChar();
        if (c < 0) {
          break;
        }
        if (c <= 0xffff) {
          builder.append((char)c);
        } else if (c <= 0x10ffff) {
          builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
          builder.append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
        }
      }
      return builder.toString();
    }

    private void GetBodyStrings(
      List<String> bodyStrings,
      List<MediaType> mediaTypes) {
      if (this.getContentDisposition() != null &&
        !this.getContentDisposition().isInline()) {
        // Content-Disposition is present and other than inline; ignore.
        // See also RFC 2183 sec. 2.8 and 2.9.
        return;
      }
      MediaType mt = this.getContentType();
      if (mt.isMultipart()) {
        List<Message> parts = this.getParts();
        if (mt.getSubType().equals("alternative")) {
          // Navigate the parts in reverse order
          int i = parts.size() - 1;
          for (; i >= 0; --i) {
            int oldCount = bodyStrings.size();
            parts.get(i).GetBodyStrings(bodyStrings, mediaTypes);
            if (oldCount != bodyStrings.size()) {
              break;
            }
          }
        } else {
          // Any other multipart
          for (int i = 0; i < parts.size(); ++i) {
            parts.get(i).GetBodyStrings(bodyStrings, mediaTypes);
          }
        }
      }
      if (!DefinesCharsetParameter(this.getContentType())) {
        // Nontext and no charset parameter defined
        return;
      }
      String charsetName = this.getContentType().GetCharset();
      ICharacterEncoding charset = GetEncoding(charsetName);
      if (charset == null && this.getContentType().getTypeAndSubType().equals(
        "text/html")) {
        charsetName = GuessHtmlEncoding(this.body);
        charset = Encodings.GetEncoding(charsetName);
      }
      if (charset != null) {
        int capacityHint = this.body.length < Integer.MAX_VALUE / 2 ?
            Math.min(this.body.length * 2, this.body.length + 32) :
            this.body.length;
        ICharacterInput cinput = Encodings.GetDecoderInput(
            charset,
            DataIO.ToReader(this.body));
        bodyStrings.add(InputToStringWithHint(
            cinput,
            capacityHint));
        mediaTypes.add(this.getContentType());
      }
    }

    private String GetBodyStringNoThrow() {
      List<String> bodyStrings = new ArrayList<String>();
      List<MediaType> mediaTypes = new ArrayList<MediaType>();
      this.GetBodyStrings(
        bodyStrings,
        mediaTypes);
      if (bodyStrings.size() > 0) {
        return bodyStrings.get(0);
      } else {
        return null;
      }
    }

    private void AccumulateAttachments(
      List<Message> attachments,
      boolean root) {
      if (this.getContentDisposition() != null &&
        !this.getContentDisposition().isInline() && !root) {
        attachments.add(this);
        return;
      }
      MediaType mt = this.getContentType();
      if (mt.getSubType().equals("alternative")) {
        // Navigate the parts in reverse order
        int i = this.parts.size() - 1;
        for (; i >= 0; --i) {
          if (this.GetBodyStringNoThrow() != null) {
            this.parts.get(i).AccumulateAttachments(attachments, false);
            break;
          }
        }
      } else {
        // Any other multipart
        for (int i = 0; i < this.parts.size(); ++i) {
          this.parts.get(i).AccumulateAttachments(attachments, false);
        }
      }
    }

    /**
     * Gets a list of descendant body parts of this message that are considered
     * attachments. An <i>attachment</i> is a body part or descendant body
     * part that has a content disposition with a type other than inline.
     * This message itself is not included in the list even if it's an
     * attachment as just defined.
     * @return A list of descendant body parts of this message that are considered
     * attachments.
     */
    public List<Message> GetAttachments() {
      ArrayList<Message> list = new ArrayList<Message>();
      this.AccumulateAttachments(
        list,
        true);
      return list;
    }

    /**
     * Gets the body of this message as a text string. If this message's media type
     *  is "multipart/alternative", returns the result of this method for
     *  the last supported body part. For any other "multipart" media type,
     * returns the result of this method for the first body part for which
     * this method returns a text string.
     * @return The body of this message as a text string.
     * @throws UnsupportedOperationException This message is a multipart message without a
     * supported body part; or this message has a content disposition with
     *  a type other than "inline"; or this message's media type is a
     *  non-multipart type and does not specify the use of a "charset"
     * parameter, has no character encoding declared or assumed for it
     * (which is usually the case for non-text messages), or has an
     * unsupported character encoding.
     */

    public String GetBodyString() {
      // TODO: Consider returning null rather than throwing an exception
      // in public API
      String str = this.GetBodyStringNoThrow();
      if (str == null) {
        throw new UnsupportedOperationException("No supported text to show");
      }
      return str;
    }

    /**
     * Gets a list of addresses found in the CC header field or fields.
     * @return A list of addresses found in the CC header field or fields.
     * @deprecated Use GetAddresses(\Cc\) instead.
 */
@Deprecated
    public final List<NamedAddress> getCCAddresses() {
        return this.GetAddresses("cc");
      }

    static String ExtractCharsetFromMeta(String value) {
      if (value == null) {
        return value;
      }
      // We assume value is lower-case here
      int index = 0;
      int length = value.length();
      char c = (char)0;
      while (true) {
        index = value.indexOf("charset",0);
        if (index < 0) {
          return null;
        }
        index += 7;
        // skip whitespace
        while (index < length) {
          c = value.charAt(index);
          if (c != 0x09 && c != 0x0c && c != 0x0d && c != 0x0a && c != 0x20) {
            break;
          }
          ++index;
        }
        if (index >= length) {
          return null;
        }
        if (value.charAt(index) == '=') {
          ++index;
          break;
        }
      }
      // skip whitespace
      while (index < length) {
        c = value.charAt(index);
        if (c != 0x09 && c != 0x0c && c != 0x0d && c != 0x0a && c != 0x20) {
          break;
        }
        ++index;
      }
      if (index >= length) {
        return null;
      }
      c = value.charAt(index);
      if (c == '"' || c == '\'') {
        ++index;
        int nextIndex = index;
        while (nextIndex < length) {
          char c2 = value.charAt(nextIndex);
          if (c == c2) {
            return Encodings.ResolveAlias(
                value.substring(
                  index, (
                  index)+(nextIndex - index)));
          }
          ++nextIndex;
        }
        return null;
      } else {
        int nextIndex = index;
        while (nextIndex < length) {
          char c2 = value.charAt(nextIndex);
          if (
            c2 == 0x09 || c2 == 0x0c || c2 == 0x0d || c2 == 0x0a || c2 ==
            0x20 || c2 == 0x3b) {
            break;
          }
          ++nextIndex;
        }
        return
          Encodings.ResolveAlias(value.substring(index, (index)+(nextIndex - index)));
      }
    }

    private static int ReadAttribute(
      byte[] data,
      int length,
      int position,
      StringBuilder attrName,
      StringBuilder attrValue) {
      if (attrName != null) {
        attrName.delete(0, attrName.length());
      }
      if (attrValue != null) {
        attrValue.delete(0, attrValue.length());
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
      boolean empty = true;
      boolean tovalue = false;
      int b = 0;
      // Skip attribute name
      while (true) {
        if (position >= length) {
          // end of stream reached, so clear
          // the attribute name to indicate failure
          if (attrName != null) {
            attrName.delete(0, attrName.length());
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
              attrName.append((char)(b + 0x20));
            } else {
              attrName.append((char)b);
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
            attrName.delete(0, attrName.length());
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
          attrName.delete(0, attrName.length());
        }
        return position;
      }
      b = data[position] & 0xff;
      if (b == 0x22 || b == 0x27) { // have quoted String
        ++position;
        while (true) {
          if (position >= length) {
            // end of stream reached, so clear
            // the attribute name and value to indicate failure
            if (attrName != null) {
              attrName.delete(0, attrName.length());
            }
            if (attrValue != null) {
              attrValue.delete(0, attrValue.length());
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
              attrValue.append((char)(b2 + 0x20));
            } else {
              attrValue.append((char)b2);
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
            attrValue.append((char)(b + 0x20));
          } else {
            attrValue.append((char)b);
          }
        }
        ++position;
      }
      while (true) {
        if (position >= length) {
          // end of stream reached, so clear
          // the attribute name and value to indicate failure
          if (attrName != null) {
            attrName.delete(0, attrName.length());
          }
          if (attrValue != null) {
            attrValue.delete(0, attrValue.length());
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
            attrValue.append((char)(b + 0x20));
          } else {
            attrValue.append((char)b);
          }
        }
        ++position;
      }
    }
    // NOTE: To be used when the encoding is not otherwise provided
    // by the transport layer (e.g., Content-Type) and the transport
    // layer's encoding is not overridden by the end user
    private static String GuessHtmlEncoding(byte[] data) {
      int b = 0;
      int count = Math.min(data.length, 1024);
      int position = 0;
      while (position < count) {
        if (position + 4 <= count && data[position + 0] == 0x3c &&
          (data[position + 1] & 0xff) == 0x21 &&
          (data[position + 2] & 0xff) == 0x2d &&
          (data[position + 3] & 0xff) == 0x2d) {
          // Skip comment
          int hyphenCount = 2;
          position += 4;
          while (position < count) {
            int c = data[position] & 0xff;
            if (c == '-') {
              hyphenCount = Math.min(2, hyphenCount + 1);
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
          boolean haveHttpEquiv = false;
          boolean haveContent = false;
          boolean haveCharset = false;
          boolean gotPragma = false;
          int needPragma = 0; // need pragma null
          String charset = null;
          StringBuilder attrName = new StringBuilder();
          StringBuilder attrValue = new StringBuilder();
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
            String attrNameString = attrName.toString();
            if (!haveHttpEquiv && attrNameString.equals("http-equiv")) {
              haveHttpEquiv = true;
              if (attrValue.toString().equals("content-type")) {
                gotPragma = true;
              }
            } else if (!haveContent && attrNameString.equals("content")) {
              haveContent = true;
              if (charset == null) {
                String newCharset =
                  ExtractCharsetFromMeta(attrValue.toString());
                if (newCharset != null) {
                  charset = newCharset;
                  needPragma = 2; // need pragma true
                }
              }
            } else if (!haveCharset && attrNameString.equals("charset")) {
              haveCharset = true;
              charset = Encodings.ResolveAlias(attrValue.toString());
              needPragma = 1; // need pragma false
            }
            position = newpos;
          }
          if (needPragma == 0 || (needPragma == 2 && !gotPragma) || charset ==
            null) {
            ++position;
          } else {
            if ("utf-16le".equals(charset) ||
              "utf-16be".equals(charset)) {
              charset = "utf-8";
            }
            return charset;
          }
        } else if ((position + 3 <= count &&
            data[position] == 0x3c && (data[position + 1] & 0xff) == 0x2f &&
            (((data[position + 2] & 0xff) >= 0x41 && (data[position + 2] &
0xff) <=
                0x5a) ||
              ((data[position + 2] & 0xff) >= 0x61 && (data[position + 2] &
0xff) <=
                0x7a))) ||
          // </X
          (position + 2 <= count && data[position] == 0x3c &&
            (((data[position + 1] & 0xff) >= 0x41 && (data[position + 1] & 0xff)
                <= 0x5a) ||
              ((data[position + 1] & 0xff) >= 0x61 && (data[position + 1] &
0xff)
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
      int byteIndex = 0;
      int b1 = byteIndex >= data.length ? -1 : ((int)data[byteIndex++]) & 0xff;
      int b2 = byteIndex >= data.length ? -1 : ((int)data[byteIndex++]) & 0xff;
      if (b1 == 0xfe && b2 == 0xff) {
        return "utf-16be";
      }
      if (b1 == 0xff && b2 == 0xfe) {
        return "utf-16le";
      }
      int b3 = byteIndex >= data.length ? -1 : ((int)data[byteIndex++]) &
        0xff;
      if (b1 == 0xef && b2 == 0xbb && b3 == 0xbf) {
        return "utf-8";
      }
      byteIndex = 0;
      int maybeUtf8 = 0;
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
          // System.out.println("%02X %02X",data[position],data[position+1]);
          position += 2;
          maybeUtf8 = 1;
        } else if (position + 3 <= count && (b >= 0xe0 && b <= 0xef) &&
          ((data[position + 2] & 0xff) >= 0x80 && (data[position + 2] & 0xff) <=
            0xbf)) {
          int startbyte = (b == 0xe0) ? 0xa0 : 0x80;
          int endbyte = (b == 0xed) ? 0x9f : 0xbf;
          // System.out.println("%02X %02X %02X"
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
          // System.out.println("%02X %02X %02X %02X"
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

    /**
     * <p>Gets a Hypertext Markup Language (HTML) rendering of this message's text
     * body. This method currently supports any message for which
     * <code>GetBodyString()</code> outputs a text string and treats the
     * following media types specially: text/plain with
     * <code>format = flowed</code>, text/enriched, text/markdown (original
     * Markdown).</p><p> <p>REMARK: The Markdown implementation currently
     * supports all features of original Markdown, except that the
     * implementation:</p> <ul> <li>does not strictly check the placement
     *  of "block-level HTML elements",</li> <li>does not prevent Markdown
     * content from being interpreted as such merely because it's contained
     *  in a "block-level HTML element", and</li> <li>does not deliberately
     * use HTML escapes to obfuscate email addresses wrapped in
     * angle-brackets.</li></ul></p>
     * @return An HTML rendering of this message's text.
     * @throws UnsupportedOperationException No supported body part was found; see {@code
     * GetBodyString()} for more information.
     */
    public String GetFormattedBodyString() {
      // TODO: Consider returning null rather than throwing an exception
      // in public API
      String text = this.GetFormattedBodyStringNoThrow();
      if (text == null) {
        throw new UnsupportedOperationException();
      }
      return text;
    }

    private String GetFormattedBodyStringNoThrow() {
      ArrayList<String> bodyStrings = new ArrayList<String>();
      ArrayList<MediaType> mediaTypes = new ArrayList<MediaType>();
      this.GetBodyStrings(
        bodyStrings,
        mediaTypes);
      if (bodyStrings.size() == 0) {
        return null;
      }
      String text = bodyStrings.get(0);
      MediaType mt = mediaTypes.get(0);
      String fmt = mt.GetParameter("format");
      String dsp = mt.GetParameter("delsp");
      boolean formatFlowed = DataUtilities.ToLowerCaseAscii(
          fmt == null ? "fixed" : fmt)
        .equals("flowed");
      boolean delSp = DataUtilities.ToLowerCaseAscii(
          dsp == null ? "no" : dsp).equals("yes");
      if (mt.getTypeAndSubType().equals("text/plain")) {
        if (formatFlowed) {
          return FormatFlowed.FormatFlowedText(text, delSp);
        } else {
          return FormatFlowed.NonFormatFlowedText(text);
        }
      } else if (mt.getTypeAndSubType().equals("text/html")) {
        return text;
      } else if (mt.getTypeAndSubType().equals("text/markdown")) {
        MediaType previewType = MediaType.Parse("text/html");
        if (this.getContentDisposition() != null) {
          String pt = this.getContentDisposition().GetParameter("preview-type");
          previewType = MediaType.Parse(
            pt == null ? "" : pt,
            previewType);
        }
        if (previewType.getTypeAndSubType().equals("text/html")) {
          return FormatFlowed.MarkdownText(text, 0);
        } else {
          return FormatFlowed.NonFormatFlowedText(text);
        }
      } else if (mt.getTypeAndSubType().equals("text/enriched")) {
        return EnrichedText.EnrichedToHtml(text, 0, text.length());
      } else {
        return FormatFlowed.NonFormatFlowedText(text);
      }
    }

    /**
     * Gets this message's content disposition. The content disposition specifies
     * how a user agent should display or otherwise handle this message.
     * Can be set to null. If set to a disposition or to null, updates the
     * Content-Disposition header field as appropriate. (There is no
     * default content disposition if this value is null, and disposition
     * types unrecognized by the application should be treated as
     *  "attachment"; see RFC 2183 sec. 2.8.).
     * @return This message's content disposition, or null if none is specified.
     */
    public final ContentDisposition getContentDisposition() {
        return this.contentDisposition;
      }
public final void setContentDisposition(ContentDisposition value) {
        if (value == null) {
          this.contentDisposition = null;
          this.RemoveHeader("content-disposition");
        } else if (!value.equals(this.contentDisposition)) {
          this.contentDisposition = value;
          this.SetHeader(
            "content-disposition",
            this.contentDisposition.toString());
        }
      }

    /**
     * Gets this message's media type. When getting, the media type may differ in
     * certain cases from the value of the Content-Type header field, if
     * any, and may have a value even if the Content-Type header field is
     * absent from this message. If set to a media type, updates the
     * Content-Type header field as appropriate. Cannot be set to null.
     * @return This message's media type.
     * @throws NullPointerException This value is being set and "value" is null.
     */
    public final MediaType getContentType() {
        return this.contentType;
      }
public final void setContentType(MediaType value) {
        if (value == null) {
          throw new NullPointerException("value");
        }
        if (this.contentType == null ||
          !this.contentType.equals(value)) {
          this.contentType = value;
          if (!value.isMultipart()) {
            List<Message> thisParts = this.getParts();
            thisParts.clear();
          }
          this.SetHeader(
            "content-type",
            this.contentType.toString());
        }
      }

    /**
     * <p>Gets a file name suggested by this message for saving the message's body
     * to a file. For more information on the algorithm, see
     * ContentDisposition.MakeFilename.</p> <p>This method generates a file
     * name based on the <code>filename</code> parameter of the
     * Content-Disposition header field, if it exists, or on the
     * <code>name</code> parameter of the Content-Type header field,
     * otherwise.</p>
     * @return A suggested name for the file. Returns the empty string if there is
     * no filename suggested by the content type or content disposition, or
     * if that filename is an empty string.
     */
    public final String getFileName() {
        ContentDisposition disp = this.contentDisposition;
        return (disp != null) ?
          ContentDisposition.MakeFilename(disp.GetParameter("filename")) :
          ContentDisposition.MakeFilename(this.contentType.GetParameter(
              "name"));
      }

    /**
     * Gets a list of addresses contained in the header fields with the given name
     * in this message.
     * @param headerName The name of the header fields to retrieve.
     * @return A list of addresses, in the order in which they appear in this
     * message's header fields of the given name.
     * @throws UnsupportedOperationException The parameter {@code headerName} is not
     * supported for this method. Currently, the only header fields
     * supported are To, Cc, Bcc, Reply-To, Sender, and From.
     * @throws NullPointerException The parameter {@code headerName} is null.
     * @throws IllegalArgumentException The parameter {@code headerName} is empty.
     */
    public List<NamedAddress> GetAddresses(String headerName) {
      if (headerName == null) {
        throw new NullPointerException("headerName");
      }
      if (headerName.length() == 0) {
        throw new IllegalArgumentException("headerName" + " is empty.");
      }
      headerName = DataUtilities.ToLowerCaseAscii(headerName);
      if (ValueHeaderIndices.containsKey(headerName) &&
        ValueHeaderIndices.get(headerName) <= 5) {
        return ParseAddresses(this.GetMultipleHeaders(headerName));
      } else {
        throw new UnsupportedOperationException("Not supported for: " + headerName);
      }
    }

    /**
     * Gets a list of addresses found in the From header field or fields.
     * @return A list of addresses found in the From header field or fields.
     * @deprecated Use GetAddresses(\From\) instead.
 */
@Deprecated
    public final List<NamedAddress> getFromAddresses() {
        return this.GetAddresses("from");
      }

    /**
     * Gets a snapshot of the header fields of this message, in the order in which
     * they appear in the message. For each item in the list, the key is
     * the header field's name (where any basic upper-case letters, U+0041
     * to U+005A, are converted to basic lower-case letters) and the value
     * is the header field's value.
     * @return A snapshot of the header fields of this message.
     */
    public final List<Map.Entry<String, String>> getHeaderFields() {
        ArrayList<Map.Entry<String, String>> list = new ArrayList<Map.Entry<String, String>>();
        for (int i = 0; i < this.headers.size(); i += 2) {
          Map.Entry<String, String> kp = new AbstractMap.SimpleImmutableEntry<String, String>(
            this.headers.get(i),
            this.headers.get(i + 1));
          list.add(kp);
        }
        return list;
      }

    /**
     * Gets a list of all the parts of this message. This list is editable. This
     * will only be used if the message is a multipart message.
     * @return A list of all the parts of this message. This list is editable. This
     * will only be used if the message is a multipart message.
     */
    public final List<Message> getParts() {
        return this.parts;
      }

    /**
     * Gets this message's subject. The subject's value is found as though
     *  GetHeader("subject") were called.
     * @return This message's subject, or null if there is none.
     */
    public final String getSubject() {
        return this.GetHeader("subject");
      }
public final void setSubject(String value) {
        this.SetHeader("subject", value);
      }

    /**
     * Gets a list of addresses found in the To header field or fields.
     * @return A list of addresses found in the To header field or fields.
     * @deprecated Use GetAddresses(\To\) instead.
 */
@Deprecated
    public final List<NamedAddress> getToAddresses() {
        return this.GetAddresses("to");
      }

    /**
     * Adds a header field to the end of the message's header. <p>This method
     * updates the ContentType and ContentDisposition properties if those
     * header fields have been modified by this method.</p>
     * @param header A key/value pair. The key is the name of the header field,
     *  such as "From" or "Content-ID". The value is the header field's
     * value.
     * @return This instance.
     * @throws NullPointerException The key or value of {@code header} is null.
     * @throws IllegalArgumentException The header field name is too long or contains an
     * invalid character, or the header field's value is syntactically
     * invalid.
     */
    public Message AddHeader(Map.Entry<String, String> header) {
      return this.AddHeader(header.getKey(), header.getValue());
    }

    /**
     * Adds a header field to the end of the message's header. <p>This method
     * updates the ContentType and ContentDisposition properties if those
     * header fields have been modified by this method.</p>
     * @param name Name of a header field, such as "From" or "Content-ID" .
     * @param value Value of the header field.
     * @return This instance.
     * @throws NullPointerException The parameter {@code name} or {@code value} is
     * null.
     * @throws IllegalArgumentException The header field name is too long or contains an
     * invalid character, or the header field's value is syntactically
     * invalid.
     */
    public Message AddHeader(String name, String value) {
      name = ValidateHeaderField(name, value);
      int index = this.headers.size() / 2;
      this.headers.add("");
      this.headers.add("");
      return this.SetHeader(index, name, value);
    }

    /**
     * Generates this message's data in text form. <p>The generated message will
     * have only Basic Latin code points (U+0000 to U+007F), and the
     * transfer encoding will always be 7bit, quoted-printable, or base64
     * (the declared transfer encoding for this message will be
     * ignored).</p> <p>The following applies to the following header
     * fields: From, To, Cc, Bcc, Reply-To, Sender, Resent-To, Resent-From,
     * Resent-Cc, Resent-Bcc, and Resent-Sender. If the header field
     * exists, but has an invalid syntax, has no addresses, or appears more
     * than once, this method will combine the addresses into one header
     * field if possible (in the case of all fields given other than From
     * and Sender), and otherwise generate a synthetic header field with
     * the display-name set to the contents of all of the header fields
     * with the same name, and the address set to
     * <code>me@[header-name]-address.invalid</code> as the address (a
     * <code>.invalid</code> address is a reserved address that can never belong
     * to anyone). (An exception is that the Resent-* header fields may
     * appear more than once.) The generated message should always have a
     * From header field.</p> <p>If a Date and/or Message-ID header field
     * doesn't exist, a field with that name will be generated (using the
     * current local time for the Date field).</p> <p>When encoding the
     *  message's body, if the message has a text content type ("text/*"),
     * the line breaks are a CR byte (carriage return, 0x0d) followed by an
     * LF byte (line feed, 0x0a), CR alone, or LF alone. If the message has
     * any other content type, only CR followed by LF is considered a line
     * break.</p>
     * @return The generated message.
     * @throws com.upokecenter.mail.MessageDataException The message can't be
     * generated.
     */
    public String Generate() {
      ArrayWriter aw = new ArrayWriter();
      this.Generate(
        aw,
        0);
      return DataUtilities.GetUtf8String(aw.ToArray(), false);
    }

    /**
     * Generates this message's data as a byte array, using the same algorithm as
     * the Generate method.
     * @return The generated message as a byte array.
     */
    public byte[] GenerateBytes() {
      ArrayWriter aw = new ArrayWriter();
      this.Generate(
        aw,
        0);
      return aw.ToArray();
    }

    /**
     * Gets the byte array for this message's body. This method doesn' t make a
     * copy of that byte array.
     * @return A byte array.
     */
    public byte[] GetBody() {
      return this.body;
    }

    /**
     * Gets the date and time extracted from this message's Date header field (the
     *  value of which is found as though GetHeader("date") were called).
     * See <b>MailDateTime.ParseDateString(string, boolean)</b> for more
     * information on the format of the date-time array returned by this
     * method.
     * @return An array of 32-bit unsigned integers.
     */
    public int[] GetDate() {
      String field = this.GetHeader("date");
      return (field == null) ? null : MailDateTime.ParseDateString(field,
  true);
    }

    /**
     * Sets this message's Date header field to the given date and time.
     * @param dateTime An array containing at least eight elements expressing a
     * date and time. See <b>MailDateTime.ParseDateString(string,
     * boolean)</b> for more information on this parameter.
     * @return This object.
     * @throws IllegalArgumentException The parameter {@code dateTime} contains fewer than
     * eight elements or contains invalid values (see
     * MailDateTime.ParseDateString(string, boolean)).
     * @throws NullPointerException The parameter {@code dateTime} is null.
     */
    public Message SetDate(int[] dateTime) {
      if (dateTime == null) {
        throw new NullPointerException("dateTime");
      }
      if (!MailDateTime.IsValidDateTime(dateTime)) {
        throw new IllegalArgumentException("Invalid date and time");
      }
      if (dateTime[0] < 0) {
        throw new IllegalArgumentException("Invalid year: " +
          ParserUtility.IntToString(dateTime[0]));
      }
      return this.SetHeader(
          "date",
          MailDateTime.GenerateDateString(dateTime));
    }

    /**
     * Returns the mail message contained in this message's body.
     * @return A message object if this object's content type is "message/rfc822",
     *  "message/news", or "message/global", or null otherwise.
     */

    public Message GetBodyMessage() {
      return (this.getContentType().getTopLevelType().equals("message") && (this.getContentType().getSubType().equals(
              "rfc822") ||
            this.getContentType().getSubType().equals("news") || this.getContentType().getSubType().equals("global"))) ? new Message(this.body) : null;
    }

    /**
     * Gets the name and value of a header field by index.
     * @param index Zero-based index of the header field to get.
     * @return A key/value pair. The key is the name of the header field, such as
     *  "From" or "Content-ID". The value is the header field's value.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at least as
     * high as the number of header fields.
     */
    public Map.Entry<String, String> GetHeader(int index) {
      if (index < 0) {
        throw new IllegalArgumentException("index(" + index + ") is less than " +
          "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index(" + index +
          ") is not less than " + (this.headers.size()
            / 2));
      }
      return new AbstractMap.SimpleImmutableEntry<String, String>(
        this.headers.get(index),
        this.headers.get(index + 1));
    }

    /**
     * Gets the first instance of the header field with the specified name, using a
     * basic case-insensitive comparison. (Two strings are equal in such a
     * comparison, if they match after converting the basic upper-case
     * letters A to Z (U+0041 to U+005A) in both strings to basic
     * lower-case letters.).
     * @param name The name of a header field.
     * @return The value of the first header field with that name, or null if there
     * is none.
     * @throws NullPointerException Name is null.
     */
    public String GetHeader(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          // Get the first instance of the header field
          return this.headers.get(i + 1);
        }
      }
      return null;
    }

    /**
     * Gets an array with the values of all header fields with the specified name,
     * using a basic case-insensitive comparison. (Two strings are equal in
     * such a comparison, if they match after converting the basic
     * upper-case letters A to Z (U+0041 to U+005A) in both strings to
     * basic lower-case letters.).
     * @param name The name of a header field.
     * @return An array containing the values of all header fields with the given
     * name, in the order they appear in the message. The array will be
     * empty if no header field has that name.
     * @throws NullPointerException Name is null.
     */
    public String[] GetHeaderArray(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      ArrayList<String> list = new ArrayList<String>();
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          list.add(this.headers.get(i + 1));
        }
      }
      return list.toArray(new String[] { });
    }

    /**
     * Deletes all header fields in this message. Also clears this message's
     * content disposition and resets its content type to
     * MediaType.TextPlainAscii.
     * @return This object.
     */
    public Message ClearHeaders() {
      this.headers.clear();
      this.contentType = MediaType.TextPlainAscii;
      this.contentDisposition = null;
      return this;
    }

    /**
     * Removes a header field by index. <p>This method updates the ContentType and
     * ContentDisposition properties if those header fields have been
     * modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @return This instance.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at least as
     * high as the number of header fields.
     */
    public Message RemoveHeader(int index) {
      if (index < 0) {
        throw new IllegalArgumentException("index(" + index + ") is less than " +
          "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index(" + index +
          ") is not less than " + (this.headers.size()
            / 2));
      }
      String name = this.headers.get(index * 2);
      this.headers.remove(index * 2);
      this.headers.remove(index * 2);
      if (name.equals("content-type")) {
        this.contentType = MediaType.TextPlainAscii;
      } else if (name.equals("content-disposition")) {
        this.contentDisposition = null;
      }
      return this;
    }

    /**
     * Removes all instances of the given header field from this message. If this
     * is a multipart message, the header field is not removed from its
     * body part headers. A basic case-insensitive comparison is used. (Two
     * strings are equal in such a comparison, if they match after
     * converting the basic upper-case letters A to Z (U+0041 to U+005A) in
     * both strings to basic lower-case letters.). <p>This method updates
     * the ContentType and ContentDisposition properties if those header
     * fields have been modified by this method.</p>
     * @param name The name of the header field to remove.
     * @return This instance.
     * @throws NullPointerException The parameter {@code name} is null.
     */
    public Message RemoveHeader(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      // Remove the header field
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          this.headers.remove(i);
          this.headers.remove(i);
          i -= 2;
        }
      }
      if (name.equals("content-type")) {
        this.contentType = MediaType.TextPlainAscii;
      } else if (name.equals("content-disposition")) {
        this.contentDisposition = null;
      }
      return this;
    }

    /**
     * Sets the body of this message to the given byte array. This method doesn't
     * make a copy of that byte array.
     * @param bytes A byte array.
     * @return This object.
     * @throws NullPointerException The parameter {@code bytes} is null.
     */
    public Message SetBody(byte[] bytes) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      this.body = bytes;
      return this;
    }

    /**
     * Sets the name and value of a header field by index. <p>This method updates
     * the ContentType and ContentDisposition properties if those header
     * fields have been modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @param header A key/value pair. The key is the name of the header field,
     *  such as "From" or "Content-ID". The value is the header field's
     * value.
     * @return A Message object.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at least as
     * high as the number of header fields; or, the header field name is
     * too long or contains an invalid character, or the header field's
     * value is syntactically invalid.
     * @throws NullPointerException The key or value of {@code header} is null.
     */
    public Message SetHeader(int index, Map.Entry<String, String> header) {
      return this.SetHeader(index, header.getKey(), header.getValue());
    }

    /**
     * Sets the name and value of a header field by index. <p>This method updates
     * the ContentType and ContentDisposition properties if those header
     * fields have been modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @param name Name of a header field, such as "From" or "Content-ID" .
     * @param value Value of the header field.
     * @return This instance.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at least as
     * high as the number of header fields; or, the header field name is
     * too long or contains an invalid character, or the header field's
     * value is syntactically invalid.
     * @throws NullPointerException The parameter {@code name} or {@code value} is
     * null.
     */
    public Message SetHeader(int index, String name, String value) {
      if (index < 0) {
        throw new IllegalArgumentException("index(" + index + ") is less than " +
          "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index(" + index +
          ") is not less than " + (this.headers.size()
            / 2));
      }
      name = ValidateHeaderField(name, value);
      this.headers.set(index * 2, name);
      this.headers.set((index * 2) + 1, value);
      if (name.equals("content-type")) {
        this.contentType = MediaType.Parse(value);
      } else if (name.equals("content-disposition")) {
        this.contentDisposition = ContentDisposition.Parse(value);
      }
      return this;
    }

    /**
     * Sets the value of a header field by index without changing its name. <p>This
     * method updates the ContentType and ContentDisposition properties if
     * those header fields have been modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @param value Value of the header field.
     * @return This instance.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at least as
     * high as the number of header fields; or, the header field name is
     * too long or contains an invalid character, or the header field's
     * value is syntactically invalid.
     * @throws NullPointerException The parameter {@code value} is null.
     */
    public Message SetHeader(int index, String value) {
      if (index < 0) {
        throw new IllegalArgumentException("index(" + index + ") is less than " +
          "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index(" + index +
          ") is not less than " + (this.headers.size()
            / 2));
      }
      return this.SetHeader(index, this.headers.get(index * 2), value);
    }

    /**
     * Decodes RFC 2047 encoded words from the given header field value and returns
     * a string with those words decoded. For an example of encoded words,
     * see the constructor for PeterO.Mail.NamedAddress.
     * @param name Name of the header field. This determines the syntax of the
     *  "value" parameter and is necessary to help this method interpret
     * encoded words properly.
     * @param value A header field value that could contain encoded words. For
     *  example, if the name parameter is "From", this parameter could be
     *  "=?utf-8?q?me?= &lt;me@example.com&gt;".
     * @return The header field value with valid encoded words decoded.
     * @throws NullPointerException The parameter {@code name} is null.
     */
    public static String DecodeHeaderValue(String name, String value) {
      return HeaderFieldParsers.GetParser(name).DecodeEncodedWords(value);
    }

    /**
     * Sets the value of this message's header field. If a header field with the
     * same name exists, its value is replaced. If the header field's name
     * occurs more than once, only the first instance of the header field
     * is replaced. <p>This method updates the ContentType and
     * ContentDisposition properties if those header fields have been
     * modified by this method.</p>
     * @param name The name of a header field, such as "from" or "subject" .
     * @param value The header field's value.
     * @return This instance.
     * @throws IllegalArgumentException The header field name is too long or contains an
     * invalid character, or the header field's value is syntactically
     * invalid.
     * @throws NullPointerException The parameter {@code name} or {@code value} is
     * null.
     */
    public Message SetHeader(String name, String value) {
      name = ValidateHeaderField(name, value);
      // Add the header field
      int index = 0;
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          return this.SetHeader(index, name, value);
        }
        ++index;
      }
      this.headers.add("");
      this.headers.add("");
      return this.SetHeader(index, name, value);
    }

    private static final MediaType TextHtmlAscii =
      MediaType.Parse("text/html; charset=us-ascii");

    private static final MediaType TextHtmlUtf8 =
      MediaType.Parse("text/html; charset=utf-8");

    /**
     * Sets the body of this message to the specified string in Hypertext Markup
     * Language (HTML) format. The character sequences CR (carriage return,
     *  "&#x5c;r", U+000D), LF (line feed, "&#x5c;n", U+000A), and CR/LF will be
     * converted to CR/LF line breaks. Unpaired surrogate code points will
     * be replaced with replacement characters.
     * @param str A string consisting of the message in HTML format.
     * @return This instance.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public Message SetHtmlBody(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      this.body = DataUtilities.GetUtf8Bytes(str, true, true);
      this.setContentType(IsShortAndAllAscii(str) ? TextHtmlAscii :
        TextHtmlUtf8);
      return this;
    }

    /**
     * Sets the body of this message to a multipart body with plain text and
     * Hypertext Markup Language (HTML) versions of the same message. The
     *  character sequences CR (carriage return, "&#x5c;r", U+000D), LF (line
     *  feed, "&#x5c;n", U+000A), and CR/LF will be converted to CR/LF line
     * breaks. Unpaired surrogate code points will be replaced with
     * replacement characters.
     * @param text A string consisting of the plain text version of the message.
     * @param html A string consisting of the HTML version of the message.
     * @return This instance.
     * @throws NullPointerException The parameter {@code text} or {@code html} is
     * null.
     */
    public Message SetTextAndHtml(String text, String html) {
      if (text == null) {
        throw new NullPointerException("text");
      }
      if (html == null) {
        throw new NullPointerException("html");
      }
      // The spec for multipart/alternative (RFC 2046) says that
      // the fanciest version of the message should go last (in
      // this case, the HTML version)
      Message textMessage = NewBodyPart().SetTextBody(text);
      Message htmlMessage = NewBodyPart().SetHtmlBody(html);
      String mtypestr = "multipart/alternative; " +
        "boundary=\"=_Boundary00000000\"";
      this.setContentType(MediaType.Parse(mtypestr));
      List<Message> messageParts = this.getParts();
      messageParts.clear();
      messageParts.add(textMessage);
      messageParts.add(htmlMessage);
      return this;
    }

    /**
     * Sets the body of this message to a multipart body with plain text, Markdown,
     * and Hypertext Markup Language (HTML) versions of the same message.
     *  The character sequences CR (carriage return, "&#x5c;r", U+000D), LF (line
     *  feed, "&#x5c;n", U+000A), and CR/LF will be converted to CR/LF line
     * breaks. Unpaired surrogate code points will be replaced with
     * replacement characters.<p> <p>REMARK: The Markdown-to-HTML
     * implementation currently supports all features of original Markdown,
     * except that the implementation:</p> <ul> <li>does not strictly check
     *  the placement of "block-level HTML elements",</li> <li>does not
     * prevent Markdown content from being interpreted as such merely
     *  because it's contained in a "block-level HTML element", and</li>
     * <li>does not deliberately use HTML escapes to obfuscate email
     * addresses wrapped in angle-brackets.</li></ul></p>
     * @param text A string consisting of the plain text version of the message.
     *  Can be null, in which case the value of the "markdown" parameter is
     * used as the plain text version.
     * @param markdown A string consisting of the Markdown version of the message.
     * For interoperability, this Markdown version will be converted to
     * HTML, where the Markdown text is assumed to be in the original
     * Markdown flavor.
     * @return This instance.
     * @throws NullPointerException The parameter {@code markdown} is null.
     */
    public Message SetTextAndMarkdown(String text, String markdown) {
      if (markdown == null) {
        throw new NullPointerException("markdown");
      }
      text = (text == null) ? (markdown) : text;
      Message textMessage = NewBodyPart().SetTextBody(text);
      Message markdownMessage = NewBodyPart().SetTextBody(markdown);
      String mtypestr = "text/markdown; charset=utf-8";
      markdownMessage.setContentType(MediaType.Parse(mtypestr));
      // Take advantage of SetTextBody's line break conversion
      String markdownText = markdownMessage.GetBodyString();
      Message htmlMessage = NewBodyPart().SetHtmlBody(
          FormatFlowed.MarkdownText(markdownText, 0));
      mtypestr = "multipart/alternative; boundary=\"=_Boundary00000000\"";
      this.setContentType(MediaType.Parse(mtypestr));
      List<Message> messageParts = this.getParts();
      messageParts.clear();
      messageParts.add(textMessage);
      messageParts.add(markdownMessage);
      messageParts.add(htmlMessage);
      return this;
    }

    /**
     * Sets the body of this message to the specified plain text string. The
     *  character sequences CR (carriage return, "&#x5c;r", U+000D), LF (line
     *  feed, "&#x5c;n", U+000A), and CR/LF will be converted to CR/LF line
     * breaks. Unpaired surrogate code points will be replaced with
     * replacement characters. This method changes this message's media
     * type to plain text.
     * @param str A string consisting of the message in plain text format.
     * @return This instance.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public Message SetTextBody(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      this.body = DataUtilities.GetUtf8Bytes(str, true, true);
      this.setContentType(IsShortAndAllAscii(str) ? MediaType.TextPlainAscii :
        MediaType.TextPlainUtf8);
      return this;
    }

    private Message AddBodyPart(
      InputStream inputStream,
      MediaType mediaType,
      String filename,
      String disposition) {
      return this.AddBodyPart(
        inputStream,
        mediaType,
        filename,
        disposition,
        false);
    }

    /**
     * Adds an inline body part with an empty body and with the given media type to
     * this message. Before the new body part is added, if this message
     *  isn't already a multipart message, it becomes a "multipart/mixed"
     * message with the current body converted to an inline body part.
     * @param mediaType A media type to assign to the body part.
     * @return A Message object for the generated body part.
     */
    public Message AddInline(MediaType mediaType) {
      return this.AddBodyPart(null, mediaType, null, "inline", true);
    }

    /**
     * Adds an attachment with an empty body and with the given media type to this
     * message. Before the new attachment is added, if this message isn't
     *  already a multipart message, it becomes a "multipart/mixed" message
     * with the current body converted to an inline body part.
     * @param mediaType A media type to assign to the attachment.
     * @return A Message object for the generated attachment.
     */
    public Message AddAttachment(MediaType mediaType) {
      return this.AddBodyPart(null, mediaType, null, "attachment", true);
    }

    private Message AddBodyPart(
      InputStream inputStream,
      MediaType mediaType,
      String filename,
      String disposition,
      boolean allowNullStream) {
      if (!allowNullStream && inputStream == null) {
        throw new NullPointerException("inputStream");
      }
      if (mediaType == null) {
        throw new NullPointerException("mediaType");
      }
      Message bodyPart = NewBodyPart();
      bodyPart.SetHeader(
        "content-id",
        this.GenerateMessageID());
      // NOTE: Using the setter because it also adds a Content-Type
      // header field
      bodyPart.setContentType(mediaType);
      if (inputStream != null) {
        try {
          {
            java.io.ByteArrayOutputStream ms = null;
try {
ms = new java.io.ByteArrayOutputStream();

            if (mediaType.isMultipart()) {
              try {
                IByteReader transform = DataIO.ToReader(inputStream);
                bodyPart.ReadMultipartBody(transform);
              } catch (IllegalStateException ex) {
                throw new MessageDataException(ex.getMessage(), ex);
              }
            } else {
              byte[] buffer = new byte[4096];
              while (true) {
                int cp = inputStream.read(buffer, 0, buffer.length);
                if (cp <= 0) {
                  break;
                }
                ms.write(buffer, 0, cp);
              }
              bodyPart.SetBody(ms.toByteArray());
            }
}
finally {
try { if (ms != null) { ms.close(); } } catch (java.io.IOException ex) {}
}
}
        } catch (IOException ex) {
          throw new MessageDataException("An I/O error occurred.", ex);
        }
      }
      DispositionBuilder dispBuilder = new DispositionBuilder(disposition);
      if (!((filename) == null || (filename).length() == 0)) {
        String basename = BaseName(filename);
        if (!((basename) == null || (basename).length() == 0)) {
          dispBuilder.SetParameter(
            "filename",
            basename);
        }
      }
      bodyPart.setContentDisposition(dispBuilder.ToDisposition());
      if (this.getContentType().isMultipart()) {
        this.getParts().add(bodyPart);
      } else {
        Message existingBody = NewBodyPart();
        existingBody.setContentDisposition(ContentDisposition.Parse("inline"));
        existingBody.setContentType(this.getContentType());
        existingBody.SetBody(this.GetBody());
        String mtypestr = "multipart/mixed; boundary=\"=_Boundary00000000\"";
        this.setContentType(MediaType.Parse(mtypestr));
        this.getParts().add(existingBody);
        this.getParts().add(bodyPart);
      }
      return bodyPart;
    }

    private static String BaseName(String filename) {
      int i = 0;
      for (i = filename.length() - 1; i >= 0; --i) {
        if (filename.charAt(i) == '\\' || filename.charAt(i) == '/') {
          return filename.substring(i + 1);
        }
      }
      return filename;
    }

    private static String ExtensionName(String filename) {
      int i = 0;
      for (i = filename.length() - 1; i >= 0; --i) {
        if (filename.charAt(i) == '\\' || filename.charAt(i) == '/') {
          return "";
        } else if (filename.charAt(i) == '.') {
          return filename.substring(i);
        }
      }
      return "";
    }

    private static MediaType SuggestMediaType(String filename) {
      if (!((filename) == null || (filename).length() == 0)) {
        String ext = DataUtilities.ToLowerCaseAscii(
            ExtensionName(filename));
        if (ext.equals(".doc") ||
          ext.equals(".dot")) {
          return MediaType.Parse("application/msword");
        }
        if (ext.equals(".pdf")) {
          return MediaType.Parse("application/pdf");
        }
        if (ext.equals(".key")) {
          return MediaType.Parse("application/pgp-keys");
        }
        if (ext.equals(".sig")) {
          return MediaType.Parse("application/pgp-signature");
        }
        if (ext.equals(".rtf")) {
          return MediaType.Parse("application/rtf");
        }
        if (ext.equals(".docx")) {
          return

            MediaType.Parse(
  "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        if (ext.equals(".zip")) {
          return MediaType.Parse("application/zip");
        }
        if (ext.equals(".m4a") ||
          ext.equals(".mp2") ||
          ext.equals(".mp3") ||
          ext.equals(".mpega") ||
          ext.equals(".mpga")) {
          return MediaType.Parse("audio/mpeg");
        }
        if (ext.equals(".gif")) {
          return MediaType.Parse("image/gif");
        }
        if (ext.equals(".jpe") ||
          ext.equals(".jpeg") ||
          ext.equals(".jpg")) {
          return MediaType.Parse("image/jpeg");
        }
        if (ext.equals(".png")) {
          return MediaType.Parse("image/png");
        }
        if (ext.equals(".tif") ||
          ext.equals(".tiff")) {
          return MediaType.Parse("image/tiff");
        }
        if (ext.equals(".eml")) {
          return MediaType.Parse("message/rfc822");
        }
        if (ext.equals(".rst")) {
          return MediaType.Parse(
  "text/prs.fallenstein.rst\u003bcharset=utf-8");
        }
        if (ext.equals(".htm") ||
          ext.equals(".html") ||
          ext.equals(".shtml")) {
          return MediaType.Parse("text/html\u003bcharset=utf-8");
        }
        if (ext.equals(".md") ||
          ext.equals(".markdown")) {
          return MediaType.Parse("text/markdown\u003bcharset=utf-8");
        }
        if (ext.equals(".asc") ||
          ext.equals(".brf") ||
          ext.equals(".pot") ||
          ext.equals(".srt") ||
          ext.equals(".text") ||
          ext.equals(".txt")) {
          return MediaType.Parse("text/plain\u003bcharset=utf-8");
        }
      }
      return MediaType.ApplicationOctetStream;
    }

    /**
     * Adds an attachment to this message in the form of data from the given
     * readable stream, and with the given media type. Before the new
     * attachment is added, if this message isn't already a multipart
     *  message, it becomes a "multipart/mixed" message with the current
     * body converted to an inline body part.<p> The following example
     * (written in C# for the.NET version) is an extension method that adds
     * an attachment from a byte array to a message. <pre>public static Message AddAttachmentFromBytes(Message msg, byte[] bytes, MediaType mediaType) { {
java.io.ByteArrayInputStream fs = null;
try {
fs = new java.io.ByteArrayInputStream(bytes);
 return msg.AddAttachment(fs, mediaType);
}
finally {
try { if (fs != null) { fs.close(); } } catch (java.io.IOException ex) {}
}
} }</pre> . </p>
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the attachment.
     * @return A Message object for the generated attachment.
     * @throws NullPointerException The parameter {@code inputStream} or {@code
     * mediaType} is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddAttachment(InputStream inputStream, MediaType mediaType) {
      return this.AddBodyPart(inputStream, mediaType, null, "attachment");
    }

    /**
     * Adds an attachment to this message in the form of data from the given
     * readable stream, and with the given file name. Before the new
     * attachment is added, if this message isn't already a multipart
     *  message, it becomes a "multipart/mixed" message with the current
     * body converted to an inline body part.
     * @param inputStream A readable data stream.
     * @param filename A file name to assign to the attachment. Can be null or
     * empty, in which case no file name is assigned. Only the file name
     * portion of this parameter is used, which in this case means the
     *  portion of the string after the last "/" or "&#x5c;", if either character
     * exists, or the entire string otherwise An appropriate media type (or
     *  "application/octet-stream") will be assigned to the attachment based
     * on this file name's extension. If the file name has an extension
     *.txt, .text, .htm, .html, .shtml, .asc, .brf, .pot, .rst, .md,
     *.markdown, or .srt, the media type will have a "charset" of "utf-8".
     * @return A Message object for the generated attachment.
     * @throws NullPointerException The parameter {@code inputStream} is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddAttachment(InputStream inputStream, String filename) {
      return this.AddBodyPart(
          inputStream,
          SuggestMediaType(filename),
          filename,
          "attachment");
    }

    /**
     * Adds an attachment to this message in the form of data from the given
     * readable stream, and with the given media type and file name. Before
     * the new attachment is added, if this message isn't already a
     *  multipart message, it becomes a "multipart/mixed" message with the
     * current body converted to an inline body part.
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the attachment.
     * @param filename A file name to assign to the attachment. Can be null or
     * empty, in which case no file name is assigned. Only the file name
     * portion of this parameter is used, which in this case means the
     *  portion of the string after the last "/" or "&#x5c;", if either character
     * exists, or the entire string otherwise.
     * @return A Message object for the generated attachment.
     * @throws NullPointerException The parameter {@code inputStream} or {@code
     * mediaType} is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddAttachment(
      InputStream inputStream,
      MediaType mediaType,
      String filename) {
      return this.AddBodyPart(
        inputStream,
        mediaType,
        filename,
        "attachment");
    }

    /**
     * Adds an inline body part to this message in the form of data from the given
     * readable stream, and with the given media type. Before the new body
     * part is added, if this message isn't already a multipart message, it
     *  becomes a "multipart/mixed" message with the current body converted
     * to an inline body part.<p> The following example (written in C# for
     * the.NET version) is an extension method that adds an inline body
     * part from a byte array to a message. <pre>public static Message AddInlineFromBytes(Message msg, byte[] bytes, MediaType mediaType) { {
java.io.ByteArrayInputStream fs = null;
try {
fs = new java.io.ByteArrayInputStream(bytes);
 return msg.AddInline(fs, mediaType);
}
finally {
try { if (fs != null) { fs.close(); } } catch (java.io.IOException ex) {}
}
} }</pre> . </p>
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the body part.
     * @return A Message object for the generated body part.
     * @throws NullPointerException The parameter {@code inputStream} or {@code
     * mediaType} is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddInline(InputStream inputStream, MediaType mediaType) {
      return this.AddBodyPart(inputStream, mediaType, null, "inline");
    }

    /**
     * Adds an inline body part to this message in the form of data from the given
     * readable stream, and with the given file name. Before the new body
     * part is added, if this message isn't already a multipart message, it
     *  becomes a "multipart/mixed" message with the current body converted
     * to an inline body part.
     * @param inputStream A readable data stream.
     * @param filename A file name to assign to the inline body part. Can be null
     * or empty, in which case no file name is assigned. Only the file name
     * portion of this parameter is used, which in this case means the
     *  portion of the string after the last "/" or "&#x5c;", if either character
     * exists, or the entire string otherwise An appropriate media type (or
     *  "application/octet-stream") will be assigned to the body part based
     * on this file name's extension. If the file name has an extension
     *.txt, .text, .htm, .html, .shtml, .asc, .brf, .pot, .rst, .md,
     *.markdown, or .srt, the media type will have a "charset" of "utf-8".
     * @return A Message object for the generated body part.
     * @throws NullPointerException The parameter {@code inputStream} or
     *  "mediaType" is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddInline(InputStream inputStream, String filename) {
      return this.AddBodyPart(
          inputStream,
          SuggestMediaType(filename),
          filename,
          "inline");
    }

    /**
     * Adds an inline body part to this message in the form of data from the given
     * readable stream, and with the given media type and file name. Before
     * the new body part is added, if this message isn't already a
     *  multipart message, it becomes a "multipart/mixed" message with the
     * current body converted to an inline body part.
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the body part.
     * @param filename A file name to assign to the body part.
     * @return A Message object for the generated body part.
     * @throws NullPointerException The parameter {@code inputStream} or {@code
     * mediaType} is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddInline(
      InputStream inputStream,
      MediaType mediaType,
      String filename) {
      return this.AddBodyPart(inputStream, mediaType, filename, "inline");
    }

    private static boolean HasSameAddresses(Message m1, Message m2) {
      List<NamedAddress> n1 = m1.GetAddresses("from");
      List<NamedAddress> n2 = m2.GetAddresses("from");
      if (n1.size() != n2.size()) {
        return false;
      }
      for (int i = 0; i < n1.size(); ++i) {
        if (!n1.get(i).AddressesEqual(n2.get(i))) {
          return false;
        }
      }
      return true;
    }
    private static String GetContentTranslationType(String ctt) {
      if (((ctt) == null || (ctt).length() == 0)) {
        return "";
      }
      int index = HeaderParser.ParseFWS(ctt, 0, ctt.length(), null);
      int cttEnd = HeaderParser.ParsePhraseAtom(ctt, index, ctt.length(), null);
      if (cttEnd != ctt.length()) {
        return "";
      }
      return DataUtilities.ToLowerCaseAscii(
          ctt.substring(index, (index)+(cttEnd - index)));
    }

    /**
     * Selects a body part for a multiple-language message(
     * <code>multipart/multilingual</code>) according to the given language
     * priority list.
     * @param languages A list of basic language ranges, sorted in descending order
     * of priority (see the LanguageTags.LanguageTagFilter method).
     * @return The best matching body part for the given languages. If the body
     * part has no subject, then the top-level subject is used. If this
     * message is not a multipart/multilingual message or has fewer than
     * two body parts, returns this object. If no body part matches the
     * given languages, returns the last body part if its language is
     *  "zxx", or the second body part otherwise.
     * @throws NullPointerException The parameter {@code languages} is null.
     */
    public Message SelectLanguageMessage(
      List<String> languages) {
      return this.SelectLanguageMessage(languages, false);
    }

    /**
     * Selects a body part for a multiple-language message(
     * <code>multipart/multilingual</code>) according to the given language
     * priority list and original-language preference.
     * @param languages A list of basic language ranges, sorted in descending order
     * of priority (see the LanguageTags.LanguageTagFilter method).
     * @param preferOriginals If true, a body part marked as the original language
     * version is chosen if it matches one of the given language ranges,
     * even if the original language has a lower priority than another
     * language with a matching body part.
     * @return The best matching body part for the given languages. If the body
     * part has no subject, then the top-level subject is used. If this
     * message is not a multipart/multilingual message or has fewer than
     * two body parts, returns this object. If no body part matches the
     * given languages, returns the last body part if its language is
     *  "zxx", or the second body part otherwise.
     * @throws NullPointerException The parameter {@code languages} is null.
     */
    public Message SelectLanguageMessage(
      List<String> languages,
      boolean preferOriginals) {
      if (this.getContentType().getTypeAndSubType().equals("multipart/multilingual") && this.getParts().size() >= 2) {
        String subject = this.GetHeader("subject");
        int passes = preferOriginals ? 2 : 1;
        List<String> clang;
        List<String> filt;
        for (int i = 0; i < passes; ++i) {
          for (Message part : this.getParts()) {
            clang = LanguageTags.GetLanguageList(part.GetHeader(
                  "content-language"));
            if (clang == null) {
              continue;
            }
            if (preferOriginals && i == 0) { // Allow originals only, on first
              String ctt = GetContentTranslationType(part.GetHeader(
  "content-translation-type"));
              if (!ctt.equals("original")) {
                continue;
              }
            }
            filt = LanguageTags.LanguageTagFilter(languages, clang);
            if (filt.size() > 0) {
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
        Message firstmsg = this.getParts().get(1);
        Message lastPart = this.getParts().get(this.getParts().size() - 1);
        List<String> zxx = Arrays.asList(new String[] { "zxx" });
        clang = LanguageTags.GetLanguageList(
            lastPart.GetHeader("content-language"));
        if (clang != null) {
          filt = LanguageTags.LanguageTagFilter(zxx, clang);
          if (filt.size() > 0) {
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

    /**
     * Generates a multilingual message (see RFC 8255) from a list of messages and
     * a list of language strings.
     * @param messages A list of messages forming the parts of the multilingual
     * message object. Each message should have the same content, but be in
     * a different language. Each message must have a From header field and
     * use the same email address in that field as the other messages. The
     * messages should be ordered in descending preference of language.
     * @param languages A list of language strings corresponding to the messages
     *  given in the "messages" parameter. A language string at a given
     * index corresponds to the message at the same index. Each language
     * string must follow the syntax of the Content-Language header field
     * (see LanguageTags.GetLanguageList).
     * @return A Message object with the content type "multipart/multilingual" . It
     * will begin with an explanatory body part and be followed by the
     * messages given in the {@code messages} parameter in the order given.
     * @throws NullPointerException The parameter {@code messages} or {@code
     * languages} is null.
     * @throws IllegalArgumentException The parameter {@code messages} or {@code
     * languages} is empty, their lengths don't match, at least one message
     *  is "null", each message doesn't contain the same email addresses in
     * their From header fields, {@code languages} contains a syntactically
     * invalid language tag list, {@code languages} contains the language
     *  tag "zzx" not appearing alone or at the end of the language tag
     * list, or the first message contains no From header field.
     */
    public static Message MakeMultilingualMessage(
      List<Message> messages,
      List<String> languages) {
      if (messages == null) {
        throw new NullPointerException("messages");
      }
      if (languages == null) {
        throw new NullPointerException("languages");
      }
      if (messages.size() < 0) {
        throw new IllegalArgumentException("messages.size()(" + messages.size() +
          ") is less than 0");
      }
      if (messages.size() != languages.size()) {
        throw new IllegalArgumentException("messages.size()(" + messages.size() +
          ") is not equal to " + languages.size());
      }
      StringBuilder prefaceBody;
      for (int i = 0; i < messages.size(); ++i) {
        if (messages.get(i) == null) {
          throw new IllegalArgumentException("A message in 'messages' is null");
        }
        if (i > 0 && !HasSameAddresses(messages.get(0), messages.get(i))) {
          throw new IllegalArgumentException(
            "Each message doesn't contain the same email addresses");
        }
      }
      for (String lang : languages) {
        List<String> langtags = LanguageTags.GetLanguageList(lang);
        if (langtags == null) {
          throw new IllegalArgumentException(
            lang + " is an invalid list of language tags");
        }
      }
      prefaceBody = new StringBuilder().append("This is a multilingual " +
        "message, a message that\r\ncan be read in one or more different " +
        "languages. Each\r\npart of the message may appear inline, as an " +
        "attachment, or both.\r\n\r\n");
      prefaceBody.append("Languages available:\r\n\r\n");
      for (String lang : languages) {
        prefaceBody.append("- ").append(lang).append("\r\n");
      }
      StringBuilder prefaceSubject = new StringBuilder();
      ArrayList<String> zxx = new ArrayList<String>();
      zxx.add("zxx");
      for (int i = 0; i < languages.size(); ++i) {
        List<String> langs = LanguageTags.GetLanguageList(languages.get(i));
        boolean langInd = i == languages.size() - 1 && langs.size() == 1 &&
          langs.get(0).equals("zxx");
        if (!langInd && LanguageTags.LanguageTagFilter(
          zxx,
          langs).size() > 0) {
          throw new IllegalArgumentException("zxx tag can only appear at end");
        }
        String subject = messages.get(i).GetHeader("subject");
        if (!((subject) == null || (subject).length() == 0)) {
          if (prefaceSubject.length() > 0) {
            prefaceSubject.append(" / ");
          }
          prefaceSubject.append(subject);
        }
      }
      if (prefaceSubject.length() == 0) {
        prefaceSubject.append("Multilingual Message");
        prefaceSubject.append('(');
        for (int i = 0; i < languages.size(); ++i) {
          if (i > 0) {
            prefaceSubject.append(", ");
          }
          prefaceSubject.append(languages.get(i));
        }
        prefaceSubject.append(')');
      }
      String fromHeader = messages.get(0).GetHeader("from");
      if (fromHeader == null) {
        throw new IllegalArgumentException("First message has no From header");
      }
      Message msg = new Message();
      msg.setContentType(MediaType.Parse("multipart/multilingual"));
      msg.SetHeader(
        "from",
        fromHeader);
      msg.setContentDisposition(ContentDisposition.Parse("inline"));
      String toHeader = messages.get(0).GetHeader("to");
      Message preface;
      if (toHeader != null) {
        msg.SetHeader("to", toHeader);
      }
      msg.SetHeader(
        "subject",
        prefaceSubject.toString());
      preface = msg.AddInline(MediaType.Parse("text/plain;charset=utf-8"));
      preface.SetTextBody(prefaceBody.toString());
      for (int i = 0; i < messages.size(); ++i) {
        MediaType mt = MediaType.Parse("message/rfc822");
        String msgstring = messages.get(i).Generate();
        if (msgstring.indexOf("\r\n--") >= 0 || (
            msgstring.length() >= 2 && msgstring.charAt(0) == '-' &&
            msgstring.charAt(1) == '-')) {
          // Message/global allows quoted-printable and
          // base64, so we can avoid raw boundary delimiters
          mt = MediaType.Parse("message/global");
        }
        Message part = msg.AddInline(mt);
        part.SetHeader(
          "content-language",
          languages.get(i));
        part.SetBody(DataUtilities.GetUtf8Bytes(msgstring, true));
      }
      return msg;
    }

    static boolean CanBeUnencoded(
      byte[] bytes,
      boolean checkBoundaryDelimiter) {
      if (bytes == null || bytes.length == 0) {
        return true;
      }
      int lineLength = 0;
      int index = 0;
      int endIndex = bytes.length;
      boolean headers = true;
      while (index < endIndex) {
        int c = ((int)bytes[index]) & 0xff;
        if (c >= 0x80) {
          // System.out.println("Non-ASCII character (0x {0:X2})",(int)c);
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
          // System.out.println("Bare CR or bare LF");
          return false;
        }
        if (c == 0 || c == 0x7f) {
          // System.out.println("NULL or DEL character");
          return false;
        }
        ++lineLength;
        if (lineLength > MaxRecHeaderLineLength) {
          // System.out.println("Line length exceeded (" + lineLength +
          // " " + (str.substring(index-MaxRecHeaderLineLength,(index-MaxRecHeaderLineLength)+(// MaxRecHeaderLineLength))) + ")");
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
    static byte[] DowngradeDeliveryStatus(byte[] bytes) {
      StringBuilder sb = new StringBuilder();
      int index = 0;
      int endIndex = bytes.length;
      int lastIndex = -1;
      ArrayWriter writer = null;
      while (index < endIndex) {
        sb.delete(0, sb.length());
        boolean first = true;
        int headerNameStart = index;
        int headerNameEnd = index;
        // lineCount = 0;
        boolean endOfHeaders = false;
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
            sb.append((char)c);
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
        String fieldName = DataUtilities.ToLowerCaseAscii(
            DataUtilities.GetUtf8String(
              bytes,
              headerNameStart,
              headerNameEnd - headerNameStart,
              true));
        boolean origRecipient = fieldName.equals("original-recipient");
        boolean finalRecipient = fieldName.equals("final-recipient");
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
              boolean fwsFirst = true;
              boolean haveFWS = false;
              boolean lineStart = true;
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
          String headerValue = null;
          int[] status = new int[1];
          try {
            headerValue = DataUtilities.GetUtf8String(
              bytes,
              headerValueStart,
              headerValueEnd - headerValueStart,
              false); // throws on invalid UTF-8
          } catch (IllegalArgumentException ex) {
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
              writer.write(bytes, 0, headerNameStart);
            } else {
              writer.write(bytes, lastIndex, headerNameStart - lastIndex);
            }
            if (status[0] == 1) {
              // Downgraded
              byte[] newBytes = DataUtilities.GetUtf8Bytes(
                headerValue,
                true);
              writer.write(newBytes, 0, newBytes.length);
            } else {
              // Encapsulated
              String field = origRecipient ? "Downgraded-Original-Recipient" :
                "Downgraded-Final-Recipient";
              headerValue = DataUtilities.GetUtf8String(
                bytes,
                headerValueStart,
                headerValueEnd - headerValueStart,
                true); // replaces invalid UTF-8
              String newField = HeaderEncoder.EncodeFieldAsEncodedWords(
                field,
                headerValue);
              byte[] newBytes = DataUtilities.GetUtf8Bytes(
                newField,
                true);
              writer.write(newBytes, 0, newBytes.length);
            }
          }
          lastIndex = headerValueEnd;
        }
      }
      if (writer != null) {
        writer.write(bytes, lastIndex, bytes.length - lastIndex);
        bytes = writer.ToArray();
      }
      return bytes;
    }

    private static HeaderEncoder EncodeCommentsInText(
      HeaderEncoder enc,
      String str) {
      int i = 0;
      int begin = 0;
      if (str.indexOf('(') < 0) {
        return enc.AppendString(str);
      }
      StringBuilder sb = new StringBuilder();
      while (i < str.length()) {
        if (str.charAt(i) == '(') {
          int si = HeaderParserUtility.ParseCommentLax(
            str,
            i,
            str.length(),
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
      return enc.AppendString(str, begin, str.length());
    }

    static String DowngradeRecipientHeaderValue(
      String fieldName,
      String headerValue,
      int[] status) {
      int index;
      if (
        HasTextToEscape(
          headerValue,
          0,
          headerValue.length())) {
        index = HeaderParser.ParseCFWS(
          headerValue,
          0,
          headerValue.length(),
          null);
        int atomText = HeaderParser.ParsePhraseAtom(
          headerValue,
          index,
          headerValue.length(),
          null);
        int typeEnd = atomText;
        String origValue = headerValue;
        boolean isUtf8 = typeEnd - index == 5 &&
          (headerValue.charAt(index) & ~0x20) == 'U' && (headerValue.charAt(index +
              1) & ~0x20) == 'T' && (headerValue.charAt(index + 2) & ~0x20) == 'F' &&
          headerValue.charAt(index + 3) == '-' && headerValue.charAt(index + 4) == '8';
        atomText = HeaderParser.ParseCFWS(
          headerValue,
          atomText,
          headerValue.length(),
          null);
        // NOTE: Commented out for now (see below)
        // if (atomText != typeEnd) {
        // isUtf8 = false;
        // }
        if (index < headerValue.length() && headerValue.charAt(atomText) == ';') {
          int addressPart = HeaderParser.ParseCFWS(
            headerValue,
            atomText + 1,
            headerValue.length(),
            null);
          HeaderEncoder encoder = new HeaderEncoder().AppendFieldName(fieldName);
          if (isUtf8) {
            String typePart = headerValue.substring(0, addressPart);
            // Downgrade the non-ASCII characters in the address
            // NOTE: The ABNF for utf-8-type-addr in RFC 6533
            // appears not to allow linear white space.
            // DEVIATION: Allow CFWS between "utf-8" and semicolon
            // for sake of robustness, even though it doesn't fit
            // utf-8-type-addr (see also RFC 8098 secs. 3.2.3
            // and 3.2.4)
            StringBuilder builder = new StringBuilder();
            String ValueHex = "0123456789ABCDEF";
            for (int i = addressPart; i < headerValue.length(); ++i) {
              if (headerValue.charAt(i) < 0x7f && headerValue.charAt(i) > 0x20 &&
                headerValue.charAt(i) != '\\' && headerValue.charAt(i) != '+' &&
                headerValue.charAt(i) != '-') {
                builder.append(headerValue.charAt(i));
              } else {
                int cp = DataUtilities.CodePointAt(headerValue, i);
                if (cp >= 0x10000) {
                  ++i;
                }
                builder.append("\\x");
                builder.append('{');
                for (int j = 20; j >= 0; j -= 4) {
                  if ((cp >> j) != 0) {
                    builder.append(ValueHex.charAt((cp >> j) & 15));
                  }
                }
                builder.append('}');
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
          headerValue = encoder.toString();
        }
        if (
          HasTextToEscape(
            headerValue,
            0,
            headerValue.length())) {
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

    static String GenerateAddressList(List<NamedAddress> list) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < list.size(); ++i) {
        if (i > 0) {
          sb.append(", ");
        }
        sb.append(list.get(i).toString());
      }
      return sb.toString();
    }

    static boolean HasTextToEscapeOrEncodedWordStarts(String s) {
      // <summary>Returns true if the String has:
      // * non-ASCII characters,
      // * "=?",
      // * CTLs other than tab, or
      // * a word longer than MaxRecHeaderLineLength minus 1 characters.
      // Can return false even if the String has:
      // * CRLF followed by a line with just whitespace.</summary>
      return HasTextToEscapeOrEncodedWordStarts(s, 0, s.length(), true);
    }

    static boolean HasTextToEscapeOrEncodedWordStarts(
      String s,
      int index,
      int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, true);
    }

    static boolean HasTextToEscape(String s, int index, int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, false);
    }

    static boolean HasTextToEscape(String s) {
      return HasTextToEscapeOrEncodedWordStarts(s, 0, s.length(), false);
    }

    static boolean HasTextToEscapeOrEncodedWordStarts(
      String s,
      int index,
      int endIndex,
      boolean checkEWStarts) {
      int len = endIndex;
      int chunkLength = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = s.charAt(i);
        if (checkEWStarts && c == '=' && i + 1 < len && c == '?') {
          // "=?" (start of an encoded word)
          return true;
        }
        if (c == 0x0d) {
          if (i + 1 >= len || s.charAt(i + 1) != 0x0a) {
            // bare CR
            // System.out.println("bare CR");
            return true;
          }
          if (i + 2 >= len || (s.charAt(i + 2) != 0x09 && s.charAt(i + 2) != 0x20)) {
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

    static List<NamedAddress> ParseAddresses(String[] values) {
      ArrayList<NamedAddress> list = new ArrayList<NamedAddress>();
      for (String addressValue : values) {
        list.addAll(NamedAddress.ParseAddresses(addressValue));
      }
      return list;
    }

    static int ParseUnstructuredText(
      String s,
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
          if (i + 2 >= endIndex || s.charAt(i + 1) != 0x0a || (s.charAt(i + 2) != 0x09 &&
              s.charAt(i + 2) != 0x20)) {
            // bare CR, or CRLF not followed by SP/TAB
            return i;
          }
          i += 3;
          boolean found = false;
          for (int j = i; j < endIndex; ++j) {
            if (s.charAt(j) != 0x09 && s.charAt(j) != 0x20 && s.charAt(j) != 0x0d) {
              found = true;
              break;
            } else if (s.charAt(j) == 0x0d) {
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

    private static void AppendAscii(IWriter output, String str) {
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (c >= 0x80) {
          throw new MessageDataException("ASCII expected");
        }
        output.write((byte)c);
      }
    }

    private static String GenerateBoundary(int num) {
      StringBuilder sb = new StringBuilder();
      String ValueHex = "0123456789ABCDEF";
      sb.append("=_Boundary");
      for (int i = 0; i < 4; ++i) {
        int b = (num >> 56) & 255;
        sb.append(ValueHex.charAt((b >> 4) & 15));
        sb.append(ValueHex.charAt(b & 15));
        num <<= 8;
      }
      return sb.toString();
    }

    private static boolean IsShortAndAllAscii(String str) {
      if (str.length() > 0x10000) {
        return false;
      }
      for (int i = 0; i < str.length(); ++i) {
        if ((((int)str.charAt(i)) >> 7) != 0) {
          return false;
        }
      }
      return true;
    }

    private static boolean IsWellFormedBoundary(String str) {
      if (((str) == null || (str).length() == 0) || str.length() > 70) {
        return false;
      }
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (i == str.length() - 1 && c == 0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
            (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' &&
c <=
              '9') || c == 0x20 || c == 0x2c || "'()-./+_:=?".indexOf(c) >=
            0)) {
          return false;
        }
      }
      return true;
    }

    private static Map<String, Integer> MakeHeaderIndices() {
      HashMap<String, Integer> dict = new HashMap<String, Integer>();
      dict.put("to",0);
      dict.put("cc",1);
      dict.put("bcc",2);
      dict.put("from",3);
      dict.put("sender",4);
      dict.put("reply-to",5);
      dict.put("resent-to",6);
      dict.put("resent-cc",7);
      dict.put("resent-bcc",8);
      dict.put("resent-from",9);
      dict.put("resent-sender",10);
      return dict;
    }

    private static IByteReader MakeTransferEncoding(
      IByteReader stream,
      int encoding,
      boolean useLiberalSevenBit) {
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
          transform = useLiberalSevenBit ? ((IByteReader)new
LiberalSevenBitTransform(stream)) :
            ((IByteReader)new SevenBitTransform(stream));

          break;
      }

      return transform;
    }

    private static final class StringBuilderKeepBuffer {
      // To maintain control over how a String
      // builder's capacity is handled when the
      // String builder is "reset"
      private char[] buffer = new char[64];
      private int ptr = 0;
      public void Reset() {
        this.ptr = 0;
      }
      public final boolean isEmpty() {
          return this.buffer.length == 0;
        }
      @Override public String toString() {
        return new String(this.buffer, 0, this.ptr);
      }
      private void Grow() {
        int newlen = (this.buffer.length >= (Integer.MAX_VALUE >> 1)) ?
          Integer.MAX_VALUE : this.buffer.length * 2;
        char[] newbuffer = new char[newlen];
        System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
        this.buffer = newbuffer;
      }
      public void AppendChar(char c) {
        if (this.ptr < this.buffer.length) {
          this.buffer[this.ptr++] = c;
        } else {
          this.Grow();
          this.buffer[this.ptr++] = c;
        }
      }
    }

    private static void ReadHeaders(
      IByteReader stream,
      Collection<String> headerList,
      boolean start) {
      // Line length in OCTETS, not characters
      int lineCount = 0;
      int[] bytesRead = new int[1];
      StringBuilderKeepBuffer sb = new StringBuilderKeepBuffer();
      int ss = 0;
      boolean ungetLast = false;
      int lastByte = 0;
      while (true) {
        sb.Reset();
        boolean first = true;
        boolean endOfHeaders = false;
        boolean wsp = false;
        lineCount = 0;
        while (true) {
          int c = ungetLast ? lastByte : stream.read();
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
            if (stream.read() == '\n') {
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
            sb.AppendChar((char)c);
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
              boolean possibleMbox = true;
              boolean isFromField = false;
              sb.Reset();
              while (true) {
                c = stream.read();
                if (c == -1) {
                  throw new MessageDataException(
                    "Premature end before all headers were read (Mbox" +
"\u0020convention)");
                } else if (c == ':' && possibleMbox) {
                  // Full fledged From header field
                  isFromField = true;
                  sb.AppendChar('f');
                  sb.AppendChar('r');
                  sb.AppendChar('o');
                  sb.AppendChar('m');
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
        if (sb.isEmpty()) {
          throw new MessageDataException("Empty header field name");
        }
        // Set the header field name to the
        // String builder's current value
        String fieldName = sb.toString();
        // Clear the String builder to read the
        // header field's value
        sb.Reset();
        // Skip initial spaces in the header field value,
        // to keep them from being added by the String builder
        while (true) {
          lastByte = stream.read();
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
          int c = ungetLast ? lastByte : stream.read();
          ungetLast = false;
          if (c == -1) {
            String exstring = "Premature end before all headers were read," +
              " while reading header field value";
            throw new MessageDataException(exstring);
          }
          if (c == '\r') {
            // We're only looking for the single-byte LF, so
            // there's no need to use ReadUtf8Char
            c = stream.read();
            if (c < 0) {
              String exstring = "Premature end before all headers were read," +
                " while looking for LF";
              throw new MessageDataException(exstring);
            }
            if (c == '\n') {
              lineCount = 0;
              // Parse obsolete folding whitespace (obs-fws) under RFC5322
              // (parsed according to errata), same as LWSP in RFC5234
              boolean fwsFirst = true;
              boolean haveFWS = false;
              while (true) {
                // Skip the CRLF pair, if any (except if iterating for
                // the first time, since CRLF was already parsed)
                // Use ReadByte here since we're just looking for the single
                // byte characters CR and LF
                if (!fwsFirst) {
                  c = ungetLast ? lastByte : stream.read();
                  ungetLast = false;
                  if (c == '\r') {
                    c = stream.read();
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
                int c2 = ungetLast ? lastByte : stream.read();
                ungetLast = false;
                if (c2 == 0x20 || c2 == 0x09) {
                  ++lineCount;
                  // Don't write SPACE as the first character of the value
                  if (c2 != 0x20 || !sb.isEmpty()) {
                    sb.AppendChar((char)c2);
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
            sb.AppendChar('\r');
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
            sb.AppendChar((char)c);
            ++lineCount;
          } else {
            int[] state = { lineCount, c, 1 };
            c = ReadUtf8Char(stream, state);
            // System.out.println("c=" + c + "," + lineCount + "," +
            // state[0]+ ","+state[1]+","+state[2]);
            lineCount = state[0];
            ungetLast = state[2] == 1;
            lastByte = state[1];
            if (c <= 0xffff) {
              sb.AppendChar((char)c);
            } else if (c <= 0x10ffff) {
              sb.AppendChar((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
              sb.AppendChar((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
            }
          }
        }
        String fieldValue = sb.toString();
        headerList.add(fieldName);
        headerList.add(fieldValue);
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
        throw new NullPointerException("stream");
      }
      // NOTE: Currently assumes the last byte read
      // is 0x80 or greater (non-ASCII). This excludes
      // CR, which complicates a bit how line count in ungetState
      // is handled

      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xbf;
      int read = ungetState[0];
      while (true) {
        int b = ungetState[2] == 1 ?
          ungetState[1] : stream.read();
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

    private static boolean StartsWithWhitespace(String str) {
      return str.length() > 0 && (str.charAt(0) == ' ' || str.charAt(0) == 0x09 || str.charAt(0) ==
          '\r');
    }

    private static int TransferEncodingToUse(byte[] body, boolean isBodyPart) {
      if (body == null || body.length == 0) {
        return EncodingSevenBit;
      }
      int lengthCheck = Math.min(body.length, 1024);
      int highBytes = 0;
      int ctlBytes = 0;
      int lineLength = 0;
      // Assume 'allTextBytes' is false if this is a body part or not
      // all of the body is checked
      boolean allTextBytes = (!isBodyPart) && lengthCheck == body.length;
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
          if (i + 1 >= body.length || body[i + 1] != (byte)'\n') {
            // bare CR
            allTextBytes = false;
          } else if (i > 0 && (body[i - 1] == (byte)' ' || body[i - 1] ==
              (byte)'\t')) {
            // Space followed immediately by CRLF
            allTextBytes = false;
          } else if (i + 2 < body.length && body[i + 1] == (byte)'\n' &&
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
        allTextBytes &= lineLength != 0 || i >= body.length || body[i] !=
          '.';
        allTextBytes &= lineLength != 0 || i + 4 >= body.length || body[i] !=
          'F' || body[i + 1] != 'r' || body[i + 2] != 'o' || body[i + 3] !=
          'm' || (body[i + 4] != ' ' && body[i + 4] != '\t');
        allTextBytes &= lineLength != 0 || i + 1 >= body.length || body[i] !=
          '-' || body[i + 1] != '-';
        ++lineLength;
        allTextBytes &= lineLength <= MaxShortHeaderLineLength;
      }
      return allTextBytes ? EncodingSevenBit :
        ((highBytes > lengthCheck / 3) ? EncodingBase64 :
          EncodingQuotedPrintable);
    }

    private static String ValidateHeaderField(String name, String value) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (value == null) {
        throw new NullPointerException("value");
      }
      if (name.length() > MaxHardHeaderLineLength - 1) {
        throw new IllegalArgumentException("Header field name too long");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < name.length(); ++i) {
        if (name.charAt(i) <= 0x20 || name.charAt(i) == ':' || name.charAt(i) >= 0x7f) {
          throw new
          IllegalArgumentException("Header field name contains an invalid character");
        }
      }
      // Check characters in structured header fields
      IHeaderFieldParser parser = HeaderFieldParsers.GetParser(name);
      if (parser.IsStructured()) {
        if (ParseUnstructuredText(value, 0, value.length()) != value.length()) {
          throw new IllegalArgumentException("Header field value contains invalid" +
            "\u0020text");
        }
        if (parser.Parse(value, 0, value.length(), null) != value.length()) {
          throw new
          IllegalArgumentException("Header field value is not in the correct format");
        }
      }
      return name;
    }

    private void Generate(IWriter output, int depth) {
      boolean haveMimeVersion = false;
      boolean haveContentEncoding = false;
      boolean haveContentType = false;
      boolean haveContentDisp = false;
      boolean haveMsgId = false;
      boolean haveFrom = false;
      boolean haveDate = false;
      boolean[] haveHeaders = new boolean[11];
      byte[] bodyToWrite = this.body;
      MediaTypeBuilder builder = new MediaTypeBuilder(this.getContentType());
      String contentDisp = (this.getContentDisposition() == null) ? null :
        this.getContentDisposition().toString();
      int transferEnc = 0;
      boolean isMultipart = builder.getTopLevelType().equals(
        "multipart");
      String boundary = "";
      if (isMultipart) {
        boundary = GenerateBoundary(depth);
        builder.SetParameter(
          "boundary",
          boundary);
      } else {
        if (builder.getTopLevelType().equals("message")) {
          if (builder.getSubType().equals("delivery-status") ||
            builder.getSubType().equals("global-delivery-status") ||
            builder.getSubType().equals("disposition-notification") ||
            builder.getSubType().equals("global-disposition-notification")) {
            bodyToWrite = DowngradeDeliveryStatus(bodyToWrite);
          }
          boolean msgCanBeUnencoded = CanBeUnencoded(bodyToWrite, depth > 0);
          if ((builder.getSubType().equals("rfc822") ||
              builder.getSubType().equals(
                "news")) && !msgCanBeUnencoded) {
            builder.SetSubType("global");
          } else if (builder.getSubType().equals("disposition-notification") && !msgCanBeUnencoded) {
            builder.SetSubType("global-disposition-notification");
          } else if (builder.getSubType().equals("delivery-status") && !msgCanBeUnencoded) {
            builder.SetSubType("global-delivery-status");
          } else if (!msgCanBeUnencoded && !builder.getSubType().equals("global") &&
            !builder.getSubType().equals("global-disposition-notification") &&
            !builder.getSubType().equals("global-delivery-status") && !builder.getSubType().equals(
                "global-headers")) {
          }
        }
      }
      String topLevel = builder.getTopLevelType();
      // NOTE: RFC 6532 allows any transfer encoding for the
      // four global message types listed below.
      transferEnc = topLevel.equals("message") ||
        topLevel.equals("multipart") ?
(topLevel.equals("multipart") || (
            !builder.getSubType().equals("global") &&
            !builder.getSubType().equals("global-headers") &&
            !builder.getSubType().equals("global-disposition-notification") &&
            !builder.getSubType().equals("global-delivery-status"))) ? EncodingSevenBit :
TransferEncodingToUse(
          bodyToWrite,
          depth > 0) : TransferEncodingToUse(bodyToWrite, depth > 0);
      String encodingString = "7bit";
      if (transferEnc == EncodingBase64) {
        encodingString = "base64";
      } else if (transferEnc == EncodingQuotedPrintable) {
        encodingString = "quoted-printable";
      }
      // Write the header fields
      for (int i = 0; i < this.headers.size(); i += 2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        String rawField = null;
        if (name.equals("content-type")) {
          if (haveContentType) {
            // Already outputted, continue
            continue;
          }
          haveContentType = true;
          value = builder.toString();
        }
        if (name.equals("content-disposition")) {
          if (haveContentDisp || contentDisp == null) {
            // Already outputted, continue
            continue;
          }
          haveContentDisp = true;
          value = contentDisp;
        } else if (name.equals("content-transfer-encoding")) {
          if (haveContentEncoding) {
            // Already outputted, continue
            continue;
          }
          haveContentEncoding = true;
          value = encodingString;
        } else if (name.equals("date")) {
          if (haveDate) {
            continue;
          }
          haveDate = true;
        } else if (name.equals("from")) {
          if (haveFrom) {
            // Already outputted, continue
            continue;
          }
          haveFrom = true;
        }
        if (
          depth > 0 &&
          name.length() >= 2 && name.charAt(0) == '-' && name.charAt(1) == '-') {
          // don't generate header fields starting with "--"
          // in body parts
          continue;
        }
        if (name.equals("mime-version")) {
          if (depth > 0) {
            // Don't output if this is a body part
            continue;
          }
          haveMimeVersion = true;
        } else if (name.equals("message-id")) {
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
          if (ValueHeaderIndices.containsKey(name)) {
            if (depth > 0) {
              // Don't output if this is a body part
              continue;
            }
            int headerIndex = ValueHeaderIndices.get(name);
            if (headerIndex <= 5) {
              if (haveHeaders[headerIndex]) {
                // Already outputted, continue
                continue;
              }
              boolean isValidAddressing = this.IsValidAddressingField(name);
              haveHeaders[headerIndex] = true;
              /*

              */ if (!isValidAddressing) {
                value = "";
                if (!name.equals("from") &&
                  !name.equals("sender")) {
                  value = GenerateAddressList(
                      NamedAddress.ParseAddresses(value));
                }
                if (value.length() == 0) {
                  // Synthesize a field
                  rawField = this.SynthesizeField(name);
                }
              }
            } else if (headerIndex <= 10) {
              // Resent-* fields can appear more than once
              value = GenerateAddressList(
                  NamedAddress.ParseAddresses(value));
              if (value.length() == 0) {
                // No addresses, synthesize a field
                rawField = this.SynthesizeField(name);
              }
            }
          }
        }
        rawField = (rawField == null) ? (HeaderEncoder.EncodeField(name, value)) : rawField;
        if (HeaderEncoder.CanOutputRaw(rawField)) {
          AppendAscii(output, rawField);
        } else {
          // System.out.println("Can't output '"+name+"' raw");
          String downgraded = HeaderFieldParsers.GetParser(name)
            .DowngradeHeaderField(name, value);
          if (
            HasTextToEscape(
              downgraded,
              0,
              downgraded.length())) {
            if (name.equals("message-id") ||
                         name.equals("resent-message-id") ||
                         name.equals("in-reply-to") ||
                         name.equals("references") ||
                         name.equals(
                         "original-recipient") ||
                         name.equals("final-recipient")) {
              // Header field still contains invalid characters (such
              // as non-ASCII characters in 7-bit messages), convert
              // to a downgraded field
              downgraded = HeaderEncoder.EncodeFieldAsEncodedWords(
                  "downgraded-" + name,
                  ParserUtility.TrimSpaceAndTab(value));
            } else {
            }
          } else {
            AppendAscii(
              output,
              downgraded);
          }
        }
        AppendAscii(
          output,
          "\r\n");
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
        String dateString = MailDateTime.GenerateDateString(
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
        AppendAscii(
          output,
          "\r\n");
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
      boolean isText = builder.getTopLevelType().equals("text");
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
        int index = 0;
        while (true) {
          int c = (index >= bodyToWrite.length) ? -1 :
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
        boolean writeNewLine = depth > 0;
        for (Message part : this.getParts()) {
          if (writeNewLine) {
            AppendAscii(output, "\r\n");
          }
          writeNewLine = true;
          AppendAscii(
            output,
            "--" + boundary + "\r\n");
          part.Generate(output, depth + 1);
        }
        AppendAscii(
          output,
          "\r\n--" + boundary + "--");
      }
    }

    private String GenerateMessageID() {
      StringBuilder builder = new StringBuilder();
      int seq = 0;
      builder.append('<');
      synchronized (ValueSequenceSync) {
        if (seqFirstTime) {
          msgidSequence = ValueMsgidRandom.nextInt(65536);
          msgidSequence <<= 16;
          msgidSequence |= ValueMsgidRandom.nextInt(65536);
          seqFirstTime = false;
        }
        seq = (msgidSequence++);
      }
      String ValueHex = "0123456789abcdef";
      byte[] ent;
      {
        ent = new byte[16];
        for (int i = 0; i < ent.length; ++i) {
          ent[i] = ((byte)ValueMsgidRandom.nextInt(256));
        }
      }
      long ticks = new java.util.Date().getTime();
      for (int i = 0; i < 10; ++i) {
        builder.append(ValueHex.charAt((int)(ticks & 15)));
        ticks >>= 4;
      }
      for (int i = 0; i < ent.length; ++i) {
        builder.append(ValueHex.charAt((int)(ent[i] & 15)));
        builder.append(ValueHex.charAt((int)((ent[i] >> 4) & 15)));
      }
      for (int i = 0; i < 8; ++i) {
        builder.append(ValueHex.charAt(seq & 15));
        seq >>= 4;
      }
      List<NamedAddress> addresses = this.GetAddresses("from");
      if (addresses == null || addresses.size() == 0) {
        builder.append("@local.invalid");
      } else {
        builder.append('@');
        seq = addresses.get(0).isGroup() ? addresses.get(0).getName().hashCode() :
          addresses.get(0).getAddress().toString().hashCode();
        for (int i = 0; i < 8; ++i) {
          builder.append(ValueHex.charAt(seq & 15));
          seq >>= 4;
        }
        builder.append(".local.invalid");
      }
      builder.append('>');
      // System.out.println(builder.toString());
      return builder.toString();
    }

    private String[] GetMultipleHeaders(String name) {
      ArrayList<String> headerList = new ArrayList<String>();
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          headerList.add(this.headers.get(i + 1));
        }
      }
      return headerList.toArray(new String[] { });
    }

    private boolean IsValidAddressingField(String name) {
      name = DataUtilities.ToLowerCaseAscii(name);
      boolean have = false;
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          if (have) {
            return false;
          }
          String headerValue = this.headers.get(i + 1);
          if (
            HeaderFieldParsers.GetParser(
              name).Parse(
              headerValue,
              0,
              headerValue.length(),
              null) != headerValue.length()) {
            return false;
          }
          have = true;
        }
      }
      return true;
    }

    /**
     * Creates a message object from a MailTo URI (uniform resource identifier).
     * For more information, see <b>FromMailtoUri(string)</b>.
     * @param url A MailTo URI.
     * @return A Message object created from the given MailTo URI. Returs null if
     * {@code url} is null, is syntactically invalid, or is not a MailTo
     * URI.
     * @deprecated Renamed to FromMailtoUri.
 */
@Deprecated

    public static Message FromMailtoUrl(String url) {
      return MailtoUris.MailtoUriMessage(url);
    }

    /**
     * Generates a MailTo URI (uniform resource identifier) corresponding to this
     * message. The following header fields, and only these, are used to
     * generate the URI: To, Cc, Bcc, In-Reply-To, Subject, Keywords,
     * Comments. The message body is included in the URI only if
     * <code>GetBodyString()</code> would return a non-empty string. The To
     * header field is included in the URI only if it has display names or
     * group syntax.
     * @return A MailTo URI corresponding to this message.
     * @deprecated Renamed to ToMailtoUri.
 */
@Deprecated

    public String ToMailtoUrl() {
      return MailtoUris.MessageToMailtoUri(this);
    }

    /**
     * Creates a message object from a MailTo URI (uniform resource identifier).
     * The MailTo URI can contain key-value pairs that follow a
     * question-mark, as in the following example:
     *  "mailto:me@example.com?subject=A%20Subject". In this example,
     *  "subject" is the subject of the email address. Only certain keys are
     *  supported, namely, "to", "cc", "bcc", "subject", "in-reply-to",
     *  "comments", "keywords", and "body". The first seven are header field
     * names that will be used to set the returned message's corresponding
     *  header fields. The last, "body", sets the body of the message to the
     * given text. Keys other than these eight will be ignored. (Keys are
     * compared using a basic case-sensitive comparison, in which two
     * strings are equal if they match after converting the basic
     * upper-case letters A to Z (U+0041 to U+005A) in both strings to
     * basic lower-case letters.) The same key (matched using a basic
     * case-insensitive comparison) can appear more than once; for
     *  "subject", "cc", "bcc", and "in-reply-to", the last value with the
     *  given key is used; for "to", all header field values as well as the
     *  path are combined to a single To header field; for "keywords" and
     *  "comments", each value adds another header field of the given key;
     *  and for "body", the last value with that key is used as the body.
     * @param uri The parameter {@code uri} is a text string.
     * @return A Message object created from the given MailTo URI. Returs null if
     * {@code uri} is null, is syntactically invalid, or is not a MailTo
     * URI.
     */
    public static Message FromMailtoUri(String uri) {
      return MailtoUris.MailtoUriMessage(uri);
    }

    /**
     * Creates a message object from a MailTo URI (uniform resource identifier) in
     * the form of a URI object. For more information, see
     * <b>FromMailtoUri(string)</b>.
     * @param uri The MailTo URI in the form of a URI object.
     * @return A Message object created from the given MailTo URI. Returs null if
     * {@code uri} is null, is syntactically invalid, or is not a MailTo
     * URI.
     * @throws NullPointerException The parameter {@code uri} is null.
     */
    public static Message FromMailtoUri(java.net.URI uri) {
      if (uri == null) {
        throw new NullPointerException("uri");
      }
      return MailtoUris.MailtoUriMessage(uri.toString());
    }

    /**
     * Generates a MailTo URI (uniform resource identifier) corresponding to this
     * message. The following header fields, and only these, are used to
     * generate the URI: To, Cc, Bcc, In-Reply-To, Subject, Keywords,
     * Comments. The message body is included in the URI only if
     * <code>GetBodyString()</code> would return a non-empty string.. The To
     * header field is included in the URI only if it has display names or
     * group syntax.
     * @return A MailTo URI corresponding to this message.
     */
    public String ToMailtoUri() {
      return MailtoUris.MessageToMailtoUri(this);
    }

    private void ProcessHeaders(boolean assumeMime, boolean digest) {
      boolean haveContentType = false;
      boolean mime = assumeMime;
      boolean haveContentDisp = false;
      String transferEncodingValue = "";
      for (int i = 0; i < this.headers.size(); i += 2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (name.equals("content-transfer-encoding")) {
          int startIndex = HeaderParser.ParseCFWS(
            value,
            0,
            value.length(),
            null);
          // NOTE: Actually "token", but all known transfer encoding values
          // fit the same syntax as the stricter one for top-level types and
          // subtypes
          int endIndex = MediaType.SkipMimeTypeSubtype(
            value,
            startIndex,
            value.length(),
            null);
          transferEncodingValue = (
              HeaderParser.ParseCFWS(
                value,
                endIndex,
                value.length(),
                null) == value.length()) ? value.substring(
              startIndex, (
              startIndex)+(endIndex - startIndex)) : "";
        }
        mime |= name.equals("mime-version");
        if (value.indexOf("=?") >= 0) {
          IHeaderFieldParser parser = HeaderFieldParsers.GetParser(name);
          // Decode encoded words in the header field where possible
          value = parser.DecodeEncodedWords(value);
          this.headers.set(i + 1, value);
        }
      }
      MediaType ctype = digest ? MediaType.MessageRfc822 :
        MediaType.TextPlainAscii;
      boolean haveInvalid = false;
      boolean haveContentEncoding = false;
      for (int i = 0; i < this.headers.size(); i += 2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (mime && name.equals("content-transfer-encoding")) {
          value = DataUtilities.ToLowerCaseAscii(transferEncodingValue);
          this.headers.set(i + 1, value);
          if (value.equals("7bit")) {
            this.transferEncoding = EncodingSevenBit;
          } else if (value.equals("8bit")) {
            this.transferEncoding = EncodingEightBit;
          } else if (value.equals("binary")) {
            this.transferEncoding = EncodingBinary;
          } else if (value.equals("quoted-printable")) {
            this.transferEncoding = EncodingQuotedPrintable;
          } else if (value.equals("base64")) {
            this.transferEncoding = EncodingBase64;
          } else {
            // Unrecognized transfer encoding
            // System.out.println("unrecognized: " + value);
            this.transferEncoding = EncodingUnknown;
          }
          haveContentEncoding = true;
        } else if (mime && name.equals("content-type")) {
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
            if (ctype.isText()) {
              if (((ctype.GetCharset()) == null || (ctype.GetCharset()).length() == 0)) {
                // If we get here, then either:
                // - The charset is present but unrecognized or empty, or
                // - The charset is absent and the media type has
                // no default charset assumed for it.
                Map<String, String> parameters = ctype.getParameters();
                if (parameters.containsKey("charset")) {
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
                MediaTypeBuilder builder = new MediaTypeBuilder(ctype)
                .SetParameter("charset", ctype.GetCharset());
                ctype = builder.ToMediaType();
              }
            }
            haveContentType = true;
          }
        } else if (mime && name.equals("content-disposition")) {
          if (haveContentDisp) {
            String valueExMessage = "Already have this header: " + name;

            throw new MessageDataException(valueExMessage);
          }
          this.contentDisposition = ContentDisposition.Parse(value);
          haveContentDisp = true;
        }
      }
      if (this.transferEncoding == EncodingUnknown) {
        ctype = MediaType.ApplicationOctetStream;
      }
      if (!haveContentEncoding && ctype.getTypeAndSubType().equals(
        "message/rfc822")) {
        // DEVIATION: Be a little more liberal with rfc822
        // messages with bytes greater than 127
        this.transferEncoding = EncodingEightBit;
      }
      if (this.transferEncoding == EncodingQuotedPrintable ||
        this.transferEncoding == EncodingBase64) {
        if (ctype.isMultipart() || (ctype.getTopLevelType().equals("message") && !ctype.getSubType().equals("global") && !ctype.getSubType().equals(
                "global-headers") && !ctype.getSubType().equals(
                "global-disposition-notification") &&
            !ctype.getSubType().equals("global-delivery-status"))) {
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
        String charset = this.contentType.GetCharset();
        if (charset.equals("utf-8")) {
          // DEVIATION: Be a little more liberal with UTF-8
          this.transferEncoding = EncodingEightBit;
        } else if (ctype.getTypeAndSubType().equals("text/html")) {
          if (charset.equals("us-ascii") ||
            charset.equals("windows-1252") ||
            charset.equals("windows-1251") ||
(charset.length() > 9 && charset.substring(0, 9).equals(
              "iso-8859-"))) {
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
        this.ProcessHeaders(
          false,
          false);
        if (this.contentType.isMultipart()) {
          this.ReadMultipartBody(stream);
        } else {
          this.ReadSimpleBody(stream);
        }
      } catch (IllegalStateException ex) {
        throw new MessageDataException(ex.getMessage(), ex);
      }
    }

    private void ReadMultipartBody(IByteReader stream) {
      int baseTransferEncoding = this.transferEncoding;
      List<MessageStackEntry> multipartStack = new ArrayList<MessageStackEntry>();
      MessageStackEntry entry = new MessageStackEntry(this);
      multipartStack.add(entry);
      BoundaryCheckerTransform boundaryChecker = new BoundaryCheckerTransform(
        stream,
        entry.getBoundary());
      // Be liberal on the preamble and epilogue of multipart
      // messages, as they will be ignored.
      IByteReader currentTransform = MakeTransferEncoding(
        boundaryChecker,
        baseTransferEncoding,
        true);
      Message leaf = null;
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.length;
      this.body = new byte[0];
      ArrayWriter aw = new ArrayWriter();
      {
        while (true) {
          int ch = 0;
          try {
            ch = currentTransform.read();
          } catch (MessageDataException ex) {
            String valueExMessage = ex.getMessage();

            throw new MessageDataException(valueExMessage);
          }
          if (ch < 0) {
            if (boundaryChecker.getHasNewBodyPart()) {
              Message msg = NewBodyPart();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack

              if (leaf != null) {
                if (bufferCount > 0) {
                  aw.write(buffer, 0, bufferCount);
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
              while (multipartStack.size() > stackCount) {
                multipartStack.remove(stackCount);
              }
              Message parentMessage = multipartStack.get(
                  multipartStack.size() - 1).getMessage();
              boundaryChecker.StartBodyPartHeaders();
              MediaType ctype = parentMessage.getContentType();
              boolean parentIsDigest = ctype.getSubType().equals("digest") && ctype.isMultipart();
              ReadHeaders(stream, msg.headers, false);
              msg.ProcessHeaders(
                true,
                parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.getParts().add(msg);
              multipartStack.add(entry);
              aw.Clear();
              ctype = msg.getContentType();
              leaf = ctype.isMultipart() ? null : msg;
              boundaryChecker.EndBodyPartHeaders(entry.getBoundary());
              boolean isTextPlain = ctype.getTypeAndSubType().equals(
                "text/plain");
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                isTextPlain);
            } else {
              // All body parts were read
              if (leaf != null) {
                if (bufferCount > 0) {
                  aw.write(buffer, 0, bufferCount);
                  bufferCount = 0;
                }
                leaf.body = aw.ToArray();
              }
              return;
            }
          } else {
            buffer[bufferCount++] = (byte)ch;
            if (bufferCount >= bufferLength) {
              aw.write(buffer, 0, bufferCount);
              bufferCount = 0;
            }
          }
        }
      }
    }

    private void ReadSimpleBody(IByteReader stream) {
      boolean isTextPlain = this.getContentType().getTypeAndSubType().equals("text/plain");
      IByteReader transform = MakeTransferEncoding(
        stream,
        this.transferEncoding,
        isTextPlain);
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.length;
      ArrayWriter aw = new ArrayWriter();
      {
        while (true) {
          int ch = 0;
          try {
            ch = transform.read();
          } catch (MessageDataException ex) {
            String valueExMessage = ex.getMessage();

            throw new MessageDataException(valueExMessage, ex);
          }
          if (ch < 0) {
            break;
          }
          buffer[bufferCount++] = (byte)ch;
          if (bufferCount >= bufferLength) {
            aw.write(buffer, 0, bufferCount);
            bufferCount = 0;
          }
        }
        if (bufferCount > 0) {
          aw.write(buffer, 0, bufferCount);
        }
        this.body = aw.ToArray();
      }
    }

    private String SynthesizeField(String name) {
      HeaderEncoder encoder = new HeaderEncoder(76, 0).AppendFieldName(name);
      String fullField = ParserUtility.Implode(
          this.GetMultipleHeaders(name),
          "\u002c ");
      String lcname = DataUtilities.ToLowerCaseAscii(name);
      if (fullField.length() == 0) {
        encoder.AppendSymbol("me@" + name + "-address.invalid");
      } else {
        encoder.AppendAsEncodedWords(fullField);
        encoder.AppendSpace();
        encoder.AppendSymbol("<me@" + name + "-address.invalid>");
      }
      return encoder.toString();
    }

    private static class MessageStackEntry {
      /**
       * Gets a value which is used in an internal API.
       * @return This is an internal API.
       */
      public final Message getMessage() { return propVarmessage; }
private final Message propVarmessage;

      /**
       * Gets a value which is used in an internal API.
       * @return This is an internal API.
       */
      public final String getBoundary() { return propVarboundary; }
private final String propVarboundary;

      public MessageStackEntry(Message msg) {
        String newBoundary = "";
        MediaType mediaType = msg.getContentType();
        if (mediaType.isMultipart()) {
          newBoundary = mediaType.GetParameter("boundary");
          if (newBoundary == null) {
            throw new
            MessageDataException("Multipart message has no boundary defined");
          }
          if (!IsWellFormedBoundary(newBoundary)) {
            throw new
            MessageDataException("Multipart message has an invalid boundary" +
"\u0020defined: " +
              newBoundary);
          }
        }
        this.propVarmessage = msg;
        this.propVarboundary = newBoundary;
      }
    }
  }
