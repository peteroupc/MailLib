package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

import com.upokecenter.mail.*;
import org.junit.Assert;
import org.junit.Test;

  public class ContentDispositionTest {
    @Test
    public void TestDispositionType() {
      // not implemented yet
    }
    @Test
    public void TestEquals() {
      // not implemented yet
    }
    @Test
    public void TestGetHashCode() {
      // not implemented yet
    }
    @Test
    public void TestGetParameter() {
      // not implemented yet
    }
    @Test
    public void TestIsAttachment() {
      ContentDisposition cd=ContentDisposition.Parse("inline");
      if (cd.isAttachment())Assert.fail();
      cd=ContentDisposition.Parse("cd-unknown");
      if (cd.isAttachment())Assert.fail();
      cd=ContentDisposition.Parse("attachment");
      if (!(cd.isAttachment()))Assert.fail();
    }
    @Test
    public void TestIsInline() {
      ContentDisposition cd=ContentDisposition.Parse("inline");
      if (!(cd.isInline()))Assert.fail();
      cd=ContentDisposition.Parse("cd-unknown");
      if (cd.isInline())Assert.fail();
      cd=ContentDisposition.Parse("attachment");
      if (cd.isInline())Assert.fail();
    }
    @Test
    public void TestMakeFilename() {
      Assert.assertEquals("", ContentDisposition.MakeFilename(null));
    }
    @Test
    public void TestParameters() {
      // not implemented yet
    }
    @Test
    public void TestParse() {
      try {
 ContentDisposition.Parse(null);
Assert.fail("Should have failed");
} catch(NullPointerException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
    }
    @Test
    public void TestToString() {
      // not implemented yet
    }
  }
