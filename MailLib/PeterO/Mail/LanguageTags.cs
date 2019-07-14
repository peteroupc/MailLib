using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace PeterO.Mail {
    /// <summary>Contains methods for parsing and matching language
    /// tags.</summary>
  public static class LanguageTags {
    private static string[] SplitAt(string str, string delimiter) {
      if (delimiter == null) {
        throw new ArgumentNullException(nameof(delimiter));
      }
      if (delimiter.Length == 0) {
        throw new ArgumentException("delimiter is empty.");
      }
      if (String.IsNullOrEmpty(str)) {
        return new[] { String.Empty };
      }
      var index = 0;
      var first = true;
      List<string> strings = null;
      int delimLength = delimiter.Length;
      while (true) {
        int index2 = str.IndexOf(delimiter, index, StringComparison.Ordinal);
        if (index2 < 0) {
          if (first) {
            var strret = new string[1];
            strret[0] = str;
            return strret;
          }
          strings = strings ?? new List<string>();
          strings.Add(str.Substring(index));
          break;
        } else {
          first = false;
          string newstr = str.Substring(index, index2 - index);
          strings = strings ?? new List<string>();
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return (string[])strings.ToArray();
    }

    /// <summary>Returns whether the given string is a basic language range
    /// under RFC 4647. Examples include "&#x2a;", "en-us", and
    /// "fr".</summary>
    /// <param name='str'>The string to check. Can be null.</param>
    /// <returns><c>true</c> if the given string is a basic language range;
    /// otherwise, <c>false</c>.</returns>
    public static bool IsLanguageRange(string str) {
      return IsLanguageRange(str, false);
    }

    /// <summary>Returns whether the given string is a basic or extended
    /// language range under RFC 4647. Examples of basic (and extended)
    /// language ranges include "&#x2a;", "en-us", and "fr". Examples of
    /// extended language ranges include "&#x2a;-de" and
    /// "it-&#x2a;".</summary>
    /// <param name='str'>The string to check. Can be null.</param>
    /// <param name='extended'>Check whether the string is a basic language
    /// range if "false", or an extended language range if "true".</param>
    /// <returns><c>true</c> if the given string is a basic language range
    /// (depending on the <paramref name='extended'/> parameter);
    /// otherwise, <c>false</c>.</returns>
    public static bool IsLanguageRange(string str, bool extended) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      if (str.Length == 1 && str[0] == '*') {
        return true;
      }
      var count = 0;
      var i = 0;
      var first = true;
      while (i < str.Length) {
        char c = str[i];
        if (i > 0 && c == '-' && i + 1 < str.Length && str[i + 1] != '-') {
          count = 0;
          ++i;
          first = false;
          continue;
        }
        if (extended && c == '*' && count == 0 && (i + 1 == str.Length ||
          str[i + 1] == '-')) {
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

    private static int SkipFWS(string str, int index, int endIndex) {
      // NOTE: Includes obsolete syntax under RFC 5322 (with errata)
      while (index < endIndex) {
        if (endIndex - index > 1 && str[index] == 0x0d && str[index + 1] ==
              0x0a) {
          index += 2;
        }
        if (str[index] == 0x09 || str[index] == 0x20) {
          ++index;
        } else {
          break;
        }
      }
      return index;
    }

    private static int SkipCFWS(string str, int index, int endIndex) {
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
      string str,
      int index,
      int endIndex) {
      int indexStart = index;
      var depth = 0;
      if (index < endIndex && (str[index] == 40)) {
        ++index;
      } else {
        return index;
      }
      while (index < endIndex) {
        index = SkipFWS(str, index, endIndex);
        bool backslash = index < endIndex && str[index] == '\\';
        if (backslash) {
          ++index;
        }
        if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <=
          56319) && (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
          index += 2;
        } else if (!backslash && index < endIndex && ((str[index] >= 1 &&
          str[index] <= 8) || (str[index] >= 11 && str[index] <= 12) ||
          (str[index] >= 14 && str[index] <= 31) || (str[index] >= 33 &&
          str[index] <= 39) || (str[index] >= 42 && str[index] <= 91) ||
          (str[index] >= 93 && str[index] <= 55295) || (str[index] >= 57344 &&
          str[index] <= 65535))) {
          ++index;
        } else if (backslash && index < endIndex && ((str[index] >= 0 &&
          str[index] <= 55295) || (str[index] >= 57344 && str[index] <=
          65535))) {
          // NOTE: Includes parentheses, which are also handled
          // in later conditions
          ++index;
        } else if (index < endIndex && str[index] == 41) {
          // End of current comment
          ++index;
          if (depth == 0) {
            return index;
          }
          --depth;
        } else if (index < endIndex && str[index] == 40) {
          // Start of nested comment
          ++index;
          ++depth;
        } else {
          return indexStart;
        }
      }
      return indexStart;
    }

    /// <summary>Sets the given language tag to the case combination
    /// recommended by RFC 5646. For example, "en-us" becomes "en-US", and
    /// "zh-hant" becomes "zh-Hant".</summary>
    /// <param name='str'>A string of a language tag. Can be null.</param>
    /// <returns>A text string in the recommended case combination, or null
    /// if <paramref name='str'/> is null.</returns>
    public static string LanguageTagCase(string str) {
      if (String.IsNullOrEmpty(str)) {
        return str;
      }
      if (str.IndexOf('-') == -1) {
        return DataUtilities.ToLowerCaseAscii(str);
      }
      var count = 0;
      var i = 0;
      var lastSubtagLength = -1;
      var sb = new StringBuilder();
      while (i <= str.Length) {
        if (i == str.Length || str[i] == '-') {
          if (count == 4 && lastSubtagLength >= 2) {
            sb.Append(DataUtilities.ToUpperCaseAscii(str.Substring(i - 4, 1)));
            sb.Append(DataUtilities.ToLowerCaseAscii(str.Substring(i - 3, 3)));
          } else if (count == 2 && lastSubtagLength >= 2) {
            sb.Append(DataUtilities.ToUpperCaseAscii(str.Substring(i - 2, 2)));
          } else {
            sb.Append(
          DataUtilities.ToLowerCaseAscii(
          str.Substring(
            i - count,
            count)));
          }
          lastSubtagLength = count;
          count = 0;
          ++i;
          if (i < str.Length) {
            sb.Append('-');
          }
          continue;
        }
        ++count;
        ++i;
      }
      return sb.ToString();
    }

    private static int SkipLDHSequence(string str, int index, int endIndex) {
      while (index < endIndex) {
        if ((str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 &&
          str[index] <= 122) || (str[index] >= 48 && str[index] <= 57) ||
          (str[index] == 45)) {
          ++index;
        } else {
          break;
        }
      }
      return index;
    }

    /// <summary>Parses a language list from a Content-Language header
    /// field.</summary>
    /// <param name='str'>A string following the syntax of a
    /// Content-Language header field (see RFC 3282). This is a
    /// comma-separated list of language tags. RFC 5322 comments (in
    /// parentheses) can appear. This parameter can be null.</param>
    /// <returns>A list of language tags. Returns an empty list if
    /// <paramref name='str'/> is null or the empty string, or null if
    /// <paramref name='str'/> syntactically invalid.</returns>
    public static IList<string> GetLanguageList(
      string str) {
      string tag = null;
      var first = true;
      var index = 0;
      var ret = new List<string>();
      if (String.IsNullOrEmpty(str)) {
        return ret;
      }
      while (index < str.Length) {
        if (!first) {
          if (index < str.Length && str[index] == ',') {
            ++index;
          } else {
            return null;
          }
        }
        first = false;
        index = SkipCFWS(str, index, str.Length);
        int newindex = SkipLDHSequence(str, index, str.Length);
        if (newindex == index) {
          return null;
        }
        tag = str.Substring(index, newindex - index);
        if (!IsPotentiallyValidLanguageTag(tag)) {
          return null;
        }
        ret.Add(tag);
        index = SkipCFWS(str, newindex, str.Length);
      }
      return ret;
    }

    private static void SortQualityList(IList<StringAndQuality> list) {
      // Do an insertion sort by descending quality.
      // Insertion sort is used even though it has
      // quadratic complexity, because it's stable
      // (it preserves the order of equal items) and
      // because the size of language
      // priority lists is expected to be small.
      for (var i = 1; i < list.Count; ++i) {
        StringAndQuality saq = list[i];
        var k = -1;
        for (k = i - 1; k >= 0; --k) {
          bool cmp = list[k].Quality < saq.Quality;
          if (cmp) {
            list[k + 1] = list[k];
          } else {
            break;
          }
        }
        list[k] = saq;
      }
    }

    /// <summary>Parses a language range list from an Accept-Language
    /// header field.</summary>
    /// <param name='str'>A string following the syntax of an
    /// Accept-Language header field (see RFC 3282). This is a
    /// comma-separated list of language ranges, with an optional "quality"
    /// after the language tag (examples include "en; q=0.5" or "de-DE").
    /// RFC 5322 comments (in parentheses) can appear. This parameter can
    /// be null.</param>
    /// <returns>A list of language ranges with their associated qualities.
    /// The list will be sorted in descending order by quality; if two or
    /// more language ranges have the same quality, they will be sorted in
    /// the order in which they appeared in the given string. Returns null
    /// if <paramref name='str'/> is null or syntactically
    /// invalid.</returns>
    public static IList<StringAndQuality> GetRangeListWithQuality(
      string str) {
      string tag = null;
      var first = true;
      var index = 0;
      var ret = new List<StringAndQuality>();
      if (str == null) {
        return null;
      }
      while (index < str.Length) {
        if (!first) {
          if (index < str.Length && str[index] == ',') {
            ++index;
          } else {
            return null;
          }
        }
        first = false;
        index = SkipCFWS(str, index, str.Length);
        int newindex;
        newindex = (index < str.Length && str[index] == '*') ? (index + 1) :
          SkipLDHSequence(str, index, str.Length);
        if (newindex == index) {
          return null;
        }
        tag = str.Substring(index, newindex - index);
        if (!IsLanguageRange(tag)) {
          return null;
        }
        index = SkipCFWS(str, newindex, str.Length);
        if (index < str.Length && str[index] == ';') {
          index = SkipCFWS(str, newindex, str.Length);
          if (index < str.Length && str[index] == 'q') {
            ++index;
          } else {
            return null;
          }
          index = SkipCFWS(str, newindex, str.Length);
          if (index < str.Length && str[index] == '=') {
            ++index;
          } else {
            return null;
          }
          if (index < str.Length && (str[index] == '0' || str[index] == '1')) {
            int qvalue = (str[index] == '0') ? 0 : 1000;
            ++index;
            if (index < str.Length && str[index] == '.') {
              ++index;
              int[] mults = { 100, 10, 1 };
              for (var i = 0; i < 3; ++i) {
                if (index < str.Length &&
                (str[index] >= '0' || str[index] <= '9')) {
                  qvalue += mults[i] * (str[index] - '0');
                  ++index;
                } else {
                  break;
                }
              }
            }
            if (qvalue < 0 || qvalue > 1000) {
              return null;
            }
            ret.Add(new StringAndQuality(tag, qvalue));
            index = SkipCFWS(str, newindex, str.Length);
          } else {
            return null;
          }
        } else {
          ret.Add(new StringAndQuality(tag, 1000));
        }
      }
      SortQualityList(ret);
      return ret;
    }

    private static bool MatchLangTagBasic(
      string rangeLowerCased,
      string tagLowerCased) {
      if (rangeLowerCased.Equals("*", StringComparison.Ordinal)) {
        return true;
      }
      if (rangeLowerCased.Equals(tagLowerCased, StringComparison.Ordinal)) {
        return true;
      }
      if (tagLowerCased.Length > rangeLowerCased.Length &&
          tagLowerCased[rangeLowerCased.Length] == '-') {
        string prefix = tagLowerCased.Substring(0, rangeLowerCased.Length);
        if (rangeLowerCased.Equals(prefix, StringComparison.Ordinal)) {
          return true;
        }
      }
      return false;
    }

    private static bool MatchLangTagExtended(
      string rangeLowerCased,
      string tagLowerCased) {
      string[] rangeSub = SplitAt(rangeLowerCased, "-");
      string[] tagSub = SplitAt(tagLowerCased, "-");
      if (rangeSub.Length == 0 || tagSub.Length == 0) {
        return false;
      }
      if (!rangeSub[0].Equals("*", StringComparison.Ordinal) &&
!rangeSub[0].Equals(tagSub[0], StringComparison.Ordinal)) {
        return false;
      }
      var rangeIndex = 1;
      var tagIndex = 1;
      while (rangeIndex < rangeSub.Length) {
        string range = rangeSub[rangeIndex];
        if (range.Length == 0) {
          return false;
        }
        if (range.Equals("*", StringComparison.Ordinal)) {
          continue;
        }
        if (tagIndex >= tagSub.Length) {
          return false;
        }
        string tag = tagSub[tagIndex];
        if (range.Equals(tag, StringComparison.Ordinal)) {
          ++rangeIndex;
          ++tagIndex;
        }
        if (tag.Length == 1) {
          return false;
        }
        ++tagIndex;
      }
      return true;
    }

    /// <summary>Finds the language tags that match a priority list of
    /// language ranges.</summary>
    /// <param name='ranges'>A list of language ranges (see documentation
    /// for the "IsLanguageRange" method), which should be given in order
    /// of descending preference.</param>
    /// <param name='languages'>A list of language tags, which should be
    /// given in order of descending preference.</param>
    /// <param name='extended'>If true, the ranges in "ranges" are extended
    /// language ranges; otherwise, they are basic language ranges.</param>
    /// <param name='matchStarAtEnd'>If true, treats any range equaling
    /// "&#x2a;" as appearing at the end of the language priority list, no
    /// matter where it appears on that list.</param>
    /// <returns>A list of language tags that match the given range, in
    /// descending order of preference.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> or <paramref name='ranges'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ranges'/> contains a value that is not a basic or extended
    /// language range, or <paramref name='languages'/> contains a value
    /// that is not a potentially valid language tag.</exception>
    public static IList<string> LanguageTagFilter(
           IList<string> ranges,
           IList<string> languages,
           bool extended,
           bool matchStarAtEnd) {
      if (ranges == null) {
        throw new ArgumentNullException(nameof(ranges));
      }
      if (languages == null) {
        throw new ArgumentNullException(nameof(languages));
      }
      var retlist = new List<string>();
      if (ranges.Count == 0) {
        return retlist;
      }
      if (languages.Count == 0) {
        return retlist;
      }
      var hasStar = false;
      var langsMatch = new bool[languages.Count];
      foreach (string range in ranges) {
        if (!IsLanguageRange(range, extended)) {
          throw new ArgumentException("ranges is not a language range.");
        }
      }
      foreach (string lang in languages) {
        if (!IsPotentiallyValidLanguageTag(lang)) {
          throw new ArgumentException("languages is not a language tag");
        }
      }
      foreach (string range in ranges) {
        if (matchStarAtEnd && range.Equals("*", StringComparison.Ordinal)) {
          hasStar = true;
          continue;
        }
        string lcrange = DataUtilities.ToLowerCaseAscii(range);
        for (var k = 0; k < languages.Count; ++k) {
          if (langsMatch[k]) {
            continue;
          }
          string lclang = DataUtilities.ToLowerCaseAscii(languages[k]);
          if (extended) {
            if (MatchLangTagExtended(lcrange, lclang)) {
              retlist.Add(languages[k]);
              langsMatch[k] = true;
            }
          } else {
            if (MatchLangTagBasic(lcrange, lclang)) {
              retlist.Add(languages[k]);
              langsMatch[k] = true;
            }
          }
        }
      }
      if (matchStarAtEnd && hasStar) {
        for (var k = 0; k < languages.Count; ++k) {
          if (!langsMatch[k]) {
            retlist.Add(languages[k]);
          }
        }
      }
      return retlist;
    }

    private static string TruncateLangRange(string range) {
      var i = 0;
      for (i = range.Length - 1; i >= 0; --i) {
        if (range[i] == '-' && i >= 2 &&
            range[i - 1] != '-' && range[i - 2] != '-') {
          return range.Substring(0, i);
        }
      }
      return String.Empty;
    }

    /// <summary>Determines whether the given language tag matches the
    /// given language range.</summary>
    /// <param name='range'>A basic language range (see the documentation
    /// for "IsLanguageRange").</param>
    /// <param name='tag'>A language tag.</param>
    /// <returns><c>true</c> if the language tag matches the language range
    /// by the filtering method under RFC 4647; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='range'/> is not a basic language range, or <paramref
    /// name='tag'/> is not a potentially valid language tag.</exception>
    public static bool MatchesLanguageTag(string range, string tag) {
      IList<string> tags = LanguageTagFilter(
        new List<string>(new string[] { range }),
        new List<string>(new string[] { tag }),
        false,
        false);
      return tags.Count > 0;
    }

    /// <summary>Does a language tag lookup (under RFC 4647) for a matching
    /// language tag.</summary>
    /// <param name='range'>A basic language range (see the documentation
    /// for "IsLanguageRange").</param>
    /// <param name='languages'>A list of language tags, which should be
    /// given in order of descending preference.</param>
    /// <param name='defaultValue'>The value to return if no matching
    /// language tag was found.</param>
    /// <returns>The matching language tag, or the parameter <paramref
    /// name='defaultValue'/> if there is no matching language
    /// tag.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='range'/> is not a basic language range, or <paramref
    /// name='languages'/> contains a value that is not a potentially valid
    /// language tag.</exception>
    public static string LanguageTagLookup(
  string range,
  IList<string> languages,
  string defaultValue) {
      return LanguageTagLookup(range, languages, defaultValue, false);
    }

    /// <summary>Does a language tag lookup (under RFC 4647) for a matching
    /// language tag.</summary>
    /// <param name='ranges'>A list of basic language ranges (see
    /// documentation for the "IsLanguageRange" method), which should be
    /// given in order of descending preference.</param>
    /// <param name='languages'>A list of language tags, which should be
    /// given in order of descending preference.</param>
    /// <param name='defaultValue'>The value to return if no matching
    /// language tag was found.</param>
    /// <returns>The matching language tag, or the parameter <paramref
    /// name='defaultValue'/> if there is no matching language
    /// tag.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> or <paramref name='ranges'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ranges'/> contains a value that is not a basic language
    /// range, or <paramref name='languages'/> contains a value that is not
    /// a potentially valid language tag.</exception>
    public static string LanguageTagLookup(
  IList<string> ranges,
  IList<string> languages,
  string defaultValue) {
      return LanguageTagLookup(ranges, languages, defaultValue, false);
    }

    /// <summary>Finds the language tags that match a priority list of
    /// basic language ranges.</summary>
    /// <param name='ranges'>A list of basic language ranges (see
    /// documentation for the "IsLanguageRange" method), which should be
    /// given in order of descending preference.</param>
    /// <param name='languages'>A list of language tags, which should be
    /// given in order of descending preference.</param>
    /// <returns>A list of language tags that match the given range, in
    /// descending order of preference.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> or <paramref name='ranges'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ranges'/> contains a value that is not a basic language
    /// range, or <paramref name='languages'/> contains a value that is not
    /// a potentially valid language tag.</exception>
    public static IList<string> LanguageTagFilter(
  IList<string> ranges,
  IList<string> languages) {
      return LanguageTagFilter(ranges, languages, false, false);
    }

    /// <summary>Does a language tag lookup (under RFC 4647) for a matching
    /// language tag.</summary>
    /// <param name='range'>A language range (see the documentation for
    /// "IsLanguageRange").</param>
    /// <param name='languages'>A list of language tags, which should be
    /// given in order of descending preference.</param>
    /// <param name='defaultValue'>The value to return if no matching
    /// language tag was found.</param>
    /// <param name='extended'>If true, "range" is an extended language
    /// range; otherwise, it's a are basic language range.</param>
    /// <returns>The matching language tag, or the parameter <paramref
    /// name='defaultValue'/> if there is no matching language
    /// tag.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> is null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='range'/> is not a basic or extended language range, or
    /// <paramref name='languages'/> contains a value that is not a
    /// potentially valid language tag.</exception>
    public static string LanguageTagLookup(
  string range,
  IList<string> languages,
  string defaultValue,
  bool extended) {
      return LanguageTagLookup(
        new List<string>(new string[] { range }),
        languages,
        defaultValue,
        extended);
    }

    /// <summary>Does a language tag lookup (under RFC 4647) for a matching
    /// language tag.</summary>
    /// <param name='ranges'>A list of language ranges (see documentation
    /// for the "IsLanguageRange" method), which should be given in order
    /// of descending preference.</param>
    /// <param name='languages'>A list of language tags, which should be
    /// given in order of descending preference.</param>
    /// <param name='defaultValue'>The value to return if no matching
    /// language tag was found.</param>
    /// <param name='extended'>If true, the ranges in "ranges" are extended
    /// language ranges; otherwise, they are basic language ranges.</param>
    /// <returns>The matching language tag, or the parameter <paramref
    /// name='defaultValue'/> if there is no matching language
    /// tag.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='languages'/> or <paramref name='ranges'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='ranges'/> contains a value that is not a basic or extended
    /// language range, or <paramref name='languages'/> contains a value
    /// that is not a potentially valid language tag.</exception>
    public static string LanguageTagLookup(
         IList<string> ranges,
         IList<string> languages,
         string defaultValue,
         bool extended) {
      if (ranges == null) {
        throw new ArgumentNullException(nameof(ranges));
      }
      if (languages == null) {
        throw new ArgumentNullException(nameof(languages));
      }
      if (ranges.Count == 0) {
        return defaultValue;
      }
      if (languages.Count == 0) {
        return defaultValue;
      }
      foreach (string range in ranges) {
        if (!IsLanguageRange(range, extended)) {
          throw new ArgumentException("ranges is not a lnaguage range");
        }
      }
      foreach (string lang in languages) {
        if (!IsPotentiallyValidLanguageTag(lang)) {
          throw new ArgumentException("languages is not a language tag.");
        }
      }
      foreach (string range in ranges) {
        if (range.Equals("*", StringComparison.Ordinal)) {
          continue;
        }
        string lcrange = DataUtilities.ToLowerCaseAscii(range);
        while (lcrange.Length > 0) {
          foreach (string lang in languages) {
            string lclang = DataUtilities.ToLowerCaseAscii(lang);
            if (extended && MatchLangTagExtended(lcrange, lclang)) {
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

    /// <summary>Returns true if (1) the given string is a well-formed
    /// language tag under RFC 5646 (that is, the string follows the syntax
    /// given in section 2.1 of that RFC), and (2) the language tag
    /// contains at most one extended language subtag, no variant subtags
    /// with the same value, and no extension singleton subtags with the
    /// same value.</summary>
    /// <param name='str'>The string to check.</param>
    /// <returns><c>true</c>, if the string meets the conditions given in
    /// the summary, <c>false</c> otherwise.</returns>
    public static bool IsPotentiallyValidLanguageTag(string str) {
      if (String.IsNullOrEmpty(str)) {
        return false;
      }
      var index = 0;
      int endIndex = str.Length;
      int startIndex = index;
      if (index + 1 < endIndex) {
        char c1 = str[index];
        char c2 = str[index + 1];
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
          c1 = str[index];
          // Straightforward cases
          if (c1 >= 'a' && c1 <= 'z') {
            ++index;
            // case AAA: a 3-character language
            if (index == endIndex) {
              return true;
            }
            c1 = str[index]; // get the next character
          }
          if (c1 == '-') { // case AA- or AAA-
            ++index;
            if (index + 2 == endIndex) { // case AA-?? or AAA-??
              c1 = str[index];
              c2 = str[index];
              if ((c1 >= 'a' && c1 <= 'z') && (c2 >= 'a' && c2 <= 'z')) {
                // case AA-BB or AAA-BB: BB is a 2-letter region
                return true;
              }
            }
          }
          // match grandfathered language tags (the last
          // is necessary because it would otherwise be rejected
          // by the code that checks extended language subtags)
          if (str.Equals("sgn-be-fr", StringComparison.Ordinal) ||
str.Equals("sgn-be-nl", StringComparison.Ordinal) ||
str.Equals("sgn-ch-de", StringComparison.Ordinal) ||
str.Equals("en-gb-oed", StringComparison.Ordinal) ||
str.Equals("zh-min-nan", StringComparison.Ordinal)) {
            return true;
          }
          // More complex cases
          string[] splitString = SplitAt(
  str.Substring(startIndex, endIndex - startIndex),
  "-");
          if (splitString.Length == 0) {
            return false;
          }
          var splitIndex = 0;
          int splitLength = splitString.Length;
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
          IList<string> variants = null;
          while (splitIndex < splitLength) {
            string curString = splitString[splitIndex];
            len = LengthIfAllAlphaNum(curString);
            if (len >= 5 && len <= 8) {
              variants = variants ?? new List<string>();
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                // variant already exists; see point 5 in section
                // 2.2.5
                return false;
              }
              ++splitIndex;
            } else if (len == 4 &&
                 (curString[0] >= '0' && curString[0] <= '9')) {
              variants = variants ?? new List<string>();
              if (!variants.Contains(curString)) {
                variants.Add(curString);
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
            variants.Clear();
          }
          while (splitIndex < splitLength) {
            string curString = splitString[splitIndex];
            int curIndex = splitIndex;
            if (LengthIfAllAlphaNum(curString) == 1 &&
                    !curString.Equals("x", StringComparison.Ordinal)) {
              variants = variants ?? new List<string>();
              if (!variants.Contains(curString)) {
                variants.Add(curString);
              } else {
                // extension already exists (see point
                // 3 of sec. 2.2.6)
                return false;
              }
              ++splitIndex;
              var havetoken = false;
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
            if (splitString[splitIndex].Equals("x", StringComparison.Ordinal)) {
              ++splitIndex;
              var havetoken = false;
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
            var count = 0;
            if (str[index] != '-') {
              return false;
            }
            ++index;
            while (index < endIndex) {
              c1 = str[index];
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
          return str.Equals("i-ami", StringComparison.Ordinal) ||
str.Equals("i-bnn", StringComparison.Ordinal) ||
          str.Equals("i-default", StringComparison.Ordinal) ||
str.Equals("i-enochian", StringComparison.Ordinal) ||
          str.Equals("i-hak", StringComparison.Ordinal) ||
str.Equals("i-klingon", StringComparison.Ordinal) ||
          str.Equals("i-lux", StringComparison.Ordinal) ||
str.Equals("i-navajo", StringComparison.Ordinal) ||
          str.Equals("i-mingo", StringComparison.Ordinal) ||
str.Equals("i-pwn", StringComparison.Ordinal) ||
          str.Equals("i-tao", StringComparison.Ordinal) ||
str.Equals("i-tay", StringComparison.Ordinal) ||
          str.Equals("i-tsu", StringComparison.Ordinal);
        }
        return false;
      }
      return false;
    }

    private static int LengthIfAllAlpha(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z'))) {
          return 0;
        }
      }
      return len;
    }

    private static int LengthIfAllAlphaNum(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!((c1 >= 'A' && c1 <= 'Z') || (c1 >= 'a' && c1 <= 'z') || (c1
          >= '0' && c1 <= '9'))) {
          return 0;
        }
      }
      return len;
    }

    private static int LengthIfAllDigit(string str) {
      int len = (str == null) ? 0 : str.Length;
      for (int i = 0; i < len; ++i) {
        char c1 = str[i];
        if (!(c1 >= '0' && c1 <= '9')) {
          return 0;
        }
      }
      return len;
    }
  }
}
