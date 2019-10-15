using System;
using NUnit.Framework;
using PeterO.Mail;

namespace MailLibTest {
  [TestFixture]
  public class MediaTypeBuilderTest {
    [Test]
    public void TestConstructor() {
      // not implemented yet
    }
    [Test]
    public void TestIsMultipart() {
      // not implemented yet
    }
    [Test]
    public void TestIsText() {
      // not implemented yet
    }
    [Test]
    public void TestRemoveParameter() {
      var builder = new MediaTypeBuilder();
      try {
        builder.RemoveParameter(null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetParameter() {
      var builder = new MediaTypeBuilder();
      builder.SetParameter("a", "b");
      {
        string stringTemp = builder.ToMediaType().GetParameter("a");
        Assert.AreEqual(
          "b",
          stringTemp);
      }
      builder.SetParameter("a", String.Empty);
      Assert.AreEqual(String.Empty, builder.ToMediaType().GetParameter("a"));
      try {
        new MediaTypeBuilder().SetParameter(null, null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter(null, "test");
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter(null, String.Empty);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("test", null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("test", String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter(String.Empty, "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new MediaTypeBuilder().SetParameter("test\u00e0", "value");
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestSetSubType() {
      // not implemented yet
    }
    [Test]
    public void TestSetTopLevelType() {
      // not implemented yet
    }
    [Test]
    public void TestSubType() {
      // not implemented yet
    }
    [Test]
    public void TestToMediaType() {
      // not implemented yet
    }
    [Test]
    public void TestTopLevelType() {
      // not implemented yet
    }
    [Test]
    public void TestToString() {
      // not implemented yet
    }
  }
}
