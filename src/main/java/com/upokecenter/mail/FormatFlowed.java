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
      if (str.charAt(0) != '-' && str.charAt(0) != '*' && str.charAt(0) != '_') {
        return false;
      }
      int count = 0;
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == str.charAt(0)) {
          if (count < 3) {
            ++count;
          }
        } else if (str.charAt(i) != ' ') {
          return false;
        }
      }
      return count >= 3;
    }

    private static boolean IsEqualsLine(String str) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) != '=') {
          return false;
        }
      }
      return true;
    }

    private static boolean IsDashLine(String str) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) != '-') {
          return false;
        }
      }
      return true;
    }

    private static boolean IsBlankishLine(String str) {
      if (((str) == null || (str).length() == 0)) {
        return true;
      }
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) != '\t' && str.charAt(i) != ' ') {
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

    private static String GetLinkTitle(String str) {
      if (((str) == null || (str).length() == 0)) {
 return null;
}
      int index = 0;
      while (index < str.length() && (str.charAt(index) == ' ' ||
          str.charAt(index) == '\t')) {
 ++index;
}
      if (index == 0 || index == str.length()) {
 return null;
}
      if (str.charAt(index) == '"' || str.charAt(index) == '\'' || str.charAt(index) == '(') {
        int titleStart = index + 1;
        char endDelim = '"';
        if (str.charAt(index) == '\'') {
 endDelim = '\'';
}
        if (str.charAt(index) == '(') endDelim = ') {
 ';
}
        ++index;
        while (index < str.length() && str.charAt(index) != endDelim) {
 ++index;
}
        if (index == str.length()) {
 return null;
}
        int titleEnd = index;
        ++index;
        while (index < str.length() && (str.charAt(index) == ' ' ||
            str.charAt(index) == '\t')) {
 ++index;
}
        return (index != str.length()) ? (null) : (str.substring(titleStart, (titleStart)+(titleEnd - titleStart)));
      }
      return null;
    }

    private static String[] GetLinkLine(String str) {
      if (((str) == null || (str).length() == 0)) {
 return null;
}
      str = TrimShortSpaces(str);
      if (str.length() > 0 && str.charAt(0) == '[') {
        int index = 1;
        int labelStart = index;
        while (index < str.length() && str.charAt(index) != ']') {
 ++index;
}
        if (index == str.length()) {
 return null;
}
        int labelEnd = index;
        ++index;
        if (index >= str.length() || str.charAt(index) != ':') {
 return null;
}
        ++index;
        int tmp = index;
        while (index < str.length() && (str.charAt(index) == ' ' ||
            str.charAt(index) == '\t')) {
 ++index;
}
        if (tmp == index) {
 return null;
}
        int urlStart = index;
        String label = DataUtilities.ToLowerCaseAscii(
          str.substring(labelStart, (labelStart)+(labelEnd - labelStart)));
        String url = str.substring(urlStart, (urlStart)+(str.length() - urlStart));
        String[] urltitle = SplitUrl(url, true);
        url = urltitle[0];
        if (url.length() > 0 && url.charAt(0) == '<' && url.charAt(url.length() - 1) == '>') {
          url = url.substring(1, (1)+(url.length() - 2));
        }
        return new String[] { label, url, urltitle[1] };
      }
      return null;
    }

    private static String TrimShortSpaces(String str) {
      if (str != null) {
        int i = 0;
        while (i < str.length()) {
          //if (i == 0 && str.charAt(i) == '\t') {
          // return str.substring(1);
          //}
          if (str.charAt(i) == ' ') {
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
          (str.charAt(0) == '-' || str.charAt(0) == '*' || str.charAt(0) == '+') &&
          (str.charAt(1) == ' ' || str.charAt(1) == '\t');
    }

    private static boolean IsOrderedListLine(String str) {
      return IsOrderedListLine(str, true);
    }

    private static boolean IsOrderedListLine(String str, boolean trim) {
      if (trim) {
        str = TrimShortSpaces(str);
      }
      if (str == null) {
        return false;
      }
      boolean digit = false;
      int i = 0;
      while (i < str.length()) {
        if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
          digit = true;
        } else if (str.charAt(i) == '.') {
          return digit;
        } else {
          return false;
        }
        ++i;
      }
      return false;
    }

    private static boolean IsCodeBlockLine(String str) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      if (str.length() >= 1 && str.charAt(0) == '\t') {
        return true;
      }
      return (str.length() >= 4 && str.charAt(0) == ' ' && str.charAt(1) == ' ' &&

          str.charAt(2) == ' ' && str.charAt(3) == ' ') ? (true) : false;
    }
    private
static String ReplaceTwoOrMoreSpacesWithBR(String str) {
      if (((str) == null || (str).length() == 0)) {
        return "";
      }
      if (str.length() >= 2 && str.charAt(str.length() - 1) == ' ' && str.charAt(str.length() -
        2) == ' ') {
        int i = str.length() - 1;
        while (i >= 0) {
          if (str.charAt(i) != ' ') {
            return str.substring(0,i + 1) + "<br/>";
          }
          --i;
        }
        return "<br/>";
      }
      return str;
    }

    private static String StripCodeBlockSpaces(String str) {
      if (((str) == null || (str).length() == 0)) {
        return "";
      }
      if (str.length() >= 1 && str.charAt(0) == '\t') {
        return str.substring(1);
      }
      return (str.length() >= 4 && str.charAt(0) == ' ' && str.charAt(1) == ' ' &&

          str.charAt(2) == ' ' && str.charAt(3) == ' ') ? (str.substring(4)) : str;
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
        if (str.charAt(i) == '<') {
          // Skip HTML markup tags
          sb.append(str.charAt(i));
          int qi = i + 1;
          while (qi < str.length()) {
            sb.append(str.charAt(qi));
            if (str.charAt(qi) == '>') {
              ++qi; break;
            }
            ++qi;
          }
          i = qi - 1;
          continue;
        } else if (StartsWith(str, i, delim)) {
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

    private static String[] SplitUrl(String urlText, boolean extended) {
      String titleText = null;
      int qi = 0;
      while (qi < urlText.length()) {
        if (urlText.charAt(qi) == ' ' || urlText.charAt(qi) == '\t') {
          int possibleUrlEnd = qi;
          while (qi < urlText.length() &&
           (urlText.charAt(qi) == ' ' || urlText.charAt(qi) == '\t')) {
            {
              ++qi;
            }
          }
          if (qi < urlText.length() && (urlText.charAt(qi) == '"' ||
              (extended && (urlText.charAt(qi) == '\'' || urlText.charAt(qi) == '(')))) {
            char startDelim = urlText.charAt(qi);
            ++qi;
            int possibleTitleStart = qi;
            char endDelim = '"';
            if (startDelim == '\'') {
 endDelim = '\'';
}
            if (startDelim == '(') endDelim = ') {
 ';
}
            while (qi < urlText.length() && (urlText.charAt(qi) != endDelim)) {
              ++qi;
            }
            if (qi == urlText.length() - 1) {
              titleText = urlText.substring(possibleTitleStart, (possibleTitleStart)+((urlText.length() - 1) - possibleTitleStart));
              urlText = urlText.substring(0, possibleUrlEnd);
              return new String[] { urlText, titleText };
            }
          }
          qi = possibleUrlEnd + 1;
        } else {
          ++qi;
        }
      }
      return new String[] { urlText, null };
    }

    private static String ReplaceImageLinks(
        String str,
        Map<String,
        String[]> links) {
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
            sb.append(str.charAt(i));
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
              found = true;
              break;
            }
            ++index;
          }
          if (!found) {
            sb.append(str.charAt(i));
            continue;
          }
          String urlText = str.substring(linkStart, (linkStart)+(index - linkStart));
          String[] urlTitle = SplitUrl(urlText, false);
          sb.append("<img src=\"")
          .append(HtmlEscapeStrong(urlTitle[0])).append("\" alt=\"")
          .append(HtmlEscapeStrong(linkText)).append("\"");
          if (urlTitle[1] != null) {
            sb.append(" title=\"")
              .append(HtmlEscapeStrong(urlTitle[1])).append("\"");
          }
          sb.append(" />");
          i = index;
        } else {
          sb.append(str.charAt(i));
        }
      }
      return sb.toString();
    }

    private static String ReplaceInlineLinks(
      String str,
      Map<String,
      String[]> links) {
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
          String[] urlTitle = SplitUrl(urlText, false);
          sb.append("<a href=\"")
          .append(HtmlEscapeStrong(urlTitle[0])).append("\"");
          if (urlTitle[1] != null) {
            sb.append(" title=\"")
              .append(HtmlEscapeStrong(urlTitle[1])).append("\"");
          }
          sb.append(">")
          .append(linkText).append("</a>");
          i = index;
        } else {
          sb.append(str.charAt(i));
        }
      }
      return sb.toString();
    }

    private static void HexEscape(StringBuilder sb, char c) {
      if (c >= 0x100) {
 throw new IllegalArgumentException();
}
      String hex = "0123456789abcdef";
      sb.append("&#x");
      if (c >= 0x10) {
        sb.append(hex.charAt((c >> 4) & 15));
      }
      sb.append(hex.charAt(c & 15)).append(";");
    }

    private static String CodeSpansAndEscapes(
         String str) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '&') {
          int qi = i + 1;
          boolean plausibleEscape = false;
          while (qi < str.length()) {
            if (str.charAt(qi) == ';') {
              {
                plausibleEscape = true;
              }
              break;
            } else if (str.charAt(qi) == '#' && qi != i + 1) {
              break;
            } else if (str.charAt(qi) == '#') {
              {
                ++qi;
              }
              continue;
            } else if (str.charAt(qi) >= '0' && str.charAt(qi) <= '9') {
              {
                ++qi;
              }
              continue;
            } else if (str.charAt(qi) >= 'A' && str.charAt(qi) <= 'Z') {
              {
                ++qi;
              }
              continue;
            } else if (str.charAt(qi) >= 'a' && str.charAt(qi) <= 'z') {
              {
                ++qi;
              }
              continue;
            }
            break;
          }
          if (plausibleEscape) {
            sb.append('&');
          } else {
            sb.append("&amp;");
          }
          continue;
        } else if (str.charAt(i) == '<') {
          int qi = i + 1;
          boolean plausibleTag = false;
          if (qi < str.length() && (
             (str.charAt(qi) >= '0' && str.charAt(qi) <= '9') ||
             (str.charAt(qi) >= 'A' && str.charAt(qi) <= 'Z') ||
             (str.charAt(qi) >= 'a' && str.charAt(qi) <= 'z') ||
             (str.charAt(qi) == '_') || (str.charAt(qi) == '/'))) {
            {
              plausibleTag = true;
            }
            ++qi;
          }
          if (plausibleTag) {
            boolean found = false;
            while (qi < str.length()) {
              if (str.charAt(qi) == '>') {
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
            sb.append('<');
          } else {
            sb.append("&lt;");
          }
          continue;
        } else if (str.charAt(i) == '\\' && i + 1 < str.length() &&
                  "[]()\u007b\u007d#+-_`\\*!.".indexOf(str.charAt(i + 1)) >= 0) {
          HexEscape(sb, str.charAt(i + 1));
          ++i;
          continue;
        }
        if (str.charAt(i) == '`') {
          int qi = i + 1;
          int backTicks = 1;
          sb.append("<code>");
          while (qi < str.length()) {
            if (str.charAt(qi) == '`') {
              ++backTicks;
            } else break;
            ++qi;
          }
          while (qi < str.length()) {
            if (str.charAt(qi) == '`') {
              int endBackTicks = 1;
              int qi2 = qi + 1;
              while (backTicks > 1 && qi2 < str.length()) {
                if (str.charAt(qi2) == '`') {
                  ++endBackTicks;
                  if (endBackTicks >= backTicks) {
                    qi = qi2;
                    break;
                  }
                } else break;
                ++qi2;
              }
              if (endBackTicks >= backTicks) {
                break;
              }
            }
            char c = str.charAt(qi);
            if ("[]()\u007b\u007d#+-_`\\*!.<>&".indexOf(str.charAt(qi)) >= 0) {
              HexEscape(sb, c);
            } else {
              sb.append(str.charAt(qi));
            }
            ++qi;
          }
          i = qi;
          sb.append("</code>");
          continue;
        }
        sb.append(str.charAt(i));
      }
      return sb.toString();
    }

    private static String ReplaceAutomaticLinks(
         String str) {
      if (str.indexOf('<') < 0) {
 return str;
}
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < str.length(); ++i) {
        if (str.charAt(i) == '<') {
          int qi = i + 1;
          int linkStart = qi;
          int linkEnd = linkStart;
          boolean plausibleTag = false;
          if (qi < str.length() && (
             (str.charAt(qi) >= '0' && str.charAt(qi) <= '9') ||
             (str.charAt(qi) >= 'A' && str.charAt(qi) <= 'Z') ||
             (str.charAt(qi) >= 'a' && str.charAt(qi) <= 'z') ||
             (str.charAt(qi) == '_') || (str.charAt(qi) == '/'))) {
            {
              plausibleTag = true;
            }
            ++qi;
          }
          if (plausibleTag) {
            boolean found = false;
            while (qi < str.length()) {
              if (str.charAt(qi) == ' ' || str.charAt(qi) == '\t') {
 break;
}
              if (str.charAt(qi) == '>') {
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
            String payload = str.substring(linkStart, (linkStart)+(linkEnd - linkStart));
            if (payload.indexOf("@") >= 1 && payload.indexOf("?") < 0) {
              sb.append("<a href=\"");
              sb.append(HtmlEscapeStrong("mailto:" + payload));
              sb.append("\">") .append(HtmlEscapeStrong(payload))
               .append("</a>");
              i = linkEnd;
              continue;
            } else if (payload.indexOf(":") >= 1) {
              sb.append("<a href=\"");
              sb.append(HtmlEscapeStrong(payload));
              sb.append("\">") .append(HtmlEscapeStrong(payload))
               .append("</a>");
              i = linkEnd;
              continue;
            }
          }
        }
        sb.append(str.charAt(i));
      }
      return sb.toString();
    }

    private static String FormatParagraph(
      String str,
      Map<String,
      String[]> links) {
      // TODO: Reference-style link/image syntax
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
      str = str.substring(i);
      i = str.length() - 1;
      boolean hashes = false;
      while (i >= 0 && str.charAt(i) == '#') {
        --i;
        hashes = true;
      }
      if (hashes) {
        hashes = false;
        while (i >= 0 && (str.charAt(i) == ' ' || str.charAt(i) == '\t')) {
          --i;
          hashes = true;
        }
        ++i;
        return hashes ? str.substring(0, i) : str;
      }
      return str;
    }

    private static String StripOrderedListItemStart(String str) {
      if (((str) == null || (str).length() == 0)) {
        return "";
      }
      if (IsOrderedListLine(str)) {
        str = TrimShortSpaces(str);
      }
      int i = 0;
      if (IsOrderedListLine(str, false)) {
        while (i < str.length() && (str.charAt(i) >= '0' && str.charAt(i) <= '9')) {
          ++i;
        }
        ++i;
        while (i < str.length() && (str.charAt(i) == ' ' || str.charAt(i) == '\t')) {
          ++i;
        }
      } else {
        if (i + 1 < str.length() && str.charAt(i) == '\t') {
          return str.substring(i + 1);
        }
        while (i < str.length() && (str.charAt(i) == ' ') && i < 4) {
          ++i;
        }
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
      if (str.charAt(i) == '*' || str.charAt(i) == '-' || str.charAt(i) == '+') {
        ++i;
        while (i < str.length() && (str.charAt(i) == ' ' || str.charAt(i) == '\t')) {
          ++i;
        }
      } else {
        if (i + 1 < str.length() && str.charAt(i) == '\t') {
          return str.substring(i + 1);
        }
        while (i < str.length() && (str.charAt(i) == ' ') && i < 4) {
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
      HashMap<String, String[] dict = new HashMap<String, String[]>();
      return MarkdownText(str, depth, true, dict);
    }

    public static boolean IsListLine(String line, boolean ordered) {
      return ordered ? IsOrderedListLine(line) :
                    IsUnorderedListLine(line);
    }

    public static String StripItemStart(String line, boolean ordered) {
      return ordered ? StripOrderedListItemStart(line) :
                    StripListItemStart(line);
    }

    private static String MarkdownText(
      String str,
      int depth,
      boolean alwaysUseParas,
      Map<String, String[]> links
) {
      int i = 0;
      int lineStart = i;
      StringBuilder formatted = new StringBuilder();
      ArrayList<String> lines = new ArrayList<String>();
      StringBuilder paragraph = new StringBuilder();
      boolean haveParagraph = false;
      boolean isSingleParagraph = true;
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
      if (depth == 0) {
        // Get Markdown links
        for (; i < lines.size(); ++i) {
          String line = lines.get(i);
          String[] linkline = GetLinkLine(line);
          if (linkline != null) {
            lines.set(i,"");
            if (i + 1 < lines.size() && linkline[2] == null) {
              String title = GetLinkTitle(lines.get(i + 1));
              if (title != null) {
                linkline[2] = title;
                lines.set(i + 1,"");
              }
            }
            links.put(linkline[0], new String[] { linkline[1], linkline[2] });
          }
        }
      }
      i = 0;
      for (; i < lines.size(); ++i) {
        String line = lines.get(i);
        if (IsQuoteLine(line)) {
          // Quote
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int qi = i + 1;
          StringBuilder qs = new StringBuilder().append(StripQuoteStart(line));
          boolean haveAnotherQuoteLine = false;
          boolean valueGoToEndOfPara = false;
          while (qi < lines.size()) {
            line = lines.get(qi);
            if (IsQuoteLine(line)) {
              qs.append("\r\n").append(StripQuoteStart(line));
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
            while (qi < lines.size()) {
              line = lines.get(qi);
              if (IsQuoteLine(line)) {
                qs.append("\r\n").append(StripQuoteStart(line));
                ++qi;
              } else if (IsBlankishLine(line)) {
                break;
              } else {
                qs.append("\r\n").append(line);
                ++qi;
              }
            }
          }
          i = qi - 1;
          formatted.append("<blockquote>");
          if (depth > 10) {
            formatted.append("<pre><code>");
            formatted.append(HtmlEscape(qs.toString()));
            formatted.append("</code></pre>");
          } else {
         formatted.append(MarkdownText(qs.toString(), depth + 1, true,
              links));
          }
          formatted.append("</blockquote>");
        } else if (IsEqualsLine(line) && haveParagraph) {
          isSingleParagraph = false;
          haveParagraph = false;
          formatted.append("<h1>");
          formatted.append(paragraph.toString());
          formatted.append("</h1>");
        } else if (IsDashLine(line) && haveParagraph) {
          isSingleParagraph = false;
          haveParagraph = false;
          formatted.append("<h2>");
          formatted.append(paragraph.toString());
          formatted.append("</h2>");
        } else if (IsBarLine(line)) {
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          formatted.append("<hr/>");
        } else if (IsUnorderedListLine(line) || IsOrderedListLine(line)) {
          boolean ordered = IsOrderedListLine(line);
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int qi = i + 1;
          String strippedLine = StripItemStart(line, ordered);
          StringBuilder qs = new StringBuilder().append(strippedLine);
          formatted.append(ordered ? "<ol>" : "<ul>");
          formatted.append("<li>");
          int itemLineCount = 0;
          boolean seenBlankishLine = false;
          boolean wrapLinesInParas = false;

          // DebugUtility.Log("newlist");
          while (qi < lines.size()) {
            line = lines.get(qi);
            if (IsListLine(line, ordered)) {
              wrapLinesInParas |= seenBlankishLine;
              // DebugUtility.Log("para=" + qs.toString());
              String qss2 = MarkdownText(
        qs.toString(),
        depth + 1,
        wrapLinesInParas,
        links);
              formatted.append(qss2);
              formatted.append("</li><li>");
              qs.delete(0, (0)+(qs.length()));
              ++qi;
              strippedLine = StripItemStart(line, ordered);
              qs.append(strippedLine);
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
                  qs.append("\r\n");
                }
                seenBlankishLine = false;
                qs.append("\r\n").append(StripItemStart(line, ordered));
                // DebugUtility.Log("qs=" + qs);
                ++qi;
                ++itemLineCount;
              } else if (seenBlankishLine) {
                break;
              } else {
                if (seenBlankishLine) {
                  qs.append("\r\n");
                }
                seenBlankishLine = false;
                qs.append("\r\n").append(StripItemStart(line, ordered));
                // DebugUtility.Log("qs=" + qs);
                ++qi;
                ++itemLineCount;
              }
            }
          }
          i = qi - 1;
  String qss = MarkdownText(qs.toString(), depth + 1, wrapLinesInParas,
            links);
          formatted.append(qss);
          formatted.append("</li>");
          formatted.append(ordered ? "</ol>" : "</ul>");
        } else if (IsHeadingLine(line)) {
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int heading = HeadingLevel(line);
          String stripped = StripHeadingStart(line);
          formatted.append("<h").append((char)('0' + heading))
               .append(">").append(FormatParagraph(stripped, links))
               .append("</h").append((char)('0' + heading))
               .append(">");
        } else if (IsCodeBlockLine(line)) {
          isSingleParagraph = false;
          if (haveParagraph) {
            haveParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
          int qi = i + 1;
          StringBuilder qs = new StringBuilder().append(StripCodeBlockSpaces(line));
          while (qi < lines.size()) {
            line = lines.get(qi);
            if (IsCodeBlockLine(line)) {
              qs.append("\r\n").append(StripCodeBlockSpaces(line));
              ++qi;
            } else {
              break;
            }
          }
          i = qi - 1;
          formatted.append("<pre><code>");
          formatted.append(HtmlEscape(qs.toString()));
          formatted.append("</code></pre>");
        } else {
          if (!IsBlankishLine(line)) {
            if (haveParagraph) {
              paragraph.append("\r\n");
            } else {
              paragraph.delete(0, (0)+(paragraph.length()));
            }
            haveParagraph = true;
            paragraph.append(FormatParagraph(line, links));
          } else if (haveParagraph) {
            haveParagraph = false;
            isSingleParagraph = false;
            formatted.append("<p>");
            formatted.append(paragraph.toString());
            formatted.append("</p>");
          }
        }
      }
      if (haveParagraph) {
        haveParagraph = false;
        if (depth == 0 || !isSingleParagraph || alwaysUseParas) {
          formatted.append("<p>");
          formatted.append(paragraph.toString());
          formatted.append("</p>");
        } else {
          formatted.append(paragraph.toString());
        }
      }

      return formatted.toString();
    }
  }
