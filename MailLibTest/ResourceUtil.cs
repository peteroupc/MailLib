using System;
using System.IO;
using System.Resources;
using MailLibTest;
namespace Test {
  public static class ResourceUtil {
    public static string[] GetStrings(string name) {
      var resources = new AppResources("Resources");
      return DictUtility.ParseJSONStringArray(
        resources.GetString(name));
    }
  }
}
