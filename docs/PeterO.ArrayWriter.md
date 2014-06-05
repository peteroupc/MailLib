## PeterO.ArrayWriter

    public sealed class ArrayWriter

A lightweight version of MemoryStream, since it doesn't derive from Stream and doesn't use IO exceptions.

### ArrayWriter Constructor

    public ArrayWriter();

Initializes a new instance of the ArrayWriter class.

### ArrayWriter Constructor

    public ArrayWriter(
        int initialSize);

Initializes a new instance of the ArrayWriter class.

<b>Parameters:</b>

 * <i>initialSize</i>: A 32-bit signed integer.

### SetLength

    public void SetLength(
        int length);

Not documented yet.

<b>Parameters:</b>

 * <i>length</i>: A 32-bit signed integer.

### ToArray

    public byte[] ToArray();

Not documented yet.

<b>Returns:</b>

A byte array.

### ReadBytes

    public int ReadBytes(
        byte[] src,
        int offset,
        int length);

Not documented yet.

<b>Parameters:</b>

 * <i>src</i>: A byte array.

 * <i>offset</i>: A 32-bit signed integer. (2).

 * <i>length</i>: A 32-bit signed integer. (3).

<b>Returns:</b>

A 32-bit signed integer.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>src</i>
 is null.

### WriteBytes

    public void WriteBytes(
        byte[] src,
        int offset,
        int length);

Not documented yet.

<b>Parameters:</b>

 * <i>src</i>: A byte array.

 * <i>offset</i>: A 32-bit signed integer.

 * <i>length</i>: A 32-bit signed integer. (2).

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>src</i>
 is null.

### Position

    public int Position { get; set;}

Gets or sets a value not documented yet.

<b>Returns:</b>

A value not documented yet.


