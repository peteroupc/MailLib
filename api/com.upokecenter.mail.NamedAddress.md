# com.upokecenter.mail.NamedAddress

## Constructors

* `NamedAddress​(java.lang.String address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress
  class.
* `NamedAddress​(java.lang.String displayName,
Address address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress
 class using the given display name and email address.
* `NamedAddress​(java.lang.String displayName,
java.lang.String address) NamedAddress`<br>
 Initializes a new instance of the NamedAddress
 class using the given display name and email address.
* `NamedAddress​(java.lang.String displayName,
java.lang.String localPart,
java.lang.String domain) NamedAddress`<br>
 Initializes a new instance of the NamedAddress
 class using the given name and an email address made up of its local
 part and domain.
* `NamedAddress​(java.lang.String groupName,
java.util.List<NamedAddress> mailboxes) NamedAddress`<br>
 Initializes a new instance of the NamedAddress
 class.

## Methods

* `boolean AddressesEqual​(NamedAddress na)`<br>
 Determines whether the email addresses stored this object are the same
 between this object and the given object, regardless of the display
 names they store.
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
* `java.lang.String ToDisplayString()`<br>
 Converts this named-address object to a text string intended for display to
 end users.
* `static java.lang.String ToDisplayString​(java.util.List<NamedAddress> addresses)`<br>
 Generates a string containing the display names and email addresses of the
 given named-address objects, separated by commas.
* `static java.lang.String ToDisplayStringShort​(java.util.List<NamedAddress> addresses)`<br>
 Generates a string containing the display names of the given named-address
 objects, separated by commas.
* `java.lang.String toString()`<br>
 Converts this object to a text string.

## Method Details

### <a id='ToDisplayStringShort(java.util.List)'>ToDisplayStringShort</a>

Generates a string containing the display names of the given named-address
 objects, separated by commas. The generated string is intended to be
 displayed to end users, and is not intended to be parsed by computer
 programs. If a named address has no display name, its email address
 is used as the display name.

**Parameters:**

* <code>addresses</code> - A list of named address objects.

**Returns:**

* A string containing the display names of the given named-address
 objects, separated by commas.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>addresses</code> is null.

### <a id='ToDisplayString(java.util.List)'>ToDisplayString</a>

Generates a string containing the display names and email addresses of the
 given named-address objects, separated by commas. The generated
 string is intended to be displayed to end users, and is not intended
 to be parsed by computer programs.

**Parameters:**

* <code>addresses</code> - A list of named address objects.

**Returns:**

* A string containing the display names and email addresses of the
 given named-address objects, separated by commas.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>addresses</code> is null.

### <a id='ParseAddresses(java.lang.String)'>ParseAddresses</a>

Generates a list of NamedAddress objects from a comma-separated list of
 addresses. Each address must follow the syntax accepted by the
 one-argument constructor of NamedAddress.

**Parameters:**

* <code>addressValue</code> - A comma-separated list of addresses in the form of a
 text string.

**Returns:**

* A list of addresses generated from the <code>addressValue</code>
 parameter.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>addressValue</code> is null.

### <a id='hashCode()'>hashCode</a>

Calculates the hash code of this object. The exact algorithm used by this
 method is not guaranteed to be the same between versions of this
 library, and no application or process IDs are used in the hash code
 calculation.

**Overrides:**

* <code>hashCode</code> in class <code>java.lang.Object</code>

**Returns:**

* A 32-bit hash code.

### <a id='equals(java.lang.Object)'>equals</a>

Determines whether this object and another object are equal. For groups, the
 named addresses (display name/email address pairs) must be equal and
 in the same order in both objects.

**Overrides:**

* <code>equals</code> in class <code>java.lang.Object</code>

**Parameters:**

* <code>obj</code> - An arbitrary object to compare with this one.

**Returns:**

* <code>true</code> if this object and another object are equal and have
 the same type; otherwise, <code>false</code>.

### <a id='AddressesEqual(com.upokecenter.mail.NamedAddress)'>AddressesEqual</a>

Determines whether the email addresses stored this object are the same
 between this object and the given object, regardless of the display
 names they store. For groups, the email addresses must be equal and
 in the same order in both objects.

**Parameters:**

* <code>na</code> - A named address object to compare with this one. Can be null.

**Returns:**

* Either <code>true</code> or <code>false</code>.

### <a id='getName()'>getName</a>

Gets the display name for this email address, or the email address's value
 if the display name is null. Returns an empty string if the address
 and display name are null.

**Returns:**

* The name for this email address.

### <a id='getDisplayName()'>getDisplayName</a>

Gets the display name for this email address.

**Returns:**

* The display name for this email address. Returns null if the display
 name is absent.

### <a id='getAddress()'>getAddress</a>

Gets the email address associated with this object.

**Returns:**

* The email address associated with this object. This value is null if
 this object represents a group of addresses instead.

### <a id='isGroup()'>isGroup</a>

Gets a value indicating whether this represents a group of addresses rather
 than a single address.

**Returns:**

* <code>true</code> If this represents a group of addresses; otherwise,
 <code>false</code>.

### <a id='toString()'>toString</a>

Converts this object to a text string. This will generally be the form of
  this NamedAddress object as it could appear in a "To" header field.

**Overrides:**

* <code>toString</code> in class <code>java.lang.Object</code>

**Returns:**

* A string representation of this object.

### <a id='ToDisplayString()'>ToDisplayString</a>

Converts this named-address object to a text string intended for display to
 end users. The returned string is not intended to be parsed by
 computer programs.

**Returns:**

* A text string of this named-address object, intended for display to
 end-users.

### <a id='getGroupAddresses()'>getGroupAddresses</a>

Gets a read-only list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.

**Returns:**

* A list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.
