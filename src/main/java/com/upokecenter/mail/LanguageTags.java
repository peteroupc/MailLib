package com.upokecenter.mail;

import java.util.*;

import com.upokecenter.util.*;

    /**
     * Contains methods for parsing and matching language tags.
     */
  public final class LanguageTags {
private LanguageTags() {
}
    private static String[] SplitAt(String str, String delimiter) {
      if (delimiter == null) {
        throw new NullPointerException("delimiter");
      }
      if (delimiter.length() == 0) {
        throw new IllegalArgumentException("delimiter is empty.");
      }
      if (((str) == null || (str).length() == 0)) {
        return new String[] { "" };
      }
      int index = 0;
      boolean first = true;
      ArrayList<String> strings = null;
      int delimLength = delimiter.length();
      while (true) {
        int index2 = str.indexOf(delimiter, index);
        if (index2 < 0) {
          if (first) {
            String[] strret = new String[1];
            strret[0] = str;
            return strret;
          }
          strings = (strings == null) ? ((new ArrayList<String>())) : strings;
          strings.add(str.substring(index));
          break;
        } else {
          first = false;
          String newstr = str.substring(index, (index)+(index2 - index));
          strings = (strings == null) ? ((new ArrayList<String>())) : strings;
          strings.add(newstr);
          index = index2 + delimLength;
        }
      }
      return strings.toArray(new String[] { });
    }

    /**
     * Returns whether the given string is a basic language range under RFC 4647.
     * Examples include "&#x2a;", "en-us", and "fr".
     * @param str The string to check. Can be null.
     * @return {@code true} if the given string is a basic language range;
     * otherwise, {@code false} .
     */
    public static boolean IsLanguageRange(String str) {
      return IsLanguageRange(str, false);
    }

    /**
     * Returns whether the given string is a basic or extended language range under
     * RFC 4647. Examples of basic (and extended) language ranges include
     * "&#x2a;", "en-us", and "fr". Examples of extended language ranges
     * include "&#x2a;-de" and "it-&#x2a;".
     * @param str The string to check. Can be null.
     * @param extended Check whether the string is a basic language range if
     * "false", or an extended language range if "true".
     * @return {@code true} if the given string is a basic language range
     * (depending on the {@code extended} parameter); otherwise, {@code
     * false} .
     */
    public static boolean IsLanguageRange(String str, boolean extended) {
      if (((str) == null || (str).length() == 0)) {
 return false;
}
      if (str.length() == 1 && str.charAt(0) == '*') {
 return true;
}
      int count = 0;
      int i = 0;
      boolean first = true;
      while (i < str.length()) {
        char c = str.charAt(i);
        if (i > 0 && c == '-' && i + 1 < str.length() && str.charAt(i + 1) != '-') {
          count = 0;
          ++i;
          first = false;
          continue;
        }
        if (extended && c == '*' && count == 0 && (i + 1 == str.length() ||
          str.charAt(i + 1) == '-')) {
          ++count;
          ++i;
          continue;
        }
        if (first && !((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))) {
 return false;
}
        if (!first && !((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
          (c >= '0' && c <= '9'))) {
 return false;
}
        if (count >= 8) {
 return false;
}
        ++count;
        ++i;
      }
      return true;
    }

    private static int SkipFWS(String str, int index, int endIndex) {
      // NOTE: Includes obsolete syntax under RFC 5322 (with errata)
      while (index < endIndex) {
    if (endIndex - index > 1 && str.charAt(index) == 0x0d && str.charAt(index + 1) ==
          0x0a) {
 index += 2;
}
        if (str.charAt(index) == 0x09 || str.charAt(index) == 0x20) {
 ++index;
} else {
 break;
}
      }
      return index;
    }

    private static int SkipCFWS(String str, int index, int endIndex) {
      // NOTE: Includes obsolete syntax under RFC 5322 (with errata)
      while (index < endIndex) {
        int oldindex = index;
        index = SkipFWS(str, index, endIndex);
        index = ParseCommentLax(str, index, endIndex);
        index = SkipFWS(str, index, endIndex);
        if (oldindex == index) {
 break;
}
      }
      return index;
    }

    // Parses a comment using the obsolete syntax.
    private static int ParseCommentLax(
  String str,
  int index,
  int endIndex) {
      int indexStart = index;
      int depth = 0;
      if (index < endIndex && (str.charAt(index) == 40)) {
        ++index;
      } else {
        return index;
      }
      while (index < endIndex) {
        index = SkipFWS(str, index, endIndex);
        boolean backslash = index < endIndex && str.charAt(index) == '\\';
        if (backslash) {
 ++index;
}
        if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <=
          56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
          index += 2;
        } else if (!backslash && index < endIndex && ((str.charAt(index) >= 1 &&
          str.charAt(index) <= 8) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) ||
          (str.charAt(index) >= 14 && str.charAt(index) <= 31) || (str.charAt(index) >= 33 &&
          str.charAt(index) <= 39) || (str.charAt(index) >= 42 && str.charAt(index) <= 91) ||
          (str.charAt(index) >= 93 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 &&
          str.charAt(index) <= 65535))) {
          ++index;
        } else if (backslash && index < endIndex && ((str.charAt(index) >= 0 &&
          str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <=
          65535))) {
          // NOTE: Includes parentheses, which are also handled
          // in later conditions
          ++index;
        } else if (index < endIndex && str.charAt(index) == 41) {
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
      }
      return indexStart;
    }

    /**
     * Sets the given language tag to the case combination recommended by RFC 5646.
     * For example, "en-us" becomes "en-US", and "zh-hant" becomes
     * "zh-Hant".
     * @param str A string of a language tag. Can be null.
     * @return A text string in the recommended case combination, or null if {@code
     * str} is null.
     */
    public static String LanguageTagCase(String str) {
      if (((str) == null || (str).length() == 0)) {
 return str;
}
      if (str.indexOf('-') == -1) {
 return DataUtilities.ToLowerCaseAscii(str);
}
      int count = 0;
      int i = 0;
      int lastSubtagLength = -1;
      StringBuilder sb = new StringBuilder();
      while (i <= str.length()) {
        if (i == str.length() || str.charAt(i) == '-') {
          if (count == 4 && lastSubtagLength >= 2) {
            sb.append(DataUtilities.ToUpperCaseAscii(str.substring(i - 4, (i - 4)+(1))));
            sb.append(DataUtilities.ToLowerCaseAscii(str.substring(i - 3, (i - 3)+(3))));
          } else if (count == 2 && lastSubtagLength >= 2) {
            sb.append(DataUtilities.ToUpperCaseAscii(str.substring(i - 2, (i - 2)+(2))));
          } else {
    sb.append(
  DataUtilities.ToLowerCaseAscii(
  str.substring(
  i - count, (
  i - count)+(count))));
          }
          lastSubtagLength = count;
          count = 0;
          ++i;
          if (i < str.length()) {
 sb.append('-');
}
          continue;
        }
        ++count;
        ++i;
      }
      return sb.toString();
    }

    private static int SkipLDHSequence(String str, int index, int endIndex) {
      while (index < endIndex) {
        if ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 &&
          str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) ||
          (str.charAt(index) == 45)) {
          ++index;
        } else {
 break;
}
      }
      return index;
    }

    /**
     * Parses a language list from a Content-Language header field.
     * @param str A string following the syntax of a Content-Language header field
     * (see RFC 3282). This is a comma-separated list of language tags. RFC
     * 5322 comments (in parentheses) can appear. This parameter can be
     * null.
     * @return A list of language tags. Returns an empty list if {@code str} is
     * null or the empty string, or null if {@code str} syntactically
     * invalid.
     */
    public static List<String> GetLanguageList(
      String str) {
      String tag = null;
      boolean first = true;
      int index = 0;
      ArrayList<String> ret = new ArrayList<String>();
      if (((str) == null || (str).length() == 0)) {
 return ret;
}
      while (index < str.length()) {
        if (!first) {
          if (index < str.length() && str.charAt(index) == ',') { index++;
} else {
 return null;
}
        }
        first = false;
        index = SkipCFWS(str, index, str.length());
        int newindex = SkipLDHSequence(str, index, str.length());
        if (newindex == index) {
 return null;
}
        tag = str.substring(index, (index)+(newindex - index));
        if (!IsPotentiallyValidLanguageTag(tag)) {
 return null;
}
        ret.add(tag);
        index = SkipCFWS(str, newindex, str.length());
      }
      return ret;
    }

    private static void SortQualityList(List<StringAndQuality> list) {
      // Do an insertion sort by descending quality.
      // Insertion sort is used even though it has
      // quadratic complexity, because it's stable
      // (it preserves the order of equal items) and
      // because the size of language
      // priority lists is expected to be small.
      for (int i = 1; i < list.size(); ++i) {
        StringAndQuality saq = list.get(i);
        int k = -1;
        for (k = i - 1; k >= 0; --k) {
          boolean cmp = list.get(k).getQuality() < saq.getQuality();
          if (cmp) {
 list.set(k + 1, list.get(k));
} else {
 break;
}
        }
        list.set(k, saq);
      }
    }

    /**
     * Parses a language range list from an Accept-Language header field.
     * @param str A string following the syntax of an Accept-Language header field
     * (see RFC 3282). This is a comma-separated list of language ranges,
     * with an optional "quality" after the language tag (examples include
     * "en; q=0.5" or "de-DE"). RFC 5322 comments (in parentheses) can
     * appear. This parameter can be null.
     * @return A list of language ranges with their associated qualities. The list
     * will be sorted in descending order by quality; if two or more
     * language ranges have the same quality, they will be sorted in the
     * order in which they appeared in the given string. Returns null if
     * {@code str} is null or syntactically invalid.
     */
    public static List<StringAndQuality> GetRangeListWithQuality(
      String str) {
      String tag = null;
      boolean first = true;
      int index = 0;
      ArrayList<StringAndQuality> ret = new ArrayList<StringAndQuality>();
if (str == null) {
 return null;
}
      while (index < str.length()) {
        if (!first) {
          if (index < str.length() && str.charAt(index) == ',') { index++;
} else {
 return null;
}
        }
        first = false;
        index = SkipCFWS(str, index, str.length());
        int newindex;
        newindex = (index < str.length() && str.charAt(index) == '*') ? (index + 1) :
          SkipLDHSequence(str, index, str.length());
        if (newindex == index) {
 return null;
}
        tag = str.substring(index, (index)+(newindex - index));
        if (!IsLanguageRange(tag)) {
 return null;
}
        index = SkipCFWS(str, newindex, str.length());
        if (index < str.length() && str.charAt(index) == ';') {
          index = SkipCFWS(str, newindex, str.length());
          if (index < str.length() && str.charAt(index) == 'q') {
 ++index;
} else {
 return null;
}
          index = SkipCFWS(str, newindex, str.length());
          if (index < str.length() && str.charAt(index) == '=') {
 ++index;
} else {
 return null;
}
          if (index < str.length() && (str.charAt(index) == '0' || str.charAt(index) == '1')) {
            int qvalue = (str.charAt(index) == '0') ? 0 : 1000;
            ++index;
            if (index < str.length() && str.charAt(index) == '.') {
              ++index;
              int[] mults = { 100, 10, 1 };
              for (int i = 0; i < 3; ++i) {
          if (index < str.length() && (str.charAt(index) >= '0' || str.charAt(index) <= '9'
)) {
                  qvalue += mults[i] * (str.charAt(index) - '0');
                  ++index;
                } else {
 break;
}
              }
            }
            if (qvalue < 0 || qvalue > 1000) {
 return null;
}
            ret.add(new StringAndQuality(tag, qvalue));
            index = SkipCFWS(str, newindex, str.length());
          } else {
 return null;
}
        } else {
          ret.add(new StringAndQuality(tag, 1000));
        }
      }
      SortQualityList(ret);
      return ret;
    }

    private static boolean MatchLangTagBasic(
  String rangeLowerCased,
  String tagLowerCased) {
      if (rangeLowerCased.equals("*")) {
 return true;
}
      if (rangeLowerCased.equals(tagLowerCased)) {
        return true;
      }
      if (tagLowerCased.length() > rangeLowerCased.length() &&
          tagLowerCased.charAt(rangeLowerCased.length()) == '-') {
        String prefix = tagLowerCased.substring(0, rangeLowerCased.length());
        if (rangeLowerCased.equals(prefix)) {
          return true;
        }
      }
      return false;
    }

    private static boolean MatchLangTagExtended(
  String rangeLowerCased,
  String tagLowerCased) {
      String[] rangeSub = SplitAt(rangeLowerCased, "-");
      String[] tagSub = SplitAt(tagLowerCased, "-");
      if (rangeSub.length == 0 || tagSub.length == 0) {
        return false;
      }
      if (!rangeSub[0].equals("*") && !rangeSub[0].equals(tagSub[0])) {
        return false;
      }
      int rangeIndex = 1;
      int tagIndex = 1;
      while (rangeIndex < rangeSub.length) {
     String range = rangeSub[rangeIndex];
        if (range.length() == 0) {
          return false;
        }
        if (range.equals("*")) {
          continue;
        }
        if (tagIndex >= tagSub.length) {
          return false;
        }
String tag = tagSub[tagIndex];
        if (range.equals(tag)) {
          ++rangeIndex;
          ++tagIndex;
        }
        if (tag.length() == 1) {
          return false;
        }
        ++tagIndex;
      }
      return true;
    }

    /**
     * Finds the language tags that match a priority list of language ranges.
     * @param ranges A list of language ranges (see documentation for the
     * "IsLanguageRange" method), which should be given in order of
     * descending preference.
     * @param languages A list of language tags, which should be given in order of
     * descending preference.
     * @param extended If true, the ranges in "ranges" are extended language
     * ranges; otherwise, they are basic language ranges.
     * @param matchStarAtEnd If true, treats any range equaling "&#x2a;" as
     * appearing at the end of the language priority list, no matter where
     * it appears on that list.
     * @return A list of language tags that match the given range, in descending
     * order of preference.
     * @throws java.lang.NullPointerException The parameter {@code languages} or
     * {@code ranges} is null.
     * @throws IllegalArgumentException The parameter {@code ranges} contains a
     * value that is not a basic or extended language range, or {@code
     * languages} contains a value that is not a potentially valid language
     * tag.
     */
    public static List<String> LanguageTagFilter(
           List<String> ranges,
           List<String> languages,
           boolean extended,
           boolean matchStarAtEnd) {
      if (ranges == null) {
  throw new NullPointerException("ranges");
}
      if (languages == null) {
  throw new NullPointerException("languages");
}
      ArrayList<String> retlist = new ArrayList<String>();
      if (ranges.size() == 0) {
 return retlist;
}
      if (languages.size() == 0) {
 return retlist;
}
      boolean hasStar = false;
      boolean[] langsMatch = new boolean[languages.size()];
      for (String range : ranges) {
        if (!IsLanguageRange(range, extended)) {
 throw new IllegalArgumentException("ranges");
}
      }
      for (String lang : languages) {
        if (!IsPotentiallyValidLanguageTag(lang)) {
 throw new IllegalArgumentException("languages");
}
      }
      for (String range : ranges) {
        if (matchStarAtEnd && range.equals("*")) {
          hasStar = true;
          continue;
        }
        String lcrange = DataUtilities.ToLowerCaseAscii(range);
        for (int k = 0; k < languages.size(); ++k) {
          if (langsMatch[k]) {
 continue;
}
          String lclang = DataUtilities.ToLowerCaseAscii(languages.get(k));
          if (extended) {
            if (MatchLangTagExtended(lcrange, lclang)) {
              retlist.add(languages.get(k));
              langsMatch[k] = true;
            }
          } else {
            if (MatchLangTagBasic(lcrange, lclang)) {
              retlist.add(languages.get(k));
              langsMatch[k] = true;
            }
          }
        }
      }
      if (matchStarAtEnd && hasStar) {
        for (int k = 0; k < languages.size(); ++k) {
          if (!langsMatch[k]) {
 retlist.add(languages.get(k));
}
        }
      }
      return retlist;
    }

    private static String TruncateLangRange(String range) {
     int i = 0;
      for (i = range.length() - 1; i >= 0; --i) {
 if (range.charAt(i) == '-' && i >= 2 && range.charAt(i - 1) != '-' && range.charAt(i - 2) != '-'
) {
 return range.substring(0, i);
}
      }
      return "";
    }

    /**
     * Determines whether the given language tag matches the given language range.
     * @param range A basic language range (see the documentation for
     * "IsLanguageRange").
     * @param tag A language tag.
     * @return {@code true} if the language tag matches the language range by the
     * filtering method under RFC 4647; otherwise, {@code false} .
     * @throws IllegalArgumentException The parameter {@code range} is not a basic
     * language range, or {@code tag} is not a potentially valid language
     * tag.
     */
    public static boolean MatchesLanguageTag(String range, String tag) {
      List<String> tags = LanguageTagFilter(
        Arrays.asList(new String[] { range }),
        Arrays.asList(new String[] { tag }),
        false,
 false);
      return tags.size() > 0;
    }

    /**
     * Does a language tag lookup (under RFC 4647) for a matching language tag.
     * @param range A basic language range (see the documentation for
     * "IsLanguageRange").
     * @param languages A list of language tags, which should be given in order of
     * descending preference.
     * @param defaultValue The value to return if no matching language tag was
     * found.
     * @return The matching language tag, or the parameter {@code defaultValue} if
     * there is no matching language tag.
     * @throws java.lang.NullPointerException The parameter {@code languages} is
     * null.
     * @throws IllegalArgumentException The parameter {@code range} is not a basic
     * language range, or {@code languages} contains a value that is not a
     * potentially valid language tag.
     */
    public static String LanguageTagLookup(
  String range,
  List<String> languages,
  String defaultValue) {
      return LanguageTagLookup(range, languages, defaultValue, false);
    }

    /**
     * Does a language tag lookup (under RFC 4647) for a matching language tag.
     * @param ranges A list of basic language ranges (see documentation for the
     * "IsLanguageRange" method), which should be given in order of
     * descending preference.
     * @param languages A list of language tags, which should be given in order of
     * descending preference.
     * @param defaultValue The value to return if no matching language tag was
     * found.
     * @return The matching language tag, or the parameter {@code defaultValue} if
     * there is no matching language tag.
     * @throws java.lang.NullPointerException The parameter {@code languages} or
     * {@code ranges} is null.
     * @throws IllegalArgumentException The parameter {@code ranges} contains a
     * value that is not a basic language range, or {@code languages}
     * contains a value that is not a potentially valid language tag.
     */
    public static String LanguageTagLookup(
  List<String> ranges,
  List<String> languages,
  String defaultValue) {
      return LanguageTagLookup(ranges, languages, defaultValue, false);
    }

    /**
     * Finds the language tags that match a priority list of basic language ranges.
     * @param ranges A list of basic language ranges (see documentation for the
     * "IsLanguageRange" method), which should be given in order of
     * descending preference.
     * @param languages A list of language tags, which should be given in order of
     * descending preference.
     * @return A list of language tags that match the given range, in descending
     * order of preference.
     * @throws java.lang.NullPointerException The parameter {@code languages} or
     * {@code ranges} is null.
     * @throws IllegalArgumentException The parameter {@code ranges} contains a
     * value that is not a basic language range, or {@code languages}
     * contains a value that is not a potentially valid language tag.
     */
    public static List<String> LanguageTagFilter(
  List<String> ranges,
  List<String> languages) {
      return LanguageTagFilter(ranges, languages, false, false);
    }

    /**
     * Does a language tag lookup (under RFC 4647) for a matching language tag.
     * @param range A language range (see the documentation for "IsLanguageRange").
     * @param languages A list of language tags, which should be given in order of
     * descending preference.
     * @param defaultValue The value to return if no matching language tag was
     * found.
     * @param extended If true, "range" is an extended language range; otherwise,
     * it's a are basic language range.
     * @return The matching language tag, or the parameter {@code defaultValue} if
     * there is no matching language tag.
     * @throws java.lang.NullPointerException The parameter {@code languages} is
     * null.
     * @throws IllegalArgumentException The parameter {@code range} is not a basic
     * or extended language range, or {@code languages} contains a value
     * that is not a potentially valid language tag.
     */
    public static String LanguageTagLookup(
  String range,
  List<String> languages,
  String defaultValue,
  boolean extended) {
      return LanguageTagLookup(
        Arrays.asList(new String[] { range }),
        languages,
        defaultValue,
        extended);
    }

    /**
     * Does a language tag lookup (under RFC 4647) for a matching language tag.
     * @param ranges A list of language ranges (see documentation for the
     * "IsLanguageRange" method), which should be given in order of
     * descending preference.
     * @param languages A list of language tags, which should be given in order of
     * descending preference.
     * @param defaultValue The value to return if no matching language tag was
     * found.
     * @param extended If true, the ranges in "ranges" are extended language
     * ranges; otherwise, they are basic language ranges.
     * @return The matching language tag, or the parameter {@code defaultValue} if
     * there is no matching language tag.
     * @throws java.lang.NullPointerException The parameter {@code languages} or
     * {@code ranges} is null.
     * @throws IllegalArgumentException The parameter {@code ranges} contains a
     * value that is not a basic or extended language range, or {@code
     * languages} contains a value that is not a potentially valid language
     * tag.
     */
    public static String LanguageTagLookup(
         List<String> ranges,
         List<String> languages,
         String defaultValue,
         boolean extended) {
      if (ranges == null) {
  throw new NullPointerException("ranges");
}
      if (languages == null) {
  throw new NullPointerException("languages");
}
      if (ranges.size() == 0) {
 return defaultValue;
}
      if (languages.size() == 0) {
 return defaultValue;
}
      for (String range : ranges) {
        if (!IsLanguageRange(range, extended)) {
 throw new IllegalArgumentException("ranges");
}
      }
      for (String lang : languages) {
        if (!IsPotentiallyValidLanguageTag(lang)) {
 throw new IllegalArgumentException("languages");
}
      }
      for (String range : ranges) {
        if (range.equals("*")) {
 continue;
}
        String lcrange = DataUtilities.ToLowerCaseAscii(range);
        while (lcrange.length() > 0) {
          for (String lang : languages) {
            String lclang = DataUtilities.ToLowerCaseAscii(lang);
            if (extended) if (MatchLangTagExtended(lcrange, lclang)) {
 return lang;
  } else if (MatchLangTagBasic(lcrange, lclang)) {
 return lang;
}
          }
          lcrange = TruncateLangRange(lcrange);
        }
      }
      return defaultValue;
    }

    /**
     * Returns true if (1) the given string is a well-formed language tag under RFC
     * 5646 (that is, the string follows the syntax given in section 2.1 of
     * that RFC), and (2) the language tag contains at most one extended
     * language subtag, no variant subtags with the same value, and no
     * extension singleton subtags with the same value.
     * @param str The string to check.
     * @return {@code true} , if the string meets the conditions given in the
     * summary, {@code false} otherwise.
     */
    public static boolean IsPotentiallyValidLanguageTag(String str) {
      if (((str) == null || (str).length() == 0)) {
        return false;
      }
      int index = 0;
      int endIndex = str.length();
      int startIndex = index;
      if (index + 1 < endIndex) {
        char c1 = str.charAt(index);
        char c2 = str.charAt(index + 1);
        if (((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z')) && ((c2
          >= 'A' && c2 <= 'Z') || (c2 >= 'a' && c2 <= 'z'))) {
          index += 2;
          if (index == endIndex) {
            // case AA: a 2-letter language
            return true;
          }
          index += 2;
          // convert the language tag to lower case
          // to simplify handling
          str = DataUtilities.ToLowerCaseAscii(str);
          c1 = str.charAt(index);
          // Straightforward cases
          if (c1 >= 'a' && c1 <= 'z') {
            ++index;
            // case AAA: a 3-character language
            if (index == endIndex) {
              return true;
            }
            c1 = str.charAt(index);  // get the next character
          }
          if (c1 == '-') {  // case AA- or AAA-
            ++index;
            if (index + 2 == endIndex) {  // case AA-?? or AAA-??
              c1 = str.charAt(index);
              c2 = str.charAt(index);
              if ((c1 >= 'a' && c1 <= 'z') && (c2 >= 'a' && c2 <= 'z')) {
                // case AA-BB or AAA-BB: BB is a 2-letter region
                return true;
              }
            }
          }
          // match grandfathered language tags (the last
          // is necessary because it would otherwise be rejected
          // by the code that checks extended language subtags)
          if (str.equals("sgn-be-fr") || str.equals("sgn-be-nl") ||
            str.equals("sgn-ch-de") || str.equals("en-gb-oed") ||
              str.equals("zh-min-nan")) {
            return true;
          }
          // More complex cases
          String[] splitString = SplitAt(
  str.substring(startIndex, (startIndex)+(endIndex - startIndex)),
  "-");
          if (splitString.length == 0) {
            return false;
          }
          int splitIndex = 0;
          int splitLength = splitString.length;
          int len = LengthIfAllAlpha(splitString[splitIndex]);
          if (len < 2 || len > 8) {
            return false;
          }
          if (len == 2 || len == 3) {
            ++splitIndex;
            // skip optional extended language subtags
            for (int i = 0; i < 3; ++i) {
              if (splitIndex < splitLength &&
                LengthIfAllAlpha(splitString[splitIndex]) == 3) {
                if (i >= 1) {
                  // point 4 in section 2.2.2 renders two or
                  // more extended language subtags invalid
                  return false;
                }
                ++splitIndex;
              } else {
                break;
              }
            }
          }
          // optional script
          if (splitIndex < splitLength &&
            LengthIfAllAlpha(splitString[splitIndex]) == 4) {
            ++splitIndex;
          }
          // optional region
          if (splitIndex < splitLength &&
            LengthIfAllAlpha(splitString[splitIndex]) == 2) {
            ++splitIndex;
          } else if (splitIndex < splitLength &&
            LengthIfAllDigit(splitString[splitIndex]) == 3) {
            ++splitIndex;
          }
          // variant, any number
          List<String> variants = null;
          while (splitIndex < splitLength) {
            String curString = splitString[splitIndex];
            len = LengthIfAllAlphaNum(curString);
            if (len >= 5 && len <= 8) {
              variants = (variants == null) ? ((new ArrayList<String>())) : variants;
              if (!variants.contains(curString)) {
                variants.add(curString);
              } else {
                // variant already exists; see point 5 in section
                // 2.2.5
                return false;
              }
              ++splitIndex;
            } else if (len == 4 &&
                 (curString.charAt(0) >= '0' && curString.charAt(0) <= '9')) {
              variants = (variants == null) ? ((new ArrayList<String>())) : variants;
              if (!variants.contains(curString)) {
                variants.add(curString);
              } else {
                // variant already exists; see point 5 in section
                // 2.2.5
                return false;
              }
              ++splitIndex;
            } else {
              break;
            }
          }
          // extension, any number
          if (variants != null) {
            variants.clear();
          }
          while (splitIndex < splitLength) {
            String curString = splitString[splitIndex];
            int curIndex = splitIndex;
            if (LengthIfAllAlphaNum(curString) == 1 &&
                    !curString.equals("x")) {
              variants = (variants == null) ? ((new ArrayList<String>())) : variants;
              if (!variants.contains(curString)) {
                variants.add(curString);
              } else {
                // extension already exists (see point
                // 3 of sec. 2.2.6)
                return false;
              }
              ++splitIndex;
              boolean havetoken = false;
              while (splitIndex < splitLength) {
                curString = splitString[splitIndex];
                len = LengthIfAllAlphaNum(curString);
                if (len >= 2 && len <= 8) {
                  havetoken = true;
                  ++splitIndex;
                } else {
                  break;
                }
              }
              if (!havetoken) {
                splitIndex = curIndex;
                break;
              }
            } else {
              break;
            }
          }
          // optional private use
          if (splitIndex < splitLength) {
            int curIndex = splitIndex;
            if (splitString[splitIndex].equals("x")) {
              ++splitIndex;
              boolean havetoken = false;
              while (splitIndex < splitLength) {
                len = LengthIfAllAlphaNum(splitString[splitIndex]);
                if (len >= 1 && len <= 8) {
                  havetoken = true;
                  ++splitIndex;
                } else {
                  break;
                }
              }
              if (!havetoken) {
                splitIndex = curIndex;
              }
            }
          }
          // check if all the tokens were used
          return splitIndex == splitLength;
        }
        if (c2 == '-' && (c1 == 'x' || c1 == 'X')) {
          // private use
          ++index;
          while (index < endIndex) {
            int count = 0;
            if (str.charAt(index) != '-') {
              return false;
            }
            ++index;
            while (index < endIndex) {
              c1 = str.charAt(index);
              if ((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') ||
                (c1 >= '0' && c1 <= '9')) {
                ++count;
                if (count > 8) {
                  return false;
                }
              } else if (c1 == '-') {
                break;
              } else {
                return false;
              }
              ++index;
            }
            if (count < 1) {
              return false;
            }
          }
          return true;
        }
        if (c2 == '-' && (c1 == 'i' || c1 == 'I')) {
          // grandfathered language tags
          str = DataUtilities.ToLowerCaseAscii(str);
          return str.equals("i-ami") || str.equals("i-bnn") ||
          str.equals("i-default") || str.equals("i-enochian") ||
          str.equals("i-hak") || str.equals("i-klingon") ||
          str.equals("i-lux") || str.equals("i-navajo") ||
          str.equals("i-mingo") || str.equals("i-pwn") ||
          str.equals("i-tao") || str.equals("i-tay") ||
          str.equals("i-tsu");
        }
        return false;
      }
      return false;
    }

    private static int LengthIfAllAlpha(String str) {
      int len = (str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z'))) {
          return 0;
        }
      }
      return len;
    }

    private static int LengthIfAllAlphaNum(String str) {
      int len = (str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') || (c1
          >= '0' && c1 <= '9'))) {
          return 0;
        }
      }
      return len;
    }

    private static int LengthIfAllDigit(String str) {
      int len = (str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!(c1 >= '0' && c1 <= '9')) {
          return 0;
        }
      }
      return len;
    }
  }
