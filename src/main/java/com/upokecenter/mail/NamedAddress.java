package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;

    /**
     * Represents an email address and a name for that address. Can represent a
     * group of email addresses instead.
     */
  public class NamedAddress {
    private final String displayName;

    /**
     * Generates a list of NamedAddress objects from a comma-separated list of
     * addresses. Each address must follow the syntax accepted by the
     * one-argument constructor of NamedAddress.
     * @param addressValue A comma-separate list of addresses in the form of a text
     * string.
     * @return A list of addresses generated from the {@code addressValue}
     * parameter.
     */
    public static List<NamedAddress> ParseAddresses(String addressValue) {
      ArrayList<NamedAddress> list = new ArrayList<NamedAddress>();
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
     * Calculates the hash code of this object. No application or process IDs are
     * used in the hash code calculation.
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
     * Determines whether this object and another object are equal.
     * @param obj The parameter {@code obj} is an arbitrary object.
     * @return {@code true} if this object and another object are equal; otherwise,
     * {@code false}.
     */
    @Override public boolean equals(Object obj) {
      NamedAddress other = ((obj instanceof NamedAddress) ? (NamedAddress)obj : null);
      return other != null &&
      (this.displayName == null ? other.displayName == null :
        this.displayName.equals(other.displayName, StringComparison.Ordinal)) &&
   (this.address == null ? other.address == null :
     this.address.equals(other.address)) && this.isGroup == other.isGroup &&
          (!this.isGroup || CollectionUtilities.ListEquals(
            this.groupAddresses,
            other.groupAddresses));
    }

    /**
     * Not documented yet.
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
     * if the display name is null. Returns an empty string if the address
     * and display name are null.
     * @return The name for this email address.
     */
    public final String getName() {
        return (this.displayName == null) ? ((this.address == null) ?
                 "" : this.address.toString()) : this.displayName;
      }

    /**
     * Gets the display name for this email address. Returns null if the display
     * name is absent.
     * @return The display name for this email address.
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
     * @return {@code true} If this represents a group of addresses; otherwise, .
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
     *  this NamedAddress object as it could appear in a "To" header field.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      HeaderEncoder enc = new HeaderEncoder(Message.MaxRecHeaderLineLength, 15);
      this.AppendThisAddress(enc);
      return enc.toString();
    }

    /**
     * Initializes a new instance of the {@link NamedAddress} class.
     * @param address The parameter {@code address} is a text string.
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
     * Initializes a new instance of the {@link NamedAddress} class.
     * @param displayName The parameter {@code displayName} is a text string.
     * @param address The parameter {@code address} is a text string.
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
     * Initializes a new instance of the {@link NamedAddress} class.
     * @param displayName The parameter {@code displayName} is a text string.
     * @param address The parameter {@code address} is an Address object.
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
     * Initializes a new instance of the {@link NamedAddress} class.
     * @param displayName The parameter {@code displayName} is a text string.
     * @param localPart The parameter {@code localPart} is a text string.
     * @param domain The parameter {@code domain} is a text string.
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
     * Initializes a new instance of the {@link NamedAddress} class.
     * @param groupName The parameter {@code groupName} is a text string.
     * @param mailboxes The parameter {@code mailboxes} is an List object.
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
