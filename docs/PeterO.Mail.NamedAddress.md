## PeterO.Mail.NamedAddress

    public class NamedAddress

Represents an email address and a name for that address. Can represent a group of email addresses instead.

### Member Summary
* <code>[Address](#Address)</code> - Gets the email address associated with this object.
* <code>[AddressesEqual(PeterO.Mail.NamedAddress)](#AddressesEqual_PeterO_Mail_NamedAddress)</code> - Not documented yet.
* <code>[DisplayName](#DisplayName)</code> - Gets the display name for this email address.
* <code>[Equals(object)](#Equals_object)</code> - Determines whether this object and another object are equal.
* <code>[GetHashCode()](#GetHashCode)</code> - Calculates the hash code of this object.
* <code>[GroupAddresses](#GroupAddresses)</code> - Gets a read-only list of addresses that make up the group, if this object represents a group, or an empty list otherwise.
* <code>[IsGroup](#IsGroup)</code> - Gets a value indicating whether this represents a group of addresses rather than a single address.
* <code>[Name](#Name)</code> - Gets the display name for this email address, or the email address's value if the display name is null.
* <code>[ParseAddresses(string)](#ParseAddresses_string)</code> - Generates a list of NamedAddress objects from a comma-separated list of addresses.
* <code>[ToString()](#ToString)</code> - Converts this object to a text string.

<a id="Void_ctor_String"></a>
### NamedAddress Constructor

    public NamedAddress(
        string address);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class.

<b>Parameters:</b>

 * <i>address</i>: The parameter <i>address</i>
is a text string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>address</i>
is null.

 * System.ArgumentException:
Address has an invalid syntax.; Address has an invalid syntax.

<a id="Void_ctor_String_Address"></a>
### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        PeterO.Mail.Address address);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class.

<b>Parameters:</b>

 * <i>displayName</i>: The parameter <i>displayName</i>
is a text string.

 * <i>address</i>: The parameter <i>address</i>
is an Address object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>address</i>
is null.

<a id="Void_ctor_String_String"></a>
### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string address);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class.

<b>Parameters:</b>

 * <i>displayName</i>: The parameter <i>displayName</i>
is a text string.

 * <i>address</i>: The parameter <i>address</i>
is a text string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>address</i>
is null.

<a id="Void_ctor_String_String_String"></a>
### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string localPart,
        string domain);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class.

<b>Parameters:</b>

 * <i>displayName</i>: The parameter <i>displayName</i>
is a text string.

 * <i>localPart</i>: The parameter <i>localPart</i>
is a text string.

 * <i>domain</i>: The parameter <i>domain</i>
is a text string.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>localPart</i>
or <i>domain</i>
is null.

<a id="Void_ctor_String_IList_1"></a>
### NamedAddress Constructor

    public NamedAddress(
        string groupName,
        System.Collections.Generic.IList mailboxes);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class.

<b>Parameters:</b>

 * <i>groupName</i>: The parameter <i>groupName</i>
is a text string.

 * <i>mailboxes</i>: The parameter <i>mailboxes</i>
is an IList object.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>groupName</i>
or <i>mailboxes</i>
is null.

 * System.ArgumentException:
GroupName is empty.; A mailbox in the list is a group.

<a id="Address"></a>
### Address

    public PeterO.Mail.Address Address { get; }

Gets the email address associated with this object.

<b>Returns:</b>

The email address associated with this object. This value is null if this object represents a group of addresses instead.

<a id="DisplayName"></a>
### DisplayName

    public string DisplayName { get; }

Gets the display name for this email address. Returns null if the display name is absent.

<b>Returns:</b>

The display name for this email address.

<a id="GroupAddresses"></a>
### GroupAddresses

    public System.Collections.Generic.IList GroupAddresses { get; }

Gets a read-only list of addresses that make up the group, if this object represents a group, or an empty list otherwise.

<b>Returns:</b>

A list of addresses that make up the group, if this object represents a group, or an empty list otherwise.

<a id="IsGroup"></a>
### IsGroup

    public bool IsGroup { get; }

Gets a value indicating whether this represents a group of addresses rather than a single address.

<b>Returns:</b>

 `true`  If this represents a group of addresses; otherwise, . `false`  .

<a id="Name"></a>
### Name

    public string Name { get; }

Gets the display name for this email address, or the email address's value if the display name is null. Returns an empty string if the address and display name are null.

<b>Returns:</b>

The name for this email address.

<a id="AddressesEqual_PeterO_Mail_NamedAddress"></a>
### AddressesEqual

    public bool AddressesEqual(
        PeterO.Mail.NamedAddress na);

Not documented yet.

<b>Parameters:</b>

 * <i>na</i>: A named address object to compare with this one. Can be null.

<b>Return Value:</b>

Either `true`  or `false`  .

<a id="Equals_object"></a>
### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: The parameter <i>obj</i>
is an arbitrary object.

<b>Return Value:</b>

 `true`  if this object and another object are equal; otherwise, `false`  .

<a id="GetHashCode"></a>
### GetHashCode

    public override int GetHashCode();

Calculates the hash code of this object. No application or process IDs are used in the hash code calculation.

<b>Return Value:</b>

A 32-bit hash code.

<a id="ParseAddresses_string"></a>
### ParseAddresses

    public static System.Collections.Generic.IList ParseAddresses(
        string addressValue);

Generates a list of NamedAddress objects from a comma-separated list of addresses. Each address must follow the syntax accepted by the one-argument constructor of NamedAddress.

<b>Parameters:</b>

 * <i>addressValue</i>: A comma-separate list of addresses in the form of a text string.

<b>Return Value:</b>

A list of addresses generated from the <i>addressValue</i>
parameter.

<a id="ToString"></a>
### ToString

    public override string ToString();

Converts this object to a text string. This will generally be the form of this NamedAddress object as it could appear in a "To" header field.

<b>Return Value:</b>

A string representation of this object.
