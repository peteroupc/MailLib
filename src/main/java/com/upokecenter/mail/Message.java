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
     * content-transfer-encoding "quoted-printable" is treated as 7bit
     * instead if it occurs in a message or body part with content type
     * "multipart/*" or "message/*" (other than "message/global",
     * "message/global-headers", "message/global-disposition-notification",
     * or "message/global-delivery-status").</li> <li>If a message has two
     * or more Content-Type header fields, it is treated as having a content
     * type of "application/octet-stream", unless one or more of the header
     * fields is syntactically invalid.</li> <li>Illegal UTF-8 byte
     * sequences appearing in header field values are replaced with
     * replacement characters. Moreover, UTF-8 is parsed everywhere in
     * header field values, even in those parts of some structured header
     * fields where this appears not to be allowed. (UTF-8 is a character
     * encoding for the Unicode character set.)</li> <li>The To and Cc
     * header fields are allowed to contain only comments and whitespace,
     * but these "empty" header fields will be omitted when generating.</li>
     * <li>There is no line length limit imposed when parsing header fields,
     * except header field names.</li> <li>There is no line length limit
     * imposed when parsing quoted-printable or base64 encoded bodies.</li>
     * <li>If the transfer encoding is absent and the content type is
     * "message/rfc822", bytes with values greater than 127 (called "8-bit
     * bytes" in the rest of this summary) are still allowed, despite the
     * default value of "7bit" for "Content-Transfer-Encoding".</li> <li>In
     * the following cases, if the transfer encoding is absent or declared
     * as 7bit, 8-bit bytes are still allowed:</li> <li>(a) The preamble and
     * epilogue of multipart messages, which will be ignored.</li> <li>(b)
     * If the charset is declared to be <code>utf-8</code>.</li> <li>(c) If the
     * content type is "text/html" and the charset is declared to be
     * <code>ascii</code>, <code>us-ascii</code>, "windows-1252", "windows-1251", or
     * "iso-8859-*" (all single byte encodings).</li> <li>(d) In non-MIME
     * message bodies and in text/plain message bodies. Any 8-bit bytes are
     * replaced with the substitute character byte (0x1a).</li> <li>If the
     * first line of the message starts with the word "From" followed by a
     * space, it is skipped.</li> <li>The name <code>ascii</code> is treated as a
     * synonym for <code>us-ascii</code>, despite being a reserved name under RFC
     * 2046. The name <code>cp1252</code> is treated as a synonym for
     * <code>windows-1252</code> , even though it's not an IANA registered
     * alias.</li> <li>The following deviations involve encoded words under
     * RFC 2047:</li> <li>(a) If a sequence of encoded words decodes to a
     * string with a CTL character (U + 007F, or a character less than U + 0020
     * and not TAB) after being converted to Unicode, the encoded words are
     * left un-decoded.</li> <li>(b) </li></ul> <p>It would be appreciated
     * if users of this library contact the author if they find other ways
     * in which this implementation deviates from the mail specifications or
     * other applicable specifications.</p> <p>Note that this class
     * currently doesn't support the "padding" parameter for message bodies
     * with the media type "application/octet-stream" or treated as that
     * media type (see RFC 2046 sec. 4.5.1).</p> <p>Note that this
     * implementation can decode an RFC 2047 encoded word that uses
     * ISO-2022-JP (the only supported encoding that uses code switching)
     * even if the encoded word's payload ends in a different mode from
     * "ASCII mode". (Each encoded word still starts in "ASCII mode",
     * though.) This, however, is not a deviation to RFC 2047 because the
     * relevant rule only concerns bringing the output device back to "ASCII
     * mode" after the decoded text is displayed (see last paragraph of sec.
     * 6.2) -- since the decoded text is converted to Unicode rather than
     * kept as ISO-2022-JP, this is not applicable since there is no such
     * thing as "ASCII mode" in the Unicode Standard.</p>
     */
  public final class Message {
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

    private static String valueDigits = "0123456789";

    private static String IntToString(int value) {
      if (value == Integer.MIN_VALUE) {
        return "-2147483648";
      }
      if (value == 0) {
        return "0";
      }
      boolean neg = value < 0;
      char[] chars = new char[24];
      int count = 0;
      if (neg) {
        chars[0] = '-';
        ++count;
        value = -value;
      }
      while (value != 0) {
        char digit = valueDigits.charAt((int)(value % 10));
        chars[count++] = digit;
        value /= 10;
      }
      if (neg) {
        ReverseChars(chars, 1, count - 1);
      } else {
        ReverseChars(chars, 0, count);
      }
      return new String(chars, 0, count);
    }

    private static String[] valueDaysOfWeek = {
      "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"
    };

    private static String[] valueMonths = {
      "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul",
      "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    private static String GetDateString(int[] dateTime) {
      if (!DateTimeUtilities.IsValidDateTime(dateTime) ||
        dateTime[0] < 0) {
        throw new IllegalArgumentException("Invalid date and time");
      }
      int dow = DateTimeUtilities.GetDayOfWeek(dateTime);
      if (dow < 0) {
        throw new IllegalArgumentException("Invalid date and time");
      }
      String dayString = valueDaysOfWeek[dow];
      String monthString = valueMonths[dateTime[1]];
      StringBuilder sb = new StringBuilder();
      sb.append(dayString);
      sb.append(", ");
      sb.append((char)('0' + ((dateTime[2] / 10) % 10)));
      sb.append((char)('0' + (dateTime[2] % 10)));
      sb.append(' ');
      sb.append(monthString);
      sb.append(' ');
      String yearString = IntToString(dateTime[0]);
      if (yearString.length() < 4) {
        for (int i = 0; i < 4 - yearString.length(); ++i) {
          sb.append('0');
        }
      }
      sb.append(yearString);
      sb.append(' ');
      sb.append((char)('0' + ((dateTime[3] / 10) % 10)));
      sb.append((char)('0' + (dateTime[3] % 10)));
      sb.append(':');
      sb.append((char)('0' + ((dateTime[4] / 10) % 10)));
      sb.append((char)('0' + (dateTime[4] % 10)));
      sb.append(':');
      sb.append((char)('0' + ((dateTime[5] / 10) % 10)));
      sb.append((char)('0' + (dateTime[5] % 10)));
      sb.append(' ');
      int offset = dateTime[7];
      sb.append((offset < 0) ? '-' : '+');
      offset = Math.abs(offset);
      int hours = (offset / 60) % 24;
      int minutes = offset % 60;
      sb.append((char)('0' + ((hours / 10) % 10)));
      sb.append((char)('0' + (hours % 10)));
      sb.append((char)('0' + ((minutes / 10) % 10)));
      sb.append((char)('0' + (minutes % 10)));
      return sb.toString();
    }

    /**
     * Gets a list of addresses found in the BCC header field or fields.
     * @return A list of addresses found in the BCC header field or fields.
     */
    public final List<NamedAddress> getBccAddresses() {
        return ParseAddresses(this.GetMultipleHeaders("bcc"));
      }

    /**
     * Gets the body of this message as a text string.
     * @return The body of this message as a text string.
     * @throws UnsupportedOperationException This message has no character encoding
     * declared on it (which is usually the case for non-text messages), or
     * the character encoding is not supported.
     */
    public final String getBodyString() {
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
     */
    public final List<NamedAddress> getCCAddresses() {
        return ParseAddresses(this.GetMultipleHeaders("cc"));
      }

    /**
     * Gets this message's content disposition. The content disposition specifies
     * how a user agent should display or otherwise handle this message. Can
     * be set to null.
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
     * Gets this message's media type.
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
        if (!this.getContentType().equals(value)) {
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
     * parameter of the Content-Type header field, otherwise.</p><p>
     * <p><b>Remark:</b> Note that RFC 2046 sec. 4.5.1
     * (<code>application/octet-stream</code> subtype) cites an earlier RFC 1341,
     * which "defined the use of a 'NAME' parameter which gave a
     * <i>suggested</i> file name to be used if the data were written to a
     * file". (Although the same section says this parameter "has been
     * deprecated in anticipation of [the] Content-Disposition header
     * field", the <code>name</code> parameter may still be written out to email
     * messages in some implementations, even in media types other than
     * <code>application/octet-stream</code>.) Also, RFC 2183 sec. 2.3
     * (<code>filename</code> parameter) confirms that the "<i>suggested</i>
     * filename" in the <code>filename</code> parameter "should be <i>used as a
     * basis</i> for the actual filename, where possible", and that that
     * file name should "not [be] blindly use[d]". See also RFC 6266,
     * section 4.3, which discusses the use of that parameter in Hypertext
     * Transfer Protocol (HTTP). Thus, this implementation is justified in
     * not using the exact file name given, but rather adapting it to
     * increase the chance of the name being usable in file systems.</p>
     * <p>To the extent that the "name" parameter is not allowed in message
     * bodies with the media type "application/octet-stream" or treated as
     * that media-type, this is a deviation of RFC 2045 and 2046 (see also
     * RFC 2045 sec. 5, which says that "[t]here are NO globally meaningful
     * parameters that apply to all media types").</p> </p>
     * @return A suggested name for the file, or the empty string if there is no
     * filename suggested by the content type or content disposition.
     */
    public final String getFileName() {
        ContentDisposition disp = this.contentDisposition;
        return (disp != null) ?
          ContentDisposition.MakeFilename(disp.GetParameter("filename")) :
        ContentDisposition.MakeFilename(this.contentType.GetParameter(
  "name"));
      }

    /**
     * Gets a list of addresses found in the From header field or fields.
     * @return A list of addresses found in the From header field or fields.
     */
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
     */
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
     * to anyone). (An exception is that the Resent-From and Resent-Sender
     * header fields may appear more than once.) The generated message
     * should always have a From header field.</p> <p>If a Date and/or
     * Message-ID header field doesn't exist, a field with that name will be
     * generated (using the current local time for the Date field).</p>
     * <p>When encoding the message's body, if the message has a text
     * content type ("text/*"), the line breaks are a CR byte (carriage
     * return, 0x0d) followed by an LF byte (line feed, 0x0a), CR alone, or
     * LF alone. If the message has any other content type, only CR followed
     * by LF is considered a line break.</p>
     * @return The generated message.
     * @throws PeterO.Mail.MessageDataException The message can't be generated.
     */
    public String Generate() {
      ArrayWriter aw = new ArrayWriter();
      this.Generate(aw, 0);
      return DataUtilities.GetUtf8String(aw.ToArray(), false);
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
     * Gets the date and time extracted from this message's Date header field (as
     * though GetHeader("date") were called). Each element of the array
     * (starting from 0) is as follows: <ul> <li>0 - The year. For example,
     * the value 2000 means 2000 C.E.</li> <li>1 - Month of the year, from 1
     * (January) through 12 (December).</li> <li>2 - Day of the month, from
     * 1 through 31.</li> <li>3 - Hour of the day, from 0 through 23.</li>
     * <li>4 - Minute of the hour, from 0 through 59.</li> <li>5 - Second of
     * the minute, from 0 through 60 (this value can go up to 60 to
     * accommodate leap seconds). (Leap seconds are additional seconds added
     * to adjust international atomic time, or TAI, to an approximation of
     * astronomical time known as coordinated universal time, or UTC.)</li>
     * <li>6 - Milliseconds of the second, from 0 through 999. Will always
     * be 0.</li> <li>7 - Number of minutes to subtract from this date and
     * time to get global time. This number can be positive or
     * negative.</li></ul>
     * @return An array containing eight elements. Returns null if the Date header
     * doesn't exist, if the Date field is syntactically or semantically
     * invalid, or if the field's year would overflow a 32-bit signed
     * integer.
     */
    public int[] GetDate() {
      String field = this.GetHeader("date");
      if (field == null) {
 return null;
}
      int[] date = new int[8];
      return HeaderParserUtility.ParseHeaderExpandedDate(
  field,
  0,
  field.length(),
  date) != 0 ? date : null;
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
          IntToString(dateTime[0]));
      }
      return this.SetHeader("date", GetDateString(dateTime));
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
      this.contentType = IsShortAndAllAscii(str) ? MediaType.TextPlainAscii :
        MediaType.TextPlainUtf8;
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
      Message textMessage = new Message().SetTextBody(text);
      Message htmlMessage = new Message().SetHtmlBody(html);
      this.contentType =
  MediaType.Parse("multipart/alternative; boundary=\"=_Boundary00000000\"");
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
      this.contentType = IsShortAndAllAscii(str) ? MediaType.TextPlainAscii :
        MediaType.TextPlainUtf8;
      return this;
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
            bytes[index + 4] == ' ') {
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
        if (lineLength > 78) {
          // System.out.println("Line length exceeded (" + maxLineLength +
          // " " + (str.substring(index-78,(index-78)+(78))) + ")");
          return false;
        }
        ++index;
      }
      return true;
    }

    // Returns true only if:
    // * Text matches the production "unstructured"
    // in RFC 5322 without any obsolete syntax
    // * Each line is no more than 75 characters in length
    // * Text has only printable ASCII characters, CR,
    // LF, and/or TAB
    static boolean CanOutputRaw(String s) {
      int len = s.length();
      int chunkLength = 0;
      boolean maybe = false;
      boolean firstColon = true;
      for (int i = 0; i < len; ++i) {
        char c = s.charAt(i);
        if (c == ':' && firstColon) {
          if (i + 1 >= len || s.charAt(i + 1) != 0x20) {
            // colon not followed by SPACE (0x20)
            return false;
          }
          firstColon = false;
        }
        if (c == 0x0d) {
          if (i + 1 >= len || s.charAt(i + 1) != 0x0a) {
            // bare CR
            return false;
          }
          if (i + 2 >= len || (s.charAt(i + 2) != 0x09 && s.charAt(i + 2) != 0x20)) {
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
      return (!maybe) || (ParseUnstructuredText(s, 0, s.length()) == s.length());
    }

    // Parse the delivery status byte array to downgrade
    // the Original-Recipient and Final-Recipient header fields
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
          if (c != 0x20 && c != 0x09) {
            headerNameEnd = index;
          }
        }
        if (endOfHeaders) {
          break;
        }
        int headerValueStart = index;
        int headerValueEnd = index;
        String origFieldName =
          DataUtilities.GetUtf8String(
            bytes,
            headerNameStart,
            headerValueStart - headerNameStart,
            true);
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
                  if (lineStart) {
                    haveFWS = false;
                  }
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
          String headerValue = DataUtilities.GetUtf8String(
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
              writer.write(bytes, 0, headerNameStart);
            } else {
              writer.write(bytes, lastIndex, headerNameStart - lastIndex);
            }
            WordWrapEncoder encoder = new WordWrapEncoder(true);
            String field = (origRecipient ?
        "Downgraded-Original-Recipient" : "Downgraded-Final-Recipient") +
                  ": ";
            if (status[0] != 2) {
 field = origFieldName + " ";
}
            encoder.AddString(field + headerValue);
            byte[] newBytes = DataUtilities.GetUtf8Bytes(
              encoder.toString(),
              true);
            writer.write(newBytes, 0, newBytes.length);
            lastIndex = headerValueEnd;
          }
        }
      }
      if (writer != null) {
        writer.write(bytes, lastIndex, bytes.length - lastIndex);
        bytes = writer.ToArray();
      }
      return bytes;
    }

    static String DowngradeRecipientHeaderValue(
      String headerValue,
      int[] status) {
      int index;
      if (
        HasTextToEscapeIgnoreEncodedWords(
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
        if (index < headerValue.length() && headerValue.charAt(atomText) == ';') {
          String typePart = headerValue.substring(0, atomText + 1);
          // Downgrade the comments in the type part
          // NOTE: Final-recipient has the same syntax as original-recipient,
          // except for the header field name
          typePart = HeaderFieldParsers.GetParser(
            "original-recipient").DowngradeFieldValue(typePart);
          if (isUtf8) {
            // Downgrade the non-ASCII characters in the address
            StringBuilder builder = new StringBuilder();
            String ValueHex = "0123456789ABCDEF";
            for (int i = atomText + 1; i < headerValue.length(); ++i) {
              if (headerValue.charAt(i) < 0x80) {
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
            headerValue = typePart + builder;
          } else {
            headerValue = typePart + headerValue.substring(atomText + 1);
          }
        }
        if (
          HasTextToEscapeIgnoreEncodedWords(
            headerValue,
            0,
            headerValue.length())) {
          // Encapsulate the header field in encoded words
          if (status != null) {
            // Encapsulated
            status[0] = 2;
          }
          int spaceAndTabEnd = ParserUtility.SkipSpaceAndTab(
  origValue,
  0,
  origValue.length());
          return Rfc2047.EncodeString(origValue.substring(spaceAndTabEnd));
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
      return headerValue;
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

    static boolean HasTextToEscape(String s) {
      // <summary>Returns true if the String has: * non-ASCII characters *
      // "=?" * CTLs other than tab, or * a word longer than 75 characters.
      // Can return false even if the String has: * CRLF followed by a line
      // with just whitespace.</summary>
      return HasTextToEscape(s, 0, s.length());
    }

    static boolean HasTextToEscape(String s, int index, int endIndex) {
      int len = endIndex;
      int chunkLength = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = s.charAt(i);
        if (c == '=' && i + 1 < len && c == '?') {
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
          if (chunkLength > 75) {
            return true;
          }
        }
      }
      return false;
    }

    static boolean HasTextToEscapeIgnoreEncodedWords(
      String s,
      int index,
      int endIndex) {
      int len = endIndex;
      int chunkLength = 0;

      for (int i = index; i < endIndex; ++i) {
        char c = s.charAt(i);
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
          if (chunkLength > 998) {
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

    static int ParseUnstructuredText(
      String str,
      int index,
      int endIndex) {
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
                  while (index < endIndex && ((str.charAt(index) == 32) ||
                (str.charAt(index) == 9))) {
                    ++index;
                    }
                 if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index +
                1) == 10) {
                    index += 2;
                    } else {
                    index = indexStart4; break;
                    }
                    indexTemp4 = index;
                    index = indexStart4;
                  } while (false);
                  if (indexTemp4 != index) {
                    index = indexTemp4;
                  } else {
                    break;
                  }
                } while (false);
                if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                    9))) {
                  ++index;
                  while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index)
                   == 9))) {
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
              } else {
                break;
              }
            } while (false);
            do {
              int indexTemp3 = index;
              do {
                if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <=
                    55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
                    str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 &&
                    str.charAt(index + 1) <= 57343))) {
                  indexTemp3 += 2; break;
                }
                if (index < endIndex && (str.charAt(index) >= 33 && str.charAt(index) <=
                    126)) {
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
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
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

    private static String Capitalize(String s) {
      StringBuilder builder = new StringBuilder();
      boolean afterHyphen = true;
      for (int i = 0; i < s.length(); ++i) {
        if (afterHyphen && s.charAt(i) >= 'a' && s.charAt(i) <= 'z') {
          builder.append((char)(s.charAt(i) - 0x20));
        } else {
          builder.append(s.charAt(i));
        }
        afterHyphen = s.charAt(i) == '-';
      }
      String ret = builder.toString();
      return ret.equals("Mime-Version") ? "MIME-Version" :
        (ret.equals("Message-Id") ? "Message-ID" : ret);
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
      if (str == null || str.length() < 1 || str.length() > 70) {
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
      dict.put("reply-to",4);
      dict.put("resent-to",5);
      dict.put("resent-cc",6);
      dict.put("resent-bcc",7);
      dict.put("sender",8);
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
          transform = new QuotedPrintableTransform(stream, false, -1);
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
      TransformWithUnget ungetStream = new TransformWithUnget(stream);
      while (true) {
        sb.delete(0, (0)+(sb.length()));
        boolean first = true;
        boolean endOfHeaders = false;
        boolean wsp = false;
        lineCount = 0;
        while (true) {
          int c = ungetStream.read();
          if (c == -1) {
            throw new
  MessageDataException("Premature end before all headers were read");
          }
          ++lineCount;
          if (first && c == '\r') {
            if (ungetStream.read() == '\n') {
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
            if (lineCount >= 999) {
              // 998 characters includes the colon
              throw new MessageDataException("Header field name too long");
            }
            break;
          } else if (c == 0x20 || c == 0x09) {
            if (start && c == 0x20 && sb.length() == 4 && sb.toString().equals(
              "From")) {
              // Mbox convention, skip the entire line
              sb.delete(0, (0)+(sb.length()));
              while (true) {
                c = ungetStream.read();
                if (c == -1) {
                  throw new
  MessageDataException("Premature end before all headers were read");
                }
                if (c == '\r') {
                  if (ungetStream.read() == '\n') {
                    // End of line was reached
                    break;
                  }
                  ungetStream.Unget();
                }
              }
              start = false;
              wsp = false;
              first = true;
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
        // Read the header field value using UTF-8 characters
        // rather than bytes (DEVIATION: RFC 6532 allows UTF-8
        // in header field values, but not everywhere in these values,
        // as is done here for convenience)
        while (true) {
          int c = ReadUtf8Char(ungetStream, bytesRead);
          if (c == -1) {
            throw new MessageDataException(
              "Premature end before all headers were read");
          }
          if (c == '\r') {
            // We're only looking for the single-byte LF, so
            // there's no need to use ReadUtf8Char
            c = ungetStream.read();
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
                  c = ungetStream.read();
                  if (c == '\r') {
                    c = ungetStream.read();
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
                    ungetStream.Unget();
                  }
                }
                fwsFirst = false;
                // Use ReadByte here since we're just looking for the single
                // byte characters space and tab
                int c2 = ungetStream.read();
                if (c2 == 0x20 || c2 == 0x09) {
                  ++lineCount;
                  // Don't write SPACE as the first character of the value
                  if (c2 != 0x20 || sb.length() != 0) {
                    sb.append((char)c2);
                  }
                  haveFWS = true;
                } else {
                  ungetStream.Unget();
                  // this isn't space or tab; if this is the start
                  // of the line, this is no longer FWS
                  if (lineCount == 0) {
                    haveFWS = false;
                  }
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
            if (c < 0) {
              throw new
  MessageDataException("Premature end before all headers were read");
            }
            sb.append('\r');
            ungetStream.Unget();
            ++lineCount;
          }
          lineCount += bytesRead[0];
          // NOTE: Header field line limit not enforced here, only
          // in the header field name; it's impossible to generate
          // a conforming message if the name is too long
          // NOTE: Some emails still have 8-bit bytes in an unencoded
          // subject line
          // or other unstructured header field; however, since RFC6532,
          // we can just assume the UTF-8 encoding in these cases; in
          // case the bytes are not valid UTF-8, a replacement character
          // will be output
          if (c != 0x20 || sb.length() != 0) {
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
      TransformWithUnget stream,
      int[] bytesRead) {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      int cp = 0;
      int bytesSeen = 0;
      int bytesNeeded = 0;
      int lower = 0x80;
      int upper = 0xbf;
      int read = 0;
      while (true) {
        int b = stream.read();
        ++read;
        if (b < 0) {
          if (bytesNeeded != 0) {
            stream.Unget();
            --read;
            bytesRead[0] = read;
            return 0xfffd;
          }
          return -1;
        }
        if (bytesNeeded == 0) {
          if ((b & 0x7f) == b) {
            bytesRead[0] = read;
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
            bytesRead[0] = read;
            return 0xfffd;
          }
          continue;
        }
        if (b < lower || b > upper) {
          stream.Unget();
          return 0xfffd;
        }
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

    private static boolean StartsWithWhitespace(String str) {
      return str.length() > 0 && (str.charAt(0) == ' ' || str.charAt(0) == 0x09 || str.charAt(0) ==
                    '\r');
    }

    private static int TransferEncodingToUse(byte[] body, boolean isBodyPart) {
      if (body == null || body.length == 0) {
        return EncodingSevenBit;
      }
      int lengthCheck = Math.min(body.length, 4096);
      int highBytes = 0;
      int ctlBytes = 0;
      int lineLength = 0;
      boolean allTextBytes = !isBodyPart;
      for (int i = 0; i < lengthCheck; ++i) {
        if (highBytes + ctlBytes > 100 && i == 300) {
          return EncodingBase64;
        }
        if (highBytes + ctlBytes > 10 && i == 300) {
          return EncodingQuotedPrintable;
        }
        if ((body[i] & 0x80) != 0) {
          ++highBytes;
          allTextBytes = false;
        } else if (body[i] == 0x00) {
          allTextBytes = false;
          ++ctlBytes;
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
          'm' || body[i + 4] != ' ';
        ++lineLength;
        allTextBytes &= lineLength <= 78;
      }
      return (lengthCheck == body.length && allTextBytes) ? EncodingSevenBit :
        ((highBytes > (lengthCheck / 3)) ? EncodingBase64 : ((ctlBytes >
                    10) ? EncodingBase64 : EncodingQuotedPrintable));
    }

    private static String ValidateHeaderField(String name, String value) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (value == null) {
        throw new NullPointerException("value");
      }
      if (name.length() > 997) {
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
              builder.getSubType().equals("global-delivery-status")) {
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
            if (headerIndex < 9) {
              if (haveHeaders[headerIndex]) {
                // Already outputted, continue
                continue;
              }
              haveHeaders[headerIndex] = true;
              if (!this.IsValidAddressingField(name)) {
                /*DebugUtility.Log (name);
                {
                  StringBuilder ssb = new StringBuilder();
                  for (Object mhs : this.GetMultipleHeaders (name)) {
                    ssb.append (mhs + " ");
                  }
                  DebugUtility.Log (ssb.toString());
                }*/
                value = GenerateAddressList(
    ParseAddresses(this.GetMultipleHeaders(name)));
                if (value.length() == 0) {
                  // No addresses, synthesize a field
                  value = this.SynthesizeField(name);
                }
              }
            }
            if (headerIndex == 9 || headerIndex == 10) {
              // Resent-From/Resent-Sender, can appear
              // more than once
              value = GenerateAddressList(ParseAddresses(value));
              if (value.length() == 0) {
                // No addresses, synthesize a field
                value = this.SynthesizeField(name);
              }
            }
          }
        }
        String rawField = Capitalize(name) + ":" +
          (StartsWithWhitespace(value) ? "" : " ") + value;
        if (CanOutputRaw(rawField)) {
          AppendAscii(output, rawField);
          if (rawField.indexOf(": ") < 0) {
            throw new MessageDataException("No colon+space: " + rawField);
          }
        } else if (HasTextToEscape(value)) {
          String downgraded =
            HeaderFieldParsers.GetParser(name).DowngradeFieldValue(value);
          if (
            HasTextToEscapeIgnoreEncodedWords(
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
              name = "downgraded-" + name;
              downgraded =
                    Rfc2047.EncodeString(ParserUtility.TrimSpaceAndTab(value));
            } else {
            }
          }
          boolean haveDquote = downgraded.indexOf('"') >= 0;
          WordWrapEncoder encoder = new WordWrapEncoder(!haveDquote);
          encoder.AddString(Capitalize(name) + ": " + downgraded);
          String newValue = encoder.toString();
          AppendAscii(output, newValue);
        } else {
          boolean haveDquote = value.indexOf('"') >= 0;
          WordWrapEncoder encoder = new WordWrapEncoder(!haveDquote);
          encoder.AddString(Capitalize(name) + ": " + value);
          String newValue = encoder.toString();
          AppendAscii(output, newValue);
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
          GetDateString(DateTimeUtilities.GetCurrentLocalTime()));
        AppendAscii(output, "\r\n");
      }
      if (!haveMsgId && depth == 0) {
        AppendAscii(output, "Message-ID:\r\n ");
        AppendAscii(output, this.GenerateMessageID());
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
      List<NamedAddress> addresses = this.getFromAddresses();
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
      this.contentType = digest ? MediaType.MessageRfc822 :
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
              this.contentType = MediaType.TextPlainAscii;
              haveInvalid = true;
            } else {
              this.contentType = MediaType.ApplicationOctetStream;
            }
          } else {
            this.contentType = MediaType.Parse(
              value,
              null);
            if (this.contentType == null) {
              this.contentType = digest ? MediaType.MessageRfc822 :
                MediaType.TextPlainAscii;
              haveInvalid = true;
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
        this.contentType = MediaType.Parse("application/octet-stream");
      }
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
          if (charset.equals("us-ascii") || charset.equals("ascii") ||
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
          if (this.transferEncoding == EncodingQuotedPrintable) {
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
      BoundaryCheckerTransform boundaryChecker = new BoundaryCheckerTransform(stream);
      // Be liberal on the preamble and epilogue of multipart
      // messages, as they will be ignored.
      IByteReader currentTransform = MakeTransferEncoding(
        boundaryChecker,
        baseTransferEncoding,
        true);
      List<MessageStackEntry> multipartStack = new ArrayList<MessageStackEntry>();
      MessageStackEntry entry = new MessageStackEntry(this);
      multipartStack.add(entry);
      boundaryChecker.PushBoundary(entry.getBoundary());
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
              Message msg = new Message();
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
              boundaryChecker.PushBoundary(entry.getBoundary());
              boundaryChecker.EndBodyPartHeaders();
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
      String fullField = Implode(this.GetMultipleHeaders(name), ", ");
      String value = new EncodedWordEncoder().AddString(fullField)
        .FinalizeEncoding().toString();
      if (value.length() > 0) {
        value += " <me@" + name + "address.invalid>";
      } else {
        value = "me@" + name + "-address.invalid";
      }
      return value;
    }

    private static class MessageStackEntry {
      private final Message message;

    /**
     * Gets an internal value.
     * @return An internal value.
     */
      public final Message getMessage() {
          return this.message;
        }

      private final String boundary;

    /**
     * Gets an internal value.
     * @return An internal value.
     */
      public final String getBoundary() {
          return this.boundary;
        }

      public MessageStackEntry(Message msg) {
        this.message = msg;
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
        this.boundary = newBoundary;
      }
    }
  }
