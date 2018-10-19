using System;
using System.Text;
using PeterO;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="T:PeterO.Mail.DataUrls"]/*'/>
  [Obsolete("Renamed to DataUris.")]
  public static class DataUrls {
    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Mail.DataUrls.DataUrlMediaType(System.String)"]/*'/>
  [Obsolete("Renamed to DataUriMediaType.")]
    public static MediaType DataUrlMediaType(string url) {
return DataUris.DataUriMediaType(url);
    }

    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Mail.DataUrls.DataUrlBytes(System.String)"]/*'/>
  [Obsolete("Renamed to DataUriBytes.")]
    public static byte[] DataUrlBytes(string url) {
return DataUris.DataUriBytes(url);
    }

    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Mail.DataUrls.MakeDataUrl(System.String)"]/*'/>
  [Obsolete("Renamed to MakeDataUri.")]
    public static string MakeDataUrl(string textString) {
return DataUris.MakeDataUri(textString);
    }

    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="M:PeterO.Mail.DataUrls.MakeDataUrl(System.Byte[],PeterO.Mail.MediaType)"]/*'/>
  [Obsolete("Renamed to MakeDataUri.")]
    public static string MakeDataUrl(byte[] bytes, MediaType mediaType) {
return DataUris.MakeDataUri(bytes, mediaType);
  }
 }
}
