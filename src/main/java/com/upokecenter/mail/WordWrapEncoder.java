package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

  final class WordWrapEncoder {
    private static final int MaxLineLength = 76;
    private String lastSpaces;
    private final StringBuilder fullString;
    private int lineLength;
    private final boolean collapseSpaces;
    private boolean haveNonwhitespace;

    public WordWrapEncoder (String c) {
 this(c, true);
    }

    public WordWrapEncoder (String c, boolean collapseSpaces) {
      if (c == null) {
        throw new NullPointerException("c");
      }
      this.fullString = new StringBuilder();
      this.fullString.append(c);
      this.collapseSpaces = collapseSpaces;
      if (this.fullString.length() >= MaxLineLength) {
        this.fullString.append("\r\n");
        this.lastSpaces = " ";
        this.haveNonwhitespace = false;
      } else {
        this.haveNonwhitespace = true;  // assume have nonwhitespace
        this.lastSpaces = c.length() == 0 ? "" : " ";
        this.lineLength = this.fullString.length();
      }
    }

    private void AppendSpaces(String str) {
   if (this.lineLength + this.lastSpaces.length() + str.length() >
        MaxLineLength) {
        // Too big to fit the current line
        this.lastSpaces = " ";
      } else {
        this.lastSpaces = this.collapseSpaces ? " " : str;
      }
    }

    private void AppendWord(String str) {
   if (this.lineLength + this.lastSpaces.length() + str.length() >
        MaxLineLength) {
        if (this.haveNonwhitespace) {
          // Too big to fit the current line,
          // create a new line (but only if the current
          // line isn't all whitespace)
          this.fullString.append("\r\n");
          this.lastSpaces = " ";
          this.haveNonwhitespace = false;
          this.lineLength = 0;
        } else {
          // Too big to fit the current line,
          // but the current line is all whitespace, so
          // shorten the lastSpaces to a single space
          this.lastSpaces = " ";
        }
      }
      this.fullString.append(this.lastSpaces);
      this.fullString.append(str);
      this.haveNonwhitespace = true;
      this.lineLength += this.lastSpaces.length();
      this.lineLength += str.length();
      this.lastSpaces = "";
    }

    private static String FoldSubstring(String str, int index, int length) {
      int endIndex = index + length;
      boolean nonSpace = false;
      for (int i = index; i < endIndex; ++i) {
        if (str.charAt(i) != 0x20 && str.charAt(i) != 0x09) {
          nonSpace = true;
          break;
        }
      }
      if (!nonSpace) {
        return str.substring(index, (index)+(length));
      }
      StringBuilder sb = new StringBuilder();
      for (int i = index; i < endIndex; ++i) {
        if (str.charAt(i) == 0x20 || str.charAt(i) == 0x09) {
          sb.append(str.charAt(i));
        }
      }
      return sb.toString();
    }

    public WordWrapEncoder AddString(String str) {
      int wordStart = 0;
      for (int j = 0; j < str.length(); ++j) {
        int c = str.charAt(j);
        if (c == 0x20 || c == 0x09 || (c == 0x0d && j + 1 < str.length() &&
          str.charAt(j + 1) == 0x0a)) {
          int wordEnd = j;
          if (wordStart != wordEnd) {
            this.AppendWord(str.substring(wordStart, (wordStart)+(wordEnd - wordStart)));
          }
          while (j < str.length()) {
            if (c == 0x0d && j + 1 < str.length() && str.charAt(j + 1) == 0x0a) {
              j += 2;
            } else if (str.charAt(j) == 0x20 || str.charAt(j) == 0x09) {
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
      if (wordStart != str.length()) {
        this.AppendWord(str.substring(wordStart, (wordStart)+(str.length() - wordStart)));
      }
      return this;
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.fullString.toString();
    }
  }
