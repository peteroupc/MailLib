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

Writes a series of bytes to the array.

<b>Parameters:</b>

 * <i>src</i>: Byte array containing the data to write.

 * <i>offset</i>: A zero-based index showing where the desired portion of  <i>src</i>
 begins.

 * <i>length</i>: The number of elements in the desired portion of  <i>src</i>
 (but not more than  <i>src</i>
's length).

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>src</i>
 is null.

 * System.ArgumentException:
Either  <i>offset</i>
 or  <i>length</i>
 is less than 0 or greater than  <i>src</i>
 's length, or  <i>src</i>
 's length minus  <i>offset</i>
 is less than <i>length</i>
.

### WriteByte

    public sealed void WriteByte(
        int byteValue);

Writes an 8-bit byte to the array.

<b>Parameters:</b>

 * <i>byteValue</i>: A 32-bit signed integer.
