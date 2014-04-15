/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

namespace PeterO.Mail
{
    /// <summary>An IdentityEncoder.</summary>
  internal sealed class IdentityEncoder : IStringEncoder
  {
    public IdentityEncoder() {
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A StringBuilder object.</param>
    /// <param name='data'>A byte array.</param>
    /// <param name='offset'>A 32-bit signed integer.</param>
    /// <param name='count'>A 32-bit signed integer. (2).</param>
public void WriteToString(StringBuilder str, byte[] data, int offset, int count) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count < 0) {
        throw new ArgumentException("count (" + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" + Convert.ToString((long)(data.Length - offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count == 0) {
 return;
}
      for (int i = 0; i < count; ++i) {
        str.Append((char)(((int)data[i + offset]) & 0xff));
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A StringBuilder object.</param>
public void FinalizeEncoding(StringBuilder str) {
      // No need to finalize for identity encodings
    }
  }
}
