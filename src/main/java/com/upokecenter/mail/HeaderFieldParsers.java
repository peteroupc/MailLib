package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

  final class HeaderFieldParsers {
private HeaderFieldParsers() {
}
    private static final class UnstructuredHeaderField implements IHeaderFieldParser {
      public String DowngradeHeaderField(String name, String str) {
        return HeaderEncoder.EncodeFieldAsEncodedWords(name, str);
      }

      public String DecodeEncodedWords(String str) {
        // For unstructured header fields.
        return Rfc2047.DecodeEncodedWords(
          str,
          0,
          str.length(),
          EncodedWordContext.Unstructured);
      }

      public boolean IsStructured() {
        return false;
      }

      public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return endIndex;
      }
    }

    private static abstract class StructuredHeaderField implements IHeaderFieldParser {
      public abstract int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener);

      private List<String> ParseGroupLists(
        String str,
        int index,
        int endIndex) {
        ArrayList<String> groups = new ArrayList<String>();
        Tokener tokener = new Tokener();
        this.Parse(str, index, endIndex, tokener);
        for (int[] token : tokener.GetTokens()) {
          if (token[0] == HeaderParserUtility.TokenGroup) {
            int startIndex = token[1];
            endIndex = token[2];
            String groupList = HeaderParserUtility.ParseGroupList(
              str,
              startIndex,
              endIndex);
            groupList = ParserUtility.TrimSpaceAndTab(groupList);
            groups.add(groupList);
          }
        }
        return groups;
      }

      public String DowngradeHeaderField(String name, String str) {
        // The 2 below is for the colon and space after the header field name
        HeaderEncoder enc = new HeaderEncoder(
          Message.MaxRecHeaderLineLength,
          name.length() + 2);
        str = HeaderEncoder.TrimLeadingFWS(str);
        str = DowngradeHeaderFieldValue(enc, this, str);
        return new HeaderEncoder().AppendFieldName(name) + str;
      }

      private static String DowngradeComments(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        String str) {
        if (str.indexOf('(') < 0 ||
          !Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // Contains no comments, or no text needs to be encoded
          return str;
        }
        Tokener tokener = new Tokener();
        int endIndex = shf.Parse(str, 0, str.length(), tokener);
        if (endIndex != str.length()) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        List<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        for (int[] token : tokens) {
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
        enc.AppendString(str, lastIndex, str.length());
        return enc.toString();
      }

      private static String DowngradePhrases(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        String str) {
        if (!Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No text needs to be encoded
          return str;
        }
        Tokener tokener = new Tokener();
        int endIndex = shf.Parse(str, 0, str.length(), tokener);
        if (endIndex != str.length()) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        List<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        for (int[] token : tokens) {
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
        enc.AppendString(str, lastIndex, str.length());
        return enc.toString();
      }

      private static String EncodeDomain(
        String str,
        int startIndex,
        int endIndex) {
        String domain = HeaderParserUtility.ParseDomain(
          str,
          startIndex,
          endIndex);
        // NOTE: "domain" can include domain literals, enclosed
        // in brackets; they are invalid under
        // "IsValidDomainName" .
        domain = (Message.HasTextToEscape(domain) &&
            Idna.IsValidDomainName(domain, false)) ?
          Idna.EncodeDomainName(domain) : str.substring(
            startIndex, (
            startIndex)+(endIndex - startIndex));
        return Message.HasTextToEscape(domain) ? null : domain;
      }

      private static String DowngradeGroups(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        String str) {
        String originalString = str;
        if (str.indexOf(':') < 0 ||
          str.indexOf('@') < 0 ||
          !Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No colon or at-sign, or no text needs to be encoded
          return str;
        }
        Tokener tokener = new Tokener();
        List<String> originalGroups = null;
        int groupIndex = 0;
        int endIndex = shf.Parse(str, 0, str.length(), tokener);
        if (endIndex != str.length()) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        List<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        for (int[] token : tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenGroup) {
            int startIndex = token[1];
            endIndex = token[2];
            boolean nonasciiLocalParts = false;
            int displayNameEnd = -1;
            // Try to find non-ASCII local-parts (HasTextToEscape is broader
            // than non-ASCII; say non-((ASCII instanceof shorthand) ? (shorthand)ASCII : null)), and find the end
            // of the group's display name
            for (int[] token2 : tokens) {
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
              boolean nonasciiDomains = false;
              StringBuilder sb2 = new StringBuilder();
              for (int[] token2 : tokens) {
                if (token2[0] == HeaderParserUtility.TokenDomain &&
                  token2[1] >= startIndex && token2[2] <= endIndex) {
                  // Domain within the group
                  String domain = EncodeDomain(str, token2[1], token2[2]);
                  if (domain == null) {
                    // ASCII encoding failed
                    nonasciiDomains = true;
                    break;
                  }
                  sb2.append(
                    str.substring(
                      localLastIndex, (
                      localLastIndex)+(token2[1] - localLastIndex)));
                  sb2.append(domain);
                  localLastIndex = token2[2];
                }
              }
              nonasciiLocalParts = nonasciiDomains;
              if (!nonasciiLocalParts) {
                // All of the domains could be converted to ASCII
                sb2.append(
                  str.substring(
                    localLastIndex, (
                    localLastIndex)+(endIndex - localLastIndex)));
                enc.AppendString(str, lastIndex, startIndex);
                enc.AppendString(sb2.toString());
                lastIndex = endIndex;
              }
            }
            if (nonasciiLocalParts) {
              // At least some of the domains could not
              // be converted to ASCII
              originalGroups = (originalGroups == null) ? (shf.ParseGroupLists(
                originalString,
                0,
                originalString.length())) : originalGroups;
              enc.AppendString(str, lastIndex, displayNameEnd);
              enc.AppendSpace();
              enc.AppendAsEncodedWords(originalGroups.get(groupIndex));
              enc.AppendSpace();
              enc.AppendSymbol(":;");
              lastIndex = endIndex;
            }
            ++groupIndex;
          }
        }
        enc.AppendString(str, lastIndex, str.length());
        return enc.toString();
      }

      private static String DowngradeDomains(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        String str) {
        String originalString = str;
        if (!Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No text needs to be encoded
          return str;
        }
        Tokener tokener = new Tokener();
        int endIndex = shf.Parse(str, 0, str.length(), tokener);
        if (endIndex != str.length()) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        List<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        for (int[] token : tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenDomain) {
            String domain = EncodeDomain(str, token[1], token[2]);
            if (domain != null) {
              // Treat only domain literals and dot-atoms with
              // at least one dot as domains (this is primarily used
              // for the Received header field, where RFC 5322 atoms not
              // separated by dots are ((treated instanceof words) ? (words)treated : null), not domains)
              if (str.charAt(token[1]) == '[' || domain.indexOf('.') >= 0) {
                enc.AppendString(str, lastIndex, token[1]);
                enc.AppendString(domain);
                lastIndex = token[2];
              }
            }
          }
        }
        enc.AppendString(str, lastIndex, str.length());
        return enc.toString();
      }

      private static String DowngradeMailboxes(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        String str) {
        String originalString = str;
        if (str.indexOf('@') < 0 ||
          !Message.HasTextToEscapeOrEncodedWordStarts(str)) {
          // No at-sign, or no text needs to be encoded
          return str;
        }
        Tokener tokener = new Tokener();
        int groupIndex = 0;
        int endIndex = shf.Parse(str, 0, str.length(), tokener);
        if (endIndex != str.length()) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        List<int[]> tokens = tokener.GetTokens();
        enc.Reset();
        for (int[] token : tokens) {
          if (token[1] < lastIndex) {
            continue;
          }
          if (token[0] == HeaderParserUtility.TokenMailbox) {
            int startIndex = token[1];
            endIndex = token[2];
            boolean nonasciiLocalPart = false;
            boolean hasPhrase = false;
            for (int[] token2 : tokens) {
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
              boolean nonasciiDomains = false;
              StringBuilder sb2 = new StringBuilder();
              for (int[] token2 : tokens) {
                if (token2[0] == HeaderParserUtility.TokenDomain) {
                  if (token2[1] >= startIndex && token2[2] <= endIndex) {
                    // Domain within the mailbox
                    String domain = EncodeDomain(str, token2[1], token2[2]);
                    if (domain == null) {
                      // ASCII encoding failed
                      nonasciiDomains = true;
                      break;
                    }
                    sb2.append(
                      str.substring(
                        localLastIndex, (
                        localLastIndex)+(token2[1] - localLastIndex)));
                    sb2.append(domain);
                    localLastIndex = token2[2];
                  }
                }
              }
              nonasciiLocalPart = nonasciiDomains;
              if (!nonasciiLocalPart) {
                // All of the domains could be converted to ASCII
                sb2.append(
                  str.substring(
                    localLastIndex, (
                    localLastIndex)+(endIndex - localLastIndex)));
                enc.AppendString(str, lastIndex, startIndex);
                enc.AppendString(sb2.toString());
                lastIndex = endIndex;
              }
            }
            // Downgrading failed
            if (nonasciiLocalPart) {
              if (!hasPhrase) {
                String addrSpec = str.substring(
                  token[1], (
                  token[1])+(token[2] - token[1]));
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
                // append the rest of the String so far up to and
                // including the phrase
                enc.AppendString(str, lastIndex, angleAddrStart);
                int addrSpecStart = HeaderParser.ParseCFWS(
                  str,
                  angleAddrStart,
                  token[2],
                  null);
                if (addrSpecStart < token[2] && str.charAt(addrSpecStart) == '<') {
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
                String addrSpec = str.substring(
                  addrSpecStart, (
                  addrSpecStart)+(addrSpecEnd - addrSpecStart));
                enc.AppendSpaceIfNeeded();
                enc.AppendAsEncodedWords(addrSpec);
                enc.AppendSpace();
                // NOTE: Here, the ":;" symbol is atomic and is
                // not separated into ":", which separates the group
                // name and the addresses, and ";", which ends
                // the group
                enc.AppendSymbol(":;");
              }
              lastIndex = endIndex;
            }
            ++groupIndex;
          }
        }
        enc.AppendString(str, lastIndex, str.length());
        return enc.toString();
      }

      public static String DowngradeHeaderFieldValue(
        HeaderEncoder enc,
        StructuredHeaderField shf,
        String str) {
        str = DowngradeComments(enc, shf, str);
        str = DowngradePhrases(enc, shf, str);
        str = DowngradeGroups(enc, shf, str);
        str = DowngradeMailboxes(enc, shf, str);
        str = DowngradeDomains(enc, shf, str);
        return str;
      }

      public String DecodeEncodedWords(String str) {
        // For structured header fields that allow comments only wherever
        // whitespace
        // is allowed, and allow parentheses only for comments
        if (str.length() < 9) {
          // too short for encoded words
          return str;
        }
        if (str.indexOf("=?") < 0) {
          // No encoded words
          return str;
        }
        StringBuilder sb = new StringBuilder();
        Tokener tokener = new Tokener();
        int endIndex = this.Parse(str, 0, str.length(), tokener);
        if (endIndex != str.length()) {
          // The header field is syntactically invalid,
          // so don't decode any encoded words
          // System.out.println("Invalid syntax: " + this.getClass().getName() +
          // ", " + str);
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        List<int[]> tokens = tokener.GetTokens();
        for (int[] token : tokens) {
          // System.out.println("" + token[0] + " [" +
          // (str.substring(token[1],(token[1])+(token[2]-token[1]))) + "]");
          if (token[0] == HeaderParserUtility.TokenComment && token[0] >=
            lastIndex) {
            // This is a comment token
            int startIndex = token[1];
            endIndex = token[2];
            String newComment = Rfc2047.DecodeEncodedWords(
              str,
              startIndex + 1,
              endIndex - 1,
              EncodedWordContext.Comment);
            sb.append(str.substring(lastIndex, (lastIndex)+(startIndex + 1 - lastIndex)));
            sb.append(newComment);
            lastIndex = endIndex - 1;
          } else if (token[0] == HeaderParserUtility.TokenPhrase) {
            // This is a phrase token
            int startIndex = token[1];
            endIndex = token[2];
            String newComment = Rfc2047.DecodePhraseText(
              str,
              startIndex,
              endIndex,
              tokens,
              true);
            sb.append(str.substring(lastIndex, (lastIndex)+(startIndex - lastIndex)));
            sb.append(newComment);
            lastIndex = endIndex;
          }
        }
        sb.append(str.substring(lastIndex, (lastIndex)+(str.length() - lastIndex)));
        return sb.toString();
      }

      public boolean IsStructured() {
        return true;
      }
    }

    private static final class HeaderReceived extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }
      private static String TrimFWSFromRight(String str) {
        if (((str) == null || (str).length() == 0)) {
          return str;
        }
        int index = 0;
        int valueSLength = str.length();
        int indexStart = index;
        index = str.length() - 1;
        while (index >= 0) {
          char c = str.charAt(index);
          if (c == 0x0a && index >= 1 && str.charAt(index - 1) == 0x0d) {
            // CRLF
            index -= 2;
          } else if (c == 0x09 || c == 0x20) {
            --index;
          } else {
            int indexEnd = index + 1;
            if (indexEnd == indexStart) {
              return "";
            }
            return (indexEnd == str.length() && indexStart == 0) ? str :
              str.substring(indexStart, (indexStart)+(indexEnd - indexStart));
          }
        }
        return "";
      }

      private static boolean IsCFWSWordCFWS(
        String str,
        int index,
        int endIndex,
        String word) {
        int si;
        StringBuilder sb = new StringBuilder();
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        si = HeaderParserUtility.ParseWord(str, index, endIndex, sb);
        if (si == index) {
          return false;
        }
        String checkword = DataUtilities.ToLowerCaseAscii(sb.toString());
        if (!checkword.equals(word)) {
          return false;
        }
        index = HeaderParser.ParseCFWS(
          str,
          si,
          endIndex,
          null);
        return index == endIndex;
      }

      @Override public String DowngradeHeaderField(String name, String str) {
        // The 2 below is for the colon and space after the header field name
        HeaderEncoder enc = new HeaderEncoder(
          Message.MaxRecHeaderLineLength,
          name.length() + 2);
        str = HeaderEncoder.TrimLeadingFWS(str);
        str = StructuredHeaderField.DowngradeHeaderFieldValue(
          enc,
          this,
          str);
        if (this.Parse(str, 0, str.length(), null) != str.length()) {
          // The header field is syntactically invalid,
          // so downgrading is not possible
          return str;
        }
        String header = str;
        int index = 0;
        boolean changed = false;
        StringBuilder sb = new StringBuilder();
        int tokenEnd = HeaderParser.ParseCFWS(header, 0, header.length(), null);
        while (index < header.length()) {
          int newindex = HeaderParser.ParseReceivedToken(
            header,
            index,
            header.length(),
            null);
          if (newindex == index) {
            tokenEnd = HeaderParser.ParseCFWS(
              header,
              index,
              header.length(),
              null);
            sb.append(header.substring(index, (index)+(tokenEnd - index)));
            break;
          }
          if (IsCFWSWordCFWS(header, index, newindex, "for")) {
            Tokener tokener = new Tokener();
            int clauseEnd = HeaderParser.ParseReceivedToken(
              header,
              newindex,
              header.length(),
              tokener);
            List<int[]> tokens = tokener.GetTokens();
            boolean notGoodLocalPart = false;
            for (int[] token : tokens) {
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
              sb.append(header.substring(index, (index)+(clauseEnd - index)));
            }
            index = clauseEnd;
          } else if (IsCFWSWordCFWS(header, index, newindex, "id")) {
            int clauseEnd = HeaderParser.ParseReceivedToken(
              header,
              newindex,
              header.length(),
              null);
            if (Message.HasTextToEscape(header, index, clauseEnd)) {
              changed = true;
            } else {
              sb.append(header.substring(index, (index)+(clauseEnd - index)));
            }
            index = clauseEnd;
          } else {
            sb.append(header.substring(index, (index)+(newindex - index)));
            index = newindex;
          }
        }
        String receivedPart = sb.toString();
        String datePart = header.substring(tokenEnd);
        if (changed) {
          receivedPart = TrimFWSFromRight(receivedPart);
          return new HeaderEncoder().AppendFieldName(name)
            .AppendString(receivedPart + datePart).toString();
          } else {
          return new HeaderEncoder().AppendFieldName(name).toString() +
            str;
        }
      }
    }

    private static final class HeaderContentDisposition extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        String s = str.substring(index, (index)+(endIndex - index));
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

    static String DowngradeListHeaderIfNo(HeaderEncoder enc, String
      str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      int index = HeaderParser.ParseCFWS(str, 0, str.length(), null);
      int noIndex = index;
      if (index + 1 < str.length() && str.charAt(index) == 'N' && str.charAt(index + 1) ==
        'O') {
        index = HeaderParser.ParseCFWS(str, index + 2, str.length(), null);
        if (index != str.length()) {
          return null;
        }
        String sstr = str.substring(0, noIndex);
        DowngradeCFWS(enc, sstr, false);
        enc.AppendSymbol("NO");
        noIndex += 2;
        sstr = str.substring(noIndex, (noIndex)+(str.length() - noIndex));
        DowngradeCFWS(enc, sstr, false);
        return enc.toString();
      } else {
        return null;
      }
    }

    static String DowngradeListHeader(HeaderEncoder enc, String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (str.length() == 0) {
        return str;
      }
      ArrayList<String> list = new ArrayList<String>();
      StringBuilder sb = new StringBuilder();
      int index = 0;
      int index2 = HeaderParser.ParseCFWS(str, index, str.length(), null);
      String sstr = str.substring(index, (index)+(index2 - index));
      DowngradeCFWS(enc, sstr, false);
      index = index2;
      while (index < str.length()) {
        if (index >= str.length() || str.charAt(index) != '<') {
          // Downgrade rest of String
          sstr = str.substring(index, (index)+(str.length() - index));
          DowngradeCFWS(enc, sstr, true);
          return enc.toString();
        }
        boolean found = false;
        enc.AppendSymbol("<");
        ++index;
        while (index < str.length()) {
          index = HeaderParser.ParseFWS(str, index, str.length(), null);
          int c = str.charAt(index);
          if ((c & 0xfc00) == 0xd800 && index + 1 < str.length() && (c &
              0xfc00) == 0xdc00) {
            sstr = str.substring(index, (index)+(2));
            enc.AppendSymbol(sstr);
            index += 2;
          } else if ((c & 0xf800) == 0xd800) {
            // Downgrade rest of String
            sstr = str.substring(index, (index)+(str.length() - index));
            DowngradeCFWS(enc, sstr, true);
            return enc.toString();
          } else if (c == 0x3e) {
            // Right angle bracket '>'
            String uri = sb.toString();
            enc.AppendSymbol(">");
            ++index;
            if (!URIUtility.IsValidIRI(uri)) {
              // Downgrade rest of String
              sstr = str.substring(index, (index)+(str.length() - index));
              DowngradeCFWS(enc, sstr, true);
              return enc.toString();
            }
            list.add(uri);
            found = true;
            break;
          } else {
            sstr = str.substring(index, (index)+(1));
            enc.AppendSymbol(sstr);
            ++index;
          }
        }
        if (!found) {
          break;
        }
        index2 = HeaderParser.ParseCFWS(str, index, str.length(), null);
        sstr = str.substring(index, (index)+(index2 - index));
        DowngradeCFWS(enc, sstr, false);
        index = index2;
        if (index >= str.length() || str.charAt(index) != ',') {
          sstr = str.substring(index, (index)+(str.length() - index));
          DowngradeCFWS(enc, sstr, true);
          return enc.toString();
        }
        enc.AppendSymbol(",");
        index2 = HeaderParser.ParseCFWS(str, index + 1, str.length(), null);
        sstr = str.substring(index, (index)+(index2 - index));
        DowngradeCFWS(enc, sstr, false);
        index = index2;
      }
      return enc.toString();
    }

    static void DowngradeCFWS(
      HeaderEncoder enc,
      String str,
      boolean multiple) {
      if (str.indexOf('(') < 0 ||
        !Message.HasTextToEscapeOrEncodedWordStarts(str)) {
        // Contains no comments, or no text needs to be encoded
        enc.AppendString(str, 0, str.length());
        return;
      }
      Tokener tokener = new Tokener();
      int endIndex;
      if (multiple) {
        endIndex = str.length();
        HeaderParserUtility.TraverseCFWSAndQuotedStrings(
          str,
          0,
          str.length(),
          tokener);
      } else {
        endIndex = HeaderParser.ParseCFWS(str, 0, str.length(), tokener);
      }
      if (endIndex != str.length()) {
        // The CFWS is syntactically invalid,
        // so downgrading is not possible
        enc.AppendString(str, 0, str.length());
        return;
      }
      int lastIndex = 0;
      // Get each relevant token sorted by starting index
      List<int[]> tokens = tokener.GetTokens();
      enc.Reset();
      for (int[] token : tokens) {
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
      enc.AppendString(str, lastIndex, str.length());
    }

    // Determines whether a List-Post field value contains "NO" in that
    // combination of case, and optional CFWS before or after the "NO"
    static boolean IsListPostNo(String str, int index, int endIndex) {
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index + 1 < endIndex && str.charAt(index) == 'N' && str.charAt(index + 1) == 'O') {
        index = HeaderParser.ParseCFWS(str, index + 2, endIndex, null);
        return index == endIndex;
      }
      return false;
    }

    // NOTE: See RFC 2369 section 2
    // NOTE: Applies to List-Help, List-Archive, List-Unsubscribe,
    // List-Owner, List-Subscribe, List-Post
    static String[] GetListHeaderUris(
      String str,
      int index,
      int endIndex,
      ITokener tokener) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      ArrayList<String> list = new ArrayList<String>();
      index = HeaderParser.ParseCFWS(str, index, endIndex, tokener);
      while (index < endIndex) {
        if (index >= endIndex || str.charAt(index) != '<') {
          break;
        }
        ++index;
        boolean found = false;
        StringBuilder sb = new StringBuilder();
        while (index < endIndex) {
          index = HeaderParser.ParseFWS(str, index, endIndex, tokener);
          int c = str.charAt(index);
          if ((c & 0xfc00) == 0xd800 && index + 1 < endIndex &&
            (c & 0xfc00) == 0xdc00) {
            sb.append(str.charAt(index));
            sb.append(str.charAt(index + 1));
            index += 2;
          } else if ((c & 0xf800) == 0xd800) {
            break;
          }
          if (c == 0x3e) {
            // Right angle bracket '>'
            String uri = sb.toString();
            if (!URIUtility.IsValidIRI(uri)) {
              break;
            }
            list.add(uri);
            ++index;
            found = true;
            break;
          } else {
            sb.append(str.charAt(index));
            ++index;
          }
        }
        if (!found) {
          break;
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, tokener);
        if (index >= endIndex || str.charAt(index) != ',') {
          break;
        }
        index = HeaderParser.ParseCFWS(str, index + 1, endIndex, tokener);
      }
      return list.toArray(new String[] { });
    }

    // Applies to List-Help, List-Archive, List-Unsubscribe,
    // List-Owner, List-Subscribe, List-Post
    private static final class HeaderListRfc2369 extends StructuredHeaderField {
      @Override public String DowngradeHeaderField(String name, String str) {
        // The 2 below is for the colon and space after the header field name
        // NOTE: May introduce spaces within URLs; even though message
        // transport agents (MTAs) must not introduce such spaces, this is not
        // a deviation from RFC 2369, however, because this method is not
        // being called in the context of sending already generated messages,
        // where MTAs are generally not allowed to modify the messages
        // they send (especially to add or remove whitespace in header
        // field values) except to add certain header fields at the top of
        // the message.
        // System.out.println("before = "+str);
        String lcname = DataUtilities.ToLowerCaseAscii(name);
        HeaderEncoder enc = new HeaderEncoder(
          Message.MaxRecHeaderLineLength,
          name.length() + 2);
        str = HeaderEncoder.TrimLeadingFWS(str);
        if (lcname.equals("list-post")) {
          String s2 = DowngradeListHeaderIfNo(enc, str);
          str = (s2 == null) ? (HeaderFieldParsers.DowngradeListHeader(enc, str)) : s2;
        } else {
          str = HeaderFieldParsers.DowngradeListHeader(enc, str);
        }
        // System.out.println("after = "+str);
        return new HeaderEncoder().AppendFieldName(name) + str;
      }

      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        HeaderParserUtility.TraverseCFWSAndQuotedStrings(
          str,
          index,
          endIndex,
          tokener);
        return endIndex;
      }
    }

    private static final class HeaderContentType extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        String s = str.substring(index, (index)+(endIndex - index));
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

    private static final class HeaderAutoSubmitted extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        // NOTE: Same syntax as Content-Disposition
        String s = str.substring(index, (index)+(endIndex - index));
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

    private static final class HeaderSioLabel extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        int si = index;
        si = HeaderParser.ParseFWS(str, si, endIndex, tokener);
        HashMap<String, String> parameters = new HashMap<String, String>();
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

    private static final class HeaderArchive extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        // NOTE: Almost the same syntax as Content-Disposition, except
        // first character must be a space (since this is a Netnews header
        // field), and a limited selection of "disposition types" is valid;
        // however, the initial space is not checked here, a behavior
        // allowed by RFC 5536 sec. 2.2
        String s = str.substring(index, (index)+(endIndex - index));
        ContentDisposition cd = ContentDisposition.Parse(s, null);
        if (cd == null) {
          return index;
        }
        int ret = (cd.getDispositionType().equals("no") ||
            cd.getDispositionType().equals("yes")) ? endIndex : index;
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

    private static final class HeaderInjectionInfo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        // NOTE: Under the syntax of InjectionInfo, the
        // first character must be a space (since this is a Netnews header
        // field); however, the initial space is not checked here, a behavior
        // allowed by RFC 5536 sec. 2.2
        int indexStart, indexTemp, state, tx2;
        indexStart = index;
        state = (tokener != null) ? tokener.GetState() : 0;
        indexTemp = index;
        do {
          index = HeaderParser.ParseCFWS(str, index, endIndex, tokener);
          tx2 = HeaderParser.ParsePathIdentity(str, index, endIndex, tokener);
          if (tx2 == index) {
            index = indexStart;
            break;
          } else {
            index = tx2;
          }
          index = HeaderParser.ParseCFWS(str, index, endIndex, tokener);
          if (index < endIndex && (str.charAt(index) == 59)) {
            ++index;
            HashMap<String, String> parameters = new HashMap<String, String>();
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
    private static final class HeaderX400ContentReturn extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderDeliveryDate extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderPriority extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderPriority(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderImportance extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderSensitivity extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderX400ContentIdentifier extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderX400Received extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderX400MtsIdentifier extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderTlsRequired extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderTlsRequired(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderX400Originator extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderX400Recipients extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderConversion extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderConversionWithLoss extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderSupersedes extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderAutoforwarded extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderContentTranslationType extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderGenerateDeliveryReport extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderIncompleteCopy extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderPreventNondeliveryReport extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderAlternateRecipient extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderDiscloseRecipients extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderExpandedDate extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderNewsgroups extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderPath extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderPath(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderControl extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderControl(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDistribution extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderFollowupTo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderInjectionDate extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderUserAgent extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderUserAgent(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderXref extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderXref(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderNntpPostingHost extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderAcceptLanguage extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderArchivedAt extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderArcAuthenticationResults extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderAuthenticationResults extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderBcc extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderCancelLock extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderCancelKey extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderCancelKey(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderTlsReportDomain extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderTlsReportSubmitter extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderFormSub extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderFormSub(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderXPgpSig extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderXPgpSig(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentBase extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderContentDuration extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderContentId extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderContentId(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderContentLanguage extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderContentLocation extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderContentMd5 extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderContentTransferEncoding extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderDate extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderDate(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDeferredDelivery extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderDispositionNotificationOptions extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderDispositionNotificationTo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsAuthorizingUsers extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderDkimSignature extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderArcMessageSignature extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderArcSeal extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderArcSeal(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderEdiintFeatures extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderEesstVersion extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderEncoding extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderEncoding(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderEncrypted extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderEncrypted(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderFrom extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderFrom(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderInReplyTo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderInReplyTo(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderJabberId extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderJabberId(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderKeywords extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderKeywords(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderLanguage extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderLanguage(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderLatestDeliveryTime extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderListId extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderListId(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderListUnsubscribePost extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMessageContext extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMessageId extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderMessageId(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderMimeVersion extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsAcp127MessageIdentifier extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsCodressMessageIndicator extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsCopyPrecedence extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsExemptedAddress extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsExtendedAuthorisationInfo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsHandlingInstructions extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsMessageInstructions extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsMessageType extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsOriginatorPlad extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsOriginatorReference extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsOtherRecipientsIndicatorCc extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsOtherRecipientsIndicatorTo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsPrimaryPrecedence extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMmhsSubjectIndicatorCodes extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderMtPriority extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderObsoletes extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderObsoletes(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderOriginalRecipient extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderReceivedSpf extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderRequireRecipientValidSince extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderResentTo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderResentTo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderReturnPath extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderSender extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderSender(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderSolicitation extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderTo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderTo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderVbrInfo extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderVbrInfo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderXArchivedAt extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderXRicevuta extends StructuredHeaderField {
      @Override public int Parse(
        String str,
        int index,
        int endIndex,
        ITokener tokener) {
        return HeaderParser.ParseHeaderXRicevuta(
          str,
          index,
          endIndex,
          tokener);
      }
    }

    private static final class HeaderXTiporicevuta extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderXTrasporto extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final class HeaderXVerificasicurezza extends StructuredHeaderField {
      @Override public int Parse(
        String str,
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

    private static final IHeaderFieldParser Unstructured = new
    UnstructuredHeaderField();

    private static final Map<String, IHeaderFieldParser> FieldMap =
      CreateHeaderFieldList();

    private static Map<String, IHeaderFieldParser> CreateHeaderFieldList() {
      // NOTE: Header fields not mentioned here are treated as unstructured
      HashMap<String, IHeaderFieldParser> fieldMap = new HashMap<String, IHeaderFieldParser>();
      fieldMap.put("content-disposition",new HeaderContentDisposition());
      fieldMap.put("content-type",new HeaderContentType());
      fieldMap.put("auto-submitted",new HeaderAutoSubmitted());
      fieldMap.put("archive",new HeaderArchive());
      fieldMap.put("autosubmitted",new HeaderAutoforwarded()); // same syntax
      fieldMap.put("list-archive",new HeaderListRfc2369());
      fieldMap.put("list-help",new HeaderListRfc2369());
      fieldMap.put("list-post",new HeaderListRfc2369());
      fieldMap.put("list-owner",new HeaderListRfc2369());
      fieldMap.put("list-subscribe",new HeaderListRfc2369());
      fieldMap.put("list-unsubscribe",new HeaderListRfc2369());
      fieldMap.put("sio-label-history",new HeaderSioLabel());
      fieldMap.put("injection-info",new HeaderInjectionInfo());
      //------------------ generic ------------------
      fieldMap.put("tls-required",new HeaderTlsRequired());
      fieldMap.put("content-return",new HeaderX400ContentReturn());
      fieldMap.put("x400-content-return",new HeaderX400ContentReturn());
      fieldMap.put("delivery-date",new HeaderDeliveryDate());
      fieldMap.put("priority",new HeaderPriority());
      fieldMap.put("importance",new HeaderImportance());
      fieldMap.put("sensitivity",new HeaderSensitivity());
      fieldMap.put("reply-by",new HeaderDate());
      fieldMap.put("x400-content-identifier",new HeaderX400ContentIdentifier());
      fieldMap.put("x400-received",new HeaderX400Received());
      fieldMap.put("x400-mts-identifier",new HeaderX400MtsIdentifier());
      fieldMap.put("x400-trace",new HeaderX400Received());
      fieldMap.put("x400-originator",new HeaderX400Originator());
      fieldMap.put("x400-recipients",new HeaderX400Recipients());
      fieldMap.put("conversion",new HeaderConversion());
      fieldMap.put("conversion-with-loss",new HeaderConversionWithLoss());
      fieldMap.put("supersedes",new HeaderSupersedes());
      fieldMap.put("expires",new HeaderDate());
      fieldMap.put("autoforwarded",new HeaderAutoforwarded());
      fieldMap.put("content-translation-type",new HeaderContentTranslationType());
      fieldMap.put("generate-delivery-report",new HeaderGenerateDeliveryReport());
      fieldMap.put("incomplete-copy",new HeaderIncompleteCopy());
      fieldMap.put("prevent-nondelivery-report",new
      HeaderPreventNondeliveryReport());
      fieldMap.put("alternate-recipient",new HeaderAlternateRecipient());
      fieldMap.put("disclose-recipients",new HeaderDiscloseRecipients());
      fieldMap.put("expanded-date",new HeaderExpandedDate());
      fieldMap.put("newsgroups",new HeaderNewsgroups());
      fieldMap.put("path",new HeaderPath());
      fieldMap.put("control",new HeaderControl());
      fieldMap.put("distribution",new HeaderDistribution());
      fieldMap.put("followup-to",new HeaderFollowupTo());
      fieldMap.put("injection-date",new HeaderInjectionDate());
      fieldMap.put("user-agent",new HeaderUserAgent());
      fieldMap.put("xref",new HeaderXref());
      fieldMap.put("nntp-posting-date",new HeaderInjectionDate());
      fieldMap.put("nntp-posting-host",new HeaderNntpPostingHost());
      fieldMap.put("accept-language",new HeaderAcceptLanguage());
      fieldMap.put("archived-at",new HeaderArchivedAt());
      fieldMap.put("arc-authentication-results",new
      HeaderArcAuthenticationResults());
      fieldMap.put("authentication-results",new HeaderAuthenticationResults());
      fieldMap.put("base",new HeaderContentBase());
      fieldMap.put("bcc",new HeaderBcc());
      fieldMap.put("cc",new HeaderTo());
      fieldMap.put("cancel-lock",new HeaderCancelLock());
      fieldMap.put("cancel-key",new HeaderCancelKey());
      fieldMap.put("tls-report-domain",new HeaderTlsReportDomain());
      fieldMap.put("tls-report-submitter",new HeaderTlsReportSubmitter());
      fieldMap.put("form-sub",new HeaderFormSub());
      fieldMap.put("x-pgp-sig",new HeaderXPgpSig());
      fieldMap.put("content-base",new HeaderContentBase());
      fieldMap.put("content-duration",new HeaderContentDuration());
      fieldMap.put("content-id",new HeaderContentId());
      fieldMap.put("content-language",new HeaderContentLanguage());
      fieldMap.put("content-location",new HeaderContentLocation());
      fieldMap.put("content-md5",new HeaderContentMd5());
      fieldMap.put("content-transfer-encoding",new
      HeaderContentTransferEncoding());
      fieldMap.put("date",new HeaderDate());
      fieldMap.put("deferred-delivery",new HeaderDeferredDelivery());
      fieldMap.put("disposition-notification-options",new
      HeaderDispositionNotificationOptions());
      fieldMap.put("disposition-notification-to",new
      HeaderDispositionNotificationTo());
      fieldMap.put("mmhs-authorizing-users",new HeaderMmhsAuthorizingUsers());
      fieldMap.put("dkim-signature",new HeaderDkimSignature());
      fieldMap.put("arc-message-signature",new HeaderArcMessageSignature());
      fieldMap.put("arc-seal",new HeaderArcSeal());
      fieldMap.put("ediint-features",new HeaderEdiintFeatures());
      fieldMap.put("eesst-version",new HeaderEesstVersion());
      fieldMap.put("encoding",new HeaderEncoding());
      fieldMap.put("encrypted",new HeaderEncrypted());
      fieldMap.put("expiry-date",new HeaderDate());
      fieldMap.put("from",new HeaderFrom());
      fieldMap.put("in-reply-to",new HeaderInReplyTo());
      fieldMap.put("jabber-id",new HeaderJabberId());
      fieldMap.put("keywords",new HeaderKeywords());
      fieldMap.put("language",new HeaderLanguage());
      fieldMap.put("latest-delivery-time",new HeaderLatestDeliveryTime());
      fieldMap.put("list-id",new HeaderListId());
      fieldMap.put("list-unsubscribe-post",new HeaderListUnsubscribePost());
      fieldMap.put("message-context",new HeaderMessageContext());
      fieldMap.put("message-id",new HeaderMessageId());
      fieldMap.put("mime-version",new HeaderMimeVersion());
      fieldMap.put("mmhs-acp127-message-identifier",new
      HeaderMmhsAcp127MessageIdentifier());
      fieldMap.put("mmhs-codress-message-indicator",new
      HeaderMmhsCodressMessageIndicator());
      fieldMap.put("mmhs-copy-precedence",new HeaderMmhsCopyPrecedence());
      fieldMap.put("mmhs-exempted-address",new HeaderMmhsExemptedAddress());
      fieldMap.put("mmhs-extended-authorisation-info",new
      HeaderMmhsExtendedAuthorisationInfo());
      fieldMap.put("mmhs-handling-instructions",new
      HeaderMmhsHandlingInstructions());
      fieldMap.put("mmhs-message-instructions",new
      HeaderMmhsMessageInstructions());
      fieldMap.put("mmhs-message-type",new HeaderMmhsMessageType());
      fieldMap.put("mmhs-originator-plad",new HeaderMmhsOriginatorPlad());
      fieldMap.put("mmhs-originator-reference",new
      HeaderMmhsOriginatorReference());
      fieldMap.put("mmhs-other-recipients-indicator-cc",new
      HeaderMmhsOtherRecipientsIndicatorCc());
      fieldMap.put("mmhs-other-recipients-indicator-to",new
      HeaderMmhsOtherRecipientsIndicatorTo());
      fieldMap.put("mmhs-primary-precedence",new HeaderMmhsPrimaryPrecedence());
      fieldMap.put("mmhs-subject-indicator-codes",new
      HeaderMmhsSubjectIndicatorCodes());
      fieldMap.put("mt-priority",new HeaderMtPriority());
      fieldMap.put("obsoletes",new HeaderObsoletes());
      fieldMap.put("original-from",new HeaderFrom());
      fieldMap.put("original-message-id",new HeaderMessageId());
      fieldMap.put("original-recipient",new HeaderOriginalRecipient());
      fieldMap.put("received",new HeaderReceived());
      fieldMap.put("received-spf",new HeaderReceivedSpf());
      fieldMap.put("references",new HeaderInReplyTo());
      fieldMap.put("reply-to",new HeaderResentTo());
      fieldMap.put("require-recipient-valid-since",new
      HeaderRequireRecipientValidSince());
      fieldMap.put("resent-bcc",new HeaderBcc());
      fieldMap.put("resent-cc",new HeaderResentTo());
      fieldMap.put("resent-date",new HeaderDate());
      fieldMap.put("resent-from",new HeaderFrom());
      fieldMap.put("resent-message-id",new HeaderMessageId());
      fieldMap.put("resent-reply-to",new HeaderResentTo());
      fieldMap.put("resent-sender",new HeaderSender());
      fieldMap.put("resent-to",new HeaderResentTo());
      fieldMap.put("return-path",new HeaderReturnPath());
      fieldMap.put("sender",new HeaderSender());
      fieldMap.put("solicitation",new HeaderSolicitation());
      fieldMap.put("to",new HeaderTo());
      fieldMap.put("vbr-info",new HeaderVbrInfo());
      fieldMap.put("x-archived-at",new HeaderXArchivedAt());
      fieldMap.put("x-mittente",new HeaderSender());
      fieldMap.put("x-ricevuta",new HeaderXRicevuta());
      fieldMap.put("x-riferimento-message-id",new HeaderMessageId());
      fieldMap.put("x-tiporicevuta",new HeaderXTiporicevuta());
      fieldMap.put("x-trasporto",new HeaderXTrasporto());
      fieldMap.put("x-verificasicurezza",new HeaderXVerificasicurezza());
      return fieldMap;
    }

    public static IHeaderFieldParser GetParser(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      return FieldMap.containsKey(name) ? FieldMap.get(name) : Unstructured;
    }
  }
