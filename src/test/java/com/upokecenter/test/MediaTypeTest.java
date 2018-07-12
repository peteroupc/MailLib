package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class MediaTypeTest {
    private static Map<String, String>[] testMediaTypes = new
      Map<String, String>[] {
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
      DictUtility.MakeDict(
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
    };

    @Test
    public void TestEquals() {
      MediaType mt =
          MediaType.Parse("text/example;param1=value1;param2=value2");
      MediaType mt2 =
           MediaType.Parse("text/example;param2=value2;param1=value1");
      MediaType mt3 =
           MediaType.Parse("text/example;param1=value2;param2=value2");
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
      for (int i = 0; i < testMediaTypes.length; ++i) {
        for (int j = 0; j < testMediaTypes.length; ++j) {
          Map<String, String> dictI = testMediaTypes.get(i);
          Map<String, String> dictJ = testMediaTypes.get(j);
          TestCommon.AssertEqualsHashCode(
            MediaType.Parse(dictI.get("name")),
            MediaType.Parse(dictJ.get("name")));
        }
      }
    }
    @Test
    public void TestGetCharset() {
      MediaType mt;
      mt = MediaType.Parse("text/plain");
      {
        {
          String stringTemp = mt.GetCharset();
          Assert.assertEquals(
          "us-ascii",
          stringTemp);
        }
      }
      mt = MediaType.Parse("text/vcard");
      {
        {
          String stringTemp = mt.GetCharset();
          Assert.assertEquals(
          "utf-8",
          stringTemp);
        }
      }
      mt = MediaType.Parse("text/x-unknown");
      Assert.assertEquals("", mt.GetCharset());

      {
        String stringTemp = MediaType.Parse("text/plain").GetCharset();
        Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("TEXT/PLAIN").GetCharset();
        Assert.assertEquals(
              "us-ascii",
              stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("TeXt/PlAiN").GetCharset();
        Assert.assertEquals(
                "us-ascii",
                stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("text/troff").GetCharset();
        Assert.assertEquals(
                  "us-ascii",
                  stringTemp);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("text/plain; CHARSET=UTF-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("text/plain; ChArSeT=UTF-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("text/plain; charset=UTF-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      // Note that MIME implicitly allows whitespace around the equal sign
      {
        String stringTemp = MediaType.Parse("text/plain; charset = UTF-8")
.GetCharset();
        Assert.assertEquals(
                  "utf-8",
                  stringTemp);
      }
      {
 String stringTemp = MediaType.Parse("text/plain; charset (cmt) = (cmt) UTF-8")
             .GetCharset();
        Assert.assertEquals(
                  "utf-8",
                  stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("text/plain; charset='UTF-8'")
.GetCharset();
        Assert.assertEquals(
                  "'utf-8'",
                  stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("text/plain; charset=\"UTF-8\"")
.GetCharset();
        Assert.assertEquals(
                  "utf-8",
                  stringTemp);
      }
      {
        String stringTemp =
          MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"")
.GetCharset();
        Assert.assertEquals(
                  "utf-8",
                  stringTemp);
      }
      {
        String stringTemp =
          MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"")
.GetCharset();
        Assert.assertEquals(
                  "us-ascii",
                  stringTemp);
      }
      {
    String stringTemp = MediaType.Parse("text/plain; foo='; charset=\"UTF-8\"")
       .GetCharset();
        Assert.assertEquals(
                  "utf-8",
                  stringTemp);
      }
      {
  String stringTemp = MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\"")
           .GetCharset();
        Assert.assertEquals(
                  "utf-8",
                  stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("text/plain; charset=\"UTF-\\8\"")
.GetCharset();
        Assert.assertEquals(
                  "utf-8",
                  stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("nana").GetCharset();
        Assert.assertEquals(
                  "us-ascii",
                  stringTemp);
      }
      Assert.assertEquals("", MediaType.Parse("text/xyz").GetCharset());
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("text/xyz;charset=UTF-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("text/xyz;charset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "";
        Object objectTemp2 = MediaType.Parse("text/xyz;chabset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("text/xml;charset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("text/plain;charset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        String stringTemp = MediaType.Parse("text/plain;chabset=utf-8")
      .GetCharset();
        Assert.assertEquals(
                  "us-ascii",
                  stringTemp);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("image/xml;charset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "";
        Object objectTemp2 = MediaType.Parse("image/xml;chabset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "utf-8";
        Object objectTemp2 = MediaType.Parse("image/plain;charset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
      {
        Object objectTemp = "";
        Object objectTemp2 = MediaType.Parse("image/plain;chabset=utf-8")
        .GetCharset();
        Assert.assertEquals(objectTemp, objectTemp2);
      }
    }
    static Map<String, String>[] testParamTypes = new
      Map<String, String>[] {
DictUtility.MakeDict("params",";filename=x.y","filename","x.y"),
  DictUtility.MakeDict("params",";filename=\"cc\"","filename","cc"),
  DictUtility.MakeDict("params",";filename    =    x.y","filename","x.y"),
  DictUtility.MakeDict("params",";filename    =    \"cc\"","filename","cc"),
  DictUtility.MakeDict("params",";filename=    x.y","filename","x.y"),
  DictUtility.MakeDict("params",";filename=    \"cc\"","filename","cc"),
  DictUtility.MakeDict("params",";filename    =x.y","filename","x.y"),
  DictUtility.MakeDict("params",";filename    =\"cc\"","filename","cc"),
  DictUtility.MakeDict("params",";filename=x.y    ","filename","x.y"),
  DictUtility.MakeDict("params",";filename=\"cc\"    ","filename","cc"),
  DictUtility.MakeDict("params" ,";filename=\"ccaaaaaaaaaaaaaaaaaaaa\"",
"filename" ,"ccaaaaaaaaaaaaaaaaaaaa"),
  DictUtility.MakeDict("params" ,";filename=\"ccaaaaaaa,;=aaaaaaaaaaa\"",
"filename" ,"ccaaaaaaa,;=aaaaaaaaaaa"),
  DictUtility.MakeDict("params" ,";filename=\"ccaaaaaaa,;e1=xxx;e2=yyy\"",
"filename" ,"ccaaaaaaa,;e1=xxx;e2=yyy"),
  DictUtility.MakeDict("params" ,";filename=\"cc\\a\\b\\c\\1\\2\\3\"",
"filename" ,"ccabc123"),
  DictUtility.MakeDict("params" ,";filename=\"cc\\\\\\'\\\"\\[\\]\"",
"filename" ,"cc\\'\"[]"),
  DictUtility.MakeDict("params",";filename=\"cc%\\ab\"","filename","cc%ab"),
  DictUtility.MakeDict("params",";filename=\"\u00e7\"","filename","\u00e7"),
  DictUtility.MakeDict("params",";filename=e's","filename","e's"),
  DictUtility.MakeDict("params",";filename='es","filename","'es"),
  DictUtility.MakeDict("params",";filename='es'","filename","'es'"),
  DictUtility.MakeDict("params" ,";filename=utf-8'en'example" ,"filename",
"utf-8'en'example"),
  DictUtility.MakeDict("params" ,";filename=utf-8''example" ,"filename",
"utf-8''example"),
  DictUtility.MakeDict("params",";filename=\"%ab\u00e7\"","filename","%ab\u00e7"),
  DictUtility.MakeDict("params" ,";filename=\"%ab\u00c2\u00a0\"" ,"filename",
"%ab\u00c2\u00a0"),
  DictUtility.MakeDict("params",";filename=\"cc%\\66\"","filename","cc%66"),
  DictUtility.MakeDict("params",";filename=\"cc%xy\"","filename","cc%xy"),
  DictUtility.MakeDict("params" ,";filename=\"cc\\\"x\\\"y\"" ,"filename",
"cc\"x\"y"),
  DictUtility.MakeDict("params",";FILENAME=x.y","filename","x.y"),
  DictUtility.MakeDict("params",";FILENAME=\"cc\"","filename","cc"),
  DictUtility.MakeDict("params",";FiLeNaMe=x.y","filename","x.y"),
  DictUtility.MakeDict("params",";FiLeNaMe=\"cc\"","filename","cc"),
  DictUtility.MakeDict("params",";fIlEnAmE=x.y","filename","x.y"),
  DictUtility.MakeDict("params",";fIlEnAmE=\"cc\"","filename","cc"),
  DictUtility.MakeDict("params",";filename=\"\\\\ab\"","filename","\\ab")
// DictUtility.MakeDict("params",";notfilename=x.y","filename",null),
  // DictUtility.MakeDict("params",";notfilename=\"cc\"","filename",null),
  };

    @Test
    public void TestGetParameter() {
      foreach (Map<String, String> dict in testParamTypes) {
        MediaType mt = MediaType.Parse("x/x"+ dict.get("params"));
        Assert.assertEquals(
          dict.get("filename"),
          mt.GetParameter("filename"));
      }
    }
    @Test
    public void TestIsMultipart() {
      foreach (Map<String, String> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict.get("name"));
        Assert.assertEquals(dict.get("multipart").equals("1"), mt.isMultipart());
      }
    }
    @Test
    public void TestIsText() {
      foreach (Map<String, String> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict.get("name"));
        Assert.assertEquals(dict.get("text").equals("1"), mt.isText());
      }
    }
    @Test
    public void TestParameters() {
      MediaType mt =
          MediaType.Parse("text/example;param1=value1;param2=value2");
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
      // TODO: Consider simply ignoring the
      // charset parameter in these two cases
      if ((MediaType.Parse("x/x; charset*='i-unknown'utf-8", null)) != null) {
 Assert.fail();
 }
 if ((MediaType.Parse("x/x; charset*=us-ascii'i-unknown'utf-8", null)) !=
        null) {
 Assert.fail();
 }
      try {
        MediaType.Parse(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      MediaType mt;
      Map<String, String> parameters;
      mt = MediaType.Parse("text/example;param1=\"value1\"");
      parameters = mt.getParameters();
      Assert.assertEquals("value1", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=utf-8'en'value3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=iso-8859-1''valu%e72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=iso-8859-1''valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%e72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%E72");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%4E2");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u004e2", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%4e2");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u004e2", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1*=utf-8''value2;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt = MediaType.Parse("text/example;param1=dummy;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value2", parameters.get("param1"));
      mt =

  MediaType.Parse("text/example;param1*0*=utf-8'en'val;param1*1*=ue4;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt =

  MediaType.Parse("text/example;param1=dummy;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.getParameters();
      Assert.assertEquals("value4", parameters.get("param1"));
      mt =
  MediaType.Parse("text/example;param1*=iso-8859-1''valu%e72;param1=dummy");
      parameters = mt.getParameters();
      Assert.assertEquals("valu\u00e72", parameters.get("param1"));
      mt =
  MediaType.Parse("text/example;param1=dummy;param1*=iso-8859-1''valu%E72");
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

  MediaType.Parse("text/example;param=value1;param1*=utf-8''value2;param1*0=value3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

  MediaType.Parse("text/example;param=value1;param1*0=value3;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

  MediaType.Parse("text/example;param1*0=value3;param=value1;param1*=utf-8''value2");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      mt =

  MediaType.Parse("text/example;param1*0*=utf8''val;param=value1;param1*=utf-8''value2;param1*1*=ue3");
      parameters = mt.getParameters();
      Assert.assertEquals("value3", parameters.get("param1"));
      if (MediaType.Parse("text/plain;param*xx=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*0xx=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*xx0=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*xx*=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*0xx*=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*xx0*=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*0*0=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*0*x=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*0*0*=value", null) != null) {
        Assert.fail();
      }
      if (MediaType.Parse("text/plain;param*0*x*=value", null) != null) {
        Assert.fail();
      }

      if (
       MediaType.Parse(
       "text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz",
       null) != null) {
        Assert.fail();
      }

      if (
       MediaType.Parse(
       "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz",
       null) != null) {
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
        Assert.IsNull(MediaType.Parse("text/plain"+ str,null), str);
      }
    }

    private static void TestPercentEncodingOne(String expected, String input) {
      MediaType cd = MediaType.Parse("text/plain; filename*=utf-8''" + input);
      Assert.assertEquals(expected, cd.GetParameter("filename"));
    }

    @Test
    public void TestSubType() {
      foreach (Map<String, String> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict.get("name"));
        Assert.assertEquals(
          dict.get("subtype"),
          mt.getSubType());
      }
    }
    @Test
    public void TestTopLevelType() {
      foreach (Map<String, String> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict.get("name"));
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
      foreach (Map<String, String> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict.get("name"));
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
      foreach (Map<String, String> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict.get("name"));
        Assert.assertEquals(
          dict.get("toplevel") + "/" + dict.get("subtype"),
          mt.getTypeAndSubType());
      }
    }
  }
