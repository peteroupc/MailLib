## PeterO.ITransform

    public interface ITransform

A generic interface for reading data one byte at a time.

### ReadByte

    int ReadByte();

Reads a byte from the data source.

<b>Returns:</b>

The byte read, or -1 if the end of the source is reached.
