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
      public string DecodeEncodedWords(string str) {
        // For unstructured header fields.
        return Rfc2047.DecodeEncodedWords(str, 0, str.Length, EncodedWordContext.Unstructured);
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
      public string DecodeEncodedWords(string str) {
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
              string newComment = Rfc2047.DecodeEncodedWords(
str,
i + 1,
endIndex - 1,
EncodedWordContext.Comment);
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

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        if (str.IndexOf('(') < 0) {
          // No comments in the header field value, a common case
          return str;
        }
        for (int phase = 0; phase < 5; ++phase) {
          // Console.WriteLine("Downgrading: phase " + phase + ", " + (str));
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
                // Console.WriteLine(str.Substring(startIndex, endIndex - startIndex));
                if (Message.HasTextToEscape(str, startIndex, endIndex)) {
                  string newComment = Message.ConvertCommentsToEncodedWords(str, startIndex, endIndex);
                  sb.Append(str.Substring(lastIndex, startIndex - lastIndex));
                  // Console.WriteLine("newcomment "+newComment);
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
            string newComment = Rfc2047.GetPhraseText(
str,
startIndex,
endIndex,
tokens,
PhraseTextMode.DecodedTextAndComments);
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
