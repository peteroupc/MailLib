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
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.Address"]/*'/>
  public class Address {
    private readonly string localPart;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Address.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      var other = obj as Address;
      return other != null && this.localPart.Equals(other.localPart) &&
        this.domain.Equals(other.domain); }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Address.LocalPart"]/*'/>
    public string LocalPart {
      get {
        return this.localPart;
      }
    }

private string DomainToString(bool useALabelDomain) {
 var dom = this.domain;
 if (useALabelDomain && dom.Length > 0 && dom[0] != '[') {
  dom = Idna.EncodeDomainName(this.domain);
 }
 return dom;
}

private string LocalPartToString() {
     if (this.localPart.Length > 0 && HeaderParser.ParseDotAtomText(
  this.localPart,
  0,
  this.localPart.Length,
  null) == this.localPart.Length) {
  return this.localPart;
 } else {
        var sb = new StringBuilder();
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
        return sb.ToString();
      }
}

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Address.ToString"]/*'/>
    public override string ToString() {
// TODO: Check whether this method is used by
// any message encoders and use or make a more
// robust alternative to this method.
     string localPart = LocalPartToString();
     string domain = DomainToString(true);
long localPartLength = DataUtilities.GetUtf8Length(localPart, true);
long domainLength = DataUtilities.GetUtf8Length(domain, true);
if (localPartLength + domainLength + 1 <= Message.MaxHardHeaderLineLength - 1) {
return localPart+"@"+domain;
} else if (localPartLength + 1 <= Message.MaxHardHeaderLineLength - 1) {
return localPart+"@\r\n "+domain;
} else if (domainLength + 1 <= Message.MaxHardHeaderLineLength - 1) {
return localPart+"\r\n @"+domain;
} else {
return localPart+"\r\n @\r\n "+domain;
}
    }

    private bool IsTooLong() {
      string localPart = LocalPartToString();
     string domain = DomainToString(true);
     string domain2 = DomainToString(false);
        // Maximum character length per line for an Internet message minus 1;
        // we check if the length exceeds that number (thus excluding the space
        // character of a folded line).
     if
  (DataUtilities.GetUtf8Length(localPart, true)>Message.MaxHardHeaderLineLength
       - 1) {
 return true;
}
     if
  (DataUtilities.GetUtf8Length(domain, true)>Message.MaxHardHeaderLineLength
       - 1) {
 return true;
}
     return
  (DataUtilities.GetUtf8Length(domain2, true)>Message.MaxHardHeaderLineLength
       - 1) ? (true) : (false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Address.GetHashCode"]/*'/>
    public override int GetHashCode() {
      var hashCode = -1524613162;
      if (domain != null) {
        for (var i = 0; i < domain.Length; ++i) {
          hashCode *= -1521134295 + domain[i];
        }
      }
      if (localPart != null) {
        for (var i = 0; i < localPart.Length; ++i) {
          hashCode *= -1521134295 + localPart[i];
        }
      }
      return hashCode;
    }

    private readonly string domain;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.Address.Domain"]/*'/>
    public string Domain {
      get {
        return this.domain;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Address.#ctor(System.String)"]/*'/>
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
