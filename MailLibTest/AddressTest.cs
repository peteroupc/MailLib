using System;
using NUnit.Framework;
using PeterO.Mail;
using Test;

namespace MailLibTest {
  [TestFixture]
  public class AddressTest {
    private static void TestParseLocalPart(string str, string expected) {
      var na = new NamedAddress("b <" + str + "@example.com>");
      Address addr = na.Address;
      Assert.AreEqual(expected, addr.LocalPart);
      addr = new Address(str + "@example.com");
      Assert.AreEqual(expected, addr.LocalPart);
    }

    private static void TestParseLocalPartNAOnly(string str, string expected) {
      var na = new NamedAddress("b <" + str + "@example.com>");
      Address addr = na.Address;
      Assert.AreEqual(expected, addr.LocalPart);
    }

    private static void TestParseDomain(string str, string expected) {
      var na = new NamedAddress("b <example@" + str + ">");
      Address addr = na.Address;
      Assert.AreEqual(expected, addr.Domain);
      addr = new Address("example@" + str);
      Assert.AreEqual(expected, addr.Domain);
    }

    private static void TestParseDomainNAOnly(string str, string expected) {
      var na = new NamedAddress("b <example@" + str + ">");
      Address addr = na.Address;
      Assert.AreEqual(expected, addr.Domain);
    }

    [Test]
    public void TestConstructor() {
      try {
        Assert.AreEqual(null, new Address("local=domain.example"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("local@"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        {
          object objectTemp = null;
          object objectTemp2 = new Address(EncodingTest.Repeat("local", 200) +
            "@example.com");
          Assert.AreEqual(objectTemp, objectTemp2);
        }
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("lo,cal@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }

    [Test]
    public void TestUpperCase() {
      Address addr;
      addr = new Address("test@EXAMPLE.COM");
      Assert.AreEqual("EXAMPLE.COM", addr.Domain);
      {
        string stringTemp = addr.ToString();
        Assert.AreEqual(
          "test@EXAMPLE.COM",
          stringTemp);
      }
    }

    [Test]
    public void TestDomain() {
      var addr = new Address("local.local@example.com");
      Assert.AreEqual("example.com", addr.Domain);
      TestParseDomain("x", "x");
      TestParseDomain("x.example", "x.example");
      TestParseDomain(
        "x.example\ud800\udc00.example.com",
        "x.example\ud800\udc00.example.com");
      TestParseDomain("x.example.com", "x.example.com");
      TestParseDomainNAOnly("(comment1) x(comment2)", "x");
      TestParseDomainNAOnly(
        "(comment1) example (comment2) . (comment3) com",
        "example.com");
      TestParseDomainNAOnly(
        "(co(mme)nt1) example (comment2) . (comment3) com",
        "example.com");
      TestParseDomainNAOnly("(comment1) [x](comment2)", "[x]");
      TestParseDomainNAOnly("(comment1) [a.b.c.d](comment2)", "[a.b.c.d]");
      TestParseDomain("[]", "[]");
      TestParseDomainNAOnly("[a .\r\n b. c.d ]", "[a.b.c.d]");
    }
    [Test]
    public void TestLocalPart() {
      var addr = new Address("local.local@example.com");
      Assert.AreEqual("local.local", addr.LocalPart);
      addr = new Address("x!y!z!example@example.com");
      Assert.AreEqual("x!y!z!example", addr.LocalPart);
      addr = new Address("x%y%z%example@example.com");
      Assert.AreEqual("x%y%z%example", addr.LocalPart);
      addr = new Address("x!y%z!example@example.com");
      Assert.AreEqual("x!y%z!example", addr.LocalPart);

      TestParseLocalPart("x", "x");
      TestParseLocalPart("x!y!z", "x!y!z");
      TestParseLocalPart("\"" + "\"", String.Empty);
      TestParseLocalPart("x.example", "x.example");
      TestParseLocalPart(
        "x.example\ud800\udc00.example.com",
        "x.example\ud800\udc00.example.com");
      TestParseLocalPart("x.example.com", "x.example.com");
      TestParseLocalPart("\"(not a comment)\"", "(not a comment)");
      TestParseLocalPartNAOnly("(comment1)x(comment2)", "x");
      TestParseLocalPartNAOnly("(comment1)x(comment2)", "x");
      TestParseLocalPartNAOnly(
        "(comment1) example (comment2) . (comment3) com",
        "example.com");
      TestParseLocalPartNAOnly(
        "(com(plex)comment) example (comment2) . (comment3) com",
        "example.com");
    }
    [Test]
    [Timeout(5000)]
    public void TestToString() {
      var addr = new Address("local.local@example.com");
      {
        string stringTemp = addr.ToString();
        Assert.AreEqual(
          "local.local@example.com",
          stringTemp);
      }
      addr = new Address("local-local@example.com");
      {
        string stringTemp = addr.ToString();
        Assert.AreEqual(
          "local-local@example.com",
          stringTemp);
      }
    }
  }
}
