using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using PeterO.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeterO.Text;

namespace MailLibTest {
  [TestClass]
  public class NormalizationTest {
    private static string TrimSpaces(String s) {
      return s.Trim();
    }

    public static int[] GetCodePoints(String cp) {
      String[] cpArray = TrimSpaces(cp).Split(' ');
      var retArray = new int[cpArray.Length];
      var index = 0;
      foreach (String v in cpArray) {
        int hex = Int32.Parse(TrimSpaces(v),
          System.Globalization.NumberStyles.AllowHexSpecifier,
  System.Globalization.CultureInfo.InvariantCulture);
        retArray[index++] = hex;
      }
      return retArray;
    }

    public static String ToString(int[] array) {
      var builder = new StringBuilder();
      var first = true;
      builder.Append("[");
      foreach (int v in array) {
        if (!first) {
          builder.Append(", ");
        }
        builder.Append(Convert.ToString(v,
          System.Globalization.CultureInfo.InvariantCulture));
        first = false;
      }
      builder.Append("]");
      return builder.ToString();
    }

    public static String ToCodePointString(int[] array) {
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

    public static int[] NormalizerGetChars(string cp, Normalization form) {
      string ret = NormalizingCharacterInput.Normalize(cp, form);
      IList<int> list = new List<int>();
      for (int i = 0; i < ret.Length; ++i) {
        int c = PeterO.DataUtilities.CodePointAt(ret, i);
        if (c >= 0x10000) {
          ++i;
        }
        list.Add(c);
      }
      return ToIntArray(list);
    }

    public static int[] ToIntArray(IList<int> list) {
      var retArray = new int[list.Count];
      var index = 0;
      foreach (int v in list) {
        retArray[index++] = v;
      }
      return retArray;
    }
    public static void AssertEqual(int expected, int actual, String msg) {
      if (expected != actual) {
        Assert.AreEqual(expected, actual, msg);
      }
    }
    public static void AssertEqual(int[] expected, int[] actual, String msg) {
      if (expected.Length != actual.Length) {
        Assert.Fail(
          "\nexpected: " + ToString(expected) + "\n" + "\nwas:      " +
            ToString(actual) + "\n" + msg);
      }
      for (int i = 0; i < expected.Length; ++i) {
        if (expected[i] != actual[i]) {
      Assert.Fail("\nexpected: " + ToString(expected) + "\n" +
            "\nwas:      " + ToString(actual) + "\n" + msg);
        }
      }
    }

    private sealed class NormResult {
      private readonly int[] orig;
      private readonly string origstr;
      private readonly int[] nfc;
      private readonly int[] nfd;
      private readonly int[] nfkc;
      private readonly int[] nfkd;
      private readonly string line;
      public NormResult(string column, string line) {
        this.line = line;
        this.orig = GetCodePoints(column);
        this.origstr = ToCodePointString(this.orig);
        this.nfc = NormalizerGetChars(this.origstr, Normalization.NFC);
        this.nfd = NormalizerGetChars(this.origstr, Normalization.NFD);
        this.nfkc = NormalizerGetChars(this.origstr, Normalization.NFKC);
        this.nfkd = NormalizerGetChars(this.origstr, Normalization.NFKD);
        if (!NormalizingCharacterInput.IsNormalized(
            this.nfc,
            Normalization.NFC)) {
  { Assert.Fail(line);
} }
        if (!NormalizingCharacterInput.IsNormalized(
            this.nfd,
            Normalization.NFD)) {
  { Assert.Fail(line);
} }
        if (!NormalizingCharacterInput.IsNormalized(
            this.nfkc,
            Normalization.NFKC)) {
  { Assert.Fail(line);
} }
        if (!NormalizingCharacterInput.IsNormalized(
            this.nfkd,
            Normalization.NFKD)) {
  { Assert.Fail(line);
} }
      }
      public void AssertNFC(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.orig, o.nfc, this.line);
      }
      public void AssertNFD(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.orig, o.nfd, this.line);
      }
      public void AssertNFKC(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.orig, o.nfkc, this.line);
      }
      public void AssertNFKD(params NormResult[] other) {
        foreach (NormResult o in other)
          AssertEqual(this.orig, o.nfkd, this.line);
      }
    }

    [TestMethod]
    public void NormTest() {
      const string normTestFile = "..\\..\\..\\cache\\NormalizationTest.txt";
      if (!File.Exists(normTestFile)) {
        Assert.Inconclusive();
      }
      var handled = new bool[0x110000];
      using (var reader = new StreamReader(normTestFile)) {
        var part1 = false;
        while (true) {
          String line = reader.ReadLine();
          if (line == null) {
            break;
          }
          line = TrimSpaces(line);
          int hash = line.IndexOf("#", StringComparison.Ordinal);
          if (hash >= 0) {
            line = TrimSpaces(line.Substring(0, hash));
          }
          if (line.StartsWith("@", StringComparison.Ordinal)) {
            part1 = (line.Equals("@Part1"));
            continue;
          }
          line = TrimSpaces(line);
          if (line.Length == 0) {
            continue;
          }
          String[] columns = line.Split(';');
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
      }
      var cptemp = new char[2];
      // Individual code points that don't appear in Part 1 of the
      // test will normalize to themselves in all four normalization forms
      for (int i = 0; i < handled.Length; ++i) {
        if ((i & 0xf800) == 0xd800) {
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
          string cpstr = new String(cptemp, 0, (i >= 0x10000 ? 2 : 1));
          string imsg = "" + i;
          if (!cpstr.Equals(NormalizingCharacterInput.Normalize(
            cpstr,
            Normalization.NFC))) {
 Assert.Fail(imsg);
}
          if (!cpstr.Equals(NormalizingCharacterInput.Normalize(cpstr,
              Normalization.NFD))) {
 Assert.Fail(imsg);
}
          if (!cpstr.Equals(NormalizingCharacterInput.Normalize(cpstr,
               Normalization.NFKC))) {
 Assert.Fail(imsg);
}
          if (!cpstr.Equals(NormalizingCharacterInput.Normalize(cpstr,
               Normalization.NFKD))) {
 Assert.Fail(imsg);
}
       if (!NormalizingCharacterInput.IsNormalized(cpstr,
            Normalization.NFC)) {
            Assert.Fail(imsg);
          }
       if (!NormalizingCharacterInput.IsNormalized(cpstr,
            Normalization.NFD)) {
            Assert.Fail(imsg);
          }
      if (!NormalizingCharacterInput.IsNormalized(cpstr,
            Normalization.NFKC)) {
            Assert.Fail(imsg);
          }
      if (!NormalizingCharacterInput.IsNormalized(cpstr,
            Normalization.NFKD)) {
            Assert.Fail(imsg);
          }
        }
      }
    }
  }
}
