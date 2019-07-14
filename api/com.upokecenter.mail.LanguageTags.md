# com.upokecenter.mail.LanguageTags

    public final class LanguageTags extends java.lang.Object

Contains methods for parsing and matching language tags.

## Methods

* `static java.util.List<java.lang.String> GetLanguageList​(java.lang.String str)`<br>
 Parses a language list from a Content-Language header field.
* `static java.util.List<StringAndQuality> GetRangeListWithQuality​(java.lang.String str)`<br>
 Parses a language range list from an Accept-Language header field.
* `static boolean IsLanguageRange​(java.lang.String str)`<br>
 Returns whether the given string is a basic language range under RFC 4647.
* `static boolean IsLanguageRange​(java.lang.String str,
               boolean extended)`<br>
 Returns whether the given string is a basic or extended language range under
 RFC 4647.
* `static boolean IsPotentiallyValidLanguageTag​(java.lang.String str)`<br>
 Returns true if (1) the given string is a well-formed language tag under RFC
 5646 (that is, the string follows the syntax given in section 2.1 of
 that RFC), and (2) the language tag contains at most one extended
 language subtag, no variant subtags with the same value, and no
 extension singleton subtags with the same value.
* `static java.lang.String LanguageTagCase​(java.lang.String str)`<br>
 Sets the given language tag to the case combination recommended by RFC 5646.
* `static java.util.List<java.lang.String> LanguageTagFilter​(java.util.List<java.lang.String> ranges,
                 java.util.List<java.lang.String> languages)`<br>
 Finds the language tags that match a priority list of basic language ranges.
* `static java.util.List<java.lang.String> LanguageTagFilter​(java.util.List<java.lang.String> ranges,
                 java.util.List<java.lang.String> languages,
                 boolean extended,
                 boolean matchStarAtEnd)`<br>
 Finds the language tags that match a priority list of language ranges.
* `static java.lang.String LanguageTagLookup​(java.lang.String range,
                 java.util.List<java.lang.String> languages,
                 java.lang.String defaultValue)`<br>
 Does a language tag lookup (under RFC 4647) for a matching language tag.
* `static java.lang.String LanguageTagLookup​(java.lang.String range,
                 java.util.List<java.lang.String> languages,
                 java.lang.String defaultValue,
                 boolean extended)`<br>
 Does a language tag lookup (under RFC 4647) for a matching language tag.
* `static java.lang.String LanguageTagLookup​(java.util.List<java.lang.String> ranges,
                 java.util.List<java.lang.String> languages,
                 java.lang.String defaultValue)`<br>
 Does a language tag lookup (under RFC 4647) for a matching language tag.
* `static java.lang.String LanguageTagLookup​(java.util.List<java.lang.String> ranges,
                 java.util.List<java.lang.String> languages,
                 java.lang.String defaultValue,
                 boolean extended)`<br>
 Does a language tag lookup (under RFC 4647) for a matching language tag.
* `static boolean MatchesLanguageTag​(java.lang.String range,
                  java.lang.String tag)`<br>
 Determines whether the given language tag matches the given language range.

## Method Details

### IsLanguageRange
    public static boolean IsLanguageRange​(java.lang.String str)
Returns whether the given string is a basic language range under RFC 4647.
  Examples include "*", "en-us", and "fr".

**Parameters:**

* <code>str</code> - The string to check. Can be null.

**Returns:**

* <code>true</code> if the given string is a basic language range;
 otherwise, <code>false</code>.

### IsLanguageRange
    public static boolean IsLanguageRange​(java.lang.String str, boolean extended)
Returns whether the given string is a basic or extended language range under
 RFC 4647. Examples of basic (and extended) language ranges include
  "*", "en-us", and "fr". Examples of extended language ranges
  include "*-de" and "it-*".

**Parameters:**

* <code>str</code> - The string to check. Can be null.

* <code>extended</code> - Check whether the string is a basic language range if
  "false", or an extended language range if "true".

**Returns:**

* <code>true</code> if the given string is a basic language range
 (depending on the <code>extended</code> parameter); otherwise, <code>
 false</code>.

