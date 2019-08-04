package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class DispositionBuilderTest {
    @Test
    public void TestConstructor() {
      String stringNull = null;
      ContentDisposition dispNull = null;
      try {
Assert.assertEquals(null, new DispositionBuilder(stringNull));
Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
Assert.assertEquals(null, new DispositionBuilder(dispNull));
Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
Assert.assertEquals(null, new DispositionBuilder(""));
Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestDispositionType() {
      DispositionBuilder db = new DispositionBuilder();
      db.SetDispositionType("inline");
      Assert.assertEquals("inline", db.getDispositionType());
    }
    @Test
    public void TestRemoveParameter() {
      try {
        new DispositionBuilder().RemoveParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetDispositionType() {
      try {
        new DispositionBuilder().SetDispositionType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new DispositionBuilder().SetDispositionType("");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetParameter() {
      DispositionBuilder db = new DispositionBuilder().SetParameter("a", "b");
      {
        String stringTemp = db.ToDisposition().GetParameter("a");
Assert.assertEquals(
  "b",
  stringTemp);
}
      db.SetParameter("a", "");
      Assert.assertEquals("", db.ToDisposition().GetParameter("a"));
      try {
 new DispositionBuilder().SetParameter(null, null);
 Assert.fail("Should have failed");
} catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.fail(ex.toString());
 throw new IllegalStateException("", ex);
}
      try {
 new DispositionBuilder().SetParameter(null, "test");
 Assert.fail("Should have failed");
} catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.fail(ex.toString());
 throw new IllegalStateException("", ex);
}
      try {
 new DispositionBuilder().SetParameter(null, "");
 Assert.fail("Should have failed");
} catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.fail(ex.toString());
 throw new IllegalStateException("", ex);
}
      try {
 new DispositionBuilder().SetParameter("test", null);
 Assert.fail("Should have failed");
} catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.fail(ex.toString());
 throw new IllegalStateException("", ex);
}
      try {
 new DispositionBuilder().SetParameter("test", "");
} catch (Exception ex) {
Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 new DispositionBuilder().SetParameter("", "value");
 Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.fail(ex.toString());
 throw new IllegalStateException("", ex);
}
      try {
 new DispositionBuilder().SetParameter("test\u00e0", "value");
 Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
 Assert.fail(ex.toString());
 throw new IllegalStateException("", ex);
}
    }
    @Test
    public void TestToDisposition() {
      // not implemented yet
    }
    @Test
    public void TestToString() {
      DispositionBuilder disp = new DispositionBuilder();
      disp.SetDispositionType("attachment");
      disp.SetParameter("a", "b");
      {
        String stringTemp = disp.toString();
Assert.assertEquals(
  "attachment;a=b",
  stringTemp);
}
    }
  }
