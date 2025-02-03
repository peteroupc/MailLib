package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class DataUrlTest {
    private void TestMatchBasicNone(String[] langranges, String str) {
      List<String> r = LanguageTags.LanguageTagFilter(
          Arrays.asList(langranges),
          Arrays.asList(str),
          false,
          false);
      Assert.assertEquals(0, r.size());
    }

    private void TestMatchBasicContained(String[] langranges, String str) {
      List<String> r = LanguageTags.LanguageTagFilter(
          Arrays.asList(langranges),
          Arrays.asList(str),
          false,
          false);
      if (!(r.size() > 0)) {
 Assert.fail();
 }
    }
    @Test
    public void TestMatchLangTagsBasic() {
      String[] langranges = { "fr-ca", "es" };
      this.TestMatchBasicContained(langranges, "fr-ca");
      this.TestMatchBasicContained(langranges, "es");
      this.TestMatchBasicNone(langranges, "fr-cam");
      this.TestMatchBasicContained(langranges, "fr-ca-9999");
      this.TestMatchBasicNone(langranges, "fr-zxxx-ca");
      this.TestMatchBasicContained(langranges, "es-cam");
      this.TestMatchBasicContained(langranges, "es-ca-9999");
      this.TestMatchBasicContained(langranges, "es-zxxx-ca");
      langranges = new String[] {
        "fr-ca", "*", "es",
      };
      this.TestMatchBasicContained(langranges, "es");
      this.TestMatchBasicContained(langranges, "nl");
      langranges = new String[] { null, "*" };
    }

    private Message TestMailToOne(String s) {
      System.out.println(s);
      Message msg = Message.FromMailtoUri(s);
      System.out.println(msg);
      if (msg == null) {
        Assert.fail();
      }
      return msg;
    }

    static void TestDataUrlRoundTrip(String data) {
      MediaType mt = DataUris.DataUriMediaType(data);
      byte[] bytes = DataUris.DataUriBytes(data);
      if ((mt) == null) {
 Assert.fail(data);
 }
      if ((bytes) == null) {
 Assert.fail(data);
 }
      String data2 = DataUris.MakeDataUri(bytes, mt);
      MediaType mt2 = DataUris.DataUriMediaType(data2);
      byte[] bytes2 = DataUris.DataUriBytes(data2);
      TestCommon.AssertByteArraysEqual(bytes, bytes2);
      Assert.assertEquals(data, mt, mt2);
    }

    @Test(timeout = 5000)
    public void TestMailTo() {
      Message msg;
      this.TestMailToOne("mailto:me@example.com");
      this.TestMailToOne("mailto:you@example.com?subject=current-issue");
      msg = this.TestMailToOne("mailto:you@example.com?body=x%20abcdefg-hijk");
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "x abcdefg-hijk",
          stringTemp);
      }
      msg = this.TestMailToOne(
          "mailto:you@example.com?body=x%20abcdefg-h%0d%0aijk");
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "x abcdefg-h\r\nijk",
          stringTemp);
      }
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
      {
        String stringTemp = msg.GetBodyString();
        Assert.assertEquals(
          "b\u00e7",
          stringTemp);
      }
      msg =

        this.TestMailToOne(
          "mailto:me@example.com?subject=%3D%3futf-8%3fQ%3fb%3dC3%3dA7%3f%3d");
      System.out.print(msg.GetHeader("subject"));
    }
  }
