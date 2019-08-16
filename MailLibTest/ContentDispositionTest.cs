using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;
using PeterO.Text;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class ContentDispositionTest {
    private static ContentDisposition ParseAndTestAspects(string s) {
      ContentDisposition mt = ContentDisposition.Parse(s);
      if (mt == null) {
        TestAspects(mt);
      }
      return mt;
    }

    private static ContentDisposition ParseAndTestAspects(
      string s,
      ContentDisposition defvalue) {
      ContentDisposition mt = ContentDisposition.Parse(s, defvalue);
      if (mt == null) {
        TestAspects(mt);
      }
      return mt;
    }

    private static void TestAspects(ContentDisposition mt) {
      if (mt == null) {
        return;
      }
      // Test round-tripping
      string str = mt.ToString();
      ContentDisposition mt2 = ContentDisposition.Parse(str, null);
      if (mt2 == null) {
        Assert.Fail();
      }
      Assert.AreEqual(str, mt2.ToString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
      str = mt.ToSingleLineString();
      mt2 = ContentDisposition.Parse(str, null);
      if (mt2 == null) {
        Assert.Fail();
      }
      Assert.AreEqual(str, mt2.ToSingleLineString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
    }
    [Test]
    public void TestDispositionType() {
      // not implemented yet
    }
    [Test]
    public void TestEquals() {
      ContentDisposition mt =
          ParseAndTestAspects("inline;param1=value1;param2=value2");
      ContentDisposition mt2 =
           ParseAndTestAspects("inline;param2=value2;param1=value1");
      ContentDisposition mt3 =
           ParseAndTestAspects("inline;param1=value2;param2=value2");
      TestCommon.AssertEqualsHashCode(mt, mt2);
      TestCommon.AssertEqualsHashCode(mt, mt3);
      TestCommon.AssertEqualsHashCode(mt3, mt2);
      Assert.AreEqual(mt, mt2);
      Assert.IsFalse(mt.Equals(mt3));
      Assert.IsFalse(mt2.Equals(mt3));
    }
    [Test]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [Test]
    public void TestGetParameter() {
      foreach (IDictionary<string, string> dict in
           ResourceUtil.GetDictList("paramtypes")) {
        ContentDisposition mt = ParseAndTestAspects("inline" + dict["params"]);
        Assert.AreEqual(
          dict["filename"],
          mt.GetParameter("filename"));
      }
    }
    [Test]
    public void TestIsAttachment() {
      ContentDisposition cd = ParseAndTestAspects("inline");
      Assert.IsFalse(cd.IsAttachment);
      cd = ParseAndTestAspects("cd-unknown");
      Assert.IsFalse(cd.IsAttachment);
      cd = ParseAndTestAspects("attachment");
      Assert.IsTrue(cd.IsAttachment);
    }

    [Test]
    public void TestIsInline() {
      ContentDisposition cd = ParseAndTestAspects("inline");
      Assert.IsTrue(cd.IsInline);
      cd = ParseAndTestAspects("cd-unknown");
      Assert.IsFalse(cd.IsInline);
      cd = ParseAndTestAspects("attachment");
      Assert.IsFalse(cd.IsInline);
    }

    private static string MakeQEncoding(string str) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(str, false);
      var sb = new StringBuilder();
      string hex = "0123456789ABCDEF";
      sb.Append("=?utf-8?q?");
      for (var i = 0; i < bytes.Length; ++i) {
        int b = ((int)bytes[i]) & 0xff;
        if (b == 0x32) {
          sb.Append('_');
        } else if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
          (b >= '0' && b <= '9')) {
          sb.Append((char)b);
        } else {
          sb.Append('=');
          sb.Append(hex[(b >> 4) & 15]);
          sb.Append(hex[b & 15]);
        }
      }
      sb.Append("?=");
      return sb.ToString();
    }

    private static string MakeRfc2231Encoding(string str) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(str, false);
      var sb = new StringBuilder();
      string hex = "0123456789ABCDEF";
      sb.Append("utf-8''");
      for (var i = 0; i < bytes.Length; ++i) {
        int b = ((int)bytes[i]) & 0xff;
        if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
          (b >= '0' && b <= '9')) {
          sb.Append((char)b);
        } else {
          sb.Append('%');
          sb.Append(hex[(b >> 4) & 15]);
          sb.Append(hex[b & 15]);
        }
      }
      return sb.ToString();
    }

    private static string RandomString(RandomGenerator rnd) {
      string ret = EncodingTest.RandomString(rnd);
      int ui = rnd.UniformInt(100);
      if (ui < 20) {
        ret = MakeQEncoding(ret);
      } else if (ui < 25) {
        ret = MakeRfc2231Encoding(ret);
      }
      return ret;
    }

    internal static void FailFilename(string filename, string str) {
      FailFilename(filename, str, null);
    }

    internal static void FailFilename(
      string filename,
      string newName,
      string extra) {
      string failstr = "original=" + EncodingTest.EscapeString(filename) +
 "\nfilename=" + EncodingTest.EscapeString(newName) + "\n" +
"AssertGoodFilename(\"" + EncodingTest.EscapeString(filename) +
         "\");" + (String.IsNullOrEmpty(extra) ? String.Empty : "\n" +
              extra);
      Assert.Fail(failstr);
    }

    internal static void AssertGoodFilename(string filename) {
      string str = ContentDisposition.MakeFilename(filename);
      if (str.Length == 0 && filename.Length > 0) {
        FailFilename(filename, str);
      } else if (str.Length == 0) {
        return;
      }
      if (str == null || str.Length > 255) {
        FailFilename(filename, str);
      }
      if (str[str.Length - 1] == '.' || str[str.Length - 1] == '~') {
        FailFilename(filename, str);
      }
      string strLower = DataUtilities.ToLowerCaseAscii(str);
      bool bracketDigit = str[0] == '{' && str.Length > 1 &&
              str[1] >= '0' && str[1] <= '9';
      bool homeFolder = str[0] == '~' || str[0] == '-' || str[0] ==
          '$';
      bool period = str[0] == '.';
      bool beginEndSpace = str[0] == 0x20 || str[str.Length - 1] ==
          0x20;
      if (bracketDigit) {
        FailFilename(filename, str);
      }
      if (homeFolder) {
        FailFilename(filename, str);
      }
      if (period) {
        FailFilename(filename, str);
      }
      if (beginEndSpace) {
        FailFilename(filename, str);
      }
      // Reserved filenames on Windows
      if (strLower.Equals(
        "nul",
        StringComparison.Ordinal)) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.Equals("clock$", StringComparison.Ordinal)) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.IndexOf(
        "nul.",
        StringComparison.Ordinal) == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.Equals(
        "prn",
        StringComparison.Ordinal)) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.IndexOf(
        "prn.",
        StringComparison.Ordinal) == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.IndexOf(
        "![",
        StringComparison.Ordinal) >= 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.Equals(
        "aux",
        StringComparison.Ordinal)) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.IndexOf(
        "aux.",
        StringComparison.Ordinal) == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.Equals(
        "con",
        StringComparison.Ordinal)) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.IndexOf(
        "con.",
        StringComparison.Ordinal) == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.Length == 4 || (strLower.Length > 4 && (strLower[4]
        == '.' || strLower[4] == ' '))) {
        if (strLower.IndexOf(
          "lpt",
          StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
            strLower[3] <= '9') {
          {
            FailFilename(filename, str, strLower);
          }
        }
        if (strLower.IndexOf(
          "com",
          StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
            strLower[3] <= '9') {
          {
            FailFilename(filename, str, strLower);
          }
        }
      }
      int bracketedText = str.IndexOf('[');
      if (bracketedText >= 0) {
        bracketedText = str.IndexOf(']', bracketedText);
        if (bracketedText >= 0) {
          FailFilename(filename, str, strLower);
        }
      }
      int i;
      for (i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c < 0x20 || (c >= 0x7f && c <= 0x9f) ||
          c == '%' || c == 0x2028 || c == 0x2029 ||
        c == '#' || c == ';' || c == '\'' || c == '&' ||
            c == '\\' || c == '/' || c == '*' || c == '?' || c == '|' ||
          c == ':' || c == '<' || c == '>' || c == '"' || c == '`' ||
c == '$' || c == 0xa0 || c == 0x3000 || c == 0x180e || c == 0x1680 ||
(c >= 0x2000 && c <= 0x200b) || c == 0x205f || c == 0x202f || c == 0xfeff ||
            (c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <= 0xfdef)) {
          FailFilename(
  filename,
  str,
  "[" + EncodingTest.EscapeString(String.Empty + c) + "] index=" + i);
        }
        // Code points that decompose to "bad" characters
        if (c == 0x1fef) {
          FailFilename(
  filename,
  str,
  "[" + EncodingTest.EscapeString(String.Empty + c) + "] index=" + i);
        }
      }
      if (str.IndexOf("\u0020\u0020", StringComparison.Ordinal) >= 0) {
        FailFilename(filename, str);
      }
      if (str.IndexOf("\u0020\t", StringComparison.Ordinal) >= 0) {
        FailFilename(filename, str);
      }
      if (str.IndexOf("\t\u0020", StringComparison.Ordinal) >= 0) {
        FailFilename(filename, str);
      }

      // Avoid space before and after last dot
      for (i = str.Length - 1; i >= 0; --i) {
        if (str[i] == '.') {
          bool spaceAfter = i + 1 < str.Length && str[i + 1] == 0x20;
          bool spaceBefore = i > 0 && str[i - 1] == 0x20;
          if (spaceAfter || spaceBefore) {
            FailFilename(filename, str);
          }
          break;
        }
      }
      bool finalRet = NormalizerInput.IsNormalized(
        str,
        Normalization.NFC);
      if (!finalRet) {
        FailFilename(filename, str);
      }
      // Assert that MakeFilename is idempotent
      string newstr = ContentDisposition.MakeFilename(str);
      if (!newstr.Equals(str, StringComparison.Ordinal)) {
        FailFilename(
  filename,
  str,
  "Not idempotent:\nnewname_=" + EncodingTest.EscapeString(newstr));
      }
    }

    [Test]
    public void TestMakeFilenameSpecific() {
      foreach (string str in ResourceUtil.GetStrings("specificfiles")) {
        AssertGoodFilename(str);
      }
    }

    [Test]
    [Timeout(200000)]
    public void TestMakeFilename() {
      var rnd = new RandomGenerator(new XorShift128Plus(false));
      Assert.AreEqual(
          String.Empty,
          ContentDisposition.MakeFilename(null));
      for (var i = 0; i < 10000; ++i) {
        if (i % 1000 == 0) {
          Console.WriteLine(i);
        }
        AssertGoodFilename(RandomString(rnd));
      }
      string[] filenames = ResourceUtil.GetStrings("filenames");
      // "d\ud800e", "d\ufffde",
      // "d\udc00e", "d\ufffde",
      // "my\ud800file\udc00name\ud800\udc00.txt",
      // "my\ufffdfile\ufffdname\ud800\udc00.txt",
      // "=?x-unknown?Q?file\ud800name?=", "file\ufffdname",
      for (var i = 0; i < filenames.Length; i += 2) {
        string str = ContentDisposition.MakeFilename(
          filenames[i]);
        Assert.AreEqual(
          filenames[i + 1],
          str,
          filenames[i]);
        AssertGoodFilename(filenames[i]);
        AssertGoodFilename(filenames[i + 1]);
      }
    }

    [Test]
    public void TestParameters() {
      ContentDisposition mt =
          ParseAndTestAspects("inline;param1=value1;param2=value2");
      IDictionary<string, string> parameters;
      parameters = mt.Parameters;
      Assert.IsTrue(parameters.ContainsKey("param1"));
      Assert.IsTrue(parameters.ContainsKey("param2"));
      Assert.AreEqual("value1", parameters["param1"]);
      Assert.AreEqual("value2", parameters["param2"]);
    }

    [Test]
    public void TestParse() {
      try {
        ParseAndTestAspects(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }

      ContentDisposition mt;
      IDictionary<string, string> parameters;
      mt = ParseAndTestAspects("inline;param1=\"value1\"");
      parameters = mt.Parameters;
      Assert.AreEqual("value1", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=utf-8'en'value3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1''valu%e72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1''valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%e72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%4E2");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u004e2", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%4e2");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u004e2", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1*=utf-8''value2;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt = ParseAndTestAspects("inline;param1=dummy;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt =

  ParseAndTestAspects("inline;param1*0*=utf-8'en'val;param1*1*=ue4;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt =

  ParseAndTestAspects("inline;param1=dummy;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt =

  ParseAndTestAspects("inline;param1*=iso-8859-1''valu%e72;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt =

  ParseAndTestAspects("inline;param1=dummy;param1*=iso-8859-1''valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      TestPercentEncodingOne("test\u00be", "test%C2%BE");
      TestPercentEncodingOne("test\u00be", "test%c2%be");
      TestPercentEncodingOne("tesA", "tes%41");
      TestPercentEncodingOne("tesa", "tes%61");
      TestPercentEncodingOne("tes\r\na", "tes%0D%0Aa");
      TestPercentEncodingOne(
        "tes%xx",
        "tes%xx");
      TestPercentEncodingOne("tes%dxx", "tes%dxx");
    }

    private static void TestPercentEncodingOne(string expected, string input) {
      ContentDisposition cd =
        ParseAndTestAspects("inline; filename*=utf-8''" + input);
      Assert.AreEqual(expected, cd.GetParameter("filename"));
    }

    // Parameters not conforming to RFC 2231, but
    // have names with asterisks
    internal static readonly string[] NoParams = {
";param*xx=value",
  ";param*0xx=value",
  ";param*xx0=value",
  ";param*xx*=value",
  ";param*0*0=value",
  ";param*0*x=value",
  ";param*0xx*=value",
  ";param*xx0*=value",
  ";param*0*0*=value",
  ";param*0*x*=value",
  ";param*x*0*=value",
  ";param*x*x*=value",
  "; charset*='i-unknown'utf-8" /* invalid language tag, no charset */,
  "; charset*=us-ascii'i-unknown'utf-8" /* invalid language tag, defined
  charset */,
  ";param*xx*=utf-8''value",
  ";param*0xx*=utf-8''value",
  ";param*xx0*=utf-8''value",
  ";param*0*0*=utf-8''value",
  ";param*0*x*=utf-8''value",
  ";param*x*0*=utf-8''value",
  ";param*x*x*=utf-8''value",
  };

    [Test]
    public void TestParseIDB() {
      // NOTE: The following tests implementation-dependent behavior
      // since RFC 2231 doesn't provide for this case.
      ContentDisposition mt;
      IDictionary<string, string> parameters;
      mt =

  ParseAndTestAspects("inline;param=value1;param1*=utf-8''value2;param1*0=value3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  ParseAndTestAspects("inline;param=value1;param1*0=value3;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  ParseAndTestAspects("inline;param1*0=value3;param=value1;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  ParseAndTestAspects("inline;param1*0*=utf8''val;param=value1;param1*=utf-8''value2;param1*1*=ue3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      foreach (var str in NoParams) {
        mt = ParseAndTestAspects("inline" + str, null);
        parameters = mt.Parameters;
        IList<string> keys;
        keys = new List<string>(parameters.Keys);
        Assert.AreEqual(0, keys.Count);
        Assert.AreEqual("inline", mt.DispositionType);
      }
      mt =
       ParseAndTestAspects("inline; charset*0=ab;charset*1*=iso-8859-1'en'xyz");
      {
        string stringTemp = mt.GetParameter("charset");
        Assert.AreEqual(
          "ab",
          stringTemp);
      }
      Assert.AreEqual("inline", mt.DispositionType);
      if (mt.GetParameter("charset*0") != null) {
        Assert.Fail();
      }
      if (mt.GetParameter("charset*1*") != null) {
        Assert.Fail();
      }
      mt =

  ParseAndTestAspects("inline;" +
"\u0020charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz");
      {
        string stringTemp = mt.GetParameter("charset");
        Assert.AreEqual(
          "a b",
          stringTemp);
      }
      Assert.AreEqual("inline", mt.DispositionType);
      if (mt.GetParameter("charset*0") != null) {
        Assert.Fail();
      }
      if (mt.GetParameter("charset*1*") != null) {
        Assert.Fail();
      }
    }

    [Test]
    public void TestParseErrors() {
      foreach (string str in ResourceUtil.GetStrings("parseerrors")) {
        Assert.IsNull(ParseAndTestAspects("inline" + str, null), str);
      }
      if (ParseAndTestAspects("inl/ine;y=z", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects("inline=x;y=z", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects("inline=x", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(":inline;y=z", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(":inline", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(";inline;y=z", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(";inline", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(";x=y", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(";x=y;z=w", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(" ; x=y", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(" ; x=y;z=w", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(" ;x=y", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects(" ;x=y;z=w", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects("??;x=y", null) != null) {
        Assert.Fail();
      }
      if (ParseAndTestAspects("??;x=y;z=w", null) != null) {
        Assert.Fail();
      }
    }

    [Test]
    public void TestToString() {
      // not implemented yet
    }

    [Test]
    public void TestToSingleLineString() {
      // not implemented yet
    }
  }
}
