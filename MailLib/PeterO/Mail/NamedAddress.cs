/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail {
    /// <summary>Represents an email address and a name for that address.
    /// Can represent a group of email addresses instead.</summary>
  public class NamedAddress {
    private readonly string displayName;

    /// <summary>Generates a list of NamedAddress objects from a
    /// comma-separated list of addresses. Each address must follow the
    /// syntax accepted by the one-argument constructor of
    /// NamedAddress.</summary>
    /// <param name='addressValue'>A comma-separate list of addresses in
    /// the form of a text string.</param>
    /// <returns>A list of addresses generated from the <paramref name='addressValue'/> parameter.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='addressValue'/> is null.</exception>
    public static IList<NamedAddress> ParseAddresses(string addressValue) {
      var list = new List<NamedAddress>();

if (addressValue == null) {
  return list;
}
      if (String.IsNullOrEmpty(addressValue)) {
  return list;
}
      var tokener = new Tokener();
      if (
          HeaderParser.ParseHeaderTo(
            addressValue,
            0,
            addressValue.Length,
            tokener) != addressValue.Length) {
        // Invalid syntax
        return list;
      }
      list.AddRange(
        HeaderParserUtility.ParseAddressList(
          addressValue,
          0,
          addressValue.Length,
          tokener.GetTokens()));
      return list;
    }

    /// <summary>Calculates the hash code of this object. No application or
    /// process IDs are used in the hash code calculation.</summary>
    /// <returns>A 32-bit hash code.</returns>
    public override int GetHashCode() {
      var hashCode = -1524613162;
      if (this.displayName != null) {
        for (var i = 0; i < this.displayName.Length; ++i) {
          hashCode *= -1521134295 + this.displayName[i];
        }
      }
      hashCode *= -1521134295 + (this.address == null ? 0 :
        this.address.GetHashCode());
      hashCode *= -1521134295 + (this.isGroup ? 0 : 1);
      if (this.groupAddresses != null) {
        hashCode *= -1521134295 + this.groupAddresses.Count;
      }
      return hashCode;
    }

    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns><c>true</c> if this object and another object are equal;
    /// otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) {
      var other = obj as NamedAddress;
      return other != null &&
      (this.displayName == null ? other.displayName == null :
        this.displayName.Equals(other.displayName, StringComparison.Ordinal)) &&
   (this.address == null ? other.address == null :
     this.address.Equals(other.address)) && this.isGroup == other.isGroup &&
          (!this.isGroup || CollectionUtilities.ListEquals(
            this.groupAddresses,
            other.groupAddresses));
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='na'>A named address object to compare with this one.
    /// Can be null.</param>
    /// <returns>Either <c>true</c> or <c>false</c>.</returns>
    public bool AddressesEqual(NamedAddress na) {
      if (na == null || this.isGroup != na.isGroup) {
        return false;
      }
      if (this.isGroup) {
        if (this.groupAddresses.Count != na.GroupAddresses.Count) {
          return false;
        }
        for (var i = 0; i < this.groupAddresses.Count; ++i) {
          NamedAddress a1 = this.groupAddresses[i];
          NamedAddress a2 = na.groupAddresses[i];
          if (!a1.AddressesEqual(a2)) {
            return false;
          }
        }
        return true;
      } else {
        return this.address.Equals(na.address);
      }
    }

    /// <summary>Gets the display name for this email address, or the email
    /// address's value if the display name is null. Returns an empty
    /// string if the address and display name are null.</summary>
    /// <value>The name for this email address.</value>
    public string Name {
      get {
        return (this.displayName == null) ? ((this.address == null) ?
                 String.Empty : this.address.ToString()) : this.displayName;
      }
    }

    /// <summary>Gets the display name for this email address. Returns null
    /// if the display name is absent.</summary>
    /// <value>The display name for this email address.</value>
    public string DisplayName {
      get {
        return this.displayName;
      }
    }

    private readonly Address address;

    /// <summary>Gets the email address associated with this
    /// object.</summary>
    /// <value>The email address associated with this object. This value is
    /// null if this object represents a group of addresses
    /// instead.</value>
    public Address Address {
      get {
        return this.address;
      }
    }

    private readonly bool isGroup;

    /// <summary>Gets a value indicating whether this represents a group of
    /// addresses rather than a single address.</summary>
    /// <value><c>true</c> If this represents a group of addresses;
    /// otherwise, <c>false</c>.</value>
    public bool IsGroup {
      get {
        return this.isGroup;
      }
    }

    internal void AppendThisAddress(HeaderEncoder enc) {
      if (this.IsGroup) {
        enc.AppendPhrase(this.displayName);
        enc.AppendSymbol(":");
        enc.AppendSpaceIfNeeded();
        var first = true;
        foreach (NamedAddress groupAddress in this.groupAddresses) {
          if (!first) {
            enc.AppendSymbol(",");
            enc.AppendSpaceIfNeeded();
          }
          first = false;
          groupAddress.AppendThisAddress(enc);
        }
        enc.AppendSymbol(";");
      } else if (this.displayName == null) {
        this.address.AppendThisAddress(enc);
      } else {
        enc.AppendPhrase(this.displayName);
        enc.AppendSpaceIfNeeded();
        enc.AppendSymbol("<");
        this.address.AppendThisAddress(enc);
        enc.AppendSymbol(">");
      }
    }

    /// <summary>Converts this object to a text string. This will generally
    /// be the form of this NamedAddress object as it could appear in a
    /// "To" header field.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      var enc = new HeaderEncoder(Message.MaxRecHeaderLineLength, 15);
      this.AppendThisAddress(enc);
      return enc.ToString();
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.NamedAddress'/> class.</summary>
    /// <param name='address'>The parameter <paramref name='address'/> is a
    /// text string.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='address'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>Address has an invalid syntax.;
    /// Address has an invalid syntax.</exception>
    public NamedAddress(string address) {
      if (address == null) {
        throw new ArgumentNullException(nameof(address));
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
      this.displayName = na.displayName;
      this.address = na.address;
      this.groupAddresses = na.groupAddresses;
      this.isGroup = na.isGroup;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.NamedAddress'/> class.</summary>
    /// <param name='displayName'>The parameter <paramref name='displayName'/> is a text string.</param>
    /// <param name='address'>The parameter <paramref name='address'/> is a
    /// text string.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='address'/> is null.</exception>
    public NamedAddress(string displayName, string address) {
      if (address == null) {
        throw new ArgumentNullException(nameof(address));
      }
      this.displayName = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = new Address(address);
      this.isGroup = false;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.NamedAddress'/> class.</summary>
    /// <param name='displayName'>The parameter <paramref name='displayName'/> is a text string.</param>
    /// <param name='address'>The parameter <paramref name='address'/> is
    /// an Address object.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='address'/> is null.</exception>
    public NamedAddress(string displayName, Address address) {
      if (address == null) {
        throw new ArgumentNullException(nameof(address));
      }
      this.displayName = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = address;
      this.isGroup = false;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.NamedAddress'/> class.</summary>
    /// <param name='displayName'>The parameter <paramref name='displayName'/> is a text string.</param>
    /// <param name='localPart'>The parameter <paramref name='localPart'/>
    /// is a text string.</param>
    /// <param name='domain'>The parameter <paramref name='domain'/> is a
    /// text string.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='localPart'/> or <paramref name='domain'/> is
    /// null.</exception>
    public NamedAddress(string displayName, string localPart, string domain) {
      if (localPart == null) {
        throw new ArgumentNullException(nameof(localPart));
      }
      if (domain == null) {
        throw new ArgumentNullException(nameof(domain));
      }
      this.address = new Address(localPart, domain);
      this.groupAddresses = new List<NamedAddress>();
      this.displayName = displayName;
      this.isGroup = false;
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.NamedAddress'/> class.</summary>
    /// <param name='groupName'>The parameter <paramref name='groupName'/>
    /// is a text string.</param>
    /// <param name='mailboxes'>The parameter <paramref name='mailboxes'/>
    /// is an IList object.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='groupName'/> or <paramref name='mailboxes'/> is
    /// null.</exception>
    /// <exception cref='System.ArgumentException'>GroupName is empty.; A mailbox
    /// in the list is a group.</exception>
    public NamedAddress(string groupName, IList<NamedAddress> mailboxes) {
      if (groupName == null) {
        throw new ArgumentNullException(nameof(groupName));
      }
      if (groupName.Length == 0) {
        throw new ArgumentException("groupName is empty.");
      }
      if (mailboxes == null) {
        throw new ArgumentNullException(nameof(mailboxes));
      }
      this.isGroup = true;
      this.displayName = groupName;
      foreach (NamedAddress mailbox in mailboxes) {
        if (mailbox.IsGroup) {
          throw new ArgumentException("A mailbox in the list is a group");
        }
      }
      this.address = null;
      this.groupAddresses = new List<NamedAddress>();
      foreach (NamedAddress mailbox in mailboxes) {
        this.groupAddresses.Add(mailbox);
      }
    }

    private readonly IList<NamedAddress> groupAddresses;

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
