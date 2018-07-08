package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

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
            ContentDisposition cd = ContentDisposition.Parse("inline");
            if (cd.isAttachment()) {
 Assert.fail();
 }
            cd = ContentDisposition.Parse("cd-unknown");
            if (cd.isAttachment()) {
 Assert.fail();
 }
            cd = ContentDisposition.Parse("attachment");
            if (!(cd.isAttachment())) {
 Assert.fail();
 }
        }

        @Test
public void TestIsInline() {
            ContentDisposition cd = ContentDisposition.Parse("inline");
            if (!(cd.isInline())) {
 Assert.fail();
 }
            cd = ContentDisposition.Parse("cd-unknown");
            if (cd.isInline()) {
 Assert.fail();
 }
            cd = ContentDisposition.Parse("attachment");
            if (cd.isInline()) {
 Assert.fail();
 }
        }

        private static String MakeQEncoding(String str) {
            byte[] bytes = DataUtilities.GetUtf8Bytes(str, false);
            StringBuilder sb = new StringBuilder();
            String hex = "0123456789ABCDEF";
            sb.append("=?utf-8?q?");
            for (int i = 0; i < bytes.length; ++i) {
                int b = ((int)bytes[i]) & 0xff;
                if (b == 0x32) {
                    sb.append('_');
                } else if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
                  (b >= '0' && b <= '9')) {
                    sb.append((char)b);
                } else {
                    sb.append('=');
                    sb.append(hex.charAt((b >> 4) & 15));
                    sb.append(hex.charAt(b & 15));
                }
            }
            sb.append("?=");
            return sb.toString();
        }

        private static String MakeRfc2231Encoding(String str) {
            byte[] bytes = DataUtilities.GetUtf8Bytes(str, false);
            StringBuilder sb = new StringBuilder();
            String hex = "0123456789ABCDEF";
            sb.append("utf-8''");
            for (int i = 0; i < bytes.length; ++i) {
                int b = ((int)bytes[i]) & 0xff;
                if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
                  (b >= '0' && b <= '9')) {
                    sb.append((char)b);
                } else {
                    sb.append('%');
                    sb.append(hex.charAt((b >> 4) & 15));
                    sb.append(hex.charAt(b & 15));
                }
            }
            return sb.toString();
        }

        private static String RandomString(RandomGenerator rnd) {
            String ret = EncodingTest.RandomString(rnd);
            int ui = rnd.UniformInt(100);
            if (ui < 20) {
                ret = MakeQEncoding(ret);
            } else if (ui < 25) {
                ret = MakeRfc2231Encoding(ret);
            }
            return ret;
        }

        static void FailFilename(String filename, String str) {
            FailFilename(filename, str, null);
        }

        static void FailFilename(
        String filename,
        String newName,
        String extra) {
            String failstr = "original=" + EncodingTest.EscapeString(filename) +
       "\nfilename=" + EncodingTest.EscapeString(newName) + "\n" +
      "AssertGoodFilename(\"" + EncodingTest.EscapeString(filename) +
               "\");" + (((extra)==null || (extra).length()==0) ? "" : "\n" +
                    extra);
            Assert.fail(failstr);
        }

        static void AssertGoodFilename(String filename) {
            String str = ContentDisposition.MakeFilename(filename);
            if (str == null || str.length() == 0 || str.length() > 255) {
                FailFilename(filename, str);
            }
            if (str.charAt(str.length() - 1) == '.' || str.charAt(str.length() - 1) == '~') {
                FailFilename(filename, str);
            }
            String strLower = DataUtilities.ToLowerCaseAscii(str);
            boolean bracketDigit = str.charAt(0) == '{' && str.length() > 1 &&
                    str.charAt(1) >= '0' && str.charAt(1) <= '9';
            boolean homeFolder = str.charAt(0) == '~' || str.charAt(0) == '-' || str.charAt(0) ==
                '$';
            boolean period = str.charAt(0) == '.';
            boolean beginEndSpace = str.charAt(0) == 0x20 || str.charAt(str.length() - 1) ==
                0x20;
            if (bracketDigit || homeFolder ||
                period || beginEndSpace) {
                FailFilename(filename, str);
            }
            // Reserved filenames on Windows
            if (strLower.equals(
       "nul")) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.equals("clock$")) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.indexOf(
              "nul.") == 0) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.equals(
              "prn")) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.indexOf(
              "prn.") == 0) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.indexOf(
              "![") >= 0) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.equals(
              "aux")) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.indexOf(
              "aux.") == 0) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.equals(
              "con")) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.indexOf(
              "con.") == 0) {
                {
                    FailFilename(filename, str, strLower);
                }
            }
            if (strLower.length() == 4 || (strLower.length() > 4 && (strLower.charAt(4)
              == '.' || strLower.charAt(4) == ' '))) {
                if (strLower.indexOf(
                  "lpt") == 0 && strLower.charAt(3) >= '0' &&
                    strLower.charAt(3) <= '9') {
                    {
                    FailFilename(filename, str, strLower);
                    }
                }
                if (strLower.indexOf(
                    "com") == 0 && strLower.charAt(3) >= '0' &&
                    strLower.charAt(3) <= '9') {
                    {
                    FailFilename(filename, str, strLower);
                    }
                }
            }

            int i;
            for (i = 0; i < str.length(); ++i) {
                char c = str.charAt(i);
                if (c < 0x20 || (c >= 0x7f && c <= 0x9f) ||
                  c == '%' || c == 0x2028 || c == 0x2029 ||
                c == '#' || c == ';' ||
                    c == '\\' || c == '/' || c == '*' || c == '?' || c == '|' ||
                  c == ':' || c == '<' || c == '>' || c == '"' || c == '`' ||
    c == '$' || c == 0xa0 || c == 0x3000 || c == 0x180e || c == 0x1680 ||
  (c >= 0x2000 && c <= 0x200b) || c == 0x205f || c == 0x202f || c == 0xfeff ||
                    (c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <= 0xfdef)) {
                    FailFilename(
  filename,
  str,
  "[" + EncodingTest.EscapeString("" + c) + "] index=" + i);
                }
                // Code points that decompose to "bad" characters
                if (c == 0x1fef) {
                    FailFilename(
            filename,
            str,
            "[" + EncodingTest.EscapeString("" + c) + "] index=" + i);
                }
            }
            if (str.indexOf("\u0020\u0020") >= 0) {
                FailFilename(filename, str);
            }
            if (str.indexOf("\u0020\t") >= 0) {
                FailFilename(filename, str);
            }
            if (str.indexOf("\t\u0020") >= 0) {
                FailFilename(filename, str);
            }

            // Avoid space before and after last dot
            for (i = str.length() - 1; i >= 0; --i) {
                if (str.charAt(i) == '.') {
                    boolean spaceAfter = i + 1 < str.length() && str.charAt(i + 1) == 0x20;
                    boolean spaceBefore = i > 0 && str.charAt(i - 1) == 0x20;
                    if (spaceAfter || spaceBefore) {
                    FailFilename(filename, str);
                    }
                    break;
                }
            }
            boolean finalRet = NormalizerInput.IsNormalized(
     str,
     Normalization.NFC);
            if (!finalRet) {
                FailFilename(filename, str);
            }
        }

        @Test
