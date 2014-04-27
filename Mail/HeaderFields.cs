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

namespace PeterO.Mail {
  internal class HeaderFields
  {
    private class UnstructuredHeaderField : IHeaderFieldParser {
      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object. (2).</param>
      /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        return new EncodedWordEncoder()
          .AddString(str)
          .FinalizeEncoding()
          .ToString();
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

      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object. (2).</param>
      /// <returns>A string object.</returns>
      public string DowngradeFieldValue(string str) {
        for (int phase = 0; phase < 5; ++phase) {
          if (str.IndexOf('(') < 0 && phase == 0) {
            // No comments in the header field value, a common case
            continue;
          }
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
                  string newComment = Rfc2047.EncodeComment(str, startIndex, endIndex);
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
          Console.WriteLine("Invalid syntax: " + this.GetType().Name + ", " + str);
          return str;
        }
        int lastIndex = 0;
        // Get each relevant token sorted by starting index
        IList<int[]> tokens = tokener.GetTokens();
        foreach (int[] token in tokens) {
          //Console.WriteLine("" + token[0] + " [" + (str.Substring(token[1],token[2]-token[1])) + "]");
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

    private sealed class HeaderAutosubmitted : StructuredHeaderField {
      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object.</param>
      /// <param name='index'>A 32-bit signed integer. (2).</param>
      /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
      /// <param name='tokener'>An object that receives parsed tokens.</param>
      /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderAutosubmitted(str, index, endIndex, tokener);
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

    private sealed class HeaderContentAlternative : StructuredHeaderField {
      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object.</param>
      /// <param name='index'>A 32-bit signed integer. (2).</param>
      /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
      /// <param name='tokener'>An object that receives parsed tokens.</param>
      /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentAlternative(str, index, endIndex, tokener);
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

    private sealed class HeaderContentFeatures : StructuredHeaderField {
      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object.</param>
      /// <param name='index'>A 32-bit signed integer. (2).</param>
      /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
      /// <param name='tokener'>An object that receives parsed tokens.</param>
      /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderContentFeatures(str, index, endIndex, tokener);
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

    private sealed class HeaderPicsLabel : StructuredHeaderField {
      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object.</param>
      /// <param name='index'>A 32-bit signed integer. (2).</param>
      /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
      /// <param name='tokener'>An object that receives parsed tokens.</param>
      /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderPicsLabel(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderPrivicon : StructuredHeaderField {
      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object.</param>
      /// <param name='index'>A 32-bit signed integer. (2).</param>
      /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
      /// <param name='tokener'>An object that receives parsed tokens.</param>
      /// <returns>A 32-bit signed integer.</returns>
      public override int Parse(string str, int index, int endIndex, ITokener tokener) {
        return HeaderParser.ParseHeaderPrivicon(str, index, endIndex, tokener);
      }
    }

    private sealed class HeaderReceived : StructuredHeaderField {
      /// <summary>Not documented yet.</summary>
      /// <param name='str'>A string object.</param>
      /// <param name='index'>A 32-bit signed integer. (2).</param>
      /// <param name='endIndex'>A 32-bit signed integer. (3).</param>
      /// <param name='tokener'>An object that receives parsed tokens.</param>
      /// <returns>A 32-bit signed integer.</returns>
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

    private static IDictionary<string, IHeaderFieldParser> list = CreateHeaderFieldList();
    private static IHeaderFieldParser unstructured = new UnstructuredHeaderField();

    private static IDictionary<string, IHeaderFieldParser> CreateHeaderFieldList() {
      list = new Dictionary<string, IHeaderFieldParser>();
      list["content-return"] = new HeaderX400ContentReturn();
      list["x400-content-return"] = new HeaderX400ContentReturn();
      list["delivery-date"] = new HeaderDeliveryDate();
      list["priority"] = new HeaderPriority();
      list["importance"] = new HeaderImportance();
      list["sensitivity"] = new HeaderSensitivity();
      list["reply-by"] = new HeaderDate();
      list["x400-content-identifier"] = new HeaderX400ContentIdentifier();
      list["x400-received"] = new HeaderX400Received();
      list["x400-mts-identifier"] = new HeaderX400MtsIdentifier();
      list["x400-trace"] = new HeaderX400Received();
      list["x400-originator"] = new HeaderX400Originator();
      list["x400-recipients"] = new HeaderX400Recipients();
      list["conversion"] = new HeaderConversion();
      list["conversion-with-loss"] = new HeaderConversionWithLoss();
      list["expires"] = new HeaderDate();
      list["autoforwarded"] = new HeaderAutoforwarded();
      list["generate-delivery-report"] = new HeaderGenerateDeliveryReport();
      list["incomplete-copy"] = new HeaderIncompleteCopy();
      list["autosubmitted"] = new HeaderAutosubmitted();
      list["prevent-nondelivery-report"] = new HeaderPreventNondeliveryReport();
      list["alternate-recipient"] = new HeaderAlternateRecipient();
      list["disclose-recipients"] = new HeaderDiscloseRecipients();
      list["accept-language"] = new HeaderAcceptLanguage();
      list["archived-at"] = new HeaderArchivedAt();
      list["authentication-results"] = new HeaderAuthenticationResults();
      list["auto-submitted"] = new HeaderAutoSubmitted();
      list["base"] = new HeaderContentBase();
      list["bcc"] = new HeaderBcc();
      list["cc"] = new HeaderTo();
      list["content-alternative"] = new HeaderContentAlternative();
      list["content-base"] = new HeaderContentBase();
      list["content-disposition"] = new HeaderContentDisposition();
      list["content-duration"] = new HeaderContentDuration();
      list["content-features"] = new HeaderContentFeatures();
      list["content-id"] = new HeaderContentId();
      list["content-language"] = new HeaderContentLanguage();
      list["content-location"] = new HeaderContentLocation();
      list["content-md5"] = new HeaderContentMd5();
      list["content-transfer-encoding"] = new HeaderContentTransferEncoding();
      list["content-type"] = new HeaderContentType();
      list["date"] = new HeaderDate();
      list["deferred-delivery"] = new HeaderDeferredDelivery();
      list["disposition-notification-options"] = new HeaderDispositionNotificationOptions();
      list["disposition-notification-to"] = new HeaderDispositionNotificationTo();
      list["dkim-signature"] = new HeaderDkimSignature();
      list["ediint-features"] = new HeaderEdiintFeatures();
      list["encoding"] = new HeaderEncoding();
      list["encrypted"] = new HeaderEncrypted();
      list["expiry-date"] = new HeaderDate();
      list["from"] = new HeaderFrom();
      list["in-reply-to"] = new HeaderInReplyTo();
      list["jabber-id"] = new HeaderJabberId();
      list["keywords"] = new HeaderKeywords();
      list["language"] = new HeaderLanguage();
      list["latest-delivery-time"] = new HeaderLatestDeliveryTime();
      list["list-id"] = new HeaderListId();
      list["message-context"] = new HeaderMessageContext();
      list["message-id"] = new HeaderMessageId();
      list["mime-version"] = new HeaderMimeVersion();
      list["mmhs-acp127-message-identifier"] = new HeaderMmhsAcp127MessageIdentifier();
      list["mmhs-codress-message-indicator"] = new HeaderMmhsCodressMessageIndicator();
      list["mmhs-copy-precedence"] = new HeaderMmhsCopyPrecedence();
      list["mmhs-exempted-address"] = new HeaderMmhsExemptedAddress();
      list["mmhs-extended-authorisation-info"] = new HeaderMmhsExtendedAuthorisationInfo();
      list["mmhs-handling-instructions"] = new HeaderMmhsHandlingInstructions();
      list["mmhs-message-instructions"] = new HeaderMmhsMessageInstructions();
      list["mmhs-message-type"] = new HeaderMmhsMessageType();
      list["mmhs-originator-plad"] = new HeaderMmhsOriginatorPlad();
      list["mmhs-originator-reference"] = new HeaderMmhsOriginatorReference();
      list["mmhs-other-recipients-indicator-cc"] = new HeaderMmhsOtherRecipientsIndicatorCc();
      list["mmhs-other-recipients-indicator-to"] = new HeaderMmhsOtherRecipientsIndicatorTo();
      list["mmhs-primary-precedence"] = new HeaderMmhsPrimaryPrecedence();
      list["mmhs-subject-indicator-codes"] = new HeaderMmhsSubjectIndicatorCodes();
      list["mt-priority"] = new HeaderMtPriority();
      list["obsoletes"] = new HeaderObsoletes();
      list["original-from"] = new HeaderFrom();
      list["original-message-id"] = new HeaderMessageId();
      list["original-recipient"] = new HeaderOriginalRecipient();
      list["pics-label"] = new HeaderPicsLabel();
      list["privicon"] = new HeaderPrivicon();
      list["received"] = new HeaderReceived();
      list["received-spf"] = new HeaderReceivedSpf();
      list["references"] = new HeaderInReplyTo();
      list["reply-to"] = new HeaderResentTo();
      list["resent-bcc"] = new HeaderBcc();
      list["resent-cc"] = new HeaderResentTo();
      list["resent-date"] = new HeaderDate();
      list["resent-from"] = new HeaderFrom();
      list["resent-message-id"] = new HeaderMessageId();
      list["resent-reply-to"] = new HeaderResentTo();
      list["resent-sender"] = new HeaderSender();
      list["resent-to"] = new HeaderResentTo();
      list["return-path"] = new HeaderReturnPath();
      list["sender"] = new HeaderSender();
      list["solicitation"] = new HeaderSolicitation();
      list["to"] = new HeaderTo();
      list["vbr-info"] = new HeaderVbrInfo();
      list["x-archived-at"] = new HeaderArchivedAt();
      list["x-mittente"] = new HeaderSender();
      list["x-ricevuta"] = new HeaderXRicevuta();
      list["x-riferimento-message-id"] = new HeaderMessageId();
      list["x-tiporicevuta"] = new HeaderXTiporicevuta();
      list["x-trasporto"] = new HeaderXTrasporto();
      list["x-verificasicurezza"] = new HeaderXVerificasicurezza();
      // These following header fields, defined in the
      // Message Headers registry as of Apr. 3, 2014,
      // are treated as unstructured.
      // list["apparently-to"] = unstructured;
      // list["body"] = unstructured;
      // list["comments"] = unstructured;
      // list["content-description"] = unstructured;
      // list["downgraded-bcc"] = unstructured;
      // list["downgraded-cc"] = unstructured;
      // list["downgraded-disposition-notification-to"] = unstructured;
      // list["downgraded-final-recipient"] = unstructured;
      // list["downgraded-from"] = unstructured;
      // list["downgraded-in-reply-to"] = unstructured;
      // list["downgraded-mail-from"] = unstructured;
      // list["downgraded-message-id"] = unstructured;
      // list["downgraded-original-recipient"] = unstructured;
      // list["downgraded-rcpt-to"] = unstructured;
      // list["downgraded-references"] = unstructured;
      // list["downgraded-reply-to"] = unstructured;
      // list["downgraded-resent-bcc"] = unstructured;
      // list["downgraded-resent-cc"] = unstructured;
      // list["downgraded-resent-from"] = unstructured;
      // list["downgraded-resent-reply-to"] = unstructured;
      // list["downgraded-resent-sender"] = unstructured;
      // list["downgraded-resent-to"] = unstructured;
      // list["downgraded-return-path"] = unstructured;
      // list["downgraded-sender"] = unstructured;
      // list["downgraded-to"] = unstructured;
      // list["errors-to"] = unstructured;
      // list["subject"] = unstructured;
      return list;
    }

    public static IHeaderFieldParser GetParser(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (list.ContainsKey(name)) {
        return list[name];
      }
      return unstructured;
    }
  }
}
