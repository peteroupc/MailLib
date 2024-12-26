# com.upokecenter.mail.NamedAddress

    public class NamedAddress extends Object

Represents an email address and a name for that address. Can represent a
 group of email addresses instead.

## Constructors

## Methods

* `boolean AddressesEqual(NamedAddress na)`<br>
 Determines whether the email addresses stored this object are the same
 between this object and the given object, regardless of the display names
 they store.

* `boolean equals(Object obj)`<br>
 Determines whether this object and another object are equal.

* `final Address getAddress()`<br>
 Gets the email address associated with this object.

* `final String getDisplayName()`<br>
 Gets the display name for this email address.

* `final List<NamedAddress> getGroupAddresses()`<br>
 Gets a read-only list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.

* `final String getName()`<br>
 Gets the display name for this email address, or the email address's value
 if the display name is null.

* `int hashCode()`<br>
 Calculates the hash code of this object.

* `final boolean isGroup()`<br>
 Gets a value indicating whether this represents a group of addresses rather
 than a single address.

* `static List<NamedAddress> ParseAddresses(String addressValue)`<br>
 Generates a list of NamedAddress objects from a comma-separated list of
 addresses.

* `String ToDisplayString()`<br>
 Converts this named-address object to a text string intended to be displayed
 for others to see.

* `static String ToDisplayString(List<NamedAddress> addresses)`<br>
 Generates a string containing the display names and email addresses of the
 given named-address objects, separated by commas.

* `static String ToDisplayStringShort(List<NamedAddress> addresses)`<br>
 Generates a string containing the display names of the given named-address
 objects, separated by commas.

* `String toString()`<br>
 Converts this object to a text string.

## Method Details

### ToDisplayStringShort
    public static String ToDisplayStringShort(List<NamedAddress> addresses)
Generates a string containing the display names of the given named-address
 objects, separated by commas. The generated string is intended to be
 displayed for others to see, and is not intended to be parsed by computer
 programs. If a named address has no display name, its email address is used
 as the display name.

**Parameters:**

* <code>addresses</code> - A list of named address objects.

**Returns:**

* A string containing the display names of the given named-address
 objects, separated by commas.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>addresses</code> is null.

### ToDisplayString
    public static String ToDisplayString(List<NamedAddress> addresses)
Generates a string containing the display names and email addresses of the
 given named-address objects, separated by commas. The generated string is
 intended to be displayed for others to see, and is not intended to be parsed
 by computer programs.

**Parameters:**

* <code>addresses</code> - A list of named address objects.

**Returns:**

* A string containing the display names and email addresses of the
 given named-address objects, separated by commas.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>addresses</code> is null.

### ParseAddresses
    public static List<NamedAddress> ParseAddresses(String addressValue)
Generates a list of NamedAddress objects from a comma-separated list of
 addresses. Each address must follow the syntax accepted by the one-argument
 constructor of NamedAddress.

**Parameters:**

* <code>addressValue</code> - A comma-separated list of addresses in the form of a
 text string.

**Returns:**

* A list of addresses generated from the <code>addressValue</code>
 parameter.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>addressValue</code> is null.

### hashCode
    public int hashCode()
Calculates the hash code of this object. The exact algorithm used by this
 method is not guaranteed to be the same between versions of this library,
 and no application or process IDs are used in the hash code calculation.

**Overrides:**

* <code>hashCode</code> in class <code>Object</code>

**Returns:**

* A 32-bit hash code.

### equals
    public boolean equals(Object obj)
Determines whether this object and another object are equal. For groups, the
 named addresses (display name/email address pairs) must be equal and in the
 same order in both objects.

**Overrides:**

* <code>equals</code> in class <code>Object</code>

**Parameters:**

* <code>obj</code> - An arbitrary object to compare with this one.

**Returns:**

* <code>true</code> if this object and another object are equal and have
 the same type; otherwise, <code>false</code>.

### AddressesEqual
    public boolean AddressesEqual(NamedAddress na)
Determines whether the email addresses stored this object are the same
 between this object and the given object, regardless of the display names
 they store. For groups, the email addresses must be equal and in the same
 order in both objects.

**Parameters:**

* <code>na</code> - A named address object to compare with this one. Can be null.

**Returns:**

* Either <code>true</code> or <code>false</code>.

### getName
    public final String getName()
Gets the display name for this email address, or the email address's value
 if the display name is null. Returns an empty string if the address and
 display name are null.

**Returns:**

* The name for this email address.

### getDisplayName
    public final String getDisplayName()
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
    public String toString()
Converts this object to a text string. This will generally be the form of
 this NamedAddress object as it could appear in a "To" header field.

**Overrides:**

* <code>toString</code> in class <code>Object</code>

**Returns:**

* A string representation of this object.

### ToDisplayString
    public String ToDisplayString()
Converts this named-address object to a text string intended to be displayed
 for others to see. The returned string is not intended to be parsed by
 computer programs.

**Returns:**

* A text string of this named-address object, intended for display to
 end-users.

### getGroupAddresses
    public final List<NamedAddress> getGroupAddresses()
Gets a read-only list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.

**Returns:**

* A list of addresses that make up the group, if this object
 represents a group, or an empty list otherwise.
