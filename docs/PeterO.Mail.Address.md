## PeterO.Mail.Address

    public class Address

Represents an email address.

### Address Constructor

    public Address(
        string addressValue);

Initializes a new instance of the [PeterO.Mail.Address](PeterO.Mail.Address.md) class.

<b>Parameters:</b>

 * <i>addressValue</i>: An email address.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter <i>addressValue</i>
 is null.

 * System.ArgumentException:
The email address contains invalid syntax. For example, it doesn't contain an '@' sign or either side of the '@' contains invalid characters, the address is too long, or the address contains comments (text within parentheses).

### Domain

    public string Domain { get; }

Gets the domain of this email address (the part after the "@" sign).

<b>Returns:</b>

The domain of this email address (the part after the "@" sign).

### LocalPart

    public string LocalPart { get; }

Gets the local part of this email address (the part before the "@" sign).

<b>Returns:</b>

The local part of this email address (the part before the "@" sign).

### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: The parameter <i>obj</i>
is an arbitrary object.

<b>Return Value:</b>

 `true` if this object and another object are equal; otherwise,  `false` .

### GetHashCode

    public override int GetHashCode();

Returns a hash code for this address object. No application or process identifiers are used in the hash code calculation.

<b>Return Value:</b>

A hash code for this instance.

### ToString

    public override string ToString();

Converts this address object to a text string.

<b>Return Value:</b>

A string representation of this object.
