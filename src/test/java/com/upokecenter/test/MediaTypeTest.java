package com.upokecenter.test; import com.upokecenter.util.*;

import com.upokecenter.mail.*;
import org.junit.Assert;
import org.junit.Test;

  public class MediaTypeTest {
    @Test
    public void TestEquals() {
      // not implemented yet
    }
    @Test
    public void TestGetCharset() {
      MediaType mt;
      mt=MediaType.Parse("text/plain");
      {
{
String stringTemp = mt.GetCharset();
Assert.assertEquals(
"us-ascii",
stringTemp);
}
}
      mt=MediaType.Parse("text/vcard");
      {
{
String stringTemp = mt.GetCharset();
Assert.assertEquals(
"utf-8",
stringTemp);
}
}
      mt=MediaType.Parse("text/x-unknown");
      Assert.assertEquals("", mt.GetCharset());
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
