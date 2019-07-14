using System;
using System.Text;
using PeterO;

namespace PeterO.Mail {
    /// <summary>
    ///  Contains methods for parsing and generating Data URIs
    /// (uniform // /resource identifiers). Data URIs are
    /// described in RFC 2397. Examples for Data URIs follow.
    /// <code>data:, hello%20world</code>
    /// <code>data:text/markdown,
    /// ///hello%20world</code>
    /// <code>data:application/octet-stream;base64,
    /// ///AAAAAA==</code>
    ///  .
    /// </summary>
  [Obsolete("Renamed to DataUris.")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design",
    "CA1054",
    Justification="This API is obsolete.")]
  public static class DataUrls {
    /// <summary>Extracts the media type from a Data URI (uniform resource
    /// identifier).</summary>
    /// <param name='url'>A data URI.</param>
    /// <returns>The media type. Returns null if <paramref name='url'/> is
    /// null, is syntactically invalid, or is not a Data URI.</returns>
    [Obsolete("Renamed to DataUriMediaType.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1054",
      Justification="This API is obsolete.")]
    public static MediaType DataUrlMediaType(string url) {
return DataUris.DataUriMediaType(url);
    }

    /// <summary>Extracts the data from a Data URI (uniform resource
    /// identifier) in the form of a byte array.</summary>
    /// <param name='url'>A data URI.</param>
    /// <returns>The data as a byte array. Returns null if <paramref
    /// name='url'/> is null, is syntactically invalid, or is not a data
    /// URI.</returns>
    [Obsolete("Renamed to DataUriBytes.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1054",
      Justification="This API is obsolete.")]
    public static byte[] DataUrlBytes(string url) {
return DataUris.DataUriBytes(url);
    }

    /// <summary>Encodes text as a Data URI (uniform resource
    /// identifier).</summary>
    /// <param name='textString'>A text string to encode as a data
    /// URI.</param>
    /// <returns>A Data URI that encodes the given text.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='textString'/> is null.</exception>
    [Obsolete("Renamed to MakeDataUri.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1054",
      Justification="This API is obsolete.")]
    public static string MakeDataUrl(string textString) {
return DataUris.MakeDataUri(textString);
    }

    /// <summary>Encodes data with the given media type in a Data URI
    /// (uniform resource identifier).</summary>
    /// <param name='bytes'>A byte array containing the data to encode in a
    /// Data URI.</param>
    /// <param name='mediaType'>A media type to assign to the data.</param>
    /// <returns>A Data URI that encodes the given data and media
    /// type.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> or <paramref name='mediaType'/> is null.</exception>
    [Obsolete("Renamed to MakeDataUri.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1054",
      Justification="This API is obsolete.")]
    public static string MakeDataUrl(byte[] bytes, MediaType mediaType) {
return DataUris.MakeDataUri(bytes, mediaType);
  }
 }
}
