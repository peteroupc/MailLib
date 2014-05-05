package com.upokecenter.util;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * A lightweight version of MemoryStream, since it doesn't derive from
     * InputStream and doesn't use IO exceptions.
     */
  public final class ArrayWriter {
    private int retvalPos;
    private int retvalMax;
    private byte[] retval;

    public ArrayWriter () {
 this(16);
    }

    public ArrayWriter (int initialSize) {
      this.retval = new byte[initialSize];
    }

    /**
     * Not documented yet.
     * @param length A 32-bit signed integer.
     */
    public void SetLength(int length) {
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > this.retvalMax) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)this.retvalMax));
      }
      this.retvalMax = length;
      if (this.retvalPos > this.retvalMax) {
        this.retvalPos = this.retvalMax;
      }
    }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public int getPosition() {
        return this.retvalPos;
      }
public void setPosition(int value) {
        if (value < 0) {
          throw new IllegalArgumentException("value (" + Long.toString((long)value) + ") is less than " + "0");
        }
        if (value > this.retvalMax) {
          throw new IllegalArgumentException("value (" + Long.toString((long)value) + ") is more than " + Long.toString((long)this.retvalMax));
        }
        this.retvalPos = value;
      }

    public byte[] ToArray() {
      byte[] ret = new byte[this.retvalMax];
      System.arraycopy(this.retval, 0, ret, 0, this.retvalMax);
      return ret;
    }

    /**
     * Not documented yet.
     * @param src A byte array.
     * @param offset A 32-bit signed integer. (2).
     * @param length A 32-bit signed integer. (3).
     * @return A 32-bit signed integer.
     */
    public int ReadBytes(byte[] src, int offset, int length) {
      if (src == null) {
        throw new NullPointerException("src");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > src.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)src.length));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > src.length) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)src.length));
      }
      if (src.length - offset < length) {
        throw new IllegalArgumentException("src's length minus " + offset + " (" + Long.toString((long)(src.length - offset)) + ") is less than " + Long.toString((long)length));
      }
      int maxLength = Math.min(length, this.retvalMax - this.retvalPos);
      System.arraycopy(this.retval, this.retvalPos, src, offset, maxLength);
      this.retvalPos = (this.retvalPos + maxLength);
      this.retvalMax = Math.max(this.retvalMax, this.retvalPos);
      return maxLength;
    }

    /**
     * Not documented yet.
     * @param src A byte array.
     * @param offset A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     */
    public void WriteBytes(byte[] src, int offset, int length) {
      if (src == null) {
        throw new NullPointerException("src");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > src.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)src.length));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > src.length) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)src.length));
      }
      if (src.length - offset < length) {
        throw new IllegalArgumentException("src's length minus " + offset + " (" + Long.toString((long)(src.length - offset)) + ") is less than " + Long.toString((long)length));
      }
      if (this.retval.length - this.retvalPos > length) {
        // Array too big, make it grow
        int newLength = Math.max(this.retvalPos + length + 1000, this.retval.length * 2);
        byte[] newArray = new byte[newLength];
        System.arraycopy(this.retval, 0, newArray, 0, this.retvalPos);
        this.retval = newArray;
      }
      System.arraycopy(src, offset, this.retval, this.retvalPos, length);
      this.retvalPos = (this.retvalPos + length);
      this.retvalMax = Math.max(this.retvalMax, this.retvalPos);
    }
  }
