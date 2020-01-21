using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace PeterO.Mail {
  internal static class FormatFlowed {
    private static void AppendHtmlEscape(StringBuilder sb, string str) {
      var i = 0;
      for (; i < str.Length; ++i) {
        if (str[i] == '&' || str[i] == '<' || str[i] == '>') {
          break;
        }
      }
      if (i == str.Length) {
        sb.Append(str);
        return;
      }
      sb.Append(str, 0, i);
      int lastI = i;
      for (; i < str.Length; ++i) {
        switch (str[i]) {
          case '&':
            if (lastI < i) {
              sb.Append(str, lastI, i - lastI);
              lastI = i + 1;
            }
            sb.Append("&amp;");
            break;
          case '<':
            if (lastI < i) {
              sb.Append(str, lastI, i - lastI);
              lastI = i + 1;
            }
            sb.Append("&lt;");
            break;
          case '>':
            if (lastI < i) {
              sb.Append(str, lastI, i - lastI);
              lastI = i + 1;
            }
            sb.Append("&gt;");
            break;
        }
      }
      if (lastI < str.Length) {
        sb.Append(str, lastI, str.Length - lastI);
      }
    }

    public static string NonFormatFlowedText(string str) {
      int sblength = str.Length < Int32.MaxValue / 2 ?
        Math.Min(str.Length * 2, str.Length + 64) :
        str.Length;
      var sb = new StringBuilder(sblength);
      sb.Append("<pre>");
      AppendHtmlEscape(sb, str);
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
              formatted.Append("<p>");
              AppendHtmlEscape(formatted, paragraph.ToString());
              formatted.Append("</p>");
              haveParagraph = false;
              endedParagraph = true;
              paragraph.Remove(0, paragraph.Length);
            }
          } else if (haveParagraph && (!flowed || lastLine)) {
            formatted.Append("<p>");
            AppendHtmlEscape(formatted, paragraph.ToString());
            haveParagraph = false;
            endedParagraph = true;
            paragraph.Remove(0, paragraph.Length);
            AppendHtmlEscape(formatted, str.Substring(index, lineEnd -
index));
            formatted.Append("</p>");
          }
          if (lastQuotes < quotes) {
            var k = 0;
            for (k = lastQuotes; k < quotes; ++k) {
              formatted.Append("<blockquote>");
            }
          } else if (quotes < lastQuotes) {
            var k = 0;
            for (k = quotes; k < lastQuotes; ++k) {
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
                  {
                    string s = str.Substring(index, lineEnd -
                        index);
                    AppendHtmlEscape(formatted, s);
                  }
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
      var count = 0;
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] == str[0]) {
          if (count < 3) {
            ++count;
          }
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
      if (String.IsNullOrEmpty(str)) {
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

    private static string GetLinkTitle(string str) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      var index = 0;
      while (index < str.Length && (str[index] == ' ' ||
          str[index] == '\t')) {
        ++index;
      }
      if (index == 0 || index == str.Length) {
        return null;
      }
      if (str[index] == '"' || str[index] == '\'' || str[index] == '\u0028') {
        int titleStart = index + 1;
        char endDelim = '"';
        if (str[index] == '\'') {
          endDelim = '\'';
        }
        if (str[index] == '\u0028') {
          endDelim = '\u0029';
        }
        ++index;
        while (index < str.Length && str[index] != endDelim) {
          ++index;
        }
        if (index == str.Length) {
          return null;
        }
        int titleEnd = index;
        ++index;
        while (index < str.Length && (str[index] == ' ' ||
            str[index] == '\t')) {
          ++index;
        }
        return (
            index != str.Length) ? null : str.Substring(
            titleStart,
            titleEnd - titleStart);
      }
      return null;
    }

    private static string[] GetLinkLine(string str) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      str = TrimShortSpaces(str);
      if (str.Length > 0 && str[0] == '[') {
        var index = 1;
        int labelStart = index;
        while (index < str.Length && str[index] != ']') {
          ++index;
        }
        if (index == str.Length) {
          return null;
        }
        int labelEnd = index;
        ++index;
        if (index >= str.Length || str[index] != ':') {
          return null;
        }
        ++index;
        int tmp = index;
        while (index < str.Length && (str[index] == ' ' ||
            str[index] == '\t')) {
          ++index;
        }
        if (tmp == index) {
          return null;
        }
        int urlStart = index;
        string label = DataUtilities.ToLowerCaseAscii(
            str.Substring(labelStart, labelEnd - labelStart));
        string url = str.Substring(urlStart, str.Length - urlStart);
        string[] urltitle = SplitUrl(url, true);
        url = urltitle[0];
        if (url.Length > 0 && url[0] == '<' && url[url.Length - 1] == '>') {
          url = url.Substring(1, url.Length - 2);
        }
        return new string[] { label, url, urltitle[1] };
      }
      return null;
    }

    private static string TrimShortSpaces(string str) {
      if (str != null) {
        var i = 0;
        while (i < str.Length) {
          if (str[i] == ' ') {
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
      if (trim) {
        str = TrimShortSpaces(str);
      }
      if (str == null) {
        return false;
      }
      var digit = false;
      var i = 0;
      while (i < str.Length) {
        if (str[i] >= '0' && str[i] <= '9') {
          digit = true;
        } else if (str[i] == '.' && i + 1 < str.Length &&
          (str[i + 1] == ' ' || str[i + 1] == '\t')) {
          return digit;
        } else {
          return false;
        }
        ++i;
      }
      return false;
    }

    private static bool IsCodeBlockLine(string str) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      if (str.Length >= 1 && str[0] == '\t') {
        return true;
      }
      return (str.Length >= 4 && str[0] == ' ' && str[1] == ' ' &&
          str[2] == ' ' && str[3] == ' ') ? true : false;
    }
    private static string ReplaceTwoOrMoreSpacesWithBR(
      string str) {
      if (String.IsNullOrEmpty(str)) {
        return String.Empty;
      }
      if (str.Length >= 2 && str[str.Length - 1] == ' ' && str[str.Length -
          2] == ' ') {
        int i = str.Length - 1;
        while (i >= 0) {
          if (str[i] != ' ') {
            return str.Substring(0, i + 1) + "<br/>";
          }
          --i;
        }
        return "<br/>";
      }
      return str;
    }

    private static string StripCodeBlockSpaces(string str) {
      if (String.IsNullOrEmpty(str)) {
        return String.Empty;
      }
      if (str.Length >= 1 && str[0] == '\t') {
        return str.Substring(1);
      }
      return (str.Length >= 4 && str[0] == ' ' && str[1] == ' ' &&

          str[2] == ' ' && str[3] == ' ') ? str.Substring(4) : str;
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
              ++qi;
              break;
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

    private static string[] SplitUrl(string urlText, bool extended) {
      string titleText = null;
      var qi = 0;
      while (qi < urlText.Length) {
        if (urlText[qi] == ' ' || urlText[qi] == '\t') {
          int possibleUrlEnd = qi;
          while (qi < urlText.Length &&
            (urlText[qi] == ' ' || urlText[qi] == '\t')) {
            {
              ++qi;
            }
          }
          if (qi < urlText.Length && (urlText[qi] == '"' ||
              (extended && (urlText[qi] == '\'' || urlText[qi] == '\u0028')))) {
            char startDelim = urlText[qi];
            ++qi;
            int possibleTitleStart = qi;
            char endDelim = '"';
            if (startDelim == '\'') {
              endDelim = '\'';
            }
            if (startDelim == '\u0028') {
              {
                endDelim = '\u0029';
              }
            }
            while (qi < urlText.Length && (urlText[qi] != endDelim)) {
              ++qi;
            }
            if (qi == urlText.Length - 1) {
              titleText = urlText.Substring(
                  possibleTitleStart,
                  (urlText.Length - 1) - possibleTitleStart);
              urlText = urlText.Substring(0, possibleUrlEnd);
              return new string[] { urlText, titleText };
            }
          }
          qi = possibleUrlEnd + 1;
        } else {
          ++qi;
        }
      }
      return new string[] { urlText, null };
    }

    private static int GetLinkRefStart(string str, int index) {
      if (index < str.Length && str[index] == '\u0028') {
        return index;
      }
      while (index < str.Length && (str[index] == ' ' || str[index] == '\t')) {
        ++index;
      }
      return (index < str.Length && str[index] == '[') ? index : (-1);
    }

    private static string ReplaceImageLinks(
      string str,
      IDictionary<string, string[]> links) {
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
              found = true;
              break;
            }
            ++index;
          }
          if (!found) {
            sb.Append(str[i]);
            continue;
          }
          string linkText = str.Substring(linkStart, index - linkStart);
          ++index;
          int linkRefStart = GetLinkRefStart(str, index);
          if (linkRefStart < 0) {
            sb.Append(str[i]);
            continue;
          }
          index = linkRefStart + 1;
          linkStart = linkRefStart + 1;
          bool urlRef = str[linkRefStart] == '\u0028';
          char endChar = urlRef ? '\u0029' : ']';
          found = false;
          linkStart = index;
          while (index < str.Length) {
            if (str[index] == endChar) {
              found = true;
              break;
            }
            ++index;
          }
          if (!found) {
            sb.Append(str[i]);
            continue;
          }
          string urlText = str.Substring(linkStart, index - linkStart);
          string[] urlTitle = null;
          if (urlRef) {
            urlTitle = SplitUrl(urlText, false);
          } else {
            urlText = DataUtilities.ToLowerCaseAscii(
                String.IsNullOrEmpty(urlText) ? linkText : urlText);
            if (links.ContainsKey(urlText)) {
              urlTitle = links[urlText];
            } else {
              sb.Append(str[i]);
              continue;
            }
          }
          sb.Append("<img src=\"")
          .Append(HtmlEscapeStrong(urlTitle[0])).Append("\" alt=\"")
          .Append(HtmlEscapeStrong(linkText)).Append("\"");
          if (urlTitle[1] != null) {
            sb.Append(" title=\"")
            .Append(HtmlEscapeStrong(urlTitle[1])).Append("\"");
          }
          sb.Append(" />");
          i = index;
        } else {
          sb.Append(str[i]);
        }
      }
      return sb.ToString();
    }

    private static string ReplaceInlineLinks(
      string str,
      IDictionary<string, string[]> links) {
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
          int linkRefStart = GetLinkRefStart(str, index);
          if (linkRefStart < 0) {
            sb.Append(str[i]);
            continue;
          }
          index = linkRefStart + 1;
          linkStart = linkRefStart + 1;
          bool urlRef = str[linkRefStart] == '\u0028';
          char endChar = urlRef ? '\u0029' : ']';
          found = false;
          linkStart = index;
          while (index < str.Length) {
            if (str[index] == endChar) {
              found = true;
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
          string[] urlTitle = null;
          if (urlRef) {
            urlTitle = SplitUrl(urlText, false);
          } else {
            urlText = DataUtilities.ToLowerCaseAscii(
                String.IsNullOrEmpty(urlText) ? linkText : urlText);
            if (links.ContainsKey(urlText)) {
              urlTitle = links[urlText];
            } else {
              sb.Append(str[i]);
              continue;
            }
          }
          sb.Append("<a href=\"")
          .Append(HtmlEscapeStrong(urlTitle[0])).Append("\"");
          if (urlTitle[1] != null) {
            sb.Append(" title=\"")
            .Append(HtmlEscapeStrong(urlTitle[1])).Append("\"");
          }
          sb.Append(">")
          .Append(linkText).Append("</a>");
          i = index;
        } else {
          sb.Append(str[i]);
        }
      }
      return sb.ToString();
    }

    private static void HexEscape(StringBuilder sb, char c) {
      if (c >= 0x100) {
        throw new ArgumentOutOfRangeException(nameof(c));
      }
      string hex = "0123456789abcdef";
      sb.Append("&#x");
      if (c >= 0x10) {
        sb.Append(hex[(c >> 4) & 15]);
      }
      sb.Append(hex[c & 15]).Append(";");
    }

    private static string CodeSpansAndEscapes(
      string str) {
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] == '&') {
          int qi = i + 1;
          var plausibleEscape = false;
          while (qi < str.Length) {
            if (str[qi] == ';') {
              {
                plausibleEscape = true;
              }
              break;
            } else if (str[qi] == '#' && qi != i + 1) {
              break;
            } else if (str[qi] == '#') {
              {
                ++qi;
              }
              continue;
            } else if (str[qi] >= '0' && str[qi] <= '9') {
              {
                ++qi;
              }
              continue;
            } else if (str[qi] >= 'A' && str[qi] <= 'Z') {
              {
                ++qi;
              }
              continue;
            } else if (str[qi] >= 'a' && str[qi] <= 'z') {
              {
                ++qi;
              }
              continue;
            }
            break;
          }
          if (plausibleEscape) {
            sb.Append('&');
          } else {
            sb.Append("&amp;");
          }
          continue;
        } else if (str[i] == '<') {
          int qi = i + 1;
          var plausibleTag = false;
          if (qi < str.Length && (
              (str[qi] >= '0' && str[qi] <= '9') ||
              (str[qi] >= 'A' && str[qi] <= 'Z') ||
              (str[qi] >= 'a' && str[qi] <= 'z') ||
              (str[qi] == '_') || (str[qi] == '/'))) {
            {
              plausibleTag = true;
            }
            ++qi;
          }
          if (plausibleTag) {
            var found = false;
            while (qi < str.Length) {
              if (str[qi] == '>') {
                {
                  found = true;
                }
                break;
              }
              ++qi;
            }
            plausibleTag = plausibleTag && found;
          }
          if (plausibleTag) {
            sb.Append('<');
          } else {
            sb.Append("&lt;");
          }
          continue;
        } else if (str[i] == '\\' && i + 1 < str.Length &&
          "[]()\u007b\u007d#+-_`\\*!.".IndexOf(str[i + 1]) >= 0) {
          HexEscape(sb, str[i + 1]);
          ++i;
          continue;
        }
        if (str[i] == '`') {
          int qi = i + 1;
          var backTicks = 1;
          sb.Append("<code>");
          while (qi < str.Length) {
            if (str[qi] == '`') {
              ++backTicks;
            } else {
              break;
            }
            ++qi;
          }
          while (qi < str.Length) {
            if (str[qi] == '`') {
              int endBackTicks = 1;
              int qi2 = qi + 1;
              while (backTicks > 1 && qi2 < str.Length) {
                if (str[qi2] == '`') {
                  ++endBackTicks;
                  if (endBackTicks >= backTicks) {
                    qi = qi2;
                    break;
                  }
                } else {
                  break;
                }
                ++qi2;
              }
              if (endBackTicks >= backTicks) {
                break;
              }
            }
            char c = str[qi];
            if ("[]()\u007b\u007d#+-_`\\*!.<>&".IndexOf(str[qi]) >= 0) {
              HexEscape(sb, c);
            } else {
              sb.Append(str[qi]);
            }
            ++qi;
          }
          i = qi;
          sb.Append("</code>");
          continue;
        }
        sb.Append(str[i]);
      }
      return sb.ToString();
    }

    private static string ReplaceAutomaticLinks(
      string str) {
      if (str.IndexOf('<') < 0) {
        return str;
      }
      var sb = new StringBuilder();
      for (var i = 0; i < str.Length; ++i) {
        if (str[i] == '<') {
          int qi = i + 1;
          int linkStart = qi;
          int linkEnd = linkStart;
          var plausibleTag = false;
          if (qi < str.Length && (
              (str[qi] >= '0' && str[qi] <= '9') ||
              (str[qi] >= 'A' && str[qi] <= 'Z') ||
              (str[qi] >= 'a' && str[qi] <= 'z') ||
              (str[qi] == '_') || (str[qi] == '/'))) {
            {
              plausibleTag = true;
            }
            ++qi;
          }
          if (plausibleTag) {
            var found = false;
            while (qi < str.Length) {
              if (str[qi] == ' ' || str[qi] == '\t') {
                break;
              }
              if (str[qi] == '>') {
                {
                  linkEnd = qi;
                  found = true;
                }
                break;
              }
              ++qi;
            }
            plausibleTag = plausibleTag && found;
          }
          if (plausibleTag) {
            string payload = str.Substring(linkStart, linkEnd - linkStart);
            if (payload.IndexOf('@') >= 1 && payload.IndexOf('?') < 0) {
              sb.Append("<a href=\"");
              sb.Append(HtmlEscapeStrong("mailto:" + payload));
              sb.Append("\">").Append(HtmlEscapeStrong(payload))
              .Append("</a>");
              i = linkEnd;
              continue;
            } else if (payload.IndexOf(':') >= 1) {
              sb.Append("<a href=\"");
              sb.Append(HtmlEscapeStrong(payload));
              sb.Append("\">");
              sb.Append(HtmlEscapeStrong(payload))
              .Append("</a>");
              i = linkEnd;
              continue;
            }
          }
        }
        sb.Append(str[i]);
      }
      return sb.ToString();
    }

    private static string FormatParagraph(
      string str,
      IDictionary<string, string[]> links) {
      str = CodeSpansAndEscapes(str);
      str = ReplaceAutomaticLinks(str);
      str = ReplaceImageLinks(str, links);
      str = ReplaceInlineLinks(str, links);
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
      var hashes = false;
      while (i >= 0 && str[i] == '#') {
        --i;
        hashes = true;
      }
      if (hashes) {
        hashes = false;
        while (i >= 0 && (str[i] == ' ' || str[i] == '\t')) {
          --i;
          hashes = true;
        }
        ++i;
        return hashes ? str.Substring(0, i) : str;
      }
      return str;
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
        if (i + 1 < str.Length && str[i] == '\t') {
          return str.Substring(i + 1);
        }
        while (i < str.Length && (str[i] == ' ') && i < 4) {
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
        if (i + 1 < str.Length && str[i] == '\t') {
          return str.Substring(i + 1);
        }
        while (i < str.Length && (str[i] == ' ') && i < 4) {
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
      Dictionary<string, string[]> dict;
      dict = new Dictionary<string, string[]>();
      return MarkdownText(str, depth, true, dict);
    }

    public static bool IsListLine(string line, bool ordered) {
      return ordered ? IsOrderedListLine(line) :
        IsUnorderedListLine(line);
    }

    public static string StripItemStart(string line, bool ordered) {
      return ordered ? StripOrderedListItemStart(line) :
        StripListItemStart(line);
    }

    private static string MarkdownText(
      string str,
      int depth,
      bool alwaysUseParas,
      IDictionary<string, string[]> links) {
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
      if (depth == 0) {
        // Get Markdown links
        for (; i < lines.Count; ++i) {
          string line = lines[i];
          string[] linkline = GetLinkLine(line);
          if (linkline != null) {
            lines[i] = String.Empty;
            if (i + 1 < lines.Count && linkline[2] == null) {
              string title = GetLinkTitle(lines[i + 1]);
              if (title != null) {
                linkline[2] = title;
                lines[i + 1] = String.Empty;
              }
            }
            links[linkline[0]] = new string[] { linkline[1], linkline[2] };
          }
        }
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
          var qs = new StringBuilder().Append(StripQuoteStart(line));
          var haveAnotherQuoteLine = false;
          var valueGoToEndOfPara = false;
          while (qi < lines.Count) {
            line = lines[qi];
            if (IsQuoteLine(line)) {
              qs.Append("\r\n").Append(StripQuoteStart(line));
              haveAnotherQuoteLine = true;
              ++qi;
            } else if (!haveAnotherQuoteLine && !IsBlankishLine(line)) {
              valueGoToEndOfPara = true;
              break;
            } else {
              break;
            }
          }
          if (valueGoToEndOfPara) {
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
            formatted.Append("<pre><code>");
            AppendHtmlEscape(formatted, qs.ToString());
            formatted.Append("</code></pre>");
          } else {
            formatted.Append(
              MarkdownText(
                qs.ToString(),
                depth + 1,
                true,
                links));
          }
          formatted.Append("</blockquote>");
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
          var itemLineCount = 0;
          var seenBlankishLine = false;
          var wrapLinesInParas = false;

          // DebugUtility.Log("newlist");
          while (qi < lines.Count) {
            line = lines[qi];
            if (IsListLine(line, ordered)) {
              wrapLinesInParas |= seenBlankishLine;
              // DebugUtility.Log("para=" + qs.ToString());
              string qss2;
              if (depth >= 100) {
                formatted.Append("<pre><code>");
                formatted.Append(qs.ToString());
                formatted.Append("</code></pre>");
              } else {
                qss2 = MarkdownText(
                    qs.ToString(),
                    depth + 1,
                    wrapLinesInParas,
                    links);
                formatted.Append(qss2);
              }
              formatted.Append("</li><li>");
              qs.Remove(0, qs.Length);
              ++qi;
              strippedLine = StripItemStart(line, ordered);
              qs.Append(strippedLine);
              ++itemLineCount;
              seenBlankishLine = false;
            } else {
              // DebugUtility.Log("[" + line + "]");
              // DebugUtility.Log("blankish=" + IsBlankishLine(line));
              if (IsBlankishLine(line)) {
                seenBlankishLine = true;
                ++qi;
              } else if (IsCodeBlockLine(line)) {
                if (seenBlankishLine) {
                  qs.Append("\r\n");
                }
                seenBlankishLine = false;
                qs.Append("\r\n").Append(StripItemStart(line, ordered));
                // DebugUtility.Log("qs=" + qs);
                ++qi;
                ++itemLineCount;
              } else if (seenBlankishLine) {
                break;
              } else {
                if (seenBlankishLine) {
                  qs.Append("\r\n");
                }
                seenBlankishLine = false;
                qs.Append("\r\n").Append(StripItemStart(line, ordered));
                // DebugUtility.Log("qs=" + qs);
                ++qi;
                ++itemLineCount;
              }
            }
          }
          i = qi - 1;
          // DebugUtility.Log("listitem = "+qs+", wrapinparas="+wrapLinesInParas);
          string qss = MarkdownText(
              qs.ToString(),
              depth + 1,
              wrapLinesInParas,
              links);
          // DebugUtility.Log("formatted_listitem = "+qss);
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
          .Append(">").Append(FormatParagraph(stripped, links))
          .Append("</h").Append((char)('0' + heading));
          formatted.Append(">");
        } else if (IsCodeBlockLine(line)) {
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.Append("<p>");
            formatted.Append(paragraph.ToString());
            formatted.Append("</p>");
          }
          int qi = i + 1;
          var qs = new StringBuilder().Append(StripCodeBlockSpaces(line));
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
          AppendHtmlEscape(formatted, qs.ToString());
          formatted.Append("</code></pre>");
        } else {
          if (!IsBlankishLine(line)) {
            if (haveParagraph) {
              paragraph.Append("\r\n");
            } else {
              paragraph.Remove(0, paragraph.Length);
            }
            haveParagraph = true;
            paragraph.Append(FormatParagraph(line, links));
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
