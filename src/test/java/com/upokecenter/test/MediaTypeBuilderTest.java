package com.upokecenter.test; import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class MediaTypeBuilderTest {
    @Test
    public void TestConstructor() {
      // not implemented yet
    }
    @Test
    public void TestIsMultipart() {
      // not implemented yet
    }
    @Test
    public void TestIsText() {
      // not implemented yet
    }
    @Test
    public void TestRemoveParameter() {
      MediaTypeBuilder builder = new MediaTypeBuilder();
      try {
 builder.RemoveParameter(null);
Assert.fail("Should have failed");
} catch (NullPointerException ex) {
new Object();
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
    }
    @Test
    public void TestSetParameter() {
      MediaTypeBuilder builder = new MediaTypeBuilder();
      builder.SetParameter("a", "b");
      {
String stringTemp = builder.ToMediaType().GetParameter("a");
Assert.assertEquals(
  "b",
  stringTemp);
}
      builder.SetParameter("a", "");
      Assert.assertEquals("", builder.ToMediaType().GetParameter("a"));
      try {
 builder.SetParameter(null, "");
Assert.fail("Should have failed");
} catch (NullPointerException ex) {
new Object();
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 builder.SetParameter("", null);
Assert.fail("Should have failed");
} catch (NullPointerException ex) {
new Object();
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 builder.SetParameter("", "a");
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
new Object();
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 builder.SetParameter("a\u00e0", "a");
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
new Object();
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
    }
    @Test
    public void TestSetSubType() {
      // not implemented yet
    }
    @Test
    public void TestSetTopLevelType() {
      // not implemented yet
    }
    @Test
    public void TestSubType() {
      // not implemented yet
    }
    @Test
    public void TestToMediaType() {
      // not implemented yet
    }
    @Test
    public void TestTopLevelType() {
      // not implemented yet
    }
    @Test
    public void TestToString() {
      // not implemented yet
    }
  }
