package com.upokecenter.mail;

import java.util.*;

  final class FormatFlowed {
private FormatFlowed() {
}
    private static String HtmlEscape(String str) {
      int i = 0;
      for (; i < str.length(); ++i) {
        if (str.charAt(i) == '&' || str.charAt(i) == '<' || str.charAt(i) == '>') {
          break;
        }
      }
      if (i == str.length()) {
        return str;
      }
      StringBuilder sb = new StringBuilder();
      sb.append(str.substring(0, i));
      for (; i < str.length(); ++i) {
        switch (str.charAt(i)) {
          case '&':
            sb.append("&amp;");
            break;
          case '<':
            sb.append("&lt;");
            break;
          case '>':
            sb.append("&gt;");
            break;
          default:
            sb.append(str.charAt(i));
            break;
        }
      }
      return sb.toString();
    }

    public static String NonFormatFlowedText(String str) {
      StringBuilder sb = new StringBuilder();
      sb.append("<pre>");
      sb.append(HtmlEscape(str));
      sb.append("</pre>");
      return sb.toString();
    }

    public static String FormatFlowedText(String str, boolean delSp) {
      int i = 0;
      int lineStart = i;
      int lastQuotes = 0;
      StringBuilder paragraph = new StringBuilder();
      StringBuilder formatted = new StringBuilder();
      boolean haveParagraph = false;
      while (i <= str.length()) {
        if (i == str.length() || (str.charAt(i) == 0x0d && i + 1 < str.length() &&
          str.charAt(i + 1) == 0x0a)) {
          boolean lastLine = i == str.length();
          int lineEnd = i;
          int index = lineStart;
          int quotes = 0;
          boolean flowed = false;
          boolean signature = false;
          while (index < lineEnd && str.charAt(index) == '>') {
            ++quotes;
            ++index;
          }
          if (index < lineEnd && str.charAt(index) == ' ') {
            // Space stuffing
            ++index;
          }
          if (index + 3 == lineEnd && str.charAt(index) == '-' &&
             str.charAt(index + 1) == '-' && str.charAt(index + 2) == ' ') {
            signature = true;
          } else if (index < lineEnd && str.charAt(lineEnd - 1) == ' ') {
            flowed = true;
            if (delSp) {
              --lineEnd;
            }
          }
          boolean endedParagraph = false;
          if (signature || lastQuotes != quotes) {
            // Signature line reached, or
            // change in quote depth, or fixed after flowed
            if (haveParagraph) {
              formatted.append("<p>")
                    .append(HtmlEscape(paragraph.toString()))
                    .append("</p>");
              haveParagraph = false;
              endedParagraph = true;
              paragraph.delete(0, (0)+(paragraph.length()));
            }
          } else if (haveParagraph && (!flowed || lastLine)) {
            formatted.append("<p>").append(HtmlEscape(paragraph.toString()));
            haveParagraph = false;
            endedParagraph = true;
            paragraph.delete(0, (0)+(paragraph.length()));
            formatted.append(
                HtmlEscape(str.substring(index, (index)+(lineEnd - index))))
                    .append("</p>");
          }
          if (lastQuotes < quotes) {
            for (var k = lastQuotes; k < quotes; ++k) {
              formatted.append("<blockquote>");
            }
          } else if (quotes < lastQuotes) {
            for (var k = quotes; k < lastQuotes; ++k) {
              formatted.append("</blockquote>");
            }
          }
          if (!endedParagraph) {
            if (flowed) {
              haveParagraph = true;
              paragraph.append(str.substring(index, (index)+(lineEnd - index)));
            } else {
              // Line is fixed, or is a signature line
              if (signature) {
                formatted.append("<hr/>");
              } else {
                if (index < lineEnd) {
                  formatted.append("<tt>");
                  formatted.append(
                    HtmlEscape(str.substring(index, (index)+(lineEnd - index))));
                  formatted.append("</tt>");
                }
                if (!lastLine) {
                  formatted.append("<br/>\r\n");
                }
              }
            }
          }
          if (i == str.length() && quotes > 0) {
            for (int k = 0; k < quotes; ++k) {
              formatted.append("</blockquote>");
            }
          }
          lineStart = i + 2;
          lastQuotes = quotes;
          if (i == str.length()) {
            break;
          }
        }
        ++i;
      }
      return formatted.toString();
    }

    private static boolean IsQuoteLine(String str) {
      return !((str)==null || (str).length()==0) && str.charAt(0) == '>';
    }

    private static boolean IsBarLine(String str) {
      if (str == null || str.length() < 3) {
        return false;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) != '-') {
          return false;
        }
      }
      return true;
    }

    private static boolean IsEqualsLine(String str) {
      if (str == null || str.length() < 3) {
        return false;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) != '=') {
          return false;
        }
      }
      return true;
    }

    private static boolean IsHeadingLine(String str) {
      return !((str)==null || (str).length()==0) && str.charAt(0) == '#';
    }

    private static int HeadingLevel(String str) {
      if (str == null) {
        return 0;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) != '#') {
          return Math.min(i, 6);
        }
      }
      return Math.min(str.length(), 6);
    }

    private static String TrimShortSpaces(String str) {
      if (str != null) {
        int i = 0;
        while (i < str.length()) {
          if (str.charAt(i) == ' ' || str.charAt(i) == '\t') {
            if (i >= 4) {
 return str;
}
          } else if (i <= 3) {
            return str.substring(i);
          }
          ++i;
        }
      }
      return str;
    }

    private static boolean IsUnorderedListLine(String str) {
      str = TrimShortSpaces(str);
      return str != null && str.length() >= 2 &&
          (str.charAt(0) == '-' || str.charAt(0) == '*') && (str.charAt(1) == ' ' || str.charAt(1) == '\t');
    }

    private static boolean IsCodeBlockLine(String str) {
      return str != null && str.length() >= 4 &&
          (str.charAt(0) == ' ' || str.charAt(0) == '\t') &&
          (str.charAt(1) == ' ' || str.charAt(1) == '\t') &&
          (str.charAt(2) == ' ' || str.charAt(2) == '\t') &&
          (str.charAt(3) == ' ' || str.charAt(3) == '\t');
    }

    private static boolean IsSpacedOrEmptyLine(String str) {
      return ((str)==null || (str).length()==0) || (str.charAt(0) == ' ' || str.charAt(0) ==
           '\t');
    }

    private static String HtmlEscapeStrong(String str) {
      int i = 0;
      StringBuilder sb = new StringBuilder();
      for (; i < str.length(); ++i) {
        switch (str.charAt(i)) {
          case '&':
            sb.append("&amp;");
            break;
          case '<':
            sb.append("&lt;");
            break;
          case '>':
            sb.append("&gt;");
            break;
          case '\'':
            sb.append("&#x27;");
            break;
          case '\"':
            sb.append("&#x22;");
            break;
          case '_':
            sb.append("&#x5f;");
            break;
          case '*':
            sb.append("&#x2a;");
            break;
          case '\\':
            sb.append("&#x5c;");
            break;
          case '`':
            sb.append("&#x60;");
            break;
          default:
            sb.append(str.charAt(i));
            break;
        }
      }
      return sb.toString();
    }

    private static boolean StartsWith(String str, int index, String delim) {
      for (int i = 0; i < delim.length(); ++i) {
        if (index + i >= str.length() || str.charAt(index + i) != delim.charAt(i)) {
          return false;
        }
      }
      return true;
    }

    private static String FormatParagraph(
         String str,
         String delim,
         String tag,
         boolean escaped) {
      if (delim.length() == 1 && str.indexOf(delim.charAt(0)) < 0) {
        return str;
      }
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.length(); ++i) {
        if (StartsWith(str, i, delim)) {
          int qi = i + delim.length();
          int textStart = qi;
          while (qi < str.length()) {
            if (StartsWith(str, qi, delim)) {
              break;
            }
            ++qi;
          }
          if (qi != str.length()) {
            String inner = str.substring(textStart, (textStart)+(qi - textStart));
            if (escaped) {
              inner = HtmlEscapeStrong(inner);
            }
            sb.append('<').append(tag).append('>').append(inner)
              .append("</").append(tag).append('>');
            i = qi + delim.length() - 1;
            continue;
          }
        }
        sb.append(str.charAt(i));
      }
      return sb.toString();
    }

    private static String ReplaceImageLinks(String str) {
      if (str.indexOf('!') < 0) {
        return str;
      }
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '!' && i + 1 < str.length() && str.charAt(i + 1) == '[') {
          boolean found = false;
          int linkStart = i + 2;
          int index = i + 2;
          while (index < str.length()) {
            if (str.charAt(index) == ']') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.append(str.charAt(i));
            }
            continue;
          }
          String linkText = str.substring(linkStart, (linkStart)+(index - linkStart));
          ++index;
          if (index >= str.length() || str.charAt(index) != '(') {
            sb.append(str.charAt(i));
            continue;
          }
          ++index;
          found = false;
          linkStart = index;
          while (index < str.length()) {
            if (str.charAt(index) == ')') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.append(str.charAt(i));
            }
            continue;
          }
          String urlText = str.substring(linkStart, (linkStart)+(index - linkStart));
          sb.append("<img src=\"")
          .append(HtmlEscapeStrong(urlText)).append("\" alt=\"")
          .append(HtmlEscapeStrong(linkText)).append("\" />");
          i = index;
        } else {
          sb.append(str.charAt(i));
        }
      }
      return sb.toString();
    }

    private static String ReplaceInlineLinks(String str) {
      if (str.indexOf('[') < 0) {
        return str;
      }
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '[') {
          boolean found = false;
          int linkStart = i + 1;
          int index = i + 1;
          while (index < str.length()) {
            if (str.charAt(index) == ']') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.append(str.charAt(i));
            }
            continue;
          }
          String linkText = str.substring(linkStart, (linkStart)+(index - linkStart));
          ++index;
          if (index >= str.length() || str.charAt(index) != '(') {
            sb.append(str.charAt(i));
            continue;
          }
          ++index;
          found = false;
          linkStart = index;
          while (index < str.length()) {
            if (str.charAt(index) == ')') {
              {
                found = true;
              }
              break;
            }
            ++index;
          }
          if (!found) {
            {
              sb.append(str.charAt(i));
            }
            continue;
          }
          String urlText = str.substring(linkStart, (linkStart)+(index - linkStart));
          sb.append("<a href=\"")
          .append(HtmlEscapeStrong(urlText)).append("\">")
          .append(linkText).append("</a>");
          i = index;
        } else {
          sb.append(str.charAt(i));
        }
      }
      return sb.toString();
    }

    private static String ReplaceBackslashEscapes(
         String str) {
      if (str.indexOf('\\') < 0) {
        return str;
      }
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '\\' && i + 1 < str.length()) {
          int c = DataUtilities.CodePointAt(str, i + 1);
          if (c >= 0x10000) {
            i += 2;
          } else {
            ++i;
          }
          String hex = "0123456789abcdef";
          boolean havestr = false;
          sb.append("&#x");
          if (c >= 0x100000) {
            {
              sb.append(hex.charAt((c >> 20) & 15));
            }
            havestr = true;
          }
          if (c >= 0x10000 || havestr) {
            {
              sb.append(hex.charAt((c >> 16) & 15));
            }
            havestr = true;
          }
          if (c >= 0x1000 || havestr) {
            {
              sb.append(hex.charAt((c >> 12) & 15));
            }
            havestr = true;
          }
          if (c >= 0x100 || havestr) {
            {
              sb.append(hex.charAt((c >> 8) & 15));
            }
            havestr = true;
          }
          if (c >= 0x10 || havestr) {
            {
              sb.append(hex.charAt((c >> 4) & 15));
            }
            havestr = true;
          }
          sb.append(hex.charAt(c & 15)).append(";");
          continue;
        }
        sb.append(str.charAt(i));
      }
      return sb.toString();
    }

    private static String FormatParagraph(String str) {
      str = ReplaceBackslashEscapes(str);
      str = ReplaceImageLinks(str);
      str = ReplaceInlineLinks(str);
      str = FormatParagraph(str, "`", "code", true);
      str = FormatParagraph(str, "__", "strong", false);
      str = FormatParagraph(str, "**", "strong", false);
      str = FormatParagraph(str, "_", "em", false);
      str = FormatParagraph(str, "*", "em", false);
      return str;
    }

    private static String StripHeadingStart(String str) {
      if (((str) == null || (str).length() == 0)) {
        return "";
      }
      int i = 0;
      while (i < str.length() && str.charAt(i) == '#') {
        ++i;
      }
      while (i < str.length() && (str.charAt(i) == ' ' || str.charAt(i) == '\t')) {
        ++i;
      }
      return str.substring(i);
    }

    private static String StripListItemStart(String str) {
      if (((str) == null || (str).length() == 0)) {
        return "";
      }
      if (IsUnorderedListLine(str)) {
        str = TrimShortSpaces(str);
      }
      int i = 0;
      if (str.charAt(i) == '*' || str.charAt(i) == '-') {
        ++i;
        while (i < str.length() && (str.charAt(i) == ' ' || str.charAt(i) == '\t')) {
          ++i;
        }
      } else {
        while (i < str.length() && (str.charAt(i) == ' ' || str.charAt(i) == '\t') && i < 4) {
          ++i;
        }
      }
      return str.substring(i);
    }

    private static String StripQuoteStart(String str) {
      if (str == null || str.length() < 2) {
        return "";
      }
      return (str.charAt(1) == ' ' || str.charAt(1) == '\t') ? str.substring(2) :
        str.substring(1);
    }

    public static String MarkdownText(String str, int depth) {
      int i = 0;
      int lineStart = i;
      StringBuilder formatted = new StringBuilder();
      ArrayList<String> lines = new ArrayList<String>();
      StringBuilder paragraph = new StringBuilder();
      boolean haveParagraph = false;
      while (i <= str.length()) {
        if (i == str.length() ||
            (str.charAt(i) == 0x0d && i + 1 < str.length() && str.charAt(i + 1) == 0x0a)) {
          lines.add(str.substring(lineStart, (lineStart)+(i - lineStart)));
          if (i == str.length()) {
            break;
          }
          lineStart = i + 2;
        }
        ++i;
      }
      i = 0;
      for (; i < lines.size(); ++i) {
        String line = lines.get(i);
        if (IsQuoteLine(line)) {
          // Quote
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int qi = i + 1;
          StringBuilder qs = new StringBuilder()
              .append(StripQuoteStart(line));
          while (qi < lines.size()) {
            line = lines.get(qi);
            if (IsQuoteLine(line)) {
              qs.append("\r\n").append(StripQuoteStart(line));
              ++qi;
            } else {
              break;
            }
          }
          i = qi - 1;
          formatted.append("<blockquote>");
          if (depth > 10) {
            formatted.append("<pre>");
            formatted.append(HtmlEscape(qs.toString()));
            formatted.append("</pre>");
          } else {
            formatted.append(MarkdownText(qs.toString(), depth + 1));
          }
          formatted.append("</blockquote>");
        } else if (IsBarLine(line)) {
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          formatted.append("<hr/>");
        } else if (IsUnorderedListLine(line)) {
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int qi = i + 1;
          StringBuilder qs = new StringBuilder().append(StripListItemStart(line));
          formatted.append("<ul><li>");
          while (qi < lines.size()) {
            line = lines.get(qi);
            if (IsSpacedOrEmptyLine(line)) {
              qs.append("\r\n").append(StripQuoteStart(line));
              ++qi;
            } else if (IsUnorderedListLine(line)) {
              String qss2 = MarkdownText(qs.toString(), depth + 1);
              formatted.append(qss2);
              formatted.append("</li><li>");
              qs.delete(0, (0)+(qs.length()));
              ++qi;
              qs.append(StripListItemStart(line));
            } else {
              break;
            }
          }
          i = qi - 1;
          String qss = MarkdownText(qs.toString(), depth + 1);
          formatted.append(qss);
          formatted.append("</li></ul>");
        } else if (IsHeadingLine(line)) {
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int heading = HeadingLevel(line);
          String stripped = StripHeadingStart(line);
          formatted.append("<h").append((char)('0' + heading))
               .append(">").append(FormatParagraph(stripped))
               .append("</h").append((char)('0' + heading))
               .append(">");
        } else if (IsCodeBlockLine(line)) {
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int qi = i + 1;
          StringBuilder qs = new StringBuilder()
              .append(line.substring(4));
          while (qi < lines.size()) {
            line = lines.get(qi);
            if (IsCodeBlockLine(line)) {
              qs.append("\r\n").append(line.substring(4));
              ++qi;
            } else {
              break;
            }
          }
          i = qi - 1;
          formatted.append("<pre>");
          formatted.append(HtmlEscape(qs.toString()));
          formatted.append("</pre>");
        } else if (IsEqualsLine(line) && haveParagraph) {
          haveParagraph = false;
          formatted.append("<h1>");
          formatted.append(paragraph.toString());
          formatted.append("</h1>");
        } else {
          if (line.length() > 0) {
            if (haveParagraph) {
              paragraph.append("\r\n");
            } else {
              paragraph.delete(0, (0)+(paragraph.length()));
            }
            haveParagraph = true;
            paragraph.append(FormatParagraph(line));
          } else if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
        }
      }

      if (haveParagraph) {
        haveParagraph = false;
        formatted.append("<p>");
        formatted.append(paragraph.toString());
        formatted.append("</p>");
      }

      return formatted.toString();
    }
  }
