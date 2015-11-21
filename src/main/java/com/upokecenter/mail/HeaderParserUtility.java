package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.util.*;

  final class HeaderParserUtility {
private HeaderParserUtility() {
}
    static final int TokenComment = 2;
    static final int TokenPhraseAtom = 3;
    static final int TokenPhraseAtomOrDot = 4;
    static final int TokenPhrase = 1;
    static final int TokenGroup = 5;
    static final int TokenMailbox = 6;
    static final int TokenQuotedString = 7;
    static final int TokenLocalPart = 8;
    static final int TokenDomain = 9;

    private static boolean ShouldQuote(String str) {
      if (str.length() == 0) {
        // Empty String
        return true;
      }
      if (str.charAt(str.length() - 1) == ' ' || str.charAt(str.length() - 1) == '\t') {
        // Space or tab at end
        return true;
      }
      if (str.charAt(0) == ' ' || str.charAt(0) == '\t') {
        // Space or tab at beginning
        return true;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '\\' || str.charAt(i) == '"') {
          return true;
        }
        if ((str.charAt(i) == ' ' || str.charAt(i) == '\t') && i + 1 < str.length() &&
            (str.charAt(i + 1) == ' ' || str.charAt(i + 1) == '\t')) {
          // run of two or more space and/or tab
          return true;
        }
        if ((str.charAt(i) == '\r') && i + 1 < str.length() &&
            (str.charAt(i + 1) == '\n')) {
          // CRLF
          if (i == 0 && i + 2 < str.length() && (str.charAt(i + 1) == ' ' || str.charAt(i +
            1) == '\t')) {
            // CRLF followed by space or tab at beginning
            return true;
          }
          continue;
        }
        char c = str.charAt(i);
        // Has specials, or CTLs other than tab
        if ((c < 0x20 && c != '\t') || c == 0x7F || c == 0x28 || c == 0x29 ||
          c == 0x3c || c == 0x3e || c == 0x5b || c == 0x5d || c == 0x3a || c
            == 0x3b || c == 0x40 ||
              c == 0x5c || c == 0x2c || c == 0x2e || c == '"') {
          return true;
        }
      }
      return false;
    }

    public static String QuoteValueIfNeeded(String str) {
      return (!ShouldQuote(str)) ? str : QuoteValue(str);
    }

    public static String ParseGroupList(String str, int index, int endIndex) {
      // NOTE: Assumes the String matches the production "group"
      int tmp = HeaderParser.ParsePhrase(str, index, endIndex, null);
      if (tmp == index) {
        return "";
      }
      index = tmp;
      if (index < endIndex && str.charAt(index) == ':') {
        ++index;
      } else {
        return "";
      }
      tmp = HeaderParser.ParseGroupList(str, index, endIndex, null);
      return str.substring(index, (index)+(tmp - index));
    }

    public static String QuoteValue(String str) {
      // Quotes a String according to RFC 5322 rules.
      StringBuilder builder = new StringBuilder();
      builder.append('"');
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '\\' || str.charAt(i) == '"') {
          builder.append('\\');
          builder.append(str.charAt(i));
        } else {
          builder.append(str.charAt(i));
        }
      }
      builder.append('"');
      return builder.toString();
    }

    private static String ParseDotAtomAfterCFWS(
String str,
int index,
int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a dot-atom
      StringBuilder builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          break;
        }
        builder.append(str.substring(start, (start)+(index - start)));
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && str.charAt(index) == '.') {
          builder.append('.');
          ++index;
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
      }
      return builder.toString();
    }

    private static String ParseDotWordAfterCFWS(
String str,
int index,
int endIndex) {
      // NOTE: Also parses the obsolete syntax of CFWS between parts
      // of a word separated by dots
      StringBuilder builder = new StringBuilder();
      while (index < endIndex) {
        int start = index;
        index = HeaderParser.ParsePhraseAtom(str, index, endIndex, null);
        if (index == start) {
          if (index < endIndex && str.charAt(index) == '"') {
            // it's a quoted String instead
            index = MediaType.skipQuotedString(str, index, endIndex, builder);
          } else {
            break;
          }
        } else {
          builder.append(str.substring(start, (start)+(index - start)));
        }
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        if (index < endIndex && str.charAt(index) == '.') {
          builder.append('.');
          ++index;
          index = HeaderParser.ParseCFWS(str, index, endIndex, null);
        }
      }
      return builder.toString();
    }

    public static String ParseLocalPart(String str, int index, int endIndex) {
      // NOTE: Assumes the String matches the production "local-part"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      return ParseDotWordAfterCFWS(str, index, endIndex);
    }

    public static String ParseDomain(String str, int index, int endIndex) {
      // NOTE: Assumes the String matches the production "domain"
      index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (index < endIndex && str.charAt(index) == '[') {
        // It's a domain literal
        ++index;
        StringBuilder builder = new StringBuilder();
        builder.append('[');
        while (index < endIndex) {
          index = HeaderParser.ParseFWS(str, index, endIndex, null);
          if (index >= endIndex) {
            break;
          }
          if (str.charAt(index) == ']') {
            break;
          }
          if (str.charAt(index) == '\\') {
            int startQuote = index;
            index = HeaderParser.ParseQuotedPair(str, index, endIndex, null);
            if (index == startQuote) {
       builder.append(
str.substring(
startQuote + 1, (
startQuote + 1)+(index - (startQuote + 1))));
            } else {
              ++index;
            }
          } else {
            builder.append(str.charAt(index));
            ++index;
          }
        }
        builder.append(']');
        return builder.toString();
      }
      // It's a dot-atom
      return ParseDotAtomAfterCFWS(str, index, endIndex);
    }

    public static List<NamedAddress> ParseAddressList(
String str,
int index,
int endIndex,
List<int[]> tokens) {
      int lastIndex = index;
      List<NamedAddress> addresses = new ArrayList<NamedAddress>();
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= lastIndex && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
          if (tokenKind == TokenGroup) {
            addresses.add(ParseGroup(str, tokenIndex, tokenEnd, tokens));
            lastIndex = tokenEnd;
          } else if (tokenKind == TokenMailbox) {
            addresses.add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
            lastIndex = tokenEnd;
          }
        }
      }
      return addresses;
    }

    public static NamedAddress ParseAddress(
String str,
int index,
int endIndex,
List<int[]> tokens) {
      int lastIndex = index;
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= lastIndex && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
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
String str,
int index,
int endIndex,
List<int[]> tokens) {
      String displayName = null;
      boolean haveDisplayName = false;
      List<NamedAddress> mailboxes = new ArrayList<NamedAddress>();
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
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
            mailboxes.add(ParseMailbox(str, tokenIndex, tokenEnd, tokens));
          }
        }
      }
      return new NamedAddress(displayName, mailboxes);
    }

    public static NamedAddress ParseMailbox(
String str,
int index,
int endIndex,
List<int[]> tokens) {
      String displayName = null;
      String localPart = null;
      String domain = null;
      for (int i = 0; i < tokens.size(); ++i) {
        int tokenIndex = tokens.get(i)[1];
        int tokenEnd = tokens.get(i)[2];
        if (tokenIndex >= index && tokenIndex < endIndex) {
          int tokenKind = tokens.get(i)[0];
          if (tokenKind == TokenPhrase) {
            // Phrase
            displayName = Rfc2047.DecodePhraseText(
              str,
              tokenIndex,
              tokenEnd,
              tokens,
              false);
          } else if (tokenKind == TokenLocalPart) {
            localPart = ParseLocalPart(str, tokenIndex, tokenEnd);
          } else if (tokenKind == TokenDomain) {
            // NOTE: Domain will end up as the last domain token,
            // even if the mailbox contains obsolete route syntax
            domain = ParseDomain(str, tokenIndex, tokenEnd);
          }
        }
      }

      return new NamedAddress(displayName, localPart, domain);
    }

    // Parses a comment using the obsolete syntax.
    static int ParseCommentLax(
String str,
int index,
int endIndex,
ITokener tokener) {
      int indexStart = index;
      int depth = 0;
      if (index < endIndex && (str.charAt(index) == 40)) {
        ++index;
      } else {
        return index;
      }
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
                if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <=
                  55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
                  str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 &&
                  str.charAt(index + 1) <= 57343))) {
                  indexTemp3 += 2; break;
                }
                int indexTemp4;
      indexTemp4 = HeaderParser.ParseQuotedPair(
str,
index,
endIndex,
tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                if (index < endIndex && ((str.charAt(index) >= 1 && str.charAt(index) <=
                  8) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) ||
                  (str.charAt(index) >= 14 && str.charAt(index) <= 31) || (str.charAt(index) ==
                  127))) {
                  ++indexTemp3; break;
                }
                if (index < endIndex && ((str.charAt(index) >= 93 && str.charAt(index) <=
                  126) || (str.charAt(index) >= 42 && str.charAt(index) <= 91) ||
                  (str.charAt(index) >= 33 && str.charAt(index) <= 39))) {
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
        if (index < endIndex && str.charAt(index) == 41) {
          // End of current comment
          ++index;
          if (depth == 0) {
            if (tokener != null) {
              tokener.Commit(TokenComment, indexStart, index);
            }
            return index;
          }
          --depth;
        } else if (index < endIndex && str.charAt(index) == 40) {
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
  static int ParseCommentStrict(
String str,
int index,
int endIndex) {
      if (index < endIndex && (str.charAt(index) == 40)) {
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
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                    9))) {
                    ++index;
                    }
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) ==
                    10) {
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
           if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                  9))) {
                  ++index;
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                    9))) {
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
                if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <=
                  55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                  ++indexTemp3; break;
                }
                if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
                  str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 &&
                  str.charAt(index + 1) <= 57343))) {
                  indexTemp3 += 2; break;
                }
                int indexTemp4;
                indexTemp4 = index;
                do {
                  int indexStart4 = index;
                  if (index < endIndex && (str.charAt(index) == 92)) {
                    ++index;
                  } else {
                    break;
                  }
                  do {
                    int indexTemp5;
                    indexTemp5 = index;
                    do {
                    if (index < endIndex && ((str.charAt(index) == 32) ||
                    (str.charAt(index) == 9) || (str.charAt(index) >= 128 &&
                    str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 &&
                    str.charAt(index) <= 65535))) {
                    ++indexTemp5; break;
                    }
                    if (index + 1 < endIndex && ((str.charAt(index) >= 55296 &&
                    str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 &&
                    str.charAt(index + 1) <= 57343))) {
                    indexTemp5 += 2; break;
                    }
             if (index < endIndex && (str.charAt(index) >= 33 && str.charAt(index) <=
                    126)) {
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
                if (index < endIndex && ((str.charAt(index) >= 93 && str.charAt(index) <=
                  126) || (str.charAt(index) >= 42 && str.charAt(index) <= 91) ||
                  (str.charAt(index) >= 33 && str.charAt(index) <= 39))) {
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
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                  9))) {
                  ++index;
                }
        if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) ==
                  10) {
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
            if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
              ++index;
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) ==
                9))) {
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
        if (index < endIndex && str.charAt(index) == 41) {
          // End of current comment
          ++index;
          if (depth == 0) {
            return index;
          }
          --depth;
        } else if (index < endIndex && str.charAt(index) == 40) {
          // Start of nested comment
          ++index;
          ++depth;
        } else {
          return indexStart;
        }
      } while (true);
    }
  }
