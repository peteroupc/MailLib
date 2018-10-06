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

 Finds the language tags that match a priority list of basic language ranges.

* `static String LanguageTagLookup​(String range,
                 List<String> languages,
                 String defaultValue,
                 boolean extended)`<br>

 Does a language tag lookup (under RFC 4647) for a matching language tag.
* `static String LanguageTagLookup​(List<String> ranges,
                 List<String> languages,
                 String defaultValue)`<br>
 Does a language tag lookup (under RFC 4647) for a matching language tag.

* `static String LanguageTagLookup​(List<String> ranges,
                 List<String> languages,
                 String defaultValue,
                 boolean extended)`<br>

 Does a language tag lookup (under RFC 4647) for a matching language tag.

### MatchesLanguageTag
    public static boolean MatchesLanguageTag​(String range, String tag)
Determines whether the given language tag matches the given language range.

**Parameters:**

* <code>range</code> - A basic language range (see the documentation for
 "IsLanguageRange").

* <code>tag</code> - A language tag.

**Returns:**

* <code>true</code> if the language tag matches the language range by the
 filtering method under RFC 4647; otherwise, <code>false</code> .

**Throws:**

* <code>IllegalArgumentException</code> - The parameter <code>range</code> is not a basic
 language range, or <code>tag</code> is not a potentially valid language
 tag.

### LanguageTagLookup
    public static String LanguageTagLookup​(String range, List<String> languages, String defaultValue)

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

* <code>NullPointerException</code> - The parameter <code>languages</code> is
 null.

* <code>IllegalArgumentException</code> - The parameter <code>range</code> is not a basic
 language range, or <code>languages</code> contains a value that is not a
 potentially valid language tag.

### LanguageTagLookup
    public static String LanguageTagLookup​(List<String> ranges, List<String> languages, String defaultValue)
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

* <code>NullPointerException</code> - The parameter <code>languages</code> or
 <code>ranges</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>ranges</code> contains a
 value that is not a basic language range, or <code>languages</code>
 contains a value that is not a potentially valid language tag.

### LanguageTagFilter
    public static List<String> LanguageTagFilter​(List<String> ranges, List<String> languages)
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

* <code>NullPointerException</code> - The parameter <code>languages</code> or
 <code>ranges</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>ranges</code> contains a
 value that is not a basic language range, or <code>languages</code>
 contains a value that is not a potentially valid language tag.

### LanguageTagLookup
    public static String LanguageTagLookup​(String range, List<String> languages, String defaultValue, boolean extended)
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

* <code>NullPointerException</code> - The parameter <code>languages</code> is
 null.

* <code>IllegalArgumentException</code> - The parameter <code>range</code> is not a basic
 or extended language range, or <code>languages</code> contains a value
 that is not a potentially valid language tag.

### LanguageTagLookup
    public static String LanguageTagLookup​(List<String> ranges, List<String> languages, String defaultValue, boolean extended)
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

* <code>NullPointerException</code> - The parameter <code>languages</code> or
 <code>ranges</code> is null.

* <code>IllegalArgumentException</code> - The parameter <code>ranges</code> contains a
 value that is not a basic or extended language range, or <code>
 languages</code> contains a value that is not a potentially valid language
 tag.

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

* <code>true</code> , if the string meets the conditions given in the
 summary, <code>false</code> otherwise.