public void TestMakeFilenameSpecific1() {
            // Contains GREEK VARIA which decomposes to a "bad" filename
            // character
            AssertGoodFilename("xx\u1fefxx");
            AssertGoodFilename("xx!\ud845\udd33[xx");
            AssertGoodFilename("com20020");
            System.out.println(ContentDisposition.MakeFilename(
        "xx!\ud845\udd33[xx"));
            AssertGoodFilename("xx#xx");
            AssertGoodFilename("xx!\ud845\udd33[");
            AssertGoodFilename("xx![xx");
            String

  stmp =

  "0_@S\u2000\u164d7~?H |Hw\u669b\u007f\u12c6>kT\ud802\udda8 .hu-z\u2e03A\ua944}N.\u176cL\uffffJRm+{K `\"\u7ff2D\u2f2d7Q(\"[~JWP@Dy\u2f77voW\u202f\u001b\uf8e2;w\ud9d6\ude5c\udb66\udf9d?o\ufeff\u2f0c \u1384l1N_tG?\u2eae\u008f.\ubf99l5<p.@\"S\ufdefiU ~\u73d6ag/l7\ufdd0\u27a4u W$vZ!(jxf^eQ\u13ac \uf8a4l0\"\ufeffz3.2\u179b$V9,b\u24fe\u4b27S\u001bO\u072a|H6 e_\u2377.F*##xX\u007f ~\u2aa4xU\uc09b$MXPI\u007fMr\uc880 u#'Do5qR; |;:1(A\u200bRR*T*:xLAR#;%.||k?v4/~6EB..1\udacc\udd9bP\ud824\ude7d.|%'Z970<?u'\\gG\u3189V.es0.f\tu? %t&=6\u205f\u2a4f#F\u2000 {*\\ &(]Rk<u<|\ud80e\udeb7l>s}/JG'\u205f-t-k\uc774#.\u0020\u0020";
            AssertGoodFilename(stmp);

            stmp =

  "\u1492\u0f68r\t\ud9d6\udf2cquip %T;K=_%\u008fW\ufffe-\u2ef5u%.p\u0010~#uI_U0!6 {1?uIFA 0iO4y9 (\u53807aPz+\u009f}rj\u0a81 \u2446fo[\\? \u0757 I\t0\u19c8.z(c\uffffgc)J5VdW\ufeff4*\u200b%.\u00aaR\ufbbfw\u16d0Rm%\u001f\u189d\u00a0W\ufffe\u0bfa(9!.'\u1499.\u200b_m\ufffe>7v\u2000N},gch.get(3\ufffe\u2a07pD:gXhG_\u0000&\ufffeyT$&\u2fa5 N\ud8fd\udd2am \u0000)1\u009f$y1 \ud9e1\udc78\u001f;\u2bdd9.\u1d21B}\u1271 X.U|O/\ufeffQQ.A>cW!1'\u2374hmt \u0866\ufeff&t\\fe\u009f\u2cado?\ud9f5\ude56;\\b^\udb81\udf2d\u008f \u00a0\udbfd\udf53@.)7MD\u008fxwKs\u202f}\"\u17ff^m\u0010\ufdef'FF\u2000T>:\u2000h.J\u1e10\ufdefI$\u0355eIL`CDcH\u0dffrX4.'qx {. p:w. E64cF?1\\;";
            AssertGoodFilename(stmp);

            stmp =

  ".Zw\"\ud91f\udd46\u0010p *L {fN_\t5Wvs%Bka-\u00fb\ufdef@\uffffy|-\u001fDWqpX\uda60\udeadznb'\u00a0h1 \udb32\uddcah..\u0cab\u26eff\u21114\u2b8aSz,\t \u2d0d.Q\u009f\ufacfD\u0010E+r@!.MY/\u0010dFN\ubdb8\u21203\ud9af\udf5e`\u3000\u15ae _>vg";
            AssertGoodFilename(stmp);
            String str8675 =

  "\u216a2s\u1e19C<snhs\ud87a\ude8dX(\ufdef\ufdd0,u.y\u001c.|}Y \u2f18Yx\u2a11N%(..s3^(N\u0084`(r|41X_.})\ud84c\udef3\ufe3c/\\/ sq?G![{\ufeffZ\"qSMdgv3#dg\tK@^X;`jl\ud892\udcd3' e@5a(\u00a0 wg0g hH?5\u202flh\u04c1 \uffff(,\u044d qQ7b:uFs9m\u0b6b\\AT|HDAsH6's!_B>rb(q?KpUv;fa r!\u1dc2.5.U\\Ez\u1f5a/J.8`?U\u01ba\\/v\ufdef_p.%|}.;.(OL9\u00001O.getRV()\u2433z,E\u008f%o\u008f.fpDN=G {(\udac5\udd76XC\uffff..z\ud9e4\udc62^(u=|'93\u0f6bWvz\u0f09\u26d2$?y\ud9c5\udcd4P:)+iO\u009f[f?>JTo,Ge`:'I\u5ccf\u009f\u9c3a<+yC {\ub10bm(j\u7959.tL=\ud86a\udea3\\(i \u001fG0 +np\u180erFt.hoy ny)\".6 +j ";

            AssertGoodFilename(
  str8675);
            String str9309 =

  "K GY2n8 Uml\ufdd0U {\\\udbee\ude3brac,;8d\u3000i\ud965\uddd1W&9\ufffe`)nM@(\u1125=nZ:_='g5 ?g[\u1432S\ufdef/Y\u001fzF\ud84d\udda1\u009fb'C:\u00a0-M\u205f476\u001b\ud8c7\udc378=\"z\u0010\u2d33 {:\"mN\"5V!\ufdd0\u00a0> \u1680mdnR8\u03b2s\u008f^7*{yH\u001fil1>\u00108C6 \"p\u009fV,?/.C.o P9yP}s {\"{>\u205f\\(U\ufdd0\u205f!/ \"%K\u2000@ Y\u205fP/C,?O(\u03eb+\u009f\ubd8b\udbf3\udf10f.rv8\u009f%v6!]H6\u001bp`.\u008f:BVkI\u09f5|8!FQ\\Fp.88\u2000m\u0933 s.~cO$ fQoq\"\u3000\u6b07\ud8bd\udca6H\ud9be\udc02zY(N.h1\u0000|=!\ud845\udd33[\ua233o'dt;)H1p\u00a0?TVw5sZ\"\u205fF5.)M&?Kq<#\u0f96Td5Zr3@`~8.:";

            AssertGoodFilename(
        str9309);
            {
String stringTemp = ContentDisposition.MakeFilename("com1.x");
Assert.assertEquals(
  "_com1.x",
  stringTemp);
}
            {
String stringTemp = ContentDisposition.MakeFilename("lpt1.x");
Assert.assertEquals(
  "_lpt1.x",
  stringTemp);
}
            {
String stringTemp = ContentDisposition.MakeFilename("com1 .x");
Assert.assertEquals(
  "_com1_.x",
  stringTemp);
}
            {
String stringTemp = ContentDisposition.MakeFilename("lpt1 .x");
Assert.assertEquals(
  "_lpt_.x",
  stringTemp);
}
            {
String stringTemp = ContentDisposition.MakeFilename("com1 .x.y");
Assert.assertEquals(
  "_com1 .x.y",
  stringTemp);
}
            {
String stringTemp = ContentDisposition.MakeFilename("lpt1 .x.y");
Assert.assertEquals(
  "_lpt .x.y",
  stringTemp);
}
        }

        @Test(timeout = 200000)
        public void TestMakeFilename() {
            String stringTemp;
            RandomGenerator rnd = new RandomGenerator(new XorShift128Plus(false));
            {
                Object objectTemp = "";
                Object objectTemp2 = ContentDisposition.MakeFilename(
                    null);
                Assert.assertEquals(objectTemp, objectTemp2);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("");
                Assert.assertEquals(
                  "_",
                  stringTemp);
            }

  AssertGoodFilename("utf-8''%2A%EF%AB%87%EC%A5%B2%2B67%20Tqd%20R%E3%80%80%2E");
            for (int i = 0; i < 10000; ++i) {
                if (i % 1000 == 0) {
                    System.out.println(i);
                }

                AssertGoodFilename(RandomString(rnd));
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello. txt");
                Assert.assertEquals(
                  "hello._txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello .txt");
                Assert.assertEquals(
                  "hello_.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello . txt");
                Assert.assertEquals(
                  "hello_._txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello._");
                Assert.assertEquals(
                  "hello._",
                  stringTemp);
            }
            {
                stringTemp =
                  ContentDisposition.MakeFilename("=?utf-8?q?long_filename?=");
                Assert.assertEquals(
                  "long filename",
                  stringTemp);
            }
            stringTemp = ContentDisposition.MakeFilename("xx#xx");
            Assert.assertEquals(
              "xx_xx",
              stringTemp);
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
                stringTemp = ContentDisposition.MakeFilename(
                    "utf-8'en'hello%2Etxt");
                Assert.assertEquals(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    "=?utf-8?q?hello.txt?=");
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
        ContentDisposition.MakeFilename(" " + "hello.txt");
            Assert.assertEquals(
              "hello.txt",
              stringTemp);
            stringTemp =
        ContentDisposition.MakeFilename("hello" + " " + "txt");
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
                stringTemp =
  ContentDisposition.MakeFilename("=?iso-8859-1*en?q?fil=E7test?=");
                Assert.assertEquals(
                  "fil\u00e7test",
                  stringTemp);
            }
            {
                stringTemp =
  ContentDisposition.MakeFilename("=?iso-8859-1*en-us?q?fil=E7test?=");
                Assert.assertEquals(
                  "fil\u00e7test",
                  stringTemp);
            }
            {
                stringTemp =
  ContentDisposition.MakeFilename("=?iso-8859-1*xx9x9x?q?fil=E7test?=");
                Assert.assertEquals(
                  "fil\u00e7test",
                  stringTemp);
            }
            {
                stringTemp =
  ContentDisposition.MakeFilename("=?iso-8859-1*en?q?fil=e7test?=");
                Assert.assertEquals(
                  "fil\u00e7test",
                  stringTemp);
            }
            {
                stringTemp =
  ContentDisposition.MakeFilename("=?iso-8859-1*en-us?q?fil=e7test?=");
                Assert.assertEquals(
                  "fil\u00e7test",
                  stringTemp);
            }
            {
                stringTemp =
  ContentDisposition.MakeFilename("=?iso-8859-1*xx9x9x?q?fil=e7test?=");
                Assert.assertEquals(
                  "fil\u00e7test",
                  stringTemp);
            }
            {
                stringTemp =
                  ContentDisposition.MakeFilename("=?us-ascii*en?q?filetest?=");
                Assert.assertEquals(
                  "filetest",
                  stringTemp);
            }
            {
                stringTemp =
  ContentDisposition.MakeFilename("=?us-ascii*en-us?q?filetest?=");
                Assert.assertEquals(
                  "filetest",
                  stringTemp);
            }
            {
                stringTemp =
  ContentDisposition.MakeFilename("=?us-ascii*xx9x9x?q?filetest?=");
                Assert.assertEquals(
                  "filetest",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("d\ud800e");
                Assert.assertEquals(
                  "d\ufffde",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("d\udc00e");
                Assert.assertEquals(
                  "d\ufffde",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("d'e");
                Assert.assertEquals(
                  "d'e",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("utf-8'e");
                Assert.assertEquals(
                  "utf-8'e",
                  stringTemp);
            }
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
                stringTemp = ContentDisposition.MakeFilename("hello . txt");
                Assert.assertEquals("hello_._txt", stringTemp);
            }
            {
          stringTemp =
                  ContentDisposition.MakeFilename("hello\u2028world.txt");
                Assert.assertEquals("hello_world.txt", stringTemp);
            }
            {
          stringTemp =
                  ContentDisposition.MakeFilename("hello\u2029world.txt");
                Assert.assertEquals("hello_world.txt", stringTemp);
            }
            {
          stringTemp =
                  ContentDisposition.MakeFilename("hello\u0085world.txt");
                Assert.assertEquals("hello_world.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("prn.txt");
                Assert.assertEquals("_prn.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("aux.txt");
                Assert.assertEquals("_aux.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("con.txt");
                Assert.assertEquals("_con.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                  " =?utf-8?q?hello.txt?= ");
                Assert.assertEquals(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    " =?utf-8?q?___hello.txt___?= ");
                Assert.assertEquals(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    " =?utf-8*en?q?___hello.txt___?= ");
                Assert.assertEquals(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    " =?utf-8*?q?___hello.txt___?= ");
                Assert.assertEquals(
                  "___hello.txt___",
                  stringTemp);
            }
            {
                stringTemp =

          ContentDisposition.MakeFilename(
          " =?utf-8*i-unknown?q?___hello.txt___?= ");
                Assert.assertEquals(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
             ContentDisposition.MakeFilename(" =?*en?q?___hello.txt___?= ");
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
                stringTemp = ContentDisposition.MakeFilename(
                    "a\u00e7\u00e3o.txt");
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
                stringTemp = ContentDisposition.MakeFilename(
                    "my file\tname\".txt");
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
                stringTemp = ContentDisposition.MakeFilename(
                    "utf-8'en'file%c2%bename.txt");
                Assert.assertEquals(
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    "windows-1252'en'file%bename.txt");
                Assert.assertEquals(
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    "x-unknown'en'file%c2%bename.txt");
                Assert.assertEquals(
                  "x-unknown'en'file_c2_bename.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    "utf-8'en-us'file%c2%bename.txt");
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
                stringTemp = ContentDisposition.MakeFilename(
                    "myfilename.txt.");
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
                stringTemp = ContentDisposition.MakeFilename(" nul ");
                Assert.assertEquals(
                  "_nul",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(" ordinary ");
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
                stringTemp = ContentDisposition.MakeFilename(
                    "folder/hello.txt");
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
        }

        @Test
public void TestParse() {
            try {
                ContentDisposition.Parse(null);
                Assert.fail("Should have failed");
            } catch (NullPointerException ex) {
                // NOTE: Intentionally empty
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
