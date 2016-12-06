using NUnit.Framework;
using PeterO.Text;
using PeterO;
using System;
using Test;
using System.Collections.Generic;
using System.Text;

namespace MailLibTest {
  [TestFixture]
  public class NormalizationTest {
    public static string[] SplitAt(string str, string delimiter) {
      if (delimiter == null) {
        throw new ArgumentNullException("delimiter");
      }
      if (delimiter.Length == 0) {
        throw new ArgumentException("delimiter is empty.");
      }
      if (String.IsNullOrEmpty(str)) {
        return new[] { String.Empty };
      }
      var index = 0;
      var first = true;
      List<string> strings = null;
      int delimLength = delimiter.Length;
      while (true) {
        int index2 = str.IndexOf(delimiter, index, StringComparison.Ordinal);
        if (index2 < 0) {
          if (first) {
            var strret = new string[1];
            strret[0] = str;
            return strret;
          }
          strings = strings ?? (new List<string>());
          strings.Add(str.Substring(index));
          break;
        } else {
          first = false;
          string newstr = str.Substring(index, (index2) - index);
          strings = strings ?? (new List<string>());
          strings.Add(newstr);
          index = index2 + delimLength;
        }
      }
      return (string[])strings.ToArray();
    }

    public static int[] GetCodePoints(string cp) {
      var index = 0;
      var state = 0;
      var codePoint = 0;
      var retArray = new int[cp.Length];
      var count = 0;
      while (index <= cp.Length) {
        int c = (index >= cp.Length) ? -1 : (int)cp[index];
        ++index;
        switch (state) {
          case 0:
            if (c >= 0x41 && c <= 0x46) {
              codePoint = c - 0x41 + 10;
              state = 1;
            } else if (c >= 0x61 && c <= 0x66) {
              codePoint = c - 0x61 + 10;
              state = 1;
            } else if (c >= 0x30 && c <= 0x39) {
              codePoint = c - 0x30;
              state = 1;
            }
            break;
          case 1:
            if (c >= 0x41 && c <= 0x46) {
              codePoint <<= 4;
              codePoint |= c - 0x41 + 10;
            } else if (c >= 0x61 && c <= 0x61) {
              codePoint <<= 4;
              codePoint |= c - 0x61 + 10;
            } else if (c >= 0x30 && c <= 0x39) {
              codePoint <<= 4;
              codePoint |= c - 0x30;
            } else {
              if (count == retArray.Length) {
                var newArray = new int[retArray.Length * 2];
                Array.Copy(retArray, 0, newArray, 0, count);
                retArray = newArray;
              }
              retArray[count++] = codePoint;
              state = 0;
              codePoint = 0;
            }
            break;
        }
      }
      var newArray2 = new int[count];
      Array.Copy(retArray, 0, newArray2, 0, count);
      return newArray2;
    }

    public static string ToString(int[] array) {
      var builder = new StringBuilder();
      var first = true;
      builder.Append("[");
      foreach (int v in array) {
        if (!first) {
          builder.Append(", ");
        }
        builder.Append(TestCommon.IntToString(v));
        first = false;
      }
      builder.Append("]");
      return builder.ToString();
    }

