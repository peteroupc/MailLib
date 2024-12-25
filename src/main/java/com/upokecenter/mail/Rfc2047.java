package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.mail.transforms.*;
import com.upokecenter.text.*;

  final class Rfc2047 {
private Rfc2047() {
}
    private static boolean HasSuspiciousTextInComments(String str) {
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if ((c < 0x20 && c != '\t') || c == '(' || c == ')' || c == '\\' ||
          c == 0x7f) {
          return true;
        }
      }
      return false;
    }

    private static boolean HasAsciiCtl(String str) {
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if ((c < 0x20 && c != '\t') || c == 0x7f) {
          return true;
        }
      }
      return false;
    }

    private static boolean HasSuspiciousTextInStructured(String str) {
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
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
      String str,
      int index,
      int endIndex) {
      // NOTE: Assumes that the comment is syntactically valid

      int length = endIndex - index;
      if (length < 2 || str.charAt(index) != '(' || str.charAt(endIndex - 1) != ')') {
        enc.AppendString(str.substring(index, (index)+(length - index)));
      }
      int nextComment = str.indexOf('(',index + 1);
      int nextBackslash = str.indexOf('\\',index + 1);
      // don't count comments or backslashes beyond
      // the desired portion
      if (nextComment >= endIndex) {
        nextComment = -1;
      }
      if (nextBackslash >= endIndex) {
        nextBackslash = -1;
      }
      boolean haveEscape = nextBackslash >= 0;
      if (!haveEscape) {
        // Check for possible folding whitespace
        nextBackslash = str.indexOf('\n',index + 1);
        if (nextBackslash >= endIndex) {
          nextBackslash = -1;
        }
        haveEscape = nextBackslash >= 0;
      }
      if (nextComment < 0 && nextBackslash < 0) {
        // No escapes or nested comments, so it's relatively easy
        enc.AppendSymbol("(");
        if (length > 2) {
          enc.AppendAsEncodedWords(str.substring(index + 1, (index + 1)+(length - 2)));
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
            if (str.charAt(index) == '(' || str.charAt(index) == ')') {
              ++index;
            } else {
              break;
            }
          }
          // Get the next run of nonparentheses
          int parenEnd = index;
          while (index < endIndex) {
            if (str.charAt(index) == '(' || str.charAt(index) == ')') {
              break;
            }
            ++index;
          }
          if (parenEnd == index) {
            for (int k = parenStart; k < parenEnd; ++k) {
              enc.AppendSymbol(str.substring(k, (k)+(1)));
            }
            break;
          }
          for (int k = parenStart; k < parenEnd; ++k) {
            enc.AppendSymbol(str.substring(k, (k)+(1)));
          }
          enc.AppendAsEncodedWords(str.substring(
            parenEnd, (
            parenEnd)+(index - parenEnd)));
        }
        return;
      }
      StringBuilder builder = new StringBuilder();
      // escapes, but no nested comments
      if (nextComment < 0) {
        // skip the first parenthesis
        ++index;
        while (index < endIndex) {
          if (str.charAt(index) == ')') {
            // End of the comment
            break;
          }
          if (str.charAt(index) == '\r' && index + 2 < endIndex &&
            str.charAt(index + 1) == '\n' && (str.charAt(index + 2) == 0x20 || str.charAt(index +
2) ==
              0x09)) {
            // Folding whitespace
            builder.append(str.charAt(index + 2));
            index += 3;
          } else if (str.charAt(index) == '\\' && index + 1 < endIndex) {
            // Quoted pair
            int cp = com.upokecenter.util.DataUtilities.CodePointAt(str, index + 1);
            if (cp <= 0xffff) {
              {
                builder.append((char)cp);
              }
            } else if (cp <= 0x10ffff) {
              builder.append((char)((((cp - 0x10000) >> 10) & 0x3ff) |
0xd800));
              builder.append((char)(((cp - 0x10000) & 0x3ff) | 0xdc00));
            }
            index += 1 + (cp >= 0x10000 ? 2 : 1);
          } else {
            // Other comment text
            builder.append(str.charAt(index));
            ++index;
          }
        }
        enc.AppendSymbol("(");
        enc.AppendAsEncodedWords(builder.toString());
        enc.AppendSymbol(")");
        return;
      }
      // escapes and nested comments
      while (true) {
        int parenStart = index;
        // Get the next run of parentheses
        while (index < endIndex) {
          if (str.charAt(index) == '(' || str.charAt(index) == ')') {
            ++index;
          } else {
            break;
          }
        }
        // Get the next run of nonparentheses
        int parenEnd = index;
        builder.delete(
          0, (
          0)+(builder.length()));
        while (index < endIndex) {
          if (str.charAt(index) == '(' || str.charAt(index) == ')') {
            break;
          }
          if (str.charAt(index) == '\r' && index + 2 < endIndex &&
            str.charAt(index + 1) == '\n' && (str.charAt(index + 2) == 0x20 || str.charAt(index +
2) ==
              0x09)) {
            // Folding whitespace
            builder.append(str.charAt(index + 2));
            index += 3;
          } else if (str.charAt(index) == '\\' && index + 1 < endIndex) {
            // Quoted pair
            int cp = com.upokecenter.util.DataUtilities.CodePointAt(str, index + 1);
            if (cp <= 0xffff) {
              builder.append((char)cp);
            } else if (cp <= 0x10ffff) {
              builder.append((char)((((cp - 0x10000) >> 10) & 0x3ff) |
0xd800));
              builder.append((char)(((cp - 0x10000) & 0x3ff) | 0xdc00));
            }
            index += 1 + (cp >= 0x10000 ? 2 : 1);
          } else {
            // Other comment text
            builder.append(str.charAt(index));
            ++index;
          }
        }
        if (builder.length() == 0) {
          for (int k = parenStart; k < parenEnd; ++k) {
            enc.AppendSymbol(str.substring(k, (k)+(1)));
          }
          break;
        }
        for (int k = parenStart; k < parenEnd; ++k) {
          enc.AppendSymbol(str.substring(k, (k)+(1)));
        }
        enc.AppendAsEncodedWords(builder.toString());
      }
      return;
    }

    private static int SkipCharsetOrEncoding(
      String str,
      int index,
      int endIndex) {
      String ValueSpecials = "()<>[]@,;:\\\"/?=.";
      int i = index;
      while (i < endIndex) {
        char c = str.charAt(i);
        if (c <= 0x20 || c >= 0x7f || ((c & 0x7f) == c &&
            ValueSpecials.indexOf(c) >= 0)) {
          break;
        }
        ++i;
      }
      return i;
    }

    // See point 3 of RFC 2047 sec. 5 (but excludes '=')
    private static final int[] ValueSmallchars = {
      0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    };

    // ASCII characters allowed in atoms
    private static final int[] ValueAsciiAtext = {
      0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1,
      0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
    };

    private static int SkipEncodedText(
      String str,
      int index,
      int endIndex,
      EncodedWordContext context,
      int encodingChar) {
      int i = index;
      while (i < endIndex) {
        char c = str.charAt(i);
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

    public static String DecodeEncodedWords(
      String str,
      int index,
      int endIndex,
      EncodedWordContext context) {
      if (endIndex - index < 9) {
        // Too short for encoded words to appear
        return str.substring(index, (index)+(endIndex - index));
      }
      if (str.indexOf('=') < 0) {
        // Contains no equal sign, and therefore no
        // encoded words
        return str.substring(index, (index)+(endIndex - index));
      }
      int start = index;
      StringBuilder builder = new StringBuilder();
      boolean hasSuspiciousText = false;
      boolean lastWordWasEncodedWord = false;
      int whitespaceStart = -1;
      int whitespaceEnd = -1;
      boolean wordsWereDecoded = false;
      while (index < endIndex) {
        int charCount = 2;
        boolean acceptedEncodedWord = false;
        String decodedWord = null;
        int startIndex = 0;
        boolean havePossibleEncodedWord = false;
        boolean startParen = false;
        if (index + 1 < endIndex && str.charAt(index) == '=' &&
          str.charAt(index + 1) == '?') {
          startIndex = index + 2;
          index += 2;
          havePossibleEncodedWord = true;
        } else if (context == EncodedWordContext.Comment &&
          index + 2 < endIndex && str.charAt(index) == '(' &&
          str.charAt(index + 1) == '=' && str.charAt(index + 2) == '?') {
          startIndex = index + 3;
          index += 3;
          startParen = true;
          havePossibleEncodedWord = true;
        }
        if (havePossibleEncodedWord) {
          boolean maybeWord = true;
          int afterLast = endIndex;
          while (index < endIndex) {
            char c = str.charAt(index);
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
            // System.out.println("maybe "
            // +str.substring(startIndex-2, (startIndex-2)+(afterLast-(startIndex-2))));
            index = startIndex;
            int i2;
            // Parse charset
            // (NOTE: Compatible with RFC 2231's addition of language
            // to charset, since charset is defined as a 'token' in
            // RFC 2047, which includes '*')
            int charsetEnd = -1;
            int encodedTextStart = -1;
            boolean base64 = false;
            i2 = SkipCharsetOrEncoding(str, index, afterLast);
            if (i2 != index && i2 < endIndex && str.charAt(i2) == '?') {
              // Parse encoding
              charsetEnd = i2;
              index = i2 + 1;
              i2 = SkipCharsetOrEncoding(str, index, afterLast);
              if (i2 != index && i2 < endIndex && str.charAt(i2) == '?') {
                // check for supported encoding (B or Q)
                char encodingChar = str.charAt(index);
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
                    str.charAt(i2) == '?' && str.charAt(i2 + 1) == '=' &&
                    i2 + 2 == afterLast) {
                    acceptedEncodedWord = true;
                    i2 += 2;
                  }
                }
              }
            }
            if (acceptedEncodedWord) {
              String charset = str.substring(
                startIndex, (
                startIndex)+(charsetEnd - startIndex));
              String encodedText = str.substring(
                  encodedTextStart, (
                  encodedTextStart)+((afterLast - 2) - encodedTextStart));
              // System.out.println("enctext " + encodedText);
              int asterisk = charset.indexOf('*');
              if (asterisk >= 1) {
                String language = charset.substring(
                    asterisk + 1, (
                    asterisk + 1)+(charset.length() - (asterisk + 1)));
                charset = charset.substring(0, asterisk);
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
                    charset.equals("gb2312") ||
                    charset.equals("GB2312"))) {
                  encoding = Encodings.GetEncoding(
                    charset,
                    false);
                }
                if (encoding == null) {
                  // System.out.println("Unknown charset " + charset);
                  decodedWord = str.substring(
                      startIndex - 2, (
                      startIndex - 2)+(afterLast - (startIndex - 2)));
                  acceptedEncodedWord = false;
                } else {
                  // NOTE: This method converts to Unicode the text
                  // in encoded words
                  // one encoded word at a time; encoded word payloads
                  // spanning multiple encoded words in a row are not
                  // concatenated then converted to Unicode.
                  // ----
                  // System.out.println("Encoded " + (base64 ? "B" : "Q") +
                  // " to: " + (encoding.GetString(transform)));
                  decodedWord = Encodings.DecodeToString(encoding, transform);
                  // Check for text in the decoded String
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
                decodedWord = str.substring(
                    startIndex - 2, (
                    startIndex - 2)+(afterLast - (startIndex - 2)));
              }
            } else {
              decodedWord = str.substring(
                  startIndex - 2, (
                  startIndex - 2)+(afterLast - (startIndex - 2)));
            }
            index = afterLast;
          }
        }
        if (whitespaceStart >= 0 && whitespaceStart < whitespaceEnd &&
          (!acceptedEncodedWord || !lastWordWasEncodedWord)) {
          // Append whitespace as long as it doesn't occur between two
          // encoded words
          builder.append(
            str.substring(
              whitespaceStart, (
              whitespaceStart)+(whitespaceEnd - whitespaceStart)));
        }
        if (startParen) {
          builder.append('(');
        }
        if (decodedWord != null) {
          builder.append(decodedWord);
        }
        // System.out.println(builder);
        // System.out.println("" + index + " " + endIndex + " [" +
        // (index<endIndex ? str.charAt(index) : '~') + "]");
        // Read to whitespace
        int oldIndex = index;
        while (index < endIndex) {
          char c = str.charAt(index);
          if (c == 0x0d && index + 2 < endIndex && str.charAt(index + 1) == 0x0a &&
            (str.charAt(index + 2) == 0x09 || str.charAt(index + 2) == 0x20)) {
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
        if (builder.length() == 0 && oldIndex == 0 && index == str.length()) {
          // Nothing to replace, and the whole String
          // is being checked
          return str;
        }
        if (oldIndex != index) {
          // Append nonwhitespace only, unless this is the end
          if (index == endIndex) {
            builder.append(str.substring(oldIndex, (oldIndex)+(index - oldIndex)));
          } else {
            String appendstr = str.substring(
              oldIndex, (
              oldIndex)+(whitespaceStart - oldIndex));
            builder.append(appendstr);
          }
        }
        lastWordWasEncodedWord = acceptedEncodedWord;
      }
      String retval = builder.toString();
      if (
        wordsWereDecoded && (
          hasSuspiciousText || (
            retval.indexOf(
              "=?") >= 0 && retval.indexOf(
              "?=") >= 0))) {
        if (context == EncodedWordContext.Comment) {
          String wrappedComment = "(" + retval + ")";
          if (
            HeaderParserUtility.ParseCommentStrict(
              wrappedComment,
              0,
              wrappedComment.length()) != wrappedComment.length()) {
            // Comment is syntactically invalid after decoding, so
            // don't decode any of the encoded words
            return str.substring(start, (start)+(endIndex - start));
          }
        }
        if (context == EncodedWordContext.Phrase) {
          if (HasAsciiCtl(retval)) {
            return str.substring(start, (start)+(endIndex - start));
          }
          // Quote the retval according to RFC 5322 rules
          StringBuilder builder2 = new StringBuilder();
          builder2.append('"');
          for (int i = 0; i < retval.length(); ++i) {
            if (retval.charAt(i) == '\\' || retval.charAt(i) == '"') {
              builder2.append('\\');
              builder2.append(retval.charAt(i));
            } else {
              builder2.append(retval.charAt(i));
            }
          }
          builder2.append('"');
          retval = builder2.toString();
        }
      }

      return retval;
    }

    private static boolean FollowedByEndOrLinearWhitespace(
      String str,
      int index,
      int endIndex) {
      if (index == endIndex) {
        return true;
      }
      if (str.charAt(index) != 0x09 && str.charAt(index) != 0x20 && str.charAt(index) != 0x0d) {
        return false;
      }
      int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
      return cws != index;
    }

    private static boolean PrecededByStartOrLinearWhitespace(
      String str,
      int index) {
      return (index == 0) || (index - 1 >= 0 && (str.charAt(index - 1) == 0x09 ||
            str.charAt(index - 1) == 0x20));
    }

    private static int IndexOfNextPossibleEncodedWord(
      String str,
      int index,
      int endIndex) {
      int cws = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (cws == index) {
        // No linear whitespace
        return -1;
      }
      while (index < cws) {
        if (str.charAt(index) == '(') {
          // Has a comment, so no encoded word
          // immediately follows
          return -1;
        }
        ++index;
      }
      if (index + 1 < endIndex && str.charAt(index) == '=' && str.charAt(index + 1) == '?') {
        // Has a possible encoded word
        return index;
      }
      return -1;
    }

    public static String DecodePhraseText(
      String str,
      int index,
      int endIndex,
      List<int[]> tokens,
      boolean withComments) {
      // Assumes the value matches the production "phrase",
      // and assumes that endIndex is the end of all CFWS
      // found after the phrase.
      if (withComments) {
        int io = str.indexOf("=?",index);
        if (io < 0 || io >= endIndex) {
          // No encoded words found in the given area
          return str.substring(index, (index)+(endIndex - index));
        }
      }
      StringBuilder builder = new StringBuilder();
      int lastIndex = index;
      boolean appendSpace = false;
      // Get each relevant token sorted by starting index
      for (int[] token : tokens) {
        boolean hasCFWS = false;
        if (
          !(token[1] >= lastIndex && token[1] >= index && token[1] <=
            endIndex && token[2] >= index && token[2] <= endIndex)) {
          continue;
        }
        if (token[0] == HeaderParserUtility.TokenComment && withComments) {
          // This is a comment token
          int startIndex = token[1];
          String appendstr = str.substring(
            lastIndex, (
            lastIndex)+(startIndex + 1 - lastIndex));
          builder.append(appendstr);
          String newComment = Rfc2047.DecodeEncodedWords(
            str,
            startIndex + 1,
            token[2] - 1,
            EncodedWordContext.Comment);
          builder.append(newComment);
          lastIndex = token[2] - 1;
        } else if (token[0] == HeaderParserUtility.TokenPhraseAtom) {
          // This is an atom token; only words within
          // a phrase can be encoded words; the first character
          // starts the actual atom rather than a comment or whitespace
          int wordStart = token[1];
          int wordEnd = wordStart;
          int previousWord = wordStart;
          if (wordStart < token[2] && str.charAt(wordStart) == '=') {
            // This word may be an encoded word
            wordEnd = wordStart;
            while (true) {
              if (!PrecededByStartOrLinearWhitespace(str, wordEnd)) {
                // The encoded word is not preceded by whitespace and
                // doesn't start the String, so it's not valid
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
            builder.append(str.substring(lastIndex, (lastIndex)+(wordStart - lastIndex)));
          } else {
            if (appendSpace) {
              builder.append(' ');
              appendSpace = false;
            }
          }
          if (wordStart == wordEnd) {
            wordEnd = token[2];
            builder.append(str.substring(wordStart, (wordStart)+(wordEnd - wordStart)));
          } else {
            String replacement = Rfc2047.DecodeEncodedWords(
              str,
              wordStart,
              wordEnd,
              EncodedWordContext.Phrase);
            builder.append(replacement);
          }
          hasCFWS = HeaderParser.ParseCFWS(str, wordEnd, endIndex, null) !=
            wordEnd;
          lastIndex = wordEnd;
        } else if (token[0] == HeaderParserUtility.TokenQuotedString &&
          !withComments) {
          if (appendSpace) {
            builder.append(' ');
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
        builder.append(str.substring(lastIndex, (lastIndex)+(endIndex - lastIndex)));
      }
      return builder.toString();
    }

    private static void EncodePhraseTextInternal(
      String str,
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
        enc.AppendString(str.substring(index, (index)+(endIndex - index)));
        return;
      }
      if (!PrecededByStartOrLinearWhitespace(str, index2)) {
        // Append a space before the encoded words
        enc.AppendSpace();
      } else if (index != index2) {
        // Append the linear whitespace
        enc.AppendSpace();
      }
      StringBuilder builderPhrase = new StringBuilder();
      index = index2;
      while (index < endIndex) {
        if (str.charAt(index) == '"') {
          // Quoted String
          index = MediaType.SkipQuotedString(
            str,
            index,
            endIndex,
            builderPhrase);
        } else if (str.charAt(index) == '.') {
          // Dot
          builderPhrase.append('.');
          ++index;
        } else {
          // Atom
          index2 = HeaderParser.ParsePhraseAtom(
            str,
            index,
            endIndex,
            null);
          builderPhrase.append(str.substring(index, (index)+(index2 - index)));
          index = index2;
        }
        index2 = HeaderParser.ParseFWS(str, index, endIndex, null);
        if (index2 == endIndex) {
          enc.AppendAsEncodedWords(builderPhrase.toString());
          if (index2 != index) {
            enc.AppendSpace();
          } else if (!FollowedByEndOrLinearWhitespace(
            str,
            endIndex,
            str.length())) {
            // Add a space if no linear whitespace follows
            enc.AppendSpace();
          }
          break;
        }
        if (index2 != index) {
          builderPhrase.append(' ');
        }
        index = index2;
      }
    }

    public static void EncodePhraseText(
      HeaderEncoder enc,
      String str,
      int index,
      int endIndex,
      List<int[]> tokens) {
      // Assumes the value matches the production "phrase",
      // and assumes that endIndex is the end of all whitespace
      // found after the phrase. Doesn't encode text within comments.
      if (index == endIndex) {
        return;
      }
      if (!Message.HasTextToEscapeOrEncodedWordStarts(str, index, endIndex)) {
        // No need to use encoded words
        enc.AppendString(str.substring(index, (index)+(endIndex - index)));
        return;
      }
      int lastIndex = index;
      for (int[] token : tokens) {
        if (token[1] < lastIndex || token[1] < index ||
          token[1] > endIndex || token[2] < index ||
          token[2] > endIndex) {
          continue;
        }
        if (token[0] == HeaderParserUtility.TokenComment) {
          // Process this piece of the phrase
          EncodePhraseTextInternal(str, lastIndex, token[1], enc);
          // Append the comment
          enc.AppendString(str.substring(token[1], (token[1])+(token[2] - token[1])));
          lastIndex = token[2];
        }
      }
      EncodePhraseTextInternal(str, lastIndex, endIndex, enc);
    }
  }
