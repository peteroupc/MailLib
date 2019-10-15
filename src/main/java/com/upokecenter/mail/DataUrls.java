package com.upokecenter.mail;

import com.upokecenter.util.*;

  /**
   * Contains methods for parsing and generating Data URIs (uniform resource
   * identifiers). Data URIs are described in RFC 2397. Examples for Data
   * URIs follow. <pre>data:, hello%20world</pre> <pre>data:text/markdown, hello%20world</pre> <pre>data:application/octet-stream;base64, AAAAAA==</pre>.
   * @deprecated Renamed to DataUris.
 */
@Deprecated

  public final class DataUrls {
private DataUrls() {
}
    /**
     * Extracts the media type from a Data URI (uniform resource identifier).
     * @param url A data URI.
     * @return The media type. Returns null if {@code url} is null, is
     * syntactically invalid, or is not a Data URI.
     * @deprecated Renamed to DataUriMediaType.
 */
@Deprecated

    public static MediaType DataUrlMediaType(String url) {
      return DataUris.DataUriMediaType(url);
    }

    /**
     * Extracts the data from a Data URI (uniform resource identifier) in the form
     * of a byte array.
     * @param url A data URI.
     * @return The data as a byte array. Returns null if {@code url} is null, is
     * syntactically invalid, or is not a data URI.
     * @deprecated Renamed to DataUriBytes.
 */
@Deprecated

    public static byte[] DataUrlBytes(String url) {
      return DataUris.DataUriBytes(url);
    }

    /**
     * Encodes text as a Data URI (uniform resource identifier).
     * @param textString A text string to encode as a data URI.
     * @return A Data URI that encodes the given text.
     * @throws NullPointerException The parameter {@code textString} is null.
     * @deprecated Renamed to MakeDataUri.
 */
@Deprecated

    public static String MakeDataUrl(String textString) {
      return DataUris.MakeDataUri(textString);
    }

    /**
     * Encodes data with the given media type in a Data URI (uniform resource
     * identifier).
     * @param bytes A byte array containing the data to encode in a Data URI.
     * @param mediaType A media type to assign to the data.
     * @return A Data URI that encodes the given data and media type.
     * @throws NullPointerException The parameter {@code bytes} or {@code
     * mediaType} is null.
     * @deprecated Renamed to MakeDataUri.
 */
@Deprecated

    public static String MakeDataUrl(byte[] bytes, MediaType mediaType) {
      return DataUris.MakeDataUri(bytes, mediaType);
    }
  }
