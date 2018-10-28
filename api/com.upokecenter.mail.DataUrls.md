# com.upokecenter.mail.DataUrls

    @Deprecated public final class DataUrls extends Object

Deprecated.
<div class='deprecationComment'>Renamed to DataUris.</div>

## Methods

* `static byte[] DataUrlBytes​(String url)`<br>
 Deprecated.
Renamed to DataUriBytes.
 Renamed to DataUriBytes.
* `static MediaType DataUrlMediaType​(String url)`<br>
 Deprecated.
Renamed to DataUriMediaType.
 Renamed to DataUriMediaType.
* `static String MakeDataUrl​(byte[] bytes,
           MediaType mediaType)`<br>
 Deprecated.
Renamed to MakeDataUri.
 Renamed to MakeDataUri.
* `static String MakeDataUrl​(String textString)`<br>
 Deprecated.
Renamed to MakeDataUri.
 Renamed to MakeDataUri.

## Method Details

### DataUrlMediaType
    @Deprecated public static MediaType DataUrlMediaType​(String url)
Deprecated.
<div class='deprecationComment'>Renamed to DataUriMediaType.</div>

**Parameters:**

* <code>url</code> - A data URI.

**Returns:**

* The media type. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a Data URI.

### DataUrlBytes
    @Deprecated public static byte[] DataUrlBytes​(String url)
Deprecated.
<div class='deprecationComment'>Renamed to DataUriBytes.</div>

**Parameters:**

* <code>url</code> - A data URI.

**Returns:**

* The data as a byte array. Returns null if <code>url</code> is null, is
 syntactically invalid, or is not a data URI.

### MakeDataUrl
    @Deprecated public static String MakeDataUrl​(String textString)
Deprecated.
<div class='deprecationComment'>Renamed to MakeDataUri.</div>

**Parameters:**

* <code>textString</code> - A text string to encode as a data URI.

**Returns:**

* A Data URI that encodes the given text.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>textString</code> is
 null.

### MakeDataUrl
    @Deprecated public static String MakeDataUrl​(byte[] bytes, MediaType mediaType)
Deprecated.
<div class='deprecationComment'>Renamed to MakeDataUri.</div>

**Parameters:**

* <code>bytes</code> - A byte array containing the data to encode in a Data URI.

* <code>mediaType</code> - A media type to assign to the data.

**Returns:**

* A Data URI that encodes the given data and media type.

**Throws:**

* <code>NullPointerException</code> - The parameter <code>bytes</code> or <code>
 mediaType</code> is null.
