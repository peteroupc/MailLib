## PeterO.Mail.LanguageTags

    public static class LanguageTags

Contains methods for parsing and matching language tags.

### GetLanguageList

    public static System.Collections.Generic.IList GetLanguageList(
        string str);

Parses a language list from a Content-Language header field.

<b>Parameters:</b>

 * <i>str</i>: A string following the syntax of a Content-Language header field (see RFC 3282). This is a comma-separated list of language tags. RFC 5322 comments (in parentheses) can appear. This parameter can be null.

<b>Return Value:</b>

A list of language tags. Returns an empty list if "str" is null or the empty string, or null if "str" syntactically invalid.

### GetRangeListWithQuality

    public static System.Collections.Generic.IList GetRangeListWithQuality(
        string str);

Parses a language range list from an Accept-Language header field.

<b>Parameters:</b>

 * <i>str</i>: A string following the syntax of an Accept-Language header field (see RFC 3282). This is a comma-separated list of language ranges, with an optional "quality" after the language tag (examples include "en; q=0.5" or "de-DE"). RFC 5322 comments (in parentheses) can appear. This parameter can be null.

<b>Return Value:</b>

A list of language ranges with their associated qualities. The list will be sorted in descending order by quality; if two or more language ranges have the same quality, they will be sorted in the order in which they appeared in the given string. Returns null if "str" is null or syntactically invalid.

### IsLanguageRange

    public static bool IsLanguageRange(
        string str);

Returns whether the given string is a basic language range under RFC 4647. Examples include "*", "en-us", and "fr".

<b>Parameters:</b>

 * <i>str</i>: The string to check. Can be null.

<b>Return Value:</b>

 `true`  if the given string is a basic language range; otherwise,  `false` .

### IsLanguageRange

    public static bool IsLanguageRange(
        string str,
        bool extended);

Returns whether the given string is a basic or extended language range under RFC 4647. Examples of basic (and extended) language ranges include "*", "en-us", and "fr". Examples of extended language ranges include "*-de" and "it-*".

<b>Parameters:</b>

 * <i>str</i>: The string to check. Can be null.

 * <i>extended</i>: Check whether the string is a basic language range if "false", or an extended language range if "true".

<b>Return Value:</b>

 `true`  if the given string is a basic language range (depending on the "extended" parameter); otherwise,  `false` .

### IsPotentiallyValidLanguageTag

    public static bool IsPotentiallyValidLanguageTag(
        string str);

Returns true if (1) the given string is a well-formed language tag under RFC 5646 (that is, the string follows the syntax given in section 2.1 of that RFC), and (2) the language tag contains at most one extended language subtag, no variant subtags with the same value, and no extension singleton subtags with the same value.

<b>Parameters:</b>

 * <i>str</i>: The string to check.

<b>Return Value:</b>

 `true` , if the string meets the conditions given in the summary,  `false`  otherwise.

### LanguageTagCase

    public static string LanguageTagCase(
        string str);

Sets the given language tag to the case combination recommended by RFC 5646. For example, "en-us" becomes "en-US", and "zh-hant" becomes "zh-Hant".

<b>Parameters:</b>

 * <i>str</i>: A string of a language tag. Can be null.

<b>Return Value:</b>

A text string in the recommended case combination, or null if "str" is null.

### LanguageTagFilter

    public static System.Collections.Generic.IList LanguageTagFilter(
        System.Collections.Generic.IList ranges,
        System.Collections.Generic.IList languages);

Finds the language tags that match a priority list of basic language ranges.

<b>Parameters:</b>

 * <i>ranges</i>: A list of basic language ranges (see documentation for the "IsLanguageRange" method), which should be given in order of descending preference.

 * <i>languages</i>: A list of language tags, which should be given in order of descending preference.

<b>Return Value:</b>

A list of language tags that match the given range, in descending order of preference.

<b>Exceptions:</b>

 * System.ArgumentNullException:
"languages" or "ranges" is null.

 * System.ArgumentException:
"ranges" contains a value that is not a basic language range, or "languages" contains a value that is not a potentially valid language tag.

### LanguageTagFilter

    public static System.Collections.Generic.IList LanguageTagFilter(
        System.Collections.Generic.IList ranges,
        System.Collections.Generic.IList languages,
        bool extended,
        bool matchStarAtEnd);

Finds the language tags that match a priority list of language ranges.

