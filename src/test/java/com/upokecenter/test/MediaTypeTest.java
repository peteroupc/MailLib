package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class MediaTypeTest {
    private static List<Map<String, String>> testMediaTypes =
      DictUtility.DictList(
      DictUtility.MakeDict(
        "name",
        "multipart/example",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multipart/example;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MULTIPART/example",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MULTIPART/example;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MuLtIpArT/example",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MuLtIpArT/example;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multipart/EXAMPLE",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multipart/EXAMPLE;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MuLtIpArT/EXAMPLE",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MuLtIpArT/EXAMPLE;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multipart/ExAmPlE",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multipart/ExAmPlE;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multipart/eXaMpLe",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multipart/eXaMpLe;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MuLtIpArT/eXaMpLe",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "MuLtIpArT/eXaMpLe;x=y",
        "toplevel",
        "multipart",
        "subtype",
        "example",
        "multipart",
        "1",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "multi-part/example",
        "toplevel",
        "multi-part",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "font/otf",
        "toplevel",
        "font",
        "subtype",
        "otf",
        "multipart",
        "0",
        "text",
        "0"),
        DictUtility.MakeDict(
          "name",
          "message/alternative",
          "toplevel",
          "message",
          "subtype",
          "alternative",
          "multipart",
          "0",
          "text",
          "0"),
          DictUtility.MakeDict(
            "name",
            "multipart/alternative",
            "toplevel",
            "multipart",
            "subtype",
            "alternative",
            "multipart",
            "1",
            "text",
            "0"),
      DictUtility.MakeDict(
        "name",
        "multi-part/example;x=y",
        "toplevel",
        "multi-part",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "texb/example",
        "toplevel",
        "texb",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "texb/example;x=y",
        "toplevel",
        "texb",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "0"),
      DictUtility.MakeDict(
        "name",
        "text/example",
        "toplevel",
        "text",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "1"),
      DictUtility.MakeDict(
        "name",
        "text/example;x=y",
        "toplevel",
        "text",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "1"),
      DictUtility.MakeDict(
        "name",
        "TEXT/example",
        "toplevel",
        "text",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "1"),
      DictUtility.MakeDict(
        "name",
        "TEXT/example;x=y",
        "toplevel",
        "text",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "1"),
      DictUtility.MakeDict(
        "name",
        "TexT/example",
        "toplevel",
        "text",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "1"),
      DictUtility.MakeDict(,
        "name",
        "TexT/example;x=y",
        "toplevel",
        "text",
        "subtype",
        "example",
        "multipart",
        "0",
        "text",
        "1")
);

    private static MediaType ParseAndTestAspects(String s) {
      MediaType mt = MediaType.Parse(s);
      if (mt == null) {
        TestAspects(mt);
      }
      return mt;
    }

    private static MediaType ParseAndTestAspects(String s, MediaType defvalue) {
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
      for (Map<String, String> dictI : testMediaTypes) {
        for (Map<String, String> dictJ : testMediaTypes) {
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
                 ParseAndTestAspects("text/plain; charset (cmt) = (cmt) UTF-8")
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
        String stringTemp = ParseAndTestAspects("text/plain; charset=\"UTF-8\"")
.GetCharset();
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
"\u0020charset=\"UTF-\\8\"")
   .GetCharset();
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

    public static final List<Map<String, String>>
          ValueTestParamTypes = DictUtility.DictList(
  DictUtility.MakeDict("params", ";filename=x.y", "filename", "x.y"),
  DictUtility.MakeDict("params", ";filename=\"cc\"", "filename", "cc"),
  DictUtility.MakeDict("params", ";filename = x.y", "filename", "x.y"),
  DictUtility.MakeDict("params", ";filename = \"cc\"", "filename", "cc"),
  DictUtility.MakeDict("params", ";filename= x.y", "filename", "x.y"),
  DictUtility.MakeDict("params", ";filename= \"cc\"", "filename", "cc"),
  DictUtility.MakeDict("params", ";filename =x.y", "filename", "x.y"),
  DictUtility.MakeDict("params", ";filename =\"cc\"", "filename", "cc"),
  DictUtility.MakeDict("params", ";filename=x.y ", "filename", "x.y"),
  DictUtility.MakeDict("params", ";filename=\"cc\" ", "filename", "cc"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"a\\" + "\u0020b\"",
    "filename",
    "a b"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"a\\" + "\tb\"",
    "filename",
    "a\tb"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"a\\" + "\\b\"",
    "filename",
    "a\\b"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"ccaaaaaaaaaaaaaaaaaaaa\"",
    "filename",
    "ccaaaaaaaaaaaaaaaaaaaa"),
  DictUtility.MakeDict(
  "params",
  ";filename=\"ccaaaaaaa,;=aaaaaaaaaaa\"",
  "filename",
  "ccaaaaaaa,;=aaaaaaaaaaa"),
  DictUtility.MakeDict(
  "params",
  ";filename=\"ccaaaaaaa,;e1=xxx;e2=yyy\"",
  "filename",
  "ccaaaaaaa,;e1=xxx;e2=yyy"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"cc\\a\\b\\c\\1\\2\\3\"",
    "filename",
    "ccabc123"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"cc\\\\\\'\\\"\\[\\]\"",
    "filename",
    "cc\\'\"[]"),
  DictUtility.MakeDict("params", ";filename=\"cc%\\ab\"", "filename", "cc%ab"),
  DictUtility.MakeDict("params", ";filename=\"\u00e7\"", "filename", "\u00e7"),
  DictUtility.MakeDict("params", ";filename=e's", "filename", "e's"),
  DictUtility.MakeDict("params", ";filename='es", "filename", "'es"),
  DictUtility.MakeDict("params", ";filename='es'", "filename", "'es'"),
  DictUtility.MakeDict(
    "params",
    ";filename=utf-8'en'example",
    "filename",
    "utf-8'en'example"),
  DictUtility.MakeDict(
    "params",
    ";filename=utf-8''example",
    "filename",
    "utf-8''example"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"%ab\u00e7\"",
    "filename",
    "%ab\u00e7"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"%ab\u00c2\u00a0\"",
    "filename",
    "%ab\u00c2\u00a0"),
  DictUtility.MakeDict("params", ";filename=\"cc%\\66\"", "filename", "cc%66"),
  DictUtility.MakeDict("params", ";filename=\"cc%xy\"", "filename", "cc%xy"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"cc\\\"x\\\"y\"",
    "filename",
    "cc\"x\"y"),
  DictUtility.MakeDict("params", ";FILENAME=x.y", "filename", "x.y"),
  DictUtility.MakeDict("params", ";FILENAME=\"cc\"", "filename", "cc"),
  DictUtility.MakeDict("params", ";FiLeNaMe=x.y", "filename", "x.y"),
  DictUtility.MakeDict("params", ";FiLeNaMe=\"cc\"", "filename", "cc"),
  DictUtility.MakeDict("params", ";fIlEnAmE=x.y", "filename", "x.y"),
  DictUtility.MakeDict("params", ";fIlEnAmE=\"cc\"", "filename", "cc"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"a\u0020\u0020b\"",
    "filename",
    "a\u0020\u0020b"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"a\u0020\tb\"",
    "filename",
    "a\u0020\tb"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"a\t\u0020b\"",
    "filename",
    "a\t\u0020b"),
  DictUtility.MakeDict("params", ";filename=\"a\t\tb\"", "filename", "a\t\tb"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"\u0020\u0020ab\"",
    "filename",
    "\u0020\u0020ab"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"\u0020\tab\"",
    "filename",
    "\u0020\tab"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"\t\u0020ab\"",
    "filename",
    "\t\u0020ab"),
  DictUtility.MakeDict("params", ";filename=\"\t\tab\"", "filename", "\t\tab"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"ab\u0020\u0020\"",
    "filename",
    "ab\u0020\u0020"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"ab\u0020\t\"",
    "filename",
    "ab\u0020\t"),
  DictUtility.MakeDict(
    "params",
    ";filename=\"ab\t\u0020\"",
    "filename",
    "ab\t\u0020"),
  DictUtility.MakeDict("params", ";filename=\"ab\t\t\"", "filename", "ab\t\t"),
  DictUtility.MakeDict("params", ";filename=\"\\\\ab\"", "filename", "\\ab"),
);

    @Test
    public void TestGetParameter() {
      for (Map<String, String> dict : ValueTestParamTypes) {
        MediaType mt = ParseAndTestAspects("x/x" + dict.get("params"));
        Assert.assertEquals(
          dict.get("filename"),
          mt.GetParameter("filename"));
      }
    }
    @Test
    public void TestIsMultipart() {
      for (Map<String, String> dict : testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        {
          Object objectTemp = dict.get("multipart").startsWith("1");
  Object objectTemp2 = mt.isMultipart();
  Assert.assertEquals(objectTemp, objectTemp2);
}
      }
    }
    @Test
    public void TestIsText() {
      for (Map<String, String> dict : testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Assert.assertEquals(dict.get("text").startsWith("1"),
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
      mt =
  ParseAndTestAspects("text/example;param1*0*=utf-8'en'val;param1*1*=ue4");
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

  ParseAndTestAspects("text/example;param1*0*=utf-8'en'val;param1*1*=ue4;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt =

  ParseAndTestAspects("text/example;param1=dummy;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt =
  ParseAndTestAspects("text/example;param1*=iso-8859-1''valu%e72;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt =
  ParseAndTestAspects("text/example;param1=dummy;param1*=iso-8859-1''valu%E72");
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

  ParseAndTestAspects("text/example;param=value1;param1*=utf-8''value2;param1*0=value3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

  ParseAndTestAspects("text/example;param=value1;param1*0=value3;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

  ParseAndTestAspects("text/example;param1*0=value3;param=value1;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

  ParseAndTestAspects("text/example;param1*0*=utf8''val;param=value1;param1*=utf-8''value2;param1*1*=ue3");
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
      mt =
  ParseAndTestAspects("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz");
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
      for (String str : ContentDispositionTest.ParseErrors) {
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
      for (Map<String, String> dict : testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Assert.assertEquals(
          dict.get("subtype"),
          mt.getSubType());
      }
    }
    @Test
    public void TestTopLevelType() {
      for (Map<String, String> dict : testMediaTypes) {
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
    @Test
    public void TestToSingleLineString() {
      for (Map<String, String> dict : testMediaTypes) {
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
      for (Map<String, String> dict : testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict.get("name"));
        Assert.assertEquals(
          dict.get("toplevel") + "/" + dict.get("subtype"),
          mt.getTypeAndSubType());
      }
    }
  }
