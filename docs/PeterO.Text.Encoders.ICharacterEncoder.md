## PeterO.Text.Encoders.ICharacterEncoder

    public interface ICharacterEncoder

### Encode

    int Encode(
        int c,
        System.IO.Stream output);

Converts a Unicode code point to bytes and writes them to an output stream.

<b>Returns:</b>

The number of bytes written to the stream; -1 if no further code points remain (for example, if _c_ is -1 indicating the end of the stream), or -2 if an encoding error occurs.
