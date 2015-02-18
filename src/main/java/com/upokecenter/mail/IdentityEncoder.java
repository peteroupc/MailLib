package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

    /**
     * An IdentityEncoder.
     */
  final class IdentityEncoder implements IStringEncoder
  {
    public void WriteToString(StringBuilder str, byte[] data, int offset,
      int count) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
    throw new IllegalArgumentException("offset (" + offset + ") is less than " +
          "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          data.length);
      }
      if (count < 0) {
      throw new IllegalArgumentException("count (" + count + ") is less than " +
          "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + count + ") is more than " +
          data.length);
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" +
          (data.length - offset) + ") is less than " + count);
      }
      if (count == 0) {
        return;
      }
      for (int i = 0; i < count; ++i) {
        str.append((char)(((int)data[i + offset]) & 0xff));
      }
    }

    public void FinalizeEncoding(StringBuilder str) {
      // No need to finalize for identity encodings
    }

    public void WriteToString(StringBuilder str, byte b) {
      str.append((char)(((int)b) & 0xff));
    }
  }
