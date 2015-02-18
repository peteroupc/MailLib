package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

    /**
     * A lightweight version of MemoryStream, since it doesn't derive from InputStream
     * and doesn't use IO exceptions.
     */
  final class ArrayWriter {
    private int retvalPos;
    private int retvalMax;
    private byte[] retval;

    public ArrayWriter () {
 this(16);
    }

    public ArrayWriter (int initialSize) {
      this.retval = new byte[initialSize];
    }

    public void SetLength(int length) {
      if (length < 0) {
    throw new IllegalArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > this.retvalMax) {
        throw new IllegalArgumentException("length (" + length + ") is more than "+
          (this.retvalMax));
      }
      this.retvalMax = length;
      if (this.retvalPos > this.retvalMax) {
        this.retvalPos = this.retvalMax;
      }
    }

    public final int getPosition() {
        return this.retvalPos;
      }
public final void setPosition(int value) {
        if (value < 0) {
      throw new IllegalArgumentException("value (" + value + ") is less than " +
            "0");
        }
        if (value > this.retvalMax) {
          throw new IllegalArgumentException("value (" + value + ") is more than "+
            (this.retvalMax));
        }
        this.retvalPos = value;
      }

    public byte[] ToArray() {
      byte[] ret = new byte[this.retvalMax];
      System.arraycopy(this.retval, 0, ret, 0, this.retvalMax);
      return ret;
    }

    public int ReadBytes(byte[] src, int offset, int length) {
      if (src == null) {
        throw new NullPointerException("src");
      }
      if (offset < 0) {
    throw new IllegalArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > src.length) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          src.length);
      }
      if (length < 0) {
    throw new IllegalArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > src.length) {
        throw new IllegalArgumentException("length (" + length + ") is more than " +
          src.length);
      }
      if (src.length - offset < length) {
        throw new IllegalArgumentException("src's length minus " + offset + " (" +
          (src.length - offset) + ") is less than " + length);
      }
      int maxLength = Math.min(length, this.retvalMax - this.retvalPos);
      System.arraycopy(this.retval, this.retvalPos, src, offset, maxLength);
      this.retvalPos = (this.retvalPos + maxLength);
      this.retvalMax = Math.max(this.retvalMax, this.retvalPos);
      return maxLength;
    }

    public void WriteBytes(byte[] src, int offset, int length) {
      if (src == null) {
        throw new NullPointerException("src");
      }
      if (offset < 0) {
    throw new IllegalArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > src.length) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          src.length);
      }
      if (length < 0) {
    throw new IllegalArgumentException("length (" + length + ") is less than " +
          "0");
      }
      if (length > src.length) {
        throw new IllegalArgumentException("length (" + length + ") is more than " +
          src.length);
      }
      if (src.length - offset < length) {
        throw new IllegalArgumentException("src's length minus " + offset + " (" +
          (src.length - offset) + ") is less than " + length);
      }
      if (this.retval.length - this.retvalPos < length) {
        // Array too small, make it grow
        int newLength = Math.max(this.retvalPos + length + 1000,
          this.retval.length * 2);
        byte[] newArray = new byte[newLength];
        System.arraycopy(this.retval, 0, newArray, 0, this.retvalPos);
        this.retval = newArray;
      }
      System.arraycopy(src, offset, this.retval, this.retvalPos, length);
      this.retvalPos = (this.retvalPos + length);
      this.retvalMax = Math.max(this.retvalMax, this.retvalPos);
    }
  }
