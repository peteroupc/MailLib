## PeterO.DataIO

    public static class DataIO

Contains static methods for converting byte arrays and data streams to byte readers and byte writers.

### ToTransform

    public static PeterO.IByteReader ToTransform(
        this byte[] bytes);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing byte[] and can be called as follows:  `bytes.ToTransform()` . If the object's class already has a ToTransform method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>bytes</i>: Not documented yet.

<b>Returns:</b>

An ITransform object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

### ToTransform

    public static PeterO.IByteReader ToTransform(
        this byte[] bytes,
        int offset,
        int length);

Wraps a portion of a byte array into a byte reader.In the .NET implementation, this method is implemented as an extension method to any object implementing byte[] and can be called as follows:  `bytes.ToTransform(offset, length)` . If the object's class already has a ToTransform method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array.

 * <i>offset</i>: Zero-based index to the start of the portion of the byte array to read.

 * <i>length</i>: Length of the portion of the byte array to read.

<b>Returns:</b>

An ITransform object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

 * System.ArgumentException:
Either "offset" or "length" is less than 0 or greater than "bytes"'s length, or "bytes"'s length minus "offset" is less than "length".

### ToTransform

    public static PeterO.IByteReader ToTransform(
        this System.IO.Stream input);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing Stream and can be called as follows:  `input.ToTransform()` . If the object's class already has a ToTransform method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>input</i>: Not documented yet.

<b>Returns:</b>

An ITransform object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>input</i>
 is null.

### ToWriter

    public static PeterO.IWriter ToWriter(
        this PeterO.IByteWriter output);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing IByteWriter and can be called as follows:  `output.ToWriter()` . If the object's class already has a ToWriter method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>output</i>: Not documented yet.

<b>Returns:</b>

An IWriter object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>output</i>
 is null.

### ToWriter

    public static PeterO.IWriter ToWriter(
        this System.IO.Stream output);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing Stream and can be called as follows:  `output.ToWriter()` . If the object's class already has a ToWriter method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>output</i>: Not documented yet.

<b>Returns:</b>

An IWriter object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>output</i>
 is null.
