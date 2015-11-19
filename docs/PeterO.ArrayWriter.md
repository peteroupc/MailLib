## PeterO.ArrayWriter

    public sealed class ArrayWriter :
        PeterO.IWriter,
        PeterO.IByteWriter

A growable array of bytes.

### ArrayWriter Constructor

    public ArrayWriter(
        int initialSize);

Initializes a new instance of the ArrayWriter class.

<b>Parameters:</b>

 * <i>initialSize</i>: A 32-bit signed integer.

### ArrayWriter Constructor

    public ArrayWriter();

Initializes a new instance of the ArrayWriter class.

### ToArray

    public byte[] ToArray();

Generates an array of all bytes written so far to it.

<b>Returns:</b>

A byte array.

### Write

    public sealed void Write(
        byte[] src,
        int offset,
        int length);

Not documented yet.

<b>Parameters:</b>

 * <i>src</i>: Byte array containing the data to write.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: Another 32-bit signed integer.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>src</i>
 is null.

 * System.ArgumentException:
Either "offset" or "length" is less than 0 or greater than "src"'s length, or "src"'s length minus "offset" is less than "length".

### WriteByte

    public sealed void WriteByte(
        int byteValue);

Not documented yet.

<b>Parameters:</b>

 * <i>byteValue</i>: A 32-bit signed integer.
