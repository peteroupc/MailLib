## PeterO.Text.ICharacterInput

    public interface ICharacterInput

An interface for reading Unicode characters from a data source.

### Read

    int Read(
        int[] chars,
        int index,
        int length);

Reads a sequence of Unicode code points from a data source.

<b>Parameters:</b>

 * <i>chars</i>: Output buffer.

 * <i>index</i>: Index in the output buffer to start writing to.

 * <i>length</i>: Maximum number of code points to write.

<b>Returns:</b>

The number of Unicode code points read, or 0 if the end of the source is reached.

### ReadChar

    int ReadChar();

Reads a Unicode character from a data source.

<b>Returns:</b>

The Unicode character read, from U + 0000 to U + 10FFFF. Returns -1 if the end of the source is reached.
