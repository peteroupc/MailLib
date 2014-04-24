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
      if (!File.Exists("..\\..\\..\\cache\\NormalizationTest.txt")) {
        Assert.Ignore();
      }
      bool[] handled = new bool[0x110000];
      using(StreamReader reader=new StreamReader("..\\..\\..\\cache\\NormalizationTest.txt")) {
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
          AssertEqual(expected, actual, line);
          ci = new IntArrayCharacterInput(cps);
          actual = ToIntArray(Normalizer.GetChars(ci, Normalization.NFD));
          AssertEqual(GetCodePoints(columns[2]), actual, line);
          ci = new IntArrayCharacterInput(cps);
          actual = ToIntArray(Normalizer.GetChars(ci, Normalization.NFKC));
          AssertEqual(GetCodePoints(columns[3]), actual, line);
          ci = new IntArrayCharacterInput(cps);
          actual = ToIntArray(Normalizer.GetChars(ci, Normalization.NFKD));
          AssertEqual(GetCodePoints(columns[4]), actual, line);
        }
      }
      int[] cptemp = new int[1];
      for (int i = 0;i<handled.Length; ++i) {
        if (i >= 0xd800 && i <= 0xdfff) {
          continue;
        }
        if (!handled[i]) {
          cptemp[0]=i;
          ICharacterInput ci = new IntArrayCharacterInput(cptemp);
          string imsg=""+i;
          AssertEqual(cptemp, ToIntArray(Normalizer.GetChars(ci, Normalization.NFC)), imsg);
          ci = new IntArrayCharacterInput(cptemp);
          AssertEqual(cptemp, ToIntArray(Normalizer.GetChars(ci, Normalization.NFD)), imsg);
          ci = new IntArrayCharacterInput(cptemp);
          AssertEqual(cptemp, ToIntArray(Normalizer.GetChars(ci, Normalization.NFKC)), imsg);
          ci = new IntArrayCharacterInput(cptemp);
          AssertEqual(cptemp, ToIntArray(Normalizer.GetChars(ci, Normalization.NFKD)), imsg);
        }
      }
    }
  }
}
