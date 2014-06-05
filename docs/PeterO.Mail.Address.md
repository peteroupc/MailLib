## PeterO.Mail.Address

    public class Address

Represents an email address.

### ToString

    public override string ToString();

Converts this object to a text string.

<b>Returns:</b>

A string representation of this object.

### Address Constructor

    public Address(
        string addressValue);

Initializes a new instance of the Address class.

<b>Parameters:</b>

 * <i>addressValue</i>: An email address.

<b>Exceptions:</b>

 * System.ArgumentNullException: 
The parameter <i>addressValue</i>
 is null.

### LocalPart

    public string LocalPart { get; }

Gets the local part of this email address (the part before the "@" sign).

<b>Returns:</b>

The local part of this email address (the part before the "@" sign).

### Domain

    public string Domain { get; }

Gets the domain of this email address (the part after the "@" sign).

<b>Returns:</b>

The domain of this email address (the part after the "@" sign).


