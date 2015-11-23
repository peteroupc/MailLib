package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

import com.upokecenter.text.*;
import org.junit.Assert;
import org.junit.Test;

  public class EncodingsTest {
    @Test
    public void TestDecodeToString() {
      // not implemented yet
    }
    @Test
    public void TestEncodeToBytes() {
      try {
        ICharacterInput ici = null;
 Encodings.EncodeToBytes(ici, Encodings.UTF8);
Assert.fail("Should have failed");
} catch (NullPointerException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 Encodings.EncodeToBytes("test",null);
Assert.fail("Should have failed");
} catch (NullPointerException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
    }
    @Test
    public void TestEncodeToWriter() {
      // not implemented yet
    }
    @Test
    public void TestGetDecoderInput() {
      // not implemented yet
    }
    @Test
    public void TestGetEncoding() {
      if ((Encodings.GetEncoding("utf-8")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("Utf-8")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("uTf-8")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("utF-8")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("UTF-8")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("utg-8")) != null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("utf-9")) != null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("   utf-8    ")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("   utf-8")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("utf-8    ")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("\t\tutf-8\t\t")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding(" \r\n utf-8 \r ")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("\nutf-8\n")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("\tutf-8\t")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("\rutf-8\r")) == null) {
        Assert.fail();
      }
      if ((Encodings.GetEncoding("\futf-8\f")) == null) {
        Assert.fail();
      }
    }
    @Test
    public void TestInputToString() {
      // not implemented yet
    }
    @Test
    public void TestResolveAlias() {
      // not implemented yet
    }
    @Test
    public void TestResolveAliasForEmail() {
      // not implemented yet
    }
    @Test
    public void TestStringToBytes() {
      // not implemented yet
    }
    @Test
    public void TestStringToInput() {
      try {
 Encodings.StringToInput(null, 0, 0);
Assert.fail("Should have failed");
} catch (NullPointerException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 Encodings.StringToInput("t",-1,1);
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 Encodings.StringToInput("t",5,1);
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 Encodings.StringToInput("t",0,-1);
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 Encodings.StringToInput("t",0,5);
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
      try {
 Encodings.StringToInput("tt",1,2);
Assert.fail("Should have failed");
} catch (IllegalArgumentException ex) {
System.out.println(ex.getMessage());
} catch (Exception ex) {
 Assert.fail(ex.toString());
throw new IllegalStateException("", ex);
}
    }
  }
