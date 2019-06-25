namespace PeterO.Text {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Text.Normalization"]/*'/>
  public enum Normalization {
    /// <summary>Normalization form C: canonical decomposition followed by canonical
    /// composition.</summary>
    NFC,

    /// <summary>Normalization form D: canonical decomposition.</summary>
    NFD,

    /// <summary>Normalization form KC: compatibility decomposition followed by canonical
    /// composition.</summary>
    NFKC,

    /// <summary>Normalization form KD: compatibility decomposition.</summary>
    NFKD,
  }
}
