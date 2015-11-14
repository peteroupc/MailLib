using System;
using System.Collections.Generic;

namespace PeterO.Text.Encoders {
    /// <summary>Not documented yet.</summary>
  public static class Encodings {
    private class DecoderToInputClass : ICharacterInput {
      private ITransform stream;
      private ICharacterDecoder reader;

      public DecoderToInputClass(ICharacterDecoder reader, ITransform stream) {
        this.reader = reader;
        this.stream = stream;
      }

/// </summary>
/// <returns></returns>
public int ReadChar() {
        int c = this.reader.ReadChar(this.stream);
        return (c == -2) ? 0xfffd : c;
      }

/// </summary>
/// <param name="buffer"></param>
/// <param name="offset"></param>
/// <param name="length"></param>
/// <returns></returns>
public int Read(int[] buffer, int offset, int length) {
        if (buffer == null) {
          throw new ArgumentNullException("buffer");
        }
        if (offset < 0) {
          throw new ArgumentException("offset (" + offset +
            ") is less than " + 0);
        }
        if (offset > buffer.Length) {
          throw new ArgumentException("offset (" + offset + ") is more than " +
            buffer.Length);
        }
        if (length < 0) {
          throw new ArgumentException("length (" + length +
            ") is less than " + 0);
        }
        if (length > buffer.Length) {
          throw new ArgumentException("length (" + length + ") is more than " +
            buffer.Length);
        }
        if (buffer.Length - offset < length) {
          throw new ArgumentException("buffer's length minus " + offset + " (" +
            (buffer.Length - offset) + ") is less than " + length);
        }
        int count = 0;
        for (var i = 0; i < length; ++i) {
          int c = this.ReadChar();
          if (c == -1) {
            break;
          }
          buffer[offset] = c;
          ++count;
          ++offset;
        }
        return count;
      }
    }

    private static IDictionary<string, string> charsetAliases =
        CreateAliasMap();

    /// <summary>Character encoding object for the UTF-8 character
    /// encoding.</summary>
    public static readonly ICharacterEncoding UTF8 = new EncodingUtf8();

    private static string TrimAsciiWhite(string str) {
      return string.IsNullOrEmpty(str) ? str :
        TrimAsciiWhiteLeft(TrimAsciiWhiteRight(str));
    }

    private static string TrimAsciiWhiteLeft(string str) {
      if (string.IsNullOrEmpty(str)) {
        return str;
      }
      int index = 0;
      int valueSLength = str.Length;
      while (index < valueSLength) {
        char c = str[index];
        if (c != 0x09 && c != 0x20 && c != 0x0c && c != 0x0d && c != 0x0a) {
          break;
        }
        ++index;
      }
      return (index == valueSLength) ? String.Empty : ((index == 0) ? str :
        str.Substring(index));
    }

    private static string TrimAsciiWhiteRight(string str) {
      if (string.IsNullOrEmpty(str)) {
        return str;
      }
      int index = str.Length - 1;
      while (index >= 0) {
        char c = str[index];
        if (c != 0x09 && c != 0x20 && c != 0x0c && c != 0x0d && c != 0x0a) {
          return str.Substring(0, index + 1);
        }
        --index;
      }
      return String.Empty;
    }

