package com.upokecenter.util;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.io.*;

  /**
   * Not documented yet.
   */
  public final class DataIO {
private DataIO() {
}
    private static final class ByteArrayTransform implements ITransform {
      private byte[] bytes;
      private int offset;
      private int endOffset;

      public ByteArrayTransform (byte[] bytes, int offset, int length) {
        this.bytes = bytes;
        this.offset = offset;
        this.endOffset = offset + length;
      }

      /**
       *
       */
/// </summary>
/// <returns></returns>
public int read() {
        if (this.offset >= this.endOffset) {
          return -1;
        }
        int b = this.bytes[this.offset++];
        return ((int)b) & 0xff;
      }
    }

    private static final class WrappedStream implements ITransform {
      private InputStream stream;

      public WrappedStream (InputStream stream) {
        this.stream = stream;
      }

      /**
       *
       */
/// </summary>
/// <returns></returns>
public int read() {
        try {
          return this.stream.read();
        } catch (IOException ex) {
          throw new IllegalStateException(ex.getMessage(), ex);
        }
      }
    }

    private static final class WrappedOutputStreamFromByteWriter implements IWriter {
      private IByteWriter output;

      public WrappedOutputStreamFromByteWriter (IByteWriter output) {
        this.output = output;
      }

      /**
       *
       */
/// </summary>
/// <param name="byteValue"></param>
/// <returns></returns>
      public void write(int byteValue) {
          this.output.write((byte)byteValue);
      }

      /**
       *
       */
/// </summary>
/// <param name="bytes"></param>
/// <param name="offset"></param>
/// <param name="length"></param>
/// <returns></returns>
public void write(byte[] bytes, int offset, int length) {
        if (bytes == null) {
          throw new NullPointerException("bytes");
        }
        if (offset < 0) {
          throw new IllegalArgumentException("offset (" + offset +
            ") is less than " + 0);
        }
        if (offset > bytes.length) {
          throw new IllegalArgumentException("offset (" + offset + ") is more than " +
            bytes.length);
        }
        if (length < 0) {
          throw new IllegalArgumentException("length (" + length +
            ") is less than " + 0);
        }
        if (length > bytes.length) {
          throw new IllegalArgumentException("length (" + length + ") is more than " +
            bytes.length);
        }
        if (bytes.length - offset < length) {
          throw new IllegalArgumentException("bytes's length minus " + offset + " (" +
            (bytes.length - offset) + ") is less than " + length);
        }
        for (int i = 0; i < length; ++i) {
          this.output.write((byte)bytes[i]);
        }
      }
    }

    private static final class WrappedOutputStream implements IWriter {
      private OutputStream output;

      public WrappedOutputStream (OutputStream output) throws java.io.IOException {
        this.output = output;
      }

      /**
       *
       */
/// </summary>
/// <param name="byteValue"></param>
/// <returns></returns>
public void write(int byteValue) {
        try {
          this.output.write((byte)byteValue);
        } catch (IOException ex) {
          throw new IllegalStateException(ex.getMessage(), ex);
        }
      }

      /**
       *
       */
/// </summary>
/// <param name="bytes"></param>
/// <param name="offset"></param>
/// <param name="length"></param>
/// <returns></returns>
public void write(byte[] bytes, int offset, int length) {
        try {
          this.output.write(bytes, offset, length);
        } catch (IOException ex) {
          throw new IllegalStateException(ex.getMessage(), ex);
        }
      }
    }

  /**
   * Not documented yet.
   * @return Not documented yet.
   */
    public static ITransform ToTransform(byte[] bytes) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      return new ByteArrayTransform(bytes, 0, bytes.length);
    }

  /**
   * Not documented yet.
   * @return Not documented yet.
   */
    public static ITransform ToTransform(
byte[] bytes,
int offset,
int length) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + offset +
          ") is less than " + 0);
      }
      if (offset > bytes.length) {
        throw new IllegalArgumentException("offset (" + offset + ") is more than " +
          bytes.length);
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + length +
          ") is less than " + 0);
      }
      if (length > bytes.length) {
        throw new IllegalArgumentException("length (" + length + ") is more than " +
          bytes.length);
      }
      if (bytes.length - offset < length) {
        throw new IllegalArgumentException("bytes's length minus " + offset + " (" +
          (bytes.length - offset) + ") is less than " + length);
      }
      return new ByteArrayTransform(bytes, offset, length);
    }

  /**
   * Not documented yet.
   * @return Not documented yet.
   */
    public static ITransform ToTransform(InputStream input) {
      if (input == null) {
        throw new NullPointerException("input");
      }
      return new WrappedStream(input);
    }

  /**
   * Not documented yet.
   * @return Not documented yet.
   */
    public static IWriter ToWriter(OutputStream output) throws java.io.IOException {
      if (output == null) {
        throw new NullPointerException("output");
      }
      return new WrappedOutputStream(output);
    }

  /**
   * Not documented yet.
   * @return Not documented yet.
   */
    public static IWriter ToWriter(IByteWriter output) throws java.io.IOException {
      if (output == null) {
        throw new NullPointerException("output");
      }
      return new WrappedOutputStreamFromByteWriter(output);
    }
  }
