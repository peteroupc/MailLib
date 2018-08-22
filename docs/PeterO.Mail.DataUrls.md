## PeterO.Mail.DataUrls

    public static class DataUrls

Contains methods for parsing and generating data URLs. Data URLs are described in RFC 2397. Examples for data URLs follow.

    data:,hello%20world

    data:text/markdown,hello%20world

    data:application/octet-stream;base64,AAAAAA==

### DataUrlBytes

    public static byte[] DataUrlBytes(
        string url);

Extracts the data from a data URL in the form of a byte array.

<b>Parameters:</b>

 * <i>url</i>: A data URL string.

<b>Return Value:</b>

The data as a byte array. Returns null if  <i>url</i>
 is null, is syntactically invalid, or is not a data URL.

### DataUrlMediaType

    public static PeterO.Mail.MediaType DataUrlMediaType(
        string url);

Extracts the media type from a data URL.

<b>Parameters:</b>

 * <i>url</i>: A data URL string.

<b>Return Value:</b>

The media type. Returns null if  <i>url</i>
 is null, is syntactically invalid, or is not a data URL.

### MakeDataUrl

    public static string MakeDataUrl(
        byte[] bytes,
        PeterO.Mail.MediaType mediaType);

Encodes data with the given media type in a data URL.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array containing the data to encode in a data URL.

 * <i>mediaType</i>: A media type to assign to the data.

<b>Return Value:</b>

A data URL that encodes the given data and media type.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>bytes</i>
 or  <i>mediaType</i>
 is null.

### MakeDataUrl

    public static string MakeDataUrl(
        string textString);

Encodes text as a data URL.

<b>Parameters:</b>

 * <i>textString</i>: A text string to encode as a data URL.

<b>Return Value:</b>

A data URL that encodes the given text.

<b>Exceptions:</b>

 * System.ArgumentNullException:
The parameter  <i>textString</i>
 is null.
