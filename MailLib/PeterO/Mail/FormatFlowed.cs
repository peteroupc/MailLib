using System;
using System.Text;
namespace PeterO.Mail {
  internal static class FormatFlowed {
    private static string HtmlEscape(string str) {
      int i = 0;
      for (; i < str.Length; i++) {
        if (str[i] == '&' || str[i] == '<' || str[i] == '>') {
          break;
        }
      }
      if (i == str.Length) return str;
      var sb = new StringBuilder();
      sb.Append(str.Substring(0, i));
      for (; i < str.Length; i++) {
        if (str[i] == '&') sb.Append("&amp;");
        else if (str[i] == '<') sb.Append("&lt;");
        else if (str[i] == '>') sb.Append("&gt;");
        else sb.Append(str[i]);
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
      int i = 0;
      int lineStart = i;
      int lastQuotes = 0;
      StringBuilder paragraph = new StringBuilder();
      StringBuilder formatted = new StringBuilder();
      bool haveParagraph = false;
      while (i <= str.Length) {
        if (i == str.Length || (str[i] == 0x0d && i + 1 < str.Length && str[i + 1] == 0x0a)) {
          bool lastLine = (i == str.Length);
          int lineEnd = i;
          int index = lineStart;
          int quotes = 0;
          bool flowed = false;
          bool signature = false;
          while (index < lineEnd && str[index] == '>') {
            quotes++;
            index++;
          }
          if (index < lineEnd && str[index] == ' ') {
            // Space stuffing
            index++;
          }
          if (index + 3 == lineEnd && str[index] == '-' &&
             str[index + 1] == '-' && str[index + 2] == ' ') {
            signature = true;
          } else if (index < lineEnd && str[lineEnd - 1] == ' ') {
            flowed = true;
            if (delSp) lineEnd--;
          }
          bool endedParagraph = false;
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
            formatted.Append("<p>")
                     .Append(HtmlEscape(paragraph.ToString()));
            haveParagraph = false;
            endedParagraph = true;
            paragraph.Remove(0, paragraph.Length);
            formatted.Append(
                HtmlEscape(str.Substring(index, lineEnd - index)))
                     .Append("</p>");
          }
          if (lastQuotes < quotes) {
            for (var k = lastQuotes; k < quotes; k++) {
              formatted.Append("<blockquote>");
            }
          } else if (quotes < lastQuotes) {
            for (var k = quotes; k < lastQuotes; k++) {
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
            for (var k = 0; k < quotes; k++) {
              formatted.Append("</blockquote>");
            }
          }
          lineStart = i + 2;
          lastQuotes = quotes;
          if (i == str.Length) break;
        }
        i++;
      }
      return formatted.ToString();
    }

  }
}
