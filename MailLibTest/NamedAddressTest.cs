using System;
using System.Collections.Generic;
using NUnit.Framework;
using PeterO.Mail;

namespace MailLibTest {
  [TestFixture]
  public class NamedAddressTest {
    [Test]
    public void TestConstructor() {
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty, (string)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty, (Address)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x at example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x@"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address((string)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestAddress() {
      // not implemented yet
    }
    [Test]
    public void TestGroupAddresses() {
      // not implemented yet
    }

    [Test]
    public void TestUpperCaseDomain() {
      NamedAddress na;
      na = new NamedAddress("test@EXAMPLE.COM");
      {
        string stringTemp = na.ToString();
        Assert.AreEqual(
          "test@EXAMPLE.COM",
          stringTemp);
      }
      na = new NamedAddress("Me <test@EXAMPLE.COM>");
      {
        string stringTemp = na.ToString();
        Assert.AreEqual(
  "Me <test@EXAMPLE.COM>",
  stringTemp);
      }
    }

    [Test]
    public void TestIsGroup() {
      // not implemented yet
      NamedAddress na;
      var valueNaList = new List<NamedAddress>();
      valueNaList.Add(new NamedAddress("test@example.com"));
      na = new NamedAddress("test", valueNaList);
      Assert.IsTrue(na.IsGroup);
      na = new NamedAddress("test@example.com");
      Assert.IsFalse(na.IsGroup);
      na = new NamedAddress("Test <test@example.com>");
      Assert.IsFalse(na.IsGroup);
      na = new NamedAddress("Group: test@example.com;");
      Assert.IsTrue(na.IsGroup);
      na = new NamedAddress("Group: Test <test@example.com>;");
      Assert.IsTrue(na.IsGroup);
      na = new NamedAddress("Group:;");
      Assert.IsTrue(na.IsGroup);
    }