### LanguageTagCase
    public static java.lang.String LanguageTagCase​(java.lang.String str)
Sets the given language tag to the case combination recommended by RFC 5646.
  For example, "en-us" becomes "en-US", and "zh-hant" becomes
  "zh-Hant".

**Parameters:**

* <code>str</code> - A string of a language tag. Can be null.

**Returns:**

* A text string in the recommended case combination, or null if <code>
 str</code> is null.

### GetLanguageList
    public static java.util.List<java.lang.String> GetLanguageList​(java.lang.String str)
Parses a language list from a Content-Language header field.

**Parameters:**

* <code>str</code> - A string following the syntax of a Content-Language header field
 (see RFC 3282). This is a comma-separated list of language tags. RFC
 5322 comments (in parentheses) can appear. This parameter can be
 null.

**Returns:**

* A list of language tags. Returns an empty list if <code>str</code> is
 null or the empty string, or null if <code>str</code> syntactically
 invalid.

### GetRangeListWithQuality
    public static java.util.List<StringAndQuality> GetRangeListWithQuality​(java.lang.String str)
Parses a language range list from an Accept-Language header field.

**Parameters:**

* <code>str</code> - A string following the syntax of an Accept-Language header field
 (see RFC 3282). This is a comma-separated list of language ranges,
  with an optional "quality" after the language tag (examples include
  "en; q=0.5" or "de-DE"). RFC 5322 comments (in parentheses) can
 appear. This parameter can be null.

**Returns:**

* A list of language ranges with their associated qualities. The list
 will be sorted in descending order by quality; if two or more
 language ranges have the same quality, they will be sorted in the
 order in which they appeared in the given string. Returns null if
 <code>str</code> is null or syntactically invalid.

### LanguageTagFilter
    public static java.util.List<java.lang.String> LanguageTagFilter​(java.util.List<java.lang.String> ranges, java.util.List<java.lang.String> languages, boolean extended, boolean matchStarAtEnd)
Finds the language tags that match a priority list of language ranges.

**Parameters:**

* <code>ranges</code> - A list of language ranges (see documentation for the
  "IsLanguageRange" method), which should be given in order of
 descending preference.

* <code>languages</code> - A list of language tags, which should be given in order of
 descending preference.

* <code>extended</code> - If true, the ranges in "ranges" are extended language
 ranges; otherwise, they are basic language ranges.

* <code>matchStarAtEnd</code> - If true, treats any range equaling "*" as
 appearing at the end of the language priority list, no matter where
 it appears on that list.

**Returns:**

* A list of language tags that match the given range, in descending
 order of preference.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>languages</code> or <code>
 ranges</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>ranges</code> contains a value that
 is not a basic or extended language range, or <code>languages</code>
 contains a value that is not a potentially valid language tag.

### MatchesLanguageTag
    public static boolean MatchesLanguageTag​(java.lang.String range, java.lang.String tag)
Determines whether the given language tag matches the given language range.

**Parameters:**

* <code>range</code> - A basic language range (see the documentation for
  "IsLanguageRange").

* <code>tag</code> - A language tag.

**Returns:**

* <code>true</code> if the language tag matches the language range by the
 filtering method under RFC 4647; otherwise, <code>false</code>.

**Throws:**

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>range</code> is not a basic
 language range, or <code>tag</code> is not a potentially valid language
 tag.

### LanguageTagLookup
    public static java.lang.String LanguageTagLookup​(java.lang.String range, java.util.List<java.lang.String> languages, java.lang.String defaultValue)
Does a language tag lookup (under RFC 4647) for a matching language tag.

**Parameters:**

* <code>range</code> - A basic language range (see the documentation for
  "IsLanguageRange").

* <code>languages</code> - A list of language tags, which should be given in order of
 descending preference.

* <code>defaultValue</code> - The value to return if no matching language tag was
 found.

**Returns:**

* The matching language tag, or the parameter <code>defaultValue</code> if
 there is no matching language tag.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>languages</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>range</code> is not a basic
 language range, or <code>languages</code> contains a value that is not a
 potentially valid language tag.

