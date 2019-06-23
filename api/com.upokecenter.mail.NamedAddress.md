# com.upokecenter.mail.NamedAddress

    public class NamedAddress extends java.lang.Object

Represents an email address and a name for that address. Can represent a
 group of email addresses instead.

## Methods

* `NamedAddress​(java.lang.String address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String displayName,
            Address address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String displayName,
            java.lang.String address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String displayName,
            java.lang.String localPart,
            java.lang.String domain) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String groupName,
            java.util.List<NamedAddress> mailboxes) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `boolean AddressesEqual​(NamedAddress na)`<br>
 Not documented yet.
* `boolean equals​(java.lang.Object obj)`<br>
 Determines whether this object and another object are equal.
* `Address getAddress()`<br>
 Gets the email address associated with this object.
* `java.lang.String getDisplayName()`<br>
 Gets the display name for this email address.
* `java.util.List<NamedAddress> getGroupAddresses()`<br>
 Gets a read-only list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.
* `java.lang.String getName()`<br>
 Gets the display name for this email address, or the email address's value
 if the display name is null.
* `int hashCode()`<br>
 Calculates the hash code of this object.
* `boolean isGroup()`<br>
 Gets a value indicating whether this represents a group of addresses rather
 than a single address.
* `static java.util.List<NamedAddress> ParseAddresses​(java.lang.String addressValue)`<br>
 Generates a list of NamedAddress objects from a comma-separated list of
 addresses.
* `java.lang.String toString()`<br>
 Converts this object to a text string.

## Constructors

* `NamedAddress​(java.lang.String address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String displayName,
            Address address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String displayName,
            java.lang.String address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String displayName,
            java.lang.String localPart,
            java.lang.String domain) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.
* `NamedAddress​(java.lang.String groupName,
            java.util.List<NamedAddress> mailboxes) NamedAddress`<br>
 Initializes a new instance of the NamedAddress class.

## Method Details

### NamedAddress
    public NamedAddress​(java.lang.String address)
Initializes a new instance of the <code>NamedAddress</code> class.

**Parameters:**

* <code>address</code> - The parameter <code>address</code> is a text string.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>address</code> is null.

* <code>java.lang.IllegalArgumentException</code> - Address has an invalid syntax.; Address has
 an invalid syntax.

### NamedAddress
    public NamedAddress​(java.lang.String displayName, java.lang.String address)
Initializes a new instance of the <code>NamedAddress</code> class.

**Parameters:**

* <code>displayName</code> - The parameter <code>displayName</code> is a text string.

* <code>address</code> - The parameter <code>address</code> is a text string.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>address</code> is null.

### NamedAddress
    public NamedAddress​(java.lang.String displayName, Address address)
Initializes a new instance of the <code>NamedAddress</code> class.

**Parameters:**

* <code>displayName</code> - The parameter <code>displayName</code> is a text string.

* <code>address</code> - The parameter <code>address</code> is an Address object.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>address</code> is null.

### NamedAddress
    public NamedAddress​(java.lang.String displayName, java.lang.String localPart, java.lang.String domain)
Initializes a new instance of the <code>NamedAddress</code> class.

**Parameters:**

* <code>displayName</code> - The parameter <code>displayName</code> is a text string.

* <code>localPart</code> - The parameter <code>localPart</code> is a text string.

* <code>domain</code> - The parameter <code>domain</code> is a text string.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>localPart</code> or
 <code>domain</code> is null.

### NamedAddress
    public NamedAddress​(java.lang.String groupName, java.util.List<NamedAddress> mailboxes)
Initializes a new instance of the <code>NamedAddress</code> class.

**Parameters:**

* <code>groupName</code> - The parameter <code>groupName</code> is a text string.

* <code>mailboxes</code> - The parameter <code>mailboxes</code> is an IList object.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>groupName</code> or
 <code>mailboxes</code> is null.

* <code>java.lang.IllegalArgumentException</code> - GroupName is empty.; A mailbox in the list
 is a group.

### ParseAddresses
    public static java.util.List<NamedAddress> ParseAddresses​(java.lang.String addressValue)
Generates a list of NamedAddress objects from a comma-separated list of
 addresses. Each address must follow the syntax accepted by the
 one-argument constructor of NamedAddress.

**Parameters:**

* <code>addressValue</code> - A comma-separate list of addresses in the form of a text
 string.

**Returns:**

* A list of addresses generated from the <code>addressValue</code>
 parameter.

### hashCode
    public int hashCode()
Calculates the hash code of this object. No application or process IDs are
 used in the hash code calculation.

**Overrides:**

* <code>hashCode</code> in class <code>java.lang.Object</code>

**Returns:**

* A 32-bit hash code.

### equals
    public boolean equals​(java.lang.Object obj)
Determines whether this object and another object are equal.

**Overrides:**

* <code>equals</code> in class <code>java.lang.Object</code>

**Parameters:**

* <code>obj</code> - The parameter <code>obj</code> is an arbitrary object.

**Returns:**

* <code>true</code> if this object and another object are equal; otherwise,
 <code>false</code> .

### AddressesEqual
    public boolean AddressesEqual​(NamedAddress na)
Not documented yet.

**Parameters:**

* <code>na</code> - A named address object to compare with this one. Can be null.

**Returns:**

* Either <code>true</code> or <code>false</code> .

### getName
    public final java.lang.String getName()
Gets the display name for this email address, or the email address's value
 if the display name is null. Returns an empty string if the address
 and display name are null.

**Returns:**

* The name for this email address.

### getDisplayName
    public final java.lang.String getDisplayName()
Gets the display name for this email address. Returns null if the display
 name is absent.

**Returns:**

* The display name for this email address.

### getAddress
    public final Address getAddress()
Gets the email address associated with this object.

**Returns:**

* The email address associated with this object. This value is null if
 this object represents a group of addresses instead.

### isGroup
    public final boolean isGroup()
Gets a value indicating whether this represents a group of addresses rather
 than a single address.

**Returns:**

* <code>true</code> If this represents a group of addresses; otherwise, .
 <code>false</code> .

### toString
    public java.lang.String toString()
Converts this object to a text string. This will generally be the form of
 this NamedAddress object as it could appear in a "To" header field.

**Overrides:**

* <code>toString</code> in class <code>java.lang.Object</code>

**Returns:**

* A string representation of this object.

### getGroupAddresses
    public final java.util.List<NamedAddress> getGroupAddresses()
Gets a read-only list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.

**Returns:**

* A list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.
