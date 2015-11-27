## PeterO.Text.Encodings

    public static class Encodings

Contains methods for converting text from one character encoding to another. This class also contains convenience methods for converting strings and other character inputs to bytes.The Encoding Standard, which is a Candidate Recommendation as of early November 2015, defines algorithms for the most common character encodings used on Web pages and recommends the UTF-8 encoding for new specifications and Web pages. Calling the `GetEncoding(name)`  method returns one of the character encodings with the given name under the Encoding Standard.

Now let's define some terms:

 * A code point is a number that identifies a single text character, such as a letter, digit, or symbol.

 * An encoder is a class that converts a sequence of bytes to a sequence of code points in the universal character set (otherwise known under the name Unicode). An encoder implements the `ICharacterEncoder`  interface.

 * A decoder is a class that converts a sequence of Unicode code points to a sequence of bytes. A decoder implements the  `ICharacterDecoder`  interface.

 * An encoding is a mapping from bytes to universal code points and from universal code points to bytes. An encoding allows access to both an encoder and a decoder and implements the `ICharacterEncoding`  interface.

 * A character set is a set of code points which are each assigned to a single text character. (This may also be called acoded character set.)

There are several kinds of encodings:

 * Single-byte encodings define a character set that assigns one code point to one byte. For example, the ISO 8859 encodings and  `windows-1252`  are single-byte encodings. ASCII is also a single-byte encoding, although its character set only uses the lower 7 bits of an eight-bit byte. In the Encoding Standard, all single-byte encodings use the ASCII characters as the first 128 code points of their character sets. (Here, ASCII is the same as the International Reference Version of the ISO 646 standard.)

 * Multi-byte encodings define a character set that assigns some or all code points to several bytes. (They can include multiple character sets, such as single-byte ASCII and a multi-byte Chinese character set.) For example, most legacy East Asian encodings, such as  `shift_jis` ,  `gbk` , and  `big5` are multi-byte encodings, as well as  `utf-8`  and UTF-16, which both encode the Unicode character set.

 * Escape-based encodings use escape sequences to encode one or more character sets in the same sequence of bytes. Each escape sequence "shifts" the bytes that follow into a different character set. The best example of an escape-based encoding supported in the Encoding Standard is  `iso-2022-jp` , which supports several escape sequences that shift into different character sets, including a Katakana, a Kanji, and an ASCII character set.

 * The Encoding Standard also defines a replacement encoding, which causes a decoding error and is used to alias a few problematic or unsupported encoding names, such as `hz-gb-2312` .

Getting an Encoding

The Encoding Standard includes UTF-8, UTF-16, and many legacy encodings, and gives each one of them a name. The `GetEncoding(name)`  method takes a name string and returns an ICharacterEncoding object that implements that encoding, or `null`  if the name is unrecognized.

However, the Encoding Standard is designed to include only encodings commonly used on Web pages, not in other protocols such as email. For email, the Encoding class includes an alternate function  `GetEncoding(name, forEmail)` . Setting `forEmail`  to  `true`  will use rules modified from the Encoding Standard to better suit encoding and decoding text from email messages.

### UTF8

    public static readonly PeterO.Text.ICharacterEncoding UTF8;

Character encoding object for the UTF-8 character encoding.

### DecodeToString

    public static string DecodeToString(
        this PeterO.Text.ICharacterEncoding enc,
        byte[] bytes);

Reads a byte array from a data source and converts the bytes from a given encoding to a text string. Errors in decoding are handled by replacing erroneous bytes with the replacement character (U + FFFD).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows:  `enc.DecodeToString(bytes)` . If the object's class already has a DecodeToString method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>enc</i>: An ICharacterEncoding object.

 * <i>bytes</i>: A byte array.

<b>Returns:</b>

A string consisting of the decoded text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>enc</i>
 or  <i>bytes</i>
 is null.

### DecodeToString

    public static string DecodeToString(
        this PeterO.Text.ICharacterEncoding enc,
        byte[] bytes,
        int offset,
        int length);

