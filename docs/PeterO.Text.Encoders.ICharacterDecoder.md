## PeterO.Text.Encoders.ICharacterDecoder

    public interface ICharacterDecoder

### ReadChar

    int ReadChar(
        PeterO.Mail.ITransform input);

Reads bytes from an input transform until a Unicode character is decoded or until the end of the stream is reached.

<b>Parameters:</b>

 * <i>input</i>: Source of bytes to decode into characters. The decoder can maintain internal state, including data on bytes already read, so this parameter should not change when using the same character decoder object.

<b>Returns:</b>

The Unicode character decoded, from U + 0000 to U + 10FFFF. Returns -1 if the end of the source is reached.
