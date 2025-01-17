# com.upokecenter.text.Normalization

    public enum Normalization extends Enum<Normalization>

Represents a Unicode normalization form.

## Nested Classes

## Enum Constants

* `NFC `<br>
 Normalization form C: canonical decomposition followed by canonical
 composition.

* `NFD `<br>
 Normalization form D: canonical decomposition.

* `NFKC `<br>
 Normalization form KC: compatibility decomposition followed by canonical
 composition.

* `NFKD `<br>
 Normalization form KD: compatibility decomposition.

## Methods

* `static Normalization valueOf(String name)`<br>
 Returns the enum constant of this class with the specified name.

* `static Normalization[] values()`<br>
 Returns an array containing the constants of this enum class, in
the order they are declared.

## Method Details

### values

    public static Normalization[] values()

### valueOf

    public static Normalization valueOf(String name)
