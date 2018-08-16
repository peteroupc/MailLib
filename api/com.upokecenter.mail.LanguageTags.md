# com.upokecenter.mail.LanguageTags

    public final class LanguageTags extends Object

Contains methods for parsing and matching language tags.

## Methods

* `static List<String> GetLanguageList​(String str)`<br>
 Parses a language list from a Content-Language header field.
* `static List<StringAndQuality> GetRangeListWithQuality​(String str)`<br>
 Parses a language range list from an Accept-Language header field.
* `static boolean IsLanguageRange​(String str)`<br>
 Returns whether the given string is a basic language range under RFC 4647.
* `static boolean IsLanguageRange​(String str,
               boolean extended)`<br>
 Returns whether the given string is a basic or extended language range under
 RFC 4647.
* `static boolean IsPotentiallyValidLanguageTag​(String str)`<br>
 Returns true if (1) the given string is a well-formed language tag under RFC
 5646 (that is, the string follows the syntax given in section 2.1 of
 that RFC), and (2) the language tag contains at most one extended
 language subtag, no variant subtags with the same value, and no
 extension singleton subtags with the same value.
* `static String LanguageTagCase​(String str)`<br>
 Sets the given language tag to the case combination recommended by RFC 5646.
* `static List<String> LanguageTagFilter​(List<String> ranges,
                 List<String> languages)`<br>
* `static List<String> LanguageTagFilter​(List<String> ranges,
                 List<String> languages,
                 boolean extended,
                 boolean matchStarAtEnd)`<br>
* `static String LanguageTagLookup​(String range,
                 List<String> languages,
                 String defaultValue)`<br>
* `static String LanguageTagLookup​(String range,
                 List<String> languages,
                 String defaultValue,
                 boolean extended)`<br>
* `static String LanguageTagLookup​(List<String> ranges,
                 List<String> languages,
                 String defaultValue)`<br>
* `static String LanguageTagLookup​(List<String> ranges,
                 List<String> languages,
                 String defaultValue,
                 boolean extended)`<br>
* `static boolean MatchesLanguageTag​(String range,
                  String tag)`<br>
 Determines whether the given language tag matches the given language range.

## Method Details

### IsLanguageRange
    public static boolean IsLanguageRange​(String str)
Returns whether the given string is a basic language range under RFC 4647.
 Examples include "*", "en-us", and "fr".

**Parameters:**

* <code>str</code> - The string to check. Can be null.

**Returns:**

* <code>true</code> if the given string is a basic language range;
 otherwise, <code>false</code>.

### IsLanguageRange
    public static boolean IsLanguageRange​(String str, boolean extended)
Returns whether the given string is a basic or extended language range under
 RFC 4647. Examples of basic (and extended) language ranges include
 "*", "en-us", and "fr". Examples of extended language ranges include
 "*-de" and "it-*".

**Parameters:**

* <code>str</code> - The string to check. Can be null.

* <code>extended</code> - Check whether the string is a basic language range if
 "false", or an extended language range if "true".

**Returns:**

* <code>true</code> if the given string is a basic language range
 (depending on the "extended" parameter); otherwise, <code>false</code>.

### LanguageTagCase
    public static String LanguageTagCase​(String str)
Sets the given language tag to the case combination recommended by RFC 5646.
 For example, "en-us" becomes "en-US", and "zh-hant" becomes
 "zh-Hant".

**Parameters:**

* <code>str</code> - A string of a language tag. Can be null.

**Returns:**

* A text string in the recommended case combination, or null if "str"
 is null.

### GetLanguageList
    public static List<String> GetLanguageList​(String str)
Parses a language list from a Content-Language header field.

**Parameters:**

* <code>str</code> - A string following the syntax of a Content-Language header field
 (see RFC 3282). This is a comma-separated list of language tags. RFC
 5322 comments (in parentheses) can appear. This parameter can be
 null.

**Returns:**

* A list of language tags. Returns an empty list if "str" is null or
 the empty string, or null if "str" syntactically invalid.

### GetRangeListWithQuality
    public static List<StringAndQuality> GetRangeListWithQuality​(String str)
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
 "str" is null or syntactically invalid.

### LanguageTagFilter
    public static List<String> LanguageTagFilter​(List<String> ranges, List<String> languages, boolean extended, boolean matchStarAtEnd)

**Parameters:**

* <code>matchStarAtEnd</code> - The parameter <code>matchStarAtEnd</code> is not documented
 yet.

### MatchesLanguageTag
    public static boolean MatchesLanguageTag​(String range, String tag)
Determines whether the given language tag matches the given language range.

**Parameters:**

* <code>range</code> - A basic language range (see the documentation for
 "IsLanguageRange").

* <code>tag</code> - A language tag.

**Returns:**

* <code>true</code> if the language tag matches the language range by the
 filtering method under RFC 4647; otherwise, <code>false</code>.

**Throws:**

* <code>IllegalArgumentException</code> - "range" is not a basic language range, or "tag" is
 not a potentially valid language tag.

### LanguageTagLookup
    public static String LanguageTagLookup​(String range, List<String> languages, String defaultValue)
### LanguageTagLookup
    public static String LanguageTagLookup​(List<String> ranges, List<String> languages, String defaultValue)
### LanguageTagFilter
    public static List<String> LanguageTagFilter​(List<String> ranges, List<String> languages)
### LanguageTagLookup
    public static String LanguageTagLookup​(String range, List<String> languages, String defaultValue, boolean extended)
### LanguageTagLookup
    public static String LanguageTagLookup​(List<String> ranges, List<String> languages, String defaultValue, boolean extended)
### IsPotentiallyValidLanguageTag
    public static boolean IsPotentiallyValidLanguageTag​(String str)
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
