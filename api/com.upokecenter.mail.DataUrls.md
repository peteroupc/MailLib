# com.upokecenter.mail.DataUrls

    public final class DataUrls extends Object

Contains methods for parsing and generating data URLs. Data URLs are
 described in RFC 2397. Examples for data URLs follow.
 <pre>data:, hello%20world </pre>
 <pre>data:text/markdown, hello%20world </pre>
 <pre>data:application/octet-stream;base64, AAAAAA== </pre>

## Methods

* `static byte[] DataUrlBytes​(String url)`<br>
 Extracts the data from a data URL in the form of a byte array.
* `static MediaType DataUrlMediaType​(String url)`<br>
 Extracts the media type from a data URL.
* `static String MakeDataUrl​(byte[] bytes,
           MediaType mediaType)`<br>
 Encodes data with the given media type in a data URL.
* `static String MakeDataUrl​(String textString)`<br>
 Encodes text as a data URL.

## Method Details

### DataUrlMediaType
    public static MediaType DataUrlMediaType​(String url)
Extracts the media type from a data URL.

**Parameters:**

* <code>url</code> - A data URL string.

**Returns:**

* The media type. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a data URL.

### DataUrlBytes
    public static byte[] DataUrlBytes​(String url)
Extracts the data from a data URL in the form of a byte array.

**Parameters:**

* <code>url</code> - A data URL string.

**Returns:**

* The data as a byte array. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a data URL.

### MakeDataUrl
    public static String MakeDataUrl​(String textString)
Encodes text as a data URL.

**Parameters:**

* <code>textString</code> - A text string to encode as a data URL.

**Returns:**

* A data URL that encodes the given text.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>textString</code> is
 null.

### MakeDataUrl
    public static String MakeDataUrl​(byte[] bytes, MediaType mediaType)
Encodes data with the given media type in a data URL.

**Parameters:**

* <code>bytes</code> - A byte array containing the data to encode in a data URL.

* <code>mediaType</code> - A media type to assign to the data.

**Returns:**

* A data URL that encodes the given data and media type.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> or <code>
 mediaType</code> is null.
