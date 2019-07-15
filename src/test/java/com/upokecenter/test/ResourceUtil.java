package com.upokecenter.test;

import java.io.*;
using System.Resources;
using MailLibTest;

  public final class ResourceUtil {
private ResourceUtil() {
}
    public static String[] GetStrings(String name) {
      AppResources resources = new AppResources("Resources");
      return DictUtility.ParseJSONStringArray(
        resources.GetString(name));
    }
  }