Reads a portion of a byte array from a data source and converts the bytes from a given encoding to a text string. Errors in decoding are handled by replacing erroneous bytes with the replacement character (U + FFFD).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows:  `enc.DecodeToString(bytes, offset,
            length)` . If the object's class already has a DecodeToString method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>enc</i>: An object implementing a character encoding (gives access to an encoder and a decoder).

 * <i>bytes</i>: A byte array containing the desired portion to read.

 * <i>offset</i>: A zero-based index showing where the desired portion of  <i>bytes</i>
 begins.

 * <i>length</i>: The length, in bytes, of the desired portion of  <i>bytes</i>
 (but not more than  <i>bytes</i>
 's length).

<b>Returns:</b>

A string consisting of the decoded text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>enc</i>
 or  <i>bytes</i>
 is null.

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>bytes</i>
 's length, or  <i>bytes</i>
 's length minus  <i>offset</i>
 is less than  <i>length</i>
.

### DecodeToString

    public static string DecodeToString(
        this PeterO.Text.ICharacterEncoding encoding,
        PeterO.IByteReader transform);

Reads bytes from a data source and converts the bytes from a given encoding to a text string.In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows: "encoding.DecodeString(transform)". If the object's class already has a DecodeToString method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>encoding</i>: An object that implements a given character encoding. Any bytes that can't be decoded are converted to the replacement character (U + FFFD).

 * <i>transform</i>: An object that implements a byte stream.

<b>Returns:</b>

The converted string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoding</i>
 or  <i>transform</i>
 is null.

### EncodeToBytes

    public static byte[] EncodeToBytes(
        this PeterO.Text.ICharacterInput input,
        PeterO.Text.ICharacterEncoder encoder);

Reads Unicode characters from a character input and writes them to a byte array encoded using a given character encoding. When writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterInput and can be called as follows:  `input.EncodeToBytes(encoder)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: An object that implements a stream of universal code points.

 * <i>encoder</i>: An object that implements a character encoder.

<b>Returns:</b>

A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoder</i>
 or  <i>input</i>
 is null.

### EncodeToBytes

    public static byte[] EncodeToBytes(
        this PeterO.Text.ICharacterInput input,
        PeterO.Text.ICharacterEncoding encoding);

Reads Unicode characters from a character input and writes them to a byte array encoded using the given character encoder. When writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterInput and can be called as follows:  `input.EncodeToBytes(encoding)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: An object that implements a stream of universal code points.

 * <i>encoding</i>: An object that implements a given character encoding.

<b>Returns:</b>

A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoding</i>
 is null.

### EncodeToBytes

    public static byte[] EncodeToBytes(
        this string str,
        PeterO.Text.ICharacterEncoding enc);

Reads Unicode characters from a text string and writes them to a byte array encoded in a given character encoding. When reading the string, any unpaired surrogate characters are replaced with the replacement character (U + FFFD), and when writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing string and can be called as follows:  `str.EncodeToBytes(enc)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>enc</i>: An object implementing a character encoding (gives access to an encoder and a decoder).

<b>Returns:</b>

A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 or  <i>enc</i>
 is null.

### EncodeToWriter

    public static void EncodeToWriter(
        this PeterO.Text.ICharacterInput input,
        PeterO.Text.ICharacterEncoder encoder,
        PeterO.IWriter writer);

Reads Unicode characters from a character input and writes them to a byte array encoded in a given character encoding. When writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterInput and can be called as follows:  `input.EncodeToBytes(encoder)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: An object that implements a stream of universal code points.

 * <i>encoder</i>: An object that implements a character encoder.

 * <i>writer</i>: A byte writer to write the encoded bytes to.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoder</i>
 or  <i>input</i>
 is null.

### EncodeToWriter

    public static void EncodeToWriter(
        this PeterO.Text.ICharacterInput input,
        PeterO.Text.ICharacterEncoding encoding,
        PeterO.IWriter writer);

Reads Unicode characters from a character input and writes them to a byte array encoded using the given character encoder. When writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterInput and can be called as follows:  `input.EncodeToBytes(encoding)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: An object that implements a stream of universal code points.

 * <i>encoding</i>: An object that implements a character encoding.

 * <i>writer</i>: A byte writer to write the encoded bytes to.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoding</i>
 is null.

### EncodeToWriter

    public static void EncodeToWriter(
        this string str,
        PeterO.Text.ICharacterEncoding enc,
        PeterO.IWriter writer);

Converts a text string to bytes and writes the bytes to an output byte writer. When reading the string, any unpaired surrogate characters are replaced with the replacement character (U + FFFD), and when writing to the byte stream, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing string and can be called as follows:  `str.EncodeToBytes(enc, writer)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>str</i>: A string object to encode.

 * <i>enc</i>: An object implementing a character encoding (gives access to an encoder and a decoder).

 * <i>writer</i>: A byte writer where the encoded bytes will be written to.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 or  <i>enc</i>
 is null.

### GetDecoderInput

    public static PeterO.Text.ICharacterInput GetDecoderInput(
        this PeterO.Text.ICharacterEncoding encoding,
        PeterO.IByteReader stream);

Converts a character encoding into a character input stream, given a streamable source of bytes.In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows: "encoding.GetDecoderInput(transform)". If the object's class already has a GetDecoderInput method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>encoding</i>: Encoding that exposes a decoder to be converted into a character input stream. If the decoder returns -2 (indicating a decode error), the character input stream handles the error by returning a replacement character in its place.

 * <i>stream</i>: Byte stream to convert into Unicode characters.

<b>Returns:</b>

An ICharacterInput object.

### GetEncoding

    public static PeterO.Text.ICharacterEncoding GetEncoding(
        string name);

Returns a character encoding from the given name.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding. See the ResolveAlias method. Can be null.

<b>Returns:</b>

An ICharacterEncoding object.

### GetEncoding

    public static PeterO.Text.ICharacterEncoding GetEncoding(
        string name,
        bool forEmail);

Returns a character encoding from the given name.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding. See the ResolveAlias method. Can be null.

 * <i>forEmail</i>: A Boolean object.

<b>Returns:</b>

An ICharacterEncoding object.

### GetEncoding

    public static PeterO.Text.ICharacterEncoding GetEncoding(
        string name,
        bool forEmail,
        bool allowReplacement);

Returns a character encoding from the given name.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding. See the ResolveAlias method. Can be null.

 * <i>forEmail</i>: If false, uses the encoding resolution rules in the Encoding Standard. If true, uses modified rules as described in the ResolveAliasForEmail method.

 * <i>allowReplacement</i>: If true, allows the label `replacement`  to return the replacement encoding.

<b>Returns:</b>

An object that enables encoding and decoding text in the given character encoding. Returns null if the name is null or empty, or if it names an unrecognized or unsupported encoding.

### InputToString

    public static string InputToString(
        this PeterO.Text.ICharacterInput reader);

Reads Unicode characters from a character input and converts them to a text string.In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterInput and can be called as follows:  `reader.InputToString()` . If the object's class already has a InputToString method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>reader</i>: A character input whose characters will be converted to a text string.

<b>Returns:</b>

A text string containing the characters read.

### ResolveAlias

    public static string ResolveAlias(
        string name);

Resolves a character encoding's name to a standard form.

<b>Parameters:</b>

 * <i>name</i>: A string that names a given character encoding. Can be null. Any leading and trailing whitespace is removed and the name converted to lowercase before resolving the encoding's name. The Encoding Standard supports only the following encodings (and defines aliases for most of them):.

 *  `utf-8`  - UTF-8 (8-bit universal character set, the encoding recommended by the Encoding Standard for new data formats)

 *  `utf-16le`  - UTF-16 little-endian (16-bit UCS)

 *  `utf-16be`  - UTF-16 big-endian (16-bit UCS)

 * The special-purpose encoding  `x-user-defined`

 * The special-purpose encoding  `replacement` , which this function returns only if one of several aliases are passed to it, as defined in the Encoding Standard.

 * 28 legacy single-byte encodings:

 *  `windows-1252`  - Western Europe (Note: The Encoding Standard aliases the names  `us-ascii`  and  `iso-8859-1`  to `windows-1252` , which specifies a different character set from either; it differs from  `iso-8859-1`  by assigning different characters to some bytes from 0x80 to 0x9F. The Encoding Standard does this for compatibility with existing Web pages.)

 *  `iso-8859-2` ,  `windows-1250`  : Central Europe

 *  `iso-8859-10`  : Northern Europe

 *  `iso-8859-4` ,  `windows-1257`  : Baltic

 *  `iso-8859-13`  : Estonian

 *  `iso-8859-14`  : Celtic

 *  `iso-8859-16`  : Romanian

 *  `iso-8859-5` ,  `ibm866` ,  `koi8-r` , `windows-1251` ,  `x-mac-cyrillic`  : Cyrillic

 *  `koi8-u`  : Ukrainian

 *  `iso-8859-7` ,  `windows-1253`  : Greek

 *  `iso-8859-6` ,  `windows-1256`  : Arabic

 *  `iso-8859-8` ,  `iso-8859-8-i` ,  `windows-1255` : Hebrew

 *  `iso-8859-3`  : Latin 3

 *  `iso-8859-15`  : Latin 9

 *  `windows-1254`  : Turkish

 *  `windows-874`  : Thai

 *  `windows-1258`  : Vietnamese

 *  `macintosh`  : Mac Roman

 * Three legacy Japanese encodings:  `shift_jis` , `euc-jp` ,  `iso-2022-jp`

 * Two legacy simplified Chinese encodings:  `gbk`  and `gb18030`

 *  `big5`  : legacy traditional Chinese encoding

 *  `euc-kr`  : legacy Korean encoding

.

<b>Returns:</b>

A standardized name for the encoding. Returns the empty string if  <i>name</i>
 is null or empty, or if the encoding name is unsupported.

### ResolveAliasForEmail

    public static string ResolveAliasForEmail(
        string name);

Resolves a character encoding's name to a canonical form, using rules more suitable for email.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding. Can be null. Uses a modified version of the rules in the Encoding Standard to better conform, in some cases, to email standards like MIME. In addition to the encodings mentioned in ResolveAlias, the following additional encodings are supported:.

 *  `us-ascii`  - ASCII 7-bit encoding, rather than an alias to  `windows-1252` , as specified in the Encoding Standard

 *  `iso-8859-1`  - Latin-1 8-bit encoding, rather than an alias to  `windows-1252` , as specified in the Encoding Standard

 *  `utf-7`  - UTF-7 (7-bit universal character set)

.

<b>Returns:</b>

A standardized name for the encoding. Returns the empty string if  <i>name</i>
 is null or empty, or if the encoding name is unsupported.

### StringToBytes

    public static byte[] StringToBytes(
        this PeterO.Text.ICharacterEncoder encoder,
        string str);

Converts a text string to a byte array using the given character encoder. When reading the string, any unpaired surrogate characters are replaced with the replacement character (U + FFFD), and when writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoder and can be called as follows:  `encoder.StringToBytes(str)` . If the object's class already has a StringToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>encoder</i>: An object that implements a character encoder.

 * <i>str</i>: A text string to encode into a byte array.

<b>Returns:</b>

A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoder</i>
 or  <i>str</i>
 is null.

### StringToBytes

    public static byte[] StringToBytes(
        this PeterO.Text.ICharacterEncoding encoding,
        string str);

Converts a text string to a byte array encoded in a given character encoding. When reading the string, any unpaired surrogate characters are replaced with the replacement character (U + FFFD), and when writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows:  `encoding.StringToBytes(str)` . If the object's class already has a StringToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>encoding</i>: An object that implements a character encoding.

 * <i>str</i>: A string to be encoded into a byte array.

<b>Returns:</b>

A byte array containing the string encoded in the given text encoding.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoding</i>
 is null.

### StringToInput

    public static PeterO.Text.ICharacterInput StringToInput(
        this string str,
        int offset,
        int length);

Converts a portion of a text string to a character input. The resulting input can then be used to encode the text to bytes, or to read the string code point by code point, among other things.In the .NET implementation, this method is implemented as an extension method to any object implementing string and can be called as follows:  `str.StringToInput(offset, length)` . If the object's class already has a StringToInput method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>offset</i>: A zero-based index showing where the desired portion of  <i>str</i>
 begins.

 * <i>length</i>: The length, in code units, of the desired portion of  <i>str</i>
 (but not more than  <i>str</i>
 's length).

<b>Returns:</b>

An ICharacterInput object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>str</i>
 is null.

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>str</i>
 's length, or  <i>str</i>
 's length minus  <i>offset</i>
 is less than <i>length</i>
.
