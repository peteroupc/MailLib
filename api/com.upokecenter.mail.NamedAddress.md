# com.upokecenter.mail.NamedAddress

    public class NamedAddress extends java.lang.Object

Represents an email address and a name for that address. Can represent a
 group of email addresses instead.

## Methods

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

## Method Details

### NamedAddress
    public NamedAddress​(java.lang.String address)
Initializes a new instance of the <code>NamedAddress</code>
  class. Examples: <ul> <li><code>john@example.com</code></li> <li><code>"John
  Doe" &lt;john@example.com&gt;</code></li>

  <li><code>=?utf-8?q?John</code><code>=</code><code>27s_Office?=&lt;john@example.com&gt;</code></li>
  <li><code>John &lt;john@example.com&gt;</code></li> <li><code>"Group" : Tom
 &lt;tom@example.com&gt;, Jane
 &lt;jane@example.com&gt;;</code></li></ul>

**Parameters:**

* <code>address</code> - A text string identifying a single email address or a group
 of email addresses. Comments, or text within parentheses, can
 appear. Multiple email addresses are not allowed unless they appear
 in the group syntax given above. Encoded words under RFC 2047 that
 appear within comments or display names will be decoded. <p>An RFC
  2047 encoded word consists of "=?", a character encoding name, such
  as <code>utf-8</code>, either "?B?" or "?Q?" (in upper or lower case), a
 series of bytes in the character encoding, further encoded using B
  or Q encoding, and finally "?=". B encoding uses Base64, while in Q
  encoding, spaces are changed to "_", equals are changed to "=3D",
 and most bytes other than the basic digits 0 to 9 (0x30 to 0x39) and
 the basic letters A/a to Z/z (0x41 to 0x5a, 0x61 to 0x7a) are
  changed to "=" followed by their 2-digit hexadecimal form. An
 encoded word's maximum length is 75 characters. See the third
 example.</p>.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>address</code> is null.

* <code>java.lang.IllegalArgumentException</code> - Address has an invalid syntax.; Address has an
 invalid syntax.

### NamedAddress
    public NamedAddress​(java.lang.String displayName, java.lang.String address)
Initializes a new instance of the <code>NamedAddress</code>
 class using the given display name and email address.

**Parameters:**

* <code>displayName</code> - The display name of the email address. Can be null or
 empty. Encoded words under RFC 2047 will not be decoded.

* <code>address</code> - An email address.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>address</code> is null.

### NamedAddress
    public NamedAddress​(java.lang.String displayName, Address address)
Initializes a new instance of the <code>NamedAddress</code>
 class using the given display name and email address.

**Parameters:**

* <code>displayName</code> - The display name of the email address. Can be null or
 empty. Encoded words under RFC 2047 will not be decoded.

* <code>address</code> - An email address.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>address</code> is null.

### NamedAddress
    public NamedAddress​(java.lang.String displayName, java.lang.String localPart, java.lang.String domain)
Initializes a new instance of the <code>NamedAddress</code>
 class using the given name and an email address made up of its local
 part and domain.

**Parameters:**

* <code>displayName</code> - The display name of the email address. Can be null or
 empty.

* <code>localPart</code> - The local part of the email address (before the "@").

* <code>domain</code> - The domain of the email address (before the "@").

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>localPart</code> or <code>
 domain</code> is null.

### NamedAddress
    public NamedAddress​(java.lang.String groupName, java.util.List<NamedAddress> mailboxes)
Initializes a new instance of the <code>NamedAddress</code>
 class. Takes a group name and several named email addresses as
 parameters, and forms a group with them.

**Parameters:**

* <code>groupName</code> - The group's name.

* <code>mailboxes</code> - A list of named addresses that make up the group.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>groupName</code> or <code>
 mailboxes</code> is null.

* <code>java.lang.IllegalArgumentException</code> - GroupName is empty.; A mailbox in the list is a
 group.

### ToDisplayStringShort
    public static java.lang.String ToDisplayStringShort​(java.util.List<NamedAddress> addresses)
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

### ToDisplayString
    public static java.lang.String ToDisplayString​(java.util.List<NamedAddress> addresses)
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

### ParseAddresses
    public static java.util.List<NamedAddress> ParseAddresses​(java.lang.String addressValue)
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

### hashCode
    public int hashCode()
Calculates the hash code of this object. The exact algorithm used by this
 method is not guaranteed to be the same between versions of this
 library, and no application or process IDs are used in the hash code
 calculation.

**Overrides:**

* <code>hashCode</code> in class <code>java.lang.Object</code>

**Returns:**

* A 32-bit hash code.

### equals
    public boolean equals​(java.lang.Object obj)
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

### AddressesEqual
    public boolean AddressesEqual​(NamedAddress na)
Determines whether the email addresses stored this object are the same
 between this object and the given object, regardless of the display
 names they store. For groups, the email addresses must be equal and
 in the same order in both objects.

**Parameters:**

* <code>na</code> - A named address object to compare with this one. Can be null.

**Returns:**

* Either <code>true</code> or <code>false</code>.

### getName
    public final java.lang.String getName()
Gets the display name for this email address, or the email address's value
 if the display name is null. Returns an empty string if the address
 and display name are null.

**Returns:**

* The name for this email address.

### getDisplayName
    public final java.lang.String getDisplayName()
Gets the display name for this email address.

**Returns:**

* The display name for this email address. Returns null if the display
 name is absent.

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

* <code>true</code> If this represents a group of addresses; otherwise,
 <code>false</code>.

### toString
    public java.lang.String toString()
Converts this object to a text string. This will generally be the form of
  this NamedAddress object as it could appear in a "To" header field.

**Overrides:**

* <code>toString</code> in class <code>java.lang.Object</code>

**Returns:**

* A string representation of this object.

### ToDisplayString
    public java.lang.String ToDisplayString()
Converts this named-address object to a text string intended for display to
 end users. The returned string is not intended to be parsed by
 computer programs.

**Returns:**

* A text string of this named-address object, intended for display to
 end-users.

### getGroupAddresses
    public final java.util.List<NamedAddress> getGroupAddresses()
Gets a read-only list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.

**Returns:**

* A list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.
