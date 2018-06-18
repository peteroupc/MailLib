# com.upokecenter.mail.Address

    public class Address extends Object

Represents an email address.

## Methods

* `Address​(String addressValue) Address`<br>
 Initializes a new instance of the Address
 class.
* `String getDomain()`<br>
 Gets the domain of this email address (the part after the "@" sign).
* `String getLocalPart()`<br>
 Gets the local part of this email address (the part before the "@" sign).
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

### getDomain
    public final String getDomain()
Gets the domain of this email address (the part after the "@" sign).

**Returns:**

* The domain of this email address (the part after the "@" sign).
