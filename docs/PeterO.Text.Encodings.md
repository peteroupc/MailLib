## PeterO.Text.Encodings

    public static class Encodings

Contains methods for converting text from one character encoding to another. This class also contains convenience methods for converting strings and other character inputs to bytes.A character encoding is a mapping from characters to a sequence of bytes.

### UTF8

    public static readonly PeterO.Text.ICharacterEncoding UTF8;

Character encoding object for the UTF-8 character encoding.

### DecodeToString

    public static string DecodeToString(
        this PeterO.Text.ICharacterEncoding enc,
        byte[] bytes);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows:  `enc.DecodeToString(bytes)` . If the object's class already has a DecodeToString method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>enc</i>: An ICharacterEncoding object.

 * <i>bytes</i>: A byte array.

<b>Returns:</b>

A string object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>enc</i>
 or  <i>bytes</i>
 is null.

### DecodeToString

    public static string DecodeToString(
        this PeterO.Text.ICharacterEncoding enc,
        byte[] bytes,
        int offset,
        int length);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows:  `enc.DecodeToString(bytes, offset,
            length)` . If the object's class already has a DecodeToString method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>enc</i>: An ICharacterEncoding object.

 * <i>bytes</i>: A byte array.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: Another 32-bit signed integer.

<b>Returns:</b>

A string object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>enc</i>
 or  <i>bytes</i>
 is null.

### DecodeToString

    public static string DecodeToString(
        this PeterO.Text.ICharacterEncoding encoding,
        PeterO.ITransform transform);

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

