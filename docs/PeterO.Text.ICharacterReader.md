## PeterO.Text.ICharacterReader

    public interface ICharacterReader

An interface for reading Unicode characters from a data source.

### ReadChar

    int ReadChar();

Reads a Unicode character from a data source.

<b>Returns:</b>

The Unicode character read, from U + 0000 to U + 10FFFF. Returns -1 if the end of the source is reached.
