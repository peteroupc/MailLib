package com.upokecenter.mail;

import java.util.*;

import com.upokecenter.util.*;

  final class MailtoUris {
private MailtoUris() {
}
    private static String Implode(String[] strings, String delim) {
      if (strings.length == 0) {
        return "";
      }
      if (strings.length == 1) {
        return strings[0];
      }
      StringBuilder sb = new StringBuilder();
      boolean first = true;
      for (String s : strings) {
        if (!first) {
          sb.append(delim);
        }
        sb.append(s);
        first = false;
      }
      return sb.toString();
    }

    private static String CombineAddresses(Message msg, String headerName) {
      List<NamedAddress> addresses = msg.GetAddresses(headerName);
      HeaderEncoder encoder = new HeaderEncoder().AppendFieldName(headerName);
      boolean first = true;
      for (NamedAddress addr : addresses) {
        if (!first) {
 encoder.AppendSymbol(",");
}
        addr.AppendThisAddress(encoder);
        first = false;
      }
      String ret = encoder.toString();
      // Strip out header field name, colon, and space
      return ret.substring(headerName.length() + 2);
    }

    public static String MessageToMailtoUri(Message msg) {
      if (msg == null) {
        throw new NullPointerException("msg");
      }
      List<NamedAddress> addresses = msg.GetAddresses("to");
      StringBuilder sb = new StringBuilder();
      sb.append("mailto:");
      boolean hasGroupOrDisplay = false;
      for (NamedAddress addr : addresses) {
        if (addr.isGroup() || !((addr.getDisplayName()) == null || (addr.getDisplayName()).length() == 0)) {
          hasGroupOrDisplay = true;
          break;
        }
      }
      String field = null;
      boolean firstField = true;
      if (!hasGroupOrDisplay) {
        boolean first = true;
        for (NamedAddress addr : addresses) {
          if (!first) {
            {
              first = false;
            }
            sb.append(",");
          }
          Address address = addr.getAddress();
          sb.append(URIUtility.EncodeStringForURI(address.getLocalPart()));
          sb.append("@");
          sb.append(URIUtility.EncodeStringForURI(address.getDomain()));
        }
      } else {
        field = CombineAddresses(msg, "to");
      }
      firstField = true;
      if (!((field) == null || (field).length() == 0)) {
        sb.append(firstField ? "?to=" : "&to=");
        firstField = false;
        sb.append(URIUtility.EncodeStringForURI(field));
      }
      field = Implode(msg.GetHeaderArray("subject"), "\n ");
      if (!((field) == null || (field).length() == 0)) {
        sb.append(firstField ? "?subject=" : "&subject=");
        firstField = false;
        sb.append(URIUtility.EncodeStringForURI(field));
      }
      field = Implode(msg.GetHeaderArray("in-reply-to"), "\n ");
      if (!((field) == null || (field).length() == 0)) {
        sb.append(firstField ? "?in-reply-to=" : "&in-reply-to=");
        firstField = false;
        sb.append(URIUtility.EncodeStringForURI(field));
      }
      field = CombineAddresses(msg, "cc");
      if (!((field) == null || (field).length() == 0)) {
        sb.append(firstField ? "?cc=" : "&cc=");
        firstField = false;
        sb.append(URIUtility.EncodeStringForURI(field));
      }
      field = CombineAddresses(msg, "bcc");
      if (!((field) == null || (field).length() == 0)) {
        sb.append(firstField ? "?bcc=" : "&bcc=");
        firstField = false;
        sb.append(URIUtility.EncodeStringForURI(field));
      }
      String[] fields = msg.GetHeaderArray("keywords");
      for (String field2 : fields) {
        if (!((field) == null || (field).length() == 0)) {
          sb.append(firstField ? "?keywords=" : "&keywords=");
          firstField = false;
          sb.append(URIUtility.EncodeStringForURI(field));
        }
      }
      fields = msg.GetHeaderArray("comments");
      for (String field2 : fields) {
        if (!((field) == null || (field).length() == 0)) {
          sb.append(firstField ? "?comments=" : "&comments=");
          firstField = false;
          sb.append(URIUtility.EncodeStringForURI(field));
        }
      }
      if (msg.getContentType().isText() &&
          !((msg.getContentType().GetCharset()) == null || (msg.getContentType().GetCharset()).length() == 0)) {
        field = msg.getBodyString();
        if (!((field) == null || (field).length() == 0)) {
          sb.append(firstField ? "?body=" : "&body=");
          firstField = false;
          sb.append(URIUtility.EncodeStringForURI(field));
        }
      }
      return sb.toString();
    }

    public static Message MailtoUriMessage(String uri) {
      String[] parts = URIUtility.SplitIRIToStrings(
        uri);
      if (parts == null || parts[0] == null) {
        return null;
      }

      if (parts[0].equals("mailto")) {
        Message msg = new Message();
        String emails = "";
        if (!((parts[2]) == null || (parts[2]).length() == 0)) {
          // Extract the email address
          emails = URIUtility.PercentDecode(parts[2]);
          if (HeaderParser2.ParseHeaderEmail(emails, 0, emails.length()) !=
                  emails.length()) {
            return null;
          }
        }
        if (parts[3] != null) {
          // If query String is present it must not be empty
          String query = parts[3];
          if (query.length() == 0) {
            return null;
          }
          int index = 0;
          while (index < query.length()) {
            int startName = index;
            int endName = -1;
            int startValue = -1;
            int endValue = -1;
            while (index < query.length()) {
              if (query.charAt(index) == '&') {
                if (endName < 0) {
                  return null;
                }
                endValue = index;
                ++index;
                break;
              } else if (query.charAt(index) == '=') {
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
              endValue = query.length();
            }
            String name = query.substring(startName, (startName)+(endName - startName));
            String value = query.substring(startValue, (startValue)+(endValue - startValue));
            name =
     DataUtilities.ToLowerCaseAscii(URIUtility.PercentDecode(name));
            value = URIUtility.PercentDecode(value);
            // Support only To, Cc, Bcc, Subject, In-Reply-To,
            // Keywords, Comments, and Body.
            // Of these, the first four can appear only once in a message
            if (name.equals("body")) {
              msg.SetTextBody(value);
            } else if (name.equals("keywords") || name.equals("comments")) {
              String decoded = Message.DecodeHeaderValue(name, value);
              msg.AddHeader(name, decoded);
            } else if (name.equals("subject") ||
name.equals("cc") ||
name.equals("bcc") ||
name.equals("in-reply-to")) {
              String decoded = Message.DecodeHeaderValue(name, value);
              msg.SetHeader(name, decoded);
            } else if (name.equals("to")) {
              String decoded = Message.DecodeHeaderValue(name, value);
              if (((emails) == null || (emails).length() == 0)) {
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
        if (!((emails) == null || (emails).length() == 0)) {
          if (emails.indexOf('(') >= 0) {
            // Contains opening parenthesis, a comment delimiter
            return null;
          }
          try {
            msg.SetHeader("to", emails);
          } catch (Exception ex) {
            return null;
          }
        }
        return msg;
      }
      return null;
    }
  }