<b>Parameters:</b>

 * <i>ranges</i>: A list of language ranges (see documentation for the "IsLanguageRange" method), which should be given in order of descending preference.

 * <i>languages</i>: A list of language tags, which should be given in order of descending preference.

 * <i>extended</i>: If true, the ranges in "ranges" are extended language ranges; otherwise, they are basic language ranges.

 * <i>matchStarAtEnd</i>: The parameter  <i>matchStarAtEnd</i>
 is not documented yet.

<b>Return Value:</b>

A list of language tags that match the given range, in descending order of preference.

<b>Exceptions:</b>

 * System.ArgumentNullException:
"languages" or "ranges" is null.

 * System.ArgumentException:
"ranges" contains a value that is not a basic or extended language range, or "languages" contains a value that is not a potentially valid language tag.

### LanguageTagLookup

    public static string LanguageTagLookup(
        string range,
        System.Collections.Generic.IList languages,
        string defaultValue);

Does a language tag lookup (under RFC 4647) for a matching language tag.

<b>Parameters:</b>

 * <i>range</i>: A basic language range (see the documentation for "IsLanguageRange").

 * <i>languages</i>: A list of language tags, which should be given in order of descending preference.

 * <i>defaultValue</i>: The value to return if no matching language tag was found.

<b>Return Value:</b>

The matching language tag, or the parameter "defaultValue" if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
"languages" is null.

 * System.ArgumentException:
"range" is not a basic language range, or "languages" contains a value that is not a potentially valid language tag.

### LanguageTagLookup

    public static string LanguageTagLookup(
        string range,
        System.Collections.Generic.IList languages,
        string defaultValue,
        bool extended);

Does a language tag lookup (under RFC 4647) for a matching language tag.

<b>Parameters:</b>

 * <i>range</i>: A language range (see the documentation for "IsLanguageRange").

 * <i>languages</i>: A list of language tags, which should be given in order of descending preference.

 * <i>defaultValue</i>: The value to return if no matching language tag was found.

 * <i>extended</i>: If true, "range" is an extended language range; otherwise, it's a are basic language range.

<b>Return Value:</b>

The matching language tag, or the parameter "defaultValue" if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
"languages" is null.

 * System.ArgumentException:
"range" is not a basic or extended language range, or "languages" contains a value that is not a potentially valid language tag.

### LanguageTagLookup

    public static string LanguageTagLookup(
        System.Collections.Generic.IList ranges,
        System.Collections.Generic.IList languages,
        string defaultValue);

Does a language tag lookup (under RFC 4647) for a matching language tag.

<b>Parameters:</b>

 * <i>ranges</i>: A list of basic language ranges (see documentation for the "IsLanguageRange" method), which should be given in order of descending preference.

 * <i>languages</i>: A list of language tags, which should be given in order of descending preference.

 * <i>defaultValue</i>: The value to return if no matching language tag was found.

<b>Return Value:</b>

The matching language tag, or the parameter "defaultValue" if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
"languages" or "ranges" is null.

 * System.ArgumentException:
"ranges" contains a value that is not a basic language range, or "languages" contains a value that is not a potentially valid language tag.

### LanguageTagLookup

    public static string LanguageTagLookup(
        System.Collections.Generic.IList ranges,
        System.Collections.Generic.IList languages,
        string defaultValue,
        bool extended);

Does a language tag lookup (under RFC 4647) for a matching language tag.

<b>Parameters:</b>

 * <i>ranges</i>: A list of language ranges (see documentation for the "IsLanguageRange" method), which should be given in order of descending preference.

 * <i>languages</i>: A list of language tags, which should be given in order of descending preference.

 * <i>defaultValue</i>: The value to return if no matching language tag was found.

 * <i>extended</i>: If true, the ranges in "ranges" are extended language ranges; otherwise, they are basic language ranges.

<b>Return Value:</b>

The matching language tag, or the parameter "defaultValue" if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
"languages" or "ranges" is null.

 * System.ArgumentException:
"ranges" contains a value that is not a basic or extended language range, or "languages" contains a value that is not a potentially valid language tag.

### MatchesLanguageTag

    public static bool MatchesLanguageTag(
        string range,
        string tag);

Determines whether the given language tag matches the given language range.

<b>Parameters:</b>

 * <i>range</i>: A basic language range (see the documentation for "IsLanguageRange").

 * <i>tag</i>: A language tag.

<b>Return Value:</b>

 `true`  if the language tag matches the language range by the filtering method under RFC 4647; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentException:
"range" is not a basic language range, or "tag" is not a potentially valid language tag.
