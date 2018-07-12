package com.upokecenter.test; import com.upokecenter.util.*;

import java.util.*;

  public final class DictUtility {
private DictUtility() {
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
