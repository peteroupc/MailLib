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
    /// <summary>Represents an email address and a name for that address.</summary>
  public class NamedAddress {
    private string name;

    /// <summary>Gets the display name for this email address.</summary>
    /// <value>The display name for this email address.</value>
    public string Name {
      get {
        return this.name;
      }
    }

    private Address address;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public Address Address {
      get {
        return this.address;
      }
    }

    private bool isGroup;

    /// <summary>Gets a value indicating whether this represents a group of
    /// addresses rather than a single address.</summary>
    /// <value>True if this represents a group of addresses; otherwise,
    /// false..</value>
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

    /// <summary>Initializes a new instance of the NamedAddress class.</summary>
    /// <param name='address'>A string object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
    public NamedAddress(string address) {
      if (address == null) {
        throw new ArgumentNullException("address");
      }
      var tokener = new Tokener();
      if (HeaderParser.ParseAddress(address, 0, address.Length, tokener) !=
        address.Length) {
        throw new ArgumentException("Address has an invalid syntax.");
      }
      NamedAddress na = HeaderParserUtility.ParseAddress(address, 0,
        address.Length, tokener.GetTokens());
      if (na == null) {
        throw new ArgumentException("Address has an invalid syntax.");
      }
      this.name = na.name;
      this.address = na.address;
      this.groupAddresses = na.groupAddresses;
    }

    /// <summary>Initializes a new instance of the NamedAddress class using the
    /// given display name and email address.</summary>
    /// <param name='displayName'>A string object.</param>
    /// <param name='address'>Another string object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
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

    /// <summary>Initializes a new instance of the NamedAddress class.</summary>
    /// <param name='displayName'>A string object.</param>
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

    /// <summary>Initializes a new instance of the NamedAddress class using the
    /// given name and an email address made up of its local part and
    /// domain.</summary>
    /// <param name='displayName'>A string object.</param>
    /// <param name='localPart'>Another string object.</param>
    /// <param name='domain'>A string object. (3).</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='localPart'/> or <paramref name='domain'/> is null.</exception>
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

    /// <summary>Initializes a new instance of the NamedAddress class.</summary>
    /// <param name='groupName'>A string object.</param>
    /// <param name='mailboxes'>An IList object.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='groupName'/> or <paramref name='mailboxes'/> is null.</exception>
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

    /// <summary>Gets a list of address that make up the group, if this object
    /// represents a group, or an empty list otherwise.</summary>
    /// <value>A list of address that make up the group, if this object represents a
    /// group, or an empty list otherwise.</value>
    public IList<NamedAddress> GroupAddresses {
      get {
        return new

  System.Collections.ObjectModel.ReadOnlyCollection<NamedAddress>(this.groupAddresses);
      }
    }
  }
}
