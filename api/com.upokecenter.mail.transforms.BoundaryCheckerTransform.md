# com.upokecenter.mail.transforms.BoundaryCheckerTransform

    public final class BoundaryCheckerTransform extends java.lang.Object implements com.upokecenter.util.IByteReader

## Methods

* `BoundaryCheckerTransform​(com.upokecenter.util.IByteReader stream,
                        java.lang.String initialBoundary)`<br>
* `int BoundaryCount()`<br>
* `void EndBodyPartHeaders​(java.lang.String boundary)`<br>
* `boolean getHasNewBodyPart()`<br>
 Gets a value indicating whether a new body part was detected.
* `int read()`<br>
* `void StartBodyPartHeaders()`<br>

## Constructors

* `BoundaryCheckerTransform​(com.upokecenter.util.IByteReader stream,
                        java.lang.String initialBoundary)`<br>

## Method Details

### BoundaryCheckerTransform
    public BoundaryCheckerTransform​(com.upokecenter.util.IByteReader stream, java.lang.String initialBoundary)
### BoundaryCheckerTransform
    public BoundaryCheckerTransform​(com.upokecenter.util.IByteReader stream, java.lang.String initialBoundary)
### read
    public int read()

**Specified by:**

* <code>read</code>&nbsp;in interface&nbsp;<code>com.upokecenter.util.IByteReader</code>

### BoundaryCount
    public int BoundaryCount()
### StartBodyPartHeaders
    public void StartBodyPartHeaders()
### EndBodyPartHeaders
    public void EndBodyPartHeaders​(java.lang.String boundary)
### getHasNewBodyPart
    public final boolean getHasNewBodyPart()
Gets a value indicating whether a new body part was detected.

**Returns:**

* <code>true</code> If a new body part was detected; otherwise, . <code>
 false</code>.
