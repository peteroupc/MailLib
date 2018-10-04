# com.upokecenter.mail.transforms.Base64Transform

    public final class Base64Transform extends java.lang.Object implements com.upokecenter.util.IByteReader

## Fields

* `static int[] Alphabet`<br>
* `static int MaxLineLength`<br>

## Constructors

* `Base64Transform​(com.upokecenter.util.IByteReader input,
               boolean lenientLineBreaks)`<br>
* `Base64Transform​(com.upokecenter.util.IByteReader input,
               boolean lenientLineBreaks,
               int maxLineLength,
               boolean checkStrictEncoding)`<br>

## Methods

* `int read()`<br>

## Field Details

### Alphabet
    public static final int[] Alphabet
### MaxLineLength
    public static final int MaxLineLength
## Method Details

### read
    public int read()

**Specified by:**

* <code>read</code>&nbsp;in interface&nbsp;<code>com.upokecenter.util.IByteReader</code>
