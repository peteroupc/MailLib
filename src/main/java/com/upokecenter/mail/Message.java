package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
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
     * <p>Represents an email message, and contains methods and properties for
     * accessing and modifying email message data. This class implements the
     * Internet Message Format (RFC 5322) and Multipurpose Internet Mail
     * Extensions (MIME; RFC 2045-2047, RFC 2049).</p> <p><b>Thread
     * safety:</b> This class is mutable; its properties can be changed.
     * None of its instance methods are designed to be thread safe.
     * Therefore, access to objects from this class must be synchronized if
     * multiple threads can access them at the same time.</p> <p>The
     * following lists known deviations from the mail specifications
     * (Internet Message Format and MIME):</p> <ul> <li>The
     * content-transfer-encodings "quoted-printable" and "base64" are
     * treated as 7bit instead if they occur in a message or body part with
     * content type "multipart/*" or "message/*" (other than
     * "message/global", "message/global-headers",
     * "message/global-disposition-notification", or
     * "message/global-delivery-status").</li> <li>If a message has two or
     * more Content-Type header fields, it is treated as having a content
     * type of "application/octet-stream", unless one or more of the header
     * fields is syntactically invalid.</li> <li>Illegal UTF-8 byte
     * sequences appearing in header field values are replaced with
     * replacement characters. Moreover, UTF-8 is parsed everywhere in
     * header field values, even in those parts of some structured header
     * fields where this appears not to be allowed. (UTF-8 is a character
     * encoding for the Unicode character set.)</li> <li>This implementation
     * can parse a message even if that message is without a From header
     * field, without a Date header field, or without both.</li> <li>The To
     * and Cc header fields are allowed to contain only comments and
     * whitespace, but these "empty" header fields will be omitted when
     * generating.</li> <li>There is no line length limit imposed when
     * parsing header fields, except header field names.</li> <li>There is
     * no line length limit imposed when parsing quoted-printable or base64
     * encoded bodies.</li> <li>If the transfer encoding is absent and the
     * content type is "message/rfc822", bytes with values greater than 127
     * (called "8-bit bytes" in the rest of this summary) are still allowed,
     * despite the default value of "7bit" for
     * "Content-Transfer-Encoding".</li> <li>In the following cases, if the
     * transfer encoding is absent, declared as 7bit, or treated as 7bit,
     * 8-bit bytes are still allowed:</li> <li>(a) The preamble and epilogue
     * of multipart messages, which will be ignored.</li> <li>(b) If the
     * charset is declared to be <code>utf-8</code>.</li> <li>(c) If the content
     * type is "text/html" and the charset is declared to be
     * <code>us-ascii</code>, "windows-1252", "windows-1251", or "iso-8859-*" (all
     * single byte encodings).</li> <li>(d) In non-MIME message bodies and
     * in text/plain message bodies. Any 8-bit bytes are replaced with the
     * substitute character byte (0x1a).</li> <li>If the message starts with
     * the word "From" (and no other case variations of that word) followed
     * by one or more space (U + 0020) not followed by colon, that text and
     * the rest of the text is skipped up to and including a line feed
     * (U+000A). (See also RFC 4155, which describes the so-called "mbox"
     * convention with "From" lines of this kind.)</li> <li>The name
     * <code>ascii</code> is treated as a synonym for <code>us-ascii</code>, despite
     * being a reserved name under RFC 2046. The name <code>cp1252</code> is
     * treated as a synonym for <code>windows-1252</code> , even though it's not
     * an IANA registered alias.</li> <li>The following deviations involve
     * encoded words under RFC 2047:</li> <li>(a) If a sequence of encoded
     * words decodes to a string with a CTL character (U + 007F, or a
     * character less than U + 0020 and not TAB) after being converted to
     * Unicode, the encoded words are left un-decoded.</li> <li>(b) This
     * implementation can decode encoded words regardless of the character
     * length of the line in which they appear. This implementation can
     * generate a header field line with one or more encoded words even if
     * that line is more than 76 characters long. (This implementation
     * follows the recommendation in RFC 5322 to limit header field lines to
     * no more than 78 characters, where possible.)</li></ul> <p>It would be
     * appreciated if users of this library contact the author if they find
     * other ways in which this implementation deviates from the mail
     * specifications or other applicable specifications.</p> <p>Note that
     * this class currently doesn't support the "padding" parameter for
     * message bodies with the media type "application/octet-stream" or
     * treated as that media type (see RFC 2046 sec. 4.5.1).</p> <p>Note
     * that this implementation can decode an RFC 2047 encoded word that
     * uses ISO-2022-JP (the only supported encoding that uses code
     * switching) even if the encoded word's payload ends in a different
     * mode from "ASCII mode". (Each encoded word still starts in "ASCII
     * mode", though.) This, however, is not a deviation to RFC 2047 because
     * the relevant rule only concerns bringing the output device back to
     * "ASCII mode" after the decoded text is displayed (see last paragraph
     * of sec. 6.2) -- since the decoded text is converted to Unicode rather
     * than kept as ISO-2022-JP, this is not applicable since there is no
     * such thing as "ASCII mode" in the Unicode Standard.</p> <p>Note that
     * this library (the MailLib library) has no facilities for sending and
     * receiving email messages, since that's outside this library's
     * scope.</p>
     */
  public final class Message {
    static final int MaxRecHeaderLineLength = 78;
    static final int MaxShortHeaderLineLength = 76;
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
     * class. Reads from the given InputStream object to initialize the message.
     * @param stream A readable data stream.
     * @throws java.lang.NullPointerException The parameter {@code stream} is null.
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

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.Message}
     * class. Reads from the given byte array to initialize the message.
     * @param bytes A readable data stream.
     * @throws java.lang.NullPointerException The parameter {@code bytes} is null.
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
      this.contentType = MediaType.TextPlainUtf8;
      this.headers.add("message-id");
      this.headers.add(this.GenerateMessageID());
      this.headers.add("from");
      this.headers.add("me@from-address.invalid");
      this.headers.add("mime-version");
      this.headers.add("1.0");
    }

    /**
     * Not documented yet.
     * @return This object.
     */
    public static Message NewBodyPart() {
      Message msg = new Message();
      msg.contentType = MediaType.TextPlainAscii;
      // No headers by default (see RFC 2046 sec. 5.1)
      msg.headers.clear();
      return msg;
    }

    /**
     * Sets this message's Date header field to the current time as its value.
     * <p>This method can be used when the message is considered complete
     * and ready to be generated, for example, using the "Generate()"
     * method.</p>
     * @return This object.
     */
    public Message SetCurrentDate() {
      return this.SetDate(DateTimeUtilities.GetCurrentLocalTime());
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
        return ParseAddresses(this.GetMultipleHeaders("bcc"));
      }

    /**
     * Gets the body of this message as a text string.
     * @return The body of this message as a text string.
     * @throws UnsupportedOperationException Either this message is a multipart
     * message, so it doesn't have its own body text, or this message has no
     * character encoding declared or assumed for it (which is usually the
     * case for non-text messages), or the character encoding is not
     * supported.
     */
    public final String getBodyString() {
        if (this.getContentType().isMultipart()) {
          throw new

  UnsupportedOperationException("This is a multipart message, so it doesn't have its own body text.");
        }
        ICharacterEncoding charset = Encodings.GetEncoding(
          this.getContentType().GetCharset(),
          true);
        if (charset == null) {
          throw new
            UnsupportedOperationException("Not in a supported character encoding.");
        }
        return Encodings.DecodeToString(
          charset,
          DataIO.ToReader(this.body));
      }

    /**
     * Gets a list of addresses found in the CC header field or fields.
     * @return A list of addresses found in the CC header field or fields.
     * @deprecated Use GetAddresses(\Cc\) instead.
 */
