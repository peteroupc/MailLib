using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;
using PeterO.Text;

namespace MailLibTest {
  [TestFixture]
  public class NormalizationTest {
    private static string TrimSpaces(String s) {
      return s.Trim();
    }

    public static int[] GetCodePoints(String cp) {
      String[] cpArray=TrimSpaces(cp).Split(' ');
      int[] ret = new int[cpArray.Length];
      int index = 0;
      foreach(String v in cpArray) {
        int hex = Int32.Parse(TrimSpaces(v), System.Globalization.NumberStyles.AllowHexSpecifier,
                              System.Globalization.CultureInfo.InvariantCulture);
        ret[index++]=hex;
      }
      return ret;
    }

    public static String ToString(int[] array) {
      StringBuilder builder = new StringBuilder();
      bool first = true;
      builder.Append("[");
      foreach(int v in array) {
        if (!first) {
          builder.Append(", ");
        }
        builder.Append(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture));
        first = false;
      }
      builder.Append("]");
      return builder.ToString();
    }

    public static int[] ToIntArray(IList<int> list) {
      int[] ret = new int[list.Count];
      int index = 0;
      foreach(int v in list) {
        ret[index++]=v;
      }
      return ret;
    }
    public static void AssertEqual(int expected, int actual, String msg) {
      if (expected != actual) {
        Assert.AreEqual(expected, actual, msg);
      }
    }
    public static void AssertEqual(int[] expected, int[] actual, String msg) {
      if (expected.Length != actual.Length) {
        Assert.Fail(
          "\nexpected: "+ToString(expected)+"\n"+
          "\nwas:      "+ToString(actual)+"\n"+msg);
      }
      for (int i = 0;i<expected.Length; ++i) {
        if (expected[i]!=actual[i]) {
          Assert.Fail(
            "\nexpected: "+ToString(expected)+"\n"+
            "\nwas:      "+ToString(actual)+"\n"+msg);
        }
      }
    }

    [Test]
    public void NormTest() {
      string normTestFile="..\\..\\..\\cache\\NormalizationTest.txt";
      if (!File.Exists(normTestFile)) {
        Assert.Ignore();
      }
      bool[] handled = new bool[0x110000];
      using(StreamReader reader = new StreamReader(normTestFile)) {
        bool part1 = false;
        while (true) {
          String line = reader.ReadLine();
          if (line == null) {
            break;
          }
          line = TrimSpaces(line);
          int hash=line.IndexOf("#");
          if (hash >= 0) {
            line = TrimSpaces(line.Substring(0, hash));
          }
          if (line.StartsWith("@")) {
            if (line.Equals("@Part1")) {
              part1 = true;
            } else {
              part1 = false;
            }
            continue;
          }
          line = TrimSpaces(line);
          if (line.Length == 0) {
            continue;
          }
          String[] columns=line.Split(';');
          int[] cps = GetCodePoints(columns[0]);
          if (part1) {
            handled[cps[0]]=true;
          }
          int[] expected = GetCodePoints(columns[1]);
          int[] actual;
          ICharacterInput ci = new IntArrayCharacterInput(cps);
          actual = ToIntArray(Normalizer.GetChars(ci, Normalization.NFC));
          if (!Normalizer.IsNormalized(expected, Normalization.NFC)) {
 Assert.Fail(line);
}
          AssertEqual(expected, actual, line);
          ci = new IntArrayCharacterInput(cps);
          actual = ToIntArray(Normalizer.GetChars(ci, Normalization.NFD));
          AssertEqual(GetCodePoints(columns[2]), actual, line);
          if (!Normalizer.IsNormalized(GetCodePoints(columns[2]), Normalization.NFD)) {
 Assert.Fail(line);
}
          ci = new IntArrayCharacterInput(cps);
          actual = ToIntArray(Normalizer.GetChars(ci, Normalization.NFKC));
          AssertEqual(GetCodePoints(columns[3]), actual, line);
          if (!Normalizer.IsNormalized(GetCodePoints(columns[3]), Normalization.NFKC)) {
 Assert.Fail(line);
}
          ci = new IntArrayCharacterInput(cps);
          actual = ToIntArray(Normalizer.GetChars(ci, Normalization.NFKD));
          AssertEqual(GetCodePoints(columns[4]), actual, line);
          if (!Normalizer.IsNormalized(GetCodePoints(columns[4]), Normalization.NFKD)) {
 Assert.Fail(line);
}
        }
      }
      int[] cptemp = new int[1];
      // Individual code points that don't appear in Part 1 of the
      // test will normalize to themselves in all four normalization forms
      for (int i = 0;i<handled.Length; ++i) {
        if (i >= 0xd800 && i <= 0xdfff) {
          // skip surrogate code points
          continue;
        }
        if (!handled[i]) {
          cptemp[0]=i;
          string imsg=""+i;
          ICharacterInput ci = new IntArrayCharacterInput(cptemp);
          Normalizer norm;
          norm = new Normalizer(ci, Normalization.NFC);
          AssertEqual(i, norm.Read(), imsg);
          AssertEqual(-1, norm.Read(), imsg);
          ci = new IntArrayCharacterInput(cptemp);
          norm = new Normalizer(ci, Normalization.NFD);
          AssertEqual(i, norm.Read(), imsg);
          AssertEqual(-1, norm.Read(), imsg);
          ci = new IntArrayCharacterInput(cptemp);
          norm = new Normalizer(ci, Normalization.NFKC);
          AssertEqual(i, norm.Read(), imsg);
          AssertEqual(-1, norm.Read(), imsg);
          ci = new IntArrayCharacterInput(cptemp);
          norm = new Normalizer(ci, Normalization.NFKD);
          AssertEqual(i, norm.Read(), imsg);
          AssertEqual(-1, norm.Read(), imsg);
          if (!Normalizer.IsNormalized(cptemp, Normalization.NFC)) {
 Assert.Fail(imsg);
}
          if (!Normalizer.IsNormalized(cptemp, Normalization.NFD)) {
 Assert.Fail(imsg);
}
          if (!Normalizer.IsNormalized(cptemp, Normalization.NFKC)) {
 Assert.Fail(imsg);
}
          if (!Normalizer.IsNormalized(cptemp, Normalization.NFKD)) {
 Assert.Fail(imsg);
}
        }
      }
      // Additional normalization tests
      Assert.IsFalse(Normalizer.IsNormalized("x\u0300\u0323yz",Normalization.NFC));
      Assert.IsFalse(Normalizer.IsNormalized("x\u0300\u0323",Normalization.NFC));
    }
  }
}
