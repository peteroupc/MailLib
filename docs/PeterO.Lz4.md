## PeterO.Lz4

    public static class Lz4

Implements the LZ4 algorithm (see "LZ4 Format Description" by Y. Collet for more information).

### Decompress

    public static byte[] Decompress(
        byte[] input);

Decompresses a byte array compressed using the LZ4 format.

<b>Parameters:</b>

 * <i>input</i>: Input byte array.

<b>Returns:</b>

Decompressed output byte array.