    [Test]
    public void TestNamedAddressParse() {
      NamedAddress na;
      IList<NamedAddress> groupAddr;
      na = new NamedAddress("me@example.com");
      Assert.AreEqual("me@example.com", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual(
          "me@example.com",
          stringTemp);
      }
      {
        string stringTemp = na.ToString();
        Assert.AreEqual(
          "me@example.com",
          stringTemp);
      }
      na = new NamedAddress("John's Office <me@example.com>");
      Assert.AreEqual("John's Office", na.Name);
      na = new NamedAddress("<me@example.com>");
      if (na.DisplayName != null) {
        Assert.Fail();
      }
      na = new NamedAddress("Me <me@example.com>");
      Assert.AreEqual("Me", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual("me@example.com", stringTemp);
      }
      na = new NamedAddress(
        "(comment) Me (comment) <me@example.com> (comment)");
      Assert.AreEqual("Me", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual("me@example.com", stringTemp);
      }
      na = new NamedAddress("=?utf-8?q?Me?= <me@example.com>");
      Assert.AreEqual("Me", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual("me@example.com", stringTemp);
      }
      {
        string stringTemp = na.ToString();
        Assert.AreEqual("Me <me@example.com>", stringTemp);
      }
      na = new NamedAddress("=?utf-8?q?John=27s_Office?= <" +
                  "me@example.com>");
      Assert.AreEqual("John's Office", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual("me@example.com", stringTemp);
      }
      {
        string stringTemp = na.ToString();
        Assert.AreEqual("John's Office <me@example.com>", stringTemp);
      }
      na = new NamedAddress("\"Me\" <me@example.com>");
      Assert.AreEqual("Me", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual(
          "me@example.com",
          stringTemp);
      }
      {
        string stringTemp = na.ToString();
        Assert.AreEqual(
  "Me <me@example.com>",
  stringTemp);
      }
      //------------
      na = new NamedAddress("Group: \"Me\" <me@example.com>;");
      Assert.IsTrue(na.IsGroup);
      groupAddr = na.GroupAddresses;
      Assert.AreEqual(1, groupAddr.Count);
      na = groupAddr[0];
      Assert.AreEqual("Me", na.Name);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual(
          "me@example.com",
          stringTemp);
      }
      na = new
  NamedAddress("Group: \"Me\" <me@example.com>, Fred <fred@example.com>;");
      Assert.IsTrue(na.IsGroup);
      {
        string stringTemp = na.ToString();
        const string ValueS1 =
        "Group: Me <me@example.com>, Fred <fred@example.com>;";
        Assert.AreEqual(ValueS1, stringTemp);
      }
      groupAddr = na.GroupAddresses;
      Assert.AreEqual(2, groupAddr.Count);
      na = groupAddr[0];
      Assert.AreEqual("Me", na.Name);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual(
          "me@example.com",
          stringTemp);
      }
      na = groupAddr[1];
      Assert.AreEqual("Fred", na.Name);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual(
          "fred@example.com",
          stringTemp);
      }
      //------------
      na = new
        NamedAddress("Group: \"Me\" <me@example.com>, somebody@example.com;");
      Assert.AreEqual("Group", na.Name);
      Assert.IsTrue(na.IsGroup);
      {
        const string ValueS1 = "Group: Me <me@example.com>," +
"\u0020somebody@example.com;";
        Assert.AreEqual(ValueS1, na.ToString());
      }
      groupAddr = na.GroupAddresses;
      Assert.AreEqual(2, groupAddr.Count);
      na = groupAddr[0];
      Assert.AreEqual("Me", na.Name);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual(
          "me@example.com",
          stringTemp);
      }
      na = groupAddr[1];
      Assert.AreEqual("somebody@example.com", na.Name);
      {
        string stringTemp = na.Address.ToString();
        Assert.AreEqual(
          "somebody@example.com",
          stringTemp);
      }
    }
    [Test]
    public void TestDisplayName() {
      NamedAddress na;
      na = new NamedAddress("=?utf-8?q?Me?=", "me@example.com");
      Assert.AreEqual("=?utf-8?q?Me?=", na.DisplayName);
      na = new NamedAddress(null, "me@example.com");
      Assert.AreEqual(null, na.DisplayName);
    }

    [Test]
    public void TestName() {
      NamedAddress na;
      na = new NamedAddress("=?utf-8?q?Me?=", "me@example.com");
      Assert.AreEqual("=?utf-8?q?Me?=", na.Name);
      na = new NamedAddress(null, "me@example.com");
      Assert.AreEqual("me@example.com", na.Name);
    }

    [Test]
    public void TestNameEqualQMark() {
      NamedAddress na;
      na = new NamedAddress("=?utf-8?q?Me?=", "me@example.com");
      Assert.AreEqual("=?utf-8?q?Me?=", na.Name);
      Assert.AreEqual("\"=?utf-8?q?Me?=\" <me@example.com>", na.ToString());
      na = new NamedAddress("=?xyz", "me@example.com");
      Assert.AreEqual("=?xyz", na.Name);
      Assert.AreEqual("\"=?xyz\" <me@example.com>", na.ToString());
      na = new NamedAddress("=?utf-8?q?Me", "me@example.com");
      Assert.AreEqual("=?utf-8?q?Me", na.Name);
      Assert.AreEqual("\"=?utf-8?q?Me\" <me@example.com>", na.ToString());
      na = new NamedAddress("=?utf-8?=", "me@example.com");
      Assert.AreEqual("=?utf-8?=", na.Name);
      Assert.AreEqual("\"=?utf-8?=\" <me@example.com>", na.ToString());
    }

    [Test]
    public void TestToString() {
      const string ValueMbox =
              "Me <@example.org,@example.net,@example.com:me@x.example>";
      var result = new NamedAddress(ValueMbox);
      {
        string stringTemp = result.ToString();
        Assert.AreEqual(
          "Me <me@x.example>",
          stringTemp);
      }
    }
  }
}
