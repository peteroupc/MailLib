## PeterO.Mail.DataUris

    public static class DataUris

Contains methods for parsing and generating Data URIs (uniform resource identifiers). Data URIs are described in RFC 2397. Examples for Data URIs follow.

    data:,hello%20world

    data:text/markdown,hello%20world

    data:application/octet-stream;base64,AAAAAA==

### DataUriBytes

    public static byte[] DataUriBytes(
        string uri);

Extracts the data from a Data URI (uniform resource identifier) in the form of a byte array.

<b>Parameters:</b>

 * <i>uri</i>: The parameter  <i>uri</i>
 is a text string.

<b>Return Value:</b>

The data as a byte array. Returns null if  <i>uri</i>
 is null, is syntactically invalid, or is not a data URI.

### DataUriMediaType

    public static PeterO.Mail.MediaType DataUriMediaType(
        string uri);

Extracts the media type from a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>uri</i>: The parameter  <i>uri</i>
 is a text string.

<b>Return Value:</b>

The media type. Returns null if  <i>uri</i>
 is null, is syntactically invalid, or is not a Data URI.

### MakeDataUri

    public static string MakeDataUri(
        byte[] bytes,
        PeterO.Mail.MediaType mediaType);

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

### MakeDataUri

    public static string MakeDataUri(
        string textString);

Encodes text as a Data URI (uniform resource identifier).

<b>Parameters:</b>

 * <i>textString</i>: A text string to encode as a data URI.

<b>Return Value:</b>

A Data URI that encodes the given text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>textString</i>
 is null.
