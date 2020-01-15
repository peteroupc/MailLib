package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;
import java.io.*;

  public final class ResourceUtil {
private ResourceUtil() {
}
    public static String[] GetStrings(String name) {
      AppResources resources = new AppResources("Resources");
      return DictUtility.ParseJSONStringArray(
          resources.GetString(name));
    }
    public static List<Map<String, String>> GetDictList(String name) {
      AppResources resources = new AppResources("Resources");
      return DictUtility.ParseJSONDictList(
          resources.GetString(name));
    }
  }
