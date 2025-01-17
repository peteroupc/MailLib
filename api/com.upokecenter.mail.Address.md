# com.upokecenter.mail.Address

    public class Address extends Object

Represents an email address.

## Constructors

## Methods

* `boolean equals(Object obj)`<br>
 Determines whether this object and another object are equal.

* `final String getDomain()`<br>
 Gets the domain of this email address (the part after the "@" sign).

* `final String getLocalPart()`<br>
 Gets the local part of this email address (the part before the "@" sign).

* `int hashCode()`<br>
 Returns a hash code for this address object.

* `String toString()`<br>
 Converts this address object to a text string.

## Method Details

### equals

    public boolean equals(Object obj)

Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code> in class <code>Object</code>

**Parameters:**

* <code>obj</code> - The parameter <code>obj</code> is an arbitrary object.

**Returns:**

* <code>true</code> if this object and another object are equal; otherwise,
 <code>false</code>.

### getLocalPart

    public final String getLocalPart()

Gets the local part of this email address (the part before the "@" sign).

**Returns:**

* The local part of this email address (the part before the "@" sign).

### toString

    public String toString()

Converts this address object to a text string.

**Overrides:**

* <code>toString</code> in class <code>Object</code>

**Returns:**

* A string representation of this object.

### hashCode

    public int hashCode()

Returns a hash code for this address object. No application or process
 identifiers are used in the hash code calculation.

**Overrides:**

* <code>hashCode</code> in class <code>Object</code>

**Returns:**

* A hash code for this instance.

### getDomain

    public final String getDomain()

Gets the domain of this email address (the part after the "@" sign).

**Returns:**

* The domain of this email address (the part after the "@" sign).
