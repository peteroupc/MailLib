using NUnit.Framework;
using PeterO.Text;
using System;

namespace MailLibTest {
  [TestFixture]
  public partial class EncodingsTest {
    [Test]
    public void TestDecodeToString() {
      // not implemented yet
    }
    [Test]
    public void TestEncodeToBytes() {
      try {
        ICharacterInput ici = null;
        Encodings.EncodeToBytes(ici, Encodings.UTF8);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Encodings.EncodeToBytes("test", null);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
    [Test]
    public void TestEncodeToWriter() {
      // not implemented yet
    }
    [Test]
    public void TestGetDecoderInput() {
      // not implemented yet
    }
    [Test]
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
    [Test]
    public void TestInputToString() {
      // not implemented yet
    }
    [Test]
    public void TestResolveAlias() {
      Assert.AreEqual(String.Empty, Encodings.ResolveAlias(null));
      Assert.AreEqual(String.Empty, Encodings.ResolveAlias(String.Empty));
      {
        string stringTemp = Encodings.ResolveAlias("iso-8859-1");
        Assert.AreEqual(
        "windows-1252",
        stringTemp);
      }
      {
        string stringTemp = Encodings.ResolveAlias("windows-1252");
        Assert.AreEqual(
        "windows-1252",
        stringTemp);
      }
      {
        string stringTemp = Encodings.ResolveAlias("us-ascii");
        Assert.AreEqual(
        "windows-1252",
        stringTemp);
      }
      Assert.AreEqual(String.Empty, Encodings.ResolveAlias("utf-7"));
      Assert.AreEqual(String.Empty, Encodings.ResolveAlias("replacement"));
      {
        string stringTemp = Encodings.ResolveAlias("hz-gb-2312");
        Assert.AreEqual(
        "replacement",
        stringTemp);
      }
    }
    [Test]
    public void TestResolveAliasForEmail() {
      Assert.AreEqual(String.Empty, Encodings.ResolveAliasForEmail(null));
      Assert.AreEqual(String.Empty,
           Encodings.ResolveAliasForEmail(String.Empty));
      {
        string stringTemp = Encodings.ResolveAliasForEmail("iso-8859-1");
        Assert.AreEqual(
        "iso-8859-1",
        stringTemp);
      }
      {
        string stringTemp = Encodings.ResolveAliasForEmail("windows-1252");
        Assert.AreEqual(
        "windows-1252",
        stringTemp);
      }
      {
        string stringTemp = Encodings.ResolveAliasForEmail("us-ascii");
        Assert.AreEqual(
        "us-ascii",
        stringTemp);
      }
      {
        string stringTemp = Encodings.ResolveAliasForEmail("utf-7");
        Assert.AreEqual(
        "utf-7",
        stringTemp);
      }
      Assert.AreEqual(String.Empty, Encodings.ResolveAliasForEmail(
    "replacement"));
      {
        string stringTemp = Encodings.ResolveAliasForEmail("hz-gb-2312");
        Assert.AreEqual(
        "replacement",
        stringTemp);
      }
    }

    [Test]
    public void TestStringToBytes() {
      // not implemented yet
    }
    [Test]
    public void TestStringToInput() {
      try {
        Encodings.StringToInput(null, 0, 0);
        Assert.Fail("Should have failed");
      } catch (ArgumentNullException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Encodings.StringToInput("t", -1, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Encodings.StringToInput("t", 5, 1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Encodings.StringToInput("t", 0, -1);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Encodings.StringToInput("t", 0, 5);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
      try {
        Encodings.StringToInput("tt", 1, 2);
        Assert.Fail("Should have failed");
      } catch (ArgumentException) {
        Console.Write(String.Empty);
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
        throw new InvalidOperationException(String.Empty, ex);
      }
    }
  }
}
