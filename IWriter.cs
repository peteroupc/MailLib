/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO {
    /// <summary>A generic interface for writing bytes of data.</summary>
  public interface IWriter : IByteWriter {
  /// <summary>Not documented yet.</summary>
    void Write(byte[] bytes, int offset, int length);
  }
}
