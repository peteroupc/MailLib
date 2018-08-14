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
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    @Override public int hashCode() {
      int valueHashCode = -1524613162;
      if (displayName != null) {
        for (int i = 0; i < displayName.length(); ++i) {
          valueHashCode *= -1521134295 + displayName.charAt(i);
        }
      }
      valueHashCode *= -1521134295 + (address == null ? 0 :
        address.hashCode());
      valueHashCode *= -1521134295 + (isGroup ? 0 : 1);
      if (groupAddresses != null) {
        valueHashCode *= -1521134295 + (groupAddresses.size());
      }
      return valueHashCode;
    }

    /**
     * Not documented yet.
     * @param obj Not documented yet.
     * @return A Boolean object.
     */
    @Override public boolean equals(Object obj) {
      NamedAddress other = ((obj instanceof NamedAddress) ? (NamedAddress)obj : null);
      return other != null &&
      (displayName == null ? other.displayName == null :
        displayName.equals(other.displayName)) &&
   (address == null ? other.address == null : address.equals(other.address))&&
        isGroup == other.isGroup &&
          (!isGroup || CollectionUtilities.ListEquals(groupAddresses,
            other.groupAddresses));
    }

    /**
     *
     * @param na A named address object to compare with this one. Can be null.
     * @return Either {@code true} or {@code false}.
     */
    public boolean AddressesEqual(NamedAddress na) {
      if (na == null || isGroup != na.isGroup) {
 return false;
}
      if (isGroup) {
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
     * @return {@code true} If this represents a group of addresses; otherwise,
     * {@code false}.
     */
    public final boolean isGroup() {
        return this.isGroup;
      }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      if (this.isGroup) {
        StringBuilder builder = new StringBuilder();
  builder.append(HeaderParserUtility.QuoteValueIfNeeded(this.displayName));
        builder.append(": ");
        boolean first = true;
        for (NamedAddress groupAddress : this.groupAddresses) {
          if (!first) {
            builder.append(", ");
          }
          first = false;
          builder.append(groupAddress.toString());
        }
        builder.append(";");
        return builder.toString();
      }
      if (this.displayName == null) {
        return this.address.toString();
      } else {
        String addressString = this.address.toString();
        if (addressString.length() > 990) {
          // Give some space to ease line wrapping
          return HeaderParserUtility.QuoteValueIfNeeded(this.displayName) +
          " < " + addressString + " >";
        }
        return HeaderParserUtility.QuoteValueIfNeeded(this.displayName) +
        " <" + addressString + ">";
      }
    }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.NamedAddress}
     * class. Examples: <ul> <li><code>john@example.com</code></li> <li><code>"John
     * Doe" &lt;john@example.com&gt;</code></li>
     * <li><code>=?utf-8?q?John</code><code>=</code><code>27s_Office?=
     * &lt;john@example.com&gt;</code></li> <li><code>John
     * &lt;john@example.com&gt;</code></li> <li><code>"Group" : Tom
     * &lt;tom@example.com&gt;, Jane &lt;jane@example.com&gt;;</code></li></ul>
     * @param address A text string identifying a single email address or a group
     * of email addresses. Comments, or text within parentheses, can appear.
     * Multiple email addresses are not allowed unless they appear in the
     * group syntax given above. Encoded words under RFC 2047 that appear
     * within comments or display names will be decoded. <p>An RFC 2047
     * encoded word consists of "=?", a character encoding name, such as
     * {@code utf-8}, either "?B?" or "?Q?" (in upper or lower case), a
     * series of bytes in the character encoding, further encoded using B or
     * Q encoding, and finally "?=". B encoding uses Base64, while in Q
     * encoding, spaces are changed to "_", equals are changed to "=3D", and
     * most bytes other than the basic digits 0 to 9 (0x30 to 0x39) and the
     * basic letters A/a to Z/z (0x41 to 0x5a, 0x61 to 0x7a) are changed to
     * "=" followed by their 2-digit hexadecimal form. An encoded word's
     * maximum length is 75 characters. See the third example.</p>.
     * @throws java.lang.NullPointerException The parameter {@code address} is null.
     * @throws IllegalArgumentException The named address has an invalid syntax.
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
     * @throws java.lang.NullPointerException The parameter {@code address} is null.
     * @throws IllegalArgumentException The display name or address has an invalid
     * syntax.
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
     * @throws java.lang.NullPointerException The parameter {@code address} is null.
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
     * class using the given name and an email address made up of its local
     * part and domain.
     * @param displayName The display name of the email address. Can be null or
     * empty.
     * @param localPart The local part of the email address (before the "@").
     * @param domain The domain of the email address (before the "@").
     * @throws java.lang.NullPointerException The parameter {@code localPart} or
     * {@code domain} is null.
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
     *
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
      this.groupAddresses = new ArrayList<NamedAddress>(mailboxes);
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
