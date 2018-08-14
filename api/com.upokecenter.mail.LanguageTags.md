# com.upokecenter.mail.LanguageTags

    public final class LanguageTags extends Object

Contains methods for parsing and matching language tags.

## Methods

* `static List<String> GetLanguageList​(String str)`<br>
 Parses a language list from a Content-Language header field.
* `static List<StringAndQuality> GetRangeListWithQuality​(String str)`<br>
 Parses a language range list from an Accept-Language header field.
* `static boolean IsLanguageRange​(String str)`<br>
 Not documented yet.
* `static boolean IsLanguageRange​(String str,
               boolean extended)`<br>
 Not documented yet.
* `static boolean IsPotentiallyValidLanguageTag​(String str)`<br>
 Returns true if (1) the given string is a well-formed language tag under RFC
 5646 (that is, the string follows the syntax given in section 2.1 of
 that RFC), and (2) the language tag contains at most one extended
 language subtag, no variant subtags with the same value, and no
 extension singleton subtags with the same value.
* `static String LanguageTagCase​(String str)`<br>
 Not documented yet.
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
 Not documented yet.

## Method Details

### IsLanguageRange
    public static boolean IsLanguageRange​(String str)
Not documented yet.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is not documented yet.

**Returns:**

* Either <code>true</code> or <code>false</code>.

### IsLanguageRange
    public static boolean IsLanguageRange​(String str, boolean extended)
Not documented yet.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is not documented yet.

* <code>extended</code> - The parameter <code>extended</code> is not documented yet.

**Returns:**

* Either <code>true</code> or <code>false</code>.

### LanguageTagCase
    public static String LanguageTagCase​(String str)
Not documented yet.

**Parameters:**

* <code>str</code> - The parameter <code>str</code> is not documented yet.

**Returns:**

* A text string.

### GetLanguageList
    public static List<String> GetLanguageList​(String str)
Parses a language list from a Content-Language header field.

**Parameters:**

* <code>str</code> - A string following the syntax of a Content-Language header field
 (see RFC 3282). This is a comma-separated list of language tags. RFC
 5322 comments (in parentheses) can appear. This parameter can be
 null.

**Returns:**

* A list of language tags. Returns null if "str" is null or
 syntactically invalid.

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
### MatchesLanguageTag
    public static boolean MatchesLanguageTag​(String range, String tag)
Not documented yet.

**Parameters:**

* <code>range</code> - The parameter <code>range</code> is not documented yet.

* <code>tag</code> - The parameter <code>tag</code> is not documented yet.

**Returns:**

* Either <code>true</code> or <code>false</code>.

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
