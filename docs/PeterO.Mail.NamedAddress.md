## PeterO.Mail.NamedAddress

    public class NamedAddress

Represents an email address and a name for that address. Can represent a group of email addresses instead.

### Member Summary
* <code>[Address](#Address)</code> - Gets the email address associated with this object.
* <code>[AddressesEqual(PeterO.Mail.NamedAddress)](#AddressesEqual_PeterO_Mail_NamedAddress)</code> - Determines whether the email addresses stored this object are the same between this object and the given object, regardless of the display names they store.
* <code>[DisplayName](#DisplayName)</code> - Gets the display name for this email address.
* <code>[Equals(object)](#Equals_object)</code> - Determines whether this object and another object are equal.
* <code>[GetHashCode()](#GetHashCode)</code> - Calculates the hash code of this object.
* <code>[GroupAddresses](#GroupAddresses)</code> - Gets a read-only list of addresses that make up the group, if this object represents a group, or an empty list otherwise.
* <code>[IsGroup](#IsGroup)</code> - Gets a value indicating whether this represents a group of addresses rather than a single address.
* <code>[Name](#Name)</code> - Gets the display name for this email address, or the email address's value if the display name is null.
* <code>[ParseAddresses(string)](#ParseAddresses_string)</code> - Generates a list of NamedAddress objects from a comma-separated list of addresses.
* <code>[ToDisplayString()](#ToDisplayString)</code> - Converts this named-address object to a text string intended for display to end users.
* <code>[ToDisplayString(System.Collections.Generic.IList)](#ToDisplayString_System_Collections_Generic_IList)</code> - Generates a string containing the display names and email addresses of the given named-address objects, separated by commas.
* <code>[ToDisplayStringShort(System.Collections.Generic.IList)](#ToDisplayStringShort_System_Collections_Generic_IList)</code> - Generates a string containing the display names of the given named-address objects, separated by commas.
* <code>[ToString()](#ToString)</code> - Converts this object to a text string.

<a id="Void_ctor_System_String"></a>
### NamedAddress Constructor

    public NamedAddress(
        string address);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class. Examples:

 *  `john@example.com`

 *  `"John Doe" <john@example.com>`

 *  `=?utf-8?q?John`  `=`  `27s_Office?=<john@example.com>`

 *  `John <john@example.com>`

 *  `"Group" : Tom <tom@example.com>, Jane
            <jane@example.com>;`

<b>Parameters:</b>

 * <i>address</i>: A text string identifying a single email address or a group of email addresses. Comments, or text within parentheses, can appear. Multiple email addresses are not allowed unless they appear in the group syntax given above. Encoded words under RFC 2047 that appear within comments or display names will be decoded. An RFC 2047 encoded word consists of "=?", a character encoding name, such as  `utf-8` , either "?B?" or "?Q?" (in upper or lower case), a series of bytes in the character encoding, further encoded using B or Q encoding, and finally "?=". B encoding uses Base64, while in Q encoding, spaces are changed to "_", equals are changed to "=3D", and most bytes other than the basic digits 0 to 9 (0x30 to 0x39) and the basic letters A/a to Z/z (0x41 to 0x5a, 0x61 to 0x7a) are changed to "=" followed by their 2-digit hexadecimal form. An encoded word's maximum length is 75 characters. See the third example.

.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>address</i>
 is null.

 * System.ArgumentException:
Address has an invalid syntax.; Address has an invalid syntax.

<a id="Void_ctor_System_String_PeterO_Mail_Address"></a>
### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        PeterO.Mail.Address address);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class using the given display name and email address.

<b>Parameters:</b>

 * <i>displayName</i>: The display name of the email address. Can be null or empty. Encoded words under RFC 2047 will not be decoded.

 * <i>address</i>: An email address.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>address</i>
 is null.

<a id="Void_ctor_System_String_System_String"></a>
### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string address);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class using the given display name and email address.

<b>Parameters:</b>

 * <i>displayName</i>: The display name of the email address. Can be null or empty. Encoded words under RFC 2047 will not be decoded.

 * <i>address</i>: An email address.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>address</i>
 is null.

<a id="Void_ctor_System_String_System_String_System_String"></a>
### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string localPart,
        string domain);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class using the given name and an email address made up of its local part and domain.

<b>Parameters:</b>

 * <i>displayName</i>: The display name of the email address. Can be null or empty.

 * <i>localPart</i>: The local part of the email address (before the "@").

 * <i>domain</i>: The domain of the email address (before the "@").

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>localPart</i>
 or  <i>domain</i>
 is null.

<a id="Void_ctor_System_String_System_Collections_Generic_IList_1_PeterO_Mail_NamedAddress"></a>
### NamedAddress Constructor

    public NamedAddress(
        string groupName,
        System.Collections.Generic.IList mailboxes);

Initializes a new instance of the [PeterO.Mail.NamedAddress](PeterO.Mail.NamedAddress.md) class. Takes a group name and several named email addresses as parameters, and forms a group with them.

<b>Parameters:</b>

 * <i>groupName</i>: The group's name.

 * <i>mailboxes</i>: A list of named addresses that make up the group.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>groupName</i>
 or  <i>mailboxes</i>
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

Gets the display name for this email address.

<b>Returns:</b>

The display name for this email address. Returns null if the display name is absent.

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

 `true`  If this represents a group of addresses; otherwise,  `false` .

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

Determines whether the email addresses stored this object are the same between this object and the given object, regardless of the display names they store. For groups, the email addresses must be equal and in the same order in both objects.

<b>Parameters:</b>

 * <i>na</i>: A named address object to compare with this one. Can be null.

<b>Return Value:</b>

Either  `true`  or  `false` .

<a id="Equals_object"></a>
### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal. For groups, the named addresses (display name/email address pairs) must be equal and in the same order in both objects.

<b>Parameters:</b>

 * <i>obj</i>: An arbitrary object to compare with this one.

<b>Return Value:</b>

 `true`  if this object and another object are equal and have the same type; otherwise,  `false` .

<a id="GetHashCode"></a>
### GetHashCode

    public override int GetHashCode();

Calculates the hash code of this object. The exact algorithm used by this method is not guaranteed to be the same between versions of this library, and no application or process IDs are used in the hash code calculation.

<b>Return Value:</b>

A 32-bit hash code.

<a id="ParseAddresses_string"></a>
### ParseAddresses

    public static System.Collections.Generic.IList ParseAddresses(
        string addressValue);

Generates a list of NamedAddress objects from a comma-separated list of addresses. Each address must follow the syntax accepted by the one-argument constructor of NamedAddress.

<b>Parameters:</b>

 * <i>addressValue</i>: A comma-separated list of addresses in the form of a text string.

<b>Return Value:</b>

A list of addresses generated from the  <i>addressValue</i>
 parameter.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>addressValue</i>
 is null.

<a id="ToDisplayString_System_Collections_Generic_IList"></a>
### ToDisplayString

    public static string ToDisplayString(
        System.Collections.Generic.IList addresses);

Generates a string containing the display names and email addresses of the given named-address objects, separated by commas. The generated string is intended to be displayed to end users, and is not intended to be parsed by computer programs.

<b>Parameters:</b>

 * <i>addresses</i>: A list of named address objects.

<b>Return Value:</b>

A string containing the display names and email addresses of the given named-address objects, separated by commas.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>addresses</i>
 is null.

<a id="ToDisplayString"></a>
### ToDisplayString

    public string ToDisplayString();

Converts this named-address object to a text string intended for display to end users. The returned string is not intended to be parsed by computer programs.

<b>Return Value:</b>

A text string of this named-address object, intended for display to end-users.

<a id="ToDisplayStringShort_System_Collections_Generic_IList"></a>
### ToDisplayStringShort

    public static string ToDisplayStringShort(
        System.Collections.Generic.IList addresses);

Generates a string containing the display names of the given named-address objects, separated by commas. The generated string is intended to be displayed to end users, and is not intended to be parsed by computer programs. If a named address has no display name, its email address is used as the display name.

<b>Parameters:</b>

 * <i>addresses</i>: A list of named address objects.

<b>Return Value:</b>

A string containing the display names of the given named-address objects, separated by commas.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>addresses</i>
 is null.

<a id="ToString"></a>
### ToString

    public override string ToString();

Converts this object to a text string. This will generally be the form of this NamedAddress object as it could appear in a "To" header field.

<b>Return Value:</b>

A string representation of this object.
