## PeterO.DataIO

    public static class DataIO

Convenience class that contains static methods for wrapping byte arrays and streams into byte readers and byte writers.

### ToByteReader

    public static PeterO.IByteReader ToByteReader(
        this byte[] bytes);

Wraps a byte array into a byte reader. The reader will start at the beginning of the byte array.In the .NET implementation, this method is implemented as an extension method to any object implementing byte[] and can be called as follows:  `bytes.ToByteReader()` . If the object's class already has a ToByteReader method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>bytes</i>: The byte array to wrap.

<b>Returns:</b>

A byte reader wrapping the byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

### ToByteReader

    public static PeterO.IByteReader ToByteReader(
        this byte[] bytes,
        int offset,
        int length);

Wraps a portion of a byte array into a byte reader object.In the .NET implementation, this method is implemented as an extension method to any object implementing byte[] and can be called as follows:  `bytes.ToByteReader(offset, length)` . If the object's class already has a ToByteReader method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>bytes</i>: The byte array to wrap.

 * <i>offset</i>: A zero-based index showing where the desired portion of "bytes" begins.

 * <i>length</i>: The length, in bytes, of the desired portion of "bytes" (but not more than "bytes" 's length).

<b>Returns:</b>

A byte reader wrapping the byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>bytes</i>
 's length, or  <i>bytes</i>
 's length minus  <i>offset</i>
 is less than  <i>length</i>
.

### ToByteReader

    public static PeterO.IByteReader ToByteReader(
        this System.IO.Stream input);

Wraps an input stream into a reader object. If an IOException is thrown by the input stream, the reader object throws InvalidOperationException instead.In the .NET implementation, this method is implemented as an extension method to any object implementing Stream and can be called as follows:  `input.ToByteReader()` . If the object's class already has a ToByteReader method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: The input stream to wrap.

<b>Returns:</b>

A byte reader wrapping the input stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>input</i>
 is null.

### ToTransform

    public static PeterO.IByteReader ToTransform(
        this byte[] bytes);

<b>Deprecated.</b> Renamed to ToByteReader.

Wraps a byte array into a byte reader. The reader will start at the beginning of the byte array.In the .NET implementation, this method is implemented as an extension method to any object implementing byte[] and can be called as follows:  `bytes.ToTransform()` . If the object's class already has a ToTransform method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>bytes</i>: The byte array to wrap.

<b>Returns:</b>

A byte reader wrapping the byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

### ToTransform

    public static PeterO.IByteReader ToTransform(
        this byte[] bytes,
        int offset,
        int length);

<b>Deprecated.</b> Renamed to ToByteReader.

Wraps a portion of a byte array into a byte reader object.In the .NET implementation, this method is implemented as an extension method to any object implementing byte[] and can be called as follows:  `bytes.ToTransform(offset, length)` . If the object's class already has a ToTransform method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>bytes</i>: The byte array to wrap.

 * <i>offset</i>: A zero-based index showing where the desired portion of "bytes" begins.

 * <i>length</i>: The length, in bytes, of the desired portion of "bytes" (but not more than "bytes" 's length).

<b>Returns:</b>

A byte reader wrapping the byte array.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>bytes</i>
 's length, or  <i>bytes</i>
 's length minus  <i>offset</i>
 is less than  <i>length</i>
.

### ToTransform

    public static PeterO.IByteReader ToTransform(
        this System.IO.Stream input);

<b>Deprecated.</b> Renamed to ToByteReader.

Wraps an input stream into a reader object. If an IOException is thrown by the input stream, the reader object throws InvalidOperationException instead.In the .NET implementation, this method is implemented as an extension method to any object implementing Stream and can be called as follows:  `input.ToTransform()` . If the object's class already has a ToTransform method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: The input stream to wrap.

<b>Returns:</b>

A byte reader wrapping the input stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>input</i>
 is null.

### ToWriter

    public static PeterO.IWriter ToWriter(
        this PeterO.IByteWriter output);

Wraps a byte writer (one that only implements a ReadByte method) to a writer (one that also implements a three-parameter Read method.)In the .NET implementation, this method is implemented as an extension method to any object implementing IByteWriter and can be called as follows:  `output.ToWriter()` . If the object's class already has a ToWriter method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>output</i>: A byte stream.

<b>Returns:</b>

A writer that wraps the given stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>output</i>
 is null.

### ToWriter

    public static PeterO.IWriter ToWriter(
        this System.IO.Stream output);

Wraps an output stream into a writer object. If an IOException is thrown by the input stream, the writer object throws InvalidOperationException instead.In the .NET implementation, this method is implemented as an extension method to any object implementing Stream and can be called as follows:  `output.ToWriter()` . If the object's class already has a ToWriter method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>output</i>: Output stream to wrap.

<b>Returns:</b>

A byte writer that wraps the given output stream.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>output</i>
 is null.
