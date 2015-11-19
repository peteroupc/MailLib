## PeterO.Text.ICharacterEncoding

    public interface ICharacterEncoding

Defines methods that can be implemented by classes that convert to and from bytes and character code points.

### GetDecoder

    PeterO.Text.ICharacterDecoder GetDecoder();

Creates a decoder for this character encoding with initial state. If the decoder is stateless, multiple calls of this method can return the same decoder.

<b>Returns:</b>

A character decoder object.

### GetEncoder

    PeterO.Text.ICharacterEncoder GetEncoder();

Creates an encoder for this character encoding with initial state. If the encoder is stateless, multiple calls of this method can return the same encoder.

<b>Returns:</b>

A character encoder object.
