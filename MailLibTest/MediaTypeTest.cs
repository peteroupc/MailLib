using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MailLibTest {
  [TestClass]
  public partial class MediaTypeTest {
    [TestMethod]
    public void TestEquals() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetCharset() {
      MediaType mt;
      mt=MediaType.Parse("text/plain");
      {
string stringTemp = mt.GetCharset();
Assert.AreEqual(
"us-ascii",
stringTemp);
}
      mt=MediaType.Parse("text/vcard");
      {
string stringTemp = mt.GetCharset();
Assert.AreEqual(
"utf-8",
stringTemp);
}
      mt=MediaType.Parse("text/x-unknown");
      Assert.AreEqual(String.Empty, mt.GetCharset());
    }
    [TestMethod]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetParameter() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsMultipart() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsText() {
      // not implemented yet
    }
    [TestMethod]
    public void TestParameters() {
      // not implemented yet
    }
    [TestMethod]
    public void TestParse() {
      // not implemented yet
    }
    [TestMethod]
    public void TestSubType() {
      // not implemented yet
    }
    [TestMethod]
    public void TestTopLevelType() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestTypeAndSubType() {
      // not implemented yet
    }
  }
}
