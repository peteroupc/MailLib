## PeterO.Text.Normalization

    public sealed struct Normalization :
        System.Enum,
        System.IFormattable,
        System.IComparable,
        System.IConvertible

Represents a Unicode normalization form.

### NFC

    public static PeterO.Text.Normalization NFC = 0;

Normalization form C: canonical decomposition followed by canonical composition.

### NFD

    public static PeterO.Text.Normalization NFD = 1;

Normalization form D: canonical decomposition.

### NFKC

    public static PeterO.Text.Normalization NFKC = 2;

Normalization form KC: compatibility decomposition followed by canonical composition.

### NFKD

    public static PeterO.Text.Normalization NFKD = 3;

Normalization form KD: compatibility decomposition.
