/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.IO;
using System.Net;

namespace MailLibTest {
  internal static class NetHelper {
    public static string[] DownloadOrOpenAllLines(
      string location,
      string cachedPath) {
      if (!File.Exists(cachedPath)) {
        var request = WebRequest.Create(
            new Uri(location));
        var response = request.GetResponse();
        using (var stream = response.GetResponseStream()) {
          using (var output = new FileStream(cachedPath, FileMode.Create)) {
            var buffer = new byte[8192];
            while (true) {
              var b = stream.Read(buffer, 0, buffer.Length);
              if (b == 0) {
                break;
              }
              output.Write(buffer, 0, b);
            }
          }
        }
      }
      return File.ReadAllLines(cachedPath, System.Text.Encoding.UTF8);
    }
  }
}
