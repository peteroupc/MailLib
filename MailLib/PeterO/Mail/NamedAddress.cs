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
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.NamedAddress"]/*'/>
  public class NamedAddress {
    private readonly string displayName;

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.NamedAddress.ParseAddresses(System.String)"]/*'/>
    public static IList<NamedAddress> ParseAddresses(string addressValue) {
      var list = new List<NamedAddress>();
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.GetHashCode"]/*'/>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.NamedAddress.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var other = obj as NamedAddress;
      return other != null &&
      (this.displayName == null ? other.displayName == null :
        this.displayName.Equals(other.displayName)) &&
   (this.address == null ? other.address == null :
     this.address.Equals(other.address)) && this.isGroup == other.isGroup &&
          (!this.isGroup || CollectionUtilities.ListEquals(
            this.groupAddresses,
            other.groupAddresses));
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Mail.NamedAddress.AddressesEqual(PeterO.Mail.NamedAddress)"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.NamedAddress.Name"]/*'/>
    public string Name {
      get {
        return (this.displayName == null) ? ((this.address == null) ?
                 String.Empty : this.address.ToString()) : this.displayName;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.NamedAddress.DisplayName"]/*'/>
    public string DisplayName {
      get {
        return this.displayName;
      }
    }

    private readonly Address address;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.NamedAddress.Address"]/*'/>
    public Address Address {
      get {
        return this.address;
      }
    }

    private readonly bool isGroup;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.NamedAddress.IsGroup"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.ToString"]/*'/>
    public override string ToString() {
      var enc = new HeaderEncoder(Message.MaxRecHeaderLineLength, 15);
      this.AppendThisAddress(enc);
      return enc.ToString();
    }

    /// <xmlbegin id='10'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='NamedAddress'/> class.</summary>
    /// <param name='address'>The parameter <paramref name='address'/> is a
    /// text string.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='address'/> is null.</exception>
    /// <exception cref='T:System.ArgumentException'>Address has an invalid
    /// syntax.; Address has an invalid syntax.</exception>
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

    /// <xmlbegin id='11'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='NamedAddress'/> class.</summary>
    /// <param name='displayName'>The parameter <paramref
    /// name='displayName'/> is a text string.</param>
    /// <param name='address'>The parameter <paramref name='address'/> is a
    /// text string.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='address'/> is null.</exception>
    public NamedAddress(string displayName, string address) {
      if (address == null) {
        throw new ArgumentNullException(nameof(address));
      }
      this.displayName = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = new Address(address);
      this.isGroup = false;
    }

    /// <xmlbegin id='12'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='NamedAddress'/> class.</summary>
    /// <param name='displayName'>The parameter <paramref
    /// name='displayName'/> is a text string.</param>
    /// <param name='address'>The parameter <paramref name='address'/> is
    /// an Address object.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='address'/> is null.</exception>
    public NamedAddress(string displayName, Address address) {
      if (address == null) {
        throw new ArgumentNullException(nameof(address));
      }
      this.displayName = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = address;
      this.isGroup = false;
    }

    /// <xmlbegin id='13'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='NamedAddress'/> class.</summary>
    /// <param name='displayName'>The parameter <paramref
    /// name='displayName'/> is a text string.</param>
    /// <param name='localPart'>The parameter <paramref name='localPart'/>
    /// is a text string.</param>
    /// <param name='domain'>The parameter <paramref name='domain'/> is a
    /// text string.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='localPart'/> or <paramref name='domain'/> is
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

    /// <xmlbegin id='14'/>
    /// <summary>Initializes a new instance of the
    /// <see cref='NamedAddress'/> class.</summary>
    /// <param name='groupName'>The parameter <paramref name='groupName'/>
    /// is a text string.</param>
    /// <param name='mailboxes'>The parameter <paramref name='mailboxes'/>
    /// is an IList object.</param>
    /// <exception cref='T:System.ArgumentNullException'>The parameter
    /// <paramref name='groupName'/> or <paramref name='mailboxes'/> is
    /// null.</exception>
    /// <exception cref='T:System.ArgumentException'>GroupName is empty.; A
    /// mailbox in the list is a group.</exception>
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

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Mail.NamedAddress.GroupAddresses"]/*'/>
    public IList<NamedAddress> GroupAddresses {
      get {
        return new
    System.Collections.ObjectModel.ReadOnlyCollection<NamedAddress>(
  this.groupAddresses);
      }
    }
  }
}
