using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MailLibTest {
  [TestClass]
  public partial class AddressTest {
    [TestMethod]
    public void TestConstructor() {
      try {
        Assert.AreEqual(null, new Address("local=domain.example"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("local@"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address(EncodingTest.Repeat("local", 200) +
          "@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address("lo,cal@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestDomain() {
      var addr = new Address("local.local@example.com");
      Assert.AreEqual("example.com",addr.Domain);
    }
    [TestMethod]
    public void TestLocalPart() {
      var addr = new Address("local.local@example.com");
      Assert.AreEqual("local.local", addr.LocalPart);
    }
    [TestMethod]
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
