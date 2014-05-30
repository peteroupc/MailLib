/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

namespace PeterO.Mail {
    /// <summary>An IdentityEncoder.</summary>
  internal sealed class IdentityEncoder : IStringEncoder
  {
    public IdentityEncoder() {
    }

    public void WriteToString(StringBuilder str, byte[] data, int offset, int count) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((int)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((int)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count < 0) {
        throw new ArgumentException("count (" + Convert.ToString((int)count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + Convert.ToString((int)count, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((int)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" + Convert.ToString((int)(data.Length - offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((int)count, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count == 0) {
        return;
      }
      for (int i = 0; i < count; ++i) {
        str.Append((char)(((int)data[i + offset]) & 0xff));
      }
    }

    public void FinalizeEncoding(StringBuilder str) {
      // No need to finalize for identity encodings
    }

    public void WriteToString(StringBuilder str, byte b) {
      str.Append((char)(((int)b) & 0xff));
    }
  }
}
