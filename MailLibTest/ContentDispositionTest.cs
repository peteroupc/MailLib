using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MailLibTest {
  [TestClass]
  public partial class ContentDispositionTest {
    [TestMethod]
    public void TestDispositionType() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEquals() {
      // not implemented yet
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
    public void TestIsAttachment() {
      ContentDisposition cd=ContentDisposition.Parse("inline");
      Assert.IsFalse(cd.IsAttachment);
      cd=ContentDisposition.Parse("cd-unknown");
      Assert.IsFalse(cd.IsAttachment);
      cd=ContentDisposition.Parse("attachment");
      Assert.IsTrue(cd.IsAttachment);
    }
    [TestMethod]
    public void TestIsInline() {
      ContentDisposition cd=ContentDisposition.Parse("inline");
      Assert.IsTrue(cd.IsInline);
      cd=ContentDisposition.Parse("cd-unknown");
      Assert.IsFalse(cd.IsInline);
      cd=ContentDisposition.Parse("attachment");
      Assert.IsFalse(cd.IsInline);
    }
    [TestMethod]
    public void TestMakeFilename() {
      Assert.AreEqual(String.Empty, ContentDisposition.MakeFilename(null));
    }
    [TestMethod]
    public void TestParameters() {
      // not implemented yet
    }
    [TestMethod]
    public void TestParse() {
      try {
 ContentDisposition.Parse(null);
Assert.Fail("Should have failed");
} catch(ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
  }
}
