## PeterO.Mail.LanguageTags

    public static class LanguageTags

Contains methods for parsing and matching language tags.

### Member Summary
* <code>[GetLanguageList(string)](#GetLanguageList_string)</code> - Parses a language list from a Content-Language header field.
* <code>[GetRangeListWithQuality(string)](#GetRangeListWithQuality_string)</code> - Parses a language range list from an Accept-Language header field.
* <code>[IsLanguageRange(string)](#IsLanguageRange_string)</code> - Returns whether the given string is a basic language range under RFC 4647.
* <code>[IsLanguageRange(string, bool)](#IsLanguageRange_string_bool)</code> - Returns whether the given string is a basic or extended language range under RFC 4647.
* <code>[IsPotentiallyValidLanguageTag(string)](#IsPotentiallyValidLanguageTag_string)</code> - Returns true if (1) the given string is a well-formed language tag under RFC 5646 (that is, the string follows the syntax given in section 2.
* <code>[LanguageTagCase(string)](#LanguageTagCase_string)</code> - Sets the given language tag to the case combination recommended by RFC 5646.
* <code>[LanguageTagFilter(System.Collections.Generic.IList, System.Collections.Generic.IList)](#LanguageTagFilter_System_Collections_Generic_IList_System_Collections_Generic_IList)</code> - Finds the language tags that match a priority list of basic language ranges.
* <code>[LanguageTagFilter(System.Collections.Generic.IList, System.Collections.Generic.IList, bool, bool)](#LanguageTagFilter_System_Collections_Generic_IList_System_Collections_Generic_IList_bool_bool)</code> - Finds the language tags that match a priority list of language ranges.
* <code>[LanguageTagLookup(System.Collections.Generic.IList, System.Collections.Generic.IList, string)](#LanguageTagLookup_System_Collections_Generic_IList_System_Collections_Generic_IList_string)</code> - Does a language tag lookup (under RFC 4647) for a matching language tag.
* <code>[LanguageTagLookup(System.Collections.Generic.IList, System.Collections.Generic.IList, string, bool)](#LanguageTagLookup_System_Collections_Generic_IList_System_Collections_Generic_IList_string_bool)</code> - Does a language tag lookup (under RFC 4647) for a matching language tag.
* <code>[LanguageTagLookup(string, System.Collections.Generic.IList, string)](#LanguageTagLookup_string_System_Collections_Generic_IList_string)</code> - Does a language tag lookup (under RFC 4647) for a matching language tag.
* <code>[LanguageTagLookup(string, System.Collections.Generic.IList, string, bool)](#LanguageTagLookup_string_System_Collections_Generic_IList_string_bool)</code> - Does a language tag lookup (under RFC 4647) for a matching language tag.
* <code>[MatchesLanguageTag(string, string)](#MatchesLanguageTag_string_string)</code> - Determines whether the given language tag matches the given language range.

<a id="GetLanguageList_string"></a>
### GetLanguageList

    public static System.Collections.Generic.IList GetLanguageList(
        string str);

Parses a language list from a Content-Language header field.

<b>Parameters:</b>

 * <i>str</i>: A string following the syntax of a Content-Language header field (see RFC 3282). This is a comma-separated list of language tags. RFC 5322 comments (in parentheses) can appear. This parameter can be null.

<b>Return Value:</b>

A list of language tags. Returns an empty list if <i>str</i>
is null or the empty string, or null if <i>str</i>
syntactically invalid.

<a id="GetRangeListWithQuality_string"></a>
### GetRangeListWithQuality

    public static System.Collections.Generic.IList GetRangeListWithQuality(
        string str);

Parses a language range list from an Accept-Language header field.

<b>Parameters:</b>

 * <i>str</i>: A string following the syntax of an Accept-Language header field (see RFC 3282). This is a comma-separated list of language ranges, with an optional "quality" after the language tag (examples include "en; q=0.5" or "de-DE"). RFC 5322 comments (in parentheses) can appear. This parameter can be null.

<b>Return Value:</b>

A list of language ranges with their associated qualities. The list will be sorted in descending order by quality; if two or more language ranges have the same quality, they will be sorted in the order in which they appeared in the given string. Returns null if <i>str</i>
is null or syntactically invalid.

<a id="IsLanguageRange_string"></a>
### IsLanguageRange

    public static bool IsLanguageRange(
        string str);

Returns whether the given string is a basic language range under RFC 4647. Examples include "*", "en-us", and "fr".

<b>Parameters:</b>

 * <i>str</i>: The string to check. Can be null.

<b>Return Value:</b>

 `true` if the given string is a basic language range; otherwise,  `false` .

<a id="IsLanguageRange_string_bool"></a>
### IsLanguageRange

    public static bool IsLanguageRange(
        string str,
        bool extended);

Returns whether the given string is a basic or extended language range under RFC 4647. Examples of basic (and extended) language ranges include "*", "en-us", and "fr". Examples of extended language ranges include "*-de" and "it-*".

<b>Parameters:</b>

 * <i>str</i>: The string to check. Can be null.

 * <i>extended</i>: Check whether the string is a basic language range if "false", or an extended language range if "true".

<b>Return Value:</b>

 `true` if the given string is a basic language range (depending on the <i>extended</i>
parameter); otherwise,  `false` .

<a id="IsPotentiallyValidLanguageTag_string"></a>
### IsPotentiallyValidLanguageTag

    public static bool IsPotentiallyValidLanguageTag(
        string str);

Returns true if (1) the given string is a well-formed language tag under RFC 5646 (that is, the string follows the syntax given in section 2.1 of that RFC), and (2) the language tag contains at most one extended language subtag, no variant subtags with the same value, and no extension singleton subtags with the same value.

<b>Parameters:</b>

 * <i>str</i>: The string to check.

<b>Return Value:</b>

 `true` , if the string meets the conditions given in the summary, `false` otherwise.

<a id="LanguageTagCase_string"></a>
### LanguageTagCase

    public static string LanguageTagCase(
        string str);

Sets the given language tag to the case combination recommended by RFC 5646. For example, "en-us" becomes "en-US", and "zh-hant" becomes "zh-Hant".

<b>Parameters:</b>

 * <i>str</i>: A string of a language tag. Can be null.

<b>Return Value:</b>

A text string in the recommended case combination, or null if <i>str</i>
is null.

<a id="LanguageTagFilter_System_Collections_Generic_IList_System_Collections_Generic_IList"></a>
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
The parameter <i>languages</i>
or <i>ranges</i>
is null.

 * System.ArgumentException:
The parameter <i>ranges</i>
contains a value that is not a basic language range, or <i>languages</i>
contains a value that is not a potentially valid language tag.

<a id="LanguageTagFilter_System_Collections_Generic_IList_System_Collections_Generic_IList_bool_bool"></a>
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

 * <i>matchStarAtEnd</i>: If true, treats any range equaling "*" as appearing at the end of the language priority list, no matter where it appears on that list.

<b>Return Value:</b>

A list of language tags that match the given range, in descending order of preference.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>languages</i>
or <i>ranges</i>
is null.

 * System.ArgumentException:
The parameter <i>ranges</i>
contains a value that is not a basic or extended language range, or <i>languages</i>
contains a value that is not a potentially valid language tag.

<a id="LanguageTagLookup_string_System_Collections_Generic_IList_string"></a>
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

The matching language tag, or the parameter <i>defaultValue</i>
if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>languages</i>
is null.

 * System.ArgumentException:
The parameter <i>range</i>
is not a basic language range, or <i>languages</i>
contains a value that is not a potentially valid language tag.

<a id="LanguageTagLookup_string_System_Collections_Generic_IList_string_bool"></a>
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

The matching language tag, or the parameter <i>defaultValue</i>
if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>languages</i>
is null.

 * System.ArgumentException:
The parameter <i>range</i>
is not a basic or extended language range, or <i>languages</i>
contains a value that is not a potentially valid language tag.

<a id="LanguageTagLookup_System_Collections_Generic_IList_System_Collections_Generic_IList_string"></a>
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

The matching language tag, or the parameter <i>defaultValue</i>
if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>languages</i>
or <i>ranges</i>
is null.

 * System.ArgumentException:
The parameter <i>ranges</i>
contains a value that is not a basic language range, or <i>languages</i>
contains a value that is not a potentially valid language tag.

<a id="LanguageTagLookup_System_Collections_Generic_IList_System_Collections_Generic_IList_string_bool"></a>
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

The matching language tag, or the parameter <i>defaultValue</i>
if there is no matching language tag.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>languages</i>
or <i>ranges</i>
is null.

 * System.ArgumentException:
The parameter <i>ranges</i>
contains a value that is not a basic or extended language range, or <i>languages</i>
contains a value that is not a potentially valid language tag.

<a id="MatchesLanguageTag_string_string"></a>
### MatchesLanguageTag

    public static bool MatchesLanguageTag(
        string range,
        string tag);

Determines whether the given language tag matches the given language range.

<b>Parameters:</b>

 * <i>range</i>: A basic language range (see the documentation for "IsLanguageRange").

 * <i>tag</i>: A language tag.

<b>Return Value:</b>

 `true` if the language tag matches the language range by the filtering method nder RFC 4647; otherwise,  `false` .

<b>Exceptions:</b>

 * System.ArgumentException:
The parameter <i>range</i>
is not a basic language range, or <i>tag</i>
is not a potentially valid language tag.
