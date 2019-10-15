package com.upokecenter.text;

  /**
   * Represents a Unicode normalization form.
   */
  public enum Normalization {
    /**
     * Normalization form C: canonical decomposition followed by canonical
     * composition.
     */
    NFC,

    /**
     * Normalization form D: canonical decomposition.
     */
    NFD,

    /**
     * Normalization form KC: compatibility decomposition followed by canonical
     * composition.
     */
    NFKC,

    /**
     * Normalization form KD: compatibility decomposition.
     */
    NFKD,
  }
