/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Text;

using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
  internal static class HeaderFields
  {
    private sealed class UnstructuredHeaderField : IHeaderFieldParser {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        return Rfc2047.EncodeString(str);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DecodeEncodedWords(string str) {
        // For unstructured header fields.
        return Rfc2047.DecodeEncodedWords(str, 0, str.Length, EncodedWordContext.Unstructured);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
      public bool IsStructured() {
        return false;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int Parse(string str, int index, int endIndex, ITokener tokener) {
        return endIndex;
      }
    }

    private abstract class StructuredHeaderField : IHeaderFieldParser {
      public abstract int Parse(string str, int index, int endIndex, ITokener tokener);

      private IList<string> ParseGroupLists(string str, int index, int endIndex) {
        var groups = new List<string>();
        Tokener tokener = new Tokener();
        this.Parse(str, index, endIndex, tokener);
        foreach (int[] token in tokener.GetTokens()) {
          if (token[0] == HeaderParserUtility.TokenGroup) {
            int startIndex = token[1];
            endIndex = token[2];
            string groupList = HeaderParserUtility.ParseGroupList(str, startIndex, endIndex);
            groupList = ParserUtility.TrimSpaceAndTab(groupList);
            groups.Add(groupList);
          }
        }
        return groups;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
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
          StringBuilder sb = new StringBuilder();
          Tokener tokener = new Tokener();
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
                // Console.WriteLine(str.Substring(startIndex, endIndex - startIndex));
                if (Message.HasTextToEscape(str, startIndex, endIndex)) {
                  string newComment = Rfc2047.EncodeComment(str, startIndex, endIndex);
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
                string newComment = Rfc2047.EncodePhraseText(str, startIndex, endIndex, tokens);
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
                      if (Message.HasTextToEscapeIgnoreEncodedWords(str, token2[1], token2[2])) {
                        nonasciiLocalParts = true;
                        break;
                      }
                    }
                  }
                }
                if (!nonasciiLocalParts) {
                  int localLastIndex = startIndex;
                  bool nonasciiDomains = false;
                  StringBuilder sb2 = new StringBuilder();
                  foreach (int[] token2 in tokens) {
                    if (token2[0] == HeaderParserUtility.TokenDomain) {
                      if (token2[1] >= startIndex && token2[2] <= endIndex) {
                        // Domain within the group
                        string domain = HeaderParserUtility.ParseDomain(str, token2[1], token[2]);
                        // NOTE: "domain" can include domain literals, enclosed
                        // in brackets; they are invalid under "IsValidDomainName".
                        if (Message.HasTextToEscapeIgnoreEncodedWords(domain, 0, domain.Length) &&
                            Idna.IsValidDomainName(domain, false)) {
                          domain = Idna.EncodeDomainName(domain);
                        } else {
                          domain = str.Substring(token2[1], token2[2] - token2[1]);
                        }
                        if (Message.HasTextToEscapeIgnoreEncodedWords(domain, 0, domain.Length)) {
                          // ASCII encoding failed
                          nonasciiDomains = true;
                          break;
                        }
                        sb2.Append(str.Substring(localLastIndex, token2[1] - localLastIndex));
                        sb2.Append(domain);
                        localLastIndex = token2[2];
                      }
                    }
                  }
                  nonasciiLocalParts = nonasciiDomains;
                  if (!nonasciiLocalParts) {
                    // All of the domains could be converted to ASCII
                    sb2.Append(str.Substring(localLastIndex, endIndex - localLastIndex));
                    sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                    sb.Append(sb2.ToString());
                    lastIndex = endIndex;
                  }
                }
                if (nonasciiLocalParts) {
                  // At least some of the domains could not
                  // be converted to ASCII
                  if (originalGroups == null) {
                    originalGroups = this.ParseGroupLists(originalString, 0, originalString.Length);
                  }
                  originalGroupList = originalGroups[groupIndex];
                  string groupText = originalGroupList;
                  string displayNameText = str.Substring(startIndex, displayNameEnd - startIndex);
                  string encodedText = displayNameText + " " + Rfc2047.EncodeString(groupText) + " :;";
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
                if (!nonasciiLocalPart) {
                  int localLastIndex = startIndex;
                  bool nonasciiDomains = false;
                  StringBuilder sb2 = new StringBuilder();
                  foreach (int[] token2 in tokens) {
                    if (token2[0] == HeaderParserUtility.TokenDomain) {
                      if (token2[1] >= startIndex && token2[2] <= endIndex) {
                        // Domain within the group
                        string domain = HeaderParserUtility.ParseDomain(str, token2[1], token[2]);
                        // NOTE: "domain" can include domain literals, enclosed
                        // in brackets; they are invalid under "IsValidDomainName".
                        if (Message.HasTextToEscapeIgnoreEncodedWords(domain, 0, domain.Length) &&
                            Idna.IsValidDomainName(domain, false)) {
                          domain = Idna.EncodeDomainName(domain);
                        } else {
                          domain = str.Substring(token2[1], token2[2] - token2[1]);
                        }
                        if (Message.HasTextToEscapeIgnoreEncodedWords(domain, 0, domain.Length)) {
                          // ASCII encoding failed
                          nonasciiDomains = true;
                          break;
                        }
                        sb2.Append(str.Substring(localLastIndex, token2[1] - localLastIndex));
                        sb2.Append(domain);
                        localLastIndex = token2[2];
                      }
                    }
                  }
                  nonasciiLocalPart = nonasciiDomains;
                  if (!nonasciiLocalPart) {
                    // All of the domains could be converted to ASCII
                    sb2.Append(str.Substring(localLastIndex, endIndex - localLastIndex));
                    sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                    sb.Append(sb2.ToString());
                    lastIndex = endIndex;
                  }
                }
                // Downgrading failed
                if (nonasciiLocalPart) {
                  sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                  if (!hasPhrase) {
                    string addrSpec = str.Substring(token[1], token[2] - token[1]);
                    string encodedText = " " + Rfc2047.EncodeString(addrSpec) + " :;";
                    sb.Append(encodedText);
                  } else {
                    // Has a phrase, extract the addr-spec and convert
                    // the mailbox to a group
                    int angleAddrStart = HeaderParser.ParsePhrase(str, token[1], token[2], null);
                    // append the rest of the string so far up to and including the phrase
                    sb.Append(str.Substring(lastIndex, angleAddrStart - lastIndex));
                    int addrSpecStart = HeaderParser.ParseCFWS(str, angleAddrStart, token[2], null);
                    if (addrSpecStart < token[2] && str[addrSpecStart] == '<') {
                      ++addrSpecStart;
                    }
                    addrSpecStart = HeaderParser.ParseObsRoute(str, addrSpecStart, token[2], null);
                    int addrSpecEnd = HeaderParser.ParseAddrSpec(str, addrSpecStart, token[2], null);
                    string addrSpec = str.Substring(addrSpecStart, addrSpecEnd - addrSpecStart);
                    bool endsWithSpace = sb.Length > 0 && (sb[sb.Length - 1] == 0x20 || sb[sb.Length - 1] == 0x09);
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

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DecodeEncodedWords(string str) {
        #if DEBUG
        if (str == null) {
          throw new ArgumentNullException("str");
        }
        #endif

        // For structured header fields that allow comments only wherever whitespace
        // is allowed, and allow parentheses only for comments
        if (str.Length < 9) {
          // too short for encoded words
          return str;
        }
        if (str.IndexOf("=?") < 0) {
          // No encoded words
          return str;
        }
        StringBuilder sb = new StringBuilder();
        Tokener tokener = new Tokener();
        int endIndex = this.Parse(str, 0, str.Length, tokener);
        if (endIndex != str.Length) {
          // The header field is syntactically invalid,
          // so don't decode any encoded words
          // Console.WriteLine("Invalid syntax: " + this.GetType().Name + ", " + str);
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        foreach (int[] token in tokens) {
          // Console.WriteLine("" + token[0] + " [" + (str.Substring(token[1],token[2]-token[1])) + "]");
          if (token[0] == HeaderParserUtility.TokenComment && token[0] >= lastIndex) {
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

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
      public bool IsStructured() {
        return true;
      }
    }

    private sealed class HeaderX400ContentReturn : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400ContentReturn(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDeliveryDate : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDeliveryDate(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderPriority : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderPriority(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderImportance : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderImportance(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderSensitivity : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSensitivity(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderX400ContentIdentifier : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400ContentIdentifier(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderX400Received : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400Received(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderX400MtsIdentifier : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400MtsIdentifier(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderX400Originator : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400Originator(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderX400Recipients : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderX400Recipients(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderConversion : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderConversion(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderConversionWithLoss : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderConversionWithLoss(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderAutoforwarded : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutoforwarded(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderGenerateDeliveryReport : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderGenerateDeliveryReport(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderIncompleteCopy : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderIncompleteCopy(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderPreventNondeliveryReport : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderPreventNondeliveryReport(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderAlternateRecipient : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAlternateRecipient(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDiscloseRecipients : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDiscloseRecipients(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderAcceptLanguage : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAcceptLanguage(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderArchivedAt : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderArchivedAt(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderAuthenticationResults : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAuthenticationResults(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderAutoSubmitted : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutoSubmitted(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderBcc : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentBase : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentBase(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentDisposition : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentDisposition(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentDuration : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentDuration(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentId : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentLanguage : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentLanguage(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentLocation : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentLocation(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentMd5 : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentMd5(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentTransferEncoding : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentTransferEncoding(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentType : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentType(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDate : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDate(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDeferredDelivery : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDeferredDelivery(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDispositionNotificationOptions : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationOptions(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDispositionNotificationTo : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDkimSignature : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDkimSignature(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderEdiintFeatures : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEdiintFeatures(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderEncoding : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEncoding(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderEncrypted : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEncrypted(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderFrom : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderFrom(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderInReplyTo : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderInReplyTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderJabberId : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderJabberId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderKeywords : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderKeywords(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderLanguage : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderLanguage(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderLatestDeliveryTime : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderLatestDeliveryTime(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderListId : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMessageContext : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMessageContext(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMessageId : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMessageId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMimeVersion : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMimeVersion(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsAcp127MessageIdentifier : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsAcp127MessageIdentifier(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsCodressMessageIndicator : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCodressMessageIndicator(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsCopyPrecedence : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCopyPrecedence(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsExemptedAddress : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExemptedAddress(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsExtendedAuthorisationInfo : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExtendedAuthorisationInfo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsHandlingInstructions : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsHandlingInstructions(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsMessageInstructions : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageInstructions(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsMessageType : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageType(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsOriginatorPlad : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOriginatorPlad(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsOriginatorReference : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOriginatorReference(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsOtherRecipientsIndicatorCc : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOtherRecipientsIndicatorCc(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsOtherRecipientsIndicatorTo : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsOtherRecipientsIndicatorTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsPrimaryPrecedence : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsPrimaryPrecedence(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsSubjectIndicatorCodes : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsSubjectIndicatorCodes(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMtPriority : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMtPriority(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderObsoletes : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderObsoletes(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderOriginalRecipient : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderOriginalRecipient(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReceived : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      // TODO: Downgrade the Received header field
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReceivedSpf : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceivedSpf(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderResentTo : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderResentTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReturnPath : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReturnPath(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderSender : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSender(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderSolicitation : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSolicitation(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderTo : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderVbrInfo : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderVbrInfo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXRicevuta : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXRicevuta(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXTiporicevuta : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXTiporicevuta(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXTrasporto : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXTrasporto(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderXVerificasicurezza : StructuredHeaderField {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='index'>A 32-bit signed integer. (2).</param>
    /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
    /// <param name='tokener'>An object that receives parsed tokens.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXVerificasicurezza(str, index, endIndex, tokener);
      }
    }

    private static IDictionary<string, IHeaderFieldParser> fieldMap = CreateHeaderFieldList();
    private static IHeaderFieldParser unstructured = new UnstructuredHeaderField();

    private static IDictionary<string, IHeaderFieldParser> CreateHeaderFieldList() {
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
      fieldMap["expires"] = new HeaderDate();
      fieldMap["autoforwarded"] = new HeaderAutoforwarded();
      fieldMap["generate-delivery-report"] = new HeaderGenerateDeliveryReport();
      fieldMap["incomplete-copy"] = new HeaderIncompleteCopy();
      fieldMap["autosubmitted"] = new HeaderAutoforwarded();
      fieldMap["prevent-nondelivery-report"] = new HeaderPreventNondeliveryReport();
      fieldMap["alternate-recipient"] = new HeaderAlternateRecipient();
      fieldMap["disclose-recipients"] = new HeaderDiscloseRecipients();
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
      fieldMap["content-transfer-encoding"] = new HeaderContentTransferEncoding();
      fieldMap["content-type"] = new HeaderContentType();
      fieldMap["date"] = new HeaderDate();
      fieldMap["deferred-delivery"] = new HeaderDeferredDelivery();
      fieldMap["disposition-notification-options"] = new HeaderDispositionNotificationOptions();
      fieldMap["disposition-notification-to"] = new HeaderDispositionNotificationTo();
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
      fieldMap["mmhs-acp127-message-identifier"] = new HeaderMmhsAcp127MessageIdentifier();
      fieldMap["mmhs-codress-message-indicator"] = new HeaderMmhsCodressMessageIndicator();
      fieldMap["mmhs-copy-precedence"] = new HeaderMmhsCopyPrecedence();
      fieldMap["mmhs-exempted-address"] = new HeaderMmhsExemptedAddress();
      fieldMap["mmhs-extended-authorisation-info"] = new HeaderMmhsExtendedAuthorisationInfo();
      fieldMap["mmhs-handling-instructions"] = new HeaderMmhsHandlingInstructions();
      fieldMap["mmhs-message-instructions"] = new HeaderMmhsMessageInstructions();
      fieldMap["mmhs-message-type"] = new HeaderMmhsMessageType();
      fieldMap["mmhs-originator-plad"] = new HeaderMmhsOriginatorPlad();
      fieldMap["mmhs-originator-reference"] = new HeaderMmhsOriginatorReference();
      fieldMap["mmhs-other-recipients-indicator-cc"] = new HeaderMmhsOtherRecipientsIndicatorCc();
      fieldMap["mmhs-other-recipients-indicator-to"] = new HeaderMmhsOtherRecipientsIndicatorTo();
      fieldMap["mmhs-primary-precedence"] = new HeaderMmhsPrimaryPrecedence();
      fieldMap["mmhs-subject-indicator-codes"] = new HeaderMmhsSubjectIndicatorCodes();
      fieldMap["mt-priority"] = new HeaderMtPriority();
      fieldMap["obsoletes"] = new HeaderObsoletes();
      fieldMap["original-from"] = new HeaderFrom();
      fieldMap["original-message-id"] = new HeaderMessageId();
      fieldMap["original-recipient"] = new HeaderOriginalRecipient();
      fieldMap["received"] = new HeaderReceived();
      fieldMap["received-spf"] = new HeaderReceivedSpf();
      fieldMap["references"] = new HeaderInReplyTo();
      fieldMap["reply-to"] = new HeaderResentTo();
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
      fieldMap["x-archived-at"] = new HeaderArchivedAt();
      fieldMap["x-mittente"] = new HeaderSender();
      fieldMap["x-ricevuta"] = new HeaderXRicevuta();
      fieldMap["x-riferimento-message-id"] = new HeaderMessageId();
      fieldMap["x-tiporicevuta"] = new HeaderXTiporicevuta();
      fieldMap["x-trasporto"] = new HeaderXTrasporto();
      fieldMap["x-verificasicurezza"] = new HeaderXVerificasicurezza();
      // These following header fields, defined in the
      // Message Headers registry as of Apr. 3, 2014,
      // are treated as unstructured.
      // fieldMap["apparently-to"] = unstructured;
      // fieldMap["body"] = unstructured;
      // fieldMap["comments"] = unstructured;
      // fieldMap["content-description"] = unstructured;
      // fieldMap["downgraded-bcc"] = unstructured;
      // fieldMap["downgraded-cc"] = unstructured;
      // fieldMap["downgraded-disposition-notification-to"] = unstructured;
      // fieldMap["downgraded-final-recipient"] = unstructured;
      // fieldMap["downgraded-from"] = unstructured;
      // fieldMap["downgraded-in-reply-to"] = unstructured;
      // fieldMap["downgraded-mail-from"] = unstructured;
      // fieldMap["downgraded-message-id"] = unstructured;
      // fieldMap["downgraded-original-recipient"] = unstructured;
      // fieldMap["downgraded-rcpt-to"] = unstructured;
      // fieldMap["downgraded-references"] = unstructured;
      // fieldMap["downgraded-reply-to"] = unstructured;
      // fieldMap["downgraded-resent-bcc"] = unstructured;
      // fieldMap["downgraded-resent-cc"] = unstructured;
      // fieldMap["downgraded-resent-from"] = unstructured;
      // fieldMap["downgraded-resent-reply-to"] = unstructured;
      // fieldMap["downgraded-resent-sender"] = unstructured;
      // fieldMap["downgraded-resent-to"] = unstructured;
      // fieldMap["downgraded-return-path"] = unstructured;
      // fieldMap["downgraded-sender"] = unstructured;
      // fieldMap["downgraded-to"] = unstructured;
      // fieldMap["errors-to"] = unstructured;
      // fieldMap["subject"] = unstructured;
      return fieldMap;
    }

    public static IHeaderFieldParser GetParser(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (fieldMap.ContainsKey(name)) {
        return fieldMap[name];
      }
      return unstructured;
    }
  }
}
