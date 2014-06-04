package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.text.*;

  public class NormalizationTest {
    private static String TrimSpaces(String s) {
      return s.Trim();
    }

    public static int[] GetCodePoints(String cp) {
      String[] cpArray=TrimSpaces(cp).Split(' ');
      int[] ret = new int[cpArray.length];
      int index = 0;
      for(String v : cpArray) {
        int hex = Integer.parseInt(TrimSpaces(v));
        ret.charAt(index++)=hex;
      }
      return ret;
    }

    public static String toString(int[] array) {
      StringBuilder builder = new StringBuilder();
      boolean first = true;
      builder.append("[");
      for(int v : array) {
        if (!first) {
          builder.append(", ");
        }
        builder.append((v).toString());
        first = false;
      }
      builder.append("]");
      return builder.toString();
    }

    public static String ToCodePointString(int[] array) {
      StringBuilder builder = new StringBuilder();
      for(int v : array) {
        if (v <= 0xffff) {
          builder.append((char)v);
        } else if (v <= 0x10ffff) {
          builder.append((char)((((v - 0x10000) >> 10) & 0x3ff) + 0xd800));
          builder.append((char)(((v - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
      return builder.toString();
    }

    public static int[] NormalizerGetChars(int[] cp, Normalization form) {
      String ret = PeterO.Text.Normalizer.Normalize(ToCodePointString(cp), form);
      List<Integer> list = new ArrayList<Integer>();
      for (int i = 0;i<ret.length(); ++i) {
        int c = PeterO.DataUtilities.CodePointAt(ret, i);
        if (c >= 0x10000) {
          ++i;
        }
        list.add(c);
      }
      return ToIntArray(list);
    }

    public static int[] ToIntArray(List<Integer> list) {
      int[] ret = new int[list.size()];
      int index = 0;
      for(int v : list) {
        ret.charAt(index++)=v;
      }
      return ret;
    }
    public static void AssertEqual(int expected, int actual, String msg) {
      if (expected != actual) {
        Assert.assertEquals(msg,expected,actual);
      }
    }
    public static void AssertEqual(int[] expected, int[] actual, String msg) {
      if (expected.length != actual.length) {
        Assert.fail(
          "\nexpected: "+toString(expected)+"\n"+
          "\nwas:      "+toString(actual)+"\n"+msg);
      }
      for (int i = 0;i<expected.length; ++i) {
        if (expected[i]!=actual[i]) {
          Assert.fail(
            "\nexpected: "+toString(expected)+"\n"+
            "\nwas:      "+toString(actual)+"\n"+msg);
        }
      }
    }

    @Test
    public void NormTest() {
      String normTestFile="..\\..\\..\\cache\\NormalizationTest.txt";
      if (!File.Exists(normTestFile)) {
        Assert.Inconclusive();
      }
      boolean[] handled = new boolean[0x110000];
      using(LineNumberReader reader = new LineNumberReader(normTestFile)) {
        boolean part1 = false;
        while (true) {
          String line = reader.readLine();
          if (line == null) {
            break;
          }
          line = TrimSpaces(line);
          int hash=line.indexOf("#");
          if (hash >= 0) {
            line = TrimSpaces(line.substring(0,hash));
          }
          if (line.StartsWith("@")) {
            if (line.equals("@Part1")) {
              part1 = true;
            } else {
              part1 = false;
            }
            continue;
          }
          line = TrimSpaces(line);
          if (line.length() == 0) {
            continue;
          }
          String[] columns=line.Split(';');
          int[] cps = GetCodePoints(columns[0]);
          if (part1) {
            handled[cps[0]]=true;
          }
          int[] expected = GetCodePoints(columns[1]);
          int[] actual;
          int[] ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFC);
          if (!NormalizingCharacterInput.IsNormalized(expected, Normalization.NFC)) {
            Assert.fail(line);
          }
          AssertEqual(expected, actual, line);
          ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFD);
          AssertEqual(GetCodePoints(columns[2]), actual, line);
          if (!NormalizingCharacterInput.IsNormalized(GetCodePoints(columns[2]), Normalization.NFD)) {
            Assert.fail(line);
          }
          ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFKC);
          AssertEqual(GetCodePoints(columns[3]), actual, line);
          if (!NormalizingCharacterInput.IsNormalized(GetCodePoints(columns[3]), Normalization.NFKC)) {
            Assert.fail(line);
          }
          ci = cps;
          actual = NormalizerGetChars(ci, Normalization.NFKD);
          AssertEqual(GetCodePoints(columns[4]), actual, line);
          if (!NormalizingCharacterInput.IsNormalized(GetCodePoints(columns[4]), Normalization.NFKD)) {
            Assert.fail(line);
          }
        }
      }
      char[] cptemp = new char[2];
      // Individual code points that don't appear in Part 1 of the
      // test will normalize to themselves in all four normalization forms
      for (int i = 0;i<handled.length; ++i) {
        if ((i & 0xf800) == 0xd800) {
          // skip surrogate code points
          continue;
        }
        if (!handled[i]) {
          if (i >= 0x10000) {
            cptemp[0]=((char)((((i - 0x10000) >> 10) & 0x3ff) + 0xd800));
            cptemp[1]=((char)(((i - 0x10000) & 0x3ff) + 0xdc00));
          } else {
            cptemp[0] = (char)i;
          }
          String cpstr = new String(cptemp, 0, (i >= 0x10000 ? 2 : 1));
          String imsg=""+i;
          Assert.assertEquals(cpstr, Normalizer.Normalize(cpstr, Normalization.NFC));
          Assert.assertEquals(cpstr, Normalizer.Normalize(cpstr, Normalization.NFD));
          Assert.assertEquals(cpstr, Normalizer.Normalize(cpstr, Normalization.NFKC));
          Assert.assertEquals(cpstr, Normalizer.Normalize(cpstr, Normalization.NFKD));
          if (!PeterO.Text.Normalizer.IsNormalized(cpstr, Normalization.NFC)) {
            Assert.fail(imsg);
          }
          if (!PeterO.Text.Normalizer.IsNormalized(cpstr, Normalization.NFD)) {
            Assert.fail(imsg);
          }
          if (!PeterO.Text.Normalizer.IsNormalized(cpstr, Normalization.NFKC)) {
            Assert.fail(imsg);
          }
          if (!PeterO.Text.Normalizer.IsNormalized(cpstr, Normalization.NFKD)) {
            Assert.fail(imsg);
          }
        }
      }
      // Additional normalization tests
      if(NormalizingCharacterInput.IsNormalized("x\u0300\u0323yz",Normalization.NFC))Assert.fail();
      if(NormalizingCharacterInput.IsNormalized("x\u0300\u0323",Normalization.NFC))Assert.fail();
    }
  }
