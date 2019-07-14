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
     * Determines whether this object and another object are equal.
     * @param obj The parameter {@code obj} is an arbitrary object.
     * @return {@code true} if this object and another object are equal; otherwise,
     * {@code false}.
     */
    @Override public boolean equals(Object obj) {
      Address other = ((obj instanceof Address) ? (Address)obj : null);
      return other != null && this.localPart.equals(other.localPart,
  StringComparison.Ordinal) &&
        this.domain.equals(other.domain, StringComparison.Ordinal);
    }

    /**
     * Gets the local part of this email address (the part before the "@" sign).
     * @return The local part of this email address (the part before the "@" sign).
     */
    public final String getLocalPart() {
        return this.localPart;
      }
    private static String DomainToString(String domain, boolean useALabelDomain) {
      String dom = domain;
      if (useALabelDomain && dom.length() > 0 && dom.charAt(0) != '[') {
        dom = Idna.EncodeDomainName(domain);
      }
      return dom;
    }
    static String LocalPartToString(String localPart) {
      if (localPart.length() > 0 && HeaderParser.ParseDotAtomText(
        localPart,
        0,
        localPart.length(),
        null) == localPart.length()) {
        return localPart;
      } else {
        StringBuilder sb = new StringBuilder();
        sb.append('"');
        for (int i = 0; i < localPart.length(); ++i) {
          char c = localPart.charAt(i);
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
        return sb.toString();
      }
    }

    void AppendThisAddress(HeaderEncoder encoder) {
      String lp = LocalPartToString(this.localPart);
      String domainstr = DomainToString(this.domain, true);
      int length = DataUtilities.CodePointLength(lp);
      int length2 = DataUtilities.CodePointLength(domainstr);
      if (length2 + length + 1 <= Message.MaxRecHeaderLineLength - 1) {
        // Avoid breaking email addresses if it can comfortably
        // fit the recommended line length
        int tlength = (int)(length2 + length + 1);
        encoder.AppendSymbolWithLength(lp + "@" + domainstr, tlength);
      } else {
        encoder.AppendSymbolWithLength(lp, length);
        encoder.AppendSymbol("@");
        encoder.AppendSymbolWithLength(domainstr, length);
      }
    }

    /**
     * Converts this address object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      HeaderEncoder sa = new HeaderEncoder(Message.MaxRecHeaderLineLength, 15);
      this.AppendThisAddress(sa);
      return sa.toString();
    }

    private boolean IsTooLong() {
      String lp = LocalPartToString(this.localPart);
      String domainstr = DomainToString(this.domain, true);
      String domain2 = DomainToString(this.domain, false);
      // Maximum OCTET length per line for an Internet message minus 1;
      // we check if the length exceeds that number (thus excluding the space
      // character of a folded line).
      if (DataUtilities.GetUtf8Length(lp, true) >
    Message.MaxHardHeaderLineLength - 1) {
        return true;
      }
      if (DataUtilities.GetUtf8Length(domainstr, true) >
        Message.MaxHardHeaderLineLength - 1) {
        return true;
      }
      return (DataUtilities.GetUtf8Length(domain2, true) >
        Message.MaxHardHeaderLineLength - 1) ? true : false;
    }

    /**
     * Returns a hash code for this address object. No application or process
     * identifiers are used in the hash code calculation.
     * @return A hash code for this instance.
     */
    @Override public int hashCode() {
      int valueHashCode = -1524613162;
      if (this.domain != null) {
        for (int i = 0; i < this.domain.length(); ++i) {
          valueHashCode *= -1521134295 + this.domain.charAt(i);
        }
      }
      if (this.localPart != null) {
        for (int i = 0; i < this.localPart.length(); ++i) {
          valueHashCode *= -1521134295 + this.localPart.charAt(i);
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
     * Initializes a new instance of the {@link Address} class.
     * @param addressValue The parameter {@code addressValue} is a text string.
     * @throws NullPointerException The parameter {@code addressValue} is null.
     * @throws IllegalArgumentException AddressValue is empty.; Address doesn't contain a
     * '@' sign; Invalid local part; Expected '@' sign after local part;
     * Expected domain after '@'; Invalid domain; Address too long.
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
      if (this.IsTooLong()) {
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
      // Check length restrictions.
      if (this.IsTooLong()) {
        throw new IllegalArgumentException("Address too long");
      }
    }
  }
