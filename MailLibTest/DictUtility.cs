using System;
using System.Collections.Generic;

namespace MailLibTest {
  public static class DictUtility {
    public static IDictionary<string, string> MakeDict(params string[]
  keyvalues) {
      if (keyvalues == null) {
        throw new ArgumentNullException("keyvalues");
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
