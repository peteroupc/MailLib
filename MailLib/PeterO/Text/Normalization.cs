namespace PeterO.Text {
  /// <summary>Represents a Unicode normalization form.</summary>
  public enum Normalization {
    /// <summary>Normalization form C: canonical decomposition followed by
    /// canonical composition.</summary>
    NFC,

    /// <summary>Normalization form D: canonical decomposition.</summary>
    NFD,

    /// <summary>Normalization form KC: compatibility decomposition
    /// followed by canonical composition.</summary>
    NFKC,

    /// <summary>Normalization form KD: compatibility
    /// decomposition.</summary>
    NFKD,
  }
}
