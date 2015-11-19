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
    /// <summary>Writes a portion of a byte array to the data
    /// source.</summary>
    /// <param name='bytes'>A byte array containing the data to
    /// write.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of <paramref name='bytes'/> begins.</param>
    /// <param name='length'>The number of elements in the desired portion
    /// of <paramref name='bytes'/> (but not more than <paramref
    /// name='bytes'/> 's length).</param>
    /// <exception cref='ArgumentNullException'>Should be thrown if the
    /// parameter "bytes" is null.</exception>
    /// <exception cref='ArgumentException'>Should be thrown if either
    /// "offset" or "length" is less than 0 or greater than "bytes" 's
    /// length, or "bytes" 's length minus "offset" is less than
    /// "length".</exception>
    void Write(byte[] bytes, int offset, int length);
  }
}
