package com.upokecenter.test; import com.upokecenter.util.*;

    /**
     * A class that implements a statistically-random byte generator, using
     * Sebastiano Vigna's <a
     * href='http://xorshift.di.unimi.it/xorshift128plus.c'>xorshift128+</a>
     * RNG as the underlying implementation. By default, this class is safe
     * for concurrent use among multiple threads.
     */
  public class XorShift128Plus implements IRandomGen {
    private long[] s = new long[2];
    private Object syncRoot = new Object();
    private boolean threadSafe;

    /**
     * Initializes a new instance of the XorShift128Plus class.
     */
    public XorShift128Plus() {
 this(true);
        }

    /**
     * Initializes a new instance of the XorShift128Plus class.
     * @param threadSafe A Boolean object.
     */
        public XorShift128Plus(boolean threadSafe) {
      this.threadSafe = threadSafe;
      this.Seed();
    }

    private int GetBytesInternal(byte[] bytes, int offset, int length) {
            int count = length;
            while (length >= 8) {
                long nv = this.NextValue();
                bytes[offset++] = ((byte)nv);
                nv >>= 8;
                bytes[offset++] = ((byte)nv);
                nv >>= 8;
                bytes[offset++] = ((byte)nv);
                nv >>= 8;
                bytes[offset++] = ((byte)nv);
                nv >>= 8;
                bytes[offset++] = ((byte)nv);
                nv >>= 8;
                bytes[offset++] = ((byte)nv);
                nv >>= 8;
                bytes[offset++] = ((byte)nv);
                nv >>= 8;
                bytes[offset++] = ((byte)nv);
                length -= 8;
            }
            if (length != 0) {
                long nv = this.NextValue();
                while (length > 0) {
                    bytes[offset++] = ((byte)nv);
                    nv >>= 8;
                    --length;
                }
            }
      return count;
    }

    /**
     * Not documented yet.
     * @param bytes The parameter {@code bytes} is not documented yet.
     * @param offset A zero-based index showing where the desired portion of {@code
     * bytes} begins.
     * @param length The length, in bytes, of the desired portion of {@code bytes}
     * (but not more than {@code bytes} 's length).
     * @return A 32-bit signed integer.
     * @throws NullPointerException The parameter {@code bytes} is null.
     * @throws IllegalArgumentException Either {@code offset} or {@code length} is less
     * than 0 or greater than {@code bytes} 's length, or {@code bytes} 's
     * length minus {@code offset} is less than {@code length}.
     */
    public int GetBytes(byte[] bytes, int offset, int length) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + offset +
          ") is less than 0");
      }
      if (offset > bytes.length) {
        throw new IllegalArgumentException("offset (" + offset +
          ") is more than " + bytes.length);
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + length +
          ") is less than 0");
      }
      if (length > bytes.length) {
        throw new IllegalArgumentException("length (" + length +
          ") is more than " + bytes.length);
      }
      if (bytes.length - offset < length) {
        throw new IllegalArgumentException("bytes's length minus " + offset + " (" +
          (bytes.length - offset) + ") is less than " + length);
      }
      if (this.threadSafe) {
        synchronized (this.syncRoot) {
          return this.GetBytesInternal(bytes, offset, length);
        }
      } else {
        return this.GetBytesInternal(bytes, offset, length);
      }
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
           return this.s[1] + s0;
    }

    private void Seed() {
      long lb = new java.util.Date().getTime() & 0xffffffffffL;
      this.s[0] = lb;
      lb = 0L;
      this.s[1] = lb;
      if ((this.s[0] | this.s[1]) == 0) {
        ++this.s[0];
      }
    }
  }
