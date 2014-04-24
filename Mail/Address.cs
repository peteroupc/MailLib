/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;

using PeterO.Text;

namespace PeterO.Mail {
    /// <summary>Represents an email address.</summary>
  public class Address {
    private string localPart;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string LocalPart {
      get {
        return this.localPart;
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      if (this.localPart.Length > 0 &&
          HeaderParser.ParseDotAtomText(this.localPart, 0, this.localPart.Length, null) == this.localPart.Length) {
        return this.localPart + "@" + this.domain;
      } else {
        StringBuilder sb = new StringBuilder();
        sb.Append('"');
        for (int i = 0; i < this.localPart.Length; ++i) {
          char c = this.localPart[i];
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
        sb.Append('@');
        sb.Append(this.domain);
        return sb.ToString();
      }
    }

    private int StringLength() {
      int domainLength = this.domain.Length;
      if (domainLength > 0 && this.domain[0] != '[') {
        // "domain" is a domain name, and not an address literal,
        // so get its A-label length
        domainLength = checked((int)DataUtilities.GetUtf8Length(Idna.DomainNameEncode(this.domain), true));
      }
      if (this.localPart.Length > 0 &&
          HeaderParser.ParseDotAtomText(this.localPart, 0, this.localPart.Length, null) == this.localPart.Length) {
        return this.localPart.Length + domainLength + 1;
      } else {
        int length = 3 + domainLength;  // two quotes, at sign, and domain length
        for (int i = 0; i < this.localPart.Length; ++i) {
          char c = this.localPart[i];
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

    private string domain;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string Domain {
      get {
        return this.domain;
      }
    }

    public Address(string addressValue) {
      if (addressValue == null) {
        throw new ArgumentNullException("addressValue");
      }
      if (addressValue.Length == 0) {
        throw new ArgumentException("addressValue is empty.");
      }
      if (addressValue.IndexOf('@') < 0) {
        throw new ArgumentException("Address doesn't contain a '@' sign");
      }
      int localPartEnd = HeaderParser.ParseLocalPartNoCfws(addressValue, 0, addressValue.Length, null);
      if (localPartEnd == 0) {
        throw new ArgumentException("Invalid local part");
      }
      if (localPartEnd >= addressValue.Length || addressValue[localPartEnd] != '@') {
        throw new ArgumentException("Expected '@' sign after local part");
      }
      if (localPartEnd + 1 == addressValue.Length) {
        throw new ArgumentException("Expected domain after '@'");
      }
      int domainEnd = HeaderParser.ParseDomainNoCfws(addressValue, localPartEnd + 1, addressValue.Length, null);
      if (domainEnd != addressValue.Length) {
        throw new ArgumentException("Invalid domain");
      }
      this.localPart = HeaderParserUtility.ParseLocalPart(addressValue, 0, localPartEnd);
      this.domain = HeaderParserUtility.ParseDomain(addressValue, localPartEnd + 1, addressValue.Length);
      // Check length restrictions.
      if (this.StringLength() > 997) {
        // Maximum character length per line for an Internet message is 998;
        // we check if the length exceeds 997 (thus excluding the space character
        // of a folded line).
        throw new ArgumentException("Address too long");
      }
    }

    internal Address(string localPart, string domain) {
      if (localPart == null) {
        throw new ArgumentNullException("localPart");
      }
      if (domain == null) {
        throw new ArgumentNullException("domain");
      }
      this.localPart = localPart;
      this.domain = domain;
      // Check length restrictions. See above.
      if (this.StringLength() > 997) {
        throw new ArgumentException("Address too long");
      }
    }
  }
}
