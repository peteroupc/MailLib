# com.upokecenter.mail.StringAndQuality

    public final class StringAndQuality extends Object

Stores an arbitrary string and a "quality value" for that string. For
 instance, the string can be a language tag, and the "quality value"
 can be the degree of preference for that language.

## Methods

* `StringAndQuality​(String value,
                int quality) StringAndQuality`<br>
 Initializes a new instance of the StringAndQuality class.
* `int getQuality()`<br>
 Gets the quality value stored by this object.
* `String getValue()`<br>
 Gets the arbitrary string stored by this object.

## Constructors

* `StringAndQuality​(String value,
                int quality) StringAndQuality`<br>
 Initializes a new instance of the StringAndQuality class.

## Method Details

### StringAndQuality
    public StringAndQuality​(String value, int quality)
Initializes a new instance of the <code>StringAndQuality</code> class.

**Parameters:**

* <code>value</code> - An arbitrary text string.

* <code>quality</code> - A 32-bit signed integer serving as the "quality" value.

### StringAndQuality
    public StringAndQuality​(String value, int quality)
Initializes a new instance of the <code>StringAndQuality</code> class.

**Parameters:**

* <code>value</code> - An arbitrary text string.

* <code>quality</code> - A 32-bit signed integer serving as the "quality" value.

### getValue
    public final String getValue()
Gets the arbitrary string stored by this object.

**Returns:**

* The arbitrary string stored by this object.

### getQuality
    public final int getQuality()
Gets the quality value stored by this object.

**Returns:**

* The quality value stored by this object.
