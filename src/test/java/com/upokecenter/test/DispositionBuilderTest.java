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
        Assert.assertEquals(null, new ContentDisposition.Builder(stringNull));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new ContentDisposition.Builder(dispNull));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new ContentDisposition.Builder(""));
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
      ContentDisposition.Builder db = new ContentDisposition.Builder();
      db.SetDispositionType("inline");
      Assert.assertEquals("inline", db.getDispositionType());
    }
    @Test
    public void TestRemoveParameter() {
      try {
        new ContentDisposition.Builder().RemoveParameter(null);
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
        new ContentDisposition.Builder().SetDispositionType(null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new ContentDisposition.Builder().SetDispositionType("");
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
      ContentDisposition.Builder db = new ContentDisposition.Builder().SetParameter("a", "b");
      {
        String stringTemp = db.ToDisposition().GetParameter("a");
        Assert.assertEquals(
          "b",
          stringTemp);
      }
      db.SetParameter("a", "");
      Assert.assertEquals("", db.ToDisposition().GetParameter("a"));
      try {
        new ContentDisposition.Builder().SetParameter(null, null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter(null, "test");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter(null, "");
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter("test", null);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter("test", "");
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter("", "value");
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
      } catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        new ContentDisposition.Builder().SetParameter("test\u00e0", "value");
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
      ContentDisposition.Builder disp = new ContentDisposition.Builder();
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
