package com.upokecenter.mail;

import com.upokecenter.util.*;

  /**
   * <p>Contains methods for parsing and generating Data URIs (uniform resource
   * identifiers). Data URIs are described in RFC 2397. Examples for Data URIs
   * follow. </p><pre>data:, hello%20world</pre> <pre>data:text/markdown,
   * hello%20world</pre> <pre>data:application/octet-stream;base64,
   * AAAAAA==</pre>.
   */
  public final class DataUris {
private DataUris() {
}
    /**
     * Extracts the media type from a Data URI (uniform resource identifier) in the
     * form of a text string.
     * @param uri A data URI in the form of a text string.
     * @return The media type. Returns null if {@code uri} is null, is
     * syntactically invalid, or is not a Data URI.
     * @throws NullPointerException The parameter {@code uri} is null.
     */
    public static MediaType DataUriMediaType(String uri) {
      if (uri == null) {
        throw new NullPointerException("uri");
      }
      String url = uri;
      String[] parts = URIUtility.SplitIRIToStrings(
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
          com.upokecenter.util.DataUtilities.ToLowerCaseAscii(mediaType.substring(
  mediaType.length() - 7)).equals(";base64")) {
          mediaType = mediaType.substring(0, mediaType.length() - 7);
        }
        if (mediaType.length() == 0 || mediaType.charAt(0) == ';') {
          // Under RFC 2397, the type and subtype can be left
          // out. If left out, the media
          // type "text/plain" is assumed.
          mediaType = "text/plain" + mediaType;
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

    /**
     * Extracts the media type from a Data URI (uniform resource identifier) in the
     * form of a URI object.
     * @param uri A data URI in the form of a URI object.
     * @return The media type. Returns null if {@code uri} is null, is
     * syntactically invalid, or is not a Data URI.
     * @throws NullPointerException The parameter {@code uri} is null.
     */
    public static MediaType DataUriMediaType(java.net.URI uri) {
      return (uri == null) ? null : DataUriMediaType(uri.toString());
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

    static final int[] Alphabet = {
      -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
      52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
      41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1,
    };

    /**
     * Extracts the data from a Data URI (uniform resource identifier) in the form
     * of a byte array, where the Data URI is given as a URI object.
     * @param uri The Data URI in the form of a URI object.
     * @return The data as a byte array. Returns null if {@code uri} is null, is
     * syntactically invalid, or is not a data URI.
     * @throws NullPointerException The parameter {@code uri} is null.
     */
    public static byte[] DataUriBytes(java.net.URI uri) {
      return (uri == null) ? null : DataUriBytes(uri.toString());
    }

    /**
     * Extracts the data from a Data URI (uniform resource identifier) in the form
     * of a byte array.
     * @param uri The parameter {@code uri} is a text string.
     * @return The data as a byte array. Returns null if {@code uri} is null, is
     * syntactically invalid, or is not a data URI.
     * @throws NullPointerException The parameter {@code uri} is null.
     */
    public static byte[] DataUriBytes(String uri) {
      if (uri == null) {
        throw new NullPointerException("uri");
      }
      String url = uri;
      String[] parts = URIUtility.SplitIRIToStrings(
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
        boolean usesBase64 = mediaTypePart >= 7 && com.upokecenter.util.DataUtilities.ToLowerCaseAscii(
            path.substring(
              mediaTypePart - 7, (
              mediaTypePart - 7)+(7))).equals(";base64");
        // NOTE: Rejects base64 if nonbase64 characters
        // are present, since RFC 2397 doesn't state otherwise
        // (see RFC 4648). Base 64 also uses no line breaks
        // even if longer than 76 characters,
        // since RFC 2397 doesn't state otherwise
        // (see RFC 4648).
        ArrayWriter aw = new ArrayWriter();
        int i = 0;
        if (usesBase64) {
          int base64Length;
          int payloadIndex = mediaTypePart + 1;
          String payload = path;
          boolean hasPercent = false;
          for (i = mediaTypePart + 1; i < path.length(); i += 4) {
            if (path.charAt(i) == '%') {
              hasPercent = true;
              break;
            }
          }
          if (hasPercent) {
            payload = URIUtility.PercentDecode(
                path.substring(
                  mediaTypePart + 1, (
                  mediaTypePart + 1)+(path.length() - (mediaTypePart + 1))));
            payloadIndex = 0;
          }
          base64Length = payload.length() - payloadIndex;
          if ((base64Length % 4) != 0) {
            return null;
          }
          for (i = payloadIndex; i < payload.length(); i += 4) {
            boolean lastBlock = i + 4 >= payload.length();
            int b1 = 0, b2 = 0, b3 = 0, b4 = 0;
            b1 = (payload.charAt(i) > 0x7f) ? -1 : Alphabet[(int)payload.charAt(i)];
            b2 = (payload.charAt(i + 1) > 0x7f) ? -1 : Alphabet[(int)payload.charAt(i +
1)];
            if (lastBlock && payload.charAt(i + 2) == '=' && payload.charAt(i + 3) == '=') {
            } else if (lastBlock && path.charAt(i + 3) == '=') {
              b3 = (payload.charAt(i + 2) > 0x7f) ? -1 : Alphabet[(int)payload.charAt(i +
2)];
            } else {
              b3 = (payload.charAt(i + 2) > 0x7f) ? -1 : Alphabet[(int)payload.charAt(i +
2)];
              b4 = (payload.charAt(i + 3) > 0x7f) ? -1 : Alphabet[(int)payload.charAt(i +
3)];
            }
            if (b1 < 0 || b2 < 0 || b3 < 0 || b4 < 0) {
              return null;
            }
            int v = (b1 << 18) | (b2 << 12) | (b3 << 6) | b4;
            aw.write((byte)((v >> 16) & 0xff));
            if (payload.charAt(i + 2) != '=') {
              aw.write((byte)((v >> 8) & 0xff));
            }
            if (payload.charAt(i + 3) != '=') {
              aw.write((byte)(v & 0xff));
            }
          }
        } else {
          for (i = mediaTypePart + 1; i < path.length();) {
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

    private static final String Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdef" +
      "ghijklmnopqrstuvwxyz0123456789+/";

    private static void AppendBase64(StringBuilder builder, byte[] bytes) {
      int b1 = 0;
      int b2 = 0;
      int quantumCount = 0;
      for (int i = 0; i < bytes.length; ++i) {
        int value = bytes[i] & 0xff;
        switch (quantumCount) {
          case 2:
            builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
            builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4)
&
                  15)));
            builder.append((char)Base64Classic.charAt(((b2 & 15) << 2) + ((value
>>
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
        builder.append((char)Base64Classic.charAt((b2 & 15) << 2));
        builder.append('=');
      } else if (quantumCount == 1) {
        builder.append((char)Base64Classic.charAt((b1 >> 2) & 63));
        builder.append((char)Base64Classic.charAt(((b1 & 3) << 4) + ((b2 >> 4) &
              15)));
        builder.append("==");
      }
    }

    /**
     * Encodes text as a Data URI (uniform resource identifier).
     * @param textString A text string to encode as a data URI.
     * @return A Data URI that encodes the given text.
     * @throws NullPointerException The parameter {@code textString} is null.
     */
    public static String MakeDataUri(String textString) {
      if (textString == null) {
        throw new NullPointerException("textString");
      }
      return MakeDataUri(
          com.upokecenter.util.DataUtilities.GetUtf8Bytes(textString, true),
          MediaType.Parse("text/plain;charset=utf-8"));
    }

    /**
     * Encodes data with the given media type in a Data URI (uniform resource
     * identifier).
     * @param bytes A byte array containing the data to encode in a Data URI.
     * @param mediaType A media type to assign to the data.
     * @return A Data URI that encodes the given data and media type.
     * @throws NullPointerException The parameter {@code bytes} or {@code
     * mediaType} is null.
     */
    public static String MakeDataUri(byte[] bytes, MediaType mediaType) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      if (mediaType == null) {
        throw new NullPointerException("mediaType");
      }
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
