using NUnit.Framework;
using PeterO.Mail;
using System;
namespace MailLibTest {
  [TestFixture]
  public partial class ContentDispositionTest {
    [Test]
    public void TestDispositionType() {
      // not implemented yet
    }
    [Test]
    public void TestEquals() {
      // not implemented yet
    }
    [Test]
    public void TestGetHashCode() {
      // not implemented yet
    }
    [Test]
    public void TestGetParameter() {
      // not implemented yet
    }
    [Test]
    public void TestIsAttachment() {
      ContentDisposition cd=ContentDisposition.Parse("inline");
      Assert.IsFalse(cd.IsAttachment);
      cd=ContentDisposition.Parse("cd-unknown");
      Assert.IsFalse(cd.IsAttachment);
      cd=ContentDisposition.Parse("attachment");
      Assert.IsTrue(cd.IsAttachment);
    }
    [Test]
    public void TestIsInline() {
      ContentDisposition cd=ContentDisposition.Parse("inline");
      Assert.IsTrue(cd.IsInline);
      cd=ContentDisposition.Parse("cd-unknown");
      Assert.IsFalse(cd.IsInline);
      cd=ContentDisposition.Parse("attachment");
      Assert.IsFalse(cd.IsInline);
    }

    [Test]
    public void TestMakeFilename() {
      Assert.AreEqual(String.Empty, ContentDisposition.MakeFilename(null));
      {
string stringTemp = ContentDisposition.MakeFilename ("hello. txt");
Assert.AreEqual(
  "hello._txt",
  stringTemp);
}
      {
string stringTemp = ContentDisposition.MakeFilename ("hello .txt");
Assert.AreEqual(
  "hello_.txt",
  stringTemp);
}
            {
string stringTemp = ContentDisposition.MakeFilename ("hello . txt");
Assert.AreEqual(
  "hello_._txt",
  stringTemp);
}
 {
string stringTemp = ContentDisposition.MakeFilename ("hello._");
Assert.AreEqual(
  "hello._",
  stringTemp);
}
 string stringTemp;
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?long_filename?=");
        Assert.AreEqual(
          "long filename",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf=?utf-8?q?test?=");
        Assert.AreEqual(
          "=_utftest",
          stringTemp);
      }
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q=?utf-8?q?test?=");
        Assert.AreEqual(
          "=_utf-8_qtest",
          stringTemp);
      stringTemp =
        ContentDisposition.MakeFilename("=?utf-8?=?utf-8?q?test?=");
      Assert.AreEqual(
        "=_utf-8_test",
        stringTemp);
      stringTemp =
        ContentDisposition.MakeFilename("=?utf-8?q?t=?utf-8?q?test?=");
      Assert.AreEqual(
        "ttest",
        stringTemp);
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?long_filename?=");
        Assert.AreEqual(
          "long filename",
          stringTemp);
      }

      {
        stringTemp = ContentDisposition.MakeFilename("utf-8'en'hello%2Etxt");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("=?utf-8?q?hello.txt?=");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }

