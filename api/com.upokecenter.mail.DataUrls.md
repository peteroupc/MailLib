# com.upokecenter.mail.DataUrls

    @Deprecated public final class DataUrls extends Object

<p>Contains methods for parsing and generating Data URIs (uniform resource
 identifiers). Data URIs are described in RFC 2397. Examples for Data URIs
 follow. </p><pre>data:, hello%20world</pre> <pre>data:text/markdown,
 hello%20world</pre> <pre>data:application/octet-stream;base64,
 AAAAAA==</pre>.

## Methods

* `static byte[] DataUrlBytes(String url)`<br>
 Deprecated.
Renamed to DataUriBytes.

* `static MediaType DataUrlMediaType(String url)`<br>
 Deprecated.
Renamed to DataUriMediaType.

* `static String MakeDataUrl(byte[] bytes,
 MediaType mediaType)`<br>
 Deprecated.
Renamed to MakeDataUri.

* `static String MakeDataUrl(String textString)`<br>
 Deprecated.
Renamed to MakeDataUri.

## Method Details

### DataUrlMediaType

    @Deprecated public static MediaType DataUrlMediaType(String url)

Extracts the media type from a Data URI (uniform resource identifier).

**Parameters:**

* <code>url</code> - A data URI.

**Returns:**

* The media type. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a Data URI.

### DataUrlBytes

    @Deprecated public static byte[] DataUrlBytes(String url)

Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.

**Parameters:**

* <code>url</code> - A data URI.

**Returns:**

* The data as a byte array. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a data URI.

### MakeDataUrl

    @Deprecated public static String MakeDataUrl(String textString)

Encodes text as a Data URI (uniform resource identifier).

**Parameters:**

* <code>textString</code> - A text string to encode as a data URI.

**Returns:**

* A Data URI that encodes the specified text.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>textString</code> is null.

### MakeDataUrl

    @Deprecated public static String MakeDataUrl(byte[] bytes, MediaType mediaType)

Encodes data with the specified media type in a Data URI (uniform resource
 identifier).

**Parameters:**

* <code>bytes</code> - A byte array containing the data to encode in a Data URI.

* <code>mediaType</code> - A media type to assign to the data.

**Returns:**

* A Data URI that encodes the specified data and media type.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> or <code>
 mediaType</code> is null.
