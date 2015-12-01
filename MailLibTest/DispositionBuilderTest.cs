using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO.Mail;
namespace MailLibTest {
  [TestClass]
  public partial class DispositionBuilderTest {
    [TestMethod]
    public void TestConstructor() {
      string stringNull = null;
      ContentDisposition dispNull = null;
      try {
Assert.AreEqual(null, new DispositionBuilder(stringNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.WriteLine(ex.Message);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
Assert.AreEqual(null, new DispositionBuilder(dispNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.WriteLine(ex.Message);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
Assert.AreEqual(null, new DispositionBuilder(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.WriteLine(ex.Message);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestDispositionType() {
      var db = new DispositionBuilder();
      db.SetDispositionType("inline");
      Assert.AreEqual("inline", db.DispositionType);
    }
    [TestMethod]
    public void TestRemoveParameter() {
      try {
        new DispositionBuilder().RemoveParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.WriteLine(ex.Message);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestSetDispositionType() {
      try {
        new DispositionBuilder().SetDispositionType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException ex) {
        Console.WriteLine(ex.Message);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new DispositionBuilder().SetDispositionType(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException ex) {
        Console.WriteLine(ex.Message);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [TestMethod]
    public void TestSetParameter() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToDisposition() {
      // not implemented yet
    }
    [TestMethod]
    public void TestToString() {
      // not implemented yet
    }
  }
}
