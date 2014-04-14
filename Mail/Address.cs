/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace PeterO.Mail
{
    /// <summary>Represents an email address.</summary>
  public class Address {
    private string localPart;

    // TODO: Honor length limits when outputting (recommended
    // limit 76 characters, required limit 998 bytes)
    public string LocalPart {
      get {
        return this.localPart;
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      if (HeaderParser.ParseDotAtomText(this.localPart, 0, this.localPart.Length, null) == this.localPart.Length) {
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
        throw new ArgumentNullException("addressString");
      }
      if (addressValue.Length == 0) {
        throw new ArgumentException("addressString is empty.");
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
      int domainEnd = HeaderParser.ParseDomainNoCfws(addressValue, localPartEnd + 1, addressValue.Length, null);
      if (domainEnd != addressValue.Length) {
        throw new ArgumentException("Invalid domain");
      }
      this.localPart = HeaderParserUtility.ParseLocalPart(addressValue, 0, localPartEnd);
      this.domain = HeaderParserUtility.ParseDomain(addressValue, localPartEnd + 1, addressValue.Length);
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
    }
  }
}
