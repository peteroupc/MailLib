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
using PeterO;
using PeterO.Text;

namespace PeterO.Mail {
    /// <summary>Represents an email address.</summary>
  public class Address {
    private readonly string localPart;

    /// <summary>Determines whether this object and another object are
    /// equal.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns><c>true</c> if this object and another object are equal;
    /// otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj) {
      var other = obj as Address;
      return other != null && this.localPart.Equals(other.localPart,
  StringComparison.Ordinal) &&
        this.domain.Equals(other.domain, StringComparison.Ordinal);
    }

    /// <summary>Gets the local part of this email address (the part before
    /// the "@" sign).</summary>
    /// <value>The local part of this email address (the part before the
    /// "@" sign).</value>
    public string LocalPart {
      get {
        return this.localPart;
      }
    }
    private static string DomainToString(string domain, bool useALabelDomain) {
      string dom = domain;
      if (useALabelDomain && dom.Length > 0 && dom[0] != '[') {
        dom = Idna.EncodeDomainName(domain);
      }
      return dom;
    }
    internal static string LocalPartToString(string localPart) {
      if (localPart.Length > 0 && HeaderParser.ParseDotAtomText(
        localPart,
        0,
        localPart.Length,
        null) == localPart.Length) {
        return localPart;
      } else {
        var sb = new StringBuilder();
        sb.Append('"');
        for (int i = 0; i < localPart.Length; ++i) {
          char c = localPart[i];
          if (c == 0x20 || c == 0x09) {
            sb.Append(c);
          } else if (c == '"' || c == 0x7f || c == '\\' || c < 0x20) {
            sb.Append('\\');
            sb.Append(c);
          } else {
            sb.Append(c);
          }
        }
        sb.Append('"');
        return sb.ToString();
      }
    }

    internal void AppendThisAddress(HeaderEncoder encoder) {
      string lp = LocalPartToString(this.localPart);
      string domainstr = DomainToString(this.domain, true);
      int length = DataUtilities.CodePointLength(lp);
      int length2 = DataUtilities.CodePointLength(domainstr);
      if (length2 + length + 1 <= Message.MaxRecHeaderLineLength - 1) {
        // Avoid breaking email addresses if it can comfortably
        // fit the recommended line length
        var tlength = (int)(length2 + length + 1);
        encoder.AppendSymbolWithLength(lp + "@" + domainstr, tlength);
      } else {
        encoder.AppendSymbolWithLength(lp, length);
        encoder.AppendSymbol("@");
        encoder.AppendSymbolWithLength(domainstr, length);
      }
    }

    /// <summary>Converts this address object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      var sa = new HeaderEncoder(Message.MaxRecHeaderLineLength, 15);
      this.AppendThisAddress(sa);
      return sa.ToString();
    }

    private bool IsTooLong() {
      string lp = LocalPartToString(this.localPart);
      string domainstr = DomainToString(this.domain, true);
      string domain2 = DomainToString(this.domain, false);
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

    /// <summary>Returns a hash code for this address object. No
    /// application or process identifiers are used in the hash code
    /// calculation.</summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode() {
      var hashCode = -1524613162;
      if (this.domain != null) {
        for (var i = 0; i < this.domain.Length; ++i) {
          hashCode *= -1521134295 + this.domain[i];
        }
      }
      if (this.localPart != null) {
        for (var i = 0; i < this.localPart.Length; ++i) {
          hashCode *= -1521134295 + this.localPart[i];
        }
      }
      return hashCode;
    }

    private readonly string domain;

    /// <summary>Gets the domain of this email address (the part after the
    /// "@" sign).</summary>
    /// <value>The domain of this email address (the part after the "@"
    /// sign).</value>
    public string Domain {
      get {
        return this.domain;
      }
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.Address'/> class.</summary>
    /// <param name='addressValue'>The parameter <paramref name='addressValue'/> is a text string.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='addressValue'/> is null.</exception>
    /// <exception cref='System.ArgumentException'>AddressValue is empty.; Address
    /// doesn't contain a '@' sign; Invalid local part; Expected '@' sign
    /// after local part; Expected domain after '@'; Invalid domain;
    /// Address too long.</exception>
    public Address(string addressValue) {
      if (addressValue == null) {
        throw new ArgumentNullException(nameof(addressValue));
      }
      if (addressValue.Length == 0) {
        throw new ArgumentException("addressValue is empty.");
      }
      if (addressValue.IndexOf('@') < 0) {
        throw new ArgumentException("Address doesn't contain a '@' sign");
      }
      int localPartEnd = HeaderParser.ParseLocalPartNoCfws(
        addressValue,
        0,
        addressValue.Length,
        null);
      if (localPartEnd == 0) {
        throw new ArgumentException("Invalid local part");
      }
      if (localPartEnd >= addressValue.Length ||
     addressValue[localPartEnd] != '@') {
        throw new ArgumentException("Expected '@' sign after local part");
      }
      if (localPartEnd + 1 == addressValue.Length) {
        throw new ArgumentException("Expected domain after '@'");
      }
      int domainEnd = HeaderParser.ParseDomainNoCfws(
        addressValue,
        localPartEnd + 1,
        addressValue.Length,
        null);
      if (domainEnd != addressValue.Length) {
        throw new ArgumentException("Invalid domain");
      }
      this.localPart = HeaderParserUtility.ParseLocalPart(
        addressValue,
        0,
        localPartEnd);
      this.domain = HeaderParserUtility.ParseDomain(
        addressValue,
        localPartEnd + 1,
        addressValue.Length);
      // Check length restrictions.
      if (this.IsTooLong()) {
        throw new ArgumentException("Address too long");
      }
    }

    internal Address(string localPart, string domain) {
      if (localPart == null) {
        throw new ArgumentNullException(nameof(localPart));
      }
      if (domain == null) {
        throw new ArgumentNullException(nameof(domain));
      }
      this.localPart = localPart;
      this.domain = domain;
      // Check length restrictions.
      if (this.IsTooLong()) {
        throw new ArgumentException("Address too long");
      }
    }
  }
}
