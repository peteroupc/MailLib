## PeterO.Mail.Address

    public class Address

 Represents an email address.  ### Member Summary
* <code>[Domain](#Domain)</code> - Gets the domain of this email address (the part after the "@" sign).
* <code>[Equals(object)](#Equals_object)</code> - Determines whether this object and another object are equal.
* <code>[GetHashCode()](#GetHashCode)</code> - Returns a hash code for this address object.
* <code>[LocalPart](#LocalPart)</code> - Gets the local part of this email address (the part before the "@" sign).
* <code>[ToString()](#ToString)</code> - Converts this address object to a text string.

<a id="Domain"></a>
### Domain

    public string Domain { get; }

 Gets the domain of this email address (the part after the "@" sign).  <b>Returns:</b>

The domain of this email address (the part after the "@" sign).

<a id="LocalPart"></a>
### LocalPart

    public string LocalPart { get; }

 Gets the local part of this email address (the part before the "@" sign).  <b>Returns:</b>

The local part of this email address (the part before the "@" sign).

<a id="Equals_object"></a>
### Equals

    public override bool Equals(
        object obj);

 Determines whether this object and another object are equal.  <b>Parameters:</b>

 * <i>obj</i>: The parameter  <i>obj</i>
 is an arbitrary object.

<b>Return Value:</b>

 `true` true if this object and another object are equal; otherwise,  `false` false .

<a id="GetHashCode"></a>
### GetHashCode

    public override int GetHashCode();

 Returns a hash code for this address object. No application or process identifiers are used in the hash code calculation.  <b>Return Value:</b>

A hash code for this instance.

<a id="ToString"></a>
### ToString

    public override string ToString();

 Converts this address object to a text string.  <b>Return Value:</b>

A string representation of this object.
