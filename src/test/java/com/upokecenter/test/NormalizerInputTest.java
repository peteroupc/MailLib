package com.upokecenter.test; import com.upokecenter.util.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
import com.upokecenter.text.*;

  public class NormalizerInputTest {
    @Test
    public void TestConstructor() {
      // not implemented yet
    }
    @Test
    public void TestGetChars() {
      // not implemented yet
    }

    public static String RandomAscii(RandomGenerator rnd) {
      int length = rnd.UniformInt(50) + 1;
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < length; ++i) {
        char c = (char)rnd.UniformInt(128);
        sb.append(c);
      }
      return sb.toString();
    }

    public static String RandomLatinOne(RandomGenerator rnd) {
      int length = rnd.UniformInt(50) + 1;
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < length; ++i) {
        char c = (char)rnd.UniformInt(256);
        sb.append(c);
      }
      return sb.toString();
    }

    @Test
public void TestNormalizationAscii() {
  RandomGenerator rnd = new RandomGenerator();
  for (int i = 0; i < 50000; ++i) {
    String str = RandomAscii(rnd);
    // ASCII strings are already normalized
    if (!(NormalizerInput.IsNormalized(
      str,
      Normalization.NFC))) {
 Assert.fail();
 }
    if (!(NormalizerInput.IsNormalized(
      str,
      Normalization.NFD))) {
 Assert.fail();
 }
    if (!(NormalizerInput.IsNormalized(
      str,
      Normalization.NFKC))) {
 Assert.fail();
 }
    if (!(NormalizerInput.IsNormalized(
      str,
      Normalization.NFKD))) {
 Assert.fail();
 }
    // ASCII strings normalize to themselves
    String str2 = NormalizerInput.Normalize(
  str,
  Normalization.NFC);
    Assert.assertEquals(str, str2);
    str2 = NormalizerInput.Normalize(
  str,
  Normalization.NFD);
    Assert.assertEquals(str, str2);
    str2 = NormalizerInput.Normalize(
  str,
  Normalization.NFKC);
    Assert.assertEquals(str, str2);
    str2 = NormalizerInput.Normalize(
  str,
  Normalization.NFKD);
    Assert.assertEquals(str, str2);
  }
}

    @Test
public void TestNormalizationLatinOne() {
  RandomGenerator rnd = new RandomGenerator();
  for (int i = 0; i < 50000; ++i) {
    String str = RandomLatinOne(rnd);
    // Latin-1 strings are already normalized in NFC
    if (!(NormalizerInput.IsNormalized(
      str,
      Normalization.NFC))) {
 Assert.fail();
 }
    // Latin-1 strings normalize to themselves in NFC
    String str2 = NormalizerInput.Normalize(
  str,
  Normalization.NFC);
    Assert.assertEquals(str, str2);
  }
}

    @Test
    public void TestIsNormalized() {
      // Additional normalization tests
      if (NormalizerInput.IsNormalized(
        "x\u0300\u0323yz",
        Normalization.NFC)) {
 Assert.fail();
 }
      if (NormalizerInput.IsNormalized(
        "x\u0300\u0323",
        Normalization.NFC)) {
 Assert.fail();
 }
      try {
        NormalizerInput.IsNormalized(
  (ICharacterInput)null,
  Normalization.NFC);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }

      if (!(
  NormalizerInput.IsNormalized(
  "\ud836\udc00\ud836\udd00\ud836\ude00\ud836\udf00\ud837\udc00\ud837\udf00",
  Normalization.NFC))) {
 Assert.fail();
 }
      if (!(
      NormalizerInput.IsNormalized(
  "\ud836\udc00\ud836\udd00\ud836\ude00\ud836\udf00\ud837\udc00\ud837\udf00",
  Normalization.NFD))) {
 Assert.fail();
 }
      if (!(
      NormalizerInput.IsNormalized(
  "\ud836\udc00\ud836\udd00\ud836\ude00\ud836\udf00\ud837\udc00\ud837\udf00",
  Normalization.NFKC))) {
 Assert.fail();
 }
      if (!(
      NormalizerInput.IsNormalized(
  "\ud836\udc00\ud836\udd00\ud836\ude00\ud836\udf00\ud837\udc00\ud837\udf00",
  Normalization.NFKD))) {
 Assert.fail();
 }
    }
    @Test
    public void TestRead() {
      NormalizerInput nci = new NormalizerInput("test");
      try {
        nci.Read(null, 0, 0);
        Assert.fail("Should have failed");
      } catch (NullPointerException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        nci.Read(new int[] { 't' }, -1, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        nci.Read(new int[] { 't' }, 5, 1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        nci.Read(new int[] { 't' }, 0, -1);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        nci.Read(new int[] { 't' }, 0, 5);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      try {
        nci.Read(new int[] { 't', 't' }, 1, 2);
        Assert.fail("Should have failed");
      } catch (IllegalArgumentException ex) {
        // NOTE: Intentionally empty
} catch (Exception ex) {
        Assert.fail(ex.toString());
        throw new IllegalStateException("", ex);
      }
      Assert.assertEquals(1, nci.Read(new int[] { 't', 't' }, 1, 1));
    }
    @Test
    public void TestReadChar() {
      // not implemented yet
    }
  }
