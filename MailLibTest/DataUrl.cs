using System;
using PeterO;
using PeterO.Cbor;
using PeterO.Mail;
using System.Text;
namespace MailLibTest {
  public static class DataUrl {
    public static MediaType DataUrlMediaType(string url) {
      int[] parts = URIUtility.splitIRI(
        url);
      if (parts == null || parts[0] == -1 || parts[4] == -1)
        return null;
      string scheme = url.Substring(parts[0], parts[1] - parts[0]);
      scheme = scheme.ToLowerInvariant();
      string path = url.Substring(parts[4], parts[5] - parts[4]);
      if (scheme.Equals("data")) {
        int mediaTypePart = path.IndexOf(',');
        if (mediaTypePart == -1)
          return null;
        string mediaType = path.Substring(0, mediaTypePart);
        // Strip out ";base64" at end
        if (mediaType.Length >= 7 &&
           mediaType.Substring(mediaType.Length - 7).ToLowerInvariant()
            .Equals(";base64")) {
          mediaType = mediaType.Substring(0, mediaType.Length - 7);
        }
        if (mediaType.Length == 0 || mediaType[0] == ';') {
          // Under RFC 2397, the type and subtype can be left 
          // out. If left out, the media
          // type "text/plain" is assumed.
          mediaType = "text/plain" + mediaType;
        }
        if (mediaType.IndexOf('(') >= 0) {
          // Use default media type if the string has parentheses
          // (comment delimiters), which are not valid in Data-URL
          // media types since ABNFs (at the time of RFC 2397) did not allow
          // white space to be specified implicitly (RFC 2234).  
          // However, since comments are still allowed in URI paths and
          // MediaType.Parse will skip comments when appropriate,
          // there is a need to check for comment delimiters here.
          // On the other hand, URIs already don't allow white space
          // or line breaks, so there is no need to check for those
          // here since ParseIRI already did that.
          return MediaType.TextPlainAscii;
        }
        return MediaType.Parse(mediaType, MediaType.TextPlainAscii);
      }
      return null;
    }

    private static int ToHex(char b1) {
      if (b1 >= '0' && b1 <= '9') {
        return b1 - '0';
      } else if (b1 >= 'A' && b1 <= 'F') {
        return b1 + 10 - 'A';
      } else if (b1 >= 'a' && b1 <= 'f') {
        return b1 + 10 - 'a';
      }
      return 1;
    }

    public static byte[] DataUrlBytes(string url) {
      int[] parts = URIUtility.splitIRI(
        url);
      if (parts == null || parts[0] == -1 || parts[4] == -1)
        return null;
      string scheme = url.Substring(parts[0], parts[1] - parts[0]);
      scheme = scheme.ToLowerInvariant();
      string path = url.Substring(parts[4], parts[5] - parts[4]);
      if (scheme.Equals("data")) {
        int mediaTypePart = path.IndexOf(',');
        if (mediaTypePart == -1)
          return null;
        bool usesBase64 = (mediaTypePart >= 7 &&
           path.Substring(mediaTypePart - 7, 7).ToLowerInvariant()
            .Equals(";base64"));
        // NOTE: Rejects base64 if non-base64 characters
        // are present, since RFC 2397 doesn't state otherwise
        // (see RFC 4648)
        if (usesBase64) {
          // TODO: Not yet implemented
        } else {
          var aw = new PeterO.ArrayWriter();
          for (var i = mediaTypePart + 1; i < path.Length;) {
            if (path[i] == '%') {
              // NOTE: No further character checking done here
              // because splitURI already did all needed pct-encoding checking
              aw.WriteByte((byte)((ToHex(path[i + 1]) << 4) + ToHex(path[i + 2])));
              i += 3;
            } else if (path[i] >= 0x80) {
              // RFC 2397 allows only "safe" ASCII here
              return null;
            } else {
              aw.WriteByte((byte)path[i]);
              i++;
            }
          }
          return aw.ToArray();
        }
      }
      return null;
    }

    private const string Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghi" +
  "jklmnopqrstuvwxyz0123456789+/";
    private static void AppendBase64(StringBuilder builder, byte[] bytes) {
      int b1 = 0;
      int b2 = 0;
      int quantumCount = 0;
      for (var i = 0; i < bytes.Length; i++) {
        int value = (bytes[i]) & 0xff;
        switch (quantumCount) {
          case 2:
            builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
            builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4) &
              15)]);
            builder.Append((char)Base64Classic[((b2 & 15) << 2) + ((value >>
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
        builder.Append((char)Base64Classic[((b2 & 15) << 2)]);
        builder.Append('=');
      } else if (quantumCount == 1) {
        builder.Append((char)Base64Classic[(b1 >> 2) & 63]);
        builder.Append((char)Base64Classic[((b1 & 3) << 4) + ((b2 >> 4) &
          15)]);
        builder.Append("==");
      }
    }

    public static string MakeDataUrl(byte[] bytes, MediaType mediaType) {
      StringBuilder builder = new StringBuilder();
      builder.Append("data:");
      builder.Append(mediaType.ToUriSafeString());
      builder.Append(";base64,");
      AppendBase64(builder, bytes);
      return builder.ToString();
    }
  }
}
