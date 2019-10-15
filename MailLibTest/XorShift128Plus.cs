using System;

namespace PeterO {
  /// <summary>A class that implements a statistically-random byte
  /// generator, using Sebastiano Vigna's
  /// <a
  ///   href='http://xorshift.di.unimi.it/xorshift128plus.c'>xorshift128+</a>
  /// RNG as the underlying implementation. By default, this class is
  /// safe for concurrent use among multiple threads.</summary>
  public class XorShift128Plus : IRandomGen {
    private readonly long[] s = new long[2];
    private object syncRoot = new Object();
    private bool threadSafe;

    /// <summary>Initializes a new instance of the XorShift128Plus
    /// class.</summary>
    public XorShift128Plus() : this(true) {
    }

    /// <summary>Initializes a new instance of the XorShift128Plus
    /// class.</summary>
    /// <param name='threadSafe'>A Boolean object.</param>
    public XorShift128Plus(bool threadSafe) {
      this.threadSafe = threadSafe;
      this.Seed();
    }

    private int GetBytesInternal(byte[] bytes, int offset, int length) {
      int count = length;
      while (length >= 8) {
        long nv = this.NextValue();
        bytes[offset++] = unchecked((byte)nv);
        nv >>= 8;
        bytes[offset++] = unchecked((byte)nv);
        nv >>= 8;
        bytes[offset++] = unchecked((byte)nv);
        nv >>= 8;
        bytes[offset++] = unchecked((byte)nv);
        nv >>= 8;
        bytes[offset++] = unchecked((byte)nv);
        nv >>= 8;
        bytes[offset++] = unchecked((byte)nv);
        nv >>= 8;
        bytes[offset++] = unchecked((byte)nv);
        nv >>= 8;
        bytes[offset++] = unchecked((byte)nv);
        length -= 8;
      }
      if (length != 0) {
        long nv = this.NextValue();
        while (length > 0) {
          bytes[offset++] = unchecked((byte)nv);
          nv >>= 8;
          --length;
        }
      }
      return count;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bytes'>The parameter <paramref name='bytes'/> is not
    /// documented yet.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of <paramref name='bytes'/> begins.</param>
    /// <param name='length'>The length, in bytes, of the desired portion
    /// of <paramref name='bytes'/> (but not more than <paramref
    /// name='bytes'/> 's length).</param>
    /// <returns>A 32-bit signed integer.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='length'/>.</exception>
    public int GetBytes(byte[] bytes, int offset, int length) {
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (offset < 0) {
        throw new ArgumentException("offset(" + offset +
          ") is less than 0");
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("offset(" + offset +
          ") is more than " + bytes.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length(" + length +
          ") is less than 0");
      }
      if (length > bytes.Length) {
        throw new ArgumentException("length(" + length +
          ") is more than " + bytes.Length);
      }
      if (bytes.Length - offset < length) {
        throw new ArgumentException("bytes's length minus " + offset + "(" +
          (bytes.Length - offset) + ") is less than " + length);
      }
      if (this.threadSafe) {
        lock (this.syncRoot) {
          return this.GetBytesInternal(bytes, offset, length);
        }
      }
      return this.GetBytesInternal(bytes, offset, length);
    }

    // xorshift128 + generator
    // http://xorshift.di.unimi.it/xorshift128plus.c
    private long NextValue() {
      long s1 = this.s[0];
      long s0 = this.s[1];
      this.s[0] = s0;
      s1 ^= s1 << 23;
      long t1 = (s1 >> 18) & 0x3fffffffffffL;
      long t0 = (s0 >> 5) & 0x7ffffffffffffffL;
      this.s[1] = s1 ^ s0 ^ t1 ^ t0;
      return unchecked(this.s[1] + s0);
    }

    private void Seed() {
      long lb = DateTime.UtcNow.Ticks & 0xffffffffffL;
      this.s[0] = lb;
      lb = 0L;
      this.s[1] = lb;
      if ((this.s[0] | this.s[1]) == 0) {
        ++this.s[0];
      }
    }
  }
}
