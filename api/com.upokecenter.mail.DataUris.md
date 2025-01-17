# com.upokecenter.mail.DataUris

    public final class DataUris extends Object

<p>Contains methods for parsing and generating Data URIs (uniform resource
 identifiers). Data URIs are described in RFC 2397. Examples for Data URIs
 follow. </p><pre>data:, hello%20world</pre> <pre>data:text/markdown,
 hello%20world</pre> <pre>data:application/octet-stream;base64,
 AAAAAA==</pre>.

## Methods

* `static byte[] DataUriBytes(String uri)`<br>
 Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.

* `static byte[] DataUriBytes(URI uri)`<br>
 Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array, where the Data URI is given as a URI object.

* `static MediaType DataUriMediaType(String uri)`<br>
 Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a text string.

* `static MediaType DataUriMediaType(URI uri)`<br>
 Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a URI object.

* `static String MakeDataUri(byte[] bytes,
 MediaType mediaType)`<br>
 Encodes data with the given media type in a Data URI (uniform resource
 identifier).

* `static String MakeDataUri(String textString)`<br>
 Encodes text as a Data URI (uniform resource identifier).

## Method Details

### DataUriMediaType

    public static MediaType DataUriMediaType(String uri)

Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a text string.

**Parameters:**

* <code>uri</code> - A data URI in the form of a text string.

**Returns:**

* The media type. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a Data URI.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>uri</code> is null.

### DataUriMediaType

    public static MediaType DataUriMediaType(URI uri)

Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a URI object.

**Parameters:**

* <code>uri</code> - A data URI in the form of a URI object.

**Returns:**

* The media type. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a Data URI.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>uri</code> is null.

### DataUriBytes

    public static byte[] DataUriBytes(URI uri)

Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array, where the Data URI is given as a URI object.

**Parameters:**

* <code>uri</code> - The Data URI in the form of a URI object.

**Returns:**

* The data as a byte array. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a data URI.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>uri</code> is null.

### DataUriBytes

    public static byte[] DataUriBytes(String uri)

Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.

**Parameters:**

* <code>uri</code> - The parameter <code>uri</code> is a text string.

**Returns:**

* The data as a byte array. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a data URI.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>uri</code> is null.

### MakeDataUri

    public static String MakeDataUri(String textString)

Encodes text as a Data URI (uniform resource identifier).

**Parameters:**

* <code>textString</code> - A text string to encode as a data URI.

**Returns:**

* A Data URI that encodes the given text.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>textString</code> is null.

### MakeDataUri

    public static String MakeDataUri(byte[] bytes, MediaType mediaType)

Encodes data with the given media type in a Data URI (uniform resource
 identifier).

**Parameters:**

* <code>bytes</code> - A byte array containing the data to encode in a Data URI.

* <code>mediaType</code> - A media type to assign to the data.

**Returns:**

* A Data URI that encodes the given data and media type.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> or <code>
 mediaType</code> is null.
