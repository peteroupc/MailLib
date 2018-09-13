using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail {
  internal static class FormatFlowed {
    private static string HtmlEscape(string str) {
      var i = 0;
      for (; i < str.Length; ++i) {
        if (str[i] == '&' || str[i] == '<' || str[i] == '>') {
          break;
        }
      }
      if (i == str.Length) {
        return str;
      }
      var sb = new StringBuilder();
      sb.Append(str.Substring(0, i));
      for (; i < str.Length; ++i) {
        switch (str[i]) {
          case '&':
            sb.Append("&amp;");
            break;
          case '<':
            sb.Append("&lt;");
            break;
          case '>':
            sb.Append("&gt;");
            break;
          default:
            sb.Append(str[i]);
            break;
        }
      }
      return sb.ToString();
    }

    public static string NonFormatFlowedText(string str) {
      var sb = new StringBuilder();
      sb.Append("<pre>");
      sb.Append(HtmlEscape(str));
      sb.Append("</pre>");
      return sb.ToString();
    }

    public static string FormatFlowedText(string str, bool delSp) {
      var i = 0;
      int lineStart = i;
      var lastQuotes = 0;
      var paragraph = new StringBuilder();
      var formatted = new StringBuilder();
      var haveParagraph = false;
      while (i <= str.Length) {
        if (i == str.Length || (str[i] == 0x0d && i + 1 < str.Length &&
          str[i + 1] == 0x0a)) {
          bool lastLine = i == str.Length;
          int lineEnd = i;
          int index = lineStart;
          var quotes = 0;
          var flowed = false;
          var signature = false;
          while (index < lineEnd && str[index] == '>') {
            ++quotes;
            ++index;
          }
          if (index < lineEnd && str[index] == ' ') {
            // Space stuffing
            ++index;
          }
          if (index + 3 == lineEnd && str[index] == '-' &&
             str[index + 1] == '-' && str[index + 2] == ' ') {
            signature = true;
          } else if (index < lineEnd && str[lineEnd - 1] == ' ') {
            flowed = true;
            if (delSp) {
              --lineEnd;
            }
          }
          var endedParagraph = false;
          if (signature || lastQuotes != quotes) {
            // Signature line reached, or
            // change in quote depth, or fixed after flowed
            if (haveParagraph) {
              formatted.Append("<p>")
                    .Append(HtmlEscape(paragraph.ToString()))
                    .Append("</p>");
              haveParagraph = false;
              endedParagraph = true;
              paragraph.Remove(0, paragraph.Length);
            }
          } else if (haveParagraph && (!flowed || lastLine)) {
            formatted.Append("<p>").Append(HtmlEscape(paragraph.ToString()));
            haveParagraph = false;
            endedParagraph = true;
            paragraph.Remove(0, paragraph.Length);
            formatted.Append(
                HtmlEscape(str.Substring(index, lineEnd - index)))
                    .Append("</p>");
          }
          if (lastQuotes < quotes) {
            for (var k = lastQuotes; k < quotes; ++k) {
              formatted.Append("<blockquote>");
            }
          } else if (quotes < lastQuotes) {
            for (var k = quotes; k < lastQuotes; ++k) {
              formatted.Append("</blockquote>");
            }
          }
          if (!endedParagraph) {
            if (flowed) {
              haveParagraph = true;
              paragraph.Append(str.Substring(index, lineEnd - index));
            } else {
              // Line is fixed, or is a signature line
              if (signature) {
                formatted.Append("<hr/>");
              } else {
                if (index < lineEnd) {
                  formatted.Append("<tt>");
                  formatted.Append(
                    HtmlEscape(str.Substring(index, lineEnd - index)));
                  formatted.Append("</tt>");
                }
                if (!lastLine) {
                  formatted.Append("<br/>\r\n");
                }
              }
            }
          }
          if (i == str.Length && quotes > 0) {
            for (var k = 0; k < quotes; ++k) {
              formatted.Append("</blockquote>");
            }
          }
          lineStart = i + 2;
          lastQuotes = quotes;
          if (i == str.Length) {
            break;
          }
        }
        ++i;
      }
      return formatted.ToString();
    }

    private static bool IsQuoteLine(string str) {
      return !String.IsNullOrEmpty(str) && str[0] == '>';
    }

    private static bool IsBarLine(string str) {
      if (str == null || str.Length < 3) {
        return false;
      }
      if (str[0] != '-' && str[0] != '*' && str[0] != '_') {
        return false;
      }
      int count = 0;
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] != str[0]) {
          if (count < 3) count++;
        } else if (str[i] != ' ') {
          return false;
        }
      }
      return count >= 3;
    }

    private static bool IsEqualsLine(string str) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] != '=') {
          return false;
        }
      }
      return true;
    }

    private static bool IsDashLine(string str) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] != '-') {
          return false;
        }
      }
      return true;
    }

    private static bool IsBlankishLine(string str) {
      if (str == null || str.Length == 0) {
        return true;
      }
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] != '\t' && str[i] != ' ') {
          return false;
        }
      }
      return true;
    }

    private static bool IsHeadingLine(string str) {
      return !String.IsNullOrEmpty(str) && str[0] == '#';
    }

    private static int HeadingLevel(string str) {
      if (str == null) {
        return 0;
      }
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] != '#') {
          return Math.Min(i, 6);
        }
      }
      return Math.Min(str.Length, 6);
    }

    private static string TrimShortSpaces(string str) {
      if (str != null) {
        var i = 0;
        while (i < str.Length) {
          if (str[i] == ' ' || str[i] == '\t') {
            if (i >= 4) {
              return str;
            }
          } else if (i <= 3) {
            return str.Substring(i);
          }
          ++i;
        }
      }
      return str;
    }

    private static bool IsUnorderedListLine(string str) {
      str = TrimShortSpaces(str);
      return str != null && str.Length >= 2 &&
          (str[0] == '-' || str[0] == '*' || str[0] == '+') &&
          (str[1] == ' ' || str[1] == '\t');
    }

    private static bool IsOrderedListLine(string str) {
      return IsOrderedListLine(str, true);
    }


    private static bool IsOrderedListLine(string str, bool trim) {
      if (trim) str = TrimShortSpaces(str);
      if (str == null) return false;
      bool digit = false;
      int i = 0;
      while (i < str.Length) {
        if (str[i] >= '0' && str[i] <= '9') digit = true;
        else if (str[i] == '.') return digit;
        else return false;
        i++;
      }
      return false;
    }

    private static bool IsCodeBlockLine(string str) {
      if (String.IsNullOrEmpty(str)) return false;
      if (str.Length >= 1 && str[0] == '\t') return true;
      if (str.Length >= 4 && str[0] == ' ' &&
          str[1] == ' ' &&
          str[2] == ' ' &&
          str[3] == ' ') return true;
      return false;
    }
    private static string ReplaceTwoOrMoreSpacesWithBR(string str) {
      if (String.IsNullOrEmpty(str)) return String.Empty;
      if (str.Length >= 2 && str[str.Length - 1] == ' ' && str[str.Length - 2] == ' ') {
        int i = str.Length - 1;
        while (i >= 0) {
          if (str[i] != ' ') return str.Substring(0, i) + "<br/>";
        }
        return "<br/>";
      }
      return str;
    }

    private static string StripCodeBlockSpaces(string str) {
      if (String.IsNullOrEmpty(str)) return String.Empty;
      if (str.Length >= 1 && str[0] == '\t') return str.Substring(1);
      if (str.Length >= 4 && str[0] == ' ' &&
          str[1] == ' ' &&
          str[2] == ' ' &&
          str[3] == ' ') return str.Substring(4);
      return str;
    }

    private static string HtmlEscapeStrong(string str) {
      var i = 0;
      var sb = new StringBuilder();
      for (; i < str.Length; ++i) {
        switch (str[i]) {
          case '&':
            sb.Append("&amp;");
            break;
          case '<':
            sb.Append("&lt;");
            break;
          case '>':
            sb.Append("&gt;");
            break;
          case '\'':
            sb.Append("&#x27;");
            break;
          case '\"':
            sb.Append("&#x22;");
            break;
          case '_':
            sb.Append("&#x5f;");
            break;
          case '*':
            sb.Append("&#x2a;");
            break;
          case '\\':
            sb.Append("&#x5c;");
            break;
          case '`':
            sb.Append("&#x60;");
            break;
          default:
            sb.Append(str[i]);
            break;
        }
      }
      return sb.ToString();
    }

    private static bool StartsWith(string str, int index, string delim) {
      for (var i = 0; i < delim.Length; ++i) {
        if (index + i >= str.Length || str[index + i] != delim[i]) {
          return false;
        }
      }
      return true;
    }

    private static string FormatParagraph(
         string str,
         string delim,
         string tag,
         bool escaped) {
      if (delim.Length == 1 && str.IndexOf(delim[0]) < 0) {
        return str;
      }
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] == '<') {
          // Skip HTML markup tags
          sb.Append(str[i]);
          int qi = i + 1;
          while (qi < str.Length) {
            sb.Append(str[qi]);
            if (str[qi] == '>') {
              ++qi; break;
            }
            ++qi;
          }
          i = qi - 1;
          continue;
        } else if (StartsWith(str, i, delim)) {
          int qi = i + delim.Length;
          int textStart = qi;
          while (qi < str.Length) {
            if (StartsWith(str, qi, delim)) {
              break;
            }
            ++qi;
          }
          if (qi != str.Length) {
            string inner = str.Substring(textStart, qi - textStart);
            if (escaped) {
              inner = HtmlEscapeStrong(inner);
            }
            sb.Append('<').Append(tag).Append('>').Append(inner)
              .Append("</").Append(tag).Append('>');
            i = qi + delim.Length - 1;
            continue;
          }
        }
        sb.Append(str[i]);
      }
      return sb.ToString();
    }

    private static string ReplaceImageLinks(string str) {
      if (str.IndexOf('!') < 0) {
        return str;
      }
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] == '!' && i + 1 < str.Length && str[i + 1] == '[') {
          var found = false;
          int linkStart = i + 2;
          int index = i + 2;
          while (index < str.Length) {
            if (str[index] == ']') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.Append(str[i]);
            }
            continue;
          }
          string linkText = str.Substring(linkStart, index - linkStart);
          ++index;
          if (index >= str.Length || str[index] != '(') {
            sb.Append(str[i]);
            continue;
          }
          ++index;
          found = false;
          linkStart = index;
          while (index < str.Length) {
            if (str[index] == ')') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.Append(str[i]);
            }
            continue;
          }
          string urlText = str.Substring(linkStart, index - linkStart);
          sb.Append("<img src=\"")
          .Append(HtmlEscapeStrong(urlText)).Append("\" alt=\"")
          .Append(HtmlEscapeStrong(linkText)).Append("\" />");
          i = index;
        } else {
          sb.Append(str[i]);
        }
      }
      return sb.ToString();
    }

    private static string ReplaceInlineLinks(string str) {
      if (str.IndexOf('[') < 0) {
        return str;
      }
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] == '[') {
          var found = false;
          int linkStart = i + 1;
          int index = i + 1;
          while (index < str.Length) {
            if (str[index] == ']') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.Append(str[i]);
            }
            continue;
          }
          string linkText = str.Substring(linkStart, index - linkStart);
          ++index;
          if (index >= str.Length || str[index] != '(') {
            sb.Append(str[i]);
            continue;
          }
          ++index;
          found = false;
          linkStart = index;
          while (index < str.Length) {
            if (str[index] == ')') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.Append(str[i]);
            }
            continue;
          }
          string urlText = str.Substring(linkStart, index - linkStart);
          sb.Append("<a href=\"")
          .Append(HtmlEscapeStrong(urlText)).Append("\">")
          .Append(linkText).Append("</a>");
          i = index;
        } else {
          sb.Append(str[i]);
        }
      }
      return sb.ToString();
    }

    private static string ReplaceCodeSpansAndBackslashEscapes(
         string str) {
      if (str.IndexOf('`') < 0 && str.IndexOf('\\') < 0) {
        return str;
      }
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] == '\\' && i + 1 < str.Length &&
          "[](){}#+-_`\\*!.".IndexOf(str[i + 1]) >= 0) {
          int c = str[i + 1];
          string hex = "0123456789abcdef";
          sb.Append("&#x");
          if (c >= 0x10) {
            sb.Append(hex[(c >> 4) & 15]);
          }
          sb.Append(hex[c & 15]).Append(";");
          continue;
        }
        if (str[i] == '`') {
          int qi = i + 1;
          int backTicks = 1;
          int endBackTicks = 0;
          sb.Append("<code>");
          while (qi < str.Length) {
            if (str[qi] == '`') backTicks++;
            else break;
            qi++;
          }
          while (qi < str.Length) {
            if (str[qi] == '`') {
              endBackTicks++;
              if (endBackTicks >= backTicks) break;
            } else { endBackTicks = 0; }
            char c = str[qi];
            qi++;
            if ("[](){}#+-_`\\*!.<>&".IndexOf(str[qi]) >= 0) {
              string hex = "0123456789abcdef";
              sb.Append("&#x");
              if (c >= 0x10) {
                sb.Append(hex[(c >> 4) & 15]);
              }
              sb.Append(hex[c & 15]).Append(";");
            } else {
              sb.Append(str[qi]);
            }
          }
          i = qi;
          sb.Append("<code>");
          continue;
        }
        sb.Append(str[i]);
      }
      return sb.ToString();
    }


    private static string FormatParagraph(string str) {
      // TODO: Escape ampersand/LT if necessary
      // TODO: Automatic links
      // TODO: Reference-style link/image syntax
      str = ReplaceCodeSpansAndBackslashEscapes(str);
      str = ReplaceImageLinks(str);
      str = ReplaceInlineLinks(str);
      str = FormatParagraph(str, "__", "strong", false);
      str = FormatParagraph(str, "**", "strong", false);
      str = FormatParagraph(str, "_", "em", false);
      str = FormatParagraph(str, "*", "em", false);
      str = ReplaceTwoOrMoreSpacesWithBR(str);
      return str;
    }

    private static string StripHeadingStart(string str) {
      if (String.IsNullOrEmpty(str)) {
        return String.Empty;
      }
      var i = 0;
      while (i < str.Length && str[i] == '#') {
        ++i;
      }
      while (i < str.Length && (str[i] == ' ' || str[i] == '\t')) {
        ++i;
      }
      str = str.Substring(i);
      i = str.Length - 1;
      bool hashes = false;
      while (i >= 0 && str[i] == '#') {
        i--;
        hashes = true;
      }
      if (hashes) {
        while (i >= 0 && (str[i] == ' ' || str[i] == '\t')) {
          i--;
          hashes = true;
        }
      }
      return str.Substring(0, i);
    }
    private static string StripOrderedListItemStart(string str) {
      if (String.IsNullOrEmpty(str)) {
        return String.Empty;
      }
      if (IsOrderedListLine(str)) {
        str = TrimShortSpaces(str);
      }
      var i = 0;
      if (IsOrderedListLine(str, false)) {
        while (i < str.Length && (str[i] >= '0' && str[i] <= '9')) {
          ++i;
        }
        ++i;
        while (i < str.Length && (str[i] == ' ' || str[i] == '\t')) {
          ++i;
        }
      } else {
        while (i < str.Length && (str[i] == ' ' || str[i] == '\t') && i < 4) {
          ++i;
        }
      }
      return str.Substring(i);
    }

    private static string StripListItemStart(string str) {
      if (String.IsNullOrEmpty(str)) {
        return String.Empty;
      }
      if (IsUnorderedListLine(str)) {
        str = TrimShortSpaces(str);
      }
      var i = 0;
      if (str[i] == '*' || str[i] == '-' || str[i] == '+') {
        ++i;
        while (i < str.Length && (str[i] == ' ' || str[i] == '\t')) {
          ++i;
        }
      } else {
        while (i < str.Length && (str[i] == ' ' || str[i] == '\t') && i < 4) {
          ++i;
        }
      }
      return str.Substring(i);
    }

    private static string StripQuoteStart(string str) {
      if (str == null || str.Length < 2) {
        return String.Empty;
      }
      return (str[1] == ' ' || str[1] == '\t') ? str.Substring(2) :
        str.Substring(1);
    }

    public static string MarkdownText(string str, int depth) {
      return MarkdownText(str, depth, true);
    }

    public static bool IsListLine(string line, bool ordered) {
      return ordered ? IsOrderedListLine(line) :
                       IsUnorderedListLine(line);
    }

    public static string StripItemStart(string line, bool ordered) {
      return ordered ? StripOrderedListItemStart(line) :
                       StripListItemStart(line);
    }

    public static string MarkdownText(string str, int depth, bool alwaysUseParas) {
      var i = 0;
      int lineStart = i;
      var formatted = new StringBuilder();
      var lines = new List<string>();
      var paragraph = new StringBuilder();
      var haveParagraph = false;
      var isSingleParagraph = true;
      while (i <= str.Length) {
        if (i == str.Length ||
            (str[i] == 0x0d && i + 1 < str.Length && str[i + 1] == 0x0a)) {
          lines.Add(str.Substring(lineStart, i - lineStart));
          if (i == str.Length) {
            break;
          }
          lineStart = i + 2;
        }
        ++i;
      }
      i = 0;
      for (; i < lines.Count; ++i) {
        string line = lines[i];
        if (IsQuoteLine(line)) {
          // Quote
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.Append("<p>");
            formatted.Append(paragraph.ToString());
            formatted.Append("</p>");
          }
          int qi = i + 1;
          var qs = new StringBuilder()
              .Append(StripQuoteStart(line));
          bool haveAnotherQuoteLine = false;
          bool goToEndOfPara = false;
          while (qi < lines.Count) {
            line = lines[qi];
            if (IsQuoteLine(line)) {
              qs.Append("\r\n").Append(StripQuoteStart(line));
              haveAnotherQuoteLine = true;
              ++qi;
            } else if (!haveAnotherQuoteLine && !IsBlankishLine(line)) {
              goToEndOfPara = true;
              break;
            } else {
              break;
            }
          }
          if (goToEndOfPara) {
            while (qi < lines.Count) {
              line = lines[qi];
              if (IsQuoteLine(line)) {
                qs.Append("\r\n").Append(StripQuoteStart(line));
                ++qi;
              } else if (IsBlankishLine(line)) {
                break;
              } else {
                qs.Append("\r\n").Append(line);
                ++qi;
              }
            }
          }
          i = qi - 1;
          formatted.Append("<blockquote>");
          if (depth > 10) {
            formatted.Append("<pre>");
            formatted.Append(HtmlEscape(qs.ToString()));
            formatted.Append("</pre>");
          } else {
            formatted.Append(MarkdownText(qs.ToString(), depth + 1));
          }
          formatted.Append("</blockquote>");
        } else if (IsBarLine(line)) {
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.Append("<p>");
            formatted.Append(paragraph.ToString());
            formatted.Append("</p>");
          }
          formatted.Append("<hr/>");
        } else if (IsUnorderedListLine(line) || IsOrderedListLine(line)) {
          bool ordered = IsOrderedListLine(line);
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.Append("<p>");
            formatted.Append(paragraph.ToString());
            formatted.Append("</p>");
          }
          int qi = i + 1;
          string strippedLine = StripItemStart(line, ordered);
          var qs = new StringBuilder().Append(strippedLine);
          formatted.Append(ordered ? "<ol>" : "<ul>");
          formatted.Append("<li>");
          int itemLineCount = 0;
          bool seenBlankishLine = false;
          bool wrapLinesInParas = false;

          // DebugUtility.Log("newlist");
          while (qi < lines.Count) {
            line = lines[qi];
            if (IsListLine(line, ordered)) {
              if (seenBlankishLine) wrapLinesInParas = true;
              // DebugUtility.Log("para=" + qs.ToString());
              string qss2 = MarkdownText(qs.ToString(), depth + 1, wrapLinesInParas);
              formatted.Append(qss2);
              formatted.Append("</li><li>");
              qs.Remove(0, qs.Length);
              ++qi;
              strippedLine = StripItemStart(line, ordered);
              qs.Append(strippedLine);
              itemLineCount++;
              seenBlankishLine = false;
            } else {
              // DebugUtility.Log("[" + line + "]");
              // DebugUtility.Log("blankish=" + IsBlankishLine(line));
              if (IsBlankishLine(line)) {
                seenBlankishLine = true;
                ++qi;
              } else if (seenBlankishLine) {
                break;
              } else {
                seenBlankishLine = false;
                qs.Append("\r\n").Append(StripItemStart(line, ordered));

                // DebugUtility.Log("qs=" + qs);
                ++qi;
                itemLineCount++;
              }
            }
          }
          i = qi - 1;
          string qss = MarkdownText(qs.ToString(), depth + 1, wrapLinesInParas);
          formatted.Append(qss);
          formatted.Append("</li>");
          formatted.Append(ordered ? "</ol>" : "</ul>");
        } else if (IsHeadingLine(line)) {
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.Append("<p>");
            formatted.Append(paragraph.ToString());
            formatted.Append("</p>");
          }
          int heading = HeadingLevel(line);
          string stripped = StripHeadingStart(line);
          formatted.Append("<h").Append((char)('0' + heading))
               .Append(">").Append(FormatParagraph(stripped))
               .Append("</h").Append((char)('0' + heading))
               .Append(">");
        } else if (IsCodeBlockLine(line)) {
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.Append("<p>");
            formatted.Append(paragraph.ToString());
            formatted.Append("</p>");
          }
          int qi = i + 1;
          var qs = new StringBuilder()
              .Append(StripCodeBlockSpaces(line));
          while (qi < lines.Count) {
            line = lines[qi];
            if (IsCodeBlockLine(line)) {
              qs.Append("\r\n").Append(StripCodeBlockSpaces(line));
              ++qi;
            } else {
              break;
            }
          }
          i = qi - 1;
          formatted.Append("<pre><code>");
          formatted.Append(HtmlEscape(qs.ToString()));
          formatted.Append("</code></pre>");
        } else if (IsEqualsLine(line) && haveParagraph) {
          isSingleParagraph = false;
          haveParagraph = false;
          formatted.Append("<h1>");
          formatted.Append(paragraph.ToString());
          formatted.Append("</h1>");
        } else if (IsDashLine(line) && haveParagraph) {
          isSingleParagraph = false;
          haveParagraph = false;
          formatted.Append("<h2>");
          formatted.Append(paragraph.ToString());
          formatted.Append("</h2>");
        } else {
          if (!IsBlankishLine(line)) {
            if (haveParagraph) {
              paragraph.Append("\r\n");
            } else {
              paragraph.Remove(0, paragraph.Length);
            }
            haveParagraph = true;
            paragraph.Append(FormatParagraph(line));
          } else if (haveParagraph) {
            haveParagraph = false;
            isSingleParagraph = false;
            formatted.Append("<p>");
            formatted.Append(paragraph.ToString());
            formatted.Append("</p>");
          }
        }
      }
      if (haveParagraph) {
        haveParagraph = false;
        if (depth == 0 || !isSingleParagraph || alwaysUseParas) {
          formatted.Append("<p>");
          formatted.Append(paragraph.ToString());
          formatted.Append("</p>");
        } else {
          formatted.Append(paragraph.ToString());
        }
      }

      return formatted.ToString();
    }
  }
}
