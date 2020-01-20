using System;
using System.Collections.Generic;
using System.IO;
using Test;

namespace MailLibTest {
  public static class ResourceUtil {
    public static string[] GetStrings(string name) {
      // NOTE: Must have a different name from Resources,
      // which is currently used in the Encoding library. Somehow,
      // in the Java version, if the test program and
      // the encoding library share a class loader,
      // the test program's Resources bundle overrides the encoding
      // library's Resources bundle, so that the encoding library could
      // access the former by accident and not find the resources
      // it needs.
      var resources = new AppResources("TestResources");
      return DictUtility.ParseJSONStringArray(
          resources.GetString(name));
    }
    public static IList<IDictionary<string, string>> GetDictList(string name) {
      var resources = new AppResources("TestResources");
      return DictUtility.ParseJSONDictList(
          resources.GetString(name));
    }
  }
}
