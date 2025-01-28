/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Mail.Transforms;
using PeterO.Text;

namespace PeterO.Mail {
  internal static class Rfc2047 {
    private static bool HasSuspiciousTextInComments(string str) {
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if ((c < 0x20 && c != '\t') || c == '(' || c == ')' || c == '\\' ||
          c == 0x7f) {
          return true;
        }
      }
      return false;
    }

    private static bool HasAsciiCtl(string str) {
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if ((c < 0x20 && c != '\t') || c == 0x7f) {
          return true;
        }
      }
      return false;
    }

    private static bool HasSuspiciousTextInStructured(string str) {
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        // Has 'specials', or CTLs other than tab
        if ((c < 0x20 && c != '\t') || c == 0x7F || c == 0x28 || c == 0x29 ||
          c == 0x3c || c == 0x3e || c == 0x5b || c == 0x5d || c == 0x3a || c
          == 0x3b || c == 0x40 ||
          c == 0x5c || c == 0x2c || c == 0x2e || c == '"') {
          return true;
        }
      }
      return false;
    }

    public static void EncodeComment(
      HeaderEncoder enc,
      string str,
      int index,
      int endIndex) {
      // NOTE: Assumes that the comment is syntactically valid
      #if DEBUG
      if (str == null) {
        throw new ArgumentNullException(nameof(str));
      }
      if (index < 0) {
        throw new ArgumentException("index(" + index + ") is less than " +
          "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index(" + index + ") is more than " +
          str.Length);
      }
      if (endIndex < 0) {
        throw new ArgumentException("endIndex(" + endIndex +
          ") is less than " + "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex(" + endIndex +
          ") is more than " + str.Length);
      }
      #endif
      int length = endIndex - index;
      if (length < 2 || str[index] != '(' || str[endIndex - 1] != ')') {
        enc.AppendString(str.Substring(index, length - index));
      }
      int nextComment = str.IndexOf('(', index + 1);
      int nextBackslash = str.IndexOf('\\', index + 1);
      // don't count comments or backslashes beyond
      // the desired portion
      if (nextComment >= endIndex) {
        nextComment = -1;
      }
      if (nextBackslash >= endIndex) {
        nextBackslash = -1;
      }
      bool haveEscape = nextBackslash >= 0;
      if (!haveEscape) {
        // Check for possible folding whitespace
        nextBackslash = str.IndexOf('\n', index + 1);
        if (nextBackslash >= endIndex) {
          nextBackslash = -1;
        }
        haveEscape = nextBackslash >= 0;
      }
      if (nextComment < 0 && nextBackslash < 0) {
        // No escapes or nested comments, so it's relatively easy
        enc.AppendSymbol("(");
        if (length > 2) {
          enc.AppendAsEncodedWords(str.Substring(index + 1, length - 2));
        }
        enc.AppendSymbol(")");
        return;
      }
      if (nextBackslash < 0) {
        // No escapes; just look for '(' and ')'
        while (true) {
          int parenStart = index;
          // Get the next run of parentheses
          while (index < endIndex) {
            if (str[index] == '(' || str[index] == ')') {
              ++index;
            } else {
              break;
            }
          }
          // Get the next run of nonparentheses
          int parenEnd = index;
          while (index < endIndex) {
            if (str[index] == '(' || str[index] == ')') {
              break;
            }
            ++index;
          }
          if (parenEnd == index) {
            for (int k = parenStart; k < parenEnd; ++k) {
              enc.AppendSymbol(str.Substring(k, 1));
            }
            break;
          }
          for (int k = parenStart; k < parenEnd; ++k) {
            enc.AppendSymbol(str.Substring(k, 1));
          }
          enc.AppendAsEncodedWords(str.Substring(
            parenEnd,
            index - parenEnd));
        }
        return;
      }
      var builder = new StringBuilder();
      // escapes, but no nested comments
      if (nextComment < 0) {
        // skip the first parenthesis
        ++index;
        while (index < endIndex) {
          if (str[index] == ')') {
            // End of the comment
            break;
          }
          if (str[index] == '\r' && index + 2 < endIndex &&
            str[index + 1] == '\n' && (str[index + 2] == 0x20 || str[index +
2] ==
              0x09)) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1 < endIndex) {
            // Quoted pair
            int cp = DataUtilities.CodePointAt(str, index + 1);
            if (cp <= 0xffff) {
              {
                builder.Append((char)cp);
              }
            } else if (cp <= 0x10ffff) {
              builder.Append((char)((((cp - 0x10000) >> 10) & 0x3ff) |
0xd800));
              builder.Append((char)(((cp - 0x10000) & 0x3ff) | 0xdc00));
            }
            index += 1 + (cp >= 0x10000 ? 2 : 1);
          } else {
            // Other comment text
            builder.Append(str[index]);
            ++index;
          }
        }
        enc.AppendSymbol("(");
        enc.AppendAsEncodedWords(builder.ToString());
        enc.AppendSymbol(")");
        return;
      }
      // escapes and nested comments
      while (true) {
        int parenStart = index;
        // Get the next run of parentheses
        while (index < endIndex) {
          if (str[index] == '(' || str[index] == ')') {
            ++index;
          } else {
            break;
          }
        }
        // Get the next run of nonparentheses
        int parenEnd = index;
        builder.Remove(
          0,
          builder.Length);
        while (index < endIndex) {
          if (str[index] == '(' || str[index] == ')') {
            break;
          }
          if (str[index] == '\r' && index + 2 < endIndex &&
            str[index + 1] == '\n' && (str[index + 2] == 0x20 || str[index +
2] ==
              0x09)) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1 < endIndex) {
            // Quoted pair
            int cp = DataUtilities.CodePointAt(str, index + 1);
            if (cp <= 0xffff) {
              builder.Append((char)cp);
            } else if (cp <= 0x10ffff) {
              builder.Append((char)((((cp - 0x10000) >> 10) & 0x3ff) |
0xd800));
              builder.Append((char)(((cp - 0x10000) & 0x3ff) | 0xdc00));
            }
            index += 1 + (cp >= 0x10000 ? 2 : 1);
          } else {
            // Other comment text
            builder.Append(str[index]);
            ++index;
          }
        }
        if (builder.Length == 0) {
          for (int k = parenStart; k < parenEnd; ++k) {
            enc.AppendSymbol(str.Substring(k, 1));
          }
          break;
        }
        for (int k = parenStart; k < parenEnd; ++k) {
          enc.AppendSymbol(str.Substring(k, 1));
        }
        enc.AppendAsEncodedWords(builder.ToString());
      }
      return;
    }

    private static int SkipCharsetOrEncoding(
      string str,
      int index,
      int endIndex) {
      const string ValueSpecials = "()<>[]@,;:\\\"/?=.";
      int i = index;
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7f || ((c & 0x7f) == c &&
            ValueSpecials.IndexOf(c) >= 0)) {
          break;
        }
        ++i;
      }
      return i;
    }

    // See point 3 of RFC 2047 sec. 5 (but excludes '=')
    private static readonly int[] ValueSmallchars = {
      0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    };

    // ASCII characters allowed in atoms
    private static readonly int[] ValueAsciiAtext = {
      0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
    };

    private static int SkipEncodedText(
      string str,
      int index,
      int endIndex,
      EncodedWordContext context,
      int encodingChar) {
      int i = index;
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7F || c == '?') {
          break;
        }
        if (context == EncodedWordContext.Comment &&
          (c == '(' || c == ')' || c == '\\')) {
          break;
        }
        if (context == EncodedWordContext.Phrase &&
          (encodingChar == 'Q' || encodingChar == 'q') &&
          (ValueSmallchars[c - 0x20] == 0 && c != '=')) {
          // NOTE: Smallchars excludes '=' to be consistent
          // with the same array used in HeaderEncoder.
          // We check for '=' here separately. This is
          // not a problem since we're finding out
          // what the encoded text is rather than decoding
          // that text directly.
          break;
        }
        if (context == EncodedWordContext.Phrase &&
          ValueAsciiAtext[c - 0x20] == 0) {
          break;
        }
        ++i;
      }
      return i;
    }

    public static string DecodeEncodedWords(
      string str,
      int index,
      int endIndex,
      EncodedWordContext context) {
      if (endIndex - index < 9) {
        // Too short for encoded words to appear
        return str.Substring(index, endIndex - index);
      }
      if (str.IndexOf('=') < 0) {
        // Contains no equal sign, and therefore no
        // encoded words
        return str.Substring(index, endIndex - index);
      }
      int start = index;
      var builder = new StringBuilder();
      var hasSuspiciousText = false;
      var lastWordWasEncodedWord = false;
      var whitespaceStart = -1;
      var whitespaceEnd = -1;
      var wordsWereDecoded = false;
      while (index < endIndex) {
        var charCount = 2;
        var acceptedEncodedWord = false;
        string decodedWord = null;
        var startIndex = 0;
        var havePossibleEncodedWord = false;
        var startParen = false;
        if (index + 1 < endIndex && str[index] == '=' &&
          str[index + 1] == '?') {
          startIndex = index + 2;
          index += 2;
          havePossibleEncodedWord = true;
        } else if (context == EncodedWordContext.Comment &&
          index + 2 < endIndex && str[index] == '(' &&
          str[index + 1] == '=' && str[index + 2] == '?') {
          startIndex = index + 3;
          index += 3;
          startParen = true;
          havePossibleEncodedWord = true;
        }
        if (havePossibleEncodedWord) {
          var maybeWord = true;
          int afterLast = endIndex;
          while (index < endIndex) {
            char c = str[index];
            ++index;
            // Check for a run of printable ASCII characters (except space)
            // with length up to 75 (also exclude '(' , '\', and ')' if the
            // context is a comment)
            if (c >= 0x21 && c < 0x7e && (context !=
                EncodedWordContext.Comment || (c != '(' && c != ')' &&
                  c != '\\'))) {
              ++charCount;
              if (charCount > 75) {
                maybeWord = false;
                index = startIndex - 2;
                break;
              }
            } else {
              afterLast = index - 1;
              break;
            }
          }
          if (maybeWord) {
            // May be an encoded word
            // DebugUtility.Log("maybe "
            // +str.Substring(startIndex-2, afterLast-(startIndex-2)));
            index = startIndex;
            int i2;
            // Parse charset
            // (NOTE: Compatible with RFC 2231's addition of language
            // to charset, since charset is defined as a 'token' in
            // RFC 2047, which includes '*')
            var charsetEnd = -1;
            var encodedTextStart = -1;
            var base64 = false;
            i2 = SkipCharsetOrEncoding(str, index, afterLast);
            if (i2 != index && i2 < endIndex && str[i2] == '?') {
              // Parse encoding
              charsetEnd = i2;
              index = i2 + 1;
              i2 = SkipCharsetOrEncoding(str, index, afterLast);
              if (i2 != index && i2 < endIndex && str[i2] == '?') {
                // check for supported encoding (B or Q)
                char encodingChar = str[index];
                if (i2 - index == 1 && (encodingChar == 'b' ||
                    encodingChar == 'B' ||
                    encodingChar == 'q' || encodingChar == 'Q')) {
                  // Parse encoded text
                  base64 = encodingChar == 'b' || encodingChar == 'B';
                  index = i2 + 1;
                  encodedTextStart = index;
                  i2 = SkipEncodedText(
                    str,
                    index,
                    afterLast,
                    context,
                    encodingChar);
                  if (i2 != index && i2 + 1 < endIndex &&
                    str[i2] == '?' && str[i2 + 1] == '=' &&
                    i2 + 2 == afterLast) {
                    acceptedEncodedWord = true;
                    i2 += 2;
                  }
                }
              }
            }
            if (acceptedEncodedWord) {
              string charset = str.Substring(
                startIndex,
                charsetEnd - startIndex);
              string encodedText = str.Substring(
                  encodedTextStart,
                  (afterLast - 2) - encodedTextStart);
              // DebugUtility.Log("enctext " + encodedText);
              int asterisk = charset.IndexOf('*');
              if (asterisk >= 1) {
                string language = charset.Substring(
                    asterisk + 1,
                    charset.Length - (asterisk + 1));
                charset = charset.Substring(0, asterisk);
                acceptedEncodedWord &=
                  LanguageTags.IsPotentiallyValidLanguageTag(language);
                } else {
                acceptedEncodedWord &= asterisk != 0;
              }
              if (acceptedEncodedWord) {
                IByteReader transform = base64 ? (IByteReader)new
BEncodingStringTransform(encodedText) :
                  (IByteReader)new QEncodingStringTransform(encodedText);

                ICharacterEncoding encoding = Encodings.GetEncoding(
                  charset,
                  true);
                // HACK
                if (encoding == null && (
                    charset.Equals("gb2312", StringComparison.Ordinal) ||
                    charset.Equals("GB2312", StringComparison.Ordinal))) {
                  encoding = Encodings.GetEncoding(
                    charset,
                    false);
                }
                if (encoding == null) {
                  // Console.WriteLine("Unknown charset " + charset);
                  decodedWord = str.Substring(
                      startIndex - 2,
                      afterLast - (startIndex - 2));
                  acceptedEncodedWord = false;
                } else {
                  // NOTE: This method converts to Unicode the text
                  // in encoded words
                  // one encoded word at a time; encoded word payloads
                  // spanning multiple encoded words in a row are not
                  // concatenated then converted to Unicode.
                  // ----
                  // Console.WriteLine("Encoded " + (base64 ? "B" : "Q") +
                  // " to: " + (encoding.GetString(transform)));
                  decodedWord = Encodings.DecodeToString(encoding, transform);
                  // Check for text in the decoded string
                  // that could render the comment syntactically invalid
                  // (the encoded
                  // word could even encode ASCII control characters and
                  // specials)
                  if (context == EncodedWordContext.Phrase &&
                    HasSuspiciousTextInStructured(decodedWord)) {
                    hasSuspiciousText = true;
                  } else {
                    hasSuspiciousText |= context ==
EncodedWordContext.Comment &&
                      HasSuspiciousTextInComments(decodedWord);
                  }
                  wordsWereDecoded = true;
                }
              } else {
                decodedWord = str.Substring(
                    startIndex - 2,
                    afterLast - (startIndex - 2));
              }
            } else {
              decodedWord = str.Substring(
                  startIndex - 2,
                  afterLast - (startIndex - 2));
            }
            index = afterLast;
          }
        }
        if (whitespaceStart >= 0 && whitespaceStart < whitespaceEnd &&
          (!acceptedEncodedWord || !lastWordWasEncodedWord)) {
          // Append whitespace as long as it doesn't occur between two
          // encoded words
          builder.Append(
            str.Substring(
              whitespaceStart,
              whitespaceEnd - whitespaceStart));
        }
        if (startParen) {
          builder.Append('(');
        }
        if (decodedWord != null) {
          builder.Append(decodedWord);
        }
        // Console.WriteLine(builder);
        // Console.WriteLine("" + index + " " + endIndex + " [" +
        // (index<endIndex ? str[index] : '~') + "]");
        // Read to whitespace
        int oldIndex = index;
        while (index < endIndex) {
          char c = str[index];
          if (c == 0x0d && index + 2 < endIndex && str[index + 1] == 0x0a &&
            (str[index + 2] == 0x09 || str[index + 2] == 0x20)) {
            index += 2; // skip the CRLF break;
          }
          if (c == 0x09 || c == 0x20) {
            break;
          }
          ++index;
        }
        whitespaceStart = index;
        // Read to nonwhitespace
        index = HeaderParser.ParseFWS(str, index, endIndex, null);
        whitespaceEnd = index;
        if (builder.Length == 0 && oldIndex == 0 && index == str.Length) {
          // Nothing to replace, and the whole string
          // is being checked
          return str;
        }
        if (oldIndex != index) {
          // Append nonwhitespace only, unless this is the end
          if (index == endIndex) {
            builder.Append(str.Substring(oldIndex, index - oldIndex));
          } else {
            string appendstr = str.Substring(
              oldIndex,
              whitespaceStart - oldIndex);
            builder.Append(appendstr);
          }
        }
        lastWordWasEncodedWord = acceptedEncodedWord;
      }
      string retval = builder.ToString();
      if (
        wordsWereDecoded && (
          hasSuspiciousText || (
            retval.IndexOf(
              "=?",
              StringComparison.Ordinal) >= 0 && retval.IndexOf(
              "?=",
              StringComparison.Ordinal) >= 0))) {
        if (context == EncodedWordContext.Comment) {
          string wrappedComment = "(" + retval + ")";
          if (
            HeaderParserUtility.ParseCommentStrict(
              wrappedComment,
              0,
              wrappedComment.Length) != wrappedComment.Length) {
            // Comment is syntactically invalid after decoding, so
            // don't decode any of the encoded words
            return str.Substring(start, endIndex - start);
          }
        }
        if (context == EncodedWordContext.Phrase) {
          if (HasAsciiCtl(retval)) {
            return str.Substring(start, endIndex - start);
          }
          // Quote the retval according to RFC 5322 rules
          var builder2 = new StringBuilder();
          builder2.Append('"');
          for (int i = 0; i < retval.Length; ++i) {
            if (retval[i] == '\\' || retval[i] == '"') {
              builder2.Append('\\');
              builder2.Append(retval[i]);
            } else {
              builder2.Append(retval[i]);
            }
          }
          builder2.Append('"');
          retval = builder2.ToString();
        }
      }

      return retval;
    }

    private static bool FollowedByEndOrLinearWhitespace(
      string str,
      int index,
      int endIndex) {
      if (index == endIndex) {
        return true;
      }
      if (str[index] != 0x09 && str[index] != 0x20 && str[index] != 0x0d) {
        return false;
      }
      int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
      return cws != index;
    }

    private static bool PrecededByStartOrLinearWhitespace(
      string str,
      int index) {
      return (index == 0) || (index - 1 >= 0 && (str[index - 1] == 0x09 ||
            str[index - 1] == 0x20));
    }

    private static int IndexOfNextPossibleEncodedWord(
      string str,
      int index,
      int endIndex) {
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

    public static string DecodePhraseText(
      string str,
      int index,
      int endIndex,
      IList<int[]> tokens,
      bool withComments) {
      // Assumes the value matches the production "phrase",
      // and assumes that endIndex is the end of all CFWS
      // found after the phrase.
      if (withComments) {
        int io = str.IndexOf("=?", index, StringComparison.Ordinal);
        if (io < 0 || io >= endIndex) {
          // No encoded words found in the specified area
          return str.Substring(index, endIndex - index);
        }
      }
      var builder = new StringBuilder();
      int lastIndex = index;
      var appendSpace = false;
      // Get each relevant token sorted by starting index
      foreach (int[] token in tokens) {
        var hasCFWS = false;
        if (
          !(token[1] >= lastIndex && token[1] >= index && token[1] <=
            endIndex && token[2] >= index && token[2] <= endIndex)) {
          continue;
        }
        if (token[0] == HeaderParserUtility.TokenComment && withComments) {
          // This is a comment token
          int startIndex = token[1];
          string appendstr = str.Substring(
            lastIndex,
            startIndex + 1 - lastIndex);
          builder.Append(appendstr);
          string newComment = Rfc2047.DecodeEncodedWords(
            str,
            startIndex + 1,
            token[2] - 1,
            EncodedWordContext.Comment);
          builder.Append(newComment);
          lastIndex = token[2] - 1;
        } else if (token[0] == HeaderParserUtility.TokenPhraseAtom) {
          // This is an atom token; only words within
          // a phrase can be encoded words; the first character
          // starts the actual atom rather than a comment or whitespace
          int wordStart = token[1];
          int wordEnd = wordStart;
          int previousWord = wordStart;
          if (wordStart < token[2] && str[wordStart] == '=') {
            // This word may be an encoded word
            wordEnd = wordStart;
            while (true) {
              if (!PrecededByStartOrLinearWhitespace(str, wordEnd)) {
                // The encoded word is not preceded by whitespace and
                // doesn't start the string, so it's not valid
                break;
              }
              // Find the end of the atom
              wordEnd = HeaderParser.ParsePhraseAtom(
                str,
                wordEnd,
                endIndex,
                null);
              if (!FollowedByEndOrLinearWhitespace(str, wordEnd, endIndex)) {
                // The encoded word is not followed by whitespace, so it's
                // not valid
                wordEnd = previousWord;
                break;
              }
              int nextWord = IndexOfNextPossibleEncodedWord(
                str,
                wordEnd,
                endIndex);
              if (nextWord < 0) {
                // The next word isn't an encoded word
                break;
              }
              previousWord = nextWord;
              wordEnd = nextWord;
            }
          }
          if (withComments) {
            builder.Append(str.Substring(lastIndex, wordStart - lastIndex));
          } else {
            if (appendSpace) {
              builder.Append(' ');
              appendSpace = false;
            }
          }
          if (wordStart == wordEnd) {
            wordEnd = token[2];
            builder.Append(str.Substring(wordStart, wordEnd - wordStart));
          } else {
            string replacement = Rfc2047.DecodeEncodedWords(
              str,
              wordStart,
              wordEnd,
              EncodedWordContext.Phrase);
            builder.Append(replacement);
          }
          hasCFWS = HeaderParser.ParseCFWS(str, wordEnd, endIndex, null) !=
            wordEnd;
          lastIndex = wordEnd;
        } else if (token[0] == HeaderParserUtility.TokenQuotedString &&
          !withComments) {
          if (appendSpace) {
            builder.Append(' ');
            appendSpace = false;
          }
          int tokenIndex = MediaType.SkipQuotedString(
            str,
            token[1],
            token[2],
            builder);
          // tokenIndex is now just after the end quote
          hasCFWS = HeaderParser.ParseCFWS(str, tokenIndex, endIndex, null) !=
            tokenIndex;
        }
        appendSpace |= hasCFWS;
      }
      if (withComments) {
        builder.Append(str.Substring(lastIndex, endIndex - lastIndex));
      }
      return builder.ToString();
    }

    private static void EncodePhraseTextInternal(
      string str,
      int index,
      int endIndex,
      HeaderEncoder enc) {
      // Assumes the value matches the production "phrase"
      // and that there are no comments in the value
      if (index == endIndex) {
        return; // Empty, so nothing to do
      }
      int index2 = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index2 == endIndex) {
        // Just linear whitespace
        enc.AppendString(str.Substring(index, endIndex - index));
        return;
      }
      if (!PrecededByStartOrLinearWhitespace(str, index2)) {
        // Append a space before the encoded words
        enc.AppendSpace();
      } else if (index != index2) {
        // Append the linear whitespace
        enc.AppendSpace();
      }
      var builderPhrase = new StringBuilder();
      index = index2;
      while (index < endIndex) {
        if (str[index] == '"') {
          // Quoted string
          index = MediaType.SkipQuotedString(
            str,
            index,
            endIndex,
            builderPhrase);
        } else if (str[index] == '.') {
          // Dot
          builderPhrase.Append('.');
          ++index;
        } else {
          // Atom
          index2 = HeaderParser.ParsePhraseAtom(
            str,
            index,
            endIndex,
            null);
          builderPhrase.Append(str.Substring(index, index2 - index));
          index = index2;
        }
        index2 = HeaderParser.ParseFWS(str, index, endIndex, null);
        if (index2 == endIndex) {
          enc.AppendAsEncodedWords(builderPhrase.ToString());
          if (index2 != index) {
            enc.AppendSpace();
          } else if (!FollowedByEndOrLinearWhitespace(
            str,
            endIndex,
            str.Length)) {
            // Add a space if no linear whitespace follows
            enc.AppendSpace();
          }
          break;
        }
        if (index2 != index) {
          builderPhrase.Append(' ');
        }
        index = index2;
      }
    }

    public static void EncodePhraseText(
      HeaderEncoder enc,
      string str,
      int index,
      int endIndex,
      IList<int[]> tokens) {
      // Assumes the value matches the production "phrase",
      // and assumes that endIndex is the end of all whitespace
      // found after the phrase. Doesn't encode text within comments.
      if (index == endIndex) {
        return;
      }
      if (!Message.HasTextToEscapeOrEncodedWordStarts(str, index, endIndex)) {
        // No need to use encoded words
        enc.AppendString(str.Substring(index, endIndex - index));
        return;
      }
      int lastIndex = index;
      foreach (int[] token in tokens) {
        if (token[1] < lastIndex || token[1] < index ||
          token[1] > endIndex || token[2] < index ||
          token[2] > endIndex) {
          continue;
        }
        if (token[0] == HeaderParserUtility.TokenComment) {
          // Process this piece of the phrase
          EncodePhraseTextInternal(str, lastIndex, token[1], enc);
          // Append the comment
          enc.AppendString(str.Substring(token[1], token[2] - token[1]));
          lastIndex = token[2];
        }
      }
      EncodePhraseTextInternal(str, lastIndex, endIndex, enc);
    }
  }
}
