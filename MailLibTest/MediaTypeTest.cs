using System;
using System.Collections.Generic;
using NUnit.Framework;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class MediaTypeTest {
    private static IDictionary<string, string>[] testMediaTypes = new
      IDictionary<string, string>[] {
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

private static MediaType ParseAndTestAspects(string s) {
      MediaType mt = MediaType.Parse(s);
      if (mt == null) {
 TestAspects(mt);
}
      return mt;
    }

    private static MediaType ParseAndTestAspects(string s, MediaType defvalue) {
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
      string str = mt.ToString();
      MediaType mt2 = MediaType.Parse(str, null);
      if (mt2 == null) {
 Assert.Fail();
 }
      Assert.AreEqual(str, mt2.ToString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
      str = mt.ToSingleLineString();
      mt2 = MediaType.Parse(str, null);
      if (mt2 == null) {
 Assert.Fail();
 }
      Assert.AreEqual(str, mt2.ToSingleLineString());
      TestCommon.AssertEqualsHashCode(mt, mt2);
    }
    [Test]
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
      Assert.AreEqual(mt, mt2);
      Assert.IsFalse(mt.Equals(mt3));
      Assert.IsFalse(mt2.Equals(mt3));
      for (var i = 0; i < testMediaTypes.Length; ++i) {
        for (var j = 0; j < testMediaTypes.Length; ++j) {
          IDictionary<string, string> dictI = testMediaTypes[i];
          IDictionary<string, string> dictJ = testMediaTypes[j];
          TestCommon.AssertEqualsHashCode(
            ParseAndTestAspects(dictI["name"]),
            ParseAndTestAspects(dictJ["name"]));
        }
      }
    }
    [Test]
    public void TestGetCharset() {
      MediaType mt;
      mt = ParseAndTestAspects("text/plain");
      {
        {
          string stringTemp = mt.GetCharset();
          Assert.AreEqual(
          "us-ascii",
          stringTemp);
        }
      }
      mt = ParseAndTestAspects("text/vcard");
      {
        {
          string stringTemp = mt.GetCharset();
          Assert.AreEqual(
          "utf-8",
          stringTemp);
        }
      }
      mt = ParseAndTestAspects("text/x-unknown");
      Assert.AreEqual(String.Empty, mt.GetCharset());

      {
        string stringTemp = ParseAndTestAspects("text/plain").GetCharset();
        Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      {
        string stringTemp = ParseAndTestAspects("TEXT/PLAIN").GetCharset();
        Assert.AreEqual(
              "us-ascii",
              stringTemp);
      }
      {
        string stringTemp = ParseAndTestAspects("TeXt/PlAiN").GetCharset();
        Assert.AreEqual(
                "us-ascii",
                stringTemp);
      }
      {
        string stringTemp = ParseAndTestAspects("text/troff").GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("text/plain; CHARSET=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("text/plain; ChArSeT=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("text/plain; charset=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      // Note that MIME implicitly allows whitespace around the equal sign
      {
        string stringTemp = ParseAndTestAspects("text/plain; charset = UTF-8")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
 string stringTemp =
          ParseAndTestAspects("text/plain; charset (cmt) = (cmt) UTF-8")
             .GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp = ParseAndTestAspects("text/plain; charset='UTF-8'")
.GetCharset();
        Assert.AreEqual(
                  "'utf-8'",
                  stringTemp);
      }
      {
        string stringTemp = ParseAndTestAspects("text/plain; charset=\"UTF-8\"")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp =
          ParseAndTestAspects("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp =
          ParseAndTestAspects("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"")
.GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
      {
    string stringTemp =
          ParseAndTestAspects("text/plain; foo='; charset=\"UTF-8\"")
       .GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
  string stringTemp =
          ParseAndTestAspects("text/plain; foo=bar; charset=\"UTF-8\"")
           .GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
     string stringTemp = ParseAndTestAspects("text/plain; charset=\"UTF-\\8\"")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp = ParseAndTestAspects("nana").GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
   {
object objectTemp = String.Empty;
object objectTemp2 = ParseAndTestAspects("text/xyz")
.GetCharset();
Assert.AreEqual(objectTemp, objectTemp2);
}
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("text/xyz;charset=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("text/xyz;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = String.Empty;
        object objectTemp2 = ParseAndTestAspects("text/xyz;chabset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("text/xml;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("text/plain;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        string stringTemp = ParseAndTestAspects("text/plain;chabset=utf-8")
      .GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("image/xml;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = String.Empty;
        object objectTemp2 = ParseAndTestAspects("image/xml;chabset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = ParseAndTestAspects("image/plain;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = String.Empty;
        object objectTemp2 = ParseAndTestAspects("image/plain;chabset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }

    internal static IDictionary<string, string>[] ValueTestParamTypes = new
      IDictionary<string, string>[] {
DictUtility.MakeDict("params", ";filename=x.y", "filename","x.y"),
  DictUtility.MakeDict("params", ";filename=\"cc\"", "filename","cc"),
  DictUtility.MakeDict("params", ";filename = x.y", "filename","x.y"),
  DictUtility.MakeDict("params", ";filename = \"cc\"", "filename","cc"),
  DictUtility.MakeDict("params", ";filename= x.y", "filename","x.y"),
  DictUtility.MakeDict("params", ";filename= \"cc\"", "filename","cc"),
  DictUtility.MakeDict("params", ";filename =x.y", "filename","x.y"),
  DictUtility.MakeDict("params", ";filename =\"cc\"", "filename","cc"),
  DictUtility.MakeDict("params", ";filename=x.y ", "filename","x.y"),
  DictUtility.MakeDict("params", ";filename=\"cc\" ", "filename","cc"),
  DictUtility.MakeDict("params", ";filename=\"a\\" + "\u0020b\"",
"filename" ,"a b"),
  DictUtility.MakeDict("params", ";filename=\"a\\" + "\tb\"","filename","a\tb"),
  DictUtility.MakeDict("params", ";filename=\"a\\" + "\\b\"","filename","a\\b"),
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
  DictUtility.MakeDict("params", ";filename=\"cc%\\ab\"", "filename","cc%ab"),
  DictUtility.MakeDict("params", ";filename=\"\u00e7\"", "filename","\u00e7"),
  DictUtility.MakeDict("params", ";filename=e's", "filename","e's"),
  DictUtility.MakeDict("params", ";filename='es", "filename","'es"),
  DictUtility.MakeDict("params", ";filename='es'", "filename","'es'"),
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
  DictUtility.MakeDict("params", ";filename=\"cc%\\66\"", "filename","cc%66"),
  DictUtility.MakeDict("params", ";filename=\"cc%xy\"", "filename","cc%xy"),
  DictUtility.MakeDict(
  "params",
  ";filename=\"cc\\\"x\\\"y\"",
  "filename",
  "cc\"x\"y"),
  DictUtility.MakeDict("params", ";FILENAME=x.y", "filename","x.y"),
  DictUtility.MakeDict("params", ";FILENAME=\"cc\"", "filename","cc"),
  DictUtility.MakeDict("params", ";FiLeNaMe=x.y", "filename","x.y"),
  DictUtility.MakeDict("params", ";FiLeNaMe=\"cc\"", "filename","cc"),
  DictUtility.MakeDict("params", ";fIlEnAmE=x.y", "filename","x.y"),
  DictUtility.MakeDict("params", ";fIlEnAmE=\"cc\"", "filename","cc"),
  DictUtility.MakeDict("params", ";filename=\"\\\\ab\"", "filename","\\ab")
};

    [Test]
    public void TestGetParameter() {
      foreach (IDictionary<string, string> dict in ValueTestParamTypes) {
        MediaType mt = ParseAndTestAspects("x/x" + dict["params"]);
        Assert.AreEqual(
          dict["filename"],
          mt.GetParameter("filename"));
      }
    }
    [Test]
    public void TestIsMultipart() {
      foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict["name"]);
        Assert.AreEqual(dict["multipart"].Equals("1"), mt.IsMultipart);
      }
    }
    [Test]
    public void TestIsText() {
      foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict["name"]);
        Assert.AreEqual(dict["text"].Equals("1"), mt.IsText);
      }
    }
    [Test]
    public void TestParameters() {
      MediaType mt =
          ParseAndTestAspects("text/example;param1=value1;param2=value2");
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
      MediaType mt;
      IDictionary<string, string> parameters;
      mt = ParseAndTestAspects("text/example;param1=\"value1\"");
      parameters = mt.Parameters;
      Assert.AreEqual("value1", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=utf-8'en'value3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
 mt =
  ParseAndTestAspects("text/example;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1''valu%e72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1''valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%e72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%4E2");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u004e2", parameters["param1"]);
      mt = ParseAndTestAspects("text/example;param1*=iso-8859-1'en'valu%4e2");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u004e2", parameters["param1"]);
   mt =
  ParseAndTestAspects("text/example;param1*=utf-8''value2;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
   mt =
  ParseAndTestAspects("text/example;param1=dummy;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt =

  ParseAndTestAspects("text/example;param1*0*=utf-8'en'val;param1*1*=ue4;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt =

  ParseAndTestAspects("text/example;param1=dummy;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt =
  ParseAndTestAspects("text/example;param1*=iso-8859-1''valu%e72;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt =
  ParseAndTestAspects("text/example;param1=dummy;param1*=iso-8859-1''valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
    }

    [Test]
    public void TestParseIDB() {
      // NOTE: The following tests implementation-dependent behavior
      // since RFC 2231 doesn't provide for this case.
      MediaType mt;
      IDictionary<string, string> parameters;
      mt =

  ParseAndTestAspects("text/example;param=value1;param1*=utf-8''value2;param1*0=value3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  ParseAndTestAspects("text/example;param=value1;param1*0=value3;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  ParseAndTestAspects("text/example;param1*0=value3;param=value1;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  ParseAndTestAspects("text/example;param1*0*=utf8''val;param=value1;param1*=utf-8''value2;param1*1*=ue3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
foreach (var str in ContentDispositionTest.NoParams) {
        mt = ParseAndTestAspects("x/y" + str, null);
        parameters = mt.Parameters;
        IList<string> keys;
        keys = new List<string>(parameters.Keys);
        Assert.AreEqual(0, keys.Count);
        Assert.AreEqual("x/y", mt.TypeAndSubType);
      }
mt =
  ParseAndTestAspects("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz");
      {
string stringTemp = mt.GetParameter("charset");
Assert.AreEqual(
  "ab",
  stringTemp);
}
      Assert.AreEqual("text/plain", mt.TypeAndSubType);
      if (mt.GetParameter("charset*0") != null) {
 Assert.Fail();
 }
      if (mt.GetParameter("charset*1*") != null) {
 Assert.Fail();
 }
      mt =

  ParseAndTestAspects("text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz");
      {
string stringTemp = mt.GetParameter("charset");
Assert.AreEqual(
  "a b",
  stringTemp);
}
      Assert.AreEqual("text/plain", mt.TypeAndSubType);
      if (mt.GetParameter("charset*0") != null) {
 Assert.Fail();
 }
      if (mt.GetParameter("charset*1*") != null) {
 Assert.Fail();
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

    [Test]
    public void TestParseErrors() {
      foreach (string str in ContentDispositionTest.ParseErrors) {
        Assert.IsNull(ParseAndTestAspects("text/plain" + str, null), str);
      }
    }

    private static void TestPercentEncodingOne(string expected, string input) {
   MediaType cd = ParseAndTestAspects("text/plain; filename*=utf-8''" +
        input);
      Assert.AreEqual(expected, cd.GetParameter("filename"));
    }

    [Test]
    public void TestSubType() {
      foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict["name"]);
        Assert.AreEqual(
          dict["subtype"],
          mt.SubType);
      }
    }
    [Test]
    public void TestTopLevelType() {
      foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict["name"]);
        Assert.AreEqual(
          dict["toplevel"],
          mt.TopLevelType);
      }
    }
    [Test]
    public void TestToString() {
      // not implemented yet
    }
    [Test]
    public void TestToSingleLineString() {
      foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict["name"]);
        string str = mt.ToSingleLineString();
        Assert.IsFalse(str.IndexOf("\r") >= 0);
        Assert.IsFalse(str.IndexOf("\n") >= 0);
      }
    }
    [Test]
    public void TestTypeAndSubType() {
      foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = ParseAndTestAspects(dict["name"]);
        Assert.AreEqual(
          dict["toplevel"] + "/" + dict["subtype"],
          mt.TypeAndSubType);
      }
    }
  }
}
