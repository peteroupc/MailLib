# com.upokecenter.mail.DataUrls

## Methods

* `static byte[] DataUrlBytes​(java.lang.String url)`<br>
 Deprecated.
Renamed to DataUriBytes.
 Renamed to DataUriBytes.
* `static MediaType DataUrlMediaType​(java.lang.String url)`<br>
 Deprecated.
Renamed to DataUriMediaType.
 Renamed to DataUriMediaType.
* `static java.lang.String MakeDataUrl​(byte[] bytes,
MediaType mediaType)`<br>
 Deprecated.
Renamed to MakeDataUri.
 Renamed to MakeDataUri.
* `static java.lang.String MakeDataUrl​(java.lang.String textString)`<br>
 Deprecated.
Renamed to MakeDataUri.
 Renamed to MakeDataUri.

## Method Details

### <a id='DataUrlMediaType(java.lang.String)'>DataUrlMediaType</a>

Extracts the media type from a Data URI (uniform resource identifier).

**Parameters:**

* <code>url</code> - A data URI.

**Returns:**

* The media type. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a Data URI.

### <a id='DataUrlBytes(java.lang.String)'>DataUrlBytes</a>

Extracts the data from a Data URI (uniform resource identifier) in the form
 of a byte array.

**Parameters:**

* <code>url</code> - A data URI.

**Returns:**

* The data as a byte array. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a data URI.

### <a id='MakeDataUrl(java.lang.String)'>MakeDataUrl</a>

Encodes text as a Data URI (uniform resource identifier).

**Parameters:**

* <code>textString</code> - A text string to encode as a data URI.

**Returns:**

* A Data URI that encodes the given text.

**Throws:**

* <code>java.lang.NullPointerException</code> - The parameter <code>textString</code> is null.

### <a id='MakeDataUrl(byte[],com.upokecenter.mail.MediaType)'>MakeDataUrl</a>

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