      stringTemp =
  ContentDisposition.MakeFilename(" " + " " + "hello.txt");
      Assert.AreEqual(
        "hello.txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello" + " " + " " + "txt");
      Assert.AreEqual(
        "hello txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello.txt" + " " + " ");
      Assert.AreEqual(
        "hello.txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename(" "  + "hello.txt");
      Assert.AreEqual(
        "hello.txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello" + " "  + "txt");
      Assert.AreEqual(
        "hello txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello.txt" + " ");
      Assert.AreEqual(
        "hello.txt",
        stringTemp);

      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?___hello.txt___?=");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?a?= =?utf-8?q?b?=");
        Assert.AreEqual(
          "ab",
          stringTemp);
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?a?= =?x-unknown?q?b?=");
        Assert.AreEqual(
          "a b",
          stringTemp);
        stringTemp =
          ContentDisposition.MakeFilename("a" + " " + " " + " " + "b");
        Assert.AreEqual(
          "a b",
          stringTemp);
      {
        stringTemp = ContentDisposition.MakeFilename("com0.txt");
        Assert.AreEqual("_com0.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("-hello.txt");
        Assert.AreEqual("_-hello.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("lpt0.txt");
        Assert.AreEqual("_lpt0.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("com1.txt");
        Assert.AreEqual("_com1.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("lpt1.txt");
        Assert.AreEqual("_lpt1.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("nul.txt");
        Assert.AreEqual("_nul.txt", stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("prn.txt");
        Assert.AreEqual("_prn.txt", stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("aux.txt");
        Assert.AreEqual("_aux.txt", stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("con.txt");
        Assert.AreEqual("_con.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "  =?utf-8?q?hello.txt?=  ");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?utf-8?q?___hello.txt___?=  ");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?utf-8*en?q?___hello.txt___?=  ");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?utf-8*?q?___hello.txt___?=  ");
        Assert.AreEqual(
          "___hello.txt___",
          stringTemp);
      }
      {
        stringTemp =

  ContentDisposition.MakeFilename(
  "  =?utf-8*i-unknown?q?___hello.txt___?=  ");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?*en?q?___hello.txt___?=  ");
        Assert.AreEqual(
          "___hello.txt___",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?iso-8859-1?q?a=E7=E3o.txt?=");
        Assert.AreEqual(
          "a\u00e7\u00e3o.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("a\u00e7\u00e3o.txt");
        Assert.AreEqual(
          "a\u00e7\u00e3o.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "=?x-unknown?q?hello.txt?=");
        Assert.AreEqual(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("=?x-unknown");
        Assert.AreEqual(
          "=_x-unknown",
          stringTemp);
      }
      {
        stringTemp =
            ContentDisposition.MakeFilename("my?file<name>.txt");
        Assert.AreEqual(
          "my_file_name_.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("my file\tname\".txt");
        Assert.AreEqual(
          "my file name_.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "my\ud800file\udc00name\ud800\udc00.txt");
        Assert.AreEqual(
          "my\ufffdfile\ufffdname\ud800\udc00.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?x-unknown?Q?file\ud800name?=");
        Assert.AreEqual(
          "file\ufffdname",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "utf-8''file%c2%bename.txt");
        Assert.AreEqual(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("utf-8'en'file%c2%bename.txt");
        Assert.AreEqual(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("windows-1252'en'file%bename.txt");
        Assert.AreEqual(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("x-unknown'en'file%c2%bename.txt");
        Assert.AreEqual(
          "x-unknown'en'file_c2_bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("utf-8'en-us'file%c2%bename.txt");
        Assert.AreEqual(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("utf-8''file%c2%bename.txt");
        Assert.AreEqual(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("...");
        Assert.AreEqual(
          "_..._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("~home");
        Assert.AreEqual(
          "_~home",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("~nul");
        Assert.AreEqual(
          "_~nul",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("myfilename.txt.");
        Assert.AreEqual(
          "myfilename.txt._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("nul");
        Assert.AreEqual(
          "_nul",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("   nul   ");
        Assert.AreEqual(
          "_nul",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("   ordinary   ");
        Assert.AreEqual(
          "ordinary",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("nul.txt");
        Assert.AreEqual(
          "_nul.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("con");
        Assert.AreEqual(
          "_con",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("aux");
        Assert.AreEqual(
          "_aux",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("lpt1device");
        Assert.AreEqual(
          "_lpt1device",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("my\u0001file\u007fname*.txt");
        Assert.AreEqual(
          "my_file_name_.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?folder\\hello.txt?=");
        Assert.AreEqual(
          "folder_hello.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("folder/");
        Assert.AreEqual(
          "folder_",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("folder//////");
        Assert.AreEqual(
          "folder______",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(".");
        Assert.AreEqual(
          "_._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("..");
        Assert.AreEqual(
          "_.._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("fol/der/");
        Assert.AreEqual(
          "fol_der_",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("fol/der//////");
        Assert.AreEqual(
          "fol_der______",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("folder/hello.txt");
        Assert.AreEqual(
          "folder_hello.txt",
          stringTemp);
      }
      {
        stringTemp =
            ContentDisposition.MakeFilename("fol/der/hello.txt");
        Assert.AreEqual(
          "fol_der_hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?x-unknown?q?folder\\hello.txt?=");
        Assert.AreEqual(
          "folder_hello.txt",
          stringTemp);
      }
    }
    [Test]
    public void TestParameters() {
      // not implemented yet
    }
    [Test]
    public void TestParse() {
      try {
 ContentDisposition.Parse(null);
Assert.Fail("Should have failed");
} catch (ArgumentNullException) {
new Object();
} catch (Exception ex) {
 Assert.Fail(ex.ToString());
throw new InvalidOperationException(String.Empty, ex);
}
    }
    [Test]
    public void TestToString() {
      // not implemented yet
    }
  }
}
