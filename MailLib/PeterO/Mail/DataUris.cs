using System;
using System.Text;
using PeterO;

namespace PeterO.Mail {
  /// <summary>
  ///  Contains methods for parsing and generating Data URIs
  /// (uniform resource identifiers). Data URIs are described
  /// in RFC 2397. Examples for Data URIs follow.
  /// <code>data:, hello%20world</code>
  /// <code>data:text/markdown, hello%20world</code>
  /// <code>data:application/octet-stream;base64, AAAAAA==</code>
  ///  .
  /// </summary>
  public static class DataUris {
    /// <summary>Extracts the media type from a Data URI (uniform resource
    /// identifier) in the form of a text string.</summary>
    /// <param name='uri'>A data URI in the form of a text string.</param>
    /// <returns>The media type. Returns null if <paramref name='uri'/> is
    /// null, is syntactically invalid, or is not a Data URI.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='uri'/> is null.</exception>
    public static MediaType DataUriMediaType(string uri) {
      if (uri == null) {
        throw new ArgumentNullException(nameof(uri));
      }
      string url = uri;
      string[] parts = URIUtility.SplitIRIToStrings(
          url);
      if (parts == null || parts[0] == null || parts[2] == null) {
        return null;
      }
      string path = parts[2];
      if (parts[0].Equals("data", StringComparison.Ordinal)) {
        int mediaTypePart = path.IndexOf(',');
        if (mediaTypePart == -1) {
          return null;
        }
        string mediaType = path.Substring(0, mediaTypePart);
        // Strip out ";base64" at end
        if (mediaType.Length >= 7 &&
          DataUtilities.ToLowerCaseAscii(mediaType.Substring(
  mediaType.Length - 7)).Equals(";base64", StringComparison.Ordinal)) {
          mediaType = mediaType.Substring(0, mediaType.Length - 7);
        }
        if (mediaType.Length == 0 || mediaType[0] == ';') {
          // Under RFC 2397, the type and subtype can be left
          // out. If left out, the media
          // type "text/plain" is assumed.
          mediaType = "text/plain" + mediaType;
        }
        if (mediaType.IndexOf('(') >= 0) {
          // The media type string has parentheses
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

    /// <summary>Extracts the media type from a Data URI (uniform resource
    /// identifier) in the form of a URI object.</summary>
    /// <param name='uri'>A data URI in the form of a URI object.</param>
    /// <returns>The media type. Returns null if <paramref name='uri'/> is
    /// null, is syntactically invalid, or is not a Data URI.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='uri'/> is null.</exception>
    public static MediaType DataUriMediaType(Uri uri) {
      return (uri == null) ? null : DataUriMediaType(uri.ToString());
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

    internal static readonly int[] Alphabet = {
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

    /// <summary>Extracts the data from a Data URI (uniform resource
    /// identifier) in the form of a byte array, where the Data URI is
    /// given as a URI object.</summary>
    /// <param name='uri'>The Data URI in the form of a URI object.</param>
    /// <returns>The data as a byte array. Returns null if <paramref
    /// name='uri'/> is null, is syntactically invalid, or is not a data
    /// URI.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='uri'/> is null.</exception>
    public static byte[] DataUriBytes(Uri uri) {
      return (uri == null) ? null : DataUriBytes(uri.ToString());
    }

    /// <summary>Extracts the data from a Data URI (uniform resource
    /// identifier) in the form of a byte array.</summary>
    /// <param name='uri'>The parameter <paramref name='uri'/> is a text
    /// string.</param>
    /// <returns>The data as a byte array. Returns null if <paramref
    /// name='uri'/> is null, is syntactically invalid, or is not a data
    /// URI.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='uri'/> is null.</exception>
    public static byte[] DataUriBytes(string uri) {
      if (uri == null) {
        throw new ArgumentNullException(nameof(uri));
      }
      string url = uri;
      string[] parts = URIUtility.SplitIRIToStrings(
          url);
      if (parts == null || parts[0] == null || parts[2] == null) {
        return null;
      }
      string path = parts[2];
      if (parts[0].Equals("data", StringComparison.Ordinal)) {
        int mediaTypePart = path.IndexOf(',');
        if (mediaTypePart == -1) {
          return null;
        }
        bool usesBase64 = mediaTypePart >= 7 && DataUtilities.ToLowerCaseAscii(
            path.Substring(
              mediaTypePart - 7,
              7)).Equals(";base64", StringComparison.Ordinal);
        // NOTE: Rejects base64 if nonbase64 characters
        // are present, since RFC 2397 doesn't state otherwise
        // (see RFC 4648). Base 64 also uses no line breaks
        // even if longer than 76 characters,
        // since RFC 2397 doesn't state otherwise
        // (see RFC 4648).
        var aw = new ArrayWriter();
        var i = 0;
        if (usesBase64) {
          int base64Length;
          int payloadIndex = mediaTypePart + 1;
          string payload = path;
          var hasPercent = false;
          for (i = mediaTypePart + 1; i < path.Length; i += 4) {
            if (path[i] == '%') {
              hasPercent = true;
              break;
            }
          }
          if (hasPercent) {
            payload = URIUtility.PercentDecode(
                path.Substring(
                  mediaTypePart + 1,
                  path.Length - (mediaTypePart + 1)));
            payloadIndex = 0;
          }
          base64Length = payload.Length - payloadIndex;
          if ((base64Length % 4) != 0) {
            return null;
          }
          for (i = payloadIndex; i < payload.Length; i += 4) {
            bool lastBlock = i + 4 >= payload.Length;
            int b1 = 0, b2 = 0, b3 = 0, b4 = 0;
            b1 = (payload[i] > 0x7f) ? -1 : Alphabet[(int)payload[i]];
            b2 = (payload[i + 1] > 0x7f) ? -1 : Alphabet[(int)payload[i +
1]];
            if (lastBlock && payload[i + 2] == '=' && payload[i + 3] == '=') {
            } else if (lastBlock && path[i + 3] == '=') {
              b3 = (payload[i + 2] > 0x7f) ? -1 : Alphabet[(int)payload[i +
2]];
            } else {
              b3 = (payload[i + 2] > 0x7f) ? -1 : Alphabet[(int)payload[i +
2]];
              b4 = (payload[i + 3] > 0x7f) ? -1 : Alphabet[(int)payload[i +
3]];
            }
            if (b1 < 0 || b2 < 0 || b3 < 0 || b4 < 0) {
              return null;
            }
            int v = (b1 << 18) | (b2 << 12) | (b3 << 6) | b4;
            aw.WriteByte((byte)((v >> 16) & 0xff));
            if (payload[i + 2] != '=') {
              aw.WriteByte((byte)((v >> 8) & 0xff));
            }
            if (payload[i + 3] != '=') {
              aw.WriteByte((byte)(v & 0xff));
            }
          }
        } else {
          for (i = mediaTypePart + 1; i < path.Length;) {
            if (path[i] == '%') {
              // NOTE: No further character checking done here
              // because splitURI already did all needed pct-encoding checking
              aw.WriteByte((byte)((ToHex(path[i + 1]) << 4) + ToHex(path[i +
                      2])));
              i += 3;
            } else if (path[i] >= 0x80) {
              // RFC 2397 allows only "safe" ASCII here
              return null;
            } else {
              aw.WriteByte((byte)path[i]);
              ++i;
            }
          }
        }
        return aw.ToArray();
      }
      return null;
    }

    private const string Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdef" +
      "ghijklmnopqrstuvwxyz0123456789+/";

    private static void AppendBase64(StringBuilder builder, byte[] bytes) {
      var b1 = 0;
      var b2 = 0;
      var quantumCount = 0;
      for (var i = 0; i < bytes.Length; ++i) {
        int value = bytes[i] & 0xff;
        switch (quantumCount) {
          case 2:
            builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
            builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4)
&
                  15)]);
            builder.Append((char)Base64Classic[((b2 & 15) << 2) + ((value
>>
                    6) & 3)]);
            builder.Append((char)Base64Classic[value & 63]);
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
        builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
        builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4) &
              15)]);
        builder.Append((char)Base64Classic[(b2 & 15) << 2]);
        builder.Append('=');
      } else if (quantumCount == 1) {
        builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
        builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4) &
              15)]);
        builder.Append("==");
      }
    }

    /// <summary>Encodes text as a Data URI (uniform resource
    /// identifier).</summary>
    /// <param name='textString'>A text string to encode as a data
    /// URI.</param>
    /// <returns>A Data URI that encodes the given text.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='textString'/> is null.</exception>
    public static string MakeDataUri(string textString) {
      if (textString == null) {
        throw new ArgumentNullException(nameof(textString));
      }
      return MakeDataUri(
          DataUtilities.GetUtf8Bytes(textString, true),
          MediaType.Parse("text/plain;charset=utf-8"));
    }

    /// <summary>Encodes data with the given media type in a Data URI
    /// (uniform resource identifier).</summary>
    /// <param name='bytes'>A byte array containing the data to encode in a
    /// Data URI.</param>
    /// <param name='mediaType'>A media type to assign to the data.</param>
    /// <returns>A Data URI that encodes the given data and media
    /// type.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> or <paramref name='mediaType'/> is null.</exception>
    public static string MakeDataUri(byte[] bytes, MediaType mediaType) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (mediaType == null) {
        throw new ArgumentNullException(nameof(mediaType));
      }
      var builder = new StringBuilder();
      builder.Append("data:");
      string mediaTypeString = mediaType.ToUriSafeString();
      if (mediaType.TypeAndSubType.Equals("text/plain",
          StringComparison.Ordinal)) {
        if (mediaTypeString.Substring(0, 10).Equals("text/plain",
            StringComparison.Ordinal)) {
          // Strip 'text/plain' from the media type string,
          // since that's the default for data URIs
          mediaTypeString = mediaTypeString.Substring(10);
        }
      }
      builder.Append(mediaTypeString);
      builder.Append(";base64,");
      AppendBase64(builder, bytes);
      return builder.ToString();
    }
  }
}