### LanguageTagLookup
    public static java.lang.String LanguageTagLookup​(java.util.List<java.lang.String> ranges, java.util.List<java.lang.String> languages, java.lang.String defaultValue)
Does a language tag lookup (under RFC 4647) for a matching language tag.

**Parameters:**

* <code>ranges</code> - A list of basic language ranges (see documentation for the
  "IsLanguageRange" method), which should be given in order of
 descending preference.

* <code>languages</code> - A list of language tags, which should be given in order of
 descending preference.

* <code>defaultValue</code> - The value to return if no matching language tag was
 found.

**Returns:**

* The matching language tag, or the parameter <code>defaultValue</code> if
 there is no matching language tag.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>languages</code> or <code>
 ranges</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>ranges</code> contains a value that
 is not a basic language range, or <code>languages</code> contains a value
 that is not a potentially valid language tag.

### LanguageTagFilter
    public static java.util.List<java.lang.String> LanguageTagFilter​(java.util.List<java.lang.String> ranges, java.util.List<java.lang.String> languages)
Finds the language tags that match a priority list of basic language ranges.

**Parameters:**

* <code>ranges</code> - A list of basic language ranges (see documentation for the
  "IsLanguageRange" method), which should be given in order of
 descending preference.

* <code>languages</code> - A list of language tags, which should be given in order of
 descending preference.

**Returns:**

* A list of language tags that match the given range, in descending
 order of preference.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>languages</code> or <code>
 ranges</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>ranges</code> contains a value that
 is not a basic language range, or <code>languages</code> contains a value
 that is not a potentially valid language tag.

### LanguageTagLookup
    public static java.lang.String LanguageTagLookup​(java.lang.String range, java.util.List<java.lang.String> languages, java.lang.String defaultValue, boolean extended)
Does a language tag lookup (under RFC 4647) for a matching language tag.

**Parameters:**

* <code>range</code> - A language range (see the documentation for "IsLanguageRange").

* <code>languages</code> - A list of language tags, which should be given in order of
 descending preference.

* <code>defaultValue</code> - The value to return if no matching language tag was
 found.

* <code>extended</code> - If true, "range" is an extended language range; otherwise,
 it's a are basic language range.

**Returns:**

* The matching language tag, or the parameter <code>defaultValue</code> if
 there is no matching language tag.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>languages</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>range</code> is not a basic or
 extended language range, or <code>languages</code> contains a value that
 is not a potentially valid language tag.

### LanguageTagLookup
    public static java.lang.String LanguageTagLookup​(java.util.List<java.lang.String> ranges, java.util.List<java.lang.String> languages, java.lang.String defaultValue, boolean extended)
Does a language tag lookup (under RFC 4647) for a matching language tag.

**Parameters:**

* <code>ranges</code> - A list of language ranges (see documentation for the
  "IsLanguageRange" method), which should be given in order of
 descending preference.

* <code>languages</code> - A list of language tags, which should be given in order of
 descending preference.

* <code>defaultValue</code> - The value to return if no matching language tag was
 found.

* <code>extended</code> - If true, the ranges in "ranges" are extended language
 ranges; otherwise, they are basic language ranges.

**Returns:**

* The matching language tag, or the parameter <code>defaultValue</code> if
 there is no matching language tag.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>languages</code> or <code>
 ranges</code> is null.

* <code>java.lang.IllegalArgumentException</code> - The parameter <code>ranges</code> contains a value that
 is not a basic or extended language range, or <code>languages</code>
 contains a value that is not a potentially valid language tag.

### IsPotentiallyValidLanguageTag
    public static boolean IsPotentiallyValidLanguageTag​(java.lang.String str)
Returns true if (1) the given string is a well-formed language tag under RFC
 5646 (that is, the string follows the syntax given in section 2.1 of
 that RFC), and (2) the language tag contains at most one extended
 language subtag, no variant subtags with the same value, and no
 extension singleton subtags with the same value.

**Parameters:**

* <code>str</code> - The string to check.

**Returns:**

* <code>true</code>, if the string meets the conditions given in the
 summary, <code>false</code> otherwise.
