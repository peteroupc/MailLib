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

namespace PeterO.Mail {
  internal static class HeaderParserUtility {
    internal const int TokenPhrase = 1;
    internal const int TokenComment = 2;
    internal const int TokenPhraseAtom = 3;
    internal const int TokenGroup = 4;
    internal const int TokenMailbox = 5;
    internal const int TokenQuotedString = 6;
    internal const int TokenLocalPart = 7;
    internal const int TokenDomain = 8;
    public static int ParseQuotedStringCore(
      string str,
      int index,
      int endIndex) {
      int indexStart, indexStart2, indexTemp2, tx3;
      indexStart = index;
      if (index < endIndex && (str[index] == 34)) {
        ++index;
      } else {
        {
          return indexStart;
        }
      }
      while (true) {
        indexTemp2 = index;
        do {
          indexStart2 = index;
          index = HeaderParser.ParseFWS(str, index, endIndex, null);
          tx3 = HeaderParser.ParseQcontent(str, index, endIndex, null);
          if (tx3 == index) {
            index = indexStart2;
            break;
          }
          index = tx3;
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          index = indexTemp2;
        } else {
          break;
        }
      }
      index = HeaderParser.ParseFWS(str, index, endIndex, null);
      if (index < endIndex && (str[index] == 34)) {
        ++index;
      } else {
        {
          return indexStart;
        }
      }
      return index;
    }

    public static bool HasComments(string str, int startIndex, int endIndex) {
      // Determines whether the string portion has comments.
      // Assumes the portion of the string is a syntactically valid
      // header field according to the Parse method of the header
      // field in question (except that comments may be allowed within white
      // space), and that parentheses can appear in that
      // field only within quoted strings or as comment delimiters.
      int index = startIndex;
      while (index < endIndex) {
        int c = str[index];
        if (c == 0x28 || c == 0x29) {
          // comment found
          return true;
        } else if (c == 0x22) {
          // quoted string found, skip it
          int
si = HeaderParserUtility.ParseQuotedStringCore(
      str,
      index,
      endIndex);
          if (si == index) {
            throw new InvalidOperationException("Internal error: " + str);
          }
          index = si;
        } else {
          ++index;
        }
      }
      return false;
    }

    public static void TraverseCFWSAndQuotedStrings(
      string str,
      int startIndex,
      int endIndex,
      ITokener tokener) {
      // Fills a tokener with "comment" and "quoted-string"
      // tokens. Assumes the portion of the string is a syntactically valid
      // header field according to the Parse method of the header
      // field in question.
      if (tokener != null) {
        int index = startIndex;
        while (index < endIndex) {
          int c = str[index];
          if (c == 0x20 || c == 0x0d || c == 0x0a || c == 0x28 || c == 0x29) {
            // Whitespace or parentheses
            int state = tokener.GetState();
            int si = HeaderParser.ParseCFWS(str, index, endIndex, tokener);
            if (si == index) {
              throw new InvalidOperationException("Internal error: " + str);
            }
            if (si < endIndex && str[si] == (char)0x22) {
              // Note that quoted-string starts with optional CFWS
              tokener.RestoreState(state);
              si = HeaderParser.ParseQuotedString(str, index, endIndex,
                tokener);
              if (si == index) {
                throw new InvalidOperationException("Internal error: " + str);
              }
            }
            index = si;
          } else if (c == 0x22) {
            int
 si = HeaderParser.ParseQuotedString(str, index, endIndex, tokener);
            if (si == index) {
              throw new InvalidOperationException("Internal error: " + str);
            }
            index = si;
          } else {
            ++index;
          }
        }
      }
    }

    public static string ParseGroupList(string str, int index, int endIndex) {
      // NOTE: Assumes the string matches the production "group"
      int tmp = HeaderParser.ParsePhrase(str, index, endIndex, null);
      if (tmp == index) {
        return String.Empty;
      }
      index = tmp;
      if (index < endIndex && str[index] == ':') {
        ++index;
      } else {
        return String.Empty;
      }
      tmp = HeaderParser.ParseGroupList(str, index, endIndex, null);
      return str.Substring(index, tmp - index);
    }

