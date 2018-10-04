# com.upokecenter.mail.transforms.QuotedPrintableTransform

    public final class QuotedPrintableTransform extends java.lang.Object implements com.upokecenter.util.IByteReader

## Fields

* `static int MaxLineLength`<br>

## Constructors

* `QuotedPrintableTransform​(com.upokecenter.util.IByteReader input,
                        boolean allowBareLfCr)`<br>
* `QuotedPrintableTransform​(com.upokecenter.util.IByteReader input,
                        boolean allowBareLfCr,
                        int maxLineLength)`<br>
* `QuotedPrintableTransform​(com.upokecenter.util.IByteReader input,
                        boolean allowBareLfCr,
                        int maxLineSize,
                        boolean checkStrictEncoding)`<br>

## Methods

* `int read()`<br>

## Field Details

### MaxLineLength
    public static final int MaxLineLength
## Method Details

### read
    public int read()

**Specified by:**

* <code>read</code>&nbsp;in interface&nbsp;<code>com.upokecenter.util.IByteReader</code>
