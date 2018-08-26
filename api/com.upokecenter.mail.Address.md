# com.upokecenter.mail.Address

    public class Address extends Object

Represents an email address.

## Methods

* `Address​(String addressValue) Address`<br>
 Initializes a new instance of the Address
 class.
* `boolean equals​(Object obj)`<br>
 Determines whether this object and another object are equal.
* `String getDomain()`<br>
 Gets the domain of this email address (the part after the "@" sign).
* `String getLocalPart()`<br>
 Gets the local part of this email address (the part before the "@" sign).
* `int hashCode()`<br>
 Not documented yet.
* `String toString()`<br>
 Converts this address object to a text string.

## Constructors

* `Address​(String addressValue) Address`<br>
 Initializes a new instance of the Address
 class.

## Method Details

### Address
    public Address​(String addressValue)
Initializes a new instance of the <code>Address</code>
 class.

**Parameters:**

* <code>addressValue</code> - An email address.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>addressValue</code> is
 null.

* <code>IllegalArgumentException</code> - The email address contains invalid syntax.
 For example, it doesn't contain an '@' sign or either side of the '@'
 contains invalid characters, the address is too long, or the address
 contains comments (text within parentheses).

### Address
    public Address​(String addressValue)
Initializes a new instance of the <code>Address</code>
 class.

**Parameters:**

* <code>addressValue</code> - An email address.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>addressValue</code> is
 null.

* <code>IllegalArgumentException</code> - The email address contains invalid syntax.
 For example, it doesn't contain an '@' sign or either side of the '@'
 contains invalid characters, the address is too long, or the address
 contains comments (text within parentheses).

### equals
    public boolean equals​(Object obj)
Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code>&nbsp;in class&nbsp;<code>Object</code>

**Parameters:**

* <code>obj</code> - The parameter <code>obj</code> is an arbitrary object.

**Returns:**

* <code>true</code> if this object and another object are equal; otherwise,
 <code>false</code> .

### getLocalPart
    public final String getLocalPart()
Gets the local part of this email address (the part before the "@" sign).

**Returns:**

* The local part of this email address (the part before the "@" sign).

### toString
    public String toString()
Converts this address object to a text string.

**Overrides:**

* <code>toString</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A string representation of this object.

### hashCode
    public int hashCode()
Not documented yet.

**Overrides:**

* <code>hashCode</code>&nbsp;in class&nbsp;<code>Object</code>

**Returns:**

* A 32-bit signed integer.

### getDomain
    public final String getDomain()
Gets the domain of this email address (the part after the "@" sign).

**Returns:**

* The domain of this email address (the part after the "@" sign).
