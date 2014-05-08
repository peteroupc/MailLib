/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Mail {
  // A seven-bit transform used for text/plain data
  internal sealed class LiberalSevenBitTransform : ITransform {
    private ITransform input;

    public LiberalSevenBitTransform(ITransform stream) {
      this.input = stream;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int ReadByte() {
      int ret = this.input.ReadByte();
      if (ret > 0x80 || ret == 0) {
        return '?';
      }
      return ret;
    }
  }
}
