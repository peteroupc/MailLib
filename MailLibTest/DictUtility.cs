using System;
using System.Collections.Generic;

namespace MailLibTest {
  public static class DictUtility {
    public static IList<IDictionary<String, String>>
      DictList(
      params IDictionary<String, String>[] dicts) {
      if (dicts == null) {
        throw new ArgumentNullException(nameof(dicts));
      }
       IList<IDictionary<String, String>> list = 
        new List<IDictionary<String, String>>();
       foreach (IDictionary<String, String> dict in dicts) {
          list.Add(dict);
       }
       return list;
    }

    public static IDictionary<string, string> MakeDict(params string[]
  keyvalues) {
      if (keyvalues == null) {
        throw new ArgumentNullException(nameof(keyvalues));
      }
      if (keyvalues.Length % 2 != 0) {
        throw new ArgumentException("keyvalues");
      }
      var dict = new Dictionary<string, string>();
      for (var i = 0; i < keyvalues.Length; i += 2) {
        dict.Add((string)keyvalues[i], keyvalues[i + 1]);
      }
      return dict;
    }
  }
}
