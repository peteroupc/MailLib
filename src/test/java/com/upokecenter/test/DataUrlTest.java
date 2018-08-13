package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class DataUrlTest {
    private void TestMatchBasicNone(String[] langranges, String str) {
      List<String> r = LanguageTags.LanguageTagFilter(
        langranges,
        new String[] { str },
        false,
 false);
      Assert.assertEquals(0, r.size());
    }

    private void TestMatchBasicContained(String[] langranges, String str) {
      List<String> r = LanguageTags.LanguageTagFilter(
        langranges,
        new String[] { str },
        false,
 false);
      if (!(r.size() > 0)) {
 Assert.fail();
 }
    }
    @Test
    public void TestMatchLangTagsBasic() {
      String[] langranges = { "fr-ca", "es" };
      this.TestMatchBasicContained(langranges,"fr-ca");
      this.TestMatchBasicContained(langranges, "es");
      this.TestMatchBasicNone(langranges,"fr-cam");
      this.TestMatchBasicContained(langranges, "fr-ca-9999");
      this.TestMatchBasicNone(langranges,"fr-zxxx-ca");
      this.TestMatchBasicContained(langranges,"es-cam");
      this.TestMatchBasicContained(langranges, "es-ca-9999");
      this.TestMatchBasicContained(langranges,"es-zxxx-ca");
      langranges = new String[] { "fr-ca","*","es"};
      this.TestMatchBasicContained(langranges,"es");
      this.TestMatchBasicContained(langranges, "nl");
      langranges = new String[] { null, "*" };
    }

    private Message TestMailToOne(String s) {
      System.out.println(s);
      Message msg = DataUrl.MailToUrlMessage(s);
      System.out.println(msg);
      if (msg == null) {
 Assert.fail();
 }
      return msg;
    }

    static void TestDataUrlRoundTrip(String data) {
      MediaType mt = DataUrl.DataUrlMediaType(data);
      byte[] bytes = DataUrl.DataUrlBytes(data);
      if ((mt) == null) {
 Assert.fail(data);
 }
      if ((bytes) == null) {
 Assert.fail(data);
 }
      String data2 = DataUrl.MakeDataUrl(bytes, mt);
      MediaType mt2 = DataUrl.DataUrlMediaType(data2);
      byte[] bytes2 = DataUrl.DataUrlBytes(data2);
      Test.TestCommon.AssertByteArraysEqual(bytes, bytes2);
      Assert.assertEquals(data, mt, mt2);
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
