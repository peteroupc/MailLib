package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

  public class NamedAddressTest {
    @Test
    public void TestConstructor() {
      try {
        Assert.assertEquals(null, new NamedAddress("", (String)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("", (Address)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("x at example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("x"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("x@"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("@example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress("example.com"));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new Address((String)null));
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        Assert.assertEquals(null, new NamedAddress(""));
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
// NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
    }
    @Test
    public void TestAddress() {
      // not implemented yet
    }
    @Test
    public void TestGroupAddresses() {
      // not implemented yet
    }
    @Test
    public void TestIsGroup() {
      // not implemented yet
      NamedAddress na;
      ArrayList<NamedAddress> valueNaList = new ArrayList<NamedAddress>();
      valueNaList.add(new NamedAddress("test@example.com"));
      na = new NamedAddress("test", valueNaList);
      if (!(na.isGroup())) {
 Assert.fail();
 }
      na = new NamedAddress("test@example.com");
      if (na.isGroup()) {
 Assert.fail();
 }
      na = new NamedAddress("Test <test@example.com>");
      if (na.isGroup()) {
 Assert.fail();
 }
      na = new NamedAddress("Group: test@example.com;");
      if (!(na.isGroup())) {
 Assert.fail();
 }
      na = new NamedAddress("Group: Test <test@example.com>;");
      if (!(na.isGroup())) {
 Assert.fail();
 }
      na = new NamedAddress("Group:;");
      if (!(na.isGroup())) {
 Assert.fail();
 }
    }

    @Test
    public void TestNamedAddressParse() {
      NamedAddress na;
      List<NamedAddress> groupAddr;
      na = new NamedAddress("me@example.com");
      Assert.assertEquals("me@example.com", na.getName());
      if (na.isGroup()) {
 Assert.fail();
 }
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      {
        String stringTemp = na.toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      na = new NamedAddress("John's Office <me@example.com>");
      Assert.assertEquals("John's Office", na.getName());
      na = new NamedAddress("<me@example.com>");
      if (na.getDisplayName() != null) {
        Assert.fail();
      }
      na = new NamedAddress("Me <me@example.com>");
      Assert.assertEquals("Me", na.getName());
      if (na.isGroup()) {
 Assert.fail();
 }
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
na = new NamedAddress("(comment) Me (comment) <me@example.com> (comment)");
      Assert.assertEquals("Me", na.getName());
      if (na.isGroup()) {
 Assert.fail();
 }
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      na = new NamedAddress("=?utf-8?q?Me?= <me@example.com>");
      Assert.assertEquals("Me", na.getName());
      if (na.isGroup()) {
 Assert.fail();
 }
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      {
        String stringTemp = na.toString();
        Assert.assertEquals(
  "Me <me@example.com>",
  stringTemp);
}
na = new NamedAddress("=?utf-8?q?John=27s_Office?= <" +
                  "me@example.com>");
      Assert.assertEquals("John's Office", na.getName());
      if (na.isGroup()) {
 Assert.fail();
 }
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      {
        String stringTemp = na.toString();
        Assert.assertEquals(
  "John's Office <me@example.com>",
  stringTemp);
}
      na = new NamedAddress("\"Me\" <me@example.com>");
      Assert.assertEquals("Me", na.getName());
      if (na.isGroup()) {
 Assert.fail();
 }
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      {
        String stringTemp = na.toString();
        Assert.assertEquals(
  "Me <me@example.com>",
  stringTemp);
}
      //------------
      na = new NamedAddress("Group: \"Me\" <me@example.com>;");
      if (!(na.isGroup())) {
 Assert.fail();
 }
      groupAddr = na.getGroupAddresses();
      Assert.assertEquals(1, groupAddr.size());
      na = groupAddr.get(0);
      Assert.assertEquals("Me", na.getName());
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      na = new
  NamedAddress("Group: \"Me\" <me@example.com>, Fred <fred@example.com>;");
      if (!(na.isGroup())) {
 Assert.fail();
 }
      {
        String stringTemp = na.toString();
        String ValueS1 =
        "Group: Me <me@example.com>, Fred <fred@example.com>;";
        Assert.assertEquals(ValueS1, stringTemp);
}
      groupAddr = na.getGroupAddresses();
      Assert.assertEquals(2, groupAddr.size());
      na = groupAddr.get(0);
      Assert.assertEquals("Me", na.getName());
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      na = groupAddr.get(1);
      Assert.assertEquals("Fred", na.getName());
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "fred@example.com",
          stringTemp);
}
      //------------
      na = new
        NamedAddress("Group: \"Me\" <me@example.com>, somebody@example.com;");
      Assert.assertEquals("Group", na.getName());
      if (!(na.isGroup())) {
 Assert.fail();
 }
      {
        String stringTemp = na.toString();
        String ValueS1 = "Group: Me <me@example.com>," +
"\u0020 somebody@example.com;";
        Assert.assertEquals(ValueS1, stringTemp);
}
      groupAddr = na.getGroupAddresses();
      Assert.assertEquals(2, groupAddr.size());
      na = groupAddr.get(0);
      Assert.assertEquals("Me", na.getName());
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "me@example.com",
          stringTemp);
}
      na = groupAddr.get(1);
      Assert.assertEquals("somebody@example.com", na.getName());
      {
        String stringTemp = na.getAddress().toString();
        Assert.assertEquals(
          "somebody@example.com",
          stringTemp);
}
    }
    @Test
    public void TestDisplayName() {
      NamedAddress na;
      na = new NamedAddress("=?utf-8?q?Me?=", "me@example.com");
      Assert.assertEquals("=?utf-8?q?Me?=", na.getDisplayName());
      na = new NamedAddress(null, "me@example.com");
      Assert.assertEquals(null, na.getDisplayName());
    }

    @Test
    public void TestName() {
      NamedAddress na;
      na = new NamedAddress("=?utf-8?q?Me?=", "me@example.com");
      Assert.assertEquals("=?utf-8?q?Me?=", na.getName());
      na = new NamedAddress(null, "me@example.com");
      Assert.assertEquals("me@example.com", na.getName());
    }

    @Test
    public void TestNameEqualQMark() {
      NamedAddress na;
      na = new NamedAddress("=?utf-8?q?Me?=", "me@example.com");
      Assert.assertEquals("=?utf-8?q?Me?=", na.getName());
      Assert.assertEquals("\"=?utf-8?q?Me?=\" <me@example.com>", na.toString());
      na = new NamedAddress("=?xyz", "me@example.com");
      Assert.assertEquals("=?xyz", na.getName());
      Assert.assertEquals("\"=?xyz\" <me@example.com>", na.toString());
      na = new NamedAddress("=?utf-8?q?Me", "me@example.com");
      Assert.assertEquals("=?utf-8?q?Me", na.getName());
      Assert.assertEquals("\"=?utf-8?q?Me\" <me@example.com>", na.toString());
      na = new NamedAddress("=?utf-8?=", "me@example.com");
      Assert.assertEquals("=?utf-8?=", na.getName());
      Assert.assertEquals("\"=?utf-8?=\" <me@example.com>", na.toString());
    }

    @Test
    public void TestToString() {
      String ValueMbox =
              "Me <@example.org,@example.net,@example.com:me@x.example>";
      NamedAddress result = new NamedAddress(ValueMbox);
      {
        String stringTemp = result.toString();
        Assert.assertEquals(
          "Me <me@x.example>",
          stringTemp);
      }
    }
  }