    private static string ParseDotAtomAfterCFWS(
      string str,
      int index,
      int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a dot-atom
      var builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          break;
        }
        builder.Append(str.Substring(start, index - start));
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && str[index] == '.') {
          builder.Append('.');
          ++index;
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
      }
      return builder.ToString();
    }

    private static string ParseDotWordAfterCFWS(
      string str,
      int index,
      int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a word separated by dots
      var builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          if (index < endIndex && str[index] == '"') {
            // it's a quoted string instead
            index = MediaType.SkipQuotedString(str, index, endIndex, builder);
          } else {
            break;
          }
        } else {
          builder.Append(str.Substring(start, index - start));
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && str[index] == '.') {
          builder.Append('.');
          ++index;
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
      }
      return builder.ToString();
    }

    public static string ParseLocalPart(string str, int index, int endIndex) {
      // NOTE: Assumes the string matches the production "local-part"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      return ParseDotWordAfterCFWS(str, index, endIndex);
    }

    public static string ParseDomain(string str, int index, int endIndex) {
      // NOTE: Assumes the string matches the production "domain"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index < endIndex && str[index] == '[') {
        // It's a domain literal
        ++index;
        var builder = new StringBuilder();
        builder.Append('[');
        while (index < endIndex) {
          index = HeaderParser.ParseFWS(str, index, endIndex, null);
          if (index >= endIndex) {
            break;
          }
          if (str[index] == ']') {
            break;
          }
          if (str[index] == '\\') {
            int startQuote = index;
            index = HeaderParser.ParseQuotedPair(str, index, endIndex, null);
            if (index == startQuote) {
              builder.Append(
         str.Substring(
         startQuote + 1,
         index - (startQuote + 1)));
       } else {
              ++index;
            }
          } else {
            builder.Append(str[index]);
            ++index;
          }
        }
        builder.Append(']');
        return builder.ToString();
      }
      // It's a dot-atom
      return ParseDotAtomAfterCFWS(str, index, endIndex);
    }

    public static IList<NamedAddress> ParseAddressList(
  string str,
  int index,
  int endIndex,
  IList<int[]> tokens) {
      int lastIndex = index;
      IList<NamedAddress> addresses = new List<NamedAddress>();
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= lastIndex && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenGroup) {
            addresses.Add(ParseGroup(str, tokenIndex, tokenEnd, tokens));
            lastIndex = tokenEnd;
          } else if (tokenKind == TokenMailbox) {
            // try {
            addresses.Add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
            // } catch (IndexOutOfRangeException ex) {
            // throw new InvalidOperationException(
              // "str=" + str + " index=" + index, // ex);
            // }
            lastIndex = tokenEnd;
          }
        }
      }
      return addresses;
    }

    public static int ParseWord(
      string str,
      int index,
      int endIndex,
      StringBuilder builder) {
      // NOTE: Assumes all CFWS has been read beforehand
      if (index == endIndex) {
        return index;
      }
      if (str[index] == '"') {
        // May be a quoted string
        return MediaType.SkipQuotedString(str, index, endIndex, builder);
      } else {
        // May be an atom
        int si = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (si != index) {
          builder.Append(str.Substring(index, si - index));
        }
        return si;
      }
    }

    public static NamedAddress ParseAddress(
  string str,
  int index,
  int endIndex,
  IList<int[]> tokens) {
      int lastIndex = index;
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= lastIndex && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenGroup) {
            return ParseGroup(str, tokenIndex, tokenEnd, tokens);
          }
          if (tokenKind == TokenMailbox) {
            return ParseMailbox(str, tokenIndex, tokenEnd, tokens);
          }
        }
      }
      return null;
    }

    public static NamedAddress ParseGroup(
  string str,
  int index,
  int endIndex,
  IList<int[]> tokens) {
      string displayName = null;
      var haveDisplayName = false;
      IList<NamedAddress> mailboxes = new List<NamedAddress>();
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenPhrase && !haveDisplayName) {
            // Phrase
            displayName = Rfc2047.DecodePhraseText(
              str,
              tokenIndex,
              tokenEnd,
              tokens,
              false);
            // Set haveDisplayName, which needs to be done because
            // the mailboxes that follow may themselves have display names
            haveDisplayName = true;
          } else if (tokenKind == TokenMailbox) {
            mailboxes.Add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
          }
        }
      }
      return new NamedAddress(displayName, mailboxes);
    }

    public static NamedAddress ParseMailbox(
  string str,
  int index,
  int endIndex,
  IList<int[]> tokens) {
      string displayName = null;
      string localPart = null;
      string domain = null;
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          switch (tokenKind) {
            case TokenPhrase:
              // Phrase
              displayName = Rfc2047.DecodePhraseText(
                str,
                tokenIndex,
                tokenEnd,
                tokens,
                false);
              break;
            case TokenLocalPart:
              localPart = ParseLocalPart(str, tokenIndex, tokenEnd);
              break;
            case TokenDomain:
              // NOTE: Domain will end up as the last domain token,
              // even if the mailbox contains obsolete route syntax
              domain = ParseDomain(str, tokenIndex, tokenEnd);
              break;
          }
        }
      }
#if DEBUG
      if (localPart == null) {
        throw new ArgumentException("localPart is null");
      }
      if (domain == null) {
        throw new ArgumentException("domain is null");
      }
