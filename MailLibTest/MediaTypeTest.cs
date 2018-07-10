using System;
using System.Collections.Generic;
using NUnit.Framework;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class MediaTypeTest {
    private static IDictionary<string, string> MakeDict(params string[]
      keyvalues) {
      if (keyvalues == null) {
 throw new ArgumentNullException("keyvalues");
}
      if (keyvalues.Length % 2 != 0) {
 throw new ArgumentException("keyvalues");
}
      var dict = new Dictionary<string, string>();
      for (var i = 0; i < keyvalues.Length; i += 2) {
        dict.Add((string)keyvalues[i], keyvalues[i + 1]);
      }
      return dict;
    }

    private static IDictionary<string, string>[] testMediaTypes = new
      IDictionary<string, string>[] {
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
  MakeDict(
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
  MakeDict(
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
  MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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
      MakeDict(
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

    [Test]
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
      Assert.AreEqual(mt, mt2);
      Assert.IsFalse(mt.Equals(mt3));
      Assert.IsFalse(mt2.Equals(mt3));
      for (var i = 0; i < testMediaTypes.Length; ++i) {
        for (var j = 0; j < testMediaTypes.Length; ++j) {
          IDictionary<string, string> dictI = testMediaTypes[i];
          IDictionary<string, string> dictJ = testMediaTypes[j];
          TestCommon.AssertEqualsHashCode(
            MediaType.Parse(dictI["name"]),
            MediaType.Parse(dictJ["name"]));
        }
      }
    }
    [Test]
    public void TestGetCharset() {
      MediaType mt;
      mt = MediaType.Parse("text/plain");
      {
        {
          string stringTemp = mt.GetCharset();
          Assert.AreEqual(
          "us-ascii",
          stringTemp);
        }
      }
      mt = MediaType.Parse("text/vcard");
      {
        {
          string stringTemp = mt.GetCharset();
          Assert.AreEqual(
          "utf-8",
          stringTemp);
        }
      }
      mt = MediaType.Parse("text/x-unknown");
      Assert.AreEqual(String.Empty, mt.GetCharset());

      {
        string stringTemp = MediaType.Parse("text/plain").GetCharset();
        Assert.AreEqual(
          "us-ascii",
          stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("TEXT/PLAIN").GetCharset();
        Assert.AreEqual(
              "us-ascii",
              stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("TeXt/PlAiN").GetCharset();
        Assert.AreEqual(
                "us-ascii",
                stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/troff").GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("text/plain; CHARSET=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("text/plain; ChArSeT=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("text/plain; charset=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      // Note that MIME implicitly allows whitespace around the equal sign
      {
        string stringTemp = MediaType.Parse("text/plain; charset = UTF-8")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
 string stringTemp = MediaType.Parse("text/plain; charset (cmt) = (cmt) UTF-8")
      .GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/plain; charset='UTF-8'")
.GetCharset();
        Assert.AreEqual(
                  "'utf-8'",
                  stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/plain; charset=\"UTF-8\"")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp =
          MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp =
          MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"")
.GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
      {
    string stringTemp = MediaType.Parse("text/plain; foo='; charset=\"UTF-8\"")
   .GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
  string stringTemp = MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\"")
     .GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("text/plain; charset=\"UTF-\\8\"")
.GetCharset();
        Assert.AreEqual(
                  "utf-8",
                  stringTemp);
      }
      {
        string stringTemp = MediaType.Parse("nana").GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
      Assert.AreEqual(String.Empty, MediaType.Parse("text/xyz").GetCharset());
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("text/xyz;charset=UTF-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("text/xyz;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = String.Empty;
        object objectTemp2 = MediaType.Parse("text/xyz;chabset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("text/xml;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("text/plain;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        string stringTemp = MediaType.Parse("text/plain;chabset=utf-8")
      .GetCharset();
        Assert.AreEqual(
                  "us-ascii",
                  stringTemp);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("image/xml;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = String.Empty;
        object objectTemp2 = MediaType.Parse("image/xml;chabset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = "utf-8";
        object objectTemp2 = MediaType.Parse("image/plain;charset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
      {
        object objectTemp = String.Empty;
        object objectTemp2 = MediaType.Parse("image/plain;chabset=utf-8")
        .GetCharset();
        Assert.AreEqual(objectTemp, objectTemp2);
      }
    }
    [Test]
    public void TestGetParameter() {
      // not implemented yet
    }
    [Test]
    public void TestIsMultipart() {
foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict["name"]);
        Assert.AreEqual(dict["multipart"].Equals("1"), mt.IsMultipart);
      }
    }
    [Test]
    public void TestIsText() {
foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict["name"]);
        Assert.AreEqual(dict["text"].Equals("1"), mt.IsText);
      }
    }
    [Test]
    public void TestParameters() {
      MediaType mt =
          MediaType.Parse("text/example;param1=value1;param2=value2");
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
        MediaType.Parse(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      MediaType mt;
      IDictionary<string, string> parameters;
      mt = MediaType.Parse("text/example;param1=\"value1\"");
      parameters = mt.Parameters;
      Assert.AreEqual("value1", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=utf-8'en'value3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=iso-8859-1''valu%e72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=iso-8859-1''valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%e72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%E72");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%4E2");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u004e2", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%4e2");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u004e2", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1*=utf-8''value2;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt = MediaType.Parse("text/example;param1=dummy;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value2", parameters["param1"]);
      mt =

  MediaType.Parse("text/example;param1*0*=utf-8'en'val;param1*1*=ue4;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
      mt =

  MediaType.Parse("text/example;param1=dummy;param1*0*=utf-8'en'val;param1*1*=ue4");
      parameters = mt.Parameters;
      Assert.AreEqual("value4", parameters["param1"]);
mt =
  MediaType.Parse("text/example;param1*=iso-8859-1''valu%e72;param1=dummy");
      parameters = mt.Parameters;
      Assert.AreEqual("valu\u00e72", parameters["param1"]);
mt =
  MediaType.Parse("text/example;param1=dummy;param1*=iso-8859-1''valu%E72");
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

  MediaType.Parse("text/example;param=value1;param1*=utf-8''value2;param1*0=value3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  MediaType.Parse("text/example;param=value1;param1*0=value3;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  MediaType.Parse("text/example;param1*0=value3;param=value1;param1*=utf-8''value2");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
      mt =

  MediaType.Parse("text/example;param1*0*=utf8''val;param=value1;param1*=utf-8''value2;param1*1*=ue3");
      parameters = mt.Parameters;
      Assert.AreEqual("value3", parameters["param1"]);
if (MediaType.Parse("text/plain;param*xx=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*0xx=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*xx0=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*xx*=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*0xx*=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*xx0*=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*0*0=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*0*x=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*0*0*=value", null) != null) {
 Assert.Fail();
 }
if (MediaType.Parse("text/plain;param*0*x*=value", null) != null) {
 Assert.Fail();
 }

 if (
  MediaType.Parse(
  "text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz",
  null) != null) {
        Assert.Fail();
      }

 if (
  MediaType.Parse(
  "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz",
  null) != null) {
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

    private static void TestPercentEncodingOne(string expected, string input) {
      MediaType cd = MediaType.Parse("text/plain; filename*=utf-8''" + input);
      Assert.AreEqual(expected, cd.GetParameter("filename"));
    }

    [Test]
    public void TestSubType() {
foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict["name"]);
        Assert.AreEqual(
          dict["subtype"],
          mt.SubType);
      }
    }
    [Test]
    public void TestTopLevelType() {
foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict["name"]);
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
    public void TestTypeAndSubType() {
foreach (IDictionary<string, string> dict in testMediaTypes) {
        MediaType mt = MediaType.Parse(dict["name"]);
        Assert.AreEqual(
          dict["toplevel"] + "/" + dict["subtype"],
          mt.TypeAndSubType);
      }
    }
  }
}
