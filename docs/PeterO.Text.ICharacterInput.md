## PeterO.Text.ICharacterInput

    public abstract interface ICharacterInput

An interface for reading Unicode characters from a data source.

### ReadChar

    public abstract virtual int ReadChar();

Reads a Unicode character from a data source.

<b>Returns:</b>

The Unicode character read, from U + 0000 to U + 10FFFF. Returns -1 if the end of the source is reached.

### Read

    public abstract virtual int Read(
        int[] chars,
        int index,
        int length);

Reads a sequence of Unicode code points from a data source.

<b>Returns:</b>

The number of Unicode code points read, or 0 if the end of the source is reached.


