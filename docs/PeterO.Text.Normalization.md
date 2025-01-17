## PeterO.Text.Normalization

    public sealed struct Normalization :
        System.Enum,
        System.IComparable,
        System.IConvertible,
        System.IFormattable,
        System.ISpanFormattable

Represents a Unicode normalization form.

### Member Summary
* <code>[public static PeterO.Text.Normalization NFC = 0;](#NFC)</code> - Normalization form C: canonical decomposition followed by canonical composition.
* <code>[public static PeterO.Text.Normalization NFD = 1;](#NFD)</code> - Normalization form D: canonical decomposition.
* <code>[public static PeterO.Text.Normalization NFKC = 2;](#NFKC)</code> - Normalization form KC: compatibility decomposition followed by canonical composition.
* <code>[public static PeterO.Text.Normalization NFKD = 3;](#NFKD)</code> - Normalization form KD: compatibility decomposition.

<a id="NFC"></a>
### NFC

    public static PeterO.Text.Normalization NFC = 0;

Normalization form C: canonical decomposition followed by canonical composition.

<a id="NFD"></a>
### NFD

    public static PeterO.Text.Normalization NFD = 1;

Normalization form D: canonical decomposition.

<a id="NFKC"></a>
### NFKC

    public static PeterO.Text.Normalization NFKC = 2;

Normalization form KC: compatibility decomposition followed by canonical composition.

<a id="NFKD"></a>
### NFKD

    public static PeterO.Text.Normalization NFKD = 3;

Normalization form KD: compatibility decomposition.
