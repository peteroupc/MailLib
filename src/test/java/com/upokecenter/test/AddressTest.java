package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class AddressTest {
    private static void TestParseLocalPart(String str, String expected) {
      NamedAddress na = new NamedAddress("b <" + str + "@example.com>");
      Address addr = na.getAddress();
      Assert.assertEquals(expected, addr.getLocalPart());
      addr = new Address(str + "@example.com");
      Assert.assertEquals(expected, addr.getLocalPart());
    }

    private static void TestParseLocalPartNAOnly(String str, String expected) {
      NamedAddress na = new NamedAddress("b <" + str + "@example.com>");
      Address addr = na.getAddress();
      Assert.assertEquals(expected, addr.getLocalPart());
    }

    private static void TestParseDomain(String str, String expected) {
      NamedAddress na = new NamedAddress("b <example@" + str + ">");
      Address addr = na.getAddress();
      Assert.assertEquals(expected, addr.getDomain());
      addr = new Address("example@" + str);
      Assert.assertEquals(expected, addr.getDomain());
    }

    private static void TestParseDomainNAOnly(String str, String expected) {
      NamedAddress na = new NamedAddress("b <example@" + str + ">");
      Address addr = na.getAddress();
      Assert.assertEquals(expected, addr.getDomain());
    }

    @Test
    public void TestConstructor() {
      try {
        Assert.assertEquals(null, new Address("local=domain.example"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("local@"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        {
          Object objectTemp = null;
Object objectTemp2 = new Address(EncodingTest.Repeat("local", 200) +
          "@example.com");
Assert.assertEquals(objectTemp, objectTemp2);
}
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("lo,cal@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestDomain() {
      Address addr = new Address("local.local@example.com");
      Assert.assertEquals("example.com", addr.getDomain());
      TestParseDomain("x", "x");
      TestParseDomain("x.example", "x.example");
      TestParseDomain(
        "x.example\ud800\udc00.example.com",
        "x.example\ud800\udc00.example.com");
      TestParseDomain("x.example.com", "x.example.com");
      TestParseDomainNAOnly("(comment1) x (comment2)", "x");
      TestParseDomainNAOnly(
  "(comment1) example (comment2) . (comment3) com",
  "example.com");
      TestParseDomainNAOnly(
        "(co(mme)nt1) example (comment2) . (comment3) com",
        "example.com");
      TestParseDomainNAOnly("(comment1) [x] (comment2)", "[x]");
      TestParseDomainNAOnly("(comment1) [a.b.c.d] (comment2)", "[a.b.c.d]");
      TestParseDomain("[]", "[]");
      TestParseDomainNAOnly("[a .\r\n b. c.d ]", "[a.b.c.d]");
    }
    @Test
    public void TestLocalPart() {
      Address addr = new Address("local.local@example.com");
      Assert.assertEquals("local.local", addr.getLocalPart());
      addr = new Address("x!y!z!example@example.com");
      Assert.assertEquals("x!y!z!example", addr.getLocalPart());
      addr = new Address("x%y%z%example@example.com");
      Assert.assertEquals("x%y%z%example", addr.getLocalPart());
      addr = new Address("x!y%z!example@example.com");
      Assert.assertEquals("x!y%z!example", addr.getLocalPart());

      TestParseLocalPart("x", "x");
      TestParseLocalPart("x!y!z", "x!y!z");
      TestParseLocalPart("\"" + "\"", "");
      TestParseLocalPart("x.example", "x.example");
      TestParseLocalPart(
        "x.example\ud800\udc00.example.com",
        "x.example\ud800\udc00.example.com");
      TestParseLocalPart("x.example.com", "x.example.com");
      TestParseLocalPart("\"(not a comment)\"", "(not a comment)");
      TestParseLocalPartNAOnly("(comment1) x (comment2)", "x");
      TestParseLocalPartNAOnly("(comment1)x(comment2)", "x");
      TestParseLocalPartNAOnly(
  "(comment1) example (comment2) . (comment3) com",
  "example.com");
      TestParseLocalPartNAOnly(
        "(com(plex)comment) example (comment2) . (comment3) com",
        "example.com");
    }
    @Test(timeout = 5000)
    public void TestToString() {
      Address addr = new Address("local.local@example.com");
      {
        String stringTemp = addr.toString();
Assert.assertEquals(
  "local.local@example.com",
  stringTemp);
}
      addr = new Address("local-local@example.com");
      {
        String stringTemp = addr.toString();
        Assert.assertEquals(
          "local-local@example.com",
          stringTemp);
      }
    }
  }
