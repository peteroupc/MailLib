package com.upokecenter.test; import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class MediaTypeTest {
    @Test
    public void TestEquals() {
      // not implemented yet
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
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("text/plain; CHARSET=UTF-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("text/plain; ChArSeT=UTF-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("text/plain; charset=UTF-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      // Note that MIME implicitly allows whitespace around the equal sign
      {
        String stringTemp = MediaType.Parse("text/plain; charset = UTF-8"
).GetCharset();
Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
  String stringTemp = MediaType.Parse("text/plain; charset (cmt) = (cmt) UTF-8"
).GetCharset();
Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("text/plain; charset='UTF-8'"
).GetCharset();
Assert.assertEquals(
          "'utf-8'",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("text/plain; charset=\"UTF-8\""
).GetCharset();
Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp =
          MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\""
).GetCharset();
Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp =
          MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\""
).GetCharset();
Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
     String stringTemp = MediaType.Parse("text/plain; foo='; charset=\"UTF-8\""
).GetCharset();
Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
   String stringTemp = MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\""
).GetCharset();
Assert.assertEquals(
          "utf-8",
          stringTemp);
      }
      {
        String stringTemp = MediaType.Parse("text/plain; charset=\"UTF-\\8\""
).GetCharset();
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
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("text/xyz;charset=UTF-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("text/xyz;charset=utf-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "" ;
Object objectTemp2 = MediaType.Parse("text/xyz;chabset=utf-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("text/xml;charset=utf-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("text/plain;charset=utf-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
  String stringTemp = MediaType.Parse("text/plain;chabset=utf-8"
).GetCharset();
Assert.assertEquals(
          "us-ascii",
          stringTemp);
      }
      {
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("image/xml;charset=utf-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "" ;
Object objectTemp2 = MediaType.Parse("image/xml;chabset=utf-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "utf-8" ;
Object objectTemp2 = MediaType.Parse("image/plain;charset=utf-8"
).GetCharset();
Assert.assertEquals(objectTemp, objectTemp2);
}
      {
Object objectTemp = "" ;
Object objectTemp2 = MediaType.Parse("image/plain;chabset=utf-8"
).GetCharset();
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
    }
    @Test
    public void TestIsText() {
      // not implemented yet
    }
    @Test
    public void TestParameters() {
      // not implemented yet
    }
    @Test
    public void TestParse() {
      // not implemented yet
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
