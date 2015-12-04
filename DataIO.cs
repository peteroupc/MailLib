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
    /// <summary>Convenience class that contains static methods for
    /// wrapping byte arrays and streams into byte readers and byte
    /// writers.</summary>
  public static class DataIO {
    /// <summary>Wraps a byte array into a byte reader. The reader will
    /// start at the beginning of the byte array.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any byte array object and can be called as
    /// follows: <c>bytes.ToByteReader()</c>. If the object's class
    /// already has a ToByteReader method with the same parameters, that
    /// method takes precedence over this extension
    /// method.</para></summary>
    /// <param name='bytes'>The byte array to wrap.</param>
    /// <returns>A byte reader wrapping the byte array.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    public static IByteReader ToByteReader(this byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      return new ByteArrayTransform(bytes, 0, bytes.Length);
    }

    /// <summary>Wraps a portion of a byte array into a byte reader object.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any byte array object and can be called as
    /// follows: <c>bytes.ToByteReader(offset, length)</c>. If the
    /// object's class already has a ToByteReader method with the same
    /// parameters, that method takes precedence over this extension
    /// method.</para></summary>
    /// <param name='bytes'>The byte array to wrap.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of "bytes" begins.</param>
    /// <param name='length'>The length, in bytes, of the desired portion
    /// of "bytes" (but not more than "bytes" 's length).</param>
    /// <returns>A byte reader wrapping the byte array.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='length'/>.</exception>
    public static IByteReader ToByteReader(
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

    /// <summary>Wraps an input stream into a reader object. If an
    /// IOException is thrown by the input stream, the reader object throws
    /// InvalidOperationException instead.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any object implementing Stream and can be
    /// called as follows: <c>input.ToByteReader()</c>. If the object's
    /// class already has a ToByteReader method with the same parameters,
    /// that method takes precedence over this extension
    /// method.</para></summary>
    /// <param name='input'>The input stream to wrap.</param>
    /// <returns>A byte reader wrapping the input stream.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='input'/> is null.</exception>
    public static IByteReader ToByteReader(this Stream input) {
      if (input == null) {
        throw new ArgumentNullException("input");
      }
      return new WrappedStream(input);
    }

    /// <summary>Wraps a byte array into a byte reader. The reader will
    /// start at the beginning of the byte array.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any byte array object and can be called as
    /// follows: <c>bytes.ToTransform()</c>. If the object's class already
    /// has a ToTransform method with the same parameters, that method
    /// takes precedence over this extension method.</para></summary>
    /// <param name='bytes'>The byte array to wrap.</param>
    /// <returns>A byte reader wrapping the byte array.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    [Obsolete("Renamed to ToByteReader.")]
    public static IByteReader ToTransform(this byte[] bytes) {
      if (bytes == null) {
        throw new ArgumentNullException("bytes");
      }
      return new ByteArrayTransform(bytes, 0, bytes.Length);
    }

    /// <summary>Wraps a portion of a byte array into a byte reader object.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any byte array object and can be called as
    /// follows: <c>bytes.ToTransform(offset, length)</c>. If the object's
    /// class already has a ToTransform method with the same parameters,
    /// that method takes precedence over this extension
    /// method.</para></summary>
    /// <param name='bytes'>The byte array to wrap.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of "bytes" begins.</param>
    /// <param name='length'>The length, in bytes, of the desired portion
    /// of "bytes" (but not more than "bytes" 's length).</param>
    /// <returns>A byte reader wrapping the byte array.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='length'/>.</exception>
    [Obsolete("Renamed to ToByteReader.")]
    public static IByteReader ToTransform(
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

    /// <summary>Wraps an input stream into a reader object. If an
    /// IOException is thrown by the input stream, the reader object throws
    /// InvalidOperationException instead.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any object implementing Stream and can be
    /// called as follows: <c>input.ToTransform()</c>. If the object's
    /// class already has a ToTransform method with the same parameters,
    /// that method takes precedence over this extension
    /// method.</para></summary>
    /// <param name='input'>The input stream to wrap.</param>
    /// <returns>A byte reader wrapping the input stream.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='input'/> is null.</exception>
    [Obsolete("Renamed to ToByteReader.")]
    public static IByteReader ToTransform(this Stream input) {
      if (input == null) {
        throw new ArgumentNullException("input");
      }
      return new WrappedStream(input);
    }

    /// <summary>Wraps an output stream into a writer object. If an
    /// IOException is thrown by the input stream, the writer object throws
    /// InvalidOperationException instead.
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any object implementing Stream and can be
    /// called as follows: <c>output.ToWriter()</c>. If the object's class
    /// already has a ToWriter method with the same parameters, that method
    /// takes precedence over this extension method.</para></summary>
    /// <param name='output'>Output stream to wrap.</param>
    /// <returns>A byte writer that wraps the given output
    /// stream.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='output'/> is null.</exception>
    public static IWriter ToWriter(this Stream output) {
      if (output == null) {
        throw new ArgumentNullException("output");
      }
      return new WrappedOutputStream(output);
    }

    /// <summary>Wraps a byte writer (one that only implements a ReadByte
    /// method) to a writer (one that also implements a three-parameter
    /// Read method.)
    /// <para>In the .NET implementation, this method is implemented as an
    /// extension method to any object implementing IByteWriter and can be
    /// called as follows: <c>output.ToWriter()</c>. If the object's class
    /// already has a ToWriter method with the same parameters, that method
    /// takes precedence over this extension method.</para></summary>
    /// <param name='output'>A byte stream.</param>
    /// <returns>A writer that wraps the given stream.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='output'/> is null.</exception>
    public static IWriter ToWriter(this IByteWriter output) {
      if (output == null) {
        throw new ArgumentNullException("output");
      }
      return new WrappedOutputStreamFromByteWriter(output);
    }

    private sealed class ByteArrayTransform : IByteReader {
      private readonly byte[] bytes;
      private int offset;
      private readonly int endOffset;

      public ByteArrayTransform(byte[] bytes, int offset, int length) {
        this.bytes = bytes;
        this.offset = offset;
        this.endOffset = offset + length;
      }

    /// <summary>This is an internal method.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.offset >= this.endOffset) {
          return -1;
        }
        int b = this.bytes[this.offset];
        ++this.offset;
        return ((int)b) & 0xff;
      }
    }

    private sealed class WrappedOutputStream : IWriter {
      private readonly Stream output;

      public WrappedOutputStream(Stream output) {
        this.output = output;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='byteValue'>A 32-bit signed integer.</param>
      public void WriteByte(int byteValue) {
        try {
          this.output.WriteByte((byte)byteValue);
        } catch (IOException ex) {
          throw new InvalidOperationException(ex.Message, ex);
        }
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of "bytes" begins.</param>
    /// <param name='length'>The length, in bytes, of the desired portion
    /// of "bytes" (but not more than "bytes" 's length).</param>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='length'/>.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
      public void Write(byte[] bytes, int offset, int length) {
        try {
          this.output.Write(bytes, offset, length);
        } catch (IOException ex) {
          throw new InvalidOperationException(ex.Message, ex);
        }
      }
    }

    private sealed class WrappedOutputStreamFromByteWriter : IWriter {
      private readonly IByteWriter output;

      public WrappedOutputStreamFromByteWriter(IByteWriter output) {
        this.output = output;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='byteValue'>A 32-bit signed integer.</param>
      public void WriteByte(int byteValue) {
        this.output.WriteByte((byte)byteValue);
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='bytes'>A byte array.</param>
    /// <param name='offset'>A zero-based index showing where the desired
    /// portion of "bytes" begins.</param>
    /// <param name='length'>The length, in bytes, of the desired portion
    /// of "bytes" (but not more than "bytes" 's length).</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='length'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='length'/>.</exception>
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

    private sealed class WrappedStream : IByteReader {
      private readonly Stream stream;

      public WrappedStream(Stream stream) {
        this.stream = stream;
      }

    /// <summary>This is an internal method.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        try {
          return this.stream.ReadByte();
        } catch (IOException ex) {
          throw new InvalidOperationException(ex.Message, ex);
        }
      }
    }
  }
}
