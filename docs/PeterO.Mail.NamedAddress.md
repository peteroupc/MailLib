## PeterO.Mail.NamedAddress

    public class NamedAddress

Represents an email address and a name for that address. Can represent a group of email addresses instead.

### NamedAddress Constructor

    public NamedAddress(
        string address);

Initializes a new instance of the NamedAddress class. Examples:

 *  `john@example.com`

 *  `"John Doe" <john@example.com>`

 *  `=?utf-8?q?John`  `=`  `27s_Office?=
            <john@example.com>`

 *  `John <john@example.com>`

 *  `"Group" : Tom <tom@example.com>, Jane
            <jane@example.com>;`

<b>Parameters:</b>

 * <i>address</i>: A string object identifying a single email address or a group of email addresses. Comments, or text within parentheses, can appear. Multiple email addresses are not allowed unless they appear in the group syntax given above. Encoded words under RFC 2047 that appear within comments or display names will be decoded.An RFC 2047 encoded word consists of "=?", a character encoding name, such as  `utf-8` , either "?B?" or "?Q?" (in upper or lower case), a series of bytes in the character encoding, further encoded using B or Q encoding, and finally "?=". B encoding uses Base64, while in Q encoding, spaces are changed to "_", equals are changed to "=3D" , and most bytes other than the basic digits 0 to 9 (0x30 to 0x39) and the basic letters A/a to Z/z (0x41 to 0x5a, 0x61 to 0x7a) are changed to "=" followed by their 2-digit hexadecimal form. An encoded word's maximum length is 75 characters. See the second example.

.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>address</i>
 is null.

 * System.ArgumentException:
The named address has an invalid syntax.

### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        PeterO.Mail.Address address);

Initializes a new instance of the NamedAddress class.

<b>Parameters:</b>

 * <i>displayName</i>: A string object. If this value is null or empty, uses the email address as the display name. Encoded words under RFC 2047 will not be decoded.

 * <i>address</i>: An email address.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>address</i>
 is null.

### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string address);

Initializes a new instance of the NamedAddress class using the given display name and email address.

<b>Parameters:</b>

 * <i>displayName</i>: The address's display name. Can be null or empty, in which case the email address is used instead. Encoded words under RFC 2047 will not be decoded.

 * <i>address</i>: An email address.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>address</i>
 is null.

 * System.ArgumentException:
The display name or address has an invalid syntax.

### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string localPart,
        string domain);

Initializes a new instance of the NamedAddress class using the given name and an email address made up of its local part and domain.

<b>Parameters:</b>

 * <i>displayName</i>: A string object. If this value is null or empty, uses the email address as the display name.

 * <i>localPart</i>: The local part of the email address (before the "@").

 * <i>domain</i>: The domain of the email address (before the "@").

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>localPart</i>
 or  <i>domain</i>
 is null.

### NamedAddress Constructor

    public NamedAddress(
        string groupName,
        System.Collections.Generic.IList mailboxes);

Initializes a new instance of the NamedAddress class. Takes a group name and several named email addresses as parameters, and forms a group with them.

<b>Parameters:</b>

 * <i>groupName</i>: The group's name.

 * <i>mailboxes</i>: A list of named addresses that make up the group.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>groupName</i>
 or  <i>mailboxes</i>
 is null.

 * System.ArgumentException:
The parameter  <i>groupName</i>
 is empty, or an item in the list is itself a group.

### Address

    public PeterO.Mail.Address Address { get; }

Gets the email address associated with this object.

<b>Returns:</b>

The email address associated with this object. This value is null if this object represents a group of addresses instead.

### GroupAddresses

    public System.Collections.Generic.IList GroupAddresses { get; }

Gets a read-only list of addresses that make up the group, if this object represents a group, or an empty list otherwise.

<b>Returns:</b>

A list of addresses that make up the group, if this object represents a group, or an empty list otherwise.

### IsGroup

    public bool IsGroup { get; }

Gets a value indicating whether this represents a group of addresses rather than a single address.

<b>Returns:</b>

True if this represents a group of addresses; otherwise, false.

### Name

    public string Name { get; }

Gets the display name for this email address (or the email address's value if the display name is absent).

<b>Returns:</b>

The display name for this email address.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string representation of this object.
