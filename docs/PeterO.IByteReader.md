## PeterO.IByteReader

    public interface IByteReader

A generic interface for reading data one byte at a time.

### ReadByte

    int ReadByte();

Reads a byte from the data source.

<b>Returns:</b>

The byte read (from 0 through 255), or -1 if the end of the source is reached.
