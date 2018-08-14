package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;

import com.upokecenter.util.*;
import com.upokecenter.text.*;

    /**
     * Represents an email address.
     */
  public class Address {
    private final String localPart;

    /**
     * Not documented yet.
     * @param obj The parameter {@code obj} is not documented yet.
     * @return Either {@code true} or {@code false}.
     */
    @Override public boolean equals(Object obj) {
      Address other = ((obj instanceof Address) ? (Address)obj : null);
      return other != null && this.localPart.equals(other.localPart) &&
        this.domain.equals(other.domain); }

    /**
     * Gets the local part of this email address (the part before the "@" sign).
     * @return The local part of this email address (the part before the "@" sign).
     */
    public final String getLocalPart() {
        return this.localPart;
      }

    /**
     * Converts this address object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      if (this.localPart.length() > 0 &&
          HeaderParser.ParseDotAtomText(
  this.localPart,
  0,
  this.localPart.length(),
  null) == this.localPart.length()) {
        return this.localPart + "@" + this.domain;
      } else {
        StringBuilder sb = new StringBuilder();
        sb.append('"');
        for (int i = 0; i < this.localPart.length(); ++i) {
          char c = this.localPart.charAt(i);
          if (c == 0x20 || c == 0x09) {
            sb.append(c);
          } else if (c == '"' || c == 0x7f || c == '\\' || c < 0x20) {
            sb.append('\\');
            sb.append(c);
          } else {
            sb.append(c);
          }
        }
        sb.append('"');
        sb.append('@');
        sb.append(this.domain);
        return sb.toString();
      }
    }

    private int StringLength() {
      int domainLength = this.domain.length();
      if (domainLength > 0 && this.domain.charAt(0) != '[') {
        // "domain" is a domain name, and not an address literal,
        // so get its A-label length
        domainLength =
  ((int)
  DataUtilities.GetUtf8Length(
  Idna.EncodeDomainName(this.domain),
  true));
      }
      if (this.localPart.length() > 0 && HeaderParser.ParseDotAtomText(
  this.localPart,
  0,
  this.localPart.length(),
  null) == this.localPart.length()) {
        return this.localPart.length() + domainLength + 1;
      } else {
        // two quotes, at sign, and domain length
        int length = 3 + domainLength;
        for (int i = 0; i < this.localPart.length(); ++i) {
          char c = this.localPart.charAt(i);
          if (c == 0x20 || c == 0x09) {
            ++length;
          } else if (c == '"' || c == 0x7f || c == '\\' || c < 0x20) {
            length += 2;
          } else {
            ++length;
          }
        }
        return length;
      }
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    @Override public int hashCode() {
      int valueHashCode = -1524613162;
      if (domain != null) {
        for (int i = 0; i < domain.length(); ++i) {
          valueHashCode *= -1521134295 + domain.charAt(i);
        }
      }
      if (localPart != null) {
        for (int i = 0; i < localPart.length(); ++i) {
          valueHashCode *= -1521134295 + localPart.charAt(i);
        }
      }
      return valueHashCode;
    }

    private final String domain;

    /**
     * Gets the domain of this email address (the part after the "@" sign).
     * @return The domain of this email address (the part after the "@" sign).
     */
    public final String getDomain() {
        return this.domain;
      }

    /**
     * Initializes a new instance of the {@link com.upokecenter.mail.Address}
     * class.
     * @param addressValue An email address.
     * @throws java.lang.NullPointerException The parameter {@code addressValue} is
     * null.
     * @throws IllegalArgumentException The email address contains invalid syntax.
     * For example, it doesn't contain an '@' sign or either side of the '@'
     * contains invalid characters, the address is too long, or the address
     * contains comments (text within parentheses).
     */
    public Address(String addressValue) {
      if (addressValue == null) {
        throw new NullPointerException("addressValue");
      }
      if (addressValue.length() == 0) {
        throw new IllegalArgumentException("addressValue is empty.");
      }
      if (addressValue.indexOf('@') < 0) {
        throw new IllegalArgumentException("Address doesn't contain a '@' sign");
      }
      int localPartEnd = HeaderParser.ParseLocalPartNoCfws(
  addressValue,
  0,
  addressValue.length(),
  null);
      if (localPartEnd == 0) {
        throw new IllegalArgumentException("Invalid local part");
      }
   if (localPartEnd >= addressValue.length() ||
     addressValue.charAt(localPartEnd) != '@') {
        throw new IllegalArgumentException("Expected '@' sign after local part");
      }
      if (localPartEnd + 1 == addressValue.length()) {
        throw new IllegalArgumentException("Expected domain after '@'");
      }
      int domainEnd = HeaderParser.ParseDomainNoCfws(
  addressValue,
  localPartEnd + 1,
  addressValue.length(),
  null);
      if (domainEnd != addressValue.length()) {
        throw new IllegalArgumentException("Invalid domain");
      }
      this.localPart = HeaderParserUtility.ParseLocalPart(
  addressValue,
  0,
  localPartEnd);
      this.domain = HeaderParserUtility.ParseDomain(
  addressValue,
  localPartEnd + 1,
  addressValue.length());
      // Check length restrictions.
      if (this.StringLength() > Message.MaxHardHeaderLineLength - 1) {
        // Maximum character length per line for an Internet message minus 1;
        // we check if the length exceeds that number (thus excluding the space
        // character
        // of a folded line).
        throw new IllegalArgumentException("Address too long");
      }
    }

    Address(String localPart, String domain) {
      if (localPart == null) {
        throw new NullPointerException("localPart");
      }
      if (domain == null) {
        throw new NullPointerException("domain");
      }
      this.localPart = localPart;
      this.domain = domain;
      // Check length restrictions. See above.
      if (this.StringLength() > 997) {
        throw new IllegalArgumentException("Address too long");
      }
    }
  }
