## PeterO.Text.Encoders.ICharacterEncoding

    public interface ICharacterEncoding

### GetDecoder

    PeterO.Text.Encoders.ICharacterDecoder GetDecoder();

Creates a decoder for this character encoding with initial state. If the decoder is stateless, multiple calls of this method can return the same decoder.

<b>Returns:</b>

Not documented yet.

### GetEncoder

    PeterO.Text.Encoders.ICharacterEncoder GetEncoder();

Creates an encoder for this character encoding with initial state. If the encoder is stateless, multiple calls of this method can return the same encoder.

<b>Returns:</b>

Not documented yet.
