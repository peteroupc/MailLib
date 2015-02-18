/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Text {
    /// <summary>A lightweight version of MemoryStream, since it doesn't derive from
    /// Stream and doesn't use IO exceptions.</summary>
  internal sealed class ArrayWriter {
    private int retvalPos;
    private int retvalMax;
    private byte[] retval;

    /// <summary>Initializes a new instance of the ArrayWriter class.</summary>
    public ArrayWriter() : this(16) {
    }

    /// <summary>Initializes a new instance of the ArrayWriter class.</summary>
    /// <param name='initialSize'>A 32-bit signed integer.</param>
    public ArrayWriter(int initialSize) {
      this.retval = new byte[initialSize];
    }

    public void SetLength(int length) {
      if (length < 0) {
    throw new ArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > this.retvalMax) {
        throw new ArgumentException("length (" + length + ") is more than "+
          (this.retvalMax));
      }
      this.retvalMax = length;
      if (this.retvalPos > this.retvalMax) {
        this.retvalPos = this.retvalMax;
      }
    }

    public int Position {
      get {
        return this.retvalPos;
      }

      set {
        if (value < 0) {
      throw new ArgumentException("value (" + value + ") is less than " +
            "0");
        }
        if (value > this.retvalMax) {
          throw new ArgumentException("value (" + value + ") is more than "+
            (this.retvalMax));
        }
        this.retvalPos = value;
      }
    }

    public byte[] ToArray() {
      var ret = new byte[this.retvalMax];
      Array.Copy(this.retval, 0, ret, 0, this.retvalMax);
      return ret;
    }

    public int ReadBytes(byte[] src, int offset, int length) {
      if (src == null) {
        throw new ArgumentNullException("src");
      }
      if (offset < 0) {
    throw new ArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > src.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          src.Length);
      }
      if (length < 0) {
    throw new ArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > src.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          src.Length);
      }
      if (src.Length - offset < length) {
        throw new ArgumentException("src's length minus " + offset + " (" +
          (src.Length - offset) + ") is less than " + length);
      }
      int maxLength = Math.Min(length, this.retvalMax - this.retvalPos);
      Array.Copy(this.retval, this.retvalPos, src, offset, maxLength);
      this.retvalPos = checked(this.retvalPos + maxLength);
      this.retvalMax = Math.Max(this.retvalMax, this.retvalPos);
      return maxLength;
    }

    public void WriteBytes(byte[] src, int offset, int length) {
      if (src == null) {
        throw new ArgumentNullException("src");
      }
      if (offset < 0) {
    throw new ArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > src.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          src.Length);
      }
      if (length < 0) {
    throw new ArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > src.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          src.Length);
      }
      if (src.Length - offset < length) {
        throw new ArgumentException("src's length minus " + offset + " (" +
          (src.Length - offset) + ") is less than " + length);
      }
      if (this.retval.Length - this.retvalPos < length) {
        // Array too small, make it grow
        int newLength = Math.Max(this.retvalPos + length + 1000,
          this.retval.Length * 2);
        var newArray = new byte[newLength];
        Array.Copy(this.retval, 0, newArray, 0, this.retvalPos);
        this.retval = newArray;
      }
      Array.Copy(src, offset, this.retval, this.retvalPos, length);
      this.retvalPos = checked(this.retvalPos + length);
      this.retvalMax = Math.Max(this.retvalMax, this.retvalPos);
    }
  }
}
