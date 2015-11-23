using System;
using System.Collections.Generic;
using System.Text;
using PeterO.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MailLibTest {
  [TestClass]
  public partial class NormalizingCharacterInputTest {
    [TestMethod]
    public void TestConstructor() {
      // not implemented yet
    }
    [TestMethod]
    public void TestGetChars() {
      // not implemented yet
    }
    [TestMethod]
    public void TestIsNormalized() {
      // Additional normalization tests
      Assert.IsFalse(NormalizingCharacterInput.IsNormalized(
        "x\u0300\u0323yz",
        Normalization.NFC));
      Assert.IsFalse(NormalizingCharacterInput.IsNormalized(
        "x\u0300\u0323",
        Normalization.NFC));
      try {
  NormalizingCharacterInput.IsNormalized((ICharacterInput)null,
    Normalization.NFC);
Assert.Fail("Should have failed");
} catch(ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      }
    [TestMethod]
    public void TestNormalize() {
      // not implemented yet
    }
    [TestMethod]
    public void TestRead() {
      var nci=new NormalizingCharacterInput("test");
      try {
 nci.Read(null, 0, 0);
Assert.Fail("Should have failed");
} catch(ArgumentNullException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 nci.Read(new int[] { 't' },-1,1);
Assert.Fail("Should have failed");
} catch(ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 nci.Read(new int[] { 't' },5,1);
Assert.Fail("Should have failed");
} catch(ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 nci.Read(new int[] { 't' },0,-1);
Assert.Fail("Should have failed");
} catch(ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 nci.Read(new int[] { 't' },0,5);
Assert.Fail("Should have failed");
} catch(ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      try {
 nci.Read(new int[] { 't','t' },1,2);
Assert.Fail("Should have failed");
} catch(ArgumentException ex) {
Console.WriteLine(ex.Message);
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
      Assert.AreEqual(1,nci.Read(new int[] { 't','t' },1,1));
    }
    [TestMethod]
    public void TestReadChar() {
      // not implemented yet
    }
  }
}
