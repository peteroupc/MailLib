using System;
using System.Collections.Generic;
using PeterO.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MailLibTest {
  [TestClass]
  public partial class NamedAddressTest {
    [TestMethod]
    public void TestConstructor() {
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty, (string)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty, (Address)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x at example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("x@"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("@example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress("example.com"));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new Address((string)null));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new NamedAddress(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestAddress() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGroupAddresses() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsGroup() {
      // not implemented yet
      NamedAddress na;
      var naList = new List<NamedAddress>();
      naList.Add(new NamedAddress("test@example.com"));
      na=new NamedAddress("test",naList);
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

    [TestMethod]
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
      na = new NamedAddress("Me <me@example.com>");
      Assert.AreEqual("Me", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
string stringTemp = na.Address.ToString();
Assert.AreEqual(
"me@example.com",
stringTemp);
}
    na = new NamedAddress("(comment) Me (comment) <me@example.com> (comment)");
      Assert.AreEqual("Me", na.Name);
      Assert.IsFalse(na.IsGroup);
      {
string stringTemp = na.Address.ToString();
Assert.AreEqual(
"me@example.com",
stringTemp);
}
      na = new NamedAddress("=?utf-8?q?Me?= <me@example.com>");
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
   na = new NamedAddress("=?utf-8?q?John=27s_Office?= <" +
                  "me@example.com>");
      Assert.AreEqual("John's Office", na.Name);
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
"John's Office <me@example.com>",
stringTemp);
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
      const string s1 = "Group: Me <me@example.com>, Fred <fred@example.com>;" ;
        Assert.AreEqual(s1, stringTemp);
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
string stringTemp = na.ToString();
        const string s1 = "Group: Me <me@example.com>, somebody@example.com;";
        Assert.AreEqual(s1, stringTemp);
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

    [TestMethod]
    public void TestName() {
      NamedAddress na;
      na = new NamedAddress("=?utf-8?q?Me?=", "me@example.com");
      Assert.AreEqual("=?utf-8?q?Me?=", na.Name);
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
  }
}
