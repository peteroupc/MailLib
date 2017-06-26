using System;
using System.Text;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;
using PeterO.Text;

namespace MailLibTest {
    [TestFixture]
    public partial class ContentDispositionTest {
        [Test] public void TestDispositionType() {
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
            ContentDisposition cd = ContentDisposition.Parse("inline");
            Assert.IsFalse(cd.IsAttachment);
            cd = ContentDisposition.Parse("cd-unknown");
            Assert.IsFalse(cd.IsAttachment);
            cd = ContentDisposition.Parse("attachment");
            Assert.IsTrue(cd.IsAttachment);
        }

        [Test] public void TestIsInline() {
            ContentDisposition cd = ContentDisposition.Parse("inline");
            Assert.IsTrue(cd.IsInline);
            cd = ContentDisposition.Parse("cd-unknown");
            Assert.IsFalse(cd.IsInline);
            cd = ContentDisposition.Parse("attachment");
            Assert.IsFalse(cd.IsInline);
        }

        private static string MakeQEncoding(string str) {
            byte[] bytes = DataUtilities.GetUtf8Bytes(str, false);
            var sb = new StringBuilder();
            string hex = "0123456789ABCDEF";
            sb.Append("=?utf-8?q?");
            for (var i = 0; i < bytes.Length; ++i) {
                int b = ((int)bytes[i]) & 0xff;
                if (b == 0x32) {
                    sb.Append('_');
                } else if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
                  (b >= '0' && b <= '9')) {
                    sb.Append((char)b);
                } else {
                    sb.Append('=');
                    sb.Append(hex[(b >> 4) & 15]);
                    sb.Append(hex[b & 15]);
                }
            }
            sb.Append("?=");
            return sb.ToString();
        }

