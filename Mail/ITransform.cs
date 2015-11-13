/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Mail {
    /// <summary>A generic interface for reading data one byte at a time.</summary>
  public interface ITransform {
    /// <summary>Reads a byte from the data source.</summary>
    /// <returns>The byte read, or -1 if the end of the source is reached.</returns>
    int ReadByte();
  }
}
