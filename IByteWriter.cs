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
  public interface IByteWriter {
    /// <summary>Writes an 8-bit byte to a data source.</summary>
    void WriteByte(int b);
  }
}
