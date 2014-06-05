package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

    /**
     * Represents an email address and a name for that address.
     */
  public class NamedAddress {
    private String name;

    /**
     * Gets the display name for this email address.
     * @return The display name for this email address.
     */
    public String getName() {
        return this.name;
      }

    private Address address;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public Address getAddress() {
        return this.address;
      }

    private boolean isGroup;

    /**
     * Gets a value indicating whether this represents a group of addresses
     * rather than a single address.
     * @return True if this represents a group of addresses; otherwise,
     * false..
     */
    public boolean isGroup() {
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
        for(NamedAddress address : this.groupAddresses) {
          if (!first) {
            builder.append(", ");
          }
          first = false;
          builder.append(address.toString());
        }
        builder.append(";");
        return builder.toString();
      } else if (((this.name)==null || (this.name).length()==0)) {
        return this.address.toString();
      } else {
        String addressString = this.address.toString();
        if (addressString.equals(this.name)) {
          return addressString;
        } else {
          if (addressString.length() > 990) {
            // Give some space to ease line wrapping
            return HeaderParserUtility.QuoteValueIfNeeded(this.name) +
              " < " + addressString + " >";
          } else {
            return HeaderParserUtility.QuoteValueIfNeeded(this.name) +
              " <" + addressString + ">";
          }
        }
      }
    }

    public NamedAddress (String address) {
      if (address == null) {
        throw new NullPointerException("address");
      }
      Tokener tokener = new Tokener();
      if (HeaderParser.ParseAddress(address, 0, address.length(), tokener) != address.length()) {
        throw new IllegalArgumentException("Address has an invalid syntax.");
      }
      NamedAddress na = HeaderParserUtility.ParseAddress(address, 0, address.length(), tokener.GetTokens());
      if (na == null) {
        throw new IllegalArgumentException("Address has an invalid syntax.");
      }
      this.name = na.name;
      this.address = na.address;
      this.groupAddresses = na.groupAddresses;
    }

    public NamedAddress (String displayName, String address) {
      if (((displayName)==null || (displayName).length()==0)) {
        displayName = address;
      }
      if (address == null) {
        throw new NullPointerException("address");
      }
      this.name = displayName;
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.address = new Address(address);
    }

    public NamedAddress (String displayName, Address address) {
      if (address == null) {
        throw new NullPointerException("address");
      }
      if (((displayName)==null || (displayName).length()==0)) {
        displayName = address.toString();
      }
      this.name = displayName;
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.address = address;
    }

    public NamedAddress (String displayName, String localPart, String domain) {
      if (localPart == null) {
        throw new NullPointerException("localPart");
      }
      if (domain == null) {
        throw new NullPointerException("domain");
      }
      this.address = new Address(localPart, domain);
      if (((displayName)==null || (displayName).length()==0)) {
        displayName = this.address.toString();
      }
      this.groupAddresses = new ArrayList<NamedAddress>();
      this.name = displayName;
    }

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
      for(NamedAddress mailbox : mailboxes) {
        if (mailbox.isGroup()) {
          throw new IllegalArgumentException("A mailbox in the list is a group");
        }
      }
      this.groupAddresses = new ArrayList<NamedAddress>(mailboxes);
    }

    private List<NamedAddress> groupAddresses;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<NamedAddress> getGroupAddresses() {
        return java.util.Collections.unmodifiableList(this.groupAddresses);
      }
  }
