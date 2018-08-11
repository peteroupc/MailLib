package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class DataUrlTest {
    private Message TestMailToOne(String s) {
      System.out.println(s);
      Message msg = DataUrl.MailToUrlMessage(s);
      System.out.println(msg);
      if (msg == null) {
 Assert.fail();
 }
      return msg;
    }

    private static void TestEmptyPathOne(String uri) {
      int[] iri = Test.URIUtility.splitIRI(uri);
      if (iri == null) {
 Assert.fail();
 }
      Assert.GreaterOrEqual(iri[4], 0);
      Assert.assertEquals(iri[4], iri[5]);
    }

    @Test
    public void TestEmptyPathURIs() {
      TestEmptyPathOne("s://h");
      TestEmptyPathOne("s://h?x");
      TestEmptyPathOne("s://h#x");
      TestEmptyPathOne("//h");
      TestEmptyPathOne("//h?x");
      TestEmptyPathOne("//h#x");
      TestEmptyPathOne("s://");
      TestEmptyPathOne("s://?x");
      TestEmptyPathOne("s://#x");
      TestEmptyPathOne("s:");
      TestEmptyPathOne("s:?x");
      TestEmptyPathOne("s:#x");
    }
    @Test
    public void TestMailTo() {
      Message msg;
      this.TestMailToOne("mailto:me@example.com");
      this.TestMailToOne("mailto:you@example.com?subject=current-issue");
 msg =
  this.TestMailToOne("mailto:you@example.com?body=x%20abcdefg-hijk");
      Assert.assertEquals("x abcdefg-hijk", msg.getBodyString());
      msg =
  this.TestMailToOne("mailto:you@example.com?body=x%20abcdefg-h%0d%0aijk");
      Assert.assertEquals("x abcdefg-h\r\nijk", msg.getBodyString());
      // ----
      msg = this.TestMailToOne("mailto:e%25f@m.example");
      {
String stringTemp = msg.GetHeader("to");
Assert.assertEquals(
  "e%f@m.example",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:e%26f@m.example");
      {
String stringTemp = msg.GetHeader("to");
Assert.assertEquals(
  "e&f@m.example",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:e%3ff@m.example");
      {
String stringTemp = msg.GetHeader("to");
Assert.assertEquals(
  "e?f@m.example",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:%22e%3ff%22@m.example");
      Assert.assertEquals("\"e?f\"@m.example", msg.GetHeader("to"));
      msg = this.TestMailToOne("mailto:%22e%40f%22@m.example");
      Assert.assertEquals("\"e@f\"@m.example", msg.GetHeader("to"));
      msg = this.TestMailToOne("mailto:%22e%5c%5c%40%5c%22f%22@m.example");
      Assert.assertEquals("\"e\\\\@\\\"f\"@m.example", msg.GetHeader("to"));
      msg = this.TestMailToOne("mailto:%22e'f%22@m.example");
      Assert.assertEquals("\"e'f\"@m.example", msg.GetHeader("to"));
      List<NamedAddress> toaddrs = msg.GetAddresses("to");
      System.out.println(toaddrs.get(0));
      msg = this.TestMailToOne("mailto:me@b%c3%a8.example");
      {
String stringTemp = msg.GetHeader("to");
Assert.assertEquals(
  "me@b\u00e8.example",
  stringTemp);
}
      List<NamedAddress> toaddr = msg.GetAddresses("to");
      System.out.println(toaddr.get(0));
      msg = this.TestMailToOne("mailto:me@example.com?subject=b%c3%a7");
      {
String stringTemp = msg.GetHeader("subject");
Assert.assertEquals(
  "b\u00e7",
  stringTemp);
}
      msg = this.TestMailToOne("mailto:me@example.com?body=b%c3%a7");
      Assert.assertEquals("b\u00e7", msg.getBodyString());
      msg =

  this.TestMailToOne("mailto:me@example.com?subject=%3D%3futf-8%3fQ%3fb%3dC3%3dA7%3f%3d");
      System.out.print(msg.GetHeader("subject"));
    }
  }
