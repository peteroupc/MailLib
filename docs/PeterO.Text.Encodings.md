## PeterO.Text.Encodings

    public static class Encodings

Not documented yet.

### UTF8

    public static readonly PeterO.Text.ICharacterEncoding UTF8;

Character encoding object for the UTF-8 character encoding.

### DecodeToString

    public static string DecodeToString(
        this PeterO.Text.ICharacterEncoding encoding,
        PeterO.ITransform transform);

Reads bytes from a data source and converts the bytes to a text string in a given encoding. In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows: "encoding.DecodeString(transform)". If the object's class already has a DecodeString method with the same parameters, that method takes precedence over this extension method.

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

### GetDecoderInput

    public static PeterO.Text.ICharacterInput GetDecoderInput(
        this PeterO.Text.ICharacterEncoding encoding,
        PeterO.ITransform stream);

Converts a character encoding into a character input stream. In the .NET implementation, this method is implemented as an extension method to any object implementing ICharacterEncoding and can be called as follows: "encoding.GetDecoderInput(transform)". If the object's class already has a GetDecoderInput method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>encoding</i>: Encoding that exposes a decoder to be converted into a character input stream. If the decoder returns -2 (indicating a decode error), the character input stream handles the error by returning a replacement character in its place.

 * <i>stream</i>: Byte stream to convert into Unicode characters.

<b>Returns:</b>

An ICharacterInput object.

### GetEncoding

    public static PeterO.Text.ICharacterEncoding GetEncoding(
        string name);

Not documented yet.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding.

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

An object that enables encoding and decoding text in the given character encoding.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>name</i>
 is null.

### ResolveAlias

    public static string ResolveAlias(
        string name);

<b>Parameters:</b>

 * <i>name</i>: Not documented yet.

<b>Returns:</b>

A string object.

### ResolveAliasForEmail

    public static string ResolveAliasForEmail(
        string name);

Resolves a character encoding's name to a canonical form, using rules more suitable for email.

<b>Parameters:</b>

 * <i>name</i>: A string naming a character encoding. Uses a modified version of the rules in the Encoding Standard to better conform, in some cases, to email standards such as MIME, and some additional encodings may be supported. For instance, setting this value to true will enable the "utf-7" encoding and change.  `"us-ascii"` and "iso-8859-1" to a 7 bit encoding and the 8-bit Latin-1 encoding, respectively, rather than aliases to "windows-1252", as specified in the Encoding Standard.

<b>Returns:</b>

A standardized name for the encoding. Returns the empty string if  <i>name</i>
 is null or empty, or if the encoding name is unsupported.
