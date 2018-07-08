package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class MediaTypeTest {
    @Test
    public void TestEquals() {
      MediaType mt=MediaType.Parse("text/example;param1=value1;param2=value2");
      MediaType mt2=MediaType.Parse("text/example;param2=value2;param1=value1");
      MediaType mt3=MediaType.Parse("text/example;param1=value2;param2=value2");
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
        stringTemp); }
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
    @Test
    public void TestGetHashCode() {
      // not implemented yet
    }
    @Test
    public void TestGetParameter() {
      // not implemented yet
    }
    @Test
    public void TestIsMultipart() {
      // not implemented yet
      if (!(MediaType.Parse("multipart/alternative").isMultipart())) {
 Assert.fail();
 }
      if (MediaType.Parse("message/alternative").isMultipart()) {
 Assert.fail();
 }
      if (MediaType.Parse("font/otf").isMultipart()) {
 Assert.fail();
 }
    }
    @Test
    public void TestIsText() {
      // not implemented yet
    }
    @Test
    public void TestParameters() {
      MediaType mt=MediaType.Parse("text/example;param1=value1;param2=value2");
      Map<String, String> parameters;
      parameters = mt.getParameters();
      if (!(parameters.containsKey("param1"))) {
 Assert.fail();
 }
      if (!(parameters.containsKey("param2"))) {
 Assert.fail();
 }
      Assert.assertEquals("value1",parameters.get("param1"));
      Assert.assertEquals("value2",parameters.get("param2"));
    }
    @Test
    public void TestParse() {
      try {
 MediaType.Parse(null);
Assert.fail("Should have failed");
} catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      MediaType mt=MediaType.Parse("text/example;param1=\"value1\"");
      Map<String, String> parameters;
      parameters = mt.getParameters();
      Assert.assertEquals("value1",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=utf-8''value2");
      Assert.assertEquals("value2",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=utf-8'en'value3");
      Assert.assertEquals("value3",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*0*=utf-8'en'val;param1*1*=ue4");
      Assert.assertEquals("value4",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=iso-8859-1''valu%e72");
      Assert.assertEquals("valu\u00e72",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=iso-8859-1''valu%E72");
      Assert.assertEquals("valu\u00e72",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%e72");
      Assert.assertEquals("valu\u00e72",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%E72");
      Assert.assertEquals("valu\u00e72",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%4E2");
      Assert.assertEquals("valu\u004e2",parameters.get("param1"));
      mt=MediaType.Parse("text/example;param1*=iso-8859-1'en'valu%4e2");
      Assert.assertEquals("valu\u004e2",parameters.get("param1"));
    }
    @Test
    public void TestSubType() {
      // not implemented yet
    }
    @Test
    public void TestTopLevelType() {
      // not implemented yet
    }
    @Test
    public void TestToString() {
      // not implemented yet
    }
    @Test
    public void TestTypeAndSubType() {
      // not implemented yet
    }
  }
