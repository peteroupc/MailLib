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
      MediaType.Builder builder = new MediaType.Builder();
      try {
        builder.RemoveParameter(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestSetParameter() {
      MediaType.Builder builder = new MediaType.Builder();
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
        new MediaType.Builder().SetParameter(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter(null, "test");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter(null, "");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter("test", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter("test", "");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter("", "value");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new MediaType.Builder().SetParameter("test\u00e0", "value");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
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
