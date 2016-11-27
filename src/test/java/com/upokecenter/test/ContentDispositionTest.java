package com.upokecenter.test; import com.upokecenter.util.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.text.*;
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
            ContentDisposition cd = ContentDisposition.Parse ("inline");
            Assert.IsFalse (cd.isAttachment());
            cd = ContentDisposition.Parse ("cd-unknown");
            Assert.IsFalse (cd.isAttachment());
            cd = ContentDisposition.Parse ("attachment");
            Assert.assertTrue (cd.isAttachment());
        }
        @Test public void TestIsInline() {
            ContentDisposition cd = ContentDisposition.Parse ("inline");
            Assert.assertTrue (cd.isInline());
            cd = ContentDisposition.Parse ("cd-unknown");
            Assert.IsFalse (cd.isInline());
            cd = ContentDisposition.Parse ("attachment");
            Assert.IsFalse (cd.isInline());
        }

        private static String MakeQEncoding(String str) {
            byte [] bytes = DataUtilities.GetUtf8Bytes (str, false);
            StringBuilder sb = new StringBuilder();
            String hex = "0123456789ABCDEF";
            sb.append ("=?utf-8?q?");
            for (int i = 0; i < bytes.length; ++i) {
                int b = ((int)bytes [i]) & 0xff;
                if (b == 0x32) {
                    sb.append ('_');
                } else if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
                  (b >= '0' && b <= '9')) {
                    sb.append ((char)b);
                } else {
                    sb.append ('=');
                    sb.append (hex.get((b >> 4) & 15));
                    sb.append (hex.get(b & 15));
                }
            }
            sb.append ("?=");
            return sb.toString();
        }
        private static String MakeRfc2231Encoding(String str) {
            byte [] bytes = DataUtilities.GetUtf8Bytes (str, false);
            StringBuilder sb = new StringBuilder();
            String hex = "0123456789ABCDEF";
            sb.append ("utf-8''");
            for (int i = 0; i < bytes.length; ++i) {
                int b = ((int)bytes [i]) & 0xff;
                if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
                  (b >= '0' && b <= '9')) {
                    sb.append ((char)b);
                } else {
                    sb.append ('%');
                    sb.append (hex.get((b >> 4) & 15));
                    sb.append (hex.get(b & 15));
                }
            }
            return sb.toString();
        }
        private static String RandomString(RandomGenerator rnd) {
            String ret = EncodingTest.RandomString (rnd);
            int ui = rnd.UniformInt (100);
            if (ui < 20) {
                ret = MakeQEncoding (ret);
            } else if (ui < 25) {
                ret = MakeRfc2231Encoding (ret);
            }
            return ret;
        }
        private boolean IsGoodFilename(String str) {
            if (str == null || str.length() == 0 || str.length() > 255) {
                return false;
            }
            if (str.get(str.length() - 1) == '.' || str.get(str.length() - 1) == '~') {
                return false;
            }
            String strLower = DataUtilities.ToLowerCaseAscii (str);
            boolean bracketDigit = str.get(0) == '{' && str.length() > 1 &&
                    str.get(1) >= '0' && str.get(1) <= '9';
          boolean homeFolder = str.get(0) == '~' || str.get(0) == '-' || str.get(0) ==
              '$' ;
            boolean period = str.get(0) == '.';
          boolean beginEndSpace = str.get(0) == 0x20 || str.get(str.length() - 1) ==
              0x20;
            if (bracketDigit || homeFolder || period || beginEndSpace) {
                return false;
            }
            // Reserved filenames on Windows
            boolean reservedFilename = strLower.equals (
        "nul") || strLower.equals ("clock$") || strLower.IndexOf (
        "nul.",
        StringComparison.Ordinal) == 0 || strLower.equals (
        "prn") || strLower.IndexOf (
        "prn.",
        StringComparison.Ordinal) == 0 || strLower.equals (
        "aux") || strLower.IndexOf (
        "aux.",
        StringComparison.Ordinal) == 0 || strLower.equals (
        "con") || strLower.IndexOf (
        "con.",
        StringComparison.Ordinal) == 0 || (
        strLower.length() >= 4 && strLower.IndexOf (
        "lpt",
        StringComparison.Ordinal) == 0 && strLower.get(3) >= '0' &&
             strLower.get(3) <= '9') || (strLower.length() >= 4 &&
                    strLower.IndexOf (
        "com",
        StringComparison.Ordinal) == 0 && strLower.get(3) >= '0' &&
                  strLower.get(3) <= '9');
            if (reservedFilename) {
                return false;
            }
            for (int i = 0; i < str.length(); ++i) {
                char c = str.get(i);
                if (c < 0x20 || (c >= 0x7f && c <= 0x9f) ||
                  c == '%' || c == 0x2028 || c == 0x2029 ||
                c == '\\' || c == '/' || c == '*' || c == '?' || c == '|' ||
                  c == ':' || c == '<' || c == '>' || c == '"' ||
                c == 0xa0 || c == 0x3000 || c == 0x180e || c == 0x1680 ||
   (c >= 0x2000 && c <= 0x200b) || c == 0x205f || c == 0x202f || c == 0xfeff
                   ||
                 (c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <= 0xfdef)) {
                    return false;
                }
            }
            // Avoid space before and after last dot
            for (var i = str.length() - 1; i >= 0; --i) {
                if (str.get(i) == '.') {
                 boolean spaceAfter = (i + 1 < str.length() && str.get(i + 1) ==
                      0x20);
                    boolean spaceBefore = (i > 0 && str.get(i - 1) == 0x20);
                    if (spaceAfter || spaceBefore) {
                    return false;
                    }
                    break;
                }
            }
            return (NormalizingCharacterInput.IsNormalized (
              str,
              Normalization.NFC));
        }

        @Test public void TestMakeFilename () {
            String stringTemp;
      RandomGenerator  rnd = new RandomGenerator (new XorShift128Plus(false));
        Assert.assertEquals ("", ContentDisposition.MakeFilename
              (null));
            {
                stringTemp = ContentDisposition.MakeFilename ("");
                Assert.assertEquals (
                  "_",
                  stringTemp);
            }
            String mfn = ContentDisposition.MakeFilename (
              "utf-8''%2A%EF%AB%87%EC%A5%B2%2B67%20Tqd%20R%E3%80%80%2E");
            Assert.assertTrue (IsGoodFilename (mfn), mfn);
            for (int i = 0; i < 1000000; ++i) {
        if (i % 1000 == 0) {
          System.out.println (i);
        }
                String str = RandomString (rnd);
                String filename = ContentDisposition.MakeFilename (str);
                if (!IsGoodFilename (filename)) {
            Assert.fail ("str_____=" + EncodingTest.EscapeString (str) +
                      "\n" +
                    "filename=" + EncodingTest.EscapeString (filename) + "\n" +
  "Assert.assertTrue(IsGoodFilename(ContentDisposition.MakeFilename(\n" +
                    "  \"" + EncodingTest.EscapeString (str) + "\")));");
                }
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("hello. txt");
                Assert.assertEquals (
                  "hello._txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("hello .txt");
                Assert.assertEquals (
                  "hello_.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("hello . txt");
                Assert.assertEquals (
                  "hello_._txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("hello._");
                Assert.assertEquals (
                  "hello._",
                  stringTemp);
            }
            {
                stringTemp =
                  ContentDisposition.MakeFilename ("=?utf-8?q?long_filename?=");
                Assert.assertEquals (
                  "long filename",
                  stringTemp);
            }
            {
                stringTemp =
                  ContentDisposition.MakeFilename ("=?utf=?utf-8?q?test?=");
                Assert.assertEquals (
                  "=_utftest",
                  stringTemp);
            }
            stringTemp =
              ContentDisposition.MakeFilename ("=?utf-8?q=?utf-8?q?test?=");
            Assert.assertEquals (
              "=_utf-8_qtest",
              stringTemp);
            stringTemp =
              ContentDisposition.MakeFilename ("=?utf-8?=?utf-8?q?test?=");
            Assert.assertEquals (
              "=_utf-8_test",
              stringTemp);
            stringTemp =
              ContentDisposition.MakeFilename ("=?utf-8?q?t=?utf-8?q?test?=");
            Assert.assertEquals (
              "ttest",
              stringTemp);
            {
                stringTemp =
                  ContentDisposition.MakeFilename ("=?utf-8?q?long_filename?=");
                Assert.assertEquals (
                  "long filename",
                  stringTemp);
            }

            {
         stringTemp = ContentDisposition.MakeFilename
                  ("utf-8'en'hello%2Etxt");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }
            {
        stringTemp = ContentDisposition.MakeFilename
                  ("=?utf-8?q?hello.txt?=");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }

            stringTemp =
        ContentDisposition.MakeFilename (" " + " " + "hello.txt");
            Assert.assertEquals (
              "hello.txt",
              stringTemp);
            stringTemp =
        ContentDisposition.MakeFilename ("hello" + " " + " " + "txt");
            Assert.assertEquals (
              "hello txt",
              stringTemp);
            stringTemp =
        ContentDisposition.MakeFilename ("hello.txt" + " " + " ");
            Assert.assertEquals (
              "hello.txt",
              stringTemp);
            stringTemp =
        ContentDisposition.MakeFilename (" " + "hello.txt");
            Assert.assertEquals (
              "hello.txt",
              stringTemp);
            stringTemp =
        ContentDisposition.MakeFilename ("hello" + " " + "txt");
            Assert.assertEquals (
              "hello txt",
              stringTemp);
            stringTemp =
        ContentDisposition.MakeFilename ("hello.txt" + " ");
            Assert.assertEquals (
              "hello.txt",
              stringTemp);

            {
                stringTemp =
               ContentDisposition.MakeFilename
                    ("=?utf-8?q?___hello.txt___?=");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }
            stringTemp =
              ContentDisposition.MakeFilename ("=?utf-8?q?a?= =?utf-8?q?b?=");
            Assert.assertEquals (
              "ab",
              stringTemp);
            stringTemp =
           ContentDisposition.MakeFilename
                ("=?utf-8?q?a?= =?x-unknown?q?b?=");
            Assert.assertEquals (
              "a b",
              stringTemp);
            stringTemp =
              ContentDisposition.MakeFilename ("a" + " " + " " + " " + "b");
            Assert.assertEquals (
              "a b",
              stringTemp);
            {
                stringTemp = ContentDisposition.MakeFilename ("com0.txt");
                Assert.assertEquals ("_com0.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("-hello.txt");
                Assert.assertEquals ("_-hello.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("lpt0.txt");
                Assert.assertEquals ("_lpt0.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("com1.txt");
                Assert.assertEquals ("_com1.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("lpt1.txt");
                Assert.assertEquals ("_lpt1.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("nul.txt");
                Assert.assertEquals ("_nul.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("prn.txt");
                Assert.assertEquals ("_prn.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("aux.txt");
                Assert.assertEquals ("_aux.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("con.txt");
                Assert.assertEquals ("_con.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename (
                  "  =?utf-8?q?hello.txt?=  ");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
           ContentDisposition.MakeFilename
                    ("  =?utf-8?q?___hello.txt___?=  ");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
        ContentDisposition.MakeFilename
                    ("  =?utf-8*en?q?___hello.txt___?=  ");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
          ContentDisposition.MakeFilename
                    ("  =?utf-8*?q?___hello.txt___?=  ");
                Assert.assertEquals (
                  "___hello.txt___",
                  stringTemp);
            }
            {
                stringTemp =

          ContentDisposition.MakeFilename (
          "  =?utf-8*i-unknown?q?___hello.txt___?=  ");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
             ContentDisposition.MakeFilename
                    ("  =?*en?q?___hello.txt___?=  ");
                Assert.assertEquals (
                  "___hello.txt___",
                  stringTemp);
            }
            {
                stringTemp =
             ContentDisposition.MakeFilename
                    ("=?iso-8859-1?q?a=E7=E3o.txt?=");
                Assert.assertEquals (
                  "a\u00e7\u00e3o.txt",
                  stringTemp);
            }
            {
           stringTemp = ContentDisposition.MakeFilename
                  ("a\u00e7\u00e3o.txt");
                Assert.assertEquals (
                  "a\u00e7\u00e3o.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename (
                  "=?x-unknown?q?hello.txt?=");
                Assert.assertEquals (
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("=?x-unknown");
                Assert.assertEquals (
                  "=_x-unknown",
                  stringTemp);
            }
            {
                stringTemp =
                    ContentDisposition.MakeFilename ("my?file<name>.txt");
                Assert.assertEquals (
                  "my_file_name_.txt",
                  stringTemp);
            }
            {
          stringTemp = ContentDisposition.MakeFilename
                  ("my file\tname\".txt");
                Assert.assertEquals (
                  "my file name_.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename (
                  "my\ud800file\udc00name\ud800\udc00.txt");
                Assert.assertEquals (
                  "my\ufffdfile\ufffdname\ud800\udc00.txt",
                  stringTemp);
            }
            {
                stringTemp =
            ContentDisposition.MakeFilename
                    ("=?x-unknown?Q?file\ud800name?=");
                Assert.assertEquals (
                  "file\ufffdname",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename (
                  "utf-8''file%c2%bename.txt");
                Assert.assertEquals (
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp =
               ContentDisposition.MakeFilename
                    ("utf-8'en'file%c2%bename.txt");
                Assert.assertEquals (
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp =
           ContentDisposition.MakeFilename
                    ("windows-1252'en'file%bename.txt");
                Assert.assertEquals (
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp =
           ContentDisposition.MakeFilename
                    ("x-unknown'en'file%c2%bename.txt");
                Assert.assertEquals (
                  "x-unknown'en'file_c2_bename.txt",
                  stringTemp);
            }
            {
                stringTemp =
            ContentDisposition.MakeFilename
                    ("utf-8'en-us'file%c2%bename.txt");
                Assert.assertEquals (
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp =
                  ContentDisposition.MakeFilename ("utf-8''file%c2%bename.txt");
                Assert.assertEquals (
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("...");
                Assert.assertEquals (
                  "_..._",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("~home");
                Assert.assertEquals (
                  "_~home",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("~nul");
                Assert.assertEquals (
                  "_~nul",
                  stringTemp);
            }
            {
              stringTemp = ContentDisposition.MakeFilename
                  ("myfilename.txt.");
                Assert.assertEquals (
                  "myfilename.txt._",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("nul");
                Assert.assertEquals (
                  "_nul",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("   nul   ");
                Assert.assertEquals (
                  "_nul",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("   ordinary   ");
                Assert.assertEquals (
                  "ordinary",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("nul.txt");
                Assert.assertEquals (
                  "_nul.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("con");
                Assert.assertEquals (
                  "_con",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("aux");
                Assert.assertEquals (
                  "_aux",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("lpt1device");
                Assert.assertEquals (
                  "_lpt1device",
                  stringTemp);
            }
            {
                stringTemp =
               ContentDisposition.MakeFilename
                    ("my\u0001file\u007fname*.txt");
                Assert.assertEquals (
                  "my_file_name_.txt",
                  stringTemp);
            }
            {
                stringTemp =
             ContentDisposition.MakeFilename
                    ("=?utf-8?q?folder\\hello.txt?=");
                Assert.assertEquals (
                  "folder_hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("folder/");
                Assert.assertEquals (
                  "folder_",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("folder//////");
                Assert.assertEquals (
                  "folder______",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename (".");
                Assert.assertEquals (
                  "_._",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("..");
                Assert.assertEquals (
                  "_.._",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("fol/der/");
                Assert.assertEquals (
                  "fol_der_",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename ("fol/der//////");
                Assert.assertEquals (
                  "fol_der______",
                  stringTemp);
            }
            {
             stringTemp = ContentDisposition.MakeFilename
                  ("folder/hello.txt");
                Assert.assertEquals (
                  "folder_hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
                    ContentDisposition.MakeFilename ("fol/der/hello.txt");
                Assert.assertEquals (
                  "fol_der_hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
         ContentDisposition.MakeFilename
                    ("=?x-unknown?q?folder\\hello.txt?=");
                Assert.assertEquals (
                  "folder_hello.txt",
                  stringTemp);
            }
        }

        @Test public void TestParameters () {
        }
        @Test public void TestParse () {
            try {
                ContentDisposition.Parse (null);
                Assert.fail ("Should have failed");
            } catch (NullPointerException ex) {
                new Object();
            } catch (Exception ex) {
                Assert.fail (ex.toString());
                throw new IllegalStateException ("", ex);
            }
        }
        @Test public void TestToString () {
            // not implemented yet
        }
    }
