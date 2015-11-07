/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;
using System.Text;

using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
  internal static class HeaderFieldParsers {
    private sealed class UnstructuredHeaderField : IHeaderFieldParser {
      public string DowngradeFieldValue(string str) {
        return Rfc2047.EncodeString(str);
      }

      public string DecodeEncodedWords(string str) {
        // For unstructured header fields.
        return Rfc2047.DecodeEncodedWords(
str,
0,
str.Length,
EncodedWordContext.Unstructured);
      }

      public bool IsStructured() {
        return false;
      }

      public int Parse(string str, int index, int endIndex, ITokener tokener) {
        return endIndex;
      }
    }

    private abstract class StructuredHeaderField : IHeaderFieldParser {
      public abstract int Parse(
string str,
int index,
int endIndex,
ITokener tokener);

   private IList<string> ParseGroupLists(
string str,
int index,
int endIndex) {
        var groups = new List<string>();
        var tokener = new Tokener();
        this.Parse(str, index, endIndex, tokener);
        foreach (int[] token in tokener.GetTokens()) {
          if (token[0] == HeaderParserUtility.TokenGroup) {
            int startIndex = token[1];
            endIndex = token[2];
            string groupList = HeaderParserUtility.ParseGroupList(
str,
startIndex,
endIndex);
            groupList = ParserUtility.TrimSpaceAndTab(groupList);
            groups.Add(groupList);
          }
        }
        return groups;
      }

      public string DowngradeFieldValue(string str) {
        string originalString = str;
        IList<string> originalGroups = null;
        for (int phase = 0; phase < 5; ++phase) {
          if (str.IndexOf('(') < 0 && phase == 0) {
            // No comments in the header field value, a common case
            continue;
          }
          if (!Message.HasTextToEscape(str)) {
            // No text needs to be encoded
            return str;
          }
          var sb = new StringBuilder();
          var tokener = new Tokener();
          int endIndex = this.Parse(str, 0, str.Length, tokener);
          if (endIndex != str.Length) {
            // The header field is syntactically invalid,
            // so downgrading is not possible
            return str;
          }
          int lastIndex = 0;
          // Get each relevant token sorted by starting index
          IList<int[]> tokens = tokener.GetTokens();
          int groupIndex = 0;
          // TODO: Received field downgrading
          foreach (int[] token in tokens) {
            if (token[1] < lastIndex) {
              continue;
            }
            // NOTE: Doesn't downgrade ID-Left or ID-Right
            // if extended characters appear in those areas
            if (phase == 0) {  // Comment downgrading
              if (token[0] == HeaderParserUtility.TokenComment) {
                int startIndex = token[1];
                endIndex = token[2];
                // Console.WriteLine(str.Substring(startIndex, endIndex -
                // startIndex));
                if (Message.HasTextToEscape(str, startIndex, endIndex)) {
          string newComment = Rfc2047.EncodeComment(
str,
startIndex,
endIndex);
                  sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                  sb.Append(newComment);
                } else {
                  // No text needs to be escaped, output the comment as is
                  sb.Append(str.Substring(lastIndex, endIndex - lastIndex));
                }
                lastIndex = endIndex;
              }
            } else if (phase == 1) {  // Phrase downgrading
              if (token[0] == HeaderParserUtility.TokenPhrase) {
                int startIndex = token[1];
                endIndex = token[2];
                string newComment = Rfc2047.EncodePhraseText(
str,
startIndex,
endIndex,
tokens);
                sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                sb.Append(newComment);
                lastIndex = endIndex;
              }
            } else if (phase == 2) {  // Group downgrading
              if (token[0] == HeaderParserUtility.TokenGroup) {
                int startIndex = token[1];
                endIndex = token[2];
                bool nonasciiLocalParts = false;
                int displayNameEnd = -1;
                string originalGroupList;
                foreach (int[] token2 in tokens) {
                  if (token2[0] == HeaderParserUtility.TokenPhrase) {
                    if (displayNameEnd < 0) {
                    displayNameEnd = token2[2];
                    }
                  }
                  if (token2[0] == HeaderParserUtility.TokenLocalPart) {
                    if (token2[1] >= startIndex && token2[2] <= endIndex) {
                    // Local part within a group
    if (
Message.HasTextToEscapeIgnoreEncodedWords(
str,
token2[1],
token2[2])) {
                    nonasciiLocalParts = true;
                    break;
                    }
                    }
                  }
                }
                if (!nonasciiLocalParts) {
                  int localLastIndex = startIndex;
                  bool nonasciiDomains = false;
                  var sb2 = new StringBuilder();
                  foreach (int[] token2 in tokens) {
                    if (token2[0] == HeaderParserUtility.TokenDomain) {
                    if (token2[1] >= startIndex && token2[2] <= endIndex) {
                    // Domain within the group
     string domain = HeaderParserUtility.ParseDomain(
str,
token2[1],
token[2]);
                    // NOTE: "domain" can include domain literals, enclosed
                    // in brackets; they are invalid under
                    // "IsValidDomainName" .
                domain = (
Message.HasTextToEscapeIgnoreEncodedWords(
domain,
0,
domain.Length) && Idna.IsValidDomainName(
domain,
false)) ? Idna.EncodeDomainName(
domain) : str.Substring(
token2[1],
token2[2] - token2[1]);
     if (
Message.HasTextToEscapeIgnoreEncodedWords(
domain,
0,
domain.Length)) {
                    // ASCII encoding failed
                    nonasciiDomains = true;
                    break;
                    }
         sb2.Append(
str.Substring(
localLastIndex,
token2[1] - localLastIndex));
                    sb2.Append(domain);
                    localLastIndex = token2[2];
                    }
                    }
                  }
                  nonasciiLocalParts = nonasciiDomains;
                  if (!nonasciiLocalParts) {
                    // All of the domains could be converted to ASCII
          sb2.Append(
str.Substring(
localLastIndex,
endIndex - localLastIndex));
                    sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                    sb.Append(sb2.ToString());
                    lastIndex = endIndex;
                  }
                }
                if (nonasciiLocalParts) {
                  // At least some of the domains could not
                  // be converted to ASCII
                   originalGroups = originalGroups ??
                this.ParseGroupLists(originalString, 0, originalString.Length);
                  originalGroupList = originalGroups[groupIndex];
                  string groupText = originalGroupList;
                  string displayNameText = str.Substring(
startIndex,
displayNameEnd - startIndex);
                  string encodedText = displayNameText + " " +
                    Rfc2047.EncodeString(groupText) + " :;";
                  sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                  sb.Append(encodedText);
                  lastIndex = endIndex;
                }
                ++groupIndex;
              }
            } else if (phase == 3) {  // Mailbox downgrading
              if (token[0] == HeaderParserUtility.TokenMailbox) {
                int startIndex = token[1];
                endIndex = token[2];
                bool nonasciiLocalPart = false;
                bool hasPhrase = false;
                foreach (int[] token2 in tokens) {
                  hasPhrase |= token2[0] == HeaderParserUtility.TokenPhrase;
                  if (token2[0] == HeaderParserUtility.TokenLocalPart) {
                    if (token2[1] >= startIndex && token2[2] <= endIndex) {
    if (
Message.HasTextToEscapeIgnoreEncodedWords(
str,
token2[1],
token2[2])) {
                    nonasciiLocalPart = true;
                    break;
                    }
                    }
                  }
                }
                if (!nonasciiLocalPart) {
                  int localLastIndex = startIndex;
                  bool nonasciiDomains = false;
                  var sb2 = new StringBuilder();
                  foreach (int[] token2 in tokens) {
                    if (token2[0] == HeaderParserUtility.TokenDomain) {
                    if (token2[1] >= startIndex && token2[2] <= endIndex) {
                    // Domain within the group
     string domain = HeaderParserUtility.ParseDomain(
str,
token2[1],
token[2]);
                    // NOTE: "domain" can include domain literals, enclosed
                    // in brackets; they are invalid under
                    // "IsValidDomainName" .
                domain = (
Message.HasTextToEscapeIgnoreEncodedWords(
domain,
0,
domain.Length) && Idna.IsValidDomainName(
domain,
false)) ? Idna.EncodeDomainName(
domain) : str.Substring(
token2[1],
token2[2] - token2[1]);
     if (
Message.HasTextToEscapeIgnoreEncodedWords(
domain,
0,
domain.Length)) {
                    // ASCII encoding failed
                    nonasciiDomains = true;
                    break;
                    }
         sb2.Append(
str.Substring(
localLastIndex,
token2[1] - localLastIndex));
                    sb2.Append(domain);
                    localLastIndex = token2[2];
                    }
                    }
                  }
                  nonasciiLocalPart = nonasciiDomains;
                  if (!nonasciiLocalPart) {
                    // All of the domains could be converted to ASCII
          sb2.Append(
str.Substring(
localLastIndex,
endIndex - localLastIndex));
                    sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                    sb.Append(sb2.ToString());
                    lastIndex = endIndex;
                  }
                }
                // Downgrading failed
                if (nonasciiLocalPart) {
                  sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                  if (!hasPhrase) {
                string addrSpec = str.Substring(
token[1],
token[2] - token[1]);
             string encodedText = " " + Rfc2047.EncodeString(addrSpec) +
                    " :;";
                    sb.Append(encodedText);
                  } else {
                    // Has a phrase, extract the addr-spec and convert
                    // the mailbox to a group
  int angleAddrStart = HeaderParser.ParsePhrase(
str,
token[1],
token[2],
null);
                    // append the rest of the string so far up to and
                    // including the phrase
               sb.Append(
str.Substring(
lastIndex,
angleAddrStart - lastIndex));
                    int addrSpecStart = HeaderParser.ParseCFWS(
str,
angleAddrStart,
token[2],
null);
                    if (addrSpecStart < token[2] && str[addrSpecStart] == '<') {
                    ++addrSpecStart;
                    }
addrSpecStart = HeaderParser.ParseObsRoute(
str,
addrSpecStart,
token[2],
null);
                    int addrSpecEnd = HeaderParser.ParseAddrSpec(
str,
addrSpecStart,
token[2],
null);
   string addrSpec = str.Substring(
addrSpecStart,
addrSpecEnd - addrSpecStart);
                    bool endsWithSpace = sb.Length > 0 && (sb[sb.Length - 1]
                    == 0x20 || sb[sb.Length - 1] == 0x09);
                    string encodedText = (endsWithSpace ? String.Empty : " ") +
                    Rfc2047.EncodeString(addrSpec) + " :;";
                    sb.Append(encodedText);
                  }
                  lastIndex = endIndex;
                }
                ++groupIndex;
              }
            }
          }
          sb.Append(str.Substring(lastIndex, str.Length - lastIndex));
          str = sb.ToString();
        }
        return str;
      }

      public string DecodeEncodedWords(string str) {
        #if DEBUG
if (str == null) {
          throw new ArgumentNullException("str");
        }
        #endif

        // For structured header fields that allow comments only wherever
        // whitespace
        // is allowed, and allow parentheses only for comments
        if (str.Length < 9) {
          // too short for encoded words
          return str;
        }
        if (str.IndexOf("=?", StringComparison.Ordinal) < 0) {
          // No encoded words
          return str;
        }
        var sb = new StringBuilder();
        var tokener = new Tokener();
        int endIndex = this.Parse(str, 0, str.Length, tokener);
        if (endIndex != str.Length) {
          // The header field is syntactically invalid,
          // so don't decode any encoded words
          // Console.WriteLine("Invalid syntax: " + this.GetType().Name +
          // ", " + str);
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        foreach (int[] token in tokens) {
          // Console.WriteLine("" + token[0] + " [" +
          // (str.Substring(token[1],token[2]-token[1])) + "]");
   if (token[0] == HeaderParserUtility.TokenComment && token[0] >=
            lastIndex) {
            // This is a comment token
            int startIndex = token[1];
            endIndex = token[2];
            string newComment = Rfc2047.DecodeEncodedWords(
              str,
              startIndex + 1,
              endIndex - 1,
              EncodedWordContext.Comment);
            sb.Append(str.Substring(lastIndex, startIndex + 1 - lastIndex));
            sb.Append(newComment);
            lastIndex = endIndex - 1;
          } else if (token[0] == HeaderParserUtility.TokenPhrase) {
            // This is a phrase token
            int startIndex = token[1];
            endIndex = token[2];
            string newComment = Rfc2047.DecodePhraseText(
              str,
              startIndex,
              endIndex,
              tokens,
              true);
            sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
            sb.Append(newComment);
            lastIndex = endIndex;
          }
        }
        sb.Append(str.Substring(lastIndex, str.Length - lastIndex));
        return sb.ToString();
      }

      public bool IsStructured() {
        return true;
      }
    }

    private sealed class HeaderX400ContentReturn : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderX400ContentReturn(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderDeliveryDate : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
    return HeaderParser.ParseHeaderDeliveryDate(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderPriority : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderPriority(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderImportance : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderImportance(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderSensitivity : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
     return HeaderParser.ParseHeaderSensitivity(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderX400ContentIdentifier : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderX400ContentIdentifier(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderX400Received : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
    return HeaderParser.ParseHeaderX400Received(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderX400MtsIdentifier : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderX400MtsIdentifier(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderX400Originator : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
  return HeaderParser.ParseHeaderX400Originator(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderX400Recipients : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
  return HeaderParser.ParseHeaderX400Recipients(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderConversion : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderConversion(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderConversionWithLoss : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderConversionWithLoss(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderSupersedes : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderSupersedes(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderAutoforwarded : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
   return HeaderParser.ParseHeaderAutoforwarded(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderGenerateDeliveryReport : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderGenerateDeliveryReport(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderIncompleteCopy : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
  return HeaderParser.ParseHeaderIncompleteCopy(
str,
index,
endIndex,
tokener);
      }
    }

  private sealed class HeaderPreventNondeliveryReport :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderPreventNondeliveryReport(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderAlternateRecipient : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderAlternateRecipient(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderDiscloseRecipients : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderDiscloseRecipients(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderNewsgroups : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderNewsgroups(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderPath : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderPath(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderArchive : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderArchive(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderControl : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderControl(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDistribution : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
    return HeaderParser.ParseHeaderDistribution(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderFollowupTo : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderFollowupTo(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderInjectionDate : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
   return HeaderParser.ParseHeaderInjectionDate(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderInjectionInfo : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
   return HeaderParser.ParseHeaderInjectionInfo(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderUserAgent : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderUserAgent(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXref : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderXref(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderAcceptLanguage : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
  return HeaderParser.ParseHeaderAcceptLanguage(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderArchivedAt : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderArchivedAt(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderAuthenticationResults : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderAuthenticationResults(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderAutoSubmitted : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
   return HeaderParser.ParseHeaderAutoSubmitted(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderBcc : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentBase : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
     return HeaderParser.ParseHeaderContentBase(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderContentDisposition : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderContentDisposition(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderContentDuration : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
 return HeaderParser.ParseHeaderContentDuration(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderContentId : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderContentId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentLanguage : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
 return HeaderParser.ParseHeaderContentLanguage(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderContentLocation : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
 return HeaderParser.ParseHeaderContentLocation(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderContentMd5 : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderContentMd5(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderContentTransferEncoding : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderContentTransferEncoding(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderContentType : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
     return HeaderParser.ParseHeaderContentType(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderDate : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderDate(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDeferredDelivery : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
return HeaderParser.ParseHeaderDeferredDelivery(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderDispositionNotificationOptions :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationOptions(
str,
index,
endIndex,
tokener);
      }
    }

 private sealed class HeaderDispositionNotificationTo :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationTo(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderDkimSignature : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
   return HeaderParser.ParseHeaderDkimSignature(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderEdiintFeatures : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
  return HeaderParser.ParseHeaderEdiintFeatures(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderEncoding : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderEncoding(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderEncrypted : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderEncrypted(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderFrom : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderFrom(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderInReplyTo : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderInReplyTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderJabberId : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderJabberId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderKeywords : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderKeywords(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderLanguage : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderLanguage(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderLatestDeliveryTime : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderLatestDeliveryTime(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderListId : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderListId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMessageContext : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
  return HeaderParser.ParseHeaderMessageContext(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMessageId : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMessageId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMimeVersion : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
     return HeaderParser.ParseHeaderMimeVersion(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsAcp127MessageIdentifier :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsAcp127MessageIdentifier(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsCodressMessageIndicator :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCodressMessageIndicator(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsCopyPrecedence : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCopyPrecedence(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsExemptedAddress : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExemptedAddress(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsExtendedAuthorisationInfo :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExtendedAuthorisationInfo(
str,
index,
endIndex,
tokener);
      }
    }

  private sealed class HeaderMmhsHandlingInstructions :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsHandlingInstructions(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsMessageInstructions : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageInstructions(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsMessageType : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
 return HeaderParser.ParseHeaderMmhsMessageType(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsOriginatorPlad : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOriginatorPlad(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsOriginatorReference : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOriginatorReference(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsOtherRecipientsIndicatorCc :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOtherRecipientsIndicatorCc(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsOtherRecipientsIndicatorTo :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOtherRecipientsIndicatorTo(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMmhsPrimaryPrecedence : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsPrimaryPrecedence(
str,
index,
endIndex,
tokener);
      }
    }

 private sealed class HeaderMmhsSubjectIndicatorCodes :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsSubjectIndicatorCodes(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderMtPriority : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderMtPriority(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderObsoletes : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderObsoletes(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderOriginalRecipient : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderOriginalRecipient(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderReceived : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReceivedSpf : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
     return HeaderParser.ParseHeaderReceivedSpf(
str,
index,
endIndex,
tokener);
      }
    }

private sealed class HeaderRequireRecipientValidSince :
      StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderRequireRecipientValidSince(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderResentTo : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderResentTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReturnPath : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderReturnPath(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderSender : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderSender(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderSolicitation : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
    return HeaderParser.ParseHeaderSolicitation(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderTo : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderVbrInfo : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderVbrInfo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXArchivedAt : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
     return HeaderParser.ParseHeaderXArchivedAt(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderXRicevuta : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderXRicevuta(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXTiporicevuta : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
   return HeaderParser.ParseHeaderXTiporicevuta(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderXTrasporto : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
      return HeaderParser.ParseHeaderXTrasporto(
str,
index,
endIndex,
tokener);
      }
    }

    private sealed class HeaderXVerificasicurezza : StructuredHeaderField {
      public override int Parse(
string str,
int index,
int endIndex,
ITokener tokener) {
        return HeaderParser.ParseHeaderXVerificasicurezza(
str,
index,
endIndex,
tokener);
      }
    }

    private static IDictionary<string, IHeaderFieldParser> fieldMap =
      CreateHeaderFieldList();

    private static readonly IHeaderFieldParser Unstructured = new
      UnstructuredHeaderField();

    private static IDictionary<string, IHeaderFieldParser>
      CreateHeaderFieldList() {
      // NOTE: Header fields not mentioned here are treated as unstructured
      fieldMap = new Dictionary<string, IHeaderFieldParser>();
      fieldMap["content-return"] = new HeaderX400ContentReturn();
      fieldMap["x400-content-return"] = new HeaderX400ContentReturn();
      fieldMap["delivery-date"] = new HeaderDeliveryDate();
      fieldMap["priority"] = new HeaderPriority();
      fieldMap["importance"] = new HeaderImportance();
      fieldMap["sensitivity"] = new HeaderSensitivity();
      fieldMap["reply-by"] = new HeaderDate();
      fieldMap["x400-content-identifier"] = new HeaderX400ContentIdentifier();
      fieldMap["x400-received"] = new HeaderX400Received();
      fieldMap["x400-mts-identifier"] = new HeaderX400MtsIdentifier();
      fieldMap["x400-trace"] = new HeaderX400Received();
      fieldMap["x400-originator"] = new HeaderX400Originator();
      fieldMap["x400-recipients"] = new HeaderX400Recipients();
      fieldMap["conversion"] = new HeaderConversion();
      fieldMap["conversion-with-loss"] = new HeaderConversionWithLoss();
      fieldMap["supersedes"] = new HeaderSupersedes();
      fieldMap["expires"] = new HeaderDate();
      fieldMap["autoforwarded"] = new HeaderAutoforwarded();
      fieldMap["generate-delivery-report"] = new HeaderGenerateDeliveryReport();
      fieldMap["incomplete-copy"] = new HeaderIncompleteCopy();
 fieldMap["prevent-nondelivery-report"] = new
        HeaderPreventNondeliveryReport();
      fieldMap["alternate-recipient"] = new HeaderAlternateRecipient();
      fieldMap["disclose-recipients"] = new HeaderDiscloseRecipients();
      fieldMap["newsgroups"] = new HeaderNewsgroups();
      fieldMap["path"] = new HeaderPath();
      fieldMap["archive"] = new HeaderArchive();
      fieldMap["control"] = new HeaderControl();
      fieldMap["distribution"] = new HeaderDistribution();
      fieldMap["followup-to"] = new HeaderFollowupTo();
      fieldMap["injection-date"] = new HeaderInjectionDate();
      fieldMap["injection-info"] = new HeaderInjectionInfo();
      fieldMap["user-agent"] = new HeaderUserAgent();
      fieldMap["xref"] = new HeaderXref();
      fieldMap["accept-language"] = new HeaderAcceptLanguage();
      fieldMap["archived-at"] = new HeaderArchivedAt();
      fieldMap["authentication-results"] = new HeaderAuthenticationResults();
      fieldMap["auto-submitted"] = new HeaderAutoSubmitted();
      fieldMap["base"] = new HeaderContentBase();
      fieldMap["bcc"] = new HeaderBcc();
      fieldMap["cc"] = new HeaderTo();
      fieldMap["content-base"] = new HeaderContentBase();
      fieldMap["content-disposition"] = new HeaderContentDisposition();
      fieldMap["content-duration"] = new HeaderContentDuration();
      fieldMap["content-id"] = new HeaderContentId();
      fieldMap["content-language"] = new HeaderContentLanguage();
      fieldMap["content-location"] = new HeaderContentLocation();
      fieldMap["content-md5"] = new HeaderContentMd5();
   fieldMap["content-transfer-encoding"] = new
        HeaderContentTransferEncoding();
      fieldMap["content-type"] = new HeaderContentType();
      fieldMap["date"] = new HeaderDate();
      fieldMap["deferred-delivery"] = new HeaderDeferredDelivery();
      fieldMap["disposition-notification-options"] = new
        HeaderDispositionNotificationOptions();
      fieldMap["disposition-notification-to"] = new
        HeaderDispositionNotificationTo();
      fieldMap["dkim-signature"] = new HeaderDkimSignature();
      fieldMap["ediint-features"] = new HeaderEdiintFeatures();
      fieldMap["encoding"] = new HeaderEncoding();
      fieldMap["encrypted"] = new HeaderEncrypted();
      fieldMap["expiry-date"] = new HeaderDate();
      fieldMap["from"] = new HeaderFrom();
      fieldMap["in-reply-to"] = new HeaderInReplyTo();
      fieldMap["jabber-id"] = new HeaderJabberId();
      fieldMap["keywords"] = new HeaderKeywords();
      fieldMap["language"] = new HeaderLanguage();
      fieldMap["latest-delivery-time"] = new HeaderLatestDeliveryTime();
      fieldMap["list-id"] = new HeaderListId();
      fieldMap["message-context"] = new HeaderMessageContext();
      fieldMap["message-id"] = new HeaderMessageId();
      fieldMap["mime-version"] = new HeaderMimeVersion();
      fieldMap["mmhs-acp127-message-identifier"] = new
        HeaderMmhsAcp127MessageIdentifier();
      fieldMap["mmhs-codress-message-indicator"] = new
        HeaderMmhsCodressMessageIndicator();
      fieldMap["mmhs-copy-precedence"] = new HeaderMmhsCopyPrecedence();
      fieldMap["mmhs-exempted-address"] = new HeaderMmhsExemptedAddress();
      fieldMap["mmhs-extended-authorisation-info"] = new
        HeaderMmhsExtendedAuthorisationInfo();
 fieldMap["mmhs-handling-instructions"] = new
        HeaderMmhsHandlingInstructions();
   fieldMap["mmhs-message-instructions"] = new
        HeaderMmhsMessageInstructions();
      fieldMap["mmhs-message-type"] = new HeaderMmhsMessageType();
      fieldMap["mmhs-originator-plad"] = new HeaderMmhsOriginatorPlad();
   fieldMap["mmhs-originator-reference"] = new
        HeaderMmhsOriginatorReference();
      fieldMap["mmhs-other-recipients-indicator-cc"] = new
        HeaderMmhsOtherRecipientsIndicatorCc();
      fieldMap["mmhs-other-recipients-indicator-to"] = new
        HeaderMmhsOtherRecipientsIndicatorTo();
      fieldMap["mmhs-primary-precedence"] = new HeaderMmhsPrimaryPrecedence();
      fieldMap["mmhs-subject-indicator-codes"] = new
        HeaderMmhsSubjectIndicatorCodes();
      fieldMap["mt-priority"] = new HeaderMtPriority();
      fieldMap["obsoletes"] = new HeaderObsoletes();
      fieldMap["original-from"] = new HeaderFrom();
      fieldMap["original-message-id"] = new HeaderMessageId();
      fieldMap["original-recipient"] = new HeaderOriginalRecipient();
      fieldMap["received"] = new HeaderReceived();
      fieldMap["received-spf"] = new HeaderReceivedSpf();
      fieldMap["references"] = new HeaderInReplyTo();
      fieldMap["reply-to"] = new HeaderResentTo();
      fieldMap["require-recipient-valid-since"] = new
        HeaderRequireRecipientValidSince();
      fieldMap["resent-bcc"] = new HeaderBcc();
      fieldMap["resent-cc"] = new HeaderResentTo();
      fieldMap["resent-date"] = new HeaderDate();
      fieldMap["resent-from"] = new HeaderFrom();
      fieldMap["resent-message-id"] = new HeaderMessageId();
      fieldMap["resent-reply-to"] = new HeaderResentTo();
      fieldMap["resent-sender"] = new HeaderSender();
      fieldMap["resent-to"] = new HeaderResentTo();
      fieldMap["return-path"] = new HeaderReturnPath();
      fieldMap["sender"] = new HeaderSender();
      fieldMap["solicitation"] = new HeaderSolicitation();
      fieldMap["to"] = new HeaderTo();
      fieldMap["vbr-info"] = new HeaderVbrInfo();
      fieldMap["x-archived-at"] = new HeaderXArchivedAt();
      fieldMap["x-mittente"] = new HeaderSender();
      fieldMap["x-ricevuta"] = new HeaderXRicevuta();
      fieldMap["x-riferimento-message-id"] = new HeaderMessageId();
      fieldMap["x-tiporicevuta"] = new HeaderXTiporicevuta();
      fieldMap["x-trasporto"] = new HeaderXTrasporto();
      fieldMap["x-verificasicurezza"] = new HeaderXVerificasicurezza();
      return fieldMap;
    }

    public static IHeaderFieldParser GetParser(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return fieldMap.ContainsKey(name) ? fieldMap[name] : Unstructured;
    }
  }
}
