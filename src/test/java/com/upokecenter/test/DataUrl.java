package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;

import org.junit.Assert;

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public class DataUrl {
    public static MediaType DataUrlMediaType(String url) {
      String[] parts = URIUtility.splitIRIToStrings(
        url);
      if (parts == null || parts[0] == null || parts[2] == null) {
        return null;
      }
      String path = parts[2];
      if (parts[0].equals("data")) {
        int mediaTypePart = path.indexOf(',');
        if (mediaTypePart == -1) {
          return null;
        }
        String mediaType = path.substring(0, mediaTypePart);
        // Strip out ";base64" at end
        if (mediaType.length() >= 7 &&
           mediaType.substring(mediaType.length() - 7).toLowerCase()
            .equals(";base64")) {
          mediaType = mediaType.substring(0, mediaType.length() - 7);
        }
        if (mediaType.length() == 0 || mediaType.charAt(0) == ';') {
          // Under RFC 2397, the type and subtype can be left
          // out. If left out, the media
          // type "text/plain" is assumed.
          mediaType = "text/plain" + mediaType;
        }
        if (mediaType.indexOf('/') < 0) {
        }
        if (mediaType.indexOf('(') >= 0) {
          // The media type String has parentheses
          // (comment delimiters), which are not valid in Data-URL
          // media types since ABNFs (at the time of RFC 2397) did not allow
          // white space to be specified implicitly (RFC 2234).
          // However, since comments are still allowed in URI paths and
          // MediaType.Parse will skip comments when appropriate,
          // there is a need to check for comment delimiters here.
          // On the other hand, URIs already don't allow white space
          // or line breaks, so there is no need to check for those
          // here since ParseIRI already did that.
          // NOTE: This code returns null, but another alternative
          // is to use MediaType.TextPlainAscii, the recommended default
          // media type for
          // syntactically invalid Content-Type values (RFC 2045 sec. 5.2).
          // However, returning null here is the saner thing to do,
          // since the purported data URL might be problematic in other ways.
          return null;
        }
        return MediaType.Parse(mediaType, null);
      }
      return null;
    }

    static void TestDataUrlRoundTrip(String data) {
      MediaType mt = DataUrlMediaType(data);
      byte[] bytes = DataUrlBytes(data);
      if ((mt) == null) {
 Assert.fail(data);
 }
      if ((bytes) == null) {
 Assert.fail(data);
 }
      String data2 = MakeDataUrl(bytes, mt);
      MediaType mt2 = DataUrlMediaType(data2);
      byte[] bytes2 = DataUrlBytes(data2);
      TestCommon.AssertByteArraysEqual(bytes, bytes2);
      Assert.assertEquals(data, mt, mt2);
    }

    private static int ToHex(char b1) {
      if (b1 >= '0' && b1 <= '9') {
        return b1 - '0';
      } else if (b1 >= 'A' && b1 <= 'F') {
        return b1 + 10 - 'A';
      } else {
        return (b1 >= 'a' && b1 <= 'f') ? (b1 + 10 - 'a') : 1;
      }
    }

    public static Message MailToUrlMessage(String url) {
      String[] parts = URIUtility.splitIRIToStrings(
        url);
      if (parts == null || parts[0] == null) {
        return null;
      }

      if (parts[0].equals("mailto")) {
        Message msg = new Message();
        String emails = "";
        if (!((parts[2]) == null || (parts[2]).length() == 0)) {
          // Extract the email address
          emails = URIUtility.PercentDecode(parts[2]);
if (HeaderParser.ParseHeaderEmail(emails, 0, emails.length()) !=
            emails.length()) {
            System.out.println(emails);
            return null;
          }
        }
        if (parts[3] != null) {
          // If query String is present it must not be empty
          String query = parts[3];
          if (query.length() == 0) {
 return null;
}
          int index = 0;
          while (index < query.length()) {
            int startName = index;
            int endName = -1;
            int startValue = -1;
            int endValue = -1;
            while (index < query.length()) {
              if (query.charAt(index) == '&') {
                if (endName < 0) {
 return null;
}
                endValue = index;
                ++index;
                break;
              } else if (query.charAt(index) == '=') {
                if (endName >= 0) {
 return null;
}
                endName = index;
                startValue = index + 1;
                ++index;
              } else {
 ++index;
}
            }
            if (endName < 0) {
 return null;
  } else if (endValue < 0) {
 endValue = query.length();
}
            String name = query.substring(startName, (startName)+(endName - startName));
            String value = query.substring(startValue, (startValue)+(endValue - startValue));
         name =
  DataUtilities.ToLowerCaseAscii(URIUtility.PercentDecode(name));
            value = URIUtility.PercentDecode(value);
            // Support only To, Cc, Bcc, Subject, In-Reply-To,
            // Keywords, Comments, and Body.
            // Of these, the first four can appear only once in a message
            if (name.equals("body")) {
              msg.SetTextBody(value);
            } else if (name.equals("keywords") || name.equals("comments")) {
              msg.AddHeader(name, value);
            } else if (name.equals("subject") ||
              name.equals("to") || name.equals("cc") || name.equals("bcc") ||
              name.equals("in-reply-to")) {
              // TODO: Decode encoded words
              msg.SetHeader(name, value);
            } else if (name.equals("to")) {
              if (((emails) == null || (emails).length() == 0)) {
 emails = value;
} else {
 emails += "," + value;
}
            } else {
              System.out.println(name);
              System.out.println(value);
            }
          }
        }
        if (!((emails) == null || (emails).length() == 0)) {
          if (emails.indexOf('(') >= 0) {
            // Contains opening parenthesis, a comment delimiter
            return null;
          }
          try {
            msg.SetHeader("to", emails);
          } catch (Exception ex) {
            System.out.println(emails);
            return null;
          }
        }
        System.out.println(msg.Generate());
        return msg;
      }
      return null;
    }

    static final int[] Alphabet = { -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
      52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
      41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

    public static byte[] DataUrlBytes(String url) {
      String[] parts = URIUtility.splitIRIToStrings(
        url);
      if (parts == null || parts[0] == null || parts[2] == null) {
        return null;
      }
      String path = parts[2];
      if (parts[0].equals("data")) {
        int mediaTypePart = path.indexOf(',');
        if (mediaTypePart == -1) {
          return null;
        }
     boolean usesBase64 = (mediaTypePart >= 7 && path.substring(mediaTypePart -
          7, (mediaTypePart -
          7)+(7)).toLowerCase().equals(";base64"));
        // NOTE: Rejects base64 if non-base64 characters
        // are present, since RFC 2397 doesn't state otherwise
        // (see RFC 4648). Base 64 also uses no line breaks
        // even if longer than 76 characters,
        // since RFC 2397 doesn't state otherwise
        // (see RFC 4648).
        PeterO.ArrayWriter aw = new PeterO.ArrayWriter();
        if (usesBase64) {
          int base64Length = path.length() - (mediaTypePart + 1);
          if ((base64Length % 4) != 0) {
            return null;
          }
          for (var i = mediaTypePart + 1; i < path.length(); i += 4) {
            boolean lastBlock = i + 4 >= path.length();
            int b1 = 0, b2 = 0, b3 = 0, b4 = 0;
            b1 = (path.charAt(i) > 0x7f) ? -1 : Alphabet[(int)path.charAt(i)];
            b2 = (path.charAt(i + 1) > 0x7f) ? -1 : Alphabet[(int)path.charAt(i + 1)];
            if (lastBlock && path.charAt(i + 2) == '=' && path.charAt(i + 3) == '=') {
            } else if (lastBlock && path.charAt(i + 3) == '=') {
              b3 = (path.charAt(i + 2) > 0x7f) ? -1 : Alphabet[(int)path.charAt(i + 2)];
            } else {
              b3 = (path.charAt(i + 2) > 0x7f) ? -1 : Alphabet[(int)path.charAt(i + 2)];
              b4 = (path.charAt(i + 3) > 0x7f) ? -1 : Alphabet[(int)path.charAt(i + 3)];
            }
            if (b1 < 0 || b2 < 0 || b3 < 0 || b4 < 0) {
              return null;
            }
            int v = (b1 << 18) | (b2 << 12) | (b3 << 6) | b4;
            aw.write((byte)((v >> 16) & 0xff));
            if (path.charAt(i + 2) != '=') {
              aw.write((byte)((v >> 8) & 0xff));
            }
            if (path.charAt(i + 3) != '=') {
              aw.write((byte)(v & 0xff));
            }
          }
        } else {
          for (var i = mediaTypePart + 1; i < path.length();) {
            if (path.charAt(i) == '%') {
              // NOTE: No further character checking done here
              // because splitURI already did all needed pct-encoding checking
              aw.write((byte)((ToHex(path.charAt(i + 1)) << 4) + ToHex(path.charAt(i +
                    2))));
              i += 3;
            } else if (path.charAt(i) >= 0x80) {
              // RFC 2397 allows only "safe" ASCII here
              return null;
            } else {
              aw.write((byte)path.charAt(i));
              ++i;
            }
          }
        }
        return aw.ToArray();
      }
      return null;
    }

    private static final String Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghi" +
  "jklmnopqrstuvwxyz0123456789+/";

    private static void AppendBase64(StringBuilder builder, byte[] bytes) {
      int b1 = 0;
      int b2 = 0;
      int quantumCount = 0;
      for (int i = 0; i < bytes.length; ++i) {
        int value = bytes[i] & 0xff;
        switch (quantumCount) {
          case 2:
            builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
            builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4) &
              15)));
            builder.append((char)Base64Classic.charAt(((b2 & 15) << 2) + ((value >>
              6) & 3)));
            builder.append((char)Base64Classic.charAt(value & 63));
            quantumCount = 0;
            break;
          case 1:
            b2 = value;
            quantumCount = 2;
            break;
          case 0:
            b1 = value;
            quantumCount = 1;
            break;
        }
      }
      if (quantumCount == 2) {
        builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
        builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4) &
          15)));
        builder.append((char)Base64Classic.charAt(((b2 & 15) << 2)));
        builder.append('=');
      } else if (quantumCount == 1) {
        builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
        builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4) &
          15)));
        builder.append("==");
      }
    }

    public static String MakeDataUrl(byte[] bytes, MediaType mediaType) {
      StringBuilder builder = new StringBuilder();
      builder.append("data:");
      String mediaTypeString = mediaType.ToUriSafeString();
      if (mediaType.getTypeAndSubType().equals("text/plain")) {
        if (mediaTypeString.substring(0,10).equals("text/plain")) {
          // Strip 'text/plain' from the media type String,
          // since that's the default for data URIs
          mediaTypeString = mediaTypeString.substring(10);
        }
      }
      builder.append(mediaTypeString);
      builder.append(";base64,");
      AppendBase64(builder, bytes);
      return builder.toString();
    }
  }
