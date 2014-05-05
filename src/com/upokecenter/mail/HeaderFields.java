package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

  final class HeaderFields {
private HeaderFields() {
}
    private static final class UnstructuredHeaderField implements IHeaderFieldParser {
    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String DowngradeFieldValue(String str) {
        return Rfc2047.EncodeString(str);
      }

    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String DecodeEncodedWords(String str) {
        // For unstructured header fields.
        return Rfc2047.DecodeEncodedWords(str, 0, str.length(), EncodedWordContext.Unstructured);
      }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
      public boolean IsStructured() {
        return false;
      }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return endIndex;
      }
    }

    private static abstract class StructuredHeaderField implements IHeaderFieldParser {
      public abstract int Parse(String str, int index, int endIndex, ITokener tokener);

      private List<String> ParseGroupLists(String str, int index, int endIndex) {
        ArrayList<String> groups=new ArrayList<String>();
        Tokener tokener = new Tokener();
        this.Parse(str, index, endIndex, tokener);
        for(int[] token : tokener.GetTokens()) {
          if (token[0] == HeaderParserUtility.TokenGroup) {
            int startIndex = token[1];
            endIndex = token[2];
            String groupList = HeaderParserUtility.ParseGroupList(str, startIndex, endIndex);
            groupList = ParserUtility.TrimSpaceAndTab(groupList);
            groups.add(groupList);
          }
        }
        return groups;
      }

    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String DowngradeFieldValue(String str) {
        String originalString = str;
        List<String> originalGroups = null;
        for (int phase = 0; phase < 5; ++phase) {
          if (str.indexOf('(') < 0 && phase == 0) {
            // No comments in the header field value, a common case
            continue;
          }
          if (!Message.HasTextToEscape(str)) {
            // No text needs to be encoded
            return str;
          }
          StringBuilder sb = new StringBuilder();
          Tokener tokener = new Tokener();
          int endIndex = this.Parse(str, 0, str.length(), tokener);
          if (endIndex != str.length()) {
            // The header field is syntactically invalid,
            // so downgrading is not possible
            return str;
          }
          int lastIndex = 0;
          // Get each relevant token sorted by starting index
          List<int[]> tokens = tokener.GetTokens();
          int groupIndex = 0;
          for(int[] token : tokens) {
            if (token[1] < lastIndex) {
              continue;
            }
            if (phase == -1) {  // ID-Left and ID-right
              // TODO: Don't downgrade if extended characters appear
              // in ID-Left or ID-right (doesn't apply to the Received header
              // field)
            }
            if (phase == 0) {  // Comment downgrading
              if (token[0] == HeaderParserUtility.TokenComment) {
                int startIndex = token[1];
                endIndex = token[2];
                // System.out.println(str.substring(startIndex,(startIndex)+(endIndex - startIndex)));
                if (Message.HasTextToEscape(str, startIndex, endIndex)) {
                  String newComment = Rfc2047.EncodeComment(str, startIndex, endIndex);
                  sb.append(str.substring(lastIndex,(lastIndex)+(startIndex - lastIndex)));
                  sb.append(newComment);
                } else {
                  // No text needs to be escaped, output the comment as is
                  sb.append(str.substring(lastIndex,(lastIndex)+(endIndex - lastIndex)));
                }
                lastIndex = endIndex;
              }
            } else if (phase == 1) {  // Phrase downgrading
              if (token[0] == HeaderParserUtility.TokenPhrase) {
                int startIndex = token[1];
                endIndex = token[2];
                String newComment = Rfc2047.EncodePhraseText(str, startIndex, endIndex, tokens);
                sb.append(str.substring(lastIndex,(lastIndex)+(startIndex - lastIndex)));
                sb.append(newComment);
                lastIndex = endIndex;
              }
            } else if (phase == 2) {  // Group downgrading
              if (token[0] == HeaderParserUtility.TokenGroup) {
                int startIndex = token[1];
                endIndex = token[2];
                boolean nonasciiLocalParts = false;
                int displayNameEnd = -1;
                String originalGroupList;
                for(int[] token2 : tokens) {
                  if (token2[0] == HeaderParserUtility.TokenPhrase) {
                    if (displayNameEnd < 0) {
                      displayNameEnd = token2[2];
                    }
                  }
                  if (token2[0] == HeaderParserUtility.TokenLocalPart) {
                    if (token2[1] >= startIndex && token2[2] <= endIndex) {
                      // Local part within a group
                      if (Message.HasTextToEscapeIgnoreEncodedWords(str, token2[1], token2[2])) {
                        nonasciiLocalParts = true;
                        break;
                      }
                    }
                  }
                }
                if (nonasciiLocalParts) {
                  if (originalGroups == null) {
                    originalGroups = this.ParseGroupLists(originalString, 0, originalString.length());
                  }
                  originalGroupList = originalGroups.get(groupIndex);
                  String groupText = originalGroupList;
                  String displayNameText = str.substring(startIndex,(startIndex)+(displayNameEnd - startIndex));
                  String encodedText = displayNameText + " " + Rfc2047.EncodeString(groupText) + " :;";
                  sb.append(str.substring(lastIndex,(lastIndex)+(startIndex - lastIndex)));
                  sb.append(encodedText);
                  lastIndex = endIndex;
                  ++groupIndex;
                } else {
                  int localLastIndex = startIndex;
                  boolean nonasciiDomains = false;
                  StringBuilder sb2 = new StringBuilder();
                  for(int[] token2 : tokens) {
                    if (token2[0] == HeaderParserUtility.TokenDomain) {
                      if (token2[1] >= startIndex && token2[2] <= endIndex) {
                        // Domain within the group
                        String domain = HeaderParserUtility.ParseDomain(str, token2[1], token[2]);
                        // NOTE: "domain" can include domain literals, enclosed
                        // in brackets; they are invalid under "IsValidDomainName".
                        if (Message.HasTextToEscapeIgnoreEncodedWords(domain, 0, domain.length()) &&
                            Idna.IsValidDomainName(domain, false)) {
                          domain = Idna.EncodeDomainName(domain);
                        } else {
                          domain = str.substring(token2[1],(token2[1])+(token2[2] - token2[1]));
                        }
                        if (Message.HasTextToEscapeIgnoreEncodedWords(domain, 0, domain.length())) {
                          // ASCII encoding failed
                          nonasciiDomains = true;
                          break;
                        }
                        sb2.append(str.substring(localLastIndex,(localLastIndex)+(token2[1] - localLastIndex)));
                        sb2.append(domain);
                        localLastIndex = token2[2];
                      }
                    }
                  }
                  if (nonasciiDomains) {
                    // At least some of the domains could not
                    // be converted to ASCII
                    if (originalGroups == null) {
                      originalGroups = this.ParseGroupLists(originalString, 0, originalString.length());
                    }
                    originalGroupList = originalGroups.get(groupIndex);
                    String groupText = originalGroupList;
                    String displayNameText = str.substring(startIndex,(startIndex)+(displayNameEnd - startIndex));
                    String encodedText = displayNameText + " " + Rfc2047.EncodeString(groupText) + " :;";
                    sb.append(str.substring(lastIndex,(lastIndex)+(startIndex - lastIndex)));
                    sb.append(encodedText);
                    lastIndex = endIndex;
                  } else {
                    // All of the domains could be converted to ASCII
                    sb2.append(str.substring(localLastIndex,(localLastIndex)+(endIndex - localLastIndex)));
                    sb.append(str.substring(lastIndex,(lastIndex)+(startIndex - lastIndex)));
                    sb.append(sb2.toString());
                    lastIndex = endIndex;
                  }
                  ++groupIndex;
                }
              }
            } else if (phase == 3) {  // Mailbox downgrading
              if (token[0] == HeaderParserUtility.TokenGroup) {
                int startIndex = token[1];
                endIndex = token[2];
                boolean nonasciiLocalPart = false;
                boolean hasPhrase = false;
                for(int[] token2 : tokens) {
                  if (token2[0] == HeaderParserUtility.TokenPhrase) {
                    hasPhrase = true;
                  }
                  if (token2[0] == HeaderParserUtility.TokenLocalPart) {
                    if (token2[1] >= startIndex && token2[2] <= endIndex) {
                      if (Message.HasTextToEscapeIgnoreEncodedWords(str, token2[1], token2[2])) {
                        nonasciiLocalPart = true;
                        break;
                      }
                    }
                  }
                }
                if (nonasciiLocalPart) {
                  sb.append(str.substring(lastIndex,(lastIndex)+(startIndex - lastIndex)));
                  if (!hasPhrase) {
                    String addrSpec = str.substring(token[1],(token[1])+(token[2] - token[1]));
                    String encodedText = " " + Rfc2047.EncodeString(addrSpec) + " :;";
                    sb.append(encodedText);
                  } else {
                    // Has a phrase, extract the addr-spec and convert
                    // the mailbox to a group
                    int angleAddrStart = HeaderParser.ParsePhrase(str, token[1], token[2], null);
                    // append the rest of the String so far up to and including the phrase
                    sb.append(str.substring(lastIndex,(lastIndex)+(angleAddrStart - lastIndex)));
                    int addrSpecStart = HeaderParser.ParseCFWS(str, angleAddrStart, token[2], null);
                    if (addrSpecStart < token[2] && str.charAt(addrSpecStart) == '<') {
                      ++addrSpecStart;
                    }
                    addrSpecStart = HeaderParser.ParseObsRoute(str, addrSpecStart, token[2], null);
                    int addrSpecEnd = HeaderParser.ParseAddrSpec(str, addrSpecStart, token[2], null);
                    String addrSpec = str.substring(addrSpecStart,(addrSpecStart)+(addrSpecEnd - addrSpecStart));
                    boolean endsWithSpace = sb.length() > 0 && (sb.charAt(sb.length() - 1) == 0x20 || sb.charAt(sb.length() - 1) == 0x09);
                    String encodedText = (endsWithSpace ? "" : " ") +
                      Rfc2047.EncodeString(addrSpec) + " :;";
                    sb.append(encodedText);
                  }
                  lastIndex = endIndex;
                  ++groupIndex;
                } else {
                  // TODO: Downgrade domains within the group
                  ++groupIndex;
                }
              }
            }
          }
          sb.append(str.substring(lastIndex,(lastIndex)+(str.length() - lastIndex)));
          str = sb.toString();
        }
        return str;
      }

    /**
     * Not documented yet.
     * @param str A string object. (2).
     * @return A string object.
     */
      public String DecodeEncodedWords(String str) {

        // For structured header fields that allow comments only wherever whitespace
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
          // System.out.println("Invalid syntax: " + this.getClass().getName() + ", " + str);
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        List<int[]> tokens = tokener.GetTokens();
        for(int[] token : tokens) {
          // System.out.println("" + token[0] + " [" + (str.substring(token[1],(token[1])+(token[2]-token[1]))) + "]");
          if (token[0] == HeaderParserUtility.TokenComment && token[0] >= lastIndex) {
            // This is a comment token
            int startIndex = token[1];
            endIndex = token[2];
            String newComment = Rfc2047.DecodeEncodedWords(
              str,
              startIndex + 1,
              endIndex - 1,
              EncodedWordContext.Comment);
            sb.append(str.substring(lastIndex,(lastIndex)+(startIndex + 1 - lastIndex)));
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
            sb.append(str.substring(lastIndex,(lastIndex)+(startIndex - lastIndex)));
            sb.append(newComment);
            lastIndex = endIndex;
          }
        }
        sb.append(str.substring(lastIndex,(lastIndex)+(str.length() - lastIndex)));
        return sb.toString();
      }

    /**
     * Not documented yet.
     * @return A Boolean object.
     */
      public boolean IsStructured() {
        return true;
      }
    }

    private static final class HeaderX400ContentReturn extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400ContentReturn(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDeliveryDate extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDeliveryDate(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderPriority extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderPriority(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderImportance extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderImportance(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderSensitivity extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSensitivity(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderX400ContentIdentifier extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400ContentIdentifier(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderX400Received extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400Received(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderX400MtsIdentifier extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400MtsIdentifier(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderX400Originator extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400Originator(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderX400Recipients extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400Recipients(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderConversion extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderConversion(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderConversionWithLoss extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderConversionWithLoss(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderAutoforwarded extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutoforwarded(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderGenerateDeliveryReport extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderGenerateDeliveryReport(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderIncompleteCopy extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderIncompleteCopy(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderAutosubmitted extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutosubmitted(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderPreventNondeliveryReport extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderPreventNondeliveryReport(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderAlternateRecipient extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAlternateRecipient(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDiscloseRecipients extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDiscloseRecipients(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderAcceptLanguage extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAcceptLanguage(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderArchivedAt extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderArchivedAt(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderAuthenticationResults extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAuthenticationResults(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderAutoSubmitted extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutoSubmitted(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderBcc extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentBase extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentBase(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentDisposition extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentDisposition(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentDuration extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentDuration(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentId extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentId(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentLanguage extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentLanguage(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentLocation extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentLocation(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentMd5 extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentMd5(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentTransferEncoding extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentTransferEncoding(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderContentType extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentType(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDate extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDate(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDeferredDelivery extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDeferredDelivery(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDispositionNotificationOptions extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationOptions(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDispositionNotificationTo extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationTo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderDkimSignature extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDkimSignature(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderEdiintFeatures extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEdiintFeatures(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderEncoding extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEncoding(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderEncrypted extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEncrypted(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderFrom extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderFrom(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderInReplyTo extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderInReplyTo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderJabberId extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderJabberId(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderKeywords extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderKeywords(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderLanguage extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderLanguage(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderLatestDeliveryTime extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderLatestDeliveryTime(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderListId extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListId(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMessageContext extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMessageContext(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMessageId extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMessageId(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMimeVersion extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMimeVersion(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsAcp127MessageIdentifier extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsAcp127MessageIdentifier(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsCodressMessageIndicator extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCodressMessageIndicator(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsCopyPrecedence extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCopyPrecedence(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsExemptedAddress extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExemptedAddress(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsExtendedAuthorisationInfo extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExtendedAuthorisationInfo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsHandlingInstructions extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsHandlingInstructions(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsMessageInstructions extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageInstructions(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsMessageType extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageType(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsOriginatorPlad extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOriginatorPlad(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsOriginatorReference extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOriginatorReference(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsOtherRecipientsIndicatorCc extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOtherRecipientsIndicatorCc(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsOtherRecipientsIndicatorTo extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOtherRecipientsIndicatorTo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsPrimaryPrecedence extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsPrimaryPrecedence(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMmhsSubjectIndicatorCodes extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsSubjectIndicatorCodes(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderMtPriority extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMtPriority(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderObsoletes extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderObsoletes(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderOriginalRecipient extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderOriginalRecipient(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderReceived extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderReceivedSpf extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceivedSpf(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderResentTo extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderResentTo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderReturnPath extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReturnPath(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderSender extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSender(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderSolicitation extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSolicitation(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderTo extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderTo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderVbrInfo extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderVbrInfo(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderXRicevuta extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXRicevuta(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderXTiporicevuta extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXTiporicevuta(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderXTrasporto extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXTrasporto(str, index, endIndex, tokener);
      }
    }

    private static final class HeaderXVerificasicurezza extends StructuredHeaderField {
    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer. (2).
     * @param endIndex A 32-bit signed integer. (3).
     * @param tokener An object that receives parsed tokens.
     * @return A 32-bit signed integer.
     */
      @Override public int Parse(String str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXVerificasicurezza(str, index, endIndex, tokener);
      }
    }

    private static Map<String, IHeaderFieldParser> fieldMap = CreateHeaderFieldList();
    private static IHeaderFieldParser unstructured = new UnstructuredHeaderField();

    private static Map<String, IHeaderFieldParser> CreateHeaderFieldList() {
      fieldMap = new HashMap<String, IHeaderFieldParser>();
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
      fieldMap.put("expires",new HeaderDate());
      fieldMap.put("autoforwarded",new HeaderAutoforwarded());
      fieldMap.put("generate-delivery-report",new HeaderGenerateDeliveryReport());
      fieldMap.put("incomplete-copy",new HeaderIncompleteCopy());
      fieldMap.put("autosubmitted",new HeaderAutosubmitted());
      fieldMap.put("prevent-nondelivery-report",new HeaderPreventNondeliveryReport());
      fieldMap.put("alternate-recipient",new HeaderAlternateRecipient());
      fieldMap.put("disclose-recipients",new HeaderDiscloseRecipients());
      fieldMap.put("accept-language",new HeaderAcceptLanguage());
      fieldMap.put("archived-at",new HeaderArchivedAt());
      fieldMap.put("authentication-results",new HeaderAuthenticationResults());
      fieldMap.put("auto-submitted",new HeaderAutoSubmitted());
      fieldMap.put("base",new HeaderContentBase());
      fieldMap.put("bcc",new HeaderBcc());
      fieldMap.put("cc",new HeaderTo());
      fieldMap.put("content-base",new HeaderContentBase());
      fieldMap.put("content-disposition",new HeaderContentDisposition());
      fieldMap.put("content-duration",new HeaderContentDuration());
      fieldMap.put("content-id",new HeaderContentId());
      fieldMap.put("content-language",new HeaderContentLanguage());
      fieldMap.put("content-location",new HeaderContentLocation());
      fieldMap.put("content-md5",new HeaderContentMd5());
      fieldMap.put("content-transfer-encoding",new HeaderContentTransferEncoding());
      fieldMap.put("content-type",new HeaderContentType());
      fieldMap.put("date",new HeaderDate());
      fieldMap.put("deferred-delivery",new HeaderDeferredDelivery());
      fieldMap.put("disposition-notification-options",new HeaderDispositionNotificationOptions());
      fieldMap.put("disposition-notification-to",new HeaderDispositionNotificationTo());
      fieldMap.put("dkim-signature",new HeaderDkimSignature());
      fieldMap.put("ediint-features",new HeaderEdiintFeatures());
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
      fieldMap.put("message-context",new HeaderMessageContext());
      fieldMap.put("message-id",new HeaderMessageId());
      fieldMap.put("mime-version",new HeaderMimeVersion());
      fieldMap.put("mmhs-acp127-message-identifier",new HeaderMmhsAcp127MessageIdentifier());
      fieldMap.put("mmhs-codress-message-indicator",new HeaderMmhsCodressMessageIndicator());
      fieldMap.put("mmhs-copy-precedence",new HeaderMmhsCopyPrecedence());
      fieldMap.put("mmhs-exempted-address",new HeaderMmhsExemptedAddress());
      fieldMap.put("mmhs-extended-authorisation-info",new HeaderMmhsExtendedAuthorisationInfo());
      fieldMap.put("mmhs-handling-instructions",new HeaderMmhsHandlingInstructions());
      fieldMap.put("mmhs-message-instructions",new HeaderMmhsMessageInstructions());
      fieldMap.put("mmhs-message-type",new HeaderMmhsMessageType());
      fieldMap.put("mmhs-originator-plad",new HeaderMmhsOriginatorPlad());
      fieldMap.put("mmhs-originator-reference",new HeaderMmhsOriginatorReference());
      fieldMap.put("mmhs-other-recipients-indicator-cc",new HeaderMmhsOtherRecipientsIndicatorCc());
      fieldMap.put("mmhs-other-recipients-indicator-to",new HeaderMmhsOtherRecipientsIndicatorTo());
      fieldMap.put("mmhs-primary-precedence",new HeaderMmhsPrimaryPrecedence());
      fieldMap.put("mmhs-subject-indicator-codes",new HeaderMmhsSubjectIndicatorCodes());
      fieldMap.put("mt-priority",new HeaderMtPriority());
      fieldMap.put("obsoletes",new HeaderObsoletes());
      fieldMap.put("original-from",new HeaderFrom());
      fieldMap.put("original-message-id",new HeaderMessageId());
      fieldMap.put("original-recipient",new HeaderOriginalRecipient());
      fieldMap.put("received",new HeaderReceived());
      fieldMap.put("received-spf",new HeaderReceivedSpf());
      fieldMap.put("references",new HeaderInReplyTo());
      fieldMap.put("reply-to",new HeaderResentTo());
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
      fieldMap.put("x-archived-at",new HeaderArchivedAt());
      fieldMap.put("x-mittente",new HeaderSender());
      fieldMap.put("x-ricevuta",new HeaderXRicevuta());
      fieldMap.put("x-riferimento-message-id",new HeaderMessageId());
      fieldMap.put("x-tiporicevuta",new HeaderXTiporicevuta());
      fieldMap.put("x-trasporto",new HeaderXTrasporto());
      fieldMap.put("x-verificasicurezza",new HeaderXVerificasicurezza());
      // These following header fields, defined in the
      // Message Headers registry as of Apr. 3, 2014,
      // are treated as unstructured.
      // fieldMap.put("apparently-to",unstructured);
      // fieldMap.put("body",unstructured);
      // fieldMap.put("comments",unstructured);
      // fieldMap.put("content-description",unstructured);
      // fieldMap.put("downgraded-bcc",unstructured);
      // fieldMap.put("downgraded-cc",unstructured);
      // fieldMap.put("downgraded-disposition-notification-to",unstructured);
      // fieldMap.put("downgraded-final-recipient",unstructured);
      // fieldMap.put("downgraded-from",unstructured);
      // fieldMap.put("downgraded-in-reply-to",unstructured);
      // fieldMap.put("downgraded-mail-from",unstructured);
      // fieldMap.put("downgraded-message-id",unstructured);
      // fieldMap.put("downgraded-original-recipient",unstructured);
      // fieldMap.put("downgraded-rcpt-to",unstructured);
      // fieldMap.put("downgraded-references",unstructured);
      // fieldMap.put("downgraded-reply-to",unstructured);
      // fieldMap.put("downgraded-resent-bcc",unstructured);
      // fieldMap.put("downgraded-resent-cc",unstructured);
      // fieldMap.put("downgraded-resent-from",unstructured);
      // fieldMap.put("downgraded-resent-reply-to",unstructured);
      // fieldMap.put("downgraded-resent-sender",unstructured);
      // fieldMap.put("downgraded-resent-to",unstructured);
      // fieldMap.put("downgraded-return-path",unstructured);
      // fieldMap.put("downgraded-sender",unstructured);
      // fieldMap.put("downgraded-to",unstructured);
      // fieldMap.put("errors-to",unstructured);
      // fieldMap.put("subject",unstructured);
      return fieldMap;
    }

    public static IHeaderFieldParser GetParser(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (fieldMap.containsKey(name)) {
        return fieldMap.get(name);
      }
      return unstructured;
    }
  }
