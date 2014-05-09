package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

    /**
     * Description of ContentDisposition.
     */
  public class ContentDisposition
  {
    private String dispositionType;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public String getDispositionType() {
        return this.dispositionType;
      }

    @Override public boolean equals(Object obj) {
      ContentDisposition other = ((obj instanceof ContentDisposition) ? (ContentDisposition)obj : null);
      if (other == null) {
        return false;
      }
      return this.dispositionType == other.dispositionType &&
        CollectionUtilities.MapEquals(this.parameters, other.parameters);
    }

    /**
     * Returns the hash code for this instance.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 632580499;
      {
        if (this.dispositionType != null) {
          valueHashCode += 632580503 * this.dispositionType.hashCode();
        }
        if (this.parameters != null) {
          valueHashCode += 632580587 * this.parameters.size();
        }
      }
      return valueHashCode;
    }

    /**
     * Gets a value indicating whether the disposition type is inline.
     * @return Whether the disposition type is inline.
     */
    public boolean isInline() {
        return this.dispositionType.equals("inline");
      }

    /**
     * Gets a value indicating whether the disposition type is attachment.
     * @return Whether the disposition type is attachment.
     */
    public boolean isAttachment() {
        return this.dispositionType.equals("attachment");
      }

    ContentDisposition(String type, Map<String, String> parameters) {
      this.dispositionType = type;
      this.parameters = new TreeMap<String, String>(parameters);
    }

    private TreeMap<String, String> parameters;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public Map<String, String> getParameters() {
        return java.util.Collections.unmodifiableMap(this.parameters);
      }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      StringBuilder sb = new StringBuilder();
      sb.append(this.dispositionType);
      MediaType.AppendParameters(this.parameters, sb);
      return sb.toString();
    }

    private static String RemoveEncodedWordEnds(String str) {
      StringBuilder sb = new StringBuilder();
      int index = 0;
      boolean inEncodedWord = false;
      while (index<str.length()) {
        if (!inEncodedWord && index+1<str.length() && str.charAt(index)=='=' && str.charAt(index+1)=='?') {
          // Remove start of encoded word
          inEncodedWord = true;
          index+=2;
          int qmarks = 0;
          // skip charset and encoding
          while (index<str.length()) {
            if (str.charAt(index)=='?') {
              ++qmarks;
              ++index;
              if (qmarks == 2) {
 break;
}
            } else {
              ++index;
            }
          }
          inEncodedWord = true;
        } else if (inEncodedWord && index+1<str.length() && str.charAt(index)=='?' && str.charAt(index+1)=='=') {
          // End of encoded word
          index+=2;
          inEncodedWord = false;
        } else {
          // Everything else {
 int c = DataUtilities.CodePointAt(str, index);
}
          if (c == 0xfffd) {
            sb.append(0xfffd);
            ++index;
          } else {
            sb.append(str.charAt(index++));
            if (c >= 0x10000) {
              sb.append(str.charAt(index++));
            }
          }
        }
      }
      return sb.toString();
    }

    /**
     * Converts a filename from the Content-Disposition header to a suitable
     * name for saving data to a file.
     * @param str A string representing a file name.
     * @return A string object.
     */
    public static String MakeFilename(String str) {
      if (str == null) {
        return "";
      }
      str = ParserUtility.TrimSpaceAndTab(str);
      if (str.indexOf("=?")>= 0) {
        // May contain encoded words, which are very frequent
        // in Content-Disposition filenames
        str = Rfc2047.DecodeEncodedWords(str, 0, str.length(), EncodedWordContext.Unstructured);
        if (str.indexOf("=?")>= 0) {
          // Remove ends of encoded words that remain
          str = RemoveEncodedWordEnds(str);
        }
      }
      str = ParserUtility.TrimSpaceAndTab(str);
      // NOTE: Even if there are directory separators (backslash
      // and forward slash), the filename is not treated as a
      // file system path (in accordance with sec. 2.3 or RFC
      // 2183); as a result, the directory separators
      // will be treated as unsuitable characters for filenames
      // and are handled below.
      if (str.length() == 0) {
        return "_";
      }
      StringBuilder builder = new StringBuilder();
      // Replace unsuitable characters for filenames
      // and make sure the filename's
      // length doesn't exceed 250
      for (int i = 0;i<str.length() && builder.length()<250; ++i) {
        int c = DataUtilities.CodePointAt(str, i);
        if (c >= 0x10000) {
 ++i;
}
        if (c==(int)'\t') {
          // Replace tab with space
          builder.append(' ');
        } else if (c<0x20 || c=='\\' || c=='/' || c=='*' || c=='?' || c=='|' ||
                  c==':' || c=='<' || c=='>' || c=='"' || c==0x7f) {
          // Unsuitable character for a filename
          builder.append('_');
        } else {
          if (builder.length()<249 || c<0x10000) {
            if (c <= 0xffff) {
              builder.append((char)c);
            } else if (c <= 0x10ffff) {
              builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
              builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
            }
          }
        }
      }
      str = builder.toString();
      str = ParserUtility.TrimSpaceAndTab(str);
      if (str.length() == 0) {
        return "_";
      }
      if (str.charAt(str.length()-1)=='.') {
        // Ends in a dot
        str+="_";
      }
      String strLower = DataUtilities.ToLowerCaseAscii(str);
      if (strLower.equals("nul") ||
         strLower.indexOf("nul.")==0 ||
         strLower.equals("prn") ||
         strLower.indexOf("prn.")==0 ||
         strLower.equals("aux") ||
         strLower.indexOf("aux.")==0 ||
         strLower.equals("con") ||
         strLower.indexOf("con.")==0 ||
         (strLower.length()>= 4 && strLower.indexOf("lpt")==0 && strLower.charAt(3)>= '1' && strLower.charAt(3)<= '9') ||
         (strLower.length()>= 4 && strLower.indexOf("com")==0 && strLower.charAt(3)>= '1' && strLower.charAt(3)<= '9')
) {
        // Reserved filenames on Windows
        str="_"+str;
      }
      if (str.charAt(0)=='~') {
        // Home folder convention
        str="_"+str;
      }
      if (str.charAt(0)=='.') {
        // Starts with period; may be hidden in some configurations
        str="_"+str;
      }
      return Normalizer.Normalize(str, Normalization.NFC);
    }

    /**
     * Not documented yet.
     * @param name A string object. (2).
     * @return A string object.
     */
    public String GetParameter(String name) {
      if (name == null) {
        throw new NullPointerException("name");
      }
      if (name.length() == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (this.parameters.containsKey(name)) {
        return this.parameters.get(name);
      }
      return null;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A Boolean object.
     */
    private boolean ParseDisposition(String str) {
      boolean httpRules = false;
      int index = 0;
      if (str == null) {
        throw new NullPointerException("str");
      }
      int endIndex = str.length();
      if (httpRules) {
        index = MediaType.skipLws(str, index, endIndex);
      } else {
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      }
      int i = MediaType.SkipMimeToken(str, index, endIndex, null, httpRules);
      if (i == index) {
        return false;
      }
      this.dispositionType = DataUtilities.ToLowerCaseAscii(str.substring(index,(index)+(i - index)));
      if (i < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i, endIndex, null);
        if (i3 == endIndex) {
          // at end
          return true;
        }
        if (i3 < endIndex && str.charAt(i3) != ';') {
          // not followed by ";", so not a media type
          return false;
        }
      }
      index = i;
      return MediaType.ParseParameters(str, index, endIndex, httpRules, this.parameters);
    }

    private static ContentDisposition Build(String name) {
      ContentDisposition dispo = new ContentDisposition();
      dispo.parameters = new TreeMap<String, String>();
      dispo.dispositionType = name;
      return dispo;
    }

    public static final ContentDisposition Attachment = Build("attachment");

    public static final ContentDisposition Inline = Build("inline");

    private ContentDisposition() {
    }

    /**
     * Parses a media type string and returns a media type object.
     * @param dispoValue A string object.
     * @return A media type object, or "Attachment" if {@code dispoValue}
     * is empty or syntactically invalid.
     */
    public static ContentDisposition Parse(String dispoValue) {
      return Parse(dispoValue, Attachment);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param defaultValue Can be null.
     * @return A ContentDisposition object.
     */
    public static ContentDisposition Parse(String str, ContentDisposition defaultValue) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      ContentDisposition dispo = new ContentDisposition();
      dispo.parameters = new TreeMap<String, String>();
      if (!dispo.ParseDisposition(str)) {

        return defaultValue;
      }
      return dispo;
    }
  }
