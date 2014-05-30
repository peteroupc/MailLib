/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Text;

using PeterO;

namespace PeterO.Mail {
  internal static class Charsets
  {
    public static readonly ICharset Ascii = new AsciiEncoding();
    public static readonly ICharset Utf8 = new Utf8Encoding();

    private static IDictionary<string, string> charsetAliases = CreateAliasMap();

    private static IDictionary<string, string> CreateAliasMap() {
      var charsetAliases = new Dictionary<string, string>();
      charsetAliases["unicode-1-1-utf-8"] = "utf-8";
      charsetAliases["utf-8"] = "utf-8";
      charsetAliases["utf8"] = "utf-8";
      charsetAliases["866"] = "ibm866";
      charsetAliases["cp866"] = "ibm866";
      charsetAliases["csibm866"] = "ibm866";
      charsetAliases["ibm866"] = "ibm866";
      charsetAliases["csisolatin2"] = "iso-8859-2";
      charsetAliases["iso-8859-2"] = "iso-8859-2";
      charsetAliases["iso-ir-101"] = "iso-8859-2";
      charsetAliases["iso8859-2"] = "iso-8859-2";
      charsetAliases["iso88592"] = "iso-8859-2";
      charsetAliases["iso_8859-2"] = "iso-8859-2";
      charsetAliases["iso_8859-2:1987"] = "iso-8859-2";
      charsetAliases["l2"] = "iso-8859-2";
      charsetAliases["latin2"] = "iso-8859-2";
      charsetAliases["csisolatin3"] = "iso-8859-3";
      charsetAliases["iso-8859-3"] = "iso-8859-3";
      charsetAliases["iso-ir-109"] = "iso-8859-3";
      charsetAliases["iso8859-3"] = "iso-8859-3";
      charsetAliases["iso88593"] = "iso-8859-3";
      charsetAliases["iso_8859-3"] = "iso-8859-3";
      charsetAliases["iso_8859-3:1988"] = "iso-8859-3";
      charsetAliases["l3"] = "iso-8859-3";
      charsetAliases["latin3"] = "iso-8859-3";
      charsetAliases["csisolatin4"] = "iso-8859-4";
      charsetAliases["iso-8859-4"] = "iso-8859-4";
      charsetAliases["iso-ir-110"] = "iso-8859-4";
      charsetAliases["iso8859-4"] = "iso-8859-4";
      charsetAliases["iso88594"] = "iso-8859-4";
      charsetAliases["iso_8859-4"] = "iso-8859-4";
      charsetAliases["iso_8859-4:1988"] = "iso-8859-4";
      charsetAliases["l4"] = "iso-8859-4";
      charsetAliases["latin4"] = "iso-8859-4";
      charsetAliases["csisolatincyrillic"] = "iso-8859-5";
      charsetAliases["cyrillic"] = "iso-8859-5";
      charsetAliases["iso-8859-5"] = "iso-8859-5";
      charsetAliases["iso-ir-144"] = "iso-8859-5";
      charsetAliases["iso8859-5"] = "iso-8859-5";
      charsetAliases["iso88595"] = "iso-8859-5";
      charsetAliases["iso_8859-5"] = "iso-8859-5";
      charsetAliases["iso_8859-5:1988"] = "iso-8859-5";
      charsetAliases["arabic"] = "iso-8859-6";
      charsetAliases["asmo-708"] = "iso-8859-6";
      charsetAliases["csiso88596e"] = "iso-8859-6";
      charsetAliases["csiso88596i"] = "iso-8859-6";
      charsetAliases["csisolatinarabic"] = "iso-8859-6";
      charsetAliases["ecma-114"] = "iso-8859-6";
      charsetAliases["iso-8859-6"] = "iso-8859-6";
      charsetAliases["iso-8859-6-e"] = "iso-8859-6";
      charsetAliases["iso-8859-6-i"] = "iso-8859-6";
      charsetAliases["iso-ir-127"] = "iso-8859-6";
      charsetAliases["iso8859-6"] = "iso-8859-6";
      charsetAliases["iso88596"] = "iso-8859-6";
      charsetAliases["iso_8859-6"] = "iso-8859-6";
      charsetAliases["iso_8859-6:1987"] = "iso-8859-6";
      charsetAliases["csisolatingreek"] = "iso-8859-7";
      charsetAliases["ecma-118"] = "iso-8859-7";
      charsetAliases["elot_928"] = "iso-8859-7";
      charsetAliases["greek"] = "iso-8859-7";
      charsetAliases["greek8"] = "iso-8859-7";
      charsetAliases["iso-8859-7"] = "iso-8859-7";
      charsetAliases["iso-ir-126"] = "iso-8859-7";
      charsetAliases["iso8859-7"] = "iso-8859-7";
      charsetAliases["iso88597"] = "iso-8859-7";
      charsetAliases["iso_8859-7"] = "iso-8859-7";
      charsetAliases["iso_8859-7:1987"] = "iso-8859-7";
      charsetAliases["sun_eu_greek"] = "iso-8859-7";
      charsetAliases["csiso88598e"] = "iso-8859-8";
      charsetAliases["csisolatinhebrew"] = "iso-8859-8";
      charsetAliases["hebrew"] = "iso-8859-8";
      charsetAliases["iso-8859-8"] = "iso-8859-8";
      charsetAliases["iso-8859-8-e"] = "iso-8859-8";
      charsetAliases["iso-ir-138"] = "iso-8859-8";
      charsetAliases["iso8859-8"] = "iso-8859-8";
      charsetAliases["iso88598"] = "iso-8859-8";
      charsetAliases["iso_8859-8"] = "iso-8859-8";
      charsetAliases["iso_8859-8:1988"] = "iso-8859-8";
      charsetAliases["visual"] = "iso-8859-8";
      charsetAliases["csiso88598i"] = "iso-8859-8-i";
      charsetAliases["iso-8859-8-i"] = "iso-8859-8-i";
      charsetAliases["logical"] = "iso-8859-8-i";
      charsetAliases["csisolatin6"] = "iso-8859-10";
      charsetAliases["iso-8859-10"] = "iso-8859-10";
      charsetAliases["iso-ir-157"] = "iso-8859-10";
      charsetAliases["iso8859-10"] = "iso-8859-10";
      charsetAliases["iso885910"] = "iso-8859-10";
      charsetAliases["l6"] = "iso-8859-10";
      charsetAliases["latin6"] = "iso-8859-10";
      charsetAliases["iso-8859-13"] = "iso-8859-13";
      charsetAliases["iso8859-13"] = "iso-8859-13";
      charsetAliases["iso885913"] = "iso-8859-13";
      charsetAliases["iso-8859-14"] = "iso-8859-14";
      charsetAliases["iso8859-14"] = "iso-8859-14";
      charsetAliases["iso885914"] = "iso-8859-14";
      charsetAliases["csisolatin9"] = "iso-8859-15";
      charsetAliases["iso-8859-15"] = "iso-8859-15";
      charsetAliases["iso8859-15"] = "iso-8859-15";
      charsetAliases["iso885915"] = "iso-8859-15";
      charsetAliases["iso_8859-15"] = "iso-8859-15";
      charsetAliases["l9"] = "iso-8859-15";
      charsetAliases["iso-8859-16"] = "iso-8859-16";
      charsetAliases["cskoi8r"] = "koi8-r";
      charsetAliases["koi"] = "koi8-r";
      charsetAliases["koi8"] = "koi8-r";
      charsetAliases["koi8-r"] = "koi8-r";
      charsetAliases["koi8_r"] = "koi8-r";
      charsetAliases["koi8-u"] = "koi8-u";
      charsetAliases["csmacintosh"] = "macintosh";
      charsetAliases["mac"] = "macintosh";
      charsetAliases["macintosh"] = "macintosh";
      charsetAliases["x-mac-roman"] = "macintosh";
      charsetAliases["dos-874"] = "windows-874";
      charsetAliases["iso-8859-11"] = "windows-874";
      charsetAliases["iso8859-11"] = "windows-874";
      charsetAliases["iso885911"] = "windows-874";
      charsetAliases["tis-620"] = "windows-874";
      charsetAliases["windows-874"] = "windows-874";
      charsetAliases["cp1250"] = "windows-1250";
      charsetAliases["windows-1250"] = "windows-1250";
      charsetAliases["x-cp1250"] = "windows-1250";
      charsetAliases["cp1251"] = "windows-1251";
      charsetAliases["windows-1251"] = "windows-1251";
      charsetAliases["x-cp1251"] = "windows-1251";
      charsetAliases["ansi_x3.4-1968"] = "windows-1252";
      charsetAliases["ascii"] = "windows-1252";
      charsetAliases["cp1252"] = "windows-1252";
      charsetAliases["cp819"] = "windows-1252";
      charsetAliases["csisolatin1"] = "windows-1252";
      charsetAliases["ibm819"] = "windows-1252";
      charsetAliases["iso-8859-1"] = "windows-1252";
      charsetAliases["iso-ir-100"] = "windows-1252";
      charsetAliases["iso8859-1"] = "windows-1252";
      charsetAliases["iso88591"] = "windows-1252";
      charsetAliases["iso_8859-1"] = "windows-1252";
      charsetAliases["iso_8859-1:1987"] = "windows-1252";
      charsetAliases["l1"] = "windows-1252";
      charsetAliases["latin1"] = "windows-1252";
      charsetAliases["us-ascii"] = "windows-1252";
      charsetAliases["windows-1252"] = "windows-1252";
      charsetAliases["x-cp1252"] = "windows-1252";
      charsetAliases["cp1253"] = "windows-1253";
      charsetAliases["windows-1253"] = "windows-1253";
      charsetAliases["x-cp1253"] = "windows-1253";
      charsetAliases["cp1254"] = "windows-1254";
      charsetAliases["csisolatin5"] = "windows-1254";
      charsetAliases["iso-8859-9"] = "windows-1254";
      charsetAliases["iso-ir-148"] = "windows-1254";
      charsetAliases["iso8859-9"] = "windows-1254";
      charsetAliases["iso88599"] = "windows-1254";
      charsetAliases["iso_8859-9"] = "windows-1254";
      charsetAliases["iso_8859-9:1989"] = "windows-1254";
      charsetAliases["l5"] = "windows-1254";
      charsetAliases["latin5"] = "windows-1254";
      charsetAliases["windows-1254"] = "windows-1254";
      charsetAliases["x-cp1254"] = "windows-1254";
      charsetAliases["cp1255"] = "windows-1255";
      charsetAliases["windows-1255"] = "windows-1255";
      charsetAliases["x-cp1255"] = "windows-1255";
      charsetAliases["cp1256"] = "windows-1256";
      charsetAliases["windows-1256"] = "windows-1256";
      charsetAliases["x-cp1256"] = "windows-1256";
      charsetAliases["cp1257"] = "windows-1257";
      charsetAliases["windows-1257"] = "windows-1257";
      charsetAliases["x-cp1257"] = "windows-1257";
      charsetAliases["cp1258"] = "windows-1258";
      charsetAliases["windows-1258"] = "windows-1258";
      charsetAliases["x-cp1258"] = "windows-1258";
      charsetAliases["x-mac-cyrillic"] = "x-mac-cyrillic";
      charsetAliases["x-mac-ukrainian"] = "x-mac-cyrillic";
      charsetAliases["chinese"] = "gb18030";
      charsetAliases["csgb2312"] = "gb18030";
      charsetAliases["csiso58gb231280"] = "gb18030";
      charsetAliases["gb18030"] = "gb18030";
      charsetAliases["gb2312"] = "gb18030";
      charsetAliases["gb_2312"] = "gb18030";
      charsetAliases["gb_2312-80"] = "gb18030";
      charsetAliases["gbk"] = "gb18030";
      charsetAliases["iso-ir-58"] = "gb18030";
      charsetAliases["x-gbk"] = "gb18030";
      // charsetAliases["hz-gb-2312"] = "hz-gb-2312";
      charsetAliases["big5"] = "big5";
      charsetAliases["big5-hkscs"] = "big5";
      charsetAliases["cn-big5"] = "big5";
      charsetAliases["csbig5"] = "big5";
      charsetAliases["x-x-big5"] = "big5";
      charsetAliases["cseucpkdfmtjapanese"] = "euc-jp";
      charsetAliases["euc-jp"] = "euc-jp";
      charsetAliases["x-euc-jp"] = "euc-jp";
      charsetAliases["csiso2022jp"] = "iso-2022-jp";
      charsetAliases["iso-2022-jp"] = "iso-2022-jp";
      charsetAliases["csshiftjis"] = "shift_jis";
      charsetAliases["ms_kanji"] = "shift_jis";
      charsetAliases["shift-jis"] = "shift_jis";
      charsetAliases["shift_jis"] = "shift_jis";
      charsetAliases["sjis"] = "shift_jis";
      charsetAliases["windows-31j"] = "shift_jis";
      charsetAliases["x-sjis"] = "shift_jis";
      charsetAliases["cseuckr"] = "euc-kr";
      charsetAliases["csksc56011987"] = "euc-kr";
      charsetAliases["euc-kr"] = "euc-kr";
      charsetAliases["iso-ir-149"] = "euc-kr";
      charsetAliases["korean"] = "euc-kr";
      charsetAliases["ks_c_5601-1987"] = "euc-kr";
      charsetAliases["ks_c_5601-1989"] = "euc-kr";
      charsetAliases["ksc5601"] = "euc-kr";
      charsetAliases["ksc_5601"] = "euc-kr";
      charsetAliases["windows-949"] = "euc-kr";
      charsetAliases["csiso2022kr"] = "replacement";
      charsetAliases["iso-2022-cn"] = "replacement";
      charsetAliases["iso-2022-cn-ext"] = "replacement";
      charsetAliases["iso-2022-kr"] = "replacement";
      charsetAliases["utf-16be"] = "utf-16be";
      charsetAliases["utf-16"] = "utf-16le";
      charsetAliases["utf-16le"] = "utf-16le";
      charsetAliases["x-user-defined"] = "x-user-defined";
      return charsetAliases;
    }

