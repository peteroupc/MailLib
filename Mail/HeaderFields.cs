/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail
{
  internal class HeaderFields
  {
    private class UnstructuredHeaderField : IHeaderFieldParser {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        return str;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
        // For unstructured header fields.
        return Message.ReplaceEncodedWords(str);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
      public bool IsStructured() {
        return false;
      }
    }

    private sealed class NoCommentsOrEncodedWords : IHeaderFieldParser {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
        // For structured header fields that don't allow comments
        return str;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        return str;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
      public bool IsStructured() {
        return true;
      }
    }

    private sealed class EncodedWordsInComments : IHeaderFieldParser {
    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        if (str.IndexOf('(') < 0) {
          // No comments in the header field value, a common case
          return str;
        }
        if (!Message.HasTextToEscape(str)) {
          return str;
        }
        StringBuilder sb = new StringBuilder();
        int lastIndex = 0;
        for (int i = 0; i < str.Length; ++i) {
          if (str[i] == '(') {
            int endIndex = HeaderParser.ParseComment(str, i, str.Length, null);
            if (endIndex != i) {
              // This is a comment, so replace any encoded words
              // in the comment
              string newComment = Message.ConvertCommentsToEncodedWords(str, i, endIndex);
              sb.Append(str.Substring(lastIndex, i - lastIndex));
              sb.Append(newComment);
              lastIndex = endIndex;
              // Set i to the end of the comment, since
              // comments can nest
              i = endIndex;
            }
          }
        }
        sb.Append(str.Substring(lastIndex, str.Length - lastIndex));
        return sb.ToString();
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
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
        if (str.IndexOf('(') < 0) {
          // No comments in the header field value, a common case
          return str;
        }
        if (str.IndexOf("=?") < 0) {
          // No encoded words
          return str;
        }
        StringBuilder sb = new StringBuilder();
        int lastIndex = 0;
        for (int i = 0; i < str.Length; ++i) {
          if (str[i] == '(') {
            int endIndex = HeaderParser.ParseComment(str, i, str.Length, null);
            if (endIndex != i) {
              // This is a comment, so replace any encoded words
              // in the comment
              string newComment = Message.ReplaceEncodedWords(str, i + 1, endIndex - 1, true);
              sb.Append(str.Substring(lastIndex, i + 1 - lastIndex));
              sb.Append(newComment);
              lastIndex = endIndex - 1;
              // Set i to the end of the comment, since
              // comments can nest
              i = endIndex;
            }
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

    internal abstract class EncodedWordsInSyntax : IHeaderFieldParser {
      protected abstract int Parse(string str, int index, int endIndex, ITokener tokener);

      private static string GetCFWS(string str, int index, int endIndex) {
        StringBuilder builder = new StringBuilder();
        while (index < endIndex) {
          if (str[index] == '"') {
            int index2 = MediaType.skipQuotedString(str, index, endIndex, null);
            if (index2 == index) {
              ++index;
            } else {
              index = index2;
            }
          } else if (str[index] == '(' || str[index] == 0x0d || str[index] == 0x09 || str[index] == 0x20) {
            int index2 = HeaderParser.ParseCFWS(str, index, endIndex, null);
            if (index2 != index) {
              string cfws = str.Substring(index, index2 - index);
              // Don't append if there are no comments in
              // this CFWS and the last character
              // in the buffer is a space or tab
              if (!(cfws.IndexOf('(') < 0 && builder.Length > 0 && (str[index] == 0x09 || str[index] == 0x20))) {
                builder.Append(cfws);
              }
              index = index2;
            } else {
              ++index;
            }
          } else {
            ++index;
          }
        }
        return builder.ToString();
      }

      private static string ReplacePhraseWithEncodedWords(string str, int index, int endIndex, IList<int[]> tokens) {
        // Assumes the value matches the production "phrase",
        // and assumes that endIndex is the end of all CFWS
        // found after the phrase
        if (!Message.HasTextToEscape(str, index, endIndex)) {
          // No need to use encoded words
          return str.Substring(index, endIndex - index);
        }
        int startIndex = index;
        int startIndexAfterCFWS = HeaderParser.ParseCFWS(str, index, endIndex, null);
        string phrase = HeaderParserUtility.ParsePhrase(str, index, endIndex, tokens);
        StringBuilder builder = new StringBuilder();
        if (startIndexAfterCFWS != startIndex) {
          // Copy CFWS
          builder.Append(str.Substring(index, startIndexAfterCFWS - index));
          index = startIndexAfterCFWS;
          if (!PrecededByStartOrLinearWhitespace(str, index)) {
            builder.Append(' ');
          }
        }
        // Convert the parsed string to encoded words
        EncodedWordEncoder encoder = new EncodedWordEncoder();
        encoder.AddString(phrase);
        encoder.FinalizeEncoding();
        if (!PrecededByStartOrLinearWhitespace(str, index)) {
          builder.Append(' ');
        }
        builder.Append(encoder.ToString());
        // Extract all CFWS after the start of the phrase
        string cfws = GetCFWS(str, startIndexAfterCFWS, endIndex);
        if (cfws.Length == 0 || (cfws[0] != 0x20 && cfws[0] != 0x09)) {
          // Append a space if the CFWS is empty or doesn't start with
          // whitespace (RFC 2047 requires encoded words to be separated
          // by linear whitespace)
          builder.Append(' ');
        }
        builder.Append(cfws);
        return builder.ToString();
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        if (str.IndexOf('(') < 0) {
          // No comments in the header field value, a common case
          return str;
        }
        for (int phase = 0; phase < 5; ++phase) {
          if (!Message.HasTextToEscape(str)) {
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
          foreach (int[] token in tokens) {
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
                if (Message.HasTextToEscape(str, startIndex, endIndex)) {
                  string newComment = Message.ConvertCommentsToEncodedWords(str, startIndex, endIndex);
                  sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                  sb.Append(newComment);
                } else {
                  // No text needs to be escaped, output the comment as is
                  sb.Append(str.Substring(lastIndex, endIndex - lastIndex));
                }
                lastIndex = endIndex;
              }
            } else if (phase == 1) {  // Word downgrading
              if (token[0] == HeaderParserUtility.TokenPhrase) {
                int startIndex = token[1];
                endIndex = token[2];
                string newComment = ReplacePhraseWithEncodedWords(str, startIndex, endIndex, tokens);
                sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                sb.Append(newComment);
                lastIndex = endIndex;
              } else if (token[0] == HeaderParserUtility.TokenAtom ||
                         token[0] == HeaderParserUtility.TokenQuotedString) {
                // TODO: Currently not limited to atoms and quoted strings in "words"
                int startIndex = token[1];
                endIndex = token[2];
                string newComment = ReplacePhraseWithEncodedWords(str, startIndex, endIndex, tokens);
                sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                sb.Append(newComment);
                lastIndex = endIndex;
              }
            } else if (phase == 2) {  // Group downgrading
              // TODO: Group downgrading
            } else if (phase == 3) {  // Mailbox downgrading
              // TODO: Mailbox downgrading
            } else if (phase == 4) {  // type addr downgrading
              // TODO: check RFC 6533
            }
          }
          sb.Append(str.Substring(lastIndex, str.Length - lastIndex));
          str = sb.ToString();
        }
        return str;
      }

      private static bool FollowedByEndOrLinearWhitespace(string str, int index, int endIndex) {
        if (index == endIndex) {
          return true;
        }
        if (str[index] != 0x09 && str[index] != 0x20 && str[index] != 0x0d) {
          return false;
        }
        int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (cws == index) {
          // No linear whitespace
          return false;
        }
        return true;
      }

      private static bool PrecededByStartOrLinearWhitespace(string str, int index) {
        if (index == 0) {
          return true;
        }
        if (index - 1 >= 0 && (str[index - 1] == 0x09 || str[index - 1] == 0x20)) {
          return true;
        }
        return false;
      }

      private static int IndexOfNextPossibleEncodedWord(string str, int index, int endIndex) {
        int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (cws == index) {
          // No linear whitespace
          return -1;
        }
        while (index < cws) {
          if (str[index] == '(') {
            // Has a comment, so no encoded word
            // immediately follows
            return -1;
          }
          ++index;
        }
        if (index + 1 < endIndex && str[index] == '=' && str[index + 1] == '?') {
          // Has a possible encoded word
          return index;
        }
        return -1;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string ReplaceEncodedWords(string str) {
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
        int lastPhraseStart = -1;
        int lastPhraseEnd = -1;
        // Get each relevant token sorted by starting index
        foreach (int[] token in tokener.GetTokens()) {
          if (token[0] == 1) {
            // This is a comment token
            int startIndex = token[1];
            endIndex = token[2];
            string newComment = Message.ReplaceEncodedWords(str, startIndex + 1, endIndex - 1, true);
            sb.Append(str.Substring(lastIndex, startIndex + 1 - lastIndex));
            sb.Append(newComment);
            lastIndex = endIndex - 1;
          } else if (token[0] == 2) {
            // This is a phrase token
            lastPhraseStart = token[1];
            lastPhraseEnd = token[2];
          } else if (token[0] == HeaderParserUtility.TokenPhraseAtom ||
                     token[0] == HeaderParserUtility.TokenPhraseAtomOrDot) {
            // This is an atom token; only words within
            // a phrase can be encoded words
            if (token[1] >= lastIndex &&
                token[1] >= lastPhraseStart && token[1] <= lastPhraseEnd &&
                token[2] >= lastPhraseStart && token[2] <= lastPhraseEnd) {
              // This is an atom within a phrase
              int wordStart = HeaderParser.ParseCFWS(str, token[1], token[2], null);
              int wordEnd;
              int previousWord = wordStart;
              if (wordStart >= token[2] || str[wordStart] != '=') {
                // Not an encoded word
                continue;
              }
              wordEnd = wordStart;
              while (true) {
                if (!PrecededByStartOrLinearWhitespace(str, wordEnd)) {
                  break;
                }
                // Find the end of the atom
                wordEnd = HeaderParser.ParsePhraseAtom(str, wordEnd, lastPhraseEnd, null);
                if (!FollowedByEndOrLinearWhitespace(str, wordEnd, lastPhraseEnd)) {
                  // The encoded word is not followed by whitespace, so it's not valid
                  wordEnd = previousWord;
                  break;
                }
                int nextWord = IndexOfNextPossibleEncodedWord(str, wordEnd, lastPhraseEnd);
                if (nextWord < 0) {
                  // The next word isn't an encoded word
                  break;
                }
                previousWord = nextWord;
                wordEnd = nextWord;
              }
              string replacement = Message.ReplaceEncodedWords(str, wordStart, wordEnd, false);
              sb.Append(str.Substring(lastIndex, wordStart - lastIndex));
              sb.Append(replacement);
              lastIndex = wordEnd;
            }
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

    private sealed class HeaderAuthenticationResults : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAuthenticationResults(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderAutoSubmitted : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutoSubmitted(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderBcc : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderBcc(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentBase : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentBase(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentDisposition : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentDisposition(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentId : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderContentType : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentType(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDate : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDate(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDispositionNotificationOptions : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationOptions(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderDispositionNotificationTo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderDispositionNotificationTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderEncrypted : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderEncrypted(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderFrom : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderFrom(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderInReplyTo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderInReplyTo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderKeywords : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderKeywords(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderLanguage : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderLanguage(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderListArchive : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListArchive(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderListId : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListId(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderListPost : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderListPost(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsCopyPrecedence : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsCopyPrecedence(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsExemptedAddress : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExemptedAddress(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsExtendedAuthorisationInfo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsExtendedAuthorisationInfo(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderMmhsMessageType : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderMmhsMessageType(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderObsoletes : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderObsoletes(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderOriginalRecipient : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderOriginalRecipient(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReceived : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceived(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReceivedSpf : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReceivedSpf(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReturnPath : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderReturnPath(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderSender : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderSender(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderTo : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderTo(str, index, endIndex, tokener);
      }
    }

    internal class Mailbox : EncodedWordsInSyntax {
      protected override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderXMittente(str, index, endIndex, tokener);
      }
    }

    private static IDictionary<string, IHeaderFieldParser> list = CreateHeaderFieldList();
    private static IHeaderFieldParser unstructured = new UnstructuredHeaderField();

    private static IDictionary<string, IHeaderFieldParser> CreateHeaderFieldList() {
      list = new Dictionary<string, IHeaderFieldParser>();
      IHeaderFieldParser structuredNoComments = new NoCommentsOrEncodedWords();
      IHeaderFieldParser structuredComments = new EncodedWordsInComments();
      IHeaderFieldParser unstructured = new UnstructuredHeaderField();
      // These structured header fields won't be parsed for comments.
      list["alternate-recipient"] = structuredNoComments;
      list["archived-at"] = structuredNoComments;
      list["autoforwarded"] = structuredNoComments;
      list["autosubmitted"] = structuredNoComments;
      list["content-alternative"] = structuredNoComments;
      list["content-features"] = structuredNoComments;
      list["content-return"] = structuredNoComments;
      list["conversion"] = structuredNoComments;
      list["conversion-with-loss"] = structuredNoComments;
      list["disclose-recipients"] = structuredNoComments;
      list["dkim-signature"] = structuredNoComments;
      list["ediint-features"] = structuredNoComments;
      list["generate-delivery-report"] = structuredNoComments;
      list["importance"] = structuredNoComments;
      list["incomplete-copy"] = structuredNoComments;
      list["jabber-id"] = structuredNoComments;
      list["mmhs-acp127-message-identifier"] = structuredNoComments;
      list["mmhs-codress-message-indicator"] = structuredNoComments;
      list["mmhs-handling-instructions"] = structuredNoComments;
      list["mmhs-message-instructions"] = structuredNoComments;
      list["mmhs-originator-plad"] = structuredNoComments;
      list["mmhs-originator-reference"] = structuredNoComments;
      list["mmhs-other-recipients-indicator-cc"] = structuredNoComments;
      list["mmhs-other-recipients-indicator-to"] = structuredNoComments;
      list["mmhs-subject-indicator-codes"] = structuredNoComments;
      list["original-subject"] = structuredNoComments;
      list["pics-label"] = structuredNoComments;
      list["prevent-nondelivery-report"] = structuredNoComments;
      list["priority"] = structuredNoComments;
      list["privicon"] = structuredNoComments;
      list["sensitivity"] = structuredNoComments;
      list["solicitation"] = structuredNoComments;
      list["vbr-info"] = structuredNoComments;
      list["x-archived-at"] = structuredNoComments;
      list["x400-content-identifier"] = structuredNoComments;
      list["x400-content-return"] = structuredNoComments;
      list["x400-mts-identifier"] = structuredNoComments;
      list["x400-received"] = structuredNoComments;
      list["x400-trace"] = structuredNoComments;
      // These structured header fields allow comments anywhere
      // they allow whitespace (thus, if a comment occurs anywhere
      // it can't appear, replacing it with a space will result
      // in a syntactically invalid header field).
      // They also don't allow parentheses outside of comments.
      list["accept-language"] = structuredComments;
      list["content-duration"] = structuredComments;
      list["content-language"] = structuredComments;
      list["content-md5"] = structuredComments;
      list["content-transfer-encoding"] = structuredComments;
      list["encoding"] = structuredComments;
      list["message-context"] = structuredComments;
      list["mime-version"] = structuredComments;
      list["mt-priority"] = structuredComments;
      list["x-ricevuta"] = structuredComments;
      list["x-tiporicevuta"] = structuredComments;
      list["x-trasporto"] = structuredComments;
      list["x-verificasicurezza"] = structuredComments;
      // These following header fields, defined in the
      // Message Headers registry as of Apr. 3, 2014,
      // are treated as unstructured.
      list["apparently-to"] = unstructured;
      list["body"] = unstructured;
      list["comments"] = unstructured;
      list["content-description"] = unstructured;
      list["downgraded-bcc"] = unstructured;
      list["downgraded-cc"] = unstructured;
      list["downgraded-disposition-notification-to"] = unstructured;
      list["downgraded-final-recipient"] = unstructured;
      list["downgraded-from"] = unstructured;
      list["downgraded-in-reply-to"] = unstructured;
      list["downgraded-mail-from"] = unstructured;
      list["downgraded-message-id"] = unstructured;
      list["downgraded-original-recipient"] = unstructured;
      list["downgraded-rcpt-to"] = unstructured;
      list["downgraded-references"] = unstructured;
      list["downgraded-reply-to"] = unstructured;
      list["downgraded-resent-bcc"] = unstructured;
      list["downgraded-resent-cc"] = unstructured;
      list["downgraded-resent-from"] = unstructured;
      list["downgraded-resent-reply-to"] = unstructured;
      list["downgraded-resent-sender"] = unstructured;
      list["downgraded-resent-to"] = unstructured;
      list["downgraded-return-path"] = unstructured;
      list["downgraded-sender"] = unstructured;
      list["downgraded-to"] = unstructured;
      list["errors-to"] = unstructured;
      list["subject"] = unstructured;
      // These header fields have their own syntax rules.
      list["authentication-results"] = new HeaderAuthenticationResults();
      list["auto-submitted"] = new HeaderAutoSubmitted();
      list["base"] = new HeaderContentBase();
      list["bcc"] = new HeaderBcc();
      list["cc"] = new HeaderTo();
      list["content-base"] = new HeaderContentBase();
      list["content-disposition"] = new HeaderContentDisposition();
      list["content-id"] = new HeaderContentId();
      list["content-location"] = new HeaderContentBase();
      list["content-type"] = new HeaderContentType();
      list["date"] = new HeaderDate();
      list["deferred-delivery"] = new HeaderDate();
      list["delivery-date"] = new HeaderDate();
      list["disposition-notification-options"] = new HeaderDispositionNotificationOptions();
      list["disposition-notification-to"] = new HeaderDispositionNotificationTo();
      list["encrypted"] = new HeaderEncrypted();
      list["expires"] = new HeaderDate();
      list["expiry-date"] = new HeaderDate();
      list["from"] = new HeaderFrom();
      list["in-reply-to"] = new HeaderInReplyTo();
      list["keywords"] = new HeaderKeywords();
      list["language"] = new HeaderLanguage();
      list["latest-delivery-time"] = new HeaderDate();
      list["list-archive"] = new HeaderListArchive();
      list["list-help"] = new HeaderListArchive();
      list["list-id"] = new HeaderListId();
      list["list-owner"] = new HeaderListArchive();
      list["list-post"] = new HeaderListPost();
      list["list-subscribe"] = new HeaderListArchive();
      list["list-unsubscribe"] = new HeaderListArchive();
      list["message-id"] = new HeaderContentId();
      list["mmhs-copy-precedence"] = new HeaderMmhsCopyPrecedence();
      list["mmhs-exempted-address"] = new HeaderMmhsExemptedAddress();
      list["mmhs-extended-authorisation-info"] = new HeaderMmhsExtendedAuthorisationInfo();
      list["mmhs-message-type"] = new HeaderMmhsMessageType();
      list["mmhs-primary-precedence"] = new HeaderMmhsCopyPrecedence();
      list["obsoletes"] = new HeaderObsoletes();
      list["original-from"] = new HeaderFrom();
      list["original-message-id"] = new HeaderContentId();
      list["original-recipient"] = new HeaderOriginalRecipient();
      list["received"] = new HeaderReceived();
      list["received-spf"] = new HeaderReceivedSpf();
      list["references"] = new HeaderInReplyTo();
      list["reply-by"] = new HeaderDate();
      list["reply-to"] = new HeaderTo();
      list["resent-bcc"] = new HeaderBcc();
      list["resent-cc"] = new HeaderTo();
      list["resent-date"] = new HeaderDate();
      list["resent-from"] = new HeaderFrom();
      list["resent-message-id"] = new HeaderContentId();
      list["resent-reply-to"] = new HeaderTo();
      list["resent-sender"] = new HeaderSender();
      list["resent-to"] = new HeaderTo();
      list["return-path"] = new HeaderReturnPath();
      list["sender"] = new HeaderSender();
      list["to"] = new HeaderTo();
      list["x-mittente"] = new Mailbox();
      list["x-riferimento-message-id"] = new HeaderContentId();
      list["x400-originator"] = new Mailbox();
      list["x400-recipients"] = new HeaderDispositionNotificationTo();
      return list;
    }

    public static IHeaderFieldParser GetParser(string name) {
      #if DEBUG
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      #endif

      name = ParserUtility.ToLowerCaseAscii(name);
      if (list.ContainsKey(name)) {
        return list[name];
      }
      return unstructured;
    }
  }
}
