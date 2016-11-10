package com.upokecenter.test; import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;

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
      {
String stringTemp = ContentDisposition.MakeFilename ("hello. txt");
Assert.assertEquals(
  "hello._txt",
  stringTemp);
}
      {
String stringTemp = ContentDisposition.MakeFilename ("hello .txt");
Assert.assertEquals(
  "hello_.txt",
  stringTemp);
}
            {
String stringTemp = ContentDisposition.MakeFilename ("hello . txt");
Assert.assertEquals(
  "hello_._txt",
  stringTemp);
}
 {
String stringTemp = ContentDisposition.MakeFilename ("hello._");
Assert.assertEquals(
  "hello._",
  stringTemp);
}
 String stringTemp;
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?long_filename?=");
        Assert.assertEquals(
          "long filename",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf=?utf-8?q?test?=");
        Assert.assertEquals(
          "=_utftest",
          stringTemp);
      }
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q=?utf-8?q?test?=");
        Assert.assertEquals(
          "=_utf-8_qtest",
          stringTemp);
      stringTemp =
        ContentDisposition.MakeFilename("=?utf-8?=?utf-8?q?test?=");
      Assert.assertEquals(
        "=_utf-8_test",
        stringTemp);
      stringTemp =
        ContentDisposition.MakeFilename("=?utf-8?q?t=?utf-8?q?test?=");
      Assert.assertEquals(
        "ttest",
        stringTemp);
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?long_filename?=");
        Assert.assertEquals(
          "long filename",
          stringTemp);
      }

      {
        stringTemp = ContentDisposition.MakeFilename("utf-8'en'hello%2Etxt");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("=?utf-8?q?hello.txt?=");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }

      stringTemp =
  ContentDisposition.MakeFilename(" " + " " + "hello.txt");
      Assert.assertEquals(
        "hello.txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello" + " " + " " + "txt");
      Assert.assertEquals(
        "hello txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello.txt" + " " + " ");
      Assert.assertEquals(
        "hello.txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename(" "  + "hello.txt");
      Assert.assertEquals(
        "hello.txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello" + " "  + "txt");
      Assert.assertEquals(
        "hello txt",
        stringTemp);
      stringTemp =
  ContentDisposition.MakeFilename("hello.txt" + " ");
      Assert.assertEquals(
        "hello.txt",
        stringTemp);

      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?___hello.txt___?=");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?a?= =?utf-8?q?b?=");
        Assert.assertEquals(
          "ab",
          stringTemp);
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?a?= =?x-unknown?q?b?=");
        Assert.assertEquals(
          "a b",
          stringTemp);
        stringTemp =
          ContentDisposition.MakeFilename("a" + " " + " " + " " + "b");
        Assert.assertEquals(
          "a b",
          stringTemp);
      {
        stringTemp = ContentDisposition.MakeFilename("com0.txt");
        Assert.assertEquals("_com0.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("-hello.txt");
        Assert.assertEquals("_-hello.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("lpt0.txt");
        Assert.assertEquals("_lpt0.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("com1.txt");
        Assert.assertEquals("_com1.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("lpt1.txt");
        Assert.assertEquals("_lpt1.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("nul.txt");
        Assert.assertEquals("_nul.txt", stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("prn.txt");
        Assert.assertEquals("_prn.txt", stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("aux.txt");
        Assert.assertEquals("_aux.txt", stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("con.txt");
        Assert.assertEquals("_con.txt", stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "  =?utf-8?q?hello.txt?=  ");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?utf-8?q?___hello.txt___?=  ");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?utf-8*en?q?___hello.txt___?=  ");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?utf-8*?q?___hello.txt___?=  ");
        Assert.assertEquals(
          "___hello.txt___",
          stringTemp);
      }
      {
        stringTemp =

  ContentDisposition.MakeFilename(
  "  =?utf-8*i-unknown?q?___hello.txt___?=  ");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("  =?*en?q?___hello.txt___?=  ");
        Assert.assertEquals(
          "___hello.txt___",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?iso-8859-1?q?a=E7=E3o.txt?=");
        Assert.assertEquals(
          "a\u00e7\u00e3o.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("a\u00e7\u00e3o.txt");
        Assert.assertEquals(
          "a\u00e7\u00e3o.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "=?x-unknown?q?hello.txt?=");
        Assert.assertEquals(
          "hello.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("=?x-unknown");
        Assert.assertEquals(
          "=_x-unknown",
          stringTemp);
      }
      {
        stringTemp =
            ContentDisposition.MakeFilename("my?file<name>.txt");
        Assert.assertEquals(
          "my_file_name_.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("my file\tname\".txt");
        Assert.assertEquals(
          "my file name_.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "my\ud800file\udc00name\ud800\udc00.txt");
        Assert.assertEquals(
          "my\ufffdfile\ufffdname\ud800\udc00.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?x-unknown?Q?file\ud800name?=");
        Assert.assertEquals(
          "file\ufffdname",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(
          "utf-8''file%c2%bename.txt");
        Assert.assertEquals(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("utf-8'en'file%c2%bename.txt");
        Assert.assertEquals(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("windows-1252'en'file%bename.txt");
        Assert.assertEquals(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("x-unknown'en'file%c2%bename.txt");
        Assert.assertEquals(
          "x-unknown'en'file_c2_bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("utf-8'en-us'file%c2%bename.txt");
        Assert.assertEquals(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("utf-8''file%c2%bename.txt");
        Assert.assertEquals(
          "file\u00bename.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("...");
        Assert.assertEquals(
          "_..._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("~home");
        Assert.assertEquals(
          "_~home",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("~nul");
        Assert.assertEquals(
          "_~nul",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("myfilename.txt.");
        Assert.assertEquals(
          "myfilename.txt._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("nul");
        Assert.assertEquals(
          "_nul",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("   nul   ");
        Assert.assertEquals(
          "_nul",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("   ordinary   ");
        Assert.assertEquals(
          "ordinary",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("nul.txt");
        Assert.assertEquals(
          "_nul.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("con");
        Assert.assertEquals(
          "_con",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("aux");
        Assert.assertEquals(
          "_aux",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("lpt1device");
        Assert.assertEquals(
          "_lpt1device",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("my\u0001file\u007fname*.txt");
        Assert.assertEquals(
          "my_file_name_.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?utf-8?q?folder\\hello.txt?=");
        Assert.assertEquals(
          "folder_hello.txt",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("folder/");
        Assert.assertEquals(
          "folder_",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("folder//////");
        Assert.assertEquals(
          "folder______",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename(".");
        Assert.assertEquals(
          "_._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("..");
        Assert.assertEquals(
          "_.._",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("fol/der/");
        Assert.assertEquals(
          "fol_der_",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("fol/der//////");
        Assert.assertEquals(
          "fol_der______",
          stringTemp);
      }
      {
        stringTemp = ContentDisposition.MakeFilename("folder/hello.txt");
        Assert.assertEquals(
          "folder_hello.txt",
          stringTemp);
      }
      {
        stringTemp =
            ContentDisposition.MakeFilename("fol/der/hello.txt");
        Assert.assertEquals(
          "fol_der_hello.txt",
          stringTemp);
      }
      {
        stringTemp =
          ContentDisposition.MakeFilename("=?x-unknown?q?folder\\hello.txt?=");
        Assert.assertEquals(
          "folder_hello.txt",
          stringTemp);
      }
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
} catch (NullPointerException ex) {
new Object();
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