    public static string ResolveAliasForWeb(string name) {
      if (String.IsNullOrEmpty(name)) {
        return String.Empty;
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (charsetAliases.ContainsKey(name)) {
        return charsetAliases[name];
      }
      return String.Empty;
    }

    public static string ResolveAliasForEmail(string name) {
      if (String.IsNullOrEmpty(name)) {
        return String.Empty;
      }
      if (name.Equals("utf-8") || name.Equals("iso-8859-1")) {
        return name;
      }
      if (name.Equals("us-ascii") || name.Equals("ascii") || name.Equals("ansi_x3.4-1968")) {
        // DEVIATION: "ascii" is not an IANA-registered name,
        // but occurs quite frequently
        return "us-ascii";
      }
      name = DataUtilities.ToLowerCaseAscii(name);
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

    public static ICharset GetCharset(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      name = ResolveAliasForEmail(name);
      if (String.IsNullOrEmpty(name)) {
        return null;
      }
      name = DataUtilities.ToLowerCaseAscii(name);
      if (name.Equals("utf-8")) {
        return Utf8;
      }
      if (name.Equals("us-ascii")) {
        return Ascii;
      }
      if (name.Equals("iso-8859-1")) {
        return new LatinOneEncoding();
      }
      if (name.Equals("windows-1252")) {
        return new SingleByteEncoding(
          new int[] { 8364,
          129,
          8218,
          402,
          8222,
          8230,
          8224,
          8225,
          710,
          8240,
          352,
          8249,
          338,
          141,
          381,
          143,
          144,
          8216,
          8217,
          8220,
          8221,
          8226,
          8211,
          8212,
          732,
          8482,
          353,
          8250,
          339,
          157,
          382,
          376,
          160,
          161,
          162,
          163,
          164,
          165,
          166,
          167,
          168,
          169,
          170,
          171,
          172,
          173,
          174,
          175,
          176,
          177,
          178,
          179,
          180,
          181,
          182,
          183,
          184,
          185,
          186,
          187,
          188,
          189,
          190,
          191,
          192,
          193,
          194,
          195,
          196,
          197,
          198,
          199,
          200,
          201,
          202,
          203,
          204,
          205,
          206,
          207,
          208,
          209,
          210,
          211,
          212,
          213,
          214,
          215,
          216,
          217,
          218,
          219,
          220,
          221,
          222,
          223,
          224,
          225,
          226,
          227,
          228,
          229,
          230,
          231,
          232,
          233,
          234,
          235,
          236,
          237,
          238,
          239,
          240,
          241,
          242,
          243,
          244,
          245,
          246,
          247,
          248,
          249,
          250,
          251,
          252,
          253,
          254,
          255 });
      } else if (name.Equals("utf-7") || name.Equals("unicode-1-1-utf-7")) {
        return new Utf7Encoding();
      } else if (name.Equals("iso-8859-10")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 274, 290, 298, 296, 310, 167, 315, 272, 352, 358, 381, 173, 362, 330, 176, 261, 275, 291, 299, 297, 311, 183, 316, 273, 353, 359, 382, 8213, 363, 331, 256, 193, 194, 195, 196, 197, 198, 302, 268, 201, 280, 203, 278, 205, 206, 207, 208, 325, 332, 211, 212, 213, 214, 360, 216, 370, 218, 219, 220, 221, 222, 223, 257, 225, 226, 227, 228, 229, 230, 303, 269, 233, 281, 235, 279, 237, 238, 239, 240, 326, 333, 243, 244, 245, 246, 361, 248, 371, 250, 251, 252, 253, 254, 312 });
      } else if (name.Equals("iso-8859-13")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 8221, 162, 163, 164, 8222, 166, 167, 216, 169, 342, 171, 172, 173, 174, 198, 176, 177, 178, 179, 8220, 181, 182, 183, 248, 185, 343, 187, 188, 189, 190, 230, 260, 302, 256, 262, 196, 197, 280, 274, 268, 201, 377, 278, 290, 310, 298, 315, 352, 323, 325, 211, 332, 213, 214, 215, 370, 321, 346, 362, 220, 379, 381, 223, 261, 303, 257, 263, 228, 229, 281, 275, 269, 233, 378, 279, 291, 311, 299, 316, 353, 324, 326, 243, 333, 245, 246, 247, 371, 322, 347, 363, 252, 380, 382, 8217 });
      } else if (name.Equals("iso-8859-14")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 7682, 7683, 163, 266, 267, 7690, 167, 7808, 169, 7810, 7691, 7922, 173, 174, 376, 7710, 7711, 288, 289, 7744, 7745, 182, 7766, 7809, 7767, 7811, 7776, 7923, 7812, 7813, 7777, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 372, 209, 210, 211, 212, 213, 214, 7786, 216, 217, 218, 219, 220, 221, 374, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 373, 241, 242, 243, 244, 245, 246, 7787, 248, 249, 250, 251, 252, 253, 375, 255 });
      } else if (name.Equals("iso-8859-15")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 8364, 165, 352, 167, 353, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 381, 181, 182, 183, 382, 185, 186, 187, 338, 339, 376, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255 });
      } else if (name.Equals("iso-8859-16")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 261, 321, 8364, 8222, 352, 167, 353, 169, 536, 171, 377, 173, 378, 379, 176, 177, 268, 322, 381, 8221, 182, 183, 382, 269, 537, 187, 338, 339, 376, 380, 192, 193, 194, 258, 196, 262, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 272, 323, 210, 211, 212, 336, 214, 346, 368, 217, 218, 219, 220, 280, 538, 223, 224, 225, 226, 259, 228, 263, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 273, 324, 242, 243, 244, 337, 246, 347, 369, 249, 250, 251, 252, 281, 539, 255 });
      } else if (name.Equals("iso-8859-2")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 728, 321, 164, 317, 346, 167, 168, 352, 350, 356, 377, 173, 381, 379, 176, 261, 731, 322, 180, 318, 347, 711, 184, 353, 351, 357, 378, 733, 382, 380, 340, 193, 194, 258, 196, 313, 262, 199, 268, 201, 280, 203, 282, 205, 206, 270, 272, 323, 327, 211, 212, 336, 214, 215, 344, 366, 218, 368, 220, 221, 354, 223, 341, 225, 226, 259, 228, 314, 263, 231, 269, 233, 281, 235, 283, 237, 238, 271, 273, 324, 328, 243, 244, 337, 246, 247, 345, 367, 250, 369, 252, 253, 355, 729 });
      } else if (name.Equals("iso-8859-3")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 294, 728, 163, 164, 0xfffd, 292, 167, 168, 304, 350, 286, 308, 173, 0xfffd, 379, 176, 295, 178, 179, 180, 181, 293, 183, 184, 305, 351, 287, 309, 189, 0xfffd, 380, 192, 193, 194, 0xfffd, 196, 266, 264, 199, 200, 201, 202, 203, 204, 205, 206, 207, 0xfffd, 209, 210, 211, 212, 288, 214, 215, 284, 217, 218, 219, 220, 364, 348, 223, 224, 225, 226, 0xfffd, 228, 267, 265, 231, 232, 233, 234, 235, 236, 237, 238, 239, 0xfffd, 241, 242, 243, 244, 289, 246, 247, 285, 249, 250, 251, 252, 365, 349, 729 });
      } else if (name.Equals("iso-8859-4")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 260, 312, 342, 164, 296, 315, 167, 168, 352, 274, 290, 358, 173, 381, 175, 176, 261, 731, 343, 180, 297, 316, 711, 184, 353, 275, 291, 359, 330, 382, 331, 256, 193, 194, 195, 196, 197, 198, 302, 268, 201, 280, 203, 278, 205, 206, 298, 272, 325, 332, 310, 212, 213, 214, 215, 216, 370, 218, 219, 220, 360, 362, 223, 257, 225, 226, 227, 228, 229, 230, 303, 269, 233, 281, 235, 279, 237, 238, 299, 273, 326, 333, 311, 244, 245, 246, 247, 248, 371, 250, 251, 252, 361, 363, 729 });
      } else if (name.Equals("iso-8859-5")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033, 1034, 1035, 1036, 173, 1038, 1039, 1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071, 1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103, 8470, 1105, 1106, 1107, 1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 167, 1118, 1119 });
      } else if (name.Equals("iso-8859-6")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 0xfffd, 0xfffd, 0xfffd, 164, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 1548, 173, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 1563, 0xfffd, 0xfffd, 0xfffd, 1567, 0xfffd, 1569, 1570, 1571, 1572, 1573, 1574, 1575, 1576, 1577, 1578, 1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587, 1588, 1589, 1590, 1591, 1592, 1593, 1594, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 1600, 1601, 1602, 1603, 1604, 1605, 1606, 1607, 1608, 1609, 1610, 1611, 1612, 1613, 1614, 1615, 1616, 1617, 1618, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd });
      } else if (name.Equals("iso-8859-7")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 8216, 8217, 163, 8364, 8367, 166, 167, 168, 169, 890, 171, 172, 173, 0xfffd, 8213, 176, 177, 178, 179, 900, 901, 902, 183, 904, 905, 906, 187, 908, 189, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 0xfffd, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 0xfffd });
      } else if (name.Equals("iso-8859-8-i")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 0xfffd, 162, 163, 164, 165, 166, 167, 168, 169, 215, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 247, 187, 188, 189, 190, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 8215, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503, 1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 0xfffd, 0xfffd, 8206, 8207, 0xfffd });
      } else if (name.Equals("iso-8859-8")) {
        return new SingleByteEncoding(new int[] { 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 0xfffd, 162, 163, 164, 165, 166, 167, 168, 169, 215, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 247, 187, 188, 189, 190, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 8215, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503, 1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 0xfffd, 0xfffd, 8206, 8207, 0xfffd });
      } else if (name.Equals("koi8-r")) {
        return new SingleByteEncoding(new int[] { 9472, 9474, 9484, 9488, 9492, 9496, 9500, 9508, 9516, 9524, 9532, 9600, 9604, 9608, 9612, 9616, 9617, 9618, 9619, 8992, 9632, 8729, 8730, 8776, 8804, 8805, 160, 8993, 176, 178, 183, 247, 9552, 9553, 9554, 1105, 9555, 9556, 9557, 9558, 9559, 9560, 9561, 9562, 9563, 9564, 9565, 9566, 9567, 9568, 9569, 1025, 9570, 9571, 9572, 9573, 9574, 9575, 9576, 9577, 9578, 9579, 9580, 169, 1102, 1072, 1073, 1094, 1076, 1077, 1092, 1075, 1093, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1103, 1088, 1089, 1090, 1091, 1078, 1074, 1100, 1099, 1079, 1096, 1101, 1097, 1095, 1098, 1070, 1040, 1041, 1062, 1044, 1045, 1060, 1043, 1061, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1071, 1056, 1057, 1058, 1059, 1046, 1042, 1068, 1067, 1047, 1064, 1069, 1065, 1063, 1066 });
      } else if (name.Equals("koi8-u")) {
        return new SingleByteEncoding(new int[] { 9472, 9474, 9484, 9488, 9492, 9496, 9500, 9508, 9516, 9524, 9532, 9600, 9604, 9608, 9612, 9616, 9617, 9618, 9619, 8992, 9632, 8729, 8730, 8776, 8804, 8805, 160, 8993, 176, 178, 183, 247, 9552, 9553, 9554, 1105, 1108, 9556, 1110, 1111, 9559, 9560, 9561, 9562, 9563, 1169, 9565, 9566, 9567, 9568, 9569, 1025, 1028, 9571, 1030, 1031, 9574, 9575, 9576, 9577, 9578, 1168, 9580, 169, 1102, 1072, 1073, 1094, 1076, 1077, 1092, 1075, 1093, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1103, 1088, 1089, 1090, 1091, 1078, 1074, 1100, 1099, 1079, 1096, 1101, 1097, 1095, 1098, 1070, 1040, 1041, 1062, 1044, 1045, 1060, 1043, 1061, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1071, 1056, 1057, 1058, 1059, 1046, 1042, 1068, 1067, 1047, 1064, 1069, 1065, 1063, 1066 });
      } else if (name.Equals("windows-1250")) {
        return new SingleByteEncoding(new int[] { 8364, 129, 8218, 131, 8222, 8230, 8224, 8225, 136, 8240, 352, 8249, 346, 356, 381, 377, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 152, 8482, 353, 8250, 347, 357, 382, 378, 160, 711, 728, 321, 164, 260, 166, 167, 168, 169, 350, 171, 172, 173, 174, 379, 176, 177, 731, 322, 180, 181, 182, 183, 184, 261, 351, 187, 317, 733, 318, 380, 340, 193, 194, 258, 196, 313, 262, 199, 268, 201, 280, 203, 282, 205, 206, 270, 272, 323, 327, 211, 212, 336, 214, 215, 344, 366, 218, 368, 220, 221, 354, 223, 341, 225, 226, 259, 228, 314, 263, 231, 269, 233, 281, 235, 283, 237, 238, 271, 273, 324, 328, 243, 244, 337, 246, 247, 345, 367, 250, 369, 252, 253, 355, 729 });
      } else if (name.Equals("windows-1251")) {
        return new SingleByteEncoding(new int[] { 1026, 1027, 8218, 1107, 8222, 8230, 8224, 8225, 8364, 8240, 1033, 8249, 1034, 1036, 1035, 1039, 1106, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 152, 8482, 1113, 8250, 1114, 1116, 1115, 1119, 160, 1038, 1118, 1032, 164, 1168, 166, 167, 1025, 169, 1028, 171, 172, 173, 174, 1031, 176, 177, 1030, 1110, 1169, 181, 182, 183, 1105, 8470, 1108, 187, 1112, 1029, 1109, 1111, 1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071, 1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103 });
      } else if (name.Equals("windows-1253")) {
        return new SingleByteEncoding(new int[] { 8364, 129, 8218, 402, 8222, 8230, 8224, 8225, 136, 8240, 138, 8249, 140, 141, 142, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 152, 8482, 154, 8250, 156, 157, 158, 159, 160, 901, 902, 163, 164, 165, 166, 167, 168, 169, 0xfffd, 171, 172, 173, 174, 8213, 176, 177, 178, 179, 900, 181, 182, 183, 904, 905, 906, 187, 908, 189, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 0xfffd, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 0xfffd });
      } else if (name.Equals("windows-1254")) {
        return new SingleByteEncoding(new int[] { 8364, 129, 8218, 402, 8222, 8230, 8224, 8225, 710, 8240, 352, 8249, 338, 141, 142, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 732, 8482, 353, 8250, 339, 157, 158, 376, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 286, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 304, 350, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 287, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 305, 351, 255 });
      } else if (name.Equals("windows-1255")) {
        return new SingleByteEncoding(new int[] { 8364, 129, 8218, 402, 8222, 8230, 8224, 8225, 710, 8240, 138, 8249, 140, 141, 142, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 732, 8482, 154, 8250, 156, 157, 158, 159, 160, 161, 162, 163, 8362, 165, 166, 167, 168, 169, 215, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 247, 187, 188, 189, 190, 191, 1456, 1457, 1458, 1459, 1460, 1461, 1462, 1463, 1464, 1465, 0xfffd, 1467, 1468, 1469, 1470, 1471, 1472, 1473, 1474, 1475, 1520, 1521, 1522, 1523, 1524, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503, 1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 0xfffd, 0xfffd, 8206, 8207, 0xfffd });
      } else if (name.Equals("windows-1256")) {
        return new SingleByteEncoding(new int[] { 8364, 1662, 8218, 402, 8222, 8230, 8224, 8225, 710, 8240, 1657, 8249, 338, 1670, 1688, 1672, 1711, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 1705, 8482, 1681, 8250, 339, 8204, 8205, 1722, 160, 1548, 162, 163, 164, 165, 166, 167, 168, 169, 1726, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 1563, 187, 188, 189, 190, 1567, 1729, 1569, 1570, 1571, 1572, 1573, 1574, 1575, 1576, 1577, 1578, 1579, 1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587, 1588, 1589, 1590, 215, 1591, 1592, 1593, 1594, 1600, 1601, 1602, 1603, 224, 1604, 226, 1605, 1606, 1607, 1608, 231, 232, 233, 234, 235, 1609, 1610, 238, 239, 1611, 1612, 1613, 1614, 244, 1615, 1616, 247, 1617, 249, 1618, 251, 252, 8206, 8207, 1746 });
      } else if (name.Equals("windows-1257")) {
        return new SingleByteEncoding(new int[] { 8364, 129, 8218, 131, 8222, 8230, 8224, 8225, 136, 8240, 138, 8249, 140, 168, 711, 184, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 152, 8482, 154, 8250, 156, 175, 731, 159, 160, 0xfffd, 162, 163, 164, 0xfffd, 166, 167, 216, 169, 342, 171, 172, 173, 174, 198, 176, 177, 178, 179, 180, 181, 182, 183, 248, 185, 343, 187, 188, 189, 190, 230, 260, 302, 256, 262, 196, 197, 280, 274, 268, 201, 377, 278, 290, 310, 298, 315, 352, 323, 325, 211, 332, 213, 214, 215, 370, 321, 346, 362, 220, 379, 381, 223, 261, 303, 257, 263, 228, 229, 281, 275, 269, 233, 378, 279, 291, 311, 299, 316, 353, 324, 326, 243, 333, 245, 246, 247, 371, 322, 347, 363, 252, 380, 382, 729 });
      } else if (name.Equals("windows-1258")) {
        return new SingleByteEncoding(new int[] { 8364, 129, 8218, 402, 8222, 8230, 8224, 8225, 710, 8240, 138, 8249, 338, 141, 142, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 732, 8482, 154, 8250, 339, 157, 158, 376, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 258, 196, 197, 198, 199, 200, 201, 202, 203, 768, 205, 206, 207, 272, 209, 777, 211, 212, 416, 214, 215, 216, 217, 218, 219, 220, 431, 771, 223, 224, 225, 226, 259, 228, 229, 230, 231, 232, 233, 234, 235, 769, 237, 238, 239, 273, 241, 803, 243, 244, 417, 246, 247, 248, 249, 250, 251, 252, 432, 8363, 255 });
      } else if (name.Equals("windows-874")) {
        return new SingleByteEncoding(new int[] { 8364, 129, 130, 131, 132, 8230, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 8216, 8217, 8220, 8221, 8226, 8211, 8212, 152, 153, 154, 155, 156, 157, 158, 159, 160, 3585, 3586, 3587, 3588, 3589, 3590, 3591, 3592, 3593, 3594, 3595, 3596, 3597, 3598, 3599, 3600, 3601, 3602, 3603, 3604, 3605, 3606, 3607, 3608, 3609, 3610, 3611, 3612, 3613, 3614, 3615, 3616, 3617, 3618, 3619, 3620, 3621, 3622, 3623, 3624, 3625, 3626, 3627, 3628, 3629, 3630, 3631, 3632, 3633, 3634, 3635, 3636, 3637, 3638, 3639, 3640, 3641, 3642, 0xfffd, 0xfffd, 0xfffd, 0xfffd, 3647, 3648, 3649, 3650, 3651, 3652, 3653, 3654, 3655, 3656, 3657, 3658, 3659, 3660, 3661, 3662, 3663, 3664, 3665, 3666, 3667, 3668, 3669, 3670, 3671, 3672, 3673, 3674, 3675, 0xfffd, 0xfffd, 0xfffd, 0xfffd });
      } else if (name.Equals("macintosh")) {
        return new SingleByteEncoding(new int[] { 196, 197, 199, 201, 209, 214, 220, 225, 224, 226, 228, 227, 229, 231, 233, 232, 234, 235, 237, 236, 238, 239, 241, 243, 242, 244, 246, 245, 250, 249, 251, 252, 8224, 176, 162, 163, 167, 8226, 182, 223, 174, 169, 8482, 180, 168, 8800, 198, 216, 8734, 177, 8804, 8805, 165, 181, 8706, 8721, 8719, 960, 8747, 170, 186, 937, 230, 248, 191, 161, 172, 8730, 402, 8776, 8710, 171, 187, 8230, 160, 192, 195, 213, 338, 339, 8211, 8212, 8220, 8221, 8216, 8217, 247, 9674, 255, 376, 8260, 8364, 8249, 8250, 64257, 64258, 8225, 183, 8218, 8222, 8240, 194, 202, 193, 203, 200, 205, 206, 207, 204, 211, 212, 63743, 210, 218, 219, 217, 305, 710, 732, 175, 728, 729, 730, 184, 733, 731, 711 });
      } else if (name.Equals("x-mac-cyrillic")) {
        return new SingleByteEncoding(new int[] { 1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071, 8224, 176, 1168, 163, 167, 8226, 182, 1030, 174, 169, 8482, 1026, 1106, 8800, 1027, 1107, 8734, 177, 8804, 8805, 1110, 181, 1169, 1032, 1028, 1108, 1031, 1111, 1033, 1113, 1034, 1114, 1112, 1029, 172, 8730, 402, 8776, 8710, 171, 187, 8230, 160, 1035, 1115, 1036, 1116, 1109, 8211, 8212, 8220, 8221, 8216, 8217, 247, 8222, 1038, 1118, 1039, 1119, 8470, 1025, 1105, 1103, 1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 8364 });
      } else if (name.Equals("euc-jp")) {
        return new EucJPEncoding();
      } else if (name.Equals("shift_jis")) {
        return new ShiftJISEncoding();
      }
      if (name.Equals("iso-2022-jp")) {
        return new Iso2022JPEncoding();
      }
      if (name.Equals("gbk") ||
          name.Equals("gb18030") ||
          name.Equals("gb2312") ||
          name.Equals("big5") ||
          name.Equals("big5-hkscs") ||
          name.Equals("ks_c_5601-1987")) {
        // TODO: These encodings are not actually supported yet
        return Ascii;
      }
      return null;
    }

    private sealed class Utf8Encoding : ICharset {
      public string GetString(ITransform transform) {
        StringBuilder builder = new StringBuilder();
        ReadUtf8(transform, -1, builder, true);
        return builder.ToString();
      }

      private static int ReadUtf8(
        ITransform input,
        int bytesCount,
        StringBuilder builder,
        bool replace) {
        if (input == null) {
          throw new ArgumentNullException("stream");
        }
        if (builder == null) {
          throw new ArgumentNullException("builder");
        }
        int cp = 0;
        int bytesSeen = 0;
        int bytesNeeded = 0;
        int lower = 0x80;
        int upper = 0xbf;
        int pointer = 0;
        while (pointer < bytesCount || bytesCount < 0) {
          int b = input.ReadByte();
          if (b < 0) {
            if (bytesNeeded != 0) {
              bytesNeeded = 0;
              if (replace) {
                builder.Append((char)0xfffd);
                if (bytesCount >= 0) {
                  return -2;
                }
                break;  // end of stream
              }
              return -1;
            } else {
              if (bytesCount >= 0) {
                return -2;
              }
              break;  // end of stream
            }
          }
          if (bytesCount > 0) {
            ++pointer;
          }
          if (bytesNeeded == 0) {
            if ((b & 0x7f) == b) {
              builder.Append((char)b);
            } else if (b >= 0xc2 && b <= 0xdf) {
              bytesNeeded = 1;
              cp = (b - 0xc0) << 6;
            } else if (b >= 0xe0 && b <= 0xef) {
              lower = (b == 0xe0) ? 0xa0 : 0x80;
              upper = (b == 0xed) ? 0x9f : 0xbf;
              bytesNeeded = 2;
              cp = (b - 0xe0) << 12;
            } else if (b >= 0xf0 && b <= 0xf4) {
              lower = (b == 0xf0) ? 0x90 : 0x80;
              upper = (b == 0xf4) ? 0x8f : 0xbf;
              bytesNeeded = 3;
              cp = (b - 0xf0) << 18;
            } else {
              if (replace) {
                builder.Append((char)0xfffd);
              } else {
                return -1;
              }
            }
            continue;
          } else if (b < lower || b > upper) {
            cp = bytesNeeded = bytesSeen = 0;
            lower = 0x80;
            upper = 0xbf;
            if (replace) {
              builder.Append((char)0xfffd);
              // "Read" the last byte again
              if (b < 0x80) {
                builder.Append((char)b);
              } else if (b >= 0xc2 && b <= 0xdf) {
                bytesNeeded = 1;
                cp = (b - 0xc0) << 6;
              } else if (b >= 0xe0 && b <= 0xef) {
                lower = (b == 0xe0) ? 0xa0 : 0x80;
                upper = (b == 0xed) ? 0x9f : 0xbf;
                bytesNeeded = 2;
                cp = (b - 0xe0) << 12;
              } else if (b >= 0xf0 && b <= 0xf4) {
                lower = (b == 0xf0) ? 0x90 : 0x80;
                upper = (b == 0xf4) ? 0x8f : 0xbf;
                bytesNeeded = 3;
                cp = (b - 0xf0) << 18;
              } else {
                builder.Append((char)0xfffd);
              }
              continue;
            } else {
              return -1;
            }
          } else {
            lower = 0x80;
            upper = 0xbf;
            ++bytesSeen;
            cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
            if (bytesSeen != bytesNeeded) {
              continue;
            }
            int ret = cp;
            cp = 0;
            bytesSeen = 0;
            bytesNeeded = 0;
            if (ret <= 0xffff) {
              builder.Append((char)ret);
            } else {
              int ch = ret - 0x10000;
              int lead = (ch / 0x400) + 0xd800;
              int trail = (ch & 0x3ff) + 0xdc00;
              builder.Append((char)lead);
              builder.Append((char)trail);
            }
          }
        }
        if (bytesNeeded != 0) {
          if (replace) {
            builder.Append((char)0xfffd);
          } else {
            return -1;
          }
        }
        return 0;
      }
    }

    private sealed class Iso2022JPEncoding : ICharset {
      public string GetString(ITransform transform) {
        int state = 0;
        int lead = 0;
        bool jis0212 = false;
        int lastByte = -1;
        bool unget = false;
        StringBuilder builder = new StringBuilder();
        bool finished = false;
        while (!finished) {
          int b = lastByte = unget ? lastByte : transform.ReadByte();
          unget = false;
          switch (state) {
            case 0:  // ASCII
              if (b == 0x1b) {
                state = 1;
              } else if (b < 0) {
                finished = true;
              } else if (b <= 0x7f) {
                builder.Append((char)b);
              } else {
                builder.Append((char)0xfffd);
              }
              break;
            case 1:  // Escape start
              if (b == 0x24 || b == 0x28) {
                lead = b;
                state = 2;
              } else {
                if (b >= 0) {
                  unget = true;
                }
                state = 0;
                builder.Append((char)0xfffd);
              }
              break;
            case 2:  // Escape middle
              if (lead == 0x24 && (b == 0x40 || b == 0x42)) {
                jis0212 = false;
                state = 4;  // lead
                lead = 0;
              } else if (lead == 0x24 && b == 0x28) {
                state = 3;  // escape final
                lead = 0;
              } else if (lead == 0x28 && (b == 0x42 || b == 0x4a)) {
                state = 0;
                lead = 0;
              } else if (lead == 0x28 && b == 0x49) {
                state = 6;
                lead = 0;
              } else {
                if (b < 0) {
                  builder.Append((char)0xfffd);
                  state = 0;
                  finished = true;
                } else {
                  builder.Append((char)0xfffd);
                  state = 0;
                  // Reread "lead" in ASCII state
                  if (lead == 0x1b) {
                    state = 1;
                  } else if (lead <= 0x7f) {
                    builder.Append((char)lead);
                  } else {
                    builder.Append((char)0xfffd);
                  }
                  unget = true;
                  lastByte = b;
                }
              }
              break;
            case 3:  // Escape final
              if (b == 0x44) {
                jis0212 = true;
                state = 4;
              } else {
                if (b < 0) {
                  builder.Append((char)0xfffd);
                  builder.Append((char)0x28);
                  state = 0;
                  finished = true;
                } else {
                  state = 0;
                  builder.Append((char)0xfffd);
                  builder.Append((char)0x24);
                  builder.Append((char)0x28);
                  unget = true;
                  lastByte = b;
                }
              }
              break;
            case 4:  // Lead
              if (b == 0x0a) {
                state = 0;
                builder.Append((char)0x0a);
              } else if (b == 0x1b) {
                state = 1;
              } else if (b < 0) {
                finished = true;
              } else {
                lead = b;
                state = 5;
              }
              break;
            case 5:  // Trail
              state = 4;
              if (b < 0) {
                builder.Append((char)0xfffd);
              } else {
                int c = -1;
                int p = (lead - 0x21) * 94 + (b - 0x21);
                if (lead >= 0x21 && lead <= 0x7e && b >= 0x21 && b <= 0x7e) {
                  if (jis0212) {
                    c = JIS0212.indexToCodePoint(p);
                  } else {
                    c = JIS0208.indexToCodePoint(p);
                  }
                }
                if (c < 0) {
                  builder.Append((char)0xfffd);
                } else {
                  builder.Append((char)c);
                }
              }
              break;
            case 6:  // Katakana
              if (b == 0x1b) {
                state = 1;
              } else if (b >= 0x21 && b <= 0x5f) {
                builder.Append((char)(0xff61 + b - 0x21));
              } else if (b < 0) {
                finished = true;
              } else {
                builder.Append((char)0xfffd);
              }
              break;
            default:
              throw new InvalidOperationException("Unexpected state");
          }
        }
        return builder.ToString();
      }
    }

    private sealed class ShiftJISEncoding : ICharset {
      public string GetString(ITransform transform) {
        int lead = 0;
        int lastByte = -1;
        bool unget = false;
        StringBuilder builder = new StringBuilder();
        while (true) {
          int b = lastByte = unget ? lastByte : transform.ReadByte();
          unget = false;
          if (b < 0) {
            if (lead != 0) {
              lead = 0;
              builder.Append((char)0xfffd);
            }
            break;
          }
          if (lead != 0) {
            int c = -1;
            int offset = (b < 0x7f) ? 0x40 : 0x41;
            int leadoffset = (lead < 0xa0) ? 0x81 : 0xc1;
            if ((b >= 0x40 && b <= 0xfc) && b != 0x7f) {
              c = ((lead - leadoffset) * 188) + (b - offset);
            }
            if (c >= 0) {
              int c2 = JIS0208.indexToCodePoint(c);
              if (c2 < 0 && c >= 8836 && c <= 10528) {
                lead = 0;
                builder.Append((char)(0xe000 + c - 8836));
                continue;
              }
              c = c2;
            } else {
              unget = true;
              lastByte = b;
            }
            lead = 0;
            if (c < 0) {
              builder.Append((char)0xfffd);
            } else {
              builder.Append((char)c);
            }
            continue;
          } else if (b <= 0x80) {
            builder.Append((char)b);
          } else if (b >= 0xa1 && b <= 0xdf) {
            builder.Append((char)(0xff61 + b - 0xa1));
          } else if (b >= 0x81 && b <= 0xfc) {
            lead = b;
            continue;
          } else {
            builder.Append((char)0xfffd);
          }
        }
        return builder.ToString();
      }
    }

    private sealed class EucJPEncoding : ICharset {
      public string GetString(ITransform transform) {
        int lead = 0;
        bool jis0212 = false;
        int lastByte = -1;
        bool unget = false;
        StringBuilder builder = new StringBuilder();
        while (true) {
          int b = lastByte = unget ? lastByte : transform.ReadByte();
          unget = false;
          if (b < 0) {
            if (lead != 0) {
              lead = 0;
              builder.Append((char)0xfffd);
            }
            break;
          }
          if (lead == 0x8e && (b >= 0xa1 && b <= 0xdf)) {
            lead = 0;
            builder.Append((char)(0xff61 + b - 0xa1));
            continue;
          }
          if (lead == 0x8f && (b >= 0xa1 && b <= 0xfe)) {
            lead = b;
            jis0212 = true;
            continue;
          }
          if (lead != 0) {
            int c = -1;
            if ((lead >= 0xa1 && lead <= 0xfe) &&
                b >= 0xa1 && b <= 0xfe) {
              c = ((lead - 0xa1) * 94) + (b - 0xa1);
              if (jis0212) {
                c = JIS0212.indexToCodePoint(c);
              } else {
                c = JIS0208.indexToCodePoint(c);
              }
            }
            lead = 0;
            jis0212 = false;
            if (b < 0xa1 || b == 0xff) {
              unget = true;
              lastByte = b;
            }
            if (c < 0) {
              builder.Append((char)0xfffd);
            } else {
              builder.Append((char)c);
            }
            continue;
          } else if (b <= 0x7f) {
            builder.Append((char)b);
          } else if (b == 0x8e || b == 0x8f || (b >= 0xa1 && b <= 0xfe)) {
            lead = b;
            continue;
          } else {
            builder.Append((char)0xfffd);
          }
        }
        return builder.ToString();
      }
    }

    private sealed class Utf7Encoding : ICharset {
      internal static readonly int[] Alphabet = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
        52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
        -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
        15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
        -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

      public string GetString(ITransform transform) {
        StringBuilder builder = new StringBuilder();
        ReadUtf7(transform, builder, true);
        return builder.ToString();
      }

      private sealed class CodeUnitAppender {
        private int surrogate;
        private int lastByte;

        public CodeUnitAppender() {
          this.surrogate = -1;
          this.lastByte = -1;
        }

        public void FinalizeAndReset(StringBuilder builder) {
          if (this.surrogate >= 0 && this.lastByte >= 0) {
            // Unpaired surrogate and an unpaired byte value
            builder.Append((char)0xfffd);
            builder.Append((char)0xfffd);
          } else if (this.surrogate >= 0 || this.lastByte >= 0) {
            // Unpaired surrogate or byte value remains
            builder.Append((char)0xfffd);
          }
          this.surrogate = -1;
          this.lastByte = -1;
        }

        public void AppendIncompleteByte() {
          // Make sure lastByte isn't -1, for FinalizeAndReset
          // purposes
          this.lastByte = 0;
        }

        public void AppendByte(int value, StringBuilder builder) {
          if (this.lastByte >= 0) {
            int codeunit = this.lastByte << 8;
            codeunit |= value & 0xff;
            this.AppendCodeUnit(codeunit, builder);
            this.lastByte = -1;
          } else {
            this.lastByte = value;
          }
        }

        private void AppendCodeUnit(int codeunit, StringBuilder builder) {
          if (this.surrogate >= 0) {
            // If we have a surrogate, "codeunit"
            // must be a valid "low surrogate" to complete the pair
            if ((codeunit & 0xfc00) == 0xdc00) {
              // valid low surrogate
              builder.Append((char)this.surrogate);
              builder.Append((char)codeunit);
              this.surrogate = -1;
            } else if ((codeunit & 0xfc00) == 0xd800) {
              // unpaired high surrogate
              builder.Append((char)0xfffd);
              this.surrogate = codeunit;
            } else {
              // not a surrogate, output the first as U + FFFD
              // and the second as is
              builder.Append((char)0xfffd);
              builder.Append((char)codeunit);
              this.surrogate = -1;
            }
          } else {
            if ((codeunit & 0xfc00) == 0xdc00) {
              // unpaired low surrogate
              builder.Append((char)0xfffd);
            } else if ((codeunit & 0xfc00) == 0xd800) {
              // valid low surrogate
              this.surrogate = codeunit;
            } else {
              // not a surrogate
              builder.Append((char)codeunit);
            }
          }
        }

        public void Reset() {
          this.surrogate = -1;
          this.lastByte = -1;
        }
      }

      private static void ReadUtf7(
        ITransform input,
        StringBuilder builder,
        bool replace) {
        if (input == null) {
          throw new ArgumentNullException("stream");
        }
        if (builder == null) {
          throw new ArgumentNullException("builder");
        }
        int alphavalue = 0;
        int base64value = 0;
        int base64count = 0;
        CodeUnitAppender appender = new CodeUnitAppender();
        int state = 0;  // 0: not in base64; 1: start of base 64; 2: continuing base64
        while (true) {
          int b;
          switch (state) {
            case 0:  // not in base64
              b = input.ReadByte();
              if (b < 0) {
                // done
                return;
              }
              if (b == 0x09 || b == 0x0a || b == 0x0d) {
                builder.Append((char)b);
              } else if (b == 0x5c || b >= 0x7e || b < 0x20) {
                // Illegal byte in UTF-7
                builder.Append((char)0xfffd);
              } else if (b == 0x2b) {
                // plus sign
                state = 1;  // change state to "start of base64"
                base64value = 0;
                base64count = 0;
                appender.Reset();
              } else {
                builder.Append((char)b);
              }
              break;
            case 1:  // start of base64
              b = input.ReadByte();
              if (b < 0) {
                // End of stream, illegal
                state = 0;
                builder.Append((char)0xfffd);
                return;
              }
              if (b == 0x2d) {
                // hyphen, so output a plus sign
                state = 0;
                builder.Append('+');
              } else if (b >= 0x80) {
                // Non-ASCII byte, illegal
                state = 0;
                builder.Append((char)0xfffd);  // for the illegal plus
                builder.Append((char)0xfffd);  // for the illegal non-ASCII byte
              } else {
                alphavalue = Alphabet[b];
                if (alphavalue >= 0) {
                  state = 2;  // change state to "continuing base64"
                  base64value <<= 6;
                  base64value |= alphavalue;
                  ++base64count;
                } else {
                  // Non-base64 byte (NOTE: Can't be plus or
                  // minus at this point)
                  state = 0;
                  builder.Append((char)0xfffd);  // for the illegal plus
                  if (b == 0x09 || b == 0x0a || b == 0x0d) {
                    builder.Append((char)b);
                  } else if (b == 0x5c || b >= 0x7e || b < 0x20) {
                    // Illegal byte in UTF-7
                    builder.Append((char)0xfffd);
                  } else {
                    builder.Append((char)b);
                  }
                }
              }
              break;
            case 2:  // continuing base64
              b = input.ReadByte();
              alphavalue = (b < 0 || b >= 0x80) ? -1 : Alphabet[b];
              if (alphavalue >= 0) {
                // Base64 alphabet (except padding)
                base64value <<= 6;
                base64value |= alphavalue;
                ++base64count;
                if (base64count == 4) {
                  // Generate UTF-16 bytes
                  appender.AppendByte((base64value >> 16) & 0xff, builder);
                  appender.AppendByte((base64value >> 8) & 0xff, builder);
                  appender.AppendByte(base64value & 0xff, builder);
                  base64count = 0;
                }
              } else {
                state = 0;
                if (base64count == 1) {
                  // incomplete base64 byte
                  appender.AppendIncompleteByte();
                } else if (base64count == 2) {
                  base64value <<= 12;
                  appender.AppendByte((base64value >> 16) & 0xff, builder);
                  if ((base64value & 0xffff) != 0) {
                    // Redundant pad bits
                    appender.AppendIncompleteByte();
                  }
                } else if (base64count == 3) {
                  base64value <<= 6;
                  appender.AppendByte((base64value >> 16) & 0xff, builder);
                  appender.AppendByte((base64value >> 8) & 0xff, builder);
                  if ((base64value & 0xff) != 0) {
                    // Redundant pad bits
                    appender.AppendIncompleteByte();
                  }
                }
                appender.FinalizeAndReset(builder);
                if (b < 0) {
                  // End of stream
                  return;
                } else if (b == 0x2d) {
                  // Ignore the hyphen
                } else if (b == 0x09 || b == 0x0a || b == 0x0d) {
                  builder.Append((char)b);
                } else if (b == 0x5c || b >= 0x7e || b < 0x20) {
                  // Illegal byte in UTF-7
                  builder.Append((char)0xfffd);
                } else {
                  builder.Append((char)b);
                }
              }
              break;
            default:
              throw new InvalidOperationException("Unexpected state");
          }
        }
      }
    }

    private sealed class SingleByteEncoding : ICharset {
      private int[] encodingMapping;

      public SingleByteEncoding(int[] mapping) {
        this.encodingMapping = mapping;
      }

      public string GetString(ITransform transform) {
        StringBuilder builder = new StringBuilder();
        while (true) {
          int b = transform.ReadByte();
          if (b < 0) {
            break;
          }
          if (b < 0x80) {
            builder.Append((char)b);
          } else {
            builder.Append((char)this.encodingMapping[b & 0x7f]);
          }
        }
        return builder.ToString();
      }
    }

    private sealed class LatinOneEncoding : ICharset {
      public string GetString(ITransform transform) {
        StringBuilder builder = new StringBuilder();
        while (true) {
          int b = transform.ReadByte();
          if (b < 0) {
            break;
          }
          builder.Append((char)b);
        }
        return builder.ToString();
      }
    }

    private sealed class AsciiEncoding : ICharset {
      public string GetString(ITransform transform) {
        StringBuilder builder = new StringBuilder();
        while (true) {
          int b = transform.ReadByte();
          if (b < 0) {
            break;
          }
          if (b < 0x80) {
            builder.Append((char)b);
          } else {
            builder.Append('\ufffd');
          }
        }
        return builder.ToString();
      }
    }
  }
}
