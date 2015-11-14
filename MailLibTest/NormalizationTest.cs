using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
      int index = 0;
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
      bool first = true;
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

    public static int[] NormalizerGetChars(int[] cp, Normalization form) {
 string ret = NormalizingCharacterInput.Normalize(ToCodePointString(cp), form);
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
      int index = 0;
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
          "\nexpected: " + ToString(expected) + "\n" + "\nwas:      "+
            ToString(actual) + "\n" + msg);
      }
      for (int i = 0; i < expected.Length; ++i) {
        if (expected[i] != actual[i]) {
        Assert.Fail("\nexpected: " + ToString(expected) + "\n" + "\nwas:      "+
              ToString(actual) + "\n" + msg);
        }
      }
    }

    [TestMethod]
    public void TestIsNormalizedAlmostSurrogates() {
          Assert.IsTrue(
    NormalizingCharacterInput.IsNormalized(
     new int[] { 0x1d800, 0x1d900, 0x1da00, 0x1db00, 0x1dc00, 0x1df00
       }, Normalization.NFC));
    Assert.IsTrue(
    NormalizingCharacterInput.IsNormalized(
     new int[] { 0x1d800, 0x1d900, 0x1da00, 0x1db00, 0x1dc00, 0x1df00
       }, Normalization.NFD));
    Assert.IsTrue(
    NormalizingCharacterInput.IsNormalized(
     new int[] { 0x1d800, 0x1d900, 0x1da00, 0x1db00, 0x1dc00, 0x1df00
       }, Normalization.NFKC));
    Assert.IsTrue(
    NormalizingCharacterInput.IsNormalized(
     new int[] { 0x1d800, 0x1d900, 0x1da00, 0x1db00, 0x1dc00, 0x1df00
       }, Normalization.NFKD));
    }

    [TestMethod]
    public void NormTest() {
      string normTestFile = "..\\..\\..\\cache\\NormalizationTest.txt";
      if (!File.Exists(normTestFile)) {
        Assert.Inconclusive();
      }
      var handled = new bool[0x110000];
      using (var reader = new StreamReader(normTestFile)) {
        bool part1 = false;
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
          int[] expected = GetCodePoints(columns[1]);
          int[] actual;
          int[] ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFC);
          if (!NormalizingCharacterInput.IsNormalized(
            expected,
            Normalization.NFC)) {
            Assert.Fail(line);
          }
          AssertEqual(expected, actual, line);
          ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFD);
          AssertEqual(GetCodePoints(columns[2]), actual, line);
          if (!NormalizingCharacterInput.IsNormalized(
  GetCodePoints(columns[2]),
  Normalization.NFD)) {
            Assert.Fail(line);
          }
          ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFKC);
          AssertEqual(GetCodePoints(columns[3]), actual, line);
          if (!NormalizingCharacterInput.IsNormalized(
  GetCodePoints(columns[3]),
  Normalization.NFKC)) {
            Assert.Fail(line);
          }
          ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFKD);
          AssertEqual(GetCodePoints(columns[4]), actual, line);
          if (!NormalizingCharacterInput.IsNormalized(
  GetCodePoints(columns[4]),
  Normalization.NFKD)) {
            Assert.Fail(line);
          }
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
          Assert.AreEqual(cpstr, NormalizingCharacterInput.Normalize(cpstr,
              Normalization.NFC));
          Assert.AreEqual(cpstr, NormalizingCharacterInput.Normalize(cpstr,
              Normalization.NFD));
          Assert.AreEqual(cpstr, NormalizingCharacterInput.Normalize(cpstr,
               Normalization.NFKC));
          Assert.AreEqual(cpstr, NormalizingCharacterInput.Normalize(cpstr,
               Normalization.NFKD));
       if (!NormalizingCharacterInput.IsNormalized(cpstr, Normalization.NFC)) {
            Assert.Fail(imsg);
          }
       if (!NormalizingCharacterInput.IsNormalized(cpstr, Normalization.NFD)) {
            Assert.Fail(imsg);
          }
      if (!NormalizingCharacterInput.IsNormalized(cpstr, Normalization.NFKC)) {
            Assert.Fail(imsg);
          }
      if (!NormalizingCharacterInput.IsNormalized(cpstr, Normalization.NFKD)) {
            Assert.Fail(imsg);
          }
        }
      }
      // Additional normalization tests
      Assert.IsFalse(NormalizingCharacterInput.IsNormalized(
        "x\u0300\u0323yz",
        Normalization.NFC));
      Assert.IsFalse(NormalizingCharacterInput.IsNormalized(
        "x\u0300\u0323",
        Normalization.NFC));
    }
  }
}