    public static string ToCodePointString(int[] array) {
      var builder = new StringBuilder();
      foreach (int v in array) {
        if (v <= 0xffff) {
          builder.Append((char)v);
        } else if (v <= 0x10ffff) {
          builder.Append((char)((((v - 0x10000) >> 10) & 0x3ff) + 0xd800));
          builder.Append((char)(((v - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
      return builder.ToString();
    }

    public static void AssertEqual(
  string expectedStr,
  string actualStr,
  string msg) {
      if (!expectedStr.Equals(actualStr)) {
        Assert.AreEqual(expectedStr, actualStr, msg);
      }
    }

    public static void AssertEqual(int expected, int actual, string msg) {
      if (expected != actual) {
        Assert.AreEqual(expected, actual, msg);
      }
    }

    public static void AssertEqual(int[] expected, int[] actual, string msg) {
      if (expected.Length != actual.Length) {
        Assert.Fail(
          "\nexpected: " + ToString(expected) + "\n" + "\nwas: " +
            ToString(actual) + "\n" + msg);
      }
      for (int i = 0; i < expected.Length; ++i) {
        if (expected[i] != actual[i]) {
          Assert.Fail("\nexpected: " + ToString(expected) + "\n" +
                "\nwas: " + ToString(actual) + "\n" + msg);
        }
      }
    }

    private void TestIdempotent(string str, Normalization norm) {
      bool isForm = NormalizerInput.IsNormalized(str, norm);
      string newStr = NormalizerInput.Normalize(str, norm);
      ICharacterInput sci = new CharacterReader(str, false, true);
            bool isForm2 = NormalizerInput.IsNormalized(sci, norm);
      if (isForm) {
        AssertEqual(str, newStr, EncodingTest.EscapeString(str));
      }
      Assert.AreEqual(isForm, isForm2);
      if (!NormalizerInput.IsNormalized(newStr, norm)) {
        Assert.Fail(EncodingTest.EscapeString(str));
      }
      if (!isForm) {
        string newStr2 = NormalizerInput.Normalize(newStr, norm);
        AssertEqual(newStr, newStr2, EncodingTest.EscapeString(str));
      }
    }

    [Test]
    public void NormTestRandom() {
      var rand = new RandomGenerator(new XorShift128Plus(false));
      for (var i = 0; i < 100000; ++i) {
        string str = EncodingTest.RandomString(rand);
                this.TestIdempotent(str, Normalization.NFC);
                this.TestIdempotent(str, Normalization.NFD);
                this.TestIdempotent(str, Normalization.NFKC);
                this.TestIdempotent(str, Normalization.NFKD);
}
    }

    [Test]
    public void NormTestSpecific() {
      string str = "_\ufac7\uc972+67 Tqd R_.";
            {
string stringTemp = NormalizerInput.Normalize(
  str,
  Normalization.NFC);
Assert.AreEqual(
  "_\u96e3\uc972+67 Tqd R_.",
  stringTemp);
}
      Assert.IsFalse(
        NormalizerInput.IsNormalized(str, Normalization.NFC));
      this.TestIdempotent(str, Normalization.NFC);
            str = "_\u96e3\uc972+67 Tqd R_._";
            Assert.IsTrue(
        NormalizerInput.IsNormalized(str, Normalization.NFC));
            this.TestIdempotent(str, Normalization.NFC);
    }

    private sealed class NormResult {
      private readonly int[] orig;
      private readonly string origstr;
      private readonly string nfc;
      private readonly string nfd;
      private readonly string nfkc;
      private readonly string nfkd;
      private readonly string line;

      public NormResult(string column, string line) {
        this.line = line;
        this.orig = GetCodePoints(column);
        this.origstr = ToCodePointString(this.orig);
        this.nfc = NormalizerInput.Normalize(
  this.origstr,
  Normalization.NFC);
        this.nfd = NormalizerInput.Normalize(
  this.origstr,
  Normalization.NFD);
        this.nfkc = NormalizerInput.Normalize(
  this.origstr,
  Normalization.NFKC);
        this.nfkd = NormalizerInput.Normalize(
  this.origstr,
  Normalization.NFKD);
        if (!NormalizerInput.IsNormalized(
            this.nfc,
            Normalization.NFC)) {
          {
            Assert.Fail(line);
          }
        }
        if (!NormalizerInput.IsNormalized(
            this.nfd,
            Normalization.NFD)) {
          {
            Assert.Fail(line);
          }
        }
        if (!NormalizerInput.IsNormalized(
            this.nfkc,
            Normalization.NFKC)) {
          {
            Assert.Fail(line);
          }
        }
        if (!NormalizerInput.IsNormalized(
            this.nfkd,
            Normalization.NFKD)) {
          {
            Assert.Fail(line);
          }
        }
      }

      public void AssertNFC(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.origstr, o.nfc, this.line);
      }

      public void AssertNFD(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.origstr, o.nfd, this.line);
      }

      public void AssertNFKC(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.origstr, o.nfkc, this.line);
      }

      public void AssertNFKD(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.origstr, o.nfkd, this.line);
      }
    }

    [Test]
    [Timeout(60000)]

    public void NormTest() {
      var handled = new bool[0x110000];
      string[] lines = NetHelper.DownloadOrOpenAllLines(
        "http://www.unicode.org/Public/UNIDATA/NormalizationTest.txt",
        "NormalizationTest.txt");
      if (lines == null) {
 Assert.Fail();
 }
      Assert.IsTrue(lines.Length > 0);
      var part1 = false;
       foreach (string lineItem in lines) {
        string line = lineItem;
        int hash = line.IndexOf("#", StringComparison.Ordinal);
        if (hash >= 0) {
          line = line.Substring(0, hash);
        }
        if (line.StartsWith("@", StringComparison.Ordinal)) {
          part1 = line.IndexOf("@Part1", StringComparison.Ordinal) == 0;
          continue;
        }
        if (line.Length == 0) {
          continue;
        }
        string[] columns = SplitAt(line, ";");
        int[] cps = GetCodePoints(columns[0]);
        if (part1) {
          handled[cps[0]] = true;
        }
        var nr = new NormResult[5];
        for (var i = 0; i < 5; ++i) {
          nr[i] = new NormResult(columns[i], line);
        }
        nr[1].AssertNFC(nr[0], nr[1], nr[2]);
        nr[3].AssertNFC(nr[3], nr[4]);
        nr[2].AssertNFD(nr[0], nr[1], nr[2]);
        nr[4].AssertNFD(nr[3], nr[4]);
        nr[4].AssertNFKD(nr[0], nr[1], nr[2]);
        nr[4].AssertNFKD(nr[3], nr[4]);
        nr[3].AssertNFKC(nr[0], nr[1], nr[2]);
        nr[3].AssertNFKC(nr[3], nr[4]);
      }
      var cptemp = new char[2];
      // Individual code points that don't appear in Part 1 of the
      // test will normalize to themselves in all four normalization forms
      for (int i = 0; i < handled.Length; ++i) {
        if ((i & 0xfff800) == 0xd800) {
          // skip surrogate code points
          continue;
        }
        if (!handled[i]) {
          if (i >= 0x10000) {
            cptemp[0] = ((char)((((i - 0x10000) >> 10) & 0x3ff) + 0xd800));
            cptemp[1] = ((char)(((i - 0x10000) & 0x3ff) + 0xdc00));
          } else {
            cptemp[0] = (char)i;
          }
          string cpstr = new String(cptemp, 0, i >= 0x10000 ? 2 : 1);
          if (!NormalizerInput.IsNormalized(
  cpstr,
  Normalization.NFC)) {
            Assert.Fail(TestCommon.IntToString(i));
          }
          if (!NormalizerInput.IsNormalized(
  cpstr,
  Normalization.NFD)) {
                    Assert.Fail(TestCommon.IntToString(i));
          }
          if (!NormalizerInput.IsNormalized(
  cpstr,
  Normalization.NFKC)) {
                    Assert.Fail(TestCommon.IntToString(i));
 }
          if (!NormalizerInput.IsNormalized(
  cpstr,
  Normalization.NFKD)) {
                    Assert.Fail(TestCommon.IntToString(i));
        }
          string imsg = TestCommon.IntToString(i);
          AssertEqual(
  cpstr,
  NormalizerInput.Normalize(cpstr, Normalization.NFC),
  imsg);
          AssertEqual(
  cpstr,
  NormalizerInput.Normalize(cpstr, Normalization.NFD),
  imsg);
          AssertEqual(
  cpstr,
  NormalizerInput.Normalize(cpstr, Normalization.NFKC),
  imsg);
          AssertEqual(
  cpstr,
  NormalizerInput.Normalize(cpstr, Normalization.NFKD),
  imsg);
        }
      }
    }
  }
}
