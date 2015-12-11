package com.upokecenter.test; import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class AddressTest {
    private static void TestParseLocalPart(String str, String expected) {
      NamedAddress na = new NamedAddress("b <" + str + "@example.com>");
      Address addr = na.getAddress();
      Assert.assertEquals(expected, addr.getLocalPart());
    }

    private static void TestParseDomain(String str, String expected) {
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
System.out.print("");
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("local@"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
System.out.print("");
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address(EncodingTest.Repeat("local", 200) +
          "@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
System.out.print("");
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("lo,cal@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
System.out.print("");
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestDomain() {
      Address addr = new Address("local.local@example.com");
      Assert.assertEquals("example.com",addr.getDomain());
      TestParseDomain("x", "x");
      TestParseDomain("x.example", "x.example");
      TestParseDomain("x.example\ud800\udc00.example.com",
                    "x.example\ud800\udc00.example.com");
      TestParseDomain("x.example.com", "x.example.com");
      TestParseDomain("(comment1) x (comment2)", "x");
      TestParseDomain("(comment1) example (comment2) . (comment3) com",
                    "example.com");
      TestParseDomain("(comment1) [x] (comment2)", "[x]");
      TestParseDomain("(comment1) [a.b.c.d] (comment2)", "[a.b.c.d]");
      TestParseDomain("[]", "[]");
      TestParseDomain("[a .\r\n b. c.d ]", "[a.b.c.d]");
    }
    @Test
    public void TestLocalPart() {
      Address addr = new Address("local.local@example.com");
      Assert.assertEquals("local.local", addr.getLocalPart());
      TestParseLocalPart("x", "x");
      TestParseLocalPart("\"" + "\"", "");
      TestParseLocalPart("x.example", "x.example");
      TestParseLocalPart("x.example\ud800\udc00.example.com",
                    "x.example\ud800\udc00.example.com");
      TestParseLocalPart("x.example.com", "x.example.com");
      TestParseLocalPart("\"(not a comment)\"", "(not a comment)");
      TestParseLocalPart("(comment1) x (comment2)", "x");
      TestParseLocalPart("(comment1) example (comment2) . (comment3) com",
                    "example.com");
    }
    @Test
    public void TestToString() {
      Address addr = new Address("local.local@example.com");
      {
String stringTemp = addr.toString();
Assert.assertEquals(
"local.local@example.com",
stringTemp);
}
    }
  }
