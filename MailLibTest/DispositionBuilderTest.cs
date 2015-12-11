using NUnit.Framework;
using PeterO.Mail;
using System;
namespace MailLibTest {
  [TestFixture]
  public partial class DispositionBuilderTest {
    [Test]
    public void TestConstructor() {
      string stringNull = null;
      ContentDisposition dispNull = null;
      try {
Assert.AreEqual(null, new DispositionBuilder(stringNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
Assert.AreEqual(null, new DispositionBuilder(dispNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
Assert.AreEqual(null, new DispositionBuilder(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestDispositionType() {
      var db = new DispositionBuilder();
      db.SetDispositionType("inline");
      Assert.AreEqual("inline", db.DispositionType);
    }
    [Test]
    public void TestRemoveParameter() {
      try {
        new DispositionBuilder().RemoveParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetDispositionType() {
      try {
        new DispositionBuilder().SetDispositionType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new DispositionBuilder().SetDispositionType(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetParameter() {
      // not implemented yet
    }
    [Test]
    public void TestToDisposition() {
      // not implemented yet
    }
    [Test]
    public void TestToString() {
      var disp = new DispositionBuilder();
      disp.SetDispositionType("attachment");
      disp.SetParameter("a", "b");
      Assert.AreEqual("attachment;a=b", disp.ToString());
    }
  }
}