    private static IDictionary<string, string> CreateAliasMap() {
      var aliases = new Dictionary<string, string>();
      aliases["unicode-1-1-utf-8"] = "utf-8";
      aliases["utf-8"] = "utf-8";
      aliases["utf8"] = "utf-8";
      aliases["866"] = "ibm866";
      aliases["cp866"] = "ibm866";
      aliases["csibm866"] = "ibm866";
      aliases["ibm866"] = "ibm866";
      aliases["csisolatin2"] = "iso-8859-2";
      aliases["iso-8859-2"] = "iso-8859-2";
      aliases["iso-ir-101"] = "iso-8859-2";
      aliases["iso8859-2"] = "iso-8859-2";
      aliases["iso88592"] = "iso-8859-2";
      aliases["iso_8859-2"] = "iso-8859-2";
      aliases["iso_8859-2:1987"] = "iso-8859-2";
      aliases["l2"] = "iso-8859-2";
      aliases["latin2"] = "iso-8859-2";
      aliases["csisolatin3"] = "iso-8859-3";
      aliases["iso-8859-3"] = "iso-8859-3";
      aliases["iso-ir-109"] = "iso-8859-3";
      aliases["iso8859-3"] = "iso-8859-3";
      aliases["iso88593"] = "iso-8859-3";
      aliases["iso_8859-3"] = "iso-8859-3";
      aliases["iso_8859-3:1988"] = "iso-8859-3";
      aliases["l3"] = "iso-8859-3";
      aliases["latin3"] = "iso-8859-3";
      aliases["csisolatin4"] = "iso-8859-4";
      aliases["iso-8859-4"] = "iso-8859-4";
      aliases["iso-ir-110"] = "iso-8859-4";
      aliases["iso8859-4"] = "iso-8859-4";
      aliases["iso88594"] = "iso-8859-4";
      aliases["iso_8859-4"] = "iso-8859-4";
      aliases["iso_8859-4:1988"] = "iso-8859-4";
      aliases["l4"] = "iso-8859-4";
      aliases["latin4"] = "iso-8859-4";
      aliases["csisolatincyrillic"] = "iso-8859-5";
      aliases["cyrillic"] = "iso-8859-5";
      aliases["iso-8859-5"] = "iso-8859-5";
      aliases["iso-ir-144"] = "iso-8859-5";
      aliases["iso8859-5"] = "iso-8859-5";
      aliases["iso88595"] = "iso-8859-5";
      aliases["iso_8859-5"] = "iso-8859-5";
      aliases["iso_8859-5:1988"] = "iso-8859-5";
      aliases["arabic"] = "iso-8859-6";
      aliases["asmo-708"] = "iso-8859-6";
      aliases["csiso88596e"] = "iso-8859-6";
      aliases["csiso88596i"] = "iso-8859-6";
      aliases["csisolatinarabic"] = "iso-8859-6";
      aliases["ecma-114"] = "iso-8859-6";
      aliases["iso-8859-6"] = "iso-8859-6";
      aliases["iso-8859-6-e"] = "iso-8859-6";
      aliases["iso-8859-6-i"] = "iso-8859-6";
      aliases["iso-ir-127"] = "iso-8859-6";
      aliases["iso8859-6"] = "iso-8859-6";
      aliases["iso88596"] = "iso-8859-6";
      aliases["iso_8859-6"] = "iso-8859-6";
      aliases["iso_8859-6:1987"] = "iso-8859-6";
      aliases["csisolatingreek"] = "iso-8859-7";
      aliases["ecma-118"] = "iso-8859-7";
      aliases["elot_928"] = "iso-8859-7";
      aliases["greek"] = "iso-8859-7";
      aliases["greek8"] = "iso-8859-7";
      aliases["iso-8859-7"] = "iso-8859-7";
      aliases["iso-ir-126"] = "iso-8859-7";
      aliases["iso8859-7"] = "iso-8859-7";
      aliases["iso88597"] = "iso-8859-7";
      aliases["iso_8859-7"] = "iso-8859-7";
      aliases["iso_8859-7:1987"] = "iso-8859-7";
      aliases["sun_eu_greek"] = "iso-8859-7";
      aliases["csiso88598e"] = "iso-8859-8";
      aliases["csisolatinhebrew"] = "iso-8859-8";
      aliases["hebrew"] = "iso-8859-8";
      aliases["iso-8859-8"] = "iso-8859-8";
      aliases["iso-8859-8-e"] = "iso-8859-8";
      aliases["iso-ir-138"] = "iso-8859-8";
      aliases["iso8859-8"] = "iso-8859-8";
      aliases["iso88598"] = "iso-8859-8";
      aliases["iso_8859-8"] = "iso-8859-8";
      aliases["iso_8859-8:1988"] = "iso-8859-8";
      aliases["visual"] = "iso-8859-8";
      aliases["csiso88598i"] = "iso-8859-8-i";
      aliases["iso-8859-8-i"] = "iso-8859-8-i";
      aliases["logical"] = "iso-8859-8-i";
      aliases["csisolatin6"] = "iso-8859-10";
      aliases["iso-8859-10"] = "iso-8859-10";
      aliases["iso-ir-157"] = "iso-8859-10";
      aliases["iso8859-10"] = "iso-8859-10";
      aliases["iso885910"] = "iso-8859-10";
      aliases["l6"] = "iso-8859-10";
      aliases["latin6"] = "iso-8859-10";
      aliases["iso-8859-13"] = "iso-8859-13";
      aliases["iso8859-13"] = "iso-8859-13";
      aliases["iso885913"] = "iso-8859-13";
      aliases["iso-8859-14"] = "iso-8859-14";
      aliases["iso8859-14"] = "iso-8859-14";
      aliases["iso885914"] = "iso-8859-14";
      aliases["csisolatin9"] = "iso-8859-15";
      aliases["iso-8859-15"] = "iso-8859-15";
      aliases["iso8859-15"] = "iso-8859-15";
      aliases["iso885915"] = "iso-8859-15";
      aliases["iso_8859-15"] = "iso-8859-15";
      aliases["l9"] = "iso-8859-15";
      aliases["iso-8859-16"] = "iso-8859-16";
      aliases["cskoi8r"] = "koi8-r";
      aliases["koi"] = "koi8-r";
      aliases["koi8"] = "koi8-r";
      aliases["koi8-r"] = "koi8-r";
      aliases["koi8_r"] = "koi8-r";
      aliases["koi8-ru"] = "koi8-u";
      aliases["koi8-u"] = "koi8-u";
      aliases["csmacintosh"] = "macintosh";
      aliases["mac"] = "macintosh";
      aliases["macintosh"] = "macintosh";
      aliases["x-mac-roman"] = "macintosh";
      aliases["dos-874"] = "windows-874";
      aliases["iso-8859-11"] = "windows-874";
      aliases["iso8859-11"] = "windows-874";
      aliases["iso885911"] = "windows-874";
      aliases["tis-620"] = "windows-874";
      aliases["windows-874"] = "windows-874";
      aliases["cp1250"] = "windows-1250";
      aliases["windows-1250"] = "windows-1250";
      aliases["x-cp1250"] = "windows-1250";
      aliases["cp1251"] = "windows-1251";
      aliases["windows-1251"] = "windows-1251";
      aliases["x-cp1251"] = "windows-1251";
      aliases["ansi_x3.4-1968"] = "windows-1252";
      aliases["ascii"] = "windows-1252";
      aliases["cp1252"] = "windows-1252";
      aliases["cp819"] = "windows-1252";
      aliases["csisolatin1"] = "windows-1252";
      aliases["ibm819"] = "windows-1252";
      aliases["iso-8859-1"] = "windows-1252";
      aliases["iso-ir-100"] = "windows-1252";
      aliases["iso8859-1"] = "windows-1252";
      aliases["iso88591"] = "windows-1252";
      aliases["iso_8859-1"] = "windows-1252";
      aliases["iso_8859-1:1987"] = "windows-1252";
      aliases["l1"] = "windows-1252";
      aliases["latin1"] = "windows-1252";
      aliases["us-ascii"] = "windows-1252";
      aliases["windows-1252"] = "windows-1252";
      aliases["x-cp1252"] = "windows-1252";
      aliases["cp1253"] = "windows-1253";
      aliases["windows-1253"] = "windows-1253";
      aliases["x-cp1253"] = "windows-1253";
      aliases["cp1254"] = "windows-1254";
      aliases["csisolatin5"] = "windows-1254";
      aliases["iso-8859-9"] = "windows-1254";
      aliases["iso-ir-148"] = "windows-1254";
      aliases["iso8859-9"] = "windows-1254";
      aliases["iso88599"] = "windows-1254";
      aliases["iso_8859-9"] = "windows-1254";
      aliases["iso_8859-9:1989"] = "windows-1254";
      aliases["l5"] = "windows-1254";
      aliases["latin5"] = "windows-1254";
      aliases["windows-1254"] = "windows-1254";
      aliases["x-cp1254"] = "windows-1254";
      aliases["cp1255"] = "windows-1255";
      aliases["windows-1255"] = "windows-1255";
      aliases["x-cp1255"] = "windows-1255";
      aliases["cp1256"] = "windows-1256";
      aliases["windows-1256"] = "windows-1256";
      aliases["x-cp1256"] = "windows-1256";
      aliases["cp1257"] = "windows-1257";
      aliases["windows-1257"] = "windows-1257";
      aliases["x-cp1257"] = "windows-1257";
      aliases["cp1258"] = "windows-1258";
      aliases["windows-1258"] = "windows-1258";
      aliases["x-cp1258"] = "windows-1258";
      aliases["x-mac-cyrillic"] = "x-mac-cyrillic";
      aliases["x-mac-ukrainian"] = "x-mac-cyrillic";
      aliases["chinese"] = "gbk";
      aliases["csgb2312"] = "gbk";
      aliases["csiso58gb231280"] = "gbk";
      aliases["gb2312"] = "gbk";
      aliases["gb_2312"] = "gbk";
      aliases["gb_2312-80"] = "gbk";
      aliases["gbk"] = "gbk";
      aliases["iso-ir-58"] = "gbk";
      aliases["x-gbk"] = "gbk";
      aliases["gb18030"] = "gb18030";
      aliases["big5"] = "big5";
      aliases["big5-hkscs"] = "big5";
      aliases["cn-big5"] = "big5";
      aliases["csbig5"] = "big5";
      aliases["x-x-big5"] = "big5";
      aliases["cseucpkdfmtjapanese"] = "euc-jp";
      aliases["euc-jp"] = "euc-jp";
      aliases["x-euc-jp"] = "euc-jp";
      aliases["csiso2022jp"] = "iso-2022-jp";
      aliases["iso-2022-jp"] = "iso-2022-jp";
      aliases["csshiftjis"] = "shift_jis";
      aliases["ms932"] = "shift_jis";
      aliases["ms_kanji"] = "shift_jis";
      aliases["shift-jis"] = "shift_jis";
      aliases["shift_jis"] = "shift_jis";
      aliases["sjis"] = "shift_jis";
      aliases["windows-31j"] = "shift_jis";
      aliases["x-sjis"] = "shift_jis";
      aliases["cseuckr"] = "euc-kr";
      aliases["csksc56011987"] = "euc-kr";
      aliases["euc-kr"] = "euc-kr";
      aliases["iso-ir-149"] = "euc-kr";
      aliases["korean"] = "euc-kr";
      aliases["ks_c_5601-1987"] = "euc-kr";
      aliases["ks_c_5601-1989"] = "euc-kr";
      aliases["ksc5601"] = "euc-kr";
      aliases["ksc_5601"] = "euc-kr";
      aliases["windows-949"] = "euc-kr";
      aliases["csiso2022kr"] = "replacement";
      aliases["hz-gb-2312"] = "replacement";
      aliases["iso-2022-cn"] = "replacement";
      aliases["iso-2022-cn-ext"] = "replacement";
      aliases["iso-2022-kr"] = "replacement";
      aliases["utf-16be"] = "utf-16be";
      aliases["utf-16"] = "utf-16le";
      aliases["utf-16le"] = "utf-16le";
      aliases["x-user-defined"] = "x-user-defined";
      return aliases;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string that names a given character
    /// encoding.</param>
    /// <returns>A string object.</returns>
    public static string ResolveAliasForWeb(string name) {
      if (String.IsNullOrEmpty(name)) {
        return String.Empty;
      }
      name = TrimAsciiWhite(name);
      name = DataUtilities.ToLowerCaseAscii(name);
      return charsetAliases.ContainsKey(name) ? charsetAliases[name] :
             String.Empty;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string naming a character encoding. Uses a modified
    /// version of the rules in the Encoding Standard to better conform, in some
    /// cases, to email standards such as MIME, and some additional encodings
    /// may be
    /// supported. For instance, setting this value to true will enable the
    /// &#x22;utf-7&#x22; encoding and change &#x22;us-ascii&#x22; and
    /// &#x22;iso-8859-1&#x22; to a 7 bit encoding and the 8-bit Latin-1
    /// encoding,
    /// respectively, rather than aliases to &#x22;windows-1252&#x22;, as
    /// specified
    /// in the Encoding Standard.</param>
    /// <returns>A string object.</returns>
    public static string ResolveAliasForEmail(string name) {
      if (String.IsNullOrEmpty(name)) {
        return String.Empty;
      }
      name = TrimAsciiWhite(name);
      name = DataUtilities.ToLowerCaseAscii(name);
      if (name.Equals("utf-8") || name.Equals("iso-8859-1")) {
        return name;
      }
      if (name.Equals("us-ascii") || name.Equals("ascii") ||
        name.Equals("ansi_x3.4-1968")) {
        // DEVIATION: "ascii" is not an IANA-registered name,
        // but occurs quite frequently
        return "us-ascii";
      }
      if (charsetAliases.ContainsKey(name)) {
        return charsetAliases[name];
      }
      if (name.Equals("iso-2022-jp-2")) {
        // NOTE: Treat as the same as iso-2022-jp
        return "iso-2022-jp";
      }
      if (name.Equals("utf-7") || name.Equals("unicode-1-1-utf-7")) {
        return "utf-7";
      }
      if (name.Length > 9 && name.Substring(0, 9).Equals("iso-8859-")) {
        // NOTE: For conformance to MIME, treat unknown iso-8859-* encodings
        // as ASCII
        return "us-ascii";
      }
      return String.Empty;
    }

    /// <summary>Reads bytes from a data source and converts the bytes to a text
    /// string in a given encoding. <para>In the .NET implementation, this
    /// method is
    /// implemented as an extension method to any object implementing
    /// ICharacterEncoding and can be called as follows:
    /// "encoding.DecodeString(transform)". If the object's class already has a
    /// DecodeString method with the same parameters, that method takes
    /// precedence
    /// over this extension method.</para>
    /// </summary>
    /// <param name='encoding'>An object that implements a given character
    /// encoding.
    /// Any bytes that can&#x27;t be decoded are converted to the replacement
    /// character (U + FFFD).</param>
    /// <param name='transform'>An object that implements a byte stream.</param>
    /// <returns>The converted string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='encoding'/> or <paramref name='transform'/> is null.</exception>
    public static string DecodeString(
     this ICharacterEncoding encoding,
     ITransform transform) {
      if (encoding == null) {
  throw new ArgumentNullException("encoding");
}
      if (transform == null) {
  throw new ArgumentNullException("transform");
}
      return EncoderHelper.InputToString(
         GetDecoderInput(encoding, transform));
    }

    /// <summary>Converts a character encoding into a character input stream.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension
    /// method to any object implementing ICharacterEncoding and can be called
    /// as
    /// follows: "encoding.GetDecoderInput(transform)". If the object's class
    /// already has a GetDecoderInput method with the same parameters, that
    /// method
    /// takes precedence over this extension method.</para>
    /// </summary>
    /// <param name='encoding'>Encoding that exposes a decoder to be converted
    /// into
    /// a character input stream. If the decoder returns -2 (indicating a decode
    /// error), the character input stream handles the error by outputting a
    /// replacement character in its place.</param>
    /// <param name='stream'>Byte stream to convert into Unicode
    /// characters.</param>
    /// <returns>An ICharacterInput object.</returns>
    public static ICharacterInput GetDecoderInput(
      this ICharacterEncoding encoding,
      ITransform stream) {
        return new DecoderToInputClass(
          encoding.GetDecoder(),
          stream);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string naming a character encoding.</param>
    /// <returns>An ICharacterEncoding object.</returns>
    public static ICharacterEncoding GetEncoding(string name) {
      return GetEncoding(name, false);
    }

    /// <summary>Returns a character encoding from the given name.</summary>
    /// <param name='name'>A string naming a character encoding.</param>
    /// <param name='forEmail'>If false, uses the encoding resolution rules in
    /// the
    /// Encoding Standard. If true, uses modified rules as described in the
    /// ResolveAliasForEmail method.</param>
    /// <returns>An object that enables encoding and decoding text in the given
    /// character encoding.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='name'/> is null.</exception>
    public static ICharacterEncoding GetEncoding(string name, bool forEmail) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      if (String.IsNullOrEmpty(name)) {
        return null;
      }
      name = forEmail ? ResolveAliasForEmail(name) :
        ResolveAliasForWeb(name);
      if (name.Equals("utf-8")) {
        return UTF8;
      }
      if (name.Equals("us-ascii")) {
        return (ICharacterEncoding)(new EncodingAscii());
      }
      if (name.Equals("iso-8859-1")) {
        return (ICharacterEncoding)(new EncodingLatinOne());
      }
      if (name.Equals("utf-7") || name.Equals("unicode-1-1-utf-7")) {
        return (ICharacterEncoding)(new EncodingUtf7());
      }
      if (name.Equals("windows-1252")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 8218,
    402, 8222, 8230, 8224,
    8225, 710, 8240, 352, 8249, 338, 141, 381, 143, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 732, 8482, 353, 8250, 339, 157, 382, 376, 160, 161, 162,
    163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177,
    178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192,
    193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
    208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222,
    223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237,
    238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252,
    253, 254, 255 });
      }

      if (name.Equals("ibm866")) {
      return (ICharacterEncoding)new EncodingSingleByte(new[] { 1040, 1041,
        1042, 1043, 1044, 1045,
    1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057,
    1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069,
    1070, 1071, 1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081,
    1082, 1083, 1084, 1085, 1086, 1087, 9617, 9618, 9619, 9474, 9508, 9569,
    9570, 9558, 9557, 9571, 9553, 9559, 9565, 9564, 9563, 9488, 9492, 9524,
    9516, 9500, 9472, 9532, 9566, 9567, 9562, 9556, 9577, 9574, 9568, 9552,
    9580, 9575, 9576, 9572, 9573, 9561, 9560, 9554, 9555, 9579, 9578, 9496,
    9484, 9608, 9604, 9612, 9616, 9600, 1088, 1089, 1090, 1091, 1092, 1093,
    1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103, 1025, 1105,
    1028, 1108, 1031, 1111, 1038, 1118, 176, 8729, 183, 8730, 8470, 164, 9632,
    160 });
      }
      if (name.Equals("iso-8859-10")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 274, 290, 298, 296,
    310, 167, 315, 272, 352, 358, 381, 173, 362, 330, 176, 261, 275, 291, 299,
    297, 311, 183, 316, 273, 353, 359, 382, 8213, 363, 331, 256, 193, 194, 195,
    196, 197, 198, 302, 268, 201, 280, 203, 278, 205, 206, 207, 208, 325, 332,
    211, 212, 213, 214, 360, 216, 370, 218, 219, 220, 221, 222, 223, 257, 225,
    226, 227, 228, 229, 230, 303, 269, 233, 281, 235, 279, 237, 238, 239, 240,
    326, 333, 243, 244, 245, 246, 361, 248, 371, 250, 251, 252, 253, 254, 312
    });
      }
      if (name.Equals("iso-8859-13")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 8221, 162, 163, 164, 8222,
    166, 167, 216, 169, 342, 171, 172, 173, 174, 198, 176, 177, 178, 179, 8220,
    181, 182, 183, 248, 185, 343, 187, 188, 189, 190, 230, 260, 302, 256, 262,
    196, 197, 280, 274, 268, 201, 377, 278, 290, 310, 298, 315, 352, 323, 325,
    211, 332, 213, 214, 215, 370, 321, 346, 362, 220, 379, 381, 223, 261, 303,
    257, 263, 228, 229, 281, 275, 269, 233, 378, 279, 291, 311, 299, 316, 353,
    324, 326, 243, 333, 245, 246, 247, 371, 322, 347, 363, 252, 380, 382, 8217
    });
      }
      if (name.Equals("iso-8859-14")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 7682, 7683, 163, 266, 267,
    7690, 167, 7808, 169, 7810, 7691, 7922, 173, 174, 376, 7710, 7711, 288, 289,
    7744, 7745, 182, 7766, 7809, 7767, 7811, 7776, 7923, 7812, 7813, 7777, 192,
    193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
    372, 209, 210, 211, 212, 213, 214, 7786, 216, 217, 218, 219, 220, 221, 374,
    223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237,
    238, 239, 373, 241, 242, 243, 244, 245, 246, 7787, 248, 249, 250, 251, 252,
    253, 375, 255 });
      }
      if (name.Equals("iso-8859-15")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 8364, 165,
    352, 167, 353, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 381,
    181, 182, 183, 382, 185, 186, 187, 338, 339, 376, 191, 192, 193, 194, 195,
    196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210,
    211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225,
    226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240,
    241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
    });
      }
      if (name.Equals("iso-8859-16")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 261, 321, 8364, 8222,
    352, 167, 353, 169, 536, 171, 377, 173, 378, 379, 176, 177, 268, 322, 381,
    8221, 182, 183, 382, 269, 537, 187, 338, 339, 376, 380, 192, 193, 194, 258,
    196, 262, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 272, 323, 210,
    211, 212, 336, 214, 346, 368, 217, 218, 219, 220, 280, 538, 223, 224, 225,
    226, 259, 228, 263, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 273,
    324, 242, 243, 244, 337, 246, 347, 369, 249, 250, 251, 252, 281, 539, 255
    });
      }
      if (name.Equals("iso-8859-2")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 728, 321, 164, 317,
    346, 167, 168, 352, 350, 356, 377, 173, 381, 379, 176, 261, 731, 322, 180,
    318, 347, 711, 184, 353, 351, 357, 378, 733, 382, 380, 340, 193, 194, 258,
    196, 313, 262, 199, 268, 201, 280, 203, 282, 205, 206, 270, 272, 323, 327,
    211, 212, 336, 214, 215, 344, 366, 218, 368, 220, 221, 354, 223, 341, 225,
    226, 259, 228, 314, 263, 231, 269, 233, 281, 235, 283, 237, 238, 271, 273,
    324, 328, 243, 244, 337, 246, 247, 345, 367, 250, 369, 252, 253, 355, 729
    });
      }
      if (name.Equals("iso-8859-3")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 294, 728, 163, 164, -2,
    292, 167, 168, 304, 350, 286, 308, 173, -2, 379, 176, 295, 178, 179, 180,
    181, 293, 183, 184, 305, 351, 287, 309, 189, -2, 380, 192, 193, 194,
    -2, 196, 266, 264, 199, 200, 201, 202, 203, 204, 205, 206, 207, -2,
    209, 210, 211, 212, 288, 214, 215, 284, 217, 218, 219, 220, 364, 348, 223,
    224, 225, 226, -2, 228, 267, 265, 231, 232, 233, 234, 235, 236, 237, 238,
    239, -2, 241, 242, 243, 244, 289, 246, 247, 285, 249, 250, 251, 252, 365,
    349, 729 });
      }
      if (name.Equals("iso-8859-4")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 312, 342, 164, 296,
    315, 167, 168, 352, 274, 290, 358, 173, 381, 175, 176, 261, 731, 343, 180,
    297, 316, 711, 184, 353, 275, 291, 359, 330, 382, 331, 256, 193, 194, 195,
    196, 197, 198, 302, 268, 201, 280, 203, 278, 205, 206, 298, 272, 325, 332,
    310, 212, 213, 214, 215, 216, 370, 218, 219, 220, 360, 362, 223, 257, 225,
    226, 227, 228, 229, 230, 303, 269, 233, 281, 235, 279, 237, 238, 299, 273,
    326, 333, 311, 244, 245, 246, 247, 248, 371, 250, 251, 252, 361, 363, 729
    });
      }
      if (name.Equals("iso-8859-5")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 1025, 1026, 1027, 1028,
    1029, 1030, 1031, 1032, 1033, 1034, 1035, 1036, 173, 1038, 1039, 1040, 1041,
    1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053,
    1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065,
    1066, 1067, 1068, 1069, 1070, 1071, 1072, 1073, 1074, 1075, 1076, 1077,
    1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089,
    1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101,
    1102, 1103, 8470, 1105, 1106, 1107, 1108, 1109, 1110, 1111, 1112, 1113,
    1114, 1115, 1116, 167, 1118, 1119 });
      }
      if (name.Equals("iso-8859-6")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, -2, -2, -2, 164,
    -2, -2, -2, -2, -2, -2, -2, 1548, 173, -2, -2,
    -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
    1563, -2, -2, -2, 1567, -2, 1569, 1570, 1571, 1572, 1573, 1574,
    1575, 1576, 1577, 1578, 1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586,
    1587, 1588, 1589, 1590, 1591, 1592, 1593, 1594, -2, -2, -2, -2,
    -2, 1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608, 1609, 1610,
    1611, 1612, 1613, 1614, 1615, 1616, 1617, 1618, -2, -2, -2, -2,
    -2, -2, -2, -2, -2, -2, -2, -2, -2 });
      }
      if (name.Equals("iso-8859-7")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 8216, 8217, 163, 8364,
    8367, 166, 167, 168, 169, 890, 171, 172, 173, -2, 8213, 176, 177, 178,
    179, 900, 901, 902, 183, 904, 905, 906, 187, 908, 189, 910, 911, 912, 913,
    914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928,
    929, -2, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943,
    944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958,
    959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973,
    974, -2 });
      }
      if (name.Equals("iso-8859-8") || name.Equals("iso-8859-8-i")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 128, 129, 130, 131,
    132, 133, 134, 135,
    136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160, -2, 162, 163, 164, 165,
    166, 167, 168, 169, 215, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180,
    181, 182, 183, 184, 185, 247, 187, 188, 189, 190, -2, -2, -2,
    -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
    -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2,
    -2, -2, -2, -2, -2, -2, -2, 8215, 1488, 1489, 1490,
    1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502,
    1503, 1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514,
    -2, -2, 8206, 8207, -2 });
      }
      if (name.Equals("koi8-r")) {
      return (ICharacterEncoding)new EncodingSingleByte(new[] { 9472, 9474,
        9484, 9488, 9492, 9496,
    9500, 9508, 9516, 9524, 9532, 9600, 9604, 9608, 9612, 9616, 9617, 9618,
    9619, 8992, 9632, 8729, 8730, 8776, 8804, 8805, 160, 8993, 176, 178, 183,
    247, 9552, 9553, 9554, 1105, 9555, 9556, 9557, 9558, 9559, 9560, 9561, 9562,
    9563, 9564, 9565, 9566, 9567, 9568, 9569, 1025, 9570, 9571, 9572, 9573,
    9574, 9575, 9576, 9577, 9578, 9579, 9580, 169, 1102, 1072, 1073, 1094, 1076,
    1077, 1092, 1075, 1093, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087,
    1103, 1088, 1089, 1090, 1091, 1078, 1074, 1100, 1099, 1079, 1096, 1101,
    1097, 1095, 1098, 1070, 1040, 1041, 1062, 1044, 1045, 1060, 1043, 1061,
    1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1071, 1056, 1057, 1058,
    1059, 1046, 1042, 1068, 1067, 1047, 1064, 1069, 1065, 1063, 1066 });
      }
      if (name.Equals("koi8-u")) {
      return (ICharacterEncoding)new EncodingSingleByte(new[] { 9472, 9474,
        9484, 9488, 9492, 9496,
    9500, 9508, 9516, 9524, 9532, 9600, 9604, 9608, 9612, 9616, 9617, 9618,
    9619, 8992, 9632, 8729, 8730, 8776, 8804, 8805, 160, 8993, 176, 178, 183,
    247, 9552, 9553, 9554, 1105, 1108, 9556, 1110, 1111, 9559, 9560, 9561, 9562,
    9563, 1169, 1118, 9566, 9567, 9568, 9569, 1025, 1028, 9571, 1030, 1031,
    9574, 9575, 9576, 9577, 9578, 1168, 1038, 169, 1102, 1072, 1073, 1094, 1076,
    1077, 1092, 1075, 1093, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087,
    1103, 1088, 1089, 1090, 1091, 1078, 1074, 1100, 1099, 1079, 1096, 1101,
    1097, 1095, 1098, 1070, 1040, 1041, 1062, 1044, 1045, 1060, 1043, 1061,
    1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1071, 1056, 1057, 1058,
    1059, 1046, 1042, 1068, 1067, 1047, 1064, 1069, 1065, 1063, 1066 });
      }
      if (name.Equals("macintosh")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 196, 197, 199, 201,
    209, 214, 220, 225,
    224, 226, 228, 227, 229, 231, 233, 232, 234, 235, 237, 236, 238, 239, 241,
    243, 242, 244, 246, 245, 250, 249, 251, 252, 8224, 176, 162, 163, 167, 8226,
    182, 223, 174, 169, 8482, 180, 168, 8800, 198, 216, 8734, 177, 8804, 8805,
    165, 181, 8706, 8721, 8719, 960, 8747, 170, 186, 937, 230, 248, 191, 161,
    172, 8730, 402, 8776, 8710, 171, 187, 8230, 160, 192, 195, 213, 338, 339,
    8211, 8212, 8220, 8221, 8216, 8217, 247, 9674, 255, 376, 8260, 8364, 8249,
    8250, 64257, 64258, 8225, 183, 8218, 8222, 8240, 194, 202, 193, 203, 200,
    205, 206, 207, 204, 211, 212, 63743, 210, 218, 219, 217, 305, 710, 732, 175,
    728, 729, 730, 184, 733, 731, 711 });
      }
      if (name.Equals("windows-1250")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 8218,
    131, 8222, 8230, 8224,
    8225, 136, 8240, 352, 8249, 346, 356, 381, 377, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 152, 8482, 353, 8250, 347, 357, 382, 378, 160, 711, 728,
    321, 164, 260, 166, 167, 168, 169, 350, 171, 172, 173, 174, 379, 176, 177,
    731, 322, 180, 181, 182, 183, 184, 261, 351, 187, 317, 733, 318, 380, 340,
    193, 194, 258, 196, 313, 262, 199, 268, 201, 280, 203, 282, 205, 206, 270,
    272, 323, 327, 211, 212, 336, 214, 215, 344, 366, 218, 368, 220, 221, 354,
    223, 341, 225, 226, 259, 228, 314, 263, 231, 269, 233, 281, 235, 283, 237,
    238, 271, 273, 324, 328, 243, 244, 337, 246, 247, 345, 367, 250, 369, 252,
    253, 355, 729 });
      }
      if (name.Equals("windows-1251")) {
      return (ICharacterEncoding)new EncodingSingleByte(new[] { 1026, 1027,
        8218, 1107, 8222, 8230,
    8224, 8225, 8364, 8240, 1033, 8249, 1034, 1036, 1035, 1039, 1106, 8216,
    8217, 8220, 8221, 8226, 8211, 8212, 152, 8482, 1113, 8250, 1114, 1116, 1115,
    1119, 160, 1038, 1118, 1032, 164, 1168, 166, 167, 1025, 169, 1028, 171, 172,
    173, 174, 1031, 176, 177, 1030, 1110, 1169, 181, 182, 183, 1105, 8470, 1108,
    187, 1112, 1029, 1109, 1111, 1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047,
    1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057, 1058, 1059,
    1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071,
    1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083,
    1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094, 1095,
    1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103 });
      }
      if (name.Equals("windows-1253")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 8218,
    402, 8222, 8230, 8224,
    8225, 136, 8240, 138, 8249, 140, 141, 142, 143, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 152, 8482, 154, 8250, 156, 157, 158, 159, 160, 901, 902,
    163, 164, 165, 166, 167, 168, 169, -2, 171, 172, 173, 174, 8213, 176,
    177, 178, 179, 900, 181, 182, 183, 904, 905, 906, 187, 908, 189, 910, 911,
    912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926,
    927, 928, 929, -2, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941,
    942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956,
    957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971,
    972, 973, 974, -2 });
      }
      if (name.Equals("windows-1254")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 8218,
    402, 8222, 8230, 8224,
    8225, 710, 8240, 352, 8249, 338, 141, 142, 143, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 732, 8482, 353, 8250, 339, 157, 158, 376, 160, 161, 162,
    163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177,
    178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192,
    193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207,
    286, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 304, 350,
    223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237,
    238, 239, 287, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252,
    305, 351, 255 });
      }
      if (name.Equals("windows-1255")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 8218,
    402, 8222, 8230, 8224,
    8225, 710, 8240, 138, 8249, 140, 141, 142, 143, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 732, 8482, 154, 8250, 156, 157, 158, 159, 160, 161, 162,
    163, 8362, 165, 166, 167, 168, 169, 215, 171, 172, 173, 174, 175, 176, 177,
    178, 179, 180, 181, 182, 183, 184, 185, 247, 187, 188, 189, 190, 191, 1456,
    1457, 1458, 1459, 1460, 1461, 1462, 1463, 1464, 1465, -2, 1467, 1468,
    1469, 1470, 1471, 1472, 1473, 1474, 1475, 1520, 1521, 1522, 1523, 1524,
    -2, -2, -2, -2, -2, -2, -2, 1488, 1489, 1490, 1491,
    1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503,
    1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, -2,
    -2, 8206, 8207, -2 });
      }
      if (name.Equals("windows-1256")) {
 return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 1662, 8218,
   402, 8222, 8230, 8224,
    8225, 710, 8240, 1657, 8249, 338, 1670, 1688, 1672, 1711, 8216, 8217, 8220,
    8221, 8226, 8211, 8212, 1705, 8482, 1681, 8250, 339, 8204, 8205, 1722, 160,
    1548, 162, 163, 164, 165, 166, 167, 168, 169, 1726, 171, 172, 173, 174, 175,
    176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 1563, 187, 188, 189, 190,
    1567, 1729, 1569, 1570, 1571, 1572, 1573, 1574, 1575, 1576, 1577, 1578,
    1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587, 1588, 1589, 1590, 215,
    1591, 1592, 1593, 1594, 1600, 1601, 1602, 1603, 224, 1604, 226, 1605, 1606,
    1607, 1608, 231, 232, 233, 234, 235, 1609, 1610, 238, 239, 1611, 1612, 1613,
    1614, 244, 1615, 1616, 247, 1617, 249, 1618, 251, 252, 8206, 8207, 1746 });
      }
      if (name.Equals("windows-1257")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 8218,
    131, 8222, 8230, 8224,
    8225, 136, 8240, 138, 8249, 140, 168, 711, 184, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 152, 8482, 154, 8250, 156, 175, 731, 159, 160, -2, 162,
    163, 164, -2, 166, 167, 216, 169, 342, 171, 172, 173, 174, 198, 176, 177,
    178, 179, 180, 181, 182, 183, 248, 185, 343, 187, 188, 189, 190, 230, 260,
    302, 256, 262, 196, 197, 280, 274, 268, 201, 377, 278, 290, 310, 298, 315,
    352, 323, 325, 211, 332, 213, 214, 215, 370, 321, 346, 362, 220, 379, 381,
    223, 261, 303, 257, 263, 228, 229, 281, 275, 269, 233, 378, 279, 291, 311,
    299, 316, 353, 324, 326, 243, 333, 245, 246, 247, 371, 322, 347, 363, 252,
    380, 382, 729 });
      }
      if (name.Equals("windows-1258")) {
  return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 8218,
    402, 8222, 8230, 8224,
    8225, 710, 8240, 138, 8249, 338, 141, 142, 143, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 732, 8482, 154, 8250, 339, 157, 158, 376, 160, 161, 162,
    163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177,
    178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192,
    193, 194, 258, 196, 197, 198, 199, 200, 201, 202, 203, 768, 205, 206, 207,
    272, 209, 777, 211, 212, 416, 214, 215, 216, 217, 218, 219, 220, 431, 771,
    223, 224, 225, 226, 259, 228, 229, 230, 231, 232, 233, 234, 235, 769, 237,
    238, 239, 273, 241, 803, 243, 244, 417, 246, 247, 248, 249, 250, 251, 252,
    432, 8363, 255 });
      }
      if (name.Equals("windows-874")) {
     return (ICharacterEncoding)new EncodingSingleByte(new[] { 8364, 129, 130,
       131, 132, 8230, 134,
    135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 8216, 8217, 8220, 8221,
    8226, 8211, 8212, 152, 153, 154, 155, 156, 157, 158, 159, 160, 3585, 3586,
    3587, 3588, 3589, 3590, 3591, 3592, 3593, 3594, 3595, 3596, 3597, 3598,
    3599, 3600, 3601, 3602, 3603, 3604, 3605, 3606, 3607, 3608, 3609, 3610,
    3611, 3612, 3613, 3614, 3615, 3616, 3617, 3618, 3619, 3620, 3621, 3622,
    3623, 3624, 3625, 3626, 3627, 3628, 3629, 3630, 3631, 3632, 3633, 3634,
    3635, 3636, 3637, 3638, 3639, 3640, 3641, 3642, -2, -2, -2, -2,
    3647, 3648, 3649, 3650, 3651, 3652, 3653, 3654, 3655, 3656, 3657, 3658,
    3659, 3660, 3661, 3662, 3663, 3664, 3665, 3666, 3667, 3668, 3669, 3670,
    3671, 3672, 3673, 3674, 3675, -2, -2, -2, -2 });
      }
      if (name.Equals("x-mac-cyrillic")) {
      return (ICharacterEncoding)new EncodingSingleByte(new[] { 1040, 1041,
        1042, 1043, 1044, 1045,
    1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057,
    1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069,
    1070, 1071, 8224, 176, 1168, 163, 167, 8226, 182, 1030, 174, 169, 8482,
    1026, 1106, 8800, 1027, 1107, 8734, 177, 8804, 8805, 1110, 181, 1169, 1032,
    1028, 1108, 1031, 1111, 1033, 1113, 1034, 1114, 1112, 1029, 172, 8730, 402,
    8776, 8710, 171, 187, 8230, 160, 1035, 1115, 1036, 1116, 1109, 8211, 8212,
    8220, 8221, 8216, 8217, 247, 8222, 1038, 1118, 1039, 1119, 8470, 1025, 1105,
    1103, 1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082,
    1083, 1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094,
    1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 8364 });
      } else if (name.Equals("euc-jp")) {
        return (ICharacterEncoding)(new EncodingEUCJP());
      } else if (name.Equals("euc-kr")) {
        return (ICharacterEncoding)(new EncodingKoreanEUC());
      } else if (name.Equals("big5")) {
        return (ICharacterEncoding)(new EncodingBig5());
      } else if (name.Equals("shift_jis")) {
        return (ICharacterEncoding)(new EncodingShiftJIS());
      } else if (name.Equals("x-user-defined")) {
        return (ICharacterEncoding)(new EncodingXUserDefined());
      } else if (name.Equals("gbk")) {
        return (ICharacterEncoding)(new EncodingGBK());
      } else if (name.Equals("gb18030")) {
        return (ICharacterEncoding)(new EncodingGB18030());
      } else if (name.Equals("utf-16le")) {
        return (ICharacterEncoding)(new EncodingUtf16());
      } else if (name.Equals("utf-16be")) {
        return (ICharacterEncoding)(new EncodingUtf16BE());
      }
      return name.Equals("iso-2022-jp") ? (ICharacterEncoding)(new
        EncodingISO2022JP()) :
        (name.Equals("replacement") ? (ICharacterEncoding)(new
          EncodingReplacement()) : null);
    }
  }
}
