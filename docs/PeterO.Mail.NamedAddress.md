## PeterO.Mail.NamedAddress

    public class NamedAddress

Represents an email address and a name for that address.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string representation of this object.

### NamedAddress Constructor

    public NamedAddress(
        string address);

Initializes a new instance of the NamedAddress class.

<b>Parameters:</b>

 * <i>address</i>: A string object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>address</i>
 is null.

### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string address);

Initializes a new instance of the NamedAddress class using the given display name and email address.

<b>Parameters:</b>

 * <i>displayName</i>: A string object.

 * <i>address</i>: A string object. (2).

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>address</i>
 is null.

### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        PeterO.Mail.Address address);

Initializes a new instance of the NamedAddress class.

<b>Parameters:</b>

 * <i>displayName</i>: A string object.

 * <i>address</i>: An email address.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>address</i>
 is null.

### NamedAddress Constructor

    public NamedAddress(
        string displayName,
        string localPart,
        string domain);

Initializes a new instance of the NamedAddress class using the given name and an email address made up of its local part and domain.

<b>Parameters:</b>

 * <i>displayName</i>: A string object.

 * <i>localPart</i>: A string object. (2).

 * <i>domain</i>: A string object. (3).

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>localPart</i>
 or  <i>domain</i>
 is null.

### NamedAddress Constructor

    public NamedAddress(
        string groupName,
        System.Collections.Generic.IList mailboxes);

Initializes a new instance of the NamedAddress class.

<b>Parameters:</b>

 * <i>groupName</i>: A string object.

 * <i>mailboxes</i>: An IList object.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>groupName</i>
 or  <i>mailboxes</i>
is null.

### Name

    public string Name { get; }

Gets the display name for this email address.

<b>Returns:</b>

The display name for this email address.

### Address

    public PeterO.Mail.Address Address { get; }

Gets a value not documented yet.

<b>Returns:</b>

A value not documented yet.

### IsGroup

    public bool IsGroup { get; }

Gets a value indicating whether this represents a group of addresses rather than a single address.

<b>Returns:</b>

True if this represents a group of addresses; otherwise, false..

### GroupAddresses

    public System.Collections.Generic.IList GroupAddresses { get; }

Gets a list of address that make up the group, if this object represents a group, or an empty list otherwise.

<b>Returns:</b>

A list of address that make up the group, if this object represents a group, or an empty list otherwise.