Reads Unicode characters from a character input and writes them to a byte array encoded in a given character encoding. When writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterInput and can be called as follows:  `input.EncodeToBytes(encoder)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: Not documented yet.

 * <i>encoder</i>: Not documented yet.

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

 * <i>input</i>: Not documented yet.

 * <i>encoding</i>: Not documented yet.

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

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing string and can be called as follows:  `str.EncodeToBytes(enc)` . If the object's class already has a EncodeToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>enc</i>: An ICharacterEncoding object.

<b>Returns:</b>

A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 or  <i>enc</i>
 is null.

### GetDecoderInput

    public static PeterO.Text.ICharacterInput GetDecoderInput(
        this PeterO.Text.ICharacterEncoding encoding,
        PeterO.ITransform stream);

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

 * <i>name</i>: A string naming a character encoding. See the ResolveAlias method.

<b>Returns:</b>

An ICharacterEncoding object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

### GetEncoding

    public static PeterO.Text.ICharacterEncoding GetEncoding(
        string name,
        bool forEmail);

Returns a character encoding from the given name.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding. See the ResolveAlias method.

 * <i>forEmail</i>: If false, uses the encoding resolution rules in the Encoding Standard. If true, uses modified rules as described in the ResolveAliasForEmail method.

<b>Returns:</b>

An object that enables encoding and decoding text in the given character encoding. Returns null if the name is null or empty, or if it names an unrecognized or unsupported encoding.

### InputToString

    public static string InputToString(
        this PeterO.Text.ICharacterInput reader);

Reads Unicode characters from a character input and converts them to a text string.In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterInput and can be called as follows:  `reader.InputToString()` . If the object's class already has a InputToString method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>reader</i>: Not documented yet.

<b>Returns:</b>

A string object.

### ResolveAlias

    public static string ResolveAlias(
        string name);

Resolves a character encoding's name to a standard form.

<b>Parameters:</b>

 * <i>name</i>: A string that names a given character encoding. Any leading and trailing whitespace is removed and the name converted to lowercase before resolving the encoding's name. The Encoding Standard supports only the following encodings (and defines aliases for most of them):.

 *  `utf-8`  - UTF-8 (8-bit universal character set, the encoding recommended by the Encoding Standard for new data formats)

 *  `utf-16le`  - UTF-16 little-endian (16-bit UCS)

 *  `utf-16be`  - UTF-16 big-endian (16-bit UCS)

 * Two special purpose encodings:  `x-user-defined`  and `replacement`

 * 28 legacy single-byte encodings:

 *  `windows-1252`  - Western Europe

 *  `iso-8859-2` ,  `windows-1250`  - Central Europe

 *  `iso-8859-10`  - Northern Europe

 *  `iso-8859-4` ,  `windows-1257`  - Baltic

 *  `iso-8859-13`  - Estonian

 *  `iso-8859-14`  - Celtic

 *  `iso-8859-16`  - Romanian

 *  `iso-8859-5` ,  `ibm866` ,  `koi8-r` , `windows-1251` ,  `x-mac-cyrillic`  - Cyrillic

 *  `koi8-u`  - Ukrainian

 *  `iso-8859-7` ,  `windows-1253`  - Greek

 *  `iso-8859-6` ,  `windows-1256`  - Arabic

 *  `iso-8859-8` ,  `iso-8859-8-i` ,  `windows-1255` - Hebrew

 *  `iso-8859-3`  - Latin 3

 *  `iso-8859-15`  - Latin 9

 *  `windows-1254`  - Turkish

 *  `windows-874`  - Thai

 *  `windows-1258`  - Vietnamese

 *  `macintosh`  - Mac Roman

 * Three legacy Japanese encodings:  `shift_jis` , `euc-jp` ,  `iso-2022-jp`

 * Two legacy simplified Chinese encodings:  `gbk`  and `gb18030`

 *  `big5`  - legacy traditional Chinese encoding

 *  `euc-kr`  - legacy Korean encoding

<b>Returns:</b>

A standardized name for the encoding. Returns the empty string if  <i>name</i>
 is null or empty, or if the encoding name is unsupported.

### ResolveAliasForEmail

    public static string ResolveAliasForEmail(
        string name);

Resolves a character encoding's name to a canonical form, using rules more suitable for email.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding. Uses a modified version of the rules in the Encoding Standard to better conform, in some cases, to email standards like MIME. In addition to the encodings mentioned in ResolveAlias, the following additional encodings are supported:.

 *  `us-ascii`  - ASCII 7-bit encoding, rather than an alias to  `windows-1252` , as specified in the Encoding Standard

 *  `iso-8859-1`  - Latin-1 8-bit encoding, rather than an alias to  `windows-1252` , as specified in the Encoding Standard

 *  `utf-7`  - UTF-7 (7-bit universal character set)

<b>Returns:</b>

A standardized name for the encoding. Returns the empty string if  <i>name</i>
 is null or empty, or if the encoding name is unsupported.

### StringToBytes

    public static byte[] StringToBytes(
        this PeterO.Text.ICharacterEncoder encoder,
        string str);

Converts a text string to a byte array using the given character encoder. When reading the string, any unpaired surrogate characters are replaced with the replacement character (U + FFFD), and when writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoder and can be called as follows:  `encoder.StringToBytes(str)` . If the object's class already has a StringToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>encoder</i>: Not documented yet.

 * <i>str</i>: Not documented yet.

<b>Returns:</b>

A byte array.

### StringToBytes

    public static byte[] StringToBytes(
        this PeterO.Text.ICharacterEncoding encoding,
        string str);

Converts a text string to a byte array encoded in a given character encoding. When reading the string, any unpaired surrogate characters are replaced with the replacement character (U + FFFD), and when writing to the byte array, any characters that can't be encoded are replaced with the byte 0x3f (the question mark character).In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows:  `encoding.StringToBytes(str)` . If the object's class already has a StringToBytes method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>encoding</i>: Not documented yet.

 * <i>str</i>: Not documented yet.

<b>Returns:</b>

A byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>encoding</i>
 is null.

### StringToInput

    public static PeterO.Text.ICharacterInput StringToInput(
        this string str,
        int offset,
        int length);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing string and can be called as follows:  `str.StringToInput(offset, length)` . If the object's class already has a StringToInput method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>str</i>: A string object.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: Another 32-bit signed integer.

<b>Returns:</b>

An ICharacterInput object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>str</i>
 is null.
