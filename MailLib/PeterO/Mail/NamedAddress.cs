/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail {
  /// <summary>Represents an email address and a name for that address.
  /// Can represent a group of email addresses instead.</summary>
  public class NamedAddress {
    private readonly string displayName;

    /// <summary>Generates a string containing the display names of the
    /// given named-address objects, separated by commas. The generated
    /// string is intended to be displayed to end users, and is not
    /// intended to be parsed by computer programs. If a named address has
    /// no display name, its email address is used as the display
    /// name.</summary>
    /// <param name='addresses'>A list of named address objects.</param>
    /// <returns>A string containing the display names of the given
    /// named-address objects, separated by commas.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='addresses'/> is null.</exception>
    public static string ToDisplayStringShort(IList<NamedAddress> addresses) {
      if (addresses == null) {
        throw new ArgumentNullException(nameof(addresses));
      }
      var sb = new StringBuilder();
      for (var i = 0; i < addresses.Count; ++i) {
        if (i > 0) {
          sb.Append(", ");
        }
        sb.Append(addresses[i].Name);
      }
      return sb.ToString();
    }

    /// <summary>Generates a string containing the display names and email
    /// addresses of the given named-address objects, separated by commas.
    /// The generated string is intended to be displayed to end users, and
    /// is not intended to be parsed by computer programs.</summary>
    /// <param name='addresses'>A list of named address objects.</param>
    /// <returns>A string containing the display names and email addresses
    /// of the given named-address objects, separated by commas.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='addresses'/> is null.</exception>
    public static string ToDisplayString(IList<NamedAddress> addresses) {
      if (addresses == null) {
        throw new ArgumentNullException(nameof(addresses));
      }
      var sb = new StringBuilder();
      for (var i = 0; i < addresses.Count; ++i) {
        if (i > 0) {
          sb.Append(", ");
        }
        sb.Append(addresses[i].ToDisplayString());
      }
      return sb.ToString();
    }

    /// <summary>Generates a list of NamedAddress objects from a
    /// comma-separated list of addresses. Each address must follow the
    /// syntax accepted by the one-argument constructor of
    /// NamedAddress.</summary>
    /// <param name='addressValue'>A comma-separated list of addresses in
    /// the form of a text string.</param>
    /// <returns>A list of addresses generated from the <paramref
    /// name='addressValue'/> parameter.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='addressValue'/> is null.</exception>
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

    /// <summary>Calculates the hash code of this object. The exact
    /// algorithm used by this method is not guaranteed to be the same
    /// between versions of this library, and no application or process IDs
    /// are used in the hash code calculation.</summary>
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
    /// equal. For groups, the named addresses (display name/email address
    /// pairs) must be equal and in the same order in both
    /// objects.</summary>
    /// <param name='obj'>An arbitrary object to compare with this
    /// one.</param>
    /// <returns><c>true</c> if this object and another object are equal
    /// and have the same type; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) {
      var other = obj as NamedAddress;
      return other != null &&
        (this.displayName == null ? other.displayName == null :
          this.displayName.Equals(other.displayName,
  StringComparison.Ordinal)) && (this.address == null ? other.address == null :
          this.address.Equals(other.address)) && this.isGroup ==
other.isGroup &&
        (!this.isGroup || CollectionUtilities.ListEquals(
          this.groupAddresses,
          other.groupAddresses));
    }

    /// <summary>Determines whether the email addresses stored this object
    /// are the same between this object and the given object, regardless
    /// of the display names they store. For groups, the email addresses
    /// must be equal and in the same order in both objects.</summary>
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
        // TODO: Sort group addresses
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

    /// <summary>Gets the display name for this email address.</summary>
    /// <value>The display name for this email address. Returns null if the
    /// display name is absent.</value>
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

    /// <summary>Converts this named-address object to a text string
    /// intended for display to end users. The returned string is not
    /// intended to be parsed by computer programs.</summary>
    /// <returns>A text string of this named-address object, intended for
    /// display to end-users.</returns>
    public string ToDisplayString() {
      if (this.IsGroup) {
        var sb = new StringBuilder();
        sb.Append(this.displayName).Append(": ");
        var first = true;
        foreach (NamedAddress groupAddress in this.groupAddresses) {
          if (!first) {
            sb.Append("; ");
          }
          first = false;
          sb.Append(groupAddress.ToDisplayString());
        }
        sb.Append(';');
        return sb.ToString();
      } else if (this.displayName == null) {
        return this.address.ToString();
      } else {
        var sb = new StringBuilder();
        sb.Append(this.displayName).Append(" <")
        .Append(this.address.ToString()).Append('>');
        return sb.ToString();
      }
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.NamedAddress'/> class. Examples:
    /// <list>
    /// <item><c>john@example.com</c></item>
    /// <item><c>"John Doe" &lt;john@example.com&gt;</c></item>
    ///
    ///   <item><c>=?utf-8?q?John</c><c>=</c><c>27s_Office?=&lt;john@example.com&gt;</c></item>
    /// <item><c>John &lt;john@example.com&gt;</c></item>
    /// <item><c>"Group" : Tom &lt;tom@example.com&gt;, Jane
    /// &lt;jane@example.com&gt;;</c></item></list></summary>
    /// <param name='address'>A text string identifying a single email
    /// address or a group of email addresses. Comments, or text within
    /// parentheses, can appear. Multiple email addresses are not allowed
    /// unless they appear in the group syntax given earlier. Encoded words
    /// under RFC 2047 that appear within comments or display names will be
    /// decoded.
    /// <para>An RFC 2047 encoded word consists of "=?", a character
    /// encoding name, such as <c>utf-8</c>, either "?B?" or "?Q?" (in
    /// upper or lower case), a series of bytes in the character encoding,
    /// further encoded using B or Q encoding, and finally "?=". B encoding
    /// uses Base64, while in Q encoding, spaces are changed to "_", equals
    /// are changed to "=3D", and most bytes other than the basic digits 0
    /// to 9 (0x30 to 0x39) and the basic letters A/a to Z/z (0x41 to 0x5a,
    /// 0x61 to 0x7a) are changed to "=" followed by their 2-digit
    /// hexadecimal form. An encoded word's maximum length is 75
    /// characters. See the third example.</para>.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
    /// <exception cref='ArgumentException'>Address has an invalid syntax.;
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
    /// <see cref='PeterO.Mail.NamedAddress'/> class using the given
    /// display name and email address.</summary>
    /// <param name='displayName'>The display name of the email address.
    /// Can be null or empty. Encoded words under RFC 2047 will not be
    /// decoded.</param>
    /// <param name='address'>An email address.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
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
    /// <see cref='PeterO.Mail.NamedAddress'/> class using the given
    /// display name and email address.</summary>
    /// <param name='displayName'>The display name of the email address.
    /// Can be null or empty. Encoded words under RFC 2047 will not be
    /// decoded.</param>
    /// <param name='address'>An email address.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='address'/> is null.</exception>
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
    /// <see cref='PeterO.Mail.NamedAddress'/> class using the given name
    /// and an email address made up of its local part and
    /// domain.</summary>
    /// <param name='displayName'>The display name of the email address.
    /// Can be null or empty.</param>
    /// <param name='localPart'>The local part of the email address (before
    /// the "@").</param>
    /// <param name='domain'>The domain of the email address (before the
    /// "@").</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='localPart'/> or <paramref name='domain'/> is
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
    /// <see cref='PeterO.Mail.NamedAddress'/> class. Takes a group name
    /// and several named email addresses as parameters, and forms a group
    /// with them.</summary>
    /// <param name='groupName'>The group's name.</param>
    /// <param name='mailboxes'>A list of named addresses that make up the
    /// group.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='groupName'/> or <paramref name='mailboxes'/> is
    /// null.</exception>
    /// <exception cref='ArgumentException'>GroupName is empty.; A mailbox
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
