## PeterO.Mail.DataUrls

    public static class DataUrls

<b>Obsolete.</b> Renamed to DataUris.

 Contains methods for parsing and generating Data URIs (uniform resource identifiers). Data URIs are described in RFC 2397. Examples for Data URIs follow.

    data:, hello%20world

    data:text/markdown, hello%20world

    data:application/octet-stream;base64, AAAAAA==

 .

### Member Summary
* <code>[DataUrlBytes(string)](#DataUrlBytes_string)</code> - <b>Obsolete:</b> Renamed to DataUriBytes.
* <code>[DataUrlMediaType(string)](#DataUrlMediaType_string)</code> - <b>Obsolete:</b> Renamed to DataUriMediaType.
* <code>[MakeDataUrl(byte[], PeterO.Mail.MediaType)](#MakeDataUrl_byte_PeterO_Mail_MediaType)</code> - <b>Obsolete:</b> Renamed to MakeDataUri.
* <code>[MakeDataUrl(string)](#MakeDataUrl_string)</code> - <b>Obsolete:</b> Renamed to MakeDataUri.

<a id="DataUrlBytes_string"></a>
### DataUrlBytes

    public static byte[] DataUrlBytes(
        string url);

<b>Obsolete.</b> Renamed to DataUriBytes.

Extracts the data from a Data URI (uniform resource identifier) in the form of a byte array.

<b>Parameters:</b>

 * <i>url</i>: A data URI.

<b>Return Value:</b>

The data as a byte array. Returns null if  <i>url</i>
 is null, is syntactically invalid, or is not a data URI.

<a id="DataUrlMediaType_string"></a>
### DataUrlMediaType

    public static PeterO.Mail.MediaType DataUrlMediaType(
        string url);

<b>Obsolete.</b> Renamed to DataUriMediaType.

Extracts the media type from a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>url</i>: A data URI.

<b>Return Value:</b>

The media type. Returns null if  <i>url</i>
 is null, is syntactically invalid, or is not a Data URI.

<a id="MakeDataUrl_byte_PeterO_Mail_MediaType"></a>
### MakeDataUrl

    public static string MakeDataUrl(
        byte[] bytes,
        PeterO.Mail.MediaType mediaType);

<b>Obsolete.</b> Renamed to MakeDataUri.

Encodes data with the specified media type in a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>bytes</i>: A byte array containing the data to encode in a Data URI.

 * <i>mediaType</i>: A media type to assign to the data.

<b>Return Value:</b>

A Data URI that encodes the specified data and media type.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 or  <i>mediaType</i>
 is null.

<a id="MakeDataUrl_string"></a>
### MakeDataUrl

    public static string MakeDataUrl(
        string textString);

<b>Obsolete.</b> Renamed to MakeDataUri.

Encodes text as a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>textString</i>: A text string to encode as a data URI.

<b>Return Value:</b>

A Data URI that encodes the specified text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>textString</i>
 is null.
