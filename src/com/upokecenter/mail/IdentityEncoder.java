package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * An IdentityEncoder.
     */
  final class IdentityEncoder implements IStringEncoder
  {
    public IdentityEncoder () {
    }

    /**
     * Not documented yet.
     * @param str A StringBuilder object.
     * @param data A byte array.
     * @param offset A 32-bit signed integer.
     * @param count A 32-bit signed integer. (2).
     */
    public void WriteToString(StringBuilder str, byte[] data, int offset, int count) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)data.length));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is more than " + Long.toString((long)data.length));
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length - offset)) + ") is less than " + Long.toString((long)count));
      }
      if (count == 0) {
        return;
      }
      for (int i = 0; i < count; ++i) {
        str.append((char)(((int)data[i + offset]) & 0xff));
      }
    }

    /**
     * Not documented yet.
     * @param str A StringBuilder object.
     */
    public void FinalizeEncoding(StringBuilder str) {
      // No need to finalize for identity encodings
    }

    /**
     * Not documented yet.
     * @param str A StringBuilder object.
     * @param b A Byte object.
     */
    public void WriteToString(StringBuilder str, byte b) {
      str.append((char)(((int)b) & 0xff));
    }
  }
