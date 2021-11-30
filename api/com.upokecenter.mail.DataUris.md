# com.upokecenter.mail.DataUris

## Methods

* `static byte[] DataUriBytes​(java.lang.String uri)`<br>
 Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.
* `static byte[] DataUriBytes​(java.net.URI uri)`<br>
 Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array, where the Data URI is given as a URI object.
* `static MediaType DataUriMediaType​(java.lang.String uri)`<br>
 Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a text string.
* `static MediaType DataUriMediaType​(java.net.URI uri)`<br>
 Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a URI object.
* `static java.lang.String MakeDataUri​(byte[] bytes,
MediaType mediaType)`<br>
 Encodes data with the given media type in a Data URI (uniform resource
 identifier).
* `static java.lang.String MakeDataUri​(java.lang.String textString)`<br>
 Encodes text as a Data URI (uniform resource identifier).

## Method Details

### <a id='DataUriMediaType(java.lang.String)'>DataUriMediaType</a>

Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a text string.

**Parameters:**

* <code>uri</code> - A data URI in the form of a text string.

**Returns:**

* The media type. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a Data URI.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>uri</code> is null.

### <a id='DataUriMediaType(java.net.URI)'>DataUriMediaType</a>

Extracts the media type from a Data URI (uniform resource identifier) in the
 form of a URI object.

**Parameters:**

* <code>uri</code> - A data URI in the form of a URI object.

**Returns:**

* The media type. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a Data URI.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>uri</code> is null.

### <a id='DataUriBytes(java.net.URI)'>DataUriBytes</a>

Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array, where the Data URI is given as a URI object.

**Parameters:**

* <code>uri</code> - The Data URI in the form of a URI object.

**Returns:**

* The data as a byte array. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a data URI.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>uri</code> is null.

### <a id='DataUriBytes(java.lang.String)'>DataUriBytes</a>

Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.

**Parameters:**

* <code>uri</code> - The parameter <code>uri</code> is a text string.

**Returns:**

* The data as a byte array. Returns null if <code>uri</code> is null, is
 syntactically invalid, or is not a data URI.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>uri</code> is null.

### <a id='MakeDataUri(java.lang.String)'>MakeDataUri</a>

Encodes text as a Data URI (uniform resource identifier).

**Parameters:**

* <code>textString</code> - A text string to encode as a data URI.

**Returns:**

* A Data URI that encodes the given text.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>textString</code> is null.

### <a id='MakeDataUri(byte[],com.upokecenter.mail.MediaType)'>MakeDataUri</a>

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
