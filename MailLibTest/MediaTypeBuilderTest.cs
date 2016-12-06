using NUnit.Framework;
using PeterO.Mail;
using System;

namespace MailLibTest {
  [TestFixture]
  public partial class MediaTypeBuilderTest {
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
new Object();
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
 builder.SetParameter(null, String.Empty);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 builder.SetParameter(String.Empty, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 builder.SetParameter(String.Empty, "a");
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 builder.SetParameter("a\u00e0", "a");
Assert.Fail("Should have failed");
} catch (ArgumentException) {
new Object();
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
