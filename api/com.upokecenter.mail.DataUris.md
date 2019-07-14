# com.upokecenter.mail.DataUris

    public final class DataUris extends java.lang.Object

Contains methods for parsing and generating Data URIs (uniform // /resource
 identifiers). Data URIs are described in RFC 2397. Examples for Data
 URIs follow. <pre>data:, hello%20world</pre>
 <pre>data:text/markdown, hello%20world</pre>
 <pre>data:application/octet-stream;base64, AAAAAA==</pre> .

## Methods

* `static byte[] DataUriBytes​(java.lang.String uri)`<br>
 Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.
* `static MediaType DataUriMediaType​(java.lang.String uri)`<br>
 Extracts the media type from a Data URI (uniform resource identifier).
* `static java.lang.String MakeDataUri​(byte[] bytes,
           MediaType mediaType)`<br>
 Encodes data with the given media type in a Data URI (uniform resource
 identifier).
* `static java.lang.String MakeDataUri​(java.lang.String textString)`<br>
 Encodes text as a Data URI (uniform resource identifier).

## Method Details

### DataUriMediaType
    public static MediaType DataUriMediaType​(java.lang.String uri)
Extracts the media type from a Data URI (uniform resource identifier).

**Parameters:**

* <code>uri</code> - The parameter <code>uri</code> is a text string.

**Returns:**

* The media type. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a Data URI.

### DataUriBytes
    public static byte[] DataUriBytes​(java.lang.String uri)
Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.

**Parameters:**

* <code>uri</code> - The parameter <code>uri</code> is a text string.

**Returns:**

* The data as a byte array. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a data URI.

### MakeDataUri
    public static java.lang.String MakeDataUri​(java.lang.String textString)
Encodes text as a Data URI (uniform resource identifier).

**Parameters:**

* <code>textString</code> - A text string to encode as a data URI.

**Returns:**

* A Data URI that encodes the given text.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>textString</code> is null.

### MakeDataUri
    public static java.lang.String MakeDataUri​(byte[] bytes, MediaType mediaType)
Encodes data with the given media type in a Data URI (uniform resource
 identifier).

**Parameters:**

* <code>bytes</code> - A byte array containing the data to encode in a Data URI.

* <code>mediaType</code> - A media type to assign to the data.

**Returns:**

* A Data URI that encodes the given data and media type.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>bytes</code> or <code>
 mediaType</code> is null.
