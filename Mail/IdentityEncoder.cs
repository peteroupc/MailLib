/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;

namespace PeterO.Mail {
    /// <summary>An IdentityEncoder.</summary>
  internal sealed class IdentityEncoder : IStringEncoder
  {
    public void WriteToString(StringBuilder str, byte[] data, int offset,
      int count) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
    throw new ArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          data.Length);
      }
      if (count < 0) {
      throw new ArgumentException("count (" + count + ") is less than " +
          "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + count + ") is more than " +
          data.Length);
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" +
          (data.Length - offset) + ") is less than " + count);
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