        private static string MakeRfc2231Encoding(string str) {
            byte[] bytes = DataUtilities.GetUtf8Bytes(str, false);
            var sb = new StringBuilder();
            string hex = "0123456789ABCDEF";
            sb.Append("utf-8''");
            for (var i = 0; i < bytes.Length; ++i) {
                int b = ((int)bytes[i]) & 0xff;
                if ((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') ||
                  (b >= '0' && b <= '9')) {
                    sb.Append((char)b);
                } else {
                    sb.Append('%');
                    sb.Append(hex[(b >> 4) & 15]);
                    sb.Append(hex[b & 15]);
                }
            }
            return sb.ToString();
        }

        private static string RandomString(RandomGenerator rnd) {
            string ret = EncodingTest.RandomString(rnd);
            int ui = rnd.UniformInt(100);
            if (ui < 20) {
                ret = MakeQEncoding(ret);
            } else if (ui < 25) {
                ret = MakeRfc2231Encoding(ret);
            }
            return ret;
        }

        internal static bool IsGoodFilename(string str) {
            if (str == null || str.Length == 0 || str.Length > 255) {
                return false;
            }
            if (str[str.Length - 1] == '.' || str[str.Length - 1] == '~') {
                return false;
            }
            string strLower = DataUtilities.ToLowerCaseAscii(str);
            bool bracketDigit = str[0] == '{' && str.Length > 1 &&
                    str[1] >= '0' && str[1] <= '9';
          bool homeFolder = str[0] == '~' || str[0] == '-' || str[0] ==
              '$';
            bool period = str[0] == '.';
          bool beginEndSpace = str[0] == 0x20 || str[str.Length - 1] ==
              0x20;
            if (bracketDigit || homeFolder || period || beginEndSpace) {
                return false;
            }
            // Reserved filenames on Windows
            bool reservedFilename = strLower.Equals(
        "nul") || strLower.Equals("clock$") || strLower.IndexOf(
        "nul.",
        StringComparison.Ordinal) == 0 || strLower.Equals(
        "prn") || strLower.IndexOf(
        "prn.",
        StringComparison.Ordinal) == 0 || strLower.IndexOf(
        "![",
        StringComparison.Ordinal) >= 0 || strLower.Equals(
        "aux") || strLower.IndexOf(
        "aux.",
        StringComparison.Ordinal) == 0 || strLower.Equals(
        "con") || strLower.IndexOf(
        "con.",
        StringComparison.Ordinal) == 0 || (
        strLower.Length >= 4 && strLower.IndexOf(
        "lpt",
        StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
             strLower[3] <= '9') || (strLower.Length >= 4 &&
                    strLower.IndexOf(
        "com",
        StringComparison.Ordinal) == 0 && strLower[3] >= '0' &&
                  strLower[3] <= '9');
            if (reservedFilename) {
                return false;
            }
            int i;
            for (i = 0; i < str.Length; ++i) {
                char c = str[i];
                if (c < 0x20 || (c >= 0x7f && c <= 0x9f) ||
                  c == '%' || c == 0x2028 || c == 0x2029 ||
                c == '\\' || c == '/' || c == '*' || c == '?' || c == '|' ||
                  c == ':' || c == '<' || c == '>' || c == '"' || c == '`' ||
    c == '$' || c == 0xa0 || c == 0x3000 || c == 0x180e || c == 0x1680 ||
  (c >= 0x2000 && c <= 0x200b) || c == 0x205f || c == 0x202f || c == 0xfeff ||
                    (c & 0xfffe) == 0xfffe || (c >= 0xfdd0 && c <= 0xfdef)) {
                    return false;
                }
            }
            // Avoid space before and after last dot
            for (i = str.Length - 1; i >= 0; --i) {
                if (str[i] == '.') {
                 bool spaceAfter = i + 1 < str.Length && str[i + 1] == 0x20;
                    bool spaceBefore = i > 0 && str[i - 1] == 0x20;
                    if (spaceAfter || spaceBefore) {
                    return false;
                    }
                    break;
                }
            }
             return NormalizerInput.IsNormalized(str, Normalization.NFC);
        }

        [Test] public void TestMakeFilename() {
            string stringTemp;
      var rnd = new RandomGenerator(new XorShift128Plus(false));
        {
object objectTemp = String.Empty;
object objectTemp2 = ContentDisposition.MakeFilename(
              null);
Assert.AreEqual(objectTemp, objectTemp2);
}
            {
                stringTemp = ContentDisposition.MakeFilename(String.Empty);
                Assert.AreEqual(
                  "_",
                  stringTemp);
            }
            string mfn = ContentDisposition.MakeFilename(
              "utf-8''%2A%EF%AB%87%EC%A5%B2%2B67%20Tqd%20R%E3%80%80%2E");
            Assert.IsTrue(IsGoodFilename(mfn), mfn);
            for (var i = 0; i < 1000000; ++i) {
        if (i % 1000 == 0) {
          // Console.WriteLine (i);
        }
                string str = RandomString(rnd);
                string filename = ContentDisposition.MakeFilename(str);
                if (!IsGoodFilename(filename)) {
            Assert.Fail("str_____=" + EncodingTest.EscapeString(str) +
                    "\n" +
                    "filename=" + EncodingTest.EscapeString(filename) + "\n" +
  "Assert.IsTrue(IsGoodFilename(ContentDisposition.MakeFilename(\n" +
                    " \"" + EncodingTest.EscapeString(str) + "\")));");
                }
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello. txt");
                Assert.AreEqual(
                  "hello._txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello .txt");
                Assert.AreEqual(
                  "hello_.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello . txt");
                Assert.AreEqual(
                  "hello_._txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("hello._");
                Assert.AreEqual(
                  "hello._",
                  stringTemp);
            }
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
         stringTemp = ContentDisposition.MakeFilename(
                  "utf-8'en'hello%2Etxt");
                Assert.AreEqual(
                  "hello.txt",
                  stringTemp);
            }
            {
        stringTemp = ContentDisposition.MakeFilename(
                  "=?utf-8?q?hello.txt?=");
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
        ContentDisposition.MakeFilename(" " + "hello.txt");
            Assert.AreEqual(
              "hello.txt",
              stringTemp);
            stringTemp =
        ContentDisposition.MakeFilename("hello" + " " + "txt");
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
                stringTemp = ContentDisposition.MakeFilename("prn.txt");
                Assert.AreEqual("_prn.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("aux.txt");
                Assert.AreEqual("_aux.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename("con.txt");
                Assert.AreEqual("_con.txt", stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                  " =?utf-8?q?hello.txt?= ");
                Assert.AreEqual(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    " =?utf-8?q?___hello.txt___?= ");
                Assert.AreEqual(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    " =?utf-8*en?q?___hello.txt___?= ");
                Assert.AreEqual(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    " =?utf-8*?q?___hello.txt___?= ");
                Assert.AreEqual(
                  "___hello.txt___",
                  stringTemp);
            }
            {
                stringTemp =

          ContentDisposition.MakeFilename(
          " =?utf-8*i-unknown?q?___hello.txt___?= ");
                Assert.AreEqual(
                  "hello.txt",
                  stringTemp);
            }
            {
                stringTemp =
             ContentDisposition.MakeFilename(" =?*en?q?___hello.txt___?= ");
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
           stringTemp = ContentDisposition.MakeFilename(
                  "a\u00e7\u00e3o.txt");
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
          stringTemp = ContentDisposition.MakeFilename(
                  "my file\tname\".txt");
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
                stringTemp = ContentDisposition.MakeFilename(
                    "utf-8'en'file%c2%bename.txt");
                Assert.AreEqual(
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    "windows-1252'en'file%bename.txt");
                Assert.AreEqual(
                  "file\u00bename.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    "x-unknown'en'file%c2%bename.txt");
                Assert.AreEqual(
                  "x-unknown'en'file_c2_bename.txt",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(
                    "utf-8'en-us'file%c2%bename.txt");
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
              stringTemp = ContentDisposition.MakeFilename(
                  "myfilename.txt.");
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
                stringTemp = ContentDisposition.MakeFilename(" nul ");
                Assert.AreEqual(
                  "_nul",
                  stringTemp);
            }
            {
                stringTemp = ContentDisposition.MakeFilename(" ordinary ");
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
             stringTemp = ContentDisposition.MakeFilename(
                  "folder/hello.txt");
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
Assert.IsTrue(IsGoodFilename(ContentDisposition.MakeFilename(
  "\u216a2s\u1e19C<snhs\ud87a\ude8dX(\ufdef\ufdd0,u.y\u001c.|}Y \u2f18Yx\u2a11N%(..s3^(N\u0084`(r|41X_.})\ud84c\udef3\ufe3c/\\/ sq?G![{\ufeffZ\"qSMdgv3#dg\tK@^X;`jl\ud892\udcd3' e@5a(\u00a0 wg0g hH?5\u202flh\u04c1 \uffff(,\u044d qQ7b:uFs9m\u0b6b\\AT|HDAsH6's!_B>rb(q?KpUv;fa r!\u1dc2.5.U\\Ez\u1f5a/J.8`?U\u01ba\\/v\ufdef_p.%|}.;.(OL9\u00001O.RV\u2433z,E\u008f%o\u008f.fpDN=G {(\udac5\udd76XC\uffff..z\ud9e4\udc62^(u=|'93\u0f6bWvz\u0f09\u26d2$?y\ud9c5\udcd4P:)+iO\u009f[f?>JTo,Ge`:'I\u5ccf\u009f\u9c3a<+yC {\ub10bm(j\u7959.tL=\ud86a\udea3\\(i \u001fG0 +np\u180erFt.hoy ny)\".6 +j "
)));
    }

        [Test] public void TestParameters() {
        }

        [Test] public void TestParse() {
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

        [Test] public void TestToString() {
            // not implemented yet
        }
    }
}