@Deprecated
    public final List<NamedAddress> getCCAddresses() {
        return ParseAddresses(this.GetMultipleHeaders("cc"));
      }

    /**
     * Gets this message's content disposition. The content disposition specifies
     * how a user agent should display or otherwise handle this message. Can
     * be set to null. If set to a disposition or to null, updates the
     * Content-Disposition header field as appropriate.
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
     * @throws java.lang.NullPointerException This value is being set and "value" is
     * null.
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
          this.SetHeader("content-type", this.contentType.toString());
        }
      }

    /**
     * <p>Gets a file name suggested by this message for saving the message's body
     * to a file. For more information on the algorithm, see
     * ContentDisposition.MakeFilename.</p> <p>This method generates a file
     * name based on the <code>filename</code> parameter of the
     * Content-Disposition header field, if it exists, or on the <code>name</code>
     * parameter of the Content-Type header field, otherwise.</p>
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
     * Not documented yet.
     * @param headerName The parameter {@code headerName} is not documented yet.
     * @return A list of addresses, in the order in which they appear in this
     * message's header fields of the given name.
     */
    public List<NamedAddress> GetAddresses(String headerName) {
      if ((headerName) == null) {
        throw new NullPointerException("headerName");
      }
      if ((headerName).length == 0) {
        throw new IllegalArgumentException("headerName" + " is empty.");
      }
      headerName = DataUtilities.ToLowerCaseAscii(headerName);
      if (ValueHeaderIndices.containsKey(headerName) &&
         ValueHeaderIndices.get(headerName) <= 5) {
        // TODO: Maybe support Resent-*
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
        return ParseAddresses(this.GetMultipleHeaders("from"));
      }

    /**
     * Gets a snapshot of the header fields of this message, in the order in which
     * they appear in the message. For each item in the list, the key is the
     * header field's name (where any basic upper-case letters [U+0041 to
     * U + 005A] are converted to lower case) and the value is the header
     * field's value.
     * @return A snapshot of the header fields of this message.
     */
    public final List<Map.Entry<String, String>> getHeaderFields() {
        ArrayList<Map.Entry<String, String>> list = new ArrayList<Map.Entry<String, String>>();
        for (int i = 0; i < this.headers.size(); i += 2) {
          list.add(
            new AbstractMap.SimpleImmutableEntry<String, String>(
              this.headers.get(i),
              this.headers.get(i + 1)));
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
     * Gets this message's subject.
     * @return This message's subject.
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
        return ParseAddresses(this.GetMultipleHeaders("to"));
      }

    /**
     * Adds a header field to the end of the message's header. <p>Updates the
     * ContentType and ContentDisposition properties if those header fields
     * have been modified by this method.</p>
     * @param header A key/value pair. The key is the name of the header field,
     * such as "From" or "Content-ID". The value is the header field's
     * value.
     * @return This instance.
     * @throws java.lang.NullPointerException The key or value of {@code header} is
     * null.
     * @throws IllegalArgumentException The header field name is too long or
     * contains an invalid character, or the header field's value is
     * syntactically invalid.
     */
    public Message AddHeader(Map.Entry<String, String> header) {
      return this.AddHeader(header.getKey(), header.getValue());
    }

    /**
     * Adds a header field to the end of the message's header. <p>Updates the
     * ContentType and ContentDisposition properties if those header fields
     * have been modified by this method.</p>
     * @param name Name of a header field, such as "From" or "Content-ID".
     * @param value Value of the header field.
     * @return This instance.
     * @throws java.lang.NullPointerException The parameter {@code name} or {@code
     * value} is null.
     * @throws IllegalArgumentException The header field name is too long or
     * contains an invalid character, or the header field's value is
     * syntactically invalid.
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
     * have only Basic Latin code points (U + 0000 to U + 007F), and the
     * transfer encoding will always be 7bit, quoted-printable, or base64
     * (the declared transfer encoding for this message will be
     * ignored).</p> <p>The following applies to the following header
     * fields: From, To, Cc, Bcc, Reply-To, Sender, Resent-To, Resent-From,
     * Resent-Cc, Resent-Bcc, and Resent-Sender. If the header field exists,
     * but has an invalid syntax, has no addresses, or appears more than
     * once, this method will generate a synthetic header field with the
     * display-name set to the contents of all of the header fields with the
     * same name, and the address set to
     * <code>me@[header-name]-address.invalid</code> as the address (a
     * <code>.invalid</code> address is a reserved address that can never belong
     * to anyone). (An exception is that the Resent-* header fields may
     * appear more than once.) The generated message should always have a
     * From header field.</p> <p>If a Date and/or Message-ID header field
     * doesn't exist, a field with that name will be generated (using the
     * current local time for the Date field).</p> <p>When encoding the
     * message's body, if the message has a text content type ("text/*"),
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
      this.Generate(aw, 0);
      return DataUtilities.GetUtf8String(aw.ToArray(), false);
    }

    /**
     * Generates this message's data as a byte array, using the same algorithm as
     * the Generate method.
     * @return The generated message as a byte array.
     */
    public byte[] GenerateBytes() {
      ArrayWriter aw = new ArrayWriter();
      this.Generate(aw, 0);
      return aw.ToArray();
    }

    /**
     * Gets the byte array for this message's body. This method doesn't make a copy
     * of that byte array.
     * @return A byte array.
     */
    public byte[] GetBody() {
      return this.body;
    }

    /**
     * Gets the date and time extracted from this message's Date header field (the
     * value of which is found as though GetHeader("date") were called). See
     * <see
  * cref='M:PeterO.Mail.MailDateTime.ParseDateString(System.String,System.Boolean)'/>
     * for more information on the format of the date-time array returned by
     * this method.
     * @return An array containing eight elements. Returns null if the Date header
     * doesn't exist, if the Date field is syntactically or semantically
     * invalid, or if the field's year would overflow a 32-bit signed
     * integer.
     */
    public int[] GetDate() {
      String field = this.GetHeader("date");
      return (field == null) ? (null) : (MailDateTime.ParseDateString(field,
             true));
    }

    /**
     * Sets this message's Date header field to the given date and time.
     * @param dateTime An array containing eight elements. Each element of the
     * array (starting from 0) is as follows: <ul> <li>0 - The year. For
     * example, the value 2000 means 2000 C.E.</li> <li>1 - Month of the
     * year, from 1 (January) through 12 (December).</li> <li>2 - Day of the
     * month, from 1 through 31.</li> <li>3 - Hour of the day, from 0
     * through 23.</li> <li>4 - Minute of the hour, from 0 through 59.</li>
     * <li>5 - Second of the minute, from 0 through 60 (this value can go up
     * to 60 to accommodate leap seconds). (Leap seconds are additional
     * seconds added to adjust international atomic time, or TAI, to an
     * approximation of astronomical time known as coordinated universal
     * time, or UTC.)</li> <li>6 - Milliseconds of the second, from 0
     * through 999. This value is not used to generate the date string, but
     * must still be valid.</li> <li>7 - Number of minutes to subtract from
     * this date and time to get global time. This number can be positive or
     * negative.</li></ul>.
     * @return This object.
     * @throws IllegalArgumentException The parameter {@code dateTime} contains
     * fewer than eight elements, contains invalid values, or contains a
     * year less than 0.
     * @throws java.lang.NullPointerException The parameter {@code dateTime} is null.
     */
    public Message SetDate(int[] dateTime) {
      if (dateTime == null) {
        throw new NullPointerException("dateTime");
      }
      if (!DateTimeUtilities.IsValidDateTime(dateTime)) {
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
     * "message/news", or "message/global", or null otherwise.
     */

    public Message GetBodyMessage() {
      return (this.getContentType().getTopLevelType().equals("message") &&
          (this.getContentType().getSubType().equals("rfc822") ||
           this.getContentType().getSubType().equals("news") ||
           this.getContentType().getSubType().equals("global"))) ? (new
             Message(this.body)) : null;
    }

    /**
     * Gets the name and value of a header field by index.
     * @param index Zero-based index of the header field to get.
     * @return A key/value pair. The key is the name of the header field, such as
     * "From" or "Content-ID". The value is the header field's value.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at
     * least as high as the number of header fields.
     */
    public Map.Entry<String, String> GetHeader(int index) {
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index (" + index +
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
     * letters A to Z (U + 0041 to U + 005A) in both strings to lower case.).
     * @param name The name of a header field.
     * @return The value of the first header field with that name, or null if there
     * is none.
     * @throws java.lang.NullPointerException Name is null.
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
     * upper-case letters A to Z (U + 0041 to U + 005A) in both strings to lower
     * case.).
     * @param name The name of a header field.
     * @return An array containing the values of all header fields with the given
     * name, in the order they appear in the message. The array will be
     * empty if no header field has that name.
     * @throws java.lang.NullPointerException Name is null.
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
     * Not documented yet.
     * @return A Message object.
     */
    public Message ClearHeaders() {
      this.headers.clear();
      this.contentType = MediaType.TextPlainAscii;
      this.contentDisposition = null;
      return this;
    }

    /**
     * Removes a header field by index. <p>Updates the ContentType and
     * ContentDisposition properties if those header fields have been
     * modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @return This instance.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at
     * least as high as the number of header fields.
     */
    public Message RemoveHeader(int index) {
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index (" + index +
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
     * is a multipart message, the header field is not removed from its body
     * part headers. A basic case-insensitive comparison is used. (Two
     * strings are equal in such a comparison, if they match after
     * converting the basic upper-case letters A to Z (U + 0041 to U + 005A) in
     * both strings to lower case.). <p>Updates the ContentType and
     * ContentDisposition properties if those header fields have been
     * modified by this method.</p>
     * @param name The name of the header field to remove.
     * @return This instance.
     * @throws java.lang.NullPointerException The parameter {@code name} is null.
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
     * @throws java.lang.NullPointerException The parameter {@code bytes} is null.
     */
    public Message SetBody(byte[] bytes) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      this.body = bytes;
      return this;
    }

    /**
     * Sets the name and value of a header field by index. <p>Updates the
     * ContentType and ContentDisposition properties if those header fields
     * have been modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @param header A key/value pair. The key is the name of the header field,
     * such as "From" or "Content-ID". The value is the header field's
     * value.
     * @return A Message object.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at
     * least as high as the number of header fields; or, the header field
     * name is too long or contains an invalid character, or the header
     * field's value is syntactically invalid.
     * @throws java.lang.NullPointerException The key or value of {@code header} is
     * null.
     */
    public Message SetHeader(int index, Map.Entry<String, String> header) {
      return this.SetHeader(index, header.getKey(), header.getValue());
    }

    /**
     * Sets the name and value of a header field by index. <p>Updates the
     * ContentType and ContentDisposition properties if those header fields
     * have been modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @param name Name of a header field, such as "From" or "Content-ID".
     * @param value Value of the header field.
     * @return This instance.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at
     * least as high as the number of header fields; or, the header field
     * name is too long or contains an invalid character, or the header
     * field's value is syntactically invalid.
     * @throws java.lang.NullPointerException The parameter {@code name} or {@code
     * value} is null.
     */
    public Message SetHeader(int index, String name, String value) {
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index (" + index +
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
     * Sets the value of a header field by index without changing its name.
     * <p>Updates the ContentType and ContentDisposition properties if those
     * header fields have been modified by this method.</p>
     * @param index Zero-based index of the header field to set.
     * @param value Value of the header field.
     * @return This instance.
     * @throws IllegalArgumentException The parameter {@code index} is 0 or at
     * least as high as the number of header fields; or, the header field
     * name is too long or contains an invalid character, or the header
     * field's value is syntactically invalid.
     * @throws java.lang.NullPointerException The parameter {@code value} is null.
     */
    public Message SetHeader(int index, String value) {
      if (index < 0) {
        throw new IllegalArgumentException("index (" + index + ") is less than " +
                    "0");
      }
      if (index >= (this.headers.size() / 2)) {
        throw new IllegalArgumentException("index (" + index +
                    ") is not less than " + (this.headers.size()
                    / 2));
      }
      return this.SetHeader(index, this.headers.get(index * 2), value);
    }

    /**
     * Sets the value of this message's header field. If a header field with the
     * same name exists, its value is replaced. If the header field's name
     * occurs more than once, only the first instance of the header field is
     * replaced. <p>Updates the ContentType and ContentDisposition
     * properties if those header fields have been modified by this
     * method.</p>
     * @param name The name of a header field, such as "from" or "subject".
     * @param value The header field's value.
     * @return This instance.
     * @throws IllegalArgumentException The header field name is too long or
     * contains an invalid character, or the header field's value is
     * syntactically invalid.
     * @throws java.lang.NullPointerException The parameter {@code name} or {@code
     * value} is null.
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
     * Sets the body of this message to the specified string in HTML format. The
     * character sequences CR (carriage return, "&#x5c;r", U+000D), LF (line
     * feed, "&#x5c;n", U+000A), and CR/LF will be converted to CR/LF line
     * breaks. Unpaired surrogate code points will be replaced with
     * replacement characters.
     * @param str A string consisting of the message in HTML format.
     * @return This instance.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
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
     * Sets the body of this message to a multipart body with plain text and HTML
     * versions of the same message. The character sequences CR (carriage
     * return, "&#x5c;r" , U+000D), LF (line feed, "&#x5c;n", U+000A), and CR/LF will
     * be converted to CR/LF line breaks. Unpaired surrogate code points
     * will be replaced with replacement characters.
     * @param text A string consisting of the plain text version of the message.
     * @param html A string consisting of the HTML version of the message.
     * @return This instance.
     * @throws java.lang.NullPointerException The parameter {@code text} or {@code
     * html} is null.
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
      var textMessage = NewBodyPart().SetTextBody(text);
      var htmlMessage = NewBodyPart().SetHtmlBody(html);
    String mtypestr = "multipart/alternative; boundary=\"=_Boundary00000000\"" ;
      this.setContentType(MediaType.Parse(mtypestr));
      List<Message> messageParts = this.getParts();
      messageParts.clear();
      messageParts.add(textMessage);
      messageParts.add(htmlMessage);
      return this;
    }

    /**
     * Sets the body of this message to the specified plain text string. The
     * character sequences CR (carriage return, "&#x5c;r", U+000D), LF (line
     * feed, "&#x5c;n", U+000A), and CR/LF will be converted to CR/LF line
     * breaks. Unpaired surrogate code points will be replaced with
     * replacement characters. This method changes this message's media type
     * to plain text.
     * @param str A string consisting of the message in plain text format.
     * @return This instance.
     * @throws java.lang.NullPointerException The parameter {@code str} is null.
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
      if ((inputStream) == null) {
        throw new NullPointerException("inputStream");
      }
      if ((mediaType) == null) {
        throw new NullPointerException("mediaType");
      }
      Message bodyPart = NewBodyPart();
      bodyPart.SetHeader("content-id", this.GenerateMessageID());
      // NOTE: Using the setter because it also adds a Content-Type
      // header field
      bodyPart.setContentType(mediaType);
      try {
        java.io.ByteArrayOutputStream ms = null;
try {
ms = new java.io.ByteArrayOutputStream();

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
finally {
try { if (ms != null) {
 ms.close();
 } } catch (java.io.IOException ex) {}
}
      } catch (IOException ex) {
        throw new MessageDataException("An I/O error occurred.", ex);
      }
      DispositionBuilder dispBuilder = new DispositionBuilder(disposition);
      if (!((filename) == null || (filename).length() == 0)) {
        String basename = BaseName(filename);
        if (!((basename) == null || (basename).length() == 0)) {
          dispBuilder.SetParameter("filename",
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
      for (var i = filename.length() - 1; i >= 0; --i) {
        if (filename.charAt(i) == '\\' || filename.charAt(i) == '/') {
          return filename.substring(i + 1);
        }
      }
      return filename;
    }

    private static String ExtensionName(String filename) {
      for (var i = filename.length() - 1; i >= 0; --i) {
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
        if (ext.equals(".doc") || ext.equals(".dot")) {
          return MediaType.Parse("application/msword");
        }
        if (ext.equals(".bin") || ext.equals(".deploy") || ext.equals(".msp") ||
          ext.equals(".msu")) {
          return MediaType.Parse("application/octet-stream");
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

  MediaType.Parse("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        if (ext.equals(".zip")) {
          return MediaType.Parse("application/zip");
        }
        if (ext.equals(".m4a") || ext.equals(".mp2") || ext.equals(".mp3") ||
          ext.equals(".mpega") || ext.equals(".mpga")) {
          return MediaType.Parse("audio/mpeg");
        }
        if (ext.equals(".gif")) {
          return MediaType.Parse("image/gif");
        }
        if (ext.equals(".jpe") || ext.equals(".jpeg") || ext.equals(".jpg")) {
          return MediaType.Parse("image/jpeg");
        }
        if (ext.equals(".png")) {
          return MediaType.Parse("image/png");
        }
        if (ext.equals(".tif") || ext.equals(".tiff")) {
          return MediaType.Parse("image/tiff");
        }
        if (ext.equals(".eml")) {
          return MediaType.Parse("message/rfc822");
        }
        if (ext.equals(".htm") || ext.equals(".html") || ext.equals(".shtml")) {
          return MediaType.Parse("text/html\u003bcharset=utf-8");
        }
        if (ext.equals(".asc") || ext.equals(".brf") || ext.equals(".pot") ||
          ext.equals(".srt") || ext.equals(".text") || ext.equals(".txt")) {
          return MediaType.Parse("text/plain\u003bcharset=utf-8");
        }
      }
      return MediaType.ApplicationOctetStream;
    }

    /**
     * Adds an attachment to this message in the form of data from the given
     * readable stream, and with the given media type. Before the new
     * attachment is added, if this message isn't already a multipart
     * message, it becomes a "multipart/mixed" message with the current body
     * converted to an inline body part.
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the attachment.
     * @return A Message object for the generated attachment.
     * @throws java.lang.NullPointerException The parameter "inputStream" or
     * "mediaType" is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddAttachment(InputStream inputStream, MediaType mediaType) {
      return AddBodyPart(inputStream, mediaType, null, "attachment");
    }

    /**
     * Adds an attachment to this message in the form of data from the given
     * readable stream, and with the given file name. Before the new
     * attachment is added, if this message isn't already a multipart
     * message, it becomes a "multipart/mixed" message with the current body
     * converted to an inline body part.
     * @param inputStream A readable data stream.
     * @param filename A file name to assign to the attachment. If the file name
     * has one of certain extensions (such as ".html"), an appropriate media
     * type will be assigned to the attachment based on that extension;
     * otherwise, the media type "application/octet-stream" is assigned. Can
     * be null or empty, in which case no file name is assigned. Only the
     * file name portion of this parameter is used, which in this case means
     * the portion of the string after the last "/" or "&#x5c;", if either
     * character exists, or the entire string otherwise.
     * @return A Message object for the generated attachment.
     * @throws java.lang.NullPointerException The parameter "inputStream" is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddAttachment(InputStream inputStream, String filename) {
      return
  AddBodyPart(inputStream, SuggestMediaType(filename), filename, "attachment");
    }

    /**
     * Adds an attachment to this message in the form of data from the given
     * readable stream, and with the given media type and file name. Before
     * the new attachment is added, if this message isn't already a
     * multipart message, it becomes a "multipart/mixed" message with the
     * current body converted to an inline body part.
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the attachment.
     * @param filename A file name to assign to the attachment. Can be null or
     * empty, in which case no file name is assigned. Only the file name
     * portion of this parameter is used, which in this case means the
     * portion of the string after the last "/" or "&#x5c;", if either character
     * exists, or the entire string otherwise.
     * @return A Message object for the generated attachment.
     * @throws java.lang.NullPointerException The parameter "inputStream" or
     * "mediaType" is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddAttachment(InputStream inputStream, MediaType mediaType,
      String filename) {
      return AddBodyPart(inputStream, mediaType, filename, "attachment");
    }

    /**
     * Adds an inline body part to this message in the form of data from the given
     * readable stream, and with the given media type. Before the new body
     * part is added, if this message isn't already a multipart message, it
     * becomes a "multipart/mixed" message with the current body converted
     * to an inline body part.
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the body part.
     * @return A Message object for the generated body part.
     * @throws java.lang.NullPointerException The parameter "inputStream" or
     * "mediaType" is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddInline(InputStream inputStream, MediaType mediaType) {
      return AddBodyPart(inputStream, mediaType, null, "inline");
    }

    /**
     * Adds an inline body part to this message in the form of data from the given
     * readable stream, and with the given file name. Before the new body
     * part is added, if this message isn't already a multipart message, it
     * becomes a "multipart/mixed" message with the current body converted
     * to an inline body part.
     * @param inputStream A readable data stream.
     * @param filename A file name to assign to the body part. If the file name has
     * one of certain extensions (such as ".html"), an appropriate media
     * type will be assigned to the body part based on that extension;
     * otherwise, the media type "application/octet-stream" is assigned. Can
     * be null or empty, in which case no file name is assigned. Only the
     * file name portion of this parameter is used, which in this case means
     * the portion of the string after the last "/" or "&#x5c;", if either
     * character exists, or the entire string otherwise.
     * @return A Message object for the generated body part.
     * @throws java.lang.NullPointerException The parameter "inputStream" is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddInline(InputStream inputStream, String filename) {
      return AddBodyPart(inputStream, SuggestMediaType(filename), filename,
        "inline");
    }

    /**
     * Adds an inline body part to this message in the form of data from the given
     * readable stream, and with the given media type and file name. Before
     * the new body part is added, if this message isn't already a multipart
     * message, it becomes a "multipart/mixed" message with the current body
     * converted to an inline body part.
     * @param inputStream A readable data stream.
     * @param mediaType A media type to assign to the body part.
     * @param filename A file name to assign to the body part.
     * @return A Message object for the generated body part.
     * @throws java.lang.NullPointerException The parameter "inputStream" or
     * "mediaType" is null.
     * @throws com.upokecenter.mail.MessageDataException An I/O error occurred.
     */
    public Message AddInline(InputStream inputStream, MediaType mediaType, String
      filename) {
      return AddBodyPart(inputStream, mediaType, filename, "inline");
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
        sb.delete(0, (0)+(sb.length()));
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
            false);  // throws on invalid UTF-8
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
              String field = (origRecipient ?
       "Downgraded-Original-Recipient" : "Downgraded-Final-Recipient");
              headerValue = DataUtilities.GetUtf8String(
                  bytes,
                  headerValueStart,
                  headerValueEnd - headerValueStart,
                  true);  // replaces invalid UTF-8
              String newField = HeaderEncoder.EncodeFieldAsEncodedWords(field,
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

    private static HeaderEncoder EncodeCommentsInText(HeaderEncoder enc,
      String str) {
      int i = 0;
      int begin = 0;
      if (str.indexOf('(') < 0) return enc.AppendString(str);
      StringBuilder sb = new StringBuilder();
      while (i < str.length()) {
        if (str.charAt(i) == '(') {
          int si = HeaderParserUtility.ParseCommentLax(str, i, str.length(),
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
               1) & ~0x20) == 'T' && (headerValue.charAt(index + 2) & ~0x20) == 'F'
                    &&
          headerValue.charAt(index + 3) == '-' && headerValue.charAt(index + 4) == '8';
        atomText = HeaderParser.ParseCFWS(
          headerValue,
          atomText,
          headerValue.length(),
          null);
        // NOTE: Commented out for now (see below)
        //if (atomText != typeEnd) {
        // isUtf8 = false;
        //}
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
            EncodeCommentsInText(encoder,
                    HeaderEncoder.TrimLeadingFWS(typePart + builder));
          } else {
            EncodeCommentsInText(encoder,
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
        status[0] = 0;  // Unchanged
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

    static boolean HasTextToEscapeOrEncodedWordStarts(String s, int
      index, int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, true);
    }

    static boolean HasTextToEscape(String s, int index, int endIndex) {
      return HasTextToEscapeOrEncodedWordStarts(s, index, endIndex, false);
    }

    static boolean HasTextToEscape(String s) {
      return HasTextToEscapeOrEncodedWordStarts(s, 0, s.length(), false);
    }

    static boolean HasTextToEscapeOrEncodedWordStarts(String s, int
      index, int endIndex, boolean checkEWStarts) {
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

    static List<NamedAddress> ParseAddresses(String value) {
      Tokener tokener = new Tokener();
      if (value == null) {
        return new ArrayList<NamedAddress>();
      }
      // Check for valid syntax
      return (HeaderParser.ParseHeaderTo(value, 0, value.length(), tokener) !=
              value.length()) ? (new ArrayList<NamedAddress>()) :
        HeaderParserUtility.ParseAddressList(
          value,
          0,
          value.length(),
          tokener.GetTokens());
    }

    static List<NamedAddress> ParseAddresses(String[] values) {
      ArrayList<NamedAddress> list = new ArrayList<NamedAddress>();
      for (String addressValue : values) {
        if (addressValue == null) {
          continue;
        }
        Tokener tokener = new Tokener();
        if (
          HeaderParser.ParseHeaderTo(
            addressValue,
            0,
            addressValue.length(),
            tokener) != addressValue.length()) {
          // Invalid syntax
          continue;
        }
        list.addAll(
          HeaderParserUtility.ParseAddressList(
            addressValue,
            0,
            addressValue.length(),
            tokener.GetTokens()));
      }
      return list;
    }

    static int ParseUnstructuredText(String s, int startIndex, int
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
          if (i + 2 >= endIndex || s.charAt(i + 1) != 0x0a || (s.charAt(i + 2) != 0x09 &&
              s.charAt(i + 2) != 0x20)) {
            // bare CR, or CRLF not followed by SP/TAB
            return i;
          }
          i += 3;
          boolean found = false;
          for (int j = i; j < endIndex; ++j) {
            if (s.charAt(j) != 0x09 && s.charAt(j) != 0x20 && s.charAt(j) != 0x0d) {
              found = true; break;
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

    private static String Implode(String[] strings, String delim) {
      if (strings.length == 0) {
        return "";
      }
      if (strings.length == 1) {
        return strings[0];
      }
      StringBuilder sb = new StringBuilder();
      boolean first = true;
      for (String s : strings) {
        if (!first) {
          sb.append(delim);
        }
        sb.append(s);
        first = false;
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
          (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <=
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
      Collection<String> headerList,
      boolean start) {
      int lineCount = 0;
      int[] bytesRead = new int[1];
      StringBuilder sb = new StringBuilder();
      int ss = 0;
      boolean ungetLast = false;
      int lastByte = 0;
      while (true) {
        sb.delete(0, (0)+(sb.length()));
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
            throw new

  MessageDataException("Premature end of message before all headers were read, while reading header field name");
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
            sb.append((char)c);
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
              boolean possibleMbox = true;
              boolean isFromField = false;
              sb.delete(0, (0)+(sb.length()));
              while (true) {
                c = stream.read();
                if (c == -1) {
                  throw new MessageDataException(
  "Premature end before all headers were read (Mbox convention)");
                } else if (c == ':' && possibleMbox) {
                  // Full fledged From header field
                  isFromField = true;
                  sb.append("from");
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
        if (sb.length() == 0) {
          throw new MessageDataException("Empty header field name");
        }
        // Set the header field name to the
        // String builder's current value
        String fieldName = sb.toString();
        // Clear the String builder to read the
        // header field's value
        sb.delete(0, (0)+(sb.length()));
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
              "while reading header field value";
            throw new MessageDataException(exstring);
          }
          if (c == '\r') {
            // We're only looking for the single-byte LF, so
            // there's no need to use ReadUtf8Char
            c = stream.read();
            if (c < 0) {
              String exstring = "Premature end before all headers were read," +
                "while looking for LF";
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
                  if (c2 != 0x20 || sb.length() != 0) {
                    sb.append((char)c2);
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
            sb.append('\r');
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
            sb.append((char)c);
            ++lineCount;
          } else {
            int[] state = { lineCount, c, 1 };
            c = ReadUtf8Char(stream, state);
            //DebugUtility.Log("c=" + c + "," + lineCount + "," +
             //       state[0]+ ","+state[1]+","+state[2]);
            lineCount = state[0];
            ungetLast = (state[2] == 1);
            lastByte = state[1];
            if (c <= 0xffff) {
              sb.append((char)c);
            } else if (c <= 0x10ffff) {
              sb.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              sb.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
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
      var read = ungetState[0];
      while (true) {
        int b = ungetState[2] == 1 ?
          ungetState[1] : stream.read();
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
      return (allTextBytes) ? EncodingSevenBit :
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
       throw new IllegalArgumentException("Header field value contains invalid text");
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
      boolean isMultipart = false;
      String boundary = "";
      if (builder.isMultipart()) {
        boundary = GenerateBoundary(depth);
        builder.SetParameter("boundary", boundary);
        isMultipart = true;
      }
      if (!isMultipart) {
        if (builder.getTopLevelType().equals("message")) {
          if (builder.getSubType().equals("delivery-status") ||
              builder.getSubType().equals("global-delivery-status") ||
              builder.getSubType().equals("disposition-notification") ||
              builder.getSubType().equals("global-disposition-notification")) {
            bodyToWrite = DowngradeDeliveryStatus(bodyToWrite);
          }
          boolean msgCanBeUnencoded = CanBeUnencoded(bodyToWrite, depth > 0);
          if ((builder.getSubType().equals("rfc822") || builder.getSubType().equals(
            "news")) && !msgCanBeUnencoded) {
            builder.SetSubType("global");
          } else if (builder.getSubType().equals("disposition-notification") &&
                    !msgCanBeUnencoded) {
            builder.SetSubType("global-disposition-notification");
          } else if (builder.getSubType().equals("delivery-status") &&
                    !msgCanBeUnencoded) {
            builder.SetSubType("global-delivery-status");
          } else if (!msgCanBeUnencoded && !builder.getSubType().equals("global") &&
            !builder.getSubType().equals("global-disposition-notification") &&
            !builder.getSubType().equals("global-delivery-status") &&
            !builder.getSubType().equals("global-headers")) {
          }
        }
      }
      String topLevel = builder.getTopLevelType();
      // NOTE: RFC 6532 allows any transfer encoding for the
      // four global message types listed below.
      transferEnc = topLevel.equals("message") ||
        topLevel.equals("multipart") ? (topLevel.equals("multipart") || (
          !builder.getSubType().equals("global") &&
          !builder.getSubType().equals("global-headers") &&
          !builder.getSubType().equals("global-disposition-notification") &&
          !builder.getSubType().equals("global-delivery-status"))) ?
          EncodingSevenBit : TransferEncodingToUse(
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
          depth > 0 && (
            name.length() < 8 || !name.substring(
              0, (
              0)+(8)).equals("content-"))) {
          // don't generate header fields not starting with "Content-"
          // in body parts
          continue;
        }
        if (name.equals("mime-version")) {
          haveMimeVersion = true;
        } else if (name.equals("message-id")) {
          if (haveMsgId) {
            // Already outputted, continue
            continue;
          }
          haveMsgId = true;
        } else {
          if (ValueHeaderIndices.containsKey(name)) {
            int headerIndex = ValueHeaderIndices.get(name);
            if (headerIndex <= 5) {
              if (haveHeaders[headerIndex]) {
                // Already outputted, continue
                continue;
              }
              boolean isValidAddressing = this.IsValidAddressingField(name);
              haveHeaders[headerIndex] = true;
              /*DebugUtility.Log (name+" "+isValidAddressing);
                {
                  StringBuilder ssb = new StringBuilder();
                  for (Object mhs : this.GetMultipleHeaders (name)) {
                    ssb.append (mhs + " ");
                 if (isValidAddressing && name=="sender") {
                    DebugUtility.Log(""+new NamedAddress(mhs));
                    DebugUtility.Log("" + new NamedAddress(mhs).getDisplayName());
                    DebugUtility.Log("" + new NamedAddress(mhs).getAddress());
                 }
                  }
                  DebugUtility.Log (ssb.toString());
                }*/
              if (!isValidAddressing) {
                value = GenerateAddressList(
    ParseAddresses(this.GetMultipleHeaders(name)));
                if (value.length() == 0) {
                  // No addresses, synthesize a field
                  rawField = this.SynthesizeField(name);
                }
              }
            } else if (headerIndex <= 10) {
              // Resent-* fields can appear more than once
              value = GenerateAddressList(ParseAddresses(value));
              if (value.length() == 0) {
                // No addresses, synthesize a field
                rawField = this.SynthesizeField(name);
              }
            }
          }
        }
        rawField = (rawField == null) ? ((HeaderEncoder.EncodeField(name, value))) : rawField;
        if (HeaderEncoder.CanOutputRaw(rawField)) {
          AppendAscii(output, rawField);
        } else {
          //DebugUtility.Log("Can't output '"+name+"' raw");
          String downgraded = HeaderFieldParsers.GetParser(name)
                    .DowngradeHeaderField(name, value);
          if (
            HasTextToEscape(
              downgraded,
              0,
              downgraded.length())) {
            if (name.equals("message-id") ||
                name.equals("resent-message-id") || name.equals(
                "in-reply-to") || name.equals("references") || name.equals(
                "original-recipient") || name.equals("final-recipient")) {
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
          bodyEncoder = new Base64Encoder(true, builder.isText(), false);
          break;
        case EncodingQuotedPrintable:
          bodyEncoder = new QuotedPrintableEncoder(
            builder.isText() ? 2 : 0,
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
        for (Message part : this.getParts()) {
          AppendAscii(output, "\r\n--" + boundary + "\r\n");
          part.Generate(output, depth + 1);
        }
        AppendAscii(output, "\r\n--" + boundary + "--");
      }
    }

    private String GenerateMessageID() {
      StringBuilder builder = new StringBuilder();
      int seq = 0;
      builder.append("<");
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
        builder.append("@");
        seq = addresses.get(0).isGroup() ? addresses.get(0).getName().hashCode() :
          addresses.get(0).getAddress().toString().hashCode();
        for (int i = 0; i < 8; ++i) {
          builder.append(ValueHex.charAt(seq & 15));
          seq >>= 4;
        }
        builder.append(".local.invalid");
      }
      builder.append(">");
      // DebugUtility.Log(builder.toString());
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

    private void ProcessHeaders(boolean assumeMime, boolean digest) {
      boolean haveContentType = false;
      boolean mime = assumeMime;
      boolean haveContentDisp = false;
      String transferEncodingValue = "";
      for (int i = 0; i < this.headers.size(); i += 2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (name.equals("content-transfer-encoding")) {
          int startIndex = HeaderParser.ParseCFWS(value, 0, value.length(), null);
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
                if (!ctype.StoresCharsetInPayload()) {
                  // Used unless the media type defines how charset
                  // is determined from the payload
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
      // Update content type as appropriate
      // NOTE: Setting the field, not the setter,
      // because it's undesirable here to add a Content-Type
      // header field, as the setter does
      this.contentType = ctype;
      if (!haveContentEncoding && this.contentType.getTypeAndSubType().equals(
        "message/rfc822")) {
        // DEVIATION: Be a little more liberal with rfc822
        // messages with 8-bit bytes
        this.transferEncoding = EncodingEightBit;
      }
      if (this.transferEncoding == EncodingSevenBit) {
        String charset = this.contentType.GetCharset();
        if (charset.equals("utf-8")) {
          // DEVIATION: Be a little more liberal with UTF-8
          this.transferEncoding = EncodingEightBit;
        } else if (this.contentType.getTypeAndSubType().equals("text/html")) {
          if (charset.equals("us-ascii") ||
              charset.equals("windows-1252") || charset.equals(
                "windows-1251") ||
              (charset.length() > 9 && charset.substring(0, 9).equals(
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
        if (this.contentType.isMultipart() ||
            (this.contentType.getTopLevelType().equals("message") &&
             !this.contentType.getSubType().equals("global") &&
             !this.contentType.getSubType().equals("global-headers") &&
             !this.contentType.getSubType().equals(
               "global-disposition-notification") &&
             !this.contentType.getSubType().equals("global-delivery-status"))) {
          if (this.transferEncoding == EncodingQuotedPrintable ||
              this.transferEncoding == EncodingBase64) {
            // DEVIATION: Treat quoted-printable for multipart and message
            // as 7bit instead
            this.transferEncoding = EncodingSevenBit;
          } else {
            String exceptText =
              "Invalid content encoding for multipart or message";

            throw new MessageDataException(exceptText);
          }
        }
      }
    }

    private void ReadMessage(IByteReader stream) {
      try {
        ReadHeaders(stream, this.headers, true);
        this.ProcessHeaders(false, false);
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
              var msg = NewBodyPart();
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
              Message parentMessage = multipartStack.get(multipartStack.size() -
                    1).getMessage();
              boundaryChecker.StartBodyPartHeaders();
              MediaType ctype = parentMessage.getContentType();
              boolean parentIsDigest = ctype.getSubType().equals("digest") &&
                ctype.isMultipart();
              ReadHeaders(stream, msg.headers, false);
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.getParts().add(msg);
              multipartStack.add(entry);
              aw.Clear();
              ctype = msg.getContentType();
              leaf = ctype.isMultipart() ? null : msg;
              boundaryChecker.EndBodyPartHeaders(entry.getBoundary());
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                ctype.getTypeAndSubType().equals("text/plain"));
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
      IByteReader transform = MakeTransferEncoding(
        stream,
        this.transferEncoding,
        this.getContentType().getTypeAndSubType().equals("text/plain"));
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
      HeaderEncoder encoder = new HeaderEncoder(76, 0);
      encoder.AppendSymbol(name + ":");
      encoder.AppendSpace();
      String fullField = Implode(this.GetMultipleHeaders(name), ", ");
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
     * This is an internal API.
     * @return This is an internal API.
     */
      public final Message getMessage() { return propVarmessage; }
private final Message propVarmessage;

    /**
     * This is an internal API.
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
  MessageDataException("Multipart message has an invalid boundary defined: " +
                newBoundary);
          }
        }
        this.propVarmessage = msg;
        this.propVarboundary = newBoundary;
      }
    }
  }
