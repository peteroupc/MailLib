package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

  public class ContentDispositionTest {
    private static ContentDisposition ParseAndTestAspects(String s) {
      ContentDisposition mt = ContentDisposition.Parse(s);
      if (mt == null) {
        TestAspects(mt);
      }
      return mt;
    }

    private static ContentDisposition ParseAndTestAspects(
      String s,
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
      String str = mt.toString();
      ContentDisposition mt2 = ContentDisposition.Parse(str, null);
      if (mt2 == null) {
        Assert.fail();
      }
      Assert.assertEquals(str, mt2.toString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
      str = mt.ToSingleLineString();
      mt2 = ContentDisposition.Parse(str, null);
      if (mt2 == null) {
        Assert.fail();
      }
      Assert.assertEquals(str, mt2.ToSingleLineString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
    }
    @Test
    public void TestDispositionType() {
      // not implemented yet
    }
    @Test
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
      Assert.assertEquals(mt, mt2);
      if (mt.equals(mt3)) {
 Assert.fail();
 }
      if (mt2.equals(mt3)) {
 Assert.fail();
 }
    }
    @Test
    public void TestGetHashCode() {
      // not implemented yet
    }
    @Test
    public void TestGetParameter() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("paramtypes")) {
        ContentDisposition mt = ParseAndTestAspects("inline" + dict.get("params"));
        Assert.assertEquals(
          dict.get("filename"),
          mt.GetParameter("filename"));
      }
    }
    @Test
    public void TestIsAttachment() {
      ContentDisposition cd = ParseAndTestAspects("inline");
      if (cd.isAttachment()) {
 Assert.fail();
 }
      cd = ParseAndTestAspects("cd-unknown");
      if (cd.isAttachment()) {
 Assert.fail();
 }
      cd = ParseAndTestAspects("attachment");
      if (!(cd.isAttachment())) {
 Assert.fail();
 }
    }

    @Test
    public void TestIsInline() {
      ContentDisposition cd = ParseAndTestAspects("inline");
      if (!(cd.isInline())) {
 Assert.fail();
 }
      cd = ParseAndTestAspects("cd-unknown");
      if (cd.isInline()) {
 Assert.fail();
 }
      cd = ParseAndTestAspects("attachment");
      if (cd.isInline()) {
 Assert.fail();
 }
    }

    private static String MakeQEncoding(String str) {
      byte[] bytes = com.upokecenter.util.DataUtilities.GetUtf8Bytes(str, false);
      StringBuilder sb = new StringBuilder();
      String hex = "0123456789ABCDEF";
      sb.append("=?utf-8?q?");
      for (int i = 0; i < bytes.length; ++i) {
        int b = ((int)bytes[i]) & 0xff;
        if (b == 0x32) {
          sb.append('_');
        } else if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
          (b >= '0' && b <= '9')) {
          sb.append((char)b);
        } else {
          sb.append('=');
          sb.append(hex.charAt((b >> 4) & 15));
          sb.append(hex.charAt(b & 15));
        }
      }
      sb.append("?=");
      return sb.toString();
    }

    private static String MakeRfc2231Encoding(String str) {
      byte[] bytes = com.upokecenter.util.DataUtilities.GetUtf8Bytes(str, false);
      StringBuilder sb = new StringBuilder();
      String hex = "0123456789ABCDEF";
      sb.append("utf-8''");
      for (int i = 0; i < bytes.length; ++i) {
        int b = ((int)bytes[i]) & 0xff;
        if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
          (b >= '0' && b <= '9')) {
          sb.append((char)b);
        } else {
          sb.append('%');
          sb.append(hex.charAt((b >> 4) & 15));
          sb.append(hex.charAt(b & 15));
        }
      }
      return sb.toString();
    }

    private static String RandomString(RandomGenerator rnd) {
      String ret = EncodingTest.RandomString(rnd);
      int ui = rnd.UniformInt(100);
      if (ui < 20) {
        ret = MakeQEncoding(ret);
      } else if (ui < 25) {
        ret = MakeRfc2231Encoding(ret);
      }
      return ret;
    }

    static void FailFilename(String filename, String str) {
      FailFilename(filename, str, null);
    }

    static void FailFilename(
      String filename,
      String newName,
      String extra) {
      String failstr = "original=" + EncodingTest.EscapeString(filename) +
        "\nfilename=" + EncodingTest.EscapeString(newName) + "\n" +
        "AssertGoodFilename(\"" + EncodingTest.EscapeString(filename) +
        "\");" + (((extra)==null || (extra).length()==0) ? "" : "\n" +
          extra);
      Assert.fail(failstr);
    }

    static void AssertGoodFilename(String filename) {
      String str = ContentDisposition.MakeFilename(filename);
      if (str.length() == 0 && filename.length() > 0) {
        FailFilename(filename, str);
      } else if (str.length() == 0) {
        return;
      }
      if (str == null || str.length() > 255) {
        FailFilename(filename, str);
      }
      if (str.charAt(str.length() - 1) == '.' || str.charAt(str.length() - 1) == '~') {
        FailFilename(filename, str);
      }
      String strLower = com.upokecenter.util.DataUtilities.ToLowerCaseAscii(str);
      boolean bracketDigit = str.charAt(0) == '{' && str.length() > 1 &&
        str.charAt(1) >= '0' && str.charAt(1) <= '9';
      boolean homeFolder = str.charAt(0) == '~' || str.charAt(0) == '-' || str.charAt(0) ==
        '$';
      boolean period = str.charAt(0) == '.';
      boolean beginEndSpace = str.charAt(0) == 0x20 || str.charAt(str.length() - 1) ==
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
      if (strLower.equals(
        "nul")) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.equals("clock$")) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.indexOf(
        "nul.") == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.equals(
        "prn")) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.indexOf(
        "prn.") == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.indexOf(
        "![") >= 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.equals(
        "aux")) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.indexOf(
        "aux.") == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.equals(
        "con")) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.indexOf(
        "con.") == 0) {
        {
          FailFilename(filename, str, strLower);
        }
      }
      if (strLower.length() == 4 || (strLower.length() > 4 && (strLower.charAt(4)
        == '.' || strLower.charAt(4) == ' '))) {
        if (strLower.indexOf(
          "lpt") == 0 && strLower.charAt(3) >= '0' &&
          strLower.charAt(3) <= '9') {
          {
            FailFilename(filename, str, strLower);
          }
        }
        if (strLower.indexOf(
          "com") == 0 && strLower.charAt(3) >= '0' &&
          strLower.charAt(3) <= '9') {
          {
            FailFilename(filename, str, strLower);
          }
        }
      }
      int bracketedText = str.indexOf('[');
      if (bracketedText >= 0) {
        bracketedText = str.indexOf(']',bracketedText);
        if (bracketedText >= 0) {
          FailFilename(filename, str, strLower);
        }
      }
      int i;
      for (i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
        if (c < 0x20 || (c >= 0x7f && c <= 0x9f) ||
          c == '%' || c == 0x2028 || c == 0x2029 ||
          c == '#' || c == ';' || c == '\'' || c == '&' ||
          c == '\\' || c == '/' || c == '*' || c == '?' || c == '|' ||
          c == ':' || c == '<' || c == '>' || c == '"' || c == '`' ||
          c == '$' || c == 0xa0 || c == 0x3000 || c == 0x180e || c == 0x1680 ||
          (c >= 0x2000 && c <= 0x200b) || c == 0x205f || c == 0x202f || c ==
          0xfeff || (c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <= 0xfdef)) {
          FailFilename(
            filename,
            str,
            "[" + EncodingTest.EscapeString("" + c) + "] index=" + i);
        }
        // Code points that decompose to "bad" characters
        if (c == 0x1fef) {
          FailFilename(
            filename,
            str,
            "[" + EncodingTest.EscapeString("" + c) + "] index=" + i);
        }
      }
      if (str.indexOf("\u0020\u0020") >= 0) {
        FailFilename(filename, str);
      }
      if (str.indexOf("\u0020\t") >= 0) {
        FailFilename(filename, str);
      }
      if (str.indexOf("\t\u0020") >= 0) {
        FailFilename(filename, str);
      }

      // Avoid space before and after last dot
      for (i = str.length() - 1; i >= 0; --i) {
        if (str.charAt(i) == '.') {
          boolean spaceAfter = i + 1 < str.length() && str.charAt(i + 1) == 0x20;
          boolean spaceBefore = i > 0 && str.charAt(i - 1) == 0x20;
          if (spaceAfter || spaceBefore) {
            FailFilename(filename, str);
          }
          break;
        }
      }
      boolean finalRet = NormalizerInput.IsNormalized(
          str,
          Normalization.NFC);
      if (!finalRet) {
        FailFilename(filename, str);
      }
      // Assert that MakeFilename is idempotent
      String newstr = ContentDisposition.MakeFilename(str);
      if (!newstr.equals(str)) {
        FailFilename(
          filename,
          str,
          "Not idempotent:\nnewname_=" + EncodingTest.EscapeString(newstr));
      }
    }

    @Test
    public void TestMakeFilenameSpecific() {
      for (String str : ResourceUtil.GetStrings("specificfiles")) {
        AssertGoodFilename(str);
      }
    }

    @Test(timeout = 200000)
    public void TestMakeFilename() {
      RandomGenerator rnd = new RandomGenerator(new XorShift128Plus(false));
      Assert.assertEquals(
        "",
        ContentDisposition.MakeFilename(null));
      for (int i = 0; i < 10000; ++i) {
        if (i % 1000 == 0) {
          System.out.println(i);
        }
        AssertGoodFilename(RandomString(rnd));
      }
      String[] filenames = ResourceUtil.GetStrings("filenames");
      // "d\ud800e", "d\ufffde",
      // "d\udc00e", "d\ufffde",
      // "my\ud800file\udc00name\ud800\udc00.txt",
      // "my\ufffdfile\ufffdname\ud800\udc00.txt",
      // "=?x-unknown?Q?file\ud800name?=", "file\ufffdname",
      for (int i = 0; i < filenames.length; i += 2) {
        String str = ContentDisposition.MakeFilename(
            filenames[i]);
        Assert.assertEquals(filenames[i], filenames[i + 1], str);
        AssertGoodFilename(filenames[i]);
        AssertGoodFilename(filenames[i + 1]);
      }
    }

    @Test
    public void TestParameters() {
      ContentDisposition mt =
        ParseAndTestAspects("inline;param1=value1;param2=value2");
      Map<String, String> parameters;
      parameters = mt.getParameters();
      if (!(parameters.containsKey("param1"))) {
 Assert.fail();
 }
      if (!(parameters.containsKey("param2"))) {
 Assert.fail();
 }
      Assert.assertEquals("value1", parameters.get("param1"));
      Assert.assertEquals("value2", parameters.get("param2"));
    }

    @Test
    public void TestParse() {
      try {
        ParseAndTestAspects(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      ContentDisposition mt;
      Map<String, String> parameters;
      mt = ParseAndTestAspects("inline;param1=\"value1\"");
      parameters = mt.getParameters();
      Assert.assertEquals("value1", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=utf-8'en'value3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1''valu%e72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1''valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%e72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%4E2");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u004e2", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=iso-8859-1'en'valu%4e2");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u004e2", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1*=utf-8''value2;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt = ParseAndTestAspects("inline;param1=dummy;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
          "inline;param1*0*=utf-8'en'val;param1*1*=ue4;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
          "inline;param1=dummy;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
          "inline;param1*=iso-8859-1''valu%e72;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
          "inline;param1=dummy;param1*=iso-8859-1''valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
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

    private static void TestPercentEncodingOne(String expected, String input) {
      ContentDisposition cd =
        ParseAndTestAspects("inline; filename*=utf-8''" + input);
      Assert.assertEquals(expected, cd.GetParameter("filename"));
    }

    // Parameters not conforming to RFC 2231, but
    // have names with asterisks
    static final String[] NoParams = {
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

    @Test
    public void TestParseIDB() {
      // NOTE: The following tests implementation-dependent behavior
      // since RFC 2231 doesn't provide for this case.
      ContentDisposition mt;
      Map<String, String> parameters;
      mt =

        ParseAndTestAspects(
          "inline;param=value1;param1*=utf-8''value2;param1*0=value3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
          "inline;param=value1;param1*0=value3;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
          "inline;param1*0=value3;param=value1;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
  "inline;param1*0*=utf8''val;param=value1;param1*=utf-8''value2;param1*1*=ue3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      for (String str : NoParams) {
        mt = ParseAndTestAspects("inline" + str, null);
        parameters = mt.getParameters();
        List<String> keys;
        keys = new ArrayList<String>(parameters.keySet());
        Assert.assertEquals(0, keys.size());
        Assert.assertEquals("inline", mt.getDispositionType());
      }
      String mts = "inline;charset*0=ab;charset*1*=iso-8859-1'en'xyz";
      mt = ParseAndTestAspects(mts);
      {
        String stringTemp = mt.GetParameter("charset");
        Assert.assertEquals(
          "ab",
          stringTemp);
      }
      Assert.assertEquals("inline", mt.getDispositionType());
      if (mt.GetParameter("charset*0") != null) {
        Assert.fail();
      }
      if (mt.GetParameter("charset*1*") != null) {
        Assert.fail();
      }
      mt =

        ParseAndTestAspects("inline;" +
          "\u0020charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz");
      {
        String stringTemp = mt.GetParameter("charset");
        Assert.assertEquals(
          "a b",
          stringTemp);
      }
      Assert.assertEquals("inline", mt.getDispositionType());
      if (mt.GetParameter("charset*0") != null) {
        Assert.fail();
      }
      if (mt.GetParameter("charset*1*") != null) {
        Assert.fail();
      }
    }

    @Test
    public void TestParseErrors() {
      for (String str : ResourceUtil.GetStrings("parseerrors")) {
        if ((ParseAndTestAspects("inline" + str, null))!=null) {
 Assert.fail(str);
 }
      }
      if (ParseAndTestAspects("inl/ine;y=z", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects("inline=x;y=z", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects("inline=x", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(":inline;y=z", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(":inline", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(";inline;y=z", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(";inline", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(" ;x=y", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(" ;x=y;z=w", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(" ;x=y", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(" ;x=y;z=w", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(" ;x=y", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects(" ;x=y;z=w", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects("??;x=y", null) != null) {
        Assert.fail();
      }
      if (ParseAndTestAspects("??;x=y;z=w", null) != null) {
        Assert.fail();
      }
    }

    @Test
    public void TestToString() {
      // not implemented yet
    }

    @Test
    public void TestToSingleLineString() {
      // not implemented yet
    }
  }
