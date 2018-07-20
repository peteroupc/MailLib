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
Assert.AreEqual(null, new DispositionBuilder(stringNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
Assert.AreEqual(null, new DispositionBuilder(dispNull));
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
Assert.AreEqual(null, new DispositionBuilder(String.Empty));
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
        // NOTE: Intentionally empty
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
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        new DispositionBuilder().SetDispositionType(String.Empty);
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
      var db = new DispositionBuilder().SetParameter("a", "b");
      {
string stringTemp = db.ToDisposition().GetParameter("a");
Assert.AreEqual(
  "b",
  stringTemp);
}
      db.SetParameter("a", String.Empty);
      Assert.AreEqual(String.Empty, db.ToDisposition().GetParameter("a"));
      try {
 new DispositionBuilder().SetParameter(null, null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 new DispositionBuilder().SetParameter(null, "test");
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 new DispositionBuilder().SetParameter(null, String.Empty);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 new DispositionBuilder().SetParameter("test", null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 new DispositionBuilder().SetParameter("test", String.Empty);
} catch (Exception ex) {
Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 new DispositionBuilder().SetParameter(String.Empty, "value");
Assert.Fail("Should have failed");
} catch (ArgumentException) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 new DispositionBuilder().SetParameter("test\u00e0", "value");
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
      var disp = new DispositionBuilder();
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
