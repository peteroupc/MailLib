/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
  internal static class HeaderFieldParsers {
    private sealed class UnstructuredHeaderField : IHeaderFieldParser {
      public string DowngradeHeaderField(string name, string str) {
        return HeaderEncoder.EncodeFieldAsEncodedWords(name, str);
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

      public virtual string DowngradeHeaderField(string name, string str) {
        // The 2 below is for the colon and space after the header field name
        var enc = new HeaderEncoder(
  Message.MaxRecHeaderLineLength,
  name.Length + 2);
        str = HeaderEncoder.TrimLeadingFWS(str);
        str = DowngradeHeaderFieldValue(enc, this, str);
        return new HeaderEncoder().AppendFieldName(name) + str;
      }

      private static string DowngradeComments(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        string str) {
        if (str.IndexOf('(') < 0 ||
          !Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // Contains no comments, or no text needs to be encoded
          return str;
        }
        var tokener = new Tokener();
        int endIndex = shf.Parse(str, 0, str.Length, tokener);
        if (endIndex != str.Length) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        var lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        foreach (int[] token in tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenComment) {
            int startIndex = token[1];
            endIndex = token[2];
            if (Message.HasTextToEscape(str, startIndex, endIndex)) {
              enc.AppendString(str, lastIndex, startIndex);
              Rfc2047.EncodeComment(
               enc,
               str,
               startIndex,
               endIndex);
              lastIndex = endIndex;
            }
          }
        }
        enc.AppendString(str, lastIndex, str.Length);
        return enc.ToString();
      }

      private static string DowngradePhrases(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        string str) {
        if (!Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No text needs to be encoded
          return str;
        }
        var tokener = new Tokener();
        int endIndex = shf.Parse(str, 0, str.Length, tokener);
        if (endIndex != str.Length) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        var lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        foreach (int[] token in tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenPhrase) {
            int startIndex = token[1];
            endIndex = token[2];
            enc.AppendString(str, lastIndex, startIndex);
            Rfc2047.EncodePhraseText(
                enc,
                str,
                startIndex,
                endIndex,
                tokens);
            lastIndex = endIndex;
          }
        }
        enc.AppendString(str, lastIndex, str.Length);
        return enc.ToString();
      }

      private static string EncodeDomain(
  string str,
  int startIndex,
  int endIndex) {
        string domain = HeaderParserUtility.ParseDomain(
        str,
        startIndex,
        endIndex);
        // NOTE: "domain" can include domain literals, enclosed
        // in brackets; they are invalid under
        // "IsValidDomainName" .
        domain = (Message.HasTextToEscape(domain) &&
                  Idna.IsValidDomainName(domain, false)) ?
          Idna.EncodeDomainName(domain) :
              str.Substring(
        startIndex,
        endIndex - startIndex);
        return Message.HasTextToEscape(domain) ? null : domain;
      }

      private static string DowngradeGroups(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        string str) {
        string originalString = str;
        if (str.IndexOf(':') < 0 ||
           str.IndexOf('@') < 0 ||
           !Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No colon or at-sign, or no text needs to be encoded
          return str;
        }
        var tokener = new Tokener();
        IList<string> originalGroups = null;
        var groupIndex = 0;
        int endIndex = shf.Parse(str, 0, str.Length, tokener);
        if (endIndex != str.Length) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        var lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        foreach (int[] token in tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenGroup) {
            int startIndex = token[1];
            endIndex = token[2];
            var nonasciiLocalParts = false;
            var displayNameEnd = -1;
            // Try to find non-ASCII local-parts (HasTextToEscape is broader
            // than non-ASCII; say non-ASCII as shorthand), and find the end
            // of the group's display name
            foreach (int[] token2 in tokens) {
              if (token2[0] == HeaderParserUtility.TokenPhrase &&
                   displayNameEnd < 0) {
                displayNameEnd = token2[2];
              }
              if (token2[0] == HeaderParserUtility.TokenLocalPart &&
                  token2[1] >= startIndex && token2[2] <= endIndex) {
                // Local part within a group
                if (Message.HasTextToEscape(
                str,
                token2[1],
                token2[2])) {
                  nonasciiLocalParts = true;
                  break;
                }
              }
            }
            // No non-ASCII local parts found, so
            // downgrade the domains if they exist
            if (!nonasciiLocalParts) {
              int localLastIndex = startIndex;
              var nonasciiDomains = false;
              var sb2 = new StringBuilder();
              foreach (int[] token2 in tokens) {
                if (token2[0] == HeaderParserUtility.TokenDomain &&
                   token2[1] >= startIndex && token2[2] <= endIndex) {
                  // Domain within the group
                  string domain = EncodeDomain(str, token2[1], token2[2]);
                  if (domain == null) {
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
              nonasciiLocalParts = nonasciiDomains;
              if (!nonasciiLocalParts) {
                // All of the domains could be converted to ASCII
                sb2.Append(
                str.Substring(
                localLastIndex,
                endIndex - localLastIndex));
                enc.AppendString(str, lastIndex, startIndex);
                enc.AppendString(sb2.ToString());
                lastIndex = endIndex;
              }
            }
            if (nonasciiLocalParts) {
              // At least some of the domains could not
              // be converted to ASCII
              originalGroups = originalGroups ?? shf.ParseGroupLists(
    originalString,
    0,
    originalString.Length);
              enc.AppendString(str, lastIndex, displayNameEnd);
              enc.AppendSpace();
              enc.AppendAsEncodedWords(originalGroups[groupIndex]);
              enc.AppendSpace();
              enc.AppendSymbol(":;");
              lastIndex = endIndex;
            }
            ++groupIndex;
          }
        }
        enc.AppendString(str, lastIndex, str.Length);
        return enc.ToString();
      }

      private static string DowngradeDomains(
          HeaderEncoder enc,
          StructuredHeaderField shf,
          string str) {
        string originalString = str;
        if (!Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No text needs to be encoded
          return str;
        }
        var tokener = new Tokener();
       int endIndex = shf.Parse(str, 0, str.Length, tokener);
        if (endIndex != str.Length) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        var lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        foreach (int[] token in tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenDomain) {
                    string domain = EncodeDomain(str, token[1], token[2]);
                    if (domain != null) {
                    // Treat only domain literals and dot-atoms with
                    // at least one dot as domains (this is primarily used
                    // for the Received header field, where RFC 5322 atoms not
                    // separated by dots are treated as words, not domains)
                    if (str[token[1]] == '[' || domain.IndexOf('.') >= 0) {
            enc.AppendString(str, lastIndex, token[1]);
            enc.AppendString(domain);
            lastIndex = token[2];
                    }
                  }
              }
        }
        enc.AppendString(str, lastIndex, str.Length);
        return enc.ToString();
      }

      private static string DowngradeMailboxes(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        string str) {
        string originalString = str;
        if (str.IndexOf('@') < 0 ||
           !Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No at-sign, or no text needs to be encoded
          return str;
        }
        var tokener = new Tokener();
        var groupIndex = 0;
        int endIndex = shf.Parse(str, 0, str.Length, tokener);
        if (endIndex != str.Length) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        var lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        foreach (int[] token in tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenMailbox) {
            int startIndex = token[1];
            endIndex = token[2];
            var nonasciiLocalPart = false;
            var hasPhrase = false;
            foreach (int[] token2 in tokens) {
              hasPhrase |= token2[0] == HeaderParserUtility.TokenPhrase;
              if (token2[0] == HeaderParserUtility.TokenLocalPart &&
              token2[1] >= startIndex && token2[2] <= endIndex &&
              Message.HasTextToEscape(
              str,
              token2[1],
              token2[2])) {
                nonasciiLocalPart = true;
                break;
              }
            }
            if (!nonasciiLocalPart) {
              int localLastIndex = startIndex;
              var nonasciiDomains = false;
              var sb2 = new StringBuilder();
              foreach (int[] token2 in tokens) {
                if (token2[0] == HeaderParserUtility.TokenDomain) {
                  if (token2[1] >= startIndex && token2[2] <= endIndex) {
                    // Domain within the mailbox
                    string domain = EncodeDomain(str, token2[1], token2[2]);
                    if (domain == null) {
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
                enc.AppendString(str, lastIndex, startIndex);
                enc.AppendString(sb2.ToString());
                lastIndex = endIndex;
              }
            }
            // Downgrading failed
            if (nonasciiLocalPart) {
              if (!hasPhrase) {
                string addrSpec = str.Substring(
                token[1],
                token[2] - token[1]);
                enc.AppendString(str, lastIndex, startIndex);
                enc.AppendSpace();
                enc.AppendAsEncodedWords(addrSpec);
                enc.AppendSpace();
                enc.AppendSymbol(":;");
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
                enc.AppendString(str, lastIndex, angleAddrStart);
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
                enc.AppendSpaceIfNeeded();
                enc.AppendAsEncodedWords(addrSpec);
                enc.AppendSpace();
                enc.AppendSymbol(":;");
              }
              lastIndex = endIndex;
            }
            ++groupIndex;
          }
        }
        enc.AppendString(str, lastIndex, str.Length);
        return enc.ToString();
      }

      public static string DowngradeHeaderFieldValue(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        string str) {
        str = DowngradeComments(enc, shf, str);
        str = DowngradePhrases(enc, shf, str);
        str = DowngradeGroups(enc, shf, str);
        str = DowngradeMailboxes(enc, shf, str);
        str = DowngradeDomains(enc, shf, str);
        return str;
      }

      public string DecodeEncodedWords(string str) {
#if DEBUG
        if (str == null) {
          throw new ArgumentNullException(nameof(str));
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
        var lastIndex = 0;
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

    private sealed class HeaderReceived : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }

     private static string TrimFWSFromRight(string str) {
      if (String.IsNullOrEmpty(str)) {
        return str;
      }
      var index = 0;
      int valueSLength = str.Length;
      int indexStart = index;
      index = str.Length - 1;
      while (index >= 0) {
        char c = str[index];
        if (c == 0x0a && index >= 1 && str[index - 1] == 0x0d) {
           // CRLF
           index -= 2;
        } else if (c == 0x09 || c == 0x20) {
          --index;
        } else {
          int indexEnd = index + 1;
          if (indexEnd == indexStart) {
            return String.Empty;
          }
          return (indexEnd == str.Length && indexStart == 0) ? str :
            str.Substring(indexStart, indexEnd - indexStart);
        }
      }
      return String.Empty;
    }

      private static bool IsCFWSWordCFWS(
  string str,
  int index,
  int endIndex,
  string word) {
        int si;
        var sb = new StringBuilder();
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        si = HeaderParserUtility.ParseWord(str, index, endIndex, sb);
        if (si == index) {
          return false;
        }
        string checkword = DataUtilities.ToLowerCaseAscii(sb.ToString());
        if (!checkword.Equals(word)) {
          return false;
        }
        index = HeaderParser.ParseCFWS(
  str,
  si,
  endIndex,
  null);
        return index == endIndex;
      }

      public override string DowngradeHeaderField(string name, string str) {
        // The 2 below is for the colon and space after the header field name
        var enc = new HeaderEncoder(
  Message.MaxRecHeaderLineLength,
  name.Length + 2);
        str = HeaderEncoder.TrimLeadingFWS(str);
        str = StructuredHeaderField.DowngradeHeaderFieldValue(
          enc,
          this,
          str);
        if (this.Parse(str, 0, str.Length, null) != str.Length) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        string header = str;
       var index = 0;
       var changed = false;
       var sb = new StringBuilder();
       int tokenEnd = HeaderParser.ParseCFWS(header, 0, header.Length, null);
       while (index < header.Length) {
         int newindex = HeaderParser.ParseReceivedToken(
  header,
  index,
  header.Length,
  null);
         if (newindex == index) {
         tokenEnd = HeaderParser.ParseCFWS(
  header,
  index,
  header.Length,
  null);
           sb.Append(header.Substring(index, tokenEnd - index));
           break;
         }
         if (IsCFWSWordCFWS(header, index, newindex, "for")) {
           var tokener = new Tokener();
           int clauseEnd = HeaderParser.ParseReceivedToken(
               header,
               newindex,
               header.Length,
               tokener);
           IList<int[]> tokens = tokener.GetTokens();
           var notGoodLocalPart = false;
           foreach (int[] token in tokens) {
             if (token[0] == HeaderParserUtility.TokenLocalPart) {
               if (Message.HasTextToEscape(header, token[1], token[2])) {
                 notGoodLocalPart = true;
                 break;
               }
             }
           }
           if (notGoodLocalPart) {
             changed = true;
           } else {
             sb.Append(header.Substring(index, clauseEnd - index));
           }
           index = clauseEnd;
         } else if (IsCFWSWordCFWS(header, index, newindex, "id")) {
           int clauseEnd = HeaderParser.ParseReceivedToken(
               header,
               newindex,
               header.Length,
               null);
           if (Message.HasTextToEscape(header, index, clauseEnd)) {
             changed = true;
           } else {
             sb.Append(header.Substring(index, clauseEnd - index));
           }
           index = clauseEnd;
         } else {
           sb.Append(header.Substring(index, newindex - index));
           index = newindex;
         }
       }
       string receivedPart = sb.ToString();
       string datePart = header.Substring(tokenEnd);
       if (changed) {
         receivedPart = TrimFWSFromRight(receivedPart);
         return new HeaderEncoder().AppendFieldName(name)
            .AppendString(receivedPart + datePart).ToString();
       } else {
         return new HeaderEncoder().AppendFieldName(name).ToString() +
            str;
       }
     }
    }

    private sealed class HeaderContentDisposition : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        string s = str.Substring(index, endIndex - index);
        int ret = ContentDisposition.Parse(s, null) == null ? index : endIndex;
        if (ret == endIndex) {
          HeaderParserUtility.TraverseCFWSAndQuotedStrings(
  str,
  index,
  endIndex,
  tokener);
        }
        return ret;
      }
    }

    private sealed class HeaderContentType : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        string s = str.Substring(index, endIndex - index);
        int ret = MediaType.Parse(s, null) == null ? index : endIndex;
        if (ret == endIndex) {
          HeaderParserUtility.TraverseCFWSAndQuotedStrings(
  str,
  index,
  endIndex,
  tokener);
        }
        return ret;
      }
    }

    private sealed class HeaderAutoSubmitted : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        // NOTE: Same syntax as Content-Disposition
        string s = str.Substring(index, endIndex - index);
        int ret = ContentDisposition.Parse(s, null) == null ? index : endIndex;
        if (ret == endIndex) {
          HeaderParserUtility.TraverseCFWSAndQuotedStrings(
  str,
  index,
  endIndex,
  tokener);
        }
        return ret;
      }
    }

    private sealed class HeaderSioLabel : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        int si = index;
        si = HeaderParser.ParseFWS(str, si, endIndex, tokener);
        var parameters = new Dictionary<string, string>();
        int ret = MediaType.ParseParameters(
           str,
           si,
           endIndex,
           false,
           parameters) ? endIndex : index;
        if (ret != endIndex) {
          return index;
        }
        if (HeaderParserUtility.HasComments(str, index, endIndex)) {
          return index;
        }
        HeaderParserUtility.TraverseCFWSAndQuotedStrings(
  str,
  index,
  endIndex,
  tokener);
        return ret;
      }
    }

    private sealed class HeaderArchive : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        // NOTE: Almost the same syntax as Content-Disposition, except
        // first character must be a space (since this is a Netnews header
        // field),
        // and a limited selection of "disposition types" is valid;
        // however, the initial space is not checked here, a behavior
        // allowed by RFC
        // 5536 sec. 2.2
        string s = str.Substring(index, endIndex - index);
        ContentDisposition cd = ContentDisposition.Parse(s, null);
        if (cd == null) {
          return index;
        }
        int ret = (cd.DispositionType.Equals("no") ||
 cd.DispositionType.Equals("yes")) ? endIndex : index; if (ret ==
            endIndex) {
          HeaderParserUtility.TraverseCFWSAndQuotedStrings(
  str,
  index,
  endIndex,
  tokener);
        }
        return ret;
      }
    }

    private sealed class HeaderInjectionInfo : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        // NOTE: Under the syntax of InjectionInfo, the
        // first character must be a space (since this is a Netnews header
        // field);
        // however, the initial space is not checked here, a behavior
        // allowed by RFC
        // 5536 sec. 2.2
        int indexStart, indexTemp, state, tx2;
        indexStart = index;
        state = (tokener != null) ? tokener.GetState() : 0;
        indexTemp = index;
        do {
          index = HeaderParser.ParseCFWS(str, index, endIndex, tokener);
          tx2 = HeaderParser.ParsePathIdentity(str, index, endIndex, tokener);
          if (tx2 == index) {
            index = indexStart; break;
          } else {
            index = tx2;
          }
          index = HeaderParser.ParseCFWS(str, index, endIndex, tokener);
          if (index < endIndex && (str[index] == 59)) {
            ++index;
            var parameters = new Dictionary<string, string>();
            index = MediaType.ParseParameters(
                    str,
                    index,
                    endIndex,
                    false,
                    parameters) ? endIndex : indexStart;
            if (index == endIndex) {
              HeaderParserUtility.TraverseCFWSAndQuotedStrings(
  str,
  indexStart,
  endIndex,
  tokener);
            }
          }
          indexTemp = index;
        } while (false);
        if (tokener != null && indexTemp == indexStart) {
          tokener.RestoreState(state);
        }
        return indexTemp;
      }
    }
    // -------------- generic classes --------------
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

    private sealed class HeaderContentTranslationType : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderContentTranslationType(
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

    private sealed class HeaderExpandedDate : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderExpandedDate(
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

    private sealed class HeaderNntpPostingHost : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderNntpPostingHost(
  str,
  index,
  endIndex,
  tokener);
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

    private sealed class HeaderArcAuthenticationResults :
        StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderArcAuthenticationResults(
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

    private sealed class HeaderBcc : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderCancelLock : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderCancelLock(
  str,
  index,
  endIndex,
  tokener);
      }
    }

    private sealed class HeaderCancelKey : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderCancelKey(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderTlsReportDomain : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderTlsReportDomain(
  str,
  index,
  endIndex,
  tokener);
      }
    }

    private sealed class HeaderTlsReportSubmitter : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderTlsReportSubmitter(
  str,
  index,
  endIndex,
  tokener);
      }
    }

    private sealed class HeaderFormSub : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderFormSub(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXPgpSig : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderXPgpSig(str, index, endIndex, tokener);
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

    private sealed class HeaderMmhsAuthorizingUsers : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsAuthorizingUsers(
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

    private sealed class HeaderArcMessageSignature : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderArcMessageSignature(
  str,
  index,
  endIndex,
  tokener);
      }
    }

    private sealed class HeaderArcSeal : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderArcSeal(str, index, endIndex, tokener);
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

    private sealed class HeaderEesstVersion : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderEesstVersion(
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

    private sealed class HeaderListArchive : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderListArchive(
  str,
  index,
  endIndex,
  tokener);
      }
    }

    private sealed class HeaderListHelp : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderListHelp(str, index, endIndex, tokener);
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

    private sealed class HeaderListOwner : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderListOwner(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderListPost : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderListPost(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderListSubscribe : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderListSubscribe(
  str,
  index,
  endIndex,
  tokener);
      }
    }

    private sealed class HeaderListUnsubscribe : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderListUnsubscribe(
  str,
  index,
  endIndex,
  tokener);
      }
    }

    private sealed class HeaderListUnsubscribePost : StructuredHeaderField {
      public override int Parse(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
        return HeaderParser.ParseHeaderListUnsubscribePost(
  str,
  index,
  endIndex,
  tokener);
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
      fieldMap["content-disposition"] = new HeaderContentDisposition();
      fieldMap["content-type"] = new HeaderContentType();
      fieldMap["auto-submitted"] = new HeaderAutoSubmitted();
      fieldMap["archive"] = new HeaderArchive();
      fieldMap["autosubmitted"] = new HeaderAutoforwarded();  // same syntax
      fieldMap["sio-label"] = new HeaderSioLabel();
      fieldMap["sio-label-history"] = new HeaderSioLabel();
      fieldMap["injection-info"] = new HeaderInjectionInfo();
      //------------------ generic ------------------
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
      fieldMap["content-translation-type"] = new HeaderContentTranslationType();
      fieldMap["generate-delivery-report"] = new HeaderGenerateDeliveryReport();
      fieldMap["incomplete-copy"] = new HeaderIncompleteCopy();
      fieldMap["prevent-nondelivery-report"] = new
             HeaderPreventNondeliveryReport();
      fieldMap["alternate-recipient"] = new HeaderAlternateRecipient();
      fieldMap["disclose-recipients"] = new HeaderDiscloseRecipients();
      fieldMap["expanded-date"] = new HeaderExpandedDate();
      fieldMap["newsgroups"] = new HeaderNewsgroups();
      fieldMap["path"] = new HeaderPath();
      fieldMap["control"] = new HeaderControl();
      fieldMap["distribution"] = new HeaderDistribution();
      fieldMap["followup-to"] = new HeaderFollowupTo();
      fieldMap["injection-date"] = new HeaderInjectionDate();
      fieldMap["user-agent"] = new HeaderUserAgent();
      fieldMap["xref"] = new HeaderXref();
      fieldMap["nntp-posting-date"] = new HeaderInjectionDate();
      fieldMap["nntp-posting-host"] = new HeaderNntpPostingHost();
      fieldMap["accept-language"] = new HeaderAcceptLanguage();
      fieldMap["archived-at"] = new HeaderArchivedAt();
      fieldMap["arc-authentication-results"] = new
             HeaderArcAuthenticationResults();
      fieldMap["authentication-results"] = new HeaderAuthenticationResults();
      fieldMap["base"] = new HeaderContentBase();
      fieldMap["bcc"] = new HeaderBcc();
      fieldMap["cc"] = new HeaderTo();
      fieldMap["cancel-lock"] = new HeaderCancelLock();
      fieldMap["cancel-key"] = new HeaderCancelKey();
      fieldMap["tls-report-domain"] = new HeaderTlsReportDomain();
      fieldMap["tls-report-submitter"] = new HeaderTlsReportSubmitter();
      fieldMap["form-sub"] = new HeaderFormSub();
      fieldMap["x-pgp-sig"] = new HeaderXPgpSig();
      fieldMap["content-base"] = new HeaderContentBase();
      fieldMap["content-duration"] = new HeaderContentDuration();
      fieldMap["content-id"] = new HeaderContentId();
      fieldMap["content-language"] = new HeaderContentLanguage();
      fieldMap["content-location"] = new HeaderContentLocation();
      fieldMap["content-md5"] = new HeaderContentMd5();
      fieldMap["content-transfer-encoding"] = new
           HeaderContentTransferEncoding();
      fieldMap["date"] = new HeaderDate();
      fieldMap["deferred-delivery"] = new HeaderDeferredDelivery();
      fieldMap["disposition-notification-options"] = new
        HeaderDispositionNotificationOptions();
      fieldMap["disposition-notification-to"] = new
        HeaderDispositionNotificationTo();
      fieldMap["mmhs-authorizing-users"] = new HeaderMmhsAuthorizingUsers();
      fieldMap["dkim-signature"] = new HeaderDkimSignature();
      fieldMap["arc-message-signature"] = new HeaderArcMessageSignature();
      fieldMap["arc-seal"] = new HeaderArcSeal();
      fieldMap["ediint-features"] = new HeaderEdiintFeatures();
      fieldMap["eesst-version"] = new HeaderEesstVersion();
      fieldMap["encoding"] = new HeaderEncoding();
      fieldMap["encrypted"] = new HeaderEncrypted();
      fieldMap["expiry-date"] = new HeaderDate();
      fieldMap["from"] = new HeaderFrom();
      fieldMap["in-reply-to"] = new HeaderInReplyTo();
      fieldMap["jabber-id"] = new HeaderJabberId();
      fieldMap["keywords"] = new HeaderKeywords();
      fieldMap["language"] = new HeaderLanguage();
      fieldMap["latest-delivery-time"] = new HeaderLatestDeliveryTime();
      fieldMap["list-archive"] = new HeaderListArchive();
      fieldMap["list-help"] = new HeaderListHelp();
      fieldMap["list-id"] = new HeaderListId();
      fieldMap["list-owner"] = new HeaderListOwner();
      fieldMap["list-post"] = new HeaderListPost();
      fieldMap["list-subscribe"] = new HeaderListSubscribe();
      fieldMap["list-unsubscribe"] = new HeaderListUnsubscribe();
      fieldMap["list-unsubscribe-post"] = new HeaderListUnsubscribePost();
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
        throw new ArgumentNullException(nameof(name));
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return fieldMap.ContainsKey(name) ? fieldMap[name] : Unstructured;
    }
  }
}
