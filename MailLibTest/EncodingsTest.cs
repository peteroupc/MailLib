using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MailLibTest {
  [TestClass]
  public partial class EncodingsTest {
    [TestMethod]
    public void TestDecodeToString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestEncodeToBytes() {
      //ArgNull(Encodings.EncodeToBytes(null, Encodings.UTF8);
      //ArgNull(Encodings.EncodeToBytes("test",null);
    }
    [TestMethod]
    public void TestEncodeToWriter() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetDecoderInput() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetEncoding() {
      if ((Encodings.GetEncoding("utf-8")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("Utf-8")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("uTf-8")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("utF-8")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("UTF-8")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("utg-8")) != null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("utf-9")) != null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("   utf-8    ")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("   utf-8")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("utf-8    ")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("\t\tutf-8\t\t")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding(" \r\n utf-8 \r ")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("\nutf-8\n")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("\tutf-8\t")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("\rutf-8\r")) == null) {
        Assert.Fail();
      }
      if ((Encodings.GetEncoding("\futf-8\f")) == null) {
        Assert.Fail();
      }
    }
    [TestMethod]
    public void TestInputToString() {
      // not implemented yet
    }
    [TestMethod]
    public void TestResolveAlias() {
      // not implemented yet
    }
    [TestMethod]
    public void TestResolveAliasForEmail() {
      // not implemented yet
    }
    [TestMethod]
    public void TestStringToBytes() {
      // not implemented yet
    }
    [TestMethod]
    public void TestStringToInput() {
      try {
 Encodings.StringToInput(null, 0, 0);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 Encodings.StringToInput("t",-1,1);
Assert.Fail("Should have failed");
} catch (ArgumentException) {
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 Encodings.StringToInput("t",5,1);
Assert.Fail("Should have failed");
} catch (ArgumentException) {
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 Encodings.StringToInput("t",0,-1);
Assert.Fail("Should have failed");
} catch (ArgumentException) {
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 Encodings.StringToInput("t",0,5);
Assert.Fail("Should have failed");
} catch (ArgumentException) {
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 Encodings.StringToInput("tt",1,2);
Assert.Fail("Should have failed");
} catch (ArgumentException) {
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }
  }
}
