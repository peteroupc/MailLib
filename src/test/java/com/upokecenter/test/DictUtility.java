package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

  public final class DictUtility {
private DictUtility() {
}
    public static List<Map<String, String>>
      DictList(
      Map<String, String>... dicts) {
      if (dicts == null) {
        throw new NullPointerException("dicts");
      }
       List<Map<String, String>> list =
        new ArrayList<Map<String, String>>();
       for (Map<String, String> dict : dicts) {
          list.add(dict);
       }
       return list;
    }

    public static Map<String, String> MakeDict(String...
  keyvalues) {
      if (keyvalues == null) {
        throw new NullPointerException("keyvalues");
      }
      if (keyvalues.length % 2 != 0) {
        throw new IllegalArgumentException("keyvalues");
      }
      HashMap<String, String> dict = new HashMap<String, String>();
      for (int i = 0; i < keyvalues.length; i += 2) {
        dict.put((String)keyvalues[i], keyvalues[i + 1]);
      }
      return dict;
    }
  }
