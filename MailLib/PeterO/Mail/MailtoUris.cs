using System;
using System.Collections.Generic;
using System.Text;
using PeterO;

namespace PeterO.Mail {
  internal static class MailtoUris {
    private static string Implode(string[] strings, string delim) {
      if (strings.Length == 0) {
        return String.Empty;
      }
      if (strings.Length == 1) {
        return strings[0];
      }
      var sb = new StringBuilder();
      var first = true;
      foreach (string s in strings) {
        if (!first) {
          sb.Append(delim);
        }
        sb.Append(s);
        first = false;
      }
      return sb.ToString();
    }

    private static string CombineAddresses(Message msg, string headerName) {
      IList<NamedAddress> addresses = msg.GetAddresses(headerName);
      var encoder = new HeaderEncoder().AppendFieldName(headerName);
      var first = true;
      foreach (NamedAddress addr in addresses) {
        if (!first) {
          encoder.AppendSymbol(",");
        }
        addr.AppendThisAddress(encoder);
        first = false;
      }
      string ret = encoder.ToString();
      // Strip out header field name, colon, and space
      return ret.Substring(headerName.Length + 2);
    }

    public static string MessageToMailtoUri(Message msg) {
      if (msg == null) {
        throw new ArgumentNullException(nameof(msg));
      }
      IList<NamedAddress> addresses = msg.GetAddresses("to");
      var sb = new StringBuilder();
      sb.Append("mailto:");
      var hasGroupOrDisplay = false;
      foreach (NamedAddress addr in addresses) {
        if (addr.IsGroup || !String.IsNullOrEmpty(addr.DisplayName)) {
          hasGroupOrDisplay = true;
          break;
        }
      }
      string field = null;
      var firstField = true;
      if (!hasGroupOrDisplay) {
        var first = true;
        foreach (NamedAddress addr in addresses) {
          if (!first) {
            {
              first = false;
            }
            sb.Append(",");
          }
          Address address = addr.Address;
          sb.Append(URIUtility.EncodeStringForURI(address.LocalPart));
          sb.Append("@");
          sb.Append(URIUtility.EncodeStringForURI(address.Domain));
        }
      } else {
        field = CombineAddresses(msg, "to");
      }
      firstField = true;
      if (!String.IsNullOrEmpty(field)) {
        sb.Append(firstField ? "?to=" : "&to=");
        firstField = false;
        sb.Append(URIUtility.EncodeStringForURI(field));
      }
      field = Implode(msg.GetHeaderArray("subject"), "\n ");
      if (!String.IsNullOrEmpty(field)) {
        sb.Append(firstField ? "?subject=" : "&subject=");
        firstField = false;
        sb.Append(URIUtility.EncodeStringForURI(field));
      }
      field = Implode(msg.GetHeaderArray("in-reply-to"), "\n ");
      if (!String.IsNullOrEmpty(field)) {
        sb.Append(firstField ? "?in-reply-to=" : "&in-reply-to=");
        firstField = false;
        sb.Append(URIUtility.EncodeStringForURI(field));
      }
      field = CombineAddresses(msg, "cc");
      if (!String.IsNullOrEmpty(field)) {
        sb.Append(firstField ? "?cc=" : "&cc=");
        firstField = false;
        sb.Append(URIUtility.EncodeStringForURI(field));
      }
      field = CombineAddresses(msg, "bcc");
      if (!String.IsNullOrEmpty(field)) {
        sb.Append(firstField ? "?bcc=" : "&bcc=");
        firstField = false;
        sb.Append(URIUtility.EncodeStringForURI(field));
      }
      string[] fields = msg.GetHeaderArray("keywords");
      foreach (string field2 in fields) {
        if (!String.IsNullOrEmpty(field)) {
          sb.Append(firstField ? "?keywords=" : "&keywords=");
          firstField = false;
          sb.Append(URIUtility.EncodeStringForURI(field));
        }
      }
      fields = msg.GetHeaderArray("comments");
      foreach (string field2 in fields) {
        if (!String.IsNullOrEmpty(field)) {
          sb.Append(firstField ? "?comments=" : "&comments=");
          firstField = false;
          sb.Append(URIUtility.EncodeStringForURI(field));
        }
      }
      try {
        field = msg.GetBodyString();
      } catch (NotSupportedException) {
        field = null;
      }
      if (!String.IsNullOrEmpty(field)) {
        sb.Append(firstField ? "?body=" : "&body=");
        firstField = false;
        sb.Append(URIUtility.EncodeStringForURI(field));
      }
      return sb.ToString();
    }

