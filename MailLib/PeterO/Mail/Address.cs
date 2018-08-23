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
 long length = DataUtilities.GetUtf8Length(lp, true);
 long length2 = DataUtilities.GetUtf8Length(domainstr, true);
 if (length2 + length + 1 <= Message.MaxRecHeaderLineLength-1) {
  // Avoid breaking email addresses if it can comfortably
  // fit the recommended line length
        int tlength=(int)(length2 + length + 1);
  encoder.AppendSymbolWithLength(lp+"@"+domainstr,tlength);
 } else {
        // NOTE: Both lengths can't exceed MaxRecHeaderLineLength,
        // which is well below the maximum value for 32-bit
        // integers, so it's acceptable to cast to int here
        encoder.AppendSymbolWithLength(lp, (int)length);
  encoder.AppendSymbol("@");
        encoder.AppendSymbolWithLength(domainstr, (int)length);
 }
}

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Address.ToString"]/*'/>
    public override string ToString() {
     var sa = new HeaderEncoder(Message.MaxRecHeaderLineLength, 15);
     AppendThisAddress(sa);
  return sa.ToString();
    }

    private bool IsTooLong() {
      string lp = LocalPartToString(this.localPart);
     string domainstr = DomainToString(this.domain, true);
     string domain2 = DomainToString(this.domain, false);
        // Maximum character length per line for an Internet message minus 1;
        // we check if the length exceeds that number (thus excluding the space
        // character of a folded line).
     if
  (DataUtilities.GetUtf8Length(lp, true)>Message.MaxHardHeaderLineLength
       - 1) {
 return true;
}
     if
  (DataUtilities.GetUtf8Length(domainstr, true)>Message.MaxHardHeaderLineLength
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
