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
    private static final class ByteArrayTransform implements IByteReader {
      private byte[] bytes;
      private int offset;
      private int endOffset;

      public ByteArrayTransform (byte[] bytes, int offset, int length) {
        this.bytes = bytes;
        this.offset = offset;
        this.endOffset = offset + length;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int read() {
        if (this.offset >= this.endOffset) {
          return -1;
        }
        int b = this.bytes[this.offset++];
        return ((int)b) & 0xff;
      }
    }

    private static final class WrappedStream implements IByteReader {
      private InputStream stream;

      public WrappedStream (InputStream stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
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
     * Not documented yet.
     * @param byteValue A 32-bit signed integer.
     */
      public void write(int byteValue) {
        this.output.write((byte)byteValue);
      }

    /**
     * Not documented yet.
     * @param bytes A byte array.
     * @param offset A 32-bit signed integer.
     * @param length Another 32-bit signed integer.
     * @throws NullPointerException The parameter {@code bytes} is null.
     * @throws java.lang.IllegalArgumentException Either "offset" or "length" is less than 0
     * or greater than "bytes"'s length, or "bytes"'s length minus "offset"
     * is less than "length".
     */
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
     * Not documented yet.
     * @param byteValue A 32-bit signed integer.
     */
      public void write(int byteValue) {
        try {
          this.output.write((byte)byteValue);
        } catch (IOException ex) {
          throw new IllegalStateException(ex.getMessage(), ex);
        }
      }

    /**
     * Not documented yet.
     * @param bytes A byte array.
     * @param offset A 32-bit signed integer.
     * @param length Another 32-bit signed integer.
     * @throws java.lang.IllegalArgumentException Either "offset" or "length" is less than 0
     * or greater than "bytes"'s length, or "bytes"'s length minus "offset"
     * is less than "length".
     * @throws java.lang.NullPointerException The parameter "bytes" is null.
     */
      public void write(byte[] bytes, int offset, int length) {
        try {
          this.output.write(bytes, offset, length);
        } catch (IOException ex) {
          throw new IllegalStateException(ex.getMessage(), ex);
        }
      }
    }

    /**
     * Not documented yet. <p>In the .NET implementation, this method is
     * implemented as an extension method to any object implementing byte[]
     * and can be called as follows: <code>bytes.ToTransform()</code>. If the
     * object's class already has a ToTransform method with the same
     * parameters, that method takes precedence over this extension
     * method.</p>
     * @param bytes Not documented yet.
     * @return An ITransform object.
     * @throws NullPointerException The parameter {@code bytes} is null.
     */
    public static IByteReader ToTransform(byte[] bytes) {
      if (bytes == null) {
        throw new NullPointerException("bytes");
      }
      return new ByteArrayTransform(bytes, 0, bytes.length);
    }

    /**
     * Not documented yet. <p>In the .NET implementation, this method is
     * implemented as an extension method to any object implementing byte[]
     * and can be called as follows: <code>bytes.ToTransform(offset,
     * length)</code>. If the object's class already has a ToTransform method
     * with the same parameters, that method takes precedence over this
     * extension method.</p>
     * @param bytes Not documented yet.
     * @param offset Not documented yet.
     * @param length Not documented yet. (3).
     * @return An ITransform object.
     * @throws NullPointerException The parameter {@code bytes} is null.
     * @throws java.lang.IllegalArgumentException Either "offset" or "length" is less than 0
     * or greater than "bytes"'s length, or "bytes"'s length minus "offset"
     * is less than "length".
     */
    public static IByteReader ToTransform(
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
     * Not documented yet. <p>In the .NET implementation, this method is
     * implemented as an extension method to any object implementing InputStream
     * and can be called as follows: <code>input.ToTransform()</code>. If the
     * object's class already has a ToTransform method with the same
     * parameters, that method takes precedence over this extension
     * method.</p>
     * @param input Not documented yet.
     * @return An ITransform object.
     * @throws NullPointerException The parameter {@code input} is null.
     */
    public static IByteReader ToTransform(InputStream input) {
      if (input == null) {
        throw new NullPointerException("input");
      }
      return new WrappedStream(input);
    }

    /**
     * Not documented yet. <p>In the .NET implementation, this method is
     * implemented as an extension method to any object implementing InputStream
     * and can be called as follows: <code>output.ToWriter()</code>. If the
     * object's class already has a ToWriter method with the same
     * parameters, that method takes precedence over this extension
     * method.</p>
     * @param output Not documented yet.
     * @return An IWriter object.
     * @throws NullPointerException The parameter {@code output} is null.
     */
    public static IWriter ToWriter(OutputStream output) throws java.io.IOException {
      if (output == null) {
        throw new NullPointerException("output");
      }
      return new WrappedOutputStream(output);
    }

    /**
     * Not documented yet. <p>In the .NET implementation, this method is
     * implemented as an extension method to any object implementing
     * IByteWriter and can be called as follows: <code>output.ToWriter()</code>.
     * If the object's class already has a ToWriter method with the same
     * parameters, that method takes precedence over this extension
     * method.</p>
     * @param output Not documented yet.
     * @return An IWriter object.
     * @throws NullPointerException The parameter {@code output} is null.
     */
    public static IWriter ToWriter(IByteWriter output) throws java.io.IOException {
      if (output == null) {
        throw new NullPointerException("output");
      }
      return new WrappedOutputStreamFromByteWriter(output);
    }
  }