    public static Message MailtoUriMessage(string uri) {
      string[] parts = URIUtility.SplitIRIToStrings(
          uri);
      if (parts == null || parts[0] == null) {
        return null;
      }

      if (parts[0].Equals("mailto", StringComparison.Ordinal)) {
        var msg = new Message();
        string emails = String.Empty;
        if (!String.IsNullOrEmpty(parts[2])) {
          // Extract the email address
          emails = URIUtility.PercentDecode(parts[2]);
          if (HeaderParser2.ParseHeaderEmail(emails, 0, emails.Length) !=
            emails.Length) {
            return null;
          }
        }
        if (parts[3] != null) {
          // If query string is present it must not be empty
          string query = parts[3];
          if (query.Length == 0) {
            return null;
          }
          var index = 0;
          while (index < query.Length) {
            int startName = index;
            var endName = -1;
            var startValue = -1;
            var endValue = -1;
            while (index < query.Length) {
              if (query[index] == '&') {
                if (endName < 0) {
                  return null;
                }
                endValue = index;
                ++index;
                break;
              } else if (query[index] == '=') {
                if (endName >= 0) {
                  return null;
                }
                endName = index;
                startValue = index + 1;
                ++index;
              } else {
                ++index;
              }
            }
            if (endName < 0) {
              return null;
            } else if (endValue < 0) {
              endValue = query.Length;
            }
            string name = query.Substring(startName, endName - startName);
            string value = query.Substring(startValue, endValue - startValue);
            name =
              DataUtilities.ToLowerCaseAscii(URIUtility.PercentDecode(name));
            value = URIUtility.PercentDecode(value);
            // Support only To, Cc, Bcc, Subject, In-Reply-To,
            // Keywords, Comments, and Body.
            // Of these, the first five can appear only once in a message
            if (name.Equals("body", StringComparison.Ordinal)) {
              msg.SetTextBody(value);
            } else if (name.Equals("keywords", StringComparison.Ordinal) ||
              name.Equals("comments", StringComparison.Ordinal)) {
              string decoded = Message.DecodeHeaderValue(name, value);
              msg.AddHeader(name, decoded);
            } else if (name.Equals("subject", StringComparison.Ordinal) ||
              name.Equals("cc", StringComparison.Ordinal) ||
              name.Equals("bcc", StringComparison.Ordinal) ||
              name.Equals("in-reply-to", StringComparison.Ordinal)) {
              string decoded = Message.DecodeHeaderValue(name, value);
              msg.SetHeader(name, decoded);
            } else if (name.Equals("to", StringComparison.Ordinal)) {
              string decoded = Message.DecodeHeaderValue(name, value);
              if (String.IsNullOrEmpty(emails)) {
                emails = decoded;
              } else {
                emails += "," + decoded;
              }
            } else {
              // DebugUtility.Log(name);
              // DebugUtility.Log(value);
            }
          }
        }
        if (!String.IsNullOrEmpty(emails)) {
          if (emails.IndexOf('(') >= 0) {
            // Contains opening parenthesis, a comment delimiter
            return null;
          }
          try {
            msg.SetHeader("to", emails);
          } catch (ArgumentException) {
            return null;
          }
        }
        return msg;
      }
      return null;
    }
  }
}
