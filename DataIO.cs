/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;

namespace PeterO {
  /// <summary>Not documented yet.</summary>
  public static class DataIO {
    private sealed class ByteArrayTransform : ITransform {
      private byte[] bytes;
      private int offset;
      private int endOffset;

      public ByteArrayTransform(byte[] bytes, int offset, int length) {
        this.bytes = bytes;
        this.offset = offset;
        this.endOffset = offset + length;
      }

      /// <summary>
/// </summary>
/// <returns></returns>
public int ReadByte() {
        if (this.offset >= this.endOffset) {
          return -1;
        }
        int b = this.bytes[this.offset++];
        return ((int)b) & 0xff;
      }
    }

    private sealed class WrappedStream : ITransform {
      private Stream stream;

      public WrappedStream(Stream stream) {
        this.stream = stream;
      }

      /// <summary>
/// </summary>
/// <returns></returns>
public int ReadByte() {
        try {
          return this.stream.ReadByte();
        } catch (IOException ex) {
          throw new InvalidOperationException(ex.Message, ex);
        }
      }
    }

    private sealed class WrappedOutputStreamFromByteWriter : IWriter {
      private IByteWriter output;

      public WrappedOutputStreamFromByteWriter(IByteWriter output) {
        this.output = output;
      }

      /// <summary>
/// </summary>
/// <param name="byteValue"></param>
/// <returns></returns>
      public void WriteByte(int byteValue) {
          this.output.WriteByte((byte)byteValue);
      }

      /// <summary>
/// </summary>
/// <param name="bytes"></param>
/// <param name="offset"></param>
/// <param name="length"></param>
/// <returns></returns>
public void Write(byte[] bytes, int offset, int length) {
        if (bytes == null) {
          throw new ArgumentNullException("bytes");
        }
        if (offset < 0) {
          throw new ArgumentException("offset (" + offset +
            ") is less than " + 0);
        }
        if (offset > bytes.Length) {
          throw new ArgumentException("offset (" + offset + ") is more than " +
            bytes.Length);
        }
        if (length < 0) {
          throw new ArgumentException("length (" + length +
            ") is less than " + 0);
        }
        if (length > bytes.Length) {
          throw new ArgumentException("length (" + length + ") is more than " +
            bytes.Length);
        }
        if (bytes.Length - offset < length) {
          throw new ArgumentException("bytes's length minus " + offset + " (" +
            (bytes.Length - offset) + ") is less than " + length);
        }
        for (int i = 0; i < length; ++i) {
          this.output.WriteByte((byte)bytes[i]);
        }
      }
    }

    private sealed class WrappedOutputStream : IWriter {
      private Stream output;

      public WrappedOutputStream(Stream output) {
        this.output = output;
      }

      /// <summary>
/// </summary>
/// <param name="byteValue"></param>
/// <returns></returns>
public void WriteByte(int byteValue) {
        try {
          this.output.WriteByte((byte)byteValue);
        } catch (IOException ex) {
          throw new InvalidOperationException(ex.Message, ex);
        }
      }

      /// <summary>
/// </summary>
/// <param name="bytes"></param>
/// <param name="offset"></param>
/// <param name="length"></param>
/// <returns></returns>
public void Write(byte[] bytes, int offset, int length) {
        try {
          this.output.Write(bytes, offset, length);
        } catch (IOException ex) {
          throw new InvalidOperationException(ex.Message, ex);
        }
      }
    }

  /// <summary>Not documented yet.</summary>
  /// <returns>Not documented yet.</returns>
    public static ITransform ToTransform(this byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      return new ByteArrayTransform(bytes, 0, bytes.Length);
    }

  /// <summary>Not documented yet.</summary>
  /// <returns>Not documented yet.</returns>
    public static ITransform ToTransform(
this byte[] bytes,
int offset,
int length) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + offset +
          ") is less than " + 0);
      }
      if (offset > bytes.Length) {
        throw new ArgumentException("offset (" + offset + ") is more than " +
          bytes.Length);
      }
      if (length < 0) {
        throw new ArgumentException("length (" + length +
          ") is less than " + 0);
      }
      if (length > bytes.Length) {
        throw new ArgumentException("length (" + length + ") is more than " +
          bytes.Length);
      }
      if (bytes.Length - offset < length) {
        throw new ArgumentException("bytes's length minus " + offset + " (" +
          (bytes.Length - offset) + ") is less than " + length);
      }
      return new ByteArrayTransform(bytes, offset, length);
    }

  /// <summary>Not documented yet.</summary>
  /// <returns>Not documented yet.</returns>
    public static ITransform ToTransform(this Stream input) {
      if (input == null) {
        throw new ArgumentNullException("input");
      }
      return new WrappedStream(input);
    }

  /// <summary>Not documented yet.</summary>
  /// <returns>Not documented yet.</returns>
    public static IWriter ToWriter(this Stream output) {
      if (output == null) {
        throw new ArgumentNullException("output");
      }
      return new WrappedOutputStream(output);
    }

  /// <summary>Not documented yet.</summary>
  /// <returns>Not documented yet.</returns>
    public static IWriter ToWriter(this IByteWriter output) {
      if (output == null) {
        throw new ArgumentNullException("output");
      }
      return new WrappedOutputStreamFromByteWriter(output);
    }
  }
}
