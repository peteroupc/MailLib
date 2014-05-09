package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;
import java.io.*;

import com.upokecenter.util.*;

    /**
     * Represents an email message. <p><b>Thread safety:</b> This class
     * is mutable; its properties can be changed. None of its methods are
     * designed to be thread safe. Therefore, access to objects from this
     * class must be synchronized if multiple threads can access them at
     * the same time.</p> <p>The following lists known deviations from
     * the mail specifications (Internet Message Format and MIME):</p>
     * <ul> <li>The content-transfer-encoding "quoted-printable" is
     * treated as 7bit instead if it occurs in a message or body part with content
     * type "multipart/*" or "message/*" (other than "message/global",
     * "message/global-headers", "message/global-disposition-notification",
     * or "message/global-delivery-status").</li> <li>Non-UTF-8 bytes
     * appearing in header field values are replaced with replacement characters.
     * Moreover, UTF-8 is parsed everywhere in header field values, even
     * in those parts of some structured header fields where this appears
     * not to be allowed.</li> <li>The To and Cc header fields are allowed
     * to contain only comments and whitespace, but these "empty" header
     * fields will be omitted when generating.</li> <li>There is no line
     * length limit imposed when parsing quoted-printable or base64 encoded
     * bodies.</li> <li>In non-MIME message bodies, in text/plain message
     * bodies, and in the prologue and epilogue of multipart messages (which
     * will be ignored), if the transfer encoding is absent or ((declared
     * instanceof 7bit) ? (7bit)declared : null), any 8-bit bytes are replaced
     * with question marks.</li> <li>The name <code>ascii</code> is treated
     * as a synonym for <code>us-ascii</code> , despite being a reserved
     * name under RFC 2046. The name <code>cp1252</code> is treated as a
     * synonym for <code>windows-1252</code> , even though it's not an
     * IANA registered alias.</li> <li>If a sequence of encoded words (RFC
     * 2047) decodes to a string with a CTL character (U + 007F, or a character
     * less than U + 0020 and not TAB) after being converted to Unicode, the
     * encoded words are left un-decoded.</li> </ul>
     */
  public final class Message {
    private static final int EncodingSevenBit = 0;
    private static final int EncodingUnknown = -1;
    private static final int EncodingEightBit = 3;
    private static final int EncodingBinary = 4;
    private static final int EncodingQuotedPrintable = 1;
    private static final int EncodingBase64 = 2;

    private List<String> headers;

    private List<Message> parts;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<Message> getParts() {
        return this.parts;
      }

    /**
     * Gets a snapshot of the header fields of this message. The list contains
     * an alternating set of header field names and values, in the order they
     * were declared in the message.
     * @return A snapshot of the header fields of this message. The list contains
     * an alternating set of header field names and values, in the order they
     * were declared in the message.
     */
    public List<String> getHeaders() {
        return new ArrayList<String>(this.headers);
      }

    private byte[] body;

    /**
     * Gets a value not documented yet.
     * @return A byte array.
     */
    public byte[] GetBody() {
      return this.body;
    }

    private static byte[] GetUtf8Bytes(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      try {
        java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

          if (DataUtilities.WriteUtf8(str, 0, str.length(), ms, true, true) != 0) {
            throw new IllegalArgumentException("Unpaired surrogate code point");
          }
          return ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
      } catch (IOException ex) {
        throw new IllegalArgumentException("I/O error occurred", ex);
      }
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

    /**
     * Sets the body of this message to the specified plain text string. The
     * character sequences CR, LF, and CR/LF will be converted to CR/LF line
     * breaks. Unpaired surrogate code points will be replaced with replacement
     * characters.
     * @param str A string object.
     * @return This instance.
     */
    public Message SetTextBody(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      this.body = GetUtf8Bytes(str);
      this.contentType = IsShortAndAllAscii(str) ? MediaType.TextPlainAscii : MediaType.TextPlainUtf8;
      return this;
    }

    /**
     * Sets the body of this message to the specified string in HTML format.
     * The character sequences CR, LF, and CR/LF will be converted to CR/LF
     * line breaks. Unpaired surrogate code points will be replaced with
     * replacement characters.
     * @param str A string object.
     * @return This instance.
     */
    public Message SetHtmlBody(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      this.body = GetUtf8Bytes(str);
      this.contentType = IsShortAndAllAscii(str) ? MediaType.TextPlainAscii : MediaType.TextPlainUtf8;
      return this;
    }

    /**
     * Sets the body of this message to a multipart body with plain text and
     * HTML versions of the same message. The character sequences CR, LF,
     * and CR/LF will be converted to CR/LF line breaks. Unpaired surrogate
     * code points will be replaced with replacement characters.
     * @param text A string object.
     * @param html A string object. (2).
     * @return This instance.
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
      this.contentType = MediaType.Parse("multipart/alternative; boundary=\"=_boundary\"");
      List<Message> parts = this.getParts();
      parts.clear();
      parts.add(textMessage);
      parts.add(htmlMessage);
      return this;
    }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<NamedAddress> getFromAddresses() {
        return ParseAddresses(this.GetHeader("from"));
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
          if (HeaderFields.GetParser(name).Parse(headerValue, 0, headerValue.length(), null) !=
              headerValue.length()) {
            return false;
          }
          have = true;
        }
      }
      return true;
    }

    static List<NamedAddress> ParseAddresses(String value) {
      Tokener tokener = new Tokener();
      if (value == null) {
        return new ArrayList<NamedAddress>();
      }
      if (HeaderParser.ParseHeaderTo(value, 0, value.length(), tokener) != value.length()) {
        // Invalid syntax
        return new ArrayList<NamedAddress>();
      }
      return HeaderParserUtility.ParseAddressList(value, 0, value.length(), tokener.GetTokens());
    }

    static List<NamedAddress> ParseAddresses(String[] values) {
      Tokener tokener = new Tokener();
      ArrayList<NamedAddress> list=new ArrayList<NamedAddress>();
      for(String addressValue : values) {
        if (addressValue == null) {
          continue;
        }
        if (HeaderParser.ParseHeaderTo(addressValue, 0, addressValue.length(), tokener) != addressValue.length()) {
          // Invalid syntax
          continue;
        }
        list.addAll(HeaderParserUtility.ParseAddressList(addressValue, 0, addressValue.length(), tokener.GetTokens()));
      }
      return list;
    }

    /**
     * Gets a list of addresses found in the To header field or fields.
     * @return A list of addresses found in the To header field or fields.
     */
    public List<NamedAddress> getToAddresses() {
        return ParseAddresses(this.GetMultipleHeaders("to"));
      }

    /**
     * Gets a list of addresses found in the CC header field or fields.
     * @return A list of addresses found in the CC header field or fields.
     */
    public List<NamedAddress> getCCAddresses() {
        return ParseAddresses(this.GetMultipleHeaders("cc"));
      }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<NamedAddress> getBccAddresses() {
        return ParseAddresses(this.GetMultipleHeaders("bcc"));
      }

    /**
     * Gets this message's subject.
     * @return This message's subject.
     */
    public String getSubject() {
        return this.GetHeader("subject");
      }
public void setSubject(String value) {
        this.SetHeader("subject", value);
      }

    /**
     * Gets the body of this message as a Unicode string.
     * @return The body of this message as a Unicode string.
     * @throws UnsupportedOperationException This message has no character encoding
     * declared on it, or the character encoding is not supported.
     */
    public String getBodyString() {
        java.io.ByteArrayInputStream ms=null;
try {
ms=new java.io.ByteArrayInputStream(this.body);

          Charsets.ICharset charset = Charsets.GetCharset(this.getContentType().GetCharset());
          if (charset == null) {
            throw new UnsupportedOperationException("Not in a supported character set.");
          }
          ITransform transform = new WrappedStream(ms);
          return charset.GetString(transform);
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
      }

    public Message (InputStream stream) {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      this.headers = new ArrayList<String>();
      this.parts = new ArrayList<Message>();
      this.ReadMessage(new WrappedStream(stream));
    }

    public Message () {
      this.headers = new ArrayList<String>();
      this.parts = new ArrayList<Message>();
      this.body = new byte[0];
    }

    /**
     * Returns the mail message contained in this message's body.
     * @return A message object if this object's content type is "message/rfc822"
     * or "message/global", or null otherwise.
     */

    public Message GetBodyMessage() {
      if (this.getContentType().getTopLevelType().equals("message") &&
          (this.getContentType().getSubType().equals("rfc822") || this.getContentType().getSubType().equals("global"))) {
        java.io.ByteArrayInputStream ms=null;
try {
ms=new java.io.ByteArrayInputStream(this.body);

          return new Message(ms);
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
      }
      return null;
    }

    private MediaType contentType;
    private ContentDisposition contentDisposition;
    private int transferEncoding;

    /**
     * Gets this message's media type.
     * @return This message's media type.
     */
    public MediaType getContentType() {
        return this.contentType;
      }
public void setContentType(MediaType value) {
        if (value == null) {
          throw new NullPointerException("value");
        }
        if (!this.getContentType().equals(value)) {
          this.contentType = value;
          if (!value.isMultipart()) {
            List<Message> parts = this.getParts();
            parts.clear();
          }
          this.SetHeader("content-type", this.contentType.toString());
        }
      }

    /**
     * Gets this message's content disposition.
     * @return This message's content disposition, or null if none is specified.
     */
    public ContentDisposition getContentDisposition() {
        return this.contentDisposition;
      }
public void setContentDisposition(ContentDisposition value) {
        if (value == null) {
          this.contentDisposition = null;
          this.RemoveHeader("content-disposition");
        } else if (!value.equals(this.contentDisposition)) {
          this.contentDisposition = value;
          this.SetHeader("content-disposition", this.contentDisposition.toString());
        }
      }

    /**
     * Gets a filename suggested by this message for saving the message's
     * body to a file.
     * @return A suggested name for the file, or the empty string if there
     * is no filename suggested by the content type or content disposition.
     */
    public String getFileName() {
        ContentDisposition disp = this.contentDisposition;
        if (disp != null) {
          return ContentDisposition.MakeFilename(disp.GetParameter("filename"));
        }
        return ContentDisposition.MakeFilename(this.contentType.GetParameter("name"));
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
          // fit the same syntax as the stricter one for top-level types and subtypes
          int endIndex = MediaType.skipMimeTypeSubtype(value, startIndex, value.length(), null);
          if (HeaderParser.ParseCFWS(value, endIndex, value.length(), null) == value.length()) {
            transferEncodingValue = value.substring(startIndex,(startIndex)+(endIndex - startIndex));
          } else {
            transferEncodingValue = "";
          }
        }
        if (name.equals("mime-version")) {
          mime = true;
        }
        if (value.indexOf("=?") >= 0) {
          IHeaderFieldParser parser = HeaderFields.GetParser(name);
          // Decode encoded words in the header field where possible
          value = parser.DecodeEncodedWords(value);
          this.headers.set(i + 1,value);
        }
      }
      this.contentType = digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii;
      for (int i = 0; i < this.headers.size(); i += 2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (mime && name.equals("content-transfer-encoding")) {
          value = DataUtilities.ToLowerCaseAscii(transferEncodingValue);
          this.headers.set(i + 1,value);
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
        } else if (mime && name.equals("content-type")) {
          if (haveContentType) {
            String valueExMessage = "Already have this header: " + name;
            /*
            valueExMessage+="[old="+this.contentType+", new="+value+"]";
            valueExMessage = valueExMessage.replace("\r\n"," ");
             */
            throw new MessageDataException(valueExMessage);
          }
          this.contentType = MediaType.Parse(
            value,
            digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii);
          haveContentType = true;
        } else if (mime && name.equals("content-disposition")) {
          if (haveContentDisp) {
            String valueExMessage = "Already have this header: " + name;
            /*
            valueExMessage+="[old="+this.contentType+", new="+value+"]";
            valueExMessage = valueExMessage.replace("\r\n"," ");
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
        if (this.contentType.isMultipart() ||
            (this.contentType.getTopLevelType().equals("message") &&
             !this.contentType.getSubType().equals("global") &&
             !this.contentType.getSubType().equals("global-headers") &&
             !this.contentType.getSubType().equals("global-disposition-notification") &&
             !this.contentType.getSubType().equals("global-delivery-status"))) {
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
          (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
          c == 0x20 || c == 0x2c || "'()-./+_:=?".indexOf(c) >= 0)) {
          return false;
        }
      }
      return true;
    }

    /**
     * Gets the first instance of the header field with the specified name,
     * comparing the field name in an ASCII case-insensitive manner.
     * @param name A string object.
     * @return The value of the first header field with that name, or null
     * if there is none.
     */
    public String GetHeader(String name) {
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          // Get the first instance of the header field
          return this.headers.get(i + 1);
        }
      }
      return null;
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
      for(String s : strings) {
        if (!first) {
          sb.append(delim);
        }
        sb.append(s);
      }
      return sb.toString();
    }

    private String[] GetMultipleHeaders(String name) {
      ArrayList<String> headers=new ArrayList<String>();
      name = DataUtilities.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          headers.add(this.headers.get(i));
        }
      }
      return headers.toArray(new String[]{});
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
      for (int i = 0; i < len; ++i) {
        char c = s.charAt(i);
        if (c == 0x0d) {
          if (i + 1 >= len || s.charAt(i + 1) != 0x0a) {
            // bare CR
            return false;
          } else if (i + 2 >= len || (s.charAt(i + 2) != 0x09 && s.charAt(i + 2) != 0x20)) {
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
        return ParseUnstructuredText(s, 0, s.length()) == s.length();
      }
      return true;
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
        if (s.charAt(i) == '-') {
          afterHyphen = true;
        } else {
          afterHyphen = false;
        }
      }
      String ret = builder.toString();
      if (ret.equals("Mime-Version")) {
        return "MIME-Version";
      }
      if (ret.equals("Message-Id")) {
        return "Message-ID";
      }
      return ret;
    }

    /**
     * Returns true if the string has: * non-ASCII characters * "=?" * CTLs
     * other than tab, or * a word longer than 75 characters. Can return false
     * even if the string has: * CRLF followed by a line with just whitespace.
     * @param s A string object.
     * @return A Boolean object.
     */
    static boolean HasTextToEscape(String s) {
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
          } else if (i + 2 >= len || (s.charAt(i + 2) != 0x09 && s.charAt(i + 2) != 0x20)) {
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

    static boolean HasTextToEscapeIgnoreEncodedWords(String s, int index, int endIndex) {
      int len = endIndex;
      int chunkLength = 0;
      for (int i = index; i < endIndex; ++i) {
        char c = s.charAt(i);
        if (c == 0x0d) {
          if (i + 1 >= len || s.charAt(i + 1) != 0x0a) {
            // bare CR
            // System.out.println("bare CR");
            return true;
          } else if (i + 2 >= len || (s.charAt(i + 2) != 0x09 && s.charAt(i + 2) != 0x20)) {
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

    static int ParseUnstructuredText(String str, int index, int endIndex) {
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
                    while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
                      ++index;
                    }
                    if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) == 10) {
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
                if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
                  ++index;
                  while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
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
                if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                  indexTemp3 += 2; break;
                }
                if (index < endIndex && (str.charAt(index) >= 33 && str.charAt(index) <= 126)) {
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

    /**
     * Sets the value of this message's header field. If a header field with
     * the same name exists, its value is replaced.
     * @param name The name of a header field, such as &quot;from&quot; or
     * &quot;subject&quot;.
     * @param value The header field&apos;s value.
     * @return This instance.
     * @throws java.lang.IllegalArgumentException The header field name is too long
     * or contains an invalid character, or the header field's value is syntactically
     * invalid.
     */
    public Message SetHeader(String name, String value) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (value == null) {
        throw new NullPointerException("value");
      }
      if (name.length() > 997) {
        throw new IllegalArgumentException("Header field name too long");
      }
      for (int i = 0; i < name.length(); ++i) {
        if (name.charAt(i) <= 0x20 || name.charAt(i) == ':' || name.charAt(i) >= 0x7f) {
          throw new IllegalArgumentException("Header field name contains an invalid character");
        }
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      // Check characters in structured header fields
      if (HeaderFields.GetParser(name).IsStructured()) {
        if (ParseUnstructuredText(value, 0, value.length()) != value.length()) {
          throw new IllegalArgumentException("Header field value contains invalid text");
        }
      }
      // Add the header field
      for (int i = 0; i < this.headers.size(); i += 2) {
        if (this.headers.get(i).equals(name)) {
          this.headers.set(i + 1,value);
          return this;
        }
      }
      this.headers.add(name);
      this.headers.add(value);
      return this;
    }

    /**
     * Not documented yet.
     * @param name A string object.
     * @return A Message object.
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
      return this;
    }

    private static boolean StartsWithWhitespace(String str) {
      return str.length() > 0 && (str.charAt(0) == ' ' || str.charAt(0) == 0x09 || str.charAt(0) == '\r');
    }

    private static int TransferEncodingToUse(byte[] body, boolean isBodyPart) {
      if (body == null || body.length == 0) {
        return EncodingSevenBit;
      }
      int lengthCheck = Math.min(body.length, 4096);
      int highBytes = 0;
      int ctlBytes = 0;
      int lineLength = 0;
      boolean allTextBytes = isBodyPart ? false : true;
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
          if (i + 1 >= body.length || body[i + 1] != (byte)'\n') {
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
        if (lineLength == 0 && i + 2 < body.length &&
            body[i] == '.' && body[i + 1] == '\r' && body[i + 2] == '\n') {
          // See RFC2049 sec. 3
          allTextBytes = false;
        }
        if (lineLength == 0 && i + 4 < body.length &&
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
      if (lengthCheck == body.length && allTextBytes) {
        return EncodingSevenBit;
      } if (highBytes > (lengthCheck / 3)) {
        return EncodingBase64;
      } if (ctlBytes > 10) {
        return EncodingBase64;
      } else {
        return EncodingQuotedPrintable;
      }
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

    static boolean CanBeUnencoded(byte[] bytes, boolean checkBoundaryDelimiter) {
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
            bytes[index] == '-' &&
            bytes[index + 1] == '-' &&
            bytes[index + 2] == '=' &&
            bytes[index + 3] == '_' &&
            bytes[index + 4] == 'B') {
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
          // System.out.println("Bare CR or bare LF");
          return false;
        }
        ++lineLength;
        if (lineLength > 78) {
          // System.out.println("Line length exceeded (" + maxLineLength + " " + (str.substring(index-78,(index-78)+(78))) + ")");
          return false;
        }
        ++index;
      }
      return true;
    }

    /**
     * Generates this message's data in text form.<p>The generated message
     * will always be 7-bit ASCII, and the transfer encoding will always
     * be 7bit, quoted-printable, or base64 (the declared transfer encoding
     * for this message will be ignored).</p> <p>The following applies
     * to the From, To, Cc, and Bcc header fields. If the header field has an
     * invalid syntax or has no addresses, this method will generate a synthetic
     * header field with the display-name set to the contents of all of the
     * header fields with the same name, and the address set to <code>me@[header-name]-address.invalid</code>
     * as the address (a <code>.invalid</code> address is a reserved address
     * that can never belong to anyone). </p>
     * @return The generated message.
     * @throws MessageDataException The message can't be generated.
     */
    public String Generate() {
      return this.Generate(0);
    }

    private static String GenerateBoundary(int num) {
      StringBuilder sb = new StringBuilder();
      String hex = "0123456789ABCDEF";
      sb.append("=_Boundary");
      for (int i = 0; i < 8; ++i) {
        int b = (num >> 56) & 255;
        sb.append(hex.charAt((b >> 4) & 15));
        sb.append(hex.charAt(b & 15));
        num <<= 8;
      }
      return sb.toString();
    }

    private String SynthesizeField(String name) {
      String fullField = Implode(this.GetMultipleHeaders(name), ", ");
      String value = new EncodedWordEncoder().AddString(fullField).FinalizeEncoding().toString();
      if (value.length() > 0) {
        value += " <me@"+name+"address.invalid>";
      } else {
        value = "me@"+name+"-address.invalid";
      }
      return value;
    }

    private String Generate(int depth) {
      StringBuilder sb = new StringBuilder();
      boolean haveMimeVersion = false;
      boolean haveContentEncoding = false;
      boolean haveContentType = false;
      boolean haveContentDisp = false;
      boolean haveFrom = false;
      boolean outputtedFrom = false;
      boolean haveTo = false;
      byte[] bodyToWrite = this.body;
      boolean haveCc = false;
      boolean haveBcc = false;
      MediaTypeBuilder builder = new MediaTypeBuilder(this.getContentType());
      String contentDisp = (this.getContentDisposition() == null) ? null :
        this.getContentDisposition().toString();
      int transferEncoding = 0;
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
          if (builder.getSubType().equals("rfc822") && !msgCanBeUnencoded) {
            builder.SetSubType("global");
          } else if (builder.getSubType().equals("disposition-notification") && !msgCanBeUnencoded) {
            builder.SetSubType("global-disposition-notification");
          } else if (builder.getSubType().equals("delivery-status") && !msgCanBeUnencoded) {
            builder.SetSubType("global-delivery-status");
          } else if (!msgCanBeUnencoded) {
            throw new MessageDataException("Message body can't be encoded");
          }
        }
      }
      String topLevel = builder.getTopLevelType();
      if (topLevel.equals("message") || topLevel.equals("multipart")) {
        if (topLevel.equals("multipart") || (
          !builder.getSubType().equals("global") &&
          !builder.getSubType().equals("global-headers") &&
          !builder.getSubType().equals("global-disposition-notification") &&
          !builder.getSubType().equals("global-delivery-status"))) {
          transferEncoding = EncodingSevenBit;
        } else {
          transferEncoding = TransferEncodingToUse(bodyToWrite, depth > 0);
        }
      } else {
        transferEncoding = TransferEncodingToUse(bodyToWrite, depth > 0);
      }
      String encodingString = "7bit";
      if (transferEncoding == EncodingBase64) {
        encodingString = "base64";
      } else if (transferEncoding == EncodingQuotedPrintable) {
        encodingString = "quoted-printable";
      }
      // Write the header fields
      for (int i = 0; i < this.headers.size(); i += 2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (name.equals("mime-version")) {
          haveMimeVersion = true;
        }
        if (name.equals("content-type")) {
          if (haveContentType) {
            // Already outputted, continue
            continue;
          }
          haveContentType = true;
          value = builder.toString();
        } if (name.equals("content-disposition")) {
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
        } else if (name.equals("from")) {
          if (haveFrom) {
            // Already outputted, continue
            continue;
          }
          haveFrom = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.getFromAddresses());
            if (value.length() == 0) {
              // No addresses, synthesize a From field
              value = SynthesizeField(name);
            }
          }
          outputtedFrom = true;
        } else if (name.equals("to")) {
          if (haveTo) {
            // Already outputted, continue
            continue;
          }
          haveTo = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.getToAddresses());
            if (value.length() == 0) {
              // No addresses, synthesize a field
              value = SynthesizeField(name);
            }
          }
        } else if (name.equals("cc")) {
          if (haveCc) {
            // Already outputted, continue
            continue;
          }
          haveCc = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.getCCAddresses());
            if (value.length() == 0) {
              // No addresses, synthesize a field
              value = SynthesizeField(name);
            }
          }
        } else if (name.equals("bcc")) {
          if (haveBcc) {
            // Already outputted, continue
            continue;
          }
          haveBcc = true;
          if (!this.IsValidAddressingField(name)) {
            value = GenerateAddressList(this.getBccAddresses());
            if (value.length() == 0) {
              // No addresses, synthesize a field
              value = SynthesizeField(name);
            }
          }
        }
        // TODO: Reply-To, Sender, Resent-From/-To/-Bcc/-Cc/-Sender
        String rawField = Capitalize(name) + ":" +
          (StartsWithWhitespace(value) ? "" : " ") + value;
        if (CanOutputRaw(rawField)) {
          sb.append(rawField);
        } else if (HasTextToEscape(value)) {
          String downgraded = HeaderFields.GetParser(name).DowngradeFieldValue(value);
          if (HasTextToEscapeIgnoreEncodedWords(downgraded, 0, downgraded.length())) {
            if (name.equals("message-id") ||
                name.equals("resent-message-id") ||
                name.equals("in-reply-to") ||
                name.equals("references") ||
                name.equals("original-recipient") ||
                name.equals("final-recipient")) {
              // Header field still contains non-ASCII characters, convert
              // to a downgraded field
              name = "downgraded-" + name;
              downgraded = Rfc2047.EncodeString(ParserUtility.TrimSpaceAndTab(value));
            } else {
              throw new MessageDataException("Header field still has non-Ascii: " + name + " " + value);
            }
          }
          boolean haveDquote = downgraded.indexOf('"') >= 0;
          WordWrapEncoder encoder=new WordWrapEncoder(Capitalize(name) + ":", !haveDquote);
          encoder.AddString(downgraded);
          sb.append(encoder.toString());
        } else {
          boolean haveDquote = value.indexOf('"') >= 0;
          WordWrapEncoder encoder=new WordWrapEncoder(Capitalize(name) + ":", !haveDquote);
          encoder.AddString(value);
          sb.append(encoder.toString());
        }
        sb.append("\r\n");
      }
      if (!outputtedFrom && depth == 0) {
        // Output a synthetic From field if it doesn't
        // exist and this isn't a body part
        sb.append("From: me@author-address.invalid\r\n");
      }
      if (!haveMimeVersion && depth == 0) {
        sb.append("MIME-Version: 1.0\r\n");
      }
      if (!haveContentType) {
        sb.append("Content-Type: " + builder.toString() + "\r\n");
      }
      if (!haveContentEncoding) {
        sb.append("Content-Transfer-Encoding: " + encodingString + "\r\n");
      }
      IStringEncoder bodyEncoder = null;
      switch (transferEncoding) {
        case EncodingBase64:
          bodyEncoder = new Base64Encoder(true, builder.isText() ? true : false, false);
          break;
        case EncodingQuotedPrintable:
          bodyEncoder = new QuotedPrintableEncoder(builder.isText() ? 2 : 0, false);
          break;
        default:
          bodyEncoder = new IdentityEncoder();
          break;
      }
      // Write the body
      sb.append("\r\n");
      if (!isMultipart) {
        bodyEncoder.WriteToString(sb, bodyToWrite, 0, bodyToWrite.length);
        bodyEncoder.FinalizeEncoding(sb);
      } else {
        for(Message part : this.getParts()) {
          sb.append("\r\n--" + boundary + "\r\n");
          sb.append(part.Generate(depth + 1));
        }
        sb.append("\r\n--" + boundary + "--");
      }
      return sb.toString();
    }

    private static int ReadUtf8Char(TransformWithUnget stream, int[] bytesRead) {
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

    static String DowngradeRecipientHeaderValue(String headerValue) {
      return DowngradeRecipientHeaderValue(headerValue, null);
    }

    static String DowngradeRecipientHeaderValue(String headerValue, int[] status) {
      int index;
      if (HasTextToEscapeIgnoreEncodedWords(headerValue, 0, headerValue.length())) {
        index = HeaderParser.ParseCFWS(headerValue, 0, headerValue.length(), null);
        int atomText = HeaderParser.ParsePhraseAtom(headerValue, index, headerValue.length(), null);
        int typeEnd = atomText;
        String origValue = headerValue;
        boolean isUtf8 = typeEnd - index == 5 &&
          (headerValue.charAt(index) & ~0x20) == 'U' &&
          (headerValue.charAt(index + 1) & ~0x20) == 'T' &&
          (headerValue.charAt(index + 2) & ~0x20) == 'F' &&
          headerValue.charAt(index + 3) == '-' &&
          headerValue.charAt(index + 4) == '8';
        atomText = HeaderParser.ParseCFWS(headerValue, atomText, headerValue.length(), null);
        if (index < headerValue.length() && headerValue.charAt(atomText) == ';') {
          String typePart = headerValue.substring(0,atomText + 1);
          // Downgrade the comments in the type part
          // NOTE: Final-recipient has the same syntax as original-recipient,
          // except for the header field name
          typePart = HeaderFields.GetParser("original-recipient").DowngradeFieldValue(typePart);
          if (isUtf8) {
            // Downgrade the non-ASCII characters in the address
            StringBuilder builder = new StringBuilder();
            String hex = "0123456789ABCDEF";
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
                    builder.append(hex.charAt((cp >> j) & 15));
                  }
                }
                builder.append('}');
              }
            }
            headerValue = typePart + builder.toString();
          } else {
            headerValue = typePart + headerValue.substring(atomText + 1);
          }
        }
        if (HasTextToEscapeIgnoreEncodedWords(headerValue, 0, headerValue.length())) {
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
    static byte[] DowngradeDeliveryStatus(byte[] bytes) {
      // int lineCount = 0;
      StringBuilder sb = new StringBuilder();
      int index = 0;
      int endIndex = bytes.length;
      int lastIndex = -1;
      ArrayWriter writer = null;
      while (index < endIndex) {
        sb.delete(0,(0)+(sb.length()));
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
        String origFieldName =
          DataUtilities.GetUtf8String(bytes, headerNameStart, headerValueStart - headerNameStart, true);
        String fieldName = DataUtilities.ToLowerCaseAscii(
          DataUtilities.GetUtf8String(bytes, headerNameStart, headerNameEnd - headerNameStart, true));
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
            byte[] newBytes = DataUtilities.GetUtf8Bytes(encoder.toString(), true);
            writer.WriteBytes(newBytes, 0, newBytes.length);
            lastIndex = headerValueEnd;
          }
        }
      }
      if (writer != null) {
        writer.WriteBytes(bytes, lastIndex, bytes.length - lastIndex);
        bytes = writer.ToArray();
      }
      return bytes;
    }

    private static void ReadHeaders(
      ITransform stream,
      List<String> headerList) {
      int lineCount = 0;
      int[] bytesRead = new int[1];
      StringBuilder sb = new StringBuilder();
      TransformWithUnget ungetStream = new TransformWithUnget(stream);
      while (true) {
        sb.delete(0,(0)+(sb.length()));
        boolean first = true;
        boolean endOfHeaders = false;
        boolean wsp = false;
        lineCount = 0;
        while (true) {
          int c = ungetStream.read();
          if (c == -1) {
            throw new MessageDataException("Premature end before all headers were read");
          }
          ++lineCount;
          if (first && c == '\r') {
            if (ungetStream.read() == '\n') {
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
            sb.append((char)c);
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
        if (sb.length() == 0) {
          throw new MessageDataException("Empty header field name");
        }
        String fieldName = sb.toString();
        sb.delete(0,(0)+(sb.length()));
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
                      sb.append("\r\n");
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
                  sb.append((char)c2);
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
              sb.append('\r');
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
            sb.append((char)c);
          } else if (c <= 0x10ffff) {
            sb.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
            sb.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
          }
        }
        String fieldValue = sb.toString();
        headerList.add(fieldName);
        headerList.add(fieldValue);
      }
    }

    private static class MessageStackEntry {
      private Message message;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public Message getMessage() {
          return this.message;
        }

      private String boundary;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public String getBoundary() {
          return this.boundary;
        }

      public MessageStackEntry (Message msg) {

        this.message = msg;
        MediaType mediaType = msg.getContentType();
        if (mediaType.isMultipart()) {
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
      List<MessageStackEntry> multipartStack = new ArrayList<MessageStackEntry>();
      MessageStackEntry entry = new Message.MessageStackEntry(this);
      multipartStack.add(entry);
      boundaryChecker.PushBoundary(entry.getBoundary());
      Message leaf = null;
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.length;
      this.body = new byte[0];
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

        while (true) {
          int ch = 0;
          try {
            ch = currentTransform.read();
          } catch (MessageDataException ex) {
            String valueExMessage = ex.getMessage();
            /*
            ms.write(buffer,0,bufferCount);
            buffer = ms.toByteArray();
            String ss = DataUtilities.GetUtf8String(
              buffer,
              Math.max(buffer.length - 35, 0),
              Math.min(buffer.length, 35),
              true);
            String transferEnc = (leaf ?? this).GetHeader("content-transfer-encoding");
            valueExMessage += " ["+ss+"] [type="+((leaf ?? this).getContentType() ?? MediaType.TextPlainAscii)+
              "] [encoding=" + transferEnc+"]";
            valueExMessage = valueExMessage.replace('\r',' ').replace('\n',' ').replace('\0',' ');
             */
            throw new MessageDataException(valueExMessage);
          }
          if (ch < 0) {
            if (boundaryChecker.getHasNewBodyPart()) {
              Message msg = new Message();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack

              if (leaf != null) {
                if (bufferCount > 0) {
                  ms.write(buffer,0,bufferCount);
                }
                leaf.body = ms.toByteArray();
                // Clear for the next body
                ms.reset();
                bufferCount = 0;
              } else {
                // Clear for the next body
                bufferCount = 0;
                ms.reset();
              }
              while (multipartStack.size() > stackCount) {
                multipartStack.remove(stackCount);
              }
              Message parentMessage = multipartStack.get(multipartStack.size() - 1).getMessage();
              boundaryChecker.StartBodyPartHeaders();
              ReadHeaders(stream, msg.headers);
              MediaType ctype = parentMessage.getContentType();
              boolean parentIsDigest = ctype.getSubType().equals("digest") &&
                ctype.isMultipart();
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.getParts().add(msg);
              multipartStack.add(entry);
              ms.reset();
              ctype = msg.getContentType();
              if (ctype.isMultipart()) {
                leaf = null;
              } else {
                leaf = msg;
              }
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
                  ms.write(buffer,0,bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.toByteArray();
              }
              return;
            }
          } else {
            buffer[bufferCount++] = (byte)ch;
            if (bufferCount >= bufferLength) {
              ms.write(buffer,0,bufferCount);
              bufferCount = 0;
            }
          }
        }
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    private static ITransform MakeTransferEncoding(
      ITransform stream,
      int encoding,
      boolean useLiberalSevenBit) {
      ITransform transform = new EightBitTransform(stream);
      if (encoding == EncodingQuotedPrintable) {
        // NOTE: The max line size is actually 76, but some emails
        // have lines that exceed this size, so use an unlimited line length
        // when parsing
        transform = new QuotedPrintableTransform(stream, false, -1);
        // transform = new QuotedPrintableTransform(stream, false, 76, true);
      } else if (encoding == EncodingBase64) {
        // NOTE: Same as quoted-printable regarding line length
        transform = new Base64Transform(stream, false, -1, false);
        // transform = new Base64Transform(stream, false, 76, true);
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
        this.getContentType().getTypeAndSubType().equals("text/plain"));
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.length;
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new java.io.ByteArrayOutputStream();

        while (true) {
          int ch = 0;
          try {
            ch = transform.read();
          } catch (MessageDataException ex) {
            String valueExMessage = ex.getMessage();
            /*
            ms.write(buffer,0,bufferCount);
            buffer = ms.toByteArray();
            String ss = DataUtilities.GetUtf8String(
              buffer,
              Math.max(buffer.length - 35, 0),
              Math.min(buffer.length, 35),
              true);
            String transferEnc=this.GetHeader("content-transfer-encoding");
            valueExMessage+=" ["+ss+"] [type="+(this.getContentType() ?? MediaType.TextPlainAscii)+
              "] [encoding=" + transferEnc+"]";
            valueExMessage = valueExMessage.replace('\r',' ').replace('\n',' ').replace('\0',' ');
             */
            throw new MessageDataException(valueExMessage, ex);
          }
          if (ch < 0) {
            break;
          }
          buffer[bufferCount++] = (byte)ch;
          if (bufferCount >= bufferLength) {
            ms.write(buffer,0,bufferCount);
            bufferCount = 0;
          }
        }
        if (bufferCount > 0) {
          ms.write(buffer,0,bufferCount);
        }
        this.body = ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch (java.io.IOException ex){}
}
    }

    private void ReadMessage(ITransform stream) {
      ReadHeaders(stream, this.headers);
      this.ProcessHeaders(false, false);
      if (this.contentType.isMultipart()) {
        this.ReadMultipartBody(stream);
      } else {
        this.ReadSimpleBody(stream);
      }
    }
  }
