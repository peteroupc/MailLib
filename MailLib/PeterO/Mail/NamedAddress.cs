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
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.GetHashCode"]/*'/>
    public override int GetHashCode() {
      var hashCode = -1524613162;
      if (displayName != null) {
        for (var i = 0; i < displayName.Length; ++i) {
          hashCode *= -1521134295 + displayName[i];
        }
      }
      hashCode *= -1521134295 + (address == null ? 0 :
        address.GetHashCode());
      hashCode *= -1521134295 + (isGroup ? 0 : 1);
      if (groupAddresses != null) {
        hashCode *= -1521134295 + (groupAddresses.Count);
      }
      return hashCode;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var other = obj as NamedAddress;
      return other != null &&
      (displayName == null ? other.displayName == null :
        displayName.Equals(other.displayName)) &&
   (address == null ? other.address == null : address.Equals(other.address))&&
        isGroup == other.isGroup &&
          (!isGroup || CollectionUtilities.ListEquals(groupAddresses,
            other.groupAddresses));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.AddressesEqual(PeterO.Mail.NamedAddress)"]/*'/>
    public bool AddressesEqual(NamedAddress na) {
      if (na == null || isGroup != na.isGroup) {
 return false;
}
      if (isGroup) {
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.ToString"]/*'/>
    public override string ToString() {
      if (this.isGroup) {
#if DEBUG
if (this.displayName == null) {
  throw new InvalidOperationException("this");
}
#endif

        var builder = new StringBuilder();
  builder.Append(HeaderParserUtility.QuoteValueIfNeeded(this.displayName));
        builder.Append(": ");
        var first = true;
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
      if (this.displayName == null) {
        return this.address.ToString();
      } else {
        string addressString = this.address.ToString();
        if (addressString.Length > 990) {
          // Give some space to ease line wrapping
          return HeaderParserUtility.QuoteValueIfNeeded(this.displayName) +
          " < " + addressString + " >";
        }
        return HeaderParserUtility.QuoteValueIfNeeded(this.displayName) +
        " <" + addressString + ">";
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String)"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,System.String)"]/*'/>
    public NamedAddress(string displayName, string address) {
      if (address == null) {
        throw new ArgumentNullException(nameof(address));
      }
      this.displayName = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = new Address(address);
      this.isGroup = false;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,PeterO.Mail.Address)"]/*'/>
    public NamedAddress(string displayName, Address address) {
      if (address == null) {
        throw new ArgumentNullException(nameof(address));
      }
      this.displayName = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = address;
      this.isGroup = false;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,System.String,System.String)"]/*'/>
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,System.Collections.Generic.IList{PeterO.Mail.NamedAddress})"]/*'/>
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
      this.groupAddresses = new List<NamedAddress>(mailboxes);
    }

    private readonly IList<NamedAddress> groupAddresses;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.NamedAddress.GroupAddresses"]/*'/>
    public IList<NamedAddress> GroupAddresses {
      get {
        return new
    System.Collections.ObjectModel.ReadOnlyCollection<NamedAddress>(
  this.groupAddresses);
      }
    }
  }
}
