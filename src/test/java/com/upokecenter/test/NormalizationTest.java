package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.text.*;

  public class NormalizationTest {
    public static String[] SplitAt(String str, String delimiter) {
      if (delimiter == null) {
        throw new NullPointerException("delimiter");
      }
      if (delimiter.length() == 0) {
        throw new IllegalArgumentException("delimiter is empty.");
      }
      if (((str) == null || (str).length() == 0)) {
        return new String[] { "" };
      }
      int index = 0;
      boolean first = true;
      ArrayList<String> strings = null;
      int delimLength = delimiter.length();
      while (true) {
        int index2 = str.indexOf(delimiter, index);
        if (index2 < 0) {
          if (first) {
            String[] strret = new String[1];
            strret[0] = str;
            return strret;
          }
          strings = (strings == null) ? (new ArrayList<String>()) : strings;
          strings.add(str.substring(index));
          break;
        } else {
          first = false;
          String newstr = str.substring(index, (index)+(index2 - index));
          strings = (strings == null) ? (new ArrayList<String>()) : strings;
          strings.add(newstr);
          index = index2 + delimLength;
        }
      }
      return strings.toArray(new String[] { });
    }

    public static int[] GetCodePoints(String cp) {
      int index = 0;
      int state = 0;
      int codePoint = 0;
      int[] retArray = new int[cp.length()];
      int count = 0;
      while (index <= cp.length()) {
        int c = (index >= cp.length()) ? -1 : (int)cp.charAt(index);
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
              if (count == retArray.length) {
                int[] newArray = new int[retArray.length * 2];
                System.arraycopy(retArray, 0, newArray, 0, count);
                retArray = newArray;
              }
              retArray[count++] = codePoint;
              state = 0;
              codePoint = 0;
            }
            break;
        }
      }
      int[] newArray2 = new int[count];
      System.arraycopy(retArray, 0, newArray2, 0, count);
      return newArray2;
    }

    public static String toString(int[] array) {
      StringBuilder builder = new StringBuilder();
      boolean first = true;
      builder.append("[");
      for (int v : array) {
        if (!first) {
          builder.append(", ");
        }
        builder.append(TestCommon.IntToString(v));
        first = false;
      }
      builder.append("]");
      return builder.toString();
    }

    public static String ToCodePointString(int[] array) {
      StringBuilder builder = new StringBuilder();
      for (int v : array) {
        if (v <= 0xffff) {
          builder.append((char)v);
        } else if (v <= 0x10ffff) {
          builder.append((char)((((v - 0x10000) >> 10) & 0x3ff) | 0xd800));
          builder.append((char)(((v - 0x10000) & 0x3ff) | 0xdc00));
        }
      }
      return builder.toString();
    }

    public static void AssertEqual(
      String expectedStr,
      String actualStr,
      String msg) {
      if (!expectedStr.equals(actualStr, StringComparison.Ordinal)) {
                Assert.fail(
  "\nexpected: " + EncodingTest.EscapeString(expectedStr) + "\n" +
            "\nwas: " + EncodingTest.EscapeString(actualStr) + "\n" + msg);
      }
    }

    public static void AssertEqual(int expected, int actual, String msg) {
      if (expected != actual) {
                Assert.fail(
          "\nexpected: " + expected + "\n" + "\nwas: " + actual + "\n" + msg);
      }
    }

    public static void AssertEqual(int[] expected, int[] actual, String msg) {
      if (expected.length != actual.length) {
        Assert.fail(
          "\nexpected: " + toString(expected) + "\n" + "\nwas: " +
            toString(actual) + "\n" + msg);
      }
      for (int i = 0; i < expected.length; ++i) {
        if (expected[i] != actual[i]) {
          Assert.fail("\nexpected: " + toString(expected) + "\n" +
                "\nwas: " + toString(actual) + "\n" + msg);
        }
      }
    }

    private void TestIdempotent(String str, Normalization norm) {
      boolean isForm = NormalizerInput.IsNormalized(str, norm);
      String newStr = NormalizerInput.Normalize(str, norm);
      ICharacterInput sci = new CharacterReader(str, false, true);
      boolean isForm2 = NormalizerInput.IsNormalized(sci, norm);
      if (isForm) {
        AssertEqual(str, newStr, EncodingTest.EscapeString(str));
      }
      Assert.assertEquals(isForm, isForm2);
      if (!NormalizerInput.IsNormalized(newStr, norm)) {
        Assert.fail(EncodingTest.EscapeString(str));
      }
      if (!isForm) {
        String newStr2 = NormalizerInput.Normalize(newStr, norm);
        AssertEqual(newStr, newStr2, EncodingTest.EscapeString(str));
      }
    }

    @Test(timeout = 200000)
    public void NormTestRandom() {
      System.out.println("Initializing random");
      RandomGenerator rand = new RandomGenerator(new XorShift128Plus(false));
      System.out.println("Initialized random");
      for (int i = 0; i < 10000; ++i) {
        if (i % 100 == 0) {
 System.out.println(i);
}
        String str = EncodingTest.RandomString(rand);
        this.TestIdempotent(str, Normalization.NFC);
        this.TestIdempotent(str, Normalization.NFD);
        this.TestIdempotent(str, Normalization.NFKC);
        this.TestIdempotent(str, Normalization.NFKD);
}
System.out.println("Done");
    }

    @Test
    public void NormTestSpecific() {
      String str = "_\ufac7\uc972+67 Tqd R_.";
      {
String stringTemp = NormalizerInput.Normalize(
  str,
  Normalization.NFC);
Assert.assertEquals(
  "_\u96e3\uc972+67 Tqd R_.",
  stringTemp);
}
      if (
        NormalizerInput.IsNormalized(str, Normalization.NFC)) {
 Assert.fail();
 }
      this.TestIdempotent(str, Normalization.NFC);
      str = "_\u96e3\uc972+67 Tqd R_._";
      if (!(
        NormalizerInput.IsNormalized(str, Normalization.NFC))) {
 Assert.fail();
 }
        this.TestIdempotent(str, Normalization.NFC);
    }

    private static final class NormResult {
      private final int[] orig;
      private final String origstr;
      private final String nfc;
      private final String nfd;
      private final String nfkc;
      private final String nfkd;
      private final String line;

      public NormResult(String column, String line) {
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
            Assert.fail(line);
          }
        }
        if (!NormalizerInput.IsNormalized(
          this.nfd,
          Normalization.NFD)) {
          {
            Assert.fail(line);
          }
        }
        if (!NormalizerInput.IsNormalized(
          this.nfkc,
          Normalization.NFKC)) {
          {
            Assert.fail(line);
          }
        }
        if (!NormalizerInput.IsNormalized(
          this.nfkd,
          Normalization.NFKD)) {
          {
            Assert.fail(line);
          }
        }
      }

      public void AssertNFC(NormResult... other) {
        for (NormResult o : other) {
 AssertEqual(this.origstr, o.nfc, this.line);
 }
      }

      public void AssertNFD(NormResult... other) {
        for (NormResult o : other) {
 AssertEqual(this.origstr, o.nfd, this.line);
 }
      }

      public void AssertNFKC(NormResult... other) {
        for (NormResult o : other) {
 AssertEqual(this.origstr, o.nfkc, this.line);
 }
      }

      public void AssertNFKD(NormResult... other) {
        for (NormResult o : other) {
 AssertEqual(this.origstr, o.nfkd, this.line);
 }
      }
    }
    /*

@Test
public void TestNormD() {
  int[] cp = new int[1];
      int[] pairs = NormalizationData.ComposedPairs;
      for (int i = 0; i < pairs.length; i += 3) {
        int p1 = pairs[i];
        int p2 = pairs[i + 1];
        int p3 = pairs[i + 2];
        if (IsSpecialChar(p1) ||
IsSpecialChar(p2) ||
IsSpecialChar(p3)) {
          System.out.println("{0:X4},{1:X4},{2:X4}", p1, p2, p3);
        }
      }
  for (int i = 0; i < 0x110000; ++i) {
          if ((i & 0xf800) == 0xd800) {
 continue;
}
          cp.charAt(0) = i;
          String codePointString = ToCodePointString(cp);
    String nfc = NormalizerInput.Normalize(codePointString,
            Normalization.NFD);
    String nfd = NormalizerInput.Normalize(codePointString,
            Normalization.NFD);
String nfdfs = NormalizerInput.Normalize(codePointString,
            Normalization.NFDForFilesystems);
   if (!(nfdfs.length() > 0)) {
 Assert.fail(
            EncodingTest.EscapeString(codePointString));
 }
          if (!nfc.equals(codePointString)) {
 continue;
}
          if (!nfd.equals(nfdfs)) {
            System.out.println("---");
            System.out.println(EncodingTest.EscapeString(codePointString));
            System.out.println(EncodingTest.EscapeString(nfc));
            System.out.println(EncodingTest.EscapeString(nfd));
            System.out.println(EncodingTest.EscapeString(nfdfs));
          }
        }
    }
*/
    @Test(timeout = 60000)
    public void NormTest() {
      boolean[] handled = new boolean[0x110000];
      String[] lines = NetHelper.DownloadOrOpenAllLines(
        "http://www.unicode.org/Public/UNIDATA/NormalizationTest.txt",
        "NormalizationTest.txt");
      if (lines == null) {
 Assert.fail();
 }
      if (!(lines.length > 0)) {
 Assert.fail();
 }
      boolean part1 = false;
      for (String lineItem : lines) {
        String line = lineItem;
        int hash = line.indexOf("#");
        if (hash >= 0) {
          line = line.substring(0, hash);
        }
        if (line.startsWith("@")) {
          part1 = line.indexOf("@Part1") == 0;
          continue;
        }
        if (line.length() == 0) {
          continue;
        }
        String[] columns = SplitAt(line, ";");
        int[] cps = GetCodePoints(columns[0]);
        if (part1) {
          handled[cps[0]] = true;
        }
        NormResult[] nr = new NormResult[5];
        for (int i = 0; i < 5; ++i) {
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
      char[] cptemp = new char[2];
      // Individual code points that don't appear in Part 1 of the
      // test will normalize to themselves in all four normalization forms
      for (int i = 0; i < handled.length; ++i) {
        if ((i & 0xfff800) == 0xd800) {
          // skip surrogate code points
          continue;
        }
        if (!handled[i]) {
          if (i >= 0x10000) {
            cptemp[0] = (char)((((i - 0x10000) >> 10) & 0x3ff) | 0xd800);
            cptemp[1] = (char)(((i - 0x10000) & 0x3ff) | 0xdc00);
          } else {
            cptemp[0] = (char)i;
          }
          String cpstr = new String(cptemp, 0, i >= 0x10000 ? 2 : 1);
          if (!NormalizerInput.IsNormalized(
            cpstr,
            Normalization.NFC)) {
            Assert.fail(TestCommon.IntToString(i));
          }
          if (!NormalizerInput.IsNormalized(
            cpstr,
            Normalization.NFD)) {
                    Assert.fail(TestCommon.IntToString(i));
          }
          if (!NormalizerInput.IsNormalized(
            cpstr,
            Normalization.NFKC)) {
                    Assert.fail(TestCommon.IntToString(i));
 }
          if (!NormalizerInput.IsNormalized(
            cpstr,
            Normalization.NFKD)) {
                    Assert.fail(TestCommon.IntToString(i));
        }
          String imsg = TestCommon.IntToString(i);
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
