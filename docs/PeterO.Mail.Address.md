## PeterO.Mail.Address

    public class Address

Represents an email address.

### Member Summary
* <code>[Domain](#Domain)</code> - Gets the domain of this email address (the part after the "@" sign).
* <code>[Equals(object)](#Equals_object)</code> - Determines whether this object and another object are equal.
* <code>[GetHashCode()](#GetHashCode)</code> - Returns a hash code for this address object.
* <code>[LocalPart](#LocalPart)</code> - Gets the local part of this email address (the part before the "@" sign).
* <code>[ToString()](#ToString)</code> - Converts this address object to a text string.

<a id="Void_ctor_System_String"></a>
### Address Constructor

    public Address(
        string addressValue);

Initializes a new instance of the [PeterO.Mail.Address](PeterO.Mail.Address.md) class.

<b>Parameters:</b>

 * <i>addressValue</i>: An email address. This parameter must contain an at-sign, and may not contain extraneous whitespace, and comments enclosed in parentheses are also not allowed.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>addressValue</i>
 is null.

 * System.ArgumentException:
AddressValue is empty.; Address doesn't contain a '@'sign; Invalid local part; Expected '@'sign after local part; Expected domain after '@'; Invalid domain; Address too long.

<a id="Domain"></a>
### Domain

    public string Domain { get; }

Gets the domain of this email address (the part after the "@" sign).

<b>Returns:</b>

The domain of this email address (the part after the "@" sign).

<a id="LocalPart"></a>
### LocalPart

    public string LocalPart { get; }

Gets the local part of this email address (the part before the "@" sign).

<b>Returns:</b>

The local part of this email address (the part before the "@" sign).

<a id="Equals_object"></a>
### Equals

    public override bool Equals(
        object obj);

Determines whether this object and another object are equal.

<b>Parameters:</b>

 * <i>obj</i>: The parameter  <i>obj</i>
 is an arbitrary object.

<b>Return Value:</b>

 `true`  if this object and another object are equal; otherwise,  `false` .

<a id="GetHashCode"></a>
### GetHashCode

    public override int GetHashCode();

Returns a hash code for this address object. No application or process identifiers are used in the hash code calculation.

<b>Return Value:</b>

A hash code for this instance.

<a id="ToString"></a>
### ToString

    public override string ToString();

Converts this address object to a text string.

<b>Return Value:</b>

A string representation of this object.
