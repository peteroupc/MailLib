/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 4/8/2014
 * Time: 3:50 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail
{
  internal class HeaderParserUtility
  {
    internal const int TokenComment = 2;
    internal const int TokenPhraseAtom = 3;
    internal const int TokenPhraseAtomOrDot = 4;
    internal const int TokenPhrase = 1;
    internal const int TokenGroup = 5;
    internal const int TokenMailbox = 6;
    internal const int TokenQuotedString = 7;
    internal const int TokenLocalPart = 8;
    internal const int TokenDomain = 9;

    private static string ParseDotAtomAfterCFWS(string str, int index, int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a dot-atom
      StringBuilder builder = new StringBuilder();
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

    private static string ParseDotWordAfterCFWS(string str, int index, int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a word separated by dots
      StringBuilder builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          if (index < endIndex && str[index] == '"') {
            // it's a quoted string instead
            index = MediaType.skipQuotedString(str, index, endIndex, builder);
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

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='endIndex'>A 32-bit signed integer. (2).</param>
    /// <returns>A string object.</returns>
    public static string ParseLocalPart(string str, int index, int endIndex) {
      // NOTE: Assumes the string matches the production "local-part"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      return ParseDotWordAfterCFWS(str, index, endIndex);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object. (2).</param>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='endIndex'>A 32-bit signed integer. (2).</param>
    /// <returns>A string object.</returns>
    public static string ParseDomain(string str, int index, int endIndex) {
      // NOTE: Assumes the string matches the production "domain"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index < endIndex && str[index] == '[') {
        // It's a domain literal
        ++index;
        StringBuilder builder = new StringBuilder();
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
              builder.Append(str.Substring(startQuote + 1, index - (startQuote + 1)));
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
      } else {
        // It's a dot-atom
        return ParseDotAtomAfterCFWS(str, index, endIndex);
      }
    }

    public static IList<NamedAddress> ParseAddressList(string str, int index, int endIndex, IList<int[]> tokens) {
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
            addresses.Add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
            lastIndex = tokenEnd;
          }
        }
      }
      return addresses;
    }

    public static NamedAddress ParseGroup(string str, int index, int endIndex, IList<int[]> tokens) {
      string displayName = null;
      bool haveDisplayName = false;
      IList<NamedAddress> mailboxes = new List<NamedAddress>();
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenPhrase && !haveDisplayName) {
            // Phrase
            displayName = Rfc2047.GetPhraseText(
              str,
              tokenIndex,
              tokenEnd,
              tokens,
              PhraseTextMode.DecodedText);
            // Set haveDisplayName, which needs to be done because
            // the mailboxes that follow may themselves have display names
            haveDisplayName = true;
          } else if (tokenKind == TokenMailbox) {
            mailboxes.Add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
          }
        }
      }
      #if DEBUG
      if (displayName == null) {
        throw new ArgumentNullException("displayName");
      }
      #endif

      return new NamedAddress(displayName, mailboxes);
    }

    public static NamedAddress ParseMailbox(string str, int index, int endIndex, IList<int[]> tokens) {
      string displayName = null;
      string localPart = null;
      string domain = null;
      for (int i = 0; i < tokens.Count; ++i) {
        int tokenIndex = tokens[i][1];
        int tokenEnd = tokens[i][2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens[i][0];
          if (tokenKind == TokenPhrase) {
            // Phrase
            displayName = Rfc2047.GetPhraseText(
              str,
              tokenIndex,
              tokenEnd,
              tokens,
              PhraseTextMode.DecodedText);
          } else if (tokenKind == TokenLocalPart) {
            localPart = ParseLocalPart(str, tokenIndex, tokenEnd);
          } else if (tokenKind == TokenDomain) {
            domain = ParseDomain(str, tokenIndex, tokenEnd);
          }
        }
      }
      #if DEBUG
      if (localPart == null) {
        throw new ArgumentNullException("localPart");
      }
      if (domain == null) {
        throw new ArgumentNullException("domain");
      }
      #endif

      return new NamedAddress(displayName, localPart, domain);
    }

    // Parses a comment using the obsolete syntax.
    internal static int ParseCommentLax(string str, int index, int endIndex, ITokener tokener) {
      if (index < endIndex && (str[index] == 40)) {
        ++index;
      } else {
        return index;
      }
      int indexStart = index;
      int depth = 0;
      int state = (tokener != null) ? tokener.GetState() : 0;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = HeaderParser.ParseFWS(str, index, endIndex, tokener);
            do {
              int indexTemp3 = index;
              do {
                if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
                  indexTemp3 += 2; break;
                }
                int indexTemp4;
                indexTemp4 = HeaderParser.ParseQuotedPair(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                if (index < endIndex && ((str[index] >= 1 && str[index] <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 && str[index] <= 31) || (str[index] == 127))) {
                  ++indexTemp3; break;
                }
                if (index < endIndex && ((str[index] >= 93 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 91) || (str[index] >= 33 && str[index] <= 39))) {
                  ++indexTemp3; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
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
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = HeaderParser.ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && str[index] == 41) {
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
      } while (true);
    }

    // Parses a comment without using the obsolete syntax.
    internal static int ParseCommentStrict(string str, int index, int endIndex) {
      if (index < endIndex && (str[index] == 40)) {
        ++index;
      } else {
        return index;
      }

      int indexStart = index;
      int depth = 0;
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
                    while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                      ++index;
                    }
                    if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
                      index += 2;
                    } else {
                      index = indexStart4; break;
                    }
                    indexTemp4 = index;
                    index = indexStart4;
                  } while (false);
                  if (indexTemp4 != index) {
                    index = indexTemp4;
                  } else { break;
                  }
                } while (false);
                if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                  ++index;
                  while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                    ++index;
                  }
                } else {
                  index = indexStart3; break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
                  indexTemp3 += 2; break;
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
                      if (index < endIndex && ((str[index] == 32) || (str[index] == 9) || (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 && str[index] <= 65535))) {
                        ++indexTemp5; break;
                      }
                      if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
                        indexTemp5 += 2; break;
                      }
                      if (index < endIndex && (str[index] >= 33 && str[index] <= 126)) {
                        ++indexTemp5; break;
                      }
                    } while (false);
                    if (indexTemp5 != index) {
                      index = indexTemp5;
                    } else {
                      index = indexStart4; break;
                    }
                  } while (false);
                  if (index == indexStart4) {
                    break;
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                if (index < endIndex && ((str[index] >= 93 && str[index] <= 126) || (str[index] >= 42 && str[index] <= 91) || (str[index] >= 33 && str[index] <= 39))) {
                  ++indexTemp3; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
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
                while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                  ++index;
                }
                if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
                  index += 2;
                } else {
                  index = indexStart3; break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
              ++index;
              while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
                ++index;
              }
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
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
