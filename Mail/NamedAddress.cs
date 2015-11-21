/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail {
    /// <summary>Represents an email address and a name for that address.
    /// Can represent a group of email addresses instead.</summary>
  public class NamedAddress {
    private string name;

    /// <summary>Gets the display name for this email address (or the email
    /// address's value if the display name is absent).</summary>
    /// <value>The display name for this email address.</value>
    public string Name {
      get {
        return this.name;
      }
    }

    private Address address;

    /// <summary>Gets the email address associated with this
    /// object.</summary>
    /// <value>The email address associated with this object.</value>
    public Address Address {
      get {
        return this.address;
      }
    }

    private bool isGroup;

    /// <summary>Gets a value indicating whether this represents a group of
    /// addresses rather than a single address.</summary>
    /// <value>True if this represents a group of addresses; otherwise,
    /// false.</value>
    public bool IsGroup {
      get {
        return this.isGroup;
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      if (this.isGroup) {
        var builder = new StringBuilder();
        builder.Append(HeaderParserUtility.QuoteValueIfNeeded(this.name));
        builder.Append(": ");
        bool first = true;
        foreach (NamedAddress groupAddress in this.groupAddresses) {
          if (!first) {
            builder.Append(", ");
          }
          first = false;
          builder.Append(groupAddress.ToString());
        }
        builder.Append(";");
        return builder.ToString();
      }
      if (String.IsNullOrEmpty(this.name)) {
        return this.address.ToString();
      } else {
        string addressString = this.address.ToString();
        if (addressString.Equals(this.name)) {
          return addressString;
        }
        if (addressString.Length > 990) {
          // Give some space to ease line wrapping
          return HeaderParserUtility.QuoteValueIfNeeded(this.name) +
          " < " + addressString + " >";
        }
        return HeaderParserUtility.QuoteValueIfNeeded(this.name) +
        " <" + addressString + ">";
      }
    }

    /// <summary>Initializes a new instance of the NamedAddress class.
    /// Examples:
    /// <list>
    /// <item><c>john@example.com</c></item>
    /// <item><c>"John Doe" &lt;john@example.com&gt;</c></item>
    /// <item><c>=?utf-8?q?John</c><c>=</c><c>27s_Office?=
    /// &lt;john@example.com&gt;</c></item>
    /// <item><c>John &lt;john@example.com&gt;</c></item>
    /// <item><c>"Group" : Tom &lt;tom@example.com&gt;, Jane
    /// &lt;jane@example.com&gt;;</c></item></list></summary>
    /// <param name='address'>A string object identifying a single email
    /// address or a group of email addresses. Comments, or text within
    /// parentheses, can appear. Multiple email addresses are not allowed
    /// unless they appear in the group syntax given above. Encoded words
    /// under RFC 2047 that appear within comments or display names will be
    /// decoded.
    /// <para>An RFC 2047 encoded word consists of "=?", a character
    /// encoding name, such as <c>utf-8</c>, either "?B?" or "?Q?" (in
    /// upper or lower case), a series of bytes in the character encoding,
    /// further encoded using B or Q encoding, and finally "?=". B encoding
    /// uses Base64, while in Q encoding, spaces are changed to "_", equals
    /// are changed to "=3D" , and most bytes other than ASCII letters and
    /// digits are changed to "=" followed by their 2-digit hexadecimal
    /// form. An encoded word's maximum length is 75 characters. See the
    /// second example.</para>.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
    /// <exception cref='ArgumentException'>The named address has an
    /// invalid syntax.</exception>
    public NamedAddress(string address) {
      if (address == null) {
        throw new ArgumentNullException("address");
      }
      var tokener = new Tokener();
      if (HeaderParser.ParseAddress(address, 0, address.Length, tokener) !=
        address.Length) {
        throw new ArgumentException("Address has an invalid syntax.");
      }
      NamedAddress na = HeaderParserUtility.ParseAddress(
address,
0,
address.Length,
tokener.GetTokens());
      if (na == null) {
        throw new ArgumentException("Address has an invalid syntax.");
      }
      this.name = na.name;
      this.address = na.address;
      this.groupAddresses = na.groupAddresses;
      this.isGroup = na.isGroup;
    }

    /// <summary>Initializes a new instance of the NamedAddress class using
    /// the given display name and email address.</summary>
    /// <param name='displayName'>The address's display name. Can be null
    /// or empty, in which case the email address is used instead. Encoded
    /// words under RFC 2047 will not be decoded.</param>
    /// <param name='address'>An email address.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
    /// <exception cref='ArgumentException'>The display name or address has
    /// an invalid syntax.</exception>
    public NamedAddress(string displayName, string address) {
      if (String.IsNullOrEmpty(displayName)) {
        displayName = address;
      }
      if (address == null) {
        throw new ArgumentNullException("address");
      }
      this.name = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = new Address(address);
    }

    /// <summary>Initializes a new instance of the NamedAddress
    /// class.</summary>
    /// <param name='displayName'>A string object. If this value is null or
    /// empty, uses the email address as the display name. Encoded words
    /// under RFC 2047 will not be decoded.</param>
    /// <param name='address'>An email address.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
    public NamedAddress(string displayName, Address address) {
      if (address == null) {
        throw new ArgumentNullException("address");
      }
      if (String.IsNullOrEmpty(displayName)) {
        displayName = address.ToString();
      }
      this.name = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = address;
    }

    /// <summary>Initializes a new instance of the NamedAddress class using
    /// the given name and an email address made up of its local part and
    /// domain.</summary>
    /// <param name='displayName'>A string object. If this value is null or
    /// empty, uses the email address as the display name.</param>
    /// <param name='localPart'>The local part of the email address (before
    /// the "@").</param>
    /// <param name='domain'>The domain of the email address (before the
    /// "@").</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='localPart'/> or <paramref name='domain'/> is
    /// null.</exception>
    public NamedAddress(string displayName, string localPart, string domain) {
      if (localPart == null) {
        throw new ArgumentNullException("localPart");
      }
      if (domain == null) {
        throw new ArgumentNullException("domain");
      }
      this.address = new Address(localPart, domain);
      if (String.IsNullOrEmpty(displayName)) {
        displayName = this.address.ToString();
      }
      this.groupAddresses = new List<NamedAddress>();
      this.name = displayName;
    }

    /// <summary>Initializes a new instance of the NamedAddress class.
    /// Takes a group name and several named email addresses as parameters,
    /// and forms a group with them.</summary>
    /// <param name='groupName'>The group's name.</param>
    /// <param name='mailboxes'>A list of named addresses that make up the
    /// group.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='groupName'/> or <paramref name='mailboxes'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>The parameter <paramref
    /// name='groupName'/> is empty, or an item in the list is itself a
    /// group.</exception>
    public NamedAddress(string groupName, IList<NamedAddress> mailboxes) {
      if (groupName == null) {
        throw new ArgumentNullException("groupName");
      }
      if (groupName.Length == 0) {
        throw new ArgumentException("groupName is empty.");
      }
      if (mailboxes == null) {
        throw new ArgumentNullException("mailboxes");
      }
      this.isGroup = true;
      this.name = groupName;
      foreach (NamedAddress mailbox in mailboxes) {
        if (mailbox.IsGroup) {
          throw new ArgumentException("A mailbox in the list is a group");
        }
      }
      this.groupAddresses = new List<NamedAddress>(mailboxes);
    }

    private IList<NamedAddress> groupAddresses;

    /// <summary>Gets a read-only list of addresses that make up the group,
    /// if this object represents a group, or an empty list
    /// otherwise.</summary>
    /// <value>A list of addresses that make up the group, if this object
    /// represents a group, or an empty list otherwise.</value>
    public IList<NamedAddress> GroupAddresses {
      get {
        return new
    System.Collections.ObjectModel.ReadOnlyCollection<NamedAddress>(
  this.groupAddresses);
      }
    }
  }
}
