using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  public class DataUrl {
    private static bool MatchLangTagExtended(string rangeLowerCased,
  string tagLowerCased) {
      string[] rangeSub = rangeLowerCased.Split('-');
      string[] tagSub = tagLowerCased.Split('-');
      if (rangeSub.Length == 0 || tagSub.Length == 0) return false;
      if (!rangeSub[0].Equals("*") && !rangeSub[0].Equals(tagSub[0]))
        return false;
      int rangeIndex = 1;
      int tagIndex = 1;
      while (rangeIndex < rangeSub.Length) {
        if (rangeSub[rangeIndex].Length == 0) return false;
        if (rangeSub[rangeIndex].Equals("*")) {
          continue;
        }
        if (tagIndex >= tagSub.Length) return false;
        if (rangeSub[rangeIndex].Equals(tagSub[tagIndex])) {
          rangeIndex++;
          tagIndex++;
        }
        if (tagSub[tagIndex].Length == 1) return false;
        tagIndex++;
      }
      return true;
    }

    public static int MatchLangTagsExtended(IList<string> ranges, string langToMatch) {
      // ArgumentAssert.NotNull(ranges);
      if (String.IsNullOrEmpty(langToMatch)) return -1;
      string lclang = DataUtilities.ToLowerCaseAscii(langToMatch);
      for (var i = 0; i < ranges.Count; i++) {
        var r = ranges[i];
        r = DataUtilities.ToLowerCaseAscii(r);
        if (MatchLangTagExtended(r, lclang)) return i;
      }
      return -1;
    }


    public static int MatchLangTagsBasic(IList<string> ranges, string langToMatch, bool matchStarLast) {
      int starMatch = -1;
      // ArgumentAssert.NotNull(ranges);
      if (String.IsNullOrEmpty(langToMatch)) return -1;
      string lclang = DataUtilities.ToLowerCaseAscii(langToMatch);
      for (var i = 0; i < ranges.Count; i++) {
        var r = ranges[i];
        if (String.IsNullOrEmpty(r)) throw new ArgumentException("ranges");
        if (r.Equals("*")) {
          if (!matchStarLast) return i;
          else if (starMatch < 0) starMatch = i;
        } else if (langToMatch.Length >= r.Length) {
          r = DataUtilities.ToLowerCaseAscii(r);
          if (r.Equals(lclang)) return i;
          if (lclang.Length > r.Length && langToMatch[r.Length] == '-') {
            string prefix = lclang.Substring(0, r.Length);
            if (r.Equals(prefix)) return i;
          }
        }
      }
      return starMatch;
    }

    public static int MatchLangTagsBasic(IList<string> ranges, string langToMatch) {
      return MatchLangTagsBasic(ranges, langToMatch, false);
    }
    // TODO: Convert Messages to mailto URIs
    public static MediaType DataUrlMediaType(string url) {
      string[] parts = URIUtility.splitIRIToStrings(
        url);
      if (parts == null || parts[0] == null || parts[2] == null) {
        return null;
      }
      string path = parts[2];
      if (parts[0].Equals("data")) {
        int mediaTypePart = path.IndexOf(',');
        if (mediaTypePart == -1) {
          return null;
        }
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
        if (mediaType.IndexOf('/') < 0) {
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


    private static int ToHex(char b1) {
      if (b1 >= '0' && b1 <= '9') {
        return b1 - '0';
      } else if (b1 >= 'A' && b1 <= 'F') {
        return b1 + 10 - 'A';
      } else {
        return (b1 >= 'a' && b1 <= 'f') ? (b1 + 10 - 'a') : 1;
      }
    }

    public static Message MailToUrlMessage(string url) {
      string[] parts = URIUtility.splitIRIToStrings(
        url);
      if (parts == null || parts[0] == null) {
        return null;
      }

      if (parts[0].Equals("mailto")) {
        var msg = new Message();
        string emails = String.Empty;
        if (!String.IsNullOrEmpty(parts[2])) {
          // Extract the email address
          emails = URIUtility.PercentDecode(parts[2]);
          if (HeaderParser.ParseHeaderEmail(emails, 0, emails.Length) !=
                      emails.Length) {
            Console.WriteLine(emails);
            return null;
          }
        }
        if (parts[3] != null) {
          // If query string is present it must not be empty
          string query = parts[3];
          if (query.Length == 0) {
            return null;
          }
          var index = 0;
          while (index < query.Length) {
            int startName = index;
            var endName = -1;
            var startValue = -1;
            var endValue = -1;
            while (index < query.Length) {
              if (query[index] == '&') {
                if (endName < 0) {
                  return null;
                }
                endValue = index;
                ++index;
                break;
              } else if (query[index] == '=') {
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
              endValue = query.Length;
            }
            string name = query.Substring(startName, endName - startName);
            string value = query.Substring(startValue, endValue - startValue);
            name =
     DataUtilities.ToLowerCaseAscii(URIUtility.PercentDecode(name));
            value = URIUtility.PercentDecode(value);
            // Support only To, Cc, Bcc, Subject, In-Reply-To,
            // Keywords, Comments, and Body.
            // Of these, the first four can appear only once in a message
            if (name.Equals("body")) {
              msg.SetTextBody(value);
            } else if (name.Equals("keywords") || name.Equals("comments")) {
              msg.AddHeader(name, value);
            } else if (name.Equals("subject") ||
              name.Equals("to") || name.Equals("cc") || name.Equals("bcc") ||
              name.Equals("in-reply-to")) {
              // TODO: Decode encoded words
              msg.SetHeader(name, value);
            } else if (name.Equals("to")) {
              if (String.IsNullOrEmpty(emails)) {
                emails = value;
              } else {
                emails += "," + value;
              }
            } else {
              Console.WriteLine(name);
              Console.WriteLine(value);
            }
          }
        }
        if (!String.IsNullOrEmpty(emails)) {
          if (emails.IndexOf('(') >= 0) {
            // Contains opening parenthesis, a comment delimiter
            return null;
          }
          try {
            msg.SetHeader("to", emails);
          } catch (Exception) {
            Console.WriteLine(emails);
            return null;
          }
        }
        Console.WriteLine(msg.Generate());
        return msg;
      }
      return null;
    }

    internal static readonly int[] Alphabet = { -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1,
      -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
      52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
      -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
      15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
      -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
      41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

    public static byte[] DataUrlBytes(string url) {
      string[] parts = URIUtility.splitIRIToStrings(
        url);
      if (parts == null || parts[0] == null || parts[2] == null) {
        return null;
      }
      string path = parts[2];
      if (parts[0].Equals("data")) {
        int mediaTypePart = path.IndexOf(',');
        if (mediaTypePart == -1) {
          return null;
        }
        bool usesBase64 = mediaTypePart >= 7 && path.Substring(
     mediaTypePart - 7,
     7).ToLowerInvariant().Equals(";base64");
        // NOTE: Rejects base64 if non-base64 characters
        // are present, since RFC 2397 doesn't state otherwise
        // (see RFC 4648). Base 64 also uses no line breaks
        // even if longer than 76 characters,
        // since RFC 2397 doesn't state otherwise
        // (see RFC 4648).
        var aw = new PeterO.ArrayWriter();
        if (usesBase64) {
          int base64Length = path.Length - (mediaTypePart + 1);
          if ((base64Length % 4) != 0) {
            return null;
          }
          for (var i = mediaTypePart + 1; i < path.Length; i += 4) {
            bool lastBlock = i + 4 >= path.Length;
            int b1 = 0, b2 = 0, b3 = 0, b4 = 0;
            b1 = (path[i] > 0x7f) ? -1 : Alphabet[(int)path[i]];
            b2 = (path[i + 1] > 0x7f) ? -1 : Alphabet[(int)path[i + 1]];
            if (lastBlock && path[i + 2] == '=' && path[i + 3] == '=') {
            } else if (lastBlock && path[i + 3] == '=') {
              b3 = (path[i + 2] > 0x7f) ? -1 : Alphabet[(int)path[i + 2]];
            } else {
              b3 = (path[i + 2] > 0x7f) ? -1 : Alphabet[(int)path[i + 2]];
              b4 = (path[i + 3] > 0x7f) ? -1 : Alphabet[(int)path[i + 3]];
            }
            if (b1 < 0 || b2 < 0 || b3 < 0 || b4 < 0) {
              return null;
            }
            int v = (b1 << 18) | (b2 << 12) | (b3 << 6) | b4;
            aw.WriteByte((byte)((v >> 16) & 0xff));
            if (path[i + 2] != '=') {
              aw.WriteByte((byte)((v >> 8) & 0xff));
            }
            if (path[i + 3] != '=') {
              aw.WriteByte((byte)(v & 0xff));
            }
          }
        } else {
          for (var i = mediaTypePart + 1; i < path.Length;) {
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

    private const string Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghi" +
  "jklmnopqrstuvwxyz0123456789+/";

    private static void AppendBase64(StringBuilder builder, byte[] bytes) {
      var b1 = 0;
      var b2 = 0;
      var quantumCount = 0;
      for (var i = 0; i < bytes.Length; ++i) {
        int value = bytes[i] & 0xff;
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

    public static string MakeDataUrl(string textString) {
      return MakeDataUrl(DataUtilities.GetUtf8Bytes(textString, true),
                         MediaType.Parse("text/plain;charset=utf-8"));
    }

    public static string MakeDataUrl(byte[] bytes, MediaType mediaType) {
      // ArgumentAssert.NotNull(bytes);
      // ArgumentAssert.NotNull(mediaType);
      var builder = new StringBuilder();
      builder.Append("data:");
      string mediaTypeString = mediaType.ToUriSafeString();
      if (mediaType.TypeAndSubType.Equals("text/plain")) {
        if (mediaTypeString.Substring(0, 10).Equals("text/plain")) {
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
