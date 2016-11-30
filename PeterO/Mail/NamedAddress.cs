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
    /// path='docs/doc[@name="P:PeterO.Mail.NamedAddress.Name"]/*'/>
    public string Name {
      get {
        if (displayName == null) {
    return (address == null) ? (String.Empty) : (address.ToString());
  }
        return displayName;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.NamedAddress.DisplayName"]/*'/>
    public string DisplayName {
    get {
      return displayName;
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
if ((this.displayName) == null) {
  throw new ArgumentNullException("this.displayName");
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
      if (displayName == null) {
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,System.String)"]/*'/>
    public NamedAddress(string displayName, string address) {
      if (address == null) {
        throw new ArgumentNullException("address");
      }
      this.name = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = new Address(address);
      this.isGroup = false;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,PeterO.Mail.Address)"]/*'/>
    public NamedAddress(string displayName, Address address) {
      if (address == null) {
        throw new ArgumentNullException("address");
      }
      this.name = displayName;
      this.groupAddresses = new List<NamedAddress>();
      this.address = address;
      this.isGroup = false;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,System.String,System.String)"]/*'/>
    public NamedAddress(string displayName, string localPart, string domain) {
      if (localPart == null) {
        throw new ArgumentNullException("localPart");
      }
      if (domain == null) {
        throw new ArgumentNullException("domain");
      }
      this.address = new Address(localPart, domain);
      this.groupAddresses = new List<NamedAddress>();
      this.name = displayName;
      this.isGroup = false;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.NamedAddress.#ctor(System.String,System.Collections.Generic.IList{PeterO.Mail.NamedAddress})"]/*'/>
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
