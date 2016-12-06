using NUnit.Framework;
using PeterO.Mail;
using System;

namespace MailLibTest {
  [TestFixture]
  public partial class AddressTest {
    private static void TestParseLocalPart(string str, string expected) {
      var na = new NamedAddress("b <" + str + "@example.com>");
      Address addr = na.Address;
      Assert.AreEqual(expected, addr.LocalPart);
    }

    private static void TestParseDomain(string str, string expected) {
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
new Object();
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("local@"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
new Object();
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
new Object();
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("lo,cal@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
new Object();
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
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
      TestParseDomain("(comment1) x (comment2)", "x");
      TestParseDomain(
  "(comment1) example (comment2) . (comment3) com",
  "example.com");
      TestParseDomain("(comment1) [x] (comment2)", "[x]");
      TestParseDomain("(comment1) [a.b.c.d] (comment2)", "[a.b.c.d]");
      TestParseDomain("[]", "[]");
      TestParseDomain("[a .\r\n b. c.d ]", "[a.b.c.d]");
    }
    [Test]
    public void TestLocalPart() {
      var addr = new Address("local.local@example.com");
      Assert.AreEqual("local.local", addr.LocalPart);
      TestParseLocalPart("x", "x");
      TestParseLocalPart("\"" + "\"", String.Empty);
      TestParseLocalPart("x.example", "x.example");
      TestParseLocalPart(
  "x.example\ud800\udc00.example.com",
  "x.example\ud800\udc00.example.com");
      TestParseLocalPart("x.example.com", "x.example.com");
      TestParseLocalPart("\"(not a comment)\"", "(not a comment)");
      TestParseLocalPart("(comment1) x (comment2)", "x");
      TestParseLocalPart(
  "(comment1) example (comment2) . (comment3) com",
  "example.com");
    }
    [Test]
    public void TestToString() {
      var addr = new Address("local.local@example.com");
      {
string stringTemp = addr.ToString();
Assert.AreEqual(
  "local.local@example.com",
  stringTemp);
}
    }
  }
}
