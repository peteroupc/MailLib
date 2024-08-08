package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

import java.util.*;

  /**
   * Represents an email address and a name for that address. Can represent a
   * group of email addresses instead.
   */
  public class NamedAddress {
    private final String displayName;

    /**
     * Generates a string containing the display names of the given named-address
     * objects, separated by commas. The generated string is intended to be
     * displayed to end users, and is not intended to be parsed by computer
     * programs. If a named address has no display name, its email address is used
     * as the display name.
     * @param addresses A list of named address objects.
     * @return A string containing the display names of the given named-address
     * objects, separated by commas.
     * @throws NullPointerException The parameter {@code addresses} is null.
     */
    public static String ToDisplayStringShort(List<NamedAddress> addresses) {
      if (addresses == null) {
        throw new NullPointerException("addresses");
      }
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < addresses.size(); ++i) {
        if (i > 0) {
          sb.append(", ");
        }
        sb.append(addresses.get(i).getName());
      }
      return sb.toString();
    }

    /**
     * Generates a string containing the display names and email addresses of the
     * given named-address objects, separated by commas. The generated string is
     * intended to be displayed to end users, and is not intended to be parsed by
     * computer programs.
     * @param addresses A list of named address objects.
     * @return A string containing the display names and email addresses of the
     * given named-address objects, separated by commas.
     * @throws NullPointerException The parameter {@code addresses} is null.
     */
    public static String ToDisplayString(List<NamedAddress> addresses) {
      if (addresses == null) {
        throw new NullPointerException("addresses");
      }
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < addresses.size(); ++i) {
        if (i > 0) {
          sb.append(", ");
        }
        sb.append(addresses.get(i).ToDisplayString());
      }
      return sb.toString();
    }

    /**
     * Generates a list of NamedAddress objects from a comma-separated list of
     * addresses. Each address must follow the syntax accepted by the one-argument
     * constructor of NamedAddress.
     * @param addressValue A comma-separated list of addresses in the form of a
     * text string.
     * @return A list of addresses generated from the {@code addressValue}
     * parameter.
     * @throws NullPointerException The parameter {@code addressValue} is null.
     */
    public static List<NamedAddress> ParseAddresses(String addressValue) {
      ArrayList<NamedAddress> list = new ArrayList<NamedAddress>();
      if (addressValue == null) {
        return list;
      }
      if (((addressValue) == null || (addressValue).length() == 0)) {
        return list;
      }
      Tokener tokener = new Tokener();
      if (
        HeaderParser.ParseHeaderTo(
          addressValue,
          0,
          addressValue.length(),
          tokener) != addressValue.length()) {
        // Invalid syntax
        return list;
      }
      list.addAll(
        HeaderParserUtility.ParseAddressList(
          addressValue,
          0,
          addressValue.length(),
          tokener.GetTokens()));
      return list;
    }

    /**
     * Calculates the hash code of this object. The exact algorithm used by this
     * method is not guaranteed to be the same between versions of this library,
     * and no application or process IDs are used in the hash code calculation.
     * @return A 32-bit hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = -1524613162;
      if (this.displayName != null) {
        for (int i = 0; i < this.displayName.length(); ++i) {
          valueHashCode *= -1521134295 + this.displayName.charAt(i);
        }
      }
      valueHashCode *= -1521134295 + (this.address == null ? 0 :
          this.address.hashCode());
      valueHashCode *= -1521134295 + (this.isGroup ? 0 : 1);
      if (this.groupAddresses != null) {
        valueHashCode *= -1521134295 + this.groupAddresses.size();
      }
      return valueHashCode;
    }

    /**
     * Determines whether this object and another object are equal. For groups, the
     * named addresses (display name/email address pairs) must be equal and in the
     * same order in both objects.
     * @param obj An arbitrary object to compare with this one.
     * @return {@code true} if this object and another object are equal and have
     * the same type; otherwise, {@code false}.
     */
    @Override public boolean equals(Object obj) {
      NamedAddress other = ((obj instanceof NamedAddress) ? (NamedAddress)obj : null);
      return other != null &&
        (this.displayName == null ? other.displayName == null :
          this.displayName.equals(other.displayName)) && (this.address == null ? other.address == null :
          this.address.equals(other.address)) && this.isGroup ==
other.isGroup &&
        (!this.isGroup || CollectionUtilities.ListEquals(
          this.groupAddresses,
          other.groupAddresses));
    }

    /**
     * Determines whether the email addresses stored this object are the same
     * between this object and the given object, regardless of the display names
     * they store. For groups, the email addresses must be equal and in the same
     * order in both objects.
     * @param na A named address object to compare with this one. Can be null.
     * @return Either {@code true} or {@code false}.
     */
    public boolean AddressesEqual(NamedAddress na) {
      if (na == null || this.isGroup != na.isGroup) {
        return false;
      }
      if (this.isGroup) {
        if (this.groupAddresses.size() != na.getGroupAddresses().size()) {
          return false;
        }
        // TODO: Sort group addresses
        for (int i = 0; i < this.groupAddresses.size(); ++i) {
          NamedAddress a1 = this.groupAddresses.get(i);
          NamedAddress a2 = na.groupAddresses.get(i);
          if (!a1.AddressesEqual(a2)) {
            return false;
          }
        }
        return true;
      } else {
        return this.address.equals(na.address);
      }
    }

    /**
     * Gets the display name for this email address, or the email address's value
     * if the display name is null. Returns an empty string if the address and
     * display name are null.
     * @return The name for this email address.
     */
    public final String getName() {
        return (this.displayName == null) ? ((this.address == null) ?
            "" : this.address.toString()) : this.displayName;
      }

    /**
     * Gets the display name for this email address.
     * @return The display name for this email address. Returns null if the display
     * name is absent.
     */
    public final String getDisplayName() {
        return this.displayName;
      }

    private final Address address;

    /**
     * Gets the email address associated with this object.
     * @return The email address associated with this object. This value is null if
     * this object represents a group of addresses instead.
     */
    public final Address getAddress() {
        return this.address;
      }

    private final boolean isGroup;

    /**
     * Gets a value indicating whether this represents a group of addresses rather
     * than a single address.
     * @return {@code true} If this represents a group of addresses; otherwise,
     * {@code false}.
     */
    public final boolean isGroup() {
        return this.isGroup;
      }

    void AppendThisAddress(HeaderEncoder enc) {
      if (this.isGroup()) {
        enc.AppendPhrase(this.displayName);
        enc.AppendSymbol(":");
        enc.AppendSpaceIfNeeded();
        boolean first = true;
        for (NamedAddress groupAddress : this.groupAddresses) {
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

    /**
     * Converts this object to a text string. This will generally be the form of
     * this NamedAddress object as it could appear in a "To" header field.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      HeaderEncoder enc = new HeaderEncoder(Message.MaxRecHeaderLineLength, 15);
      this.AppendThisAddress(enc);
      return enc.toString();
    }

    /**
     * Converts this named-address object to a text string intended for display to
     * end users. The returned string is not intended to be parsed by computer
     * programs.
     * @return A text string of this named-address object, intended for display to
     * end-users.
     */
    public String ToDisplayString() {
      if (this.isGroup()) {
        StringBuilder sb = new StringBuilder();
        sb.append(this.displayName).append(": ");
        boolean first = true;
        for (NamedAddress groupAddress : this.groupAddresses) {
          if (!first) {
            sb.append("; ");
          }
          first = false;
          sb.append(groupAddress.ToDisplayString());
        }
        sb.append(';');
        return sb.toString();
      } else if (this.displayName == null) {
        return this.address.toString();
      } else {
        StringBuilder sb = new StringBuilder();
        sb.append(this.displayName).append(" <")
        .append(this.address.toString()).append('>');
        return sb.toString();
      }
    }

    /**
     * <p>Initializes a new instance of the {@link
     * com.upokecenter.mail.NamedAddress} class. Examples: </p> <ul> <li> {@code
     * john@example.com}</li><li>{@code "John Doe"
     * &lt;john@example.com&gt;}</li><li> {@code =?utf-8?q?John}{@code =}{@code
     * 27s_Office?=&lt;john@example.com&gt;}</li><li> {@code John
     * &lt;john@example.com&gt;}</li><li>{@code "Group" : Tom
     * &lt;tom@example.com&gt;, Jane &lt;jane@example.com&gt;;}</li></ul>
     * @param address <p>A text string identifying a single email address or a
     * group of email addresses. Comments, or text within parentheses, can appear.
     * Multiple email addresses are not allowed unless they appear in the group
     * syntax given above. Encoded words under RFC 2047 that appear within comments
     * or display names will be decoded. </p><p>An RFC 2047 encoded word consists
     * of "=?", a character encoding name, such as {@code utf-8}, either "?B?" or
     * "?Q?" (in upper or lower case), a series of bytes in the character encoding,
     * further encoded using B or Q encoding, and finally "?=". B encoding uses
     * Base64, while in Q encoding, spaces are changed to "_", equals are changed
     * to "=3D", and most bytes other than the basic digits 0 to 9 (0x30 to 0x39)
     * and the basic letters A/a to Z/z (0x41 to 0x5a, 0x61 to 0x7a) are changed to
     * "=" followed by their 2-digit hexadecimal form. An encoded word's maximum
     * length is 75 characters. See the third example.</p>.
     * @throws NullPointerException The parameter {@code address} is null.
     * @throws IllegalArgumentException Address has an invalid syntax.; Address has an
     * invalid syntax.
     */
    public NamedAddress(String address) {
      if (address == null) {
        throw new NullPointerException("address");
      }
      Tokener tokener = new Tokener();
      if (HeaderParser.ParseAddress(address, 0, address.length(), tokener) !=
        address.length()) {
        throw new IllegalArgumentException("Address has an invalid syntax.");
      }
      NamedAddress na = HeaderParserUtility.ParseAddress(
          address,
          0,
          address.length(),
          tokener.GetTokens());
      if (na == null) {
        throw new IllegalArgumentException("Address has an invalid syntax.");
      }
      this.displayName = na.displayName;
      this.address = na.address;
      this.groupAddresses = na.groupAddresses;
      this.isGroup = na.isGroup;
    }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.NamedAddress}
     * class using the given display name and email address.
     * @param displayName The display name of the email address. Can be null or
     * empty. Encoded words under RFC 2047 will not be decoded.
     * @param address An email address.
     * @throws NullPointerException The parameter {@code address} is null.
     */
    public NamedAddress(String displayName, String address) {
      if (address == null) {
        throw new NullPointerException("address");
      }
      this.displayName = displayName;
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.address = new Address(address);
      this.isGroup = false;
    }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.NamedAddress}
     * class using the given display name and email address.
     * @param displayName The display name of the email address. Can be null or
     * empty. Encoded words under RFC 2047 will not be decoded.
     * @param address An email address.
     * @throws NullPointerException The parameter {@code address} is null.
     */
    public NamedAddress(String displayName, Address address) {
      if (address == null) {
        throw new NullPointerException("address");
      }
      this.displayName = displayName;
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.address = address;
      this.isGroup = false;
    }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.NamedAddress}
     * class using the given name and an email address made up of its local part
     * and domain.
     * @param displayName The display name of the email address. Can be null or
     * empty.
     * @param localPart The local part of the email address (before the "@").
     * @param domain The domain of the email address (before the "@").
     * @throws NullPointerException The parameter {@code localPart} or {@code
     * domain} is null.
     */
    public NamedAddress(String displayName, String localPart, String domain) {
      if (localPart == null) {
        throw new NullPointerException("localPart");
      }
      if (domain == null) {
        throw new NullPointerException("domain");
      }
      this.address = new Address(localPart, domain);
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.displayName = displayName;
      this.isGroup = false;
    }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.NamedAddress}
     * class. Takes a group name and several named email addresses as parameters,
     * and forms a group with them.
     * @param groupName The group's name.
     * @param mailboxes A list of named addresses that make up the group.
     * @throws NullPointerException The parameter {@code groupName} or {@code
     * mailboxes} is null.
     * @throws IllegalArgumentException GroupName is empty.; A mailbox in the list is a
     * group.
     */
    public NamedAddress(String groupName, List<NamedAddress> mailboxes) {
      if (groupName == null) {
        throw new NullPointerException("groupName");
      }
      if (groupName.length() == 0) {
        throw new IllegalArgumentException("groupName is empty.");
      }
      if (mailboxes == null) {
        throw new NullPointerException("mailboxes");
      }
      this.isGroup = true;
      this.displayName = groupName;
      for (NamedAddress mailbox : mailboxes) {
        if (mailbox.isGroup()) {
          throw new IllegalArgumentException("A mailbox in the list is a group");
        }
      }
      this.address = null;
      this.groupAddresses = new ArrayList<NamedAddress>();
      for (NamedAddress mailbox : mailboxes) {
        this.groupAddresses.add(mailbox);
      }
    }

    private final List<NamedAddress> groupAddresses;

    /**
     * Gets a read-only list of addresses that make up the group, if this object
     * represents a group, or an empty list otherwise.
     * @return A list of addresses that make up the group, if this object
     * represents a group, or an empty list otherwise.
     */
    public final List<NamedAddress> getGroupAddresses() {
        return java.util.Collections.unmodifiableList(
            this.groupAddresses);
      }
  }
