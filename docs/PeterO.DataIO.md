## PeterO.DataIO

    public static class DataIO

Not documented yet.

### ToTransform

    public static PeterO.ITransform ToTransform(
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

    public static PeterO.ITransform ToTransform(
        this byte[] bytes,
        int offset,
        int length);

Not documented yet.In the .NET implementation, this method is implemented as an extension method to any object implementing byte[] and can be called as follows:  `bytes.ToTransform(offset, length)` . If the object's class already has a ToTransform method with the same parameters, that method takes precedence over this extension method.

<b>Parameters:</b>

 * <i>bytes</i>: Not documented yet.

 * <i>offset</i>: Not documented yet.

 * <i>length</i>: Not documented yet. (3).

<b>Returns:</b>

An ITransform object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 is null.

### ToTransform

    public static PeterO.ITransform ToTransform(
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
