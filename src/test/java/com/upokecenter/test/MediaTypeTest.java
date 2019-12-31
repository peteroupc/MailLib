package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class MediaTypeTest {
    private static MediaType ParseAndTestAspects(String s) {
      MediaType mt = MediaType.Parse(s);
      if (mt == null) {
        TestAspects(mt);
      }
      return mt;
    }

    private static MediaType ParseAndTestAspects(String s,
      MediaType defvalue) {
      MediaType mt = MediaType.Parse(s, defvalue);
      if (mt == null) {
        TestAspects(mt);
      }
      return mt;
    }

    private static void TestAspects(MediaType mt) {
      if (mt == null) {
        return;
      }
      // Test round-tripping
      String str = mt.toString();
      MediaType mt2 = MediaType.Parse(str, null);
      if (mt2 == null) {
        Assert.fail();
      }
      Assert.assertEquals(str, mt2.toString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
      str = mt.ToSingleLineString();
      mt2 = MediaType.Parse(str, null);
      if (mt2 == null) {
        Assert.fail();
      }
      Assert.assertEquals(str, mt2.ToSingleLineString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
    }
    @Test
    public void TestEquals() {
      MediaType mt =
        ParseAndTestAspects("text/example;param1=value1;param2=value2");
      MediaType mt2 =
        ParseAndTestAspects("text/example;param2=value2;param1=value1");
      MediaType mt3 =
        ParseAndTestAspects("text/example;param1=value2;param2=value2");
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
      for (Map<String, String> dictI : ResourceUtil.GetDictList("mediatypes")) {
        for (Map<String, String> dictJ : ResourceUtil.GetDictList("mediatypes")) {
          TestCommon.AssertEqualsHashCode(
            ParseAndTestAspects(dictI.get("name")),
            ParseAndTestAspects(dictJ.get("name")));
        }
      }
    }
    @Test
    public void TestGetCharset() {
      MediaType mt;
      mt = ParseAndTestAspects("text/plain");
      {
        {
          String stringTemp = mt.GetCharset();
          Assert.assertEquals(
            "us-ascii",
            stringTemp);
        }
      }
      mt = ParseAndTestAspects("text/vcard");
      {
        {
          String stringTemp = mt.GetCharset();
          Assert.assertEquals(
            "utf-8",
            stringTemp);
        }
      }
      mt = ParseAndTestAspects("text/x-unknown");
      Assert.assertEquals("", mt.GetCharset());
      {
        String stringTemp = ParseAndTestAspects("text/plain").GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        String stringTemp = ParseAndTestAspects("TEXT/PLAIN").GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        String stringTemp = ParseAndTestAspects("TeXt/PlAiN").GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        String stringTemp = ParseAndTestAspects("text/troff").GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("text/plain; CHARSET=UTF-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("text/plain; ChArSeT=UTF-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("text/plain; charset=UTF-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      // Note that MIME implicitly allows whitespace around the equal sign
      {
        String stringTemp = ParseAndTestAspects("text/plain; charset = UTF-8")
          .GetCharset();
        Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp =
          ParseAndTestAspects("text/plain; charset(cmt) =(cmt) UTF-8")
          .GetCharset();
        Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        // NOTE: 'UTF-8' (with single quotes) is now treated as unknown
        String stringTemp = ParseAndTestAspects("text/plain; charset='UTF-8'")
          .GetCharset();
        Assert.assertEquals(
          "",
          stringTemp);
      }
      {
        String stringTemp = "text/plain; charset=\"UTF-8\"";
        stringTemp = ParseAndTestAspects(stringTemp).GetCharset();
        Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp =
          ParseAndTestAspects("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"")
          .GetCharset();
        Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp =
          ParseAndTestAspects("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"")
          .GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        String stringTemp =
          ParseAndTestAspects("text/plain; foo='; charset=\"UTF-8\"")
          .GetCharset();
        Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp =
          ParseAndTestAspects("text/plain; foo=bar; charset=\"UTF-8\"")
          .GetCharset();
        Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp = ParseAndTestAspects("text/plain;" +
            "\u0020charset=\"UTF-\\8\"").GetCharset();
        Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp = ParseAndTestAspects("nana").GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        Object objectTemp = "";
        Object objectTemp2 = ParseAndTestAspects("text/xyz")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("text/xyz;charset=UTF-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("text/xyz;charset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "";
        Object objectTemp2 = ParseAndTestAspects("text/xyz;chabset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("text/xml;charset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("text/plain;charset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        String stringTemp = ParseAndTestAspects("text/plain;chabset=utf-8")
          .GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("image/xml;charset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "";
        Object objectTemp2 = ParseAndTestAspects("image/xml;chabset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = ParseAndTestAspects("image/plain;charset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "";
        Object objectTemp2 = ParseAndTestAspects("image/plain;chabset=utf-8")
          .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
    }

    @Test
    public void TestGetParameter() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("paramtypes")) {
        MediaType mt = ParseAndTestAspects("x/x" + dict.get("params"));
        Assert.assertEquals(
          dict.get("filename"),
          mt.GetParameter("filename"));
      }
    }
    @Test
    public void TestIsMultipart() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("mediatypes")) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Object objectTemp = dict.get("multipart").equals("1");
        Object objectTemp2 = mt.isMultipart();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
    }
    @Test
    public void TestIsText() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("mediatypes")) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Assert.assertEquals(dict.get("text").equals("1"),
          mt.isText());
      }
    }
    @Test
    public void TestParameters() {
      MediaType mt =
        ParseAndTestAspects("text/example;param1=value1;param2=value2");
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
      MediaType mt;
      Map<String, String> parameters;
      mt = ParseAndTestAspects("text/example;param1=\"value1\"");
      parameters = mt.getParameters();
      Assert.assertEquals("value1", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=utf-8'en'value3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt = ParseAndTestAspects(
  "text/example;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1''valu%e72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1''valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%e72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%4E2");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u004e2", parameters.get("param1"));
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%4e2");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u004e2", parameters.get("param1"));
      mt =
        ParseAndTestAspects("text/example;param1*=utf-8''value2;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt =
        ParseAndTestAspects("text/example;param1=dummy;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
  "text/example;param1*0*=utf-8'en'val;param1*1*=ue4;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
  "text/example;param1=dummy;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt = ParseAndTestAspects(
  "text/example;param1*=iso-8859-1''valu%e72;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = ParseAndTestAspects(
  "text/example;param1=dummy;param1*=iso-8859-1''valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
    }

    @Test
    public void TestParseIDB() {
      // NOTE: The following tests implementation-dependent behavior
      // since RFC 2231 doesn't provide for this case.
      MediaType mt;
      Map<String, String> parameters;
      mt =

        ParseAndTestAspects(
  "text/example;param=value1;param1*=utf-8''value2;param1*0=value3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
  "text/example;param=value1;param1*0=value3;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
  "text/example;param1*0=value3;param=value1;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

        ParseAndTestAspects(
  "text/example;param1*0*=utf8''val;param=value1;param1*=utf-8''value2;param1*1*=ue3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      for (String str : ContentDispositionTest.NoParams) {
        mt = ParseAndTestAspects("x/y" + str, null);
        parameters = mt.getParameters();
        List<String> keys;
        keys = new ArrayList<String>(parameters.keySet());
        Assert.assertEquals(0, keys.size());
        Assert.assertEquals("x/y", mt.getTypeAndSubType());
      }
      String mts = "text/plain; charset*0=ab;charset*1*=iso-8859-1'en";
      mts += "'xyz";
      mt = ParseAndTestAspects(mts);
      {
        String stringTemp = mt.GetParameter("charset");
        Assert.assertEquals(
          "ab",
          stringTemp);
      }
      Assert.assertEquals("text/plain", mt.getTypeAndSubType());
      if (mt.GetParameter("charset*0") != null) {
        Assert.fail();
      }
      if (mt.GetParameter("charset*1*") != null) {
        Assert.fail();
      }
      mt =

        ParseAndTestAspects("text/plain;" +
          "\u0020charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz");
      {
        String stringTemp = mt.GetParameter("charset");
        Assert.assertEquals(
          "a b",
          stringTemp);
      }
      Assert.assertEquals("text/plain", mt.getTypeAndSubType());
      if (mt.GetParameter("charset*0") != null) {
        Assert.fail();
      }
      if (mt.GetParameter("charset*1*") != null) {
        Assert.fail();
      }
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

    @Test
    public void TestParseErrors() {
      for (String str : ResourceUtil.GetStrings("parseerrors")) {
        if ((ParseAndTestAspects("text/plain" + str, null))!=null) {
 Assert.fail(str);
 }
      }
    }

    private static void TestPercentEncodingOne(String expected, String input) {
      MediaType cd = ParseAndTestAspects("text/plain; filename*=utf-8''" +
          input);
      Assert.assertEquals(expected, cd.GetParameter("filename"));
    }

    @Test
    public void TestSubType() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("mediatypes")) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Assert.assertEquals(
          dict.get("subtype"),
          mt.getSubType());
      }
    }
    @Test
    public void TestTopLevelType() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("mediatypes")) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Assert.assertEquals(
          dict.get("toplevel"),
          mt.getTopLevelType());
      }
    }
    @Test
    public void TestToString() {
      // not implemented yet
    }

    private static void AssertHasSuffixTrue(MediaType mt, String s) {
      if (mt == null || !mt.HasStructuredSuffix(s)) {
         Assert.fail("mt=" + mt + ", s=" + s);
      }
    }

    private static void AssertHasSuffixFalse(MediaType mt, String s) {
      if (mt != null && mt.HasStructuredSuffix(s)) {
         Assert.fail("mt=" + mt + ", s=" + s);
      }
    }

    @Test
    public void TestHasStructuredSuffix() {
      MediaType mt;
      mt = MediaType.Parse("example/b+xml", null);
      AssertHasSuffixTrue(mt, "xml");
      mt = MediaType.Parse("example/b+xml", null);
      AssertHasSuffixTrue(mt, "XML");
      mt = MediaType.Parse("example/b+xml", null);
      AssertHasSuffixTrue(mt, "xMl");
      mt = MediaType.Parse("example/+xml", null);
      AssertHasSuffixFalse(mt, "xml");
      mt = MediaType.Parse("example/bxml", null);
      AssertHasSuffixFalse(mt, "xml");
      mt = MediaType.Parse("example/b-xml", null);
      AssertHasSuffixFalse(mt, "xml");
      mt = MediaType.Parse("example/xml", null);
      AssertHasSuffixFalse(mt, "xml");
      mt = MediaType.Parse("example/xm", null);
      AssertHasSuffixFalse(mt, "xml");
      mt = MediaType.Parse("example/x", null);
      AssertHasSuffixFalse(mt, "xml");
      mt = MediaType.Parse("example/b+XML", null);
      AssertHasSuffixTrue(mt, "xml");
      mt = MediaType.Parse("example/b+XML", null);
      AssertHasSuffixTrue(mt, "XML");
      mt = MediaType.Parse("example/bcd+xMl", null);
      AssertHasSuffixTrue(mt, "xml");
      mt = MediaType.Parse("example/+XML", null);
      AssertHasSuffixFalse(mt, "xml");
      mt = MediaType.Parse("example/b+xml", null);
      AssertHasSuffixFalse(mt, "xmc");
      mt = MediaType.Parse("example/b+xml", null);
      AssertHasSuffixFalse(mt, "gml");
    }

    @Test
    public void TestToSingleLineString() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("mediatypes")) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        String str = mt.ToSingleLineString();
        if (str.indexOf("\r") >= 0) {
 Assert.fail();
 }
        if (str.indexOf("\n") >= 0) {
 Assert.fail();
 }
      }
    }
    @Test
    public void TestTypeAndSubType() {
      for (Map<String, String> dict : ResourceUtil.GetDictList("mediatypes")) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Assert.assertEquals(
          dict.get("toplevel") + "/" + dict.get("subtype"),
          mt.getTypeAndSubType());
      }
    }
  }
