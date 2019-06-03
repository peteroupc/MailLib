# com.upokecenter.mail.StringAndQuality

    public final class StringAndQuality extends java.lang.Object

Stores an arbitrary string and a "quality value" for that string. For
 instance, the string can be a language tag, and the "quality value"
 can be the degree of preference for that language.

## Methods

* `StringAndQuality​(java.lang.String value,
                int quality) StringAndQuality`<br>
 Initializes a new instance of the StringAndQuality class.
* `int getQuality()`<br>
 Gets the quality value stored by this object.
* `java.lang.String getValue()`<br>
 Gets the arbitrary string stored by this object.

## Constructors

* `StringAndQuality​(java.lang.String value,
                int quality) StringAndQuality`<br>
 Initializes a new instance of the StringAndQuality class.

## Method Details

### StringAndQuality
    public StringAndQuality​(java.lang.String value, int quality)
Initializes a new instance of the <code>StringAndQuality</code> class.

**Parameters:**

* <code>value</code> - The parameter <code>value</code> is a text string.

* <code>quality</code> - The parameter <code>quality</code> is a 32-bit signed integer.

### StringAndQuality
    public StringAndQuality​(java.lang.String value, int quality)
Initializes a new instance of the <code>StringAndQuality</code> class.

**Parameters:**

* <code>value</code> - The parameter <code>value</code> is a text string.

* <code>quality</code> - The parameter <code>quality</code> is a 32-bit signed integer.

### getValue
    public final java.lang.String getValue()
Gets the arbitrary string stored by this object.

**Returns:**

* The arbitrary string stored by this object.

### getQuality
    public final int getQuality()
Gets the quality value stored by this object.

**Returns:**

* The quality value stored by this object.
