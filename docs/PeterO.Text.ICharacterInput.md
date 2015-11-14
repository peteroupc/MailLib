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

Either a Unicode code point (from 0-0xd7ff or from 0xe000 to 0x10ffff), or the value -1 indicating the end of the source.
