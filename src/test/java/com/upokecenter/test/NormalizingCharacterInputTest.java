package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

import com.upokecenter.text.*;
import org.junit.Assert;
import org.junit.Test;

  public class NormalizingCharacterInputTest {
    @Test
    public void TestConstructor() {
      // not implemented yet
    }
    @Test
    public void TestGetChars() {
      // not implemented yet
    }
    @Test
    public void TestIsNormalized() {
      // Additional normalization tests
      if (NormalizingCharacterInput.IsNormalized(
        "x\u0300\u0323yz",
        Normalization.NFC))Assert.fail();
      if (NormalizingCharacterInput.IsNormalized(
        "x\u0300\u0323",
        Normalization.NFC))Assert.fail();
      try {
  NormalizingCharacterInput.IsNormalized((ICharacterInput)null,
    Normalization.NFC);
Assert.fail("Should have failed");
} catch(NullPointerException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      }
    @Test
    public void TestNormalize() {
      // not implemented yet
    }
    @Test
    public void TestRead() {
      NormalizingCharacterInput nci = new NormalizingCharacterInput("test");
      try {
 nci.Read(null, 0, 0);
Assert.fail("Should have failed");
} catch(NullPointerException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 nci.Read(new int[] { 't' },-1,1);
Assert.fail("Should have failed");
} catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 nci.Read(new int[] { 't' },5,1);
Assert.fail("Should have failed");
} catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 nci.Read(new int[] { 't' },0,-1);
Assert.fail("Should have failed");
} catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 nci.Read(new int[] { 't' },0,5);
Assert.fail("Should have failed");
} catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 nci.Read(new int[] { 't','t' },1,2);
Assert.fail("Should have failed");
} catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      Assert.assertEquals(1,nci.Read(new int[] { 't','t' },1,1));
    }
    @Test
    public void TestReadChar() {
      // not implemented yet
    }
  }
