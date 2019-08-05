using System;
using System.Collections.Generic;
using System.IO;
using MailLibTest;
namespace Test {
  public static class ResourceUtil {
    public static string[] GetStrings(string name) {
      var resources = new AppResources("Resources");
      return DictUtility.ParseJSONStringArray(
        resources.GetString(name));
    }
    public static IList<IDictionary<string, string>>
           GetDictList(string name) {
      var resources = new AppResources("Resources");
      return DictUtility.ParseJSONDictList(
        resources.GetString(name));
    }
  }
}
