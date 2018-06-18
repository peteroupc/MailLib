# com.upokecenter.text.Normalization

    public enum Normalization extends Enum<Normalization>

Represents a Unicode normalization form.

## Enum Constants

* `NFC`<br>
 Normalization form C: canonical decomposition followed by canonical
 composition.
* `NFD`<br>
 Normalization form D: canonical decomposition.
* `NFKC`<br>
 Normalization form KC: compatibility decomposition followed by canonical
 composition.
* `NFKD`<br>
 Normalization form KD: compatibility decomposition.

## Methods

* `static Normalization valueOf​(String name)`<br>
 Returns the enum constant of this type with the specified name.
* `static Normalization[] values()`<br>
 Returns an array containing the constants of this enum type, in
the order they are declared.

## Method Details

### NFC
    public static final Normalization NFC
### NFD
    public static final Normalization NFD
### NFKC
    public static final Normalization NFKC
### NFKD
    public static final Normalization NFKD
### values
    public static Normalization[] values()
### valueOf
    public static Normalization valueOf​(String name)
## Enum Constant Details

### NFC
    public static final Normalization NFC
Normalization form C: canonical decomposition followed by canonical
 composition.
### NFD
    public static final Normalization NFD
Normalization form D: canonical decomposition.
### NFKC
    public static final Normalization NFKC
Normalization form KC: compatibility decomposition followed by canonical
 composition.
### NFKD
    public static final Normalization NFKD
Normalization form KD: compatibility decomposition.
