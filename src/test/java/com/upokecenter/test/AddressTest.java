package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

import com.upokecenter.mail.*;
import org.junit.Assert;
import org.junit.Test;

  public class AddressTest {
    @Test
    public void TestConstructor() {
      try {
        Assert.assertEquals(null, new Address("local=domain.example"));
        Assert.fail("Should have failed");
      } catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("local@"));
        Assert.fail("Should have failed");
      } catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address(EncodingTest.Repeat("local", 200) +
          "@example.com"));
        Assert.fail("Should have failed");
      } catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address("lo,cal@example.com"));
        Assert.fail("Should have failed");
      } catch(IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestDomain() {
      Address addr = new Address("local.local@example.com");
      Assert.assertEquals("example.com",addr.getDomain());
    }
    @Test
    public void TestLocalPart() {
      Address addr = new Address("local.local@example.com");
      Assert.assertEquals("local.local", addr.getLocalPart());
    }
    @Test
    public void TestToString() {
      Address addr = new Address("local.local@example.com");
      {
String stringTemp = addr.toString();
Assert.assertEquals(
"local.local@example.com",
stringTemp);
}
    }
  }
