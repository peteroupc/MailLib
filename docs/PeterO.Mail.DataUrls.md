## PeterO.Mail.DataUrls

    public static class DataUrls

<b>Deprecated.</b> Renamed to DataUris.

Contains methods for parsing and generating Data URIs (uniform resource identifiers). Data URIs are described in RFC 2397. Examples for Data URIs follow.

    data:,hello%20world

    data:text/markdown,hello%20world

    data:application/octet-stream;base64,AAAAAA==

### DataUrlBytes

    public static byte[] DataUrlBytes(
        string url);

<b>Deprecated.</b> Renamed to DataUriBytes.

Extracts the data from a Data URI (uniform resource identifier) in the form of a byte array.

<b>Parameters:</b>

 * <i>url</i>: The parameter  <i>url</i>
 is not documented yet.

<b>Return Value:</b>

The data as a byte array. Returns null if  <i>uri</i>
 is null, is syntactically invalid, or is not a data URI.

### DataUrlMediaType

    public static PeterO.Mail.MediaType DataUrlMediaType(
        string url);

<b>Deprecated.</b> Renamed to DataUriMediaType.

Extracts the media type from a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>url</i>: The parameter  <i>url</i>
 is not documented yet.

<b>Return Value:</b>

The media type. Returns null if  <i>uri</i>
 is null, is syntactically invalid, or is not a Data URI.

### MakeDataUrl

    public static string MakeDataUrl(
        byte[] bytes,
        PeterO.Mail.MediaType mediaType);

<b>Deprecated.</b> Renamed to MakeDataUri.

Encodes data with the given media type in a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>bytes</i>: A byte array containing the data to encode in a Data URI.

 * <i>mediaType</i>: A media type to assign to the data.

<b>Return Value:</b>

A Data URI that encodes the given data and media type.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 or  <i>mediaType</i>
 is null.

### MakeDataUrl

    public static string MakeDataUrl(
        string textString);

<b>Deprecated.</b> Renamed to MakeDataUri.

Encodes text as a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>textString</i>: A text string to encode as a data URI.

<b>Return Value:</b>

A Data URI that encodes the given text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>textString</i>
 is null.
