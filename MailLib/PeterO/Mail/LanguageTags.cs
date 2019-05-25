using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.LanguageTags"]/*'/>
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
          strings = strings ?? (new List<string>());
          strings.Add(str.Substring(index));
          break;
        } else {
          first = false;
          string newstr = str.Substring(index, index2 - index);
          strings = strings ?? (new List<string>());
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return (string[])strings.ToArray();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.IsLanguageRange(System.String)"]/*'/>
    public static bool IsLanguageRange(string str) {
      return IsLanguageRange(str, false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.IsLanguageRange(System.String,System.Boolean)"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.LanguageTagCase(System.String)"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.GetLanguageList(System.String)"]/*'/>
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
          if (index < str.Length && str[index] == ',') { index++;
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.GetRangeListWithQuality(System.String)"]/*'/>
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
          if (index < str.Length && str[index] == ',') { index++;
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
          if (index < str.Length && (str[index] >= '0' || str[index] <= '9'
)) {
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
      if (rangeLowerCased.Equals("*")) {
 return true;
}
      if (rangeLowerCased.Equals(tagLowerCased)) {
        return true;
      }
      if (tagLowerCased.Length > rangeLowerCased.Length &&
          tagLowerCased[rangeLowerCased.Length] == '-') {
        string prefix = tagLowerCased.Substring(0, rangeLowerCased.Length);
        if (rangeLowerCased.Equals(prefix)) {
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
      if (!rangeSub[0].Equals("*") && !rangeSub[0].Equals(tagSub[0])) {
        return false;
      }
      var rangeIndex = 1;
      var tagIndex = 1;
      while (rangeIndex < rangeSub.Length) {
     string range = rangeSub[rangeIndex];
        if (range.Length == 0) {
          return false;
        }
        if (range.Equals("*")) {
          continue;
        }
        if (tagIndex >= tagSub.Length) {
          return false;
        }
string tag = tagSub[tagIndex];
        if (range.Equals(tag)) {
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.LanguageTagFilter(System.Collections.Generic.IList{System.String},System.Collections.Generic.IList{System.String},System.Boolean,System.Boolean)"]/*'/>
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
 throw new ArgumentException("ranges");
}
      }
      foreach (string lang in languages) {
        if (!IsPotentiallyValidLanguageTag(lang)) {
 throw new ArgumentException("languages");
}
      }
      foreach (string range in ranges) {
        if (matchStarAtEnd && range.Equals("*")) {
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
 if (range[i] == '-' && i >= 2 && range[i - 1] != '-' && range[i - 2] != '-'
) {
 return range.Substring(0, i);
}
      }
      return String.Empty;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.MatchesLanguageTag(System.String,System.String)"]/*'/>
    public static bool MatchesLanguageTag(string range, string tag) {
      IList<string> tags = LanguageTagFilter(
        new List<string>(new string[] { range }),
        new List<string>(new string[] { tag }),
        false,
 false);
      return tags.Count > 0;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.LanguageTagLookup(System.String,System.Collections.Generic.IList{System.String},System.String)"]/*'/>
    public static string LanguageTagLookup(
  string range,
  IList<string> languages,
  string defaultValue) {
      return LanguageTagLookup(range, languages, defaultValue, false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.LanguageTagLookup(System.Collections.Generic.IList{System.String},System.Collections.Generic.IList{System.String},System.String)"]/*'/>
    public static string LanguageTagLookup(
  IList<string> ranges,
  IList<string> languages,
  string defaultValue) {
      return LanguageTagLookup(ranges, languages, defaultValue, false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.LanguageTagFilter(System.Collections.Generic.IList{System.String},System.Collections.Generic.IList{System.String})"]/*'/>
    public static IList<string> LanguageTagFilter(
  IList<string> ranges,
  IList<string> languages) {
      return LanguageTagFilter(ranges, languages, false, false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.LanguageTagLookup(System.String,System.Collections.Generic.IList{System.String},System.String,System.Boolean)"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.LanguageTagLookup(System.Collections.Generic.IList{System.String},System.Collections.Generic.IList{System.String},System.String,System.Boolean)"]/*'/>
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
 throw new ArgumentException("ranges");
}
      }
      foreach (string lang in languages) {
        if (!IsPotentiallyValidLanguageTag(lang)) {
 throw new ArgumentException("languages");
}
      }
      foreach (string range in ranges) {
        if (range.Equals("*")) {
 continue;
}
        string lcrange = DataUtilities.ToLowerCaseAscii(range);
        while (lcrange.Length > 0) {
          foreach (string lang in languages) {
            string lclang = DataUtilities.ToLowerCaseAscii(lang);
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.IsPotentiallyValidLanguageTag(System.String)"]/*'/>
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
            c1 = str[index];  // get the next character
          }
          if (c1 == '-') {  // case AA- or AAA-
            ++index;
            if (index + 2 == endIndex) {  // case AA-?? or AAA-??
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
          if (str.Equals("sgn-be-fr") ||
str.Equals("sgn-be-nl") ||
str.Equals("sgn-ch-de") ||
str.Equals("en-gb-oed") ||
str.Equals("zh-min-nan")) {
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
              variants = variants ?? (new List<string>());
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
              variants = variants ?? (new List<string>());
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
                    !curString.Equals("x")) {
              variants = variants ?? (new List<string>());
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
            if (splitString[splitIndex].Equals("x")) {
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
          return str.Equals("i-ami") || str.Equals("i-bnn") ||
          str.Equals("i-default") || str.Equals("i-enochian") ||
          str.Equals("i-hak") || str.Equals("i-klingon") ||
          str.Equals("i-lux") || str.Equals("i-navajo") ||
          str.Equals("i-mingo") || str.Equals("i-pwn") ||
          str.Equals("i-tao") || str.Equals("i-tay") ||
          str.Equals("i-tsu");
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
