package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;

  public final class ResourceUtil {
private ResourceUtil() {
}
    public static String[] GetStrings(String name) {
      // NOTE: Must have a different name from Resources,
      // which is currently used in the Encoding library. Somehow,
      // in the Java version, if the test program and
      // the encoding library share a class loader,
      // the test program's Resources bundle overrides the encoding
      // library's Resources bundle, so that the encoding library could
      // access the former by accident and not find the resources
      // it needs.
      AppResources resources = new AppResources("TestResources");
      return DictUtility.ParseJSONStringArray(
          resources.GetString(name));
    }
    public static List<Map<String, String>> GetDictList(String name) {
      AppResources resources = new AppResources("TestResources");
      return DictUtility.ParseJSONDictList(
          resources.GetString(name));
    }
  }