#endif

      return new NamedAddress(displayName, localPart, domain);
    }

    // Parses a comment using the obsolete syntax.
    internal static int ParseCommentLax(
      string str,
      int index,
      int endIndex,
      ITokener tokener) {
      int indexStart = index;
      var depth = 0;
      if (index < endIndex && (str[index] == 40)) {
        ++index;
      } else {
        return index;
      }
      int state = (tokener != null) ? tokener.GetState() : 0;
      while (index < endIndex) {
        index = HeaderParser.ParseFWS(str, index, endIndex, null);
        bool backslash = index < endIndex && str[index] == '\\';
        if (backslash) {
          ++index;
        }
        if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <=
          56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
          index += 2;
        } else if (!backslash && index < endIndex && ((str[index] >= 1 &&
          str[index]
        <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14
            &&
     str[index] <= 31) || (str[index] >= 33 && str[index] <= 39) ||
            (str[index]
     >= 42 && str[index] <= 91) || (str[index] >= 93 && str[index] <= 55295) ||
            (str[index] >= 57344 && str[index] <= 65535))) {
          ++index;
        } else if (backslash && index < endIndex && ((str[index] >= 0 &&
          str[index]
          <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
          // NOTE: Includes parentheses, which are also handled
          // in later conditions
          ++index;
        } else if (index < endIndex && str[index] == 41) {
          // End of current comment
          ++index;
          if (depth == 0) {
            if (tokener != null) {
              tokener.Commit(TokenComment, indexStart, index);
            }
            return index;
          }
          --depth;
        } else if (index < endIndex && str[index] == 40) {
          // Start of nested comment
          ++index;
          ++depth;
        } else {
          if (tokener != null) {
            tokener.RestoreState(state);
          }
          return indexStart;
        }
      }
      if (tokener != null) {
        tokener.RestoreState(state);
      }
      return indexStart;
    }

    // Parses a comment without using the obsolete syntax.
    internal static int ParseCommentStrict(
      string str,
      int index,
      int endIndex) {
      if (index < endIndex && (str[index] == 40)) {
        ++index;
      } else {
        return index;
      }

      int indexStart = index;
      var depth = 0;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                do {
                  int indexTemp4;
                  indexTemp4 = index;
                  do {
                    int indexStart4 = index;
                    while (index < endIndex && ((str[index] == 32) ||
                (str[index] == 9))) {
                      ++index;
                    }
                    if (index + 1 < endIndex && str[index] == 13 && str[index +
                1] == 10) {
                      index += 2;
                    } else {
                      index = indexStart4;
                      break;
                    }
                    indexTemp4 = index;
                    index = indexStart4;
                  } while (false);
                  if (indexTemp4 != index) {
                    index = indexTemp4;
                  } else {
                    break;
                  }
                } while (false);
                if (index < endIndex && ((str[index] == 32) || (str[index] ==
                  9))) {
                  ++index;
                  while (index < endIndex && ((str[index] == 32) || (str[index]
                == 9))) {
                    ++index;
                  }
                } else {
                  index = indexStart3;
                  break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                break;
              }
            } while (false);
            do {
              int indexTemp3 = index;
              do {
                if (index < endIndex && ((str[index] >= 128 && str[index] <=
                  55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                  ++indexTemp3;
                  break;
                }
                if (index + 1 < endIndex && ((str[index] >= 55296 &&
                  str[index] <= 56319) && (str[index + 1] >= 56320 &&
                  str[index + 1] <= 57343))) {
                  indexTemp3 += 2;
                  break;
                }
                int indexTemp4;
                indexTemp4 = index;
                do {
                  int indexStart4 = index;
                  if (index < endIndex && (str[index] == 92)) {
                    ++index;
                  } else {
                    break;
                  }
                  do {
                    int indexTemp5;
                    indexTemp5 = index;
                    do {
                      if (index < endIndex && ((str[index] == 32) ||
                      (str[index] == 9) || (str[index] >= 128 &&
                      str[index] <= 55295) || (str[index] >= 57344 &&
                      str[index] <= 65535))) {
                        ++indexTemp5;
                        break;
                      }
                      if (index + 1 < endIndex && ((str[index] >= 55296 &&
                      str[index] <= 56319) && (str[index + 1] >= 56320 &&
                      str[index + 1] <= 57343))) {
                        indexTemp5 += 2;
                        break;
                      }
                      if (index < endIndex && (str[index] >= 33 && str[index] <=
                      126)) {
                        ++indexTemp5;
                        break;
                      }
                    } while (false);
                    if (indexTemp5 != index) {
                      index = indexTemp5;
                    } else {
                      index = indexStart4;
                      break;
                    }
                  } while (false);
                  if (index == indexStart4) {
                    break;
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4;
                  break;
                }
                if (index < endIndex && ((str[index] >= 93 && str[index] <=
                  126) || (str[index] >= 42 && str[index] <= 91) ||
                  (str[index] >= 33 && str[index] <= 39))) {
                  ++indexTemp3;
                  break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2;
                break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                while (index < endIndex && ((str[index] == 32) || (str[index] ==
                  9))) {
                  ++index;
                }
                if (index + 1 < endIndex && str[index] == 13 && str[index + 1]
                == 10) {
                  index += 2;
                } else {
                  index = indexStart3;
                  break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                break;
              }
            } while (false);
            if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
              ++index;
              while (index < endIndex && ((str[index] == 32) || (str[index] ==
                9))) {
                ++index;
              }
            } else {
              index = indexStart2;
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        } while (false);
        if (index < endIndex && str[index] == 41) {
          // End of current comment
          ++index;
          if (depth == 0) {
            return index;
          }
          --depth;
        } else if (index < endIndex && str[index] == 40) {
          // Start of nested comment
          ++index;
          ++depth;
        } else {
          return indexStart;
        }
      } while (true);
    }
  }
}
