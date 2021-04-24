using System;
using NUnit.Framework;
using PeterO.Mail;

namespace MailLibTest {
  [TestFixture]
  public class DispositionBuilderTest {
    [Test]
    public void TestConstructor() {
      string stringNull = null;
      ContentDisposition dispNull = null;
      try {
        Assert.AreEqual(null, new ContentDisposition.Builder(stringNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new ContentDisposition.Builder(dispNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Assert.AreEqual(null, new ContentDisposition.Builder(String.Empty));
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestDispositionType() {
      var db = new ContentDisposition.Builder();
      db.SetDispositionType("inline");
      Assert.AreEqual("inline", db.DispositionType);
    }
    [Test]
    public void TestRemoveParameter() {
      try {
        new ContentDisposition.Builder().RemoveParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetDispositionType() {
      try {
        new ContentDisposition.Builder().SetDispositionType(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new ContentDisposition.Builder().SetDispositionType(String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetParameter() {
      var db = new ContentDisposition.Builder().SetParameter("a", "b");
      {
        string stringTemp = db.ToDisposition().GetParameter("a");
        Assert.AreEqual(
          "b",
          stringTemp);
      }
      db.SetParameter("a", String.Empty);
      Assert.AreEqual(String.Empty, db.ToDisposition().GetParameter("a"));
      try {
        new ContentDisposition.Builder().SetParameter(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter(null, "test");
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter(null, String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter("test", null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter("test", String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter(String.Empty, "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter("test\u00e0", "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestToDisposition() {
      // not implemented yet
    }
    [Test]
    public void TestToString() {
      var disp = new ContentDisposition.Builder();
      disp.SetDispositionType("attachment");
      disp.SetParameter("a", "b");
      {
        string stringTemp = disp.ToString();
        Assert.AreEqual(
          "attachment;a=b",
          stringTemp);
      }
    }
  }
}
