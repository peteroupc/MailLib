/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;

namespace PeterO.Mail {
  internal sealed class WordWrapEncoder {
    private const int MaxLineLength = 76;
    private readonly StringBuilder fullString;
    private readonly bool collapseSpaces;
    private string lastSpaces;
    private int lineLength;
    private bool haveNonwhitespace;

    public WordWrapEncoder(bool collapseSpaces) {
      this.fullString = new StringBuilder();
      this.collapseSpaces = collapseSpaces;
      this.haveNonwhitespace = true;
      this.lastSpaces = String.Empty;
      this.lineLength = this.fullString.Length;
    }

    private void AppendSpaces(string str) {
      if (this.lineLength + this.lastSpaces.Length + str.Length >
           MaxLineLength) {
        // Too big to fit the current line
        this.lastSpaces = " ";
      } else {
        this.lastSpaces = this.collapseSpaces ? " " : str;
      }
    }

    private void AppendWord(string str) {
      if (this.lineLength + this.lastSpaces.Length + str.Length >
        MaxLineLength) {
        if (this.haveNonwhitespace) {
          // Too big to fit the current line,
          // create a new line (but only if the current
          // line isn't all whitespace)
          if (this.fullString.Length > 0) {
            this.fullString.Append("\r\n");
            this.lastSpaces = " ";
          } else {
            this.lastSpaces = String.Empty;
          }
          this.haveNonwhitespace = false;
          this.lineLength = 0;
        } else {
          // Too big to fit the current line,
          // but the current line is all whitespace, so
          // shorten the lastSpaces to a single space
          this.lastSpaces = " ";
        }
      }
      this.fullString.Append(this.lastSpaces);
      this.fullString.Append(str);
      this.haveNonwhitespace = true;
      this.lineLength += this.lastSpaces.Length;
      this.lineLength += str.Length;
      this.lastSpaces = String.Empty;
    }

    private static string FoldSubstring(string str, int index, int length) {
      int endIndex = index + length;
      var nonSpace = false;
      for (int i = index; i < endIndex; ++i) {
        if (str[i] != 0x20 && str[i] != 0x09) {
          nonSpace = true;
          break;
        }
      }
      if (!nonSpace) {
        return str.Substring(index, length);
      }
      var sb = new StringBuilder();
      for (int i = index; i < endIndex; ++i) {
        if (str[i] == 0x20 || str[i] == 0x09) {
          sb.Append(str[i]);
        }
      }
      return sb.ToString();
    }

    public WordWrapEncoder AddString(string str) {
      var wordStart = 0;
      for (int j = 0; j < str.Length; ++j) {
        int c = str[j];
        if (c == 0x20 || c == 0x09 || (c == 0x0d && j + 1 < str.Length &&
          str[j + 1] == 0x0a)) {
          int wordEnd = j;
          if (wordStart != wordEnd) {
            this.AppendWord(str.Substring(wordStart, wordEnd - wordStart));
          }
          while (j < str.Length) {
            if (c == 0x0d && j + 1 < str.Length && str[j + 1] == 0x0a) {
              j += 2;
            } else if (str[j] == 0x20 || str[j] == 0x09) {
              ++j;
            } else {
              break;
            }
          }
          wordStart = j;
          // Fold the spaces by eliminating CRLF pairs, then append
          // what remains
          this.AppendSpaces(FoldSubstring(str, wordEnd, wordStart - wordEnd));
          --j;
        }
      }
      if (wordStart != str.Length) {
        this.AppendWord(str.Substring(wordStart, str.Length - wordStart));
      }
      return this;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.WordWrapEncoder.ToString"]/*'/>
    public override string ToString() {
      return this.fullString.ToString();
    }
  }
}
