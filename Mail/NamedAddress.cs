/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail
{
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

    /// <summary>Gets a value indicating whether this represents a group
    /// of addresses rather than a single address.</summary>
    /// <value>Whether this represents a group of addresses.</value>
    public bool IsGroup {
      get {
        return this.isGroup;
      }
    }

    private static bool ShouldQuote(string str) {
      for (int i = 0; i<str.Length; ++i) {
        if (str[i] == '\\' || str[i] == '"') {
          return true;
        }
        if ((str[i] == ' ' || str[i] == '\t') && i+1<str.Length &&
            (str[i + 1] == ' ' || str[i+1] == '\t')) {
          // run of two or more space and/or tab
          return true;
        }
        if ((str[i] == '\r') && i+1<str.Length &&
            (str[i + 1] == '\n')) {
          // CRLF
          continue;
        }
        char c = str[i];
        // Has specials, or CTLs other than tab
        if ((c < 0x20 && c != '\t') || c == 0x7F || c == 0x28 || c == 0x29 || c == 0x3c || c == 0x3e ||
            c == 0x5b || c == 0x5d || c == 0x3a || c == 0x3b || c == 0x40 || c == 0x5c || c == 0x2c || c == 0x2e || c == '"') {
          return true;
        }
      }
      return false;
    }

    private static string QuoteValue(String str) {
      if (!ShouldQuote(str)) {
        return str;
      }
      StringBuilder builder = new StringBuilder();
      builder.Append('"');
      for (int i = 0; i<str.Length; ++i) {
        if (str[i] == '\\' || str[i] == '"') {
          builder.Append('\\');
          builder.Append(str[i]);
        } else {
          builder.Append(str[i]);
        }
      }
      builder.Append('"');
      return builder.ToString();
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      if (this.isGroup) {
        StringBuilder builder = new StringBuilder();
        builder.Append(QuoteValue(this.name));
        builder.Append(": ");
        bool first = true;
        foreach(NamedAddress address in this.groupAddresses) {
          if (!first) {
            builder.Append(", ");
          }
          first = false;
          builder.Append(address.ToString());
        }
        builder.Append(";");
        return builder.ToString();
      } else if (String.IsNullOrEmpty(this.name)) {
        return this.address.ToString();
      } else {
        string addressString = this.address.ToString();
        if (addressString.Equals(this.name)) {
          return addressString;
        } else {
          return QuoteValue(this.name)+" <"+addressString+">";
        }
      }
    }

    public NamedAddress(string address) : this(address, address) {
    }

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

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<NamedAddress> GroupAddresses {
      get {
        return new System.Collections.ObjectModel.ReadOnlyCollection<NamedAddress>(this.groupAddresses);
      }
    }
  }
}
