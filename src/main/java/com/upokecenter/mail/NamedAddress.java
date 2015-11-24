package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.util.*;

    /**
     * Represents an email address and a name for that address. Can represent a
     * group of email addresses instead.
     */
  public class NamedAddress {
    private final String name;

    /**
     * Gets the display name for this email address (or the email address's value
     * if the display name is absent).
     * @return The display name for this email address.
     */
    public final String getName() {
        return this.name;
      }

    private final Address address;

    /**
     * Gets the email address associated with this object.
     * @return The email address associated with this object.
     */
    public final Address getAddress() {
        return this.address;
      }

    private final boolean isGroup;

    /**
     * Gets a value indicating whether this represents a group of addresses rather
     * than a single address.
     * @return True if this represents a group of addresses; otherwise, false.
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
        builder.append(HeaderParserUtility.QuoteValueIfNeeded(this.name));
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
      if (((this.name) == null || (this.name).length() == 0)) {
        return this.address.toString();
      } else {
        String addressString = this.address.toString();
        if (addressString.equals(this.name)) {
          return addressString;
        }
        if (addressString.length() > 990) {
          // Give some space to ease line wrapping
          return HeaderParserUtility.QuoteValueIfNeeded(this.name) +
          " < " + addressString + " >";
        }
        return HeaderParserUtility.QuoteValueIfNeeded(this.name) +
        " <" + addressString + ">";
      }
    }

    /**
     * Initializes a new instance of the NamedAddress class. Examples: <ul>
     * <li><code>john@example.com</code></li> <li><code>"John Doe"
     * &lt;john@example.com&gt;</code></li>
     * <li><code>=?utf-8?q?John</code><code>=</code><code>27s_Office?=
     * &lt;john@example.com&gt;</code></li> <li><code>John
     * &lt;john@example.com&gt;</code></li> <li><code>"Group" : Tom
     * &lt;tom@example.com&gt;, Jane &lt;jane@example.com&gt;;</code></li></ul>
     * @param address A string object identifying a single email address or a group
     * of email addresses. Comments, or text within parentheses, can appear.
     * Multiple email addresses are not allowed unless they appear in the
     * group syntax given above. Encoded words under RFC 2047 that appear
     * within comments or display names will be decoded. <p>An RFC 2047
     * encoded word consists of "=?", a character encoding name, such as
     * {@code utf-8}, either "?B?" or "?Q?" (in upper or lower case), a
     * series of bytes in the character encoding, further encoded using B or
     * Q encoding, and finally "?=". B encoding uses Base64, while in Q
     * encoding, spaces are changed to "_", equals are changed to "=3D",
     * and most bytes other than ASCII letters and digits are changed to "="
     * followed by their 2-digit hexadecimal form. An encoded word's maximum
     * length is 75 characters. See the second example.</p>.
     * @throws NullPointerException The parameter {@code address} is null.
     * @throws IllegalArgumentException The named address has an invalid syntax.
     */
    public NamedAddress (String address) {
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
      this.name = na.name;
      this.address = na.address;
      this.groupAddresses = na.groupAddresses;
      this.isGroup = na.isGroup;
    }

    /**
     * Initializes a new instance of the NamedAddress class using the given display
     * name and email address.
     * @param displayName The address's display name. Can be null or empty, in
     * which case the email address is used instead. Encoded words under RFC
     * 2047 will not be decoded.
     * @param address An email address.
     * @throws NullPointerException The parameter {@code address} is null.
     * @throws IllegalArgumentException The display name or address has an invalid syntax.
     */
    public NamedAddress (String displayName, String address) {
      if (((displayName) == null || (displayName).length() == 0)) {
        displayName = address;
      }
      if (address == null) {
        throw new NullPointerException("address");
      }
      this.name = displayName;
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.address = new Address(address);
    }

    /**
     * Initializes a new instance of the NamedAddress class.
     * @param displayName A string object. If this value is null or empty, uses the
     * email address as the display name. Encoded words under RFC 2047 will
     * not be decoded.
     * @param address An email address.
     * @throws NullPointerException The parameter {@code address} is null.
     */
    public NamedAddress (String displayName, Address address) {
      if (address == null) {
        throw new NullPointerException("address");
      }
      if (((displayName) == null || (displayName).length() == 0)) {
        displayName = address.toString();
      }
      this.name = displayName;
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.address = address;
    }

    /**
     * Initializes a new instance of the NamedAddress class using the given name
     * and an email address made up of its local part and domain.
     * @param displayName A string object. If this value is null or empty, uses the
     * email address as the display name.
     * @param localPart The local part of the email address (before the "@").
     * @param domain The domain of the email address (before the "@").
     * @throws NullPointerException The parameter {@code localPart} or {@code
     * domain} is null.
     */
    public NamedAddress (String displayName, String localPart, String domain) {
      if (localPart == null) {
        throw new NullPointerException("localPart");
      }
      if (domain == null) {
        throw new NullPointerException("domain");
      }
      this.address = new Address(localPart, domain);
      if (((displayName) == null || (displayName).length() == 0)) {
        displayName = this.address.toString();
      }
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.name = displayName;
    }

    /**
     * Initializes a new instance of the NamedAddress class. Takes a group name and
     * several named email ((addresses instanceof parameters) ?
     * (parameters)addresses : null), and forms a group with them.
     * @param groupName The group's name.
     * @param mailboxes A list of named addresses that make up the group.
     * @throws NullPointerException The parameter {@code groupName} or {@code
     * mailboxes} is null.
     * @throws IllegalArgumentException The parameter {@code groupName} is empty, or an
     * item in the list is itself a group.
     */
    public NamedAddress (String groupName, List<NamedAddress> mailboxes) {
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
      this.name = groupName;
      for (NamedAddress mailbox : mailboxes) {
        if (mailbox.isGroup()) {
          throw new IllegalArgumentException("A mailbox in the list is a group");
        }
      }
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
